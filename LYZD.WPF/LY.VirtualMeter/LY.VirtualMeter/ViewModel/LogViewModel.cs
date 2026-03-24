using LY.VirtualMeter.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LY.VirtualMeter.ViewModel
{
    /// <summary>
    /// 日志
    /// </summary>
    public class LogViewModel : NotifyPropertyBase
    {
        private string log_PC ;
        /// <summary>
        /// 与主程序通讯日志
        /// </summary>
        public string Log_PC
        {
            get { return log_PC; }
            set { SetPropertyValue(value, ref log_PC, "Log_PC"); }
        }

        //private ObservableCollection<MeterLogViewModel> log_Meter = new ObservableCollection<MeterLogViewModel>();
        ///// <summary>
        ///// 表位端口数据
        ///// </summary>
        //public ObservableCollection<MeterLogViewModel> Log_Meter
        //{
        //    get { return log_Meter; }
        //    set { SetPropertyValue(value, ref log_Meter, "Log_Meter"); }
        //}

    }

    public class MeterLogViewModel :NotifyPropertyBase
    {
        private string name = "";
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetPropertyValue(value, ref name, "Name"); }
        }

        private string log = "";
        /// <summary>
        /// 模拟表与终端的通讯日志
        /// </summary>
        public string Log
        {
            get { return log; }
            set { SetPropertyValue(value, ref log, "Log"); }
        }
    }
}
