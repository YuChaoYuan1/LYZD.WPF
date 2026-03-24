using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.WPF.Controls
{
    /// <summary>
    /// 带圆角的按钮
    /// </summary>
    public class BorderRadiusButton : System.Windows.Controls.Button
    {
        #region 属性
        //圆角
        public int BorderRadius

        {
            get { return (int)GetValue(BorderRadiusProperty); }

            set { SetValue(BorderRadiusProperty, value); }
        }

        public static readonly System.Windows.DependencyProperty BorderRadiusProperty = System.Windows.DependencyProperty.Register("BorderRadius", typeof(int), typeof(BorderRadiusButton), new System.Windows.FrameworkPropertyMetadata());

        #endregion 属性
    }
}
