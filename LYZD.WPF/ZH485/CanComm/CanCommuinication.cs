using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZH.SocketModule.Sock;
using ZH485.Enum;

namespace ZH485.CanComm
{
    internal class CanCommuinication : IConnection
    {

        ///// <summary>
        ///// 波特率
        ///// </summary>
        //private string BaudRate = "2400";
        ///// <summary>
        ///// 数据位
        ///// </summary>
        //private string DataBits = "8";
        ///// <summary>
        ///// 停止位
        ///// </summary>
        //private string StopBits = "1";
        ///// <summary>
        ///// 校验位
        ///// </summary>
        //private string CheckBits = "E";

        //线程对象
        private Object ThreadObject = new object();

        //private object disPosObj = new object();

        private Enum.Cus_EmDeviceType m_EmDeviceType;

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
        ///// <summary>
        ///// 设备类型
        ///// </summary>
        //private uint deviceType = 16;
        ///// <summary>
        ///// Can索引号（卡的索引号）
        ///// </summary>
        //private uint deviceInd = 0;
        ///// <summary>
        ///// can 通道号
        ///// </summary>
        //private uint canInd = 0;
        /// <summary>
        /// Can通讯ID
        /// </summary>
        private uint CanCmtID;

        private CANCannel canCannel = null;

        /// <summary>
        /// 通道开启标志
        /// </summary>
        private bool bCanCanel = false;

        //private bool bUpdateBTL = false;

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
        public CanCommuinication( StCanParams sc, uint canCmtID)
        {
            str_Key = string.Format("Key|{0:D}|{1:D}|{2:D}", sc.devType, sc.DeviceIndex, sc.CanId);
            m_EmDeviceType = sc.m_EmDeviceType;
            if (!canDictionary.ContainsKey(str_Key))
            {
                canCannel = new CANCannel(sc);
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

            string[] ArraysParam = szSetting.Split(',');

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

                try
                {
                    if (canCannel.RevcieBuffs.ContainsKey(CanCmtID))
                    {
                        canCannel.RevcieBuffs[CanCmtID].Clear();
                        //Thread.Sleep(20);
                    }

                    canCannel.SendData(vData, CanCmtID);
                }
                catch (Exception)
                {
                    //Console.WriteLine("┗发送数据异常");
                    throw;
                }

                // 若不需要返回数据则直接返回
                if (!IsReturn)
                {
                    //Console.WriteLine("┗本包不需要回复");
                    return true;
                }

                //检测数据是否已经返回。





                bool IsOut = false;
                //System.Threading.Thread.Sleep(WaiteTime);
                DateTime TmpTime1 = DateTime.Now;

                while (TimeSub(DateTime.Now, TmpTime1) < MaxWaitSeconds)          //1秒超时器，如果超过表示收不到任何数据，直接退出
                {
                    try
                    {
                        System.Threading.Thread.Sleep(30);
                        IsOut = true;
                        //if (canCannel.RevcieBuffs.ContainsKey(CanCmtID))
                        //{
                        //    if (canCannel.RevcieBuffs[CanCmtID].Count > 0)  //如果缓冲区待接收数据量大于0
                        //    {
                        //        IsOut = false;
                        //        break;
                        //    }
                        //}

                        if (bol_youxiaozhen(canCannel.RevcieBuffs))
                        {
                            IsOut = false;
                            break;
                        }
                    }
                    catch (Exception)
                    {

                    }

                }
                if (IsOut)      //如果超时就将需要返回的数组数量设置为0
                {
                    vData = new byte[0];
                    //Console.WriteLine("┗RecvData:接收超时");
                    return false;
                }

                vData = canCannel.RevcieBuffs[CanCmtID].ToArray();
                canCannel.RevcieBuffs[CanCmtID].Clear();
                //Console.WriteLine("┗RecvData:{0}", BitConverter.ToString(vData));
            }


            else
            {
                //通道没有打开
                return false;
            }



            return true;
        }

