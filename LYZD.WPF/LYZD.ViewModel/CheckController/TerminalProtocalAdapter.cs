using LYZD.Core.Function;
using LYZD.SocketComm;
using LYZD.ViewModel.CheckController.MulitThread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ZH.MeterProtocol.Comm;

namespace LYZD.ViewModel.CheckController
{
    public class TerminalProtocalAdapter : SingletonBase<TerminalProtocalAdapter>
    {
        private bool runFlag = false;

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
        /// 232和维护口发送数据
        /// </summary>
        /// <param name="talker"></param>
        /// <param name="pos"></param>
        /// <param name="byt_SendData"></param>
        /// <param name="byt_OutFrame"></param>
        /// <param name="intResult"></param>
        /// <param name="dicData"></param>
        /// <param name="p_byt_Afn"></param>
        /// <param name="p_byt_Fn"></param>
        /// <param name="MaxWaitSeconds"></param>
        public void CommComunication(TerminalTalker talker, int pos, byte[] byt_SendData, byte[] byt_OutFrame, ref int[] intResult, ref Dictionary<int, string[]> dicData, byte p_byt_Afn, byte p_byt_Fn, int MaxWaitSeconds)
        {
            int int_Return = -1; int iTmp = 0;
            while (iTmp < 3 && int_Return == -1)
            {
                string str_OutFrame = Core.Function.UsefulMethods.ConvertBytesToString(byt_SendData);
                talker.Analysisernew.Analysis(str_OutFrame, 0, ref talker.AnalysisedString, ref talker.AlalysisedData, ref talker.AnalysisedStruce);
                Utility.Log.LogManager.AddMessage(pos + 1, "发送：" + str_OutFrame + "\r\n" + talker.AlalysisedData);
                //Thread.Sleep(1500);
                int_Return = talker.My485Port.SendData(byt_SendData, out byt_OutFrame, MaxWaitSeconds);

                str_OutFrame = Core.Function.UsefulMethods.ConvertBytesToString(byt_OutFrame);
                if (talker.Analysisernew.Analysis(str_OutFrame, 0, ref talker.AnalysisedString, ref talker.AlalysisedData, ref talker.AnalysisedStruce))
                {
                    if (talker.AlalysisedData.Contains(GetReturnFn(p_byt_Afn, p_byt_Fn)))
                    {
                        Utility.Log.LogManager.AddMessage(pos + 1, "接受：" + str_OutFrame + "\r\n" + talker.AlalysisedData);

                        int_Return = 0;
                        intResult[pos] = 0;
                        dicData.Add(pos, talker.AnalysisedString);
                    }
                    else
                    {
                        Utility.Log.LogManager.AddMessage(pos + 1, "接受：" + str_OutFrame + "\r\n" + talker.AlalysisedData);
                        UsefulMethods.WriteLog(DateTime.Now.ToString() + " 表位" + (pos + 1) + "：" + str_OutFrame, "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\错误通讯帧");
                        iTmp++;
                        int_Return = -1;
                        if (iTmp >= 3)
                        {
                            intResult[pos] = 1;
                            dicData.Add(pos, new string[] { "0" });
                        }
                    }
                }
                else
                {
                    Utility.Log.LogManager.AddMessage(pos + 1, "接受：" + str_OutFrame + "\r\n" + talker.AlalysisedData);
                    UsefulMethods.WriteLog(DateTime.Now.ToString() + " 表位" + (pos + 1) + "：" + str_OutFrame, "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\错误通讯帧");
                    iTmp++;
                    int_Return = -1;
                    if (iTmp >= 3)
                    {
                        intResult[pos] = 1;
                        dicData.Add(pos, new string[] { "0" });
                    }
                }
            }
        }

        /// <summary>
        /// 以太网GPRS发送数据
        /// </summary>
        /// <param name="talker"></param>
        /// <param name="pos"></param>
        /// <param name="byt_SendData"></param>
        /// <param name="byt_OutFrame"></param>
        /// <param name="intResult"></param>
        /// <param name="dicData"></param>
        /// <param name="p_byt_Afn"></param>
        /// <param name="p_byt_Fn"></param>
        /// <param name="MaxWaitSeconds"></param>
        public void CommComunication_Ether(TerminalTalker talker, int pos, byte[] byt_SendData, byte[] byt_OutFrame, ref int[] intResult, ref Dictionary<int, string[]> dicData, byte p_byt_Afn, byte p_byt_Fn, int MaxWaitSeconds)
        {
            if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelEther || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelGPRS)
                VerifyBase.CheckTalkersNetStatus();

