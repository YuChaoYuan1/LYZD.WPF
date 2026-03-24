using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.Core.Helper;
using LYZD.Core.Model.Meter;
using LYZD.Core.Struct;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;

namespace LYZD.Verify.AccurateTest
{
    /// <summary>
    /// 基本误差试验
    /// </summary>
    class BasicError : VerifyBase
    {
        /// <summary>
        /// 误差限倍数
        /// </summary>
        private float ErrorProportion = 1f;  //误差限倍数

        public bool IsPC = false;
        private StPlan_WcPoint CurPlan;

        //1:设置标准表挡位、常数
        //2：设置标准表脉冲方向
        //3：升源
        //4：设置误差版标准常数
        //5：设置误差版被检常数
        //6：启动误差版计算
        //7: 读取误差版                          
        //8：停止误差版
        //判断结果
        //9：关电流(电流置零                             
        //
        public override void Verify()
        {
            base.Verify();
          
            float meterLevel = 2;
            bool[] arrCheckOver = new bool[MeterNumber];                                        //表位完成记录
            int[] lastNum = new int[MeterNumber];                   //保存上一次误差的序号
            lastNum.Fill(-1);
            int[] WCNumner = new int[MeterNumber]; //检定次数
            //TODO2--标准偏差部分还没加进去，暂时用5
            int maxWCnum = IsPC ? VerifyConfig.PcCount : VerifyConfig.ErrorCount;      //每个点合格误差次数

            StPlan_WcPoint[] arrPlanList = new StPlan_WcPoint[MeterNumber];      // 误差点数据
            int[] arrPulseLap = new int[MeterNumber];
            InitVerifyPara(ref arrPlanList, ref arrPulseLap);
            if (Stop) return;

            ErrorResoult[] errorResoults = new ErrorResoult[MeterNumber];

            #region 上传误差参数
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    //"功率元件","功率方向","电流倍数","功率因数","误差下限","误差上限","误差圈数"
                    ResultDictionary["功率元件"][i] = arrPlanList[i].PowerYuanJian.ToString();
                    ResultDictionary["功率方向"][i] = arrPlanList[i].PowerFangXiang.ToString();
                    ResultDictionary["电流倍数"][i] = arrPlanList[i].PowerDianLiu;
                    ResultDictionary["功率因素"][i] = arrPlanList[i].PowerYinSu;
                    ResultDictionary["误差下限"][i] = arrPlanList[i].ErrorXiaXian.ToString();
                    ResultDictionary["误差上限"][i] = arrPlanList[i].ErrorShangXian.ToString();
                    ResultDictionary["误差圈数"][i] = arrPlanList[i].LapCount.ToString();
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

            if (IsDemo)
            {
                Random rd = new Random();  //无参即为使用系统时钟为种子
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        float Wc1 = (float)rd.NextDouble() * arrPlanList[i].ErrorXiaXian;
                        float Wc2 = Wc1 + rd.Next(-1000, 1000) / 10000f;
                        float[] tpmWc = new float[2] { Wc1, Wc2 };
                        string[] value = SetWuCha(arrPlanList[i], meterLevel, tpmWc).ErrorValue.Split('|');
                        ResultDictionary["误差1"][i] = value[0];
                        ResultDictionary["误差2"][i] = value[1];
                        ResultDictionary["平均值"][i] = value[2];
                        ResultDictionary["化整值"][i] = value[3];
                        ResultDictionary[$"结论"][i] = SetWuCha(arrPlanList[i], meterLevel, tpmWc).Resoult;
                    }
                }

                RefUIData("误差1");
                RefUIData("误差2");
                RefUIData("平均值");
                RefUIData("化整值");
                RefUIData("结论");
                return;
            }

            if (!ErrorInitEquipment(CurPlan.PowerFangXiang, CurPlan.PowerYuanJian, CurPlan.PowerYinSu, CurPlan.PowerDianLiu, arrPulseLap[0]))
            {
                MessageAdd("初始化基本误差设备参数失败", EnumLogType.错误信息);
                return;
            }
            if (Stop) return;