        private bool bol_youxiaozhen(Dictionary<uint, List<byte>> drs)
        {
            lock (ThreadObject)
            {
                bool b1 = false;

                if (canCannel.RevcieBuffs.ContainsKey(CanCmtID))
                {
                    //if (canCannel.RevcieBuffs[CanCmtID].Count > 0)
                    //{
                    byte[] vData = canCannel.RevcieBuffs[CanCmtID].ToArray();
                    string sData = GetByteToStr(vData);
                    if (m_EmDeviceType == Enum.Cus_EmDeviceType.cl3761)
                    {
                        b1 = ReturnFlagFrame_3761(sData);
                    }
                    else if (m_EmDeviceType ==Enum.Cus_EmDeviceType.cl69845)
                    {
                        b1 = ReturnFlagFrame_69845(sData);
                    }
                    else
                    {
                        b1 = ReturnFlagFrame_3115(sData);
                    }
                    //}
                }
                return b1;
            }
        }

        public bool ReturnFlagFrame_69845(string strData)
        {
            //return true;
            //return true;

            bool boolfalg = true;
            string strTmp = strData;
            try
            {
                int a = strTmp.IndexOf("68");
                if (a > -1)//1.先判断有没有帧头68
                {

                    int ilen = Convert.ToInt16(strTmp.Substring(a + 2, 2), 16) + Convert.ToInt16(strTmp.Substring(a + 4, 2), 16) * 256;//3.获取帧长

                    if (strTmp.Substring(a + 2 + ilen * 2, 2) == "16")//4.判断帧尾16
                    {
                        //long crc16 = ClFrame_698.CheckCrc16(strTmp.Substring(2, ilen * 2 - 4), ilen - 2);
                        //long crc16Tmp = Convert.ToInt32(strTmp.Substring(ilen * 2 - 2, 2), 16) + Convert.ToInt32(strTmp.Substring(ilen * 2, 2), 16) * 256;
                        //if (crc16 != crc16Tmp)
                        //{
                        //    boolfalg = false;
                        //    strErr = "校验和不对,应为" + ClFrame_698.CheckCrc16(strTmp.Substring(2, ilen * 2 - 4), ilen - 2).ToString("x2");
                        //}
                    }
                    else
                    {
                        boolfalg = false;
                        //strErr = "无帧尾16";
                    }
                }
                else
                {
                    boolfalg = false;
                    //strErr = "无帧头68";
                }
            }
            catch
            {
                //strErr = "未知异常";
                boolfalg = false;//未对长度作详细判断，长度不够时，直接判无效帧
            }
            return boolfalg;
        }

        /// <summary>
        /// 返回376.1是否含有有效帧
        /// </summary>
        /// <returns></returns>
        public bool ReturnFlagFrame_3761(string strData)
        {
            bool boolfalg = true;
            string strTmp = strData;
            try
            {
                int a = strTmp.IndexOf("68");
                if (a > -1)//1.先判断有没有帧头68
                {
                    int ilen = Convert.ToInt16(strTmp.Substring(a + 2, 2), 16) / 4 + Convert.ToInt16(strTmp.Substring(a + 4, 2), 16) * 64;//3.获取帧长

                    if (strTmp.Substring(a + 14 + ilen * 2, 2) == "16")//4.判断帧尾16
                    {
                        if (GetChkSum(strTmp.Substring(a + 12, ilen * 2)) != Convert.ToByte(strTmp.Substring(a + 12 + ilen * 2, 2), 16))//5.判断校验和
                        {
                            boolfalg = false;
                        }
                    }
                    else
                    {
                        boolfalg = false;
                    }

                }
                else
                {
                    boolfalg = false;
                }
            }
            catch
            {
                boolfalg = false;//未对长度作详细判断，长度不够时，直接判无效帧
            }
            return boolfalg;
        }

