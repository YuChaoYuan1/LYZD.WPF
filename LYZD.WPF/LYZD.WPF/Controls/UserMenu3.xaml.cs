using LYZD.DAL.Config;
using LYZD.Utility.Log;
using LYZD.ViewModel;
using LYZD.ViewModel.Menu;
using LYZD.WPF.Skin;
using LYZD.WPF.UiGeneral;
using LYZD.WPF.View.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Shapes = System.Windows.Shapes;

namespace LYZD.WPF.Controls
{
    /// <summary>
    /// UserMenu3.xaml 的交互逻辑
    /// </summary>
    public partial class UserMenu3 : UserControl
    {
        public UserMenu3()
        {
            InitializeComponent();
            LoadMenu();
            MouseLeftButtonDown += UserMenu_MouseLeftButtonDown;

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            string name = "深圳隆元科技有限公司";
            if (ConfigHelper.Instance.IsVersionNumber)
            {
                name += $"({version.ToString()})";
            }
            if (ConfigHelper.Instance.IsDeviceNumber)
            {
                name += $" - { ViewModel.EquipmentData.Equipment.ID }号";
            }
            TestName.ToolTip = name;
            //
            //if (!ConfigHelper.Instance.IsTestButtonSuspension)
            //{
            //    VerifyGrid.DataContext = EquipmentData.Controller;
            //    VerifyGrid.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    VerifyGrid.Visibility = Visibility.Collapsed;
            //}
            TimeText.DataContext= EquipmentData.Controller;
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    EquipmentData.Controller.TimeText = DateTime.Now.ToString("HH:mm:ss");
                    System.Threading.Thread.Sleep(1000);
                }
            });
        }

        private void LoadMenu()
        {
            MenuViewModel menuViewModel = new MenuViewModel();
            EnumMenuType menuType = menuViewModel.Menus[0].MenuType;
            for (int i = 0; i < menuViewModel.Menus.Count; i++)
            {
                Button button = ControlFactory.CreateButton(menuViewModel.Menus[i], true);

                if (menuType != menuViewModel.Menus[i].MenuType)  //设备类型发生改变--添加分割符号
                {
                    Border border = new Border()
                    {
                        Height = 32,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 2,
                        Background = System.Windows.Media.Brushes.Orange,
                    };
                    stackPannelMenu.Children.Add(border);
                    menuType = menuViewModel.Menus[i].MenuType;
                }

                if (button != null)
                {
                    stackPannelMenu.Children.Add(button);
                }
            }
        }


        //数据管理
        private void StartDataManage()
        {

            Process[] processes = Process.GetProcesses();
            Process processTemp = processes.FirstOrDefault(item => item.ProcessName == "LYZD.DataManage");
            if (processTemp == null)
            {
                try
                {
                    Process.Start(string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), "LYZD.DataManager.exe"));
                    //LYZD.DataManager.MainWindow main = new DataManager.MainWindow();
                    //main.Show();

                }
                catch (Exception e)
                {
                    LogManager.AddMessage(string.Format("数据管理程序启动失败:{0}", e.Message), EnumLogSource.用户操作日志, EnumLevel.Error);
                }
            }
            else
            {
                return;
            }
        }


        #region 基础功能

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Shapes.Path)
            {
                ThemeItem itemTemp = ((Shapes.Path)sender).DataContext as ThemeItem;
                if (itemTemp != null)
                {
                    itemTemp.Load();
                    //Properties.Settings.Default.FacadeName = itemTemp.Name;
                    Properties.Settings.Default.Save();
                }
            }
        }

        //双击改变大小
        private void UserMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                {
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                    imageMax.Source = new BitmapImage(new Uri(@"../Images/Max.png", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
                    imageMax.Source = new BitmapImage(new Uri(@"../Images/Reduction.png", UriKind.RelativeOrAbsolute));
                }
            }
        }


        //功能按钮
        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image imageTemp = e.OriginalSource as Image;
            if (imageTemp != null)
            {
                switch (imageTemp.Name)
                {
                    case "imageMin":
                        Application.Current.MainWindow.WindowState = WindowState.Minimized;
                        break;
                    case "imageMax":
                        if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                        {
                            Application.Current.MainWindow.WindowState = WindowState.Normal;
                            imageMax.Source = new BitmapImage(new Uri(@"../Images/Max.png", UriKind.RelativeOrAbsolute));
                        }
                        else
                        {
                            Application.Current.MainWindow.WindowState = WindowState.Maximized;
                            imageMax.Source = new BitmapImage(new Uri(@"../Images/Reduction.png", UriKind.RelativeOrAbsolute));
                        }
                        break;
                    case "imageClose":
                        ViewModel.EquipmentData.ApplicationIsOver = true;
                        try
                        {
                            Application.Current.MainWindow.Close();
                        }
                        catch
                        {
                            Application.Current.Shutdown();
                        }
                        break;
                    case "ThemeChoice":  //切换主题
                                         //Window_ThemeChoice window_Theme =  Window_ThemeChoice.Instance;
                                         //window_Theme.Show();
                        Window_ColorSet.Instance.Show();
                        //Window_ColorSet colorSet = new Window_ColorSet();
                        //  colorSet.Show();
                        //window_Theme.
                        break;
                }
            }
            if (e.OriginalSource is TextBlock)
            {
                TextBlock textBlock = e.OriginalSource as TextBlock;
                if (textBlock.Name == "TextDataManager")
                {
                    StartDataManage();
                }
            }

        }


        /// 拖动
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                Application.Current.MainWindow.DragMove();
            }
        }
        #endregion
    }
}
