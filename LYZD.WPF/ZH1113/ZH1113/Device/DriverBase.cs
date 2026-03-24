using System;
using System.Collections.Generic;
using System.Text;

using ZH.SocketModule.Packet;
using ZH.SocketModule;
using System.Runtime.InteropServices;
using ZH.Struct;

namespace ZH
{
     class DriverBase
    {
        public DriverBase()
        {
            
        }

        /// <summary>
        /// 根据端口号、IP地址获取唯一端口名称
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="UDPorCOM">true UDP,false COM</param>
        /// <param name="ip">IP isNullOrEmpty COM ,other wise UDP/TCP</param>
        /// <returns></returns>
        public string GetPortNameByPortNumber(int port, bool UDPorCOM,string ip)
        {
            if (false == UDPorCOM)
            {
                return string.Format("COM_{0}", port);
            }
            else
            {
                return string.Format("Port_{0}_{1}", ip, port);
            }
        }

        public void RegisterPort(int port, string strSetting, string IP, int RemotePort, int LocalStartPoet, int maxWaittime, int waitSencondsPerByte)
        {
            System.Net.IPAddress ipa = System.Net.IPAddress.Parse(IP);
            string portName = GetPortNameByPortNumber(port, true, IP);
            //注册数据端口
            SockPool.Instance.AddUdpSock(portName, ipa, RemotePort, port, LocalStartPoet, maxWaittime, waitSencondsPerByte);
            
            SockPool.Instance.UpdatePortSetting(portName, strSetting);
            SockPool.Instance.UpdatePortSetting(portName, strSetting);
        }

        /// <summary>
        /// 注册端口[串口]
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="strSetting">串口设置</param>
        public void RegisterPort(int port, string strSetting, int maxWaittime, int waitSencondsPerByte)
        {
            string portName = GetPortNameByPortNumber(port, false, "");
            //注册设置端口
            SockPool.Instance.AddComSock(portName, port, strSetting, maxWaittime, waitSencondsPerByte);
            SockPool.Instance.UpdatePortSetting(portName, strSetting);
        }

        
        /// <summary>
        /// UDP发送,重新初始化波特率
        /// </summary>
        ///<param name="stPort"></param>
        /// <param name="sendPacket"></param>
        /// <param name="recvPacket"></param>
        /// <returns></returns>
        public bool SendData(StPortInfo stPort, SendPacket sendPacket, RecvPacket recvPacket)
        {
            if (stPort.m_Exist == 0)
            {
                return true;
            }
            return SendData(stPort.m_Port, stPort.m_Port_IsUDPorCom, stPort.m_IP, stPort.m_Port_Setting, sendPacket, recvPacket);
        }
        /// <summary>
        /// true UDP,false COM。重新初始化波特率
        /// </summary>
        /// <param name="port"></param>
        /// <param name="UDPorCOM">true UDP,false COM</param>
        /// <param name="IP">IP</param>
        /// <param name="sendPacket"></param>
        /// <param name="recvPacket"></param>
        /// <returns></returns>
        private bool SendData(int port, bool UDPorCOM, string IP, string strSetting,SendPacket sendPacket, RecvPacket recvPacket)
        {
            string portName = GetPortNameByPortNumber(port, UDPorCOM, IP);
            if (strSetting.Trim() != "")
            {
                SockPool.Instance.UpdatePortSetting(portName, strSetting);
            }
            return SockPool.Instance.Send(portName, sendPacket, recvPacket);
        }
        

    }
}
