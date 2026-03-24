using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.Core.Model.Meter;
using LYZD.Core.Struct;
using LYZD.SocketComm;
using LYZD.TerminalProtocol.Encryption;
using LYZD.TerminalProtocol.GW;
using LYZD.Utility.Log;
using LYZD.ViewModel.CheckController.MulitThread;
using LYZD.ViewModel.IMICP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ZH.MeterProtocol.Enum;

namespace LYZD.ViewModel.CheckController
{
    /// <summary>
    /// 检定的虚方法,所有检定类的基类
    /// </summary>
    public class VerifyBase
    {
        public VerifyBase()
        {
            MeterNumber = meterInfo.Length;
            setData = new byte[MeterNumber][];
            ImportantEvnetCount = new int[MeterNumber];
            NormalEventCount = new int[MeterNumber];
            //Item_Std = new string[MeterNumber];
            //Item_Value= new string[MeterNumber];
            //Item_Resoult = new string[MeterNumber];

            //更新一下电能表数据
            //MeterHelper.Instance.Init();
            //UpdateMeterProtocol();
        }

        #region 检定

        /// <summary>
        /// 检定入口
        /// </summary>
        /// <param name="Verify"></param>
        public void DoVerify()
        {
            try
            {
                if (!CheckPara()) //检定参数是否合法
                {
                    MessageAdd($"解析方案参数出错,参数内容:{Test_Value}", EnumLogType.错误信息);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageAdd($"解析方案参数出错,参数内容:{Test_Value}\r\n" + ex.ToString(), EnumLogType.错误信息);
                return;
            }

            try
            {
                testTempData.Clear();
                StartTime = DateTime.Now;
                Resoult = new string[MeterNumber];
                Resoult.Fill(Core.Helper.Const.合格);
                MessageAdd("开始检定", EnumLogType.提示信息);
                Verify(); //跳转到具体的检定方法
                //刷新总结论
                if (!Stop)
                {
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (!meterInfo[i].YaoJianYn) continue;
                        ResultDictionary["结论"][i] = Resoult[i];
                    }
                    RefUIData("结论");
                }

                MessageAdd("检定完成", EnumLogType.提示信息);
                Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                if (IsRunWc) StopWcb(WcControlClose, 0xff);
                MessageAdd("检定出错,错误编号：999－10001　\r\n" + ex, EnumLogType.错误信息);
                return;
            }
            finally
            {
                //完成后清理工作
                Stop = true;
                CheckOver = true;
                if (IsRunWc) StopWcb(WcControlClose, 0xff);
            }
        }

        /// <summary>
        /// 参数合法性检测[由具体检定器实现]
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckPara()
        {
            return true;
        }



        public virtual void Verify()    //基类实现
        {

            //获取额定电流--额定电压
            xIb = Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA, HGQ);
            Xub = OnMeterInfo.MD_UB;
            //NoResoult = new string[MeterNumber];  //不合格原因
            //NoResoult.Fill("");
        }


        #endregion

        #region 终端通讯属性
        public int MaxWaitSeconds_Write = 20000;//参数设置，查询，读内存区数据最大延时
        public int MaxWaitSeconds_Read485 = 80000;//读电能表一类数据最大延时
        public byte[] SetData = new byte[0];                  // 下发终端数据内容
        public string SendData = "";
        public byte[] SendDataBytes = new byte[0];
        public byte[] OutFrame = new byte[0];
        public int[] NormalEventCount = new int[0];
        public int[] ImportantEvnetCount = new int[0];


        /// <summary>
        /// 232总线数量
        /// </summary>
        public static int Conn232Number = 1;
        /// <summary>
        /// 间隔时间
        /// </summary>
        public double dintervaltime = 0;
        /// <summary>
        /// 结论--用来判断子项是否合格--开始检定默认合格
        /// </summary>
        public string[] Resoult;
        /// <summary>
        /// 遥信路数
        /// </summary>
        public int RemoteCount;
        /// <summary>
        /// 触点类型
        /// </summary>
        public string RemoteType;
        /// <summary>
        /// 设置d数据
        /// </summary>
        public byte[][] setData = new byte[0][];
        /// <summary>
        /// 通讯返回结果 0：正确，非0：不正确 
        /// </summary>
        public int[] TalkResult = new int[0];
        /// <summary>
        /// 通讯返回数据内容
        /// </summary>
        public Dictionary<int, string[]> RecData = new Dictionary<int, string[]>();
        #endregion

        #region 属性
        /// <summary>
        /// 额定电流
        /// </summary>
        public float xIb = 0f;
        /// <summary>
        /// 额定电压
        /// </summary>
        public float Xub = 0f;
        /// <summary>
        /// 临时数据存放列表
        /// </summary>
        public List<TestTempData[]> testTempData = new List<TestTempData[]>();
        /// <summary>
        /// 临时数据
        /// </summary>
        public static TestTempData[] TempData;
        /// <summary>
        /// 建立应用连接的时间
        /// </summary>
        public static DateTime Dog_ConnectTime;

        /// <summary>
        /// 终端通讯协议--698-376
        /// </summary>
        public static TerminalProtocolTypeEnum TerminalProtocolType = TerminalProtocolTypeEnum._698;
        /// <summary>
        /// 通讯方式--232-网线
        /// </summary>
        public static Cus_EmChannelType TerminalChannelType = Cus_EmChannelType.Channel232;
        /// <summary>
        /// 终端通讯组
        /// </summary>
        public static TerminalTalker[] Talkers = new TerminalTalker[0];// 终端通讯组
        public static string[] VerifyDatas = new string[0];        // 检定数据数组
        /// <summary>
        /// 是否是双协议的电表
        /// </summary>
        //public static bool IsDoubleProtocol = false;

        public string WcLimitName = "规程误差限";

        public MulitThread.MulitEncryptionWorkThreadManager EncryptionThread;

        /// <summary>
        /// 上一次联接加密机的时间
        /// </summary>
        private static DateTime EncryLastLinkTime { get; set; }
        /// <summary>
        /// 结论字典
        /// </summary>
        private Dictionary<string, string[]> resultDictionary = new Dictionary<string, string[]>();
        /// <summary>
        /// 结论字典
        /// </summary>
        protected Dictionary<string, string[]> ResultDictionary
        {
            get { return resultDictionary; }
            set { resultDictionary = value; }
        }

        /// <summary>
        /// 不合格原因
        /// </summary>
        public string[] NoResoult;

        /// <summary>
        /// 检定点的参数值
        /// </summary>
        public string Test_Value { get; set; }
        /// <summary>
        /// 检定点的参数值描述
        /// </summary>
        public string Test_Format { get; set; }
        /// <summary>
        /// 检定点的编号
        /// </summary>
        public string Test_No { get; set; }
        /// <summary>
        /// 检定点的名字
        /// </summary>
        public string Test_Name { get; set; }
        /// <summary>
        /// 是否默认合格
        /// </summary>
        public bool DefaultValue { get; set; }

        /// <summary>
        /// 是否是演示版本-True是
        /// </summary>
        public bool IsDemo { get; set; }
        /// <summary>
        /// 获取或设置停止检定状态
        /// </summary>
        public bool Stop { get; set; }
        /// <summary>
        /// 是否调表
        /// </summary>
        public bool IsMeterDebug { get; set; }
        /// <summary>
        /// 是否已经完成本项目检定
        /// 只有m_Stop=true且m_CheckOver=true时，检定停止操作才算真正完成
        /// </summary>
        protected bool CheckOver = false;
        /// <summary>
        /// 表位数量
        /// </summary>
        public int MeterNumber { get; set; }

        /// <summary>
        /// 1/2 做过/共
        /// </summary>
        public static string Progress { get; set; } = "";

        /// <summary>
        /// 用于程序退出或其他情况时关闭误差板
        /// </summary>
        public int WcControlClose { get; set; }
        /// <summary>
        /// 误差板是否在运行
        /// </summary>
        public bool IsRunWc { get; set; }

        /// <summary>
        /// 功率方向
        /// </summary>
        public PowerWay FangXiang = PowerWay.正向有功;

        /// <summary>
        /// 获取当前检定是有功还是无功
        /// </summary>
        public bool IsYouGong
        {
            get
            {
                bool _IsP = false;
                if (FangXiang == PowerWay.正向有功 || FangXiang == PowerWay.反向有功)
                    _IsP = true;
                return _IsP;
            }
        }

        /// <summary>
        /// 表数据
        /// </summary>
        public static TestMeterInfo[] meterInfo { get; set; }


        private static TestMeterInfo onMeterInfo;
        public static TestMeterInfo OnMeterInfo
        {
            get
            {
                //if (onMeterInfo == null)
                //{
                onMeterInfo = GetOneMeterInfo();
                //}
                return onMeterInfo;
            }
            set
            {
                onMeterInfo = value;
            }

        }
        /// <summary>
        /// 检定到当前计时，单位：秒
        /// </summary>
        public long VerifyPassTime
        {
            get
            {
                return (long)((TimeSpan)(DateTime.Now - StartTime)).TotalSeconds;
            }
        }
        /// <summary>
        /// 检定开始时间,用于检定计时
        /// </summary>
        protected DateTime StartTime;

        /// <summary>
        /// 获得要检数组
        /// </summary>
        /// <param name="IsYaoJIan">false的时候获取不要检定的数组</param>
        /// <returns></returns>
        public bool[] GetYaoJian(bool IsYaoJIan = true)
        {
            bool[] yaoJianList = new bool[MeterNumber];
            for (int i = 0; i < MeterNumber; i++)
            {
                if (IsYaoJIan)
                {
                    yaoJianList[i] = meterInfo[i].YaoJianYn;
                }
                else
                {
                    yaoJianList[i] = !meterInfo[i].YaoJianYn;
                }
            }
            return yaoJianList;

        }

        #region 静态使用的属性
        public static float U
        {
            get
            {
                try
                {
                    if (OnMeterInfo == null)
                    {
                        return 57.7F;
                    }
                    else
                    {
                        return OnMeterInfo.MD_UB;
                    }
                }
                catch
                {
                    return 57.7F;
                }
            }
        }

