using LYZD.Core.Enum;
using LYZD.TerminalProtocol.Encryption;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LYZD.Verify.DataCollect
{
    /// <summary>
    /// 电能表当前数据
    /// </summary>
    public class EnergyCurrent : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //三相电压|电能表日历时钟|时段表编程总次数|校时次数|电表运行状态字|正向有功电能示值|反向有功电能示值|正向有功最大需量|反向有功最大需量
            ResultNames = new string[] { "读取采集任务监控数据", "电表1当前数据(有功电能示值)", "电表1当前数据(电压)", "电表1当前数据(电流)", "电表2当前数据(有功电能示值)", "电表2当前数据(电压)", "电表2当前数据(电流)", "结论" };
            return true;
        }
        public override void Verify()
        {
            base.Verify();        
            StartVerify698();

            ConnectLink(false);        

            ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
            ControlVirtualMeter("Cmd,DLS,5000,4000,1000,1000,1000,1000");

            SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
            SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

            MessageAdd("打开模拟表采集数据开关", EnumLogType.提示与流程信息, true);
            ControlVirtualMeter("MOP");

            SetData_698("06010343000800030000", "关闭主动上报");
            SetData_698("060119F1010200160000", "设置安全模式参数");
            ResetTerimal_698(2);
            ConnectLink2(false);
            DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 12:00:00");

            SetTime_698(dttmp, 0);

            dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 00:00:00").AddDays(1);
            SetData_698_No("07 01 14 60 14 7F 00 01 01 02 06 11 01 12 01 00 02 02 11 00 00 01 03 5B 00 00 10 02 00 5B 00 20 00 02 00 5B 00 20 01 02 00 5C 01 16 02 00", "下装普通采集方案");
            SetData_698_No("07 01 15 60 12 7F 00 01 01 02 0C 11 01 54 01 00 05 16 01 11 01 1C " + Talkers[0].Framer698.SetDateTimeBCD(dttmp, false) + " 1C 08 33 09 09 09 09 09 54 01 00 02 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00", "下装采集任务");


            //SetData_698_No("07 01 0F 60 14 7F 00 01 01 02 06 11 01 12 01 00 02 02 11 01 11 01 01 01 5B 01 50 04 02 00 03 20 21 02 00 00 10 02 00 00 20 02 00 5C 03 02 07 05 00 00 00 00 00 01 07 05 00 00 00 00 00 02 16 02 00  ", "下装普通采集方案");
            //SetData_698_No("07 01 10 60 12 7F 00 01 01 02 0C 11 01 54 03 00 01 16 01 11 01 1C 07 E1 07 1C 00 00 00 1C 08 33 09 09 09 09 09 54 01 00 02 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00 ", "下装采集任务");

            dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:50");
            SetTime_698(dttmp, 0);
            WaitTime("等待延时，", 240);

        


            MessageAdd("读取采集任务监控数据", EnumLogType.提示信息, true);
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
            AddItemsResoult("读取采集任务监控数据", TempData);

            MessageAdd("读取电表当前数据", EnumLogType.提示信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {

                   //Talkers[i].Framer698.sAPDU = "05 03 18 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 04 00 20 2A 02 00 00 00 10 02 00 00 20 00 02 00 00 20 01 02 00 00  ".Replace(" ", "");
                    Talkers[i].Framer698.sAPDU = "05 03 18 60 12 03 00 07 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(86409), false) + " 00 00 00 01 02 00 20 2a 02 00 00 00 10 02 00 00 ".Replace(" ", "");

                    //68 35 00 43 05 32 79 28 00 12 65 00 28 20 05 03 18 60 12 03 00 07 07 e6 06 19 00 00 00 07 e6 06 19 17 3b 3b 00 00 00 01 02 00 20 2a 02 00 00 00 10 02 00 00 26 f2 16
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
                        TempData[i].Data = GetData(RecData, i, 10, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 12, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 14, EnumTerimalDataType.e_string);
                        string sTmp = GetData(RecData, i, 10, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 12, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 14, EnumTerimalDataType.e_string);
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
            AddItemsResoult("电表1当前数据(有功电能示值)", TempData);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = "500000,125000,125000,125000,125000";
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data = GetData(RecData, i, 16, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 18, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 19, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 20, EnumTerimalDataType.e_string);
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
            AddItemsResoult("电表2当前数据(有功电能示值)", TempData);
            #region 电压电流采集不到--暂时不用

            //for (int i = 0; i < MeterNumber; i++)
            //{
            //    if (meterInfo[i].YaoJianYn)
            //    {
            //        TempData[i].StdData = "500000,125000,125000,125000,125000";
            //        if (TalkResult[i] == 0)
            //        {
            //            TempData[i].Data = GetData(RecData, i, 12, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 14, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 15, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 16, EnumTerimalDataType.e_string);
            //            string sTmp = GetData(RecData, i, 12, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 14, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 15, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 16, EnumTerimalDataType.e_string);
            //            if (sTmp != "500000,125000,125000,125000,125000")
            //                TempData[i].Resoult = "不合格";
            //            TempData[i].Data = sTmp;
            //        }
            //        else
            //        {
            //            TempData[i].Tips = "无回复";
            //            TempData[i].Resoult = "不合格";
            //        }

            //    }
            //}
            //AddItemsResoult("电表1当前数据(有功电能示值)", TempData);

            //for (int i = 0; i < MeterNumber; i++)
            //{
            //    if (meterInfo[i].YaoJianYn)
            //    {
            //        TempData[i].StdData = "2200,2200,2200";
            //        if (TalkResult[i] == 0)
            //        {
            //            TempData[i].Data = GetData(RecData, i, 17, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 18, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 19, EnumTerimalDataType.e_string);
            //            string sTmp = GetData(RecData, i, 17, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 18, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 19, EnumTerimalDataType.e_string);
            //            if (sTmp != "2200,2200,2200")
            //                TempData[i].Resoult = "不合格";
            //            TempData[i].Data = sTmp;
            //        }
            //        else
            //        {
            //            TempData[i].Tips = "无回复";
            //            TempData[i].Resoult = "不合格";
            //        }

            //    }
            //}
            //AddItemsResoult("电表1当前数据(电压)", TempData);

            //for (int i = 0; i < MeterNumber; i++)
            //{
            //    if (meterInfo[i].YaoJianYn)
            //    {
            //        TempData[i].StdData = "5000,5000,5000";
            //        if (TalkResult[i] == 0)
            //        {
            //            TempData[i].Data = GetData(RecData, i, 20, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 21, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 22, EnumTerimalDataType.e_string);
            //            string sTmp = GetData(RecData, i, 20, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 21, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 22, EnumTerimalDataType.e_string);
            //            if (sTmp != "5000,5000,5000")
            //                TempData[i].Resoult = "不合格";
            //            TempData[i].Data = sTmp;
            //        }
            //        else
            //        {
            //            TempData[i].Tips = "无回复";
            //            TempData[i].Resoult = "不合格";
            //        }

            //    }
            //}
            //AddItemsResoult("电表1当前数据(电流)", TempData);

            //for (int i = 0; i < MeterNumber; i++)
            //{
            //    if (meterInfo[i].YaoJianYn)
            //    {
            //        TempData[i].StdData = "500000,125000,125000,125000,125000";
            //        if (TalkResult[i] == 0)
            //        {
            //            TempData[i].Data = GetData(RecData, i, 24, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 25, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 26, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 27, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 28, EnumTerimalDataType.e_string);
            //            string sTmp = GetData(RecData, i, 24, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 25, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 26, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 27, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 28, EnumTerimalDataType.e_string);
            //            if (sTmp != "500000,125000,125000,125000,125000")
            //                TempData[i].Resoult = "不合格";
            //            TempData[i].Data = sTmp;
            //        }
            //        else
            //        {
            //            TempData[i].Tips = "无回复";
            //            TempData[i].Resoult = "不合格";
            //        }

            //    }
            //}
            //AddItemsResoult("电表2当前数据(有功电能示值)", TempData);

            //for (int i = 0; i < MeterNumber; i++)
            //{
            //    if (meterInfo[i].YaoJianYn)
            //    {
            //        TempData[i].StdData = "2200,2200,2200";
            //        if (TalkResult[i] == 0)
            //        {
            //            TempData[i].Data = GetData(RecData, i, 29, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 31, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 31, EnumTerimalDataType.e_string);
            //            string sTmp = GetData(RecData, i, 29, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 31, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 31, EnumTerimalDataType.e_string);
            //            if (sTmp != "2200,2200,2200")
            //                TempData[i].Resoult = "不合格";
            //            TempData[i].Data = sTmp;
            //        }
            //        else
            //        {
            //            TempData[i].Tips = "无回复";
            //            TempData[i].Resoult = "不合格";
            //        }

            //    }
            //}
            //AddItemsResoult("电表2当前数据(电压)", TempData);

            //for (int i = 0; i < MeterNumber; i++)
            //{
            //    if (meterInfo[i].YaoJianYn)
            //    {
            //        TempData[i].StdData = "5000,5000,5000";
            //        if (TalkResult[i] == 0)
            //        {
            //            TempData[i].Data = GetData(RecData, i, 32, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 33, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 34, EnumTerimalDataType.e_string);
            //            string sTmp = GetData(RecData, i, 32, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 33, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 34, EnumTerimalDataType.e_string);
            //            if (sTmp != "5000,5000,5000")
            //                TempData[i].Resoult = "不合格";
            //            TempData[i].Data = sTmp;
            //        }
            //        else
            //        {
            //            TempData[i].Tips = "无回复";
            //            TempData[i].Resoult = "不合格";
            //        }

            //    }
            //}
            //AddItemsResoult("电表2当前数据(电流)", TempData);
            #endregion

            MessageAdd("关闭模拟表采集数据开关", EnumLogType.提示与流程信息, true);
            ControlVirtualMeter("MCL");
        }
    }
}
