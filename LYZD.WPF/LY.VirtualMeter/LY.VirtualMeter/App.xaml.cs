using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LY.VirtualMeter
{ 
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static Dictionary<int, Core.CAN.CanCommuinication> DicSock = new Dictionary<int, Core.CAN.CanCommuinication>();
        public static Dictionary<uint, List<byte>> RevcieBuffs = new Dictionary<uint, List<byte>>();
        /// <summary>
        /// 程序启动前要执行的动作
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //UI线程未捕获异常处理事件
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            //非UI线程未捕获异常处理事件
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            base.OnStartup(e);
        }
        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true; //把 Handled 属性设为true，表示此异常已处理，程序可以继续运行，不会强制退出
                Log(DateTime.Now.ToString() + "：未处理的异常" + "\r\n" + e.Exception.ToString());
                MessageBox.Show("捕获未处理异常:" + e.Exception.Message + "\r\n" + e.Exception.ToString());

            }
            catch (Exception ex)
            {
                //此时程序出现严重异常，将强制结束退出
                Log(DateTime.Now.ToString() + "：未处理的异常" + "\r\n" + ex.ToString());
                MessageBox.Show("程序发生致命错误，将终止，请联系运营商！");
            }

        }
        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            //task线程内未处理捕获
            Log(DateTime.Now.ToString() + "：未处理的异常" + "\r\n" + e.ToString());
            MessageBox.Show("捕获线程内未处理异常：" + e.Exception.Message + "\r\n" + e.Exception.ToString());
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
            MessageBox.Show("检测到未处理异常\r\n请查看文件:" + LogFile + "\r\n" + sbEx.ToString());

        }
        static string LogFile = System.IO.Directory.GetCurrentDirectory() + "\\Log\\错误日志.txt";
        public static void Log(string message)
        {
            System.IO.File.AppendAllText(LogFile, message + Environment.NewLine);
        }
    }
}
