using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LYZD.Verify.DataCollect
{
    /// <summary>
    /// 12个/分脉冲量采集
    /// </summary>
    public class PulseCollection12Unitmin_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "总加组1有功功率", "测量点3有功功率", "测量点3月最大需量", "测量点3电能量", "总加3有功电能量", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();

                Dictionary<int, string[]> m_dic_RecData_F35 = new Dictionary<int, string[]>();
                Dictionary<int, string[]> m_dic_RecData_F17 = new Dictionary<int, string[]>();
                Dictionary<int, string[]> m_dic_RecData_F19 = new Dictionary<int, string[]>();

                byte GroupTotalCount = VerifyConfig.GroupTotalCount;

                byte GroupTotal1Count = VerifyConfig.GroupTotal1Count;

                byte GroupTotal2Count = VerifyConfig.GroupTotal2Count;

                string[] GroupTotal1Pn = VerifyConfig.GroupTotal1Pn.Split('|');

                string[] GroupTotal2Pn = VerifyConfig.GroupTotal2Pn.Split('|');

                byte PulseCount = VerifyConfig.PulseCount;

                MessageAdd("台体输出脉冲信号。", EnumLogType.错误信息);


                //if (deviceDriver.m_strErrorBoardType == "CL188M")
                //{
                //    deviceDriver.StopRemoteSignalOutput(GlobalUnit.g_TerminalVerifyFlags, 0, 2);

                //    deviceDriver.StopRemoteSignalOutput(GlobalUnit.g_TerminalVerifyFlags, 0, 3);
                //}
                //else
                //{
                //    deviceDriver.StopRemoteSignalOutput(GlobalUnit.g_TerminalVerifyFlags, 0, int.Parse(PulseCount.ToString()));
                //}


                //deviceDriver.StopTest(GlobalUnit.g_TerminalVerifyFlags, 4);
                DeviceControl.SetPulseOutput(0x00, 0xff, 2f, 16, 2160, 2f, 16, 2160);

                Thread.Sleep(500);

                // MessageAdd("设置终端参数 F11.终端脉冲配置参数...",EnumLogType.错误信息);
                //SetData = Core.Function.UsefulMethods.ConvertStringToBytes("02010300E803020400E803");


                //SetData = new byte[PulseCount * 5 + 1];
                //SetData[0] = PulseCount;
                //for (byte i = 0; i < PulseCount; i++)
                //{
                //    SetData[i * 5 + 1] = Convert.ToByte(i + 1);
                //    SetData[i * 5 + 2] = Convert.ToByte(i + 3);
                //    SetData[i * 5 + 3] = 0;
                //    SetData[i * 5 + 4] = 0xe8;
                //    SetData[i * 5 + 5] = 0x03;
                //}
                //TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 11, SetData, RecData, MaxWaitSeconds_Write);

                //for (byte p1 = 3; p1 <= PulseCount + 2; p1++)
                //{
                //     MessageAdd("下发终端参数：设置测量点" + p1 + "基本参数。",EnumLogType.错误信息);
                //    TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(4, p1, 25, "220,1.5,9.9", RecData, MaxWaitSeconds_Write);
                //}
                #region 总加组2 抄模拟表
                if (GroupTotalCount > 1)
                {
                    //#region 总加组2 抄模拟表

                    // MessageAdd("下发终端参数：测量点。",EnumLogType.错误信息);
                    //SetData = Core.Function.UsefulMethods.ConvertStringToBytes("020007000000621E07000000000000000000000004090100000000000008000000621E080000000000000000000000040901000000000000");
                    //TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 10, SetData, RecData, MaxWaitSeconds_Write);

                    // MessageAdd("下发终端参数：测量点。",EnumLogType.错误信息);
                    //SetData = Core.Function.UsefulMethods.ConvertStringToBytes("020007000700621E07000000000000000000000004090100000000000008000800621E080000000000000000000000040901000000000000");
                    //TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 10, SetData, RecData, MaxWaitSeconds_Write);

                    //#endregion

                    //for (byte p1 = 7; p1 <= GroupTotal2Count + 6; p1++)
                    //{
                    //     MessageAdd("下发终端参数：设置测量点" + p1 + "基本参数。",EnumLogType.错误信息);
                    //    TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(4, p1, 25, "220,1.5,9.9", RecData, MaxWaitSeconds_Write);
                    //}
                }
                #endregion
                // MessageAdd("设置终端参数 F14.总加组配置...",EnumLogType.错误信息);
                //SetData = Core.Function.UsefulMethods.ConvertStringToBytes("02010102020101");


                //if (GroupTotalCount == 1)
                //    SetData = new byte[GroupTotal1Count + 2 + 1];
                //else
                //    SetData = new byte[GroupTotal1Count + 2 + GroupTotal2Count + 2 + 1];

                //SetData[0] = Convert.ToByte(GroupTotalCount);

                //for (byte i = 0; i < GroupTotalCount; i++)
                //{
                //    if (i == 0)
                //    {
                //        SetData[i * (GroupTotal1Count + 2) + 1] = Convert.ToByte(i + 1);
                //        SetData[i * (GroupTotal1Count + 2) + 2] = Convert.ToByte(GroupTotal1Count);
                //        SetData[i * (GroupTotal1Count + 2) + 3] = Convert.ToByte(int.Parse(GroupTotal1Pn[0]) - 1);
                //        if (GroupTotal1Count > 1 && GroupTotal1Pn.Length > 1)
                //            SetData[i * (GroupTotal1Count + 2) + 4] = Convert.ToByte(int.Parse(GroupTotal1Pn[1]) - 1);
                //    }
                //    else
                //    {
                //        SetData[i * (GroupTotal2Count + 2) + 1] = Convert.ToByte(i + 1);
                //        SetData[i * (GroupTotal2Count + 2) + 2] = Convert.ToByte(GroupTotal2Count);
                //        SetData[i * (GroupTotal2Count + 2) + 3] = Convert.ToByte(int.Parse(GroupTotal2Pn[0]) - 1);
                //        if (GroupTotal2Count > 1 && GroupTotal2Pn.Length > 1)
                //            SetData[i * (GroupTotal2Count + 2) + 4] = Convert.ToByte(int.Parse(GroupTotal2Pn[1]) - 1);

                //    }
                //}
                //TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 14, SetData, RecData, MaxWaitSeconds_Write);


                MessageAdd("设置终端参数 F12.终端状态量输入参数...", EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("0000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 12, SetData, RecData, MaxWaitSeconds_Write);

                #region 数据区初始化
                ResetTerimal(2);
                #endregion

                Thread.Sleep(3000);

                MessageAdd("台体输出脉冲信号。", EnumLogType.错误信息);

                //if (deviceDriver.m_strErrorBoardType == "CL188M")
                //{
                //    deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, 2, 1, 216, 0.2f, 1.6f);

                //    deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, 3, 1, 216, 0.2f, 1.6f);
                //}
                //else
                //{
                //    deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, int.Parse(PulseCount.ToString()), 1, 216, 0.2f, 1.6f);
                //}
                DeviceControl.SetPulseOutput(0x03, 0xff, 0.2f, 1.6f, 216, 0.2f, 1.6f, 216);
                Thread.Sleep(500);

                //if (GroupTotalCount > 1)
                //{
                //    ControlVirtualMeter("Cmd,Set,220,220,220,1.5,1.5,1.5,0,0,0,1,0");
                //    ControlVirtualMeter("Cmd,DLS,8000,4000,1000,1000,1000,1000");
                //}
                WaitTime("等待", 360);
                //if (GroupTotalCount > 1)
                //{
                //    ControlVirtualMeter("Cmd,DLS,8100,4000,1000,1000,1000,1000");
                //}
                //m_int_ItemIndex = 1;

                for (byte p1 = 1; p1 <= 1; p1++)
                {
                    int intTotalCount = GroupTotal1Count;
                    int intEnergy = 720 * intTotalCount;
                    if (p1 > 1)
                    {
                        intTotalCount = GroupTotal2Count;
                        intEnergy = 990 * intTotalCount;
                    }

                    MessageAdd("读总加组有功功率。", EnumLogType.错误信息);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 17, m_dic_RecData_F17, MaxWaitSeconds_Read485);

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = intEnergy.ToString();
                            if (TalkResult[i] == 0 && m_dic_RecData_F17[i].Length >= 4)
                            {

                                TempData[i].Data = GetData(m_dic_RecData_F17, i, 3, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F17, i, 3, EnumTerimalDataType.e_double)) - intEnergy) / (intEnergy / 100));

                                if (Math.Abs((double.Parse(GetData(m_dic_RecData_F17, i, 3, EnumTerimalDataType.e_double)) - intEnergy) / (intEnergy / 100)) <= 4)
                                {
                                    TempData[i].Resoult = "合格";
                                    //ResultDictionary[""][i] = GetData(m_dic_RecData_F17, i, 3, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F17, i, 3, EnumTerimalDataType.e_double)) - intEnergy) / (intEnergy / 100)) + "|" + intEnergy;
                                }
                                else
                                {
                                    TempData[i].Resoult = "不合格";
                                    //ResultDictionary[""][i] = GetData(m_dic_RecData_F17, i, 3, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F17, i, 3, EnumTerimalDataType.e_double)) - intEnergy) / (intEnergy / 100)) + "|" + intEnergy + "|误差超限2";
                                    TempData[i].Tips = "误差超限";
                                }
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                    }
                    AddItemsResoult("总加组" + p1 + "有功功率", TempData);

                }
                for (byte p1 = 3; p1 <= GroupTotal1Count + 2; p1++)
                {
                    MessageAdd("读当前总有功功率。", EnumLogType.错误信息);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 25, RecData, MaxWaitSeconds_Read485);

                    int intEnergy = 720;

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = intEnergy.ToString();
                            if (TalkResult[i] == 0 && RecData[i].Length >= 9)
                            {
                                //TempData[i].Data = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                                TempData[i].Data = (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_string)) * 1000).ToString("F2") + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_double)) * 1000 - intEnergy) / 7.2);

                                if (Math.Abs((double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_double)) * 1000 - intEnergy)) / 7.2 <= 2)
                                {
                                    TempData[i].Resoult = "合格";
                                    //ResultDictionary[""][i] = (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_string)) * 1000).ToString("F2") + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_double)) * 1000 - intEnergy) / 7.2) + "|" + intEnergy.ToString();
                                }
                                else
                                {
                                    TempData[i].Resoult = "不合格";
                                    //ResultDictionary[""][i] = (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_string)) * 1000).ToString("F2") + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_double)) * 1000 - intEnergy) / 7.2) + "|" + intEnergy.ToString() + "|误差超限2";
                                    TempData[i].Tips = "误差超限";
                                }
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                    }
                    AddItemsResoult("测量点" + p1 + "有功功率", TempData);


                }
                #region 总加组2 抄模拟表
                //if (GroupTotalCount > 1)
                //{
                //    for (byte p1 = 7; p1 <= GroupTotal2Count + 6; p1++)
                //    {
                //         MessageAdd("读当前总有功功率。",EnumLogType.错误信息);
                //        TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 25, RecData, MaxWaitSeconds_Read485);

                //        int intEnergy = 990;
                //        for (int i = 0; i < MeterNumber; i++)
                //        {
                //           TempData[i].Resoult="合格";
                //        }
                //        for (int i = 0; i < MeterNumber; i++)
                //        {
                //            if (meterInfo[i].YaoJianYn)
                //            {
                //                if (TalkResult[i] == 0 && RecData[i].Length >= 9)
                //                {
                //                    ResultDictionary[""][i] = GetData(RecData, i, 4, EnumTerimalDataType.e_string);

                //                    if (Math.Abs((double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_double)) * 1000 - intEnergy)) / 9.9 <= 2)
                //                    {
                //                       TempData[i].Resoult="合格";
                //                        ResultDictionary[""][i] = (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_string)) * 1000).ToString("F2") + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_double)) * 1000 - intEnergy) / 9.9) + "|" + intEnergy.ToString();
                //                    }
                //                    else
                //                    {
                //                       TempData[i].Resoult="不合格";
                //                        ResultDictionary[""][i] = (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_string)) * 1000).ToString("F2") + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_double)) * 1000 - intEnergy) / 9.9) + "|" + intEnergy.ToString() + "|误差超限2";
                //                         MessageAdd("终端" + (i + 1) + "误差超限！",EnumLogType.错误信息);
                //                    }
                //                }
                //                else
                //                {
                //                    MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
                //                   TempData[i].Resoult="不合格";
                //                    ResultDictionary[""][i] = "无回复";
                //                }
                //            }
                //        }
                //         RefUIData("测量点" + p1 + "有功功率");

                //    }
                //}
                #endregion
                WaitTime("等待", 840);

                for (byte p1 = 3; p1 <= GroupTotal1Count + 2; p1++)
                {

                    MessageAdd("读当月正向有/无功最大需量及发生时间。", EnumLogType.错误信息);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 35, m_dic_RecData_F35, MaxWaitSeconds_Read485);


                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                    TempData[i].StdData ="0.72";
                            if (TalkResult[i] == 0 && m_dic_RecData_F35[i].Length >= 25)
                            {
                                TempData[i].Data = GetData(RecData, i, 35, EnumTerimalDataType.e_string);
                                TempData[i].Data = GetData(m_dic_RecData_F35, i, 5, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F35, i, 5, EnumTerimalDataType.e_double)) - 0.72) / 0.0072) ;

                                if (Math.Abs((double.Parse(GetData(m_dic_RecData_F35, i, 5, EnumTerimalDataType.e_double)) - 0.72) / 0.0072) <= 2)
                                {
                                    TempData[i].Resoult = "合格";
                                   //ResultDictionary[""][i] = GetData(m_dic_RecData_F35, i, 5, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F35, i, 5, EnumTerimalDataType.e_double)) - 0.72) / 0.0072) + "|0.72";
                                }
                                else
                                {
                                    TempData[i].Resoult = "不合格";
                                    //ResultDictionary[""][i] = GetData(RecData, i, 35, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F35, i, 5, EnumTerimalDataType.e_double)) - 0.72) / 0.0072) + "|0.72|误差超限2";
                                    TempData[i].Tips = "误差超限";
                                }
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                    }
                    AddItemsResoult("测量点" + p1 + "月最大需量", TempData);

                }

                for (byte p1 = 3; p1 <= GroupTotal1Count + 2; p1++)
                {
                    MessageAdd("读当前总有功电量。", EnumLogType.错误信息);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 33, RecData, MaxWaitSeconds_Write);

                    int intEnergy = 216;
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                    TempData[i].StdData = intEnergy.ToString();
                            if (TalkResult[i] == 0 && RecData[i].Length >= 9)
                            {
                                TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                                TempData[i].Data = (double.Parse(GetData(RecData, i, 5, EnumTerimalDataType.e_string)) * 1000).ToString("F2") + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 5, EnumTerimalDataType.e_double)) * 1000 - intEnergy));

                                if (Math.Abs((double.Parse(GetData(RecData, i, 5, EnumTerimalDataType.e_double)) * 1000 - intEnergy)) <= 2)
                                {
                                    TempData[i].Resoult = "合格";
                                    //ResultDictionary[""][i] = (double.Parse(GetData(RecData, i, 5, EnumTerimalDataType.e_string)) * 1000).ToString("F2") + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 5, EnumTerimalDataType.e_double)) * 1000 - intEnergy)) + "|" + intEnergy.ToString();
                                }
                                else
                                {
                                    TempData[i].Resoult = "不合格";
                                    //ResultDictionary[""][i] = (double.Parse(GetData(RecData, i, 5, EnumTerimalDataType.e_string)) * 1000).ToString("F2") + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 5, EnumTerimalDataType.e_double)) * 1000 - intEnergy)) + "|" + intEnergy.ToString() + "|误差超限2";
                                    TempData[i].Tips = "误差超限";
                                }
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                    }
                    AddItemsResoult("测量点" + p1 + "电能量", TempData);

                }
                #region 总加组2 抄模拟表
                if (GroupTotalCount > 1)
                {
                    //for (byte p1 = 7; p1 <= GroupTotal2Count + 6; p1++)
                    //{
                    //     MessageAdd("读当前总有功电量。",EnumLogType.错误信息);
                    //    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 41, RecData, MaxWaitSeconds_Write);

                    //    int intEnergy = 100000;
                    //    for (int i = 0; i < MeterNumber; i++)
                    //    {
                    //       TempData[i].Resoult="合格";
                    //    }
                    //    for (int i = 0; i < MeterNumber; i++)
                    //    {
                    //        if (meterInfo[i].YaoJianYn)
                    //        {
                    //            if (TalkResult[i] == 0 && RecData[i].Length >= 9)
                    //            {
                    //                ResultDictionary[""][i] = GetData(RecData, i, 4, EnumTerimalDataType.e_string);

                    //                if (Math.Abs((double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_double)) * 1000  - intEnergy)) <= 2)
                    //                {
                    //                   TempData[i].Resoult="合格";
                    //                    ResultDictionary[""][i] = (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_string)) * 1000).ToString("F2") + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_double)) * 1000 - intEnergy)) + "|" + intEnergy.ToString();
                    //                }
                    //                else
                    //                {
                    //                   TempData[i].Resoult="不合格";
                    //                    ResultDictionary[""][i] = (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_string)) * 1000).ToString("F2") + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_double)) * 1000 - intEnergy)) + "|" + intEnergy.ToString() + "|误差超限2";
                    //                     MessageAdd("终端" + (i + 1) + "误差超限！",EnumLogType.错误信息);
                    //                }
                    //            }
                    //            else
                    //            {
                    //                MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
                    //               TempData[i].Resoult="不合格";
                    //                ResultDictionary[""][i] = "无回复";
                    //            }
                    //        }
                    //    }
                    //     RefUIData("测量点" + p1 + "电能量");
                    //}
                }
                #endregion
                for (byte p1 = 1; p1 <= 1; p1++)
                {
                    MessageAdd("读总加有功电能量。", EnumLogType.错误信息);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 19, m_dic_RecData_F19, MaxWaitSeconds_Read485);

                    int intTotalCount = GroupTotal1Count;
                    int intEnergy = 216 * intTotalCount;
                    if (p1 > 1)
                    {
                        intTotalCount = GroupTotal2Count;
                        intEnergy = 100000 * intTotalCount;
                    }
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                    TempData[i].StdData = intEnergy.ToString();
                            if (TalkResult[i] == 0 && m_dic_RecData_F19[i].Length >= 9)
                            {
                                //TempData[i].Data = GetData(m_dic_RecData_F19, i, 4, EnumTerimalDataType.e_string);
                                TempData[i].Data = GetData(m_dic_RecData_F19, i, 4, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F19, i, 4, EnumTerimalDataType.e_double)) - intEnergy)) ;

                                if (Math.Abs((double.Parse(GetData(m_dic_RecData_F19, i, 4, EnumTerimalDataType.e_double)) - intEnergy)) <= 2)
                                {
                                    TempData[i].Resoult = "合格";
                                    //ResultDictionary[""][i] = GetData(m_dic_RecData_F19, i, 4, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F19, i, 4, EnumTerimalDataType.e_double)) - intEnergy)) + "|" + intEnergy.ToString();
                                }
                                else
                                {
                                    TempData[i].Resoult = "不合格";
                                    //ResultDictionary[""][i] = GetData(m_dic_RecData_F19, i, 4, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F19, i, 4, EnumTerimalDataType.e_double)) - intEnergy)) + "|" + intEnergy.ToString() + "|误差超限2";
                                    TempData[i].Tips = "误差超限";
                                }
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";

                            }
                        }
                    }
                    AddItemsResoult("总加" + p1 + "有功电能量", TempData);

                }

                // MessageAdd("下发终端参数：测量点。",EnumLogType.错误信息);
                //SetData = Core.Function.UsefulMethods.ConvertStringToBytes("020007000000621E07000000000000000000000004090100000000000008000000611E080000000000000000000000040901000000000000");                
                //TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 10, SetData, RecData, MaxWaitSeconds_Write);

            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
