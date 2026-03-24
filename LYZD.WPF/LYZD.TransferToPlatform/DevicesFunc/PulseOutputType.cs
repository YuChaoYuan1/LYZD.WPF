using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.DevicesFunc
{
    public class PulseOutputType
    {
        /// <summary>
        /// 0x00=两组都不输出脉冲；0x01=仅第一组输出设定脉冲；0x02=仅第二组输出设定脉冲；0x03=两组都输出设定脉冲
        /// </summary>
        public byte contrnlType;
        /// <summary>
        /// 表位号
        /// </summary>
        public byte bwNum;
        /// <summary>
        /// 第一组频率
        /// </summary>
        public float GuoupOneFreq;
        /// <summary>
        /// 第一组占空比
        /// </summary>
        public float GuoupOnePWM;
        /// <summary>
        /// 第一组脉冲个数
        /// </summary>
        public int GuoupOneNum;
        /// <summary>
        /// 第二组频率
        /// </summary>
        public float GuoupTweFreq;
        /// <summary>
        /// 第二组占空比
        /// </summary>
        public float GuoupTwePWM;
        /// <summary>
        /// 第二组脉冲个数
        /// </summary>
        public int GuoupTweNum;

        public int DevicId;
    }
}
