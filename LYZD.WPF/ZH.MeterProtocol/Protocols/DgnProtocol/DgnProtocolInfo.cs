using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using ZH.MeterProtocol.Helper;

namespace ZH.MeterProtocol.Protocols.DgnProtocol
{
    [Serializable()]
    /// <summary>
    /// 多功能通信协议配置模型
    /// </summary>
    public class DgnProtocolInfo
    {

        #region 构造函数
        /// <summary>
        /// 多功能通信协议配置模型
        /// </summary>
        public DgnProtocolInfo() : this("")
        { }

        /// <summary>
        /// 构造函数，根据协议名称获取通信协议
        /// </summary>
        /// <param name="ProtocolName">协议名称</param>
        public DgnProtocolInfo(string protocolName)
        {
            #region 协议模型结构部分
            ProtocolName = protocolName;
            DllFile = "";
            ClassName = "";
            Setting = "";
            UserID = "";
            VerifyPasswordType = 0;
            WritePassword = "";
            WriteClass = "";
            ClearDemandPassword = "";
            ClearDemandClass = "";
            ClearDLPassword = "";
            ClearDLClass = "";
            TariffOrderType = "1234";
            DateTimeFormat = "";
            SundayIndex = 0;
            FECount = 0;
            ClockPL = 1;
            DataFieldPassword = false;
            BlockAddAA = false;
            ConfigFile = "";
            DgnPras = new Dictionary<string, string>();
            HaveProgrammingkey = false;
            //Loading = false;
            #endregion

            Load(ProtocolName);
        }

        #endregion

        #region---------------------------------------------协议模型结构部分-----------------------------------------


        /// <summary>
        /// 协议名称
        /// </summary>
        public string ProtocolName = "";

        /// <summary>
        /// 协议库名称
        /// </summary>
        public string DllFile = "";

        /// <summary>
        /// 协议类
        /// </summary>
        public string ClassName = "";

        /// <summary>
        /// 通信参数
        /// </summary>
        public string Setting = "";

        /// <summary>
        /// 用户代码
        /// </summary>
        public string UserID = "";

        /// <summary>
        /// 验证密码类型
        /// </summary>
        public int VerifyPasswordType = 0;
        #region 密码
        /// <summary>
        /// 一类写操作密码
        /// </summary>
        public string WritePassword = "";

        /// <summary>
        /// 一类写操作密码等级
        /// </summary>
        public string WriteClass = "";

        /// <summary>
        /// 二类写操作密码/写密码
        /// </summary>
        public string WritePassword2 = "";

        /// <summary>
        /// 二类写操作密码等级/写等级
        /// </summary>
        public string WriteClass2 = "";

        private string clearDemandPassword = "";

        /// <summary>
        /// 清需量密码
        /// </summary>
        public string ClearDemandPassword
        {
            get { return this.clearDemandPassword; }
            set
            {
                this.clearDemandPassword = value;
            }

        }

        /// <summary>
        /// 清需量密码等级
        /// </summary>
        public string ClearDemandClass = "";

        /// <summary>
        /// 清电量密码
        /// </summary>
        public string ClearDLPassword = "";

        /// <summary>
        /// 清电量密码等级
        /// </summary>
        public string ClearDLClass = "";

        /// <summary>
        /// 清事件密码
        /// </summary>
        public string ClearEventPassword = "";

        /// <summary>
        /// 清事件等级
        /// </summary>
        public string ClearEventClass = "";

        /// <summary>
        /// 拉合闸密码
        /// </summary>
        public string RelayPassword = "";

        /// <summary>
        /// 拉合闸等级
        /// </summary>
        public string RelayClass = "";
        #endregion
        /// <summary>
        /// 费率排序（峰平谷尖2341）
        /// </summary>
        public string TariffOrderType = "2341";
        /// <summary>
        /// 日期时间格式
        /// </summary>
        public string DateTimeFormat = "";
        /// <summary>
        /// 星期天序号
        /// </summary>
        public int SundayIndex = 0;
        /// <summary>
        /// 下发帧的唤醒符个数
        /// </summary>
        public int FECount = 0;

