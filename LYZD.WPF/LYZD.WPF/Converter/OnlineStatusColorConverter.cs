using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace LYZD.WPF.Converter
{
    /// <summary>
    ///上线状态颜色转换器
    /// </summary>
    public class OnlineStatusColorConverter : IValueConverter
    {
        /// <summary>
        /// 结论颜色转换器
        /// </summary>
        /// <param name="value">合格:绿色,不合格:红色,默认:黑色</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string temp = value as string;
            switch (temp)
            {
                case "0":  //未上线
                    return new SolidColorBrush(Colors.Transparent);
                    //return new SolidColorBrush(Color.FromArgb(0xFF, 0xA4, 0xA4, 0xA4));
                case "1":  //上线
                    return new SolidColorBrush(Color.FromArgb(0xFF, 0x2E, 0XFE, 0X2E));
                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }
        /// 未定义
        /// <summary>
        /// 未定义
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
