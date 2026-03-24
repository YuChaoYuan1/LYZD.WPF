using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.DevicesFunc
{
    public static class Phase
    {
        //PhaseUa, PhaseUb, PhaseUc, PhaseIa, PhaseIb, PhaseIc
        /// <summary>
        /// A相电压角
        /// </summary>
        public static double PhaseUa =0.0;
        /// <summary>
        /// B相电压角
        /// </summary>
        public static double PhaseUb = 240.0;
        /// <summary>
        /// C相电压角
        /// </summary>
        public static double PhaseUc = 120.0;
        /// <summary>
        /// A相电流角
        /// </summary>
        public static double PhaseIa = 0.0;
        /// <summary>
        /// B相电流角
        /// </summary>
        public static double PhaseIb = 240.0;
        /// <summary>
        /// C相电流角
        /// </summary>
        public static double PhaseIc = 120.0;
    }
}
