using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LYZD.TerminalProtocol.GW
{
    public class FrameAfnDataNode
    {
        /// <summary>
        /// 加载规约XML文档，设置为static表示共用一个，少内存使用  
        /// </summary>
        public static XElement FrameDataNodeRoot = XElement.Load("376.1ProDocument.xml");

        /// <summary>
        /// ANF
        /// </summary>
        public int FrameDataNodeAfn
        {
            get;
            set;
        }

        /// <summary>
        /// 一条帧的（一个AFN）的数据域（DADT DATA 数据标示 和数据）
        /// </summary>
        public string FrameDataArea
        {
            get;
            set;
        }

        public string FrameDataStr
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
        /// 可以有多个FN 和Pn对应的组成
        /// </summary>
        public List<FrameFnDataNode> FnDatas
        {
            get;
            set;
        }

        /// <summary>
        /// 帧是否为上行帧
        /// </summary>
        public bool BoolUp
        {
            get;
            set;
        }


        public FrameAfnDataNode()
        {
            FnDatas = new List<FrameFnDataNode>();
        }

        public FrameAfnDataNode(string frameData) : this()
        {
            FrameDataStr = frameData;
            SplitFNFromFrame(frameData);
        }

        public FrameAfnDataNode(string frameData, bool boolUp)
            : this()
        {
            FrameDataStr = frameData;
            BoolUp = boolUp;
            SplitFNFromFrame(frameData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameData"></param>
        private void SplitFNFromFrame(string frameData)
        {
            FrameDataNodeAfn = Convert.ToInt32(frameData.Substring(24, 2), 16);

            XElement TheFrameNode = GetTheAFNDocument();
            FrameDataNodeName = TheFrameNode.Attribute("name").Value;

            FrameDataArea = frameData.Substring(28, frameData.Length - 32 - 6);

            int theFnFrameIndex = 0;

            ///一个DADAT 一个数据10个
            while ((theFnFrameIndex) <= FrameDataArea.Length)
            {

                try
                {
                    //这里传值进去修改了AFN ，，，如果是是事件的时候要修改下事件的FN  每个FN表示一个事件
                    if ((FrameDataArea.Substring(theFnFrameIndex)).Length < 8) break;
                    var f = new FrameFnDataNode(FrameDataNodeAfn == 10 ? 4 : FrameDataNodeAfn, FrameDataArea.Substring(theFnFrameIndex), BoolUp);
                    FnDatas.Add(f);
                    theFnFrameIndex += (f.FrameDataNodeLenght + 8);
                }
                catch (System.Exception ex)
                {
                    break;
                }

            }
        }

        /// <summary>
        /// 通过本身的 AFN 获取其XMDOCUMENT
        /// </summary>
        /// <returns></returns>
        public XElement GetTheAFNDocument()
        {
            ///查找出来AFN了 
            var theAFN = from el in FrameDataNodeRoot.Descendants("ProAFNs").Elements("OneAFN")
                         where Convert.ToInt32((string)el.Attribute("id"), 16) == (FrameDataNodeAfn == 10 ? 4 : FrameDataNodeAfn)
                         select el;

            return theAFN.Cast<XElement>().First();
        }

        public Array ToArrayValue()
        {
            List<string> ValueList = new List<string>();
            ValueList.Add(string.Format("AFN={0}", FrameDataNodeAfn.ToString("x2").ToUpper()));
            FnDatas.ForEach((x) =>
            {
                Array tempAry = x.ToArrayValue();
                if (tempAry != null && tempAry.Length > 0)
                {
                    ValueList.AddRange(tempAry.Cast<string>());
                }
            });
            return ValueList.ToArray();
        }

        public override string ToString()
        {
            string tempAFN = "[帧]:" + FrameDataStr + "\n";
            tempAFN += "[AFN=" + FrameDataNodeAfn.ToString("x2").ToUpper() + "," + FrameDataNodeName + "]\n";
            FnDatas.ForEach((x) => { tempAFN += x.ToString(); });
            return tempAFN;
        }
    }

}
