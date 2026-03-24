using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LY.VirtualMeter.Base
{
    public class NotifyPropertyBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 属性值变化时发生
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        ///用于设置具有相等属性值的泛型方法
        /// </summary>
        internal protected bool SetPropertyValue<T>(T value, ref T field, string propertyName)
        {
            if ((value != null && !value.Equals(field)) || (value == null && field != null))
            {
                field = value;
                if (propertyName != null)
                {
                    OnPropertyChanged(propertyName);
                }
                return true;
            }
            return false;
        }

        public virtual event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    }
}
