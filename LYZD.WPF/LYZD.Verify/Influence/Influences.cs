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

namespace LYZD.Verify.Influence
{

    ///// <summary>
    ///// 影响量试验
    ///// </summary>
    //public class Influences : VerifyBase
    //{
    //    #region ----变量声明---- 

    //    bool[] resultKey;

    //    public bool IsPC = false;

    //    #endregion

    //    public override void Verify()
    //    {
    //        #region ---影响量流程---

    //        // 1.获取台子检定状态
    //        // 2.初始化检定参数
    //        // 3.设置设备参数
    //        // 4.启动误差板
    //        // 5.计算误差
    //        // 6.读取误差:开始先判断个表位是否要检,判断上一次读取的误差序号不能大于当前读取的误差序号,判断当前误差序号是否为空
    //        // 7.刷新界面
    //        // 8.影响后
    //        // 9.判断当前项目是否完成
    //        // 10.判断当前检定状态
    //        // 11.设置谐波参数
    //        // 12.设置设备参数
    //        // 13.启动误差板
    //        // 14.计算误差
    //        // 15.读取误差:开始先判断个表位是否要检,判断上一次读取的误差序号不能大于当前读取的误差序号,判断当前误差序号是否为空
    //        // 16.算影响后的误差1和影响后的误差2得出改变量
    //        // 17.刷新界面
    //        // 18.检定完成

    //        #endregion

    //        base.Verify();

    //        #region --------变量声明--------

    //        //StPlan_Influence StPlan = (StPlan_Influence)StPlan1;
    //        StPlan_WcPoint curPlana = (StPlan_WcPoint)CurPlana;

    //        int[] Pulselap = new int[MeterNumber];
    //        int[] arrPulseLap = new int[MeterNumber];

    //        StPlan_WcPoint[] arrPlanList = new StPlan_WcPoint[MeterNumber];

    //        bool[] arrCheckOver1 = new bool[MeterNumber]; // 表位完成记录
    //        int[] lastNum1 = new int[MeterNumber];        // 保存上一次误差的序号
    //        int[] curNum = new int[MeterNumber];          // 当前读取的误差序号
    //        string[] curWC = new string[MeterNumber];     // 本次误差
    //        int[] WCNumner1 = new int[MeterNumber];       // 检定次数
    //        int maxWCnum1 = IsPC ? VerifyConfig.PcCount : VerifyConfig.ErrorCount;    //每个点合格误差次数
    //        float meterLevel = 2;                         // 圈数

    //        string[] value1 = new string[MeterNumber];   // 误差检定
    //        #endregion


    //        if (Stop) return;
    //        MessageShow("正在初始化检定参数...");
    //        if (!InitVerifyPara(ref arrPlanList, ref arrPulseLap))
    //        {
    //            MessageShow("初始化检定参数失败");
    //            return;
    //        }

    //        #region ---上传结论参数,刷新UI---
    //        for (int i = 0; i < MeterNumber; i++)
    //        {
    //            if (meterInfo[i].YaoJianYn)
    //            {
    //                ResultDictionary["当前项目"][i] = arrPlanList[i].PrjName.ToString();
    //                ResultDictionary["误差上限"][i] = arrPlanList[i].ErrorShangXian.ToString();
    //                ResultDictionary["误差下限"][i] = arrPlanList[i].ErrorXiaXian.ToString();
    //                ResultDictionary["误差圈数"][i] = arrPlanList[i].LapCount.ToString();
    //                ResultDictionary["功率元件"][i] = arrPlanList[i].PowerYuanJian.ToString();
    //                ResultDictionary["功率方向"][i] = arrPlanList[i].PowerFangXiang.ToString();
    //                ResultDictionary["电流倍数"][i] = arrPlanList[i].PowerDianLiu.ToString();
    //            }
    //            RefUIData("当前项目");
    //            RefUIData("误差上限");
    //            RefUIData("误差下限");
    //            RefUIData("误差圈数");
    //            RefUIData("功率方向");
    //            RefUIData("功率元件");
    //            RefUIData("电流倍数");
    //        }
    //        #endregion

    //        if (Stop) return;
    //        MessageShow("正在设置设备参数...");
    //        if (!InitEquipment(CurPlana, arrPulseLap))
    //        {
    //            MessageShow("初始化误差设备参数失败");
    //            return;
    //        }
    //        List<string>[] errList1 = new List<string>[MeterNumber];
    //        for (int i = 0; i < MeterNumber; i++) errList1[i] = new List<string>();

    //        MessageShow("正在启动误差板...");
    //        if (!StartWcb(GetFangXianIndex(CurPlana.PowerFangXiang), 0xff))
    //        {
    //            MessageShow("误差板启动失败");
    //            return;
    //        }

    //        MessageShow("开始检定...");
    //        //MessageAdd
    //        DateTime TmpTime1 = DateTime.Now;  // 检定开始时间,用于判断是否超时
    //        int MaxTime1 = VerifyConfig.MaxHandleTime * 1000;
    //        if (OnMeterInfo.MD_JJGC == "IR46")
    //        {
    //            if (OnMeterInfo.MD_UA.IndexOf("Imin") != -1)
    //            {
    //                if (HGQ)
    //                {
    //                    MaxTime1 = MaxTime1 * 6;
    //                }
    //                else
    //                {
    //                    MaxTime1 = MaxTime1 * 2;
    //                }
    //            }
    //            else if (CurPlana.PowerDianLiu.IndexOf("0.0") != -1)
    //            {
    //                MaxTime1 = MaxTime1 * 4;
    //            }
    //        }
    //        while (true)
    //        {
    //            if (Stop) break;
    //            if (TimeSub(DateTime.Now, TmpTime1) > MaxTime1 && !IsMeterDebug)
    //            {
    //                MessageShow("超出最大处理时间,正在退出...");
    //                break;
    //            }
    //            curWC.Fill("");
    //            curNum.Fill(0);
    //            MessageShow("正在读取误差....");
    //            if (!ReadWc(ref curWC, ref curNum, CurPlana.PowerFangXiang))    // 读取误差
    //            {
    //                continue;
    //            }
    //            if (Stop) break;

    //            for (int i = 0; i < MeterNumber; i++)
    //            {
    //                if (!meterInfo[i].YaoJianYn) arrCheckOver1[i] = true;     //表位不要检
    //                if (arrCheckOver1[i] && !IsMeterDebug) continue;          //表位检定通过了
    //                if (lastNum1[i] >= curNum[i]) continue;
    //                if (string.IsNullOrEmpty(curWC[i])) continue;
    //                if (curNum[i] <= VerifyConfig.ErrorStartCount) continue; //当前误差次数小于去除的个数

