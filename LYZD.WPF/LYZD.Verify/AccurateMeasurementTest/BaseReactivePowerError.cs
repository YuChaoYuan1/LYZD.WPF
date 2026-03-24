using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using System;


namespace LYZD.Verify.AccurateMeasurementTest
{
    /// <summary>
    ///无功功率基本误差
    /// </summary>
    public class BaseReactivePowerError : VerifyBase
    {
        float Ia = 0f;
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
                StartVerify698();
                ConnectLink(false);
                if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, Ia, Ia, Ia, Cus_PowerYuanJian.H, PowerWay.正向无功, Glys))
                {
                    MessageAdd("升源失败,退出检定", EnumLogType.错误信息);
                    return;
                }
                //PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB,Ia,Ia,Ia, 0, 240, 120, Pa, Pb, Pc);
                //WaitTime("等待升源", 30);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (!meterInfo[i].YaoJianYn) continue;
                    int ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                    Talkers[i].Framer698.sAPDU = "10000805010F" + "20050200" + "00" + "0110" + Talkers[i].Framer698.cOutRand;//读取版本信息
                    setData[i] = Talkers[i].Framer698.ReadData_jiaocai(Talkers[i].Framer698.sAPDU);

                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
               
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (!meterInfo[i].YaoJianYn) continue;

                    string[] _DJ = Core.Function.Number.GetDj(meterInfo[i].MD_Grane);
                    ErrorLimit = float.Parse(_DJ[true ? 0 : 1]);
                    ResultDictionary["误差限"][i] = ErrorLimit.ToString();

                    float StdPQ = EquipmentData.StdInfo.Q;

                    if (TalkResult[i] == 0)
                    {
                        if (RecData[i].Length < 10)
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
                            float fTerminalErr = (float.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_float)) / 10 - StdPQ) / fStandard * 100;
                            if (Math.Abs(fTerminalErr) > ErrorLimit)
                                Resoult[i] = "不合格";

                            ResultDictionary["标准功率"][i] = StdPQ.ToString("F5");
                            ResultDictionary["终端功率"][i] = (float.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_float)) / 10).ToString("F5");
                            ResultDictionary["误差"][i] = fTerminalErr.ToString("F5");
                        }
                    }
                    else
                    {
                        MessageAdd("终端【" + (i + 1) + "】1类数据无回复！", EnumLogType.错误信息);
                        Resoult[i] = "不合格";
                    }
                }
                RefUIData("误差限");
                RefUIData("标准功率");
                RefUIData("终端功率");
                RefUIData("误差");
                PowerOn();
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (!meterInfo[i].YaoJianYn) continue;
                    ResultDictionary["结论"][i] = Resoult[i];
                }
                RefUIData("结论");
                MessageAdd("检定完成", EnumLogType.提示信息);
            }
            catch (Exception ex)
            {

                MessageAdd(ex.ToString(), EnumLogType.错误信息);
            }
        }
    }
}
