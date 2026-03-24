using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatabaseInitialized
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TerminalEven.Terminal += TerminalEven_Terminal;
        }

        #region //初始化给

        private void InintData()
        {
            Regions chace = Regions.终端通用台;//10.173.014.005 9017  192.168.127.244
            Universal init;
            switch (chace)
            {
                case Regions.终端通用台:
                    init = new Universal();
                    break;
                default:
                    init = new Universal();
                    break;
            }
            init.OutMessage += Init_OutMessage;
            Task.Run(() =>
            {
                init.Execute();
                
            });
        }

        #endregion

        #region 日志部分
        private void TerminalEven_Terminal(string GetSetMsg)
        {
            //KEAIXINXINXFEITIANZHUANG!(>_<)!QaQ
            logShow(DateTime.Now.ToString()+":"+ GetSetMsg);
        }
      
        int LogIndex = 0;
        private void logShow(string str)
        {
            var log = str + "\n";
            if (LogIndex == 0)
            {
                log = log + "\n";
            }

            if (Thread.CurrentThread == Dispatcher.Thread)
            {
                if (LogIndex > 1000)
                {
                    InitLogRic.Document.Blocks.Clear();
                    InitLogRic.AppendText("日志数量达到1000条，清空日志");
                    LogIndex = 0;

                }

                LogIndex++;
                InitLogRic.AppendText(log);
                InitLogRic.ScrollToEnd();

            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    if (LogIndex > 1000)
                    {
                        InitLogRic.Document.Blocks.Clear();
                        InitLogRic.AppendText("日志数量达到1000条，清空日志");
                        LogIndex = 0;
                    }

                    LogIndex++;
                    InitLogRic.AppendText(log);
                    InitLogRic.ScrollToEnd();
                });
            }

        }

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InintData();
        }
        private void AppendText(string msg)
        {
            InitLogRic.Dispatcher.Invoke(() =>
            {
                InitLogRic.AppendText(msg);
            });
        }

        private void Init_OutMessage(object sender, string e)
        {
            AppendText(e);
            AppendText(Environment.NewLine);
        }
    }
}
