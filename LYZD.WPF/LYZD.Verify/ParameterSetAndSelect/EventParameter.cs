using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.ParameterSetAndSelect
{
    /// <summary>
    /// 事件参数
    /// </summary>
    public class EventParameter : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "电能表时钟超差事件阈值", "电能表时钟超差事件阈值", "终端停上电事件参数",
                "电能量超差事件阈值","电能表飞走事件阈值","电能表停走事件阈值","月通信流量超限事件阈值",
                "终端停电事件甄别限值参数","结论" };
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
                SetData_6982("060119F1010200160100", "设置安全模式参数启用");

                #region 读取电能表时钟超差事件阈值
                SetData_698("060100310506000202120001110100", "设置电能表时钟超差事件阈值");
                MessageAdd("读取电能表时钟超差事件阈值", EnumLogType.提示信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "31050600" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "1,1";
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

                            string stmp = GetData(RecData, i, 7, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 8, EnumTerimalDataType.e_string);
                            TempData[i].Data = stmp;
                            if (stmp != "1,1")
                                TempData[i].Resoult = "不合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("电能表时钟超差事件阈值", TempData);
                #endregion

                #region 读取终端停上电事件参数
                SetData_698("0601023106060002020204040800110111010101550705000000000001020612000112000212000312000412000512000600", "设置终端停上电事件参数");
                MessageAdd("读取终端停上电事件参数",EnumLogType.提示信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "31060600" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
           
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "00000000,1";
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2,  EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);


                            ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }
                            string stmp = GetData(RecData, i, 7,  EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 8,  EnumTerimalDataType.e_string);

                            TempData[i].Data = stmp;
                            if (stmp != "00000000,1")
                               TempData[i].Resoult="不合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("终端停上电事件参数", TempData);
                #endregion

                #region 读取电能量超差事件阈值
                SetData_698("060104310C060002020600000001110100", "设置电能量超差事件阈值");
                MessageAdd("读取电能量超差事件阈值",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "310C0600" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
           
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "1,1";
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2,  EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);

                            ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);
                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }
                            string stmp = GetData(RecData, i, 7,  EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 8,  EnumTerimalDataType.e_string);
                            TempData[i].Data = stmp ;
                            if (stmp != "1,1")
                               TempData[i].Resoult="不合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("电能量超差事件阈值", TempData);
                #endregion

                #region 读取电能表飞走事件阈值
                SetData_698("060106310D060002020600000001110100", "设置电能表飞走事件阈值");
                MessageAdd("读取电能表飞走事件阈值",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "310D0600" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "1,1";
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2,  EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);

                            ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }

                            string stmp = GetData(RecData, i, 7,  EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 8,  EnumTerimalDataType.e_string);
                            TempData[i].Data = stmp;
                            if (stmp != "1,1")
                               TempData[i].Resoult="不合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("电能表飞走事件阈值", TempData);
                #endregion

                #region 读取电能表停走事件阈值
                SetData_698("060108310E0600020254010002110100", "设置电能表停走事件阈值");
                MessageAdd("读取电能表停走事件阈值",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "310E0600" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "2,1";
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2,  EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);

                            ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }

                            string stmp = GetData(RecData, i, 7,  EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 8,  EnumTerimalDataType.e_string);

                            TempData[i].Data = stmp;
                            if (stmp != "2,1")
                               TempData[i].Resoult="不合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("电能表停走事件阈值", TempData);
                #endregion

                #region 读取月通信流量超限事件阈值
                SetData_698("06010A311006000201067777777700", "设置月通信流量超限事件阈值");
                MessageAdd("读取月通信流量超限事件阈值",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "31100600" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
           
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "2004318071";
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2,  EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);

                            ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);
                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }

                            string stmp = GetData(RecData, i, 7,  EnumTerimalDataType.e_string);

                            TempData[i].Data = stmp ;
                            if (stmp != "2004318071")
                               TempData[i].Resoult="不合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("月通信流量超限事件阈值", TempData);
                #endregion

                #region 读取终端停电数据采集配置参数
                SetData_698("06010C31060601020404080011011101010155070500000000000100", "设置终端停电数据采集配置参数");
                MessageAdd("读取终端停电数据采集配置参数",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "31060601" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
           
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "00000000,1,1";
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2,  EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);
                            ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);
                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }
                            string stmp = GetData(RecData, i, 7,  EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 8,  EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 9,  EnumTerimalDataType.e_string);
                            TempData[i].Data = stmp ;
                            if (stmp != "00000000,1,1")
                               TempData[i].Resoult="不合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("终端停电数据采集配置参数", TempData);
                #endregion

                #region 读取终端停电事件甄别限值参数
                SetData_698("06010E31060602020612000112000212000312000412000512000600", "设置终端停电事件甄别限值参数");
                MessageAdd("读取终端停电事件甄别限值参数", EnumLogType.提示信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "31060602" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
           
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "1,2";
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2,  EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);
                            ret =TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);
                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";
                            }
                            string stmp = GetData(RecData, i, 7,  EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 8,  EnumTerimalDataType.e_string);
                            TempData[i].Data = stmp ;
                            if (stmp != "1,2")
                               TempData[i].Resoult="不合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("终端停电事件甄别限值参数", TempData);
                #endregion



            }
            catch (Exception ex)
            {
                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }


        }
    }
}
