using ZH.MeterProtocol.Enum;
using ZH.MeterProtocol.Struct;

namespace ZH.MeterProtocol
{
   public class App
    {
        /// <summary>
        ///  载波协议信息
        /// </summary>
        public static CarrierWareInfo CarrierInfo = new CarrierWareInfo();
        /// <summary>
        /// 当前载波配置
        /// </summary>
        public static CarrierWareInfo[] CarrierInfos = null;
        /// <summary>
        /// 载波当前表位
        /// </summary>
        public static int Carrier_Cur_BwIndex = 0;

        /// <summary>
        /// 通讯类型
        /// </summary>
        public static Cus_ChannelType g_ChannelType = Cus_ChannelType.通讯485;
        /// <summary>
    }
}
