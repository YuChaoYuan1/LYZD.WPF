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
using System.Windows.Shapes;

namespace LYZD.WPF.View.Windows
{
    /// <summary>
    /// Window_MessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class Window_MessageBox : Window
    {
        private static Window_MessageBox instance = null;

        public static Window_MessageBox Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Window_MessageBox();
                }
                return instance;
            }
        }
        public Window_MessageBox()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed;
            //LogViewModel.Instance.PropertyChanged += Instance_PropertyChanged;
            Topmost = true;
        }
        public void MessageShow(string message)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    Visibility = Visibility.Visible;
                    textBlock.Text = message;
                    copytext.Text = "";
                    Show();
                }
                catch
                { }
            }));
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void Copy_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetDataObject(textBlock.Text);
                copytext.Text = "复制成功";
            }
            catch (Exception)
            {
                copytext.Text = "复制失败";
            }
        }
    }
}
