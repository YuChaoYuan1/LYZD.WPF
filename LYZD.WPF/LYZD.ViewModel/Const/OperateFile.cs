using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.ViewModel.Const
{
    public class OperateFile
    {
        #region "读取写入INI"
        [DllImport("kernel32.dll")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32.dll")]
        //private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);

       public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 写入INI文件bai
        /// </summary>
        /// <param name=^Section^>节点名称</param>
        /// <param name=^Key^>关键字</param>
        /// <param name=^Value^>值</param>
        /// <param name=^filepath^>INI文件路径</param>
        public static void WriteINI(string Section, string Key, string Value, string filepath)
        {
            WritePrivateProfileString(Section, Key, Value, filepath);
        }

        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="Section">节点名称</param>
        /// <param name="Key">键</param>
        /// <param name="filepath">路径</param>
        /// <returns></returns>
        public static string GetINI(string Section, string Key, string filepath)
        {
            try
            {
                filepath = GetPhyPath(filepath);
                if (File.Exists(filepath) == false)
                {
                    File.Create(filepath).Close();
                }
                StringBuilder temp = new StringBuilder(255);
                int i = GetPrivateProfileString(Section, Key, "", temp, 255, filepath);
                return temp.ToString();

                //Byte[] Buffer = new Byte[65535];
                //int bufLen = GetPrivateProfileString(Section, Key, "", Buffer, Buffer.GetUpperBound(0), filepath);
                ////必须设定0（系统默认的代码页）的编码方式，否则无法支持中文
                //string s = Encoding.GetEncoding(0).GetString(Buffer);
                //s = s.Substring(0, bufLen);
                //return s.Trim();

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }



        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="Section">节点名称</param>
        /// <param name="Key">键</param>
        /// <param name="filepath">路径</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public static  string GetINI(string Section, string Key, string filepath,string def)
        {
            try
            {
                filepath = GetPhyPath(filepath);
                if (File.Exists(filepath) == false)
                {
                    File.Create(filepath).Close();
                }
                StringBuilder temp = new StringBuilder(255);
                int i = GetPrivateProfileString(Section, Key, def, temp, 255, filepath);
                return temp.ToString();

                //Byte[] Buffer = new Byte[65535];
                //int bufLen = GetPrivateProfileString(Section, Key, def, Buffer, Buffer.GetUpperBound(0), filepath);
                ////必须设定0（系统默认的代码页）的编码方式，否则无法支持中文
                //string s = Encoding.GetEncoding(0).GetString(Buffer);
                //s = s.Substring(0, bufLen);
                //return s.Trim();

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 根据相对路径获取文件、文件夹绝对路径
        /// </summary>
        /// <param name="FileName">相对路径</param>   
        /// <returns></returns>
        public static string GetPhyPath(string FileName)
        {
            FileName = FileName.Replace('/', '\\');             //规范路径写法
            if (FileName.IndexOf(':') != -1) return FileName;   //已经是绝对路径了
            if (FileName.Length > 0 && FileName[0] == '\\') FileName = FileName.Substring(1);
            return string.Format("{0}\\{1}", Directory.GetCurrentDirectory(), FileName);
        }
        #endregion;

    }
}
