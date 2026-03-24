using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.DataHandling
{
    public class TransparentScheme : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //三相电压|电能表日历时钟|时段表编程总次数|校时次数|电表运行状态字|正向有功电能示值|反向有功电能示值|正向有功最大需量|反向有功最大需量
            ResultNames = new string[] { "透明转发", "正向有功电能示数据", "反向有功电能示数据", "组合无功1电能示数据", "组合无功2电能示数据", "正向有功最大需量数据", "电压", "电流", "电能表时钟", "事件类数据",  "结论" };
            return true;
        }

        public override void Verify()
        {
            base.Verify();
            StartVerify698();
            ConnectLink(true);
            SetTime_698(DateTime.Now, 0);
            ControlVirtualMeter("Cmd,DLS,5000,4000,1000,1000,1000,1000");
            ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
            ControlVirtualMeter("Cmd,ZDXLTIME");
            SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
            SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

            SetData_698("06010343000800030000", "关闭主动上报");
            SetData_698("060119F1010200160000", "设置安全模式参数");
            MessageAdd("打开模拟表采集数据开关", EnumLogType.提示与流程信息, true);
            ControlVirtualMeter("MOP");

            for (int kk = 0; kk < 1; kk++)
            {
                MessageAdd("透明转发", EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = " 09 07 08 F2 01 02 01 06 02 08 01 00 00 0A 00 0A 19 68 17 00 43 05 02 00 00 00 00 00 10 48 5E 05 01 07 00 10 02 00 00 04 01 16 00".Replace(" ", "");
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//代理读取电能量类数据
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Read485);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "54";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                            string sTmp = GetData(RecData, i, 4, EnumTerimalDataType.e_string);
                            if (sTmp != "54")
                                TempData[i].Resoult = "不合格";
                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }

                    }
                }
                AddItemsResoult("透明转发", TempData);

                MessageAdd("代理电能量数据", EnumLogType.提示信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = " 09 01 02 00 64 01 07 05 00 00 00 00 00 02 00 00 09 00 10 02 00 00 20 02 00 00 00 02 00 00 30 02 01 00 40 02 01 00 50 02 00 00 60 02 00 00 70 02 00 00 80 02 00 00".Replace(" ", "");
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//代理读取电能量类数据
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Read485);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "500000,125000,125000,125000,125000";
                        if (TalkResult[i] == 0)
                        {
                            string sTmp = GetData(RecData, i, 8, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 9, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 10, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 11, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 12, EnumTerimalDataType.e_string);
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
                AddItemsResoult("正向有功电能示数据", TempData);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "400000,100000,100000,100000,100000";
                        if (TalkResult[i] == 0)
                        {
                            string sTmp = GetData(RecData, i, 15, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 16, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 17, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 18, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 19, EnumTerimalDataType.e_string);
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
                AddItemsResoult("反向有功电能示数据", TempData);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "200000";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 29, EnumTerimalDataType.e_string);
                            string sTmp = GetData(RecData, i, 29, EnumTerimalDataType.e_string);
                            if (sTmp != "200000")
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
                AddItemsResoult("组合无功1电能示数据", TempData);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "200000";
                        if (TalkResult[i] == 0)
                        {
                            string sTmp = GetData(RecData, i, 32, EnumTerimalDataType.e_string);
                            if (sTmp != "200000")
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
                AddItemsResoult("组合无功2电能示数据", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = " 09 01 03 00 64 01 07 05 00 00 00 00 00 02 00 00 04 10 10 02 00 10 20 02 00 10 30 02 00 10 40 02 00 00".Replace(" ", "");
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//代理读取电能量类数据
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Read485);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "33000";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 8, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 9, EnumTerimalDataType.e_string);
                            string sTmp = GetData(RecData, i, 8, EnumTerimalDataType.e_string);
                            if (sTmp != "33000")
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
                AddItemsResoult("正向有功最大需量数据", TempData);


                MessageAdd("代理电压电流数据", EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = " 09 01 04 00 64 01 07 05 00 00 00 00 00 02 00 00 02 20 00 02 00 20 01 02 00 00".Replace(" ", "");
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//代理读取电能量类数据
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Read485);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "2200,2200,2200";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 8, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 9, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 10, EnumTerimalDataType.e_string);
                            string sTmp = GetData(RecData, i, 8, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 9, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 10, EnumTerimalDataType.e_string);
                            if (sTmp != "2200,2200,2200")
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
                AddItemsResoult("电压数据", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "5000,5000,5000";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 14, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 15, EnumTerimalDataType.e_string);
                            string sTmp = GetData(RecData, i, 13, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 14, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 15, EnumTerimalDataType.e_string);
                            if (sTmp != "5000,5000,5000")
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
                AddItemsResoult("电流数据", TempData);


                MessageAdd("电能表时钟", EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        Talkers[i].Framer698.sAPDU = " 09 01 05 00 64 01 07 05 00 00 00 00 00 02 00 00 01 40 00 02 00 00".Replace(" ", "");
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//代理读取电能量类数据
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Read485);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "01";
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = GetData(RecData, i, 8, EnumTerimalDataType.e_string);
                            string sTmp = GetData(RecData, i, 7, EnumTerimalDataType.e_string);
                            if (sTmp != "01")
                                TempData[i].Resoult = "不合格";
                        }
                        else
                        {
                            TempData[i].Tips = "无回复";
                            TempData[i].Resoult = "不合格";
                        }

                    }
                }
                AddItemsResoult("电能表时钟", TempData);

                

               

            }
            SetData_698("060119F1010200160100", "设置安全模式参数");

            MessageAdd("关闭模拟表采集数据开关", EnumLogType.提示与流程信息, true);
            ControlVirtualMeter("MCL");

        }
    }
}
