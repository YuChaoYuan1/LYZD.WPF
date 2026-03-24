using LYZD.TransferToPlatform.DevicesFunc;
using LYZD.TransferToPlatform.Test;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.SocketEven
{
    internal class COM32 : IConnection
    {
        /// <summary>
        /// 波特率
        /// </summary>
        private string BaudRate;
        /// <summary>
        /// 数据位
        /// </summary>
        private string DataBits;
        /// <summary>
        /// 停止位
        /// </summary>
        private string StopBits;
        /// <summary>
        /// 校验位
        /// </summary>
        private string CheckBits;
        /// <summary>
        /// 端口号
        /// </summary>
        private string PortNum;

        private Object ThreadObject = new object();


        private SerialPort spCom;

        private byte[] buff;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Settings">通信参数1200,e,8,1</param>
        /// <param name="ComNum">端口号(1,2,3,4)</param>
        public COM32(string Settings, int ComNum)
        {
            UpdatePortInfo(Settings);
            PortNum = "COM" + ComNum;
            spCom = new SerialPort();
            MaxWaitSeconds = 1000;
            WaitSecondsPerByte = 1000;
            spCom.DataReceived += SpCom_DataReceived;
        }
        #region IConnection 成员

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            lock (ThreadObject)
            {
                try
                {
                    if (spCom.IsOpen)
                    {
                        //if (spCom.BaudRate != int.Parse(BaudRate)) {
                        //    spCom.BaudRate = int.Parse(BaudRate);
                        //}
                        return true;
                        //spCom.Close();
                    }

                    spCom.BaudRate = int.Parse(BaudRate);
                    spCom.StopBits = (StopBits)int.Parse(StopBits);
                    spCom.DataBits = int.Parse(DataBits);
                    spCom.Parity = CheckBits.ToLower() == "n" ? Parity.None : CheckBits.ToLower() == "e" ? Parity.Even : Parity.Mark;
                    spCom.PortName = PortNum;
                    spCom.DtrEnable = true;
                    spCom.ReceivedBytesThreshold = 1;

                    spCom.Open();

                    return true;
                }
                catch
                {
                    spCom.Close();
                    return false;
                }
            }
        }

        /// <summary>
        /// 关闭端口
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                if (spCom.IsOpen) spCom.Close();
            }
            catch { }
            return spCom.IsOpen == false;
        }
        /// <summary>
        /// 更新串口波特率
        /// </summary>
        /// <param name="szSetting"></param>
        /// <returns></returns>
        public bool UpdateBltSetting(string szSetting)
        {
            UpdatePortInfo(szSetting);
            return true;
        }

        /// <summary>
        /// 连接名称
        /// </summary>
        public string ConnectName
        {
            get
            {
                return PortNum;
            }
            set
            {
                PortNum = value;
            }
        }

        public int MaxWaitSeconds
        {
            get;
            set;
        }

        public int WaitSecondsPerByte
        {
            get;
            set;
        }

        public byte[] RetBuff
        {
            get { return buff; }
            set { SerPortEven.GetSeriPostMsg(RetBuff, PortNum); }
        }

        //private static object SetSendLock = new object();

        /// <summary>
        /// 发送数据\接收数据
        /// </summary>
        /// <param name="vData">发送数据</param>
        /// <param name="IsReturn">是否需要等待返回</param>
        /// <param name="WaiteTime"></param>
        /// <returns></returns>
        public bool SendData(byte[] vData, bool IsReturn, int WaiteTime)
        {
            lock (ThreadObject)
            {
                if (!this.Open())
                {
                    this.Close();
                    return false;
                }

                spCom.DiscardOutBuffer();
                spCom.DiscardInBuffer();
                Console.WriteLine("┏SendData:{0}", BitConverter.ToString(vData));
                spCom.WriteTimeout = MaxWaitSeconds;
                try
                {
                    spCom.Write(vData, 0, vData.Length);
                }
                catch (TimeoutException ex)
                {
                    ex.ToString();
                    Console.WriteLine("┗发送数据超时");
                    return false;
                }


                return true;
            }
        }


        /// <summary>
        /// 发送数据\接收数据
        /// </summary>
        /// <param name="vData">发送数据</param>
        /// <param name="IsReturn">是否需要等待返回</param>
        /// <param name="WaiteTime"></param>
        /// <returns></returns>
        public bool SendData(ref byte[] vData, bool IsReturn, int WaiteTime, out byte[] Rdata)
        {
            lock (ThreadObject)
            {
                Rdata = null;
                if (!this.Open())
                {
                    this.Close();
                    return false;
                }

                spCom.DiscardOutBuffer();
                spCom.DiscardInBuffer();
                Console.WriteLine("┏SendData:{0}", BitConverter.ToString(vData));
                spCom.WriteTimeout = MaxWaitSeconds;
                try
                {
                    spCom.Write(vData, 0, vData.Length);
                }
                catch (TimeoutException ex)
                {
                    ex.ToString();
                    Console.WriteLine("┗发送数据超时");
                    return false;
                }
                if (!IsReturn)
                {
                    //spCom.Close();
                    Console.WriteLine("┗本包不需要回复");
                    return true;     //如果不需要返回
                }
                bool IsOut = false;

                DateTime TmpTime1 = DateTime.Now;

                while (TimeSub(DateTime.Now, TmpTime1) < MaxWaitSeconds)          //1秒超时器，如果超过表示收不到任何数据，直接退出
                {
                    System.Threading.Thread.Sleep(1);
                    IsOut = true;
                    if (spCom.BytesToRead > 0)      //如果缓冲区待接收数据量大于0
                    {
                        IsOut = false;
                        break;
                    }
                }
                if (IsOut)      //如果超时就将需要返回的数组数量设置为0
                {
                    vData = new byte[0];
                    //spCom.Close();
                    Console.WriteLine("┗RecvData:接收超时");
                    return true;
                }

                List<byte> TmpBytes = new List<byte>();

                TmpTime1 = DateTime.Now;
                byte[] buf = new byte[0];
                while (TimeSub(DateTime.Now, TmpTime1) < WaitSecondsPerByte)       //100毫秒超时器，目的是检查最后一个字符后面是否还存在待接受的数据
                {
                    System.Threading.Thread.Sleep(1);
                    if (spCom.BytesToRead != 0)
                    {
                        buf = new byte[spCom.BytesToRead];
                        spCom.Read(buf, 0, buf.Length);
                        TmpBytes.AddRange(buf);
                        TmpTime1 = DateTime.Now;
                    }
                }

                //验证数据完整性
                //if (!DataValidation698(BitConverter.ToString(TmpBytes.ToArray())))//报文不完整-开始延时等待
                //{
                //    TmpTime1 = DateTime.Now;
                //    while (TimeSub(DateTime.Now, TmpTime1) < 200)   //5秒超时
                //    {
                //        System.Threading.Thread.Sleep(50);//50毫秒读取一次
                //        if (spCom.BytesToRead != 0)
                //        {
                //            buf = new byte[spCom.BytesToRead];
                //            spCom.Read(buf, 0, buf.Length);
                //            TmpBytes.AddRange(buf);
                //            if (DataValidation698(BitConverter.ToString(TmpBytes.ToArray()))) //数据完整了，退出
                //            {
                //                break;
                //            }
                //        }
                //    }
                //}

                Rdata = TmpBytes.ToArray();



                Console.WriteLine("┗RecvData:{0}", BitConverter.ToString(vData));
                //this.Close();
                return true;
            }
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

        /// <summary>
        /// 数据验证698
        /// </summary>
        /// <returns></returns>
        private bool DataValidation698(string data)
        {
            data = data.Replace("-", "").Replace(" ", "");//清除所有空格
            if (data.IndexOf("68") == -1) return false;//没有帧头的
            data = data.Substring(data.IndexOf("68"));
            if (data.Length < 10) return false;//没有帧头的
            //获取他的长度
            int ilen = Convert.ToInt32(data.Substring(4, 2) + data.Substring(2, 2), 16) + 2;
            if (data.Length < ilen * 2) return false;
            if (data.Substring(ilen * 2 - 2, 2) != "16") return false;//没有帧尾
            return true;
        }

        private void SpCom_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            bool IsOut = false;
            DateTime TmpTime1 = DateTime.Now;
            while (TimeSub(DateTime.Now, TmpTime1) < MaxWaitSeconds)          //1秒超时器，如果超过表示收不到任何数据，直接退出
            {
                System.Threading.Thread.Sleep(1);
                IsOut = true;
                if (spCom.BytesToRead > 0)      //如果缓冲区待接收数据量大于0
                {
                    IsOut = false;
                    break;
                }
            }
            if (IsOut)      //如果超时就将需要返回的数组数量设置为0
            {
                spCom.DiscardOutBuffer();
                spCom.DiscardInBuffer();
                //spCom.Close();
                Console.WriteLine("┗RecvData:接收超时");

            }
            List<byte> TmpBytes = new List<byte>();

            TmpTime1 = DateTime.Now;
            byte[] buf = new byte[0];
            while (TimeSub(DateTime.Now, TmpTime1) < WaitSecondsPerByte)       //100毫秒超时器，目的是检查最后一个字符后面是否还存在待接受的数据
            {
                System.Threading.Thread.Sleep(1);
                if (spCom.BytesToRead != 0)
                {
                    buf = new byte[spCom.BytesToRead];
                    spCom.Read(buf, 0, buf.Length);
                    TmpBytes.AddRange(buf);
                    TmpTime1 = DateTime.Now;
                }
            }

            buff = TmpBytes.ToArray();
            RetBuff = buff;
            //InToTheQueue(spCom.PortName, buff);


        }

        private void InToTheQueue(string PortName, byte[] NewBuff)
        {
            int portName = Convert.ToInt32(PortName.Remove(0, 3));
            if (portName > LoadIni.LoadIni.Port4851End)
            {
                lock (MainWindow.SetSendLock)
                {
                    string PortMsg = ByteToHexStr(NewBuff);
                    TerminalEven.TerminalMsg("4852来自终端：" + portName.ToString() + "Com:" + PortMsg);
                    List<string> SplitSmg = BackSplitMsg(PortMsg);
                    foreach (string item in SplitSmg)
                    {
                        TransferToPlatform.MainWindow.queueReceiveData.Enqueue(new EventsReceiveData() { CMD = "cmd=1001", Ret = "ret=0,", Data = (portName - LoadIni.LoadIni.Port4851End).ToString() + ";" + item });
                    }
                }
            }
            else
            {

                lock (MainWindow.SetSendLock)
                {
                    TerminalEven.TerminalMsg("4851来自终端：" + portName.ToString() + "Com:" + ByteToHexStr(NewBuff));
                    TransferToPlatform.MainWindow.queueReceiveData.Enqueue(new EventsReceiveData() { CMD = "cmd=1002", Ret = null, Data = portName.ToString() + ";" + ByteToHexStr(NewBuff) }); ;
                }
            }
        }
        #endregion

        private long TimeSub(DateTime Time1, DateTime Time2)
        {
            TimeSpan tsSub = Time1.Subtract(Time2);
            return tsSub.Hours * 60 * 60 * 1000 + tsSub.Minutes * 60 * 1000 + tsSub.Seconds * 1000 + tsSub.Milliseconds;
        }

        /// <summary>
        /// 更新端口信息
        /// </summary>
        /// <param name="Settings"></param>
        private void UpdatePortInfo(string Settings)
        {
            string[] Tmp = Settings.Split('-');

            if (Tmp.Length != 4)
            {

                BaudRate = "1200";
                CheckBits = "n";
                DataBits = "8";
                StopBits = "1";
            }
            else
            {
                BaudRate = Tmp[0];
                CheckBits = Tmp[1];
                DataBits = Tmp[2];
                StopBits = Tmp[3];
                if (spCom != null && spCom.BaudRate != int.Parse(BaudRate))
                {
                    spCom.BaudRate = int.Parse(BaudRate);
                    spCom.StopBits = (StopBits)int.Parse(StopBits);
                    spCom.DataBits = int.Parse(DataBits);
                    spCom.Parity = CheckBits.ToLower() == "n" ? Parity.None : CheckBits.ToLower() == "e" ? Parity.Even : Parity.Mark;
                    //spCom.PortName = PortNum;
                    //spCom.DtrEnable = true;
                    //spCom.ReceivedBytesThreshold = 1;
                }
            }
        }


    }
}
