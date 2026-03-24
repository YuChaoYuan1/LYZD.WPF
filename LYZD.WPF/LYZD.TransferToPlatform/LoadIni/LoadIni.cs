using LYZD.TransferToPlatform.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.LoadIni
{
    public static class LoadIni
    {
        /// <summary>
        /// 台体注册码
        /// </summary>
        public static string SoftwareCode { get; set; }
        /// <summary>
        /// 设备id区分值，大于此值设备号1小于则0
        /// </summary>
        public static int DeviceID { get; set; }
        /// <summary>
        /// 表位数量
        /// </summary>
        public static int AllMeterNumber { get; set; }
        /// <summary>
        /// 485-1-端口号截至，从1开始
        /// </summary>
        public static int Port4851End { get; set; }
        /// <summary>
        /// 485-2-端口号截至，从最大485-1-端口号开始
        /// </summary>
        public static int Port4852End { get; set; }

        /// <summary>
        /// Can端口号截至，从最大485-2-端口号开始
        /// </summary>
        public static int PortCanEnd { get; set; }

        private static string DeviceDataPath = System.IO.Directory.GetCurrentDirectory() + "\\Ini\\TransferToPlatform.ini";
        public static bool LoadIniInfo()
        {
            try
            {
                SoftwareCode = ViewModel.Const.OperateFile.GetINI("Data", "SoftwareCode", DeviceDataPath);
                DeviceID =Convert.ToInt32(ViewModel.Const.OperateFile.GetINI("Data", "DeviceID", DeviceDataPath));
                AllMeterNumber = Convert.ToInt32(ViewModel.Const.OperateFile.GetINI("Data", "AllMeterNumber", DeviceDataPath));
                Port4851End = Convert.ToInt32(ViewModel.Const.OperateFile.GetINI("Data", "Port4851End", DeviceDataPath));
                Port4852End = Convert.ToInt32(ViewModel.Const.OperateFile.GetINI("Data", "Port4852End", DeviceDataPath));
                PortCanEnd = Convert.ToInt32(ViewModel.Const.OperateFile.GetINI("Data", "PortCanEnd", DeviceDataPath));
            }
            catch (Exception ex)
            {
                SoftwareCode = "0000000";
                DeviceID = 8;
                AllMeterNumber = 16;
                Port4851End = 16;
                Port4852End = 32;
                LogtestEven.add("载入台体注册码文件数据异常，文件位置："+ DeviceDataPath+"\n"+ex.ToString());
                return false;
            }
            return true;
        }
    }
}
