using LY.VirtualMeter.Core;
using LY.VirtualMeter.Core.CAN;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LY.VirtualMeter.ViewModel
{
    /// <summary>
    /// 终端对象
    /// </summary>
    public class Server : Function
    {

      private  MeterLogViewModel meterLog = new MeterLogViewModel();

        /// <summary>
        /// 终端日志
        /// </summary>
        public MeterLogViewModel MeterLog
        {
            get { return meterLog; }
            set { SetPropertyValue(value, ref meterLog, "MeterLog"); }
        }


  


        public delegate void EMoniBack(object sender, string Tdata, int Index);
        //public event EMoniBack ClEventMoni;


        public Thread TThread;
        public Socket TupdOne = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public Socket TupdOneInitPort = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private IPEndPoint Tlocat;
        private IPEndPoint Tremoto;
        private string g_Chen;
        public int CommPort;
       

        public string L_2018ReturnCmd;

        public string G_BTL;
        public string ZbAddress = "000000000099";
        public string strKeHuAddr = "00";//客户机地址
        public int intesamtype = 0;

        public bool RunFalg = false;//运行模式，false为采集器模式，ture为集中器模式
        public bool boolIsReturn = true;//是否返回数据

        //Dictionary<int, Meter> dic_Meter = new Dictionary<int, Meter>();
        ObservableCollection<Meter> MeterObject;

        public ProtocolAnalysis.Portocol_645 portocol_645 = new ProtocolAnalysis.Portocol_645();

        public ProtocolAnalysis.Analysis_698 portocol_698 = new ProtocolAnalysis.Analysis_698();


        public int BiaoType = 2; //协议类型,0为97，1为07，2为698,3为水表，4为气表，5为热表

        int Framelen = 1;

        int index = 0;

        System.Windows.Forms.Timer tm = new System.Windows.Forms.Timer();//定时器，定时对2018进行初始化
        DateTime timLastReceive = new DateTime();//最近一次接受数据数据时间

        /// <summary>
        /// 2018版本号
        /// </summary>
        byte _2018Ver = 0;
        public void AriseEventMn(string Data)
        {
            if (meterLog.Log.Length > 5000)
            {
                meterLog.Log = "";
            }
            meterLog.Log += Data+"\r\n";
        }
        public SerialPort TComm;
        //public Server(int MyId, int comm, string TRemoIp, int RomComm, int StartComm, string strCanParams, uint devInd, uint canId, Dictionary<int, Meter> dic_Tmp, byte _2018VerTmp)
        //{
        //    tm.Enabled = true;
        //    tm.Interval = 1000;
        //    //tm.Tick += tm_Tick;

        //    G_BTL = "2400-e-8-1";
        //    //G_BTL = "9600-e-8-1";

        //    dic_Meter = dic_Tmp;
        //    index = MyId;

        //    MeterLog.Name = "终端" + MyId;
        //    //MeterLog.Log = "终端" + MyId;
        //    CommPort = comm;
        //    _2018Ver = _2018VerTmp;

        //    if (_2018Ver == 3)
        //    {
        //        if (!App.DicSock.ContainsKey(comm))
        //        {
        //            CanCommuinication Cancom = new CanCommuinication(strCanParams, 14, devInd, canId, Convert.ToUInt16(comm));
        //            bool bValue = Cancom.Open();
        //            //Cancom.UpdateBltSetting("2400,e,8,1");
        //            App.DicSock.Add(comm, Cancom);
        //            TThread = new Thread(CanAcceptC);
        //            TThread.Start();
        //        }
        //    }
        //    else
        //    {
        //        string Tip; int Ti;
        //        Tip = Dns.GetHostName();
        //        IPHostEntry Tentryip = Dns.GetHostEntry(Tip);
        //        for (Ti = 0; Ti < Tentryip.AddressList.Length; Ti++)
        //        {
        //            if (Tentryip.AddressList[Ti].ToString().Split('.').Length == 4)
        //            {

        //                if (Tentryip.AddressList[Ti].ToString().Split('.')[0] == TRemoIp.Split('.')[0] && Tentryip.AddressList[Ti].ToString().Split('.')[1] == TRemoIp.Split('.')[1] && Tentryip.AddressList[Ti].ToString().Split('.')[2] == TRemoIp.Split('.')[2])
        //                    break;
        //            }
        //        }
        //        if (Ti == Tentryip.AddressList.Length)
        //            Tip = Tentryip.AddressList[0].ToString();
        //        else
        //            Tip = Tentryip.AddressList[Ti].ToString();
        //        if (Tip.Length < 7)
        //            Tip = "127.0.0.1";
        //        if (_2018Ver == 0)
        //            Tlocat = new IPEndPoint(IPAddress.Parse(Tip), StartComm + (comm - 1) * 2);
        //        else if (_2018Ver == 4)
        //            Tlocat = new IPEndPoint(IPAddress.Parse(Tip), StartComm + (comm - 1));
        //        else if (_2018Ver == 5)
        //        {
        //            //  建立串口通讯
        //            TComm = new SerialPort();
        //            TComm.DataReceived += new SerialDataReceivedEventHandler(TComm_DataReceived);
        //            TComm.BaudRate = 38400;
        //            TComm.DataBits = 8;
        //            TComm.Parity = System.IO.Ports.Parity.None;
        //            TComm.StopBits = System.IO.Ports.StopBits.One;
        //            TComm.DtrEnable = true;
        //            TComm.RtsEnable = true;
        //        }
        //        else
        //            Tlocat = new IPEndPoint(IPAddress.Parse(Tip), StartComm + (comm - 1) * 2);
        //        Tremoto = new IPEndPoint(IPAddress.Parse(TRemoIp), RomComm);
        //        if (_2018Ver == 4)
        //            Tremoto = new IPEndPoint(IPAddress.Parse(TRemoIp), RomComm + (comm - 1));
        //        IPEndPoint Tlocat2 = new IPEndPoint(IPAddress.Parse(Tip), StartComm + (comm - 1) * 2 + 1);
        //        try
        //        {
        //            TupdOne.Bind(Tlocat); ;
        //            if (_2018Ver != 0 && _2018Ver != 4)
        //                TupdOneInitPort.Bind(Tlocat2);
        //        }
        //        catch
        //        {
        //            AriseEventMn("联机不上！");
        //        }
        //        //TODO先不开
        //        //Open();
        //        if (_2018Ver == 5)
        //        { }
        //        else
        //        {
        //            TThread = new Thread(AcceptC);
        //            TThread.Start();
        //        }

        //        L_2018ReturnCmd = "";
        //        return;
        //    }
        //}

        public Server(int MyId, int comm, string TRemoIp, int RomComm, int StartComm, string strCanParams, uint devInd, uint canId, ObservableCollection<Meter> dic_Tmp, byte _2018VerTmp)
        {
            tm.Enabled = true;
            tm.Interval = 1000;
            //tm.Tick += tm_Tick;

            G_BTL = "9600-e-8-1";

            //TODO 测试使用
            //G_BTL = "2400-e-8-1";
            //BiaoType = 1;

            MeterObject = dic_Tmp;
            index = MyId;

            MeterLog.Name = "终端"+MyId;
            //MeterLog.Log = "终端" + MyId;
            CommPort = comm;
            _2018Ver = _2018VerTmp;

            if (_2018Ver == 3)
            {
                if (!App.DicSock.ContainsKey(comm))
                {
                    CanCommuinication Cancom = new CanCommuinication(strCanParams, 14, devInd, canId, Convert.ToUInt16(comm));
                    bool bValue = Cancom.Open();
                    //Cancom.UpdateBltSetting("2400,e,8,1");
                    App.DicSock.Add(comm, Cancom);
                    TThread = new Thread(CanAcceptC);
                    TThread.Start();
                }
            }
            else
            {
                string Tip; int Ti;
                Tip = Dns.GetHostName();
                IPHostEntry Tentryip = Dns.GetHostEntry(Tip);
                for (Ti = 0; Ti < Tentryip.AddressList.Length; Ti++)
                {
                    if (Tentryip.AddressList[Ti].ToString().Split('.').Length == 4)
                    {

                        if (Tentryip.AddressList[Ti].ToString().Split('.')[0] == TRemoIp.Split('.')[0] && Tentryip.AddressList[Ti].ToString().Split('.')[1] == TRemoIp.Split('.')[1] && Tentryip.AddressList[Ti].ToString().Split('.')[2] == TRemoIp.Split('.')[2])
                            break;
                    }
                }
                if (Ti == Tentryip.AddressList.Length)
                    Tip = Tentryip.AddressList[0].ToString();
                else
                    Tip = Tentryip.AddressList[Ti].ToString();
                if (Tip.Length < 7)
                    Tip = "127.0.0.1";
                if (_2018Ver == 0)
                    Tlocat = new IPEndPoint(IPAddress.Parse(Tip), StartComm + (comm - 1) * 2);
                else if (_2018Ver == 4)
                    Tlocat = new IPEndPoint(IPAddress.Parse(Tip), StartComm + (comm - 1));
                else if (_2018Ver == 5)
                {
                  //  建立串口通讯
                    TComm = new SerialPort();
                    TComm.DataReceived += new SerialDataReceivedEventHandler(TComm_DataReceived);
                    TComm.BaudRate = 38400;
                    TComm.DataBits = 8;
                    TComm.Parity = System.IO.Ports.Parity.None;
                    TComm.StopBits = System.IO.Ports.StopBits.One;
                    TComm.DtrEnable = true;
                    TComm.RtsEnable = true;
                }
                else
                    Tlocat = new IPEndPoint(IPAddress.Parse(Tip), StartComm + (comm - 1) * 2);
                Tremoto = new IPEndPoint(IPAddress.Parse(TRemoIp), RomComm);
                if (_2018Ver == 4)
                    Tremoto = new IPEndPoint(IPAddress.Parse(TRemoIp), RomComm + (comm - 1));
                IPEndPoint Tlocat2 = new IPEndPoint(IPAddress.Parse(Tip), StartComm + (comm - 1) * 2 + 1);
                try
                {
                    TupdOne.Bind(Tlocat); ;
                    if (_2018Ver != 0 && _2018Ver != 4)
                        TupdOneInitPort.Bind(Tlocat2);
                }
                catch
                {
                    AriseEventMn("联机不上！");
                }
                ////TODO 先不开
                if (Open())
                {
                    AriseEventMn("开启端口监听成功");
                }
                else
                {
                    AriseEventMn("开启端口监听失败");
                }


                if (_2018Ver == 5)
                { }
                else
                {
                    TThread = new Thread(AcceptC);
                    TThread.Start();
                }

                L_2018ReturnCmd = "";
                return;
            }
        }

        /// <summary>
        /// 监视端口的数据--收到数据进行转发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TComm_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            byte[] TInBuffer = new byte[0];        //本口用于接收数据的临时缓冲区
            int TInBufferSize = 0;
            string Tstr = "";
            while (TInBufferSize != TComm.BytesToRead)
            {
                TInBufferSize = TComm.BytesToRead;
                //Delay(5);
            }

            TInBufferSize = TComm.BytesToRead;
            TInBuffer = new byte[TInBufferSize];
            TComm.Read(TInBuffer, 0, TInBufferSize);

            for (int i = 0; i < TInBufferSize; i++)
            {
                Tstr += Convert.ToString(TInBuffer[i], 16).PadLeft(2, '0');
            }
            //AriseEventMn(Tstr);
            //TComm.Write(TInBuffer, 0, TInBuffer.Length);
            //return;
            string input = "";
            //Debug.Print((Tstr.Length / 2).ToString());
            input = strCache + Tstr;
            //AriseEventMn("接收数据：" + input);
            if (input.Length > 0)
            {
                strCache = GetOneFrameAndFillListCache(input);
            }
            //可能需要将回复数据放在这里可以避免没有回复数据
 

            if (ListCache.Count > 0)
             {
                for (   int i = ListCache.Count - 1; i >= 0; i--)
                {
                    g_Chen = ListCache.Dequeue();
                    if (BiaoType == 1 || BiaoType == 0)
                        SCoreSock();
                    else
                        SCoreSock698();
                }

            }
        }

        void tm_Tick(object sender, EventArgs e)
        {
            tm.Enabled = false;
            if ((DateTime.Now.Ticks / 10000000 - timLastReceive.Ticks / 10000000) > 120)
            {
                timLastReceive = DateTime.Now;
                Open();
            }
        }


        string strCache = "";

        /// <summary>
        /// 存储完整帧的帧队列
        /// </summary>
        public Queue<string> ListCache = new Queue<string>();

        public string GetOneFrameAndFillListCache(string frmStr)
        {
            try
            {
                if (BiaoType == 1 || BiaoType == 0)
                {
                    #region 645解析报文
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
                    #endregion
                }
                else
                {
                    #region 698.45

                    frmStr = frmStr.ToUpper();
                    if (frmStr.IndexOf("68") < 0)
                        return "";
                    else
                    {
                        frmStr = frmStr.Substring(frmStr.IndexOf("68"));
                        if (frmStr.Length >= 10)
                        {
                            int ilen = Convert.ToInt16(frmStr.Substring(2, 2), 16) + Convert.ToInt16(frmStr.Substring(4, 2), 16) * 256;
                            int addresslen = ((Convert.ToInt16(frmStr.Substring(8, 2), 16) << 5) % 256 / 32) + 1;
                            if (frmStr.Length > 16 + addresslen * 2)
                            {
                                long crc16 = CheckCrc16(frmStr.Substring(2, 10 + addresslen * 2), 5 + addresslen);
                                long crc16Tmp = Convert.ToInt32(frmStr.Substring(12 + addresslen * 2, 2), 16) + Convert.ToInt32(frmStr.Substring(14 + addresslen * 2, 2), 16) * 256;
                                if (crc16 == crc16Tmp)
                                {
                                    if (frmStr.Length >= 4 + ilen * 2)
                                    {
                                        if (frmStr.Substring(2 + ilen * 2, 2) == "16")
                                        {
                                            crc16 = CheckCrc16(frmStr.Substring(2, ilen * 2 - 4), ilen - 2);
                                            crc16Tmp = Convert.ToInt32(frmStr.Substring(ilen * 2 - 2, 2), 16) + Convert.ToInt32(frmStr.Substring(ilen * 2, 2), 16) * 256;
                                            if (crc16 == crc16Tmp)
                                            {
                                                ListCache.Enqueue(frmStr.Substring(0, ilen * 2 + 4));
                                                frmStr = frmStr.Remove(0, frmStr.Substring(0, ilen * 2 + 4).Length);
                                                frmStr = GetOneFrameAndFillListCache(frmStr);
                                            }
                                            else
                                            {
                                                frmStr = frmStr.Remove(0, 2);
                                                frmStr = GetOneFrameAndFillListCache(frmStr);
                                            }
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
                    }

                    return frmStr;
                    #endregion
                }
            }
            catch
            {
                frmStr = "";
                return frmStr;
            }
        }

        object obj = new object();
        private void CanAcceptC()
        {
            byte[] Tbyte;
            while (true)
            {
                Thread.Sleep(30);

                Tbyte = new byte[1024];

                string input = "";
                string Tstr = "";
                Tstr = "";

                lock (obj)
                {
                    if (App.RevcieBuffs.ContainsKey(Convert.ToUInt16(CommPort)))
                    {
                        if (App.RevcieBuffs[Convert.ToUInt16(CommPort)].Count > 0)
                        {
                            Tbyte = App.RevcieBuffs[Convert.ToUInt16(CommPort)].ToArray();
                            App.RevcieBuffs[Convert.ToUInt16(CommPort)].Clear();

                            for (int i = 0; i < Tbyte.Length; i++)
                            {
                                Tstr += Convert.ToString(Tbyte[i], 16).PadLeft(2, '0');
                            }
                        }
                    }
                }

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
                        if (BiaoType == 1 || BiaoType == 0)
                            SCoreSock();
                        else
                            SCoreSock698();
                    }
                }


                //lock (obj)
                //{
                //    if (App.RevcieBuffs.ContainsKey(Convert.ToUInt16(CommPort)))
                //    {
                //        if (App.RevcieBuffs[Convert.ToUInt16(CommPort)].Count > 0)
                //        {
                //            Tbyte = App.RevcieBuffs[Convert.ToUInt16(CommPort)].ToArray();
                //            App.RevcieBuffs[Convert.ToUInt16(CommPort)].Clear();

                //            for (int i = 0; i < Tbyte.Length; i++)
                //            {
                //                Tstr += Convert.ToString(Tbyte[i], 16).PadLeft(2, '0');
                //            }


                //            input = strCache + Tstr;

                //            if (input.Length > 0)
                //            {
                //                strCache = GetOneFrameAndFillListCache(input);
                //            }

                //            if (ListCache.Count > 0)
                //            {
                //                for (int i = ListCache.Count - 1; i >= 0; i--)
                //                {
                //                    g_Chen = ListCache.Dequeue();

                //                    SCoreSock();
                //                }
                //            }
                //        }
                //    }
                //}
            }
        }

        private void AcceptC()
        {
            byte[] Tbyte;
            int Tlen = 0;
            while (true)
            {


                Tbyte = new byte[1024];

                try
                {
                    Tlen = TupdOne.Receive(Tbyte);
                }
                catch
                {
                    //AriseEventMn("通信错误！");
                    //if (AppData.BaseData.IsLog)
                    //    WriteText("AcceptC:通信错误！");
                    //break;
                }

                string input = "";
                string Tstr = "";
                Tstr = "";

                if (_2018Ver == 0)
                {
                    int Ti;
                    Tstr = System.Text.Encoding.ASCII.GetString(Tbyte);

                    Ti = Tstr.IndexOf(",>");
                    if (Ti != 0)
                    {

                        Tstr = Tstr.Substring(0, Ti + 1);
                        try
                        {
                            switch (g_GetItem(Tstr, 3, ","))
                            {
                                case "resetinit":
                                    AriseEventMn("初始化成功！");
                                    break;
                                case "open":
                                    AriseEventMn("打开端口成功！");
                                    break;
                                case "close":
                                    AriseEventMn("关闭端口成功！");
                                    break;
                                case "hello":
                                    AriseEventMn("连接成功！");
                                    break;
                                case "ask":
                                    if (g_GetItem(Tstr, 2, ",") == "cl2018")
                                    {
                                        Tstr = g_GetItem(Tstr, 7, ",");
                                        Tstr = Tstr.Substring(4, Tstr.Length - 4);
                                    }
                                    break;
                            }

                        }
                        catch { break; }
                    }
                }
                else
                {

                    for (int i = 0; i < Tlen; i++)
                    {
                        Tstr += Convert.ToString(Tbyte[i], 16).PadLeft(2, '0');
                    }
                }

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
                        if (BiaoType == 1 || BiaoType == 0)
                            SCoreSock();
                        else
                            SCoreSock698();
                    }

                }

            }

        }

        public void test1()
        {
            g_Chen = "682C004305010000000000007137100008050100302B070000011022222222222222222222222222222222B6F116";
            SCoreSock698();
        }

        private void SCoreSock698()
        {
            int Tint; string Tstr; string Tre;
            int Tint2 = 0; string strTmpBdz = "";
            byte[] Tcl;




            strTmpBdz = revStr(g_Chen.Substring(2, 12));
            timLastReceive = DateTime.Now;

            if (g_Chen.ToUpper().IndexOf("AAAAAAAAAAAA") > 0)
            {
                string sls = ZbAddress.Substring(0, 12);
                if (g_Chen.Substring(16, 2) == "13")
                {
                    g_Chen = "68" + revStr(sls) + g_Chen.Substring(14, 6);
                    g_Chen = g_Chen + getChkSum(g_Chen) + "16";
                }
                else
                {
                    //g_Chen = "68" + revStr(sls) + g_Chen.Substring(14, 14);
                    //g_Chen = g_Chen + getChkSum(g_Chen) + "16";
                    g_Chen = g_Chen.Replace("AAAAAAAAAAAA", revStr(sls));

                }
                strTmpBdz = ZbAddress;
            }

            if (g_Chen.ToUpper().IndexOf("999999999999") > 0)
            {
                string sls = ZbAddress.Substring(0, 12);
                g_Chen = "68" + revStr(sls) + g_Chen.Substring(14, 10);
                g_Chen = g_Chen + getChkSum(g_Chen) + "16";
                strTmpBdz = ZbAddress;
            }
            //Tint = Convert.ToInt32(g_Chen.Substring(18, 2), 16);
            Tint = 0;
            //if ((g_Chen.Length) >= (Tint + 12) * 2)
            {
                Tstr = g_Chen;
                if (AppData.BaseData.IsLog) WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, DateTime.Now.ToString("HH:mm:ss fff ") + "收:" + Tstr + " [" + MeterObject[0].DateBron() + "]");
                AriseEventMn(DateTime.Now.ToString("HH:mm:ss fff ") + "收:" + Tstr);
                //string s1 = ""; string s2 = ""; string s3 = ""; bool b1 = false; bool b2 = false;
                string s1 = ""; string[] s2 =new string[0]; string[] s3 =new string[0]; bool b1 = false;
                string s4 = "";

                if (AppData.BaseData.IsAnalysis)
                {
                    portocol_698.Analysis(Tstr,ref s2,ref s1, ref s3);
                    s4 = s1 +"\r\n"+ string.Join("\r\n", s3);
        
                    AriseEventMn(s4);
         //ProtocolAnalysis.Protocol_698_45 portocol_698 = new ProtocolAnalysis.Protocol_698_45();
         //           s2= portocol_698.Analysis(Tstr);
                    if (AppData.BaseData.IsLog) WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, s4);
                }
                //if (Tstr.Substring(Tstr.Length - 2, 2) == "16")//  Right(Tstr, 2) = 16)
                {
                    g_Chen = "";



                    if (RunFalg)
                    {
                        if (MeterObject[0] != null)
                        {
                            if (strTmpBdz.Substring(10, 2) == index.ToString().PadLeft(2, '0'))
                            {
                                if (BiaoType == 1)
                                    Tre = MeterObject[0].CodeReturn2007(Tstr, ZbAddress);
                                else if (BiaoType == 0)
                                    Tre = MeterObject[0].CodeReturn1997(Tstr, ZbAddress);
                                else
                                    Tre = MeterObject[0].CodeReturn698(Tstr, ZbAddress, intesamtype);
                                if (boolIsReturn)
                                {
                                    if (AppData.BaseData.IsLog) WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, DateTime.Now.ToString("HH:mm:ss fff ") + "发:" + Tre + " [" + MeterObject[0].DateBron() + "]");
                                    AriseEventMn(DateTime.Now.ToString("HH:mm:ss fff ") + "发:" + Tre);
                                    if (AppData.BaseData.IsAnalysis)
                                    {
                                        portocol_698.Analysis(Tre, ref s2, ref s1, ref s3);
                                        s4 = s1 + "\r\n" + string.Join("\r\n", s3);
                                        AriseEventMn(s4);
                                        if (AppData.BaseData.IsLog) WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, s4);
                                    }
                                }
                                if (boolIsReturn)
                                {
                                    if (_2018Ver == 0)
                                    {
                                        string Tcmd = "<cl2018 ,comserver ,ask ,pn ,pcom" + string.Format("{0:0}", CommPort) + " ,data" + Tre + " ,>";
                                        Tcl = System.Text.Encoding.ASCII.GetBytes(Tcmd);
                                        TupdOne.SendTo(Tcl, Tremoto);
                                    }
                                    else if (_2018Ver == 3)
                                    {
                                        byte[] b_ = new byte[Tre.Length / 2];
                                        for (int k_ = 0; k_ < b_.Length; k_++)
                                        {
                                            b_[k_] = Convert.ToByte(Tre.Substring(k_ * 2, 2), 16);
                                        }

                                        App.DicSock[CommPort].SendData(ref b_, false, 0, 18);
                                    }
                                    else if (_2018Ver == 1)
                                    {

                                        try
                                        {
                                            string msg = "";

                                            //msg = "reset";
                                            //Tcl = System.Text.Encoding.ASCII.GetBytes(msg);
                                            //TupdOneInitPort.SendTo(Tcl, Tremoto);
                                            //Delay(10);

                                            msg = "init " + G_BTL.Replace('-', ' ');
                                            Tcl = System.Text.Encoding.ASCII.GetBytes(msg);
                                            TupdOneInitPort.SendTo(Tcl, Tremoto);
                                            Delay(10);
                                            Tcl = ChangeTtoByte(Tre);
                                            TupdOne.SendTo(Tcl, Tremoto);
                                        }
                                        catch
                                        { }
                                    }
                                    else if (_2018Ver == 4)
                                    {
                                        try
                                        {
                                            Tcl = ChangeTtoByte(Tre);
                                            TupdOne.SendTo(Tcl, Tremoto);
                                        }
                                        catch
                                        { }
                                    }
                                    else if (_2018Ver == 5)
                                    {
                                        Tcl = ChangeTtoByte(Tre);
                                        TComm.Write(Tcl, 0, Tcl.Length);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {

                        //Meter Meter = null;
                        //for (int i = 1; i <= dic_Meter.Count; i++)
                        //{
                        //    if (dic_Meter[i].BAddress.IndexOf(strTmpBdz) > -1)
                        //    {
                        //        Meter = dic_Meter[i];
                        //        break;
                        //    }
                        //}

                        Meter Meter = MeterObject[0];

                        if (Meter != null)
                        {
                            if (BiaoType == 1)
                                Tre = Meter.CodeReturn2007(Tstr, ZbAddress);
                            else if (BiaoType == 0)
                                Tre = Meter.CodeReturn1997(Tstr, ZbAddress);
                            else
                                Tre = Meter.CodeReturn698(Tstr, ZbAddress, intesamtype);
                            if (boolIsReturn)
                            {
                                if (AppData.BaseData.IsLog) WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, DateTime.Now.ToString("HH:mm:ss fff ") + "发:" + Tre + "[" + Meter.DateBron() + "]");
                                AriseEventMn(DateTime.Now.ToString("HH:mm:ss fff ") + "发:" + Tre);
                            }
                            if (AppData.BaseData.IsAnalysis)
                            {
              
                                portocol_698.Analysis(Tre, ref s2, ref s1, ref s3);
                                s4 = s1 + "\r\n" + string.Join("\r\n", s3);
                                AriseEventMn(s4);
                                if (AppData.BaseData.IsLog) WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, s4);
                            }
                            if (boolIsReturn)
                            {
                                if (_2018Ver == 0)
                                {
                                    string Tcmd = "<cl2018 ,comserver ,ask ,pn ,pcom" + string.Format("{0:0}", CommPort) + " ,data" + Tre + " ,>";
                                    Tcl = System.Text.Encoding.ASCII.GetBytes(Tcmd);
                                    TupdOne.SendTo(Tcl, Tremoto);
                                }
                                else if (_2018Ver == 3)
                                {
                                    byte[] b_ = new byte[Tre.Length / 2];
                                    for (int k_ = 0; k_ < b_.Length; k_++)
                                    {
                                        b_[k_] = Convert.ToByte(Tre.Substring(k_ * 2, 2), 16);
                                    }

                                    App.DicSock[CommPort].SendData(ref b_, false, 0, 18);
                                }
                                else if (_2018Ver == 1)
                                {

                                    try
                                    {
                                        string msg = "";

                                        //msg = "reset";
                                        //Tcl = System.Text.Encoding.ASCII.GetBytes(msg);
                                        //TupdOneInitPort.SendTo(Tcl, Tremoto);
                                        //Delay(10);

                                        msg = "init " + G_BTL.Replace('-', ' ');
                                        Tcl = System.Text.Encoding.ASCII.GetBytes(msg);
                                        TupdOneInitPort.SendTo(Tcl, Tremoto);
                                        Delay(10);
                                        Tcl = ChangeTtoByte(Tre);
                                        TupdOne.SendTo(Tcl, Tremoto);
                                    }
                                    catch
                                    { }
                                }
                                else if (_2018Ver == 4)
                                {
                                    try
                                    {
                                        Tcl = ChangeTtoByte(Tre);
                                        TupdOne.SendTo(Tcl, Tremoto);
                                    }
                                    catch
                                    { }
                                }
                                else if (_2018Ver == 5)
                                {
                                    Tcl = ChangeTtoByte(Tre);
                                    TComm.Write(Tcl, 0, Tcl.Length);
                                }
                            }
                        }
                    }

                }
            }
        }

        public void testComm(string strdata)
        {
            g_Chen = strdata;
            SCoreSock698();
        }

        private void SCoreSock()
        {
            try
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
                timLastReceive = DateTime.Now;

                if (g_Chen.ToUpper().IndexOf("AAAAAAAAAAAA") > 0)
                {
                    string sls = ZbAddress.Substring(0, 12);
                    if (g_Chen.Substring(16, 2) == "13")
                    {
                        g_Chen = "68" + revStr(sls) + g_Chen.Substring(14, 6);
                        g_Chen = g_Chen + getChkSum(g_Chen) + "16";
                    }
                    else
                    {
                        g_Chen = "68" + revStr(sls) + g_Chen.Substring(14, 14);
                        g_Chen = g_Chen + getChkSum(g_Chen) + "16";
                    }
                    strTmpBdz = ZbAddress;
                }

                if (g_Chen.ToUpper().IndexOf("999999999999") > 0)
                {
                    string sls = ZbAddress.Substring(0, 12);
                    g_Chen = "68" + revStr(sls) + g_Chen.Substring(14, 10);
                    g_Chen = g_Chen + getChkSum(g_Chen) + "16";
                    strTmpBdz = ZbAddress;
                }
                Tint = Convert.ToInt32(g_Chen.Substring(18, 2), 16);
                if ((g_Chen.Length) >= (Tint + 12) * 2)
                {
                    Tstr = g_Chen.Substring(0, (Tint + 12) * 2);
                    if (AppData.BaseData.IsLog)
                    {
                        //string strFileName = "Meter" + index;
                        //string strLogMessage = DateTime.Now.ToString("HH:mm:ss fff ") + "收:" + Tstr + " [" + dic_Meter[1].DateBron() + "]";
                        //LogInfoHelper.WriteLog(strFileName, strLogMessage);
                        WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, DateTime.Now.ToString("HH:mm:ss fff ") + "收:" + Tstr + " [" + MeterObject[0].DateBron() + "]");
                    }
                    AriseEventMn(DateTime.Now.ToString("HH:mm:ss fff ") + "收" + Framelen + ":" + Tstr);
                    Framelen++;
                    string s1 = ""; string s2 = ""; string s3 = ""; bool b1 = false; bool b2 = false;
                    if (AppData.BaseData.IsAnalysis)
                    {
                        portocol_645.Analysis(Tstr, "11", ref s1, ref s2, ref b1, ref b2, ref s3);
                        AriseEventMn(s2);

                        if (AppData.BaseData.IsLog)
                        {
                            string strFileName = "Meter" + index;

                            //LogInfoHelper.WriteLog(strFileName, s2);
                            WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, s2);
                        }
                    }
                    if (Tstr.Substring(Tstr.Length - 2, 2) == "16")//  Right(Tstr, 2) = 16)
                    {
                        g_Chen = "";



                        if (RunFalg)
                        {
                            if (MeterObject[0] != null)
                            {
                                if (strTmpBdz.Substring(10, 2) == index.ToString().PadLeft(2, '0'))
                                {
                                    if (BiaoType == 1)
                                        Tre = MeterObject[0].CodeReturn2007(Tstr, ZbAddress);
                                    else
                                        Tre = MeterObject[0].CodeReturn1997(Tstr, ZbAddress);
                                    if (AppData.BaseData.IsLog)
                                    {
                                        //string strFileName = "Meter" + index;
                                        //string strLogMessage = DateTime.Now.ToString("HH:mm:ss fff ") + "发:" + Tstr + " [" + dic_Meter[1].DateBron() + "]";
                                        //LogInfoHelper.WriteLog(strFileName, strLogMessage);
                                        WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, DateTime.Now.ToString("HH:mm:ss fff ") + "发:" + Tre + " [" + MeterObject[0].DateBron() + "]");
                                    }
                                    AriseEventMn(DateTime.Now.ToString("HH:mm:ss fff ") + "发:" + Tre);
                                    if (AppData.BaseData.IsAnalysis)
                                    {
                                        portocol_645.Analysis(Tre, "11", ref s1, ref s2, ref b1, ref b2, ref s3);
                                        AriseEventMn(s2);
                                        if (AppData.BaseData.IsLog)
                                        {
                                            string strFileName = "Meter" + index;
                                            LogInfoHelper.WriteLog(strFileName, s2);
                                        }
                                    }

                                    if (_2018Ver == 0)
                                    {
                                        string Tcmd = "<cl2018 ,comserver ,ask ,pn ,pcom" + string.Format("{0:0}", CommPort) + " ,data" + Tre + " ,>";
                                        Tcl = System.Text.Encoding.ASCII.GetBytes(Tcmd);
                                        TupdOne.SendTo(Tcl, Tremoto);
                                    }
                                    else if (_2018Ver == 1)
                                    {

                                        try
                                        {
                                            string msg = "";

                                            //msg = "reset";
                                            //Tcl = System.Text.Encoding.ASCII.GetBytes(msg);
                                            //TupdOneInitPort.SendTo(Tcl, Tremoto);
                                            //Delay(10);

                                            msg = "init " + G_BTL.Replace('-', ' ');
                                            Tcl = System.Text.Encoding.ASCII.GetBytes(msg);
                                            TupdOneInitPort.SendTo(Tcl, Tremoto);
                                            Delay(10);
                                            Tcl = ChangeTtoByte(Tre);
                                            TupdOne.SendTo(Tcl, Tremoto);
                                        }
                                        catch
                                        { }
                                    }
                                    else if (_2018Ver == 4)
                                    {
                                        try
                                        {
                                            Tcl = ChangeTtoByte(Tre);
                                            TupdOne.SendTo(Tcl, Tremoto);
                                        }
                                        catch
                                        { }
                                    }
                                    else if (_2018Ver == 5)
                                    {
                                        Tcl = ChangeTtoByte(Tre);
                                        TComm.Write(Tcl, 0, Tcl.Length);
                                    }
                                }
                            }
                        }
                        else
                        {

                            Meter Meter = null;
                            for (int i = 1; i <= MeterObject.Count; i++)
                            {
                                if (MeterObject[i-1].BAddress.IndexOf(strTmpBdz) > -1)
                                {
                                    Meter = MeterObject[i-1];
                                    break;
                                }
                            }

                            if (Meter != null)
                            {
                                if (BiaoType == 1)
                                    Tre = Meter.CodeReturn2007(Tstr, ZbAddress);
                                else
                                    Tre = Meter.CodeReturn1997(Tstr, ZbAddress);
                                if (AppData.BaseData.IsLog)
                                {
                                    //string strFileName = "Meter" + index;
                                    //string strLogMessage = DateTime.Now.ToString("HH:mm:ss fff ") + "发:" + Tstr + " [" + Meter.DateBron() + "]";
                                    //LogInfoHelper.WriteLog(strFileName, strLogMessage);
                                    WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, DateTime.Now.ToString("HH:mm:ss fff ") + "发:" + Tre + "[" + Meter.DateBron() + "]");
                                }
                                AriseEventMn(DateTime.Now.ToString("HH:mm:ss fff ") + "发:" + Tre);
                                if (AppData.BaseData.IsAnalysis)
                                {
                                    portocol_645.Analysis(Tre, "11", ref s1, ref s2, ref b1, ref b2, ref s3);
                                    AriseEventMn(s2);
                                    if (AppData.BaseData.IsLog)
                                    {
                                        //string strFileName = "Meter" + index;
                                        //LogInfoHelper.WriteLog(strFileName, s2);
                                        WriteLog("Log\\虚拟表日志\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Meter" + index, s2);
                                    }
                                }

                                if (_2018Ver == 0)
                                {
                                    string Tcmd = "<cl2018 ,comserver ,ask ,pn ,pcom" + string.Format("{0:0}", CommPort) + " ,data" + Tre + " ,>";
                                    Tcl = System.Text.Encoding.ASCII.GetBytes(Tcmd);
                                    TupdOne.SendTo(Tcl, Tremoto);
                                }
                                else if (_2018Ver == 3)
                                {
                                    byte[] b_ = new byte[Tre.Length / 2];
                                    for (int k_ = 0; k_ < b_.Length; k_++)
                                    {
                                        b_[k_] = Convert.ToByte(Tre.Substring(k_ * 2, 2), 16);
                                    }

                                    App.DicSock[CommPort].SendData(ref b_, false, 0, 18);
                                }
                                else if (_2018Ver == 1)
                                {
                                    try
                                    {
                                        string msg = "";
                                        //msg = "reset";
                                        //Tcl = System.Text.Encoding.ASCII.GetBytes(msg);
                                        //TupdOneInitPort.SendTo(Tcl, Tremoto);
                                        //Delay(10);

                                        msg = "init " + G_BTL.Replace('-', ' ');
                                        Tcl = System.Text.Encoding.ASCII.GetBytes(msg);
                                        TupdOneInitPort.SendTo(Tcl, Tremoto);
                                        Delay(10);
                                        Tcl = ChangeTtoByte(Tre);
                                        TupdOne.SendTo(Tcl, Tremoto);
                                    }
                                    catch
                                    { }
                                }
                                else if (_2018Ver == 4)
                                {
                                    try
                                    {
                                        Tcl = ChangeTtoByte(Tre);
                                        TupdOne.SendTo(Tcl, Tremoto);
                                    }
                                    catch
                                    { }
                                }
                                else if (_2018Ver == 5)
                                {
                                    Tcl = ChangeTtoByte(Tre);
                                    TComm.Write(Tcl, 0, Tcl.Length);
                                }
                            }
                        }

                    }
                }
            }
            catch
            {
                g_Chen = "";
            }
        }

        public bool Open()
        {
            string Tcmd;
            byte[] Tcl;
            try
            {
                if (_2018Ver == 0)
                {
                    Tcmd = "<cl2018 ,comserver ,hello ,py ,>";
                    Tcl = System.Text.Encoding.ASCII.GetBytes(Tcmd);
                    TupdOne.SendTo(Tcl, Tremoto);
                    Delay(100);

                    Tcmd = "<cl2018 ,comserver ,close ,py ,pcom" + string.Format("{0:0}", CommPort) + " ,>";
                    Tcl = System.Text.Encoding.ASCII.GetBytes(Tcmd);
                    TupdOne.SendTo(Tcl, Tremoto);
                    Delay(100);

                    Tcmd = "<cl2018 ,comserver ,open ,py ,pcom" + string.Format("{0:0}", CommPort) + " ,pdir1 ,>";
                    Tcl = System.Text.Encoding.ASCII.GetBytes(Tcmd);
                    TupdOne.SendTo(Tcl, Tremoto);
                    Delay(100);

                    Tcmd = "<cl2018 ,comserver ,init ,py ,pcom" + string.Format("{0:0}", CommPort) + " ,p" + G_BTL + " ,>";

                    Tcl = System.Text.Encoding.ASCII.GetBytes(Tcmd);
                    TupdOne.SendTo(Tcl, Tremoto);
                    Delay(100);
                }
                else if (_2018Ver == 3)
                {
                    Thread.Sleep(30);
                    App.DicSock[CommPort].UpdateBltSetting(G_BTL);
                }
                else if (_2018Ver == 4)
                {
                    TupdOne.SendTo(new byte[] { 0, 1 }, Tremoto);
                }
                else if (_2018Ver == 5)
                {
                    //根据设置参数配置串口并打开
                    string[] TSub = new string[4];
                    TSub = G_BTL.Split('-');
                    if (TComm.IsOpen == true) TComm.Close();
                    TComm.PortName = "COM" + string.Format("{0:0}", CommPort);
                    TComm.BaudRate = int.Parse(TSub[0]);
                    if (TSub[1] == "n" || TSub[1] == "N")
                        TComm.Parity = System.IO.Ports.Parity.None;
                    else
                        TComm.Parity = System.IO.Ports.Parity.Even;
                    TComm.DataBits = int.Parse(TSub[2]);
                    TComm.StopBits = System.IO.Ports.StopBits.One;
                    if (TComm.IsOpen == false)
                    {
                        TComm.Open();
                        AriseEventMn(TComm.PortName +"-" + TComm.BaudRate+"-");
                    } 
                    //Thread.Sleep(200);
                 

                }
                else
                {
                    //WriteLog("test", "开始" + G_BTL);
                    string msg = "";
                    //msg = "reset";
                    //Tcl = System.Text.Encoding.ASCII.GetBytes(msg);
                    //TupdOneInitPort.SendTo(Tcl, Tremoto);
                    //Delay(10);

                    msg = "init " + G_BTL.Replace('-', ' '); ;
                    Tcl = System.Text.Encoding.ASCII.GetBytes(msg);
                    //WriteLog("test", "开始" + Tremoto.Address + "," + Tremoto.Port);
                    //WriteLog("test", "开始" + TupdOneInitPort.LocalEndPoint);
                    TupdOneInitPort.SendTo(Tcl, Tremoto);
                    //WriteLog("test", "开始" + TupdOne.LocalEndPoint);
                    TupdOne.SendTo(new byte[] { 0, 1 }, Tremoto);
                    //WriteLog("test", "结束");
                }
                return true;

            }
            catch
            {
                return false;
            }
        }

        public bool Colse()
        {
            //string Tcmd;
            //byte[] Tcl;

            try
            {
                //Tcmd = "<cl2018 ,comserver ,close ,py ,pcom" + string.Format("{0:0}", CommPort) + " ,>";
                //Tcl = System.Text.Encoding.ASCII.GetBytes(Tcmd);
                ////Tudp.Send(Tcl, Tcl.Length);
                //TupdOne.SendTo(Tcl, Tremoto);
                //Delay(10);
                //State = CST_CLOSE;
                //Ttimer2.Enabled = false;
                return true;

            }
            catch
            {
                return false;
            }
        }
    }
}
