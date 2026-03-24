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
using System.Threading.Tasks;

namespace LYZD.Verify.AccurateTest
{
    /// <summary>
    ///  初始固有误差
    /// </summary>
    public class InitialError : VerifyBase
    {

        #region 检定参数

        /// <summary>
        /// 功率方向
        /// </summary>
        PowerWay Test_GLFX = PowerWay.正向有功;
        /// <summary>
        /// 功率因素
        /// </summary>
        string Test_GLYS = "1.0";
        /// <summary>
        /// 功率元件
        /// </summary>
        Cus_PowerYuanJian Test_YJ = Cus_PowerYuanJian.H;
        /// <summary>
        /// 电流倍数
        /// </summary>
        string Test_DLBS = "Ib";

        /// <summary>
        /// 检定的电流
        /// </summary>
        float Test_Xib = 1f;

        /// <summary>
        /// 是否是上升误差--- true下降误差上升误差--false下降误差
        /// </summary>
        bool IsUpErr = true;

        /// <summary>
        /// 未完成的标志--当只做了上升或下降一个的时候显示这个-不显示结论
        /// </summary>
        string IncompleteFlag = "未完成";

        #endregion

        #region 结论使用参数
        /// <summary>
        /// 误差限倍数--初始固有误差默认60%
        /// </summary>
        float Test_ErrorProportion = 0.6f;  //误差限倍数
        ///// <summary>
        ///// 误差上限
        ///// </summary>
        //float ErrorShangXian = 1f;
        ///// <summary>
        ///// 误差下限
        ///// </summary>
        //float ErrorXiaXian = 1f;
        #endregion

        public override void Verify()
        {
            base.Verify();
            bool[] arrCheckOver = new bool[MeterNumber];                                        //表位完成记录
            StPlan_WcPoint[] arrPlanList = new StPlan_WcPoint[MeterNumber];      // 误差点数据
            int[] arrPulseLap = new int[MeterNumber];
            int[] lastNum = new int[MeterNumber];                   //保存上一次误差的序号
            lastNum.Fill(-1);
            int[] WCNumner = new int[MeterNumber]; //检定次数
            InitVerifyPara(ref arrPlanList, ref arrPulseLap);

            #region 上传误差参数
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    ResultDictionary["误差下限"][i] = arrPlanList[i].ErrorXiaXian.ToString();
                    ResultDictionary["误差上限"][i] = arrPlanList[i].ErrorShangXian.ToString();
                    ResultDictionary["误差圈数"][i] = arrPlanList[i].LapCount.ToString();
                }
            }
            RefUIData("误差下限");
            RefUIData("误差上限");
            RefUIData("误差圈数");
            #endregion


            string directionName = "上升";
            if (!IsUpErr)
            {
                directionName = "下降";
            }

            List<string>[] errList = new List<string>[MeterNumber]; //记录当前误差[数组长度，]
            for (int i = 0; i < MeterNumber; i++)
                errList[i] = new List<string>();

