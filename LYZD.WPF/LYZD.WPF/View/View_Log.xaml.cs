using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using LYZD.ViewModel.Log;
using LYZD.WPF.Model;
using LYZD.WPF.View.Windows;
//using DevComponents.WpfDock;
namespace LYZD.WPF.View
{
    /// <summary>
    /// View_Log.xaml 的交互逻辑
    /// </summary>
    public partial class View_Log
    {
        public View_Log()
        {

            InitializeComponent();
            Name = "运行日志";
            DockStyle.Position = eDockSide.Bottom;
            DockStyle.CanClose = false;
            DockStyle.CanFloat = false;
            DockStyle.CanDockAsDocument = false;
            //DockStyle.CanDockBottom = false;
            DockStyle.CanDockLeft = false;
            DockStyle.CanDockRight = false;
            DockStyle.CanDockTop = false;
            //LogViewModel.Instance.LogViewModelS
            //dataGrid.ItemsSource = LogViewModel.Instance.LogsCheckLogic;
            //LogViewModel.LogCollection logCollection = dataGrid.ItemsSource as LogViewModel.LogCollection;
            //if (logCollection != null)
            //{
            //    logCollection.CollectionChanged += logCollection_CollectionChanged;
            //}

            //【标注】 需要修改部分，右键清空日志，加给清空所有日志，清空日志是清空当前选中的tab的日志，tab绑定数据需要修改
            logTab.DataContext = viewModel;
            logTab.SelectedIndex = 0;//设置默认选中0
            for (int i = 0; i < viewModel.LogViews.Count; i++)
            {
                LogViewModel.LogCollection logCollection = viewModel.LogViews[i].LogsCheckLogic;
                if (logCollection != null)
                {
                    logCollection.CollectionChanged += logCollection_CollectionChanged;
                }
            }
            //logTab.SelectionChanged += logTab_SelectionChanged;
        }


        LogViewModelS viewModel = LogViewModelS.Instance;

        void logCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ContentPresenter cp = this.logTab.Template.FindName("PART_SelectedContentHost", this.logTab) as ContentPresenter;
            if (cp == null) return;
            DataGrid dataGrid = System.Windows.Media.VisualTreeHelper.GetChild(cp, 0) as DataGrid;
            //将表格滚动至最后一行
            var border = System.Windows.Media.VisualTreeHelper.GetChild(dataGrid, 0) as Decorator;
            if (border != null)
            {
                var scroll = border.Child as ScrollViewer;
                if (scroll != null) scroll.ScrollToEnd();
            }

        }

        //清空日志
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //LogViewModel.LogCollection collection = dataGrid.ItemsSource as LogViewModel.LogCollection;
            if (logTab.SelectedIndex < 0) return;
            LogViewModel.LogCollection collection = viewModel.LogViews[logTab.SelectedIndex].LogsCheckLogic as LogViewModel.LogCollection;
            if (collection != null)
            {
                collection.Clear();
            }

        }

        public override void Dispose()
        {
            //LogViewModel.LogCollection logCollection = dataGrid.ItemsSource as LogViewModel.LogCollection;
            //if (logCollection != null)
            //{
            //    logCollection.CollectionChanged -= logCollection_CollectionChanged;
            //}
            ////清除绑定
            //BindingOperations.ClearAllBindings(this);
            //dataGrid.ItemsSource = null;
            //menuItemClearLog.Click -= MenuItem_Click;
            for (int i = 0; i < viewModel.LogViews.Count; i++)
            {
                LogViewModel.LogCollection logCollection = viewModel.LogViews[i].LogsCheckLogic;
                if (logCollection != null)
                {
                    logCollection.CollectionChanged -= logCollection_CollectionChanged;
                }
            }
            //menuItemClearLog.Click -= MenuItem_Click;
            BindingOperations.ClearAllBindings(this);
            logTab.DataContext = null;
            base.Dispose();
        }

        private void logTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            logCollection_CollectionChanged(null, null);
        }

        private void MenuItem_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (logTab.SelectedIndex < 0) return;
           LogViewModel.LogCollection collection = viewModel.LogViews[logTab.SelectedIndex].LogsCheckLogic as LogViewModel.LogCollection;
            //MenuItem menu = e.OriginalSource as MenuItem;
            //if (menu == null) return;
            //LogViewModel nodeTemp = menu.DataContext as LogViewModel;
            //Clipboard.SetDataObject(nodeTemp.TipMessage);
            if (collection != null)   //当前选中的是那个    
            {

                TabItem a = (TabItem)logTab.Items[logTab.SelectedIndex];
                //TabItem a = (TabItem)logTab.SelectedItem;
                //a.Controls.
                //logTab.Items[logTab.SelectedIndex] 


                //string str= collection[viewModel.LogViews[logTab.SelectedIndex].Index].Message;
                //collection[0].Message
                //viewModel.LogViews[logTab.SelectedIndex].s
                //collection.
            }
        }

        private void BorderRadiusButton_Click_1(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            if (button == null) return;
            LogUnitViewModel nodeTemp = button.DataContext as LogUnitViewModel;
            Window_MessageBox.Instance.MessageShow(nodeTemp.Message);
        }
    }

}

