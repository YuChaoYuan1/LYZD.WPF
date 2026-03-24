using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.DataCollect
{
    /// <summary>
    /// 电能表当前数据(2路)
    /// </summary>
    public class EnergyCurrent02 : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "采集任务监控数据", "电表3当前数据(有功电能示值)", "电表3当前数据(电压)", "电表3当前数据(电流)", "结论" };
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

                ControlVirtualMeter("Cmd,Set2,221,221,221,6,6,6,0,0,0,1,0," + (0) + "," + MeterNumber);
                ControlVirtualMeter("Cmd,DLS2,5001,4000,1000,1000,1000,1000," + (0) + "," + MeterNumber);
                SetData_698_No("07013760007F000204120003020A5507050000000000031606160351F2010202090600000000000011041100160312089812000F02045507050000000000000906000000000000120001120001010000", "下发一块采集档案");


                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

                DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 12:00:00");
                SetTime_698(dttmp, 0);

                SetData_698_No("07 01 14 60 14 7F 00 01 01 02 06 11 01 12 01 00 02 02 11 00 00 01 03 5B 00 00 10 02 00 5B 00 20 00 02 00 5B 00 20 01 02 00 5C 01 16 02 00", "下装普通采集方案");
                SetData_698_No("07 01 15 60 12 7F 00 01 01 02 0C 11 01 54 01 00 05 16 01 11 01 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(43200), false) + " 1C 08 33 09 09 09 09 09 54 01 00 02 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00", "下装采集任务");

                dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:50");
                SetTime_698(dttmp, 0);

                WaitTime("等待延时，", 240);

                MessageAdd("读取采集任务监控数据",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = " 05 03 17 60 34 02 00 01 60 35 02 01 11 01 00 00 ".Replace(" ", "");
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
                AddItemsResoult("采集任务监控数据", TempData);

                MessageAdd("读取电表当前数据",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 18 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 03 01 07 05 00 00 00 00 00 03  04 00 20 2A 02 00 00 00 10 02 00 00 20 00 02 00 00 20 01 02 00 00  ".Replace(" ", "");
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//代理读取电能量类数据
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "500100,125025,125025,125025,125025";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 12, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 14, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 15, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 16, EnumTerimalDataType.e_string);
                            string sTmp = GetData(RecData, i, 12, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 14, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 15, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 16, EnumTerimalDataType.e_string);
                            if (sTmp != "500100,125025,125025,125025,125025")
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

                AddItemsResoult("电表3当前数据(有功电能示值)", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "2210,2210,2210";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 17, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 18, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 19, EnumTerimalDataType.e_string);
                            string sTmp = GetData(RecData, i, 17, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 18, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 19, EnumTerimalDataType.e_string);
                            if (sTmp != "2210,2210,2210")
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
                AddItemsResoult("电表3当前数据(电压)", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "6000,6000,6000";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 20, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 21, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 22, EnumTerimalDataType.e_string);
                            string sTmp = GetData(RecData, i, 20, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 21, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 22, EnumTerimalDataType.e_string);
                            if (sTmp != "6000,6000,6000")
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
                AddItemsResoult("电表3当前数据(电流)", TempData);
                SetData_698_No("07 01 13 60 00 83 00 12 00 03 00  ", "删除采集档案");
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
