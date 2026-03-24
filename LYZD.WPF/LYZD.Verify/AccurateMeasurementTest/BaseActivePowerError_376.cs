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
    ///有功功率基本误差
    /// </summary>
    public class BaseActivePowerError_376 : VerifyBase
    {
        float Ia = 0f;
        float Pa = 0f;
        float Pb = 0f;
        float Pc = 0f;
        float ErrorLimit = 1;
        string Glys = "1.0";
        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            string[] data = Test_Value.Split('|');
            int value = int.Parse(data[0].TrimEnd('%'));
            Ia = Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA) * value / 100;
            Glys = data[1];
            //Pa = float.Parse(data[2]);
            //Pa = float.Parse(data[3]);
            //Pa = float.Parse(data[4]);
            ResultNames = new string[] { "标准功率", "终端功率", "误差", "误差限", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify();

                if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, Ia, Ia, Ia, Cus_PowerYuanJian.H, PowerWay.正向有功, Glys))
                {
                    MessageAdd("升源失败,退出检定",EnumLogType.提示信息);
                    return;
                }

                //1.读取终端电压、电流、功率、功率因数
                MessageAdd("正在读取终端1类数据",EnumLogType.流程信息);
                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 1, 25,RecData, MaxWaitSeconds_Read485);

                for (int i = 0; i < MeterNumber; i++)
                {
                    if (!meterInfo[i].YaoJianYn) continue;

                    string[] _DJ = Core.Function.Number.GetDj(meterInfo[i].MD_Grane);
                    ErrorLimit = float.Parse(_DJ[true ? 0 : 1]);
                    ResultDictionary["误差限"][i] = ErrorLimit.ToString();

                    float StdPQ = EquipmentData.StdInfo.P;

                    if (TalkResult[i] == 0)
                    {
                        if (RecData[i].Length < 27)
                        {
                            MessageAdd("终端【" + (i + 1) + "】1类数据无回复！", EnumLogType.错误信息);
                            Resoult[i] = "不合格";
                        }
                        else
                        {
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
                            float fTerminalErr = (float.Parse(GetData(RecData, i,4, EnumTerimalDataType.e_float)) * 1000 - StdPQ) / fStandard * 100;
                            if (Math.Abs(fTerminalErr) > ErrorLimit)
                                Resoult[i] = "不合格";

                            ResultDictionary["标准功率"][i] = StdPQ.ToString("F2");
                            ResultDictionary["终端功率"][i] = (float.Parse(GetData(RecData, i, 4, EnumTerimalDataType.e_float)) * 1000).ToString("F2");
                            ResultDictionary["误差"][i] = fTerminalErr.ToString("F2");
                        }
                    }
                    else
                    {
                        MessageAdd("终端【" + (i + 1) + "】1类数据无回复！", EnumLogType.错误信息);
                        TempData[i].Resoult = "不合格";
                        Resoult[i] = "不合格";
                    }
                }
                RefUIData("误差限");
                RefUIData("标准功率");
                RefUIData("终端功率");
                RefUIData("误差");
                PowerOn();//关闭电流
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
