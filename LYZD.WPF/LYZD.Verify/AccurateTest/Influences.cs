using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.Core.Helper;
using LYZD.Core.Model.Meter;
using LYZD.Core.Struct;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LYZD.Verify.AccurateTest
{

    public class Influences : VerifyBase
    {
        #region 参数

        /// <summary>
        /// 误差限
        /// </summary>
        float ErrorLimit = 1f;
        /// <summary>
        /// 功率元件
        /// </summary>
        Cus_PowerYuanJian YJ = Cus_PowerYuanJian.H;
        /// <summary>
        /// 功率方向
        /// </summary>
        PowerWay FangXian = PowerWay.正向有功;
        /// <summary>
        /// 功率因素
        /// </summary>
        string Glys = "1.0";
        /// <summary>
        /// 电流倍数
        /// </summary>
        string Xib = "1.0Ib";
        /// <summary>
        /// 频率
        /// </summary>
        float PL = 50f;
        /// <summary>
        /// 电压倍数
        /// </summary>
        float VoltageMultiple = 1f;

        int maxWCnum = VerifyConfig.ErrorCount;//最多误差次数
        float meterLevel = 2;//等级
        int MaxTime = 300000;
        bool IsStop = false; //是否退出当前检定项目--
        int qs = 2;
        #endregion

        public override void Verify()
        {
            base.Verify();
            ResultDictionary["结论"][0] = "不合格";
            RefUIData("结论");
            return;

            IsStop = Stop;
            qs = GetQs(VerifyConfig.IsTimeWcLapCount, OnMeterInfo.MD_UB, OnMeterInfo.MD_UA, Xib, Clfs, YJ, OnMeterInfo.MD_Constant, Glys, HGQ, VerifyConfig.WcMinTime);

            #region 上传误差参数
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    //"功率元件","功率方向","电流倍数","功率因数","误差下限","误差上限","误差圈数"
                    ResultDictionary["功率元件"][i] = YJ.ToString();
                    ResultDictionary["功率方向"][i] = FangXian.ToString();
                    ResultDictionary["电流倍数"][i] = Xib.ToString();
                    ResultDictionary["功率因素"][i] = Glys;
                    ResultDictionary["误差下限"][i] = (-ErrorLimit).ToString();
                    ResultDictionary["误差上限"][i] = ErrorLimit.ToString();
                    ResultDictionary["误差圈数"][i] = qs.ToString();
                }
            }
            RefUIData("功率元件");
            RefUIData("功率方向");
            RefUIData("电流倍数");
            RefUIData("功率因素");
            RefUIData("误差下限");
            RefUIData("误差上限");
            RefUIData("误差圈数");
            #endregion


            MaxTime = VerifyConfig.MaxHandleTime * 1000;

            //开始做一次基本误差
            StartError("");
            if (Stop) return;
            if (!IsStop)//如果之前的误差有问题就别做影响量后的了，浪费时间
            {
                PowerOn();//先把电流关了
                WaitTime("影响量前检定完成，关闭电流", 5);

                InitEquipMent();
                WaitTime("正在修改影响量", 3);
                //影响后误差
                StartError("影响后");
                //这里计算改变量--计算误差
            }
            if (Stop) return;
            if (IsStop)
            {
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (!meterInfo[i].YaoJianYn) continue;     //表位不要检
                    ResultDictionary["结论"][i] = "不合格";
                }
            }
            else
            {
                //计算变差值

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (!meterInfo[i].YaoJianYn) continue;     //表位不要检

                    string bc = "";
                    if (ResultDictionary["平均值"][i]!=null && ResultDictionary["影响后平均值"][i] != null
                        && ResultDictionary["平均值"][i] != "" || ResultDictionary["影响后平均值"][i] != "")
                    {
                        bc = Math.Abs(float.Parse(ResultDictionary["影响后平均值"][i]) - float.Parse(ResultDictionary["平均值"][i])).ToString();
                    }
                    ResultDictionary["变差值"][i] = bc;
                    if (bc!="")
                    {
                        if (Math.Abs(float.Parse(bc)) > ErrorLimit)
                        {
                            ResultDictionary["结论"][i] = "不合格";
                        }
                        else
                        {
                            ResultDictionary["结论"][i] = "合格";
                        }
                    }
                }
                RefUIData("变差值");
            }

            RefUIData("结论");
            PowerOn();
            WaitTime("检定完成，关闭电流", 5);

            //切换回去正常谐波
            if (!IsStop)
            {
                if (Test_No == ProjectID.方形波波形改变 || Test_No == ProjectID.尖顶波波形改变 || Test_No == ProjectID.间谐波波形改变 || Test_No == ProjectID.奇次谐波波形试验)
                {
                    if (!DeviceControl.SetPowerHarmonic("1", "1", "1", "1", "1", "1", 0))
                    {
                        MessageAdd("切换回正常谐波出错,请检查", EnumLogType.错误信息);
                    }
                }
            }
            MessageAdd("检定完成",EnumLogType.提示信息);
        }
        protected override bool CheckPara()
        {
            string[] data = Test_Value.Split('|');
            FangXian = (PowerWay)Enum.Parse(typeof(PowerWay), data[0]);
            switch (data[1])
            {
                case "H":
                    YJ = Cus_PowerYuanJian.H;
                    break;
                case "A":
                    YJ = Cus_PowerYuanJian.A;
                    break;
                case "B":
                    YJ = Cus_PowerYuanJian.B;
                    break;
                case "C":
                    YJ = Cus_PowerYuanJian.C;
                    break;
                default:
                    YJ = Cus_PowerYuanJian.H;
                    break;
            }
            Glys = data[2];
            Xib = data[3];
            ErrorLimit =int.Parse( data[4] ?? "1");
            switch (Test_No)
            {
                case ProjectID.频率改变:
                    PL = float.Parse(data[5]!="" ? data[5] : "50");
                    break;
                case ProjectID.电压改变:
                    VoltageMultiple =float.Parse(data[5] != "" ? data[5].TrimEnd('%') : "1");
                    break;
                default:
                    break;
            }
            ResultNames = new string[] { "功率元件", "功率方向", "电流倍数", "功率因素", "误差下限", "误差上限", "误差圈数", "误差1", "误差2", "平均值", "化整值", "影响后误差1", "影响后误差2", "影响后平均值", "影响后化整值", "结论" };
            return true;
        }


        #region 计算方法

        /// <summary>
        /// 获取检定圈数
        /// </summary>
        /// <param name="IsTimecalculation">是否使用时间计算</param>
        /// <param name="U">电压</param>
        /// <param name="I">电流</param>
        /// <param name="PowerDianLiu">电流倍数</param>
        /// <param name="JXFS">接线方式</param>
        /// <param name="YJ">功率元件</param>
        /// <param name="MeConst">常数</param>
        /// <param name="GLYS">功率因素</param>
        /// <param name="HGQ">是否经过互感器</param>
        /// <param name="time">最小时间</param>
        /// <returns></returns>
        public int GetQs(bool IsTimecalculation, float U, string I, string PowerDianLiu, WireMode JXFS, Cus_PowerYuanJian YJ, string MeConst, string GLYS, bool HGQ, float time = 5)
        {
            //电压，电流，测量方式，元件，功率因素，有功无功，
            float xIb = Number.GetCurrentByIb(PowerDianLiu, I, HGQ);
            int _MeConst;
            bool IsYouGong = true;
            if (this.FangXian == PowerWay.正向有功 || this.FangXian == PowerWay.反向有功)
            {
                _MeConst = Number.GetBcs(MeConst, FangXian);
            }
            else
            {
                _MeConst = Number.GetBcs(MeConst, FangXian);
                IsYouGong = false;
            }
            int QS = 2;
            if (IsTimecalculation)    //使用时间计算
            {
                // 圈数计算方法
                float currentPower = Number.CalculatePower(U, xIb, JXFS, YJ, GLYS, IsYouGong);
                //计算一度大需要的时间,单位分钟
                int onePulseTime = 99;
                onePulseTime = (int)Math.Ceiling(time / (1 / (currentPower * _MeConst / 3600000)));
                QS = onePulseTime;

            }
            else
            { 
            float Tqs = xIb / Number.GetCurrentByIb("1.0Ib", I, HGQ) * Number.GetGlysValue(GLYS);
                if (YJ!= Cus_PowerYuanJian.H)
                {
                    Tqs /= 3;
                }
                QS = (int)Math.Round((double)Tqs, 0);
            }

            if (QS <= 0)
                QS = 1;
            return QS;

        }


        private bool InitEquipment(int qs)
        {
            if (IsDemo) return true;
            if (IsDemo) return true;
            bool isP = (FangXiang == PowerWay.正向有功 ||FangXiang == PowerWay.反向有功) ? true : false;
            int[] meterconst = MeterHelper.Instance.MeterConst(isP);
            long constants = isP? MeterHelper.Instance.MeterConstMin()[0] : MeterHelper.Instance.MeterConstMin()[1];
            float xIb = Number.GetCurrentByIb(Xib, OnMeterInfo.MD_UA, HGQ);
            if (!VerifyConfig.GearModel)
            {
                StdGear(0x13, 0, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb);
                WaitTime("正在设置电流挡位", 2);
            }

            MessageAdd("正在升源...",EnumLogType.提示信息);
            if (!PowerOn(OnMeterInfo.MD_UB* VoltageMultiple, OnMeterInfo.MD_UB* VoltageMultiple, OnMeterInfo.MD_UB* VoltageMultiple, xIb, xIb, xIb, YJ, FangXiang, Glys, PL))
            {
                MessageAdd("升源失败,退出检定",EnumLogType.提示信息);
                return false;
            }
            constants = VerifyConfig.StdConst;
            if (!VerifyConfig.ConstModel)
            {
                constants = GetStaConst(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb);
            }
            else
            {
                //1:设置标准表挡位、常数
                MessageAdd("正在设置标准表常数...",EnumLogType.提示信息);
                StdGear(0x13, constants, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb);
            }

            MessageAdd("正在设置标准表脉冲...",EnumLogType.提示信息);
            int index = 0;
            if (!isP)
            {
                index = 1;
            }
            SetPulseType((index + 49).ToString("x"));
            if (Stop) return true;
            MessageAdd("开始初始化基本误差检定参数!",EnumLogType.提示信息);
            //设置误差版被检常数
            MessageAdd("正在设置误差版标准常数...",EnumLogType.提示信息);
            int SetConstants = (int)(constants / 100);
            SetStandardConst(0, SetConstants, -2, 0xff);
            //设置误差版标准常数 TODO2
            MessageAdd("正在设置误差版被检常数...",EnumLogType.提示信息);
            int[] q = new int[MeterNumber];
            q.Fill(qs);
            if (!SetTestedConst(index, meterconst, 0, q))
            {
                MessageAdd("初始化误差检定参数失败",EnumLogType.提示信息);
                return false;
            }
            return true;
        }

        private int GetFangXianIndex(PowerWay fx)
        {
            int readType = 0;
            switch (fx)
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
            }
            return readType;
        }

        /// <summary>
        /// 计算基本误差
        /// </summary>
        /// <param name="data">要参与计算的误差数组</param>
        /// <returns></returns>
        public ErrorResoult SetWuCha(StPlan_WcPoint wcPoint, float meterLevel, float[] data)
        {
            ErrorResoult resoult = new ErrorResoult();
            float space = GetWuChaHzzJianJu(false, meterLevel);                              //化整间距 
            float avg = Number.GetAvgA(data);
            float hz = Number.GetHzz(avg, space);

            //添加符号
            int hzPrecision = Common.GetPrecision(space.ToString());
            string AvgNumber = AddFlag(avg, VerifyConfig.PjzDigit).ToString();

            string HZNumber = hz.ToString(string.Format("F{0}", hzPrecision));
            if (hz != 0f) //化整值为0时，不加正负号
                HZNumber = AddFlag(hz, hzPrecision);

            if (avg < 0) HZNumber = HZNumber.Replace('+', '-'); //平均值<0时，化整值需为负数

            // 检测是否超过误差限
            if (avg >= wcPoint.ErrorXiaXian && avg <= wcPoint.ErrorShangXian)
                resoult.Resoult = Const.合格;
            else
                resoult.Resoult = Const.不合格;

            //记录误差
            string strWuCha = string.Empty;
            int wcCount = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != Const.没有误差默认值)
                {
                    wcCount++;
                    strWuCha += string.Format("{0}|", AddFlag(data[i], VerifyConfig.PjzDigit));
                }
                else
                {
                    strWuCha += " |";
                }
            }
            if (wcCount != data.Length)
            {
                resoult.Resoult = Const.不合格;
            }

            strWuCha += string.Format("{0}|", AvgNumber);
            strWuCha += string.Format("{0}", HZNumber);
            resoult.ErrorValue = strWuCha;

            return resoult;
        }

        /// <summary>
        /// 修正数字加+-号
        /// </summary>
        /// <param name="data">要修正的数字</param>
        /// <param name="Priecision">修正精度</param>
        /// <returns>返回指定精度的带+-号的字符串</returns>
        private string AddFlag(float data, int Priecision)
        {
            string v = data.ToString(string.Format("F{0}", Priecision));
            if (float.Parse(v) > 0)
                return string.Format("+{0}", data);
            else
                return v;
        }
        /// <summary>
        /// 返回修正间距
        /// </summary>
        /// <IsWindage>是否是偏差</IsWindage> 
        /// <returns></returns>
        private float GetWuChaHzzJianJu(bool IsWindage, float meterLevel)
        {
            Dictionary<string, float[]> DicJianJu = null;
            string Key = string.Format("Level{0}", meterLevel);
            //根据表精度及表类型生成主键
            //if (ErrLimit.IsSTM)
            //    Key = string.Format("Level{0}B", ErrLimit.MeterLevel);
            //else
            //    Key = string.Format("Level{0}", ErrLimit.MeterLevel);

            if (DicJianJu == null)
            {
                DicJianJu = new Dictionary<string, float[]>
                {
                    { "Level0.02B", new float[] { 0.002F, 0.0002F } },      //0.02级表标准表
                    { "Level0.05B", new float[] { 0.005F, 0.0005F } },      //0.05级表标准表
                    { "Level0.1B", new float[] { 0.01F, 0.001F } },         //0.1级表标准表
                    { "Level0.2B", new float[] { 0.02F, 0.002F } },         //0.2级标准表
                    { "Level0.2", new float[] { 0.02F, 0.004F } },          //0.2级普通表
                    { "Level0.5", new float[] { 0.05F, 0.01F } },           //0.5级表
                    { "Level1", new float[] { 0.1F, 0.02F } },              //1级表
                    { "Level1.5", new float[] { 0.2F, 0.04F } }  ,           //2级表
                    { "Level2", new float[] { 0.2F, 0.04F } }               //2级表
                };
            }

            float[] JianJu;
            if (DicJianJu.ContainsKey(Key))
            {
                JianJu = DicJianJu[Key];
            }
            else
            {
                JianJu = new float[] { 2, 2 };    //没有在字典中找到，则直接按2算
            }

            if (IsWindage)
                return JianJu[1];//标偏差
            else
                return JianJu[0];//普通误差
        }

        #endregion

        private void StartError(string startString)
        {
            if (Stop || IsStop) return;

            if (Stop) return;

            if (!InitEquipment(qs))
            //if (!ErrorInitEquipment(FangXiang, YJ, Test_GLYS, Test_DLBS, qs))
            {
                MessageAdd("初始化基本误差设备参数失败",EnumLogType.提示信息);
                return;
            }
            if (Stop) return;

            MessageAdd("正在启动误差版...",EnumLogType.提示信息);
            if (!StartWcb(GetFangXianIndex(FangXian), 0xff))
            {
                MessageAdd("误差板启动失败...",EnumLogType.提示信息);
                return;
            }
            MessageAdd("开始检定...",EnumLogType.提示信息);
            ErrorResoult[] errorResoults = new ErrorResoult[MeterNumber];
            StPlan_WcPoint[] arrPlanList = new StPlan_WcPoint[MeterNumber];      // 误差点数据
            int[] WCNumner = new int[MeterNumber]; //检定次数
            bool[] arrCheckOver = new bool[MeterNumber];     //表位完成记录
            int[] lastNum = new int[MeterNumber];                   //保存上一次误差的序号
            lastNum.Fill(-1);
            List<string>[] errList = new List<string>[MeterNumber]; //记录当前误差[数组长度，]
            for (int i = 0; i < MeterNumber; i++)
                errList[i] = new List<string>();
        
            DateTime TmpTime1 = DateTime.Now;//检定开始时间，用于判断是否超时
            while (true)
            {
                if (Stop) break;
                if (TimeSub(DateTime.Now, TmpTime1) > MaxTime && !IsMeterDebug) //超出最大处理时间并且不是调表状态
                {
                    IsStop = true;
                    MessageAdd("超出最大处理时间,正在退出...",EnumLogType.提示信息);
                    break;
                }
                if (IsStop|| Stop) break;
                string[] curWC = new string[MeterNumber];   //重新初始化本次误差
                int[] curNum = new int[MeterNumber];        //当前读取的误差序号
                curWC.Fill("");
                curNum.Fill(0);
                if (!ReadWc(ref curWC, ref curNum, FangXiang))    //读取误差
                {
                    continue;
                }
                if (Stop) break;

                //依次处理每个表位的误差数据
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (!meterInfo[i].YaoJianYn) arrCheckOver[i] = true;     //表位不要检
                    if (arrCheckOver[i] && !IsMeterDebug) continue;   //表位检定通过了
                    if (lastNum[i] >= curNum[i]) continue;
                    if (string.IsNullOrEmpty(curWC[i])) continue;
                    if (curNum[i] <= VerifyConfig.ErrorStartCount) continue; //当前误差次数小于去除的个数

                    if (curNum[i] > lastNum[i]) //大于上一次误差次数
                    {
                        WCNumner[i]++;   //检定次数

                        lastNum[i] = curNum[i];
                    }
                    errList[i].Insert(0, curWC[i]);
                    if (errList[i].Count > maxWCnum)
                        errList[i].RemoveAt(errList[i].Count - 1);
                    meterLevel = MeterLevel(meterInfo[i]);
                    if (Stop) break;
                    //计算误差
                    float[] tpmWc = ArrayConvert.ToSingle(errList[i].ToArray());  //Datable行到数组的转换

                    ErrorResoult tem = SetWuCha(arrPlanList[i], meterLevel, tpmWc);

                    if (errList[i].Count >= maxWCnum)  //误差数量>=需要的最大误差数2
                    {
                        arrCheckOver[i] = true;
                        if (tem.Resoult != Const.合格)
                        {
                            if (WCNumner[i] <= VerifyConfig.ErrorMax)
                            {
                                arrCheckOver[i] = false;
                            }
                        }
                    }
                    else
                    {
                        arrCheckOver[i] = false;
                        tem.Resoult = Const.不合格;
                        NoResoult[i] = "没有读取到俩次误差";
                    }
                    errorResoults[i] = tem;

                    string[] value = errorResoults[i].ErrorValue.Split('|');
                    ResultDictionary[$"误差1"][i] = value[0].ToString();
                    RefUIData($"误差1");
                    if (Stop) break;
                    if (value.Length > 3)
                    {
                        if (value[1].ToString().Trim() != "")
                        {
                            ResultDictionary[startString+"误差2"][i] = value[0].ToString();
                            RefUIData(startString +"误差2");
                            ResultDictionary[startString + "误差1"][i] = value[1].ToString();
                            RefUIData(startString + "误差1");

                            //跳差判断
                            if (CheckJumpError(ResultDictionary[startString + "误差1"][i], ResultDictionary[startString + "误差2"][i], meterLevel, VerifyConfig.JumpJudgment))
                            {
                                arrCheckOver[i] = false;
                                if (WCNumner[i] > VerifyConfig.ErrorMax)
                                    arrCheckOver[i] = true;
                                else
                                {
                                    MessageAdd("检测到" + string.Format("{0}", i + 1) + "跳差，重新取误差进行计算",EnumLogType.提示信息);
                                    MessageAdd("检测到" + string.Format("{0}", i + 1) + "跳差，重新取误差进行计算",EnumLogType.流程信息);
                                }
                            }
                        }

                    }
                    if (value.Length > 3)
                    {
                        ResultDictionary[startString + "平均值"][i] = value[2];
                        ResultDictionary[startString + "化整值"][i] = value[3];
                    }
                }

            }
            if (Stop) return;
            StopWcb(GetFangXianIndex(FangXiang), 0xff);//停止误差板

        }


        public bool InitEquipMent()
        { 
            if (IsDemo) return true;
            bool T=true;
            switch (Test_No)
            {
                case ProjectID.方形波波形改变:
                    T= DeviceControl.SetPowerHarmonic("1","1","1","1","1","1",1);
                    break;
                case ProjectID.尖顶波波形改变:
                    T = DeviceControl.SetPowerHarmonic("1", "1", "1", "1", "1", "1", 2);
                    break;
                case ProjectID.间谐波波形改变:
                    T = DeviceControl.SetPowerHarmonic("1", "1", "1", "1", "1", "1", 3);
                    break;
                case ProjectID.奇次谐波波形试验:
                    T = DeviceControl.SetPowerHarmonic("1", "1", "1", "1", "1", "1", 4);
                    break;
                default:
                    break;
            }
            return T;
        }
    }
}