            int int_Return = -1; int iTmp = 0;
            while (iTmp < 2 && int_Return == -1)
            {
                string str_OutFrame = Core.Function.UsefulMethods.ConvertBytesToString(byt_SendData);
                talker.Analysisernew1.Analysis(str_OutFrame, 0, ref talker.AnalysisedString1, ref talker.AlalysisedData1, ref talker.AnalysisedStruce1);
                Utility.Log.LogManager.AddMessage(pos+1, "发送：" + str_OutFrame + "\r\n" + talker.AlalysisedData1);

                //talker.ListCache.Clear();
                talker.ReceiveData = "";
                talker.AFn = p_byt_Afn;
                talker.Fn = p_byt_Fn;
                SendMsgEven.GetSeriPostMsg(byt_SendData, talker.IP_PORT);
                int p_int_Time = MaxWaitSeconds / 1000;
                while (p_int_Time != 0)
                {
                    Thread.Sleep(1000);
                    p_int_Time--;
                    if (talker.ReceiveData != "")
                    {
                        p_int_Time = 0;
                    }
                }

                str_OutFrame = talker.ReceiveData;
                if (talker.Analysisernew1.Analysis(str_OutFrame, 0, ref talker.AnalysisedString1, ref talker.AlalysisedData1, ref talker.AnalysisedStruce1))
                {
                    Utility.Log.LogManager.AddMessage(pos + 1, "接受：" + str_OutFrame + "\r\n" + talker.AlalysisedData);
                    int_Return = 0;
                    dicData.Add(pos, talker.AnalysisedString1);
                    if (talker.AnalysisedString1.Length < 3)
                        intResult[pos] = 1;
                    else
                        intResult[pos] = 0;
                }
                else
                 {
                    Utility.Log.LogManager.AddMessage(pos + 1, "接受：" + str_OutFrame + "\r\n" + talker.AlalysisedData);
                    UsefulMethods.WriteLog(DateTime.Now.ToString() + " 表位" + (pos + 1) + "：" + str_OutFrame, "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\错误通讯帧");
                    iTmp++;
                    int_Return = -1;
                    if (iTmp >= 2)
                    {
                        intResult[pos] = 1;
                        dicData.Add(pos, new string[] { "0" });
                    }
                }
            }
        }

