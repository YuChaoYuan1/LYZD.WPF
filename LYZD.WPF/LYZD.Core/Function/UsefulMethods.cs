using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace LYZD.Core.Function
{
    public class UsefulMethods
    {
        /// <summary>
        /// 获取IPv4地址
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIpv4()
        {
            try
            {
                // 获得网络接口，网卡，拨号器，适配器都会有一个网络接口
                NetworkInterface[] nets = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface net in nets)
                {
                    // 获得当前网络接口属性
                    IPInterfaceProperties prop = net.GetIPProperties();

                    // 每个网络接口可能会有多个IP地址
                    foreach (IPAddressInformation address in prop.UnicastAddresses)
                    {
                        // 如果此IP不是ipv4，则进行下一次循环
                        if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;

                        // 忽略127.0.0.1
                        if (IPAddress.IsLoopback(address.Address))
                            continue;

                        return address.Address.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
              
            }

            return null;
        }

        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="Pstr"></param>
        /// <returns></returns>
        public static string revStr(string Pstr) // '反转字符串
        {
            int Ti;
            string Ts;
            int Tlen;
            Tlen = (int)(Pstr.Length / 2) - 1;
            Ts = "";
            for (Ti = Tlen; Ti >= 0; Ti--)
                Ts = Ts + Pstr.Substring(Ti * 2, 2);
            return Ts;
        }

        /// <summary>
        /// 字符串转换成Byte数组
        /// </summary>
        /// <param name="p_str_Context"></param>
        /// <returns></returns>
        public static byte[] ConvertStringToBytes(string p_str_Context)
        {
            if (p_str_Context.Length < 1)
                return new byte[0];
            int int_ByteCount = p_str_Context.Length / 2;
            byte[] byt_Return = new byte[int_ByteCount];

            for (int i = 0; i < int_ByteCount; i++)
            {
                byt_Return[i] = Convert.ToByte(p_str_Context.Substring(i * 2, 2), 16);
            }

            return byt_Return;
        }

        /// <summary>
        /// Byte数组转换成字符串
        /// </summary>
        /// <param name="p_str_Context"></param>
        /// <returns></returns>
        public static string ConvertBytesToString(byte[] p_byte_Context)
        {
            string strFrame = "";
            try
            {
                if (p_byte_Context == null) return strFrame;
                for (int i = 0; i < p_byte_Context.Length; i++)
                {
                    strFrame += Convert.ToString(p_byte_Context[i], 16).PadLeft(2, '0') + " ";
                }
                return strFrame;
            }
            catch
            { return strFrame; }

        }

        /// <summary>
        /// List数组转换成字符串
        /// </summary>
        /// <param name="p_str_Context"></param>
        /// <returns></returns>
        public static string ConvertListToString(List<byte> p_list_Context)
        {
            string strFrame = "";
            try
            {
                if (p_list_Context == null) return strFrame;
                for (int i = 0; i < p_list_Context.Count; i++)
                {
                    strFrame += Convert.ToString(p_list_Context[i], 16).PadLeft(2, '0');
                }
                return strFrame;
            }
            catch
            { return strFrame; }

        }

        public static string ConverStringToString(string[] p_string_Context)
        {
            string strFrame = "";
            for (int i = 0; i < p_string_Context.Length; i++)
            {
                strFrame += "\r\n" + p_string_Context[i];
            }
            return strFrame;
        }

        /// <summary>
        /// 将运行日志保存到文件
        /// </summary>
        /// <param name="info">信息内容</param>
        /// <param name="str">文件名</param>
        public static void WriteLog(string info, string str)
        {

            string ls_filename;
            string ls_line;

            TextWriter s;
            StringWriter strWriter;
            try
            {
                ls_filename = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\" + str + ".log";
                string FilePath = Path.GetDirectoryName(ls_filename);
                if (!Directory.Exists(FilePath))
                    Directory.CreateDirectory(FilePath);
                strWriter = new StringWriter();
                s = new StreamWriter(ls_filename, true, System.Text.Encoding.Default);

                ls_line = info;
                s.WriteLine(ls_line);
                s.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        static object obj = new object();
        /// <summary>
        /// 将运行日志保存到文件
        /// </summary>
        /// <param name="info">信息内容</param>
        /// <param name="str">文件名</param>
        public static void WriteLog2(string info, string str)
        {

            lock (obj)
            {
                string ls_filename;
                string ls_line;

                TextWriter s;
                StringWriter strWriter;
                try
                {
                    ls_filename = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\" + str + ".log";
                    string FilePath = Path.GetDirectoryName(ls_filename);
                    if (!Directory.Exists(FilePath))
                        Directory.CreateDirectory(FilePath);
                    strWriter = new StringWriter();
                    s = new StreamWriter(ls_filename, true, System.Text.Encoding.Default);

                    ls_line = info;
                    s.WriteLine(ls_line);
                    s.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// string数组转换成int数组
        /// </summary>
        /// <param name="p_str_Input"></param>
        /// <returns></returns>
        public static int[] ConverStringToIntGroup(string[] p_str_Input)
        {
            Converter<string, int> myCon = new Converter<string, int>(ConverStringToInt);
            int[] int_ReturnGroup = Array.ConvertAll<string, int>(p_str_Input, myCon);
            return int_ReturnGroup;
        }

        /// <summary>
        /// 校验和,将所有的项加起来除以256得到的余数
        /// </summary>
        /// <param name="aryData"></param>
        /// <returns></returns>
        public static byte GetChkSum(string str)
        {
            int bytChk = 0;
            for (int i = 0; i < str.Length / 2; i++)
            {
                bytChk += Convert.ToInt16(str.Substring(2 * i, 2), 16);
            }
            return Convert.ToByte(bytChk % 256);
        }

        private static int ConverStringToInt(string str)
        {
            try
            {
                return Convert.ToInt32(str);
            }
            catch
            {
                return -999;
            }
        }

        /// <summary>
        /// 组合bin类型数据
        /// </summary>
        /// <param name="num">数据</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string OrgBinFun(Int64 num, int len)
        {
            List<byte> tempData = new List<byte>();
            string strTmp = "";
            for (int i = len - 1; i >= 0; i--)
            {
                var data = (num >> (8 * i)) % 256;
                tempData.Add(Convert.ToByte(data));
            }
            tempData.Reverse();
            for (int i = 0; i < tempData.Count; i++)
            {
                strTmp += tempData[i].ToString("x2");
            }
            return strTmp;
        }

        public static object objlock = new object();
        public static void Log(string message)
        {
            lock (objlock)
            {
                //System.IO.File.AppendAllText(Application.StartupPath + "\\CL3220SHlog.txt", "-------------------------" + DateTime.Now.ToString());
                //System.IO.File.AppendAllText(Application.StartupPath + "\\CL3220SHlog.txt", message + Environment.NewLine);
            }
        }
    }
}
