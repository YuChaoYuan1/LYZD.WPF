using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LYZD.SocketComm
{
    public class TcpServerTool
    {
        #region 事件委托
        /// <summary>
        /// 执行操作消息变更
        /// </summary>
        public event EventHandler<string> ExecuteMessageChanged;
        /// <summary>
        /// 客户端个数变更
        /// </summary>
        public event EventHandler<string> ClientCountChanged;

        public event EventHandler ReceivedFinshedEvent;

        
        #endregion

        #region 字段、属性
        private string _ip = "127.0.0.1";           //ip
        private int _port = 9000;                   //端口
        private string _ipPort = "127.0.0.1:9000";  //
        private bool _isConnected = false;          //是否连接
        private bool _isListened = false;           //是否侦听
        private NetworkStream _stream;              //网络基础流
        private Socket _serverSocket;               //服务端套接字对象
        private Thread listenThread = null;         //侦听线程
        private List<string> _clientIpPortList = new List<string>();   //客户端Ip端口集合
        private List<Socket> _clientSocketList = new List<Socket>();    //客户端套接字对象集合    
        private List<Thread> _clientSocketThreadList = new List<Thread>(); //接收线程：接收客户端对象集合

        public string IP { get => _ip; set => _ip = value; }
        public int Port { get => _port; set => _port = value; }
        public bool IsConnected { get => _isConnected; set => _isConnected = value; }
        public bool IsListened { get => _isListened; set => _isListened = value; }
        public NetworkStream Stream { get => _stream; set => _stream = value; }
        public Socket ServerSocket { get => _serverSocket; set => _serverSocket = value; }
        public List<string> ClientIpPortList { get => _clientIpPortList; set => _clientIpPortList = value; }
        public List<Socket> ClientSocketList { get => _clientSocketList; set => _clientSocketList = value; }
        public List<Thread> ClientSocketThreadList { get => _clientSocketThreadList; set => _clientSocketThreadList = value; }
        public string IpPort { get => _ipPort; set => _ipPort = value; }
        #endregion

        #region 构造方法
        public TcpServerTool(string ip, int port)
        {
            this.IP = ip;
            this.Port = port;
        }
        public TcpServerTool(string ip, string port)
        {
            this.IP = ip;
            if (int.TryParse(port, out int portStr))
            {
                this.Port = portStr;
            }
        }
        #endregion

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            //状态
            IsListened = false;
            IsConnected = false;

            foreach (Thread item in ClientSocketThreadList)
            {
                item.Abort();
            }
            //关闭对象集合，清除集合项
            foreach (Socket item in ClientSocketList)
            {
                item.Close();
            }

            //关闭线程
            listenThread?.Abort();
            listenThread = null;

            //关闭流
            Stream?.Close();
            ServerSocket?.Close();
            ServerSocket = null;
            Stream = null;

            ClientSocketThreadList?.Clear();
            ClientSocketList?.Clear();
            ClientIpPortList?.Clear();
        }
        /// <summary>
        /// 服务端打开
        /// </summary>
        public void Open()
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(IP);  //IP地址
                // 创建一个新的 Socket 对象，指定为 IPv4、面向流的（TCP）协议
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //允许套接字复用
                ServerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                // 服务器绑定指定终端（IP,Port）
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);//创建终端
                ServerSocket.Bind(localEndPoint);   //绑定终端

                //ExecuteMessageChanged?.Invoke(this, $"开始侦听准备...");
                ServerSocket.Listen(100);
                
                //创建并使用线程侦听
                listenThread = new Thread(OnListenClient);
                listenThread.IsBackground = true;
                listenThread.Start();
                IsListened = true;

                //监听线程安全
                Thread monitorThread = new Thread(() =>
                {
                    while (IsListened)  // 循环检测直到服务停止
                    {
                        try
                        {
                            // 执行检测
                            SocketStart();
                            // 等待指定时间后再次检测
                            Thread.Sleep(3 * 1000);
                        }
                        catch (ThreadAbortException)
                        {
                            break;
                        }
                        catch (Exception ex)
                        {
                            ExecuteMessageChanged?.Invoke(this, $"连接监控异常：{ex.Message}");
                         
                        }
                    }
                });
                monitorThread.Start();
            }
            catch (Exception ex)
            {
                ExecuteMessageChanged?.Invoke(this, $"创建连接失败....");
                ExecuteMessageChanged?.Invoke(this, $"{ex.Message}");
            }
        }

        /// <summary>
        /// 侦听客户端
        /// </summary>
        public void OnListenClient()
        {
            try
            {
                while (true)
                {
                    //接受一个客户端的连接请求
                    Socket socket = ServerSocket.Accept();
                    ExecuteMessageChanged?.Invoke(this, $"收到来自【{socket.LocalEndPoint}】远程终端的连接请求...");

                    //创建接收数据线程
                    Thread thread = new Thread(Received);
                    thread.Name = (ClientSocketThreadList.Count + 1) + "";
                    thread.IsBackground = true;
                    thread.Start(socket);

                    //添加对象到集合
                    ClientIpPortList.Add(socket.RemoteEndPoint.ToString());  //添加远程终端到集合
                    ClientSocketList.Add(socket);                                   //添加Socket对现象到集合
                    ClientSocketThreadList.Add(thread);                             //创建对应的客户端Socket线程对象并添加到集合

                    //触发客户端个数变更事件
                    ClientCountChanged?.Invoke("Add", socket.RemoteEndPoint.ToString());
                }
            }
            catch (Exception ex)
            {
                ExecuteMessageChanged?.Invoke(this, $"侦听异常：{ex.Message}");
            }
        }
        /// <summary>
        /// 接收数据方法
        /// </summary>
        public void Received(object socketClientPara)
        {
            Socket socketServer = socketClientPara as Socket;
            string remoteEndPoint = socketServer.RemoteEndPoint.ToString(); ;
            while (true)
            {
                try
                {
                    // 读取客户端发送的数据
                    byte[] buffer = new byte[1024 * 1024];
                    if (socketServer == null) break;
                    // 接收客户端发来的数据
                    int dataLength = socketServer.Receive(buffer);
                    // 将接收的数据转换为字符串并输出
                    byte[] tempResult = new byte[dataLength];
                    Array.Copy(buffer, 0, tempResult, 0, dataLength);
                    string dataReceived = ByteToHexStr(tempResult);

                    if (ReceivedFinshedEvent != null)
                    {
                        CommEventArgs _commEventMsg = new CommEventArgs();
                        _commEventMsg.State = "Conneted";
                        /// 正确格式： _commEventMsg.StrArgs = sRecieved;
                        _commEventMsg.StrArgs = dataReceived;
                        _commEventMsg.Ip_Port = ((IPEndPoint)socketServer.RemoteEndPoint).Address.ToString()+":"+ ((IPEndPoint)socketServer.RemoteEndPoint).Port;
                        ///自动回复时，发送给接收到的帧，这里要枷锁
                        ReceivedFinshedEvent(this, _commEventMsg);
                    }
                    //ExecuteMessageChanged.Invoke(this, "接收数据：");
                    //ExecuteMessageChanged.Invoke(this, $"{socketServer.RemoteEndPoint}->{dataReceived}");
                }
                catch (Exception ex)
                {
                    if (IsListened)
                    {
                        ClientIpPortList.Remove(remoteEndPoint);
                        ClientCountChanged?.Invoke("Remove", remoteEndPoint);
                        Stream = null;
                        ExecuteMessageChanged.Invoke(this, "客户端已断开连接！");
                        ExecuteMessageChanged.Invoke(this, $"接收异常：{ex.Message}");

                        ClientSocketList.Find(s => s.RemoteEndPoint.Equals(remoteEndPoint))?.Close();
                        ClientSocketList.Remove(socketServer);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 发送数据：根据指定IpPort,
        /// </summary>
        public void Send(string ipPort, byte[] dataBytes)
        {
            try
            {
                if (IsListened)
                {
                    string socketIpPort = ClientIpPortList.Find(s => s.Equals(ipPort));
                    Socket socket = ClientSocketList.Find(s => s.RemoteEndPoint.ToString().Equals(ipPort));
                    if (socket != null)
                    {
                        Stream = new NetworkStream(socket);
                        Stream.Write(dataBytes, 0, dataBytes.Length);
                        ExecuteMessageChanged?.Invoke(this, $"发送数据长度：{dataBytes.Length}");
                    }
                    else
                    {
                        ExecuteMessageChanged?.Invoke(this, $"发送失败！找不到ip地址为"+ ipPort+"的端口");
                    }
                }
            }
            catch (Exception ex)
            {
                ExecuteMessageChanged?.Invoke(this, $"发送异常：{ex.Message}");
            }
        }
        /// <summary>
        /// 发送数据：根据指定Socket对象
        /// </summary>
        public void Send(Socket socket, string data)
        {
            try
            {
                if (IsListened)
                {
                    if (Stream != null)
                    {
                        string dataToSend = data;
                        byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSend);
                        Stream.Write(dataBytes, 0, dataBytes.Length);
                        ExecuteMessageChanged?.Invoke(this, $"发送数据长度：{dataBytes.Length}");
                    }
                    else
                    {
                        Stream = new NetworkStream(socket);
                        string dataToSend = data;
                        byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSend);
                        Stream.Write(dataBytes, 0, dataBytes.Length);
                        ExecuteMessageChanged?.Invoke(this, $"发送数据长度：{dataBytes.Length}");
                    }
                }
            }
            catch (Exception ex)
            {
                ExecuteMessageChanged?.Invoke(this, $"发送异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 群发数据:发送数据到所有在连接客户端。
        /// </summary>
        /// <param name="data"></param>
        public void SendGroup(byte[] dataBytes)
        {
            try
            {
                if (IsListened)
                {
                    foreach (Socket socket in ClientSocketList)
                    {
                        Stream = new NetworkStream(socket);
                        Stream.Write(dataBytes, 0, dataBytes.Length);
                        ExecuteMessageChanged.Invoke(this, $"发送到终端：{socket.RemoteEndPoint}");
                        ExecuteMessageChanged.Invoke(this, $"协议版本：{socket.RemoteEndPoint.AddressFamily}");
                        ExecuteMessageChanged.Invoke(this, $"发送数据长度：{dataBytes.Length}");
                    }
                }
            }
            catch (Exception ex)
            {
                ExecuteMessageChanged.Invoke(this, $"发送异常：{ex.Message}");
            }
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


        public void SocketStart()
        {
            List<Socket> socketsToCheck = null;

            // 线程安全地获取Socket列表副本
            lock (ClientSocketList)
            {
                socketsToCheck = ClientSocketList.ToList();
            }

            foreach (Socket socket in socketsToCheck)
            {
                try
                {
                    if (!IsConnectedBySend(socket))
                    {
                        string remoteEndPoint = socket.RemoteEndPoint?.ToString();
                        if (!string.IsNullOrEmpty(remoteEndPoint))
                        {
                            // 触发断开事件
                            ClientCountChanged?.Invoke("Remove", remoteEndPoint);
                            ExecuteMessageChanged?.Invoke(this, $"检测到客户端【{remoteEndPoint}】已断开");

                            // 从列表中移除
                            lock (ClientSocketList)
                            {
                                ClientSocketList.Remove(socket);
                                ClientIpPortList.Remove(remoteEndPoint);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExecuteMessageChanged?.Invoke(this, $"检测客户端异常：{ex.Message}");
                }
            }
        }

        public bool IsConnectedBySend(Socket socket)
        {
            if (socket == null) return false;

            if (socket == null || !socket.Connected) return false;

            try
            {
                // 使用同步方法（最简单可靠）
                socket.Send(new byte[0], 0, SocketFlags.None);
                return true;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionReset ||
                    ex.SocketErrorCode == SocketError.ConnectionAborted ||
                    ex.SocketErrorCode == SocketError.Shutdown)
                {
                    return false;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
