using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.IdentityAndKeyAgreement
{
    /// <summary>
    /// 密钥下装
    /// </summary>
    public class KeyDownload : VerifyBase
    {
        public override void Verify()
        {
            base.Verify();
            StartVerify698();

            int ret = 0;
            ConnectLink(true);
            //string tem = "";
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetTrmKeyData(i, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.str_Ter_Address.PadLeft(16, '0'), "00", ref Talkers[i].Framer698.cOutSID, ref Talkers[i].Framer698.cOutAttachData, ref Talkers[i].Framer698.cOutTrmKeyData, ref Talkers[i].Framer698.cOutMAC);
                    Talkers[i].Framer698.cTaskData = "070105F100070002020982" + (Talkers[i].Framer698.cOutTrmKeyData.Length / 2).ToString("x4") + Talkers[i].Framer698.cOutTrmKeyData + "5E" + Talkers[i].Framer698.cOutSID + "0B" + Talkers[i].Framer698.cOutAttachData + "04" + Talkers[i].Framer698.cOutMAC + "00";
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetTerminalSetData(i, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cTaskData, ref Talkers[i].Framer698.cOutSID, ref Talkers[i].Framer698.cOutAttachData, ref Talkers[i].Framer698.cOutTaskData, ref Talkers[i].Framer698.cOutTaskMAC);
                    Talkers[i].Framer698.sAPDU = "1001" + GetMiWenLen(Talkers[i].Framer698.cOutTaskData) + Talkers[i].Framer698.cOutTaskData + "00" + Talkers[i].Framer698.cOutSID + "02" + Talkers[i].Framer698.cOutAttachData + "04" + Talkers[i].Framer698.cOutTaskMAC;
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//应用连接
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "00";
                    if (TalkResult[i] == 0)
                    {
                         Talkers[i].Framer698.cOutAttachData = GetData(RecData, i, 2,  EnumTerimalDataType.e_string);
                         Talkers[i].Framer698.cOutMAC = GetData(RecData, i, 5,  EnumTerimalDataType.e_string);

                        ret =  TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(i,  Talkers[i].Framer698.iKeyState, 3,  Talkers[i].Framer698.cTESAMNO,  Talkers[i].Framer698.cOutSessionKey,  Talkers[i].Framer698.cOutAttachData,  Talkers[i].Framer698.cOutMAC, ref  Talkers[i].Framer698.cOutData);

                        if ( Talkers[i].Framer698.cOutData.Length > 16)
                        {
                            if ( Talkers[i].Framer698.cOutData.Substring(14, 2) == "00")
                                TempData[i].Resoult = "合格";
                            else
                               TempData[i].Resoult="不合格";
                        }
                        else
                           TempData[i].Resoult="不合格";

                        TempData[i].Data = Talkers[i].Framer698.cOutData;
                    }
                    else
                    {
                       TempData[i].Resoult="不合格";
                       TempData[i].Tips = "无回复";
                    }
                }
            }
            AddItemsResoult("密钥更新", TempData);



            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    ret =  TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetCACertificateData(i, 1,  Talkers[i].Framer698.cTESAMNO,  Talkers[i].Framer698.cOutSessionKey, "01", ref  Talkers[i].Framer698.cOutSID, ref  Talkers[i].Framer698.cOutAttachData, ref  Talkers[i].Framer698.ucOutAttachData, ref  Talkers[i].Framer698.cOutMAC);
                    ret =  TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetCACertificateData(i, 1,  Talkers[i].Framer698.cTESAMNO,  Talkers[i].Framer698.cOutSessionKey, "02", ref  Talkers[i].Framer698.cOutSID, ref  Talkers[i].Framer698.cOutAttachData, ref  Talkers[i].Framer698.ucOutAttachData, ref  Talkers[i].Framer698.cOutMAC);
                    if ( Talkers[i].Framer698.cOutAttachData.Length >  Talkers[i].Framer698.ucOutAttachData.Length)
                    {
                         Talkers[i].Framer698.cTaskData = "070105F100080002020982" +  Talkers[i].Framer698.ucOutAttachData +  Talkers[i].Framer698.cOutAttachData + "5D" +  Talkers[i].Framer698.cOutSID + "02" +  Talkers[i].Framer698.ucOutAttachData + "00";
                    }
                    else
                    {
                         Talkers[i].Framer698.cTaskData = "070105F100080002020982" +  Talkers[i].Framer698.cOutAttachData +  Talkers[i].Framer698.ucOutAttachData + "5D" +  Talkers[i].Framer698.cOutSID + "02" +  Talkers[i].Framer698.cOutAttachData + "00";
                    }

                    ret =  TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetTerminalSetData(i, 3,  Talkers[i].Framer698.cTESAMNO,  Talkers[i].Framer698.cOutSessionKey,  Talkers[i].Framer698.cTaskData, ref  Talkers[i].Framer698.cOutSID, ref  Talkers[i].Framer698.cOutAttachData, ref  Talkers[i].Framer698.cOutTaskData, ref  Talkers[i].Framer698.cOutTaskMAC);
                    Talkers[i].Framer698.sAPDU = "1001" + GetMiWenLen( Talkers[i].Framer698.cOutTaskData) +  Talkers[i].Framer698.cOutTaskData + "00" +  Talkers[i].Framer698.cOutSID + "02" +  Talkers[i].Framer698.cOutAttachData + "04" +  Talkers[i].Framer698.cOutTaskMAC;
                    setData[i] =  Talkers[i].Framer698.ReadData( Talkers[i].Framer698.sAPDU);//应用连接
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "00";
                    if (TalkResult[i] == 0)
                    {
                         Talkers[i].Framer698.cOutAttachData = GetData(RecData, i, 2,  EnumTerimalDataType.e_string);
                         Talkers[i].Framer698.cOutMAC = GetData(RecData, i, 5,  EnumTerimalDataType.e_string);

                        ret =  TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(i,  Talkers[i].Framer698.iKeyState, 3,  Talkers[i].Framer698.cTESAMNO,  Talkers[i].Framer698.cOutSessionKey,  Talkers[i].Framer698.cOutAttachData,  Talkers[i].Framer698.cOutMAC, ref  Talkers[i].Framer698.cOutData);

                        if ( Talkers[i].Framer698.cOutData.Length > 16)
                        {
                            if ( Talkers[i].Framer698.cOutData.Substring(14, 2) == "00")
                                TempData[i].Resoult = "合格";
                            else
                               TempData[i].Resoult="不合格";
                        }
                        else
                           TempData[i].Resoult="不合格";
                        TempData[i].Data =  Talkers[i].Framer698.cOutData;
                    }
                    else
                    {
                       TempData[i].Resoult="不合格";
                        TempData[i].Tips = "无回复";

                    }
                }
            }
            AddItemsResoult("更新主站证书", TempData);



            ConnectLink(true);

            SetTime_698(DateTime.Now, 0);

            if (VerifyConfig. Dog_IsKeySwitch_698)
            {
                SetData_698("060119F1010200160100", "设置安全模式参数");
                MessageAdd("正在读取安全模式参数",EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        setData[i] =  Talkers[i].Framer698.ReadData_05("F1010200");
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
                            string sTmp = GetData(RecData, i, 5,  EnumTerimalDataType.e_string);
                            TempData[i].Data = sTmp ;
                            if (sTmp != "1")
                               TempData[i].Resoult="不合格";
                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult="不合格";
                        }
                    }
                }
                AddItemsResoult("安全模式参数", TempData);
            }
            else
            {
                SetData_698("060119F1010200160000", "设置安全模式参数");
                MessageAdd("正在读取安全模式参数",EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        setData[i] =  Talkers[i].Framer698.ReadData_05("F1010200");
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
                            string sTmp = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                            TempData[i].Data = sTmp;
                            if (sTmp != "0")
                                TempData[i].Resoult = "不合格";
                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }

                    }
                }
                AddItemsResoult("安全模式参数", TempData);
            }
        }

        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "密钥更新", "更新主站证书", "安全模式参数", "结论" };
            return true;
        }
    }
}