    //                if (curNum[i] > lastNum1[i]) //大于上一次误差次数
    //                {
    //                    WCNumner1[i]++;      //检定次数
    //                    lastNum1[i] = curNum[i];
    //                }
    //                errList1[i].Insert(0, curWC[i]);
    //                if (errList1[i].Count > maxWCnum1) errList1[i].RemoveAt(errList1[i].Count - 1);
    //                meterLevel = MeterLevel(meterInfo[i]);
    //                ErrorResoult[] errorResoults1 = new ErrorResoult[MeterNumber];

    //                //计算误差
    //                float[] tpmWc = ArrayConvert.ToSingle(errList1[i].ToArray());  // Datable行到数组的转换

    //                ErrorResoult tem;

    //                if (IsPC)   //标准偏差
    //                {
    //                    tem = SetPcCha(arrPlanList[i], meterLevel, tpmWc);
    //                }
    //                else        //基本误差
    //                {
    //                    tem = SetWuCha(arrPlanList[i], meterLevel, tpmWc);
    //                }
    //                if (errList1[i].Count >= maxWCnum1)  // 误差数量>=需要的最大误差数2
    //                {
    //                    arrCheckOver1[i] = true;
    //                    if (tem.Resoult != Const.合格)
    //                    {
    //                        if (WCNumner1[i] <= VerifyConfig.ErrorMax)
    //                        {
    //                            arrCheckOver1[i] = false;
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    arrCheckOver1[i] = false;
    //                    tem.Resoult = Const.不合格;
    //                    NoResoult[i] = "没有读取到两次误差";
    //                }
    //                errorResoults1[i] = tem;

    //                value1 = errorResoults1[i].ErrorValue.Split('|');
    //                ResultDictionary[$"误差1"][i] = value1[0].ToString();
    //                RefUIData($"误差1");
    //                if (value1.Length > 3)
    //                {
    //                    if (value1[1].ToString().Trim() != "")
    //                    {
    //                        ResultDictionary[$"误差2"][i] = value1[0].ToString();
    //                        RefUIData($"误差2");
    //                        ResultDictionary[$"误差1"][i] = value1[1].ToString();
    //                        RefUIData($"误差1");
    //                        //跳差判断
    //                        if (CheckJumpError(ResultDictionary["误差1"][i], ResultDictionary["误差2"][i], meterLevel, VerifyConfig.JumpJudgment))
    //                        {
    //                            arrCheckOver1[i] = false;
    //                            if (WCNumner1[i] > VerifyConfig.ErrorMax) arrCheckOver1[i] = true;
    //                            else
    //                            {
    //                                MessageShow("检测到" + string.Format("{0}", i + 1) + "跳差，重新取误差进行计算");
    //                                MessageAdd("检测到" + string.Format("{0}", i + 1) + "跳差，重新取误差进行计算");
    //                            }
    //                        }
    //                    }
    //                }
    //                if (value1.Length > 3)
    //                {
    //                    ResultDictionary[$"平均值"][i] = value1[2];
    //                    ResultDictionary[$"化整值"][i] = value1[3];
    //                }
    //                ResultDictionary[$"结论"][i] = errorResoults1[i].Resoult;
    //            }
    //            RefUIData("平均值");
    //            RefUIData("化整值");
    //            if (Array.IndexOf(arrCheckOver1, false) < 0 && !IsMeterDebug) break;  //表位全部要检的,检定完成
    //        }
    //        StopWcb(GetFangXianIndex(CurPlana.PowerFangXiang), 0xff);  //停止误差板

    //        #region ---影响前---
    //        //if (bTestBefore)
    //        //{
    //        //    if (!IsDemo)
    //        //    {
    //        //        PowerOff();
    //        //        Thread.Sleep(3000);
    //        //    }
    //        //    if (InitEquipment(CurPlana, Pulselap)) return;

    //        //    DateTime _thisPointStartTime = DateTime.Now; // 记录检定开始时间
    //        //    curWC = new string[MeterNumber];

    //        //    while (CheckOver != true)
    //        //    {
    //        //        MessageShow("正在检定.....");
    //        //        if (Stop) break;
    //        //        //int maxSeconds = CurPlana.MaxSeconds; // 最大时间
    //        //        if (CurPlana.PrjName.IndexOf("Imin") != -1)
    //        //        {
    //        //            //maxSeconds = StPlan.MaxSeconds * 2;
    //        //        }
    //        //        else if (StPlan.Name.IndexOf("0.0") != -1 && StPlan.Name.IndexOf("50.00Hz") == -1)
    //        //        {
    //        //            //maxSeconds = StPlan.MaxSeconds * 2;
    //        //        }
    //        //        //if (TimeSub(DateTime.Now, _thisPointStartTime) > maxSeconds && !IsMeterDebug)
    //        //        //{
    //        //        //    MessageShow("超出最大处理时间,正在退出....");
    //        //        //    break;
    //        //        //}

    //        //        ReadData(ref curWC, ref curNum, StPlan.UpLimit);
    //        //        if ((CheckState & Cus_CheckStaute.调表) == Cus_CheckStaute.调表)
    //        //        {
    //        //            CheckOver = false;
    //        //        }
    //        //    }
    //        //    bTestBefore = false;


    //        //}
    //        #endregion


    //        #region ---影响后---

    //        #region ---影响后变量声明---
    //        string[] CurWc = new string[MeterNumber]; // 影响后的误差
    //        int[] CurWcNum = new int[MeterNumber]; // 影响后的检定次数
    //        ErrorResoult[] errorResoults2 = new ErrorResoult[MeterNumber];
    //        bool[] arrCheckOver2 = new bool[MeterNumber]; // 表位完成记录
    //        int[] lastNum2 = new int[MeterNumber];          // 保存上一次误差序号
    //        int[] WCNumner2 = new int[MeterNumber];         // 检定次数
    //        int maxWCnum2 = IsPC ? VerifyConfig.PcCount : VerifyConfig.ErrorCount;    // 每个点合格误差次数
    //        string[] value2 = new string[MeterNumber];
    //        #endregion

    //        #region  ---第一步:判断项目是否完成---

    //        if (CheckOver == true) return; // 是否完成项目
    //        MessageShow("正在开始检定影响后....");
    //        #endregion

    //        #region ---第二步:判断当前的运行模式---

    //        if (!IsDemo)
    //        {
    //            PowerOff();
    //            Thread.Sleep(5000);
    //        }

    //        #endregion

    //        #region ---第三步:设置谐波含量---

