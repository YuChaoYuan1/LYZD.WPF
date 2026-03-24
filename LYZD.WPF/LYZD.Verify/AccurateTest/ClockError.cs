using LYZD.Core.Function;
using LYZD.Core.Helper;
using LYZD.Core.Model.Meter;
using LYZD.Core.Struct;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using LYZD.ViewModel.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LYZD.Verify.AccurateTest
{
    /// <summary>
    /// 日记时误差
    /// </summary>
    public class ClockError : VerifyBase
    {

        #region 变量
        int WcCount = 5;   //误差次数
        int WcQs = 60;      //误差圈数
        float wcLimit = 1.0f;//误差限控制比例
        int maxErrorTimes = 5;               //最大误差次数
        #endregion


        /// <summary>
        /// 日记时误差
        /// </summary>
        public override void Verify()
        {
            base.Verify();
            MessageAdd("日记时误差试验检定开始...", EnumLogType.提示信息);

            if (IsDemo)
            {
                for (int j = 0; j < MeterNumber; j++)
                {
                    if (!meterInfo[j].YaoJianYn) continue;
                    for (int i = 0; i < 5; i++)
                    {
                        ResultDictionary[string.Format("误差{0}", i + 1)][j] = (0.00111).ToString("f5");
                        RefUIData("误差" + (i + 1).ToString());
                    }
                    ResultDictionary["结论"][j] = "合格";
                    ResultDictionary["平均值"][j] = 0.001.ToString("0.000");
                    ResultDictionary["化整值"][j] = 0.0.ToString("0.0");

                }
                RefUIData("平均值");
                RefUIData("化整值");
                RefUIData("结论");
            }
            else
            {
                if (!InitEquipment())   //设备控制
                {
                    MessageAdd("【日计时试验】初始化基本误差设备参数失败", EnumLogType.错误信息);
                }
                if (Stop) return;

                List<string>[] errData = new List<string>[MeterNumber]; //误差数据
                for (int i = 0; i < MeterNumber; i++)
                {
                    errData[i] = new List<string>();
                }
                int[] lastNums = new int[MeterNumber];
                lastNums.Fill(-1);

                MessageAdd("正在启动误差板", EnumLogType.提示信息);
                StartWcb(04, 0xff);   //启动误差板
                Thread.Sleep(200);

                MessageAdd("开始检定", EnumLogType.提示信息);

                bool[] arrCheckOver = new bool[MeterNumber];            //表位完成记录
                arrCheckOver.Fill(false);
                DateTime TmpTime1 = DateTime.Now;//检定开始时间，用于判断是否超时

                while (true)
                {
                    if (Stop)
                    {
                        MessageAdd("停止检定", EnumLogType.提示信息);
                        break;
                    }
                    if (TimeSub(DateTime.Now, TmpTime1) > (VerifyConfig.MaxHandleTime + 100) * 1000 && !IsMeterDebug) //超出最大处理时间
                    {
                        MessageAdd("超出最大处理时间,正在退出...", EnumLogType.错误信息);
                        break;
                    }

                    string[] curWC = new string[MeterNumber];   //重新初始化本次误差
                    int[] curNum = new int[MeterNumber];        //当前读取的误差序号
                    curWC.Fill("");
                    curNum.Fill(0);
                    if (!ReadWc(ref curWC, ref curNum, 4))    //读取误差
                    {
                        continue;
                    }
                    if (Stop) break;

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (Stop) break;
                        TestMeterInfo meter = meterInfo[i];      //表基本信息
                        if (!meterInfo[i].YaoJianYn) arrCheckOver[i] = true;     //表位不要检
                        if (arrCheckOver[i]) continue;   //表位检定通过了
                        if (string.IsNullOrEmpty(curWC[i])) continue;
                        if (curNum[i] <= lastNums[i]) continue;
                        if (curNum[i] < 1) continue;
                        errData[i].Insert(0, curWC[i]);
                        lastNums[i] = curNum[i];

                        ErrorLimit limit = new ErrorLimit
                        {
                            UpLimit = wcLimit * 0.5F,
                            DownLimit = -wcLimit * 0.5F,
                        };
                        string[] dj = Number.GetDj(meter.MD_Grane);
                        MessageAdd($"获取等级：【有功等级{dj[0]}】【无功等级{dj[1]}】", EnumLogType.详细信息);
                        float[] wc = ArrayConvert.ToSingle(errData[i].ToArray());  //Datable行到数组的转换
                        string Result = SetWuCha(wc, limit);
                        ResultDictionary["结论"][i] = Result;


                        #region 获取详细数据
                        float fSum = 0.0f;
                        string[] strWc = new string[WcCount];
                        for (int j = 0; j < wc.Length; j++)
                        {
                            strWc[j] = wc[j].ToString("F5");
                            fSum = fSum + Convert.ToSingle(strWc[j]);
                            ResultDictionary[string.Format("误差{0}", j + 1)][i] = wc[j].ToString("F5");
                        }
                        fSum = fSum / wc.Length;
                        if (wc.Length == maxErrorTimes)
                        {
                            ResultDictionary["平均值"][i] = fSum.ToString("0.000");
                            ResultDictionary["化整值"][i] = fSum.ToString("0.0");
                            arrCheckOver[i] = true;
                        }
                        #endregion
                    }

                    #region 上传结论

                    for (int i = 0; i < WcCount; i++)
                    {
                        RefUIData("误差" + (i + 1).ToString());
                    }
                    #endregion
                    if (Array.IndexOf(arrCheckOver, false) < 0 && !IsMeterDebug)  //全部都为true了
                        break;
                    Thread.Sleep(100);

                }

                RefUIData("平均值");
                RefUIData("化整值");

                StopWcb(04, 0xff);//关闭误差板
                RefUIData("结论");
            }

            MessageAdd("检定完成", EnumLogType.提示信息);
        }


        protected override bool CheckPara()
        {
            string[] str = Test_Value.Split('|');
            if (str.Length < 3) return false;
            float Count;
            int count2;
            if (float.TryParse(str[0], out Count))
            {
                wcLimit = Count;
            }
            if (int.TryParse(str[1], out count2))
            {
                WcCount = count2;
            }
            if (int.TryParse(str[2], out count2))
            {
                WcQs = count2;
            }
            MessageAdd($"日记时实验初始化参数：【误差线比例{wcLimit}】【误差次数{WcCount}】【圈数{WcQs}】", EnumLogType.详细信息);
            ResultNames = new string[] { "误差1", "误差2", "误差3", "误差4", "误差5", "平均值", "化整值", "结论" };
            return true;
        }

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <returns></returns>
        private bool InitEquipment()
        {
            MessageAdd("日记时误差实验开始初始化设备", EnumLogType.详细信息);
            if (IsDemo) return true;
            try
            {
                MessageAdd("正在升源...", EnumLogType.提示信息);
                if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, 0, 0, 0, Core.Enum.Cus_PowerYuanJian.H, FangXiang, "1.0"))
                {
                    MessageAdd("升源失败,退出检定", EnumLogType.错误信息);
                    return false;
                }
                if (Stop) return true;
                //设置误差版被检常数
                MessageAdd("正在设置误差版标准常数...", EnumLogType.提示信息);
                SetStandardConst(1, 500000, 0);
                //设置误差版标准常数
                MessageAdd("正在设置误差版被检常数...", EnumLogType.提示信息);
                int[] meterconst = MeterHelper.Instance.MeterConst(true);
                meterconst.Fill(1);
                int[] pulselap = new int[MeterNumber];
                pulselap.Fill(WcQs);
                if (!SetTestedConst(04, meterconst, 0, pulselap))
                {
                    MessageAdd("初始化误差检定参数失败", EnumLogType.错误信息);
                    return false;
                }
                MessageAdd("日记时误差实验初始化设备完成", EnumLogType.详细信息);
                return true;
            }
            catch (Exception ex)
            {
                MessageAdd("日记时误差实验初始化设备异常" + ex.ToString(), EnumLogType.错误信息);
                return false;
            }

        }


        public string SetWuCha(float[] data, ErrorLimit limit)
        {
            float avgWC = Number.GetAvgA(data);
            ////取平均值
            avgWC = (float)Math.Round(avgWC, VerifyConfig.PjzDigit);

            ////由电源供电的时钟试验化整间距为0.01
            ////参照JJG-596-1999 5.1.1
            avgWC = Number.GetHzz(avgWC, 0.01F);

            if (avgWC >= limit.DownLimit && avgWC <= limit.UpLimit)
            {
                return Core.Helper.Const.合格;
            }
            return Core.Helper.Const.不合格;
        }
    }
}
