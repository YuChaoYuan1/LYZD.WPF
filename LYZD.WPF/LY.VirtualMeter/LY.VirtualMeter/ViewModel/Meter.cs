using LY.VirtualMeter.Base;
using LY.VirtualMeter.Core;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LY.VirtualMeter.ViewModel
{
    /// <summary>
    /// 虚拟表的对象
    /// </summary>
    public class Meter : Function
    {

        #region 属性
        /// <summary>
        /// 序号
        /// </summary>
        private int index = 0;

        public int Index
        {
            get { return index; }
            set { SetPropertyValue(value, ref index, "Index"); }
        }
        /// <summary>
        /// A相电压
        /// </summary>
        private double ua = 220;

        public double Ua
        {
            get { return ua; }
            set { SetPropertyValue(value, ref ua, "Ua"); }
        }
        /// <summary>
        /// B相电压
        /// </summary>
        private double ub = 220;

        public double Ub
        {
            get { return ub; }
            set { SetPropertyValue(value, ref ub, "Ub"); }
        }
        /// <summary>
        ///C相电压
        /// </summary>
        private double uc = 220;

        public double Uc
        {
            get { return uc; }
            set { SetPropertyValue(value, ref uc, "Uc"); }
        }
        /// <summary>
        ///A相电流
        /// </summary>
        private double ia = 1.5;

        public double Ia
        {
            get { return ia; }
            set { SetPropertyValue(value, ref ia, "Ia"); }
        }
        /// <summary>
        ///B相电流
        /// </summary>
        private double ib = 1.5;

        public double Ib
        {
            get { return ib; }
            set { SetPropertyValue(value, ref ib, "Ib"); }
        }
        /// <summary>
        ///C相电流
        /// </summary>
        private double ic = 1.5;

        public double Ic
        {
            get { return ic; }
            set { SetPropertyValue(value, ref ic, "Ic"); }
        }

        /// <summary>
        ///A相相位角
        /// </summary>
        private double phia = 0;

        public double Phia
        {
            get { return phia; }
            set { SetPropertyValue(value, ref phia, "Phia"); }
        }
        /// <summary>
        ///B相相位角
        /// </summary>
        private double phib = 0;

        public double Phib
        {
            get { return phib; }
            set { SetPropertyValue(value, ref phib, "Phib"); }
        }
        /// <summary>
        ///C相相位角
        /// </summary>
        private double phic = 0;

        public double Phic
        {
            get { return phic; }
            set { SetPropertyValue(value, ref phic, "Phic"); }
        }

        /// <summary>
        ///表位地址
        /// </summary>
        private string bAddress = "000000000001";

        public string BAddress
        {
            get { return bAddress; }
            set { SetPropertyValue(value, ref bAddress, "BAddress"); }
        }

        /// <summary>
        ///A相有功功率
        /// </summary>
        private double pa = 0;

        public double Pa
        {
            get { return pa; }
            set { SetPropertyValue(value, ref pa, "Pa"); }
        }


        private double pb = 0;
        /// B相功率
        /// <summary>
        /// B相功率
        /// </summary>
        public double Pb
        {
            get { return pb; }
            set { SetPropertyValue(value, ref pb, "Pb"); }
        }
        private double pc = 0;

        public double Pc
        {
            get { return pc; }
            set { SetPropertyValue(value, ref pc, "Pc"); }
        }
        private double qa;

        public double Qa
        {
            get { return qa; }
            set { SetPropertyValue(value, ref qa, "Qa"); }
        }
        private double qb;

        public double Qb
        {
            get { return qb; }
            set { SetPropertyValue(value, ref qb, "Qb"); }
        }
        private double qc;

        public double Qc
        {
            get { return qc; }
            set { SetPropertyValue(value, ref qc, "Qc"); }
        }
        private double sa;

        public double Sa
        {
            get { return sa; }
            set { SetPropertyValue(value, ref sa, "Sa"); }
        }
        private double sb;

        public double Sb
        {
            get { return sb; }
            set { SetPropertyValue(value, ref sb, "Sb"); }
        }
        private double sc;

        public double Sc
        {
            get { return sc; }
            set { SetPropertyValue(value, ref sc, "Sc"); }
        }
        private double p;

        public double P
        {
            get { return p; }
            set { SetPropertyValue(value, ref p, "P"); }
        }
        private double q;

        public double Q
        {
            get { return q; }
            set { SetPropertyValue(value, ref q, "Q"); }
        }
        private double s;

        public double S
        {
            get { return s; }
            set { SetPropertyValue(value, ref s, "S"); }
        }
        #endregion


        /// <summary>
        /// 频率
        /// </summary>
        public double Hz = 50;

        /// <summary>
        /// 电量
        /// </summary>
        public double[,] Pq = new double[8, 5];

        /// <summary>
        /// 需量
        /// </summary>
        public double[,] Zdxl = new double[8, 5];
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime[,] ZdxlTime = new DateTime[8, 5];


        private string _当前显示电量;

        public string 当前显示电量
        {
            get { return _当前显示电量; }
            set { SetPropertyValue(value, ref _当前显示电量, "当前显示电量"); }

        }
        private string _当前显示时间;

        public string 当前显示时间
        {
            get { return _当前显示时间; }
            set { SetPropertyValue(value, ref _当前显示时间, "当前显示时间"); }

        }

        /// <summary>
        /// 编程时间
        /// </summary>
        public DateTime LastBCtime = new DateTime();
        /// <summary>
        /// 清零时间
        /// </summary>
        public DateTime LastQLtime = new DateTime();
        public DateTime LastSDtime = new DateTime();
        public string AutoDate = "0001";
        public int QLCS = 8;//电表清零次数
        public int BCCS = 8;//编程次数
        public int SDBCCS = 8;//时段编程次数
        public int Mfunusual = 8;//磁场异常总次数
        public int MCoverOpen = 8;//开表盖总次数
        public int MHeOpen = 8;//开端钮盒总次数
        public int CutOffCurCount_A = 0;//A相断流次数
        public int CutOffCurCount_B = 0;//B相断流次数
        public int CutOffCurCount_C = 0;//C相断流次数
        public string MHeOpenEvent = "EE";
        public string MCoverOpennEvent = "EE";
        public int XSNum = 8;//校时总次数
        public int DCWorkTime = 8;
        public int intDdcs = 0;//停电次数
        public string[] strtdsj = new string[] { "430713050516", "430713050516", "430713050516", "430713050516", "430713050516", "430713050516", "430713050516", "430713050516", "430713050516", "430713050516" };//停电时间
        public string[] strsdsj = new string[] { "430713050516", "430713050516", "430713050516", "430713050516", "430713050516", "430713050516", "430713050516", "430713050516", "430713050516", "430713050516" };//上电时间
        public string dyhgq = "000040";//电压互感器倍率
        public string dlhgq = "000040";//电流互感器倍率
        public int Cmd19010001 = 0;
        public string Cmd19010101 = "430713050516";//A相过流发生时刻
        public string Cmd19012101 = "430714050516";//A相过流结束时刻
        public int DXCS = 8;//断相次数
        public string DXFSTime = "121012020509";   //断相发生时间
        public string DXJSTime = "121013020509";   //断相结束时间
        public int SYCS = 8;//失压次数
        public string SYFSTime = "121012020509";   //失压发生时间
        public string SYJSTime = "121013020509";   //失压结束时间
        public DateTime dtEventHappen = new DateTime();
        public DateTime dtEventEnd = new DateTime();
        public int QSYCS = 8;//全失压次数
        public int DcZhenChang = 1;
        public byte BiaoZt = 0;
        public byte Wangzt = 0;
        public int BiaoYgCS = 600;
        public int BiaoWgCS = 600;
        public double TimeLoss = 0;
        public double DateLoss = 0;
        public double zzXiShu = 1;//用来走字
        public double DnXiShu = 1;//用来示度下降
        //public double[,] Pq = new double[8, 5];

        public string NoResponseMac = "9999999999999999";

        public double[,] PqDj = new double[8, 5];
        public double[,] ZdxlDj = new double[8, 5];
        public DateTime[] ShiDuan = new DateTime[8];
        public int[] FeiLei = new int[8];
        public int BiaoType = 0;
        public string[] B_ZT = new string[7];
        public string YouGong_ZT = "A0";

        public bool IsLog = true;//是否打印报文
        public System.Windows.Forms.Timer tm = new System.Windows.Forms.Timer();

        public bool bol_MeterDateTimeCheck = false;
        public DateTime MeterDateTime = DateTime.Now;
        public DateTime MeterDataTime698 = DateTime.Now;
        string Tseq = "";
        public Meter(int intTmp)
        {
            BAddress = (intTmp).ToString().PadLeft(12, '0');
            index = intTmp;
            LoadData();
            tm.Enabled = true;
            tm.Interval = 5000;
            tm.Tick += new System.EventHandler(this.timer1_Tick);
        }
        public Meter(int intTmp, string address)
        {
            BAddress = address.PadLeft(12, '0');
            index = intTmp;
            LoadData();
            tm.Enabled = true;
            tm.Interval = 5000;
            tm.Tick += new System.EventHandler(this.timer1_Tick);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            //tm.Enabled = false;
            Thread t = new Thread(Run);
            t.Start();
        }

        public DateTime DateBron()
        {
            if (bol_MeterDateTimeCheck) return MeterDateTime;
            DateTime Tdatetime;
            Tdatetime = DateTime.Now.AddDays(0 - DateLoss);
            Tdatetime = Tdatetime.AddMinutes(0 - TimeLoss);
            return Tdatetime;
            //Tdatetime = DateAndTime.DateAdd(DateInterval.Day,(double)(0-DateLoss),DateTime.Now);
            //return DateAndTime.DateAdd(DateInterval.Minute,0-TimeLoss,Tdatetime);
        }

        public void Run()
        {
            Time_Run(5000, zzXiShu);
            SaveData();
        }

        public void SaveData()
        {
            string Tf;
            string[] Cl = new string[7]; int Ti; string Tstr; int Tj;
            string TShiDuan = "";

            //if (Ia == 0 && Ib == 0 && Ic == 0 && !IsSave) return;

            for (Ti = 0; Ti < 8; Ti++)
                //TShiDuan = TShiDuan + ShiDuan[Ti].ToString("yy-MM-dd HH:mm:ss") + (Ti != 7 ? "," : "");
                TShiDuan = TShiDuan + ShiDuan[Ti].ToString("HH:mm") + (Ti != 7 ? "," : "");

            string TFeiLei = "";
            for (Ti = 0; Ti < 8; Ti++)
                TFeiLei = TFeiLei + string.Format("{0:0}", FeiLei[Ti]) + (Ti != 7 ? "," : "");



            Tf = Application.StartupPath + "\\Ini\\MeterData\\meter" + index + ".txt";


            Function.g_WriteINI(Tf, "para", "Ua", string.Format("{0:0.0}", Ua));
            Function.g_WriteINI(Tf, "para", "Ub", string.Format("{0:0.0}", Ub));
            Function.g_WriteINI(Tf, "para", "Uc", string.Format("{0:0.0}", Uc));
            Function.g_WriteINI(Tf, "para", "Ia", string.Format("{0:0.000}", Ia));
            Function.g_WriteINI(Tf, "para", "Ib", string.Format("{0:0.000}", Ib));
            Function.g_WriteINI(Tf, "para", "Ic", string.Format("{0:0.000}", Ic));

            Function.g_WriteINI(Tf, "para", "Phia", string.Format("{0:0.0}", Phia));
            Function.g_WriteINI(Tf, "para", "Phib", string.Format("{0:0.0}", Phib));
            Function.g_WriteINI(Tf, "para", "Phic", string.Format("{0:0.0}", Phic));
            for (Ti = 0; Ti < 8; Ti++)
            {
                Tstr = "" + Interaction.Choose(Ti + 1, "01", "02", "11", "12", "13", "14", "15", "16");
                Function.g_WriteINI(Tf, "data", "9" + Tstr + "0", string.Format("{0:0.00}", Pq[Ti, 0]));
                Function.g_WriteINI(Tf, "data", "9" + Tstr + "1", string.Format("{0:0.00}", Pq[Ti, 1]));
                Function.g_WriteINI(Tf, "data", "9" + Tstr + "2", string.Format("{0:0.00}", Pq[Ti, 2]));
                Function.g_WriteINI(Tf, "data", "9" + Tstr + "3", string.Format("{0:0.00}", Pq[Ti, 3]));
                Function.g_WriteINI(Tf, "data", "9" + Tstr + "4", string.Format("{0:0.00}", Pq[Ti, 4]));
            }

            for (Tj = 0; Tj < 8; Tj++)
                for (Ti = 0; Ti < 5; Ti++)
                {
                    Function.g_WriteINI(Tf, "data", "A" + Interaction.Choose(Tj + 1, "01", "02", "11", "12", "13", "14", "15", "16") + string.Format("{0:0}", Ti), string.Format("{0:0.0000}", Zdxl[Tj, Ti]));
                    Function.g_WriteINI(Tf, "data", "B" + Interaction.Choose(Tj + 1, "01", "02", "11", "12", "13", "14", "15", "16") + string.Format("{0:0}", Ti), ZdxlTime[Tj, Ti].ToString("yy-MM-dd HH:mm:ss"));
                }

            Tstr = "";
            for (Tj = 0; Tj < 7; Tj++)
                //Tstr += B_ZT[Tj].PadLeft(4, '0');

                Function.g_WriteINI(Tf, "para", "B_ZT", Tstr);

            Function.g_WriteINI(Tf, "para", "ShiDuan", TShiDuan);
            Function.g_WriteINI(Tf, "para", "Feilei", TFeiLei);
            Function.g_WriteINI(Tf, "para", "LastBCtime", LastBCtime.ToString("yy-MM-dd HH:mm:ss"));
            Function.g_WriteINI(Tf, "para", "LastQLtime", LastQLtime.ToString("yy-MM-dd HH:mm:ss"));
            Function.g_WriteINI(Tf, "para", "QLCS", string.Format("{0:0}", QLCS));
            Function.g_WriteINI(Tf, "para", "BCCS", string.Format("{0:0}", BCCS));
            Function.g_WriteINI(Tf, "para", "DCWorkTime", string.Format("{0:0}", DCWorkTime));
            Function.g_WriteINI(Tf, "para", "DXCS", string.Format("{0:0}", DXCS));
        }

        public void LoadData()
        {
            string Tf;
            string[] Cl = new string[9];
            int Ti; int Tj;
            Tf = Application.StartupPath + "\\Ini\\MeterData\\meter" + index + ".txt";
            dtEventHappen = DateTime.Now;
            dtEventEnd = DateTime.Now.AddMinutes(5);
            Ua = double.Parse(Function.g_GetINI(Tf, "para", "Ua", "220"));
            Ub = double.Parse(Function.g_GetINI(Tf, "para", "Ub", "220"));
            Uc = double.Parse(Function.g_GetINI(Tf, "para", "Uc", "220"));
            Ia = double.Parse(Function.g_GetINI(Tf, "para", "Ia", "1.5"));
            Ib = double.Parse(Function.g_GetINI(Tf, "para", "Ib", "1.5"));
            Ic = double.Parse(Function.g_GetINI(Tf, "para", "Ic", "1.5"));

            Phia = double.Parse(Function.g_GetINI(Tf, "para", "Phia", "0"));
            Phib = double.Parse(Function.g_GetINI(Tf, "para", "Phib", "0"));
            Phic = double.Parse(Function.g_GetINI(Tf, "para", "Phic", "0"));

            Pq[0, 0] = double.Parse(Function.g_GetINI(Tf, "data", "9010", "0"));
            Pq[0, 1] = double.Parse(Function.g_GetINI(Tf, "data", "9011", "0"));
            Pq[0, 2] = double.Parse(Function.g_GetINI(Tf, "data", "9012", "0"));
            Pq[0, 3] = double.Parse(Function.g_GetINI(Tf, "data", "9013", "0"));
            Pq[0, 4] = double.Parse(Function.g_GetINI(Tf, "data", "9014", "0"));

            Pq[1, 0] = double.Parse(Function.g_GetINI(Tf, "data", "9020", "0"));
            Pq[1, 1] = double.Parse(Function.g_GetINI(Tf, "data", "9021", "0"));
            Pq[1, 2] = double.Parse(Function.g_GetINI(Tf, "data", "9022", "0"));
            Pq[1, 3] = double.Parse(Function.g_GetINI(Tf, "data", "9023", "0"));
            Pq[1, 4] = double.Parse(Function.g_GetINI(Tf, "data", "9024", "0"));

            Pq[2, 0] = double.Parse(Function.g_GetINI(Tf, "data", "9110", "0"));
            Pq[2, 1] = double.Parse(Function.g_GetINI(Tf, "data", "9111", "0"));
            Pq[2, 2] = double.Parse(Function.g_GetINI(Tf, "data", "9112", "0"));
            Pq[2, 3] = double.Parse(Function.g_GetINI(Tf, "data", "9113", "0"));
            Pq[2, 4] = double.Parse(Function.g_GetINI(Tf, "data", "9114", "0"));

            Pq[3, 0] = double.Parse(Function.g_GetINI(Tf, "data", "9120", "0"));
            Pq[3, 1] = double.Parse(Function.g_GetINI(Tf, "data", "9121", "0"));
            Pq[3, 2] = double.Parse(Function.g_GetINI(Tf, "data", "9122", "0"));
            Pq[3, 3] = double.Parse(Function.g_GetINI(Tf, "data", "9123", "0"));
            Pq[3, 4] = double.Parse(Function.g_GetINI(Tf, "data", "9124", "0"));

            Pq[4, 0] = double.Parse(Function.g_GetINI(Tf, "data", "9130", "0"));
            Pq[4, 1] = double.Parse(Function.g_GetINI(Tf, "data", "9131", "0"));
            Pq[4, 2] = double.Parse(Function.g_GetINI(Tf, "data", "9132", "0"));
            Pq[4, 3] = double.Parse(Function.g_GetINI(Tf, "data", "9133", "0"));
            Pq[4, 4] = double.Parse(Function.g_GetINI(Tf, "data", "9134", "0"));

            Pq[5, 0] = double.Parse(Function.g_GetINI(Tf, "data", "9140", "0"));
            Pq[5, 1] = double.Parse(Function.g_GetINI(Tf, "data", "9141", "0"));
            Pq[5, 2] = double.Parse(Function.g_GetINI(Tf, "data", "9142", "0"));
            Pq[5, 3] = double.Parse(Function.g_GetINI(Tf, "data", "9143", "0"));
            Pq[5, 4] = double.Parse(Function.g_GetINI(Tf, "data", "9144", "0"));

            Pq[6, 0] = double.Parse(Function.g_GetINI(Tf, "data", "9150", "0"));
            Pq[6, 1] = double.Parse(Function.g_GetINI(Tf, "data", "9151", "0"));
            Pq[6, 2] = double.Parse(Function.g_GetINI(Tf, "data", "9152", "0"));
            Pq[6, 3] = double.Parse(Function.g_GetINI(Tf, "data", "9153", "0"));
            Pq[6, 4] = double.Parse(Function.g_GetINI(Tf, "data", "9154", "0"));

            Pq[7, 0] = double.Parse(Function.g_GetINI(Tf, "data", "9160", "0"));
            Pq[7, 1] = double.Parse(Function.g_GetINI(Tf, "data", "9161", "0"));
            Pq[7, 2] = double.Parse(Function.g_GetINI(Tf, "data", "9162", "0"));
            Pq[7, 3] = double.Parse(Function.g_GetINI(Tf, "data", "9163", "0"));
            Pq[7, 4] = double.Parse(Function.g_GetINI(Tf, "data", "9164", "0"));

            for (Tj = 0; Tj < 8; Tj++)
                for (Ti = 0; Ti < 5; Ti++)
                {
                    Zdxl[Tj, Ti] = double.Parse(Function.g_GetINI(Tf, "data", "A" + Interaction.Choose(Tj + 1, "01", "02", "11", "12", "13", "14", "15", "16") + string.Format("{0:0}", Ti), "0"));
                    ZdxlTime[Tj, Ti] = DateTime.Parse(Function.g_GetINI(Tf, "data", "B" + Interaction.Choose(Tj + 1, "01", "02", "11", "12", "13", "14", "15", "16") + string.Format("{0:0}", Ti), DateTime.Now.ToString("yy-MM-dd HH:mm:ss")));
                }

            string TShiDuan = Function.g_GetINI(Tf, "para", "ShiDuan", "3:00,6:00,9:00,12:00,15:00,18:00,21:00,23:00");
            string TFeiLei = Function.g_GetINI(Tf, "para", "Feilei", "2,3,2,3,2,3,2,4");
            Cl = TShiDuan.Split(',');//split(tShiDuan, ",")     
            for (Ti = 0; Ti < 8; Ti++)
                ShiDuan[Ti] = DateTime.Parse(DateTime.Parse(Cl[Ti]).ToString("HH:mm"));

            Cl = TFeiLei.Split(',');//Split(FeiLei, ",")
            for (Ti = 0; Ti < 8; Ti++)
                FeiLei[Ti] = int.Parse(Cl[Ti]);




            LastBCtime = DateTime.Parse(Function.g_GetINI(Tf, "para", "LastBCtime", "05-05-05 05:05:05"));
            LastQLtime = DateTime.Parse(Function.g_GetINI(Tf, "para", "LastQLtime", "05-05-05 05:05:05"));
            QLCS = 8;//int.Parse(Function.g_GetINI(Tf, "para", "QLCS", "8"));
            BCCS = 8;//int.Parse(Function.g_GetINI(Tf, "para", "BCCS", "8"));
            DCWorkTime = int.Parse(Function.g_GetINI(Tf, "para", "DCWorkTime", "8"));
            DXCS = 8;//int.Parse(Function.g_GetINI(Tf, "para", "DXCS", "8"));
            DcZhenChang = int.Parse(Function.g_GetINI(Tf, "para", "DcZhenChang", "1"));

            for (int i = 0; i < 7; i++)
            {
                B_ZT[i] = "0000";
            }
        }

        public void Time_Run(int TInterval, double TXiShu)
        {
            const int CST_QWS = 3600000; //千瓦时
            const double CST_Pai = 3.1415926 / 180;


            double Tphia;
            double Tphib;
            double Tphic;
            //计算电能量
            int Ti; int Tj; int TFl; int Tz;

            TFl = 4;

            if (DateBron().Hour == 23 && DateBron().Minute == 59)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        ZdxlDj[i, j] = Zdxl[i, j];
                        PqDj[i, j] = Pq[i, j];
                    }
                }
            }

            //WriteLog("test", DateBron().ToString() + " " + ZdxlTime[0, 0].ToString());

            for (Tj = 0; Tj <= 6; Tj++)
            {
                ////Application.DoEvents();
                if (DateTime.Now >= ShiDuan[Tj] && DateTime.Now < ShiDuan[Tj + 1])
                {
                    TFl = FeiLei[Tj]; break;
                }
            }

            if (DateTime.Now >= ShiDuan[7] || DateTime.Now < ShiDuan[0])
                TFl = FeiLei[7];


            Tphia = Phia * CST_Pai; Tphib = Phib * CST_Pai; Tphic = Phic * CST_Pai;
            Sa = Ua * Ia * 0.001; Sb = Ub * Ib * 0.001; Sc = Uc * Ic * 0.001;
            Pa = Sa * Math.Cos(Tphia); Pb = Sb * Math.Cos(Tphib); Pc = Sc * Math.Cos(Tphic);

            Qa = Sa * Math.Sin(Tphia); Qb = Sb * Math.Sin(Tphib); Qc = Sc * Math.Sin(Tphic);

            for (Tj = 0; Tj <= 7; Tj++)
                for (Tz = 0; Tz <= 4; Tz++)
                {
                    ////Application.DoEvents();
                    if (ZdxlTime[Tj, Tz].Month != DateBron().Month || ZdxlTime[Tj, Tz].Year != DateBron().Year)
                    { Zdxl[Tj, Tz] = 0; ZdxlTime[Tj, Tz] = DateBron(); }
                }



            for (Tz = 0; Tz <= TFl; Tz = Tz + TFl)
                if (Zdxl[0, Tz] < Pa + Pb + Pc)
                { Zdxl[0, Tz] = Pa + Pb + Pc; ZdxlTime[0, Tz] = DateBron(); }


            for (Tz = 0; Tz <= TFl; Tz = Tz + TFl)
                if (Zdxl[1, Tz] < Math.Abs(Pa + Pb + Pc) && Pa + Pb + Pc < 0)
                { Zdxl[1, Tz] = Math.Abs(Pa + Pb + Pc); ZdxlTime[1, Tz] = DateBron(); }



            switch ((int)(Phia / 90))
            {
                case 0:
                    for (Tz = 0; Tz <= TFl; Tz = Tz + TFl)
                    {
                        if (Zdxl[4, Tz] < Math.Abs(Qa + Qb + Qc))
                        { Zdxl[4, Tz] = Math.Abs(Qa + Qb + Qc); ZdxlTime[4, Tz] = DateBron(); }
                        ////Application.DoEvents();
                    }
                    break;
                case 1:
                    for (Tz = 0; Tz <= TFl; Tz = Tz + TFl)
                    {
                        if (Zdxl[6, Tz] < Math.Abs(Qa + Qb + Qc))
                        { Zdxl[6, Tz] = Math.Abs(Qa + Qb + Qc); ZdxlTime[6, Tz] = DateBron(); }
                        ////Application.DoEvents();
                    }
                    break;
                case 2:
                    for (Tz = 0; Tz <= TFl; Tz = Tz + TFl)
                    {
                        if (Zdxl[7, Tz] < Math.Abs(Qa + Qb + Qc))
                        { Zdxl[7, Tz] = Math.Abs(Qa + Qb + Qc); ZdxlTime[7, Tz] = DateBron(); }
                        ////Application.DoEvents();
                    }
                    break;
                case 3:
                    for (Tz = 0; Tz <= TFl; Tz = Tz + TFl)
                    {
                        if (Zdxl[5, Tz] < Math.Abs(Qa + Qb + Qc))
                        { Zdxl[5, Tz] = Math.Abs(Qa + Qb + Qc); ZdxlTime[5, Tz] = DateBron(); }
                        ////Application.DoEvents();
                    }
                    break;
            }

            for (Tz = 0; Tz <= TFl; Tz = Tz + TFl)
            {
                ////Application.DoEvents();
                if (Zdxl[2, Tz] < Zdxl[4, Tz])
                { Zdxl[2, Tz] = Zdxl[4, Tz]; ZdxlTime[2, Tz] = DateBron(); }

                if (Zdxl[2, Tz] < Zdxl[5, Tz])
                { Zdxl[2, Tz] = Zdxl[5, Tz]; ZdxlTime[2, Tz] = DateBron(); }
            }
            for (Tz = 0; Tz <= TFl; Tz = Tz + TFl)
            {
                ////Application.DoEvents();
                if (Zdxl[3, Tz] < Zdxl[6, Tz])
                { Zdxl[3, Tz] = Zdxl[6, Tz]; ZdxlTime[3, Tz] = DateBron(); }

                if (Zdxl[3, Tz] < Zdxl[7, Tz])
                { Zdxl[3, Tz] = Zdxl[7, Tz]; ZdxlTime[3, Tz] = DateBron(); }

            }


            double Tdelta;
            double Tx;

            Tx = (double)TInterval / (double)CST_QWS;

            Tdelta = (Pa + Pb + Pc) * Tx * TXiShu;

            if (Tdelta > 0)
            {
                for (Tz = 0; Tz <= TFl; Tz = Tz + TFl)
                {
                    //Application.DoEvents();
                    Pq[0, Tz] = Pq[0, Tz] + Tdelta; //P+    
                }
            }

            else
            {
                for (Tz = 0; Tz <= TFl; Tz = Tz + TFl)
                {
                    //Application.DoEvents();
                    Pq[1, Tz] = Pq[1, Tz] + Math.Abs(Tdelta); //P-
                }
            }



            for (Tz = 0; Tz <= TFl; Tz = Tz + TFl)
            {
                //Application.DoEvents();
                if (0 <= Phia && Phia <= 90)
                    Pq[4, Tz] = Pq[4, Tz] + Math.Abs((Qa + Qb + Qc) * Tx * TXiShu);
                if (90 < Phia && Phia <= 180)
                    Pq[7, Tz] = Pq[7, Tz] + Math.Abs((Qa + Qb + Qc) * Tx * TXiShu);

                if (180 < Phia && Phia <= 270)
                    Pq[6, Tz] = Pq[6, Tz] + Math.Abs((Qa + Qb + Qc) * Tx * TXiShu);
                if (270 < Phia && Phia <= 360)
                    Pq[5, Tz] = Pq[5, Tz] + Math.Abs((Qa + Qb + Qc) * Tx * TXiShu);
            }


            for (Tz = 0; Tz <= 4; Tz++)
            {
                Pq[2, Tz] = Pq[4, Tz] + Pq[7, Tz];
                //Application.DoEvents();
            }


            for (Tz = 0; Tz <= 4; Tz++)
            {
                Pq[3, Tz] = Pq[6, Tz] + Pq[5, Tz];
                //Application.DoEvents();
            }

            for (Ti = 0; Ti <= 7; Ti++)
            {
                for (Tz = 0; Tz <= 4; Tz++)
                {
                    //Application.DoEvents();
                    { Pq[Ti, Tz] = Math.Round(Pq[Ti, Tz], 4); Zdxl[Ti, Tz] = Math.Round(Zdxl[Ti, Tz], 4); }
                }
            }


            if (0 <= Phia && Phia <= 90)
                BiaoZt = (byte)((DcZhenChang == 0) ? Math.Pow(2, 2) : 0);
            if (90 < Phia && Phia <= 180)
                BiaoZt = (byte)(Math.Pow(2, 4) + DcZhenChang == 0 ? Math.Pow(2, 2) : 0);
            if (180 < Phia && Phia <= 270)
                BiaoZt = (byte)(Math.Pow(2, 5) + Math.Pow(2, 4) + DcZhenChang == 0 ? Math.Pow(2, 2) : 0);
            if (270 < Phia && Phia <= 360)
                BiaoZt = (byte)(Math.Pow(2, 5) + DcZhenChang == 0 ? 4 : 0);

            //if (DcZhenChang == 0)
            //    B_ZT[0] = "000C";
            //else
            //    B_ZT[0] = "0000";

            if (Uc > 225)
                Wangzt = (byte)((Wangzt & (byte)Math.Pow(2, 6)) > 0 ? Wangzt : Wangzt + Math.Pow(2, 6));
            else
                Wangzt = (byte)((Wangzt & (byte)Math.Pow(2, 6)) > 0 ? Wangzt - Math.Pow(2, 6) : Wangzt);
            if (Ub > 225)
                Wangzt = (byte)((Wangzt & (byte)Math.Pow(2, 5)) > 0 ? Wangzt : Wangzt + Math.Pow(2, 5));
            else
                Wangzt = (byte)((Wangzt & (byte)Math.Pow(2, 5)) > 0 ? Wangzt - Math.Pow(2, 5) : Wangzt);
            if (Ua > 225)
                Wangzt = (byte)((Wangzt & (byte)Math.Pow(2, 4)) > 0 ? Wangzt : Wangzt + Math.Pow(2, 4));
            else
                Wangzt = (byte)((Wangzt & (byte)Math.Pow(2, 4)) > 0 ? Wangzt - Math.Pow(2, 4) : Wangzt);

            if (Ic == 0)
                Wangzt = (byte)((Wangzt & (byte)Math.Pow(2, 2)) > 0 ? Wangzt : Wangzt + Math.Pow(2, 2));
            else
                Wangzt = (byte)((Wangzt & (byte)Math.Pow(2, 2)) > 0 ? Wangzt - Math.Pow(2, 2) : Wangzt);
            if (Ib == 0)
                Wangzt = (byte)((Wangzt & (byte)Math.Pow(2, 1)) > 0 ? Wangzt : Wangzt + Math.Pow(2, 1));
            else
                Wangzt = (byte)((Wangzt & (byte)Math.Pow(2, 1)) > 0 ? Wangzt - Math.Pow(2, 1) : Wangzt);
            if (Ia == 0)
                Wangzt = (byte)((Wangzt & (byte)Math.Pow(2, 0)) > 0 ? Wangzt : Wangzt + Math.Pow(2, 0));
            else
                Wangzt = (byte)((Wangzt & (byte)Math.Pow(2, 0)) > 0 ? Wangzt - Math.Pow(2, 0) : Wangzt);
        }
        private static IPEndPoint IPRemotePoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5002);
        public string GetReturnApdu(string codeIn, string seasm, int seamtype)
        {
            MeterDataTime698 = DateBron();
            string sAPDU = "";
            string sCMD = codeIn.Substring(0, 4);
            string[] soad = new string[0];
            int soadlen = 0;
            string strPid = "01";
            if (sCMD == "0501")
            {
                strPid = codeIn.Substring(4, 2);
                soad = new string[1];

                for (int i = 0; i < soad.Length; i++)
                {
                    soad[i] = codeIn.Substring(6 + 8 * i, 8);
                }

                sAPDU = "8501" + strPid;

                for (int i = 0; i < soad.Length; i++)
                {
                    sAPDU += soad[i] + "01" + GetOadData(soad[i], seasm.Substring(4));
                }
                sAPDU += "0000";
            }
            else if (sCMD == "0502")
            {
                strPid = codeIn.Substring(4, 2);
                soadlen = Convert.ToInt16(codeIn.Substring(6, 2), 16);

                soad = new string[soadlen];

                for (int i = 0; i < soad.Length; i++)
                {
                    soad[i] = codeIn.Substring(8 + 8 * i, 8);
                }

                sAPDU = "8502" + strPid + soadlen.ToString("x2");

                for (int i = 0; i < soad.Length; i++)
                {
                    sAPDU += soad[i] + "01" + GetOadData(soad[i], seasm.Substring(4));
                }
                sAPDU += "0000";
            }
            else if (sCMD == "0503")
            {
                strPid = codeIn.Substring(4, 2);
                string djbz = codeIn.Substring(6, 8);//冻结标识
                string selectType = codeIn.Substring(14, 2);
                if (selectType == "01")
                {
                    string oad1 = codeIn.Substring(6, 8);
                    DateTime dtstart = Convert.ToDateTime(date_time_s(codeIn.Substring(26, 14)));
                    soadlen = Convert.ToInt16(codeIn.Substring(40, 2), 16);
                    soad = new string[soadlen];
                    for (int i = 0; i < soad.Length; i++)
                    {
                        soad[i] = codeIn.Substring(44 + 10 * i, 8);
                    }

                    sAPDU = "8503" + strPid + oad1 + soadlen.ToString("x2");

                    for (int i = 0; i < soad.Length; i++)
                    {
                        sAPDU += "00" + soad[i];
                    }
                    sAPDU += "0101";

                    for (int j = 0; j < soad.Length; j++)
                    {
                        MeterDataTime698 = dtstart;
                        sAPDU += GetOadData(soad[j], seasm.Substring(4));
                    }

                }
                else if (selectType == "02")
                {
                    string oad1 = codeIn.Substring(6, 8);
                    DateTime dtstart = Convert.ToDateTime(date_time_s(codeIn.Substring(26, 14)));
                    DateTime dtEnd = Convert.ToDateTime(date_time_s(codeIn.Substring(42, 14)));
                    string stimetype = codeIn.Substring(58, 2);
                    int timeinterval = Convert.ToInt32(codeIn.Substring(60, 4), 16);
                    if (oad1 == "50040200")
                    {
                        long datalen = 1;
                        soadlen = Convert.ToInt16(codeIn.Substring(64, 2), 16);
                        soad = new string[soadlen];
                        for (int i = 0; i < soad.Length; i++)
                        {
                            soad[i] = codeIn.Substring(68 + 10 * i, 8);
                        }

                        sAPDU = "8503" + strPid + oad1 + soadlen.ToString("x2");

                        for (int i = 0; i < soad.Length; i++)
                        {
                            sAPDU += "00" + soad[i];
                        }
                        sAPDU += "01" + datalen.ToString("x2");
                        for (int i = 0; i < datalen; i++)
                        {
                            for (int j = 0; j < soad.Length; j++)
                            {
                                MeterDataTime698 = Convert.ToDateTime(dtEnd.Year + "-" + dtEnd.Month + "-" + dtEnd.Day + " 00:00:00");
                                sAPDU += GetOadData(soad[j], seasm.Substring(4));
                            }
                        }
                    }
                    else if (oad1 == "50020200")
                    {
                        long datalen = 0;
                        if (((dtEnd.Ticks - dtstart.Ticks) / 10000000) / 60 % timeinterval == 0)
                            datalen = ((dtEnd.Ticks - dtstart.Ticks) / 10000000) / 60 / timeinterval;//czx
                        else
                            datalen = ((dtEnd.Ticks - dtstart.Ticks) / 10000000) / 60 / timeinterval + 1;//czx
                        soadlen = Convert.ToInt16(codeIn.Substring(64, 2), 16);
                        soad = new string[soadlen];
                        for (int i = 0; i < soad.Length; i++)
                        {
                            soad[i] = codeIn.Substring(68 + 10 * i, 8);
                        }

                        sAPDU = "8503" + strPid + oad1 + soadlen.ToString("x2");

                        for (int i = 0; i < soad.Length; i++)
                        {
                            sAPDU += "00" + soad[i];
                        }
                        sAPDU += "01" + datalen.ToString("x2");
                        for (int i = 0; i < datalen; i++)
                        {
                            for (int j = 0; j < soad.Length; j++)
                            {
                                MeterDataTime698 = dtstart.AddMinutes(i * 15);
                                sAPDU += GetOadData(soad[j], seasm.Substring(4));
                            }
                        }
                    }
                    else if (oad1 == "50060200")
                    {
                        long datalen = 1;
                        soadlen = Convert.ToInt16(codeIn.Substring(64, 2), 16);
                        soad = new string[soadlen];
                        for (int i = 0; i < soad.Length; i++)
                        {
                            soad[i] = codeIn.Substring(68 + 10 * i, 8);
                        }

                        sAPDU = "8503" + strPid + oad1 + soadlen.ToString("x2");

                        for (int i = 0; i < soad.Length; i++)
                        {
                            sAPDU += "00" + soad[i];
                        }
                        sAPDU += "01" + datalen.ToString("x2");
                        for (int i = 0; i < datalen; i++)
                        {
                            for (int j = 0; j < soad.Length; j++)
                            {
                                MeterDataTime698 = Convert.ToDateTime(dtEnd.Year + "-" + dtEnd.Month + "-1 00:00:00");
                                sAPDU += GetOadData(soad[j], seasm.Substring(4));
                            }
                        }
                    }
                    else
                    {
                        long datalen = 1;
                        soadlen = Convert.ToInt16(codeIn.Substring(64, 2), 16);
                        soad = new string[soadlen];
                        for (int i = 0; i < soad.Length; i++)
                        {
                            soad[i] = codeIn.Substring(68 + 10 * i, 8);
                        }

                        sAPDU = "8503" + strPid + oad1 + soadlen.ToString("x2");

                        for (int i = 0; i < soad.Length; i++)
                        {
                            sAPDU += "00" + soad[i];
                        }
                        sAPDU += "01" + datalen.ToString("x2");
                        for (int i = 0; i < datalen; i++)
                        {
                            for (int j = 0; j < soad.Length; j++)
                            {
                                MeterDataTime698 = dtEnd;
                                sAPDU += GetOadData(soad[j], seasm.Substring(4));
                            }
                        }
                    }
                }
                else if (selectType == "09")
                {
                    string oad1 = codeIn.Substring(6, 8);
                    //DateTime dtstart = Convert.ToDateTime(date_time_s(codeIn.Substring(26, 14)));
                    soadlen = Convert.ToInt16(codeIn.Substring(18, 2), 16);
                    soad = new string[soadlen];
                    for (int i = 0; i < soad.Length; i++)
                    {
                        soad[i] = codeIn.Substring(22 + 10 * i, 8);
                    }

                    sAPDU = "8503" + strPid + oad1 + soadlen.ToString("x2");

                    for (int i = 0; i < soad.Length; i++)
                    {
                        sAPDU += "00" + soad[i];
                    }
                    sAPDU += "0101";

                    DateTime dtTmp = DateBron();

                    if (oad1 == "50060200")
                    {
                        MeterDataTime698 = Convert.ToDateTime(dtTmp.Year + "-" + dtTmp.Month + "-1" + " 00:00:00");
                    }
                    else if (oad1 == "50040200")
                    {
                        MeterDataTime698 = Convert.ToDateTime(dtTmp.Year + "-" + dtTmp.Month + "-" + dtTmp.Day + " 00:00:00");
                    }

                    for (int j = 0; j < soad.Length; j++)
                    {
                        sAPDU += GetOadData(soad[j], seasm);
                    }
                }
                else
                {

                }
                sAPDU += "0000";
            }
            else if (sCMD == "1000")//安全请求：明文+随机数
            {
                int imingwenlen = Convert.ToInt16(codeIn.Substring(4, 2), 16);
                string smingwen = codeIn.Substring(6, imingwenlen * 2);
                string sRand = codeIn.Substring(10 + imingwenlen * 2, 32);
                string sTmp1 = GetReturnApdu(smingwen, seasm.Substring(4), seamtype);

                StringBuilder strbu1 = new StringBuilder(10000);
                StringBuilder strbu2 = new StringBuilder(10000);

                //string strbu1 = null;
                //string strbu2 = null;

                byte[] data = ASCIIEncoding.ASCII.GetBytes(1 + "|" + 1 + "|" + strEsam + "|" + sRand + "|" + sTmp1 + "|" + strbu1.ToString() + "|" + strbu2.ToString());

                int ret = 0;

                string mac = "12345678";
                //ret = EncryptionFunction698.iObj_Meter_Formal_GenReadData(0,1, strEsam, sRand, sTmp1, ref strbu1,ref strbu2);

                if (SocketUdpControl.isStartMac)
                {
                    mac = RetMac(data, 8000, 500);
                }

                if (mac == "12345678")
                {
                    ret = 1;
                }

                if (isErrMac || ret != 0)
                {
                    sAPDU = "9000" + GetMiWenLen(sTmp1) + sTmp1 + "010004" + "12345678";
                }
                else
                {
                    sAPDU = "9000" + GetMiWenLen(sTmp1) + sTmp1 + "010004" + mac;
                    //sAPDU = "9000" + GetMiWenLen(sTmp1) + sTmp1 + "010004" + "12345678";
                }

            }
            return sAPDU;
        }

        /// <summary>
        /// 发送参数，获取mac地址
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="MaxWaitSeconds">帧最大等待时间</param>
        /// <param name="WaitSecondsPerByte">字节最大等待时间</param>
        /// <returns></returns>
        private string RetMac(byte[] data, int MaxWaitSeconds, int WaitSecondsPerByte)
        {
            string MAC = "12345678";
            try
            {
                UdpClient Macserver = new UdpClient();
                Macserver.Send(data, data.Length, IPRemotePoint);//将数据发送到指定的终结点
                
                byte[] BytReceived = new byte[0];
                bool IsReveive = false;     //标志是否返回
                List<byte> RevItems = new List<byte>();     //接收的数据集合
                DateTime Dt;            //等待时间变量
                Dt = DateTime.Now;
                while (TimeSub(DateTime.Now, Dt) < MaxWaitSeconds)
                {
                    Thread.Sleep(1);
                    try
                    {
                        if (Macserver.Available > 0)
                        {
                            BytReceived = Macserver.Receive(ref IPRemotePoint);
                            IsReveive = true;
                            break;
                        }
                    }
                    catch
                    {
                        Macserver.Close();
                    }
                }
                byte[] BackData = new byte[0];
                //LogInfoHelper.WriteLog("Meter5", "RetMac():BackData.Length:" + BackData.Length + "\r\n");
                if (!IsReveive)
                {
                    data = new byte[0];
                }
                else
                {
                    RevItems.AddRange(BytReceived);
                    Dt = DateTime.Now;
                    while (TimeSub(DateTime.Now, Dt) < WaitSecondsPerByte)
                    {
                        if (Macserver.Available > 0)
                        {
                            BytReceived = Macserver.Receive(ref IPRemotePoint);
                            RevItems.AddRange(BytReceived);
                            Dt = DateTime.Now;
                        }
                    }
                    BackData = RevItems.ToArray();
                }
                Macserver.Close();
                
                if (BackData.Length > 0)
                {
                    MAC = Encoding.ASCII.GetString(BackData, 0, BackData.Length);
                }
                return MAC;
            }
            catch (Exception ex)
            {
                //LogInfoHelper.WriteLog("Meter5", ex.Message + "\r\n" + ex.ToString() + "\r\n" + "RetMac()");
                return "12345678";
            }


            ////Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));


            //if (BackData.Length > 0)
            //{
            //    MAC = Encoding.ASCII.GetString(BackData, 0, BackData.Length);
            //}

        }

        private long TimeSub(DateTime Time1, DateTime Time2)
        {
            TimeSpan tsSub = Time1.Subtract(Time2);
            return tsSub.Hours * 60 * 60 * 1000 + tsSub.Minutes * 60 * 1000 + tsSub.Seconds * 1000 + tsSub.Milliseconds;
        }

        public bool isErrMac = false;

        public string GetMiWenLen(string strData)
        {
            strData = strData.Replace(" ", "");
            string slen = ""; ;
            if (strData.Length / 2 > 127)
            {
                slen = "82" + (strData.Length / 2).ToString("x4");
            }
            else
                slen = (strData.Length / 2).ToString("x2");
            return slen;
        }

        public static string date_time_s(string strData)
        {
            int year, month, day, hour, minute, second;
            string strDateStr;

            int intStrLen;

            strData = strData.Trim();
            intStrLen = strData.Length;
            //判断是不是无效帧数据"ee ee.."
            if (strData.Substring(0, 4) == "EEEE")
            {
                return "99999.9";
            }
            else
            {
                try
                {
                    year = Convert.ToInt16(strData.Substring(0, 2), 16) * 256 + Convert.ToInt16(strData.Substring(2, 2), 16);
                    month = Convert.ToInt16(strData.Substring(4, 2), 16);
                    day = Convert.ToInt16(strData.Substring(6, 2), 16);
                    hour = Convert.ToInt16(strData.Substring(8, 2), 16);
                    minute = Convert.ToInt16(strData.Substring(10, 2), 16);
                    second = Convert.ToInt16(strData.Substring(12, 2), 16);

                    strDateStr = year.ToString("0000") + "-" + month.ToString("00") + "-" + day.ToString("00")
                                 + " " + hour.ToString("00") + ":" + minute.ToString("00") + ":" + second.ToString("00");
                    return strDateStr;

                }
                catch (Exception)
                {
                    return "2099-1-1";
                    //return strDefDate;
                }

            }
        }

        public string strEsam = "0000000000000001";
        public string CodeReturn698(string codeIn, string BiaoDz, int seamtype)
        {
            string sAddress = codeIn.Substring(10, 12);
            strEsam = Function.revStr(sAddress).PadLeft(16, '0');
            string strKehuAddr = codeIn.Substring(22, 2);
            string sAPDU = "";
            try
            {
                sAPDU = GetReturnApdu(codeIn.Substring(28), strEsam, seamtype);
            }
            catch
            {
                sAPDU = "";
            }

            string slen = (sAPDU.Length / 2 + 15).ToString("x4").Substring(2, 2) + (sAPDU.Length / 2 + 15).ToString("x4").Substring(0, 2);

            string sHFrame = slen + "c305" + sAddress + strKehuAddr;
            long checkhead = CheckCrc16(sHFrame, sHFrame.Length / 2);
            string sFrame = sHFrame + (checkhead & 0xff).ToString("x2") + ((checkhead >> 8) & 0xff).ToString("x2") + sAPDU;
            checkhead = CheckCrc16(sFrame, sFrame.Length / 2);

            sFrame = "68" + sFrame + (checkhead & 0xff).ToString("x2") + ((checkhead >> 8) & 0xff).ToString("x2") + "16";
            return sFrame;
        }


        public string GetOadData(string sOAd, string sAddress)
        {
            string sAPDU = "";
            switch (sOAd)
            {
                #region 电能类对象
                case "00000200"://组合有功电能
                    sAPDU += "0105" + "05" + getSingle698(Pq[0, 0] + Pq[1, 0], 2, 4) + "05" + getSingle698(Pq[0, 1] + Pq[1, 1], 2, 4) + "05" + getSingle698(Pq[0, 2] + Pq[1, 2], 2, 4) + "05" + getSingle698(Pq[0, 3] + Pq[1, 3], 2, 4) + "05" + getSingle698(Pq[0, 4] + Pq[1, 4], 2, 4);
                    break;
                case "00000201":
                    sAPDU += "05" + getSingle698(Pq[0, 0] + Pq[1, 0], 2, 4);
                    break;
                case "00100200"://正向有功电能
                    sAPDU += "0105" + "06" + getSingle698(Pq[0, 0], 2, 4) + "06" + getSingle698(Pq[0, 1], 2, 4) + "06" + getSingle698(Pq[0, 2], 2, 4) + "06" + getSingle698(Pq[0, 3], 2, 4) + "06" + getSingle698(Pq[0, 4], 2, 4);
                    break;
                case "00100201":
                    sAPDU += "06" + getSingle698(Pq[0, 0], 2, 4);
                    break;
                case "00200200"://反向有功电能
                    sAPDU += "0105" + "06" + getSingle698(Pq[1, 0], 2, 4) + "06" + getSingle698(Pq[1, 1], 2, 4) + "06" + getSingle698(Pq[1, 2], 2, 4) + "06" + getSingle698(Pq[1, 3], 2, 4) + "06" + getSingle698(Pq[1, 4], 2, 4);
                    break;
                case "00200201":
                    sAPDU += "06" + getSingle698(Pq[1, 0], 2, 4);
                    break;
                case "00300200"://组合无功1电能
                    sAPDU += "0105" + "05" + getSingle698(Pq[2, 0], 2, 4) + "05" + getSingle698(Pq[2, 1], 2, 4) + "05" + getSingle698(Pq[2, 2], 2, 4) + "05" + getSingle698(Pq[2, 3], 2, 4) + "05" + getSingle698(Pq[2, 4], 2, 4);
                    break;
                case "00300201":
                    sAPDU += "05" + getSingle698(Pq[2, 0], 2, 4);
                    break;
                case "00400200"://组合无功2电能
                    sAPDU += "0105" + "05" + getSingle698(Pq[3, 0], 2, 4) + "05" + getSingle698(Pq[3, 1], 2, 4) + "05" + getSingle698(Pq[3, 2], 2, 4) + "05" + getSingle698(Pq[3, 3], 2, 4) + "05" + getSingle698(Pq[3, 4], 2, 4);
                    break;
                case "00400201":
                    sAPDU += "05" + getSingle698(Pq[3, 0], 2, 4);
                    break;
                case "00500200"://第一象限无功电能
                    sAPDU += "0105" + "06" + getSingle698(Pq[4, 0], 2, 4) + "06" + getSingle698(Pq[4, 1], 2, 4) + "06" + getSingle698(Pq[4, 2], 2, 4) + "06" + getSingle698(Pq[4, 3], 2, 4) + "06" + getSingle698(Pq[4, 4], 2, 4);
                    break;
                case "00500201":
                    sAPDU += "06" + getSingle698(Pq[4, 0], 2, 4);
                    break;
                case "00600200"://第二象限无功电能
                    sAPDU += "0105" + "06" + getSingle698(Pq[5, 0], 2, 4) + "06" + getSingle698(Pq[5, 1], 2, 4) + "06" + getSingle698(Pq[5, 2], 2, 4) + "06" + getSingle698(Pq[5, 3], 2, 4) + "06" + getSingle698(Pq[5, 4], 2, 4);
                    break;
                case "00600201":
                    sAPDU += "06" + getSingle698(Pq[5, 0], 2, 4);
                    break;
                case "00700200"://第三象限无功电能
                    sAPDU += "0105" + "06" + getSingle698(Pq[6, 0], 2, 4) + "06" + getSingle698(Pq[6, 1], 2, 4) + "06" + getSingle698(Pq[6, 2], 2, 4) + "06" + getSingle698(Pq[6, 3], 2, 4) + "06" + getSingle698(Pq[6, 4], 2, 4);
                    break;
                case "00700201":
                    sAPDU += "06" + getSingle698(Pq[6, 0], 2, 4);
                    break;
                case "00800200"://第四象限无功电能
                    sAPDU += "0105" + "06" + getSingle698(Pq[7, 0], 2, 4) + "06" + getSingle698(Pq[7, 1], 2, 4) + "06" + getSingle698(Pq[7, 2], 2, 4) + "06" + getSingle698(Pq[7, 3], 2, 4) + "06" + getSingle698(Pq[7, 4], 2, 4);
                    break;
                case "00800201":
                    sAPDU += "06" + getSingle698(Pq[2, 0], 2, 4);
                    break;
                #endregion

                #region 需量类对象
                case "10100200"://正向有功需量
                    sAPDU += "0105" + "020206" + getSingle698(Zdxl[0, 0], 4, 4) + getDateTimeBCD(ZdxlTime[0, 0]) + "020206" + getSingle698(Zdxl[0, 1], 4, 4) + getDateTimeBCD(ZdxlTime[0, 1]) + "020206" + getSingle698(Zdxl[0, 2], 4, 4) + getDateTimeBCD(ZdxlTime[0, 2]) + "020206" + getSingle698(Zdxl[0, 3], 4, 4) + getDateTimeBCD(ZdxlTime[0, 3]) + "020206" + getSingle698(Zdxl[0, 4], 4, 4) + getDateTimeBCD(ZdxlTime[0, 4]);
                    break;
                case "10200200":
                    sAPDU += "0105" + "020206" + getSingle698(Zdxl[1, 0], 4, 4) + getDateTimeBCD(ZdxlTime[1, 0]) + "020206" + getSingle698(Zdxl[1, 1], 4, 4) + getDateTimeBCD(ZdxlTime[1, 1]) + "020206" + getSingle698(Zdxl[1, 2], 4, 4) + getDateTimeBCD(ZdxlTime[1, 2]) + "020206" + getSingle698(Zdxl[1, 3], 4, 4) + getDateTimeBCD(ZdxlTime[1, 3]) + "020206" + getSingle698(Zdxl[1, 4], 4, 4) + getDateTimeBCD(ZdxlTime[1, 4]);
                    break;
                case "10300200":
                    sAPDU += "0105" + "020205" + getSingle698(Zdxl[2, 0], 4, 4) + getDateTimeBCD(ZdxlTime[2, 0]) + "020205" + getSingle698(Zdxl[2, 1], 4, 4) + getDateTimeBCD(ZdxlTime[2, 1]) + "020205" + getSingle698(Zdxl[2, 2], 4, 4) + getDateTimeBCD(ZdxlTime[2, 2]) + "020205" + getSingle698(Zdxl[2, 3], 4, 4) + getDateTimeBCD(ZdxlTime[2, 3]) + "020205" + getSingle698(Zdxl[2, 4], 4, 4) + getDateTimeBCD(ZdxlTime[2, 4]);
                    break;
                case "10400200":
                    sAPDU += "0105" + "020205" + getSingle698(Zdxl[3, 0], 4, 4) + getDateTimeBCD(ZdxlTime[3, 0]) + "020205" + getSingle698(Zdxl[3, 1], 4, 4) + getDateTimeBCD(ZdxlTime[3, 1]) + "020205" + getSingle698(Zdxl[3, 2], 4, 4) + getDateTimeBCD(ZdxlTime[3, 2]) + "020205" + getSingle698(Zdxl[3, 3], 4, 4) + getDateTimeBCD(ZdxlTime[3, 3]) + "020205" + getSingle698(Zdxl[3, 4], 4, 4) + getDateTimeBCD(ZdxlTime[3, 4]);
                    break;
                case "10500200"://
                    sAPDU += "0105" + "020206" + getSingle698(Zdxl[4, 0], 4, 4) + getDateTimeBCD(ZdxlTime[4, 0]) + "020206" + getSingle698(Zdxl[4, 1], 4, 4) + getDateTimeBCD(ZdxlTime[4, 1]) + "020206" + getSingle698(Zdxl[4, 2], 4, 4) + getDateTimeBCD(ZdxlTime[4, 2]) + "020206" + getSingle698(Zdxl[4, 3], 4, 4) + getDateTimeBCD(ZdxlTime[4, 3]) + "020206" + getSingle698(Zdxl[4, 4], 4, 4) + getDateTimeBCD(ZdxlTime[4, 4]);
                    break;
                case "10600200"://
                    sAPDU += "0105" + "020206" + getSingle698(Zdxl[5, 0], 4, 4) + getDateTimeBCD(ZdxlTime[5, 0]) + "020206" + getSingle698(Zdxl[5, 1], 4, 4) + getDateTimeBCD(ZdxlTime[5, 1]) + "020206" + getSingle698(Zdxl[5, 2], 4, 4) + getDateTimeBCD(ZdxlTime[5, 2]) + "020206" + getSingle698(Zdxl[5, 3], 4, 4) + getDateTimeBCD(ZdxlTime[5, 3]) + "020206" + getSingle698(Zdxl[5, 4], 4, 4) + getDateTimeBCD(ZdxlTime[5, 4]);
                    break;
                case "10700200"://
                    sAPDU += "0105" + "020206" + getSingle698(Zdxl[6, 0], 4, 4) + getDateTimeBCD(ZdxlTime[6, 0]) + "020206" + getSingle698(Zdxl[6, 1], 4, 4) + getDateTimeBCD(ZdxlTime[6, 1]) + "020206" + getSingle698(Zdxl[6, 2], 4, 4) + getDateTimeBCD(ZdxlTime[6, 2]) + "020206" + getSingle698(Zdxl[6, 3], 4, 4) + getDateTimeBCD(ZdxlTime[6, 3]) + "020206" + getSingle698(Zdxl[6, 4], 4, 4) + getDateTimeBCD(ZdxlTime[6, 4]);
                    break;
                case "10800200"://
                    sAPDU += "0105" + "020206" + getSingle698(Zdxl[7, 0], 4, 4) + getDateTimeBCD(ZdxlTime[7, 0]) + "020206" + getSingle698(Zdxl[7, 1], 4, 4) + getDateTimeBCD(ZdxlTime[7, 1]) + "020206" + getSingle698(Zdxl[7, 2], 4, 4) + getDateTimeBCD(ZdxlTime[7, 2]) + "020206" + getSingle698(Zdxl[7, 3], 4, 4) + getDateTimeBCD(ZdxlTime[7, 3]) + "020206" + getSingle698(Zdxl[7, 4], 4, 4) + getDateTimeBCD(ZdxlTime[7, 4]);
                    break;
                #endregion

                #region 变量类对象
                case "20210200":
                    sAPDU += getDateTimeBCD(MeterDataTime698);
                    break;
                case "20000200"://电压
                    sAPDU += "0103" + "12" + getSingle698(Ua, 1, 2) + "12" + getSingle698(Ub, 1, 2) + "12" + getSingle698(Uc, 1, 2);
                    break;
                case "20000201":
                    sAPDU += "" + "12" + getSingle698(Ua, 1, 2);
                    break;
                case "20000202":
                    sAPDU += "" + "12" + getSingle698(Ub, 1, 2);
                    break;
                case "20000203":
                    sAPDU += "" + "12" + getSingle698(Uc, 1, 2);
                    break;
                case "20010200"://电流
                    sAPDU += "0103" + "05" + getSingle698(Ia, 3, 4) + "05" + getSingle698(Ib, 3, 4) + "05" + getSingle698(Ic, 3, 4);
                    break;
                case "20010201":
                    sAPDU += "" + "05" + getSingle698(Ia, 3, 4);
                    break;
                case "20010202":
                    sAPDU += "" + "05" + getSingle698(Ib, 3, 4);
                    break;
                case "20010203":
                    sAPDU += "" + "05" + getSingle698(Ic, 3, 4);
                    break;
                case "20040200"://有功功率
                    sAPDU += "0103" + "05" + getSingle698(Pa * 1000, 1, 4) + "05" + getSingle698(Pb * 1000, 1, 4) + "05" + getSingle698(Pc * 1000, 1, 4);
                    break;
                case "20040201":
                    sAPDU += "" + "05" + getSingle698((Pa + Pb + Pc) * 1000, 1, 4);
                    break;
                case "20040202":
                    sAPDU += "" + "05" + getSingle698(Pa * 1000, 1, 4);
                    break;
                case "20040203":
                    sAPDU += "" + "05" + getSingle698(Pb * 1000, 1, 4);
                    break;
                case "20040204":
                    sAPDU += "" + "05" + getSingle698(Pc * 1000, 1, 4);
                    break;
                case "20050200"://无功功率
                    sAPDU += "0103" + "05" + getSingle698(Qa * 1000, 1, 4) + "05" + getSingle698(Qb * 1000, 1, 4) + "05" + getSingle698(Qc * 1000, 1, 4);
                    break;
                case "20050201":
                    sAPDU += "" + "05" + getSingle698((Qa + Qb + Qc) * 1000, 1, 4);
                    break;
                case "20050202":
                    sAPDU += "" + "05" + getSingle698(Qa * 1000, 1, 4);
                    break;
                case "20050203":
                    sAPDU += "" + "05" + getSingle698(Qb * 1000, 1, 4);
                    break;
                case "20050204":
                    sAPDU += "" + "05" + getSingle698(Qc * 1000, 1, 4);
                    break;
                case "20060200"://视在功率
                    sAPDU += "0103" + "05" + getSingle698(Sa * 1000, 1, 4) + "05" + getSingle698(Sb * 1000, 1, 4) + "05" + getSingle698(Sc * 1000, 1, 4);
                    break;
                case "20060201":
                    sAPDU += "" + "05" + getSingle698((Sa + Sb + Sc) * 1000, 1, 4);
                    break;
                case "20060202":
                    sAPDU += "" + "05" + getSingle698(Sa * 1000, 1, 4);
                    break;
                case "20060203":
                    sAPDU += "" + "05" + getSingle698(Sb * 1000, 1, 4);
                    break;
                case "20060204":
                    sAPDU += "" + "05" + getSingle698(Sc * 1000, 1, 4);
                    break;
                case "200A0200"://功率因数
                    sAPDU += "0104" + "10" + getSingle698(0.99, 3, 2) + "10" + getSingle698(0.99, 3, 2) + "10" + getSingle698(0.99, 3, 2) + "10" + getSingle698(0.99, 3, 2);
                    break;
                case "200A0201":
                    sAPDU += "" + "10" + getSingle698(0.99, 3, 2);
                    break;
                case "200A0202":
                    sAPDU += "" + "10" + getSingle698(0.99, 3, 2);
                    break;
                case "200A0203":
                    sAPDU += "" + "10" + getSingle698(0.99, 3, 2);
                    break;
                case "200A0204":
                    sAPDU += "" + "10" + getSingle698(0.99, 3, 2);
                    break;
                case "200F0200"://电网频率
                    sAPDU += "" + "12" + getSingle698(50, 2, 2);
                    break;
                case "20100200"://表内温度
                    sAPDU += "" + "10" + getSingle698(25, 1, 2);
                    break;
                case "20110200"://时钟电池电压
                    sAPDU += "" + "12" + getSingle698(5, 2, 2);
                    break;
                case "20120200"://停电抄表电池电压
                    sAPDU += "" + "12" + getSingle698(5, 2, 2);
                    break;
                case "20130200"://时钟电池工作时间
                    sAPDU += "" + "06" + getSingle698(500, 2, 4);
                    break;
                case "20140200"://电能表运行状态字
                    sAPDU += "0107" + "0410" + "AAAA" + "0410" + "AAAA" + "0410" + "AAAA" + "0410" + "AAAA" + "0410" + "AAAA" + "0410" + "AAAA" + "0410" + "AAAA";
                    break;
                #endregion

                #region 事件类对象
                case "30150700"://电能表事件清零事件，属性7
                    sAPDU += "01010202000202" + "06" + getSingle698(1008, 0, 4) + "06" + getSingle698(1008, 0, 4);
                    break;
                case "30170700"://电能表时段表编程事件，属性7
                    sAPDU += "01010202000202" + "06" + getSingle698(1008, 0, 4) + "06" + getSingle698(1008, 0, 4);
                    break;
                case "30030A00"://电能表断相事件，当前值记录表
                    sAPDU += "01030202" + "06" + getSingle698(1008, 0, 4) + "06" + getSingle698(1008, 0, 4) + "0202" + "06" + getSingle698(1008, 0, 4) + "06" + getSingle698(1008, 0, 4) + "0202" + "06" + getSingle698(1008, 0, 4) + "06" + getSingle698(1008, 0, 4);
                    break;
                case "302B0700"://电能表断相事件，当前值记录表
                    sAPDU += "01010202000202" + "06" + getSingle698(1, 0, 4) + "06" + getSingle698(10, 0, 4);
                    break;
                #endregion

                #region 参变类对象
                case "40000200"://日期时间
                    sAPDU += "" + getDateTimeBCD(DateBron());
                    break;
                case "40000100"://通信地址
                    sAPDU += "" + "0906" + "000000000001";
                    break;
                case "41120200"://有功状态字
                    sAPDU += "" + "0408" + YouGong_ZT;
                    break;
                #endregion

                case "20220200":
                    sAPDU += "06" + getSingle698(1, 0, 4);
                    break;
                case "201E0200":
                    sAPDU += "" + getDateTimeBCD(dtEventHappen);
                    break;
                case "20200200":
                    sAPDU += "" + getDateTimeBCD(dtEventEnd);
                    break;
                case "F1010200":
                    sAPDU += "1601";
                    break;
                case "40010200":
                case "40020200":
                    sAPDU += "0906" + sAddress;
                    break;
                default:
                    if (sOAd.Contains("0700"))
                    {
                        sAPDU += "01010202000202" + "06" + getSingle698(1, 0, 4) + "06" + getSingle698(10, 0, 4);
                    }
                    //WriteLog("err", sOAd);
                    break;
            }

            return sAPDU;
        }

        public string CodeReturn2007(string CodeIn, string BiaoDz)
        {
            try
            {
                string str2 = "";
                double num;
                string str5;
                int num4;
                int num5;
                int num8;
                int num9;
                int pbyte = 0;
                int pdot = 0;
                string tstrAddr = "";
                double num6 = Math.Pow((Pa + Pb) + Pc, 2.0);
                double num7 = Math.Pow((Qa + Qb) + Qc, 2.0);
                num6 = Math.Pow(num6 + num7, 0.5);
                string str6 = "";
                if (!XiaoYan(CodeIn))
                {
                    return "FF";
                }
                if ((((((Pa + Pb) + Pc) == 0.0) && (Ia != 0.0)) && (Ib != 0.0)) && (Ic != 0.0))
                {
                    num = 0.0;
                }
                else if (((Ia == 0.0) && (Ib == 0.0)) && (Ic == 0.0))
                {
                    num = 1.0;
                }
                else
                {
                    num = ((Pa + Pb) + Pc) / num6;
                }
                tstrAddr = CodeIn.Substring(2, 12);
                if (CodeIn.Substring(0x10, 2) == "12")
                {
                    this.Tseq = CodeIn.Substring(CodeIn.Length - 6, 2);
                    return "92";
                }
                if (CodeIn.Substring(0x10, 2) == "11")
                {
                    str2 = CodeIn.Substring(0, 0x10) + "91";
                }
                if (CodeIn.Substring(0x10, 2) == "1C")
                {

                    str2 = CodeIn.Substring(0, 0x10) + "91";
                }
                else
                {
                    if (CodeIn.Substring(0x10, 2) == "13")
                    {
                        if ((tstrAddr.ToUpper().IndexOf("AAAAAAAAAAAA") >= 0) || (tstrAddr.IndexOf("999999999999") >= 0))
                        {
                            BiaoDz = BiaoDz.PadLeft(12, '0');
                            tstrAddr = BiaoDz.Substring(10, 2) + BiaoDz.Substring(8, 2) + BiaoDz.Substring(6, 2) + BiaoDz.Substring(4, 2) + BiaoDz.Substring(2, 2) + BiaoDz.Substring(0, 2);
                            str2 = "68" + tstrAddr + "689306" + tstrAddr;
                        }
                        else
                        {
                            str2 = CodeIn.Substring(0, 0x10) + "9306" + Add33H(tstrAddr);
                        }
                        return (str2 + getChk(str2) + "16");
                    }
                    if (CodeIn.Substring(0x10, 2).ToUpper() == "1F")
                    {
                        BiaoDz = BiaoDz.PadLeft(12, '0');
                        tstrAddr = BiaoDz.Substring(10, 2) + BiaoDz.Substring(8, 2) + BiaoDz.Substring(6, 2) + BiaoDz.Substring(4, 2) + BiaoDz.Substring(2, 2) + BiaoDz.Substring(0, 2);
                        str2 = "68" + tstrAddr + "689F08E235" + Add33H(tstrAddr);
                        return (str2 + getChk(str2) + "16");
                    }

                    if (CodeIn.Substring(0x10, 2).ToUpper() == "9F")
                    {
                        return "";
                    }
                    //str2 = CodeIn.Substring(0, 0x10) + "D1";
                }

                string pstr = string.Format("{0:X2}", ((Convert.ToByte(CodeIn.Substring(0x1a, 2), 16) + 0x100) - 0x33) % 0x100) + string.Format("{0:X2}", ((Convert.ToByte(CodeIn.Substring(0x18, 2), 16) + 0x100) - 0x33) % 0x100) + string.Format("{0:X2}", ((Convert.ToByte(CodeIn.Substring(0x16, 2), 16) + 0x100) - 0x33) % 0x100);
                string s = string.Format("{0:X2}", ((Convert.ToByte(CodeIn.Substring(20, 2), 16) + 0x100) - 0x33) % 0x100);
                string str11 = pstr;
                switch (str11)
                {
                    case "000000":
                        str2 = str2 + "08" + Add33H(s + "000000" + getSingle((s == "00") ? (Pq[0, 0] + Pq[1, 0]) : (PqDj[0, 0] + PqDj[1, 0]), 2, 4));
                        break;

                    case "000001":
                        str2 = str2 + "08" + Add33H(s + "010000" + getSingle((s == "00") ? (Pq[0, 1] + Pq[1, 1]) : (PqDj[0, 1] + PqDj[1, 1]), 2, 4));
                        break;

                    case "000002":
                        str2 = str2 + "08" + Add33H(s + "020000" + getSingle((s == "00") ? (Pq[0, 2] + Pq[1, 2]) : (PqDj[0, 2] + PqDj[1, 2]), 2, 4));
                        break;

                    case "000003":
                        str2 = str2 + "08" + Add33H(s + "030000" + getSingle((s == "00") ? (Pq[0, 3] + Pq[1, 3]) : (PqDj[0, 3] + PqDj[1, 3]), 2, 4));
                        break;

                    case "000004":
                        str2 = str2 + "08" + Add33H(s + "040000" + getSingle((s == "00") ? (Pq[0, 4] + Pq[1, 4]) : (PqDj[0, 4] + PqDj[1, 4]), 2, 4));
                        break;

                    case "0000FF":
                        str2 = str2 + "18" + Add33H(s + "FF0000" + getSingle((s == "00") ? (Pq[0, 0] + Pq[1, 0]) : (PqDj[0, 0] + PqDj[1, 0]), 2, 4));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle((s == "00") ? (Pq[0, num5] + Pq[1, num5]) : (PqDj[0, num5] + PqDj[1, num5]), 2, 4));
                        }
                        break;

                    case "000100":
                        str2 = str2 + "08" + Add33H(s + "000100" + getSingle((s == "00") ? Pq[0, 0] * DnXiShu : PqDj[0, 0], 2, 4));
                        break;

                    case "000101":
                        str2 = str2 + "08" + Add33H(s + "010100" + getSingle((s == "00") ? Pq[0, 1] : PqDj[0, 1], 2, 4));
                        break;

                    case "000102":
                        str2 = str2 + "08" + Add33H(s + "020100" + getSingle((s == "00") ? Pq[0, 2] : PqDj[0, 2], 2, 4));
                        break;

                    case "000103":
                        str2 = str2 + "08" + Add33H(s + "030100" + getSingle((s == "00") ? Pq[0, 3] : PqDj[0, 3], 2, 4));
                        break;

                    case "000104":
                        str2 = str2 + "08" + Add33H(s + "040100" + getSingle((s == "00") ? Pq[0, 4] : PqDj[0, 4], 2, 4));
                        break;

                    case "0001FF":
                        str2 = str2 + "18" + Add33H(s + "FF0100" + getSingle((s == "00") ? Pq[0, 0] * DnXiShu : PqDj[0, 0], 2, 4));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle((s == "00") ? Pq[0, num5] * DnXiShu : PqDj[0, num5], 2, 4));
                        }
                        break;

                    case "000200":
                        str2 = str2 + "08" + Add33H(s + "000200" + getSingle((s == "00") ? Pq[1, 0] : PqDj[1, 0], 2, 4));
                        break;

                    case "000201":
                        str2 = str2 + "08" + Add33H(s + "010200" + getSingle((s == "00") ? Pq[1, 1] : PqDj[1, 1], 2, 4));
                        break;

                    case "000202":
                        str2 = str2 + "08" + Add33H(s + "020200" + getSingle((s == "00") ? Pq[1, 2] : PqDj[1, 2], 2, 4));
                        break;

                    case "000203":
                        str2 = str2 + "08" + Add33H(s + "030200" + getSingle((s == "00") ? Pq[1, 3] : PqDj[1, 3], 2, 4));
                        break;

                    case "000204":
                        str2 = str2 + "08" + Add33H(s + "040200" + getSingle((s == "00") ? Pq[1, 4] : PqDj[1, 4], 2, 4));
                        break;

                    case "0002FF":

                        str2 = str2 + "18" + Add33H(s + "FF0200" + getSingle((s == "00") ? Pq[1, 0] : PqDj[1, 0], 2, 4));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle((s == "00") ? Pq[1, num5] : PqDj[1, num5], 2, 4));
                        }

                        break;

                    case "000300":

                        str2 = str2 + "08" + Add33H(s + "000300" + getSingle((s == "00") ? Pq[2, 0] : PqDj[2, 0], 2, 4));

                        break;

                    case "000301":

                        str2 = str2 + "08" + Add33H(s + "010300" + getSingle((s == "00") ? Pq[2, 1] : PqDj[2, 1], 2, 4));

                        break;

                    case "000302":

                        str2 = str2 + "08" + Add33H(s + "020300" + getSingle((s == "00") ? Pq[2, 2] : PqDj[2, 2], 2, 4));

                        break;

                    case "000303":

                        str2 = str2 + "08" + Add33H(s + "030300" + getSingle((s == "00") ? Pq[2, 3] : PqDj[2, 3], 2, 4));

                        break;

                    case "000304":

                        str2 = str2 + "08" + Add33H(s + "040300" + getSingle((s == "00") ? Pq[2, 4] : PqDj[2, 4], 2, 4));

                        break;

                    case "0003FF":

                        str2 = str2 + "18" + Add33H(s + "FF0300" + getSingle((s == "00") ? Pq[2, 0] : PqDj[2, 0], 2, 4));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle((s == "00") ? Pq[2, num5] : PqDj[2, num5], 2, 4));
                        }


                        break;

                    case "000400":
                        str2 = str2 + "08" + Add33H(s + "000400" + getSingle((s == "00") ? Pq[3, 0] : PqDj[3, 0], 2, 4));
                        break;

                    case "000401":
                        str2 = str2 + "08" + Add33H(s + "010400" + getSingle((s == "00") ? Pq[3, 1] : PqDj[3, 1], 2, 4));
                        break;

                    case "000402":
                        str2 = str2 + "08" + Add33H(s + "020400" + getSingle((s == "00") ? Pq[3, 2] : PqDj[3, 2], 2, 4));
                        break;

                    case "000403":
                        str2 = str2 + "08" + Add33H(s + "030400" + getSingle((s == "00") ? Pq[3, 3] : PqDj[3, 3], 2, 4));
                        break;

                    case "000404":
                        str2 = str2 + "08" + Add33H(s + "040400" + getSingle((s == "00") ? Pq[3, 4] : PqDj[3, 4], 2, 4));
                        break;

                    case "0004FF":
                        str2 = str2 + "18" + Add33H(s + "FF0400" + getSingle((s == "00") ? Pq[3, 0] : PqDj[3, 0], 2, 4));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle((s == "00") ? Pq[3, num5] : PqDj[3, num5], 2, 4));
                        }

                        break;

                    case "000500":
                        str2 = str2 + "08" + Add33H(s + "000500" + getSingle((s == "00") ? Pq[4, 0] : PqDj[4, 0], 2, 4));
                        break;

                    case "000501":
                        str2 = str2 + "08" + Add33H(s + "010500" + getSingle((s == "00") ? Pq[4, 1] : PqDj[4, 0], 2, 4));
                        break;

                    case "000502":
                        str2 = str2 + "08" + Add33H(s + "020500" + getSingle((s == "00") ? Pq[4, 2] : PqDj[4, 0], 2, 4));
                        break;

                    case "000503":
                        str2 = str2 + "08" + Add33H(s + "030500" + getSingle((s == "00") ? Pq[4, 3] : PqDj[4, 0], 2, 4));
                        break;

                    case "000504":
                        str2 = str2 + "08" + Add33H(s + "040500" + getSingle((s == "00") ? Pq[4, 4] : PqDj[4, 0], 2, 4));
                        break;

                    case "0005FF":

                        str2 = str2 + "18" + Add33H(s + "FF0500" + getSingle((s == "00") ? Pq[4, 0] : PqDj[4, 0], 2, 4));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle((s == "00") ? Pq[4, num5] : PqDj[4, num5], 2, 4));
                        }

                        break;

                    case "000600":
                        str2 = str2 + "08" + Add33H(s + "000600" + getSingle((s == "00") ? Pq[7, 0] : PqDj[7, 0], 2, 4));
                        break;

                    case "000601":
                        str2 = str2 + "08" + Add33H(s + "010600" + getSingle((s == "00") ? Pq[7, 1] : PqDj[7, 1], 2, 4));
                        break;

                    case "000602":
                        str2 = str2 + "08" + Add33H(s + "020600" + getSingle((s == "00") ? Pq[7, 2] : PqDj[7, 2], 2, 4));
                        break;

                    case "000603":
                        str2 = str2 + "08" + Add33H(s + "030600" + getSingle((s == "00") ? Pq[7, 3] : PqDj[7, 3], 2, 4));
                        break;

                    case "000604":
                        str2 = str2 + "08" + Add33H(s + "040600" + getSingle((s == "00") ? Pq[7, 4] : PqDj[7, 4], 2, 4));
                        break;

                    case "0006FF":
                        str2 = str2 + "18" + Add33H(s + "FF0600" + getSingle((s == "00") ? Pq[7, 0] : PqDj[7, 0], 2, 4));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle((s == "00") ? Pq[7, num5] : PqDj[7, num5], 2, 4));
                        }
                        break;

                    case "000700":
                        str2 = str2 + "08" + Add33H(s + "000700" + getSingle((s == "00") ? Pq[5, 0] : PqDj[5, 0], 2, 4));
                        break;

                    case "000701":
                        str2 = str2 + "08" + Add33H(s + "010700" + getSingle((s == "00") ? Pq[5, 1] : PqDj[5, 1], 2, 4));
                        break;

                    case "000702":
                        str2 = str2 + "08" + Add33H(s + "020700" + getSingle((s == "00") ? Pq[5, 2] : PqDj[5, 2], 2, 4));
                        break;

                    case "000703":
                        str2 = str2 + "08" + Add33H(s + "030700" + getSingle((s == "00") ? Pq[5, 3] : PqDj[5, 3], 2, 4));
                        break;

                    case "000704":
                        str2 = str2 + "08" + Add33H(s + "040700" + getSingle((s == "00") ? Pq[5, 4] : PqDj[5, 4], 2, 4));
                        break;

                    case "0007FF":

                        str2 = str2 + "18" + Add33H(s + "FF0700" + getSingle((s == "00") ? Pq[5, 0] : PqDj[5, 0], 2, 4));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle((s == "00") ? Pq[5, num5] : PqDj[5, num5], 2, 4));
                        }

                        break;

                    case "000800":
                        str2 = str2 + "08" + Add33H(s + "000800" + getSingle((s == "00") ? Pq[6, 0] : PqDj[6, 0], 2, 4));
                        break;

                    case "000801":
                        str2 = str2 + "08" + Add33H(s + "010800" + getSingle((s == "00") ? Pq[6, 1] : PqDj[6, 1], 2, 4));
                        break;

                    case "000802":
                        str2 = str2 + "08" + Add33H(s + "020800" + getSingle((s == "00") ? Pq[6, 2] : PqDj[6, 2], 2, 4));
                        break;

                    case "000803":
                        str2 = str2 + "08" + Add33H(s + "030800" + getSingle((s == "00") ? Pq[6, 3] : PqDj[6, 3], 2, 4));
                        break;

                    case "000804":
                        str2 = str2 + "08" + Add33H(s + "040800" + getSingle((s == "00") ? Pq[6, 4] : PqDj[6, 4], 2, 4));
                        break;

                    case "0008FF":

                        str2 = str2 + "18" + Add33H(s + "FF0800" + getSingle((s == "00") ? Pq[6, 0] : PqDj[6, 0], 2, 4));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle((s == "00") ? Pq[6, num5] : PqDj[6, num5], 2, 4));
                        }

                        break;

                    case "000900":
                        str2 = str2 + "08" + Add33H(s + "000900" + getSingle(Pq[0, 0] + 100.0, 2, 4));
                        break;

                    case "000901":
                        str2 = str2 + "08" + Add33H(s + "010900" + getSingle(Pq[0, 1] + 25.0, 2, 4));
                        break;

                    case "000902":
                        str2 = str2 + "08" + Add33H(s + "020900" + getSingle(Pq[0, 2] + 25.0, 2, 4));
                        break;

                    case "000903":
                        str2 = str2 + "08" + Add33H(s + "030900" + getSingle(Pq[0, 3] + 25.0, 2, 4));
                        break;

                    case "000904":
                        str2 = str2 + "08" + Add33H(s + "040900" + getSingle(Pq[0, 4] + 25.0, 2, 4));
                        break;

                    case "0009FF":
                        str2 = str2 + "18" + Add33H(s + "FF0900" + getSingle(Pq[0, 0] + 100.0, 2, 4));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle(Pq[0, num5] + 25.0, 2, 4));
                        }
                        break;

                    case "000A00":
                        str2 = str2 + "08" + Add33H(s + "000A00" + getSingle(Pq[1, 0] + 100.0, 2, 4));
                        break;

                    case "000A01":
                        str2 = str2 + "08" + Add33H(s + "010A00" + getSingle(Pq[1, 1] + 25.0, 2, 4));
                        break;

                    case "000A02":
                        str2 = str2 + "08" + Add33H(s + "020A00" + getSingle(Pq[1, 2] + 25.0, 2, 4));
                        break;

                    case "000A03":
                        str2 = str2 + "08" + Add33H(s + "030A00" + getSingle(Pq[1, 3] + 25.0, 2, 4));
                        break;

                    case "000A04":
                        str2 = str2 + "08" + Add33H(s + "040A00" + getSingle(Pq[1, 4] + 25.0, 2, 4));
                        break;

                    case "000AFF":
                        str2 = str2 + "18" + Add33H(s + "FF0A00" + getSingle(Pq[1, 0] + 100.0, 2, 4));
                        num5 = 1;
                        while (num5 <= 4)
                        {
                            str2 = str2 + Add33H(getSingle(Pq[1, num5] + 25.0, 2, 4));
                            num5++;
                        }
                        break;

                    case "008000":
                        str2 = str2 + "08" + Add33H(s + "008000" + getSingle(10.0, 2, 4));
                        break;

                    case "008100":
                        str2 = str2 + "08" + Add33H(s + "008100" + getSingle(10.0, 2, 4));
                        break;

                    case "008200":
                        str2 = str2 + "08" + Add33H(s + "008200" + getSingle(10.0, 2, 4));
                        break;

                    case "008300":
                        str2 = str2 + "08" + Add33H(s + "008300" + getSingle(10.0, 2, 4));
                        break;

                    case "008400":
                        str2 = str2 + "08" + Add33H(s + "008400" + getSingle(10.0, 2, 4));
                        break;

                    case "008500":
                        str2 = str2 + "08" + Add33H(s + "008500" + getSingle(100.0, 2, 4));
                        break;

                    case "008600":
                        str2 = str2 + "08" + Add33H(s + "008600" + getSingle(100.0, 2, 4));
                        break;

                    case "001500":
                        str2 = str2 + "08" + Add33H(s + "001500" + getSingle((s == "00") ? (Pq[0, 0] / 3.0) : 4.1133333333333333, 2, 4));
                        break;

                    case "001600":
                        str2 = str2 + "08" + Add33H(s + "001600" + getSingle((s == "00") ? (Pq[1, 0] / 3.0) : 0.39999999999999997, 2, 4));
                        break;

                    case "001700":
                        str2 = str2 + "08" + Add33H(s + "001700" + getSingle((s == "00") ? (Pq[2, 0] / 3.0) : 0.43333333333333335, 2, 4));
                        break;

                    case "001800":
                        str2 = str2 + "08" + Add33H(s + "001800" + getSingle((s == "00") ? (Pq[3, 0] / 3.0) : 0.46666666666666662, 2, 4));
                        break;

                    case "001900":
                        str2 = str2 + "08" + Add33H(s + "001900" + getSingle((s == "00") ? (Pq[4, 0] / 3.0) : 0.5, 2, 4));
                        break;

                    case "001A00":
                        str2 = str2 + "08" + Add33H(s + "001A00" + getSingle((s == "00") ? (Pq[5, 0] / 3.0) : 0.53333333333333333, 2, 4));
                        break;

                    case "001B00":
                        str2 = str2 + "08" + Add33H(s + "001B00" + getSingle((s == "00") ? (Pq[6, 0] / 3.0) : 0.56666666666666665, 2, 4));
                        break;

                    case "001C00":
                        str2 = str2 + "08" + Add33H(s + "001C00" + getSingle((s == "00") ? (Pq[7, 0] / 3.0) : 0.6, 2, 4));
                        break;

                    case "001D00":
                        str2 = str2 + "08" + Add33H(s + "001D00" + getSingle((Pq[0, 0] + 100.0) / 3.0, 2, 4));
                        break;

                    case "001E00":
                        str2 = str2 + "08" + Add33H(s + "001E00" + getSingle((Pq[1, 0] + 100.0) / 3.0, 2, 4));
                        break;

                    case "009001":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "08" + Add33H(s + "019000" + getSingle(20.0, 2, 4));
                        }
                        else
                        {
                            str2 = str2 + "08" + Add33H(s + "019000" + getSingle(150.0, 2, 4));
                        }
                        break;

                    case "009002":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "08" + Add33H(s + "029000" + getSingle(10.0, 2, 4));
                        }
                        else
                        {
                            str2 = str2 + "08" + Add33H(s + "029000" + getSingle(100.0, 2, 4));
                        }
                        break;

                    case "009400":
                        str2 = str2 + "08" + Add33H(s + "009400" + getSingle(100.0, 2, 4));
                        break;

                    case "009500":
                        str2 = str2 + "08" + Add33H(s + "009500" + getSingle(100.0, 2, 4));
                        break;

                    case "009600":
                        str2 = str2 + "08" + Add33H(s + "009600" + getSingle(100.0, 2, 4));
                        break;

                    case "009700":
                        str2 = str2 + "08" + Add33H(s + "009700" + getSingle(100.0, 2, 4));
                        break;

                    case "009800":
                        str2 = str2 + "08" + Add33H(s + "009800" + getSingle(100.0, 2, 4));
                        break;

                    case "009900":
                        str2 = str2 + "08" + Add33H(s + "009900" + getSingle(100.0, 2, 4));
                        break;

                    case "009A00":
                        str2 = str2 + "08" + Add33H(s + "009A00" + getSingle(100.0, 2, 4));
                        break;

                    case "002900":
                        str2 = str2 + "08" + Add33H(s + "002900" + getSingle((s == "00") ? (Pq[0, 0] / 3.0) : 4.1133333333333333, 2, 4));
                        break;

                    case "002A00":
                        str2 = str2 + "08" + Add33H(s + "002A00" + getSingle((s == "00") ? (Pq[1, 0] / 3.0) : 0.39999999999999997, 2, 4));
                        break;

                    case "002B00":
                        str2 = str2 + "08" + Add33H(s + "002B00" + getSingle((s == "00") ? (Pq[2, 0] / 3.0) : 0.43333333333333335, 2, 4));
                        break;

                    case "002C00":
                        str2 = str2 + "08" + Add33H(s + "002C00" + getSingle((s == "00") ? (Pq[4, 0] / 3.0) : 0.5, 2, 4));
                        break;

                    case "002D00":
                        str2 = str2 + "08" + Add33H(s + "002D00" + getSingle((s == "00") ? (Pq[5, 0] / 3.0) : 0.53333333333333333, 2, 4));
                        break;

                    case "002E00":
                        str2 = str2 + "08" + Add33H(s + "002E00" + getSingle((s == "00") ? (Pq[6, 0] / 3.0) : 0.56666666666666665, 2, 4));
                        break;

                    case "002F00":
                        str2 = str2 + "08" + Add33H(s + "002F00" + getSingle((s == "00") ? (Pq[7, 0] / 3.0) : 0.6, 2, 4));
                        break;

                    case "003000":
                        str2 = str2 + "08" + Add33H(s + "003000" + getSingle((Pq[0, 0] + 100.0) / 3.0, 2, 4));
                        break;

                    case "003100":
                        str2 = str2 + "08" + Add33H(s + "003100" + getSingle((Pq[1, 0] + 100.0) / 3.0, 2, 4));
                        break;

                    case "003200":
                        str2 = str2 + "08" + Add33H(s + "003200" + getSingle(200.0, 2, 4));
                        break;

                    case "00A800":
                        str2 = str2 + "08" + Add33H(s + "00A800" + getSingle(200.0, 2, 4));
                        break;

                    case "00A900":
                        str2 = str2 + "08" + Add33H(s + "00A900" + getSingle(200.0, 2, 4));
                        break;

                    case "00AA00":
                        str2 = str2 + "08" + Add33H(s + "00AA00" + getSingle(200.0, 2, 4));
                        break;

                    case "00AB00":
                        str2 = str2 + "08" + Add33H(s + "00AB00" + getSingle(200.0, 2, 4));
                        break;

                    case "00AC00":
                        str2 = str2 + "08" + Add33H(s + "00AC00" + getSingle(200.0, 2, 4));
                        break;

                    case "00AD00":
                        str2 = str2 + "08" + Add33H(s + "00AD00" + getSingle(200.0, 2, 4));
                        break;

                    case "00AE00":
                        str2 = str2 + "08" + Add33H(s + "00AE00" + getSingle(200.0, 2, 4));
                        break;

                    case "000B00":
                        if (s == "00")//当前结算周期组合有功总累计用电量 
                            str2 = str2 + "08" + Add33H(s + "000B00" + getSingle(200.0, 2, 4));
                        else//上1结算周期组合有功总累计用电量 
                            str2 = str2 + "08" + Add33H(s + "000B00" + getSingle(200.0, 2, 4));
                        break;
                    case "003D00":
                        str2 = str2 + "08" + Add33H(s + "003D00" + getSingle((s == "00") ? (Pq[0, 0] / 3.0) : 4.1133333333333333, 2, 4));
                        break;

                    case "003E00":
                        str2 = str2 + "08" + Add33H(s + "003E00" + getSingle((s == "00") ? (Pq[1, 0] / 3.0) : 0.39999999999999997, 2, 4));
                        break;

                    case "003F00":
                        str2 = str2 + "08" + Add33H(s + "003F00" + getSingle((s == "00") ? (Pq[2, 0] / 3.0) : 0.43333333333333335, 2, 4));
                        break;

                    case "004000":
                        str2 = str2 + "08" + Add33H(s + "004000" + getSingle((s == "00") ? (Pq[3, 0] / 3.0) : 0.46666666666666662, 2, 4));
                        break;

                    case "004100":
                        str2 = str2 + "08" + Add33H(s + "004100" + getSingle((s == "00") ? (Pq[4, 0] / 3.0) : 0.5, 2, 4));
                        break;

                    case "004200":
                        str2 = str2 + "08" + Add33H(s + "004200" + getSingle((s == "00") ? (Pq[5, 0] / 3.0) : 0.53333333333333333, 2, 4));
                        break;

                    case "004300":
                        str2 = str2 + "08" + Add33H(s + "004300" + getSingle((s == "00") ? (Pq[6, 0] / 3.0) : 0.56666666666666665, 2, 4));
                        break;

                    case "004400":
                        str2 = str2 + "08" + Add33H(s + "004400" + getSingle((s == "00") ? (Pq[7, 0] / 3.0) : 0.6, 2, 4));
                        break;

                    case "004500":
                        str2 = str2 + "08" + Add33H(s + "004500" + getSingle((Pq[0, 0] + 100.0) / 3.0, 2, 4));
                        break;

                    case "004600":
                        str2 = str2 + "08" + Add33H(s + "004600" + getSingle((Pq[1, 0] + 100.0) / 3.0, 2, 4));
                        break;

                    case "00BC00":
                        str2 = str2 + "08" + Add33H(s + "00BC00" + getSingle(300.0, 2, 4));
                        break;

                    case "00BD00":
                        str2 = str2 + "08" + Add33H(s + "00BD00" + getSingle(300.0, 2, 4));
                        break;

                    case "00BE00":
                        str2 = str2 + "08" + Add33H(s + "00BE00" + getSingle(300.0, 2, 4));
                        break;

                    case "00BF00":
                        str2 = str2 + "08" + Add33H(s + "00BF00" + getSingle(300.0, 2, 4));
                        break;

                    case "00C000":
                        str2 = str2 + "08" + Add33H(s + "00C000" + getSingle(300.0, 2, 4));
                        break;

                    case "00C100":
                        str2 = str2 + "08" + Add33H(s + "00C100" + getSingle(300.0, 2, 4));
                        break;

                    case "00C200":
                        str2 = str2 + "08" + Add33H(s + "00C200" + getSingle(300.0, 2, 4));
                        break;

                    case "010100":
                        str2 = str2 + "0C" + Add33H(s + "000101" + getSingle((s == "00") ? Zdxl[0, 0] : ZdxlDj[0, 0], 4, 3) + ((s == "00") ? ZdxlTime[0, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy") : ("0200" + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Year % 100))));
                        break;

                    case "010101":
                        str2 = str2 + "0C" + Add33H(s + "010101" + getSingle((s == "00") ? Zdxl[0, 1] : ZdxlDj[0, 1], 4, 3) + ((s == "00") ? ZdxlTime[0, 1].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy") : ("0200" + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Year % 100))));
                        break;

                    case "010102":
                        str2 = str2 + "0C" + Add33H(s + "020101" + getSingle((s == "00") ? Zdxl[0, 2] : ZdxlDj[0, 2], 4, 3) + ((s == "00") ? ZdxlTime[0, 2].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy") : ("0200" + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Year % 100))));
                        break;

                    case "010103":
                        str2 = str2 + "0C" + Add33H(s + "030101" + getSingle((s == "00") ? Zdxl[0, 3] : ZdxlDj[0, 3], 4, 3) + ((s == "00") ? ZdxlTime[0, 3].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy") : ("0200" + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Year % 100))));
                        break;

                    case "010104":
                        str2 = str2 + "0C" + Add33H(s + "040101" + getSingle((s == "00") ? Zdxl[0, 4] : ZdxlDj[0, 4], 4, 3) + ((s == "00") ? ZdxlTime[0, 4].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy") : ("0200" + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Year % 100))));
                        break;

                    case "0101FF":
                        if (!(s == "00"))
                        {

                            str2 = str2 + "2C" + Add33H(s + "FF0101" + getSingle(ZdxlDj[0, 0], 4, 3) + "0200" + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Year % 100));
                            num5 = 1;
                            while (num5 <= 4)
                            {
                                str2 = str2 + Add33H(getSingle(ZdxlDj[0, num5], 4, 3) + "0200" + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Year % 100));
                                num5++;
                            }

                        }
                        else
                        {
                            str2 = str2 + "2C" + Add33H(s + "FF0101" + getSingle(Zdxl[0, 0], 4, 3) + ZdxlTime[0, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                            for (num5 = 1; num5 <= 4; num5++)
                            {
                                str2 = str2 + Add33H(getSingle(Zdxl[0, num5], 4, 3) + ZdxlTime[0, num5].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                            }
                        }

                        break;

                    case "010200":
                        str2 = str2 + "0C" + Add33H(s + "000201" + getSingle((s == "00") ? Zdxl[1, 0] : ZdxlDj[1, 0], 4, 3) + ((s == "00") ? ZdxlTime[1, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy") : ("0200" + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Year % 100))));
                        break;

                    case "010201":
                        str2 = str2 + "0C" + Add33H(s + "010201" + getSingle((s == "00") ? Zdxl[1, 1] : ZdxlDj[1, 1], 4, 3) + ((s == "00") ? ZdxlTime[1, 1].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy") : ("0200" + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Year % 100))));
                        break;

                    case "010202":
                        str2 = str2 + "0C" + Add33H(s + "020201" + getSingle((s == "00") ? Zdxl[1, 2] : ZdxlDj[1, 2], 4, 3) + ((s == "00") ? ZdxlTime[1, 2].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy") : ("0200" + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Year % 100))));
                        break;

                    case "010203":
                        str2 = str2 + "0C" + Add33H(s + "030201" + getSingle((s == "00") ? Zdxl[1, 3] : ZdxlDj[1, 3], 4, 3) + ((s == "00") ? ZdxlTime[1, 3].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy") : ("0200" + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Year % 100))));
                        break;

                    case "010204":
                        str2 = str2 + "0C" + Add33H(s + "040201" + getSingle((s == "00") ? Zdxl[1, 4] : ZdxlDj[1, 4], 4, 3) + ((s == "00") ? ZdxlTime[1, 4].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy") : ("0200" + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Year % 100))));
                        break;

                    case "0102FF":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "2C" + Add33H(s + "FF0201" + getSingle(ZdxlDj[1, 0], 4, 3) + "0300" + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Year % 100));
                            for (num5 = 1; num5 <= 4; num5++)
                            {
                                str2 = str2 + Add33H(getSingle(ZdxlDj[1, num5], 4, 3) + "0300" + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)-Convert.ToInt16(s, 16)).Year % 100));
                            }
                        }
                        else
                        {
                            str2 = str2 + "2C" + Add33H(s + "FF0201" + getSingle(Zdxl[1, 0], 4, 3) + ZdxlTime[1, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                            for (num5 = 1; num5 <= 4; num5++)
                            {
                                str2 = str2 + Add33H(getSingle(Zdxl[1, num5], 4, 3) + ZdxlTime[1, num5].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                            }
                        }
                        break;

                    case "010300":
                        str2 = str2 + "0C" + Add33H(s + "000301" + getSingle(Zdxl[2, 0], 4, 3) + ZdxlTime[2, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010301":
                        str2 = str2 + "0C" + Add33H(s + "010301" + getSingle(Zdxl[2, 1], 4, 3) + ZdxlTime[2, 1].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010302":
                        str2 = str2 + "0C" + Add33H(s + "020301" + getSingle(Zdxl[2, 2], 4, 3) + ZdxlTime[2, 2].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010303":
                        str2 = str2 + "0C" + Add33H(s + "030301" + getSingle(Zdxl[2, 3], 4, 3) + ZdxlTime[2, 3].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010304":
                        str2 = str2 + "0C" + Add33H(s + "040301" + getSingle(Zdxl[2, 4], 4, 3) + ZdxlTime[2, 4].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "0103FF":
                        str2 = str2 + "2C" + Add33H(s + "FF0301" + getSingle(Zdxl[2, 0], 4, 3) + ZdxlTime[2, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle(Zdxl[2, num5], 4, 3) + ZdxlTime[2, num5].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        }
                        break;

                    case "010400":
                        str2 = str2 + "0C" + Add33H(s + "000401" + getSingle(Zdxl[3, 0], 4, 3) + ZdxlTime[3, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010401":
                        str2 = str2 + "0C" + Add33H(s + "010401" + getSingle(Zdxl[3, 1], 4, 3) + ZdxlTime[3, 1].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010402":
                        str2 = str2 + "0C" + Add33H(s + "020401" + getSingle(Zdxl[3, 2], 4, 3) + ZdxlTime[3, 2].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010403":
                        str2 = str2 + "0C" + Add33H(s + "030401" + getSingle(Zdxl[3, 3], 4, 3) + ZdxlTime[3, 3].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010404":
                        str2 = str2 + "0C" + Add33H(s + "040401" + getSingle(Zdxl[3, 4], 4, 3) + ZdxlTime[3, 4].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "0104FF":
                        str2 = str2 + "2C" + Add33H(s + "FF0401" + getSingle(Zdxl[3, 0], 4, 3) + ZdxlTime[3, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle(Zdxl[3, num5], 4, 3) + ZdxlTime[3, num5].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        }
                        break;

                    case "010500":
                        str2 = str2 + "0C" + Add33H(s + "000501" + getSingle(Zdxl[4, 0], 4, 3) + ZdxlTime[4, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010501":
                        str2 = str2 + "0C" + Add33H(s + "010501" + getSingle(Zdxl[4, 1], 4, 3) + ZdxlTime[4, 1].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010502":
                        str2 = str2 + "0C" + Add33H(s + "020501" + getSingle(Zdxl[4, 2], 4, 3) + ZdxlTime[4, 2].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010503":
                        str2 = str2 + "0C" + Add33H(s + "030501" + getSingle(Zdxl[4, 3], 4, 3) + ZdxlTime[4, 3].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010504":
                        str2 = str2 + "0C" + Add33H(s + "040501" + getSingle(Zdxl[4, 4], 4, 3) + ZdxlTime[4, 4].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "0105FF":
                        str2 = str2 + "2C" + Add33H(s + "FF0501" + getSingle(Zdxl[4, 0], 4, 3) + ZdxlTime[4, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle(Zdxl[4, num5], 4, 3) + ZdxlTime[4, num5].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        }
                        break;

                    case "010600":
                        str2 = str2 + "0C" + Add33H(s + "000601" + getSingle(Zdxl[5, 0], 4, 3) + ZdxlTime[5, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010601":
                        str2 = str2 + "0C" + Add33H(s + "010601" + getSingle(Zdxl[5, 1], 4, 3) + ZdxlTime[5, 1].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010602":
                        str2 = str2 + "0C" + Add33H(s + "020601" + getSingle(Zdxl[5, 2], 4, 3) + ZdxlTime[5, 2].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010603":
                        str2 = str2 + "0C" + Add33H(s + "030601" + getSingle(Zdxl[5, 3], 4, 3) + ZdxlTime[5, 3].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010604":
                        str2 = str2 + "0C" + Add33H(s + "040601" + getSingle(Zdxl[5, 4], 4, 3) + ZdxlTime[5, 4].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "0106FF":
                        str2 = str2 + "2C" + Add33H(s + "FF0601" + getSingle(Zdxl[5, 0], 4, 3) + ZdxlTime[5, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle(Zdxl[5, num5], 4, 3) + ZdxlTime[5, num5].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        }
                        break;

                    case "010700":
                        str2 = str2 + "0C" + Add33H(s + "000701" + getSingle(Zdxl[6, 0], 4, 3) + ZdxlTime[6, 0].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010701":
                        str2 = str2 + "0C" + Add33H(s + "010701" + getSingle(Zdxl[6, 1], 4, 3) + ZdxlTime[6, 1].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010702":
                        str2 = str2 + "0C" + Add33H(s + "020701" + getSingle(Zdxl[6, 2], 4, 3) + ZdxlTime[6, 2].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010703":
                        str2 = str2 + "0C" + Add33H(s + "030701" + getSingle(Zdxl[6, 3], 4, 3) + ZdxlTime[6, 3].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010704":
                        str2 = str2 + "0C" + Add33H(s + "040701" + getSingle(Zdxl[6, 4], 4, 3) + ZdxlTime[6, 4].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "0107FF":
                        str2 = str2 + "2C" + Add33H(s + "FF0701" + getSingle(Zdxl[6, 0], 4, 3) + ZdxlTime[6, 0].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle(Zdxl[6, num5], 4, 3) + ZdxlTime[6, num5].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        }
                        break;

                    case "010800":
                        str2 = str2 + "0C" + Add33H(s + "000801" + getSingle(Zdxl[7, 0], 4, 3) + ZdxlTime[7, 0].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010801":
                        str2 = str2 + "0C" + Add33H(s + "010801" + getSingle(Zdxl[7, 1], 4, 3) + ZdxlTime[7, 1].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010802":
                        str2 = str2 + "0C" + Add33H(s + "020801" + getSingle(Zdxl[7, 2], 4, 3) + ZdxlTime[7, 2].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010803":
                        str2 = str2 + "0C" + Add33H(s + "030801" + getSingle(Zdxl[7, 3], 4, 3) + ZdxlTime[7, 3].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010804":
                        str2 = str2 + "0C" + Add33H(s + "040801" + getSingle(Zdxl[7, 4], 4, 3) + ZdxlTime[7, 4].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "0108FF":
                        str2 = str2 + "2C" + Add33H(s + "FF0801" + getSingle(Zdxl[7, 0], 4, 3) + ZdxlTime[7, 0].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle(Zdxl[7, num5], 4, 3) + ZdxlTime[7, num5].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        }
                        break;

                    case "010900":
                        str2 = str2 + "0C" + Add33H(s + "000901" + getSingle(21.0, 4, 3) + ZdxlTime[0, 0].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010901":
                        str2 = str2 + "0C" + Add33H(s + "010901" + getSingle(22.0, 4, 3) + ZdxlTime[0, 1].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010902":
                        str2 = str2 + "0C" + Add33H(s + "020901" + getSingle(23.0, 4, 3) + ZdxlTime[0, 2].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010903":
                        str2 = str2 + "0C" + Add33H(s + "030901" + getSingle(24.0, 4, 3) + ZdxlTime[0, 3].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010904":
                        str2 = str2 + "0C" + Add33H(s + "040901" + getSingle(25.0, 4, 3) + ZdxlTime[0, 4].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "0109FF":
                        str2 = str2 + "2C" + Add33H(s + "FF0901" + getSingle(21.0, 4, 3) + ZdxlTime[0, 0].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        for (num5 = 1; num5 <= 4; num5++)
                        {
                            str2 = str2 + Add33H(getSingle((double)(0x15 + num5), 4, 3) + ZdxlTime[0, num5].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        }
                        break;

                    case "010A00":
                        str2 = str2 + "0C" + Add33H(s + "000A01" + getSingle(31.0, 4, 3) + ZdxlTime[0, 0].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010A01":
                        str2 = str2 + "0C" + Add33H(s + "010A01" + getSingle(32.0, 4, 3) + ZdxlTime[0, 1].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010A02":
                        str2 = str2 + "0C" + Add33H(s + "020A01" + getSingle(33.0, 4, 3) + ZdxlTime[0, 2].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010A03":
                        str2 = str2 + "0C" + Add33H(s + "030A01" + getSingle(34.0, 4, 3) + ZdxlTime[0, 3].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010A04":
                        str2 = str2 + "0C" + Add33H(s + "040A01" + getSingle(35.0, 4, 3) + ZdxlTime[0, 4].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "010AFF":
                        str2 = str2 + "2C" + Add33H(s + "FF0A01" + getSingle(31.0, 4, 3) + ZdxlTime[0, 0].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        num5 = 1;
                        while (num5 <= 4)
                        {
                            str2 = str2 + Add33H(getSingle((double)(0x1f + num5), 4, 3) + ZdxlTime[0, num5].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                            num5++;
                        }
                        break;

                    case "011500":
                        str2 = str2 + "0C" + Add33H(s + "001501" + getSingle(0.91, 4, 3) + ZdxlTime[0, 0].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "011600":
                        str2 = str2 + "0C" + Add33H(s + "001601" + getSingle(0.92, 4, 3) + ZdxlTime[0, 1].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "011700":
                        str2 = str2 + "0C" + Add33H(s + "001701" + getSingle(0.93, 4, 3) + ZdxlTime[0, 2].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "011800":
                        str2 = str2 + "0C" + Add33H(s + "001801" + getSingle(0.94, 4, 3) + ZdxlTime[0, 3].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "011900":
                        str2 = str2 + "0C" + Add33H(s + "001901" + getSingle(0.95, 4, 3) + ZdxlTime[0, 4].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "011A00":
                        str2 = str2 + "0C" + Add33H(s + "001A01" + getSingle(0.96, 4, 3) + ZdxlTime[0, 0].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "011B00":
                        str2 = str2 + "0C" + Add33H(s + "001B01" + getSingle(0.97, 4, 3) + ZdxlTime[0, 1].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "011C00":
                        str2 = str2 + "0C" + Add33H(s + "001C01" + getSingle(0.98, 4, 3) + ZdxlTime[0, 2].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "011D00":
                        str2 = str2 + "0C" + Add33H(s + "001D01" + getSingle(0.99, 4, 3) + ZdxlTime[0, 3].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "011E00":
                        str2 = str2 + "0C" + Add33H(s + "001E01" + getSingle(0.9, 4, 3) + ZdxlTime[0, 4].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "012900":
                        str2 = str2 + "0C" + Add33H(s + "002901" + getSingle(0.81, 4, 3) + ZdxlTime[0, 0].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "012A00":
                        str2 = str2 + "0C" + Add33H(s + "002A01" + getSingle(0.82, 4, 3) + ZdxlTime[0, 1].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "012B00":
                        str2 = str2 + "0C" + Add33H(s + "002B01" + getSingle(0.83, 4, 3) + ZdxlTime[0, 2].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "012C00":
                        str2 = str2 + "0C" + Add33H(s + "002C01" + getSingle(0.84, 4, 3) + ZdxlTime[0, 3].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "012D00":
                        str2 = str2 + "0C" + Add33H(s + "002D01" + getSingle(0.85, 4, 3) + ZdxlTime[0, 4].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "012E00":
                        str2 = str2 + "0C" + Add33H(s + "002E01" + getSingle(0.86, 4, 3) + ZdxlTime[0, 0].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "012F00":
                        str2 = str2 + "0C" + Add33H(s + "002F01" + getSingle(0.87, 4, 3) + ZdxlTime[0, 1].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "013000":
                        str2 = str2 + "0C" + Add33H(s + "003001" + getSingle(0.88, 4, 3) + ZdxlTime[0, 2].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "013100":
                        str2 = str2 + "0C" + Add33H(s + "003101" + getSingle(0.89, 4, 3) + ZdxlTime[0, 3].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "013200":
                        str2 = str2 + "0C" + Add33H(s + "003201" + getSingle(0.8, 4, 3) + ZdxlTime[0, 4].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "013D00":
                        str2 = str2 + "0C" + Add33H(s + "003D01" + getSingle(0.71, 4, 3) + ZdxlTime[0, 0].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "013E00":
                        str2 = str2 + "0C" + Add33H(s + "003E01" + getSingle(0.72, 4, 3) + ZdxlTime[0, 1].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "013F00":
                        str2 = str2 + "0C" + Add33H(s + "003F01" + getSingle(0.73, 4, 3) + ZdxlTime[0, 2].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "014000":
                        str2 = str2 + "0C" + Add33H(s + "004001" + getSingle(0.74, 4, 3) + ZdxlTime[0, 3].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "014100":
                        str2 = str2 + "0C" + Add33H(s + "004101" + getSingle(0.75, 4, 3) + ZdxlTime[0, 4].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "014200":
                        str2 = str2 + "0C" + Add33H(s + "004201" + getSingle(0.76, 4, 3) + ZdxlTime[0, 0].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "014300":
                        str2 = str2 + "0C" + Add33H(s + "004301" + getSingle(0.77, 4, 3) + ZdxlTime[0, 1].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "014400":
                        str2 = str2 + "0C" + Add33H(s + "004401" + getSingle(0.78, 4, 3) + ZdxlTime[0, 2].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "014500":
                        str2 = str2 + "0C" + Add33H(s + "004501" + getSingle(0.79, 4, 3) + ZdxlTime[0, 3].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "014600":
                        str2 = str2 + "0C" + Add33H(s + "004601" + getSingle(0.7, 4, 3) + ZdxlTime[0, 4].AddMonths(Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        break;

                    case "020101":
                        str2 = str2 + "06" + Add33H(s + "010102" + getSingle(Ua, 1, 2));
                        break;

                    case "020102":
                        str2 = str2 + "06" + Add33H(s + "020102" + getSingle(Ub, 1, 2));
                        break;

                    case "020103":
                        str2 = str2 + "06" + Add33H(s + "030102" + getSingle(Uc, 1, 2));
                        break;

                    case "0201FF":
                        str2 = str2 + "0A" + Add33H(s + "FF0102" + getSingle(Ua, 1, 2)) + Add33H(getSingle(Ub, 1, 2)) + Add33H(getSingle(Uc, 1, 2));
                        break;

                    case "020201":
                        str2 = str2 + "07" + Add33H(s + "010202" + getSingle(Ia, 3, 3));
                        break;

                    case "020202":
                        str2 = str2 + "07" + Add33H(s + "020202" + getSingle(Ib, 3, 3));
                        break;

                    case "020203":
                        str2 = str2 + "07" + Add33H(s + "030202" + getSingle(Ic, 3, 3));
                        break;

                    case "0202FF":
                        str2 = str2 + "0D" + Add33H(s + "FF0202" + getSingle(Ia, 3, 3)) + Add33H(getSingle(Ib, 3, 3)) + Add33H(getSingle(Ic, 3, 3));
                        break;

                    case "020300":
                        str2 = str2 + "07" + Add33H(s + "000302" + getSingle((Pa + Pb) + Pc, 4, 3));
                        break;

                    case "020301":
                        str2 = str2 + "07" + Add33H(s + "010302" + getSingle(Pa, 4, 3));
                        break;

                    case "020302":
                        str2 = str2 + "07" + Add33H(s + "020302" + getSingle(Pb, 4, 3));
                        break;

                    case "020303":
                        str2 = str2 + "07" + Add33H(s + "030302" + getSingle(Pc, 4, 3));
                        break;

                    case "0203FF":
                        str2 = str2 + "10" + Add33H(s + "FF0302" + getSingle((Pa + Pb) + Pc, 4, 3)) + Add33H(getSingle(Pa, 4, 3)) + Add33H(getSingle(Pb, 4, 3)) + Add33H(getSingle(Pc, 4, 3));
                        break;

                    case "020400":
                        str2 = str2 + "07" + Add33H(s + "000402" + getSingle((Qa + Qb) + Qc, 4, 3));
                        break;

                    case "020401":
                        str2 = str2 + "07" + Add33H(s + "010402" + getSingle(Qa, 4, 3));
                        break;

                    case "020402":
                        str2 = str2 + "07" + Add33H(s + "020402" + getSingle(Qb, 4, 3));
                        break;

                    case "020403":
                        str2 = str2 + "07" + Add33H(s + "030402" + getSingle(Qc, 4, 3));
                        break;

                    case "0204FF":
                        str2 = str2 + "10" + Add33H(s + "FF0402" + getSingle((Qa + Qb) + Qc, 4, 3)) + Add33H(getSingle(Qa, 4, 3)) + Add33H(getSingle(Qb, 4, 3)) + Add33H(getSingle(Qc, 4, 3));
                        break;

                    case "020500":
                        str2 = str2 + "07" + Add33H(s + "000502" + getSingle((Sa + Sb) + Sc, 4, 3));
                        break;

                    case "020501":
                        str2 = str2 + "07" + Add33H(s + "010502" + getSingle(Sa, 4, 3));
                        break;

                    case "020502":
                        str2 = str2 + "07" + Add33H(s + "020502" + getSingle(Sb, 4, 3));
                        break;

                    case "020503":
                        str2 = str2 + "07" + Add33H(s + "030502" + getSingle(Sc, 4, 3));
                        break;

                    case "0205FF":
                        str2 = str2 + "10" + Add33H(s + "FF0502" + getSingle((Sa + Sb) + Sc, 4, 3)) + Add33H(getSingle(Sa, 4, 3)) + Add33H(getSingle(Sb, 4, 3)) + Add33H(getSingle(Sc, 4, 3));
                        break;

                    case "020600":
                        str2 = str2 + "06" + Add33H("00000602" + getSingle(num, 3, 2));
                        break;

                    case "020601":
                        str2 = str2 + "06" + Add33H("00010602" + getSingle((Ia == 0.0) ? 1.0 : Math.Cos((Phia / 180.0) * 3.1415936), 3, 2));
                        break;

                    case "020602":
                        str2 = str2 + "06" + Add33H("00020602" + getSingle((Ib == 0.0) ? 1.0 : Math.Cos((Phib / 180.0) * 3.1415936), 3, 2));
                        break;

                    case "020603":
                        str2 = str2 + "06" + Add33H("00030602" + getSingle((Ic == 0.0) ? 1.0 : Math.Cos((Phic / 180.0) * 3.1415936), 3, 2));
                        break;

                    case "0206FF":
                        str2 = str2 + "0C" + Add33H("00FF0602" + getSingle(num, 3, 2)) + Add33H(getSingle((Ia == 0.0) ? 1.0 : Math.Cos((Phia / 180.0) * 3.1415936), 3, 2)) + Add33H(getSingle((Ib == 0.0) ? 1.0 : Math.Cos((Phib / 180.0) * 3.1415936), 3, 2)) + Add33H(getSingle((Ic == 0.0) ? 1.0 : Math.Cos((Phic / 180.0) * 3.1415936), 3, 2));
                        break;

                    case "020701":
                        str2 = str2 + "06" + Add33H("00010702" + getSingle(Phia % 360.0, 1, 2));
                        break;

                    case "020702":
                        str2 = str2 + "06" + Add33H("00020702" + getSingle(Phib % 360.0, 1, 2));
                        break;

                    case "020703":
                        str2 = str2 + "06" + Add33H("00030702" + getSingle(Phic % 360.0, 1, 2));
                        break;

                    case "0207FF":
                        str2 = str2 + "0A" + Add33H("00FF0702" + getSingle(Phia % 360.0, 1, 2)) + Add33H(getSingle(Phib % 360.0, 1, 2)) + Add33H(getSingle(Phic % 360.0, 1, 2));
                        break;

                    case "020801":
                        str2 = str2 + "06" + Add33H("00010802" + getSingle(1.1, 2, 2));
                        break;

                    case "020802":
                        str2 = str2 + "06" + Add33H("00020802" + getSingle(1.2, 2, 2));
                        break;

                    case "020803":
                        str2 = str2 + "06" + Add33H("00030802" + getSingle(1.3, 2, 2));
                        break;

                    case "0208FF":
                        str2 = str2 + "0A" + Add33H("00FF0802" + getSingle(1.1, 2, 2)) + Add33H(getSingle(1.2, 2, 2)) + Add33H(getSingle(1.3, 2, 2));
                        break;

                    case "020901":
                        str2 = str2 + "06" + Add33H("00010902" + getSingle(2.1, 2, 2));
                        break;

                    case "020902":
                        str2 = str2 + "06" + Add33H("00020902" + getSingle(2.2, 2, 2));
                        break;

                    case "020903":
                        str2 = str2 + "06" + Add33H("00030902" + getSingle(2.3, 2, 2));
                        break;

                    case "0209FF":
                        str2 = str2 + "0A" + Add33H("00FF0902" + getSingle(2.1, 2, 2)) + Add33H(getSingle(2.2, 2, 2)) + Add33H(getSingle(2.3, 2, 2));
                        break;

                    case "020A01":
                        if (!(s != "FF"))
                        {
                            str2 = str2 + "2E" + Add33H(s + "010A02");
                            for (num8 = 1; num8 < 0x16; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(0.11 * num8, 2, 2));
                            }
                        }
                        else
                        {
                            str2 = str2 + "06" + Add33H(s + "010A02" + getSingle(0.11 * Convert.ToByte(s, 16), 2, 2));
                        }
                        break;

                    case "020A02":
                        if (!(s != "FF"))
                        {
                            str2 = str2 + "2E" + Add33H(s + "020A02");
                            for (num8 = 1; num8 < 0x16; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(0.12 * num8, 2, 2));
                            }
                        }
                        else
                        {
                            str2 = str2 + "06" + Add33H(s + "020A02" + getSingle(0.12 * Convert.ToByte(s, 16), 2, 2));
                        }
                        break;

                    case "020A03":
                        if (!(s != "FF"))
                        {
                            str2 = str2 + "2E" + Add33H(s + "030A02");
                            for (num8 = 1; num8 < 0x16; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(0.13 * num8, 2, 2));
                            }
                        }
                        else
                        {
                            str2 = str2 + "06" + Add33H(s + "030A02" + getSingle(0.13 * Convert.ToByte(s, 16), 2, 2));
                        }
                        break;

                    case "020B01":
                        if (!(s != "FF"))
                        {
                            str2 = str2 + "2E" + Add33H(s + "010B02");
                            for (num8 = 1; num8 < 0x16; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(0.2 * num8, 2, 2));
                            }
                        }
                        else
                        {
                            str2 = str2 + "06" + Add33H(s + "010B02" + getSingle(0.2 * Convert.ToByte(s, 16), 2, 2));
                        }
                        break;

                    case "020B02":
                        if (!(s != "FF"))
                        {
                            str2 = str2 + "2E" + Add33H(s + "020B02");
                            for (num8 = 1; num8 < 0x16; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(0.21 * num8, 2, 2));
                            }
                        }
                        else
                        {
                            str2 = str2 + "06" + Add33H(s + "020B02" + getSingle(0.21 * Convert.ToByte(s, 16), 2, 2));
                        }
                        break;

                    case "020B03":
                        if (!(s != "FF"))
                        {
                            str2 = str2 + "2E" + Add33H(s + "030B02");
                            for (num8 = 1; num8 < 0x16; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(0.22 * num8, 2, 2));
                            }
                        }
                        else
                        {
                            str2 = str2 + "06" + Add33H(s + "030B02" + getSingle(0.22 * Convert.ToByte(s, 16), 2, 2));
                        }
                        break;

                    case "028000":
                        switch (s)
                        {
                            case "01":
                                pbyte = 3;
                                pdot = 3;
                                str2 = str2 + string.Format("{0:X2}", pbyte + 4) + Add33H(s + "008002" + getSingle(0.1 * Convert.ToByte(s, 16), pdot, pbyte));
                                break;

                            case "02":
                                pbyte = 2;
                                pdot = 2;
                                str2 = str2 + string.Format("{0:X2}", pbyte + 4) + Add33H(s + "008002" + getSingle(0.1 * Convert.ToByte(s, 16), pdot, pbyte));
                                break;

                            case "03":
                                pbyte = 3;
                                pdot = 4;
                                str2 = str2 + string.Format("{0:X2}", pbyte + 4) + Add33H(s + "008002" + getSingle(0.1 * Convert.ToByte(s, 16), pdot, pbyte));
                                break;

                            case "04":
                                pbyte = 3;
                                pdot = 4;
                                str2 = str2 + string.Format("{0:X2}", pbyte + 4) + Add33H(s + "008002" + getSingle(0.1 * Convert.ToByte(s, 16), pdot, pbyte));
                                break;

                            case "05":
                                pbyte = 3;
                                pdot = 4;
                                break;

                            case "06":
                                pbyte = 3;
                                pdot = 4;
                                str2 = str2 + string.Format("{0:X2}", pbyte + 4) + Add33H(s + "008002" + getSingle(0.1 * Convert.ToByte(s, 16), pdot, pbyte));
                                break;

                            case "07":
                                pbyte = 2;
                                pdot = 1;
                                str2 = str2 + string.Format("{0:X2}", pbyte + 4) + Add33H(s + "008002" + getSingle(0.1 * Convert.ToByte(s, 16), pdot, pbyte));
                                break;

                            case "08":
                                pbyte = 2;
                                pdot = 2;
                                str2 = str2 + string.Format("{0:X2}", pbyte + 4) + Add33H(s + "008002" + getSingle(0.1 * Convert.ToByte(s, 16), pdot, pbyte));
                                break;

                            case "09":
                                pbyte = 2;
                                pdot = 2;
                                str2 = str2 + string.Format("{0:X2}", pbyte + 4) + Add33H(s + "008002" + getSingle(0.1 * Convert.ToByte(s, 16), pdot, pbyte));
                                break;

                            case "0A":
                                pbyte = 4;
                                pdot = 0;
                                str2 = str2 + string.Format("{0:X2}", pbyte + 4) + Add33H(s + "008002" + getSingle((double)DCWorkTime, pdot, pbyte));
                                break;
                        }
                        break;

                    case "030100":
                        str2 = str2 + "16" + Add33H("00000103" + getSingle(5.0, 0, 3) + getSingle(5.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(7.0, 0, 3) + getSingle(7.0, 0, 3));
                        break;

                    case "030101":
                        str2 = str2 + "77" + Add33H(s + "010103121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030102":
                        str2 = str2 + "77" + Add33H(s + "020103121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030103":
                        str2 = str2 + "77" + Add33H(s + "030103121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030200":
                        str2 = str2 + "16" + Add33H("00000203" + getSingle(5.0, 0, 3) + getSingle(5.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(7.0, 0, 3) + getSingle(7.0, 0, 3));
                        break;

                    case "030201":
                        str2 = str2 + "77" + Add33H(s + "010203121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030202":
                        str2 = str2 + "77" + Add33H(s + "020203121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030203":
                        str2 = str2 + "77" + Add33H(s + "030203121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030300":
                        str2 = str2 + "16" + Add33H("00000303" + getSingle(5.0, 0, 3) + getSingle(5.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(7.0, 0, 3) + getSingle(7.0, 0, 3));
                        break;

                    case "030301":
                        str2 = str2 + "77" + Add33H(s + "010303121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030302":
                        str2 = str2 + "77" + Add33H(s + "020303121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030303":
                        str2 = str2 + "77" + Add33H(s + "030303121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030400":
                        str2 = str2 + "16" + Add33H("00000403" + getSingle((double)Math.Abs((int)(DXCS - 2)), 0, 3) + getSingle(1.0, 0, 3) + getSingle(1.0, 0, 3) + getSingle(1.0, 0, 3) + getSingle(1.0, 0, 3) + getSingle(1.0, 0, 3));
                        break;

                    case "030401":
                        str2 = str2 + "77" + Add33H(s + "010403121012020509091512020509");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030402":
                        str2 = str2 + "77" + Add33H(s + "020403121012030509091512030509");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030403":
                        str2 = str2 + "77" + Add33H(s + "030403121012010509091512010509");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "033500":
                        if (s == "00")
                        {
                            str2 = str2 + "07" + Add33H(s + "003503" + string.Format("{0:D2}", Math.Abs((int)(Mfunusual)) % 100) + "0000");
                        }
                        else
                        {

                            str2 = str2 + "20" + Add33H(s + "003503" + DateBron().ToString("ssmmHHddMMyy") + DateBron().ToString("ssmmHHddMMyy") + getSingle(Pq[1, 0], 2, 4) + getSingle(Pq[2, 0], 2, 4) + getSingle(Pq[3, 0], 2, 4) + getSingle(Pq[4, 0], 2, 4));
                        }
                        break;
                    case "100000"://总失压次数
                        if (s == "01")
                            str2 = str2 + "07" + Add33H("01000010" + getSingle((double)Math.Abs((int)(SYCS)), 0, 3));
                        else//累积时间
                            str2 = str2 + "07" + Add33H("02000010" + getSingle(SYCS, 0, 3));
                        break;
                    case "100001":
                        if (s == "01")
                            str2 = str2 + "0A" + Add33H("01010010" + SYFSTime);//最近1次失压发生时刻
                        break;
                    case "100002":
                        if (s == "01")
                            str2 = str2 + "0A" + Add33H("01020010" + SYJSTime);//最近1次失压结束时刻
                        break;
                    case "100100"://A相失压次数
                        if (s == "01")
                            str2 = str2 + "07" + Add33H("01000110" + getSingle((double)Math.Abs((int)(SYCS - 2)), 0, 3));
                        else
                            str2 = str2 + "07" + Add33H("02000110" + getSingle(SYCS - 2, 0, 3));
                        break;
                    case "100200"://B相失压次数
                        if (s == "01")
                            str2 = str2 + "07" + Add33H("01000210" + getSingle((double)Math.Abs((int)(1)), 0, 3));
                        else
                            str2 = str2 + "07" + Add33H("02000210" + getSingle(1, 0, 3));
                        break;
                    case "100300"://C相失压次数
                        if (s == "01")
                            str2 = str2 + "07" + Add33H("01000310" + getSingle((double)Math.Abs((int)(1)), 0, 3));
                        else
                            str2 = str2 + "07" + Add33H("02000310" + getSingle(1, 0, 3));
                        break;

                    case "100101":
                        str2 = str2 + "0A" + Add33H(s + "010110" + SYFSTime);
                        break;

                    case "100125":
                        str2 = str2 + "0A" + Add33H(s + "250110" + SYJSTime);
                        break;

                    case "100201":
                        str2 = str2 + "0A" + Add33H(s + "010210121012030509");
                        break;

                    case "100225":
                        str2 = str2 + "0A" + Add33H(s + "250210091512030509");
                        break;

                    case "100301":
                        str2 = str2 + "0A" + Add33H(s + "010310121012010509");
                        break;

                    case "100325":
                        str2 = str2 + "0A" + Add33H(s + "250310091512010509");
                        break;

                    case "1001FF":
                        str2 = str2 + "C7" + Add33H(s + "FF0110" + SYFSTime);
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        str2 = ((str2 + Add33H(getSingle(3.0, 0, 4))) + Add33H(getSingle(1.0, 0, 4)) + Add33H(getSingle(1.0, 0, 4))) + Add33H(getSingle(1.0, 0, 4)) + Add33H(SYJSTime);
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                        }
                        break;

                    case "1002FF":
                        str2 = str2 + "C7" + Add33H(s + "FF0210121012030509");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        str2 = ((str2 + Add33H(getSingle(3.0, 0, 4))) + Add33H(getSingle(1.0, 0, 4)) + Add33H(getSingle(1.0, 0, 4))) + Add33H(getSingle(1.0, 0, 4)) + Add33H("091512030509");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                        }
                        break;

                    case "1003FF":
                        str2 = str2 + "C7" + Add33H(s + "FF0310121012010509");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        str2 = ((str2 + Add33H(getSingle(3.0, 0, 4))) + Add33H(getSingle(1.0, 0, 4)) + Add33H(getSingle(1.0, 0, 4))) + Add33H(getSingle(1.0, 0, 4)) + Add33H("091512010509");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            num8 = 0;
                            while (num8 < 4)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                                num8++;
                            }
                        }
                        break;



                    case "1A0000"://总断流次数
                        if (s == "01")
                            str2 = str2 + "07" + Add33H("0100001A" + getSingle((double)Math.Abs((int)(9)), 0, 3));
                        else//累积时间
                            str2 = str2 + "07" + Add33H("0200001A" + getSingle(9, 0, 3));
                        break;
                    case "1A0100"://A相断流次数
                        if (s == "01")
                            str2 = str2 + "07" + Add33H("0100011A" + getSingle((double)Math.Abs((int)(CutOffCurCount_A)), 0, 3));
                        else
                            str2 = str2 + "07" + Add33H("0200011A" + getSingle(3 * CutOffCurCount_A, 0, 3));
                        break;
                    case "1A0200"://B相断流次数
                        if (s == "01")
                            str2 = str2 + "07" + Add33H("0100021A" + getSingle((double)Math.Abs((int)(CutOffCurCount_B)), 0, 3));
                        else
                            str2 = str2 + "07" + Add33H("0200021A" + getSingle(3 * CutOffCurCount_B, 0, 3));
                        break;
                    case "1A0300"://C相断流次数
                        if (s == "01")
                            str2 = str2 + "07" + Add33H("0100031A" + getSingle((double)Math.Abs((int)(CutOffCurCount_C)), 0, 3));
                        else
                            str2 = str2 + "07" + Add33H("0200031A" + getSingle(3 * CutOffCurCount_C, 0, 3));
                        break;
                    case "130000":
                        if (s == "01")
                            str2 = str2 + "07" + Add33H("01000013" + getSingle((double)Math.Abs((int)(DXCS)), 0, 3));
                        else
                            str2 = str2 + "07" + Add33H("02000013" + getSingle(3 * DXCS, 0, 3));
                        break;
                    case "130001":
                        if (s == "00")
                            str2 = str2 + "07" + Add33H("00010013" + getSingle((double)Math.Abs((int)(DXCS)), 0, 3));//断相总次数
                        else
                            str2 = str2 + "0A" + Add33H("01010013" + DXFSTime);//最近一次发生时刻
                        break;
                    case "130002":
                        if (s == "00")
                            str2 = str2 + "07" + Add33H("00020013" + getSingle(DXCS, 0, 3));//断相总累计时间
                        else
                            str2 = str2 + "0A" + Add33H("01020013" + DXJSTime);//最近一次结束时刻
                        break;

                    case "130100":
                        if (!(s == "01"))
                        {
                            if (s == "02")
                            {
                                str2 = str2 + "07" + Add33H("02000113" + getSingle(DXCS - 2, 0, 3));
                            }
                        }
                        else
                        {
                            str2 = str2 + "07" + Add33H("01000113" + getSingle((double)Math.Abs((int)(DXCS - 2)), 0, 3));
                        }
                        break;

                    case "130200":
                        if (!(s == "01"))
                        {
                            if (s == "02")
                            {
                                str2 = str2 + "07" + Add33H("02000213" + getSingle(1.0, 0, 3));
                            }
                        }
                        else
                        {
                            str2 = str2 + "07" + Add33H("01000213" + getSingle(1.0, 0, 3));
                        }
                        break;

                    case "130300":
                        if (!(s == "01"))
                        {
                            if (s == "02")
                            {
                                str2 = str2 + "07" + Add33H("02000313" + getSingle(1.0, 0, 3));
                            }
                        }
                        else
                        {
                            str2 = str2 + "07" + Add33H("01000313" + getSingle(1.0, 0, 3));
                        }
                        break;

                    case "130101":
                        str2 = str2 + "0A" + Add33H(s + "010113" + DXFSTime);
                        break;

                    case "130125":
                        str2 = str2 + "0A" + Add33H(s + "250113" + DXJSTime);
                        break;

                    case "130201":
                        str2 = str2 + "0A" + Add33H(s + "010213121012030509");
                        break;

                    case "130225":
                        str2 = str2 + "0A" + Add33H(s + "250213091512030509");
                        break;

                    case "130301":
                        str2 = str2 + "0A" + Add33H(s + "010313121012010509");
                        break;

                    case "130325":
                        str2 = str2 + "0A" + Add33H(s + "250313091512010509");
                        break;

                    case "1301FF":
                        str2 = str2 + "C7" + Add33H(s + "FF0113" + DXFSTime);
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        str2 = ((str2 + Add33H(getSingle(3.0, 0, 4))) + Add33H(getSingle(1.0, 0, 4)) + Add33H(getSingle(1.0, 0, 4))) + Add33H(getSingle(1.0, 0, 4)) + Add33H(DXJSTime);
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                        }
                        break;

                    case "1302FF":
                        str2 = str2 + "C7" + Add33H(s + "FF0213121012030509");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        str2 = ((str2 + Add33H(getSingle(3.0, 0, 4))) + Add33H(getSingle(1.0, 0, 4)) + Add33H(getSingle(1.0, 0, 4))) + Add33H(getSingle(1.0, 0, 4)) + Add33H("091512030509");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                        }
                        break;

                    case "1303FF":
                        str2 = str2 + "C7" + Add33H(s + "FF0313121012010509");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        str2 = ((str2 + Add33H(getSingle(3.0, 0, 4))) + Add33H(getSingle(1.0, 0, 4)) + Add33H(getSingle(1.0, 0, 4))) + Add33H(getSingle(1.0, 0, 4)) + Add33H("091512010509");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            num8 = 0;
                            while (num8 < 4)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                                num8++;
                            }
                        }
                        break;

                    case "030500":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "13" + Add33H(s + "000503121110010109" + getSingle(5.0, 3, 3) + "091011010109");
                        }
                        else
                        {
                            str2 = str2 + "0A" + Add33H("00000503" + getSingle(QSYCS, 0, 3) + getSingle(QSYCS * 3, 0, 3));
                        }
                        break;

                    case "030600":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "10" + Add33H(s + "000503121110010109091011010109");
                        }
                        else
                        {
                            str2 = str2 + "0A" + Add33H("00000503" + getSingle(5.0, 0, 3) + getSingle(5.0, 0, 3));
                        }
                        break;

                    case "030700":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "20" + Add33H(s + "000703121110010109091011010109");
                            for (num8 = 0; num8 < 0x10; num8++)
                            {
                                str2 = str2 + Add33H(getSingle((double)(num8 + 5), 2, 4));
                            }
                        }
                        else
                        {
                            str2 = str2 + "0A" + Add33H("00000703" + getSingle(5.0, 0, 3) + getSingle(5.0, 0, 3));
                        }
                        break;

                    case "030800":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "20" + Add33H(s + "000803121110010109091011010109");
                            for (num8 = 0; num8 < 0x10; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(num8 + 2.5, 2, 4));
                            }
                        }
                        else
                        {
                            str2 = str2 + "0A" + Add33H("00000803" + getSingle(5.0, 0, 3) + getSingle(5.0, 0, 3));
                        }
                        break;

                    case "030900":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "22" + Add33H(s + "000903121110010109091011010109" + getSingle(5.0, 2, 2));
                            for (num8 = 0; num8 < 0x10; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(num8 + 1.5, 2, 4));
                            }
                        }
                        else
                        {
                            str2 = str2 + "0A" + Add33H("00000903" + getSingle(5.0, 0, 3) + getSingle(5.0, 0, 3));
                        }
                        break;

                    case "030A00":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "22" + Add33H(s + "000A03121110010109091011010109" + getSingle(5.0, 2, 2));
                            for (num8 = 0; num8 < 0x10; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(num8 + 1.5, 2, 4));
                            }
                        }
                        else
                        {
                            str2 = str2 + "0A" + Add33H("00000A03" + getSingle(5.0, 0, 3) + getSingle(5.0, 0, 3));
                        }
                        break;

                    case "030B00":
                        str2 = str2 + "16" + Add33H("00000B03" + getSingle(5.0, 0, 3) + getSingle(5.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(7.0, 0, 3) + getSingle(7.0, 0, 3));
                        break;

                    case "030B01":
                        str2 = str2 + "77" + Add33H(s + "010B03121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030B02":
                        str2 = str2 + "77" + Add33H(s + "020B03121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030B03":
                        str2 = str2 + "77" + Add33H(s + "030B03121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030C00":
                        str2 = str2 + "16" + Add33H("00000C03" + getSingle(5.0, 0, 3) + getSingle(5.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(7.0, 0, 3) + getSingle(7.0, 0, 3));
                        break;

                    case "030C01":
                        str2 = str2 + "77" + Add33H(s + "010C03121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030C02":
                        str2 = str2 + "77" + Add33H(s + "020C03121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030C03":
                        str2 = str2 + "77" + Add33H(s + "030C03121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030D00":
                        str2 = str2 + "16" + Add33H("00000D03" + getSingle(5.0, 0, 3) + getSingle(5.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(7.0, 0, 3) + getSingle(7.0, 0, 3));
                        break;

                    case "030D01":
                        str2 = str2 + "77" + Add33H(s + "010D03121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030D02":
                        str2 = str2 + "77" + Add33H(s + "020D03121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        for (num9 = 0; num9 < 3; num9++)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                        }
                        break;

                    case "030D03":
                        str2 = str2 + "77" + Add33H(s + "030D03121110010109091011010109");
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(100.0, 2, 4));
                            num8++;
                        }
                        num9 = 0;
                        while (num9 < 3)
                        {
                            for (num8 = 0; num8 < 4; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(40.0, 2, 4));
                            }
                            str2 = ((str2 + Add33H(getSingle(180.0, 1, 2))) + Add33H(getSingle(6.0, 3, 3)) + Add33H(getSingle(6.0, 4, 3))) + Add33H(getSingle(6.0, 4, 3)) + Add33H(getSingle(0.7, 2, 2));
                            num9++;
                        }
                        break;

                    case "030E00":
                        str2 = str2 + "16" + Add33H("00000D03" + getSingle(5.0, 0, 3) + getSingle(5.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(7.0, 0, 3) + getSingle(7.0, 0, 3));
                        break;

                    case "030E01":
                        str2 = str2 + "50" + Add33H(s + "010E03121110010109091011010109");
                        for (num8 = 0; num8 < 0x10; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(num8 + 0.5, 2, 4));
                        }
                        break;

                    case "030E02":
                        str2 = str2 + "50" + Add33H(s + "020E03121110010109091011010109");
                        for (num8 = 0; num8 < 0x10; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(num8 + 0.5, 2, 4));
                        }
                        break;

                    case "030E03":
                        str2 = str2 + "50" + Add33H(s + "030E03121110010109091011010109");
                        for (num8 = 0; num8 < 0x10; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(num8 + 0.5, 2, 4));
                        }
                        break;

                    case "030F00":
                        str2 = str2 + "16" + Add33H("00000F03" + getSingle(5.0, 0, 3) + getSingle(5.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(6.0, 0, 3) + getSingle(7.0, 0, 3) + getSingle(7.0, 0, 3));
                        break;

                    case "030F01":
                        str2 = str2 + "50" + Add33H(s + "010F03121110010109091011010109");
                        for (num8 = 0; num8 < 0x10; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(num8 + 0.5, 2, 4));
                        }
                        break;

                    case "030F02":
                        str2 = str2 + "50" + Add33H(s + "020F03121110010109091011010109");
                        for (num8 = 0; num8 < 0x10; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(num8 + 0.5, 2, 4));
                        }
                        break;

                    case "030F03":
                        str2 = str2 + "50" + Add33H(s + "030F03121110010109091011010109");
                        for (num8 = 0; num8 < 0x10; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(num8 + 0.5, 2, 4));
                        }
                        break;

                    case "031000":
                        str2 = str2 + "1F" + Add33H(s + "001003") + Add33H(getSingle(100.0, 0, 3)) + Add33H(getSingle(10.0, 2, 3)) + Add33H(getSingle(10.0, 2, 3)) + Add33H(getSingle(100.0, 0, 3)) + Add33H(getSingle(100.0, 0, 3)) + Add33H(getSingle(100.0, 1, 2)) + Add33H("1122") + Add33H(DateAndTime.Now.AddMonths(-Convert.ToInt16(s, 16)).ToString("ddMM")) + Add33H(getSingle(100.0, 1, 2)) + Add33H("3344") + Add33H(DateAndTime.Now.AddMonths(-Convert.ToInt16(s, 16)).ToString("ddMM"));
                        break;

                    case "031001":
                        //str2 = ((((((str2 + "1F" + Add33H(s + "011003")) + Add33H(getSingle(100.0, 0, 3)) + Add33H(getSingle(10.0, 2, 3))) + Add33H(getSingle(10.0, 2, 3)) + Add33H(getSingle(77.0, 0, 3))) + Add33H(getSingle(88.0, 0, 3)) + Add33H(getSingle(100.0, 1, 2))) + Add33H("1122") + DateAndTime.Now.AddMonths(-Convert.ToInt16(s, 16)).ToString("ddMM")) + Add33H(getSingle(100.0, 1, 2))) + Add33H("3344") + DateAndTime.Now.AddMonths(-Convert.ToInt16(s, 16)).ToString("ddMM");
                        str2 = str2 + "1D" + Add33H(s + "011003") +
                            Add33H(getSingle(100, 0, 3)) +//A相电压检测时间
                            Add33H(getSingle(100.0, 0, 2)) +//电压合格率
                            Add33H(getSingle(10.0, 2, 2)) +//电压超限率
                            Add33H(getSingle(50, 0, 3)) +//电压超上限时间
                            Add33H(getSingle(40, 0, 3)) +//电压超下限时间
                            Add33H(getSingle(100.0, 0, 2)) +//最高电压//czx1219
                            Add33H("11123001") +//
                                                //Add33H(DateAndTime.Now.AddDays(-Convert.ToInt16(s, 16)).ToString("MM")) +//最高电压出现时间
                            Add33H(getSingle(100, 0, 2)) +//最低电压
                            Add33H("33142901");//
                        //Add33H(DateAndTime.Now.AddDays(-int.Parse(s + 1)).ToString("MM"));//最低电压出现时间
                        break;

                    case "031002":
                        str2 = str2 + "1D" + Add33H(s + "021003") +
                            Add33H(getSingle(100, 0, 3)) +
                            Add33H(getSingle(100.0, 0, 2)) +
                            Add33H(getSingle(10.0, 2, 2)) +
                            Add33H(getSingle(30, 0, 3)) +
                            Add33H(getSingle(20, 0, 3)) +
                            Add33H(getSingle(100.0, 0, 2)) +
                            Add33H("11123001") +
                            //Add33H(DateAndTime.Now.AddDays(-Convert.ToInt16(s, 16)).ToString("MM")) +
                            Add33H(getSingle(90, 0, 2)) +
                            Add33H("33142901");
                        //Add33H(DateAndTime.Now.AddDays(-int.Parse(s + 1)).ToString("MM"));
                        break;

                    case "031003":
                        str2 = str2 + "1D" + Add33H(s + "031003") +
                            Add33H(getSingle(100, 0, 3)) +
                            Add33H(getSingle(100.0, 0, 2)) +
                            Add33H(getSingle(10.0, 2, 2)) +
                            Add33H(getSingle(10, 0, 3)) +
                            Add33H(getSingle(0, 0, 3)) +
                            Add33H(getSingle(100.0, 0, 2)) +
                            Add33H("11123001") +
                            //Add33H(DateAndTime.Now.AddDays(-Convert.ToInt16(s, 16)).ToString("MM")) +
                            Add33H(getSingle(80, 0, 2)) +
                            Add33H("33142901");
                        //Add33H(DateAndTime.Now.AddDays(-int.Parse(s + 1)).ToString("MM"));
                        break;

                    case "031100":
                        if (s == "00")
                        {
                            str2 = str2 + "07" + Add33H("00001103" + getSingle(intDdcs, 0, 3));
                        }
                        else
                        {
                            str2 = str2 + "10" + Add33H(s + "001103" + strtdsj[Convert.ToInt32(s, 16) - 1].PadLeft(12, '1') + strsdsj[Convert.ToInt32(s, 16) - 1].PadLeft(12, '1'));
                        }
                        break;

                    case "031200":
                        str2 = str2 + "16" + Add33H("00001203");
                        num8 = 0;
                        while (num8 < 6)
                        {
                            str2 = str2 + Add33H(getSingle(5.0, 0, 3));
                            num8++;
                        }
                        break;

                    case "031201":
                        str2 = (str2 + "18" + Add33H("011203")) + Add33H(s + "1110010109" + s + "1210010109") + Add33H(getSingle(5.0, 3, 3) + "1210010109");
                        break;

                    case "031202":
                        str2 = (str2 + "18" + Add33H("021203")) + Add33H(s + "1110010109" + s + "1210010109") + Add33H(getSingle(5.0, 3, 3) + "1210010109");
                        break;

                    case "031203":
                        str2 = (str2 + "18" + Add33H("031203")) + Add33H(s + "1110010109" + s + "1210010109") + Add33H(getSingle(5.0, 3, 3) + "1210010109");
                        break;

                    case "031204":
                        str2 = (str2 + "18" + Add33H("041203")) + Add33H(s + "1110010109" + s + "1210010109") + Add33H(getSingle(5.0, 3, 3) + "1210010109");
                        break;

                    case "031205":
                        str2 = (str2 + "18" + Add33H("051203")) + Add33H(s + "1110010109" + s + "1210010109") + Add33H(getSingle(5.0, 3, 3) + "1210010109");
                        break;

                    case "031206":
                        str2 = (str2 + "18" + Add33H("061203")) + Add33H(s + "1110010109" + s + "1210010109") + Add33H(getSingle(5.0, 3, 3) + "1210010109");
                        break;

                    case "033000":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "36" + Add33H(s + "003003" + s + LastBCtime.ToString("mmHHddMM") + "0901010004");
                            for (num8 = 0; num8 < 10; num8++)
                            {
                                str2 = str2 + "FFFFFFFF";
                            }
                        }
                        else
                        {
                            str2 = str2 + "07" + Add33H("00003003" + getSingle((double)(BCCS % 0x3e8), 0, 3));
                        }
                        break;

                    case "033001":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "6E" + Add33H(s + "013003" + s + "111001010900000000");
                            for (num8 = 0; num8 < 0x18; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(5.0, 2, 4));
                            }
                        }
                        else
                        {
                            str2 = str2 + "07" + Add33H("00013003" + getSingle(1.0, 0, 3));
                        }
                        break;

                    case "033002":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "CE" + Add33H(s + "023003" + s + LastQLtime.ToString("mmHHddMM") + "0900000000");
                            for (num8 = 0; num8 < 0x18; num8++)
                            {
                                str2 = str2 + Add33H(getSingle(5.0, 4, 3) + DateAndTime.Now.AddDays((double)-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                            }
                        }
                        else
                        {
                            str2 = str2 + "07" + Add33H("00023003" + getSingle((double)QLCS, 0, 3));
                        }
                        break;

                    case "033003":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "12" + Add33H(s + "033003" + s + "11100101090000000001003003");
                        }
                        else
                        {
                            str2 = str2 + "07" + Add33H("00033003" + getSingle(1.0, 0, 3));
                        }
                        break;

                    case "033004":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "14" + Add33H(s + "04300300000000" + s + "1110010109111210010109");
                        }
                        else
                        {
                            str2 = str2 + "07" + Add33H("00043003" + getSingle(XSNum, 0, 3));
                        }
                        break;

                    case "033005":
                        if (!(s == "00"))
                        {
                            //str2 = str2 + "A8" + Add33H(s + "053003" + s + "111001010900000000");
                            //for (num8 = 0; num8 < 4; num8++)
                            //{
                            //    for (num9 = 0; num9 < 14; num9++)
                            //    {
                            //        str2 = str2 + ShiDuan[num8 % 8].ToString("mmhh") + FeiLei[num8 % 8];
                            //    }
                            //}
                            str2 = str2 + "3E" + Add33H(s + "053003" + LastSDtime.ToString("ssmmHHddMMyy") + "00000000");//czx1219
                            for (num8 = 0; num8 < 2; num8++)
                            {
                                for (num9 = 0; num9 < 8; num9++)
                                {
                                    //for (int i1 = 0; i1 < 14; i1++)
                                    //{
                                    str2 = str2 + Add33H(ShiDuan[num8 % 8].ToString("mmhh") + FeiLei[num8 % 8].ToString().PadLeft(2, '0'));
                                    //}
                                }
                            }
                        }
                        else
                        {
                            str2 = str2 + "07" + Add33H("00053003" + getSingle(SDBCCS, 0, 3));
                        }
                        break;

                    case "033006":
                        if (!(s == "00"))
                        {
                            str2 = str2 + "62" + Add33H(s + "063003" + s + "111001010900000000");
                            for (num8 = 0; num8 < 0x1c; num8++)
                            {
                                str2 = str2 + "010101";
                            }
                        }
                        else
                        {
                            str2 = str2 + "07" + Add33H("00063003" + getSingle(1.0, 0, 3));
                        }
                        break;

                    case "03300D":
                        if (!(s == "00"))
                        {
                            //str2 = str2 + "40" + Add33H(s + "0D3003011110011209011510011209");
                            //for (num8 = 0; num8 < 12; num8++)
                            //{
                            //    if (num8 < 6)
                            //    {
                            //        str2 = str2 + Add33H(getSingle((double)(30 - num8), 2, 4));
                            //    }
                            //    else
                            //    {
                            //        str2 = str2 + Add33H(getSingle((double)(20 - num8), 2, 4));
                            //    }
                            //}

                            str2 = str2 + (4 + MCoverOpennEvent.Length / 2).ToString("x2") + Add33H(s + "0D3003" + MCoverOpennEvent);
                        }
                        else
                        {
                            str2 = str2 + "07" + Add33H(s + "0D3003" + getSingle(MCoverOpen, 0, 3));
                        }
                        break;
                    case "03300E"://开端钮盒总次数
                        if (!(s == "00"))
                        {
                            str2 = str2 + (4 + MHeOpenEvent.Length / 2).ToString("x2") + Add33H(s + "0E3003" + MHeOpenEvent);
                        }
                        else
                        {
                            str2 = str2 + "07" + Add33H(s + "0E3003" + getSingle(MHeOpen, 0, 3));
                        }
                        break;
                    case "033201":
                        str2 = str2 + "09" + Add33H(s + "0132031010010110");
                        break;

                    case "033202":
                        str2 = str2 + "06" + Add33H(s + "023203" + getSingle(2.0, 0, 2));
                        break;

                    case "033203":
                        str2 = str2 + "08" + Add33H(s + "033203" + getSingle(100.0, 2, 4));
                        break;

                    case "033204":
                        str2 = str2 + "08" + Add33H(s + "043203" + getSingle(50.0, 2, 4));
                        break;

                    case "033205":
                        str2 = str2 + "08" + Add33H(s + "053203" + getSingle(150.0, 2, 4));
                        break;

                    case "033206":
                        str2 = str2 + "08" + Add33H(s + "063203" + getSingle(200.0, 2, 4));
                        break;

                    case "033301":
                        str2 = str2 + "09" + Add33H(s + "0133031010010110");
                        break;

                    case "033302":
                        str2 = str2 + "06" + Add33H(s + "023303" + getSingle(2.0, 0, 2));
                        break;

                    case "033303":
                        str2 = str2 + "08" + Add33H(s + "033303" + getSingle(300.0, 2, 4));
                        break;

                    case "033304":
                        str2 = str2 + "08" + Add33H(s + "043303" + getSingle(50.0, 2, 4));
                        break;

                    case "033305":
                        str2 = str2 + "08" + Add33H(s + "053303" + getSingle(350.0, 2, 4));
                        break;

                    case "033306":
                        str2 = str2 + "08" + Add33H(s + "063303" + getSingle(400.0, 2, 4));
                        break;

                    case "040001":
                        switch (s)
                        {
                            case "01":
                                if (CodeIn.Substring(0x10, 2) == "11")
                                {
                                    //string str7 = string.Format("{0:D2}", (DateBron().DayOfWeek == ~DayOfWeek.Sunday) ? (DayOfWeek.Saturday | DayOfWeek.Monday) : DateBron().DayOfWeek);
                                    string str7 = "";//czx1219
                                    if (DateBron().DayOfWeek == DayOfWeek.Sunday)
                                        str7 = string.Format("{0:D2}", 0);
                                    else if (DateBron().DayOfWeek == DayOfWeek.Monday)
                                        str7 = string.Format("{0:D2}", 1);
                                    else if (DateBron().DayOfWeek == DayOfWeek.Tuesday)
                                        str7 = string.Format("{0:D2}", 2);
                                    else if (DateBron().DayOfWeek == DayOfWeek.Wednesday)
                                        str7 = string.Format("{0:D2}", 3);
                                    else if (DateBron().DayOfWeek == DayOfWeek.Thursday)
                                        str7 = string.Format("{0:D2}", 4);
                                    else if (DateBron().DayOfWeek == DayOfWeek.Friday)
                                        str7 = string.Format("{0:D2}", 5);
                                    else if (DateBron().DayOfWeek == DayOfWeek.Saturday)
                                        str7 = string.Format("{0:D2}", 6);
                                    str2 = str2 + "08" + Add33H("01010004" + str7 + DateBron().ToString("ddMMyy"));
                                }
                                else if (CodeIn.Substring(0x10, 2) == "14")
                                {
                                    string str8 = Del33H(CodeIn.Substring(0x2e, 6));
                                    DateLoss = DateTime.Now.Day - DateTime.Parse(str8.Substring(4, 2) + "-" + str8.Substring(2, 2) + "-" + str8.Substring(0, 2)).Day;
                                }
                                break;

                            case "02":
                                if (CodeIn.Substring(0x10, 2) == "11")
                                {
                                    str2 = str2 + "07" + Add33H("02010004" + string.Format("{0:D2}", DateBron().Second)) + Add33H(string.Format("{0:D2}", DateBron().Minute)) + Add33H(string.Format("{0:D2}", DateBron().Hour));
                                }
                                else if (CodeIn.Substring(0x10, 2) == "14")
                                {
                                    string str9 = Del33H(CodeIn.Substring(0x2c, 6));
                                    TimeLoss = DateTime.Now.Minute - DateTime.Parse(str9.Substring(4, 2) + ":" + str9.Substring(2, 2) + ":" + str9.Substring(0, 2)).Minute;
                                    TimeLoss += (DateTime.Now.Hour - DateTime.Parse(str9.Substring(4, 2) + ":" + str9.Substring(2, 2) + ":" + str9.Substring(0, 2)).Hour) * 60;
                                }
                                break;

                            case "03":
                            case "04":
                            case "05":
                            case "06":
                            case "07":
                                break;
                        }
                        break;

                    case "040005":
                        switch (s)
                        {
                            case "01":
                                str2 = str2 + "06" + Add33H("01050004" + B_ZT[0].Substring(2, 2) + B_ZT[0].Substring(0, 2));
                                break;

                            case "02":
                                str2 = str2 + "06" + Add33H("02050004" + B_ZT[1].Substring(2, 2) + B_ZT[1].Substring(0, 2));
                                break;

                            case "03":
                                str2 = str2 + "06" + Add33H("03050004" + B_ZT[2].Substring(2, 2) + B_ZT[2].Substring(0, 2));
                                break;

                            case "04":
                                str2 = str2 + "06" + Add33H("04050004" + B_ZT[3].Substring(2, 2) + B_ZT[3].Substring(0, 2));
                                break;

                            case "05":
                                str2 = str2 + "06" + Add33H("05050004" + B_ZT[4].Substring(2, 2) + B_ZT[4].Substring(0, 2));
                                break;

                            case "06":
                                str2 = str2 + "06" + Add33H("06050004" + B_ZT[5].Substring(2, 2) + B_ZT[5].Substring(0, 2));
                                break;

                            case "07":
                                str2 = str2 + "06" + Add33H("07050004" + B_ZT[6].Substring(2, 2) + B_ZT[6].Substring(0, 2));
                                break;

                            case "FF":
                                str2 = str2 + "12" + Add33H("FF050004");
                                for (num8 = 0; num8 < 7; num8++)
                                {
                                    str2 = str2 + Add33H(B_ZT[num8].Substring(2, 2) + B_ZT[num8].Substring(0, 2));
                                }
                                break;
                        }
                        break;

                    case "040002":
                        switch (s)
                        {
                            case "01":
                                str2 = str2 + "05" + Add33H("0102000401");
                                break;

                            case "02":
                                str2 = str2 + "05" + Add33H("0202000401");
                                break;

                            case "03":
                                str2 = str2 + "05" + Add33H("0302000408");
                                break;

                            case "04":
                                str2 = str2 + "05" + Add33H("0402000404");
                                break;

                            case "05":
                                str2 = str2 + "06" + Add33H("050200043000");
                                break;

                            case "06":
                                str2 = str2 + "05" + Add33H("0602000415");
                                break;
                        }
                        break;

                    case "040004":
                        switch (s)
                        {
                            case "01":
                                str2 = str2 + "0A" + Add33H("0104000408000000");
                                break;

                            case "02":
                                str2 = str2 + "0A" + Add33H("0204000408000000");
                                break;

                            case "03":
                                str2 = str2 + "24" + Add33H("03040004");
                                for (num8 = 0; num8 < 4; num8++)
                                {
                                    str2 = str2 + Add33H(string.Format("{0:X2}", (byte)0x61) + string.Format("{0:X2}", (byte)0x62) + string.Format("{0:X2}", (byte)0x31) + string.Format("{0:X2}", (byte)50));
                                }
                                break;

                            case "09":
                                str6 = string.Format("{0:D6}", BiaoYgCS);
                                str2 = str2 + "07" + Add33H("09040004" + str6.Substring(4, 2) + str6.Substring(2, 2) + str6.Substring(0, 2));
                                break;

                            case "0A":
                                str6 = string.Format("{0:D6}", BiaoWgCS);
                                str2 = str2 + "07" + Add33H("0A040004" + str6.Substring(4, 2) + str6.Substring(2, 2) + str6.Substring(0, 2));
                                break;
                        }
                        break;
                    case "040006":
                        switch (s)
                        {
                            case "01":
                                str2 = str2 + "05" + Add33H("0106000403");
                                break;

                            case "02":
                                str2 = str2 + "05" + Add33H("0206000405");
                                break;

                            case "03":
                                str2 = str2 + "05" + Add33H("030600040A");
                                break;
                        }
                        break;
                    case "04000B":
                        if (!(s == "01"))
                        {
                            str2 = str2 + "06" + Add33H(s + "0B00049999");
                        }
                        else
                        {
                            str2 = str2 + "06" + Add33H("010B0004" + string.Format("{0:4}", AutoDate));
                        }
                        break;

                    case "04000F":
                        if (s == "01")//报警电量1限值
                            str2 = str2 + "08" + Add33H("010F0004" + getSingle(100, 2, 4));
                        else if (s == "02")//报警电量2限值
                            str2 = str2 + "08" + Add33H("020F0004" + getSingle(100, 2, 4));
                        else if (s == "03")//囤积电量限值
                            str2 = str2 + "08" + Add33H("030F0004" + getSingle(100, 2, 4));
                        else//透支电量限值 
                            str2 = str2 + "08" + Add33H("040F0004" + getSingle(100, 2, 4));
                        break;

                    case "040100":
                        str11 = s;
                        if ((str11 == null) || !(str11 == "00"))
                        {
                            str2 = str2 + "2E" + Add33H(s + "000104");
                            for (num8 = 0; num8 < 14; num8++)
                            {
                                num5 = (num8 < 8) ? num8 : 7;
                                str2 = str2 + Add33H(string.Format("{0:D2}", FeiLei[num5]) + string.Format("{0:D2}", ShiDuan[num5].Minute) + string.Format("{0:D2}", ShiDuan[num5].Hour));
                            }
                        }
                        else
                        {
                            str2 = str2 + "2E" + Add33H("00000104");
                            for (num8 = 0; num8 < 14; num8++)
                            {
                                str2 = str2 + "010101";
                            }
                        }
                        break;
                    case "040003":
                        switch (s)
                        {
                            case "01"://自动循环显示屏数
                                str2 = str2 + "05" + Add33H(s + "030004" + "20"); break;
                            case "02"://每屏显示时间
                                str2 = str2 + "05" + Add33H(s + "030004" + "03"); break;
                            case "03"://显示电能小数位数
                                str2 = str2 + "05" + Add33H(s + "030004" + "02"); break;
                            case "04"://显示功率（最大需量）小数位数
                                str2 = str2 + "05" + Add33H(s + "030004" + "02"); break;
                            case "05"://按键循环显示屏数
                                str2 = str2 + "05" + Add33H(s + "030004" + "20"); break;
                            case "06"://电流互感器变比
                                str2 = str2 + "07" + Add33H(s + "030004" + dlhgq); break;
                            case "07"://电压互感器变比 
                                str2 = str2 + "07" + Add33H(s + "030004" + dyhgq); break;
                            case "08"://上电全显时间 
                                str2 = str2 + "05" + Add33H(s + "030004" + "20"); break;
                        }
                        break;
                    case "040200":
                        str11 = s;
                        if ((str11 == null) || !(str11 == "00"))
                        {
                            str2 = str2 + "2E" + Add33H(s + "000204");
                            for (num8 = 0; num8 < 14; num8++)
                            {
                                num5 = (num8 < 8) ? num8 : 7;
                                str2 = str2 + Add33H(string.Format("{0:D2}", FeiLei[num5]) + string.Format("{0:D2}", ShiDuan[num5].Minute) + string.Format("{0:D2}", ShiDuan[num5].Hour));
                            }
                        }
                        else
                        {
                            str2 = str2 + "2E" + Add33H("00000204");
                            for (num8 = 0; num8 < 14; num8++)
                            {
                                str2 = str2 + "010101";
                            }
                        }
                        break;

                    case "050000":
                        str2 = str2 + "09" + Add33H(s + "0000050000" + string.Format("{0:D2}", DateBron().AddDays((double)(-Convert.ToInt16(s, 16) + 1)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)(-Convert.ToInt16(s, 16) + 1)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)(-Convert.ToInt16(s, 16) + 1)).Year % 100));
                        break;

                    case "050001":
                        str2 = str2 + "18" + Add33H(s + "010005" + getSingle(Pq[0, 0], 2, 4));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(Pq[0, (int)(num + 1)], 2, 4));
                        }

                        break;

                    case "050002":

                        str2 = str2 + "18" + Add33H(s + "020005" + getSingle(Pq[1, 0], 2, 4));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(Pq[1, (int)(num + 1)], 2, 4));
                        }
                        break;

                    case "050003":

                        str2 = str2 + "18" + Add33H(s + "030005" + getSingle(Pq[2, 0], 2, 4));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(Pq[2, (int)(num + 1)], 2, 4));
                        }

                        break;

                    case "050004":
                        str2 = str2 + "18" + Add33H(s + "040005" + getSingle(Pq[3, 0], 2, 4));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(Pq[3, (int)(num + 1)], 2, 4));
                        }

                        break;

                    case "050005":
                        str2 = str2 + "18" + Add33H(s + "050005" + getSingle(Pq[4, 0], 2, 4));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(Pq[4, (int)(num + 1)], 2, 4));
                        }

                        break;

                    case "050006":
                        str2 = str2 + "18" + Add33H(s + "060005" + getSingle(Pq[5, 0], 2, 4));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(Pq[5, (int)(num + 1)], 2, 4));
                        }
                        break;

                    case "050007":
                        str2 = str2 + "18" + Add33H(s + "070005" + getSingle(Pq[6, 0], 2, 4));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(Pq[6, (int)(num + 1)], 2, 4));
                        }
                        break;

                    case "050008":

                        str2 = str2 + "18" + Add33H(s + "080005" + getSingle(Pq[7, 0], 2, 4));
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(Pq[7, (int)(num + 1)], 2, 4));
                            num8++;
                        }

                        break;

                    case "050009":

                        str2 = str2 + "2C" + Add33H(s + "090005" + getSingle(Zdxl[0, 0], 4, 3) + ZdxlTime[0, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        for (num8 = 0; num8 <= 3; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(Zdxl[0, (int)(num8 + 1)], 4, 3) + ZdxlTime[0, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        }

                        break;

                    case "05000A":
                        str2 = str2 + "2C" + Add33H(s + "0A0005" + getSingle(Zdxl[1, 0], 4, 3) + ZdxlTime[1, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(Zdxl[1, (int)(num8 + 1)], 4, 3) + ZdxlTime[1, (int)(num8 + 1)].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        }
                        break;

                    case "050010":
                        str2 = str2 + "1C" + Add33H(s + "100005" + getSingle(9.0, 4, 3));
                        num8 = 0;
                        while (num8 < 3)
                        {
                            str2 = str2 + Add33H(getSingle(3.0, 4, 3));
                            num8++;
                        }
                        str2 = str2 + Add33H(getSingle(6.0, 4, 3));
                        for (num8 = 0; num8 < 3; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(2.0, 4, 3));
                        }
                        break;
                    case "050201":
                        str2 = str2 + "08" + Add33H(s + "010205" + getSingle(PqDj[0, 0], 2, 4));
                        break;

                    case "050600":
                        str2 = str2 + "09" + Add33H(s + "000605" + string.Format("{0:D2}", 0) + string.Format("{0:D2}", 0) + string.Format("{0:D2}", DateBron().AddDays((double)(-Convert.ToInt16(s, 16) + 1)).Day) + string.Format("{0:D2}", DateBron().AddDays((double)(-Convert.ToInt16(s, 16) + 1)).Month) + string.Format("{0:D2}", DateBron().AddDays((double)(-Convert.ToInt16(s, 16) + 1)).Year % 100));
                        break;

                    case "050601":
                        str2 = str2 + "18" + Add33H(s + "010605" + getSingle(PqDj[0, 0], 2, 4));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(PqDj[0, (int)(num8 + 1)], 2, 4));
                        }

                        break;

                    case "050602":

                        str2 = str2 + "18" + Add33H(s + "020605" + getSingle(PqDj[1, 0], 2, 4));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(PqDj[1, (int)(num8 + 1)], 2, 4));
                        }

                        break;

                    case "050603":

                        str2 = str2 + "18" + Add33H(s + "030605" + getSingle(PqDj[2, 0], 2, 4));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(PqDj[2, (int)(num8 + 1)], 2, 4));
                        }


                        break;

                    case "050604":
                        str2 = str2 + "18" + Add33H(s + "040605" + getSingle(PqDj[3, 0], 2, 4));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(PqDj[3, (int)(num8 + 1)], 2, 4));
                        }
                        break;

                    case "050605":
                        str2 = str2 + "18" + Add33H(s + "050605" + getSingle(PqDj[4, 0], 2, 4));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(PqDj[4, (int)(num8 + 1)], 2, 4));
                        }

                        break;

                    case "050606":
                        str2 = str2 + "18" + Add33H(s + "060605" + getSingle(PqDj[5, 0], 2, 4));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(PqDj[5, (int)(num8 + 1)], 2, 4));
                        }

                        break;

                    case "050607":

                        str2 = str2 + "18" + Add33H(s + "070605" + getSingle(PqDj[6, 0], 2, 4));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(PqDj[6, (int)(num8 + 1)], 2, 4));
                        }

                        break;

                    case "050608":

                        str2 = str2 + "18" + Add33H(s + "080605" + getSingle(PqDj[7, 0], 2, 4));
                        num8 = 0;
                        while (num8 < 4)
                        {
                            str2 = str2 + Add33H(getSingle(PqDj[7, (int)(num8 + 1)], 2, 4));
                            num8++;
                        }

                        break;

                    case "050609":

                        str2 = str2 + "2C" + Add33H(s + "090605" + getSingle(ZdxlDj[0, 0], 4, 3) + ZdxlTime[0, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));

                        //str2 = str2 + "2C" + Add33H(s + "090605" + getSingle(Zdxl[0, 0], 4, 3) + ZdxlTime[0, 0].ToString("mmHHddMMyy"));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(ZdxlDj[0, (int)(num8 + 1)], 4, 3) + ZdxlTime[0, (int)(num8 + 1)].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                            //str2 = str2 + Add33H(getSingle(Zdxl[0, (int)(num8 + 1)], 4, 3) + ZdxlTime[0, (int)(num8 + 1)].ToString("mmHHddMMyy"));
                        }

                        break;

                    case "05060A":
                        str2 = str2 + "2C" + Add33H(s + "0A0605" + getSingle(Zdxl[1, 0], 4, 3) + ZdxlTime[1, 0].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        for (num8 = 0; num8 < 4; num8++)
                        {
                            str2 = str2 + Add33H(getSingle(Zdxl[1, (int)(num8 + 1)], 4, 3) + ZdxlTime[1, (int)(num8 + 1)].AddMonths(-Convert.ToInt16(s, 16)).ToString("mmHHddMMyy"));
                        }
                        break;

                    case "050400":
                        str2 = str2 + "09" + Add33H(s + "00040500" + DateBron().AddHours((double)(-Convert.ToInt16(s, 16) + 1)).ToString("HHddMMyy"));
                        break;

                    case "050401":
                        str2 = str2 + "08" + Add33H(s + "010405" + getSingle(Pq[0, 0], 2, 4));
                        break;

                    case "050402":
                        str2 = str2 + "08" + Add33H(s + "020405" + getSingle(Pq[1, 0], 2, 4));
                        break;

                    case "0504FF":
                        str2 = str2 + "14" + Add33H(s + "FF040500" + DateBron().AddHours((double)(-Convert.ToInt16(s, 16) + 1)).ToString("HHddMMyy") + "AA" + getSingle(Pq[0, 0], 2, 4) + "AA" + getSingle(Pq[1, 0], 2, 4) + "AA");
                        break;

                    case "052001":
                        str2 = str2 + "1C" + Add33H(s + "012005" + getSingle(220.0, 1, 2) + getSingle(220.0, 1, 2) + getSingle(220.0, 1, 2) + getSingle(220.0, 1, 2) + getSingle(220.0, 1, 2) + getSingle(220.0, 1, 2) + getSingle(220.0, 1, 2) + getSingle(220.0, 1, 2) + getSingle(220.0, 1, 2) + getSingle(220.0, 1, 2) + getSingle(220.0, 1, 2) + getSingle(220.0, 1, 2));

                        break;

                    case "052009":
                        str2 = str2 + "15" + Add33H(s + "092005" + DateBron().ToString("mmHHddMMyy") + getSingle(13.2, 2, 4) + getSingle(10.1, 2, 4) + getSingle(13.1, 2, 4));
                        break;
                    case "1D0001":
                        if (!(s == "01"))
                        {
                            str2 = str2.Substring(0, 0x10) + "D101" + string.Format("{0:X2}", (int)Math.Pow(2.0, 6.0));
                        }
                        else
                        {
                            str2 = str2 + "0A" + Add33H(s + "01001D101110051209");
                        }
                        break;

                    case "1E0001":
                        if (!(s == "01"))
                        {
                            str2 = str2.Substring(0, 0x10) + "D101" + string.Format("{0:X2}", (int)Math.Pow(2.0, 6.0));
                        }
                        else
                        {
                            str2 = str2 + "0A" + Add33H(s + "01001E101610051209");
                        }
                        break;

                    case "060000":
                        if ((DateBron().Minute < 0) || (DateBron().Minute >= 15))
                        {
                            if ((DateBron().Minute >= 15) && (DateBron().Minute < 30))
                            {
                                num4 = 15;
                            }
                            else if ((DateBron().Minute >= 30) && (DateBron().Minute < 0x2d))
                            {
                                num4 = 30;
                            }
                            else
                            {
                                num4 = 0x2d;
                            }
                        }
                        else
                        {
                            num4 = 0;
                        }
                        str5 = "A0A062" + string.Format("{0:D2}", num4) + string.Format("{0:D2}", DateBron().Hour) + string.Format("{0:D2}", DateBron().Day) + string.Format("{0:D2}", DateBron().Month) + string.Format("{0:D2}", DateBron().Year % 100) + "0022002200220000010000010000010050AA000030000010000010000010000030000010000010000010AA0003000100010001AA00200000001600000020000000200000AA00040000000400000004000000040000AA000010000010AA";
                        str5 = str5 + getChk(str5) + "E5";
                        str2 = str2 + "6B" + Add33H(s + "000006" + str5);
                        break;

                    case "060100":
                        if ((DateBron().Minute < 0) || (DateBron().Minute >= 15))
                        {
                            if ((DateBron().Minute >= 15) && (DateBron().Minute < 30))
                            {
                                num4 = 15;
                            }
                            else if ((DateBron().Minute >= 30) && (DateBron().Minute < 0x2d))
                            {
                                num4 = 30;
                            }
                            else
                            {
                                num4 = 0x2d;
                            }
                        }
                        else
                        {
                            num4 = 0;
                        }
                        str5 = "A0A01C" + string.Format("{0:D2}", num4) + string.Format("{0:D2}", DateBron().Hour) + string.Format("{0:D2}", DateBron().Day) + string.Format("{0:D2}", DateBron().Month) + string.Format("{0:D2}", DateBron().Year % 100) + "0022002200220060000060000060000050AAAAAAAAAAAA";
                        str5 = str5 + getChk(str5) + "E5";
                        str2 = str2 + "25" + Add33H(s + revStr(pstr) + str5);
                        break;

                    case "060200":
                        if ((DateBron().Minute < 0) || (DateBron().Minute >= 15))
                        {
                            if ((DateBron().Minute >= 15) && (DateBron().Minute < 30))
                            {
                                num4 = 15;
                            }
                            else if ((DateBron().Minute >= 30) && (DateBron().Minute < 0x2d))
                            {
                                num4 = 30;
                            }
                            else
                            {
                                num4 = 0x2d;
                            }
                        }
                        else
                        {
                            num4 = 0;
                        }
                        str5 = "A0A023" + string.Format("{0:D2}", num4) + string.Format("{0:D2}", DateBron().Hour) + string.Format("{0:D2}", DateBron().Day) + string.Format("{0:D2}", DateBron().Month) + string.Format("{0:D2}", DateBron().Year % 100) + "AA000030000010000010000010000030000010000010000010AAAAAAAAAA";
                        str5 = str5 + getChk(str5) + "E5";
                        str2 = str2 + "2C" + Add33H(s + revStr(pstr) + str5);
                        break;

                    case "060300":
                        if ((DateBron().Minute < 0) || (DateBron().Minute >= 15))
                        {
                            if ((DateBron().Minute >= 15) && (DateBron().Minute < 30))
                            {
                                num4 = 15;
                            }
                            else if ((DateBron().Minute >= 30) && (DateBron().Minute < 0x2d))
                            {
                                num4 = 30;
                            }
                            else
                            {
                                num4 = 0x2d;
                            }
                        }
                        else
                        {
                            num4 = 0;
                        }
                        str5 = "A0A013" + string.Format("{0:D2}", num4) + string.Format("{0:D2}", DateBron().Hour) + string.Format("{0:D2}", DateBron().Day) + string.Format("{0:D2}", DateBron().Month) + string.Format("{0:D2}", DateBron().Year % 100) + "AAAA0003000100010001AAAAAAAA";
                        str5 = str5 + getChk(str5) + "E5";
                        str2 = str2 + "1C" + Add33H(s + revStr(pstr) + str5);
                        break;

                    case "060400":
                        if ((DateBron().Minute < 0) || (DateBron().Minute >= 15))
                        {
                            if ((DateBron().Minute >= 15) && (DateBron().Minute < 30))
                            {
                                num4 = 15;
                            }
                            else if ((DateBron().Minute >= 30) && (DateBron().Minute < 0x2d))
                            {
                                num4 = 30;
                            }
                            else
                            {
                                num4 = 0x2d;
                            }
                        }
                        else
                        {
                            num4 = 0;
                        }
                        str5 = "A0A01B" + string.Format("{0:D2}", num4) + string.Format("{0:D2}", DateBron().Hour) + string.Format("{0:D2}", DateBron().Day) + string.Format("{0:D2}", DateBron().Month) + string.Format("{0:D2}", DateBron().Year % 100) + "AAAAAA00200000001600000020000000200000AAAAAA";
                        str5 = str5 + getChk(str5) + "E5";
                        str2 = str2 + "24" + Add33H(s + revStr(pstr) + str5);
                        break;

                    case "060500":
                        if ((DateBron().Minute < 0) || (DateBron().Minute >= 15))
                        {
                            if ((DateBron().Minute >= 15) && (DateBron().Minute < 30))
                            {
                                num4 = 15;
                            }
                            else if ((DateBron().Minute >= 30) && (DateBron().Minute < 0x2d))
                            {
                                num4 = 30;
                            }
                            else
                            {
                                num4 = 0x2d;
                            }
                        }
                        else
                        {
                            num4 = 0;
                        }
                        str5 = "A0A01B" + string.Format("{0:D2}", num4) + string.Format("{0:D2}", DateBron().Hour) + string.Format("{0:D2}", DateBron().Day) + string.Format("{0:D2}", DateBron().Month) + string.Format("{0:D2}", DateBron().Year % 100) + "AAAAAAAA00040000000400000004000000040000AAAA";
                        str5 = str5 + getChk(str5) + "E5";
                        str2 = str2 + "24" + Add33H(s + revStr(pstr) + str5);
                        break;

                    case "060600":
                        if ((DateBron().Minute < 0) || (DateBron().Minute >= 15))
                        {
                            if ((DateBron().Minute >= 15) && (DateBron().Minute < 30))
                            {
                                num4 = 15;
                            }
                            else if ((DateBron().Minute >= 30) && (DateBron().Minute < 0x2d))
                            {
                                num4 = 30;
                            }
                            else
                            {
                                num4 = 0x2d;
                            }
                        }
                        else
                        {
                            num4 = 0;
                        }
                        str5 = "A0A011" + string.Format("{0:D2}", num4) + string.Format("{0:D2}", DateBron().Hour) + string.Format("{0:D2}", DateBron().Day) + string.Format("{0:D2}", DateBron().Month) + string.Format("{0:D2}", DateBron().Year % 100) + "AAAAAAAAAA000010000010AA";
                        str5 = str5 + getChk(str5) + "E5";
                        str2 = str2 + "1A" + Add33H(s + revStr(pstr) + str5);
                        break;
                    case "040015":
                        if (s == "01")
                        {
                            str2 = str2 + "10" + Add33H("01150004" + "000000000000000000000000");
                        }
                        break;
                    case "190100"://过流总次数
                        if (s == "01")
                        {
                            str2 = str2 + "07" + Add33H("01000119" + getSingle(Cmd19010001, 0, 3));
                        }
                        break;
                    case "190101":
                        if (s == "01")
                        {
                            str2 = str2 + "0A" + Add33H("01010119" + Cmd19010101);
                        }
                        break;
                    case "190121":
                        if (s == "01")
                        {
                            str2 = str2 + "0A" + Add33H("01210119" + Cmd19012101);
                        }
                        break;
                    default:
                        if (IsLog)
                        {
                            string strFileName = "err";
                            string strLogMessage = "未能返回数据DI:" + str11 + s;
                            LogInfoHelper.WriteLog(strFileName, strLogMessage);
                        }
                        if (str2.Length > 16)
                            str2 = str2.Substring(0, 0x10) + "D101" + string.Format("{0:X2}", 1);
                        break;
                }
                if ((str2.Length == 0x12) && (pstr.Substring(0, 1) == "9"))
                {
                    str2 = str2.Substring(0, 0x10) + "D101" + string.Format("{0:X2}", 1);
                }
                else if (str2.Length == 0x12)
                {
                    str2 = str2.Substring(0, 0x10) + "D10101";
                }
                return (str2 + getChk(str2) + "16");
            }
            catch
            {
                return "FF";
            }
        }

        private string DLStringDeal(string Tsou)
        {
            string Tstr = Tsou;
            int Tint;
            Tint = Convert.ToByte(Tsou.Substring(18, 2), 16) + 1;
            Tstr = Tsou.Substring(0, 18) + string.Format("{0:X2}", Tint) + Tsou.Substring(20);
            return Tstr;
        }
        public string CodeReturn1997(string CodeIn, string BiaoDz)
        {
            string str2;
            double num;
            string str3;
            string str4;
            int num2;
            double num3 = Math.Pow((Pa + Pb) + Pc, 2.0);
            double num4 = Math.Pow((Qa + Qb) + Qc, 2.0);
            num3 = Math.Pow(num3 + num4, 0.5);
            if (!XiaoYan(CodeIn))
            {
                return "FF";
            }
            if ((((((Pa + Pb) + Pc) == 0.0) && (Ia != 0.0)) && (Ib != 0.0)) && (Ic != 0.0))
            {
                num = 0.0;
            }
            else if (((Ia == 0.0) && (Ib == 0.0)) && (Ic == 0.0))
            {
                num = 1.0;
            }
            else
            {
                num = ((Pa + Pb) + Pc) / num3;
            }
            string tstrAddr = CodeIn.Substring(2, 12);
            if ((tstrAddr != "999999999999") && (tstrAddr.ToUpper() != "AAAAAAAAAAAA"))
            {
                return "FF";
            }
            else
            {
                BiaoDz = BiaoDz.PadLeft(12, '0');
                tstrAddr = BiaoDz.Substring(10, 2) + BiaoDz.Substring(8, 2) + BiaoDz.Substring(6, 2) + BiaoDz.Substring(4, 2) + BiaoDz.Substring(2, 2) + BiaoDz.Substring(0, 2);
                if (CodeIn.Substring(0x10, 2).ToUpper() == "1F")
                {
                    str2 = "68" + tstrAddr + "689F08E235" + Add33H(tstrAddr);
                    return (str2 + getChk(str2) + "16");
                }
                if (CodeIn.Substring(0x10, 2).ToUpper() == "06")
                {
                    str2 = "68" + tstrAddr + "688606" + Add33H(tstrAddr);
                    return (str2 + getChk(str2) + "16");
                }
                if ((CodeIn.Substring(0x10, 2).ToUpper() == "01") && (CodeIn.Substring(20, 4).ToUpper() == "65F3"))
                {
                    str2 = "68" + tstrAddr + "68810865F3" + tstrAddr;
                    return (str2 + getChk(str2) + "16");
                }
            }
            str2 = CodeIn.Substring(0, 0x10) + "81";
            string str = CodeIn.Substring(0x16, 2);
            str = string.Format("{0:X2}", Convert.ToByte(CodeIn.Substring(0x16, 2), 16) - 0x33) + string.Format("{0:X2}", Convert.ToByte(CodeIn.Substring(20, 2), 16) - 0x33);
            switch (str)
            {
                case "9010":
                    str2 = str2 + "06" + Add33H("1090" + getSingle(Pq[0, 0], 2, 4));
                    break;

                case "9011":
                    str2 = str2 + "06" + Add33H("1190" + getSingle(Pq[0, 1], 2, 4));
                    break;

                case "9012":
                    str2 = str2 + "06" + Add33H("1290" + getSingle(Pq[0, 2], 2, 4));
                    break;

                case "9013":
                    str2 = str2 + "06" + Add33H("1390" + getSingle(Pq[0, 3], 2, 4));
                    break;

                case "9014":
                    str2 = str2 + "06" + Add33H("1490" + getSingle(Pq[0, 4], 2, 4));
                    break;

                case "901F":
                    str2 = str2 + "16" + Add33H("1F90" + getSingle(Pq[0, 0], 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[0, num2], 2, 4));
                    }
                    break;

                case "9020":
                    str2 = str2 + "06" + Add33H("2090" + getSingle(Pq[1, 0], 2, 4));
                    break;

                case "9021":
                    str2 = str2 + "06" + Add33H("2190" + getSingle(Pq[1, 1], 2, 4));
                    break;

                case "9022":
                    str2 = str2 + "06" + Add33H("2290" + getSingle(Pq[1, 2], 2, 4));
                    break;

                case "9023":
                    str2 = str2 + "06" + Add33H("2390" + getSingle(Pq[1, 3], 2, 4));
                    break;

                case "9024":
                    str2 = str2 + "06" + Add33H("2490" + getSingle(Pq[1, 4], 2, 4));
                    break;

                case "902F":
                    str2 = str2 + "16" + Add33H("2F90" + getSingle(Pq[1, 0], 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[1, num2], 2, 4));
                    }
                    break;

                case "90FF":
                    str2 = str2 + "2C" + Add33H("FF90" + getSingle(Pq[0, 0], 2, 4));
                    num2 = 1;
                    while (num2 <= 4)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[0, num2], 2, 4));
                        num2++;
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(Pq[1, 0], 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[1, num2], 2, 4));
                    }
                    str2 = str2 + "DD";
                    break;

                case "9110":

                    str2 = str2 + "06" + Add33H("1091" + getSingle(Pq[2, 0], 2, 4));

                    break;

                case "9111":

                    str2 = str2 + "06" + Add33H("1191" + getSingle(Pq[2, 1], 2, 4));

                    break;

                case "9112":

                    str2 = str2 + "06" + Add33H("1291" + getSingle(Pq[2, 2], 2, 4));

                    break;

                case "9113":

                    str2 = str2 + "06" + Add33H("1391" + getSingle(Pq[2, 3], 2, 4));

                    break;

                case "9114":

                    str2 = str2 + "06" + Add33H("1491" + getSingle(Pq[2, 4], 2, 4));

                    break;

                case "911F":

                    str2 = str2 + "16" + Add33H("1F91" + getSingle(Pq[2, 0], 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[2, num2], 2, 4));
                    }

                    break;

                case "9120":
                    str2 = str2 + "06" + Add33H("2091" + getSingle(Pq[3, 0], 2, 4));
                    break;

                case "9121":
                    str2 = str2 + "06" + Add33H("2191" + getSingle(Pq[3, 1], 2, 4));
                    break;

                case "9122":
                    str2 = str2 + "06" + Add33H("2291" + getSingle(Pq[3, 2], 2, 4));
                    break;

                case "9123":
                    str2 = str2 + "06" + Add33H("2391" + getSingle(Pq[3, 3], 2, 4));
                    break;

                case "9124":
                    str2 = str2 + "06" + Add33H("2491" + getSingle(Pq[3, 4], 2, 4));
                    break;

                case "912F":
                    str2 = str2 + "16" + Add33H("2F91" + getSingle(Pq[3, 0], 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[3, num2], 2, 4));
                    }
                    break;

                case "9130":
                    str2 = str2 + "06" + Add33H("3091" + getSingle(Pq[4, 0], 2, 4));
                    break;

                case "9131":
                    str2 = str2 + "06" + Add33H("3191" + getSingle(Pq[4, 1], 2, 4));
                    break;

                case "9132":
                    str2 = str2 + "06" + Add33H("3291" + getSingle(Pq[4, 2], 2, 4));
                    break;

                case "9133":
                    str2 = str2 + "06" + Add33H("3391" + getSingle(Pq[4, 3], 2, 4));
                    break;

                case "9134":
                    str2 = str2 + "06" + Add33H("3491" + getSingle(Pq[4, 4], 2, 4));
                    break;

                case "913F":
                    str2 = str2 + "16" + Add33H("3F91" + getSingle(Pq[4, 0], 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[4, num2], 2, 4));
                    }
                    break;

                case "9140":
                    str2 = str2 + "06" + Add33H("4091" + getSingle(Pq[7, 0], 2, 4));
                    break;

                case "9141":
                    str2 = str2 + "06" + Add33H("4191" + getSingle(Pq[7, 1], 2, 4));
                    break;

                case "9142":
                    str2 = str2 + "06" + Add33H("4291" + getSingle(Pq[7, 2], 2, 4));
                    break;

                case "9143":
                    str2 = str2 + "06" + Add33H("4391" + getSingle(Pq[7, 3], 2, 4));
                    break;

                case "9144":
                    str2 = str2 + "06" + Add33H("4491" + getSingle(Pq[7, 4], 2, 4));
                    break;

                case "914F":
                    str2 = str2 + "16" + Add33H("4F91" + getSingle(Pq[7, 0], 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[7, num2], 2, 4));
                    }
                    break;

                case "9150":
                    str2 = str2 + "06" + Add33H("5091" + getSingle(Pq[5, 0], 2, 4));
                    break;

                case "9151":
                    str2 = str2 + "06" + Add33H("5191" + getSingle(Pq[5, 1], 2, 4));
                    break;

                case "9152":
                    str2 = str2 + "06" + Add33H("5291" + getSingle(Pq[5, 2], 2, 4));
                    break;

                case "9153":
                    str2 = str2 + "06" + Add33H("5391" + getSingle(Pq[5, 3], 2, 4));
                    break;

                case "9154":
                    str2 = str2 + "06" + Add33H("5491" + getSingle(Pq[5, 4], 2, 4));
                    break;

                case "915F":
                    str2 = str2 + "16" + Add33H("5F91" + getSingle(Pq[5, 0], 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[5, num2], 2, 4));
                    }
                    break;

                case "9160":
                    str2 = str2 + "06" + Add33H("6091" + getSingle(Pq[6, 0], 2, 4));
                    break;

                case "9161":
                    str2 = str2 + "06" + Add33H("6191" + getSingle(Pq[6, 1], 2, 4));
                    break;

                case "9162":
                    str2 = str2 + "06" + Add33H("6291" + getSingle(Pq[6, 2], 2, 4));
                    break;

                case "9163":
                    str2 = str2 + "06" + Add33H("6391" + getSingle(Pq[6, 3], 2, 4));
                    break;

                case "9164":
                    str2 = str2 + "06" + Add33H("6491" + getSingle(Pq[6, 4], 2, 4));
                    break;

                case "916F":
                    str2 = str2 + "16" + Add33H("6F91" + getSingle(Pq[6, 0], 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[6, num2], 2, 4));
                    }
                    break;

                case "91FF":
                    str2 = str2 + "80" + Add33H("FF91" + getSingle(Pq[2, 0], 2, 4));
                    num2 = 1;
                    while (num2 <= 4)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[2, num2], 2, 4));
                        num2++;
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(Pq[3, 0], 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[3, num2], 2, 4));
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(Pq[4, 0], 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[4, num2], 2, 4));
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(Pq[7, 0], 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[7, num2], 2, 4));
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(Pq[5, 0], 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[5, num2], 2, 4));
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(Pq[6, 0], 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[6, num2], 2, 4));
                    }
                    str2 = str2 + "DD";
                    break;

                case "91E0":
                    str2 = str2 + "32" + Add33H("E091" + getSingle(Pq[0, 0] / 3.0, 2, 4) + getSingle(Pq[0, 0] / 3.0, 2, 4) + getSingle(Pq[0, 0] / 3.0, 2, 4));
                    for (num2 = 1; num2 <= 3; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Pq[num2, 0] / 3.0, 2, 4)) + Add33H(getSingle(Pq[num2, 0] / 3.0, 2, 4)) + Add33H(getSingle(Pq[num2, 0] / 3.0, 2, 4));
                    }
                    break;

                case "9410":
                    str2 = str2 + "06" + Add33H("1094" + getSingle(36.4, 2, 4));
                    break;

                case "9411":
                    str2 = str2 + "06" + Add33H("1194" + getSingle(9.1, 2, 4));
                    break;

                case "9412":
                    str2 = str2 + "06" + Add33H("1294" + getSingle(9.1, 2, 4));
                    break;

                case "9413":
                    str2 = str2 + "06" + Add33H("1394" + getSingle(9.1, 2, 4));
                    break;

                case "9414":
                    str2 = str2 + "06" + Add33H("1494" + getSingle(9.1, 2, 4));
                    break;

                case "941F":
                    str2 = str2 + "16" + Add33H("1F94" + getSingle(36.4, 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(9.1, 2, 4));
                    }
                    break;

                case "9420":
                    str2 = str2 + "06" + Add33H("2094" + getSingle(32.0, 2, 4));
                    break;

                case "9421":
                    str2 = str2 + "06" + Add33H("2194" + getSingle(8.0, 2, 4));
                    break;

                case "9422":
                    str2 = str2 + "06" + Add33H("2294" + getSingle(8.0, 2, 4));
                    break;

                case "9423":
                    str2 = str2 + "06" + Add33H("2394" + getSingle(8.0, 2, 4));
                    break;

                case "9424":
                    str2 = str2 + "06" + Add33H("2494" + getSingle(8.0, 2, 4));
                    break;

                case "942F":
                    str2 = str2 + "16" + Add33H("2F94" + getSingle(32.0, 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(8.0, 2, 4));
                    }
                    break;

                case "94FF":
                    str2 = str2 + "2C" + Add33H("FF94" + getSingle(36.4, 2, 4));
                    num2 = 1;
                    while (num2 <= 4)
                    {
                        str2 = str2 + Add33H(getSingle(9.1, 2, 4));
                        num2++;
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(32.0, 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(8.0, 2, 4));
                    }
                    str2 = str2 + "DD";
                    break;

                case "9510":
                    str2 = str2 + "06" + Add33H("1095" + getSingle(28.0, 2, 4));
                    break;

                case "9511":
                    str2 = str2 + "06" + Add33H("1195" + getSingle(7.0, 2, 4));
                    break;

                case "9512":
                    str2 = str2 + "06" + Add33H("1295" + getSingle(7.0, 2, 4));
                    break;

                case "9513":
                    str2 = str2 + "06" + Add33H("1395" + getSingle(7.0, 2, 4));
                    break;

                case "9514":
                    str2 = str2 + "06" + Add33H("1495" + getSingle(7.0, 2, 4));
                    break;

                case "951F":
                    str2 = str2 + "16" + Add33H("1F95" + getSingle(28.0, 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(7.0, 2, 4));
                    }
                    break;

                case "9520":
                    str2 = str2 + "06" + Add33H("2095" + getSingle(24.0, 2, 4));
                    break;

                case "9521":
                    str2 = str2 + "06" + Add33H("2195" + getSingle(6.0, 2, 4));
                    break;

                case "9522":
                    str2 = str2 + "06" + Add33H("2295" + getSingle(6.0, 2, 4));
                    break;

                case "9523":
                    str2 = str2 + "06" + Add33H("2395" + getSingle(6.0, 2, 4));
                    break;

                case "9524":
                    str2 = str2 + "06" + Add33H("2495" + getSingle(6.0, 2, 4));
                    break;

                case "952F":
                    str2 = str2 + "16" + Add33H("2F95" + getSingle(24.0, 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(6.0, 2, 4));
                    }
                    break;

                case "9530":
                    str2 = str2 + "06" + Add33H("3095" + getSingle(20.0, 2, 4));
                    break;

                case "9531":
                    str2 = str2 + "06" + Add33H("3195" + getSingle(5.0, 2, 4));
                    break;

                case "9532":
                    str2 = str2 + "06" + Add33H("3295" + getSingle(5.0, 2, 4));
                    break;

                case "9533":
                    str2 = str2 + "06" + Add33H("3395" + getSingle(5.0, 2, 4));
                    break;

                case "9534":
                    str2 = str2 + "06" + Add33H("3495" + getSingle(5.0, 2, 4));
                    break;

                case "953F":
                    str2 = str2 + "16" + Add33H("3F95" + getSingle(20.0, 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(5.0, 2, 4));
                    }
                    break;

                case "9540":
                    str2 = str2 + "06" + Add33H("4095" + getSingle(8.0, 2, 4));
                    break;

                case "9541":
                    str2 = str2 + "06" + Add33H("4195" + getSingle(2.0, 2, 4));
                    break;

                case "9542":
                    str2 = str2 + "06" + Add33H("4295" + getSingle(2.0, 2, 4));
                    break;

                case "9543":
                    str2 = str2 + "06" + Add33H("4395" + getSingle(2.0, 2, 4));
                    break;

                case "9544":
                    str2 = str2 + "06" + Add33H("4495" + getSingle(2.0, 2, 4));
                    break;

                case "954F":
                    str2 = str2 + "16" + Add33H("4F95" + getSingle(8.0, 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(2.0, 2, 4));
                    }
                    break;

                case "9550":
                    str2 = str2 + "06" + Add33H("5095" + getSingle(16.0, 2, 4));
                    break;

                case "9551":
                    str2 = str2 + "06" + Add33H("5195" + getSingle(4.0, 2, 4));
                    break;

                case "9552":
                    str2 = str2 + "06" + Add33H("5295" + getSingle(4.0, 2, 4));
                    break;

                case "9553":
                    str2 = str2 + "06" + Add33H("5395" + getSingle(4.0, 2, 4));
                    break;

                case "9554":
                    str2 = str2 + "06" + Add33H("5495" + getSingle(4.0, 2, 4));
                    break;

                case "955F":
                    str2 = str2 + "16" + Add33H("5F95" + getSingle(16.0, 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(4.0, 2, 4));
                    }
                    break;

                case "9560":
                    str2 = str2 + "06" + Add33H("6095" + getSingle(12.0, 2, 4));
                    break;

                case "9561":
                    str2 = str2 + "06" + Add33H("6195" + getSingle(3.0, 2, 4));
                    break;

                case "9562":
                    str2 = str2 + "06" + Add33H("6295" + getSingle(3.0, 2, 4));
                    break;

                case "9563":
                    str2 = str2 + "06" + Add33H("6395" + getSingle(3.0, 2, 4));
                    break;

                case "9564":
                    str2 = str2 + "06" + Add33H("6495" + getSingle(3.0, 2, 4));
                    break;

                case "956F":
                    str2 = str2 + "16" + Add33H("6F95" + getSingle(12.0, 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(3.0, 2, 4));
                    }
                    break;

                case "95FF":
                    str2 = str2 + "80" + Add33H("FF95" + getSingle(28.0, 2, 4));
                    num2 = 1;
                    while (num2 <= 4)
                    {
                        str2 = str2 + Add33H(getSingle(7.0, 2, 4));
                        num2++;
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(24.0, 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(6.0, 2, 4));
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(20.0, 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(5.0, 2, 4));
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(8.0, 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(2.0, 2, 4));
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(16.0, 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(4.0, 2, 4));
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(12.0, 2, 4));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(3.0, 2, 4));
                    }
                    str2 = str2 + "DD";
                    break;

                case "A010":
                    str2 = str2 + "05" + Add33H("10A0" + getSingle(Zdxl[0, 0], 4, 3));
                    break;

                case "A011":
                    str2 = str2 + "05" + Add33H("11A0" + getSingle(Zdxl[0, 1], 4, 3));
                    break;

                case "A012":
                    str2 = str2 + "05" + Add33H("12A0" + getSingle(Zdxl[0, 2], 4, 3));
                    break;

                case "A013":
                    str2 = str2 + "05" + Add33H("13A0" + getSingle(Zdxl[0, 3], 4, 3));
                    break;

                case "A014":
                    str2 = str2 + "05" + Add33H("14A0" + getSingle(Zdxl[0, 4], 4, 3));
                    break;

                case "A01F":
                    str2 = str2 + "12" + Add33H("1FA0" + getSingle(Zdxl[0, 0], 4, 3));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Zdxl[0, num2], 4, 3));
                    }
                    str2 = str2 + "DD";
                    break;

                case "A020":
                    str2 = str2 + "05" + Add33H("20A0" + getSingle(Zdxl[1, 0], 4, 3));
                    break;

                case "A021":
                    str2 = str2 + "05" + Add33H("21A0" + getSingle(Zdxl[1, 1], 4, 3));
                    break;

                case "A022":
                    str2 = str2 + "05" + Add33H("22A0" + getSingle(Zdxl[1, 2], 4, 3));
                    break;

                case "A023":
                    str2 = str2 + "05" + Add33H("23A0" + getSingle(Zdxl[1, 3], 4, 3));
                    break;

                case "A024":
                    str2 = str2 + "05" + Add33H("24A0" + getSingle(Zdxl[1, 4], 4, 3));
                    break;

                case "A02F":
                    str2 = str2 + "12" + Add33H("2FA0" + getSingle(Zdxl[1, 0], 4, 3));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Zdxl[1, num2], 4, 3));
                    }
                    str2 = str2 + "DD";
                    break;

                case "A0FF":
                    str2 = str2 + "22" + Add33H("FFA0" + getSingle(Zdxl[0, 0], 4, 3));
                    num2 = 1;
                    while (num2 <= 4)
                    {
                        str2 = str2 + Add33H(getSingle(Zdxl[0, num2], 4, 3));
                        num2++;
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(Zdxl[1, 0], 4, 3));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Zdxl[1, num2], 4, 3));
                    }
                    str2 = str2 + "DD";
                    break;

                case "A110":
                    str2 = str2 + "05" + Add33H("10A1" + getSingle(Zdxl[2, 0], 4, 3));
                    break;

                case "A111":
                    str2 = str2 + "05" + Add33H("11A1" + getSingle(Zdxl[2, 1], 4, 3));
                    break;

                case "A112":
                    str2 = str2 + "05" + Add33H("12A1" + getSingle(Zdxl[2, 2], 4, 3));
                    break;

                case "A113":
                    str2 = str2 + "05" + Add33H("13A1" + getSingle(Zdxl[2, 3], 4, 3));
                    break;

                case "A114":
                    str2 = str2 + "05" + Add33H("14A1" + getSingle(Zdxl[2, 4], 4, 3));
                    break;

                case "A11F":
                    str2 = str2 + "12" + Add33H("1FA1" + getSingle(Zdxl[2, 0], 4, 3));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Zdxl[2, num2], 4, 3));
                    }
                    str2 = str2 + "DD";
                    break;

                case "A120":
                    str2 = str2 + "05" + Add33H("20A1" + getSingle(Zdxl[3, 0], 4, 3));
                    break;

                case "A121":
                    str2 = str2 + "05" + Add33H("21A1" + getSingle(Zdxl[3, 1], 4, 3));
                    break;

                case "A122":
                    str2 = str2 + "05" + Add33H("22A1" + getSingle(Zdxl[3, 2], 4, 3));
                    break;

                case "A123":
                    str2 = str2 + "05" + Add33H("23A1" + getSingle(Zdxl[3, 3], 4, 3));
                    break;

                case "A124":
                    str2 = str2 + "05" + Add33H("24A1" + getSingle(Zdxl[3, 4], 4, 3));
                    break;

                case "A12F":
                    str2 = str2 + "12" + Add33H("2FA1" + getSingle(Zdxl[3, 0], 4, 3));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Zdxl[3, num2], 4, 3));
                    }
                    str2 = str2 + "DD";
                    break;

                case "A1FF":
                    str2 = str2 + "22" + Add33H("FFA1" + getSingle(Zdxl[2, 0], 4, 3));
                    num2 = 1;
                    while (num2 <= 4)
                    {
                        str2 = str2 + Add33H(getSingle(Zdxl[2, num2], 4, 3));
                        num2++;
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(Zdxl[3, 0], 4, 3));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(Zdxl[3, num2], 4, 3));
                    }
                    str2 = str2 + "DD";
                    break;

                case "A410":
                    str2 = str2 + "05" + Add33H("10A4" + getSingle(1.5, 4, 3));
                    break;

                case "A411":
                    str2 = str2 + "05" + Add33H("11A4" + getSingle(1.5, 4, 3));
                    break;

                case "A412":
                    str2 = str2 + "05" + Add33H("12A4" + getSingle(1.5, 4, 3));
                    break;

                case "A413":
                    str2 = str2 + "05" + Add33H("13A4" + getSingle(1.5, 4, 3));
                    break;

                case "A414":
                    str2 = str2 + "05" + Add33H("14A4" + getSingle(1.5, 4, 3));
                    break;

                case "A41F":
                    str2 = str2 + "11" + Add33H("1FA4" + getSingle(1.5, 4, 3));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(1.5, 4, 3));
                    }
                    break;

                case "A420":
                    str2 = str2 + "05" + Add33H("20A4" + getSingle(1.6, 4, 3));
                    break;

                case "A421":
                    str2 = str2 + "05" + Add33H("21A4" + getSingle(1.6, 4, 3));
                    break;

                case "A422":
                    str2 = str2 + "05" + Add33H("22A4" + getSingle(1.6, 4, 3));
                    break;

                case "A423":
                    str2 = str2 + "05" + Add33H("23A4" + getSingle(1.6, 4, 3));
                    break;

                case "A424":
                    str2 = str2 + "05" + Add33H("24A4" + getSingle(1.6, 4, 3));
                    break;

                case "A42F":
                    str2 = str2 + "11" + Add33H("2FA4" + getSingle(1.6, 4, 3));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(1.6, 4, 3));
                    }
                    break;

                case "A4FF":
                    str2 = str2 + "22" + Add33H("FFA4" + getSingle(1.5, 4, 3));
                    num2 = 1;
                    while (num2 <= 4)
                    {
                        str2 = str2 + Add33H(getSingle(1.5, 4, 3));
                        num2++;
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(1.6, 4, 3));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(1.6, 4, 3));
                    }
                    str2 = str2 + "DD";
                    break;

                case "A510":
                    str2 = str2 + "05" + Add33H("10A5" + getSingle(1.0, 4, 3));
                    break;

                case "A511":
                    str2 = str2 + "05" + Add33H("11A5" + getSingle(1.0, 4, 3));
                    break;

                case "A512":
                    str2 = str2 + "05" + Add33H("12A5" + getSingle(1.0, 4, 3));
                    break;

                case "A513":
                    str2 = str2 + "05" + Add33H("13A5" + getSingle(1.0, 4, 3));
                    break;

                case "A514":
                    str2 = str2 + "05" + Add33H("14A5" + getSingle(1.0, 4, 3));
                    break;

                case "A51F":
                    str2 = str2 + "12" + Add33H("1FA5" + getSingle(1.0, 4, 3));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(1.0, 4, 3));
                    }
                    str2 = str2 + "DD";
                    break;

                case "A520":
                    str2 = str2 + "05" + Add33H("20A5" + getSingle(1.0, 4, 3));
                    break;

                case "A521":
                    str2 = str2 + "05" + Add33H("21A5" + getSingle(1.0, 4, 3));
                    break;

                case "A522":
                    str2 = str2 + "05" + Add33H("22A5" + getSingle(1.0, 4, 3));
                    break;

                case "A523":
                    str2 = str2 + "05" + Add33H("23A5" + getSingle(1.0, 4, 3));
                    break;

                case "A524":
                    str2 = str2 + "05" + Add33H("24A5" + getSingle(1.0, 4, 3));
                    break;

                case "A52F":
                    str2 = str2 + "12" + Add33H("2FA5" + getSingle(1.0, 4, 3));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(1.0, 4, 3));
                    }
                    str2 = str2 + "DD";
                    break;

                case "A5FF":
                    str2 = str2 + "22" + Add33H("FFA5" + getSingle(1.0, 4, 3));
                    num2 = 1;
                    while (num2 <= 4)
                    {
                        str2 = str2 + Add33H(getSingle(1.0, 4, 3));
                        num2++;
                    }
                    str2 = str2 + "DD" + Add33H(getSingle(1.0, 4, 3));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(getSingle(1.0, 4, 3));
                    }
                    str2 = str2 + "DD";
                    break;

                case "B010":
                    str2 = str2 + "06" + Add33H("10B0" + ZdxlTime[0, 0].ToString("mmHHddMM"));
                    break;

                case "B011":
                    str2 = str2 + "06" + Add33H("11B0" + ZdxlTime[0, 1].ToString("mmHHddMM"));
                    break;

                case "B012":
                    str2 = str2 + "06" + Add33H("12B0" + ZdxlTime[0, 2].ToString("mmHHddMM"));
                    break;

                case "B013":
                    str2 = str2 + "06" + Add33H("13B0" + ZdxlTime[0, 3].ToString("mmHHddMM"));
                    break;

                case "B014":
                    str2 = str2 + "06" + Add33H("14B0" + ZdxlTime[0, 4].ToString("mmHHddMM"));
                    break;

                case "B01F":
                    str2 = str2 + "16" + Add33H("1FB0" + ZdxlTime[0, 0].ToString("mmHHddMM"));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(ZdxlTime[0, num2].ToString("mmHHddMM"));
                    }
                    break;

                case "B020":
                    str2 = str2 + "06" + Add33H("20B0" + ZdxlTime[1, 0].ToString("mmHHddMM"));
                    break;

                case "B021":
                    str2 = str2 + "06" + Add33H("21B0" + ZdxlTime[1, 1].ToString("mmHHddMM"));
                    break;

                case "B022":
                    str2 = str2 + "06" + Add33H("22B0" + ZdxlTime[1, 2].ToString("mmHHddMM"));
                    break;

                case "B023":
                    str2 = str2 + "06" + Add33H("23B0" + ZdxlTime[1, 3].ToString("mmHHddMM"));
                    break;

                case "B024":
                    str2 = str2 + "06" + Add33H("24B0" + ZdxlTime[1, 4].ToString("mmHHddMM"));
                    break;

                case "B02F":
                    str2 = str2 + "16" + Add33H("2FB0" + ZdxlTime[1, 0].ToString("mmHHddMM"));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(ZdxlTime[1, num2].ToString("mmHHddMM"));
                    }
                    break;

                case "B0FF":
                    str2 = str2 + "2C" + Add33H("FFB0" + ZdxlTime[0, 0].ToString("mmHHddMM"));
                    num2 = 1;
                    while (num2 <= 4)
                    {
                        str2 = str2 + Add33H(ZdxlTime[1, num2].ToString("mmHHddMM"));
                        num2++;
                    }
                    str2 = str2 + "DD" + Add33H(ZdxlTime[1, 0].ToString("mmHHddMM"));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(ZdxlTime[1, num2].ToString("mmHHddMM"));
                    }
                    str2 = str2 + "DD";
                    break;

                case "B110":
                    str2 = str2 + "06" + Add33H("10B1" + ZdxlTime[2, 0].ToString("mmHHddMM"));
                    break;

                case "B111":
                    str2 = str2 + "06" + Add33H("11B1" + ZdxlTime[2, 1].ToString("mmHHddMM"));
                    break;

                case "B112":
                    str2 = str2 + "06" + Add33H("12B1" + ZdxlTime[2, 2].ToString("mmHHddMM"));
                    break;

                case "B113":
                    str2 = str2 + "06" + Add33H("13B1" + ZdxlTime[2, 3].ToString("mmHHddMM"));
                    break;

                case "B114":
                    str2 = str2 + "06" + Add33H("14B1" + ZdxlTime[2, 4].ToString("mmHHddMM"));
                    break;

                case "B11F":
                    str2 = str2 + "16" + Add33H("1FB1" + ZdxlTime[2, 0].ToString("mmHHddMM"));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(ZdxlTime[2, num2].ToString("mmHHddMM"));
                    }
                    break;

                case "B120":
                    str2 = str2 + "06" + Add33H("20B1" + ZdxlTime[3, 0].ToString("mmHHddMM"));
                    break;

                case "B121":
                    str2 = str2 + "06" + Add33H("21B1" + ZdxlTime[3, 1].ToString("mmHHddMM"));
                    break;

                case "B122":
                    str2 = str2 + "06" + Add33H("22B1" + ZdxlTime[3, 2].ToString("mmHHddMM"));
                    break;

                case "B123":
                    str2 = str2 + "06" + Add33H("23B1" + ZdxlTime[3, 3].ToString("mmHHddMM"));
                    break;

                case "B124":
                    str2 = str2 + "06" + Add33H("24B1" + ZdxlTime[3, 4].ToString("mmHHddMM"));
                    break;

                case "B12F":
                    str2 = str2 + "16" + Add33H("2FB1" + ZdxlTime[3, 0].ToString("mmHHddMM"));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(ZdxlTime[3, num2].ToString("mmHHddMM"));
                    }
                    break;

                case "B1FF":
                    str2 = str2 + "2C" + Add33H("FFB1" + ZdxlTime[2, 0].ToString("mmHHddMM"));
                    num2 = 1;
                    while (num2 <= 4)
                    {
                        str2 = str2 + Add33H(ZdxlTime[2, num2].ToString("mmHHddMM"));
                        num2++;
                    }
                    str2 = str2 + "DD" + Add33H(ZdxlTime[3, 0].ToString("mmHHddMM"));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(ZdxlTime[3, num2].ToString("mmHHddMM"));
                    }
                    str2 = str2 + "DD";
                    break;

                case "B410":
                    str2 = str2 + "06" + Add33H("10B4" + ZdxlTime[0, 0].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B411":
                    str2 = str2 + "06" + Add33H("11B4" + ZdxlTime[0, 1].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B412":
                    str2 = str2 + "06" + Add33H("12B4" + ZdxlTime[0, 2].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B413":
                    str2 = str2 + "06" + Add33H("13B4" + ZdxlTime[0, 3].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B414":
                    str2 = str2 + "06" + Add33H("14B4" + ZdxlTime[0, 4].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B41F":
                    str2 = str2 + "17" + Add33H("1FB4" + ZdxlTime[0, 0].AddMonths(-1).ToString("mmHHddMM"));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(ZdxlTime[0, num2].AddMonths(-1).ToString("mmHHddMM"));
                    }
                    str2 = str2 + "DD";
                    break;

                case "B420":
                    str2 = str2 + "06" + Add33H("20B4" + ZdxlTime[1, 0].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B421":
                    str2 = str2 + "06" + Add33H("21B4" + ZdxlTime[1, 1].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B422":
                    str2 = str2 + "06" + Add33H("22B4" + ZdxlTime[1, 2].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B423":
                    str2 = str2 + "06" + Add33H("23B4" + ZdxlTime[1, 3].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B424":
                    str2 = str2 + "06" + Add33H("24B4" + ZdxlTime[1, 4].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B42F":
                    str2 = str2 + "17" + Add33H("2FB4" + ZdxlTime[1, 0].AddMonths(-1).ToString("mmHHddMM"));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(ZdxlTime[1, num2].AddMonths(-1).ToString("mmHHddMM"));
                    }
                    str2 = str2 + "DD";
                    break;

                case "B4FF":
                    str2 = str2 + "2C" + Add33H("FFB4" + ZdxlTime[0, 0].AddMonths(-1).ToString("mmHHddMM"));
                    num2 = 1;
                    while (num2 <= 4)
                    {
                        str2 = str2 + Add33H(ZdxlTime[0, num2].AddMonths(-1).ToString("mmHHddMM"));
                        num2++;
                    }
                    str2 = str2 + "DD" + Add33H(ZdxlTime[1, 0].AddMonths(-1).ToString("mmHHddMM"));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(ZdxlTime[1, num2].AddMonths(-1).ToString("mmHHddMM"));
                    }
                    str2 = str2 + "DD";
                    break;

                case "B510":
                    str2 = str2 + "06" + Add33H("10B5" + ZdxlTime[2, 0].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B511":
                    str2 = str2 + "06" + Add33H("11B5" + ZdxlTime[2, 1].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B512":
                    str2 = str2 + "06" + Add33H("12B5" + ZdxlTime[2, 2].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B513":
                    str2 = str2 + "06" + Add33H("13B5" + ZdxlTime[2, 3].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B514":
                    str2 = str2 + "06" + Add33H("14B5" + ZdxlTime[2, 4].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B51F":
                    str2 = str2 + "17" + Add33H("1FB5" + ZdxlTime[2, 0].AddMonths(-1).ToString("mmHHddMM"));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(ZdxlTime[2, num2].AddMonths(-1).ToString("mmHHddMM"));
                    }
                    str2 = str2 + "DD";
                    break;

                case "B520":
                    str2 = str2 + "06" + Add33H("20B5" + ZdxlTime[3, 0].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B521":
                    str2 = str2 + "06" + Add33H("21B5" + ZdxlTime[3, 1].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B522":
                    str2 = str2 + "06" + Add33H("22B5" + ZdxlTime[3, 2].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B523":
                    str2 = str2 + "06" + Add33H("23B5" + ZdxlTime[3, 3].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B524":
                    str2 = str2 + "06" + Add33H("24B5" + ZdxlTime[3, 4].AddMonths(-1).ToString("mmHHddMM"));
                    break;

                case "B52F":
                    str2 = str2 + "17" + Add33H("2FB5" + ZdxlTime[3, 0].AddMonths(-1).ToString("mmHHddMM"));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(ZdxlTime[3, num2].AddMonths(-1).ToString("mmHHddMM"));
                    }
                    str2 = str2 + "DD";
                    break;

                case "B5FF":
                    str2 = str2 + "2C" + Add33H("FFB5" + ZdxlTime[2, 0].AddMonths(-1).ToString("mmHHddMM"));
                    num2 = 1;
                    while (num2 <= 4)
                    {
                        str2 = str2 + Add33H(ZdxlTime[2, num2].AddMonths(-1).ToString("mmHHddMM"));
                        num2++;
                    }
                    str2 = str2 + "DD" + Add33H(ZdxlTime[3, 0].AddMonths(-1).ToString("mmHHddMM"));
                    for (num2 = 1; num2 <= 4; num2++)
                    {
                        str2 = str2 + Add33H(ZdxlTime[3, num2].AddMonths(-1).ToString("mmHHddMM"));
                    }
                    str2 = str2 + "DD";
                    break;

                case "B611":
                    str2 = str2 + "04" + Add33H("11B6" + getSingle(Ua, 0, 2));
                    break;

                case "B612":
                    str2 = str2 + "04" + Add33H("12B6" + getSingle(Ub, 0, 2));
                    break;

                case "B613":
                    str2 = str2 + "04" + Add33H("13B6" + getSingle(Uc, 0, 2));
                    break;

                case "B61F":
                    str2 = str2 + "09" + Add33H("1FB6" + getSingle(Ua, 0, 2)) + Add33H(getSingle(Ub, 0, 2)) + Add33H(getSingle(Uc, 0, 2)) + "DD";
                    break;

                case "B621":
                    str2 = str2 + "04" + Add33H("21B6" + getSingle(Ia, 2, 2));
                    break;

                case "B622":
                    str2 = str2 + "04" + Add33H("22B6" + getSingle(Ib, 2, 2));
                    break;

                case "B623":
                    str2 = str2 + "04" + Add33H("23B6" + getSingle(Ic, 2, 2));
                    break;

                case "B62F":
                    str2 = str2 + "09" + Add33H("2FB6" + getSingle(Ia, 2, 2)) + Add33H(getSingle(Ib, 2, 2)) + Add33H(getSingle(Ic, 2, 2)) + "DD";
                    break;

                case "B630":
                    str2 = str2 + "05" + Add33H("30B6" + getSingle((Pa + Pb) + Pc, 4, 3));
                    break;

                case "B631":
                    str2 = str2 + "05" + Add33H("31B6" + getSingle(Pa, 4, 3));
                    break;

                case "B632":
                    str2 = str2 + "05" + Add33H("32B6" + getSingle(Pb, 4, 3));
                    break;

                case "B633":
                    str2 = str2 + "05" + Add33H("33B6" + getSingle(Pc, 4, 3));
                    break;

                case "B63F":
                    str2 = str2 + "13" + Add33H("3FB6" + getSingle((Pa + Pb) + Pc, 4, 3)) + Add33H(getSingle(Pa, 4, 3)) + Add33H(getSingle(Pb, 4, 3)) + Add33H(getSingle(Pc, 4, 3)) + Add33H(getSingle(Pa * 4.0, 2, 2)) + Add33H(getSingle(Pa * 4.0, 2, 2)) + "DD";
                    break;

                case "B640":
                    str2 = str2 + "04" + Add33H("40B6" + getSingle((Qa + Qb) + Qc, 2, 2));
                    break;

                case "B641":
                    str2 = str2 + "04" + Add33H("41B6" + getSingle(Qa, 2, 2));
                    break;

                case "B642":
                    str2 = str2 + "04" + Add33H("42B6" + getSingle(Qb, 2, 2));
                    break;

                case "B643":
                    str2 = str2 + "04" + Add33H("43B6" + getSingle(Qc, 2, 2));
                    break;

                case "B64F":
                    str2 = str2 + "0B" + Add33H("4FB6" + getSingle((Qa + Qb) + Qc, 2, 2)) + Add33H(getSingle(Qa, 2, 2)) + Add33H(getSingle(Qb, 2, 2)) + Add33H(getSingle(Qc, 2, 2)) + "DD";
                    break;

                case "B650":
                    str2 = str2 + "04" + Add33H("50B6" + getSingle(num, 3, 2));
                    break;

                case "B651":
                    str2 = str2 + "04" + Add33H("51B6" + getSingle((Ia == 0.0) ? 1.0 : Math.Cos((Phia / 180.0) * 3.1415936), 3, 2));
                    break;

                case "B652":
                    str2 = str2 + "04" + Add33H("52B6" + getSingle((Ib == 0.0) ? 1.0 : Math.Cos((Phib / 180.0) * 3.1415936), 3, 2));
                    break;

                case "B653":
                    str2 = str2 + "04" + Add33H("53B6" + getSingle((Ic == 0.0) ? 1.0 : Math.Cos((Phic / 180.0) * 3.1415936), 3, 2));
                    break;

                case "B65F":
                    str2 = str2 + "0B" + Add33H("5FB6" + getSingle(num, 3, 2)) + Add33H(getSingle((Ia == 0.0) ? 1.0 : Math.Cos((Phia / 180.0) * 3.1415936), 3, 2)) + Add33H(getSingle((Ib == 0.0) ? 1.0 : Math.Cos((Phib / 180.0) * 3.1415936), 3, 2)) + Add33H(getSingle((Ic == 0.0) ? 1.0 : Math.Cos((Phic / 180.0) * 3.1415936), 3, 2)) + "DD";
                    break;

                case "B690":
                    str2 = str2 + "05" + Add33H("90B6" + getSingle((0.0 + Phia) % 360.0, 2, 3));
                    break;

                case "B691":
                    str2 = str2 + "05" + Add33H("91B6" + getSingle((240.0 + Phib) % 360.0, 2, 3));
                    break;

                case "B692":
                    str2 = str2 + "05" + Add33H("92B6" + getSingle((120.0 + Phic) % 360.0, 2, 3));
                    break;

                case "B69F":
                    str2 = str2 + "0C" + Add33H("9FB6" + getSingle((0.0 + Phia) % 360.0, 2, 3)) + Add33H(getSingle((240.0 + Phib) % 360.0, 2, 3)) + Add33H(getSingle((120.0 + Phic) % 360.0, 2, 3)) + "DD";
                    break;

                case "B6A0":
                    str2 = str2 + "05" + Add33H("A0B6" + getSingle(0.0, 2, 3));
                    break;

                case "B6A1":
                    str2 = str2 + "05" + Add33H("A1B6" + getSingle(240.0, 2, 3));
                    break;

                case "B6A2":
                    str2 = str2 + "05" + Add33H("A2B6" + getSingle(120.0, 2, 3));
                    break;

                case "B6AF":
                    str2 = str2 + "0C" + Add33H("AFB6" + getSingle(0.0, 2, 3)) + Add33H(getSingle(240.0, 2, 3)) + Add33H(getSingle(120.0, 2, 3)) + "DD";
                    break;

                case "B6B0":
                    str2 = str2 + "04" + Add33H("B0B6" + getSingle(0.0, 2, 2));
                    break;

                case "B6B1":
                    str2 = str2 + "05" + Add33H("B1B6" + getSingle(0.0, 2, 3));
                    break;

                case "B6FF":
                    str2 = str2 + "33" + Add33H("FFB6" + getSingle(Ua, 0, 2)) + Add33H(getSingle(Ub, 0, 2)) + Add33H(getSingle(Uc, 0, 2)) + "DD";
                    str2 = str2 + Add33H(getSingle(Ia, 2, 2)) + Add33H(getSingle(Ib, 2, 2)) + Add33H(getSingle(Ic, 2, 2)) + "DD";
                    str2 = str2 + Add33H(getSingle((Pa + Pb) + Pc, 4, 3)) + Add33H(getSingle(Pa, 4, 3)) + Add33H(getSingle(Pb, 4, 3)) + Add33H(getSingle(Pc, 4, 3)) + Add33H(getSingle(Pa * 4.0, 2, 2)) + Add33H(getSingle(Pa * 4.0, 2, 2)) + "DD";
                    str2 = str2 + Add33H(getSingle((Qa + Qb) + Qc, 2, 2)) + Add33H(getSingle(Qa, 2, 2)) + Add33H(getSingle(Qb, 2, 2)) + Add33H(getSingle(Qc, 2, 2)) + "DD";
                    str2 = str2 + Add33H(getSingle(num, 3, 2)) + Add33H(getSingle((Ia == 0.0) ? 1.0 : Math.Cos((Phia / 180.0) * 3.1415936), 3, 2)) + Add33H(getSingle((Ib == 0.0) ? 1.0 : Math.Cos((Phib / 180.0) * 3.1415936), 3, 2)) + Add33H(getSingle((Ic == 0.0) ? 1.0 : Math.Cos((Phic / 180.0) * 3.1415936), 3, 2)) + "DD";
                    break;

                case "B210":
                    str2 = str2 + "06" + Add33H("10B2" + string.Format("{0:D2}", LastBCtime.Minute) + LastBCtime.ToString("HHddMM"));
                    break;

                case "B211":
                    str2 = str2 + "06" + Add33H("11B2" + string.Format("{0:D2}", LastQLtime.Minute) + LastQLtime.ToString("HHddMM"));
                    break;

                case "B212":
                    str2 = str2 + "04" + Add33H("12B2" + string.Format("{0:D2}", BCCS % 100) + "00");
                    break;

                case "B213":
                    str2 = str2 + "04" + Add33H("13B2" + string.Format("{0:D2}", QLCS % 100) + "00");
                    break;

                case "B214":
                    str2 = str2 + "05" + Add33H("14B2" + string.Format("{0:D2}", DCWorkTime % 100) + "0000");
                    break;

                case "B21F":
                    str2 = (((str2 + "12" + Add33H("1FB2" + string.Format("{0:D2}", LastBCtime.Minute) + LastBCtime.ToString("HHddMM"))) + Add33H(string.Format("{0:D2}", LastQLtime.Minute) + LastQLtime.ToString("HHddMM"))) + Add33H(string.Format("{0:D2}", BCCS % 100) + "00") + Add33H(string.Format("{0:D2}", QLCS % 100) + "00")) + Add33H(string.Format("{0:D2}", DCWorkTime % 100) + "0000") + "DD";
                    break;

                case "B2FF":
                    str2 = (((str2 + "12" + Add33H("FFB2" + string.Format("{0:D2}", LastBCtime.Minute) + LastBCtime.ToString("HHddMM"))) + Add33H(string.Format("{0:D2}", LastQLtime.Minute) + LastQLtime.ToString("HHddMM"))) + Add33H(string.Format("{0:D2}", BCCS % 100) + "00") + Add33H(string.Format("{0:D2}", QLCS % 100) + "00")) + Add33H(string.Format("{0:D2}", DCWorkTime % 100) + "0000") + "DD";
                    break;

                case "B310":
                    str2 = str2 + "04" + Add33H("10B3" + string.Format("{0:D2}", DXCS % 100) + "00");
                    break;

                case "B311":
                    str2 = str2 + "04" + Add33H("11B3" + string.Format("{0:D2}", Math.Abs((int)(DXCS - 2)) % 100) + "00");
                    break;

                case "B312":
                    str2 = str2 + "04" + Add33H("12B3" + string.Format("{0:D2}", 1) + "00");
                    break;

                case "B313":
                    str2 = str2 + "04" + Add33H("13B3" + string.Format("{0:D2}", 1) + "00");
                    break;

                case "B31F":
                    str2 = ((str2 + "0B" + Add33H("1FB3" + string.Format("{0:D2}", DXCS % 100) + "00")) + Add33H(string.Format("{0:D2}", Math.Abs((int)(DXCS - 2)) % 100) + "00") + Add33H(string.Format("{0:D2}", 1) + "00")) + Add33H(string.Format("{0:D2}", 1) + "00") + "DD";
                    break;

                case "B320":
                    str2 = str2 + "05" + Add33H("20B3030000");
                    break;

                case "B321":
                    str2 = str2 + "05" + Add33H("21B3010000");
                    break;

                case "B322":
                    str2 = str2 + "05" + Add33H("22B3010000");
                    break;

                case "B323":
                    str2 = str2 + "05" + Add33H("23B3010000");
                    break;

                case "B32F":
                    str2 = ((str2 + "0F" + Add33H("2FB3030000")) + Add33H("010000") + Add33H("010000")) + Add33H("010000") + "DD";
                    break;

                case "B330":
                    str2 = str2 + "06" + Add33H("30B310120305");
                    break;

                case "B331":
                    str2 = str2 + "06" + Add33H("31B310120205");
                    break;

                case "B332":
                    str2 = str2 + "06" + Add33H("32B310120305");
                    break;

                case "B333":
                    str2 = str2 + "06" + Add33H("33B310120105");
                    break;

                case "B33F":
                    str2 = ((str2 + "13" + Add33H("3FB310120305")) + Add33H("10120205") + Add33H("10120305")) + Add33H("10120105") + "DD";
                    break;

                case "B340":
                    str2 = str2 + "06" + Add33H("40B315120305");
                    break;

                case "B341":
                    str2 = str2 + "06" + Add33H("41B315120205");
                    break;

                case "B342":
                    str2 = str2 + "06" + Add33H("42B315120305");
                    break;

                case "B343":
                    str2 = str2 + "06" + Add33H("43B315120105");
                    break;

                case "B34F":
                    str2 = ((str2 + "13" + Add33H("4FB315120305")) + Add33H("15120205") + Add33H("15120305")) + Add33H("15120105") + "DD";
                    break;

                case "B3FF":
                    str2 = (((((((((((str2 + "3A" + Add33H("FFB3" + string.Format("{0:D2}", DXCS % 100) + "00")) + Add33H(string.Format("{0:D2}", Math.Abs((int)(DXCS - 2)) % 100) + "00") + Add33H(string.Format("{0:D2}", 1) + "00")) + Add33H(string.Format("{0:D2}", 1) + "00") + "DD") + Add33H("030000")) + Add33H("010000") + Add33H("010000")) + Add33H("010000") + "DD") + Add33H("10120305")) + Add33H("10120205") + Add33H("10120305")) + Add33H("10120105") + "DD") + Add33H("15120305")) + Add33H("15120205") + Add33H("15120305")) + Add33H("15120105") + "DD";
                    break;

                case "C010":
                    str3 = string.Format("{0:D2}", (DateBron().DayOfWeek == ~DayOfWeek.Sunday) ? (DayOfWeek.Saturday | DayOfWeek.Monday) : DateBron().DayOfWeek);
                    str2 = str2 + "06" + Add33H("10C0" + str3 + DateBron().ToString("ddMMyy"));
                    break;

                case "C011":
                    str2 = str2 + "05" + Add33H("11C0" + string.Format("{0:D2}", DateBron().Second)) + Add33H(string.Format("{0:D2}", DateBron().Minute)) + Add33H(string.Format("{0:D2}", DateBron().Hour));
                    break;

                case "C01F":
                    str3 = string.Format("{0:D2}", (DateBron().DayOfWeek == ~DayOfWeek.Sunday) ? (DayOfWeek.Saturday | DayOfWeek.Monday) : DateBron().DayOfWeek);
                    str2 = str2 + "0A" + Add33H("1FC0" + str3 + DateBron().ToString("ddMMyy")) + Add33H(string.Format("{0:D2}", DateBron().Second)) + Add33H(string.Format("{0:D2}", DateBron().Minute)) + Add33H(string.Format("{0:D2}", DateBron().Hour)) + "DD";
                    break;

                case "C020":
                    str2 = str2 + "03" + Add33H("20C0" + string.Format("{0:X2}", BiaoZt));
                    break;

                case "C021":
                    str2 = str2 + "03" + Add33H("21C0" + string.Format("{0:X2}", Wangzt));
                    break;

                case "C022":
                    str2 = str2 + "03" + Add33H("22C07E");
                    break;

                case "C02F":
                    str2 = str2 + "06" + Add33H("2FC0" + string.Format("{0:X2}", BiaoZt)) + Add33H(string.Format("{0:X2}", Wangzt)) + Add33H("7E") + "DD";
                    break;

                case "C030":
                    str4 = string.Format("{0:D6}", BiaoYgCS);
                    str2 = str2 + "05" + Add33H("30C0" + str4.Substring(4, 2) + str4.Substring(2, 2) + str4.Substring(0, 2));
                    break;

                case "C031":
                    str4 = string.Format("{0:D6}", BiaoWgCS);
                    str2 = str2 + "05" + Add33H("31C0" + str4.Substring(4, 2) + str4.Substring(2, 2) + str4.Substring(0, 2));
                    break;

                case "C03F":
                    str4 = string.Format("{0:D6}", BiaoYgCS);
                    str2 = str2 + "1B" + Add33H("3FC0" + str4.Substring(4, 2) + str4.Substring(2, 2) + str4.Substring(0, 2));
                    str4 = string.Format("{0:D6}", BiaoWgCS);
                    str2 = ((str2 + Add33H(str4.Substring(4, 2) + str4.Substring(2, 2) + str4.Substring(0, 2))) + Add33H("000000123456") + Add33H("000000123456")) + Add33H("000000123456") + "DD";
                    break;

                case "C0FF":
                    str3 = string.Format("{0:D2}", (DateBron().DayOfWeek == ~DayOfWeek.Sunday) ? (DayOfWeek.Saturday | DayOfWeek.Monday) : DateBron().DayOfWeek);
                    str4 = string.Format("{0:D6}", BiaoYgCS);
                    str2 = str2 + "27" + Add33H("FFC0" + str3 + DateBron().ToString("DDMMyy")) + Add33H(string.Format("{0:D2}", DateBron().Second)) + Add33H(string.Format("{0:D2}", DateBron().Minute)) + Add33H(string.Format("{0:D2}", DateBron().Hour)) + "DD";
                    str2 = (str2 + Add33H(string.Format("{0:X2}", BiaoZt)) + Add33H(string.Format("{0:X2}", Wangzt)) + Add33H("7E") + "DD") + Add33H(str4.Substring(4, 2) + str4.Substring(2, 2) + str4.Substring(0, 2));
                    str4 = string.Format("{0:D6}", BiaoWgCS);
                    str2 = ((str2 + Add33H(str4.Substring(4, 2) + str4.Substring(2, 2) + str4.Substring(0, 2))) + Add33H("000000123456") + Add33H("000000123456")) + Add33H("000000123456") + "DD";
                    break;

                case "C117":
                    str2 = str2 + "04" + Add33H("17C1" + string.Format("{0:4}", AutoDate));
                    break;

                case "C11F":
                    str2 = (((((str2 + "14" + Add33H("1FC10F")) + Add33H("01")) + Add33H("10") + Add33H("10")) + Add33H("02") + Add33H("04")) + Add33H(string.Format("{0:4}", AutoDate)) + Add33H(getSingle(0.0, 1, 4))) + Add33H(getSingle(0.0, 1, 4)) + "DD";
                    break;

                case "C1FF":
                    str2 = (((((str2 + "14" + Add33H("FFC10F")) + Add33H("01")) + Add33H("10") + Add33H("10")) + Add33H("02") + Add33H("04")) + Add33H(string.Format("{0:4}", AutoDate)) + Add33H(getSingle(0.0, 1, 4))) + Add33H(getSingle(0.0, 1, 4)) + "DD";
                    break;

                case "C310":
                    str2 = str2 + "03" + Add33H("10C301");
                    break;

                case "C311":
                    str2 = str2 + "03" + Add33H("11C301");
                    break;

                case "C312":
                    str2 = str2 + "03" + Add33H("12C308");
                    break;

                case "C313":
                    str2 = str2 + "03" + Add33H("13C304");
                    break;

                case "C31F":
                    str2 = str2 + "08" + Add33H("1FC30101080410") + "DD";
                    break;

                case "C321":
                    str2 = str2 + "05" + Add33H("21C3010101");
                    break;

                case "C32F":
                    str2 = str2 + "06" + Add33H("2FC3010101") + "DD";
                    break;

                case "C331":
                    str2 = str2 + "05" + Add33H("31C3" + string.Format("{0:D2}", FeiLei[0]) + string.Format("{0:D2}", ShiDuan[0].Minute) + string.Format("{0:D2}", ShiDuan[0].Hour));
                    break;

                case "C332":
                    str2 = str2 + "05" + Add33H("32C3" + string.Format("{0:D2}", FeiLei[1]) + string.Format("{0:D2}", ShiDuan[1].Minute) + string.Format("{0:D2}", ShiDuan[1].Hour));
                    break;

                case "C333":
                    str2 = str2 + "05" + Add33H("33C3" + string.Format("{0:D2}", FeiLei[2]) + string.Format("{0:D2}", ShiDuan[2].Minute) + string.Format("{0:D2}", ShiDuan[2].Hour));
                    break;

                case "C334":
                    str2 = str2 + "05" + Add33H("34C3" + string.Format("{0:D2}", FeiLei[3]) + string.Format("{0:D2}", ShiDuan[3].Minute) + string.Format("{0:D2}", ShiDuan[3].Hour));
                    break;

                case "C335":
                    str2 = str2 + "05" + Add33H("35C3" + string.Format("{0:D2}", FeiLei[4]) + string.Format("{0:D2}", ShiDuan[4].Minute) + string.Format("{0:D2}", ShiDuan[4].Hour));
                    break;

                case "C336":
                    str2 = str2 + "05" + Add33H("36C3" + string.Format("{0:D2}", FeiLei[5]) + string.Format("{0:D2}", ShiDuan[1].Minute) + string.Format("{0:D2}", ShiDuan[5].Hour));
                    break;

                case "C337":
                    str2 = str2 + "05" + Add33H("37C3" + string.Format("{0:D2}", FeiLei[6]) + string.Format("{0:D2}", ShiDuan[1].Minute) + string.Format("{0:D2}", ShiDuan[6].Hour));
                    break;

                case "C338":
                    str2 = str2 + "05" + Add33H("38C3" + string.Format("{0:D2}", FeiLei[7]) + string.Format("{0:D2}", ShiDuan[1].Minute) + string.Format("{0:D2}", ShiDuan[7].Hour));
                    break;

                case "C33F":
                    str2 = ((((str2 + "1B" + Add33H("3FC3" + string.Format("{0:D2}", FeiLei[0]) + string.Format("{0:D2}", ShiDuan[0].Minute) + string.Format("{0:D2}", ShiDuan[0].Hour))) + Add33H(string.Format("{0:D2}", FeiLei[1]) + string.Format("{0:D2}", ShiDuan[1].Minute) + string.Format("{0:D2}", ShiDuan[1].Hour)) + Add33H(string.Format("{0:D2}", FeiLei[2]) + string.Format("{0:D2}", ShiDuan[2].Minute) + string.Format("{0:D2}", ShiDuan[2].Hour))) + Add33H(string.Format("{0:D2}", FeiLei[3]) + string.Format("{0:D2}", ShiDuan[3].Minute) + string.Format("{0:D2}", ShiDuan[3].Hour)) + Add33H(string.Format("{0:D2}", FeiLei[4]) + string.Format("{0:D2}", ShiDuan[4].Minute) + string.Format("{0:D2}", ShiDuan[4].Hour))) + Add33H(string.Format("{0:D2}", FeiLei[5]) + string.Format("{0:D2}", ShiDuan[5].Minute) + string.Format("{0:D2}", ShiDuan[5].Hour)) + Add33H(string.Format("{0:D2}", FeiLei[6]) + string.Format("{0:D2}", ShiDuan[6].Minute) + string.Format("{0:D2}", ShiDuan[6].Hour))) + Add33H(string.Format("{0:D2}", FeiLei[7]) + string.Format("{0:D2}", ShiDuan[7].Minute) + string.Format("{0:D2}", ShiDuan[7].Hour)) + "DD";
                    break;

                case "C3FF":
                    str2 = ((((((str2 + "25" + Add33H("FFC30101080410") + "DD") + Add33H("010101") + "DD") + Add33H(string.Format("{0:D2}", FeiLei[0]) + string.Format("{0:D2}", ShiDuan[0].Minute) + string.Format("{0:D2}", ShiDuan[0].Hour))) + Add33H(string.Format("{0:D2}", FeiLei[1]) + string.Format("{0:D2}", ShiDuan[1].Minute) + string.Format("{0:D2}", ShiDuan[1].Hour)) + Add33H(string.Format("{0:D2}", FeiLei[2]) + string.Format("{0:D2}", ShiDuan[2].Minute) + string.Format("{0:D2}", ShiDuan[2].Hour))) + Add33H(string.Format("{0:D2}", FeiLei[3]) + string.Format("{0:D2}", ShiDuan[3].Minute) + string.Format("{0:D2}", ShiDuan[3].Hour)) + Add33H(string.Format("{0:D2}", FeiLei[4]) + string.Format("{0:D2}", ShiDuan[4].Minute) + string.Format("{0:D2}", ShiDuan[4].Hour))) + Add33H(string.Format("{0:D2}", FeiLei[5]) + string.Format("{0:D2}", ShiDuan[5].Minute) + string.Format("{0:D2}", ShiDuan[5].Hour)) + Add33H(string.Format("{0:D2}", FeiLei[6]) + string.Format("{0:D2}", ShiDuan[6].Minute) + string.Format("{0:D2}", ShiDuan[6].Hour))) + Add33H(string.Format("{0:D2}", FeiLei[7]) + string.Format("{0:D2}", ShiDuan[7].Minute) + string.Format("{0:D2}", ShiDuan[7].Hour)) + "DD";
                    break;

                case "D113":
                    str2 = str2 + "06" + Add33H("13D1E000E1E500");
                    break;

                case "D110":
                    str2 = str2 + "06" + Add33H("10D1E000E1E500");
                    break;

                case "D111":
                    str2 = str2 + "06" + Add33H("11D1E000E1E500");
                    break;

                case "D112":
                    str2 = str2 + "06" + Add33H("12D1E000E1E500");
                    break;

                case "D114":
                    str2 = str2 + "06" + Add33H("14D1E000E1E500");
                    break;

                case "D115":
                    str2 = str2 + "06" + Add33H("15D1E000E1E500");
                    break;

                case "C833":
                    str2 = str2 + "04" + Add33H("33C8" + getSingle(2.0, 2, 2));
                    break;

                case "C990":
                    str2 = str2 + "06" + Add33H("90C9" + getSingle(150.0, 2, 4));
                    break;

                case "C991":
                    str2 = str2 + "06" + Add33H("91C9" + getSingle(20.0, 2, 4));
                    break;

                case "C9A0":
                    str2 = str2 + "07" + Add33H("A0C91010010110");
                    break;

                case "C9A1":
                    str2 = str2 + "04" + Add33H("CA1C9" + getSingle(2.0, 0, 2));
                    break;

                case "C9A2":
                    str2 = str2 + "06" + Add33H("A2C9" + getSingle(100.0, 2, 4));
                    break;

                case "C9A3":
                    str2 = str2 + "06" + Add33H("A3C9" + getSingle(50.0, 2, 4));
                    break;

                case "C9A4":
                    str2 = str2 + "06" + Add33H("A4C9" + getSingle(150.0, 2, 4));
                    break;

                case "C9A5":
                    str2 = str2 + "06" + Add33H("A5C9" + getSingle(200.0, 2, 4));
                    break;

                case "C9C0":
                    str2 = str2 + "07" + Add33H("C0C91010010110");
                    break;

                case "C9C1":
                    str2 = str2 + "04" + Add33H("C1C9" + getSingle(2.0, 0, 2));
                    break;

                case "C9C2":
                    str2 = str2 + "06" + Add33H("C2C9" + getSingle(300.0, 2, 4));
                    break;

                case "C9C3":
                    str2 = str2 + "06" + Add33H("C3C9" + getSingle(50.0, 2, 4));
                    break;

                case "C9C4":
                    str2 = str2 + "06" + Add33H("C4C9" + getSingle(350.0, 2, 4));
                    break;

                case "C9C5":
                    str2 = str2 + "06" + Add33H("C5C9" + getSingle(400.0, 2, 4));
                    break;

                case "C9D0":
                    str2 = str2 + "06" + Add33H("D0C9" + getSingle(30.0, 2, 4));
                    break;

                case "C9D1":
                    str2 = str2 + "06" + Add33H("D1C9" + getSingle(60.0, 2, 4));
                    break;

                default:
                    str2 = str2.Substring(0, 0x10) + "C101" + string.Format("{0:X2}", (int)Math.Pow(2.0, 6.0));
                    break;
            }
            if ((str2.Length == 0x12) && (str.Substring(0, 1) == "9"))
            {
                str2 = str2.Substring(0, 0x10) + "C101" + string.Format("{0:X2}", (int)Math.Pow(2.0, 6.0));
            }
            else if (str2.Length == 0x12)
            {
                str2 = str2.Substring(0, 0x10) + "C10101";
            }
            //str2 = this.DLStringDeal(str2);
            return (str2 + getChk(str2) + "16");
        }

    }
}
