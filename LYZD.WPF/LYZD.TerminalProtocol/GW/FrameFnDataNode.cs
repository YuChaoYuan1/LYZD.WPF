using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LYZD.TerminalProtocol.GW
{
    public class FrameFnDataNode
    {
        /// <summary>
        /// 加载规约XML文档，设置为static表示共用一个，少内存使用  
        /// </summary>
        public static XElement FrameDataNodeRoot = XElement.Load("376.1ProDocument.xml");

        /// <summary>
        /// 表示一个翻身方法组
        /// </summary>
        public static FrameExplainLoadFunction ExplainFunction = new FrameExplainLoadFunction();


        public int FnNodeAfn
        {
            get;
            set;
        }

        /// <summary>
        /// FN
        /// </summary>
        public int FrameDataNodeFn
        {
            get;
            set;
        }

        /// <summary>
        /// Pn
        /// </summary>
        public int FrameDataNodePn
        {
            get;
            set;
        }

        /// <summary>
        /// 这个真对应的Node的string值
        /// </summary>
        public string FrameDataNodeStr
        {
            get;
            set;
        }

        /// <summary>
        /// 帧数据项名称
        /// </summary>
        public string FrameDataNodeName
        {
            get;
            set;
        }

        /// <summary>
        /// 数据帧字节长度   
        /// </summary>
        public int FrameDataNodeLenght
        {
            get
            {
                int len = 0;
                SubFrameDataValues.ForEach((x) => { len += x.FrameDataValue_Length * 2; });
                return len;
            }
            set
            { }
        }

        /// <summary>
        /// 某一个帧数据结构相中某个Fn的子项集合，在帧解析的时候，解析完整的子项集合，在往协议层传参数的时候也是一个集合，没个子项有一个值集合List<object>
        /// </summary>  
        public List<FrameFnSubDataValue> SubFrameDataValues
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public int IndexSubFrameData
        {
            get;
            set;
        }

        /// <summary>
        /// 是否为上行帧
        /// </summary>
        public bool BoolUp
        {
            get;
            set;
        }



        /// <summary>
        /// 
        /// </summary>
        public FrameFnDataNode()
        {
        }

        /// <summary>
        /// 一条帧，在这个协议解析里面就可以自己搞定了
        /// </summary>
        /// <param name="frameData"></param>
        public FrameFnDataNode(int afn, string frameData, bool boolUp)
        {
            FnNodeAfn = afn;
            BoolUp = boolUp;
            IniFrameConfig(frameData);
        }

        /// <summary>
        /// 初始化配置一个帧数据
        /// </summary>
        private void IniFrameConfig(string frameDataStr)
        {
            SubFrameDataValues = new List<FrameFnSubDataValue>();

            int dt1 = Convert.ToInt32(frameDataStr.Substring(4, 2), 16);
            int dt2 = Convert.ToInt32(frameDataStr.Substring(6, 2), 16);

            int da1 = Convert.ToInt32(frameDataStr.Substring(0, 2), 16);
            int da2 = Convert.ToInt32(frameDataStr.Substring(2, 2), 16);


            ///在必要的时候要改变Fn   比如说事件的时候，因为要共用一套解析方法
            ///
            if (FnNodeAfn == 14)
                FrameDataNodeFn = Convert.ToInt32(frameDataStr.Substring(16, 2), 16);
            else
                FrameDataNodeFn = (dt2 * 8 + (int)(Math.Log(dt1, 2.0) + 1));

            FrameDataNodePn = (da2 - 1) * 8 + (int)(Math.Log(da1, 2.0) + 1) == 2147483640 ? 0 : (da2 - 1) * 8 + (int)(Math.Log(da1, 2.0) + 1);

            FrameDataNodeStr = frameDataStr.Remove(0, 8);           // 去掉FN PN剩下数据内容
            SubFrameDataValues = ExplainFrameStringToObject();
        }

        #region 测试
        //private void IniFrameConfig(string frameData)
        //{
        //    int dt1 = Convert.ToInt32(frameData.Substring(32, 2), 16);
        //    int dt2 = Convert.ToInt32(frameData.Substring(34, 2), 16);

        //    int da1 = Convert.ToInt32(frameData.Substring(28, 2), 16);
        //    int da2 = Convert.ToInt32(frameData.Substring(30, 2), 16);

        //    FrameDataNodeFn = (dt2 * 8 + (int)(Math.Log(dt1, 2.0) + 1));

        //    FrameDataNodePn = (da2 - 1) * 8 + (int)(Math.Log(da1, 2.0) + 1);

        //    FrameDataNodeStr = frameData.Substring(36, frameData.Length - 40);
        //    SubFrameDataValues = new List<FrameFnSubDataValue>();
        //    SubFrameDataValues = ExplainFrameStringToObject();
        //}
        #endregion


        /// <summary>
        /// 从给定的报文，解析出每个Fn对应的子项对象出来，   
        /// </summary>
        /// <returns>一个FN的所有子项值</returns>
        private List<FrameFnSubDataValue> ExplainFrameStringToObject()
        {
            //int theSubSize=0;
            ///这里已经获取了一个完整的Fn了，，先在要做的就是从这个Fn中获取每一个FrameDataValue  
            XElement theFrameNode = GetTheFNDocument();

            string frameData = FrameDataNodeStr;

            ///先添加头大的头
            var oneGroupFn = ExplainTheFnValue(theFrameNode, ref frameData, BoolUp);
            FrameDataNodeName = oneGroupFn.FrameDataValue_Name;

            if (FnNodeAfn == 4 || FnNodeAfn == 12 || FnNodeAfn == 13 || FnNodeAfn == 14)
            {
                if (!BoolUp)
                {
                    return new List<FrameFnSubDataValue>();
                }
            }

            ///判断是不是单个没子数据项的Fn
            if (theFrameNode.IsEmpty)
            {
                SubFrameDataValues.Add(oneGroupFn);
            }
            else
            {
                ///Fn里面的每一项的集合  
                var tempE =
                    from el in theFrameNode.Elements("FrameSubNod")
                    select el;
                ///每一个Fn元素，这个元素可以包含子元素
                foreach (XElement el in tempE)
                {
                    SubFrameDataValues.AddRange(GetOneFnDataValues(el, ref frameData));
                    //FrameDataNodeStr = FrameDataNodeStr.Substring(theSubSize);
                }
                #region 循环每一项Fn子项
                //SubFrameDataValues = GetOneFnDataValues(theFrameNode.Element("FrameSubNod"), FrameDataNodeStr, ref theSubSize);
                //FrameDataNodeStr = FrameDataNodeStr.Substring(theSubSize);
                //tempDatas.AddRange(GetOneFnDataValues(el, FrameDataNodeStr, ref theSubSize));
                #endregion               
            }
            return SubFrameDataValues;
        }

        /// <summary>
        /// 从一个AFN - 对对应的Fn Object对象转换成一组byte用于发送
        /// </summary>
        /// <returns></returns>
        public List<byte> OrgFrameObjectToBytes()
        {
            var tempDatabytes = new List<byte>();
            var theFrameNode = GetTheFNDocument();
            if (theFrameNode.IsEmpty)
            {
                var functionName = theFrameNode.Attribute("orgFunc").Value;
                return OrgFrameFunSubDataBytes(functionName, SubFrameDataValues[IndexSubFrameData]);
            }
            ///Fn里面的每一项的集合
            var tempE =
                from el in theFrameNode.Elements("FrameSubNod")
                select el;
            foreach (XElement el in tempE)
            {
                tempDatabytes.AddRange(GetOneFnDataBytes(el));
                ///每向前前进一个子项，指针指向下一个
                IndexSubFrameData++;
            }
            return tempDatabytes;
        }

        /// <summary>
        /// 递归XML文件递归出每个格式文档 
        /// </summary>
        /// <param name="theFrameNode"></param>
        /// <param name="frameDataNodeStr"></param>
        /// <param name="theSubSize"></param>
        /// <returns></returns>
        public List<FrameFnSubDataValue> GetOneFnDataValues(XElement theFrameNode, ref string frameDataNodeStr)
        {
            var tempDatas = new List<FrameFnSubDataValue>();
            if (theFrameNode.Elements().Count() == 0)
            {
                ///最底层一个 Fn子项
                var oneBottomSunFnValue = ExplainTheFnValue(theFrameNode, ref frameDataNodeStr, BoolUp);
                return new List<FrameFnSubDataValue>() { oneBottomSunFnValue };
            }
            ///取到循环礼拜头的长度
            var oneGroupFn = ExplainTheFnValue(theFrameNode, ref frameDataNodeStr, BoolUp);
            tempDatas.Add(oneGroupFn);
            ///表示一个集合，是一个数组
            for (int i = 0; i < Convert.ToInt32(oneGroupFn.FrameDataValue_Value[0]); i++)
            {
                ///然后再取自节点
                IEnumerable tempE =
                    from el in theFrameNode.Elements("FrameSubNod")
                    select el;
                foreach (XElement el in tempE)
                {
                    tempDatas.AddRange(GetOneFnDataValues(el, ref frameDataNodeStr));
                }
            }
            return tempDatas;
        }

        /// <summary>
        /// 获取本帧的byte ，，，，
        /// </summary>
        /// <param name="theFrameNode"></param>
        /// <returns></returns>
        private List<byte> GetOneFnDataBytes(XElement theFrameNode)
        {
            var tempDatas = new List<byte>();
            var functionName = theFrameNode.Attribute("orgFunc").Value.ToString();
            ///把每个子对象的长度复制用于
            SubFrameDataValues[IndexSubFrameData].FrameDataValue_Length = Convert.ToInt32(theFrameNode.Attribute("length").Value);
            switch (theFrameNode.Elements().Count())
            {
                case 0:
                    return OrgFrameFunSubDataBytes(functionName, SubFrameDataValues[IndexSubFrameData]);
                default:
                    var oneGroupFn = OrgFrameFunSubDataBytes(functionName, SubFrameDataValues[IndexSubFrameData]);
                    tempDatas.AddRange(oneGroupFn);
                    ///表示一个集合，一个内嵌套 的组，，，然后去组额长度就
                    for (int i = 0;
                         i < Convert.ToInt32(SubFrameDataValues[IndexSubFrameData].FrameDataValue_Value[0]);
                         i++)
                    {
                        ///然后再取自节点
                        var tempE =
                            from el in theFrameNode.Elements("FrameSubNod")
                            select el;
                        foreach (XElement el in tempE)
                        {
                            ///每次循环是当前执着呢想后移动
                            IndexSubFrameData++;
                            ///去求下一个解析的bytes
                            tempDatas.AddRange(GetOneFnDataBytes(el));
                        }
                    }
                    return tempDatas;
            }
        }

        /// <summary>
        /// 没有子项的单个Fn某项，就是一行了，解析成一个value
        /// </summary>
        /// <param name="xelD">最子项，没有子项转换成一个FrameFnSubDataValue对象</param>
        /// <param name="theFrameStr"></param>
        /// <param name="size">子项的数据域长度，用这个来切割帧字符串</param>
        /// <returns></returns>
        public FrameFnSubDataValue ExplainTheFnValue(XElement xelD, ref string theFrameStr, bool BoolUp)
        {
            int size = Convert.ToInt32(xelD.Attribute("length").Value) * 2;
            if (FnNodeAfn == 4 || FnNodeAfn == 12 || FnNodeAfn == 13 || FnNodeAfn == 14)
            {
                if (!BoolUp)
                {
                    size = 0;
                }
            }
            var oneFnSub = new FrameFnSubDataValue
            {
                FrameDataValue_ID = xelD.Attribute("id").Value,
                FrameDataValue_Length = size / 2,
                FrameDataValue_Name = xelD.Attribute("name").Value,
                FrameDataValue_Value =
                                       new List<object>()
                                           {
                                               ExplainFunction.GetTheData(
                                                   xelD.Attribute("explainFunc").Value.ToString(),
                                                   new List<object>() {theFrameStr.Substring(0, size)})
                                           }
            };


            theFrameStr = theFrameStr.Substring(size);
            return oneFnSub;
        }

        /// <summary>
        /// 从一个Fn的每一项解析出来这个Fn子项的byt
        /// </summary>
        /// <param name="explainName"></param>
        /// <param name="oneSubFrame"></param>
        /// <returns></returns>
        public List<byte> OrgFrameFunSubDataBytes(string explainName, FrameFnSubDataValue oneSubFrame)
        {
            var tempData = ExplainFunction.GetTheFnSubBytes(explainName, oneSubFrame) as Byte[];
            return new List<byte>(tempData) { };
        }

        /// <summary>
        /// 通过本身的 AFN FN 获取其XMDOCUMENT
        /// </summary>
        /// <returns></returns>
        public XElement GetTheFNDocument()
        {
            ///查找出来FN了 
            var theAFN = from el in FrameDataNodeRoot.Descendants("ProAFNs").Elements("OneAFN")
                         where Convert.ToInt32((string)el.Attribute("id"), 16) == FnNodeAfn
                         select el;
            ///还要判断Fn里面有子元素没
            return theAFN.Cast<XElement>().Descendants("OneFnFrameNod").First(x => x.Attribute("id").Value == "F" + FrameDataNodeFn);
        }

        public Array ToArrayValue()
        {
            List<string> ValueList = new List<string>();
            ValueList.Add(string.Format("Fn={0}", FrameDataNodeFn));
            ValueList.Add(string.Format("Pn={0}", FrameDataNodePn));
            if (SubFrameDataValues.Count > 0 && SubFrameDataValues[0].FrameDataValue_Length > 0)
            {
                SubFrameDataValues.ForEach((x) =>
                {
                    Array tempAry = x.ToArrayValue();
                    if (tempAry != null && tempAry.Length > 0)
                    {
                        ValueList.AddRange(tempAry.Cast<string>());
                    }
                });
            }
            return ValueList.ToArray();
        }

        /// <summary>
        /// 打印一个FN
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //string tempAFN = "***************************************************************************\n";
            //tempAFN += "FN=" + FrameDataNodeFn.ToString().PadRight(7) + ComUtileFunction.PadRightEx("[" + FrameDataNodeName + "]", 60, ' ') + "PN=" + FrameDataNodePn + "\n";
            //tempAFN += "***************************************************************************\n";
            string tempAFN = "[Fn=" + FrameDataNodeFn + "," + FrameDataNodeName + "]" + "\n";
            tempAFN += "[Pn=" + FrameDataNodePn + "]" + "\n";
            string tempFNData = "";
            if (SubFrameDataValues.Count > 0 && SubFrameDataValues[0].FrameDataValue_Length > 0)
            {
                SubFrameDataValues.ForEach((x) => tempFNData += (x.ToString() + "\n"));
            }
            return tempAFN + tempFNData;
        }
    }
}