        #region 读
        public int[] ReadData_Afn12(byte Afn, byte Pn, byte Fn, Dictionary<int, string[]> dicData, int MaxWaitSeconds)
        {
            dicData.Clear();
            int[] intResult = new int[VerifyBase.meterInfo.Length];
            runFlag = true;
            SocketThreadManager.Instance.DoWork = delegate (int pos)
              {
                  if (!runFlag) return;
                  TerminalTalker talker = VerifyBase.Talkers[pos];

                  talker.AnalysisedString = new string[0];
                  string str_SendData = talker.Framer.ReadData_Afn12(Afn, Pn, Fn);
                  byte[] byt_SendData = UsefulMethods.ConvertStringToBytes(str_SendData);
                  byte[] byt_OutFrame = null;
                  if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.Channel232 || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelMaintain)
                      CommComunication(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, Afn, Fn, MaxWaitSeconds);
                  else if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelEther || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelGPRS)
                      CommComunication_Ether(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, Afn, Fn, MaxWaitSeconds);

              };
            SocketThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            int iTmp = 0;
            for (int i = 0; i < VerifyBase.meterInfo.Length; i++)
            {
                if (VerifyBase.meterInfo[i].YaoJianYn)
                    iTmp++;
            }
            if (dicData.Count < iTmp && EquipmentData.Controller.IsCheckVerify)
                ReadData_Afn12(Afn, Pn, Fn, dicData, MaxWaitSeconds);
            return intResult;
        }

        public int[] ReadData(byte p_byt_Afn, byte p_byt_Pn, byte p_byt_Fn, byte[] p_byt_SendData, Dictionary<int, string[]> dicData, int MaxWaitSeconds)
        {
            int[] intResult = new int[VerifyBase.meterInfo.Length];
            runFlag = true; dicData.Clear();
            SocketThreadManager.Instance.DoWork = delegate (int pos)
             {
                 if (!runFlag) return;
                 TerminalTalker talker = VerifyBase.Talkers[pos];

                 talker.AnalysisedString = new string[0];
                 string str_SendData = talker.Framer.ReadData(p_byt_Afn, p_byt_Pn, p_byt_Fn, p_byt_SendData);
                 byte[] byt_SendData = UsefulMethods.ConvertStringToBytes(str_SendData);
                 byte[] byt_OutFrame = null;
                 if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.Channel232 || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelMaintain)
                     CommComunication(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, p_byt_Afn, p_byt_Fn, MaxWaitSeconds);
                 else if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelEther || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelGPRS)
                     CommComunication_Ether(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, p_byt_Afn, p_byt_Fn, MaxWaitSeconds);
             };
            SocketThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            int iTmp = 0;
            for (int i = 0; i < VerifyBase.meterInfo.Length; i++)
            {
                if (VerifyBase.meterInfo[i].YaoJianYn)
                    iTmp++;
            }
            if (dicData.Count < iTmp && EquipmentData.Controller.IsCheckVerify)
                ReadData(p_byt_Afn, p_byt_Pn, p_byt_Fn, p_byt_SendData, dicData, MaxWaitSeconds);

            return intResult;
        }

        #endregion

        #region 写

        public int[] WriteData_Afn04(byte p_byt_Afn, byte p_byt_Pn, byte p_byt_Fn, string p_str_SetData, Dictionary<int, string[]> dicData, int MaxWaitSeconds)
        {
            dicData.Clear();
            int[] intResult = new int[VerifyBase.meterInfo.Length];
            runFlag = true;
            SocketThreadManager.Instance.DoWork = delegate (int pos)
              {
                  if (!runFlag) return;
                  TerminalTalker talker = VerifyBase.Talkers[pos];

                  talker.AnalysisedString = new string[0];
                  string str_SendData = talker.Framer.WriteData_Afn04(p_byt_Afn, p_byt_Pn, p_byt_Fn, p_str_SetData);
                  byte[] byt_SendData = UsefulMethods.ConvertStringToBytes(str_SendData);
                  byte[] byt_OutFrame = null;
                  if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.Channel232 || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelMaintain)
                      CommComunication(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, p_byt_Afn, p_byt_Fn, MaxWaitSeconds);
                  else if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelEther || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelGPRS)
                      CommComunication_Ether(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, p_byt_Afn, p_byt_Fn, MaxWaitSeconds);
              };
            SocketThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            int iTmp = 0;
            for (int i = 0; i < VerifyBase.meterInfo.Length; i++)
            {
                if (VerifyBase.meterInfo[i].YaoJianYn)
                    iTmp++;
            }
            if (dicData.Count < iTmp && EquipmentData.Controller.IsCheckVerify)
                WriteData_Afn04(p_byt_Afn, p_byt_Pn, p_byt_Fn, p_str_SetData, dicData, MaxWaitSeconds);

            return intResult;
        }

        public int[] WriteData(byte p_byt_Afn, byte p_byt_Pn, byte p_byt_Fn, byte[] p_byt_SetData, Dictionary<int, string[]> dicData, int MaxWaitSeconds)
        {
            int[] intResult = new int[VerifyBase.meterInfo.Length];
            runFlag = true; 
            dicData.Clear();
            SocketThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                TerminalTalker talker = VerifyBase.Talkers[pos];

                talker.AnalysisedString = new string[0];
                string str_SendData = talker.Framer.WriteData(p_byt_Afn, p_byt_Pn, p_byt_Fn, p_byt_SetData);
                byte[] byt_SendData = UsefulMethods.ConvertStringToBytes(str_SendData);
                byte[] byt_OutFrame = null;
                if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.Channel232 || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelMaintain)
                    CommComunication(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, p_byt_Afn, p_byt_Fn, MaxWaitSeconds);
                else if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelEther || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelGPRS)
                    CommComunication_Ether(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, p_byt_Afn, p_byt_Fn, MaxWaitSeconds);
            };
            SocketThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            int iTmp = 0;
            for (int i = 0; i < VerifyBase.meterInfo.Length; i++)
            {
                if (VerifyBase.meterInfo[i].YaoJianYn)
                    iTmp++;
            }
            if (dicData.Count < iTmp && EquipmentData.Controller.IsCheckVerify)
                WriteData(p_byt_Afn, p_byt_Pn, p_byt_Fn, p_byt_SetData, dicData, MaxWaitSeconds);

            return intResult;
        }

        public int[] WriteData(byte p_byt_Afn, byte p_byt_Pn, byte p_byt_Fn, byte[][] p_byt_SetData, Dictionary<int, string[]> dicData, int MaxWaitSeconds)
        {
            int[] intResult = new int[VerifyBase.meterInfo.Length];
            runFlag = true; dicData.Clear();
            SocketThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                TerminalTalker talker = VerifyBase.Talkers[pos];

                talker.AnalysisedString = new string[0];

                string str_SendData = "";
                byte[] byt_SendData = new byte[0];
                if (p_byt_SetData[pos] != null)
                {
                    str_SendData = talker.Framer.WriteData(p_byt_Afn, p_byt_Pn, p_byt_Fn, p_byt_SetData[pos]);
                    byt_SendData = UsefulMethods.ConvertStringToBytes(str_SendData);
                }
                byte[] byt_OutFrame = null;
                if (byt_SendData.Length > 0)
                {
                    if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.Channel232 || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelMaintain)
                        CommComunication(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, p_byt_Afn, p_byt_Fn, MaxWaitSeconds);
                    else if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelEther || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelGPRS)
                        CommComunication_Ether(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, p_byt_Afn, p_byt_Fn, MaxWaitSeconds);
                }
                else
                {
                    intResult[pos] = 1;
                    if (!dicData.ContainsKey(pos))
                        dicData.Add(pos, null);
                }
            };
            SocketThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            int iTmp = 0;
            for (int i = 1; i <= VerifyBase.meterInfo.Length; i++)
            {
                if (VerifyBase.meterInfo[i - 1].YaoJianYn)
                    iTmp++;
            }
            if (dicData.Count < iTmp && EquipmentData.Controller.IsCheckVerify)
                WriteData(p_byt_Afn, p_byt_Pn, p_byt_Fn, p_byt_SetData, dicData, MaxWaitSeconds);

            return intResult;
        }

        /// <summary>
        /// 读事件
        /// </summary>
        /// <param name="p_byt_Afn"></param>
        /// <param name="p_byt_Pn"></param>
        /// <param name="p_byt_Fn"></param>
        /// <param name="p_byt_SetData"></param>
        /// <param name="dicData"></param>
        /// <returns></returns>
        public int[] ReadEvent(byte p_byt_Afn, byte p_byt_Pn, byte p_byt_Fn, Dictionary<int, string[]> dicData, int[] EventNum, int MaxWaitSeconds)
        {
            int[] intResult = new int[VerifyBase.meterInfo.Length];
            runFlag = true; dicData.Clear();
            SocketThreadManager.Instance.DoWork = delegate (int pos)
               {
                   if (!runFlag) return;
                   TerminalTalker talker = VerifyBase.Talkers[pos];

                   talker.AnalysisedString = new string[0];


                   string str_SendData = Convert.ToString(EventNum[pos] - 1, 16).PadLeft(2, '0') + Convert.ToString(EventNum[pos], 16).PadLeft(2, '0');
                   byte[] byt_SendData = UsefulMethods.ConvertStringToBytes(str_SendData);
                   str_SendData = talker.Framer.ReadData(14, 0, p_byt_Fn, byt_SendData);
                   byt_SendData = UsefulMethods.ConvertStringToBytes(str_SendData);
                   byte[] byt_OutFrame = null;
                   if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.Channel232 || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelMaintain)
                       CommComunication(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, p_byt_Afn, p_byt_Fn, MaxWaitSeconds);
                   else if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelEther || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelGPRS)
                       CommComunication_Ether(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, p_byt_Afn, p_byt_Fn, MaxWaitSeconds);
               };
            SocketThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            int iTmp = 0;
            for (int i = 1; i <= VerifyBase.meterInfo.Length; i++)
            {
                if (VerifyBase.meterInfo[i - 1].YaoJianYn)
                    iTmp++;
            }
            if (dicData.Count < iTmp && EquipmentData.Controller.IsCheckVerify)
                ReadEvent(p_byt_Afn, p_byt_Pn, p_byt_Fn, dicData, EventNum, MaxWaitSeconds);

            return intResult;
        }

        /// <summary>
        /// 等待事件上报,仅限GPRS，以太网调用
        /// </summary>
        public int[] WaitEventReport(Dictionary<int, string[]> dicData, int MaxWaitSeconds)
        {
            int[] intResult = new int[VerifyBase.meterInfo.Length];
            runFlag = true;
            dicData.Clear();
            bool bol_Complete = true;//事件完成标志

            while (MaxWaitSeconds != 0)
            {
                Thread.Sleep(1000);
                MaxWaitSeconds--;
                //Utility.Log.LogManager.AddMessage("正在等待事件上报,剩余时间:" + MaxWaitSeconds + "秒");
                EquipmentData.Controller.MessageAdd("正在等待事件上报,剩余时间:" + MaxWaitSeconds + "秒",EnumLogType.提示信息);
                if (!EquipmentData.Controller.IsCheckVerify) break;//停止检定了就退出

                bol_Complete = true;
                for (int i = 0; i < VerifyBase.meterInfo.Length; i++)
                {
                    if (VerifyBase.meterInfo[i].YaoJianYn)
                    {
                        if (VerifyBase.Talkers[i].ReceiveData == "")
                        {
                            bol_Complete = false;
                        }
                    }
                }
                if (bol_Complete)
                    break;
            }


            for (int i = 0; i < VerifyBase.meterInfo.Length; i++)
            {
                if (VerifyBase.meterInfo[i].YaoJianYn)
                {
                    if (VerifyBase.Talkers[i].ReceiveData != "")
                    {
                        string str_OutFrame = VerifyBase.Talkers[i].ReceiveData;
                        if (VerifyBase.Talkers[i].Analysisernew1.Analysis(str_OutFrame, 0, ref VerifyBase.Talkers[i].AnalysisedString1, ref VerifyBase.Talkers[i].AlalysisedData1, ref VerifyBase.Talkers[i].AnalysisedStruce1))
                        {
                            Utility.Log.LogManager.AddMessage(i+1, "接受：" + str_OutFrame + "\r\n" + VerifyBase.Talkers[i].AlalysisedData);
                            intResult[i] = 0;
                            dicData.Add(i, VerifyBase.Talkers[i].AnalysisedString1);
                        }
                    }
                    else
                        intResult[i] = 1;
                }
            }

            return intResult;
        }

        /// <summary>
        /// 等待事件上报,仅限GPRS，以太网调用
        /// </summary>
        public int[] WaitEventReport_(Dictionary<int, string[]> dicData, int MaxWaitSeconds)
        {
            int[] intResult = new int[VerifyBase.meterInfo.Length];
            runFlag = true; dicData.Clear();

            bool bol_Complete = true;//事件完成标志

            while (MaxWaitSeconds != 0)
            {
                Thread.Sleep(1000);
                MaxWaitSeconds--;
                //Utility.Log.LogManager.AddMessage("正在等待事件上报,剩余时间:" + MaxWaitSeconds + "秒");
                EquipmentData.Controller.MessageAdd("正在等待事件上报,剩余时间:" + MaxWaitSeconds + "秒",EnumLogType.提示信息);
                if (!EquipmentData.Controller.IsCheckVerify) break;//停止检定了就退出

                bol_Complete = true;
                for (int i = 0; i < VerifyBase.meterInfo.Length; i++)
                {
                    if (VerifyBase.meterInfo[i].YaoJianYn)
                    {
                        if (VerifyBase.Talkers[i].ReceiveData == "" || VerifyBase.Talkers[i].int_ReportCount < 3)
                        {
                            bol_Complete = false;
                        }
                    }
                }
                if (bol_Complete)
                    break;
            }
            return intResult;
        }

        #endregion

        /// <summary>
        /// 等待所有线程完成
        /// </summary>
        private void WaitWorkDone()
        {
            int MaxTime = 120;
            while (true)
            {
                if (!runFlag) break;
                if (SocketThreadManager.Instance.IsWorkDone())
                {
                    runFlag = false;
                    break;
                }
                if (MaxTime == 1) MaxTime = 120;
                EquipmentData.Controller.MessageAdd("正在通讯,剩余时间:" + MaxTime-- + "秒",EnumLogType.提示信息);
                if (!EquipmentData.Controller.IsCheckVerify) break;//停止检定了就退出
                System.Threading.Thread.Sleep(1000);
            }
        }

        public string ReadData_Afn12(byte Afn, byte Pn, byte Fn)
        {
            return "";
        }
    }
}
