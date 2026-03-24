using lib60870.CS101;
using System;
using System.Collections.Generic;
namespace LYZD.TerminalProtocol
{
    public class ConnectStatus
    {
        public bool Connected;

        public void SetPairs(int key, PointValue pval)
        {
            if (_valuePairs.ContainsKey(key))
            {
                _valuePairs[key] = pval;
            }
            else
            {
                _valuePairs.Add(key, pval);
            }
        }
        /// <summary>
        /// 获取命令返回信息
        /// </summary>
        /// <param name="key">TI的值</param>
        /// <returns></returns>
        public PointValue GetValue(int key)
        {
            if (_valuePairs.ContainsKey(key))
            {
                return _valuePairs[key];
            }
            else
            {
                return new PointValue();
            }
        }
        private readonly Dictionary<int, PointValue> _valuePairs = new Dictionary<int, PointValue>();

    }
    public class PointValue
    {
        public CauseOfTransmission Cause;
        /// <summary>
        /// 肯定确认
        /// </summary>
        public bool IsPositive;
        /// <summary>
        /// 消息文本
        /// </summary>
        public string val;
        /// <summary>
        /// 带时标命令的时标
        /// </summary>
        public DateTime Timestamp;
        /// <summary>
        /// 本地接收时间
        /// </summary>
        public DateTime dtime;

        public InformationObject Element;
    }
}
