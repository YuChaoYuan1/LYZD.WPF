using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.IdentityAndKeyAgreement
{
    /// <summary>
    /// 密钥更新
    /// </summary>
    public class KeyDownload_376 : VerifyBase
    {

        int SubItemIndex = 1;

        string strTESAMNo = "";//ESAM序列号
        string strVersionNum = "";//密钥版本
        string strState = "";//芯片状态
        string strCount = "";//计数器
        string strCLR = "";//证书序列号

        #region 临时变量
        StringBuilder strBu1 = new StringBuilder(10000);
        StringBuilder strBu2 = new StringBuilder(10000);
        StringBuilder strBu3 = new StringBuilder(10000);
        StringBuilder strBu4 = new StringBuilder(10000);
        #endregion

        int ileng = 0;
        string sleng = "";
        int iResult = 0;
        string KeyVerion = "";
        string strR1 = "";
        string strR2 = "";
        string strR3 = "";
        string strMac3 = "";
        string strR4 = ""; string strMR4 = "";
        string strR5 = ""; string strMR5 = "";
        string strR6 = ""; string strMR6 = "";
        string OutMac = "";
        string clr = "";
        string zz_num = "";
        string zd_OutCertificate = "";
        string strSign2 = "";
        string strOutCertificate = "";
        string OutEncR1 = "";
        string OutSign1 = "";
        string myCount = "";
        string InitoutEncM1 = "";
        string InitoutSignData = "";
        string InitoutMac2 = "";
        string InitoutSign3 = "";
        string strzsmw = "";
        string OutEncCounter = "";

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
                StartVerify();
                bool[] TemYaoJian = new bool[MeterNumber];

                SubItemIndex = 1;
                iResult = TerminalProtocol.Encryption.EncryptionFunction2013.ConnectDevice(VerifyConfig.Dog_IP, VerifyConfig.Dog_Prot, "10");
                if (iResult != 0)//czx0723
                {
                    MessageAdd("连接加密机失败！返回" + iResult, EnumLogType.错误信息);
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = "0";
                            TempData[i].Data = iResult.ToString();
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "加密机连接失败";
                        }
                    }
                    AddItemsResoult("连接加密机", TempData);
                    return;
                }


                #region 启用硬件加密
                MessageAdd("启用硬件加密",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 5, new byte[] { 0xff, 0x00, 0x00 }, RecData, MaxWaitSeconds_Write);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 5, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "255";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 3, EnumTerimalDataType.e_string) != "255")
                            {
                                TempData[i].Tips = "关闭硬件认证失败";
                                TempData[i].Resoult = "不合格";
                            }
                            TempData[i].Data = GetData(RecData, i, 3, EnumTerimalDataType.e_string) ;
                        }
                        else
                        {
                            TempData[i].Tips = "关闭硬件认证失败,无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("启用硬件加密", TempData);
                #endregion

                #region 获取ESAM密钥信息
                MessageAdd("获取ESAM密钥信息",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 11, new byte[] { }, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            strTESAMNo = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
                            strCLR = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                            strCount = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                            strState = GetData(RecData, i, 6, EnumTerimalDataType.e_string);
                            strVersionNum = GetData(RecData, i, 7, EnumTerimalDataType.e_string);
                            TempData[i].Data = "ESAM序列号:" + strTESAMNo;
                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                KeyVerion = strVersionNum == "0000000000000000" ? "00" : "01";
                strState = strState.PadLeft(2, '0');
                AddItemsResoult("ESAM密钥信息", TempData);
                #endregion

                #region 芯片版本和密钥版本不一致的时候需要把芯片证书切换过来
                if ((KeyVerion == "00" && strState == "01") || (KeyVerion == "01" && strState == "00"))
                {
                    MessageAdd("内部认证",EnumLogType.流程信息);
                    iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_GetR1(strBu1);
                    strR4 = strBu1.ToString().Replace("\0", "");
                    TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 17, UsefulMethods.ConvertStringToBytes(UsefulMethods.revStr(strR4)), RecData, MaxWaitSeconds_Write);

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            if (TalkResult[i] == 0)
                            {
                                strMR4 = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
                                strR5 = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                                TempData[i].Data = "R4密文:" + strMR4;
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                    }
                    AddItemsResoult("内部认证", TempData);  


                    MessageAdd("外部认证",EnumLogType.流程信息);
                    iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_ExternalAuth(KeyVerion, strTESAMNo, strR4, strMR4, strR5, strBu1);
                    strMR5 = strBu1.ToString().Replace("\0", "");

                    TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 18, UsefulMethods.ConvertStringToBytes(UsefulMethods.revStr(strMR5)), RecData, MaxWaitSeconds_Write);

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            if (TalkResult[i] == 0)
                            {
                                strR6 = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
                                TempData[i].Data = "随机数R6:" + strR6;
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                    }
                    AddItemsResoult("外部认证", TempData);


                    MessageAdd("证书状态切换",EnumLogType.流程信息);
                    iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_CertificateStateChange(KeyVerion, strTESAMNo, KeyVerion, strR6, strBu1, strBu2);
                    strMR6 = strBu1.ToString().Replace("\0", "");
                    OutMac = strBu2.ToString().Replace("\0", "");
                    TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 19, UsefulMethods.ConvertStringToBytes((KeyVerion == "01" ? "00" : "01") + UsefulMethods.revStr(strMR6) + UsefulMethods.revStr(OutMac)), RecData, MaxWaitSeconds_Write);

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = "Fn1";
                            if (TalkResult[i] == 0)
                            {
                                TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                                if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                                {
                                    MessageAdd("证书状态切换失败", EnumLogType.错误信息);
                                    TempData[i].Resoult = "不合格";
                                }
                            }
                            else
                            {
                                MessageAdd("证书状态切换失败", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                            }
                        }
                    }
                    AddItemsResoult("证书状态切换", TempData);

                    MessageAdd("获取ESAM密钥信息",EnumLogType.流程信息);
                    TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 11, new byte[] { }, RecData, MaxWaitSeconds_Write);

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            if (TalkResult[i] == 0)
                            {
                                strTESAMNo = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
                                strCLR = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                                strCount = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                                strState = GetData(RecData, i, 6, EnumTerimalDataType.e_string);
                                strVersionNum = GetData(RecData, i, 7, EnumTerimalDataType.e_string);
                                // TempData[i].Data  = "ESAM序列号:" + strTESAMNo;
                            }
                            else
                            {
                                MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
                                //TempData[i].Resoult = "不合格";
                            }
                        }
                    }
                    KeyVerion = strVersionNum == "0000000000000000" ? "00" : "01";
                    strState = strState.PadLeft(2, '0');
                }
                else
                {
                    AddItemsResoult("内部认证", TempData);
                    AddItemsResoult("外部认证", TempData);
                    AddItemsResoult("证书状态切换", TempData);
                }
                #endregion

                #region 会话初始化
                MessageAdd("获取随机数",EnumLogType.流程信息);
                iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_GetR1(strBu1);
                strR1 = strBu1.ToString().Replace("\0", "");

                MessageAdd("会话初始化",EnumLogType.流程信息);
                iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_SessionInitRec(KeyVerion, strTESAMNo, "01", "00", strR1, strBu1, strBu2, strBu3, strBu4);

                strOutCertificate = strBu1.ToString().Replace("\0", "");
                OutEncR1 = strBu2.ToString().Replace("\0", "");
                OutMac = strBu3.ToString().Replace("\0", "");
                OutSign1 = strBu4.ToString().Replace("\0", "");

                ileng = 2 + (strOutCertificate + OutEncR1 + OutMac + OutSign1).Length / 2;
                sleng = (ileng % 256).ToString("x2") + (ileng / 256).ToString("x2");

                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 12, UsefulMethods.ConvertStringToBytes(sleng + "0100" + UsefulMethods.revStr(strOutCertificate) + UsefulMethods.revStr(OutEncR1) + UsefulMethods.revStr(OutMac) + UsefulMethods.revStr(OutSign1)), RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            clr = GetData(RecData, i, 6, EnumTerimalDataType.e_string);
                            zz_num = GetData(RecData, i, 7, EnumTerimalDataType.e_string);
                            zd_OutCertificate = GetData(RecData, i, 8, EnumTerimalDataType.e_string);
                            strR2 = GetData(RecData, i, 9, EnumTerimalDataType.e_string);
                            strSign2 = GetData(RecData, i, 10, EnumTerimalDataType.e_string);
                             TempData[i].Data  = "CRL序列号:" + clr;
                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("会话初始化", TempData);

                #endregion

                #region 会话协商
                MessageAdd("会话协商",EnumLogType.流程信息);
                iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_SessionKeyConsult(strState, strTESAMNo, "01", "00", clr, zz_num, zd_OutCertificate, strR2, strSign2, strBu1, strBu2, strBu3, strBu4);

                InitoutEncM1 = strBu1.ToString().Replace("\0", "");
                InitoutSignData = strBu2.ToString().Replace("\0", "");
                InitoutMac2 = strBu3.ToString().Replace("\0", "");
                InitoutSign3 = strBu4.ToString().Replace("\0", "");

                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 13, UsefulMethods.ConvertStringToBytes("1601" + UsefulMethods.revStr(InitoutEncM1) + UsefulMethods.revStr(InitoutSignData) + UsefulMethods.revStr(InitoutMac2) + UsefulMethods.revStr(InitoutSign3)), RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            strR3 = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
                            strMac3 = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                            iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_SessionConsultVerify(strR3, strMac3);
                             TempData[i].Data  = "R3:" + strR3;
                            if (iResult != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips += "会话协商校验失败";
                            }
                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("会话协商", TempData);
                #endregion

                #region 内部认证
                MessageAdd("内部认证",EnumLogType.流程信息);
                iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_GetR1(strBu1);
                strR4 = strBu1.ToString().Replace("\0", "");

                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 17, UsefulMethods.ConvertStringToBytes(UsefulMethods.revStr(strR4)), RecData, MaxWaitSeconds_Write);

                strMR4 = "";
                strR5 = "";
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            strMR4 = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
                            strR5 = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                             TempData[i].Data  = "R4密文:" + strMR4;
                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("内部认证", TempData);
                #endregion

                #region 外部认证

                MessageAdd("外部认证",EnumLogType.流程信息);
                iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_ExternalAuth(KeyVerion, strTESAMNo, strR4, strMR4, strR5, strBu1);
                strMR5 = strBu1.ToString().Replace("\0", "");

                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 18, UsefulMethods.ConvertStringToBytes(UsefulMethods.revStr(strMR5)), RecData, MaxWaitSeconds_Write);

                strR6 = "";
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                             strR6 = GetData(RecData, i, 3, EnumTerimalDataType.e_string);
                             TempData[i].Data  = "随机数R6:" + strR6;
                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("外部认证", TempData);
                #endregion

                #region 证书状态切换
                MessageAdd("证书状态切换",EnumLogType.流程信息);
                iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_CertificateStateChange(KeyVerion, strTESAMNo, "01", strR6, strBu1, strBu2);
                strMR6 = strBu1.ToString().Replace("\0", "");
                OutMac = strBu2.ToString().Replace("\0", "");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 19, UsefulMethods.ConvertStringToBytes("00" + UsefulMethods.revStr(strMR6) + UsefulMethods.revStr(OutMac)), RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "Fn1";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                            {
                                TempData[i].Tips = "证书状态切换失败";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                        else
                        {
                            TempData[i].Tips = "证书状态切换失败,无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("证书状态切换", TempData);
                #endregion

                #region CA证书更新

                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 11, new byte[] { }, RecData, MaxWaitSeconds_Write);

                MessageAdd("CA证书更新",EnumLogType.流程信息);
                iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_CACertificateUpdate("01", "01", strBu1);
                strzsmw = strBu1.ToString().Replace("\0", "");


                ileng = strzsmw.Length / 2;
                sleng = (ileng % 256).ToString("x2") + (ileng / 256).ToString("x2");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 16, UsefulMethods.ConvertStringToBytes(sleng + UsefulMethods.revStr(strzsmw)), RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "Fn1";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                            {
                                TempData[i].Tips = "证书状态切换失败";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                        else
                        {
                            TempData[i].Tips = "证书状态切换失败";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("CA证书更新", TempData);
                #endregion

                #region 对称密钥更新

                MessageAdd("对称密钥更新",EnumLogType.流程信息);
                iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_SymmetricKeyUpdate("01", strTESAMNo, strBu1, strBu2);

                myCount = strBu1.ToString().Replace("\0", "");
                OutMac = strBu2.ToString().Replace("\0", "");

                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 14, UsefulMethods.ConvertStringToBytes("02" + myCount + UsefulMethods.revStr(OutMac)), RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "Fn1";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "证书状态切换失败";
                            }
                        }
                        else
                        {
                            TempData[i].Tips = "证书状态切换失败";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("对称密钥更新", TempData);
                #endregion

                #region 置离线计数器
                MessageAdd("置离线计数器",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 11, new byte[] { }, RecData, MaxWaitSeconds_Write);

                iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_SetOfflineCounter("01", strTESAMNo, strCount, strBu1);
                OutEncCounter = strBu1.ToString().Replace("\0", "");

                Talkers[2].Framer.bol_Mac = false;
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(6, 0, 20, UsefulMethods.ConvertStringToBytes(UsefulMethods.revStr(OutEncCounter)), RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "Fn1";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);

                            if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                            {
                                TempData[i].Tips = "证书状态切换失败";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                        else
                        {
                            TempData[i].Tips = "证书状态切换失败";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("置离线计数器", TempData);
                #endregion


                byte[] btyTmp = new byte[1];
                if (VerifyConfig.Dog_IsKeySwitch_376)
                {
                    MessageAdd("关闭硬件认证",EnumLogType.流程信息);
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            btyTmp = Talkers[i].Framer.WriteData_Mac(4, 0, 5, UsefulMethods.ConvertStringToBytes("FF0000"));
                        }
                    }

                    iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_MACVerify("01", strTESAMNo, "04", UsefulMethods.ConvertBytesToString(btyTmp).Replace(" ", ""), strBu1);

                    OutMac = strBu1.ToString().Replace("\0", "");
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            Talkers[i].Framer.bol_Mac = true;
                            Talkers[i].Framer.str_Mac = OutMac;
                        }
                    }

                    TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 5, UsefulMethods.ConvertStringToBytes("FF0000"), RecData, MaxWaitSeconds_Write);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 5, RecData, MaxWaitSeconds_Write);

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = "255";
                            if (TalkResult[i] == 0)
                            {
                                if (GetData(RecData, i, 3, EnumTerimalDataType.e_string) != "255")
                                {
                                    TempData[i].Tips = "打开硬件认证失败";
                                    TempData[i].Resoult = "不合格";
                                }
                                 TempData[i].Data  = GetData(RecData, i, 3, EnumTerimalDataType.e_string) ;
                            }
                            else
                            {
                                TempData[i].Tips = "打开硬件认证失败,无回复";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                    }
                    AddItemsResoult("关闭硬件认证", TempData);

                }
                else
                {

                    MessageAdd("关闭硬件认证",EnumLogType.流程信息);
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            btyTmp = Talkers[i].Framer.WriteData_Mac(4, 0, 5, UsefulMethods.ConvertStringToBytes("000000"));
                        }
                    }

                    iResult = TerminalProtocol.Encryption.EncryptionFunction2013.Terminal_Formal_MACVerify("01", strTESAMNo, "04", UsefulMethods.ConvertBytesToString(btyTmp).Replace(" ", ""), strBu1);

                    OutMac = strBu1.ToString().Replace("\0", "");
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            Talkers[i].Framer.bol_Mac = true;
                            Talkers[i].Framer.str_Mac = OutMac;
                        }
                    }

                    TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 5, UsefulMethods.ConvertStringToBytes("000000"), RecData, MaxWaitSeconds_Write);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 5, RecData, MaxWaitSeconds_Write);

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = "0";
                            if (TalkResult[i] == 0)
                            {
                                if (GetData(RecData, i, 3, EnumTerimalDataType.e_string) != "0")
                                {
                                    TempData[i].Tips = "关闭硬件认证失败";

                                    TempData[i].Resoult = "不合格";
                                }
                                 TempData[i].Data  = GetData(RecData, i, 3, EnumTerimalDataType.e_string) ;
                            }
                            else
                            {
                                TempData[i].Tips = "关闭硬件认证失败,无回复";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                    }
                    AddItemsResoult("关闭硬件认证", TempData);
                }
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
