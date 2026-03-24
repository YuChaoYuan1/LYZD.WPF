using LYZD.ViewModel;
using LYZD.ViewModel.PowerControl;
using System.Windows;

namespace LYZD.WPF.View
{
    /// <summary>
    /// View_PowerControl.xaml 的交互逻辑
    /// </summary>
    public partial class View_PowerControl 
    {
        public View_PowerControl()
        {
            InitializeComponent();
            DockStyle.FloatingSize = new Size(800, 500);
            Name = "功率源控制";
            DockStyle.IsFloating = true;
            StdData.DataContext = EquipmentData.StdInfo;
        }
        private PowerControlModel viewModel
        {
            get { return Resources["PowerControlModel"] as PowerControlModel; }
        }
        //private void MainWindow_Closed(object sender, System.ComponentModel.CancelEventArgs e)
        //{ 
        //    viewModel.SaveValue();

        //}


    }
}
