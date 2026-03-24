using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LYZD.Verify.DataCollect
{
    /// <summary>
    /// 分时段电能量数据存储
    /// </summary>
    public class PeriodStorage : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "正向有功总电能示值", "费率1正向有功电能示值", "费率2正向有功电能示值", "费率3正向有功电能示值", "费率4正向有功电能示值", "结论" };
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
                SetData_698_No("07 01 04 43 00 08 00 03 01 00", "禁用终端主动上报");

                SetTime_698(DateTime.Now, 0);
                ResetTerimal_698(2);
               // ConnectLink2(false);

                SetData_698("06012524010200090680008000800100", "设置脉冲计量1_通信地址");
                SetData_698("06012624010300020212000112000100", "设置脉冲计量1_互感器倍率");
                SetData_698("07012724010300020351F20A020116001203E800", "添加脉冲输入单元");
                SetData_698("060101400C020002051101110111041104110500", "设置时区时段数");
                SetData_698("0601024016020001010104020311001100110102031106110011020203110C11001103020311121100110400", "设置当前套日时段表");


                DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day+" 00:15:00");
                SetTime_698(dttmp, 0);
                WaitTime("等待", 5);

                MessageAdd("台体输出脉冲信号", EnumLogType.流程信息);

                SetPulseOutput(GetYaoJian(), 0x03, 0.98f, 0.5f, 241, 0.98f, 0.5f, 241);
                Thread.Sleep(500);
                WaitTime("等待", 330);

                MessageAdd("台体停止脉冲信号", EnumLogType.流程信息);
                SetPulseOutputStop(GetYaoJian());
                WaitTime("等待", 5);

                dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 06:15:00");
                SetTime_698(dttmp, 0);
                WaitTime("等待", 5);


                SetPulseOutput(GetYaoJian(), 0x03, 1.06f, 0.5f, 256, 1.06f, 0.5f, 256);
                Thread.Sleep(500);
                WaitTime("等待", 330);

                MessageAdd("台体停止脉冲信号", EnumLogType.流程信息);
                SetPulseOutputStop(GetYaoJian());
                WaitTime("等待", 5);


                dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 12:15:00");
                SetTime_698(dttmp, 0);
                WaitTime("等待", 5);


                SetPulseOutput(GetYaoJian(), 0x03, 1.2f, 0.5f, 288, 0.82f, 0.5f, 288);
                Thread.Sleep(500);
                WaitTime("等待", 330);

                MessageAdd("台体停止脉冲信号", EnumLogType.流程信息);
                SetPulseOutputStop(GetYaoJian());
                WaitTime("等待", 5);

                dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 18:15:00");
                SetTime_698(dttmp, 0);
                WaitTime("等待", 5);


                SetPulseOutput(GetYaoJian(), 0x03, 0.89f, 0.5f, 215, 0.89f, 0.5f, 215);
                Thread.Sleep(500);
                WaitTime("等待", 330);
                

                MessageAdd("读取脉冲电能量数据",EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "24010700" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "1";
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
                            TempData[i].Data = (double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10000).ToString();
                            if (double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) / 10000 != 1)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "误差超限！";
                            }

                        }
                    }
                }
                AddItemsResoult("正向有功总电能示值", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "0.241";
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
                            TempData[i].Data = (double.Parse(GetData(RecData, i, 8, EnumTerimalDataType.e_double)) / 10000).ToString();

                            if (double.Parse(GetData(RecData, i, 8, EnumTerimalDataType.e_double)) / 10000 != 0.241)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "误差超限！";
                            }

                        }
                    }
                }
                AddItemsResoult("费率1正向有功电能示值", TempData);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "0.256";
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
                            TempData[i].Data = (double.Parse(GetData(RecData, i, 9, EnumTerimalDataType.e_double)) / 10000).ToString();

                            if (double.Parse(GetData(RecData, i, 9, EnumTerimalDataType.e_double)) / 10000 != 0.256)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "误差超限！";
                            }

                        }
                    }
                }
                AddItemsResoult("费率2正向有功电能示值", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "0.288";
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
                            TempData[i].Data = (double.Parse(GetData(RecData, i, 10, EnumTerimalDataType.e_double)) / 10000).ToString();

                            if (double.Parse(GetData(RecData, i, 10, EnumTerimalDataType.e_double)) / 10000 != 0.288)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "误差超限！";
                            }

                        }
                    }
                }
                AddItemsResoult("费率3正向有功电能示值", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "0.215";
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
                            TempData[i].Data = (double.Parse(GetData(RecData, i, 11, EnumTerimalDataType.e_double)) / 10000).ToString();

                            if (double.Parse(GetData(RecData, i, 11, EnumTerimalDataType.e_double)) / 10000 != 0.215)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "误差超限！";
                            }

                        }
                    }
                }
                AddItemsResoult("费率4正向有功电能示值", TempData);
                MessageAdd("开始恢复被检表时间", EnumLogType.提示信息);
                dttmp = DateTime.Now;
                SetTime_698(dttmp, 0);
                WaitTime("等待", 10);
            }
            catch (Exception ex)
            {
                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
