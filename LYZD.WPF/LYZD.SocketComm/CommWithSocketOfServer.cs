using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace LYZD.SocketComm
{
    /// 笔记：：：：只有当Socket是连接的时候才能发送，MSDN例子为，先发送一个为空的byte进行判断
    /// MSDN::Connected 属性的值反映最近操作时的连接状态。如果您需要确定连接的当前状态，请进行非阻止、零字节的 Send 调用。
    /// 当服务器端连接较少的时候也不建议使用异步的
    /// 因为Socket的TCP通讯中有一个“粘包”的现象，既：
    /// 大多数时候发送端多次发送的小数据包会被连在一起被接收端同时接收到，多个小包被组成一个大包被接收。
    /// 有时候一个大数据包又会被拆成多个小数据包发送。这样就存在一个将数据包拆分和重新组合的问题。
    public class CommWithSocketOfServer
    {
        /// <summary>
        /// 在线状态
        /// </summary>
        public bool m_bolOnLineStatus = false;
        public Socket _serverSocket = null;
        private List<Socket> _clientSocket = null;
        Socket toSocket = null;
        private const int BufferSize = 5024;
        public byte[] buffer = new byte[BufferSize];
        private int _port;                                   //接收端端口
        private string _strIP;                               //接收端IP

        private string _sendFrame = "";
        public int Index = 0;
        /// <summary>
        /// 发送帧
        /// </summary>
        public string SendFrame
        {
            get { return _sendFrame; }
            set { _sendFrame = value; }
        }
        private string _receiveFrame = "";
        /// <summary>
        /// 接收帧
        /// </summary>
        public string ReceiveFrame
        {
            get
            {
                return _receiveFrame;
            }
            set { _receiveFrame = value; }
        }

        #region Event
        public event EventHandler IniFinshedEvent;          //初始化端口成功
        public event EventHandler AcceptEvent;              //客户端连接到服务器　上线通知
        public event EventHandler SendFinishedEvent;        //发送完成
        public event EventHandler ReceivedFinshedEvent;     //接收到数据完成
        public event EventHandler ExceptionEvent;           //错误事件,掉线
        #endregion

        /// <summary>
        /// Socket通信服务器端
        /// </summary>
        /// <param name="strIP">要监听的IP</param>
        /// <param name="port">要监听的端口</param>
        public CommWithSocketOfServer()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientSocket = new List<Socket>();
        }


        /// <summary>
        /// 开始监听，监听端口
        /// </summary>
        /// <param name="strIP"></param>
        /// <param name="port"></param>
        public void StartListening(int p_index, string strIP, int port)
        {
            _port = port;
            _strIP = strIP;
            Index = p_index;
            CommEventArgs _commEventMsg = new CommEventArgs();
            try
            {
                if (_serverSocket.IsBound)
                {
                    Close("");
                }
                //IPHostEntry ieh = Dns.GetHostByName(Dns.GetHostName());
                //IPAddress ipa = ieh.AddressList[1];
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(strIP), _port);
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //设置SOCKET允许多个SOCKET 公用一个本地端口同一个本地IP地址和端口号 
                _serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _serverSocket.Bind(iep);
                _serverSocket.Listen(100);                    //每个端口只和2个进行连接
                _serverSocket.BeginAccept(new AsyncCallback(AcceptConnectCallBack), _serverSocket);

                _commEventMsg.State = "OK";
                _commEventMsg.StrArgs = "COM Open";
                if (IniFinshedEvent != null)
                {
                    IniFinshedEvent(this, _commEventMsg);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX错误：：StartListening：：" + e.Message);
                Close(e.Message);
            }
        }

        /// <summary>
        /// 当接收到连接以后回调函数 ----- 关闭端口会引发异常，所以要进行异常捕获
        /// </summary>
        /// <param name="a"></param>
        private void AcceptConnectCallBack(IAsyncResult a)
        {
            try
            {
                //litener 是服务器端socket 获取接收到的socket
                Socket litener = null;
                Socket oneClientSocket = null;
                try
                {
                    litener = a.AsyncState as Socket;
                    ///把连接过来的客户端存储起来
                    oneClientSocket = litener.EndAccept(a);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX出错：AcceptConnectCallBack2222:::" + e.Message);
                    //StartListening(_strIP, _port);
                    return;
                }

                Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>新连接过来的socket 端口：" + oneClientSocket.RemoteEndPoint.ToString());

                ///要不要把上一个端口监听的清理掉呢  
                ClearTimeOutSocket();

                ///每次接收到连接添加入客户端套接字队列
                _clientSocket.Add(oneClientSocket);

                if (AcceptEvent != null)
                {
                    CommEventArgs _commEventMsg = new CommEventArgs();
                    _commEventMsg.State = "OK";
                    //_commEventMsg.StrArgs = ((IPEndPoint)_clientSocket.RemoteEndPoint).Port.ToString();
                    _commEventMsg.StrArgs = ((IPEndPoint)oneClientSocket.RemoteEndPoint).Address.ToString();
                    _commEventMsg.port = ((IPEndPoint)oneClientSocket.RemoteEndPoint).Port;
                    AcceptEvent(this, _commEventMsg);
                }

                ///开始监听接收数据

                /////开始监听接收数据
                oneClientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveFrameCallBack, oneClientSocket);

                ///本店socket其他连接继续监听
                litener.BeginAccept(new AsyncCallback(AcceptConnectCallBack), litener);
            }
            catch (System.Exception e)
            {
                Debug.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX出错：AcceptConnectCallBack:::" + e.Message);
                _serverSocket.BeginAccept(new AsyncCallback(AcceptConnectCallBack), _serverSocket);
            }
        }

        /// <summary>
        /// 手动打开所有连接开始接收接受到的链接发送的数据
        /// </summary>
        public void Receive()
        {
            try
            {
                /// 然后我们的程序继续执行下去，当有数据到达的时候，系统将数据读入缓冲区，并执行回调函数，
                /// 
                /// 这里控制接收是把所连接上到同一服务器的端口都打开，，，，应该有问题
                foreach (var oneS in _clientSocket)
                {
                    ///这里向系统投递一个接收信息的请求，并为其指定ReceiveCallBack做为回调函数
                    oneS.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveFrameCallBack, oneS);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX出错：Receive：：：：" + e.Message);
                ///客户端关闭连接以后，服务器端进行重新监听
                //StartListening(_strIP, _port);
                OnException(e.Message.ToString());
            }
        }



        /// <summary>
        /// 收到数据回调函数,,,终端断开连接线下或 则网线掉了调用此函数
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveFrameCallBack(IAsyncResult ar)
        {
            ///锁住自动恢复的socket自动回复帧
            lock (this)
            {
                ///clientSocket
                Socket scoketStemp = ar.AsyncState as Socket;
                int lenTtemp = 0;
                try
                {
                    ///接收异步读取，并返回接收自己长度，结束本次读取
                    try
                    {
                        ///来获取错误号也许性能会好一些
                        lenTtemp = scoketStemp.EndReceive(ar);
                    }
                    ///捕捉到了终端掉线抛出异常---- 则网线掉了调用此函数，，网线拔掉调用的函数，“远程主机强迫关闭了一个现有链接”
                    ///重新连接
                    ///scoketStemp 在这里已经释放掉了，，这里要改
                    catch (Exception e)
                    {
                        Debug.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX出错：ReceiveFrameCallBack1：：：：" + scoketStemp.RemoteEndPoint.ToString() + e.Message);
                        //Debug.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"+e.Message+DateTime.Now.ToLongTimeString());
                        if (this.ExceptionEvent != null)
                        {
                            ExceptionEvent(this, new EventArgs());
                        }
                        RemoveLogoutSocket(scoketStemp);

                        ///继续监听
                        _serverSocket.BeginAccept(new AsyncCallback(AcceptConnectCallBack), _serverSocket);
                        //StartListening(_strIP, _port);               
                        return;
                    }

                    ///表示接收到了最后了，所以接收完成，接收到
                    if (lenTtemp > 0)
                    {
                        ///从缓冲区把字符串读取出来
                        string strTempReD = Encoding.Default.GetString(buffer, 0, lenTtemp).Replace(" ", "");

                        byte[] tempResult = new byte[lenTtemp];
                        Array.Copy(buffer, 0, tempResult, 0, lenTtemp);
                        string sRecieved = byteToHexStr(tempResult);
                        //Debug.WriteLine("收:"+scoketStemp.RemoteEndPoint.ToString() + "::::--------------" + strTempReD);

                        Debug.WriteLine("已经接收的:" + scoketStemp.RemoteEndPoint.ToString() + "::::--------------" + strTempReD);
                        ///说明已经接收完成一个完整帧了
                        if (ReceivedFinshedEvent != null)
                        {
                            CommEventArgs _commEventMsg = new CommEventArgs();
                            _commEventMsg.State = "Conneted";
                            /// 正确格式： _commEventMsg.StrArgs = sRecieved;
                            _commEventMsg.StrArgs = sRecieved;
                            //_commEventMsg.StrArgs = sRecieved;
                            //_commEventMsg.ip = ((IPEndPoint)scoketStemp.RemoteEndPoint).Address.ToString();
                            _commEventMsg.port = ((IPEndPoint)scoketStemp.RemoteEndPoint).Port;
                            ///自动回复时，发送给接收到的帧，这里要枷锁
                            toSocket = scoketStemp;
                            ReceivedFinshedEvent(this, _commEventMsg);
                            toSocket = null;
                            //ReceivedFinshedEvent.BeginInvoke(this, _commEventMsg, (x) => { toSocket = null; }, null);                           
                        }
                        ///继续监听接受文件
                        scoketStemp.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveFrameCallBack, scoketStemp);

                    }
                    /// lenTemp==0 是在终端断开链接以后
                    else
                    {
                        ///禁用接收和发送
                        Debug.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX出错：ReceiveFrameCallBack3：：：：");
                        if (this.ExceptionEvent != null)
                        {
                            ExceptionEvent(this, new EventArgs());
                        }
                        RemoveLogoutSocket(scoketStemp);

                        ///重新开始监听 
                        //StartListening(_strIP, _port);
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX出错：ReceiveFrameCallBack2：：：：" + e.Message);
                    if (this.ExceptionEvent != null)
                    {
                        ExceptionEvent(this, new EventArgs());
                    }
                    RemoveLogoutSocket(scoketStemp);
                }
            }
        }



        /// <summary>
        /// 发送字符串
        /// </summary>
        /// <param name="msg"></param>
        public void SendData(string msg)
        {
            byte[] mssBytes = Encoding.ASCII.GetBytes(msg);
            SendData(mssBytes);
        }


        /// <summary>
        /// 异步发送帧，等待等待回调函数
        /// </summary>
        /// <param name="frameBytes"></param>
        public void SendData(byte[] frameBytes)
        {
            try
            {
                string tempData = byteToHexStr(frameBytes);

                CommEventArgs _commEventMsg = new CommEventArgs();
                _commEventMsg.State = "SENDED";
                _commEventMsg.StrArgs = tempData;

                #region
                //_commEventMsg = new CommEventArgs();
                //_commEventMsg.State = "SENDED";
                //_commEventMsg.StrArgs = tempStr;
                //if (SendedEvent != null)
                //{
                //    SendedEvent(this, _commEventMsg);
                //}
                ///传递 byte b = 0;表示确认是否连接
                ///_serverSocket.Send
                ///BeginSned和BeginReceive可不是简单的丢在线程池里，
                ///他是投递到系统内核里去了。由系统内核来调度发送和接收的操作。 
                #endregion
                ///自动发送给目的
                if (toSocket != null)
                {
                    //toSocket.BeginSend(frameBytes, 0, frameBytes.Length, SocketFlags.None, SendFrameCallBack, tempData);

                    // Debug.WriteLine("1准备发送发:" + toSocket.RemoteEndPoint.ToString() + "::::--------------" + byteToHexStr(frameBytes));
                    toSocket.Send(frameBytes);
                    /// Debug.WriteLine("1已经发送发:" + toSocket.RemoteEndPoint.ToString() + "::::--------------" + byteToHexStr(frameBytes));
                    if (SendFinishedEvent != null)
                    {
                        Debug.WriteLine("1已经发送发:" + toSocket.RemoteEndPoint.ToString() + "::::--------------:--------------:--------------:--------------" + tempData);
                        SendFinishedEvent(this, _commEventMsg);
                    }
                }
                else
                {
                    foreach (var item in _clientSocket)
                    {
                        if (item.IsBound)
                        {
                            ///Debug.WriteLine("2准备发送发:" + item.RemoteEndPoint.ToString() + "::::--------------" + byteToHexStr(frameBytes));
                            item.Send(frameBytes);
                            Debug.WriteLine("2已经发送发:" + item.RemoteEndPoint.ToString() + "::::--------------:--------------:--------------:--------------" + tempData);
                            if (SendFinishedEvent != null)
                            {
                                ///Debug.WriteLine("2已经发送:：：：：：：：：：：：：：：：开始执行事件SendFinishedEvent" + item.RemoteEndPoint.ToString());
                                SendFinishedEvent(this, _commEventMsg);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX出错：SendData：：：：：" + e.Message);
                OnException("Closed");
            }

        }

        /// <summary>
        /// 数据发送成功回调函数，出发发送成功事件
        /// </summary>
        /// <param name="ar"></param>
        private void SendFrameCallBack(IAsyncResult ar)
        {
            try
            {
                #region
                //Socket clientSTemp = ar.AsyncState as Socket;
                //int bytesNum = clientSTemp.EndSend(ar);               //结束掉活着挂起开启的异步发送线程，返回发送的字节数表示发送成功
                //把接收到帧放到buffer
                //clientSTemp.Receive(buffer);
                //frameTE.Port = ((IPEndPoint)clientSTemp.RemoteEndPoint).Port;
                //frameTE.FrameData = buffer;
                #endregion
                string tempStr = ar.AsyncState as string;
                //_commEventMsg = new CommEventArgs();
                //_commEventMsg.State = "SENDED";
                //_commEventMsg.StrArgs = tempStr;
                //if (SendFinishedEvent != null)
                //{
                //    SendFinishedEvent(this, _commEventMsg);
                //}
            }
            catch (Exception e)
            {
                Debug.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX出错：SendFrameCallBack：：：：：" + e.Message);                                            //关闭连接
                OnException(e.Message.ToString());
            }
        }


        /// <summary>
        /// byte转换string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                returnStr += bytes[i].ToString("x2");
            }
            return returnStr;
        }


        /// <summary>
        /// 移除一个客户端组socket中的一个
        /// </summary>
        /// <param name="soc"></param>
        private void RemoveLogoutSocket(Socket soc)
        {
            try
            {
                int port = ((IPEndPoint)soc.RemoteEndPoint).Port;
                soc.Shutdown(SocketShutdown.Both);
                soc.Close();
                _clientSocket.Remove(soc);

                ///要做修改，是哪个链接的客户端报错，即登出掉线，问题，至少应该传回port
                Exit(port);
            }
            catch
            { }
        }


        /// <summary>
        /// 清除过期的连接，保证每次都只有一个连接
        /// </summary>
        public void ClearTimeOutSocket()
        {
            foreach (var item in _clientSocket)
            {
                Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>结束旧连接过来的socket 端口：" + item.RemoteEndPoint.ToString());
                item.Shutdown(SocketShutdown.Both);
                item.Close();
            }
            _clientSocket.Clear();
        }

        /// <summary>
        /// 关闭所有通信资源
        /// </summary>
        public void Close(string closeMes)
        {
            CommEventArgs _commEventMsg = new CommEventArgs();
            _commEventMsg.State = "Closed";
            try
            {
                //_serverSocket.Shutdown(SocketShutdown.Both);

                foreach (var item in _clientSocket)
                {
                    item.Close();
                }
                _serverSocket.Close();
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// 远程那个接入点端口连接
        /// </summary>
        /// <param name="RemotProt">连接的目标端口号</param>
        private void Exit(int RemotProt)
        {
            CommEventArgs _commEventMsg = new CommEventArgs();
            _commEventMsg.State = "Exit";
            _commEventMsg.port = RemotProt;
            if (this.ExceptionEvent != null)
            {
                ExceptionEvent(this, _commEventMsg);
            }
        }

        /// <summary>
        /// 报错
        /// </summary>
        /// <param name="Msg"></param>
        private void OnException(string Msg)
        {
            if (this.ExceptionEvent != null)
            {
                CommEventArgs _commEventMsg = new CommEventArgs();
                _commEventMsg.State = "Ex";
                _commEventMsg.StrArgs = Msg;
                ExceptionEvent.Invoke(this, _commEventMsg);             //触发报错回调
            }
        }


        /// <summary>
        /// 垃圾回收时，释放本地资源
        /// </summary>
        ~CommWithSocketOfServer()
        {
            _serverSocket.Close();
            foreach (var item in _clientSocket)
            {
                item.Close();
            }
        }
    }
}
