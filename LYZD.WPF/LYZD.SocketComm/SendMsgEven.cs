using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.SocketComm
{
    public class SendMsgEven
    {
        public delegate void SendMsgEvent(byte[] GetSetMsg, string Ip_Port);
        public static event SendMsgEvent sendMsgEven;

        public static void GetSeriPostMsg(byte[] SendBuff, string Ip_Port)
        {
            sendMsgEven?.Invoke(SendBuff, Ip_Port);
        }
    }
}
