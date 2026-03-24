using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.ViewModel
{
    public class EventsRecUDPData
    {
        public EventsRecUDPData()
        {
            Remote = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6666);
            Data = new byte[1024];
            rev = 0;
            dateTime = DateTime.Now;
        }


        public EndPoint Remote { get; set; }
        public byte[] Data { get; set; }
        public int rev { get; set; }
        public DateTime dateTime { get; set; }
    }

}
