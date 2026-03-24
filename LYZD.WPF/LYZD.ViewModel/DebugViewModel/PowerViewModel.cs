using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.ViewModel.DebugViewModel
{
    /// <summary>
    /// 源
    /// </summary>
    public class PowerViewModel : ViewModelBase
    {
        #region 自由升源信息
        private double ua = 220;
        public double Ua
        {
            get { return ua; }
            set { SetPropertyValue(value, ref ua, "Ua"); }
        }
        private double phaseUa;
        public double PhaseUa
        {
            get { return phaseUa; }
            set { SetPropertyValue(value, ref phaseUa, "PhaseUa"); }
        }
        private double ub = 220;
        public double Ub
        {
            get { return ub; }
            set { SetPropertyValue(value, ref ub, "Ub"); }
        }
        private double phaseUb = 240;
        public double PhaseUb
        {
            get { return phaseUb; }
            set { SetPropertyValue(value, ref phaseUb, "PhaseUb"); }
        }
        private double uc = 220;
        public double Uc
        {
            get { return uc; }
            set { SetPropertyValue(value, ref uc, "Uc"); }
        }
        private double phaseUc = 120;
        public double PhaseUc
        {
            get { return phaseUc; }
            set { SetPropertyValue(value, ref phaseUc, "PhaseUc"); }
        }
        private double ia;
        public double Ia
        {
            get { return ia; }
            set { SetPropertyValue(value, ref ia, "Ia"); }
        }
        private double phaseIa;
        public double PhaseIa
        {
            get { return phaseIa; }
            set { SetPropertyValue(value, ref phaseIa, "PhaseIa"); }
        }
        private double ib = 0;
        public double Ib
        {
            get { return ib; }
            set { SetPropertyValue(value, ref ib, "Ib"); }
        }
        private double phaseIb = 240;
        public double PhaseIb
        {
            get { return phaseIb; }
            set { SetPropertyValue(value, ref phaseIb, "PhaseIb"); }
        }
        private double ic = 0;
        public double Ic
        {
            get { return ic; }
            set { SetPropertyValue(value, ref ic, "Ic"); }
        }
        private double phaseIc = 120;
        public double PhaseIc
        {
            get { return phaseIc; }
            set { SetPropertyValue(value, ref phaseIc, "PhaseIc"); }
        }
        private float freq = 50;

        public float Freq
        {
            get { return freq; }
            set { SetPropertyValue(value, ref freq, "Freq"); }
        }


        private string setting = "三相四线";

        public string Setting
        {
            get { return setting; }
            set { SetPropertyValue(value, ref setting, "Setting"); }
        }

        /// <summary>
        /// 升源
        /// </summary>
        public void PowerOnFree()
        {
            //EquipmentData.DeviceManager.PowerOn(Ua, Ub, Uc, Ia, Ib, Ic, PhaseUa, PhaseUb, PhaseUc, PhaseIa, PhaseIb, PhaseIc, Freq);
            int jxfs = 0;
            if (Setting == "三相四线")
            {
                jxfs = 0;
            }
            else if (Setting == "三相三线")
            {
                jxfs = 1;
            }
            else if (Setting == "单相")
            {
                jxfs = 5;
            }

            ViewModel.Device.DeviceData device = null;// EquipmentData.DeviceManager.Devices.FirstOrDefault(item => item.Model == "E_CL309");
            if (device != null)
            {
                Utility.TaskManager.AddDeviceAction(() =>
                {
                    EquipmentData.DeviceManager.CL309PowerOn(jxfs, 1, "1.0", (float)Ua, (float)Ub, (float)Uc, (float)Ia, (float)Ib, (float)Ic, 1, Freq, false);
                });
                return;
            }

            Utility.TaskManager.AddDeviceAction(() =>
            {
                EquipmentData.DeviceManager.PowerOn(jxfs, Ua, Ub, Uc, Ia, Ib, Ic, PhaseUa, PhaseUb, PhaseUc, PhaseIa, PhaseIb, PhaseIc, Freq, 1);
            });

        }

        /// <summary>
        /// 关源
        /// </summary>
        public void PowerOffFree()
        {
            Utility.TaskManager.AddDeviceAction(() =>
            {
                EquipmentData.DeviceManager.PowerOff();
            });
        }

        //bool t = false;
        public void PowerOn()
        {


            Utility.TaskManager.AddDeviceAction(() =>
            {
                EquipmentData.DeviceManager.PowerOn();
            });
            //Utility.Log.LogManager.AddMessage("测试",Utility.Log.EnumLogSource.检定业务日志,Utility.Log .EnumLevel .Error);
            //Utility.Log.LogManager.AddMessage("测试", Utility.Log.EnumLogSource.检定业务日志, Utility.Log.EnumLevel.Tip);
            //EquipmentData .CallMsg("VerifyCompelate");
            // double std =0;
            //double[] s = new double[6] { 0,0,0,0,0,0 };
            //EquipmentData.DeviceManager.stdGear2(0x10, ref std,ref s);
            //EquipmentData.DeviceManager.SetEquipmentThreeColor();

            //EquipmentData.DeviceManager.SetTimePulse(t);
            //t = !t;
        }


        public void test()
        {

            //View_Input. 

            //VerifyBase.ControlVirtualMeter("DLS00010");

            //VerifyBase.ControlVirtualMeter("Cmd,Set,220,220,220,3,3,3,0,0,0,1,0");

            //
            //VerifyBase.ControlVirtualMeter("Cmd,RunFlag,0");

            //Window_TipsLog log = new Window_TipsLog();
            //log.Show();

            //Utility.TaskManager.AddDeviceAction(() =>
            //{
            //    EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)i);
            //});

            //System.Diagnostics.Stopwatch TimeS = new System.Diagnostics.Stopwatch();
            //TimeS.Start();//开始
            //TimeS.Stop(); //结束
            //Utility.Log.LogManager.AddMessage("开启继电器---共用时：" + TimeS.ElapsedMilliseconds);
            //VerifyBase verifyBase = new VerifyBase();
            //Utility.Log.LogManager.AddMessage("开始发送测试");

            //try
            //{
            //    verifyBase.StdGear(0x13, VerifyConfig.StdConst, 220, 220, 220, 0, 0, 0);
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
            //Utility.Log.LogManager.AddMessage("发送测试完成");

        }
        public void test2()
        {
            //Mis.DataHelper.DataManage.GetDnbInfoNew(VerifyBase.meterInfo[0], false);
        }
        #endregion
    }
}
