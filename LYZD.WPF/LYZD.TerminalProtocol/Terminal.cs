namespace LYZD.TerminalProtocol
{
    public class Terminal
    {
        /// <summary>
        /// 终端行政码A1
        /// </summary>
        public string str_Ter_Code = "0109";

        /// <summary>
        /// 终端地址A2
        /// </summary>
        public string str_Ter_Address = "";

        /// <summary>
        /// 主站地址和组地址标志A3
        /// </summary>
        public string str_MainStation = "02";

        /// <summary>
        /// 给终端回复确认时的主站地址和组地址标志A3
        /// </summary>
        public string str_ConMainStation = "00";

        /// <summary>
        /// 终端组地址
        /// </summary>
        public string str_Ter_GroupAddress = "1234";

        /// <summary>
        /// 密码
        /// </summary>
        public string str_Password = "0000";

        /// <summary>
        /// 身份认证时下发参数时的mac
        /// </summary>
        public string str_Mac = "";

        /// <summary>
        /// 密码长度
        /// </summary>
        public int int_Password = 16;

        /// <summary>
        /// 启动帧帧序号计数器PFC 
        /// </summary>
        public byte int_PFC = 0;

        /// <summary>
        /// 给终端回复确认的帧序号，从终端上行报文获取
        /// </summary>
        public byte int_ConPFC = 0;

        /// <summary>
        /// 给终端回复确认的控制域，+链路状态为11，其它确认为0
        /// </summary>
        public byte int_ConC = 0;

        /// <summary>
        /// 启动帧序号PSEQ
        /// </summary>
        public int int_PSEQ = 0;

        /// <summary>
        /// 接线方式，0：三相四线，1：三相三线，2：单相
        /// </summary>
        public byte byt_Jxfs = 0;

        /// <summary>
        /// 额定电压
        /// </summary>
        public int int_U = 220;

        /// <summary>
        /// 额定电流
        /// </summary>
        public float f_Ib = 1.5f;

        /// <summary>
        /// 最大电流
        /// </summary>
        public float f_Imax = 6;

        #region 698参数
        /// <summary>
        /// 数据区
        /// </summary>
        public string sAPDU = "";

        /// <summary>
        /// ESAM序列号
        /// </summary>
        public string cTESAMNO = "";

        /// <summary>
        /// 密钥版本
        /// </summary>
        public string KeyVerison = "";

        /// <summary>
        /// 对称密钥版本,0时为公钥，1为私钥
        /// </summary>
        public int iKeyState = 0;

        /// <summary>
        /// 单地址应用协商计数器
        /// </summary>
        public int cASCTR = 0;

        /// <summary>
        /// 主动上报计数器
        /// </summary>
        public int ReportCount = 0;

        /// <summary>
        /// 广播计数器
        /// </summary>
        public int BroadCastCount = 0;

        /// <summary>
        /// 主站证书
        /// </summary>
        public string cMasterCert = "";

        /// <summary>
        /// 终端证书
        /// </summary>
        public string cTerminalCert = "";

        /// <summary>
        /// 会话密钥，建立会话成功后密码机产生的会话密钥
        /// </summary>
        public string cOutSessionKey = "";

        /// <summary>
        /// 会话密钥，建立会话成功后系统存储的会话密钥
        /// </summary>
        public string cSessionKey = "";

        /// <summary>
        /// 操作模式
        /// </summary>
        public int iOperateMode = 0;

        /// <summary>
        /// 安全标识
        /// </summary>
        public string cOutSID = "";

        /// <summary>
        /// SID 的附加数据
        /// </summary>
        public string cOutAddData = "";

        /// <summary>
        /// 数据
        /// </summary>
        public string cOutData = "";

        /// <summary>
        /// 数据校验码
        /// </summary>
        public string cOutMAC = "";

        /// <summary>
        /// 主站随机数
        /// </summary>
        public string cOutRandHost = "";

        /// <summary>
        /// 随机数
        /// </summary>
        public string cOutRand = "";

        /// <summary>
        /// 会话协商数据
        /// </summary>
        public string cOutSessionInit = "";

        /// <summary>
        /// 协商数据签名(4Byte)，建立应用连接中的客户机签名1
        /// </summary>
        public string cOutSign = "";

        /// <summary>
        /// 终端返回的应用会话协商数据签名(64Byte)
        /// </summary>
        public string cSign = "";


        /// <summary>
        /// 终端返回的应用会话协商数据(48Byte)
        /// </summary>
        public string cSessionData = "";

        /// <summary>
        /// 
        /// </summary>
        public string cOutAttachData = "";

        /// <summary>
        /// 密钥更新数据
        /// </summary>
        public string cOutTrmKeyData = "";


        public string cTaskData = "";

        public string cOutTaskData = "";
        public string cOutTaskMAC = "";

        public string ucReadData = "";
        public string ucMac = "";

        public string ucOutAttachData = "";
        #endregion
    }
}
