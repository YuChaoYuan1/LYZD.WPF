using LYZD.Core.Enum;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Verify.AccurateMeasurementTest
{
    /// <summary>
    /// 电压基本误差
    /// </summary>
    public class BaseVoltageError : VerifyBase
    {

        float ub = 0f;
        //float A_Ua = 0;
        //float A_Ub = 240;
        //float A_Uc = 120;
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
            ub = OnMeterInfo.MD_UB * value / 100;
            Glys = data[1];
            //if (data[2] == "" || data[3] == "" || data[4] == "")
            //{
            //    return false;
            //}
            //A_Ua = int.Parse(data[2]);
            //A_Ub = int.Parse(data[3]);
            //A_Uc = int.Parse(data[4]);

            //ErrorLimit = int.Parse(data[5]);
            ResultNames = new string[] { "A相误差", "B相误差", "C相误差", "误差限", "结论" };

            return true;
        }
        public override void Verify()
        {
            try
            {
                base.Verify();
                StartVerify698();
                ConnectLink(false);
                if (!PowerOn(ub, ub, ub, 0, 0, 0, Cus_PowerYuanJian.H, PowerWay.正向有功, Glys))
                {
                    MessageAdd("升源失败,退出检定", EnumLogType.错误信息);
                    return;
                }
                //PowerOn(ub, ub, ub, 0, 0, 0, A_Ua, A_Ub, A_Uc, 0, 240, 120);
                //WaitTime("等待升源", 30);
                for (int i = 0; i < MeterNumber; i++)
                {
                    if (!meterInfo[i].YaoJianYn) continue;
                    int ret = TerminalProtocol.Encryption.IEncryptionFunction698.Obj_Formal_GetRandHost(i, ref Talkers[i].Framer698.cOutRand);
                    Talkers[i].Framer698.sAPDU = "10000805010F" + "20000200" + "00" + "0110" + Talkers[i].Framer698.cOutRand;//读取版本信息
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
                            TempData[i].Resoult = "不合格";
                        }
                        else
                        {
                            float fTerminalErrA = (float.Parse(GetData(RecData, i, 7, EnumTerimalDataType.e_float)) / 10 - EquipmentData.StdInfo.Ua) / OnMeterInfo.MD_UB * 100;
                            float fTerminalErrB = 0;
                            float fTerminalErrC = (float.Parse(GetData(RecData, i, 9, EnumTerimalDataType.e_float)) / 10 - EquipmentData.StdInfo.Uc) / OnMeterInfo.MD_UB * 100;

                            if (Clfs == WireMode.三相四线)
                            {
                                fTerminalErrB = (float.Parse(GetData(RecData, i, 8, EnumTerimalDataType.e_float)) / 10 - EquipmentData.StdInfo.Ub) / OnMeterInfo.MD_UB * 100;
                            }

                            if (Math.Abs(fTerminalErrA) > ErrorLimit || Math.Abs(fTerminalErrB) > ErrorLimit || Math.Abs(fTerminalErrC) > ErrorLimit)
                            {
                                TempData[i].Resoult = "不合格";
                            }

                            ResultDictionary["A相误差"][i] = fTerminalErrA.ToString("F5");
                            ResultDictionary["B相误差"][i] = fTerminalErrB.ToString("F5");
                            ResultDictionary["C相误差"][i] = fTerminalErrC.ToString("F5");
                        }
                    }
                    else
                    {
                        MessageAdd("终端【" + (i + 1) + "】1类数据无回复！", EnumLogType.错误信息);
                        TempData[i].Resoult = "不合格";
                    }
                }
                RefUIData("误差限");
                RefUIData("A相误差");
                RefUIData("B相误差");
                RefUIData("C相误差");
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