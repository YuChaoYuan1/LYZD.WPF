//using System.Runtime.Serialization;

//namespace LYZD.ViewModel
//{
//    [DataContract]
//    public class MeterInfo
//    {
//        #region 一块表基本信息
//        /// <summary>
//        /// 表位号		在表架上所挂位置
//        /// </summary>
//        [DataMember]
//        public int Mb_intBno { get; set; }

//        /// <summary>
//        /// 条形码	
//        /// </summary>
//        [DataMember]
//        public string Mb_ChrTxm { get; set; }
//        /// <summary>
//        /// 表通信地址
//        /// </summary>
//        [DataMember]
//        public string Mb_chrAddr { get; set; }
//        /// <summary>
//        /// 表号
//        /// </summary>
//        [DataMember]
//        public string _Mb_MeterNo { get; set; }
//        /// <summary>
//        /// 表型号
//        /// </summary>
//        [DataMember]
//        public string Mb_Bxh { get; set; }
//        /// <summary>
//        /// 表常数		有功（无功）
//        /// </summary>
//        [DataMember]
//        public string Mb_chrBcs { get; set; }
//        /// <summary>
//        /// 表类型
//        /// </summary>
//        [DataMember]
//        public string Mb_chrBlx { get; set; }
//        /// <summary>
//        /// 表等级		有功（无功）
//        /// </summary>
//        [DataMember]
//        public string Mb_chrBdj { get; set; }
//        /// <summary>
//        /// 共阴 共阳类型
//        /// </summary>
//        [DataMember]
//        public int Mb_gygy { get; set; }
//        /// <summary>
//        /// 测量方式	
//        /// </summary>
//        [DataMember]
//        public int Mb_intClfs { get; set; }
//        /// <summary>
//        /// 电压		XXX（不带单位）
//        /// </summary>
//        [DataMember]
//        public string Mb_chrUb { get; set; }
//        /// <summary>
//        /// 电流		Ib(Imax)（不带单位）
//        /// </summary>
//        [DataMember]
//        public string Mb_chrIb { get; set; }
//        /// <summary>
//        /// 频率		XX（不带单位）
//        /// </summary>
//        [DataMember]
//        public string Mb_chrHz { get; set; }
//        /// <summary>
//        /// 止逆器		1-有，0-无
//        /// </summary>
//        [DataMember]
//        public bool Mb_BlnZnq { get; set; }
//        /// <summary>
//        /// 互感器		1-经互感器
//        /// </summary>
//        [DataMember]
//        public bool Mb_BlnHgq { get; set; }
//        /// <summary>
//        /// 软件版本号
//        /// </summary>
//        [DataMember]
//        public string Mb_chrSoftVer { get; set; }
//        /// <summary>
//        /// 硬件版本号
//        /// </summary>
//        [DataMember]
//        public string Mb_chrHardVer { get; set; }
//        /// <summary>
//        /// 通讯协议名称
//        /// </summary>
//        [DataMember]
//        public string AVR_PROTOCOL_NAME { get; set; }
//        /// <summary>
//        /// 载波协议名称
//        /// </summary>
//        [DataMember]
//        public string AVR_CARR_PROTC_NAME { get; set; }
//        /// <summary>
//        /// 费控类型,1本地，0远程
//        /// </summary>
//        [DataMember]
//        public int Mb_intFKType { get; set; }
//        /// <summary>
//        /// 是否要检,用于检定
//        /// </summary>
//        [DataMember]
//        public bool YaoJianYn { get; set; }
//        /// <summary>
//        /// 使用的规程名称
//        /// </summary>
//        [DataMember]
//        public string GuiChengName { get; set; }
//        /// <summary>
//        /// 表MAC通信地址
//        /// </summary>
//        [DataMember]
//        public string AVR_OTHER_2 { get; set; }
//        /// <summary>
//        /// 备用3
//        /// </summary>
//        [DataMember]
//        public string AVR_OTHER_3 { get; set; }
//        /// <summary>
//        /// 备用4
//        /// </summary>
//        [DataMember]
//        public string AVR_OTHER_4 { get; set; }
//        #endregion
//    }
//}