        /// <summary>
        /// 时钟频率
        /// </summary>
        public float ClockPL = 1;

        /// <summary>
        /// 数据域是否包含密码
        /// </summary>
        public bool DataFieldPassword = false;

        /// <summary>
        /// 写块操作是否加AA
        /// </summary>
        public bool BlockAddAA = false;
        /// <summary>
        /// 配置文件
        /// </summary>
        public string ConfigFile = "";

        /// <summary>
        /// 协议参数列表，KEY值为协议测试项目ID，并非多功能试验项目ID
        /// </summary>
        public Dictionary<string, string> DgnPras;

        /// <summary>
        /// 区别有无编程键，false：无，true：有
        /// </summary>
        public bool HaveProgrammingkey = false;

        private bool _Loading = false;
        /// <summary>
        /// 标志检查（只读），如果loading为假表示加载协议失败！
        /// </summary>
        public bool Loading
        {
            get
            {
                return _Loading;
            }
        }

        #endregion

        #region ---------------------------------下面部分为协议文件操作配置部分-------------------------------------- 




        /// <summary>
        /// 电能表制造厂家
        /// </summary>
        public string Factory { get; set; }

        /// <summary>
        /// 电能表型号
        /// </summary>
        public string ModelNo { get; set; }

        /// <summary>
        /// 根据协议名称加载协议信息
        /// </summary>
        /// <param name="ProtocolName"></param>
        public void Load(string ProtocolName)
        {
            LoadXmlData(ProtocolName, "", "");
        }
        /// <summary>
        /// 加载协议信息，调用该函数的前提是要么协议名称有值，要么制造厂家和表型号有值
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            if (this.ProtocolName == "" || (this.Factory == "" && this.ModelNo == ""))
            {
                return false;
            }

            this.LoadXmlData(this.ProtocolName, this.Factory, this.ModelNo);

            return true;
        }

