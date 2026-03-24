using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.TerminalProtocol.GW
{
     public class Frame_698 : Terminal
    {


        /// <summary>
        /// 登陆
        /// </summary>
        /// <returns></returns>
        public string ReturnLogin(string piid, DateTime dt)
        {
            string str_Frame = "";
            List<byte> lisTmp = new List<byte>(); byte[] bytTmp; string[] strTmp2;
            float Volt;
            float Curt;
            byte Clfs;
            string Ip = ""; string Port = "";

            string sAPDU = "81" + piid + "80" + SetDateTimeBCD(dt) + SetDateTimeBCD(DateTime.Now) + SetDateTimeBCD(DateTime.Now);
            string slen = (sAPDU.Length / 2 + str_Ter_Address.Length / 2 + 9).ToString("x4").Substring(2, 2) + (sAPDU.Length / 2 + str_Ter_Address.Length / 2 + 9).ToString("x4").Substring(0, 2);

            string sHFrame = slen + "01" + (str_Ter_Address.Length / 2 - 1).ToString("x2") + revStr(str_Ter_Address) + "00";
            long checkhead = CheckCrc16(sHFrame, sHFrame.Length / 2);
            string sFrame = sHFrame + (checkhead & 0xff).ToString("x2") + ((checkhead >> 8) & 0xff).ToString("x2") + sAPDU;
            checkhead = CheckCrc16(sFrame, sFrame.Length / 2);

            sFrame = "68" + sFrame + (checkhead & 0xff).ToString("x2") + ((checkhead >> 8) & 0xff).ToString("x2") + "16";
            //str_Frame = GetByteToStr(byt_Frame);
            return sFrame;
        }

        private string GetByteToStr(byte[] bytTmp)
        {
            string strFrame = "";
            for (int i = 0; i < bytTmp.Length; i++)
            {
                strFrame += Convert.ToString(bytTmp[i], 16).PadLeft(2, '0');
            }
            return strFrame;
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
            int int_ByteCount = p_str_Context.Length / 2;
            byte[] byt_Return = new byte[int_ByteCount];

            for (int i = 0; i < int_ByteCount; i++)
            {
                byt_Return[i] = Convert.ToByte(p_str_Context.Substring(i * 2, 2), 16);
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
        #endregion



        public byte[] ReadData(string sAPDU)
        {

            sAPDU = sAPDU.Replace(" ", "");
            List<byte> lisTmp = new List<byte>(); byte[] bytTmp; string[] strTmp2;
            float Volt;
            float Curt;
            byte Clfs;
            string Ip = ""; string Port = "";

            //string sAPDU = "050100" + sOAD + "00";
            string slen = (sAPDU.Length / 2 + str_Ter_Address.Length / 2 + 9).ToString("x4").Substring(2, 2) + (sAPDU.Length / 2 + str_Ter_Address.Length / 2 + 9).ToString("x4").Substring(0, 2);

            string sHFrame = slen + "43" + (str_Ter_Address.Length / 2 - 1).ToString("x2") + revStr(str_Ter_Address) + "00";

            //sHFrame = slen + "4345" + revStr("AAAAAAAAAAAA") + "00";
            long checkhead = CheckCrc16(sHFrame, sHFrame.Length / 2);
            string sFrame = sHFrame + (checkhead & 0xff).ToString("x2") + ((checkhead >> 8) & 0xff).ToString("x2") + sAPDU;
            checkhead = CheckCrc16(sFrame, sFrame.Length / 2);

            sFrame = "68" + sFrame + (checkhead & 0xff).ToString("x2") + ((checkhead >> 8) & 0xff).ToString("x2") + "16";


            return ConvertStringToBytes(sFrame);
        }

        public byte[] ReadData_jiaocai(string sAPDU)
        {
            sAPDU = sAPDU.Replace(" ", "");
            List<byte> lisTmp = new List<byte>(); 
            byte[] bytTmp; string[] strTmp2;
            float Volt;
            float Curt;
            byte Clfs;
            string Ip = ""; string Port = "";

            //string sAPDU = "050100" + sOAD + "00";
            string slen = (sAPDU.Length / 2 + str_Ter_Address.Length / 2 + 9).ToString("x4").Substring(2, 2) + (sAPDU.Length / 2 + str_Ter_Address.Length / 2 + 9).ToString("x4").Substring(0, 2);

            string sHFrame = slen + "43" + (str_Ter_Address.Length / 2 - 1 + 16).ToString("x2") + revStr(str_Ter_Address) + "00";

            //sHFrame = slen + "4345" + revStr("AAAAAAAAAAAA") + "00";
            long checkhead = CheckCrc16(sHFrame, sHFrame.Length / 2);
            string sFrame = sHFrame + (checkhead & 0xff).ToString("x2") + ((checkhead >> 8) & 0xff).ToString("x2") + sAPDU;
            checkhead = CheckCrc16(sFrame, sFrame.Length / 2);

            sFrame = "68" + sFrame + (checkhead & 0xff).ToString("x2") + ((checkhead >> 8) & 0xff).ToString("x2") + "16";


            return ConvertStringToBytes(sFrame);

        }

        /// <summary>
        /// 获取时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string SetDateTimeBCD(DateTime dt)
        {
            int week = 0;
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    week = 0; break;
                case DayOfWeek.Monday:
                    week = 1; break;
                case DayOfWeek.Tuesday:
                    week = 2; break;
                case DayOfWeek.Wednesday:
                    week = 3; break;
                case DayOfWeek.Thursday:
                    week = 4; break;
                case DayOfWeek.Friday:
                    week = 5; break;
                case DayOfWeek.Saturday:
                    week = 6; break;
            }

            return dt.Year.ToString("x4") + dt.Month.ToString("x2") + dt.Day.ToString("x2") + week.ToString("x2") + dt.Hour.ToString("x2") + dt.Minute.ToString("x2") + dt.Second.ToString("x2") + "0000";
        }

        /// <summary>
        /// 获取时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string SetDateTimeBCD(DateTime dt, bool booldw)
        {
            if (booldw)
                return "1C" + dt.Year.ToString("x4") + dt.Month.ToString("x2") + dt.Day.ToString("x2") + dt.Hour.ToString("x2") + dt.Minute.ToString("x2") + dt.Second.ToString("x2");
            return dt.Year.ToString("x4") + dt.Month.ToString("x2") + dt.Day.ToString("x2") + dt.Hour.ToString("x2") + dt.Minute.ToString("x2") + dt.Second.ToString("x2");
        }

        public byte[] ReadData_05(string sOAD)
        {
            List<byte> lisTmp = new List<byte>(); byte[] bytTmp; string[] strTmp2;
            float Volt;
            float Curt;
            byte Clfs;
            string Ip = ""; string Port = "";

            string sAPDU = "050100" + sOAD + "00";
            string slen = (sAPDU.Length / 2 + str_Ter_Address.Length / 2 + 9).ToString("x4").Substring(2, 2) + (sAPDU.Length / 2 + str_Ter_Address.Length / 2 + 9).ToString("x4").Substring(0, 2);

            string sHFrame = slen + "43" + (str_Ter_Address.Length / 2 - 1).ToString("x2") + revStr(str_Ter_Address) + "00";
            long checkhead = CheckCrc16(sHFrame, sHFrame.Length / 2);
            string sFrame = sHFrame + (checkhead & 0xff).ToString("x2") + ((checkhead >> 8) & 0xff).ToString("x2") + sAPDU;
            checkhead = CheckCrc16(sFrame, sFrame.Length / 2);

            sFrame = "68" + sFrame + (checkhead & 0xff).ToString("x2") + ((checkhead >> 8) & 0xff).ToString("x2") + "16";


            return ConvertStringToBytes(sFrame);

        }

        public byte[] ReadData_address(string sOAD)
        {
            List<byte> lisTmp = new List<byte>(); byte[] bytTmp; string[] strTmp2;
            float Volt;
            float Curt;
            byte Clfs;
            string Ip = ""; string Port = "";

            string sAPDU = "050100" + sOAD + "00";
            string slen = (sAPDU.Length / 2 + 15).ToString("x4").Substring(2, 2) + (sAPDU.Length / 2 + 15).ToString("x4").Substring(0, 2);

            string sHFrame = slen + "4345" + revStr("AAAAAAAAAAAA") + "00"; ;

            long checkhead = CheckCrc16(sHFrame, sHFrame.Length / 2);
            string sFrame = sHFrame + (checkhead & 0xff).ToString("x2") + ((checkhead >> 8) & 0xff).ToString("x2") + sAPDU;
            checkhead = CheckCrc16(sFrame, sFrame.Length / 2);

            sFrame = "68" + sFrame + (checkhead & 0xff).ToString("x2") + ((checkhead >> 8) & 0xff).ToString("x2") + "16";


            return ConvertStringToBytes(sFrame);

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


    }
}
