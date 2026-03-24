using LYZD.DAL.DataBaseView;
using LYZD.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.ViewModel.MisConfig
{
    public class MisConfigViewModel : ViewModelBase
    {
        #region 数据库相关
        private string tableName;
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName
        {
            get { return tableName; }
            set
            {
                SetPropertyValue(value, ref tableName, "TableName");
                LoadColumnNames();
            }
        }
        private AsyncObservableCollection<string> tableNames = new AsyncObservableCollection<string>();
        /// <summary>
        /// 表名称集合
        /// </summary>
        public AsyncObservableCollection<string> TableNames
        {
            get { return tableNames; }
            set { SetPropertyValue(value, ref tableNames, "TableNames"); }
        }
        private AsyncObservableCollection<string> fieldNames = new AsyncObservableCollection<string>();
        /// <summary>
        /// 字段名称集合
        /// </summary>
        public AsyncObservableCollection<string> FieldNames
        {
            get { return fieldNames; }
            set { SetPropertyValue(value, ref fieldNames, "FieldNames"); }
        }
        /// <summary>
        /// 加载字段名称列表
        /// </summary>
        private void LoadColumnNames()
        {
            FieldNames.Clear();
        }
        #endregion
        #region 字段配置集合
        private AsyncObservableCollection<ColumnResultUnit> units = new AsyncObservableCollection<ColumnResultUnit>();
        /// <summary>
        /// 检定结论配置集合
        /// </summary>
        public AsyncObservableCollection<ColumnResultUnit> Units
        {
            get { return units; }
            set { SetPropertyValue(value, ref units, "Units"); }
        }
        #endregion
        #region 根据检定项编号加载集合
        private string paraNo;
        /// <summary>
        /// 检定点编号
        /// </summary>
        public string ParaNo
        {
            get { return paraNo; }
            set
            {
                SetPropertyValue(value, ref paraNo, "ParaNo");
                LoadMisConfigUnits();
            }
        }
        /// <summary>
        /// 加载字段结论配置集合,这里需要进行调试
        /// </summary>
        private void LoadMisConfigUnits()
        {
            Units.Clear();
            #region 初始化所有名称
            TableDisplayModel displayModel = ResultViewHelper.GetParaNoDisplayModel(paraNo);
            if (displayModel != null)
            {
                List<string> namesList = displayModel.GetDisplayNames();
                for (int i = 0; i < namesList.Count; i++)
                {
                    Units.Add(new ColumnResultUnit() { ResultName = namesList[i] });
                }
            }
            #endregion
            #region 加载数据库配置
            #endregion
        }
        #endregion
        #region 保存到数据库
        #endregion
    }
}