    //        //MessageShow("正在设置谐波含量...");
    //        //int gcxb = 15;
    //        //if (CurPlana.PrjName.IndexOf("高次谐波") != -1)
    //        //{
    //        //    if (CurPlana.PrjName.IndexOf("高次谐波正向") != -1)
    //        //    {
    //        //        CurPlana.XieBoItem[0].Num = gcxb;
    //        //        CurPlana.XieBoItem[1].Num = gcxb;
    //        //        CurPlana.XieBoItem[2].Num = gcxb;
    //        //    }
    //        //    if (CurPlana.PrjName.IndexOf("高次谐波反向") != -1)
    //        //    {
    //        //        CurPlana.XieBoItem[0].Num = fxxbsc;
    //        //        CurPlana.XieBoItem[1].Num = fxxbsc;
    //        //        CurPlana.XieBoItem[2].Num = fxxbsc;
    //        //    }
    //        //    CheckOver = false;
    //        //}
    //        //if (CurPlana.PrjName.IndexOf("逆相序试验") != -1)
    //        //{
    //        //    for (int i = 0; i < MeterNumber; i++)
    //        //    {
    //        //        if (meterInfo[i].YaoJianYn != false)
    //        //        {
    //        //            //CurPlana.XiangXu = 1;
    //        //        }
    //        //    }
    //        //}
    //        //if (CurPlana.PrjName.IndexOf("电源电压") != -1 && CurPlana.xUa < 0.8)
    //        //{
    //        //    CurPlana.ErrorShangXian = 10;
    //        //    CurPlana.ErrorXiaXian = -100;
    //        //    for (int i = 0; i < MeterNumber; i++)
    //        //    {
    //        //        if (meterInfo[i].YaoJianYn != false)
    //        //        {
    //        //            CurWc[i] = "-100";
    //        //            CurWcNum[i]++;
    //        //        }
    //        //    }
    //        //}
    //        //if (CurPlana.PrjName.IndexOf("频率影响+2%") != -1 || CurPlana.PrjName.IndexOf("频率影响-2%") != -1)
    //        //{
    //        //    float frequency1 = CurPlana.PL; //频率影响+2%
    //        //    float frequency2 = CurPlana.PL; //频率影响-2%
    //        //    if (CurPlana.PrjName.IndexOf("频率影响+2") != -1)
    //        //    {
    //        //        float f1 = (float)(frequency1 * 0.2);
    //        //        for (int i = 0; i < MeterNumber; i++)
    //        //        {
    //        //            if (meterInfo[i].YaoJianYn != false)
    //        //            {
    //        //                CurWc[i] = f1.ToString();
    //        //                CurWcNum[i]++;
    //        //            }
    //        //        }
    //        //    }
    //        //    if (CurPlana.PrjName.IndexOf("频率影响-2%") != -1)
    //        //    {
    //        //        float f2 = (float)(frequency2 * 0.2);
    //        //        for (int i = 0; i < MeterNumber; i++)
    //        //        {
    //        //            if (meterInfo[i].YaoJianYn != false)
    //        //            {
    //        //                CurWc[i] = f2.ToString();
    //        //                curNum[i]++;
    //        //            }
    //        //        }
    //        //    }
    //        //}
    //        //MessageShow("谐波含量设置完成");
    //        #endregion

    //        #region ---第四步:初始化检定参数---

    //        MessageShow("正在初始化检定参数");
    //        if (!InitEquipMent(ref curPlana, ref arrPulseLap))
    //        {
    //            MessageShow("初始化检定参数失败");
    //            return;
    //        }
    //        if (Stop) return;

    //        #endregion

    //        #region ---第五步:设置设备参数---

    //        MessageShow("正在设置设备参数...");
    //        if (!InitEquipMent(CurPlana, arrPulseLap))
    //        {
    //            MessageShow("初始化误差设备参数失败");
    //            return;
    //        }
    //        List<string>[] errList2 = new List<string>[MeterNumber];
    //        for (int i = 0; i < MeterNumber; i++) errList2[i] = new List<string>();

    //        #endregion

    //        #region ---第六步:启动误差板---

    //        MessageShow("正在启动误差板...");
    //        if (!StartWcb(GetFangXianIndex(CurPlana.PowerFangXiang), 0xff))
    //        {
    //            MessageShow("误差板启动失败");
    //            return;
    //        }
    //        MessageShow("误差板启动成功");
    //        #endregion

    //        #region ---第七步:检定开始时间,判断有没有超过最大时间---

    //        DateTime TmpTime2 = DateTime.Now;  // 再次检定开始时间,用于判断是否超时
    //        int MaxTime2 = VerifyConfig.MaxHandleTime * 1000;
    //        if (OnMeterInfo.MD_JJGC == "IR46")
    //        {
    //            if (OnMeterInfo.MD_UA.IndexOf("Imin") != -1)
    //            {
    //                if (HGQ)
    //                {
    //                    MaxTime2 = MaxTime2 * 6;
    //                }
    //                else
    //                {
    //                    MaxTime2 = MaxTime2 * 2;
    //                }
    //            }
    //            else if (CurPlana.PowerDianLiu.IndexOf("0.0") != -1)
    //            {
    //                MaxTime2 = MaxTime2 * 4;
    //            }
    //            if (DateTimes.DateDiff(TmpTime2) > MaxTime2 && !IsMeterDebug)
    //            {
    //                MessageShow("当前检定点已经超过最大检定时间" + string.Format("{0}", MaxTime2) + "秒!");
    //                CheckOver = true;
    //            }
    //        }

    //        #endregion

    //        while (true)
    //        {
    //            MessageShow("正在检定...");
    //            if (Stop) break;
    //            if (TimeSub(DateTime.Now, TmpTime2) > MaxTime2 && !IsMeterDebug)
    //            {
    //                MessageShow("超出最大处理时间,正在退出...");
    //                break;
    //            }
    //            if (Stop) break;

    //            #region ---第九步:读取误差---

    //            CurWc = new string[MeterNumber];
    //            CurWcNum = new int[MeterNumber];
    //            CurWc.Fill("");
    //            CurWcNum.Fill(0);
    //            MessageShow("正在读取误差...");
    //            if (!ReadWc(ref CurWc, ref CurWcNum, CurPlana.PowerFangXiang))    //读取误差
    //            {
    //                continue;
    //            }

    //            #endregion

    //            #region ---第十步:获取表位的检定状态---

    //            for (int i = 0; i < MeterNumber; i++)
    //            {
    //                #region ---第十一步:表位是否要检定---
    //                if (!meterInfo[i].YaoJianYn) arrCheckOver2[i] = true;    // 表位不要检
    //                if (arrCheckOver2[i] && !IsMeterDebug) continue;         // 表位检定通过了
    //                if (lastNum2[i] >= CurWcNum[i]) continue;

    //                if (string.IsNullOrEmpty(CurWc[i])) continue;
    //                if (CurWcNum[i] <= VerifyConfig.ErrorStartCount) continue; //当前误差次数小于去除的个数

    //                if (CurWcNum[i] > lastNum2[i]) //大于上一次误差次数1
    //                {
    //                    WCNumner2[i]++;   //检定次数
    //                    lastNum2[i] = CurWcNum[i];
    //                }
    //                errList2[i].Insert(0, CurWc[i]);
    //                if (errList2[i].Count > maxWCnum2) errList2[i].RemoveAt(errList2[i].Count - 1);
    //                meterLevel = MeterLevel(meterInfo[i]);

