using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using lib60870.CS101;


namespace LYZD.TerminalProtocol
{
    public class InformationData
    {
        public object Value;
        /// <summary>
        /// 真值，规一化值的，与Value等效
        /// </summary>
        public object RawValue;
        public Type ValueType;
        public QualityDescriptor Quality;
        public DateTime Timestamp;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isefault">空类型</param>
        public InformationData(bool isEmpty = false)
        {
            IsEmpty = isEmpty;
        }
        /// <summary>
        /// 空类型未赋值
        /// </summary>
        public bool IsEmpty { get; }
    }
}
