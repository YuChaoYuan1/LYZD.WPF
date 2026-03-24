using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseInitialized
{
    public class TerminalEven
    {
        public delegate void TerminalEvent(string GetSetMsg);
        public static event TerminalEvent Terminal;

        public static void TerminalMsg(string GetSetMsg)
        {
            Terminal?.Invoke(GetSetMsg);
        }
    }
}
