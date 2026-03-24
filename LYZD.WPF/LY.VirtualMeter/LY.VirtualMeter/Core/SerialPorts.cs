using LY.VirtualMeter.ViewModel;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace LY.VirtualMeter.Core
{
    class SerialPorts : Function
    {
        public SerialPort TComm;
        public Dictionary<int, Meter> dic_Meter = new Dictionary<int, Meter>();
        int BiaoType = 1;

        public SerialPorts()
        {

            TComm = new SerialPort();
            TComm.DataReceived += new SerialDataReceivedEventHandler(TComm_DataReceived);
            TComm.BaudRate = 38400;
            TComm.DataBits = 8;
            TComm.Parity = System.IO.Ports.Parity.None;
            TComm.StopBits = System.IO.Ports.StopBits.One;
            TComm.DtrEnable = true;
            TComm.RtsEnable = true;
        }

        private void TComm_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            byte[] TInBuffer = new byte[0];        //本口用于接收数据的临时缓冲区
            int TInBufferSize = 0;
            string Tstr = "";
            while (TInBufferSize != TComm.BytesToRead)
            {
                TInBufferSize = TComm.BytesToRead;
                Delay(60);
            }

            TInBufferSize = TComm.BytesToRead;
            TInBuffer = new byte[TInBufferSize];
            TComm.Read(TInBuffer, 0, TInBufferSize);

            for (int i = 0; i < TInBufferSize; i++)
            {
                Tstr += Convert.ToString(TInBuffer[i], 16).PadLeft(2, '0');
            }
            string input = "";

            input = strCache + Tstr;

            if (input.Length > 0)
            {
                strCache = GetOneFrameAndFillListCache(input);
            }

            if (ListCache.Count > 0)
            {
                for (int i = ListCache.Count - 1; i >= 0; i--)
                {
                    g_Chen = ListCache.Dequeue();
                    SCoreSock();
                }

            }
        }

        string strCache = "";

        /// <summary>
        /// 存储完整帧的帧队列
        /// </summary>
        public Queue<string> ListCache = new Queue<string>();
        public string GetOneFrameAndFillListCache(string frmStr)
        {
            frmStr = frmStr.ToUpper();
            if (frmStr.IndexOf("68") < 0)
                return "";
            else
            {
                frmStr = frmStr.Substring(frmStr.IndexOf("68"));
                if (frmStr.Length >= 20)
                {
                    if (frmStr.Substring(14, 2) == "68")
                    {
                        int ilen = Convert.ToInt16(frmStr.Substring(18, 2), 16);
                        if (frmStr.Length >= 24 + ilen * 2)
                        {
                            if (frmStr.Substring(22 + ilen * 2, 2) == "16")
                            {
                                ListCache.Enqueue(frmStr.Substring(0, ilen * 2 + 24));
                                frmStr = frmStr.Remove(0, frmStr.Substring(0, ilen * 2 + 24).Length);
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

        private string g_Chen;
        public bool IsLog = false;
        public bool IsAnalysis = false;
        public delegate void EMoniBack(object sender, string Tdata, int Index);
        public event EMoniBack ClEventMoni;
        public ProtocolAnalysis.Portocol_645 portocol_645 = new ProtocolAnalysis.Portocol_645();
        int index = 0;
        public string ZbAddress = "000000000099";

        public void AriseEventMn(string Data)
        {
            if (ClEventMoni != null)
            {
                ClEventMoni(this, Data, index);
            }
        }

        private void SCoreSock()
        {
            int Tint; string Tstr; string Tre;
            int Tint2; string strTmpBdz = "";
            byte[] Tcl;

            Tint2 = g_Chen.IndexOf("68");
            if (g_Chen.Substring(Tint2 + 14, 2) == "68")// Mid(g_Chen, Tint2 + 14, 2) = "68" Then
                g_Chen = g_Chen.Substring(Tint2, g_Chen.Length - Tint2);//  Mid(g_Chen, InStr(g_Chen, "68"))
            else
            {
                g_Chen = "";
                return;
            }
            strTmpBdz = revStr(g_Chen.Substring(2, 12));

            Tint = Convert.ToInt32(g_Chen.Substring(18, 2), 16);
            if ((g_Chen.Length) >= (Tint + 12) * 2)
            {
                Tstr = g_Chen.Substring(0, (Tint + 12) * 2);
                if (IsLog)
                {
                    string strFileName = "Meter" + index;
                    string strLogMessage = DateTime.Now.ToString("HH:mm:ss fff ") + "收:" + Tstr;
                    LogInfoHelper.WriteLog(strFileName, strLogMessage);
                    //WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, DateTime.Now.ToString("HH:mm:ss fff ") + "收:" + Tstr);
                }
                AriseEventMn(DateTime.Now.ToString("HH:mm:ss fff ") + "收:" + Tstr);
                string s1 = ""; string s2 = ""; string s3 = ""; bool b1 = false; bool b2 = false;
                if (IsAnalysis)
                {
                    portocol_645.Analysis(Tstr, "11", ref s1, ref s2, ref b1, ref b2, ref s3);
                    AriseEventMn(s2);

                    if (IsLog)
                    {
                        string strFileName = "Meter" + index;
                        string strLogMessage = s2;
                        LogInfoHelper.WriteLog(strFileName, strLogMessage);
                        //WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, s2);
                    }
                }
                if (Tstr.Substring(Tstr.Length - 2, 2) == "16")//  Right(Tstr, 2) = 16)
                {
                    g_Chen = "";

                    Meter clmeter = null;
                    for (int i = 1; i <= dic_Meter.Count; i++)
                    {
                        if (dic_Meter[i].BAddress.IndexOf(strTmpBdz) > -1)
                        {
                            clmeter = dic_Meter[i];
                            break;
                        }
                    }
                    if (clmeter != null)
                    {
                        if (BiaoType == 1)
                            Tre = clmeter.CodeReturn2007(Tstr, ZbAddress);
                        else
                            Tre = clmeter.CodeReturn1997(Tstr, ZbAddress);
                        if (IsLog)
                        {
                            string strFileName = "Meter" + index;
                            string strLogMessage = DateTime.Now.ToString("HH:mm:ss fff ") + "发:" + Tre;
                            LogInfoHelper.WriteLog(strFileName, strLogMessage);
                            //WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, DateTime.Now.ToString("HH:mm:ss fff ") + "发:" + Tre);
                        }
                        AriseEventMn(DateTime.Now.ToString("HH:mm:ss fff ") + "发:" + Tre);
                        if (IsAnalysis)
                        {
                            portocol_645.Analysis(Tre, "11", ref s1, ref s2, ref b1, ref b2, ref s3);
                            AriseEventMn(s2);
                            if (IsLog)
                            {
                                string strFileName = "Meter" + index;
                                string strLogMessage = s2;
                                LogInfoHelper.WriteLog(strFileName, strLogMessage);
                                //WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, s2);
                            }
                        }


                        Tcl = ChangeTtoByte(Tre);
                        try
                        {
                            TComm.Write(Tcl, 0, Tcl.Length);
                        }
                        catch
                        { }

                    }
                }
            }
        }

        public bool Open(int MyId, int PortNo, string TBtl, Dictionary<int, Meter> dic_Tmp)
        {
            index = MyId;
            string[] TSub = new string[4];
            TSub = TBtl.Split('-');
            try
            {
                dic_Meter = dic_Tmp;
                if (TComm.IsOpen == true) TComm.Close();
                TComm.PortName = "COM" + string.Format("{0:0}", PortNo);
                TComm.BaudRate = int.Parse(TSub[0]);
                if (TSub[1] == "n" || TSub[1] == "N")
                    TComm.Parity = System.IO.Ports.Parity.None;
                else
                    TComm.Parity = System.IO.Ports.Parity.Even;
                TComm.DataBits = int.Parse(TSub[2]);
                TComm.StopBits = System.IO.Ports.StopBits.One;
                if (TComm.IsOpen == false) TComm.Open();

            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("串口有误！");
                return false;
            }
            return TComm.IsOpen;
        }

        public void Close()
        {
            if (TComm.IsOpen) TComm.Close();
        }

    }
}
