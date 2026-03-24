using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.AccurateMeasurementTest
{
    public class PowerFactorOver : VerifyBase
    {
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {

            ResultNames = new string[] { "添加一个日区间统计关联对象属性", "添加一个月区间统计关联对象属性", "读取功率因数日区间统计", "读取功率因数月区间统计","结论" };
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

                SetTime_698(DateTime.Now, 0);

                SetData_698_No("07 01 1C 60 00 83 00 12 00 0A 00", "删除配置序号为10的交采档案");

                SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
                SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

                SetData_698_No("07 01 1F 60 00 7F 00 02 04 12 00 0A 02 0A 55 07 15 10 34 00 78 00 58 16 03 16 03 51 F2 08 02 01 09 02 00 00 11 05 11 00 16 01 12 08 98 12 00 0F 02 04 55 07 05 00 00 00 00 00 00 09 02 00 00 12 00 01 12 00 01 01 00 00 ", "下发配置序号为10的交采档案");

                SetMingSessionData_698("07 01 22 21 02 04 00 51 20 0A 02 01 00", "删除日区间统计关联OAD");
                SetMingSessionData_698("07 01 23 21 03 04 00 51 20 0A 02 01 00 ", "删除月区间统计关联OAD");
                ResetTerimal_698(2);


                SetData_698_No("07 01 04 43 00 08 00 03 01 00", "禁用终端主动上报");
                float Ia = Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA);
                PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, Ia, Ia, Ia, Cus_PowerYuanJian.H, PowerWay.正向有功, "1.0");

                SetMingSessionData_698("07 01 25 21 02 03 00 02 04 51 20 0A 02 01 01 03 58 00 10 00 00 10 02 BC 58 00 10 02 BC 10 03 84 58 02 10 03 84 10 03 E8 11 01 54 00 00 0A 00", "添加一个日区间统计关联对象属性");
                SetMingSessionData_698("07 01 26 21 03 03 00 02 04 51 20 0A 02 01 01 03 58 00 10 00 00 10 02 BC 58 00 10 02 BC 10 03 84 58 02 10 03 84 10 03 E8 11 01 54 00 00 0A 00", "添加一个月区间统计关联对象属性");

                WaitTime("等待", 300);
                PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, Ia, Ia, Ia, Cus_PowerYuanJian.H, PowerWay.正向有功, "0.8");
                WaitTime("等待", 240);
                PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, Ia, Ia, Ia, Cus_PowerYuanJian.H, PowerWay.正向有功, "0.6");
                WaitTime("等待", 360);
                PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, Ia, Ia, Ia, Cus_PowerYuanJian.H, PowerWay.正向有功, "1.0");


                MessageAdd("读取功率因数日区间统计", EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10 00 08 05 01 27 21 02 02 00 00 01 10" + Talkers[i].Framer698.cOutRand;

                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "≈360,240,300 +30";
                        if (TalkResult[i] == 0)
                        {
                            int num1 = GetDataInt(RecData, i, 8, EnumTerimalDataType.e_int);
                            int num2 = GetDataInt(RecData, i, 10, EnumTerimalDataType.e_int);
                            int num3 = GetDataInt(RecData, i, 12, EnumTerimalDataType.e_int);
                            TempData[i].Data = GetDataInt(RecData, i, 8, EnumTerimalDataType.e_int) + "," + GetDataInt(RecData, i, 10, EnumTerimalDataType.e_int) + "," + GetDataInt(RecData, i, 12, EnumTerimalDataType.e_int);
                            if (GetData(RecData, i, 6, EnumTerimalDataType.e_string) == "01"&& num1<=390 && num2 <= 270 && num3 <= 330)
                            {
                                TempData[i].Resoult = "合格";
                            }
                            else
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "结果错误或者越限时间超出";
                            }

                        }
                    }
                }

                AddItemsResoult("读取功率因数日区间统计", TempData);
                MessageAdd("读取功率因数月区间统计", EnumLogType.流程信息, true);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                        Talkers[i].Framer698.sAPDU = "10 00 08 05 01 27 21 03 02 00 00 01 10" + Talkers[i].Framer698.cOutRand;
                        setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = "≈360,240,300 +30";
                        if (TalkResult[i] == 0)
                        {
                            int num1 = GetDataInt(RecData, i, 8, EnumTerimalDataType.e_int);
                            int num2 = GetDataInt(RecData, i, 10, EnumTerimalDataType.e_int);
                            int num3 = GetDataInt(RecData, i, 12, EnumTerimalDataType.e_int);
                            TempData[i].Data = GetDataInt(RecData, i, 8, EnumTerimalDataType.e_int) + "," + GetDataInt(RecData, i, 10, EnumTerimalDataType.e_int) + "," + GetDataInt(RecData, i, 12, EnumTerimalDataType.e_int);
                            if (GetData(RecData, i, 6, EnumTerimalDataType.e_string) == "01" && num1 <= 390 && num2 <= 270 && num3 <= 330)
                            {
                                TempData[i].Resoult = "合格";
                            }
                            else
                            {
                                TempData[i].Resoult = "不合格";
                                TempData[i].Tips = "结果错误或者越限时间超出";
                            }

                        }
                    }
                }
                AddItemsResoult("读取功率因数月区间统计", TempData);
                ConnectLink(false);
                SetTime_698(DateTime.Now, 0);

                SetMingSessionData_698("07 01 22 21 02 04 00 51 20 0A 02 01 00", "删除日区间统计关联OAD");
                SetMingSessionData_698("07 01 23 21 03 04 00 51 20 0A 02 01 00 ", "删除月区间统计关联OAD");
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }


    }
}