    //                #endregion

    //                #region ---第十二步:计算误差---

    //                float[] tpmWc = ArrayConvert.ToSingle(errList2[i].ToArray());  //Datable行到数组的转换
    //                ErrorResoult tem;
    //                if (IsPC)   //标准偏差
    //                {
    //                    tem = SetPcCha(arrPlanList[i], meterLevel, tpmWc);
    //                }
    //                else        //基本误差
    //                {
    //                    tem = SetWuCha(arrPlanList[i], meterLevel, tpmWc);
    //                }
    //                if (errList2[i].Count >= maxWCnum2)  //误差数量>=需要的最大误差数2
    //                {
    //                    arrCheckOver2[i] = true;
    //                    if (tem.Resoult != Const.合格)
    //                    {
    //                        if (WCNumner2[i] <= VerifyConfig.ErrorMax)
    //                        {
    //                            arrCheckOver2[i] = false;
    //                        }
    //                    }
    //                }

    //                #endregion

    //                #region ---第十三步:刷新检定表位的影响误差界面---

    //                else
    //                {
    //                    arrCheckOver2[i] = false;
    //                    tem.Resoult = Const.不合格;
    //                    NoResoult[i] = "没有读取到俩次误差";
    //                }
    //                errorResoults2[i] = tem;

    //                value2 = errorResoults2[i].ErrorValue.Split('|');
    //                ResultDictionary[$"影响后误差1"][i] = value2[0].ToString();
    //                RefUIData($"影响后误差1");
    //                MessageShow("出影响后误差1...");

    //                if (value2.Length > 3)
    //                {
    //                    if (value2[1].ToString().Trim() != "")
    //                    {
    //                        ResultDictionary[$"影响后误差2"][i] = value2[0].ToString();
    //                        RefUIData($"影响后误差2");
    //                        MessageShow("出影响后误差2...");
    //                        ResultDictionary[$"影响后误差1"][i] = value2[1].ToString();
    //                        RefUIData($"影响后误差");
    //                        MessageShow("出影响后误差1...");

    //                        #region --跳差判断--

    //                        if (CheckJumpError(ResultDictionary[$"影响后误差1"][i], ResultDictionary[$"影响后误差2"][i], meterLevel, VerifyConfig.JumpJudgment))
    //                        {
    //                            arrCheckOver2[i] = false;
    //                            if (WCNumner2[i] > VerifyConfig.ErrorMax) arrCheckOver2[i] = true;
    //                            else
    //                            {
    //                                MessageShow("检测到" + string.Format("{0}", i + 1) + "跳差，重新取误差进行计算");
    //                                MessageAdd("检测到" + string.Format("{0}", i + 1) + "跳差，重新取误差进行计算");
    //                            }
    //                        }
    //                        #endregion   
    //                    }
    //                }
    //                if (value2.Length > 3)
    //                {
    //                    ResultDictionary[$"影响后平均值"][i] = value2[2];
    //                    ResultDictionary[$"影响后化整值"][i] = value2[3];
    //                }
    //                ResultDictionary["结论"][i] = errorResoults2[i].Resoult;

    //                if (value2.Length > 3)
    //                {
    //                    for (int j = 0; j < MeterNumber; j++)
    //                    {
    //                        float[] f1 = ArrayConvert.ToSingle(value2.ToArray());
    //                        float f2 = Number.GetAvgA(f1, -999F);

    //                        float f5 = Convert.ToSingle(value2[2]);
    //                        float f6 = Convert.ToSingle(value1[2]);
    //                        float f4 = Change(f6, f5, f2);
    //                        ResultDictionary["改变量"][i] = f4.ToString("#0.00");
    //                        MessageShow("正在出改变量...");
    //                    }
    //                }
    //                #endregion
    //            }
    //            #endregion

    //            RefUIData($"影响后平均值");
    //            RefUIData($"影响后化整值");
    //            RefUIData("改变量");

    //            #region ---第十四步:判断表位检定是否完成,并且不是调表状态---

    //            if (Array.IndexOf(arrCheckOver2, false) < 0 && !IsMeterDebug) break; // 全部表位检定完成并且不是调表状态

    //            #endregion

    //            #endregion

    //            if (CurPlana.PrjName.IndexOf("偶次") != -1)
    //            {
    //                DeviceControl.setZH3001PowerHarmonic("1", "1", "1", "1", "1", "1", 5);
    //            }
    //            else if (CurPlana.PrjName.IndexOf("谐波") != -1)
    //            {
    //                DeviceControl.setZH3001PowerHarmonic("1", "1", "1", "1", "1", "1", 0);
    //            }
    //            MessageShow("正在出检定结论...");
    //            for (int i = 0; i < MeterNumber; i++)
    //            {
    //                if (ResultDictionary[$"影响后平均值"][i] == null || ResultDictionary[$"影响后平均值"][i].Trim() == "" || ResultDictionary[$"影响后误差1"][i] == null)
    //                {
    //                    ResultDictionary[$"结论"][i] = Const.不合格;
    //                }
    //                else
    //                {
    //                    ResultDictionary[$"结论"][i] = Const.合格;
    //                }
    //            }
    //        }
    //        RefUIData("结论");
    //        MessageShow("正在停止误差板...");
    //        StopWcb(GetFangXianIndex(CurPlana.PowerFangXiang), 0xff);
    //        MessageShow("检定完成");
    //    }

    //    #region ----检定方法----

    //    #region ---检定参数---
    //    /// <summary>
    //    /// 误差值
    //    /// </summary>
    //    public string Error { get; set; }

    //    /// <summary>
    //    ///  标识当前属于第几次误差
    //    /// </summary>
    //    public int Index { get; set; }

    //    /// <summary>
    //    /// 误差检定方式
    //    /// </summary>
    //    public ErrorVerifyType error = ErrorVerifyType.误差板;


    //    /// <summary>
    //    /// 检定状态
    //    /// </summary>
    //    public Cus_CheckStaute CheckState = Cus_CheckStaute.停止检定;

    //    /// <summary>
    //    /// 误差限倍数
    //    /// </summary>
    //    private float ErrorProportion = 1f;

    //    /// <summary>
    //    /// 当前误差点的检定方案
    //    /// </summary>
    //    private StPlan_WcPoint CurPlana;

    //    /// <summary>
    //    /// 当前影响量的检定方案
    //    /// </summary>
    //    public StPlan_Influence StPlan1;

    //    public class ErrorResoult
    //    {
    //        /// <summary>
    //        /// 结论
    //        /// </summary>
    //        public string Resoult { get; set; }

    //        /// <summary>
    //        /// 误差值
    //        /// </summary>
    //        public string ErrorValue { get; set; }
    //    }
    //    #endregion

