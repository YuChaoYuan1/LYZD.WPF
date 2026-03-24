using LY.VirtualMeter.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LY.VirtualMeter.ViewModel
{
    /// <summary>
    /// 电量模型
    /// </summary>
    public class MeterDLViewModel : NotifyPropertyBase
    {

        private string name;

        public string Name
        {
            get { return name; }
            set { SetPropertyValue(value, ref name, "Name"); }
        }
        private double _总电量;

        public double 总电量
        {
            get { return _总电量; }
            set { SetPropertyValue(value, ref _总电量, "总电量"); }
        }
        private double _电量1;

        public double 电量1
        {
            get { return _电量1; }
            set { SetPropertyValue(value, ref _电量1, "电量1"); }
        }
        private double _电量2;

        public double 电量2
        {
            get { return _电量2; }
            set { SetPropertyValue(value, ref _电量2, "电量2"); }
        }
        private double _电量3;

        public double 电量3
        {
            get { return _电量3; }
            set { SetPropertyValue(value, ref _电量3, "电量3"); }
        }
        private double _电量4;

        public double 电量4
        {
            get { return _电量4; }
            set { SetPropertyValue(value, ref _电量4, "电量4"); }
        }
        private double _总需量;

        public double 总需量
        {
            get { return _总需量; }
            set { SetPropertyValue(value, ref _总需量, "总需量"); }
        }
        private double _需量1;

        public double 需量1
        {
            get { return _需量1; }
            set { SetPropertyValue(value, ref _需量1, "需量1"); }
        }
        private double _需量2;

        public double 需量2
        {
            get { return _需量2; }
            set { SetPropertyValue(value, ref _需量2, "需量2"); }
        }
        private double _需量3;

        public double 需量3
        {
            get { return _需量3; }
            set { SetPropertyValue(value, ref _需量3, "需量3"); }
        }
        private double _需量4;

        public double 需量4
        {
            get { return _需量4; }
            set { SetPropertyValue(value, ref _需量4, "需量4"); }
        }
        private string _总时间;

        public string 总时间
        {
            get { return _总时间; }
            set { SetPropertyValue(value, ref _总时间, "总时间"); }
        }
        private string _时间1;

        public string 时间1
        {
            get { return _时间1; }
            set { SetPropertyValue(value, ref _时间1, "时间1"); }
        }
        private string _时间2;

        public string 时间2
        {
            get { return _时间2; }
            set { SetPropertyValue(value, ref _时间2, "时间2"); }
        }
        private string _时间3;

        public string 时间3
        {
            get { return _时间3; }
            set { SetPropertyValue(value, ref _时间3, "时间3"); }
        }
        private string _时间4;

        public string 时间4
        {
            get { return _时间4; }
            set { SetPropertyValue(value, ref _时间4, "时间4"); }
        }
    }
}
