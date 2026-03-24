using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LY.VirtualMeter.ViewModel
{
   public class SocketUdpControl
    {
        public static Socket SocketUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private static IPEndPoint IPLocalPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
        private static IPEndPoint IPRemotePoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5001);

        public static string Mac;

        public static bool Flag = false;
        public static bool isStartMac = false; //

        public static void Init()
        {
            if (!SocketUdp.IsBound) SocketUdp.Bind(IPLocalPoint);
        }

        public static void SendTo(string msg) {
            if (!SocketUdp.IsBound) SocketUdp.Bind(IPLocalPoint);
            //发送前重置MAC的值
            Mac = "12345678";
            Flag = false;
            if (!isStartMac) return;
            SocketUdp.SendTo(System.Text.ASCIIEncoding.ASCII.GetBytes(msg), IPRemotePoint);
            Thread.Sleep(200);
        }
    }
}
