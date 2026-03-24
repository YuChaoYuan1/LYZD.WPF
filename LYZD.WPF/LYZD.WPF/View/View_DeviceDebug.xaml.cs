using LYZD.DAL.Config;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using LYZD.ViewModel.DebugViewModel;
using LYZD.ViewModel.Device;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LYZD.WPF.View
{
    /// <summary>
    /// View_DeviceDebug.xaml 的交互逻辑
    /// </summary>
    public partial class View_DeviceDebug
    {
        private PowerViewModel viewModel
        {
            get { return Resources["PowerViewModel"] as PowerViewModel; }
        }

        private ErrorControlViewModel viewModel2
        {
            get { return Resources["ErrorControlViewModel"] as ErrorControlViewModel; }
        }

        private MeterControlViewModel MeterViewModel
        {
            get { return Resources["MeterControlViewModel"] as MeterControlViewModel; }
        }

        private Debug_ProtViewModel protViewModel
        {
            get { return Resources["Debug_ProtViewModel"] as Debug_ProtViewModel; }
        }


        private SocketViewModel socketViewModel
        {
            get { return Resources["SocketViewModel"] as SocketViewModel; }
        }

        private Debug_HarmonicViewModel HarmonicViewModel
        {
            get { return Resources["Debug_HarmonicViewModel"] as Debug_HarmonicViewModel; }
        }

        //public Dictionary<string, MeterStartControlViewModel> meterStartS = new Dictionary<string, MeterStartControlViewModel>();

        public View_DeviceDebug()
        {
            InitializeComponent();
            Name = "设备调试";
    
            for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
            {
                Controls.MeterStartControl meterStart = new Controls.MeterStartControl();
                MeterStartControlViewModel meter = new MeterStartControlViewModel();
                meter.MeterNo = (i + 1).ToString().PadLeft(2,'0');
                meterStart.DataContext = meter;
                if (!viewModel2.meterStartS.ContainsKey((i + 1).ToString()))
                {
                    viewModel2.meterStartS.Add((i + 1).ToString(), meter);
                }
                if (i < EquipmentData.Equipment.MeterCount/2)
                {
                     panel1.Children.Add(meterStart);
                }
                else
                { 
                    panel2.Children.Add(meterStart);
                }
            }
            if (!EquipmentData.Equipment.IsDemo && ConfigHelper.Instance.VerifyModel == "自动模式")
            {
                if (EquipmentData.DeviceManager.Devices.ContainsKey(ViewModel.Device.DeviceName.误差板 ))
                {
                    List<DeviceData> deviceDatas = EquipmentData.DeviceManager.Devices[ViewModel.Device.DeviceName.误差板];
                    int DeviceCount = deviceDatas.Count; //误差板485数量
                    int MaxTaskCountPerThread = EquipmentData.Equipment.MeterCount / DeviceCount;//每个485备带几个误差板
                    isReadMeterStart = true;
                    for (int i = 0; i < DeviceCount; i++)
                    {
                        int startpos = i * MaxTaskCountPerThread;
                        int endpos = startpos + MaxTaskCountPerThread;
                        ReadMeterStart(startpos+1, endpos);
                    }
                }
            }

            initializationHarmonic();


        }
        public override void Dispose()
        {
            isReadMeterStart = false;
             isReadHarmonicData = false;



        }
        bool isReadMeterStart = false;
        bool isReadHarmonicData = true;

        /// <summary>
        /// 读取表位状态
        /// </summary>
        public void ReadMeterStart(int start,int end)
        {
          
            Task task = new Task(() =>
            {
                while (true)
                {
                    for (int i = start; i <= end; i++)
                    {
                        if (!isReadMeterStart)
                        {
                            break;
                        }
                        byte[] OutResult;
          

                        bool t = EquipmentData.DeviceManager.Read_Fault(03, (byte)i, out OutResult);
                        if (t)
                        {
                            if (!viewModel2.meterStartS.ContainsKey(i.ToString()))
                            {
                                continue;
                            }
                            MeterStartControlViewModel meter = viewModel2.meterStartS[i.ToString()];
                            meter.Motor = OutResult[2];
                            if (OutResult[3] == 1)
                                meter.IsMeter = true;
                            else
                                meter.IsMeter = false;
                        }
                        else
                        {
                        }
                    }
                    System.Threading.Thread.Sleep(200);
                }
            });
            task.Start();
        }
        
        private QueryDataBase DataBase
        {
            get { return Resources["QueryDataBase"] as QueryDataBase; }
        }

        private void tableNameChanged(object sender, SelectionChangedEventArgs e)
        {

        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataBase.Data = DataBase.GeneralDal.GetAllTableData(DataBase.TableName);
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            //MeterProtocolAdapter.Instance.ReadAddress(111);
            //DataBase.Data = DataBase.GeneralDal.GetAllTableData(DataBase.TableName,txt_sql.Text);
            string str = txt_sql.Text.Trim().Trim('\"'); ;
            string connString = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", str);
            DataBase.GeneralDal = new DAL.GeneralDal(connString);
            List<string> fieldModels = DataBase.GeneralDal.GetTableNames();
            DataBase.TableNames.Clear();
            for (int i = 0; i < fieldModels.Count(); i++)
            {
                DataBase.TableNames.Add(fieldModels[i]);
            }
        }

        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_test_click(object sender, RoutedEventArgs e)
        {
            Mis.Common.IMis mis = Mis.MISFactory.Create();
            mis.UpdateInit();

            mis.UpdateCompleted(EquipmentData.LoadDETECT_TASK.DETECT_TASK_NO, EquipmentData.Equipment.ID);
            
            //分量脉冲测试
            //bool[] t = new bool[1];
            //t[0] = false ;
            //EquipmentData.DeviceManager.SetZBGZDYContrnl(255,12, t);

            //EquipmentData.DeviceManager.SetZBSJContrnl5(1,0,0,0,0);
            //var a = 1;


            //ViewModel.MeterResoultModel.MeterDataHelper.GetDnbInfoNew();

            //EquipmentData.DeviceManager.ControlDoorSignalRelayrRelay(0, 1, 0);
            //EquipmentData.DeviceManager.ContnrRemoteSignalingStatusOutput(1, true, true, false, false, false, false, 0);
            //EquipmentData.DeviceManager.SetPulseOutput( 0x03,1,2, 0.5f, 0, 2,0.5f, 0);
            //var a=2;


            //Window_TipsLog log = new Window_TipsLog();
            //log.Show();
            //var c = Application.Current.Resources.Keys.Count;


            //var b = Application.Current.Resources["分隔条颜色"] as Brush;
            // var a=   Application.Current.Resources.Count;
            //Window_ColorSet colorSet = new Window_ColorSet();
            //colorSet.Show();
            //Application.Current.Resources.Remove("窗口背景色");
            //Application.Current.Resources.Add("窗口背景色", new SolidColorBrush(Color.FromArgb(0xff,0xff, 0, 0)));

            //Application.Current.Resources.Remove("字体颜色标准");
            //Application.Current.Resources.Add("字体颜色标准", new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0, 0)));

            //Application.Current.Resources.Remove("窗口背景深色");
            //Application.Current.Resources.Add("窗口背景深色", new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0x80, 0)));

            //Application.Current.Resources.Remove("分隔条颜色");
            //Application.Current.Resources.Add("分隔条颜色", new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xff, 0)));

            //Application.Current.Resources.Remove("边框颜色浅");
            //Application.Current.Resources.Add("边框颜色浅", new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0xff, 0xff)));

            //Application.Current.Resources.Remove("边框颜色");
            //Application.Current.Resources.Add("边框颜色", new SolidColorBrush(Color.FromArgb(0xff, 0x00, 0xbf, 0xff)));

            //Application.Current.Resources.Remove("字体颜色浅");
            //Application.Current.Resources.Add("字体颜色浅", new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0, 0xff)));

            //Application.Current.Resources.Remove("屏幕颜色");
            //Application.Current.Resources.Add("屏幕颜色", new SolidColorBrush(Color.FromArgb(0xff, 0xA4, 0xa4, 0xa4)));

            //Application.Current.Resources.Remove("表行颜色");
            //Application.Current.Resources.Add("表行颜色", new SolidColorBrush(Color.FromArgb(0xff, 0xcc,0x2e, 0xfa)));

            //Application.Current.Resources.Remove("表行颜色1");
            //Application.Current.Resources.Add("表行颜色1", new SolidColorBrush(Color.FromArgb(0xff, 0x08, 0x08, 0x8a)));


            //GradientStopCollection stops = new GradientStopCollection();
            //stops.Add(new GradientStop(Color.FromArgb(0xff, 0x33, 0x34, 0x3b), 0));
            //stops.Add(new GradientStop(Color.FromArgb(0xff, 0xca, 0xc6, 0xc5), 1));

            //LinearGradientBrush linear = new LinearGradientBrush(stops, new Point(0.5, 0), new Point(0.5, 1));
            //Application.Current.Resources.Remove("线性渐变颜色");
            //Application.Current.Resources.Add("线性渐变颜色", linear);


            //Application.Current.Resources.Remove("工作状态颜色");
            //Application.Current.Resources.Add("工作状态颜色", new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0, 0)));

            //Application.Current.Resources.Remove("控件有效");
            //Application.Current.Resources.Add("控件有效", new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0, 0)));

            //Application.Current.Resources.Remove("窗口背景深色半透明");
            //Application.Current.Resources.Add("窗口背景深色半透明", new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0, 0)));

        }

        /// <summary>
        /// 清空接收区
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            protViewModel.ReceiveData = "";
        }
         /// <summary>
         /// 清空发送区
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
        private void MenuItem_Click2(object sender, RoutedEventArgs e)
        {
            protViewModel.SendData = "";
        }


        #region 谐波调试
        /// <summary>
        /// 初始化谐波数据
        /// </summary>
        private void initializationHarmonic()
        {
            //tabHarmonic.Items.Add(new obje);
            string[] name = new string[] { "UA", "UB", "UC", "IA", "IB", "IC" };
            for (int z = 1; z <= 6; z++)
            {
                Dictionary<int, HarmonicData> keyValues = new Dictionary<int, HarmonicData>();
     
                TabItem item = new TabItem();
                item.Header = name[z - 1];
                item.Width = 100;
                //item.Background= System.Windows.Media.Brush.
                StackPanel panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;
                for (int i = 1; i <= 4; i++)
                {
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Orientation = Orientation.Vertical;
                    stackPanel.Margin = new Thickness(10, 5, 10, 5);
                    StackPanel stack2 = new StackPanel();
                    stack2.Orientation = Orientation.Horizontal;
                    TextBlock textTips = new TextBlock();
                    textTips.TextAlignment = TextAlignment.Center;
                    textTips.Text = $"次数";
                    textTips.Width = 50;

                    TextBlock text11 = new TextBlock();
                    text11.TextAlignment = TextAlignment.Center;
                    text11.Text = "含量";
                    text11.Width = 70;
                    text11.Margin = new Thickness(5, 0, 5, 5);

                    TextBlock text22 = new TextBlock();
                    text22.TextAlignment = TextAlignment.Center;
                    text22.Text = "相位";
                    text22.Width = 70;
                    text22.Margin = new Thickness(5, 0, 5, 5);
                    stack2.Children.Add(textTips);
                    stack2.Children.Add(text11);
                    stack2.Children.Add(text22);
                    stackPanel.Children.Add(stack2);


                    for (int j = 1; j <= 15; j++)
                    {
                        HarmonicData data = new HarmonicData();
                        StackPanel stack = new StackPanel();

                        stack.DataContext = data;
                        stack.Orientation = Orientation.Horizontal;
                        TextBlock text = new TextBlock();
                        text.Text = $"第{(i - 1) * 15 + j}次：";
                        text.Width = 50;
                        TextBox text1 = new TextBox();
                        text1.Width = 70;
                        text1.Style = null;
                        text1.Margin = new Thickness(5, 0, 5, 5);
                        Binding binding = new Binding("HarmonicContent");
                        text1.SetBinding(TextBox.TextProperty, binding);

                        TextBox text2 = new TextBox();
                        text2.Width = 70;
                        text2.Style = null;
                        text2.Margin = new Thickness(5, 0, 5, 5);
                        Binding binding2 = new Binding("HarmonicPhase");
                        text2.SetBinding(TextBox.TextProperty, binding2);
                        stack.Children.Add(text);
                        stack.Children.Add(text1);
                        stack.Children.Add(text2);
                        stackPanel.Children.Add(stack);

                        if (!keyValues.ContainsKey((i - 1) * 15 + j))
                        {
                            keyValues.Add((i - 1) * 15 + j, data);
                        }
                    }
                    panel.Children.Add(stackPanel);
                }




                if (!HarmonicViewModel.harmonicData.ContainsKey(name[z - 1]))
                {
                    HarmonicViewModel.harmonicData.Add(name[z - 1], keyValues);
                }
                item.Content = panel;
                tabHarmonic.Items.Add(item);
            }
            ReadHarmonicData();
        }


        /// <summary>
        /// 读取标准表谐波数据
        /// </summary>
        private void ReadHarmonicData()
        {
            string[] name = new string[] { "UA", "UB", "UC", "IA", "IB", "IC" };
            Task task = new Task(() =>
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(1000);
                    if (!isReadHarmonicData)  break;
                    if (!HarmonicViewModel. IsReadStaData) continue;
             
                    float[] HarmonicEnerg= EquipmentData.DeviceManager.ReadHarmonicEnergy();
                    float[] HarmonicAngle = EquipmentData.DeviceManager.ReadHarmonicAngle();
                    if (HarmonicEnerg == null || HarmonicAngle == null) continue;
                    if (HarmonicEnerg.Length>=192 && HarmonicAngle.Length>=192)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            for (int i = 1; i <= 60; i++)
                            {
                                HarmonicViewModel.harmonicData[name[j]][i].HarmonicContent = HarmonicEnerg[i];
                                HarmonicViewModel.harmonicData[name[j]][i].HarmonicPhase = HarmonicAngle[i];
                            }
                        }
                    }
                }
            });
            task.Start();
        }
        #endregion

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            VerifyBase.ControlVirtualMeter("Cmd,Set,220,220,220,5,5,5,0,0,0,1,0");
            VerifyBase.ControlVirtualMeter("Cmd,DLS,5000,4000,1000,1000,1000,1000");
            VerifyBase.ControlVirtualMeter("MOP");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Mis.Common.IMis mis = Mis.MISFactory.Create();
            mis.UpdateInit();

            mis.UpdateCompleted();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (EquipmentData.SingleOffPower)
            {
                EquipmentData.SingleOffPower = false;
            }
            else
            {
                EquipmentData.SingleOffPower = true;
            }

            if (EquipmentData.SingleOffPower) {
                ContrlSinglePower.Content = "启用单步检定结束不关源";
            }
            else
            {
                ContrlSinglePower.Content = "启用单步检定结束关源";
            }
            
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            EquipmentData.CallMsg("VerifyCompelate");
        }
    }
}
