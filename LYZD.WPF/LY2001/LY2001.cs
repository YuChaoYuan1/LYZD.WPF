using LY.Device;
using LY.SocketModule.Packet;
using LY.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ZH
{
    public class LY2001
    {
        #region 通用属性方法

        /// <summary>
        /// 控制端口
        /// </summary>
        private StPortInfo m_SourcePort = null;

        private DriverBase driverBase = null;
        //是否发送数据标志
        private bool sendFlag = true;

        bool isInitSetting = false;
        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 1;
        public LY2001()
        {
            m_SourcePort = new StPortInfo();
            driverBase = new DriverBase();
        }
        /// <summary>
        /// 初始化2018端口
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="MaxWaitTime">帧命令最长等待时间</param>
        /// <param name="WaitSencondsPerByte">帧字节等待时间</param>
        /// <param name="IP">Ip地址</param>
        /// <param name="RemotePort">远程端口</param>
        /// <param name="LocalStartPort">本地端口</param>
        /// <returns></returns>
        public int InitSetting(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte, string IP, int RemotePort, int LocalStartPort)
        {
            m_SourcePort.m_Exist = 1;
            m_SourcePort.m_IP = IP;
            m_SourcePort.m_Port = ComNumber;
            m_SourcePort.m_Port_IsUDPorCom = true;
            m_SourcePort.m_Port_Setting = "38400,n,8,1";
            try
            {
                driverBase.RegisterPort(ComNumber, m_SourcePort.m_Port_Setting, m_SourcePort.m_IP, RemotePort, LocalStartPort, MaxWaitTime, WaitSencondsPerByte);
                isInitSetting = true;
            }
            catch (Exception)
            {
                return 1;
            }

            return 0;
        }
        /// <summary>
        /// 初始化Com 口
        /// </summary>
        /// <param name="ComNumber"></param>
        /// <param name="MaxWaitTime"></param>
        /// <param name="WaitSencondsPerByte"></param>
        /// <returns></returns>
        public int InitSettingCom(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte)
        {
            m_SourcePort.m_Exist = 1;
            m_SourcePort.m_IP = "";
            m_SourcePort.m_Port = ComNumber;
            m_SourcePort.m_Port_IsUDPorCom = false;
            m_SourcePort.m_Port_Setting = "38400,n,8,1";
            try
            {
                driverBase.RegisterPort(ComNumber, "38400,n,8,1", MaxWaitTime, WaitSencondsPerByte);
                isInitSetting = true;
            }
            catch (Exception)
            {

                return 1;
            }

            return 0;
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="meterN0">表位号</param>
        /// <param name="FrameAry">出参</param>
        /// <returns></returns>
        public int Connect(out string[] FrameAry)
        {
            FrameAry = new string[1];
            if (!isInitSetting)  //没有初始化，或初始话失败了，就返回失败
            {
                return 1;
            }
            return 0;

        }
        /// <summary>
        /// 获取1-n的版本
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="ver"></param>
        /// <param name="frms"></param>
        /// <returns></returns>
        public int GetVersion(int pos, out string ver, out string[] frms)
        {
            ver = "";
            frms = new string[1];

            try
            {
                ZH1104_RequestVersionPacket rc2 = new ZH1104_RequestVersionPacket(pos);
                ZH1104_RequestVersionReplyPacket recv2 = new ZH1104_RequestVersionReplyPacket();
                frms[0] = BitConverter.ToString(rc2.GetPacketData()).Replace("-", "");
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_SourcePort, rc2, recv2))
                    {
                        ver = recv2.Version;
                        return recv2.ReciveResult == RecvResult.OK ? 0 : 2; ;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="stPort">端口号</param>
        /// <param name="sp">发送包</param>
        /// <param name="rp">接收包</param>
        /// <returns></returns>
        private bool SendPacketWithRetry(StPortInfo stPort, SendPacket sp, RecvPacket rp)
        {
            for (int i = 0; i < RETRYTIEMS; i++)
            {
                if (driverBase.SendData(stPort, sp, rp) == true)
                {
                    return true;
                }
                Thread.Sleep(100);
            }
            return false;
        }
        #endregion



        /// <summary>
        /// 温度台控制温控板及电压
        /// </summary>
        /// <param name="meterType">温控板类型-1单相-2三相直接-3三相互感</param>
        /// <param name="ub">电压1=57.7--2=100--3=200</param>
        /// <returns></returns>
        public int TemperaturePowerOn(int meterType, float ub)
        {
            try
            {
                long value = 0;
                byte[] data = new byte[8];
                byte meter = 0;
                byte u = 0;
                switch (meterType)
                {
                    case 1:
                        meter = 4;
                        break;
                    case 2:
                        meter = 5;
                        break;
                    case 3:
                        meter = 6;
                        break;
                    default:
                        break;
                }
                //00000000 00000000
                //switch (ub)
                //{
                //    case 1:
                //        u =15;
                //        break;
                //    case 2:
                //        u =16;
                //        break;
                //    case 3:
                //        u =17;
                //        break;
                //    default:
                //        break;
                //}
                if (0 < ub && ub < 60) u = 15;
                else if (60 < ub && ub < 120) u = 16;
                else if (120 < ub && ub < 240) u = 17;



                if (meter != 0 && u != 0)
                {
                    data = BitConverter.GetBytes(value | (1 << meter) | (1 << u));

                }

                Array.Reverse(data);
                SendContrnlTypePacket send = new SendContrnlTypePacket();
                RecvContrnlTypeReplyPacket reslut = new RecvContrnlTypeReplyPacket();
                send.SetPara("6000", data);
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_SourcePort, send, reslut))
                    {
                        return reslut.ReciveResult == RecvResult.OK ? 0 : 1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// 设置三色灯
        /// </summary>
        /// <param name="colorNum">0代表黄色，1代表绿色，2代表红色</param>
        /// <returns></returns>
        public bool SetTricolorLamp(int colorNum)
        {
            try
            {
                return false;//硬件方案冲突
                long LightColor = 1;
                SendContrnlTypePacket send = new SendContrnlTypePacket();
                RecvContrnlTypeReplyPacket reslut = new RecvContrnlTypeReplyPacket();
                //byte[] data = new byte[8];//总共8个字节64位--目前1-34可用
                //LightColor = LightColor << (15 + colorNum);
                //data = BitConverter.GetBytes(LightColor);
                //Array.Reverse(data);
                //send.SetPara("6000", data);
                byte[] data = new byte[2] { (byte)(LightColor + colorNum), 1 };
                send.SetPara("6001", data);
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_SourcePort, send, reslut))
                    {
                        return reslut.ReciveResult == RecvResult.OK;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 独立控制继电器
        /// </summary>
        /// <param name="number">通道号</param>
        /// <param name="open">0关闭，1输出</param>
        /// <returns></returns>
        public int MultiControl(byte number, byte open)
        {
            try
            {
                byte[] data = new byte[2] { number, open };//1-34

                SendContrnlTypePacket send = new SendContrnlTypePacket();
                RecvContrnlTypeReplyPacket reslut = new RecvContrnlTypeReplyPacket();
                send.SetPara("6001", data);
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_SourcePort, send, reslut))
                    {
                        return reslut.ReciveResult == RecvResult.OK ? 0 : 1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

    }
}
