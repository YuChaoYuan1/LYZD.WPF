using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ZH.SocketModule.Packet;
using ZH.Struct;
using ZH485.Device;
using ZH485.Enum;

namespace ZH485
{
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
ComVisible(true)]
    public interface IClass_Interface
    {
        /// <summary>
        /// 初始化设备通讯参数
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="Params">初始化参数 "2400,n,8,1" </param>
        /// <param name="MaxWaitTme">最长等待时间</param>
        /// <param name="WaitSencondsPerByte"></param>
        /// <param name="IP">2018IP地址</param>
        /// <param name="RemotePort">远程端口</param>
        /// <param name="LocalStartPort">本地端口</param>
        /// <returns>是否注册成功</returns>
        [DispId(1)]
        int InitSetting(int ComNumber, string Params, int MaxWaitTime, int WaitSencondsPerByte, string IP, int RemotePort, int LocalStartPort, string HaveProtocol,StCanParams sc);
        /// <summary>
        /// 注册Com 口
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="Params">初始化端口参数 "2400,n,8,1"</param>
        /// <param name="MaxWaitTime">最大等待时间</param>
        /// <param name="WaitSencondsPerByte">字节间延时时间</param>
        /// <returns></returns>
        [DispId(2)]
        int InitSettingCom(int ComNumber, string Params, int MaxWaitTime, int WaitSencondsPerByte);
        /// <summary>
        /// 连机
        /// </summary>
        /// <returns></returns>
        [DispId(3)]
        int Connect(out string[] FrameAry);
        /// <summary>
        /// 断开连机
        /// </summary>
        /// <returns></returns>
        [DispId(4)]
        int DisConnect(out string[] FrameAry);
        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="SendData">接收报文</param>
        /// <param name="ReciveData">接收报文</param>
        /// <returns></returns>
        [DispId(5)]
        int SendData(byte[] SendData, out byte[] ReciveData);
        ///// <summary>
        ///// 设置标志位
        ///// </summary>
        ///// <param name="Flag">设置标志位</param>
        ///// <returns></returns>
        //[DispId(6)]
        //int SetSendFlag(bool Flag);
        ///// <summary>
        ///// 解析下行报文
        ///// </summary>
        ///// <param name="MothedName">方法名称</param>
        ///// <param name="ReFrameAry">下行报文</param>
        ///// <param name="ReAry">解析下行报文的数据</param>
        ///// <returns></returns>
        //[DispId(7)]
        //int UnPacket(string MothedName, byte[] ReFrameAry, out string[] ReAry);
    }
    [Guid("131E61EC-B6E4-431D-95A5-3B7F2A6B18DA"),
    ProgId("CLOU.M_Rs485"),
    ClassInterface(ClassInterfaceType.None),
    ComDefaultInterface(typeof(IClass_Interface)),
    ComVisible(true)]
    public class RS485 : IClass_Interface
    {
        /// <summary>
        /// 485端口
        /// </summary>
        private StPortInfo[] m_Rs485Port = null;
        /// <summary>
        /// 通讯基类
        /// </summary>
        private DriverBase driverBase = null;
        //发送标志
        private bool sendFlag = true;

        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 1;

        public RS485()
        {
            m_Rs485Port = new StPortInfo[1];
            driverBase = new DriverBase();
        }

        /// <summary>
        /// 初始化2018 端口
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="MaxWaitTime">最大等待时间</param>
        /// <param name="WaitSencondsPerByte">字节间等待时间</param>
        /// <param name="IP">Ip地址</param>
        /// <param name="RemotePort">远程端口号</param>
        /// <param name="LocalStartPort">本地端口号</param>
        /// <returns></returns>
        public int InitSetting(int ComNumber, string Params, int MaxWaitTime, int WaitSencondsPerByte, string IP, int RemotePort, int LocalStartPort, string HaveProtocol,StCanParams sc)
        {
            m_Rs485Port[0] = new StPortInfo();
            m_Rs485Port[0].m_Exist = 1;
            m_Rs485Port[0].m_IP = IP;
            m_Rs485Port[0].m_Port = ComNumber;
            m_Rs485Port[0].m_DeviceIndex = sc.DeviceIndex;
            m_Rs485Port[0].m_CanId = sc.CanId;
            if (HaveProtocol == "Can_PCI-9840I")
                m_Rs485Port[0].m_Port_Type = Cus_EmComType.Can;
            else
                m_Rs485Port[0].m_Port_Type = Cus_EmComType.UDP;
            m_Rs485Port[0].m_Port_Setting = Params;
            //sc.m_EmDeviceType = E_CLSocketModule.Enum.Cus_EmDeviceType.cl3761;
            driverBase.RegisterPort(ComNumber, m_Rs485Port[0].m_Port_Setting, m_Rs485Port[0].m_IP, RemotePort, LocalStartPort, HaveProtocol, MaxWaitTime, WaitSencondsPerByte, sc);
            return 0;
        }
        /// <summary>
        /// 初始化COM口
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="MaxWaitTime">最大等待时间</param>
        /// <param name="WaitSencondsPerByte">字节间延时时间</param>
        /// <returns></returns>
        public int InitSettingCom(int ComNumber, string Params, int MaxWaitTime, int WaitSencondsPerByte)
        {
            m_Rs485Port[0] = new StPortInfo();
            m_Rs485Port[0].m_Exist = 1;
            m_Rs485Port[0].m_IP = "";
            m_Rs485Port[0].m_Port = ComNumber;
            m_Rs485Port[0].m_Port_Type = Cus_EmComType.COM;
            m_Rs485Port[0].m_Port_Setting = Params;
            driverBase.RegisterPort(ComNumber, Params, MaxWaitTime, WaitSencondsPerByte);
            return 0;
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="FrameAry">输出上行报文</param>
        /// <returns></returns>
        public int Connect(out string[] FrameAry)
        {
            FrameAry = new string[1];
            return 0;
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="FrameAry">输出上行报文</param>
        /// <returns></returns>
        public int DisConnect(out string[] FrameAry)
        {
            FrameAry = new string[1];
            return 0;
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="SendData">发送合成的报文</param>
        /// <param name="ReciveData">设备数据报文</param>
        /// <returns></returns>
        public int SendData(byte[] SendData, out byte[] ReciveData)
        {
            ReciveData = null;
            try
            {
                MeterProtocolRecvPacket recvPacket = new MeterProtocolRecvPacket();
                MeterProtocolSendPacket sendPacket = new MeterProtocolSendPacket()
                {
                    SendData = SendData,
                    IsNeedReturn = true
                };
                if (SendPacketWithRetry(m_Rs485Port[0], sendPacket, recvPacket))
                {
                    ReciveData = recvPacket.RecvData;
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception)
            {

                return -1;
            }

        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="SendData">发送合成的报文</param>
        /// <param name="ReciveData">设备数据报文</param>
        /// <returns></returns>
        public int SendData(byte[] SendData, out byte[] ReciveData, int MaxWaitSeconds)
        {
            ReciveData = null;
            try
            {
                MeterProtocolRecvPacket recvPacket = new MeterProtocolRecvPacket();
                MeterProtocolSendPacket sendPacket = new MeterProtocolSendPacket()
                {
                    SendData = SendData,
                    IsNeedReturn = true
                };
                if (SendPacketWithRetry(m_Rs485Port[0], sendPacket, recvPacket, MaxWaitSeconds))
                {
                    ReciveData = recvPacket.RecvData;
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception)
            {

                return -1;
            }

        }



        /// <summary>
        /// 发送数据到相应的端口
        /// </summary>
        /// <param name="stPort"></param>
        /// <param name="sp"></param>
        /// <param name="rp"></param>
        /// <returns></returns>
        private bool SendPacketWithRetry(StPortInfo stPort, SendPacket sp, RecvPacket rp)
        {
            for (int i = 0; i < RETRYTIEMS; i++)
            {
                if (driverBase.SendData(stPort, sp, rp) == true)
                {
                    return true;
                }
                System.Threading.Thread.Sleep(100);
            }
            return false;
        }

        /// <summary>
        /// 发送数据到相应的端口
        /// </summary>
        /// <param name="stPort"></param>
        /// <param name="sp"></param>
        /// <param name="rp"></param>
        /// <returns></returns>
        private bool SendPacketWithRetry(StPortInfo stPort, SendPacket sp, RecvPacket rp, int MaxWaitSeconds)
        {
            for (int i = 0; i < RETRYTIEMS; i++)
            {
                if (driverBase.SendData(stPort, sp, rp, MaxWaitSeconds) == true)
                {
                    return true;
                }
                System.Threading.Thread.Sleep(300);
            }
            return false;
        }

        /// <summary>
        /// 字节数组转字符串
        /// </summary>
        /// <param name="bytesData"></param>
        /// <returns></returns>
        private string BytesToString(byte[] bytesData)
        {
            string strRevalue = string.Empty;
            if (bytesData == null || bytesData.Length < 1)
                return strRevalue;

            strRevalue = BitConverter.ToString(bytesData).Replace("-", "");

            return strRevalue;
        }




    }
}
