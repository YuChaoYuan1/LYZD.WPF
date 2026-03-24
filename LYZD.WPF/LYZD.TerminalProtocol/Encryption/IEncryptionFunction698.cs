using LYZD.DAL.Config;
using LYZD.Utility.Log;
using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;

namespace LYZD.TerminalProtocol.Encryption
{
    /// <summary>
    /// 加密机接口函数,
    /// </summary>
    public class IEncryptionFunction698
    {
        public static string sPutIP = "";
        public static string sPutPort = "";
        public static string sPutCtime = "";
        /// <summary>
        /// 加密机类型,1为密钥服务器，2为直连机密机
        /// </summary>
        public static int EncryptionMachineType = 1;

        public static string strEncryptionType = ConfigHelper.Instance.Dog_Type; //配置文件取密码机类型

        public static bool isNewLink = false; //重新连接加密机
        /// <summary>
        /// 连接加密机
        /// </summary>
        /// <param name="PutIP">ip 地址字符型</param>
        /// <param name="PutPort">密码机端口号,短整型；</param>
        /// <param name="PutCtime">字符型，单位秒；</param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public static int ConnectDevice(string PutIP, string PutPort, string PutCtime)
        {
            try
            {
                int ret = 0;

                sPutIP = PutIP;
                sPutPort = PutPort;
                sPutCtime = PutCtime;

                if (strEncryptionType == "直连型")
                {
                    ret = FactoryEncryptionFunction698.ConnectDevice(PutIP, PutPort, PutCtime);
                }
                else
                {
                    ret = EncryptionFunction698.ConnectDevice(PutIP, PutPort, PutCtime);
                }

                return ret;
            }
            catch (Exception ex)
            {
                //WriteLog("\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【ConnectDevice】" + ex.Message + "\r\n" + ex.ToString(), "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal0");
                LogManager.AddMessage("连接加密机失败【1】" + ex.Message + "\r\n" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.TipsError);
                //throw;
                return -1;
            }

        }

