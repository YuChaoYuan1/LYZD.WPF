using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.DataHandling
{
    /// <summary>
    /// 抄表mac验证
    /// </summary>
    public class ReadMeterMacCheck : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "抄表mac验证", "结论" };
            return true;
        }
        public override void Verify()
        {
            base.Verify();
            StartVerify698();

            ConnectLink(true);

            SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
            SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

            ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
            ControlVirtualMeter("Cmd,DLS,5000,4000,1000,1000,1000,1000");

            ControlVirtualMeter("NoResMac,0000000000000001");

            DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 12:00:00");
            SetTime_698(dttmp, 0);

            ResetTerimal_698(2);

            SetData_698_No("07 01 31 60 14 7F 00 01 01 02 06 11 01 12 01 00 02 02 11 01 11 01 01 01 5B 01 50 04 02 00 03 20 21 02 00 00 10 02 00 00 20 02 00 5C 01 16 06 00", "下装普通采集方案");

            SetData_698_No("07 01 32 60 12 7f 00 01 01 02 0c 11 01 54 02 00 01 16 01 11 01 1c " + Talkers[0].Framer698.SetDateTimeBCD(dttmp, false) + "  1c 08 33 09 09 09 09 09 54 01 00 02 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3b 00", "下装采集任务");

            dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 23:59:00").AddDays(1);
            SetTime_698(dttmp, 0);

            WaitTime("等待，", 310);

            #region 读取采集任务监控数据
            MessageAdd("读取采集任务监控数据", EnumLogType.流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.sAPDU = "05 03 24 60 34 02 00 01 60 35 02 01 11 01 00 00".Replace(" ", "");
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }

            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    string endTime = GetData(RecData, i, 11, EnumTerimalDataType.e_datetime);
                    if (endTime == "NULL")
                    {
                        int AllMeter = GetDataInt(RecData, i, 12, EnumTerimalDataType.e_int);
                        int SuccessMeter = GetDataInt(RecData, i, 13, EnumTerimalDataType.e_int);
                        if (AllMeter != SuccessMeter + 1)
                        {
                            TempData[i].Tips = "采集任务监控数据成功表数量不正确";
                            TempData[i].Resoult = "不合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "合格";
                        }
                    }
                    else
                    {
                        TempData[i].Tips = "采集任务监控数据结束时间不正确";
                        TempData[i].Resoult = "不合格";
                    }
                }
            }
            #endregion

            ControlVirtualMeter("NoResMac,0000000000000000");//恢复虚拟表响应

            WaitTime("等待，", 240);
            #region 读取采集任务监控数据
            MessageAdd("读取采集任务监控数据", EnumLogType.流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.sAPDU = "05 03 24 60 34 02 00 01 60 35 02 01 11 01 00 00".Replace(" ", "");
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }

            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    string endTime = GetData(RecData, i, 11, EnumTerimalDataType.e_datetime);
                    if (endTime != "NULL")
                    {
                        int AllMeter = GetDataInt(RecData, i, 12, EnumTerimalDataType.e_int);
                        int SuccessMeter = GetDataInt(RecData, i, 13, EnumTerimalDataType.e_int);
                        if (AllMeter != SuccessMeter)
                        {
                            TempData[i].Tips = "采集任务监控数据成功表数量不正确";
                            TempData[i].Resoult = "不合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "合格";
                        }
                    }
                    else
                    {
                        TempData[i].Tips = "采集任务监控数据未结束";
                        TempData[i].Resoult = "不合格";
                    }
                }
            }
            #endregion

            #region 读取电表事件数据
            dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " 00:00:00");
            MessageAdd("读取电表事件数据", EnumLogType.流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    Talkers[i].Framer698.sAPDU = "05 03 38 60 12 03 00 05 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp, false) +" 01 05 00 20 2A 02 00 00 60 40 02 00 00 60 41 02 00 00 60 42 02 00 01 50 04 02 00 03 20 21 02 00 00 10 02 00 00 20 02 00 00 ".Replace(" ", "");
                    setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                }
            }

            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].Data = GetData(RecData, i, 13, EnumTerimalDataType.e_string);
                    if (GetData(RecData, i, 13, EnumTerimalDataType.e_string) != "1")
                    {
                        TempData[i].Tips = "抄表数据错误" + TempData[i].Data;
                        TempData[i].Resoult = "不合格";
                    }
                    else
                    {
                        TempData[i].Resoult = "合格";
                    }
                }
            }

            #endregion
        }
    }
}
