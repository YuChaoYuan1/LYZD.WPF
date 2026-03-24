using LYZD.DAL;
using LYZD.ViewModel.Model;
using System.Collections.Generic;

namespace LYZD.ViewModel.Schema
{
    /// <summary>
    /// 树形节点,用于检定方案
    /// </summary>
    public class SchemaNodeViewModel : ViewModelBase
    {
        private string name = "";
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetPropertyValue(value, ref name, "Name"); }
        }
        private string paraNo;
        /// <summary>
        /// 编号
        /// </summary>
        public string ParaNo
        {
            get { return paraNo; }
            set
            {
                SetPropertyValue(value, ref paraNo, "ParaNo");
            }
        }
        private int schemarId;
        /// <summary>
        /// 方案编号
        /// </summary>
        public int SchemaId
        {
            get { return schemarId; }
            set
            {
                SetPropertyValue(value, ref schemarId, "SchemaId");
                EquipmentData.LastCheckInfo.SaveCurrentCheckInfo();
                SaveSchemaID();
            }
        }

        private string applicability;
        /// <summary>
        /// 适用性
        /// </summary>
        public string Applicability
        {
            get { return applicability; }
            set { SetPropertyValue(value, ref applicability, "Applicability"); }
        }



        private bool isSelected = true;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                SetPropertyValue(value, ref isSelected, "IsSelected");

                if (isSelected != value)
                {
                    //如果节点被选中,那么父节点也会被选中
                    if (ParentNode != null && IsSelected)
                    {
                        ParentNode.IsSelected = true;
                    }
                }
                else
                {
                    if (isSelected && ParentNode != null)
                    {
                        ParentNode.IsSelected = isSelected;
                    }
                }
            }
        }
        /// <summary>
        /// 节点为第几层
        /// </summary>
        public int Level { get; set; }
        private AsyncObservableCollection<SchemaNodeViewModel> children = new AsyncObservableCollection<SchemaNodeViewModel>();
        /// <summary>
        /// 子节点
        /// </summary>
        public AsyncObservableCollection<SchemaNodeViewModel> Children
        {
            get { return children; }
            set
            {
                SetPropertyValue(value, ref children, "Children");
            }
        }
        /// <summary>
        /// 父节点
        /// </summary>
        public SchemaNodeViewModel ParentNode { get; set; }
        public int PointCount
        {
            get { return pointCount; }
            set { SetPropertyValue(value, ref pointCount, "PointCount"); }
        }
        private int pointCount = 0;
        private List<DynamicModel> paraValuesCurrent = new List<DynamicModel>();
        public List<DynamicModel> ParaValuesCurrent
        {
            get { return paraValuesCurrent; }
            set { paraValuesCurrent = value; }
        }
        private bool isTerminal;
        /// <summary>
        /// 是否为检定点
        /// </summary>
        public bool IsTerminal
        {
            get { return isTerminal; }
            set { SetPropertyValue(value, ref isTerminal, "IsTerminal"); }
        }

        public List<SchemaNodeViewModel> GetTerminalNodes()
        {
            return GetTerminalNodes(this);
        }
        /// <summary>
        /// 获取终端节点列表
        /// </summary>
        /// <param name="nodeTemp"></param>
        /// <returns></returns>
        private List<SchemaNodeViewModel> GetTerminalNodes(SchemaNodeViewModel nodeTemp)
        {
            List<SchemaNodeViewModel> nodesTemp = new List<SchemaNodeViewModel>();
            if (nodeTemp.IsTerminal)
            {
                nodesTemp.Add(nodeTemp);
            }
            for (int i = 0; i < nodeTemp.Children.Count; i++)
            {
                nodesTemp.AddRange(GetTerminalNodes(nodeTemp.Children[i]));
            }
            return nodesTemp;
        }

        private string viewNo;
        /// <summary>
        /// 显示视图编号
        /// </summary>
        public string ViewNo
        {
            get { return viewNo; }
            set
            {
                SetPropertyValue(value, ref viewNo, "ViewNo");
                OnPropertyChanged("HaveViewNo");
            }
        }
        public bool HaveViewNo
        {
            get { return !string.IsNullOrEmpty(viewNo); }
        }
        private string sortNo;

        public string SortNo
        {
            get { return sortNo; }
            set { SetPropertyValue(value, ref sortNo, "SortNo"); }
        }

        /// <summary>
        /// 在检定界面切换方案的时候需要改变临时数据库的方案id
        /// </summary>
        private void SaveSchemaID()
        {

            List<DynamicModel> models = new List<DynamicModel>();
            for (int i = 0; i < EquipmentData.MeterGroupInfo.Meters.Count; i++)
            {
                DynamicModel model = new DynamicModel();
                if (EquipmentData.Schema.SchemaId == 0)
                    model.SetProperty("MD_SCHEME_ID", SchemaId);
                else
                    model.SetProperty("MD_SCHEME_ID", EquipmentData.Schema.SchemaId);

                model.SetProperty("METER_ID", EquipmentData.MeterGroupInfo.Meters[i].GetProperty("METER_ID"));
                models.Add(model);
            }
            List<string> fieldNames = new List<string>() { "MD_SCHEME_ID" };
            DALManager.MeterTempDbDal.Update("T_TMP_METER_INFO", "METER_ID", models, fieldNames);
        }

    }
}
