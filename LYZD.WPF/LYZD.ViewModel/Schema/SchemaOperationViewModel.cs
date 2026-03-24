using LYZD.DAL;
using LYZD.Utility;
using LYZD.Utility.Log;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LYZD.ViewModel.Schema
{
    /// 方案操作视图模型
    /// <summary>
    /// 方案操作视图模型
    /// </summary>
    public class SchemaOperationViewModel : ViewModelBase
    {
        public SchemaOperationViewModel()
        {
            LoadSchemas();
        }
        private DynamicViewModel selectedSchema = new DynamicViewModel(0);

        public DynamicViewModel SelectedSchema
        {
            get { return selectedSchema; }
            set { SetPropertyValue(value, ref selectedSchema, "SelectedSchema"); }
        }
        private ObservableCollection<DynamicViewModel> schemasALL = new ObservableCollection<DynamicViewModel>();
        /// <summary>
        ///所有方案名称列表
        /// </summary>
        public ObservableCollection<DynamicViewModel> SchemasALL
        {
            get { return schemasALL; }
            set { SetPropertyValue(value, ref schemasALL, "SchemasALL"); }
        }


        private ObservableCollection<DynamicViewModel> schemas = new ObservableCollection<DynamicViewModel>();
        /// <summary>
        /// 方案名称列表
        /// </summary>
        public ObservableCollection<DynamicViewModel> Schemas
        {
            get { return schemas; }
            set { SetPropertyValue(value, ref schemas, "Schemas"); }
        }
        //public Dictionary<string, string> ApplicabilitySchemas = new Dictionary<string, string>();
        /// 加载方案列表
        /// <summary>
        /// 加载方案列表
        /// </summary>
        private void LoadSchemas()
        {     //EquipmentData.Equipment.MeterType
            //EquipmentData.Equipment.EquipmentType
            //List<DynamicModel> models = DALManager.ApplicationDbDal.GetList(EnumAppDbTable.T_SCHEMA_INFO.ToString());

            //根据电表类型和检测类型筛选需要的方案
            List<DynamicModel> models = DALManager.SchemaDal.GetList(EnumAppDbTable.T_SCHEMA_INFO.ToString(), $"SCHEMA_METER_TYPE='{EquipmentData.Equipment.EquipmentType}' AND SCHEMA_TEST_TYPE='{EquipmentData.Equipment.MeterType}' AND SCHEMA_ENABLED='1' ");


            //对方案根据名称进行一个排序
            models.Sort((a,b)=>a.GetProperty("SCHEMA_NAME").ToString().CompareTo(b.GetProperty("SCHEMA_NAME").ToString()));
            Schemas.Clear();
            for (int i = 0; i < models.Count; i++)
            {
                Schemas.Add(new DynamicViewModel(models[i], i));
            }

            SchemasALL.Clear();    
            models = DALManager.SchemaDal.GetList(EnumAppDbTable.T_SCHEMA_INFO.ToString(), $"SCHEMA_METER_TYPE='{EquipmentData.Equipment.EquipmentType}' AND SCHEMA_TEST_TYPE='{EquipmentData.Equipment.MeterType}'");
            for (int i = 0; i < models.Count; i++)
            {
                SchemasALL.Add(new DynamicViewModel(models[i], i));
            }
            //ApplicabilitySchemas.Clear();


        }
        /// 方案名校验
        /// <summary>
        /// 方案名校验
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        private bool CheckSchemaName(string schemaName)
        {
            //名称校验
            if (string.IsNullOrEmpty(schemaName))
            {
                LogManager.AddMessage("方案名无效,方案名不允许为空", EnumLogSource.用户操作日志, EnumLevel.Warning);
                return false;
            }
            //if (!StringCheck.IsFileName(schemaName))
            //{
            //    LogManager.AddMessage("方案名无效,方案名只允许为数字字母和下划线", EnumLogSource.用户操作日志, EnumLevel.WarningSpeech);
            //    return false;
            //}
            //不允许重名                                                                 $"SCHEMA_METER_TYPE='{EquipmentData.Equipment.EquipmentType}' AND SCHEMA_TEST_TYPE='{EquipmentData.Equipment.MeterType}'"
            // if (DALManager.ApplicationDbDal.GetCount(EnumAppDbTable.T_SCHEMA_INFO.ToString(), string.Format("schema_name ='{0}'", schemaName)) > 0)
            if (DALManager.SchemaDal.GetCount(EnumAppDbTable.T_SCHEMA_INFO.ToString(), string.Format("schema_name ='{0}' AND  SCHEMA_METER_TYPE='{1}' AND SCHEMA_TEST_TYPE='{2}'", schemaName, EquipmentData.Equipment.EquipmentType, EquipmentData.Equipment.MeterType)) > 0)
            {
                LogManager.AddMessage("方案名无效,方案名已存在", EnumLogSource.用户操作日志, EnumLevel.Warning);
                return false;
            }
            return true;
        }
        /// 方案重命名
        /// <summary>
        /// 方案重命名
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameSchema()
        {
            if (!CheckSchemaName(newName))
            {
                return;
            }
            if (MessageBox.Show("确认对方案重命名?", "重命名", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string origionalName = SelectedSchema.GetProperty("SCHEMA_NAME") as string;
                DynamicModel model = new DynamicModel();
                model.SetProperty("SCHEMA_NAME", NewName);
                DALManager.SchemaDal.Update(EnumAppDbTable.T_SCHEMA_INFO.ToString(), string.Format("schema_name = '{0}'", origionalName), model, new List<string> { "SCHEMA_NAME" });
                LoadSchemas();

                DynamicViewModel modelTemp = EquipmentData.SchemaModels.Schemas.FirstOrDefault(item => item.GetProperty("SCHEMA_NAME") as string == origionalName);
                if (modelTemp != null)
                {
                    modelTemp.SetProperty("SCHEMA_NAME", NewName);
                }
            }
        }
        #region 添加方案
        private string newName;
        public string NewName
        {
            get { return newName; }
            set { SetPropertyValue(value, ref newName, "NewName"); }
        }
        private string schemaType;
        public string SchemaType
        {
            get { return schemaType; }
            set { SetPropertyValue(value, ref schemaType, "SchemaType"); }
        }

        /// <summary>
        /// 添加方案
        /// </summary>
        public void AddSchema()
        {
            if (!CheckSchemaName(NewName))
            {
                return;
            }
            //【方案标注】如果不需要方案分类，这里需要修改
            //if (string.IsNullOrEmpty(SchemaType))
            //{
            //    LogManager.AddMessage("请选择新建的检定方案类型.", EnumLogSource.用户操作日志, EnumLevel.WarningSpeech);
            //    return;
            //}
            DynamicModel model = new DynamicModel();
            model.SetProperty("SCHEMA_NAME", NewName);
            model.SetProperty("SCHEMA_METER_TYPE", EquipmentData.Equipment.EquipmentType);
            model.SetProperty("SCHEMA_TEST_TYPE", EquipmentData.Equipment.MeterType);
            model.SetProperty("SCHEMA_ENABLED", "1");

            //{     //EquipmentData.Equipment.MeterType
            //      //EquipmentData.Equipment.EquipmentType

            //model.SetProperty("SCHEMA_TYPE", SchemaType);
            DALManager.SchemaDal.Insert(EnumAppDbTable.T_SCHEMA_INFO.ToString(), model);
            LoadSchemas();

            DynamicViewModel modelTemp = Schemas.FirstOrDefault(item => item.GetProperty("SCHEMA_NAME") as string == NewName);
            if (modelTemp != null)
            {
                EquipmentData.SchemaModels.Schemas.Add(modelTemp);
            }
        }

        /// <summary>
        /// 添加方案
        /// </summary>
        public void AddDownSchema()
        {
            if (!CheckSchemaName(NewName))
            {
                return;
            }
            //【方案标注】如果不需要方案分类，这里需要修改
            //if (string.IsNullOrEmpty(SchemaType))
            //{
            //    LogManager.AddMessage("请选择新建的检定方案类型.", EnumLogSource.用户操作日志, EnumLevel.WarningSpeech);
            //    return;
            //}
            DynamicModel model = new DynamicModel();
            model.SetProperty("SCHEMA_NAME", NewName);
            model.SetProperty("SCHEMA_METER_TYPE", EquipmentData.Equipment.EquipmentType);
            model.SetProperty("SCHEMA_TEST_TYPE", EquipmentData.Equipment.MeterType);
            model.SetProperty("SCHEMA_ENABLED", "1");
            //{     //EquipmentData.Equipment.MeterType
            //      //EquipmentData.Equipment.EquipmentType

            //model.SetProperty("SCHEMA_TYPE", SchemaType);
            DALManager.SchemaDal.Insert(EnumAppDbTable.T_SCHEMA_INFO.ToString(), model);

            DynamicViewModel modelTemp = Schemas.FirstOrDefault(item => item.GetProperty("SCHEMA_NAME") as string == NewName);
            if (modelTemp != null)
            {
                EquipmentData.SchemaModels.Schemas.Add(modelTemp);
            }
            LoadSchemas();
        }
        #endregion
        #region 拷贝方案
        /// 拷贝方案
        /// <summary>
        /// 拷贝方案
        /// </summary>
        public void CopySchema()
        {
            if (!CheckSchemaName(NewName))
            {
                return;
            }
            #region 获取旧方案信息
            int oldId = (int)(SelectedSchema.GetProperty("ID"));
            List<DynamicModel> models = DALManager.SchemaDal.GetList(EnumAppDbTable.T_SCHEMA_PARA_VALUE.ToString(), string.Format("SCHEMA_ID={0}", oldId));
            #endregion
            #region 插入新方案导数据库
            int newId = 0;
            SelectedSchema.SetProperty("SCHEMA_NAME", NewName);
            DALManager.SchemaDal.Insert(EnumAppDbTable.T_SCHEMA_INFO.ToString(), SelectedSchema.GetDataSource());
            DynamicModel newModel = DALManager.SchemaDal.GetByID(EnumAppDbTable.T_SCHEMA_INFO.ToString(), string.Format("schema_name='{0}'", NewName));
            if (newModel != null)
            {
                newId = (int)(newModel.GetProperty("ID"));
                for (int i = 0; i < models.Count; i++)
                {
                    models[i].SetProperty("SCHEMA_ID", newId);
                    DALManager.SchemaDal.Insert(EnumAppDbTable.T_SCHEMA_PARA_VALUE.ToString(), models[i]);
                }
            }
            else
            {
                LogManager.AddMessage("方案拷贝失败!!!", EnumLogSource.用户操作日志, EnumLevel.Error);
            }
            #endregion
            LoadSchemas();

            DynamicViewModel modelTemp = Schemas.FirstOrDefault(item => item.GetProperty("SCHEMA_NAME") as string == NewName);
            if (modelTemp != null)
            {
                EquipmentData.SchemaModels.Schemas.Add(modelTemp);
            }
        }
        #endregion
        /// <summary>
        /// 删除方案
        /// </summary>
        public void DeleteSchema()
        {
            if (MessageBox.Show("方案很重要,请谨慎操作,确认要删除方案吗?", "删除方案", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                int oldId = (int)(SelectedSchema.GetProperty("ID"));
                DALManager.SchemaDal.Delete(EnumAppDbTable.T_SCHEMA_INFO.ToString(), string.Format("ID={0}", oldId));
                LoadSchemas();

                DynamicViewModel modelTemp = EquipmentData.SchemaModels.Schemas.FirstOrDefault(item => (int)item.GetProperty("ID") == oldId);
                if (modelTemp != null)
                {
                    EquipmentData.SchemaModels.Schemas.Remove(modelTemp);
                }
            }
        }


        public void SetShecmaValue()
        {
            try
            {
                for (int i = 0; i < SchemasALL.Count; i++)
                {
                    DynamicViewModel dynamic = SchemasALL[i];
                    int oldId = (int)dynamic.GetProperty("ID");
                    //string name = dynamic.GetProperty("ID");

                    DynamicModel model = new DynamicModel();
                    model.SetProperty("SCHEMA_NAME", dynamic.GetProperty("SCHEMA_NAME"));
                    model.SetProperty("SCHEMA_METER_TYPE", dynamic.GetProperty("SCHEMA_METER_TYPE"));
                    model.SetProperty("SCHEMA_TEST_TYPE", dynamic.GetProperty("SCHEMA_TEST_TYPE"));
                    model.SetProperty("SCHEMA_ENABLED", dynamic.GetProperty("SCHEMA_ENABLED"));

                    DALManager.SchemaDal.Update(EnumAppDbTable.T_SCHEMA_INFO.ToString(), string.Format("ID={0}", oldId), model, new List<string>
                { "SCHEMA_NAME" ,"SCHEMA_METER_TYPE","SCHEMA_TEST_TYPE","SCHEMA_ENABLED"});

                    //DALManager.SchemaDal.Delete(EnumAppDbTable.T_SCHEMA_INFO.ToString(), string.Format("ID={0}", oldId));
                }
                MessageBox.Show("保存成功");

            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败"+ex.ToString());

            }

        }
        public void RefreshCurrrentSchema()
        {
            OnPropertyChanged("SelectedSchema");
        }
    }


}
