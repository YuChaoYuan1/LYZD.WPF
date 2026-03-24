using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LYZD.Verify.DataCollect
{       /// <summary>
        ///  120个/分脉冲量采集
        /// </summary>
    public class PulseCollection120Unitmin_376 : VerifyBase
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


                MessageAdd("台体输出脉冲信号。", EnumLogType.错误信息);

                byte PulseCount = VerifyConfig.PulseCount;

                //if (deviceDriver.m_strErrorBoardType == "CL188M")
                //{
                //    deviceDriver.StopRemoteSignalOutput(GlobalUnit.g_TerminalVerifyFlags, 0, 2);

                //    deviceDriver.StopRemoteSignalOutput(GlobalUnit.g_TerminalVerifyFlags, 0, 3);
                //}
                //else
                //{
                //    deviceDriver.StopRemoteSignalOutput(GlobalUnit.g_TerminalVerifyFlags, 0, int.Parse(PulseCount.ToString()));
                //    Thread.Sleep(500);
                //    deviceDriver.StopTest(GlobalUnit.g_TerminalVerifyFlags, 4);
                //}
                //deviceDriver.StopTest(GlobalUnit.g_TerminalVerifyFlags, 4);

                DeviceControl.SetPulseOutput(0x00, 0xff, 0.5f, 12, 0, 0.5f, 12, 0);


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
                   //4 0 11 

                #region 数据区初始化
                ResetTerimal(2);
                #endregion



                Thread.Sleep(3000);

                MessageAdd("台体输出脉冲信号。", EnumLogType.错误信息);

                //if (deviceDriver.m_strErrorBoardType == "CL188M")
                //{
                //    deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, 2, 1, 2160, 2f, 16);

                //    deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, 3, 1, 2160, 2f, 16);
                //}
                //else
                //{
                //    deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, int.Parse(PulseCount.ToString()), 1, 2160, 2f, 16);
                //}
                DeviceControl.SetPulseOutput(0x03, 0xff, 2, 0.5f, 120, 2, 0.5f, 120);
                Thread.Sleep(500);

                WaitTime("等待", 360);

                //m_int_ItemIndex = 1;

                for (byte p1 = 1; p1 <= 1; p1++)
                {
                    int intEnergy = 7200 * GroupTotal1Count;

                    MessageAdd("读总加组有功功率。", EnumLogType.错误信息);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 17, m_dic_RecData_F17, MaxWaitSeconds_Read485);
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        TempData[i].Resoult = "合格";
                    }
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = intEnergy.ToString();
                            if (TalkResult[i] == 0 && m_dic_RecData_F17[i].Length >= 4)
                            {

                                TempData[i].Data = GetData(m_dic_RecData_F17, i, 3, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F17, i, 3, EnumTerimalDataType.e_double)) - intEnergy) / intEnergy / 100);
                                if (Math.Abs((double.Parse(GetData(m_dic_RecData_F17, i, 3, EnumTerimalDataType.e_double)) - intEnergy) / intEnergy / 100) <= 2)
                                {
                                    TempData[i].Resoult = "合格";
                                    //ResultDictionary[""][i] = GetData(m_dic_RecData_F17, i, 3, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F17, i, 3, EnumTerimalDataType.e_double)) - intEnergy) / intEnergy / 100) + "|" + intEnergy.ToString();
                                }
                                else
                                {
                                    TempData[i].Resoult = "不合格";
                                    //ResultDictionary[""][i] = GetData(m_dic_RecData_F17, i, 3, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F17, i, 3, EnumTerimalDataType.e_double)) - intEnergy) / intEnergy / 100) + "|" + intEnergy.ToString() + "|误差超限2";
                                    TempData[i].Tips = "误差超限";
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
                    //RefUIData("总加组" + p1 + "有功功率");
                    AddItemsResoult("总加组" + p1 + "有功功率", TempData);

                }

                for (byte p1 = 3; p1 <= GroupTotal1Count + 2; p1++)
                {
                    MessageAdd("读当前总有功功率。", EnumLogType.错误信息);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 25, RecData, MaxWaitSeconds_Read485);

                    int intEnergy = 7200;
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        TempData[i].Resoult = "合格";
                    }
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = intEnergy.ToString();
                            if (TalkResult[i] == 0 && RecData[i].Length >= 9)
                            {
                                TempData[i].Data = GetData(RecData, i, 4, EnumTerimalDataType.e_string);

                                if (Math.Abs((double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_double)) * 1000 - intEnergy)) / 72 <= 2)
                                {
                                    TempData[i].Resoult = "合格";
                                    //ResultDictionary[""][i] = (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_string)) * 1000).ToString("F2") + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_double)) * 1000 - intEnergy) / 72) + "|" + intEnergy.ToString();
                                }
                                else
                                {
                                    TempData[i].Resoult = "不合格";
                                    //ResultDictionary[""][i] = (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_string)) * 1000).ToString("F2") + "," + string.Format("{0:0.00}", (double.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_double)) * 1000 - intEnergy) / 72) + "|" + intEnergy.ToString() + "|误差超限2";
                                    TempData[i].Tips = "误差超限";
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
                    AddItemsResoult("测量点" + p1 + "有功功率", TempData);

                }

                WaitTime("等待", 840);

                for (byte p1 = 3; p1 <= GroupTotal1Count + 2; p1++)
                {

                    MessageAdd("读当月正向有/无功最大需量及发生时间。", EnumLogType.错误信息);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 35, m_dic_RecData_F35, MaxWaitSeconds_Read485);


                    for (int i = 0; i < MeterNumber; i++)
                    {
                        TempData[i].Resoult = "合格";
                    }
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = "7.2";
                            if (TalkResult[i] == 0 && m_dic_RecData_F35[i].Length >= 25)
                            {
                                TempData[i].Data = GetData(RecData, i, 35, EnumTerimalDataType.e_string);

                                if (Math.Abs((double.Parse(GetData(m_dic_RecData_F35, i, 5, EnumTerimalDataType.e_double)) - 7.2) / 0.072) <= 2)
                                {
                                    TempData[i].Resoult = "合格";
                                    //ResultDictionary[""][i] = GetData(m_dic_RecData_F35, i, 5, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F35, i, 5, EnumTerimalDataType.e_double)) - 7.2) / 0.072) + "|7.2";
                                }
                                else
                                {
                                    TempData[i].Resoult = "不合格";
                                    //ResultDictionary[""][i] = GetData(RecData, i, 35, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F35, i, 5, EnumTerimalDataType.e_double)) - 7.2) / 0.072) + "|7.2|误差超限2";
                                    TempData[i].Tips = "误差超限";
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
                    AddItemsResoult("测量点" + p1 + "月最大需量", TempData);

                }
                for (byte p1 = 3; p1 <= PulseCount + 2; p1++)
                {
                    MessageAdd("读当前总有功电量。", EnumLogType.错误信息);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 33, RecData, MaxWaitSeconds_Write);

                    int intEnergy = 2160;
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        TempData[i].Resoult = "合格";
                    }
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = intEnergy.ToString();
                            if (TalkResult[i] == 0 && RecData[i].Length >= 9)
                            {
                                TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string);

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
                                //MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "无回复";
                            }
                        }
                    }
                    AddItemsResoult("测量点" + p1 + "电能量", TempData);

                }
                for (byte p1 = 1; p1 <= 1; p1++)
                {
                    MessageAdd("读总加有功电能量。", EnumLogType.错误信息);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 19, m_dic_RecData_F19, MaxWaitSeconds_Read485);

                    int intEnergy = 2160 * GroupTotal1Count;

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        TempData[i].Resoult = "合格";
                    }
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = intEnergy.ToString();
                            if (TalkResult[i] == 0 && m_dic_RecData_F19[i].Length >= 9)
                            {
                                TempData[i].Data = GetData(m_dic_RecData_F19, i, 4, EnumTerimalDataType.e_string);

                                if (Math.Abs((double.Parse(GetData(m_dic_RecData_F19, i, 4, EnumTerimalDataType.e_double)) - intEnergy)) <= 1)
                                {
                                    TempData[i].Resoult = "合格";
                                    //ResultDictionary[""][i] = GetData(m_dic_RecData_F19, i, 4, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F19, i, 4, EnumTerimalDataType.e_double)) - intEnergy)) + "|" + intEnergy.ToString();
                                }
                                else
                                {
                                    TempData[i].Resoult = "不合格";
                                    //ResultDictionary[""][i] = GetData(m_dic_RecData_F19, i, 4, EnumTerimalDataType.e_string) + "," + string.Format("{0:0.00}", (double.Parse(GetData(m_dic_RecData_F19, i, 4, EnumTerimalDataType.e_double)) - intEnergy)) + "|" + intEnergy.ToString() + "|误差超限1";
                                    TempData[i].Tips = "误差超限";
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
                    AddItemsResoult("总加" + p1 + "有功电能量", TempData);

                }


            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
