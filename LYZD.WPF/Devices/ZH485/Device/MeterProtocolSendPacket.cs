using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZH.SocketModule.Packet;

namespace ZH485.Device
{
    #region 电表
    /// <summary>
    /// 电能表数据发送包基类
    /// </summary>
    public class MeterProtocolSendPacket : SendPacket
    {
        public byte[] SendData { get; set; }

        public override int WaiteTime()
        {
            return 300;
        }

        public override byte[] GetPacketData()
        {
            return SendData;
        }
        ///// <summary>
        ///// 打包645成载波，TODO2:这里固定成了07
        ///// </summary>
        //public void PacketTo3762(out byte[] out_645Frame)
        //{
        //    int StartIndex = 0;
        //    int put_byt_Len = SendData.Length;
        //    for (int i = 0; i < put_byt_Len; i++)
        //    {
        //        if (SendData[i] == 0x68)
        //        {
        //            StartIndex = i;
        //            break;
        //        }
        //    }
        //    byte[] put_byt = new byte[put_byt_Len - StartIndex];
        //    Array.Copy(SendData, StartIndex, put_byt, 0, put_byt.Length);
        //    CLDC_DeviceDriver.Driver.PacketTo3762Frame(put_byt, 0x02, out out_645Frame);

        //}
    }


    /// <summary>
    /// 电能表多功能通讯数据包接收类
    /// </summary>
    public class MeterProtocolRecvPacket : RecvPacket
    {
        public byte[] RecvData { get; set; }
        public override bool ParsePacket(byte[] buf)
        {
            RecvData = buf;
            return true;
        }
    }

    #endregion
}
