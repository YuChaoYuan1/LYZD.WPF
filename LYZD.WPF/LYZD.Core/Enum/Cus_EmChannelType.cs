using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Core.Enum
{
    /// <summary>
    /// 终端通讯类型
    /// </summary>
    public enum Cus_EmChannelType
    {
        /// <summary>
        /// 232通讯
        /// </summary>
        Channel232 = 0,
        /// <summary>
        /// GPRS
        /// </summary>
        ChannelGPRS = 1,
        /// <summary>
        /// 以太网
        /// </summary>
        ChannelEther = 2,
        /// <summary>
        /// 维护口
        /// </summary>
        ChannelMaintain = 3,
    }
}
