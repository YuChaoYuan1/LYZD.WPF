using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.EventRecord
{
    /// <summary>
    /// 电能表数据变更监控记录
    /// </summary>
    public class MeterMonitorEvent : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "电能表数据变更监控记录", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify698();

                int ret = 0;
                ConnectLink(false);
                SetTime_698(DateTime.Now, 0);

                //ResetTerimal_698(2);
                //ConnectLink2(false);
                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");
                DateTime dtHappen = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 09:19:50"); ;//记录当前时间，事件发生的时间一定比这个晚

                SetData_698("06011D311C0900030100", "设置事件有效标志");
                SetData_698("06011E311C06000201110100", "设置电能表数据变更监控记录配置参数");

                ControlVirtualMeter("Cmd,YGZTZ,00");

                WaitTime("打开模拟表采集数据开关，", 2);

                MessageAdd("打开模拟表采集数据开关", EnumLogType.提示与流程信息, true);
                ControlVirtualMeter("MOP");

                ///配置模拟表
                ResetTerimal_698(2);
                ConnectLink2(false);


                //SetTime_698(DateTime.Now, 60);
                SetData_698_No("07 01 21 60 14 7F 00 01 01 02 06 11 01 12 01 00 02 02 11 00 00 01 01 5B 00 41 12 02 00 5C 01 16 01 00", "下装普通采集方案");
                SetData_698_No("07 01 22 60 12 7F 00 01 01 02 0C 11 01 54 01 00 05 16 01 11 01 1C 07 E1 08 04 00 00 00 1C 08 33 09 09 09 09 09 54 00 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00 ", "下装采集任务");
                DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 09:15:05");
                SetTime_698(dttmp, 0);
                WaitTime("等待事件发生,", 180);

                ControlVirtualMeter("Cmd,YGZTZ,A0");
                dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 09:19:50");
                SetTime_698(dttmp, 0);
                WaitTime("等待事件发生,", 180);
                MessageAdd("关闭模拟表采集数据开关", EnumLogType.提示与流程信息, true);
                ControlVirtualMeter("MCL");

                WaitTime("关闭模拟表采集数据开关", 2);
                MessageAdd("读取上一次电能表数据变更监控记录",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen("05 01 25 31 1C 02 01 00 ".Replace(" ", "")) + "05 01 25 31 1C 02 01 00 ".Replace(" ", "") + "0110" + Talkers[i].Framer698.cOutRand;

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = dtHappen.ToString();
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);// 19,  EnumTerimalDataType.e_string);
                            ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);
                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }

                            if (GetDateTime(GetData(RecData, i, 8, EnumTerimalDataType.e_datetime)) < dtHappen || !GetData(RecData, i, "41120200") || !GetData(RecData, i, "00000000") || !GetData(RecData, i, "10100000"))
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Data = "次数:" + GetData(RecData, i, 7, EnumTerimalDataType.e_int) + "," + "发生时间:" + GetData(RecData, i, 8, EnumTerimalDataType.e_datetime);

                            }
                            else
                                TempData[i].Data = "次数:" + GetData(RecData, i, 7, EnumTerimalDataType.e_int) + "," + "发生时间:" + GetData(RecData, i, 8, EnumTerimalDataType.e_datetime) + ",41120200,00000000,10100000";

                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("电能表数据变更监控记录", TempData);



            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
