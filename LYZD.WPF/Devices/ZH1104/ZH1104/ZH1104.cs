using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ZH;
using ZH.SocketModule.Packet;
using ZH.Struct;

namespace ZH
{
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
     ComVisible(true)]

    public interface IClass_Interface
    {
        /// <summary>
        /// 初始化设备通讯参数UDP
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="MaxWaitTme">最长等待时间</param>
        /// <param name="WaitSencondsPerByte">帧字节间隔时间</param>
        /// <param name="IP">Ip地址</param>
        /// <param name="RemotePort">远程端口</param>
        /// <param name="LocalStartPort">本地端口</param>
        /// <returns>是否注册成功</returns>
        [DispId(1)]
        int InitSetting(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte, string IP, int RemotePort, int LocalStartPort);
        /// <summary>
        /// 注册Com 口
        /// </summary>
        /// <param name="ComNumber"></param>
        /// <param name="strSetting"></param>
        /// <param name="maxWaittime"></param>
        /// <returns></returns>
        [DispId(2)]
        int InitSettingCom(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte);

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="FrameAry">出参</param>
        /// <returns></returns>
        [DispId(3)]
        int Connect(out string[] FrameAry);
        /// <summary>
        /// 接地故障试验控制继电器
        /// </summary>
        /// <param name="bwIndex"> 控制板ID号</param>
        /// <param name="Ua">A相电压接地 00-断开  01-闭合</param>
        /// <param name="Ub">B相电压接地 00-断开  01-闭合</param>
        /// <param name="Uc">C相电压接地 00-断开  01-闭合</param>
        /// <param name="Un">N相电压接地 00-断开  01-闭合</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        [DispId(4)]
        int SetJDGZContrnl(int bwIndex, int Ua, int Ub, int Uc, int Un, out string[] FrameAry);

        /// <summary>
        /// 载波检定台载波信号的链路切换
        /// </summary>
        /// <param name="bwIndex">控制板ID号</param>
        /// <param name="path1">路径一  00-断开  01-闭合</param>
        /// <param name="path2">路径二  00-断开  01-闭合</param>
        /// <param name="path3">路径三  00-断开  01-闭合</param>
        /// <param name="path4">路径四  00-断开  01-闭合</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        [DispId(5)]
        int SetZBSJContrnl(int bwIndex, int path1, int path2, int path3, int path4, out string[] FrameAry);

        /// <summary>
        /// 读取 交流功耗，   
        /// </summary>
        /// <param name="bwIndex">设备ID号</param>
        /// <param name="pd">功耗值float-15</param>
        /// <returns></returns>
        [DispId(6)]
        int Read_GH_Dissipation(int bwIndex, out float[] pd, out string[] FrameAry);

        /// <summary>
        /// 读取 直流功耗，   
        /// </summary>
        /// <param name="bwIndex">设备ID号</param>
        /// <param name="pd">功耗值float-1</param>
        /// <returns></returns>
        [DispId(7)]
        int Read_ZL_GH_Dissipation(int bwIndex, out float pd, out string[] FrameAry);


        /// <summary>
        /// 零线电流切换寄存器
        /// </summary>
        /// <param name="bwIndex">设备ID号</param>
        /// <param name="iA">Ia电流 00 -断开继电器  01 -闭合继电器</param>
        /// <param name="iN">IN电流 00 -断开继电器  01 -闭合继电器</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        [DispId(8)]
        int SetLXDLContrnl(int bwIndex, int iA, int iN, out string[] FrameAry);


        /// <summary>
        /// 设置载波模块工作电压
        /// </summary>
        /// <param name="bwIndex">设备ID号</param>
        /// <param name="u">设置的电压值</param>
        /// <param name="bwselect">选择要设置的表位</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        [DispId(9)]
        int SetZBGZDYContrnl(int bwIndex, float u, bool[] bwselect, out string[] FrameAry);


        /// <summary>
        /// 脱机指令没有
        /// </summary>
        /// <param name="FrameAry">输出上行报文</param>
        /// <returns></returns>
        [DispId(10)]
        int DisConnect(out string[] FrameAry);


        //add yjt 20230103 新增零线电流控制启停
        /// <summary>
        /// 零线电流控制启停
        /// </summary>
        /// <returns></returns>
        int StartZeroCurrent(int A_kz, int BC_kz);
    }

    [Guid("5BFBD907-D63A-4c0f-B51A-0AD571100176"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
    ComVisible(true)]
    public interface IClass_Events
    {
        [DispId(80)]
        void MsgCallBack(string szMessage);
    }
    //{19F31930-CD26-433E-9E74-E8935CEFD2A2}
    [Guid("19F31930-CD26-433E-9E74-E8935CEFD2A2"),
    ProgId("ZH.ZH1104"),
    ClassInterface(ClassInterfaceType.None),
    ComDefaultInterface(typeof(IClass_Interface)),
    ComSourceInterfaces(typeof(IClass_Events)),
    ComVisible(true)]
    public class ZH1104 : IClass_Interface
    {
        public delegate void MsgCallBackDelegate(string szMessage);
        public event MsgCallBackDelegate MsgCallBack;

        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 1;
        /// <summary>
        /// 误差板控制端口
        /// </summary>
        private StPortInfo m_ErrorZh1104Port = null;

        private DriverBase driverBase = null;

        //是否发送数据标志
        private bool sendFlag = true;
        /// <summary>
        /// 构造方法
        /// </summary>
        public ZH1104()
        {
            m_ErrorZh1104Port = new StPortInfo();
            driverBase = new DriverBase();
        }

        #region IClass_Interface 成员
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
            m_ErrorZh1104Port.m_Exist = 1;
            m_ErrorZh1104Port.m_IP = IP;
            m_ErrorZh1104Port.m_Port = ComNumber;
            m_ErrorZh1104Port.m_Port_IsUDPorCom = true;
            m_ErrorZh1104Port.m_Port_Setting = "2400,n,8,1";
            try
            {
                driverBase.RegisterPort(ComNumber, m_ErrorZh1104Port.m_Port_Setting, m_ErrorZh1104Port.m_IP, RemotePort, LocalStartPort, MaxWaitTime, WaitSencondsPerByte);
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
            m_ErrorZh1104Port.m_Exist = 1;
            m_ErrorZh1104Port.m_IP = "";
            m_ErrorZh1104Port.m_Port = ComNumber;
            m_ErrorZh1104Port.m_Port_IsUDPorCom = false;
            m_ErrorZh1104Port.m_Port_Setting = "38400,n,8,1";
            try
            {
                driverBase.RegisterPort(ComNumber, "38400,n,8,1", MaxWaitTime, WaitSencondsPerByte);
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
        /// <param name="FrameAry">出参</param>
        /// <returns></returns>
        public int Connect(out string[] FrameAry)
        {
            ZH1104_RequestLinkPacket rc2 = new ZH1104_RequestLinkPacket(1);
            ZH1104_RequestLinkReplyPacket recv2 = new ZH1104_RequestLinkReplyPacket();
            FrameAry = new string[1];
            int ReValue = 0;
            try
            {

                FrameAry[0] = BytesToString(rc2.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_ErrorZh1104Port, rc2, recv2))
                    {
                        ReValue = recv2.ReciveResult == RecvResult.OK ? 0 : 2;
                        return ReValue;
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
        /// 接地故障试验控制继电器
        /// </summary>
        /// <param name="bwIndex"> 控制板ID号</param>
        /// <param name="Ua">A相电压接地 00-断开  01-闭合</param>
        /// <param name="Ub">B相电压接地 00-断开  01-闭合</param>
        /// <param name="Uc">C相电压接地 00-断开  01-闭合</param>
        /// <param name="Un">N相电压接地 00-断开  01-闭合</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int SetJDGZContrnl(int bwIndex, int Ua,int Ub,int Uc,int Un, out string[] FrameAry)
        {           
            ZH1104_RequestJDGZPacket rc2 = new ZH1104_RequestJDGZPacket(bwIndex);
            rc2.SetPara(Ua, Ub, Uc, Un);
            ZH1104_RequestJDGZReplyPacket recv2 = new ZH1104_RequestJDGZReplyPacket();
            FrameAry = new string[1];
            int ReValue = 0;
            try
            {
                FrameAry[0] = BytesToString(rc2.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_ErrorZh1104Port, rc2, recv2))
                    {
                        ReValue = recv2.ReciveResult == RecvResult.OK ? 0 : 2;
                        return ReValue;
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
        /// 载波检定台载波信号的链路切换
        /// </summary>
        /// <param name="bwIndex">控制板ID号</param>
        /// <param name="path1">路径一  00-断开  01-闭合</param>
        /// <param name="path2">路径二  00-断开  01-闭合</param>
        /// <param name="path3">路径三  00-断开  01-闭合</param>
        /// <param name="path4">路径四  00-断开  01-闭合</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int SetZBSJContrnl(int bwIndex, int path1, int path2, int path3, int path4, out string[] FrameAry)
        {
            ZH1104_RequestZBSJPacket rc2 = new ZH1104_RequestZBSJPacket(bwIndex);
            rc2.SetPara(path1, path2, path3, path4);
            ZH1104_RequestZBSJReplyPacket recv2 = new ZH1104_RequestZBSJReplyPacket();
            FrameAry = new string[1];
            int ReValue = 0;
            try
            {
                FrameAry[0] = BytesToString(rc2.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_ErrorZh1104Port, rc2, recv2))
                    {
                        ReValue = recv2.ReciveResult == RecvResult.OK ? 0 : 2;
                        return ReValue;
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
        /// 读取 交流功耗，   
        /// </summary>
        /// <param name="bwIndex">设备ID号</param>
        /// <param name="pd">功耗值float-15</param>
        /// <returns></returns>
        public int Read_GH_Dissipation(int bwIndex, out float[] pd,out string[] FrameAry)
        {
            pd = new float[15];
            for (int i = 0; i < pd.Length; i++)
            {
                pd[i] = 99.99F;
            }   
            ZH1104_ReadGHDataRelayPacket rc2 = new ZH1104_ReadGHDataRelayPacket(bwIndex);
            ZH1104_ReadGHDataReplyPacket recv2 = new ZH1104_ReadGHDataReplyPacket();
            FrameAry = new string[1];
            int ReValue = 0;
            try
            {
                FrameAry[0] = BytesToString(rc2.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_ErrorZh1104Port, rc2, recv2))
                    {
                        ReValue = recv2.ReciveResult == RecvResult.OK ? 0 : 2;
                        pd = recv2.Fldata;
                        return ReValue;
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
        /// 读取 直流功耗，   
        /// </summary>
        /// <param name="bwIndex">设备ID号</param>
        /// <param name="pd">功耗值float-1</param>
        /// <returns></returns>
        public int Read_ZL_GH_Dissipation(int bwIndex, out float  pd, out string[] FrameAry)
        {
            pd = 999.99F;

            ZH1104_RequestZLGHPacket rc2 = new ZH1104_RequestZLGHPacket(bwIndex);
            ZH1104_RequestZLGHReplyPacket recv2 = new ZH1104_RequestZLGHReplyPacket();
            FrameAry = new string[1];
            int ReValue = 0;
            try
            {
                FrameAry[0] = BytesToString(rc2.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_ErrorZh1104Port, rc2, recv2))
                    {
                        ReValue = recv2.ReciveResult == RecvResult.OK ? 0 : 2;
                        pd = recv2.Fldata;
                        return ReValue;
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

        //add yjt 20230103 新增零线电流控制启停
        /// <summary>
        /// 零线电流控制启停
        /// </summary>
        /// <param name="Control"></param>
        /// <returns></returns>
        public int StartZeroCurrent(int A_kz, int BC_kz)
        {
            string[] FrameAry = new string[1];
            ZH1104_StartZeroCurrentLinkPacket rlp = new ZH1104_StartZeroCurrentLinkPacket();
            ZH1104_StartZeroCurrentLinkReplyPacket rlrp = new ZH1104_StartZeroCurrentLinkReplyPacket();
            //合成的报文
            try
            {
                //6801FE0A1340000001A7 启动
                //6801FE0A1340000100A7 关闭
                rlp.SetPara(A_kz, BC_kz);
                FrameAry[0] = BytesToString(rlp.GetPacketData());

                m_ErrorZh1104Port.m_Port_Setting = "38400,n,8,1";
                if (SendPacketWithRetry(m_ErrorZh1104Port, rlp, rlrp))
                {
                    bool linkClockOk = rlrp.ReciveResult == RecvResult.OK;
                    return linkClockOk ? 0 : 1;
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
        /// 零线电流切换寄存器
        /// </summary>
        /// <param name="bwIndex">设备ID号</param>
        /// <param name="iA">Ia电流 00 -断开继电器  01 -闭合继电器</param>
        /// <param name="iN">IN电流 00 -断开继电器  01 -闭合继电器</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int SetLXDLContrnl(int bwIndex, int iA,int iN, out string[] FrameAry)
        {
            ZH1104_RequestLXDLPacket rc2 = new ZH1104_RequestLXDLPacket(bwIndex);
            rc2.SetPara(iA,iN);
            ZH1104_RequestLXDLReplyPacket recv2 = new ZH1104_RequestLXDLReplyPacket();
            FrameAry = new string[1];
            int ReValue = 0;
            try
            {
                FrameAry[0] = BytesToString(rc2.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_ErrorZh1104Port, rc2, recv2))
                    {
                        ReValue = recv2.ReciveResult == RecvResult.OK ? 0 : 2;
                        return ReValue;
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
        /// 设置载波模块工作电压
        /// </summary>
        /// <param name="bwIndex">设备ID号</param>
        /// <param name="u">设置的电压值</param>
        /// <param name="bwselect">选择要设置的表位</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int SetZBGZDYContrnl(int bwIndex, float  u, bool[] bwselect, out string[] FrameAry)
        {
            int intU = Convert.ToInt32(1000 * u);
            string strData = "";
            for (int i = bwselect.Length - 1; i >= 0; i--)
            {
                if (bwselect[i])
                {
                    strData += "1";
                }
                else
                {
                    strData += "0";
                }
            }
            strData = strData.PadLeft(64, '1');
            byte[] setData = new byte[8];
            for (int k = 0; k < setData.Length; k++)
            {
                setData[k] = Convert.ToByte(Convert.ToInt32(strData.Substring(8 * k, 8), 2));
            }


            ZH1104_RequestZBGZDYPacket rc2 = new ZH1104_RequestZBGZDYPacket(bwIndex);
            rc2.SetPara(intU, setData);
            ZH1104_RequestZBGZDYReplyPacket recv2 = new ZH1104_RequestZBGZDYReplyPacket();
            FrameAry = new string[1];
            int ReValue = 0;
            try
            {
                FrameAry[0] = BytesToString(rc2.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_ErrorZh1104Port, rc2, recv2))
                    {
                        ReValue = recv2.ReciveResult == RecvResult.OK ? 0 : 2;
                        return ReValue;
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
        /// 脱机指令没有
        /// </summary>
        /// <param name="FrameAry">输出上行报文</param>
        /// <returns></returns>
        public int DisConnect(out string[] FrameAry)
        {
            FrameAry = new string[1];
            return 0;
        }
        #endregion

        /// <summary>
        /// 设置发送标志位
        /// </summary>
        /// <param name="Flag"></param>
        /// <returns></returns>
        public int SetSendFlag(bool Flag)
        {
            sendFlag = Flag;
            return 0;
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
