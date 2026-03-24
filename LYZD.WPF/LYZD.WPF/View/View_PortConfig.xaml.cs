using LYZD.DAL;
using LYZD.ViewModel.ProtConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LYZD.WPF.View
{
    /// <summary>
    /// View_PortConfig.xaml 的交互逻辑
    /// </summary>
    public partial class View_PortConfig
    {
        public View_PortConfig()
        {
            InitializeComponent();
            Name = "端口配置";
         
           // 绑定数据源到目录树所配置的数据源
            //columnQuickPort.ItemsSource = CodeDictionary.GetLayer2("QuickPort").Keys;
        }
        private PortConfigViewModel viewModel
        {
            get
            {
                return Resources["PortConfigViewModel"] as PortConfigViewModel;
            }
        }

        #region 表485


        /// <summary>
        /// 添加表端口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Add_Meter(object sender, RoutedEventArgs e)
        {
            MeterItems meter = new MeterItems();
            meter.Server = viewModel.Servers[0];
            meter.FlagChanged = true;
            if (viewModel.MeterItem.Count > 0)
            {
                MeterItems items = viewModel.MeterItem[viewModel.MeterItem.Count - 1];
                meter.PortCount = items.PortCount;
                meter.ComParam = items.ComParam;
                meter.IntervalValue = items.IntervalValue;
                meter.MaxTimePerByte  = items.MaxTimePerByte;
                meter.MaxTimePerFrame = items.MaxTimePerFrame;
                meter.Server = items.Server;
                meter.StartPort = items.StartPort;
            }

            viewModel.MeterItem.Add(meter);
        }

        /// <summary>
        /// 删除表端口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Del_Meter(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                MeterItems meter = button.DataContext as MeterItems;
                viewModel.MeterItem.Remove(meter);
            }
        }
        #endregion


        #region 设备

        /// <summary>
        /// 设备删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Del_Device(object sender, RoutedEventArgs e)
        {

            Button button = e.OriginalSource as Button;
            if (button != null)
            {
              DeviceItem deviceItem = button.DataContext as DeviceItem;
                if (deviceItem != null)
                {
                    for (int i = 0; i < viewModel.Groups.Count; i++)
                    {
                        if (viewModel.Groups[i].DeviceItems.Contains(deviceItem))
                        {
                            viewModel.Groups[i].DeviceItems.Remove(deviceItem);
                            break;
                        }
                    }
                }
            }



            //MessageBox.Show(str);
        }
        /// <summary>
        /// 设备添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Add_Device(object sender, RoutedEventArgs e)
        {
            Button buttonTemp = sender as Button;
            if (buttonTemp != null)
            {
                DeviceGroup groupTemp = buttonTemp.DataContext as DeviceGroup;
                if (groupTemp != null)
                {
                    if (groupTemp.DeviceItems.Count > 0)
                    {
                        DeviceItem device = groupTemp.DeviceItems[groupTemp.DeviceItems.Count - 1];
                        groupTemp.DeviceItems.Add(new DeviceItem()
                        {
                            Server = viewModel.Servers[0],
                            FlagChanged = false,
                            ComParam = device.ComParam,
                            MaxTimePerByte = device.MaxTimePerByte,
                            MaxTimePerFrame = device.MaxTimePerFrame,
                            Model = device.Model,
                            PortNum = device.PortNum,
                            IsLink=device.IsLink 
                             
                        });
                    }
                    else
                    {
                        groupTemp.DeviceItems.Add(new DeviceItem()
                        {
                            Server = viewModel.Servers[0],
                            FlagChanged = false
                        });
                    }
                }
            }
        }
        #endregion

    }
}
