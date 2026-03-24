using LY.VirtualMeter.View;
using LY.VirtualMeter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace LY.VirtualMeter.Assets.Controls
{
    /// <summary>
    /// MeterData.xaml 的交互逻辑
    /// </summary>
    public partial class MeterData : UserControl
    {
        ///// <summary>   
        ///// 获取鼠标的坐标   
        ///// </summary>   
        ///// <param name="lpPoint">传址参数，坐标point类型</param>   
        ///// <returns>获取成功返回真</returns>   

        //public struct POINT
        //{
        //    public int X;
        //    public int Y;
        //    public POINT(int x, int y)
        //    {
        //        this.X = x;
        //        this.Y = y;
        //    }

        //}

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern bool GetCursorPos(out POINT pt);

        public MeterData()
        {
            InitializeComponent();
            //test.MouseEnter += (s, r) =>
            //{
            //    //AppData.BaseData.SelectMeter = meter;
            //    //AppData.BaseData.获取电量();
            //    电表详情界面.Instance.Show();
            //    POINT p = new POINT();
            //    if (GetCursorPos(out p))//API方法
            //    {
            //        电表详情界面.Instance.Left = p.X;
            //        电表详情界面.Instance.Top = p.Y ;
            //    }
            //};
            //this.MouseLeave += (s, r) =>
            //{
            //    电表详情界面.Instance.Hide();
            //};

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
            {
                return;
            }
            Meter meter = button.DataContext as Meter;
            if (meter == null)
            {
                return;
            }
            AppData.BaseData.SelectMeter = meter;
            AppData.BaseData.获取电量();
            电表详情界面.Instance.Show();

            //MessageBox.Show("1");
        }
    }
}
