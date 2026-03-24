using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ZH485.Enum;

namespace ZH485.CanComm
{
    public class CANCannel : PortBase
    {
        private object _Lock = new object();
        private object reciveLock = new object();
        public bool IsOpen = false;
        //private CAN _can;
        private int Frame_Num = 1;
        private byte Timer0 = 0;
        private byte Timer1 = 0;

        private static Dictionary<string, string> PciCanCard = new Dictionary<string, string>();



        public CANCannel(StCanParams sc)
        {

            DeviceType = sc.devType;//设备类型
            DeviceIndex = uint.Parse(sc.DeviceIndex);//设备索引号（卡号）
            CanId = uint.Parse(sc.CanId);//Can 通道号
            CanParams = sc.CanParams;//CAN通讯参数


        }
        public void GetBoundRate(string CanParam)
        {
            switch (CanParam)
            {
                case "20Kbps": Timer0 = 0x18; Timer1 = 0x1C; return;
                case "40Kbps": Timer0 = 0x87; Timer1 = 0xFF; return;
                case "50Kbps": Timer0 = 0x09; Timer1 = 0x1C; return;
                case "80Kbps": Timer0 = 0x83; Timer1 = 0xFF; return;
                case "100Kbps": Timer0 = 0x04; Timer1 = 0x1C; return;
                case "125Kbps": Timer0 = 0x03; Timer1 = 0x1C; return;
                case "200Kbps": Timer0 = 0x81; Timer1 = 0xFA; return;
                case "250Kbps": Timer0 = 0x01; Timer1 = 0x1C; return;
                case "400Kbps": Timer0 = 0x80; Timer1 = 0xFA; return;
                case "500Kbps": Timer0 = 0x00; Timer1 = 0x1C; return;
                case "666Kbps": Timer0 = 0x80; Timer1 = 0xB6; return;
                case "800Kbps": Timer0 = 0x00; Timer1 = 0x16; return;
                case "1000Kbps": Timer0 = 0x00; Timer1 = 0x14; return;
                default: break;
            }


        }
        public override bool Open()
        {

            try
            {
                //CanPciCard
                string strKey = "Pci" + DeviceType.ToString() + "|" + DeviceIndex.ToString();
                if (!PciCanCard.ContainsKey(strKey))
                {
                    uint uValue = CANDll.VCI_OpenDevice(DeviceType, DeviceIndex, 0);
                    if (uValue == 1)
                    {
                        PciCanCard.Add(strKey, strKey + "Value");
                    }
                }
                GetBoundRate(CanParams);

                VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
                config.AccCode = 0;
                config.AccMask = 0xFFFFFFFF;
                config.Timing0 = Timer0;
                config.Timing1 = Timer1;
                config.Filter = 0;
                config.Mode = 0;

                uint Uinit = CANDll.VCI_InitCAN(DeviceType, DeviceIndex, CanId, ref config);
                uint UClear = CANDll.VCI_ClearBuffer(DeviceType, DeviceIndex, CanId);
                uint uReValue = CANDll.VCI_StartCAN(DeviceType, DeviceIndex, CanId);
                ThreadStart st = new ThreadStart(ReceiveData);
                Thread th = new Thread(st);
                th.IsBackground = true;
                th.Start();
                IsOpen = uReValue == 1;
                return IsOpen;
            }
            catch (Exception)
            {

                return false;
            }

        }
        public override bool Close()
        {
            if (CANDll.VCI_CloseDevice(DeviceType, DeviceIndex) != 1)
            {
                return false;
            }
            return true;

        }

