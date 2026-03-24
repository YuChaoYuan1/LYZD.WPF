using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.TerminalProtocol
{
    public class Comm
    {
        /// <summary>
        /// 获取冻结密度
        /// </summary>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string getdjmd(int iv)
        {
            switch (iv)
            {
                case 0:
                    return "时间间隔:不冻结";
                case 1:
                    return "时间间隔:15分";
                case 2:
                    return "时间间隔:30分";
                case 3:
                    return "时间间隔:60分";
                case 4:
                    return "时间间隔:24小时";
                default:
                    return "时间间隔:不冻结";
            }
        }

        public static string ReAscill(string str)
        {
            byte[] by = new byte[str.Length / 2];
            for (int i = 0; i < str.Length / 2; i++)
            {
                by[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
            }
            return System.Text.ASCIIEncoding.ASCII.GetString(by).Substring(0, System.Text.ASCIIEncoding.ASCII.GetString(by).IndexOf('\0'));
        }

        public static string gd1(string Tstr)
        {
            string Ts = "";
            if (Tstr.Substring(0, 4) == "EEEE") Ts = "2099-1-1";
            Ts = (Convert.ToInt16(Tstr.Substring(0, 1)) * 10 + Convert.ToInt16(Tstr.Substring(1, 1))).ToString();
            Ts = (Convert.ToInt16(Tstr.Substring(2, 1)) * 10 + Convert.ToInt16(Tstr.Substring(3, 1))).ToString() + ":" + Ts;
            Ts = (Convert.ToInt16(Tstr.Substring(4, 1)) * 10 + Convert.ToInt16(Tstr.Substring(5, 1))).ToString() + ":" + Ts;
            Ts = (Convert.ToInt16(Tstr.Substring(6, 1)) * 10 + Convert.ToInt16(Tstr.Substring(7, 1))).ToString() + " " + Ts;

            Ts = (Convert.ToInt16(Tstr.Substring(8, 1), 16) & 1) * 10 + Convert.ToInt16(Tstr.Substring(9, 1)) + "-" + Ts;
            Ts = "20" + (Convert.ToInt16(Tstr.Substring(10, 1)) * 10 + Convert.ToInt16(Tstr.Substring(11, 1))).ToString() + "-" + Ts;

            int x1 = 0; string str = "";
            x1 = Convert.ToInt16(Tstr.Substring(8, 1), 16) / 2;
            switch (x1)
            {
                case 1: str = "星期一"; break;
                case 2: str = "星期二"; break;
                case 3: str = "星期三"; break;
                case 4: str = "星期四"; break;
                case 5: str = "星期五"; break;
                case 6: str = "星期六"; break;
                case 7: str = "星期日"; break;
            }
            return Ts + " " + str;
        }

        public static string gd2(string Tstr)
        {
            try
            {
                if (Tstr.Substring(0, 2) == "EE") return "99999.9";
                return (Convert.ToSingle(Tstr.Substring(0, 1)) + Convert.ToSingle(Tstr.Substring(1, 1)) / 10 + Convert.ToSingle(Tstr.Substring(2, 1)) * 100 + Convert.ToSingle(Tstr.Substring(3, 1)) * 10).ToString();
            }
            catch
            {
                return "99999.9";
            }
        }

        /// <summary>
        /// 解析数据格式03.
        /// <param name="strData">接收帧字符串长度:11个字符,</param>
        /// 返回值单位:kWh,当接受参数无效时,返回默认值99999
        /// </summary> 
        public static string gd3(string strData)
        {
            int intSum;//求和
            int intStrLen;//字符串长度
            string strUnit;//表示单位
            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    intSum = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1))
                      + int.Parse(strData.Substring(2, 1)) * 1000 + int.Parse(strData.Substring(3, 1)) * 100
                      + int.Parse(strData.Substring(4, 1)) * 100000 + int.Parse(strData.Substring(5, 1)) * 10000
                      + int.Parse(strData.Substring(7, 1)) * 1000000;

                    if ((Convert.ToInt32((strData.Substring(6, 1)), 16) & 4) == 4)
                    {
                        strUnit = "MWh";
                    }
                    else
                    {
                        strUnit = "kWh";
                    }
                    if ((Convert.ToInt32((strData.Substring(6, 1)), 16) & 1) == 1)
                    {
                        return (-intSum).ToString() + strUnit;
                    }
                    else
                    {
                        return intSum.ToString() + strUnit;
                    }

                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return int.Parse(strDefValu);
                }

            }
        }

        /// <summary>
        /// 解析数据格式04.
        /// <param name="strData">接收帧字符串长度:2个字符,</param>
        /// 当参数无效时,返回默认参数99999
        /// </summary> 
        public static string gd4(string strData)
        {
            int intSum;//求和
            int intStrLen;//字符串长度

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 2) == "EE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    intSum = (Convert.ToInt32((strData.Substring(0, 1)), 16) & 7) * 10 + int.Parse(strData.Substring(1, 1));
                    if ((Convert.ToInt32((strData.Substring(0, 1)), 16) & 8) == 0)//s0=0代表上浮
                    {
                        return "上浮: " + intSum.ToString();
                    }
                    else
                    {
                        return "下浮: " + intSum.ToString();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return strDefValu;                    
                }

            }
        }

        /// <summary>
        /// 解析数据格式05.
        /// <param name="strData">接收帧字符串长度:5个字符,</param>
        /// <returns> 带符号的浮点数,返回值整数位最高到百位,小数位精度到十分位</returns>
        /// 
        /// 当参数无效时,返回默认值-999999
        /// </summary> 
        public static string gd5(string strData)
        {
            float fltSum;//求和
            int intStrLen;//字符长度

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    fltSum = Convert.ToInt32(strData.Substring(0, 1), 16) + Convert.ToInt16(strData.Substring(1, 1), 16) / 10F
                    + (Convert.ToInt32(strData.Substring(2, 1), 16) & 7) * 100 + Convert.ToInt32(strData.Substring(3, 1), 16) * 10;

                    if ((Convert.ToInt32(strData.Substring(2, 1), 16) & 8) == 8)
                    {
                        return (-fltSum).ToString();
                    }
                    else
                    {
                        return fltSum.ToString();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return float.Parse(strDefValu);
                }

            }
        }

        /// <summary>
        /// 解析数据格式06.
        /// <param name="strData">接收帧字符串长度:5个字符,</param>
        /// <returns> 带符号的浮点数,返回值整数位最高到十位,小数位精度到百分位</returns>
        /// 当参数无效时,返回默认值-999999
        /// </summary> 
        public static string gd6(string strData)
        {
            float fltSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    fltSum = float.Parse(strData.Substring(0, 1)) / 10 + float.Parse(strData.Substring(1, 1)) / 100
                    + (Convert.ToInt32((strData.Substring(2, 1)), 16) & 7) * 10 + int.Parse(strData.Substring(3, 1));

                    if ((Convert.ToInt32((strData.Substring(2, 1)), 16) & 8) == 8)
                    {
                        return (-fltSum).ToString();
                    }
                    else
                    {
                        return fltSum.ToString();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return float.Parse(strDefValu);                    
                }

            }
        }

        /// <summary>
        /// 解析数据格式07.
        /// <param name="strData">接收帧字符串长度:5个字符,</param>
        /// <returns>不带符号的浮点数,返回值整数位最高到百位,小数位精度到十分位</returns>
        /// 当参数无效时,返回默认值-999999
        /// </summary> 
        public static string gd7(string strData)
        {
            float fltSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 2) == "EE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    fltSum = Convert.ToInt32(strData.Substring(0, 1), 16) + Convert.ToInt32(strData.Substring(1, 1), 16) / 10f
                        + Convert.ToInt32(strData.Substring(2, 1), 16) * 100 + Convert.ToInt32(strData.Substring(3, 1), 16) * 10;
                    return fltSum.ToString();
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return float.Parse(strDefValu);
                }

            }
        }

        /// <summary>
        /// 解析数据格式08.
        /// <param name="strData">接收帧字符串长度:2个字符,</param>
        /// <returns>返回不带符号的整数,最大值4位数</returns>
        /// 当参数无效时,返回默认值-999999
        /// </summary> 
        public static string gd8(string strData)
        {
            int intSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    intSum = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1))
                        + int.Parse(strData.Substring(2, 1)) * 1000 + int.Parse(strData.Substring(3, 1)) * 100;
                    return intSum.ToString();
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return int.Parse(strDefValu); 
                }
            }
        }

        /// <summary>
        /// 解析数据格式09.
        /// <param name="strData">接收帧字符串长度:8个字符,</param>
        /// <returns> 带符号的浮点数,返回值整数位最高到十位,小数位精度到百分位</returns>
        ///  当参数无效时,返回默认值-999999 
        /// </summary> 
        public static string gd9(string strData)
        {
            float fltSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    fltSum = Convert.ToInt32(strData.Substring(0, 1), 16) / 1000f + Convert.ToInt32(strData.Substring(1, 1), 16) / 10000f
                    + Convert.ToInt32(strData.Substring(2, 1), 16) / 10f + Convert.ToInt32(strData.Substring(3, 1), 16) / 100f
                    + (Convert.ToInt32((strData.Substring(4, 1)), 16) & 7) * 10 + Convert.ToInt32(strData.Substring(5, 1), 16);

                    if ((Convert.ToInt32((strData.Substring(4, 1)), 16) & 8) == 8)
                    {
                        return (-fltSum).ToString();
                    }
                    else
                    {
                        return fltSum.ToString();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return float.Parse(strDefValu);
                }

            }
        }

        /// <summary>
        /// 解析数据格式10.
        /// <param name="strData">接收帧字符串长度:8个字符,</param>
        /// <returns>返回不带符号的整数,最大值6位数</returns>
        /// 当参数无效时,返回默认值-999999
        /// </summary> 
        public static string gd10(string strData)
        {
            int intSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    intSum = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1))
                        + int.Parse(strData.Substring(2, 1)) * 1000 + int.Parse(strData.Substring(3, 1)) * 100
                        + int.Parse(strData.Substring(4, 1)) * 100000 + int.Parse(strData.Substring(5, 1)) * 10000;
                    return intSum.ToString();
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return int.Parse(strDefValu);
                }

            }
        }

        /// <summary>
        /// 解析数据格式11.
        /// <param name="strData">接收帧字符串长度:11个字符,</param>
        /// <returns>不带符号的浮点数,返回值整数位最高到十万位,小数位精度到百分位</returns>
        /// 当参数无效时,返回默认值-999999
        /// </summary> 
        public static string gd11(string strData)
        {
            double dblSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    dblSum = double.Parse(strData.Substring(0, 1)) / 10 + double.Parse(strData.Substring(1, 1)) / 100
                        + int.Parse(strData.Substring(2, 1)) * 10 + int.Parse(strData.Substring(3, 1))
                        + int.Parse(strData.Substring(4, 1)) * 1000 + int.Parse(strData.Substring(5, 1)) * 100
                        + int.Parse(strData.Substring(6, 1)) * 100000 + int.Parse(strData.Substring(7, 1)) * 10000;
                    return dblSum.ToString();
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return float.Parse(strDefValu);
                }


            }
        }

        /// <summary>
        /// 解析数据格式12.
        /// <param name="strData">接收帧字符串长度:17个字符,</param>
        /// <returns>返回不带符号的整数,最大值12位数</returns>
        /// 当参数无效时,返回默认值-999999
        /// </summary> 
        public static string gd12(string strData)
        {
            long lngSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    lngSum = long.Parse(strData.Substring(0, 1)) * 10 + long.Parse(strData.Substring(1, 1))
                        + long.Parse(strData.Substring(2, 1)) * 1000 + long.Parse(strData.Substring(3, 1)) * 100
                        + long.Parse(strData.Substring(4, 1)) * 100000 + long.Parse(strData.Substring(5, 1)) * 10000
                        + long.Parse(strData.Substring(6, 1)) * 10000000 + long.Parse(strData.Substring(7, 1)) * 1000000
                        + long.Parse(strData.Substring(8, 1)) * 1000000000 + long.Parse(strData.Substring(9, 1)) * 100000000
                        + long.Parse(strData.Substring(10, 1)) * 100000000000 + long.Parse(strData.Substring(11, 1)) * 10000000000;
                    return lngSum.ToString();
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return long.Parse(strDefValu);
                }
            }
        }

        /// <summary>
        /// 解析数据格式13.
        /// <param name="strData">接收帧字符串长度:11个字符,</param>
        /// <returns>不带符号的浮点数,返回值整数位最高到千位,小数位精度到万分位</returns>
        /// 当参数无效时,返回默认值-999999
        /// </summary> 
        public static string gd13(string strData)
        {
            double dblSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    dblSum = double.Parse(strData.Substring(0, 1)) / 1000 + double.Parse(strData.Substring(1, 1)) / 10000
                        + double.Parse(strData.Substring(2, 1)) / 10 + double.Parse(strData.Substring(3, 1)) / 100
                        + int.Parse(strData.Substring(4, 1)) * 10 + int.Parse(strData.Substring(5, 1))
                        + int.Parse(strData.Substring(6, 1)) * 1000 + int.Parse(strData.Substring(7, 1)) * 100;
                    return dblSum.ToString();
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return float.Parse(strDefValu);
                }

            }
        }

        /// <summary>
        /// 解析数据格式14.
        /// <param name="strData">接收帧字符串长度:14个字符,</param>
        /// <returns>不带符号的浮点数,返回值整数位最高到十万位,小数位精度到万分位</returns>
        /// 当参数无效时,返回默认值-999999
        /// </summary> 
        public static string gd14(string strData)
        {
            double dblSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    dblSum = double.Parse(strData.Substring(0, 1)) / 1000 + double.Parse(strData.Substring(1, 1)) / 10000
                        + double.Parse(strData.Substring(2, 1)) / 10 + double.Parse(strData.Substring(3, 1)) / 100
                        + int.Parse(strData.Substring(4, 1)) * 10 + int.Parse(strData.Substring(5, 1))
                        + int.Parse(strData.Substring(6, 1)) * 1000 + int.Parse(strData.Substring(7, 1)) * 100
                        + int.Parse(strData.Substring(8, 1)) * 100000 + int.Parse(strData.Substring(9, 1)) * 10000;
                    return dblSum.ToString();
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return float.Parse(strDefValu);
                }

            }
        }

        /// <summary>
        /// 解析数据格式15.
        /// </summary>
        /// <param name="strData">接收帧字符串长度:14个字符</param>
        /// <returns>返回表示年 月 日 时 分 的符串</returns>
        /// 当参数无效时,返回默认值“0-0-0 0:0:0”        
        public static string gd15(string strData)
        {
            int year, month, day, hour, minute;
            string strDateStr;

            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    minute = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1));
                    hour = int.Parse(strData.Substring(2, 1)) * 10 + int.Parse(strData.Substring(3, 1));
                    day = int.Parse(strData.Substring(4, 1)) * 10 + int.Parse(strData.Substring(5, 1));
                    month = int.Parse(strData.Substring(6, 1)) * 10 + int.Parse(strData.Substring(7, 1));
                    year = int.Parse(strData.Substring(8, 1)) * 10 + int.Parse(strData.Substring(9, 1));

                    strDateStr = year.ToString("00") + "-" + month.ToString("00") + "-" + day.ToString("00")
                                 + " " + hour.ToString("00") + ":" + minute.ToString("00");
                    return strDateStr;

                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return strDefDate;
                }

            }
        }

        /// <summary>
        /// 解析数据格式16.
        /// <param name="strData">接收帧字符串长度:11个字符,</param>
        /// <returns>返回表示 日 时 分 秒的符串</returns>
        /// 当参数无效时,返回默认值“0-0-0 0:0:0”
        /// </summary>
        public static string gd16(string strData)
        {
            int day, hour, minute, second;
            string strDateStr;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    second = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1));
                    minute = int.Parse(strData.Substring(2, 1)) * 10 + int.Parse(strData.Substring(3, 1));
                    hour = int.Parse(strData.Substring(4, 1)) * 10 + int.Parse(strData.Substring(5, 1));
                    day = int.Parse(strData.Substring(6, 1)) * 10 + int.Parse(strData.Substring(7, 1));

                    strDateStr = day.ToString() + " " + hour.ToString() + ":" + minute.ToString()
                                               + ":" + second.ToString();
                    return strDateStr;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return strDefDate;
                }
            }
        }



        public static string gd17(string Tstr)
        {
            string Ts = "";
            if (Tstr.Substring(0, 4) == "EEEE" || Tstr.Substring(0, 4) == "FFFF") return "2099-1-1";
            Ts = ":00";
            Ts = (Convert.ToInt16(Tstr.Substring(0, 1)) * 10 + Convert.ToInt16(Tstr.Substring(1, 1))).ToString() + Ts;
            Ts = (Convert.ToInt16(Tstr.Substring(2, 1)) * 10 + Convert.ToInt16(Tstr.Substring(3, 1))).ToString() + ":" + Ts;
            Ts = (Convert.ToInt16(Tstr.Substring(4, 1)) * 10 + Convert.ToInt16(Tstr.Substring(5, 1))).ToString() + " " + Ts;
            Ts = (Convert.ToInt16(Tstr.Substring(6, 1)) * 10 + Convert.ToInt16(Tstr.Substring(7, 1))).ToString() + "-" + Ts;
            return Ts;
        }

        /// <summary>
        /// 解析数据格式18.
        /// <param name="strData">接收帧字符串长度:8个字符,</param>
        /// <returns>返回表示 日 时 分 的符串</returns>
        /// 当参数无效时,返回默认值“0-0-0 0:0:0”
        /// </summary>
        public static string gd18(string strData)
        {
            int day, hour, minute;
            int intStrLen;
            string strDateStr;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            if (strData.Substring(0, 4) == "FFFFFF")
            {
                return "FF日FF:FF";
            }
            else
            {
                try
                {
                    minute = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1));
                    hour = int.Parse(strData.Substring(2, 1)) * 10 + int.Parse(strData.Substring(3, 1));
                    day = int.Parse(strData.Substring(4, 1)) * 10 + int.Parse(strData.Substring(5, 1));

                    strDateStr = day.ToString("00") + ":" + hour.ToString("00") + ":" + minute.ToString("00");

                    return strDateStr;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return strDefDate;
                }
            }
        }

        /// <summary>
        /// 解析数据格式19.
        /// <param name="strData">接收帧字符串长度:5个字符,</param>
        /// <returns>返回表示 时 分 的符串</returns>
        /// 当参数无效时,返回默认值“0-0-0 0:0:0”
        /// </summary>
        public static string gd19(string strData)
        {
            int hour, minute;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            if (strData.Substring(0, 4) == "FFFF")
            {
                return "FF:FF";
            }
            else
            {
                try
                {
                    minute = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1));
                    hour = int.Parse(strData.Substring(2, 1)) * 10 + int.Parse(strData.Substring(3, 1));
                    return hour.ToString("00") + ":" + minute.ToString("00");
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return strDefDate;
                }
            }
        }

        /// <summary>
        /// 解析数据格式20.
        /// <param name="strData">接收帧字符串长度:8个字符,</param>
        /// <returns>返回表示 年 月 日 的符串</returns>
        /// 当参数无效时,返回默认值“0-0-0 0:0:0”
        /// </summary>
        public static string gd20(string strData)
        {
            int year, month, day;
            string strDateStr;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    day = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1));
                    month = int.Parse(strData.Substring(2, 1)) * 10 + int.Parse(strData.Substring(3, 1));
                    year = int.Parse(strData.Substring(4, 1)) * 10 + int.Parse(strData.Substring(5, 1));


                    strDateStr = year.ToString("00") + "-" + month.ToString() + "-" + day.ToString();

                    return strDateStr;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return strDefDate;
                }
            }
        }

        /// <summary>
        /// 解析数据格式21.
        /// <param name="strData">接收帧字符串长度:5个字符,</param>
        /// <returns>返回表示 年 月 的符串</returns>
        /// 当参数无效时,返回默认值“0-0-0 0:0:0”
        /// </summary>
        public static string gd21(string strData)
        {
            int year, month;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    month = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1));
                    year = int.Parse(strData.Substring(2, 1)) * 10 + int.Parse(strData.Substring(3, 1));

                    return year.ToString("00") + "-" + month.ToString();
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return strDefDate;
                }
            }
        }

        /// <summary>
        /// 解析数据格式22.
        /// <param name="strData">接收帧字符串长度:2个字符,</param>
        /// <returns>不带符号的浮点数,返回值整数位最高到个位,小数位精度到十分位</returns>
        /// 当参数无效时,返回默认值-999999
        /// </summary> 
        public static string gd22(string strData)
        {
            float fltSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 2) == "EE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    fltSum = int.Parse(strData.Substring(0, 1)) + float.Parse(strData.Substring(1, 1)) / 10;

                    return fltSum.ToString();
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return float.Parse(strDefValu);
                }

            }
        }

        /// <summary>
        /// 解析数据格式23.
        /// <param name="strData">接收帧字符串长度:6个字符,</param>
        /// <returns>不带符号的浮点数,返回值整数位最高到十位,小数位精度到百分位</returns>
        /// 当参数无效时,返回默认值-999999
        /// </summary> 
        public static string gd23(string strData)
        {
            float fltSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    fltSum = float.Parse(strData.Substring(0, 1)) / 1000 + float.Parse(strData.Substring(1, 1)) / 10000
                        + float.Parse(strData.Substring(2, 1)) / 10 + float.Parse(strData.Substring(3, 1)) / 100
                        + int.Parse(strData.Substring(4, 1)) * 10 + int.Parse(strData.Substring(5, 1));

                    return fltSum.ToString();
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return float.Parse(strDefValu);
                }
            }
        }

        /// <summary>
        /// 解析数据格式24.
        /// <param name="strData">接收帧字符串长度:5个字符,</param>
        /// <returns>返回表示 日 时 的符串</returns>
        /// 当参数无效时,返回默认值“0-0-0 0:0:0”
        /// </summary>
        public static string gd24(string strData)
        {
            int day, hour;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    hour = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1));
                    day = int.Parse(strData.Substring(2, 1)) * 10 + int.Parse(strData.Substring(3, 1));

                    return day.ToString() + "日" + hour.ToString() + "时";
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return strDefDate;
                }
            }
        }

        /// <summary>
        /// 解析数据格式25.
        /// <param name="strData">接收帧字符串长度:8个字符,</param>
        /// <returns>带符号的浮点数,返回值整数位最高到百位,小数位精度到千分位</returns>
        /// 当参数无效时,返回默认值-999999
        /// </summary> 
        public static string gd25(string strData)
        {
            float fltSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    fltSum = float.Parse(strData.Substring(0, 1)) / 100 + float.Parse(strData.Substring(1, 1)) / 1000
                        + float.Parse(strData.Substring(2, 1)) + float.Parse(strData.Substring(3, 1)) / 10
                        + (Convert.ToInt32((strData.Substring(4, 1)), 16) & 7) * 100 + int.Parse(strData.Substring(5, 1)) * 10;

                    if ((Convert.ToInt32((strData.Substring(4, 1)), 16) & 8) == 8)
                    {
                        return (-fltSum).ToString();
                    }
                    else
                    {
                        return fltSum.ToString();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return float.Parse(strDefValu);
                }
            }
        }

        /// <summary>
        /// 解析数据格式26.
        /// <param name="strData">接收帧字符串长度:5个字符,</param>
        /// <returns>不带符号的浮点数,返回值整数位最高到个位,小数位精度到千分位</returns>
        /// 当参数无效时,返回默认值-999999
        /// </summary> 
        public static string gd26(string strData)
        {
            float fltSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    fltSum = float.Parse(strData.Substring(0, 1)) / 100 + float.Parse(strData.Substring(1, 1)) / 1000
                        + float.Parse(strData.Substring(2, 1)) + float.Parse(strData.Substring(3, 1)) / 10;

                    return fltSum.ToString();
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return float.Parse(strDefValu);
                }
            }
        }

        /// <summary>
        /// 解析数据格式27.
        /// <param name="strData">接收帧字符串长度:11个字符,</param>
        /// <returns>返回不带符号的整数,最大值8位数</returns>
        /// 当参数无效时,返回默认值 -999999
        /// </summary> 
        public static string gd27(string strData)
        {
            int intSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    intSum = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1))
                        + int.Parse(strData.Substring(2, 1)) * 1000 + int.Parse(strData.Substring(3, 1)) * 100
                        + int.Parse(strData.Substring(4, 1)) * 100000 + int.Parse(strData.Substring(5, 1)) * 10000
                        + int.Parse(strData.Substring(6, 1)) * 10000000 + int.Parse(strData.Substring(7, 1)) * 1000000;

                    return intSum.ToString();
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return int.Parse(strDefValu);
                }
            }
        }

        /// <summary>
        /// 解析数据格式27.
        /// <param name="strData">接收帧字符串长度:11个字符,</param>
        /// <returns>返回不带符号的整数,最大值8位数</returns>
        /// 当参数无效时,返回默认值 -999999
        /// </summary> 
        public static string gd28(string strData)
        {
            double intSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    intSum = int.Parse(strData.Substring(0, 1)) * 0.1 + int.Parse(strData.Substring(1, 1)) * 0.01
                        + int.Parse(strData.Substring(2, 1)) * 10 + int.Parse(strData.Substring(3, 1))
                        + int.Parse(strData.Substring(4, 1)) * 1000 + int.Parse(strData.Substring(5, 1)) * 100;

                    //intSum = int.Parse(strData.Substring(0, 1)) * 0.1 + int.Parse(strData.Substring(1, 1)) * 0.01
                    //    + int.Parse(strData.Substring(2, 1)) * 10 + int.Parse(strData.Substring(3, 1))
                    //    +Convert.ToDouble( int.Parse(strData.Substring(4, 1)) * 1000) +Convert.ToDouble(  int.Parse(strData.Substring(5, 1)) * 100);

                    return intSum.ToString();
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return int.Parse(strDefValu);
                }
            }
        }

        public static int ShiLiuToShi(string x)
        {
            string[] sw = new string[2];
            if (x.Length == 1) x += "0"; x = x.ToUpper();
            sw[0] = x.Substring(0, 1); sw[1] = x.Substring(1, 1);
            if (sw[0] == "A") sw[0] = "10";
            else if (sw[0] == "B") sw[0] = "11";
            else if (sw[0] == "C") sw[0] = "12";
            else if (sw[0] == "D") sw[0] = "13";
            else if (sw[0] == "E") sw[0] = "14";
            else if (sw[0] == "F") sw[0] = "15";
            if (sw[1] == "A") sw[1] = "10";
            else if (sw[1] == "B") sw[1] = "11";
            else if (sw[1] == "C") sw[1] = "12";
            else if (sw[1] == "D") sw[1] = "13";
            else if (sw[1] == "E") sw[1] = "14";
            else if (sw[1] == "F") sw[1] = "15";
            return Convert.ToByte(sw[0]) * 16 + Convert.ToByte(sw[1]);
        }

        private static int GetZhiShu(int tz)
        {
            switch (tz)
            {
                case 0:
                    return 0;
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 4:
                    return 3;
                case 8:
                    return 4;
                case 16:
                    return 5;
                case 32:
                    return 6;
                case 64:
                    return 7;
                case 128:
                    return 8;
            }
            return 0;
        }

        public static int ExplanP(string p)
        {
            int DA1 = Convert.ToInt32(p.Substring(0, 2), 2);
            int DA2 = Convert.ToInt32(p.Substring(2, 2), 2);
            int pn = 0;
            if (DA1 == 0 & DA1 == 0)
            {
                pn = 0;
            }
            else
            {
                pn = (DA2 - 1) * 8 + (int)(Math.Log(DA1, 2.0) + 1);
            }

            //int tint; int tint2;
            //tint = GetZhiShu(ShiLiuToShi(p.Substring(0, 2)));
            //if (tint == 0) return 0;
            //tint2 = GetZhiShu(ShiLiuToShi(p.Substring(2, 2)));

            return pn;//(tint2 - 1) * 8 + tint;
        }

        public static int ExplanF(string F)
        {
            int DT1 = Convert.ToInt32(F.Substring(0, 2), 16);
            int DT2 = Convert.ToInt32(F.Substring(2, 2), 16);
            int fn = DT2 * 8 + (int)(Math.Log(DT1, 2.0) + 1);

            //int tint; int tint2;
            //tint = GetZhiShu(ShiLiuToShi(F.Substring(0, 2)));
            //if (tint == 0) return 0;
            //tint2 = ShiLiuToShi(F.Substring(2, 2));

            return fn;//tint2 * 8 + tint;
        }



        public string ExplanAddrFun(string strData)
        {
            string tempAddr = "";
            for (int i = strData.Length / 2; i > 0; i--)
            {
                tempAddr += strData.Substring((i - 1) * 2, 2);
            }
            return tempAddr;
        }

        public byte[] OrgAddrFun(string strData, int len)
        {
            List<byte> tempData = new List<byte>();
            strData = strData.PadLeft(len * 2, '0');
            for (int i = len; i > 0; i--)
            {
                var data = strData.Substring(i * 2 - 2, 2);
                tempData.Add(Convert.ToByte(data));
            }
            return tempData.ToArray();
        }

        public static int GetAnyValue(int tvalue, int startp, int stopp)
        {
            int iint = 0;
            iint = tvalue % Convert.ToInt16(Math.Pow(2, (stopp + 1)));
            iint = iint / Convert.ToInt16(Math.Pow(2, startp));
            if (iint < 0) iint = 0;
            return iint;
        }


        /// <summary>
        /// 解析Bin类型NN
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static int ExplanBinFun(string strData)
        {
            double tempBin = 0;
            for (int i = 0; i < strData.Length / 2; i++)
            {
                tempBin += Convert.ToInt16(strData.Substring(i * 2, 2), 16) * Math.Pow(256, i);
            }
            if (strData.Length == 1)
            {
                tempBin += Convert.ToInt16(strData.Substring(0, 1), 16);
            }
            return Convert.ToInt32(tempBin);
        }

        /// <summary>
        /// 解析Bin类型NN
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static int ExplanBinFun(ref string strData, int ilen)
        {
            double tempBin = 0;
            string strls = strData.Substring(0, ilen);
            for (int i = 0; i < strls.Length / 2; i++)
            {
                tempBin += Convert.ToInt16(strls.Substring(i * 2, 2), 16) * Math.Pow(256, i);
            }
            if (strls.Length == 1)
            {

                tempBin += Convert.ToInt16(strls.Substring(0, 1), 16);
            }
            strData = strData.Substring(ilen);
            return Convert.ToInt32(tempBin);
        }

        /// <summary>
        /// 解析数据格式11.
        /// <param name="strData">接收帧字符串长度:11个字符,</param>
        /// <returns>不带符号的浮点数,返回值整数位最高到十万位,小数位精度到百分位</returns>
        /// 当参数无效时,返回默认值-999999
        /// </summary> 
        public static string gd11(ref string strData, int ilen)
        {
            double dblSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                strData = strData.Substring(ilen);
                return "99999.9";
            }
            else
            {
                try
                {

                    dblSum = double.Parse(strData.Substring(0, 1)) / 10 + double.Parse(strData.Substring(1, 1)) / 100
                        + int.Parse(strData.Substring(2, 1)) * 10 + int.Parse(strData.Substring(3, 1))
                        + int.Parse(strData.Substring(4, 1)) * 1000 + int.Parse(strData.Substring(5, 1)) * 100
                        + int.Parse(strData.Substring(6, 1)) * 100000 + int.Parse(strData.Substring(7, 1)) * 10000;
                    strData = strData.Substring(ilen);
                    return dblSum.ToString();
                }
                catch (Exception)
                {
                    throw new Exception("参数有误,无法解析");
                    //return float.Parse(strDefValu);
                }


            }
        }

        /// <summary>
        /// 解析成二进制
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static string ExplanWei(string strData)
        {
            string str1 = ""; string str2 = "";
            str1 = Convert.ToString(Convert.ToInt16(strData, 16), 2).PadLeft(8, '0');
            for (int i = 0; i < 8; i++)
            {
                str2 += str1.Substring(i, 1) + " ";
            }
            return str2;
        }

        /// <summary>
        /// 字符串转数字 XXXX
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="Pdot">小数位数</param>
        /// <returns></returns>
        public static string GetDnData(string str, int Pdot)
        {
            str = Reduce33H(str); double i1 = 0;
            for (int i = 0; i < str.Length; i++)
            {
                i1 += Convert.ToInt16(str.Substring(i, 1)) * Math.Pow(10, str.Length - 1 - i - Pdot);
            }
            return i1.ToString();
        }


        /// <summary>
        /// 传递5个字节  YYMMDDhhmm
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetDate1(string str)
        {
            str = Reduce33H(str);
            if (str.Substring(0, 2) == "00")
                return str.Substring(0, 2) + "-" + str.Substring(2, 2) + "-" + str.Substring(4, 2) + " " + str.Substring(6, 2) + ":" + str.Substring(8, 2);
            else
                return "20" + str.Substring(0, 2) + "-" + str.Substring(2, 2) + "-" + str.Substring(4, 2) + " " + str.Substring(6, 2) + ":" + str.Substring(8, 2);
        }

        /// <summary>
        /// 传递6个字节  YYMMDDhhmmss
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetDate2(string str)
        {
            str = Reduce33H(str);
            if (str.Substring(0, 2) == "00")
                return str.Substring(0, 2) + "-" + str.Substring(2, 2) + "-" + str.Substring(4, 2) + " " + str.Substring(6, 2) + ":" + str.Substring(8, 2) + ":" + str.Substring(10, 2);
            else
                return "20" + str.Substring(0, 2) + "-" + str.Substring(2, 2) + "-" + str.Substring(4, 2) + " " + str.Substring(6, 2) + ":" + str.Substring(8, 2) + ":" + str.Substring(10, 2);
        }

        /// <summary>
        /// 传递4个字节  MMDDhhmm
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetDate3(string str)
        {
            str = Reduce33H(str);
            return str.Substring(0, 2) + "-" + str.Substring(2, 2) + " " + str.Substring(4, 2) + ":" + str.Substring(6, 2);
        }

        /// <summary>
        /// 传递3个字节  YYMMDD
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetDate4(string str)
        {
            str = Reduce33H(str);
            if (str.Substring(0, 2) == "00")
                return str.Substring(0, 2) + "-" + str.Substring(2, 2) + "-" + str.Substring(4, 2);
            else
                return "20" + str.Substring(0, 2) + "-" + str.Substring(2, 2) + "-" + str.Substring(4, 2);
        }

        /// <summary>
        /// 传递3个字节  hhmmss
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetDate5(string str)
        {
            str = Reduce33H(str);
            return str.Substring(0, 2) + ":" + str.Substring(2, 2) + ":" + str.Substring(4, 2);
        }

        /// <summary>
        /// 传递2个字节  DDhh
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetDate6(string str)
        {
            str = Reduce33H(str);
            return str.Substring(0, 2) + "日" + str.Substring(2, 2) + "时";
        }

        /// <summary>
        /// 传递2个字节  DDhh
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetDate6_(string str)
        {
            return str.Substring(0, 2) + "日" + str.Substring(2, 2) + "时";
        }

        /// <summary>
        /// 传递2个字节  MMDD
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetDate7(string str)
        {
            str = Reduce33H(str);
            return str.Substring(0, 2) + "-" + str.Substring(2, 2);
        }

        /// <summary>
        /// 传递2个字节  hhmm
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetDate8(string str)
        {
            str = Reduce33H(str);
            return str.Substring(0, 2) + ":" + str.Substring(2, 2);
        }

        /// <summary>
        /// 传递3个字节  DDhhmm
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetDate9(string str)
        {
            str = Reduce33H(str);
            return str.Substring(0, 2) + "日" + str.Substring(2, 2) + "时" + str.Substring(4, 2) + "分";
        }

        /// <summary>
        /// 传递3个字节  DDhhmm
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetDate9_(string str)
        {
            return str.Substring(0, 2) + "日" + str.Substring(2, 2) + "时" + str.Substring(4, 2) + "分";
        }

        /// <summary>
        /// 获取星期
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetXQ(string str)
        {
            str = Reduce33H(str);
            return str;
            //switch (Convert.ToInt16(str, 16))
            //{
            //    case 1:
            //        return "星期一";
            //    case 2:
            //        return "星期二";
            //    case 3:
            //        return "星期三";
            //    case 4:
            //        return "星期四";
            //    case 5:
            //        return "星期五";
            //    case 6:
            //        return "星期六";
            //    default:
            //        return "星期日";
            //}
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

        /// <summary>
        /// 减33H,反转字符串
        /// </summary>
        /// <param name="Pstr"></param>
        /// <returns></returns>
        public static string Reduce33H(string Pstr)
        {
            try
            {
                int Ti;
                int Tlen;
                string Tstr;
                Tlen = Pstr.Length;
                Tstr = "";
                for (Ti = 1; Ti <= Tlen; Ti = Ti + 2)
                    Tstr = Tstr + string.Format("{0:X2}", (TentoSix(Pstr.Substring(Ti - 1, 2)) - 0x33 + 256) % 0x100);

                string str = Tstr.Replace(" ", ""); string str2 = "";
                if (str.Length > 0)
                {
                    if (str.Length % 2 == 0)
                    {
                        for (int i = 0; i < Convert.ToInt32(str.Length) / 2; i++)
                            str2 += str.Substring(str.Length - 2 - i * 2, 2);
                    }
                }

                return str2;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 减33H
        /// </summary>
        /// <param name="Pstr"></param>
        /// <returns></returns>
        public static string Reduce33H1(string Pstr)
        {
            try
            {
                int Ti;
                int Tlen;
                string Tstr;
                Tlen = Pstr.Length;
                Tstr = "";
                for (Ti = 1; Ti <= Tlen; Ti = Ti + 2)
                    Tstr = Tstr + string.Format("{0:X2}", (TentoSix(Pstr.Substring(Ti - 1, 2)) - 0x33 + 256) % 0x100);

                string str = Tstr.Replace(" ", ""); string str2 = "";
                if (str.Length > 0)
                {
                    if (str.Length % 2 == 0)
                    {
                        for (int i = 0; i < Convert.ToInt32(str.Length) / 2; i++)
                            str2 += str.Substring(i * 2, 2);
                    }
                }

                return str2;
            }
            catch
            {
                return null;
            }
        }

        public static byte TentoSix(string Ten)
        {
            byte tb; string Ts; string Tg;
            Ts = "";
            Ts = Ten.Substring(0, 1).ToUpper();
            Ts = TentoSinD(Ts);
            Tg = Ten.Substring(1, 1).ToUpper();
            Tg = TentoSinD(Tg);

            tb = (byte)(byte.Parse(Ts) * 16 + byte.Parse(Tg));
            return tb;
        }

        private static string TentoSinD(string Ten)
        {
            string Tstr;
            switch (Ten)
            {
                case "A":
                    Tstr = "10";
                    break;
                case "B":
                    Tstr = "11";
                    break;
                case "C":
                    Tstr = "12";
                    break;
                case "D":
                    Tstr = "13";
                    break;
                case "E":
                    Tstr = "14";
                    break;
                case "F":
                    Tstr = "15";
                    break;
                default:
                    Tstr = Ten;
                    break;
            }
            return Tstr;
        }
    }
}
