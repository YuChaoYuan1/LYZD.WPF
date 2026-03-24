using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.DataCollect
{
    /// <summary>
    /// 分时段电能量数据存储
    /// </summary>
    public class PeriodStorage_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "测量点3正向有功总电能示值", "测量点3费率1正向有功电能示值", "测量点3费率2正向有功电能示值", "测量点3费率3正向有功电能示值", "测量点3费率4正向有功电能示值", "测量点4正向有功总电能示值", "测量点4费率1正向有功电能示值", "测量点4费率2正向有功电能示值", "测量点4费率3正向有功电能示值", "测量点4费率4正向有功电能示值", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();
                byte GroupTotalCount = VerifyConfig.GroupTotalCount;

                byte GroupTotal1Count = VerifyConfig.GroupTotal1Count;

                byte GroupTotal2Count = VerifyConfig.GroupTotal2Count;

                string[] GroupTotal1Pn = VerifyConfig.GroupTotal1Pn.Split('|');

                string[] GroupTotal2Pn = VerifyConfig.GroupTotal2Pn.Split('|');

                byte PulseCount = VerifyConfig.PulseCount;


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
                DeviceControl.SetPulseOutput(0x00, 0xff, 1f, 8, 60, 1f, 8, 60);

                ResetTerimal(2);


                for (int i = 0; i < 4; i++)
                {
                    // 终端对时
                    SetTime(Convert.ToDateTime(DateTime.Now.ToString("yy-MM-dd") + " " + ((i * 6) + 0) + ":15:0"), 0);
                    MessageAdd("台体输出脉冲信号3分钟250个。",EnumLogType.流程信息);

                    //if (deviceDriver.m_strErrorBoardType == "CL188M")
                    //{
                    //    deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, 2, 1, 250, 1.39f, 12);
                    //    if (PulseCount > 1)
                    //        deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, 3, 1, 250, 1.39f, 12);
                    //}
                    //else
                    //{
                    //    deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, int.Parse(PulseCount.ToString()), 1, 250, 1.39f, 12);
                    //}
                    DeviceControl.SetPulseOutput(0x03, 0xff, 1.39f, 12, 250, 1.39f, 12, 250);
                    WaitTime("等待", 300);
                }



                for (byte p1 = 3; p1 <= PulseCount + 2; p1++)
                {
                    MessageAdd("读当前正向有/无功电能示值、一/四象限无功电能示值。", EnumLogType.错误信息);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 33, RecData, MaxWaitSeconds_Write);
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = "1";
                            if (TalkResult[i] == 0)
                            {
                                TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                                if (Math.Abs(float.Parse(TempData[i].Data) - 1) <= 0.1)
                                {
                                    //TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string) + "|1";
                                }
                                else
                                {
                                    TempData[i].Tips = "值不匹配！";
                                    TempData[i].Resoult = "不合格";
                                }
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                    }

                    AddItemsResoult("测量点" + p1 + "正向有功总电能示值", TempData);

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = "0.25";
                            if (TalkResult[i] == 0)
                            {
                                TempData[i].Data = GetData(RecData, i, 6, EnumTerimalDataType.e_string);
                                if (Math.Abs(float.Parse(ResultDictionary[""][i]) - 0.25) <= 0.02)
                                {
                                    //ResultDictionary[""][i] = GetData(RecData, i, 6, EnumTerimalDataType.e_string) + "|0.25";
                                }
                                else
                                {
                                    TempData[i].Tips = "值不匹配！";
                                    TempData[i].Resoult = "不合格";
                                }
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                    }
                    AddItemsResoult("测量点" + p1 + "费率1正向有功电能示值", TempData);

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = "0.25";
                            if (TalkResult[i] == 0)
                            {
                                TempData[i].Data = GetData(RecData, i, 7, EnumTerimalDataType.e_string);
                                if (Math.Abs(float.Parse(ResultDictionary[""][i]) - 0.25) <= 0.02)
                                {
                                    //ResultDictionary[""][i] = GetData(RecData, i, 7, EnumTerimalDataType.e_string) + "|0.25";
                                }
                                else
                                {
                                    TempData[i].Tips = "值不匹配！";
                                    TempData[i].Resoult = "不合格";
                                }
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                    }
                    AddItemsResoult("测量点" + p1 + "费率2正向有功电能示值", TempData);


                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = "0.25";
                            if (TalkResult[i] == 0)
                            {
                                TempData[i].Data = GetData(RecData, i, 8, EnumTerimalDataType.e_string);
                                if (Math.Abs(float.Parse(ResultDictionary[""][i]) - 0.25) <= 0.02)
                                {
                                    //ResultDictionary[""][i] = GetData(RecData, i, 8, EnumTerimalDataType.e_string) + "|0.25";
                                }
                                else
                                {
                                    TempData[i].Tips = "值不匹配！";
                                    TempData[i].Resoult = "不合格";
                                }
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";
                            }
                        }
                    }
                    AddItemsResoult("测量点" + p1 + "费率3正向有功电能示值", TempData);



                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = "0.25";
                            if (TalkResult[i] == 0)
                            {
                                TempData[i].Data = GetData(RecData, i, 9, EnumTerimalDataType.e_string);
                                if (Math.Abs(float.Parse(ResultDictionary[""][i]) - 0.25) <= 0.02)
                                {
                                    //ResultDictionary[""][i] = GetData(RecData, i, 9, EnumTerimalDataType.e_string) + "|0.25";
                                }
                                else
                                {
                                    TempData[i].Tips = "值不匹配！";
                                    TempData[i].Resoult = "不合格";
                                }
                            }
                            else
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "无回复";
                            }
                        }
                    }
                    AddItemsResoult("测量点" + p1 + "费率4正向有功电能示值", TempData);

                }

            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
