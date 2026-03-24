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
    /// 终端报文界面.xaml 的交互逻辑
    /// </summary>
    public partial class 终端报文界面 : UserControl
    {
        public 终端报文界面()
        {
            InitializeComponent();
            DataContext = AppData.BaseData;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox text = (TextBox)sender;
            //for (int i = 0; i < length; i++)
            //{

            //}
            //text.SelectionStart = AppData.LogViewModel.Log_Meter[0].Log.Length;
        }

        private void Click_IsChecked(object sender, RoutedEventArgs e)
        {
            int index = ZDTab.SelectedIndex;
            if (index < 0 || index > AppData.BaseData.TerminalObject.Count)
            {
                return;
            }
            AppData.BaseData.TerminalObject[index].MeterLog.Log="" ;
        }
    }
}
