using LYZD.DAL.Config;
using LYZD.Utility.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace LYZD.ViewModel.InnerCommand
{
    public class VerifyControl
    {
        public static void InitControl()
        {
            CtrlCmd.CtrlClient.Start = Start;
            CtrlCmd.CtrlClient.Stop = Stop;
        }

        private static string Start(string data)
        {
            string msg = "";
            try
            {
                if (EquipmentData.Controller.IsChecking )
                {
                    msg = "已经在检定中!";
                    SendMsg(msg);
                    return msg;
                }


                if (!string.IsNullOrWhiteSpace(data))
                {
                    SendMsg(data);
                }

                EquipmentData.MeterGroupInfo.NewMeters2();

                Task.Delay(100).Wait();
                ThreadPool.QueueUserWorkItem(delegate
                {
                    SynchronizationContext.SetSynchronizationContext(new
                        DispatcherSynchronizationContext(System.Windows.Application.Current.Dispatcher));
                    SynchronizationContext.Current.Post(pl =>
                    {
                        if (EquipmentData.MeterGroupInfo.Frame_DownMeterInfoFromMis())
                        {
                            Task.Delay(100).Wait();
                            if (!EquipmentData.MeterGroupInfo.Frame_DownSchemeMis())
                            {
                                msg = "下载方案失败！";
                                SendMsg(CtrlCmd.MsgType.故障, msg);
                            }

                            if (!EquipmentData.MeterGroupInfo.UpdateMeterInfoAuto(out string err))
                            {
                                msg = $"更新电表信息失败！{err}";
                                SendMsg(CtrlCmd.MsgType.故障, msg);
                            }
                            Task.Delay(100).Wait();
                            //开始执行检定方案
                            EquipmentData.Controller.Index = 0; //设置从第一项开始检定
                            EquipmentData.Controller.RunningVerify();
                        }
                        else
                        {
                            msg = "下载电表信息失败！";
                            SendMsg(CtrlCmd.MsgType.故障, msg);
                        }

                    }, null);
                });
                
                return msg;
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                SendMsg(CtrlCmd.MsgType.故障, msg);
            }
            return msg;
        }

        private static string Stop()
        {
            string msg = "";
            try
            {
                EquipmentData.Controller.AutoStop = true;
                EquipmentData.Controller.StopVerify();
                return msg;
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                SendMsg(CtrlCmd.MsgType.故障, msg);
            }
            return msg;
        }

        public static void SendMsg(string msg)
        {
            LogManager.AddMessage($"{CtrlCmd.MsgType.运行消息},{msg}", EnumLogSource.服务器日志, EnumLevel.Information);
            CtrlCmd.CtrlClient.Send(CtrlCmd.MsgType.运行消息, msg);
        }

        public static void SendMsg(CtrlCmd.MsgType msgType, string msg)
        {
            if (msgType == CtrlCmd.MsgType.故障)
                LogManager.AddMessage($"{msgType},{msg}", EnumLogSource.服务器日志, EnumLevel.Error);
            else
                LogManager.AddMessage($"{msgType},{msg}", EnumLogSource.服务器日志, EnumLevel.Information);

            CtrlCmd.CtrlClient.Send(msgType, msg);
        }
    }
}
