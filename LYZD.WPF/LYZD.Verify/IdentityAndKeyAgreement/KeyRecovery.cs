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
    public class KeyRecovery : VerifyBase
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
                ConnectLink(true);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetTrmKeyData(i, 0, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.str_Ter_Address.PadLeft(16, '0'), "00", ref Talkers[i].Framer698.cOutSID, ref Talkers[i].Framer698.cOutAttachData, ref Talkers[i].Framer698.cOutTrmKeyData, ref Talkers[i].Framer698.cOutMAC);
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
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.cOutAttachData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.cOutMAC = GetData(RecData, i, 5, EnumTerimalDataType.e_string);

                             ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(i, Talkers[i].Framer698.iKeyState, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cOutAttachData, Talkers[i].Framer698.cOutMAC, ref Talkers[i].Framer698.cOutData);
                             //MessageAdd("读取到数据"+ Talkers[i].Framer698.cOutData, EnumLogType.流程信息);

                            if (Talkers[i].Framer698.cOutData.Length > 16)
                            {
                                if (Talkers[i].Framer698.cOutData.Substring(14, 2) != "00")
                                {
                                    TempData[i].Resoult = "不合格";
                                    TempData[i].Tips = "失败";
                                }
                                //else
                                //   TempData[i].Resoult="合格";
                            }
                            else
                                TempData[i].Resoult = "不合格";
                            TempData[i].Data = "密钥更新结果:" + Talkers[i].Framer698.cOutData;
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("密钥恢复", TempData);

                ConnectLink(true);
                SetTime_698(DateTime.Now, 0);
                SetData_698("060119F1010200160100", "设置安全模式参数");
                MessageAdd("正在读取安全模式参数", EnumLogType.流程信息,true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        setData[i] = Talkers[i].Framer698.ReadData_05("F1010200");
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
                            string sTmp = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                            TempData[i].Data = sTmp;
                            if (sTmp != "1")
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
                AddItemsResoult("安全模式参数", TempData);
            }
            catch (Exception ex)
            {
                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }

        /// <summary>
        /// 获取检定流程
        /// </summary>
        /// <returns></returns>
        public string GetTestProcess()
        {
            return @"1）	读取安全模式参数(F1010200)，如果未启用安全模式，则设置为启用；
                     2）	读取ESAM序列号(F1000200)、对称密钥版本(F1000400)、计数器(F1000700)；
                     3）	读取主站证书(F1000C00)；
                     4）	读取终端证书(F1000A00)；
                     5）	建立应用连接，采用数字签名的认证机制；
                     6）	终端密钥恢复(F1000700)；
                     7）	读取ESAM序列号(F1000200)、对称密钥版本(F1000400)、计数器(F1000700)；
                     8）	读取主站证书(F1000C00)；
                     9）	读取终端证书(F1000A00)；
                     10）	建立应用连接。 ";

            //流程控制
            //步骤字典，当前步骤，
            //写成配置文件或者数据库把，到时候通过检定编号来读取        
            //配置文件的化写代码时候就不能时时看了，也不好

            //
            
        }
    }
}
