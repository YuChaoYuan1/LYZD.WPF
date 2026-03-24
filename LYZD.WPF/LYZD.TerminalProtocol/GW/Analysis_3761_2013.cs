using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace LYZD.TerminalProtocol.GW
{
    [ComVisible(true)]
    public class Analysis_3761_2013 : Terminal
    {
        int iversions;//版本号,默认为376.1协议
        bool BoolUp;//判断是否为上行报文
        string Afn; int Pn; int Fn;
        public string Analysis(string strFrame, int iv, ref string[] ReturnFrame)
        {
            try
            {
                string restr = ""; iversions = iv; strFrame = strFrame.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToUpper();

                //68 + L(2字节) + L(2字节) + 68 + C + Addr(5字节) + AFN + SEQ + Fn(2字节) + Pn(2字节) + 16 + CS
                if (strFrame == null || strFrame.Length < 20 * 2) return "ERR";

                int Len = 8 + ((Convert.ToInt32(strFrame.Substring(4, 2), 16) * 0x100 + Convert.ToInt32(strFrame.Substring(2, 2), 16)) >> 2);

                if (strFrame.Length < Len * 2) return "ERR";

                //Afn = str.Substring(24, 2);
                BoolUp = Convert.ToInt16(strFrame.Substring(12, 2), 16) / Convert.ToInt16(Math.Pow(2, 7)) == 0 ? false : true;      // 是否为上行帧

                FrameAfnDataNode f = new FrameAfnDataNode(strFrame, BoolUp);
                restr = f.ToString();
                Array tempAry = f.ToArrayValue();
                ReturnFrame = new String[tempAry.Length];
                int i = 0;
                foreach (string s in tempAry)
                {
                    ReturnFrame[i] = s;
                    i++;
                }
                Afn = f.FrameDataNodeAfn.ToString("x2").ToUpper();

                #region 注释代码
                /*switch (Afn)
                {
                    case "00":
                        restr = "【Afn = " + Afn + ",确认/否认命令】";
                        restr += a00(str.Substring(28, str.Length - 32));
                        break;
                    case "01":
                        restr = "【Afn = " + Afn + ",复位命令】";
                        restr += a01(str.Substring(28, str.Length - 32));
                        break;
                    case "02":
                        restr = "【Afn = " + Afn + ",链路接口检测】";
                        restr += a02(str.Substring(28, str.Length - 32));
                        break;
                    case "03":
                        restr = "【Afn = " + Afn + ",中继站命令】";
                        restr += a03(str.Substring(28, str.Length - 32));
                        break;
                    case "04":
                        restr = "【Afn = " + Afn + ",设置参数】";
                        restr += a04(str.Substring(28, str.Length - 32));
                        break;
                    case "05":
                        restr = "【Afn = " + Afn + ",控制命令】";
                        restr += a05(str.Substring(28, str.Length - 32), ref ReturnFrame);
                        break;
                    case "06":
                        restr = "【Afn = " + Afn + ",身份认证及密钥协商】";
                        restr += a06(str.Substring(28, str.Length - 32));
                        break;
                    case "08":
                        restr = "【Afn = " + Afn + ",请求被级联终端主动上报】";
                        restr += a08(str.Substring(28, str.Length - 32));
                        break;
                    case "09":
                        restr = "【Afn = " + Afn + ",请求终端配置】";
                        restr += a09(str.Substring(28, str.Length - 32));
                        break;
                    case "0A":
                        restr = "【Afn = " + Afn + ",查询参数】";
                        restr += a0A(str.Substring(28, str.Length - 32),ref ReturnFrame);
                        break;
                    case "0B":
                        restr = "【Afn = " + Afn + ",请求任务数据】";
                        restr += a0B(str.Substring(28, str.Length - 32));
                        break;
                    case "0C":
                        restr = "【Afn = " + Afn + ",请求1类数据】";
                        restr += a0C(str.Substring(28, str.Length - 32), ref ReturnFrame);
                        break;
                    case "0D":
                        restr = "【Afn = " + Afn + ",确请求2类数据】";
                        restr += a0D(str.Substring(28, str.Length - 32));
                        break;
                    case "0E":
                        restr = "【Afn = " + Afn + ",请求3类数据】";
                        restr += a0E(str.Substring(28, str.Length - 32));
                        break;
                    case "0F":
                        restr = "【Afn = " + Afn + ",文件传输】";
                        restr += a0F(str.Substring(28, str.Length - 32));
                        break;
                    case "10":
                        restr = "【Afn = " + Afn + ",数据转发】";
                        restr += a10(str.Substring(28, str.Length - 32));
                        break;
                }*/
                #endregion

                return restr;
            }
            catch
            {
                return "ERR";
            }
        }



        string StartFlag = "68";
        string EndFlag = "16";

        /// <summary>
        /// 校验数据帧合法性
        /// </summary>
        /// <param name="dataframe">数据帧报文</param>
        /// <param name="DIR">DIR:null-不校验;0-下行报文;1-上行报文</param>
        /// <param name="afn">AFN:null-不校验</param>
        /// <param name="fn">Fn:null-不校验</param>
        /// <param name="pn">Pn:null-不校验</param>
        /// <returns></returns>
        public bool CheckFrame(string dataframe, int? DIR, int? afn, int? fn, int? pn, out string dataframe1, out string dataframe2)
        {
            int first, second;
            int Len;
            string FullFrame = string.Empty;

            dataframe = dataframe.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("-", "").ToUpper();
            dataframe1 = null; dataframe2 = null;

            while (true)
            {
                first = dataframe.IndexOf(StartFlag);
                if (first < 0) return false;

                second = dataframe.IndexOf(StartFlag, (first + StartFlag.Length));
                if (second < 0) return false;

                if ((second - first) == 10)
                {
                    dataframe = dataframe.Substring(first);

                    Len = 8 + ((Convert.ToInt32(dataframe.Substring(4, 2), 16) * 0x100 + Convert.ToInt32(dataframe.Substring(2, 2), 16)) >> 2);
                    if (dataframe.Length < Len * 2) return false;

                    string EndFrameByte = dataframe.Substring(Len * 2 - 2, 2).ToUpper();
                    if (EndFrameByte.Equals(EndFlag) == true)
                    {
                        FullFrame = dataframe.Substring(0, Len * 2);
                        break;
                    }
                    else
                    {
                        dataframe = dataframe.Substring(second);
                    }
                }
                else
                {
                    dataframe = dataframe.Substring(second);
                }
            }

            dataframe1 = FullFrame;
            dataframe2 = dataframe.Remove(0, FullFrame.Length);

            byte[] packetData = new byte[Len];
            for (int index = 0; index < Len; index++)
            {
                packetData[index] = Convert.ToByte(FullFrame.Substring(index * 2, 2), 16);
            }

            byte CS = BuildCheckSUM(packetData.ToArray(), 6, packetData.Length - 2 - 1);
            if (CS != packetData[packetData.Length - 2]) return false;

            int tempDIR = Convert.ToByte(FullFrame.Substring(12, 2), 16) >> 7;
            int tempAFN = Convert.ToByte(FullFrame.Substring(24, 2), 16);
            int tempFn = Comm.ExplanF(FullFrame.Substring(32, 4));
            int tempPn = Comm.ExplanP(FullFrame.Substring(28, 4));

            if (DIR != null)
            {
                if (tempDIR != DIR) return false;
            }

            if (afn != null)
            {
                if (tempAFN != afn) return false;

                if (fn != null)
                {
                    if (tempFn != fn) return false;

                    if (pn != null)
                    {
                        if (tempPn != pn) return false;
                    }
                }
            }

            return true;
        }

        public byte BuildCheckSUM(byte[] aryData, int startIndex, int endIndex)
        {
            int intAry = 0;
            int bytChk = 0;

            for (intAry = startIndex; intAry < endIndex; intAry++)
            {
                bytChk += Convert.ToInt32(aryData[intAry]) % 256;
            }

            return Convert.ToByte(bytChk % 256);
        }
    }
}
