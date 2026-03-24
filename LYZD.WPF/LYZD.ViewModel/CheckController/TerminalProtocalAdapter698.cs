using LYZD.Core.Function;
using LYZD.SocketComm;
using LYZD.Utility.Log;
using LYZD.ViewModel.CheckController.MulitThread;
using System;
using System.Collections.Generic;
using System.Threading;
using ZH.MeterProtocol.Comm;

namespace LYZD.ViewModel.CheckController
{
    public class TerminalProtocalAdapter698 : SingletonBase<TerminalProtocalAdapter698>
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
            if (p_byt_Afn == 10 || p_byt_Afn == 12 || p_byt_Afn == 13 || p_byt_Afn == 14)
            {
                return "Fn = " + p_byt_Fn;
            }
            else
                return "Fn1";
        }

        public void CommComunication(TerminalTalker talker, int pos, byte[] byt_SendData, byte[] byt_OutFrame, ref int[] intResult, ref Dictionary<int, string[]> dicData, int MaxWaitSeconds)
        {
            //Thread.Sleep(2000);
            int int_Return = -1; int iTmp = 0;
            while (iTmp < 3 && int_Return == -1)
            {
                string str_OutFrame = UsefulMethods.ConvertBytesToString(byt_SendData);
                talker.AlalysisedData = "ERR";
                talker.Analysiser698.Analysis(str_OutFrame, ref talker.AnalysisedString, ref talker.AlalysisedData, ref talker.AnalysisedStruce);
                EquipmentData.Controller.MessageAdd(pos+1, "发送：" + str_OutFrame + "\r\n" + talker.AlalysisedData, EnumLogType.流程信息);
                //Thread.Sleep(1000);
                //CLOU_Model.TerminalModels.datReadTime[pos] = DateTime.Now;
                int_Return = talker.My485Port.SendData(byt_SendData, out byt_OutFrame, MaxWaitSeconds);

                str_OutFrame = UsefulMethods.ConvertBytesToString(byt_OutFrame).ToUpper();
                talker.AlalysisedData = "ERR";
                if (talker.Analysiser698.Analysis(str_OutFrame, ref talker.AnalysisedString, ref talker.AlalysisedData, ref talker.AnalysisedStruce))
                {
                    EquipmentData.Controller.MessageAdd(pos + 1, "接收：" + str_OutFrame + "\r\n" + talker.AlalysisedData, EnumLogType.流程信息);
                    intResult[pos] = 0;
                    dicData.Add(pos, talker.AnalysisedString);
                }
                else
                {
                    EquipmentData.Controller.MessageAdd(pos + 1, "接收：" + str_OutFrame + "\r\n" + talker.AlalysisedData, EnumLogType.错误信息);
                    //UsefulMethods.WriteLog(DateTime.Now.ToString() + " 表位" + (pos + 1) + "：" + str_OutFrame, "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\错误通讯帧");
                    iTmp++;
                    int_Return = -1;
                    if (iTmp >= 3)
                    {
                        intResult[pos] = 1;
                        dicData.Add(pos, null);
                    }
                }
            }
        }

        public void CommComunication_Gprs(TerminalTalker talker, int pos, byte[] byt_SendData, byte[] byt_OutFrame, ref int[] intResult, ref Dictionary<int, string[]> dicData, int MaxWaitSeconds)
        {

            if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelEther || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelGPRS)
                VerifyBase.CheckTalkersNetStatus();
            int int_Return = -1; int iTmp = 0;
            while (iTmp < 2 && int_Return == -1)
            {
                string str_OutFrame = UsefulMethods.ConvertBytesToString(byt_SendData);
                talker.Analysiser698.Analysis(str_OutFrame, ref talker.AnalysisedString, ref talker.AlalysisedData, ref talker.AnalysisedStruce);
                EquipmentData.Controller.MessageAdd(pos + 1, "发送：" + str_OutFrame + "\r\n" + talker.AlalysisedData, EnumLogType.流程信息);

                //MessageHelper.Instance.AddTestLog(pos, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "发送：" + str_OutFrame + "\r\n" + talker.AlalysisedData + "\r\n");
                //CLOU_Model.TerminalModels.datReadTime[pos] = DateTime.Now;
                talker.ReceiveData = "";
                SendMsgEven.GetSeriPostMsg(byt_SendData,talker.IP_PORT);
                //talker.MyNetPort.SendData(byt_SendData);

                int p_int_Time = MaxWaitSeconds / 50;
                while (p_int_Time != 0)
                {
                    if (!EquipmentData.Controller.IsCheckVerify) break;//停止检定了就退出
                    Thread.Sleep(50);
                    p_int_Time--;
                    if (talker.ReceiveData != "")
                    {
                        p_int_Time = 0;
                    }
                }

                str_OutFrame = talker.ReceiveData;
                if (talker.Analysiser698.Analysis(str_OutFrame, ref talker.AnalysisedString, ref talker.AlalysisedData, ref talker.AnalysisedStruce))
                {
                    EquipmentData.Controller.MessageAdd(pos + 1, "接收：" + str_OutFrame + "\r\n" + talker.AlalysisedData, EnumLogType.流程信息);

                    //MessageHelper.Instance.AddTestLog(pos, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "接受：" + str_OutFrame + "\r\n" + talker.AlalysisedData + "\r\n");
                    int_Return = 0;
                    intResult[pos] = 0;
                    dicData.Add(pos, talker.AnalysisedString);
                }
                else
                {
                    //LogManager.AddMessage(pos + 1, "接收：" + str_OutFrame + "   " + talker.AlalysisedData);
                    EquipmentData.Controller.MessageAdd(pos + 1, "接收：" + str_OutFrame + "\r\n" + talker.AlalysisedData, EnumLogType.错误信息);

                    //MessageHelper.Instance.AddTestLog(pos, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff ") + "接受：" + str_OutFrame + "\r\n" + talker.AlalysisedData + "\r\n");
                    //UsefulMethods.WriteLog(DateTime.Now.ToString() + " 表位" + (pos + 1) + "：" + str_OutFrame, "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\错误通讯帧");
                    iTmp++;
                    int_Return = -1;
                    if (iTmp >= 2)
                    {
                        intResult[pos] = 1;
                        dicData.Add(pos, null);
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
                      CommComunication(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, MaxWaitSeconds);
                  else if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelEther || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelGPRS)
                      CommComunication_Gprs(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, MaxWaitSeconds);

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
            if (dicData.Count < iTmp)
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
                      CommComunication(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, MaxWaitSeconds);
                  else if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelEther || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelGPRS)
                      CommComunication_Gprs(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, MaxWaitSeconds);
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
            if (dicData.Count < iTmp)
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
                       CommComunication(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, MaxWaitSeconds);
                   else if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelEther || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelGPRS)
                       CommComunication_Gprs(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, MaxWaitSeconds);
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
            if (dicData.Count < iTmp)
                WriteData_Afn04(p_byt_Afn, p_byt_Pn, p_byt_Fn, p_str_SetData, dicData, MaxWaitSeconds);

            return intResult;
        }

        public int[] WriteData(byte p_byt_Afn, byte p_byt_Pn, byte p_byt_Fn, byte[] p_byt_SetData, Dictionary<int, string[]> dicData, int MaxWaitSeconds)
        {
            int[] intResult = new int[VerifyBase.meterInfo.Length];
            runFlag = true; dicData.Clear();
            SocketThreadManager.Instance.DoWork = delegate (int pos)
             {
                 if (!runFlag) return;
                 TerminalTalker talker = VerifyBase.Talkers[pos];

                 talker.AnalysisedString = new string[0];
                 string str_SendData = talker.Framer.WriteData(p_byt_Afn, p_byt_Pn, p_byt_Fn, p_byt_SetData);
                 byte[] byt_SendData = UsefulMethods.ConvertStringToBytes(str_SendData);
                 byte[] byt_OutFrame = null;
                 if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.Channel232 || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelMaintain)
                     CommComunication(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, MaxWaitSeconds);
                 else if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelEther || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelGPRS)
                     CommComunication_Gprs(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, MaxWaitSeconds);
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
            if (dicData.Count < iTmp)
                WriteData(p_byt_Afn, p_byt_Pn, p_byt_Fn, p_byt_SetData, dicData, MaxWaitSeconds);

            return intResult;
        }

        public int[] WriteData(byte[][] p_byt_SetData, Dictionary<int, string[]> dicData, int MaxWaitSeconds)
        {
            int[] intResult = new int[VerifyBase.meterInfo.Length];
            runFlag = true; dicData.Clear();
            //if (!EquipmentData.Controller.IsCheckVerify) return intResult;//停止检定了就退出
            SocketThreadManager.Instance.DoWork = delegate (int pos)
              {
                  if (!runFlag) return;
                  TerminalTalker talker = VerifyBase.Talkers[pos];

                  talker.AnalysisedString = new string[0];
                  byte[] byt_SendData = new byte[0];
                  if (p_byt_SetData[pos] != null)
                  {
                      //str_SendData = talker.Framer.WriteData(p_byt_Afn, p_byt_Pn, p_byt_Fn, p_byt_SetData[pos]);
                      byt_SendData = p_byt_SetData[pos];
                  }
                  byte[] byt_OutFrame = null;
                  if (byt_SendData.Length > 0)
                  {
                      if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.Channel232 || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelMaintain)
                          CommComunication(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, MaxWaitSeconds);
                      else if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelEther || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelGPRS)
                          CommComunication_Gprs(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, MaxWaitSeconds);
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
            for (int i = 0; i < VerifyBase.meterInfo.Length; i++)
            {
                if (VerifyBase.meterInfo[i].YaoJianYn)
                    iTmp++;
            }
            if (!EquipmentData.Controller.IsCheckVerify) return intResult;//停止检定了就退出
            if (dicData.Count < iTmp)
                WriteData(p_byt_SetData, dicData, MaxWaitSeconds);

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
                         CommComunication(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, MaxWaitSeconds);
                     else if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelEther || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelGPRS)
                         CommComunication_Gprs(talker, pos, byt_SendData, byt_OutFrame, ref intResult, ref dicData, MaxWaitSeconds);
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
            if (dicData.Count < iTmp)
                ReadEvent(p_byt_Afn, p_byt_Pn, p_byt_Fn, dicData, EventNum, MaxWaitSeconds);

            return intResult;
        }

        #endregion

        /// <summary>
        /// 等待所有线程完成
        /// </summary>
        private void WaitWorkDone()
        {
            int MaxTime = 60;
            while (true)
            {
                if (!runFlag) break;
                if (SocketThreadManager.Instance.IsWorkDone())
                {
                    runFlag = false;
                    break;
                }
                //if (MaxTime == 1) MaxTime = 60;
                EquipmentData.Controller.MessageAdd("正在通讯,剩余时间:" + MaxTime-- + "秒", EnumLogType.提示信息);
                if (!EquipmentData.Controller.IsCheckVerify) break;//停止检定了就退出
                Thread.Sleep(1000);
            }
        }

        public string ReadData_Afn12(byte Afn, byte Pn, byte Fn)
        {
            return "";
        }
    }
}