        public override void SendData(byte[] Data, UInt32 ID)
        {
            lock (_Lock)
            {
                Frame_Num = Data.Length / 8;
                if (Data.Length % 8 != 0)
                {
                    Frame_Num += 1;
                }

                for (int i = 0; i < Frame_Num; i++)
                {
                    VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
                    if (Data.Length % 8 == 0)
                    {
                        sendobj.Data = new byte[8];
                        Array.Copy(Data, i * 8, sendobj.Data, 0, 8);
                        sendobj.DataLen = 8;
                    }
                    else
                    {
                        sendobj.Data = new byte[8];
                        Array.Copy(Data, i * 8, sendobj.Data, 0, (i == Frame_Num - 1) ? Data.Length % 8 : 8);
                        sendobj.DataLen = (byte)((i == Frame_Num - 1) ? Data.Length % 8 : 8);
                    }
                    sendobj.SendType = 0;
                    sendobj.RemoteFlag = 0;
                    sendobj.ID = ID;
                    if (CanId == 1 && ID == 2)
                        System.IO.File.AppendAllText("Log\\终端日志\\CanId=" + CanId + ",Id=" + ID + ".txt", DateTime.Now.ToString("HH:mm:ss fff ") + "发:" + GetByteToStr(sendobj.Data, sendobj.DataLen) + Environment.NewLine);

                    System.Threading.Thread.Sleep(5);
                    if (CANDll.VCI_Transmit(DeviceType, DeviceIndex, CanId, ref sendobj, 1) == 0)
                        return;
                }

            }
        }

        public uint GetBw(uint id)
        {
            uint Getbw = 0;
            if (id > 128)
            {
                Getbw = (id - 128) * 2;
            }
            else
            {
                Getbw = id * 2 - 1;
            }

            return Getbw;
        }

        private string GetByteToStr(byte[] bytTmp, int ilen)
        {
            string strFrame = "";
            for (int i = 0; i < ilen; i++)
            {
                strFrame += Convert.ToString(bytTmp[i], 16).PadLeft(2, '0') + " ";
            }
            return strFrame;
        }

        public void ReceiveData()
        {

            Dictionary<uint, List<byte>> RevData = new Dictionary<uint, List<byte>>();

            CANDll.VCI_ClearBuffer(DeviceType, DeviceIndex, CanId);
            while (true)
            {
                UInt32 res = new UInt32();
                res = CANDll.VCI_GetReceiveNum(DeviceType, DeviceIndex, CanId);

                if (res > 0)
                {
                    UInt32 con_maxlen = 60 * 20;
                    IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);



                    //Thread.Sleep(200);
                    res = CANDll.VCI_Receive(DeviceType, DeviceIndex, CanId, pt, con_maxlen, 100);
                    //CANDll.VCI_ClearBuffer(DeviceType, DeviceIndex, CanId);




                    for (UInt32 i = 0; i < res; i++)
                    {
                        VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));


                        if (obj.RemoteFlag == 1)
                        {

                            continue;
                        }
                        if (obj.DataLen == 0)
                        {
                            continue;

                        }
                        int iLen = obj.DataLen;
                        if (iLen > 8)
                        {
                            iLen = 8;
                        }
                        byte[] str = new byte[iLen];

                        //////////////////////////////////////////
                        if (obj.RemoteFlag == 0)
                        {
                            Array.Copy(obj.Data, 0, str, 0, iLen);
                            List<byte> listbyteTmp = new List<byte>();
                            listbyteTmp.AddRange(str);
                            if (CanId == 1 && obj.ID == 2)
                                System.IO.File.AppendAllText("Log\\终端日志\\CanId=" + CanId + ",Id=" + obj.ID + ".txt", DateTime.Now.ToString("HH:mm:ss fff ") + "收:" + GetByteToStr(listbyteTmp.ToArray(), listbyteTmp.ToArray().Length) + Environment.NewLine);

                            if (RevData.ContainsKey(obj.ID))
                            {
                                RevData[obj.ID].AddRange(listbyteTmp);
                            }
                            else
                            {
                                RevData.Add(obj.ID, new List<byte>());
                                RevData[obj.ID].AddRange(listbyteTmp);
                            }

                        }


                    }
                    Marshal.FreeHGlobal(pt);
                    //Thread.Sleep(50);
                    //res = CANDll.VCI_GetReceiveNum(DeviceType, DeviceIndex, CanId);

                    //if (res <= 0)
                    {//无数据时，返回数据；
                        AddBuffData(RevData);
                        RevData.Clear();

                    }

                }
                System.Threading.Thread.Sleep(5);
            }
        }
    }
}
