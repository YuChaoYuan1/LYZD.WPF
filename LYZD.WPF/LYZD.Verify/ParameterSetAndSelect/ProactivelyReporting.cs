using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.ParameterSetAndSelect
{
    public class ProactivelyReporting : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "检定数据", "结论" };
            return true;
        }
        public override void Verify()
        {
            base.Verify();
            StartVerify698();
            //5
            ConnectLink(true);
            //6
            SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
            //7
            SetData_698_No("07 01 04 60 1C 81 00 00 00", "清空上报方案");
            //8
            SetData_698_No("07 01 04 43 00 08 00 03 01 00", "禁用终端主动上报");
            //9
            SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");
            //10
            DateTime dttmp = Convert.ToDateTime(DateTime.Now);
            SetTime_698(dttmp, 0);
            //11
            ResetTerimal_698(2);
            //13
            SetData_698_No("07 01 06 60 14 7f 00 01 01 02 06 11 01 12 01 00 02 02 11 01 11 01 01 01 5b 01 50 04 02 00 02 00 10 02 00 00 20 02 00 5c 01 16 04 00", "下装普通采集方案");
            //14--错误
            SetData_698_No("07 01 04 60 1C 81 00 00 00", "下装上报方案1");

            //15--错误
            SetData_698_No("07 01 04 60 1C 81 00 00 00", "下装上报方案2");
            //16
            SetData_698_No("07 01 07 60 12 7F 00 01 01 02 0C 11 01 54 03 00 01 16 01 11 01 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddDays(3), false) + " 1C 08 33 09 09 09 09 09 54 02 00 01 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00", "下装采集任务");

            //17
            SetData_698_No("07 01 07 60 12 7F 00 01 01 02 0C 11 01 54 03 00 01 16 04 11 01 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddDays(3), false) + " 1C 08 33 09 09 09 09 09 54 02 00 01 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00", "下装上报方案1任务");
            //18
            SetData_698_No("07 01 07 60 12 7F 00 01 01 02 0C 11 01 54 03 00 01 16 04 11 01 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddDays(3), false) + " 1C 08 33 09 09 09 09 09 54 02 00 01 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00", "下装上报方案2任务");

            dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:50").AddDays(2);
            //19
            SetTime_698(dttmp, 0);
            //20
            SetData_698_No("07 01 04 43 00 08 00 03 00 00", "允许终端主动上报");
            //21
            WaitTime("延时等待，", 360);

            dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:50").AddDays(4);
            //22
            SetTime_698(dttmp, 0);

            //23
            WaitTime("延时等待，", 300);
            //24
            SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
            //25
            SetData_698_No("07 01 04 60 1C 81 00 00 00", "清空上报方案");
            //26
            SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");
            //27
            dttmp = Convert.ToDateTime(DateTime.Now);
            SetTime_698(dttmp, 0);
            //28——错误
            SetData_698_No("07 01 06 60 14 7f 00 01 01 02 06 11 01 12 01 00 02 03 11 0f 11 01 01 01 5b 01 50 02 02 00 03 00 10 02 00 00 20 02 00  00 20 02 00 5c 01 16 06 00", "下装普通采集方案");
            //29
            dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 00:00:00");
            SetData_698_No("07 01 07 60 12 7F 00 01 01 02 0C 11 01 54 03 00 01 16 01 11 01 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddDays(1), false) + " 1C 08 33 09 09 09 09 09 54 02 00 01 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00", "下装采集任务");
            //30--错误
            SetData_698_No("07 01 04 60 1C 81 00 00 00", "下装上报方案1");
            //31--错误
            SetData_698_No("07 01 04 60 1C 81 00 00 00", "下装上报方案1");
            //32
            dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:56:00");
            SetTime_698(dttmp.AddDays(1), 0);
            //33
            WaitTime("延时等待，", 480);
            //34
            dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 02:59:50");
            SetTime_698(dttmp.AddDays(2), 0);
            //35
            WaitTime("延时等待，", 240);
            //36
            dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 05:59:00");
            SetTime_698(dttmp.AddDays(2), 0);
            //37
            WaitTime("延时等待，", 240);
            SetData_698_No("07 01 04 43 00 08 00 03 01 00", "禁用终端主动上报");
        }
    }
}
