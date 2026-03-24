using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.IdentityAndKeyAgreement
{
    /// <summary>
    /// 密钥恢复
    /// </summary>
    public class KeyRecovery_376 : VerifyBase
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
            //try
            //{
            //    base.Verify();
            //    StartVerify();
            //    for (int tableNo = 0; tableNo < MeterNumber; tableNo++)
            //    {
            //        meterInfo[tableNo].YaoJianYn = false;
            //    }
            //    for (int tableNo = 0; tableNo < MeterNumber; tableNo++)
            //    {
            //        if (bol_TemporaryIsVerify[tableNo])
            //        {
            //            meterInfo[tableNo].YaoJianYn = true;

            //            SubItemIndex = 1;
            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.ConnectDevice(CLOU_Comm.GlobalUnit.g_SystemConfig.SystemMode.getValue("EncryptionMachineIp"), CLOU_Comm.GlobalUnit.g_SystemConfig.SystemMode.getValue("EncryptionMachinePort"), "10");

            //            if (iResult != 0)//czx0723
            //            {
            //                 MessageAdd("连接加密机失败！返回" + iResult, DateTime.Now.ToString());
            //                for (int tableNo2 = 0; tableNo2 < MeterNumber; tableNo2++)
            //                {
            //                    CLOU_Model.TerminalModels.TerminalInfos[tableNo2 + 1].IsVerify = bol_TemporaryIsVerify[tableNo2];
            //                }
                            
            //                return;
            //            }

            //             MessageAdd("启用硬件加密",EnumLogType.错误信息);
            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 5, new byte[] { 0xff, 0x00, 0x00 }, RecData, MaxWaitSeconds_Write);
            //            TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 5, RecData, MaxWaitSeconds_Write);
                        
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        if (GetData(RecData, i, 3, EnumTerimalDataType.e_string) != "255")
            //                        {
            //                             MessageAdd("关闭硬件认证失败",EnumLogType.错误信息);
            //                           TempData[i].Resoult="不合格";
            //                        }
            //                        ResultDictionary[""][i] = GetData(RecData, i, 3, EnumTerimalDataType.e_string) + "|255";
            //                    }
            //                    else
            //                    {
            //                         MessageAdd("关闭硬件认证失败",EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("启用硬件加密", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);


            //             MessageAdd("获取ESAM密钥信息",EnumLogType.错误信息);
            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 11, new byte[] { }, RecData, MaxWaitSeconds_Write);
                        
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        strTESAMNo = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
            //                        strCLR = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
            //                        strCount = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
            //                        strState = GetData(RecData, i, 6, EnumTerimalDataType.e_string);
            //                        strVersionNum = GetData(RecData, i, 7, EnumTerimalDataType.e_string);
            //                        ResultDictionary[""][i] = "ESAM序列号:" + strTESAMNo;
            //                    }
            //                    else
            //                    {
            //                        MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //            KeyVerion = strVersionNum == "0000000000000000" ? "00" : "01";
            //            strState = strState.PadLeft(2, '0');
            //             RefUIData("获取ESAM密钥信息", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);



            //            #region 芯片版本和密钥版本不一致的时候需要把芯片证书切换过来
            //            if ((KeyVerion == "00" && strState == "01") || (KeyVerion == "01" && strState == "00"))
            //            {
            //                 MessageAdd("内部认证",EnumLogType.错误信息);
            //                iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_GetR1(strBu1);
            //                strR4 = strBu1.ToString().Replace("\0", "");

            //                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 17, Core.Function.UsefulMethods.ConvertStringToBytes(UsefulMethods.revStr(strR4)), RecData, MaxWaitSeconds_Write);
                            
            //                for (int i = 0; i < MeterNumber; i++)
            //                {
            //                    if (meterInfo[i].YaoJianYn)
            //                    {
            //                        if (TalkResult[i] == 0)
            //                        {
            //                            strMR4 = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
            //                            strR5 = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
            //                            ResultDictionary[""][i] = "R4密文:" + strMR4;
            //                        }
            //                        else
            //                        {
            //                            MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
            //                           TempData[i].Resoult="不合格";
            //                        }
            //                    }
            //                }
            //                 RefUIData("内部认证", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);

            //                 MessageAdd("外部认证",EnumLogType.错误信息);
            //                iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_ExternalAuth(KeyVerion, strTESAMNo, strR4, strMR4, strR5, strBu1);
            //                strMR5 = strBu1.ToString().Replace("\0", "");

            //                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 18, Core.Function.UsefulMethods.ConvertStringToBytes(UsefulMethods.revStr(strMR5)), RecData, MaxWaitSeconds_Write);
                            
            //                for (int i = 0; i < MeterNumber; i++)
            //                {
            //                    if (meterInfo[i].YaoJianYn)
            //                    {
            //                        if (TalkResult[i] == 0)
            //                        {
            //                            strR6 = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
            //                            ResultDictionary[""][i] = "随机数R6:" + strR6;
            //                        }
            //                        else
            //                        {
            //                            MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
            //                           TempData[i].Resoult="不合格";
            //                        }
            //                    }
            //                }
            //                 RefUIData("外部认证", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);


            //                 MessageAdd("证书状态切换",EnumLogType.错误信息);
            //                iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_CertificateStateChange(KeyVerion, strTESAMNo, KeyVerion, strR6, strBu1, strBu2);
            //                strMR6 = strBu1.ToString().Replace("\0", "");
            //                OutMac = strBu2.ToString().Replace("\0", "");
            //                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 19, Core.Function.UsefulMethods.ConvertStringToBytes((KeyVerion == "01" ? "00" : "01") + Core.Function.UsefulMethods.revStr(strMR6) + Core.Function.UsefulMethods.revStr(OutMac)), RecData, MaxWaitSeconds_Write);
                            
            //                for (int i = 0; i < MeterNumber; i++)
            //                {
            //                    if (meterInfo[i].YaoJianYn)
            //                    {
            //                        if (TalkResult[i] == 0)
            //                        {
            //                            if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
            //                            {
            //                                 MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                               TempData[i].Resoult="不合格";
            //                            }
            //                        }
            //                        else
            //                        {
            //                             MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                           TempData[i].Resoult="不合格";
            //                        }
            //                    }
            //                }
            //                 RefUIData("证书状态切换", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);

            //                 MessageAdd("获取ESAM密钥信息",EnumLogType.错误信息);
            //                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 11, new byte[] { }, RecData, MaxWaitSeconds_Write);
                            
            //                for (int i = 0; i < MeterNumber; i++)
            //                {
            //                    if (meterInfo[i].YaoJianYn)
            //                    {
            //                        if (TalkResult[i] == 0)
            //                        {
            //                            strTESAMNo = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
            //                            strCLR = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
            //                            strCount = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
            //                            strState = GetData(RecData, i, 6, EnumTerimalDataType.e_string);
            //                            strVersionNum = GetData(RecData, i, 7, EnumTerimalDataType.e_string);
            //                            ResultDictionary[""][i] = "ESAM序列号:" + strTESAMNo;
            //                        }
            //                        else
            //                        {
            //                            MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
            //                           TempData[i].Resoult="不合格";
            //                        }
            //                    }
            //                }
            //                KeyVerion = strVersionNum == "0000000000000000" ? "00" : "01";
            //                strState = strState.PadLeft(2, '0');
            //            }
            //            else
            //            {
                            
            //                 RefUIData("内部认证", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);
            //                 RefUIData("外部认证", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);
            //                 RefUIData("证书状态切换", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);
            //            }
            //            #endregion

            //             MessageAdd("获取随机数",EnumLogType.错误信息);
            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_GetR1(strBu1);
            //            strR1 = strBu1.ToString().Replace("\0", "");



            //             MessageAdd("会话初始化",EnumLogType.错误信息);
            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_SessionInitRec(KeyVerion, strTESAMNo, "01", "00", strR1, strBu1, strBu2, strBu3, strBu4);

            //            strOutCertificate = strBu1.ToString().Replace("\0", "");
            //            OutEncR1 = strBu2.ToString().Replace("\0", "");
            //            OutMac = strBu3.ToString().Replace("\0", "");
            //            OutSign1 = strBu4.ToString().Replace("\0", "");

            //            ileng = 2 + (strOutCertificate + OutEncR1 + OutMac + OutSign1).Length / 2;
            //            sleng = (ileng % 256).ToString("x2") + (ileng / 256).ToString("x2");

            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 12, Core.Function.UsefulMethods.ConvertStringToBytes(sleng + "0100" + Core.Function.UsefulMethods.revStr(strOutCertificate) + Core.Function.UsefulMethods.revStr(OutEncR1) + Core.Function.UsefulMethods.revStr(OutMac) + Core.Function.UsefulMethods.revStr(OutSign1)), RecData, MaxWaitSeconds_Write);
                        


            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        clr = GetData(RecData, i, 6, EnumTerimalDataType.e_string);
            //                        zz_num = GetData(RecData, i, 7, EnumTerimalDataType.e_string);
            //                        zd_OutCertificate = GetData(RecData, i, 8, EnumTerimalDataType.e_string);
            //                        strR2 = GetData(RecData, i, 9, EnumTerimalDataType.e_string);
            //                        strSign2 = GetData(RecData, i, 10, EnumTerimalDataType.e_string);
            //                        ResultDictionary[""][i] = "CRL序列号:" + clr;
            //                    }
            //                    else
            //                    {
            //                        MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("会话初始化", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);



            //             MessageAdd("会话协商",EnumLogType.错误信息);
            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_SessionKeyConsult(strState, strTESAMNo, "01", "00", clr, zz_num, zd_OutCertificate, strR2, strSign2, strBu1, strBu2, strBu3, strBu4);


            //            InitoutEncM1 = strBu1.ToString().Replace("\0", "");
            //            InitoutSignData = strBu2.ToString().Replace("\0", "");
            //            InitoutMac2 = strBu3.ToString().Replace("\0", "");
            //            InitoutSign3 = strBu4.ToString().Replace("\0", "");


            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 13, Core.Function.UsefulMethods.ConvertStringToBytes("1601" + Core.Function.UsefulMethods.revStr(InitoutEncM1) + Core.Function.UsefulMethods.revStr(InitoutSignData) + Core.Function.UsefulMethods.revStr(InitoutMac2) + Core.Function.UsefulMethods.revStr(InitoutSign3)), RecData, MaxWaitSeconds_Write);
                        

            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        strR3 = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
            //                        strMac3 = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
            //                        iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_SessionConsultVerify(strR3, strMac3);
            //                        ResultDictionary[""][i] = "R3:" + strR3;
            //                        if (iResult != 0)
            //                        {
            //                           TempData[i].Resoult="不合格";
            //                            ResultDictionary[""][i] += "|会话协商校验失败";
            //                        }
            //                    }
            //                    else
            //                    {
            //                        MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("会话协商", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);


            //             MessageAdd("内部认证",EnumLogType.错误信息);
            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_GetR1(strBu1);
            //            strR4 = strBu1.ToString().Replace("\0", "");

            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 17, Core.Function.UsefulMethods.ConvertStringToBytes(UsefulMethods.revStr(strR4)), RecData, MaxWaitSeconds_Write);
                        
            //            strMR4 = "";
            //            strR5 = "";
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        strMR4 = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
            //                        strR5 = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
            //                        ResultDictionary[""][i] = "R4密文:" + strMR4;
            //                    }
            //                    else
            //                    {
            //                        MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("内部认证", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);

            //             MessageAdd("外部认证",EnumLogType.错误信息);
            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_ExternalAuth(KeyVerion, strTESAMNo, strR4, strMR4, strR5, strBu1);
            //            strMR5 = strBu1.ToString().Replace("\0", "");

            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 18, Core.Function.UsefulMethods.ConvertStringToBytes(UsefulMethods.revStr(strMR5)), RecData, MaxWaitSeconds_Write);
                        
            //            strR6 = "";
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        strR6 = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
            //                        ResultDictionary[""][i] = "随机数R6:" + strR6;
            //                    }
            //                    else
            //                    {
            //                        MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("外部认证", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);


            //             MessageAdd("证书状态切换",EnumLogType.错误信息);
            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_CertificateStateChange(KeyVerion, strTESAMNo, "01", strR6, strBu1, strBu2);
            //            strMR6 = strBu1.ToString().Replace("\0", "");
            //            OutMac = strBu2.ToString().Replace("\0", "");
            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 19, Core.Function.UsefulMethods.ConvertStringToBytes("00" + Core.Function.UsefulMethods.revStr(strMR6) + Core.Function.UsefulMethods.revStr(OutMac)), RecData, MaxWaitSeconds_Write);
                        
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
            //                        {
            //                             MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                           TempData[i].Resoult="不合格";
            //                        }
            //                    }
            //                    else
            //                    {
            //                         MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("证书状态切换", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);

            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 11, new byte[] { }, RecData, MaxWaitSeconds_Write);

            //             MessageAdd("CA证书更新",EnumLogType.错误信息);
            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_CACertificateUpdate("01", "01", strBu1);
            //            strzsmw = strBu1.ToString().Replace("\0", "");


            //            ileng = strzsmw.Length / 2;
            //            sleng = (ileng % 256).ToString("x2") + (ileng / 256).ToString("x2");
            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 16, Core.Function.UsefulMethods.ConvertStringToBytes(sleng + Core.Function.UsefulMethods.revStr(strzsmw)), RecData, MaxWaitSeconds_Write);
                        
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
            //                        {
            //                             MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                           TempData[i].Resoult="不合格";
            //                        }
            //                    }
            //                    else
            //                    {
            //                         MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("CA证书更新", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);

            //             MessageAdd("对称密钥更新",EnumLogType.错误信息);
            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_SymmetricKeyUpdate("01", strTESAMNo, strBu1, strBu2);

            //            myCount = strBu1.ToString().Replace("\0", "");
            //            OutMac = strBu2.ToString().Replace("\0", "");

            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 14, Core.Function.UsefulMethods.ConvertStringToBytes("02" + myCount + Core.Function.UsefulMethods.revStr(OutMac)), RecData, MaxWaitSeconds_Write);
                        
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
            //                        {
            //                             MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                           TempData[i].Resoult="不合格";
            //                        }
            //                    }
            //                    else
            //                    {
            //                         MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("对称密钥更新", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);

            //            #region 密钥恢复
            //             MessageAdd("内部认证",EnumLogType.错误信息);
            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_GetR1(strBu1);
            //            strR4 = strBu1.ToString().Replace("\0", "");

            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 17, Core.Function.UsefulMethods.ConvertStringToBytes(UsefulMethods.revStr(strR4)), RecData, MaxWaitSeconds_Write);
                        
            //            strMR4 = "";
            //            strR5 = "";
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        strMR4 = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
            //                        strR5 = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
            //                        ResultDictionary[""][i] = "R4密文:" + strMR4;
            //                    }
            //                    else
            //                    {
            //                        MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("内部认证", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);

            //             MessageAdd("外部认证",EnumLogType.错误信息);
            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_ExternalAuth("01", strTESAMNo, strR4, strMR4, strR5, strBu1);
            //            strMR5 = strBu1.ToString().Replace("\0", "");

            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 18, Core.Function.UsefulMethods.ConvertStringToBytes(UsefulMethods.revStr(strMR5)), RecData, MaxWaitSeconds_Write);
                        
            //            strR6 = "";
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        strR6 = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
            //                        ResultDictionary[""][i] = "随机数R6:" + strR6;
            //                    }
            //                    else
            //                    {
            //                        MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("外部认证", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);


            //             MessageAdd("证书状态切换",EnumLogType.错误信息);
            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_CertificateStateChange("01", strTESAMNo, "00", strR6, strBu1, strBu2);
            //            strMR6 = strBu1.ToString().Replace("\0", "");
            //            OutMac = strBu2.ToString().Replace("\0", "");
            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 19, Core.Function.UsefulMethods.ConvertStringToBytes("01" + Core.Function.UsefulMethods.revStr(strMR6) + Core.Function.UsefulMethods.revStr(OutMac)), RecData, MaxWaitSeconds_Write);
                        
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
            //                        {
            //                             MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                           TempData[i].Resoult="不合格";
            //                        }
            //                    }
            //                    else
            //                    {
            //                         MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("证书状态切换", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);

            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 11, new byte[] { }, RecData, MaxWaitSeconds_Write);

            //             MessageAdd("CA证书更新",EnumLogType.错误信息);
            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_CACertificateUpdate("00", "01", strBu1);
            //            strzsmw = strBu1.ToString().Replace("\0", "");


            //            ileng = strzsmw.Length / 2;
            //            sleng = (ileng % 256).ToString("x2") + (ileng / 256).ToString("x2");
            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 16, Core.Function.UsefulMethods.ConvertStringToBytes(sleng + Core.Function.UsefulMethods.revStr(strzsmw)), RecData, MaxWaitSeconds_Write);
                        
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
            //                        {
            //                             MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                           TempData[i].Resoult="不合格";
            //                        }
            //                    }
            //                    else
            //                    {
            //                         MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("CA证书更新", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);

            //             MessageAdd("对称密钥更新",EnumLogType.错误信息);
            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_SymmetricKeyUpdate("00", strTESAMNo, strBu1, strBu2);

            //            myCount = strBu1.ToString().Replace("\0", "");
            //            OutMac = strBu2.ToString().Replace("\0", "");

            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 14, Core.Function.UsefulMethods.ConvertStringToBytes("00" + myCount + Core.Function.UsefulMethods.revStr(OutMac)), RecData, MaxWaitSeconds_Write);
                        
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
            //                        {
            //                             MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                           TempData[i].Resoult="不合格";
            //                        }
            //                    }
            //                    else
            //                    {
            //                         MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("对称密钥更新", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);
            //            #endregion


            //             MessageAdd("置离线计数器",EnumLogType.错误信息);
            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 11, new byte[] { }, RecData, MaxWaitSeconds_Write);

            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_SetOfflineCounter("00", strTESAMNo, strCount, strBu1);
            //            OutEncCounter = strBu1.ToString().Replace("\0", "");

            //            Talkers[2].Framer.bol_Mac = false;
            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 20, Core.Function.UsefulMethods.ConvertStringToBytes(UsefulMethods.revStr(OutEncCounter)), RecData, MaxWaitSeconds_Write);
                        
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
            //                        {
            //                             MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                           TempData[i].Resoult="不合格";
            //                        }
            //                    }
            //                    else
            //                    {
            //                         MessageAdd("证书状态切换失败",EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("置离线计数器", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);


            //             MessageAdd("错误MAC下参",EnumLogType.错误信息);
            //            byte[] btyTmp = new byte[1];
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    btyTmp = Talkers[i].Framer.WriteData_Mac(4, 0, 5, Core.Function.UsefulMethods.ConvertStringToBytes("000000"));
            //                }
            //            }

            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_MACVerify("00", strTESAMNo, "04", Core.Function.UsefulMethods.ConvertBytesToString(btyTmp).Replace(" ", ""), strBu1);

            //            OutMac = strBu1.ToString().Replace("\0", "");
            //            OutMac = (Convert.ToInt64(OutMac, 16) + 1).ToString("x4");
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    Talkers[i].Framer.bol_Mac = true; Talkers[i].Framer.str_Mac = OutMac;
            //                }
            //            }

            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 5, Core.Function.UsefulMethods.ConvertStringToBytes("000000"), RecData, MaxWaitSeconds_Write);
                        
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 1)
            //                    {
            //                    }
            //                    else
            //                    {
            //                         MessageAdd("错误MAC下参成功",EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("错误MAC下参", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);


            //             MessageAdd("关闭硬件认证",EnumLogType.错误信息);
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    btyTmp = Talkers[i].Framer.WriteData_Mac(4, 0, 5, Core.Function.UsefulMethods.ConvertStringToBytes("000000"));
            //                }
            //            }

            //            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_MACVerify("00", strTESAMNo, "04", Core.Function.UsefulMethods.ConvertBytesToString(btyTmp).Replace(" ", ""), strBu1);

            //            OutMac = strBu1.ToString().Replace("\0", "");
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    Talkers[i].Framer.bol_Mac = true; Talkers[i].Framer.str_Mac = OutMac;
            //                }
            //            }

            //            TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 5, Core.Function.UsefulMethods.ConvertStringToBytes("000000"), RecData, MaxWaitSeconds_Write);
            //            TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 5, RecData, MaxWaitSeconds_Write);
                        
            //            for (int i = 0; i < MeterNumber; i++)
            //            {
            //                if (meterInfo[i].YaoJianYn)
            //                {
            //                    if (TalkResult[i] == 0)
            //                    {
            //                        if (GetData(RecData, i, 3, EnumTerimalDataType.e_string) != "0")
            //                        {
            //                             MessageAdd("关闭硬件认证失败",EnumLogType.错误信息);
            //                           TempData[i].Resoult="不合格";
            //                        }
            //                        ResultDictionary[""][i] = GetData(RecData, i, 3, EnumTerimalDataType.e_string) + "|0";
            //                    }
            //                    else
            //                    {
            //                         MessageAdd("关闭硬件认证失败",EnumLogType.错误信息);
            //                       TempData[i].Resoult="不合格";
            //                    }
            //                }
            //            }
            //             RefUIData("关闭硬件认证", m_str_ParameterID, SubItemIndex++, Resoult, ResultDictionary[""], 0, 1, m_bol_IsLastSubItem);


            //            meterInfo[tableNo].YaoJianYn = false;
            //        }
            //    }

            //    for (int tableNo = 0; tableNo < MeterNumber; tableNo++)
            //    {
            //        meterInfo[tableNo].YaoJianYn = bol_TemporaryIsVerify[tableNo];
            //    }


            //    for (int i = 0; i < MeterNumber; i++)
            //    {
            //        if (!meterInfo[i].YaoJianYn) continue;
            //        ResultDictionary["结论"][i] = Resoult[i];
            //    }
            //    RefUIData("结论");
            //    MessageAdd("检定完成",EnumLogType.提示信息);
            //}
            //catch (Exception ex)
            //{

            //    MessageAdd(ex.ToString(), EnumLogType.错误信息);
            //}
        }
    }
}
