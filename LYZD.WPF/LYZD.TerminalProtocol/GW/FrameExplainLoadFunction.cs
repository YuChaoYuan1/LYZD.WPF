using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LYZD.TerminalProtocol.GW
{
    public class FrameExplainLoadFunction
    {

        /// <summary>
        /// 从这个方法对象里面找到要用的对象
        /// </summary>
        public FrameDataFormtUtil TheExplainUtil
        {
            get;
            set;
        }

        public FrameExplainLoadFunction()
        {
            TheExplainUtil = new FrameDataFormtUtil();
        }

        public MethodInfo TheFunction
        {
            get;
            set;
        }

        public object GetTheData(string functionName, List<object> functionArges)
        {
            TheFunction = typeof(FrameDataFormtUtil).GetMethod(functionName);
            return TheFunction.Invoke(TheExplainUtil, functionArges.ToArray());
        }

        /// <summary>
        /// 如果是 ASII 和Bin 和BS类型要穿
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="sumData"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public byte[] GetTheFnSubBytes(string functionName, FrameFnSubDataValue sumData)
        {
            var tempArgs = new List<object>();
            ///规定死最后一个是一个参数试长度
            tempArgs.AddRange(sumData.FrameDataValue_Value);
            tempArgs.Add(sumData.FrameDataValue_Length);
            TheFunction = typeof(FrameDataFormtUtil).GetMethod(functionName, new Type[] { typeof(string), typeof(int) });
            //GetMethod("MethodName", new Type[] { typeof(参数类型) });
            return TheFunction.Invoke(TheExplainUtil, tempArgs.ToArray()) as byte[];
        }
    }
}
