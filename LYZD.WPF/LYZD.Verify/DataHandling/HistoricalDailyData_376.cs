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
    /// 历史日数据
    /// </summary>
   public class HistoricalDailyData_376 : VerifyBase
    {



        public override void Verify()
        {
            base.Verify();
            StartVerify();

            StringBuilder sb = new StringBuilder();
            #region 模拟表操作
            // 设置模拟表停走
            // 设置模拟表电压电流
            sb.Clear();
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

            ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
            SetTime(Convert.ToDateTime("2015-8-3 23:55:00"), 0);
            WaitTime("等待生成数据", 60 * 15);
            SetData = Core.Function.UsefulMethods.ConvertStringToBytes("030815");
            BaseVerifyUnit("正向有功电能示值", SetData, new string[] { "100", "25", "25", "25", "25" }, new int[] { 6, 7, 8, 9, 10 }, 1, 1);
            BaseVerifyUnit("反向有功电能示值", SetData, new string[] { "100", "25", "25", "25", "25" }, new int[] { 6, 7, 8, 9, 10 }, 2, 2);
            BaseVerifyUnit("正向有功最大需量", SetData, new string[] { "3.3" }, new int[] { 6 }, 3, 3);
            BaseVerifyUnit("反向有功最大需量", SetData, new string[] { "0" }, new int[] { 6 }, 4, 4);
            BaseVerifyUnit("抄表日正向有功电能示值", SetData, new string[] { "100", "25", "25", "25", "25" }, new int[] { 6, 7, 8, 9, 10 }, 9, 5);
            BaseVerifyUnit("抄表日反向有功电能示值", SetData, new string[] { "100", "25", "25", "25", "25" }, new int[] { 6, 7, 8, 9, 10 }, 10, 6);
            BaseVerifyUnit("抄表日正向有功最大需量", SetData, new string[] { "3.3" }, new int[] { 6 }, 11, 7);
            BaseVerifyUnit("抄表日反向有功最大需量", SetData, new string[] { "0" }, new int[] { 6 }, 12, 8);
            BaseVerifyUnit2("终端日供电时间和复位累计次数", SetData, new int[] { 4 }, 0, 49, 9);


        }

        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //三相电压|电能表日历时钟|时段表编程总次数|校时次数|电表运行状态字|正向有功电能示值|反向有功电能示值|正向有功最大需量|反向有功最大需量
            ResultNames = new string[] { "正向有功电能示值", "反向有功电能示值", "正向有功最大需量", "反向有功最大需量", "抄表日正向有功电能示值", "抄表日反向有功电能示值", "抄表日正向有功最大需量", "抄表日反向有功最大需量", "终端日供电时间和复位累计次数", "结论" };
            return true;
        }
        /// <summary>
        /// 检定单元
        /// </summary>
        /// <param name="p_str_Message"></param>
        /// <param name="p_str_BaseMessage"></param>
        /// <param name="p_str_CorrectValue"></param>
        /// <param name="p_int_DataIndex"></param>
        /// <param name="p_byt_SetData"></param>
        /// <param name="p_byt_Fn"></param>
        private void BaseVerifyUnit(string p_str_Message, byte[] SetData, string[] p_str_CorrectValue, int[] p_int_DataIndex, byte p_byt_Fn, int int_index)
        {
            MessageAdd("正在进行："+p_str_Message,EnumLogType.提示信息);
            TalkResult = TerminalProtocalAdapter.Instance.ReadData(13, 2, p_byt_Fn, SetData, RecData, MaxWaitSeconds_Write);

            string[] s1 = new string[MeterNumber];
            string[] s2 = new string[MeterNumber];

            for (int tableNo = 0; tableNo < MeterNumber; tableNo++)
            {
                for (int index = 0; index < p_str_CorrectValue.Length; index++)
                {
                    try
                    {
                        if (meterInfo[tableNo].YaoJianYn)
                        {
                            s1[tableNo] += "," + GetData(RecData, tableNo, p_int_DataIndex[index],EnumTerimalDataType.e_string);
                            s2[tableNo] += "," + p_str_CorrectValue[index];
                        }
                        else
                        {
                            s1[tableNo] = "";
                            s2[tableNo] = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        s1[tableNo] = "";
                        s2[tableNo] = "";
                        MessageAdd((tableNo + 1) + "号表/错误：" + ex.Message, EnumLogType.错误信息);
                        continue;
                    }
                }
                if (s1[tableNo].Length > 1)
                    s1[tableNo] = s1[tableNo].Remove(0, 1);
                if (s2[tableNo].Length > 1)
                    s2[tableNo] = s2[tableNo].Remove(0, 1);
                TempData[tableNo].Data = s1[tableNo];
                TempData[tableNo].StdData = s2[tableNo];
                if (s1[tableNo] != s2[tableNo])
                {
                  Resoult[tableNo] = Core.Helper.Const.不合格;
                }
            }
            AddItemsResoult(p_str_Message, TempData);

        }
        private void BaseVerifyUnit2(string p_str_Message, byte[] SetData, int[] p_int_DataIndex, byte p_byt_Pn, byte p_byt_Fn, int int_index)
        {
            MessageAdd("正在进行："+ p_str_Message,EnumLogType.提示信息);
           TalkResult = TerminalProtocalAdapter.Instance.ReadData(13, p_byt_Pn, p_byt_Fn, SetData, RecData, MaxWaitSeconds_Write);

            string[] s1 = new string[MeterNumber];
            string[] s2 = new string[MeterNumber];

            for (int tableNo = 0; tableNo < MeterNumber; tableNo++)
            {
                for (int index = 0; index < p_int_DataIndex.Length; index++)
                {
                    try
                    {
                        if (meterInfo[tableNo].YaoJianYn)
                        {
                            s1[tableNo] += "," + GetData(RecData, tableNo, p_int_DataIndex[index],EnumTerimalDataType.e_string);
                        }
                        else
                        {
                            s1[tableNo] = "";
                            s2[tableNo] = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        s1[tableNo] = "";
                        s2[tableNo] = "";
                        MessageAdd((tableNo + 1) + "号表/错误：" + ex.Message, EnumLogType.错误信息);
                        continue;
                    }
                }
                if (s1[tableNo].Length > 1)
                {

                    s1[tableNo] = s1[tableNo].Remove(0, 1);
                    TempData[tableNo].Data = s1[tableNo];
                }

                if (s1[tableNo].Length < 1)
                {
                    TempData[tableNo].Tips = "数据返回异常";
                    Resoult[tableNo] = Core.Helper.Const.不合格;
                }
            }
            AddItemsResoult(p_str_Message, TempData);

        }

    }
}
