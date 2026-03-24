using LY.VirtualMeter.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LY.VirtualMeter.ViewModel
{
    public class MainViewWindow : NotifyPropertyBase
    {
        private UIElement _mainContent;
        public UIElement MainContent
        {
            get { return _mainContent; }
            set { SetPropertyValue(value, ref _mainContent, "MainContent"); }
        }

        public CommandBase TabChangeCommand { get; set; }

        public MainViewWindow()
        {
            TabChangeCommand = new CommandBase(OnTabChange);
            OnTabChange("LY.VirtualMeter.View.电表信息界面");  //默认载入界面
        }

        private void OnTabChange(Object obj)
        {
            if (obj == null) return;
            Type type = Type.GetType(obj.ToString());
            this.MainContent = (UIElement)Activator.CreateInstance(type);
        }
    }
}
