using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.TaskReporting
{
    /// <summary>
    /// 定时发送1类数据
    /// </summary>
    public class TimingOneData_376 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "上报数据", "定时发送一类数据1", "定时发送一类数据2", "结论" };

            return true;
        }
        public override void Verify()
        {

            if (!(TerminalChannelType == Cus_EmChannelType.ChannelEther || TerminalChannelType == Cus_EmChannelType.ChannelGPRS))
            {
                MessageAdd("请使用以太网或者GPRS测试", EnumLogType.错误信息);
                return;
            }


            try
            {
                base.Verify();
                StartVerify();

                WriteData("定时上报1类数据任务设置", "01" + UsefulMethods.ConvertListToString(Talkers[0].Framer.setData1(DateTime.Now)) + "01020000020000000800", 4, 1, 65);

                DateTime dtHappen = DateTime.Now.AddMinutes(-1);//记录当前时间，事件发生的时间一定比这个晚

                WriteData("设置终端参数 F67.定时发送1类数据任务启动/停止设置 ", "55", 4, 1, 67);

                WriteData("设置终端参数 F68.定时发送2类数据任务启动/停止设置", "AA", 4, 1, 68);

                WriteData("允许终端主动上报", "", 5, 0, 29);

                for (int i = 0; i < MeterNumber; i++)
                {
                    Talkers[i].ReceiveData = "";
                    Talkers[i].AFn = 12;
                    Talkers[i].Fn = 2;
                }

                TalkResult = TerminalProtocalAdapter.Instance.WaitEventReport(RecData, 180);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "主动上报";
                        if (TalkResult[i] == 0)
                        {
                            if (RecData[i].Length >= 7)
                            {
                                MessageAdd("终端" + (i + 1) + "产生主动上报！", EnumLogType.错误信息);
                                TempData[i].Resoult = "合格";
                                TempData[i].Data = Talkers[i].ReceiveData;

                            }
                            else
                            {
                                MessageAdd("终端" + (i + 1) + "未产生！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "长度不符合";
                            }
                        }
                        else
                        {
                            MessageAdd("终端无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无上报";
                        }
                    }
                }
                AddItemsResoult("上报数据", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "Pn = 0,Fn = 2";
                        if (TalkResult[i] == 0)
                        {
                            if (RecData[i].Length >= 7)
                            {
                                if (GetRemoveNumData(RecData[i][1]) == "Pn = 0" && GetRemoveNumData(RecData[i][2]).Contains("Fn = 2"))
                                {
                                    MessageAdd("终端" + (i + 1) + "产生主动上报！", EnumLogType.错误信息);
                                    TempData[i].Resoult = "合格";
                                    TempData[i].Data = "Pn = 0,Fn = 2";
                                }
                                else
                                {
                                    MessageAdd("终端" + (i + 1) + "产生主动上报！", EnumLogType.错误信息);
                                    TempData[i].Resoult = "不合格";
                                    TempData[i].Data = GetRemoveNumData(RecData[i][1]) + "," + GetRemoveNumData(RecData[i][2]);
                                }
                            }
                            else
                            {
                                MessageAdd("终端" + (i + 1) + "未产生！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "长度不符合";
                            }
                        }
                        else
                        {
                            MessageAdd("终端无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无上报";
                        }
                    }
                }
                AddItemsResoult("定时发送一类数据1".ToString(), TempData);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "Pn = 0,Fn = 4";
                        if (TalkResult[i] == 0)
                        {
                            if (RecData[i].Length >= 7)
                            {
                                if (GetRemoveNumData(RecData[i][4]) == "Pn = 0" && GetRemoveNumData(RecData[i][5]).Contains("Fn = 4"))
                                {
                                    MessageAdd("终端" + (i + 1) + "产生主动上报！", EnumLogType.错误信息);
                                    TempData[i].Resoult = "合格";
                                    TempData[i].Data = "Pn = 0,Fn = 4";
                                }
                                else
                                {
                                    MessageAdd("终端" + (i + 1) + "产生主动上报！", EnumLogType.错误信息);
                                    TempData[i].Resoult = "不合格";
                                    TempData[i].Data = GetRemoveNumData(RecData[i][4]) + "," + GetRemoveNumData(RecData[i][5]);
                                }
                            }
                            else
                            {
                                MessageAdd("终端" + (i + 1) + "未产生！", EnumLogType.错误信息);
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "长度不符合";
                            }
                        }
                        else
                        {
                            MessageAdd("终端无回复！", EnumLogType.错误信息);
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无上报";
                        }
                    }
                }

                AddItemsResoult("定时发送一类数据2".ToString(), TempData);

                WriteData("设置终端参数 F67.定时发送1类数据任务启动/停止设置 ", "AA", 4, 1, 67);

                WriteData("禁止终端主动上报", "", 5, 0, 37);


            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
