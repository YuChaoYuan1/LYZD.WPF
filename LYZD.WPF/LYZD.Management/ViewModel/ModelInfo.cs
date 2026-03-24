using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.DataManager.ViewModel
{
    public class ModelInfo
    {
        public  string SerialNumber { get; set; }
        public  string FaultType { get; set; }
        public  StringBuilder BarCode { get; set; }
        public  string Verifier { get; set; }
        public  string CHeckTime { get; set; }
    }
}
