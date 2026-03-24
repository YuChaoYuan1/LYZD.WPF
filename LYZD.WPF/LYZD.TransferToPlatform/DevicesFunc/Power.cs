using LYZD.Core.Enum;
using LYZD.TransferToPlatform.Test;
using LYZD.ViewModel;
using LYZD.ViewModel.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LYZD.TransferToPlatform.DevicesFunc
{
    public class Power
    {
        private static double UA = 0.00;
        private static double UB = 0.00;
        private static double UC = 0.00;
        private static double IA = 0.00;
        private static double IB = 0.00;
        private static double IC = 0.00;


        private static void Set_U_I(double P_ua, double P_ub, double P_uc, double P_ia, double P_ib, double P_ic) {
            UA = P_ua;
            UB = P_ub;
            UC = P_uc;
            IA = P_ia;
            IB = P_ib;
            IC = P_ic;
        }

        #region //升源关源
        /// <summary>
        /// 普通升源
        /// </summary>
        /// <param name="MeterType"></param>
        /// <param name="UI_InOut"></param>
        /// <param name="U"></param>
        /// <param name="I"></param>
        /// <param name="powerWay"></param>
        /// <param name="Freq"></param>
        public static void PowerOn(string MeterType,Dictionary<int, string> UI_InOut,double U,double I,PowerWay powerWay,float Freq,float GLVYSJ) {
            if (!EquipmentData.DeviceManager.Devices.ContainsKey(DeviceName.功率源))
            {
                LogtestEven.add("未添加功率源");
                return;
            }
            double P_ua =0.0;
            double P_ub = 0.0;
            double P_uc = 0.0;
            double P_ia = 0.0;
            double P_ib = 0.0;
            double P_ic = 0.0;

            GetPhase(powerWay);

            foreach (var item in UI_InOut)
            {
                switch (item.Key) {
                    case 0:
                        if (item.Value == "1")
                        {
                            P_ua = U;
                            
                        }
                        break;
                    case 1:
                        if (item.Value == "1")
                        {
                            P_ub = U;
                        }
                        break;
                    case 2:
                        if (item.Value == "1")
                        {
                            P_uc = U;
                        }
                        break;
                    case 3:
                        if (item.Value == "1")
                        {
                            P_ia = I;
                            if ((Phase.PhaseUa + GLVYSJ) > 360)
                            {
                                Phase.PhaseIa = Phase.PhaseUa + GLVYSJ - 360;
                            }
                            else
                            {
                                Phase.PhaseIa = Phase.PhaseUa + GLVYSJ;
                            }
                        }
                        break;
                    case 4:
                        if (item.Value == "1")
                        {
                            P_ib = I;
                            if ((Phase.PhaseUa + GLVYSJ) > 360)
                            {
                                Phase.PhaseIb = Phase.PhaseUb + GLVYSJ - 360;
                            }
                            else
                            {
                                Phase.PhaseIb = Phase.PhaseUb + GLVYSJ;
                            }
                        }
                        break;
                    case 5:
                        if (item.Value == "1")
                        {
                            P_ic = I;
                            if ((Phase.PhaseUc + GLVYSJ) > 360)
                            {
                                Phase.PhaseIc = Phase.PhaseUc + GLVYSJ - 360;
                            }
                            else
                            {
                                Phase.PhaseIc = Phase.PhaseUc + GLVYSJ;
                            }
                        }
                        break;
                }
            }


            

            int jxfs = 0;
            if (MeterType == "三相四线")
            {
                jxfs = 0;
            }
            else if (MeterType == "三相三线")
            {
                jxfs = 2;
            }
            else if (MeterType == "单相")
            {
                jxfs = 5;
            }
            EquipmentData.StdInfo.IsPowerOkVisible = false;




            //IsAgainPower = true;
            if (EquipmentData.DeviceManager.Devices[DeviceName.功率源][0].Model == "E_CL309")
            {
                //Utility.TaskManager.AddDeviceAction(() =>
                //{
                EquipmentData.DeviceManager.CL309PowerOn(jxfs, 1, "1.0", (float)P_ua, (float)P_ub, (float)P_uc, (float)P_ia, (float)P_ib, (float)P_ic, 1, Freq, false);
                //});
                return;
            }
            Set_U_I(P_ua, P_ub, P_uc, P_ia, P_ib, P_ic);
            //Utility.TaskManager.AddDeviceAction(() =>
            //{
            EquipmentData.DeviceManager.PowerOn(jxfs, P_ua, P_ub, P_uc, P_ia, P_ib, P_ic, GetUPhase(Phase.PhaseUa), GetUPhase(Phase.PhaseUb), GetUPhase(Phase.PhaseUc), GetUPhase(Phase.PhaseIa), GetUPhase(Phase.PhaseIb), GetUPhase(Phase.PhaseIc), Freq, 1);
            //EquipmentData.DeviceManager.PowerOn(jxfs, P_ua, P_ub, P_uc, P_ia, P_ib, P_ic, GetUPhase(Phase.PhaseUa), GetUPhase(Phase.PhaseUb), GetUPhase(Phase.PhaseUc), Phase.PhaseIa, Phase.PhaseIb, Phase.PhaseIc, Freq, 1);
            //升源成功，需要判断源是否稳定了

        }

        /// <summary>
        /// 高级升源
        /// </summary>
        /// <param name="MeterType"></param>
        /// <param name="UI_InOut"></param>
        /// <param name="Freq"></param>
        /// <param name="data"></param>
        public static void PowerOnHigh(string MeterType, Dictionary<int, string> UI_InOut, float Freq,string[] data) {
            if (!EquipmentData.DeviceManager.Devices.ContainsKey(DeviceName.功率源))
            {
                LogtestEven.add("未添加功率源");
                return;
            }

            double P_ua = 0.0;
            double P_ub = 0.0;
            double P_uc = 0.0;
            double P_ia = 0.0;
            double P_ib = 0.0;
            double P_ic = 0.0;
            foreach (var item in UI_InOut)
            {
                switch (item.Key)
                {
                    case 0:
                        if (item.Value == "1")
                        {
                            P_ua =Convert.ToDouble(data[3]);
                            Phase.PhaseUa = Convert.ToDouble(data[4]);
                        }
                        break;
                    case 1:
                        if (item.Value == "1")
                        {
                            P_ub = Convert.ToDouble(data[5]);
                            Phase.PhaseUb = Convert.ToDouble(data[6]);
                        }
                        break;
                    case 2:
                        if (item.Value == "1")
                        {
                            P_uc = Convert.ToDouble(data[7]);
                            Phase.PhaseUc = Convert.ToDouble(data[8]);
                        }
                        break;
                    case 3:
                        if (item.Value == "1")
                        {
                            P_ia = Convert.ToDouble(data[9]);
                            Phase.PhaseIa = Convert.ToDouble(data[10]);
                        }
                        break;
                    case 4:
                        if (item.Value == "1")
                        {
                            P_ib = Convert.ToDouble(data[11]);
                            Phase.PhaseIb = Convert.ToDouble(data[12]);
                        }
                        break;
                    case 5:
                        if (item.Value == "1")
                        {
                            P_ic = Convert.ToDouble(data[13]);
                            Phase.PhaseIc = Convert.ToDouble(data[14]);
                        }
                        break;
                }
            }

            //Dictionary<int, string> dicInt = new Dictionary<int, string>();

            //string hex = Convert.ToString(Convert.ToInt32(data[15]), 2);

            //dicInt = HxeToInt(hex.ToString());

            //Harmonic(dicInt);


            int jxfs = 0;
            if (MeterType == "三相四线")
            {
                jxfs = 0;
            }
            else if (MeterType == "三相三线")
            {
                jxfs = 1;
            }
            else if (MeterType == "单相")
            {
                jxfs = 5;
            }
            EquipmentData.StdInfo.IsPowerOkVisible = false;
            Set_U_I(P_ua, P_ub, P_uc, P_ia, P_ib, P_ic);
            //IsAgainPower = true;
            if (EquipmentData.DeviceManager.Devices[DeviceName.功率源][0].Model == "E_CL309")
            {
                //Utility.TaskManager.AddDeviceAction(() =>
                //{
                EquipmentData.DeviceManager.CL309PowerOn(jxfs, 1, "1.0", (float)P_ua, (float)P_ub, (float)P_uc, (float)P_ia, (float)P_ib, (float)P_ic, 1, Freq, false);
                //});
                return;
            }



            //Utility.TaskManager.AddDeviceAction(() =>
            //{
            string ua = GetUPhase(Phase.PhaseUa).ToString();
            string ub = GetUPhase(Phase.PhaseUb).ToString();
            string uc = GetUPhase(Phase.PhaseUc).ToString();
            string ia = GetUPhase(Phase.PhaseIa).ToString();
            string ib = GetUPhase(Phase.PhaseIb).ToString();
            string ic = GetUPhase(Phase.PhaseIc).ToString();
            EquipmentData.DeviceManager.PowerOn(jxfs, P_ua, P_ub, P_uc, P_ia, P_ib, P_ic, GetUPhase(Phase.PhaseUa), GetUPhase(Phase.PhaseUb), GetUPhase(Phase.PhaseUc), GetUPhase(Phase.PhaseIa), GetUPhase(Phase.PhaseIb), GetUPhase(Phase.PhaseIc), Freq, 1);
            //升源成功，需要判断源是否稳定了

        }

        private static double GetUPhase(double phase)
        {
            return (360.0 - phase);
        }


        public static void PowerOff() {
            EquipmentData.DeviceManager.PowerOff();
            EquipmentData.StdInfo.IsPowerOkVisible = false;
            //IsAgainPower = false;
        }

        #endregion

        #region 谐波

       

        public static void Harmonic(string harmType,string[] data)
        {

            string H_uA = "0";
            string H_uB = "0";
            string H_uC = "0";
            string H_iA = "0";
            string H_iB = "0";
            string H_iC = "0";

            switch (harmType) {
                case "data=0":
                    H_uA = "1";
                    break;
                case "data=1":
                    H_uB = "1";
                    break;
                case "data=2":
                    H_uC = "1";
                    break;
                case "data=3":
                    H_iA = "1";
                    break;
                case "data=4":
                    H_iB = "1";
                    break;
                case "data=5":
                    H_iC = "1";
                    break;
            }

            float[] temHarmonicContent = new float[60];
            float[] temHarmonicPhase = new float[60];
            float harMsgs =0.0f;
            foreach (var item in data)
            {
                string[] harMsg = item.Split('-');
                temHarmonicContent[Convert.ToInt32(harMsg[0]) - 2] = float.Parse(harMsg[1]) * 100;
                harMsgs= float.Parse(harMsg[1]) * 100;
                temHarmonicPhase[Convert.ToInt32(harMsg[0]) - 2] = float.Parse(harMsg[2]);
            }
            
            EquipmentData.DeviceManager.PowerOn(0, UA, UB, UC, IA, IB, IC, 30.0F, 270.0F, 150.0F, 50.0F, 110.0F, 170.0F, 50, 1);

            Thread.Sleep(7000);
            if(harMsgs == 0.0f)
            EquipmentData.DeviceManager.ZH3001SetPowerGetHarmonic(H_uA, H_uB, H_uC, H_iA, H_iB, H_iC, temHarmonicContent, temHarmonicPhase, true);

            Thread.Sleep(2000);

            GetPhase(PowerWay.正向有功);

            EquipmentData.DeviceManager.PowerOn(0, UA, UB, UC, IA, IB, IC, Phase.PhaseUa, Phase.PhaseUb, Phase.PhaseUc, Phase.PhaseIa, Phase.PhaseIb, Phase.PhaseIc, 50, 1);
            Thread.Sleep(10000);
        }


        #endregion

        private static void GetPhase(PowerWay powerWay) {
            switch (powerWay)
            {
                case PowerWay.正向有功:
                    Phase.PhaseUa = 0.0;
                    Phase.PhaseUb = 240.0;
                    Phase.PhaseUc = 120.0;
                    Phase.PhaseIa = 0.0;
                    Phase.PhaseIb = 240.0;
                    Phase.PhaseIc = 120.0;
                    break;
                case PowerWay.正向无功:
                    Phase.PhaseUa = 0.0;
                    Phase.PhaseUb = 240.0;
                    Phase.PhaseUc = 120.0;
                    Phase.PhaseIa = 270.0;
                    Phase.PhaseIb = 150.0;
                    Phase.PhaseIc = 30.0;
                    break;
                case PowerWay.反向有功:
                    Phase.PhaseUa = 0.0;
                    Phase.PhaseUb = 240.0;
                    Phase.PhaseUc = 120.0;
                    Phase.PhaseIa = 180.0;
                    Phase.PhaseIb = 60.0;
                    Phase.PhaseIc = 300.0;
                    break;
                case PowerWay.反向无功:
                    Phase.PhaseUa = 0.0;
                    Phase.PhaseUb = 240.0;
                    Phase.PhaseUc = 120.0;
                    Phase.PhaseIa = 90.0;
                    Phase.PhaseIb = 330.0;
                    Phase.PhaseIc = 210.0;
                    break;
            }
        }

        /// <summary>
        /// 启停零线电流板
        /// </summary>
        /// <param name="startOrStopStd">字符‘1’开启，字符‘0’关闭</param>
        public static void ZeroLineAn(int A_kz, int BC_kz) {
            EquipmentData.DeviceManager.StartZeroCurrent(A_kz, BC_kz);
        }

        #region //数据转换工具
        /// <summary>
        /// 十六进制转换为二进制切割为单个字符
        /// </summary>
        /// <param name="Hex"></param>
        /// <returns></returns>
        private static Dictionary<int, string> HxeToInt(string Hex)
        {
            Dictionary<int, string> dicInt = new Dictionary<int, string>();

            for (int i = 0; i < 6; i++)
            {
                if (Hex.Length - 1 >= i)
                {
                    dicInt.Add(i, Hex[i].ToString());
                }
                else
                {
                    dicInt.Add(i, "0");
                }
            }
            return dicInt;
        }


        #endregion
    }
}
