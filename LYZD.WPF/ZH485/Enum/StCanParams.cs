using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZH485.Enum
{
    /// <summary>
    /// Can卡通讯参数
    /// </summary>
    public class StCanParams
    {
        /// <summary>
        /// 通讯速率
        /// </summary>
        public string CanParams;
        /// <summary>
        /// 设备索引号
        /// </summary>
        public string DeviceIndex;
        /// <summary>
        /// 通道号
        /// </summary>
        public string CanId;

        /// <summary>
        /// Can版本类型
        /// </summary>
        public uint devType;

        /// <summary>
        /// 设备类型
        /// </summary>
        public Cus_EmDeviceType m_EmDeviceType;
    }
}
