using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LY.VirtualMeter.Core.CAN
{
    public struct VCI_BOARD_INFO
    {
        public UInt16 hw_Version;
        public UInt16 fw_Version;
        public UInt16 dr_Version;
        public UInt16 in_Version;
        public UInt16 irq_Num;
        public byte can_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] str_Serial_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] str_hw_Type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Reserved;
    }


    //2.定义CAN信息帧的数据类型。
    unsafe public struct VCI_CAN_OBJ  //使用不安全代码
    {
        public uint ID;
        public uint TimeStamp;
        public byte TimeFlag;
        public byte SendType;
        public byte RemoteFlag;//是否是远程帧
        public byte ExternFlag;//是否是扩展帧
        public byte DataLen;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Reserved;

    }

    //3.定义CAN控制器状态的数据类型。
    public struct VCI_CAN_STATUS
    {
        public byte ErrInterrupt;
        public byte regMode;
        public byte regStatus;
        public byte regALCapture;
        public byte regECCapture;
        public byte regEWLimit;
        public byte regRECounter;
        public byte regTECounter;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved;
    }

    //4.定义错误信息的数据类型。
    public struct VCI_ERR_INFO
    {
        public UInt32 ErrCode;
        public byte Passive_ErrData1;
        public byte Passive_ErrData2;
        public byte Passive_ErrData3;
        public byte ArLost_ErrData;
    }

    //5.定义初始化CAN的数据类型
    public struct VCI_INIT_CONFIG
    {
        public UInt32 AccCode;
        public UInt32 AccMask;
        public UInt32 Reserved;
        public byte Filter;
        public byte Timing0;
        public byte Timing1;
        public byte Mode;
    }


    public abstract class PortBase
    {

        /// <summary>
        /// 发送数据委托
        /// </summary>
        /// <param name="RevBuff">缓存数据</param>
        public delegate void DteRevBuffData(Dictionary<uint, List<byte>> buff);
        /// <summary>
        /// 发送接收数据事件
        /// </summary>
        public event DteRevBuffData SendBuffDataEvent;

        //public Dictionary<uint, List<byte>> RevcieBuffs = new Dictionary<uint, List<byte>>();

        private object obj = new object();
        /// <summary>
        /// 发送数据方法
        /// </summary>
        /// <param name="BuffData">缓存数据</param>
        public void AddBuffData(Dictionary<uint, List<byte>> BuffData)
        {
            //if (SendBuffDataEvent != null)
            //{
            //    SendBuffDataEvent(BuffData);
            //}
            //string strKey = "RecvType" + DeviceType.ToString()+ "|Index"+DeviceIndex.ToString()
            //       + "|Cannel" + CanId.ToString();
            lock (obj)
            {
                //RevcieBuffs.Clear();
                foreach (uint item in BuffData.Keys)
                {
                    if (App.RevcieBuffs.ContainsKey(item))
                    {
                        App.RevcieBuffs[item].AddRange(BuffData[item]);
                        //App.RevcieBuffs.Remove(item);
                    }
                    else
                        App.RevcieBuffs.Add(item, BuffData[item]);
                }
            }
        }
        /// <summary>
        /// 发送错误日志
        /// </summary>
        /// LogMessage第二个参数为通讯报文，系统消息，错误消息
        /// <param name="strErrMsg"></param>
        public void SendErrMsg(Exception strErrMsg)
        {
            //Log.LogException(strErrMsg, "异常消息");
        }
        public void LogMsg(string strLog)
        {
            //Log.LogMessage(strLog, "通讯消息");
        }
        //ID="1" DeviceIndex="0" CANId="0" CANName="CAN1" CANParams="500kbps" ClassName="CANSend"

        /// <summary>
        /// 设备类型
        /// </summary>
        public uint DeviceType
        {
            get;
            set;
        }

        /// <summary>
        /// 设备索引
        /// </summary>
        public uint DeviceIndex
        {
            get;
            set;
        }
        /// <summary>
        /// 设备通道号
        /// </summary>
        public uint CanId
        {
            get;
            set;
        }
        /// <summary>
        /// 通道名称
        /// </summary>
        public string CanName
        {
            get;
            set;
        }
        /// <summary>
        /// 通道通讯波特率
        /// </summary>
        public string CanParams
        {
            get;
            set;
        }
        /// <summary>
        /// 端口所属设备类名
        /// </summary>
        public string EquiqMentClassName
        {
            get;
            set;
        }

        /// <summary>
        /// 发送数据方法
        /// </summary>
        /// <param name="SendBuff"></param>
        public abstract void SendData(byte[] SendBuff, UInt32 ID);
        /// <summary>
        /// 打开端口方法
        /// </summary>
        /// <returns></returns>
        public abstract bool Open();
        /// <summary>
        /// 关闭端口方法
        /// </summary>
        /// <returns></returns>
        public abstract bool Close();

    }
}
