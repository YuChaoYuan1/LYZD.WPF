using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LYZD.Verify.ControlTest
{

    /// <summary>
    ///  厂休功控
    /// </summary>
    public class FactoryHolidayControl : VerifyBase
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

                //deviceDriver.StopRemoteSignalOutput(bol_VerifyFlags, 0, 2);
                //Thread.Sleep(500);
                //deviceDriver.StopTest(GlobalUnit.g_TerminalVerifyFlags, 4);
                //Thread.Sleep(500);

                int ret = 0;
                ConnectLink(false);

                ResetTerimal_698(2);
                ConnectLink2(false);

                DateTime dtHappen = DateTime.Now.AddMinutes(-1);//记录当前时间，事件发生的时间一定比这个晚


                SetData_698("06011024010200090680008000800100", "设置脉冲计量1_通信地址");
                SetData_698("06011124010300020212000112000100", "设置脉冲计量1_互感器倍率");
                SetData_698("07011224010300020351F20A02011600120E1000", "添加脉冲输入单元");
                SetData_698("070113230101000000", "清除总加组配置表");
                SetData_698("0701142301030002035507058000800080011600160000", "添加总加配置单元");
                SetTime_698(DateTime.Now, 0);
                SetData_698("06011623010D00110100", "设置总加组滑差时间周期");
                SetData_698("06011723010E000408FF00", "设置总加组功控轮次配置");
                SetData_698("06011823010F000408FF00", "设置总加组电控轮次配置");
                SetData_698("0601198100020014000000000000138800", "设置终端保安定值");
                SetData_698("06011A81010200010C115511A511AA115511A511AA115511A511AA115511A511AA00", "设置终端功控时段");
                SetData_698("06011B8102020001081101110111011101110111011101110100", "设置功控告警时间");
                SetData_698("0701028104030002055023011400000000000013881CFFFFFFFF00020012000A04087F00", "添加厂休配置单元总加组1");
                //SetData_698("07011D81037F00020250230102020408FF110000", "时段功控方案切换");
                SetData_698("07011E800180000001" + Talkers[0].Framer698.SetDateTimeBCD(DateTime.Now, false) + "010005", "保电解除");
                SetData_698("07011F8103070050230100", "控制解除_时段功控_总加组1");
                SetData_698("0701208104070050230100", "控制解除_厂休控_总加组1");
                SetData_698("0701218105070050230100", "控制解除_营业报停控_总加组1");

                SetData_698("0701228106070050230100", "控制解除_当前功率下浮控_总加组1");
                SetData_698("0701228106070050230100", "控制解除_购电控_总加组1");
                SetData_698("0701238107070050230100", "控制解除_月电控_总加组1");
                DateTime dttmp = Convert.ToDateTime("2017-08-09 00:02:00");
                SetTime_698(dttmp, 0);

                WaitTime("延时等待", 65);
                SetData_698("07010B8104060050230100", "控制投入_厂休控_总加组1");

                //deviceDriver.StopRemoteSignalOutput(bol_VerifyFlags, 0, 2); Thread.Sleep(500);
                MessageAdd("台体输出脉冲信号。",EnumLogType.流程信息);
                //deviceDriver.StartPuleslOutput(bol_VerifyFlags, 2, 1, 2100, 0.584f, 4);
                SetPulseOutput(GetYaoJian(), 0x03, 0.584f, 4, 2100, 0.584f, 4, 2100);

                Thread.Sleep(500);

                bool[] bol_Qualified = new bool[MeterNumber];//合格标志

                for (int p1 = 1; p1 <= 4; p1++)
                {
                    if (CheckControPass(bol_Qualified))
                    {
                        MessageAdd("第" + p1 + "次召测终端总加组1当前控制状态", EnumLogType.流程信息, true);
                        for (int i = 0; i < MeterNumber; i++)
                        {
                            if (meterInfo[i].YaoJianYn)
                            {
                                ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                                Talkers[i].Framer698.sAPDU = "10000805010F" + "23011100" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                                setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                            }
                        }
                        WaitTime("等待", 40);

                        TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                        for (int i = 0; i < MeterNumber; i++)
                        {
                            if (meterInfo[i].YaoJianYn && !bol_Qualified[i])
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

                                    TempData[i].Data = "告警状态：" + GetData(RecData, i, 12, EnumTerimalDataType.e_string);

                                    if (GetData(RecData, i, 12, EnumTerimalDataType.e_string).Substring(1,1) != "1")
                                    {
                                        TempData[i].Resoult = "不合格";
                                        TempData[i].Tips = "厂休控未告警！";
                                    }
                                    if (Resoult[i] == "合格")
                                        bol_Qualified[i] = true;
                                }
                            }
                        }
                    }

                    
                }
                AddItemsResoult("召测终端总加组1当前控制状态", TempData);

                WaitTime("延时等待", 240);

                MessageAdd("召测终端总加组1当前控制状态",EnumLogType.流程信息, true);
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
                            TempData[i].Data = "跳闸状态：" + GetData(RecData, i, 9, EnumTerimalDataType.e_string);

                            if (GetData(RecData, i, 9, EnumTerimalDataType.e_string) == "00000000")
                            {
                               TempData[i].Resoult="不合格";
                                TempData[i].Tips = "厂休控未跳闸！";
                            }
                            else
                            {
                                TempData[i].StdData = TempData[i].Data;
                            }

                        }
                    }
                }
                AddItemsResoult("召测终端总加组1当前控制状态", TempData);


                SetData_698("0701208104070050230100", "控制解除_厂休控_总加组1");

                WaitTime("延时等待", 60);

                MessageAdd("召测终端总加组1当前控制状态",EnumLogType.流程信息, true);
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
                        TempData[i].StdData = "00000000";
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
                            TempData[i].Data = "告警状态：" + GetData(RecData, i, 12, EnumTerimalDataType.e_string);

                            if (GetData(RecData, i, 12, EnumTerimalDataType.e_string) != "00000000")
                            {
                                TempData[i].Resoult = "不合格";
                                //ResultDictionary["召测终端总加组1当前控制状态"][i] = "告警状态：" + GetData(RecData, i, 12, EnumTerimalDataType.e_string) + "|00000000";
                                TempData[i].Tips = "厂休控跳闸状态未解除！";
                            }
                        }
                    }
                }

                AddItemsResoult("召测终端总加组1当前控制状态", TempData);
                SetPulseOutputStop(GetYaoJian());

            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
