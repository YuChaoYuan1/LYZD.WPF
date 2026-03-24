using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing; //添加引用
using System.Windows.Forms;//添加引用，必须用到的
using LY.VirtualMeter.ViewModel;
using LY.VirtualMeter.Core;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic;
using System.Data;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace LY.VirtualMeter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon notifyIcon = null;
        public Socket TupdOne = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public MainWindow()
        {
            InitializeComponent();
            InitialTray();
            this.DataContext = new MainViewWindow();
            this.Closing += MainWindow_Closed;

            //测试();
            初始化();
            Task task = new Task(() =>
            {
                while (true)
                {
                    AppData.BaseData.获取电量();
                    System.Threading.Thread.Sleep(1000);
                }
            });
            task.Start();
            载入配置文件();

            //connectTime = DateTime.Now;
            //Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {
            //        Thread.Sleep(6000);   //一分钟判断一次
            //        if ((DateTime.Now - connectTime).TotalSeconds > 420)    //超过7分钟就连接一次加密机
            //        {
            //            LogInfoHelper.WriteLog("异常日志", "加密机连接时间到达7分钟，开始重连");
            //            connectTime = DateTime.Now;
            //            DataArrival("Cmd,Connect");
            //        }
            //    }
            //});

            System.Windows.Application.Current.MainWindow.WindowState = WindowState.Minimized;
            //System.Windows.Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        //DateTime connectTime;




        private void MainWindow_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process[] myProcess = Process.GetProcessesByName("LYZD.WPF");
            if (myProcess.Length > 0)
            {
                System.Windows.Forms.MessageBox.Show("主控程序还没有关闭，模拟表程序不要关闭！", "提示", MessageBoxButtons.OK);
                e.Cancel = true;
                notifyIcon.Visible = true;
                return;
            }
            else
            {
                notifyIcon.Dispose();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                Environment.Exit(0);

            }
        }

        private void 初始化()
        {

            string str = "";
            str = Function.GetINI("Data", "MeterCount", System.Windows.Forms.Application.StartupPath + "\\Ini\\VirtualMeter.ini", "1");
            AppData.BaseData.Num_Meter = int.Parse(str);
            str = Function.GetINI("Data", "VirtualCount", System.Windows.Forms.Application.StartupPath + "\\Ini\\VirtualMeter.ini", "1");
            AppData.BaseData.Num_Terminal = int.Parse(str);

            //通讯方式
            AppData.BaseData.ConnType = Function.GetINI("Data", "ProtocolType", System.Windows.Forms.Application.StartupPath + "\\Ini\\VirtualMeter.ini", "串口");
            AppData.BaseData.ConnTypeList.Add("串口");
            AppData.BaseData.ProtocolType.Add("645-1997");
            AppData.BaseData.ProtocolType.Add("645-2007");
            AppData.BaseData.ProtocolType.Add("698.45");
            Dictionary<int, Meter> t = new Dictionary<int, Meter>();
            for (int i = 0; i < AppData.BaseData.Num_Meter; i++)
            {
                //Meter meter = new Meter(i + 1);


                Meter meter;
                if (i == AppData.BaseData.Num_Meter - 1)
                {
                    meter = new Meter(i + 1, "000000000099");
                }
                else
                {
                    meter = new Meter(i + 1);
                }
                AppData.BaseData.MeterObject.Add(meter);
                t.Add(i + 1, meter);

                SetViewModel setViewModel = new SetViewModel();
                setViewModel.Name = "表" + (i + 1);
                setViewModel.表地址 = (i + 1).ToString().PadLeft(12, '0');
                if (i == AppData.BaseData.Num_Meter - 1)
                {
                    setViewModel.表地址 = "000000000099";
                }
                //setViewModel.表地址 = "90000000000" + (i + 1);
                //setViewModel.表地址 = (i + 1).ToString().PadLeft(12, '0');
                AppData.BaseData.设置.Add(setViewModel);
            }

            #region 添加载波

            #endregion


            for (int i = 1; i <= AppData.BaseData.Num_Terminal; i++)
            {
                string[] strTmp = Function.GetINI("Data", "Meter" + i, System.Windows.Forms.Application.StartupPath + "\\Ini\\VirtualMeter.ini", "").Split('|');
                MeterPortData meterPort = new MeterPortData();
                meterPort.MeterNo = i;
                if (strTmp.Length > 6)
                {
                    meterPort.PortNo = int.Parse(strTmp[0]);
                    meterPort.Server_Ip = strTmp[1];
                    meterPort.Server_TelePort = int.Parse(strTmp[2]);
                    meterPort.Server_LocalPort = int.Parse(strTmp[3]);
                    meterPort.Rate = int.Parse(strTmp[4]);
                    meterPort.DeviceID = int.Parse(strTmp[5]);
                    meterPort.ChanneNo = int.Parse(strTmp[6]);
                }
                else
                {
                    meterPort.PortNo = i + 1;
                    meterPort.Server_Ip = "192.168.100.100";
                    meterPort.Server_TelePort = 20000;
                    meterPort.Server_LocalPort = 10003;
                    meterPort.Rate = 1;
                    meterPort.DeviceID = 1;
                    meterPort.ChanneNo = 1;
                }
                AppData.BaseData.Meter_PortData.Add(meterPort);
                Server server = new Server(meterPort.MeterNo, meterPort.PortNo, meterPort.Server_Ip,
                     meterPort.Server_TelePort, meterPort.Server_LocalPort, meterPort.Rate.ToString(),
                   (uint)meterPort.DeviceID, (uint)meterPort.ChanneNo,
                     AppData.BaseData.MeterObject, Function.获取通讯方式编号(AppData.BaseData.ConnType));
                AppData.BaseData.TerminalObject.Add(server);
            }


            List<string> list = new List<string>() { "组合有功", "正向有功", "反向有功", "组合无功1", "组合无功2", "第一象限无功", "第二象限无功", "第三象限无功", "第四象限无功" };
            for (int i = 0; i < list.Count; i++)
            {
                MeterDLViewModel meterDL = new MeterDLViewModel() { Name = list[i] };
                AppData.BaseData.电量信息.Add(meterDL);
            }
        }

        private void 测试()
        {

            for (int i = 0; i < 20; i++)
            {
                MeterLogViewModel meterLogView = new MeterLogViewModel();
                meterLogView.Name = "终端" + (i + 1).ToString();
                meterLogView.Log += (i + 1).ToString();
                //AppData.LogViewModel.Log_Meter.Add(meterLogView);
                AppData.LogViewModel.Log_PC += (i + 1).ToString();
            }
            for (int i = 0; i < 1000; i++)
            {
                //AppData.LogViewModel.Log_Meter[0].Log+= (i + 1).ToString() ;

                AppData.LogViewModel.Log_PC += (i + 1).ToString();
            }

        }

        private Dictionary<string, string> SystemMode = null;
        

        private void 载入配置文件()
        {

            DataTable DB_Value = AccessHelper.GetDataTable("select * from T_CONFIG_PARA_VALUE where CONFIG_NO='04001'");
            DataTable DB_Format = AccessHelper.GetDataTable("select * from T_CONFIG_PARA_FORMAT where CONFIG_NO='04001'");
            string[] value = DB_Value.Rows[0]["CONFIG_VALUE"].ToString().Split('|');
            string[] format = DB_Format.Rows[0]["CONFIG_VIEW"].ToString().Split('|');
            SystemMode = new Dictionary<string, string>();
            SystemMode.Clear();
            for (int i = 0; i < format.Length; i++)
            {
                if (SystemMode.ContainsKey(format[i]))
                    SystemMode.Remove(format[i]);
                SystemMode.Add(format[i], value[i]);
            }
            int ret = 0;
            Server.strEncryptionType = getValue("加密机连接模式");
            //if (Server.strEncryptionType == "直连密码机版")
            //    ret = FactoryEncryptionFunction698.ConnectDevice(getValue("加密机IP"), getValue("加密机端口"), "5");
            //else
            //    ret = EncryptionFunction698.ConnectDevice(getValue("加密机IP"), getValue("加密机端口"), "5");
            LogInfoHelper.OpenService();

            AppData.加密机IP = getValue("加密机IP");
            AppData.加密机端口= getValue("加密机端口");
            SocketUdpControl.Init();


            Thread t = new Thread(数据监听);
            t.Start();

        }
        private void 数据监听()
        {
            byte[] Tbyte;
            int Tlen = 0;
            string Tstr = "";
            while (true)
            {
                Tbyte = new byte[1024];
                try
                {
                    Tlen = SocketUdpControl.SocketUdp.Receive(Tbyte);
                    Tstr = System.Text.Encoding.ASCII.GetString(Tbyte, 0, Tlen);
                    DataArrival(Tstr);
                }
                catch
                {
                }

            }
        }

        /// <summary>
        /// 读取系统配置项目值
        /// </summary>
        /// <param name="Tkey">系统项目ID</param>
        /// <returns>系统项目配置值</returns>
        public string getValue(string Tkey)
        {
            if (SystemMode.Count == 0)
                return "";
            if (SystemMode.ContainsKey(Tkey))
                return SystemMode[Tkey];
            else
                return "";
        }


        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image imageTemp = e.OriginalSource as System.Windows.Controls.Image;
            if (imageTemp != null)
            {
                switch (imageTemp.Name)
                {
                    case "imageMin":
                        //initialTray();
                        System.Windows.Application.Current.MainWindow.WindowState = WindowState.Minimized;
                        break;
                    case "imageMax":
                        if (System.Windows.Application.Current.MainWindow.WindowState == WindowState.Maximized)
                        {
                            System.Windows.Application.Current.MainWindow.WindowState = WindowState.Normal;
                            imageMax.Source = new BitmapImage(new Uri(@"./Assets/Images/Max.png", UriKind.RelativeOrAbsolute));
                        }
                        else
                        {
                            System.Windows.Application.Current.MainWindow.WindowState = WindowState.Maximized;
                            imageMax.Source = new BitmapImage(new Uri(@"./Assets/Images/Reduction.png", UriKind.RelativeOrAbsolute));
                        }
                        break;
                    case "imageClose":

                        //Environment.Exit(0);
                        try
                        {
                            System.Windows.Application.Current.MainWindow.Close();
                        }
                        catch
                        {
                            System.Windows.Application.Current.Shutdown();
                        }
                        break;
                    case "imageTopping":
                        if (this.Topmost)
                        {
                            this.Topmost = false;
                            imageTopping.Source = new BitmapImage(new Uri(@"./Assets/Images/钉子2.png", UriKind.RelativeOrAbsolute));
                        }
                        else
                        {
                            this.Topmost = true;
                            imageTopping.Source = new BitmapImage(new Uri(@"./Assets/Images/钉子3.png", UriKind.RelativeOrAbsolute));

                        }
                        break;


                }
            }
        }


        #region 托盘


        private void InitialTray()
        {

            //设置托盘的各个属性
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.BalloonTipText = "程序开始运行";
            notifyIcon.Text = "模拟表正在后台运行";
            notifyIcon.Icon = new System.Drawing.Icon(Directory.GetCurrentDirectory() + @"\Images\智能电表.ico");
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(2000);
            //notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);
            notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);
            //设置菜单项
            //System.Windows.Forms.MenuItem menu1 = new System.Windows.Forms.MenuItem("菜单项1");
            //System.Windows.Forms.MenuItem menu2 = new System.Windows.Forms.MenuItem("菜单项2");
            //System.Windows.Forms.MenuItem menu = new System.Windows.Forms.MenuItem("菜单", new System.Windows.Forms.MenuItem[] { menu1 , menu2 });

            //退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += new EventHandler(exit_Click);

            //关联托盘控件
            //System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { menu, exit };
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { exit };

            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            //窗体状态改变时候触发
            this.StateChanged += new EventHandler(SysTray_StateChanged);
        }

        private void SysTray_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// 退出选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exit_Click(object sender, EventArgs e)
        {
            //if (System.Windows.MessageBox.Show("确定要关闭吗?",
            //                                   "退出",
            //                                    MessageBoxButton.YesNo,
            //                                    MessageBoxImage.Question,
            //                                    MessageBoxResult.No) == MessageBoxResult.Yes)
            //{
            //notifyIcon.Dispose();
            //Environment.Exit(0);
            System.Windows.Application.Current.MainWindow.Close();
            //System.Windows.Application.Current.Shutdown();
            //}
        }
        /// <summary>
        /// 单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    this.Activate();
                }
            }
        }
        #endregion


        #region 数据监听
        [HandleProcessCorruptedStateExceptions]
        private void DataArrival(string Tstr)
        {
            int Ti; int Tj;
            string[] CmdStrData;
            if (Tstr == "")
                return;
            //ric_Comm.AppendText(Tstr);
            //ric_Comm.AppendText("\r\n");
            //AppData.LogViewModel.Log_PC += Tstr + "\r\n";               //AppData.LogViewModel.Log_PC += Tstr + "\r\n";   
            AppData.LogViewModel.Log_PC += 命令解析(Tstr) + "\r\n";

            try
            {
                switch (Tstr.Substring(0, 3))
                {
                    case "MOP":
                        try
                        {                            
                            SocketUdpControl.isStartMac =true;
                            //for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                            //{
                            //    AppData.BaseData.MeterObject[intInc].NoResponseMac = Tstr.Substring(9, 16);
                            //}
                        }
                        catch
                        {
                        }
                        break;
                    case "MCL":
                        try
                        {
                            SocketUdpControl.isStartMac = false;
                        }
                        catch
                        {
                        }
                        break;
                    case "MAC":
                        SocketUdpControl.Mac = Tstr.Remove(0,3);
                        
                        SocketUdpControl.Flag = true;
                        break;
                    case "NoResMac":
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                        {
                            AppData.BaseData.MeterObject[intInc].NoResponseMac = Tstr.Substring(9, 16);
                        }
                        break;
                    case "Bqd":
                        for (Ti = 0; Ti < AppData.BaseData.Num_Terminal; Ti++)
                        {
                            AppData.BaseData.TerminalObject[Ti].Open();
                        }
                        break;
                    case "Bgb":
                        for (Ti = 0; Ti < AppData.BaseData.Num_Terminal; Ti++)
                            AppData.BaseData.TerminalObject[Ti].Colse();
                        break;
                    case "Sav":
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                            AppData.BaseData.MeterObject[intInc].SaveData();
                        break;
                    case "Set": //设电流电压
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                        {
                            AppData.BaseData.MeterObject[intInc].Ua = double.Parse(Tstr.Substring(3 + 0, 5));
                            AppData.BaseData.MeterObject[intInc].Ub = double.Parse(Tstr.Substring(3 + 5, 5));
                            AppData.BaseData.MeterObject[intInc].Uc = double.Parse(Tstr.Substring(3 + 10, 5));
                            AppData.BaseData.MeterObject[intInc].Ia = double.Parse(Tstr.Substring(3 + 15, 5));
                            AppData.BaseData.MeterObject[intInc].Ib = double.Parse(Tstr.Substring(3 + 20, 5));
                            AppData.BaseData.MeterObject[intInc].Ic = double.Parse(Tstr.Substring(3 + 25, 5));
                            AppData.BaseData.MeterObject[intInc].Phia = double.Parse(Tstr.Substring(3 + 30, 5));
                            AppData.BaseData.MeterObject[intInc].Phib = double.Parse(Tstr.Substring(3 + 35, 5));
                            AppData.BaseData.MeterObject[intInc].Phic = double.Parse(Tstr.Substring(3 + 40, 5));
                        }
                        break;

                    case "DLS"://'设电量
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                        {
                            AppData.BaseData.MeterObject[intInc].Pq[0, 0] = double.Parse(Tstr.Substring(3 + 0, 5));
                            for (Ti = 1; Ti <= 4; Ti++)
                                AppData.BaseData.MeterObject[intInc].Pq[0, Ti] = double.Parse(Tstr.Substring(3 + 0, 5)) / 4;

                            AppData.BaseData.MeterObject[intInc].Pq[1, 0] = double.Parse(Tstr.Substring(3 + 5, 5));
                            for (Ti = 1; Ti <= 4; Ti++)
                                AppData.BaseData.MeterObject[intInc].Pq[1, Ti] = double.Parse(Tstr.Substring(3 + 5, 5)) / 4;

                            AppData.BaseData.MeterObject[intInc].Pq[4, 0] = double.Parse(Tstr.Substring(3 + 20, 5));
                            for (Ti = 1; Ti <= 4; Ti++)
                                AppData.BaseData.MeterObject[intInc].Pq[4, Ti] = double.Parse(Tstr.Substring(3 + 20, 5)) / 4;

                            AppData.BaseData.MeterObject[intInc].Pq[5, 0] = double.Parse(Tstr.Substring(3 + 25, 5));
                            for (Ti = 1; Ti <= 4; Ti++)
                                AppData.BaseData.MeterObject[intInc].Pq[5, Ti] = double.Parse(Tstr.Substring(3 + 25, 5)) / 4;

                            AppData.BaseData.MeterObject[intInc].Pq[6, 0] = double.Parse(Tstr.Substring(3 + 30, 5));
                            for (Ti = 1; Ti <= 4; Ti++)
                                AppData.BaseData.MeterObject[intInc].Pq[6, Ti] = double.Parse(Tstr.Substring(3 + 30, 5)) / 4;

                            AppData.BaseData.MeterObject[intInc].Pq[7, 0] = double.Parse(Tstr.Substring(3 + 35, 5));
                            for (Ti = 1; Ti <= 4; Ti++)
                                AppData.BaseData.MeterObject[intInc].Pq[7, Ti] = double.Parse(Tstr.Substring(3 + 35, 5)) / 4;


                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 5; j++)
                                {
                                    AppData.BaseData.MeterObject[intInc].PqDj[i, j] = AppData.BaseData.MeterObject[intInc].Pq[i, j];
                                    AppData.BaseData.MeterObject[intInc].ZdxlDj[i, j] = AppData.BaseData.MeterObject[intInc].Zdxl[i, j];
                                }
                            }
                        }
                        break;

                    case "Dom": //设电表下降系数
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                            for (Tj = 0; Tj <= 7; Tj++)
                                AppData.BaseData.MeterObject[intInc].Pq[0, Tj] = AppData.BaseData.MeterObject[intInc].Pq[0, Tj] * double.Parse(Tstr.Substring(3 + 0, 5));
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].电能系数 = Tstr.Substring(3 + 0, 5);
                        }
                        break;
                    case "SQT"://
                        CmdStrData = Tstr.Substring(3).Split(',');
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                        {
                            AppData.BaseData.MeterObject[intInc].LastBCtime = DateTime.Parse(CmdStrData[0]);
                            AppData.BaseData.MeterObject[intInc].LastQLtime = DateTime.Parse(CmdStrData[1]);
                            AppData.BaseData.MeterObject[intInc].QLCS = int.Parse(CmdStrData[2]);
                            AppData.BaseData.MeterObject[intInc].BCCS = int.Parse(CmdStrData[3]);
                            AppData.BaseData.MeterObject[intInc].DCWorkTime = int.Parse(CmdStrData[4]);
                            AppData.BaseData.MeterObject[intInc].DXCS = int.Parse(CmdStrData[5]);
                            AppData.BaseData.MeterObject[intInc].DcZhenChang = int.Parse(CmdStrData[6]);
                        }
                        break;
                    case "BCT":// '编程时间
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].最近一次编程时间 = true;
                        }
                        ChangeParameter(1);
                        break;
                    case "QLT":// '需量清零时间
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].最大需量清零时间 = true;
                        }
                        ChangeParameter(1);
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                AppData.BaseData.MeterObject[intInc].Zdxl[i, 0] = 0;
                                AppData.BaseData.MeterObject[intInc].Zdxl[i, 1] = 0;
                                AppData.BaseData.MeterObject[intInc].Zdxl[i, 2] = 0;
                                AppData.BaseData.MeterObject[intInc].Zdxl[i, 3] = 0;
                                AppData.BaseData.MeterObject[intInc].Zdxl[i, 4] = 0;
                            }
                        }
                        break;
                    case "BCS":// '编程次数
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].编程次数 = true;
                        }
                        ChangeParameter(1);
                        break;
                    case "MCS"://磁场异常次数
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                            AppData.BaseData.MeterObject[intInc].Mfunusual++;
                        break;
                    case "MCO"://开表盖总次数
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                            AppData.BaseData.MeterObject[intInc].MCoverOpen++;
                        break;
                    case "MHO"://开端钮盒总次数
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                            AppData.BaseData.MeterObject[intInc].MHeOpen++;
                        break;
                    case "XSC"://校时总总数
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                            AppData.BaseData.MeterObject[intInc].XSNum++;
                        break;
                    case "QLS":// '需量清零次数
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].清零次数 = true;
                        }
                        ChangeParameter(1);
                        break;
                    case "DXS": //断相次数
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].断相次数 = true;
                        }
                        ChangeParameter(1);
                        break;
                    case "SYS"://失压次数
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                        {
                            AppData.BaseData.MeterObject[intInc].SYCS++;
                            AppData.BaseData.MeterObject[intInc].SYFSTime = DateTime.Now.ToString("ssmmHHddMMyy");
                            AppData.BaseData.MeterObject[intInc].SYJSTime = DateTime.Now.AddMinutes(1).ToString("ssmmHHddMMyy");
                        }
                        break;
                    case "DCY":// '电池正常
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                            AppData.BaseData.MeterObject[intInc].DcZhenChang = 1;
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].电池异常 = false;
                        }
                        //ChanGe();
                        break;
                    case "DCN":// '电池No正常
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                            AppData.BaseData.MeterObject[intInc].DcZhenChang = 0;
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].电池异常 = true;
                        }
                        //ChanGe();
                        break;
                    case "CsC":// '设常数
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].常数 = true;
                        }
                        ChangeParameter(1);
                        break;
                    case "Xis":// '运行系数
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                            AppData.BaseData.MeterObject[intInc].zzXiShu = double.Parse(Tstr.Substring(3 + 0, 5));
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].走字系数 = Tstr.Substring(3 + 0, 5);
                        }
                        break;
                    case "DnX"://电能系数
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                            AppData.BaseData.MeterObject[intInc].DnXiShu = double.Parse(Tstr.Substring(3 + 0, 5));
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].电能系数 = Tstr.Substring(3 + 0, 5);
                        }
                        break;
                    case "Tim":// '时间差
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                            AppData.BaseData.MeterObject[intInc].TimeLoss = double.Parse(Tstr.Substring(3));
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].时间慢系统分 = Tstr.Substring(3);
                        }
                        break;

                    case "Dat":// '日期差
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                            AppData.BaseData.MeterObject[intInc].DateLoss = double.Parse(Tstr.Substring(3));
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].日期慢系统天 = Tstr.Substring(3);
                        }
                        break;

                    case "Aut": //自动抄表日
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].自动抄表日 = true;
                        }
                        ChangeParameter(1);
                        break;
                    case "SDS": //'时段
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].时段 = true;
                        }
                        ChangeParameter(1);
                        break;
                    case "GYT":            //WJY2009-11
                        //TODO 这个是修改协议
                        AppData.BaseData.SelectProtocol = int.Parse(Tstr.Substring(3));
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                            AppData.BaseData.MeterObject[intInc].BiaoType = AppData.BaseData.SelectProtocol;
                        break;
                    case "STS":
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].电表运行状态字 = true;
                        }
                        ChangeParameter(1);
                        break;
                    case "STC"://单个状态量变位
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                        {
                            AppData.BaseData.MeterObject[intInc].B_ZT[3] = Tstr.Substring(3, 4);
                            AppData.BaseData.MeterObject[intInc].B_ZT[4] = Tstr.Substring(3, 4);
                            AppData.BaseData.MeterObject[intInc].B_ZT[5] = Tstr.Substring(3, 4);
                        }
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].状态字4 = AppData.BaseData.MeterObject[1].B_ZT[3];
                            AppData.BaseData.设置[i].状态字5 = AppData.BaseData.MeterObject[1].B_ZT[4];
                            AppData.BaseData.设置[i].状态字6 = AppData.BaseData.MeterObject[1].B_ZT[5];
                        }

                        break;
                    case "SZT":            //WJY2009-12
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                        {
                            for (Ti = 0; Ti < 7; Ti++)
                                AppData.BaseData.MeterObject[intInc].B_ZT[Ti] = Tstr.Substring(Ti * 4 + 3, 4);
                        }
                        for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                        {
                            AppData.BaseData.设置[i].状态字1 = AppData.BaseData.MeterObject[1].B_ZT[0];
                            AppData.BaseData.设置[i].状态字2 = AppData.BaseData.MeterObject[1].B_ZT[1];
                            AppData.BaseData.设置[i].状态字3 = AppData.BaseData.MeterObject[1].B_ZT[2];
                            AppData.BaseData.设置[i].状态字4 = AppData.BaseData.MeterObject[1].B_ZT[3];
                            AppData.BaseData.设置[i].状态字5 = AppData.BaseData.MeterObject[1].B_ZT[4];
                            AppData.BaseData.设置[i].状态字6 = AppData.BaseData.MeterObject[1].B_ZT[5];
                            AppData.BaseData.设置[i].状态字7 = AppData.BaseData.MeterObject[1].B_ZT[6];
                        }

                        break;
                    case "CLS":
                        break;
                    case "BTL":
                        for (Ti = 0; Ti < AppData.BaseData.Num_Terminal; Ti++)
                        {
                            AppData.BaseData.TerminalObject[Ti].G_BTL = Tstr.Substring(3) + "-e-8-1";
                            AppData.BaseData.TerminalObject[Ti].Open();
                        }
                        break;

                    case "DDJ":
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 5; j++)
                                {
                                    AppData.BaseData.MeterObject[intInc].PqDj[i, j] = AppData.BaseData.MeterObject[intInc].Pq[i, j];
                                }
                            }
                        }
                        break;
                    case "TDC":
                        //string strtdc = Tstr.Substring(3);
                        //AppData.BaseData.MeterObject[intInc].intDdcs++;
                        //AppData.BaseData.MeterObject[intInc].strtdsj = strtdc.Substring(1, 12);
                        //AppData.BaseData.MeterObject[intInc].strsdsj = strtdc.Substring(13, 12);
                        break;
                    case "BAR":
                        string TBaddr = Tstr.Substring(3);
                        string[] tmpAddress = TBaddr.Split(',');
                        for (Ti = 0; Ti < AppData.BaseData.MeterObject.Count; Ti++)
                        {
                            AppData.BaseData.MeterObject[Ti].BAddress = tmpAddress[Ti - 1];
                        }
                        break;
                    case "Log":
                        string strLog = Tstr.Substring(3);
                        for (int i = 0; i < AppData.BaseData.TerminalObject.Count; i++)
                        {
                            string strFileName = "MeterLog" + (i + 1).ToString();
                            string strLogMessage = Tstr;
                            LogInfoHelper.WriteLog(strFileName, strLogMessage);
                            //CLBase.WriteLog("MeterLog" + (i + 1).ToString(), Tstr);
                        }
                        break;
                    case "SCu": //设置电流
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                        {
                            AppData.BaseData.MeterObject[intInc].Ia = double.Parse(Tstr.Substring(3 + 0, 5));
                            AppData.BaseData.MeterObject[intInc].Ib = double.Parse(Tstr.Substring(3 + 5, 5));
                            AppData.BaseData.MeterObject[intInc].Ic = double.Parse(Tstr.Substring(3 + 10, 5));
                        }
                        break;
                    case "SVo": //设置电压
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                        {
                            AppData.BaseData.MeterObject[intInc].Ua = double.Parse(Tstr.Substring(3 + 0, 5));
                            AppData.BaseData.MeterObject[intInc].Ub = double.Parse(Tstr.Substring(3 + 5, 5));
                            AppData.BaseData.MeterObject[intInc].Uc = double.Parse(Tstr.Substring(3 + 10, 5));
                        }
                        break;
                    case "SPh": //设置相角
                        for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                        {
                            AppData.BaseData.MeterObject[intInc].Phia = double.Parse(Tstr.Substring(3 + 0, 5));
                            AppData.BaseData.MeterObject[intInc].Phib = double.Parse(Tstr.Substring(3 + 5, 5));
                            AppData.BaseData.MeterObject[intInc].Phic = double.Parse(Tstr.Substring(3 + 10, 5));
                        }
                        break;
                    case "Bdz":
                        SetMeterAddress(Tstr);
                        //GetMeterAddr(cob_SetDL.SelectedIndex + 1);
                        break;
                    case "Cmd":
                        CmdStrData = Tstr.Split(',');
                        switch (CmdStrData[1])
                        {
                            case "03050000":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                    AppData.BaseData.MeterObject[intInc].QSYCS = int.Parse(CmdStrData[2]);
                                break;
                            case "03110000":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].intDdcs = int.Parse(CmdStrData[2]);
                                    AppData.BaseData.MeterObject[intInc].strtdsj[0] = (CmdStrData[3]);
                                    AppData.BaseData.MeterObject[intInc].strsdsj[0] = (CmdStrData[4]);
                                }
                                break;
                            case "03110001":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].strtdsj[0] = (CmdStrData[2]);
                                    AppData.BaseData.MeterObject[intInc].strsdsj[0] = (CmdStrData[3]);
                                }
                                break;
                            case "03110002":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].strtdsj[1] = (CmdStrData[2]);
                                    AppData.BaseData.MeterObject[intInc].strsdsj[1] = (CmdStrData[3]);
                                }
                                break;
                            case "03110003":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].strtdsj[2] = (CmdStrData[2]);
                                    AppData.BaseData.MeterObject[intInc].strsdsj[2] = (CmdStrData[3]);
                                }
                                break;
                            case "03110004":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].strtdsj[3] = (CmdStrData[2]);
                                    AppData.BaseData.MeterObject[intInc].strsdsj[3] = (CmdStrData[3]);
                                }
                                break;
                            case "03110005":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].strtdsj[4] = (CmdStrData[2]);
                                    AppData.BaseData.MeterObject[intInc].strsdsj[4] = (CmdStrData[3]);
                                }
                                break;
                            case "03110006":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].strtdsj[5] = (CmdStrData[2]);
                                    AppData.BaseData.MeterObject[intInc].strsdsj[5] = (CmdStrData[3]);
                                }
                                break;
                            case "03110007":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].strtdsj[6] = (CmdStrData[2]);
                                    AppData.BaseData.MeterObject[intInc].strsdsj[6] = (CmdStrData[3]);
                                }
                                break;
                            case "03110008":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].strtdsj[7] = (CmdStrData[2]);
                                    AppData.BaseData.MeterObject[intInc].strsdsj[7] = (CmdStrData[3]);
                                }
                                break;
                            case "03110009":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].strtdsj[8] = (CmdStrData[2]);
                                    AppData.BaseData.MeterObject[intInc].strsdsj[8] = (CmdStrData[3]);
                                }
                                break;
                            case "0311000A":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].strtdsj[9] = (CmdStrData[2]);
                                    AppData.BaseData.MeterObject[intInc].strsdsj[9] = (CmdStrData[3]);
                                }
                                break;
                            case "03300E00":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                    AppData.BaseData.MeterObject[intInc].MHeOpen = int.Parse(CmdStrData[2]);
                                break;
                            case "03300E01":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                    AppData.BaseData.MeterObject[intInc].MHeOpenEvent = CmdStrData[2];
                                break;
                            case "03300D00":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                    AppData.BaseData.MeterObject[intInc].MCoverOpen = int.Parse(CmdStrData[2]);
                                break;
                            case "03300D01":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                    AppData.BaseData.MeterObject[intInc].MCoverOpennEvent = CmdStrData[2];
                                break;
                            case "1A010001":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                    AppData.BaseData.MeterObject[intInc].CutOffCurCount_A = int.Parse(CmdStrData[2]);
                                break;
                            case "04000306":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                    AppData.BaseData.MeterObject[intInc].dlhgq = CmdStrData[2];
                                break;
                            case "04000307":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                    AppData.BaseData.MeterObject[intInc].dyhgq = CmdStrData[2];
                                break;
                            case "19010001":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                    AppData.BaseData.MeterObject[intInc].Cmd19010001 = int.Parse(CmdStrData[2]);
                                break;
                            case "19010101":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                    AppData.BaseData.MeterObject[intInc].Cmd19010101 = CmdStrData[2];
                                break;
                            case "19012101":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                    AppData.BaseData.MeterObject[intInc].Cmd19012101 = CmdStrData[2];
                                break;
                            case "Set":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].Ua = double.Parse(CmdStrData[2]);
                                    AppData.BaseData.MeterObject[intInc].Ub = double.Parse(CmdStrData[3]);
                                    AppData.BaseData.MeterObject[intInc].Uc = double.Parse(CmdStrData[4]);
                                    AppData.BaseData.MeterObject[intInc].Ia = double.Parse(CmdStrData[5]);
                                    AppData.BaseData.MeterObject[intInc].Ib = double.Parse(CmdStrData[6]);
                                    AppData.BaseData.MeterObject[intInc].Ic = double.Parse(CmdStrData[7]);
                                    AppData.BaseData.MeterObject[intInc].Phia = double.Parse(CmdStrData[8]);
                                    AppData.BaseData.MeterObject[intInc].Phib = double.Parse(CmdStrData[9]);
                                    AppData.BaseData.MeterObject[intInc].Phic = double.Parse(CmdStrData[10]);
                                    AppData.BaseData.MeterObject[intInc].DnXiShu = double.Parse(CmdStrData[11]);
                                    AppData.BaseData.MeterObject[intInc].zzXiShu = double.Parse(CmdStrData[12]);
                                }
                                break;
                            case "Set2":
                                try
                                {
                                    for (Ti = Convert.ToInt32(CmdStrData[13]); Ti < Convert.ToInt32(CmdStrData[14]); Ti++)
                                    {
                                        AppData.BaseData.TerminalObject[Ti].boolIsReturn = false;
                                    }
                                    for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                    {
                                        AppData.BaseData.MeterObject[intInc].Ua = double.Parse(CmdStrData[2]);
                                        AppData.BaseData.MeterObject[intInc].Ub = double.Parse(CmdStrData[3]);
                                        AppData.BaseData.MeterObject[intInc].Uc = double.Parse(CmdStrData[4]);
                                        AppData.BaseData.MeterObject[intInc].Ia = double.Parse(CmdStrData[5]);
                                        AppData.BaseData.MeterObject[intInc].Ib = double.Parse(CmdStrData[6]);
                                        AppData.BaseData.MeterObject[intInc].Ic = double.Parse(CmdStrData[7]);
                                        AppData.BaseData.MeterObject[intInc].Phia = double.Parse(CmdStrData[8]);
                                        AppData.BaseData.MeterObject[intInc].Phib = double.Parse(CmdStrData[9]);
                                        AppData.BaseData.MeterObject[intInc].Phic = double.Parse(CmdStrData[10]);
                                        AppData.BaseData.MeterObject[intInc].DnXiShu = double.Parse(CmdStrData[11]);
                                        AppData.BaseData.MeterObject[intInc].zzXiShu = double.Parse(CmdStrData[12]);
                                    }
                                }
                                catch
                                {

                                }
                                break;
                            case "SetEventHappen":

                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].dtEventHappen = Convert.ToDateTime(CmdStrData[2]);
                                    AppData.BaseData.MeterObject[intInc].dtEventEnd = Convert.ToDateTime(CmdStrData[2]).AddMinutes(5);
                                }
                                break;
                            case "Connect":
                                try
                                {

                                    int ret = 0;

                                    //if (getValue("加密机连接模式") == "直连密码机版")
                                    //{
                                    //    ret = FactoryEncryptionFunction698.ConnectDevice(getValue("加密机IP"), getValue("加密机端口"), "5");

                                    //}
                                    //else
                                    //{
                                    //    ret = EncryptionFunction698.ConnectDevice(getValue("加密机IP"), getValue("加密机端口"), "5");
                                    //}

                                    //if (ret == 0)
                                    //{
                                    //    connectTime = DateTime.Now;
                                    //}
                                    for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                    {
                                        {
                                            AppData.BaseData.MeterObject[intInc].isErrMac = false;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogInfoHelper.WriteLog("异常日志", ex.Message + "\r\n" + ex.ToString() + "\r\n" + getValue("加密机连接模式") + "--" + getValue("加密机IP") + "--" + getValue("加密机端口"));

                                    //System.Windows.Forms.MessageBox.Show(ex.Message+"\r\n"+ ex.ToString() + "\r\n"+ getValue("加密机连接模式")+"--"+ getValue("加密机IP")+"--" + getValue("加密机端口"));
                                }

                                break;
                            case "ErrMac":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].isErrMac = true;
                                }
                                break;
                            case "DLS"://'设电量
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].Pq[0, 0] = double.Parse(CmdStrData[2]);
                                    for (Ti = 1; Ti <= 4; Ti++)
                                        AppData.BaseData.MeterObject[intInc].Pq[0, Ti] = double.Parse(CmdStrData[2]) / 4;
                                    AppData.BaseData.MeterObject[intInc].Pq[1, 0] = double.Parse(CmdStrData[3]);
                                    for (Ti = 1; Ti <= 4; Ti++)
                                        AppData.BaseData.MeterObject[intInc].Pq[1, Ti] = double.Parse(CmdStrData[3]) / 4;
                                    AppData.BaseData.MeterObject[intInc].Pq[4, 0] = double.Parse(CmdStrData[4]);
                                    for (Ti = 1; Ti <= 4; Ti++)
                                        AppData.BaseData.MeterObject[intInc].Pq[4, Ti] = double.Parse(CmdStrData[4]) / 4;
                                    AppData.BaseData.MeterObject[intInc].Pq[5, 0] = double.Parse(CmdStrData[5]);
                                    for (Ti = 1; Ti <= 4; Ti++)
                                        AppData.BaseData.MeterObject[intInc].Pq[5, Ti] = double.Parse(CmdStrData[5]) / 4;
                                    AppData.BaseData.MeterObject[intInc].Pq[6, 0] = double.Parse(CmdStrData[6]);
                                    for (Ti = 1; Ti <= 4; Ti++)
                                        AppData.BaseData.MeterObject[intInc].Pq[6, Ti] = double.Parse(CmdStrData[6]) / 4;
                                    AppData.BaseData.MeterObject[intInc].Pq[7, 0] = double.Parse(CmdStrData[7]);
                                    for (Ti = 1; Ti <= 4; Ti++)
                                        AppData.BaseData.MeterObject[intInc].Pq[7, Ti] = double.Parse(CmdStrData[7]) / 4;
                                    for (int i = 0; i < 8; i++)
                                    {
                                        for (int j = 0; j < 5; j++)
                                        {
                                            AppData.BaseData.MeterObject[intInc].PqDj[i, j] = AppData.BaseData.MeterObject[intInc].Pq[i, j];
                                            AppData.BaseData.MeterObject[intInc].ZdxlDj[i, j] = AppData.BaseData.MeterObject[intInc].Zdxl[i, j];
                                        }
                                    }
                                }

                                break;
                            case "DLS2"://'设电量
                                try
                                {
                                    for (Ti = Convert.ToInt32(CmdStrData[8]); Ti < Convert.ToInt32(CmdStrData[9]); Ti++)
                                    {
                                        AppData.BaseData.TerminalObject[Ti].boolIsReturn = false;
                                    }

                                    for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                    {
                                        AppData.BaseData.MeterObject[intInc].Pq[0, 0] = double.Parse(CmdStrData[2]);
                                        for (Ti = 1; Ti <= 4; Ti++)
                                            AppData.BaseData.MeterObject[intInc].Pq[0, Ti] = double.Parse(CmdStrData[2]) / 4;
                                        AppData.BaseData.MeterObject[intInc].Pq[1, 0] = double.Parse(CmdStrData[3]);
                                        for (Ti = 1; Ti <= 4; Ti++)
                                            AppData.BaseData.MeterObject[intInc].Pq[1, Ti] = double.Parse(CmdStrData[3]) / 4;
                                        AppData.BaseData.MeterObject[intInc].Pq[4, 0] = double.Parse(CmdStrData[4]);
                                        for (Ti = 1; Ti <= 4; Ti++)
                                            AppData.BaseData.MeterObject[intInc].Pq[4, Ti] = double.Parse(CmdStrData[4]) / 4;
                                        AppData.BaseData.MeterObject[intInc].Pq[5, 0] = double.Parse(CmdStrData[5]);
                                        for (Ti = 1; Ti <= 4; Ti++)
                                            AppData.BaseData.MeterObject[intInc].Pq[5, Ti] = double.Parse(CmdStrData[5]) / 4;
                                        AppData.BaseData.MeterObject[intInc].Pq[6, 0] = double.Parse(CmdStrData[6]);
                                        for (Ti = 1; Ti <= 4; Ti++)
                                            AppData.BaseData.MeterObject[intInc].Pq[6, Ti] = double.Parse(CmdStrData[6]) / 4;
                                        AppData.BaseData.MeterObject[intInc].Pq[7, 0] = double.Parse(CmdStrData[7]);
                                        for (Ti = 1; Ti <= 4; Ti++)
                                            AppData.BaseData.MeterObject[intInc].Pq[7, Ti] = double.Parse(CmdStrData[7]) / 4;
                                        for (int i = 0; i < 8; i++)
                                        {
                                            for (int j = 0; j < 5; j++)
                                            {
                                                AppData.BaseData.MeterObject[intInc].PqDj[i, j] = AppData.BaseData.MeterObject[intInc].Pq[i, j];
                                                AppData.BaseData.MeterObject[intInc].ZdxlDj[i, j] = AppData.BaseData.MeterObject[intInc].Zdxl[i, j];
                                            }
                                        }
                                    }
                                }
                                catch
                                { }
                                break;
                            case "ZDXLTIME":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        for (int j = 0; j < 5; j++)
                                        {
                                            AppData.BaseData.MeterObject[intInc].ZdxlTime[i, j] = DateTime.Now;
                                            AppData.BaseData.MeterObject[intInc].Zdxl[i, j] = 0f;
                                        }
                                    }
                                }
                                break;
                            case "SZT":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    for (Ti = 0; Ti < 7; Ti++)
                                        AppData.BaseData.MeterObject[intInc].B_ZT[Ti] = CmdStrData[2 + Ti];
                                }
                                for (int i = 0; i < AppData.BaseData.设置.Count; i++)
                                {
                                    AppData.BaseData.设置[i].状态字1 = AppData.BaseData.MeterObject[1].B_ZT[0];
                                    AppData.BaseData.设置[i].状态字2 = AppData.BaseData.MeterObject[1].B_ZT[1];
                                    AppData.BaseData.设置[i].状态字3 = AppData.BaseData.MeterObject[1].B_ZT[2];
                                    AppData.BaseData.设置[i].状态字4 = AppData.BaseData.MeterObject[1].B_ZT[3];
                                    AppData.BaseData.设置[i].状态字5 = AppData.BaseData.MeterObject[1].B_ZT[4];
                                    AppData.BaseData.设置[i].状态字6 = AppData.BaseData.MeterObject[1].B_ZT[5];
                                    AppData.BaseData.设置[i].状态字7 = AppData.BaseData.MeterObject[1].B_ZT[6];
                                }
                                break;
                            case "RunFlag":
                                for (Ti = 0; Ti < AppData.BaseData.TerminalObject.Count; Ti++)
                                {
                                    if (CmdStrData[2] == "0")
                                        AppData.BaseData.TerminalObject[Ti].RunFalg = false;
                                    else
                                        AppData.BaseData.TerminalObject[Ti].RunFalg = true;
                                }
                                break;
                            case "MeterTimeCheck":
                                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                                {
                                    AppData.BaseData.MeterObject[intInc].bol_MeterDateTimeCheck = Convert.ToBoolean(int.Parse(CmdStrData[2]));
                                    AppData.BaseData.MeterObject[intInc].MeterDateTime = Convert.ToDateTime("2016-1-11 12:00:00");
                                }
                                break;
                            case "ProtocalType"://协议类型,0为97，1为07，2为698,3为水表，4为气表，5为热表
                                for (Ti = 0; Ti < AppData.BaseData.TerminalObject.Count; Ti++)
                                {
                                    AppData.BaseData.TerminalObject[Ti].BiaoType = int.Parse(CmdStrData[2]);
                                }
                                break;
                            case "YGZTZ":
                                AppData.BaseData.MeterObject[1].YouGong_ZT = CmdStrData[2];
                                break;
                            case "boolIsReturn":
                                for (Ti = 0; Ti < AppData.BaseData.TerminalObject.Count; Ti++)
                                {
                                    AppData.BaseData.TerminalObject[Ti].boolIsReturn = int.Parse(CmdStrData[2]) == 1 ? true : false;
                                }
                                break;
                            case "intesamtype":
                                try
                                {
                                    for (Ti = 0; Ti < AppData.BaseData.TerminalObject.Count; Ti++)
                                    {
                                        AppData.BaseData.TerminalObject[Ti].intesamtype = int.Parse(CmdStrData[Ti + 2]);
                                    }
                                }
                                catch
                                {

                                }
                                break;
                            case "ActiveRepot":
                                try
                                {

                                }
                                catch (Exception)
                                {

                                    throw;
                                }
                                break;
                        }
                        break;
                    case "End":
                        try
                        {
                            System.Windows.Application.Current.MainWindow.Close();
                        }
                        catch
                        {
                            System.Windows.Application.Current.Shutdown();
                        }
                        break;
                }
            }

            catch (Exception ex)
            {
                ; //MessageBox.Show(ex.ToString());
                AppData.LogViewModel.Log_PC += "数据异常：" + ex.ToString() + "\r\n";
            }
        }



        private void ChangeParameter(int iTmp)
        {
            int index = iTmp - 1;
            if (AppData.BaseData.设置[index].最近一次编程时间 == true)
            {
                AppData.BaseData.MeterObject[index].LastBCtime = DateAndTime.Now;
                AppData.BaseData.MeterObject[index].BCCS = (AppData.BaseData.MeterObject[index].BCCS + 1) % 100;
            }
            if (AppData.BaseData.设置[index].最大需量清零时间 == true)
            {
                AppData.BaseData.MeterObject[index].LastQLtime = DateAndTime.Now;
                AppData.BaseData.MeterObject[index].QLCS = (AppData.BaseData.MeterObject[index].QLCS + 1) % 100;
            }

            if (AppData.BaseData.设置[index].电池工作时间 == true)
                AppData.BaseData.MeterObject[index].DCWorkTime = (AppData.BaseData.MeterObject[index].DCWorkTime + 1) % 100;
            if (AppData.BaseData.设置[index].断相次数 == true)
            {
                AppData.BaseData.MeterObject[index].DXCS = (AppData.BaseData.MeterObject[index].DXCS + 1) % 100;
                AppData.BaseData.MeterObject[index].DXFSTime = DateTime.Now.ToString("ssmmHHddMMyy");
                AppData.BaseData.MeterObject[index].DXJSTime = DateTime.Now.AddMinutes(1).ToString("ssmmHHddMMyy");
            }
            if (AppData.BaseData.设置[index].常数 == true)
            {
                if (AppData.BaseData.MeterObject[index].BiaoYgCS == 600)
                {
                    AppData.BaseData.MeterObject[index].BiaoYgCS = 800;
                    AppData.BaseData.MeterObject[index].BiaoWgCS = 800;
                }
                else
                {
                    AppData.BaseData.MeterObject[index].BiaoYgCS = 600;
                    AppData.BaseData.MeterObject[index].BiaoWgCS = 600;
                }
            }
            if (AppData.BaseData.设置[index].时段 == true)
            {
                if (AppData.BaseData.MeterObject[index].ShiDuan[0].Hour == 5)
                {
                    AppData.BaseData.MeterObject[index].LastSDtime = DateTime.Now;
                    AppData.BaseData.MeterObject[index].ShiDuan[0] = AppData.BaseData.MeterObject[index].ShiDuan[0].AddHours(1);
                    AppData.BaseData.MeterObject[index].SDBCCS++;
                }
                else
                {
                    AppData.BaseData.MeterObject[index].LastSDtime = DateTime.Now;
                    AppData.BaseData.MeterObject[index].ShiDuan[0] = AppData.BaseData.MeterObject[index].ShiDuan[0].AddHours(-1);
                    AppData.BaseData.MeterObject[index].SDBCCS++;
                }

            }
            if (AppData.BaseData.设置[index].自动抄表日 == true)
            {
                if (AppData.BaseData.MeterObject[index].AutoDate == "0001")
                    AppData.BaseData.MeterObject[index].AutoDate = "0002";
                else
                    AppData.BaseData.MeterObject[index].AutoDate = "0001";
            }
            if (AppData.BaseData.设置[index].清零次数 == true)
                AppData.BaseData.MeterObject[index].QLCS = (AppData.BaseData.MeterObject[index].QLCS + 1) % 100;
            if (AppData.BaseData.设置[index].编程次数 == true)
                AppData.BaseData.MeterObject[index].BCCS = (AppData.BaseData.MeterObject[index].BCCS + 1) % 100;

            if (AppData.BaseData.设置[index].电池异常 == true)
                AppData.BaseData.MeterObject[index].DcZhenChang = 0;
            else
                AppData.BaseData.MeterObject[index].DcZhenChang = 1;

            if (AppData.BaseData.设置[index].电表运行状态字 == true)
            {
                if (AppData.BaseData.设置[index].状态字7 == "0000")
                    AppData.BaseData.设置[index].状态字7 = "0001";
                else
                    AppData.BaseData.设置[index].状态字7 = "0000";
                AppData.BaseData.MeterObject[index].B_ZT[6] = AppData.BaseData.设置[index].状态字7;
            }
            //else
            //{
            //    txt_zt7.Text = "0000";
            //    dic_Meter[iTmp].B_ZT[0] = txt_zt7.Text;
            //}
        }

        public void SetMeterAddress(string strAddress)
        {
            try
            {
                //int intIndex = cob_SetDL.SelectedIndex + 1;
                //dic_Meter[intIndex].TimeLoss = Convert.ToSingle(txt_lossm.Text);
                //dic_Meter[intIndex].DateLoss = double.Parse(txt_lossd.Text);
                //dic_Meter[intIndex].zzXiShu = double.Parse(txt_yxxs.Text);
                //dic_Meter[intIndex].DnXiShu = double.Parse(txt_dnxs.Text);

                //string[] arrAddress = strAddress.Split(',');

                //string strZbAddress = "";

                //if (arrAddress.Length > 1)
                //    strZbAddress = arrAddress[2].PadLeft(12, '0');

                //dic_Meter[intIndex].BAddress = "";
                //int intLength = arrAddress.Length;
                //if (intLength > 1)
                //    intLength--;
                //for (int i = 1; i < intLength; i++)
                //{
                //    dic_Meter[intIndex].BAddress += arrAddress[i].PadLeft(12, '0') + " ";
                //}
                //dic_Meter[intIndex].BAddress = dic_Meter[intIndex].BAddress.Remove(dic_Meter[intIndex].BAddress.Length - 1, 1);
                //if (arrAddress.Length > 1)
                //    dic_Meter[1].BAddress += " " + strZbAddress;
            }
            catch
            {; }
        }
        #endregion

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                MaxForm();
            }
        }
        private bool IsFormMax;
        private void MaxForm()
        {
            if (IsFormMax)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
            IsFormMax = !IsFormMax;
        }


        private string 命令解析(string value)
        {
            string str = "";
            try
            {
                switch (value.Substring(0, 3))
                {
                    case "Bqd":
                        break;
                    case "Bgb":
                        break;
                    case "Sav":
                        break;
                    case "Set": //设电流电压
                        str = "设电流电压:";
                        break;
                    case "DLS"://'设电量
                        str = "设电量:";
                        break;
                    case "Dom": //设电表下降系数
                        str = "设电表下降系数:";
                        break;
                    case "SQT"://
                        break;
                    case "BCT":// '编程时间
                        str = "编程时间:";
                        break;
                    case "QLT":// '需量清零时间
                        str = "需量清零时间:";
                        break;
                    case "BCS":// '编程次数
                        str = "编程次数:";

                        break;
                    case "MCS"://磁场异常次数
                        str = "磁场异常次数:";

                        break;
                    case "MCO"://开表盖总次数
                        str = "开表盖总次数:";

                        break;
                    case "MHO"://开端钮盒总次数
                        str = "开端钮盒总次数:";

                        break;
                    case "XSC"://校时总总数
                        str = "校时总总数:";

                        break;
                    case "QLS":// '需量清零次数
                        str = "需量清零次数:";

                        break;
                    case "DXS": //断相次数
                        str = "断相次数:";

                        break;
                    case "SYS"://失压次数
                        str = "失压次数:";

                        break;
                    case "DCY":// '电池正常
                        str = "电池正常:";

                        break;
                    case "DCN":// '电池No正常
                        str = "电池不正常:";

                        break;
                    case "CsC":// '设常数
                        str = "设常数:";

                        break;
                    case "Xis":// '运行系数
                        str = "运行系数:";

                        break;
                    case "DnX"://电能系数
                        str = "电能系数:";

                        break;
                    case "Tim":// '时间差
                        str = "时间差:";

                        break;

                    case "Dat":// '日期差
                        str = "日期差:";

                        break;

                    case "Aut": //自动抄表日
                        str = "自动抄表日:";

                        break;
                    case "SDS": //'时段
                        str = "时段:";

                        break;
                    case "GYT":            //WJY2009-11

                        break;
                    case "STS":

                        break;
                    case "STC"://单个状态量变位
                        str = "单个状态量变位:";

                        break;
                    case "SZT":            //WJY2009-12

                        break;
                    case "CLS":
                        break;
                    case "BTL":

                        break;

                    case "DDJ":

                        break;
                    case "TDC":

                        break;
                    case "BAR":

                        break;
                    case "Log":

                        break;
                    case "SCu": //设置电流
                        str = "设置电流:";

                        break;
                    case "SVo": //设置电压
                        str = "设置电压:";

                        break;
                    case "SPh": //设置相角
                        str = "设置相角:";
                        break;
                    case "Bdz":
                        //GetMeterAddr(cob_SetDL.SelectedIndex + 1);
                        break;
                    case "Cmd":
                        str = "命令:";

                        break;
                    case "End":

                        break;
                }
            }

            catch
            {
                ; //MessageBox.Show(ex.ToString());

            }
            if (str != "")
            {
                str += value.Substring(3);
            }
            return str;

        }
    }
}