        /// <summary>
        /// 测量方式
        /// </summary>
        public static WireMode Clfs
        {
            get
            {
                if (OnMeterInfo == null) return WireMode.三相四线;
                return (WireMode)Enum.Parse(typeof(WireMode), OnMeterInfo.MD_WiringMode);
            }
        }
        /// <summary>
        /// 是否经互感器 True-互感式，false直接式
        /// </summary>
        public static bool HGQ
        {
            get
            {
                if (OnMeterInfo != null)
                {
                    if (OnMeterInfo.MD_ConnectionFlag == "互感式")
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }
        /// <summary>
        /// 是否经止逆器
        /// </summary>
        public static bool ZNQ
        {
            get
            {
                if (OnMeterInfo != null)
                {
                    if (OnMeterInfo.MD_ConnectionFlag == "有止逆")
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
        }

        /// <summary>
        /// 频率
        /// </summary>
        public static float PL
        {
            get
            {
                if (OnMeterInfo != null)
                {
                    return float.Parse(OnMeterInfo.MD_Frequency.ToString());
                }
                return 50F;
            }
        }
        #endregion


        #endregion

        #region 需要用到的通用方法

        #region 读取误差

        /// <summary>
        /// 读取误差
        /// </summary>
        /// <param name="Wc">返回误差值</param>
        /// <param name="WcNum">误差次数</param>
        /// <param name="power">功率方向</param>
        /// <returns></returns>
        public bool ReadWc(ref string[] Wc, ref int[] WcNum, PowerWay power)
        {
            try
            {
                int readType = 0;
                switch (power)
                {
                    case PowerWay.正向有功:
                        readType = 0;
                        break;
                    case PowerWay.正向无功:
                        readType = 1;
                        break;
                    case PowerWay.反向有功:
                        readType = 0;
                        break;
                    case PowerWay.反向无功:
                        readType = 1;
                        break;
                    default:
                        break;
                } //功率方向

                stError[] stErrors = ReadWcbData(GetYaoJian(), readType);  //读取表位误差
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (stErrors[i] != null)
                    {
                        Wc[i] = stErrors[i].szError;
                        WcNum[i] = stErrors[i].Index;
                    }
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// 读取误差
        /// </summary>
        /// <param name="Wc">返回误差值</param>
        /// <param name="WcNum">误差次数</param>
        /// <param name="power">功率方向</param>
        /// <returns></returns>
        public bool ReadWc(ref string[] Wc, ref int[] WcNum, int readType)
        {
            try
            {
                stError[] stErrors = ReadWcbData(GetYaoJian(), readType);  //读取表位误差
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (stErrors[i] != null)
                    {
                        Wc[i] = stErrors[i].szError;
                        WcNum[i] = stErrors[i].Index;
                    }
                }
                return true;

            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }


        /// <summary>
        /// 读取误差
        /// </summary>
        /// <param name="Wc">返回误差值</param>
        /// <param name="WcNum">误差次数</param>
        /// <param name="power">功率方向</param>
        /// <returns></returns>
        public stError[] ReadWc(PowerWay power)
        {
            try
            {
                int readType = 0;
                switch (power)
                {
                    case PowerWay.正向有功:
                        readType = 0;
                        break;
                    case PowerWay.正向无功:
                        readType = 1;
                        break;
                    case PowerWay.反向有功:
                        readType = 0;
                        break;
                    case PowerWay.反向无功:
                        readType = 1;
                        break;
                    default:
                        break;
                } //功率方向

                stError[] stErrors = ReadWcbData(GetYaoJian(), readType);  //读取表位误差
                return stErrors;

            }
            catch (Exception)
            {
                return null;
                throw;
            }

        }


        /// <summary>
        /// 做误差的时候初始化设备
        /// </summary>
        /// <param name="PowerFangXiang">功率方向</param>
        /// <param name="PowerYuanJian">功率元件</param>
        /// <param name="PowerYinSu">功率因素</param>
        /// <param name="PowerDianLiu">负载电流</param>
        /// <param name="pulselap">圈数</param>
        /// <returns></returns>
        public bool ErrorInitEquipment(PowerWay PowerFangXiang, Cus_PowerYuanJian PowerYuanJian, string PowerYinSu, string PowerDianLiu, int pulselap)
        {
            if (IsDemo) return true;
            bool isP = (PowerFangXiang == PowerWay.正向有功 || PowerFangXiang == PowerWay.反向有功) ? true : false;
            int[] meterconst = MeterHelper.Instance.MeterConst(isP);
            float xIb = Number.GetCurrentByIb(PowerDianLiu, OnMeterInfo.MD_UA, HGQ);//计算电流
            if (!VerifyConfig.GearModel)
            {
                StdGear(0x13, 0, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb);
                WaitTime("正在设置电流挡位", 2);
            }
            MessageAdd("正在升源...", EnumLogType.提示信息);
            if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb, PowerYuanJian, PowerFangXiang, PowerYinSu))
            {
                MessageAdd("升源失败,退出检定", EnumLogType.提示信息);
               
                return false;
            }
            long constants = VerifyConfig.StdConst;
            if (!VerifyConfig.ConstModel)  //自动常数--从标准表读取
            {
                constants = GetStaConst(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb);
            }
            else    //固定常数
            {
                //1:设置标准表挡位、常数
                MessageAdd("正在设置标准表常数...", EnumLogType.提示信息);
                constants = GetStdConst(OnMeterInfo.MD_UB, xIb);
                StdGear(0x13, constants, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb);
            }
            MessageAdd("正在设置标准表脉冲...", EnumLogType.提示信息);
            int index = 0;
            if (!isP) index = 1;

            SetPulseType((index + 49).ToString("x"));
            if (Stop) return true;
            MessageAdd("开始初始化基本误差检定参数!", EnumLogType.提示信息);
            //设置误差版被检常数
            MessageAdd("正在设置误差版标准常数...", EnumLogType.提示信息);
            int SetConstants = (int)(constants / 100);
            SetStandardConst(0, SetConstants, -2, 0xff);
            //设置误差版标准常数 TODO2
            MessageAdd("正在设置误差版被检常数...", EnumLogType.提示信息);
            int[] pulselaps = new int[MeterNumber];  //这里是为了以后不同表位不同圈数预留--目前暂时用着吧
            pulselaps.Fill(pulselap);
            if (!SetTestedConst(index, meterconst, 0, pulselaps, 0xff))
            {
                MessageAdd("初始化误差检定参数失败", EnumLogType.提示信息);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 固定常数下获取设置的常数值
        /// </summary>
        ///<param name="u">电压</param>
        /// <param name="i">电流</param>
        /// <returns></returns>
        private long GetStdConst(float u, float i)
        {
            long constants = VerifyConfig.StdConst;
            if (constants == 0)//0的情况就用这里设置的，否则用设置里面设置的
            {
                #region 获取电压电流的挡位

                //电压挡位
                int Gear_U = 2;  // 预设220V挡
                int Gear_I = 6;  // 预设220V挡
                if (u > 350)             // 380V
                    Gear_U = 3;
                else if (u > 144)         // 220V
                    Gear_U = 2;
                else if (u > 72)            // 100V
                    Gear_U = 1;
                else if (u > 0)               // 57.7V
                    Gear_U = 0;

                if (i > 60)                     // 100A
                    Gear_I = 6;
                else if (i > 30)               // 50A
                    Gear_I = 5;
                else if (i > 15)                // 20A
                    Gear_I = 5;
                else if (i > 7)                // 10A
                    Gear_I = 3;
                else if (i > 1.5)                   // 2.5A
                    Gear_I = 2;
                else if (i > 0.3)             // 0.5A
                    Gear_I = 1;
                else if (i > 0.03)          // 0.1A
                    Gear_I = 0;
                else            // 0.1A
                    Gear_I = 0;
                #endregion


                #region 根据电压电流挡位获取常数 --采用降一档
                switch (Gear_U)
                {
                    case 0:            // 57.7V
                        switch (Gear_I)
                        {
                            case 0: // 0.1A及以下
                                constants = 1600000000;
                                break;
                            case 1: // 0.5A
                                constants = 400000000;
                                break;
                            case 2: // 2.5A
                                constants = 100000000;
                                break;
                            case 3: // 10A
                                constants = 25000000;
                                break;
                            case 4: // 20A
                                constants = 8000000;
                                break;
                            case 5: // 50A
                                constants = 4000000;
                                break;
                            case 6: // 100A
                                constants = 4000000;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 1:          // 100V
                        switch (Gear_I)
                        {
                            case 0: // 0.1A及以下
                                constants = 800000000;
                                break;
                            case 1: // 0.5A
                                constants = 200000000;
                                break;
                            case 2: // 2.5A
                                constants = 50000000;
                                break;
                            case 3: // 10A
                                constants = 12500000;
                                break;
                            case 4: // 20A
                                constants = 4000000;
                                break;
                            case 5: // 50A
                                constants = 2000000;
                                break;
                            case 6: // 100A
                                constants = 2000000;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 2:       // 220V
                        switch (Gear_I)
                        {
                            case 0: // 0.1A及以下
                                constants = 400000000;
                                break;
                            case 1: // 0.5A
                                constants = 100000000;
                                break;
                            case 2: // 2.5A
                                constants = 25000000;
                                break;
                            case 3: // 10A
                                constants = 6000000;
                                break;
                            case 4: // 20A
                                constants = 2000000;
                                break;
                            case 5: // 50A
                                constants = 1000000;
                                break;
                            case 6: // 100A
                                constants = 1000000;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 3:     // 380V
                        switch (Gear_I)
                        {
                            case 0: // 0.1A及以下
                                constants = 200000000;
                                break;
                            case 1: // 0.5A
                                constants = 50000000;
                                break;
                            case 2: // 2.5A
                                constants = 12000000;
                                break;
                            case 3: // 10A
                                constants = 3000000;
                                break;
                            case 4: // 20A
                                constants = 1000000;
                                break;
                            case 5: // 50A
                                constants = 500000;
                                break;
                            case 6: // 100A
                                constants = 500000;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                #endregion
            }
            return constants;
        }
        #endregion

        /// <summary>
        /// 计算指定负载下的标准功率.(W)
        /// </summary>
        /// <param name="U">负载电压</param>
        /// <param name="I">负载电流</param>
        /// <param name="Clfs">测量方式</param>
        /// <param name="Yj">元件H，ABC</param>
        /// <param name="Glys">功率因数，0.5L</param>
        /// <param name="isP">true 有功，false 无功</param>
        /// <returns>标准功率</returns>
        protected float CalculatePower(float U, float I, WireMode Clfs, Cus_PowerYuanJian Yj, string Glys, bool isP)
        {
            float flt_GlysP = 1;
            float flt_GlysQ;
            if (isP)
            {
                float.TryParse(Glys.Replace("C", "").Replace("L", "").ToString(), out flt_GlysP);
                flt_GlysQ = (float)Math.Sqrt(1 - Math.Pow(flt_GlysP, 2));
            }
            else
            {
                float.TryParse(Glys.Replace("C", "").Replace("L", "").ToString(), out flt_GlysQ);
                flt_GlysP = (float)Math.Sqrt(1 - Math.Pow(flt_GlysP, 2));
            }
            float p = U * I * flt_GlysP;
            float q = U * I * flt_GlysQ;
            if (Cus_PowerYuanJian.H == Yj)
            {
                if (Clfs == WireMode.三相四线)
                {
                    p *= 3F;
                    q *= 3F;
                }
                else if (Clfs == WireMode.单相)
                {

                }
                else
                {
                    p *= 1.732F;
                    q *= 1.732F;
                }
            }
            return isP ? p : q;
        }
        /// <summary>
        /// 获取指定电能表当前功率方向下表等级
        /// </summary>
        /// <returns></returns>
        protected float MeterLevel(TestMeterInfo meter)
        {
            string[] level = Number.GetDj(meter.MD_Grane);
            float _MeterLevel = 0;
            if (level[IsYouGong ? 0 : 1] == "A")
            {
                _MeterLevel = 2.0F;
            }
            else if (level[IsYouGong ? 0 : 1] == "B")
            {
                _MeterLevel = 1.0F;
            }
            else if (level[IsYouGong ? 0 : 1] == "C")
            {
                _MeterLevel = 0.5F;
            }
            else if (level[IsYouGong ? 0 : 1] == "D")
            {
                _MeterLevel = 0.2F;
            }
            else
            {
                _MeterLevel = float.Parse(level[IsYouGong ? 0 : 1]);                   //当前表的等级
            }
            return _MeterLevel;                   //当前表的等级
        }

        public long TimeSub(DateTime Time1, DateTime Time2)
        {
            TimeSpan tsSub = Time1.Subtract(Time2);
            return tsSub.Hours * 60 * 60 * 1000 + tsSub.Minutes * 60 * 1000 + tsSub.Seconds * 1000 + tsSub.Milliseconds;
        }

        /// <summary>
        /// 返回第一个要检定表的数据
        /// </summary>
        /// <returns></returns>
        public static TestMeterInfo GetOneMeterInfo()
        {
            for (int i = 0; i < meterInfo.Length; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    return meterInfo[i];
                }
            }
            return new TestMeterInfo();

        }
        /// <summary>
        /// 首个要检表有效位,由0开始
        /// </summary>
        public static int FirstIndex
        {
            get
            {

                for (int i = 0; i < meterInfo.Length; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                        return i;
                }
                return -1;
            }
        }


        /// <summary>
        /// 结论的所有列名称
        /// </summary>
        /// <param name="arrayResultName"></param>
        protected string[] ResultNames
        {
            set
            {
                if (value != null)
                {
                    ResultDictionary.Clear();
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (!resultDictionary.ContainsKey(value[i]))
                        {
                            resultDictionary.Add(value[i], new string[MeterNumber]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检测是否跳差
        /// </summary>
        /// <param name="lastError">前一误差</param>
        /// <param name="curError">当前误差</param>
        /// <param name="meterLevel">表等级</param>
        /// <param name="m_WCJump">跳差系数</param>
        /// <returns>T:跳差;F:不跳差</returns>
        protected bool CheckJumpError(string lastError, string curError, float meterLevel, float m_WCJump)
        {
            bool result = false;
            if (Number.IsNumeric(lastError) && Number.IsNumeric(curError))
            {
                float _Jump = float.Parse(curError) - float.Parse(lastError);
                if (Math.Abs(_Jump) > meterLevel * m_WCJump)
                {
                    result = true;
                }
            }
            return result;
        }

        #region 结论转换
        public void ConvertTestResult(string resultName, float[] arrayResult, int dotNumber = 2)
        {
            string formatTemp = "0";
            if (dotNumber > 0)
            {
                formatTemp = "0.".PadRight(2 + dotNumber, '0');
            }
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (arrayResult.Length > i)
                    {
                        ResultDictionary[resultName][i] = arrayResult[i].ToString(formatTemp);
                    }
                }
            }
            RefUIData(resultName);
        }
        #endregion

        #endregion

        #region 电表命令
        public static Queue<EventsRecUDPData> queueReceiveData = new Queue<EventsRecUDPData>();//接受mac获取指令
        public static Socket SocketUdpControl = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private static IPEndPoint IPLocalPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5001);
        private static IPEndPoint IPRemotePoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);
        private static IPEndPoint MACIPLocalPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5002);
        /// <summary>
        /// 控制模拟表
        /// </summary>
        /// <param name="strCommand"></param>
        public static void ControlVirtualMeter(string strCommand)
        {
            if (!SocketUdpControl.IsBound) SocketUdpControl.Bind(IPLocalPoint);
            SocketUdpControl.SendTo(System.Text.ASCIIEncoding.ASCII.GetBytes(strCommand), IPRemotePoint);
            Thread.Sleep(100);
        }
        private static Socket udpServer;
        public static void ListenUDP()
        {
            //1,创建socket
            udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //UdpClient udpServer = new UdpClient(IPLocalPoint);

            //TODO:处理UDP通讯远程主机强制关闭了一个现有连接的问题——不知道会不会有问题
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            udpServer.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);

            //2,绑定ip跟端口号
            udpServer.Bind(MACIPLocalPoint);

            //3，监听接收数据，并且丢到队列里
            new Thread(ReceiveMessage) { IsBackground = true }.Start();

            //4.开个线程去处理队列里面的mac数据并且返回
            new Thread(ReceiveDataMac) { IsBackground = true }.Start();
        }

        /// <summary>
        /// 监听虚拟表发送过来的数据线程
        /// </summary>
        public static void ReceiveMessage()
        {
            while (true)
            {
                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = new byte[1024*1024];
                int length = udpServer.ReceiveFrom(data, ref remoteEndPoint);//这个方法会把数据的来源(ip:port)放到第二个参数上
                queueReceiveData.Enqueue(new EventsRecUDPData() { Remote = remoteEndPoint, Data = data, rev = length, dateTime = DateTime.Now });
            }
        }

        private static object RevLock = new object();
        /// <summary>
        /// 处理队列
        /// </summary>
        private static void ReceiveDataMac()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(10);
                    if (queueReceiveData.Count > 0)
                    {
                        lock (RevLock)
                        {
                            EventsRecUDPData RecUDPData = queueReceiveData.Dequeue();
                            if (RecUDPData == null) continue;
                            string[] msg = Encoding.UTF8.GetString(RecUDPData.Data, 0, RecUDPData.rev).Split('|');
                            Console.WriteLine(Encoding.ASCII.GetString(RecUDPData.Data, 0, RecUDPData.rev));
                            int ret = 0;
                            string cOutData = null;
                            string cOutMAC = null;
                            if (msg.Length == 7)
                            {
                                //DataArrival(Tstr);
                                int iKeyState = Convert.ToInt32(msg[0]);
                                int cOperateMode = Convert.ToInt32(msg[1]);
                                string cTESAMNO = msg[2];
                                string cRandHost = msg[3];
                                string cReadData = msg[4];

                                ret = IEncryptionFunction698.iObj_Meter_Formal_GenReadData(iKeyState, cOperateMode, cTESAMNO, cRandHost, cReadData, ref cOutData, ref cOutMAC);
                                //LogManager.AddMessage("第一次ret=" + ret.ToString(), EnumLogSource.检定业务日志, EnumLevel.Warning);
                                if (ret != 0)
                                {
                                    Thread.Sleep(500);
                                    ret = IEncryptionFunction698.iObj_Meter_Formal_GenReadData(iKeyState, cOperateMode, cTESAMNO, cRandHost, cReadData, ref cOutData, ref cOutMAC);
                                    //LogManager.AddMessage("第二次ret=" + ret.ToString(), EnumLogSource.检定业务日志, EnumLevel.Warning);
                                }
                            }
                            else
                            {
                                ret = -1;//获取MAC地址参数错误
                            }

                            if (ret != 0)
                            {
                                Thread.Sleep(1000);
                                //udpServer.SendTo(System.Text.ASCIIEncoding.ASCII.GetBytes(RecUDPData.Remote.ToString()+"|"+ RecUDPData.dateTime.ToString("yyyy-MM-dd HH-mm-ss fff") + "|" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss fff")), RecUDPData.Remote);
                                udpServer.SendTo(System.Text.ASCIIEncoding.ASCII.GetBytes("12345678"), RecUDPData.Remote);
                            }
                            else
                            {
                                udpServer.SendTo(System.Text.ASCIIEncoding.ASCII.GetBytes(cOutMAC.ToString()), RecUDPData.Remote);
                                LogManager.AddMessage("发送MAC值" + cOutMAC.ToString(), EnumLogSource.检定业务日志, EnumLevel.Warning);
                                //sAPDU = "9000" + GetMiWenLen(sTmp1) + sTmp1 + "010004" + "12345678";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.AddMessage("获取MAC值异常" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.Error);
                    return;
                }
            }
        }



        #endregion

        #region UI

        //刷新界面上的检定标记
        public void RefMeterYaoJian()
        {
            for (int i = 0; i < MeterNumber; i++)
            {
                string value = meterInfo[i].YaoJianYn ? "1" : "0";
                EquipmentData.MeterGroupInfo.Meters[i].SetProperty("MD_CHECKED", value);
                DAL.DynamicModel Model2 = new DAL.DynamicModel();
                Model2.SetProperty("MD_CHECKED", value);
                string id = EquipmentData.MeterGroupInfo.Meters[i].GetProperty("METER_ID") as string;
                string where1 = $"METER_ID = '{id}'";
                DAL.DALManager.MeterTempDbDal.Update("T_TMP_METER_INFO", where1, Model2, new List<string> { "MD_CHECKED" });
            }

        }

        /// <summary>
        /// 获得表位的结论
        /// </summary>
        /// <param name="meterNo">表位编号，从0开始</param>
        /// <returns></returns>
        public bool GetMeterResult(int meterNo, string exceptId = "")
        {
            return EquipmentData.CheckResults.GetMeterResult(meterNo, exceptId);
        }

        /// <summary>
        /// 刷新UI数据--
        /// </summary>
        /// <param name="testNo">检定点编号</param>
        /// <param name="columnName">详细数据列名</param>
        /// <param name="value">值</param>
        public void RefUIData(string columnName)
        {
            if (columnName == "结论")
            {
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (!meterInfo[i].YaoJianYn) continue;
                    Resoult[i] = ResultDictionary[columnName][i];
                }
            }
            EquipmentData.CheckResults.UpdateCheckResult(Test_No, columnName, ResultDictionary[columnName]);
        }
        public Dictionary<string, string[]> GetCheckResult()
        {
            return EquipmentData.CheckResults.GetCheckResult();
        }
        #region 分项结论

        /// 添加分项结论
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="tempData">数据</param>
        /// <param name="IsClear">是否清空数据</param>
        public void AddItemsResoult(string name, TestTempData[] tempData, bool IsClear = true)
        {

            string itemName = "分项结论";
            if (TerminalProtocolType == TerminalProtocolTypeEnum._698)
            {
                itemName += "698";
            }
            else if (TerminalProtocolType == TerminalProtocolTypeEnum._376)
            {
                itemName += "376";
            }

            if (!ResultDictionary.ContainsKey(itemName))
            {
                ResultDictionary.Add(itemName, new string[MeterNumber]);
            }
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (ResultDictionary[itemName][i] == null || ResultDictionary[itemName][i] == "")
                    {
                        ResultDictionary[itemName][i] = $"{name}|{DateTime.Now.ToString()}|{tempData[i].ToString()}";
                    }
                    else
                    {
                        ResultDictionary[itemName][i] += $"#{name}|{DateTime.Now.ToString()}|{tempData[i].ToString()}";
                    }

                    if (ResultDictionary.ContainsKey(name)) //是否存在这个列--就是是否需要显示的
                    {
                        ResultDictionary[name][i] = tempData[i].Resoult; //只显示结论
                    }
                    if (Resoult[i] == "合格" && tempData[i].Resoult == "不合格")  //分项有一个不合格那么总结论不合格
                    {
                        Resoult[i] = "不合格";
                    }


                }
            }
            RefUIData(itemName);

            if (ResultDictionary.ContainsKey(name))
            {
                RefUIData(name);
            }
            if (IsClear)
            {
                for (int i = 0; i < meterInfo.Length; i++)
                {
                    tempData[i] = new TestTempData();
                }
            }
        }


        #endregion


        /// <summary>
        /// 日志提示
        /// </summary>
        /// <param name="Tips">日志内容</param>
        /// <param name="logType">日志类型</param>
        /// <param name="IsSync">是否同步到表位</param>
        public void MessageAdd(string Tips, EnumLogType logType, bool IsSync = false)
        {
            EquipmentData.Controller.MessageAdd(Tips, logType);
            if (IsSync)
            {
                for (int i = 0; i < meterInfo.Length; i++)
                {
                    if (meterInfo[i].YaoJianYn) EquipmentData.Controller.MessageAdd(i + 1, Tips, logType);
                }
            }
        }

        /// <summary>
        /// 日志提示--表位日志
        /// </summary>
        /// <param name="Id">终端编号-1开始</param>
        /// <param name="Tips">日志内容</param>
        /// <param name="logType">日志类型</param>
        public void MessageAdd(int Id, string Tips, EnumLogType logType)
        {
            EquipmentData.Controller.MessageAdd(Id, Tips, logType);
        }

        /// <summary>
        /// 异常停止检定
        /// </summary>
        public void TryStopTest()
        {
            EquipmentData.Controller.TryStopVerify();
        }

        /// <summary>
        /// 等待方法
        /// </summary>
        /// <param name="tips">提示信息</param>
        /// <param name="time">等待时间(秒)</param>
        public void WaitTime(string tips, int time)
        {
            while (time > 0)
            {
                MessageAdd($"{tips},等待{time}秒...", EnumLogType.提示信息);
                System.Threading.Thread.Sleep(1000);
                time--;
                if (IsDemo) return;
                if (Stop) return;
            }
        }

        #endregion

        #region 终端方法

        /// <summary>
        /// 切换线程数量--232为true
        /// </summary>
        /// <param name="Set232"></param>
        public void Set232ThreadNmber(bool Set232)
        {
            if (Set232)
            {
                SocketThreadManager.Instance.MaxThread = Conn232Number;
                SocketThreadManager.Instance.MaxTaskCountPerThread = MeterNumber / Conn232Number;
            }
            else
            {
                SocketThreadManager.Instance.MaxTaskCountPerThread = 1;
                SocketThreadManager.Instance.MaxThread = MeterNumber;
            }
        }


        #region 获取和解析数据
        /// <summary>
        /// 获取去除序号后的数据
        /// </summary>
        /// <param name="p_str_data"></param>
        /// <returns></returns>
        public virtual string GetRemoveNumData(string p_str_data)
        {
            try
            {

                if (!p_str_data.Contains("."))
                {
                    return "";
                }
                return p_str_data.Split('.')[1];
            }
            catch
            {
                return "";
            }
        }

        public DateTime GetDateTime(string strDate)
        {
            try
            {
                return Convert.ToDateTime(strDate);
            }
            catch
            {
                return Convert.ToDateTime("2000-1-1");
            }
        }
        public virtual string GetMac(Dictionary<int, string[]> RecData, int index)
        {
            try
            {
                string strReturn = "";
                string[] sRecData = RecData[index];
                for (int i = 0; i < sRecData.Length; i++)
                {
                    if (sRecData[i].Contains("MAC"))
                    {
                        strReturn = sRecData[i].Split('：')[1];
                        break;
                    }
                }
                return strReturn;
            }
            catch
            {
                return "9999";
            }
        }

        public virtual string GetMac(string[] sRecData, int index, string sData)
        {
            try
            {
                string strReturn = "";
                for (int i = 0; i < sRecData.Length; i++)
                {
                    if (sRecData[i].Contains(sData))
                    {
                        strReturn = sRecData[i + index].Split('：')[1];
                        break;
                    }
                }
                return strReturn;
            }
            catch
            {
                return "9999";
            }
        }
        /// <summary>
        /// 获取密文长度
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public string GetMiWenLen(string strData)
        {
            strData = strData.Replace(" ", "");
            string slen = ""; ;
            if (strData.Length / 2 > 127)
            {
                slen = "82" + (strData.Length / 2).ToString("x4");
            }
            else
                slen = (strData.Length / 2).ToString("x2");
            return slen;
        }
        public virtual string GetData(string[] RecData, int serinalNumber, EnumTerimalDataType e_type)
        {
            try
            {
                string strReturn = "";
                strReturn = RecData[serinalNumber].Split('：')[1];
                switch (e_type)
                {
                    case EnumTerimalDataType.e_byte:
                        return Convert.ToByte(strReturn).ToString();
                    case EnumTerimalDataType.e_int:
                        return Convert.ToInt32(strReturn).ToString();
                    case EnumTerimalDataType.e_float:
                        return Convert.ToSingle(strReturn).ToString();
                    case EnumTerimalDataType.e_double:
                        return Convert.ToDouble(strReturn).ToString();
                    case EnumTerimalDataType.e_datetime:
                        return Convert.ToDateTime(strReturn).ToString();
                    case EnumTerimalDataType.e_bs8:
                        if (strReturn.Length == 8)
                            return strReturn;
                        else
                            return "99999999";
                    case EnumTerimalDataType.e_bs16:
                        if (strReturn.Length == 16)
                            return strReturn;
                        else
                            return "9999999999999999";
                    case EnumTerimalDataType.e_string:
                        return strReturn;
                    default:
                        return "99999";
                }
            }
            catch
            {
                switch (e_type)
                {
                    case EnumTerimalDataType.e_byte:
                        return "99";
                    case EnumTerimalDataType.e_int:
                        return "99999";
                    case EnumTerimalDataType.e_float:
                        return "99999.9";
                    case EnumTerimalDataType.e_double:
                        return "99999.99";
                    case EnumTerimalDataType.e_datetime:
                        return "2000-1-1";
                    case EnumTerimalDataType.e_bs8:
                        return "99999999";
                    case EnumTerimalDataType.e_bs16:
                        return "9999999999999999";
                    case EnumTerimalDataType.e_string:
                        return "99999";
                    default:
                        return "99999";
                }
            }
        }
        public virtual string GetData(Dictionary<int, string[]> RecData, int index, int serinalNumber, EnumTerimalDataType e_type)
        {
            try
            {
                string strReturn = "";
                strReturn = RecData[index][serinalNumber].Split('：')[1];
                switch (e_type)
                {
                    case EnumTerimalDataType.e_byte:
                        return Convert.ToByte(strReturn).ToString();
                    case EnumTerimalDataType.e_int:
                        return Convert.ToInt32(strReturn).ToString();
                    case EnumTerimalDataType.e_float:
                        return Convert.ToSingle(strReturn).ToString();
                    case EnumTerimalDataType.e_double:
                        return Convert.ToDouble(strReturn).ToString();
                    case EnumTerimalDataType.e_datetime:
                        return Convert.ToDateTime(strReturn).ToString();
                    case EnumTerimalDataType.e_bs8:
                        if (strReturn.Length == 8)
                            return strReturn;
                        else
                            return "99999999";
                    case EnumTerimalDataType.e_bs16:
                        if (strReturn.Length == 16)
                            return strReturn;
                        else
                            return "9999999999999999";
                    case EnumTerimalDataType.e_string:
                        return strReturn;
                    default:
                        return "99999";
                }
            }
            catch
            {
                switch (e_type)
                {
                    case EnumTerimalDataType.e_byte:
                        return "99";
                    case EnumTerimalDataType.e_int:
                        return "99999";
                    case EnumTerimalDataType.e_float:
                        return "99999.9";
                    case EnumTerimalDataType.e_double:
                        return "99999.99";
                    case EnumTerimalDataType.e_datetime:
                        return "2000-1-1";
                    case EnumTerimalDataType.e_bs8:
                        return "99999999";
                    case EnumTerimalDataType.e_bs16:
                        return "9999999999999999";
                    case EnumTerimalDataType.e_string:
                        return "99999";
                    default:
                        return "99999";
                }
            }
        }

        public virtual int GetDataInt(Dictionary<int, string[]> RecData, int index, int serinalNumber, EnumTerimalDataType e_type)
        {
            try
            {
                string strReturn = "";
                strReturn = RecData[index][serinalNumber].Split('：')[1];
                switch (e_type)
                {
                    case EnumTerimalDataType.e_int:
                        return Convert.ToInt32(strReturn);
                    case EnumTerimalDataType.e_string:
                        return Convert.ToInt32(strReturn); ;
                    default:
                        return 99999;
                }
            }
            catch
            {
                switch (e_type)
                {

                    case EnumTerimalDataType.e_int:
                        return 99999;

                    case EnumTerimalDataType.e_string:
                        return 99999;
                    default:
                        return 99999;
                }
            }
        }

        public virtual float GetDataFloat(Dictionary<int, string[]> RecData, int index, int serinalNumber, EnumTerimalDataType e_type)
        {
            try
            {
                string strReturn = "";
                strReturn = RecData[index][serinalNumber].Split('：')[1];
                switch (e_type)
                {
                    case EnumTerimalDataType.e_int:
                        return Convert.ToInt32(strReturn);
                    case EnumTerimalDataType.e_string:
                        return Convert.ToInt32(strReturn);
                    case EnumTerimalDataType.e_float:
                        return float.Parse(strReturn);
                    default:
                        return 99999;
                }
            }
            catch
            {
                switch (e_type)
                {

                    case EnumTerimalDataType.e_int:
                        return 99999;

                    case EnumTerimalDataType.e_string:
                        return 99999;
                    case EnumTerimalDataType.e_float:
                        return 9999.9f;
                    default:
                        return 99999;
                }
            }
        }

        public virtual bool GetData(Dictionary<int, string[]> RecData, int index, string sdata)
        {
            try
            {
                bool b1 = false;
                string[] sRecData = RecData[index];
                for (int i = 0; i < sRecData.Length; i++)
                {
                    if (sRecData[i].Contains(sdata))
                    {
                        return true;
                    }
                }
                return b1;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 对事件编号进行处理
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="int_index"></param>
        public void BaseVerifyUnit_EventNum(int int_index)
        {
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        ImportantEvnetCount[i] = int.Parse(GetData(RecData, i, 3, EnumTerimalDataType.e_int));
                        NormalEventCount[i] = int.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_int));
                    }
                    else
                    {
                        TempData[i].Tips = "读事件无回复！";
                        TempData[i].Resoult = "不合格";
                    }
                }
            }
            AddItemsResoult("终端事件计数当前值", TempData);
        }

        public void WriteData(string msg, string cmd, byte afn, byte pn, byte fn)
        {
            MessageAdd(msg, EnumLogType.提示信息);
            RecData.Clear();  //清除上一次数据
            byte[] m_byt_SetData = UsefulMethods.ConvertStringToBytes(cmd);
            TalkResult = TerminalProtocalAdapter.Instance.WriteData(afn, pn, fn, m_byt_SetData, RecData, MaxWaitSeconds_Write);
        }

        public static SocketComm.TcpServerTool TcpServerTool;

        public static void TcpServerInin()
        {
            TcpServerTool = new SocketComm.TcpServerTool(VerifyConfig.Tcp_Ip, VerifyConfig.Tcp_Port);
            TcpServerTool.ExecuteMessageChanged += TcpServerTool_ExecuteMessageChanged;
            TcpServerTool.ReceivedFinshedEvent += TcpServerTool_ReceivedFinshedEvent;
            TcpServerTool.ClientCountChanged += TcpServerTool_ClientCountChanged;
            SendMsgEven.sendMsgEven += SendMsgEven_sendMsgEven;
        }

        private static object SendObjectLock = new object();
        private static void SendMsgEven_sendMsgEven(byte[] GetSetMsg, string Ip_Port)
        {
            lock (SendObjectLock)
            {
                if (TcpServerTool.IsListened)
                {
                    TcpServerTool.Send(Ip_Port, GetSetMsg);
                }
            }

        }

        private static object OnLineLock = new object();
        private static void TcpServerTool_ClientCountChanged(object sender, string e)
        {
            lock (OnLineLock)
            {
                Utility.Log.LogManager.AddMessage(sender as string + "->" + e, Utility.Log.EnumLogSource.检定业务日志, Utility.Log.EnumLevel.Information);
                if (sender as string == "Remove")
                {
                    if (!EquipmentData.TerminalIndexEthernetAddress.ContainsKey(e))
                    {
                        string LogicalAddress = EquipmentData.TerminalIndexEthernetAddress[e];
                        if (EquipmentData.TerminalIndexEthernet.ContainsKey(LogicalAddress))
                        {
                            int index = EquipmentData.TerminalIndexEthernet[LogicalAddress];
                            EquipmentData.MeterGroupInfo.Meters[index].SetProperty("MD_ONLINE", "0");
                        }
                        EquipmentData.TerminalIndexEthernetAddress.Remove(e);
                    }
                }
            }
        }

        private static object EventLock = new object();
        private static void TcpServerTool_ReceivedFinshedEvent(object sender, EventArgs e)
        {
            try
            {
                lock (EventLock)
                {
                    string IP_Port = (e as SocketComm.CommEventArgs).Ip_Port;
                    string StrArgs = (e as CommEventArgs).StrArgs;
                    //等待上线，等待时的报文，只处理心跳包
                    if (!EquipmentData.TerminalIndexEthernetAddress.ContainsKey(IP_Port))
                    {
                        string LogicalAddress = "000000000000";
                        if (GetHaertAddress(StrArgs, ref LogicalAddress))
                        {
                            Utility.Log.LogManager.AddMessage(IP_Port + "->" + LogicalAddress + "上线", Utility.Log.EnumLogSource.检定业务日志, Utility.Log.EnumLevel.Information);
                            Utility.Log.LogManager.AddMessage(IP_Port + "->" + LogicalAddress + "通讯上线心跳包" + StrArgs, Utility.Log.EnumLogSource.检定业务日志, Utility.Log.EnumLevel.Information);
                            if (EquipmentData.TerminalIndexEthernet.ContainsKey(LogicalAddress))
                            {
                                int index = EquipmentData.TerminalIndexEthernet[LogicalAddress];
                                EquipmentData.TerminalIndexEthernetAddress.Add(IP_Port, LogicalAddress);
                                EquipmentData.MeterGroupInfo.Meters[index].SetProperty("MD_ONLINE", "1");
                                Talkers[index].m_bolOnLineStatus = true;
                                Talkers[index].IP_PORT = IP_Port;
                                Talkers[index].Index = index;
                            }
                        }
                    }
                    else if (EquipmentData.TerminalIndexEthernetAddress.ContainsKey(IP_Port))//检定时处理数据
                    {
                        string address = EquipmentData.TerminalIndexEthernetAddress[IP_Port];
                        int index = EquipmentData.TerminalIndexEthernet[address];

                        if (OnMeterInfo.MD_Protocol_Type == "698.45")
                        {
                            Talkers[index].ReceiveDataCache += StrArgs;
                            if (DataValidation698(Talkers[index].ReceiveDataCache))
                            {
                                string msg = SubString698(Talkers[index].ReceiveDataCache);
                                Talkers[index].GetValidFrame_698(msg);
                                Talkers[index].ReceiveDataCache = "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Log.LogManager.AddMessage(ex.ToString(), Utility.Log.EnumLogSource.检定业务日志, Utility.Log.EnumLevel.Warning);
            }


        }

        #region 解析心跳包附带的地址
        public static Analysis_698 Analysiser698ResolveAddress = new Analysis_698();
        public static string AlalysisedDataResolveAddress = "";

        public static string[] AnalysisedStringResolveAddress = new string[0];                                           // 解析出来的字符串
        public static string[] AnalysisedStruceResolveAddress = new string[0];                                          //帧结构
        public static bool GetHaertAddress(string frmStr, ref string address)
        {
            address = "000000000000";
            if (Analysiser698ResolveAddress.Analysis(frmStr, ref AnalysisedStringResolveAddress, ref AlalysisedDataResolveAddress, ref AnalysisedStruceResolveAddress))
            {
                if (AlalysisedDataResolveAddress.Contains("登录") || AlalysisedDataResolveAddress.Contains("心跳"))
                {
                    foreach (var item in AnalysisedStruceResolveAddress)
                    {
                        if (item.Contains("逻辑地址"))
                        {
                            address = AnalysisedStruceResolveAddress[4].Remove(0, 6);
                            break;
                        }
                    }
                    return true;
                }
            }
            return false;
        }


        private static bool DataValidation698(string data)
        {
            data = data.Replace("-", "").Replace(" ", "");//清除所有空格
            if (data.IndexOf("68") == -1) return false;//没有帧头的
            data = data.Substring(data.IndexOf("68"));
            if (data.Length < 10) return false;//没有帧头的
            //获取他的长度
            int ilen = Convert.ToInt32(data.Substring(4, 2) + data.Substring(2, 2), 16) + 2;
            if (data.Length < ilen * 2) return false;
            if (data.Substring(ilen * 2 - 2, 2) != "16") return false;//没有帧尾
            return true;
        }
        private static string SubString698(string data)
        {
            data = data.Replace("-", "").Replace(" ", "");//清除所有空格
            if (data.IndexOf("68") == -1) return "";//没有帧头的
            data = data.Substring(data.IndexOf("68"));
            if (data.Length < 10) return "";//没有帧头的
            //获取他的长度
            int ilen = Convert.ToInt32(data.Substring(4, 2) + data.Substring(2, 2), 16) + 2;
            if (data.Length < ilen * 2) return "";
            if (data.Substring(ilen * 2 - 2, 2) != "16") return "";//没有帧尾
            int endIndex = ilen * 2;
            return data.Substring(0, endIndex);
        }
        #endregion
        private static object ExecuteEventLock = new object();
        private static void TcpServerTool_ExecuteMessageChanged(object sender, string e)
        {
            lock (ExecuteEventLock)
            {
                Utility.Log.LogManager.AddMessage(sender as string + "->" + e, Utility.Log.EnumLogSource.检定业务日志, Utility.Log.EnumLevel.Information);
            }
        }

        /// <summary>
        /// 初始化终端通讯组
        /// </summary>
        /// <param name="p"></param>
        public static void InitTerminalTalks()
        {
            string strAddress = VerifyConfig.Tcp_Ip;
            string strPort = VerifyConfig.Tcp_Port;
            if (strAddress.Length == 0 || strPort.Length == 0)
            {
                //MessageBox.Show("监听地址或端口配置错误,请检查!", "提示");
                return;
            }
            TempData = new TestTempData[meterInfo.Length];
            for (int i = 0; i < meterInfo.Length; i++)
            {
                TempData[i] = new TestTempData();
            }

            if (Talkers == null || Talkers.Length < meterInfo.Length)
            {
                Talkers = new TerminalTalker[meterInfo.Length];
            }
            VerifyDatas = new string[meterInfo.Length];

            #region
            if (TerminalChannelType == Cus_EmChannelType.ChannelEther)
            {
                if (TcpServerTool.IsListened)
                {
                    TcpServerTool.Disconnect();
                }
                TcpServerTool.Open();

            }
            else
            {
                if (TcpServerTool.IsListened)
                {
                    TcpServerTool.Disconnect();
                }
            }
            #endregion



            for (int i = 0; i < meterInfo.Length; i++)
            {
                if (Talkers[i] == null)
                {
                    Talkers[i] = new TerminalTalker();
                }
                Talkers[i].Analysisernew.LenAddr = VerifyConfig.AddressLen;
                Talkers[i].Analysisernew1.LenAddr = VerifyConfig.AddressLen;
                Talkers[i].Framer.LenAddr = VerifyConfig.AddressLen;
                EquipmentData.MeterGroupInfo.Meters[i].SetProperty("MD_ONLINE", "0");//上线状态

                //TODO 终端地址，先测试
                if (meterInfo[i].Address != "" && meterInfo[i].Address.Length >= 8)
                {
                    Talkers[i].Framer.str_Ter_Code = meterInfo[i].Address.Substring(0, 4);
                    Talkers[i].Framer.str_Ter_Address = meterInfo[i].Address.Substring(4);
                }

                try
                {
                    if (TerminalChannelType == Cus_EmChannelType.Channel232 || TerminalChannelType == Cus_EmChannelType.ChannelMaintain)
                    {

                        Rs485Port rs485port = new Rs485Port(i);
                        int MaxTimePerFrame = 100;
                        int MaxTimePerByte = 3000;
                        int tem;
                        if (int.TryParse(meterInfo[i].ProtInfo.MaxTimePerFrame, out tem))
                            MaxTimePerFrame = tem;
                        if (int.TryParse(meterInfo[i].ProtInfo.MaxTimePerByte, out tem))
                            MaxTimePerByte = tem;


                        if (meterInfo[i].ProtInfo.LinkType == LinkType.COM)
                        {
                            rs485port.InitSettingCom(meterInfo[i].ProtInfo.Port, meterInfo[i].ProtInfo.Setting, MaxTimePerFrame, MaxTimePerByte);
                        }
                        else
                        {
                            rs485port.InitSetting(meterInfo[i].ProtInfo.Port, meterInfo[i].ProtInfo.Setting, MaxTimePerFrame, MaxTimePerByte
                                 , meterInfo[i].ProtInfo.IP, meterInfo[i].ProtInfo.RemotePort, meterInfo[i].ProtInfo.StartPort
                                 , "", null);
                        }
                        Talkers[i].My485Port = rs485port;
                    }
                    else
                    {
                        //if (Talkers[i].MyNetPort != null)
                        //{
                        //    Talkers[i].MyNetPort.Close("");
                        //}
                        ////Talkers[i].MyNetPort = null;
                        //Talkers[i].MyNetPort = new SocketComm.CommWithSocketOfServer();
                        //Talkers[i].MyNetPort.ReceivedFinshedEvent += VerifyControler_ReceivedFinshedEvent;
                        //Talkers[i].MyNetPort.ExceptionEvent += VerifyControler_ExceptionEvent;
                        //Talkers[i].MyNetPort.AcceptEvent += VerifyControler_AcceptEvent;
                        //Talkers[i].MyNetPort.StartListening(i, strAddress, int.Parse(strPort) + i);



                    }
                }
                catch
                { }
            }
        }

        public static bool isopenRep = false;
        #region
        ///// <summary>
        ///// 接收到数据完成
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public static void VerifyControler_ReceivedFinshedEvent(object sender, EventArgs e)
        //{
        //    SocketComm.CommWithSocketOfServer comm = sender as SocketComm.CommWithSocketOfServer;
        //    //var a = comm.sock
        //    if (OnMeterInfo.MD_Protocol_Type == "698.45")
        //    {
        //        Talkers[comm.Index].ReceiveDataCache = (e as SocketComm.CommEventArgs).StrArgs;
        //        Talkers[comm.Index].GetValidFrame_698(comm, Talkers[comm.Index].ReceiveDataCache);
        //    }
        //    else
        //    {
        //        Talkers[comm.Index].ReceiveDataCache += (e as SocketComm.CommEventArgs).StrArgs;
        //        Talkers[comm.Index].ReceiveDataCache = Talkers[comm.Index].GetOneFrameAndFillListCacheAll(Talkers[comm.Index].ReceiveDataCache);
        //        for (int i = Talkers[comm.Index].ListCache.Count - 1; i >= 0; i--)
        //        {
        //            Talkers[comm.Index].GetValidFrame(comm, Talkers[comm.Index].ListCache.Dequeue());
        //        }
        //    }
        //}
        ///// <summary>
        ///// 错误事件,掉线
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public static void VerifyControler_ExceptionEvent(object sender, EventArgs e)
        //{
        //    SocketComm.CommWithSocketOfServer comm = sender as SocketComm.CommWithSocketOfServer;
        //    if (Talkers[comm.Index].MyNetPort == null) return;
        //    Talkers[comm.Index].MyNetPort.m_bolOnLineStatus = false;
        //    EquipmentData.MeterGroupInfo.Meters[comm.Index].SetProperty("MD_ONLINE", "0");
        //    meterInfo[comm.Index].Bol_GprsStatus = false;
        //}
        ///// <summary>
        ///// 客户端连接到服务器　上线通知
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public static void VerifyControler_AcceptEvent(object sender, EventArgs e)
        //{
        //    SocketComm.CommWithSocketOfServer comm = sender as SocketComm.CommWithSocketOfServer;
        //    if (Talkers[comm.Index].MyNetPort == null) return;
        //    Talkers[comm.Index].MyNetPort.m_bolOnLineStatus = true;
        //    EquipmentData.MeterGroupInfo.Meters[comm.Index].SetProperty("MD_ONLINE", "1");
        //    meterInfo[comm.Index].Bol_GprsStatus = true;
        //}
        #endregion
        /// <summary>
        /// 检查终端上线情况
        /// </summary>
        /// <returns></returns>
        public static bool CheckTalkersNetStatus()
        {
            bool bStatus = true;
            string strMessage = "";
            while (true)
            {
                strMessage = "";
                bStatus = true;
                for (int i = 0; i < Talkers.Length; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (!Talkers[i].m_bolOnLineStatus)
                        {
                            strMessage += i + 1 + "、";
                            bStatus = false;
                            EquipmentData.MeterGroupInfo.Meters[i].SetProperty("MD_ONLINE", "0");
                        }
                        else
                        {
                            EquipmentData.MeterGroupInfo.Meters[i].SetProperty("MD_ONLINE", "1");
                        }
                    }
                }
                if (!bStatus)
                {
                    strMessage = strMessage.Trim('、') + "终端未上线，正在等待";
                    if (!EquipmentData.Controller.IsCheckVerify) break;
                    Utility.Log.LogManager.AddMessage(strMessage, Utility.Log.EnumLogSource.检定业务日志, Utility.Log.EnumLevel.Warning);
                    //MessageHelper.Instance.AddMessage(msg_CurrentItem.ItemName, EnumMessageType.检定业务日志, 0, strMessage, DateTime.Now.ToString());
                }
                else
                {
                    break;
                }

                Thread.Sleep(1000);
            }

            return bStatus;
        }


        /// <summary>
        /// 终端复位 1为硬件复位，2为数据初始化
        /// </summary>
        /// <param name="iCommand">指令类型，1为硬件复位，2为数据初始化</param>
        public void ResetTerimal(byte bCommand)
        {
            MessageAdd("终端数据区初始化...", EnumLogType.提示信息);
            TalkResult = TerminalProtocalAdapter.Instance.WriteData(1, 0, bCommand, new byte[] { }, RecData, 120000);
            WaitTime("终端复位等待,", VerifyConfig.WaitTime_Reset);
        }

        public void SetTime(DateTime dtTime, int iTmp)
        {
            string str_TimeDiff = ((DateTime.Now - dtTime).TotalMinutes + iTmp).ToString();
            ControlVirtualMeter("Tim" + str_TimeDiff);
            // 终端对时
            MessageAdd("正在终端对时...", EnumLogType.提示信息);
            TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 31, dtTime.ToString(), RecData, MaxWaitSeconds_Write);
        }
        /// <summary>
        /// 设置终端时钟
        /// </summary>
        /// <param name="dtTime"></param>
        /// <param name="iTmp"></param>
        public void SetTime_698(DateTime dtTime, int iTmp)
        {
            string str_TimeDiff = ((DateTime.Now - dtTime).TotalMinutes + iTmp).ToString();

            int ret = 0;
            // 终端对时
            MessageAdd("终端对时到" + dtTime, EnumLogType.提示信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {

                    Talkers[i].Framer698.cTaskData = "06011940000200" + Talkers[i].Framer698.SetDateTimeBCD(dtTime, true) + "00";

                    MessageAdd(i + 1, "明文APDU：" + Talkers[i].Framer698.cTaskData, EnumLogType.提示信息);
                    Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cTaskData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                    MessageAdd(i + 1, "明文APDU：" + Talkers[i].AlalysisedData, EnumLogType.提示信息);

                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetTerminalSetData(i, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cTaskData, ref Talkers[i].Framer698.cOutSID, ref Talkers[i].Framer698.cOutAttachData, ref Talkers[i].Framer698.cOutTaskData, ref Talkers[i].Framer698.cOutTaskMAC);
                    Talkers[i].Framer698.sAPDU = "1001" + GetMiWenLen(Talkers[i].Framer698.cOutTaskData) + Talkers[i].Framer698.cOutTaskData + "00" + Talkers[i].Framer698.cOutSID + "02" + Talkers[i].Framer698.cOutAttachData + "04" + Talkers[i].Framer698.cOutTaskMAC;

                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//应用连接
                }
            }
            ControlVirtualMeter("Tim" + str_TimeDiff);
            dintervaltime = (DateTime.Now - dtTime).TotalMinutes;
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.cOutAttachData = "";
                    Talkers[i].Framer698.cOutMAC = "";
                    if (TalkResult[i] == 0)
                    {

                        Talkers[i].Framer698.cOutAttachData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.cOutMAC = GetData(RecData, i, 5, EnumTerimalDataType.e_string);


                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(i, Talkers[i].Framer698.iKeyState, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cOutAttachData, Talkers[i].Framer698.cOutMAC, ref Talkers[i].Framer698.cOutData);

                        MessageAdd(i + 1, "明文APDU：" + Talkers[i].Framer698.cOutData, EnumLogType.提示信息);
                        Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cOutData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                        MessageAdd(i + 1, "明文APDU：" + Talkers[i].AlalysisedData, EnumLogType.提示信息);

                        if (GetMac(Talkers[i].AnalysisedString, 0, "结果类型") == "00")
                        {
                            TempData[i].Resoult = "合格";
                            TempData[i].Data = dtTime.ToString();
                        }
                        else
                            TempData[i].Resoult = "不合格";
                    }
                    else
                        TempData[i].Resoult = "不合格";
                }
            }
            AddItemsResoult("设置时钟", TempData);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    setData[i] = Talkers[i].Framer698.ReadData_05("40000200");//主站证书
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                        TempData[i].Resoult = "合格";
                    }
                    else
                    {
                        MessageAdd(i + 1, "对时无回复！", EnumLogType.错误信息);
                        TempData[i].Resoult = "不合格";
                    }
                }
            }
            AddItemsResoult("读取时钟", TempData);
        }



