using LYZD.DAL.Config;
using LYZD.Utility.Log;
using LYZD.ViewModel;
using LYZD.ViewModel.CodeTree;
using LYZD.ViewModel.Log;
using LYZD.ViewModel.Time;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LYZD.WPF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private System.Threading.Mutex mutex;
        RegisterHelper Register = new RegisterHelper();
        public App()
        {
             CheckRegister();
            this.Startup += new StartupEventHandler(app_Startup);

        }
         private void app_Startup(object sender,StartupEventArgs e)
        {
            bool ret;
            mutex = new Mutex(true,"LYZD.WPF",out ret);
            if (!ret)
            {
                MessageBox.Show("程序已启动","",MessageBoxButton.OK,MessageBoxImage.Stop);
                Environment.Exit(0);
            }
        }

        public bool CheckRegister()
        {
               // RegistryKey retkey = Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("lzd").CreateSubKey("lzd.INI").CreateSubKey(textBox2.Text);
            RegistryKey retkey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey("lzd").CreateSubKey("lzd.INI");
            foreach (string strRNum in retkey.GetSubKeyNames())
            {
                if (strRNum == Register.MD5Decrypt())
                {
                    return true;
                }
            }
            MessageBox.Show("程序未授权", "", MessageBoxButton.OK, MessageBoxImage.Stop);
            Process[] myProcess = Process.GetProcessesByName("demonstration");
            if (myProcess.Length == 0)
            {
                //这里运行数据服务
                Process.Start(string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), "demonstration.exe"));
            }
            else
            {
                //myProcess[0].Start();
            }
            Environment.Exit(0);
            return false;
        }

        private DispatcherTimer timer = new DispatcherTimer();
        /// 程序启动前要执行的动作
        /// <summary>
        /// 程序启动前要执行的动作
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //InitializeDog();
            UiInterface.UiDispatcher = SynchronizationContext.Current;
            CodeTreeViewModel.Instance.InitializeTree();

            timer.Interval = new TimeSpan(1000);
            timer.Tick += Timer_Tick;
            timer.Start();

            #region 新版本--2021-08-19：修改目的加上终端表位的日志信息


            #region 初始化日志

            //LogManager.AddMessage(string.Format("更新检定项 {0} 的结论", EquipmentData.CheckResults.ResultCollection[EquipmentData.Controller.Index].Name), EnumLogSource.数据库存取日志);

            #endregion
            ConfigHelper.Instance.LoadAllConfig();

            LogViewModelS.Instance.Initialize();
            LogManager.LogMessageArrived += (sender, args) =>
            {
                if (sender is LogModel)
                {
                    LogViewModelS.Instance.AddLogModel(sender as LogModel);
                }
            };
            EquipmentData.LastCheckInfo.LoadLastCheckInfo();    //加载最后一次的信息


            LogFile = System.IO.Directory.GetCurrentDirectory() + string.Format(@"\Log\系统日志\{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));

            //EquipmentData.LastCheckInfo.InitializeDog();//连接加密机
            //UI线程未捕获异常处理事件
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            //非UI线程未捕获异常处理事件
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            base.OnStartup(e);
            #endregion



        }



        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true; //把 Handled 属性设为true，表示此异常已处理，程序可以继续运行，不会强制退出
                Log(DateTime.Now.ToString() + "：未处理的异常" + "\r\n" + e.Exception.ToString());
                //MessageBox.Show("捕获未处理异常:" + e.Exception.Message+"\r\n"+e.Exception.ToString());

            }
            catch (Exception ex)
            {
                //此时程序出现严重异常，将强制结束退出
                Log(DateTime.Now.ToString() + "：未处理的异常" + "\r\n" + ex.ToString());
                //MessageBox.Show("程序发生致命错误，将终止，请联系运营商！");
            }

        }
        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            //task线程内未处理捕获
            Log(DateTime.Now.ToString() + "：未处理的异常" + "\r\n" + e.ToString());
            //MessageBox.Show("捕获线程内未处理异常：" + e.Exception.Message + "\r\n" + e.Exception.ToString());
            e.SetObserved();//设置该异常已察觉（这样处理后就不会引起程序崩溃）
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
           Log(DateTime.Now.ToString() + "：未处理的异常");
           Log(e.ExceptionObject.ToString());
           Log(e.IsTerminating.ToString());
           ////Exception error = (Exception)e.ExceptionObject;
           //System.Windows.Forms.MessageBox.Show("检测到未处理异常\r\n请查看文件:" + LogFile+ "\r\n错误信息：" + e.ExceptionObject.ToString(), "错误", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

            StringBuilder sbEx = new StringBuilder();
            if (e.IsTerminating)
            {
                sbEx.Append("程序发生致命错误，将终止，请联系运营商！\n");
            }
            sbEx.Append("捕获未处理异常：");
            if (e.ExceptionObject is Exception)
            {
                sbEx.Append(((Exception)e.ExceptionObject).Message);
            }
            else
            {
                sbEx.Append(e.ExceptionObject);
            }
            MessageBox.Show("检测到未处理异常\r\n请查看文件:" + LogFile + "\r\n"+sbEx.ToString());

        }
        static string LogFile ;
        public static void Log(string message)
        {
            try{
                System.IO.File.AppendAllText(LogFile, message + "\r\n");
            }
            catch { }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeMonitor.Instance.Timer_Elapsed();
        }

        /// 界面初始化标记
        /// <summary>
        /// 界面初始化标记
        /// </summary>
        private bool flagInitialize = false;
        /// 程序可见后要执行的动作
        /// <summary>
        /// 程序可见后要执行的动作
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            if (!flagInitialize)
            {
                UiInterface.UiDispatcher = SynchronizationContext.Current;
                flagInitialize = true;
            }
            base.OnActivated(e);
        }

    }
}
