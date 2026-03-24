using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.SocketEven
{
    internal class Connection
    {
        private object objSendLock = new object();
        private object objPackLock = new object();

        /// <summary>
        /// 连接对象
        /// </summary>
        IConnection connection = null;

        public byte[] RetBuff;

        public void GetBuff()
        {
            RetBuff = connection.RetBuff;
        }

        /// <summary>
        /// 初始化为COM连接，并打开连接
        /// </summary>
        /// <param name="commPort">COM端口</param>
        /// <param name="szBtl">波特率字符串，如：1200,e,8,1</param>
        /// <param name="WaitSecondsPerByte">单字节最大等等时间</param>
        /// <param name="MaxWaitMSecond">指示最大等待时间</param>
        public Connection(int commPort, string szBtl, int MaxWaitMSecond, int WaitSecondsPerByte)
        {
            
            connection = new COM32(szBtl, commPort);
            connection.MaxWaitSeconds = MaxWaitMSecond;
            connection.WaitSecondsPerByte = WaitSecondsPerByte;
            
            //connection = new COM32(szBtl, commPort);
            ////connection.MaxWaitSeconds = MaxWaitMSecond;
            ////connection.WaitSecondsPerByte = WaitSecondsPerByte;
            //connection.MaxWaitSeconds = MaxWaitMSecond ;
            //connection.WaitSecondsPerByte = WaitSecondsPerByte;

        }

        public bool Open()
        {
            if (connection == null) return true;
            return connection.Open();
        }

        private void COM32_LTest(string GetSetMsg, DevicesFunc.SP_PortType sP_Port)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 更新端口对应的COMM口波特率参数
        /// </summary>
        /// <param name="szBlt">要更新的波特率</param>
        /// <returns>更新是否成功</returns>
        public bool UpdatePortSetting(string szBlt)
        {
            if (connection != null) connection.UpdateBltSetting(szBlt);
            return true;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="vData"></param>
        /// <param name="isNeedReturn"></param>
        /// <param name="WaiteTime"></param>
        /// <returns></returns>
        public bool Send(byte[] vData, bool isNeedReturn, int WaiteTime)
        {
            if (connection == null) return false;
            lock (objSendLock)
            {
                if (connection != null)
                {
                    if (!connection.SendData(vData, isNeedReturn, WaiteTime))
                        return false;
                }
                if (isNeedReturn && vData.Length < 1)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="vData"></param>
        /// <param name="isNeedReturn"></param>
        /// <param name="WaiteTime"></param>
        /// <returns></returns>
        public bool Send(ref byte[] vData, bool isNeedReturn, int WaiteTime,out byte[] Rdata)
        {
            Rdata = null;
            if (connection == null) return false;
            lock (objSendLock)
            {
                if (connection != null)
                {
                    if (!connection.SendData(ref vData, isNeedReturn, WaiteTime,out Rdata))
                        return false;
                }
                if (isNeedReturn && vData.Length < 1)
                {
                    return false;
                }
                return true;
            }
        }

        public bool Close()
        {
            if (connection == null) return true;
            return connection.Close();
        }

    }
}
