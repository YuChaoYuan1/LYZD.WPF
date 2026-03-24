using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace LY.VirtualMeter.Assets.Controls
{
   public class RadioImageButton : System.Windows.Controls.RadioButton
    {
        /// <summary>
        /// 图片
        /// </summary>
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(RadioImageButton),
            new PropertyMetadata(null));

        /// <summary>
        /// 图片的宽度
        /// </summary>
        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.Register("ImageWidth", typeof(double), typeof(RadioImageButton),
            new PropertyMetadata(double.NaN));

        /// <summary>
        /// 图片的高度
        /// </summary>
        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.Register("ImageHeight", typeof(double), typeof(RadioImageButton),
            new PropertyMetadata(double.NaN));

        /// <summary>
        /// 构造函数
        /// </summary>
        static RadioImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioImageButton),
                new System.Windows.FrameworkPropertyMetadata(typeof(RadioImageButton)));
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        public ImageSource Image
        {
            get
            {
                return GetValue(ImageProperty) as ImageSource;
            }
            set
            {
                SetValue(ImageProperty, value);
            }
        }

        /// <summary>
        /// 图片宽度(属性)
        /// </summary>
        public double ImageWidth
        {
            get
            {
                return (double)GetValue(ImageWidthProperty);
            }
            set
            {
                SetValue(ImageWidthProperty, value);
            }
        }

        /// <summary>
        /// 图片高度(属性)
        /// </summary>
        public double ImageHeight
        {
            get
            {
                return (double)GetValue(ImageHeightProperty);
            }
            set
            {
                SetValue(ImageHeightProperty, value);
            }
        }
    }
}
