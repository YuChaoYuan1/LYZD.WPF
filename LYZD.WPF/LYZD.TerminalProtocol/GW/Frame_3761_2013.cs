using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.TerminalProtocol.GW
{
     public class Frame_3761_2013 : Terminal
    {
        public int LenAddr = 2;//地址长度，吉林用的4个字节，其它地方2个字节
        public bool bol_Mac = false;//是否使用MAC地址作为组帧;
        FrameDataFormtUtil FrameUtil = new FrameDataFormtUtil();

        /// <summary>
        /// 读数据,AFN=21
        /// </summary>
        /// <param name="Afn"></param>
        /// <param name="Pn"></param>
        /// <param name="Fn"></param>
        /// <returns></returns>
        public string ReadData_Afn12(byte Afn, byte Pn, byte Fn)
        {
            string str_Frame = "";
            byte[] byt_Frame = UseFieldSet(Afn, Pn, Fn);
            byt_Frame = SendZhenData_Read(byt_Frame, Afn);
            str_Frame = GetByteToStr(byt_Frame);
            return str_Frame;
        }

        /// <summary>
        /// 写数据，Afn=04,05
        /// </summary>
        /// <param name="Afn"></param>
        /// <param name="Pn"></param>
        /// <param name="Fn"></param>
        /// <returns></returns>
        public string WriteData_Afn04(byte Afn, byte Pn, byte Fn, string strTmp)
        {
            string str_Frame = ""; byte[] bytTmp;
            if (Afn == 4)
            {
                bytTmp = GetFnToByte_Afn04(Fn, strTmp);
            }
            else
            {
                bytTmp = GetFnToByte_Afn05(Fn, strTmp);
            }

            byte[] byt_Frame = UseFieldSet(Afn, Pn, Fn, bytTmp);
            byt_Frame = SendZhenData_Write(byt_Frame, Afn);
            str_Frame = GetByteToStr(byt_Frame);
            return str_Frame;
        }

        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="Afn"></param>
        /// <param name="Pn"></param>
        /// <param name="Fn"></param>
        /// <param name="bytTmp">数据区内容</param>
        /// <returns></returns>
        public string ReadData(byte Afn, byte Pn, byte Fn, byte[] bytTmp)
        {
            string str_Frame = ""; byte[] byt_Frame;
            byt_Frame = UseFieldSet(Afn, Pn, Fn, bytTmp);
            byt_Frame = SendZhenData_Read(byt_Frame, Afn);
            str_Frame = GetByteToStr(byt_Frame);
            return str_Frame;
        }

        /// <summary>
        /// 返回确认
        /// </summary>
        /// <returns></returns>
        public string ReturnOk()
        {
            string str_Frame = ""; byte[] byt_Frame;
            byt_Frame = UseFieldSet_Afn00(0, 0, 1);
            byt_Frame = SendZhenData_Ok(byt_Frame, 0);
            str_Frame = GetByteToStr(byt_Frame);
            return str_Frame;
        }

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="Afn"></param>
        /// <param name="Pn"></param>
        /// <param name="Fn"></param>
        /// <param name="bytTmp">数据区内容</param>
        /// <returns></returns>
        public byte[] WriteData_Mac(byte Afn, byte Pn, byte Fn, byte[] bytTmp)
        {
            byte[] byt_Frame;
            byt_Frame = UseFieldSet(Afn, Pn, Fn, bytTmp);
            return byt_Frame;
        }

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="Afn"></param>
        /// <param name="Pn"></param>
        /// <param name="Fn"></param>
        /// <param name="bytTmp">数据区内容</param>
        /// <returns></returns>
        public string WriteData(byte Afn, byte Pn, byte Fn, byte[] bytTmp)
        {
            string str_Frame = ""; byte[] byt_Frame;
            byt_Frame = UseFieldSet(Afn, Pn, Fn, bytTmp);
            byt_Frame = SendZhenData_Write(byt_Frame, Afn);
            str_Frame = GetByteToStr(byt_Frame);
            return str_Frame;
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

        /// <summary>
        /// 完整帧组帧，读取
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        private byte[] SendZhenData_Ok(byte[] UserData, byte Afn)
        {
            int intLen = 6;
            int intCsLen = 12;
            if (LenAddr == 4)
            {
                intLen = 8;
                intCsLen = 14;
            }
            List<byte> lisTmp = new List<byte>();
            byte Cs = 0;
            lisTmp.Add(0x68);//起始字符（68H）	
            lisTmp.Add(Convert.ToByte(((UserData.Length + intLen) * 4 % 256) + 2));//长度L
            lisTmp.Add(Convert.ToByte(((UserData.Length + intLen) * 4) / 256));
            lisTmp.Add(Convert.ToByte(((UserData.Length + intLen) * 4 % 256) + 2));//长度L
            lisTmp.Add(Convert.ToByte(((UserData.Length + intLen) * 4) / 256));
            lisTmp.Add(0x68);//起始字符（68H）
            lisTmp.Add(int_ConC);//控制域C
            lisTmp.Add(Convert.ToByte(str_Ter_Code.Substring(2, 2), 16));//行政码
            lisTmp.Add(Convert.ToByte(str_Ter_Code.Substring(0, 2), 16));
            if (Convert.ToInt16(str_MainStation, 16) % 2 == 1)
            {
                if (LenAddr == 4)
                {
                    lisTmp.Add(Convert.ToByte(str_Ter_GroupAddress.Substring(6, 2), 16));//地址码
                    lisTmp.Add(Convert.ToByte(str_Ter_GroupAddress.Substring(4, 2), 16));
                }
                lisTmp.Add(Convert.ToByte(str_Ter_GroupAddress.Substring(2, 2), 16));//地址码
                lisTmp.Add(Convert.ToByte(str_Ter_GroupAddress.Substring(0, 2), 16));
            }
            else
            {
                if (LenAddr == 4)
                {
                    lisTmp.Add(Convert.ToByte(str_Ter_Address.Substring(6, 2), 16));//地址码
                    lisTmp.Add(Convert.ToByte(str_Ter_Address.Substring(4, 2), 16));
                }
                lisTmp.Add(Convert.ToByte(str_Ter_Address.Substring(2, 2), 16));//地址码
                lisTmp.Add(Convert.ToByte(str_Ter_Address.Substring(0, 2), 16));
            }
            if ((LenAddr == 4 && str_Ter_Address == "FFFFFFFF") || (LenAddr == 2 && str_Ter_Address == "FFFF"))
                lisTmp.Add(Convert.ToByte("03", 16));
            else//暂时屏蔽
                lisTmp.Add(Convert.ToByte(str_ConMainStation, 16));//主站地址和组地址标志A3
            for (int i = 0; i < UserData.Length; i++)//用户数据区
            {
                lisTmp.Add(UserData[i]);
            }


            byte[] bi = lisTmp.ToArray();
            for (int i = 6; i < intCsLen + UserData.Length; i++)//校验算法
            {
                Cs += bi[i];
            }

            lisTmp.Add(Cs);
            lisTmp.Add(0x16);//结束字符（16H）

            return lisTmp.ToArray();
        }

        /// <summary>
        /// 完整帧组帧，读取
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        private byte[] SendZhenData_Read(byte[] UserData, byte Afn)
        {
            int intLen = 12;
            int intCsLen = 18;
            if (LenAddr == 4)
            {
                intLen = 14;
                intCsLen = 20;
            }
            List<byte> lisTmp = new List<byte>();
            byte Cs = 0;
            lisTmp.Add(0x68);//起始字符（68H）	
            lisTmp.Add(Convert.ToByte(((UserData.Length + intLen) * 4 % 256) + 2));//长度L
            lisTmp.Add(Convert.ToByte(((UserData.Length + intLen) * 4) / 256));
            lisTmp.Add(Convert.ToByte(((UserData.Length + intLen) * 4 % 256) + 2));//长度L
            lisTmp.Add(Convert.ToByte(((UserData.Length + intLen) * 4) / 256));
            lisTmp.Add(0x68);//起始字符（68H）
            lisTmp.Add(0x4B);//控制域C
            lisTmp.Add(Convert.ToByte(str_Ter_Code.Substring(2, 2), 16));//行政码
            lisTmp.Add(Convert.ToByte(str_Ter_Code.Substring(0, 2), 16));
            if (Convert.ToInt16(str_MainStation, 16) % 2 == 1)
            {
                if (LenAddr == 4)
                {
                    lisTmp.Add(Convert.ToByte(str_Ter_GroupAddress.Substring(6, 2), 16));//地址码
                    lisTmp.Add(Convert.ToByte(str_Ter_GroupAddress.Substring(4, 2), 16));
                }
                lisTmp.Add(Convert.ToByte(str_Ter_GroupAddress.Substring(2, 2), 16));//地址码
                lisTmp.Add(Convert.ToByte(str_Ter_GroupAddress.Substring(0, 2), 16));
            }
            else
            {
                if (LenAddr == 4)
                {
                    lisTmp.Add(Convert.ToByte(str_Ter_Address.Substring(6, 2), 16));//地址码
                    lisTmp.Add(Convert.ToByte(str_Ter_Address.Substring(4, 2), 16));
                }
                lisTmp.Add(Convert.ToByte(str_Ter_Address.Substring(2, 2), 16));//地址码
                lisTmp.Add(Convert.ToByte(str_Ter_Address.Substring(0, 2), 16));
            }
            if ((LenAddr == 4 && str_Ter_Address == "FFFFFFFF") || (LenAddr == 2 && str_Ter_Address == "FFFF"))
                lisTmp.Add(Convert.ToByte("03", 16));
            else//暂时屏蔽
                lisTmp.Add(Convert.ToByte(str_MainStation, 16));//主站地址和组地址标志A3
            for (int i = 0; i < UserData.Length; i++)//用户数据区
            {
                lisTmp.Add(UserData[i]);
            }

            lisTmp.Add(int_PFC); int_PFC++;//Tpv部分
            lisTmp.Add(Convert.ToByte(DateTime.Now.Second.ToString(), 16));
            lisTmp.Add(Convert.ToByte(DateTime.Now.Minute.ToString(), 16));
            lisTmp.Add(Convert.ToByte(DateTime.Now.Hour.ToString(), 16));
            lisTmp.Add(Convert.ToByte(DateTime.Now.Day.ToString(), 16));
            lisTmp.Add(0);

            byte[] bi = lisTmp.ToArray();
            for (int i = 6; i < intCsLen + UserData.Length; i++)//校验算法
            {
                Cs += bi[i];
            }

            lisTmp.Add(Cs);
            lisTmp.Add(0x16);//结束字符（16H）

            return lisTmp.ToArray();
        }

        /// <summary>
        /// 完整帧组帧，设置
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        protected byte[] SendZhenData_Write(byte[] UserData, byte Afn)
        {
            int intLen = 28;
            int intCsLen = 34;
            if (LenAddr == 4)
            {
                intLen = 30;
                intCsLen = 36;
            }
            List<byte> lisTmp = new List<byte>();
            byte Cs = 0;
            lisTmp.Add(0x68);//起始字符（68H）	
            lisTmp.Add(Convert.ToByte(((UserData.Length + intLen) * 4 % 256) + 2));//长度L
            lisTmp.Add(Convert.ToByte(((UserData.Length + intLen) * 4) / 256));
            lisTmp.Add(Convert.ToByte(((UserData.Length + intLen) * 4 % 256) + 2));//长度L
            lisTmp.Add(Convert.ToByte(((UserData.Length + intLen) * 4) / 256));
            lisTmp.Add(0x68);//起始字符（68H）
            if (Afn == 1)
                lisTmp.Add(0x41);//控制域C
            else
                lisTmp.Add(0x4A);//控制域C
            lisTmp.Add(Convert.ToByte(str_Ter_Code.Substring(2, 2), 16));//行政码
            lisTmp.Add(Convert.ToByte(str_Ter_Code.Substring(0, 2), 16));
            if (Convert.ToInt16(str_MainStation, 16) % 2 == 1)
            {
                if (LenAddr == 4)
                {
                    lisTmp.Add(Convert.ToByte(str_Ter_GroupAddress.Substring(6, 2), 16));//地址码
                    lisTmp.Add(Convert.ToByte(str_Ter_GroupAddress.Substring(4, 2), 16));
                }
                lisTmp.Add(Convert.ToByte(str_Ter_GroupAddress.Substring(2, 2), 16));//地址码
                lisTmp.Add(Convert.ToByte(str_Ter_GroupAddress.Substring(0, 2), 16));
            }
            else
            {
                if (LenAddr == 4)
                {
                    lisTmp.Add(Convert.ToByte(str_Ter_Address.Substring(6, 2), 16));//地址码
                    lisTmp.Add(Convert.ToByte(str_Ter_Address.Substring(4, 2), 16));
                }
                lisTmp.Add(Convert.ToByte(str_Ter_Address.Substring(2, 2), 16));//地址码
                lisTmp.Add(Convert.ToByte(str_Ter_Address.Substring(0, 2), 16));
            }
            if ((LenAddr == 4 && str_Ter_Address == "FFFFFFFF") || (LenAddr == 2 && str_Ter_Address == "FFFF"))
                lisTmp.Add(Convert.ToByte("03", 16));
            else//暂时屏蔽
                lisTmp.Add(Convert.ToByte(str_MainStation, 16));//主站地址和组地址标志A3

            for (int i = 0; i < UserData.Length; i++)//用户数据区
            {
                lisTmp.Add(UserData[i]);
            }

            if (bol_Mac)
            {
                str_Mac = str_Mac.PadLeft(8, '0');
                lisTmp.Add(Convert.ToByte(str_Mac.Substring(6, 2), 16));
                lisTmp.Add(Convert.ToByte(str_Mac.Substring(4, 2), 16));
                lisTmp.Add(Convert.ToByte(str_Mac.Substring(2, 2), 16));
                lisTmp.Add(Convert.ToByte(str_Mac.Substring(0, 2), 16));
                for (int i = 0; i < 12; i++)
                {
                    lisTmp.Add(0);
                }
            }
            else
            {
                lisTmp.Add(Convert.ToByte(str_Password.Substring(2, 2), 16));//密码，密码为16个字节，方案号为0时只有2个字节，其它字节补0
                lisTmp.Add(Convert.ToByte(str_Password.Substring(0, 2), 16));

                for (int i = 0; i < 14; i++)
                {
                    lisTmp.Add(0);
                }
            }
            lisTmp.Add(int_PFC); int_PFC++;//Tpv部分
            lisTmp.Add(Convert.ToByte(DateTime.Now.Second.ToString(), 16));
            lisTmp.Add(Convert.ToByte(DateTime.Now.Minute.ToString(), 16));
            lisTmp.Add(Convert.ToByte(DateTime.Now.Hour.ToString(), 16));
            lisTmp.Add(Convert.ToByte(DateTime.Now.Day.ToString(), 16));
            lisTmp.Add(0);

            byte[] bi = lisTmp.ToArray();
            for (int i = 6; i < intCsLen + UserData.Length; i++)//校验算法
            {
                Cs += bi[i];
            }

            lisTmp.Add(Cs);
            lisTmp.Add(0x16);

            return lisTmp.ToArray();
        }

        /// <summary>
        /// 用户数据区组帧,读取
        /// </summary>
        /// <param name="Afn">控制码</param>
        /// <param name="P">信息点DA</param>
        /// <param name="F">信息类DT</param>
        /// <returns></returns>
        protected byte[] UseFieldSet(byte Afn, byte P, byte F)
        {
            List<byte> lisTmp = new List<byte>();
            lisTmp.Add(Afn);
            lisTmp.Add(Convert.ToByte(240 + (int_PFC % 16))); ;//帧序列域SEQ
            byte[] bp = ExplanP(P); byte[] bf = ExplanF(F);
            lisTmp.Add(bp[0]); lisTmp.Add(bp[1]);
            lisTmp.Add(bf[0]); lisTmp.Add(bf[1]);
            return lisTmp.ToArray();
        }

        public byte[] GetFnToByte_Afn04(byte Fn, string strTmp)
        {
            List<byte> lisTmp = new List<byte>(); byte[] bytTmp; string[] strTmp2;
            float Volt;
            float Curt;
            byte Clfs;
            string Ip = ""; string Port = "";
            switch (Fn)
            {
                case 3://主站IP和端口

                    Ip = strTmp.Split(',')[0];
                    Port = strTmp.Split(',')[1];
                    lisTmp.AddRange(OrgBinFun(Ip.Split('.')[0], 1));
                    lisTmp.AddRange(OrgBinFun(Ip.Split('.')[1], 1));
                    lisTmp.AddRange(OrgBinFun(Ip.Split('.')[2], 1));
                    lisTmp.AddRange(OrgBinFun(Ip.Split('.')[3], 1));//IP
                    lisTmp.AddRange(OrgBinFun(Port, 2));//端口
                    lisTmp.Add(1); lisTmp.Add(1); lisTmp.Add(1); lisTmp.Add(1); lisTmp.Add(1); lisTmp.Add(1);//备用
                    for (int i = 0; i < 16; i++)
                        lisTmp.Add(71);
                    break;
                case 7://终端IP和端口
                    Ip = strTmp.Split(',')[0];
                    lisTmp.AddRange(OrgBinFun(Ip.Split('.')[0], 1));
                    lisTmp.AddRange(OrgBinFun(Ip.Split('.')[1], 1));
                    lisTmp.AddRange(OrgBinFun(Ip.Split('.')[2], 1));
                    lisTmp.AddRange(OrgBinFun(Ip.Split('.')[3], 1));//IP
                    lisTmp.Add(255); lisTmp.Add(255); lisTmp.Add(255); lisTmp.Add(0);//子网掩码
                    lisTmp.Add(0); lisTmp.Add(0); lisTmp.Add(0); lisTmp.Add(0);//网关
                    lisTmp.Add(0);//代理类型
                    lisTmp.Add(0); lisTmp.Add(0); lisTmp.Add(0); lisTmp.Add(0);//代理IP
                    lisTmp.Add(1); lisTmp.Add(1);//代理端口
                    lisTmp.Add(0);//代理连接放肆
                    lisTmp.Add(0);
                    lisTmp.Add(0);
                    lisTmp.Add(1); lisTmp.Add(1);
                    break;
                case 9://终端事件记录配置设置
                    bytTmp = new byte[16];
                    for (int i = 0; i < 16; i++)
                    {
                        bytTmp[0] = 0;
                    }
                    strTmp2 = strTmp.Split(',');
                    for (int i = 0; i < strTmp2.Length; i++)
                        bytTmp[Convert.ToByte(strTmp2[i]) / 8] = Convert.ToByte(Math.Pow(2, Convert.ToByte(strTmp2[i]) % 8) / 2);
                    //bytTmp[Convert.ToByte(strTmp) / 8 + 8] = Convert.ToByte(Math.Pow(2, Convert.ToByte(strTmp) % 8) / 2);
                    FrameDataFormtUtil ds = new FrameDataFormtUtil();

                    return bytTmp;
                case 10://终端电能表/交流采样装置配置参数
                    bytTmp = new byte[29];
                    bytTmp[0] = 1; bytTmp[1] = 0;//测量点数量
                    bytTmp[2] = Convert.ToByte(strTmp.Split(',')[0]); bytTmp[3] = 0;//装置序号
                    bytTmp[4] = Convert.ToByte(strTmp.Split(',')[0]); bytTmp[5] = 0;//测量点序号
                    bytTmp[6] = Convert.ToByte(strTmp.Split(',')[1], 16);//通信速率及端口号
                    bytTmp[7] = Convert.ToByte(strTmp.Split(',')[2], 16);//协议类型号
                    for (int i = 0; i < 6; i++)//电表地址
                    {
                        bytTmp[8 + i] = setData12(strTmp.Split(',')[3]).ToArray()[i];
                    }
                    for (int i = 0; i < 6; i++)//通信密码
                    {
                        bytTmp[14 + i] = 0;
                    }
                    bytTmp[20] = 4;//电能费率个数
                    bytTmp[21] = 9;
                    for (int i = 0; i < 6; i++)//采集器地址
                    {
                        bytTmp[22 + i] = 0;
                    }
                    bytTmp[28] = 0;//用户大小类号
                    return bytTmp;
                case 12://状态量输入
                    bytTmp = new byte[2];
                    bytTmp[0] = Convert.ToByte(Math.Pow(2, Convert.ToByte(strTmp)) - 1);
                    bytTmp[1] = bytTmp[0];
                    return bytTmp;
                case 25://测量点基本参数
                    lisTmp.Add(10); lisTmp.Add(0);//电压互感器倍率
                    lisTmp.Add(100); lisTmp.Add(0);//电流互感器倍率
                    lisTmp.AddRange(setData7(strTmp.Split(',')[0]));
                    lisTmp.AddRange(setData22(strTmp.Split(',')[1]));
                    lisTmp.AddRange(setData23(strTmp.Split(',')[2]));
                    lisTmp.Add(2);
                    break;
                case 26:
                    Volt = Convert.ToSingle(strTmp.Split(',')[0]);
                    Curt = Convert.ToSingle(strTmp.Split(',')[1]);
                    Clfs = Convert.ToByte(strTmp.Split(',')[2]);
                    lisTmp.AddRange(setData7((float.Parse(strTmp.Split(',')[3]) * Volt).ToString()));//电压合格上限
                    lisTmp.AddRange(setData7((float.Parse(strTmp.Split(',')[4]) * Volt).ToString()));//电压合格下限
                    lisTmp.AddRange(setData7((float.Parse(strTmp.Split(',')[5]) * Volt).ToString()));//电压断相门限

                    lisTmp.AddRange(setData7((float.Parse(strTmp.Split(',')[6]) * Volt).ToString()));//电压上上限（过压门限）
                    lisTmp.AddRange(OrgBinFun("1", 1));//越限持续时间
                    lisTmp.AddRange(setData5("0"));//越限恢复系数

                    lisTmp.AddRange(setData7((float.Parse(strTmp.Split(',')[7]) * Volt).ToString()));//电压下下限（欠压门限）
                    lisTmp.AddRange(OrgBinFun("1", 1));//越限持续时间
                    lisTmp.AddRange(setData5("0"));//越限恢复系数

                    lisTmp.AddRange(setData25((float.Parse(strTmp.Split(',')[8]) * Curt).ToString()));//相电流上上限（过流门限）
                    lisTmp.AddRange(OrgBinFun("1", 1));//越限持续时间
                    lisTmp.AddRange(setData5("0"));//越限恢复系数

                    lisTmp.AddRange(setData25((float.Parse(strTmp.Split(',')[9]) * Curt).ToString()));//相电流上限（额定电流门限）
                    lisTmp.AddRange(OrgBinFun("1", 1));//越限持续时间
                    lisTmp.AddRange(setData5("0"));//越限恢复系数

                    lisTmp.AddRange(setData25((float.Parse(strTmp.Split(',')[10]) * Curt).ToString()));//零序电流上限
                    lisTmp.AddRange(OrgBinFun("1", 1));//越限持续时间
                    lisTmp.AddRange(setData5("0"));//越限恢复系数

                    lisTmp.AddRange(setData23("3.6"));//视在功率上上限
                    lisTmp.AddRange(OrgBinFun("1", 1));//越限持续时间
                    lisTmp.AddRange(setData5("0"));//越限恢复系数

                    lisTmp.AddRange(setData23("3.4"));//视在功率上限
                    lisTmp.AddRange(OrgBinFun("1", 1));//越限持续时间
                    lisTmp.AddRange(setData5("0"));//越限恢复系数

                    lisTmp.AddRange(setData5("2"));//三相电压不平衡限值
                    lisTmp.AddRange(OrgBinFun("1", 1));//越限持续时间
                    lisTmp.AddRange(setData5("0"));//越限恢复系数

                    lisTmp.AddRange(setData5("2"));//三相电流不平衡限值
                    lisTmp.AddRange(OrgBinFun("1", 1));//越限持续时间
                    lisTmp.AddRange(setData5("0"));//越限恢复系数

                    lisTmp.AddRange(OrgBinFun("1", 1));//连续失压时间限值
                    break;
                case 33:
                    lisTmp.Add(1);
                    lisTmp.Add(Convert.ToByte(strTmp.Split(',')[0]));//通信端口号
                    lisTmp.Add(0x20); lisTmp.Add(0);//运行控制字
                    lisTmp.Add(255); lisTmp.Add(255); lisTmp.Add(255); lisTmp.Add(255);
                    lisTmp.Add(0x57); lisTmp.Add(0x23);
                    lisTmp.Add(Convert.ToByte(strTmp.Split(',')[1]));//抄表间隔时间
                    lisTmp.Add(0); lisTmp.Add(0); lisTmp.Add(1);//广播校时时间
                    lisTmp.Add(1);
                    lisTmp.Add(0); lisTmp.Add(0); lisTmp.Add(0x23); lisTmp.Add(0x59);
                    break;
                case 57:
                    lisTmp.Add(0);
                    lisTmp.Add(0);
                    lisTmp.Add(0);
                    break;
                case 98:
                    Volt = Convert.ToSingle(strTmp.Split(',')[0]);
                    lisTmp.AddRange(OrgBinFun("1", 2));
                    lisTmp.AddRange(OrgBinFun("4320", 2));
                    lisTmp.AddRange(OrgBinFun("5", 2));
                    lisTmp.AddRange(OrgBinFun("60", 2));
                    lisTmp.AddRange(setData7((float.Parse(strTmp.Split(',')[1]) * Volt).ToString()));//
                    lisTmp.AddRange(setData7((float.Parse(strTmp.Split(',')[2]) * Volt).ToString()));//
                    lisTmp.Add(3);
                    break;
                case 105://传值1
                    lisTmp.Add(Convert.ToByte(strTmp));//数据分级类号
                    break;
                case 106://传值2,19020301*1,19030301*2
                    strTmp2 = strTmp.Split(',');
                    lisTmp.Add(Convert.ToByte(strTmp2[0]));
                    for (byte i = 1; i <= Convert.ToByte(strTmp2[0]); i++)
                    {
                        lisTmp.Add(Convert.ToByte(strTmp2[i].Split('*')[0].Substring(0, 2), 16));
                        lisTmp.Add(Convert.ToByte(strTmp2[i].Split('*')[0].Substring(2, 2), 16));
                        lisTmp.Add(Convert.ToByte(strTmp2[i].Split('*')[0].Substring(4, 2), 16));
                        lisTmp.Add(Convert.ToByte(strTmp2[i].Split('*')[0].Substring(6, 2), 16));
                        lisTmp.Add(Convert.ToByte(strTmp2[i].Split('*')[1], 16));
                    }
                    break;
                case 107://传值1,1
                    lisTmp.Add(Convert.ToByte(strTmp.Split(',')[0]));
                    lisTmp.Add(Convert.ToByte(strTmp.Split(',')[1]));
                    break;
            }
            return lisTmp.ToArray();
        }

        protected byte[] GetFnToByte_Afn05(byte Fn, string strTmp)
        {
            List<byte> lisTmp = new List<byte>();
            switch (Fn)
            {
                case 31://对时命令
                    lisTmp = setData1(Convert.ToDateTime(strTmp));
                    break;
            }
            return lisTmp.ToArray();
        }
        /// <summary>
        /// 用户数据区组帧,设置
        /// </summary>
        /// <param name="Afn">控制码</param>
        /// <param name="P">信息点DA</param>
        /// <param name="F">信息类DT</param>
        /// <param name="tcl">带数据内容，比如时钟设置内容</param>
        /// <returns></returns>
        protected byte[] UseFieldSet(byte Afn, byte P, byte F, byte[] tcl)
        {
            List<byte> lisTmp = new List<byte>();
            lisTmp.Add(Afn); lisTmp.Add(Convert.ToByte(240 + (int_PFC % 16))); ;//帧序列域SEQ
            byte[] bp = ExplanP(P); byte[] bf = ExplanF(F);
            lisTmp.Add(bp[0]); lisTmp.Add(bp[1]);
            lisTmp.Add(bf[0]); lisTmp.Add(bf[1]);
            for (int i = 0; i < tcl.Length; i++)
            {
                lisTmp.Add(tcl[i]);
            }
            return lisTmp.ToArray();
        }

        /// <summary>
        /// 用户数据区组帧,确认,仅作网络传输时给终端回复确认使用，需要判断上行报文的帧序号，
        /// </summary>
        /// <param name="Afn">控制码</param>
        /// <param name="P">信息点DA</param>
        /// <param name="F">信息类DT</param>
        /// <param name="tcl">带数据内容，比如时钟设置内容</param>
        /// <returns></returns>
        protected byte[] UseFieldSet_Afn00(byte Afn, byte P, byte F)
        {
            List<byte> lisTmp = new List<byte>();
            lisTmp.Add(Afn); lisTmp.Add(Convert.ToByte(96 + (int_ConPFC % 16)));//帧序列域SEQ
            byte[] bp = ExplanP(P); byte[] bf = ExplanF(F);
            lisTmp.Add(bp[0]); lisTmp.Add(bp[1]);
            lisTmp.Add(bf[0]); lisTmp.Add(bf[1]);
            return lisTmp.ToArray();
        }

        #region 基础数据格式单元,未对数据的大小进行分析，调用时请使用合理的数据

        /// <summary>
        /// 信息点DA组帧
        /// </summary>
        /// <param name="P"></param>
        /// <returns></returns>
        protected byte[] ExplanP(byte P)
        {
            byte[] bytTmp = new byte[2];
            if (P == 0)
            {
                bytTmp[0] = 0; bytTmp[1] = 0;
            }
            else
            {
                bytTmp[0] = Convert.ToByte(Math.Pow(2, (P - 1) % 8));
                bytTmp[1] = Convert.ToByte((P - 1) / 8 + 1);
            }
            return bytTmp;
        }

        /// <summary>
        /// 信息类DT组帧 
        /// </summary>
        /// <param name="F"></param>
        /// <returns></returns>
        protected byte[] ExplanF(byte F)
        {
            byte[] bytTmp = new byte[2];

            bytTmp[0] = Convert.ToByte(Math.Pow(2, (F - 1) % 8));
            bytTmp[1] = Convert.ToByte((F - 1) / 8);
            return bytTmp;
        }

        /// <summary>
        /// 解析bin类型数据
        /// </summary>
        /// <param name="num">数据</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        protected List<byte> OrgBinFun(string strTmp, int len)
        {
            List<byte> lisTmp = new List<byte>();
            for (int i = len - 1; i >= 0; i--)
            {
                var data = (Convert.ToInt32(strTmp) >> (8 * i)) % 256;
                lisTmp.Add(Convert.ToByte(data));
            }
            lisTmp.Reverse();
            return lisTmp;
        }

        /// <summary>
        /// 设置数据格式
        /// </summary>
        /// <param name="dtTmp"></param>
        public List<byte> setData1(DateTime dtTmp)
        {
            List<byte> lisTmp = new List<byte>();
            int week = Convert.ToInt16(dtTmp.DayOfWeek);
            lisTmp.Add(Convert.ToByte(dtTmp.Second / 10 * 16 + dtTmp.Second % 10));//当前时钟内容
            lisTmp.Add(Convert.ToByte(dtTmp.Minute / 10 * 16 + dtTmp.Minute % 10));
            lisTmp.Add(Convert.ToByte(dtTmp.Hour / 10 * 16 + dtTmp.Hour % 10));
            lisTmp.Add(Convert.ToByte(dtTmp.Day / 10 * 16 + dtTmp.Day % 10));
            lisTmp.Add(Convert.ToByte(week * 32 + dtTmp.Month / 10 * 16 + dtTmp.Month % 10));
            lisTmp.Add(Convert.ToByte((dtTmp.Year - 2000) / 10 * 16 + (dtTmp.Year - 2000) % 10));
            return lisTmp;
        }

        /// <summary>
        /// 设置数据格式
        /// </summary>
        /// <param name="dtTmp"></param>
        protected List<byte> setData5(string strTmp)
        {
            List<byte> lisTmp = new List<byte>();
            float fTmp = Math.Abs(Convert.ToSingle(strTmp) * 10);
            lisTmp.Add(Convert.ToByte(Convert.ToInt32(fTmp) % 100 / 10 * 16 + Convert.ToInt32(fTmp) % 10));
            lisTmp.Add(Convert.ToByte(Convert.ToInt32(fTmp) % 10000 / 1000 * 16 + Convert.ToInt32(fTmp) % 1000 / 100));
            if (Convert.ToSingle(strTmp) < 0)
                lisTmp[1] += 128;
            return lisTmp;
        }

        /// <summary>
        /// 设置数据格式
        /// </summary>
        /// <param name="dtTmp"></param>
        protected List<byte> setData23(string strTmp)
        {
            List<byte> lisTmp = new List<byte>();
            float fTmp = Convert.ToSingle(strTmp) * 10000;
            lisTmp.Add(Convert.ToByte(Convert.ToInt32(fTmp) % 100 / 10 * 16 + Convert.ToInt32(fTmp) % 10));
            lisTmp.Add(Convert.ToByte(Convert.ToInt32(fTmp) % 10000 / 1000 * 16 + Convert.ToInt32(fTmp) % 1000 / 100));
            lisTmp.Add(Convert.ToByte(Convert.ToInt32(fTmp) % 1000000 / 100000 * 16 + Convert.ToInt32(fTmp) % 100000 / 10000));
            return lisTmp;
        }

        protected List<byte> setData25(string strTmp)
        {
            List<byte> lisTmp = new List<byte>();
            float fTmp = Math.Abs(Convert.ToSingle(strTmp)) * 1000;
            lisTmp.Add(Convert.ToByte(Convert.ToInt32(fTmp) % 100 / 10 * 16 + Convert.ToInt32(fTmp) % 10));
            lisTmp.Add(Convert.ToByte(Convert.ToInt32(fTmp) % 10000 / 1000 * 16 + Convert.ToInt32(fTmp) % 1000 / 100));
            lisTmp.Add(Convert.ToByte(Convert.ToInt32(fTmp) % 1000000 / 100000 * 16 + Convert.ToInt32(fTmp) % 100000 / 10000));
            if (Convert.ToSingle(strTmp) < 0)
                lisTmp[2] += 128;
            return lisTmp;
        }

        /// <summary>
        /// 设置数据格式
        /// </summary>
        /// <param name="dtTmp"></param>
        protected List<byte> setData22(string strTmp)
        {
            List<byte> lisTmp = new List<byte>();
            float fTmp = Convert.ToSingle(strTmp) * 10;
            lisTmp.Add(Convert.ToByte(Convert.ToInt32(fTmp) % 100 / 10 * 16 + Convert.ToInt32(fTmp) % 10));
            return lisTmp;
        }

        /// <summary>
        /// 设置数据格式
        /// </summary>
        /// <param name="dtTmp"></param>
        protected List<byte> setData7(string strTmp)
        {
            List<byte> lisTmp = new List<byte>();
            float fTmp = Convert.ToSingle(strTmp) * 10;
            lisTmp.Add(Convert.ToByte(Convert.ToInt32(fTmp) % 100 / 10 * 16 + Convert.ToInt32(fTmp) % 10));
            lisTmp.Add(Convert.ToByte(Convert.ToInt32(fTmp) % 10000 / 1000 * 16 + Convert.ToInt32(fTmp) % 1000 / 100));
            return lisTmp;
        }

        protected List<byte> setData12(string strTmp)
        {
            List<byte> lisTmp = new List<byte>();
            strTmp = strTmp.PadLeft(12, '0');
            for (int i = 0; i < 6; i++)
                lisTmp.Add(Convert.ToByte(strTmp.Substring(10 - 2 * i, 2), 16));
            return lisTmp;
        }



        #endregion
    }
}