    //    /// <summary>
    //    /// 一相或两相电压中断试验
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool PowerON()
    //    {
    //        if (IsDemo) return true;
    //        if (CurPlana.PrjName == "一相电压中断试验")
    //        {
    //            MessageShow("正在开始一相中断试验...开始升源");
    //            float XIb1 = Number.GetCurrentByIb(CurPlana.PowerDianLiu, OnMeterInfo.MD_UA, HGQ);
    //            PowerOn(OnMeterInfo.MD_UB, 0, OnMeterInfo.MD_UB, XIb1, 0, XIb1, Cus_PowerYuanJian.H, FangXiang, "1.0");
    //        }
    //        else if (CurPlana.PrjName == "两相电压中断试验")
    //        {
    //            MessageShow("正在开始两相中断试验...开始升源");
    //            float XIb2 = Number.GetCurrentByIb(CurPlana.PowerDianLiu, OnMeterInfo.MD_UA, HGQ);
    //            PowerOn(OnMeterInfo.MD_UB, 0, 0, XIb2, 0, 0, Cus_PowerYuanJian.H, FangXiang, "1.0");
    //        }
    //        return true;
    //    }


    //    /// <summary>
    //    /// 设置谐波参数
    //    /// </summary>
    //    /// <param name="stPlan">当前检定方案</param>
    //    /// <param name="pulselap">检定圈数</param>
    //    /// <returns></returns>
    //    public bool InitEquipMent(ref StPlan_WcPoint stPlan, ref int[] pulselap)
    //    {
    //        if (IsDemo) return true;
    //        int[] meterconst = new int[MeterNumber];

    //        if (CurPlana.PrjName.IndexOf("谐波") != -1)
    //        {
    //            if (CurPlana.XieBoFa == "方顶波" || CurPlana.XieBoFa == "方形波")
    //            {
    //                DeviceControl.setZH3001PowerHarmonic("1", "1", "1", "1", "1", "1", 1);
    //            }
    //            else if (CurPlana.XieBoFa == "尖顶波")
    //            {
    //                DeviceControl.setZH3001PowerHarmonic("1", "1", "1", "1", "1", "1", 2);
    //            }
    //            else if (CurPlana.XieBoFa == "次谐波" || CurPlana.XieBoFa == "间谐波")
    //            {
    //                DeviceControl.setZH3001PowerHarmonic("1", "1", "1", "1", "1", "1", 3);
    //            }
    //            else if (CurPlana.XieBoFa == "奇次谐波")
    //            {
    //                DeviceControl.setZH3001PowerHarmonic("1", "1", "1", "1", "1", "1", 4);
    //            }
    //            else if (CurPlana.XieBoFa == "无")
    //            {
    //                DeviceControl.setZH3001PowerHarmonic("1", "1", "1", "1", "1", "1", 0);
    //            }
    //            else if (CurPlana.XieBoFa == "偶次谐波")
    //            {
    //                //DeviceControl.setZH3001PowerHarmonic("1", "1", "1", "1", "1", "1", 5);
    //                EquipmentData.DeviceManager.SetEvenHarmonic("1", "1", "1", 0);
    //            }
    //            //else
    //            //{
    //            //    OpenXieBoSet(CurPlana.XieBoItem);
    //            //}
    //            PowerWay power = CurPlana.PowerFangXiang;

    //            float XIb = Number.GetCurrentByIb(CurPlana.PowerDianLiu, OnMeterInfo.MD_UA, HGQ);

    //            if (OnMeterInfo.MD_JJGC == "IR46")
    //            {
    //                XIb = Number.GetCurrentByIb(OnMeterInfo.MD_UA, CurPlana.PowerDianLiu);
    //                if (OnMeterInfo.MD_UA.IndexOf("Ib") != -1)
    //                {
    //                    XIb = XIb * 10;
    //                }
    //            }
    //            else
    //            {
    //                XIb = Number.GetCurrentByIb(OnMeterInfo.MD_UA, CurPlana.PowerDianLiu);
    //            }
    //            DeviceControl.setZH3001PowerHarmonic("1", "1", "1", "1", "1", "1", 1);

    //            if (Stop) return false;
    //            bool isP = (power == PowerWay.正向有功 || power == PowerWay.反向有功) ? true : false;
    //            //获取所有表的参数
    //            meterconst = MeterHelper.Instance.MeterConst(isP);

    //            if (CurPlana.XieBoFa == "偶次谐波")
    //            {
    //                MessageShow("正在等待输出偶次谐波....");
    //                Thread.Sleep(8000);

    //                float[] floatData = new float[6];
    //                //读取偶次谐波电流采样值
    //                int ID = 0;
    //                EquipmentData.DeviceManager.GetEvenHarmonicValue(out floatData, ID);
    //                float DLPJ = (floatData[0] + floatData[1] + floatData[2] + floatData[3] + floatData[4] + floatData[5]) / 2F;

    //                int const1 = Convert.ToInt32(meterconst[0] / 2 * ((floatData[0] + floatData[2] + floatData[4]) / DLPJ));
    //                int const2 = Convert.ToInt32(meterconst[0] / 2 * ((floatData[1] + floatData[3] + floatData[5]) / DLPJ));
    //                for (int i = 0; i < meterconst.Length; i++)
    //                {
    //                    meterconst[i] = const1;
    //                    if (i >= (meterconst.Length / 2)) meterconst[i] = const2;
    //                }
    //            }
    //        }
    //        if (CurPlana.PrjName == "一相电压中断试验" || CurPlana.PrjName == "两相电压中断试验")
    //        {
    //            PowerON();
    //        }
    //        else if (CurPlana.PrjName == "频率影响+2%" || CurPlana.PrjName == "频率影响-2%")
    //        {
    //            MessageShow("开始检定");
    //            float value = float.Parse(CurPlana.PrjName.Replace("频率影响", "").Replace("%", "")) / 100;
    //            float f1 = (float)(CurPlana.PL * value) + CurPlana.PL;
    //            float PL = 0F;
    //            for (int i = 0; i < MeterNumber; i++)
    //                if (meterInfo[i].YaoJianYn != false) PL = (int)f1;

    //            Cus_PowerPhase nxx = Cus_PowerPhase.正相序;
    //            float XIb2 = Number.GetCurrentByIb(CurPlana.PowerDianLiu, OnMeterInfo.MD_UA, HGQ);
    //            MessageShow("开始频率影响的升源");
    //            DeviceControl.PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, XIb2, XIb2, XIb2, CurPlana.PowerYuanJian, PL, CurPlana.PowerYinSu, nxx, CurPlana.PowerFangXiang);
    //        }
    //        else if (CurPlana.PrjName == "逆相序试验")
    //        {
    //            MessageShow("正在进行逆相序试验....开始升源");
    //            float XIb2 = Number.GetCurrentByIb(CurPlana.PowerDianLiu, OnMeterInfo.MD_UA, HGQ);
    //            Cus_PowerPhase nxx = Cus_PowerPhase.逆相序;
    //            DeviceControl.PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, XIb2, XIb2, XIb2, CurPlana.PowerYuanJian, 50F, CurPlana.PowerYinSu, nxx, CurPlana.PowerFangXiang);
    //        }
    //        return true;
    //    }

