using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LY.VirtualMeter.Core.CAN
{
    public  class CanCommuinication
    {

        /// <summary>
        /// 波特率
        /// </summary>
        private string BaudRate = "2400";
        /// <summary>
        /// 数据位
        /// </summary>
        private string DataBits = "8";
        /// <summary>
        /// 停止位
        /// </summary>
        private string StopBits = "1";
        /// <summary>
        /// 校验位
        /// </summary>
        private string CheckBits = "E";

        //线程对象
        private Object ThreadObject = new object();

        private object disPosObj = new object();

        /// <summary>
        /// 链接名称
        /// </summary>
        public string ConnectName
        {
            get;
            set;
        }
        /// <summary>
        /// 最大等待时间
        /// </summary>
        public int MaxWaitSeconds
        {
            get;
            set;
        }
        /// <summary>
        /// 字节间延时
        /// </summary>
        public int WaitSecondsPerByte
        {
            get;
            set;
        }
        /// <summary>
        /// 设备类型
        /// </summary>
        private uint deviceType = 16;
        /// <summary>
        /// Can索引号（卡的索引号）
        /// </summary>
        private uint deviceInd = 0;
        /// <summary>
        /// can 通道号
        /// </summary>
        private uint canInd = 0;
        /// <summary>
        /// Can通讯ID
        /// </summary>
        private uint CanCmtID;

        private CANCannel canCannel = null;

        /// <summary>
        /// 通道开启标志
        /// </summary>
        private bool bCanCanel = false;

        private bool bUpdateBTL = false;

        private string str_Key = string.Empty;

        public static Dictionary<string, CANCannel> canDictionary = new Dictionary<string, CANCannel>();

        //线程信号
        //static AutoResetEvent myResetEvent = new AutoResetEvent(false);

        //将接收到数据缓存起来
        //private Dictionary<uint, LinkedList<VCI_CAN_OBJ>> ReciveData = new Dictionary<uint, LinkedList<VCI_CAN_OBJ>>();

        //接收的处理过后的数据
        //private Dictionary<uint, List<byte>> ReciveBytes = new Dictionary<uint, List<byte>>();

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="devType">设备类型</param>
        /// <param name="devInd">设备索引号</param>
        /// <param name="canId">通道号</param>
        /// <param name="canCmtID">Can通讯ID号</param>
        public CanCommuinication(string strCanParams, uint devType, uint devInd, uint canId, uint canCmtID)
        {
            str_Key = string.Format("Key|{0:D}|{1:D}|{2:D}", devType, devInd, canId);
            if (!canDictionary.ContainsKey(str_Key))
            {
                canCannel = new CANCannel(strCanParams, devType, devInd, canId);

                //canDictionary.Add(str_Key, canCannel);
                //
                //canCannel.SendBuffDataEvent += new PortBase.DteRevBuffData(canCannel_SendBuffDataEvent);

            }
            else
            {
                canCannel = canDictionary[str_Key];
                //canCannel.SendBuffDataEvent += new PortBase.DteRevBuffData(canCannel_SendBuffDataEvent);
            }

            CanCmtID = canCmtID;

        }

        /// <summary>
        /// 打开设备 打开设备相应的Can通道号
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            if (!canDictionary.ContainsKey(str_Key))
            {
                bCanCanel = canCannel.Open();
                if (bCanCanel)
                {
                    canDictionary.Add(str_Key, canCannel);
                }
            }
            else
            {
                bCanCanel = true;
            }
            //if (!canCannel.IsOpen)
            //{
            //    canCannel.Open();
            //}

            return bCanCanel == true;
        }
        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            //uint reValue = CanCardFucntion.VCI_CloseDevice(deviceType, deviceInd);
            //return reValue == 1;
            if (canDictionary.ContainsKey(str_Key))
            {
                return canCannel.Close();
            }
            return true;
        }

        /// <summary>
        /// 更新设备波特率
        /// </summary>
        /// <param name="szSetting"></param>
        /// <returns></returns>
        public bool UpdateBltSetting(string szSetting)
        {

            if (!canDictionary.ContainsKey(str_Key))
            {
                return false;
            }

            string strReValue = string.Empty;

            string[] ArraysParam = szSetting.Split('-');

            List<byte> listDataField = new List<byte>();


            listDataField.Add(Convert.ToByte(CanCmtID));
            listDataField.Add(0xFE);
            listDataField.Add(0x0F);
            byte[] byteData = BitConverter.GetBytes(Convert.ToInt32(ArraysParam[0], 10));
            listDataField.Add(byteData[1]);
            listDataField.Add(byteData[0]);
            //数据位
            if (ArraysParam[1].ToUpper().Equals("E") || ArraysParam[1].ToUpper().Equals("O"))
            {
                listDataField.Add(0x10);
                listDataField.Add(0x00);
            }
            else
            {
                listDataField.Add(0x00);
                listDataField.Add(0x00);
            }
            //停止位
            if (ArraysParam[3].Equals("1"))
            {
                listDataField.Add(0x00);
                listDataField.Add(0x00);
            }
            else if (ArraysParam[3].Equals("1.5"))
            {
                listDataField.Add(0x30);
                listDataField.Add(0x00);
            }
            else if (ArraysParam[3].Equals("2"))
            {
                listDataField.Add(0x20);
                listDataField.Add(0x00);
            }

            if (ArraysParam[1].ToUpper().Equals("N"))
            {
                //无校验
                listDataField.Add(0x00);
                listDataField.Add(0x00);
            }
            else if (ArraysParam[1].ToUpper().Equals("E"))
            {
                //偶校验
                listDataField.Add(0x04);
                listDataField.Add(0x00);
            }
            else if (ArraysParam[1].ToUpper().Equals("O"))
            {
                //奇校验
                listDataField.Add(0x06);
                listDataField.Add(0x00);
            }

            //数据流控制位
            listDataField.Add(0x00);
            listDataField.Add(0x00);
            byte byteJy = listDataField[0];
            for (int i = 1; i < listDataField.Count; i++)
            {
                byteJy = Convert.ToByte(byteJy ^ listDataField[i]);
            }

            listDataField.Add(byteJy);
            listDataField.Insert(0, 0x81);

            //strReValue += BytesToString(listDataField.ToArray());

            byte[] SendDataCmd = listDataField.ToArray();

            //canCannel.SendData(SendDataCmd, CanCmtID);
            this.SendData(ref SendDataCmd, false, 100, 100);

            return true;

        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="vData">发送的数据</param>
        /// <param name="IsReturn">是否需要返回数据</param>
        /// <param name="WaiteTime">发送完成后等待时间</param>
        /// <returns></returns>
        unsafe public bool SendData(ref byte[] vData, bool IsReturn, int WaiteTime, int MaxByte)
        {
            //
            if (vData.Length < 1)
                return false;


            if (bCanCanel)
            {
                lock (ThreadObject)
                {
                    try
                    {
                        if (App.RevcieBuffs.ContainsKey(CanCmtID))
                        {
                            App.RevcieBuffs[CanCmtID].Clear();
                            //Thread.Sleep(20);
                        }
                        //Thread.Sleep(10);
                        canCannel.SendData(vData, CanCmtID);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("┗发送数据异常");
                        throw;
                    }

                    // 若不需要返回数据则直接返回
                    if (!IsReturn)
                    {
                        Console.WriteLine("┗本包不需要回复");
                        return true;
                    }

                    //检测数据是否已经返回。





                    bool IsOut = false;
                    System.Threading.Thread.Sleep(WaiteTime);
                    DateTime TmpTime1 = DateTime.Now;

                    while (TimeSub(DateTime.Now, TmpTime1) < MaxWaitSeconds)          //1秒超时器，如果超过表示收不到任何数据，直接退出
                    {
                        try
                        {
                            System.Threading.Thread.Sleep(1);
                            IsOut = true;
                            if (App.RevcieBuffs.ContainsKey(CanCmtID))
                            {
                                if (App.RevcieBuffs[CanCmtID].Count > 0)  //如果缓冲区待接收数据量大于0
                                {
                                    IsOut = false;
                                    break;
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }

                    }
                    if (IsOut)      //如果超时就将需要返回的数组数量设置为0
                    {
                        vData = new byte[0];
                        Console.WriteLine("┗RecvData:接收超时");
                        return false;
                    }

                    vData = App.RevcieBuffs[CanCmtID].ToArray();
                    App.RevcieBuffs[CanCmtID].Clear();
                    Console.WriteLine("┗RecvData:{0}", BitConverter.ToString(vData));
                }

            }
            else
            {
                //通道没有打开
                return false;
            }



            return true;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="vData">发送的数据</param>
        /// <param name="IsReturn">是否需要返回数据</param>
        /// <param name="WaiteTime">发送完成后等待时间</param>
        /// <returns></returns>
        unsafe public bool SendData(ref byte[] vData, bool IsReturn, int WaiteTime, int MaxByte, int MaxWaitSeconds)
        {
            //
            if (vData.Length < 1)
                return false;


            if (bCanCanel)
            {
                lock (ThreadObject)
                {
                    try
                    {
                        if (App.RevcieBuffs.ContainsKey(CanCmtID))
                        {
                            App.RevcieBuffs[CanCmtID].Clear();
                            //Thread.Sleep(20);
                        }

                        canCannel.SendData(vData, CanCmtID);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("┗发送数据异常");
                        throw;
                    }

                    // 若不需要返回数据则直接返回
                    if (!IsReturn)
                    {
                        Console.WriteLine("┗本包不需要回复");
                        return true;
                    }

                    //检测数据是否已经返回。





                    bool IsOut = false;
                    System.Threading.Thread.Sleep(WaiteTime);
                    DateTime TmpTime1 = DateTime.Now;

                    while (TimeSub(DateTime.Now, TmpTime1) < MaxWaitSeconds)          //1秒超时器，如果超过表示收不到任何数据，直接退出
                    {
                        try
                        {
                            System.Threading.Thread.Sleep(1);
                            IsOut = true;
                            if (App.RevcieBuffs.ContainsKey(CanCmtID))
                            {
                                if (App.RevcieBuffs[CanCmtID].Count > 0)  //如果缓冲区待接收数据量大于0
                                {
                                    IsOut = false;
                                    break;
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }

                    }
                    if (IsOut)      //如果超时就将需要返回的数组数量设置为0
                    {
                        vData = new byte[0];
                        Console.WriteLine("┗RecvData:接收超时");
                        return false;
                    }

                    vData = App.RevcieBuffs[CanCmtID].ToArray();
                    App.RevcieBuffs[CanCmtID].Clear();
                    Console.WriteLine("┗RecvData:{0}", BitConverter.ToString(vData));
                }

            }
            else
            {
                //通道没有打开
                return false;
            }



            return true;
        }

        private long TimeSub(DateTime Time1, DateTime Time2)
        {
            TimeSpan tsSub = Time1.Subtract(Time2);
            return tsSub.Hours * 60 * 60 * 1000 + tsSub.Minutes * 60 * 1000 + tsSub.Seconds * 1000 + tsSub.Milliseconds;
        }


    }
}
