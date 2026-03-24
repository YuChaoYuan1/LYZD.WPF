using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace LYZD.TerminalProtocol.GW
{
    /// <summary>
    /// 最新2013版本
    /// </summary>
    [ComVisible(true)]
    public class Analysis_3761_2013new
    {
        int iversions;//版本号,默认为376.1协议
        protected List<string> lsTmp = new List<string>();//数据集合
        protected List<string> lsStructure = new List<string>();//结构集合
        string strFrameRemove = "";//帧数据，采用位移移除数据，迭代解析
        public bool BoolCheck = true;//是否校验,True为校验
        public int LenAddr = 2;//地址长度，吉林用的4个字节，其它地方2个字节
        public int LenPassword = 16;//密码长度，最新版本用16个字节，老版本05用过2个字节
        bool BoolFalg = false;//返回标志符，正常true,异常false
        #region 帧结构参数
        bool BoolUp;//判断是否为上行报文,True为上行，False为下行
        bool BoolStart;//判断是否为启动站,True来自机启动站,False来自从动站
        bool BoolTpv;//判断是否存在时间标签，True为存在
        bool BoolAcd;//判断是否存在事件标签，True为存在
        string Afn; int Pn; int Fn;
        string strFrames = "";

        int intReadData = 0;
        int intWriteData = 0;
        int intStartIndex = 0;
        #endregion

        /// <summary>
        /// 解析，帧不合格返回异常
        /// </summary>
        /// <param name="strFrame">解析数据</param>
        /// <param name="iver">版本号，主要应对不同地方的细微差别</param>
        /// <param name="strReturnAnalysisdata">返回的解析数据</param>
        /// <param name="strReturnAnalysisStructure">返回的帧结构</param>
        /// <returns></returns>
        public bool Analysis(string strFrame, int iver, ref string[] strReturnAnalysisdata, ref string strReturnAlData, ref string[] strReturnAnalysisStructure)
        {
            try
            {
                strFrames = strFrame;
                string strErr = "";
                lsTmp.Clear(); lsStructure.Clear(); string strTmp = "";
                iversions = iver; strFrameRemove = strFrame.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper();
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
                Afn = strFrameRemove.Substring(24 + (LenAddr - 2) * 2, 2);
                BoolUp = Comm.GetAnyValue(Convert.ToInt16(strFrameRemove.Substring(12, 2), 16), 7, 7) == 1 ? true : false;
                BoolStart = Comm.GetAnyValue(Convert.ToInt16(strFrameRemove.Substring(12, 2), 16), 6, 6) == 1 ? true : false;
                BoolTpv = Comm.GetAnyValue(Convert.ToInt16(strFrameRemove.Substring(26 + (LenAddr - 2) * 2, 2), 16), 7, 7) == 1 ? true : false;
                BoolAcd = BoolUp && Comm.GetAnyValue(Convert.ToInt16(strFrameRemove.Substring(12, 2), 16), 5, 5) == 1 ? true : false;
                lsStructure.Add(" L：" + (Convert.ToInt16(strFrameRemove.Substring(2, 2), 16) / 4 + Convert.ToInt16(strFrameRemove.Substring(4, 2), 16) * 64));
                lsStructure.Add(" DIR：" + Convert.ToInt16(BoolUp) + "," + (BoolUp ? "上行报文" : "下行报文"));
                lsStructure.Add(" PRM：" + Convert.ToInt16(BoolStart) + "," + (BoolStart ? "来自启动站" : "来自从动站"));
                lsStructure.Add(" ACD：" + Convert.ToInt16(BoolAcd) + "," + (BoolAcd ? "有事件" : "无事件"));
                int iTmp = Comm.GetAnyValue(Convert.ToInt16(strFrameRemove.Substring(12, 2), 16), 0, 3);
                if (BoolStart)
                {
                    switch (iTmp)
                    {
                        case 0:
                            strTmp += "——	备用"; break;
                        case 1:
                            strTmp += "发送∕确认	复位命令"; break;
                        case 4:
                            strTmp += "发送∕无回答	用户数据"; break;
                        case 9:
                            strTmp += "请求∕响应帧	链路测试"; break;
                        case 10:
                            strTmp += "请求∕响应帧	请求1级数据"; break;
                        case 11:
                            strTmp += "请求∕响应帧	请求2级数据"; break;
                        default:
                            strTmp += "——	备用"; break;

                    }
                }
                else
                {
                    switch (iTmp)
                    {
                        case 0:
                            strTmp += "确认	认可"; break;
                        case 8:
                            strTmp += "响应帧	用户数据"; break;
                        case 9:
                            strTmp += "响应帧	否认：无所召唤的数据"; break;
                        case 11:
                            strTmp += "响应帧	链路状态"; break;
                        default:
                            strTmp += "——	备用"; break;
                    }
                }

                lsStructure.Add(" 功能码：" + iTmp + "," + strTmp);
                lsStructure.Add(" A1：" + Comm.revStr(strFrameRemove.Substring(14, 4)));
                lsStructure.Add(" A2：" + Comm.revStr(strFrameRemove.Substring(18, 4 + (LenAddr - 2) * 2)));
                lsStructure.Add(" A3：" + strFrameRemove.Substring(22 + (LenAddr - 2) * 2, 2));
                lsStructure.Add(" Tpv：" + Convert.ToInt16(BoolTpv) + "," + (BoolTpv ? "有时间标签" : "无时间标签"));
                lsStructure.Add(" FIR：" + Comm.GetAnyValue(Convert.ToInt16(strFrameRemove.Substring(26 + (LenAddr - 2) * 2, 2), 16), 6, 6));
                lsStructure.Add(" FIN：" + Comm.GetAnyValue(Convert.ToInt16(strFrameRemove.Substring(26 + (LenAddr - 2) * 2, 2), 16), 5, 5));
                lsStructure.Add(" CON：" + Comm.GetAnyValue(Convert.ToInt16(strFrameRemove.Substring(26 + (LenAddr - 2) * 2, 2), 16), 4, 4));
                lsStructure.Add(" PSEQ：" + Comm.GetAnyValue(Convert.ToInt16(strFrameRemove.Substring(26 + (LenAddr - 2) * 2, 2), 16), 0, 3));
                lsStructure.Add("");
                strReturnAnalysisStructure = lsStructure.ToArray();
                #endregion



                intStartIndex = 28 + (LenAddr - 2) * 2;
                intReadData = strFrameRemove.Length - 32 - (BoolTpv ? 12 : 0) - (BoolAcd ? 4 : 0) - (LenAddr - 2) * 2;
                intWriteData = intReadData - LenPassword * 2;

                switch (Afn)
                {
                    case "00":
                        lsTmp.Add("【Afn = " + Afn + ",确认/否认命令】");
                        strFrameRemove = strFrameRemove.Substring(intStartIndex, intReadData);
                        a00();
                        break;
                    case "01":
                        lsTmp.Add("【Afn = " + Afn + ",复位命令】");
                        strFrameRemove = strFrameRemove.Substring(intStartIndex, intWriteData);
                        a01();
                        break;
                    case "02":
                        lsTmp.Add("【Afn = " + Afn + ",链路接口检测】");
                        strFrameRemove = strFrameRemove.Substring(intStartIndex, intReadData);
                        a02();
                        break;
                    case "03":
                        lsTmp.Add("【Afn = " + Afn + ",中继站命令】");
                        strFrameRemove = strFrameRemove.Substring(intStartIndex, intReadData);
                        a03();
                        break;
                    case "04":
                        lsTmp.Add("【Afn = " + Afn + ",设置参数】");
                        strFrameRemove = strFrameRemove.Substring(intStartIndex, intWriteData);
                        a040A();
                        break;
                    case "05":
                        lsTmp.Add("【Afn = " + Afn + ",控制命令】");
                        strFrameRemove = strFrameRemove.Substring(intStartIndex, intWriteData);
                        a05();
                        break;
                    case "06":
                        lsTmp.Add("【Afn = " + Afn + ",身份认证及密钥协商】");
                        if (BoolUp)
                            strFrameRemove = strFrameRemove.Substring(intStartIndex, intReadData);
                        else
                            strFrameRemove = strFrameRemove.Substring(intStartIndex, intWriteData);
                        a06();
                        break;
                    case "08":
                        lsTmp.Add("【Afn = " + Afn + ",请求被级联终端主动上报】");
                        strFrameRemove = strFrameRemove.Substring(intStartIndex, intReadData);
                        a08();
                        break;
                    case "09":
                        lsTmp.Add("【Afn = " + Afn + ",请求终端配置】");
                        strFrameRemove = strFrameRemove.Substring(intStartIndex, intReadData);
                        a09();
                        break;
                    case "0A":
                        lsTmp.Add("【Afn = " + Afn + ",查询参数】");
                        strFrameRemove = strFrameRemove.Substring(intStartIndex, intReadData);
                        a040A();
                        break;
                    case "0B":
                        lsTmp.Add("【Afn = " + Afn + ",请求任务数据】");
                        strFrameRemove = strFrameRemove.Substring(intStartIndex, intReadData);
                        a0B();
                        break;
                    case "0C":
                        lsTmp.Add("【Afn = " + Afn + ",请求1类数据】");
                        strFrameRemove = strFrameRemove.Substring(intStartIndex, intReadData);
                        a0C();
                        break;
                    case "0D":
                        lsTmp.Add("【Afn = " + Afn + ",确请求2类数据】");
                        strFrameRemove = strFrameRemove.Substring(intStartIndex, intReadData);
                        a0D();
                        break;
                    case "0E":
                        lsTmp.Add("【Afn = " + Afn + ",请求3类数据】");
                        strFrameRemove = strFrameRemove.Substring(intStartIndex, intReadData);
                        a0E();
                        break;
                    case "0F":
                        lsTmp.Add("【Afn = " + Afn + ",文件传输】");
                        strFrameRemove = strFrameRemove.Substring(intStartIndex, intReadData);
                        a0F();
                        break;
                    case "10":
                        lsTmp.Add("【Afn = " + Afn + ",数据转发】");
                        if (BoolUp)
                            strFrameRemove = strFrameRemove.Substring(intStartIndex, intReadData);
                        else
                            strFrameRemove = strFrameRemove.Substring(intStartIndex, intWriteData);
                        a10();
                        break;
                }

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

            }
            return BoolFalg;
        }

        /// <summary>
        /// 返回376.1是否含有有效帧
        /// </summary>
        /// <returns></returns>
        public bool ReturnFlagFrame(string strData, ref string strErr)
        {
            bool boolfalg = true;
            string strTmp = strData;
            try
            {
                int a = strTmp.IndexOf("68");
                if (a > -1)//1.先判断有没有帧头68
                {
                    if (strTmp.Substring(a + 10, 2) == "68")//2.判断第2个68
                    {
                        int ilen = Convert.ToInt16(strTmp.Substring(a + 2, 2), 16) / 4 + Convert.ToInt16(strTmp.Substring(a + 4, 2), 16) * 64;//3.获取帧长

                        if (strTmp.Substring(a + 14 + ilen * 2, 2) == "16")//4.判断帧尾16
                        {
                            if (Comm.GetChkSum(strTmp.Substring(a + 12, ilen * 2)) != Convert.ToByte(strTmp.Substring(a + 12 + ilen * 2, 2), 16))//5.判断校验和
                            {
                                boolfalg = false;
                                strErr = "校验和不对,应为" + Comm.GetChkSum(strTmp.Substring(a + 12, ilen * 2)).ToString("x2");
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
                        strErr = "无第二个68";
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

        public void a00()
        {
            try
            {
                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    case 1:
                        lsTmp.Add("全部确认：Fn1"); break;
                    case 2:
                        lsTmp.Add("全部否认：Fn2"); break;
                    case 3:
                        lsTmp.Add("按数据单元标识确认和否认：Fn3"); strFrameRemove = ""; break;
                    case 4:
                        lsTmp.Add("硬件安全认证错误应答：Fn4"); strFrameRemove = ""; break;
                    default:
                        lsTmp.Add("ERR：" + "Fn" + Fn); break;
                }
                if (strFrameRemove.Length > 0)
                    a00();
            }
            catch
            {

            }
        }

        public void a01()
        {

            try
            {
                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    case 1:
                        lsTmp.Add("Fn = " + Fn + ",硬件初始化："); break;
                    case 2:
                        lsTmp.Add("Fn = " + Fn + ",数据区初始化："); break;
                    case 3:
                        lsTmp.Add("Fn = " + Fn + ",参数及全体数据区初始化(即恢复至出厂配置)："); break;
                    case 4:
                        lsTmp.Add("Fn = " + Fn + ",参数(除与系统主站通信有关的)及全体数据区初始化："); break;
                    default:
                        lsTmp.Add("Fn = " + Fn + ",ERR"); break;
                }
                if (strFrameRemove.Length > 0)
                    a01();
            }
            catch
            {

            }
        }

        public void a02()
        {

            try
            {
                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    case 1:
                        lsTmp.Add("Fn = " + Fn + ",登录："); break;
                    case 2:
                        lsTmp.Add("Fn = " + Fn + ",退出登录："); break;
                    case 3:
                        lsTmp.Add("Fn = " + Fn + ",心跳："); break;
                    default:
                        lsTmp.Add("Fn = " + Fn + ",备用"); break;
                }
                //if (strFrameRemove.Length > 0)
                //    a02();
            }
            catch
            {

            }
        }

        public void a03()
        {

            try
            {
                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    default:
                        lsTmp.Add("Fn = " + Fn + ",ERR"); break;
                }
                if (strFrameRemove.Length > 0)
                    a03();
            }
            catch
            {

            }

        }

        public void a05()
        {
            int ins1 = 0; int ins2 = 0;
            try
            {
                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    case 1:
                        lsTmp.Add("Fn = " + Fn + ",遥控跳闸");
                        lsTmp.Add("告警延时时间：" + GetTheData("ExplanNo", 1));
                        break;
                    case 2:
                        lsTmp.Add("Fn = " + Fn + ",允许合闸：");
                        break;
                    case 9:
                        lsTmp.Add("Fn = " + Fn + ",时段功控投入：");
                        lsTmp.Add("时段功控投入标志：" + GetTheData("ExplanNo", 1));
                        lsTmp.Add("时段功控定值方案号：" + GetTheData("ExplanNo", 1));
                        break;
                    case 10:
                        lsTmp.Add("Fn = " + Fn + ",厂休功控投入：");
                        break;
                    case 11:
                        lsTmp.Add("Fn = " + Fn + ",营业报停控投入：");
                        break;
                    case 12:
                        lsTmp.Add("Fn = " + Fn + ",当前功率下浮控投入");
                        GetOneData(new string[]{
                            "当前功率下浮控定值滑差时间","ExplanNo","1",
                            "当前功率下浮控定值浮动系数","gd4","1",
                            "控后总加有功功率冻结延时时间","ExplanNo","1",
                            "当前功率下浮控的控制时间","ExplanNo","1",
                            "当前功率下浮控第1轮告警时间","ExplanNo","1",
                            "当前功率下浮控第2轮告警时间","ExplanNo","1",
                            "当前功率下浮控第3轮告警时间","ExplanNo","1",
                            "当前功率下浮控第4轮告警时间","ExplanNo","1"});
                        break;
                    case 15:
                        lsTmp.Add("Fn = " + Fn + ",月电控投入："); break;
                    case 16:
                        lsTmp.Add("Fn = " + Fn + ",购电控投入："); break;
                    case 17:
                        lsTmp.Add("Fn = " + Fn + ",时段功控解除："); break;
                    case 18:
                        lsTmp.Add("Fn = " + Fn + ",厂休功控解除："); break;
                    case 19:
                        lsTmp.Add("Fn = " + Fn + ",营业报停控解除："); break;
                    case 20:
                        lsTmp.Add("Fn = " + Fn + ",当前功率下浮控解除："); break;
                    case 23:
                        lsTmp.Add("Fn = " + Fn + ",月电控解除："); break;
                    case 24:
                        lsTmp.Add("Fn = " + Fn + ",购电控解除："); break;
                    case 25:
                        lsTmp.Add("Fn = " + Fn + ",终端保电投入：");
                        lsTmp.Add("保电持续时间4轮告警时间：" + Convert.ToInt16(GetTheData("ExplanNo", 1)) * 0.5 + "小时");
                        break;
                    case 26:
                        lsTmp.Add("Fn = " + Fn + ",催费告警投入："); break;
                    case 27:
                        lsTmp.Add("Fn = " + Fn + ",允许终端与主站通话："); break;
                    case 28:
                        lsTmp.Add("Fn = " + Fn + ",终端剔除投入："); break;
                    case 29:
                        lsTmp.Add("Fn = " + Fn + ",允许终端主动上报："); break;
                    case 30:
                        lsTmp.Add("Fn = " + Fn + ",终端对电能表对时功能的开启与关闭：");
                        lsTmp.Add("开启状态：" + (GetTheData("ExplanBinFun", 1) == "1" ? "开启" : "关闭"));
                        break;
                    case 31:
                        lsTmp.Add("Fn = " + Fn + ",对时命令：");
                        lsTmp.Add("秒、分、时、日、星期-月、年：" + GetTheData("gd1", 6));
                        break;
                    case 33:
                        lsTmp.Add("Fn = " + Fn + ",终端保电解除："); break;
                    case 34:
                        lsTmp.Add("Fn = " + Fn + ",催费告警解除："); break;
                    case 35:
                        lsTmp.Add("Fn = " + Fn + ",禁止终端与主站通话："); break;
                    case 36:
                        lsTmp.Add("Fn = " + Fn + ",终端剔除解除："); break;
                    case 37:
                        lsTmp.Add("Fn = " + Fn + ",禁止终端主动上报："); break;
                    case 38:
                        lsTmp.Add("Fn = " + Fn + ",激活终端连接主站："); break;
                    case 39:
                        lsTmp.Add("Fn = " + Fn + ",命令终端断开连接："); break;
                    case 41:
                        lsTmp.Add("Fn = " + Fn + ",电容器控制投入：");
                        lsTmp.Add("电容器组：" + GetTheData("ExplanNo", 2));
                        break;
                    case 42:
                        lsTmp.Add("Fn = " + Fn + ",电容器控制切除：");
                        lsTmp.Add("电容器组：" + GetTheData("ExplanNo", 2));
                        break;
                    case 49:
                        lsTmp.Add("Fn = " + Fn + ",指定通信端口暂停抄表：");
                        lsTmp.Add("终端通信端口号：" + GetTheData("ExplanBinFun", 1));
                        break;
                    case 50:
                        lsTmp.Add("Fn = " + Fn + ",命令指定通信端口恢复抄表：");
                        lsTmp.Add("终端通信端口号：" + GetTheData("ExplanBinFun", 1));
                        break;
                    case 51:
                        lsTmp.Add("Fn = " + Fn + ",命令指定通信端口重新抄表：");
                        lsTmp.Add("终端通信端口号：" + GetTheData("ExplanBinFun", 1));
                        break;
                    case 52:
                        lsTmp.Add("Fn = " + Fn + ",初始化指定通信端口下的全部中继路由信息：");
                        lsTmp.Add("终端通信端口号：" + GetTheData("ExplanBinFun", 1));
                        break;
                    case 53:
                        lsTmp.Add("Fn = " + Fn + ",删除指定通信端口下的全部电表：");
                        lsTmp.Add("终端通信端口号：" + GetTheData("ExplanBinFun", 1));
                        break;
                    case 57:
                        lsTmp.Add("Fn = " + Fn + ",删除指定事件代码下的全部事件：");
                        ins1 = Convert.ToInt32(GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("本次清除的事件个数：" + ins1);
                        for (int i = 1; i <= ins1; i++)
                        {
                            lsTmp.Add("第" + i + "个事件代码ERC：" + GetTheData("ExplanBinFun", 1));
                        }
                        break;
                    case 58:
                        lsTmp.Add("Fn = " + Fn + ",停止搜索表地址：");
                        break;
                    default:
                        lsTmp.Add("ERR"); break;
                }
                if (strFrameRemove.Length > 0)
                    a05();
            }
            catch
            {

            }
        }

        public void a06()
        {
            int ins = 0; int ins2 = 0; int ins3 = 0; string s1 = ""; string s2 = ""; int i1 = 0;
            try
            {
                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    case 11:
                        lsTmp.Add("Fn = " + Fn + ",获取终端信息");
                        if (BoolUp)
                        {
                            lsTmp.Add("芯片序列号：" + GetTheData("RExplanNo", 8));
                            lsTmp.Add("证书序列号：" + GetTheData("RExplanNo", 16));
                            lsTmp.Add("计数器：" + GetTheData("RExplanNo", 4));
                            lsTmp.Add("芯片状态：" + Convert.ToDouble(GetTheData("ExplanBinFun", 1)));
                            lsTmp.Add("密钥版本：" + GetTheData("RExplanNo", 8));
                        }
                        break;
                    case 12:
                        lsTmp.Add("Fn = " + Fn + ",会话初始化/恢复");
                        if (BoolUp)
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("数据长度：" + ins);
                            lsTmp.Add("版本号：" + GetTheData("ExplanBinFun", 1));
                            ins2 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("会话ID(0表示协商新的会话,1表示恢复上一个会话)：" + ins2);
                            if (ins2 == 0)
                            {
                                lsTmp.Add("CRL序列号：" + GetTheData("RExplanNo", 16));
                                lsTmp.Add("主站证书序列号：" + GetTheData("RExplanNo", 16));
                                lsTmp.Add("终端证书：" + GetTheData("RExplanNo", ins - 114));
                                lsTmp.Add("Eks2(R2)：" + GetTheData("RExplanNo", 16));
                                lsTmp.Add("签名数据S2：" + GetTheData("RExplanNo", 64));
                            }
                            else
                            {
                                lsTmp.Add("Eks2：" + GetTheData("RExplanNo", 16));
                                lsTmp.Add("R3：" + GetTheData("RExplanNo", 16));
                                lsTmp.Add("MAC3：" + GetTheData("RExplanNo", 4));
                            }
                        }
                        else
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("数据长度：" + ins);
                            lsTmp.Add("版本号：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("会话ID(0表示协商新的会话,1表示恢复上一个会话)：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("主站证书：" + GetTheData("RExplanNo", ins - 86));
                            lsTmp.Add("Eks1(R1)：" + GetTheData("RExplanNo", 16));
                            lsTmp.Add("MAC1：" + GetTheData("RExplanNo", 4));
                            lsTmp.Add("签名数据S1：" + GetTheData("RExplanNo", 64));
                        }
                        break;
                    case 13:
                        lsTmp.Add("Fn = " + Fn + ",会话协商");
                        if (BoolUp)
                        {
                            lsTmp.Add("R3：" + GetTheData("RExplanNo", 16));
                            lsTmp.Add("MAC3：" + GetTheData("RExplanNo", 4));
                        }
                        else
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("数据长度：" + ins);
                            lsTmp.Add("会话密钥密文：" + GetTheData("RExplanNo", 113));
                            lsTmp.Add("主站证书验证：" + GetTheData("RExplanNo", ins - 181));
                            lsTmp.Add("MAC2：" + GetTheData("RExplanNo", 4));
                            lsTmp.Add("签名数据S3：" + GetTheData("RExplanNo", 64));
                        }
                        break;
                    case 14:
                        lsTmp.Add("Fn = " + Fn + ",CA对称密钥更新");
                        if (BoolUp)
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("数据长度：" + ins);
                            lsTmp.Add("版本号：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("会话ID(0表示协商新的会话,1表示恢复上一个会话)：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("CRL序列号：" + GetTheData("RExplanNo", 8));
                            lsTmp.Add("主站证书序列号：" + GetTheData("RExplanNo", 8));
                            lsTmp.Add("终端证书：" + GetTheData("RExplanNo", ins - 98));
                            lsTmp.Add("Eks2(R2)：" + GetTheData("RExplanNo", 16));
                            lsTmp.Add("签名数据S2：" + GetTheData("RExplanNo", 64));
                        }
                        else
                        {
                            lsTmp.Add("密钥版本：" + GetTheData("ExplanBinFun", 1));
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("更新密钥总条数：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                lsTmp.Add("证书更新密文" + i + "：" + GetTheData("RExplanNo", 32));
                            }
                        }
                        break;
                    case 16:
                        lsTmp.Add("Fn = " + Fn + ",CA证书更新");
                        if (BoolUp)
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("数据长度：" + ins);
                            lsTmp.Add("版本号：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("会话ID(0表示协商新的会话,1表示恢复上一个会话)：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("CRL序列号：" + GetTheData("RExplanNo", 8));
                            lsTmp.Add("主站证书序列号：" + GetTheData("RExplanNo", 8));
                            lsTmp.Add("终端证书：" + GetTheData("RExplanNo", ins - 98));
                            lsTmp.Add("Eks2(R2)：" + GetTheData("RExplanNo", 16));
                            lsTmp.Add("签名数据S2：" + GetTheData("RExplanNo", 64));
                        }
                        else
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("数据长度：" + ins);
                            lsTmp.Add("证书更新密文：" + GetTheData("RExplanNo", ins));
                        }
                        break;
                    case 17:
                        lsTmp.Add("Fn = " + Fn + ",内部认证");
                        if (BoolUp)
                        {
                            lsTmp.Add("随机数R4密文：" + GetTheData("RExplanNo", 16));
                            lsTmp.Add("随机数R5：" + GetTheData("RExplanNo", 16));
                        }
                        else
                        {
                            lsTmp.Add("随机数R4：" + GetTheData("RExplanNo", 16));
                        }
                        break;
                    case 18:
                        lsTmp.Add("Fn = " + Fn + ",外部认证");
                        if (BoolUp)
                        {
                            lsTmp.Add("随机数R6：" + GetTheData("RExplanNo", 16));
                        }
                        else
                        {
                            lsTmp.Add("随机数R5密文：" + GetTheData("RExplanNo", 16));
                        }
                        break;
                    case 19:
                        lsTmp.Add("Fn = " + Fn + ",状态切换");
                        if (BoolUp)
                        {

                        }
                        else
                        {
                            lsTmp.Add("切换状态(00–从测试证书切换到正式证书01–从正式证书切换到测试证书)：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("随机数R6密文：" + GetTheData("RExplanNo", 16));
                            lsTmp.Add("MAC6：" + GetTheData("RExplanNo", 4));
                        }
                        break;
                    case 20:
                        lsTmp.Add("Fn = " + Fn + ",置离线计数器");
                        if (BoolUp)
                        {
                            lsTmp.Add("R3：" + GetTheData("RExplanNo", 16));
                            lsTmp.Add("MAC3：" + GetTheData("RExplanNo", 4));
                        }
                        else
                        {
                            lsTmp.Add("数据：" + GetTheData("RExplanNo", 20));
                        }
                        break;
                    case 21:
                        lsTmp.Add("Fn = " + Fn + ",转加密授权数据");
                        if (BoolUp)
                        {
                            lsTmp.Add("转加密授权数据：" + GetTheData("RExplanNo", 32));
                        }
                        else
                        {
                            lsTmp.Add("转加密授权数据：" + GetTheData("RExplanNo", 32));
                        }
                        break;
                    default:
                        lsTmp.Add("ERR"); break;
                }
                if (strFrameRemove.Length > 0)
                    a06();
            }
            catch
            {

            }
        }

        public void a08()
        {

            try
            {
                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    default:
                        lsTmp.Add("ERR"); break;
                }
                if (strFrameRemove.Length > 0)
                    a08();
            }
            catch
            {

            }

        }
        public void a09()
        {

            try
            {
                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    case 1:
                        GetOneData("终端版本信息", new string[]{
                            "厂商代号","ReAscill","4",
                            "设备编号","ReAscill","8",
                            "终端软件版本号","ReAscill","4",
                            "终端软件发布日期:日月年","gd20","3",
                            "终端配置容量信息码","ReAscill","11",
                            "终端通信规约版本号","ReAscill","4",
                            "终端硬件版本号","ReAscill","4",
                            "终端硬件发布日期:日月年","gd20","3"});
                        break;
                    default:
                        lsTmp.Add("ERR"); break;
                }

            }
            catch
            {

            }
        }

        public void a040A()
        {
            int ins = 0; int ins2 = 0; int ins3 = 0; string s1 = ""; string s2 = ""; int i1 = 0;
            try
            {
                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    case 1:
                        GetOneData("终端上行通信口通信参数设置", new string[]{
                            "终端数传机延时时间RTS","ExplanBinFun","1",
                            "终端作为启动站允许发送传输延时时间","ExplanBinFun","1",
                            "终端等待从动站响应的超时时间和重发次数","ExplanNo","2",
                            "需要主站确认的通信服务（CON=1）的标志","ExplanBinFun","1",
                            "心跳周期","ExplanBinFun","1"});
                        break;
                    case 2:
                        lsTmp.Add("Fn = " + Fn + ",终端上行通信口无线中继转发设置");
                        if (BoolUp || Afn == "04")
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("允许∕禁止：" + Comm.GetAnyValue(ins, 7, 7));
                            ins2 = Comm.GetAnyValue(ins, 0, 6);
                            lsTmp.Add("被转发的终端地址数：" + ins2);
                            for (int i = 1; i <= ins2; i++)
                                lsTmp.Add("被转发终端地址" + i + "：" + GetTheData("RExplanNo", 2));//(4 + 4 * i, 4)));
                        }
                        break;
                    case 3:
                        lsTmp.Add("Fn = " + Fn + ",主站IP地址和端口");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("主用IP地址：" + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("主用端口：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("备用IP地址：" + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("备用端口：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("APN：" + GetTheData("ReAscill", 16));
                        }
                        break;
                    case 4:
                        GetOneData("主站电话号码和短信中心号码", new string[]{
                            "主站电话号码","ExplanNo","8",
                            "短信中心号码","ExplanNo","8"});
                        break;
                    case 5:
                        GetOneData("终端上行通信消息认证参数设置", new string[]{
                            "消息认证方案号","ExplanBinFun","1",
                            "消息认证方案参数","ExplanBinFun","2"});
                        break;
                    case 6:
                        lsTmp.Add("Fn = " + Fn + ",终端组地址设置");
                        if (BoolUp || Afn == "04")
                        {
                            for (int i = 1; i <= 8; i++)
                                lsTmp.Add("终端组地址" + i + "：" + GetTheData("ExplanBinFun", LenAddr));//(4 + 4 * i, 4)));
                        }
                        break;
                    case 7:
                        lsTmp.Add("Fn = " + Fn + ",终端IP地址和端口");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("终端IP地址：" + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("子网掩码：" + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("网关地址：" + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("代理类型：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("代理服务器地址：" + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1) + "." + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("代理服务器端口：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("代理服务器连接方式：" + GetTheData("ExplanBinFun", 1));
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("用户名长度m：" + ins);
                            lsTmp.Add("用户名：" + GetTheData("ReAscill", ins));
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("密码长度n：" + ins);
                            lsTmp.Add("密码：" + GetTheData("ReAscill", ins));
                            lsTmp.Add("终端侦听端口：" + GetTheData("ExplanBinFun", 2));
                        }
                        break;
                    case 8:
                        lsTmp.Add("Fn = " + Fn + ",终端上行通信工作方式");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("工作模式：" + GetTheData("ExplanNo", 1));
                            lsTmp.Add("永久在线、时段在线模式重拨间隔：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("被动激活模式重拨次数：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("被动激活模式连续无通信自动断线时间：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("时段在线模式允许在线时段标志：" + GetTheData("ExplanNo", 3));
                        }
                        break;
                    case 9:
                        lsTmp.Add("Fn = " + Fn + ",终端事件记录配置设置");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("事件记录有效标志位：" + GetTheData("GetEventAl", 8));
                            lsTmp.Add("事件重要性等级标志位：" + GetTheData("GetEventAl", 8));
                        }
                        break;
                    case 10:
                        lsTmp.Add("Fn = " + Fn + ",终端电能表/交流采样装置配置参数：");
                        if (BoolUp || Afn == "04")
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("本次电能表/交流采样装置配置数量n：" + ins);
                            for (int i = 0; i < ins; i++)
                            {
                                lsTmp.Add("电能表/交流采样装置序号：" + GetTheData("ExplanBinFun", 2));
                                lsTmp.Add("所属测量点号：" + GetTheData("ExplanBinFun", 2));
                                ins2 = int.Parse(GetTheData("ExplanBinFun", 1));
                                switch (Comm.GetAnyValue(ins2, 5, 7))
                                {
                                    case 0:
                                        s1 = "无需设置或使用默认的"; break;
                                    case 1:
                                        s1 = "600"; break;
                                    case 2:
                                        s1 = "1200"; break;
                                    case 3:
                                        s1 = "2400"; break;
                                    case 4:
                                        s1 = "4800"; break;
                                    case 5:
                                        s1 = "7200"; break;
                                    case 6:
                                        s1 = "9600"; break;
                                    case 7:
                                        s1 = "19200"; break;
                                }
                                lsTmp.Add("通信速率：" + s1);
                                lsTmp.Add("端口号：" + Comm.GetAnyValue(ins2, 0, 4));
                                switch (GetTheData("ExplanBinFun", 1))
                                {
                                    case "0":
                                        s1 = "终端无需对本序号的电能表/交流采样装置进行抄表"; break;
                                    case "1":
                                        s1 = "DL/T645—1997"; break;
                                    case "2":
                                        s1 = "交流采样装置通信协议"; break;
                                    case "30":
                                        s1 = "DL/T645—2007"; break;
                                    case "31":
                                        s1 = "串行接口连接窄带低压载波通信模块"; break;
                                    default:
                                        s1 = "备用"; break;
                                }
                                lsTmp.Add("通信协议类型：" + s1);
                                lsTmp.Add("通信地址：" + GetTheData("gd12", 6));
                                lsTmp.Add("通信密码：" + GetTheData("ExplanNo", 6));
                                lsTmp.Add("电能费率个数：" + GetTheData("ExplanBinFun", 1));
                                s1 = GetTheData("ExplanNo", 1);
                                lsTmp.Add("有功电能示值整数位：" + (Comm.GetAnyValue(Convert.ToInt16(s1, 16), 2, 3) + 4));
                                lsTmp.Add("有功电能示值小数位：" + (Comm.GetAnyValue(Convert.ToInt16(s1, 16), 0, 1) + 1));
                                lsTmp.Add("所属采集器通信地址：" + GetTheData("gd12", 6));
                                s1 = GetTheData("ExplanNo", 1);
                                lsTmp.Add("用户大类号：" + Comm.GetAnyValue(Convert.ToInt16(s1, 16), 4, 7));
                                lsTmp.Add("用户小类号：" + Comm.GetAnyValue(Convert.ToInt16(s1, 16), 0, 3));
                            }
                        }
                        else
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("本次查询数量：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                lsTmp.Add("本次查询的第" + i + "个对象序号：" + GetTheData("ExplanBinFun", 2));
                            }
                        }
                        break;
                    case 11:
                        lsTmp.Add("Fn = " + Fn + ",终端脉冲配置参数：");
                        if (BoolUp || Afn == "04")
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("本次脉冲配置路数：" + ins);
                            for (int i = 0; i < ins; i++)
                            {
                                lsTmp.Add("脉冲输入端口号：" + GetTheData("ExplanBinFun", 1));
                                lsTmp.Add("所属测量点号：" + GetTheData("ExplanBinFun", 1));
                                i1 = Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 0, 1);

                                switch (i1)
                                {
                                    case 0:
                                        s1 = "正向有功"; break;
                                    case 1:
                                        s1 = "正向无功"; break;
                                    case 2:
                                        s1 = "反向有功"; break;
                                    case 3:
                                        s1 = "反向无功"; break;
                                }
                                lsTmp.Add("脉冲属性：" + s1);
                                lsTmp.Add("电表常数k：" + GetTheData("ExplanBinFun", 2));
                            }
                        }

                        break;

                    case 12:
                        GetOneData("终端状态量输入参数", new string[]{
                            "状态量接入标志位（对应1～8路状态量）","ExplanWei","1",
                            "状态量属性标志位（对应1～8路状态量）","ExplanWei","1"});
                        break;
                    case 14:
                        lsTmp.Add("Fn = " + Fn + ",终端总加组配置参数：");
                        if (BoolUp || Afn == "04")
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("本次总加组配置数量n：" + ins);
                            for (int i = 0; i < ins; i++)
                            {
                                lsTmp.Add("总加组序号：" + GetTheData("ExplanBinFun", 1));
                                i1 = Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 0, 1);
                                lsTmp.Add("总加组的测量点数量m" + (i + 1) + "：" + i1);
                                for (int j = 0; j < i1; j++)
                                {
                                    ins2 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                                    ins3 = Comm.GetAnyValue(ins2, 0, 5);
                                    lsTmp.Add("第" + (j + 1) + "测量点号：" + (ins3 + 1));
                                    ins3 = Comm.GetAnyValue(ins2, 6, 6);
                                    lsTmp.Add("第" + (j + 1) + "总加标志：" + (Comm.GetAnyValue(ins2, 6, 6) == 0 ? "正向" : "反向") + "," + (Comm.GetAnyValue(ins2, 7, 7) == 0 ? "加运算" : "减运算"));
                                }
                            }
                        }
                        else if (Afn == "0A")
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("本次总加组配置数量n：" + ins);
                            for (int i = 0; i < ins; i++)
                            {
                                lsTmp.Add("总加组序号：" + GetTheData("ExplanBinFun", 1));
                                //i1 = Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 0, 1);
                                //lsTmp.Add("总加组的测量点数量m" + (i + 1) + "：" + i1);
                                //for (int j = 0; j < i1; j++)
                                //{
                                //    ins2 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                                //    ins3 = Comm.GetAnyValue(ins2, 0, 5);
                                //    lsTmp.Add("第" + (j + 1) + "测量点号：" + (ins3 + 1));
                                //    ins3 = Comm.GetAnyValue(ins2, 6, 6);
                                //    lsTmp.Add("第" + (j + 1) + "总加标志：" + (Comm.GetAnyValue(ins2, 6, 6) == 0 ? "正向" : "反向") + "," + (Comm.GetAnyValue(ins2, 7, 7) == 0 ? "加运算" : "减运算"));
                                //}
                            }
                        }

                        break;
                    case 15:
                        lsTmp.Add("Fn = " + Fn + ",有功总电能量差动越限事件参数设置：");
                        if (BoolUp || Afn == "04")
                        {
                            ins = int.Parse(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("本次有功总电能量差动组配置数量n：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                lsTmp.Add("有功总电能量差动组序号：" + GetTheData("ExplanBinFun", 1));
                                lsTmp.Add("对比的总加组序号：" + GetTheData("ExplanBinFun", 1));
                                lsTmp.Add("参照的总加组序号：" + GetTheData("ExplanBinFun", 1));
                                lsTmp.Add("参与差动的电能量的时间区间及对比方法标志：" + GetTheData("ExplanBinFun", 1));
                                lsTmp.Add("差动越限相对偏差值：" + GetTheData("ExplanBinFun", 1));
                                lsTmp.Add("差动越限绝对偏差值：" + GetTheData("gd3", 4));
                            }
                        }
                        else
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("本次有功总电能量差动组配置数量n：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                lsTmp.Add("本次查询的第" + i + "个对象序号：" + GetTheData("ExplanBinFun", 1));
                            }
                        }
                        break;
                    case 17:
                        GetOneData("终端保安定值", new string[]{
                            "保安定值","gd2","2"});
                        break;
                    case 18:
                        lsTmp.Add("Fn = " + Fn + ",终端功控时段：");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("终端功控时段设置(取值0～3依次表示不控制、控制1、控制2、保留)");
                            for (int i = 0; i < 12; i++)
                            {
                                ins = int.Parse(GetTheData("ExplanBinFun", 1));
                                lsTmp.Add((i * 2).ToString() + ":00～" + (i * 2).ToString() + ":30时段：" + Comm.GetAnyValue(ins, 0, 1));
                                lsTmp.Add((i * 2).ToString() + ":30～" + (i * 2 + 1).ToString() + ":00时段：" + Comm.GetAnyValue(ins, 2, 3));
                                lsTmp.Add((i * 2 + 1).ToString() + ":30～" + (i * 2 + 1).ToString() + ":00时段：" + Comm.GetAnyValue(ins, 4, 5));
                                lsTmp.Add((i * 2).ToString() + ":30～" + (i * 2 + 2).ToString() + ":00时段：" + Comm.GetAnyValue(ins, 6, 7));
                            }
                        }
                        break;
                    case 19:
                        GetOneData("终端时段功控定值浮动系数", new string[]{
                            "时段功控定值浮动系数","gd4","1"});
                        break;
                    case 20:
                        GetOneData("终端月电能量控定值浮动系数", new string[]{
                            "月电能量控定值浮动系数","gd4","1"});
                        break;
                    case 21:
                        lsTmp.Add("Fn = " + Fn + ",终端电能量费率时段和费率数：");
                        if (BoolUp || Afn == "04")
                        {
                            for (int i = 0; i < 24; i++)
                            {
                                lsTmp.Add(i.ToString() + ":00～" + i.ToString() + ":30时段费率号：" + (Convert.ToInt16(GetTheData("ExplanBinFun", 1)) + 1));
                                lsTmp.Add(i.ToString() + ":30～" + (i + 1).ToString() + ":00时段费率号：" + (Convert.ToInt16(GetTheData("ExplanBinFun", 1)) + 1));
                            }
                            lsTmp.Add("费率数：" + GetTheData("ExplanBinFun", 1));
                        }
                        break;
                    case 22:
                        lsTmp.Add("Fn = " + Fn + ",终端电能量费率：");
                        if (BoolUp || Afn == "04")
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("费率数M：" + ins);
                            for (int i = 1; i <= ins; i++)

                                lsTmp.Add("第" + i + "费率：" + GetTheData("gd3", 4));
                        }
                        break;
                    case 23:
                        lsTmp.Add("Fn = " + Fn + ",终端催费告警参数：");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("催费告警允许∕禁止标志位：" + GetTheData("ExplanNo", 3));
                        }
                        break;
                    case 25:
                        lsTmp.Add("Fn = " + Fn + ",测量点基本参数：");
                        if (BoolUp || Afn == "04")
                        {
                            GetOneData(new string[]{
                            "电压互感器倍率","ExplanBinFun","2",
                            "电流互感器倍率","ExplanBinFun","2",
                            "额定电压","gd7","2",
                            "额定电流","gd22","1",
                            "额定负荷","gd23","3"});
                            ins = int.Parse(GetTheData("ExplanNo", 1));
                            switch (Comm.GetAnyValue(ins, 0, 1))
                            {
                                case 1:
                                    s1 = "三相三线"; break;
                                case 2:
                                    s1 = "三相四线"; break;
                                case 3:
                                    s1 = "单相表"; break;
                            }
                            switch (Comm.GetAnyValue(ins, 2, 3))
                            {
                                case 0:
                                    s1 += ",不确定"; break;
                                case 1:
                                    s1 += ",A相"; break;
                                case 2:
                                    s1 += ",B相"; break;
                                case 3:
                                    s1 += ",C相"; break;
                            }

                            lsTmp.Add("电源接线方式：" + s1);
                        }
                        break;
                    case 26:
                        GetOneData("测量点限值参数", new string[]{
                            "电压合格上限","gd7","2",
                            "电压合格下限","gd7","2",
                            "电压断相门限","gd7","2",
                            "电压上上限（过压门限）","gd7","2",
                            "越限持续时间","ExplanBinFun","1",
                            "越限恢复系数","gd5","2",
                            "电压下下限（欠压门限）","gd7","2",
                            "越限持续时间","ExplanBinFun","1",
                            "越限恢复系数","gd5","2",
                            "相电流上上限（过流门限）","gd25","3",
                            "越限持续时间","ExplanBinFun","1",
                            "越限恢复系数","gd5","2",
                            "相电流上限（额定电流门限）","gd25","3",
                            "越限持续时间","ExplanBinFun","1",
                            "越限恢复系数","gd5","2",
                            "零序电流上限","gd25","3",
                            "越限持续时间","ExplanBinFun","1",
                            "越限恢复系数","gd5","2",
                            "视在功率上上限","gd23","3",
                            "越限持续时间","ExplanBinFun","1",
                            "越限恢复系数","gd5","2",
                            "视在功率上限","gd23","3",
                            "越限持续时间","ExplanBinFun","1",
                            "越限恢复系数","gd5","2",
                            "三相电压不平衡限值","gd5","2",
                            "越限持续时间","ExplanBinFun","1",
                            "越限恢复系数","gd5","2",
                            "三相电流不平衡限值","gd5","2",
                            "越限持续时间","ExplanBinFun","1",
                            "越限恢复系数","gd5","2",
                            "连续失压时间限值","ExplanBinFun","1"});
                        break;
                    case 27:
                        lsTmp.Add("Fn = " + Fn + ",测量点铜损、铁损参数：");
                        break;
                    case 28:
                        GetOneData("测量点功率因数分段限值", new string[]{
                            "功率因数分段限值1","gd5","2",
                            "功率因数分段限值2","gd5","2" });
                        break;
                    case 29:
                        GetOneData("终端当地电能表显示号", new string[]{
                            "终端当地电能表显示号","ReAscill","12"});
                        break;
                    case 30:
                        lsTmp.Add("Fn = " + Fn + ",终端台区集中抄表停抄/投抄设置：");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("终端台区集中抄表停抄/投抄设置：" + GetTheData("ExplanBinFun", 1) == "1" ? "停抄" : "投抄");
                        }
                        break;
                    case 33:
                        lsTmp.Add("Fn = " + Fn + ",终端抄表运行参数设置：");
                        if (BoolUp || Afn == "04")
                        {
                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("本次设置的参数块个数n：" + i1);
                            for (int j = 0; j < i1; j++)
                            {
                                lsTmp.Add("终端通信端口号：" + GetTheData("ExplanBinFun", 1));
                                s1 = GetTheData("ExplanWei", 2);
                                s2 = s1.Substring(15, 1) == "1" ? "不允许自动抄表" : "要求终端根据抄表时段自动抄表" + ",";
                                s2 += s1.Substring(14, 1) == "1" ? "要求终端只抄重点表" : "要求终端抄所有表" + ",";
                                s2 += s1.Substring(13, 1) == "1" ? "要求终端采用广播冻结抄表" : "不要求终端采用广播冻结抄表" + ",";
                                s2 += s1.Substring(12, 1) == "1" ? "要求终端定时对电表广播校时" : "不要求终端定时对电表广播校时" + ",";
                                s2 += s1.Substring(11, 1) == "1" ? "要求终端搜寻新增或更换的电表" : "不要求终端搜寻新增或更换的电表" + ",";
                                s2 += s1.Substring(10, 1) == "1" ? "要求终端抄读电表状态字" : "不要求终端抄读电表状态字";

                                lsTmp.Add("台区集中抄表运行控制字：" + s2);
                                s1 = GetTheData("ExplanWei", 4);
                                s2 = "";
                                for (int i = 1; i <= 31; i++)
                                {
                                    if (s1.Substring(32 - i, 1) == "1")
                                        s2 += i.ToString() + ",";
                                }
                                lsTmp.Add("抄表日-日期：" + s2);
                                lsTmp.Add("抄表日-时间：" + GetTheData("gd19", 2));
                                lsTmp.Add("抄表间隔时间：" + GetTheData("ExplanBinFun", 1));

                                lsTmp.Add("对电表广播校时定时时间：" + GetTheData("gd18", 3));
                                ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                                lsTmp.Add("允许抄表时段数m（0≤m≤24）：" + ins);
                                for (int i = 1; i <= ins; i++)
                                {
                                    lsTmp.Add("第" + i + "个允许抄表时段开始时间：" + GetTheData("gd19", 2));
                                    lsTmp.Add("第" + i + "个允许抄表时段结束时间：" + GetTheData("gd19", 2));
                                }
                            }
                        }

                        break;
                    case 35:
                        lsTmp.Add("Fn = " + Fn + ",台区集中抄表重点户设置：");
                        if (BoolUp || Afn == "04")
                        {
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("台区集中抄表重点户个数n：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                lsTmp.Add("第" + i + "个重点户的电能表/交流采样装置序号：" + GetTheData("ExplanBinFun", 2));
                            }
                        }
                        break;
                    case 36:
                        GetOneData("终端上行通信流量门限设置", new string[]{
                            "月通信流量门限","ExplanBinFun","4"});
                        break;
                    case 38:
                        lsTmp.Add("Fn = " + Fn + ",1类数据配置设置 （在终端支持的1类数据配置内）：");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("本次设置所对应的用户大类号：" + GetTheData("ExplanBinFun", 1));
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("本次设置的组数m：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                lsTmp.Add("用户小类号：" + GetTheData("ExplanBinFun", 1));
                                ins2 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                                lsTmp.Add("信息类组数n：" + ins2);
                                for (int j = 1; j <= ins2; j++)
                                {
                                    lsTmp.Add("第" + j + "组信息类组所对应的信息类元标志位：" + GetTheData("ExplanBinFun", 1));
                                }
                            }
                        }
                        else
                        {
                            lsTmp.Add("本次查询的用户大类号：" + GetTheData("ExplanBinFun", 1));
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("本次查询数量n：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                lsTmp.Add("本次查询的第" + i + "个用户小类号：" + GetTheData("ExplanBinFun", 1));
                            }
                        }
                        break;
                    case 39:
                        lsTmp.Add("Fn = " + Fn + ",2类数据配置设置（在终端支持的2类数据配置内）：");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("本次设置所对应的用户大类号：" + GetTheData("ExplanBinFun", 1));
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("本次设置的组数m：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                lsTmp.Add("用户小类号：" + GetTheData("ExplanBinFun", 1));
                                ins2 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                                lsTmp.Add("信息类组数n：" + ins2);
                                for (int j = 1; j <= ins2; j++)
                                {
                                    lsTmp.Add("第" + j + "组信息类组所对应的信息类元标志位：" + GetTheData("ExplanBinFun", 1));
                                }
                            }
                        }
                        else
                        {
                            lsTmp.Add("本次查询的用户大类号：" + GetTheData("ExplanBinFun", 1));
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("本次查询数量n：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                lsTmp.Add("本次查询的第" + i + "个用户小类号：" + GetTheData("ExplanBinFun", 1));
                            }
                        }
                        break;
                    case 41:
                        lsTmp.Add("Fn = " + Fn + ",时段功控定值：");
                        if (BoolUp || Afn == "04")
                        {
                            int izjz = Comm.ExplanByteWeiValue(strFrameRemove.Substring(0, 2));
                            lsTmp.Add("方案标志：" + GetTheData("ExplanWei", 1));
                            for (int i = 1; i <= izjz; i++)
                            {
                                lsTmp.Add("第" + i + "套定值");
                                ins2 = Comm.ExplanByteWeiValue(strFrameRemove.Substring(0, 2));
                                lsTmp.Add("时段号：" + GetTheData("ExplanWei", 1));
                                for (int j = 1; j <= ins2; j++)
                                {
                                    lsTmp.Add("时段" + j + "功控定值(kW)：" + GetTheData("gd2", 2));
                                }
                            }
                        }
                        break;
                    case 42:
                        GetOneData("厂休功控参数", new string[]{
                            "厂休控定值","gd2","2",
                            "限电起始时间","gd19","2",
                            "限电延续时间（范围1~48）","ExplanBinFun","1",
                            "每周限电日:D1~D7表示星期一~星期日,D0=0","ExplanWei","1"});
                        break;
                    case 43:
                        GetOneData("厂休功控参数", new string[]{
                            "功率控制的功率计算滑差时间","ExplanBinFun","1"});
                        break;
                    case 44:
                        GetOneData("营业报停控参数", new string[]{
                            "报停起始时间","gd20","3",
                            "报停结束时间","gd19","3",
                            "报停控功率定值（范围1~48）","gd2","2"});
                        break;
                    case 45:
                        GetOneData("功控轮次设定", new string[]{
                            "功控轮次标志位","ExplanWei","1"});
                        break;
                    case 46:
                        GetOneData("月电量控定值", new string[]{
                            "月电量控定值","gd3","4",
                            "报警门限值系数","ExplanNo","1"});
                        break;
                    case 47:
                        lsTmp.Add("Fn = " + Fn + ",购电量（费）控参数：");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("购电单号：" + GetTheData("ExplanBinFun", 4));
                            lsTmp.Add("追加/刷新标志：" + (GetTheData("ExplanNo", 1) == "55" ? "追加" : "刷新"));
                            lsTmp.Add("购电量（费）值：" + GetTheData("gd3", 4));
                            lsTmp.Add("报警门限值：" + GetTheData("gd3", 4));
                            lsTmp.Add("跳闸门限值：" + GetTheData("gd3", 4));
                        }
                        break;
                    case 48:
                        GetOneData("电控轮次设定", new string[]{
                            "电控轮次标志位","ExplanWei","1"});
                        break;
                    case 49:
                        GetOneData("功控告警时间", new string[]{
                            "功控告警时间","ExplanBinFun","1"});
                        break;
                    case 57:
                        lsTmp.Add("Fn = " + Fn + ",终端声音告警允许∕禁止设置：");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("声音告警允许∕禁止标志位：" + GetTheData("ExplanNo", 3));
                        }
                        break;
                    case 58:
                        GetOneData("终端自动保电参数", new string[]{
                            "允许与主站连续无通信时间","ExplanBinFun","1"});
                        break;
                    case 59:
                        lsTmp.Add("Fn = " + Fn + ",电能表异常判别阈值设定：");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("电能量超差阈值：" + GetTheData("gd22", 1));
                            lsTmp.Add("电能表飞走阈值：" + GetTheData("gd22", 1));
                            lsTmp.Add("电能表停走阈值：" + Convert.ToInt16(GetTheData("ExplanBinFun", 1)) * 15 + "min");
                            lsTmp.Add("电能表校时阈值：" + GetTheData("ExplanBinFun", 2) + "s");
                        }
                        break;
                    case 65:
                        lsTmp.Add("Fn = " + Fn + ",定时上报1类数据任务设置：");
                        if (BoolUp || Afn == "04")
                        {
                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            switch (Comm.GetAnyValue(i1, 6, 7))
                            {
                                case 0:
                                    s1 += "分"; break;
                                case 1:
                                    s1 += "时"; break;
                                case 2:
                                    s1 += "日"; break;
                                case 3:
                                    s1 += "月"; break;
                            }

                            lsTmp.Add("定时上报周期单位：" + s1);
                            lsTmp.Add("定时上报周期：" + Comm.GetAnyValue(i1, 0, 5));
                            lsTmp.Add("上报基准时间:秒分时日月年：" + GetTheData("gd1", 6));
                            lsTmp.Add("曲线数据抽取倍率R：" + GetTheData("ExplanBinFun", 1));
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("数据单元标识个数n：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                lsTmp.Add("数据单元标识" + i + "：Pn:" + Comm.ExplanP(GetTheData("ExplanNo", 2)) + ",Fn:" + Comm.ExplanF(GetTheData("ExplanNo", 2)));
                            }
                        }
                        break;
                    case 66:
                        lsTmp.Add("Fn = " + Fn + ",定时上报2类数据任务设置：");
                        if (BoolUp || Afn == "04")
                        {
                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            switch (Comm.GetAnyValue(i1, 6, 7))
                            {
                                case 0:
                                    s1 += "分"; break;
                                case 1:
                                    s1 += "时"; break;
                                case 2:
                                    s1 += "日"; break;
                                case 3:
                                    s1 += "月"; break;
                            }

                            lsTmp.Add("定时上报周期单位：" + s1);
                            lsTmp.Add("定时上报周期：" + Comm.GetAnyValue(i1, 0, 5));
                            lsTmp.Add("上报基准时间:秒分时日月年：" + GetTheData("gd1", 6));
                            lsTmp.Add("曲线数据抽取倍率R：" + GetTheData("ExplanBinFun", 1));
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("数据单元标识个数n：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                lsTmp.Add("数据单元标识" + i + "：Pn:" + Comm.ExplanP(GetTheData("ExplanNo", 2)) + ",Fn:" + Comm.ExplanF(GetTheData("ExplanNo", 2)));
                            }
                        }
                        break;
                    case 67:
                        lsTmp.Add("Fn = " + Fn + ",定时上报1类数据任务启动/停止设置：");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("启动/停止标志：" + (GetTheData("ExplanNo", 1) == "55" ? "启动" : "停止"));
                        }
                        break;
                    case 68:
                        lsTmp.Add("Fn = " + Fn + ",定时上报2类数据任务启动/停止设置：");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("启动/停止标志：" + (GetTheData("ExplanNo", 1) == "55" ? "启动" : "停止"));
                        }
                        break;
                    case 89:
                        GetOneData("终端逻辑地址", new string[]{
                            "行政区码","RExplanNo","2",
                            "终端地址","RExplanNo",LenAddr.ToString()});
                        break;
                    case 91:
                        GetOneData("终端地理位置信息", new string[]{
                            "经度","gd28","5",
                            "纬度","gd28","5"});
                        break;
                    case 97:
                        lsTmp.Add("Fn = " + Fn + ",停电数据采集配置参数");
                        if (BoolUp || Afn == "04")
                        {
                            lsTmp.Add("停电数据采集标志：" + GetTheData("ExplanWei", 1));
                            lsTmp.Add("停电事件抄读时间间隔：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("停电事件抄读时间限值：" + GetTheData("ExplanBinFun", 1));
                            ins = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("需要读取停电事件电能表个数：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                lsTmp.Add("第" + i + "块需要抄读的电能表：" + GetTheData("ExplanBinFun", 2));
                            }
                        }
                        break;
                    case 98:
                        GetOneData("停电事件甄别限值参数", new string[]{
                            "停电时间最小有效间隔","ExplanBinFun","2",
                            "停电时间最大有效间隔","ExplanBinFun","2",
                            "停电事件起止时间偏差限值","ExplanBinFun","2",
                            "停电事件时间区段偏差限值","ExplanBinFun","2",
                            "终端停电发生电压限值","gd7","2",
                            "终端停电恢复电压限值","gd7","2",
                            "停电事件主动上报标志位","ExplanBinFun","1"});
                        break;
                    case 105:
                        lsTmp.Add("Fn = " + Fn + ",电能表数据分级归类参数：");
                        lsTmp.Add("数据分级类号：" + GetTheData("ExplanNo", 1));
                        break;
                    case 106:
                        lsTmp.Add("Fn = " + Fn + ",电能表数据分级参数：");
                        ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                        for (int i = 1; i <= ins; i++)
                        {
                            lsTmp.Add("第" + i + "个电能表数据标识：" + GetTheData("ExplanNo", 4));
                            lsTmp.Add("第" + i + "个电能表数据等级：" + GetTheData("ExplanBinFun", 1));
                        }
                        break;
                    case 107:
                        lsTmp.Add("Fn = " + Fn + ",电能表数据分级周期参数：");
                        lsTmp.Add("周期数值：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("周期单位(1为分钟,2为小时,3为天,4为月)：" + GetTheData("ExplanBinFun", 1));
                        break;
                    default:
                        lsTmp.Add("ERR"); break;
                }
                if (strFrameRemove.Length > 0)
                    a040A();
            }
            catch
            {

            }
        }
        public void a0B()
        {
            try
            {
                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    default:
                        lsTmp.Add("ERR"); break;
                }
                if (strFrameRemove.Length > 0)
                    a0B();
            }
            catch
            {

            }
        }



        public void a0C()
        {
            int ins1 = 0; int ins2 = 0;
            try
            {

                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    case 2:
                        GetOneData("终端日历时钟", new string[]{
                            "终端日历时钟","gd1","6"});
                        break;
                    case 3:
                        lsTmp.Add("Fn = " + Fn + ",终端参数状态：");
                        if (BoolUp)
                        {
                            lsTmp.Add("参数映射表：" + GetTheData("ExplanNo", 31));
                        }
                        break;
                    case 4:
                        lsTmp.Add("Fn = " + Fn + ",终端上行通信状态：");
                        if (BoolUp)
                        {
                            lsTmp.Add("终端上行通信状态：" + GetTheData("ExplanNo", 1));
                        }
                        break;
                    case 5://待解析
                        lsTmp.Add("Fn = " + Fn + ",终端控制设置状态：");
                        if (BoolUp)
                        {
                            lsTmp.Add("终端上行通信状态：" + GetTheData("ExplanWei", 1));
                            strFrameRemove = "";
                        }
                        break;
                    case 6:
                        lsTmp.Add("Fn = " + Fn + ",终端当前控制状态：");
                        if (BoolUp)
                        {
                            lsTmp.Add("遥控跳闸输出状态：" + GetTheData("ExplanWei", 1));
                            lsTmp.Add("当前催费告警状态：" + GetTheData("ExplanWei", 1));
                            int izjz = Comm.ExplanByteWeiValue(strFrameRemove.Substring(0, 2));
                            lsTmp.Add("总加组有效标志位：" + GetTheData("ExplanWei", 1));
                            for (int i = 1; i <= izjz; i++)
                            {
                                lsTmp.Add("当前功控定值：" + GetTheData("gd2", 2));
                                lsTmp.Add("当前功率下浮控浮动系数：" + GetTheData("gd4", 1));
                                lsTmp.Add("功控跳闸输出状态：" + GetTheData("ExplanWei", 1));
                                lsTmp.Add("月电控跳闸输出状态：" + GetTheData("ExplanWei", 1));
                                lsTmp.Add("购电控跳闸输出状态：" + GetTheData("ExplanWei", 1));
                                lsTmp.Add("功控越限告警状态：" + GetTheData("ExplanWei", 1));
                                lsTmp.Add("电控越限告警状态：" + GetTheData("ExplanWei", 1));
                            }
                        }
                        break;
                    case 7:
                        GetOneData("终端事件计数器当前值", new string[]{
                            "重要事件计数器EC1值","ExplanBinFun","1",
                            "普通事件计数器EC2值","ExplanBinFun","1"});
                        break;
                    case 8:
                        lsTmp.Add("Fn = " + Fn + ",终端事件标志状态：");
                        if (BoolUp)
                        {
                            lsTmp.Add("事件状态标志：" + GetTheData("ExplanNo", 8));
                        }
                        break;
                    case 9:
                        GetOneData("终端状态量及变位标志", new string[]{
                            "状态量的状态ST","ExplanWei","1",
                            "状态量的变位CD","ExplanWei","1"});
                        break;
                    case 10:
                        GetOneData("终端与主站当日、月通信流量", new string[]{
                            "终端与主站当日通信流量","ExplanBinFun","4",
                            "终端与主站当月通信流量","ExplanBinFun","4"});
                        break;
                    case 11:
                        lsTmp.Add("Fn = " + Fn + ",终端集中抄表状态信息：");
                        if (BoolUp)
                        {
                            int ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("本项数据块数：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                lsTmp.Add("终端通信端口号：" + GetTheData("ExplanBinFun", 1));
                                lsTmp.Add("要抄电表总数：" + GetTheData("ExplanBinFun", 2));
                                lsTmp.Add("当前抄表工作状态标志：" + GetTheData("ExplanWei", 1));
                                lsTmp.Add("抄表成功块数：" + GetTheData("ExplanBinFun", 2));
                                lsTmp.Add("抄重点表成功块数：" + GetTheData("ExplanBinFun", 1));
                                lsTmp.Add("抄表开始时间：" + GetTheData("gd1", 6));
                                lsTmp.Add("抄表结束时间：" + GetTheData("gd1", 6));
                            }
                        }
                        break;
                    case 13:
                        lsTmp.Add("Fn = " + Fn + ",搜索到的电表信息：");
                        if (BoolUp)
                        {
                            ins1 = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("搜到结果总数量：" + ins1);
                            ins2 = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("本帧包含结果数量：" + ins2);
                            for (int i = 1; i <= ins2; i++)
                            {
                                lsTmp.Add("电能表地址：" + GetTheData("ExplanNo", 6));
                                lsTmp.Add("通信协议类型：" + GetTheData("ExplanBinFun", 1));
                                lsTmp.Add("所属采集器通信地址：" + GetTheData("ExplanNo", 6));
                            }
                        }
                        break;
                    case 14:
                        lsTmp.Add("Fn = " + Fn + ",文件下装未收到数据段：");
                        if (BoolUp)
                        {
                            lsTmp.Add("组号：" + Convert.ToInt32(GetTheData("ExplanBinFun", 2)));
                            lsTmp.Add("组内各数据段未收到标志：" + GetTheData("ExplanNo", 128));
                        }
                        break;
                    case 17:
                        GetOneData("当前总加有功功率", new string[]{
                            "当前总加有功功率","gd2","2"});
                        break;
                    case 18:
                        GetOneData("当前总加无功功率", new string[]{
                            "当前总加无功功率","gd2","2"});
                        break;
                    case 19:
                        GetOneData("当日总加有功电能量", new string[]{
                            "费率数M","ExplanBinFun","1",
                            "当日总加有功总电能量","gd3","4",
                            "费率1当日总加有功电能量","gd3","4",
                            "费率2当日总加有功电能量","gd3","4",
                            "费率3当日总加有功电能量","gd3","4",
                            "费率4当日总加有功电能量","gd3","4"});
                        break;
                    case 20:
                        GetOneData("当日总加无功电能量", new string[]{
                            "费率数M","ExplanBinFun","1",
                            "当日总加无功总电能量","gd3","4",
                            "费率1当日总加无功电能量","gd3","4",
                            "费率2当日总加无功电能量","gd3","4",
                            "费率3当日总加无功电能量","gd3","4",
                            "费率4当日总加无功电能量","gd3","4"});
                        break;
                    case 21:
                        GetOneData("当月总加有功电能量", new string[]{
                            "费率数M","ExplanBinFun","1",
                            "当月总加有功总电能量","gd3","4",
                            "费率1当月总加有功电能量","gd3","4",
                            "费率2当月总加有功电能量","gd3","4",
                            "费率3当月总加有功电能量","gd3","4",
                            "费率4当月总加有功电能量","gd3","4"});
                        break;
                    case 22:
                        GetOneData("当月总加无功电能量", new string[]{
                            "费率数M","ExplanBinFun","1",
                            "当月总加有功总电能量","gd3","4",
                            "费率1当月总加无功电能量","gd3","4",
                            "费率2当月总加无功电能量","gd3","4",
                            "费率3当月总加无功电能量","gd3","4",
                            "费率4当月总加无功电能量","gd3","4"});
                        break;
                    case 23:
                        GetOneData("终端当前剩余电量(费)", new string[]{
                            "当前剩余电量(费)","gd3","4"});
                        break;
                    case 24:
                        GetOneData("当前功率下浮控控后总加有功功率冻结值", new string[]{
                            "控后总加有功功率冻结值","gd2","2"});
                        break;
                    case 25:
                        GetOneData("当前三相及总有/无功功率、功率因数,三相电压、电流、零序电流、视在功率", new string[]{
                            "终端抄表时间","gd15","5",
                            "当前总有功功率","gd9","3",
                            "当前A相有功功率","gd9","3",
                            "当前B相有功功率","gd9","3",
                            "当前C相有功功率","gd9","3",
                            "当前总无功功率","gd9","3",
                            "当前A相无功功率","gd9","3",
                            "当前B相无功功率","gd9","3",
                            "当前C相无功功率","gd9","3",
                            "当前总功率因数","gd5","2",
                            "当前A相功率因数","gd5","2",
                            "当前B相功率因数","gd5","2",
                            "当前C相功率因数","gd5","2",
                            "当前A相电压","gd7","2",
                            "当前B相电压","gd7","2",
                            "当前C相电压","gd7","2",
                            "当前A相电流","gd25","3",
                            "当前B相电流","gd25","3",
                            "当前C相电流","gd25","3",
                            "当前零序电流","gd25","3",
                            "当前总视在功率","gd9","3",
                            "当前A相视在功率","gd9","3",
                            "当前B相视在功率","gd9","3",
                            "当前C相视在功率","gd9","3"});
                        break;
                    case 26:
                        GetOneData("A、B、C三相断相统计数据及最近一次断相记录", new string[]{
                            "终端抄表时间","gd15","5",
                            "总断相次数","gd10","3",
                            "A相断相次数","gd10","3",
                            "B相断相次数","gd10","3",
                            "C相断相次数","gd10","3",
                            "断相时间累计值","gd10","3",
                            "A相断相时间累计值","gd10","3",
                            "B相断相时间累计值","gd10","3",
                            "C相断相时间累计值","gd10","3",
                            "最近一次断相起始时刻","gd17","4",
                            "A相最近断相起始时刻","gd17","4",
                            "B相最近断相起始时刻","gd17","4",
                            "C相最近断相起始时刻","gd17","4",
                            "最近一次断相结束时刻","gd17","4",
                            "A相最近断相结束时刻","gd17","4",
                            "B相最近断相结束时刻","gd17","4",
                            "C相最近断相结束时刻","gd17","4"});
                        break;
                    case 27:
                        GetOneData("电能表日历时钟、编程次数及其最近一次操作时间", new string[]{
                            "终端抄表时间","gd15","5",
                            "电能表日历时钟","gd1","6",
                            "电池工作时间","gd27","4",
                            "编程总次数","gd10","3",
                            "最近一次编程发生时刻","gd1","6",
                            "电表清零总次数","gd10","3",
                            "最近一次清零发生时刻","gd1","6",
                            "需量清零总次数","gd10","3",
                            "最近一次清零发生时刻","gd1","6",
                            "事件清零总次数","gd10","3",
                            "最近一次清零发生时刻","gd1","6",
                            "校时总次数","gd10","3",
                            "最近一次校时发生时刻","gd1","6"
                            });
                        break;
                    case 28:
                        GetOneData("电表运行状态字及其变位标志", new string[]{
                            "终端抄表时间","gd15","5",
                            "电表运行状态字变位标志1","ExplanNo","2",
                            "电表运行状态字变位标志2","ExplanNo","2",
                            "电表运行状态字变位标志3","ExplanNo","2",
                            "电表运行状态字变位标志4","ExplanNo","2",
                            "电表运行状态字变位标志5","ExplanNo","2",
                            "电表运行状态字变位标志6","ExplanNo","2",
                            "电表运行状态字变位标志7","ExplanNo","2",
                            "电表运行状态字1","ExplanNo","2",
                            "电表运行状态字2","ExplanNo","2",
                            "电表运行状态字3","ExplanNo","2",
                            "电表运行状态字4","ExplanNo","2",
                            "电表运行状态字5","ExplanNo","2",
                            "电表运行状态字6","ExplanNo","2",
                            "电表运行状态字7","ExplanNo","2"
                            });
                        break;
                    case 29:
                        GetOneData("当前铜损、铁损有功总电能示值", new string[]{
                            "当前铜损有功总电能示值","gd14","5",
                            "当前铁损有功总电能示值","gd14","5"
                            });
                        break;
                    case 30:
                        GetOneData("上一结算铜损、铁损有功总电能示值", new string[]{
                            "上一结算铜损有功总电能示值","gd14","5",
                            "上一结算铁损有功总电能示值","gd14","5"
                            });
                        break;
                    case 31:
                        GetOneData("当前A、B、C三相正/反向有功电能示值、组合无功1/2电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "当前A相正向有功电能示值","gd14","5",
                            "当前A相反向有功电能示值","gd14","5",
                            "当前A相组合无功1电能示值","gd11","4",
                            "当前A相组合无功2电能示值","gd11","4",
                            "当前B相正向有功电能示值","gd14","5",
                            "当前B相反向有功电能示值","gd14","5",
                            "当前B相组合无功1电能示值","gd11","4",
                            "当前B相组合无功2电能示值","gd11","4",
                            "当前C相正向有功电能示值","gd14","5",
                            "当前C相反向有功电能示值","gd14","5",
                            "当前C相组合无功1电能示值","gd11","4",
                            "当前C相组合无功2电能示值","gd11","4"
                            });
                        break;
                    case 32:
                        GetOneData("上一结算日A、B、C三相正/反向有功电能示值、组合无功1/2电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "上一结算日A相正向有功电能示值","gd14","5",
                            "上一结算日A相反向有功电能示值","gd14","5",
                            "上一结算日A相组合无功1电能示值","gd11","4",
                            "上一结算日A相组合无功2电能示值","gd11","4",
                            "上一结算日B相正向有功电能示值","gd14","5",
                            "上一结算日B相反向有功电能示值","gd14","5",
                            "上一结算日B相组合无功1电能示值","gd11","4",
                            "上一结算日B相组合无功2电能示值","gd11","4",
                            "上一结算日C相正向有功电能示值","gd14","5",
                            "上一结算日C相反向有功电能示值","gd14","5",
                            "上一结算日C相组合无功1电能示值","gd11","4",
                            "上一结算日C相组合无功2电能示值","gd11","4"
                            });
                        break;
                    case 33:
                        GetOneData("当前正向有/无功电能示值、一/四象限无功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "当前正向有功总电能示值","gd14","5",
                            "当前费率1正向有功总电能示值","gd14","5",
                            "当前费率2正向有功总电能示值","gd14","5",
                            "当前费率3正向有功总电能示值","gd14","5",
                            "当前费率4正向有功总电能示值","gd14","5",
                            "当前正向无功(组合无功1)总电能示值","gd11","4",
                            "当前费率1正向无功(组合无功1)总电能示值","gd11","4",
                            "当前费率2正向无功(组合无功1)总电能示值","gd11","4",
                            "当前费率3正向无功(组合无功1)总电能示值","gd11","4",
                            "当前费率4正向无功(组合无功1)总电能示值","gd11","4",
                            "当前一象限无功总电能示值","gd11","4",
                            "当前一象限费率1无功电能示值","gd11","4",
                            "当前一象限费率2无功电能示值","gd11","4",
                            "当前一象限费率3无功电能示值","gd11","4",
                            "当前一象限费率4无功电能示值","gd11","4",
                            "当前四象限无功总电能示值","gd11","4",
                            "当前四象限费率1无功电能示值","gd11","4",
                            "当前四象限费率2无功电能示值","gd11","4",
                            "当前四象限费率3无功电能示值","gd11","4",
                            "当前四象限费率4无功电能示值","gd11","4"
                            });
                        break;
                    case 34:
                        GetOneData("当前反向有/无功电能示值、二/三象限无功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "当前反向有功总电能示值","gd14","5",
                            "当前费率1反向有功总电能示值","gd14","5",
                            "当前费率2反向有功总电能示值","gd14","5",
                            "当前费率3反向有功总电能示值","gd14","5",
                            "当前费率4反向有功总电能示值","gd14","5",
                            "当前反向无功(组合无功2)总电能示值","gd11","4",
                            "当前费率1反向无功(组合无功2)总电能示值","gd11","4",
                            "当前费率2反向无功(组合无功2)总电能示值","gd11","4",
                            "当前费率3反向无功(组合无功2)总电能示值","gd11","4",
                            "当前费率4反向无功(组合无功2)总电能示值","gd11","4",
                            "当前二象限无功总电能示值","gd11","4",
                            "当前二象限费率1无功电能示值","gd11","4",
                            "当前二象限费率2无功电能示值","gd11","4",
                            "当前二象限费率3无功电能示值","gd11","4",
                            "当前二象限费率4无功电能示值","gd11","4",
                            "当前三象限无功总电能示值","gd11","4",
                            "当前三象限费率1无功电能示值","gd11","4",
                            "当前三象限费率2无功电能示值","gd11","4",
                            "当前三象限费率3无功电能示值","gd11","4",
                            "当前三象限费率4无功电能示值","gd11","4"
                            });
                        break;
                    case 35:
                        GetOneData("当月正向有/无功最大需量及发生时间", new string[] {
                            "终端抄表时间","gd15", "5",
                            "费率数M)", "ExplanBinFun","1",
                            "当月反向有功总最大需量", "gd23","3",
                            "当月反向有功费率1最大需量", "gd23","3",
                            "当月反向有功费率2最大需量", "gd23","3",
                            "当月反向有功费率3最大需量","gd23","3",
                            "当月反向有功费率4最大需量","gd23","3",
                            "当月反向有功总最大需量发生时间","gd17","4",
                            "当月反向有功费率1最大需量发生时间","gd17","4",
                            "当月反向有功费率2最大需量发生时间", "gd17","4",
                            "当月反向有功费率3最大需量发生时间", "gd17","4",
                            "当月反向有功费率4最大需量发生时间", "gd17","4",
                            "当月反向无功总最大需量","gd23","3",
                            "当月反向无功费率1最大需量", "gd23","3",
                            "当月反向无功费率2最大需量", "gd23","3",
                            "当月反向无功费率3最大需量", "gd23","3",
                            "当月反向无功费率4最大需量", "gd23","3",
                            "当月反向无功总最大需量发生时间", "gd17","4",
                            "当月反向无功费率1最大需量发生时间", "gd17","4",
                            "当月反向无功费率2最大需量发生时间", "gd17","4",
                            "当月反向无功费率3最大需量发生时间", "gd17","4",
                            "当月反向无功费率4最大需量发生时间","gd17","4" });
                        break;
                    case 36:
                        GetOneData("当月反向有/无功最大需量及发生时间", new string[] {
                            "终端抄表时间","gd15", "5",
                            "费率数M)", "ExplanBinFun","1",
                            "当月反向有功总最大需量", "gd23","3",
                            "当月反向有功费率1最大需量", "gd23","3",
                            "当月反向有功费率2最大需量", "gd23","3",
                            "当月反向有功费率3最大需量","gd23","3",
                            "当月反向有功费率4最大需量","gd23","3",
                            "当月反向有功总最大需量发生时间","gd17","4",
                            "当月反向有功费率1最大需量发生时间","gd17","4",
                            "当月反向有功费率2最大需量发生时间", "gd17","4",
                            "当月反向有功费率3最大需量发生时间", "gd17","4",
                            "当月反向有功费率4最大需量发生时间", "gd17","4",
                            "当月反向无功总最大需量","gd23","3",
                            "当月反向无功费率1最大需量", "gd23","3",
                            "当月反向无功费率2最大需量", "gd23","3",
                            "当月反向无功费率3最大需量", "gd23","3",
                            "当月反向无功费率4最大需量", "gd23","3",
                            "当月反向无功总最大需量发生时间", "gd17","4",
                            "当月反向无功费率1最大需量发生时间", "gd17","4",
                            "当月反向无功费率2最大需量发生时间", "gd17","4",
                            "当月反向无功费率3最大需量发生时间", "gd17","4",
                            "当月反向无功费率4最大需量发生时间","gd17","4" });
                        break;
                    case 37:
                        GetOneData("上月正向有/无功电能示值、一/四象限无功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "上月正向有功总电能示值","gd14","5",
                            "上月费率1正向有功总电能示值","gd14","5",
                            "上月费率2正向有功总电能示值","gd14","5",
                            "上月费率3正向有功总电能示值","gd14","5",
                            "上月费率4正向有功总电能示值","gd14","5",
                            "上月正向无功(组合无功1)总电能示值","gd11","4",
                            "上月费率1正向无功(组合无功1)总电能示值","gd11","4",
                            "上月费率2正向无功(组合无功1)总电能示值","gd11","4",
                            "上月费率3正向无功(组合无功1)总电能示值","gd11","4",
                            "上月费率4正向无功(组合无功1)总电能示值","gd11","4",
                            "上月一象限无功总电能示值","gd11","4",
                            "上月一象限费率1无功电能示值","gd11","4",
                            "上月一象限费率2无功电能示值","gd11","4",
                            "上月一象限费率3无功电能示值","gd11","4",
                            "上月一象限费率4无功电能示值","gd11","4",
                            "上月四象限无功总电能示值","gd11","4",
                            "上月四象限费率1无功电能示值","gd11","4",
                            "上月四象限费率2无功电能示值","gd11","4",
                            "上月四象限费率3无功电能示值","gd11","4",
                            "上月四象限费率4无功电能示值","gd11","4"
                            });
                        break;
                    case 38:
                        GetOneData("上月反向有/无功电能示值、二/三象限无功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "上月正向有功总电能示值","gd14","5",
                            "上月费率1反向有功总电能示值","gd14","5",
                            "上月费率2反向有功总电能示值","gd14","5",
                            "上月费率3反向有功总电能示值","gd14","5",
                            "上月费率4反向有功总电能示值","gd14","5",
                            "上月反向无功(组合无功2)总电能示值","gd11","4",
                            "上月费率1反向无功(组合无功2)总电能示值","gd11","4",
                            "上月费率2反向无功(组合无功2)总电能示值","gd11","4",
                            "上月费率3反向无功(组合无功2)总电能示值","gd11","4",
                            "上月费率4反向无功(组合无功2)总电能示值","gd11","4",
                            "上月二象限无功总电能示值","gd11","4",
                            "上月二象限费率1无功电能示值","gd11","4",
                            "上月二象限费率2无功电能示值","gd11","4",
                            "上月二象限费率3无功电能示值","gd11","4",
                            "上月二象限费率4无功电能示值","gd11","4",
                            "上月三象限无功总电能示值","gd11","4",
                            "上月三象限费率1无功电能示值","gd11","4",
                            "上月三象限费率2无功电能示值","gd11","4",
                            "上月三象限费率3无功电能示值","gd11","4",
                            "上月三象限费率4无功电能示值","gd11","4"
                            });
                        break;
                    case 39:
                        GetOneData("上月正向有/无功最大需量及发生时间", new string[] {
                            "终端抄表时间","gd15", "5",
                            "费率数M)", "ExplanBinFun","1",
                            "上月反向有功总最大需量", "gd23","3",
                            "上月反向有功费率1最大需量", "gd23","3",
                            "上月反向有功费率2最大需量", "gd23","3",
                            "上月反向有功费率3最大需量","gd23","3",
                            "上月反向有功费率4最大需量","gd23","3",
                            "上月反向有功总最大需量发生时间","gd17","4",
                            "上月反向有功费率1最大需量发生时间","gd17","4",
                            "上月反向有功费率2最大需量发生时间", "gd17","4",
                            "上月反向有功费率3最大需量发生时间", "gd17","4",
                            "上月反向有功费率4最大需量发生时间", "gd17","4",
                            "上月反向无功总最大需量","gd23","3",
                            "上月反向无功费率1最大需量", "gd23","3",
                            "上月反向无功费率2最大需量", "gd23","3",
                            "上月反向无功费率3最大需量", "gd23","3",
                            "上月反向无功费率4最大需量", "gd23","3",
                            "上月反向无功总最大需量发生时间", "gd17","4",
                            "上月反向无功费率1最大需量发生时间", "gd17","4",
                            "上月反向无功费率2最大需量发生时间", "gd17","4",
                            "上月反向无功费率3最大需量发生时间", "gd17","4",
                            "上月反向无功费率4最大需量发生时间","gd17","4" });
                        break;
                    case 40:
                        GetOneData("上月反向有/无功最大需量及发生时间", new string[] {
                            "终端抄表时间","gd15", "5",
                            "费率数M)", "ExplanBinFun","1",
                            "上月反向有功总最大需量", "gd23","3",
                            "上月反向有功费率1最大需量", "gd23","3",
                            "上月反向有功费率2最大需量", "gd23","3",
                            "上月反向有功费率3最大需量","gd23","3",
                            "上月反向有功费率4最大需量","gd23","3",
                            "上月反向有功总最大需量发生时间","gd17","4",
                            "上月反向有功费率1最大需量发生时间","gd17","4",
                            "上月反向有功费率2最大需量发生时间", "gd17","4",
                            "上月反向有功费率3最大需量发生时间", "gd17","4",
                            "上月反向有功费率4最大需量发生时间", "gd17","4",
                            "上月反向无功总最大需量","gd23","3",
                            "上月反向无功费率1最大需量", "gd23","3",
                            "上月反向无功费率2最大需量", "gd23","3",
                            "上月反向无功费率3最大需量", "gd23","3",
                            "上月反向无功费率4最大需量", "gd23","3",
                            "上月反向无功总最大需量发生时间", "gd17","4",
                            "上月反向无功费率1最大需量发生时间", "gd17","4",
                            "上月反向无功费率2最大需量发生时间", "gd17","4",
                            "上月反向无功费率3最大需量发生时间", "gd17","4",
                            "上月反向无功费率4最大需量发生时间","gd17","4" });
                        break;
                    case 41:
                        GetOneData("当日正向有功电能量", new string[] {
                            "费率数M)","ExplanBinFun", "1",
                            "当日正向有功总电能量", "gd13","4",
                            "当日费率1正向有功电能量", "gd13","4",
                            "当日费率2正向有功电能量", "gd13","4",
                            "当日费率3正向有功电能量", "gd13","4",
                            "当日费率4正向有功电能量", "gd13","4"});
                        break;
                    case 42:
                        GetOneData("当日正向无功电能量", new string[] {
                            "费率数M)","ExplanBinFun", "1",
                            "当日正向无功总电能量", "gd13","4",
                            "当日费率1正向无功电能量", "gd13","4",
                            "当日费率2正向无功电能量", "gd13","4",
                            "当日费率3正向无功电能量", "gd13","4",
                            "当日费率4正向无功电能量", "gd13","4"});
                        break;
                    case 43:
                        GetOneData("当日反向有功电能量", new string[] {
                            "费率数M)","ExplanBinFun", "1",
                            "当日反向有功总电能量", "gd13","4",
                            "当日费率1反向有功电能量", "gd13","4",
                            "当日费率2反向有功电能量", "gd13","4",
                            "当日费率3反向有功电能量", "gd13","4",
                            "当日费率4反向有功电能量", "gd13","4"});
                        break;
                    case 44:
                        GetOneData("当日反向无功电能量", new string[] {
                            "费率数M)","ExplanBinFun", "1",
                            "当日反向无功总电能量", "gd13","4",
                            "当日费率1反向无功电能量", "gd13","4",
                            "当日费率2反向无功电能量", "gd13","4",
                            "当日费率3反向无功电能量", "gd13","4",
                            "当日费率4反向无功电能量", "gd13","4"});
                        break;
                    case 45:
                        GetOneData("当月正向有功电能量", new string[] {
                            "费率数M)","ExplanBinFun", "1",
                            "当月正向有功总电能量", "gd13","4",
                            "当月费率1正向有功电能量", "gd13","4",
                            "当月费率2正向有功电能量", "gd13","4",
                            "当月费率3正向有功电能量", "gd13","4",
                            "当月费率4正向有功电能量", "gd13","4"});
                        break;
                    case 46:
                        GetOneData("当月正向无功电能量", new string[] {
                            "费率数M)","ExplanBinFun", "1",
                            "当月正向无功总电能量", "gd13","4",
                            "当月费率1正向无功电能量", "gd13","4",
                            "当月费率2正向无功电能量", "gd13","4",
                            "当月费率3正向无功电能量", "gd13","4",
                            "当月费率4正向无功电能量", "gd13","4"});
                        break;
                    case 47:
                        GetOneData("当月反向有功电能量", new string[] {
                            "费率数M)","ExplanBinFun", "1",
                            "当月反向有功总电能量", "gd13","4",
                            "当月费率1反向有功电能量", "gd13","4",
                            "当月费率2反向有功电能量", "gd13","4",
                            "当月费率3反向有功电能量", "gd13","4",
                            "当月费率4反向有功电能量", "gd13","4"});
                        break;
                    case 48:
                        GetOneData("当月反向无功电能量", new string[] {
                            "费率数M)","ExplanBinFun", "1",
                            "当月反向无功总电能量", "gd13","4",
                            "当月费率1反向无功电能量", "gd13","4",
                            "当月费率2反向无功电能量", "gd13","4",
                            "当月费率3反向无功电能量", "gd13","4",
                            "当月费率4反向无功电能量", "gd13","4"});
                        break;
                    case 49:
                        GetOneData("上月电压、电流相位角", new string[] {
                            "Uab/Ua相位角","gd5", "2",
                            "Ub相位角","gd5", "2",
                            "Ucb/Uc相位角","gd5", "2",
                            "Ia相位角","gd5", "2",
                            "Ib相位角","gd5", "2",
                            "Ic相位角","gd5", "2"
                        });
                        break;
                    case 50:
                        GetOneData("当前剩余电量、剩余金额", new string[] {
                            "剩余电量","gd11", "4",
                            "剩余金额","gd11", "4"
                        });
                        break;
                    case 57:
                        lsTmp.Add("Fn = " + Fn + ",当前A、B、C三相电压电流2~N次谐波有效值：");
                        if (BoolUp)
                        {
                            ins1 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            for (int i = 2; i <= ins1; i++)
                            {
                                lsTmp.Add("A相" + i + "次谐波电压：" + GetTheData("gd7", 2));
                            }
                            for (int i = 2; i <= ins1; i++)
                            {
                                lsTmp.Add("B相" + i + "次谐波电压：" + GetTheData("gd7", 2));
                            }
                            for (int i = 2; i <= ins1; i++)
                            {
                                lsTmp.Add("C相" + i + "次谐波电压：" + GetTheData("gd7", 2));
                            }
                            for (int i = 2; i <= ins1; i++)
                            {
                                lsTmp.Add("A相" + i + "次谐波电流：" + GetTheData("gd6", 2));
                            }
                            for (int i = 2; i <= ins1; i++)
                            {
                                lsTmp.Add("B相" + i + "次谐波电流：" + GetTheData("gd6", 2));
                            }
                            for (int i = 2; i <= ins1; i++)
                            {
                                lsTmp.Add("C相" + i + "次谐波电流：" + GetTheData("gd6", 2));
                            }
                        }
                        break;
                    case 58:
                        lsTmp.Add("Fn = " + Fn + ",当前A、B、C三相电压电流2~N次谐波含有率：");
                        if (BoolUp)
                        {
                            ins1 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("A相总谐波电压含有率：" + GetTheData("gd5", 2));
                            for (int i = 2; i <= ins1; i++)
                            {
                                lsTmp.Add("A相" + i + "次谐波电压含有率：" + GetTheData("gd5", 2));
                            }
                            lsTmp.Add("B相总谐波电压含有率：" + GetTheData("gd5", 2));
                            for (int i = 2; i <= ins1; i++)
                            {
                                lsTmp.Add("B相" + i + "次谐波电压含有率：" + GetTheData("gd5", 2));
                            }
                            lsTmp.Add("C相总谐波电压含有率：" + GetTheData("gd5", 2));
                            for (int i = 2; i <= ins1; i++)
                            {
                                lsTmp.Add("C相" + i + "次谐波电压含有率：" + GetTheData("gd5", 2));
                            }
                            lsTmp.Add("A相总谐波电流含有率：" + GetTheData("gd5", 2));
                            for (int i = 2; i <= ins1; i++)
                            {
                                lsTmp.Add("A相" + i + "次谐波电流含有率：" + GetTheData("gd5", 2));
                            }
                            lsTmp.Add("A相总谐波电流含有率：" + GetTheData("gd5", 2));
                            for (int i = 2; i <= ins1; i++)
                            {
                                lsTmp.Add("B相" + i + "次谐波电流含有率：" + GetTheData("gd5", 2));
                            }
                            lsTmp.Add("A相总谐波电流含有率：" + GetTheData("gd5", 2));
                            for (int i = 2; i <= ins1; i++)
                            {
                                lsTmp.Add("C相" + i + "次谐波电流含有率：" + GetTheData("gd5", 2));
                            }
                        }
                        break;
                    case 65:
                        lsTmp.Add("Fn = " + Fn + ",当前电容器投切状态：");
                        if (BoolUp)
                        {
                            lsTmp.Add("运行方式：" + GetTheData("ExplanNo", 1));
                            lsTmp.Add("电容器的投切状态：" + GetTheData("ExplanNo", 2));
                        }
                        break;
                    case 67:
                        GetOneData("当日、当月电容器累计补偿的无功电能量", new string[] {
                            "当前日补偿的无功电能量","gd13", "4",
                            "当前月补偿的无功电能量","gd13", "4"
                        });
                        break;
                    case 73:
                        GetOneData("直流模拟量当前数据", new string[] {
                            "直流模拟量当前数据","gd2", "2"
                        });
                        break;
                    case 81:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结总加有功功率：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结总加有功功率1","gd2", "2",
                                 "小时冻结总加有功功率2","gd2", "2",
                                 "小时冻结总加有功功率3","gd2", "2",
                                 "小时冻结总加有功功率4","gd2", "2"
                            });
                        }
                        break;
                    case 82:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结总加无功功率：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结总加无功功率1","gd2", "2",
                                 "小时冻结总加无功功率2","gd2", "2",
                                 "小时冻结总加无功功率3","gd2", "2",
                                 "小时冻结总加无功功率4","gd2", "2"
                            });

                        }
                        break;
                    case 83:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结总加有功总电能量：");
                        if (BoolUp)
                        {

                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结总加有功总电能量1","gd3", "4",
                                 "小时冻结总加有功总电能量2","gd3", "4",
                                 "小时冻结总加有功总电能量3","gd3", "4",
                                 "小时冻结总加有功总电能量4","gd3", "4"
                            });

                        }
                        break;
                    case 84:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结总加无功总电能量：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结总加无功总电能量1","gd3", "4",
                                 "小时冻结总加无功总电能量2","gd3", "4",
                                 "小时冻结总加无功总电能量3","gd3", "4",
                                 "小时冻结总加无功总电能量4","gd3", "4"
                             });

                        }
                        break;
                    case 89:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结有功功率：");
                        if (BoolUp)
                        {

                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结有功功率1","gd2", "2",
                                 "小时冻结有功功率2","gd2", "2",
                                 "小时冻结有功功率3","gd2", "2",
                                 "小时冻结有功功率4","gd2", "2"
                            });
                        }
                        break;
                    case 90:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结A相有功功率：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结A相有功功率1","gd9", "3",
                                 "小时冻结A相有功功率2","gd9", "3",
                                 "小时冻结A相有功功率3","gd9", "3",
                                 "小时冻结A相有功功率4","gd9", "3"
                            });

                        }
                        break;
                    case 91:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结B相有功功率：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1)); GetOneData(new string[]{
                                 "小时冻结B相有功功率1","gd9", "3",
                                 "小时冻结B相有功功率2","gd9", "3",
                                 "小时冻结B相有功功率3","gd9", "3",
                                 "小时冻结B相有功功率4","gd9", "3"
                            });
                        }
                        break;
                    case 92:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结C相有功功率：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1)); GetOneData(new string[]{
                                 "小时冻结C相有功功率1","gd9", "3",
                                 "小时冻结C相有功功率2","gd9", "3",
                                 "小时冻结C相有功功率3","gd9", "3",
                                 "小时冻结C相有功功率4","gd9", "3"
                            });
                        }
                        break;
                    case 93:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结无功功率：");
                        if (BoolUp)
                        {

                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结无功功率1","gd2", "2",
                                 "小时冻结无功功率2","gd2", "2",
                                 "小时冻结无功功率3","gd2", "2",
                                 "小时冻结无功功率4","gd2", "2"
                            });
                        }
                        break;
                    case 94:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结A相无功功率：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结A相无功功率1","gd9", "3",
                                 "小时冻结A相无功功率2","gd9", "3",
                                 "小时冻结A相无功功率3","gd9", "3",
                                 "小时冻结A相无功功率4","gd9", "3"
                            });

                        }
                        break;
                    case 95:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结B相无功功率：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1)); GetOneData(new string[]{
                                 "小时冻结B相无功功率1","gd9", "3",
                                 "小时冻结B相无功功率2","gd9", "3",
                                 "小时冻结B相无功功率3","gd9", "3",
                                 "小时冻结B相无功功率4","gd9", "3"
                            });
                        }
                        break;
                    case 96:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结C相无功功率：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1)); GetOneData(new string[]{
                                 "小时冻结C相无功功率1","gd9", "3",
                                 "小时冻结C相无功功率2","gd9", "3",
                                 "小时冻结C相无功功率3","gd9", "3",
                                 "小时冻结C相无功功率4","gd9", "3"
                            });
                        }
                        break;
                    case 97:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结A相电压：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结A电压1","gd7", "2",
                                 "小时冻结A电压2","gd7", "2",
                                 "小时冻结A电压3","gd7", "2",
                                 "小时冻结A电压4","gd7", "2"
                            });

                        }
                        break;
                    case 98:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结B相电压：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结B电压1","gd7", "2",
                                 "小时冻结B电压2","gd7", "2",
                                 "小时冻结B电压3","gd7", "2",
                                 "小时冻结B电压4","gd7", "2"
                            });

                        }
                        break;
                    case 99:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结C相电压：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1)); GetOneData(new string[]{
                                 "小时冻结C电压1","gd7", "2",
                                 "小时冻结C电压2","gd7", "2",
                                 "小时冻结C电压3","gd7", "2",
                                 "小时冻结C电压4","gd7", "2"
                            });
                        }
                        break;
                    case 100:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结A相电流：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1)); GetOneData(new string[]{
                                 "小时冻结A电流1","gd25", "3",
                                 "小时冻结A电流2","gd25", "3",
                                 "小时冻结A电流3","gd25", "3",
                                 "小时冻结A电流4","gd25", "3"
                            });
                        }
                        break;
                    case 101:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结B相电流：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1)); GetOneData(new string[]{
                                 "小时冻结B电流1","gd25", "3",
                                 "小时冻结B电流2","gd25", "3",
                                 "小时冻结B电流3","gd25", "3",
                                 "小时冻结B电流4","gd25", "3"
                            });
                        }
                        break;
                    case 102:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结C相电流：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1)); GetOneData(new string[]{
                                 "小时冻结C电流1","gd25", "3",
                                 "小时冻结C电流2","gd25", "3",
                                 "小时冻结C电流3","gd25", "3",
                                 "小时冻结C电流4","gd25", "3"
                            });
                        }
                        break;
                    case 103:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结零序电流：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结零序电流1","gd25", "3",
                                 "小时冻结零序电流2","gd25", "3",
                                 "小时冻结零序电流3","gd25", "3",
                                 "小时冻结零序电流4","gd25", "3"
                            });

                        }
                        break;
                    case 105:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结正向有功总电能量：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结正向有功总电能量1","gd13", "4",
                                 "小时冻结正向有功总电能量2","gd13", "4",
                                 "小时冻结正向有功总电能量3","gd13", "4",
                                 "小时冻结正向有功总电能量4","gd13", "4"
                            });
                        }
                        break;
                    case 106:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结正向无功总电能量：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结正向无功总电能量1","gd13", "4",
                                 "小时冻结正向无功总电能量2","gd13", "4",
                                 "小时冻结正向无功总电能量3","gd13", "4",
                                 "小时冻结正向无功总电能量4","gd13", "4"
                            });

                        }
                        break;
                    case 107:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结反向有功总电能量：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1)); GetOneData(new string[]{
                                 "小时冻结反向有功总电能量1","gd13", "4",
                                 "小时冻结反向有功总电能量2","gd13", "4",
                                 "小时冻结反向有功总电能量3","gd13", "4",
                                 "小时冻结反向有功总电能量4","gd13", "4"
                            });

                        }
                        break;
                    case 108:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结反向无功总电能量：");
                        if (BoolUp)
                        {

                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结反向无功总电能量1","gd13", "4",
                                 "小时冻结反向无功总电能量2","gd13", "4",
                                 "小时冻结反向无功总电能量3","gd13", "4",
                                 "小时冻结反向无功总电能量4","gd13", "4"
                            });

                        }
                        break;
                    case 109:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结正向有功总电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结正向有功总电能示值1","gd11", "4",
                                 "小时冻结正向有功总电能示值2","gd11", "4",
                                 "小时冻结正向有功总电能示值3","gd11", "4",
                                 "小时冻结正向有功总电能示值4","gd11", "4"
                            });

                        }
                        break;
                    case 110:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结正向无功总电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1)); GetOneData(new string[]{
                                 "小时冻结正向无功总电能示值1","gd11", "4",
                                 "小时冻结正向无功总电能示值2","gd11", "4",
                                 "小时冻结正向无功总电能示值3","gd11", "4",
                                 "小时冻结正向无功总电能示值4","gd11", "4"
                            });

                        }
                        break;
                    case 111:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结反向有功总电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1)); GetOneData(new string[]{
                                 "小时冻结反向有功总电能示值1","gd11", "4",
                                 "小时冻结反向有功总电能示值2","gd11", "4",
                                 "小时冻结反向有功总电能示值3","gd11", "4",
                                 "小时冻结反向有功总电能示值4","gd11", "4"
                            });

                        }
                        break;
                    case 112:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结反向无功总电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[]{
                                 "小时冻结反向无功总电能示值1","gd11", "4",
                                 "小时冻结反向无功总电能示值2","gd11", "4",
                                 "小时冻结反向无功总电能示值3","gd11", "4",
                                 "小时冻结反向无功总电能示值4","gd11", "4"
                            });

                        }
                        break;
                    case 113:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结总功率因数：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[] {
                                "小时冻结总功率因数1", "gd5","2",
                                "小时冻结总功率因数2", "gd5","2",
                                "小时冻结总功率因数3", "gd5","2",
                                "小时冻结总功率因数4" ,"gd5","2"});

                        }
                        break;
                    case 114:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结A相功率因数：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[] {
                                "小时冻结B相功率因数1", "gd5","2",
                                "小时冻结B相功率因数2", "gd5","2",
                                "小时冻结B相功率因数3", "gd5","2",
                                "小时冻结B相功率因数4" ,"gd5","2"});

                        }
                        break;
                    case 115:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结B相功率因数：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[] {
                                "小时冻结B相功率因数1", "gd5","2",
                                "小时冻结B相功率因数2", "gd5","2",
                                "小时冻结B相功率因数3", "gd5","2",
                                "小时冻结B相功率因数4" ,"gd5","2"});
                        }
                        break;
                    case 116:
                        lsTmp.Add("Fn = " + Fn + ",小时冻结总功率因数：");
                        if (BoolUp)
                        {
                            lsTmp.Add("小时冻结类数据时标Td_h：" + GetTheData("HourTd_h", 1) + ":00 " + GetTheData("getdjmd", 1));
                            GetOneData(new string[] {
                                "小时冻结C相功率因数1", "gd5","2",
                                "小时冻结C相功率因数2", "gd5","2",
                                "小时冻结C相功率因数3", "gd5","2",
                                "小时冻结C相功率因数4" ,"gd5","2"});

                        }
                        break;
                    case 129:
                        GetOneData("当前正向有功电能示值", new string[] {
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "当前正向有功总电能示值","gd14","5",
                            "当前费率1正向有功总电能示值","gd14","5",
                            "当前费率2正向有功总电能示值","gd14","5",
                            "当前费率3正向有功总电能示值","gd14","5",
                            "当前费率4正向有功总电能示值","gd14","5"
                            });
                        break;
                    case 130:
                        GetOneData("当前正向无功(组合无功1)电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "正向无功(组合无功1)总电能示值","gd11","4",
                            "费率1正向无功(组合无功1)总电能示值","gd11","4",
                            "费率2正向无功(组合无功1)总电能示值","gd11","4",
                            "费率3正向无功(组合无功1)总电能示值","gd11","4",
                            "费率4正向无功(组合无功1)总电能示值","gd11","4"
                            });
                        break;
                    case 131:
                        GetOneData("当前反向有功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "当前反向有功总电能示值","gd14","5",
                            "当前费率1反向有功总电能示值","gd14","5",
                            "当前费率2反向有功总电能示值","gd14","5",
                            "当前费率3反向有功总电能示值","gd14","5",
                            "当前费率4反向有功总电能示值","gd14","5"
                            });

                        break;
                    case 132:
                        GetOneData("当前反向无功(组合无功1)电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "反向无功(组合无功1)总电能示值","gd11","4",
                            "费率1反向无功(组合无功1)总电能示值","gd11","4",
                            "费率2反向无功(组合无功1)总电能示值","gd11","4",
                            "费率3反向无功(组合无功1)总电能示值","gd11","4",
                            "费率4反向无功(组合无功1)总电能示值","gd11","4"
                            });

                        break;
                    case 133:
                        GetOneData("当前一象限无功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "一象限无功总电能示值","gd11","4",
                            "一象限费率1无功电能示值","gd11","4",
                            "一象限费率2无功电能示值","gd11","4",
                            "一象限费率3无功电能示值","gd11","4",
                            "一象限费率4无功电能示值","gd11","4"
                            });

                        break;
                    case 134:
                        GetOneData("当前二象限无功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "二象限无功总电能示值","gd11","4",
                            "二象限费率1无功电能示值","gd11","4",
                            "二象限费率2无功电能示值","gd11","4",
                            "二象限费率3无功电能示值","gd11","4",
                            "二象限费率4无功电能示值","gd11","4"
                            });
                        break;
                    case 135:
                        GetOneData("当前三象限无功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "三象限无功总电能示值","gd11","4",
                            "三象限费率1无功电能示值","gd11","4",
                            "三象限费率2无功电能示值","gd11","4",
                            "三象限费率3无功电能示值","gd11","4",
                            "三象限费率4无功电能示值","gd11","4"
                            });
                        break;
                    case 136:
                        GetOneData("当前四象限无功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "四象限无功总电能示值","gd11","4",
                            "四象限费率1无功电能示值","gd11","4",
                            "四象限费率2无功电能示值","gd11","4",
                            "四象限费率3无功电能示值","gd11","4",
                            "四象限费率4无功电能示值","gd11","4"
                            });
                        break;
                    case 137:
                        GetOneData("上月（上一结算日）正向有功电能示值", new string[] {
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "上月（上一结算日）正向有功总电能示值","gd14","5",
                            "上月（上一结算日）费率1正向有功总电能示值","gd14","5",
                            "上月（上一结算日）费率2正向有功总电能示值","gd14","5",
                            "上月（上一结算日）费率3正向有功总电能示值","gd14","5",
                            "上月（上一结算日）费率4正向有功总电能示值","gd14","5"
                            });
                        break;
                    case 138:
                        GetOneData("当上月（上一结算日）正向无功(组合无功1)电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "正向无功(组合无功1)总电能示值","gd11","4",
                            "费率1正向无功(组合无功1)总电能示值","gd11","4",
                            "费率2正向无功(组合无功1)总电能示值","gd11","4",
                            "费率3正向无功(组合无功1)总电能示值","gd11","4",
                            "费率4正向无功(组合无功1)总电能示值","gd11","4"
                            });
                        break;
                    case 139:
                        GetOneData("上月（上一结算日）反向有功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "上月（上一结算日）反向有功总电能示值","gd14","5",
                            "上月（上一结算日）费率1反向有功总电能示值","gd14","5",
                            "上月（上一结算日）费率2反向有功总电能示值","gd14","5",
                            "上月（上一结算日）费率3反向有功总电能示值","gd14","5",
                            "上月（上一结算日）费率4反向有功总电能示值","gd14","5"
                            });

                        break;
                    case 140:
                        GetOneData("上月（上一结算日）反向无功(组合无功1)电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "反向无功(组合无功1)总电能示值","gd11","4",
                            "费率1反向无功(组合无功1)总电能示值","gd11","4",
                            "费率2反向无功(组合无功1)总电能示值","gd11","4",
                            "费率3反向无功(组合无功1)总电能示值","gd11","4",
                            "费率4反向无功(组合无功1)总电能示值","gd11","4"
                            });

                        break;
                    case 141:
                        GetOneData("上月（上一结算日）一象限无功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "一象限无功总电能示值","gd11","4",
                            "一象限费率1无功电能示值","gd11","4",
                            "一象限费率2无功电能示值","gd11","4",
                            "一象限费率3无功电能示值","gd11","4",
                            "一象限费率4无功电能示值","gd11","4"
                            });

                        break;
                    case 142:
                        GetOneData("上月（上一结算日）二象限无功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "二象限无功总电能示值","gd11","4",
                            "二象限费率1无功电能示值","gd11","4",
                            "二象限费率2无功电能示值","gd11","4",
                            "二象限费率3无功电能示值","gd11","4",
                            "二象限费率4无功电能示值","gd11","4"
                            });
                        break;
                    case 143:
                        GetOneData("上月（上一结算日）三象限无功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "三象限无功总电能示值","gd11","4",
                            "三象限费率1无功电能示值","gd11","4",
                            "三象限费率2无功电能示值","gd11","4",
                            "三象限费率3无功电能示值","gd11","4",
                            "三象限费率4无功电能示值","gd11","4"
                            });
                        break;
                    case 144:
                        GetOneData("上月（上一结算日）四象限无功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "四象限无功总电能示值","gd11","4",
                            "四象限费率1无功电能示值","gd11","4",
                            "四象限费率2无功电能示值","gd11","4",
                            "四象限费率3无功电能示值","gd11","4",
                            "四象限费率4无功电能示值","gd11","4"
                            });
                        break;
                    case 145:
                        GetOneData("当月正向有功最大需量及发生时间", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "正向有功总最大需量","gd23","3",
                            "正向有功总最大需量发生时间","gd17","4",
                            "正向有功费率1最大需量","gd23","3",
                            "正向有功费率1最大需量发生时间","gd17","4",
                            "正向有功费率2最大需量","gd23","3",
                            "正向有功费率2最大需量发生时间","gd17","4",
                            "正向有功费率3最大需量","gd23","3",
                            "正向有功费率3最大需量发生时间","gd17","4",
                            "正向有功费率4最大需量","gd23","3",
                            "正向有功费率4最大需量发生时间","gd17","4"
                            });

                        break;
                    case 146:
                        GetOneData("当月正向无功最大需量及发生时间", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "正向无功总最大需量","gd23","3",
                            "正向无功总最大需量发生时间","gd17","4",
                            "正向无功费率1最大需量","gd23","3",
                            "正向无功费率1最大需量发生时间","gd17","4",
                            "正向无功费率2最大需量","gd23","3",
                            "正向无功费率2最大需量发生时间","gd17","4",
                            "正向无功费率3最大需量","gd23","3",
                            "正向无功费率3最大需量发生时间","gd17","4",
                            "正向无功费率4最大需量","gd23","3",
                            "正向无功费率4最大需量发生时间","gd17","4"
                            });


                        break;
                    case 147:
                        GetOneData("当月反向有功最大需量及发生时间", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "反向有功总最大需量","gd23","3",
                            "反向有功总最大需量发生时间","gd17","4",
                            "反向有功费率1最大需量","gd23","3",
                            "反向有功费率1最大需量发生时间","gd17","4",
                            "反向有功费率2最大需量","gd23","3",
                            "反向有功费率2最大需量发生时间","gd17","4",
                            "反向有功费率3最大需量","gd23","3",
                            "反向有功费率3最大需量发生时间","gd17","4",
                            "反向有功费率4最大需量","gd23","3",
                            "反向有功费率4最大需量发生时间","gd17","4"
                            });

                        break;
                    case 148:
                        GetOneData("当月反向无功最大需量及发生时间", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "反向无功总最大需量","gd23","3",
                            "反向无功总最大需量发生时间","gd17","4",
                            "反向无功费率1最大需量","gd23","3",
                            "反向无功费率1最大需量发生时间","gd17","4",
                            "反向无功费率2最大需量","gd23","3",
                            "反向无功费率2最大需量发生时间","gd17","4",
                            "反向无功费率3最大需量","gd23","3",
                            "反向无功费率3最大需量发生时间","gd17","4",
                            "反向无功费率4最大需量","gd23","3",
                            "反向无功费率4最大需量发生时间","gd17","4"
                            });
                        break;

                    case 149:
                        GetOneData("上月（上一结算日）正向有功最大需量及发生时间", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "正向有功总最大需量","gd23","3",
                            "正向有功总最大需量发生时间","gd17","4",
                            "正向有功费率1最大需量","gd23","3",
                            "正向有功费率1最大需量发生时间","gd17","4",
                            "正向有功费率2最大需量","gd23","3",
                            "正向有功费率2最大需量发生时间","gd17","4",
                            "正向有功费率3最大需量","gd23","3",
                            "正向有功费率3最大需量发生时间","gd17","4",
                            "正向有功费率4最大需量","gd23","3",
                            "正向有功费率4最大需量发生时间","gd17","4"
                            });

                        break;
                    case 150:
                        GetOneData("上月（上一结算日）正向无功最大需量及发生时间", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "正向无功总最大需量","gd23","3",
                            "正向无功总最大需量发生时间","gd17","4",
                            "正向无功费率1最大需量","gd23","3",
                            "正向无功费率1最大需量发生时间","gd17","4",
                            "正向无功费率2最大需量","gd23","3",
                            "正向无功费率2最大需量发生时间","gd17","4",
                            "正向无功费率3最大需量","gd23","3",
                            "正向无功费率3最大需量发生时间","gd17","4",
                            "正向无功费率4最大需量","gd23","3",
                            "正向无功费率4最大需量发生时间","gd17","4"
                            });


                        break;
                    case 151:
                        GetOneData("上月（上一结算日）反向有功最大需量及发生时间", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "反向有功总最大需量","gd23","3",
                            "反向有功总最大需量发生时间","gd17","4",
                            "反向有功费率1最大需量","gd23","3",
                            "反向有功费率1最大需量发生时间","gd17","4",
                            "反向有功费率2最大需量","gd23","3",
                            "反向有功费率2最大需量发生时间","gd17","4",
                            "反向有功费率3最大需量","gd23","3",
                            "反向有功费率3最大需量发生时间","gd17","4",
                            "反向有功费率4最大需量","gd23","3",
                            "反向有功费率4最大需量发生时间","gd17","4"
                            });

                        break;
                    case 152:
                        GetOneData("上月（上一结算日）反向无功最大需量及发生时间", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M)","ExplanBinFun","1",
                            "反向无功总最大需量","gd23","3",
                            "反向无功总最大需量发生时间","gd17","4",
                            "反向无功费率1最大需量","gd23","3",
                            "反向无功费率1最大需量发生时间","gd17","4",
                            "反向无功费率2最大需量","gd23","3",
                            "反向无功费率2最大需量发生时间","gd17","4",
                            "反向无功费率3最大需量","gd23","3",
                            "反向无功费率3最大需量发生时间","gd17","4",
                            "反向无功费率4最大需量","gd23","3",
                            "反向无功费率4最大需量发生时间","gd17","4"
                            });
                        break;
                    case 162:
                        GetOneData("电能表日历时钟", new string[]{
                            "终端抄表时间","gd1","6",
                            "电能表日历时钟","gd1","6"});
                        break;
                    case 165:
                        GetOneData("电能表开关操作次数及时间", new string[]{
                            "终端抄表时间","gd15","5",
                            "电能表编程次数","gd10","3",
                            "最近一次电能表编程时间","gd1","6",
                            "电能表尾盖打开次数","gd10","3",
                            "最近一次电能表尾盖打开次数","gd1","6"});
                        break;
                    case 166:
                        GetOneData("电能表参数修改次数及时间", new string[]{
                            "终端抄表时间","gd15","5",
                            "校时总次数","gd10","3",
                            "校时前时间","gd1","6",
                            "校时后时间","gd1","6",
                            "时段表编程总次数","gd10","3",
                            "最近一次时段表编程时间","gd1","6"});
                        break;
                    case 170://集中抄表电表相位信息
                        GetOneData("集中抄表电表相位信息", new string[]{
                            "所属终端通信端口号","ExplanBinFun","1",
                            "中继路由级数","ExplanBinFun","1",
                            "载波抄读通信相位","ExplanBinFun","1",
                            "载波信号品质","ExplanBinFun","1",
                            "最近一次抄表成功/失败标志","ExplanBinFun","1",
                            "最近一次抄表成功时间","ExplanBinFun","6",
                            "最近一次抄表失败时间","ExplanBinFun","6",
                            "最近连续失败累计次数","ExplanBinFun","1"});
                        break;
                    case 177:
                        GetOneData("当前组合有功电能示值", new string[]{
                            "终端抄表时间","gd15","5",
                            "费率数M","ExplanBinFun","1",
                            "（当前）组合有功总电能示值","gd14","5",
                            "（当前）组合有功费率1电能示值","gd14","5",
                            "（当前）组合有功费率2电能示值","gd14","5",
                            "（当前）组合有功费率3电能示值","gd14","5",
                            "（当前）组合有功费率4电能示值","gd14","5"});
                        break;
                    case 179:
                        GetOneData("当日电压统计数据", new string[]{
                            "A相电压越上上限当日累计时间","ExplanBinFun","2",
                            "A相电压越下下限当日累计时间","ExplanBinFun","2",
                            "A相电压越上限当日累计时间","ExplanBinFun","2",
                            "A相电压越下限当日累计时间","ExplanBinFun","2",
                            "A相电压合格当日累计时间","ExplanBinFun","2",
                            "B相电压越上上限当日累计时间","ExplanBinFun","2",
                            "B相电压越下下限当日累计时间","ExplanBinFun","2",
                            "B相电压越上限当日累计时间","ExplanBinFun","2",
                            "B相电压越下限当日累计时间","ExplanBinFun","2",
                            "B相电压合格当日累计时间","ExplanBinFun","2",
                            "C相电压越上上限当日累计时间","ExplanBinFun","2",
                            "C相电压越下下限当日累计时间","ExplanBinFun","2",
                            "C相电压越上限当日累计时间","ExplanBinFun","2",
                            "C相电压越下限当日累计时间","ExplanBinFun","2",
                            "C相电压合格当日累计时间","ExplanBinFun","2",
                            "A相电压最大值","gd7","2",
                            "A相电压最大值发生时间","gd18","3",
                            "A相电压最小值","gd7","2",
                            "A相电压最小值发生时间","gd18","3",
                            "B相电压最大值","gd7","2",
                            "B相电压最大值发生时间","gd18","3",
                            "B相电压最小值","gd7","2",
                            "B相电压最小值发生时间","gd18","3",
                            "C相电压最大值","gd7","2",
                            "C相电压最大值发生时间","gd18","3",
                            "C相电压最小值","gd7","2",
                            "C相电压最小值发生时间","gd18","3",
                            "A相平均电压","gd7","2",
                            "B相平均电压","gd7","2",
                            "C相平均电压","gd7","2",
                            "A相电压越上限率","gd25","3",
                            "A相电压越下限率","gd25","3",
                            "A相电压合格率","gd25","3",
                            "B相电压越上限率","gd25","3",
                            "B相电压越下限率","gd25","3",
                            "B相电压合格率","gd25","3",
                            "C相电压越上限率","gd25","3",
                            "C相电压越下限率","gd25","3",
                            "C相电压合格率","gd25","3"});
                        break;
                    case 180:
                        GetOneData("当月电压统计数据", new string[]{
                            "A相电压越上上限当日累计时间","ExplanBinFun","2",
                            "A相电压越下下限当日累计时间","ExplanBinFun","2",
                            "A相电压越上限当日累计时间","ExplanBinFun","2",
                            "A相电压越下限当日累计时间","ExplanBinFun","2",
                            "A相电压合格当日累计时间","ExplanBinFun","2",
                            "B相电压越上上限当日累计时间","ExplanBinFun","2",
                            "B相电压越下下限当日累计时间","ExplanBinFun","2",
                            "B相电压越上限当日累计时间","ExplanBinFun","2",
                            "B相电压越下限当日累计时间","ExplanBinFun","2",
                            "B相电压合格当日累计时间","ExplanBinFun","2",
                            "C相电压越上上限当日累计时间","ExplanBinFun","2",
                            "C相电压越下下限当日累计时间","ExplanBinFun","2",
                            "C相电压越上限当日累计时间","ExplanBinFun","2",
                            "C相电压越下限当日累计时间","ExplanBinFun","2",
                            "C相电压合格当日累计时间","ExplanBinFun","2",
                            "A相电压最大值","gd7","2",
                            "A相电压最大值发生时间","gd18","3",
                            "A相电压最小值","gd7","2",
                            "A相电压最小值发生时间","gd18","3",
                            "B相电压最大值","gd7","2",
                            "B相电压最大值发生时间","gd18","3",
                            "B相电压最小值","gd7","2",
                            "B相电压最小值发生时间","gd18","3",
                            "C相电压最大值","gd7","2",
                            "C相电压最大值发生时间","gd18","3",
                            "C相电压最小值","gd7","2",
                            "C相电压最小值发生时间","gd18","3",
                            "A相平均电压","gd7","2",
                            "B相平均电压","gd7","2",
                            "C相平均电压","gd7","2",
                            "A相电压越上限率","gd25","3",
                            "A相电压越下限率","gd25","3",
                            "A相电压合格率","gd25","3",
                            "B相电压越上限率","gd25","3",
                            "B相电压越下限率","gd25","3",
                            "B相电压合格率","gd25","3",
                            "C相电压越上限率","gd25","3",
                            "C相电压越下限率","gd25","3",
                            "C相电压合格率","gd25","3"});
                        break;
                    case 201:
                        lsTmp.Add("Fn = " + Fn + ",终端网络信号品质：");

                        string strPin = Convert.ToInt32(GetTheData("ExplanBinFun", 2)).ToString("X4");
                        byte byt = Convert.ToByte(strPin.Substring(0, 2), 16);
                        string strDex = Convert.ToString(Convert.ToByte(strPin.Substring(0, 2), 16), 2).PadLeft(8, '0') + Convert.ToString(Convert.ToByte(strPin.Substring(2, 2), 16), 2).PadLeft(8, '0');

                        int intPin = Convert.ToInt32(strDex.Substring(1, 3), 2) * 100 +
                            Convert.ToInt32(strDex.Substring(4, 4), 2) * 10 +
                            Convert.ToInt32(strDex.Substring(8, 4), 2) +
                            Convert.ToInt32(strDex.Substring(12, 4), 2) / 10;
                        string strMess = "";
                        intPin = -intPin;
                        for (int intInc = 0; intInc <= 31; intInc += 2)
                        {
                            if (intPin < -113 + intInc)
                            {
                                strMess = intInc.ToString();
                                break;
                            }
                        }
                        if (strMess.Length <= 0) strMess = "99";
                        lsTmp.Add("信号品质dBm:" + intPin.ToString());
                        lsTmp.Add("网络信号强度:" + strMess);

                        break;
                    case 245:
                        lsTmp.Add("Fn = " + Fn + ",事件产生编号：");
                        if (BoolUp)
                        {
                            int ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("事件编号个数：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                lsTmp.Add("第" + i + "事件编号：" + GetTheData("ExplanBinFun", 1));
                                lsTmp.Add("第" + i + "事件个数：" + GetTheData("ExplanBinFun", 1));
                            }
                        }
                        break;
                    default:
                        lsTmp.Add("ERR");
                        break;
                }
                if (strFrameRemove.Length > 0)
                    a0C();
            }
            catch
            {

            }
        }
        public void a0D()
        {

            try
            {
                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    case 1:
                        GetTwoData("日冻结正向有/无功电能示值、一/四象限无功电能示值", new string[]{
                            "日冻结类数据时标Td_d","gd20","3",
                            "终端抄表时间","gd15","5",
                            "费率数M","ExplanBinFun","1",
                            "正向有功总电能示值","gd14","5",
                            "费率1正向有功总电能示值","gd14","5",
                            "费率2正向有功总电能示值","gd14","5",
                            "费率3正向有功总电能示值","gd14","5",
                            "费率4正向有功总电能示值","gd14","5",
                            "正向无功(组合无功1)总电能示值","gd11","4",
                            "费率1正向无功(组合无功1)总电能示值","gd11","4",
                            "费率2正向无功(组合无功1)总电能示值","gd11","4",
                            "费率3正向无功(组合无功1)总电能示值","gd11","4",
                            "费率4正向无功(组合无功1)总电能示值","gd11","4",
                            "一象限无功总电能示值","gd11","4",
                            "一象限费率1无功电能示值","gd11","4",
                            "一象限费率2无功电能示值","gd11","4",
                            "一象限费率3无功电能示值","gd11","4",
                            "一象限费率4无功电能示值","gd11","4",
                            "四象限无功总电能示值","gd11","4",
                            "四象限费率1无功电能示值","gd11","4",
                            "四象限费率2无功电能示值","gd11","4",
                            "四象限费率3无功电能示值","gd11","4",
                            "四象限费率4无功电能示值","gd11","4"});
                        break;
                    case 2:
                        GetTwoData("日冻结反向有/无功电能示值、二/三象限无功电能示值", new string[]{
                            "日冻结类数据时标Td_d","gd20","3",
                            "终端抄表时间","gd15","5",
                            "费率数M","ExplanBinFun","1",
                            "反向有功总电能示值","gd14","5",
                            "费率1反向有功总电能示值","gd14","5",
                            "费率2反向有功总电能示值","gd14","5",
                            "费率3反向有功总电能示值","gd14","5",
                            "费率4反向有功总电能示值","gd14","5",
                            "反向无功(组合无功2)总电能示值","gd11","4",
                            "费率1反向无功(组合无功2)总电能示值","gd11","4",
                            "费率2反向无功(组合无功2)总电能示值","gd11","4",
                            "费率3反向无功(组合无功2)总电能示值","gd11","4",
                            "费率4反向无功(组合无功2)总电能示值","gd11","4",
                            "二象限无功总电能示值","gd11","4",
                            "二象限费率1无功电能示值","gd11","4",
                            "二象限费率2无功电能示值","gd11","4",
                            "二象限费率3无功电能示值","gd11","4",
                            "二象限费率4无功电能示值","gd11","4",
                            "三象限无功总电能示值","gd11","4",
                            "三象限费率1无功电能示值","gd11","4",
                            "三象限费率2无功电能示值","gd11","4",
                            "三象限费率3无功电能示值","gd11","4",
                            "三象限费率4无功电能示值","gd11","4"});
                        break;
                    case 3:

                        GetTwoData("日冻结正向有/无功最大需量及发生时间", new string[]{
                            "日冻结类数据时标Td_d","gd20","3",
                            "终端抄表时间","gd15","5",
                            "费率数M","ExplanBinFun","1",
                            "正向有功总最大需量", "gd23","3",
                            "正向有功费率1最大需量", "gd23","3",
                            "正向有功费率2最大需量", "gd23","3",
                            "正向有功费率3最大需量","gd23","3",
                            "正向有功费率4最大需量","gd23","3",
                            "正向有功总最大需量发生时间","gd17","4",
                            "正向有功费率1最大需量发生时间","gd17","4",
                            "正向有功费率2最大需量发生时间", "gd17","4",
                            "正向有功费率3最大需量发生时间", "gd17","4",
                            "正向有功费率4最大需量发生时间", "gd17","4",
                            "正向无功总最大需量","gd23","3",
                            "正向无功费率1最大需量", "gd23","3",
                            "正向无功费率2最大需量", "gd23","3",
                            "正向无功费率3最大需量", "gd23","3",
                            "正向无功费率4最大需量", "gd23","3",
                            "正向无功总最大需量发生时间", "gd17","4",
                            "正向无功费率1最大需量发生时间", "gd17","4",
                            "正向无功费率2最大需量发生时间", "gd17","4",
                            "正向无功费率3最大需量发生时间", "gd17","4",
                            "正向无功费率4最大需量发生时间","gd17","4"});

                        break;
                    case 4:
                        GetTwoData("日冻结反向有/无功最大需量及发生时间", new string[]{
                            "日冻结类数据时标Td_d","gd20","3",
                            "终端抄表时间","gd15","5",
                            "费率数M","ExplanBinFun","1",
                            "反向有功总最大需量", "gd23","3",
                            "反向有功费率1最大需量", "gd23","3",
                            "反向有功费率2最大需量", "gd23","3",
                            "反向有功费率3最大需量","gd23","3",
                            "反向有功费率4最大需量","gd23","3",
                            "反向有功总最大需量发生时间","gd17","4",
                            "反向有功费率1最大需量发生时间","gd17","4",
                            "反向有功费率2最大需量发生时间", "gd17","4",
                            "反向有功费率3最大需量发生时间", "gd17","4",
                            "反向有功费率4最大需量发生时间", "gd17","4",
                            "反向无功总最大需量","gd23","3",
                            "反向无功费率1最大需量", "gd23","3",
                            "反向无功费率2最大需量", "gd23","3",
                            "反向无功费率3最大需量", "gd23","3",
                            "反向无功费率4最大需量", "gd23","3",
                            "反向无功总最大需量发生时间", "gd17","4",
                            "反向无功费率1最大需量发生时间", "gd17","4",
                            "反向无功费率2最大需量发生时间", "gd17","4",
                            "反向无功费率3最大需量发生时间", "gd17","4",
                            "反向无功费率4最大需量发生时间","gd17","4"});

                        break;
                    case 5:
                        GetTwoData("日冻结正向有功电能量", new string[]{
                            "日冻结类数据时标Td_d","gd20","3",
                            "费率数M","ExplanBinFun","1",
                            "日正向有功总电能量", "gd13","4",
                            "日费率1正向有功电能量", "gd13","4",
                            "日费率1正向有功电能量", "gd13","4",
                            "日费率1正向有功电能量","gd13","4",
                            "日费率1正向有功电能量","gd13","4"
                          });
                        break;
                    case 6:
                        GetTwoData("日冻结正向无功电能量", new string[]{
                            "日冻结类数据时标Td_d","gd20","3",
                            "费率数M","ExplanBinFun","1",
                            "日正向有功总电能量", "gd13","4",
                            "日费率1正向无功电能量", "gd13","4",
                            "日费率1正向无功电能量", "gd13","4",
                            "日费率1正向无功电能量","gd13","4",
                            "日费率1正向无功电能量","gd13","4"
                          });
                        break;
                    case 7:
                        GetTwoData("日冻结反向有功电能量", new string[]{
                            "日冻结类数据时标Td_d","gd20","3",
                            "费率数M","ExplanBinFun","1",
                            "日反向有功总电能量", "gd13","4",
                            "日费率1反向有功电能量", "gd13","4",
                            "日费率1反向有功电能量", "gd13","4",
                            "日费率1反向有功电能量","gd13","4",
                            "日费率1反向有功电能量","gd13","4"
                          });
                        break;
                    case 8:
                        GetTwoData("日冻结反向无功电能量", new string[]{
                            "日冻结类数据时标Td_d","gd20","3",
                            "费率数M","ExplanBinFun","1",
                            "日反向有功总电能量", "gd13","4",
                            "日费率1反向无功电能量", "gd13","4",
                            "日费率1反向无功电能量", "gd13","4",
                            "日费率1反向无功电能量","gd13","4",
                            "日费率1反向无功电能量","gd13","4"
                          });
                        break;
                    case 9:
                        GetTwoData("抄表日冻结正向有/无功电能示值、一/四象限无功电能示值", new string[]{
                            "日冻结类数据时标Td_d","gd20","3",
                            "终端抄表时间","gd15","5",
                            "费率数M","ExplanBinFun","1",
                            "正向有功总电能示值","gd14","5",
                            "费率1正向有功总电能示值","gd14","5",
                            "费率2正向有功总电能示值","gd14","5",
                            "费率3正向有功总电能示值","gd14","5",
                            "费率4正向有功总电能示值","gd14","5",
                            "正向无功(组合无功1)总电能示值","gd11","4",
                            "费率1正向无功(组合无功1)总电能示值","gd11","4",
                            "费率2正向无功(组合无功1)总电能示值","gd11","4",
                            "费率3正向无功(组合无功1)总电能示值","gd11","4",
                            "费率4正向无功(组合无功1)总电能示值","gd11","4",
                            "一象限无功总电能示值","gd11","4",
                            "一象限费率1无功电能示值","gd11","4",
                            "一象限费率2无功电能示值","gd11","4",
                            "一象限费率3无功电能示值","gd11","4",
                            "一象限费率4无功电能示值","gd11","4",
                            "四象限无功总电能示值","gd11","4",
                            "四象限费率1无功电能示值","gd11","4",
                            "四象限费率2无功电能示值","gd11","4",
                            "四象限费率3无功电能示值","gd11","4",
                            "四象限费率4无功电能示值","gd11","4"});
                        break;
                    case 10:
                        GetTwoData("抄表日冻结反向有/无功电能示值、二/三象限无功电能示值", new string[]{
                            "日冻结类数据时标Td_d","gd20","3",
                            "终端抄表时间","gd15","5",
                            "费率数M","ExplanBinFun","1",
                            "反向有功总电能示值","gd14","5",
                            "费率1反向有功总电能示值","gd14","5",
                            "费率2反向有功总电能示值","gd14","5",
                            "费率3反向有功总电能示值","gd14","5",
                            "费率4反向有功总电能示值","gd14","5",
                            "反向无功(组合无功2)总电能示值","gd11","4",
                            "费率1反向无功(组合无功2)总电能示值","gd11","4",
                            "费率2反向无功(组合无功2)总电能示值","gd11","4",
                            "费率3反向无功(组合无功2)总电能示值","gd11","4",
                            "费率4反向无功(组合无功2)总电能示值","gd11","4",
                            "二象限无功总电能示值","gd11","4",
                            "二象限费率1无功电能示值","gd11","4",
                            "二象限费率2无功电能示值","gd11","4",
                            "二象限费率3无功电能示值","gd11","4",
                            "二象限费率4无功电能示值","gd11","4",
                            "三象限无功总电能示值","gd11","4",
                            "三象限费率1无功电能示值","gd11","4",
                            "三象限费率2无功电能示值","gd11","4",
                            "三象限费率3无功电能示值","gd11","4",
                            "三象限费率4无功电能示值","gd11","4"});
                        break;
                    case 11:

                        GetTwoData("抄表日冻结正向有/无功最大需量及发生时间", new string[]{
                            "日冻结类数据时标Td_d","gd20","3",
                            "终端抄表时间","gd15","5",
                            "费率数M","ExplanBinFun","1",
                            "反向有功总最大需量", "gd23","3",
                            "反向有功费率1最大需量", "gd23","3",
                            "反向有功费率2最大需量", "gd23","3",
                            "反向有功费率3最大需量","gd23","3",
                            "反向有功费率4最大需量","gd23","3",
                            "反向有功总最大需量发生时间","gd17","4",
                            "反向有功费率1最大需量发生时间","gd17","4",
                            "反向有功费率2最大需量发生时间", "gd17","4",
                            "反向有功费率3最大需量发生时间", "gd17","4",
                            "反向有功费率4最大需量发生时间", "gd17","4",
                            "反向无功总最大需量","gd23","3",
                            "反向无功费率1最大需量", "gd23","3",
                            "反向无功费率2最大需量", "gd23","3",
                            "反向无功费率3最大需量", "gd23","3",
                            "反向无功费率4最大需量", "gd23","3",
                            "反向无功总最大需量发生时间", "gd17","4",
                            "反向无功费率1最大需量发生时间", "gd17","4",
                            "反向无功费率2最大需量发生时间", "gd17","4",
                            "反向无功费率3最大需量发生时间", "gd17","4",
                            "反向无功费率4最大需量发生时间","gd17","4"});

                        break;
                    case 12:
                        GetTwoData("抄表日冻结反向有/无功最大需量及发生时间", new string[]{
                            "日冻结类数据时标Td_d","gd20","3",
                            "终端抄表时间","gd15","5",
                            "费率数M","ExplanBinFun","1",
                            "反向有功总最大需量", "gd23","3",
                            "反向有功费率1最大需量", "gd23","3",
                            "反向有功费率2最大需量", "gd23","3",
                            "反向有功费率3最大需量","gd23","3",
                            "反向有功费率4最大需量","gd23","3",
                            "反向有功总最大需量发生时间","gd17","4",
                            "反向有功费率1最大需量发生时间","gd17","4",
                            "反向有功费率2最大需量发生时间", "gd17","4",
                            "反向有功费率3最大需量发生时间", "gd17","4",
                            "反向有功费率4最大需量发生时间", "gd17","4",
                            "反向无功总最大需量","gd23","3",
                            "反向无功费率1最大需量", "gd23","3",
                            "反向无功费率2最大需量", "gd23","3",
                            "反向无功费率3最大需量", "gd23","3",
                            "反向无功费率4最大需量", "gd23","3",
                            "反向无功总最大需量发生时间", "gd17","4",
                            "反向无功费率1最大需量发生时间", "gd17","4",
                            "反向无功费率2最大需量发生时间", "gd17","4",
                            "反向无功费率3最大需量发生时间", "gd17","4",
                            "反向无功费率4最大需量发生时间","gd17","4"});

                        break;
                    case 17:
                        GetTwoData("月冻结正向有/无功电能示值、一/四象限无功电能示值", new string[]{
                            "月冻结类数据时标Td_d","gd21","2",
                            "终端抄表时间","gd15","5",
                            "费率数M","ExplanBinFun","1",
                            "正向有功总电能示值","gd14","5",
                            "费率1正向有功总电能示值","gd14","5",
                            "费率2正向有功总电能示值","gd14","5",
                            "费率3正向有功总电能示值","gd14","5",
                            "费率4正向有功总电能示值","gd14","5",
                            "正向无功(组合无功1)总电能示值","gd11","4",
                            "费率1正向无功(组合无功1)总电能示值","gd11","4",
                            "费率2正向无功(组合无功1)总电能示值","gd11","4",
                            "费率3正向无功(组合无功1)总电能示值","gd11","4",
                            "费率4正向无功(组合无功1)总电能示值","gd11","4",
                            "一象限无功总电能示值","gd11","4",
                            "一象限费率1无功电能示值","gd11","4",
                            "一象限费率2无功电能示值","gd11","4",
                            "一象限费率3无功电能示值","gd11","4",
                            "一象限费率4无功电能示值","gd11","4",
                            "四象限无功总电能示值","gd11","4",
                            "四象限费率1无功电能示值","gd11","4",
                            "四象限费率2无功电能示值","gd11","4",
                            "四象限费率3无功电能示值","gd11","4",
                            "四象限费率4无功电能示值","gd11","4"});
                        break;
                    case 18:
                        GetTwoData("月冻结反向有/无功电能示值、二/三象限无功电能示值", new string[]{
                            "月冻结类数据时标Td_d","gd21","2",
                            "终端抄表时间","gd15","5",
                            "费率数M","ExplanBinFun","1",
                            "反向有功总电能示值","gd14","5",
                            "费率1反向有功总电能示值","gd14","5",
                            "费率2反向有功总电能示值","gd14","5",
                            "费率3反向有功总电能示值","gd14","5",
                            "费率4反向有功总电能示值","gd14","5",
                            "反向无功(组合无功2)总电能示值","gd11","4",
                            "费率1反向无功(组合无功2)总电能示值","gd11","4",
                            "费率2反向无功(组合无功2)总电能示值","gd11","4",
                            "费率3反向无功(组合无功2)总电能示值","gd11","4",
                            "费率4反向无功(组合无功2)总电能示值","gd11","4",
                            "二象限无功总电能示值","gd11","4",
                            "二象限费率1无功电能示值","gd11","4",
                            "二象限费率2无功电能示值","gd11","4",
                            "二象限费率3无功电能示值","gd11","4",
                            "二象限费率4无功电能示值","gd11","4",
                            "三象限无功总电能示值","gd11","4",
                            "三象限费率1无功电能示值","gd11","4",
                            "三象限费率2无功电能示值","gd11","4",
                            "三象限费率3无功电能示值","gd11","4",
                            "三象限费率4无功电能示值","gd11","4"});
                        break;
                    case 19:

                        GetTwoData("月冻结正向有/无功最大需量及发生时间", new string[]{
                            "月冻结类数据时标Td_d","gd21","2",
                            "终端抄表时间","gd15","5",
                            "费率数M","ExplanBinFun","1",
                            "正向有功总最大需量", "gd23","3",
                            "正向有功费率1最大需量", "gd23","3",
                            "正向有功费率2最大需量", "gd23","3",
                            "正向有功费率3最大需量","gd23","3",
                            "正向有功费率4最大需量","gd23","3",
                            "正向有功总最大需量发生时间","gd17","4",
                            "正向有功费率1最大需量发生时间","gd17","4",
                            "正向有功费率2最大需量发生时间", "gd17","4",
                            "正向有功费率3最大需量发生时间", "gd17","4",
                            "正向有功费率4最大需量发生时间", "gd17","4",
                            "正向无功总最大需量","gd23","3",
                            "正向无功费率1最大需量", "gd23","3",
                            "正向无功费率2最大需量", "gd23","3",
                            "正向无功费率3最大需量", "gd23","3",
                            "正向无功费率4最大需量", "gd23","3",
                            "正向无功总最大需量发生时间", "gd17","4",
                            "正向无功费率1最大需量发生时间", "gd17","4",
                            "正向无功费率2最大需量发生时间", "gd17","4",
                            "正向无功费率3最大需量发生时间", "gd17","4",
                            "正向无功费率4最大需量发生时间","gd17","4"});

                        break;
                    case 20:
                        GetTwoData("月冻结反向有/无功最大需量及发生时间", new string[]{
                            "月冻结类数据时标Td_d","gd21","2",
                            "终端抄表时间","gd15","5",
                            "费率数M","ExplanBinFun","1",
                            "反向有功总最大需量", "gd23","3",
                            "反向有功费率1最大需量", "gd23","3",
                            "反向有功费率2最大需量", "gd23","3",
                            "反向有功费率3最大需量","gd23","3",
                            "反向有功费率4最大需量","gd23","3",
                            "反向有功总最大需量发生时间","gd17","4",
                            "反向有功费率1最大需量发生时间","gd17","4",
                            "反向有功费率2最大需量发生时间", "gd17","4",
                            "反向有功费率3最大需量发生时间", "gd17","4",
                            "反向有功费率4最大需量发生时间", "gd17","4",
                            "反向无功总最大需量","gd23","3",
                            "反向无功费率1最大需量", "gd23","3",
                            "反向无功费率2最大需量", "gd23","3",
                            "反向无功费率3最大需量", "gd23","3",
                            "反向无功费率4最大需量", "gd23","3",
                            "反向无功总最大需量发生时间", "gd17","4",
                            "反向无功费率1最大需量发生时间", "gd17","4",
                            "反向无功费率2最大需量发生时间", "gd17","4",
                            "反向无功费率3最大需量发生时间", "gd17","4",
                            "反向无功费率4最大需量发生时间","gd17","4"});

                        break;
                    case 21:
                        GetTwoData("月冻结正向有功电能量", new string[]{
                            "月冻结类数据时标Td_d","gd21","2",
                            "费率数M","ExplanBinFun","1",
                            "月正向有功总电能量", "gd13","4",
                            "月费率1正向有功电能量", "gd13","4",
                            "月费率1正向有功电能量", "gd13","4",
                            "月费率1正向有功电能量","gd13","4",
                            "月费率1正向有功电能量","gd13","4"
                          });
                        break;
                    case 22:
                        GetTwoData("月冻结正向无功电能量", new string[]{
                            "月冻结类数据时标Td_d","gd21","2",
                            "费率数M","ExplanBinFun","1",
                            "月正向有功总电能量", "gd13","4",
                            "月费率1正向无功电能量", "gd13","4",
                            "月费率1正向无功电能量", "gd13","4",
                            "月费率1正向无功电能量","gd13","4",
                            "月费率1正向无功电能量","gd13","4"
                          });
                        break;
                    case 23:
                        GetTwoData("月冻结反向有功电能量", new string[]{
                            "月冻结类数据时标Td_d","gd21","2",
                            "费率数M","ExplanBinFun","1",
                            "月反向有功总电能量", "gd13","4",
                            "月费率1反向有功电能量", "gd13","4",
                            "月费率1反向有功电能量", "gd13","4",
                            "月费率1反向有功电能量","gd13","4",
                            "月费率1反向有功电能量","gd13","4"
                          });
                        break;
                    case 24:
                        GetTwoData("月冻结反向无功电能量", new string[]{
                            "月冻结类数据时标Td_d","gd21","2",
                            "费率数M","ExplanBinFun","1",
                            "月反向有功总电能量", "gd13","4",
                            "月费率1反向无功电能量", "gd13","4",
                            "月费率1反向无功电能量", "gd13","4",
                            "月费率1反向无功电能量","gd13","4",
                            "月费率1反向无功电能量","gd13","4"
                          });
                        break;
                    case 25:
                        lsTmp.Add("Fn = " + Fn + ",日冻结日总及分相最大有功功率及发生时间、有功功率为零时间：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("三相总最大有功功率：" + GetTheData("gd23", 3));
                            lsTmp.Add("三相总最大有功功率发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("A相最大有功功率：" + GetTheData("gd23", 3));
                            lsTmp.Add("A相最大有功功率发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("B相最大有功功率：" + GetTheData("gd23", 3));
                            lsTmp.Add("B相最大有功功率发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("C相最大有功功率：" + GetTheData("gd23", 3));
                            lsTmp.Add("C相最大有功功率发生时间：" + GetTheData("gd18", 3));

                            lsTmp.Add("A相有功功率为零时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("B相有功功率为零时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("C相有功功率为零时间：" + GetTheData("ExplanBinFun", 2));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 26:
                        lsTmp.Add("Fn = " + Fn + ",日冻结日总及分相最大需量及发生时间：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("三相总有功最大需量：" + GetTheData("gd23", 3));
                            lsTmp.Add("三相总有功最大需量发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("A相有功最大需量：" + GetTheData("gd23", 3));
                            lsTmp.Add("A相有功最大需量发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("B相有功最大需量：" + GetTheData("gd23", 3));
                            lsTmp.Add("B相有功最大需量发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("C相有功最大需量：" + GetTheData("gd23", 3));
                            lsTmp.Add("C相有功最大需量发生时间：" + GetTheData("gd18", 3));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 27:
                        lsTmp.Add("Fn = " + Fn + ",日冻结日电压统计数据：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("A相电压越上上限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("A相电压越下下限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("A相电压越上限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("A相电压越下限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("A相电压合格日累计时间：" + GetTheData("ExplanBinFun", 2));

                            lsTmp.Add("B相电压越上上限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("B相电压越下下限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("B相电压越上限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("B相电压越下限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("B相电压合格日累计时间：" + GetTheData("ExplanBinFun", 2));

                            lsTmp.Add("C相电压越上上限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("C相电压越下下限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("C相电压越上限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("C相电压越下限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("C相电压合格日累计时间：" + GetTheData("ExplanBinFun", 2));

                            lsTmp.Add("A相电压最大值：" + GetTheData("gd7", 2));
                            lsTmp.Add("A相电压最大值发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("A相电压最小值：" + GetTheData("gd7", 2));
                            lsTmp.Add("A相电压最小值发生时间：" + GetTheData("gd18", 3));

                            lsTmp.Add("B相电压最大值：" + GetTheData("gd7", 2));
                            lsTmp.Add("B相电压最大值发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("B相电压最小值：" + GetTheData("gd7", 2));
                            lsTmp.Add("B相电压最小值发生时间：" + GetTheData("gd18", 3));

                            lsTmp.Add("C相电压最大值：" + GetTheData("gd7", 2));
                            lsTmp.Add("C相电压最大值发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("C相电压最小值：" + GetTheData("gd7", 2));
                            lsTmp.Add("C相电压最小值发生时间：" + GetTheData("gd18", 3));

                            lsTmp.Add("A相平均电压：" + GetTheData("gd7", 2));
                            lsTmp.Add("B相平均电压：" + GetTheData("gd7", 2));
                            lsTmp.Add("C相平均电压：" + GetTheData("gd7", 2));

                            lsTmp.Add("A相电压越上限率：" + GetTheData("gd25", 3));
                            lsTmp.Add("A相电压越下限率：" + GetTheData("gd25", 3));
                            lsTmp.Add("A相电压合格率：" + GetTheData("gd25", 3));

                            lsTmp.Add("B相电压越上限率：" + GetTheData("gd25", 3));
                            lsTmp.Add("B相电压越下限率：" + GetTheData("gd25", 3));
                            lsTmp.Add("B相电压合格率：" + GetTheData("gd25", 3));

                            lsTmp.Add("C相电压越上限率：" + GetTheData("gd25", 3));
                            lsTmp.Add("C相电压越下限率：" + GetTheData("gd25", 3));
                            lsTmp.Add("C相电压合格率：" + GetTheData("gd25", 3));
                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 28:
                        lsTmp.Add("Fn = " + Fn + ",日冻结日不平衡度越限累计时间：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("电流不平衡度越限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("电压不平衡度越限日累计时间 ：" + GetTheData("ExplanBinFun", 2));

                            lsTmp.Add("电流不平衡最大值：" + GetTheData("gd5", 2));
                            lsTmp.Add("电流不平衡最大值发生时间 ：" + GetTheData("gd18", 3));

                            lsTmp.Add("电压不平衡最大值：" + GetTheData("gd5", 2));
                            lsTmp.Add("电压不平衡最大值发生时间 ：" + GetTheData("gd18", 3));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 29:
                        lsTmp.Add("Fn = " + Fn + ",日冻结日电流越限数据：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("A相电流越上上限累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("A相电流越上限累计时间：" + GetTheData("ExplanBinFun", 2));

                            lsTmp.Add("B相电流越上上限累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("B相电流越上限累计时间：" + GetTheData("ExplanBinFun", 2));

                            lsTmp.Add("C相电流越上上限累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("C相电流越上限累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("零序电流越上限累计时间：" + GetTheData("ExplanBinFun", 2));

                            lsTmp.Add("A相电流最大值：" + GetTheData("gd25", 3));
                            lsTmp.Add("A相电流最大值发生时间：" + GetTheData("gd18", 3));

                            lsTmp.Add("B相电流最大值：" + GetTheData("gd25", 3));
                            lsTmp.Add("B相电流最大值发生时间：" + GetTheData("gd18", 3));

                            lsTmp.Add("C相电流最大值：" + GetTheData("gd25", 3));
                            lsTmp.Add("C相电流最大值发生时间：" + GetTheData("gd18", 3));

                            lsTmp.Add("零序电流最大值：" + GetTheData("gd25", 3));
                            lsTmp.Add("零序电流最大值发生时间：" + GetTheData("gd18", 3));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 30:
                        lsTmp.Add("Fn = " + Fn + ",日冻结日视在功率越限累计时间：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("视在功率越上上限累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("视在功率越上限累计时间：" + GetTheData("ExplanBinFun", 2));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 31:
                        lsTmp.Add("Fn = " + Fn + ",日负载率统计：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("负载率最大值：" + GetTheData("gd5", 2));
                            lsTmp.Add("负载率最大值发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("负载率最小值：" + GetTheData("gd5", 2));
                            lsTmp.Add("负载率最小值发生时间：" + GetTheData("gd18", 3));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 32:
                        lsTmp.Add("Fn = " + Fn + ",日冻结电能表断相数据：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("总断相次数：" + GetTheData("gd10", 3));
                            lsTmp.Add("A相断相次数：" + GetTheData("gd10", 3));
                            lsTmp.Add("B相断相次数：" + GetTheData("gd10", 3));
                            lsTmp.Add("C相断相次数：" + GetTheData("gd10", 3));

                            lsTmp.Add("断相时间累计值   ：" + GetTheData("gd10", 3));
                            lsTmp.Add("A相断相时间累计值：" + GetTheData("gd10", 3));
                            lsTmp.Add("B相断相时间累计值：" + GetTheData("gd10", 3));
                            lsTmp.Add("C相断相时间累计值：" + GetTheData("gd10", 3));

                            lsTmp.Add("最近一次断相起始时刻：" + GetTheData("gd17", 4));
                            lsTmp.Add("A相最近断相起始时刻：" + GetTheData("gd17", 4));
                            lsTmp.Add("B相最近断相起始时刻：" + GetTheData("gd17", 4));
                            lsTmp.Add("C相最近断相起始时刻：" + GetTheData("gd17", 4));

                            lsTmp.Add("最近一次断相结束时刻：" + GetTheData("gd17", 4));
                            lsTmp.Add("A相最近断相结束时刻：" + GetTheData("gd17", 4));
                            lsTmp.Add("B相最近断相结束时刻：" + GetTheData("gd17", 4));
                            lsTmp.Add("C相最近断相结束时刻：" + GetTheData("gd17", 4));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 33:
                        GetTwoData("月冻结月总及分相最大有功功率及发生时间、有功功率为零时间", new string[] {
                            "月冻结类数据时标Td_d","gd21","2",
                            "三相总最大有功功率","gd23","3",
                            "三相总最大有功功率发生时间","gd18","3",
                            "A相最大有功功率","gd23","3",
                            "A相最大有功功率发生时间","gd18","3",
                            "B相最大有功功率","gd23","3",
                            "B相最大有功功率发生时间","gd18","3",
                            "C相最大有功功率","gd23","3",
                            "C相最大有功功率发生时间","gd18","3",
                            "三相总有功功率为零时间","ExplanBinFun","2",
                            "A相有功功率为零时间","ExplanBinFun","2",
                            "B相有功功率为零时间","ExplanBinFun","2",
                            "C相有功功率为零时间","ExplanBinFun","2"});
                        break;
                    case 34:
                        lsTmp.Add("Fn = " + Fn + ",月冻结月总及分相有功最大需量及发生时间：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));

                            lsTmp.Add("三相总有功最大需量：" + GetTheData("gd23", 3));
                            lsTmp.Add("三相总有功最大需量发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("A相有功最大需量：" + GetTheData("gd23", 3));
                            lsTmp.Add("A相有功最大需量发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("B相有功最大需量：" + GetTheData("gd23", 3));
                            lsTmp.Add("B相有功最大需量发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("C相有功最大需量：" + GetTheData("gd23", 3));
                            lsTmp.Add("C相有功最大需量发生时间：" + GetTheData("gd18", 3));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));
                        break;
                    case 35:
                        lsTmp.Add("Fn = " + Fn + ",月冻结月电压统计数据：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));

                            lsTmp.Add("A相电压越上上限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("A相电压越下下限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("A相电压越上限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("A相电压越下限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("A相电压合格日累计时间：" + GetTheData("ExplanBinFun", 2));

                            lsTmp.Add("B相电压越上上限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("B相电压越下下限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("B相电压越上限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("B相电压越下限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("B相电压合格日累计时间：" + GetTheData("ExplanBinFun", 2));

                            lsTmp.Add("C相电压越上上限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("C相电压越下下限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("C相电压越上限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("C相电压越下限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("C相电压合格日累计时间：" + GetTheData("ExplanBinFun", 2));

                            lsTmp.Add("A相电压最大值：" + GetTheData("gd7", 2));
                            lsTmp.Add("A相电压最大值发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("A相电压最小值：" + GetTheData("gd7", 2));
                            lsTmp.Add("A相电压最小值发生时间：" + GetTheData("gd18", 3));

                            lsTmp.Add("B相电压最大值：" + GetTheData("gd7", 2));
                            lsTmp.Add("B相电压最大值发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("B相电压最小值：" + GetTheData("gd7", 2));
                            lsTmp.Add("B相电压最小值发生时间：" + GetTheData("gd18", 3));

                            lsTmp.Add("C相电压最大值：" + GetTheData("gd7", 2));
                            lsTmp.Add("C相电压最大值发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("C相电压最小值：" + GetTheData("gd7", 2));
                            lsTmp.Add("C相电压最小值发生时间：" + GetTheData("gd18", 3));

                            lsTmp.Add("A相平均电压：" + GetTheData("gd7", 2));
                            lsTmp.Add("B相平均电压：" + GetTheData("gd7", 2));
                            lsTmp.Add("C相平均电压：" + GetTheData("gd7", 2));

                            lsTmp.Add("A相电压越上限率：" + GetTheData("gd25", 3));
                            lsTmp.Add("A相电压越下限率：" + GetTheData("gd25", 3));
                            lsTmp.Add("A相电压合格率：" + GetTheData("gd25", 3));

                            lsTmp.Add("B相电压越上限率：" + GetTheData("gd25", 3));
                            lsTmp.Add("B相电压越下限率：" + GetTheData("gd25", 3));
                            lsTmp.Add("B相电压合格率：" + GetTheData("gd25", 3));

                            lsTmp.Add("C相电压越上限率：" + GetTheData("gd25", 3));
                            lsTmp.Add("C相电压越下限率：" + GetTheData("gd25", 3));
                            lsTmp.Add("C相电压合格率：" + GetTheData("gd25", 3));
                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));
                        break;
                    case 36:
                        lsTmp.Add("Fn = " + Fn + ",月冻结月不平衡度越限累计时间：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));

                            lsTmp.Add("电流不平衡度越限日累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("电压不平衡度越限日累计时间 ：" + GetTheData("ExplanBinFun", 2));

                            lsTmp.Add("电流不平衡最大值：" + GetTheData("gd5", 2));
                            lsTmp.Add("电流不平衡最大值发生时间 ：" + GetTheData("gd18", 3));

                            lsTmp.Add("电压不平衡最大值：" + GetTheData("gd5", 2));
                            lsTmp.Add("电压不平衡最大值发生时间 ：" + GetTheData("gd18", 3));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));
                        break;
                    case 37:
                        lsTmp.Add("Fn = " + Fn + ",日冻结日电流越限数据：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));

                            lsTmp.Add("A相电流越上上限累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("A相电流越上限累计时间：" + GetTheData("ExplanBinFun", 2));

                            lsTmp.Add("B相电流越上上限累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("B相电流越上限累计时间：" + GetTheData("ExplanBinFun", 2));

                            lsTmp.Add("C相电流越上上限累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("C相电流越上限累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("零序电流越上限累计时间：" + GetTheData("ExplanBinFun", 2));

                            lsTmp.Add("A相电流最大值：" + GetTheData("gd25", 3));
                            lsTmp.Add("A相电流最大值发生时间：" + GetTheData("gd18", 3));

                            lsTmp.Add("B相电流最大值：" + GetTheData("gd25", 3));
                            lsTmp.Add("B相电流最大值发生时间：" + GetTheData("gd18", 3));

                            lsTmp.Add("C相电流最大值：" + GetTheData("gd25", 3));
                            lsTmp.Add("C相电流最大值发生时间：" + GetTheData("gd18", 3));

                            lsTmp.Add("零序电流最大值：" + GetTheData("gd25", 3));
                            lsTmp.Add("零序电流最大值发生时间：" + GetTheData("gd18", 3));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));
                        break;
                    case 38:
                        lsTmp.Add("Fn = " + Fn + ",月冻结日视在功率越限累计时间：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));

                            lsTmp.Add("视在功率越上上限累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("视在功率越上限累计时间：" + GetTheData("ExplanBinFun", 2));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));
                        break;
                    case 43:
                        lsTmp.Add("Fn = " + Fn + ",日冻结日功率因数区段累计时间：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("区段1累计时间（功率因数＜定值1）：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("区段2累计时间（定值1≤功率因数＜定值2） ：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("区段3累计时间（功率因数≥定值2）  ：" + GetTheData("ExplanBinFun", 2));


                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 44:
                        lsTmp.Add("Fn = " + Fn + ",月冻结日功率因数区段累计时间：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));

                            lsTmp.Add("区段1累计时间（功率因数＜定值1）：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("区段2累计时间（定值1≤功率因数＜定值2） ：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("区段3累计时间（功率因数≥定值2）  ：" + GetTheData("ExplanBinFun", 2));


                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));
                        break;
                    case 49:
                        lsTmp.Add("Fn = " + Fn + ",日冻结终端日供电时间、日复位累计次数：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端日供电时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("终端日复位累计次数：" + GetTheData("ExplanBinFun", 2));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 50:
                        lsTmp.Add("Fn = " + Fn + ",日冻结终端日控制统计数据：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("月电控跳闸日累计次数：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("购电控跳闸日累计次数：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("功控跳闸日累计次数：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("遥控跳闸日累计次数：" + GetTheData("ExplanBinFun", 1));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 51:
                        lsTmp.Add("Fn = " + Fn + ",月冻结终端月供电时间、月复位累计次数：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));

                            lsTmp.Add("终端月供电时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("终端月复位累计次数：" + GetTheData("ExplanBinFun", 2));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));
                        break;
                    case 52:
                        lsTmp.Add("Fn = " + Fn + ",月冻结终端日控制统计数据：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));

                            lsTmp.Add("月电控跳闸月累计次数：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("购电控跳闸月累计次数：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("功控跳闸月累计次数：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("遥控跳闸月累计次数：" + GetTheData("ExplanBinFun", 1));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_d：" + GetTheData("gd21", 2));
                        break;
                    case 53:
                        GetTwoData("终端与主站日通信流量", new string[] {
                            "日冻结类数据时标Td_d","gd20","3",
                            "终端与主站日通信流量","ExplanBinFun","4"});
                        break;
                    case 54:
                        GetTwoData("终端与主站月通信流量", new string[] {
                            "月冻结类数据时标Td_d","gd21","2",
                            "终端与主站月通信流量","ExplanBinFun","4"});
                        break;
                    case 57:
                        GetTwoData("日冻结总加组日最大、最小有功功率及其发生时间,有功功率为零日累计时间", new string[] {
                            "日冻结类数据时标Td_d","gd20","3",
                            "日最大有功功率","gd2","2",
                            "日最大有功功率发生时间","gd18","3",
                            "日最有功小功率","gd2","2",
                            "日最小有功功率发生时间","gd18","3",
                            "有功功率为零日累计时间","ExplanBinFun","2"});
                        break;
                    case 58:
                        lsTmp.Add("Fn = " + Fn + ",日冻结总加组日累计有功电能量：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                            lsTmp.Add("费率数M：" + GetTheData("ExplanBinFun", 1));

                            lsTmp.Add("日累计总有功电能量：" + GetTheData("gd3", 4));
                            lsTmp.Add("费率1日累计有功电能量：" + GetTheData("gd3", 4));
                            lsTmp.Add("费率2日累计有功电能量：" + GetTheData("gd3", 4));
                            lsTmp.Add("费率3日累计有功电能量：" + GetTheData("gd3", 4));
                            lsTmp.Add("费率4日累计有功电能量：" + GetTheData("gd3", 4));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 59:
                        lsTmp.Add("Fn = " + Fn + ",日冻结总加组日累计无功电能量：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                            lsTmp.Add("费率数M：" + GetTheData("ExplanBinFun", 1));

                            lsTmp.Add("日累计总无功电能量：" + GetTheData("gd3", 4));
                            lsTmp.Add("费率1日累计无功电能量：" + GetTheData("gd3", 4));
                            lsTmp.Add("费率2日累计无功电能量：" + GetTheData("gd3", 4));
                            lsTmp.Add("费率3日累计无功电能量：" + GetTheData("gd3", 4));
                            lsTmp.Add("费率4日累计无功电能量：" + GetTheData("gd3", 4));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 60:
                        lsTmp.Add("Fn = " + Fn + ",月冻结总加组月最大、最小有功功率及其发生时间,有功功率为零累计时间：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));

                            lsTmp.Add("月最大有功功率：" + GetTheData("gd2", 2));
                            lsTmp.Add("月最大有功功率发生时间：" + GetTheData("gd18", 3));

                            lsTmp.Add("月最小有功功率：" + GetTheData("gd2", 2));
                            lsTmp.Add("月最小有功功率发生时间：" + GetTheData("gd18", 3));
                            lsTmp.Add("月有功功率为零累计时间：" + GetTheData("ExplanBinFun", 2));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));

                        break;
                    case 65:
                        lsTmp.Add("Fn = " + Fn + ",月冻结总加组超功率定值的月累计时间及月累计电能量：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));

                            lsTmp.Add("超功率定值月累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("超功率定值的月累计电能量：" + GetTheData("gd3", 4));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));

                        break;
                    case 66:
                        lsTmp.Add("Fn = " + Fn + ",月冻结总加组超月电能量定值的月累计时间及月累计电能量：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));

                            lsTmp.Add("超月电能量定值月累计时间：" + GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("超月冻结电能量定值的月累计电能量：" + GetTheData("gd3", 4));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));

                        break;
                    case 73:
                        lsTmp.Add("Fn = " + Fn + ",总加组有功功率曲线：");
                        GetQxData("功率", "gd2", 2);
                        break;
                    case 74:
                        lsTmp.Add("Fn = " + Fn + ",总加组无功功率曲线：");
                        GetQxData("功率", "gd2", 2);
                        break;
                    case 75:
                        lsTmp.Add("Fn = " + Fn + ",总加组有功电能量曲线：");
                        GetQxData("电能量", "gd3", 4);
                        break;
                    case 76:
                        lsTmp.Add("Fn = " + Fn + ",总加组无功电能量曲线：");
                        GetQxData("电能量", "gd3", 4);
                        break;
                    case 81:
                        lsTmp.Add("Fn = " + Fn + ",测量点有功功率曲线：");
                        GetQxData("功率", "gd9", 3);
                        break;
                    case 82:
                        lsTmp.Add("Fn = " + Fn + ",测量点A相有功功率曲线：");
                        GetQxData("功率", "gd9", 3);
                        break;
                    case 83:
                        lsTmp.Add("Fn = " + Fn + ",测量点B相有功功率曲线：");
                        GetQxData("功率", "gd9", 3);
                        break;
                    case 84:
                        lsTmp.Add("Fn = " + Fn + ",测量点C相有功功率曲线：");
                        GetQxData("功率", "gd9", 3);
                        break;
                    case 85:
                        lsTmp.Add("Fn = " + Fn + ",测量点无功功率曲线：");
                        GetQxData("功率", "gd9", 3);
                        break;
                    case 86:
                        lsTmp.Add("Fn = " + Fn + ",测量点A相无功功率曲线：");
                        GetQxData("功率", "gd9", 3);
                        break;
                    case 87:
                        lsTmp.Add("Fn = " + Fn + ",测量点B相无功功率曲线：");
                        GetQxData("功率", "gd9", 3);
                        break;
                    case 88:
                        lsTmp.Add("Fn = " + Fn + ",测量点C相无功功率曲线：");
                        GetQxData("功率", "gd9", 3);
                        break;
                    case 89:
                        lsTmp.Add("Fn = " + Fn + ",测量点A相电压曲线：");
                        GetQxData("电压", "gd7", 2);
                        break;
                    case 90:
                        lsTmp.Add("Fn = " + Fn + ",测量点B相电压曲线：");
                        GetQxData("电压", "gd7", 2);
                        break;
                    case 91:
                        lsTmp.Add("Fn = " + Fn + ",测量点C相电压曲线：");
                        GetQxData("电压", "gd7", 2);
                        break;
                    case 92:
                        lsTmp.Add("Fn = " + Fn + ",测量点A相电流曲线：");
                        GetQxData("电流", "gd25", 3);
                        break;
                    case 93:
                        lsTmp.Add("Fn = " + Fn + ",测量点B相电流曲线：");
                        GetQxData("电流", "gd25", 3);
                        break;
                    case 94:
                        lsTmp.Add("Fn = " + Fn + ",测量点C相电流曲线：");
                        GetQxData("电流", "gd25", 3);
                        break;
                    case 95:
                        lsTmp.Add("Fn = " + Fn + ",测量点零序电流曲线：");
                        GetQxData("电流", "gd25", 3);
                        break;
                    case 96:
                        lsTmp.Add("Fn = " + Fn + ",断相统计曲线：");
                        if (BoolUp)
                        {
                            lsTmp.Add("曲线类数据时标Td_m：" + GetTheData("gd15", 5));
                            lsTmp.Add("冻结密度：" + GetTheData("getdjmd", 1));
                            int n = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("数据点个数：" + n);
                            for (int i = 1; i <= n; i++)
                            {
                                lsTmp.Add("总掉电次数" + i + "：" + GetTheData("gd10", 3));
                                lsTmp.Add("掉电时间累计值" + i + "：" + GetTheData("gd27", 4));
                            }

                        }
                        else
                        {
                            lsTmp.Add("曲线类数据时标Td_m：" + GetTheData("gd15", 5));
                            lsTmp.Add("冻结密度：" + GetTheData("getdjmd", 1));
                            int n = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("数据点个数：" + n);
                        }
                        break;
                    case 97:
                        lsTmp.Add("Fn = " + Fn + ",测量点正向有功总电能量曲线：");
                        GetQxData("电能量", "gd13", 4);
                        break;
                    case 98:
                        lsTmp.Add("Fn = " + Fn + ",测量点正向无功总电能量曲线：");
                        GetQxData("电能量", "gd13", 4);
                        break;
                    case 99:
                        lsTmp.Add("Fn = " + Fn + ",测量点反向有功总电能量曲线：");
                        GetQxData("电能量", "gd13", 4);
                        break;
                    case 100:
                        lsTmp.Add("Fn = " + Fn + ",测量点反向无功总电能量曲线：");
                        GetQxData("电能量", "gd13", 4);

                        break;
                    case 101:
                        lsTmp.Add("Fn = " + Fn + ",测量点正向有功总电能示值曲线：");
                        GetQxData("电能示值", "gd11", 4);
                        break;
                    case 102:
                        lsTmp.Add("Fn = " + Fn + ",测量点正向无功总电能示值曲线：");
                        GetQxData("电能示值", "gd11", 4);
                        break;
                    case 103:
                        lsTmp.Add("Fn = " + Fn + ",测量点反向有功总电能示值曲线：");
                        GetQxData("电能示值", "gd11", 4);
                        break;
                    case 104:
                        lsTmp.Add("Fn = " + Fn + ",测量点反向无功总电能示值曲线：");
                        GetQxData("功率因数", "gd11", 4);
                        break;
                    case 105:
                        lsTmp.Add("Fn = " + Fn + ",测量点功率因数曲线：");
                        GetQxData("功率因数", "gd5", 2);
                        break;
                    case 106:
                        lsTmp.Add("Fn = " + Fn + ",测量点A相功率因数曲线：");
                        GetQxData("功率因数", "gd5", 2);
                        break;
                    case 107:
                        lsTmp.Add("Fn = " + Fn + ",测量点B相功率因数曲线：");
                        GetQxData("功率因数", "gd5", 2);
                        break;
                    case 108:
                        lsTmp.Add("Fn = " + Fn + ",测量点C相功率因数曲线：");
                        GetQxData("功率因数", "gd5", 2);
                        break;
                    case 161:
                        lsTmp.Add("Fn = " + Fn + ",日冻结正向有功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M)：" + GetTheData("ExplanNo", 1));

                            lsTmp.Add("正向有功总电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率1正向有功总电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率2正向有功总电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率3正向有功总电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率4正向有功总电能示值：" + GetTheData("gd14", 5));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 162:
                        lsTmp.Add("Fn = " + Fn + ",日冻结正向无功（组合无功1）电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M)：" + GetTheData("ExplanNo", 1));

                            lsTmp.Add("正向无功（组合无功1）总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率1正向无功（组合无功1）总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率2正向无功（组合无功1）正向有功总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率3正向无功（组合无功1）总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率4正向无功（组合无功1）总电能示值：" + GetTheData("gd11", 4));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 163:
                        lsTmp.Add("Fn = " + Fn + ",日冻结反向有功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M)：" + GetTheData("ExplanNo", 1));

                            lsTmp.Add("正向有功总电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率1反向有功总电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率2反向有功总电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率3反向有功总电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率4反向有功总电能示值：" + GetTheData("gd14", 5));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 164:
                        lsTmp.Add("Fn = " + Fn + ",日冻结反向无功（组合无功2）电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M)：" + GetTheData("ExplanNo", 1));

                            lsTmp.Add("反向无功（组合无功1）总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率1反向无功（组合无功2）总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率2反向无功（组合无功2）总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率3反向无功（组合无功2）总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率4反向无功（组合无功2）总电能示值：" + GetTheData("gd11", 4));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 165:
                        lsTmp.Add("Fn = " + Fn + ",日冻结一象限无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M)：" + GetTheData("ExplanNo", 1));

                            lsTmp.Add("一象限无功总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率1一象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率2一象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率3一象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率4一象限无功电能示值：" + GetTheData("gd11", 4));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 166:
                        lsTmp.Add("Fn = " + Fn + ",日冻结二象限无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M)：" + GetTheData("ExplanNo", 1));

                            lsTmp.Add("二象限无功总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率1二象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率2二象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率3二象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率4二象限无功电能示值：" + GetTheData("gd11", 4));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 167:
                        lsTmp.Add("Fn = " + Fn + ",日冻结三象限无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M)：" + GetTheData("ExplanNo", 1));

                            lsTmp.Add("三象限无功总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率1三象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率2三象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率3三象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率4三象限无功电能示值：" + GetTheData("gd11", 4));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 168:
                        lsTmp.Add("Fn = " + Fn + ",日冻结四象限无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M)：" + GetTheData("ExplanNo", 1));

                            lsTmp.Add("四象限无功总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率1四象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率2四象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率3四象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率4四象限无功电能示值：" + GetTheData("gd11", 4));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 169:
                        lsTmp.Add("Fn = " + Fn + ",抄表日冻结正向有功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M)：" + GetTheData("ExplanNo", 1));

                            lsTmp.Add("正向有功总电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率1反向有功总电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率2反向有功总电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率3反向有功总电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率4反向有功总电能示值：" + GetTheData("gd14", 5));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 170:
                        lsTmp.Add("Fn = " + Fn + ",抄表日冻结正向无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M)：" + GetTheData("ExplanNo", 1));

                            lsTmp.Add("正向无功（组合无功1）总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率1正向无功（组合无功1）总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率2正向无功（组合无功1）正向有功总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率3正向无功（组合无功1）总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率4正向无功（组合无功1）总电能示值：" + GetTheData("gd11", 4));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 173:
                        lsTmp.Add("Fn = " + Fn + ",抄表日冻结一象限无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("抄表日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M)：" + GetTheData("ExplanNo", 1));

                            lsTmp.Add("一象限无功总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率1一象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率2一象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率3一象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率4一象限无功电能示值：" + GetTheData("gd11", 4));

                        }
                        else
                            lsTmp.Add("抄表日日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 174:
                        lsTmp.Add("Fn = " + Fn + ",抄表日冻结二象限无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("抄表日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M)：" + GetTheData("ExplanNo", 1));

                            lsTmp.Add("二象限无功总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率1二象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率2二象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率3二象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率4二象限无功电能示值：" + GetTheData("gd11", 4));

                        }
                        else
                            lsTmp.Add("抄表日日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 175:
                        lsTmp.Add("Fn = " + Fn + ",抄表日冻结三象限无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("抄表日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M)：" + GetTheData("ExplanNo", 1));

                            lsTmp.Add("三象限无功总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率1三象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率2三象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率3三象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率4三象限无功电能示值：" + GetTheData("gd11", 4));

                        }
                        else
                            lsTmp.Add("抄表日日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 176:
                        lsTmp.Add("Fn = " + Fn + ",抄表日冻结四象限无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("抄表日冻结类数据时标Td_d：" + GetTheData("gd20", 3));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M)：" + GetTheData("ExplanNo", 1));

                            lsTmp.Add("四象限无功总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率1四象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率2四象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率3四象限无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率4四象限无功电能示值：" + GetTheData("gd11", 4));

                        }
                        else
                            lsTmp.Add("抄表日日冻结类数据时标Td_d：" + GetTheData("gd20", 3));
                        break;
                    case 177:
                        lsTmp.Add("Fn = " + Fn + ",月冻结正向有功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M：" + GetTheData("ExplanBinFun", 1));

                            lsTmp.Add("正向有功总电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率1正向有功电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率2正向有功电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率3正向有功电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率4正向有功电能示值：" + GetTheData("gd14", 5));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));
                        break;
                    case 178:
                        lsTmp.Add("Fn = " + Fn + ",月冻结正向无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M：" + GetTheData("ExplanBinFun", 1));

                            lsTmp.Add("正向无功总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率1正向无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率2正向无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率3正向无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率4正向无功电能示值：" + GetTheData("gd11", 4));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));
                        break;
                    case 179:
                        lsTmp.Add("Fn = " + Fn + ",月冻结反向有功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M：" + GetTheData("ExplanBinFun", 1));

                            lsTmp.Add("反向有功总电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率1反向有功电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率2反向有功电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率3反向有功电能示值：" + GetTheData("gd14", 5));
                            lsTmp.Add("费率4反向有功电能示值：" + GetTheData("gd14", 5));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));
                        break;
                    case 180:
                        lsTmp.Add("Fn = " + Fn + ",月冻结反向无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));
                            lsTmp.Add("费率数M：" + GetTheData("ExplanBinFun", 1));

                            lsTmp.Add("反向无功总电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率1反向无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率2反向无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率3反向无功电能示值：" + GetTheData("gd11", 4));
                            lsTmp.Add("费率4反向无功电能示值：" + GetTheData("gd11", 4));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));//(8, 4)));
                        break;
                    case 181:
                        lsTmp.Add("Fn = " + Fn + ",月冻结一象限无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));//(8, 4)));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));//(12, 10)));
                            lsTmp.Add("费率数M：" + GetTheData("ExplanBinFun", 1));//(22, 2)));

                            lsTmp.Add("一象限无功总电能示值：" + GetTheData("gd11", 4));//(24, 8)) + "KWh");
                            lsTmp.Add("费率1一象限无功电能示值：" + GetTheData("gd11", 4));//(32, 8)) + "KWh");
                            lsTmp.Add("费率2一象限无功电能示值：" + GetTheData("gd11", 4));//(40, 8)) + "KWh");
                            lsTmp.Add("费率3一象限无功电能示值：" + GetTheData("gd11", 4));//(48, 8)) + "KWh");
                            lsTmp.Add("费率4一象限无功电能示值：" + GetTheData("gd11", 4));//(56, 8)) + "KWh");

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));//(8, 4)));
                        break;
                    case 182:
                        lsTmp.Add("Fn = " + Fn + ",月冻结二象限无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));//(8, 4)));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));//(12, 10)));
                            lsTmp.Add("费率数M：" + GetTheData("ExplanBinFun", 1));//(22, 2)));

                            lsTmp.Add("二象限无功总电能示值：" + GetTheData("gd11", 4));//(24, 8)) + "KWh");
                            lsTmp.Add("费率1二象限无功电能示值：" + GetTheData("gd11", 4));//(32, 8)) + "KWh");
                            lsTmp.Add("费率2二象限无功电能示值：" + GetTheData("gd11", 4));//(40, 8)) + "KWh");
                            lsTmp.Add("费率3二象限无功电能示值：" + GetTheData("gd11", 4));//(48, 8)) + "KWh");
                            lsTmp.Add("费率4二象限无功电能示值：" + GetTheData("gd11", 4));//(56, 8)) + "KWh");

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));//(8, 4)));
                        break;
                    case 183:
                        lsTmp.Add("Fn = " + Fn + ",月冻结三象限无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));//(8, 4)));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));//(12, 10)));
                            lsTmp.Add("费率数M：" + GetTheData("ExplanBinFun", 1));//(22, 2)));

                            lsTmp.Add("三象限无功总电能示值：" + GetTheData("gd11", 4));//(24, 8)) + "KWh");
                            lsTmp.Add("费率1三象限无功电能示值：" + GetTheData("gd11", 4));//(32, 8)) + "KWh");
                            lsTmp.Add("费率2三象限无功电能示值：" + GetTheData("gd11", 4));//(40, 8)) + "KWh");
                            lsTmp.Add("费率3三象限无功电能示值：" + GetTheData("gd11", 4));//(48, 8)) + "KWh");
                            lsTmp.Add("费率4三象限无功电能示值：" + GetTheData("gd11", 4));//(56, 8)) + "KWh");

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));//(8, 4)));
                        break;
                    case 184:
                        lsTmp.Add("Fn = " + Fn + ",月冻结四象限无功电能示值：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));//(8, 4)));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));//(12, 10)));
                            lsTmp.Add("费率数M：" + GetTheData("ExplanBinFun", 1));//(22, 2)));

                            lsTmp.Add("四象限无功总电能示值：" + GetTheData("gd11", 4));//(24, 8)) + "KWh");
                            lsTmp.Add("费率1四象限无功电能示值：" + GetTheData("gd11", 4));//(32, 8)) + "KWh");
                            lsTmp.Add("费率1四象限无功电能示值：" + GetTheData("gd11", 4));//(40, 8)) + "KWh");
                            lsTmp.Add("费率1四象限无功电能示值：" + GetTheData("gd11", 4));//(48, 8)) + "KWh");
                            lsTmp.Add("费率1四象限无功电能示值：" + GetTheData("gd11", 4));//(56, 8)) + "KWh");

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));//(8, 4)));
                        break;
                    case 185:
                        lsTmp.Add("Fn = " + Fn + ",日冻结正向有功最大需量及发生时间：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_m：" + GetTheData("gd20", 3));//(8, 6)));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));//(14, 10)));
                            lsTmp.Add("费率数M：" + GetTheData("ExplanBinFun", 1));//(24, 2)));

                            lsTmp.Add("正向有功总最大需量：" + GetTheData("gd23", 3));//(26, 6)) + "KWh");
                            lsTmp.Add("正向有功总最大需量发生时间：" + GetTheData("gd17", 4));//(32, 8)));
                            lsTmp.Add("费率1正向有功最大需量：" + GetTheData("gd23", 3));//(40, 6)) + "KWh");
                            lsTmp.Add("费率1正向有功最大需量发生时间：" + GetTheData("gd17", 4));//(46, 8)));
                            lsTmp.Add("费率2正向有功最大需量：" + GetTheData("gd23", 3));//(54, 6)) + "KWh");
                            lsTmp.Add("费率2正向有功最大需量发生时间：" + GetTheData("gd17", 4));//(60, 8)));
                            lsTmp.Add("费率3正向有功最大需量：" + GetTheData("gd23", 3));//(68, 6)) + "KWh");
                            lsTmp.Add("费率3正向有功最大需量发生时间：" + GetTheData("gd17", 4));//(74, 8)));
                            lsTmp.Add("费率4正向有功最大需量：" + GetTheData("gd23", 3));//(82, 6)) + "KWh");
                            lsTmp.Add("费率4正向有功最大需量发生时间：" + GetTheData("gd17", 4));//(88, 8)));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_m：" + GetTheData("gd20", 3));//(8, 6)));
                        break;
                    case 189:
                        lsTmp.Add("Fn = " + Fn + ",抄表日冻结正向有功最大需量及发生时间：");
                        if (BoolUp)
                        {
                            lsTmp.Add("日冻结类数据时标Td_m：" + GetTheData("gd20", 3));//(8, 6)));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));//(14, 10)));
                            lsTmp.Add("费率数M：" + GetTheData("ExplanBinFun", 1));//(24, 2)));

                            lsTmp.Add("正向有功总最大需量：" + GetTheData("gd23", 3));//(26, 6)) + "KWh");
                            lsTmp.Add("正向有功总最大需量发生时间：" + GetTheData("gd17", 4));//(32, 8)));
                            lsTmp.Add("费率1正向有功最大需量：" + GetTheData("gd23", 3));//(40, 6)) + "KWh");
                            lsTmp.Add("费率1正向有功最大需量发生时间：" + GetTheData("gd17", 4));//(46, 8)));
                            lsTmp.Add("费率2正向有功最大需量：" + GetTheData("gd23", 3));//(54, 6)) + "KWh");
                            lsTmp.Add("费率2正向有功最大需量发生时间：" + GetTheData("gd17", 4));//(60, 8)));
                            lsTmp.Add("费率3正向有功最大需量：" + GetTheData("gd23", 3));//(68, 6)) + "KWh");
                            lsTmp.Add("费率3正向有功最大需量发生时间：" + GetTheData("gd17", 4));//(74, 8)));
                            lsTmp.Add("费率4正向有功最大需量：" + GetTheData("gd23", 3));//(82, 6)) + "KWh");
                            lsTmp.Add("费率4正向有功最大需量发生时间：" + GetTheData("gd17", 4));//(88, 8)));

                        }
                        else
                            lsTmp.Add("日冻结类数据时标Td_m：" + GetTheData("gd20", 3));//(8, 6)));
                        break;
                    case 193:
                        lsTmp.Add("Fn = " + Fn + ",月冻结正向有功最大需量及发生时间：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));//(8, 4)));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));//(14 - 2, 10)));
                            lsTmp.Add("费率数M：" + GetTheData("ExplanBinFun", 1));//(24 - 2, 2)));

                            lsTmp.Add("正向有功总最大需量：" + GetTheData("gd23", 3));//(26 - 2, 6)) + "KWh");
                            lsTmp.Add("正向有功总最大需量发生时间：" + GetTheData("gd17", 4));//(32 - 2, 8)));
                            lsTmp.Add("费率1正向有功最大需量：" + GetTheData("gd23", 3));//(40 - 2, 6)) + "KWh");
                            lsTmp.Add("费率1正向有功最大需量发生时间：" + GetTheData("gd17", 4));//(46 - 2, 8)));
                            lsTmp.Add("费率2正向有功最大需量：" + GetTheData("gd23", 3));//(54 - 2, 6)) + "KWh");
                            lsTmp.Add("费率2正向有功最大需量发生时间：" + GetTheData("gd17", 4));//(60 - 2, 8)));
                            lsTmp.Add("费率3正向有功最大需量：" + GetTheData("gd23", 3));//(68 - 2, 6)) + "KWh");
                            lsTmp.Add("费率3正向有功最大需量发生时间：" + GetTheData("gd17", 4));//(74 - 2, 8)));
                            lsTmp.Add("费率4正向有功最大需量：" + GetTheData("gd23", 3));//(82 - 2, 6)) + "KWh");
                            lsTmp.Add("费率4正向有功最大需量发生时间：" + GetTheData("gd17", 4));//(88 - 2, 8)));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));//(8, 4)));
                        break;
                    case 194:
                        lsTmp.Add("Fn = " + Fn + ",月冻结正向无功最大需量及发生时间：");
                        if (BoolUp)
                        {
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));//(8, 4)));

                            lsTmp.Add("终端抄表时间：" + GetTheData("gd15", 5));//(14 - 2, 10)));
                            lsTmp.Add("费率数M：" + GetTheData("ExplanBinFun", 1));//(24 - 2, 2)));

                            lsTmp.Add("正向无功总最大需量：" + GetTheData("gd23", 3));//(26 - 2, 6)) + "KWh");
                            lsTmp.Add("正向无功总最大需量发生时间：" + GetTheData("gd17", 4));//(32 - 2, 8)));
                            lsTmp.Add("费率1正向无功最大需量：" + GetTheData("gd23", 3));//(40 - 2, 6)) + "KWh");
                            lsTmp.Add("费率1正向无功最大需量发生时间：" + GetTheData("gd17", 4));//(46 - 2, 8)));
                            lsTmp.Add("费率2正向无功最大需量：" + GetTheData("gd23", 3));//(54 - 2, 6)) + "KWh");
                            lsTmp.Add("费率2正向无功最大需量发生时间：" + GetTheData("gd17", 4));//(60 - 2, 8)));
                            lsTmp.Add("费率3正向无功最大需量：" + GetTheData("gd23", 3));//(68 - 2, 6)) + "KWh");
                            lsTmp.Add("费率3正向无功最大需量发生时间：" + GetTheData("gd17", 4));//(74 - 2, 8)));
                            lsTmp.Add("费率4正向无功最大需量：" + GetTheData("gd23", 3));//(82 - 2, 6)) + "KWh");
                            lsTmp.Add("费率4正向无功最大需量发生时间：" + GetTheData("gd17", 4));//(88 - 2, 8)));

                        }
                        else
                            lsTmp.Add("月冻结类数据时标Td_m：" + GetTheData("gd21", 2));//(8, 4)));
                        break;
                    case 225:
                        lsTmp.Add("Fn = " + Fn + ",电压合格率统计曲线：");
                        if (BoolUp)
                        {
                            int n = Convert.ToInt16(GetTheData("ExplanBinFun", 1));//(20, 2));
                            lsTmp.Add("曲线类数据时标Td_m：" + GetTheData("gd15", 5));//(8, 10)) + " " + GetTheData("getdjmd", 1));//Convert.ToInt16(strFrameRemove.Substring(18, 2), 16)) + " " + "数据点数n：" + n);
                            for (int i = 1; i <= n; i++)
                            {
                                lsTmp.Add("电压监测时间" + i + "：" + GetTheData("gd10", 3));//(-32 + i * 54, 6)));
                                lsTmp.Add("电压合格率" + i + "：" + GetTheData("gd28", 3));//(-26 + i * 54, 6)));
                                lsTmp.Add("电压超限率" + i + "：" + GetTheData("gd28", 3));//(-20 + i * 54, 6)));
                                lsTmp.Add("电压超上限时间" + i + "：" + GetTheData("gd10", 3));//(-14 + i * 54, 6)));
                                lsTmp.Add("电压超下限时间" + i + "：" + GetTheData("gd10", 3));//(-8 + i * 54, 6)));
                                lsTmp.Add("最高电压" + i + "：" + GetTheData("gd5", 2));//(-2 + i * 54, 4)));
                                lsTmp.Add("最高电压出现时间" + i + "：" + GetTheData("gd17", 4));//(2 + i * 54, 8)));
                                lsTmp.Add("最低电压" + i + "：" + GetTheData("gd5", 2));//(10 + i * 54, 4)));
                                lsTmp.Add("最低电压出现时间" + i + "：" + GetTheData("gd17", 4));//(14 + i * 54, 8)));
                            }

                        }
                        else
                        {
                            int n = Convert.ToInt16(GetTheData("ExplanBinFun", 1));//(20, 2));
                            lsTmp.Add("曲线类数据时标Td_m：" + GetTheData("gd15", 5));//(8, 10)) + " " + GetTheData("getdjmd", 1));//Convert.ToInt16(strFrameRemove.Substring(18, 2), 16)) + " " + "数据点数n：" + n);
                        }
                        break;
                    default:
                        lsTmp.Add("Fn = " + Fn + ",ERR"); break;
                }
                if (strFrameRemove.Length > 0)
                    a0D();
            }
            catch
            {

            }
        }
        public void a0E()
        {

            try
            {
                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    case 1:
                        lsTmp.Add("Fn = " + Fn + ",请求重要事件：");
                        if (BoolUp)
                        {
                            lsTmp.Add("当前重要事件计数器EC1：" + GetTheData("ExplanBinFun", 1));//(8, 2)));
                            lsTmp.Add("当前一般事件计数器EC2：" + GetTheData("ExplanBinFun", 1));//(10, 2)));
                            lsTmp.Add("本帧报文传送的事件记录起始指针Pm：" + GetTheData("ExplanBinFun", 1));//(12, 2)));
                            lsTmp.Add("本帧报文传送的事件记录结束指针Pn：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                            ExplanErc();
                        }
                        else
                        {
                            lsTmp.Add("请求事件记录起始指针Pm：" + GetTheData("ExplanBinFun", 1));//(8, 2)));
                            lsTmp.Add("请求事件记录结束指针Pn：" + GetTheData("ExplanBinFun", 1));//(10, 2)));
                        }
                        break;
                    case 2:
                        lsTmp.Add("Fn = " + Fn + ",请求一般事件：");
                        if (BoolUp)
                        {
                            lsTmp.Add("当前重要事件计数器EC1：" + GetTheData("ExplanBinFun", 1));//(8, 2)));
                            lsTmp.Add("当前一般事件计数器EC2：" + GetTheData("ExplanBinFun", 1));//(10, 2)));
                            lsTmp.Add("本帧报文传送的事件记录起始指针Pm：" + GetTheData("ExplanBinFun", 1));//(12, 2)));
                            lsTmp.Add("本帧报文传送的事件记录结束指针Pn：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                            ExplanErc();
                        }
                        else
                        {
                            lsTmp.Add("请求事件记录起始指针Pm：" + GetTheData("ExplanBinFun", 1));//(8, 2)));
                            lsTmp.Add("请求事件记录结束指针Pn：" + GetTheData("ExplanBinFun", 1));//(10, 2)));
                        }
                        break;
                    case 3:
                        lsTmp.Add("Fn = " + Fn + ",指定事件编号下的事件：");
                        if (BoolUp)
                        {
                            int ins = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("事件个数：" + ins);
                            for (int i = 1; i <= ins; i++)
                            {
                                ExplanErc();
                            }

                        }
                        else
                        {
                            lsTmp.Add("当前事件编号：" + GetTheData("ExplanBinFun", 2));
                        }
                        break;
                    default:
                        lsTmp.Add("ERR"); break;
                }

            }
            catch
            {

            }
        }

        public void ExplanErc()
        {
            int sErc = 0; string str1 = ""; int ibg = 0;
            try
            {
                sErc = Convert.ToInt16(GetTheData("ExplanBinFun", 1));//(0, 2));
                switch (sErc)
                {
                    case 1:
                        lsTmp.Add("数据初始化和版本变更记录：ERC1");
                        lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        ibg = Convert.ToInt16(GetTheData("ExplanNo", 1), 16);
                        if (Comm.GetAnyValue(ibg, 0, 0) == 1)
                            str1 += "终端进行参数及数据区初始化";
                        if (Comm.GetAnyValue(ibg, 1, 1) == 1)
                            str1 += "终端版本变更";
                        lsTmp.Add("事件标志：" + str1);
                        lsTmp.Add("变更前软件版本号：" + GetTheData("ReAscill", 4));//(16, 8)));
                        lsTmp.Add("变更后软件版本号：" + GetTheData("ReAscill", 4));//(24, 8)));
                        break;
                    case 2:
                        lsTmp.Add("参数丢失记录：ERC2"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("参数更新时间:分时日月年：" + GetTheData("gd15", 5));//(4, 10)));
                        ibg = Convert.ToInt16(GetTheData("ExplanNo", 1), 16);
                        if (Comm.GetAnyValue(ibg, 0, 0) == 1)
                            str1 += "终端参数丢失";
                        if (Comm.GetAnyValue(ibg, 1, 1) == 1)
                            str1 += "测量点参数丢失";
                        lsTmp.Add("事件标志：" + str1);
                        break;
                    case 3:
                        lsTmp.Add("参数变更记录：ERC3"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));
                        lsTmp.Add("启动站地址：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("变更参数数据单元标识：" + GetTheData("ExplanNo", 4));
                        break;
                    case 4:
                        lsTmp.Add("状态量变位记录：ERC4"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("状态变位：" + GetTheData("ExplanWei", 1));//(strFrameRemove.Substring(14, 2)));
                        lsTmp.Add("变位后状态：" + GetTheData("ExplanWei", 1));//(strFrameRemove.Substring(16, 2)));
                        break;
                    case 5:
                        lsTmp.Add("遥控跳闸记录：ERC5"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("跳闸轮次：" + GetTheData("ExplanWei", 1));//(strFrameRemove.Substring(14, 2)));
                        lsTmp.Add("跳闸时功率（总加功率）：" + GetTheData("gd2", 2));//(16, 4)));
                        lsTmp.Add("跳闸后2分钟的功率（总加功率）：" + GetTheData("gd2", 2));//(20, 4)));
                        break;
                    case 6:
                        lsTmp.Add("控跳闸记录：ERC6"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("总加组号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("跳闸轮次：" + GetTheData("ExplanWei", 1));//(strFrameRemove.Substring(16, 2)));

                        ibg = Convert.ToInt16(GetTheData("ExplanNo", 1), 16);
                        if (Comm.GetAnyValue(ibg, 0, 0) == 1)
                            str1 += "时段控";
                        if (Comm.GetAnyValue(ibg, 1, 1) == 1)
                            str1 += "厂休控";
                        if (Comm.GetAnyValue(ibg, 2, 2) == 1)
                            str1 += "营业报停控";
                        if (Comm.GetAnyValue(ibg, 3, 3) == 1)
                            str1 += "当前功率下浮控";

                        lsTmp.Add("功控类别：" + str1);
                        lsTmp.Add("跳闸前功率（总加功率）：" + GetTheData("gd2", 2));//(20, 4)));
                        lsTmp.Add("跳闸后2分钟的功率（总加功率）：" + GetTheData("gd2", 2));//(24, 4)));
                        lsTmp.Add("跳闸时功率定值：" + GetTheData("gd2", 2));//(28, 4)));
                        break;
                    case 7:
                        lsTmp.Add("电控跳闸记录：ERC7"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("总加组号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));

                        lsTmp.Add("跳闸轮次：" + GetTheData("ExplanWei", 1));//(strFrameRemove.Substring(16, 2)));

                        ibg = Convert.ToInt16(GetTheData("ExplanNo", 1), 16);
                        if (Comm.GetAnyValue(ibg, 0, 0) == 1)
                            str1 += "月电控";
                        if (Comm.GetAnyValue(ibg, 1, 1) == 1)
                            str1 += "购电控";

                        lsTmp.Add("电控类别：" + str1);

                        lsTmp.Add("跳闸前功率（总加功率）：" + GetTheData("gd3", 4));//(20, 8)));
                        lsTmp.Add("跳闸后2分钟的功率（总加功率）：" + GetTheData("gd3", 4));//(28, 8)));
                        break;
                    case 8:
                        lsTmp.Add("电能表参数变更事件：ERC8"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        GetTheData("ExplanNo", 1);
                        ibg = Convert.ToInt16(GetTheData("ExplanNo", 1), 16);
                        if (Comm.GetAnyValue(ibg, 0, 0) == 1)
                            str1 += "电能表费率时段变化";
                        if (Comm.GetAnyValue(ibg, 1, 1) == 1)
                            str1 += "电能表编程时间更改";
                        if (Comm.GetAnyValue(ibg, 2, 2) == 1)
                            str1 += "电能表抄表日更改";
                        if (Comm.GetAnyValue(ibg, 3, 3) == 1)
                            str1 += "电能表脉冲常数更改";
                        if (Comm.GetAnyValue(ibg, 4, 4) == 1)
                            str1 += "电能表的互感器倍率更改";
                        if (Comm.GetAnyValue(ibg, 5, 5) == 1)
                            str1 += "电能表最大需量清零";
                        lsTmp.Add("变更标志：" + str1);
                        break;
                    case 9:
                        lsTmp.Add("电流回路异常：ERC9"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));
                        ibg = Convert.ToInt16(GetTheData("ExplanNo", 1), 16);
                        if (Comm.GetAnyValue(ibg, 0, 0) == 1)
                            str1 += "A相";
                        if (Comm.GetAnyValue(ibg, 1, 1) == 1)
                            str1 += "B相";
                        if (Comm.GetAnyValue(ibg, 2, 2) == 1)
                            str1 += "C相";
                        switch (Comm.GetAnyValue(ibg, 6, 7))
                        {
                            case 1:
                                str1 += "短路"; break;
                            case 2:
                                str1 += "开路"; break;
                            case 3:
                                str1 += "反向"; break;
                        }

                        lsTmp.Add("异常标志：" + str1);
                        lsTmp.Add("发生时的Ua/Uab：" + GetTheData("gd7", 2));//(20, 4)));
                        lsTmp.Add("发生时的Ub：" + GetTheData("gd7", 2));//(24, 4)));
                        lsTmp.Add("发生时的Uc/Ucb：" + GetTheData("gd7", 2));//(28, 4)));

                        lsTmp.Add("发生时的Ia：" + GetTheData("gd25", 3));//(32, 6)));
                        lsTmp.Add("发生时的Ib：" + GetTheData("gd25", 3));//(38, 6)));
                        lsTmp.Add("发生时的Ic：" + GetTheData("gd25", 3));//(44, 6)));

                        lsTmp.Add("发生时电能表正向有功总电能示值：" + GetTheData("gd14", 5));//(50, 10)));
                        break;

                    case 10:
                        lsTmp.Add("电压回路异常：ERC10");
                        lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));
                        ibg = Convert.ToInt16(GetTheData("ExplanNo", 1), 16);
                        if (Comm.GetAnyValue(ibg, 0, 0) == 1)
                            str1 += "A相";
                        if (Comm.GetAnyValue(ibg, 1, 1) == 1)
                            str1 += "B相";
                        if (Comm.GetAnyValue(ibg, 2, 2) == 1)
                            str1 += "C相";
                        switch (Comm.GetAnyValue(ibg, 6, 7))
                        {
                            case 1:
                                str1 += "断相"; break;
                            case 2:
                                str1 += "失压"; break;
                        }

                        lsTmp.Add("异常标志：" + str1);
                        lsTmp.Add("发生时的Ua/Uab：" + GetTheData("gd7", 2));//(20, 4)));
                        lsTmp.Add("发生时的Ub：" + GetTheData("gd7", 2));//(24, 4)));
                        lsTmp.Add("发生时的Uc/Ucb：" + GetTheData("gd7", 2));//(28, 4)));

                        lsTmp.Add("发生时的Ia：" + GetTheData("gd25", 3));//(32, 6)));
                        lsTmp.Add("发生时的Ib：" + GetTheData("gd25", 3));//(38, 6)));
                        lsTmp.Add("发生时的Ic：" + GetTheData("gd25", 3));//(44, 6)));

                        lsTmp.Add("发生时电能表正向有功总电能示值：" + GetTheData("gd14", 5));//(50, 10)));
                        break;
                    case 11:
                        lsTmp.Add("相序异常：ERC11"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));

                        lsTmp.Add("∠Ua/Uab （单位:度）：" + GetTheData("gd5", 2));//(18, 4)));
                        lsTmp.Add("∠Ub （单位:度）：" + GetTheData("gd5", 2));//(22, 4)));
                        lsTmp.Add("∠Uc/Ucb （单位:度）：" + GetTheData("gd5", 2));//(26, 4)));

                        lsTmp.Add("∠Ia （单位:度）：" + GetTheData("gd5", 2));//(30, 4)));
                        lsTmp.Add("∠Ib（单位:度）：" + GetTheData("gd5", 2));//(34, 4)));
                        lsTmp.Add("∠Ic（单位:度）：" + GetTheData("gd5", 2));//(38, 4)));

                        lsTmp.Add("发生时电能表正向有功总电能示值：" + GetTheData("gd14", 5));//(42, 10)));
                        break;

                    case 12:
                        lsTmp.Add("电能表时间超差：ERC12"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));
                        break;
                    case 13:
                        lsTmp.Add("Erc=13,电表故障信息：ERC13"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));

                        ibg = Convert.ToInt16(GetTheData("ExplanNo", 1), 16);
                        if (Comm.GetAnyValue(ibg, 0, 0) == 1)
                            str1 += "电能表编程次数或最大需量清零次数发生变化";
                        if (Comm.GetAnyValue(ibg, 1, 1) == 1)
                            str1 += "电能表断相次数变化";
                        if (Comm.GetAnyValue(ibg, 2, 2) == 1)
                            str1 += "电能表失压次数变化";
                        if (Comm.GetAnyValue(ibg, 3, 3) == 1)
                            str1 += "电能表停电次数变化";
                        if (Comm.GetAnyValue(ibg, 4, 4) == 1)
                            str1 += "电能表电池欠压";
                        lsTmp.Add("异常标志：" + str1);

                        break;
                    case 14:
                        lsTmp.Add("终端停/上电事件：ERC14"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        str1 = GetTheData("ExplanWei", 1);
                        lsTmp.Add("事件正常异常：" + (str1.Substring(7, 1) == "1" ? "事件正常" : "事件异常"));
                        lsTmp.Add("事件有效无效：" + (str1.Substring(6, 1) == "1" ? "事件有效" : "事件无效"));
                        lsTmp.Add("停电发生时间:分时日月年：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("上电时间:分时日月年：" + GetTheData("gd15", 5));//(14, 10)));
                        break;
                    case 15:
                        lsTmp.Add("谐波越限告警：ERC15"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));
                        break;
                    case 16:
                        lsTmp.Add("直流模拟量越限记录：ERC16"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("直流模拟量端口号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));
                        break;
                    case 17:
                        lsTmp.Add("电压/电流不平衡度越限记录：ERC17"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));
                        ibg = Convert.ToInt16(GetTheData("ExplanNo", 1), 16);
                        if (Comm.GetAnyValue(ibg, 0, 0) == 1)
                            str1 += "电压不平衡度越限";
                        if (Comm.GetAnyValue(ibg, 1, 1) == 1)
                            str1 += "电流不平衡度越限";
                        lsTmp.Add("异常标志：" + str1);

                        lsTmp.Add("发生时的电压不平衡度（％）：" + GetTheData("gd5", 2));
                        lsTmp.Add("发生时的电流不平衡度（％）：" + GetTheData("gd5", 2));
                        lsTmp.Add("发生时的Ua：" + GetTheData("gd7", 2));
                        lsTmp.Add("发生时的Ub：" + GetTheData("gd7", 2));
                        lsTmp.Add("发生时的Uc：" + GetTheData("gd7", 2));
                        lsTmp.Add("发生时的Ia：" + GetTheData("gd25", 3));
                        lsTmp.Add("发生时的Ib：" + GetTheData("gd25", 3));
                        lsTmp.Add("发生时的Ic：" + GetTheData("gd25", 3));
                        break;

                    case 18:
                        lsTmp.Add("电容器投切自锁记录：ERC18"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));
                        break;
                    case 19:
                        lsTmp.Add("购电参数设置记录：ERC19"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("购电能量设置时间:分时日月年：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("总加组号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("购电单号：" + GetTheData("ExplanBinFun", 4));//(16, 8)));

                        lsTmp.Add("追加/刷新标志：" + (GetTheData("ExplanBinFun", 1) == "55" ? "55H:追加" : "AAH:刷新"));

                        lsTmp.Add("购电量值：" + GetTheData("gd3", 4));//(26, 8)));
                        lsTmp.Add("报警门限：" + GetTheData("gd3", 4));//(26, 8)));
                        lsTmp.Add("跳闸门限：" + GetTheData("gd3", 4));//(26, 8)));
                        lsTmp.Add("本次购电前剩余电能量（费）：" + GetTheData("gd3", 4));//(26, 8)));
                        lsTmp.Add("本次购电后剩余电能量（费）：" + GetTheData("gd3", 4));//(26, 8)));
                        break;
                    case 20:
                        lsTmp.Add("消息认证错误记录：ERC20"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("消息认证码PW：" + GetTheData("ExplanNo", 16));
                        lsTmp.Add("启动站地址MSA：" + GetTheData("ExplanNo", 1));
                        break;
                    case 21:
                        lsTmp.Add("终端故障记录：ERC21"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        ibg = Convert.ToInt16(GetTheData("ExplanBinFun", 1));//(14, 2));
                        switch (ibg)
                        {
                            case 1:
                                str1 = "终端主板内存故障"; break;
                            case 2:
                                str1 = "时钟故障"; break;
                            case 3:
                                str1 = "主板通信故障"; break;
                            case 4:
                                str1 = "485抄表故障"; break;
                            case 5:
                                str1 = "显示板故障"; break;
                            case 6:
                                str1 = "载波通道异常"; break;
                        }
                        lsTmp.Add("终端故障编码：" + str1);
                        break;
                    case 22:
                        lsTmp.Add("有功总电能量差动越限事件记录：ERC22"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("电能量差动组号   ：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));
                        break;
                    case 23:
                        lsTmp.Add("电控告警事件记录：ERC23"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("总加组号   ：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));

                        lsTmp.Add("投入轮次：" + GetTheData("ExplanWei", 1));//(

                        ibg = Convert.ToInt16(GetTheData("ExplanNo", 1), 16);
                        if (Comm.GetAnyValue(ibg, 0, 0) == 1)
                            str1 += "月电控";
                        if (Comm.GetAnyValue(ibg, 1, 1) == 1)
                            str1 += "购电控";

                        lsTmp.Add("电控类别：" + str1);
                        lsTmp.Add("告警时电能量：" + GetTheData("gd3", 4));//(20, 8)));
                        lsTmp.Add("告警时电控定值：" + GetTheData("gd3", 4));//(28, 8)));
                        break;
                    case 24:
                        lsTmp.Add("电压越限记录：ERC24"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));

                        ibg = Convert.ToInt16(GetTheData("ExplanNo", 1), 16);
                        if (Comm.GetAnyValue(ibg, 0, 0) == 1)
                            str1 += "A相";
                        if (Comm.GetAnyValue(ibg, 1, 1) == 1)
                            str1 += "B相";
                        if (Comm.GetAnyValue(ibg, 2, 2) == 1)
                            str1 += "C相";
                        switch (Comm.GetAnyValue(ibg, 6, 7))
                        {
                            case 1:
                                str1 += "电压越上上限"; break;
                            case 2:
                                str1 += "电压越下下限"; break;
                        }

                        lsTmp.Add("越限标志：" + str1);

                        lsTmp.Add("发生时的Ua/Uab：" + GetTheData("gd7", 2));//(20, 4)));
                        lsTmp.Add("发生时的Ub：" + GetTheData("gd7", 2));//(24, 4)));
                        lsTmp.Add("发生时的Uc/Ucb：" + GetTheData("gd7", 2));//(28, 4)));

                        break;
                    case 25:
                        lsTmp.Add("电流越限记录：ERC25"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));

                        ibg = Convert.ToInt16(GetTheData("ExplanNo", 1), 16);
                        if (Comm.GetAnyValue(ibg, 0, 0) == 1)
                            str1 += "A相";
                        if (Comm.GetAnyValue(ibg, 1, 1) == 1)
                            str1 += "B相";
                        if (Comm.GetAnyValue(ibg, 2, 2) == 1)
                            str1 += "C相";
                        switch (Comm.GetAnyValue(ibg, 6, 7))
                        {
                            case 1:
                                str1 += "电流越上上限"; break;
                            case 2:
                                str1 += "电流越上限"; break;
                        }

                        lsTmp.Add("越限标志：" + str1);

                        lsTmp.Add("发生时的Ia：" + GetTheData("gd25", 3));//(20, 6)));
                        lsTmp.Add("发生时的Ib：" + GetTheData("gd25", 3));//(26, 6)));
                        lsTmp.Add("发生时的Ic：" + GetTheData("gd25", 3));//(32, 6)));

                        break;
                    case 26:
                        lsTmp.Add("视在功率越限记录：ERC26"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));

                        ibg = Convert.ToInt16(GetTheData("ExplanNo", 1), 16);
                        switch (Comm.GetAnyValue(ibg, 6, 7))
                        {
                            case 1:
                                str1 += "视在功率越上上限"; break;
                            case 2:
                                str1 += "视在功率越上限"; break;
                        }

                        lsTmp.Add("越限标志：" + str1);

                        lsTmp.Add("发生时的视在功率：" + GetTheData("gd23", 3));//(20, 6)));
                        lsTmp.Add("发生时的视在功率限值：" + GetTheData("gd23", 3));//(26, 6)));

                        break;
                    case 27:
                        lsTmp.Add("电能表示度下降记录：ERC27"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));

                        lsTmp.Add("下降前电能表正向有功总电能示值：" + GetTheData("gd14", 5));//(18, 10)));
                        lsTmp.Add("下降后电能表正向有功总电能示值：" + GetTheData("gd14", 5));//(28, 10)));

                        break;
                    case 28:
                        lsTmp.Add("电能量超差记录：ERC28"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));

                        lsTmp.Add("电能量超差发生时对应正向有功总电能示值：" + GetTheData("gd14", 5));//(18, 10)));
                        lsTmp.Add("电能量超差发生时正向有功总电能示值：" + GetTheData("gd14", 5));//(28, 10)));
                        lsTmp.Add("电能量超差阈值：" + GetTheData("gd22", 1));//(38, 2)));
                        break;
                    case 29:
                        lsTmp.Add("电能表飞走记录：ERC29"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));

                        lsTmp.Add("电能表飞走发生前正向有功总电能示值：" + GetTheData("gd14", 5));//(18, 10)));
                        lsTmp.Add("电能表飞走发生后正向有功总电能示值：" + GetTheData("gd14", 5));//(28, 10)));
                        lsTmp.Add("电能表飞走阈值：" + GetTheData("gd22", 1));//(38, 2)));
                        break;
                    case 30:
                        lsTmp.Add("电能表停走记录：ERC30"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));

                        lsTmp.Add("电能表停走发生时正向有功总电能示值：" + GetTheData("gd14", 5));//(18, 10)));
                        lsTmp.Add("电能表停走阈值：" + GetTheData("ExplanBinFun", 1));//(28, 2)));
                        break;
                    case 31:
                        lsTmp.Add("终端485抄表失败事件记录：ERC31"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));

                        lsTmp.Add("最近一次抄表成功时间:分时日月年：" + GetTheData("gd15", 5));//(18, 10)));
                        lsTmp.Add("最近一次抄表成功正向有功总电能示值：" + GetTheData("gd14", 5));//(28, 10)));
                        lsTmp.Add("最近一次抄表成功正向无功总电能示值：" + GetTheData("gd11", 4));//(38, 8)));
                        break;
                    case 32:
                        lsTmp.Add("终端与主站通信流量超门限事件记录：ERC32"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("当月已发生的通信流量：" + GetTheData("ExplanBinFun", 1));//(14, 8)));
                        lsTmp.Add("月通信流量门限：" + GetTheData("ExplanBinFun", 1));//(22, 8)));
                        break;
                    case 33:
                        lsTmp.Add("电能表运行状态字变位事件记录：ERC33");
                        lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        str1 = GetTheData("ExplanBinFun", 1);
                        //lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));
                        lsTmp.Add("电表运行状态字变位标志1：" + GetTheData("ExplanNo", 2));
                        lsTmp.Add("电表运行状态字变位标志2：" + GetTheData("ExplanNo", 2));
                        lsTmp.Add("电表运行状态字变位标志3：" + GetTheData("ExplanNo", 2));
                        lsTmp.Add("电表运行状态字变位标志4：" + GetTheData("ExplanNo", 2));
                        lsTmp.Add("电表运行状态字变位标志5：" + GetTheData("ExplanNo", 2));
                        lsTmp.Add("电表运行状态字变位标志6：" + GetTheData("ExplanNo", 2));
                        lsTmp.Add("电表运行状态字变位标志7：" + GetTheData("ExplanNo", 2));
                        lsTmp.Add("电表运行状态字1：" + GetTheData("ExplanNo", 2));
                        lsTmp.Add("电表运行状态字2：" + GetTheData("ExplanNo", 2));
                        lsTmp.Add("电表运行状态字3：" + GetTheData("ExplanNo", 2));
                        lsTmp.Add("电表运行状态字4：" + GetTheData("ExplanNo", 2));
                        lsTmp.Add("电表运行状态字5：" + GetTheData("ExplanNo", 2));
                        lsTmp.Add("电表运行状态字6：" + GetTheData("ExplanNo", 2));
                        lsTmp.Add("电表运行状态字7：" + GetTheData("ExplanNo", 2));
                        break;
                    case 34:
                        lsTmp.Add("Erc=34,CT异常事件记录：34"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));
                        break;
                    case 35:
                        lsTmp.Add("发现未知电表事件记录：ERC35"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("终端通信端口号：" + GetTheData("ExplanBinFun", 1));//(14, 2)));
                        break;
                    case 37:
                        lsTmp.Add("电能表开表盖事件记录：ERC37"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 2));
                        lsTmp.Add("开表盖总次数：" + GetTheData("gd10", 3));
                        lsTmp.Add("发生时刻：" + GetTheData("gd1", 6));
                        lsTmp.Add("结束时刻：" + GetTheData("gd1", 6));
                        lsTmp.Add("开表盖前正向有功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开表盖前反向有功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开表盖前第一象限无功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开表盖前第二象限无功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开表盖前第三象限无功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开表盖前第四象限无功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开表盖后正向有功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开表盖后反向有功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开表盖后第一象限无功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开表盖后第二象限无功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开表盖后第三象限无功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开表盖后第四象限无功总电能：" + GetTheData("gd11", 4));
                        break;
                    case 38:
                        lsTmp.Add("电能表开端钮盒事件记录：ERC38"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 2));
                        lsTmp.Add("开端钮盒总次数：" + GetTheData("gd10", 3));
                        lsTmp.Add("发生时刻：" + GetTheData("gd1", 6));
                        lsTmp.Add("结束时刻：" + GetTheData("gd1", 6));
                        lsTmp.Add("开端钮盒前正向有功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开端钮盒前反向有功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开端钮盒前第一象限无功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开端钮盒前第二象限无功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开端钮盒前第三象限无功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开端钮盒前第四象限无功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开端钮盒后正向有功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开端钮盒后反向有功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开端钮盒后第一象限无功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开端钮盒后第二象限无功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开端钮盒后第三象限无功总电能：" + GetTheData("gd11", 4));
                        lsTmp.Add("开端钮盒后第四象限无功总电能：" + GetTheData("gd11", 4));
                        break;
                    case 40:
                        lsTmp.Add("磁场异常事件记录：ERC40"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("发生时间（分时日月年）：" + GetTheData("gd15", 5));//(4, 10)));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));
                        ibg = Convert.ToInt16(GetTheData("ExplanNo", 1), 16);
                        switch (ibg)
                        {
                            case 0:
                                str1 = "终端外接传感器"; break;
                            case 1:
                                str1 = "采集器"; break;
                            case 2:
                                str1 = "电能表"; break;
                        }
                        lsTmp.Add("设备类型：" + str1);
                        lsTmp.Add("设备地址：" + GetTheData("ExplanNo", 6));
                        lsTmp.Add("磁场异常类型：" + GetTheData("ExplanNo", 1));
                        break;
                    case 41:
                        lsTmp.Add("对时事件记录：ERC41"); lsTmp.Add("长度：" + GetTheData("ExplanBinFun", 1));
                        lsTmp.Add("测量点号：" + GetTheData("ExplanBinFun", 1));
                        str1 = GetTheData("ExplanBinFun", 1);
                        //lsTmp.Add("起止标志：" + (Comm.GetAnyValue(Convert.ToInt16(GetTheData("ExplanNo", 1), 16), 7, 7) == 1 ? "发生" : "恢复"));
                        lsTmp.Add("对时前时间：" + GetTheData("gd1", 6));
                        lsTmp.Add("对时后时间：" + GetTheData("gd1", 6));
                        break;
                    default:
                        lsTmp.Add("ERR"); break;
                }
                if (strFrameRemove.Length > 0)
                    ExplanErc();
            }
            catch
            {

            }
        }

        public void a0F()
        {

            try
            {
                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    default:
                        lsTmp.Add("ERR"); break;
                }

            }
            catch
            {

            }
        }
        public void a10()
        {
            int i1 = 0; int i2 = 0; string s1 = "";
            try
            {
                Pn = Convert.ToInt16(GetTheData("ExplanP", 2));
                Fn = Convert.ToInt16(GetTheData("ExplanF", 2));
                lsTmp.Add("Pn = " + Pn);
                switch (Fn)
                {
                    case 1:
                        lsTmp.Add("Fn = 1,透明转发：");
                        if (BoolUp)
                        {
                            lsTmp.Add("终端通信端口号：" + GetTheData("ExplanBinFun", 1));
                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("透明转发内容字节数k：" + i1);
                            lsTmp.Add("透明转发内容：" + GetTheData("ExplanNo", i1));
                        }
                        else
                        {
                            lsTmp.Add("终端通信端口号：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("透明转发通信控制字：" + GetTheData("ExplanNo", 1));
                            lsTmp.Add("透明转发接收等待报文超时时间：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("透明转发接收等待字节超时时间：" + GetTheData("ExplanBinFun", 1));
                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("透明转发内容字节数k：" + i1);
                            lsTmp.Add("透明转发内容：" + GetTheData("ExplanNo", i1));
                        }
                        break;
                    case 9:
                        lsTmp.Add("Fn = 9,转发主站直接对电表的抄读数据命令：");
                        if (BoolUp)
                        {
                            lsTmp.Add("终端通信端口号：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("转发目标地址：" + GetTheData("gd12", 6));
                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            switch (i1)
                            {
                                case 0:
                                    s1 = "不能执行转发"; break;
                                case 1:
                                    s1 = "转发接收超时"; break;
                                case 2:
                                    s1 = "转发接收错误"; break;
                                case 3:
                                    s1 = "转发接收确认"; break;
                                case 4:
                                    s1 = "转发接收否认"; break;
                                case 5:
                                    s1 = "转发接收数据"; break;
                            }
                            lsTmp.Add("转发结果标志：" + s1);
                            i2 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("转发直接抄读的数据内容字节数k+4：" + i2);
                            lsTmp.Add("转发直接抄读的数据标志：" + GetTheData("RExplanNo", 4));
                            lsTmp.Add("转发直接抄读的数据内容：" + GetTheData("RExplanNo", i2 - 4));
                        }
                        else
                        {
                            lsTmp.Add("终端通信端口号：" + GetTheData("ExplanBinFun", 1));
                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            for (int i = 0; i < i1; i++)
                            {
                                lsTmp.Add("第" + i + 1 + "级转发中继地址：" + GetTheData("gd12", 6));
                            }
                            lsTmp.Add("转发目标地址：" + GetTheData("gd12", 6));
                            i2 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            switch (i2)
                            {
                                case 0:
                                    s1 = "DL/T 645-1997"; break;
                                case 1:
                                    s1 = "DL/T 645-2007"; break;
                            }
                            lsTmp.Add("转发直接抄读的数据标识类型：" + s1);
                            lsTmp.Add("转发直接抄读的数据标识：" + GetTheData("RExplanNo", (i2 == 0) ? 2 : 4));
                        }
                        break;
                    case 10:
                        lsTmp.Add("Fn = 10,转发主站直接对电能表的批量抄读数据命令：");
                        if (BoolUp)
                        {
                            lsTmp.Add("终端通信端口号：" + GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("转发目标地址：" + GetTheData("gd12", 6));

                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            switch (i1)
                            {
                                case 0:
                                    s1 = "不能执行转发"; break;
                                case 1:
                                    s1 = "转发接收超时"; break;
                                case 2:
                                    s1 = "转发接收错误"; break;
                                case 3:
                                    s1 = "转发接收确认"; break;
                                case 4:
                                    s1 = "转发接收否认"; break;
                                case 5:
                                    s1 = "转发接收数据"; break;
                                case 6:
                                    s1 = "电能表数据主动上报"; break;
                            }
                            lsTmp.Add("转发结果标志：" + s1);
                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("转发直接抄读的数据个数n：" + i1);
                            for (int i = 1; i <= i1; i++)
                            {
                                i2 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                                lsTmp.Add("转发直接抄读的第" + i1 + "个数据内容字节数k+4：" + i2);
                                lsTmp.Add("转发直接抄读的第" + i1 + "个数据标志：" + GetTheData("RExplanNo", 4));
                                lsTmp.Add("转发直接抄读的第" + i1 + "个数据内容：" + GetTheData("RExplanNo", i2 - 4));
                            }
                        }
                        else
                        {
                            lsTmp.Add("终端通信端口号：" + GetTheData("ExplanBinFun", 1));
                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("转发中继个数：" + i1);
                            for (int i = 0; i < i1; i++)
                            {
                                lsTmp.Add("第" + i + 1 + "级转发中继地址：" + GetTheData("gd12", 6));
                            }
                            lsTmp.Add("转发目标地址：" + GetTheData("gd12", 6));
                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            i1 = Comm.GetAnyValue(i1, 0, 1);
                            lsTmp.Add("转发直接抄读的数据标志类型：" + ((i1 == 0) ? "DL/T 645-1997" : "DL/T 645-2007"));

                            i2 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("转发直接抄读的数据个数n：" + i2);
                            for (int i = 1; i <= i2; i++)
                            {
                                lsTmp.Add("转发直接抄读的第" + i1 + "个数据标志：" + GetTheData("RExplanNo", (i1 == 0) ? 2 : 4));
                            }
                        }
                        break;
                    case 12:
                        lsTmp.Add("Fn = 12,对批量电表下发任务命令：");
                        if (BoolUp)
                        {

                        }
                        else
                        {
                            s1 = GetTheData("RExplanNo", 2);
                            lsTmp.Add("任务格式：" + s1);
                            lsTmp.Add("任务类型：" + GetTheData("RExplanNo", 1));

                            if (s1 == "0000")//身份认证
                            {
                                i1 = Convert.ToInt16(GetTheData("RExplanNo", 2));
                                lsTmp.Add("任务数据长度：" + i1);
                                lsTmp.Add("任务数据：" + GetTheData("RExplanNo", i1));
                                i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                                lsTmp.Add("本次配置电表数量n：" + i1);
                                for (int i = 1; i <= i1; i++)
                                {
                                    lsTmp.Add("表号" + i + "：" + GetTheData("RExplanNo", 8));
                                    lsTmp.Add("表的密钥密文" + i + "：" + GetTheData("RExplanNo", 32));
                                }
                            }
                            else if (s1 == "0404")
                            {
                                i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                                lsTmp.Add("任务数据长度：" + i1);
                                lsTmp.Add("任务数据：" + GetTheData("RExplanNo", i1));
                                i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                                lsTmp.Add("本次配置电表数量n：" + i1);
                                for (int i = 1; i <= i1; i++)
                                {
                                    lsTmp.Add("表号" + i + "：" + GetTheData("RExplanNo", 8));
                                    lsTmp.Add("表的密钥密文" + i + "：" + GetTheData("RExplanNo", 32));
                                }
                            }
                        }
                        break;
                    case 13:
                        lsTmp.Add("Fn = 13,查询对批量电表下发任务的执行状态：");
                        if (BoolUp)
                        {
                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("任务类型：" + i1);
                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("批量执行对电表的任务中电表总数量：" + i1);
                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 2));
                            lsTmp.Add("成功电表总数量：" + i1);
                            for (int i = 1; i <= i1; i++)
                            {
                                lsTmp.Add("已成功的表号" + i + "：" + GetTheData("RExplanNo", 8));
                            }
                        }
                        else
                        {
                            i1 = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                            lsTmp.Add("任务类型：" + i1);

                        }
                        break;
                    default:
                        lsTmp.Add("ERR"); break;
                }

            }
            catch
            {

            }
        }

        public string GetTheData(string functionName, int ilen)
        {
            if (strFrameRemove.Length < ilen * 2)
            {
                strFrameRemove = "";
                return "";
            }
            object[] objTmp = new Object[] { strFrameRemove.Substring(0, ilen * 2) };
            strFrameRemove = strFrameRemove.Remove(0, ilen * 2);
            MethodInfo TheFunction = typeof(Comm).GetMethod(functionName);
            return TheFunction.Invoke(new Comm(), objTmp).ToString();
        }

        public void GetQxData(string strName, string strFromat, int ilen)
        {
            if (BoolUp)
            {

                lsTmp.Add("曲线类数据时标Td_m：" + GetTheData("gd15", 5));
                lsTmp.Add("冻结密度：" + GetTheData("getdjmd", 1));
                int n = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                lsTmp.Add("数据点个数：" + n);
                for (int i = 1; i <= n; i++)
                {
                    lsTmp.Add(strName + i + "：" + GetTheData(strFromat, ilen));//(18 + i * 4, 4)) + "%");
                }

            }
            else
            {
                lsTmp.Add("曲线类数据时标Td_m：" + GetTheData("gd15", 5));
                lsTmp.Add("冻结密度：" + GetTheData("getdjmd", 1));
                int n = Convert.ToInt16(GetTheData("ExplanBinFun", 1));
                lsTmp.Add("数据点个数：" + n);
            }
        }

        public void GetOneData(string strFnName, string[] strName)
        {
            lsTmp.Add("Fn = " + Fn + "," + strFnName);
            if (BoolUp || Afn == "04")
            {
                for (int i = 0; i < strName.Length / 3; i++)
                {
                    lsTmp.Add(strName[i * 3] + "：" + GetTheData(strName[i * 3 + 1], Convert.ToInt16(strName[i * 3 + 2])));
                }
            }
        }

        public void GetOneData(string[] strName)
        {
            for (int i = 0; i < strName.Length / 3; i++)
            {
                lsTmp.Add(strName[i * 3] + "：" + GetTheData(strName[i * 3 + 1], Convert.ToInt16(strName[i * 3 + 2])));
            }
        }

        public void GetTwoData(string strFnName, string[] strName)
        {
            lsTmp.Add("Fn = " + Fn + "," + strFnName);
            if (BoolUp)
            {
                for (int i = 0; i < strName.Length / 3; i++)
                {
                    lsTmp.Add(strName[i * 3] + "：" + GetTheData(strName[i * 3 + 1], Convert.ToInt16(strName[i * 3 + 2])));
                }
            }
            else
                lsTmp.Add(strName[0] + "：" + GetTheData(strName[1], Convert.ToInt16(strName[2])));
        }
    }
}
