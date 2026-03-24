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

namespace LY.VirtualMeter.View
{
    /// <summary>
    /// MeterDetails.xaml 的交互逻辑
    /// </summary>
    public partial class 电表详情界面 : Window
    {
        private static 电表详情界面 instance = null;

        public static 电表详情界面 Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new 电表详情界面();
                }
                return instance;
            }
        }
        public 电表详情界面()
        {
            InitializeComponent();
             DataContext = AppData.BaseData;
            //var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            //this.Left = desktopWorkingArea.Right - this.Width;
            //this.Top = desktopWorkingArea.Bottom - this.Height;
        }



        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image imageTemp = e.OriginalSource as System.Windows.Controls.Image;
            if (imageTemp != null)
            {
                switch (imageTemp.Name)
                {
                    case "imageClose":
                        this.Hide();
                        break;
                    case "imageTopping":
                        if (this.Topmost)
                        {
                            this.Topmost = false;
                            imageTopping.Source = new BitmapImage(new Uri(@"/Assets/Images/钉子2.png", UriKind.RelativeOrAbsolute));
                        }
                        else
                        {
                            this.Topmost = true;
                            imageTopping.Source = new BitmapImage(new Uri(@"/Assets/Images/钉子3.png", UriKind.RelativeOrAbsolute));

                        }
                        break;


                }
            }
        }
        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
