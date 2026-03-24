using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.SocketEven
{
    public class DevOrderEven
    {
        public delegate void DevOrderEvent(string GetSetMsg);
        public static event DevOrderEvent DevOrder;

        public static void DevOrderMsg(string GetSetMsg)
        {
            DevOrder?.Invoke(GetSetMsg);
        }
    }
}