        [HandleProcessCorruptedStateExceptions]
        public static int ConnectDevice(bool[] m_b_TerminalSelect, string PutIP, string PutPort, string PutCtime)
        {
            try
            {
                int ret = 0;

                for (int i = 0; i < m_b_TerminalSelect.Length; i++)
                {
                    if (m_b_TerminalSelect[i])
                    {
                        //WriteLog("-------------------------------------------------" + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【ConnectDevice】" + "\r\n" + "PutIP = " + PutIP + "\r\n" + "PutPort = " + PutPort + "\r\n" + "PutCtime = " + PutCtime, "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (i + 1));
                        LogManager.AddMessage(i + 1, "连接加密机【2】" + "\r\n" + "PutIP = " + PutIP + "\r\n" + "PutPort = " + PutPort + "\r\n" + "PutCtime = " + PutCtime, EnumLogSource.检定业务日志, EnumLevel.Information);
                    }
                }
                sPutIP = PutIP;
                sPutPort = PutPort;
                sPutCtime = PutCtime;

                if (strEncryptionType == "直连型")
                {
                    ret = FactoryEncryptionFunction698.ConnectDevice(PutIP, PutPort, PutCtime);
                }
                else
                {
                    StringBuilder outRand = new StringBuilder(16);      //随机数
                    StringBuilder outEndata = new StringBuilder(16);    //密文
                                                                        //调用电表身份认证函数确认加密机
                    int KeyIsLive = EncryptionFunction698.Meter_Formal_IdentityAuthentication(1, "0000000000000031", outRand, outEndata);
                    if (KeyIsLive != 0)
                    {
                        ret = EncryptionFunction698.ConnectDevice(PutIP, PutPort, PutCtime);
                    }
                }

                for (int i = 0; i < m_b_TerminalSelect.Length; i++)
                {
                    if (m_b_TerminalSelect[i])
                    {
                        //WriteLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + " ret = " + ret + "\r\n" + "-------------------------------------------------", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (i + 1));
                        LogManager.AddMessage(i + 1, "连接加密机【2】" + "ret = " + ret, EnumLogSource.检定业务日志, EnumLevel.Information);
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage("连接加密机失败【2】" + ex.Message + "\r\n" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.TipsError);
                //WriteLog("\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【ConnectDevice】" + ex.Message + "\r\n" + ex.ToString(), "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal0");
                return -1;
            }

        }

        [HandleProcessCorruptedStateExceptions]
        public static int Obj_Terminal_Formal_InitSession(int index, int iKeyState, string cTESAMNO, string cASCTR, string cFLG, string cMasterCert, ref string cOutRandHost, ref string cOutSessionInit, ref string cOutSign)
        {
            try
            {
                int ret = 0;

                LogManager.AddMessage(index + 1, "主站会话协商", EnumLogSource.检定业务日志, EnumLevel.Information);
                for (int j = 0; j < 3; j++)
                {
                    //WriteLog("-------------------------------------------------" + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Terminal_Formal_InitSession】", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    if (j > 0) LogManager.AddMessage(index + 1, "主站会话协商" + "重发第" + j + "次", EnumLogSource.检定业务日志, EnumLevel.Information);
                    //WriteLog("重发第" + j + "次", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    StringBuilder strbu1 = new StringBuilder(10000);
                    StringBuilder strbu2 = new StringBuilder(10000);
                    StringBuilder strbu3 = new StringBuilder(10000);
                    if (strEncryptionType == "直连型")
                        ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_InitSession(iKeyState, cTESAMNO, cASCTR, cFLG, cMasterCert, strbu1, strbu2, strbu3);
                    else
                        ret = EncryptionFunction698.Obj_Terminal_Formal_InitSession(iKeyState, cTESAMNO, cASCTR, cFLG, cMasterCert, strbu1, strbu2, strbu3);
                    if (ret == 64)
                    {
                        LogManager.AddMessage(index + 1, "主站会话协商失败,错误码1", EnumLogSource.检定业务日志, EnumLevel.TipsError);
                        //WriteLog("终端" + (index + 1) + "会话协商失败1!", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal0");
                        if (strEncryptionType == "直连型")
                        {
                            // ret = FactoryEncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_InitSession(iKeyState, cTESAMNO, cASCTR, cFLG, cMasterCert, strbu1, strbu2, strbu3);
                        }
                        else
                        {
                            // ret = EncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = EncryptionFunction698.Obj_Terminal_Formal_InitSession(iKeyState, cTESAMNO, cASCTR, cFLG, cMasterCert, strbu1, strbu2, strbu3);
                        }
                    }

                    //cOutRandHost = GetYouXiaoShu(strbu1.ToString());
                    //cOutSessionInit = GetYouXiaoShu(strbu2.ToString());
                    //cOutSign = GetYouXiaoShu(strbu3.ToString());
                    //cOutSign = GBKToUTF8(strbu3.ToString().Replace("\0", ""));
                    cOutRandHost = GetYouXiaoShu(strbu1.ToString().Replace("\0", ""));
                    cOutSessionInit = GetYouXiaoShu(strbu2.ToString().Replace("\0", ""));
                    cOutSign = GetYouXiaoShu(strbu3.ToString().Replace("\0", ""));
                    if (cOutSign.Length != 128)
                    {
                        ret = 999;
                        LogManager.AddMessage(index + 1, "主站会话协商失败,错误码2", EnumLogSource.检定业务日志, EnumLevel.TipsError);
                        //WriteLog("终端" + (index + 1) + "会话协商失败2!", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal0");
                    }
                    if (cOutSign.Length > 128)
                        cOutSign = cOutSign.Substring(0, 128);
                    LogManager.AddMessage(index + 1, "iKeyState = " + iKeyState + "\r\n" + "cTESAMNO = " + cTESAMNO + "\r\n" + "cASCTR = " + cASCTR + "\r\n" + "cFLG = " + cFLG + "\r\n" + "cMasterCert = " + cMasterCert + "\r\n" + "cOutRandHost = " + cOutRandHost + "\r\n" + "cOutSessionInit = " + cOutSessionInit + "\r\n" + "cOutSign = " + cOutSign + "\r\n" + " ret = " + ret, EnumLogSource.检定业务日志, EnumLevel.Information);
                    //WriteLog("iKeyState = " + iKeyState + "\r\n" + "cTESAMNO = " + cTESAMNO + "\r\n" + "cASCTR = " + cASCTR + "\r\n" + "cFLG = " + cFLG + "\r\n" + "cMasterCert = " + cMasterCert + "\r\n" + "cOutRandHost = " + cOutRandHost + "\r\n" + "cOutSessionInit = " + cOutSessionInit + "\r\n" + "cOutSign = " + cOutSign + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + " ret = " + ret + "\r\n" + "-------------------------------------------------", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    if (ret == 0)
                        break;
                }
                if (ret != 0)
                {

                }
                return ret;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(index + 1, "主站会话协商失败" + ex.Message + "\r\n" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.TipsError);
                //WriteLog("\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Terminal_Formal_InitSession】" + ex.Message + "\r\n" + ex.ToString(), "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal0");
                //throw;
                return -1;
            }

        }


        /// <summary>
        /// 对称密钥明文加密
        /// </summary>
        /// <param name="index"></param>
        /// <param name="cOperateMode"></param>
        /// <param name="cTESAMID"></param>
        /// <param name="cSessionKey"></param>
        /// <param name="cTaskData"></param>
        /// <param name="cOutSID"></param>
        /// <param name="cOutAttachData"></param>
        /// <param name="OutTaskData"></param>
        /// <param name="cOutTaskMAC"></param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public static int Obj_Terminal_Formal_GetTerminalSetData(int index, int cOperateMode, string cTESAMID, string cSessionKey, string cTaskData, ref string cOutSID, ref string cOutAttachData, ref string OutTaskData, ref string cOutTaskMAC)
        {
            try
            {
                int ret = 0;
                LogManager.AddMessage(index + 1, "对称密钥明文加密", EnumLogSource.检定业务日志, EnumLevel.Information);
                for (int j = 0; j < 3; j++)
                {
                    //LogManager.AddMessage(index + 1, "主站会话协商失败" + ex.Message + "\r\n" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.TipsError);
                    //WriteLog("-------------------------------------------------" + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Terminal_Formal_GetTerminalSetData】", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    if (j > 0) LogManager.AddMessage(index + 1, "对称密钥明文加密" + "重发第" + j + "次", EnumLogSource.检定业务日志, EnumLevel.Warning);
                    //WriteLog("重发第" + j + "次", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));

                    StringBuilder strbu1 = new StringBuilder(10000);
                    StringBuilder strbu2 = new StringBuilder(10000);
                    StringBuilder strbu3 = new StringBuilder(10000);
                    StringBuilder strbu4 = new StringBuilder(10000);
                    if (strEncryptionType == "直连型")
                        ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_GetTerminalSetData(cOperateMode, cTESAMID, cSessionKey, cTaskData, strbu1, strbu2, strbu3, strbu4);
                    else
                        ret = EncryptionFunction698.Obj_Terminal_Formal_GetTerminalSetData(cOperateMode, cTESAMID, cSessionKey, cTaskData, strbu1, strbu2, strbu3, strbu4);
                    if (ret == 64 || ret == 65)
                    {
                        if (strEncryptionType == "直连型")
                        {
                            // ret = FactoryEncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_GetTerminalSetData(cOperateMode, cTESAMID, cSessionKey, cTaskData, strbu1, strbu2, strbu3, strbu4);
                        }
                        else
                        {
                            ret = EncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = EncryptionFunction698.Obj_Terminal_Formal_GetTerminalSetData(cOperateMode, cTESAMID, cSessionKey, cTaskData, strbu1, strbu2, strbu3, strbu4);
                        }
                    }

                    cOutSID = GetYouXiaoShu(strbu1.ToString().Replace("\0", ""));
                    cOutAttachData = GetYouXiaoShu(strbu2.ToString().Replace("\0", ""));
                    OutTaskData = GetYouXiaoShu(strbu3.ToString().Replace("\0", ""));
                    cOutTaskMAC = GetYouXiaoShu(strbu4.ToString().Replace("\0", ""));
                    LogManager.AddMessage(index + 1, "cOperateMode = " + cOperateMode + "\r\n" + "cTESAMNO = " + cTESAMID + "\r\n" + "cSessionKey = " + cSessionKey + "\r\n" + "cTaskData = " + cTaskData + "\r\n" + "cOutSID = " + cOutSID + "\r\n" + "cOutAttachData = " + cOutAttachData + "\r\n" + "OutTaskData = " + OutTaskData + "\r\n" + "cOutTaskMAC = " + cOutTaskMAC + "\r\n" + " ret = " + ret, EnumLogSource.检定业务日志, EnumLevel.Warning);
                    //WriteLog("cOperateMode = " + cOperateMode + "\r\n" + "cTESAMNO = " + cTESAMID + "\r\n" + "cSessionKey = " + cSessionKey + "\r\n" + "cTaskData = " + cTaskData + "\r\n" + "cOutSID = " + cOutSID + "\r\n" + "cOutAttachData = " + cOutAttachData + "\r\n" + "OutTaskData = " + OutTaskData + "\r\n" + "cOutTaskMAC = " + cOutTaskMAC + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + " ret = " + ret + "\r\n" + "-------------------------------------------------", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    if (ret == 0)
                        break;
                }
                if (ret != 0)
                {

                }
                return ret;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(index + 1, "cOperateMode = " + cOperateMode + "\r\n" + "cTESAMNO = " + cTESAMID + "\r\n" + "cSessionKey = " + cSessionKey + "\r\n" + "cTaskData = " + cTaskData + "\r\n" + "cOutSID = " + cOutSID + "\r\n" + "cOutAttachData = " + cOutAttachData + "\r\n" + "OutTaskData = " + OutTaskData + "\r\n" + "cOutTaskMAC = " + cOutTaskMAC + "\r\n", EnumLogSource.检定业务日志, EnumLevel.Warning);
                LogManager.AddMessage(index + 1, "对称密钥明文加密失败" + ex.Message + "\r\n" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.TipsError);
                //WriteLog("cOperateMode = " + cOperateMode + "\r\n" + "cTESAMNO = " + cTESAMID + "\r\n" + "cSessionKey = " + cSessionKey + "\r\n" + "cTaskData = " + cTaskData + "\r\n" + "cOutSID = " + cOutSID + "\r\n" + "cOutAttachData = " + cOutAttachData + "\r\n" + "OutTaskData = " + OutTaskData + "\r\n" + "cOutTaskMAC = " + cOutTaskMAC + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ")  + "\r\n" + "-------------------------------------------------", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Termina0" );
                //WriteLog("\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Terminal_Formal_GetTerminalSetData】" + ex.Message + "\r\n" + ex.ToString(), "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal0");
                //throw;
                return -1;
            }
        }

        /// <summary>
        /// 安全传输加密
        /// </summary>
        /// <param name="index"></param>
        /// <param name="cOperateMode"></param>
        /// <param name="cTESAMID"></param>
        /// <param name="cSessionKey"></param>
        /// <param name="cTaskType"></param>
        /// <param name="cTaskData"></param>
        /// <param name="cOutSID"></param>
        /// <param name="cOutAttachData"></param>
        /// <param name="OutTaskData"></param>
        /// <param name="cOutTaskMAC"></param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public static int Obj_Terminal_Formal_GetSessionData(int index, int cOperateMode, string cTESAMID, string cSessionKey, int cTaskType, string cTaskData, ref string cOutSID, ref string cOutAttachData, ref string OutTaskData, ref string cOutTaskMAC)
        {
            try
            {
                int ret = 0;
                LogManager.AddMessage(index + 1, "安全传输加密", EnumLogSource.检定业务日志, EnumLevel.Information);
                for (int j = 0; j < 3; j++)
                {
                    //WriteLog("-------------------------------------------------" + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Terminal_Formal_GetSessionData】", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));

                    if (j > 0) LogManager.AddMessage(index + 1, "安全传输加密" + "重发第" + j + "次", EnumLogSource.检定业务日志, EnumLevel.Warning);
                    //WriteLog("重发第" + j + "次", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));

                    StringBuilder strbu1 = new StringBuilder(10000);
                    StringBuilder strbu2 = new StringBuilder(10000);
                    StringBuilder strbu3 = new StringBuilder(10000);
                    StringBuilder strbu4 = new StringBuilder(10000);
                    if (strEncryptionType == "直连型")
                        ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_GetSessionData(cOperateMode, cTESAMID, cSessionKey, cTaskType, cTaskData, strbu1, strbu2, strbu3, strbu4);
                    else
                        ret = EncryptionFunction698.Obj_Terminal_Formal_GetSessionData(cOperateMode, cTESAMID, cSessionKey, cTaskType, cTaskData, strbu1, strbu2, strbu3, strbu4);
                    if (ret == 64)
                    {

                        if (strEncryptionType == "直连型")
                        {
                            // ret = FactoryEncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_GetSessionData(cOperateMode, cTESAMID, cSessionKey, cTaskType, cTaskData, strbu1, strbu2, strbu3, strbu4);
                        }
                        else
                        {
                            ret = EncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = EncryptionFunction698.Obj_Terminal_Formal_GetSessionData(cOperateMode, cTESAMID, cSessionKey, cTaskType, cTaskData, strbu1, strbu2, strbu3, strbu4);
                        }
                    }
                    cOutSID = GetYouXiaoShu(strbu1.ToString().Replace("\0", ""));
                    cOutAttachData = GetYouXiaoShu(strbu2.ToString().Replace("\0", ""));
                    OutTaskData = GetYouXiaoShu(strbu3.ToString().Replace("\0", ""));
                    cOutTaskMAC = GetYouXiaoShu(strbu4.ToString().Replace("\0", ""));

                    LogManager.AddMessage(index + 1, "cOperateMode = " + cOperateMode + "\r\n" + "cTESAMNO = " + cTESAMID + "\r\n" + "cSessionKey = " + cSessionKey + "\r\n" + "cTaskType = " + cTaskType + "\r\n" + "cTaskData = " + cTaskData + "\r\n" + "cOutSID = " + cOutSID + "\r\n" + "cOutAttachData = " + cOutAttachData + "\r\n" + "OutTaskData = " + OutTaskData + "\r\n" + "cOutTaskMAC = " + cOutTaskMAC + "\r\n" + " ret = " + ret, EnumLogSource.检定业务日志, EnumLevel.Information);
                    if (ret == 0)
                        break;
                }
                if (ret != 0)
                {

                }
                return ret;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(index + 1, "安全传输加密" + ex.Message + "\r\n" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.TipsError);
                return -1;
            }
        }

        /// <summary>
        /// 对称密钥更新
        /// </summary>
        /// <param name="index"></param>
        /// <param name="iKeyState"></param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cSessionKey"></param>
        /// <param name="cTerminalAddress"></param>
        /// <param name="cKeyType"></param>
        /// <param name="cOutSID"></param>
        /// <param name="cOutAttachData"></param>
        /// <param name="cOutTrmKeyData"></param>
        /// <param name="cOutMAC"></param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public static int Obj_Terminal_Formal_GetTrmKeyData(int index, int iKeyState, string cTESAMNO, string cSessionKey, string cTerminalAddress, string cKeyType, ref string cOutSID, ref string cOutAttachData, ref string cOutTrmKeyData, ref string cOutMAC)
        {
            try
            {
                int ret = 0;

                LogManager.AddMessage(index + 1, "对称密钥更新", EnumLogSource.检定业务日志, EnumLevel.Information);
                for (int j = 0; j < 3; j++)
                {
                    if (j > 0) LogManager.AddMessage(index + 1, "对称密钥更新" + "重发第" + j + "次", EnumLogSource.检定业务日志, EnumLevel.Warning);
                    StringBuilder strbu1 = new StringBuilder(10000);
                    StringBuilder strbu2 = new StringBuilder(10000);
                    StringBuilder strbu3 = new StringBuilder(10000);
                    StringBuilder strbu4 = new StringBuilder(10000);
                    if (strEncryptionType == "直连型")
                        ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_GetTrmKeyData(iKeyState, cTESAMNO, cSessionKey, cTerminalAddress, cKeyType, strbu1, strbu2, strbu3, strbu4);
                    else
                        ret = EncryptionFunction698.Obj_Terminal_Formal_GetTrmKeyData(iKeyState, cTESAMNO, cSessionKey, cTerminalAddress, cKeyType, strbu1, strbu2, strbu3, strbu4);
                    if (ret == 64)
                    {
                        if (strEncryptionType == "直连型")
                        {
                            // ret = FactoryEncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_GetTrmKeyData(iKeyState, cTESAMNO, cSessionKey, cTerminalAddress, cKeyType, strbu1, strbu2, strbu3, strbu4);
                        }
                        else
                        {
                            //  ret = EncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = EncryptionFunction698.Obj_Terminal_Formal_GetTrmKeyData(iKeyState, cTESAMNO, cSessionKey, cTerminalAddress, cKeyType, strbu1, strbu2, strbu3, strbu4);
                        }
                    }
                    cOutSID = GetYouXiaoShu(strbu1.ToString().Replace("\0", ""));
                    cOutAttachData = GetYouXiaoShu(strbu2.ToString().Replace("\0", ""));
                    cOutTrmKeyData = GetYouXiaoShu(strbu3.ToString().Replace("\0", ""));
                    cOutMAC = GetYouXiaoShu(strbu4.ToString().Replace("\0", ""));

                    //    WriteLog("iKeyState = " + iKeyState + "\r\n" +
                    //        "cTESAMNO = " + cTESAMNO + "\r\n" +
                    //        "cSessionKey = " + cSessionKey + "\r\n" +
                    //        "cTerminalAddress = " + cTerminalAddress + "\r\n" +
                    //        "cKeyType = " + cKeyType + "\r\n" +
                    //        "cOutSID = " + cOutSID + "\r\n" +
                    //        "cOutAttachData = " + cOutAttachData + "\r\n" +
                    //        "cOutTrmKeyData = " + cOutTrmKeyData + "\r\n" +
                    //        "cOutMAC = " + cOutMAC + "\r\n" +
                    //        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + " ret = " + ret + "\r\n" + "-------------------------------------------------", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));


                    LogManager.AddMessage(index + 1, "iKeyState = " + iKeyState + "\r\n" +
                                                "cTESAMNO = " + cTESAMNO + "\r\n" +
                                                "cSessionKey = " + cSessionKey + "\r\n" +
                                                "cTerminalAddress = " + cTerminalAddress + "\r\n" +
                                                "cKeyType = " + cKeyType + "\r\n" +
                                                "cOutSID = " + cOutSID + "\r\n" +
                                                "cOutAttachData = " + cOutAttachData + "\r\n" +
                                                "cOutTrmKeyData = " + cOutTrmKeyData + "\r\n" +
                                                "cOutMAC = " + cOutMAC + "\r\n" + " ret = " + ret, EnumLogSource.检定业务日志, EnumLevel.Information);

                    if (ret == 0)
                        break;
                }
                if (ret != 0)
                {

                }
                return ret;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(index + 1, "对称密钥更新" + ex.Message + "\r\n" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.TipsError);
                //WriteLog("\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Terminal_Formal_GetTrmKeyData】" + ex.Message + "\r\n" + ex.ToString(), "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal0");
                return -1;
            }
        }

        /// <summary>
        /// 获取证书信息
        /// </summary>
        /// <param name="index"></param>
        /// <param name="iKeyState"></param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cSessionKey"></param>
        /// <param name="ucCerType"></param>
        /// <param name="cOutSID"></param>
        /// <param name="ucOutAttachData"></param>
        /// <param name="ucOutCertData"></param>
        /// <param name="cOutMAC"></param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public static int Obj_Terminal_Formal_GetCACertificateData(int index, int iKeyState, string cTESAMNO, string cSessionKey, string ucCerType, ref string cOutSID, ref string ucOutAttachData, ref string ucOutCertData, ref string cOutMAC)
        {
            try
            {


                int ret = 0;
                LogManager.AddMessage(index + 1, "获取证书信息", EnumLogSource.检定业务日志, EnumLevel.Information);
                for (int j = 0; j < 3; j++)
                {
                    //WriteLog("-------------------------------------------------" + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Terminal_Formal_GetCACertificateData】", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    if (j > 0) LogManager.AddMessage(index + 1, "获取证书信息" + "重发第" + j + "次", EnumLogSource.检定业务日志, EnumLevel.Warning);
                    //WriteLog("重发第" + j + "次", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    StringBuilder strbu1 = new StringBuilder(10000);
                    StringBuilder strbu2 = new StringBuilder(10000);
                    StringBuilder strbu3 = new StringBuilder(10000);
                    StringBuilder strbu4 = new StringBuilder(10000);
                    if (strEncryptionType == "直连型")
                        ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_GetCACertificateData(iKeyState, cTESAMNO, cSessionKey, ucCerType, strbu1, strbu2, strbu3, strbu4);
                    else
                        ret = EncryptionFunction698.Obj_Terminal_Formal_GetCACertificateData(iKeyState, cTESAMNO, cSessionKey, ucCerType, strbu1, strbu2, strbu3, strbu4);
                    if (ret == 64)
                    {
                        if (strEncryptionType == "直连型")
                        {
                            // ret = FactoryEncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_GetCACertificateData(iKeyState, cTESAMNO, cSessionKey, ucCerType, strbu1, strbu2, strbu3, strbu4);
                        }
                        else
                        {
                            //ret = EncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = EncryptionFunction698.Obj_Terminal_Formal_GetCACertificateData(iKeyState, cTESAMNO, cSessionKey, ucCerType, strbu1, strbu2, strbu3, strbu4);
                        }
                    }
                    cOutSID = GetYouXiaoShu(strbu1.ToString().Replace("\0", ""));
                    ucOutAttachData = GetYouXiaoShu(strbu2.ToString().Replace("\0", ""));
                    ucOutCertData = GetYouXiaoShu(strbu3.ToString().Replace("\0", ""));
                    cOutMAC = GetYouXiaoShu(strbu4.ToString().Replace("\0", ""));

                    LogManager.AddMessage(index + 1, "iKeyState = " + iKeyState + "\r\n" + "cTESAMNO = " + cTESAMNO + "\r\n" + "cSessionKey = " + cSessionKey + "\r\n" + "ucCerType = " + ucCerType + "\r\n" + "cOutSID = " + cOutSID + "\r\n" + "ucOutAttachData = " + ucOutAttachData + "\r\n" + "ucOutCertData = " + ucOutCertData + "\r\n" + "cOutMAC = " + cOutMAC + "\r\n" + " ret = " + ret, EnumLogSource.检定业务日志, EnumLevel.Information);

                    //WriteLog("iKeyState = " + iKeyState + "\r\n" + "cTESAMNO = " + cTESAMNO + "\r\n" + "cSessionKey = " + cSessionKey + "\r\n" + "ucCerType = " + ucCerType + "\r\n" + "cOutSID = " + cOutSID + "\r\n" + "ucOutAttachData = " + ucOutAttachData + "\r\n" + "ucOutCertData = " + ucOutCertData + "\r\n" + "cOutMAC = " + cOutMAC + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + " ret = " + ret + "\r\n" + "-------------------------------------------------", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    if (ret == 0)
                        break;
                }
                if (ret != 0)
                {

                }
                return ret;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(index + 1, "获取证书信息" + ex.Message + "\r\n" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.TipsError);
                //WriteLog("\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Terminal_Formal_GetCACertificateData】" + ex.Message + "\r\n" + ex.ToString(), "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal0");
                return -1;
            }
        }
        /// <summary>
        /// 主站会话协商验证
        /// </summary>
        /// <param name="index"></param>
        /// <param name="iKeyState"></param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cRandHost"></param>
        /// <param name="cSessionData"></param>
        /// <param name="cSign"></param>
        /// <param name="cTerminalCert"></param>
        /// <param name="cOutSessionKey"></param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public static int Obj_Terminal_Formal_VerifySession(int index, int iKeyState, string cTESAMNO, string cRandHost, string cSessionData, string cSign, string cTerminalCert, ref string cOutSessionKey)
        {
            try
            {
                int ret = 0;
                LogManager.AddMessage(index + 1, "主站会话协商验证", EnumLogSource.检定业务日志, EnumLevel.Information);
                for (int j = 0; j < 3; j++)
                {
                    //WriteLog("-------------------------------------------------" + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Terminal_Formal_VerifySession】", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    if (j > 0) LogManager.AddMessage(index + 1, "主站会话协商验证" + "重发第" + j + "次", EnumLogSource.检定业务日志, EnumLevel.Warning);
                    //WriteLog("重发第" + j + "次", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));

                    StringBuilder strbu1 = new StringBuilder(10000);
                    if (strEncryptionType == "直连型")
                        ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_VerifySession(iKeyState, cTESAMNO, cRandHost, cSessionData, cSign, cTerminalCert, strbu1);
                    else
                        ret = EncryptionFunction698.Obj_Terminal_Formal_VerifySession(iKeyState, cTESAMNO, cRandHost, cSessionData, cSign, cTerminalCert, strbu1);
                    if (ret == 64)
                    {
                        if (strEncryptionType == "直连型")
                        {
                            //   ret = FactoryEncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_VerifySession(iKeyState, cTESAMNO, cRandHost, cSessionData, cSign, cTerminalCert, strbu1);
                        }
                        else
                        {
                            // ret = EncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = EncryptionFunction698.Obj_Terminal_Formal_VerifySession(iKeyState, cTESAMNO, cRandHost, cSessionData, cSign, cTerminalCert, strbu1);
                        }
                    }
                    cOutSessionKey = GetYouXiaoShu(strbu1.ToString().Replace("\0", ""));

                    LogManager.AddMessage(index + 1, "iKeyState = " + iKeyState + "\r\n" + "cTESAMNO = " + cTESAMNO + "\r\n" + "cRandHost = " + cRandHost + "\r\n" + "cSessionData = " + cSessionData + "\r\n" + "cSign = " + cSign + "\r\n" + "cTerminalCert = " + cTerminalCert + "\r\n" + "cOutSessionKey = " + cOutSessionKey + "\r\n" + " ret = " + ret, EnumLogSource.检定业务日志, EnumLevel.Information);

                    //WriteLog("iKeyState = " + iKeyState + "\r\n" + "cTESAMNO = " + cTESAMNO + "\r\n" + "cRandHost = " + cRandHost + "\r\n" + "cSessionData = " + cSessionData + "\r\n" + "cSign = " + cSign + "\r\n" + "cTerminalCert = " + cTerminalCert + "\r\n" + "cOutSessionKey = " + cOutSessionKey + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + " ret = " + ret + "\r\n" + "-------------------------------------------------", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    if (ret == 0)
                        break;
                    System.Threading.Thread.Sleep(1000);
                }
                if (ret != 0)
                {

                }
                return ret;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(index + 1, "主站会话协商验证" + ex.Message + "\r\n" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.TipsError);
                //WriteLog("\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Terminal_Formal_VerifySession】" + ex.Message + "\r\n" + ex.ToString(), "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal0");
                return -1;
            }
        }

        /// <summary>
        /// 安全传输解密
        /// </summary>
        /// <param name="index"></param>
        /// <param name="iKeyState"></param>
        /// <param name="iOperateMode"></param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cSessionKey"></param>
        /// <param name="cTaskData"></param>
        /// <param name="cMac"></param>
        /// <param name="cOutData"></param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public static int Obj_Terminal_Formal_VerifyTerminalData(int index, int iKeyState, int iOperateMode, string cTESAMNO, string cSessionKey, string cTaskData, string cMac, ref string cOutData)
        {
            try
            {

                int ret = 0;
                LogManager.AddMessage(index + 1, "安全传输解密", EnumLogSource.检定业务日志, EnumLevel.Information);
                for (int j = 0; j < 3; j++)
                {
                    //WriteLog("-------------------------------------------------" + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Terminal_Formal_VerifyTerminalData】", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    if (j > 0) LogManager.AddMessage(index + 1, "安全传输解密" + "重发第" + j + "次", EnumLogSource.检定业务日志, EnumLevel.Warning);
                    //WriteLog("重发第" + j + "次", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));

                    StringBuilder strbu1 = new StringBuilder(10000);
                    if (strEncryptionType == "直连型")
                        ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(iKeyState, iOperateMode, cTESAMNO, cSessionKey, cTaskData, cMac, strbu1);
                    else
                        ret = EncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(iKeyState, iOperateMode, cTESAMNO, cSessionKey, cTaskData, cMac, strbu1);
                    if (ret == 64)
                    {
                        if (strEncryptionType == "直连型")
                        {
                            // ret = FactoryEncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(iKeyState, iOperateMode, cTESAMNO, cSessionKey, cTaskData, cMac, strbu1);
                        }
                        else
                        {
                            ret = EncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = EncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(iKeyState, iOperateMode, cTESAMNO, cSessionKey, cTaskData, cMac, strbu1);
                        }
                    }
                    cOutData = GetYouXiaoShu(strbu1.ToString().Replace("\0", ""));
                    LogManager.AddMessage(index + 1, "iKeyState = " + iKeyState + "\r\n" + "iOperateMode = " + iOperateMode + "\r\n" + "cTESAMNO = " + cTESAMNO + "\r\n" + "cSessionKey = " + cSessionKey + "\r\n" + "cTaskData = " + cTaskData + "\r\n" + "cMac = " + cMac + "\r\n" + "cOutData = " + cOutData + "\r\n" + " ret = " + ret, EnumLogSource.检定业务日志, EnumLevel.Information);
                    //WriteLog("iKeyState = " + iKeyState + "\r\n" + "iOperateMode = " + iOperateMode + "\r\n" + "cTESAMNO = " + cTESAMNO + "\r\n" + "cSessionKey = " + cSessionKey + "\r\n" + "cTaskData = " + cTaskData + "\r\n" + "cMac = " + cMac + "\r\n" + "cOutData = " + cOutData + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + " ret = " + ret + "\r\n" + "-------------------------------------------------", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    if (ret == 0)
                        break;
                }
                if (ret != 0)
                {

                }
                return ret;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(index + 1, "安全传输解密" + ex.Message + "\r\n" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.TipsError);
                //WriteLog("\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Terminal_Formal_VerifyTerminalData】" + ex.Message + "\r\n" + ex.ToString(), "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal0");
                return -1;
            }
        }
        /// <summary>
        /// 抄读数据验证：主站验证设备返回的抄读数据，具体指抄读终端返回的数据。 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="iKeyState"></param>
        /// <param name="iOperateMode"></param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cRandHost"></param>
        /// <param name="cReadData"></param>
        /// <param name="cMac"></param>
        /// <param name="cOutData"></param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public static int Obj_Terminal_Formal_VerifyReadData(int index, int iKeyState, int iOperateMode, string cTESAMNO, string cRandHost, string cReadData, string cMac, ref string cOutData)
        {
            try
            {


                int ret = 0;
                LogManager.AddMessage(index + 1, "抄读数据验证", EnumLogSource.检定业务日志, EnumLevel.Information);
                for (int j = 0; j < 3; j++)
                {
                    //WriteLog("-------------------------------------------------" + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Terminal_Formal_VerifyReadData】", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    if (j > 0) LogManager.AddMessage(index + 1, "抄读数据验证" + "重发第" + j + "次", EnumLogSource.检定业务日志, EnumLevel.Warning);
                    //WriteLog("重发第" + j + "次", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));

                    StringBuilder strbu1 = new StringBuilder(10000);
                    if (strEncryptionType == "直连型")
                    {
                        ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(iKeyState, iOperateMode, cTESAMNO, cRandHost, cReadData, cMac, strbu1);
                    }
                    else
                    {
                        ret = EncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(iKeyState, iOperateMode, cTESAMNO, cRandHost, cReadData, cMac, strbu1);
                    }
                    //add jx 在加密机客户端发送/接受数据失败的时候重新连接加密机
                    if (ret == 65 || ret == 64)
                    {
                        if (strEncryptionType == "直连型")
                        {
                            // ret = FactoryEncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = FactoryEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(iKeyState, iOperateMode, cTESAMNO, cRandHost, cReadData, cMac, strbu1);
                        }
                        else
                        {
                            ret = EncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            if (ret == 0)
                            {
                                ret = EncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(iKeyState, iOperateMode, cTESAMNO, cRandHost, cReadData, cMac, strbu1);
                            }
                            else
                            {
                                LogManager.AddMessage("解析数据加密机重连失败" + " ret = " + ret, EnumLogSource.检定业务日志, EnumLevel.Information);
                            }

                        }

                    }
                    cOutData = GetYouXiaoShu(strbu1.ToString());
                    LogManager.AddMessage(index + 1, "iKeyState = " + iKeyState + "\r\n" + "iOperateMode = " + iOperateMode + "\r\n" + "cTESAMNO = " + cTESAMNO + "\r\n" + "cRandHost = " + cRandHost + "\r\n" + "cReadData = " + cReadData + "\r\n" + "cMac = " + cMac + "\r\n" + "cOutData = " + cOutData + "\r\n" + " ret = " + ret, EnumLogSource.检定业务日志, EnumLevel.Information);
                    //WriteLog("iKeyState = " + iKeyState + "\r\n" + "iOperateMode = " + iOperateMode + "\r\n" + "cTESAMNO = " + cTESAMNO + "\r\n" + "cRandHost = " + cRandHost + "\r\n" + "cReadData = " + cReadData + "\r\n" + "cMac = " + cMac + "\r\n" + "cOutData = " + cOutData + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + " ret = " + ret + "\r\n" + "-------------------------------------------------", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    if (ret == 0)
                        break;
                }
                if (ret != 0)
                {

                }
                return ret;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(index + 1, "抄读数据验证" + ex.Message + "\r\n" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.TipsError);
                //WriteLog("\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Terminal_Formal_VerifyReadData】" + ex.Message + "\r\n" + ex.ToString(), "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal0");
                return -1;
            }
        }
        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="index"></param>
        /// <param name="cOutRandHost"></param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public static int Obj_Formal_GetRandHost(int index, ref string cOutRandHost)
        {
            try
            {


                int ret = 0;
                LogManager.AddMessage(index + 1, "获取随机数", EnumLogSource.检定业务日志, EnumLevel.Information);

                for (int j = 0; j < 3; j++)
                {
                    //WriteLog("-------------------------------------------------" + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "【Obj_Formal_GetRandHost】", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    if (j > 0) LogManager.AddMessage(index + 1, "获取随机数" + "重发第" + j + "次", EnumLogSource.检定业务日志, EnumLevel.Warning);
                    //WriteLog("重发第" + j + "次", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));

                    StringBuilder strbu1 = new StringBuilder(10000);
                    if (strEncryptionType == "直连型")
                        ret = FactoryEncryptionFunction698.Obj_Formal_GetRandHost(strbu1);
                    else
                        ret = EncryptionFunction698.Obj_Formal_GetRandHost(strbu1);
                    if (ret == 64)
                    {
                        if (strEncryptionType == "直连型")
                        {
                            // ret = FactoryEncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            ret = FactoryEncryptionFunction698.Obj_Formal_GetRandHost(strbu1);
                        }
                        else
                        {
                            ret = EncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                            if (ret == 0)
                            {
                                ret = EncryptionFunction698.Obj_Formal_GetRandHost(strbu1);
                            }
                            else
                            {
                                LogManager.AddMessage("解析数据加密机重连失败" + " ret = " + ret, EnumLogSource.检定业务日志, EnumLevel.Information);
                            }
                        }
                    }
                    cOutRandHost = GetYouXiaoShu(strbu1.ToString().Replace("\0", ""));
                    LogManager.AddMessage(index + 1, "cOutRandHost = " + cOutRandHost + "\r\n" + " ret = " + ret, EnumLogSource.检定业务日志, EnumLevel.Information);
                    //WriteLog("cOutRandHost = " + cOutRandHost + "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + " ret = " + ret + "\r\n" + "-------------------------------------------------", "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Terminal" + (index + 1));
                    if (ret == 0)
                        break;
                }
                if (ret != 0)
                {

                }
                return ret;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(index + 1, "获取随机数" + ex.Message + "\r\n" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.TipsError);
                return -1;
            }

        }

        public static object obj = new object();
        /// <summary>
        /// 虚拟表获取MAC地址
        /// </summary>
        /// <param name="iKeyState"></param>
        /// <param name="cOperateMode"></param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cRandHost"></param>
        /// <param name="cReadData"></param>
        /// <param name="cOutData"></param>
        /// <param name="cOutMAC"></param>
        /// <returns></returns>
        public static int iObj_Meter_Formal_GenReadData(int iKeyState, int cOperateMode, string cTESAMNO, string cRandHost, string cReadData, ref string cOutData, ref string cOutMAC)
        {
            lock (obj)
            {
                StringBuilder strbu1 = new StringBuilder(10000);
                StringBuilder strbu2 = new StringBuilder(10000);
                int ret = 0;

                //LogManager.AddMessage("获取MAC地址", EnumLogSource.检定业务日志, EnumLevel.Information);
                if (IEncryptionFunction698.strEncryptionType == "直连密码机版")
                {
                    ret = FactoryEncryptionFunction698.Obj_Meter_Formal_GenReadData(iKeyState, cOperateMode, cTESAMNO, cRandHost, cReadData, strbu1, strbu2); ;

                }
                else
                {
                    ret = EncryptionFunction698.iObj_Meter_Formal_GenReadData(iKeyState, cOperateMode, cTESAMNO, cRandHost, cReadData, strbu1, strbu2);
                    if (ret == 64)
                    {
                        ret = EncryptionFunction698.ConnectDevice(sPutIP, sPutPort, sPutCtime);
                        ret = EncryptionFunction698.iObj_Meter_Formal_GenReadData(iKeyState, cOperateMode, cTESAMNO, cRandHost, cReadData, strbu1, strbu2);
                    }
                }
                cOutData = GetYouXiaoShu(strbu1.ToString().Replace("\0", ""));
                cOutMAC = GetYouXiaoShu(strbu2.ToString().Replace("\0", ""));
                return ret;
            }
        }


        /// <summary>
        /// 获取有效的加密符号，加密机返回数据偶尔会带乱码，不进行处理程序有异常
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public static string GetYouXiaoShu(string strData)
        {
            string sTmp = "";
            for (int i = 0; i < strData.Length / 2; i++)
            {

                try
                {
                    int iData = Convert.ToInt16(strData.Substring(i * 2, 2), 16);
                    sTmp += strData.Substring(i * 2, 2);
                }
                catch
                {
                    break;
                }

            }
            return sTmp;
        }

        private static string GBKToUTF8(string str)
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding utf8;
            Encoding gbk;
            utf8 = Encoding.GetEncoding("UTF-8");
            gbk = Encoding.GetEncoding("gbk");
            byte[] gb = gbk.GetBytes(str);
            gb = Encoding.Convert(gbk, utf8, gb);
            return utf8.GetString(gb);

        }
        ///// <summary>
        ///// 将运行日志保存到文件
        ///// </summary>
        ///// <param name="info">信息内容</param>
        ///// <param name="str">文件名</param>
        //[HandleProcessCorruptedStateExceptions]
        //public static void WriteLog(string info, string str)
        //{

        //    string ls_filename;
        //    string ls_line;

        //    TextWriter s;
        //    StringWriter strWriter;
        //    try
        //    {
        //        ls_filename = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\" + str + ".log";
        //        string FilePath = Path.GetDirectoryName(ls_filename);
        //        if (!Directory.Exists(FilePath))
        //            Directory.CreateDirectory(FilePath);
        //        strWriter = new StringWriter();
        //        s = new StreamWriter(ls_filename, true, System.Text.Encoding.Default);

        //        ls_line = info;
        //        s.WriteLine(ls_line);
        //        s.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //}
    }
}
