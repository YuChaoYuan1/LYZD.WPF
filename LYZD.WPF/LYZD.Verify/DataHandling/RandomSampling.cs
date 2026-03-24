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

namespace LYZD.Verify.DataHandling
{
    /// <summary>
    /// 随机交采数据
    /// </summary>
    public class RandomSampling : VerifyBase
    {
        float ErrorLimit = 1;
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "检定数据", "结论" };
            return true;
        }
        float Ia = Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA);
        public override void Verify()
        {
            base.Verify();

            StartVerify698();

            SetData_698_No("07 01 04 43 00 08 00 03 00 00", "禁用终端主动上报");

            SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");

            DateTime dttmp = DateTime.Now;
            SetTime_698(dttmp, 0);

            #region 额定电压电流交采
            if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, Ia, Ia, Ia, Cus_PowerYuanJian.H, PowerWay.正向有功, "1.0", 49))
            {
                MessageAdd("升源失败,退出检定", EnumLogType.错误信息);
                return;
            }
            WaitTime("等待", 20);
            MessageAdd("读取额定电压电流交采数据", EnumLogType.流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    int ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                    Talkers[i].Framer698.sAPDU = "10 00 19 05 02 01 05 20 00 02 00 20 01 02 00 20 04 02 00 20 05 02 00 20 0a 02 00 00 01 10" + Talkers[i].Framer698.cOutRand;
                    setData[i] = Talkers[i].Framer698.ReadData_jiaocai(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            StdInfoViewModel stdInfo = EquipmentData.StdInfo;

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].StdData = "A:" + stdInfo.Ua.ToString("F3") + ",B:" + stdInfo.Ub.ToString("F3") + ",C:" + stdInfo.Uc.ToString("F3");

                        float Ua = GetDataFloat(RecData, i, 8, EnumTerimalDataType.e_int) / 10;
                        float Ub = GetDataFloat(RecData, i, 9, EnumTerimalDataType.e_float) / 10;
                        float Uc = GetDataFloat(RecData, i, 10, EnumTerimalDataType.e_float) / 10;
                        TempData[i].Data = "A:" + Ua.ToString("F1") + ",B:" + Ub.ToString("F1") + ",C:" + Uc.ToString("F1");
                        if (Math.Abs(stdInfo.Ua - Ua) > 2 || Math.Abs(stdInfo.Ub - Ub) > 2 || Math.Abs(stdInfo.Uc - Uc) > 2)
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
            AddItemsResoult("额定电压数据", TempData);

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].StdData = "A:" + stdInfo.Ia.ToString("F3") + ",B:" + stdInfo.Ib.ToString("F3") + ",C:" + stdInfo.Ic.ToString("F3");
                        float JCIa = GetDataFloat(RecData, i, 13, EnumTerimalDataType.e_float) / 1000;
                        float JCIb = GetDataFloat(RecData, i, 14, EnumTerimalDataType.e_float) / 1000;
                        float JCIc = GetDataFloat(RecData, i, 15, EnumTerimalDataType.e_float) / 1000;
                        TempData[i].Data = "A:" + JCIa.ToString("F1") + ",B:" + JCIb.ToString("F1") + ",C:" + JCIc.ToString("F1");
                        if (Math.Abs(stdInfo.Ia - JCIa) > 2 || Math.Abs(stdInfo.Ib - JCIb) > 2 || Math.Abs(stdInfo.Ic - JCIc) > 2)
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
            AddItemsResoult("额定电流数据", TempData);
            #endregion
            WaitTime("等待切换系数", 5);
            percentNum(0.8, "80%");
            WaitTime("等待切换系数", 5);
            percentNum(0.9, "90%");
            WaitTime("等待切换系数", 5);
            percentNum(1.1, "110%");
            WaitTime("等待切换系数", 5);
            percentNum(1.2, "120%");
            WaitTime("等待切换系数", 5);
            MessageAdd("恢复正常电压电流", EnumLogType.流程信息, true);
            if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, 0, 0, 0, Cus_PowerYuanJian.H, PowerWay.正向有功, "1.0", 49))
            {
                MessageAdd("升源失败,退出检定", EnumLogType.错误信息);
                return;
            }
        }

        /// <summary>
        /// 百分比交采数据
        /// </summary>
        /// <param name="overNum"></param>
        /// <param name="overMsg"></param>
        private void percentNum(double overNum, string overMsg)
        {
            #region 110%交采
            float overU = float.Parse((OnMeterInfo.MD_UB * overNum).ToString("F3"));
            float overI = float.Parse((Ia * overNum).ToString("F3"));
            if (!PowerOn(overU, overU, overU, Ia, Ia, Ia, Cus_PowerYuanJian.H, PowerWay.正向有功, "1.0", 49))
            {
                MessageAdd("升源失败,退出检定", EnumLogType.错误信息);
                return;
            }

            WaitTime("等待", 20);
            MessageAdd("读取" + overMsg + "额定电压交采数据", EnumLogType.流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    int ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                    Talkers[i].Framer698.sAPDU = "10 00 19 05 02 01 05 20 00 02 00 20 01 02 00 20 04 02 00 20 05 02 00 20 0a 02 00 00 01 10" + Talkers[i].Framer698.cOutRand;
                    setData[i] = Talkers[i].Framer698.ReadData_jiaocai(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            StdInfoViewModel stdInfo = EquipmentData.StdInfo;

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].StdData = "A:" + stdInfo.Ua.ToString("F3") + ",B:" + stdInfo.Ub.ToString("F3") + ",C:" + stdInfo.Uc.ToString("F3");
                        float Ua = GetDataFloat(RecData, i, 8, EnumTerimalDataType.e_float) / 10 ;
                        float Ub = GetDataFloat(RecData, i, 9, EnumTerimalDataType.e_float) / 10;
                        float Uc = GetDataFloat(RecData, i, 10, EnumTerimalDataType.e_float) / 10;
                        TempData[i].Data = "A:" + Ua.ToString("F1") + ",B:" + Ub.ToString("F1") + ",C:" + Uc.ToString("F1");
                        if (Math.Abs(stdInfo.Ua - Ua) > 2 || Math.Abs(stdInfo.Ub - Ub) > 2 || Math.Abs(stdInfo.Uc - Uc) > 2)
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
            AddItemsResoult(overMsg + "电压数据", TempData);

            #region 110%电流
            if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, overI, overI, overI, Cus_PowerYuanJian.H, PowerWay.正向有功, "1.0", 49))
            {
                MessageAdd("升源失败,退出检定", EnumLogType.错误信息);
                return;
            }

            WaitTime("等待", 20);
            MessageAdd("读取" + overMsg + "额定电流交采数据", EnumLogType.流程信息, true);
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    int ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                    Talkers[i].Framer698.sAPDU = "10 00 19 05 02 01 05 20 00 02 00 20 01 02 00 20 04 02 00 20 05 02 00 20 0a 02 00 00 01 10" + Talkers[i].Framer698.cOutRand;
                    setData[i] = Talkers[i].Framer698.ReadData_jiaocai(Talkers[i].Framer698.sAPDU);
                }
            }
            TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

            stdInfo = EquipmentData.StdInfo;

            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {
                    if (TalkResult[i] == 0)
                    {
                        TempData[i].StdData = "A:" + stdInfo.Ia.ToString("F3") + ",B:" + stdInfo.Ib.ToString("F3") + ",C:" + stdInfo.Ic.ToString("F3");
                        float JCIa = GetDataFloat(RecData, i, 13, EnumTerimalDataType.e_float) / 1000;
                        float JCIb = GetDataFloat(RecData, i, 14, EnumTerimalDataType.e_float) / 1000;
                        float JCIc = GetDataFloat(RecData, i, 15, EnumTerimalDataType.e_float) / 1000;
                        TempData[i].Data = "A:" + JCIa.ToString("F1") + ",B:" + JCIb.ToString("F1") + ",C:" + JCIc.ToString("F1");
                        if (Math.Abs(stdInfo.Ia - JCIa) > 2 || Math.Abs(stdInfo.Ib - JCIb) > 2 || Math.Abs(stdInfo.Ic - JCIc) > 2)
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
            AddItemsResoult(overMsg + "电流数据", TempData);
            #endregion
            #endregion
        }
    }
}
