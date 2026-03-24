using LYZD.WPF.Model;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Media;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using System.Windows;
//using DevComponents.WpfDock;
using LYZD.WPF.Skin;
using LYZD.ViewModel;
using LYZD.Utility;
using LYZD.WPF.Schema;
using LYZD.DAL.DataBaseView;
using LYZD.WPF.View.Windows;
using LYZD.ViewModel.Time;
using System.Diagnostics;
using System;
using System.Windows.Controls;

namespace LYZD.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private Window_TestControl verifyWindow = new Window_TestControl();
        //public static event EventHandler EventCloseWindow;
        public MainWindow()
        {
            InitializeComponent();

            ////窗体打开动画
            //System.Windows.Media.Animation.Storyboard sb = new System.Windows.Media.Animation.Storyboard();
            //System.Windows.Media.Animation.ThicknessAnimation dmargin = new System.Windows.Media.Animation.ThicknessAnimation();
            //dmargin.Duration = new TimeSpan(0, 0, 0, 1, 0);
            //dmargin.From = new Thickness(0, Height / 2, 0, Height / 2);
            //dmargin.To = new Thickness(0, 0, 0, 0);
            //System.Windows.Media.Animation.Storyboard.SetTarget(dmargin, GridMain);
            //System.Windows.Media.Animation.Storyboard.SetTargetProperty(dmargin, new PropertyPath("Margin", new object[] { }));
            //sb.Children.Add(dmargin);
            //sb.Begin();

           DataContext = MainViewModel.Instance;
            MainViewModel.Instance.WindowsAll.CollectionChanged += WindowsAll_CollectionChanged;
            //MainViewModel.Instance.WindowsAll.change
            Loaded += MainWindow_Loaded;

            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
           
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ////检定窗体【开始检定，连续检定，停止检定三个按钮】
            if (DAL.Config.ConfigHelper.Instance.IsTestButtonSuspension)
            {
                verifyWindow.Show();
                verifyWindow.Owner = this;
            }
            if (Properties.Settings.Default.ShowTip)
            {
                Window_TipsMessage.Instance.Owner = this;
                Window_ThemeChoice.Instance.Owner = this;
            }

            EquipmentData.NavigateCurrentUi(); //导航到默认界面

            //窗体主题颜色，通过name进行修改
            string ThemeName = ViewModel.Const.OperateFile.GetINI("SkinList", "SkinCurrent", System.IO.Directory.GetCurrentDirectory() + "\\Skin\\SkinData.ini", "默认").Trim();  //获得当前设计的颜色
            ThemeItem itemTemp = SkinViewModel.Instance.Themes.FirstOrDefault(item => item.Name =="默认");
            if (ThemeName!="")
            {
                itemTemp = SkinViewModel.Instance.Themes.FirstOrDefault(item => item.Name == ThemeName);
            }
            

            if (itemTemp != null)
            {
                itemTemp.Load();
            }

            DispatcherTimer timerTemp = new DispatcherTimer();
            timerTemp.Interval = new System.TimeSpan(0, 0,1);
            timerTemp.Tick += (obj, eventArg) =>
            {
                if (itemTemp != null)
                {
                    itemTemp.Load();
                }
                timerTemp.Stop();
            };
            timerTemp.Start();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            MainViewModel.Instance.WindowsAll.CollectionChanged -= WindowsAll_CollectionChanged;
            base.OnClosed(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (EquipmentData.Controller.IsChecking)
            {
                if (MessageBox.Show("程序正在检定,确认要退出吗?", "退出程序", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            else if (MessageBox.Show("确认要退出检定吗?", "退出程序", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            Process[] myProcess = Process.GetProcessesByName("LY.VirtualMeter");
            if (myProcess.Length > 0)
                myProcess[0].Kill();
            EquipmentData.DeviceManager.UnLink(); //退出时候关闭设备连接--关源什么的
            verifyWindow.Close();
    

            System.Diagnostics.Process.GetCurrentProcess().Kill();
            base.OnClosing(e);
        }

        #region 添加窗体


        /// 新添窗体时
        /// <summary>
        /// 新添窗体时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WindowsAll_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList list = e.NewItems;
            if (list == null)
                return;
            for (int i = 0; i < e.NewItems.Count; i++)
            {
                DockWindowDisposable dockWindow = list[i] as DockWindowDisposable;
                if (dockWindow != null && dockWindow.CurrentControl != null)
                {
                    AddDockWindow(dockWindow);
                }
            }
        }

        private void AddDockWindow(DockWindowDisposable dockWindow)
        {
            eDockSide dockSide = dockWindow.CurrentControl.DockStyle.Position;
            if (dockSide == eDockSide.Tab)
            {
                if (dockWindow.CurrentControl.DockStyle.IsFloating) //是否悬浮
                {
                    Window_Show show = new Window_Show();
                    show.LoaiUserWindow(dockWindow);
                    show.Closing += Show_Closing;
                    show.Show();

                }
                else
                {
                    MainTabContro.TabItemClose tabItem = new MainTabContro.TabItemClose();

                    if (dockWindow.ImageControl!=null)
                    {
                        tabItem.LogoIcon = dockWindow.ImageControl;
                        //tabItem.LogoIcon = new System.Windows.Media.Imaging.BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + "\\images\\DockIcon\\" + dockWindow.Name + ".png", UriKind.Absolute));
                        tabItem.LogoIconWidth = 16;
                        tabItem.LogoIconHeigth = 16;
                    }
                    tabItem.Content = dockWindow;
                    tabItem.Header = dockWindow.Name;
                    //tabItem.Margin = new Thickness(2,0,0,0); 
        
                    AppDock2.Items.Add(tabItem);
                    AppDock2.SelectedIndex = AppDock2.Items.Count - 1;
                    dockWindow.Index = AppDock2.Items.Count - 1;
                }
            }
            return;




            
        }

        private void Show_Closing(object sender, CancelEventArgs e)
        {
            Activate();
        }

        private void DockWindow_AutoHideOpenChanged(object sender, RoutedEventArgs e)
        {
            //DockWindowDisposable dockWindow = (DockWindowDisposable)sender;
            //if (dockWindow.AutoHideOpen)
            //{
            //    dockWindow.IsAutoHide = !dockWindow.IsAutoHide;
            //}
        }

        

        #endregion

        #region 状态栏数据  

        private bool initialFlag = false;
        protected override void OnActivated(System.EventArgs e)
        {
            if (!initialFlag)
            {
                BindingStatusBar();
                initialFlag = true;
            }
            base.OnActivated(e);
        }

        /// <summary>
        /// 状态栏数据源绑定
        /// </summary>
        private void BindingStatusBar()
        {
            //右边检验员进度条等数据
            stackPanel_CheckerUser.DataContext = EquipmentData.LastCheckInfo;
            //联机图标数据源
            imageEqupmentStatus.DataContext = EquipmentData.DeviceManager;
            textEqupmentStatus.DataContext = EquipmentData.DeviceManager;
            //检定模式
            textBlockTestMode.DataContext = EquipmentData.Controller;
            //检定方案名称
            //textBlockSchemaName.DataContext = EquipmentData.SchemaModels;
            //当前步骤
            textBlockCheckIndex.DataContext = EquipmentData.Controller;
            //当前检定项目
            textBlockCheckName.DataContext = EquipmentData.Controller;

            lightCheckStatus.DataContext = EquipmentData.Controller;

            TipsStr.DataContext = EquipmentData.Controller;

            stackPanelTime.DataContext = TimeMonitor.Instance;
            _loading.DataContext = EquipmentData.Controller;

            //监视窗口
            monitorGrid.DataContext = EquipmentData.Controller;
        }


        #endregion

        private void panel_visible_click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            switch (btn.Tag.ToString())
            {
                case "日志信息":
                    if (logName.Visibility == Visibility.Visible)
                        logName.Visibility = Visibility.Collapsed;
                    else
                        logName.Visibility = Visibility.Visible;
                    break;
                case "标准表信息":
                    if (stdName.Visibility == Visibility.Visible)
                        stdName.Visibility = Visibility.Collapsed;
                    else
                        stdName.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
    }
}
