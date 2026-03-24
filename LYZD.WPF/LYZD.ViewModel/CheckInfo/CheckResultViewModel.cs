using LYZD.DAL;
using LYZD.Utility;
using LYZD.Utility.Log;
using LYZD.ViewModel.CheckController;
using LYZD.ViewModel.Model;
using LYZD.ViewModel.Schema;
using LYZD.ViewModel.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.ViewModel.CheckInfo
{
    /// <summary>
    /// 检定结论视图模型
    /// </summary>
    public class CheckResultViewModel : ViewModelBase
    {
        public CheckResultViewModel()
        {
            DetailResultView.Clear();
            for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
            {
                DetailResultView.Add(new DynamicViewModel(i + 1));
            }
        }

        private CheckNodeViewModel checkNodeCurrent;
        /// <summary>
        /// 当前选中的检定点
        /// </summary>
        public CheckNodeViewModel CheckNodeCurrent
        {
            get { return checkNodeCurrent; }
            set
            {
                bool flagTemp = false;
                if (checkNodeCurrent == null || checkNodeCurrent.ParaNo != value.ParaNo)
                {
                    flagTemp = true;
                }
                FlagLoadColumn = true;
                SetPropertyValue(value, ref checkNodeCurrent, "CheckNodeCurrent");
                if (flagTemp)
                {
                    LoadViewColumn();
                }
                else
                {
                    RefreshDetailResult();
                }
                //DetailResultView
            }
        }



        
        private AsyncObservableCollection<CheckNodeViewModel> resultCollection = new AsyncObservableCollection<CheckNodeViewModel>();
        /// <summary>
        /// 检定结论集合
        /// </summary>
        public AsyncObservableCollection<CheckNodeViewModel> ResultCollection
        {
            get { return resultCollection; }
            set { resultCollection = value; }
        }
        #region 初始化检定结论
        /// 初始化检定结论
        /// <summary>
        /// 初始化检定结论
        /// </summary>
        /// <param name="schemaId">方案编号</param>
        public void InitialResult()
        {

            ResultCollection.Clear();
            Categories.Clear();

            for (int i = 0; i < EquipmentData.Schema.Children.Count; i++)
            {
                Categories.Add(GetResultNode(EquipmentData.Schema.Children[i]));
                Categories[Categories.Count - 1].NameFontSize = 12;

            }

            //GetInitialData();

            for (int i = 0; i < Categories.Count; i++)
            {
                for (int j = 0; j < Categories[i].Children.Count; j++)
                {
                    Categories[i].Children[j].CompressNode();
                }
            }
            #region 加载检定结论
            TaskManager.AddDataBaseAction(() =>
            {
                //TODO 这里有一个bug，上一次数据还没加载完成，就切换方案。导致这个线程还在执行，同时结论数据被清空，导致出现异常
                try
                {
                    for (int i = 0; i < ResultCollection.Count; i++)
                    {
                        CheckResultBll.Instance.LoadCheckResult(ResultCollection[i]);
                        ResultCollection[i].RefreshResultSummary();
                    }
                    for (int i = 0; i < Categories.Count; i++)
                    {
                        UpdateResultSummaryDown(Categories[i]);
                    }
                    //初始化时间统计
                    TimeMonitor.Instance.Initialize();
                }
                catch (Exception){ }

            });
            #endregion
        }

        /// <summary>
        /// 处理初始固有误差
        /// </summary>
        private void GetInitialData()
        {
            //最后创建的项目列表-
            Dictionary<string, List<CheckNodeViewModel>> list = new Dictionary<string, List<CheckNodeViewModel>>();
            Dictionary<string, List<CheckNodeViewModel>> list2 = new Dictionary<string, List<CheckNodeViewModel>>();


            //处理初始固有误差问题
            bool t = false;
            for (int i = 0; i < Categories.Count; i++)
            {
                for (int j = 0; j < Categories[i].Children.Count; j++)
                {
                    if (Categories[i].Children[j].ParaNo == Core.Enum.ProjectID.初始固有误差)
                    {
                        //低到高
                        for (int z = 0; z < Categories[i].Children[j].Children.Count; z++)   //初始固有误差的所有项目
                        {
                            CheckNodeViewModel data = Core.Function.CopyClassHelper.DeepCopyByBinary(Categories[i].Children[j].Children[z]);
                            string startStr = data.ItemKey.Substring(0, 8);  // 取到开始发字符
                            if (!list.ContainsKey(startStr))
                            {
                                list.Add(startStr, new List<CheckNodeViewModel>());
                            }
                            data.Name = data.Name+"_低到高";
                            list[startStr].Add(data);
                        }
                        //高到低
                        for (int z = 0; z < Categories[i].Children[j].Children.Count; z++)   //初始固有误差的所有项目
                        {
                            CheckNodeViewModel data = Core.Function.CopyClassHelper.DeepCopyByBinary(Categories[i].Children[j].Children[z]);
                            string startStr = data.ItemKey.Substring(0, 8);  // 取到开始发字符
                            if (!list2.ContainsKey(startStr))
                            {
                                list2.Add(startStr, new List<CheckNodeViewModel>());
                            }
                            data.Name = data.Name + "_高到低";
                            list2[startStr].Add(data);
                        }
                        //这里对list2重新排序
                        foreach (var item in list2.Keys)
                        {
                           var tem = list2[item].OrderByDescending(u => GetErrorSortString_Reversal(u.ItemKey) ).ToList(); //这样我们也把他进行了一次排序
                            list[item].AddRange(tem); ;
                        }
                        Categories[i].Children[j].Children.Clear();  //删除之前的去全部
                        foreach (var item in list.Keys)
                        {
                            for (int index = 0; index < list[item].Count; index++)
                            {
                                Categories[i].Children[j].Children.Add(list[item][index]);
                            }
                        }
                        t = true;
                        break;
                    }
                }
                if (t) break;
            }

        }
        /// <summary>
        /// 电流大到小的初始固有误差
        /// </summary>
        /// <param name="keyString"></param>
        /// <returns></returns>
        private int GetErrorSortString_Reversal(string keyString)
        {
            if (keyString == null)
            {
                return 1;
            }
            string[] arrayTemp = keyString.Split('_');

            //数据格式:功率方向|功率元件|功率因数|电流倍数     11106      11061  11062
            string strPara = arrayTemp[1];
            string currentString = strPara.Substring(3, 2);
            string glysString = strPara.Substring(2, 1);
            strPara = strPara.Remove(2, 3);
            currentString = (99 - int.Parse(currentString)).ToString();//电流从大到晓

            glysString = (99 - int.Parse(glysString)).ToString(); //功率因素还是大到小
            strPara = strPara.Insert(2, currentString + glysString);
            //return arrayTemp[0] + "_" + strPara;
            return int.Parse( strPara);

        }

        /// <summary>
        /// 初始化方案节点对应的结论节点
        /// </summary>
        /// <param name="schemaNode"></param>
        /// <returns></returns>
        public CheckNodeViewModel GetResultNode(SchemaNodeViewModel schemaNode)
        {
            #region 方案相关信息
            CheckNodeViewModel categoryModel = new CheckNodeViewModel
            {
                IsSelected = schemaNode.IsSelected,
                Name = schemaNode.Name,
                ParaNo = schemaNode.ParaNo,
                Level = schemaNode.Level
            };
            #endregion
            #region 如果为根节点则加载所有表位的详细信息
            for (int i = 0; i < schemaNode.ParaValuesCurrent.Count; i++)
            {
                CheckNodeViewModel itemModel = new CheckNodeViewModel
                {
                    IsSelected = schemaNode.IsSelected,
                    Name = schemaNode.ParaValuesCurrent[i].GetProperty("PARA_NAME") as string,
                    ParaNo = schemaNode.ParaValuesCurrent[i].GetProperty("PARA_NO") as string,
                    ItemKey = schemaNode.ParaValuesCurrent[i].GetProperty("PARA_KEY") as string,
                    Level = schemaNode.Level + 1
                };
                //初始化详细结论
                itemModel.InitializeCheckResults();
                //设置父节点
                itemModel.Parent = categoryModel;
                //添加到总结论集合,方便使用
                ResultCollection.Add(itemModel);
                categoryModel.Children.Add(itemModel);
            }
            #endregion
            #region 对子节点递归
            for (int i = 0; i < schemaNode.Children.Count; i++)
            {
                CheckNodeViewModel nodeChild = GetResultNode(schemaNode.Children[i]);
                nodeChild.Parent = categoryModel;
                categoryModel.Children.Add(nodeChild);
            }
            #endregion
            return categoryModel;
        }
        #endregion
        /// <summary>
        /// 清除当前点的检定结论
        /// </summary>
        public void ResetCurrentResult()
        {
            if (ResultCollection.Count <= EquipmentData.Controller.Index || EquipmentData.Controller.Index < 0)
            {
                return;
            }

            bool[] yaojianTemp = EquipmentData.MeterGroupInfo.YaoJian;
            List<string> listNames = ResultCollection[EquipmentData.Controller.Index].CheckResults[0].GetAllProperyName();
            #region 更新详细结论

            if (EquipmentData.Schema.ParaNo == Core.Enum.ProjectID.初始固有误差)//初始固有误差清除结论--清除当前方向的数据，将结论改成待完成
            {
                DynamicViewModel viewModel = EquipmentData.Schema.ParaValues[EquipmentData.Controller.Index];
                string name = viewModel.GetProperty("PARA_NAME") as string;//获取初始固有误差是那个方向
                name = name.EndsWith("_低到高") ? "上升" : "下降";
                List<string> setName = new List<string>() { "误差1", "误差2", "平均值", "化整值" };
                for (int i = 0; i < CheckNodeCurrent.CheckResults.Count; i++)
                {
                    //只更新要检表的结论
                    if (yaojianTemp[i])
                    {
                        for (int j = 0; j < setName.Count; j++)
                        {
                            CheckNodeCurrent.CheckResults[i].SetProperty(name + setName[j], "");
                        }
                        CheckNodeCurrent.CheckResults[i].SetProperty("差值", "");
                        string r = CheckNodeCurrent.CheckResults[i].GetProperty("结论") as string;
                        if (r != "")
                        {
                            CheckNodeCurrent.CheckResults[i].SetProperty("结论", "待完成");
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < CheckNodeCurrent.CheckResults.Count; i++)
                {
                    //只更新要检表的结论
                    if (yaojianTemp[i])
                    {
                        for (int j = 0; j < listNames.Count; j++)
                        {
                            if (listNames[j] != "要检")
                            {
                                CheckNodeCurrent.CheckResults[i].SetProperty(listNames[j], "");
                            }
                            if (listNames[j] == "分项结论376" || listNames[j] == "分项结论698")
                            {
                                CheckNodeCurrent.CheckResults[i].SetItemResoultProperty(listNames[j], "");
                            }
                        }
                    }
                }
            }
            #endregion
            RefreshDetailResult();
            CheckNodeCurrent.RefreshResultSummary();
            UpdateResultSummaryUp(CheckNodeCurrent);
        }



        /// <summary>
        /// 清除当前点的检定结论
        /// </summary>
        public void ResetCurrentResult2(int Index)
        {
            if (ResultCollection.Count <=Index ||Index < 0)
            {
                return;
            }

            bool[] yaojianTemp = EquipmentData.MeterGroupInfo.YaoJian;
            List<string> listNames = ResultCollection[Index].CheckResults[0].GetAllProperyName();
            #region 更新详细结论
            for (int i = 0; i < CheckNodeCurrent.CheckResults.Count; i++)
            {
                //只更新要检表的结论
                if (yaojianTemp[i])
                {
                    for (int j = 0; j < listNames.Count; j++)
                    {
                        if (listNames[j] != "要检")
                        {
                            EquipmentData.CheckResults.ResultCollection[Index].CheckResults[i].SetProperty(listNames[j], "");
                        }
                    }
                }
            }
            #endregion
            RefreshDetailResult();
            EquipmentData.CheckResults.ResultCollection[Index].RefreshResultSummary();
            UpdateResultSummaryUp(EquipmentData.CheckResults.ResultCollection[Index]);
        }

        /// <summary>
        /// 清除所有的结论
        /// </summary>
        public void ClearAllResult()
        {
            bool[] yaojianTemp = EquipmentData.MeterGroupInfo.YaoJian;
            for (int j = 0; j < ResultCollection.Count; j++)
            {
                List<string> listNames = ResultCollection[j].CheckResults[0].GetAllProperyName();
                #region 更新详细结论
                for (int i = 0; i < ResultCollection[j].CheckResults.Count; i++)
                {
                    try
                    {
                        ResultCollection[j].IsChecked = false;
                    }
                    catch (Exception) { }
                    //只更新要检表的结论
                    if (yaojianTemp[i])
                    {
                        for (int k = 0; k < listNames.Count; k++)
                        {
                            if (listNames[k] != "要检")
                            {
                                ResultCollection[j].CheckResults[i].SetProperty(listNames[k], "");
                            }
                        }
                    }
                }
                #endregion
                ResultCollection[j].RefreshResultSummary();
            }
            for (int i = 0; i < Categories.Count; i++)
            {
                UpdateResultSummaryDown(Categories[i]);
            }
        }
        public Object Lock = new object();

        /// <summary>
        /// 更新检定结论
        /// </summary>
        /// <param name="itemKey"></param>
        /// <param name="columnName"></param>
        /// <param name="arrayResult"></param>
        public void UpdateCheckResult(string itemKey, string columnName, string[] arrayResult) //,string[] ErrorReason=null
        {
            bool[] yaoJianTemp = EquipmentData.MeterGroupInfo.YaoJian;


            //是否传来的数据信息为表地址数据,如果是则更新表地址
            //bool flagUpdateMeterAddress = (itemKey == "08005" || itemKey == "08001") && columnName == "检定数据";

            #region 判断编号和结论


            CheckNodeViewModel nodeTemp = GetResultNode(itemKey);
            CheckNodeViewModel nodeTempTow = null; 
            if (itemKey.StartsWith(Core.Enum.ProjectID.初始固有误差))
            {
                nodeTempTow = GetResultNodeTow(itemKey); //找到对应的第二个
            }
            if (nodeTemp == null || nodeTemp.CheckResults.Count < 0)
            {
                LogManager.AddMessage(string.Format("未找到检定点编号{0}对应的检定点编号", itemKey), EnumLogSource.检定业务日志, EnumLevel.Warning);
                return;
            }
            List<string> listNames = nodeTemp.CheckResults[0].GetAllProperyName();
            if (!listNames.Contains(columnName))
            {
                LogManager.AddMessage(string.Format("不识别的检定结论:{0}", columnName), EnumLogSource.检定业务日志, EnumLevel.Warning);
                return;
            }
            #endregion




            #region 更新详细结论

            //更新列数据
            for (int i = 0; i < nodeTemp.CheckResults.Count; i++)
            {
                if (arrayResult.Length > i)
                {
                    if (yaoJianTemp[i])   //只更新要检表的结论
                    {
                        if (!(arrayResult[i] == null))
                        {
                            if (columnName.IndexOf("分项结论") != -1)
                            {
                                nodeTemp.CheckResults[i].SetItemResoultProperty(columnName, arrayResult[i]);
                            }
                            else
                            {
                                nodeTemp.CheckResults[i].SetProperty(columnName, arrayResult[i]);
                                if(nodeTempTow!=null) nodeTempTow.CheckResults[i].SetProperty(columnName, arrayResult[i]);
                            }

                        }
                    }
                }
            }
            #region 更新总结论

            lock (Lock)
            {
                if (columnName == "结论")
                {
                    nodeTemp.RefreshResultSummary();
                    UpdateResultSummaryUp(nodeTemp);
                    if (nodeTempTow != null)
                    {
                        nodeTempTow.RefreshResultSummary();
                        UpdateResultSummaryUp(nodeTempTow);
                    }
                    CheckResultBll.Instance.SaveCheckResult(nodeTemp);

                    int indexTemp = ResultCollection.IndexOf(nodeTemp);
                    TimeMonitor.Instance.ActiveCurrentItem(indexTemp, true);
                    //TimeMonitor.Instance.FinishCurrentItem(indexTemp);

                    int IsQualifiedNum = 0;
                    List<string> sqlSumRetList = new List<string>();
                    string[] strResult = new string[] { "合格" };
                    for (int i = 0; i < nodeTemp.CheckResults.Count; i++)
                    {
                        if (arrayResult.Length > i)
                        {
                            //只更新要检表的结论
                            if (yaoJianTemp[i])
                            {
                                if (!string.IsNullOrEmpty(arrayResult[i]))
                                {
                                    //获得表iD
                                    string meterId = EquipmentData.MeterGroupInfo.Meters[i].GetProperty("METER_ID") as string;
                                    //查询数据库中id等于这个表的数据
                                    string strSQL = string.Format("METER_ID  = '{0}'", meterId);
                                    //大项目结论表
                                    List<DynamicModel> models = DALManager.MeterTempDbDal.GetList("T_TMP_METER_COMMUNICATION", strSQL);

                                    if (models.Count > 0)
                                    {
                                        strResult = new string[models.Count];
                                        for (int j = 0; j < models.Count; j++)
                                        {
                                            //大项目结论
                                            strResult[j] = models[j].GetProperty("MD_RESULT") as string;
                                        }
                                    }
                                    //MD_RESULE

                                    //设置整个表的总结论
                                    if (Array.IndexOf(strResult, "不合格") > -1)  //有一个数据不合格，总结论不合格
                                    {
                                        IsQualifiedNum += 1;
                                        sqlSumRetList.Add(string.Format("update T_TMP_METER_INFO set MD_RESULT ='{0}' where METER_ID  = '{1}'", "不合格", meterId));
                                        EquipmentData.MeterGroupInfo.Meters[i].SetProperty("MD_RESULT", "不合格");
                                    }
                                    else
                                    {
                                        sqlSumRetList.Add(string.Format("update T_TMP_METER_INFO set MD_RESULT ='{0}' where METER_ID  = '{1}'", "合格", meterId));
                                        EquipmentData.MeterGroupInfo.Meters[i].SetProperty("MD_RESULT", "合格");
                                    }
                                }
                            }
                        }
                    }
                    int countTemp = DALManager.MeterTempDbDal.ExecuteOperation(sqlSumRetList);
                    LogManager.AddMessage(string.Format("更新数据库中总结论信息完成,共更新{0}条", countTemp), EnumLogSource.数据库存取日志);
                    if (IsQualifiedNum >= VerifyConfig.FailureRate)
                    {
                        LogManager.AddMessage("要检表不合格数量超标,当前不合格数量:"+ IsQualifiedNum, EnumLogSource.检定业务日志, EnumLevel.Tip);
                        EquipmentData.Controller.TryStopVerify();
                    }

                }
            }



            #endregion


            #region 更新分项结论

            #endregion
            #endregion

            RefreshDetailResult(); //刷新

            #region 更新表地址
           // List<string> sqlList = new List<string>();
            //if (flagUpdateMeterAddress)
            //{
            //    for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
            //    {
            //        if (yaoJianTemp[i] && !string.IsNullOrEmpty(arrayResult[i]))
            //        {
            //            sqlList.Add(string.Format("update T_TMP_METER_INFO set MD_POSTAL_ADDRESS ='{0}' where METER_ID  = '{1}'", arrayResult[i], EquipmentData.MeterGroupInfo.Meters[i].GetProperty("METER_ID")));
            //        }
            //    }
            //    //int countTemp = DALManager.MeterTempDbDal.ExecuteOperation(sqlList);
            //    //LogManager.AddMessage(string.Format("更新数据库中表地址信息完成,共更新{0}条", countTemp), EnumLogSource.数据库存取日志);
            //}
            #endregion


        }

        /// <summary>
        /// 获得检定项目的详细数据
        /// </summary>
        public Dictionary<string, string[]> GetCheckResult()
        {
            Dictionary<string, string[]> values = new Dictionary<string, string[]>();
            if (ResultCollection.Count <= EquipmentData.Controller.Index || EquipmentData.Controller.Index < 0)
            {
                return null;
            }

            bool[] yaojianTemp = EquipmentData.MeterGroupInfo.YaoJian;
            List<string> listNames = ResultCollection[EquipmentData.Controller.Index].CheckResults[0].GetAllProperyName();
            for (int i = 0; i < listNames.Count; i++)
            {
                if (!values.ContainsKey(listNames[i]))
                {
                    values.Add(listNames[i], new string[yaojianTemp.Length]);
                }
            }
            for (int i = 0; i < CheckNodeCurrent.CheckResults.Count; i++)
            {
                //只更新要检表的结论
                if (yaojianTemp[i])
                {
                    for (int j = 0; j < listNames.Count; j++)
                    {
                        if (listNames[j] != "要检")
                        {
                            values[listNames[j]][i] = CheckNodeCurrent.CheckResults[i].GetProperty(listNames[j]) as string;
                        }
                    }
                }
            }
            return values;

        }
        public bool GetMeterResult(int meterNo, string exceptId = "")
        {
            if (meterNo >= EquipmentData.MeterGroupInfo.Meters.Count)
            {
                return false;
            }
            string str = EquipmentData.MeterGroupInfo.Meters[meterNo].GetProperty("MD_RESULT").ToString();
            if (str == "合格")
            {
                return true;
            }
            else
            {
                //获得表iD
                string meterId = EquipmentData.MeterGroupInfo.Meters[meterNo].GetProperty("METER_ID") as string;
                //查询数据库中id等于这个表的数据
                string strSQL = string.Format("METER_ID  = '{0}'", meterId);
                //大项目结论表
                List<DynamicModel> models = DALManager.MeterTempDbDal.GetList("T_TMP_METER_COMMUNICATION", strSQL);
                string[] strResult = new string[] { "合格" };
                if (models.Count > 0)
                {
                    strResult = new string[models.Count];
                    for (int j = 0; j < models.Count; j++)
                    {
                        //大项目结论
                        string s = models[j].GetProperty("MD_PROJECT_NO") as string;
                        if (s == exceptId) continue;
                        strResult[j] = models[j].GetProperty("MD_RESULT") as string;
                    }
                }

                //设置整个表的总结论
                if (Array.IndexOf(strResult, "不合格") > -1)  //有一个数据不合格，总结论不合格
                {
                    return false;
                }
                return true;
            }
        }


        #region 刷新结论总览,从上往下和从下往上两个方法
        public void UpdateResultSummaryDown(CheckNodeViewModel nodeTop)
        {
            if (nodeTop.CheckResults.Count > 0)
            {
                return;
            }
            else
            {
                nodeTop.RefreshResultSummary();
                for (int i = 0; i < nodeTop.Children.Count; i++)
                {
                    nodeTop.Children[i].RefreshResultSummary();
                }
            }
        }
        public void UpdateResultSummaryUp(CheckNodeViewModel nodeBottom)
        {
            CheckNodeViewModel nodeParent = nodeBottom.Parent;
            while (nodeParent != null)
            {
                if (nodeParent != null)
                {
                    nodeParent.RefreshResultSummary();
                }
                nodeParent = nodeParent.Parent;
            }
        }
        #endregion


        private CheckNodeViewModel GetResultNode(string itemKey)
        {
            int indexTemp = EquipmentData.Controller.Index;
            CheckNodeViewModel nodeResult = null;
            for (; indexTemp >= 0; indexTemp--)
                if (indexTemp < ResultCollection.Count)
                {
                    CheckNodeViewModel nodeTemp = ResultCollection[indexTemp];
                    if (nodeTemp.ItemKey == itemKey && nodeTemp.IsSelected)
                    {
                        nodeResult = nodeTemp;
                        break;
                    }
                }
            if (nodeResult==null)
            {
                indexTemp = EquipmentData.Controller.Index;
                for (int i = indexTemp; i < ResultCollection.Count; i++)
                {
                    CheckNodeViewModel nodeTemp = ResultCollection[i];
                    if (nodeTemp.ItemKey == itemKey && nodeTemp.IsSelected)
                    {
                        nodeResult = nodeTemp;
                        break;
                    }
                }
            }
            return nodeResult;
        }

        private CheckNodeViewModel GetResultNodeTow(string itemKey)
        {
            int indexTemp = EquipmentData.Controller.Index;
            CheckNodeViewModel nodeResult = null;
            for (int i = 0; i < ResultCollection.Count; i++)
            {
                if (i == indexTemp) continue;//排除自身外的另一个
                CheckNodeViewModel nodeTemp = ResultCollection[i];
                if (nodeTemp.ItemKey == itemKey && nodeTemp.IsSelected)
                {
                    nodeResult = nodeTemp;
                    break;
                }
            }
            return nodeResult;
        }

        /// <summary>
        /// 更新表位要检状态
        /// </summary>
        public void UpdateYaoJian()
        {
            for (int i = 0; i < ResultCollection.Count; i++)
            {
                UpdateYaoJian(i);
            }
        }

        /// <summary>
        /// 更新要检标记,序号从0开始
        /// </summary>
        /// <param name="meterIndex"></param>
        public void UpdateYaoJian(int meterIndex)
        {
            bool[] yaojianTemp = EquipmentData.MeterGroupInfo.YaoJian;
            for (int j = 0; j < ResultCollection.Count; j++)
            {
                if (ResultCollection.Count > j && ResultCollection[j].CheckResults.Count > meterIndex)
                {
                    ResultCollection[j].CheckResults[meterIndex].SetProperty("要检", yaojianTemp[meterIndex]);
                }
            }
        }

        //private AsyncObservableCollection<CheckNodeViewModel> categories = new AsyncObservableCollection<CheckNodeViewModel>();

        private AsyncObservableCollection<CheckNodeViewModel> categories = new AsyncObservableCollection<CheckNodeViewModel>();

        /// <summary>
        /// 检定大类列表
        /// </summary>
        public AsyncObservableCollection<CheckNodeViewModel> Categories
        {
            get { return categories; }
            set { SetPropertyValue(value, ref categories, "Categories"); }
        }

        #region 更新主界面检定数据




        private AsyncObservableCollection<DynamicViewModel> detailResultView = new AsyncObservableCollection<DynamicViewModel>();
        /// <summary>
        /// 当前显示的检定点
        /// 将界面显示的检定点结论模型固定,不重新设置值,这样可以大大提高界面绑定数据的速度
        /// </summary>
        public AsyncObservableCollection<DynamicViewModel> DetailResultView
        {
            get { return detailResultView; }
            set { SetPropertyValue(value, ref detailResultView, "DetailResultView"); }
        }


        /// <summary>
        /// 更改显示结论的列
        /// </summary>
        private void LoadViewColumn()
        {
            for (int i = 0; i < DetailResultView.Count; i++)
            {
                List<string> propertyNames = detailResultView[i].GetAllProperyName();
                for (int j = 0; j < propertyNames.Count; j++)
                {
                    if (propertyNames[j] != "要检")
                    {
                        DetailResultView[i].RemoveProperty(propertyNames[j]);
                    }
                }
                if (CheckNodeCurrent.CheckResults.Count <= i)
                {
                    continue;
                }
                propertyNames = CheckNodeCurrent.CheckResults[i].GetAllProperyName();
                for (int j = 0; j < propertyNames.Count; j++)
                {
                    DetailResultView[i].SetProperty(propertyNames[j], CheckNodeCurrent.CheckResults[i].GetProperty(propertyNames[j]));
                }
            }
            FlagLoadColumn = false;
        }
        /// <summary>
        /// 更新当前显示的数据
        /// </summary>
        private void RefreshDetailResult()
        {
            Parallel.For(0, DetailResultView.Count, (i) =>
            {
                try
                {
                    List<string> propertyNames = CheckNodeCurrent.CheckResults[i].GetAllProperyName();
                    for (int j = 0; j < propertyNames.Count; j++)
                    {
                        DetailResultView[i].SetProperty(propertyNames[j], CheckNodeCurrent.CheckResults[i].GetProperty(propertyNames[j]));
                    }
                }
                catch
                {
                }
            });
        }
        private bool flagLoadColumn;
        /// <summary>
        /// 列加载完毕标记
        /// 如果检定结论视图发生了变化,值会变成true,界面加载完毕以后值会变为false,该标记用于防止界面加载过于频繁影响速度
        /// </summary>
        public bool FlagLoadColumn
        {
            get { return flagLoadColumn; }
            set
            {
                SetPropertyValue(value, ref flagLoadColumn, "FlagLoadColumn");
            }
        }
        #endregion
        /// <summary>
        /// 更新详细结论中的要检标记
        /// </summary>
        public void RefreshYaojian()
        {
            bool[] yaojian = EquipmentData.MeterGroupInfo.YaoJian;
            for (int i = 0; i < ResultCollection.Count; i++)
            {
                for (int j = 0; j < EquipmentData.Equipment.MeterCount; j++)
                {
                    if (yaojian.Length > j)
                    {
                        ResultCollection[i].CheckResults[j].SetProperty("要检", yaojian[j]);
                    }
                }
            }
            //更新界面显示
            for (int j = 0; j < EquipmentData.Equipment.MeterCount; j++)
            {
                if (yaojian.Length > j)
                {
                    detailResultView[j].SetProperty("要检", yaojian[j]);
                }
            }
        }
    }
}
