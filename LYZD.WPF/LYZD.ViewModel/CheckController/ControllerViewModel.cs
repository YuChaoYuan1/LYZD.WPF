
using LYZD.Core.Enum;
using LYZD.Core.Model.Meter;
using LYZD.DAL.Config;
using LYZD.Utility.Log;
using LYZD.ViewModel.Device;
using LYZD.ViewModel.Time;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZH.MeterProtocol.Protocols.DgnProtocol;
using ZH.MeterProtocol.Struct;

namespace LYZD.ViewModel.CheckController
{
    /// 检定控制器视图
    /// <summary>
    /// 检定控制器视图
    /// </summary>
    public class ControllerViewModel : ViewModelBase
    {
        private string timeText;
        /// <summary>
        /// 系统时间
        /// </summary>
        public string TimeText
        {
            get { return timeText; }
            set { SetPropertyValue(value, ref timeText, "TimeText"); }
        }
        /// <summary>
        /// 收到停止自动检定
        /// </summary>
        public bool AutoStop { get; set; }

        /// <summary>
        /// 是否显示检定悬浮按钮
        /// </summary>
        //public bool IsTestMainVisible { get; set; }
        private bool isTestMainVisible = true;
        /// <summary>
        /// 检定提示信息
        /// </summary>
        public bool IsTestMainVisible
        {
            get { return isTestMainVisible; }
            set { SetPropertyValue(value, ref isTestMainVisible, "IsTestMainVisible"); }
        }

        public bool IsCheckVerify = false;

        //public int DeviceStart=0;//
        private int deviceStart;
        /// <summary>
        /// 台体状态 0-空闲； 1-检测中； 2-暂停；3-停止；4-完成； 5-关机中；8-不合格率报警9-异常
        /// </summary>
        public int DeviceStart
        {
            get { return deviceStart; }
            set
            {
                deviceStart = value;
                if (deviceStart >= 0)
                {
                    EquipmentData.CallMsg("DeviceState"); //发生改变，通知主控
                    DeviceStart = -1;
                }
            }
        }

        /// <summary>
        /// 允许检定
        /// </summary>
        public bool IsEnable
        {
            get
            {
                // return true;
                return index >= 0 && EquipmentData.DeviceManager.IsReady;
            }
        }

        //public TerminalTalker[] Talkers = new TerminalTalker[0]; // 终端通讯组

        private int index;
        /// <summary>
        /// 当前检定点序号,逻辑里面的核心
        /// </summary>
        public int Index
        {
            get
            {
                if (index >= CheckCount)
                {
                    index = CheckCount - 1;
                }
                return index;
            }
            set
            {
                //将执行完毕的检定点设置为非检定状态
                if (EquipmentData.CheckResults.ResultCollection.Count > Index && Index >= 0)
                {
                    EquipmentData.CheckResults.ResultCollection[Index].IsCurrent = false;
                    EquipmentData.CheckResults.ResultCollection[Index].IsChecking = false;
                }
                SetPropertyValue(value, ref index, "StringCheckIndex");
                if (EquipmentData.CheckResults.ResultCollection.Count > Index && Index >= 0)
                {
                    EquipmentData.CheckResults.ResultCollection[Index].IsCurrent = true;
                    EquipmentData.CheckResults.CheckNodeCurrent = EquipmentData.CheckResults.ResultCollection[Index];
                }
                OnPropertyChanged("CheckCount");
                #region 加载检定参数
                if (Index >= 0 && Index < EquipmentData.Schema.ParaValues.Count)
                {
                    DynamicViewModel viewModel = EquipmentData.Schema.ParaValues[Index];
                    EquipmentData.Schema.ParaNo = viewModel.GetProperty("PARA_NO") as string;
                    string paraValue = viewModel.GetProperty("PARA_VALUE") as string;
                    var temp = from item in EquipmentData.Schema.ParaInfo.CheckParas select item.ParaDisplayName;
                    if (paraValue == null)
                    {
                        paraValue = "";
                    }
                    string[] tempValues = paraValue.Split('|');
                    stringPara = "检定参数";
                    List<string> listTemp = new List<string>();
                    for (int i = 0; i < temp.Count(); i++)
                    {
                        if (tempValues.Length > i)
                        {
                            listTemp.Add(string.Format("{0}:{1}", temp.ElementAt(i), tempValues[i]));
                        }
                    }
                    if (listTemp.Count > 0)
                    {
                        StringPara = "参数:" + string.Join(",", listTemp);
                    }
                    else
                    {
                        StringPara = "无参数";
                    }

                    CheckingName = EquipmentData.CheckResults.ResultCollection[Index].Name;
                }
                #endregion
                OnPropertyChanged("IsEnable");
                EquipmentData.LastCheckInfo.SaveCurrentCheckInfo();
                #region 时间统计
                if (index >= 0 && index < TimeMonitor.Instance.TimeCollection.ItemsSource.Count)
                {
                    TimeMonitor.Instance.TimeCollection.SelectedItem = TimeMonitor.Instance.TimeCollection.ItemsSource[index];
                }
                #endregion
            }
        }

