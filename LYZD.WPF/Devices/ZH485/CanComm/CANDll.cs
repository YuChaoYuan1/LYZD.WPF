using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ZH485.CanComm
{
    class CANDll
    {

        /// <summary>
        /// 打开设备
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引号</param>
        /// <param name="Reserved">保留参数本地端口号 默认为0</param>
        /// <returns></returns>
        [DllImport(@"CanDll\controlcan.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引号</param>
        /// <returns></returns>
        [DllImport(@"CanDll\controlcan.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        /// <summary>
        /// 初始化CAN 通道
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引号</param>
        /// <param name="CANInd">第几路CAN通道</param>
        /// <param name="pInitConfig">初始化参数</param>
        /// <returns></returns>
        [DllImport(@"CanDll\controlcan.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引号</param>
        /// <param name="pInfo">板卡信息</param>
        /// <returns></returns>
        [DllImport(@"CanDll\controlcan.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 VCI_ReadBoardInfo(UInt32 DeviceType, UInt32 DeviceInd, ref VCI_BOARD_INFO pInfo);
        /// <summary>
        /// 读取CAN卡发生的最近一次错误信息
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引号</param>
        /// <param name="CANInd">CAN索引号</param>
        /// <param name="pErrInfo">错误信息</param>
        /// <returns></returns>
        [DllImport(@"CanDll\controlcan.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 VCI_ReadErrInfo(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_ERR_INFO pErrInfo);
        /// <summary>
        /// 获取CAN状态
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引号</param>
        /// <param name="CANInd">CAN通道索引号</param>
        /// <param name="pCANStatus">Can状态</param>
        /// <returns></returns>
        [DllImport(@"CanDll\controlcan.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 VCI_ReadCANStatus(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_STATUS pCANStatus);
        /// <summary>
        /// 获取设备相应的参数（主要是CANET的相关参数）
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引号</param>
        /// <param name="CANInd">Can通道号</param>
        /// <param name="RefType">参数类型</param>
        /// <param name="pData">存储参数有关数据缓冲区</param>
        /// <returns></returns>
        [DllImport(@"CanDll\controlcan.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 VCI_GetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        /// <summary>
        /// 设置相应设备的参数
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引号</param>
        /// <param name="CANInd">Can通道号</param>
        /// <param name="RefType">参数类型</param>
        /// <param name="pData">存储参数有关数据</param>
        /// <returns></returns>
        [DllImport(@"CanDll\controlcan.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        /// <summary>
        /// 获取制定Can通道的接收缓冲区中，接收到但尚未被读取的帧数量
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引号</param>
        /// <param name="CANInd">Can通道号</param>
        /// <returns></returns>
        [DllImport(@"CanDll\controlcan.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        /// <summary>
        /// 清除接Can通道收缓冲区
        /// </summary>
        /// <param name="DeviceType">设备型号</param>
        /// <param name="DeviceInd">设备索引号</param>
        /// <param name="CANInd">Can通道号</param>
        /// <returns>1=成功   0=失败</returns>
        [DllImport(@"CanDll\controlcan.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        /// <summary>
        /// 启动Can卡的一个Can通道，有多个Can通道，需要多次调用该方法
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引号</param>
        /// <param name="CANInd">Can通道号</param>
        /// <returns>1=成功 0=失败</returns>
        [DllImport(@"CanDll\controlcan.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        /// <summary>
        /// 复位Can通道  无需初始化 即可恢复 CAN卡的正常状态 比如Can卡进入总线关闭状态时，可以调用这个函数
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引号</param>
        /// <param name="CANInd">Can通道号</param>
        /// <returns>1=成功  0=失败</returns>
        [DllImport(@"CanDll\controlcan.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        /// <summary>
        /// 发送、返回值为实际发送的帧数
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引号</param>
        /// <param name="CANInd">Can通道号</param>
        /// <param name="pSend">要发送的帧结构体</param>
        /// <param name="Len">要发送帧数量</param>
        /// <returns>返回实际发送成功的帧数</returns>
        [DllImport(@"CanDll\controlcan.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);

        //[DllImport("controlcan.dll")]
        //static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);
        /// <summary>
        /// 接收 从指定的Can 通道的接收缓冲区中读取数据
        /// </summary>
        /// <param name="DeviceType">设备类型</param>
        /// <param name="DeviceInd">设备索引号</param>
        /// <param name="CANInd">Can通道号</param>
        /// <param name="pReceive">用来接收的帧结构体</param>
        /// <param name="Len">用来接收帧结构体数组的长度</param>
        /// <param name="WaitTime">等待时间</param>
        /// <returns>返回实际读取到的帧数 返回值为0xFFFFFFFF,则表示读取数据失败，有错误发生，可调用 VCI_ReadErrInfo</returns>
        [DllImport(@"CanDll\controlcan.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, IntPtr pReceive, UInt32 Len, Int32 WaitTime);
    }
}
