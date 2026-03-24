using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace LY.VirtualMeter.Base
{
    /// <summary>
    /// 命令基类
    /// </summary>
    public class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)   //绑定的是否可用
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.DoExcute?.Invoke(parameter);
        }
        public Action<object> DoExcute { get; set; }

        public CommandBase() { }
        public CommandBase(Action<object> action)
        {
            DoExcute = action;
        }

    }
}
