using LYZD.Core.Enum;
using LYZD.Core.Model.Meter;
using LYZD.Core.Struct;
using LYZD.ViewModel.CheckController.MulitThread;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using ZH.MeterProtocol;
using ZH.MeterProtocol.Comm;
using ZH.MeterProtocol.Encryption;
using ZH.MeterProtocol.Protocols.DLT698.Enum;
using ZH.MeterProtocol.Protocols.DLT698.Struct;
using ZH.MeterProtocol.Struct;

namespace LYZD.ViewModel.CheckController
{
    public class MeterProtocolAdapter : SingletonBase<MeterProtocolAdapter>
    {
       public   LY3762  lY3762 = new LY3762();
        #region 属性
        //运行标志
        private bool runFlag = false;

        const string meterProtocolDll = "ZH.MeterProtocol.dll";
        /// <summary>
        /// 电能表多功能操作对象
        /// </summary>
        public IMeterProtocol[] MeterProtocols = null;
        /// <summary>
        /// 每一通道负载
        /// </summary>
        private int oneChannelMeterCount = 1;
        /// <summary>
        /// 硬件路数，一般来说，一路硬件将启动一个通讯线程
        /// </summary>
        private int channelCount = 1;
        /// <summary>
        /// 表位数量
        /// </summary>
        private int bwCount = 24;
        public void SetBwCount(int bws)
        {
            bwCount = bws;
            MeterProtocols = new IMeterProtocol[bws];
        }

        ///// <summary>
        ///// 运行标志
        ///// </summary>
        //public bool IsWork { get; private set; }
        #endregion

        #region 方法
        /// <summary>
        /// HEX字符串转字节数组,并反转
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        private byte[] StringToByte(string hexStr)
        {
            int len = hexStr.Length / 2;
            byte[] frame = new byte[len];
            for (int i = len - 1; i >= 0; i--)
            {
                string bb = hexStr.Substring(i * 2, 2);
                frame[len - 1 - i] = Convert.ToByte(bb, 16); ;
            }
            return frame;

        }


        /// <summary>
        /// 初始化被检表协议    
        /// </summary>
        /// <param name="dgnProtocols">电能表协议数组, string[] meterAddr_MAC</param>
        public void Initialize(ZH.MeterProtocol.Protocols.DgnProtocol.DgnProtocolInfo[] dgnProtocols, string[] meterAddr, ComPortInfo[] ComPortInfo)
        {
            //创建协议对象            
            int currentChannelID = 0;
            string currentChannelPortName = string.Empty;
            MeterProtocolManager.Instance.Initialize(ComPortInfo);
            channelCount = MeterProtocolManager.Instance.GetChannelCount();
            oneChannelMeterCount = bwCount / channelCount;
            if (oneChannelMeterCount == 0)//TODO2:每条总线负载
            {
                //MessageController.Instance.AddMessage("485通道错误，配置485通道数！（通道数最大等于表位数，不能超过表位数。）", 6, 2);

                return;
                //oneChannelMeterCount = 1;
            }
            //更新线程管理对象
            MulitThreadManager.Instance.MaxThread = channelCount;
            MulitThreadManager.Instance.MaxTaskCountPerThread = oneChannelMeterCount;

            for (int i = 0; i < dgnProtocols.Length; i++)
            {
                if (dgnProtocols[i] == null)
                {
                    continue;
                }
                IMeterProtocol newInterface = CreateInstance(dgnProtocols[i].ClassName);
                if (newInterface != null)
                {
                    newInterface.SetProtocol(dgnProtocols[i]);   //设置方案内容
                    newInterface.SetMeterAddress(meterAddr[i]);  //设置电表地址
                    //newInterface.SetMeterAddress(meterAddr_MAC[i]);  //设置电表地址
                    currentChannelID = GetChannelByPos(i + 1);
                    currentChannelPortName =MeterProtocolManager.Instance.GetChannelPortName(currentChannelID);
                    newInterface.SetPortName(currentChannelPortName);//通讯端口
                }
                else
                {
                    Utility.Log.LogManager.AddMessage(string.Format("第{0}表位创建多功能协议对象失败，没有找到与对{1}应的多功能协议", i + 1, dgnProtocols[i].ClassName), Utility.Log.EnumLogSource.检定业务日志, Utility.Log.EnumLevel.Warning);
                }
                MeterProtocols[i] = newInterface;
            }

        }

