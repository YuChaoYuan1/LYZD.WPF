using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.SocketEven
{
    public class SerPortEven
    {
        public delegate void SetEven(byte[] GetSetMsg, string spCom);
        public static event SetEven setEven;

        public static void GetSeriPostMsg(byte[] retBuff, string spCom)
        {
            setEven?.Invoke(retBuff, spCom);
        }
    }
}