            float meterLevel = MeterLevel(OnMeterInfo);
            //开始检定
            if (!IsDemo)
            {
                //if (!InitEquipment(arrPulseLap))
                if (!ErrorInitEquipment(Test_GLFX, Test_YJ, Test_GLYS, Test_DLBS, arrPulseLap[0]))
                {
                    MessageAdd("初始化基本误差设备参数失败",EnumLogType.提示信息);
                    return;
                }
                if (Stop) return;
                MessageAdd("正在启动误差版...",EnumLogType.提示信息);
                if (!StartWcb(GetFangXianIndex(Test_GLFX), 0xff))
                {
                    MessageAdd("误差板启动失败...",EnumLogType.提示信息);
                    return;
                }
                MessageAdd("开始检定...",EnumLogType.提示信息);
                int MaxTime = VerifyConfig.MaxHandleTime * 1000;
                DateTime TmpTime1 = DateTime.Now;//检定开始时间，用于判断是否超时

                //TODO 这里对于IR46的表电流小的情况有可能会超时，是否添加判断增加超时时间--后续测试再说



                bool resoult = false;
                while (true)
                {
                    if (Stop) break;
                    if (TimeSub(DateTime.Now, TmpTime1) > MaxTime && !IsMeterDebug) //超出最大处理时间并且不是调表状态
                    {
                        //NoResoult.Fill("超出最大处理时间");
                        MessageAdd("超出最大处理时间,正在退出...",EnumLogType.提示信息);
                        MessageAdd("超出最大处理时间,正在退出...",EnumLogType.流程信息);
                        break;
                    }
                    string[] curWC = new string[MeterNumber];   //重新初始化本次误差
                    int[] curNum = new int[MeterNumber];        //当前读取的误差序号
                    curWC.Fill("");
                    curNum.Fill(0);



                    MessageAdd("正在读取误差",EnumLogType.提示信息);
                    if (!ReadWc(ref curWC, ref curNum, Test_GLFX))    //读取误差
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
                        if (errList[i].Count > VerifyConfig.ErrorCount)
                            errList[i].RemoveAt(errList[i].Count - 1);
                        //计算误差
                        float[] tpmWc = ArrayConvert.ToSingle(errList[i].ToArray());  //Datable行到数组的转换
                        ErrorResoult tem = SetWuCha(arrPlanList[i], meterLevel, tpmWc);

                        if (errList[i].Count >= VerifyConfig.ErrorCount)  //误差数量>=需要的最大误差数2
                        {
                            arrCheckOver[i] = true;

                            if (!resoult)
                            {
                                if (WCNumner[i] <= VerifyConfig.ErrorMax)
                                    arrCheckOver[i] = false;
                            }
                        }
                        else
                        {
                            arrCheckOver[i] = false;
                            resoult = false;
                        }

                        ResultDictionary[directionName + "误差1"][i] = tem.Error1;
                        RefUIData(directionName + "误差1");
                        if (tem.Error2 != null && tem.Error2 != "")
                        {
                            ResultDictionary[directionName + "误差2"][i] = tem.Error2;
                            RefUIData(directionName + "误差2");
                            ResultDictionary[directionName + "误差1"][i] = tem.Error1;
                            RefUIData(directionName + "误差1");

                            bool isCheckJumpError = true;
                            //跳差判断
                            if (CheckJumpError(ResultDictionary[directionName + "误差1"][i], ResultDictionary[directionName + "误差2"][i], meterLevel, VerifyConfig.JumpJudgment))
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
                            else
                            {
                                isCheckJumpError = false;
                            }
                            ResultDictionary[directionName + "平均值"][i] = tem.ErrorPJZ;
                            ResultDictionary[directionName + "化整值"][i] = tem.ErrorHZZ;
                            float value = float.Parse(tem.ErrorHZZ);
                            //判断化整值
                            if (value <= arrPlanList[i].ErrorShangXian || value >= arrPlanList[i].ErrorXiaXian)
                            {
                                resoult = true;
                                if (!isCheckJumpError) arrCheckOver[i] = true;
                            }

                        }
                    }
                    if (Array.IndexOf(arrCheckOver, false) < 0 && !IsMeterDebug)  //全部都为true了
                        break;
                }

                RefUIData(directionName + "平均值");
                RefUIData(directionName + "化整值");

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (!meterInfo[i].YaoJianYn) continue;
                    if (ResultDictionary[directionName + "误差1"][i] == null || ResultDictionary[directionName + "误差1"][i] == "")
                    {
                        ResultDictionary[directionName + "误差1"][i] = "999";//这里是为了防止硬件出异常没有数据，软件没办法判断相对的方向是否做了，所以给个值用于代表他做过了，没读取到
                    }