            List<string>[] errList = new List<string>[MeterNumber]; //记录当前误差[数组长度，]
            for (int i = 0; i < MeterNumber; i++)
                errList[i] = new List<string>();




            MessageAdd("正在启动误差版...", EnumLogType.提示信息);
            if (!StartWcb(GetFangXianIndex(CurPlan.PowerFangXiang), 0xff))
            {
                MessageAdd("误差板启动失败...", EnumLogType.错误信息);
                return;
            }
            MessageAdd("开始检定...", EnumLogType.提示信息);
            DateTime TmpTime1 = DateTime.Now;//检定开始时间，用于判断是否超时

            int MaxTime = VerifyConfig.MaxHandleTime * 1000;
            if (OnMeterInfo.MD_JJGC == "IR46")
            {
                if (OnMeterInfo.MD_UA.IndexOf("Imin") != -1)
                {
                    if (HGQ)
                        MaxTime = MaxTime * 6;
                    else
                        MaxTime = MaxTime * 2;
                }
                else if (CurPlan.PowerDianLiu.IndexOf("0.0") != -1)
                {
                    MaxTime = MaxTime * 4;
                }
            }



            //启动误差版
            while (true)
            {
                if (Stop) break;
                if (TimeSub(DateTime.Now, TmpTime1) > MaxTime && !IsMeterDebug) //超出最大处理时间并且不是调表状态
                {
                    //NoResoult.Fill("超出最大处理时间");
                    MessageAdd("超出最大处理时间,正在退出...", EnumLogType.错误信息);
                    break;
                }
                string[] curWC = new string[MeterNumber];   //重新初始化本次误差
                int[] curNum = new int[MeterNumber];        //当前读取的误差序号
                curWC.Fill("");
                curNum.Fill(0);
                if (!ReadWc(ref curWC, ref curNum, CurPlan.PowerFangXiang))    //读取误差
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

                    //计算误差
                    float[] tpmWc = ArrayConvert.ToSingle(errList[i].ToArray());  //Datable行到数组的转换

                    ErrorResoult tem;

                    //TODO2标准偏差部分还没补充，后面补
                    if (IsPC)   //标准偏差
                    {
                        tem = SetPcCha(arrPlanList[i], meterLevel, tpmWc);
                    }
                    else  //基本误差
                    {
                        tem = SetWuCha(arrPlanList[i], meterLevel, tpmWc);
                    }



                    if (errList[i].Count >= maxWCnum)  //误差数量>=需要的最大误差数2
                    {
                        arrCheckOver[i] = true;
                        if (tem.Resoult != Const.合格)
                        {
                            if (WCNumner[i] <= VerifyConfig.ErrorMax)
                            {
                                arrCheckOver[i] = false;
                                //NoResoult[i] = "超出最大误差次数";
                            }
                        }
                    }
                    else
                    {
                        arrCheckOver[i] = false;
                        tem.Resoult = Const.不合格;
                        //NoResoult[i] = "没有读取到俩次误差";
                    }
                    errorResoults[i] = tem;

                    string[] value = errorResoults[i].ErrorValue.Split('|');
                    ResultDictionary[$"误差1"][i] = value[0].ToString();
                    RefUIData($"误差1");

                    if (value.Length > 3)
                    {
                        if (value[1].ToString().Trim() != "")
                        {
                            ResultDictionary[$"误差2"][i] = value[0].ToString();
                            RefUIData($"误差2");
                            ResultDictionary[$"误差1"][i] = value[1].ToString();
                            RefUIData($"误差1");

                            //跳差判断
                            if (CheckJumpError(ResultDictionary["误差1"][i], ResultDictionary["误差2"][i], meterLevel, VerifyConfig.JumpJudgment))
                            {
                                arrCheckOver[i] = false;
                                if (WCNumner[i] > VerifyConfig.ErrorMax)
                                    arrCheckOver[i] = true;
                                else
                                {
                                    MessageAdd("检测到" + string.Format("{0}", i + 1) + "跳差，重新取误差进行计算", EnumLogType.提示与流程信息);
                                }
                            }
                            else
                            {
                                arrCheckOver[i] = true;
                            }
                        }

                    }
                    if (value.Length > 3)
                    {
                        ResultDictionary[$"平均值"][i] = value[2];
                        ResultDictionary[$"化整值"][i] = value[3];
                    }

                    ResultDictionary[$"结论"][i] = errorResoults[i].Resoult;
                }
                RefUIData($"平均值");
                RefUIData($"化整值");
                if (Array.IndexOf(arrCheckOver, false) < 0 && !IsMeterDebug)  //全部都为true了
                    break;
            }

            for (int i = 0; i < MeterNumber; i++)
            {
                if (!meterInfo[i].YaoJianYn) continue;     //表位不要检
                if (ResultDictionary[$"平均值"][i] == null || ResultDictionary[$"平均值"][i].Trim() == "")
                {
                    ResultDictionary[$"结论"][i] = Const.不合格;
                }
            }
            RefUIData("结论");
            StopWcb(GetFangXianIndex(CurPlan.PowerFangXiang), 0xff);//停止误差板
            //PowerOn();
            //WaitTime("检定完成，关闭电流",5);
            MessageAdd("检定完成", EnumLogType.提示信息);
        }


        protected override bool CheckPara() 
        {
            //误差试验类型|功率方向|功率元件|功率因素|电流倍数|添加谐波|逆相序|误差圈数|误差限倍数(%)
            if (string.IsNullOrEmpty(Test_Value) || Test_Value.Split('|').Length < 9)
            {
                MessageAdd("基本误差参数错误", EnumLogType.错误信息);
                return false;
            }

            string[] arrayErrorPara = Test_Value.Split('|');
            IsPC = arrayErrorPara[0] == "标准偏差" ? true : false;  //是否是偏差

            ErrorProportion = Convert.ToInt32(arrayErrorPara[8]) / 100F;
            if (VerifyConfig.AreaName == "北京" && ErrorProportion == 1f)   //北京流水线误差限是60%
            {
                if (VerifyConfig.ErrorRatio != "")
                {
                    ErrorProportion = ErrorProportion * (Convert.ToInt32(VerifyConfig.ErrorRatio) / 100F);
                }
            }

            CurPlan.PrjID = "111010700";
            //st_Wc.ErrorShangXian =Convert.ToInt32(arrayErrorPara[8]) / 100F;
            //st_Wc.ErrorXiaXian = -Convert.ToInt32(arrayErrorPara[8]) / 100F;
            //GlobalUnit.g_CUS.DnbData.SetWcxPercent(Convert.ToInt32(arrayErrorPara[8]) / 100F, Convert.ToInt32(arrayErrorPara[8]) / 100F);
            CurPlan.IsCheck = true;
            CurPlan.LapCount = int.Parse(arrayErrorPara[7]);
            CurPlan.Dif_Err_Flag = 0;
            CurPlan.nCheckOrder = 1;
            CurPlan.Pc = 0;
            CurPlan.PointId = 1;
            CurPlan.PowerDianLiu = arrayErrorPara[4];
            CurPlan.PowerFangXiang = (PowerWay)Enum.Parse(typeof(PowerWay), arrayErrorPara[1]);
            CurPlan.PowerYinSu = arrayErrorPara[3];
            #region 功率元件
            switch (arrayErrorPara[2])
            {
                case "H":
                    CurPlan.PowerYuanJian = Cus_PowerYuanJian.H;
                    break;
                case "A":
                    CurPlan.PowerYuanJian = Cus_PowerYuanJian.A;
                    break;
                case "B":
                    CurPlan.PowerYuanJian = Cus_PowerYuanJian.B;
                    break;
                case "C":
                    CurPlan.PowerYuanJian = Cus_PowerYuanJian.C;
                    break;
                default:
                    CurPlan.PowerYuanJian = Cus_PowerYuanJian.H;
                    break;
            }
            #endregion
            CurPlan.XiangXu = 0;
            CurPlan.XieBo = 0;

            ResultNames = new string[] { "功率元件", "功率方向", "电流倍数", "功率因素", "误差下限", "误差上限", "误差圈数", "误差1", "误差2", "平均值", "化整值", "结论" };


            return true;
        }


        #region 方法


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
        public ErrorResoult SetPcCha(StPlan_WcPoint wcPoint, float meterLevel, float[] data)
        {
            ErrorResoult resoult = new ErrorResoult();
            float space = GetWuChaHzzJianJu(true, meterLevel);   //化整间距 

            //float Windage = ZH.Core.Function.Number.GetWindage(arrNumber); //计算标准偏差
            //Windage = (float)Math.Round(Windage, App.UserSetting.AvgPercision);

            float Windage = Number.GetWindage(data); //计算标准偏差
            Windage = (float)Math.Round(Windage, VerifyConfig.PjzDigit);
            float hz = Number.GetHzz(Windage, space);

            //添加符号
            int hzPrecision = Common.GetPrecision(space.ToString());
            string AvgNumber = AddFlag(Windage, 4).ToString().Replace("+", ""); ;

            string HZNumber = hz.ToString(string.Format("F{0}", hzPrecision));
            HZNumber = AddFlag(hz, hzPrecision).Replace("+", ""); ;

            //if (avg < 0) HZNumber = HZNumber.Replace('+', '-'); //平均值<0时，化整值需为负数

            // 检测是否超过误差限
            if (Windage >= wcPoint.ErrorXiaXian && Windage <= wcPoint.ErrorShangXian)
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


        #region ----------参数初始化InitVerifyPara----------
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
            MessageAdd("开始初始化检定参数...", EnumLogType.提示信息);

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
                        bool Hgq = true;
                        if (meter.MD_ConnectionFlag == "直接式")
                        {
                            Hgq = false;
                        }
                        if (VerifyConfig.IsTimeWcLapCount)
                        {
                            planList[t].SetLapCount2(OnMeterInfo.MD_UB, meter.MD_UA, Clfs, planList[t].PowerYuanJian, meter.MD_Constant, planList[t].PowerYinSu, IsYouGong, HGQ, VerifyConfig.WcMinTime);
                        }
                        else
                        {
                            planList[t].SetLapCount(MeterHelper.Instance.MeterConstMin(), meter.MD_Constant, meter.MD_UA, "1.0Ib", CurPlan.LapCount);
                        }
                        //try
                        //{
                        //    planList[t].SetLapCount2(OnMeterInfo.MD_UB, meter.MD_UA, Clfs, planList[t].PowerYuanJian, meter.MD_Constant, planList[t].PowerYinSu, IsYouGong,Hgq );
                        //}
                        //catch (Exception)
                        //{
                        //    planList[t].SetLapCount(MeterHelper.Instance.MeterConstMin(), meter.MD_Constant, meter.MD_UA, "1.0Ib", CurPlan.LapCount);
                        //}
                        //

                        // planList[t].SetLapCount(MeterHelper.Instance.MeterConstMin(), meter.MD_Constant, meter.MD_UA, "1.0Ib", CurPlan.LapCount);
                        planList[t].SetWcx(WcLimitName, meter.MD_JJGC, meter.MD_Grane, Hgq);
                        planList[t].ErrorShangXian *= ErrorProportion;
                        planList[t].ErrorXiaXian *= ErrorProportion;
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
            MessageAdd("初始化检定参数完毕! ", EnumLogType.提示信息);
        }


        #endregion

        #region 初始化设备参数


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
    }


    public class ErrorResoult
    {
        /// <summary>
        /// 结论
        /// </summary>
        public string Resoult { get; set; }

        /// <summary>
        /// 误差值
        /// </summary>
        public string ErrorValue { get; set; }

    }
}
