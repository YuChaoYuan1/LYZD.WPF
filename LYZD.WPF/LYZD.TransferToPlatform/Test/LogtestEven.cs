using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.Test
{
    public class LogtestEven
    {
        public delegate void Logtest (string GetSetMsg);
        public static event Logtest LTest;

        public static void add(string GetSetMsg)
        {
            LTest?.Invoke(GetSetMsg);
        }
    }
}