        /// <summary>
        /// 加载XML文档
        /// </summary>
        /// <param name="protocolname"></param>
        /// <param name="factroy"></param>
        /// <param name="size"></param>
        private void LoadXmlData2(string protocolname, string factory, string size)
        {
            if (protocolname == "" && (factory == "" || size == "")) return;

       //"\\Const\\DgnProtocol.xml";
           XmlControl xml = new XmlControl(Application.StartupPath + "DgnProtocol.xml");

            if (xml.Count() == 0) return;

            XmlNode findNode = null;

            if (protocolname != "")
                findNode = XmlControl.FindSencetion(xml.RootNode(), XmlControl.XPath(string.Format("R,Name,{0}", protocolname)));
            else if (factory != "" && size != "")
                findNode = XmlControl.FindSencetion(xml.RootNode(), XmlControl.XPath(string.Format("R,ZZCJ,{0},BXH,{1}", factory, size)));
            if (findNode == null) return;

            #region----------------------------加载协议文件信息----------------------------------------------------------------------

            ProtocolName = findNode.Attributes["Name"].Value;          //协议名称 

            Factory = XmlControl.GetNodeAttributeValue(findNode, "ZZCJ");                              //制造厂家
            ModelNo = XmlControl.GetNodeAttributeValue(findNode, "BXH");                               //表型号
            DllFile = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,DllFile"));              //协议库名称
            ClassName = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,ClassName"));          //说使用协议类名称
            Setting = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,Setting"));              //通信参数
            UserID = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,UserID"));                //用户名
            VerifyPasswordType = int.Parse(XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,VerifyPasswordType"))); //验证类型
            WritePassword = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,WritePassword"));             //写密码
            WriteClass = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,WriteClass"));                    //写密码等级
            ClearDemandPassword = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,ClearDemandPassword"));  //清需量密码
            ClearDemandClass = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,ClearDemandClass"));        //请需量密码等级    
            ClearDLPassword = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,ClearDLPassword"));          //清电量密码
            ClearDLClass = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,ClearDLClass"));                //清电量等级    
            TariffOrderType = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,TariffOrderType"));          //费率类型
            DateTimeFormat = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,DateTimeFormat"));            //日期格式
            SundayIndex = int.Parse(XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,SundayIndex")));        //星期天表示
            ClockPL = float.Parse(XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,ClockPL")) == "" ? "1" : XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,ClockPL")));        //时钟频率
            FECount = int.Parse(XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,FECount")));               //唤醒FE个数
            DataFieldPassword = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,DataFieldPassword")) == "0" ? false : true;   //数据域是否包含密码
            BlockAddAA = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,BlockAddAA")) == "0" ? false : true;    //写数据块是否加AA    
            ConfigFile = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,ConfigFile"));                          //配置文件    
            HaveProgrammingkey = XmlControl.GetValue(findNode, XmlControl.XPath("C,Name,HaveProgrammingkey")) == "0" ? false : true; //有无编程键

            findNode = XmlControl.FindSencetion(findNode, XmlControl.XPath("Prjs")); //转到项目数据节点


            //Loading = true; //改写加载标志，表示加载协议成功

            DgnPras = new Dictionary<string, string>();

            if (findNode == null) return;

            for (int i = 0; i < findNode.ChildNodes.Count; i++)
            {
                DgnPras.Add(findNode.ChildNodes[i].Attributes["ID"].Value, findNode.ChildNodes[i].ChildNodes[0].Value);        //加入ID，值
            }

            if (DgnPras.Count == 0) return;



            #endregion

        }




        public static XmlNode NodeProtocols = null;
        /// <summary>
        /// 加载XML文档
        /// </summary>
        /// <param name="protocolname"></param>
        /// <param name="factroy"></param>
        /// <param name="size"></param>
        private void LoadXmlData(string protocolname, string factory, string size)
        {

            if (protocolname == "" && (factory == "" || size == ""))
                return;

           // protocolname = "DLT645-2007-Default";
            XmlControl _XmlNode = new XmlControl(string.Format(System.Windows.Forms.Application.StartupPath + "\\Xml\\AgreementConfig.xml"));
            if (_XmlNode == null || _XmlNode.Count() == 0) return;

            System.Xml.XmlNode _FindXmlNode = null;

            if (protocolname != "")
                _FindXmlNode = XmlControl.FindSencetion(_XmlNode.RootNode() , XmlControl.XPath(string.Format("R,Name,{0}", protocolname)));

            //_FindXmlNode = XmlControl.FindSencetion(_XmlNode.RootNode(), "//R[@Name='DLT645-2007-Default']");

            if (_FindXmlNode == null) return;

            #region----------------------------加载协议文件信息----------------------------------------------------------------------

            this.ProtocolName = protocolname;         //协议名称 

            this.Factory = "";

            this.ModelNo = "";
            ProtocolName = XmlControl.GetValue(_FindXmlNode, "ClassName");  //协议库名称
            this.DllFile = XmlControl.GetValue(_FindXmlNode, "ClassName");  //协议库名称
            this.ClassName = XmlControl.GetValue(_FindXmlNode, "ClassName");  //说使用协议类名称
            this.Setting = XmlControl.GetValue(_FindXmlNode, "Setting"); //通信参数
            this.UserID = XmlControl.GetValue(_FindXmlNode, "UserID");  //用户名
            this.VerifyPasswordType = int.Parse(XmlControl.GetValue(_FindXmlNode, "VerifyPasswordType"));//验证类型

            XmlNode nodeFeiLv = _FindXmlNode.SelectSingleNode("FeiLvId");
            this.TariffOrderType = nodeFeiLv.Attributes["Jian"].Value + nodeFeiLv.Attributes["Feng"].Value + nodeFeiLv.Attributes["Ping"].Value + nodeFeiLv.Attributes["Gu"].Value; //费率类型
            this.DateTimeFormat = XmlControl.GetValue(_FindXmlNode, "DateTimeFormat"); //日期格式
            this.SundayIndex = int.Parse(XmlControl.GetValue(_FindXmlNode, "SundayIndex")); //星期天表示
            this.ClockPL = 1F; //时钟频率
            this.FECount = int.Parse(XmlControl.GetValue(_FindXmlNode, "FECount"));     //唤醒FE个数
            this.DataFieldPassword = bool.Parse(XmlControl.GetValue(_FindXmlNode, "DataFieldPassword"));  //数据域是否包含密码
            this.BlockAddAA = bool.Parse(XmlControl.GetValue(_FindXmlNode, "BlockAddAA")); //写数据块是否加AA    
            this.ConfigFile = ""; //配置文件    
            this.HaveProgrammingkey = bool.Parse(XmlControl.GetValue(_FindXmlNode, "HaveProgrammingkey"));//有无编程键
            //有编程键
            XmlNodeList nodeWithKey;
            if (HaveProgrammingkey)
            {
                nodeWithKey = _FindXmlNode.SelectNodes("OperationsHaveKey/Operation");

                this.WritePassword = nodeWithKey[0].Attributes["Password"].Value;
                this.WriteClass = nodeWithKey[0].Attributes["Class"].Value;
                this.ClearDemandPassword = nodeWithKey[1].Attributes["Password"].Value;
                this.ClearDemandClass = nodeWithKey[1].Attributes["Class"].Value;
                this.ClearDLPassword = nodeWithKey[2].Attributes["Password"].Value;
                this.ClearDLClass = nodeWithKey[2].Attributes["Class"].Value;
                ClearEventPassword = nodeWithKey[3].Attributes["Password"].Value;
                ClearDLClass = nodeWithKey[3].Attributes["Class"].Value;
                RelayPassword = nodeWithKey[4].Attributes["Password"].Value;
                RelayClass = nodeWithKey[4].Attributes["Class"].Value;
            }
            else
            {
                //无编程键
                nodeWithKey = _FindXmlNode.SelectNodes("OperationsNoKey/Operation");

                WritePassword = nodeWithKey[0].Attributes["Password"].Value;
                WriteClass = nodeWithKey[0].Attributes["Class"].Value;
                WritePassword2 = nodeWithKey[1].Attributes["Password"].Value;
                WriteClass2 = nodeWithKey[1].Attributes["Class"].Value;
                ClearDemandPassword = nodeWithKey[3].Attributes["Password"].Value;
                ClearDemandClass = nodeWithKey[3].Attributes["Class"].Value;
                ClearDLPassword = nodeWithKey[2].Attributes["Password"].Value;
                ClearDLClass = nodeWithKey[2].Attributes["Class"].Value;
                ClearEventPassword = nodeWithKey[4].Attributes["Password"].Value;
                ClearEventClass = nodeWithKey[4].Attributes["Class"].Value;
                RelayPassword = nodeWithKey[5].Attributes["Password"].Value;
                RelayClass = nodeWithKey[5].Attributes["Class"].Value;
            }


            _FindXmlNode = XmlControl.FindSencetion(_FindXmlNode, XmlControl.XPath("Prjs"));          //转到项目数据节点

            this._Loading = true;                //改写加载标志，表示加载协议成功

            this.DgnPras = new Dictionary<string, string>();

            if (_FindXmlNode == null) return;

            for (int i = 0; i < _FindXmlNode.ChildNodes.Count; i++)
            {
                this.DgnPras.Add(_FindXmlNode.ChildNodes[i].Attributes["ID"].Value, _FindXmlNode.ChildNodes[i].ChildNodes[0].Value);        //加入ID，值
            }

            if (this.DgnPras.Count == 0) return;



            #endregion




        }
        #endregion
    }
}
