using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Core.Model.DnbModel.DnbInfo
{
    /// <summary>
    /// 多功能检定数据
    /// </summary>
    [Serializable()]
    public class MeterDgn : MeterBase
    {
        public MeterDgn() : this("")
        { }

        public MeterDgn(string priId)
            : base()
        {
            PrjID = priId;
            Name = "";
            Value = "";
            Result = "";
        }


        /// <summary>
        /// 多功能项目ID	
        /// </summary>
        public string PrjID { get; set; }
        /// <summary>
        /// 项目名称描述
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 项目值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 6结论Y/N
        /// </summary>
        public string Result { get; set; }


        /// <summary>
        /// 其他参数
        /// </summary>
        public string TestValue { get; set; }

    }
}
