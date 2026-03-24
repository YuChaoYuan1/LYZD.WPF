using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.TerminalProtocol.GW
{
    //376.1附录A的数据格式
    //zzf 添加
    public class FrameDataFormtUtil
    {
        private byte[] byteAry;
        private string strDefValu = "-999999";//表示默认的无效值
        private string strDefDate = "00-00-00 00:00:00";//表示默认的无效日期返回值        
        private string strVoid = "eeeeeeeeeeeeeeeeeeee";//表示报文中的无效数据

        /// <summary>
        /// 组织数据格式01. 
        /// </summary>
        /// <param name="year"> year:1~9999</param>
        /// <param name="month"> month:1~12</param>
        /// <param name="day"> day:1~对应月的天数</param>
        /// <param name="hour"> hour:0~23</param>
        /// <param name="minute"> minnte:0~59</param>
        /// <param name="second"> second:0~59</param>
        /// <returns> 当参数无效时，返回数组默认值0</returns>        
        public List<byte> OrgFormat01(int year, int month, int day, int hour, int minute, int second)
        {
            int intDOfWeek;
            byteAry = new byte[6] { 0, 0, 0, 0, 0, 0 };
            if (year.ToString().Length == 2)    //如果只输入如年份的后两位数字，
            {                                   //默认按照20XX年来处理. 
                year = 2000 + year;
            }

            try
            {
                DateTime dt = new DateTime(year, month, day, hour, minute, second);
                intDOfWeek = (int)dt.DayOfWeek;

                if (intDOfWeek == 0)
                {
                    intDOfWeek = 7;
                }

                byteAry[0] = (byte)((second / 10) * 16 + second % 10);
                byteAry[1] = (byte)((minute / 10) * 16 + minute % 10);
                byteAry[2] = (byte)((hour / 10) * 16 + hour % 10);
                byteAry[3] = (byte)((day / 10) * 16 + day % 10);
                byteAry[4] = (byte)(intDOfWeek * 32 + (month / 10) * 16 + month % 10);
                byteAry[5] = (byte)((year % 100 / 10) * 16 + year % 10);

                return ConvertByteToList(byteAry);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                throw new Exception("输入数值不在处理范围之内");
            }
            catch (Exception)
            {

                return ConvertByteToList(byteAry);
            }

        }

        /// <summary>
        /// 解析数据格式01.
        /// </summary>
        /// <param name="strData">接收帧字符串长度：17个字符，</param>
        /// <returns>返回日期格式的字符串(y-m-d hh:mm:ss),当接受参数无效时，返回默认日期“0-0-0 0:0:0”.</returns>        
        public string ExplanFormat01(string strData)
        {
            int year, month, day, hour, minute, second;
            string strDateStr;//日期字符串
            int intStrLen;//字符串长度

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return strDefDate;
            }
            else
            {
                try
                {
                    second = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1));
                    minute = int.Parse(strData.Substring(2, 1)) * 10 + int.Parse(strData.Substring(3, 1));
                    hour = int.Parse(strData.Substring(4, 1)) * 10 + int.Parse(strData.Substring(5, 1));
                    day = int.Parse(strData.Substring(6, 1)) * 10 + int.Parse(strData.Substring(7, 1));
                    month = (Convert.ToInt32((strData.Substring(8, 1)), 16) & 1) * 10 + int.Parse(strData.Substring(9, 1));
                    year = int.Parse(strData.Substring(10, 1)) * 10 + int.Parse(strData.Substring(11, 1));

                    strDateStr = "20" + year.ToString("00") + "-" + month.ToString("00") + "-" + day.ToString("00") + " "
                              + hour.ToString("00") + ":" + minute.ToString("00") + ":" + second.ToString("00");
                    return strDateStr;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return strDefDate;
                }
            }
        }

        /// <summary>
        /// 组织数据格式02. 
        /// 接收科学记数法参数
        /// <param name="strData"> strData参数格式:aEb，a范围(-999~+999),b范围(-3~4)，E代表科学记数法</param>
        /// </summary>
        public List<byte> OrgFormat02(string strData)
        {
            string strA;//代表整数部分
            string strB;//代表幂
            int s;//符号标志
            int hundreds, tens, units;////百位，十位，个位
            int intPos;//检索位置
            int intA;//代表整数部分
            int intB;//代表幂


            byteAry = new byte[2];
            strData = strData.Trim().ToUpper();
            intPos = strData.IndexOf('E');
            if (intPos == -1 | intPos == 0)
            {
                throw new Exception("输入参数有误，应输入格式:aEb");
            }
            strA = strData.Substring(0, intPos);
            strB = strData.Substring(intPos + 1);

            if (strB == "")
            {
                throw new Exception("输入参数有误，应输入格式:aEb");
            }
            try
            {
                intA = int.Parse(strA);
                intB = int.Parse(strB);
                s = 0;
                if (intA < 0)
                {
                    s = 1;//表示为负数
                    intA = Math.Abs(intA);
                }
            }
            catch (Exception)
            {
                throw new Exception("请输入参数格式aEb，a整数范围(-999~+999),b整数范围(-3~4)，E代表科学记数法");
            }
            if (intA > 999 || intA < 0 || intB > 4 || intB < -3)
            {
                throw new Exception("请检查输入参数的范围，a(-999~+999),b(-3~4)");
            }

            units = intA % 10;
            tens = intA % 100 / 10;
            hundreds = intA % 1000 / 100;
            byteAry[0] = (byte)(tens * 16 + units);
            byteAry[1] = (byte)((4 - intB) * 32 + hundreds);
            if (s == 1)
                byteAry[1] = (byte)(16 + byteAry[1]);

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式02. 
        /// <param name="strData">接收帧字符串长度：5个字符，</param>
        /// 当接受参数无效时返回默认值 -99999
        /// </summary> 
        public float ExplanFormat02(string strData)
        {
            int fltSum;//求和
            int y;//次幂
            int intStrLen;//字符串长度

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen) ||
                strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen).ToUpper())
            {
                return float.Parse(strDefValu);
            }
            else
            {
                try
                {
                    fltSum = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1))
                          + int.Parse(strData.Substring(3, 1)) * 100;

                    y = 4 - Convert.ToInt32((strData.Substring(2, 1)), 16) / 2;//次幂

                    if ((Convert.ToInt32((strData.Substring(2, 1)), 16) & 1) == 0)//s=1代表有符号
                    {
                        return (float)(fltSum * Math.Pow(10, y));
                    }
                    else
                    {
                        return (float)(-fltSum * Math.Pow(10, y));
                    }
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return float.Parse(strDefValu);
                }

            }
        }

        ///<summary>
        /// 组织数据格式03. 
        /// <param name="intData">参数intData范围（- 9999999~ + 9999999）</param>
        /// <param name="bytG">bytG=0表示单位：kWh、厘，bytG=1表示单位：MWh、元,其他值无效</param>
        ///</summary>
        public List<byte> OrgFormat03(int intData, int bytG)
        {
            string strIntStr;
            int millions, hunThous, tenThous, thousands, hundreds,//百万位，十万位，万位，千位，百位
                                               tens, units;//十位，个位 
            int s;//表示符号
            int g;//表示单位

            byteAry = new byte[4] { 0, 0, 0, 0 };
            if (intData < -9999999 || intData > 9999999)
            {
                throw new Exception("输入数值不在处理范围之内");
            }
            //处理单位标志
            if (bytG == 0)
            {
                g = 0;
            }
            else if (bytG == 1)
            {
                g = 1;
            }
            else
            {
                throw new Exception("输入单位标志有误，有效值为0 和 1");
            }
            //处理符号位标志
            if (intData < 0)
            {
                s = 1;
                intData = (int)Math.Abs(intData);
            }
            else
            {
                s = 0;
            }

            strIntStr = intData.ToString();
            strIntStr = "0000000".Substring(0, 7 - strIntStr.Length) + strIntStr;//不足7位，前面补0

            units = int.Parse(strIntStr.Substring(6, 1));//个位           
            tens = int.Parse(strIntStr.Substring(5, 1));//十位
            hundreds = int.Parse(strIntStr.Substring(4, 1));//百位
            thousands = int.Parse(strIntStr.Substring(3, 1));//千位
            tenThous = int.Parse(strIntStr.Substring(2, 1));//万位           
            hunThous = int.Parse(strIntStr.Substring(1, 1));//十万位
            millions = int.Parse(strIntStr.Substring(0, 1));//百万位

            byteAry[0] = (byte)(tens * 16 + units);
            byteAry[1] = (byte)(thousands * 16 + hundreds);
            byteAry[2] = (byte)(hunThous * 16 + tenThous);
            if (g == 0)
            {
                if (s == 0)
                {
                    byteAry[3] = (byte)(millions);
                }
                else
                {
                    byteAry[3] = (byte)(16 + millions);
                }
            }
            else if (g == 1)
            {
                if (s == 0)
                {
                    byteAry[3] = (byte)(64 + millions);
                }
                else
                {
                    byteAry[3] = (byte)(80 + millions);
                }
            }

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式03.
        /// <param name="strData">接收帧字符串长度：11个字符，</param>
        /// 返回值单位：kWh，当接受参数无效时，返回默认值99999
        /// </summary> 
        public string ExplanFormat03(string strData)
        {
            int intSum;//求和
            int intStrLen;//字符串长度
            string strUnit;//表示单位
            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen) ||
                strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen).ToUpper())
            {
                return strDefValu;
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
                    strUnit = "";//不要单位
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
                    throw new Exception("参数有误，无法解析");
                    //return int.Parse(strDefValu);
                }

            }
        }
        /// <summary>
        /// 组织数据格式04.
        /// <param name="strData">str由0~9数字组成的两个字符</param>
        /// <param name="upOrDown">upOrDown表示上浮还是下浮，上浮是0，下浮是1  </param>
        /// 无效参数，返回默认值99999
        /// </summary> 
        public List<byte> OrgFormat04(int intData, sbyte upOrDown)
        {
            int intSum;//求和
            //string strData;

            byteAry = new byte[1] { 0 };

            if (intData < 0 || intData > 79)
            {
                throw new Exception("输入数值不在处理范围之内(0~79)");
            }
            intSum = (intData / 10) * 16 + upOrDown * (int)Math.Pow(2, 7) + intData % 10;
            byteAry[0] = (byte)intSum;
            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式04.
        /// <param name="strData">接收帧字符串长度：2个字符，</param>
        /// 当参数无效时，返回默认参数99999
        /// </summary> 
        public string ExplanFormat04(string strData)
        {
            int intSum;//求和
            int intStrLen;//字符串长度

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen) ||
                strData.Substring(0, strData.Length) == strVoid.Substring(0, intStrLen).ToUpper())
            {
                return strDefValu;
            }
            else
            {
                try
                {
                    intSum = (Convert.ToInt32((strData.Substring(0, 1)), 16) & 7) * 10 + int.Parse(strData.Substring(1, 1));
                    if ((Convert.ToInt32((strData.Substring(0, 1)), 16) & 8) == 0)//s0=0代表上浮
                    {
                        return "上浮： " + intSum.ToString();
                    }
                    else
                    {
                        return "下浮： " + intSum.ToString();
                    }
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return strDefValu;                    
                }

            }
        }

        /// <summary>
        /// 组织数据格式05.
        /// <para> 带符号的浮点数，整数位到百位，小数位到十分位，其他位上数值直接舍去</para>      
        /// </summary>  
        public List<byte> OrgFormat05(float fltData)
        {
            int hundreds, tens, units, tenths;//百位，十位，个位，十分位
            int pos;
            string strTemp;
            string strIntStr, strDeciStr;//整数部分，小数部分
            float fltTemp;
            byteAry = new byte[2];
            fltTemp = Math.Abs(fltData);
            if (fltTemp >= 800)
            {
                throw new Exception("输入数值不在处理范围之内");
            }
            strTemp = fltTemp.ToString();
            pos = strTemp.IndexOf('.');
            if (pos == -1)//只有整数部分
            {
                strIntStr = strTemp;
                strDeciStr = "0";
            }
            else
            {
                strIntStr = strTemp.Substring(0, pos);//整数部分
                strDeciStr = strTemp.Substring(pos + 1, strTemp.Length - pos - 1);//小数部分
                if (strDeciStr.Length < 1)
                    strDeciStr = strDeciStr + "0";
            }

            units = int.Parse(strIntStr) % 10;//个位
            tenths = int.Parse(strDeciStr.Substring(0, 1));//十分位
            tens = (int.Parse(strIntStr) % 100) / 10;//十位
            hundreds = (int.Parse(strIntStr) % 1000) / 100;//百位

            byteAry[0] = (byte)(units * 16 + tenths);
            byteAry[1] = (byte)(hundreds * 16 + tens);
            if (fltData < 0)
                byteAry[1] += 128;
            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式05.
        /// <param name="strData">接收帧字符串长度：5个字符，</param>
        /// <returns> 带符号的浮点数，返回值整数位最高到百位，小数位精度到十分位</returns>
        /// 
        /// 当参数无效时，返回默认值-999999
        /// </summary> 
        public double ExplanFormat05(string strData)
        {
            double fltSum;//求和
            int intStrLen;//字符长度

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return float.Parse(strDefValu);
            }
            else
            {
                try
                {
                    fltSum = Convert.ToInt32(strData.Substring(0, 1), 16) + Convert.ToDouble(Convert.ToInt32(strData.Substring(1, 1), 16)) / 10
                    + (Convert.ToInt32(strData.Substring(2, 1), 16) & 7) * 100 + Convert.ToInt32(strData.Substring(3, 1), 16) * 10;

                    if ((Convert.ToInt32(strData.Substring(2, 1), 16) & 8) == 8)
                    {
                        return -fltSum;
                    }
                    else
                    {
                        return fltSum;
                    }
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return float.Parse(strDefValu);
                }

            }
        }
        /// <summary>
        /// 组织数据格式06.
        /// <para> 带符号的浮点数，整数位到十位，小数位到百分位，其他位上数值直接舍去</para>
        /// </summary> 
        public List<byte> OrgFormat06(float fltData)
        {
            byteAry = new byte[2];
            string strIntStr, strDeciStr;//整数部分，小数部分
            int tens, units, tenths, hundredths;//十位，个位，十分位，百分位
            string strTemp;
            float fltTemp;
            int pos;


            fltTemp = Math.Abs(fltData);
            if (fltTemp >= 80)
            {
                throw new Exception("输入数值不在处理范围之内");
            }
            strTemp = fltTemp.ToString();
            pos = strTemp.IndexOf('.');

            if (pos == -1)//只有整数部分
            {
                strIntStr = strTemp;
                strDeciStr = "00";//小数部分补0
            }
            else
            {
                strIntStr = strTemp.Substring(0, pos);//整数部分
                strDeciStr = strTemp.Substring(pos + 1, strTemp.Length - pos - 1);//小数部分
                if (strDeciStr.Length < 2)
                    strDeciStr = strDeciStr + "0";//不够后面补0
            }

            units = int.Parse(strIntStr) % 10;//个位
            tens = (int.Parse(strIntStr) % 100) / 10;//十位           
            hundredths = int.Parse(strDeciStr.Substring(1, 1));//百分位
            tenths = int.Parse(strDeciStr.Substring(0, 1));//十分位

            byteAry[0] = (byte)(hundredths * 16 + tenths);
            byteAry[1] = (byte)(tens * 16 + units);
            if (fltData < 0)
                byteAry[1] += 128;
            return ConvertByteToList(byteAry);

        }

        /// <summary>
        /// 解析数据格式06.
        /// <param name="strData">接收帧字符串长度：5个字符，</param>
        /// <returns> 带符号的浮点数，返回值整数位最高到十位，小数位精度到百分位</returns>
        /// 当参数无效时，返回默认值-999999
        /// </summary> 
        public float ExplanFormat06(string strData)
        {
            float fltSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen) ||
                strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen).ToUpper())
            {
                return float.Parse(strDefValu);
            }
            else
            {
                try
                {
                    fltSum = float.Parse(strData.Substring(0, 1)) / 10 + float.Parse(strData.Substring(1, 1)) / 100
                    + (Convert.ToInt32((strData.Substring(3, 1)), 16) & 7) * 10 + int.Parse(strData.Substring(4, 1));

                    if ((Convert.ToInt32((strData.Substring(3, 1)), 16) & 8) == 8)
                    {
                        return -fltSum;
                    }
                    else
                    {
                        return fltSum;
                    }
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return float.Parse(strDefValu);                    
                }

            }
        }

        /// <summary>
        /// 组织数据格式07.
        /// <para> 不带符号的浮点数，整数位到百位，小数位到十分位，其他位上数值直接舍去</para>
        /// </summary> 
        public List<byte> OrgFormat07(float fltData)
        {
            byteAry = new byte[2];
            string strIntStr, strDeciStr;//整数部分，小数部分
            int hundreds, tens, units, tenths;//百位，十位，个位，十分位
            string strTemp;
            //float fltTemp;
            int pos;

            //fltTemp = Math.Abs(fltData);
            //strTemp = fltTemp.ToString();
            if (fltData < 0 || fltData >= 1000)
            {
                throw new Exception("输入数值不在处理范围之内");
            }
            strTemp = fltData.ToString();
            pos = strTemp.IndexOf('.');

            if (pos == -1)//只有整数部分
            {
                strIntStr = strTemp;
                strDeciStr = "0";
            }
            else
            {
                strIntStr = strTemp.Substring(0, pos);//整数部分
                strDeciStr = strTemp.Substring(pos + 1, strTemp.Length - pos - 1);//小数部分
                if (strDeciStr.Length < 1)
                    strDeciStr = strDeciStr + "0";
            }

            units = int.Parse(strIntStr) % 10;//个位
            tenths = int.Parse(strDeciStr.Substring(0, 1));//十分位
            tens = (int.Parse(strIntStr) % 100) / 10;//十位
            hundreds = (int.Parse(strIntStr) % 1000) / 100;//百位

            byteAry[0] = (byte)(units * 16 + tenths);
            byteAry[1] = (byte)(hundreds * 16 + tens);

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式07.
        /// <param name="strData">接收帧字符串长度：5个字符，</param>
        /// <returns>不带符号的浮点数，返回值整数位最高到百位，小数位精度到十分位</returns>
        /// 当参数无效时，返回默认值-999999
        /// </summary> 
        public float ExplanFormat07(string strData)
        {
            float fltSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return float.Parse(strDefValu);
            }
            else
            {
                try
                {
                    fltSum = Convert.ToInt32(strData.Substring(0, 1), 16) + Convert.ToInt32(strData.Substring(1, 1), 16) / 10f
                        + Convert.ToInt32(strData.Substring(2, 1), 16) * 100 + Convert.ToInt32(strData.Substring(3, 1), 16) * 10;
                    return fltSum;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return float.Parse(strDefValu);
                }

            }
        }

        /// <summary>
        /// 组织数据格式08.
        /// <para> 不带符号的整数，整数位到千位，其他位上数值直接舍去</para>
        /// </summary> 
        public List<byte> OrgFormat08(int intData)
        {
            byteAry = new byte[2];
            int thousands, hundreds, tens, units;//千位，百位，十位，个位           

            //intData= (int) Math.Abs(intData);            
            if (intData < 0 || intData > 9999)
            {
                throw new Exception("输入数值不在处理范围之内");
            }
            units = intData % 10;//个位
            tens = (intData % 100) / 10;//十位           
            hundreds = (intData % 1000) / 100;//百位
            thousands = (intData % 10000) / 1000;//千位

            byteAry[0] = (byte)(tens * 16 + units);
            byteAry[1] = (byte)(thousands * 16 + hundreds);

            return ConvertByteToList(byteAry);

        }

        /// <summary>
        /// 解析数据格式08.
        /// <param name="strData">接收帧字符串长度：2个字符，</param>
        /// <returns>返回不带符号的整数,最大值4位数</returns>
        /// 当参数无效时，返回默认值-999999
        /// </summary> 
        public int ExplanFormat08(string strData)
        {
            int intSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen) ||
                strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen).ToUpper())
            {
                return int.Parse(strDefValu);
            }
            else
            {
                try
                {
                    intSum = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1))
                        + int.Parse(strData.Substring(2, 1)) * 1000 + int.Parse(strData.Substring(3, 1)) * 100;
                    return intSum;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return int.Parse(strDefValu); 
                }


            }
        }

        /// <summary>
        /// 组织数据格式09.
        /// <para> 带符号的浮点数，整数位到十位，小数位到万分位，其他位上数值直接舍去</para>
        /// </summary> 
        public List<byte> OrgFormat09(float fltData)
        {
            byteAry = new byte[3];
            string strIntStr, strDeciStr;//整数部分，小数部分
            int tens, units, tenths, hundredths, thousandths, tenThouths;//十位，个位，十分位，百分位,千分位，万分位
            string strTemp;
            float fltTemp;
            int pos;


            fltTemp = Math.Abs(fltData);
            if (fltTemp >= 80)
            {
                throw new Exception("输入数值不在处理范围之内");
            }
            strTemp = fltTemp.ToString();
            pos = strTemp.IndexOf('.');

            if (pos == -1)//只有整数部分
            {
                strIntStr = strTemp;
                strDeciStr = "0000";//小数部分补0
            }
            else
            {
                strIntStr = strTemp.Substring(0, pos);//整数部分
                strDeciStr = strTemp.Substring(pos + 1, strTemp.Length - pos - 1);//小数部分
                if (strDeciStr.Length < 4)
                    strDeciStr = strDeciStr + "000";//不够后面补0
            }

            units = int.Parse(strIntStr) % 10;//个位
            tens = (int.Parse(strIntStr) % 100) / 10;//十位 
            tenThouths = int.Parse(strDeciStr.Substring(3, 1));//万分位
            thousandths = int.Parse(strDeciStr.Substring(2, 1));//千分位
            hundredths = int.Parse(strDeciStr.Substring(1, 1));//百分位
            tenths = int.Parse(strDeciStr.Substring(0, 1));//十分位

            byteAry[0] = (byte)(thousandths * 16 + tenThouths);
            byteAry[1] = (byte)(tenths * 16 + hundredths);
            byteAry[2] = (byte)(tens * 16 + units);

            if (fltData < 0)
                byteAry[2] += 128;
            return ConvertByteToList(byteAry);

        }

        /// <summary>
        /// 解析数据格式09.
        /// <param name="strData">接收帧字符串长度：8个字符，</param>
        /// <returns> 带符号的浮点数，返回值整数位最高到十位，小数位精度到百分位</returns>
        ///  当参数无效时，返回默认值-999999 
        /// </summary> 
        public float ExplanFormat09(string strData)
        {
            float fltSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return float.Parse(strDefValu);
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
                        return -fltSum;
                    }
                    else
                    {
                        return fltSum;
                    }
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return float.Parse(strDefValu);
                }

            }
        }

        /// <summary>
        /// 组织数据格式10.
        /// <para>传入不带符号的整数，整数位到十万位，其他位上数值直接舍去</para>
        /// </summary> 
        public List<byte> OrgFormat10(int intData)
        {
            byteAry = new byte[3];
            int hunThous, tenThous, thousands, hundreds, tens, units;//十万位，万位，千位，百位，十位，个位           

            //intData = (int)Math.Abs(intData);
            if (intData < 0 || intData > 999999)
            {
                throw new Exception("输入数值不在处理范围之内");
            }
            units = intData % 10;//个位
            tens = (intData % 100) / 10;//十位           
            hundreds = (intData % 1000) / 100;//百位
            thousands = (intData % 10000) / 1000;//千位
            tenThous = (intData % 100000) / 10000;//万位
            hunThous = (intData % 1000000) / 100000;//十万位

            byteAry[0] = (byte)(tens * 16 + units);
            byteAry[1] = (byte)(thousands * 16 + hundreds);
            byteAry[2] = (byte)(hunThous * 16 + tenThous);

            return ConvertByteToList(byteAry);

        }

        /// <summary>
        /// 解析数据格式10.
        /// <param name="strData">接收帧字符串长度：8个字符，</param>
        /// <returns>返回不带符号的整数,最大值6位数</returns>
        /// 当参数无效时，返回默认值-999999
        /// </summary> 
        public int ExplanFormat10(string strData)
        {
            int intSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return int.Parse(strDefValu);
            }
            else
            {
                try
                {
                    intSum = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1))
                        + int.Parse(strData.Substring(2, 1)) * 1000 + int.Parse(strData.Substring(3, 1)) * 100
                        + int.Parse(strData.Substring(4, 1)) * 100000 + int.Parse(strData.Substring(5, 1)) * 10000;
                    return intSum;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return int.Parse(strDefValu);
                }

            }
        }

        /// <summary>
        /// 组织数据格式11.
        /// <para>不带符号的浮点数，整数位到十万位，小数位到百分位，其他位上数值直接舍去</para>
        /// </summary> 
        public List<byte> OrgFormat11(double dblData)
        {
            byteAry = new byte[4];
            string strIntStr, strDeciStr;//整数部分，小数部分
            int hunThous, tenThous, thousands, hundreds, tens, units, tenths, hundredths;//十万位，万位，千位，百位，十位，个位，十分位,百分位
            string strTemp;
            //float fltTemp;
            int pos;

            //fltTemp = Math.Abs(fltData);
            //strTemp = fltTemp.ToString();
            if (dblData < 0 || dblData >= 1000000)
            {
                throw new Exception("输入数值不在处理范围之内");
            }
            strTemp = dblData.ToString();
            pos = strTemp.IndexOf('.');

            if (pos == -1)//只有整数部分
            {
                strIntStr = strTemp;
                strDeciStr = "00";
            }
            else
            {
                strIntStr = strTemp.Substring(0, pos);//整数部分
                strDeciStr = strTemp.Substring(pos + 1, strTemp.Length - pos - 1);//小数部分
                if (strDeciStr.Length < 2)
                    strDeciStr = strDeciStr + "00";//不够后面补0
            }

            units = int.Parse(strIntStr) % 10;//个位
            tens = (int.Parse(strIntStr) % 100) / 10;//十位
            hundreds = (int.Parse(strIntStr) % 1000) / 100;//百位
            thousands = (int.Parse(strIntStr) % 10000) / 1000;//千位
            tenThous = (int.Parse(strIntStr) % 100000) / 10000;//万位
            hunThous = (int.Parse(strIntStr) % 1000000) / 100000;//十万位
            hundredths = int.Parse(strDeciStr.Substring(1, 1));//百分位
            tenths = int.Parse(strDeciStr.Substring(0, 1));//十分位            

            byteAry[0] = (byte)(tenths * 16 + hundredths);
            byteAry[1] = (byte)(tens * 16 + units);
            byteAry[2] = (byte)(thousands * 16 + hundreds);
            byteAry[3] = (byte)(hunThous * 16 + tenThous);

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式11.
        /// <param name="strData">接收帧字符串长度：11个字符，</param>
        /// <returns>不带符号的浮点数，返回值整数位最高到十万位，小数位精度到百分位</returns>
        /// 当参数无效时，返回默认值-999999
        /// </summary> 
        public double ExplanFormat11(string strData)
        {
            double dblSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return double.Parse(strDefValu);
            }
            else
            {
                try
                {
                    dblSum = double.Parse(strData.Substring(0, 1)) / 10 + double.Parse(strData.Substring(1, 1)) / 100
                        + int.Parse(strData.Substring(2, 1)) * 10 + int.Parse(strData.Substring(3, 1))
                        + int.Parse(strData.Substring(4, 1)) * 1000 + int.Parse(strData.Substring(5, 1)) * 100
                        + int.Parse(strData.Substring(6, 1)) * 100000 + int.Parse(strData.Substring(7, 1)) * 10000;
                    return dblSum;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return float.Parse(strDefValu);
                }


            }
        }

        /// <summary>
        /// 组织数据格式12.
        /// <para>不带符号的整数，整数位到千亿位，其他位上数值直接舍去</para>
        /// </summary> 
        public List<byte> OrgFormat12(long lng)
        {
            byteAry = new byte[6];
            string lngStr;
            int hunBills, tenBills, billions, hunMills, tenMills, millions,//千亿位，百亿位，十亿位，亿位，千万位，百万位，
                 hunThous, tenThous, thousands, hundreds, tens, units;//十万位，万位，千位，百位，十位，个位           

            //lng = (long)Math.Abs(lng);
            if (lng < 0 || lng > 999999999999)
            {
                throw new Exception("输入数值不在处理范围之内");
            }
            lngStr = lng.ToString();
            lngStr = "000000000000".Substring(0, 12 - lngStr.Length) + lngStr;//不足12位，前面补0

            units = int.Parse(lngStr.Substring(11, 1));//个位
            tens = int.Parse(lngStr.Substring(10, 1));//十位           
            hundreds = int.Parse(lngStr.Substring(9, 1));//百位
            thousands = int.Parse(lngStr.Substring(8, 1));//千位
            tenThous = int.Parse(lngStr.Substring(7, 1));//万位
            hunThous = int.Parse(lngStr.Substring(6, 1));//十万位           
            millions = int.Parse(lngStr.Substring(5, 1));//百万位
            tenMills = int.Parse(lngStr.Substring(4, 1));//千万位
            hunMills = int.Parse(lngStr.Substring(3, 1));//亿位
            billions = int.Parse(lngStr.Substring(2, 1));//十亿位           
            tenBills = int.Parse(lngStr.Substring(1, 1));//百亿位
            hunBills = int.Parse(lngStr.Substring(0, 1));//千亿位

            byteAry[0] = (byte)(tens * 16 + units);
            byteAry[1] = (byte)(thousands * 16 + hundreds);
            byteAry[2] = (byte)(hunThous * 16 + tenThous);
            byteAry[3] = (byte)(tenMills * 16 + millions);
            byteAry[4] = (byte)(billions * 16 + hunMills);
            byteAry[5] = (byte)(hunBills * 16 + tenBills);

            return ConvertByteToList(byteAry);

        }

        /// <summary>
        /// 解析数据格式12.
        /// <param name="strData">接收帧字符串长度：17个字符，</param>
        /// <returns>返回不带符号的整数,最大值12位数</returns>
        /// 当参数无效时，返回默认值-999999
        /// </summary> 
        public long ExplanFormat12(string strData)
        {
            long lngSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen).ToUpper())
            {
                return long.Parse(strDefValu);
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
                    return lngSum;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return long.Parse(strDefValu);
                }

            }
        }

        /// <summary>
        /// 组织数据格式13.
        /// <para>不带符号的浮点数，整数位到千位，小数位到万分位，其他位上数值直接舍去</para>
        /// </summary> 
        public List<byte> OrgFormat13(double dblData)
        {
            byteAry = new byte[4];
            string strIntStr, strDeciStr;//整数部分，小数部分
            int thousands, hundreds, tens, units, tenths, hundredths, thousandths,
                             tenThouths;//千位，百位，十位，个位，十分位,百分位,千分位，万分位
            string strTemp;
            //float fltTemp;
            int pos;

            //fltTemp = Math.Abs(fltData);
            //strTemp = fltTemp.ToString();
            if (dblData < 0 || dblData >= 10000)
            {
                throw new Exception("输入数值不在处理范围之内");
            }
            strTemp = dblData.ToString();
            pos = strTemp.IndexOf('.');

            if (pos == -1)//只有整数部分
            {
                strIntStr = strTemp;
                strDeciStr = "0000";
            }
            else
            {
                strIntStr = strTemp.Substring(0, pos);//整数部分
                strDeciStr = strTemp.Substring(pos + 1, strTemp.Length - pos - 1);//小数部分
                if (strDeciStr.Length < 4)
                    strDeciStr = strDeciStr + "0000";//不够后面补0
            }

            units = int.Parse(strIntStr) % 10;//个位
            tens = (int.Parse(strIntStr) % 100) / 10;//十位
            hundreds = (int.Parse(strIntStr) % 1000) / 100;//百位
            thousands = (int.Parse(strIntStr) % 10000) / 1000;//千位
            tenThouths = int.Parse(strDeciStr.Substring(3, 1));//万分位
            thousandths = int.Parse(strDeciStr.Substring(2, 1));//千分位 
            hundredths = int.Parse(strDeciStr.Substring(1, 1));//百分位
            tenths = int.Parse(strDeciStr.Substring(0, 1));//十分位            


            byteAry[0] = (byte)(thousandths * 16 + tenThouths);
            byteAry[1] = (byte)(tenths * 16 + hundredths);
            byteAry[2] = (byte)(tens * 16 + units);
            byteAry[3] = (byte)(thousands * 16 + hundreds);

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式13.
        /// <param name="strData">接收帧字符串长度：11个字符，</param>
        /// <returns>不带符号的浮点数，返回值整数位最高到千位，小数位精度到万分位</returns>
        /// 当参数无效时，返回默认值-999999
        /// </summary> 
        public double ExplanFormat13(string strData)
        {
            double dblSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen).ToUpper())
            {
                return double.Parse(strDefValu);
            }
            else
            {
                try
                {
                    dblSum = double.Parse(strData.Substring(0, 1)) / 1000 + double.Parse(strData.Substring(1, 1)) / 10000
                        + double.Parse(strData.Substring(2, 1)) / 10 + double.Parse(strData.Substring(3, 1)) / 100
                        + int.Parse(strData.Substring(4, 1)) * 10 + int.Parse(strData.Substring(5, 1))
                        + int.Parse(strData.Substring(6, 1)) * 1000 + int.Parse(strData.Substring(7, 1)) * 100;
                    return dblSum;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return float.Parse(strDefValu);
                }

            }
        }

        /// <summary>
        /// 组织数据格式14.
        /// <para>不带符号的浮点数，整数位到十万位，小数位到万分位，其他位上数值直接舍去</para>
        /// </summary> 
        public List<byte> OrgFormat14(double dblData)
        {
            byteAry = new byte[5];
            string strIntStr, strDeciStr;//整数部分，小数部分
            int hunThous, tenThous, thousands, hundreds, tens, units, tenths, hundredths,
                              thousandths, tenThouths;//十万位，万位，千位，百位，十位，个位，十分位,百分位,千分位，万分位
            string strTemp;
            //float fltTemp;
            int pos;

            //fltTemp = Math.Abs(fltData);
            //strTemp = fltTemp.ToString();

            if (dblData < 0 || dblData >= 1000000)
            {
                throw new Exception("输入数值不在处理范围之内");
            }
            strTemp = dblData.ToString();
            pos = strTemp.IndexOf('.');

            if (pos == -1)//只有整数部分
            {
                strIntStr = strTemp;
                strDeciStr = "0000";
            }
            else
            {
                strIntStr = strTemp.Substring(0, pos);//整数部分
                strDeciStr = strTemp.Substring(pos + 1, strTemp.Length - pos - 1);//小数部分
                if (strDeciStr.Length < 4)
                    strDeciStr = strDeciStr + "0000";//不够后面补0
            }

            units = int.Parse(strIntStr) % 10;//个位
            tens = (int.Parse(strIntStr) % 100) / 10;//十位
            hundreds = (int.Parse(strIntStr) % 1000) / 100;//百位
            thousands = (int.Parse(strIntStr) % 10000) / 1000;//千位
            tenThous = (int.Parse(strIntStr) % 100000) / 10000;//万位
            hunThous = (int.Parse(strIntStr) % 1000000) / 100000;//十万位
            tenThouths = int.Parse(strDeciStr.Substring(3, 1));//万分位
            thousandths = int.Parse(strDeciStr.Substring(2, 1));//千分位 
            hundredths = int.Parse(strDeciStr.Substring(1, 1));//百分位
            tenths = int.Parse(strDeciStr.Substring(0, 1));//十分位            


            byteAry[0] = (byte)(thousandths * 16 + tenThouths);
            byteAry[1] = (byte)(tenths * 16 + hundredths);
            byteAry[2] = (byte)(tens * 16 + units);
            byteAry[3] = (byte)(thousands * 16 + hundreds);
            byteAry[4] = (byte)(hunThous * 16 + tenThous);

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式14.
        /// <param name="strData">接收帧字符串长度：14个字符，</param>
        /// <returns>不带符号的浮点数，返回值整数位最高到十万位，小数位精度到万分位</returns>
        /// 当参数无效时，返回默认值-999999
        /// </summary> 
        public double ExplanFormat14(string strData)
        {
            double dblSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return double.Parse(strDefValu);
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
                    return dblSum;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return float.Parse(strDefValu);
                }

            }
        }

        /// <summary>
        /// 组织数据格式15   
        /// <param name="year"> year:1~99</param>
        /// <param name="month"> month:1~12</param>
        /// <param name="day"> day:1~对应月的天数</param>
        /// <param name="hour"> hour:0~23</param>
        /// <param name="minute"> minnte:0~59</param>
        /// <returns>返回字节数组</returns>
        /// 当输入参数无效时，返回数组默认值0
        /// </summary>
        public List<byte> OrgFormat15(int year, int month, int day, int hour, int minute)
        {

            byteAry = new byte[5] { 0, 0, 0, 0, 0 };

            if (year > 9999 || year < 1 || month > 12 || month < 1 || day < 1 ||
                                          hour > 23 || hour < 1 || minute > 59 || minute < 0)
            {
                throw new Exception("输入数值不在处理范围之内");
                //return byt;
            }

            if (year.ToString().Length == 2)//如果年份输入为两位数
            {                               //默认年份为20XX
                year = 2000 + year;
            }
            if (month == 2)
            {

                if (DateTime.IsLeapYear(year))//瑞年2月
                {
                    if (day > 29)
                    {
                        throw new Exception("日数值不在处理范围之内");
                    }
                }
                else//平年2月
                {
                    if (day > 28)
                    {
                        throw new Exception("日数值不在处理范围之内");
                    }
                }

            }
            else if (month == 1 || month == 3 || month == 5 || month == 7 ||
                                    month == 8 || month == 10 || month == 12)//大月
            {
                if (day > 31)
                {
                    throw new Exception("日数值不在处理范围之内");
                }
            }
            else // 小月 
            {
                if (day > 30)
                {
                    throw new Exception("日数值不在处理范围之内");
                }
            }

            byteAry[0] = (byte)((minute / 10) * 16 + minute % 10);
            byteAry[1] = (byte)((hour / 10) * 16 + hour % 10);
            byteAry[2] = (byte)((day / 10) * 16 + day % 10);
            byteAry[3] = (byte)((month / 10) * 16 + month % 10);
            byteAry[4] = (byte)((year % 100 / 10) * 16 + year % 10);

            return ConvertByteToList(byteAry);
        }


        public List<byte> OrgFormat15(string timeStr, int len)
        {
            DateTime theTime = Convert.ToDateTime(timeStr);
            int year = theTime.Year;
            int month = theTime.Month;
            int day = theTime.Day;
            int hour = theTime.Hour;
            int minute = theTime.Minute;
            byteAry = new byte[5] { 0, 0, 0, 0, 0 };

            if (year > 9999 || year < 1 || month > 12 || month < 1 || day < 1 ||
                                          hour > 23 || hour < 0 || minute > 59 || minute < 0)
            {
                throw new Exception("输入数值不在处理范围之内");
                //return byt;
            }

            if (year.ToString().Length == 2)//如果年份输入为两位数
            {                               //默认年份为20XX
                year = 2000 + year;
            }
            if (month == 2)
            {

                if (DateTime.IsLeapYear(year))//瑞年2月
                {
                    if (day > 29)
                    {
                        throw new Exception("日数值不在处理范围之内");
                    }
                }
                else//平年2月
                {
                    if (day > 28)
                    {
                        throw new Exception("日数值不在处理范围之内");
                    }
                }

            }
            else if (month == 1 || month == 3 || month == 5 || month == 7 ||
                                    month == 8 || month == 10 || month == 12)//大月
            {
                if (day > 31)
                {
                    throw new Exception("日数值不在处理范围之内");
                }
            }
            else // 小月 
            {
                if (day > 30)
                {
                    throw new Exception("日数值不在处理范围之内");
                }
            }

            byteAry[0] = (byte)((minute / 10) * 16 + minute % 10);
            byteAry[1] = (byte)((hour / 10) * 16 + hour % 10);
            byteAry[2] = (byte)((day / 10) * 16 + day % 10);
            byteAry[3] = (byte)((month / 10) * 16 + month % 10);
            byteAry[4] = (byte)((year % 100 / 10) * 16 + year % 10);

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式15.
        /// </summary>
        /// <param name="strData">接收帧字符串长度：14个字符</param>
        /// <returns>返回表示年 月 日 时 分 的符串</returns>
        /// 当参数无效时，返回默认值“0-0-0 0:0:0”        
        public string ExplanFormat15(string strData)
        {
            int year, month, day, hour, minute;
            string strDateStr;

            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return strDefDate;
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
                    throw new Exception("参数有误，无法解析");
                    //return strDefDate;
                }

            }
        }

        /// <summary>
        /// 组织数据格式16  
        /// <param name="day"> day:1~对应月的天数</param>
        /// <param name="hour"> hour:0~23</param>
        /// <param name="minute"> minnte:0~59</param>
        /// <param name="second"> second:0~59</param>
        /// <returns>返回字节数组</returns>
        /// 当输入参数无效时，返回数组默认值0
        /// </summary>
        public List<byte> OrgFormat16(int day, int hour, int minute, int second)
        {

            byteAry = new byte[4] { 0, 0, 0, 0 };

            if (day > 31 || day < 1 || hour > 23 || hour < 1 || minute > 59 ||
                                      minute < 0 || second > 59 || second < 0)
            {
                throw new Exception("输入数值不在处理范围之内");
                //return byt;
            }

            byteAry[0] = (byte)((second / 10) * 16 + second % 10);
            byteAry[1] = (byte)((minute / 10) * 16 + minute % 10);
            byteAry[2] = (byte)((hour / 10) * 16 + hour % 10);
            byteAry[3] = (byte)((day / 10) * 16 + day % 10);

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式16.
        /// <param name="strData">接收帧字符串长度：11个字符，</param>
        /// <returns>返回表示 日 时 分 秒的符串</returns>
        /// 当参数无效时，返回默认值“0-0-0 0:0:0”
        /// </summary>
        public string ExplanFormat16(string strData)
        {
            int day, hour, minute, second;
            string strDateStr;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen).ToUpper())
            {
                return strDefDate;
            }
            else
            {
                try
                {
                    second = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1));
                    minute = int.Parse(strData.Substring(3, 1)) * 10 + int.Parse(strData.Substring(4, 1));
                    hour = int.Parse(strData.Substring(6, 1)) * 10 + int.Parse(strData.Substring(7, 1));
                    day = int.Parse(strData.Substring(9, 1)) * 10 + int.Parse(strData.Substring(10, 1));

                    strDateStr = day.ToString() + " " + hour.ToString() + ":" + minute.ToString()
                                               + ":" + second.ToString();
                    return strDateStr;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return strDefDate;
                }
            }
        }

        /// <summary>
        /// 组织数据格式17   
        /// <param name="month"> month:1~12</param>
        /// <param name="day"> day:1~对应月的天数</param>
        /// <param name="hour"> hour:0~23</param>
        /// <param name="minute"> minnte:0~59</param>
        /// <returns>返回字节数组</returns>
        /// 当输入参数无效时，返回数组默认值0
        /// </summary>
        public List<byte> OrgFormat17(int month, int day, int hour, int minute)
        {

            byteAry = new byte[4] { 0, 0, 0, 0 };
            if (month > 12 || month < 1 || day < 1 || hour > 23 ||
                                    hour < 1 || minute > 59 || minute < 0)
            {
                throw new Exception("输入数值不在处理范围之内");
                //return byt;
            }
            if ((month == 2) & (day > 29))
            {
                throw new Exception("输入数值不在处理范围之内");
            }

            byteAry[0] = (byte)((minute / 10) * 16 + minute % 10);
            byteAry[1] = (byte)((hour / 10) * 16 + hour % 10);
            byteAry[2] = (byte)((day / 10) * 16 + day % 10);
            byteAry[3] = (byte)((month / 10) * 16 + month % 10);

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式17.
        /// <param name="strData">接收帧字符串长度：11个字符，</param>
        /// <returns>返回表示 月 日 时 分 的符串</returns>
        /// 当参数无效时，返回默认值“0-0-0 0:0:0”
        /// </summary>
        public string ExplanFormat17(string strData)
        {
            int month, day, hour, minute;
            int intStrLen;
            string strDateStr;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return strDefDate;
            }
            else
            {
                try
                {
                    minute = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1));
                    hour = int.Parse(strData.Substring(2, 1)) * 10 + int.Parse(strData.Substring(3, 1));
                    day = int.Parse(strData.Substring(4, 1)) * 10 + int.Parse(strData.Substring(5, 1));
                    month = int.Parse(strData.Substring(6, 1)) * 10 + int.Parse(strData.Substring(7, 1));

                    strDateStr = month.ToString("00") + "-" + day.ToString("00") + " " + hour.ToString("00") + ":"
                                                  + minute.ToString("00");
                    return strDateStr;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return strDefDate;
                }
            }
        }

        /// <summary>
        /// 组织数据格式18 .
        /// <param name="day"> day:1~对应月的天数</param>
        /// <param name="hour"> hour:0~23</param>
        /// <param name="minute"> minnte:0~59</param>      
        /// <returns>返回字节数组</returns>
        /// 当输入参数无效时，返回默认数组值0
        /// </summary>
        public List<byte> OrgFormat18(int day, int hour, int minute)
        {

            byteAry = new byte[3] { 0, 0, 0 };

            if (day > 31 || day < 1 || hour > 23 ||
                                    hour < 1 || minute > 59 || minute < 0)
            {
                throw new Exception("输入数值不在处理范围之内");
                //return byt;
            }
            byteAry[0] = (byte)((minute / 10) * 16 + minute % 10);
            byteAry[1] = (byte)((hour / 10) * 16 + hour % 10);
            byteAry[2] = (byte)((day / 10) * 16 + day % 10);

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式18.
        /// <param name="strData">接收帧字符串长度：8个字符，</param>
        /// <returns>返回表示 日 时 分 的符串</returns>
        /// 当参数无效时，返回默认值“0-0-0 0:0:0”
        /// </summary>
        public string ExplanFormat18(string strData)
        {
            int day, hour, minute;
            int intStrLen;
            string strDateStr;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return strDefDate;
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
                    throw new Exception("参数有误，无法解析");
                    //return strDefDate;
                }
            }
        }

        /// <summary>
        /// 组织数据格式19  
        /// <param name="hour"> hour:0~23</param>
        /// <param name="minute"> minnte:0~59</param>
        /// <returns>返回字节数组</returns>
        /// 当输入参数无效时，返回数组默认值0
        /// </summary>
        public List<byte> OrgFormat19(int hour, int minute)
        {

            byteAry = new byte[2] { 0, 0 };

            if (hour > 23 || hour < 1 || minute > 59 || minute < 0)
            {
                throw new Exception("输入数值不在处理范围之内");
                //return byt;
            }
            byteAry[0] = (byte)((minute / 10) * 16 + minute % 10);
            byteAry[1] = (byte)((hour / 10) * 16 + hour % 10);

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式19.
        /// <param name="strData">接收帧字符串长度：5个字符，</param>
        /// <returns>返回表示 时 分 的符串</returns>
        /// 当参数无效时，返回默认值“0-0-0 0:0:0”
        /// </summary>
        public string ExplanFormat19(string strData)
        {
            int hour, minute;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen) ||
                strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen).ToUpper())
            {
                return strDefDate;
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
                    throw new Exception("参数有误，无法解析");
                    //return strDefDate;
                }
            }
        }

        /// <summary>
        /// 组织数据格式20   
        /// <param name="year"> year:1~99</param>
        /// <param name="month"> month:1~12</param>
        /// <param name="day"> day:1~对应月的天数</param>
        /// <returns>返回字节数组</returns>
        /// 当输入参数无效时，返回默认数组值0
        /// </summary>
        public List<byte> OrgFormat20(int year, int month, int day)
        {

            byteAry = new byte[3] { 0, 0, 0 };

            if (year > 9999 || year < 1 || month > 12 || month < 1 || day < 1)
            {
                throw new Exception("输入数值不在处理范围之内");
                //return byt;
            }

            if (year.ToString().Length == 2)//如果年份输入为两位数
            {                               //默认年份为20XX
                year = 2000 + year;
            }

            if (month == 2)
            {

                if (DateTime.IsLeapYear(year))//瑞年2月
                {
                    if (day > 29)
                    {
                        throw new Exception("日数值不在处理范围之内");
                    }
                }
                else //平年2月
                {
                    if (day > 28)
                    {
                        throw new Exception("日数值不在处理范围之内");
                    }
                }

            }
            else if (month == 1 || month == 3 || month == 5 || month == 7 ||
                                    month == 8 || month == 10 || month == 12)//大月
            {
                if (day > 31)
                {
                    throw new Exception("日数值不在处理范围之内");
                }
            }
            else // 小月 
            {
                if (day > 30)
                {
                    throw new Exception("日数值不在处理范围之内");
                }
            }

            byteAry[0] = (byte)((day / 10) * 16 + day % 10);
            byteAry[1] = (byte)((month / 10) * 16 + month % 10);
            byteAry[2] = (byte)((year % 100 / 10) * 16 + year % 10);

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式20.
        /// <param name="strData">接收帧字符串长度：8个字符，</param>
        /// <returns>返回表示 年 月 日 的符串</returns>
        /// 当参数无效时，返回默认值“0-0-0 0:0:0”
        /// </summary>
        public string ExplanFormat20(string strData)
        {
            int year, month, day;
            string strDateStr;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return strDefDate.Substring(0, 8);
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
                    throw new Exception("参数有误，无法解析");
                    //return strDefDate;
                }
            }
        }

        /// <summary>
        /// 组织数据格式21. 
        ///<param name="year"> year:1~99</param> 
        ///<param name="month"> month:1~12</param> 
        /// <returns>返回字节数组</returns>
        /// 当输入参数无效时，返回默认数组值0
        /// </summary>
        public List<byte> OrgFormat21(int year, int month)
        {

            byteAry = new byte[2] { 0, 0 };
            if (year > 9999 || year < 1 || month > 12 || month < 1)
            {
                throw new Exception("输入数值不在处理范围之内");
                //return byt;
            }
            byteAry[0] = (byte)((month / 10) * 16 + month % 10);
            byteAry[1] = (byte)((year % 100 / 10) * 16 + year % 10);

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式21.
        /// <param name="strData">接收帧字符串长度：5个字符，</param>
        /// <returns>返回表示 年 月 的符串</returns>
        /// 当参数无效时，返回默认值“0-0-0 0:0:0”
        /// </summary>
        public string ExplanFormat21(string strData)
        {
            int year, month;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return strDefDate;
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
                    throw new Exception("参数有误，无法解析");
                    //return strDefDate;
                }
            }
        }

        /// <summary>
        /// 组织数据格式22.
        /// <para>不带符号的浮点数，整数位到个位，小数位到十分位，其他位上数值直接舍去</para>
        /// </summary> 
        public List<byte> OrgFormat22(float fltData)
        {
            byteAry = new byte[1];
            string strIntStr, strDeciStr;//整数部分，小数部分
            int units, tenths;//个位，十分位
            string strTemp;
            // float fltTemp;
            int pos;

            //fltTemp = Math.Abs(fltData);
            //strTemp = fltTemp.ToString();
            if (fltData < 0 || fltData >= 10)
            {
                throw new Exception("输入数值不在处理范围之内");
            }
            strTemp = fltData.ToString();
            pos = strTemp.IndexOf('.');

            if (pos == -1)//只有整数部分
            {
                strIntStr = strTemp;
                strDeciStr = "0";
            }
            else
            {
                strIntStr = strTemp.Substring(0, pos);//整数部分
                strDeciStr = strTemp.Substring(pos + 1, strTemp.Length - pos - 1);//小数部分
                if (strDeciStr.Length < 1)
                {
                    strDeciStr = strDeciStr + "0";//不够后面补0
                }
            }

            units = int.Parse(strIntStr) % 10;//个位            
            tenths = int.Parse(strDeciStr.Substring(0, 1));//十分位            

            byteAry[0] = (byte)(units * 16 + tenths);

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式22.
        /// <param name="strData">接收帧字符串长度：2个字符，</param>
        /// <returns>不带符号的浮点数，返回值整数位最高到个位，小数位精度到十分位</returns>
        /// 当参数无效时，返回默认值-999999
        /// </summary> 
        public float ExplanFormat22(string strData)
        {
            float fltSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen) ||
                strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen).ToUpper())
            {
                return float.Parse(strDefValu);
            }
            else
            {
                try
                {
                    fltSum = int.Parse(strData.Substring(0, 1)) + float.Parse(strData.Substring(1, 1)) / 10;

                    return fltSum;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return float.Parse(strDefValu);
                }

            }
        }
        /// <summary>
        /// 组织数据格式23.
        /// <para>不带符号的浮点数，整数位到十位，小数位到万分位，其他位上数值直接舍去</para>
        /// </summary> 
        public List<byte> OrgFormat23(double dblData)
        {
            byteAry = new byte[3];
            string strIntStr, strDeciStr;//整数部分，小数部分
            int tens, units, tenths, hundredths, thousandths, tenThouths;//十位，个位，十分位，百分位,千分位，万分位
            string strTemp;
            // float fltTemp;
            int pos;


            //fltTemp = Math.Abs(fltData);
            //strTemp = fltTemp.ToString();
            if (dblData < 0 || dblData >= 100)
            {
                throw new Exception("输入数值不在处理范围之内");
            }

            strTemp = dblData.ToString();
            pos = strTemp.IndexOf('.');

            if (pos == -1)//只有整数部分
            {
                strIntStr = strTemp;
                strDeciStr = "0000";//小数部分补0
            }
            else
            {
                strIntStr = strTemp.Substring(0, pos);//整数部分
                strDeciStr = strTemp.Substring(pos + 1, strTemp.Length - pos - 1);//小数部分
                if (strDeciStr.Length < 4)
                    strDeciStr = strDeciStr + "000";//不够后面补0
            }

            units = int.Parse(strIntStr) % 10;//个位
            tens = (int.Parse(strIntStr) % 100) / 10;//十位 
            tenThouths = int.Parse(strDeciStr.Substring(3, 1));//万分位
            thousandths = int.Parse(strDeciStr.Substring(2, 1));//千分位
            hundredths = int.Parse(strDeciStr.Substring(1, 1));//百分位
            tenths = int.Parse(strDeciStr.Substring(0, 1));//十分位

            byteAry[0] = (byte)(thousandths * 16 + tenThouths);
            byteAry[1] = (byte)(tenths * 16 + hundredths);
            byteAry[2] = (byte)(tens * 16 + units);

            return ConvertByteToList(byteAry);

        }

        /// <summary>
        /// 解析数据格式23.
        /// <param name="strData">接收帧字符串长度：8个字符，</param>
        /// <returns>不带符号的浮点数，返回值整数位最高到十位，小数位精度到百分位</returns>
        /// 当参数无效时，返回默认值-999999
        /// </summary> 
        public float ExplanFormat23(string strData)
        {
            float fltSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return float.Parse(strDefValu);
            }
            else
            {
                try
                {
                    fltSum = float.Parse(strData.Substring(0, 1)) / 1000 + float.Parse(strData.Substring(1, 1)) / 10000
                        + float.Parse(strData.Substring(2, 1)) / 10 + float.Parse(strData.Substring(3, 1)) / 100
                        + int.Parse(strData.Substring(4, 1)) * 10 + int.Parse(strData.Substring(5, 1));

                    return fltSum;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return float.Parse(strDefValu);
                }
            }
        }

        /// <summary>
        /// 组织数据格式24.
        /// <param name="day"> day:1~对应月的天数</param>
        /// <param name="hour"> hour:0~23</param>
        /// <returns>返回字节数组</returns>
        /// 当输入参数无效时，返回默认数组值0
        /// </summary>
        public List<byte> OrgFormat24(int day, int hour)
        {

            byteAry = new byte[2] { 0, 0 };
            if (day > 31 || day < 1 || hour > 23 || hour < 0)
            {
                throw new Exception("输入数值不在处理范围之内");
                //return byt;
            }
            byteAry[0] = (byte)((hour / 10) * 16 + hour % 10);
            byteAry[1] = (byte)((day / 10) * 16 + day % 10);

            return ConvertByteToList(byteAry);
        }

        /// <summary>
        /// 解析数据格式24.
        /// <param name="strData">接收帧字符串长度：5个字符，</param>
        /// <returns>返回表示 日 时 的符串</returns>
        /// 当参数无效时，返回默认值“0-0-0 0:0:0”
        /// </summary>
        public string ExplanFormat24(string strData)
        {
            int day, hour;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen) ||
                strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen).ToUpper())
            {
                return strDefDate;
            }
            else
            {
                try
                {
                    hour = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1));
                    day = int.Parse(strData.Substring(3, 1)) * 10 + int.Parse(strData.Substring(4, 1));

                    return day.ToString() + "日" + hour.ToString() + "时";
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return strDefDate;
                }
            }
        }

        /// <summary>
        /// 组织数据格式25.
        /// <para>带符号的浮点数，整数位到百位，小数位到千分位，其他位上数值直接舍去</para>
        /// </summary> 
        public List<byte> OrgFormat25(float fltData)
        {
            string strIntStr, strDeciStr;//整数部分，小数部分
            int hundreds, tens, units, tenths, hundredths, thousandths;//百位，十位，个位，十分位，百分位,千分位
            string strTemp;
            float fltTemp;
            int pos;

            byteAry = new byte[3];

            fltTemp = Math.Abs(fltData);

            if (fltTemp >= 800)
            {
                throw new Exception("输入数值不在处理范围之内");
            }

            strTemp = fltTemp.ToString();
            pos = strTemp.IndexOf('.');

            if (pos == -1)//只有整数部分
            {
                strIntStr = strTemp;
                strDeciStr = "000";//小数部分补0
            }
            else
            {
                strIntStr = strTemp.Substring(0, pos);//整数部分
                strDeciStr = strTemp.Substring(pos + 1, strTemp.Length - pos - 1);//小数部分
                if (strDeciStr.Length < 3)
                {
                    strDeciStr = strDeciStr + "000";//不够后面补0
                }
            }

            units = int.Parse(strIntStr) % 10;//个位
            tens = (int.Parse(strIntStr) % 100) / 10;//十位 
            hundreds = (int.Parse(strIntStr) % 1000) / 100;//百位 
            thousandths = int.Parse(strDeciStr.Substring(2, 1));//千分位
            hundredths = int.Parse(strDeciStr.Substring(1, 1));//百分位
            tenths = int.Parse(strDeciStr.Substring(0, 1));//十分位

            byteAry[0] = (byte)(hundredths * 16 + thousandths);
            byteAry[1] = (byte)(units * 16 + tenths);
            byteAry[2] = (byte)(hundreds * 16 + tens);

            if (fltData < 0)
            {
                byteAry[2] += 128;
            }

            return ConvertByteToList(byteAry);

        }

        /// <summary>
        /// 解析数据格式25.
        /// <param name="strData">接收帧字符串长度：8个字符，</param>
        /// <returns>带符号的浮点数，返回值整数位最高到百位，小数位精度到千分位</returns>
        /// 当参数无效时，返回默认值-999999
        /// </summary> 
        public float ExplanFormat25(string strData)
        {
            float fltSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return float.Parse(strDefValu);
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
                        return -fltSum;
                    }
                    else
                    {
                        return fltSum;
                    }
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return float.Parse(strDefValu);
                }
            }
        }

        /// <summary>
        /// 组织数据格式26.
        /// <para>不带符号的浮点数，整数位到个位，小数位到千分位，其他位上数值直接舍去</para>
        /// </summary> 
        public List<byte> OrgFormat26(float fltData)
        {
            byteAry = new byte[2];
            string strIntStr, strDeciStr;//整数部分，小数部分
            int units, tenths, hundredths, thousandths;//个位，十分位，百分位,千分位
            string strTemp;
            // float fltTemp;
            int pos;


            if (fltData < 0 || fltData >= 10)
            {
                throw new Exception("输入数值不在处理范围之内");
            }

            //fltTemp = Math.Abs(fltData); 
            //strTemp = fltTemp.ToString();

            strTemp = fltData.ToString();
            pos = strTemp.IndexOf('.');

            if (pos == -1)//只有整数部分
            {
                strIntStr = strTemp;
                strDeciStr = "000";//小数部分补0
            }
            else
            {
                strIntStr = strTemp.Substring(0, pos);//整数部分
                strDeciStr = strTemp.Substring(pos + 1, strTemp.Length - pos - 1);//小数部分
                if (strDeciStr.Length < 3)
                {
                    strDeciStr = strDeciStr + "000";//不够后面补0
                }
            }

            units = int.Parse(strIntStr) % 10;//个位 
            thousandths = int.Parse(strDeciStr.Substring(2, 1));//千分位
            hundredths = int.Parse(strDeciStr.Substring(1, 1));//百分位
            tenths = int.Parse(strDeciStr.Substring(0, 1));//十分位

            byteAry[0] = (byte)(hundredths * 16 + thousandths);
            byteAry[1] = (byte)(units * 16 + tenths);

            return ConvertByteToList(byteAry);

        }

        /// <summary>
        /// 解析数据格式26.
        /// <param name="strData">接收帧字符串长度：5个字符，</param>
        /// <returns>不带符号的浮点数，返回值整数位最高到个位，小数位精度到千分位</returns>
        /// 当参数无效时，返回默认值-999999
        /// </summary> 
        public float ExplanFormat26(string strData)
        {
            float fltSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen) ||
                strData.Substring(0, intStrLen) == strVoid.Substring(0, intStrLen).ToUpper())
            {
                return float.Parse(strDefValu);
            }
            else
            {
                try
                {
                    fltSum = float.Parse(strData.Substring(0, 1)) / 100 + float.Parse(strData.Substring(1, 1)) / 1000
                        + float.Parse(strData.Substring(3, 1)) + float.Parse(strData.Substring(4, 1)) / 10;

                    return fltSum;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return float.Parse(strDefValu);
                }
            }
        }

        /// <summary>
        /// 组织数据格式27.
        /// <para>不带符号的整数，整数位到千万位，其他位上数值直接舍去</para>
        /// </summary> 
        public List<byte> OrgFormat27(int intData)
        {
            string strIntStr;
            int tenMills, millions, hunThous, tenThous, thousands,//千万位，百万位，十万位，万位，千位
                                              hundreds, tens, units;//百位，十位，个位 
            byteAry = new byte[4];
            if (intData < 0 || intData > 99999999)
            {
                throw new Exception("输入数值不在处理范围之内");
            }


            ////intData = (int)Math.Abs(intData);
            strIntStr = intData.ToString();
            strIntStr = "00000000".Substring(0, 8 - strIntStr.Length) + strIntStr;//不足8位，前面补0


            units = int.Parse(strIntStr.Substring(7, 1));//个位
            tens = int.Parse(strIntStr.Substring(6, 1));//十位           
            hundreds = int.Parse(strIntStr.Substring(5, 1));//百位
            thousands = int.Parse(strIntStr.Substring(4, 1));//千位
            tenThous = int.Parse(strIntStr.Substring(3, 1));//万位
            hunThous = int.Parse(strIntStr.Substring(2, 1));//十万位           
            millions = int.Parse(strIntStr.Substring(1, 1));//百万位
            tenMills = int.Parse(strIntStr.Substring(0, 1));//千万位

            byteAry[0] = (byte)(tens * 16 + units);
            byteAry[1] = (byte)(thousands * 16 + hundreds);
            byteAry[2] = (byte)(hunThous * 16 + tenThous);
            byteAry[3] = (byte)(tenMills * 16 + millions);

            return ConvertByteToList(byteAry);

        }

        /// <summary>
        /// 解析数据格式27.
        /// <param name="strData">接收帧字符串长度：11个字符，</param>
        /// <returns>返回不带符号的整数,最大值8位数</returns>
        /// 当参数无效时，返回默认值 -999999
        /// </summary> 
        public int ExplanFormat27(string strData)
        {
            int intSum;
            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen) ||
                 strData.Substring(0, intStrLen).Replace(" ", "") == strVoid.Replace(" ", "").Substring(0, intStrLen).ToUpper())
            {
                return int.Parse(strDefValu);
            }
            else
            {
                try
                {
                    intSum = int.Parse(strData.Substring(0, 1)) * 10 + int.Parse(strData.Substring(1, 1))
                        + int.Parse(strData.Substring(2, 1)) * 1000 + int.Parse(strData.Substring(3, 1)) * 100
                        + int.Parse(strData.Substring(4, 1)) * 100000 + int.Parse(strData.Substring(5, 1)) * 10000
                        + int.Parse(strData.Substring(6, 1)) * 10000000 + int.Parse(strData.Substring(7, 1)) * 1000000;

                    return intSum;
                }
                catch (Exception)
                {
                    throw new Exception("参数有误，无法解析");
                    //return int.Parse(strDefValu);
                }
            }
        }

        public string ExplanNo(string strData)
        {
            return strData;
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

        /// <summary>
        /// 解析Bin类型
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public int ExplanBinFun(string strData)
        {
            double tempBin = 0;
            for (int i = 0; i < strData.Length / 2; i++)
            {
                tempBin += Convert.ToInt16(strData.Substring(i * 2, 2), 16) * Math.Pow(256, i);
            }
            return Convert.ToInt32(tempBin);
        }

        /// <summary>
        /// 解析bin类型数据
        /// </summary>
        /// <param name="num">数据</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static List<byte> OrgBinFun(int num, int len)
        {
            List<byte> tempData = new List<byte>();
            for (int i = len - 1; i >= 0; i--)
            {
                var data = (num >> (8 * i)) % 256;
                tempData.Add(Convert.ToByte(data));
            }
            tempData.Reverse();
            return tempData;
        }

        /// <summary>
        /// 组合AS
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public List<byte> OrgASIIFun(string strData, int len)
        {
            byte[] tempData = new byte[len];
            var data = Encoding.ASCII.GetBytes(strData);
            Array.Copy(data, tempData, data.Length);
            return ConvertByteToList(byteAry);
        }

        public string ExplanASSIIFun(string strData)
        {
            byte[] asiiDatas = OrgAddrFun(strData, strData.Length / 2);
            List<byte> asiiDatas2 = new List<byte>();
            asiiDatas2.AddRange(asiiDatas);
            asiiDatas2.Reverse();
            string assiValue = Encoding.ASCII.GetString(asiiDatas2.ToArray());
            return assiValue;
        }


        /// <summary>
        /// 从10进制
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public List<byte> OrgBSXFun(string strData, int len)
        {
            List<byte> tempData = new List<byte>();
            for (int i = 0; i < len; i--)
            {
                var data = strData.Substring(i * 2, 2);
                tempData.Add(Convert.ToByte(data, 16));
            }
            return tempData;
        }


        public string ExplanBSXFun(string strData)
        {
            //string tempAddr = "";
            //for (int i = strData.Length / 2; i > 0; i--)
            //{
            //    tempAddr += strData.Substring((i - 1) * 2, 2);
            //}
            if (strData.Length < 2) return "";
            string tempData = "";
            int int_ByteLenth = strData.Length / 2;
            for (int i = int_ByteLenth; i > 0; i--)
            {
                tempData += strData.Substring((i - 1) * 2, 2);
            }
            return Convert.ToString(Convert.ToInt64(tempData, 16), 2).PadLeft(int_ByteLenth * 8, '0');
        }


        public int ExplanNullDataFun(string strData)
        {
            return 0;
        }

        /// <summary>
        /// 一个byte位表示 8个的情况 如 0000 1111 表示有四个值
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public int ExplanByteWeiValue(string strData)
        {
            string erData = Convert.ToString(Convert.ToByte(strData, 16), 2);
            int tempValue = 0;
            for (int i = 0; i < erData.Length; i++)
            {
                if (erData[i] == '1')
                {
                    tempValue++;
                }

            }
            return tempValue;
        }


        public int ExplanFnFun(string strData)
        {
            int dt1 = Convert.ToInt32(strData.Substring(0, 2), 16);
            int dt2 = Convert.ToInt32(strData.Substring(2, 2), 16);

            ///在必要的时候要改变Fn   比如说事件的时候，因为要共用一套解析方法
            return (dt2 * 8 + (int)(Math.Log(dt1, 2.0) + 1));
        }


        public int ExplanPnFun(string strData)
        {
            int da1 = Convert.ToInt32(strData.Substring(0, 2), 16);
            int da2 = Convert.ToInt32(strData.Substring(2, 2), 16);
            return (da2 - 1) * 8 + (int)(Math.Log(da1, 2.0) + 1) == 2147483640 ? 0 : (da2 - 1) * 8 + (int)(Math.Log(da1, 2.0) + 1);
        }



        public int ExplanTdh(string strData)
        {
            int intSum;//求和
            int intStrLen;//字符串长度

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."

            try
            {
                intSum = (Convert.ToInt32((strData.Substring(0, 1)), 16) & 3) * 10 + int.Parse(strData.Substring(1, 1));
            }
            catch (Exception)
            {
                throw new Exception("参数有误，无法解析");
                //return strDefValu;                    
            }

            return intSum;
        }

        public List<byte> ConvertByteToList(byte[] bytTmp)
        {
            List<byte> lsTmp = new List<byte>();
            for (int i = 0; i < bytTmp.Length; i++)
            {
                lsTmp.Add(bytTmp[i]);
            }
            return lsTmp;
        }
    }
}
