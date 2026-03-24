using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Core.Enum
{
    /// <summary>
    /// 不同协议的详细结论分割的关键字字符串
    /// </summary>
    public class ItemResoultkeyword
    {
        /// <summary>
        /// 376分割开始
        /// </summary>
        public const string splite_Statr_376 = "<376";
        /// <summary>
        /// 376分割结束
        /// </summary>
        public const string splite_End_376 = "376>";
        /// <summary>
        /// 698分割开始
        /// </summary>
        public const string splite_Statr_698 = "<698";
        /// <summary>
        /// 698分割结束
        /// </summary>
        public const string splite_End_698 = "698>";
        /// <summary>
        /// 104分割开始
        /// </summary>
        public const string splite_Statr_104 = "<104";
        /// <summary>
        /// 104分割结束
        /// </summary>
        public const string splite_End_104 = "104>";

        public static List<string> GetSpliteList()
        {
            List<string> list = new List<string>();
            list.Add(splite_Statr_376);
            list.Add(splite_End_376);
            list.Add(splite_Statr_698);
            list.Add(splite_End_698);
            list.Add(splite_Statr_104);
            list.Add(splite_End_104);
            return list;
        }


    }
}
