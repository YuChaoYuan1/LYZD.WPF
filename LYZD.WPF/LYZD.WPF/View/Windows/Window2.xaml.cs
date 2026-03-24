using LYZD.DAL.DataBaseView;
using LYZD.Utility;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using LYZD.ViewModel.Const;
using LYZD.ViewModel.Schema;
using LYZD.ViewModel.Time;
using LYZD.ViewModel.User;
using LYZD.WPF.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LYZD.WPF.View.Windows
{
    /// <summary>
    /// Window2.xaml 的交互逻辑
    /// </summary>
    public partial class Window2 : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        bool IsLogin = true;
        public Window2()
        {
            InitializeComponent();
            InitializeComponent();
            LoadUsers(); //载入用户
            DataContext = EquipmentData.LastCheckInfo;
            Chk_IsDeom.DataContext = EquipmentData.Equipment;
            UiInterface.UiDispatcher = SynchronizationContext.Current;
            timeBar.DataContext = EquipmentData.Equipment;
            timeBar.SetBinding(ProgressBar.ValueProperty, new Binding("ProgressBarValue"));
            textBlockLogin.DataContext = EquipmentData.Equipment;
            textBlockLogin.SetBinding(TextBlock.TextProperty, new Binding("TextLogin"));

           string str= OperateFile.GetINI("Config", "IsDeom", System.IO.Directory.GetCurrentDirectory() + "\\Ini\\ConfigTem.ini").Trim();  //保存当前设计的颜色
            EquipmentData.version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (str.ToLower()=="true")
            {
                EquipmentData.Equipment.IsDemo = true;
            }

            if (EquipmentData.Equipment.AutoLogion)  //自动登入
            {
                timer.Interval = new TimeSpan(0, 0, 3);
                timer.Tick += timer_Tick;
                timer.IsEnabled = true;
                timer.Start();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!IsLogin)
            {
                timer.Stop();
                return;
            }

            if (CheckLogin())
            {
                gridContent.Visibility = Visibility.Collapsed;
                gridLogIn.Visibility = Visibility.Visible;
                TimeMonitor.Instance.LogIn();
                InitializeCheckInfo();
            }
            timer.Stop();
        }

        /// <summary>
        /// 加载用户
        /// </summary>
        private void LoadUsers()
        {
            List<string> userNames = UserViewModel.Instance.GetList("");    //获得所有用户
            cmb_Checker.ItemsSource = userNames;
            cmb_Checker.SelectedItem = EquipmentData.LastCheckInfo.TestPerson;
            cmb_Auditor.ItemsSource = userNames;
            cmb_Auditor.SelectedItem = EquipmentData.LastCheckInfo.AuditPerson;
        }

        //拖动
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsLogin = false;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        //按钮，登入和取消
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                IsLogin = false;
                switch (button.Name)
                {
                    case "buttonLogin":   //登入
                        if (CheckLogin())
                        {
                            gridContent.Visibility = Visibility.Collapsed;
                            gridLogIn.Visibility = Visibility.Visible;
                            TimeMonitor.Instance.LogIn();
                            InitializeCheckInfo();

                        }
                        break;
                    case "buttonQuit":
                        Close();
                        break;
                }
            }
        }

        /// <summary>
        /// 登入判断
        /// </summary>
        /// <returns></returns>
        private bool CheckLogin()
        {
            bool flag = true;

            string checker = cmb_Checker.Text;
            string password1 = passwordBoxChecker.Password;
            string auditor = cmb_Auditor.Text;
            string password2 = passwordBoxAuditor.Password;
            if (!UserViewModel.Instance.Login(auditor, password2))
            {
                passwordBoxAuditor.BorderBrush = new SolidColorBrush(Colors.Red);
                flag = false;
            }
            else
            {
                passwordBoxAuditor.BorderBrush = SystemColors.ControlDarkDarkBrush;
            }
            if (!UserViewModel.Instance.Login(checker, password1))
            {
                passwordBoxChecker.BorderBrush = new SolidColorBrush(Colors.Red);
                flag = false;
            }
            else
            {
                passwordBoxChecker.BorderBrush = SystemColors.ControlDarkDarkBrush;
            }
            if (flag)
            {
                EquipmentData.LastCheckInfo.AuditPerson = auditor;
                EquipmentData.LastCheckInfo.ProtectedCurrent = comboBoxCurrent.SelectedItem as string;
                EquipmentData.LastCheckInfo.ProtectedVoltage = comboBoxVoltage.SelectedItem as string;
            }
            return flag;
        }

        private void InitializeCheckInfo()
        {
            new Thread(() =>
            {
                EquipmentData.Equipment.TextLogin = "软件登录中,请等待:配置信息加载完毕,开始加载方案信息...";
                EquipmentData.Equipment.ProgressBarValue = 30; //进度条进度
                FullTree.Instance.Initialize();
                EquipmentData.Equipment.TextLogin = "软件登录中,请等待:正在加载结论视图...";
                EquipmentData.Equipment.ProgressBarValue = 60;
                ResultViewHelper.Initialize(); //初始化所有结论试图
                EquipmentData.Equipment.TextLogin = "软件登录中,请等待:结论视图加载完毕,开始初始化检定信息...";
                EquipmentData.Equipment.ProgressBarValue = 90;
                EquipmentData.Initialize();
                EquipmentData.Equipment.ProgressBarValue = 100;


                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MainWindow mainWindow = new MainWindow();
                    Application.Current.MainWindow = mainWindow;
                    mainWindow.Show();
                    Close();
                }));
            }).Start();
        }

        private void IsDeom_Click(object sender, RoutedEventArgs e)
        {
            OperateFile.WriteINI("Config", "IsDeom", Chk_IsDeom.IsChecked.ToString(), System.IO.Directory.GetCurrentDirectory() + "\\Ini\\ConfigTem.ini");
        }

        private void IsDeom_Click2(object sender, RoutedEventArgs e)
        {
            OperateFile.WriteINI("Config", "IsDeom", Chk_IsDeom.IsChecked.ToString(), System.IO.Directory.GetCurrentDirectory() + "\\Ini\\ConfigTem.ini");
        }
    }
}