        public bool ReturnFlagFrame_3115(string strData)
        {
            bool boolfalg = true;
            string strTmp = strData;
            try
            {
                int a = strTmp.IndexOf("81");
                if (a > -1)//1.先判断有没有帧头68
                {

                    int ilen = Convert.ToInt16(strTmp.Substring(a + 6, 2), 16);//3.获取帧长
                    if (GetChkXor(strTmp.Substring(a + 2, ilen * 2 - 4)) != Convert.ToByte(strTmp.Substring(+ilen * 2 - 2, 2), 16))//5.判断校验和
                    {
                        boolfalg = false;
                    }
                }
                else
                {
                    boolfalg = false;
                }
            }
            catch
            {
                boolfalg = false;//未对长度作详细判断，长度不够时，直接判无效帧
            }
            return boolfalg;
        }

        /// <summary>
        /// 校验和,将所有的项加起来除以256得到的余数
        /// </summary>
        /// <param name="aryData"></param>
        /// <returns></returns>
        private byte GetChkSum(string str)
        {
            int bytChk = 0;
            for (int i = 0; i < str.Length / 2; i++)
            {
                bytChk += Convert.ToInt16(str.Substring(2 * i, 2), 16);
            }
            return Convert.ToByte(bytChk % 256);
        }

        /// <summary>
        /// 异或校验
        /// </summary>
        /// <param name="aryData"></param>
        /// <returns></returns>
        private byte GetChkXor(string str)
        {
            int bytChk = 0;
            for (int i = 0; i < str.Length / 2; i++)
            {
                bytChk ^= Convert.ToInt16(str.Substring(2 * i, 2), 16);
            }
            return Convert.ToByte(bytChk % 256);
        }

        private string GetByteToStr(byte[] bytTmp)
        {
            string strFrame = "";
            for (int i = 0; i < bytTmp.Length; i++)
            {
                strFrame += Convert.ToString(bytTmp[i], 16).PadLeft(2, '0');
            }
            return strFrame;
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

                try
                {
                    if (canCannel.RevcieBuffs.ContainsKey(CanCmtID))
                    {
                        canCannel.RevcieBuffs[CanCmtID].Clear();
                        //Thread.Sleep(20);
                    }

                    canCannel.SendData(vData, CanCmtID);
                }
                catch (Exception)
                {
                    //Console.WriteLine("┗发送数据异常");
                    throw;
                }

                // 若不需要返回数据则直接返回
                if (!IsReturn)
                {
                    //Console.WriteLine("┗本包不需要回复");
                    return true;
                }

                //检测数据是否已经返回。





                bool IsOut = false;
                //System.Threading.Thread.Sleep(WaiteTime);
                DateTime TmpTime1 = DateTime.Now;

                while (TimeSub(DateTime.Now, TmpTime1) < MaxWaitSeconds)          //1秒超时器，如果超过表示收不到任何数据，直接退出
                {
                    try
                    {
                        System.Threading.Thread.Sleep(30);
                        IsOut = true;
                        //if (canCannel.RevcieBuffs.ContainsKey(CanCmtID))
                        //{
                        //    if (canCannel.RevcieBuffs[CanCmtID].Count > 0)  //如果缓冲区待接收数据量大于0
                        //    {
                        //        IsOut = false;
                        //        break;
                        //    }
                        //}
                        if (bol_youxiaozhen(canCannel.RevcieBuffs))
                        {
                            IsOut = false;
                            break;
                        }
                    }
                    catch (Exception)
                    {

                    }

                }
                if (IsOut)      //如果超时就将需要返回的数组数量设置为0
                {
                    vData = new byte[0];
                    //Console.WriteLine("┗RecvData:接收超时");
                    return false;
                }

                vData = canCannel.RevcieBuffs[CanCmtID].ToArray();
                canCannel.RevcieBuffs[CanCmtID].Clear();
                //Console.WriteLine("┗RecvData:{0}", BitConverter.ToString(vData));


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
