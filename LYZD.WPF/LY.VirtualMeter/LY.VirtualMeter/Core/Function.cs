using LY.VirtualMeter.Base;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LY.VirtualMeter.Core
{
     public class Function :NotifyPropertyBase
    {
        public static bool Is_Thread = true;
        public static bool Is_Doing = true;
        public static int G_Bws = 15;
        public static bool G_ZB = true;
        public static bool[] G_YJ = new bool[16];
        public const int CST_CLOSE = 1;
        public const int CST_OPENZD = 2;
        public const int Cst_OPENBIAO = 3;

        #region "读取写入INI"
        [DllImport("kernel32.dll")]
        public static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32.dll")]
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
        } /// <summary>
          /// 读取INI文件
          /// </summary>
          /// <param name=^Section^>节点名称</param>
          /// <param name=^Key^>关键字</param>
          /// <param name=^filepath^>INI文件路径</param>
          /// <returns>值</returns>
        public static string GetINI(string Section, string Key, string filepath, string defValue = "")
        {
            try
            {
                StringBuilder temp = new StringBuilder(255);
                int i = GetPrivateProfileString(Section, Key, "", temp, 255, filepath);
                //默认值
                if (defValue != "" && temp.ToString().Trim() == "")
                {
                    return defValue;
                }
                return temp.ToString();
            }
            catch (Exception)
            {

                return defValue;
            }

        }

        #endregion;

        [DllImport("winmm.dll")]
        public static extern int timeGetTime();

        [DllImport("kernel32.DLL")]
        private static extern int WritePrivateProfileStringA(string lpName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32.DLL")]
        private static extern int GetPrivateProfileStringA(string lpApplicationName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32.DLL")]
        public static extern bool SetLocalTime(ref SYSTEMTIME Time);

        /// <summary>
        /// 加密机类型
        /// </summary>
        public static string strEncryptionType;
        public static void Delay(int Tms) //pyys=硬延时
        {
            int Tb; int Te;
            if (Tms < 20) Tms = 20;
            Tb = timeGetTime();
            while (true)
            {
                Application.DoEvents();
                Te = timeGetTime();
                if (Te < Tb) Tb = 0;
                if ((Te - Tb) > Tms) break;
            }

        }
        public static bool IsNumber(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");

        }
        public static string g_GetINI(string sINIFile, string sSection, string sKey, string sDefault)
        {
            StringBuilder sTemp = new StringBuilder(256);   //As String * 256
            int nLength;

            nLength = GetPrivateProfileStringA(sSection, sKey, sDefault, sTemp, 255, sINIFile);
            return sTemp.ToString().Substring(0, nLength);
        }

        public static void g_WriteINI(string sINIFile, string sSection, string sKey, string sValue)
        {
            int N;
            string sTemp;
            sTemp = sValue;
            N = WritePrivateProfileStringA(sSection, sKey, sTemp, sINIFile);

        }

        /// <summary>
        /// WriteLog 将运行日志保存到文件//czx
        /// </summary>
        /// <param name="info"></param>
        public static void WriteLog(string s1, string info)
        {
            object obj = new object();
            lock (obj)
            {
                string ls_filename;
                string ls_line;

                TextWriter s;
                StringWriter strWriter;
                try
                {
                    //ls_filename = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                    ls_filename = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\" + s1 + ".log";
                    string FilePath = Path.GetDirectoryName(ls_filename);
                    if (!Directory.Exists(FilePath))
                        Directory.CreateDirectory(FilePath);
                    strWriter = new StringWriter();
                    s = new StreamWriter(ls_filename, true, System.Text.Encoding.Default);

                    ls_line = info;
                    s.WriteLine(ls_line+"\r\n");
                    s.Close();
                }
                catch
                {
                }
            }
        }
        public static string GetEndByte(string bytStr)
        {
            List<byte> bytes = new List<byte>();

            for (int i = 0; i < bytStr.Length / 2; i++)
            {
                bytes.Add(Convert.ToByte(Convert.ToInt32(bytStr.Substring(i * 2, 2), 16)));
            }
            return getChkSum(bytes.ToArray()).ToString("x2");
        }

        public static byte getChkSum(byte[] aryData)
        {
            int Ti = 0;
            byte bytChk = 0;
            for (Ti = 0; Ti < aryData.Length - 2; Ti++)
            {
                bytChk = Convert.ToByte((bytChk + aryData[Ti]) % 256);
            }
            return bytChk;
        }

        public static byte[] ChangeTtoByte(string Tstr)
        {
            int i; int Tlen;
            Tlen = (int)(Tstr.Length / 2);
            byte[] Tcl = new byte[Tlen];
            for (i = 0; i < Tlen; i++)
                Tcl[i] = Convert.ToByte(Tstr.Substring(i * 2, 2), 16);

            return Tcl;
        }

        public static string getChkSum(string Pstr)        //获得校验码
        {
            int Ti;
            int Tlen;
            int Tall;
            Tall = 0;

            Tlen = (int)(Pstr.Length / 2) - 1;
            for (Ti = 0; Ti <= Tlen; Ti++)
                Tall = (Tall + Convert.ToByte(Pstr.Substring(Ti * 2, 2), 16)) % 256;
            return string.Format("{0:X2}", Tall);
        }

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

        public static string g_GetItem(string Pstr, int Pnum, string Pfh)//        '取得一个串中用,分开的某个字符串
        {
            int Ti;
            int Tseat;
            string Tstr;

            Ti = 0; Tstr = Pstr;
            if (Pnum < 1)
                return "";
            else
            {
                while (true)
                {
                    Tseat = Tstr.IndexOf(Pfh);
                    if (Tseat <= 0)
                        return "";
                    else
                    {
                        Ti++;
                        if (Ti == Pnum)
                            return Tstr.Substring(0, Tseat);

                        else
                            Tstr = Tstr.Substring(Tseat + 1, Tstr.Length - Tseat - 1) + " ";
                    }
                }
            }
        }

        public static string getSingle(double Ps, int Pdot, int Pbyte)
        {
            double Tl;
            string Tr;
            Tl = (double)(Math.Abs(Ps) * (int)Interaction.Choose(Pdot + 1, 1, 10, 100, 1000, 10000, 100000));
            Tl = Convert.ToInt32(Tl);
            Tr = "000000000000" + Tl.ToString();
            Tr = Tr.Substring(Tr.Length - Pbyte * 2, Pbyte * 2);
            return revStr(Tr);
        }

        public static string Del33H(string Pstr)
        {
            string Tstr = "";
            int Tlen;
            Tlen = Pstr.Length;
            for (int i = 0; i < Tlen; i = i + 2)
                Tstr = Tstr + string.Format("{0:X2}", (Convert.ToByte(Pstr.Substring(i, 2), 16) - 0x33));
            return Tstr;
        }

        public static string Add33H(string Pstr)
        {
            int Ti;
            int Tlen;
            string Tstr;
            Tlen = Pstr.Length;
            Tstr = "";
            for (Ti = 1; Ti <= Tlen; Ti = Ti + 2)
                Tstr = Tstr + string.Format("{0:X2}", (Convert.ToByte(Pstr.Substring(Ti - 1, 2), 16) + 0x33) % 0x100);
            return Tstr;
        }

        public static string getChk(string Pstr)        //获得校验码
        {
            int Ti;
            int Tlen;
            int Tall;
            Tall = 0;
            Tlen = (int)(Pstr.Length / 2) - 1;
            for (Ti = 0; Ti <= Tlen; Ti++)
                Tall = (Tall + Convert.ToByte(Pstr.Substring(Ti * 2, 2), 16)) % 256;
            return string.Format("{0:X2}", Tall);
        }

        public static bool XiaoYan(string Tvalue)
        {
            byte[] Cl;
            int TCS; int Ti; int Tlen; string Tstr;
            Tlen = (int)(Tvalue.Length / 2);
            Cl = new byte[Tlen];
            TCS = 0;
            for (Ti = 0; Ti < Tlen; Ti++)
            {
                Tstr = Tvalue.Substring(Ti * 2, 2);
                Cl[Ti] = Convert.ToByte(Tstr, 16);
                if (Ti <= Tlen - 3)
                    TCS = (TCS + Cl[Ti]) % 256; ;
            }
            if (TCS % 256 == Cl[Tlen - 2])
                return true;
            else
                return false;
        }

        public static string getSingle698(double Ps, int Pdot, int Pbyte)
        {
            double Tl;
            string Tr;
            Tl = (double)(Math.Abs(Ps) * (int)Interaction.Choose(Pdot + 1, 1, 10, 100, 1000, 10000, 100000));
            Tl = Convert.ToInt32(Tl);
            Tr = "000000000000" + Tl.ToString();

            Tr = (Convert.ToInt64(Tl)).ToString("x8");

            Tr = Tr.Substring(Tr.Length - Pbyte * 2, Pbyte * 2);
            return Tr;
            //return revStr(Tr);
        }

        public static string getDateTimeBCD(DateTime dt)
        {
            return "1C" + dt.Year.ToString("x4") + dt.Month.ToString("x2") + dt.Day.ToString("x2") + dt.Hour.ToString("x2") + dt.Minute.ToString("x2") + dt.Second.ToString("x2");
        }

        #region CRC校验

        /// <summary>
        /// 字符串转换成Byte数组
        /// </summary>
        /// <param name="p_str_Context"></param>
        /// <returns></returns>
        public static byte[] ConvertStringToBytes(string p_str_Context)
        {

            if (p_str_Context.Length < 1)
                return new byte[0];

            p_str_Context = Regex.Replace(p_str_Context, @"[\r\n]", "");
            p_str_Context.Trim();

            int int_ByteCount = p_str_Context.Length / 2;
            byte[] byt_Return = new byte[int_ByteCount];
            
            for (int i = 0; i < int_ByteCount; i++)
            {
                string value = p_str_Context.Substring(i * 2, 2);
                byt_Return[i] = Convert.ToByte(value, 16);
            }

            return byt_Return;
        }

        public static long CheckCrc16(byte[] pbyt, int iLen)
        {
            long trialfcs;
            long PPPINITFCS16 = 0xffff;
            trialfcs = pppfcs16(PPPINITFCS16, pbyt, iLen);
            trialfcs ^= 0xffff;
            return trialfcs;
        }

        public static long CheckCrc16(string str, int iLen)
        {
            byte[] pbyt = ConvertStringToBytes(str);
            long trialfcs;
            long PPPINITFCS16 = 0xffff;
            trialfcs = pppfcs16(PPPINITFCS16, pbyt, iLen);
            trialfcs ^= 0xffff;
            return trialfcs;
        }

        public static int[] fcstab = new int[256] {
            0x0000, 0x1189, 0x2312, 0x329b, 0x4624, 0x57ad, 0x6536, 0x74bf,
            0x8c48, 0x9dc1, 0xaf5a, 0xbed3, 0xca6c, 0xdbe5, 0xe97e, 0xf8f7,
            0x1081, 0x0108, 0x3393, 0x221a, 0x56a5, 0x472c, 0x75b7, 0x643e,
            0x9cc9, 0x8d40, 0xbfdb, 0xae52, 0xdaed, 0xcb64, 0xf9ff, 0xe876,
            0x2102, 0x308b, 0x0210, 0x1399, 0x6726, 0x76af, 0x4434, 0x55bd,
            0xad4a, 0xbcc3, 0x8e58, 0x9fd1, 0xeb6e, 0xfae7, 0xc87c, 0xd9f5,
            0x3183, 0x200a, 0x1291, 0x0318, 0x77a7, 0x662e, 0x54b5, 0x453c,
            0xbdcb, 0xac42, 0x9ed9, 0x8f50, 0xfbef, 0xea66, 0xd8fd, 0xc974,
            0x4204, 0x538d, 0x6116, 0x709f, 0x0420, 0x15a9, 0x2732, 0x36bb,
            0xce4c, 0xdfc5, 0xed5e, 0xfcd7, 0x8868, 0x99e1, 0xab7a, 0xbaf3,
            0x5285, 0x430c, 0x7197, 0x601e, 0x14a1, 0x0528, 0x37b3, 0x263a,
            0xdecd, 0xcf44, 0xfddf, 0xec56, 0x98e9, 0x8960, 0xbbfb, 0xaa72,
            0x6306, 0x728f, 0x4014, 0x519d, 0x2522, 0x34ab, 0x0630, 0x17b9,
            0xef4e, 0xfec7, 0xcc5c, 0xddd5, 0xa96a, 0xb8e3, 0x8a78, 0x9bf1,
            0x7387, 0x620e, 0x5095, 0x411c, 0x35a3, 0x242a, 0x16b1, 0x0738,
            0xffcf, 0xee46, 0xdcdd, 0xcd54, 0xb9eb, 0xa862, 0x9af9, 0x8b70,
            0x8408, 0x9581, 0xa71a, 0xb693, 0xc22c, 0xd3a5, 0xe13e, 0xf0b7,
            0x0840, 0x19c9, 0x2b52, 0x3adb, 0x4e64, 0x5fed, 0x6d76, 0x7cff,
            0x9489, 0x8500, 0xb79b, 0xa612, 0xd2ad, 0xc324, 0xf1bf, 0xe036,
            0x18c1, 0x0948, 0x3bd3, 0x2a5a, 0x5ee5, 0x4f6c, 0x7df7, 0x6c7e,
            0xa50a, 0xb483, 0x8618, 0x9791, 0xe32e, 0xf2a7, 0xc03c, 0xd1b5,
            0x2942, 0x38cb, 0x0a50, 0x1bd9, 0x6f66, 0x7eef, 0x4c74, 0x5dfd,
            0xb58b, 0xa402, 0x9699, 0x8710, 0xf3af, 0xe226, 0xd0bd, 0xc134,
            0x39c3, 0x284a, 0x1ad1, 0x0b58, 0x7fe7, 0x6e6e, 0x5cf5, 0x4d7c,
            0xc60c, 0xd785, 0xe51e, 0xf497, 0x8028, 0x91a1, 0xa33a, 0xb2b3,
            0x4a44, 0x5bcd, 0x6956, 0x78df, 0x0c60, 0x1de9, 0x2f72, 0x3efb,
            0xd68d, 0xc704, 0xf59f, 0xe416, 0x90a9, 0x8120, 0xb3bb, 0xa232,
            0x5ac5, 0x4b4c, 0x79d7, 0x685e, 0x1ce1, 0x0d68, 0x3ff3, 0x2e7a,
            0xe70e, 0xf687, 0xc41c, 0xd595, 0xa12a, 0xb0a3, 0x8238, 0x93b1,
            0x6b46, 0x7acf, 0x4854, 0x59dd, 0x2d62, 0x3ceb, 0x0e70, 0x1ff9,
            0xf78f, 0xe606, 0xd49d, 0xc514, 0xb1ab, 0xa022, 0x92b9, 0x8330,
            0x7bc7, 0x6a4e, 0x58d5, 0x495c, 0x3de3, 0x2c6a, 0x1ef1, 0x0f78
        };

        /// <summary>
        /// 计算校验输入： pbyt - 数据，iLen - 长度输出： pbytRes1 - 结果的第一字节（先发），pbytRes2 - 第二字节
        /// </summary>
        /// <param name="fcs"></param>
        /// <param name="cp"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static long pppfcs16(long fcs, byte[] cp, int len)
        {
            int i = 0;
            while (len-- > 0)
            {
                fcs = (fcs >> 8) ^ fcstab[(fcs ^ cp[i++]) & 0xff];
            }
            return (fcs);
        }

        public static byte 获取通讯方式编号(string name)
        {
            byte value = 5;
            switch (name)
            {
                case "串口":
                    value = 5;
                    break;
                default:
                    break;
            }
            return value;

        }
        #endregion
    }
}
