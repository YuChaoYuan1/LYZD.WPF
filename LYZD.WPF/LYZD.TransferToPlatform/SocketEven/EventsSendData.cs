using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.SocketEven
{
    /// <summary>
    /// 接受数据类
    /// </summary>
    public class EventsSendData
    {
        public EventsSendData()
        {
            CMD = "";
            Data = "";
        }
        public string CMD { get; set; }
        public string Data { get; set; }
    }
}
