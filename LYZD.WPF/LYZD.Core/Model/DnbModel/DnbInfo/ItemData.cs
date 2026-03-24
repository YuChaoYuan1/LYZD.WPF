using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Core.Model.DnbModel.DnbInfo
{
    /// <summary>
    /// 分项结论数据
    /// </summary>
    public class ItemData
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 检测时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 标准数据
        /// </summary>
        public string StandardData { get; set; }

        /// <summary>
        /// 终端数据
        /// </summary>
        public string TerminalData { get; set; }

        /// <summary>
        /// 结论
        /// </summary>
        public string Resoult { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Tips { get; set; }

        public ItemData(string str)
        {
            string[] data = str.Split('|');
            Name = data[0];
            Time = data[1];
            StandardData = data[2];
            TerminalData = data[3];
            Resoult = data[4];
            Tips = data[5];
        }
    }
}
