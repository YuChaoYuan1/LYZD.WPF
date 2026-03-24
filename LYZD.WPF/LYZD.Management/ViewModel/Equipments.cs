using LYZD.DAL;
using LYZD.DAL.DataBaseView;
using LYZD.ViewModel;
using LYZD.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.DataManager.ViewModel
{
    /// <summary>
    /// 台体信息
    /// </summary>
    public class Equipments : ViewModelBase
    {
        private static Equipments instance;

        public static Equipments Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Equipments();
                }
                return instance;
            }
        }
        private TableDisplayModel displayModel = ResultViewHelper.GetTableDisplayModel("013", true);
        public void Initialize()
        {
            //List<DynamicModel> modelList = DALManager.ApplicationDbDal.GetList("DSPTCH_EQUIP_INFO");
            //for (int i = 0; i < modelList.Count; i++)
            //{
            //    DynamicViewModel modelTemp = new DynamicViewModel(i);
            //    #region 解析台体信息
            //    DynamicModel modelEquip = modelList[i];
            //    for (int j = 0; j < displayModel.ColumnModelList.Count; j++)
            //    {
            //        ColumnDisplayModel columnModel = displayModel.ColumnModelList[j];
            //        string fieldValue = modelEquip.GetProperty(columnModel.Field) as string;
            //        if (fieldValue == null)
            //        {
            //            fieldValue = "";
            //        }
            //        string[] displayNames = columnModel.DisplayName.Split('|');
            //        string[] valueArray = fieldValue.Split('|');
            //        for (int k = 0; k < displayNames.Length; k++)
            //        {
            //            if (valueArray.Length > k)
            //            {
            //                modelTemp.SetProperty(displayNames[k], valueArray[k]);
            //            }
            //            else
            //            {
            //                modelTemp.SetProperty(displayNames[k], "");
            //            }
            //        }
            //    }
            //    #endregion
            //    Models.Add(modelTemp);
            //}
        }

        private AsyncObservableCollection<DynamicViewModel> models=new AsyncObservableCollection<DynamicViewModel>();
        /// <summary>
        /// 信息列表
        /// </summary>
        public AsyncObservableCollection<DynamicViewModel> Models
        {
            get { return models; }
            set { models = value; }
        }
        /// <summary>
        /// 创建新的台体信息
        /// </summary>
        /// <param name="equipmentNo"></param>
        public DynamicViewModel FindEquipInfo(string equipmentNo)
        {
            DynamicViewModel modelTemp = Models.FirstOrDefault(item => item.GetProperty(equipNoName) as string == equipmentNo);
            if (modelTemp != null)
            {
                return modelTemp;
            }
            modelTemp = new DynamicViewModel(0);
            for (int j = 0; j < displayModel.ColumnModelList.Count; j++)
            {
                ColumnDisplayModel columnModel = displayModel.ColumnModelList[j];
                if (columnModel.Field == "AVR_DEVICE_ID")
                {
                    modelTemp.SetProperty(columnModel.DisplayName, equipmentNo);
                    continue;
                }
                string[] displayNames = columnModel.DisplayName.Split('|');
                for (int k = 0; k < displayNames.Length; k++)
                {
                    modelTemp.SetProperty(displayNames[k], "");
                }
            }
            Models.Add(modelTemp);
            return modelTemp;
        }
        /// <summary>
        /// 获取台体编号的显示名称
        /// </summary>
        /// <returns></returns>
        private string equipNoName
        {
            get
            {
                ColumnDisplayModel columnModel = displayModel.ColumnModelList.Find(item => item.Field == "AVR_DEVICE_ID");
                return columnModel.DisplayName;
            }
        }
    }
}
