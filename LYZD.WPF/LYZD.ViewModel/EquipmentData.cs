using LYZD.DAL.Config;
using LYZD.Utility.Log;
using LYZD.ViewModel.CheckController;
using LYZD.ViewModel.CheckInfo;
using LYZD.ViewModel.Device;
using LYZD.ViewModel.InputPara;
using LYZD.ViewModel.Monitor;
using LYZD.ViewModel.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using LYZD.Mis.Houda;
using System.Diagnostics;
using System.Threading;
using LYZD.Core.Struct;
using LYZD.Core.Enum;
using System.ServiceModel;
using LYZD.Mis.Common;
using LYZD.Core.Model.Meter;
using LYZD.Mis;
using LYZD.DAL;
using LYZD.Mis.NanRui.LRDataTable;
using LYZD.ViewModel.Const;
using System.Threading.Tasks;

namespace LYZD.ViewModel
{
    /// 检定数据中心
    /// <summary>
    /// 检定数据中心
    /// </summary>
    public class EquipmentData
    {

        /// <summary>
        /// 终端上线中
        /// </summary>
        public static bool IsEthernetOnlineNow = true;
        /// <summary>
        /// 终端在线IP端口，地址
        /// </summary>
        public static Dictionary<string, string> TerminalIndexEthernetAddress = new Dictionary<string, string>();
        /// <summary>
        /// 终端在线地址，序号
        /// </summary>
        public static Dictionary<string, int> TerminalIndexEthernet = new Dictionary<string, int>();

