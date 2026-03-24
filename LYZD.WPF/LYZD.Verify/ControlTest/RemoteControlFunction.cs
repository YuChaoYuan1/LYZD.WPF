using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.ControlTest
{
    /// <summary>
    /// 遥控功能
    /// </summary>
    public class RemoteControlFunction : VerifyBase
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

                int ret = 0;
                ConnectLink(false);

                SetTime_698(DateTime.Now, 0);


                SetSessionData_698("070104800180000001" + Talkers[0].Framer698.SetDateTimeBCD(DateTime.Now, false) + "010005", "下发保电解除命令");

                SetData_698("0602050231150800160031150900030100", "设置遥控事件有效");


                SetSessionData_698("07010B800081000101020451F20502011100120000030001" + Talkers[0].Framer698.SetDateTimeBCD(DateTime.Now, false) + "010005", "下发第1轮遥控跳闸命令");

                WaitTime("延时等待", 10);

                MessageAdd("召测终端遥控跳闸状态",EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "F2050201" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
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
                                TempData[i].Tips = "数据验证失败！";
                            }
                            TempData[i].Data = GetData(RecData, i, 8, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 8, EnumTerimalDataType.e_string) != "1")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "误差超限！";

                            }

                        }
                    }
                }
                AddItemsResoult("召测终端遥控跳闸状态", TempData);

                SetSessionData_698("07012C800081000101020451F20502011101120000030001" + Talkers[0].Framer698.SetDateTimeBCD(DateTime.Now, false) + "010005", "下发第1轮遥控跳闸命令");
                WaitTime("延时等待", 180);

                MessageAdd("召测终端遥控跳闸状态",EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "F2050201" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
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
                                TempData[i].Tips = "数据验证失败！";
                            }
                            TempData[i].Data = GetData(RecData, i, 8, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 8, EnumTerimalDataType.e_string) != "1")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "误差超限！";
                            }

                        }
                    }
                }
                AddItemsResoult("召测终端遥控跳闸状态", TempData);


                WaitTime("延时等待", 20);
                MessageAdd("召测终端遥控跳闸事件",EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen(" 05 03 0C 31 15 02 00 09 01 04 00 20 22 02 00 00 20 1E 02 00 00 20 20 02 00 00 20 24 02 00 00  ".Replace(" ", "")) + "05 03 0C 31 15 02 00 09 01 04 00 20 22 02 00 00 20 1E 02 00 00 20 20 02 00 00 20 24 02 00 00 ".Replace(" ", "") + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
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
                                TempData[i].Tips = "数据验证失败！";
                            }
                            if (GetDateTime(GetData(RecData, i, 14, EnumTerimalDataType.e_datetime)) < DateTime.Now.AddMinutes(-10))
                                TempData[i].Resoult = "不合格";
                            TempData[i].Data = "次数:" + GetData(RecData, i, 13, EnumTerimalDataType.e_int) + "," + "发生时间:" + GetData(RecData, i, 14, EnumTerimalDataType.e_datetime);

                        }
                    }
                }
                AddItemsResoult("召测终端遥控跳闸事件", TempData);

                SetSessionData_698("07012E800082000101020251F2050201160001" + Talkers[0].Framer698.SetDateTimeBCD(DateTime.Now, false) + "010005", "下发第1轮遥控合闸命令");

                MessageAdd("召测终端遥控跳闸事件",EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "F2050201" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "0";
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);

                            ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);
                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败！";
                            }
                            TempData[i].Data = GetData(RecData, i, 8, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 8, EnumTerimalDataType.e_string) != "0")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "误差超限";
                            }
                        }
                    }
                }
                AddItemsResoult("召测终端遥控跳闸状态", TempData);

            }
            catch (Exception ex)
            {
                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