                    if (ResultDictionary["上升平均值"][i] != null && ResultDictionary["上升平均值"][i] != "" && ResultDictionary["下降平均值"][i] != null && ResultDictionary["下降平均值"][i] != "")
                    {
                        ResultDictionary["差值"][i] = (float.Parse(ResultDictionary["上升平均值"][i]) - float.Parse(ResultDictionary["下降平均值"][i])).ToString("F" + VerifyConfig.PjzDigit);
                    }
                }

                RefUIData("差值");
                //反方向的名字
                string NegativeDirection = IsUpErr ? "下降" : "上升";


                //这里
                //  ResultDictionary[$"结论"][i] = tem.Resoult;
                //情况1  上升和下降做了一个--结论显示待完成
                //情况2  上升和下载不合格或差值不合格显示不合格
                //情况3  上升或下降某个数据没有--比如误差2没有的情况-显示不合格--误差1有-代表做过了
                string[] colName = new string[] { "误差1", "误差2", "化整值", "平均值" };
                string[] resoultColName = new string[] { "上升化整值", "下降化整值", "差值" };
                for (int i = 0; i < MeterNumber; i++)
                {
                    bool t = false;
                    if (!meterInfo[i].YaoJianYn) continue;
                    //当前方向下有一个是空的情况就是不合格
                    for (int j = 0; j < colName.Length; j++)
                    {
                        if (ResultDictionary[directionName + colName[j]][i] == null) ResultDictionary[directionName + colName[j]][i] = "";
                        if (ResultDictionary[directionName + colName[j]][i] == "")
                        {
                            ResultDictionary["结论"][i] = Const.不合格;
                            t = true;
                            break;
                        }
                    }
                    if (t) continue;
                    if (ResultDictionary[NegativeDirection + "误差1"][i] == null || ResultDictionary[NegativeDirection + "误差1"][i] == "") //相反方向的没有误差1，说明没有做，结论刷待完成
                    {
                        ResultDictionary["结论"][i] = "待完成";
                        continue;
                    }
                    else if (ResultDictionary[NegativeDirection + "误差1"][i] == null || ResultDictionary[NegativeDirection + "化整值"][i] == "")  //对立的有误差-但是没有化整值就是不合格
                    {
                        ResultDictionary["结论"][i] = "不合格";
                        continue;
                    }

                    ResultDictionary["结论"][i] = "合格";
                    //这里开始判断他们误差以及差值
                    for (int j = 0; j < resoultColName.Length; j++)
                    {
                        float value = float.Parse(ResultDictionary[resoultColName[j]][i]);
                        if (value > arrPlanList[i].ErrorShangXian || value < arrPlanList[i].ErrorXiaXian)
                        {
                            ResultDictionary["结论"][i] = "不合格";
                            break;
                        }
                    }
                }
                RefUIData("结论");
                StopWcb(GetFangXianIndex(Test_GLFX), 0xff);//停止误差板

            }
            else//演示模式   
            {
                for (int i = 0; i < MeterNumber; i++)
                {
                    Random rd = new Random();  //无参即为使用系统时钟为种子
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (IsUpErr) //上升
                        {

                        }
                        else  //下降
                        {

                        }
                        //float Wc1 = (float)rd.NextDouble() * arrPlanList[i].ErrorXiaXian;
                        //float Wc2 = Wc1 + rd.Next(-1000, 1000) / 10000f;
                        //float[] tpmWc = new float[2] { Wc1, Wc2 };
                        //string[] value = SetWuCha(arrPlanList[i], meterLevel, tpmWc).ErrorValue.Split('|');
                        //ResultDictionary["误差1"][i] = value[0];
                        //ResultDictionary["误差2"][i] = value[1];
                        //ResultDictionary["平均值"][i] = value[2];
                        //ResultDictionary["化整值"][i] = value[3];
                        //ResultDictionary[$"结论"][i] = SetWuCha(arrPlanList[i], meterLevel, tpmWc).Resoult;
                    }
                }

            }

          


        }
        protected override bool CheckPara()
        {
            //功率方向--功率元件-功率因素-电流倍数
            string[] data = Test_Value.Split('|');
            if (data.Length < 4) return false;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].Trim() == "") return false;
            }
            try
            {
                Test_GLFX = (PowerWay)Enum.Parse(typeof(PowerWay), data[0]);
                Test_YJ = (Cus_PowerYuanJian)Enum.Parse(typeof(Cus_PowerYuanJian), data[1]);
                Test_GLYS = data[2];
                Test_DLBS = data[3];
                Test_Xib = Number.GetCurrentByIb(Test_DLBS, OnMeterInfo.MD_UA, HGQ);
                ResultNames = new string[] { "误差下限", "误差上限", "误差圈数", "上升误差1", "上升误差2", "上升平均值", "上升化整值", "下降误差1", "下降误差2", "下降平均值", "下降化整值", "差值", "结论" };
                if (Test_Name.IndexOf("低到高") != -1)
                {
                    IsUpErr = true;
                }
                else
                {
                    IsUpErr = false;
                }

                ResultDictionary = GetCheckResult();


            }
            catch (Exception ex)
            {
                MessageAdd("初始固有误差初始化参数出错！\r\n" + ex.ToString(),EnumLogType.错误信息);
                return false;
            }

            return true;
        }

        #region --------初始化----------

        ///// <summary>
        ///// 初始化设备参数,计算每一块表需要检定的圈数
        ///// </summary>
        //private bool InitEquipment(int[] pulselap)
        //{
        //    if (IsDemo) return true;
        //    long constants;
        //    int[] meterconst = MeterHelper.Instance.MeterConst((Test_GLFX == PowerWay.正向有功 || Test_GLFX == PowerWay.反向有功) ? true : false);
        //    if (!VerifyConfig.GearModel)
        //    {
        //        StdGear(0x13, 0, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb);
        //        WaitTime("正在设置电流挡位", 2);
        //    }
        //    MessageAdd("正在升源...",EnumLogType.提示信息);
        //    if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb, Test_YJ, Test_GLFX, Test_GLYS))
        //    {
        //        MessageAdd("升源失败,退出检定",EnumLogType.提示信息);
        //        return false;
        //    }
        //    constants = VerifyConfig.StdConst;
        //    if (!VerifyConfig.ConstModel)
        //    {
        //        constants = GetStaConst(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb);
        //    }
        //    else
        //    {
        //        //1:设置标准表挡位、常数
        //        MessageAdd("正在设置标准表常数...",EnumLogType.提示信息);
        //        StdGear(0x13, constants, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb);
        //    }

        //    MessageAdd("正在设置标准表脉冲...",EnumLogType.提示信息);
        //    int index = 0;
        //    if (Test_GLFX == PowerWay.反向无功 || Test_GLFX == PowerWay.正向无功)
        //    {
        //        index = 1;
        //    }
        //    SetPulseType((index + 49).ToString("x"));
        //    if (Stop) return true;
        //    MessageAdd("开始初始化基本误差检定参数!",EnumLogType.提示信息);
        //    //设置误差版被检常数
        //    MessageAdd("正在设置误差版标准常数...",EnumLogType.提示信息);
        //    int SetConstants = (int)(constants / 100);
        //    SetStandardConst(0, SetConstants, -2, 0xff);
        //    //设置误差版标准常数 TODO2
        //    MessageAdd("正在设置误差版被检常数...",EnumLogType.提示信息);
        //    if (!SetTestedConst(index, meterconst, 0, pulselap))
        //    {
        //        MessageAdd("初始化误差检定参数失败",EnumLogType.提示信息);
        //        return false;
        //    }
        //    return true;
        //}


        /// <summary>
        /// 初始化检定参数，包括初始化虚拟表单，初始化方案参数，初始化脉冲个数
        /// </summary>
        /// <param name="planList">方案列表</param>
        /// <param name="Pulselap">检定圈数</param>
        private void InitVerifyPara(ref StPlan_WcPoint[] planList, ref int[] Pulselap)
        {
            //上报数据参数
            string[] resultKeys = new string[MeterNumber];
            planList = new StPlan_WcPoint[MeterNumber];
            Pulselap = new int[MeterNumber];
            MessageAdd("开始初始化检定参数...",EnumLogType.提示信息);
            StPlan_WcPoint CurPlan = new StPlan_WcPoint();
            CurPlan.IsCheck = true;
            CurPlan.LapCount = 2;
            CurPlan.Dif_Err_Flag = 0;
            CurPlan.nCheckOrder = 1;
            CurPlan.Pc = 0;
            CurPlan.PointId = 1;
            CurPlan.PowerDianLiu = Test_DLBS;
            CurPlan.PowerFangXiang = Test_GLFX;
            CurPlan.PowerYinSu = Test_GLYS;
            CurPlan.PowerYuanJian = Test_YJ;
            CurPlan.XiangXu = 0;
            CurPlan.XieBo = 0;
            //填充空数据
            MeterHelper.Instance.Init();
            for (int iType = 0; iType < MeterHelper.Instance.TypeCount; iType++)
            {
                //从电能表数据管理器中取每一种规格型号的电能表
                string[] mTypes = MeterHelper.Instance.MeterType(iType);
                int curFirstiType = 0;//当前类型的第一块索引
                for (int i = 0; i < mTypes.Length; i++)
                {
                    if (!Number.IsIntNumber(mTypes[i])) continue;

                    //取当前要检的表号
                    int t = int.Parse(mTypes[i]);
                    TestMeterInfo meter = meterInfo[t];

                    //resultKeys[t] = ItemKey;
                    if (meter.YaoJianYn)
                    {
                        planList[t] = CurPlan;
                        if (VerifyConfig.IsTimeWcLapCount)
                            planList[t].SetLapCount2(OnMeterInfo.MD_UB, meter.MD_UA, Clfs, planList[t].PowerYuanJian, meter.MD_Constant, planList[t].PowerYinSu, IsYouGong, HGQ, VerifyConfig.WcMinTime);
                        else
                            planList[t].SetLapCount(MeterHelper.Instance.MeterConstMin(), meter.MD_Constant, meter.MD_UA, "1.0Ib", CurPlan.LapCount);
                        planList[t].SetWcx(WcLimitName, meter.MD_JJGC, meter.MD_Grane, HGQ);
                        planList[t].ErrorShangXian *= Test_ErrorProportion;
                        planList[t].ErrorXiaXian *= Test_ErrorProportion;
                        Pulselap[t] = planList[t].LapCount;
                        curFirstiType = t;
                    }
                    else
                    {
                        //不检定表设置为第一块要检定表圈数。便于发放统一检定参数。提高检定效率
                        Pulselap[t] = planList[curFirstiType].LapCount;
                    }
                }
            }
            //重新填充不检的表位
            for (int i = 0; i < MeterNumber; i++)             //这个地方创建虚表行，多少表位创建多少行！！
            {
                //如果有不检的表则直接填充为第一块要检表的圈数
                if (Pulselap[i] == 0)
                {
                    Pulselap[i] = planList[FirstIndex].LapCount;
                }
            }
            MessageAdd("初始化检定参数完毕! ",EnumLogType.提示信息);
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
        #endregion


        #region 结论计算

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

            resoult.Error1 = string.Format(AddFlag(data[0], VerifyConfig.PjzDigit));
            if (data.Length > 1)
            {
                resoult.Error2 = string.Format(AddFlag(data[1], VerifyConfig.PjzDigit));

            }
            resoult.ErrorHZZ = HZNumber;
            resoult.ErrorPJZ = AvgNumber;
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
            return AddFlag(v);
        }

        /// <summary>
        /// 加+-符号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string AddFlag(string data)
        {
            if (float.Parse(data) > 0)
                return string.Format("+{0}", data);
            else
                return data;
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
                    { "Level2", new float[] { 0.2F, 0.04F } },               //2级表
                    { "Level3", new float[] { 0.2F, 0.04F } }               //2级表

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


        public class ErrorResoult
        {
            /// <summary>
            /// 结论
            /// </summary>
            public string Resoult;
            /// <summary>
            /// 误差1
            /// </summary>
            public string Error1;
            /// <summary>
            /// 误差2
            /// </summary>
            public string Error2;
            /// <summary>
            ///   化整值
            /// </summary>
            public string ErrorHZZ;
            /// <summary>
            ///   平均值
            /// </summary>
            public string ErrorPJZ;

        }
    }
}
