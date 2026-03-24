using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.HPLCDeepening
{
    /// <summary>
    /// 卫星对时
    /// </summary>
    public class SatelliteTime : VerifyBase
    {
        public override void Verify()
        {
            base.Verify();
            ConnectLink(false);


            //6）读取终端校时模式（40000300），并保存；
            //7）配置校时模式（40000300）为主站校时;
            //8）主站给终端对时（40000200）到当日当前时间偏差5分钟；
            //9）配置校时模式（40000300）为北斗；
            //10）配置北向同步周期（40000600）为3分钟；
            //11）延时等待5分钟；
            //12）主站同步卫星时钟；
            //13）读取终端时间（40000200），检查终端时钟与系统时钟误差是否小于1秒；

        }
        protected override bool CheckPara()
        {
            //1）读取安全模式参数(F1010200)，如果未启用安全模式，则设置为启用；
            //2）读取ESAM序列号(F1000200)、对称密钥版本(F1000400)、计数器(F1000700)；
            //3）读取主站证书(F1000C00)；
            //4）读取终端证书(F1000A00)；
            //5）建立应用连接；
            //6）读取终端校时模式（40000300），并保存；
            //7）配置校时模式（40000300）为主站校时;
            //8）主站给终端对时（40000200）到当日当前时间偏差5分钟；
            //9）配置校时模式（40000300）为北斗；
            //10）配置北向同步周期（40000600）为3分钟；
            //11）延时等待5分钟；
            //12）主站同步卫星时钟；
            //13）读取终端时间（40000200），检查终端时钟与系统时钟误差是否小于1秒；
            //还原终端校时模式（40000300）。
            ResultNames = new string[] { "上1次终端初始化事件", "终端初始化事件当前记录数", "读取终端时钟", "恢复出厂默认参数", "终端对时", "结论" };
            return true;
        }
    }
}
