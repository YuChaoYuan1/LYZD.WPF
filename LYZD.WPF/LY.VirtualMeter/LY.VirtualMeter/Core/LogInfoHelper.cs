using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace LY.VirtualMeter.Core
{
    public class LogInfoHelper
    {
        private static bool stopFlag = false;
        /// <summary>
        /// 等待有数据要上传
        /// </summary>
        private static AutoResetEvent autoResetEvent = new AutoResetEvent(true);

        /// <summary>
        /// 要执行的动作队列
        /// </summary>
        private static Queue<Action<object>> queueAction = new Queue<Action<object>>();

        /// <summary>
        /// 初始化日志记录接口,在台体启动时调用
        /// </summary>
        public static void OpenService()
        {
            //执行上传数据队列中的方法
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                while (!stopFlag)
                {
                    while (queueAction.Count > 0)
                    {
                        try
                        {
                            //if(obj != null)
                            queueAction.Dequeue().Invoke(new object());
                        }
                        catch
                        {
                        }
                    }
                    autoResetEvent.WaitOne();
                }
            });
        }
        /// <summary>
        /// 关闭日志记录服务
        /// </summary>
        public static void CloseService()
        {
            stopFlag = true;
            autoResetEvent.Set();
        }
        /// <summary>
        /// 写错误日志文件(每小时一个文件)
        /// </summary>
        /// <param name="ex"></param>
        public static void Write(Exception ex)
        {
            try
            {
                Action<object> action = new Action<object>((obj) =>
                {
                    string LogPath = string.Format(@"ErrLog\{0}.txt", DateTime.Now.ToString("yyyy-MM-dd hh"));
                    LogPath = File.GetPhyPath(LogPath);
                    FileStream Fs = File.Create(LogPath);
                    if (Fs == null)
                    {
                        // MessageBox.Show(string.Format("日志文件{0}创建失败!", LogPath));
                        return;
                    }
                    Fs.Close();
                    Fs.Dispose();

                    string ErrTxt = string.Format(@"
                    {0}:{1}
                    {2}
                    {3}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), ex.Message, ex.StackTrace, ex.InnerException);

                    System.IO.File.AppendAllText(LogPath, ErrTxt);

                });
                queueAction.Enqueue(action);
                autoResetEvent.Set();
            }
            catch { }
        }


        /// <summary>
        /// 写日志文件(每天一个文件)
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteLog(string strFilePah, string strMessage)
        {
            try
            {
                Action<object> action = new Action<object>((obj) =>
                {

                    string LogPath = string.Format(@"Log\\虚拟表日志\{0}\{1}.txt", DateTime.Now.ToString("yyyy-MM-dd"), strFilePah);
                    LogPath = File.GetPhyPath(LogPath);
                    FileStream Fs = File.Create(LogPath);
                    if (Fs == null)
                    {
                        // MessageBox.Show(string.Format("日志文件{0}创建失败!", LogPath));
                        return;
                    }
                    Fs.Close();
                    Fs.Dispose();

                    string ErrTxt = string.Format(@"{0}:{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), strMessage);

                    System.IO.File.AppendAllText(LogPath, ErrTxt+"\r\n");

                });
                queueAction.Enqueue(action);
                autoResetEvent.Set();
            }
            catch { }
        }
        /// <summary>
        /// 写命令帧日志文件(每天一个文件)
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteFrameLog(string strFileName, string strMessage)
        {
            try
            {
                Action<object> action = new Action<object>((obj) =>
                {

                    string LogPath = string.Format(@"FrameLog\{0}\{1}.txt", DateTime.Now.ToString("yyyy-MM-dd"), strFileName);
                    LogPath = File.GetPhyPath(LogPath);
                    FileStream Fs = File.Create(LogPath);
                    if (Fs == null)
                    {
                        // MessageBox.Show(string.Format("日志文件{0}创建失败!", LogPath));
                        return;
                    }
                    Fs.Close();
                    Fs.Dispose();


                    System.IO.File.AppendAllText(LogPath, strMessage);

                });
                queueAction.Enqueue(action);
                autoResetEvent.Set();
            }
            catch { }
        }
    }
}