        /// <summary>
        /// 计算状态量标准值
        /// </summary>
        /// <param name="p_bol_IsOutput">输出遥信/未输出遥信</param>
        /// <returns></returns>
        public string GetStdStatus(bool p_bol_IsOutput)
        {
            string str_StdStatus = "";
            if (RemoteType == "常开触点")
            {
                if (!p_bol_IsOutput)
                {
                    for (int i = 0; i < RemoteCount; i++)
                    {
                        str_StdStatus = str_StdStatus + "0";
                    }
                }
                else
                {
                    for (int i = 0; i < RemoteCount; i++)
                    {
                        str_StdStatus = str_StdStatus + "1";
                    }
                }
            }
            else
            {
                if (p_bol_IsOutput)
                {
                    for (int i = 0; i < RemoteCount; i++)
                    {
                        str_StdStatus = str_StdStatus + "1";
                    }
                }
                else
                {
                    for (int i = 0; i < RemoteCount; i++)
                    {
                        str_StdStatus = str_StdStatus + "0";
                    }
                }
            }
            return str_StdStatus;
        }


        /// <summary>
        /// 应用连接
        /// </summary>
        public void ConnectLink(bool bCoonect)
        {
            MessageAdd("连接加密机", EnumLogType.提示与流程信息);
            int ret = TerminalProtocol.Encryption.IEncryptionFunction698.ConnectDevice(GetYaoJian(), VerifyConfig.Dog_IP, VerifyConfig.Dog_Prot, VerifyConfig.Dog_Overtime);
            if (ret != 0)//czx0723
            {
                MessageAdd("加密机连接失败，返回" + ret, EnumLogType.错误信息);
                return;
            }

            if (Stop) return;
            for (int jj = 0; jj < 1; jj++)
            {
                MessageAdd("正在读取安全模式参数", EnumLogType.提示与流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        setData[i] = Talkers[i].Framer698.ReadData_05("F1010200");
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string) == "1" ? "启动安全模式参数" : "不启用安全模式参数";
                            TempData[i].Resoult = "合格";
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "无回复消息！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                        }

                    }
                }
                testTempData.Add(TempData);
                if (bCoonect) AddItemsResoult("读取安全模式参数", TempData);

