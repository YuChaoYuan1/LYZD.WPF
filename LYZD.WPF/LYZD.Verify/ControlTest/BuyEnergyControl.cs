using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.ControlTest
{
    /// <summary>
    /// 购电控
    /// </summary>
    public class BuyEnergyControl : VerifyBase
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
            try
            {
                base.Verify();
                StartVerify698();

                bool[] bol_VerifyFlags = new bool[MeterNumber];
                for (int i = 0; i < MeterNumber; i++)
                {
                    bol_VerifyFlags[i] = true;
                }

                #region 购电控


                int ret = 0;
                ConnectLink(false);
                SetData_698("060119F1010200160100", "设置安全模式参数");
                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");
                SetData_698("070102230101000000", "清除总加组配置表");
                SetData_698("06010343000800030000", "关闭主动上报");
                SetTime_698(DateTime.Now, 0);
                ResetTerimal_698(2);
                ConnectLink2(false);

                DateTime dtHappen = DateTime.Now.AddMinutes(-1);//记录当前时间，事件发生的时间一定比这个晚

                DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 00:02:00").AddDays(1);

                ControlVirtualMeter("Cmd,DLS,0,0,0,0,0,0");
                ControlVirtualMeter("Cmd,Set,220,220,220,0,0,0,0,0,0,1,0");

                SetData_698_No("07 01 07 60 14 7F 00 01 01 02 06 11 01 12 01 00 02 02 11 00 00 01 02 5B 00 00 10 02 00 5B 00 20 04 02 00 5C 01 16 02 00", "下装普通采集方案");

                SetData_698_No("07 01 08 60 12 7F 00 01 01 02 0C 11 01 54 01 00 01 16 01 11 01 1C  " + Talkers[0].Framer698.SetDateTimeBCD(dttmp, false) + " 1C 08 33 09 09 09 09 09 54 00 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00", "下装采集任务");

                SetData_698("07010A2301030002035507050000000000011600160000", "添加总加配置单元");
                SetData_698("06010B23010F000408FF00", "设置总加组电控轮次配置");
                DateTime dtTmp = DateTime.Now;
                SetData_698("07010C81070300020850230106" + Core.Function.UsefulMethods.OrgBinFun(dtTmp.Minute + dtTmp.Hour * 100 + dtTmp.Day * 10000 + dtTmp.Month * 1000000, 4) + "160116001400000000000F4240140000000000030D40140000000000000000160000", "添加购电控配置单元");
                SetData_698("06010D8102020001081101110111011101110111011101110100", "设置功控告警时间");
                SetData_698("07011E800180000001" + Talkers[0].Framer698.SetDateTimeBCD(DateTime.Now, false) + "010005", "保电解除");

                SetData_698("07011F8103070050230100", "控制解除_时段功控_总加组1");
                SetData_698("0701208104070050230100", "控制解除_厂休控_总加组1");
                SetData_698("0701218105070050230100", "控制解除_营业报停控_总加组1");

                SetData_698("0701228106070050230100", "控制解除_当前功率下浮控_总加组1");
                SetData_698("0701228106070050230100", "控制解除_购电控_总加组1");
                SetData_698("0701238107070050230100", "控制解除_月电控_总加组1");

                MessageAdd("打开模拟表采集数据开关", EnumLogType.提示与流程信息, true);
                ControlVirtualMeter("MOP");
                WaitTime("打开模拟表采集数据开关", 5);

                SetTime_698(dttmp, 0);
                WaitTime("延时等待", 65);

                ControlVirtualMeter("MCL");
                WaitTime("关闭模拟表采集数据开关", 5);
                MessageAdd("召测终端总加组1当前控制状态", EnumLogType.流程信息, true);

                SetData_698("0701168107060050230100", "控制投入_购电控_总加组1");
                ControlVirtualMeter("Cmd,DLS,85,0,0,0,0,0");
                ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");

                MessageAdd("打开模拟表采集数据开关", EnumLogType.提示与流程信息, true);
                ControlVirtualMeter("MOP");
                WaitTime("打开模拟表采集数据开关", 5);


                WaitTime("延时等待", 240);

                //SetData_698("0701208104060050230100", "控制投入_时段功控_总加组1");


                #region 召测终端总加组1当前控制状态
                ControlVirtualMeter("MCL");
                WaitTime("关闭模拟表采集数据开关", 5);
                MessageAdd("召测终端总加组1当前控制状态", EnumLogType.流程信息, true);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "23011100" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "01000000";
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);

                            ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }

                            TempData[i].Data = GetData(RecData, i, 13, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 13, EnumTerimalDataType.e_string).Substring(1, 1) != "1")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "购电控未告警";
                            }

                        }
                    }
                }
                AddItemsResoult("召测终端总加组1当前控制状态", TempData);
                #endregion


                #region 召测终端总加组1当前控制状态

                ControlVirtualMeter("Cmd,DLS,105,0,0,0,0,0");
                ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
                ControlVirtualMeter("MOP");
                WaitTime("打开模拟表采集数据开关", 5);
                WaitTime("延时等待", 240);

                MessageAdd("关闭模拟表采集数据开关", EnumLogType.提示与流程信息, true);
                ControlVirtualMeter("MCL");
                WaitTime("关闭模拟表采集数据开关", 5);
                MessageAdd("召测终端总加组1当前控制状态", EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "23011100" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    TempData[i].StdData = "11110000";
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);
                            ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }
                            TempData[i].Data = GetData(RecData, i, 11, EnumTerimalDataType.e_string);

                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) == "00000000")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "购电控未跳闸";
                            }
                            else 
                            {
                                TempData[i].StdData = TempData[i].Data;
                            }

                        }
                    }
                }
                AddItemsResoult("召测终端总加组1当前控制状态", TempData);

                #endregion

                
                #region 召测终端总加组1当前控制状态

                SetData_698("0701198107070050230100", "控制解除_购电控_总加组1");

                WaitTime("延时等待", 60);

                MessageAdd("召测终端总加组1当前控制状态", EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "23011100" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    TempData[i].StdData = "00000000";
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);

                            ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);
                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }
                            TempData[i].Data = GetData(RecData, i, 13, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "00000000")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "购电控跳闸未解除";
                            }

                        }
                    }
                }
                AddItemsResoult("召测终端总加组1当前控制状态", TempData);
                #endregion
                #endregion
                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
