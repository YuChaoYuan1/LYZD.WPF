using LYZD.Utility.Log;
using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LYZD.TerminalProtocol.Encryption
{
    /// <summary>
    /// 698密钥服务器加密机函数
    /// </summary>
    public class EncryptionFunction698
    {
        ///3.3.1. 身份认证函数
        /// <summary>
        /// 功能描述：从密码机获取随机数以及密文,用于远程身份认证。分散因子用实际表号。2013
        /// </summary>
        /// <param name="Flag">整型, 0:公钥状态;1,私钥状态</param>
        /// <param name="PutDiv">输入的分散因子,字符型,长度16, “0000”+表号</param>
        /// <param name="OutRand">输出的随机数1,字符型,长度16</param>
        /// <param name="OutEndata">输出的密文,字符型,长度16</param>
        /// <returns>0 成功，其他 失败</returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Meter_Formal_IdentityAuthentication(int Flag, string PutDiv, StringBuilder OutRand, StringBuilder OutEndata);


        // <summary>
        /// 登录服务器函数
        /// </summary>
        /// <param name="ip">服务器IP</param>
        /// <param name="port">服务器端口</param>
        /// <param name="time">字符型，单位秒，默认10</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int ConnectDevice(string PutIP, string PutPort, string PutCtime);

        /// <summary>
        /// 断开服务器连接函数
        /// </summary>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int CloseDevice();

        /// <summary>
        /// 获取随机数 
        /// </summary>
        /// <param name="OutR1">16字节随机数</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_GetR1(StringBuilder OutR1);

        /// <summary>
        /// 会话初始化或恢复 
        /// </summary>
        /// <param name="PutState">对称密钥状态，  00‐‐第一套密钥，01‐‐第二套密钥</param>
        /// <param name="PutTESAMNo">TESAM 序列号，8字节</param>
        /// <param name="PutVersionNum">版本号，1字节</param>
        /// <param name="PutSessionID">会话 ID；1 字节；00‐‐新建注册，01‐‐恢复</param>
        /// <param name="PutR1">随机数 1，字符型，16字节</param>
        /// <param name="OutCertificate">主站证书(大于 1K，小于 2K)</param>
        /// <param name="OutEncR1">随机数 1密文，16字节</param>
        /// <param name="OutMac">Mac，4字节</param>
        /// <param name="OutSign1">签名 64字节</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_SessionInitRec(string PutState, string PutTESAMNo, string PutVersionNum, string PutSessionID, string PutR1, StringBuilder OutCertificate, StringBuilder OutEncR1, StringBuilder OutMac, StringBuilder OutSign1);

        /// <summary>
        /// 会话协商 
        /// </summary>
        /// <param name="PutState">芯片状态，  00‐‐第一套密钥，测试证书；11‐‐第二套密钥，正式证书使用） 。调用此函数时，对称密钥版是”00”，  对称密钥版本非全’0’时，须将证书状态切换到对应的状态） 。</param>
        /// <param name="PutTESAMNo">TESAM 序列号，8字节</param>
        /// <param name="PutVersionNum">1字节版本</param>
        /// <param name="PutSessionID">1字节会话ID</param>
        /// <param name="PutCRLCertificateNo">16字节主站证书序列号</param>
        /// <param name="PutTerminalCertificate">终端证书</param>
        /// <param name="PutEncR2">16字节R2（随机书2）密文</param>
        /// <param name="PutSign2">64字节签名</param>
        /// <param name="OutEncM1">会话密钥密文,113字节</param>
        /// <param name="OutSign3">主站证书验证码，97 字节</param>
        /// <param name="OutMac2">Mac2，4字节</param>
        /// <param name="OutSign4">签名，64 字节</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_SessionKeyConsult(string PutState, string PutTESAMNo, string PutVersionNum, string PutSessionID, string PutCRLCertificateNo, string PutTerCertificateNo, string PutTerminalCertificate, string PutEncR2, string PutSign2, StringBuilder OutEncM1, StringBuilder OutSign3, StringBuilder OutMac2, StringBuilder OutSign4);

        /// <summary>
        /// 会话协商验证 
        /// </summary>
        /// <param name="PutR3">16字节随机数 3</param>
        /// <param name="PutMac3">4字节 MAC</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_SessionConsultVerify(string PutR3, string PutMac3);

        /// <summary>
        /// 会话恢复验证
        /// </summary>
        /// <param name="PutState">对称密钥状态，  00‐‐第一套密钥，01‐‐第二套密钥</param>
        /// <param name="PutTESAMNo">TESAM 序列号，8字节</param>
        /// <param name="PutVersionNum">1字节版本</param>
        /// <param name="PutSessionID">1字节会话ID</param>
        /// <param name="PutEncR2">16字节 R2（随机书2）密文</param>
        /// <param name="PutR3">16字节随机数 3</param>
        /// <param name="PutMac">4字节MAC</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_SessionRecoveryVerify(string PutState, string PutTESAMNo, string PutVersionNum, string PutSessionID, string PutEncR2, string PutR3, string PutMac);

        /// <summary>
        /// 单地址数据 MAC 计算 
        /// </summary>
        /// <param name="PutState">对称密钥状态，  00‐‐第一套密钥，01‐‐第二套密钥</param>
        /// <param name="PutTESAMNo">TESAM 序列号，字符型，8字节</param>
        /// <param name="PutKeyID">密钥索引，0x20≤PutkeyID≤0x33</param>
        /// <param name="PutData">计算 MAC的数据</param>
        /// <param name="OutMac">Mac，4字节</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_MACVerify(string PutState, string PutTESAMNo, string PutKeyID, string PutData, StringBuilder OutMac);

        /// <summary>
        /// 内外部认证
        /// </summary>
        /// <param name="PutState">对称密钥状态 00‐‐第一套密钥，01—第二套密钥</param>
        /// <param name="PutTESAMNo">TESAM 序列号，8字节</param>
        /// <param name="PutR4">随机数 4，16字节，可通过产生随机数函数产生</param>
        /// <param name="PutEncR4">随机数 4密文，终端返回</param>
        /// <param name="PutR5">随机数 5，终端返回16字节</param>
        /// <param name="OutEncR5">随机数 5密文，16字节</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_ExternalAuth(string PutState, string PutTESAMNo, string PutR4, string PutEncR4, string PutR5, StringBuilder OutEncR5);

        /// <summary>
        ///  证书状态切换
        /// </summary>
        /// <param name="PutState">对称密钥状态00‐‐第一套密钥，01—第二套密钥</param>
        /// <param name="PutTESAMNo">TESAM 序列号，8字节</param>
        /// <param name="PutCertificateState">证书状态，00—切换到测试证书，01—切换到正式证书</param>
        /// <param name="PutR6">随机数 6，16字节</param>
        /// <param name="OutEncR6">随机数 6密文，16字节</param>
        /// <param name="OutMac">Mac，4字节</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_CertificateStateChange(string PutState, string PutTESAMNo, string PutCertificateState, string PutR6, StringBuilder OutEncR6, StringBuilder OutMac);

        /// <summary>
        ///  设置离线计数器
        /// </summary>
        /// <param name="PutState">对称密钥状态00‐‐第一套密钥，01—第二套密钥</param>
        /// <param name="PutTESAMNo">TESAM 序列号，8字节</param>
        /// <param name="PutCounter">离线计数器数值，4字节</param>
        /// <param name="OutEncCounter">密文数据，20字节</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_SetOfflineCounter(string PutState, string PutTESAMNo, string PutCounter, StringBuilder OutEncCounter);

        /// <summary>
        ///  转加密授权
        /// </summary>
        /// <param name="OutData">32字节转加密授权数据</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_ChangeDataAuthorize(StringBuilder OutData);

        /// <summary>
        ///  获取电表密钥密文
        /// </summary>
        /// <param name="PutMeterState">电表密钥状态，1字节，0‐‐公开密钥；01—交易密钥</param>
        /// <param name="PutMeterNo">“0000”+电表表号，共 8字节</param>
        /// <param name="PutTaskType">任务类型：0，身份认证任务；1，对时任务；2，红外认证； </param>
        /// <param name="OutMeterEncKey">对应任务和表号的密钥密文，32 字节，如果多表，调用多次； </param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_GetCipherMeterKey(string PutMeterState, string PutMeterNo, int PutTaskType, StringBuilder OutMeterEncKey);

        /// <summary>
        ///  任务数据加密函数 
        /// </summary>
        /// <param name="PutInDataType">输入数据类型，对时任务当前为0；</param>
        /// <param name="PutTaskData">任务数据，字节数小于2k</param>
        /// <param name="OutTaskData">输出的任务密文</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_EncTaskData(int PutInDataType, string PutTaskData, StringBuilder OutTaskData);

        /// <summary>
        ///  组广播数据 MAC 计算
        /// </summary>
        /// <param name="PutState">对称密钥状态00‐‐第一套密钥，01—第二套密钥</param>
        /// <param name="PutTESAMNo">TESAM 序列号，8字节</param>
        /// <param name="PutFnType">命令类型，1字节： “01” ，复位； “04” ，设参； “05” ，控制； “10” ，数据转发； </param>
        /// <param name="PutOutDataType"> 输出数据类型：0，输出为明文数据的 MAC；1，输出为密文数据；2，输出数据为密文+MAC；</param>
        /// <param name="PutGroupAdrass"> 组地址，2字节</param>
        /// <param name="PutMtime">6字节，默认“130202224622” ；</param>
        /// <param name="PutBroadcastData">广播数据</param>
        /// <param name="OutMac">4字节 Mac</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_GroupBroadcast(string PutState, string PutTESAMNo, string PutFnType, int PutOutDataType, string PutGroupAdrass, string PutMtime, string PutBroadcastData, StringBuilder OutMac);

        /// <summary>
        ///  系统广播数据 MAC 计算
        /// </summary>
        /// <param name="PutState">对称密钥状态00‐‐第一套密钥，01—第二套密钥</param>
        /// <param name="PutTESAMNo">TESAM 序列号，8字节</param>
        /// <param name="PutFnType">命令类型，1字节： “01” ，复位； “04” ，设参； “05” ，控制； “10” ，数据转发； </param>
        /// <param name="PutOutDataType"> 输出数据类型：0，输出为明文数据的 MAC；1，输出为密文数据；2，输出数据为密文+MAC；</param>
        /// <param name="PutGroupAdrass"> 组地址，2字节</param>
        /// <param name="PutMtime">6字节，默认“130202224622” ；</param>
        /// <param name="PutBroadcastData">广播数据</param>
        /// <param name="OutMac">4字节 Mac</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_SystemBroadcast(string PutState, string PutTESAMNo, string PutFnType, int PutOutDataType, string PutGroupAdrass, string PutMtime, string PutBroadcastData, StringBuilder OutMac);

        /// <summary>
        ///  对称密钥修改 
        /// </summary>
        /// <param name="PutState">00‐‐修改到第一套密钥；01‐‐修改到第二套密钥；</param>
        /// <param name="PutTESAMNo">TESAM 序列号，8字节；</param>
        /// <param name="OutKeyNum">当前更新的密钥总条数，16进制字符串；</param>
        /// <param name="OutEncKeyData">密钥密文，长度为32* OutKeyNum；</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_SymmetricKeyUpdate(string PutState, string PutTESAMNo, StringBuilder OutKeyNum, StringBuilder OutEncKeyData);

        /// <summary>
        ///  证书更新
        /// </summary>
        /// <param name="PutCertificateState">  00‐‐修改测试证书；01‐‐修改正式证书到交易状态； 11‐‐恢复正式证书到初始状态；</param>
        /// <param name="PutCertificateType">证书类型，1字节；  01‐‐CRL 证书，其他保留；</param>
        /// <param name="OutEncCertificateData">证书密文，长度小于2k；</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Terminal_Formal_CACertificateUpdate(string PutCertificateState, string PutCertificateType, StringBuilder OutEncCertificateData);



        /// <summary>
        /// 主站会话协商：数字签名连接认证机制， 用于主站与设备进行会话协商时产生密文和签名数据，该过程在建立应用连接时完成。 
        /// </summary>
        /// <param name="iKeyState"></param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cASCTR"></param>
        /// <param name="cFLG">应用密钥产生标识，1Byte，默认”01”；</param>
        /// <param name="cMasterCert">主站证书； </param>
        /// <param name="cOutRandHost">主站随机数（16Byte）</param>
        /// <param name="cOutSessionInit">会话协商数据（32Byte） ，建立应用连接中的密文1；</param>
        /// <param name="cOutSign">协商数据签名(64Byte)  ，建立应用连接中的客户机签名 1；</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Obj_Terminal_Formal_InitSession(int iKeyState, string cTESAMNO, string cASCTR, string cFLG, string cMasterCert, StringBuilder cOutRandHost, StringBuilder cOutSessionInit, StringBuilder cOutSign);

        /// <summary>
        /// 主站会话协商验证：数字签名连接认证机制，用于主站验证设备会话协商时返回的数据，验证成功主站产生会话密钥。 
        /// </summary>
        /// <param name="iKeyState"></param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cRandHost">主站随机数 R1（16Byte） ； </param>
        /// <param name="cSessionData">终端返回的应用会话协商数据(48Byte)，对应建立应用连接中的密文 2；</param>
        /// <param name="cSign">终端返回的应用会话协商数据签名(64Byte)，对应建立应用连接中的签名数据 2； </param>
        /// <param name="cTerminalCert">终端证书</param>
        /// <param name="cOutSessionKey">会话密钥</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Obj_Terminal_Formal_VerifySession(int iKeyState, string cTESAMNO, string cRandHost, string cSessionData, string cSign, string cTerminalCert, StringBuilder cOutSessionKey);

        /// <summary>
        /// 抄读数据验证：主站验证设备返回的抄读数据，具体指抄读终端返回的数据。 
        /// </summary>
        /// <param name="iKeyState"></param>
        /// <param name="iOperateMode"></param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cRandHost">主站随机数(16Byte) </param>
        /// <param name="cReadData">抄读数据</param>
        /// <param name="cMac">数据</param>
        /// <param name="cOutData">明文抄读数据，iOperateMode=1，为空 </param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Obj_Terminal_Formal_VerifyReadData(int iKeyState, int iOperateMode, string cTESAMNO, string cRandHost, string cReadData, string cMac, StringBuilder cOutData);

        /// <summary>
        /// 上报数据验证：设备主动上报数据时，主站验证数据的合法性。
        /// </summary>
        /// <param name="iKeyState"></param>
        /// <param name="iOperateMode"></param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cRandT">终端随机数(12B) </param>
        /// <param name="cReportData">上报数据 </param>
        /// <param name="cMac">MAC 数据</param>
        /// <param name="cOutData">明文数据，iOperateMode=1，为空</param>
        /// <param name="cOutRSTCTR">主动上报随机数</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Obj_Terminal_Formal_VerifyReportData(int iKeyState, int iOperateMode, string cTESAMNO, string cRandT, string cReportData, string cMac, StringBuilder cOutData, StringBuilder cOutRSTCTR);

        /// <summary>
        /// 上报数据返回报文加密 ：用于设备主动上报主站返回帧数据加密计算。
        /// </summary>
        /// <param name="iKeyState"></param>
        /// <param name="iOperateMode"></param>
        /// <param name="TESAMNO"></param>
        /// <param name="RandHost">上报随机数，12Byte</param>
        /// <param name="cReportData">上报数据</param>
        /// <param name="OutSID"></param>
        /// <param name="OutAttachData"></param>
        /// <param name="cOutData"></param>
        /// <param name="ucOutMac"></param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Obj_Terminal_Formal_GetResponseData(int iKeyState, int iOperateMode, string TESAMNO, string RandHost, string cReportData, StringBuilder OutSID, StringBuilder OutAttachData, StringBuilder cOutData, StringBuilder ucOutMac);

        /// <summary>
        /// 安全传输加密：用于对具体业务数据进行数据加密计算。 
        /// </summary>
        /// <param name="iOperateMode"></param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cSessionKey">会话密钥 </param>
        /// <param name="cTaskType">参数类型： 2,  设置会话实效门限 4，安全模式参数，会话时效； 7，拉闸； 8，文件传输； 3，除上述操作外的数据加密。 </param>
        /// <param name="cTaskData">数据明文；NByte</param>
        /// <param name="cOutSID"></param>
        /// <param name="cOutAttachData"></param>
        /// <param name="cOutData"></param>
        /// <param name="cOutMAC"></param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Obj_Terminal_Formal_GetSessionData(int iOperateMode, string cTESAMNO, string cSessionKey, int cTaskType, string cTaskData, StringBuilder cOutSID, StringBuilder cOutAttachData, StringBuilder cOutData, StringBuilder cOutMAC);

        /// <summary>
        /// 安全传输解密 ：用于验证终端返回帧数据解密验证。 
        /// </summary>
        /// <param name="iKeyState"></param>
        /// <param name="iOperateMode"></param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cSessionKey"></param>
        /// <param name="cTaskData">数据</param>
        /// <param name="cMac">MAC 数据</param>
        /// <param name="cOutData">数据明文</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Obj_Terminal_Formal_VerifyTerminalData(int iKeyState, int iOperateMode, string cTESAMNO, string cSessionKey, string cTaskData, string cMac, StringBuilder cOutData);

        /// <summary>
        /// 广播数据加密 ：用于广播数据加密计算。 
        /// </summary>
        /// <param name="iKeyState"></param>
        /// <param name="iOperateMode"></param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cBrdCstAddr">广播地址； </param>
        /// <param name="AGSEQ">广播应用通信序列号，4Byte </param>
        /// <param name="cBrdCstData">广播数据明文； </param>
        /// <param name="cOutSID"></param>
        /// <param name="cOutAttachData"></param>
        /// <param name="cOutData"></param>
        /// <param name="cOutMac"></param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Obj_Terminal_Formal_GetGrpBrdCstData(int iKeyState, int iOperateMode, string cTESAMNO, string cBrdCstAddr, string AGSEQ, string cBrdCstData, StringBuilder cOutSID, StringBuilder cOutAttachData, StringBuilder cOutData, StringBuilder cOutMac);

        /// <summary>
        /// 终端对称密钥更新 ：用于对称密钥更新。 
        /// </summary>
        /// <param name="iKeyState"> 密钥更新的目标状态，1，代表更新到私钥，0代表恢复到初始密钥； </param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cSessionKey"></param>
        /// <param name="cTerminalAddress">终端地址(8 Bytes)</param>
        /// <param name="cKeyType">密钥类型，00 应用密钥，01 链路密钥 </param>
        /// <param name="cOutSID"></param>
        /// <param name="cOutAttachData"></param>
        /// <param name="cOutTrmKeyData"></param>
        /// <param name="cOutMAC"></param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Obj_Terminal_Formal_GetTrmKeyData(int iKeyState, string cTESAMNO, string cSessionKey, string cTerminalAddress, string cKeyType, StringBuilder cOutSID, StringBuilder cOutAttachData, StringBuilder cOutTrmKeyData, StringBuilder cOutMAC);

        /// <summary>
        ///  终端对称密钥初始化 ：用于对终端密钥进行初始化，会话计数器次数为1 时，须先对密钥进行初始化。 
        /// </summary>
        /// <param name="iKeyState"></param>
        /// <param name="cTESAMNO"></param>
        /// <param name="cSessionKey"></param>
        /// <param name="cTerminalAddress">终端地址(8 Bytes) </param>
        /// <param name="cKeyType">密钥类型，00 应用密钥 </param>
        /// <param name="cOutSID"></param>
        /// <param name="cOutAttachData"></param>
        /// <param name="cOutTrmKeyData"></param>
        /// <param name="cOutMAC"></param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Obj_Terminal_Formal_InitTrmKeyData(int iKeyState, string cTESAMNO, string cSessionKey, string cTerminalAddress, string cKeyType, StringBuilder cOutSID, StringBuilder cOutAttachData, StringBuilder cOutTrmKeyData, StringBuilder cOutMAC);

        /// <summary>
        ///  获取证书信息 ：用于对终端通密钥进行初始化，会话计数器次数为1 时，须先对密钥进行初始化。 
        /// </summary>
        /// <param name="iKeyState"></param>
        /// <param name="cTESAMNO">此处可为空</param>
        /// <param name="cSessionKey"></param>
        /// <param name="cCerType">证书类型， “01”</param>
        /// <param name="cOutSID"></param>
        /// <param name="cOutAttachData"></param>
        /// <param name="cOutCertificateData">证书数据 </param>
        /// <param name="cOutMAC"></param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Obj_Terminal_Formal_GetCACertificateData(int iKeyState, string cTESAMNO, string cSessionKey, string cCerType, StringBuilder cOutSID, StringBuilder cOutAttachData, StringBuilder cOutCertificateData, StringBuilder cOutMAC);


        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="cOutRandHost"></param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Obj_Formal_GetRandHost(StringBuilder cOutRandHost);


        /// <summary>
        /// 对称密钥明文加密
        /// </summary>
        /// <param name="cOutRandHost"></param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Obj_Terminal_Formal_GetTerminalSetData(int cOperateMode, string cTESAMID, string cSessionKey, string cTaskData, StringBuilder cOutSID, StringBuilder cOutAttachData, StringBuilder OutTaskData, StringBuilder cOutTaskMAC);

        // Obj_Terminal_Formal_GetTrmKeyData
        //Obj_Terminal_Formal_GetTrmKeyData
        //Obj_Terminal_Formal_GetTerminalSetData

        [DllImport(@"EncrypGW\DLL_SERVER_698\WinSocketServer.dll")]
        public static extern int Obj_Meter_Formal_GenReadData(int iKeyState, int cOperateMode, string cTESAMNO, string cRandHost, string cReadData, StringBuilder cOutData, StringBuilder cOutMAC);


        public static object obj = new object();

        [HandleProcessCorruptedStateExceptions]
        public static int iObj_Meter_Formal_GenReadData(int iKeyState, int cOperateMode, string cTESAMNO, string cRandHost, string cReadData, StringBuilder cOutData, StringBuilder cOutMAC)
        {
            lock (obj)
            {
                try
                {
                    //LogManager.AddMessage("正在计算MAC");
                    var t = Obj_Meter_Formal_GenReadData(iKeyState, cOperateMode, cTESAMNO, cRandHost, cReadData, cOutData, cOutMAC);
                    //LogManager.AddMessage("Ret值："+ t.ToString()+"计算MAC完成，返回值" + cOutMAC.ToString(), EnumLogSource.检定业务日志, EnumLevel.Warning);
                    
                    return t;

                }
                catch (Exception ex)
                {
                    string str = "";
                    str += "【1】" + iKeyState;
                    str += "【2】" + cOperateMode;
                    str += "【3】" + cTESAMNO;
                    str += "【4】" + cRandHost;
                    str += "【5】" + cReadData;
                    LogManager.AddMessage("\r\n" + str + "\r\n" + ex.Message + "\r\n" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.Error);
                    return -1;
                }
            }
        }

    }
}
