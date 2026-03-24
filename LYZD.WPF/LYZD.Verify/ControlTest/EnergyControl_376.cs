using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.ControlTest
{
    /// <summary>
    /// 月电控
    /// </summary>
    public class EnergyControl_376 : VerifyBase
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

                ControlVirtualMeter("Cmd,Set,220,220,220,1.5,1.5,1.5,0,0,0,1,0");
                ControlVirtualMeter("DLS00010");

                ResetTerimal(2);
                //设置终端参数 F9.终端事件记录配置设置
                MessageAdd("设置终端事件记录配置...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("40000000000000000000000000000000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 9, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置月电量控定值...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("0030000080020120050030000080");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 2, 46, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置购电量控参数...",EnumLogType.流程信息);
                DateTime dtTmp = DateTime.Now;
                SetData = UsefulMethods.ConvertStringToBytes(UsefulMethods.OrgBinFun(dtTmp.Minute + dtTmp.Hour * 100 + dtTmp.Day * 10000 + dtTmp.Month * 1000000, 4) + "AA002000000002000000010000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 2, 47, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置终端月电能量控定值浮动系数...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("00");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 20, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置电控轮次设定...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("030201800503");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 2, 48, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置终端终端保电解除...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 0, 33, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置月电控解除...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 2, 23, SetData, RecData, MaxWaitSeconds_Write);

                MessageAdd("设置购电控解除...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 2, 24, SetData, RecData, MaxWaitSeconds_Write);


                SetTime(Convert.ToDateTime("2008-1-1 8:0:0"), 0);

                ControlVirtualMeter("DLS00010");
                WaitTime("等待", VerifyConfig.WaitTime_CopyMeter);
                MessageAdd("月电控投入...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 2, 15, SetData, RecData, MaxWaitSeconds_Write);
                WaitTime("等待", 30);
                ControlVirtualMeter("DLS12.44");

                WaitTime("等待", 240);

                //第1步，先判断终端是否产生告警
                MessageAdd("读终端当前控制状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 6, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "月电控告警";
                        if (TalkResult[i] == 0)
                        {
                            int iTmp = 19;
                            if (GetData(RecData, i, 5, EnumTerimalDataType.e_bs8).Substring(7, 1) == "0")
                                iTmp = 12;
                            TempData[i].Data = GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(7, 1) == "1" ? "月电控告警" : "月电控未告警";
                            if (GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(7, 1) != "1")
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
                AddItemsResoult("越限后功控越限告警状态", TempData);


                ControlVirtualMeter("DLS13.04");

                WaitTime("等待", 240);

                //第2步，先判断终端是否产生跳闸
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
                        TempData[i].StdData = "各轮都跳闸";
                        if (TalkResult[i] == 0)
                        {
                            int iTmp = 16;
                            if (GetData(RecData, i, 5, EnumTerimalDataType.e_bs8).Substring(7, 1) == "0")
                                iTmp = 9;
                            ResultDictionary[""][i] = GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(6, 2) == "11" ? "各轮都跳闸" : "有轮次未跳闸";
                            if (GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(6, 2) != "11")
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
                AddItemsResoult("越限后功控跳闸输出状态", TempData);


                WaitTime("等待", 180);

                //第3步，先判断终端是否产生跳闸事件
                // 读取终端事件
                MessageAdd("读取终端最近一条事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "ERC7";
                        string[] str_ReadData = ReadTerminalEvent(i, false, "ERC7");
                        if (str_ReadData.Length > 7)
                        {
                            TempData[i].Data = GetData(str_ReadData, 7, EnumTerimalDataType.e_string);
                            if (GetData(str_ReadData, 7, EnumTerimalDataType.e_string) != "ERC7")
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "终端未产生功控跳闸事件";
                            }
                            else
                            {
                                TempData[i].Resoult = "合格";
                                TempData[i].Tips = "ERC:" + str_ReadData[7] + ";发生时间:" + str_ReadData[9] + ";总加组号:" + str_ReadData[10] + ";功控类别:" + str_ReadData[12] + "|ERC:6";
                            }
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "终端未产生功控跳闸事件";
                        }
                    }
                }
                AddItemsResoult("月电控跳闸事件", TempData);


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
                            int iTmp = 16;
                            if (GetData(RecData, i, 5, EnumTerimalDataType.e_bs8).Substring(7, 1) == "0")
                                iTmp = 9;
                            TempData[i].Data = GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(6, 2) == "11" ? "各轮都跳闸" : "有轮次未跳闸";
                            if (GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(6, 2) != "11")
                            {
                                MessageAdd("终端" + (i + 1) + "不正确！", EnumLogType.错误信息);
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


                ControlVirtualMeter("DLS00010");

                WaitTime("等待", 10);

                MessageAdd("月电控解除...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 2, 23, SetData, RecData, MaxWaitSeconds_Write);

                WaitTime("等待", 10);

                //第5步，脉冲停止后告警状态及跳闸状态
                MessageAdd("读终端当前控制状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 6, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "月电控未告警";
                        if (TalkResult[i] == 0)
                        {
                            int iTmp = 19;
                            if (GetData(RecData, i, 5, EnumTerimalDataType.e_bs8).Substring(7, 1) == "0")
                                iTmp = 12;
                            TempData[i].Data = GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(7, 1) == "1" ? "月电控告警" : "月电控未告警";
                            if (GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(7, 1) != "0")
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



                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "轮次不跳闸";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 16, EnumTerimalDataType.e_bs8).Substring(6, 2) == "11" ? "有轮次跳闸" : "轮次不跳闸";
                            if (GetData(RecData, i, 16, EnumTerimalDataType.e_bs8).Substring(6, 2) != "00")
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
                AddItemsResoult("解除后功控跳闸输出状态", TempData);


            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
