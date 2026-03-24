using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace LYZD.TerminalProtocol.GW
{
     [ComVisible(true)]
    public class Analysis_698
    {
        protected List<string> lsTmp = new List<string>();//数据集合
        protected List<string> lsStructure = new List<string>();//结构集合
        string strFrameRemove = "";//帧数据，采用位移移除数据，迭代解析
        string strApdu = "";
        public bool BoolCheck = true;//是否校验,True为校验
        public int LenAddr = 2;//地址长度，吉林用的4个字节，其它地方2个字节
        public int LenPassword = 16;//密码长度，最新版本用16个字节，老版本05用过2个字节
        bool BoolFalg = false;//返回标志符，正常true,异常false
        #region 帧结构参数
        bool BoolUp;//判断是否为上行报文,True为上行，False为下行
        bool BoolStart;//判断是否为启动站,True来自机启动站,False来自从动站
        bool BoolFen;//分帧标准位，True为片段数据，False为完整数据
        bool BoolTpv;//判断是否存在时间标签，True为存在
        bool BoolAcd;//判断是否存在事件标签，True为存在
        string strAF = "";//地址标识AF 
        string strC = "";//控制码
        string Afn; int Pn; int Fn;
        string strFrames = "";

        int intReadData = 0;
        int intWriteData = 0;
        int intStartIndex = 0;
        #endregion

        enumSelectOp enumsop1 = new enumSelectOp();

        /// <summary>
        /// 数据选择类型
        /// </summary>
        enum enumSelectOp
        {
            op01 = 1,
            op02 = 2,
            op03 = 3,
            op04 = 4,
            op05 = 5
        }

        /// <summary>
        /// 解析类型
        /// </summary>
        public enum enumDataAnType
        {
            ExplanNo = 1,
            ExplanBinFun = 2,
            ReAscill = 3,
            date_time_s = 4,
            time_s = 5,
            gd23,
                
        }

        /// <summary>
        /// 698数据类型
        /// </summary>
        public enum enumData698Type
        {
            array_698 = 1,
            structure_698 = 2,
            bool_698 = 3,
            bit_string_698 = 4,
            double_long_698 = 5,
            double_long_unsigned_698 = 6,
            octet_string_698 = 9,
            visible_string_698 = 10,
            UTF8_string_698 = 12,
            bcd_698 = 13,
            integer_698 = 15,
            long_698 = 16,
            unsigned_698 = 17,
            long_unsigned_698 = 18,
            long64_698 = 20,
            long64_unsigned_698 = 21,
            enum_698 = 22,
            float32_698 = 23,
            float64_698 = 24,
            date_time_698 = 25,
            date_698 = 26,
            time_698 = 27,
            DateTimeBC_698 = 28,
            DateTimeBCD_H_698 = 29,
            DateTimeBCD_S_698 = 30
        }

        public bool ReturnFlagFrame(string strData, ref string strErr)
        {
            //return true;
            //return true;

            bool boolfalg = true;
            string strTmp = strData;
            try
            {
                int a = strTmp.IndexOf("68");
                if (a > -1)//1.先判断有没有帧头68
                {

                    int ilen = Convert.ToInt16(strTmp.Substring(a + 2, 2), 16) + Convert.ToInt16(strTmp.Substring(a + 4, 2), 16) * 256;//3.获取帧长

                    if (strTmp.Substring(a + 2 + ilen * 2, 2) == "16")//4.判断帧尾16
                    {
                        long crc16 = Frame_698.CheckCrc16(strTmp.Substring(2, ilen * 2 - 4), ilen - 2);
                        long crc16Tmp = Convert.ToInt32(strTmp.Substring(ilen * 2 - 2, 2), 16) + Convert.ToInt32(strTmp.Substring(ilen * 2, 2), 16) * 256;
                        if (crc16 != crc16Tmp)
                        {
                            boolfalg = false;
                            strErr = "校验和不对,应为" + Frame_698.CheckCrc16(strTmp.Substring(2, ilen * 2 - 4), ilen - 2).ToString("x2");
                        }
                    }
                    else
                    {
                        boolfalg = false;
                        strErr = "无帧尾16";
                    }
                }
                else
                {
                    boolfalg = false;
                    strErr = "无帧头68";
                }
            }
            catch
            {
                strErr = "未知异常";
                boolfalg = false;//未对长度作详细判断，长度不够时，直接判无效帧
            }
            return boolfalg;
        }

        public bool Analysis(string strFrame, ref string[] strReturnAnalysisdata, ref string strReturnAlData, ref string[] strReturnAnalysisStructure)
        {
            try
            {
                strFrames = strFrame;
                string strErr = "";
                lsTmp.Clear(); lsStructure.Clear(); string strTmp = "";
                strFrameRemove = strFrame.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper();
                if (strFrameRemove.IndexOf("68") > 0)//有些带帧起死符FE，需判断删去前面的字符
                    strFrameRemove = strFrameRemove.Substring(strFrameRemove.IndexOf("68"), strFrameRemove.Length - strFrameRemove.IndexOf("68"));
                if (BoolCheck)
                {
                    if (ReturnFlagFrame(strFrameRemove, ref strErr))
                        BoolFalg = true;
                    else
                    {
                        BoolFalg = false;
                        strReturnAnalysisdata = new string[1];
                        strReturnAnalysisdata[0] = strErr;
                        strReturnAlData = strErr;
                        return false;
                    }
                }

                #region 解析帧结构

                strC = strFrameRemove.Substring(6, 2);

                BoolUp = Comm.GetAnyValue(Convert.ToInt16(strC, 16), 7, 7) == 1 ? true : false;
                BoolStart = Comm.GetAnyValue(Convert.ToInt16(strC, 16), 6, 6) == 1 ? true : false;
                BoolFen = Comm.GetAnyValue(Convert.ToInt16(strC, 16), 5, 5) == 1 ? true : false;

                BoolTpv = Comm.GetAnyValue(Convert.ToInt16(strFrameRemove.Substring(26 + (LenAddr - 2) * 2, 2), 16), 7, 7) == 1 ? true : false;
                BoolAcd = BoolUp && Comm.GetAnyValue(Convert.ToInt16(strFrameRemove.Substring(12, 2), 16), 5, 5) == 1 ? true : false;
                lsStructure.Add(" 长度：" + (Convert.ToInt16(strFrameRemove.Substring(2, 2), 16) + Convert.ToInt16(strFrameRemove.Substring(4, 2), 16) * 256));
                lsStructure.Add(" 传输方向位：" + Convert.ToInt16(BoolUp) + "," + (BoolUp ? "终端响应主站命令" : "主站向终端下发命令"));
                lsStructure.Add(" 启动标志位：" + Convert.ToInt16(BoolStart) + "," + (BoolStart ? "来自客户机" : "来自服务器"));
                lsStructure.Add(" 分帧标志位：" + Convert.ToInt16(BoolFen) + "," + (BoolFen ? "片段APDU" : "完整APDU"));
                LenAddr = Comm.GetAnyValue(Convert.ToInt16(strFrameRemove.Substring(8, 2), 16), 0, 3) + 1;

                lsStructure.Add(" 逻辑地址：" + Comm.revStr(strFrameRemove.Substring(10, LenAddr * 2)));
                lsStructure.Add(" 客户机地址：" + strFrameRemove.Substring(10 + LenAddr * 2, 2));
                lsStructure.Add(" 帧头校验：" + strFrameRemove.Substring(12 + LenAddr * 2, 4));

                strApdu = strFrameRemove.Substring(16 + LenAddr * 2, strFrameRemove.Length - 22 - LenAddr * 2);

                lsStructure.Add("");
                strReturnAnalysisStructure = lsStructure.ToArray();
                #endregion



                Afn = strApdu.Substring(0, 2);
                Afn = GetTheData(enumDataAnType.ExplanNo, 1);
                lsTmp.Add("");


                GetAfn(Afn);
                if (strApdu.Length > 0)
                    lsTmp.Add("ERR");


                strReturnAnalysisdata = lsTmp.ToArray();
                strReturnAlData = strReturnAnalysisdata[0];
                for (int i = 1; i < strReturnAnalysisdata.Length; i++)
                {
                    strReturnAnalysisdata[i] = " " + i.ToString().PadLeft(2, '0') + "." + strReturnAnalysisdata[i];
                    strReturnAlData += "\r\n" + strReturnAnalysisdata[i];
                }
            }
            catch
            {
                lsTmp.Add("ERR");
                strReturnAnalysisdata = lsTmp.ToArray();
            }
            return BoolFalg;
        }

        public bool Analysis_apdu(string strFrame, ref string[] strReturnAnalysisdata, ref string strReturnAlData)
        {
            try
            {
                strFrames = strFrame;
                string strErr = "";
                lsTmp.Clear(); lsStructure.Clear(); string strTmp = "";
                strFrameRemove = strFrame.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper();
                //if (strFrameRemove.IndexOf("68") > 0)//有些带帧起死符FE，需判断删去前面的字符
                //    strFrameRemove = strFrameRemove.Substring(strFrameRemove.IndexOf("68"), strFrameRemove.Length - strFrameRemove.IndexOf("68"));
                if (!BoolCheck)
                {
                    if (ReturnFlagFrame(strFrameRemove, ref strErr))
                        BoolFalg = true;
                    else
                    {
                        BoolFalg = false;
                        strReturnAnalysisdata = new string[1];
                        strReturnAnalysisdata[0] = strErr;
                        strReturnAlData = strErr;
                        return false;
                    }
                }

                #region 解析帧结构


                #endregion


                strApdu = strFrameRemove;
                Afn = strApdu.Substring(0, 2);
                Afn = GetTheData(enumDataAnType.ExplanNo, 1);
                lsTmp.Add("");


                GetAfn(Afn);

                if (strApdu.Length > 0)
                    lsTmp.Add("ERR");


                strReturnAnalysisdata = lsTmp.ToArray();
                strReturnAlData = strReturnAnalysisdata[0];
                for (int i = 1; i < strReturnAnalysisdata.Length; i++)
                {
                    strReturnAnalysisdata[i] = " " + i.ToString().PadLeft(2, '0') + "." + strReturnAnalysisdata[i];
                    strReturnAlData += "\r\n" + strReturnAnalysisdata[i];
                }
            }
            catch
            {
                lsTmp.Add("ERR");
                strReturnAnalysisdata = lsTmp.ToArray();
            }
            return BoolFalg;
        }

        public void GetAfn(string afn)
        {
            #region 临时变量
            int i1 = 0;
            int i2 = 0;
            int i3 = 0;
            int i4 = 0;
            int i5 = 0;
            string s1 = "";
            string s2 = "";
            string s3 = "";
            string stime = "";
            string soad = "";
            #endregion
            switch (afn)
            {
                case "01":
                    lsTmp.Add("【Afn = " + Afn + ",预连接】");
                    lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                    s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                    if (s1 == "00")
                    {
                        lsTmp.Add("登录：00");
                        lsTmp.Add("心跳周期：" + GetTheData(enumDataAnType.ExplanBinFun, 2));
                        lsTmp.Add("请求时间：" + GetTheData(enumDataAnType.date_time_s, 7));
                        GetTheData(enumDataAnType.ExplanNo, 3);
                    }
                    else if (s1 == "01")
                    {
                        lsTmp.Add("心跳：01");
                        lsTmp.Add("心跳周期：" + GetTheData(enumDataAnType.ExplanBinFun, 2));
                        lsTmp.Add("请求时间：" + GetTheData(enumDataAnType.date_time_s, 7));
                        GetTheData(enumDataAnType.ExplanNo, 3);
                    }
                    break;
                case "81":
                    lsTmp.Add("【Afn = " + Afn + ",预连接响应】");
                    lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                    s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                    if (s1 == "00")
                    {
                        lsTmp.Add("结果Result可信：00");
                        lsTmp.Add("请求时间：" + GetTheData(enumDataAnType.date_time_s, 7));
                        GetTheData(enumDataAnType.ExplanNo, 3);
                        lsTmp.Add("收到时间：" + GetTheData(enumDataAnType.date_time_s, 7));
                        GetTheData(enumDataAnType.ExplanNo, 3);
                        lsTmp.Add("响应时间：" + GetTheData(enumDataAnType.date_time_s, 7));
                        GetTheData(enumDataAnType.ExplanNo, 3);
                    }
                    else if (s1 == "01")
                    {

                    }
                    break;
                    break;
                case "02":
                    lsTmp.Add("【Afn = " + Afn + ",建立应用连接请求】");
                    lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));

                    lsTmp.Add("期望的应用层协议版本号：" + GetTheData(enumDataAnType.ExplanBinFun, 2));

                    lsTmp.Add("期望的协议一致性块：" + GetTheData(enumDataAnType.ExplanNo, 8));
                    lsTmp.Add("期望的协议一致性块：" + GetTheData(enumDataAnType.ExplanNo, 16));

                    lsTmp.Add("客户机发送帧最大尺寸：" + GetTheData(enumDataAnType.ExplanBinFun, 2));
                    lsTmp.Add("客户机接收帧最大尺寸：" + GetTheData(enumDataAnType.ExplanBinFun, 2));

                    lsTmp.Add("客户机接收帧最大窗口尺寸：" + GetTheData(enumDataAnType.ExplanBinFun, 1));
                    lsTmp.Add("客户机最大可处理APDU尺寸：" + GetTheData(enumDataAnType.ExplanBinFun, 2));

                    lsTmp.Add("期望的应用连接超时时间：" + GetTheData(enumDataAnType.ExplanBinFun, 4));
                    lsTmp.Add("数字签名：" + GetTheData(enumDataAnType.ExplanBinFun, 1));
                    i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);

                    lsTmp.Add("密文2：" + GetTheData(enumDataAnType.ExplanNo, i1));

                    i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);

                    lsTmp.Add("客户机签名2：" + GetTheData(enumDataAnType.ExplanNo, i1));
                    GetTimeLable();
                    break;
                case "82":
                    lsTmp.Add("【Afn = " + Afn + ",建立应用连接响应】");
                    lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));

                    lsTmp.Add(" 厂商代码：" + GetTheData(enumDataAnType.ReAscill, 4));

                    lsTmp.Add("软件版本号：" + GetTheData(enumDataAnType.ReAscill, 4));
                    lsTmp.Add("软件版本日期：" + GetTheData(enumDataAnType.ReAscill, 6));

                    lsTmp.Add("硬件版本号：" + GetTheData(enumDataAnType.ReAscill, 4));
                    lsTmp.Add("硬件版本日期：" + GetTheData(enumDataAnType.ReAscill, 6));
                    lsTmp.Add("厂家扩展信息：" + GetTheData(enumDataAnType.ReAscill, 8));
                    lsTmp.Add("期望的应用层协议版本号：" + GetTheData(enumDataAnType.ExplanBinFun, 2));
                    lsTmp.Add("期望的协议一致性块：" + GetTheData(enumDataAnType.ExplanNo, 8));
                    lsTmp.Add("期望的协议一致性块：" + GetTheData(enumDataAnType.ExplanNo, 16));
                    lsTmp.Add("客户机发送帧最大尺寸：" + GetTheData(enumDataAnType.ExplanBinFun, 2));
                    lsTmp.Add("客户机接收帧最大尺寸：" + GetTheData(enumDataAnType.ExplanBinFun, 2));
                    lsTmp.Add("客户机接收帧最大窗口尺寸：" + GetTheData(enumDataAnType.ExplanBinFun, 1));
                    lsTmp.Add("客户机最大可处理APDU尺寸：" + GetTheData(enumDataAnType.ExplanBinFun, 2));
                    lsTmp.Add("期望的应用连接超时时间：" + GetTheData(enumDataAnType.ExplanBinFun, 4));
                    i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);

                    lsTmp.Add("认证结果：" + i1);

                    i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);

                    lsTmp.Add("认证附加信息：" + i1);
                    GetTheData(enumDataAnType.ExplanNo, 1);
                    lsTmp.Add("RN：" + GetTheData(enumDataAnType.ExplanNo, 48));
                    GetTheData(enumDataAnType.ExplanNo, 1);
                    lsTmp.Add("服务器签名信息：" + GetTheData(enumDataAnType.ExplanNo, 64));
                    stime = GetTheData(enumDataAnType.ExplanNo, 1);

                    lsTmp.Add("跟随上报信息域：" + stime);
                    GetTimeLable();
                    break;
                case "05":
                    s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                    if (s1 == "01")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",读取一个对象属性】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                        lsTmp.Add(getOAD2(soad) + "：" + soad);

                    }
                    else if (s1 == "02")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",读取多个对象属性】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));

                        i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        lsTmp.Add("OAD个数：" + i1);
                        for (int i = 0; i < i1; i++)
                        {
                            soad = GetTheData(enumDataAnType.ExplanNo, 4);
                            lsTmp.Add(getOAD2(soad) + "：" + soad);

                        }

                    }
                    else if (s1 == "03")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",读取一个记录型对象属性】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));

                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                        lsTmp.Add(getOAD2(soad) + "：" + soad);

                        int selectType = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));

                        if (selectType == 1)
                        {
                            lsTmp.Add("Selector01");
                            soad = GetTheData(enumDataAnType.ExplanNo, 4);
                            lsTmp.Add(getOAD2(soad) + "：" + soad);
                            DataAnalsis();
                            i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                            lsTmp.Add("CSD：" + i1);
                            for (int i = 0; i < i1; i++)
                            {
                                GetTheData(enumDataAnType.ExplanNo, 1);
                                soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                lsTmp.Add(getOAD2(soad) + "：" + soad);
                            }
                        }
                        else if (selectType == 2)
                        {
                            lsTmp.Add("Selector02");
                            soad = GetTheData(enumDataAnType.ExplanNo, 4);
                            lsTmp.Add(getOAD2(soad) + "：" + soad);
                            GetTheData(enumDataAnType.ExplanNo, 1);
                            lsTmp.Add("起始值：" + GetTheData(enumDataAnType.date_time_s, 7));
                            GetTheData(enumDataAnType.ExplanNo, 1);
                            lsTmp.Add("结束值：" + GetTheData(enumDataAnType.date_time_s, 7));
                            DataAnalsis();
                            i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                            lsTmp.Add("CSD：" + i1);
                            for (int i = 0; i < i1; i++)
                            {
                                i2 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                                if (i2 == 1)
                                {
                                    soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                    lsTmp.Add(getOAD2(soad) + "：" + soad);
                                    i3 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                                    for (int j = 0; j < i3; j++)
                                    {
                                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                        lsTmp.Add(getOAD2(soad) + "：" + soad);
                                    }
                                }
                                else
                                {

                                    soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                    lsTmp.Add(getOAD2(soad) + "：" + soad);
                                }
                            }
                        }
                        else if (selectType == 5)
                        {
                            lsTmp.Add("Selector05");

                            lsTmp.Add("采集存储时间：" + GetTheData(enumDataAnType.date_time_s, 7));
                            s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                            if (s1 == "01")
                                lsTmp.Add("数据：" + "全部电能表合集");
                            else if (s1 == "03")
                            {
                                lsTmp.Add("数据：" + "一组用户地址");
                                i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                                for (int i = 0; i < i1; i++)
                                {
                                    GetTheData(enumDataAnType.ExplanNo, 2);
                                    lsTmp.Add("数据：" + GetTheData(enumDataAnType.ExplanNo, 6));
                                }
                            }


                            i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                            lsTmp.Add("CSD：" + i1);
                            for (int i = 0; i < i1; i++)
                            {
                                i2 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                                if (i2 == 1)
                                {
                                    soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                    lsTmp.Add(getOAD2(soad) + "：" + soad);
                                    i3 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                                    for (int j = 0; j < i3; j++)
                                    {
                                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                        lsTmp.Add(getOAD2(soad) + "：" + soad);
                                    }
                                }
                                else
                                {

                                    soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                    lsTmp.Add(getOAD2(soad) + "：" + soad);
                                }
                            }
                        }
                        else if (selectType == 7)
                        {
                            lsTmp.Add("Selector07");

                            lsTmp.Add("开始时间：" + GetTheData(enumDataAnType.date_time_s, 7));
                            lsTmp.Add("结束时间：" + GetTheData(enumDataAnType.date_time_s, 7));
                            lsTmp.Add("time：" + GetTheData(enumDataAnType.ExplanBinFun, 3));
                            s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                            if (s1 == "01")
                                lsTmp.Add("数据：" + "全部电能表合集");
                            else
                            {

                            }


                            i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                            lsTmp.Add("CSD：" + i1);
                            for (int i = 0; i < i1; i++)
                            {
                                s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                                if (s1 == "00")
                                {
                                    soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                    lsTmp.Add(getOAD2(soad) + "：" + soad);
                                }
                                else
                                {
                                    soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                    lsTmp.Add(getOAD2(soad) + "：" + soad);
                                    i2 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                                    for (int j = 0; j < i2; j++)
                                    {
                                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                        lsTmp.Add(getOAD2(soad) + "：" + soad);
                                    }
                                }
                            }
                        }
                        else if (selectType == 9)
                        {
                            lsTmp.Add("Selector09");

                            i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                            lsTmp.Add("上第n次记录：" + i1);

                            i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                            lsTmp.Add("CSD：" + i1);
                            for (int i = 0; i < i1; i++)
                            {
                                GetTheData(enumDataAnType.ExplanNo, 1);
                                soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                lsTmp.Add(getOAD2(soad) + "：" + soad);
                            }
                        }
                    }
                    GetTimeLable();
                    break;
                case "88":
                    s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                    if (s1 == "01")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",上报一个记录型对象属性】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        lsTmp.Add("A-ResultNormal个数：" + i1);
                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                        lsTmp.Add(getOAD2(soad) + "：" + soad);
                        s1 = GetTheData(enumDataAnType.ExplanNo, 1);

                        if (s1 == "01")
                        {
                            lsTmp.Add("结果类型，正确：" + s1);
                            DataAnalsis();
                        }
                    }
                    else if (s1 == "02")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",上报若干个记录型对象属性】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        lsTmp.Add("A-ResultNormal个数：" + i1);
                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                        lsTmp.Add(getOAD2(soad) + "：" + soad);
                        i2 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        lsTmp.Add("CSD个数：" + i2);

                        for (int i = 0; i < i2; i++)
                        {
                            s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                            if (s1 == "00")
                            {
                                soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                lsTmp.Add(getOAD2(soad) + "：" + soad);
                            }
                            else if (s1 == "01")
                            {
                                soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                lsTmp.Add(getOAD2(soad) + "：" + soad);
                                i5 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                                for (int j = 0; j < i5; j++)
                                {
                                    soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                    lsTmp.Add(getOAD2(soad) + "：" + soad);
                                }
                            }
                        }
                        i3 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        i3 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        lsTmp.Add("记录个数：" + i3);

                        for (int i = 0; i < i3 * i2; i++)
                        {
                            DataAnalsis();
                        }
                    }
                    stime = GetTheData(enumDataAnType.ExplanNo, 1);
                    lsTmp.Add("跟随上报信息域：" + stime);
                    GetTimeLable();
                    break;
                case "85":
                    s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                    if (s1 == "01")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",读取一个对象属性响应】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                        lsTmp.Add(getOAD2(soad) + "：" + soad);
                        s1 = GetTheData(enumDataAnType.ExplanNo, 1);

                        if (s1 == "01")
                        {
                            lsTmp.Add("结果类型，正确：" + s1);
                            DataAnalsis();
                        }
                        else
                        {
                            lsTmp.Add("结果类型，错误：" + s1);
                            lsTmp.Add("DARType：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        }

                    }
                    else if (s1 == "02")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",读取多个对象属性应】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        lsTmp.Add("OAD个数：" + i1);
                        for (int i = 0; i < i1; i++)
                        {
                            soad = GetTheData(enumDataAnType.ExplanNo, 4);
                            lsTmp.Add(getOAD2(soad) + "：" + soad);
                            s1 = GetTheData(enumDataAnType.ExplanNo, 1);

                            if (s1 == "01")
                            {
                                lsTmp.Add("结果类型，正确：" + s1);
                                DataAnalsis();
                            }
                            else
                            {
                                lsTmp.Add("结果类型，错误：" + s1);
                                lsTmp.Add("DARType：" + GetTheData(enumDataAnType.ExplanNo, 1));
                            }
                        }
                    }
                    else if (s1 == "03")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",读取一个记录型对象属性响应】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));

                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                        lsTmp.Add(getOAD2(soad) + "：" + soad);

                        i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        lsTmp.Add("CSD：" + i1);
                        for (int i = 0; i < i1; i++)
                        {
                            s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                            if (s1 == "00")
                            {
                                soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                lsTmp.Add(getOAD2(soad) + "：" + soad);
                            }
                            else if (s1 == "01")
                            {
                                soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                lsTmp.Add(getOAD2(soad) + "：" + soad);
                                i5 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                                for (int j = 0; j < i5; j++)
                                {
                                    soad = GetTheData(enumDataAnType.ExplanNo, 4);
                                    lsTmp.Add(getOAD2(soad) + "：" + soad);
                                }
                            }
                        }

                        i2 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        if (i2 == 1)
                        {
                            lsTmp.Add("结果类型：数据" + i2);

                            i2 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                            lsTmp.Add("数据长度：" + i2);

                            for (int i = 0; i < i2 * i1; i++)
                            {
                                DataAnalsis();
                            }
                        }
                        else if (i2 == 0)
                        {
                            lsTmp.Add("结果类型：数据" + i2);
                            DataAnalsis();
                        }
                    }
                    stime = GetTheData(enumDataAnType.ExplanNo, 1);
                    lsTmp.Add("跟随上报信息域：" + stime);
                    GetTimeLable();
                    break;
                case "09":
                    s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                    if (s1 == "01")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",代理读取若干个服务器的若干个对象属性请求】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));

                        lsTmp.Add("整个代理请求的超时时间：" + GetTheData(enumDataAnType.ExplanBinFun, 2));
                        lsTmp.Add("代理服务器：" + GetTheData(enumDataAnType.ExplanBinFun, 1));
                        GetTheData(enumDataAnType.ExplanBinFun, 2);

                        lsTmp.Add("目标服务器地址：" + GetTheData(enumDataAnType.ExplanNo, 6));
                        lsTmp.Add("代理一个服务器的超时时间：" + GetTheData(enumDataAnType.ExplanBinFun, 2));

                        i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        lsTmp.Add("OAD个数：" + i1);
                        for (int i = 0; i < i1; i++)
                        {
                            soad = GetTheData(enumDataAnType.ExplanNo, 4);
                            lsTmp.Add(getOAD2(soad) + "：" + soad);
                        }
                        GetTimeLable();
                    }
                    else if (s1 == "07")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",代理透明转发命令请求】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                        lsTmp.Add(getOAD2(soad) + "：" + soad);
                        lsTmp.Add("COMDCB：" + GetTheData(enumDataAnType.ExplanNo, 5));
                        lsTmp.Add("接收等待报文超时时间（秒）：" + GetTheData(enumDataAnType.ExplanBinFun, 2));
                        lsTmp.Add("接收等待字节超时时间（毫秒）：" + GetTheData(enumDataAnType.ExplanBinFun, 2));

                        i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        lsTmp.Add("长度：" + i1);
                        lsTmp.Add("内容：" + GetTheData(enumDataAnType.ExplanNo, i1));
                        GetTimeLable();
                    }
                    break;
                case "89":
                    s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                    if (s1 == "01")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",代理读取若干个服务器的若干个对象属性响应】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        lsTmp.Add("代理服务器的读取结果：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        GetTheData(enumDataAnType.ExplanBinFun, 2);
                        lsTmp.Add("目标服务器地址：" + GetTheData(enumDataAnType.ExplanNo, 6));

                        i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        lsTmp.Add("OAD个数：" + i1);
                        for (int i = 0; i < i1; i++)
                        {
                            soad = GetTheData(enumDataAnType.ExplanNo, 4);
                            lsTmp.Add(getOAD2(soad) + "：" + soad);


                            if (strApdu.Substring(0, 2) == "01")
                            {
                                s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                                lsTmp.Add("结果类型，正确：" + s1);
                                DataAnalsis();
                            }
                            else
                            {
                                s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                                lsTmp.Add("结果类型，错误：" + s1);
                                GetTheData(enumDataAnType.ExplanNo, 1);
                            }
                        }
                    }
                    else if (s1 == "07")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",代理操作透明转发命令响应】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                        lsTmp.Add(getOAD2(soad) + "：" + soad);


                        s1 = GetTheData(enumDataAnType.ExplanNo, 1);

                        if (s1 == "01")
                        {
                            i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                            lsTmp.Add("长度：" + i1);
                            lsTmp.Add("内容：" + GetTheData(enumDataAnType.ExplanNo, i1));

                        }
                        else
                        {
                            lsTmp.Add("结果类型，错误：" + s1);
                            lsTmp.Add("DARType：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        }
                    }
                    stime = GetTheData(enumDataAnType.ExplanNo, 1);
                    lsTmp.Add("跟随上报信息域：" + stime);
                    GetTimeLable();
                    break;
                case "06":
                    s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                    if (s1 == "01")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",设置一个对象属性】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                        lsTmp.Add(getOAD2(soad) + "：" + soad);

                        DataAnalsis();

                        GetTimeLable();
                    }
                    else if (s1 == "02")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",设置多个对象属性】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        lsTmp.Add("OAD个数：" + i1);
                        for (int i = 0; i < i1; i++)
                        {
                            soad = GetTheData(enumDataAnType.ExplanNo, 4);
                            lsTmp.Add(getOAD2(soad) + "：" + soad);

                            DataAnalsis();
                        }

                        GetTimeLable();
                    }
                    break;
                case "86":
                    s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                    if (s1 == "01")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",设置一个对象属性响应】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                        lsTmp.Add(getOAD2(soad) + "：" + soad);
                        s1 = GetTheData(enumDataAnType.ExplanNo, 1);

                        if (s1 == "01")
                        {

                            lsTmp.Add("结果类型，错误：" + s1);
                            lsTmp.Add("DARType：" + GetTheData(enumDataAnType.ExplanNo, 1));
                            DataAnalsis();
                        }
                        else
                        {
                            lsTmp.Add("结果类型，正确：" + s1);
                        }
                        stime = GetTheData(enumDataAnType.ExplanNo, 1);
                        lsTmp.Add("跟随上报信息域：" + stime);
                        GetTimeLable();
                    }
                    else if (s1 == "02")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",设置多个对象属性响应】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        lsTmp.Add("OAD个数：" + i1);
                        for (int i = 0; i < i1; i++)
                        {
                            soad = GetTheData(enumDataAnType.ExplanNo, 4);
                            lsTmp.Add(getOAD2(soad) + "：" + soad);
                            s1 = GetTheData(enumDataAnType.ExplanNo, 1);

                            if (s1 == "00")
                            {
                                lsTmp.Add("结果类型，正确：" + s1);
                            }
                            else
                            {
                                lsTmp.Add("结果类型，错误：" + s1);
                                lsTmp.Add("DARType：" + GetTheData(enumDataAnType.ExplanNo, 1));
                            }
                        }
                        stime = GetTheData(enumDataAnType.ExplanNo, 1);
                        lsTmp.Add("跟随上报信息域：" + stime);
                        GetTimeLable();
                    }
                    break;


                case "07":
                    s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                    if (s1 == "01")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",操作一个对象属性】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                        lsTmp.Add(getOAD2(soad) + "：" + soad);

                        DataAnalsis();
                        GetTimeLable();
                    }
                    else if (s1 == "02")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",操作多个对象属性】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        lsTmp.Add("OAD个数：" + i1);
                        for (int i = 0; i < i1; i++)
                        {
                            soad = GetTheData(enumDataAnType.ExplanNo, 4);
                            lsTmp.Add(getOAD2(soad) + "：" + soad);

                            DataAnalsis();
                        }
                        GetTimeLable();
                    }
                    break;
                case "87":
                    s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                    if (s1 == "01")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",操作一个对象属性响应】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                        lsTmp.Add(getOAD2(soad) + "：" + soad);
                        s1 = GetTheData(enumDataAnType.ExplanNo, 1);

                        if (s1 == "01")
                        {
                            lsTmp.Add("结果类型，正确：" + s1);
                            DataAnalsis();
                        }
                        else
                        {
                            lsTmp.Add("结果类型，正确：" + s1);
                            lsTmp.Add("DARType：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        }
                        stime = GetTheData(enumDataAnType.ExplanNo, 1);
                        lsTmp.Add("跟随上报信息域：" + stime);
                        GetTimeLable();
                    }
                    else if (s1 == "02")
                    {
                        lsTmp.Add("【Afn = " + Afn + ",操作多个对象属性响应】");
                        lsTmp.Add("服务器优先级：" + GetTheData(enumDataAnType.ExplanNo, 1));
                        i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        lsTmp.Add("OAD个数：" + i1);
                        for (int i = 0; i < i1; i++)
                        {
                            soad = GetTheData(enumDataAnType.ExplanNo, 4);
                            lsTmp.Add(getOAD2(soad) + "：" + soad);
                            s1 = GetTheData(enumDataAnType.ExplanNo, 1);

                            if (s1 == "00")
                            {
                                lsTmp.Add("结果类型，正确：" + s1);
                            }
                            else
                            {
                                lsTmp.Add("结果类型，错误：" + s1);
                                lsTmp.Add("DARType：" + GetTheData(enumDataAnType.ExplanNo, 1));
                            }
                        }
                        stime = GetTheData(enumDataAnType.ExplanNo, 1);
                        lsTmp.Add("跟随上报信息域：" + stime);
                        GetTimeLable();
                    }
                    break;

                case "10":
                    lsTmp.Add("【Afn = " + Afn + ",安全请求】");
                    i3 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);

                    if (i3 == 0)//明文+随机数
                    {
                        i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                        if (i1 >> 7 > 0)
                        {
                            i2 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, i1 % 128), 16);
                            lsTmp.Add("明文应用数据单元：" + GetTheData(enumDataAnType.ExplanNo, i2));
                        }
                        else
                        {
                            string str1 = strApdu.Substring(0, i1 * 2);
                            lsTmp.Add("明文应用数据单元：" + str1);
                            string str2 = strApdu.Substring(0, 2);
                            Afn = str2;
                            strApdu = strApdu.Remove(0, 2);
                            GetAfn(str2);

                            i2 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);

                            if (i2 == 0)
                            {
                                lsTmp.Add("标识：" + GetTheData(enumDataAnType.ExplanNo, 4));
                                i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                                lsTmp.Add("附加数据：" + GetTheData(enumDataAnType.ExplanNo, i1));
                                i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                                lsTmp.Add("MAC：" + GetTheData(enumDataAnType.ExplanNo, i1));
                            }
                            else if (i2 == 1)
                            {
                                i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                                lsTmp.Add("随机数：" + GetTheData(enumDataAnType.ExplanNo, i1));
                            }
                            else if (i2 == 2)
                            {
                                i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                                lsTmp.Add("随机数：" + GetTheData(enumDataAnType.ExplanNo, i1));
                                i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                                lsTmp.Add("MAC：" + GetTheData(enumDataAnType.ExplanNo, i1));
                            }
                        }
                    }
                    else if (i3 == 1)//密钥+MAC
                    {
                        i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                        if (i1 >> 7 > 0)
                        {
                            i2 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, i1 % 128), 16);
                            lsTmp.Add("密文应用数据单元：" + GetTheData(enumDataAnType.ExplanNo, i2));
                        }
                        else
                        {
                            lsTmp.Add("密文应用数据单元：" + GetTheData(enumDataAnType.ExplanNo, i1));
                        }
                        Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                        lsTmp.Add("标识：" + GetTheData(enumDataAnType.ExplanNo, 4));
                        i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                        lsTmp.Add("附加数据：" + GetTheData(enumDataAnType.ExplanNo, i1));
                        i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                        lsTmp.Add("MAC：" + GetTheData(enumDataAnType.ExplanNo, i1));
                    }
                    break;
                case "90":
                    lsTmp.Add("【Afn = " + Afn + ",安全响应】");
                    i4 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                    if (i4 == 0)//明文+随机数
                    {
                        i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                        if (i1 >> 7 > 0)
                        {
                            i2 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, i1 % 128), 16);
                            string str1 = strApdu.Substring(0, i2 * 2);
                            lsTmp.Add("明文应用数据单元：" + str1);
                            string str2 = strApdu.Substring(0, 2);
                            Afn = str2;
                            strApdu = strApdu.Remove(0, 2);

                            GetAfn(str2);

                            i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                            i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                            i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                            lsTmp.Add("MAC：" + GetTheData(enumDataAnType.ExplanNo, i1));
                        }
                        else
                        {
                            string str1 = strApdu.Substring(0, i1 * 2);
                            lsTmp.Add("明文应用数据单元：" + str1);
                            string str2 = strApdu.Substring(0, 2);
                            Afn = str2;
                            strApdu = strApdu.Remove(0, 2);

                            GetAfn(str2);

                            i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                            i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                            i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                            lsTmp.Add("MAC：" + GetTheData(enumDataAnType.ExplanNo, i1));
                        }
                    }
                    else if (i4 == 1)//密钥+MAC
                    {
                        i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                        if (i1 >> 7 > 0)
                        {
                            i2 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, i1 % 128), 16);
                            lsTmp.Add("密文应用数据单元：" + GetTheData(enumDataAnType.ExplanNo, i2));
                        }
                        else
                        {
                            lsTmp.Add("密文应用数据单元：" + GetTheData(enumDataAnType.ExplanNo, i1));
                        }
                        i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                        lsTmp.Add("数据验证信息：" + i1);
                        i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                        lsTmp.Add("数据验证方式：" + i1);
                        i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                        lsTmp.Add("MAC：" + GetTheData(enumDataAnType.ExplanNo, i1));
                    }
                    break;

            }
        }

        public void GetTimeLable()
        {
            string stime = GetTheData(enumDataAnType.ExplanNo, 1);
            if (stime == "01")
            {
                lsTmp.Add("时间标签：有");
                lsTmp.Add("采集存储时间：" + GetTheData(enumDataAnType.date_time_s, 7));
                string sdw = GetTheData(enumDataAnType.ExplanNo, 1);
                switch (sdw)
                {
                    case "00":
                        lsTmp.Add("单位：秒"); break;
                    case "01":
                        lsTmp.Add("单位：分"); break;
                    case "02":
                        lsTmp.Add("单位：时"); break;
                    case "03":
                        lsTmp.Add("单位：日"); break;
                    case "04":
                        lsTmp.Add("单位：月"); break;
                    case "05":
                        lsTmp.Add("单位：年"); break;
                }
                lsTmp.Add("允许传输延时时间：" + GetTheData(enumDataAnType.ExplanBinFun, 2));
            }
            else
                lsTmp.Add("时间标签：无");
        }

        public string GetTheData(enumDataAnType functionName, int ilen)
        {
            if (strApdu.Length < ilen * 2)
            {
                strApdu = "";
                return "";
            }
            object[] objTmp = new Object[] { strApdu.Substring(0, ilen * 2) };
            strApdu = strApdu.Remove(0, ilen * 2);
            MethodInfo TheFunction = typeof(Comm698).GetMethod(functionName.ToString());
            return TheFunction.Invoke(new Comm698(), objTmp).ToString();
        }



        public string getOAD2(string soad)
        {
            string soadTmp = "";
            for (int i = 0; i < 3; i++)
            {
                soadTmp += soad.Substring(i * 2, 2) + " ";
            }
            soadTmp += soad.Substring(6, 2);
            switch (soadTmp)
            {
                case "00 10 02 00": return "正向有功电能，总及费率电能量数组";
                case "00 20 02 00": return "反向有功电能，总及费率电能量数组";
                case "00 00 02 00": return "组合有功电能，总及费率电能量数组";
                case "00 30 02 00": return "组合无功1电能，总及费率电能量数组";
                case "00 30 02 01": return "组合无功1电能，总及费率电能量数组";
                case "00 40 02 00": return "组合无功2电能，总及费率电能量数组";
                case "00 40 02 01": return "组合无功2电能，总及费率电能量数组";
                case "00 50 02 00": return "第一象限无功电能，总及费率电能量数组";
                case "00 60 02 00": return "第二象限无功电能，总及费率电能量数组";
                case "00 70 02 00": return "第三象限无功电能，总及费率电能量数组";
                case "00 80 02 00": return "第四象限无功电能，总及费率电能量数组";

                case "10 10 02 00": return "正向有功最大需量，总及费率最大需量数组";
                case "10 20 02 00": return "反向有功最大需量，总及费率最大需量数组";
                case "10 30 02 00": return "正向无功最大需量，总及费率最大需量数组";
                case "10 40 02 00": return "反向无功最大需量，总及费率最大需量数组";

                case "20 00 02 00": return "电压，分相数值组";
                case "20 01 02 00": return "电流，分相数值组";
                case "20 02 02 00": return "电压相角，分相数值组";
                case "20 03 02 00": return "电流相角，分相数值组";
                case "20 04 02 00": return "有功功率，总及分相数值组";
                case "20 05 02 00": return "无功功率，总及分相数值组";
                case "20 06 02 00": return "视在功率，总及分相数值组";
                case "20 0A 02 00": return "功率因数，总及分相数值组";
                case "20 0F 02 00": return "电网频率，总及分相数值组";
                case "20 14 02 00": return "电能表运行状态字，数值";
                case "20 2A 02 00": return "目标服务器地址，TSA";
                case "20 21 02 00": return "数据冻结时间，参数";
                case "20 22 02 00": return "事件记录序号";
                case "20 1E 02 00": return "事件发生时间";
                case "20 20 02 00": return "事件结束时间";
                case "20 24 02 00": return "事件发生源";
                case "22 00 02 00": return "通信流量";
                case "22 03 02 00": return "供电时间";
                case "22 04 02 00": return "复位次数";
                case "23 01 11 01": return "  —— 总加组1，总加组当前控制状态";
                case "24 01 07 00": return "  —— 脉冲计量1，当日正向有功电量";
                case "24 01 03 00": return "  —— 脉冲计量1，互感器倍率";
                case "24 01 05 00": return "  —— 脉冲计量1，有功功率";

                case "50 02 02 00": return "分钟冻结，属性2";
                case "50 04 02 00": return "日冻结，属性2";
                case "50 06 02 00": return "月冻结，属性2";

                case "60 00 02 00": return "采集档案配置表，配置表";
                case "60 00 7F 00": return "采集档案配置表，添加一个采集档案配置单元";
                case "60 00 80 00": return "采集档案配置表，批量添加采集档案配置单元";
                case "60 00 86 00": return "采集档案配置表，清空采集档案配置表";
                case "60 01 02 01": return "采集档案配置单元，采集档案配置单元";
                case "60 01 02 00": return "采集档案配置单元，采集档案配置单元";
                case "60 12 81 00": return "任务配置表，清空任务配置表";
                case "60 14 81 00": return "普通采集方案集，清空普通采集方案，";
                case "60 12 7F 00": return "任务配置表，添加或更新一组任务配置单元";
                case "60 14 7F 00": return "普通采集方案集，添加或更新一组普通采集方案";
                case "60 34 02 00": return "采集任务监控集，采集任务监控集";
                case "60 35 02 01": return "采集任务监控单元，属性2";
                case "60 12 03 00": return "任务配置表，属性3";
                case "60 35 02 00": return "采集任务监控单元，属性2";
                case "60 40 02 00": return "采集启动时标，采集启动时标";
                case "60 41 02 00": return "采集成功时标，采集成功时标";
                case "60 42 02 00": return "采集存储时标，采集存储时标";
                case "60 16 7F 00": return "事件采集方案集，添加或更新一组事件采集方案";

                case "80 01 02 00": return "保电，保电状态";
                case "80 01 03 00": return "保电，允许与主站最大无通信时长";
                case "80 01 81 00": return "保电，投入保电";
                case "80 02 02 00": return "催费告警，催费告警状态";
                case "80 02 7F 00": return "催费告警，催费告警投入";
                case "80 02 80 00": return "催费告警，取消催费告警";
                case "81 00 02 00": return "终端保安定值";

                case "F2 03 04 00": return "直流模拟量，设备对象列表";
                case "F2 01 02 01": return "RS485";
                case "F2 03 02 00": return "开关量输入，设备对象列表";
                case "F1 00 02 00": return "ESAM序列号";
                case "F1 00 04 00": return "对称密钥版本";
                case "F1 00 07 00": return "ESAM当前计数器";
                case "F1 00 0A 00": return "终端证书";
                case "F1 00 0C 00": return "主站证书";
                case "F1 01 02 00": return "安全模式参数";
                case "F2 00 00 00": return "  —— RS232，属性0";
                case "F2 0A 02 01": return "  —— 脉冲输入设备，设备对象列表";

                case "40 01 02 00": return "通信地址";
                case "43 00 08 00": return "电气设备，允许主动上报";
                case "43 00 03 00": return "电气设备，版本信息";
                case "43 00 04 00": return "电气设备，生产日期";
                case "43 00 06 00": return "规约列表";
                case "45 00 04 00": return "公网通信模块1，短信通信参数表";
                case "45 00 05 00": return "公网通信模块1，版本信息";
                case "45 10 02 00": return "以太网通信模块1，通信配置";
                case "45 00 04 00 ": return "公网通信模块1，通信配置";
                case "40 00 02 00": return "日期时间";
                case "40 03 02 00": return "客户编号";
                case "40 06 02 00": return "时钟源";
                case "41 03 02 00": return "资产管理编码";
                case "40 04 02 00": return "设备地理位置";
                case "42 04 02 00": return "终端广播校时，终端广播校时参数";
                case "45 10 00 00": return "以太网通信模块1，";

                case "30 00 06 00": return "电能表失压事件";
                case "30 01 06 00": return "电能表欠压事件";
                case "30 02 06 00": return "电能表过压事件";
                case "30 03 06 00": return "电能表断相事件";
                case "30 04 06 00": return "电能表失流事件";
                case "30 05 06 00": return "电能表过流事件";
                case "30 06 06 00": return "电能表断流事件";
                case "30 07 06 00": return "电能表功率反向事件";
                case "30 08 06 00": return "电能表过载事件";
                case "30 09 06 00": return "电能表正向有功需量超限事件";

                case "30 15 07 00": return "电能表事件清零事件，属性7";
                case "30 17 07 00": return "电能表时段表编程事件，属性7";
                case "30 03 0A 00": return "电能表断相事件，当前值记录表";
                case "31 00 06 00": return "终端初始化事件，配置参数";
                case "31 01 06 00": return "终端版本变更事件，配置参数";
                case "31 04 06 00": return "终端状态量变位事件，配置参数";
                case "31 04 09 00": return "  —— 终端状态量变位事件，有效标识";
                case "31 04 02 01": return "终端状态量变位事件";
                case "31 05 06 00": return "电能表时钟超差事件，配置参数";
                case "31 06 06 00": return "终端停/上电事件，配置参数";
                case "31 06 06 02": return "终端停/上电事件，配置参数";
                case "31 06 06 01": return "终端停/上电事件，配置参数";
                case "31 07 06 00": return "终端直流模拟量越上限事件，配置参数";
                case "31 08 06 00": return "终端直流模拟量越下限事件，配置参数";
                case "31 09 06 00": return "终端消息认证错误事件，配置参数";
                case "31 0A 06 00": return "设备故障记录，配置参数";
                case "31 0B 02 00": return "电能表示度下降事件，属性2";
                case "31 0B 06 00": return "电能表示度下降事件，配置参数";
                case "31 0B 09 00": return "电能表示度下降事件，有效标识";
                case "31 0C 06 00": return "电能量超差事件，配置参数";
                case "31 0D 06 00": return "电能表飞走事件，配置参数";
                case "31 0E 06 00": return "电能表停走事件，配置参数";
                case "31 0F 06 00": return "终端抄表失败事件，配置参数";
                case "31 10 06 00": return "月通信流量超限事件，配置参数";
                case "31 11 06 00": return "发现未知电能表事件，配置参数";
                case "31 12 06 00": return "跨台区电能表事件，配置参数";
                case "31 14 06 00": return "终端对时事件，配置参数";
                case "31 15 06 00": return "遥控跳闸记录，配置参数";
                case "31 16 06 00": return "有功总电能量差动越限事件记录，配置参数";
                case "31 17 06 00": return "输出回路接入状态变位事件记录，配置参数";
                case "31 18 06 00": return "终端编程记录，配置参数";
                case "31 19 06 00": return "终端电流回路异常事件，配置参数";
                case "31 1A 06 00": return "电能表在网状态切换事件，配置参数";
                case "31 1B 06 00": return "终端对电表校时记录，配置参数";
                case "31 1C 06 00": return "电能表数据变更监控记录，配置参数";
                case "32 00 06 00": return "功控跳闸记录，配置参数";
                case "32 01 06 00": return "电控跳闸记录，配置参数";
                case "32 02 06 00": return "购电参数设置记录，配置参数";
                case "32 03 06 00": return "电控告警事件记录，配置参数";

                case "45 00 00 00": return "  —— 公网通信模块1，属性0";
                case "23 01 01 00": return "  —— 总加组1，清空总加配置表";
                case "24 01 02 00": return "  —— 脉冲计量1，通信地址";
                case "23 01 03 00": return "  —— 总加组1，添加一个总加配置单元";
                case "23 01 07 01": return "  —— 总加组1，总加日有功电量";
                case "23 01 09 01": return "  —— 总加组1，总加月有功电量";
                case "60 00 83 00": return "  —— 采集档案配置表，删除配置单元";
                case "60 1E 7F 00": return "  —— 采集规则库，添加或更新一组采集规则";
                case "43 00 01 00": return "  —— 电气设备，复位";
                case "31 14 09 00": return "  —— 终端对时事件，有效标识";
                case "33 00 02 00": return "  —— 通道上报状态，属性2";
                case "30 0F 02 00": return "  —— 电能表电压逆相序事件，属性2";
                case "31 06 09 00": return "  —— 终端停/上电事件，有效标识";
                case "31 0D 09 00": return "  —— 电能表飞走事件，有效标识";
                case "31 0D 02 00": return "  —— 电能表飞走事件，属性2";
                case "31 0E 09 00": return "  —— 电能表停走事件，有效标识";
                case "31 0E 02 00": return "  —— 电能表停走事件，属性2";
                case "31 05 02 00": return "  —— 电能表时钟超差事件，属性2";
                case "31 06 02 00": return "  —— 终端停/上电事件，属性2";
                case "33 09 02 06": return "  —— 停上电事件记录单元，属性2";
                case "32 02 02 00": return "  —— 购电参数设置记录，属性2";
                case "31 0A 02 00": return "  —— 终端故障记录，属性2";
                case "31 18 02 00": return "  —— 终端编程记录，属性2";
                case "33 02 02 06": return "  —— 编程记录事件单元，属性2";
                case "31 0C 02 00": return "  —— 电能量超差事件，属性2";
                case "31 0F 02 00": return "  —— 终端抄表失败事件，属性2";
                case "60 16 81 00": return "  —— 事件采集方案集，清空事件采集方案集";
                case "30 09 02 00": return "  —— 电能表正向有功需量超限事件，属性2";
                case "30 0A 02 00": return "  —— 电能表反向有功需量超限事件，属性2";
                case "30 0B 06 00": return "  —— 电能表无功需量超限事件，属性6";
                case "30 0C 02 00": return "  —— 电能表功率因数超下限事件，属性2";
                case "30 0D 02 00": return "  —— 电能表全失压事件，属性2";
                case "30 0E 02 00": return "  —— 电能表辅助电源掉电事件，属性2";
                case "30 10 02 00": return "  —— 电能表电流逆相序事件，属性2";
                case "30 11 02 00": return "  —— 电能表掉电事件，属性2";
                case "30 12 02 00": return "  —— 电能表编程事件，属性2";
                case "30 13 02 00": return "  —— 电能表清零事件，属性2";
                case "30 14 02 00": return "  —— 电能表需量清零事件，属性2";
                case "30 15 02 00": return "  —— 电能表事件清零事件，事件记录表";
                case "30 16 02 00": return "  —— 电能表校时事件，属性2";
                case "30 17 02 00": return "  —— 电能表时段表编程事件，属性2";
                case "30 18 02 00": return "  —— 电能表时区表编程事件，属性2";
                case "30 19 02 00": return "  —— 电能表周休日编程事件，属性2";
                case "30 1A 02 00": return "  —— 电能表结算日编程事件，属性2";
                case "30 1B 02 00": return "  —— 电能表开盖事件，属性2";
                case "30 1C 02 00": return "  —— 电能表开端钮盒事件，属性2";
                case "30 1D 02 00": return "  —— 电能表电压不平衡事件，属性2";
                case "30 1E 02 00": return "  —— 电能表电流不平衡事件，属性2";
                case "30 1F 02 00": return "  —— 电能表跳闸事件，属性2";
                case "30 20 02 00": return "  —— 电能表合闸事件，属性2";
                case "30 21 02 00": return "  —— 电能表节假日编程事件，属性2";
                case "30 22 02 00": return "  —— 电能表有功组合方式编程事件，属性2";
                case "30 23 02 00": return "  —— 电能表无功组合方式编程事件，属性2";
                case "30 24 02 00": return "  —— 电能表费率参数表编程事件，属性2";
                case "30 25 02 00": return "  —— 电能表阶梯表编程事件，属性2";
                case "30 26 02 00": return "  —— 电能表密钥更新事件，属性2";
                case "30 27 02 00": return "  —— 电能表异常插卡事件，属性2";
                case "30 28 02 00": return "  —— 电能表购电记录，属性2";
                case "30 29 02 00": return "  —— 电能表退费记录，属性2";
                case "30 2A 02 00": return "  —— 电能表恒定磁场干扰事件，属性2";
                case "30 2B 02 00": return "  —— 电能表负荷开关误动作事件，属性2";
                case "30 2C 02 00": return "  —— 电能表电源异常事件，属性2";
                case "30 2D 02 00": return "  —— 电能表电流严重不平衡事件 ，属性2";
                case "30 2E 02 00": return "  —— 电能表时钟故障事件，属性2";
                case "30 2F 02 00": return "  —— 电能表计量芯片故障事件，属性2";
                case "41 12 02 00": return "  —— 有功组合方式特征字，参数";
                case "31 1C 02 01": return "  —— 电能表数据变更监控记录，属性2";
                case "31 05 09 00": return "  —— 电能表时钟超差事件，有效标识";
                case "30 0F 09 00": return "  —— 电能表电压逆相序事件，有效标识";
                case "30 0F 06 00": return "  —— 电能表电压逆相序事件，配置参数";
                case "32 02 09 00": return "  —— 购电参数设置记录，有效标识";
                case "81 07 05 00": return "  —— 购电控，方法5";
                case "31 0A 09 00": return "  —— 终端故障记录，有效标识";
                case "31 18 09 00": return "  —— 终端编程记录，有效标识";
                case "31 0C 09 00": return "  —— 电能量超差事件，有效标识";
                case "31 0F 09 00": return "  —— 终端抄表失败事件，有效标识";
                case "31 1C 09 00": return "  —— 电能表数据变更监控记录，属性9";
                case "F2 00 02 01": return "  —— RS232，设备对象列表";
                case "F2 0C 02 01": return "  —— 230M无线专网接口，设备对象列表";
                case "20 0A 02 01": return "  —— 功率因数,属性2";
                default: return "未知OAD";
            }
        }   

        public void DataAnalsis()
        {
            int datalen = 0;
            int i1 = 0; int i2 = 0;
            string s1 = "";
            string soad = "";
            int datatype = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);

            switch (datatype)
            {
                case 1:
                    datalen = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                    for (int i = 0; i < datalen; i++)
                        DataAnalsis();
                    break;
                case 20:
                    lsTmp.Add("数据：" + Convert.ToInt64(GetTheData(enumDataAnType.ExplanBinFun, 8)));
                    break;
                case 23:
                    lsTmp.Add("数据：" + Convert.ToString(GetTheData(enumDataAnType.gd23, 4)));
                    break;
                case 3:
                    lsTmp.Add("数据：" + Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1)));
                    break;
                case 4:
                    int bitLen = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                    lsTmp.Add("数据：" + Comm698.ExplanWei(GetTheData(enumDataAnType.ExplanNo, bitLen / 8)));
                    break;
                case 22:
                    lsTmp.Add("数据：" + Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1)));
                    break;
                case 9:
                    i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                    if (i1 >> 7 > 0)
                    {
                        i2 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, i1 % 128), 16);
                        lsTmp.Add("数据：" + GetTheData(enumDataAnType.ExplanNo, i2));
                    }
                    else
                    {
                        lsTmp.Add("数据：" + GetTheData(enumDataAnType.ExplanNo, i1));
                    }
                    break;
                case 2:
                    datalen = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                    for (int i = 0; i < datalen; i++)
                        DataAnalsis();
                    break;
                case 6:
                    lsTmp.Add("数据：" + Convert.ToUInt32(GetTheData(enumDataAnType.ExplanBinFun, 4)));
                    break;
                case 10:
                    i1 = Convert.ToInt32(GetTheData(enumDataAnType.ExplanNo, 1), 16);
                    lsTmp.Add("数据：" + GetTheData(enumDataAnType.ReAscill, i1));
                    break;
                case 27:
                    lsTmp.Add("数据：" + (GetTheData(enumDataAnType.time_s, 3)));
                    break;
                case 28:
                    lsTmp.Add("数据：" + (GetTheData(enumDataAnType.date_time_s, 7)));
                    break;
                case 18:
                    lsTmp.Add("数据：" + Convert.ToUInt32(GetTheData(enumDataAnType.ExplanBinFun, 2)));
                    break;
                case 17:
                    lsTmp.Add("数据：" + Convert.ToUInt32(GetTheData(enumDataAnType.ExplanBinFun, 1)));
                    break;
                case 81:
                    s1 = GetTheData(enumDataAnType.ExplanNo, 4);
                    lsTmp.Add("数据(OAD)：" + getOAD2(s1) + "，" + s1);
                    break;
                case 82:

                    soad = GetTheData(enumDataAnType.ExplanNo, 4);
                    lsTmp.Add(getOAD2(soad) + "：" + soad);
                    i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));

                    for (int i = 0; i < i1; i++)
                    {
                        soad = GetTheData(enumDataAnType.ExplanNo, 4);
                        lsTmp.Add(getOAD2(soad) + "：" + soad);
                    }

                    break;
                case 84:
                    //有问题
                    GetTheData(enumDataAnType.ExplanNo, 1);
                    lsTmp.Add("数据：" + GetTheData(enumDataAnType.ExplanBinFun, 2));
                    break;
                case 85:
                    //有问题
                    //i1= Convert.ToInt16( GetTheData(enumDataAnType.ExplanNo, 1));
                    int li = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun,1));

                    GetTheData(enumDataAnType.ExplanNo, 1);
                    lsTmp.Add("数据：" + GetTheData(enumDataAnType.ExplanNo, li-1));
                    break;
                case 0:
                    //TODO
                    lsTmp.Add("数据：" + "NULL");
                    break;
                case 16:
                    long l1 = Convert.ToUInt16(GetTheData(enumDataAnType.ExplanBinFun, 2));
                    if (l1 > 32767)
                        l1 = l1 - 65535;
                    lsTmp.Add("数据：" + l1);
                    break;
                case 5:
                    double l2 = Convert.ToUInt32(GetTheData(enumDataAnType.ExplanBinFun, 4));
                    if (l2 > 2147483647)
                        l2 = l2 - 4294967297;
                    lsTmp.Add("数据：" + l2);
                    break;
                case 91:
                    i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                    soad = GetTheData(enumDataAnType.ExplanNo, 4);
                    lsTmp.Add(getOAD2(soad) + "：" + soad);
                    if (i1 == 1)
                    {
                        i2 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        for (int i = 0; i < i2; i++)
                        {
                            soad = GetTheData(enumDataAnType.ExplanNo, 4);
                            lsTmp.Add(getOAD2(soad) + "：" + soad);
                        }
                    }
                    break;
                case 92:
                    s1 = GetTheData(enumDataAnType.ExplanNo, 1);
                    if (s1 == "01")
                        lsTmp.Add("数据：" + "全部电能表合集");
                    else if (s1 == "03")
                    {
                        lsTmp.Add("数据：" + "一组用户地址");
                        i1 = Convert.ToInt16(GetTheData(enumDataAnType.ExplanBinFun, 1));
                        for (int i = 0; i < i1; i++)
                        {
                            GetTheData(enumDataAnType.ExplanNo, 2);
                            lsTmp.Add("数据：" + GetTheData(enumDataAnType.ExplanNo, 6));
                        }
                    }
                    break;
                case 15:
                    lsTmp.Add("数据：" + GetTheData(enumDataAnType.ExplanBinFun, 1));
                    break;
                case 80:
                    lsTmp.Add("数据：" + GetTheData(enumDataAnType.ExplanNo, 2));
                    break;
                case 95://串口控制块数据类型COMDCB
                    string stmp = "";
                    s1 = GetTheData(enumDataAnType.ExplanBinFun, 1);
                    switch (s1)
                    {
                        case "0":
                            stmp = "300bps"; break;
                        case "1":
                            stmp = "600bps"; break;
                        case "2":
                            stmp = "1200bps"; break;
                        case "3":
                            stmp = "240bps"; break;
                        case "4":
                            stmp = "480bps"; break;
                        case "5":
                            stmp = "720bps"; break;
                        case "6":
                            stmp = "9600bps"; break;
                        case "7":
                            stmp = "19200bps"; break;
                        case "8":
                            stmp = "38400bps"; break;
                        case "9":
                            stmp = "57600bps"; break;
                        case "10":
                            stmp = "115200bps"; break;
                        case "255":
                            stmp = "自适应"; break;
                    }
                    lsTmp.Add("数据：波特率," + stmp);
                    s1 = GetTheData(enumDataAnType.ExplanBinFun, 1);
                    switch (s1)
                    {
                        case "0":
                            stmp = "无校验"; break;
                        case "1":
                            stmp = "奇检验"; break;
                        case "2":
                            stmp = "偶检验"; break;
                    }
                    lsTmp.Add("数据：校验位," + stmp);
                    s1 = GetTheData(enumDataAnType.ExplanBinFun, 1);
                    lsTmp.Add("数据：数据位," + s1);
                    s1 = GetTheData(enumDataAnType.ExplanBinFun, 1);
                    lsTmp.Add("数据：停止位," + s1);
                    s1 = GetTheData(enumDataAnType.ExplanBinFun, 1);
                    switch (s1)
                    {
                        case "0":
                            stmp = "无"; break;
                        case "1":
                            stmp = "硬件"; break;
                        case "2":
                            stmp = "软件"; break;
                    }
                    lsTmp.Add("数据：流控," + stmp);
                    break;
                default:
                    GetTheData(enumDataAnType.ExplanNo, 1);
                    lsTmp.Add("数据：" + GetTheData(enumDataAnType.ExplanBinFun, 2));
                    break;
            }
        }
    }
}
