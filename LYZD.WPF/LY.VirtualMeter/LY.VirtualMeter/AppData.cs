
using LY.VirtualMeter.ViewModel;

namespace LY.VirtualMeter
{
    /// <summary>
    /// 全局数据类
    /// </summary>
    public class AppData
    {
       public static  string 加密机IP;
       public static string 加密机端口;

        private static BaseData baseData; 

        /// <summary>
        /// 基本信息
        /// </summary>
        public static BaseData BaseData
        {
            get
            {
                if (baseData == null)
                {
                    baseData = new BaseData();
                }
                return baseData;
            }
            set
            {
                baseData = value;
            }
        }
        private static LogViewModel logViewModel;

        /// <summary>
        /// 日志模型
        /// </summary>
        public static LogViewModel LogViewModel
        {
            get
            {
                if (logViewModel == null)
                {
                    logViewModel = new LogViewModel();
                }
                return logViewModel;
            }
            set
            {
                logViewModel = value;
            }
        }

        
    }
}
