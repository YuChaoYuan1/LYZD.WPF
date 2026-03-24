using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.AccurateMeasurementTest
{
    /// <summary>
    /// 电流基本误差
    /// </summary>
    public class BaseCurrentError_376 : VerifyBase
    {

        float Ia = 0f;
        float ErrorLimit = 1;

        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            string[] data = Test_Value.Split('|');
            int value = int.Parse(data[0].TrimEnd('%'));
            Ia = Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA) * value / 100;
            ResultNames = new string[] { "A相误差", "B相误差", "C相误差", "误差限", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();

                if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, Ia, Ia, Ia, Cus_PowerYuanJian.H, PowerWay.正向有功, "1.0"))
                {
                    MessageAdd("升源失败,退出检定",EnumLogType.提示信息);
                    return;
                }
                //PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, Ia, Ia, Ia, 0, 240, 120, 0, 240, 120);
                //WaitTime("升源", 30);

                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 1, 25,RecData, MaxWaitSeconds_Read485);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (!meterInfo[i].YaoJianYn) continue;

                    string[] _DJ = Core.Function.Number.GetDj(meterInfo[i].MD_Grane);
                    ErrorLimit = float.Parse(_DJ[true ? 0 : 1]);
                    ResultDictionary["误差限"][i] = ErrorLimit.ToString();


                    if (TalkResult[i] == 0)
                    {
                        if (RecData[i].Length < 10)
                        {
                            MessageAdd("终端【" + (i + 1) + "】1类数据无回复！", EnumLogType.错误信息);
                            Resoult[i] = "不合格";
                        }
                        else
                        {
                            float fTerminalErrA = (float.Parse(GetData(RecData, i, 19, EnumTerimalDataType.e_float)) - EquipmentData.StdInfo.Ia) / Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA) * 100;
                            float fTerminalErrB = 0;
                            float fTerminalErrC = 0;

                            if (Clfs == WireMode.三相四线)
                            {
                                fTerminalErrB = (float.Parse(GetData(RecData, i, 20, EnumTerimalDataType.e_float)) - EquipmentData.StdInfo.Ib) / Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA) * 100;
                                fTerminalErrC = (float.Parse(GetData(RecData, i, 21, EnumTerimalDataType.e_float)) - EquipmentData.StdInfo.Ic) / Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA) * 100;

                            }
                            else if (Clfs == WireMode.三相四线)
                            {
                                fTerminalErrC = (float.Parse(GetData(RecData, i, 21, EnumTerimalDataType.e_float)) - EquipmentData.StdInfo.Ic) / Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA) * 100;
                            }


                            if (Math.Abs(fTerminalErrA) > ErrorLimit || Math.Abs(fTerminalErrB) > ErrorLimit || Math.Abs(fTerminalErrC) > ErrorLimit)
                            {
                                TempData[i].Resoult = "不合格";
                            }

                            ResultDictionary["A相误差"][i] = fTerminalErrA.ToString("F2");
                            ResultDictionary["B相误差"][i] = fTerminalErrB.ToString("F2");
                            ResultDictionary["C相误差"][i] = fTerminalErrC.ToString("F2");
                        }
                    }
                    else
                    {
                        MessageAdd("终端【" + (i + 1) + "】1类数据无回复！", EnumLogType.错误信息);
                        Resoult[i] = "不合格";
                    }
                }

                RefUIData("误差限");
                RefUIData("A相误差");
                RefUIData("B相误差");
                RefUIData("C相误差");
                PowerOn();
                WaitTime("恢复电压电流", 10);

                //for (int i = 0; i < MeterNumber; i++)
                //{
                //    if (!meterInfo[i].YaoJianYn) continue;
                //    ResultDictionary["结论"][i] = Resoult[i];
                //}
                //RefUIData("结论");
                MessageAdd("检定完成",EnumLogType.提示信息);
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
