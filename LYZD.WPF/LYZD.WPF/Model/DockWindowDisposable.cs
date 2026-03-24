//using DevComponents.WpfDock;
using System;
using System.Windows;
using LYZD.WPF.Skin;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LYZD.WPF.Model
{
    public class DockWindowDisposable : UserControl// : DockWindow
    {
        //public string ImageName;

        //public bool Visible { get; set; } = true;
        public int Index { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public BitmapImage ImageControl { get; set; }



        public bool IsSelected { get; set; }

        /// 在创建窗体时对此控件赋值
        /// <summary>
        /// 在创建窗体时对此控件赋值
        /// </summary>
        public DockControlDisposable CurrentControl
        {
            get { return Content as DockControlDisposable; }
        }
        /// 关闭窗体时调用控件注销方法
        /// <summary>
        /// 关闭窗体时调用控件注销方法
        /// </summary>
        /// <param name="e"></param>
        public void OnClosed(System.Windows.RoutedEventArgs e)
        {
            if (CurrentControl != null)
            {
                CurrentControl.Dispose();
            }
            Content = null;
            MainViewModel.Instance.WindowsAll.Remove(this);
            //

            //for (int i = this.Index + 1; i < MainViewModel.Instance.WindowsAll.Count; i++)
            //{
            //    MainViewModel.Instance.WindowsAll[i].Index--;
            //}
            for (int i = 0; i < MainViewModel.Instance.WindowsAll.Count; i++)
            {
                if (MainViewModel.Instance.WindowsAll[i].Index>this.Index)
                {
                    MainViewModel.Instance.WindowsAll[i].Index--;
                }
            }

            //DependencyObject obj = LogicalTreeHelper.GetParent(this);

            //Window window= Window.GetWindow(this);
            //if (window!=null)
            //{
            //    window.Close();
            //}
            //if (obj is DockWindowGroup)
            //{
            //    ((DockWindowGroup)obj).Items.Remove(this);
            //    if (((DockWindowGroup)obj).Items.Count > 0)
            //    {
            //        ((DockWindow)((DockWindowGroup)obj).Items[0]).IsSelected = true;
            //    }
            //}
            //base.OnClosed(e);
            GC.Collect();
            GC.SuppressFinalize(this);
        }
        ///用来重新显示窗体
        //public void Show()
        //{
        //    Window window = Window.GetWindow(this);
        //    if (window != null)
        //    {
        //        window.Show();
        //    }
        //}

        //protected override void OnDockParentChanged(EventArgs e)
        //{
        //    IsSelected = true;
        //    base.OnDockParentChanged(e);
        //    SkinManager.ChangeWindowSkin(CurrentControl);
        //}
        //protected override void OnTabVisibilityChanged(RoutedEventArgs e)
        //{
        //    base.OnTabVisibilityChanged(e);
        //    SkinManager.ChangeWindowSkin(CurrentControl);
        //}

        public void OnClosed2(System.Windows.RoutedEventArgs e)
        {
            if (CurrentControl != null)
            {
                CurrentControl.Dispose();
            }
            Content = null;
            MainViewModel.Instance.WindowsAll.Remove(this);
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.Close();
            }
            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}
