using LYZD.ViewModel.Schema.Error;
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

namespace LYZD.WPF.Schema.Error
{
    /// <summary>
    /// ControlTreeInitialError.xaml 的交互逻辑
    /// </summary>
    public partial class ControlTreeInitialError : UserControl
    {
        public ControlTreeInitialError()
        {
            InitializeComponent();
            DataContext = AllPoints2;
            AllPoints2.PointsChanged += AllPoints_PointsChanged;
        }
        void AllPoints_PointsChanged(object sender, EventArgs e)
        {
            if (PointsChanged != null)
            {
                PointsChanged(sender, e);
            }
        }


        public AllErrorModel AllPoints2
        {
            get { return (AllErrorModel)GetValue(AllPointsProperty); }
            set { SetValue(AllPointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllPoints.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllPointsProperty =
            DependencyProperty.Register("AllPoints2", typeof(AllErrorModel), typeof(ControlTreeError), new PropertyMetadata(new AllErrorModel()));

        public event EventHandler PointsChanged;


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                if (button.DataContext is ErrorCategory)
                {
                    ErrorCategory categoryToRemove = button.DataContext as ErrorCategory;
                    while (categoryToRemove.ErrorPoints.Count > 0)
                    {
                        ErrorModel errorPoint = categoryToRemove.ErrorPoints[0];
                        errorPoint.FlagRemove = true;
                        if (PointsChanged != null)
                        {
                            PointsChanged(errorPoint, null);
                        }
                        categoryToRemove.ErrorPoints.Remove(errorPoint);
                    }
                    AllPoints2.Categories.Remove(button.DataContext as ErrorCategory);
                }
            }
        }
    }
}
