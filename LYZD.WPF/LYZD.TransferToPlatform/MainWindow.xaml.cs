using LYZD.Core.Enum;
using LYZD.Mis.Common;
using LYZD.TerminalProtocol.GW;
using LYZD.TransferToPlatform.DevicesFunc;
using LYZD.TransferToPlatform.SocketEven;
using LYZD.TransferToPlatform.Test;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using LYZD.ViewModel.Device;
using LYZD.ViewModel.InputPara;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LYZD.TransferToPlatform
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑s
    /// </summary>
    public partial class MainWindow : Window
    {
        IPEndPoint RemoteTlocat;//检定软件
        IPEndPoint RemoteTlocatCamera;//视频信息

        public Queue<EventsSendData> queueSendData = new Queue<EventsSendData>();//设置指令集合
        public static Queue<EventsReceiveData> queueReceiveData = new Queue<EventsReceiveData>();//接收指令集合

        object obj = new object();

        public bool DelayFlag = false;

        Dictionary<int, Rs485Sate> dic485Sate = new Dictionary<int, Rs485Sate>();

        Dictionary<string, SP_PortType> Sp_PortDic = new Dictionary<string, SP_PortType>();

        Dictionary<string, PulseOutputType> dicPulseOutput = new Dictionary<string, PulseOutputType>();

        List<bool> PortIsOpen = new List<bool>();

        public MainWindow()
        {
            InitializeComponent();
            //ShiTry();
            //string tg = getASCIItoStr("4c593131353137");
            //EventsSendData cmds = new EventsSendData();
            //cmds.CMD = "CMD=0104";
            //cmds.Data = "DATA=0;2;7;115200-E-8-1";
            //CMD0104(cmds);
            //添加台体注册信息码文件
            //string asw = "NoResMac,0000000000000001";
            //string d = asw.Substring(9, 16);
            //string str_OutFrame = "68 92 00 C3 05 99 00 78 00 33 10 10 9E F5 85 03 30 60 12 03 00 02 00 20 2A 02 00 01 30 00 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 01 03 55 07 05 00 00 00 00 00 01 01 03 06 00 00 00 02 1C 07 E7 0A 13 17 32 00 1C 07 E7 0A 13 17 37 00 55 07 05 00 00 00 00 00 02 01 03 06 00 00 00 02 1C 07 E7 0A 13 17 32 00 1C 07 E7 0A 13 17 37 00 55 07 05 00 00 00 00 00 03 01 03 06 00 00 00 02 1C 07 E7 0A 13 17 32 00 1C 07 E7 0A 13 17 37 00 00 00 F0 CB 16 2 02 06 00 00 01 72 06 00 00 00 25 02 02 06 00 00 01 04 06 00 00 00 1A 02 02 06 00 00 01 40 06 00 00 00 20 00 00 01 00 04 B3 59 AE 38 4A 45 16 ";
            //string[] AnalysisedString = new string[0];
            //string AlalysisedData = null;
            //string[] AnalysisedStruce = new string[0];
            //Analysis_698 analysis_698 = new Analysis_698();
            //bool a = analysis_698.Analysis(str_OutFrame, ref AnalysisedString, ref AlalysisedData, ref AnalysisedStruce);
            //string b = "";
            //foreach (string item in AnalysisedString)
            //{
            //    b += item + "\n";
            //}

            LogtestEven.LTest += LogtestEven_LTest;

            SerPortEven.setEven += SerPortEven_setEven;
            TerminalEven.Terminal += TerminalEven_Terminal;

            DevOrderEven.DevOrder += DevOrderEven_DevOrder;

            if (!LoadIni.LoadIni.LoadIniInfo())
            {
                return;
            }

            startThread();
            EquipmentData.DeviceManager.LoadDevices();

            InitShakeSattus();

            VerifyBase.meterInfo = EquipmentData.MeterGroupInfo.GetVerifyMeterInfo();
            Task.Factory.StartNew(() =>
            {
                Link();
                Thread.Sleep(3000);
                CloseMeterRelay();
            });
            Task.Factory.StartNew(() =>
            {
                InitSp_Port();
                InitList();
            });
        }
        private static string SubString698(string data)
        {
            data = data.Replace("-", "").Replace(" ", "");//清除所有空格
            if (data.IndexOf("68") == -1) return "";//没有帧头的
            data = data.Substring(data.IndexOf("68"));
            if (data.Length < 10) return "";//没有帧头的
            //获取他的长度
            int ilen = Convert.ToInt32(data.Substring(4, 2) + data.Substring(2, 2), 16) + 2;
            if (data.Length < ilen * 2) return "";
            if (data.Substring(ilen * 2 - 2, 2) != "16") return "";//没有帧尾
            int endIndex = ilen * 2;
            return data.Substring(0, endIndex);
        }
        public static string getASCIItoStr(string str)
        {
            byte[] bb = Hex2Bytes(str, false);
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            string strCharacter = asciiEncoding.GetString(bb);
            Console.WriteLine(strCharacter);
            return strCharacter;
        }
        //先将16进制ASCII码转字节数组
        public static byte[] Hex2Bytes(string sHex, bool isExchange)
        {
            if (sHex == null || sHex.Length == 0)
                return null;
            sHex = sHex.Length % 2 == 0 ? sHex : "0" + sHex;
            byte[] bRtns = new byte[sHex.Length / 2];
            for (int i = 0; i < bRtns.Length; i++)
            {
                if (isExchange)
                    bRtns[bRtns.Length - 1 - i] = Convert.ToByte(sHex.Substring(i * 2, 2), 16);
                else
                    bRtns[i] = Convert.ToByte(sHex.Substring(i * 2, 2), 16);
            }
            return bRtns;
        }





        private void DevOrderEven_DevOrder(string GetSetMsg)
        {
            DevOrderlogShow(DateTime.Now.ToString() + "|" + GetSetMsg + "\n");
        }
        public static object SetSendLock = new object();
        private void SerPortEven_setEven(byte[] GetSetMsg, string spCom)
        {
            int ComNum = Convert.ToInt32(spCom.Remove(0, 3));

            if (GetSetMsg.Length > 10)
            {
                if (ComNum > LoadIni.LoadIni.Port4851End && ComNum < LoadIni.LoadIni.Port4852End)
                {
                    string PortMsg = ByteToHexStr(GetSetMsg);
                    TerminalEven.TerminalMsg("4852来自终端：" + ComNum.ToString() + "Com:" + PortMsg);
                    List<string> SplitSmg = BackSplitMsg(PortMsg);
                    foreach (string item in SplitSmg)
                    {
                        Thread.Sleep(100);
                        lock (SetSendLock)
                        {
                            //queueReceiveData.Enqueue(new EventsReceiveData() { CMD = "cmd=1001", Ret = null, Data = (ComNum - LoadIni.LoadIni.Port4851End).ToString() + ";" + ByteToHexStr(GetSetMsg) }); ;
                            queueReceiveData.Enqueue(new EventsReceiveData() { CMD = "cmd=1001", Ret = "ret=0,", Data = (ComNum - LoadIni.LoadIni.Port4851End).ToString() + ";" + item });
                        }
                    }
                }
                else if (ComNum > LoadIni.LoadIni.Port4852End)
                {
                    string PortMsg = ByteToHexStr(GetSetMsg);
                    TerminalEven.TerminalMsg("Can数据来自终端：" + ComNum.ToString() + "Com:" + PortMsg);
                    List<string> SplitSmg = BackSplitMsg(PortMsg);
                    foreach (string item in SplitSmg)
                    {
                        Thread.Sleep(100);
                        if (item.Substring(0, 2) == "68")
                        {
                            lock (SetSendLock)
                            {
                                //queueReceiveData.Enqueue(new EventsReceiveData() { CMD = "cmd=1001", Ret = null, Data = (ComNum - LoadIni.LoadIni.Port4851End).ToString() + ";" + ByteToHexStr(GetSetMsg) }); ;
                                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = "cmd=1006", Ret = null, Data = (ComNum - LoadIni.LoadIni.Port4852End).ToString() + ";1;0;0;00000000;" + item });
                            }
                        }
                    }
                }
                else
                {
                    TerminalEven.TerminalMsg("4851来自终端：" + ComNum.ToString() + "Com:" + ByteToHexStr(GetSetMsg));
                    lock (SetSendLock)
                    {
                        queueReceiveData.Enqueue(new EventsReceiveData() { CMD = "cmd=1002", Ret = null, Data = ComNum.ToString() + ";" + ByteToHexStr(GetSetMsg) }); ;
                    }
                }

            }
        }


        private void LogtestEven_LTest(string GetSetMsg)
        {
            //logShow(DateTime.Now.ToString() + "|" + GetSetMsg + "\n");

            logShow(DateTime.Now.ToString() + "|" + GetSetMsg);
        }

        private void TerminalEven_Terminal(string GetSetMsg)
        {
            TerminallogShow(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ">" + GetSetMsg + "\n");
        }

        private void startThread()
        {

            RemoteTlocat = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11240);

            IPEndPoint local = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 20000);
            receiveUdpClient = new UdpClient(local);

            Thread t = new Thread(AcceptC)
            {
                IsBackground = true
            };
            t.IsBackground = true;
            t.Start();

            LogtestEven.add("开始接收，端口与地址为" + local.Address.ToString() + ":" + local.Port);

            Thread t2 = new Thread(AcceptC2)
            {
                IsBackground = true
            };
            t2.IsBackground = true;
            t2.Start();
            Thread t3 = new Thread(AcceptC3)
            {
                IsBackground = true
            };
            t3.IsBackground = true;
            t3.Start();


        }

        private void InitShakeSattus()
        {
            ShakeStatusInfo.ShakeStatus = new List<string>();
            for (int i = 0; i < 16; i++)
            {
                ShakeStatusInfo.ShakeStatus.Add("00000000");
            }
            LogtestEven.add("遥信全部关闭");
        }


        #region 发送——接受——处理

        /// <summary>接收用</summary>
        private UdpClient receiveUdpClient;

        private void AcceptC()
        {

            while (true)
            {
                try
                {
                    IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                    byte[] Tbyte = receiveUdpClient.Receive(ref remote);


                    RemoteTlocat = remote;
                    string data = Encoding.UTF8.GetString(Tbyte, 0, Tbyte.Length);
                    if (data == "") return;

                    string[] arr = data.Split(',');
                    string cmd = arr[0];
                    string Edata = null;
                    for (int i = 1; i < arr.Length; i++)
                    {
                        Edata += arr[i];
                    }
                    if (DelayFlag == false)
                    {
                        lock (obj)
                        {
                            queueSendData.Enqueue(new EventsSendData() { CMD = cmd, Data = Edata });
                        }
                        string msg = string.Format("From PC:{0}", data);
                        LogtestEven.add(msg);
                    }
                }
                catch { }
            }
        }

        private static object RevLock = new object();
        /// <summary>
        /// 返回数据到上位机
        /// </summary>
        private void AcceptC2()
        {
            while (true)
            {
                try
                {
                    if (queueReceiveData.Count > 0)
                    {
                        lock (RevLock)
                        {
                            EventsReceiveData cmd = queueReceiveData.Dequeue();
                            if (cmd == null) continue;

                            //string msg = string.Format("{0},{1},ret=0,data={2}", cmd.LastCMD, cmd.SN, cmd.Data);

                            string msg = string.Format("{0},{1}{2}", cmd.CMD, cmd.Ret, "data=" + cmd.Data);

                            receiveUdpClient.Send(Encoding.UTF8.GetBytes(msg), Encoding.UTF8.GetBytes(msg).Length, RemoteTlocat);

                            msg = string.Format("To PC:{0}", msg);
                            LogtestEven.add(msg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogtestEven.add(ex.ToString());
                    return;
                }
            }
        }

        private void ShiTry()
        {
            string a = "aa 00 64 68 3F 00 43 04 00 00 00 00 8D 01 40 02 02 00 00 05 00 00 00 00 10 F1 01 00 02 00 20 00 68 05 00 00 00 00 10 68 11 04 34 34 33 37 CC 16 68 05 00 00 00 00 10 68 11 04 35 34 33 37 CD 16 A0 16";
            a += " 68 9C 00 43 00 00 00 00 00 47 11 01 00 14 01 00 00 00 00 10 02 04 00 00 00 00 10 02 07 00 00 00 00 10 02 30 00 00 00 00 10 03 33 00 00 00 00 10 03 36 00 00 00 00 10 03 39 00 00 00 00 10 03 22 00 00 00 00 10 03 25 00 00 00 00 10 03 28 00 00 00 00 10 03 40 00 00 00 00 10 03 11 00 00 00 00 10 03 14 00 00 00 00 10 03 17 00 00 00 00 10 03 03 00 00 00 00 10 02 06 00 00 00 00 10 02 09 00 00 00 00 10 02 32 00 00 00 00 10 03 35 00 00 00 00 10 03 38 00 00 00 00 10 03 A0 16";
            a += " 00 23 45";
            a += " 68 17 00 43 05 08 00 00 00 00 00 10 1f b7 05 01 00 43 00 03 00 00 46 58 16";
            string u = "002345";
            List<string> y = BackSplitMsg(u);
        }

        /// <summary>
        /// 处理有可能连在一起的报文
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private List<string> BackSplitMsg(string msg)
        {
            List<string> BackMsg = new List<string>();
            if (msg.Contains("68") && msg.Length > 2)
            {
                msg = msg.Substring(msg.IndexOf("68"));
                byte[] AllByte = ToHexByteArray(msg);
                int Lengthindex = AllByte[1];
                int Indexnum = 0;
                List<byte[]> t = new List<byte[]>();
                List<byte> newbyte = new List<byte>();
                if (Lengthindex <= AllByte.Count())
                {
                    foreach (byte item in AllByte)
                    {
                        //添加字节到数组
                        newbyte.Add(item);
                        Indexnum++;
                        if (Indexnum == Lengthindex && item == 22)
                        {
                            //数组添加到list
                            t.Add(newbyte.ToArray());
                            //如果后续还有
                            if (Indexnum < AllByte.Count())
                            {
                                //清理byte[]数组
                                newbyte.Clear();
                                //判断下一位byte是否是“68”即byte104
                                if (AllByte[Indexnum] == 104)
                                {
                                    Lengthindex = AllByte[Indexnum + 1] + Indexnum;
                                }
                            }
                        }
                        else if (Indexnum == AllByte.Count() && newbyte.Count() > 0)
                        {
                            t.Add(newbyte.ToArray());
                        }
                    }
                }


                foreach (var item in t)
                {
                    BackMsg.Add(ByteToHexStr(item));
                }
            }
            else
            {
                BackMsg.Add(msg);
            }

            return BackMsg;
        }

        private void AcceptC3()
        {
            while (true)
            {
                Thread.Sleep(5);
                if (queueSendData.Count <= 0) continue;

                EventsSendData cmd = queueSendData.Dequeue();
                switch (cmd.CMD)
                {
                    case "cmd=0000":
                        ShiTry();
                        break;
                    case "cmd=0104":
                        CMD0104(cmd);
                        break;
                    case "cmd=0101":
                    //CMD0101(cmd);
                    //break;
                    case "cmd=0102":
                    case "cmd=0103":


                        lock (RevLock)
                        {
                            queueReceiveData.Enqueue(new EventsReceiveData() { CMD = cmd.CMD, Ret = "ret=0,", Data = "null" });
                        }
                        break;
                    case "cmd=0105":
                        CMD0105(cmd);
                        break;
                    case "cmd=0201":
                        CMD0201(cmd);
                        break;
                    case "cmd=0202":
                        CMD0202(cmd);
                        break;
                    case "cmd=0203":
                        CMD0203(cmd);
                        break;
                    case "cmd=0204":
                        CMD0204(cmd);
                        break;
                    case "cmd=0205":
                        CMD0205(cmd);
                        break;
                    case "cmd=0206":
                        CMD0205(cmd);
                        break;
                    case "cmd=0301":
                        CMD0301(cmd);
                        break;
                    case "cmd=0401":
                        CMD0401(cmd);
                        break;
                    case "cmd=0402":
                        CMD0402(cmd);
                        break;
                    case "cmd=0403":
                        CMD0403(cmd);
                        break;
                    case "cmd=0404":
                        CMD0404(cmd);
                        break;
                    case "cmd=0405":
                        CMD0405(cmd);
                        break;
                    case "cmd=0406":
                        CMD0406(cmd);
                        break;
                    case "cmd=0407":
                        CMD0407(cmd);
                        break;
                    case "cmd=0408":
                        CMD0408(cmd);
                        break;
                    case "cmd=0409":
                        CMD0409(cmd);
                        break;
                    case "cmd=0411":
                        CMD0411(cmd);
                        break;
                    case "cmd=0412":
                        CMD0412(cmd);
                        break;
                    case "cmd=0413":
                        CMD0413(cmd);
                        break;
                    case "cmd=040A":
                        CMD040A(cmd);
                        break;
                    case "cmd=040B":
                        CMD040B(cmd);
                        break;
                    case "cmd=040C":
                        CMD040C(cmd);
                        break;
                    case "cmd=040D":
                        CMD040D(cmd);
                        break;
                    case "cmd=0422":
                        CMD0422(cmd);
                        break;
                    case "cmd=0510":
                        CMD0510(cmd);
                        break;
                    case "cmd=1001":
                        CMD1001(cmd);
                        break;
                    case "cmd=1002":
                        CMD1002(cmd);
                        break;
                    case "cmd=1006":
                        CMD1006(cmd);
                        break;

                }
            }
        }


        #region //指令方法

        private void CMD0101(object ob)
        {

            EventsSendData evdata = ob as EventsSendData;

            Link();
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "0", Data = "null" }); ;
            }
            //Task.Factory.StartNew(()=> {
            //    //连接加密机
            //    EquipmentData.DeviceManager.LinkDog();
            //});

        }



        private void CMD0104(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            string[] Data = evdata.Data.Remove(0, 5).Split(';');
            string D4 = Data[3];

            #region Log输出指令
            string D1 = "";
            switch (Data[0])
            {
                case "0":
                    D1 = "终端232";
                    break;
                case "1":
                    D1 = "485-1";
                    break;
                case "2":
                    D1 = "485-2";
                    break;
                case "3":
                    D1 = "备用";
                    break;
            }
            string D2 = "";
            switch (Data[1])
            {
                case "0":
                    D2 = "645 协议";
                    break;
                case "1":
                    D2 = "376.1协议";
                    break;
                case "2":
                    D2 = "698.45协议";
                    break;
                case "4":
                    D2 = "376.2 协议";
                    break;
            }
            string D3 = "";
            switch (Data[2])
            {
                case "0":
                    D3 = "所有串口";
                    break;
                default:
                    D3 = Data[2] + "表位";
                    break;
            }


            LogtestEven.add("串口类型：" + D1 + "  协议类型：" + D2 + " 多路服务器串口号:" + D3 + "波特率：" + D4);

            if (D1 == "485-1")
            {
                SockPool.Instance.UpdatePortSetting(Convert.ToInt32(Data[2]) + "-485-1", D4);

                Thread.Sleep(100);
                if (PortIsOpen[Convert.ToInt32(Data[2]) - 1] == true)//端口开启的情况下发点数据刷新波特率
                {
                    SockPool.Instance.Send(Convert.ToInt32(Data[2]) + "-485-1", ToHexByteArray("000000"));
                }
            }
            else if (D1 == "485-2")
            {
                SockPool.Instance.UpdatePortSetting((Convert.ToInt32(Data[2]) + LoadIni.LoadIni.Port4851End).ToString() + "-485-2", D4);
                EquipmentData.DeviceManager.ControlConnrRelay3(0, (byte)Convert.ToInt32(Data[2]), Convert.ToInt32(Data[2]) > LoadIni.LoadIni.DeviceID ? 1 : 0);
                Thread.Sleep(100);
                if (PortIsOpen[Convert.ToInt32(Data[2]) + LoadIni.LoadIni.Port4851End - 1] == true)//端口开启的情况下发点数据刷新波特率
                {
                    SockPool.Instance.Send((Convert.ToInt32(Data[2]) + LoadIni.LoadIni.Port4851End - 1).ToString() + "-485-2", ToHexByteArray("000000"));
                }
            }
            else if (D1 == "终端232")
            {

                SockPool.Instance.UpdatePortSetting((Convert.ToInt32(Data[2]) + LoadIni.LoadIni.Port4851End).ToString() + "-485-2", D4);
                EquipmentData.DeviceManager.ControlConnrRelay3(3, (byte)Convert.ToInt32(Data[2]), Convert.ToInt32(Data[2]) > LoadIni.LoadIni.DeviceID ? 1 : 0);
                Thread.Sleep(100);
                if (PortIsOpen[Convert.ToInt32(Data[2]) + LoadIni.LoadIni.Port4851End - 1] == true)//端口开启的情况下发点数据刷新波特率
                {
                    SockPool.Instance.Send((Convert.ToInt32(Data[2]) + LoadIni.LoadIni.Port4851End).ToString() + "-485-2", ToHexByteArray("000000"));
                }

            }

            #endregion
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
        }
        /// <summary>
        /// 常用升源//数据格式cmd=0201,data=0;63;220;1.5;0;50;0
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0201(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            string[] Data = evdata.Data.Split(';');
            PowerWay Pw;
            string MeterPowerType;
            GetPowerAndType(Data[0], out Pw, out MeterPowerType);
            Dictionary<int, string> dicInt = new Dictionary<int, string>();

            string hex = Convert.ToString(Convert.ToInt32(Data[1]), 2).PadLeft(6, '0');

            dicInt = HxeToInt(hex.ToString());

            double U = Convert.ToDouble(Data[2]);

            double I = Convert.ToDouble(Data[3]);

            float Freq = float.Parse(Data[5]);
            Power.PowerOn(MeterPowerType, dicInt, U, I, Pw, Freq, float.Parse(Data[4]));
            Thread.Sleep(13000);
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
        }

        private static int CanConType = 0;
        /// <summary>
        /// 关cmd=0105
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0105(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;
            string[] RecData = evdata.Data.Remove(0, 5).Split(';');
            CanConType = Convert.ToInt32(RecData[0]);
            int EpitopeNo = Convert.ToInt32(RecData[2]);
            string ChangeInCans = RecData[3];
            SetCanBote(EpitopeNo, ChangeInCans);
            Thread.Sleep(1000);
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
            bool[] a = new bool[60];

        }


        private void CMD0202(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            string[] Data = evdata.Data.Split(';');
            PowerWay Pw;
            string MeterPowerType;
            GetPowerAndType(Data[0], out Pw, out MeterPowerType);

            Dictionary<int, string> dicInt = new Dictionary<int, string>();

            string hex = Convert.ToString(Convert.ToInt32(Data[1]), 2).PadLeft(6, '0'); ;

            dicInt = HxeToInt(hex.ToString());

            float Freq = float.Parse(Data[2]);

            Power.PowerOnHigh(MeterPowerType, dicInt, Freq, Data);
            Thread.Sleep(13000);
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }

        }

        /// <summary>
        /// 关源;cmd=0203
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0203(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;
            Power.PowerOff();
            Thread.Sleep(13000);
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
            bool[] a = new bool[60];

        }

        /// <summary>
        /// 设置谐波含量
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0204(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;
            string[] Data = evdata.Data.Split(';');

            string HamrsData = evdata.Data.Remove(0, 7);

            Power.Harmonic(Data[0], HamrsData.Split(';'));

            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
        }

        /// <summary>
        /// 设置零线电流_CMD=0205;DATA=1.0
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0205(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;
            SetControlMeterRelay(false);
            double Data = Convert.ToDouble(evdata.Data.Remove(0, 5));

            Power.ZeroLineAn(00, 01);
            Dictionary<int, string> dicInt = new Dictionary<int, string>();
            dicInt = HxeToInt("111100");

            Thread.Sleep(2000);

            SetControlMeterRelay(true);

            Power.PowerOn("三相四线", dicInt, 220.0, Data, PowerWay.正向有功, 50.0F, 0.0f);
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
        }

        /// <summary>
        /// 设置零线电流_CMD=0206;DATA=1.0
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0206(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;
            Power.PowerOff();
            Power.ZeroLineAn(01, 00);
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
        }
        /// <summary>
        /// 读取标准表
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0301(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            StandardTable.readStandardTable();

            string data = EquipmentData.StdInfo.Ua.ToString("F3") + ";" + EquipmentData.StdInfo.PhaseUa.ToString("F3") + ";" + EquipmentData.StdInfo.Ub.ToString("F3") + ";" + EquipmentData.StdInfo.PhaseUb.ToString("F3") + ";"
                + EquipmentData.StdInfo.Uc.ToString("F3") + ";" + EquipmentData.StdInfo.PhaseUc.ToString("F3") + ";" + EquipmentData.StdInfo.Ia.ToString("F3") + ";" + EquipmentData.StdInfo.PhaseIa.ToString("F3") + ";"
                + EquipmentData.StdInfo.Ib.ToString("F3") + ";" + EquipmentData.StdInfo.PhaseIb.ToString("F3") + ";" + EquipmentData.StdInfo.Ic.ToString("F3") + ";" + EquipmentData.StdInfo.PhaseIc.ToString("F3") + ";"
                + EquipmentData.StdInfo.P.ToString("F3") + ";" + EquipmentData.StdInfo.Q.ToString("F3") + ";" + EquipmentData.StdInfo.PF.ToString("F3") + ";" + EquipmentData.StdInfo.Freq.ToString("F3");
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = data }); ;
            }
        }
        /// <summary>
        /// 切换485
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0401(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            string HamrsData = evdata.Data.Remove(0, 5);

            string[] strobj = HamrsData.Split(';');




            Rs485Sate rs = new Rs485Sate();
            switch (strobj[1])
            {
                case "1":
                    rs.YaoJSate = true;
                    break;
                case "0":
                    rs.YaoJSate = false;
                    break;
                default:
                    rs.YaoJSate = false;
                    break;
            }
            int ConType = 0;
            switch (strobj[2])
            {
                case "01":
                    ConType = 0;
                    rs.TheRs485Sate = true;
                    break;
                case "00":
                    ConType = 3;
                    rs.TheRs485Sate = false;
                    break;
                default:
                    ConType = 3;
                    rs.TheRs485Sate = false;
                    break;
            }

            if (strobj[1] == "1")
            {
                SockPool.Instance.OpenSpCom(Convert.ToInt32(strobj[0]).ToString() + "-485-1");
                SockPool.Instance.OpenSpCom((Convert.ToInt32(strobj[0]) + LoadIni.LoadIni.Port4851End).ToString() + "-485-2");
                SockPool.Instance.OpenSpCom((Convert.ToInt32(strobj[0]) + LoadIni.LoadIni.Port4852End).ToString() + "-Can");
                PortIsOpen[Convert.ToInt32(strobj[0]) - 1] = true;
                PortIsOpen[Convert.ToInt32(strobj[0]) + LoadIni.LoadIni.Port4851End - 1] = true;
                EquipmentData.DeviceManager.ControlMeterRelay(1, Convert.ToByte(strobj[0]), Convert.ToInt32(strobj[0]) > LoadIni.LoadIni.DeviceID ? 1 : 0);
            }
            else
            {
                SockPool.Instance.CloseSpCom(Convert.ToInt32(strobj[0]).ToString() + "-485-1");
                SockPool.Instance.CloseSpCom((Convert.ToInt32(strobj[0]) + LoadIni.LoadIni.Port4851End).ToString() + "-485-2");
                SockPool.Instance.CloseSpCom((Convert.ToInt32(strobj[0]) + LoadIni.LoadIni.Port4852End).ToString() + "-Can");
                PortIsOpen[Convert.ToInt32(strobj[0]) - 1] = false;
                PortIsOpen[Convert.ToInt32(strobj[0]) + LoadIni.LoadIni.Port4851End - 1] = false;
                EquipmentData.DeviceManager.ControlMeterRelay(2, Convert.ToByte(strobj[0]), Convert.ToInt32(strobj[0]) > LoadIni.LoadIni.DeviceID ? 1 : 0);
            }



            EquipmentData.DeviceManager.ControlConnrRelay3(ConType, Convert.ToByte(strobj[0]), Convert.ToInt32(strobj[0]) > LoadIni.LoadIni.DeviceID ? 1 : 0);

            if (dic485Sate.Count > 0 && dic485Sate.ContainsKey(Convert.ToInt32(strobj[0])))
            {
                foreach (var item in dic485Sate)
                {
                    if (Convert.ToInt32(strobj[0]) == item.Key)
                    {
                        item.Value.YaoJSate = rs.YaoJSate;
                        item.Value.TheRs485Sate = rs.TheRs485Sate;
                    }
                }
            }
            else
            {
                dic485Sate.Add(Convert.ToInt32(strobj[0]), rs);
            }



            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
        }

        /// <summary>
        /// 读取遥信状态
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0402(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            int EpitopeNo = Convert.ToInt32(evdata.Data.Remove(0, 5));

            //遥信状态
            string RevData = Convert.ToInt32(ShakeStatusInfo.ShakeStatus[EpitopeNo - 1], 2).ToString("x2"); ;

            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = RevData }); ;
            }

        }

        /// <summary>
        /// 设置遥信状态
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0403(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;



            string[] RecData = evdata.Data.Remove(0, 5).Split(';');

            int EpitopeNo = Convert.ToInt32(RecData[0]);

            DevOrderEven.DevOrderMsg("设置遥信状态:表位：" + EpitopeNo.ToString() + "上位机指令：" + evdata.CMD.ToString() + "," + evdata.Data.ToString());
            ShakeStatusInfo.ShakeStatus[EpitopeNo - 1] = Convert.ToString(Convert.ToInt32(RecData[1], 16), 2).PadLeft(8, '0');

            ShakeMessStatus.SETShhakeSate(EpitopeNo);
            //dicInt = HexToIntEight(ShakeSate);

            //ShakeMessStatus.SETShhakeSate(EpitopeNo, dicInt);

            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }

        }

        /// <summary>
        /// 读取遥控状态
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0404(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;
            int EpitopeNo = Convert.ToInt32(evdata.Data.Remove(0, 5));

            int[] OutTriggerMode;
            int[] OutPutValue;

            ShakeMessStatus.ReadRemote(EpitopeNo, out OutTriggerMode, out OutPutValue);
            string Type = null;
            foreach (var item in OutTriggerMode)
            {
                if (item == 0)
                {
                    Type = "00";
                }
                else
                {
                    Type = "01";
                }
            }

            string OutData = null;
            foreach (var item in OutPutValue)
            {
                if (item == 0)
                {
                    OutData += "0";
                }
                else
                {
                    OutData += "1";
                }
            }

            string hex = Convert.ToInt32(OutData, 2).ToString("x2");
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = Type + hex }); ;
            }
        }

        private void CMD0405(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            string[] RecData = evdata.Data.Remove(0, 5).Split(';');

            int EpitopeNo = Convert.ToInt32(RecData[0]);

            Dictionary<int, string> dic = new Dictionary<int, string>();

            dic = HxeToInt(Convert.ToString(Convert.ToInt32(RecData[1].Remove(0, 2)), 2), 5);

            //ShakeMessStatus.SetRemote(EpitopeNo-1, dic);

            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
        }


        private void CMD0406(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            string[] data = evdata.Data.Remove(0, 5).Split(';');

            int EpitopeNo = Convert.ToInt32(data[0]);

            foreach (var item in dicPulseOutput)
            {
                if (EpitopeNo.ToString() == item.Key)
                {
                    switch (data[1])
                    {
                        case "5":
                            item.Value.GuoupOneFreq = float.Parse((float.Parse(data[2]) / (float.Parse(data[3]) * 60)).ToString("F3"));
                            item.Value.GuoupOneNum = Convert.ToInt32(data[2]);
                            item.Value.GuoupOnePWM = float.Parse(data[4]) / 100;
                            break;
                        case "7":
                            item.Value.GuoupTweFreq = float.Parse((float.Parse(data[2]) / (float.Parse(data[3]) * 60)).ToString("F3"));
                            item.Value.GuoupTweNum = Convert.ToInt32(data[2]);
                            item.Value.GuoupTwePWM = float.Parse(data[4]) / 100;
                            break;
                        default:
                            item.Value.GuoupOneFreq = float.Parse((float.Parse(data[2]) / (float.Parse(data[3]) * 60)).ToString("F3"));
                            item.Value.GuoupOneNum = Convert.ToInt32(data[2]);
                            item.Value.GuoupOnePWM = float.Parse(data[4]) / 100;
                            item.Value.GuoupTweFreq = float.Parse((float.Parse(data[2]) / (float.Parse(data[3]) * 60)).ToString("F3"));
                            item.Value.GuoupTweNum = Convert.ToInt32(data[2]);
                            item.Value.GuoupTwePWM = float.Parse(data[4]) / 100;
                            break;
                    }
                    item.Value.contrnlType = 0x03;
                }
            }

            DevOrderEven.DevOrderMsg("设置脉冲状态:表位：" + EpitopeNo.ToString() + "上位机指令：" + evdata.CMD.ToString() + "," + evdata.Data.ToString());

            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
            //EquipmentData.DeviceManager.SetPulseOutput(ControlType, (byte)(pos + 1), fq1, PWM1, PulseNum1, fq2, PWM2, PulseNum2, ID);
        }
        private void CMD0407(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            string[] data = evdata.Data.Remove(0, 5).Split(';');

            int EpitopeNo = Convert.ToInt32(data[0]);
            byte contrnlType = 0x03;

            if (dicPulseOutput.ContainsKey(EpitopeNo.ToString()))
            {
                PulseOutputType pulseOutput;
                dicPulseOutput.TryGetValue(EpitopeNo.ToString(), out pulseOutput);
                PulseOutputContorl.SETShhakeSate(pulseOutput);
            }



            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
        }
        private void CMD0408(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            string[] data = evdata.Data.Remove(0, 5).Split(';');

            int EpitopeNo = Convert.ToInt32(data[0]);

            if (dicPulseOutput.ContainsKey(EpitopeNo.ToString()))
            {
                PulseOutputType pulseOutput;
                dicPulseOutput.TryGetValue(EpitopeNo.ToString(), out pulseOutput);
                pulseOutput.contrnlType = 0x00;
                PulseOutputContorl.SETShhakeSate(pulseOutput);
            }

            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
        }

        /// <summary>
        /// 电流回路复位
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0409(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            int EpitopeNo = Convert.ToInt32(evdata.Data.Remove(0, 5));
            SetControlMeterRelay(EpitopeNo, 1);


            //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)1, 0);
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
        }

        private static object objs = new object();
        private void CMD0411(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            string[] data = evdata.Data.Remove(0, 5).Split(';');

            int EpitopeNo = Convert.ToInt32(data[0]);
            int cs = Convert.ToInt32(data[2]);
            /// 设置误差版标准常数
            PulseOutputContorl.SetStandardConst(EpitopeNo);
            Thread.Sleep(500);

            PulseOutputContorl.SetTestedConst(EpitopeNo, Convert.ToInt32(data[1]));

            PulseOutputContorl.StartWcb(EpitopeNo);

            ReadWs(EpitopeNo, cs);
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }

        }


        List<List<string>> l = new List<List<string>>();
        /// <summary>
        /// 初始化日记时误差数组
        /// </summary>
        private void InitList()
        {
            for (int i = 0; i < 16; i++)
            {
                List<string> s = new List<string>();

                l.Add(s);
            }
        }

        /// <summary>
        /// 读取日记时误差
        /// </summary>
        /// <param name="EpitopeNo"></param>
        /// <param name="cs"></param>
        private void ReadWs(int EpitopeNo, int cs)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    int Ln = cs;

                    DevicesFunc.stError S = new DevicesFunc.stError();
                    S = PulseOutputContorl.ReadWcbData(EpitopeNo);

                    if (l[EpitopeNo - 1].Count == 0 && float.Parse(S.szError) != 0.0f)
                    {
                        lock (objs)
                        {
                            l[EpitopeNo - 1].Add(S.szError);
                        }
                    }

                    if (l[EpitopeNo - 1].Count < S.Index)
                    {
                        lock (objs)
                        {
                            l[EpitopeNo - 1].Add(S.szError);
                        }
                    }


                    if (l[EpitopeNo - 1].Count >= Ln)
                    {
                        break;
                    }
                    Thread.Sleep(2000);
                }

            });
        }

        private static object ClearObj = new object();
        /// <summary>
        /// 停止误差版_日记时
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0412(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            int EpitopeNo = Convert.ToInt32(evdata.Data.Remove(0, 5));
            PulseOutputContorl.StopWcb(EpitopeNo);

            lock (ClearObj)
            {
                l[EpitopeNo].Clear();
            }


            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
        }

        private void CMD0413(object ob)
        {

            EventsSendData evdata = ob as EventsSendData;

            int EpitopeNo = Convert.ToInt32(evdata.Data.Remove(0, 5));
            float Err = 0.0f;
            lock (objs)
            {
                if (l[EpitopeNo - 1].Count > 0)
                {
                    float SumErr = 0;
                    foreach (var item in l[EpitopeNo - 1])
                    {
                        SumErr += float.Parse(float.Parse(item).ToString("F5"));
                    }
                    Err = SumErr / l[EpitopeNo - 1].Count * 24 * 60;
                }
                lock (ClearObj)
                {
                    l[EpitopeNo - 1].Clear();
                }
            }

            if (Err != 0.0f)
            {
                lock (RevLock)
                {
                    queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = Err.ToString("F1") }); ;
                }
            }
            else
            {
                lock (RevLock)
                {
                    queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=1,", Data = "null" }); ;
                }
            }


        }

        /// <summary>
        /// 电流回路断开
        /// </summary>
        /// <param name="ob"></param>
        private void CMD040A(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            int EpitopeNo = Convert.ToInt32(evdata.Data.Remove(0, 5));
            SetControlMeterRelay(EpitopeNo, 2);


            //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)1, 0);
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
        }

        /// <summary>
        /// 电流回路闭合
        /// </summary>
        /// <param name="ob"></param>
        private void CMD040B(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            int EpitopeNo = Convert.ToInt32(evdata.Data.Remove(0, 5));
            SetControlMeterRelay(EpitopeNo, 1);


            //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)1, 0);
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
        }

        /// <summary>
        /// 获取485终端状态
        /// </summary>
        /// <param name="ob"></param>
        private void CMD040C(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            int EpitopeNo = Convert.ToInt32(evdata.Data.Remove(0, 5));

            string Rs485 = "00";

            if (dic485Sate.Count > 0)
            {

                foreach (var item in dic485Sate)
                {
                    if (item.Key == EpitopeNo)
                    {
                        Rs485 = item.Value.TheRs485Sate ? "01" : "00";
                    }
                }
            }

            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = Rs485 }); ;
            }
        }

        /// <summary>
        /// 设置485终端状态
        /// </summary>
        /// <param name="ob"></param>
        private void CMD040D(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;

            string[] RecData = evdata.Data.Remove(0, 5).Split(';');

            int EpitopeNo = Convert.ToInt32(RecData[0]);

            bool ConType = RecData[1] == "01" ? true : false;

            if (!ConType)
            {
                SockPool.Instance.CloseSpCom(EpitopeNo + "-485-1");
                Thread.Sleep(1000);
                SockPool.Instance.CloseSpCom((EpitopeNo + LoadIni.LoadIni.Port4851End) + "-485-2");
            }
            else
            {
                SockPool.Instance.OpenSpCom(EpitopeNo + "-485-1");
                Thread.Sleep(1000);
                SockPool.Instance.OpenSpCom((EpitopeNo + LoadIni.LoadIni.Port4851End) + "-485-2");
            }

            //SetControlConnrRelay3(ConType, EpitopeNo);
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }

        }

        /// <summary>
        /// 切换4852/CAN
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0422(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;
            string[] RecData = evdata.Data.Remove(0, 5).Split(';');
            int EpitopeNo = Convert.ToInt32(RecData[0]);
            int ChangeInCans = Convert.ToInt32(RecData[1]);
            ChangeInCan(ChangeInCans, EpitopeNo);

            Thread.Sleep(2000);
            //ChangeInCan
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "null" }); ;
            }
        }

        /// <summary>
        /// 台体注册信息
        /// </summary>
        /// <param name="ob"></param>
        private void CMD0510(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;
            lock (RevLock)
            {
                queueReceiveData.Enqueue(new EventsReceiveData() { CMD = evdata.CMD, Ret = "ret=0,", Data = "0;0;" + LoadIni.LoadIni.SoftwareCode }); ;
            }
        }

        private void CMD1001(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;
            string[] Data = null;

            Data = evdata.Data.Remove(0, 5).Split(';');

            int EpitopeNo = Convert.ToInt32(Data[0]);
            string SPname = "1-485-1";
            switch (Data[1])
            {
                case "0":
                    SPname = (EpitopeNo + LoadIni.LoadIni.Port4851End) + "-485-2";
                    EquipmentData.DeviceManager.ControlConnrRelay3(3, (byte)EpitopeNo, EpitopeNo > LoadIni.LoadIni.DeviceID ? 1 : 0);
                    break;
                case "1":
                    SPname = EpitopeNo + "-485-1";
                    EquipmentData.DeviceManager.ControlConnrRelay3(0, (byte)EpitopeNo, EpitopeNo > LoadIni.LoadIni.DeviceID ? 1 : 0);
                    break;
                case "2":
                case "3":
                default:
                    SPname = EpitopeNo + "-485-1";
                    EquipmentData.DeviceManager.ControlConnrRelay3(0, (byte)EpitopeNo, EpitopeNo > LoadIni.LoadIni.DeviceID ? 1 : 0);
                    break;
            }
            //byte[] outData = null;
            
            if (!string.IsNullOrEmpty(Data[1]))
            {
                TerminalEven.TerminalMsg("指令CMD1001" + SPname + "发给终端:" + Data[2]);
                //SockPool.Instance.Send(SPname, ToHexByteArray(Data[2]), out outData);
                SockPool.Instance.Send(SPname, ToHexByteArray(Data[2]));
            }
        }


        private void CMD1002(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;
            string[] RecData = null;
            bool isRet = false;
            if (evdata.Data.Contains("ret=0"))
            {
                RecData = evdata.Data.Remove(0, 10).Split(';');
                isRet = true;
            }
            else
            {
                RecData = evdata.Data.Remove(0, 5).Split(';');
            }

            int EpitopeNo = Convert.ToInt32(RecData[0]);
            if (isRet)
            {
                if (!string.IsNullOrEmpty(RecData[1]))
                {
                    SockPool.Instance.Send(EpitopeNo.ToString() + "-485-1", ToHexByteArray(RecData[1]));
                }

            }


        }

        private void CMD1006(object ob)
        {
            EventsSendData evdata = ob as EventsSendData;
            string[] RecData = null;
            RecData = evdata.Data.Remove(0, 10).Split(';');
            
            int EpitopeNo = Convert.ToInt32(RecData[0])+LoadIni.LoadIni.Port4852End;
            if (!string.IsNullOrEmpty(RecData[5]))
            {
                TerminalEven.TerminalMsg("指令CMD1006" + EpitopeNo + "-Can" + "发给终端:" + RecData[5]);
                SockPool.Instance.Send(EpitopeNo.ToString() + "-Can", ToHexByteArray(RecData[5]));
            }
        }

        private void SetControlConnrRelay3(int ConType, int EpitopeNo)
        {
            int Did = 0;
            if (EpitopeNo > 8)
            {
                Did = 1;
            }

            EquipmentData.DeviceManager.ControlConnrRelay3(ConType, Convert.ToByte(EpitopeNo), Did);

            if (dic485Sate.Count > 0)
            {

                foreach (var item in dic485Sate)
                {
                    if (item.Key == EpitopeNo)
                    {
                        dic485Sate[item.Key].TheRs485Sate = ConType == 0 ? true : false;
                    }
                }
            }
            else
            {
                Rs485Sate rs = new Rs485Sate();
                rs.YaoJSate = true;
                switch (ConType)
                {
                    case 0:
                        rs.TheRs485Sate = true;
                        break;
                    case 3:
                        rs.TheRs485Sate = false;
                        break;
                    default:
                        rs.TheRs485Sate = false;
                        break;
                }
                dic485Sate.Add(EpitopeNo, rs);
            }
        }



        #endregion

        private void SetControlMeterRelay(int EpitopeNo, int Onoff)
        {
            if (EpitopeNo > LoadIni.LoadIni.DeviceID)
            {
                EquipmentData.DeviceManager.ControlMeterRelay(Onoff, (byte)EpitopeNo, 1);
            }
            else
            {
                EquipmentData.DeviceManager.ControlMeterRelay(Onoff, (byte)EpitopeNo, 0);
            }
        }

        private void GetPowerAndType(string jxf, out PowerWay Pw, out string MeterPowerType)
        {
            switch (jxf)
            {
                case "data=0":
                    Pw = PowerWay.正向有功;
                    MeterPowerType = "三相四线";//PT4三相四项有功
                    break;
                case "data=1":
                    Pw = PowerWay.正向无功;
                    MeterPowerType = "三相四线";//QT4三相四相无功
                    break;
                case "data=2":
                    Pw = PowerWay.正向有功;
                    MeterPowerType = "三相三线";//P32三相三线有功
                    break;
                case "data=3":
                    Pw = PowerWay.正向无功;
                    MeterPowerType = "三相三线";//Q32三相三线无功
                    break;
                case "data=4":
                    //Q60 不知道是什么先用着PT4三相四项有功
                    Pw = PowerWay.正向有功;
                    MeterPowerType = "三相四线";//PT4三相四项有功
                    break;
                case "data=5":
                    //Q90 不知道是什么先用着PT4三相四项有功   
                    Pw = PowerWay.正向无功;
                    MeterPowerType = "三相三线";//Q32三相三线无功
                    break;
                case "data=6":
                    //Q33 不知道是什么先用着PT4三相四项有功   
                    Pw = PowerWay.正向无功;
                    MeterPowerType = "三相三线";//Q32三相三线无功
                    break;
                case "data=7":
                    //P 不知道是什么先用着PT4三相四项有功   
                    Pw = PowerWay.正向无功;
                    MeterPowerType = "三相三线";//Q32三相三线无功
                    break;
                default:
                    //先用着PT4三相四项有功   
                    Pw = PowerWay.正向有功;
                    MeterPowerType = "三相四线";//Q32三相三线无功
                    break;
            }
        }

        private void SetControlMeterRelay(bool OnOff)
        {
            if (OnOff)
            {
                for (int i = 0; i < LoadIni.LoadIni.AllMeterNumber; i++)
                {
                    if (i > LoadIni.LoadIni.DeviceID)
                    {
                        EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)i, 1);
                    }
                    else
                    {
                        EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)i, 0);
                    }
                }
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)1, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)2, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)3, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)4, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)5, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)6, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)7, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)8, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)9, 1);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)10, 1);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)11, 1);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)12, 1);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)13, 1);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)14, 1);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)15, 1);
                //EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)16, 1);

            }
            else
            {
                for (int i = 0; i < LoadIni.LoadIni.AllMeterNumber; i++)
                {
                    if (i > LoadIni.LoadIni.DeviceID)
                    {
                        EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)i, 1);
                    }
                    else
                    {
                        EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)i, 0);
                    }
                }
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)1, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)2, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)3, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)4, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)5, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)6, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)7, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)8, 0);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)9, 1);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)10, 1);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)11, 1);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)12, 1);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)13, 1);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)14, 1);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)15, 1);
                //EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)16, 1);
            }



        }


        #region //数据转换工具
        /// <summary>
        /// 十六进制转换为二进制切割为单个字符
        /// </summary>
        /// <param name="Hex"></param>
        /// <returns></returns>
        private Dictionary<int, string> HxeToInt(string Hex)
        {
            Dictionary<int, string> dicInt = new Dictionary<int, string>();
            int i = 6;
            foreach (var item in Hex)
            {
                i--;
                dicInt.Add(i, item.ToString());
            }
            return dicInt;
        }

        /// <summary>
        /// 十六进制转换为二进制切割为单个字符
        /// </summary>
        /// <param name="Hex"></param>
        /// <returns></returns>
        private Dictionary<int, string> HxeToInt(string Hex, int Number)
        {
            Dictionary<int, string> dicInt = new Dictionary<int, string>();

            for (int i = 0; i < Number; i++)
            {
                if (Hex.Length - 1 - i >= 0)
                {
                    dicInt.Add(i, Hex[Hex.Length - 1 - i].ToString());
                }
                else
                {
                    dicInt.Add(i, "0");
                }
            }
            return dicInt;
        }


        /// <summary>
        /// 十六进制转换为二进制切割为单个字符
        /// </summary>
        /// <param name="Hex"></param>
        /// <returns></returns>
        private Dictionary<int, string> HexToIntEight(string Hex)
        {
            Dictionary<int, string> dicInt = new Dictionary<int, string>();

            for (int i = 0; i < 8; i++)
            {
                if (Hex.Length - 1 >= i)
                {
                    dicInt.Add(i, Hex[i].ToString());
                }
                else
                {
                    dicInt.Add(i, "0");
                }
            }
            return dicInt;
        }


        private static string ByteToHexStr(byte[] bytes)
        {
            string retStr = "";
            retStr = retStr.Replace("-", "").Replace(" ", "");
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    retStr += bytes[i].ToString("X2");
                }
            }
            return retStr;
        }


        private static byte[] HexStringToByte(string InString)
        {
            string[] ByteString;
            ByteString = InString.Split("".ToCharArray());
            byte[] ByteOut;
            ByteOut = new byte[ByteString.Length];
            for (int i = 0; i < ByteString.Length; i++)
            {
                ByteOut[i] = byte.Parse(ByteString[i], System.Globalization.NumberStyles.HexNumber);
            }
            return ByteOut;
        }

        private static byte[] ToHexByteArray(string HexString)
        {
            byte[] retByte = new byte[HexString.Length / 2];
            int position = 0;
            for (int i = 0; i < HexString.Length / 2; i++)
            {
                retByte[i] = Convert.ToByte(HexString.Substring(position, 2), 16);
                position += 2;
            }
            return retByte;
        }

        private void SetPortName()
        {

            //string portName = GetPortNameByPortNumber(port, false, "");
            ////注册设置端口
            //SockPool.Instance.AddComSock(portName, port, strSetting, maxWaittime, waitSencondsPerByte);
            //SockPool.Instance.UpdatePortSetting(portName, strSetting);
        }


        #endregion


        #endregion


        #region 初始化设备且连接
        private void Link()
        {
            Thread.Sleep(200);
            EquipmentData.DeviceManager.InitializeDevice();
            Thread.Sleep(200);
            EquipmentData.DeviceManager.Link();

            LogtestEven.add("设备连接完成");
            LogtestEven.add("初始化表位脉冲设置...");
            InitPulsePupu();
            //SetControlMeterRelay(true);
            ////连接加密机
            //EquipmentData.DeviceManager.LinkDog();
        }

        #endregion

        #region //注册设置端口
        /// <summary>
        /// 初始化端口
        /// </summary>
        private void InitSp_Port()
        {
            InitSP();
            RegisterPort();
            LogtestEven.add("全表位串口初始化完毕");
        }
        private void RegisterPort()
        {
            foreach (var item in Sp_PortDic)
            {
                string portName = item.Key;
                SockPool.Instance.AddComSock(portName, item.Value.Com, item.Value.BoTe, 2000, 200);
                SockPool.Instance.UpdatePortSetting(portName, item.Value.BoTe);
                PortIsOpen.Add(false);
            }
        }

        /// <summary>
        /// 添加4851端口和4852端口
        /// </summary>
        private void InitSP()
        {

            for (int i = 1; i <= LoadIni.LoadIni.Port4851End; i++)
            {
                SP_PortType sP_Port = new SP_PortType();
                sP_Port.BoTe = "2400-e-8-1";
                sP_Port.Com = i;
                sP_Port.IsOpen = false;
                Sp_PortDic.Add(sP_Port.Com + "-485-1", sP_Port);
            }

            for (int i = LoadIni.LoadIni.Port4851End + 1; i <= LoadIni.LoadIni.Port4852End; i++)
            {
                SP_PortType sP_Port = new SP_PortType();
                sP_Port.BoTe = "9600-e-8-1";

                sP_Port.IsOpen = false;
                sP_Port.Com = i;
                Sp_PortDic.Add(sP_Port.Com + "-485-2", sP_Port);
            }

            for (int i = LoadIni.LoadIni.Port4852End + 1; i <= LoadIni.LoadIni.PortCanEnd; i++)
            {
                SP_PortType sP_Port = new SP_PortType();
                sP_Port.BoTe = "115200-n-8-1";

                sP_Port.IsOpen = false;
                sP_Port.Com = i;
                Sp_PortDic.Add(sP_Port.Com + "-Can", sP_Port);
            }

        }
        #endregion

        private void InitCan()
        {

        }

        #region 初始化脉冲

        /// <summary>
        /// 台体表位脉冲初始化
        /// </summary>
        private void InitPulsePupu()
        {
            for (int i = 0; i < LoadIni.LoadIni.AllMeterNumber; i++)
            {
                PulseOutputType outputType = new PulseOutputType();
                outputType.contrnlType = 0x00;
                outputType.bwNum = (byte)(i + 1);
                if (i > LoadIni.LoadIni.DeviceID)
                {
                    outputType.DevicId = 1;
                }
                else
                {
                    outputType.DevicId = 0;
                }
                outputType.GuoupOneNum = 0;
                outputType.GuoupOnePWM = 0;
                outputType.GuoupOneFreq = 0;
                outputType.GuoupTweFreq = 0;
                outputType.GuoupTwePWM = 0;
                outputType.GuoupTweFreq = 0;
                dicPulseOutput.Add((i + 1).ToString(), outputType);
            }

            if (dicPulseOutput.Count > 0)
            {
                foreach (var item in dicPulseOutput.Values)
                {
                    PulseOutputContorl.SETShhakeSate(item);
                }
            }

            LogtestEven.add("脉冲初始化完成");

        }

        #endregion
        private int LogIndex;
        private void logShow(string str)
        {
            var log = str + "\n";
            if (LogIndex == 0)
            {
                log = log + "\n";
            }

            if (Thread.CurrentThread == Dispatcher.Thread)
            {
                if (LogIndex > 1000)
                {
                    DevLogRichTxt.Document.Blocks.Clear();
                    DevLogRichTxt.AppendText("日志数量达到1000条，清空日志");
                    LogIndex = 0;

                }

                LogIndex++;
                DevLogRichTxt.AppendText(log);
                DevLogRichTxt.ScrollToEnd();

            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    if (LogIndex > 1000)
                    {
                        DevLogRichTxt.Document.Blocks.Clear();
                        DevLogRichTxt.AppendText("日志数量达到1000条，清空日志");
                        LogIndex = 0;
                    }

                    LogIndex++;
                    DevLogRichTxt.AppendText(log);
                    DevLogRichTxt.ScrollToEnd();
                });
            }

        }

        private void TerminallogShow(string str)
        {
            var log = str + "\n";
            if (LogIndex == 0)
            {
                log = log + "\n";
            }

            if (Thread.CurrentThread == TerLogRichTxt.Dispatcher.Thread)
            {
                if (LogIndex > 1000)
                {
                    LogHelpter.AddLog(GetText(TerLogRichTxt));
                    TerLogRichTxt.Document.Blocks.Clear();
                    TerLogRichTxt.AppendText("日志数量达到1000条，清空日志");
                    LogIndex = 0;
                }

                LogIndex++;
                TerLogRichTxt.AppendText(log);
                TerLogRichTxt.ScrollToEnd();
            }
            else
            {
                TerLogRichTxt.Dispatcher.Invoke(() =>
                {
                    if (LogIndex > 1000)
                    {
                        TerLogRichTxt.Document.Blocks.Clear();
                        TerLogRichTxt.AppendText("日志数量达到1000条，清空日志");
                        LogIndex = 0;
                    }

                    LogIndex++;
                    TerLogRichTxt.AppendText(log);
                    TerLogRichTxt.ScrollToEnd();
                });
            }

        }

        private void DevOrderlogShow(string str)
        {
            var log = str + "\n";
            if (LogIndex == 0)
            {
                log = log + "\n";
            }

            if (Thread.CurrentThread == OrderLogRichTxt.Dispatcher.Thread)
            {
                if (LogIndex > 1000)
                {
                    OrderLogRichTxt.Document.Blocks.Clear();
                    OrderLogRichTxt.AppendText("日志数量达到1000条，清空日志");
                    LogIndex = 0;
                }

                LogIndex++;
                OrderLogRichTxt.AppendText(log);
                OrderLogRichTxt.ScrollToEnd();
            }
            else
            {
                OrderLogRichTxt.Dispatcher.Invoke(() =>
                {
                    if (LogIndex > 1000)
                    {
                        OrderLogRichTxt.Document.Blocks.Clear();
                        OrderLogRichTxt.AppendText("日志数量达到1000条，清空日志");
                        LogIndex = 0;
                    }

                    LogIndex++;
                    OrderLogRichTxt.AppendText(log);
                    OrderLogRichTxt.ScrollToEnd();
                });
            }

        }

        public static string GetText(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            string text = textRange.Text;
            return text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int EpitopeNo = Convert.ToInt32(JDQBW.Text);
                //SockPool.Instance.OpenSpCom(EpitopeNo.ToString() + "-485-1");
                //SockPool.Instance.OpenSpCom(Convert.ToInt32(EpitopeNo + 16).ToString() + "-485-2");
                EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)EpitopeNo, EpitopeNo > LoadIni.LoadIni.DeviceID ? 1 : 0);

                LogtestEven.add("打开继电器" + JDQBW.Text + "成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                int EpitopeNo = Convert.ToInt32(JDQBW.Text);
                //SockPool.Instance.CloseSpCom(EpitopeNo.ToString() + "-485-1");
                //SockPool.Instance.CloseSpCom(Convert.ToInt32(EpitopeNo + 16).ToString() + "-485-2");
                EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)EpitopeNo, EpitopeNo > LoadIni.LoadIni.DeviceID ? 1 : 0);

                LogtestEven.add("关闭继电器" + JDQBW.Text + "成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void CloseMeterRelay()
        {
            for (int i = 1; i <= LoadIni.LoadIni.AllMeterNumber; i++)
            {
                EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)i, i > LoadIni.LoadIni.DeviceID ? 1 : 0);
                Thread.Sleep(300);
            }
            LogtestEven.add("全部继电器隔离完成");
        }

        private void OpenMeterRelay()
        {
            for (int i = 1; i <= LoadIni.LoadIni.AllMeterNumber; i++)
            {
                EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)i, i > LoadIni.LoadIni.DeviceID ? 1 : 0);
                Thread.Sleep(300);
            }
            LogtestEven.add("全部继电器打开完成");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OpenMeterRelay();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            CloseMeterRelay();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            foreach (var item in Sp_PortDic)
            {
                string portName = item.Key;
                SockPool.Instance.OpenSpCom(portName);
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            foreach (var item in Sp_PortDic)
            {
                string portName = item.Key;
                SockPool.Instance.CloseSpCom(portName);
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            try
            {
                int EpitopeNo = Convert.ToInt32(JDQBW.Text);
                SockPool.Instance.OpenSpCom(EpitopeNo.ToString() + "-485-1");
                SockPool.Instance.OpenSpCom(Convert.ToInt32(EpitopeNo + +LoadIni.LoadIni.Port4851End).ToString() + "-485-2");
                SockPool.Instance.OpenSpCom(Convert.ToInt32(EpitopeNo + LoadIni.LoadIni.Port4852End).ToString() + "-Can");
                LogtestEven.add("打开端口" + JDQBW.Text + "成功");
            }
            catch (Exception ex)
            {

                MessageBox.Show("打开端口" + JDQBW.Text + "失败" + "\r\n" + ex.Message);
            }

        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            try
            {
                int EpitopeNo = Convert.ToInt32(JDQBW.Text);

                SockPool.Instance.CloseSpCom(EpitopeNo.ToString() + "-485-1");
                SockPool.Instance.CloseSpCom(Convert.ToInt32(EpitopeNo + 16).ToString() + "-485-2");
            }
            catch (Exception ex)
            {

                MessageBox.Show("关闭" + JDQBW.Text + "失败" + "\r\n" + ex.Message);
            }
        }

        /// <summary>
        /// 设置终端类型
        /// </summary>
        public void Set_TerminalType(string name)
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            //num = meterStartS.Keys.Count / num;//每一路线上的数量
            byte type = 0;
            switch (name)
            {
                case "集中器I型13版":
                    type = 1;
                    break;
                case "集中器I型22版":
                    type = 2;
                    break;
                case "专变III型13版":
                    type = 3;
                    break;
                case "专变III型22版":
                    type = 4;
                    break;
                case "融合终端":
                    type = 5;
                    break;
                case "能源控制器":
                    type = 6;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < num; i++)
            {
                EquipmentData.DeviceManager.SetZDType(type, (byte)i);
            }
        }

        private void ChangeInCan(int InCanOn, int EpitopeNo)
        {
            EquipmentData.DeviceManager.ControlConnrRelayCan(0, (byte)EpitopeNo, InCanOn, EpitopeNo > LoadIni.LoadIni.DeviceID ? 1 : 0);
        }

        private void SetCanBote(int Enpo, string msg)
        {
            Enpo = Enpo + LoadIni.LoadIni.Port4852End;
            string NewBote = "";
            switch (msg)
            {
                case "50kbps":
                    NewBote = CanOrder.CanBote50k;
                    break;
                case "125kbps":
                    NewBote = CanOrder.CanBote125k;
                    break;
                case "250kbps":
                    NewBote = CanOrder.CanBote250k;
                    break;
                case "500kbps":
                    NewBote = CanOrder.CanBote500k;
                    break;
                case "1Mbps":
                    NewBote = CanOrder.CanBote1000k;
                    break;
            }
            SockPool.Instance.Send(Enpo + "-Can", ToHexByteArray(NewBote.Replace(" ", "")));

        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            Set_TerminalType("专变III型22版");

            LogtestEven.add("专变III型22版");
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            Set_TerminalType("集中器I型22版"); LogtestEven.add("集中器I型22版");
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            Set_TerminalType("专变III型13版"); LogtestEven.add("专变III型13版");
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            Set_TerminalType("集中器I型13版"); LogtestEven.add("集中器I型13版");
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            Set_TerminalType("融合终端"); LogtestEven.add("融合终端");
        }

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            Set_TerminalType("能源控制器"); LogtestEven.add("能源控制器");
        }

        private void Button_Click_14(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] RecData = JDQBW.Text.Split(';');

                int EpitopeNo = Convert.ToInt32(RecData[0]);

                DevOrderEven.DevOrderMsg("设置遥信状态:表位：" + EpitopeNo.ToString() + "指令：" + RecData[0]);
                ShakeStatusInfo.ShakeStatus[EpitopeNo - 1] = Convert.ToString(Convert.ToInt32(RecData[0], 16), 2).PadLeft(8, '0');
                //DevOrderEven.DevOrderMsg("设置遥信状态:表位：" + EpitopeNo.ToString() + "指令：" + RecData[1]);
                //ShakeStatusInfo.ShakeStatus[EpitopeNo - 1] = Convert.ToString(Convert.ToInt32(RecData[1], 16), 2).PadLeft(8, '0');

                ShakeMessStatus.SETShhakeSate(EpitopeNo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("设置遥信状态失败" + ex.Message);
            }
        }

        private void Button_Click_15(object sender, RoutedEventArgs e)
        {
            LogHelpter.AddLog(GetText(TerLogRichTxt));
            MessageBox.Show("保存成功");
        }

        private void Button_Click_16(object sender, RoutedEventArgs e)
        {
            try
            {
                int EpitopeNo = Convert.ToInt32(JDQBW.Text);
                //EventsSendData evdata = new EventsSendData();
                //evdata.CMD = "CMD0104";
                //evdata.Data = "DATA=0;2;6;115200-E-8-1";
                //CMD0104(evdata);

                //EventsSendData evdata = new EventsSendData();
                //evdata.CMD = "CMD1001";
                //evdata.Data = "data=1;0;683800C3000000000001030201F23B000100003C01000F029001F00100000000003408000001012001012047573235010120030280251916";
                //CMD1001(evdata);
                //string test = "68 3F 00 43 04 00 00 00 00 4D 01 40 02 02 00 00 01 00 00 00 00 10 F1 01 00 02 00 20 00 68 01 00 00 00 00 10 68 11 04 34 34 33 37 C8 16 68 01 00 00 00 00 10 68 11 04 35 34 33 37 C9 16 4C 16 68 3F 00 43 04 00 00 00 00 4E 01 40 02 02 00 00 02 00 00 00 00 10 F1 01 00 02 00 20 00 68 02 00 00 00 00 10 68 11 04 34 34 33 37 C9 16 68 02 00 00 00 00 10 68 11 04 35 34 33 37 CA 16 52 16 68 3F 00 43 04 00 00 00 00 4F 01 40 02 02 00 00 03 00 00 00 00 10 F1 01 00 02 00 20 00 68 03 00 00 00 00 10 68 11 04 34 34 33 37 CA 16 68 03 00 00 00 00 10 68 11 04 35 34 33 37 CB 16 58 16 68 3F 00 43 04 00 00 00 00 50 01 40 02 02 00 00 04 00 00 00 00 10 F1 01 00 02 00 20 00 68 04 00 00 00 00 10 68 11 04 34 34 33 37 CB 16 68 04 00 00 00 00 10 68 11 04 35 34 33 37 CC 16 5E 16 68 3F 00 43 04 00 00 00 00 51 01 40 02 02 00 00 05 00 00 00 00 10 F1 01 00 02 00 20 00 68 05 00 00 00 00 10 68 11 04 34 34 33 37 CC 16 68 05 00 00 00 00 10 68 11 04 35 34 33 37 CD 16 64 16 68 3F 00 43 04 00 00 00 00 52 01 40 02 02 00 00 06 00 00 00 00 10 F1 01 00 02 00 20 00 68 06 00 00 00 00 10 68 11 04 34 34 33 37 CD 16 68 06 00 00 00 00 10 68 11 04 35 34 33 37 CE 16 6A 16 68 3F 00 43 04 00 00 00 00 53 01 40 02 02 00 00 07 00 00 00 00 10 F1 01 00 02 00 20 00 68 07 00 00 00 00 10 68 11 04 34 34 33 37 CE 16 68 07 00 00 00 00 10 68 11 04 35 34 33 37 CF 16 70 16 68 3F 00 43 04 00 00 00 00 54 01 40 02 02 00 00 08 00 00 00 00 10 F1 01 00 02 00 20 00 68 08 00 00 00 00 10 68 11 04 34 34 33 37 CF 16 68 08 00 00 00 00 10 68 11 04 35 34 33 37 D0 16 76 16 68 3F 00 43 04 00 00 00 00 55 01 40 02 02 00 09 00 10 F1 00 00 00 00 10 11 34 33 D0 68 00 68 04 34 37 16 68 00 04 00 00 01 02 00 00 00 00 10 01 02 20 68 10 00 00 68 34 37 D7 10 00 00 10 11 35 33 D8 A0 16";
                //List<string> testmsg = BackSplitMsg(test);

                EquipmentData.DeviceManager.ControlConnrRelay3(3, (byte)(EpitopeNo), (EpitopeNo) > LoadIni.LoadIni.DeviceID ? 1 : 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(JDQBW.Text + "表位4852切换到232失败" + ex.Message);
            }
        }

        private void Button_Click_17(object sender, RoutedEventArgs e)
        {
            try
            {
                int EpitopeNo = Convert.ToInt32(JDQBW.Text);

                EquipmentData.DeviceManager.ControlConnrRelay3(0, (byte)(EpitopeNo), (EpitopeNo) > LoadIni.LoadIni.DeviceID ? 1 : 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(JDQBW.Text + "表位232切换到4852失败" + ex.Message);
            }
        }

        private void Button_Click_18(object sender, RoutedEventArgs e)
        {

            try
            {
                //CMD=0105,DATA=1;2;1;50KBPS
                EventsSendData evdata = new EventsSendData();
                evdata.CMD = "cmd=0104";
                evdata.Data = "DATA=1;2;1;115200-E-8-1";
                CMD0104(evdata);
                //string data = "cmd=1006,ret=0,data=1;1;0;0;00000000;681234567816";
                //if (data == "") return;

                //string[] arr = data.Split(',');
                //string cmd = arr[0];
                //string Edata = null;
                //for (int i = 1; i < arr.Length; i++)
                //{
                //    Edata += arr[i];
                //}
                //if (DelayFlag == false)
                //{
                //    lock (obj)
                //    {
                //        queueSendData.Enqueue(new EventsSendData() { CMD = cmd, Data = Edata });
                //    }
                //    string msg = string.Format("From PC:{0}", data);
                //    LogtestEven.add(msg);
                //}
                int EpitopeNo = Convert.ToInt32(JDQBW.Text);
                SetCanBote(EpitopeNo, "50kbps");
            }
            catch (Exception ex)
            {
                MessageBox.Show(JDQBW.Text + "表位，Can口切换波特率失败" + ex.Message);
            }
        }

        private void Button_Click_19(object sender, RoutedEventArgs e)
        {
            try
            {
                int EpitopeNo = Convert.ToInt32(JDQBW.Text);
                SetCanBote(EpitopeNo, "125kbps");
            }
            catch (Exception ex)
            {
                MessageBox.Show(JDQBW.Text + "表位，Can口切换波特率失败" + ex.Message);
            }
        }

        private void Button_Click_20(object sender, RoutedEventArgs e)
        {
            try
            {
                int EpitopeNo = Convert.ToInt32(JDQBW.Text);
                SetCanBote(EpitopeNo, "250kbps");
            }
            catch (Exception ex)
            {
                MessageBox.Show(JDQBW.Text + "表位，Can口切换波特率失败" + ex.Message);
            }
        }

        private void Button_Click_21(object sender, RoutedEventArgs e)
        {
            try
            {
                int EpitopeNo = Convert.ToInt32(JDQBW.Text);
                SetCanBote(EpitopeNo, "500kbps");
            }
            catch (Exception ex)
            {
                MessageBox.Show(JDQBW.Text + "表位，Can口切换波特率失败" + ex.Message);
            }
        }

        private void Button_Click_22(object sender, RoutedEventArgs e)
        {
            try
            {
                int EpitopeNo = Convert.ToInt32(JDQBW.Text);
                SetCanBote(EpitopeNo, "1Mbps");
            }
            catch (Exception ex)
            {
                MessageBox.Show(JDQBW.Text + "表位，Can口切换波特率失败" + ex.Message);
            }
        }

        private void Button_Click_23(object sender, RoutedEventArgs e)
        {
            try
            {
                int EpitopeNo = Convert.ToInt32(JDQBW.Text);
                ChangeInCan(1, EpitopeNo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(JDQBW.Text + "表位，4852切换到Can" + ex.Message);
            }
        }

        private void Button_Click_24(object sender, RoutedEventArgs e)
        {
            try
            {
                int EpitopeNo = Convert.ToInt32(JDQBW.Text);
                ChangeInCan(0, EpitopeNo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(JDQBW.Text + "表位，Can切换到4852" + ex.Message);
            }
        }
    }
}
