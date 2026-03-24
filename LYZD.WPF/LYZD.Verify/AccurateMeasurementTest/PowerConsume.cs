using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LYZD.Verify.AccurateMeasurementTest
{
    class PowerConsume : VerifyBase
    {
        //电压A线路有功(W)|电压B线路有功(W)|电压C线路有功(W)|电压A线路视在(VA)|电压B线路视在(VA)|电压C线路视在(VA)|电流A线路视在(VA)|电流B线路视在(VA)|电流C线路视在(VA)|有功总(W)|视在总(VA)|有功视在总
        #region    PowerConsume 功耗试验
        private readonly float U_S_Limit = 6F;
        private readonly float U_P_Limit = 1.5F;
        private readonly float I_S_Limit = 1F;
        private bool[] BuYaoJianBiaoWei = null;   //不要检定的表位


        protected override bool CheckPara()
        {
            string[] data = Test_Value.Split('|');

            ResultNames = new string[] { "电压A线路有功(W)", "电压B线路有功(W)", "电压C线路有功(W)", "电压A线路视在(VA)", "电压B线路视在(VA)", "电压C线路视在(VA)", "电流A线路视在(VA)", "电流B线路视在(VA)", "电流C线路视在(VA)", "有功总(W)", "视在总(VA)", "有功视在总", "结论" };
            return true;
        }
        public override void Verify()
        {
            MessageAdd("功耗验检定开始...", EnumLogType.提示与流程信息);

            base.Verify();

            bool[] YJMeter = new bool[MeterNumber];

            BuYaoJianBiaoWei = new bool[MeterNumber];

            BuYaoJianBiaoWei.Fill(false);

            //关闭其余继电器，打开要测的继电器
            for (int i = 0; i < MeterNumber; i++)
            {
                if (i > 2) meterInfo[i].YaoJianYn = false;  //只有123 表位能测
                if (!meterInfo[i].YaoJianYn)
                {
                    EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)i);
                }
                else
                {
                    EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)i);
                }
            }
            if (!PowerOn(OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, OnMeterInfo.MD_UB, 0, 0, 0, Cus_PowerYuanJian.H, PowerWay.正向有功, "1"))
            {
                MessageAdd("升源失败,退出检定", EnumLogType.错误信息);
                return;
            }
            //下发参数
            float[] pd;
            //获取要做功耗的表位
            for (int i = 0; i < MeterNumber; i++)
            {
                int[] channelU = new int[3] { 1, 3, 5 };
                if (meterInfo[i].YaoJianYn)
                {

                    for (int c = 0; c < channelU.Length; c++)
                    {
                        Thread.Sleep(800);
                        //电压有效值，电流有效值，基波有功功率，基波无功功率
                        if (!DeviceControl.Read_GH_Dissipation(i, out pd))
                        {
                            if (!DeviceControl.Read_GH_Dissipation(i, out pd))
                            {
                                MessageAdd("读取" + i + "表位电压功耗失败！", EnumLogType.提示信息);
                            }
                        }

                        switch (c)
                        {
                            case 0: //A相
                                ResultDictionary["电压A线路视在(VA)"][i - 1] = pd[0].ToString("F4");
                                ResultDictionary["电压A线路有功(W)"][i - 1] = pd[3].ToString("F4");
                                ResultDictionary["电流A线路视在(VA)"][i - 1] = pd[6].ToString("F4");
                                if (ResultDictionary["结论"][i - 1] == "不合格") continue;
                                if (pd[0] > U_S_Limit || pd[3] > U_P_Limit || pd[6] > I_S_Limit)  // A
                                {
                                    ResultDictionary["结论"][i - 1] = "不合格";
                                }
                                else
                                    ResultDictionary["结论"][i - 1] = "不合格";

                                break;
                            case 1: //B相
                                ResultDictionary["电压B线路视在(VA)"][i - 1] = pd[1].ToString("F4");
                                ResultDictionary["电压B线路有功(W)"][i - 1] = pd[4].ToString("F4");
                                ResultDictionary["电流B线路视在(VA)"][i - 1] = pd[7].ToString("F4");
                                if (ResultDictionary["结论"][i - 1] == "不合格") continue;
                                if (pd[1] > U_S_Limit || pd[4] > U_P_Limit || pd[7] > I_S_Limit)  // B
                                {
                                    ResultDictionary["结论"][i - 1] = "不合格";
                                }
                                else
                                    ResultDictionary["结论"][i - 1] = "不合格";

                                break;
                            case 2: //C相
                                ResultDictionary["电压C线路视在(VA)"][i - 1] = pd[2].ToString("F4");
                                ResultDictionary["电压C线路有功(W)"][i - 1] = pd[5].ToString("F4");
                                ResultDictionary["电流C线路视在(VA)"][i - 1] = pd[8].ToString("F4");
                                if (ResultDictionary["结论"][i - 1] == "不合格") continue;
                                if (pd[2] > U_S_Limit || pd[5] > U_P_Limit || pd[8] > I_S_Limit)  // C
                                {
                                    ResultDictionary["结论"][i - 1] = "不合格";
                                }
                                else
                                    ResultDictionary["结论"][i - 1] = "不合格";

                                break;
                        }

                        if (c == channelU.Length - 1)
                            CheckOver = true;
                    }
                }

                //double 有功总(W)= 0.55555;
                //ResultDictionary["电压C线路视在(VA)"][i - 1] = pd[2].ToString("F4");
            }

            //恢复要检继电器
            ControlMeterRelay(GetYaoJian(), 1);
        }


     
        #endregion


    }
}