        /// <summary>
        /// 智慧工控平台控制指令
        /// </summary>
        /// <param name="type"></param>
        internal static void IMICPKz(string type)
        {
            switch (type)
            {
                case "01":
                    controller.StopVerify();
                    break;
                case "02":
                    controller.RunningVerify();
                    break;
                case "03":
                    controller.StopVerify();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 下载任务时获取到任务表里面全部字段信息
        /// </summary>
        public static MT_DETECT_OUT_EQUIP LoadDETECT_TASK = new MT_DETECT_OUT_EQUIP();
        /// <summary>
        /// 单步模式检定结束是否关源,true:关,false:不关。每次重启软件重置为关源
        /// </summary>
        public static bool SingleOffPower = true;
        public static bool IsVerifyModelAuto = false;
        /// <summary>
        /// 通电检测供电时间
        /// </summary>
        public static string PowerOnTime = "120";
        /// <summary>
        /// 指示当前批次表是否已经组网。参数录入和登录后此值为False,组网后为True
        /// </summary>
        public static bool IsHplcNet { get; set; }


        public static Version version;
        ///// <summary>
        ///// 载波方案配置集合
        ///// </summary>
        //public static  CarrierList ZaiBoInfo { get; set; }

        #region 营销系统部分
        //private static ServiceHost host = null;
        /// <summary>
        /// 通知消息事件
        /// </summary>
        public static event EventHandler<EventArgs> CallMsgEvent = null;

        public static void CallMsg(string cmd)
        {
            if (CallMsgEvent != null)
                CallMsgEvent(cmd, new EventArgs());
        }
        /// <summary>
        /// 应用程序是否已经退出.用于结束线程
        /// </summary>
        public static bool ApplicationIsOver { get; set; }

        /// <summary>
        /// 与厚达通讯进行Tcp通讯接口业务过程
        /// </summary>
        public static HoudaInteraction HouTcpCommunication = null;

        ////是否自MDS下载检定方案
        //public static bool IsMdsDownScheme = ConfigHelper.Instance.IsMdsDownScheme;
        ///// <summary>
        ///// 营销接口类型
        ///// </summary>
        //public static string Marketing_Type = ConfigHelper.Instance.Marketing_Type;
        #endregion


        private static MeterInputParaViewModel meterGroupInfo;
        /// 表信息数据集合
        /// <summary>
        /// 表信息数据集合
        /// </summary>
        public static MeterInputParaViewModel MeterGroupInfo
        {
            get
            {
                if (meterGroupInfo == null)
                {
                    meterGroupInfo = new MeterInputParaViewModel(true);
                }
                return meterGroupInfo;
            }
            set
            {
                meterGroupInfo = value;
            }
        }


        #region 方案


        private static SchemaOperationViewModel schemaModels;
        /// <summary>
        /// 方案列表
        /// </summary>
        public static SchemaOperationViewModel SchemaModels
        {
            get
            {
                if (schemaModels == null)
                {
                    schemaModels = new SchemaOperationViewModel();
                    SchemaModels.PropertyChanged += SchemaModels_PropertyChanged;
                }
                return schemaModels;
            }
        }
        /// 更改检定方案时的事件
        /// <summary>
        /// 更改检定方案时的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SchemaModels_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {


            if (e.PropertyName == "SelectedSchema")
            {
                if (SchemaModels.SelectedSchema == null)
                {
                    return;
                }
                int schemaId = (int)SchemaModels.SelectedSchema.GetProperty("ID");
                if (Schema == null)
                {
                    schema = new SchemaViewModel(schemaId);
                }
                else
                {
                    Schema.LoadSchema(schemaId);
                }
                Equipment.TextLogin = "软件登录中,请等待:初始化检定结论信息...";
                CheckResults.InitialResult();
            }
        }


        private static SchemaViewModel schema;
        /// 检定方案
        /// <summary>
        /// 检定方案
        /// </summary>
        public static SchemaViewModel Schema
        {
            get
            {
                if (schema == null)
                {
                    schema = new SchemaViewModel();
                }
                return schema;
            }
        }


        #endregion




        private static CheckResultViewModel checkResults;
        /// 检定结论
        /// <summary>
        /// 检定结论
        /// </summary>
        public static CheckResultViewModel CheckResults
        {
            get
            {
                if (checkResults == null)
                {
                    checkResults = new CheckResultViewModel();
                }
                return checkResults;
            }
        }


        private static LastCheckInfoViewModel lastCheckInfo;
        /// 检定软件退出时的软件信息
        /// <summary>
        /// 检定软件退出时的软件信息
        /// </summary>
        public static LastCheckInfoViewModel LastCheckInfo
        {
            get
            {
                if (lastCheckInfo == null)
                {
                    lastCheckInfo = new LastCheckInfoViewModel();
                }
                return lastCheckInfo;
            }
        }


        private static EquipmentViewModel equipment;
        /// 台体信息
        /// <summary>
        /// 台体信息
        /// </summary>
        public static EquipmentViewModel Equipment
        {
            get
            {
                if (equipment == null)
                {
                    equipment = new EquipmentViewModel();
                }
                return equipment;
            }
        }


        private static StdInfoViewModel stdInfo;
        /// <summary>
        /// 标准表信息
        /// </summary>
        public static StdInfoViewModel StdInfo
        {
            get
            {
                if (stdInfo == null)
                {
                    stdInfo = new StdInfoViewModel();
                }
                return stdInfo;
            }
        }



        /// 初始化检定数据
        /// <summary>
        /// 初始化检定数据
        /// </summary>
        public static void Initialize()
        {
            Equipment.TextLogin = "软件登录中,请等待:正在加载表信息...";
            MeterGroupInfo.Initialize();
            Equipment.TextLogin = "软件登录中,请等待:正在加载设备信息...";
            DeviceManager.LoadDevices();  // 【标注002】
            //加载检定初始数据
            LastCheckInfo.LoadLastCheckInfo();
            Equipment.TextLogin = "软件登录中,请等待:正在加载方案信息...";
            //加载检定方案
            SchemaModels.SelectedSchema = SchemaModels.Schemas.FirstOrDefault(item => (int)item.GetProperty("ID") == LastCheckInfo.SchemaId);
            Controller.Index = LastCheckInfo.CheckIndex;
            if (!Equipment.IsDemo && ConfigHelper.Instance.Marketing_Type == "厚达" && ConfigHelper.Instance.VerifyModel == "自动模式")
            {
                //初始化与厚达通讯 模块。
                string HouDaIP = ConfigHelper.Instance.SetControl_Ip;
                int HouDaPort = int.Parse(ConfigHelper.Instance.SetControl_Prot);
                string XianTiCode = EquipmentData.equipment.ID;
                int iHdPort = Convert.ToInt32(HouDaPort);
                HouTcpCommunication = new HoudaInteraction(HouDaIP, iHdPort, XianTiCode);
                HouTcpCommunication.UpdataStatusLabel += new EventHandler<EventArgs>(HouTcpCommunication_UpdataStatusLabel);
                HouTcpCommunication.RevcMessageEvent += new EventHandler<Mis.Houda.XmlMsg>(HouTcpCommunication_RevcMessageEvent);
            }
            

            CallMsgEvent += new EventHandler<EventArgs>(GlobalUnit_CallMsgEvent);
            Thread.Sleep(100);
            Utility.TaskManager.AddWcfAction(() =>
            {
                Controller.DeviceStart = -1;  //开始台体状态-1
                VerifyBase.meterInfo = meterGroupInfo.GetVerifyMeterInfo();  //载入表信息
                VerifyBase.TcpServerInin();
                VerifyBase.InitTerminalTalks();
                DeviceManager.InitializeDevice();

                //连接设备
                DeviceManager.Link();
                ////连接加密机
                DeviceManager.LinkDog();

                if (DeviceManager.IsConnected == true && DeviceManager.IsReady == true)
                {
                    LogManager.AddMessage("联机成功", EnumLogSource.设备操作日志, EnumLevel.Information);
                    VerifyBase.ListenUDP();
                }
                else
                {
                    IMICP.OpenPortIMICP open = new IMICP.OpenPortIMICP();
                    open.AlarmData("联机失败", "检查网络信息");
                    LogManager.AddMessage("联机失败", EnumLogSource.设备操作日志, EnumLevel.Warning);
                }

            });
            Controller.DeviceStart = -1;  //开始台体状态-1
            PowerOnTime = ConfigHelper.Instance.PowerOnTime;
            //打开虚拟表程序
            Process[] myProcess = Process.GetProcessesByName("LY.VirtualMeter");
            if (myProcess.Length == 0)
            {
                Process.Start("LY.VirtualMeter.exe");
            }
            if (!Equipment.IsDemo && ConfigHelper.Instance.Marketing_Type == "LY数据服务" && ConfigHelper.Instance.VerifyModel == "自动模式")
            {
                LoadLyService();
            }
            LoadDETECT_TASK.DETECT_TASK_NO = OperateFile.GetINI("LoadDETECT_TASK", "DETECT_TASK_NO", System.IO.Directory.GetCurrentDirectory() + "\\Ini\\LoadDETECT_TASK.ini");

            #region 智慧工控平台
            if (ConfigHelper.Instance.OperatingConditionsYesNo == "是" && ConfigHelper.Instance.Marketing_Type == "国金")
            {
                IMICPGK(true);
            }

            if (ConfigHelper.Instance.OperatingConditionsYesNo == "是" && ConfigHelper.Instance.Marketing_Type == "智慧工控平台")
            {
                IMICPGK(true);
            }
            #endregion
        }
        /// <summary>
        /// 山西工控
        /// </summary>
        /// <param name="v"></param>
        public static void IMICPGK(bool v)
        {
            ViewModel.IMICP.OpenPortIMICP open = new ViewModel.IMICP.OpenPortIMICP();

            if (v)
            {

                string ip = ConfigHelper.Instance.OperatingConditionsIp.Trim();
                string port = ConfigHelper.Instance.OperatingConditionsProt.Trim();
                string pl = ConfigHelper.Instance.OperatingConditionsUpdataF.Trim();
                open.openFWQ();
                //open.button13_Click();
                open.Updata(ip, port, pl);
            }
            else
            {
                open.EndApi();
            }
        }
        private static void LoadLyService()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                IMis mis = MISFactory.Create();

                LYZD.Mis.LYDataServer.Api api = mis as LYZD.Mis.LYDataServer.Api;

                IsVerifyModelAuto = true;
                string centerIp = ConfigHelper.Instance.SetControl_Ip;
                if (!int.TryParse(ConfigHelper.Instance.SetControl_Prot, out int centerPort))
                {
                    centerPort = 9999;
                }
                InnerCommand.VerifyControl.InitControl();
                if (mis == null)
                {
                    LogManager.AddMessage("接口配置异常", EnumLogSource.设备操作日志, EnumLevel.Error);
                    return;
                }
                CtrlCmd.CtrlClient.InitNet(centerIp, centerPort, ConfigHelper.Instance.EquipmentNo);
                CtrlCmd.CtrlClient.Connect();
                if (api.UploadVersion(ConfigHelper.Instance.EquipmentNo, version.ToString(), out string Msg))
                {
                    LogManager.AddMessage("上传软件版本号成功!", EnumLogSource.服务器日志, EnumLevel.Information);
                    if (api.GetServerTime(out string TimeMsg))
                    {
                        InnerCommand.VerifyControl.SendMsg(Msg);
                    }
                    else
                    {
                        InnerCommand.VerifyControl.SendMsg(CtrlCmd.MsgType.故障, TimeMsg);
                    }
                }
                else
                {
                    LogManager.AddMessage(Msg, EnumLogSource.服务器日志, EnumLevel.TipsError);
                }
                while (true)
                {
                    if (!CtrlCmd.CtrlClient.Connected || !CtrlCmd.CtrlClient.Registered)
                    {
                        CtrlCmd.CtrlClient.InitNet(centerIp, centerPort, ConfigHelper.Instance.EquipmentNo);
                        CtrlCmd.CtrlClient.Connect();
                        LogManager.AddMessage($"重联主控...{CtrlCmd.CtrlClient.Connected},{CtrlCmd.CtrlClient.Registered}", EnumLogSource.服务器日志, EnumLevel.Information);
                        if (!api.UploadVersion(ConfigHelper.Instance.EquipmentNo, version.ToString(), out string LinkMsg))
                        {
                            InnerCommand.VerifyControl.SendMsg(CtrlCmd.MsgType.故障, LinkMsg);
                        }
                        else
                        {
                            InnerCommand.VerifyControl.SendMsg(Msg);
                        }
                    }
                    System.Threading.Thread.Sleep(5000);
                }

            });

        }

        /// <summary>
        /// 连接设备
        /// </summary>
        private static void LinkDevice()
        {
            //初始化所有设备
            DeviceManager.InitializeDevice();
            //连接设备
            DeviceManager.Link();
            ////连接加密机
            DeviceManager.LinkDog();
        }

        /// 导航到当前的默认界面
        /// <summary>
        /// 导航到当前的默认界面
        /// </summary>
        public static void NavigateCurrentUi()
        {
            //【标注】先用着测试
            UiInterface.ChangeUi("运行日志", "View_Log");
            //UiInterface.ChangeUi("标准表信息", "View_MeterMessage");
            UiInterface.ChangeUi("标准表信息", "View_StdMessage");
            //UiInterface.ChangeUi("检定", "View_Test");
            //return;
            //根据检定点序号来加载不同检定界面
            if (LastCheckInfo.CheckIndex == -1)     //参数录入 
            {
                UiInterface.ChangeUi("参数录入", "View_Input");
            }
            else if (LastCheckInfo.CheckIndex == -3)        //审核存盘
            {
            }
            else                    //检定界面
            {
                string errorString = "";
                if (MeterGroupInfo.CheckInfoCompleted(out errorString))
                {
                    UiInterface.ChangeUi("检定", "View_Test");
                    //UiInterface.ChangeUi("详细数据", "View_TestDetail");


                    //UiInterface.ChangeUi("运行日志", "View_Log");
                    ////UiInterface.ChangeUi("标准表信息", "View_MeterMessage");
                    //UiInterface.ChangeUi("标准表信息", "View_StdMessage");


                }
                else
                {
                    UiInterface.ChangeUi("参数录入", "View_Input");
                    LogManager.AddMessage(errorString, EnumLogSource.用户操作日志, EnumLevel.Warning);
                }
            }
        }


        private static ControllerViewModel controller;

        public static ControllerViewModel Controller
        {
            get
            {
                if (controller == null)
                {
                    controller = new ControllerViewModel();
                }
                return controller;
            }
        }

        private static DeviceViewModel deviceManager;
        public static DeviceViewModel DeviceManager
        {
            get
            {
                if (deviceManager == null)
                {
                    deviceManager = new DeviceViewModel();
                }
                return deviceManager;
            }
        }



        #region Socket
        //处理连接指示工作
        static void HouTcpCommunication_UpdataStatusLabel(object sender, EventArgs e)
        {
            string strMsg = sender as string;
            switch (strMsg)
            {
                case "connect":
                    {
                        //m_Frame.NetState = Cus_NetState.DisConnected;
                        LogManager.AddMessage("开始连接服务器...");
                    }
                    break;
                case "connectSuccess":
                    {
                        //m_Frame.NetState = Cus_NetState.Connected;
                        LogManager.AddMessage("连接服务器成功");
                        //m_Frame.NetState = Cus_NetState.Connected;
                    }
                    break;
                case "disconNexion":
                    {
                        //m_Frame.NetState = Cus_NetState.DisConnected;
                        //断开连接
                        LogManager.AddMessage("断开后重连服务器");
                    }
                    break;
                default:
                    break;
            }

        }


        /// <summary>
        /// 处理返回的服务端接口数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="serverMsg"></param>
        static void HouTcpCommunication_RevcMessageEvent(object sender, Mis.Houda.XmlMsg serverMsg)
        {
            //添加服务器指示 数据操作
            switch (serverMsg.MessageAttribute)
            {


                case Mis.Houda.NetworkMessageAttribute.应答指令9999:
                    {
                        //不做处理
                    }
                    break;
                case Mis.Houda.NetworkMessageAttribute.测试通知1004:
                    {
                        Mis.Houda.XmlMsg xmlmsg = new Mis.Houda.XmlMsg();
                        xmlmsg.headMsg.ToRecive = "Main";
                        xmlmsg.headMsg.CmdType = "1";
                        xmlmsg.headMsg.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        xmlmsg.headMsg.Command = "9999";
                        xmlmsg.headMsg.Seq = serverMsg.headMsg.Seq;
                        HouTcpCommunication.SendNetworkMessage(xmlmsg);
                        MeterGroupInfo.NewMeters2();   //换新表
                        Thread.Sleep(1000);
                        MeterGroupInfo.Frame_DownMeterInfoFromMis();
                        Thread.Sleep(500);
                        MeterGroupInfo.Frame_DownSchemeMis();   //下载方案
                        Thread.Sleep(500);
                        MeterGroupInfo.UpdateMeterInfo2(); //更新电表数据，跳转到检定界面
                        Thread.Sleep(200);
                        //开始执行检定方案
                        controller.Index = 0; //设置从第一项开始检定
                        controller.RunningVerify();
                    }
                    break;
                case Mis.Houda.NetworkMessageAttribute.台体控制指令1006:
                    {
                        Mis.Houda.XmlMsg xmlmsg = new Mis.Houda.XmlMsg();
                        xmlmsg.headMsg.ToRecive = "Main";
                        xmlmsg.headMsg.CmdType = "1";
                        xmlmsg.headMsg.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        xmlmsg.headMsg.Command = "9999";
                        xmlmsg.headMsg.Seq = serverMsg.headMsg.Seq;

                        HouTcpCommunication.SendNetworkMessage(xmlmsg);

                        #region 服务端发送控制指令
                        switch (serverMsg.OP)
                        {
                            //操作类型： 0-不执行； 1-暂停检定； 2-停止检定；3-继续检定； 9-关闭计算机
                            case "0":
                                {
                                    //    Frame_StopAdpater();
                                    controller.StopVerify();
                                }
                                break;
                            case "1":
                                {
                                    //Frame_StopAdpater();
                                    controller.StopVerify();
                                }
                                break;
                            case "2":
                                {
                                    //Frame_StopAdpater();
                                    controller.StopVerify();

                                }
                                break;
                            case "3":
                                {
                                    //ActiveItemID
                                    //App.CUS.CheckState = Cus_CheckStaute.检定;
                                    //IsChecking = true;
                                    //Frame_StartAdpater(App.CUS.ActiveItemID);
                                    controller.RunningVerify();
                                }
                                break;
                            case "9":
                                {
                                    //Controller.DeviceStart = 9;
                                    //关闭计算机
                                    Process process1 = new Process();
                                    process1.StartInfo.FileName = "shutdown";
                                    process1.StartInfo.Arguments = "-s -t 5";
                                    process1.Start();
                                }
                                break;
                            case "11":
                                {
                                    //TODO2检定完成后开始自动保存数据
                                    // m_Frame.StartSaveData();
                                }
                                break;
                            default:
                                break;
                        }
                        #endregion
                    }
                    break;

                default:
                    break;
            }

        }
        /// <summary>
        /// 全局通知消息事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void GlobalUnit_CallMsgEvent(object sender, EventArgs e)
        {

            //通知检定事件已经完成
            string strMsg = sender as string;
            switch (strMsg)
            {
                case "VerifyCompelate":
                    {
                        //检定完成后开始自动保存数据 延时等待检定线程结束
                        LogManager.AddMessage("检定完成开始保存数据!");
                        Thread.Sleep(500);
                        ////
                        //m_Frame.StartSaveData();
                        if (ConfigHelper.Instance.Marketing_Type == "厚达" && ConfigHelper.Instance.VerifyModel == "自动模式")
                        {
                            if (ConfigHelper.Instance.Marketing_UpData)//判断需要时时上传数据
                            {
                                SetMeterTime();
                                UpMeterData(VerifyBase.meterInfo);
                            }
                        }
                        if (ConfigHelper.Instance.Marketing_Type == "LY数据服务" && ConfigHelper.Instance.VerifyModel == "自动模式")
                        {
                            SetMeterTime();
                            UpMeterData2(VerifyBase.meterInfo);
                        }
                    }
                    break;
                case "CompelateOneBatch":  //检定完成
                    {
                        if (ConfigHelper.Instance.Marketing_Type == "厚达" && ConfigHelper.Instance.VerifyModel == "自动模式")
                        {
                            //检定完成
                            Mis.Houda.XmlMsg xmlmsg = new Mis.Houda.XmlMsg
                            {
                                Err = "0",
                                Des = ""
                            };
                            //xmlmsg.headMsg.FromStart = EquipmentData.equipment.ID;
                            xmlmsg.headMsg.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            xmlmsg.headMsg.ToRecive = "Main";
                            xmlmsg.headMsg.CmdType = "1";
                            //xmlmsg.headMsg.CurrentTime = string.Empty;
                            xmlmsg.headMsg.Command = "2005";
                            //Controller.DeviceStart = 4;
                            LogManager.AddMessage("发送检定完成消息给服务器!");
                            if (HouTcpCommunication != null)
                                HouTcpCommunication.SendNetworkMessage(xmlmsg);
                            RestareWindow();
                        }
                    }
                    break;
                case "DeviceState":     //台体状态
                    if (ConfigHelper.Instance.Marketing_Type == "厚达" && ConfigHelper.Instance.VerifyModel == "自动模式")
                    {
                        //检定完成
                        Mis.Houda.XmlMsg xmlmsg = new Mis.Houda.XmlMsg
                        {
                            Err = "0",
                            Des = ""
                        };
                        xmlmsg.headMsg.CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        xmlmsg.headMsg.ToRecive = "Main";
                        xmlmsg.headMsg.CmdType = "1";
                        xmlmsg.headMsg.Command = "2008";
                        xmlmsg.DeviceStart = Controller.DeviceStart.ToString();
                        // < BODY >
                        //< DESKID > 台体编号 </ DESKID >
                        //   < STATUS > 台体状态 </ STATUS >
                        //</ BODY >

                        //台体状态 ： 0-空闲； 1-检测中； 2-暂停；3-停止；4-完成； 5-关机中；8-不合格率报警 9-异常
                        LogManager.AddMessage("发送台体状态消息给服务器!");
                        if (HouTcpCommunication != null)
                            HouTcpCommunication.SendNetworkMessage(xmlmsg);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 重启电脑
        /// </summary>
        public static void RestareWindow()
        {
            Thread.Sleep(1000);
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                p.StandardInput.WriteLine("shutdown -r -t 1");
                p.StandardInput.WriteLine("exit");
            }
            catch (Exception)
            {

                throw;
            }
        }



        /// <summary>
        /// 上传数据到中间库，先从本地数据库读取表数据，然后转换上传
        /// </summary>
        /// <param name="meterInfos"></param>
        public static void UpMeterData(TestMeterInfo[] meterInfos)
        {

            #region 上传到中间库
            bool blnTotalUpRst = true;
            int iUpdateOkSum = 0;

            IMis mis = MISFactory.Create();
            mis.UpdateInit();


            // TODO2新版本数据上传，速度比原版本提高1000倍以上，还需要测试才能投入使用
            TestMeterInfo[] testMeters = MeterResoultModel.MeterDataHelper.GetDnbInfoNew();
            for (int i = 0; i < testMeters.Length; i++)
            {
                TestMeterInfo temmeter = testMeters[i];
                if (temmeter.Other1 != "1") continue;//不需要上传的话就跳过
                bool bUpdateOk = mis.Update(temmeter);
                if (!bUpdateOk)
                {
                    blnTotalUpRst = false;
                    LogManager.AddMessage(string.Format("上传到生产调度中间库失败，条形码{0}", temmeter.MD_BarCode), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                    continue;
                }
                else
                {
                    iUpdateOkSum++;
                }
                LogManager.AddMessage(string.Format("电能表[{0}]检定记录上传成功!", temmeter.MD_BarCode), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                //int updateCount = DALManager.MeterDbDal.Update("METER_INFO", "METER_ID", models, fieldNames);
                SetMeterData(temmeter, i);
            }



            #region 老版本从数据库中取数据，效率太低

            //foreach (TestMeterInfo meter in meterInfos)
            //{
            //    if (!meter.YaoJianYn && ((meter.Meter_ID == null || meter.Meter_ID.Trim() == "") || (meter.MD_BarCode == null || meter.MD_BarCode.Trim() == ""))) continue;
            //    try
            //    {
            //        LogManager.AddMessage(string.Format($"开始上传表ID【{meter.Meter_ID}】"), EnumLogSource.数据库存取日志, EnumLevel.Information);
            //    }
            //    catch (Exception)
            //    {
            //    }
            //    TestMeterInfo temmeter;
            //    try
            //    {
            //        //temmeter = Mis.DataHelper.DataManage.GetDnbInfoNew(meter, false);
            //    }
            //    catch (Exception ex)
            //    {
            //        LogManager.AddMessage(string.Format("上传到生产调度中间库失败1，条形码{0}" + ex, meter.MD_BarCode), EnumLogSource.数据库存取日志, EnumLevel.Error);
            //        continue;
            //    }
            //    LogManager.AddMessage("电表数据获取成功,开始上传", Utility.Log.EnumLogSource.检定业务日志, Utility.Log.EnumLevel.Information);
            //    bool bUpdateOk = false;
            //    try
            //    {
            //        //bUpdateOk = mis.Update(temmeter);
            //    }
            //    catch (Exception ex)
            //    {
            //        LogManager.AddMessage(string.Format("上传到生产调度中间库失败2，条形码{0}" + ex, meter.MD_BarCode), EnumLogSource.数据库存取日志, EnumLevel.Error);
            //        continue;
            //    }

            //    if (!bUpdateOk)
            //    {
            //        blnTotalUpRst = false;
            //        //LogManager.AddMessage(string.Format("上传到生产调度中间库失败3，条形码{0}", temmeter.MD_BarCode), EnumLogSource.数据库存取日志, EnumLevel.Error);
            //        continue;
            //    }
            //    else
            //    {
            //        iUpdateOkSum++;
            //    }
            //    //LogManager.AddMessage(string.Format("电能表[{0}]检定记录上传成功!", temmeter.MD_BarCode), EnumLogSource.数据库存取日志, EnumLevel.Warning);
            //}   
            #endregion

            LogManager.AddMessage("电表信息全部上传完成,开始抬起电机...");

            //mis.UpdateCompleted();
            #endregion

            if (blnTotalUpRst)        //TODO2这里写抬起电机，然后发送检定完成
            {
                DeviceManager.ElectricmachineryContrnl(00, 0xff);  //取出表

                int time = 15;
                while (time > 0)
                {
                    Controller.MessageTips = $"松开压接电机等待{time}秒";
                    Thread.Sleep(1000);
                    time--;
                }
                Controller.MessageTips = $"开始读取表位状态";
                MeterState[] meterState = new MeterState[Equipment.MeterCount];
                for (int i = 0; i < Equipment.MeterCount; i++)   //读取表位状态
                {
                    meterState[i] = new MeterState();
                    byte[] OutResult;
                    bool t = DeviceManager.Read_Fault(03, (byte)(i + 1), out OutResult);  //读取表位的状态
                    if (!t)   //读取失败在读取一次
                    {
                        t = DeviceManager.Read_Fault(03, (byte)(i + 1), out OutResult);
                    }

                    if (t)  //读取成功
                    {
                        meterState[i].U = (MeterState_U)OutResult[0];
                        meterState[i].I = (MeterState_I)OutResult[1];
                        meterState[i].Motor = (MeterState_Motor)OutResult[2];
                        meterState[i].YesOrNo = (MeterState_YesOrNo)OutResult[3];
                        meterState[i].CT = (MeterState_CT)OutResult[4];
                        meterState[i].Trip = (MeterState_Trip)OutResult[5];
                        meterState[i].TemperatureI = (MeterState_TemperatureI)OutResult[6];
                    }
                    else
                    {
                        meterState[i].Motor = MeterState_Motor.电机不确定;
                    }
                }
                bool[] rpMeter = GetRepeatPressMeters(meterState);   //这里在发送一次抬起电机命令
                //EquipmentData.DeviceManager.ElectricmachineryContrnl(1, (byte)(i + 1));//第一次失败在发一次


                if (Array.IndexOf(rpMeter, true) >= 0)
                {
                    LogManager.AddMessage(string.Format("系统检测到表位未完全松表"), EnumLogSource.设备操作日志, EnumLevel.Warning);

                    Controller.DeviceStart = 9; //状态异常
                }
                else
                {
                    Controller.MessageTips = $"表位状态正常,检定完成";
                    //通知一批表检定完成
                    CallMsg("CompelateOneBatch");

                }
            }

        }

        public static void UpMeterData2(TestMeterInfo[] meterInfos)
        {
            bool blnTotalUpRst = false;
            IMis mis = MISFactory.Create();
            mis.UpdateInit();
            TestMeterInfo[] testMeters = MeterResoultModel.MeterDataHelper.GetDnbInfoNew();
            LogManager.AddMessage("电表信息全部上传完成");
            if (mis is Mis.LYDataServer.Api api)
            {
                blnTotalUpRst = api.Update(testMeters.ToList(), 10000, out string msg);
                if (blnTotalUpRst)
                {
                    InnerCommand.VerifyControl.SendMsg($"上传成功{msg}");
                    InnerCommand.VerifyControl.SendMsg(CtrlCmd.MsgType.检定完成, "");
                }
                else
                {
                    InnerCommand.VerifyControl.SendMsg(CtrlCmd.MsgType.故障, msg);
                }
            }
        }
        //将表位状态字符串转换为布尔值
        private static bool[] GetRepeatPressMeters(MeterState[] states)
        {
            bool[] rp = new bool[states.Length];
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i] == null) continue;
                if (states[i].Motor == MeterState_Motor.电机不确定 || states[i].Motor == MeterState_Motor.电机行在下)
                {
                    rp[i] = true;
                }
            }
            return rp;
        }
        /// <summary>
        /// 修改检定日期计检日期等
        /// </summary>
        private static void SetMeterTime()
        {
            List<string> sqlSumRetList = new List<string>();
            int intTemp = 96;
            int.TryParse(ConfigHelper.Instance.TestEffectiveTime, out intTemp);
            string stringValidDate = DateTime.Now.AddMonths(intTemp).ToString();
            List<DynamicModel> meterModels = new List<DynamicModel>();
            bool[] yaojianTemp = EquipmentData.MeterGroupInfo.YaoJian;
            for (int i = 0; i < MeterGroupInfo.Meters.Count; i++)
            {
                //if (!yaojianTemp[i])
                //{
                //    string ss = MeterGroupInfo.Meters[i].GetProperty("MD_BAR_CODE") as string;
                //    if (ss == null || ss == "")
                //    {
                //        continue;
                //    }
                //}

                if (MeterGroupInfo.Meters[i].GetProperty("MD_OTHER_1") as string != "1") continue;//不需要上传的话就跳过


                MeterGroupInfo.Meters[i].SetProperty("MD_AUDIT_PERSON", LastCheckInfo.AuditPerson);  //核验员
                MeterGroupInfo.Meters[i].SetProperty("MD_TEST_PERSON", LastCheckInfo.TestPerson); //检验员
                MeterGroupInfo.Meters[i].SetProperty("MD_TEST_DATE", DateTime.Now.ToString());  //检定日期   
                MeterGroupInfo.Meters[i].SetProperty("MD_VALID_DATE", stringValidDate);    //计检日期
                meterModels.Add(MeterGroupInfo.Meters[i].GetDataSource());
            }

            DALManager.MeterTempDbDal.Update("T_TMP_METER_INFO", "METER_ID", meterModels, new List<string>()
            {
                    "MD_TEMPERATURE",                                      //温度
                    "MD_HUMIDITY",                                             //湿度
                    "MD_TEST_PERSON",                                      //检验员
                    "MD_AUDIT_PERSON",                                      //核验员
                    "MD_RESULT",                           //总结论
                    "MD_VALID_DATE",                                         //有效期
                    "MD_TEST_DATE",
            });
            //"MD_SUPERVISOR",                                         //主管

            //保存到本地
            List<string> listWhere = new List<string>();
            List<string> listWhereTemp = new List<string>();

            for (int i = 0; i < Equipment.MeterCount; i++)
            {
                if (yaojianTemp[i])
                {
                    listWhere.Add(string.Format("METER_ID='{0}'", EquipmentData.MeterGroupInfo.Meters[i].GetProperty("METER_ID")));
                    listWhereTemp.Add(string.Format("METER_ID='{0}'", EquipmentData.MeterGroupInfo.Meters[i].GetProperty("METER_ID")));
                }
            }
            string wherePk = string.Join(" or ", listWhere);
            string whereFk = string.Join(" or ", listWhereTemp);

            List<string> tableNames = DALManager.MeterDbDal.GetTableNames();
            tableNames.Remove("METER_INFO");
            #region 先删除正式库中对应的检定信息
            List<string> deleteSqlList = new List<string>();
            deleteSqlList.Add(string.Format("delete from meter_info where {0}", wherePk));
            for (int i = 0; i < tableNames.Count; i++)
            {
                if (tableNames[i].Contains("~"))
                {
                    continue;
                }
                deleteSqlList.Add(string.Format("delete from {0} where {1}", tableNames[i], whereFk));
            }
            int deleteCount = DALManager.MeterDbDal.ExecuteOperation(deleteSqlList);
            LogManager.AddMessage(string.Format("删除正式库中过时数据,共删除{0}条", deleteCount), EnumLogSource.数据库存取日志, EnumLevel.Information);
            #endregion

            #region 插入临时库中的检定信息
            List<DynamicModel> metersTemp = DALManager.MeterTempDbDal.GetList("T_TMP_METER_INFO", wherePk);
            int insertCount = DALManager.MeterDbDal.Insert("METER_INFO", metersTemp);
            if (insertCount == 0)
            {
                LogManager.AddMessage("向正式库中添加表信息失败", EnumLogSource.数据库存取日志, EnumLevel.Error);
                return;
            }
            LogManager.AddMessage(string.Format("向正式库中添加表信息,共添加{0}条记录", insertCount), EnumLogSource.数据库存取日志, EnumLevel.Information);

            for (int i = 0; i < tableNames.Count; i++)
            {
                if (tableNames[i].Contains("~"))
                {
                    continue;
                }
                List<DynamicModel> modelsResult = DALManager.MeterTempDbDal.GetList("T_TMP_" + tableNames[i], whereFk);
                if (modelsResult.Count > 0)
                {
                    insertCount = DALManager.MeterDbDal.Insert(tableNames[i], modelsResult);
                    LogManager.AddMessage(string.Format("向结论表:{0}添加检定结论,共添加{1}条记录", tableNames[i], insertCount), EnumLogSource.数据库存取日志, EnumLevel.Information);
                }
            }
            #endregion
            LogManager.AddMessage("将检定数据存储到正式数据库成功!", EnumLogSource.数据库存取日志, EnumLevel.Information);

        }


        /// <summary>
        /// 修改数据库里面的上传标识
        /// </summary>
        /// <param name="meter"></param>
        private static void SetMeterData(TestMeterInfo temmeter, int index)
        {
            try
            {
                temmeter.Other2 = "已上传";
                List<string> fieldNames = new List<string>() { "MD_OTHER_2" };//更新上传的状态
                List<DynamicModel> models = new List<DynamicModel>();
                DynamicModel model = new DynamicModel();
                model.SetProperty("METER_ID", temmeter.Meter_ID);
                model.SetProperty("MD_OTHER_2", temmeter.Other2);
                models.Add(model);
                MeterGroupInfo.Meters[index].SetProperty("MD_OTHER_2", "已上传");
                int updateCount = DALManager.MeterTempDbDal.Update("T_TMP_METER_INFO", "METER_ID", models, fieldNames);
            }
            catch (Exception ex)
            {
                LogManager.AddMessage("修改上传状态出错!\r\n" + ex.ToString(), EnumLogSource.数据库存取日志, EnumLevel.TipsError);
            }


        }
        #endregion



    }
}