        /// <summary>
        /// 通过反射创建电能表协议对象
        /// </summary>
        /// <param name="className">多功能协议类</param>
        /// <returns>IMeterProtocol</returns>
        private IMeterProtocol CreateInstance(string className)
        {
            try
            {
                string dllPath =GetPhyPath(meterProtocolDll);
                string classFullName = string.Format("{0}.{1}", "ZH.MeterProtocol", className);
                Assembly asm = Assembly.LoadFile(dllPath);
                object obj = asm.CreateInstance(classFullName);//Activator.CreateInstance(asm.GetType(),className);
                return obj as IMeterProtocol;
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 通过表位计算通道号
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <returns>通道号，下标从1开始</returns>
        private int GetChannelByPos(int pos)
        {
            pos--;
            int currentChannel = pos / oneChannelMeterCount;
            currentChannel++;
            return currentChannel;
        }

        /// <summary>
        /// 根据相对路径获取文件、文件夹绝对路径
        /// </summary>
        /// <param name="FileName">相对路径</param>   
        /// <returns></returns>
        public static string GetPhyPath(string FileName)
        {
            FileName = FileName.Replace('/', '\\');             //规范路径写法
            if (FileName.IndexOf(':') != -1) return FileName;   //已经是绝对路径了
            if (FileName.Length > 0 && FileName[0] == '\\') FileName = FileName.Substring(1);
            return string.Format("{0}\\{1}", Directory.GetCurrentDirectory(), FileName);
        }

        /// <summary>
        /// 等待所有线程完成
        /// </summary>
        private void WaitWorkDone()
        {
            while (true)
            {
                if (!runFlag) break;
                if (MulitThreadManager.Instance.IsWorkDone())
                {
                    runFlag = false;
                    break;
                }
                System.Threading.Thread.Sleep(300);
            }
        }
        #endregion

        #region 读写数据
        //#region 读取数据：指定ID

        ///// <summary>
        ///// 读取数据（字符型，数据项）
        ///// </summary>
        ///// <param name="str_ID">标识符,2个字节</param>
        ///// <param name="int_Len">数据长度(字节数)</param>
        ///// <returns></returns>
        //public string[] ReadLoadRecord(string str_ID, int int_Len, string strItem)
        //{
        //    string[] arrRet = new string[bwCount];
        //    runFlag = true;
        //    MulitThreadManager.Instance.DoWork = delegate (int pos)
        //    {
        //        if (!runFlag) return;
        //        if (!Switch485Channel(pos)) return;
        //        arrRet[pos] = ReadLoadRecordByPos(str_ID, int_Len, strItem, pos);
        //    };
        //    MulitThreadManager.Instance.Start();
        //    //等所有返回
        //    WaitWorkDone();
        //    return arrRet;
        //}

        //public string ReadLoadRecordByPos(string str_ID, int int_Len, string strItem, int pos)
        //{
        //    if (MeterProtocols[pos] == null) return string.Empty;
        //    if (!VerifyBase.meterInfo[pos].YaoJianYn) return string.Empty;
        //    return MeterProtocols[pos].ReadData(str_ID, int_Len, strItem);
        //}
        ///// <summary>
        ///// 读取数据（数据型，数据项）
        ///// </summary>
        ///// <param name="str_ID">标识符,2个字节</param>
        ///// <param name="int_Len">数据长度(字节数)</param>
        ///// <param name="int_Dot">小数位</param>
        ///// <returns>返回数据</returns>
        //public float[] ReadData(string str_ID, int int_Len, int int_Dot)
        //{
        //    float[] arrRet = new float[bwCount];
        //    runFlag = true;
        //    MulitThreadManager.Instance.DoWork = delegate (int pos)
        //    {
        //        if (!runFlag) return;
        //        if (!Switch485Channel(pos)) return;
        //        arrRet[pos] = ReadData(str_ID, int_Len, int_Dot, pos);
        //    };
        //    MulitThreadManager.Instance.Start();
        //    //等所有返回
        //    WaitWorkDone();
        //    return arrRet;
        //}

        ///// <summary>
        ///// 读取数据（数据型，数据项）
        ///// </summary>
        ///// <param name="str_ID">标识符,2个字节</param>
        ///// <param name="int_Len">数据长度(字节数)</param>
        ///// <param name="int_Dot">小数位</param>
        ///// <returns>返回数据</returns>
        //public string[] ReadData(string sendData)
        //{
        //    string[] arrRet = new string[bwCount];
        //    runFlag = true;
        //    MulitThreadManager.Instance.DoWork = delegate (int pos)
        //    {
        //        if (!runFlag) return;
        //        if (!Switch485Channel(pos)) return;
        //        arrRet[pos] = ReadDatas(sendData, pos);
        //    };
        //    MulitThreadManager.Instance.Start();
        //    //等所有返回
        //    WaitWorkDone();
        //    return arrRet;
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="str_ID"></param>
        ///// <param name="int_Len"></param>
        ///// <param name="int_Dot"></param>
        ///// <param name="pos">start zero</param>
        ///// <returns></returns>
        //public float ReadData(string str_ID, int int_Len, int int_Dot, int pos)
        //{
        //    if (MeterProtocols[pos] == null) return 0F;
        //    if (!VerifyBase.meterInfo[pos].YaoJianYn) return 0F;
        //    return MeterProtocols[pos].ReadData(str_ID, int_Len, int_Dot);
        //}


        //public string ReadDatas(string sendData, int pos)
        //{
        //    if (MeterProtocols[pos] == null) return "0F";
        //    if (!VerifyBase.meterInfo[pos].YaoJianYn) return "0F";
        //    return MeterProtocols[pos].ReadData(sendData);
        //}

        ///// <summary>
        ///// 读取数据（字符型，数据项）
        ///// </summary>
        ///// <param name="str_ID">标识符,2个字节</param>
        ///// <param name="int_Len">数据长度(字节数)</param>
        ///// <returns></returns>
        //public string[] ReadData(string str_ID, int int_Len)
        //{
        //    string[] arrRet = new string[bwCount];
        //    runFlag = true;
        //    MulitThreadManager.Instance.DoWork = delegate (int pos)
        //    {
        //        if (!runFlag) return;
        //        if (!Switch485Channel(pos)) return;
        //        arrRet[pos] = ReadDataByPos(str_ID, int_Len, pos);
        //    };
        //    MulitThreadManager.Instance.Start();
        //    //等所有返回
        //    WaitWorkDone();
        //    return arrRet;
        //}

        //public string ReadDataByPos(string str_ID, int int_Len, int pos)
        //{
        //    if (MeterProtocols[pos] == null) return string.Empty;
        //    if (!VerifyBase.meterInfo[pos].YaoJianYn) return string.Empty;
        //    return MeterProtocols[pos].ReadData(str_ID, int_Len);
        //}

        //#endregion

        //#region 写数据+
        ///// <summary>
        ///// 写数据
        ///// </summary>
        ///// <param name="str_ID">标识符</param>
        ///// <param name="byt_Value">写入数据</param>
        ///// <returns></returns>
        //public bool[] WriteData(string str_ID, byte[] byt_Value)
        //{
        //    bool[] arrRet = new bool[bwCount];
        //    runFlag = true;
        //    MulitThreadManager.Instance.DoWork = delegate (int pos)
        //    {
        //        if (!runFlag) return;
        //        if (!Switch485Channel(pos)) return;
        //        arrRet[pos] = WriteData(str_ID, byt_Value, pos);
        //    };
        //    MulitThreadManager.Instance.Start();
        //    //等所有返回
        //    WaitWorkDone();
        //    return arrRet;
        //}
        //public bool WriteData(string str_ID, byte[] byt_Value, int pos)
        //{
        //    if (MeterProtocols[pos] == null) return false;
        //    if (!VerifyBase.meterInfo[pos].YaoJianYn) return true;
        //    bool bln_Rst = false;

        //    bln_Rst = MeterProtocols[pos].WriteData(str_ID, byt_Value);

        //    return bln_Rst;
        //}

        ///// <summary>
        ///// 写数据(字符型，数据项)
        ///// </summary>
        ///// <param name="str_ID">标识符,两个字节</param>
        ///// <param name="int_Len">数据长度(块中每项字节数)</param>
        ///// <param name="str_Value">写入数据</param>
        ///// <returns></returns>
        //public bool[] WriteArrData(string str_ID, int int_Len, string[] str_Value)
        //{
        //    bool[] arrRet = new bool[bwCount];
        //    runFlag = true;
        //    MulitThreadManager.Instance.DoWork = delegate (int pos)
        //    {
        //        if (!runFlag) return;
        //        arrRet[pos] = WriteData(str_ID, int_Len, str_Value[pos], pos);
        //    };
        //    MulitThreadManager.Instance.Start();
        //    //等所有返回
        //    WaitWorkDone();
        //    return arrRet;
        //}

        ///// <summary>
        ///// 写数据(字符型，数据项)
        ///// </summary>
        ///// <param name="str_ID">标识符,两个字节</param>
        ///// <param name="int_Len">数据长度(块中每项字节数)</param>
        ///// <param name="str_Value">写入数据</param>
        ///// <returns></returns>
        //public bool[] WriteData(string str_ID, int int_Len, string str_Value)
        //{
        //    bool[] arrRet = new bool[bwCount];
        //    runFlag = true;
        //    MulitThreadManager.Instance.DoWork = delegate (int pos)
        //    {
        //        if (!runFlag) return;
        //        arrRet[pos] = WriteData(str_ID, int_Len, str_Value, pos);
        //    };
        //    MulitThreadManager.Instance.Start();
        //    //等所有返回
        //    WaitWorkDone();
        //    return arrRet;
        //}
        //public bool WriteData(string str_ID, int int_Len, string str_Value, int pos)
        //{
        //    if (MeterProtocols[pos] == null) return false;
        //    if (!VerifyBase.meterInfo[pos].YaoJianYn) return true;
        //    bool blnWriteType = false;

        //    bool bln_Rst = false;
        //    if (Helper.MeterDataHelper.Instance.Meter(pos).DgnProtocol.IsSouthEncryption) //南网费控
        //    {
        //        //身份认证
        //        //判断参数更新文件
        //        //处理数据明文
        //        //取得密文
        //        //组帧发送电表
        //        if (str_ID == "04001503")//三类数据
        //        {
        //            bln_Rst = MeterProtocols[pos].WriteData(str_ID, int_Len, str_Value);
        //        }
        //        else if (CheckStrIDType(str_ID) == 2)//二类数据
        //        {


        //            string rand1 = "";
        //            string rand2 = "";
        //            string esamNo = "";
        //            string[] PutData = new string[this.bwCount];
        //            string[] DataCode = new string[bwCount];
        //            string[] strData = new string[bwCount];
        //            string[] strID = new string[bwCount];
        //            int Fag = SouthCheckIdentity(pos, out rand1, out rand2, out esamNo); // 检查密钥状态
        //            if (Fag <= 1)
        //            {
        //                string str_div =VerifyBase.meterInfo[pos].MD_MeterNo;
        //                for (int i = 0; i < bwCount; i++)
        //                {
        //                    strID[i] = str_ID;
        //                    strData[i] = str_ID + str_Value;
        //                }
        //                bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(Convert.ToInt32(pos), Fag, rand2, strData[pos], strID[pos]);
        //            }
        //        }
        //        else//一类数据
        //        {
        //            string rand1 = "";
        //            string rand2 = "";
        //            string esamNo = "";
        //            string[] PutData = new string[this.bwCount];
        //            string[] DataCode = new string[bwCount];
        //            string[] strData = new string[bwCount];
        //            string[] strID = new string[bwCount];
        //            int Fag = SouthCheckIdentity(pos, out rand1, out rand2, out esamNo); // 检查密钥状态
        //            if (Fag <= 1)
        //            {
        //                string str_div = VerifyBase.meterInfo[pos].MD_MeterNo;
        //                for (int i = 0; i < bwCount; i++)
        //                {
        //                    strID[i] = str_ID;
        //                    strData[i] = str_ID + str_Value;
        //                }
        //                bln_Rst = MeterProtocolAdapter.Instance.SouthParameterUpdate(Convert.ToInt32(pos), Fag, rand2, "", strID[pos], strData[pos]);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        bln_Rst = MeterProtocols[pos].WriteData(str_ID, int_Len, str_Value);
        //    }
        //    return bln_Rst;
        //}

        ///// <summary>
        ///// 参变量分类
        ///// </summary>
        ///// <param name="str_ID">协议标识</param>
        ///// <returns>1类,2类,3类</returns>
        //private int CheckStrIDType(string str_IDs)
        //{
        //    int tp = 0;
        //    string str_ID = str_IDs.ToUpper();
        //    if (DicIDType.ContainsKey(str_ID))
        //    {
        //        tp = 1;
        //    }
        //    else if (str_ID.IndexOf("040501", 0) > 0)
        //    {
        //        tp = 1;
        //    }
        //    else if (str_ID.IndexOf("040502", 0) > 0)
        //    {
        //        tp = 1;
        //    }
        //    else
        //    {
        //        tp = 2;
        //    }
        //    return tp;
        //}
        //#endregion
        #endregion

        #region 试验方法
        /// <summary>
        /// 打开RS485通断状态
        /// </summary>
        /// <param name="pos">表位号,0-bwCount.0xFF为全部打开0xFE为全部关闭</param>
        /// <returns></returns>
        private bool Switch485Channel(int pos)
        {
            return true;
            //EquipHelper.Instance.Switch485Channel(pos);
            //Thread.Sleep(100);
            //return EquipHelper.Instance.Switch485Channel(pos);
        }

        #region 通信测试

        /// <summary>
        /// 通讯测试[所有表位]
        /// </summary>
        /// <returns>通讯测试是否全部通过</returns>
        public bool[] CommTest()
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (MulitThreadManager.Instance.MaxTaskCountPerThread > 2)
                    if (!Switch485Channel(pos)) return;
                arrRet[pos] = CommTest(pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }
        /// <summary>
        /// 通讯测试[指定表位]
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <returns>通讯测试是否通过</returns>
        public bool CommTest(int pos)
        {
            if (MeterProtocols[pos] == null)
                return false;
            if (!VerifyBase.meterInfo[pos].YaoJianYn) return true;
            //设置通道
            return MeterProtocols[pos].ComTest();
        }

        #endregion



        /// <summary>
        /// 读取指定表位置的电表
        /// </summary>
        /// <param name="pos">表号</param>
        /// <returns></returns>
        public string ReadAddress(int pos)
        {
            if (MeterProtocols[pos] == null) return string.Empty;
            if (!VerifyBase.meterInfo[pos].YaoJianYn) return string.Empty;
            return MeterProtocols[pos].ReadAddress();
        }
        //读通信地址
        /// <summary>
        /// 读取电表地址
        /// </summary>
        /// <returns>电表地址</returns>
        public string[] ReadAddress()
        {
            string[] arrAddress = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (MulitThreadManager.Instance.MaxTaskCountPerThread > 2)
                    if (!Switch485Channel(pos)) return;
                arrAddress[pos] = ReadAddress(pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrAddress;
        }




        #region 日期时间
        //读日期时间
        public DateTime[] ReadDateTime()
        {
            DateTime[] arrRet = new DateTime[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (MulitThreadManager.Instance.MaxTaskCountPerThread > 2)
                    if (!Switch485Channel(pos)) return;
                arrRet[pos] = ReadDateTime(pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public DateTime ReadDateTime(int pos)
        {
            if (MeterProtocols[pos] == null) return DateTime.Now;
            if (!VerifyBase.meterInfo[pos].YaoJianYn) return DateTime.Now;
            return MeterProtocols[pos].ReadDateTime();
        }


        //写日期时间+
        public bool[] WriteDateTime(DateTime newTime)
        {
            bool[] arrRet = new bool[bwCount];

            runFlag = true;

            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (MulitThreadManager.Instance.MaxTaskCountPerThread > 2)
                    if (!Switch485Channel(pos)) return;
                if (!EquipmentData.Equipment.IsDemo)
                {
                    arrRet[pos] = WriteDateTime(newTime, pos);
                }
                else
                    arrRet[pos] = true;
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();

            return arrRet;
        }

        public bool WriteDateTime(DateTime newTime, int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!VerifyBase.meterInfo[pos].YaoJianYn) return true;
            //二类数据
            bool rst = false;
            if (!VerifyBase.meterInfo[pos].DgnProtocol.HaveProgrammingkey)
            {
                if (VerifyBase.meterInfo[pos].DgnProtocol.ClassName == "CDLT6452007")
                {
                    string outEndata = "";
                    string msg = "";
                    string div = VerifyBase.meterInfo[pos].MD_MeterNo;

                    int weekday = (int)newTime.DayOfWeek;
                    bool rst3 = EncrypGW.ParameterElseUpDate(MeterProtocols[pos].IdentityStatus, VerifyBase.meterInfo[pos].Rand, VerifyBase.meterInfo[pos].EsamId, div, "04d6890014", "0400010C" + newTime.ToString("yyMMdd") + "0" + weekday + newTime.ToString("HHmmss"), ref outEndata, ref msg);

                    if (rst3 == true)
                    {
                        byte[] tmp = new byte[outEndata.Length / 2];
                        Array.Copy(StringToByte(outEndata.Substring(0, 32)), 0, tmp, 0, 16);
                        Array.Copy(StringToByte(outEndata.Substring(32, 8)), 0, tmp, 16, 4);
                        rst = MeterProtocols[pos].WriteData("0400010C", tmp);
                    }
                }
                else if (VerifyBase.meterInfo[pos].DgnProtocol.ClassName == "CDLT698")
                {
                    string year = newTime.Year.ToString("X").PadLeft(4, '0');
                    string month = newTime.Month.ToString("X").PadLeft(2, '0');
                    string day = newTime.Day.ToString("X").PadLeft(2, '0');
                    string hour = newTime.Hour.ToString("X").PadLeft(2, '0');
                    string min = newTime.Minute.ToString("X").PadLeft(2, '0');
                    string sec = newTime.Second.ToString("X").PadLeft(2, '0');

                    //"060101" + "40000200" + "1C" + year + month + day + hour + min + sec + "00"
                    string taskData = string.Format("060101400002001C{0}{1}{2}{3}{4}{5}00", year, month, day, hour, min, sec);
                    string dataFlag = "40000200";

                    rst = Operation(pos, 3, 3, taskData, dataFlag, EmSecurityMode.CiphertextMac);
                }

            }
            else
            {
                rst = MeterProtocols[pos].WriteDateTime(newTime.ToString("yyyyMMddHHmmss"));
            }
            return rst;
        }
        #endregion


        #region 钱包初始化
        //钱包初始化+
        /// <summary>
        /// 钱包初始化
        /// </summary>
        /// <param name="money">剩余金额,单位分</param>
        /// <returns></returns>
        public bool[] InitPurse(int money)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (MulitThreadManager.Instance.MaxTaskCountPerThread > 2)
                    if (!Switch485Channel(pos)) return;
                if (!EquipmentData.Equipment.IsDemo)
                    arrRet[pos] = InitPurse(pos, money);
                else
                    arrRet[pos] = true;
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        /// <summary>
        /// 单表位钱包初始化
        /// </summary>
        /// <param name="pos">表位索引号，从0开始</param>
        /// <returns></returns>
        private bool InitPurse(int pos, int money)
        {
            if (MeterProtocols[pos] == null) return false;
            TestMeterInfo meter =VerifyBase.meterInfo[pos];
            if (!meter.YaoJianYn) return true;

            bool result = false;

            //MeterProtocols[0]
            if (!meter.DgnProtocol.HaveProgrammingkey)
            {
                if (meter.DgnProtocol.ClassName == "CDLT6452007")
                {
                    string outEndata = "";
                    string msg = "";
                    //15块钱对应于1500分对应的16进制为0x5DC
                    bool rst3 = EncrypGW.Meter_Formal_InintPurse(0, meter.Rand, meter.MD_MeterNo, money.ToString("X8"), ref outEndata, ref msg);
                    if (rst3 == true)
                    {
                        byte[] data = new byte[outEndata.Length / 2];
                        Array.Copy(StringToByte(outEndata.Substring(0, 8)), 0, data, 0, 4);
                        Array.Copy(StringToByte(outEndata.Substring(8, 8)), 0, data, 4, 4);
                        Array.Copy(StringToByte(outEndata.Substring(16, 8)), 0, data, 8, 4);
                        Array.Copy(StringToByte(outEndata.Substring(24, 8)), 0, data, 12, 4);

                        string tmp = BitConverter.ToString(data).Replace("-", "");
                        result = MeterProtocols[pos].InitPurse(tmp);
                    }
                }
                else if (meter.DgnProtocol.ClassName == "CDLT698")
                {
                    string data = money.ToString("X8");

                    string outSID;
                    string outAttachData;
                    string outData;
                    string outMAC;

                    bool b = EncrypGW.Obj_Meter_Formal_GetPurseData(1, meter.EsamId, meter.SessionKey, "9", data, out outSID, out outAttachData, out outData, out outMAC);
                    if (b)
                    {
                        //两语句都有过成功试验
                        //string takeData = "07010DF1000A00020206" + outData + "5E" + outSID + "03" + outAttachData + "04" + outMAC + "01" + GetDateTimes(DateTime.Now) + "010005";
                        string takeData = "07010DF1000A00020206" + outData + "5E" + outSID + "03" + outAttachData + "04" + outMAC + "00";
                        string outSID2;
                        string outAttachData2;
                        string outData2;
                        string outMAC2;

                        b = EncrypGW.Obj_Meter_Formal_GetMeterSetData(3, meter.EsamId, meter.SessionKey, 3, takeData, out outSID2, out outAttachData2, out outData2, out outMAC2);

                        if (b)
                        {
                            StSIDMAC sidMac = new StSIDMAC
                            {
                                SID = outSID2,
                                AttachData = outAttachData2,
                                MAC = outMAC2,
                                Data = new Dictionary<string, List<string>>()
                            };
                            sidMac.Data.Add("F1000A00", new List<string>() { outData2 });

                            StPackParas packPara = new StPackParas
                            {
                                SidMac = sidMac,
                                MeterAddr = meter.Address,
                                OD = new List<string>() { "F1000A00" },
                                SecurityMode = EmSecurityMode.CiphertextMac
                            };

                            int errCode = 0;
                            List<object> objList = new List<object>();
                            if (Operation(pos, packPara, ref objList, ref errCode))
                            {
                                string OutData;
                                if (EncrypGW.Obj_Meter_Formal_VerifyMeterData(0, 3, meter.EsamId, meter.SessionKey, objList[0].ToString(), objList[1].ToString(), out OutData))
                                {
                                    if (OutData.Length > 14)
                                    {
                                        if (OutData.Substring(14, 2) == "00")
                                        {
                                            result = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            else
            {
                result = MeterProtocols[pos].ClearEnergy();
            }
            return result;
        }
        #endregion

        #region 电量清零
        /// <summary>
        /// 清空电量+
        /// </summary>
        /// <returns></returns>
        public bool[] ClearEnergy()
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (MulitThreadManager.Instance.MaxTaskCountPerThread > 2)
                    if (!Switch485Channel(pos)) return;
                if (!EquipmentData.Equipment.IsDemo)
                    arrRet[pos] = ClearEnergy(pos);
                else
                    arrRet[pos] = true;
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }


        /// <summary>
        /// 单表位电表清零
        /// </summary>
        /// <param name="pos">表位索引号，从0开始</param>
        /// <returns></returns>
        public bool ClearEnergy(int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!VerifyBase.meterInfo[pos].YaoJianYn) return true;

            bool r = false;
            //取密文 EncryptionTool
            if (!VerifyBase.meterInfo[pos].DgnProtocol.HaveProgrammingkey)
            {
                if (VerifyBase.meterInfo[pos].DgnProtocol.ClassName == "CDLT6452007")
                {
                    string outEndata = "";
                    string msg = "";

                    int keyStatus = 0;
                    if (VerifyBase.meterInfo[pos].EsamStatus == 1) //公钥
                        keyStatus = 1;


                    bool rst1 = EncrypGW.Meter_Formal_DataClear1(keyStatus, VerifyBase.meterInfo[pos].Rand, VerifyBase.meterInfo[pos].MD_MeterNo, "1A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"), ref outEndata, ref msg);
                    if (rst1 == true)
                    {
                        byte[] data = new byte[outEndata.Length / 2];
                        Array.Copy(StringToByte(outEndata.Substring(0, outEndata.Length)), 0, data, 0, data.Length);

                        string endata = BitConverter.ToString(data).Replace("-", "");
                        r = MeterProtocols[pos].ClearEnergy(endata);
                    }
                }
                else if (VerifyBase.meterInfo[pos].DgnProtocol.ClassName == "CDLT698")
                {
                    string PutcTaskData = "070101430003000001" + GetDateTimes(DateTime.Now) + "010005";
                    string strDataFlag = "43000300";

                    r = Operation(pos, 3, 3, PutcTaskData, strDataFlag, EmSecurityMode.CiphertextMac);
                }

            }
            else
            {
                r = MeterProtocols[pos].ClearEnergy();
            }
            return r;
        }

        /// <summary>
        /// 获取DateTimeS时间字符串，698
        /// </summary>
        /// <returns></returns>
        private string GetDateTimes(DateTime t)
        {
            string s = t.Year.ToString("X4");
            s += t.Month.ToString("X2");
            s += t.Day.ToString("X2");
            s += t.Hour.ToString("X2");
            s += t.Minute.ToString("X2");
            s += t.Second.ToString("X2");
            return s;
        }
        #endregion

        #region 读取电量
        //读取电量
        /// <summary>
        /// 读取电量
        /// </summary>
        /// <param name="energyType">电量功率类型0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <param name="tariffType">费率类型,当电量功率类型大于3时本参数无效</param>
        /// <returns>读取到的各表位电量</returns>
        public float[] ReadEnergy(byte energyType, byte tariffType)
        {
            float[] arrRet = new float[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (MulitThreadManager.Instance.MaxTaskCountPerThread > 2)
                    if (!Switch485Channel(pos)) return;
                arrRet[pos] = ReadEnergy(energyType, tariffType, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }
        /// <summary>
        /// 读取指定表位的电表电量
        /// </summary>
        /// <param name="energyType"></param>
        /// <param name="tariffType"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public float ReadEnergy(byte energyType, byte tariffType, int pos)
        {
            if (MeterProtocols[pos] == null) return 0F;
            if (!VerifyBase.meterInfo[pos].YaoJianYn) return 0F;
            return MeterProtocols[pos].ReadEnergy(energyType, tariffType);
        }
        #endregion

        #region 密钥
        /// <summary>
        /// 密钥更新
        /// </summary>
        /// <param name="pos">表位</param>
        /// <param name="key1">64位远程控制密钥密文，13:mac</param>
        /// <param name="keyinfo1">8位远程控制密钥信息,13:密文</param>
        /// <returns></returns>
        public bool UpdateKeyInfo(int pos, string key1, string keyinfo1)
        {
            byte[] cmd = new byte[] { 0x07, 0x02, 0x01, 0xFF };
            if (!VerifyBase.meterInfo[pos].DgnProtocol.HaveProgrammingkey)
            {
                cmd = new byte[] { 0x07, 0x03, 0x01, 0xFF };
            }
            byte[] keyinfo = new byte[4];
            return UpdateKeyInfo(pos, cmd, keyinfo, key1, keyinfo1);
        }
        /// <summary>
        /// 密钥更新通用函数
        /// </summary>
        /// <param name="pos">表位</param>
        /// <param name="bcmd">指令</param>
        /// <param name="str_Key">64位远程控制密钥密文,13:mac</param>
        /// <param name="str_Keyinfo">8位远程控制密钥信息,13:密文</param>
        /// <returns></returns>
        private bool UpdateKeyInfo(int pos, byte[] bcmd, byte[] keyinfo, string str_Key, string str_Keyinfo)
        {
            byte[] bKey1 = StringToByte(str_Key);
            byte[] bKeyinfo1 = StringToByte(str_Keyinfo);
            byte[] oper = new byte[4];
            Array.Reverse(bcmd);

            List<byte> frameData = new List<byte>();
            frameData.AddRange(bcmd);
            frameData.AddRange(oper);
            frameData.AddRange(bKeyinfo1);
            if (VerifyBase.meterInfo[pos].DgnProtocol.HaveProgrammingkey)
            {
                frameData.AddRange(keyinfo);
            }
            frameData.AddRange(bKey1);
            bool seqela = false;
            byte[] revdata = null;
            if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 更新密钥
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="isLocalMeter">远程表(否)/本地表(是)</param>
        /// <param name="key1">主控密钥密钥密文</param>
        /// <param name="keyinfo1">主控密钥信息</param>
        /// <param name="key2">远程控制密钥密文</param>
        /// <param name="keyinfo2">远程控制密钥信息</param>
        /// <param name="key3">二类参数设置密钥密文</param>
        /// <param name="keyinfo3">二类参数设置密钥信息</param>
        /// <param name="key4">远程身份认证密钥密文</param>
        /// <param name="keyinfo4">远程身份认证密钥信息</param>
        /// <returns></returns>
        public bool UpdataEncyKey(int pos, bool isLocalMeter, bool bUpdateKeyPublic, string[] keyinfo, string key1, string keyinfo1, string key2, string keyinfo2, string key3, string keyinfo3, string key4, string keyinfo4)
        {
            byte[] cmd;
            byte[] bKeyinfo;
            //假如不是本地表(是远程表)
            if (!isLocalMeter && bUpdateKeyPublic)
            {
                //远程主控密钥更新
                cmd = new byte[] { 0x07, 0x02, 0x04, 0xFF };
                bKeyinfo = StringToByte(keyinfo[0]);
                if (!UpdateKeyInfo(pos, cmd, bKeyinfo, key1, keyinfo1))
                {
                    return false;
                }
            }
            //if (App.UserSetting.EncryptionType != EncryType.融通加密机)
            if (DAL.Config.ConfigHelper.Instance.Dog_Type!= "融通加密机")
            {
                //控制命令密钥更新
                cmd = new byte[] { 0x07, 0x02, 0x01, 0xFF };
                bKeyinfo = StringToByte(keyinfo[1]);
                if (!UpdateKeyInfo(pos, cmd, bKeyinfo, key2, keyinfo2))
                {
                    return false;
                }
                //参数密钥更新
                cmd = new byte[] { 0x07, 0x02, 0x02, 0xFF };
                bKeyinfo = StringToByte(keyinfo[2]);
                if (!UpdateKeyInfo(pos, cmd, bKeyinfo, key3, keyinfo3))
                {
                    return false;
                }
            }
            else
            {
                //参数密钥更新
                cmd = new byte[] { 0x07, 0x02, 0x02, 0xFF };
                bKeyinfo = StringToByte(keyinfo[2]);
                if (!UpdateKeyInfo(pos, cmd, bKeyinfo, key3, keyinfo3))
                {
                    return false;
                }
                //控制命令密钥更新
                cmd = new byte[] { 0x07, 0x02, 0x01, 0xFF };
                bKeyinfo = StringToByte(keyinfo[1]);
                if (!UpdateKeyInfo(pos, cmd, bKeyinfo, key2, keyinfo2))
                {
                    return false;
                }
            }
            //远程身份认证更新
            cmd = new byte[] { 0x07, 0x02, 0x03, 0xFF };
            bKeyinfo = StringToByte(keyinfo[3]);
            if (!UpdateKeyInfo(pos, cmd, bKeyinfo, key4, keyinfo4))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 清空密钥信息
        /// </summary>
        /// <param name="pos">表位</param>
        /// <param name="key1">64位远程控制密钥密文，13:mac</param>
        /// <param name="keyinfo1">8位远程控制密钥信息,13:密文</param>
        /// <returns></returns>
        public bool ClearKeyInfo(int pos, string key1, string keyinfo1)
        {
            byte[] cmd = new byte[] { 0x07, 0x02, 0x01, 0xFF };
            if (!VerifyBase.meterInfo[pos].DgnProtocol.HaveProgrammingkey)
            {
                byte[] cmd3 = new byte[] { 0x07, 0x03, 0x01, 0xFF };
                cmd = cmd3;
            }
            byte[] keyinfo = new byte[4];
            return UpdateKeyInfo(pos, cmd, keyinfo, key1, keyinfo1);
        }


        /// <summary>
        /// 密钥下装645，09密钥
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="keyUpdateInfo"></param>
        /// <param name="keyType"></param>
        /// <returns></returns>
        public bool UpdataEncyKey645_09(int pos, StKeyUpdateInfo keyUpdateInfo, Cus_UpdateKeyType keyType)
        {
            byte[] cmd;
            byte[] bKeyinfo;

            switch (keyType)
            {
                case Cus_UpdateKeyType.主控密钥:
                    {
                        //本地表和私钥认证成功的表不需要再次更新主控密钥    zzg soinlove@126.com
                        if (!keyUpdateInfo.bLocalMeter && keyUpdateInfo.bUpdateKeyPublic)
                        {
                            //远程主控密钥更新
                            cmd = new byte[] { 0x07, 0x02, 0x04, 0xFF };
                            bKeyinfo = StringToByte(keyUpdateInfo.主控密钥明文);
                            if (!UpdateKeyInfo(pos, cmd, bKeyinfo, keyUpdateInfo.主控密钥密文, keyUpdateInfo.主控密钥信息))
                            {
                                return false;
                            }
                        }
                        break;
                    }
                case Cus_UpdateKeyType.远程密钥:
                    {
                        //控制命令密钥更新
                        cmd = new byte[] { 0x07, 0x02, 0x01, 0xFF };
                        bKeyinfo = StringToByte(keyUpdateInfo.远程密钥明文);
                        if (!UpdateKeyInfo(pos, cmd, bKeyinfo, keyUpdateInfo.远程密钥密文, keyUpdateInfo.远程密钥信息))
                        {
                            return false;
                        }
                        break;
                    }
                case Cus_UpdateKeyType.参数密钥:
                    {
                        //参数密钥更新
                        cmd = new byte[] { 0x07, 0x02, 0x02, 0xFF };
                        bKeyinfo = StringToByte(keyUpdateInfo.参数密钥明文);
                        if (!UpdateKeyInfo(pos, cmd, bKeyinfo, keyUpdateInfo.参数密钥密文, keyUpdateInfo.参数密钥信息))
                        {
                            return false;
                        }
                        break;
                    }
                case Cus_UpdateKeyType.身份密钥:
                    {
                        //远程身份认证更新
                        cmd = new byte[] { 0x07, 0x02, 0x03, 0xFF };
                        bKeyinfo = StringToByte(keyUpdateInfo.身份密钥明文);
                        if (!UpdateKeyInfo(pos, cmd, bKeyinfo, keyUpdateInfo.身份密钥密文, keyUpdateInfo.身份密钥信息))
                        {
                            return false;
                        }
                        break;
                    }
            }
            return true;
        }


        #endregion

        #region 远程控制
        /// <summary>
        ///   远程控置：跳合闸、报警、解除报警、保电、解除保电
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="str_Endata"></param>
        /// <returns></returns>
        public bool UserControl(int pos, string str_Endata)
        {
            byte[] oper = new byte[4];
            byte[] byt_Endata = StringToByte(str_Endata);
            List<byte> frameData = new List<byte>();
            frameData.AddRange(oper);
            frameData.AddRange(byt_Endata);

            bool seqela = false;
            byte[] revdata = null;

            if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x1C, frameData.ToArray(), ref seqela, ref revdata))
            {
                return true;
            }
            return false;
        }

        //远程密钥更新
        /// <summary>
        /// 安全认证
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="rand8">随机数</param>
        /// <param name="endata8">密文</param>
        /// <param name="div8">分散因子</param>
        /// <returns></returns>
        public bool IdentityAuthentication(int pos, string rand8, string endata8, string div8, out byte[] rand2, out byte[] esamNo)
        {
            rand2 = new byte[4];
            esamNo = new byte[8];
            if (MeterProtocols[pos] == null) return false;
            rand8 = rand8.PadLeft(16, '0');
            endata8 = endata8.PadLeft(16, '0');
            div8 = div8.PadLeft(16, '0');

            byte[] data = new byte[32];
            byte[] code = new byte[] { 0xFF, 0x00, 0x00, 0x07 };
            byte[] oper = new byte[4];
            byte[] Rand = StringToByte(rand8);
            byte[] Endata = StringToByte(endata8);
            byte[] Div = StringToByte(div8);
            Array.Copy(code, 0, data, 0, 4);
            Array.Copy(oper, 0, data, 4, 4);
            Array.Copy(Endata, 0, data, 8, Endata.Length);
            Array.Copy(Rand, 0, data, 16, Rand.Length);
            Array.Copy(Div, 0, data, 24, Div.Length);
            bool seqela = false;
            byte[] revdata = null;
            if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, data, ref seqela, ref revdata))
            {
                if (revdata.Length != 16) return false;
                Array.Copy(revdata, 4, rand2, 0, 4);
                Array.Copy(revdata, 8, esamNo, 0, 8);
                Array.Reverse(rand2); //随机数2
                Array.Reverse(esamNo);//ESAM 序列号
                return true;
            }
            return false;
        }

        /// <summary>
        /// 回抄本地/远程数据标识
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="keyinfo">密钥信息</param>
        /// <param name="bMac">Mac地址</param>
        /// <returns></returns>
        public bool ReadKeyReturnData(int pos, out StKeyInfo keyinfo, out byte[] bMac)
        {
            keyinfo = new StKeyInfo
            {
                EsamCoreInfo = ""
            };
            bMac = new byte[4];
            if (MeterProtocols[pos] == null) return false;
            List<byte> frameData = new List<byte>();
            byte[] cmd = new byte[] { 0xFF, 0x01, 0x80, 0x07 };
            byte[] oper = new byte[4];
            if (!VerifyBase.meterInfo[pos].DgnProtocol.HaveProgrammingkey)
            {
                byte[] rCode = new byte[] { 0x5A, 0x00, 0x00, 0x00, 0x1A, 0x00, 0x01, 0xDF };    //回抄数据标识
                frameData.AddRange(cmd);
                frameData.AddRange(oper);
                frameData.AddRange(rCode);
                bool seqela = false;
                byte[] revdata = null;
                if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                {
                    if (revdata.Length < 0x5E) return false;

                    byte[] kinfo = new byte[0x5A];
                    Array.Copy(revdata, 0xC, kinfo, 0, 0x5A);
                    Array.Copy(revdata, 0x66, bMac, 0, 4);
                    Array.Reverse(kinfo);
                    Array.Reverse(bMac);
                    keyinfo.EsamCoreInfo = BitConverter.ToString(kinfo).Replace("-", "");
                    return true;
                }
            }
            else
            {
                byte[] rCode = new byte[] { 0x04, 0x00, 0x00, 0x00, 0x06, 0x00, 0x01, 0xDF, };    //回抄数据标识
                frameData.AddRange(cmd);
                frameData.AddRange(oper);
                frameData.AddRange(rCode);
                bool seqela = false;
                byte[] revdata = null;
                if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, frameData.ToArray(), ref seqela, ref revdata))
                {
                    if (revdata.Length != 20) return false;

                    byte[] kinfo = new byte[4];
                    Array.Copy(revdata, 12, kinfo, 0, 4);
                    Array.Copy(revdata, 16, bMac, 0, 4);
                    Array.Reverse(kinfo);
                    Array.Reverse(bMac);
                    keyinfo.keyStatus = kinfo[0];
                    keyinfo.UpdateType = kinfo[1];
                    keyinfo.KeyCode = kinfo[2];
                    keyinfo.KeyVer = kinfo[3];
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 需量
        //清空需量+
        /// <summary>
        /// 清理需量
        /// </summary>
        /// <returns></returns>
        public bool[] ClearDemand()
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (MulitThreadManager.Instance.MaxTaskCountPerThread > 2)
                    if (!Switch485Channel(pos)) return;

                if (!EquipmentData.Equipment.IsDemo)
                    arrRet[pos] = ClearDemand(pos);
                else
                    arrRet[pos] = true;
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        /// <summary>
        /// 单表位清空需量
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool ClearDemand(int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!VerifyBase.meterInfo[pos].YaoJianYn) return true;
            bool ret = false;
            //取密文 EncryptionTool
            if (!VerifyBase.meterInfo[pos].DgnProtocol.HaveProgrammingkey)
            {

                if (VerifyBase.meterInfo[pos].DgnProtocol.ClassName == "CDLT6452007")
                {
                    string div = VerifyBase.meterInfo[pos].MD_MeterNo ;
                    string outEndata = "";
                    string msg = "";
                    bool rst3 = EncrypGW.Meter_Formal_DataClear2(0, VerifyBase.meterInfo[pos].Rand, div, "1900" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"), ref outEndata, ref msg);//
                    if (rst3 == true)
                    {
                        byte[] data = new byte[outEndata.Length / 2];
                        Array.Copy(StringToByte(outEndata.Substring(0, outEndata.Length)), 0, data, 0, data.Length);

                        string endata = BitConverter.ToString(data).Replace("-", "");
                        ret = MeterProtocols[pos].ClearDemand(endata);
                    }
                }
                else if (VerifyBase.meterInfo[pos].DgnProtocol.ClassName == "CDLT698")
                {
                    string PutcTaskData = "070101430006000001" + GetDateTimes(DateTime.Now) + "010005";
                    string strDataFlag = "43000600";

                    ret = Operation(pos, 3, 3, PutcTaskData, strDataFlag, EmSecurityMode.CiphertextMac);

                }

            }
            else
            {
                return MeterProtocols[pos].ClearDemand();
            }
            return ret;
        }

        //读取需量
        /// <summary>
        /// 读取需量
        /// </summary>
        /// <param name="energyType">功率类型</param>
        /// <param name="tariffType">费率类型</param>
        /// <returns>读取到的需量值</returns>
        public float[] ReadDemand(byte energyType, byte tariffType)
        {
            float[] arrRet = new float[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (MulitThreadManager.Instance.MaxTaskCountPerThread > 2)
                    if (!Switch485Channel(pos)) return;
                arrRet[pos] = ReadDemand(energyType, tariffType, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }
        /// <summary>
        /// 读取指定表位的需量
        /// </summary>
        /// <param name="energyType"></param>
        /// <param name="tariffType"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public float ReadDemand(byte energyType, byte tariffType, int pos)
        {
            if (MeterProtocols[pos] == null) return -1F;
            if (!VerifyBase.meterInfo[pos].YaoJianYn) return 0F;

            if (VerifyBase.meterInfo[pos].DgnProtocol.ClassName == "CDLT698")
            {
                string rand = "";
                string msg = "";
                if (EncrypGW.Create_Rand(16, ref rand, ref msg))
                {
                    VerifyBase.meterInfo[pos].Rand = rand;
                }

                string addr = "AAAAAAAAAAAA";
                if (VerifyBase.meterInfo[pos].Address.Length > 0)
                    addr = VerifyBase.meterInfo[pos].Address.PadLeft(12, '0');

                //需量
                List<string> LstOad = new List<string>
                {
                    "10" + energyType.ToString() + "00201"
                };

                StSIDMAC sidMac = new StSIDMAC
                {
                    Rand = rand
                };

                StPackParas DitPackParas = new StPackParas
                {
                    SecurityMode = EmSecurityMode.CiphertextMac,
                    OD = LstOad,
                    GetRequestMode = EmGetRequestMode.GetRequestNormal,
                    SidMac = sidMac,
                    MeterAddr = addr,
                };

                List<object> LstObj = new List<object>();
                int errCode = 0;
                MeterProtocols[pos].ReadData(DitPackParas, ref LstObj, ref errCode);
                float demand = -1;

                if (LstObj != null && LstObj.Count > 0)
                {
                    ////string dm = LstObj[0].ToString();
                    ////dm = dm.Insert(dm.Length - 4, ".");
                    demand = float.Parse(LstObj[0].ToString()) / 10000;
                }
                return demand;
            }
            else
                return MeterProtocols[pos].ReadDemand(energyType, tariffType);
        }
        #endregion
        #endregion

        #region 启用安全模式
        /// <summary>
        /// 01-启用，00-不启用
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool[] SecurityParameter(byte state)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!VerifyBase.meterInfo[pos].YaoJianYn) return;
                if (MulitThreadManager.Instance.MaxTaskCountPerThread > 2)
                    if (!Switch485Channel(pos)) return;
                if (!EquipmentData.Equipment.IsDemo)
                    arrRet[pos] = SecurityParameter(pos, state);
                else
                    arrRet[pos] = true;
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        /// <summary>
        /// 读取安全模式参数
        /// </summary>
        /// <returns></returns>
        public bool SecurityParameter(int pos, byte state)
        {
            return MeterProtocols[pos].SecurityParameter(state);
        }
        #endregion

        #region 读取数据
        //读取数据：指定ID
        /// <summary>
        /// 读取数据[645,698]
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string[] ReadData(string name)
        {
            string[] arrRet = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (MulitThreadManager.Instance.MaxTaskCountPerThread > 2)
                    if (!Switch485Channel(pos)) return;
                arrRet[pos] = ReadData1(name, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public string ReadData1(string name, int pos)
        {
            if (MeterProtocols[pos] == null) return string.Empty;
            if (!VerifyBase.meterInfo[pos].YaoJianYn) return string.Empty;
            MeterProtocalItem item = MeterProtocal.Select(name);
            if (item == null) return "";

            if (VerifyBase.meterInfo[pos].DgnProtocol.ClassName == "CDLT6452007")
                return ReadData645(item, pos);

            else if (VerifyBase.meterInfo[pos].DgnProtocol.ClassName == "CDLT698")
                return ReadData698(item, pos);

            return "";
        }
        private string ReadData698(MeterProtocalItem item, int pos)
        {
            //TODO2 上一次日冻结数据
            List<string> oads = new List<string>() { item.DataFlag698 };
            EmSecurityMode mode = (EmSecurityMode )item.Mode;

            if (item.Name == "(上1次)日冻结记录")
            {

            }

            object[] objs = ReadData(oads, mode, EmGetRequestMode.GetRequestNormal, pos);

            string value = "";
            for (int i = 0; i < objs.Length; i++)
            {
                object o = objs[i];
                string v = "";
                if (o == null)
                    continue;
                else if (item.Name == "当前套日时段表" || item.Name == "备用套日时段表" //第一套第1日时段数据,第一套第2日时段数据,第二套第1日时段数据,第二套第2日时段数据
                    || item.Name.IndexOf("日时段数据") > 0 || item.Name == "第一套时区表数据" || item.Name == "第二套时区表数据")
                { v = o.ToString().PadLeft(2, '0') + objs[i + 1].ToString().PadLeft(2, '0') + objs[i + 2].ToString().PadLeft(2, '0'); i += 2; }
                else if (item.Name == "每月第1结算日" || item.Name == "费率数" || item.Name == "日时段数" || item.Name == "年时区数" || item.Name == "日时段表数")
                    v = o.ToString().PadLeft(2, '0');
                else if (item.Name == "日期时间")
                    v = o.ToString();
                else if (item.Name == "协议版本号(ASCII码)")
                    v = Convert.ToInt32(o).ToString("X4");
                else if (item.Name.IndexOf("波特率特征字") > 0)
                {
                    //通信口1波特率特征字，通信口2波特率特征字，通信口3波特率特征字，
                    //TODO2 
                    //RS485  06 02 08 01 00:RS485 9600 偶校验 
                    //波特率 ENUMERATED
                    //  300bps（0），   600bps（1），     1200bps（2）， 2400bps（3），  4800bps（4），    7200bps（5），
                    //  9600bps（6），  19200bps（7），   38400bps（8），57600bps（9）， 115200bps（10）， 自适应（255），
                    //  校验位 ENUMERATED { 无校验（0），奇校验（1），偶校验（2）}，
                    //  数据位 ENUMERATED { 5（5），6（6），7（7），8（8）}，
                    //  停止位 ENUMERATED { 1（1），2（2）}，
                    //  流控 ENUMERATED { 无(0)，硬件(1)，软件(2)}
                    v = o.ToString() + " ";
                }
                else if (item.Name.IndexOf("冻结对象属性表") > 0)
                {
                    if (i == 0)
                    {
                        object o1 = objs[2];
                        v += string.Format("{0:D4},{1:D4},", o, o1);
                    }
                    if (i % 3 == 1)
                        v += string.Format("{0},", o);
                }
                else if (item.Name.StartsWith("自动循环显示第") || item.Name.StartsWith("按键循环显示第"))
                {
                    if (i == objs.Length - 1)
                        v = o.ToString().PadLeft(2, '0');
                    else
                        v = o.ToString() + ",";
                }
                else if (item.Name.StartsWith("第二套费率电价"))
                {
                    string s = o.ToString().PadLeft(8, '0');
                    //s = s.Insert(4, ".");
                    //v = Convert.ToSingle(s).ToString("f2") + ",";
                    v = s;
                }
                else if (item.Name.EndsWith("最大需量及发生时间数据块"))
                {//(当前)正向有功最大需量及发生时间数据块,(当前)反向有功总最大需量及发生时间数据块,(上{0}结算日)正向有功总最大需量及发生时间数据块
                    v = o.ToString() + ",";
                }
                else
                    v = o.ToString();

                int dot = item.Dot698; //小数点位值
                if (dot > 0)
                {
                    v = v.PadLeft(dot + 1, '0');
                    v = v.Insert(v.Length - dot, ".") + ",";
                }
                value += v;

                if (item.Name.EndsWith("失压总次数") || item.Name.EndsWith("欠压总次数") || item.Name.EndsWith("过压总次数") ||
                    item.Name.EndsWith("断相总次数") || item.Name.EndsWith("失流总次数") || item.Name.EndsWith("过流总次数") ||
                    item.Name.EndsWith("断流总次数") || item.Name.EndsWith("过载总次数"))
                    break;
            }

            if (item.Name.IndexOf("波特率特征字") > 0 && value.Trim().Length >= 10)
            {
                string s = value.Split(' ')[1];

                if (s.Length == 10)
                {
                    string ss = "";
                    switch (s.Substring(0, 2))
                    {
                        case "00":
                            ss = "300";
                            break;
                        case "01":
                            ss = "600";
                            break;
                        case "02":
                            ss = "1200";
                            break;
                        case "03":
                            ss = "2400";
                            break;
                        case "04":
                            ss = "4800";
                            break;
                        case "05":
                            ss = "7200";
                            break;
                        case "06":
                            ss = "9600";
                            break;
                        case "07":
                            ss = "19200";
                            break;
                        case "08":
                            ss = "38400";
                            break;
                        case "09":
                            ss = "57600";
                            break;
                        case "0A":
                            ss = "115200";
                            break;
                    }
                    switch (s.Substring(2, 2))
                    {
                        case "00":
                            ss += ",n";
                            break;
                        case "01":
                            ss += ",o";
                            break;
                        case "02":
                            ss += ",e";
                            break;
                    }
                    ss += "," + s.Substring(5, 1);
                    ss += "," + s.Substring(7, 1);

                    value = ss;
                }


            }
            else if (item.Name == "两套费率电价切换时间")
            {
                value = value.Substring(0, 12);
            }
            return value.Replace('\0', ' ').Trim().TrimEnd(',');
        }

        private string ReadData645(MeterProtocalItem item, int pos)
        {
            string data = MeterProtocols[pos].ReadData(item.DataFlag645, item.Length645);
            if (item.Name == "电压数据块" || item.Name == "电流数据块")// || name == "瞬时总有功功率" || name == "总功率因数"
            {
                string value = "";
                if (string.IsNullOrEmpty(data)) return "0.0,0.0,0.0";
                int dlen = data.Length / 3;
                int dot = item.Dot645; //小数点位值

                for (int i = 0; i < 3; i++)
                {
                    string v = data.Substring((2 - i) * dlen, dlen);
                    if (v.IndexOf("FFFF") < 0)
                        value += v.Insert(v.Length - dot, ".") + ",";
                }
                data = value.TrimEnd(',');
                return data;
            }
            else if (item.Name.IndexOf("自动循环显示第") >= 0)
            {
                if (data.Length == 10)
                    data = data.Substring(2);
            }
            else if (item.Name == "当前套日时段表" || item.Name == "备用套日时段表"
                    || item.Name == "第一套第1日时段数据" || item.Name == "第一套第2日时段数据" || item.Name == "第一套第3日时段数据"
                    || item.Name == "第二套第1日时段数据" || item.Name == "第二套第2日时段数据" || item.Name == "第二套第3日时段数据"
                    || item.Name == "第一套时区表数据" || item.Name == "第二套时区表数据")
            {
                string value = "";
                int len = data.Length / 6;
                for (int i = 0; i < len; i++)
                {
                    string v = data.Substring(i * 6, 6);
                    value = v + value;
                }
                data = value;

            }
            else if (item.Name == "(当前)组合有功电能数据块" || item.Name == "(当前)正向有功电能数据块" ||
                    item.Name == "(当前)反向有功总电能数据块" || item.Name == "(当前)组合无功1总电能数据块" ||
                    item.Name == "(当前)组合无功2总电能数据块")
            {
                string value = "";
                int len = data.Length / 8;
                for (int i = 0; i < len; i++)
                {
                    string v = data.Substring(i * 8, 8);
                    v = v.Insert(6, ".") + ",";
                    value = v + value;
                }
                data = value;
            }
            else if (item.Name == "第二套费率电价" || item.Name == "第二套阶梯电价" || item.Name == "第二套费率电价" || item.Name == "第二套阶梯电价")
            {
                string value = "";
                int len = data.Length / 8;
                for (int i = 0; i < len; i++)
                {
                    string v = data.Substring(i * 8, 8);
                    value = (Convert.ToSingle(v) / 10000).ToString() + "," + value;
                }

                data = value.TrimEnd(',');
            }

            if (data.Length > item.Dot645 && item.Dot645 > 0)
                data = data.Insert(data.Length - item.Dot645, ".");

            return data.TrimEnd(',');
        }

        /// <summary>
        /// 读取数据（数据型，数据项）
        /// </summary>
        /// <param name="sendData">标识符,2个字节</param>
        /// <returns>返回数据</returns>
        public string[] ReadData(string[] sendData)
        {
            string[] arrRet = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (MulitThreadManager.Instance.MaxTaskCountPerThread > 2)
                    if (!Switch485Channel(pos)) return;
                arrRet[pos] = ReadDatas(sendData[pos], pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public string ReadDatas(string dataFlag, int pos)
        {
            if (MeterProtocols[pos] == null) return "0F";
            if (!VerifyBase.meterInfo[pos].YaoJianYn) return "0F";
            MeterProtocalItem item = MeterProtocal.Select(dataFlag);
            if (item == null) return "";

            return MeterProtocols[pos].ReadData(dataFlag, item.Length645);
        }

        /// <summary>
        /// 载波读取HPLC芯片ID,485读取不了
        /// </summary>
        /// <param name="carrProtocal">模块厂家名称</param>
        /// <param name="pos">表位号</param>
        /// <returns></returns>
        public Dictionary<string, string> ReadHPLCID()
        {
            int pos = VerifyBase.FirstIndex;
            if (pos == -1) return new Dictionary<string, string>();

            return MeterProtocols[pos].ReadHplcID();
        }

        /// <summary>
        /// 读取数据（字符型，数据项）    //TODO2这里可能需要修改
        /// </summary>
        /// <param name="dataFlag">标识符,2个字节</param>
        /// <param name="len">数据长度(字节数)</param>
        /// <returns></returns>
        public string[] ReadLoadRecord(string dataFlag, int len, string item)
        {
            string[] arrRet = new string[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (MulitThreadManager.Instance.MaxTaskCountPerThread > 2)
                    if (!Switch485Channel(pos)) return;
                arrRet[pos] = ReadLoadRecordByPos(dataFlag, len, item, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public string ReadLoadRecordByPos(string dataFlag, int len, string item, int pos)
        {
            if (MeterProtocols[pos] == null) return string.Empty;
            if (!VerifyBase.meterInfo[pos].YaoJianYn) return string.Empty;
            return MeterProtocols[pos].ReadData(dataFlag, len, item);
        }

        /// <summary>
        /// 读取记录数据 DLT698
        /// </summary>
        /// <param name="oad"></param>
        /// <param name="mode"></param>
        /// <param name="GetRequestMode"></param>
        /// <param name="recordNo"></param>
        /// <param name="DicObj"></param>
        /// <returns></returns>
        public Dictionary<int, Dictionary<string, List<object>>> ReadRecordData(List<string> oad, EmSecurityMode mode, EmGetRequestMode GetRequestMode, int recordNo, List<string> rcsd, ref Dictionary<int, List<object>> DicObj)
        {
            Dictionary<int, Dictionary<string, List<object>>> arrRet = new Dictionary<int, Dictionary<string, List<object>>>();
            Dictionary<int, List<object>> arrObj = new Dictionary<int, List<object>>();
            List<object> LstObj = new List<object>();
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!Switch485Channel(pos)) return;

                arrRet.Add(pos, ReadRecordData(oad, mode, GetRequestMode, recordNo, rcsd, ref LstObj, pos));
                arrObj.Add(pos, LstObj);
            };
            MulitThreadManager.Instance.Start();
            WaitWorkDone();
            DicObj = arrObj;
            return arrRet;
        }

        private Dictionary<string, List<object>> ReadRecordData(List<string> LstOad, EmSecurityMode mode, EmGetRequestMode GetRequestMode, int recordNo, List<string> rcsd, ref List<object> LstObj, int pos)
        {
            int errCode = 0;
            if (MeterProtocols[pos] == null || !VerifyBase.meterInfo[pos].YaoJianYn)
                return new Dictionary<string, List<object>>();

            StPackParas DataInfo = new StPackParas
            {
                MeterAddr = VerifyBase.meterInfo[pos].Address.Length <= 0 ? "AAAAAAAAAAAA" : VerifyBase.meterInfo[pos].Address.PadLeft(12, '0'),
                SecurityMode = mode,
                OD = LstOad,
                SidMac = new StSIDMAC(),
                GetRequestMode = GetRequestMode,

            };
            if (mode == EmSecurityMode.ClearTextRand)
            {
                string OutRand1 = "";
                string msg = "";
                if (EncrypGW.Create_Rand(16, ref OutRand1, ref msg))
                    DataInfo.SidMac = new StSIDMAC()
                    {
                        Rand = OutRand1
                    };
            }

            Dictionary<string, List<object>> csdList = new Dictionary<string, List<object>>();
            MeterProtocols[pos].ReadRecordData(DataInfo, ref LstObj, ref csdList, recordNo, rcsd, ref errCode);
            if (LstObj == null)
                return new Dictionary<string, List<object>>();
            return csdList;
        }


        /// <summary>
        /// 读取数据    698
        /// </summary>
        /// <param name="oad">对象操作符</param>
        /// <param name="mode">参数安全模式</param>
        /// <param name="LstObj">返回内容列表</param>
        private object[] ReadData(List<string> oadList, EmSecurityMode mode, EmGetRequestMode GetRequestMode, int pos)
        {
            int errCode = 0;
            List<object> objList = new List<object>();
            if (MeterProtocols[pos] == null) return new object[0];
            if (!VerifyBase.meterInfo[pos].YaoJianYn) return new object[0];

            StPackParas para = new StPackParas
            {
                SecurityMode = mode,
                OD = oadList,
                SidMac = new StSIDMAC(),
                GetRequestMode = GetRequestMode
            };

            if (VerifyBase.meterInfo[pos].Address.Length > 0)
                para.MeterAddr = VerifyBase.meterInfo[pos].Address.PadLeft(12, '0');
            else
                para.MeterAddr = "AAAAAAAAAAAA";

            if (mode == EmSecurityMode.ClearTextRand) //明文 + 随机数
            {
                string rand = "";
                string msg = "";

                if (EncrypGW.Create_Rand(16, ref rand, ref msg)) //生成随机数
                {
                    StSIDMAC sidMac = new StSIDMAC
                    {
                        Rand = rand
                    };
                    para.SidMac = sidMac;
                }
            }

            MeterProtocols[pos].ReadData(para, ref objList, ref errCode);
            return objList.ToArray();
        }

        /// <summary>
        /// 读取数据    698
        /// </summary>
        /// <param name="oad">对象操作符</param>
        /// <param name="mode">参数安全模式</param>
        public Dictionary<int, object[]> ReadData(List<string> oad, EmSecurityMode mode, EmGetRequestMode GetRequestMode)
        {

            Dictionary<int, object[]> arrRet = new Dictionary<int, object[]>();
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!Switch485Channel(pos)) return;
                arrRet.Add(pos, ReadData(oad, mode, GetRequestMode, pos));
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }
        /// <summary>
        /// 读取数据    698
        /// </summary>
        /// <param name="oad">对象操作符</param>
        /// <param name="mode">参数安全模式</param>
        public bool[] AppConnection()
        {

            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (!Switch485Channel(pos)) return;
                arrRet[pos] = AppConnection(pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        /// <summary>
        /// 应用连接请求
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>帧长度</returns>
        public bool AppConnection(int pos)
        {

            int errCode = 0;
            List<object> LstObj = new List<object>();
            if (MeterProtocols[pos] == null) return false;
            if (!VerifyBase.meterInfo[pos].YaoJianYn) return false;

            int keyState = VerifyBase.meterInfo[pos].EsamStatus;
            string putDiv;
            if (keyState == 0)
                putDiv = VerifyBase.meterInfo[pos].EsamId.PadLeft(16, '0');
            else// if (keyState == 1)
                putDiv = VerifyBase.meterInfo[pos].MD_MeterNo.PadLeft(16, '0');

            string sessionNo = VerifyBase.meterInfo[pos].SessionNo;

            string outSessionKey = "";
            string outSessionInit;
            string outSign;
            string outRandHost;
            bool rst1 = EncrypGW.Obj_Meter_Formal_InitSession(keyState, putDiv, sessionNo, "01", out outRandHost, out outSessionInit, out outSign);
            VerifyBase.meterInfo[pos].SessionKey = "";
            if (rst1)
            {
                StSignatureSecurity SingnInfo = new StSignatureSecurity()
                {
                    SessionData = outSessionInit,
                    MAC = outSign,
                };

                StConnectMechanismInfo m = new StConnectMechanismInfo()
                {
                    ConnectInfo = EmConnectMechanismInfo.SymmetrySecurity,
                    SessionData = SingnInfo,
                };

                MeterProtocols[pos].AppConnection(m, ref LstObj, ref errCode);

                if (LstObj.Count > 1)
                    rst1 = EncrypGW.Obj_Meter_Formal_VerifySession(keyState, putDiv, outRandHost, LstObj[0].ToString(), LstObj[1].ToString(), out outSessionKey);

                VerifyBase.meterInfo[pos].SessionKey = outSessionKey;
            }
            else
            {
                //VerifyBase.(false, "【获取应用连接密文失败，请检查】...");
            }
            return rst1;
        }
        #endregion

        #region 其他
        /// <summary>
        /// 操作 698
        /// </summary>        
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool Operation(int pos, int iOperateMode, int taskType, string taskData, string dataFlag, EmSecurityMode securityMode)
        {
            bool ret = false;
            TestMeterInfo meter =VerifyBase.meterInfo[pos];

            string esamID = meter.EsamId.PadLeft(16, '0');
            string sessionKey = meter.SessionKey;

            string outSID;
            string outAttachData;
            string outData;
            string outMAC;
            bool rst1 = EncrypGW.Obj_Meter_Formal_GetSessionData(iOperateMode, esamID, sessionKey, taskType, taskData, out outSID, out outAttachData, out outData, out outMAC);
            if (rst1)
            {
                List<string> da = new List<string>();
                if (securityMode == EmSecurityMode.CiphertextMac)
                    da.Add(outData);
                else if (securityMode == EmSecurityMode.ClearTextMac)
                    da.Add(taskData);

                StSIDMAC sidMac = new StSIDMAC
                {
                    SID = outSID,
                    AttachData = outAttachData,
                    MAC = outMAC,
                    Data = new Dictionary<string, List<string>> { { dataFlag, da } }
                };

                StPackParas packPara = new StPackParas
                {
                    SidMac = sidMac,
                    MeterAddr = meter.Address,
                    OD = new List<string>() { dataFlag },
                    SecurityMode = securityMode,
                };

                int errCode = 0;
                List<object> LstObj = new List<object>();
                bool rst2 = MeterProtocols[pos].Operation(packPara, ref LstObj, ref errCode);

                if (rst2)
                {
                    string OutData;
                    bool rst3 = EncrypGW.Obj_Meter_Formal_VerifyMeterData(meter.EsamStatus, iOperateMode, esamID, sessionKey, LstObj[0].ToString(), LstObj[1].ToString(), out OutData);

                    if (rst3)
                    {
                        if (OutData.Length > 16)
                        {
                            if (OutData.Substring(14, 2) == "00")
                            {
                                ret = true;
                            }
                        }
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// 操作组帧
        /// </summary>        
        /// <param name="DataInfo">参数信息</param>
        /// <returns>帧长度</returns>
        public bool OperationSubFrame(int pos, StPackParas DataInfo, ref List<object> LstObj, ref int intErrCode)
        {
            return MeterProtocols[pos].OperationSubFrame(DataInfo, ref LstObj, ref intErrCode);
        }


        /// <summary>
        /// 设置脉冲端子
        /// </summary>
        /// <param name="pulseType"></param>
        /// <returns></returns>
        public bool[] SetPulseCom(byte pulseType)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                if (MulitThreadManager.Instance.MaxTaskCountPerThread > 2)
                    if (!Switch485Channel(pos)) return;
                arrRet[pos] = SetPulseCom(pulseType, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public bool SetPulseCom(byte pulseType, int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!VerifyBase.meterInfo[pos].YaoJianYn ) return true;
            return MeterProtocols[pos].SetPulseCom(pulseType);
        }
        #endregion

        #region 写入数据
        /// <summary>
        /// 写入数据，如写入值每表位不相同时
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool[] WriteData(string name, string[] data)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                arrRet[pos] = WriteData(name, data[pos], pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }

        public bool[] WriteData(string name, string data)
        {
            bool[] arrRet = new bool[bwCount];
            runFlag = true;
            MulitThreadManager.Instance.DoWork = delegate (int pos)
            {
                if (!runFlag) return;
                arrRet[pos] = WriteData(name, data, pos);
            };
            MulitThreadManager.Instance.Start();
            //等所有返回
            WaitWorkDone();
            return arrRet;
        }
        private bool WriteData(string name, string data, int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            TestMeterInfo meter = VerifyBase.meterInfo[pos];
            if (!meter.YaoJianYn) return true;

            MeterProtocalItem item = MeterProtocal.Select(name);
            if (item == null) return false;

           // EquipmentData.(false, "写入[" + name + "]:" + data);


            if (meter.DgnProtocol.ClassName == "CDLT6452007")
            {
                return WriteData645(meter, item, data, pos);
            }
            else if (meter.DgnProtocol.ClassName == "CDLT698")
            {
                return WriteData698(meter, item, name, data, pos);
            }
            return false;
        }
        /// <summary>
        /// 645 写入数据
        /// </summary>
        /// <param name="meter">电能表信息</param>
        /// <param name="protocal"></param>
        /// <param name="data">写入值</param>
        /// <param name="pos">表位号</param>
        /// <returns></returns>
        private bool WriteData645(TestMeterInfo meter, MeterProtocalItem protocal, string data, int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!meter.YaoJianYn) return true;
            bool blnWriteType = false;

            //if (App.MethodAndBasic.IsNeiMengWarningPrice)//内蒙
            //{
            //    if ("报警金额1限值" == protocal.Name || "报警金额1限值" == protocal.Name)
            //        blnWriteType = true;
            //}

            bool bln_Rst = false;
            if (!meter.DgnProtocol.HaveProgrammingkey || blnWriteType == true)
            {
                //身份认证
                //判断参数更新文件
                //处理数据明文
                //取得密文
                //组帧发送电表
                if (protocal.DataFlag645 == "04001301")//三类数据
                {
                    bln_Rst = MeterProtocols[pos].WriteData(protocal.DataFlag645, protocal.Length645, data);
                }
                //一类参数
                else if (protocal.Name == "两套费率电价切换时间" || protocal.Name == "两套阶梯切换时间" || protocal.Name == "电流互感器变比" || protocal.Name == "电压互感器变比" ||
                   protocal.Name == "表号" || protocal.Name == "客户编号" || protocal.Name == "报警金额1限值" || protocal.Name == "报警金额2限值" ||
                   protocal.Name.Contains("第一套费率电价") || protocal.Name.Contains("第二套费率电价") || protocal.Name == "第一套阶梯参数数据块" || protocal.Name == "第二套阶梯参数数据块")
                {
                    string outMac = "";
                    string msg = "";

                    data = data.PadLeft(protocal.Length645 * 2, '0');

                    string Apdu;
                    if ("报警金额1限值" == protocal.Name)//报警金额1
                        Apdu = "04d6821008";
                    else if ("报警金额2限值" == protocal.Name)//报警金额2
                        Apdu = "04d6821408";
                    else if ("两套费率电价切换时间" == protocal.Name)//两套费率电价切换时间
                    {
                        Apdu = "04d6820A09";
                        if (data.Length >= 12) //698的格式为YYYYMMDDHHmmSS,645格式为YYMMDDHHmm
                            data = data.Substring(2, 10);
                    }

                    else if ("两套阶梯电价切换时间" == protocal.Name)//两套阶梯电价切换时间
                    {
                        Apdu = "04D684C409";
                        if (data.Length >= 12)//698的格式为YYYYMMDDHHmmSS,645格式为YYMMDDHHmm
                            data = data.Substring(2, 10);
                    }
                    else if (protocal.Name.StartsWith("第二套费率电价"))
                        Apdu = "04d68404" + (data.Length / 2 + 4).ToString("X2").PadLeft(2, '0'); //
                    else if (protocal.Name.StartsWith("第二套阶梯电价"))
                        Apdu = "04d6849C" + (data.Length / 2 + 4).ToString("X2").PadLeft(2, '0'); //
                    else if (protocal.Name == "表号")
                        Apdu = "04d682" + "1E" + (data.Length / 2 + 4).ToString("X2");
                    else
                        Apdu = "04d682" + "0A" + (data.Length / 2 + 4).ToString().PadLeft(2, '0');

                    bool rst3;

                    if ("04000109" == protocal.DataFlag645)//两套阶梯电价切换时间
                        rst3 = EncrypGW.ParameterUpDate2(0,  VerifyBase.meterInfo[pos].Rand, VerifyBase.meterInfo[pos].MD_MeterNo, Apdu, data, ref outMac, ref msg);
                    else if (protocal.DataFlag645.StartsWith("040501") || protocal.DataFlag645.StartsWith("040601")) //第一套费率电价，或第一套阶梯电价
                        rst3 = EncrypGW.ParameterUpDate1(0, VerifyBase.meterInfo[pos].Rand, VerifyBase.meterInfo[pos].MD_MeterNo, Apdu, data, ref outMac, ref msg);
                    else if (protocal.DataFlag645.StartsWith("040502") || protocal.DataFlag645.StartsWith("040602")) //第二套费率电价，或第二套阶梯电价
                        rst3 = EncrypGW.ParameterUpDate2(0, VerifyBase.meterInfo[pos].Rand ,VerifyBase.meterInfo[pos].MD_MeterNo, Apdu, data, ref outMac, ref msg);

                    else
                        rst3 = EncrypGW.ParameterUpDate(0, VerifyBase.meterInfo[pos].Rand, VerifyBase.meterInfo[pos].MD_MeterNo, Apdu, data, ref outMac, ref msg);

                    if (rst3 == true)
                    {
                        byte[] bdata = new byte[outMac.Length / 2];

                        Array.Copy(StringToByte(outMac.Substring(0, outMac.Length - 8)), 0, bdata, 0, bdata.Length - 4);
                        Array.Copy(StringToByte(outMac.Substring(outMac.Length - 8, 8)), 0, bdata, bdata.Length - 4, 4);
                        bln_Rst = MeterProtocols[pos].WriteData(protocal.DataFlag645, bdata);
                    }
                }
                else //二类数据
                {
                    string outEndata = "";
                    string msg = "";

                    string[] strEdy = new string[] { "89", "90", "91", "92", "93" };

                    if (protocal.Name == "资产编号")
                    {
                        string str = "";
                        for (int i = 0; i < data.Length; i++)
                        {
                            str += "3" + data[i];
                        }

                        str = str.PadRight(64, '0');

                        data = str;
                    }
                    else if (protocal.Name == "两套时区表切换时间" || protocal.Name == "两套日时段表切换时间")
                    {
                        if (data.Length >= 14)//698的格式为YYYYMMDDHHmmss,645格式为YYMMDDHHmm
                            data = data.Substring(2, 10);
                    }

                    string strPutApdu = "04d6" + strEdy[Convert.ToInt32("0x" + protocal.DataFlag645.Substring(2, 2), 16) % 5] + "00" + (data.Length / 2 + 4).ToString().PadLeft(2, '0');

                    if (protocal.DataFlag645 == "070001FF")
                        strPutApdu = "04d6822b06";

                    bool rst3 = EncrypGW.ParameterElseUpDate(0, meter.Rand, meter.EsamId, meter.MD_MeterNo, strPutApdu, protocal.DataFlag645 + data, ref outEndata, ref msg);
                    if (rst3 == true)
                    {
                        if (protocal.DataFlag645 == "070001FF")
                        {
                            byte[] data1 = new byte[28];
                            byte[] code = new byte[] { 0xFF, 0x01, 0x00, 0x07 };
                            byte[] oper = new byte[4];

                            Array.Copy(code, 0, data1, 0, 4);
                            Array.Copy(oper, 0, data1, 4, 4);
                            Array.Copy(StringToByte(outEndata.Substring(0, outEndata.Length - 8)), 0, data1, 8, outEndata.Length / 2 - 4);
                            Array.Copy(StringToByte(outEndata.Substring(outEndata.Length - 8, 8)), 0, data1, 8 + outEndata.Length / 2 - 4, 4);
                            bool seqela = false;
                            byte[] revdata = null;
                            if (MeterProtocols[pos].UpdateRemoteEncryptionCommand(0x03, data1, ref seqela, ref revdata))
                            {
                                if (revdata.Length != 16) return false;

                                byte[] r2 = new byte[4];
                                byte[] eNo = new byte[8];
                                Array.Copy(revdata, 4, r2, 0, 4);
                                Array.Copy(revdata, 8, eNo, 0, 8);

                                return true;
                            }
                        }
                        else
                        {
                            byte[] dtmp = new byte[outEndata.Length / 2];
                            Array.Copy(StringToByte(outEndata.Substring(0, outEndata.Length - 8)), 0, dtmp, 0, dtmp.Length - 4);
                            Array.Copy(StringToByte(outEndata.Substring(outEndata.Length - 8, 8)), 0, dtmp, dtmp.Length - 4, 4);
                            bln_Rst = MeterProtocols[pos].WriteData(protocal.DataFlag645, dtmp);//outEndata.Length / 2, outEndata);
                        }
                    }
                    //}
                }

            }
            else
            {
                bln_Rst = MeterProtocols[pos].WriteData(protocal.DataFlag645, protocal.Length645, data);
            }
            return bln_Rst;
        }


        private bool WriteData698(TestMeterInfo meter, MeterProtocalItem protocal, string name, string data, int pos)
        {
            if (MeterProtocols[pos] == null) return false;
            if (!meter.YaoJianYn) return true;

            string taskData = "";
            int taskType = 3;
            int keyState = 1;

            //二类参数
            if (name == "备用套日时段表" || name == "第二套第1日时段数据" || name == "第二套第2日时段数据" || name == "第二套时区表数据")  //data格式：HHMMNN
            {

                //第二套第1日时段数据

                //第二套第1日时段数据
                int len = data.Length / 6;
                for (int i = 0; i < len; i++)
                {
                    //第二套第1日时段数据
                    string tmp = data.Substring(i * 6, 6);
                    tmp = "020311" + Convert.ToInt16(tmp.Substring(0, 2)).ToString("X2") + "11" + Convert.ToInt16(tmp.Substring(2, 2)).ToString("X2") + "11" + Convert.ToInt16(tmp.Substring(4, 2)).ToString("X2");
                    taskData += tmp;
                }
                taskData = "060109" + protocal.DataFlag698 + "01" + len.ToString("X2") + taskData + "00";
                taskType = 5;

            }
            else if (name == "费率数" || name == "日时段数" || name == "最大需量周期" || name == "日时段表数" || name == "年时区数")      //data格式：NN
            {
                taskData = "060101" + protocal.DataFlag698 + "11" + Convert.ToByte(data).ToString("X2") + "00";
            }
            else if (name == "日期时间")    //data格式:yyyyMMddhhmmss
            {//40000200
                //日期时间
                string yy = Convert.ToInt16(data.Substring(0, 4)).ToString("X4");
                string mm = Convert.ToInt16(data.Substring(4, 2)).ToString("X2");
                string dd = Convert.ToInt16(data.Substring(6, 2)).ToString("X2");
                string hh = Convert.ToInt16(data.Substring(8, 2)).ToString("X2");
                string mi = Convert.ToInt16(data.Substring(10, 2)).ToString("X2");
                string ss = Convert.ToInt16(data.Substring(12, 2)).ToString("X2");
                taskData = "060101" + protocal.DataFlag698 + "1C" + yy + mm + dd + hh + mi + ss + "00";

            }
            else if (name == "两套时区表切换时间" || name == "两套日时段表切换时间")
            {//40080200,40090200 ,data格式:yyyyMMddhhmmss

                data = data.PadRight(14, '0');
                string yy = Convert.ToInt16(data.Substring(0, 4)).ToString("X4");
                string mm = Convert.ToInt16(data.Substring(4, 2)).ToString("X2");
                string dd = Convert.ToInt16(data.Substring(6, 2)).ToString("X2");
                string hh = Convert.ToInt16(data.Substring(8, 2)).ToString("X2");
                string mi = Convert.ToInt16(data.Substring(10, 2)).ToString("X2");
                string ss = Convert.ToInt16(data.Substring(12, 2)).ToString("X2");

                taskData = "060101" + protocal.DataFlag698 + "1C" + yy + mm + dd + hh + mi + ss + "00";
                taskType = 5;

            }
            else if (name == "通信地址")
            {//40010200
                taskData = "060101" + protocal.DataFlag698 + "0906" + data.PadLeft(12, '0') + "00";
            }
            else if (name.IndexOf("自动循环显示第") == 0 || name.IndexOf("按键循环显示第") == 0)
            { //格式：数据标识1,序号
                string[] arr = data.Split(',');
                if (arr.Length == 2)
                    taskData = string.Format("060101{0}02025B00{1}11{2}00", protocal.DataFlag698, arr[0], arr[1]);
                else //arr.Length==3 ROAD 格式：数据标识1,数据标识2,序号
                    taskData = string.Format("060101{0}02025B01{1}01{2}11{3}00", protocal.DataFlag698, arr[0], arr[1], arr[2]);
            }
            else if (name == "自动循环显示屏数" || name == "按键显示屏数")
            {
                taskData = "060101" + protocal.DataFlag698 + "11" + Convert.ToInt16(data).ToString("X2") + "00";
            }
            else if (name == "自动轮显每屏显示时间")
            {
                taskData = "060101" + protocal.DataFlag698 + "12" + Convert.ToInt16(data).ToString("X4") + "00";
            }
            else if (name == "每月第1结算日" || name == "每月第2结算日" || name == "每月第3结算日")
            {
                string dd = Convert.ToByte(data.Substring(0, 2)).ToString("X2");
                string hh = Convert.ToByte(data.Substring(2, 2)).ToString("X2");

                taskData = "060101" + protocal.DataFlag698 + "020211" + dd + "11" + hh + "00";
            }


            //一类参数
            else if (name == "报警金额" || name == "报警金额1限值") //401E0200
            {
                string[] ps = data.Split(',');
                foreach (string p in ps)
                {
                    string tmp = Convert.ToUInt32(Convert.ToSingle(p) * 100).ToString("D8");
                    taskData += tmp;
                }

                taskType = 5;
            }
            else if (name == "第二套费率电价" || name == "第一套费率电价" || name == "第二套费率电价1") //备用套 
            {

                //第二套费率电价1

                //第二套费率电价1
                string[] ps = data.Split(',');
                foreach (string p in ps)
                {
                    //第二套费率电价1
                    //输入格式：0.33,0.34,0.36,0.37
                    //40190200
                    string tmp = Convert.ToUInt32(Convert.ToDouble(p) * 10000).ToString("D8");
                    taskData += tmp;
                }
                taskType = 5;

            }
            else if (name == "第二套阶梯电价")
            {
                //输入格式：XXXXXXXX,XXXXXXXX,MMDDHH分别为阶梯值数组，电价数组，结算日,中间无逗号
                //40 1B 02 00 40 00 00 11 10 00 00 11 10 00 00 11 10 00 00 11 10 00 00 11 10 00 00 11 10 00 00 00 20 00 00 00 20 00 00 00 20 00 00 00 20 00 00 00 20 00 00 00 20 00 00 00 20 01 01 00 01 01 00 01 01 00 01 01 00
                //401B0200

                taskData = data;
                taskType = 5;
            }
            else if (name == "表号") //40020200
            {
                taskData = data.PadLeft(16, '0');
                taskType = 3;
            }
            else if (name == "客户编号")
            {
                taskData = data.PadLeft(12, '0');
                taskType = 3;
            }

            else if (name == "两套阶梯电价切换时间")//data格式:yyyyMMddhhmm
            {//400B0200
                taskData = data.Substring(2, 10);
                taskType = 3;
                keyState = 0;
            }
            else if (name == "两套费率电价切换时间")//data格式:yyyyMMddhhmm
            {//400A0200
                taskData = data.Substring(data.Length - 10, 10);
                taskType = 3;
                keyState = 0;
            }


            if (string.IsNullOrEmpty(taskData)) return false;

            if (name == "第二套费率电价" || name == "表号" || name == "客户编号" ||
                name == "两套阶梯电价切换时间" || name == "两套费率电价切换时间" || name == "报警金额" ||
                name == "第二套阶梯电价") //一类参数
                return Operation1(pos, keyState, 3, taskType, taskData, protocal.DataFlag698, EmSecurityMode.CiphertextMac);
            else//二类数据
                return Operation(pos, 3, taskType, taskData, protocal.DataFlag698, EmSecurityMode.CiphertextMac);

        }

        /// <summary>
        /// 698,1类参数写入
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool Operation1(int pos, int iKeyState, int iOperateMode, int taskType, string taskData, string dataFlag, EmSecurityMode securityMode)
        {
            if (MeterProtocols[pos] == null) return false;
            TestMeterInfo meter =VerifyBase.meterInfo[pos];
            if (!meter.YaoJianYn) return true;

            string meterNo = meter.MD_MeterNo.PadLeft(16, '0');
            string esamRand = "00".PadLeft(16, '0');

            string outSID;
            string outAttachData;
            string outData;
            string outMAC;

            string cData = dataFlag + (taskData.Length / 2).ToString("X2") + taskData;//4ByteOAD+1Byte内容Len+ 内容

            if (false == EncrypGW.Obj_Meter_Formal_SetESAMData(iKeyState, 1, meter.EsamId, meter.SessionKey, meterNo, esamRand, cData, out outSID, out outAttachData, out outData, out outMAC)) return false;

            taskData = "070108F1000400020209";
            taskData += (cData.Length / 2).ToString("X2") + cData;
            taskData += "5E" + outSID;
            taskData += (outAttachData.Length / 2).ToString("X2") + outAttachData;
            taskData += (outMAC.Length / 2).ToString("X2") + outMAC;
            taskData += "00";

            return Operation(pos, iOperateMode, taskType, taskData, dataFlag, securityMode);
        }

        #endregion

        #region 组帧
        /// <summary>
        /// 操作组帧
        /// </summary>        
        /// <param name="DataInfo">参数信息</param>
        /// <returns>帧长度</returns>
        public bool Operation(int pos, StPackParas DataInfo, ref List<object> LstObj, ref int intErrCode)
        {
            return MeterProtocols[pos].Operation(DataInfo, ref LstObj, ref intErrCode);
        }
        #endregion

        #region MyRegion
        public void Init2041(int bwIndex)
        {
            //恢复出厂设置
            lY3762 .PacketToCarrierInit(2, bwIndex);
            Thread.Sleep(10000);
            //数据区初始化
            lY3762.PacketToCarrierInit(3, bwIndex);
            Thread.Sleep(10000);
            //硬件区初始化
            lY3762.PacketToCarrierInit(1, bwIndex);
            Thread.Sleep(10000);

            //添加主节点
            lY3762.PacketToCarrierCtr(1, new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 }, bwIndex); //添加主节点
            Thread.Sleep(10000);
        }
        /// <summary>
        /// 暂停路由
        /// </summary>
        public void PauseRouter(int bwIndex)
        {

            lY3762.PacketToCarrierInit(0x12, 2, bwIndex);
            Thread.Sleep(200);
        }
        /// <summary>
        /// 添加主节点
        /// </summary>
        /// <param name="bwIndex">表位号</param>
        public void AddMainNode(int bwIndex)
        {
            lY3762.PacketToCarrierCtr(1, new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 }, bwIndex);
            Thread.Sleep(200);
        }
        #endregion

        /// <summary>
        /// 打包645成376.2
        /// </summary>
        /// <param name="Frame645">645完整帧</param>
        /// <param name="dltType">
        /// 协议类型:
        ///     00[透明传输],
        ///     01[DL/T 645—1997],
        ///     02[DL/T 645—2007]
        /// </param>
        /// <param name="RFrame645">out 376.2</param>
        public  void PacketTo3762Frame(byte[] Frame645, out byte[] RFrame645, int bwIndex)
        {

            lY3762.Packet645To3762Frame(Frame645, out RFrame645, bwIndex);
        }
    }
}
