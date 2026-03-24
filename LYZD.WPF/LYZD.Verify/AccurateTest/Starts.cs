using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.Core.Model.Meter;
using LYZD.Core.Struct;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LYZD.Verify.AccurateTest
{
    /// <summary>
    /// 启动试验
    ///1:功率方向|启动电流倍数|是否自动计算启动电流|是否自动计算启动时间|是否默认合格|启动时间 
    ///2:功率方向|试验电压|标准试验时间|试验电流|开始时间|结束时间|实际运行时间|脉冲数
    /// </summary>
    public class Starts : VerifyBase
    {
        private StPlan_QiDong curPlan = new StPlan_QiDong();
        /// <summary>
        /// 每一块表需要的起动时间
        /// </summary>
        float[] arrStartTimes = new float[0];
        /// <summary>
        /// 每一块表需要的起动电流
        /// </summary>
        float[] arrStartCurrents = new float[0];
        /// <summary>
        /// 起动读取的第一个时间
        /// </summary>
        float[] StartTimeBefore = new float[0];
        /// <summary>
        /// 起动读取的第二个时间
        /// </summary>
        float[] StartTimeAffter = new float[0];
        /// <summary>
        /// 最终起动电流
        /// </summary>
        float startCurrent = 0F;

        WireMode clfs = WireMode.三相四线;
        /// <summary>
        /// 检定开始时间,用于检定计时
        /// </summary>
        protected DateTime m_StartTime;

        bool[] meterCheckOver = new bool[0];

        int wcCount = 6;//误差计数


        public override void Verify()
        {

            string[] PulseTime = new string[MeterNumber];                   //记录开始起动时间
            int[] PulseCount = new int[MeterNumber];                        //脉冲计数
            meterCheckOver = new bool[MeterNumber];
            float TotalTime = InitVerifyPara();                         //初始参数
            float _MaxStartTime = TotalTime * 60F;                      //计算最大起动时间
            StartTimeAffter = new float[MeterNumber];
            StartTimeBefore = new float[MeterNumber];
            m_StartTime = DateTime.Now;
            base.Verify();

            //curPlan.DefaultValue = 1;
            if (curPlan.DefaultValue == 1)    //默认合格
            {
                WaitTime("方案设置默认合格", 3);

                for (int Num = 0; Num < MeterNumber; Num++)
                {
                    if (!meterInfo[Num].YaoJianYn) continue;
                    ResultDictionary["结论"][Num] = Core.Helper.Const.合格;
                    ResultDictionary["试验电流"][Num] = startCurrent.ToString("F2");
                    ResultDictionary["开始时间"][Num] = m_StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                    ResultDictionary["功率方向"][Num] = curPlan.PowerFangXiang.ToString();
                    ResultDictionary["试验电压"][Num] = meterInfo[Num].MD_UB.ToString("F2");
                    ResultDictionary["实际运行时间"][Num] = (VerifyPassTime / 60.0).ToString("F4") + "分";
                    ResultDictionary["结束时间"][Num] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    ResultDictionary["脉冲数"][Num] = "1";
                    arrStartTimes[Num] = arrStartTimes[Num] / 60;
                }

                ConvertTestResult("标准试验时间", arrStartTimes, 2);

                RefUIData("结束时间");
                RefUIData("试验电压");
                RefUIData("试验电流");
                RefUIData("开始时间");
                RefUIData("功率方向");
                RefUIData("标准试验时间");
                RefUIData("实际运行时间");
                RefUIData("脉冲数");
                RefUIData("结论");
            }
            else
            {
                if (!IsDemo)
                {
                    if (Stop)
                    {
                        return;
                    }
                    //设置功能参数
                    int[] startTimes = new int[MeterNumber];
                    for (int bw = 0; bw < MeterNumber; bw++)
                    {
                        startTimes[bw] = (int)(arrStartTimes[bw] * 60F);
                    }
                    //输出启动电压电流
                    if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, startCurrent, startCurrent, startCurrent, Cus_PowerYuanJian.H, curPlan.PowerFangXiang, "1.0"))
                    {
                        MessageAdd("升源失败,退出检定",EnumLogType.提示信息);
                        return;
                    }
                    //

                    //启动误差版开始计数  06：正向有功脉冲计数， 07：正向无功脉冲计数， 08：反向有功脉冲计数，09 反向无功脉冲计数）
                    StartWcb(wcCount, 0xff);
                    if (Stop) return;
                }

                float[] arrStartTimes2 = new float[MeterNumber];
                #region 上报试验参数
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ResultDictionary["试验电流"][i] = startCurrent.ToString("F4");
                        ResultDictionary["开始时间"][i] = m_StartTime.ToString();
                        ResultDictionary["功率方向"][i] = curPlan.PowerFangXiang.ToString();
                        ResultDictionary["试验电压"][i] = meterInfo[i].MD_UB.ToString("F2");
                    }
                    arrStartTimes2[i] = arrStartTimes[i] / 60;
                }
                //arrStartTimes=(((float)arrStartTimes) / 60.0).ToString("F4") + "分";



                ConvertTestResult("标准试验时间", arrStartTimes2, 2);

                RefUIData("试验电压");
                RefUIData("试验电流");
                RefUIData("开始时间");
                RefUIData("功率方向");
                RefUIData("标准试验时间");
                #endregion

                CheckOver = false;
                MessageAdd("开始检定",EnumLogType.提示信息);
                StartTime = DateTime.Now;
                if (OnMeterInfo.MD_JJGC == "IR46")
                {
                    while (true)
                    {
                        //减少硬件负担，前半段时间读取频率为1次，后30%段以5秒/次的频率读取，后20%以一秒一次的频率
                        //每一秒刷新一次数据
                        long pastTime = base.VerifyPassTime;
                        Thread.Sleep(1000);
                        CheckOver = true;
                        if (!IsDemo)
                            ReadAndDealDataIR46(pastTime);
                        else
                            CheckOver = false;

                        float pastMinute = pastTime / 60F;
                        // App.CUS.NowMinute = pastMinute;
                        //string strDes = string.Format("启(起)动时间{0:F2}分，已经经过{1:F2}分", TotalTime * 3F, pastMinute);
                        string strDes = "启(起)动时间" + (TotalTime / 60).ToString("F2") + "分，已经经过" + pastMinute.ToString("F2") + "分";

                        if (MeterHelper.Instance.TypeCount > 1)
                            strDes += ",由于是多种表混检，大常数表可能提前出脉冲";
                        MessageAdd(strDes,EnumLogType.提示信息);

                        if (pastTime > TotalTime || Stop || CheckOver)
                        {
                            // App.CUS.NowMinute = TotalTime;
                            break;
                        }
                    }
                    ReadAndDealData(VerifyPassTime);
                    //进行误差实验
                    bool[] arrCheckOver = new bool[MeterNumber];
                    int[] lastNum = new int[MeterNumber];                   //保存上一次误差的序号
                    lastNum.Fill(-1);
                    List<string>[] errList = new List<string>[MeterNumber]; //记录当前误差[数组长度，]
                    for (int i = 0; i < MeterNumber; i++)
                        errList[i] = new List<string>();

                    bool isP = (curPlan.PowerFangXiang == PowerWay.正向有功 || curPlan.PowerFangXiang == PowerWay.反向有功) ? true : false;
                    int[] meterconst = MeterHelper.Instance.MeterConst(isP);
                    //CheckOver = false;

                    int index = 0;
                    if (!isP)
                    {
                        index = 1;
                    }
                    float xIb = Number.GetCurrentByIb("Ib", OnMeterInfo.MD_UA,HGQ );
    
                    long StdConst = VerifyConfig.StdConst;
                    if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb, Cus_PowerYuanJian.H, curPlan.PowerFangXiang, "1.0"))
                    {
                        MessageAdd("升源失败,退出检定",EnumLogType.提示信息);
                        return;
                    }
                    if (!VerifyConfig.ConstModel)
                    {
                        StdConst = GetStaConst(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb);
                    }
                    else
                    {
                        //1:设置标准表挡位、常数
                        MessageAdd("正在设置标准表常数...",EnumLogType.提示信息);
                        StdGear(0x13, StdConst, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, xIb, xIb, xIb);
                    }

                    SetPulseType((index + 49).ToString("x"));
        

                    //设置误差版被检常数
                    MessageAdd("正在设置误差版标准常数...",EnumLogType.提示信息);
                    SetStandardConst(0, (int)(StdConst / 100), -2);
                    //设置误差版标准常数
                    MessageAdd("正在设置误差版被检常数...",EnumLogType.提示信息);
                    int[] qs = new int[MeterNumber];
                    qs.Fill(2);
                    if (!SetTestedConst(index, meterconst, 0, qs))
                    {
                        MessageAdd("初始化误差检定参数失败",EnumLogType.提示信息);
                    }
                    MessageAdd("正在启动误差板",EnumLogType.提示信息);
                    StartWcb(index, 0xff);
                    CheckOver = false;
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
                    }
                    MessageAdd("开始基本误差检定",EnumLogType.提示信息);
                    while (!CheckOver)
                    {
                        if (Stop) break;
                        if (TimeSub(DateTime.Now, TmpTime1) > MaxTime) //超出最大处理时间
                        {
                            //NoResoult.Fill("超出最大处理时间");
                            CheckOver = true;
                            MessageAdd("超出最大处理时间,正在退出...",EnumLogType.提示信息);
                            break;
                        }

                        string[] curWC = new string[MeterNumber];   //重新初始化本次误差
                        int[] curNum = new int[MeterNumber];        //当前读取的误差序号
                        curWC.Fill("");
                        curNum.Fill(0);
                        if (!ReadWc(ref curWC, ref curNum, curPlan.PowerFangXiang))    //读取误差
                        {
                            continue;
                        }
                        if (Stop) break;
                        CheckOver = true; 
                        for (int i = 0; i < MeterNumber; i++)
                        {
                            if (Stop) break;
                            TestMeterInfo meter = meterInfo[i];      //表基本信息
                            if (!meter.YaoJianYn) arrCheckOver[i] = true;  //不检表处理

                            if (arrCheckOver[i]) continue;
                            if (lastNum[i] >= curNum[i]) continue;
                            if (curWC[i] == "-999.0000") continue;
                            if (string.IsNullOrEmpty(curWC[i])) continue;
                            if (curNum[i] <= VerifyConfig.ErrorStartCount) continue; //当前误差次数小于去除的个数    
                            lastNum[i] = curNum[i];
                            //meterLevel = MeterLevel(meter);                                 //当前表的等级
                            errList[i].Insert(0, curWC[i]);
                            //计算误差
                        }
                        //检测是否全部完成
                        for (int i = 0; i < MeterNumber; i++)
                        {
                            TestMeterInfo meter = meterInfo[i];      //表基本信息
                            if (!meter.YaoJianYn) arrCheckOver[i] = true;  //不检表处理
                            //MeterQdQid data = meter.MeterQdQids[ItemKey];
                            if (!meter.YaoJianYn) continue;
                            if (errList[i].Count > 0) arrCheckOver[i] = true;
                            if (!arrCheckOver[i])
                            {
                                 //MessageAdd($"第{i + 1}块表还没有通过",EnumLogType.提示信息);
                                CheckOver = false;
                                break;
                            }
                            else
                            {
                                //data.ErrorValue = errList[i][0].ToString();
                                //CheckOver = true;

                            }
                        }
                    }

                }       //IR46表--做完启动后做一便基本误差

                else
                {
                    while (true)
                    {
                        //减少硬件负担，前半段时间读取频率为3;，后30%段以5秒/次的频率读取，后20%以一秒一次的频率
                        //每一秒刷新一次数据
                        long pastTime = base.VerifyPassTime;
                        Thread.Sleep(1000);
                        CheckOver = true;
                        if (!IsDemo)
                            ReadAndDealData(pastTime);
                        else
                            CheckOver = false;
                        if (Stop) return;
                        float pastMinute = pastTime / 60F;
                        //GlobalUnit.g_CUS.DnbData.NowMinute = pastMinute;
                        string strDes = "启(起)动时间" + (TotalTime / 60).ToString("F2") + "分，已经经过" + pastMinute.ToString("F2") + "分";
                        if (MeterHelper.Instance.TypeCount > 1)
                        {
                            strDes += ",由于是多种表混检，大常数表可能提前出脉冲";
                        }
                        MessageAdd(strDes,EnumLogType.提示信息);
                        if (pastTime > TotalTime  || Stop || CheckOver)
                        {
                            // GlobalUnit.g_CUS.DnbData.NowMinute = _MaxStartTime / 60F;
                            break;
                        }
                    }
                    ReadAndDealData(VerifyPassTime);
                }  //其他表

            }

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    //ResultDictionary["结论"][i] = ResultDictionary["结论"][i] == "合格" ? "合格" : "不合格";
                    if (ResultDictionary["结论"][i] == "不合格")
                    {
                        ResultDictionary["结束时间"][i] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }


            StopWcb(wcCount, 0xff); //关闭误差版
            //PowerOn();
            //WaitTime("关闭电流", 5);
            RefUIData("结束时间");
            RefUIData("结论");
            MessageAdd("当前试验项目检定完毕",EnumLogType.提示信息);


            //加上关闭 设备

            //Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(false, 3);

            // MessageAdd("正在升源...",EnumLogType.提示信息);
            // if (!PowerOn(220,220,220,0,0,0, Cus_PowerYuanJian.H, PowerWay.正向有功, "1.0"))
            // {
            //     MessageAdd("升源失败,退出检定",EnumLogType.提示信息);
            //     return;
            // }
            // WaitTime("升源成功", 5);
            //string[] str=   MeterProtocolAdapter.Instance.ReadAddress();
            // bool t=  MeterProtocolAdapter.Instance.CommTest(0);
        }




        protected override bool CheckPara()
        {
            //功率方向|启动电流倍数|是否自动计算启动电流|是否自动计算启动时间|是否默认合格|启动时间
            string[] tem = Test_Value.Split('|');
            if (tem.Length != 6) return false;

            curPlan.PowerFangXiang = (PowerWay)Enum.Parse(typeof(PowerWay), tem[0]);

            if (tem[2] == "是")
            {
                curPlan.FloatxIb = 0;
            }
            else
            {
                //curPlan.FloatxIb = float.Parse(tem[1].ToLower().Replace("ib", ""));
                curPlan.FloatxIb = float.Parse(tem[1].ToLower().Replace("ib", ""));

            }
            if (tem[3] == "是")
            {
                curPlan.xTime = 0; //自动启动
            }
            else
            {
                //curPlan.xTime = float.Parse(tem[5]);
                curPlan.xTime = 1F;
                curPlan.CheckTime = float.Parse(tem[5]);
            }

            //06：正向有功脉冲计数， 07：正向无功脉冲计数， 08：反向有功脉冲计数，09 反向无功脉冲计数）</param>

            switch (tem[0])
            {
                case "正向有功":
                    wcCount = 6;
                    break;
                case "正向无功":
                    wcCount = 7;
                    break;
                case "反向有功":
                    wcCount = 6;
                    break;
                case "反向无功":
                    wcCount = 7;
                    break;
                default:
                    break;
            }

            clfs = (WireMode)Enum.Parse(typeof(WireMode), OnMeterInfo.MD_WiringMode);
            curPlan.DefaultValue = tem[4] == "是" ? 1 : 0;
            ResultNames = new string[] { "功率方向", "试验电压", "标准试验时间", "试验电流", "开始时间", "结束时间", "实际运行时间", "脉冲数", "结论" };
            return true;
        }

        /// <summary>
        /// 初始化检定参数
        /// </summary>
        /// <returns>起动时间</returns>
        private float InitVerifyPara()
        {

            //检测系统参照规程
            //计算每一块表的起动时间
            int[] meterConst = MeterHelper.Instance.MeterConst(IsYouGong);
            arrStartTimes = new float[MeterNumber];
            arrStartCurrents = new float[MeterNumber];

            for (int i = 0; i < MeterNumber; i++)
            {
                //计算起动电流
                TestMeterInfo meter = meterInfo[i];
                if (meter == null || !meter.YaoJianYn)
                {
                    continue;
                }
                bool bFind = false;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (!meterInfo[j].YaoJianYn) continue;
                    if (meter.MD_Constant == meterInfo[j].MD_Constant && meter.MD_Grane == meterInfo[j].MD_Grane)
                    {
                        arrStartTimes[i] = arrStartTimes[j];
                        arrStartCurrents[i] = arrStartCurrents[j];
                        bFind = true;
                        break;
                    }
                    if (Stop) return 0F;
                }


                if (!bFind)
                {
                    StPlan_QiDong _tagQiDong = (StPlan_QiDong)curPlan;
                    _tagQiDong.CheckTimeAndIb(meter.MD_JJGC, clfs, meter.MD_UB, meter.MD_UA, meter.MD_Grane, meter.MD_Constant, ZNQ, HGQ);
                    arrStartTimes[i] = (float)Math.Round(_tagQiDong.CheckTime, 2);
                    arrStartCurrents[i] = _tagQiDong.FloatIb;
                    /*
                    如果同一批平存在不同起动电流，则起动电流取最大值
                    */
                    if (_tagQiDong.FloatIb > startCurrent)
                    {
                        startCurrent = _tagQiDong.FloatIb;
                    }
                }
            }

            float[] arrStartTimeClone = (float[])arrStartTimes.Clone();
            Core.Function.Number.PopDesc(ref arrStartTimeClone, false);                        //选择一个最大起动时间
            if (IsDemo)
                return 1F;
            else
                return arrStartTimeClone[0];
        }


        /// <summary>
        /// 读取并处理检定数据
        /// </summary>
        private void ReadAndDealData(long verifyTime)
        {
            stError[] stErrors = ReadWcbData(GetYaoJian(), wcCount);
            CheckOver = true;
            for (int k = 0; k < MeterNumber; k++)
            {
                if (!meterInfo[k].YaoJianYn)
                {
                    meterCheckOver[k] = true;
                    continue;
                }
                if (stErrors[k]==null)
                {
                    continue;
                }

                ResultDictionary["脉冲数"][k] = stErrors[k].szError;
                if (stErrors[k].szError == "0")
                {
                    if (verifyTime >= arrStartTimes[k] )
                    {
                        NoResoult[k] = "规程启动时间内没有脉冲输出";
                        ResultDictionary["结论"][k] = Core.Helper.Const.不合格;
                        ResultDictionary["实际运行时间"][k] = (((float)verifyTime) / 60.0).ToString("F4") + "分";
                    }
                    CheckOver = false;
                }
                else if (!string.IsNullOrEmpty(stErrors[k].szError) && float.Parse(stErrors[k].szError) >= 1)
                {
                    //检测总时间是否已经超过本表起动时间
         
                    if (verifyTime <= arrStartTimes[k] * 1.5)
                    {
                        ResultDictionary["结束时间"][k] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        ResultDictionary["实际运行时间"][k] = (((float)verifyTime) / 60.0).ToString("F4") + "分";
                        ResultDictionary["结论"][k] = Core.Helper.Const.合格;
                    }
                }
                else
                {
                    CheckOver = false;
                }
                if (Stop) break;
            }

            RefUIData("结束时间");
            RefUIData("实际运行时间");
            RefUIData("脉冲数");
        }

        private void ReadAndDealDataIR46(long verifyTime)
        {
            stError[] stErrors = ReadWcbData(GetYaoJian(), wcCount);
            CheckOver = true;
            for (int i = 0; i < MeterNumber; i++)
            {
                TestMeterInfo meter = meterInfo[i];
                if (!meter.YaoJianYn) continue;

                if (stErrors[i].szError == null)
                {
                    CheckOver = false;
                    continue;
                }
                ResultDictionary["脉冲数"][i] = stErrors[i].szError;
                //   MeterQdQid data = meter.MeterQdQids[ItemKey];
                if (stErrors[i].szError == "0")
                {
                    if (verifyTime >= arrStartTimes[i] )
                    {
                        ResultDictionary["结论"][i] = Core.Helper.Const.不合格;
                        NoResoult[i] = "规程启动时间内没有脉冲输出";
                    }
                    CheckOver = false;
                }
                else if (!string.IsNullOrEmpty(stErrors[i].szError) && float.Parse(stErrors[i].szError) >= 1)
                {
                    ////检测总时间是否已经超过本表起动时间
                    //if (verifyTime < arrStartTimes[i] * 60 * 1.5 && data.PushTime1 == null)
                    //{
                    //    data.PushTime1 = (verifyTime / 60f).ToString("f2");
                    //    //data.Result = Variable.合格;

                    //}
                    //if (!string.IsNullOrEmpty(err[i].Error) && float.Parse(err[i].Error) >= 2 && data.PushTime1 != null && data.PushTime2 == null)
                    //{
                    //    data.PushTime2 = ((verifyTime / 60f) - float.Parse(data.PushTime1)).ToString("f2");
                    //    //data.Result = Variable.合格;
                    //}
                    //if (data.PushTime1 == null || data.PushTime2 == null)
                    //{
                    //    CheckOver = false;
                    //}
                    //检测总时间是否已经超过本表起动时间
                    if (verifyTime < arrStartTimes[i]*1.5 )
                    {
                        ResultDictionary["结束时间"][i] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        ResultDictionary["实际运行时间"][i] = (((float)verifyTime) / 60.0).ToString("F4") + "分";
                        ResultDictionary["结论"][i] = Core.Helper.Const.合格;
                    }
                }
                else
                {
                    CheckOver = false;
                }
                if (Stop) break;
            }
            RefUIData("结束时间");
            RefUIData("实际运行时间");
            RefUIData("脉冲数");
        }

        /// <summary>
        /// 读取误差
        /// </summary>
        /// <param name="Wc">返回误差值</param>
        /// <param name="WcNum">误差次数</param>
        /// <param name="power">功率方向</param>
        /// <returns></returns>
        private bool ReadWc(ref string[] Wc, ref int[] WcNum, PowerWay power)
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
                throw;
            }

        }
    }
}
