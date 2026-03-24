using LYZD.Core.Enum;
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
    public class CurveData : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "读取采集任务监控数据", "电表1_1点数据(正向有功电能示值)","电表1_1点数据(反向有功电能示值)", "电表2_1点数据(正向有功电能示值)",
 "电表2_1点数据(反向有功电能示值)", "电表1_2点数据(正向有功电能示值)","电表1_2点数据(反向有功电能示值)","电表2_2点数据(正向有功电能示值)","电表2_2点数据(反向有功电能示值)",
 "电表1_3点数据(正向有功电能示值)", "电表1_3点数据(正向有功电能示值)","电表2_3点数据(正向有功电能示值)","电表2_3点数据(反向有功电能示值)","电表1_4点数据(正向有功电能示值)",
 "电表1_4点数据(反向有功电能示值)", "电表2_4点数据(正向有功电能示值)","电表2_4点数据(反向有功电能示值)","结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify698();

                int ret = 0;
                ConnectLink(false);

                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

                

                ControlVirtualMeter("Cmd,DLS,5000,4000,1000,1000,1000,1000");
                ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");

                WaitTime("打开模拟表采集数据开关，", 2);

                MessageAdd("打开模拟表采集数据开关", EnumLogType.提示与流程信息, true);
                ControlVirtualMeter("MOP");

                DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:50");
                SetData_698_No("07 01 29 60 14 7F 00 01 01 02 06 11 01 12 01 00 02 02 11 03 54 01 00 0F 01 01 5B 01 50 02 02 00 03 20 21 02 00 00 10 02 00 00 20 02 00 5C 01 16 06 00", "下装普通采集方案");
                SetData_698_No("07 01 2A 60 12 7F 00 01 01 02 0C 11 01 54 03 00 01 16 01 11 01 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 1C 08 33 09 09 09 09 09 54 00 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00", "下装采集任务");


                SetTime_698(dttmp, 0);

                WaitTime("等待延时，", 480);
                MessageAdd("关闭模拟表采集数据开关", EnumLogType.提示与流程信息, true);
                ControlVirtualMeter("MCL");

                WaitTime("关闭模拟表采集数据开关", 2);
                MessageAdd("读取采集任务监控数据",EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = " 05 03 2C 60 34 02 00 01 60 35 02 01 11 01 00 00  ".Replace(" ", "");
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 10, EnumTerimalDataType.e_string); ;
                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
                AddItemsResoult("读取采集任务监控数据", TempData);


                for (int j = 0; j < 4; j++)
                {
                    MessageAdd("读取" + (j + 1) + "曲线数据",EnumLogType.流程信息, true);
                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            Talkers[i].Framer698.sAPDU = ("05 03 2d 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10 - 86400 + 3600 * j), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(3550 - 86400 + 3600 * j), false) + " 01 00 0f 01 03 00 60 42 02 00 00 20 2a 02 00 01 50 02 02 00 03 20 21 02 00 00 10 02 00 00 20 02 00 00").Replace(" ", "");
                            setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//代理读取电能量类数据
                        }
                    }
                    TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = "500000,125000,125000,125000,125000";
                            if (TalkResult[i] == 0)
                            {
                                string sTmp = GetData(RecData, i, 16, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 18, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 19, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 20, EnumTerimalDataType.e_string);
                                if (sTmp != "500000,125000,125000,125000,125000")
                                    TempData[i].Resoult = "不合格";
                                TempData[i].Data = sTmp;
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";
                            }

                        }
                    }
                    AddItemsResoult("电表1_" + (j+1) + "点数据(正向有功电能示值)", TempData);

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = "400000,100000,100000,100000,100000";
                            if (TalkResult[i] == 0)
                            {
                                string sTmp = GetData(RecData, i, 21, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 22, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 23, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 24, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 25, EnumTerimalDataType.e_string);
                                if (sTmp != "400000,100000,100000,100000,100000")
                                    TempData[i].Resoult = "不合格";
                                TempData[i].Data = sTmp;
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";
                            }

                        }
                    }
                    AddItemsResoult("电表1_" + (j+1) + "点数据(反向有功电能示值)", TempData);

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = "500000,125000,125000,125000,125000";
                            if (TalkResult[i] == 0)
                            {
                                string sTmp = GetData(RecData, i, 68, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 69, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 70, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 71, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 72, EnumTerimalDataType.e_string);
                                if (sTmp != "500000,125000,125000,125000,125000")
                                    TempData[i].Resoult = "不合格";
                                TempData[i].Data = sTmp;
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";
                            }

                        }
                    }
                    AddItemsResoult("电表2_" + (j+1) + "点数据(正向有功电能示值)", TempData);

                    for (int i = 0; i < MeterNumber; i++)
                    {
                        if (meterInfo[i].YaoJianYn)
                        {
                            TempData[i].StdData = "400000,100000,100000,100000,100000";
                            if (TalkResult[i] == 0)
                            {
                                string sTmp = GetData(RecData, i, 73, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 74, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 75, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 76, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 77, EnumTerimalDataType.e_string);
                                if (sTmp != "400000,100000,100000,100000,100000")
                                    TempData[i].Resoult = "不合格";
                                TempData[i].Data = sTmp;
                            }
                            else
                            {
                                TempData[i].Tips = "无回复";
                                TempData[i].Resoult = "不合格";
                            }

                        }
                    }
                    AddItemsResoult("电表2_" + (j+1) + "点数据(反向有功电能示值)", TempData);
                    MessageAdd("关闭模拟表采集数据开关", EnumLogType.提示与流程信息, true);
                    ControlVirtualMeter("MCL");

                    WaitTime("关闭模拟表采集数据开关", 2);
                }


            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
