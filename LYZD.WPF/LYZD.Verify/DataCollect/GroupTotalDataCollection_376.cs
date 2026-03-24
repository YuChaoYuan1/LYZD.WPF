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
    /// 总加组日和月电量召集
    /// </summary>
    public class GroupTotalDataCollection_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "当日总加1有功电能量", "当月总加1有功电能量", "结论" };
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


                DeviceControl.ContnrRemoteSignalingStatusOutput(0xff, false, false, false, false, false, false);
                //StopWcb(,0xff)


                byte PulseCount = VerifyConfig.PulseCount;


                MessageAdd("设置终端参数 F12.终端状态量输入参数...", EnumLogType.错误信息);
                SetData = Core.Function.UsefulMethods.ConvertStringToBytes("0000");
                TalkResult = TerminalProtocalAdapter.Instance.WriteData(4, 0, 12, SetData, RecData, MaxWaitSeconds_Write);

                #region 数据区初始化
                ResetTerimal(2);
                #endregion


                MessageAdd("终端对时到2015-11-30 23:59:00", EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 31, Convert.ToDateTime("2015-12-1 23:59:00").ToString(), RecData, MaxWaitSeconds_Write);
                WaitTime("等待", 80);

                // 终端对时
                MessageAdd("终端对时到2015-12-1 20:52:00", EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 31, Convert.ToDateTime("2015-12-1 20:52:00").ToString(), RecData, MaxWaitSeconds_Write);
                WaitTime("等待", 30);

                MessageAdd("台体输出脉冲信号。", EnumLogType.错误信息);

                //if (deviceDriver.m_strErrorBoardType == "CL188M")
                //{
                //    deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, 2, 1, 30, 0.5f, 4);

                //    deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, 3, 1, 30, 0.5f, 4);
                //}
                //else
                //{
                //    deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, int.Parse(PulseCount.ToString()), 1, 30, 0.5f, 4);
                //}

                DeviceControl.SetPulseOutput(0x03, 0xff, 0.5f, 4, 30, 0.5f, 4, 30);
                Thread.Sleep(500);

                WaitTime("等待", 180);

                MessageAdd("终端对时到2015-12-1 23:59:00", EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 31, Convert.ToDateTime("2015-12-1 23:59:00").ToString(), RecData, MaxWaitSeconds_Write);
                WaitTime("等待", 80);


                // 终端对时
                MessageAdd("终端对时到2015-12-2 20:52:00", EnumLogType.错误信息);
                TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 31, Convert.ToDateTime("2015-12-2 20:52:00").ToString(), RecData, MaxWaitSeconds_Write);
                WaitTime("等待", 30);

                MessageAdd("台体输出脉冲信号。", EnumLogType.错误信息);
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
                DeviceControl.SetPulseOutput(0x00, 0xff, 0.5f, 4, 30, 0.5f, 4, 30);

                Thread.Sleep(500);
                //if (deviceDriver.m_strErrorBoardType == "CL188M")
                //{
                //    deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, 2, 1, 60, 1f, 8);

                //    deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, 3, 1, 60, 1f, 8);
                //}
                //else
                //{
                //    deviceDriver.StartPuleslOutput(GlobalUnit.g_TerminalVerifyFlags, int.Parse(PulseCount.ToString()), 1, 60, 1f, 8);
                //}
                DeviceControl.SetPulseOutput(0x03, 0xff, 1f, 8, 60, 1f, 8, 60);
                Thread.Sleep(500);
                WaitTime("等待", 180);

                //m_int_ItemIndex = 1;
                for (byte p1 = 1; p1 <= 1; p1++)
                {

                    MessageAdd("读当月正向有/无功最大需量及发生时间。", EnumLogType.错误信息);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 19, RecData, MaxWaitSeconds_Write);

                    int intEnergy = 60 * GroupTotal1Count;
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = intEnergy.ToString();
                            if (TalkResult[i] == 0)
                            {
                                TempData[i].Data = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                                if (int.Parse(TempData[i].Data) - intEnergy < 2)
                                {
                                    //TempData[i].Data = GetData(RecData, i, 4, EnumTerimalDataType.e_string) + "|" + intEnergy.ToString();
                                }
                                else
                                {
                                    TempData[i].Tips = "值不匹配！";
                                    TempData[i].Resoult = "不合格";
                                    //TempData[i].Data = RecData[i][4].ToString() + "|" + intEnergy.ToString() + "|无回复";
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
                    AddItemsResoult("当日总加" + p1 + "有功电能量", TempData);

                }

                for (byte p1 = 1; p1 <= GroupTotalCount; p1++)
                {

                    MessageAdd("读总加有功电能量。", EnumLogType.错误信息);
                    TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, p1, 21, RecData, MaxWaitSeconds_Write);

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        TempData[i].Resoult = "合格";
                    }
                    int intEnergy = 90 * GroupTotal1Count;
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = intEnergy.ToString();
                            if (TalkResult[i] == 0)
                            {
                                TempData[i].Data = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                                if (int.Parse(TempData[i].Data) - intEnergy < 4)
                                {
                                    //TempData[i].Data = GetData(RecData, i, 4, EnumTerimalDataType.e_string) + "|" + intEnergy.ToString();
                                }
                                else
                                {
                                    TempData[i].Tips = "值不匹配！";
                                    TempData[i].Resoult = "不合格";
                                }
                            }
                            else
                            {
                                //MessageAdd("终端" + (i + 1) + "无回复！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = intEnergy.ToString() + "无回复"; ;
                            }
                        }
                    }
                    AddItemsResoult("当月总加" + p1 + "有功电能量", TempData);

                }

            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
