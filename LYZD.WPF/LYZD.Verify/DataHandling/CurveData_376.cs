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
    /// 负荷曲线
    /// </summary>
    public class CurveData_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "数据区初始化", "F81.测量点有功功率曲线", "F82.测量点A相有功功率曲线", "F83.测量点B相有功功率曲线", "F84.测量点C相有功功率曲线", "F85.测量点无功功率曲线", "F86.测量点A相无功功率曲线", "F87.测量点B相无功功率曲线", "F88.测量点C相无功功率曲线", "F89.A相电压曲线", "F90.A相电压曲线", "F91.A相电压曲线", "F92.A相电流曲线", "F93.A相电流曲线", "F94.A相电流曲线", "F97.正向有功总电能量曲线", "F98.正向无功总电能量曲线", "F99.反向有功总电能量曲线", "F100.反向无功总电能量曲线", "F105.测量点功率因数曲线", "F106.测量点A相功率因数曲线", "F107.测量点B相功率因数曲线", "F108.测量点C相功率因数曲线", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();

                ResetTerimal(2);

                int intIndex = 1;

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "Fn1";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                            {
                                MessageAdd("终端" + (i + 1) + "设置测量点限值参数无回复！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置测量点限值参数回复否认";
                            }
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "设置测量点限值参数无回复！", EnumLogType.错误信息);

                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置测量点限值参数无回复";
                        }

                    }
                }
                //RefUIData("F2.数据区初始化");
                AddItemsResoult("数据区初始化", TempData);
                #region 下发试验基本参数
                // 设置终端参数 终端电能表/交流采样装置配置参数
                MessageAdd("下发终端参数：终端电能表/交流采样装置配置参数。",EnumLogType.流程信息);

                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("020001000100010200000000000000000000000004090000000000000002000200621E010000000000000000000000040900000000000000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 10, SetData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "Fn1";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                            {
                                MessageAdd("终端" + (i + 1) + "设置终端电能表/交流采样装置配置参数失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置终端电能表/交流采样装置配置参数失败";
                            }
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "设置终端电能表/交流采样装置配置参数无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置终端电能表/交流采样装置配置参数无回复";
                        }

                    }
                }

                // 设置测量点基本参数
                MessageAdd("下发终端参数：测量点基本参数。",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(4, 1, 25, Xub.ToString() + "," + xIb.ToString() + "," + "9.9", RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "Fn1";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                            {
                                MessageAdd("终端" + (i + 1) + "设置测量点基本参数失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置测量点基本参数失败";
                            }
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "设置测量点基本参数无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置测量点基本参数无回复";
                        }
                    }
                }

                // 终端抄表运行参数设置
                MessageAdd("下发终端参数：终端抄表运行参数设置。",EnumLogType.流程信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("02010000FFFFFF7F57230200000103000000080008001600165923020000FFFFFF7F57230200000103000000080008001600165923");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 33, SetData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "Fn1";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            if (GetData(RecData, i, 2, EnumTerimalDataType.e_string) != "Fn1")
                            {
                                MessageAdd("终端" + (i + 1) + "设置终端抄表运行参数设置失败！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "设置终端抄表运行参数设置失败";
                            }
                        }
                        else
                        {
                            MessageAdd("终端" + (i + 1) + "设置终端抄表运行参数设置无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "设置终端抄表运行参数设置无回复";
                        }

                    }
                }
                #endregion

                StringBuilder sb = new StringBuilder();
                #region 模拟表操作
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

                // 使终端产生曲线数据
                DateTime dt_Temp = DateTime.Now;
                //for (int i = 0; i < 2; i++)
                //{
                // 设置模拟表时间差
                DateTime dt_SetTime = dt_Temp.Date.AddDays(0).AddMinutes(599);
                string str_TimeDiff = (DateTime.Now.TimeOfDay - dt_SetTime.TimeOfDay).TotalMinutes.ToString();
                ControlVirtualMeter("Tim" + str_TimeDiff);

                // 终端对时
                MessageAdd("终端对时",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 31, dt_SetTime.ToString(), RecData, MaxWaitSeconds_Write);

                // 等待2分钟
                WaitTime("等待终端抄表", 1500);
                //}

                //#region 请求终端数据
                // 数据单元
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes(dt_Temp.Date.AddDays(0).AddMinutes(615).ToString("mmhhddMMyy") + "0102");


                //if (CLOU_Comm.GlobalUnit.g_TerminalType == EnumTerminalType.专变III)
                //{

                //    BaseVerifyUnit("F73.总加组有功功率曲线", SetData, new string[] { "3300" }, new int[] { 6 }, 73, intIndex++);

                //    BaseVerifyUnit("F74.总加组无功功率曲线", SetData, new string[] { "0" }, new int[] { 6 }, 74, intIndex++);

                //    BaseVerifyUnit("F75.总加组有功电能量曲线", SetData, new string[] { "0" }, new int[] { 6 }, 75, intIndex++);

                //    BaseVerifyUnit("F76.总加组无功电能量曲线", SetData, new string[] { "0" }, new int[] { 6 }, 76, intIndex++);
                //}


                //#region 总加组有功/无功功率曲线、电能量曲线
                //// 读总加组有功功率曲线
                //BaseVerifyUnit("读取总加组有功功率曲线", "3300", 6, SetData, 73);

                //// 读总加组无功功率曲线
                //BaseVerifyUnit("读取总加组无功功率曲线", "0", 6, SetData, 74);

                //// 读总加组有功电能量曲线
                //BaseVerifyUnit("读取总加组有功电能量曲线", "0", 6, SetData, 75);

                //// 读总加组无功电能量曲线
                //BaseVerifyUnit("读取总加组无功电能量曲线", "0", 6, SetData, 76);
                //#endregion


                BaseVerifyUnit("F81.测量点有功功率曲线", SetData, new string[] { "3.3" }, new int[] { 6 }, 81, intIndex++);

                BaseVerifyUnit("F82.测量点A相有功功率曲线", SetData, new string[] { "1.1" }, new int[] { 6 }, 82, intIndex++);

                BaseVerifyUnit("F83.测量点B相有功功率曲线", SetData, new string[] { "1.1" }, new int[] { 6 }, 83, intIndex++);

                BaseVerifyUnit("F84.测量点C相有功功率曲线", SetData, new string[] { "1.1" }, new int[] { 6 }, 84, intIndex++);

                //#region 测量点有功功率曲线
                //// 读测量点有功功率曲线
                //BaseVerifyUnit("读取测量点有功功率曲线", "3.3", 6, SetData, 81);

                //// 读测量点A相有功功率曲线
                //BaseVerifyUnit("读取测量点A相有功功率曲线", "1.1", 6, SetData, 82);

                //// 读测量点B相有功功率曲线
                //BaseVerifyUnit("读取测量点B相有功功率曲线", "1.1", 6, SetData, 83);

                //// 读测量点C相有功功率曲线
                //BaseVerifyUnit("读取测量点C相有功功率曲线", "1.1", 6, SetData, 84);
                //#endregion

                BaseVerifyUnit("F85.测量点无功功率曲线", SetData, new string[] { "0" }, new int[] { 6 }, 85, intIndex++);

                BaseVerifyUnit("F86.测量点A相无功功率曲线", SetData, new string[] { "0" }, new int[] { 6 }, 86, intIndex++);

                BaseVerifyUnit("F87.测量点B相无功功率曲线", SetData, new string[] { "0" }, new int[] { 6 }, 87, intIndex++);

                BaseVerifyUnit("F88.测量点C相无功功率曲线", SetData, new string[] { "0" }, new int[] { 6 }, 88, intIndex++);

                //#region 测量点无功功率曲线
                //// 读测量点无功功率曲线
                //BaseVerifyUnit("读取测量点无功功率曲线", "0", 6, SetData, 85);

                //// 读测量点A相无功功率曲线
                //BaseVerifyUnit("读取测量点A相无功功率曲线", "0", 6, SetData, 86);

                //// 读测量点B相无功功率曲线
                //BaseVerifyUnit("读取测量点B相无功功率曲线", "0", 6, SetData, 87);

                //// 读测量点C相无功功率曲线
                //BaseVerifyUnit("读取测量点C相无功功率曲线", "0", 6, SetData, 88);
                //#endregion

                BaseVerifyUnit("F89.A相电压曲线", SetData, new string[] { "220" }, new int[] { 6 }, 89, intIndex++);

                BaseVerifyUnit("F90.A相电压曲线", SetData, new string[] { "220" }, new int[] { 6 }, 90, intIndex++);

                BaseVerifyUnit("F91.A相电压曲线", SetData, new string[] { "220" }, new int[] { 6 }, 91, intIndex++);

                //#region 测量点电压曲线
                //// 读测量点A相电压曲线
                //BaseVerifyUnit("读取测量点A相电压曲线", "220", 6, SetData, 89);

                //// 读测量点B相电压曲线
                //BaseVerifyUnit("读取测量点B相电压曲线", "220", 6, SetData, 90);

                //// 读测量点C相电压曲线
                //BaseVerifyUnit("读取测量点C相电压曲线", "220", 6, SetData, 91);
                //#endregion

                BaseVerifyUnit("F92.A相电流曲线", SetData, new string[] { "5" }, new int[] { 6 }, 92, intIndex++);

                BaseVerifyUnit("F93.A相电流曲线", SetData, new string[] { "5" }, new int[] { 6 }, 93, intIndex++);

                BaseVerifyUnit("F94.A相电流曲线", SetData, new string[] { "5" }, new int[] { 6 }, 94, intIndex++);

                //#region 测量点电流曲线
                //// 读测量点A相电流曲线
                //BaseVerifyUnit("读取测量点A相电流曲线", "5", 6, SetData, 92);

                //// 读测量点B相电流曲线
                //BaseVerifyUnit("读取测量点B相电流曲线", "5", 6, SetData, 93);

                //// 读测量点C相电流曲线
                //BaseVerifyUnit("读取测量点C相电流曲线", "5", 6, SetData, 94);
                //#endregion

                BaseVerifyUnit("F97.正向有功总电能量曲线", SetData, new string[] { "0" }, new int[] { 6 }, 97, intIndex++);
                BaseVerifyUnit("F98.正向无功总电能量曲线", SetData, new string[] { "0" }, new int[] { 6 }, 98, intIndex++);
                BaseVerifyUnit("F99.反向有功总电能量曲线", SetData, new string[] { "0" }, new int[] { 6 }, 99, intIndex++);
                BaseVerifyUnit("F100.反向无功总电能量曲线", SetData, new string[] { "0" }, new int[] { 6 }, 100, intIndex++);





                //#region 测量点电能量曲线
                //// 读测量点正向有功总电能量曲线
                //BaseVerifyUnit("读测量点正向有功总电能量曲线", "0", 6, SetData, 97);

                //// 读测量点正向无功总电能量曲线
                //BaseVerifyUnit("读测量点正向无功总电能量曲线", "0", 6, SetData, 98);

                //// 读测量点反向有功总电能量曲线
                //BaseVerifyUnit("读测量点反向有功总电能量曲线", "0", 6, SetData, 99);

                //// 读测量点反向无功总电能量曲线
                //BaseVerifyUnit("读测量点反向无功总电能量曲线", "0", 6, SetData, 100);
                //#endregion
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes(dt_Temp.Date.AddDays(0).AddMinutes(615).ToString("mmhhddMMyy") + "0102");
                BaseVerifyUnit("F101.正向有功总电能示值曲线", SetData, new string[] { "100" }, new int[] { 6 }, 101, intIndex++);
                BaseVerifyUnit("F102.正向无功总电能示值曲线", SetData, new string[] { "200" }, new int[] { 6 }, 102, intIndex++);
                BaseVerifyUnit("F103.反向有功总电能示值曲线", SetData, new string[] { "100" }, new int[] { 6 }, 103, intIndex++);
                BaseVerifyUnit("F104.反向无功总电能示值曲线", SetData, new string[] { "200" }, new int[] { 6 }, 104, intIndex++);

                //#region 测量点电能示值曲线
                //// 读测量点正向有功总电能示值曲线
                //BaseVerifyUnit("读测量点正向有功总电能示值曲线", "100", 6, SetData, 101);

                //// 读测量点正向无功总电能示值曲线
                //BaseVerifyUnit("读测量点正向无功总电能示值曲线", "200", 6, SetData, 102);

                //// 读测量点反向有功总电能示值曲线
                //BaseVerifyUnit("读测量点反向有功总电能示值曲线", "100", 6, SetData, 103);

                //// 读测量点反向无功总电能示值曲线
                //BaseVerifyUnit("读测量点反向无功总电能示值曲线", "200", 6, SetData, 104);
                //#endregion

                BaseVerifyUnit("F105.测量点功率因数曲线", SetData, new string[] { "100" }, new int[] { 6 }, 105, intIndex++);
                BaseVerifyUnit("F106.测量点A相功率因数曲线", SetData, new string[] { "100" }, new int[] { 6 }, 106, intIndex++);
                BaseVerifyUnit("F107.测量点B相功率因数曲线", SetData, new string[] { "100" }, new int[] { 6 }, 107, intIndex++);
                BaseVerifyUnit("F108.测量点C相功率因数曲线", SetData, new string[] { "100" }, new int[] { 6 }, 108, intIndex++);

                //#region 测量点功率因数曲线
                //// 读测量点功率因数曲线
                //BaseVerifyUnit("读测量点功率因数曲线", "100", 6, SetData, 105);

                //// 读测量点A相功率因数曲线
                //BaseVerifyUnit("读测量点A相功率因数曲线", "100", 6, SetData, 106);

                //// 读测量点B相功率因数曲线
                //BaseVerifyUnit("读测量点B相功率因数曲线", "100", 6, SetData, 107);

                //// 读测量点C相功率因数曲线
                //BaseVerifyUnit("读测量点C相功率因数曲线", "100", 6, SetData, 108);
                //#endregion

                //#endregion
                MessageAdd("终端对时",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 31, DateTime.Now.ToString(), RecData, MaxWaitSeconds_Write);
                ControlVirtualMeter("Tim0");

            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
        private void BaseVerifyUnit(string p_str_Message, byte[] SetData, string[] p_str_CorrectValue, int[] p_int_DataIndex, byte p_byt_Fn, int int_index)
        {
            MessageAdd(p_str_Message, EnumLogType.提示与流程信息);
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
                            TempData[tableNo].Resoult = "合格";
                            s1[tableNo] += "," + GetData(RecData, tableNo, p_int_DataIndex[index], EnumTerimalDataType.e_string);
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
                        s2[tableNo] = p_str_CorrectValue[index];
                        MessageAdd("来源：" + (tableNo + 1) + "号表 / 错误：" + ex.ToString(), EnumLogType.错误信息);
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
