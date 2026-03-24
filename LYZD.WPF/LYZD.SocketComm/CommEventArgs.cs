using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.SocketComm
{
    /// <summary>
    /// 传输直之间的参数
    /// </summary>
    public class CommEventArgs : EventArgs
    {
        /// <summary>
        /// 发送参数
        /// </summary>
        public string StrArgs;
        /// <summary>
        /// 状态参数 AFN/C
        /// </summary>
        public string State;
        /// <summary>
        /// 对方端口
        /// </summary>
        public int port;
        /// <summary>
        /// 对方端口
        /// </summary>
        public string Ip_Port;
        /// <summary>
        /// 广东规约用的 ID和Data对应的
        /// </summary>
        public Dictionary<string, string> DIandData;
    }
    /// <summary>
    /// 接收到否认帧
    /// </summary>
    public class ReturnDenyDataEvent : EventArgs
    {
        public string SendData
        {
            get;
            set;
        }

        public string ReciveData
        {
            get;
            set;
        }
    }

    public class SendFailEventArgs : EventArgs
    {
        public string ZhenStr
        {
            get;
            set;
        }
    }


    public enum CEArgsState
    {
        Connected,
        OK,
        Close,
        Sended,
        Received,
        Exception
    }
}
