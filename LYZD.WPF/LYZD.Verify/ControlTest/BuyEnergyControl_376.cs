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
    /// 购电控
    /// </summary>
    public class BuyEnergyControl_376 : VerifyBase
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

                //TODO 遥控数量

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
                byte byt_yk = 0;
                for (int i = 0; i < VerifyConfig.RemoteControlCoutnt; i++)
                {
                    byt_yk += Convert.ToByte(Math.Pow(2, i));
                }
                SetData = UsefulMethods.ConvertStringToBytes("030201800503");
                SetData = new byte[1] { byt_yk };
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
                MessageAdd("购电控投入...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 2, 16, SetData, RecData, MaxWaitSeconds_Write);

                WaitTime("等待", 30);

                ControlVirtualMeter("DLS11.81");

                WaitTime("等待", 360);

                #region 越限后功控越限告警状态

                //第1步，先判断终端是否产生告警
                MessageAdd("读终端当前控制状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 6, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "购电控告警";
                        if (TalkResult[i] == 0)
                        {
                            int iTmp = 19;//个别厂家存在总加组1为0的时候，所以总加组2的数据要往前递进7
                            if (GetData(RecData, i, 5, EnumTerimalDataType.e_bs8).Substring(7, 1) == "0")
                                iTmp = 12;
                            TempData[i].Data = GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(6, 1) == "1" ? "购电控告警" : "购电控未告警";
                            if (GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(6, 1) != "1")
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

                #endregion


                ControlVirtualMeter("DLS11.91");

                WaitTime("等待", VerifyConfig.RemoteControlCoutnt * (3 + 1) * 60);


                #region 越限后功控跳闸输出状态

                //第2步，先判断终端是否产生跳闸
                MessageAdd("读终端当前控制状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 6, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "各轮都跳闸" + "1".ToString().PadLeft(VerifyConfig.RemoteControlCoutnt, '1');
                        if (TalkResult[i] == 0)
                        {
                            int iTmp = 17;
                            if (GetData(RecData, i, 5, EnumTerimalDataType.e_bs8).Substring(7, 1) == "0")
                                iTmp = 10;
                            TempData[i].Data = GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(8 - VerifyConfig.RemoteControlCoutnt, VerifyConfig.RemoteControlCoutnt) == "1".ToString().PadLeft(VerifyConfig.RemoteControlCoutnt, '1') ? "各轮都跳闸" + "1".ToString().PadLeft(VerifyConfig.RemoteControlCoutnt, '1') : "有轮次未跳闸";
                            if (GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(8 - VerifyConfig.RemoteControlCoutnt, VerifyConfig.RemoteControlCoutnt) != "1".ToString().PadLeft(VerifyConfig.RemoteControlCoutnt, '1'))
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

                #region 功控跳闸事件

                WaitTime("等待", 180);

                //第3步，先判断终端是否产生跳闸事件
                // 读取终端事件
                MessageAdd("读取终端最近一条事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        string[] str_ReadData = ReadTerminalEvent(i, false, "ERC7");
                        TempData[i].StdData = "ERC7";
                        if (str_ReadData.Length > 7)
                        {
                            TempData[i].Data = GetData(str_ReadData, 7, EnumTerimalDataType.e_string);
                            if (GetData(str_ReadData, 7, EnumTerimalDataType.e_string) != "ERC7")
                            {
                                //MessageAdd("终端未产生功控跳闸事件", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "终端未产生功控跳闸事件";
                            }
                            else
                            {
                                //MessageAdd("终端产生功控跳闸事件！",EnumLogType.流程信息);
                                TempData[i].Resoult = "合格";
                                TempData[i].Tips = "ERC:" + GetData(str_ReadData, 7, EnumTerimalDataType.e_string) + ";发生时间:" + GetData(str_ReadData, 9, EnumTerimalDataType.e_string) + ";总加组号:" + GetData(str_ReadData, 10, EnumTerimalDataType.e_string) + ";功控类别:" + GetData(str_ReadData, 12, EnumTerimalDataType.e_string);
                            }
                        }
                        else
                        {
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
                        TempData[i].StdData = "各轮都跳闸" + "1".ToString().PadLeft(VerifyConfig.RemoteControlCoutnt, '1');
                        if (TalkResult[i] == 0)
                        {
                            int iTmp = 17;
                            if (GetData(RecData, i, 5, EnumTerimalDataType.e_bs8).Substring(7, 1) == "0")
                                iTmp = 10;
                            TempData[i].Data = GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(8 - VerifyConfig.RemoteControlCoutnt, VerifyConfig.RemoteControlCoutnt) == "1".ToString().PadLeft(VerifyConfig.RemoteControlCoutnt, '1') ? "各轮都跳闸" + "1".ToString().PadLeft(VerifyConfig.RemoteControlCoutnt, '1') : "有轮次未跳闸";
                            if (GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(8 - VerifyConfig.RemoteControlCoutnt, VerifyConfig.RemoteControlCoutnt) != "1".ToString().PadLeft(VerifyConfig.RemoteControlCoutnt, '1'))
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

                #region 解除后功控越限告警状态

                ControlVirtualMeter("DLS00010");
                WaitTime("等待", 10);
                MessageAdd("购电控解除...",EnumLogType.流程信息);
                SetData = UsefulMethods.ConvertStringToBytes("");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(5, 2, 24, SetData, RecData, MaxWaitSeconds_Write);

                WaitTime("等待", 10);
                //第5步，脉冲停止后告警状态及跳闸状态
                MessageAdd("读终端当前控制状态",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 0, 6, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "购电控未告警";
                        if (TalkResult[i] == 0)
                        {
                            int iTmp = 19;
                            if (GetData(RecData, i, 5, EnumTerimalDataType.e_bs8).Substring(7, 1) == "0")
                                iTmp = 12;
                            TempData[i].Data = GetData(RecData, i, iTmp, EnumTerimalDataType.e_bs8).Substring(7, 1) == "1" ? "购电控告警" : "购电控未告警";
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
                #endregion

                #region 解除后功控跳闸输出状态

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "轮次不跳闸";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 17, EnumTerimalDataType.e_bs8).Substring(8 - VerifyConfig.RemoteControlCoutnt, VerifyConfig.RemoteControlCoutnt) == "0".ToString().PadLeft(VerifyConfig.RemoteControlCoutnt, '1') ? "轮次不跳闸" : "有轮次跳闸";
                            if (GetData(RecData, i, 17, EnumTerimalDataType.e_bs8).Substring(8 - VerifyConfig.RemoteControlCoutnt, VerifyConfig.RemoteControlCoutnt) != "0".ToString().PadLeft(VerifyConfig.RemoteControlCoutnt, '0'))
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
                #endregion
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
