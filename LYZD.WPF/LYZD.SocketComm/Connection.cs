using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace LYZD.SocketComm
{
    /// <summary>
    /// 通讯连接
    /// </summary>
    internal class Connection
    {
        private object objSendLock = new object();
        private object objPackLock = new object();

        /// <summary>
        /// 连接对象
        /// </summary>
        IConnection connection = null;


        public Connection(IPAddress p_ip_RemoteIP, int p_int_RemotePort, int p_int_LocalPort, int p_int_BasePort, int p_int_MaxWaitSenconds, int p_int_WaitSencondsPerByte)
        {
            //connection = new UdpClient()
        }

        public bool Close()
        {
            if (connection == null) return true;
            return connection.Close();
        }
    }
}
