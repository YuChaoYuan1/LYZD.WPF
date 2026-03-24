using LYZD.DAL;
using LYZD.DAL.DataBaseView;
using LYZD.ViewModel;
using LYZD.ViewModel.Model;

namespace LYZD.DataManager.Mark.ViewModel
{
    /// <summary>
    /// 台体信息配置
    /// </summary>
    public class ConfigEquipment :ViewModelBase
    {
        public class EquipmentItem :ViewModelBase
        {
            private string name;

            public string Name
            {
                get { return name; }
                set { SetPropertyValue(value, ref name, "Name"); }
            }
            private string content;

            public string Content
            {
                get { return content; }
                set { SetPropertyValue(value, ref content, "Content"); }
            }
        }
        private AsyncObservableCollection<EquipmentItem> infos=new AsyncObservableCollection<EquipmentItem>();

        public AsyncObservableCollection<EquipmentItem> Infos
        {
            get { return infos; }
            set { SetPropertyValue(value, ref infos, "Infos"); }
        }

        public ConfigEquipment(string equipmentNo)
        {
            TableDisplayModel displayModel = ResultViewHelper.GetTableDisplayModel("41");
            DynamicModel modelTemp = DALManager.ApplicationDbDal.GetByID("DSPTCH_EQUIP_INFO", string.Format("avr_device_id='{0}'", equipmentNo));
            if (displayModel != null && modelTemp!=null)
            {
                for (int i = 0; i < displayModel.ColumnModelList.Count; i++)
                {
                    string fieldName = displayModel.ColumnModelList[i].Field;
                    string fieldValue = modelTemp.GetProperty(fieldName) as string;
                    if(fieldValue==null)
                    {
                        fieldValue = "";
                    }
                    string[] arrayValue = fieldValue.Split('|');
                    string[] arrayDisplayName = displayModel.ColumnModelList[i].DisplayName.Split('|');
                    for (int j = 0; j < arrayDisplayName.Length; j++)
                    {
                        if (!string.IsNullOrEmpty(arrayDisplayName[j]))
                        {
                            EquipmentItem itemTemp = new EquipmentItem();
                            itemTemp.Name = arrayDisplayName[i];
                            if(i<arrayValue.Length)
                            {
                                itemTemp.Content = arrayValue[i];
                            }
                            Infos.Add(itemTemp);
                        }
                    }
                }
            }
        }
    }
}
