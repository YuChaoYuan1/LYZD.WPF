using LY.VirtualMeter.Assets.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LY.VirtualMeter.View
{
    /// <summary>
    /// 电表信息界面.xaml 的交互逻辑
    /// </summary>
    public partial class 电表信息界面 : UserControl
    {
        public 电表信息界面()
        {
            InitializeComponent();
            DataContext = AppData.BaseData;

            //for (int i = 0; i < 24; i++)
            //{
            //    MeterData meter = new MeterData();
            //    panel.Children.Add(meter);
            //}
        }
       
    }
}
