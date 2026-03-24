using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LYZD.SocketComm
{
    internal class UDPClient : IConnection
    {
        private int int_UdpBindPort;
        private UdpClient Client;
        //private UdpClient settingClient;
        //private string str_BaudRate = "1200,e,8,1";
        private IPEndPoint ip_RemotePoint = new IPEndPoint(IPAddress.Parse("193.168.18.1"), 10003);
        private IPEndPoint ip_LocalPoint = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Ip"></param>
        /// <param name="p_int_BindPort"></param>
        /// <param name="p_int_RemotePort"></param>
        /// <param name="p_int_BasePort"></param>
        public UDPClient(string p_str_Ip, int p_int_BindPort, int p_int_RemotePort, int p_int_BasePort)
        {
            ip_RemotePoint.Address = IPAddress.Parse(p_str_Ip);
            ip_RemotePoint.Port = p_int_RemotePort;
            int_UdpBindPort = LocalPortTo2011Port(p_int_BindPort, p_int_BasePort);//转换端口成2018端口
            ip_LocalPoint = new IPEndPoint(IPAddress.Parse(GetSubnetworkIP(p_str_Ip)), int_UdpBindPort);
        }

        public static uint IPToUint(string p_str_IpAddress)
        {
            string[] strs = p_str_IpAddress.Trim().Split('.');
            byte[] buf = new byte[4];

            for (int i = 0; i < strs.Length; i++)
            {
                buf[i] = byte.Parse(strs[i]);
            }
            Array.Reverse(buf);

            return BitConverter.ToUInt32(buf, 0);
        }

        public static string GetSubnetworkIP(string p_str_TargetIP)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\Tcpip\Parameters\Interfaces", RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey);
            uint iTarget = IPToUint(p_str_TargetIP);
            foreach (string keyName in key.GetSubKeyNames())
            {
                try
                {
                    RegistryKey tmpKey = key.OpenSubKey(keyName);
                    string[] ip = tmpKey.GetValue("IPAddress") as string[];
                    if (ip == null)
                    {
                        continue;
                    }
                    string[] subnet = tmpKey.GetValue("SubnetMask") as string[];
                    for (int i = 0; i < ip.Length; i++)
                    {
                        IPAddress local = IPAddress.Parse(ip[i]);
                        if (local.IsIPv6SiteLocal)
                            continue;

                        uint iIP = IPToUint(ip[i]);
                        uint iSub = IPToUint(subnet[i]);

                        if ((iIP & iSub) == (iTarget & iSub))
                        {
                            return ip[i];
                        }
                    }
                }
                catch
                {
                }
            }
            return "127.0.0.1";
            //throw new Exception("未在本计算机上找到与目标地址：" + targetIP + " 相匹配的网段IP地址，无法发送数据,请配置应用的IP地址");
        }

        public bool SendData(ref byte[] p_byt_Data, bool p_bol_IsReturn, int p_int_WaitTime)
        {
            try
            {
                lock (this)
                {
                    Client = new UdpClient();
                    Client.Client.Bind(this.ip_LocalPoint);
                }
                Client.Connect(ip_RemotePoint);
            }
            catch { return false; }
            Client.Send(p_byt_Data, p_byt_Data.Length);
            //Console.WriteLine(int_UdpBindPort.ToString());
            //Console.WriteLine("┏SendData:{0}", BitConverter.ToString(p_byt_Data));
            if (!p_bol_IsReturn)
            {
                //Console.WriteLine("┗本包不需要回复");
                Client.Close();
                return true;
            }
            Thread.Sleep(p_int_WaitTime);
            byte[] byt_Received = new byte[0];
            bool bol_IsReceived = false;    // 返回标识
            List<byte> lst_RevItems = new List<byte>();     // 接收的数据集合
            DateTime dt_Wait;   // 等待时间变量
            dt_Wait = DateTime.Now;
            while (TimeSub(DateTime.Now, dt_Wait) < MaxWaitSenconds)
            {
                Thread.Sleep(1);
                try
                {
                    if (Client.Available > 0)
                    {
                        byt_Received = Client.Receive(ref ip_RemotePoint);
                        bol_IsReceived = true;
                        break;
                    }
                }
                catch
                {
                    Client.Close();
                    return false;
                }
            }

            if (!bol_IsReceived)
            {
                p_byt_Data = new byte[0];
            }
            else  
            {
                lst_RevItems.AddRange(byt_Received);
                dt_Wait = DateTime.Now;
                while (TimeSub(DateTime.Now, dt_Wait) < p_int_WaitTime)
                {
                    if (Client.Available > 0)
                    {
                        byt_Received = Client.Receive(ref ip_RemotePoint);
                        lst_RevItems.AddRange(byt_Received);
                        dt_Wait = DateTime.Now;
                    }
                }
                p_byt_Data = lst_RevItems.ToArray();
            }
            //Console.WriteLine("┗RecvData:{0}", BitConverter.ToString(byt_Received));
            Client.Close();
            return true;
        }

        /// <summary>
        /// 本地通道转换成2018端口:20000 + 2 * (port - 1);
        /// 数据端口，设置端口在数据端口的基础上+1
        /// </summary>
        /// <param name="p_int_Port"></param>
        /// <param name="p_int_BasePort"></param>
        /// <returns></returns>
        private int LocalPortTo2011Port(int p_int_Port, int p_int_BasePort)
        {
            return p_int_BasePort + 2 * (p_int_Port - 1);
        }

        private long TimeSub(DateTime Time1, DateTime Time2)
        {
            TimeSpan tsSub = Time1.Subtract(Time2);
            return tsSub.Hours * 60 * 60 * 1000 + tsSub.Minutes * 60 * 1000 + tsSub.Seconds * 1000 + tsSub.Milliseconds;
        }



        #region IConnection 成员

        public string ConnectName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int MaxWaitSenconds
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int WaitSecondsPerByte
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool Open()
        {
            throw new NotImplementedException();
        }

        public bool Close()
        {
            throw new NotImplementedException();
        }

        public bool UpdateBaudSetting(string p_str_BaudRate)
        {
            throw new NotImplementedException();
        }

        //public bool SendData(ref byte[] p_byt_Data, bool p_bol_IsReturn, int p_int_WaitTime)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion
    }
}
