using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.DataHandling
{
    /// <summary>
    /// 历史月数据呢
    /// </summary>
    public class HistoricalMonthlyData_376 : VerifyBase
    {
        public override void Verify()
        {
            base.Verify();
            StartVerify();
            StringBuilder sb = new StringBuilder();
            #region 模拟表操作
            // 设置模拟表停走
            // 设置模拟表电压电流
            ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
            // 设置模拟表断相信息
            sb.Clear();
            sb.Append("SQT");
            sb.Append("2015-10-12 15:04:00,");
            sb.Append("2015-10-12 15:04:00,");
            for (int i = 0; i < 6; i++)
            {
                sb.Append("0008,");
            }
            ControlVirtualMeter(sb.ToString());
            // 设置模拟表状态信息
            ControlVirtualMeter("SZT0000000000000000000000000000");
            // 设置模拟表电量
            sb.Clear();
            sb.Append("DLS");
            for (int i = 0; i < 8; i++)
            {
                sb.Append("00100");
            }
            ControlVirtualMeter(sb.ToString());
            #endregion
            ResetTerimal(2);


            //SetTime(Convert.ToDateTime("2016-6-30 23:59:30"), 0);
            //TimeSpan s = DateTime.Now - Convert.ToDateTime("2016-7-1 00:00:10");

            //// 设置模拟表时间差
            //string str_TimeDiff = (DateTime.Now.TimeOfDay - Convert.ToDateTime("2016-7-1 00:00:10").TimeOfDay).TotalMinutes.ToString();
            //string str_TimeDiff2 = (s.Days).ToString();
            //ControlVirtualMeter("Dat" + str_TimeDiff2);
            //ControlVirtualMeter("Tim" + str_TimeDiff);
            //WaitTime("等待生成数据", 240);




            SetTime(Convert.ToDateTime("2015-6-30 23:57:00"), 0);

            WaitTime("等待生成数据", 360);
            ControlVirtualMeter("Cmd,Set,264,176,260,6,4,5.9,0,0,0,1,0");
            WaitTime("等待生成数据", 600);
            //ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
            //WaitTime("等待生成数据", 180);
            SetTime(Convert.ToDateTime("2015-7-31 23:57:0"), 0);
            WaitTime("等待生成数据", 600);

            SetData = Core.Function.UsefulMethods.ConvertStringToBytes("0715");
            #region 电能示值
            // 读正向有功电能示值
            BaseVerifyUnit("读取正向有功电能示值", "100", 6, SetData, 17);

            // 读反向有功电能示值
            BaseVerifyUnit("读取反向有功电能示值", "100", 6, SetData, 18);
            #endregion

            // 读正向有功最大需量
            BaseVerifyUnit("读取正向有功最大需量", "3.822", 6, SetData, 193);

        }

        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //三相电压|电能表日历时钟|时段表编程总次数|校时次数|电表运行状态字|正向有功电能示值|反向有功电能示值|正向有功最大需量|反向有功最大需量
            ResultNames = new string[] { "读取正向有功电能示值", "读取反向有功电能示值", "读取正向有功最大需量", "结论" };
            return true;
        }

        private void BaseVerifyUnit(string p_str_BaseMessage, string p_str_CorrectValue, int p_int_DataIndex, byte[] p_byt_SetData, byte p_byt_Fn)
        {
            MessageAdd("正在进行：" + p_str_BaseMessage,EnumLogType.提示信息);
            TalkResult = TerminalProtocalAdapter.Instance.ReadData(13, 2, p_byt_Fn, p_byt_SetData, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = p_str_CorrectValue;
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data = GetData(RecData, i, p_int_DataIndex, EnumTerimalDataType.e_string);
                        // 判断数据是否正确
                        if (GetData(RecData, i, p_int_DataIndex, EnumTerimalDataType.e_string) != p_str_CorrectValue)
                        {
                            TempData[i].Resoult = Core.Helper.Const.不合格;
                            TempData[i].Tips = p_str_BaseMessage + "不正确";
                        }
                    }
                    else
                    {
                        TempData[i].Resoult = Core.Helper.Const.不合格;
                        TempData[i].Tips = "无回复";
                    }
                }
            }
            AddItemsResoult(p_str_BaseMessage, TempData);

        }

    }
}