        /// 检定点数量
        /// <summary>
        /// 检定点数量
        /// </summary>
        public int CheckCount
        {
            get
            {
                return EquipmentData.CheckResults.ResultCollection.Count;
            }
        }
        /// 检定点序号字符串
        /// <summary>
        /// 检定点序号字符串
        /// </summary>
        public string StringCheckIndex
        {
            get
            {
                if (Index == -1)
                {
                    return "参数录入";
                }
                else if (Index == -3)
                {
                    return "审核存盘";
                }
                return string.Format("({0}/{1})", index + 1, CheckCount);
            }
        }
        /// 当前检定点编号
        /// <summary>
        /// 当前检定点编号
        /// </summary>
        public string CurrentKey
        {
            get
            {
                if (Index >= 0 && Index < EquipmentData.Schema.ParaValues.Count)
                {
                    DynamicViewModel viewModel = EquipmentData.Schema.ParaValues[Index];
                    EquipmentData.Schema.ParaNo = viewModel.GetProperty("PARA_NO") as string;
                    return viewModel.GetProperty("PARA_KEY") as string;
                }
                else
                {
                    return "";
                }
            }
        }

        /// 手动结束检定标记
        /// <summary>
        /// 手动结束检定标记
        /// </summary>
        private bool flagHandStop = false;

        private EnumCheckMode checkMode = EnumCheckMode.连续模式;
        /// 检定模式
        /// <summary>
        /// 检定模式
        /// </summary>
        public EnumCheckMode CheckMode
        {
            get { return checkMode; }
            set { SetPropertyValue(value, ref checkMode, "CheckMode"); }
        }

        #region 日志和提示信息