    //    /// <summary>
    //    /// 改变量计算
    //    /// </summary>
    //    /// <param name="data1">要化整的数字</param>
    //    /// <param name="data2">要化整的数字</param>
    //    /// <param name="space">化整间距</param>
    //    /// <returns>改变后的浮点数</returns>
    //    public static float Change(float data1, float data2, float space)
    //    {
    //        float abs = Math.Max(data1, data2);
    //        int flag = abs > 0 ? 1 : -1;
    //        if (space != -1)
    //        {
    //            abs /= space;
    //        }
    //        int inte = (int)abs;
    //        float dot = abs - inte;
    //        if (dot > 0.5F)
    //        {
    //            inte++;
    //        }
    //        else if (dot == 0.5F && inte % 2 == 1)
    //        {
    //            inte++;
    //        }
    //        abs = flag * inte * space;
    //        return abs;
    //    }

    //    public string[] BasicError2(float MeterLevel, ref int[] wcTimes)
    //    {
    //        int _WcTimes = 0;
    //        string[] wcGroup = new string[MeterNumber];
    //        Array.Resize(ref wcTimes, MeterNumber);
    //        _WcTimes++;

    //        for (int i = 0; i < MeterNumber; i++)
    //        {
    //            wcGroup[i] = BasicError(MeterLevel);
    //            wcTimes[i] = _WcTimes;
    //        }
    //        return wcGroup;
    //    }

    //    /// <summary>
    //    /// 产生一个随机误差
    //    /// </summary>
    //    /// <param name="MeterLevel">表等级</param>
    //    /// <returns></returns>
    //    private string BasicError(float MeterLevel)
    //    {
    //        MeterLevel *= 10000; //扩大10000倍

    //        float tmpWC = new Random().Next((int)(-MeterLevel), (int)MeterLevel);
    //        tmpWC = tmpWC / 100000F;

    //        //延时0.2S
    //        int delayTime = 1000 / MeterNumber;
    //        Thread.Sleep(delayTime);

    //        return tmpWC.ToString();
    //    }


    //    /// <summary>
    //    /// 初始化检定参数 
    //    /// </summary>
    //    /// <param name="planList"></param>
    //    /// <param name="Pulselap"></param>
    //    public bool InitVerifyPara(ref StPlan_WcPoint[] planList, ref int[] Pulselap)
    //    {
    //        //上报数据参数
    //        string[] resultKeys = new string[MeterNumber];
    //        planList = new StPlan_WcPoint[MeterNumber];
    //        Pulselap = new int[MeterNumber];
    //        MessageShow("开始初始化检定参数....");

    //        //填充空数据
    //        MeterHelper.Instance.Init();

    //        for (int iType = 0; iType < MeterHelper.Instance.TypeCount; iType++)
    //        {
    //            //从电能表数据管理器中取每一种规格型号的电能表
    //            string[] mTypes = MeterHelper.Instance.MeterType(iType);
    //            int curFirstiType = 0;//当前类型的第一块索引
    //            for (int i = 0; i < mTypes.Length; i++)
    //            {
    //                if (!Number.IsIntNumber(mTypes[i])) continue;

    //                //取当前要检的表号
    //                int t = int.Parse(mTypes[i]);
    //                TestMeterInfo meter = meterInfo[t];

    //                if (meter.YaoJianYn)
    //                {
    //                    planList[t] = CurPlana;
    //                    bool Hgq = true;
    //                    if (meter.MD_ConnectionFlag == "直接式")
    //                    {
    //                        Hgq = false;
    //                    }
    //                    if (VerifyConfig.IsTimeWcLapCount)
    //                    {
    //                        planList[t].SetLapCount2(OnMeterInfo.MD_UB, meter.MD_UA, Clfs, planList[t].PowerYuanJian, meter.MD_Constant, planList[t].PowerYinSu, IsYouGong, HGQ, VerifyConfig.WcMinTime);
    //                    }
    //                    else
    //                    {
    //                        planList[t].SetLapCount(MeterHelper.Instance.MeterConstMin(), meter.MD_Constant, meter.MD_UA, "1.0Ib", CurPlana.LapCount);
    //                    }
    //                    planList[t].SetWcx(WcLimitName, meter.MD_JJGC, meter.MD_Grane, Hgq);
    //                    planList[t].ErrorShangXian *= ErrorProportion;
    //                    planList[t].ErrorXiaXian *= ErrorProportion;
    //                    Pulselap[t] = planList[t].LapCount;
    //                    curFirstiType = t;
    //                }
    //                else
    //                {
    //                    //不检定表设置为第一块要检定表圈数。便于发放统一检定参数。提高检定效率
    //                    Pulselap[t] = planList[curFirstiType].LapCount;
    //                }
    //            }
    //        }
    //        //重新填充不检的表位
    //        for (int i = 0; i < MeterNumber; i++)             //这个地方创建虚表行，多少表位创建多少行
    //        {
    //            //如果有不检的表则直接填充为第一块要检表的圈数
    //            if (Pulselap[i] == 0)
    //            {
    //                Pulselap[i] = planList[FirstIndex].LapCount;
    //            }
    //        }
    //        MessageShow("初始化检定参数完毕! ");
    //        return true;
    //    }

    //    /// <summary>
    //    /// 设置设备参数
    //    /// </summary>
    //    /// <returns></returns>
    //    private bool InitEquipment(StPlan_WcPoint CurPlana, int[] pulselap)
    //    {
    //        if (IsDemo) return true;
    //        StPlan_Influence st = new StPlan_Influence();
    //        bool isP = (st.PowerWay == PowerWay.正向有功 || st.PowerWay == PowerWay.反向有功) ? true : false;
    //        int[] meterconst = MeterHelper.Instance.MeterConst(isP);
    //        long constants = (st.PowerWay == PowerWay.正向有功 || st.PowerWay == PowerWay.反向有功) ? MeterHelper.Instance.MeterConstMin()[0] :
    //            MeterHelper.Instance.MeterConstMin()[1];
    //        float XIb = Number.GetCurrentByIb(CurPlana.PowerDianLiu, OnMeterInfo.MD_UA, HGQ);

