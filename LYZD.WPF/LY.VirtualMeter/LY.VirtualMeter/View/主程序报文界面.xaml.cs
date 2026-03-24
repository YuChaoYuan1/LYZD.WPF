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
    /// 主程序报文界面.xaml 的交互逻辑
    /// </summary>
    public partial class 主程序报文界面 : UserControl
    {
        public 主程序报文界面()
        {
            InitializeComponent();
            DataContext = AppData.LogViewModel;

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            //textbox1.SelectionStart = AppData.LogViewModel.Log_PC.Length;
        }

        private void Click_IsChecked(object sender, RoutedEventArgs e)
        {
            AppData.LogViewModel.Log_PC = "";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProtocolAnalysis.Portocol_645 portocol_645 = new ProtocolAnalysis.Portocol_645();
            string s1 = ""; string s2 = ""; string s3 = ""; bool b1 = false; bool b2 = false;
            string str = "68 18 06 36 04 00 91 68 91 08 33 33 34 33 33 33 41 33 F9 16";
            portocol_645.Analysis(str, "11", ref s1, ref s2, ref b1, ref b2, ref s3);
            textbox1.Text = s2;
        }
    }
}
