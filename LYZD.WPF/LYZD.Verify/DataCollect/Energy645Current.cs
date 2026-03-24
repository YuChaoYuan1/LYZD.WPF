using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.DataCollect
{
    /// <summary>
    /// 终端采集645表计数据
    /// </summary>
    public class Energy645Current : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "电表1正向有功", "电表1反向有功", "电表2正向有功", "电表2反向有功", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify698();

                //int ret = 0;
                ConnectLink(false);
                ControlVirtualMeter("Cmd,DLS,5000,4000,1000,1000,1000,1000");
                ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

                DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:50");
                SetTime_698(DateTime.Now, 0);
                ResetTerimal_698(2);
                ConnectLink2(false);

                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

                SetTime_698(DateTime.Now, 0);

                ControlVirtualMeter("Cmd,ProtocalType,1");

                SetData_698_No("07 01 0C 60 1E 7F 00 01 03 02 02 5B 01 50 04 02 00 01 00 10 02 00 02 03 02 02 01 01 09 04 05 06 01 01 01 00 02 02 01 00 01 00 02 01 09 00 02 02 5B 01 50 04 02 00 01 00 20 02 00 02 03 02 02 01 01 09 04 05 06 02 01 01 00 02 02 01 00 01 00 02 01 09 00 02 02 5B 01 50 04 02 00 01 20 21 02 00 02 03 02 02 01 01 09 04 05 06 00 01 01 00 02 02 01 00 01 00 02 01 09 00 00 ", "下发采集规则库");
                SetData_698_No("07 01 0D 60 00 7F 00 02 04 12 00 0A 02 0A 55 07 05 00 00 00 00 00 03 16 06 16 02 51 F2 01 02 01 09 02 00 00 11 04 11 00 16 01 12 08 98 12 00 0F 02 04 55 07 05 00 00 00 00 00 00 09 06 00 00 00 00 00 00 12 00 01 12 00 01 01 00 00 ", "下发645电表档案");

                 SetData_698_No("07 01 0E 60 00 7F 00 02 04 12 00 0B 02 0A 55 07 05 00 00 00 00 00 04 16 06 16 02 51 F2 01 02 01 09 02 00 00 11 04 11 00 16 01 12 08 98 12 00 0F 02 04 55 07 05 00 00 00 00 00 00 09 06 00 00 00 00 00 00 12 00 01 12 00 01 01 00 00 ", "下发645电表档案");
                SetData_698_No("07 01 0F 60 14 7F 00 01 01 02 06 11 01 12 01 00 02 02 11 01 11 01 01 01 5B 01 50 04 02 00 03 20 21 02 00 00 10 02 00 00 20 02 00 5C 03 02 07 05 00 00 00 00 00 03 07 05 00 00 00 00 00 04 16 02 00  ", "下装普通采集方案");
                SetData_698_No("07 01 10 60 12 7F 00 01 01 02 0C 11 01 54 03 00 01 16 01 11 01 1C 07 E1 07 1C 00 00 00 1C 08 33 09 09 09 09 09 54 01 00 02 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00 ", "下装采集任务");

                SetTime_698(dttmp, 0);
                WaitTime("等待", 240);

                MessageAdd("读取表计485数据", EnumLogType.提示信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 12 60 12 03 00 05 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 03 02 07 05 00 00 00 00 00 03 07 05 00 00 00 00 00 04 05 00 20 2A 02 00 00 60 40 02 00 00 60 41 02 00 00 60 42 02 00 01 50 04 02 00 03 20 21 02 00 00 10 02 00 00 20 02 00 00 ";
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
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
                            string sTmp = GetData(RecData, i, 20, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 21, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 22, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 23, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 24, EnumTerimalDataType.e_string);
                            if (sTmp != "500000,125000,125000,125000,125000")
                                TempData[i].Resoult = "不合格";
                            TempData[i].Data = sTmp;
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("电表1正向有功", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "400000,100000,100000,100000,100000";
                        if (TalkResult[i] == 0)
                        {
                            string sTmp = GetData(RecData, i, 25, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 26, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 27, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 28, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 29, EnumTerimalDataType.e_string);
                            if (sTmp != "400000,100000,100000,100000,100000")
                                TempData[i].Resoult = "不合格";
                            TempData[i].Data = sTmp;
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("电表1反向有功", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "500000,125000,125000,125000,125000";
                        if (TalkResult[i] == 0)
                        {
                            string sTmp = GetData(RecData, i, 35, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 36, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 37, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 38, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 39, EnumTerimalDataType.e_string);
                            if (sTmp != "500000,125000,125000,125000,125000")
                                TempData[i].Resoult = "不合格";
                            TempData[i].Data = sTmp;
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("电表2正向有功", TempData);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "400000,100000,100000,100000,100000";
                        if (TalkResult[i] == 0)
                        {
                            string sTmp = GetData(RecData, i, 40, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 41, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 42, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 43, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 44, EnumTerimalDataType.e_string);
                            if (sTmp != "400000,100000,100000,100000,100000")
                                TempData[i].Resoult = "不合格";
                            TempData[i].Data = sTmp;
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                            TempData[i].Tips = "无回复";
                        }
                    }
                }
                AddItemsResoult("电表2反向有功", TempData);

                ControlVirtualMeter("Cmd,ProtocalType,2");

                SetData_698_No("07 01 13 60 00 83 00 12 00 0A 00  ", "删除采集档案");
                SetData_698_No("07 01 14 60 00 83 00 12 00 0B 00  ", "删除采集档案");


            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
