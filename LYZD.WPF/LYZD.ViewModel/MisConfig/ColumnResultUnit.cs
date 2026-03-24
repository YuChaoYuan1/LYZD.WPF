using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.ViewModel.MisConfig
{
    /// <summary>
    /// 列与显示名称单元
    /// </summary>
    public class ColumnResultUnit : ViewModelBase
    {
        private string columnName;
        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName
        {
            get { return columnName; }
            set { SetPropertyValue(value, ref columnName, "ColumnName"); }
        }
        private string resultName;
        /// <summary>
        /// 结论名称
        /// </summary>
        public string ResultName
        {
            get { return resultName; }
            set { SetPropertyValue(value, ref resultName, "ResultName"); }
        }
    }
}