    //        if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, XIb, XIb, XIb, Cus_PowerYuanJian.H, FangXiang, "1.0"))
    //        {
    //            MessageShow("升源失败,退出检定");
    //            return true;
    //        }
    //        constants = VerifyConfig.StdConst;
    //        if (!VerifyConfig.ConstModel)
    //        {
    //            constants = GetStaConst(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, XIb, XIb, XIb);
    //        }
    //        else
    //        {
    //            MessageShow("正在设置标准表常数...");
    //            StdGear(0x13, constants, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, XIb, XIb, XIb);
    //        }
    //        MessageShow("正在设置标准表脉冲...");
    //        int index = 0;
    //        if (st.PowerWay == PowerWay.反向无功 || st.PowerWay == PowerWay.正向无功)
    //        {
    //            index = 1;
    //        }
    //        SetPulseType((index + 49).ToString("x"));
    //        if (Stop) return true;
    //        MessageShow("开始初始化基本误差检定参数!");
    //        //设置误差板被检常数
    //        MessageShow("正在设置误差板标准常数...");
    //        int SetConstants = (int)(constants / 100);
    //        SetStandardConst(0, SetConstants, -2, 0xff);
    //        //设置误差板标准常数
    //        MessageShow("正在设置误差板被检常数...");
    //        if (!SetTestedConst(index, meterconst, 0, pulselap))
    //        {
    //            MessageShow("初始化误差检定参数失败");
    //            return false;
    //        }
    //        return true;
    //    }

    //    /// <summary>
    //    /// 影响后设置设备参数
    //    /// </summary>
    //    /// <param name="CurPlana"></param>
    //    /// <param name="pulselap"></param>
    //    /// <returns></returns>
    //    private bool InitEquipMent(StPlan_WcPoint CurPlana, int[] pulselap)
    //    {
    //        if (IsDemo) return true;
    //        StPlan_Influence st1 = new StPlan_Influence();
    //        bool isP1 = (st1.PowerWay == PowerWay.正向有功 || st1.PowerWay == PowerWay.反向有功) ? true : false;
    //        int[] meterconst1 = MeterHelper.Instance.MeterConst(isP1);
    //        long constants1 = (st1.PowerWay == PowerWay.正向有功 || st1.PowerWay == PowerWay.反向有功) ? MeterHelper.Instance.MeterConstMin()[0] :
    //            MeterHelper.Instance.MeterConstMin()[1];
    //        float XIb1 = Number.GetCurrentByIb(CurPlana.PowerDianLiu, OnMeterInfo.MD_UA, HGQ);

    //        if (CurPlana.PrjName != "一相电压中断试验" && CurPlana.PrjName != "两相电压中断试验" && CurPlana.PrjName != "逆相序试验" && CurPlana.PrjName != "频率影响")
    //        {
    //            if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, XIb1, XIb1, XIb1, Cus_PowerYuanJian.H, FangXiang, "1.0"))
    //            {
    //                MessageShow("升源失败,退出检定");
    //                return true;
    //            }
    //        }
    //        constants1 = VerifyConfig.StdConst;
    //        if (!VerifyConfig.ConstModel)
    //        {
    //            constants1 = GetStaConst(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, XIb1, XIb1, XIb1);
    //        }
    //        else
    //        {
    //            MessageShow("正在设置标准表常数...");
    //            StdGear(0x13, constants1, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, XIb1, XIb1, XIb1);
    //        }
    //        MessageShow("正在设置标准表脉冲...");
    //        int index = 0;
    //        if (st1.PowerWay == PowerWay.反向无功 || st1.PowerWay == PowerWay.正向无功)
    //        {
    //            index = 1;
    //        }
    //        SetPulseType((index + 49).ToString("x"));
    //        if (Stop) return true;
    //        MessageShow("开始初始化基本误差检定参数!");
    //        //设置误差板被检常数
    //        MessageShow("正在设置误差板标准常数...");
    //        int SetConstants = (int)(constants1 / 100);
    //        SetStandardConst(0, SetConstants, -2, 0xff);
    //        //设置误差板标准常数
    //        MessageShow("正在设置误差板被检常数...");
    //        if (!SetTestedConst(index, meterconst1, 0, pulselap))
    //        {
    //            MessageShow("初始化误差检定参数失败");
    //            return false;
    //        }
    //        return true;

    //    }
    //    /// <summary>
    //    /// 计算基本误差
    //    /// </summary>
    //    /// <param name="data">要参与计算的误差数组</param>
    //    /// <returns></returns>
    //    public ErrorResoult SetWuCha(StPlan_WcPoint wcPoint, float meterLevel, float[] data)
    //    {
    //        ErrorResoult resoult = new ErrorResoult();
    //        float space = GetWuChaHzzJianJu(false, meterLevel);                              //化整间距 
    //        float avg = Number.GetAvgA(data);
    //        float hz = Number.GetHzz(avg, space);

    //        //添加符号
    //        int hzPrecision = Common.GetPrecision(space.ToString());
    //        string AvgNumber = AddFlag(avg, VerifyConfig.PjzDigit).ToString();

    //        string HZNumber = hz.ToString(string.Format("F{0}", hzPrecision));
    //        if (hz != 0f) //化整值为0时，不加正负号
    //            HZNumber = AddFlag(hz, hzPrecision);

    //        if (avg < 0) HZNumber = HZNumber.Replace('+', '-'); //平均值<0时，化整值需为负数

    //        // 检测是否超过误差限
    //        if (avg >= wcPoint.ErrorXiaXian && avg <= wcPoint.ErrorShangXian)
    //            resoult.Resoult = Const.合格;
    //        else
    //            resoult.Resoult = Const.不合格;

    //        //记录误差
    //        string strWuCha = string.Empty;
    //        int wcCount = 0;
    //        for (int i = 0; i < data.Length; i++)
    //        {
    //            if (data[i] != Const.没有误差默认值)
    //            {
    //                wcCount++;
    //                strWuCha += string.Format("{0}|", AddFlag(data[i], VerifyConfig.PjzDigit));
    //            }
    //            else
    //            {
    //                strWuCha += " |";
    //            }
    //        }
    //        if (wcCount != data.Length)
    //        {
    //            resoult.Resoult = Const.不合格;
    //        }
    //        strWuCha += string.Format("{0}|", AvgNumber);
    //        strWuCha += string.Format("{0}", HZNumber);
    //        resoult.ErrorValue = strWuCha;
    //        return resoult;
    //    }

    //    /// <summary>
    //    /// 加+-符号
    //    /// </summary>
    //    /// <param name="data"></param>
    //    /// <returns></returns>
    //    private string AddFlag(string data)
    //    {
    //        if (float.Parse(data) > 0)
    //            return string.Format("+{0}", data);
    //        else
    //            return data;
    //    }

