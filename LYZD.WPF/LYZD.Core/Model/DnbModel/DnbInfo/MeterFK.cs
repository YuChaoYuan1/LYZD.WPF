using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Core.Model.DnbModel.DnbInfo
{
    /// <summary>
    /// 费控数据
    /// </summary>
    [Serializable()]
    public class MeterFK : MeterBase
    {
        public MeterFK() : this("")

        { }
        public MeterFK(string itemType) : base()
        {
            Group = "";
            ItemType = itemType;
            Result = "";
            Data = "";
            Name = "";
        }


        /// <summary>
        /// 4组别
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 5项目类型
        /// </summary>
        public string ItemType { get; set; }
        /// <summary>
        /// 6结论
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 7数据
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 项目名称描述
        /// </summary>
        public string Name { get; set; }
    }
}
