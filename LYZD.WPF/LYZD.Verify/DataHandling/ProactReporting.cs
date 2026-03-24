using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.DataHandling
{
    public class ProactReporting : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "启用主动上报", "验证数据", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {

                base.Verify();
                StartVerify698();

                ConnectLink(true);
                ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
                ControlVirtualMeter("Cmd,DLS,5000,4000,1000,1000,1000,1000");
                SetTime_698(DateTime.Now, 0);
                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");
                ControlVirtualMeter("MCL");
                SetData_698_No("07 01 2C 60 14 7F 00 01 01 02 06 11 03 12 00 FF 02 02 11 01 11 01 01 01 5B 01 50 04 02 00 02 00 10 02 00 00 20 02 00 5C 01 16 04 00", "下装普通采集方案");
                SetData_698_No("07 01 2D 60 1C 7F 00 01 02 02 05 11 01 01 01 51 45 10 00 00 54 00 00 14 11 03 02 02 11 00 51 40 00 02 00 02 05 11 02 01 01 51 45 10 00 00 54 00 00 14 11 03 02 02 11 01 02 03 51 60 12 03 00 60 01 01 50 04 02 00 02 00 10 02 00 00 20 02 00 5A 00 00", "下装上报方案");

                DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 00:00:00");
                //SetData_698_No("07 01 2E 60 12 7F 00 01 03 02 0C 11 01 54 03 00 01 16 04 11 01 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp, false) + " 1C 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 02 0C 11 02 54 03 00 01 16 04 11 02 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp, false) + " 1C 08 33 09 09 09 09 09 54 01 00 04 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 02 0C 11 03 54 03 00 01 16 01 11 03 " + Talkers[0].Framer698.SetDateTimeBCD(dttmp, false) + " 1C 08 33 09 09 09 09 09 54 01 00 02 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00", "下装采集任务");
                SetData_698_No("07 01 2e 60 12 7f 00 01 03 02 0c 11 01 54 03 00 01 16 04 11 01 1c 07 e8 01 04 00 00 00 1c 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3b 02 0c 11 02 54 03 00 01 16 04 11 02 1c 07 e8 01 04 00 00 00 1c 08 33 09 09 09 09 09 54 01 00 04 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3b 02 0c 11 03 54 03 00 01 16 01 11 03 1c 07 e8 01 04 00 00 00 1c 08 33 09 09 09 09 09 54 01 00 02 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3b 00", "下装采集任务");

                DateTime dttmp1 = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:00");
                SetTime_698(dttmp1, 0);
                WaitTime("延时，", 30);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Report.Clear();
                    }
                }
                SetData_698_No("06 01 30 43 00 0A 00 01 02 51 45 00 00 00 51 45 10 00 00 00", "设置上报通道");
                SetData_698_No("06 01 31 43 00 08 00 03 01 00", "启用主动上报");

                ControlVirtualMeter("MOP");
                isopenRep = true;

                WaitTime("延时，", 360);
                SetData_698_No("06 01 31 43 00 08 00 03 00 00", "禁用主动上报");
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        List<string> newRep = Talkers[i].Report;
                        foreach (string str in newRep)
                        {
                            string[] AnalysisedString = new string[0];
                            string AlalysisedData = null;
                            string[] AnalysisedStruce = new string[0];
                            if (Talkers[i].Analysiser698.Analysis(str, ref AnalysisedString, ref AlalysisedData, ref AnalysisedStruce)) {
                                if (AnalysisedString[3].Contains("Afn = 88,上报")) {
                                    TempData[i].Data = AnalysisedString[8];
                                    TempData[i].Resoult = "合格";
                                    break;
                                }
                                else
                                {
                                    TempData[i].Resoult = "不合格";
                                }
                            }
                        }
                        
                    }
                }
                AddItemsResoult("验证数据", TempData);
            }
            catch (Exception ex)
            {
                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
            finally
            {
                isopenRep = false;
                ControlVirtualMeter("MCL");

            }


        }
    }
}
