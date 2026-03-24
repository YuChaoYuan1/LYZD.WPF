using LY.VirtualMeter.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LY.VirtualMeter.ViewModel
{
    /// <summary>
    /// 设置界面模型
    /// </summary>
    public class SetViewModel : NotifyPropertyBase
    {

        private string name;

        public string Name
        {
            get { return name; }
            set { SetPropertyValue(value, ref name, "Name"); }
        }


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

    



        #endregion

        #region 电量数据
        private string _正向有功="1000";

        public string 正向有功
        {
            get { return _正向有功; }
            set { SetPropertyValue(value, ref _正向有功, "正向有功"); }
        }
        private string _反向有功 = "1000";

        public string 反向有功
        {
            get { return _反向有功; }
            set { SetPropertyValue(value, ref _反向有功, "反向有功"); }
        }
        private string _一象限无功 = "1000";
                                                                 
        public string 一象限无功
        {
            get { return _一象限无功; }                                     
            set { SetPropertyValue(value, ref _一象限无功, "一象限无功"); }
        }
        private string _二象限无功 = "1000";

        public string 二象限无功
        {
            get { return _二象限无功; }
            set { SetPropertyValue(value, ref _二象限无功, "二象限无功"); }
        }
        private string _三象限无功 = "1000";

        public string 三象限无功
        {
            get { return _三象限无功; }
            set { SetPropertyValue(value, ref _三象限无功, "三象限无功"); }
        }
        private string _四象限无功 = "1000";

        public string 四象限无功
        {
            get { return _四象限无功; }
            set { SetPropertyValue(value, ref _四象限无功, "四象限无功"); }
        }
        #endregion

        #region 状态字
        private string _状态字1 = "0000";

        public string 状态字1
        {
            get { return _状态字1; }
            set { SetPropertyValue(value, ref _状态字1, "状态字1"); }
        }
        private string _状态字2 = "0000";

        public string 状态字2
        {
            get { return _状态字2; }
            set { SetPropertyValue(value, ref _状态字2, "状态字2"); }
        }
        private string _状态字3 = "0000";

        public string 状态字3
        {
            get { return _状态字3; }
            set { SetPropertyValue(value, ref _状态字3, "状态字3"); }
        }
        private string _状态字4 = "0000";

        public string 状态字4
        {
            get { return _状态字4; }
            set { SetPropertyValue(value, ref _状态字4, "状态字4"); }
        }
        private string _状态字5 = "0000";

        public string 状态字5
        {
            get { return _状态字5; }
            set { SetPropertyValue(value, ref _状态字5, "状态字5"); }
        }
        private string _状态字6 = "0000";

        public string 状态字6
        {
            get { return _状态字6; }
            set { SetPropertyValue(value, ref _状态字6, "状态字6"); }
        }
        private string _状态字7 = "0000";

        public string 状态字7
        {
            get { return _状态字7; }
            set { SetPropertyValue(value, ref _状态字7, "状态字7"); }
        }

        #endregion

        #region 其他
        private string _日期慢系统天 = "0";

        public string 日期慢系统天
        {
            get { return _日期慢系统天; }
            set { SetPropertyValue(value, ref _日期慢系统天, "日期慢系统天"); }
        }
        private string _时间慢系统分 = "0";

        public string 时间慢系统分
        {
            get { return _时间慢系统分; }
            set { SetPropertyValue(value, ref _时间慢系统分, "时间慢系统分"); }
        }
        private string _走字系数 = "0";

        public string 走字系数
        {
            get { return _走字系数; }
            set { SetPropertyValue(value, ref _走字系数, "走字系数"); }
        }
        private string _电能系数 = "0";

        public string 电能系数
        {
            get { return _电能系数; }
            set { SetPropertyValue(value, ref _电能系数, "电能系数"); }
        }
        private string _载波地址 = "000000000001";

        public string 载波地址
        {
            get { return _载波地址; }
            set { SetPropertyValue(value, ref _载波地址, "载波地址"); }
        }
        private string _表地址 = "000000000001";

        public string 表地址
        {
            get { return _表地址; }
            set { SetPropertyValue(value, ref _表地址, "表地址"); }
        }

        #endregion

        #region 状态改变
        private bool  _最近一次编程时间;

        public bool 最近一次编程时间
        {
            get { return _最近一次编程时间; }
            set { SetPropertyValue(value, ref _最近一次编程时间, "最近一次编程时间"); }
        }
        private bool _最大需量清零时间;

        public bool 最大需量清零时间
        {
            get { return _最大需量清零时间; }
            set { SetPropertyValue(value, ref _最大需量清零时间, "最大需量清零时间"); }
        }
        private bool _电池工作时间;

        public bool 电池工作时间
        {
            get { return _电池工作时间; }
            set { SetPropertyValue(value, ref _电池工作时间, "电池工作时间"); }
        }
        private bool _断相次数;

        public bool 断相次数
        {
            get { return _断相次数; }
            set { SetPropertyValue(value, ref _断相次数, "断相次数"); }
        }
        private bool _常数;

        public bool 常数
        {
            get { return _常数; }
            set { SetPropertyValue(value, ref _常数, "常数"); }
        }
        private bool _时段;

        public bool 时段
        {
            get { return _时段; }
            set { SetPropertyValue(value, ref _时段, "时段"); }
        }
        private bool _清零次数;

        public bool 清零次数
        {
            get { return _清零次数; }
            set { SetPropertyValue(value, ref _清零次数, "清零次数"); }
        }
        private bool _编程次数;

        public bool 编程次数
        {
            get { return _编程次数; }
            set { SetPropertyValue(value, ref _编程次数, "编程次数"); }
        }
        private bool _电表运行状态字;

        public bool 电表运行状态字
        {
            get { return _电表运行状态字; }
            set { SetPropertyValue(value, ref _电表运行状态字, "电表运行状态字"); }
        }
        private bool _电池异常;

        public bool 电池异常
        {
            get { return _电池异常; }
            set { SetPropertyValue(value, ref _电池异常, "电池异常"); }
        }
        private bool _自动抄表日;

        public bool 自动抄表日
        {
            get { return _自动抄表日; }
            set { SetPropertyValue(value, ref _自动抄表日, "自动抄表日"); }
        }

        #endregion
    }
}