    //    /// <summary>
    //    /// 返回修正间距
    //    /// </summary>
    //    /// <IsWindage>是否是偏差</IsWindage> 
    //    /// <returns></returns>
    //    public float GetWuChaHzzJianJu(bool IsWindage, float meterLevel)
    //    {
    //        Dictionary<string, float[]> DicJianJu = null;
    //        string Key = string.Format("Level{0}", meterLevel);
    //        if (DicJianJu == null)
    //        {
    //            DicJianJu = new Dictionary<string, float[]>
    //            {
    //                { "Level0.02B", new float[] { 0.002F, 0.0002F } },      // 0.02级表标准表
    //                { "Level0.05B", new float[] { 0.005F, 0.0005F } },      // 0.05级表标准表
    //                { "Level0.1B", new float[] { 0.01F, 0.001F } },         // 0.1级表标准表
    //                { "Level0.2B", new float[] { 0.02F, 0.002F } },         // 0.2级标准表
    //                { "Level0.2", new float[] { 0.02F, 0.004F } },          // 0.2级普通表
    //                { "Level0.5", new float[] { 0.05F, 0.01F } },           // 0.5级表
    //                { "Level1", new float[] { 0.1F, 0.02F } },              // 1级表
    //                { "Level1.5", new float[] { 0.2F, 0.04F } },            // 2级表
    //                { "Level2", new float[] { 0.2F, 0.04F } }               // 2级表
    //            };
    //        }
    //        float[] JianJu;
    //        if (DicJianJu.ContainsKey(Key))
    //        {
    //            JianJu = DicJianJu[Key];
    //        }
    //        else
    //        {
    //            JianJu = new float[] { 2, 2 };    //没有在字典中找到，则直接按2算
    //        }

    //        if (IsWindage)
    //            return JianJu[1];//标偏差
    //        else
    //            return JianJu[0];//普通误差
    //    }

    //    /// <summary>
    //    /// 功率方向
    //    /// </summary>
    //    /// <param name="fx"></param>
    //    /// <returns></returns>
    //    private int GetFangXianIndex(PowerWay fx)
    //    {
    //        int readType = 0;
    //        switch (fx)
    //        {
    //            case PowerWay.正向有功:
    //                readType = 0;
    //                break;
    //            case PowerWay.正向无功:
    //                readType = 1;
    //                break;
    //            case PowerWay.反向有功:
    //                readType = 2;
    //                break;
    //            case PowerWay.反向无功:
    //                readType = 3;
    //                break;
    //            default:
    //                break;
    //        }
    //        return readType;
    //    }

    //    /// <summary>
    //    /// 修正数字加+-号
    //    /// </summary>
    //    /// <param name="data">要修正的数字</param>
    //    /// <param name="Priecision">修正精度</param>
    //    /// <returns>返回指定精度的带+-号的字符串</returns>
    //    private string AddFlag(float data, int Priecision)
    //    {
    //        string v = data.ToString(string.Format("F{0}", Priecision));
    //        return AddFlag(v);
    //    }

    //    /// <summary>
    //    /// 计算标准误差
    //    /// </summary>
    //    /// <param name="wcPoint"></param>
    //    /// <param name="meterLevel"></param>
    //    /// <param name="data"></param>
    //    /// <returns></returns>
    //    public ErrorResoult SetPcCha(StPlan_WcPoint wcPoint, float meterLevel, float[] data)
    //    {
    //        ErrorResoult resoult = new ErrorResoult();
    //        float space = GetWuChaHzzJianJu(true, meterLevel);

    //        float Windage = Number.GetWindage(data); //计算标准偏差
    //        Windage = (float)Math.Round(Windage, VerifyConfig.PjzDigit);
    //        float hz = Number.GetHzz(Windage, space);

    //        //添加符号
    //        int hzPrecision = Common.GetPrecision(space.ToString());
    //        string AvgNumber = AddFlag(Windage, 4).ToString().Replace("+", ""); ;

    //        string HZNumber = hz.ToString(string.Format("F{0}", hzPrecision));
    //        HZNumber = AddFlag(hz, hzPrecision).Replace("+", ""); ;


    //        // 检测是否超过误差限
    //        if (Windage >= wcPoint.ErrorXiaXian && Windage <= wcPoint.ErrorShangXian)
    //            resoult.Resoult = Const.合格;
    //        else
    //            resoult.Resoult = Const.不合格;

    //        //记录误差
    //        string strWuCha = string.Empty;
    //        int wcCount = 0;
    //        for (int i = 0; i < data.Length; i++)
    //        {
    //            if (data[i] != Const.没有误差默认值)
    //            {
    //                wcCount++;
    //                strWuCha += string.Format("{0}|", AddFlag(data[i], VerifyConfig.PjzDigit));
    //            }
    //            else
    //            {
    //                strWuCha += "|";
    //            }
    //        }
    //        if (wcCount != data.Length)
    //        {
    //            resoult.Resoult = Const.不合格;
    //        }
    //        strWuCha += string.Format("{0}|", AvgNumber);
    //        strWuCha += string.Format("{0}", HZNumber);
    //        resoult.ErrorValue = strWuCha;

    //        return resoult;
    //    }

    //    /// <summary>
    //    /// 检定参数是否合法
    //    /// </summary>
    //    /// <returns></returns>
    //    protected override bool CheckPara()
    //    {
    //        ResultNames = new string[] { "当前项目", "功率元件", "功率方向", "电流倍数", "误差上限", "误差下限", "误差圈数", "误差1", "误差2", "平均值", "化整值", "影响后误差1", "影响后误差2", "影响后平均值", "影响后化整值", "改变量", "结论" };
    //        resultKey = new bool[MeterNumber];
    //        string[] str = Test_Value.Split('|');
    //        if (str.Length > 8)
    //        {
    //            CurPlana.PowerDianLiu = str[8];
    //            CurPlana.PowerYinSu = str[1];
    //            CurPlana.PowerFangXiang = (PowerWay)Enum.Parse(typeof(PowerWay), str[4]);
    //            CurPlana.LapCount = int.Parse(str[11]);
    //            CurPlana.PrjName = str[0];
    //            CurPlana.XieBoFa = str[2];
    //            CurPlana.PL = float.Parse(str[9]);
    //            #region ---功率方向---
    //            switch (str[3])
    //            {
    //                case "H":
    //                    CurPlana.PowerYuanJian = Cus_PowerYuanJian.H;
    //                    break;
    //                case "A":
    //                    CurPlana.PowerYuanJian = Cus_PowerYuanJian.A;
    //                    break;
    //                case "B":
    //                    CurPlana.PowerYuanJian = Cus_PowerYuanJian.B;
    //                    break;
    //                case "C":
    //                    CurPlana.PowerYuanJian = Cus_PowerYuanJian.C;
    //                    break;
    //                default:
    //                    CurPlana.PowerYuanJian = Cus_PowerYuanJian.H;
    //                    break;
    //            }
    //            #endregion
    //        }
    //        resultKey.Fill(true);
    //        return true;
    //    }
    //    #endregion
    //}
}
