using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.SocketEven
{
    public class EventsReceiveData
    {
        public EventsReceiveData()
        {
            CMD = "";
            Ret = "0";
            Data = "";
        }

        
        public string CMD { get; set; }
        public string Ret { get; set; }
        public string Data { get; set; }
    }
}
