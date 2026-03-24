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
    public class BaseVoltageError_376 : VerifyBase
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
                StartVerify();
                if (!PowerOn(ub, ub, ub, 0, 0, 0, Cus_PowerYuanJian.H, PowerWay.正向有功, Glys))
                {
                    MessageAdd("升源失败,退出检定",EnumLogType.提示信息);
                    return;
                }
                //PowerOn(ub, ub, ub, 0, 0, 0, A_Ua, A_Ub, A_Uc, 0, 240, 120);
                //WaitTime("升源成功，等待源稳定", 30);
                //1.读取终端电压、电流、功率、功率因数
                MessageAdd("正在读取终端1类数据...",EnumLogType.流程信息);

                TalkResult = TerminalProtocalAdapter.Instance.ReadData_Afn12(12, 1, 25, RecData, MaxWaitSeconds_Read485);




                for (int i = 0; i < MeterNumber; i++)
                {
                    if (!meterInfo[i].YaoJianYn) continue;

                    string[] _DJ = Core.Function.Number.GetDj(meterInfo[i].MD_Grane);
                    ErrorLimit = float.Parse(_DJ[true ? 0 : 1]);
                    ResultDictionary["误差限"][i] = ErrorLimit.ToString();


                    if (TalkResult[i] == 0)
                    {
                        if (RecData[i].Length < 27)
                        {
                            TempData[i].Tips = "1类数据无回复！";
                            TempData[i].Resoult = "不合格";
                        }
                        else
                        {
                            float fTerminalErrA = (float.Parse(GetData(RecData, i, 16, EnumTerimalDataType.e_float)) - EquipmentData.StdInfo.Ua) / OnMeterInfo.MD_UB * 100;
                            float fTerminalErrB = 0;
                            float fTerminalErrC = 0;

                            if (Clfs == WireMode.三相四线)
                            {
                                fTerminalErrB = (float.Parse(GetData(RecData, i, 17, EnumTerimalDataType.e_float)) - EquipmentData.StdInfo.Ub) / OnMeterInfo.MD_UB * 100;
                                fTerminalErrC = (float.Parse(GetData(RecData, i, 18, EnumTerimalDataType.e_float)) - EquipmentData.StdInfo.Uc) / OnMeterInfo.MD_UB * 100;

                            }
                            else if (Clfs == WireMode.三相三线)
                            {
                                fTerminalErrC = (float.Parse(GetData(RecData, i, 18, EnumTerimalDataType.e_float)) - EquipmentData.StdInfo.Uc) / OnMeterInfo.MD_UB * 100;
                            }


                            if (Math.Abs(fTerminalErrA) > ErrorLimit || Math.Abs(fTerminalErrB) > ErrorLimit || Math.Abs(fTerminalErrC) > ErrorLimit)
                            {
                                TempData[i].Resoult = "不合格";
                                Resoult[i] = "不合格";
                            }

                            ResultDictionary["A相误差"][i] = fTerminalErrA.ToString("F2");
                            ResultDictionary["B相误差"][i] = fTerminalErrB.ToString("F2");
                            ResultDictionary["C相误差"][i] = fTerminalErrC.ToString("F2");
                        }
                    }
                    else
                    {
                        TempData[i].Tips = "1类数据无回复！";
                        TempData[i].Resoult = "不合格";
                    }
                }
                RefUIData("误差限");
                RefUIData("A相误差");
                RefUIData("B相误差");
                RefUIData("C相误差");
                //AddItemsResoult("A相误差", TempData);
                //AddItemsResoult("B相误差", TempData);
                //AddItemsResoult("C相误差", TempData);

                PowerOn();
                WaitTime("恢复电压", 10);
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