using System.Runtime.InteropServices;
using System.Text;

namespace LYZD.TerminalProtocol.Encryption
{
    /// <summary>
    ///  采集终端函数 调用加密机函数
    /// </summary>
    public class EncryptionFunction2013
    {
        // <summary>
        /// 登录服务器函数
        /// </summary>
        /// <param name="ip">服务器IP</param>
        /// <param name="port">服务器端口</param>
        /// <param name="time">字符型，单位秒，默认10</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
        public static extern int ConnectDevice(string PutIP, string PutPort, string PutCtime);

        /// <summary>
        /// 断开服务器连接函数
        /// </summary>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
        public static extern int CloseDevice();

        /// <summary>
        /// 获取随机数 
        /// </summary>
        /// <param name="OutR1">16字节随机数</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
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
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
        public static extern int Terminal_Formal_SessionInitRec(string PutState, string PutTESAMNo, string PutVersionNum, string PutSessionID, string PutR1, StringBuilder OutCertificate, StringBuilder OutEncR1, StringBuilder OutMac, StringBuilder OutSign1);

        /// <summary>
        /// 会话协商 
        /// </summary>
        /// <param name="PutState">芯片状态，  00‐‐第一套密钥，测试证书；11‐‐第二套密钥，正式证书使用） 。调用此函数时，对称密钥版是”00”，  对称密钥版本非全’0’时，须将证书状态切换到对应的状态） 。</param>
        /// <param name="PutTESAMNo">TESAM 序列号，8字节</param>
        /// <param name="PutVersionNum">1字节版本</param>
        /// <param name="PutSessionID">1字节会话ID</param>
        /// <param name="PutCRLCertificateNo">16字节主站证书序列号</param>
        ///
        /// <param name="PutTerminalCertificate">终端证书</param>
        /// <param name="PutEncR2">16字节R2（随机书2）密文</param>
        /// <param name="PutSign2">64字节签名</param>
        /// <param name="OutEncM1">会话密钥密文,113字节</param>
        /// <param name="OutSign3">主站证书验证码，97 字节</param>
        /// <param name="OutMac2">Mac2，4字节</param>
        /// <param name="OutSign4">签名，64 字节</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
        public static extern int Terminal_Formal_SessionKeyConsult(string PutState, string PutTESAMNo, string PutVersionNum, string PutSessionID, string PutCRLCertificateNo, string PutTerCertificateNo, string PutTerminalCertificate, string PutEncR2, string PutSign2, StringBuilder OutEncM1, StringBuilder OutSign3, StringBuilder OutMac2, StringBuilder OutSign4);

        /// <summary>
        /// 会话协商验证 
        /// </summary>
        /// <param name="PutR3">16字节随机数 3</param>
        /// <param name="PutMac3">4字节 MAC</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
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
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
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
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
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
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
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
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
        public static extern int Terminal_Formal_CertificateStateChange(string PutState, string PutTESAMNo, string PutCertificateState, string PutR6, StringBuilder OutEncR6, StringBuilder OutMac);

        /// <summary>
        ///  设置离线计数器
        /// </summary>
        /// <param name="PutState">对称密钥状态00‐‐第一套密钥，01—第二套密钥</param>
        /// <param name="PutTESAMNo">TESAM 序列号，8字节</param>
        /// <param name="PutCounter">离线计数器数值，4字节</param>
        /// <param name="OutEncCounter">密文数据，20字节</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
        public static extern int Terminal_Formal_SetOfflineCounter(string PutState, string PutTESAMNo, string PutCounter, StringBuilder OutEncCounter);

        /// <summary>
        ///  转加密授权
        /// </summary>
        /// <param name="OutData">32字节转加密授权数据</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
        public static extern int Terminal_Formal_ChangeDataAuthorize(StringBuilder OutData);

        /// <summary>
        ///  获取电表密钥密文
        /// </summary>
        /// <param name="PutMeterState">电表密钥状态，1字节，0‐‐公开密钥；01—交易密钥</param>
        /// <param name="PutMeterNo">“0000”+电表表号，共 8字节</param>
        /// <param name="PutTaskType">任务类型：0，身份认证任务；1，对时任务；2，红外认证； </param>
        /// <param name="OutMeterEncKey">对应任务和表号的密钥密文，32 字节，如果多表，调用多次； </param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
        public static extern int Terminal_Formal_GetCipherMeterKey(string PutMeterState, string PutMeterNo, int PutTaskType, StringBuilder OutMeterEncKey);

        /// <summary>
        ///  任务数据加密函数 
        /// </summary>
        /// <param name="PutInDataType">输入数据类型，对时任务当前为0；</param>
        /// <param name="PutTaskData">任务数据，字节数小于2k</param>
        /// <param name="OutTaskData">输出的任务密文</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
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
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
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
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
        public static extern int Terminal_Formal_SystemBroadcast(string PutState, string PutTESAMNo, string PutFnType, int PutOutDataType, string PutGroupAdrass, string PutMtime, string PutBroadcastData, StringBuilder OutMac);

        /// <summary>
        ///  对称密钥修改 
        /// </summary>
        /// <param name="PutState">00‐‐修改到第一套密钥；01‐‐修改到第二套密钥；</param>
        /// <param name="PutTESAMNo">TESAM 序列号，8字节；</param>
        /// <param name="OutKeyNum">当前更新的密钥总条数，16进制字符串；</param>
        /// <param name="OutEncKeyData">密钥密文，长度为32* OutKeyNum；</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
        public static extern int Terminal_Formal_SymmetricKeyUpdate(string PutState, string PutTESAMNo, StringBuilder OutKeyNum, StringBuilder OutEncKeyData);

        /// <summary>
        ///  证书更新
        /// </summary>
        /// <param name="PutCertificateState">  00‐‐修改测试证书；01‐‐修改正式证书到交易状态； 11‐‐恢复正式证书到初始状态；</param>
        /// <param name="PutCertificateType">证书类型，1字节；  01‐‐CRL 证书，其他保留；</param>
        /// <param name="OutEncCertificateData">证书密文，长度小于2k；</param>
        /// <returns></returns>
        [DllImport(@"EncrypGW\DLL_SERVER_2013\WinSocketServer.dll")]
        public static extern int Terminal_Formal_CACertificateUpdate(string PutCertificateState, string PutCertificateType, StringBuilder OutEncCertificateData);
    }
}
