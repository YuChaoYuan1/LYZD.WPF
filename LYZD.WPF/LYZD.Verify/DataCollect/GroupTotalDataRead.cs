using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LYZD.Verify.DataCollect
{
    public class GroupTotalDataRead : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "读取总加组1的当日总加有功电能量", "读取总加组1的当月总加有功电能量", "删除采集档案", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify698();


                bool[] bol_VerifyFlags = new bool[MeterNumber];
                for (int i = 0; i < MeterNumber; i++)
                {
                    bol_VerifyFlags[i] = true;
                }

                int ret = 0;
                ConnectLink(false);

                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

                SetData_698("070121230101000000", "清除总加组配置表");
                ResetTerimal_698(2);
                ConnectLink2(true);
                SetData_698("06012524010200090680008000800100", "设置脉冲计量1_通信地址");

                SetData_698("06012624010300020212000112000100", "设置脉冲计量1_互感器倍率");
                SetData_698("07012724010300020351F20A020116001203E800", "添加脉冲输入单元");
                SetData_698("0701282301030002035507058000800080011600160000", "添加总加配置单元");
                //SetData_698_No("07 01 29 60 00 7F 00 02 04 12 00 1E 02 0A 55 07 05 80 00 80 00 80 01 16 03 16 03 51 F2 0A 02 01 09 02 00 00 11 05 11 00 16 01 12 08 98 12 00 0F 02 04 55 07 05 00 00 00 00 00 00 09 02 00 00 12 00 01 12 00 01 01 00 00", "下发采集档案");
                //SetData_698_No("06 01 2A F2 03 04 00 02 02 04 08 00 04 08 00 00", "设置开关量输入");
                //SetData_698_No("07 01 2B 60 14 7F 00 01 01 02 06 11 01 12 01 00 02 02 11 00 00 01 01 5B 00 00 10 02 00 5C 01 16 01 00", "下装普通采集方案");
                //SetData_698_No("07 01 2C 60 12 7F 00 01 01 02 0C 11 01 54 01 00 02 16 01 11 01 1C 07 E1 08 0C 00 00 00 1C 08 33 09 09 09 09 09 54 00 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3B 00", "下装采集任务");


                DateTime dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month).AddMonths(1).AddDays(-1).AddHours(23);
                SetTime_698(dttmp, 0);
                WaitTime("等待", 30);

                MessageAdd("台体输出脉冲信号", EnumLogType.流程信息);

                SetPulseOutput(GetYaoJian(), 0x03, 0.8666f, 0.5f, 52, 0.8666f, 0.5f, 52);
                Thread.Sleep(500);
                WaitTime("等待", 120);

                MessageAdd("台体停止脉冲信号", EnumLogType.流程信息);
                SetPulseOutputStop(GetYaoJian());
                WaitTime("等待", 5);

                dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month).AddMonths(1).AddHours(20).AddMinutes(52);
                SetTime_698(dttmp, 0);
                WaitTime("等待", 15);


                MessageAdd("台体输出脉冲信号", EnumLogType.流程信息);

                SetPulseOutput(GetYaoJian(), 0x03, 0.5f, 0.5f, 30, 0.5f, 0.5f, 30);
                Thread.Sleep(500);
                WaitTime("等待", 120);

                dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month).AddMonths(1).AddHours(23).AddMinutes(59);
                SetTime_698(dttmp, 0);
                WaitTime("等待", 90);

                MessageAdd("台体停止脉冲信号", EnumLogType.流程信息);
                SetPulseOutputStop(GetYaoJian());
                WaitTime("等待", 5);

                dttmp = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month).AddMonths(1).AddDays(1).AddHours(20).AddMinutes(52);
                SetTime_698(dttmp, 0);
                WaitTime("等待", 5);

                SetPulseOutput(GetYaoJian(), 0x03, 1f, 0.5f, 60, 1f, 0.5f, 60);
                Thread.Sleep(500);
                WaitTime("等待", 120);


                MessageAdd("读取总加组1的当日总加有功电能量", EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "23010701" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "600";
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);


                            ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);


                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";

                            }
                            TempData[i].Data = double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)).ToString();

                            if (double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) != 600)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "误差超限";
                            }

                        }
                    }
                }
                AddItemsResoult("读取总加组1的当日总加有功电能量", TempData);

                MessageAdd("读取总加组1的当月总加有功电能量", EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10000805010F" + "23010901" + "00" + "0110" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);


                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "900";
                        if (TalkResult[i] == 0)
                        {
                            Talkers[i].Framer698.ucReadData = GetData(RecData, i, 2, EnumTerimalDataType.e_string);
                            Talkers[i].Framer698.ucMac = GetMac(RecData, i);


                            ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Terminal_Formal_VerifyReadData(i, Talkers[i].Framer698.iKeyState, 1, Talkers[i].Framer698.cTESAMNO, Talkers[i].Framer698.cOutRand, Talkers[i].Framer698.ucReadData, Talkers[i].Framer698.ucMac, ref Talkers[i].Framer698.cOutData);


                            if (ret != 0)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "数据验证失败";

                            }
                            TempData[i].Data = double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)).ToString();

                            if (double.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_double)) != 900)
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "误差超限";
                            }

                        }
                    }
                }
                AddItemsResoult("读取总加组1的当月总加有功电能量", TempData);

                MessageAdd("开始恢复被检表时间", EnumLogType.提示信息);
                dttmp = DateTime.Now;
                SetTime_698(dttmp, 0);
                WaitTime("等待", 10);

                Thread.Sleep(500);
                MessageAdd("检定完成", EnumLogType.提示信息);
            }
            catch (Exception ex)
            {
                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
