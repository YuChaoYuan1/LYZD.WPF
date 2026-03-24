using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.TerminalProtocol.GW
{
    /// <summary>
    /// 376.1协议当中所有数据项，数据项的格式，表示每个Fn当中的所有 子项，每个子项又是一坨值，解析的时候是一个 string
    /// </summary>
    public class FrameFnSubDataValue
    {
        /// <summary>
        /// Fn 数据项里面的ID
        /// </summary>
        public string FrameDataValue_ID
        {
            get;
            set;
        }

        /// <summary>
        /// Fn 数据项里面的长度
        /// </summary>
        public int FrameDataValue_Length
        {
            get;
            set;
        }

        /// <summary>
        /// 每个项对应的名字
        /// </summary>
        public string FrameDataValue_Name
        {
            get;
            set;
        }

        /// <summary>
        /// 一个Fn里面每一项，中的一项，
        /// </summary>
        public List<object> FrameDataValue_Value
        {
            get;
            set;
        }

        public Array ToArrayValue()
        {
            List<string> ValueList = new List<string>();

            string tempValueData = string.Empty;
            FrameDataValue_Value.ForEach((x) => { tempValueData += x; });

            ValueList.Add(tempValueData);
            return ValueList.ToArray();
        }

        public override string ToString()
        {
            string tempValueData = string.Empty;
            FrameDataValue_Value.ForEach((x) => { tempValueData += x; });
            return "[" + FrameDataValue_ID + "," + FrameDataValue_Name + ']' + tempValueData;
        }
    }
}
