using LYZD.DAL;
using LYZD.Utility.Log;
using LYZD.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.ViewModel.CheckInfo
{
    /// <summary>
    /// 按表位显示的详细结论
    /// </summary>
    public class AllMeterResult : ViewModelBase
    {
        private SelectCollection<OneMeterResult> resultCollection = new SelectCollection<OneMeterResult>();
        /// <summary>
        /// 所有检定结论
        /// </summary>
        public SelectCollection<OneMeterResult> ResultCollection
        {
            get { return resultCollection; }
            set { SetPropertyValue(value, ref resultCollection, "ResultCollection"); }
        }
        /// <summary>
        /// 加载临时库所有表信息
        /// </summary>
        public AllMeterResult()
        {
            ResultCollection.ItemsSource.Clear();
            for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
            {
                string meterPk = EquipmentData.MeterGroupInfo.Meters[i].GetProperty("METER_ID") as string;
                ResultCollection.ItemsSource.Add(new OneMeterResult(meterPk, true));
            }
        }
        /// <summary>
        /// 从正式库加载表信息
        /// </summary>
        /// <param name="meters"></param>
        public AllMeterResult(IEnumerable<DynamicViewModel> meters)
        {
            LoadMeters(meters);
        }

        public void LoadMeters(IEnumerable<DynamicViewModel> meters)
        {
            ResultCollection.ItemsSource.Clear();
            if (meters == null)
            {
                return;
            }
            for (int i = 0; i < meters.Count(); i++)
            {
                string meterPk = meters.ElementAt(i).GetProperty("METER_ID") as string;
                ResultCollection.ItemsSource.Add(new OneMeterResult(meterPk, false));
            }
        }
        /// <summary>
        /// 保存所有的信息
        /// </summary>
        public void SaveAllInfo()
        {
            #region 更新表信息
            bool[] yaojianTemp = EquipmentData.MeterGroupInfo.YaoJian;
            List<DynamicModel> meterModels = new List<DynamicModel>();
            for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
            {
                if (yaojianTemp[i])
                {
                    meterModels.Add(EquipmentData.MeterGroupInfo.Meters[i].GetDataSource());
                }
            }
            int updateCount = DALManager.MeterTempDbDal.Update("T_TMP_METER_INFO", "METER_ID", meterModels, new List<string>()
            {
                    "MD_TEMPERATURE",                                      //温度
                    "MD_HUMIDITY",                                             //湿度
                    "MD_SUPERVISOR",                                         //主管
                    "MD_TEST_PERSON",                                      //检验员
                    "MD_AUDIT_PERSON",                                      //核验员
                    "MD_RESULT",                           //总结论
                    "MD_VALID_DATE",                                         //有效期
                    "MD_TEST_DATE",
            });
            LogManager.AddMessage(string.Format("更新表温湿度,检验员及总结论,共更新{0}条记录", updateCount), EnumLogSource.数据库存取日志, EnumLevel.Information);
            #endregion
            #region 将临时库数据转存到正式库
            List<string> listWhere = new List<string>();
            List<string> listWhereTemp = new List<string>();

            for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
            {
                if (yaojianTemp[i])
                {
                    listWhere.Add(string.Format("METER_ID='{0}'", EquipmentData.MeterGroupInfo.Meters[i].GetProperty("METER_ID")));
                    listWhereTemp.Add(string.Format("METER_ID='{0}'", EquipmentData.MeterGroupInfo.Meters[i].GetProperty("METER_ID")));
                }
            }
            string wherePk = string.Join(" or ", listWhere);
            string whereFk = string.Join(" or ", listWhereTemp);

            List<string> tableNames = DALManager.MeterDbDal.GetTableNames();
            tableNames.Remove("METER_INFO");
            #region 先删除正式库中对应的检定信息
            List<string> deleteSqlList = new List<string>();
            deleteSqlList.Add(string.Format("delete from meter_info where {0}", wherePk));
            for (int i = 0; i < tableNames.Count; i++)
            {
                if (tableNames[i].Contains("~"))
                {
                    continue;
                }
                deleteSqlList.Add(string.Format("delete from {0} where {1}", tableNames[i], whereFk));
            }
            int deleteCount = DALManager.MeterDbDal.ExecuteOperation(deleteSqlList);
            LogManager.AddMessage(string.Format("删除正式库中过时数据,共删除{0}条", deleteCount), EnumLogSource.数据库存取日志, EnumLevel.Information);
            #endregion

            #region 插入临时库中的检定信息
            List<DynamicModel> metersTemp = DALManager.MeterTempDbDal.GetList("T_TMP_METER_INFO", wherePk);
            int insertCount = DALManager.MeterDbDal.Insert("METER_INFO", metersTemp);
            if (insertCount == 0)
            {
                LogManager.AddMessage("向正式库中添加表信息失败", EnumLogSource.数据库存取日志, EnumLevel.Error);
                return;
            }
            LogManager.AddMessage(string.Format("向正式库中添加表信息,共添加{0}条记录", insertCount), EnumLogSource.数据库存取日志, EnumLevel.Information);

            for (int i = 0; i < tableNames.Count; i++)
            {
                if (tableNames[i].Contains("~"))
                {
                    continue;
                }
                List<DynamicModel> modelsResult = DALManager.MeterTempDbDal.GetList("T_TMP_" + tableNames[i], whereFk);

                #region 存盘只存储当前检定方案的数据

                //筛选出当前方案的项目
                List<Schema.SchemaNodeViewModel> schemaNodeViewModel = EquipmentData.Schema.GetTerminalNodes();
                List<string> selectSchemaId = new List<string>();
                string id;
                for (int j = 0; j < schemaNodeViewModel.Count; j++)
                {
                    for (int z = 0; z < schemaNodeViewModel[j].ParaValuesCurrent.Count; z++)
                    {
                        id = schemaNodeViewModel[j].ParaValuesCurrent[z].GetProperty("PARA_VALUE_NO") as string;
                        if(id!=null && id!="") selectSchemaId.Add(id);
                    }
                }
                //将不是当前方案的项目全部删除
                List<DynamicModel> modelsResult2=new List<DynamicModel>();
                for (int j = modelsResult.Count-1; j >=0; j--)
                {
                    id = modelsResult[j].GetProperty("MD_PROJECT_NO") as string;
                    if (selectSchemaId.Contains(id))
                    {
                        modelsResult2.Add(modelsResult[j]);
                    }
                }
                #endregion

                if (modelsResult2.Count > 0)
                {
                    insertCount = DALManager.MeterDbDal.Insert(tableNames[i], modelsResult2);
                    LogManager.AddMessage(string.Format("向结论表:{0}添加检定结论,共添加{1}条记录", tableNames[i], insertCount), EnumLogSource.数据库存取日志, EnumLevel.Information);
                }
            }
            #endregion
            LogManager.AddMessage("将检定数据存储到正式数据库成功!", EnumLogSource.数据库存取日志, EnumLevel.Tip);

            #endregion
        }
    }
}
