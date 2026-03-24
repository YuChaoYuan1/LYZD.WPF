using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.DataCollect
{
    /// <summary>
    /// 电能表当前数据(错误MAC)
    /// </summary>
    public class EnergyCurrentErrMac : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "错误MAC电表1当前数据(电压)", "错误MAC电表1当前数据(电流)", "错误MAC电表2当前数据(电压)", "错误MAC电表2当前数据(电流)", "结论" };
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
                ControlVirtualMeter("Cmd,ErrMac");
                ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
                ControlVirtualMeter("Cmd,DLS,5000,4000,1000,1000,1000,1000");

                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

                DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 12:00:00");
                SetTime_698(dttmp, 0);
                ResetTerimal_698(2);
                ConnectLink2(false);
                SetData_698_No("07 01 14 60 14 7F 00 01 01 02 06 11 01 12 01 00 02 02 11 00 00 01 02 5B 00 20 00 02 00 5B 00 20 01 02 00 5C 01 16 02 00", "下装普通采集方案");
                SetData_698_No("07 01 15 60 12 7F 00 01 01 02 0C 11 01 54 01 00 05 16 01 11 01 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(43200), false) + " 1C 08 33 09 09 09 09 09 54 01 00 02 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00", "下装采集任务");

                dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:50");
                SetTime_698(dttmp, 0);

                WaitTime("等待延时，", 240);

                MessageAdd("读取电表当前数据",EnumLogType.流程信息);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = "05 03 18 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00  01 03 00 20 2a 02 00  00 20 00 02 00 00 20 01 02 00 00 ".Replace(" ", "");
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//代理读取电能量类数据
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "2200,2200,2200";
                        if (TalkResult[i] == 0)
                        {
                            string sTmp = GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 12, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string);
                            TempData[i].Data = sTmp;

                            if (sTmp == "2200,2200,2200")
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
                AddItemsResoult("错误MAC电表1当前数据(电压)", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "5000,5000,5000";
                        if (TalkResult[i] == 0)
                        {
                            string sTmp = GetData(RecData, i, 14, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 15, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 16, EnumTerimalDataType.e_string);
                            TempData[i].Data = sTmp;

                            if (sTmp == "5000,5000,5000")
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

                AddItemsResoult("错误MAC电表1当前数据(电流)", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "2200,2200,2200";
                        if (TalkResult[i] == 0)
                        {
                            string sTmp = GetData(RecData, i, 18, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 19, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 20, EnumTerimalDataType.e_string);
                            TempData[i].Data = sTmp;

                            if (sTmp == "2200,2200,2200")
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
                AddItemsResoult("错误MAC电表2当前数据(电压)", TempData);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "5000,5000,5000";
                        if (TalkResult[i] == 0)
                        {
                            string sTmp = GetData(RecData, i, 21, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 22, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 23, EnumTerimalDataType.e_string);
                            TempData[i].Data = sTmp;

                            if (sTmp == "5000,5000,5000")
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
                AddItemsResoult("错误MAC电表2当前数据(电流)", TempData);

            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
