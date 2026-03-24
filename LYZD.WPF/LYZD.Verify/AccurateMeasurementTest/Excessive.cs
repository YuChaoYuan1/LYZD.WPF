using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.AccurateMeasurementTest
{
    class Excessive : VerifyBase
    {
        #region  交流工频电量过量
        byte type = 0x00;
        int bs = 0;
        int cs = 0;
        int time = 0;
        int ontime = 1;
        string Glys = "1.0";
        float ErrorLimit = 1;
        protected override bool CheckPara()
        {
            string[] data = Test_Value.Split('|');
            ErrorLimit = (float)Convert.ToDouble(data[1]);
             ResultNames = new string[] { "终端交采电压", "终端交采电流","结论" };
            return true;
        }
        public override void Verify()
        {
            MessageAdd("交流工频电量过量", EnumLogType.提示与流程信息);
            base.Verify();
            StartVerify698();
            ConnectLink2(false);
            float IAP = Number.GetCurrentByIb("imax", OnMeterInfo.MD_UA);
            GearlLock(380f, 120f, 0x00);
            if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, IAP, IAP, IAP, Cus_PowerYuanJian.H, PowerWay.正向有功, Glys))
            {
                MessageAdd("升源失败,退出检定", EnumLogType.错误信息);
                return;
            }
            if (Test_Value.Split('|').Contains("否"))
            {
                type = 0x00;
                bs = 2;
                cs = 10;
                time = 1;
                ontime = 100;
            }
            else if (Test_Value.Split('|').Contains("是"))
            {
                type = 0x01;
                bs = 20;
                cs = 5;
                time = 1;
                ontime = 20;
            }
            SetExcessive(type, bs, cs, time, ontime, 0x01);
            for (int i = 0; i < cs; i++)
            {
                WaitTime($"第{i+1}次过量", ontime + 1);
            }

            MessageAdd("过量设置完成，关闭过量设置", EnumLogType.流程信息, true);
            SetExcessive(type, bs, cs, time, ontime, 0x00);


            int ret = 0;
            // "10000805010F200002000110"
            WaitTime("等待终端采样", 60);
            MessageAdd("读取交采数据", EnumLogType.流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                    Talkers[i].Framer698.sAPDU = "10 00 19 05 02 01 05 20 00 02 00 20 01 02 00 20 04 02 00 20 05 02 00 20 0a 02 00 00 01 10" + Talkers[i].Framer698.cOutRand;
                    setData[i] = Talkers[i].Framer698.ReadData_jiaocai(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
            ViewModel.Monitor.StdInfoViewModel stdInfo = EquipmentData.StdInfo;
            #region //电压
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = stdInfo.Ua.ToString("F1") + "," + stdInfo.Ub.ToString("F1") + "," + stdInfo.Uc.ToString("F1");
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data = (GetDataInt(RecData, i, 8, EnumTerimalDataType.e_int) / 10.0).ToString("F1") + "," + (GetDataInt(RecData, i, 9, EnumTerimalDataType.e_int) / 10.0).ToString("F1") + "," + (GetDataInt(RecData, i, 10, EnumTerimalDataType.e_int) / 10.0).ToString("F1");
                        if (GetData(RecData, i, 7, EnumTerimalDataType.e_string) == "01")
                        {

                            TempData[i].Resoult = "合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                        }
                    }
                }
            }
            AddItemsResoult("终端交采电压", TempData);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        string[] _DJ = Core.Function.Number.GetDj(meterInfo[i].MD_Grane);
                        ErrorLimit = float.Parse(_DJ[true ? 0 : 1]);
                        TempData[i].StdData = "±" + ErrorLimit.ToString();
                        float fTerminalErrA = (float.Parse(GetData(RecData, i, 8, EnumTerimalDataType.e_float)) / 10 - EquipmentData.StdInfo.Ua) / OnMeterInfo.MD_UB * 100;
                        float fTerminalErrB = 0;
                        float fTerminalErrC = (float.Parse(GetData(RecData, i, 10, EnumTerimalDataType.e_float)) / 10 - EquipmentData.StdInfo.Uc) / OnMeterInfo.MD_UB * 100;
                        if (Clfs == WireMode.三相四线)
                        {
                            fTerminalErrB = (float.Parse(GetData(RecData, i, 9, EnumTerimalDataType.e_float)) / 10 - EquipmentData.StdInfo.Ub) / OnMeterInfo.MD_UB * 100;
                        }
                        TempData[i].Data = fTerminalErrA.ToString("F5") + "," + fTerminalErrB.ToString("F5") + "," + fTerminalErrC.ToString("F5");
                        if (Math.Abs(fTerminalErrA) > ErrorLimit || Math.Abs(fTerminalErrB) > ErrorLimit || Math.Abs(fTerminalErrC) > ErrorLimit)
                        {
                            TempData[i].Resoult = "不合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "合格";
                        }
                    }
                }
            }
            AddItemsResoult("电压误差数据", TempData);

            #endregion


            #region //电流
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    TempData[i].StdData = stdInfo.Ia.ToString("F3") + "," + stdInfo.Ib.ToString("F3") + "," + stdInfo.Ic.ToString("F3");
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].Data = (GetDataFloat(RecData, i, 13, EnumTerimalDataType.e_int) / 1000.0).ToString("F3") + "," + (GetDataFloat(RecData, i, 14, EnumTerimalDataType.e_int) / 1000.0).ToString("F3") + "," + (GetDataFloat(RecData, i, 15, EnumTerimalDataType.e_int) / 1000.0).ToString("F3");

                        if (GetData(RecData, i, 12, EnumTerimalDataType.e_string) == "01")
                        {
                            TempData[i].Resoult = "合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "不合格";
                        }

                    }
                }
            }
            AddItemsResoult("终端交采电流", TempData);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {

                    if (TalkResult[i] == 0)
                    {
                        string[] _DJ = Core.Function.Number.GetDj(meterInfo[i].MD_Grane);
                        ErrorLimit = float.Parse(_DJ[true ? 0 : 1]);
                        TempData[i].StdData = "±" + ErrorLimit.ToString();
                        float fTerminalErrA = (float.Parse(GetData(RecData, i, 13, EnumTerimalDataType.e_float)) / 1000 - stdInfo.Ia) / OnMeterInfo.MD_UB * 100;
                        float fTerminalErrB = 0;
                        float fTerminalErrC = (float.Parse(GetData(RecData, i, 15, EnumTerimalDataType.e_float)) / 1000 - stdInfo.Ic) / OnMeterInfo.MD_UB * 100;

                        if (Clfs == WireMode.三相四线)
                        {
                            fTerminalErrB = (float.Parse(GetData(RecData, i, 14, EnumTerimalDataType.e_float)) / 1000 - stdInfo.Ib) / OnMeterInfo.MD_UB * 100;
                        }
                        TempData[i].Data = fTerminalErrA.ToString("F5") + "," + fTerminalErrB.ToString("F5") + "," + fTerminalErrC.ToString("F5");
                        if (Math.Abs(fTerminalErrA) > ErrorLimit || Math.Abs(fTerminalErrB) > ErrorLimit || Math.Abs(fTerminalErrC) > ErrorLimit)
                        {
                            TempData[i].Resoult = "不合格";
                        }
                        else
                        {
                            TempData[i].Resoult = "合格";
                        }
                    }
                }
            }
            AddItemsResoult("电流误差数据", TempData);
            #endregion

            GearlLock(0, 0, 0x00);
        }



        #endregion

    }
}
