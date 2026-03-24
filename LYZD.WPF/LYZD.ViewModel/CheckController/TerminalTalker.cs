using LYZD.Core.Function;
using LYZD.SocketComm;
using LYZD.TerminalProtocol.GW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LYZD.ViewModel.CheckController
{
    /// <summary>
    /// 终端类
    /// </summary>
    public class TerminalTalker : VerifyBase
    {
        #region 属性
        public int TerminalNo = 0;                                                                          // 表位号
        public Rs485Port My485Port = null;                                                              // 终端232/485通讯端口
        //public CommWithSocketOfServer MyNetPort = null;
        //public TcpServerTool MyNetPort = null;
        public bool m_bolOnLineStatus = false;

        public string IP_PORT = "127.0.0.1:8010";

        public int Index = 0;

        public string ReceiveData = "";                 //以太网接受到的有效数据
        public string ReceiveDataCache = "";            //以太网接受到的缓冲数据
        public string strCacheTmp = "";
        /// <summary>
        /// 存储完整帧的帧队列
        /// </summary>
        public Queue<string> ListCache = new Queue<string>();
        public Queue<string> ListCacheTmp = new Queue<string>();
        public byte AFn = 0;//
        public byte Fn = 0;//网络通讯时用来进行匹配的值
        public bool bol_ReportCon = true;//上报确认，在全事件测试时第二步对终端回复的报文不进行确认，此时为False。其它时间为True;
        public Int32 int_ReportCount = 0;//上报次数，在全事件测试时主站不回复确认，终端应重复上送
        public Socket MySocket = null;                                                                  // 终端GPRS/以太网通讯接口
        public Frame_3761_2013 Framer = new Frame_3761_2013();                                    // 组帧器
        public Frame_698 Framer698 = new Frame_698();
        public Analysis_698 Analysiser698 = new Analysis_698();
        public Analysis_3761_2013 Analysiser = new Analysis_3761_2013();        // 解帧器
        public Analysis_3761_2013new Analysisernew = new Analysis_3761_2013new();
        public string[] AnalysisedString = new string[0];                                           // 解析出来的字符串
        public string[] AnalysisedStruce = new string[0];                                          //帧结构
        public string AlalysisedData = "";

        #region 多定义一组解析数据，以太网调用接受数据解析，和发送数据后等待有可能造成同时访问数据解析异常
        public Analysis_3761_2013new Analysisernew1 = new Analysis_3761_2013new();
        public string[] AnalysisedString1 = new string[0];                                           // 解析出来的字符串
        public string[] AnalysisedStruce1 = new string[0];                                          //帧结构
        public string AlalysisedData1 = "";
        #endregion

        public List<string> Report = new List<string>();

        public Thread thread_WaitAnswer;
        #endregion

        #region 方法
        public string ReadData(byte p_byt_Afn, byte p_byt_Pn, byte p_byt_Fn)
        {
            string str_SendFrame = Framer.ReadData_Afn12(p_byt_Afn, p_byt_Pn, p_byt_Fn);
            return SendFrame(str_SendFrame);
        }

        /// <summary>
        /// 根据当前的Afn,Fn判断返回的Fn
        /// </summary>
        /// <param name="p_byt_Afn"></param>
        /// <param name="p_byt_Fn"></param>
        /// <returns></returns>
        public string GetReturnFn(byte p_byt_Afn, byte p_byt_Fn)
        {
            if (p_byt_Afn == 6 && p_byt_Fn == 20)
                return "Fn1";
            else if (p_byt_Afn == 6 && p_byt_Fn == 19)
                return "Fn1";
            else if (p_byt_Afn == 6 && p_byt_Fn == 16)
                return "Fn1";
            else if (p_byt_Afn == 6 && p_byt_Fn == 14)
                return "Fn1";
            else if (p_byt_Afn == 16 || p_byt_Afn == 10 || p_byt_Afn == 12 || p_byt_Afn == 13 || p_byt_Afn == 14 || p_byt_Afn == 6)
            {
                return "Fn = " + p_byt_Fn;
            }
            else
                return "Fn1";
        }

        /// <summary>
        /// 给有效帧赋值
        /// </summary>
        public void GetValidFrame(CommWithSocketOfServer comm, string frmStr)
        {
            if (Analysisernew.Analysis(frmStr, 0, ref AnalysisedString, ref AlalysisedData, ref AnalysisedStruce))
            {
                if (AlalysisedData.Contains("链路接口") && AnalysisedStruce[11].Contains("CON：1"))
                {
                    Utility.Log.LogManager.AddMessage(comm.Index + 1, "接收：" + frmStr + "\r\n" + AlalysisedData);
                    if (AnalysisedStruce.Length > 6)
                    {
                        if (AnalysisedStruce[5].Contains("：") && AnalysisedStruce[6].Contains("："))
                            meterInfo[comm.Index].Address = AnalysisedStruce[5].Split('：')[1] + AnalysisedStruce[6].Split('：')[1];
                    }
                    Framer.str_Ter_Code = meterInfo[comm.Index].Address.Substring(0, 4);
                    int intLenAddr = Analysisernew.LenAddr * 2;
                    Framer.str_Ter_Address = meterInfo[comm.Index].Address.Substring(4, intLenAddr);
                    Framer.int_ConPFC = byte.Parse(GetData(AnalysisedStruce, 12, Core.Enum.EnumTerimalDataType.e_byte));
                    Framer.str_ConMainStation = GetData(AnalysisedStruce, 7, Core.Enum.EnumTerimalDataType.e_string);
                    Framer.int_ConC = 11;
                    string str_SendData = Framer.ReturnOk();
                    meterInfo[comm.Index].Bol_GprsStatus = true;
                    comm.SendData(UsefulMethods.ConvertStringToBytes(str_SendData));
                    //MyNetPort.m_bolOnLineStatus = true;
                    EquipmentData.MeterGroupInfo.Meters[comm.Index].SetProperty("MD_ONLINE", "1");

                    if (Analysisernew.Analysis(str_SendData, 0, ref AnalysisedString, ref AlalysisedData, ref AnalysisedStruce))
                    {
                        Utility.Log.LogManager.AddMessage(comm.Index + 1, "发送：" + str_SendData + "\r\n" + AlalysisedData);
                    }
                }
                else if (AnalysisedStruce[11].Contains("CON：1"))
                {
                    //Utility.Log.LogManager.AddMessage(comm.Index + 1, "接收：" + frmStr + "\r\n" + AlalysisedData);

                    //MessageHelper.Instance.AddTestLog(comm.Index, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "接受：" + frmStr + "\r\n" + AlalysisedData + "\r\n");
                    Framer.int_ConPFC = byte.Parse(GetData(AnalysisedStruce, 12, Core.Enum.EnumTerimalDataType.e_byte));
                    Framer.str_ConMainStation = GetData(AnalysisedStruce, 7, Core.Enum.EnumTerimalDataType.e_string);
                    Framer.int_ConC = 0;
                    if (bol_ReportCon)
                    {
                        string str_SendData = Framer.ReturnOk();
                        meterInfo[comm.Index].Bol_GprsStatus = true;
                        comm.SendData(UsefulMethods.ConvertStringToBytes(str_SendData));
                        string AlalysisedData_ = ""; string[] AnalysisedStruce_ = new string[0];
                        if (Analysisernew.Analysis(str_SendData, 0, ref AnalysisedString, ref AlalysisedData_, ref AnalysisedStruce_))
                        {
                            //MessageHelper.Instance.AddTestLog(comm.Index, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "发送：" + str_SendData + "\r\n" + AlalysisedData_ + "\r\n");
                        }
                    }
                    ReceiveData = frmStr;
                    if (AlalysisedData.Contains(GetReturnFn(AFn, Fn)))
                    {
                        int_ReportCount++;
                        if (bol_ReportCon)
                            ReceiveData = frmStr;
                        if (!bol_ReportCon && int_ReportCount >= 2)
                        {
                            comm.ClearTimeOutSocket();
                            bol_ReportCon = true;
                        }
                    }


                }
                else if (AlalysisedData.Contains(GetReturnFn(AFn, Fn)))
                {
                    //MessageHelper.Instance.AddTestLog(comm.Index, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "接受：" + frmStr + "\r\n" + AlalysisedData + "\r\n");
                    int_ReportCount++;
                    if (ReceiveData == "")
                        ReceiveData = frmStr;
                }
                else
                {
                    //MessageHelper.Instance.AddTestLog(comm.Index, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "接受：" + frmStr + "\r\n" + AlalysisedData + "\r\n");
                }
            }
            else
            {
                //MessageHelper.Instance.AddTestLog(comm.Index, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "接受：" + frmStr + "\r\n" + AlalysisedData + "\r\n");
            }
        }

        public void GetValidFrame_698(string frmStr)
        {
            if (Analysiser698.Analysis(frmStr, ref AnalysisedString, ref AlalysisedData, ref AnalysisedStruce))
            {
                if (AlalysisedData.Contains("登录") || AlalysisedData.Contains("心跳"))
                {
                    //MessageHelper.Instance.AddLoginLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "表位" + (comm.Index + 1) + "接受：" + frmStr + "\r\n" + AlalysisedData + "\r\n");
                    MessageAdd(Index + 1, "接受：" + frmStr + "\r\n" + AlalysisedData, EnumLogType.提示信息);
                    if (AnalysisedStruce.Length > 6)
                    {
                        if (AnalysisedStruce[4].Contains("："))
                        {
                            meterInfo[Index].Address = AnalysisedStruce[4].Split('：')[1];
                            //CLOU_Model.TerminalModels.TerminalInfos[comm.Index + 1].Address = AnalysisedStruce[4].Split('：')[1];
                            Framer698.str_Ter_Address = AnalysisedStruce[4].Split('：')[1];
                        }
                    }
                    string str_SendData = Framer698.ReturnLogin(frmStr.Substring(30, 2), Convert.ToDateTime(AnalysisedString[5].Substring(9)));
                    meterInfo[Index].Bol_GprsStatus = true;
                    SendMsgEven.GetSeriPostMsg( UsefulMethods.ConvertStringToBytes(str_SendData), IP_PORT);
                    //comm.SendData(UsefulMethods.ConvertStringToBytes(str_SendData));
                    //m_bolOnLineStatus = true;
                    EquipmentData.MeterGroupInfo.Meters[Index].SetProperty("MD_ONLINE", "1");
                    if (Analysiser698.Analysis(str_SendData, ref AnalysisedString, ref AlalysisedData, ref AnalysisedStruce))
                    {
                        //MessageHelper.Instance.AddLoginLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "表位" + (comm.Index + 1) + "发送：" + str_SendData + "\r\n" + AlalysisedData + "\r\n");
                    }
                }
                else if (AnalysisedStruce[6].Contains("CON：1"))
                {

                    //MessageHelper.Instance.AddTestLog(comm.Index, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "接受：" + frmStr + "\r\n" + AlalysisedData + "\r\n");
                    Framer.int_ConPFC = byte.Parse(GetData(AnalysisedStruce, 12, Core.Enum.EnumTerimalDataType.e_byte));
                    Framer.str_ConMainStation = GetData(AnalysisedStruce, 7, Core.Enum.EnumTerimalDataType.e_string);
                    Framer.int_ConC = 0;
                    if (bol_ReportCon)
                    {
                        string str_SendData = Framer.ReturnOk();
                        meterInfo[Index].Bol_GprsStatus = true;
                        SendMsgEven.GetSeriPostMsg(UsefulMethods.ConvertStringToBytes(str_SendData), IP_PORT);
                        string AlalysisedData_ = ""; string[] AnalysisedStruce_ = new string[0];
                        if (Analysisernew.Analysis(str_SendData, 0, ref AnalysisedString, ref AlalysisedData_, ref AnalysisedStruce_))
                        {
                            //MessageHelper.Instance.AddTestLog(comm.Index, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "发送：" + str_SendData + "\r\n" + AlalysisedData_ + "\r\n");
                        }
                    }
                    ReceiveData = frmStr;
                    if (AlalysisedData.Contains(GetReturnFn(AFn, Fn)))
                    {
                        int_ReportCount++;
                        if (bol_ReportCon)
                            ReceiveData = frmStr;
                        if (!bol_ReportCon && int_ReportCount >= 2)
                        {
                            //VerifyBase.TcpServerTool.ClearTimeOutSocket();
                            bol_ReportCon = true;
                        }
                    }


                }
                else if (AlalysisedData.Contains(GetReturnFn(AFn, Fn)))
                {
                    //MessageHelper.Instance.AddTestLog(comm.Index, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "接受：" + frmStr + "\r\n" + AlalysisedData + "\r\n");
                    int_ReportCount++;
                    if (ReceiveData == "")
                        ReceiveData = frmStr;
                }
                else
                {
                    if (ReceiveData == "")
                        ReceiveData = frmStr;
                    if (isopenRep)
                    {

                        if (Report.Count == 0)
                        {
                            Report.Add(frmStr);
                        }
                        else
                        {
                            bool isinput = true;
                            foreach (string item in Report)
                            {
                                if (item == frmStr)
                                {
                                    isinput = false;
                                }
                            }
                            if (isinput)
                            {
                                Report.Add(frmStr);
                            }
                        }
                        
                        
                    }
                    //MessageHelper.Instance.AddTestLog(comm.Index, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "接受：" + frmStr + "\r\n" + AlalysisedData + "\r\n");
                }
            }
            else
            {
                LYZD.Utility.Log.LogManager.AddMessage(Index + "==" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "接受：" + frmStr + "\r\n" + AlalysisedData);
                //MessageHelper.Instance.AddTestLog(comm.Index, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "接受：" + frmStr + "\r\n" + AlalysisedData + "\r\n");
            }
        }

        /// <summary>
        /// 获取所有帧，包括无效帧，使用此方法是为了展示收到的无效帧，能够在测试日志中存储
        /// </summary>
        /// <param name="frmStr"></param>
        /// <returns></returns>
        public string GetOneFrameAndFillListCacheAll(string frmStr)
        {
            strCacheTmp = frmStr.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper();
            ReceiveDataCache = GetOneFrameAndFillListCache(frmStr);

            if (ListCacheTmp.Count > 0)
            {
                for (int i = ListCacheTmp.Count - 1; i >= 0; i--)
                {
                    string strTmp = ListCacheTmp.Dequeue();
                    int i1 = strCacheTmp.IndexOf(strTmp);
                    if (i1 > 0)
                        ListCache.Enqueue(strCacheTmp.Substring(0, i1));
                    strCacheTmp = strCacheTmp.Remove(0, i1 + strTmp.Length);

                    ListCache.Enqueue(strTmp);
                }

            }
            if (ReceiveDataCache.Length < strCacheTmp.Length)
            {
                ListCache.Enqueue(strCacheTmp.Substring(0, strCacheTmp.Length - ReceiveDataCache.Length));
            }
            return ReceiveDataCache;
        }

        /// <summary>
        /// 获取完整有效帧,最后面的有效断帧依然保留在ReceiveDataCache中
        /// </summary>
        /// <param name="frmStr"></param>
        /// <returns></returns>
        public string GetOneFrameAndFillListCache(string frmStr)
        {
            frmStr = frmStr.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper();
            if (frmStr.IndexOf("68") < 0)
                return "";
            else
            {
                frmStr = frmStr.Substring(frmStr.IndexOf("68"));
                if (frmStr.Length >= 12)
                {
                    if (frmStr.Substring(10, 2) == "68")
                    {
                        int ilen = Convert.ToInt16(frmStr.Substring(2, 2), 16) / 4 + Convert.ToInt16(frmStr.Substring(4, 2), 16) * 64;
                        if (frmStr.Length >= 16 + ilen * 2)
                        {
                            if (frmStr.Substring(14 + ilen * 2, 2) == "16")
                            {
                                ListCacheTmp.Enqueue(frmStr.Substring(0, 16 + ilen * 2));
                                frmStr = frmStr.Remove(0, frmStr.Substring(0, 16 + ilen * 2).Length);
                                frmStr = GetOneFrameAndFillListCache(frmStr);
                            }
                            else
                            {
                                frmStr = frmStr.Remove(0, 2);
                                frmStr = GetOneFrameAndFillListCache(frmStr);
                            }
                        }
                    }
                    else
                    {
                        frmStr = frmStr.Remove(0, 2);
                        frmStr = GetOneFrameAndFillListCache(frmStr);
                    }
                }
            }

            return frmStr;
        }

        /// <summary>
        /// 发送报文
        /// </summary>
        public string SendFrame(string p_str_Frame)
        {
            byte[] byt_Send = UsefulMethods.ConvertStringToBytes(p_str_Frame);
            byte[] byt_OutFrame = new byte[0];

            if (TerminalChannelType == Core.Enum.Cus_EmChannelType.Channel232 && My485Port != null)
            {
                My485Port.rs485port.SendData(byt_Send, out byt_OutFrame);
            }

            return UsefulMethods.ConvertBytesToString(byt_OutFrame);
        }
        #endregion

    }
}