                //WaitTime("等待", 5);

                MessageAdd("正在读取ESAM信息", EnumLogType.提示与流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05021C03F1000200F1000400F100070000";
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//ESAM信息
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.cTESAMNO = GetData(RecData, i, 6, EnumTerimalDataType.e_string);
                            TempData[i].Data = "ESAM序列号:" + Talkers[i].Framer698.cTESAMNO;
                            TempData[i].Resoult = "合格";
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "无回复消息！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                        }

                    }
                }
                testTempData.Add(TempData);
                if (bCoonect) AddItemsResoult("ESAM序列号", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.KeyVerison = GetData(RecData, i, 9, EnumTerimalDataType.e_string);
                            TempData[i].Data = "对称密钥版本:" + Talkers[i].Framer698.KeyVerison;
                            if (Talkers[i].Framer698.KeyVerison == "00000000000000000000000000000000")
                                Talkers[i].Framer698.iKeyState = 0;
                            else
                                Talkers[i].Framer698.iKeyState = 1;
                            TempData[i].Resoult = "合格";
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "无回复消息！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                testTempData.Add(TempData);

                string intesamtpe = "";
                for (int i = 0; i < MeterNumber - 1; i++)
                {
                    intesamtpe += Talkers[i].Framer698.iKeyState.ToString() + ",";
                }
                intesamtpe += Talkers[MeterNumber - 1].Framer698.iKeyState.ToString();

                ControlVirtualMeter("Cmd,intesamtype," + intesamtpe);
                if (bCoonect) AddItemsResoult("对称密钥版本", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.cASCTR = Convert.ToInt32(GetData(RecData, i, 12, EnumTerimalDataType.e_string));
                            Talkers[i].Framer698.ReportCount = Convert.ToInt32(GetData(RecData, i, 13, EnumTerimalDataType.e_string));
                            Talkers[i].Framer698.BroadCastCount = Convert.ToInt32(GetData(RecData, i, 14, EnumTerimalDataType.e_string));
                            TempData[i].Data = Talkers[i].Framer698.cASCTR.ToString() + "," + Talkers[i].Framer698.ReportCount + "," + Talkers[i].Framer698.BroadCastCount;
                            TempData[i].Resoult = "合格";
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "无回复消息！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                testTempData.Add(TempData);
                if (bCoonect) AddItemsResoult("计数器", TempData);
                //Thread.Sleep(5000);
                MessageAdd("正在读取主站证书", EnumLogType.提示与流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        setData[i] = Talkers[i].Framer698.ReadData_05("F1000c00");//主站证书
                    }
                }
                //System.Diagnostics.Debug.Print("开始时间：" + DateTime.Now);
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                //System.Diagnostics.Debug.Print("结束时间：" + DateTime.Now);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.cMasterCert = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                            if (Talkers[i].Framer698.cMasterCert.Length > 32)
                            {
                                TempData[i].Data = "主站证书(部分):" + Talkers[i].Framer698.cMasterCert.Substring(0, 32);
                                TempData[i].Resoult = "合格";
                            }
                            else
                                TempData[i].Resoult = "不合格";
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "无回复消息！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                testTempData.Add(TempData);
                if (bCoonect) AddItemsResoult("主站证书", TempData);
                MessageAdd("正在读取终端证书", EnumLogType.提示与流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        setData[i] = Talkers[i].Framer698.ReadData_05("F1000a00");//终端证书
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.cTerminalCert = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                            if (Talkers[i].Framer698.cTerminalCert.Length > 32)
                            {
                                TempData[i].Data = "终端证书(部分):" + Talkers[i].Framer698.cTerminalCert.Substring(0, 32);
                                TempData[i].Resoult = "合格";
                            }
                            else
                                TempData[i].Resoult = "不合格";
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "无回复消息！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                testTempData.Add(TempData);
                if (bCoonect) AddItemsResoult("终端证书", TempData);
            }
            MessageAdd("建立应用连接", EnumLogType.提示与流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.cASCTR++;
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_InitSession(i, Talkers[i].Framer698.iKeyState, Talkers[i].Framer698.cTESAMNO, (Talkers[i].Framer698.cASCTR).ToString("x8"), "01", Talkers[i].Framer698.cMasterCert, ref Talkers[i].Framer698.cOutRandHost, ref Talkers[i].Framer698.cOutSessionInit, ref Talkers[i].Framer698.cOutSign);
                    Talkers[i].Framer698.sAPDU = "02040016FFFFFFFFC00000000001FFFE00000000000000000000000008000800011f4000001c200320" + Talkers[i].Framer698.cOutSessionInit + "40" + Talkers[i].Framer698.cOutSign + "00";
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//应用连接
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            if (Stop) return;

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        Talkers[i].Framer698.cSessionData = GetData(RecData, i, 19, EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.cSign = GetData(RecData, i, 20, EnumTerimalDataType.e_string);
                        if (Talkers[i].Framer698.cSign.Length > 32)
                        {
                            TempData[i].Data = "会话协商数据签名:" + Talkers[i].Framer698.cSign.Substring(0, 32);
                            TempData[i].Resoult = "合格";
                        }
                        else
                            TempData[i].Resoult = "不合格";
                    }
                    else
                    {
                        MessageAdd("终端" + (i + 1) + "无回复消息！", EnumLogType.错误信息);

                        TempData[i].Resoult = "不合格";
                    }
                }
            }
            testTempData.Add(TempData);
            if (Stop) return;

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifySession(i, Talkers[i].Framer698.iKeyState, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRandHost, Talkers[i].Framer698.cSessionData, Talkers[i].Framer698.cSign, Talkers[i].Framer698.cTerminalCert, ref Talkers[i].Framer698.cOutSessionKey);
                    if (ret == 0)
                        TempData[i].Resoult = "合格";
                    else
                        TempData[i].Resoult = "不合格";
                }
            }
            testTempData.Add(TempData);
            if (Stop) return;

            Dog_ConnectTime = DateTime.Now;
            if (bCoonect) AddItemsResoult("建立应用连接", TempData);
        }

        public void ConnectLink2(bool bCoonect)
        {
            #region 应用连接
            int ret = 0;
            MessageAdd("建立应用连接", EnumLogType.提示与流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {

                    Talkers[i].Framer698.cASCTR++;
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_InitSession(i, Talkers[i].Framer698.iKeyState, Talkers[i].Framer698.cTESAMNO, (Talkers[i].Framer698.cASCTR).ToString("x8"), "01", Talkers[i].Framer698.cMasterCert, ref Talkers[i].Framer698.cOutRandHost, ref Talkers[i].Framer698.cOutSessionInit, ref Talkers[i].Framer698.cOutSign);
                    Talkers[i].Framer698.sAPDU = "02040016FFFFFFFFC00000000001FFFE00000000000000000000000008000800011f4000001c200320" + Talkers[i].Framer698.cOutSessionInit + "40" + Talkers[i].Framer698.cOutSign + "00";
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//应用连接
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {

                        Talkers[i].Framer698.cSessionData = GetData(RecData, i, 19, EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.cSign = GetData(RecData, i, 20, EnumTerimalDataType.e_string);
                        if (Talkers[i].Framer698.cSign.Length > 32)
                            TempData[i].Data = "会话协商数据签名:" + Talkers[i].Framer698.cSign.Substring(0, 32);
                        else
                            TempData[i].Data = "不合格";
                    }
                    else
                    {
                        MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
                        TempData[i].Resoult = "不合格";
                    }

                }
            }
            testTempData.Add(TempData);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifySession(i, Talkers[i].Framer698.iKeyState, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRandHost, Talkers[i].Framer698.cSessionData, Talkers[i].Framer698.cSign, Talkers[i].Framer698.cTerminalCert, ref Talkers[i].Framer698.cOutSessionKey);
                    if (ret == 0)
                        TempData[i].Resoult = "合格";
                    else
                        TempData[i].Resoult = "不合格";
                }
            }
            Dog_ConnectTime = DateTime.Now;
            testTempData.Add(TempData);
            if (bCoonect) AddItemsResoult("建立应用连接", TempData);

            //if (bCoonect)
            //MessageHelper.Instance.AddVerifyData(0, m_int_SchemeId, m_str_ItemName, "建立应用连接", m_str_ParameterID, SubItemIndex++, m_str_Conclusions, m_str_VerifyDatas, 0, 0, m_bol_IsLastSubItem);
            #endregion
        }
        #endregion

        #region 设置或读取终端数据
        public void SetSessionData_698(string cTaskData, string itemName)
        {
            int ret = 0;
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.cTaskData = cTaskData.Replace(" ", "");

                    MessageAdd(i + 1, Talkers[i].Framer698.cTaskData, EnumLogType.提示信息);
                    Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cTaskData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                    MessageAdd(i + 1, Talkers[i].AlalysisedData, EnumLogType.提示信息);
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetSessionData(i, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, 3, Talkers[i].Framer698.cTaskData, ref Talkers[i].Framer698.cOutSID, ref Talkers[i].Framer698.cOutAttachData, ref Talkers[i].Framer698.cOutTaskData, ref Talkers[i].Framer698.cOutTaskMAC);
                    Talkers[i].Framer698.sAPDU = "1001" + GetMiWenLen(Talkers[i].Framer698.cOutTaskData) + Talkers[i].Framer698.cOutTaskData + "00" + Talkers[i].Framer698.cOutSID + "02" + Talkers[i].Framer698.cOutAttachData + "04" + Talkers[i].Framer698.cOutTaskMAC;

                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//应用连接
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            //InitializeConclusion();
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.cOutAttachData = "";
                    Talkers[i].Framer698.cOutMAC = "";
                    if (TalkResult[i] == 0)
                    {
                        Talkers[i].Framer698.cOutAttachData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.cOutMAC = GetMac(RecData, i);

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(i, Talkers[i].Framer698.iKeyState, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cOutAttachData, Talkers[i].Framer698.cOutMAC, ref Talkers[i].Framer698.cOutData);
                        MessageAdd(i + 1, "明文APDU：" + Talkers[i].Framer698.cOutData, EnumLogType.提示信息);
                        Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cOutData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                        MessageAdd(i + 1, Talkers[i].AlalysisedData, EnumLogType.提示信息);

                        if (GetMac(Talkers[i].AnalysisedString, 0, "结果类型") != "00")
                            TempData[i].Resoult = "不合格";
                        //else
                        //    Resoult[i] = "不合格";
                    }
                    else
                        TempData[i].Resoult = "不合格";
                }
            }
            AddItemsResoult(itemName, TempData);
        }

        /// <summary>
        /// 明文加验证码(MAC)组装APDU
        /// </summary>
        /// <param name="cTaskData"></param>
        /// <param name="itemName"></param>
        public void SetMingSessionData_698(string cTaskData, string itemName)
        {
            int ret = 0;
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.cTaskData = cTaskData.Replace(" ", "");

                    MessageAdd(i + 1, Talkers[i].Framer698.cTaskData, EnumLogType.提示信息);
                    Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cTaskData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                    MessageAdd(i + 1, Talkers[i].AlalysisedData, EnumLogType.提示信息);
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetSessionData(i, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, 3, Talkers[i].Framer698.cTaskData, ref Talkers[i].Framer698.cOutSID, ref Talkers[i].Framer698.cOutAttachData, ref Talkers[i].Framer698.cOutTaskData, ref Talkers[i].Framer698.cOutTaskMAC);
                    Talkers[i].Framer698.sAPDU = "1000" + GetMiWenLen(Talkers[i].Framer698.cTaskData) + Talkers[i].Framer698.cTaskData + "00" + Talkers[i].Framer698.cOutSID + "02" + Talkers[i].Framer698.cOutAttachData + "04" + Talkers[i].Framer698.cOutTaskMAC;

                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//应用连接
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            //InitializeConclusion();
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.cOutAttachData = "";
                    Talkers[i].Framer698.cOutMAC = "";
                    if (TalkResult[i] == 0)
                    {
                        Talkers[i].Framer698.cOutAttachData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.cOutMAC = GetMac(RecData, i);

                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cOutAttachData, Talkers[i].Framer698.cOutMAC, ref Talkers[i].Framer698.cOutData);
                        MessageAdd(i + 1, "明文APDU：" + Talkers[i].Framer698.cOutData, EnumLogType.提示信息);
                        Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cOutData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                        MessageAdd(i + 1, Talkers[i].AlalysisedData, EnumLogType.提示信息);
                        if (ret != 0)
                        {
                            TempData[i].Tips = "验证码校验失败,ret=" + ret;
                        }
                        if (GetMac(Talkers[i].AnalysisedString, 0, "结果类型") != "00")
                            TempData[i].Resoult = "不合格";
                        //else
                        //    Resoult[i] = "不合格";
                    }
                    else
                        TempData[i].Resoult = "不合格";
                }
            }
            AddItemsResoult(itemName, TempData);
        }

        public void SetData_698_No(string cTaskData, string itemName)
        {
            //WriteLog(itemName, 0);
            for (int i = 0; i < meterInfo.Length; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.sAPDU = cTaskData.Replace(" ", "");

                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < meterInfo.Length; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "00";
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                        if (GetData(RecData, i, 5, EnumTerimalDataType.e_string) != "00")
                            TempData[i].Resoult = "不合格";
                    }
                    else
                        TempData[i].Resoult = "不合格";
                }
            }
            AddItemsResoult(itemName, TempData);
        }
        /// <summary>
        /// 设置数据通用方法
        /// </summary>
        /// <param name="cTaskData"></param>
        /// <param name="itemName"></param>
        public void SetData_698(string cTaskData, string itemName)
        {
            int ret = 0;
            MessageAdd(itemName, EnumLogType.提示与流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.cTaskData = cTaskData.Replace(" ", "");


                    MessageAdd(i + 1, "明文APDU：" + Talkers[i].Framer698.cTaskData, EnumLogType.提示信息);
                    Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cTaskData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                    MessageAdd(i + 1, Talkers[i].AlalysisedData, EnumLogType.提示信息);

                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetTerminalSetData(i, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cTaskData, ref Talkers[i].Framer698.cOutSID, ref Talkers[i].Framer698.cOutAttachData, ref Talkers[i].Framer698.cOutTaskData, ref Talkers[i].Framer698.cOutTaskMAC);
                    Talkers[i].Framer698.sAPDU = "1001" + GetMiWenLen(Talkers[i].Framer698.cOutTaskData) + Talkers[i].Framer698.cOutTaskData + "00" + Talkers[i].Framer698.cOutSID + "02" + Talkers[i].Framer698.cOutAttachData + "04" + Talkers[i].Framer698.cOutTaskMAC;

                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//应用连接
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.cOutAttachData = "";
                    Talkers[i].Framer698.cOutMAC = "";
                    TempData[i].StdData = "00";
                    if (TalkResult[i] == 0)
                    {

                        Talkers[i].Framer698.cOutAttachData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.cOutMAC = GetMac(RecData, i);
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(i, Talkers[i].Framer698.iKeyState, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cOutAttachData, Talkers[i].Framer698.cOutMAC, ref Talkers[i].Framer698.cOutData);

                        MessageAdd(i + 1, "明文APDU：" + Talkers[i].Framer698.cOutData, EnumLogType.提示信息);

                        Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cOutData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                        MessageAdd(i + 1, Talkers[i].AlalysisedData, EnumLogType.提示信息);

                        TempData[i].Data = GetMac(Talkers[i].AnalysisedString, 0, "结果类型");
                        if (GetMac(Talkers[i].AnalysisedString, 0, "结果类型") == "00")
                            TempData[i].Resoult = "合格";
                        else
                        {
                            TempData[i].Tips = "通讯结果不正确";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                    else
                    {
                        TempData[i].Resoult = "不合格";
                        TempData[i].Tips = "通讯返回不正确";
                    }
                }
            }
            AddItemsResoult(itemName, TempData);
        }

        public void SetData_6982(string cTaskData, string itemName)
        {
            int ret = 0;
            MessageAdd(itemName, EnumLogType.提示与流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.cTaskData = cTaskData.Replace(" ", "");


                    MessageAdd(i + 1, "明文APDU：" + Talkers[i].Framer698.cTaskData, EnumLogType.提示信息);
                    Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cTaskData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                    MessageAdd(i + 1, Talkers[i].AlalysisedData, EnumLogType.提示信息);

                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetTerminalSetData(i, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cTaskData, ref Talkers[i].Framer698.cOutSID, ref Talkers[i].Framer698.cOutAttachData, ref Talkers[i].Framer698.cOutTaskData, ref Talkers[i].Framer698.cOutTaskMAC);
                    Talkers[i].Framer698.sAPDU = "1001" + GetMiWenLen(Talkers[i].Framer698.cOutTaskData) + Talkers[i].Framer698.cOutTaskData + "00" + Talkers[i].Framer698.cOutSID + "02" + Talkers[i].Framer698.cOutAttachData + "04" + Talkers[i].Framer698.cOutTaskMAC;

                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//应用连接
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.cOutAttachData = "";
                    Talkers[i].Framer698.cOutMAC = "";
                    if (TalkResult[i] == 0)
                    {

                        Talkers[i].Framer698.cOutAttachData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.cOutMAC = GetMac(RecData, i);


                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(i, Talkers[i].Framer698.iKeyState, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cOutAttachData, Talkers[i].Framer698.cOutMAC, ref Talkers[i].Framer698.cOutData);
                        MessageAdd(i + 1, "明文APDU：" + Talkers[i].Framer698.cOutData, EnumLogType.提示信息);
                        Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cOutData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                        MessageAdd(i + 1, Talkers[i].AlalysisedData, EnumLogType.提示信息);

                        //TODO 这里判断好像有点问题
                        if (GetMac(Talkers[i].AnalysisedString, 0, "结果类型") == "00")
                            TempData[i].Resoult = "合格";
                        else
                            TempData[i].Resoult = "不合格";
                    }
                    else
                        TempData[i].Resoult = "不合格";
                }
            }

            AddItemsResoult(itemName, TempData);
        }

        /// <summary>
        /// 设置数据通用方法
        /// </summary>
        /// <param name="cTaskData"></param>
        /// <param name="itemName"></param>
        public void SetData_698_jiaocai(string cTaskData, string itemName)
        {
            int ret = 0;
            MessageAdd(itemName, EnumLogType.提示与流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.cTaskData = cTaskData;


                    MessageAdd(i + 1, "明文APDU：" + Talkers[i].Framer698.cTaskData, EnumLogType.提示信息);
                    Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cTaskData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                    MessageAdd(i + 1, Talkers[i].AlalysisedData, EnumLogType.提示信息);

                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetTerminalSetData(i, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cTaskData, ref Talkers[i].Framer698.cOutSID, ref Talkers[i].Framer698.cOutAttachData, ref Talkers[i].Framer698.cOutTaskData, ref Talkers[i].Framer698.cOutTaskMAC);
                    Talkers[i].Framer698.sAPDU = "1001" + GetMiWenLen(Talkers[i].Framer698.cOutTaskData) + Talkers[i].Framer698.cOutTaskData + "00" + Talkers[i].Framer698.cOutSID + "02" + Talkers[i].Framer698.cOutAttachData + "04" + Talkers[i].Framer698.cOutTaskMAC;

                    setData[i] = Talkers[i].Framer698.ReadData_jiaocai(Talkers[i].Framer698.sAPDU);//应用连接
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.cOutAttachData = "";
                    Talkers[i].Framer698.cOutMAC = "";
                    if (TalkResult[i] == 0)
                    {

                        Talkers[i].Framer698.cOutAttachData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.cOutMAC = GetMac(RecData, i);


                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(i, Talkers[i].Framer698.iKeyState, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cOutAttachData, Talkers[i].Framer698.cOutMAC, ref Talkers[i].Framer698.cOutData);


                        MessageAdd(i + 1, "明文APDU：" + Talkers[i].Framer698.cOutData, EnumLogType.提示信息);

                        Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cOutData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                        MessageAdd(i + 1, Talkers[i].AlalysisedData, EnumLogType.提示信息);

                        if (GetMac(Talkers[i].AnalysisedString, 0, "结果类型") == "00")
                            TempData[i].Resoult = "合格";
                        else
                            TempData[i].Resoult = "不合格";
                    }
                    else
                        TempData[i].Resoult = "不合格";
                }
            }
            AddItemsResoult(itemName, TempData);
        }

        /// <summary>
        /// 读终端事件
        /// </summary>
        /// <param name="p_int_EventCount"></param>
        /// <param name="p_bol_IsImportant"></param>
        public string[] ReadTerminalEvent(int p_int_TableNo, bool p_bol_IsImportant, string strEventErr)
        {
            if (TerminalChannelType == Cus_EmChannelType.ChannelEther || TerminalChannelType == Cus_EmChannelType.ChannelGPRS)
                CheckTalkersNetStatus();

            string[] str_Return = new string[0];
            int int_ImportantEventCount = 0;            // 重要事件个数
            int int_NormalEventCount = 0;               // 普通事件个数

            int intResult = -1;

            TerminalTalker talker = Talkers[p_int_TableNo];

            // 读取终端事件计数器
            talker.AnalysisedString = new string[0];
            SendData = talker.Framer.ReadData_Afn12(12, 0, 7);

            talker.Analysisernew.Analysis(SendData, 0, ref talker.AnalysisedString, ref talker.AlalysisedData, ref talker.AnalysisedStruce);
            MessageAdd(p_int_TableNo + 1, "发送：" + SendData + "\r\n" + talker.AlalysisedData, EnumLogType.提示信息);

            SendDataBytes = UsefulMethods.ConvertStringToBytes(SendData);
            string str_OutFrame = "";
            if (TerminalChannelType == Cus_EmChannelType.Channel232 || TerminalChannelType == Cus_EmChannelType.ChannelMaintain)
            {
                intResult = talker.My485Port.SendData_(SendDataBytes, out SendDataBytes);
                str_OutFrame = UsefulMethods.ConvertBytesToString(OutFrame);

                talker.Analysisernew.Analysis(str_OutFrame, 0, ref talker.AnalysisedString, ref talker.AlalysisedData, ref talker.AnalysisedStruce);
                MessageAdd(p_int_TableNo + 1, "接受：" + str_OutFrame + "\r\n" + talker.AlalysisedData, EnumLogType.提示信息);
            }
            else if (TerminalChannelType == Cus_EmChannelType.ChannelEther || TerminalChannelType == Cus_EmChannelType.ChannelGPRS)
            {
                talker.ReceiveData = "";
                talker.AFn = 12;
                talker.Fn = 7;
                //talker.MyNetPort.SendData(SendDataBytes);
                SendMsgEven.GetSeriPostMsg(SendDataBytes, talker.IP_PORT);
                int p_int_Time = 20;
                while (p_int_Time != 0)
                {
                    Thread.Sleep(1000);
                    p_int_Time--;
                    if (talker.ReceiveData != "")
                    {
                        p_int_Time = 0;
                    }
                }
                if (talker.ReceiveData.Length > 0)
                {
                    str_OutFrame = talker.ReceiveData;
                    intResult = 0;
                }

            }
            if (intResult == 0)
            {
                talker.Analysisernew.Analysis(str_OutFrame, 0, ref talker.AnalysisedString, ref talker.AlalysisedData, ref talker.AnalysisedStruce);
                if (talker.AnalysisedString.Length >= 5)
                {
                    int_ImportantEventCount = GetData(talker.AnalysisedString, 3, EnumTerimalDataType.e_string) == "" ? 0 : int.Parse(GetData(talker.AnalysisedString, 3, EnumTerimalDataType.e_int));
                    int_NormalEventCount = GetData(talker.AnalysisedString, 4, EnumTerimalDataType.e_string) == "" ? 0 : int.Parse(GetData(talker.AnalysisedString, 4, EnumTerimalDataType.e_int));
                }
            }

            while (int_NormalEventCount > 0)
            {
                // 读取终端最后一条事件
                talker.AnalysisedString = new string[0];
                if (p_bol_IsImportant)
                {
                    if (int_ImportantEventCount != 0)
                    {
                        SendData = Convert.ToString(int_ImportantEventCount - 1, 16).PadLeft(2, '0') + Convert.ToString(int_ImportantEventCount, 16).PadLeft(2, '0');
                        SendDataBytes = UsefulMethods.ConvertStringToBytes(SendData);
                        SendData = talker.Framer.ReadData(14, 0, 1, SendDataBytes);
                    }
                }
                else
                {
                    if (int_NormalEventCount != 0)
                    {
                        SendData = Convert.ToString(int_NormalEventCount - 1, 16).PadLeft(2, '0') + Convert.ToString(int_NormalEventCount, 16).PadLeft(2, '0');
                        SendDataBytes = UsefulMethods.ConvertStringToBytes(SendData);
                        SendData = talker.Framer.ReadData(14, 0, 2, SendDataBytes);
                    }
                }
                SendDataBytes = UsefulMethods.ConvertStringToBytes(SendData);
                intResult = -1;
                str_OutFrame = "";
                if (TerminalChannelType == Cus_EmChannelType.Channel232 || TerminalChannelType == Cus_EmChannelType.ChannelMaintain)
                {
                    intResult = talker.My485Port.SendData_(SendDataBytes, out OutFrame);
                    str_OutFrame = UsefulMethods.ConvertBytesToString(OutFrame);
                }
                else if (TerminalChannelType == Cus_EmChannelType.ChannelEther | TerminalChannelType == Cus_EmChannelType.ChannelGPRS)
                {
                    Thread.Sleep(500);

                    talker.ReceiveData = "";
                    talker.AFn = 14;
                    talker.Fn = 2;
                    SendMsgEven.GetSeriPostMsg(SendDataBytes, talker.IP_PORT);

                    int p_int_Time = 20;
                    while (p_int_Time != 0)
                    {
                        Thread.Sleep(1000);
                        p_int_Time--;
                        if (talker.ReceiveData != "")
                        {
                            p_int_Time = 0;
                        }
                    }
                    if (talker.ReceiveData.Length > 0)
                    {
                        str_OutFrame = talker.ReceiveData;
                        intResult = 0;
                    }

                }

                if (intResult == 0)
                {
                    talker.Analysisernew.Analysis(str_OutFrame, 0, ref talker.AnalysisedString, ref talker.AlalysisedData, ref talker.AnalysisedStruce);
                    str_Return = talker.AnalysisedString;

                    if (str_Return.Length > 11)
                    {
                        if (GetData(str_Return, 7, EnumTerimalDataType.e_string) == strEventErr)
                        {
                            int_NormalEventCount = 0;
                        }
                    }
                }
                int_NormalEventCount--;
            }
            return str_Return;
        }
        /// <summary>
        /// 数据区初始化
        /// </summary>
        /// <param name="bCommand">1硬件复位,2数据初始化,3恢复出厂设置</param>
        public void ResetTerimal_698(byte bCommand)
        {
            int ret = 0;
            string sTmp = "";
            if (bCommand == 3)
                sTmp = "恢复出厂设置";
            else if (bCommand == 2)
                sTmp = "数据初始化";
            else if (bCommand == 6)
                sTmp = "需量初始化";
            else
            {
                sTmp = "硬件复位";
                EquipmentData.IsEthernetOnlineNow = true;
            }
          
            MessageAdd(sTmp, EnumLogType.提示与流程信息);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (bCommand == 3)
                        Talkers[i].Framer698.cTaskData = "07013743000400010000";
                    else if (bCommand == 2)
                        Talkers[i].Framer698.cTaskData = "070137430003000000";
                    else if (bCommand == 6)
                        Talkers[i].Framer698.cTaskData = "070137430006000000";
                    else
                        Talkers[i].Framer698.cTaskData = "07013B430001000000";

                    MessageAdd(i + 1, "明文APDU：" + Talkers[i].Framer698.cTaskData, EnumLogType.提示信息);
                    Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cTaskData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                    MessageAdd(i + 1, Talkers[i].AlalysisedData, EnumLogType.提示信息);

                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetTerminalSetData(i, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cTaskData, ref Talkers[i].Framer698.cOutSID, ref Talkers[i].Framer698.cOutAttachData, ref Talkers[i].Framer698.cOutTaskData, ref Talkers[i].Framer698.cOutTaskMAC);
                    Talkers[i].Framer698.sAPDU = "1001" + GetMiWenLen(Talkers[i].Framer698.cOutTaskData) + Talkers[i].Framer698.cOutTaskData + "00" + Talkers[i].Framer698.cOutSID + "02" + Talkers[i].Framer698.cOutAttachData + "04" + Talkers[i].Framer698.cOutTaskMAC;

                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//应用连接
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {

                        Talkers[i].Framer698.cOutAttachData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.cOutMAC = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(i, Talkers[i].Framer698.iKeyState, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cOutAttachData, Talkers[i].Framer698.cOutMAC, ref Talkers[i].Framer698.cOutData);
                        MessageAdd(i + 1, "明文APDU：" + Talkers[i].Framer698.cOutData, EnumLogType.提示信息);

                        Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cOutData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                        MessageAdd(i + 1, Talkers[i].AlalysisedData, EnumLogType.提示信息);

                        if (GetMac(Talkers[i].AnalysisedString, 0, "结果类型") == "00")
                            TempData[i].Resoult = "合格";
                        else
                            TempData[i].Resoult = "不合格";
                    }
                    else
                        TempData[i].Resoult = "不合格";
                }
            }
            AddItemsResoult(sTmp, TempData);
            WaitTime("初始化等待", 180);
            EquipmentData.IsEthernetOnlineNow = false;
        }

        /// <summary>
        /// 数据区初始化
        /// </summary>
        /// <param name="bCommand">恢复出厂设置,交采数据初始化,硬件复位</param>
        public void ResetTerimal_6982(byte bCommand)
        {
            int ret = 0;
            string sTmp = "";
            if (bCommand == 3)
                sTmp = "恢复出厂设置";
            else if (bCommand == 2)
                sTmp = "交采数据初始化";
            else
            {
                sTmp = "硬件复位";
                EquipmentData.IsEthernetOnlineNow = true;
            }
            MessageAdd(sTmp, EnumLogType.提示与流程信息);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (bCommand == 3)
                        Talkers[i].Framer698.cTaskData = "07013743000400010000";
                    else if (bCommand == 2)
                        Talkers[i].Framer698.cTaskData = "070137430003000000";
                    else
                        Talkers[i].Framer698.cTaskData = "07013B430001000000";

                    MessageAdd(i + 1, "明文APDU：" + Talkers[i].Framer698.cTaskData, EnumLogType.提示信息);
                    Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cTaskData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                    MessageAdd(i + 1, Talkers[i].AlalysisedData, EnumLogType.提示信息);

                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_GetTerminalSetData(i, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cTaskData, ref Talkers[i].Framer698.cOutSID, ref Talkers[i].Framer698.cOutAttachData, ref Talkers[i].Framer698.cOutTaskData, ref Talkers[i].Framer698.cOutTaskMAC);
                    Talkers[i].Framer698.sAPDU = "1001" + GetMiWenLen(Talkers[i].Framer698.cOutTaskData) + Talkers[i].Framer698.cOutTaskData + "00" + Talkers[i].Framer698.cOutSID + "02" + Talkers[i].Framer698.cOutAttachData + "04" + Talkers[i].Framer698.cOutTaskMAC;

                    setData[i] = Talkers[i].Framer698.ReadData_jiaocai(Talkers[i].Framer698.sAPDU);//应用连接
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {

                        Talkers[i].Framer698.cOutAttachData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                        Talkers[i].Framer698.cOutMAC = GetData(RecData, i, 5, EnumTerimalDataType.e_string);


                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyTerminalData(i, Talkers[i].Framer698.iKeyState, 3, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutSessionKey, Talkers[i].Framer698.cOutAttachData, Talkers[i].Framer698.cOutMAC, ref Talkers[i].Framer698.cOutData);

                        MessageAdd(i + 1, "明文APDU：" + Talkers[i].Framer698.cOutData, EnumLogType.提示信息);

                        Talkers[i].Analysiser698.Analysis_apdu(Talkers[i].Framer698.cOutData, ref Talkers[i].AnalysisedString, ref Talkers[i].AlalysisedData);
                        MessageAdd(i + 1, Talkers[i].AlalysisedData, EnumLogType.提示信息);

                        if (GetMac(Talkers[i].AnalysisedString, 0, "结果类型") == "00")
                            TempData[i].Resoult = "合格";
                        else
                            TempData[i].Resoult = "不合格";
                    }
                    else
                        TempData[i].Resoult = "不合格";
                }
            }
            AddItemsResoult(sTmp, TempData);

            WaitTime("初始化等待", 180);
            EquipmentData.IsEthernetOnlineNow = false;
        }
        #endregion

        #region 数据处理
        /// <summary>
        /// 对需要返回确认的数据进行处理
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="int_index"></param>
        public void BaseVerifyUnit_ReturnOk(string Name, int int_index)
        {
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                        {
                            MessageAdd("终端" + (i + 1) + Name + "设置失败！", EnumLogType.错误信息);
                            TempData[i].Resoult = Core.Helper.Const.不合格;
                            TempData[i].Data = Name + "设置失败";
                        }
                        else
                        {
                            TempData[i].Data = "成功";
                        }
                    }
                    else
                    {
                        MessageAdd("终端" + (i + 1) + Name + "设置无回复！", EnumLogType.错误信息);
                        TempData[i].Resoult = Core.Helper.Const.不合格;
                        TempData[i].Data = Name + "设置无回复";
                    }
                }
            }
            AddItemsResoult(Name, TempData);
        }

        /// <summary>
        /// 判断表位报警状态
        /// </summary>
        /// <param name="bol_Qualified"></param>
        /// <returns></returns>
        public bool CheckControPass(bool[] bol_Qualified)
        {

            for (int i = 0; i < bol_Qualified.Length; i++)
            {
                if (meterInfo[i].YaoJianYn && bol_Qualified[i] == false)
                {
                    return false;
                }
            }

            return true;
        }



        #endregion

        #region 检定基类方法

        /// <summary>
        /// 开始检定基类--设置一些基本参数，读取地址等一些基础操作
        /// </summary>
        public void StartVerify()
        {
            ControlVirtualMeter("Cmd,RunFlag,0");
            ControlVirtualMeter("BTL2400");
            ControlVirtualMeter("Cmd,boolIsReturn,1");
            ControlVirtualMeter("Cmd,ProtocalType,1");
            ControlVirtualMeter("Cmd,MeterTimeCheck,0");

            // 设置所有终端通讯类信息
            for (int i = 0; i < meterInfo.Length; i++)
            {
                TestMeterInfo ti = meterInfo[i];
                if (!ti.YaoJianYn) continue;
                Talkers[i].Framer.bol_Mac = false;
                if (ti.Address != "")
                {
                    Talkers[i].Framer.str_Ter_Code = ti.Address.Substring(0, 4);
                    if (VerifyConfig.IsHexAddress)
                    {
                        if (VerifyConfig.AddressLen == 4)
                        {
                            if (ti.Address.Length >= 8)
                                Talkers[i].Framer.str_Ter_Address = ti.Address.Substring(4, 8);
                        }
                        else
                        {
                            Talkers[i].Framer.str_Ter_Address = ti.Address.Substring(4, 4);
                        }
                    }
                    else
                    {
                        if (VerifyConfig.AddressLen == 4)
                        {
                            Talkers[i].Framer.str_Ter_Address = ti.Address.Substring(4, 8);
                        }
                        else
                        {
                            if (Talkers[i].Framer.str_Ter_Address == "")
                            {
                                Talkers[i].Framer.str_Ter_Address = Convert.ToInt32(ti.Address.Substring(4, 5)).ToString("x4");
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < meterInfo.Length; i++)
            {
                Talkers[i].Framer.str_MainStation = "02";
                Talkers[i].bol_ReportCon = true;
            }

            #region 升源
            if (EquipmentData.StdInfo.Ua < 30)
            {
                PowerOn();
                WaitTime("等待升源稳定", 60);
            }
            #endregion


            #region 获取是否需要读取终端地址
            bool bl_ReadAddress = false;
            // 设置所有终端通讯类信息
            for (int i = 0; i < Talkers.Length; i++)
            {
                TestMeterInfo ti = meterInfo[i];
                if (ti.Address != "")
                {
                    Talkers[i].Framer698.str_Ter_Address = ti.Address.PadLeft(8, '0');
                    if ((ti.Address.ToUpper().Contains("FFFF") || ti.Address.ToUpper().Contains("AAAA")) && ti.YaoJianYn)
                        bl_ReadAddress = true;
                }
            }



            if (bl_ReadAddress && Test_No != ProjectID.终端逻辑地址查询)
            {

                MessageAdd("正在读取终端地址", EnumLogType.提示与流程信息, true);

                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 89, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            if (RecData[i].Length == 5)
                            {
                                TempData[i].Data = GetData(RecData, i, 3, EnumTerimalDataType.e_string) + "|" + GetData(RecData, i, 4, EnumTerimalDataType.e_string);

                                if (VerifyConfig.IsHexAddress)
                                    meterInfo[i].Address = GetData(RecData, i, 3, EnumTerimalDataType.e_string) + GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                                else
                                    meterInfo[i].Address = GetData(RecData, i, 3, EnumTerimalDataType.e_string) + (Convert.ToInt32(GetData(RecData, i, 4, EnumTerimalDataType.e_string), 16)).ToString().PadLeft(5, '0');
                            }
                            else
                            {
                                MessageAdd("读终端地址" + (i + 1) + "返回不正确！", EnumLogType.错误信息);
                            }
                        }
                        else
                        {
                            MessageAdd("读终端地址" + (i + 1) + "无回复！", EnumLogType.错误信息);
                        }

                    }
                }
            }
            #endregion

            if (Test_No != ProjectID.终端逻辑地址查询 && Test_No != ProjectID.通电检查 && Test_No != ProjectID.IP地址和端口设置)
            {
                // 获取终端密码
                MessageAdd("获取终端密码", EnumLogType.提示与流程信息, true);
                //while (true)
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(10, 0, 5, RecData, 5000);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            try
                            {
                                if (RecData.ContainsKey(i))
                                {
                                    if (RecData[i] != null)
                                        if (RecData[i].Length >= 5)
                                        {
                                            //  Talkers[i].Framer.str_Password = Convert.ToInt32(GetData(RecData[i][4])).ToString("X4");       // 赋值终端密码
                                        }
                                        else
                                        {
                                            MessageAdd("获取终端" + (i + 1) + "密码失败！", EnumLogType.错误信息);
                                        }
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            MessageAdd("获取终端" + (i + 1) + "密码失败！", EnumLogType.错误信息);
                        }
                    }
                }
                // 下发终端参数：禁止主动上报。
                MessageAdd("下发终端参数：禁止主动上报。", EnumLogType.提示与流程信息, true);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 37, "", RecData, 5000);
                SetTime(DateTime.Now, 0);
            }
        }


        /// <summary>
        /// 读取终端地址
        /// </summary>
        public void StartVerify698()
        {
            int ret = 0;
            isopenRep = false;
            ControlVirtualMeter("MCL");
            ControlVirtualMeter("Cmd,RunFlag,0");
            ControlVirtualMeter("Cmd,boolIsReturn,1");
            ControlVirtualMeter("Cmd,ProtocalType,2");
            ControlVirtualMeter("Cmd,MeterTimeCheck,0");
            ControlVirtualMeter("Cmd,Connect");
            for (int i = 0; i < meterInfo.Length; i++)
            {
                Talkers[i].Framer.str_MainStation = "02";
            }
            #region 升源
            if (EquipmentData.StdInfo.Ua < 30)
            {
                PowerOn();
     
                WaitTime("等待升源稳定", 60);
                OpenPortIMICP open = new OpenPortIMICP();
                open.GK_ZJ_LOG_INFO_TOPIC(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "升源成功");
            }
            #endregion

            #region 获取是否需要读取终端地址
            bool bl_ReadAddress = false;
            // 设置所有终端通讯类信息
            for (int i = 0; i < Talkers.Length; i++)
            {
                TestMeterInfo ti = meterInfo[i];
                if (ti.Address != "")
                {
                    Talkers[i].Framer698.str_Ter_Address = ti.Address.PadLeft(8, '0');
                    if ((ti.Address.ToUpper().Contains("FFFF") || ti.Address.ToUpper().Contains("AAAA")) && ti.YaoJianYn)
                        bl_ReadAddress = true;
                }
            }

            if (bl_ReadAddress && Test_No != ProjectID.终端逻辑地址查询)
            {
                setData = new byte[MeterNumber][];
                MessageAdd("正在读取终端地址", EnumLogType.提示与流程信息, true);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        setData[i] = Talkers[i].Framer698.ReadData_address("40010200");
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, 5000);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.str_Ter_Address = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                            meterInfo[i].Address = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
                        }
                    }
                }
            }
            #endregion
        }

        #endregion                  x

        //===============================================【设备控制】======================================================
        //===============================================【设备控制】======================================================

        #region 设备控制

        private static DeviceControlS deviceControl;
        public static DeviceControlS DeviceControl
        {
            get
            {
                if (deviceControl == null)
                {
                    deviceControl = new DeviceControlS();
                }
                return deviceControl;
            }
        }
        /// <summary>
        /// 等待所有线程完成
        /// </summary>
        public void WaitWorkDone()
        {
            while (true)
            {
                if (Stop) break;
                if (DeviceThreadManager.Instance.IsWorkDone())
                {
                    break;
                }
                Thread.Sleep(50);
            }
        }

        #region 功率源
        /// 关源
        /// </summary>
        public bool PowerOff()
        {
            if (IsDemo) return true;

            return DeviceControl.PowerOff();

        }
        private float temIa = 0f;
        private float temIb = 0f;
        private float temIc = 0f;


        /// <summary>
        /// 自由升源
        /// </summary>
        /// <param name="jxfs">接线方式</param>
        /// <param name="Ua"></param>
        /// <param name="Ub"></param>
        /// <param name="Uc"></param>
        /// <param name="Ia"></param>
        /// <param name="Ib"></param>
        /// <param name="Ic"></param>
        /// <param name="PhiUa"></param>
        /// <param name="PhiUb"></param>
        /// <param name="PhiUc"></param>
        /// <param name="PhiIa"></param>
        /// <param name="PhiIb"></param>
        /// <param name="PhiIc"></param>
        /// <param name="Freq">频率</param>
        /// <param name="Mode">1</param>
        public bool PowerOn(float Ua, float Ub, float Uc, float Ia, float Ib, float Ic, Cus_PowerYuanJian ele, PowerWay glfx, string glys, float PL = 50)
        {
            if (IsDemo) return true;

            bool t = DeviceControl.PowerOn(Ua, Ub, Uc, Ia, Ib, Ic, ele, glfx, glys, PL);
            if (t)
            {
                WaitTime("升源成功，等待源稳定", DAL.Config.ConfigHelper.Instance.WaitTime_PowerOn);
                //WaitTime("升源成功，等待源稳定", 5);
            }
            return t;
        }

        public bool PowerOn(float Ua, float Ub, float Uc, float Ia, float Ib, float Ic, float UaPhi, float UbPhi, float UcPhi, float IaPhi, float IbPhi, float IcPhi)
        {
            if (Stop) return true;
            DeviceControlS.PowerParam tagPara = new DeviceControlS.PowerParam
            {
                clfs = VerifyBase.Clfs,
                UB = VerifyBase.U,              //Ub
                Element = Cus_PowerYuanJian.H,
                //U
                Ua = Ua,
                Ub = Ub,
                Uc = Uc,
                //I
                Ia = Ia,
                Ib = Ib,
                Ic = Ic,
                //相序
                IsNxx = Cus_PowerPhase.正相序,
                //频率
                Freq = 50,
                UaPhi = UaPhi,
                UbPhi = UbPhi,
                UcPhi = UcPhi,
                IaPhi = IaPhi,
                IbPhi = IbPhi,
                IcPhi = IcPhi,
            };
            if (tagPara.clfs == WireMode.三相三线)
            {
                tagPara.Ub = 0;
                tagPara.Ib = 0;
            }
            bool t = DeviceControl.PowerOn(tagPara, "1.0", PowerWay.正向有功);
            if (t)
            {
                WaitTime("升源成功，等待源稳定", DAL.Config.ConfigHelper.Instance.WaitTime_PowerOn);
                //WaitTime("升源成功，等待源稳定", 5);
            }
            return t;
        }

        public bool PowerOn(float Ua, float Ub, float Uc, float Ia, float Ib, float Ic, float UaPhi, float UbPhi, float UcPhi, float IaPhi, float IbPhi, float IcPhi, string strGlys, PowerWay powerWay)
        {
            if (Stop) return true;
            DeviceControlS.PowerParam tagPara = new DeviceControlS.PowerParam
            {
                clfs = VerifyBase.Clfs,
                UB = VerifyBase.U,              //Ub
                Element = Cus_PowerYuanJian.H,
                //U
                Ua = Ua,
                Ub = Ub,
                Uc = Uc,
                //I
                Ia = Ia,
                Ib = Ib,
                Ic = Ic,
                //相序
                IsNxx = Cus_PowerPhase.正相序,
                //频率
                Freq = 50,
                UaPhi = UaPhi,
                UbPhi = UbPhi,
                UcPhi = UcPhi,
                IaPhi = IaPhi,
                IbPhi = IbPhi,
                IcPhi = IcPhi,
            };
            if (tagPara.clfs == WireMode.三相三线)
            {
                tagPara.Ub = 0;
                tagPara.Ib = 0;
            }
            bool t = DeviceControl.PowerOn(tagPara, strGlys, powerWay);
            if (t)
            {
                WaitTime("升源成功，等待源稳定", DAL.Config.ConfigHelper.Instance.WaitTime_PowerOn);
                //WaitTime("升源成功，等待源稳定", 5);
            }
            return t;
        }
        private bool IsPowerOff(float Ia, float Ib, float Ic)
        {
            bool T = false;
            if (temIa != Ia || temIb != Ib || temIb != Ib)//上一次电流和这一次电流不一样，判断是否需要关闭电流
            {
                //电流挡位有不一样的，就需要关闭电流
                if (CurrentGear(Ia) != CurrentGear(temIa) || CurrentGear(Ib) != CurrentGear(temIb) || CurrentGear(Ib) != CurrentGear(temIb))
                {
                    T = true;
                }
                //保存上一次升源数据，用于判断电流挡位是否改变
            }
            if (temIa == 0 && temIb == 0 && temIc == 0)
            {
                T = false;
            }
            temIa = Ia;
            temIb = Ib;
            temIc = Ic;
            return T;
        }
        /// <summary>
        ///获得功率源电流挡位
        /// </summary>
        /// <returns></returns>
        private int CurrentGear(float I)
        {
            int Grar = 0;
            if (0 <= I && I < 0.0061)
                Grar = 0;
            else if (0.0061 <= I && I < 0.012)
                Grar = 1;
            else if (0.012 <= I && I < 0.031)
                Grar = 2;
            else if (0.031 <= I && I < 0.051)
                Grar = 3;
            else if (0.051 <= I && I < 0.12)
                Grar = 4;
            else if (0.12 <= I && I < 0.22)
                Grar = 5;
            else if (0.22 <= I && I < 1.21)
                Grar = 6;
            else if (1.21 <= I && I < 6.1)
                Grar = 7;
            else if (6.1 <= I && I < 30.01)
                Grar = 8;
            else if (30.01 <= I && I < 125)
                Grar = 9;
            return Grar;
        }



        /// <summary>
        ///  升电压不升电流
        /// </summary>
        /// <returns></returns>
        public bool PowerOn()
        {
            if (IsDemo) return true;
            MessageAdd("开始升源...", EnumLogType.提示信息);
            Cus_PowerYuanJian ele = Cus_PowerYuanJian.H;
            if (OnMeterInfo.MD_WiringMode == "单相") ele = Cus_PowerYuanJian.A;
            return DeviceControl.PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, 0, 0, 0, ele, FangXiang, "1.0");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="bs">倍数</param>
        /// <param name="cs">次数</param>
        /// <param name="time">时间</param>
        /// <param name="ontime">间隔时间</param>
        /// <param name="v"></param>
        /// <returns></returns>
        public bool SetExcessive(byte type, int bs, int cs, int time, int ontime, byte v)
        {
            if (IsDemo) return true;
            MessageAdd("过量设置", EnumLogType.提示信息);
            bs = bs*100;
            time = time * 10;
            ontime = ontime * 10;
            return EquipmentData.DeviceManager.SetExcessive(type, bs, cs, time, ontime, v);
        }

        public bool GearlLock(float U, float I, byte v3)
        {
            if (IsDemo) return true;
            MessageAdd("设置挡位", EnumLogType.提示信息);
            U = U ;
            I = I ;
            return EquipmentData.DeviceManager.GearlLock(U, I, v3);
        }


        #endregion

        #region 误差板

        /// <summary>
        /// 判断数组值是否都一样，一样就广播发送，提高效率
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsArrayValue<T>(T[] value)
        {
            try
            {
                Hashtable hb = new Hashtable();
                hb.Add(value[0], value[0]);
                for (int i = 0; i < value.Length; i++)
                {
                    if (!hb.Contains(value[i]))
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 设置误差板标准常数
        /// </summary>
        /// <param name="ControlType">控制类型(6组被检:00-有功(含正向、反向，以下同,01-无功(正向、反向，以下同),04-日计时,05-需量）</param>
        /// <param name="constant">常数</param>
        /// <param name="magnification">放大倍数-2就是缩小100倍</param>
        /// <param name="EpitopeNo">表位编号</param>
        public bool SetStandardConst(int ControlType, int constant, int magnification, byte EpitopeNo = 0x01)
        {
            if (IsDemo) return true;
            bool IsMass = EpitopeNo == 0xff ? true : false;//是否是广播
            bool[] resoult = new bool[MeterNumber];
            resoult.Fill(true);
            //return DeviceControl.SetStandardConst(ControlType, constant, magnification, EpitopeNo);

            int DeviceCount = EquipmentData.DeviceManager.Devices[Device.DeviceName.误差板].Count;
            //if (DeviceCount == 0) DeviceCount = 1;  //最少一个
            //bool[] resoult = new bool[DeviceCount];
            DeviceThreadManager.Instance.MaxThread = DeviceCount;
            DeviceThreadManager.Instance.MaxTaskCountPerThread = MeterNumber / DeviceCount;
            if (IsMass) DeviceThreadManager.Instance.MaxTaskCountPerThread = 1; //广播的情况
            DeviceThreadManager.Instance.DoWork = delegate (int ID, int pos)
            {
                if (Stop) return;
                if (IsMass) //广播的话 --没有返回值
                {
                    DeviceControl.SetStandardConst(ControlType, constant, magnification, EpitopeNo, ID);
                }
                else
                {
                    if (meterInfo[pos].YaoJianYn)//要检定的表才设置
                    {
                        resoult[pos] = DeviceControl.SetStandardConst(ControlType, constant, magnification, (byte)(pos + 1), ID);
                    }
                }

            };
            DeviceThreadManager.Instance.Start();
            WaitWorkDone();
            //有失败的情况
            if (Array.IndexOf(resoult, false) != -1)
            {
                string err = "";
                for (int i = 0; i < resoult.Length; i++)
                {
                    if (!resoult[i])
                    {
                        err += (i + 1).ToString() + ",";
                    }
                }
                MessageAdd($"表位:{err}误差板标准常数设置失败", EnumLogType.错误信息);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 设置误差板被检常数
        /// </summary>
        /// <param name="ControlType">控制类型(6组被检:00-有功(含正向、反向，以下同,01-无功(正向、反向，以下同),04-日计时,05-需量）</param>
        /// <param name="constant">常数</param>
        /// <param name="magnification">放大倍数-2就是缩小100倍</param>
        /// <param name="qs">圈数</param>
        public bool SetTestedConst(int ControlType, int[] constant, int magnification, int[] qs, byte EpitopeNo = 0x01)
        {
            if (IsDemo) return true;
            bool IsMass = EpitopeNo == 0xff ? true : false;//是否是广播
            bool[] resoult = new bool[MeterNumber];
            resoult.Fill(true);

            int DeviceCount = EquipmentData.DeviceManager.Devices[Device.DeviceName.误差板].Count;
            DeviceThreadManager.Instance.MaxThread = DeviceCount;
            DeviceThreadManager.Instance.MaxTaskCountPerThread = MeterNumber / DeviceCount;
            if (IsMass) DeviceThreadManager.Instance.MaxTaskCountPerThread = 1; //广播的情况
            DeviceThreadManager.Instance.DoWork = delegate (int ID, int pos)
            {
                if (Stop) return;
                if (IsMass) //广播的话 --没有返回值
                {
                    DeviceControl.SetTestedConst(ControlType, constant[0], magnification, qs[0], EpitopeNo, ID);
                }
                else
                {
                    if (meterInfo[pos].YaoJianYn)//要检定的表才设置
                    {
                        resoult[pos] = DeviceControl.SetTestedConst(ControlType, constant[pos], magnification, qs[pos], (byte)(pos + 1), ID);
                    }
                }

            };
            DeviceThreadManager.Instance.Start();
            WaitWorkDone();
            //有失败的情况
            if (Array.IndexOf(resoult, false) != -1)
            {
                string err = "";
                for (int i = 0; i < resoult.Length; i++)
                {
                    if (!resoult[i])
                    {
                        err += (i + 1).ToString() + ",";
                    }
                }
                MessageAdd($"表位:{err}误差板被检常数设置失败", EnumLogType.错误信息);
                return false;
            }
            return true;
        }



        /// <summary>
        /// 启动误差版
        /// </summary>
        /// <param name="ControlType">控制类型（00：正向有功，01：正向无功，02：反向有功，03：反向无功，04：日计时，05：需量， 06：正向有功脉冲计数， 07：正向无功脉冲计数， 08：反向有功脉冲计数，09 反向无功脉冲计数）</param>
        /// <param name="MeterNo">表位号，FF为广播</param>
        /// <returns></returns>
        public bool StartWcb(int ControlType, byte MeterNo)
        {
            if (IsDemo) return true;
            bool IsMass = MeterNo == 0xff ? true : false;//是否是广播
            bool[] resoult = new bool[MeterNumber];
            resoult.Fill(true);

            int DeviceCount = EquipmentData.DeviceManager.Devices[Device.DeviceName.误差板].Count;
            DeviceThreadManager.Instance.MaxThread = DeviceCount;
            DeviceThreadManager.Instance.MaxTaskCountPerThread = MeterNumber / DeviceCount;
            if (IsMass)
            {
                DeviceThreadManager.Instance.MaxTaskCountPerThread = 1; //广播的情况
                IsRunWc = true;
            }
            DeviceThreadManager.Instance.DoWork = delegate (int ID, int pos)
            {
                if (Stop) return;
                if (IsMass) //广播的话 --没有返回值
                {
                    DeviceControl.StartWcb(ControlType, MeterNo, ID);
                }
                else
                {
                    if (meterInfo[pos].YaoJianYn)//要检定的表才设置
                    {
                        resoult[pos] = DeviceControl.StartWcb(ControlType, (byte)(pos + 1), ID);
                    }
                }

            };
            DeviceThreadManager.Instance.Start();
            WaitWorkDone();
            //有失败的情况
            if (Array.IndexOf(resoult, false) != -1)
            {
                string err = "";
                for (int i = 0; i < resoult.Length; i++)
                {
                    if (!resoult[i])
                    {
                        err += (i + 1).ToString() + ",";
                    }
                }
                MessageAdd($"表位:{err}启动误差版失败", EnumLogType.错误信息);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 停止误差版
        /// </summary>
        /// <param name="ControlType">控制类型（00：正向有功，01：正向无功，02：反向有功，03：反向无功，04：日计时，05：需量， 06：正向有功脉冲计数， 07：正向无功脉冲计数， 08：反向有功脉冲计数，09 反向无功脉冲计数）</param>
        /// <param name="MeterNo">表位号，FF为广播</param>
        /// <returns></returns>
        public bool StopWcb(int ControlType, byte MeterNo)
        {
            if (IsDemo) return true;
            bool IsMass = MeterNo == 0xff ? true : false;//是否是广播
            bool[] resoult = new bool[MeterNumber];
            resoult.Fill(true);

            int DeviceCount = EquipmentData.DeviceManager.Devices[Device.DeviceName.误差板].Count;
            DeviceThreadManager.Instance.MaxThread = DeviceCount;
            DeviceThreadManager.Instance.MaxTaskCountPerThread = MeterNumber / DeviceCount;
            if (IsMass)
            {
                DeviceThreadManager.Instance.MaxTaskCountPerThread = 1; //广播的情况
                IsRunWc = false;
            }
            DeviceThreadManager.Instance.DoWork = delegate (int ID, int pos)
            {
                if (Stop) return;
                if (IsMass) //广播的话 --没有返回值
                {
                    DeviceControl.StopWcb(ControlType, MeterNo, ID);
                }
                else
                {
                    if (meterInfo[pos].YaoJianYn)//要检定的表才设置
                    {
                        resoult[pos] = DeviceControl.StopWcb(ControlType, (byte)(pos + 1), ID);
                    }
                }

            };
            DeviceThreadManager.Instance.Start();
            WaitWorkDone();
            //有失败的情况
            if (Array.IndexOf(resoult, false) != -1)
            {
                string err = "";
                for (int i = 0; i < resoult.Length; i++)
                {
                    if (!resoult[i])
                    {
                        err += (i + 1).ToString() + ",";
                    }
                }
                MessageAdd($"表位:{err}停止误差版失败", EnumLogType.错误信息);
                return false;
            }
            return true;

        }

        /// <summary>
        ///  读取误差板误差 
        /// </summary>
        /// <param name="readType">读取类型(00--正向有功，01--正向无功，02--反向有功，03--反向无功，04--日计时误差</param>
        public stError[] ReadWcbData(bool[] t, int readType)
        {
            stError[] stErrors = new stError[MeterNumber];
            if (IsDemo) return null;
            int DeviceCount = EquipmentData.DeviceManager.Devices[Device.DeviceName.误差板].Count;
            DeviceThreadManager.Instance.MaxThread = DeviceCount;
            DeviceThreadManager.Instance.MaxTaskCountPerThread = MeterNumber / DeviceCount;
            DeviceThreadManager.Instance.DoWork = delegate (int ID, int pos)
            {
                if (Stop) return;
                if (t[pos])//要检定的表才设置
                {
                    stErrors[pos] = DeviceControl.ReadWcbData(readType, (byte)(pos + 1), ID);
                }
            };
            DeviceThreadManager.Instance.Start();
            WaitWorkDone();
            //有失败的情况
            return stErrors;

        }

        /// <summary>
        ///  控制表位继电器 
        /// </summary>
        /// <param name="contrnlType">控制类型--2开启-1关闭</param>
        /// <param name="EpitopeNo">表位编号0xFF为广播</param>
        public void ControlMeterRelay(bool[] t, int contrnlType)
        {
            if (IsDemo) return;
            //DeviceControl.ControlMeterRelay(contrnlType, EpitopeNo);
            int DeviceCount = EquipmentData.DeviceManager.Devices[Device.DeviceName.误差板].Count;
            DeviceThreadManager.Instance.MaxThread = DeviceCount;
            DeviceThreadManager.Instance.MaxTaskCountPerThread = MeterNumber / DeviceCount;
            DeviceThreadManager.Instance.DoWork = delegate (int ID, int pos)
            {
                if (Stop) return;
                if (t[pos])//要检定的表才设置
                {
                    DeviceControl.ControlMeterRelay(contrnlType, (byte)(pos + 1), ID);
                }
            };
            DeviceThreadManager.Instance.Start();
            WaitWorkDone();
        }

        /// <summary>
        /// 电机控制---
        /// </summary>
        /// <param name="bwNum">表位</param>
        /// <returns></returns>
        public bool ElectricmachineryContrnl(bool[] t, int ControlType)
        {
            if (IsDemo) return true;
            bool[] IsChekc = new bool[MeterNumber];
            IsChekc.Fill(true);
            int DeviceCount = EquipmentData.DeviceManager.Devices[Device.DeviceName.误差板].Count;
            DeviceThreadManager.Instance.MaxThread = DeviceCount;
            DeviceThreadManager.Instance.MaxTaskCountPerThread = MeterNumber / DeviceCount;
            DeviceThreadManager.Instance.DoWork = delegate (int ID, int pos)
            {
                if (Stop) return;
                if (t[pos])//要检定的表才设置
                {
                    IsChekc[pos] = DeviceControl.ElectricmachineryContrnl(ControlType, (byte)(pos + 1), ID);
                    Thread.Sleep(50); //线程太快有时候电机反应不过来
                }
            };
            DeviceThreadManager.Instance.Start();
            WaitWorkDone();
            if (Array.IndexOf<bool>(IsChekc, false) >= 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读取所有表位状态
        /// </summary>
        /// <param name="bwNum">表位</param>
        /// <returns></returns>
        public MeterState[] Read_Meterstate(bool[] t)
        {
            if (IsDemo) return null;
            MeterState[] meters = new MeterState[MeterNumber];
            int DeviceCount = EquipmentData.DeviceManager.Devices[Device.DeviceName.误差板].Count;
            DeviceThreadManager.Instance.MaxThread = DeviceCount;
            DeviceThreadManager.Instance.MaxTaskCountPerThread = MeterNumber / DeviceCount;
            DeviceThreadManager.Instance.DoWork = delegate (int ID, int pos)
            {
                if (Stop) return;
                if (t[pos])//要检定的表才设置
                {
                    meters[pos] = DeviceControl.Read_Fault((byte)(pos + 1), ID);
                }
            };
            DeviceThreadManager.Instance.Start();
            WaitWorkDone();
            return meters;
        }

        /// <summary>
        /// 设置脉冲输出（两组脉冲频率可以不一样，但不能超过 500 倍）
        /// </summary>
        /// <param name="contrnlType">0x00=两组都不输出脉冲；0x01=仅第一组输出设定脉冲；0x02=仅第二组输出设定脉冲；0x03=两组都输出设定脉冲；</param>
        /// <param name="MeterNo">表位号</param>
        /// <param name="fq1">第一组脉冲-频率--比如1000HZ</param>
        /// <param name="PWM1">第一组脉冲-占空比-0.5就是百分50</param>
        /// <param name="PulseNum1">第一组脉冲-脉冲个数--0表示连续输出</param>
        /// <param name="fq2">第二组脉冲-频率--比如1000HZ</param>
        /// <param name="PWM2">第二组脉冲-占空比-0.5就是百分50</param>
        /// <param name="PulseNum2">第二组脉冲-脉冲个数--0表示连续输出</param>
        /// <returns></returns>
        public bool SetPulseOutput(bool[] t, byte ControlType, float fq1, float PWM1, int PulseNum1, float fq2, float PWM2, int PulseNum2)
        {
            if (IsDemo) return true;
            bool[] IsChekc = new bool[MeterNumber];
            IsChekc.Fill(true);
            int DeviceCount = EquipmentData.DeviceManager.Devices[Device.DeviceName.误差板].Count;
            DeviceThreadManager.Instance.MaxThread = DeviceCount;
            DeviceThreadManager.Instance.MaxTaskCountPerThread = MeterNumber / DeviceCount;
            DeviceThreadManager.Instance.DoWork = delegate (int ID, int pos)
            {
                if (Stop) return;
                if (t[pos])//要检定的表才设置
                {
                    IsChekc[pos] = DeviceControl.SetPulseOutput(ControlType, (byte)(pos + 1), fq1, PWM1, PulseNum1, fq2, PWM2, PulseNum2, ID);
                }
            };
            DeviceThreadManager.Instance.Start();
            WaitWorkDone();
            if (Array.IndexOf<bool>(IsChekc, false) >= 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///关闭脉冲输出
        /// <returns></returns>
        public bool SetPulseOutputStop(bool[] t)
        {
            if (IsDemo) return true;
            bool[] IsChekc = new bool[MeterNumber];
            IsChekc.Fill(true);
            int DeviceCount = EquipmentData.DeviceManager.Devices[Device.DeviceName.误差板].Count;
            DeviceThreadManager.Instance.MaxThread = DeviceCount;
            DeviceThreadManager.Instance.MaxTaskCountPerThread = MeterNumber / DeviceCount;
            DeviceThreadManager.Instance.DoWork = delegate (int ID, int pos)
            {
                if (Stop) return;
                if (t[pos])//要检定的表才设置
                {
                    IsChekc[pos] = DeviceControl.SetPulseOutput(0x00, (byte)(pos + 1), 0, 0, 0, 0, 0, 0, ID);
                }
            };
            DeviceThreadManager.Instance.Start();
            WaitWorkDone();
            if (Array.IndexOf<bool>(IsChekc, false) >= 0)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 控制遥信状态输出
        /// <param name="t">要检定表</param>
        /// <param name="RS1">遥信1 true输出，false不输出</param>
        /// <param name="RS2">遥信2 true输出，false不输出</param>
        /// <param name="RS3">遥信3 true输出，false不输出</param>
        /// <param name="RS4">遥信4 true输出，false不输出</param>
        /// <param name="RS5">遥信5 true输出，false不输出</param>
        /// <param name="RS6">遥信6 true输出，false不输出</param>
        /// <param name="ID"></param>
        public void ContnrRemoteSignalingStatusOutput(bool[] t, bool RS1, bool RS2, bool RS3, bool RS4, bool RS5, bool RS6)
        {
            if (IsDemo) return;
            int DeviceCount = EquipmentData.DeviceManager.Devices[Device.DeviceName.误差板].Count;
            DeviceThreadManager.Instance.MaxThread = DeviceCount;
            DeviceThreadManager.Instance.MaxTaskCountPerThread = MeterNumber / DeviceCount;

            DeviceThreadManager.Instance.DoWork = delegate (int ID, int pos)
            {
                if (Stop) return;
                if (t[pos])//要检定的表才设置
                {
                    DeviceControl.ContnrRemoteSignalingStatusOutput((byte)(pos + 1), RS1, RS2, RS3, RS4, RS5, RS6, ID);
                }
            };
            DeviceThreadManager.Instance.Start();
            WaitWorkDone();
        }
        /// <summary>
        /// 停止控制遥信状态输出
        /// </summary>
        public void ContnrRemoteSignalingStatusOutputStop()
        {
            if (IsDemo) return;
            int DeviceCount = EquipmentData.DeviceManager.Devices[Device.DeviceName.误差板].Count;
            DeviceThreadManager.Instance.MaxThread = DeviceCount;
            DeviceThreadManager.Instance.MaxTaskCountPerThread = MeterNumber / DeviceCount;
            DeviceThreadManager.Instance.DoWork = delegate (int ID, int pos)
            {
                if (Stop) return;
                DeviceControl.ContnrRemoteSignalingStatusOutput((byte)(pos + 1), false, false, false, false, false, false, ID);
            };
            DeviceThreadManager.Instance.Start();
            WaitWorkDone();
        }
        public void SetConntype(bool[] t, int type)
        {
            if (IsDemo) return;
            int DeviceCount = EquipmentData.DeviceManager.Devices[Device.DeviceName.误差板].Count;
            DeviceThreadManager.Instance.MaxThread = DeviceCount;
            DeviceThreadManager.Instance.MaxTaskCountPerThread = MeterNumber / DeviceCount;
            DeviceThreadManager.Instance.DoWork = delegate (int ID, int pos)
            {
                if (Stop) return;
                if (t[pos])//要检定的表才设置
                {
                    EquipmentData.DeviceManager.ControlConnrRelay(type, (byte)(pos + 1), ID); //切到232
                }
            };
            DeviceThreadManager.Instance.Start();
            WaitWorkDone();

            //TODO 232通讯部分
            //if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.Channel232 || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelMaintain)
            //{
            //    EquipmentData.DeviceManager.ControlConnrRelay(1, (byte)(i+1), ThreadID - 1); //切到232
            //}
            //if (DoWork != null)
            //{
            //    DoWork(i);
            //}
            //if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.Channel232 || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelMaintain)
            //{
            //    EquipmentData.DeviceManager.ControlConnrRelay(0, (byte)(i + 1), ThreadID - 1); //切回485
            //}
            //if (!m_bol_IsRun)
            //    break;
        }
        #endregion

        #region 标准表


        /// <summary>
        ///获得标准表的常数
        /// </summary>
        /// <param name="stdCmd">>0x10 读 ，0x13写</param>
        /// <param name="stdConst">常数</param>
        /// <param name="stdUIGear">电压电流挡位UA，ub，uc，ia，ib，ic</param>
        public long GetStaConst(float ua, float ub, float uc, float ia, float ib, float ic)
        {
            if (IsDemo) return 1000000;
            long stdConst = 1000000;
            double[] stdUIGear = new double[6];
            DeviceControl.StdGear(0x10, ref stdConst, stdUIGear);
            return stdConst;
        }

        /// <summary>
        ///1008H- 档位常数 读取与设置
        /// </summary>
        /// <param name="stdCmd">>0x10 读 ，0x13写</param>
        /// <param name="stdConst">常数</param>
        /// <param name="ua">A相电压当前档位</param>
        public bool StdGear(byte stdCmd, long stdConst, float ua, float ub, float uc, float ia, float ib, float ic)
        {
            if (IsDemo) return true;
            double[] stdUIGear = new double[] { ua, ub, uc, ia, ib, ic };
            return DeviceControl.StdGear(stdCmd, ref stdConst, stdUIGear);
        }

        /// <summary>
        /// 100cH-启停标准表累积电能
        /// </summary>
        /// <param name="startOrStopStd">字符’1’表示清零当前电能并开始累计（ascii 码读取）</param>
        public bool startStdEnergy(int startOrStopStd)
        {
            if (IsDemo) return true;
            return DeviceControl.startStdEnergy(startOrStopStd);
        }

        ///读取标准表累积电量
        public float[] ReadStmEnergy()
        {
            if (IsDemo) return null;
            return DeviceControl.ReadEnergy();
        }

        /// <summary>
        ///   设置脉冲
        /// </summary>
        /// <param name="pulseType">
        ///有功脉冲 设置字符’1’
        ///无功脉冲 设置字符’2’
        ///UA脉冲 设置字符’3’
        ///UB脉冲 设置字符’4’
        ///UC脉冲 设置字符’5’
        ///IA脉冲 设置字符’6’
        ///IB脉冲 设置字符’7’
        ///IC脉冲 设置字符’8’
        ///PA脉冲 设置字符’9’
        ///PB脉冲 设置字符’10’
        ///PC脉冲 设置字符’11’
        ///</param>
        /// <returns></returns>
        public bool SetPulseType(string pulseType)
        {
            if (IsDemo) return true;
            return DeviceControl.SetPulseType(pulseType);
        }
        #endregion


        #region 零线电流板
        //add yjt 20230103 新增零线电流控制启停
        /// <summary>
        /// 启停零线电流板
        /// </summary>
        /// <param name="startOrStopStd">字符‘1’开启，字符‘0’关闭</param>
        public bool StartZeroCurrent(int A_kz, int BC_kz)
        {
            if (IsDemo) return true;
            return DeviceControl.StartZeroCurrent(A_kz, BC_kz);
        }
        #endregion
        
        #endregion

    }

}
