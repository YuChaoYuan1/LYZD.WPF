using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Core.Model.DnbModel.DnbInfo
{
    /// <summary>
    /// 终端检定结论的数据
    /// </summary>
    public class MeterItemResoultData
    {
        ///// <summary>
        ///// 分项结论列表_376
        ///// </summary>
        //public List<ItemData> ItemDatas376;
        ///// <summary>
        ///// 分项结论列表_698
        ///// </summary>
        //public List<ItemData> ItemDatas698;
        /// <summary>
        /// 分项结论列表
        /// </summary>
        public List<ItemData> ItemDatas=new List<ItemData>();

        /// <summary>
        /// 结论
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 检定数据
        /// </summary>
        public Dictionary<string, string> Datas=new Dictionary<string, string>();
    }

    public class MeterResoultData
    {
        public List<MeterItemResoultData> meterResoults = new List<MeterItemResoultData>();


        /// <summary>
        /// 结论
        /// </summary>
        public string Result = "合格";
    }
}
