using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.ParameterSetAndSelect
{
    public class ParameterSetAndInfo : VerifyBase
    {
        public override void Verify()
        {
            base.Verify();
            StartVerify698();

            int ret = 0;
            ConnectLink(true);

            SetTime_698(DateTime.Now, 0);
            SetData_698_No("06 01 31 43 00 08 00 03 00 00", "禁用主动上报");

            #region 客户编号
            SetData_698("06012140030200090600000000000100", "设置客户编号");
            MessageAdd("读取客户编号", EnumLogType.流程信息);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                    Talkers[i].Framer698.sAPDU = "10000805010F" + "40030200" + "00" + "0110" + Talkers[i].Framer698.cOutRand;//读取客户编号
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "000000000001";
                    if (TalkResult[i] == 0)
                    {
                        Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.ucMac = GetMac(RecData, i);
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);
                        if (ret == 0)
                        {
                            TempData[i].Resoult = "合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "数据验证失败";
                        }

                        TempData[i].Data = GetData(RecData, i, 7, EnumTerimalDataType.e_string);
                        if (GetData(RecData, i, 7, EnumTerimalDataType.e_string) != "000000000001")
                            TempData[i].Resoult = "不合格";
                    }
                }
            }
            AddItemsResoult("客户编号", TempData);
            #endregion

            #region 读取时钟源
            MessageAdd("读取时钟源", EnumLogType.流程信息);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);

                    Talkers[i].Framer698.sAPDU = "10000805010F" + "40060200" + "00" + "0110" + Talkers[i].Framer698.cOutRand;//读取版本信息

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
                        Talkers[i].Framer698.ucMac = GetMac(RecData, i);// 11,EnumTerimalDataType.e_string);

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);

                        if (ret == 0)
                            TempData[i].Resoult = "合格";
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "数据验证失败";
                        }
                        TempData[i].Data = GetData(RecData, i, 7, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 8, EnumTerimalDataType.e_string);
                    }
                    else
                    {
                        TempData[i].Resoult = "不合格";
                        TempData[i].Tips = "无回复";
                    }
                }
            }

            AddItemsResoult("读取时钟源", TempData);
            #endregion

            #region 读取资产管理编码

            SetData_698("060128410302000A075347434D30303100", "设置资产管理编码");
            MessageAdd("读取资产管理编码", EnumLogType.流程信息);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);

                    Talkers[i].Framer698.sAPDU = "10000805010F" + "41030200" + "00" + "0110" + Talkers[i].Framer698.cOutRand;

                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);


            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "SGCM001";
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

                        string sTmp = GetData(RecData, i, 7, EnumTerimalDataType.e_string);
                        TempData[i].Data = sTmp;

                        if (sTmp != "SGCM001")
                            TempData[i].Resoult = "不合格";
                    }
                    else
                    {
                        TempData[i].Resoult = "不合格";
                        TempData[i].Tips = "无回复";
                    }
                }
            }
            AddItemsResoult("读取资产管理编码", TempData);
            #endregion

            #region 读取广播校时时间

            SetData_698("06012A4204020002021B173700030000", "设置广播校时时间");
            MessageAdd("读取广播校时时间", EnumLogType.流程信息);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);

                    Talkers[i].Framer698.sAPDU = "10000805010F" + "42040200" + "00" + "0110" + Talkers[i].Framer698.cOutRand;

                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "23:55:00";
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
                        TempData[i].Data = GetData(RecData, i, 7, EnumTerimalDataType.e_string);
                        if (GetData(RecData, i, 7, EnumTerimalDataType.e_string) != "23:55:00")
                            TempData[i].Resoult = "不合格";
                    }
                    else
                    {
                        TempData[i].Resoult = "不合格";
                        TempData[i].Tips = "无回复";
                    }
                }
            }
            AddItemsResoult("读取广播校时时间", TempData);
            #endregion

            #region 读取开关量输入
            SetData_698_No("06012CF2030400020204080004080000", "设置开关量输入");
            MessageAdd("读取开关量输入", EnumLogType.流程信息);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    setData[i] = Talkers[i].Framer698.ReadData_05("F2030400");
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "00000000,00000000";
                    if (TalkResult[i] == 0)
                    {
                        string sTmp = GetData(RecData, i, 5, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 6, EnumTerimalDataType.e_string);
                        TempData[i].Data = sTmp;

                        if (sTmp != "00000000,00000000")
                            TempData[i].Resoult = "不合格";
                    }
                    else
                    {
                        TempData[i].Tips = "无回复";
                        //MessageAdd("终端" + (i + 1) + "对时无回复！", EnumLogType.错误信息);
                        TempData[i].Resoult = "不合格";
                    }

                }
            }
            AddItemsResoult("读取开关量输入", TempData);
            #endregion

            #region 批量读取参数
            SetData_698("06022E03400002001C07E1071B0921384204020002021B17370003004500040002030A0B303030303030303030303101010A0B303030303030303030303201010A0B303030303030303030303300", "批量设置参数");
            MessageAdd("批量读取参数", EnumLogType.流程信息);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                    Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen("05022F0340000200420402004500040000 ".Replace(" ", "")) + "05022F0340000200420402004500040000".Replace(" ", "") + "0110" + Talkers[i].Framer698.cOutRand;
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "00000000001,00000000002,00000000003";
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

                        string sTmp = GetData(RecData, i, 15, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 16, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        TempData[i].Data = sTmp;
                        if (sTmp != "00000000001,00000000002,00000000003")
                            TempData[i].Resoult = "不合格";
                    }
                    else
                    {
                        TempData[i].Resoult = "不合格";
                        TempData[i].Tips = "无回复";
                    }
                }
            }
            AddItemsResoult("批量读取参数", TempData);
            #endregion

        }
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "客户编号", "禁用主动上报", "读取时钟源", "读取资产管理编码", "读取广播校时时间", "批量读取参数", "结论" };
            return true;
        }
    }
}
