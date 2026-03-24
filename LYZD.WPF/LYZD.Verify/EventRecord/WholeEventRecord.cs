using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.EventRecord
{
    /// <summary>
    /// 全事件采集上报
    /// </summary>
    public class WholeEventRecord : VerifyBase
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
                StartVerify698();

                int ret = 0;
                ConnectLink(false);


                DateTime dtHappen = DateTime.Now.AddMinutes(-1);//记录当前时间，事件发生的时间一定比这个晚

                DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:50");
                DateTime dttmp2 = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:50:00");
                ControlVirtualMeter("Cmd,SetEventHappen," + dttmp2);
                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 16 81 00 00 00", "清空事件采集方案");
                ResetTerimal_698(2);
                ConnectLink2(false);
                SetTime_698(DateTime.Now, 0);
                SetData_698_No("07 01 0E 60 16 7F 00 01 01 02 05 11 01 02 02 11 00 01 04 52 30 00 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 01 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 02 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 03 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 5C 01 03 00 12 01 00 00 ", "下装事件采集方案1");
                SetData_698_No("07 01 0F 60 16 7F 00 01 01 02 05 11 02 02 02 11 00 01 04 52 30 04 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 05 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 06 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 07 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 5C 01 03 00 12 01 00 00  ", "下装事件采集方案2");
                SetData_698_No("07 01 10 60 16 7F 00 01 01 02 05 11 03 02 02 11 00 01 04 52 30 08 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 09 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 0A 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 0B 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 5C 01 03 01 12 01 00 00  ", "下装事件采集方案3");
                SetData_698_No("07 01 11 60 16 7F 00 01 01 02 05 11 04 02 02 11 00 01 04 52 30 0C 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 0D 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 0E 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 0F 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 5C 01 03 00 12 01 00 00 ", "下装事件采集方案4");
                SetData_698_No("07 01 12 60 16 7F 00 01 01 02 05 11 05 02 02 11 00 01 04 52 30 10 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 11 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 12 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 13 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 5C 01 03 00 12 01 00 00  ", "下装事件采集方案5");
                SetData_698_No("07 01 13 60 16 7F 00 01 01 02 05 11 06 02 02 11 00 01 04 52 30 14 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 15 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 16 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 17 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 5C 01 03 00 12 01 00 00  ", "下装事件采集方案6");
                SetData_698_No("07 01 14 60 16 7F 00 01 01 02 05 11 07 02 02 11 00 01 04 52 30 18 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 19 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 1A 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 1B 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 5C 01 03 00 12 01 00 00 ", "下装事件采集方案7");
                SetData_698_No("07 01 15 60 16 7F 00 01 01 02 05 11 08 02 02 11 00 01 04 52 30 1C 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 1D 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 1E 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 1F 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 5C 01 03 00 12 01 00 00  ", "下装事件采集方案8");
                SetData_698_No("07 01 16 60 16 7F 00 01 01 02 05 11 09 02 02 11 00 01 04 52 30 20 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 21 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 22 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 23 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 5C 01 03 00 12 01 00 00 ", "下装事件采集方案9");
                SetData_698_No("07 01 17 60 16 7F 00 01 01 02 05 11 0A 02 02 11 00 01 04 52 30 24 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 25 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 26 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 27 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 5C 01 03 00 12 01 00 00 ", "下装事件采集方案10");
                SetData_698_No("07 01 18 60 16 7F 00 01 01 02 05 11 0B 02 02 11 00 01 04 52 30 28 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 29 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 2A 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 2B 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 5C 01 03 00 12 01 00 00 ", "下装事件采集方案11");
                SetData_698_No("07 01 19 60 16 7F 00 01 01 02 05 11 0C 02 02 11 00 01 04 52 30 2C 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 2D 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 2E 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 52 30 2F 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 5C 01 03 00 12 01 00 00 ", "下装事件采集方案12");

                SetData_698_No("07 01 1A 60 12 7F 00 01 01 02 0C 11 01 54 01 00 0A 16 02 11 01 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 1C 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00  ", "下装采集任务1");
                SetData_698_No("07 01 1B 60 12 7F 00 01 01 02 0C 11 02 54 01 00 0A 16 02 11 02 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 1C 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00   ", "下装采集任务2");
                SetData_698_No("07 01 1C 60 12 7F 00 01 01 02 0C 11 03 54 01 00 0A 16 02 11 03 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 1C 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00   ", "下装采集任务3");
                SetData_698_No("07 01 1D 60 12 7F 00 01 01 02 0C 11 04 54 01 00 0A 16 02 11 04 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 1C 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00  ", "下装采集任务4");
                SetData_698_No("07 01 1E 60 12 7F 00 01 01 02 0C 11 05 54 01 00 0A 16 02 11 05 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 1C 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00   ", "下装采集任务5");
                SetData_698_No("07 01 1F 60 12 7F 00 01 01 02 0C 11 06 54 01 00 0A 16 02 11 06 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 1C 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00  ", "下装采集任务6");
                SetData_698_No("07 01 20 60 12 7F 00 01 01 02 0C 11 07 54 01 00 0A 16 02 11 07 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 1C 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00  ", "下装采集任务7");
                SetData_698_No("07 01 21 60 12 7F 00 01 01 02 0C 11 08 54 01 00 0A 16 02 11 08 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 1C 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00  ", "下装采集任务8");
                SetData_698_No("07 01 22 60 12 7F 00 01 01 02 0C 11 09 54 01 00 0A 16 02 11 09 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 1C 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00 ", "下装采集任务9");
                SetData_698_No("07 01 23 60 12 7F 00 01 01 02 0C 11 0A 54 01 00 0A 16 02 11 0A 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 1C 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00 ", "下装采集任务10");
                SetData_698_No("07 01 24 60 12 7F 00 01 01 02 0C 11 0B 54 01 00 0A 16 02 11 0B 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 1C 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00 ", "下装采集任务11");
                SetData_698_No("07 01 25 60 12 7F 00 01 01 02 0C 11 0C 54 01 00 0A 16 02 11 0C 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 1C 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00 ", "下装采集任务12");


                SetTime_698(dttmp, 0);

                WaitTime("等待", 360);

                MessageAdd("读取事件记录_电能表失压事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 28 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 00 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表失压事件", TempData);

                MessageAdd("读取事件记录_电能表欠压事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 29 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + "00 00 00 01 02 00 20 2A 02 00 01 30 01 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表欠压事件", TempData);

                MessageAdd("读取事件记录_电能表过压事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 2A 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 02 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表过压事件", TempData);

                MessageAdd("读取事件记录_电能表断相事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 2B 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + "00 00 00 01 02 00 20 2A 02 00 01 30 03 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表断相事件", TempData);

                MessageAdd("读取事件记录_电能表失流事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 2C 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 04 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1，1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表失流事件", TempData);

                MessageAdd("读取事件记录_电能表过流事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 2D 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 05 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表过流事件", TempData);

                MessageAdd("读取事件记录_电能表断流事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 2E 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 06 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表断流事件", TempData);

                MessageAdd("读取事件记录_电能表潮流反向事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 2F 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 07 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表潮流反向事件", TempData);

                MessageAdd("读取事件记录_电能表过载事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 30 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 08 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表过载事件", TempData);

                MessageAdd("读取事件记录_电能表正向有功需量超限事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 31 60 12 03 00 07" + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 09 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表正向有功需量超限事件", TempData);

                MessageAdd("读取事件记录_电能表反向有功需量超限事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 32 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 0A 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表反向有功需量超限事件", TempData);

                MessageAdd("读取事件记录_电能表无功需量超限事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 33 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 0B 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表无功需量超限事件", TempData);

                MessageAdd("读取事件记录_电能表功率因数超下限事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 34 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 0C 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表功率因数超下限事件", TempData);

                MessageAdd("读取事件记录_电能表全失压事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 35 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 0D 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表全失压事件", TempData);

                MessageAdd("读取事件记录_电能表辅助电源掉电事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 36 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 0E 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表辅助电源掉电事件", TempData);

                MessageAdd("读取事件记录_电能表电压逆相序事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = ("05 03 37 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 0F 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ").Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表电压逆相序事件", TempData);

                MessageAdd("读取事件记录_电能表电流逆相序事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 38 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 10 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表电流逆相序事件", TempData);

                MessageAdd("读取事件记录_电能表掉电事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 39 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 11 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("事件记录_电能表掉电事件", TempData);

                MessageAdd("读取事件记录_电能表编程事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 3A 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 12 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("编程事件", TempData);

                MessageAdd("读取事件记录_电能表清零事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 3B 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 13 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表清零事件", TempData);

                MessageAdd("读取事件记录_电能表需量清零事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 3C 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 14 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表需量清零事件", TempData);

                MessageAdd("读取事件记录_电能表事件清零事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 3D 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 15 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }
                AddItemsResoult("电能表事件清零事件", TempData);

                MessageAdd("读取事件记录_电能表校时事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 3E 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 16 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表校时事件", TempData);

                MessageAdd("读取事件记录_电能表时段表编程事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 3F 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 17 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表时段表编程事件", TempData);

                MessageAdd("读取事件记录_电能表时区表编程事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 00 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 18 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表时区表编程事件", TempData);

                MessageAdd("读取事件记录_电能表周休日编程事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 01 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 19 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表周休日编程事件", TempData);

                MessageAdd("读取事件记录_电能表结算日编程事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 02 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 1A 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表结算日编程事件", TempData);

                MessageAdd("读取事件记录_电能表开盖事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 03 60 12 03 00 07" + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 1B 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表开盖事件", TempData);

                MessageAdd("读取事件记录_电能表开端钮盒事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 04 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 1C 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表开端钮盒事件", TempData);

                MessageAdd("读取事件记录_电能表电压不平衡事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 05 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 1D 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表电压不平衡事件", TempData);

                MessageAdd("读取事件记录_电能表电流不平衡事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 06 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 1E 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表电流不平衡事件", TempData);

                MessageAdd("读取事件记录_电能表跳闸事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 07 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 1F 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表跳闸事件", TempData);

                MessageAdd("读取事件记录_电能表合闸事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 08 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 20 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表合闸事件", TempData);

                MessageAdd("读取事件记录_电能表节假日编程事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 09 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 21 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表节假日编程事件", TempData);

                MessageAdd("读取事件记录_电能表有功组合方式编程事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 0A 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + "00 00 00 01 02 00 20 2A 02 00 01 30 22 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表有功组合方式编程事件", TempData);

                MessageAdd("读取事件记录_电能表无功组合方式编程事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 0B 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 23 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表无功组合方式编程事件", TempData);

                MessageAdd("读取事件记录_电能表费率参数表编程事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 0C 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 24 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表费率参数表编程事件", TempData);

                MessageAdd("读取事件记录_电能表阶梯表编程事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 0D 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 25 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表阶梯表编程事件", TempData);
                MessageAdd("读取事件记录_电能表密钥更新事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 0E 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 26 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表密钥更新事件", TempData);

                MessageAdd("读取事件记录_电能表异常插卡事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 0F 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 27 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表异常插卡事件", TempData);

                MessageAdd("读取事件记录_电能表购电记录",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 10 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 28 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表购电记录", TempData);

                MessageAdd("读取事件记录_电能表退费记录",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 11 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 29 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表退费记录", TempData);

                MessageAdd("读取事件记录_电能表恒定磁场干扰事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 12 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 2A 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表恒定磁场干扰事件", TempData);


                MessageAdd("读取事件记录_电能表负荷开关误动作事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 13 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + "00 00 00 01 02 00 20 2A 02 00 01 30 2B 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表负荷开关误动作事件", TempData);

                MessageAdd("读取事件记录_电能表电源异常事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 14 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 2C 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表电源异常事件", TempData);

                MessageAdd("读取事件记录_电能表电流严重不平衡事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        //Talkers[i].Framer698.sAPDU = "05 03 28 60 12 03 00 07" + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 00 06 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");
                        Talkers[i].Framer698.sAPDU = "05 03 28 60 12 03 00 07" + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 2D 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表电流严重不平衡事件", TempData);

                MessageAdd("读取事件记录_电能表时钟故障事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 16 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 2E 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00 ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表时钟故障事件", TempData);

                MessageAdd("读取事件记录_电能表计量芯片故障事件",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 17 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2A 02 00 01 30 2F 02 00 03 20 22 02 00 20 1E 02 00 20 20 02 00 00  ".Replace(" ", "");

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].Tips = "2,1,1";
                        if (TalkResult[i] == 0)
                        {
                            if (GetData(RecData, i, 11, EnumTerimalDataType.e_string) != "2" || GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1" || GetData(RecData, i, 17, EnumTerimalDataType.e_string) != "1")
                                TempData[i].Resoult = "合格";
                            TempData[i].Data = "2,1,1"; // = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string);
                        }
                    }
                }

                AddItemsResoult("电能表计量芯片故障事件", TempData);

                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 16 81 00 00 00", "清空事件采集方案");


            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
