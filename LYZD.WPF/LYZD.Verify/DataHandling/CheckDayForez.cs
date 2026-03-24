using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LYZD.Verify.DataHandling
{
    public class CheckDayForez : VerifyBase
    {
        public override void Verify()
        {
            base.Verify();
            StartVerify698();

            ConnectLink(false);
            ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
            ControlVirtualMeter("Cmd,DLS,5000,4000,1000,1000,1000,1000");
            SetTime_698(DateTime.Now, 0);
            SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
            SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

            DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 02:00:00");
            SetTime_698(dttmp, 0);
            SetData_698_No("07 01 33 60 14 7F 00 01 01 02 06 11 01 12 01 00 02 02 11 02 00 01 01 5B 01 50 04 02 00 03 20 21 02 00 00 10 02 00 00 20 02 00 5C 01 16 03 00", "下装普通采集方案");
            SetData_698_No("07 01 34 60 12 7F 00 01 01 02 0C 11 01 54 03 00 01 16 01 11 01 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp.AddSeconds(-7200), false) + " 1C 08 33 09 09 09 09 09 54 01 00 02 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00", "下装采集任务");

            MessageAdd("打开模拟表采集数据开关", EnumLogType.提示与流程信息, true);

            ControlVirtualMeter("MOP");
            WaitTime("延时，", 240);
            ControlVirtualMeter("MCL");
            Thread.Sleep(100);
            MessageAdd("关闭模拟表采集数据开关", EnumLogType.提示与流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.sAPDU = " 05 03 35 60 34 02 00 01 60 35 02 01 11 01 00 00 ".Replace(" ", "");
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
                        TempData[i].Data = GetData(RecData, i, 10, EnumTerimalDataType.e_string) + "_" + GetData(RecData, i, 13, EnumTerimalDataType.e_string);
                    }
                    else
                    {
                        TempData[i].Tips = "无回复";
                        TempData[i].Resoult = "不合格";
                    }

                }
            }
            AddItemsResoult("读取采集任务监控数据结算日冻结", TempData);

            dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 02:00:00").AddMinutes(-121);

            MessageAdd("读取电能表数据", EnumLogType.提示信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {

                    Talkers[i].Framer698.sAPDU = ("05 03 36 60 12 03 00 05 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp, false) + " 01 05 00 20 2A 02 00 00 60 40 02 00 00 60 41 02 00 00 60 42 02 00 01 50 04 02 00 03 20 21 02 00 00 10 02 00 00 20 02 00 00").Replace(" ", "");
                    //Talkers[i].Framer698.sAPDU = "05 03 12 60 12 03 00 05 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp, false) + " 03 02 07 05 00 00 00 00 00 01 07 05 90 00 00 00 00 02 05 00 20 2A 02 00 00 60 40 02 00 00 60 41 02 00 00 60 42 02 00 01 50 04 02 00 03 20 21 02 00 00 10 02 00 00 20 02 00 00 ";

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
                        //TempData[i].Data = GetData(RecData, i, 20, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 21, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 22, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 23, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 24, EnumTerimalDataType.e_string);
                        string sTmp = GetData(RecData, i, 20, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 21, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 22, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 23, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 24, EnumTerimalDataType.e_string);
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
            AddItemsResoult("电表1数据(有功电能示值)", TempData);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "400000,100000,100000,100000,100000";
                    if (TalkResult[i] == 0)
                    {
                        //TempData[i].Data = GetData(RecData, i, 25, EnumTerimalDataType.e_string) + ", " + GetData(RecData, i, 26, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 27, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 28, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 29, EnumTerimalDataType.e_string);
                        string sTmp = GetData(RecData, i, 25, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 26, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 27, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 28, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 29, EnumTerimalDataType.e_string);
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
            AddItemsResoult("电表1数据(反向有功电能)", TempData);


            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "500000,125000,125000,125000,125000";
                    if (TalkResult[i] == 0)
                    {
                        //TempData[i].Data = GetData(RecData, i, 35, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 36, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 37, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 38, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 39, EnumTerimalDataType.e_string);
                        string sTmp = GetData(RecData, i, 35, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 36, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 37, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 38, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 39, EnumTerimalDataType.e_string);
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
            AddItemsResoult("电表2数据(有功电能示值)", TempData);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "400000,100000,100000,100000,100000";
                    if (TalkResult[i] == 0)
                    {
                        //TempData[i].Data = GetData(RecData, i, 40, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 41, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 42, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 43, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 44, EnumTerimalDataType.e_string);
                        string sTmp = GetData(RecData, i, 40, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 41, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 42, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 43, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 44, EnumTerimalDataType.e_string);
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
            AddItemsResoult("电表2数据(反向有功电能)", TempData);

        }

        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //三相电压|电能表日历时钟|时段表编程总次数|校时次数|电表运行状态字|正向有功电能示值|反向有功电能示值|正向有功最大需量|反向有功最大需量
            //ResultNames = new string[] { "正向有功电能示值","反向有功电能示值","正向有功最大需量", "反向有功最大需量",
            //    "抄表日正向有功电能示值","抄表日反向有功电能示值", "抄表日正向有功最大需量", "抄表日反向有功最大需量"
            //    ,"终端日供电时间,复位累计次数","结论" };

            ResultNames = new string[] { "读取采集任务监控数据结算日冻结", "电表1数据(有功电能示值)", "电表1数据(反向有功电能)", "电表2数据(有功电能示值)", "电表2数据(反向有功电能)", "结论" };
            return true;
        }
    }
}
