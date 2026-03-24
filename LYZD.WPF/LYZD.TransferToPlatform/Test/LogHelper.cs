using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.Test
{
    /// <summary>
    /// 写本地日志。日志文件自动删除历史的，保留最近32个日志文件。
    /// </summary>
    public class LogHelpter
    {
        /// <summary>
        /// 最多记录32个日志文件，其他的删除历史文件
        /// </summary>
        const int MAX_FILE_COUNT = 200;

        /// <summary>
        /// 文件保存的目录
        /// </summary>
        static string fileSaveDir = "";

        /// <summary>
        /// 限制文件生成大小，258000字节,大约253kb
        /// </summary>
        const long fileBytes = 258000;

        //日志文件保存根路径   
        const string saveFolder = "Log\\前置机日志";

        private readonly static object lockObj = new object();

        static LogHelpter()
        {
            //删除超过指定数量的历史文件    
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        string fileFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, saveFolder);

                        if (Directory.Exists(fileFolderPath) == false)
                        {
                            Directory.CreateDirectory(fileFolderPath);
                        }

                        fileSaveDir = fileFolderPath;
                        //自动删除文件，
                        string[] filePathArr = System.IO.Directory.GetFileSystemEntries(fileSaveDir);

                        if (filePathArr == null || filePathArr.Length == 0)
                        {
                            goto DO_SELEEP;
                        }
                        List<FileInfo> files = new List<FileInfo>();
                        foreach (var item in filePathArr)
                        {
                            //Console.WriteLine("日志文件：" + item);
                            System.IO.FileInfo fileInfo = new System.IO.FileInfo(item);
                            files.Add(fileInfo);
                        }
                        var del_files = files.OrderByDescending(g => g.LastWriteTime).Skip(MAX_FILE_COUNT);
                        if (del_files == null || del_files.Count() == 0)
                        {
                            goto DO_SELEEP;
                        }
                        foreach (var file in del_files)
                        {
                            file.Delete();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("LogHelpter执行删除文件功能异常," + ex.Message);
                    }
                DO_SELEEP:
                    System.Threading.Thread.Sleep(20000);
                }
            });

        }

        /// <summary>
        /// fileFolderPath为文件保存目录
        /// </summary>
        /// <param name="fileFolderPath">文件保存目录</param>
        /// <param name="fileNameNoExtension"></param>
        /// <param name="fileNamePrefix">文件名前缀</param>
        /// <returns></returns>
        static string GetNewFileName(string fileFolderPath, string fileNameNoExtension, string fileNamePrefix = null)
        {
            string fileName = string.Empty;

            //文件名前缀
            fileNamePrefix = string.IsNullOrWhiteSpace(fileNamePrefix) ? "" : fileNamePrefix;
            int index = 1;
            if (fileNameNoExtension.LastIndexOf('_') == -1)
            {
                // fileName = Path.Combine(fileFolderPath, fileNamePrefix + DateTime.Now.ToString("yyyy-MM-dd") + "_" + index + ".log");
            }
            else
            {
                var arr = fileNameNoExtension.Split('_');
                int.TryParse(arr[arr.Length - 1], out index);
                if (index == 0 || index > 900)
                {
                    index = 1;
                }
            }
            fileName = Path.Combine(fileFolderPath, fileNamePrefix + DateTime.Now.ToString("yyyy-MM-dd") + "_" + index + ".log");
            while (File.Exists(fileName))
            {
                if (File.ReadAllBytes(fileName).Length < fileBytes)
                {
                    break;
                }
                index++;
                fileName = Path.Combine(fileFolderPath, fileNamePrefix + DateTime.Now.ToString("yyyy-MM-dd") + "_" + index + ".log");
            }
            return fileName;
        }

        /// <summary>  
        /// 日志记录，在程序执行的根目录，写入txt文件，文件固定大小，超过限定大小自动创建新日志文件
        /// </summary>  
        /// <param name="msg">记录内容</param>  
        /// <param name="storeDir">文件保存文件夹</param>  
        /// <param name="fileNamePrefix">文件名前缀_</param>  
        /// <returns></returns>  
        public static void AddLog(string msg, string storeDir = null, string fileNamePrefix = null)
        {
            //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + msg);
            lock (lockObj)
            {
                try
                {
                    //文件名前缀
                    fileNamePrefix = string.IsNullOrWhiteSpace(fileNamePrefix) ? "" : fileNamePrefix + "_";

                    string fileFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, saveFolder);
                    if (!string.IsNullOrWhiteSpace(storeDir))
                    {
                        fileFolderPath = Path.Combine(fileFolderPath, storeDir);
                    }
                    if (Directory.Exists(fileFolderPath) == false)
                    {
                        Directory.CreateDirectory(fileFolderPath);
                    }
                    fileSaveDir = fileFolderPath;
                    string fileName = string.Empty;

                    string[] files = Directory.GetFiles(fileFolderPath);
                    if (files.Length == 0)
                    {
                        fileName = Path.Combine(fileFolderPath, fileNamePrefix + DateTime.Now.ToString("yyyy-MM-dd") + "_1.log");
                        goto DO_WRITE;
                    }

                    string[] files2 = files.OrderByDescending(x =>
                    {
                        var regList = Regex.Matches(x, "_\\d*");
                        return regList.Cast<Match>().Last().Value.TrimStart('_');
                    }).ToArray();

                    FileInfo fileInfo = new FileInfo(files2[0]);
                    string fileNameNoExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    fileName = GetNewFileName(fileFolderPath, fileNameNoExtension, fileNamePrefix);

                DO_WRITE:
                    FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Write);
                    //string msg2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ">" + msg + System.Environment.NewLine;
                    string msg2 =  msg + System.Environment.NewLine;
                    byte[] logBytes = UTF8Encoding.UTF8.GetBytes(msg2);
                    fs.Write(logBytes, 0, logBytes.Length);
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                    //  tishiMsg = "写入日志成功"; 
                }
                catch (Exception ex)
                {
                    Console.WriteLine("LogHelpter日志写入异常：" + ex.ToString());
                }
            }
        }
    }
}

