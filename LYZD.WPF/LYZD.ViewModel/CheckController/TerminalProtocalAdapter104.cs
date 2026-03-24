using LYZD.Core.Function;
using LYZD.Utility.Log;
using LYZD.ViewModel.CheckController.MulitThread;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZH.MeterProtocol.Comm;

namespace LYZD.ViewModel.CheckController
{
    public class TerminalProtocalAdapter104 : SingletonBase<TerminalProtocalAdapter104>
    {
        private bool runFlag = false;

        public void CommComunication(TerminalTalker talker, int pos, string str_SendData, byte[] byt_OutFrame, ref int[] intResult, ref Dictionary<int, string[]> dicData, int MaxWaitSeconds)
        {
            //Thread.Sleep(2000);
            int int_Return = -1; int iTmp = 0;
            while (iTmp < 3 && int_Return == -1)
            {
                byte[] byt_SendData = System.Text.ASCIIEncoding.ASCII.GetBytes(str_SendData);
                string str_OutFrame = "";
                LogManager.AddMessage(pos, "发送：" + str_SendData + "==" + talker.AlalysisedData);
                //Thread.Sleep(1000);
                //CLOU_Model.TerminalModels.datReadTime[pos] = DateTime.Now;

                int_Return = talker.My485Port.SendData(byt_SendData, out byt_OutFrame, MaxWaitSeconds);
                if (byt_OutFrame == null)
                    str_OutFrame = "";
                else
                    str_OutFrame = System.Text.ASCIIEncoding.ASCII.GetString(byt_OutFrame);
                talker.AlalysisedData = "ERR";
                if (str_OutFrame != "")
                {
                    LogManager.AddMessage(pos, "接受：" + str_OutFrame);
                    intResult[pos] = 0;
                    dicData.Add(pos, new string[] { str_OutFrame });
                }
                else                  
                {
                       LogManager.AddMessage(pos, "接受：" + str_OutFrame);
                    UsefulMethods.WriteLog(DateTime.Now.ToString() + " 表位" + (pos + 1) + "：" + str_OutFrame, "Log\\终端日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\错误通讯帧");
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



        public int[] SetCommand(string str_SendData, Dictionary<int, string[]> dicData, int MaxWaitSeconds)
        {
            dicData.Clear();
            int[] intResult = new int[VerifyBase.meterInfo.Length];
            runFlag = true;
         SocketThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                TerminalTalker talker = VerifyBase.Talkers[pos];/// VerifyControler.Instance.Talkers[pos];

                talker.AnalysisedString = new string[0];
                byte[] byt_SendData = System.Text.ASCIIEncoding.ASCII.GetBytes(str_SendData);
                byte[] byt_OutFrame = null;
                CommComunication(talker, pos, str_SendData, byt_OutFrame, ref intResult, ref dicData, MaxWaitSeconds);
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
                SetCommand(str_SendData, dicData, MaxWaitSeconds);
            return intResult;
        }

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
                if (MaxTime == 1) MaxTime = 60;
                //LogManager.AddMessage( "接受：" + str_OutFrame);
                EquipmentData.Controller.MessageAdd("正在通讯,剩余时间:" + MaxTime-- + "秒",EnumLogType.提示信息);
                if (!EquipmentData.Controller.IsCheckVerify) break;//停止检定了就退出
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
