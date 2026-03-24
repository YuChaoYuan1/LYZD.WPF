using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.DataCollect
{

    /// <summary>
    /// 电能表数据采集
    /// </summary>
     public class EnergyCollection_376 : VerifyBase
    {
        protected int SubItemIndex = 1;
        protected string[] str_BaseMessage = null;
        protected string[] str_CorrectValue = null;
        protected int[] int_DataIndex = null;
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "三相电压", "电能表日历时钟", "时段表编程总次数", "校时次数", "电表运行状态字", "正向有功电能示值", "反向有功电能示值", "正向有功最大需量", "反向有功最大需量", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();


                StringBuilder sb = new StringBuilder();

                #region 模拟表操作
                // 设置模拟表电流
                ControlVirtualMeter("SCu000000000000000");
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
                    sb.Append("00010");

                }
                ControlVirtualMeter(sb.ToString());
                ControlVirtualMeter("QLT");
                ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,45,45,45,1,0");
                ControlVirtualMeter("Cmd,SZT,0100,0200,0300,0400,0500,0600,0700");
                ControlVirtualMeter("Cmd,MeterTimeCheck,1");
                #endregion


                ResetTerimal(2);

                for (   int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                     {
                        TempData[i].StdData = "Fn1";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                            {
                                //MessageAdd("终端" + (i + 1) + "设置测量点限值参数无回复！",EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置测量点限值参数回复否认";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "设置测量点限值参数无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置测量点限值参数无回复";
                        }

                    }
                }
                 //RefUIData("F2.数据区初始化");
                //AddItemsResoult("数据区初始化", TempData);
                SetTime(Convert.ToDateTime("2017-1-1 0:14:0"), 0);


                WaitTime("等待终端抄表", 180);

                // F25.读取电表有/无功功率、功率因数、三相电压、电流、零序电流、视在功率
                str_CorrectValue = new string[] { "220", "220", "220" };
                int_DataIndex = new int[] { 16, 17, 18 };
                BaseVerifyUnit("三相电压", str_CorrectValue, int_DataIndex, 25, 2);
                //TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 1, 25, RecData, MaxWaitSeconds_Read485);

                str_CorrectValue = new string[] { "2016-1-11 12:0:0" };
                int_DataIndex = new int[] { 4 };
                BaseVerifyUnit("电能表日历时钟", str_CorrectValue, int_DataIndex, 27, 3);

                str_CorrectValue = new string[] { "8" };
                int_DataIndex = new int[] { 6 };
                BaseVerifyUnit("时段表编程总次数", str_CorrectValue, int_DataIndex, 27, 4);

                str_CorrectValue = new string[] { "8" };
                int_DataIndex = new int[] { 14 };
                BaseVerifyUnit("校时次数", str_CorrectValue, int_DataIndex, 27, 5);

                str_CorrectValue = new string[] { "0001", "0002", "0003", "0004", "0005", "0006", "0007" };
                int_DataIndex = new int[] { 11, 12, 13, 14, 15, 16, 17 };
                BaseVerifyUnit("电表运行状态字", str_CorrectValue, int_DataIndex, 28, 6);

                str_CorrectValue = new string[] { "10", "2.5", "2.5", "2.5", "2.5", };
                int_DataIndex = new int[] { 5, 6, 7, 8, 9 };
                BaseVerifyUnit("正向有功电能示值", str_CorrectValue, int_DataIndex, 33, 7);

                str_CorrectValue = new string[] { "10", "2.5", "2.5", "2.5", "2.5", };
                int_DataIndex = new int[] { 5, 6, 7, 8, 9 };
                BaseVerifyUnit("反向有功电能示值", str_CorrectValue, int_DataIndex, 34, 8);

                str_CorrectValue = new string[] { "2.3335" };
                int_DataIndex = new int[] { 5 };
                BaseVerifyUnit("正向有功最大需量", str_CorrectValue, int_DataIndex, 35, 9);

                str_CorrectValue = new string[] { "0" };
                int_DataIndex = new int[] { 5 };
                BaseVerifyUnit("反向有功最大需量", str_CorrectValue, int_DataIndex, 36, 10);


                #region old
                //// F26.A、B、C三相断相统计数据及最近一次断相记录
                //str_BaseMessage = new string[] { "A相断相次数", "B相断相次数", "C相断相次数" };
                //str_CorrectValue = new string[3] { "6", "1", "1" };
                //int_DataIndex = new int[] { 5, 6, 7 };
                //BaseVerifyUnit("F26.A、B、C三相断相统计数据及最近一次断相记录", str_BaseMessage, str_CorrectValue, int_DataIndex, 26);

                //// F27.电能表日历时钟、编程次数及其最近一次操作时间
                //str_BaseMessage = new string[] { "电能表日历时钟" };
                //str_CorrectValue = new string[1];
                //int_DataIndex = new int[] { 4 };
                //BaseVerifyUnit("F27.电能表日历时钟、编程次数及其最近一次操作时间", str_BaseMessage, str_CorrectValue, int_DataIndex, 27);

                //// F28.电表运行状态字及其变位标志
                //str_BaseMessage = new string[] { "电能表运行状态字1", "电能表运行状态字2", "电能表运行状态字3", "电能表运行状态字4", "电能表运行状态字5", "电能表运行状态字6", "电能表运行状态字7" };
                //str_CorrectValue = new string[7] { "0001", "0002", "0003", "0004", "0005", "0006", "0007" };
                //int_DataIndex = new int[] { 11, 12, 13, 14, 15, 16, 17 };
                //BaseVerifyUnit("F28.电表运行状态字及其变位标志", str_BaseMessage, str_CorrectValue, int_DataIndex, 28);

                //// F33.当前正向有/无功电能示值、一/四象限无功电能示值
                //str_BaseMessage = new string[] { "正向有功总电能值", "费率1正向有功电能示值", "费率2正向有功电能示值", "费率3正向有功电能示值", "费率4正向有功电能示值", "正向无功总电能示值", "当前一象限无功电能示值", "当前三象限无功电能示值" };
                //str_CorrectValue = new string[8] { "10", "2.5", "2.5", "2.5", "2.5", "20", "10", "10" };
                //int_DataIndex = new int[] { 5, 6, 7, 8, 9, 10, 15, 20 };
                //BaseVerifyUnit("F33.当前正向有/无功电能示值、一/四象限无功电能示值", str_BaseMessage, str_CorrectValue, int_DataIndex, 33);

                //// F34.当前反向有/无功电能示值、二/三象限无功电能示值
                //str_BaseMessage = new string[] { "反向有功总电能值", "费率1反向有功电能示值", "费率2反向有功电能示值", "费率3反向有功电能示值", "费率4反向有功电能示值", "反向无功总电能示值", "当前二象限无功电能示值", "当前四象限无功电能示值" };
                //str_CorrectValue = new string[8] { "10", "2.5", "2.5", "2.5", "2.5", "20", "10", "10" }; ;
                //int_DataIndex = new int[] { 5, 6, 7, 8, 9, 10, 15, 20 };
                //BaseVerifyUnit("F34.当前反向有/无功电能示值、二/三象限无功电能示值", str_BaseMessage, str_CorrectValue, int_DataIndex, 34);

                //// F35.当月正向有/无功最大需量及发生时间
                //str_BaseMessage = new string[] { "正向有功总最大需量", "正向无功总最大需量" };
                //str_CorrectValue = new string[2] { "2.3335", "2.3335" };
                //int_DataIndex = new int[] { 5, 15 };
                //BaseVerifyUnit("F35.当月正向有/无功最大需量及发生时间", str_BaseMessage, str_CorrectValue, int_DataIndex, 35);

                //// F36.当月反向有/无功最大需量及发生时间
                //str_BaseMessage = new string[] { "反向有功总最大需量", "反向无功总最大需量" };
                //str_CorrectValue = new string[2] { "0", "0" };
                //int_DataIndex = new int[] { 5, 15 };
                //BaseVerifyUnit("F36.当月反向有/无功最大需量及发生时间", str_BaseMessage, str_CorrectValue, int_DataIndex, 36);

                //// F166.电能表参数修改次数及时间
                //str_BaseMessage = new string[] { "校时总次数", "时段表编程总次数" };
                //str_CorrectValue = new string[2] { "8", "8" };
                //int_DataIndex = new int[] { 4, 7 };
                //BaseVerifyUnit("F166.电能表参数修改次数及时间", str_BaseMessage, str_CorrectValue, int_DataIndex, 166);

                //// F177.当前组合有功电能示值
                //str_BaseMessage = new string[] { "(当前)组合有功总电能", "(当前)组合有功费率1电能", "(当前)组合有功费率2电能", "(当前)组合有功费率3电能", "(当前)组合有功费率4电能" };
                //str_CorrectValue = new string[5] { "20", "5", "5", "5", "5" };
                //int_DataIndex = new int[] { 5, 6, 7, 8, 9 };
                //BaseVerifyUnit("F177.当前组合有功电能示值", str_BaseMessage, str_CorrectValue, int_DataIndex, 177);
                #endregion

                ControlVirtualMeter("Cmd,MeterTimeCheck,0");
                SetTime(DateTime.Now, 0);
                
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
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
        private void BaseVerifyUnit(string p_str_Message, string[] p_str_CorrectValue, int[] p_int_DataIndex, byte p_byt_Fn, int int_index)
        {
            MessageAdd(p_str_Message,EnumLogType.提示与流程信息);
            TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 2, p_byt_Fn, RecData, MaxWaitSeconds_Read485);

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
                            s1[tableNo] += "," + GetData(RecData, tableNo, p_int_DataIndex[index], EnumTerimalDataType.e_string);
                            //s1[tableNo] += "," + p_str_CorrectValue[index];
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
                        MessageAdd("来源：" + (tableNo + 1) + "号表 / 错误：" + ex.ToString(),EnumLogType.错误信息);
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
                    TempData[tableNo].Resoult = "不合格";
                }
            }
            AddItemsResoult(p_str_Message, TempData);
        }

    }
}
