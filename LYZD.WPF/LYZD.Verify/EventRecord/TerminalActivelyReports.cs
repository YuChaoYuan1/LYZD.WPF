using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.EventRecord
{
    /// <summary>
    /// 终端主动上报
    /// </summary>
    public class TerminalActivelyReport : VerifyBase
    {
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify698();

                int ret = 0;

                ConnectLink(false);

                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 26 60 1c 81 00 00 00", "清空普通采集方案");
                SetData_698("06010843000800030000", "禁止终端主动上报");

                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

                SetTime_698(DateTime.Now, 0);

                ResetTerimal_698(2);

                ConnectLink2(false);

                SetData_698_No("07 01 2C 60 14 7F 00 01 01 02 06 11 03 12 00 FF 02 02 11 01 11 01 01 01 5B 01 50 04 02 00 02 00 10 02 00 00 20 02 00 5C 01 16 04 00", "下装普通采集方案");

                SetData_698_No("07 01 2D 60 1C 7F 00 01 02 02 05 11 01 01 01 51 45 10 00 00 54 00 00 14 11 03 02 02 11 00 51 40 00 02 00 02 05 11 02 01 01 51 45 10 00 00 54 00 00 14 11 03 02 02 11 01 02 03 51 60 12 03 00 60 01 01 50 04 02 00 02 00 10 02 00 00 20 02 00 5A 00 00", "下装上报方案");

                SetData_698_No("07 01 2E 60 12 7F 00 01 03 02 0C 11 01 54 03 00 01 16 04 11 01 1C 07 E7 0A 13 00 00 00 1C 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 02 0C 11 02 54 03 00 01 16 04 11 02 1C 07 E7 0A 13 00 00 00 1C 08 33 09 09 09 09 09 54 01 00 04 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 02 0C 11 03 54 03 00 01 16 01 11 03 1C 07 E7 0A 13 00 00 00 1C 08 33 09 09 09 09 09 54 01 00 02 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00", "下装采集任务");

                DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:00").AddDays(1);
                SetTime_698(dttmp, 0);

                MessageAdd("打开模拟表采集数据开关", EnumLogType.提示与流程信息, true);
                ControlVirtualMeter("MOP");


                WaitTime("等待虚拟表上报", 300);


                MessageAdd("关闭模拟表采集数据开关", EnumLogType.提示与流程信息, true);
                ControlVirtualMeter("MCL");

                WaitTime("关闭模拟表采集数据开关", 2);
            
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
