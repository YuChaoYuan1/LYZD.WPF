using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using LYZD.ViewModel.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.AccurateMeasurementTest
{
    /// <summary>
    /// 电源影响
    /// </summary>
    public class BasePowerImpactError : VerifyBase
    {
        float ErrorLimit = 1;
        float Ua;
        float Ub;
        float Uc;
        float Ia;
        float Ib;
        float Ic;
        string strGlys;
        PowerWay powerWay;
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            if (string.IsNullOrEmpty(Test_Value))
            {
                Test_Value = "100%|100%|100%|100%|100%|100%|1.0|正向有功";
            }
            string[] data = Test_Value.Replace("%", "").Split('|');
            Ua = float.Parse(data[0]) / 100;
            Ub = float.Parse(data[1]) / 100;
            Uc = float.Parse(data[2]) / 100;
            Ia = float.Parse(data[3]) / 100;
            Ib = float.Parse(data[4]) / 100;
            Ic = float.Parse(data[5]) / 100;
            strGlys = data[6];
            switch (data[7])
            {
                case "正向有功":
                    powerWay = PowerWay.正向有功;
                    break;
                case "正向无功":
                    powerWay = PowerWay.正向无功;
                    break;
                case "反向有功":
                    powerWay = PowerWay.反向有功;
                    break;
                case "反向无功":
                    powerWay = PowerWay.反向无功;
                    break;
            }
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

                //5
                //ConnectLink(true);

                //SetData_698_No("07 01 04 43 00 08 00 03 01 00", "禁用终端主动上报");

                //SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");

                float IAP = Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA);

                if (!PowerOn(OnMeterInfo.MD_UB * Ua, OnMeterInfo.MD_UB * Ub, OnMeterInfo.MD_UB * Uc, IAP * Ia, IAP * Ib, IAP * Ic, 0, 240, 120, 0f, 240f, 120f, strGlys, powerWay))
                {
                    MessageAdd("升源失败,退出检定", EnumLogType.错误信息);
                    return;
                }
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

                StdInfoViewModel stdInfo = EquipmentData.StdInfo;
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
                AddItemsResoult("读取终端电压数据", TempData);

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
                AddItemsResoult("读取终端电流数据", TempData);

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

                #region//有功功率

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = stdInfo.P.ToString("F1") + "," + stdInfo.Pa.ToString("F1") + "," + stdInfo.Pb.ToString("F1") + "," + stdInfo.Pc.ToString("F1");
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetDataInt(RecData, i, 18, EnumTerimalDataType.e_int) / 10.0).ToString("F1") + "," + (GetDataInt(RecData, i, 19, EnumTerimalDataType.e_int) / 10.0).ToString("F1") + "," + (GetDataInt(RecData, i, 20, EnumTerimalDataType.e_int) / 10.0).ToString("F1") + "," + (GetDataInt(RecData, i, 21, EnumTerimalDataType.e_int) / 10.0).ToString("F1");
                            if (GetData(RecData, i, 17, EnumTerimalDataType.e_string) == "01")
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
                AddItemsResoult("读取终端有功功率数据", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        if (TalkResult[i] == 0)
                        {
                            string[] _DJ = Core.Function.Number.GetDj(meterInfo[i].MD_Grane);
                            ErrorLimit = float.Parse(_DJ[true ? 0 : 1]);
                            TempData[i].StdData = "±" + ErrorLimit.ToString();
                            float fStandard = 0;
                            if (Clfs == WireMode.三相四线)
                            {
                                fStandard = OnMeterInfo.MD_UB * Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA) * 3;
                            }
                            else if (Clfs == WireMode.三相三线)
                            {
                                fStandard = OnMeterInfo.MD_UB * Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA) * 1.732f;
                            }
                            else
                            {
                                fStandard = OnMeterInfo.MD_UB * Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA);
                            }
                            float fTerminalErr = (float.Parse(GetData(RecData, i, 18, EnumTerimalDataType.e_float)) / 10 - stdInfo.P) / fStandard * 100;
                            TempData[i].Data = fTerminalErr.ToString("F5");
                            if (Math.Abs(fTerminalErr) > ErrorLimit)
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
                AddItemsResoult("终端有功功率误差", TempData);

                #endregion

                #region 无功功率
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = stdInfo.Q.ToString("F1") + "," + stdInfo.Qa.ToString("F1") + "," + stdInfo.Qb.ToString("F1") + "," + stdInfo.Qc.ToString("F1");
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetDataInt(RecData, i, 24, EnumTerimalDataType.e_int) / 10.0).ToString("F1") + "," + (GetDataInt(RecData, i, 25, EnumTerimalDataType.e_int) / 10.0).ToString("F1") + "," + (GetDataInt(RecData, i, 26, EnumTerimalDataType.e_int) / 10.0).ToString("F1") + "," + (GetDataInt(RecData, i, 27, EnumTerimalDataType.e_int) / 10.0).ToString("F1");
                            if (GetData(RecData, i, 23, EnumTerimalDataType.e_string) == "01")
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
                AddItemsResoult("读取终端无功功率数据", TempData);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        if (TalkResult[i] == 0)
                        {
                            string[] _DJ = Core.Function.Number.GetDj(meterInfo[i].MD_Grane);
                            ErrorLimit = float.Parse(_DJ[true ? 0 : 1]);
                            TempData[i].StdData = "±" + ErrorLimit.ToString();
                            float fStandard = 0;
                            if (Clfs == WireMode.三相四线)
                            {
                                fStandard = OnMeterInfo.MD_UB * Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA) * 3;
                            }
                            else if (Clfs == WireMode.三相三线)
                            {
                                fStandard = OnMeterInfo.MD_UB * Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA) * 1.732f;
                            }
                            else
                            {
                                fStandard = OnMeterInfo.MD_UB * Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA);
                            }
                            float fTerminalErr = (float.Parse(GetData(RecData, i, 24, EnumTerimalDataType.e_float)) / 10 - stdInfo.Q) / fStandard * 100;
                            TempData[i].Data = fTerminalErr.ToString("F5");
                            if (Math.Abs(fTerminalErr) > ErrorLimit)
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
                AddItemsResoult("终端无功功率误差", TempData);

                #endregion

                #region 功率因素
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {
                        TempData[i].StdData = stdInfo.PF.ToString("F1") + "," + stdInfo.PFA.ToString("F1") + "," + stdInfo.PFB.ToString("F1") + "," + stdInfo.PFC.ToString("F1");
                        if (TalkResult[i] == 0)
                        {
                            TempData[i].Data = (GetDataInt(RecData, i, 30, EnumTerimalDataType.e_int) / 1000.0).ToString("F1") + "," + (GetDataInt(RecData, i, 31, EnumTerimalDataType.e_int) / 1000.0).ToString("F1") + "," + (GetDataInt(RecData, i, 32, EnumTerimalDataType.e_int) / 1000.0).ToString("F1") + "," + (GetDataInt(RecData, i, 33, EnumTerimalDataType.e_int) / 1000.0).ToString("F1");
                            if (GetData(RecData, i, 29, EnumTerimalDataType.e_string) == "01")
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
                AddItemsResoult("读取终端功率因数数据", TempData);



                for (int i = 0; i < MeterNumber; i++)
                {
                    if (meterInfo[i].YaoJianYn)
                    {

                        if (TalkResult[i] == 0)
                        {
                            string[] _DJ = Core.Function.Number.GetDj(meterInfo[i].MD_Grane);
                            ErrorLimit = float.Parse(_DJ[true ? 0 : 1]);
                            TempData[i].StdData = "±" + ErrorLimit.ToString();
                            float fStandard = Convert.ToSingle(Math.Pow(Convert.ToDouble(stdInfo.P * stdInfo.P / stdInfo.S / stdInfo.S), 0.5));
                            if (EquipmentData.StdInfo.P < 0) fStandard = 0 - fStandard;

                            float fTerminalErr = (float.Parse(GetData(RecData, i, 30, EnumTerimalDataType.e_float)) / 1000 - fStandard) / 1 * 100;

                            TempData[i].Data = fTerminalErr.ToString("F5");

                            if (Math.Abs(fTerminalErr) > ErrorLimit)
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
                AddItemsResoult("功率因数误差", TempData);
                #endregion


                MessageAdd("恢复电压电流", EnumLogType.流程信息, true);
                if (!PowerOn())
                {
                    MessageAdd("升源失败,退出检定", EnumLogType.错误信息);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }


        }
    }
}
