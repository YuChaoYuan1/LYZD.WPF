using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LYZD.Verify.ControlTest
{
    /// <summary>
    /// 营业报停功控
    /// </summary>
    public class BusinessStop_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "检定数据", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();


                //设置终端参数 F9.终端事件记录配置设置
                MessageAdd("设置终端事件记录配置...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("20000000000000000000000000000000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 9, SetData, RecData, MaxWaitSeconds_Write);

                //设置终端参数 F12.终端状态量输入参数
                MessageAdd("设置终端事件记录配置...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("0000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 12, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置终端功控时段参数...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("55A5AA55A5AA55A5AA55A5AA");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 18, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置终端时段功控定值浮动系数...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("00");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 19, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置终端时段功控定值...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("07FF00620062006200620062006200620062FF10671067106710671067106710671067FF00680068006800680068006800680068");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 41, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置终端时段厂休功控参数...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("006200081EFE02010205006200081EFE");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 42, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置终端功率控制的功率计算滑差时间...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("010201040501");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 43, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置终端时段功控定值...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("0101080201080062020108050101080201080062");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 44, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置终端功控轮次设定...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("030201100503");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 45, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置终端功控告警时间...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("030201010603");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 1, 49, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置终端终端保电解除...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 0, 33, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置终端时段功控解除...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 1, 17, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("厂休功控解除...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 1, 18, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("营业报停控解除...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 1, 19, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("当前功率下浮控解除...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 1, 20, SetData, RecData, MaxWaitSeconds_Write);

                //设置脉冲输出
                SetPulseOutput(GetYaoJian(), 0x03, 0.584f, 4, 2100, 0.584f, 4, 2100);
                Thread.Sleep(500);

                SetTime(Convert.ToDateTime("2008-1-1 0:0:0"), 0);

                MessageAdd("营业报停控投入...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 1, 11, SetData, RecData, MaxWaitSeconds_Write);

                WaitTime("等待", 150);

                //第1步，先判断终端是否产生告警
                MessageAdd("读终端当前控制状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 6, RecData, MaxWaitSeconds_Write);

                #region 越限后功控越限告警状态

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "营业报停控告警";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetData(RecData, i, 11, EnumTerimalDataType.e_bs8).Substring(5, 1) == "1" ? "营业报停控告警" : "营业报停控未告警");
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_bs8).Substring(5, 1) != "1")
                            {
                                //MessageAdd("终端" + (i + 1) + "不正确！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "不正确";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("越限后功控越限告警状态", TempData);

                #endregion

                WaitTime("等待", 480);

                #region 读终端当前控制状态

                //第2步，先判断终端是否产生跳闸
                MessageAdd("读终端当前控制状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 6, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "各轮都跳闸";
                        if (TalkResult[i] == 0)
                        {
                            ResultDictionary[""][i] = (GetData(RecData, i, 8, EnumTerimalDataType.e_bs8).Substring(6, 2) == "11" ? "各轮都跳闸" : "有轮次未跳闸");
                            if (GetData(RecData, i, 8, EnumTerimalDataType.e_bs8).Substring(6, 2) != "11")
                            {
                                //MessageAdd("终端" + (i + 1) + "不正确！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "不正确";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("越限后功控跳闸输出状态", TempData);
                #endregion


                WaitTime("等待", 180);

                #region 功控跳闸事件

                //第3步，先判断终端是否产生跳闸事件
                // 读取终端事件
                MessageAdd("读取终端最近一条事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        string[] str_ReadData = ReadTerminalEvent(i, false, "ERC6");
                        TempData[i].StdData = "ERC6";
                        if (str_ReadData.Length > 7)
                        {
                            TempData[i].Data = GetData(str_ReadData, 7, EnumTerimalDataType.e_string);
                            if (GetData(str_ReadData, 7, EnumTerimalDataType.e_string) != "ERC6")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "终端未产生功控跳闸事件";
                            }
                            else
                            {
                                TempData[i].Resoult = "合格";
                                TempData[i].Tips = "ERC:" + str_ReadData[7] + ";发生时间:" + str_ReadData[9] + ";总加组号:" + str_ReadData[10] + ";功控类别:" + str_ReadData[12];
                            }
                        }
                        else
                        {
                            //MessageAdd("终端未产生功控跳闸事件", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "终端未产生功控跳闸事件";
                        }
                    }
                }
                AddItemsResoult("功控跳闸事件", TempData);
                #endregion

                #region 事件发生后功控跳闸输出状态

                //第4步，判断终端是否仍跳闸
                MessageAdd("读终端当前控制状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 6, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "各轮都跳闸";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetData(RecData, i, 8, EnumTerimalDataType.e_bs8).Substring(6, 2) == "11" ? "各轮都跳闸" : "有轮次未跳闸");
                            if (GetData(RecData, i, 8, EnumTerimalDataType.e_bs8).Substring(6, 2) != "11")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "不正确";
                            }
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("事件发生后功控跳闸输出状态", TempData);

                #endregion

                //遥信停止
                ContnrRemoteSignalingStatusOutputStop();
                //deviceDriver.StopRemoteSignalOutput(GlobalUnit.g_TerminalVerifyFlags, 0, 2);
                //Thread.Sleep(500);
                //deviceDriver.StopTest(GlobalUnit.g_TerminalVerifyFlags, 4);
                //Thread.Sleep(500);
                DeviceControl.SetPulseOutput(0x00, 0xff, 0.584f, 4, 2100, 0.584f, 4, 2100);
                MessageAdd("营业报停控控解除...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 1, 19, SetData, RecData, MaxWaitSeconds_Write);

                WaitTime("等待", 180);

                #region 解除后功控越限告警状态

                //第5步，脉冲停止后告警状态及跳闸状态
                MessageAdd("读终端当前控制状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 6, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    TempData[i].Resoult = "合格";
                }
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "营业报停控控未告警";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetData(RecData, i, 11, EnumTerimalDataType.e_bs8).Substring(5, 1) == "1" ? "营业报停控控告警" : "营业报停控控未告警");
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_bs8).Substring(5, 1) != "0")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "不正确";
                            }
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("解除后功控越限告警状态", TempData);

                #endregion


                #region 解除后功控跳闸输出状态

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "轮次不跳闸";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetData(RecData, i, 8, EnumTerimalDataType.e_bs8).Substring(6, 2) == "11" ? "有轮次跳闸" : "轮次不跳闸");
                            if (GetData(RecData, i, 8, EnumTerimalDataType.e_bs8).Substring(6, 2) != "00")
                            {
                                //MessageAdd("终端" + (i + 1) + "不正确！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = ResultDictionary[""][i] + "|" + "不正确";
                            }
                        }
                        else
                        {
                            //MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("解除后功控跳闸输出状态", TempData);
                #endregion

            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }

}