        private string messageTips = "";
        /// <summary>
        /// 检定提示信息
        /// </summary>
        public string MessageTips
        {
            get { return messageTips; }
            set { SetPropertyValue(value, ref messageTips, "MessageTips"); }
        }
        public static object obj = new object();
        /// <summary>
        /// 提示信息-日志
        /// </summary>
        /// <param name="Tips">内容</param>
        /// <param name="Error">True故障，false正常</param>
        public void MessageAdd(string Tips, EnumLogType logType)
        {
            switch (logType)
            {
                case EnumLogType.错误信息://必须全部显示
                    lock (obj)
                    {
                        LogManager.AddMessage(Tips, EnumLogSource.检定业务日志, EnumLevel.Error);
                    }
                    break;
                case EnumLogType.提示信息://提示日志打开才会保存，否是只是显示在下面
                    EquipmentData.Controller.MessageTips = Tips.Replace("\r\n", "");
                    if (ConfigHelper.Instance.IsOpenLog_Tips)
                    {
                        LogManager.AddMessage(Tips, EnumLogSource.检定业务日志, EnumLevel.Information);
                    }
                    break;
                case EnumLogType.详细信息:
                    if (ConfigHelper.Instance.IsOpenLog_Detailed)
                    {
                        LogManager.AddMessage(Tips, EnumLogSource.检定业务日志, EnumLevel.Information);
                    }
                    break;
                case EnumLogType.流程信息:
                    LogManager.AddMessage(Tips, EnumLogSource.检定业务日志, EnumLevel.Information);
                    break;
                case EnumLogType.提示与流程信息:
                    LogManager.AddMessage(Tips, EnumLogSource.检定业务日志, EnumLevel.Information);
                    EquipmentData.Controller.MessageTips = Tips.Replace("\r\n", ""); ;
                    if (ConfigHelper.Instance.IsOpenLog_Tips)
                    {
                        LogManager.AddMessage(Tips, EnumLogSource.检定业务日志, EnumLevel.Information);
                    }
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// 日志提示--表位日志
        /// </summary>
        /// <param name="Id">终端编号-1开始</param>
        /// <param name="Tips">日志内容</param>
        /// <param name="logType">日志类型</param>
        public void MessageAdd(int Id, string Tips, EnumLogType logType)
        {
            switch (logType)
            {
                case EnumLogType.错误信息://必须全部显示
                    LogManager.AddMessage(Id, Tips, EnumLogSource.检定业务日志, EnumLevel.Error);
                    break;
                case EnumLogType.提示信息://提示日志打开才会保存，否是只是显示在下面
                    EquipmentData.Controller.MessageTips = Tips.Replace("\r\n", "");
                    if (ConfigHelper.Instance.IsOpenLog_Tips)
                    {
                        LogManager.AddMessage(Id, Tips, EnumLogSource.检定业务日志, EnumLevel.Information);
                    }
                    break;
                case EnumLogType.详细信息:
                    if (ConfigHelper.Instance.IsOpenLog_Detailed)
                    {
                        LogManager.AddMessage(Id, Tips, EnumLogSource.检定业务日志, EnumLevel.Information);
                    }
                    break;
                case EnumLogType.流程信息:
                    LogManager.AddMessage(Id, Tips, EnumLogSource.检定业务日志, EnumLevel.Information);
                    break;
                case EnumLogType.提示与流程信息:
                    LogManager.AddMessage(Id, Tips, EnumLogSource.检定业务日志, EnumLevel.Information);
                    EquipmentData.Controller.MessageTips = Tips.Replace("\r\n", "");
                    if (ConfigHelper.Instance.IsOpenLog_Tips)
                    {
                        LogManager.AddMessage(Id, Tips, EnumLogSource.检定业务日志, EnumLevel.Information);
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        private bool isChecking;

        /// 是否正在检定
        /// <summary>
        /// 是否正在检定
        /// </summary>
        public bool IsChecking
        {
            get { return isChecking; }
            set
            {
                if (value != isChecking)
                {
                    isChecking = value;
                    if (isChecking && !isBusy)
                    {

                        Task.Factory.StartNew(() => VerifyProcess());
                        if (EquipmentData.DeviceManager.Devices.ContainsKey(Device.DeviceName.多功能板))
                            EquipmentData.DeviceManager.Set_Lv();
                        if (EquipmentData.DeviceManager.Devices.ContainsKey(Device.DeviceName.时基源))
                            EquipmentData.DeviceManager.SetTimePulse(true);
                    }
                    else
                    {
                        EquipmentData.CheckResults.ResultCollection[Index].IsChecking = false;
                        EquipmentData.IsEthernetOnlineNow = true;
                        LogManager.AddMessage("停止检定!", EnumLogSource.检定业务日志, EnumLevel.Information);
                        currentStepWaitHandle.Set();
                        if (EquipmentData.DeviceManager.Devices.ContainsKey(Device.DeviceName.功率源))
                        {
                            if (CheckMode == EnumCheckMode.连续模式 || EquipmentData.SingleOffPower)
                            {
                                VerifyBase.DeviceControl.PowerOff();  //TODO:停止检定后关源 
                            }

                        }
                        if (EquipmentData.DeviceManager.Devices.ContainsKey(Device.DeviceName.多功能板))
                            EquipmentData.DeviceManager.Set_Huang();
                    }
                }
                OnPropertyChanged("IsChecking");
            }
        }

        private bool newArrived;
        /// 新消息到来
        /// <summary>
        /// 新消息到来
        /// </summary>
        public bool NewArrived
        {
            get { return newArrived; }
            set
            {
                SetPropertyValue(value, ref newArrived, "NewArrived");
            }
        }

        #region 检定过程控制
        /// <summary>
        /// 异常停止检定
        /// </summary>
        public void TryStopVerify()
        {
            IsCheckVerify = false;
            flagHandStop = true;
            //手动停止，时间计数无效
            TimeMonitor.Instance.ActiveCurrentItem(Index);
            TimeMonitor.Instance.FinishCurrentItem(Index);
            if (temVerify != null)
            {
                temVerify.Stop = true;//停止检定
            }
            if (temVerify2 != null)
            {
                temVerify2.Stop = true;//停止检定
            }
            LogManager.AddMessage("请等待,正在停止检定台...", EnumLogSource.检定业务日志);
            if (EquipmentData.DeviceManager.Devices.ContainsKey(Device.DeviceName.多功能板))
            {
                EquipmentData.DeviceManager.Set_Hong();
                if (ConfigHelper.Instance.Marketing_Type == "LY数据服务" && ConfigHelper.Instance.VerifyModel == "自动模式")
                {
                    InnerCommand.VerifyControl.SendMsg(CtrlCmd.MsgType.故障, "异常停止检定");
                }
            }
        }

        /// 停止检定
        /// <summary>
        /// 停止检定
        /// </summary>
        public void StopVerify()
        {

            //DeviceStart = 3;
            flagHandStop = true;
            IsCheckVerify = false;
            //手动停止，时间计数无效
            TimeMonitor.Instance.ActiveCurrentItem(Index);
            TimeMonitor.Instance.FinishCurrentItem(Index);
            //TODO2 加上停止检定的命令
            // WcfHelper.Instance.StopVerify(CurrentKey);
            if (temVerify != null)
            {
                temVerify.Stop = true;//停止检定
            }
            if (temVerify2 != null)
            {
                temVerify2.Stop = true;//停止检定
            }
            IsMeterDebug = false;
            LogManager.AddMessage("请等待,正在停止检定台...", EnumLogSource.检定业务日志);
            if (ConfigHelper.Instance.Marketing_Type == "LY数据服务" && ConfigHelper.Instance.VerifyModel == "自动模式")
            {
                InnerCommand.VerifyControl.SendMsg(CtrlCmd.MsgType.空闲, "停止检定");
            }
        }
        /// 单步检定
        /// <summary>
        /// 单步检定
        /// </summary>
        public void StepVerify()
        {
            CheckMode = EnumCheckMode.单步模式;
            IsChecking = true;
            IsCheckVerify = true;

        }
        /// <summary>
        /// 调表
        /// </summary>
        public void MeterDebug()
        {
            IsMeterDebug = !IsMeterDebug;
            temVerify.IsMeterDebug = IsMeterDebug;
        }

        public bool isMeterDebug = false;
        /// <summary>
        /// 是否调表
        /// </summary>
        public bool IsMeterDebug
        {
            get { return isMeterDebug; }
            set { SetPropertyValue(value, ref isMeterDebug, "IsMeterDebug"); }
        }
        /// 连续检定
        /// <summary>
        /// 连续检定
        /// </summary>
        public void RunningVerify()
        {
            CheckMode = EnumCheckMode.连续模式;
            IsChecking = true;
            IsCheckVerify = true;

        }
        /// 循环检定
        /// <summary>
        /// 循环检定
        /// </summary>
        public void LoopVerify()
        {
            IsCheckVerify = true;
            IsChecking = true;
            CheckMode = EnumCheckMode.循环模式;
        }
        /// 当前检定项检定完毕
        /// <summary>
        /// 当前检定项检定完毕
        /// </summary>
        public void FinishCurrentStep()
        {
            string checkName = EquipmentData.CheckResults.ResultCollection[Index].Name;
            LogManager.AddMessage(string.Format("项目： {0}  检定完成.", checkName), EnumLogSource.检定业务日志);
            if (isBusy)
            {
                currentStepWaitHandle.Set();
            }
            else
            {
                IsChecking = false;
            }
        }
        #endregion

        #region 检定线程
        /// <summary>
        /// 更新电能表协议信息
        /// </summary>
        public void UpdateMeterProtocol()
        {

            MeterHelper.Instance.InitCarrier();//初始化一下载波协议
            MeterHelper.Instance.Init();
            //把数据库中读取到的串口数据给到类里面
            DgnProtocolInfo[] protocols = MeterHelper.Instance.GetAllProtocols();
            VerifyBase.meterInfo[0].DgnProtocol = protocols[0];
            string[] meterAddress = MeterHelper.Instance.GetMeterAddress();
            ComPortInfo[] comPorts = MeterHelper.Instance.GetComPortInfo();
            MeterProtocolAdapter.Instance.Initialize(protocols, meterAddress, comPorts);
        }


        private AutoResetEvent currentStepWaitHandle = new AutoResetEvent(false);
        /// <summary>
        /// 检定执行过程
        /// </summary>
        public void VerifyProcess()   //【标注--开始检定】
        {
            isBusy = true;//正在忙碌
            IsCheclProtocol = false;
            EquipmentData.IsEthernetOnlineNow = false;
            TestStratSet();
            //DeviceStart = 1;
            Thread.Sleep(1500);
            UpdateMeterProtocol();//每次开始检定，更新一下电表协议
            if (!ConfigHelper.Instance.IsSkipCompleted)
            {
                for (int i = 0; i < EquipmentData.CheckResults.ResultCollection.Count; i++)
                {
                    EquipmentData.CheckResults.ResultCollection[i].IsChecked = false;
                }
            }

            if (CheckMode != EnumCheckMode.连续模式) //单步检定和循环检定的时候可以重复检定
            {
                if (Index >= 0 || Index < CheckCount)
                {
                    EquipmentData.CheckResults.ResultCollection[Index].IsChecked = false;
                }
            }
            while (IsChecking)
            {
                try
                {
                    if (Index < 0 || Index >= CheckCount)
                    {
                        IsChecking = false;
                        break;
                    }
                    if (EquipmentData.CheckResults.ResultCollection[Index].IsSelected && !EquipmentData.CheckResults.ResultCollection[Index].IsChecked)
                    {

                        if (!InvokeStartVerufy()) //开始检定，根据类名调用检定方法
                        {
                            break;
                        }

                        #region 等待当前检定项结束
                        currentStepWaitHandle.Reset();
                        currentStepWaitHandle.WaitOne();
                        #endregion
                    }

                    #region 检定器将要执行的动作
                    //如果手动终止
                    if (flagHandStop)
                    {
                        LogManager.AddMessage("当前检定项被手动终止!", EnumLogSource.检定业务日志);
                        flagHandStop = false;
                        IsChecking = false;
                        EquipmentData.CheckResults.ResultCollection[Index].IsChecked = false;
                        if (ConfigHelper.Instance.OperatingConditionsYesNo == "是")
                        {
                            IMICP.OpenPortIMICP open = new IMICP.OpenPortIMICP();
                            open.WorkingStatus("02", "04");
                            open.EventEscalation("0120", EquipmentData.CheckResults.ResultCollection[Index].ItemKey);
                        }
                        break;
                    }
                    //统计检定项的时间
                    TimeMonitor.Instance.FinishCurrentItem(Index);

                    //根据检定模式判断将要执行的检定动作
                    switch (CheckMode)
                    {
                        case EnumCheckMode.单步模式:
                            IsChecking = false;
                            LogManager.AddMessage(string.Format("检定项执行完毕,{0}.", EquipmentData.CheckResults.ResultCollection[Index].Name), EnumLogSource.检定业务日志);
                            if (ConfigHelper.Instance.OperatingConditionsYesNo == "是")
                            {
                                IMICP.OpenPortIMICP open = new IMICP.OpenPortIMICP();
                                open.WorkingStatus("02", "04");
                                open.EventEscalation("0120", EquipmentData.CheckResults.ResultCollection[Index].ItemKey);
                            }
                            break;
                        case EnumCheckMode.连续模式:
                            //判断是否要执行下一个检定项
                            if (IsChecking && Index < CheckCount - 1 && Index >= 0)
                            {
                                //如果当前点是最后一个检定点
                                Index = Index + 1;
                            }
                            else
                            {
                                IsChecking = false;
                                if (ConfigHelper.Instance.OperatingConditionsYesNo == "是")
                                {
                                    IMICP.OpenPortIMICP open = new IMICP.OpenPortIMICP();
                                    open.WorkingStatus("02", "04");
                                    open.EventEscalation("0120", EquipmentData.CheckResults.ResultCollection[Index].ItemKey);
                                }
                                LogManager.AddMessage(string.Format("检定项执行完毕,{0}.", EquipmentData.CheckResults.ResultCollection[Index].Name), EnumLogSource.检定业务日志);
                                //TODO2全部检定完成，判断是否要自动上传数据，要的话就上传
                                EquipmentData.CallMsg("VerifyCompelate");
                                //DeviceStart = 4;

                            }
                            break;
                        case EnumCheckMode.循环模式:
                            //啥都不要干,进入下一个循环
                            EquipmentData.CheckResults.ResultCollection[Index].IsChecked = false;
                            break;
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    LogManager.AddMessage(string.Format("调用检定开始服务异常:{0}", e.ToString()), EnumLogSource.检定业务日志, EnumLevel.Error);
                    IsChecking = false;
                    //DeviceStart = 9;
                }
            }
            isBusy = false;
        }



        private void TestStratSet()
        {

            if (EquipmentData.Equipment.IsDemo) return;
            if (ConfigHelper.Instance.Marketing_Type == "LY数据服务" && ConfigHelper.Instance.VerifyModel == "自动模式")
            {
                InnerCommand.VerifyControl.SendMsg($"开始检定({CheckMode})。");
                InnerCommand.VerifyControl.SendMsg(CtrlCmd.MsgType.正在运行, "");
            }
            //加上判断是自动模式并且自动压接表位的情况，下压电机，等待8秒
            if (EquipmentData.Equipment.VerifyModel == "自动模式" && ConfigHelper.Instance.IsMete_Press)
            {
                MessageAdd($"正在电机下行...", EnumLogType.提示信息);
                for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
                {
                    if (EquipmentData.MeterGroupInfo.YaoJian[i])
                    {
                        if (!EquipmentData.DeviceManager.ElectricmachineryContrnl(1, (byte)(i + 1)))  //下压电机
                        {
                            EquipmentData.DeviceManager.ElectricmachineryContrnl(1, (byte)(i + 1));//第一次失败在发一次
                        }
                    }
                }
                int times = ConfigHelper.Instance.Mete_Press_Time;
                while (true)
                {
                    if (times < 0)
                        break;
                    MessageAdd($"电机下行，等待{times}秒...", EnumLogType.提示信息);
                    Thread.Sleep(1000);
                    times--;
                }
            }

            MessageAdd($"正在设置表位继电器...", EnumLogType.提示信息);
            int DeviceCount = EquipmentData.DeviceManager.Devices[Device.DeviceName.误差板].Count;
            int indexBwcount = EquipmentData.Equipment.MeterCount / DeviceCount;
            int connType = (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.Channel232 || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelMaintain) ? 3 : 0;



            string Type485 = "";
            switch (VerifyBase.TerminalChannelType)
            {
                case Core.Enum.Cus_EmChannelType.Channel232:
                    connType = 3;
                    break;
                case Core.Enum.Cus_EmChannelType.ChannelMaintain:
                    foreach (var item in VerifyBase.meterInfo)
                    {
                        if (item.YaoJianYn)
                        {
                            switch (item.MD_TerminalType)
                            {
                                case "集中器I型13版":
                                case "集中器I型22版":
                                case "专变III型13版":
                                case "专变III型22版":
                                case "能源控制器":
                                    Type485 = "4852";
                                    connType = 0;
                                    break;
                                case "融合终端":
                                    Type485 = "4854";
                                    connType = 2;
                                    break;
                                default:
                                    Type485 = "默认4852";
                                    connType = 0;
                                    break;
                            }
                            LogManager.AddMessage(string.Format($"要检终端类型{item.MD_TerminalType},切换到{Type485}通道"), EnumLogSource.检定业务日志, EnumLevel.Error);
                            break;
                        }
                    }
                    break;
                default:
                    connType = 0;
                    break;
            }
       
            for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
            {
                int id = i / indexBwcount;
                int onOff = EquipmentData.MeterGroupInfo.YaoJian[i] ? 2 : 1;
                //EquipmentData.DeviceManager.ControlConnrRelay3(connType, (byte)(i + 1), id); //0=485,1=232    
                //Thread.Sleep(50);
                EquipmentData.DeviceManager.ControlMeterRelay(onOff, (byte)(i + 1), id);  //开启继电器
                Thread.Sleep(150);

            }
            SetTerminalTypeErrorBoard();
            //大小电流切换版
            if (ConfigHelper.Instance.Marketing_Type == "LY数据服务" && ConfigHelper.Instance.VerifyModel == "自动模式")
            {
                SetPowerContlor();
            }

            if (!ConfigHelper.Instance.ConstModel)//自动常数，每次检定前发送切回自动档
            {
                double[] d = new double[6] { 0, 0, 0, 0, 0, 0 };
                long conststd = 0;
                EquipmentData.DeviceManager.StdGear(0x13, ref conststd, ref d);
            }

        }


        /// <summary>
        /// 根据终端类型设置误差版
        /// </summary>
        public void SetTerminalTypeErrorBoard()
        {
            foreach (var item in VerifyBase.meterInfo)
            {
                if (item.YaoJianYn)
                {
                    int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
                    byte type = 0;
                    for (int i = 0; i < num; i++)
                    {
                        switch (item.MD_TerminalType)
                        {
                            case "集中器I型13版":
                                type = 1;
                                break;
                            case "集中器I型22版":
                                type = 2;
                                break;
                            case "专变III型13版":
                                type = 3;
                                break;
                            case "专变III型22版":
                                type = 4;
                                break;
                            case "融合终端":
                                type = 5;
                                break;
                            case "能源控制器":
                                type = 6;
                                break;
                            default:
                                break;
                        }
                        EquipmentData.DeviceManager.SetZDType(type, (byte)i);
                    }
                    break;

                }
            }
        }
        /// <summary>
        /// 流水线控制大小电流切换版
        /// </summary>
        public void SetPowerContlor()
        {
            foreach (var item in VerifyBase.meterInfo)
            {
                if (item.YaoJianYn)
                {
                    int num = EquipmentData.DeviceManager.Devices[DeviceName.多功能板].Count;
                    byte type = 0;
                    for (int i = 0; i < num; i++)
                    {
                        switch (item.MD_TerminalType)
                        {
                            case "集中器I型13版":
                            case "集中器I型22版":
                            case "专变III型13版":
                            case "专变III型22版":
                                type = 0;
                                break;
                            case "融合终端":
                            case "能源控制器":
                                type = 1;
                                break;
                            default:
                                break;
                        }
                        EquipmentData.DeviceManager.PowerTpye((byte)0x13, type, i);
                        System.Threading.Thread.Sleep(150);
                    }
                    break;

                }
            }


        }

        #region //自动化预热升源
        public bool AutoPowerOn()
        {

            LogManager.AddMessage("开始预热升源");
            Cus_PowerYuanJian ele = Cus_PowerYuanJian.H;
            TestMeterInfo meter = VerifyBase.OnMeterInfo;
            if (meter.MD_WiringMode == "单相") ele = Cus_PowerYuanJian.A;
            return VerifyBase.DeviceControl.PowerOn(meter.MD_UB, meter.MD_UB, meter.MD_UB, 0, 0, 0, ele, PowerWay.正向有功, "1.0");
        }

        public void AutoPowerOnSuccessWait()
        {
            int time = 120;
            while (time > 0)
            {
                LogManager.AddMessage($"预热升源成功等待预热时间2分钟,剩余:{time}秒...", EnumLogSource.检定业务日志, EnumLevel.Information);
                System.Threading.Thread.Sleep(1000);
                time--;
                if (AutoStop) return;
            }
        }
        #endregion

        ///正在忙碌
        private bool isBusy = false;

        private string stringPara;
        /// <summary>
        /// 检定参数字符串
        /// </summary>
        public string StringPara
        {
            get { return stringPara; }
            set { SetPropertyValue(value, ref stringPara, "StringPara"); }
        }
        #endregion

        private string checkingName;
        /// <summary>
        /// 当前检定项的名称
        /// </summary>
        public string CheckingName
        {
            get { return checkingName; }
            set { SetPropertyValue(value, ref checkingName, "CheckingName"); }
        }

        /// <summary>
        /// 检定客户端发起的正在检定
        /// </summary>
        public void NotifyIsChecking(string checkState)
        {
            if (checkState == "1")
            {
                isChecking = true;
            }
            else
            {
                //isChecking = false;
            }
            OnPropertyChanged("IsChecking");
        }


        /// <summary>
        /// 开始检定
        /// </summary>
        /// <returns></returns>
        private bool InvokeStartVerufy()
        {

            CheckInfo.CheckNodeViewModel nodeTemp = EquipmentData.CheckResults.ResultCollection[Index];

            //解析检定参数
            DynamicViewModel viewModel = EquipmentData.Schema.ParaValues[Index];
            if (viewModel != null)
            {
                EquipmentData.Schema.ParaNo = viewModel.GetProperty("PARA_NO") as string;
                string className = EquipmentData.Schema.ParaInfo.ClassName;//检定点的类名
                var temp = from item in EquipmentData.Schema.ParaInfo.CheckParas select item.ParaDisplayName;
                string paraFormat = string.Join("|", temp);
                nodeTemp.IsChecking = true;
                string key = viewModel.GetProperty("PARA_KEY") as string;
                string paraValue = viewModel.GetProperty("PARA_VALUE") as string;
                string name = viewModel.GetProperty("PARA_NAME") as string;
                string apply = EquipmentData.Schema.ParaInfo.ApplyName;//viewModel.GetProperty("PARA_APPLY") as string;   //适用性

                if (ConfigHelper.Instance.OperatingConditionsYesNo == "是")
                {
                    IMICP.OpenPortIMICP open = new IMICP.OpenPortIMICP();
                    open.WorkingStatus("04", "02");
                    open.EventEscalation("0119", key);
                }

                //清除当前实时报文
                EquipmentData.CheckResults.ResultCollection[index].LiveFrames.ClearFrames();

                bool resultStart = StartVerify(key, className, paraFormat, paraValue, name, apply);//调用检定方法，开始检定
                if (resultStart)   //调用成功
                {
                    EquipmentData.CheckResults.ResultCollection[Index].IsChecked = true;
                    TimeMonitor.Instance.StartCurrentItem(Index);
                    //清空当前的检定
                    if (CheckMode != EnumCheckMode.循环模式)
                    {
                        EquipmentData.CheckResults.ResetCurrentResult(); // 清空旧的检定结果
                    }
                }
                else//调用失败
                {
                    IsChecking = false;
                    LogManager.AddMessage("调用开始检定方法失败!错误代码10001-001", EnumLogSource.检定业务日志, EnumLevel.Error);
                    //DeviceStart = 9;
                    return false;
                }


            }
            else
            {

                IsChecking = false;
                EquipmentData.DeviceManager.SetEquipmentThreeColor(18, 1);
                LogManager.AddMessage("检定过程出现异常,索引超出范围!错误代码10001-002", EnumLogSource.检定业务日志, EnumLevel.Error);
                //DeviceStart = 9;
                return false;
            }
            return true;
        }


        public static Assembly assemblys = null;
        public VerifyBase temVerify = null;
        public VerifyBase temVerify2 = null;

        /// 开始检定
        /// <summary>
        /// 开始检定
        /// </summary>
        /// <param name="testNo">检定点编号</param>
        /// <param name="testName">检定方法名称</param>
        /// <param name="testFormat">检定参数值的描述</param>
        /// <param name="testValue">检定参数值</param>
        /// <param name="apply">支持协议类型--适应性</param>
        private bool StartVerify(string testNo, string testName, string testFormat, string testValue, string name, string apply)
        {
            try
            {
                if (assemblys == null)
                {
                    string filePath = @"LYZD.Verify.dll";
                    FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    byte[] bFile = br.ReadBytes((int)fs.Length);
                    br.Close();
                    fs.Close();
                    assemblys = Assembly.Load(bFile);
                }
                VerifyBase Verify;
                if (testName != null && testName.Trim() != "")
                {
                    string className = "LYZD.Verify." + testName;
                    if (apply != null)
                    {
                        if (apply == "104")
                        {
                            className += "_104";
                        }
                        else if (apply == "376.1") //376协议
                        {
                            className += "_376";
                        }
                        else if (apply.IndexOf("698") != -1 && apply.IndexOf("376.1") != -1) //376和698都支持-就需要根据参数录入的协议类型来设置了
                        {
                            if (VerifyBase.TerminalProtocolType == Core.Enum.TerminalProtocolTypeEnum._376)
                            {
                                className += "_376";
                            }
                        }
                    }

                    Type type = assemblys.GetType(className); //获取当前类的类型
                    if (type == null)
                    {
                        LogManager.AddMessage($"调用开始检定方法失败!错误代码10001-004\r\n没有找到{className}的检定方法", EnumLogSource.检定业务日志, EnumLevel.Error);
                        return false;
                    }

                    object obj = Activator.CreateInstance(type);// 创建此类型实例
                    Verify = (VerifyBase)obj;
                }
                else
                {
                    Verify = new VerifyBase();
                }
                //设置检定数据
                Verify.Test_Format = testFormat;
                Verify.Test_Name = name;
                Verify.Test_No = testNo;
                Verify.Test_Value = testValue;
                Verify.IsDemo = EquipmentData.Equipment.IsDemo;
                temVerify = Verify;
                //调用检定方法
                Task task = new Task(() => { InvokeVerify(Verify); });
                task.Start();
                //Verify.Verify();
            }
            catch (Exception e)
            {
                EquipmentData.DeviceManager.Set_Hong();
                LogManager.AddMessage("调用开始检定方法失败!错误代码10001-003:" + e.ToString(), EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }

            return true;
        }


        private void InvokeVerify(VerifyBase Verify)
        {
            Thread.Sleep(50);  //稍微等待一下，保证当前项目切换过来了，以防数据丢失
            Verify.DoVerify();
            FinishCurrentStep();
        }




        #region 同时进行通讯协议检查试验
        private void WriteProtocol()
        {
            if (!IsCheclProtocol)
            {
                return;
            }
            LogManager.AddMessage("正在等待当前同步进行的通讯协议项目结束", EnumLogSource.检定业务日志);
            IsCheclProtocol = false;//结束通讯协议检查
            while (OneIsProtocolCheck)
            {
                Thread.Sleep(200);
            }

        }



        //条件：1需要是连续检定，2：需要是启动或者潜动的时候，3：通讯协议项目大于0
        //有可能遇到的情况：
        //1：启动或者潜动还没结束，通讯协议检查结束
        //2：启动潜动结束了，通讯协议检查还在进行中
        //3：项目同时结束，都在刷新界面
        //4：启动做完了切换到潜动的过程中还在协议检查，是否需要等待
        //5：多个启动潜动项目,是否需要等待
        //6：停止检定了
        //7：检定项目都结束了，通讯协议检查没有结束
        //8：遇到错误停止检定了
        //9：启动潜动和通讯协议检查都在控制源
        //10：启动有电流，而通讯协议检查没有电流，不需要重新升源===因为检定过程没有关源，所以这里进行判断一次，有电压就不需要升源

        //需要的参数：
        //每个检定点检定过没有,开始检定全部置为false，检定完成置为true
        //还需要一个通讯协议检查当前检定点的序号,用于连续检定
        //开始检定中判断当前检定点是不是启动或者潜动，还需要判断当前是否正在进行通讯协议检查试验
        //如果是启动和潜动并且正在进行通讯协议检查就跳过，如果不在就开启线程进行通讯协议检查试验
        //如果不是通讯协议检查试验，并且正在进行通讯协议检查，就需要等待当前通讯协议检查结束，然后关闭通讯协议检查线程。

        /// <summary>
        /// 是否正在进行通讯协议检查
        /// </summary>
        private bool IsCheclProtocol { get; set; }

        private bool OneIsProtocolCheck;//单个项目是否结束--用于启动潜动结束时候结束线程

        /// <summary>
        /// 通讯协议检查点ID
        /// </summary>
        private int ProtocolIndex { get; set; }

        private AutoResetEvent currentStepWaitHandle2 = new AutoResetEvent(false);
        private void VerifyProcess2()
        {
            if (CheckMode != EnumCheckMode.连续模式)
                return;
            if (!EquipmentData.Equipment.IsSame)
                return;
            if (IsCheclProtocol)
                return;
            IsCheclProtocol = true;
            if (!EquipmentData.Schema.ExistNode("17003") && !EquipmentData.Schema.ExistNode("17001"))
                return;
            ProtocolIndex = -1;
            for (int i = 0; i < EquipmentData.Schema.ParaValues.Count; i++)
            {
                DynamicViewModel viewModel = EquipmentData.Schema.ParaValues[i];
                if (viewModel != null)
                {
                    string PARA_NO = viewModel.GetProperty("PARA_NO") as string;
                    if (PARA_NO == "17003" || PARA_NO == "17001")//通讯协议检查2
                    {
                        if (!EquipmentData.CheckResults.ResultCollection[i].IsChecked)//找到第一个没有检定过的
                        {
                            ProtocolIndex = i;
                            break;
                        }
                    }
                }
            }
            if (ProtocolIndex < 0) //都检定过了,就退出了
            {
                return;
            }
            Task.Factory.StartNew(() => sss());
        }

        private void sss()
        {
            Thread.Sleep(10000); //启动潜都得升源，需要等待10秒在开始
            LogManager.AddMessage("开始同步进行通讯协议检查", EnumLogSource.检定业务日志);
            while (IsChecking)
            {
                try
                {
                    if (ProtocolIndex < 0 || ProtocolIndex >= CheckCount)
                    {
                        //IsChecking = false;
                        LogManager.AddMessage("通讯协议项目结束====", EnumLogSource.检定业务日志);
                        break;
                    }
                    DynamicViewModel viewModel = EquipmentData.Schema.ParaValues[ProtocolIndex];
                    string PARA_NO = viewModel.GetProperty("PARA_NO") as string;
                    if ((PARA_NO != "17003" && PARA_NO != "17001") || !IsCheclProtocol)   //这里说明通讯协议检查全部结束了
                    {
                        LogManager.AddMessage("通讯协议项目结束====", EnumLogSource.检定业务日志);
                        break;
                    }
                    if (EquipmentData.CheckResults.ResultCollection[ProtocolIndex].IsSelected && !EquipmentData.CheckResults.ResultCollection[ProtocolIndex].IsChecked)
                    {
                        OneIsProtocolCheck = true;
                        if (!StartVerifyProtocol(viewModel)) //开始检定，根据类名调用检定方法
                        {
                            continue;
                        }

                        #region 等待当前检定项结束
                        currentStepWaitHandle2.Reset();
                        currentStepWaitHandle2.WaitOne();
                        //如果手动终止
                        if (!IsChecking)
                        {
                            LogManager.AddMessage("当前检定项被手动终止!", EnumLogSource.检定业务日志);
                            EquipmentData.CheckResults.ResultCollection[ProtocolIndex].IsChecked = false;
                            break;
                        }
                        #endregion
                    }
                    else
                    {
                        ProtocolIndex++;
                    }
                }
                catch (Exception e)
                {

                    LogManager.AddMessage(string.Format("调用检定开始服务异常:{0}", e.ToString()), EnumLogSource.检定业务日志, EnumLevel.Error);
                    OneIsProtocolCheck = false;
                    //IsChecking = false;
                }
            }
            IsCheclProtocol = false;
        }

        private bool StartVerifyProtocol(DynamicViewModel viewModel)
        {
            //CheckInfo.CheckNodeViewModel nodeTemp = EquipmentData.CheckResults.ResultCollection[Index];
            if (viewModel == null)
            {
                return false;
            }
            Schema.ParaInfoViewModel ParaInfo = new Schema.ParaInfoViewModel();
            ParaInfo.ParaNo = viewModel.GetProperty("PARA_NO") as string;
            string className = ParaInfo.ClassName;//检定点的类名
            var temp = from item in ParaInfo.CheckParas select item.ParaDisplayName;
            string paraFormat = string.Join("|", temp);
            //nodeTemp.IsChecking = true;
            string key = viewModel.GetProperty("PARA_KEY") as string;
            string paraValue = viewModel.GetProperty("PARA_VALUE") as string;

            bool resultStart = StartVerify2(key, className, paraFormat, paraValue);//调用检定方法，开始检定
            if (resultStart)   //调用成功
            {
                EquipmentData.CheckResults.ResultCollection[ProtocolIndex].IsChecked = true;
                EquipmentData.CheckResults.ResetCurrentResult2(ProtocolIndex); // 清空旧的检定结果
            }

            return true;
        }
        /// <summary>
        /// 开始检定
        /// </summary>
        /// <param name="testNo">检定点编号</param>
        /// <param name="testName">检定方法名称</param>
        /// <param name="testFormat">检定参数值的描述</param>
        /// <param name="testValue">检定参数值</param>
        private bool StartVerify2(string testNo, string testName, string testFormat, string testValue)
        {
            try
            {
                if (assemblys == null)
                {
                    string filePath = @"LYZD.Verify.dll";
                    FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    byte[] bFile = br.ReadBytes((int)fs.Length);
                    br.Close();
                    fs.Close();
                    assemblys = Assembly.Load(bFile);
                }
                VerifyBase Verify;
                if (testName != null && testName.Trim() != "")
                {
                    string className = "LYZD.Verify." + testName;
                    Type type = assemblys.GetType(className); //获取当前类的类型
                    if (type == null)
                    {
                        LogManager.AddMessage("调用开始检定方法失败!错误代码10001-004", EnumLogSource.检定业务日志, EnumLevel.Error);
                        return false;
                    }

                    object obj = Activator.CreateInstance(type);// 创建此类型实例
                    Verify = (VerifyBase)obj;
                }
                else
                {
                    Verify = new VerifyBase();
                }
                //设置检定数据
                Verify.Test_Format = testFormat;
                Verify.Test_No = testNo;
                Verify.Test_Value = testValue;
                Verify.IsDemo = EquipmentData.Equipment.IsDemo;

                temVerify2 = Verify;
                //调用检定方法
                Task task = new Task(() => { InvokeVerify2(Verify); });
                task.Start();
                //Verify.Verify();
            }
            catch (Exception e)
            {
                LogManager.AddMessage("调用开始检定方法失败!错误代码10001-003:" + e, EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }

            return true;
        }

        private void InvokeVerify2(VerifyBase Verify)
        {
            //  LogManager.AddMessage(EquipmentData.CheckResults.ResultCollection[ProtocolIndex].Name + "=======开始检定", EnumLogSource.检定业务日志);
            Thread.Sleep(50);  //稍微等待一下，保证当前项目切换过来了，以防数据丢失
            Verify.DoVerify();
            FinishCurrentStep2();
            //Thread.Sleep(50);
        }
        /// 当前检定项检定完毕
        /// <summary>
        /// 当前检定项检定完毕
        /// </summary>
        public void FinishCurrentStep2()
        {
            string checkName = EquipmentData.CheckResults.ResultCollection[ProtocolIndex].Name;
            LogManager.AddMessage(string.Format("项目： {0}  检定完成.", checkName), EnumLogSource.检定业务日志);
            OneIsProtocolCheck = false;
            ProtocolIndex++;
            if (isBusy)
            {
                currentStepWaitHandle2.Set();
            }
        }
        #endregion



    }
}
