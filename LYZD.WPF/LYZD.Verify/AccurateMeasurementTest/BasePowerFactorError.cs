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
    /// 功率因素基本误差
    /// </summary>
    public class BasePowerFactorError : VerifyBase
    {
        float Ia = 0f;
        float Pa = 0f;
        float Pb = 0f;
        float Pc = 0f;
        float ErrorLimit = 1;

        /// <summary>
        /// 检定参数是否合法
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            string[] data = Test_Value.Split('|');
            int value = int.Parse(data[0].TrimEnd('%'));
            Ia = Number.GetCurrentByIb("ib", OnMeterInfo.MD_UA);
            //Glys = data[2];
            Pa = float.Parse( data[1]);
            Pb = float.Parse(data[2]);
            Pc = float.Parse(data[3]);
            ResultNames = new string[] { "标准功率", "终端功率", "误差", "误差限", "结论" };
            //ResultNames = new string[] { "检定数据", "结论" };
            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify698();
                ConnectLink(false);
                if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, Ia, Ia, Ia, 0,240,120, Pa,Pb,Pc))
                {
                    MessageAdd  ("升源失败,退出检定", EnumLogType.错误信息);
                    return;
                }
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (!meterInfo[i].YaoJianYn) continue;
                    int ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                    Talkers[i].Framer698.sAPDU = "10000805010F" + "200A0200" + "00" + "0110" + Talkers[i].Framer698.cOutRand;//读取版本信息
                    setData[i] = Talkers[i].Framer698.ReadData_jiaocai(Talkers[i].Framer698.sAPDU);

                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

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
                            float fStandard = Convert.ToSingle(Math.Pow(Convert.ToDouble(EquipmentData.StdInfo.P * EquipmentData.StdInfo.P / EquipmentData.StdInfo.S / EquipmentData.StdInfo.S), 0.5));
                            if (EquipmentData.StdInfo.P < 0) fStandard = 0 - fStandard;
                            float fTerminalErr = (float.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_float)) / 1000 - fStandard) / 1 * 100;
                            if (Math.Abs(fTerminalErr) > ErrorLimit)
                                Resoult[i] = "不合格";
               


                            ResultDictionary["标准功率"][i] = fStandard.ToString("F5");
                            ResultDictionary["终端功率"][i] = (float.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_float)) / 1000).ToString("F5");
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
