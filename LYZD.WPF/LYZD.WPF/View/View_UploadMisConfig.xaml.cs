using LYZD.ViewModel.MisConfig;
using LYZD.ViewModel.Schema;
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

namespace LYZD.WPF.View
{
    /// <summary>
    /// View_UploadMisConfig.xaml 的交互逻辑
    /// </summary>
    public partial class View_UploadMisConfig 
    {
        public View_UploadMisConfig()
        {
            InitializeComponent();
            Name = "上传配置";
            treeSchema.ItemsSource = FullTree.Instance.Children;
        }
        public MisConfigViewModel viewModel
        {
            get { return Resources["MisConfigViewModel"] as MisConfigViewModel; }
        }

        private void treeSchema_ActiveItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SchemaNodeViewModel nodeTemp = treeSchema.SelectedItem as SchemaNodeViewModel;
            if (nodeTemp != null && nodeTemp.IsTerminal)
            {
                viewModel.ParaNo = nodeTemp.ParaNo;
            }
        }
    }
}
