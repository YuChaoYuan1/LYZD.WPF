using Gurux.Common;
using LYZD.Utility.Log;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.CtrlCmd
{
    public class CtrlClient
    {
        public static Func<string, string> Start;
        public static Func<string> Stop;

        public static string StationID { get; private set; }

        public static bool Registered { get; private set; }

        public static bool Connected
        {
            get
            {
                if (net == null) return false;
                else return net.IsOpen;
            }
        }

        private static Gurux.Net.GXNet net;

        private static int cmdId = 0;//1-999
        private static int CmdId
        {
            get
            {
                if (cmdId >= 999) cmdId = 0;
                return ++cmdId;
            }
        }

        public static void InitNet(string ip, int port, string stationId)
        {
            try
            {
                StationID = stationId;

                if (net != null && net.IsOpen) net.Close();
                Registered = false;

                net = new Gurux.Net.GXNet()
                {
                    Protocol = Gurux.Net.NetworkType.Tcp,
                    Server = false,
                    HostName = ip,
                    Port = port,
                    WaitTime = 5000
                };
                net.OnError -= Net_OnError;
                net.OnError += Net_OnError;
                net.OnMediaStateChange -= Net_OnMediaStateChange;
                net.OnMediaStateChange += Net_OnMediaStateChange;

                net.OnReceived -= Net_OnReceived;
                net.OnReceived += Net_OnReceived;

            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex);
#endif
            }
        }
        public static void Connect()
        {
            try
            {
                if (net != null)
                {
                    net.Open();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex);
#endif
                Registered = false;
            }
        }

        public static void Send(MsgType msgtype, string msg)
        {
            try
            {
                if (Connected)
                {
                    string formatMsg;
                    {
                        formatMsg = $"{{'Initiator':'{StationID}','Receiver':'Main','Time':'{DateTime.Now:yyyy-MM-dd HH:mm:ss}','CommandIndex':'{CmdId}','CommandName':'{msgtype}','CommandContent':'{msg}'}}";
                    }
                    net.Send(formatMsg, "");
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex);
#endif
                Registered = false;
                Send(MsgType.故障, ex.ToString());
            }
        }

        private static void Net_OnReceived(object sender, ReceiveEventArgs e)
        {
            try
            {
                if (e.Data is byte[] buf)
                {
                    string data = Encoding.UTF8.GetString(buf);
#if DEBUG
                    Console.WriteLine($"Received:{e.SenderInfo},{data}");
#endif

                    JObject jo = JObject.Parse(data);
                    string id = jo["Receiver"]?.ToString();

                    if (StationID == id)
                    {
                        string cmd = jo["CommandName"]?.ToString();
                        string cmddata = jo["CommandContent"]?.ToString();
                        switch (cmd)
                        {
                            case "Connected":
                            case "请求结果":
                                if (!string.IsNullOrWhiteSpace(cmddata) && cmddata.Contains("成功")|| cmddata.Contains("工位已上线"))
                                {
                                    Registered = true;
                                }
                                LogManager.AddMessage($"请求结果...{cmddata}", EnumLogSource.服务器日志, EnumLevel.Information);
                                break;
                            case "Start":
                            case "开始检定":
                                string msg = Start?.Invoke(cmddata);
                                break;
                            case "继续检定":
                                break;
                            case "Stop":
                            case "停止检定":
                                msg = Stop?.Invoke();
                                break;
                        }
                    }


                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex);
#endif
                Send(MsgType.故障, ex.ToString());
            }
        }

        private static void Net_OnMediaStateChange(object sender, MediaStateEventArgs e)
        {
            switch (e.State)
            {
                case MediaState.Closing:
                case MediaState.Closed:
                    Registered = false;
                    break;
                case MediaState.Open:
                    Send(MsgType.心跳包, "");
                    break;
                case MediaState.Opening:
                    break;
                case MediaState.Changed:
                    Registered = false;
                    break;
            }
        }

        private static void Net_OnError(object sender, Exception ex)
        {
#if DEBUG
            Console.WriteLine(ex);
#endif
            Registered = false;
        }
    }
}
