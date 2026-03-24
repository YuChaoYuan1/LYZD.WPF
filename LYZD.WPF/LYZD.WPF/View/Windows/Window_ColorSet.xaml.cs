using LYZD.ViewModel;
using LYZD.WPF.Skin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LYZD.WPF.View.Windows
{
    /// <summary>
    /// Window_ColorSet.xaml 的交互逻辑
    /// </summary>
    public partial class Window_ColorSet : Window
    {

        private static Window_ColorSet instance = null;

        public static Window_ColorSet Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Window_ColorSet();
                }
                return instance;
            }
        }
        public Window_ColorSet()
        {
            InitializeComponent();
            this.Topmost = true;
            LoadZhuTi();
            GetAppColor();
            //Panel_ZhuTi
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image imageTemp = e.OriginalSource as Image;
            if (imageTemp != null)
            {
                switch (imageTemp.Name)
                {
                    case "imageClose":
                        Visibility = Visibility.Collapsed;
                        break;
                    default:
                        break; ;
                }
            }
        }

        private ThemeItem DefColor = null;
        private void LoadZhuTi()
        {
            string[] fileNames = Directory.GetFiles(string.Format(@"{0}\Skin", Directory.GetCurrentDirectory()));
            foreach (string fileName in fileNames)
            {
                string[] arrayName = fileName.Split('\\');
                string nameTemp = arrayName[arrayName.Length - 1];
                if (nameTemp.EndsWith(".xaml"))
                {
                    //ThemeItem item2 = new ThemeItem(nameTemp.TrimEnd(".xaml".ToCharArray()));

                    ThemeItem themeItem = SkinViewModel.Instance.Themes.FirstOrDefault(item => item.Name == nameTemp.TrimEnd(".xaml".ToCharArray()));
                    //themeItem = item2;
                    Controls.ColorBlock color = new Controls.ColorBlock();
                    color.Margin = new Thickness(10, 0, 10, 0);
                    color.Width = 20;
                    color.Height = 20;
                    color.DataContext = themeItem;
                    color.MouseDown += Color_MouseDown;
                    //color.Background = item.ThemeBrush;
                    color.ToolTip = nameTemp.TrimEnd(".xaml".ToCharArray());
                    Panel_ZhuTi.Children.Add(color);
                    if (nameTemp.IndexOf("默认") != -1)
                    {
                        DefColor = themeItem;
                    }

                }
            }
        }

        private void Color_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Controls.ColorBlock color = (Controls.ColorBlock)sender;
            if (color != null)
            {
                ThemeItem item = (ThemeItem)color.DataContext;
                if (item != null)
                {
                    item.Load();
                    GetAppColor();
                }
            }

        }

        private void GetAppColor()
        {
            //Application.Current.Resources["分隔条颜色"] as Brush;
            //viewModel.IsSetColor = true;
            viewModel.窗口背景色 = Application.Current.Resources["窗口背景色"] as Brush;
            viewModel.窗口背景深色 = Application.Current.Resources["窗口背景深色"] as Brush;
            viewModel.分隔条颜色 = Application.Current.Resources["分隔条颜色"] as Brush;
            viewModel.边框颜色浅 = Application.Current.Resources["边框颜色浅"] as Brush;
            viewModel.边框颜色 = Application.Current.Resources["边框颜色"] as Brush;
            viewModel.字体颜色浅 = Application.Current.Resources["字体颜色浅"] as Brush;
            viewModel.字体颜色标准 = Application.Current.Resources["字体颜色标准"] as Brush;
            viewModel.屏幕颜色 = Application.Current.Resources["屏幕颜色"] as Brush;
            viewModel.表行颜色 = Application.Current.Resources["表行颜色"] as Brush;
            viewModel.表行颜色1 = Application.Current.Resources["表行颜色1"] as Brush;
            viewModel.工作状态颜色 = Application.Current.Resources["工作状态颜色"] as Brush;
            viewModel.控件有效 = Application.Current.Resources["控件有效"] as Brush;
            viewModel.窗口背景深色半透明 = Application.Current.Resources["窗口背景深色半透明"] as Brush;
            viewModel.线性渐变颜色 = Application.Current.Resources["线性渐变颜色"] as LinearGradientBrush;
            //viewModel.IsSetColor = false;

        }

        private SetColorData viewModel
        {
            get { return Resources["SetColorData"] as SetColorData; }
        }

        private void Btn_Default(object sender, RoutedEventArgs e)
        {
            if (DefColor != null)
            {
                DefColor.Load();
                GetAppColor();
            }
        }

        private void Btn_Save(object sender, RoutedEventArgs e)
        {
            //viewModel.窗口背景色 = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0, 0));
            //ThemeItem itemTemp = SkinViewModel.Instance.Themes.FirstOrDefault(item => item.Name == "默认");
            ResourceDictionary dictionary = SkinViewModel.Instance.SelectThemes.GetResource();
            //dictionary.
            if (dictionary.Contains("窗口背景色"))
            {
                dictionary["窗口背景色"] = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0, 0));
            }

            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            //允许使用该对话框的自定义颜色
            colorDialog.AllowFullOpen = true;
            colorDialog.FullOpen = true;
            colorDialog.ShowHelp = true;
            //初始化当前文本框中的字体颜色，
            colorDialog.Color = System.Drawing.Color.Black;
            //当用户在ColorDialog对话框中点击"取消"按钮
            colorDialog.ShowDialog();

            //string strColor = System.Drawing.ColorTranslator.ToHtml(colorDialog.Color);

            //this.colorBtn.Content = strColor;
        }

        private void SetColor(string name)
        {
            ResourceDictionary dictionary = SkinViewModel.Instance.SelectThemes.GetResource();
            //dictionary.
            if (dictionary.Contains(name))
            {
                System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
                //允许使用该对话框的自定义颜色
                colorDialog.AllowFullOpen = true;
                colorDialog.FullOpen = true;
                colorDialog.ShowHelp = true;
                //初始化当前文本框中的字体颜色，
                colorDialog.Color = System.Drawing.Color.Black;
                //当用户在ColorDialog对话框中点击"取消"按钮
                colorDialog.ShowDialog();
                if (name.IndexOf("渐变")<0)
                {
                    dictionary[name] = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                }
                else
                {

                    //渐变色的情况只能修改一个颜色，所以浅色自动计算   、
                    //GradientStopCollection stops = new GradientStopCollection();
                    //stops.Add(new GradientStop(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B), 0));
                    //stops.Add(new GradientStop(ChangeColor(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B),-1), 1));
                    //dictionary[name] = new LinearGradientBrush(stops, new Point(0.5, 0), new Point(0.5, 1));

                    GradientStopCollection stops = new GradientStopCollection();
                    stops.Add(new GradientStop(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B), 0));
                    stops.Add(new GradientStop(Color.FromArgb(0xAA, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B), 1));
                    dictionary[name] = new LinearGradientBrush(stops, new Point(0.5, 0), new Point(0.5, 1));
                }
                SkinViewModel.Instance.SelectThemes.Load();
                GetAppColor();
            }

        }
        public static Color ChangeColor(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            if (red < 0) red = 0;

            if (red > 255) red = 255;

            if (green < 0) green = 0;

            if (green > 255) green = 255;

            if (blue < 0) blue = 0;

            if (blue > 255) blue = 255;



            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }


        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border border = (Border)sender;
            var Name = border.Tag.ToString();
            SetColor(Name);
        }
    }

    public class SetColorData : ViewModelBase
    {
        public bool IsSetColor = false;

        private Brush _窗口背景色;
        public Brush 窗口背景色
        {
            get { return _窗口背景色; }
            set
            {
                SetPropertyValue(value, ref _窗口背景色, "窗口背景色");
                SetColor("窗口背景色", 窗口背景色);
            }
        }
        private Brush _窗口背景深色;
        public Brush 窗口背景深色
        {
            get { return _窗口背景深色; }
            set { 
                SetPropertyValue(value, ref _窗口背景深色, "窗口背景深色"); 
                SetColor("窗口背景深色", 窗口背景深色);
            }
        }
        private Brush _分隔条颜色;
        public Brush 分隔条颜色
        {
            get { return _分隔条颜色; }
            set { SetPropertyValue(value, ref _分隔条颜色, "分隔条颜色");
                SetColor("分隔条颜色", 分隔条颜色);
            }
   
        }
        private Brush _边框颜色浅;
        public Brush 边框颜色浅
        {
            get { return _边框颜色浅; }
            set { SetPropertyValue(value, ref _边框颜色浅, "边框颜色浅");
                SetColor("边框颜色浅", 边框颜色浅);
            }
        }
        private Brush _边框颜色;
        public Brush 边框颜色
        {
            get { return _边框颜色; }
            set { SetPropertyValue(value, ref _边框颜色, "边框颜色"); 
                SetColor("边框颜色", 边框颜色);
            }
        }
        private Brush _字体颜色浅;
        public Brush 字体颜色浅
        {
            get { return _字体颜色浅; }
            set { SetPropertyValue(value, ref _字体颜色浅, "字体颜色浅");
                SetColor("字体颜色浅", 字体颜色浅);
            }
        }
        private Brush _字体颜色标准;
        public Brush 字体颜色标准
        {
            get { return _字体颜色标准; }
            set { SetPropertyValue(value, ref _字体颜色标准, "字体颜色标准");
                SetColor("字体颜色标准", 字体颜色标准);
            }
        }
        private Brush _屏幕颜色;
        public Brush 屏幕颜色
        {
            get { return _屏幕颜色; }
            set { SetPropertyValue(value, ref _屏幕颜色, "屏幕颜色");
                SetColor("屏幕颜色", 屏幕颜色);
            }
        }
        private Brush _表行颜色;
        public Brush 表行颜色
        {
            get { return _表行颜色; }
            set { SetPropertyValue(value, ref _表行颜色, "表行颜色"); 
                SetColor("表行颜色", 表行颜色);
            }
        }
        private Brush _表行颜色1;
        public Brush 表行颜色1
        {
            get { return _表行颜色1; }
            set { SetPropertyValue(value, ref _表行颜色1, "表行颜色1");
                SetColor("表行颜色1", 表行颜色1);
            }
        }
        private Brush _工作状态颜色;
        public Brush 工作状态颜色
        {
            get { return _工作状态颜色; }
            set { SetPropertyValue(value, ref _工作状态颜色, "工作状态颜色");
                SetColor("工作状态颜色", 工作状态颜色);
            }
        }
        private Brush _控件有效;
        public Brush 控件有效
        {
            get { return _控件有效; }
            set { SetPropertyValue(value, ref _控件有效, "控件有效"); 
                SetColor("控件有效", 控件有效);
            }
        }
        private Brush _窗口背景深色半透明;
        public Brush 窗口背景深色半透明
        {
            get { return _窗口背景深色半透明; }
            set { SetPropertyValue(value, ref _窗口背景深色半透明, "窗口背景深色半透明");
                SetColor("窗口背景深色半透明", 窗口背景深色半透明);
            }
        }
        private Brush _渐变色浅色;
        public Brush 渐变色浅色
        {
            get { return _渐变色浅色; }
            set {
                if (value != _渐变色浅色)
                {
                    Set线性渐变色();
                }
  
                SetPropertyValue(value, ref _渐变色浅色, "渐变色浅色");

            }
        }
        private Brush _渐变色深色;
        public Brush 渐变色深色
        {
            get { return _渐变色深色; }
            set {
                if (value != _渐变色深色)
                {
                    Set线性渐变色();
                }
                SetPropertyValue(value, ref _渐变色深色, "渐变色深色");

            }
        }
        private LinearGradientBrush _线性渐变颜色;
        public LinearGradientBrush 线性渐变颜色
        {
            get { return _线性渐变颜色; }
            set {
                if (value != _线性渐变颜色)
                {
                    Set线性渐变色深浅();
                }
                //SetColor("渐变色浅色", 渐变色浅色);
                SetPropertyValue(value, ref _线性渐变颜色, "线性渐变颜色");

            }
        }
        private void Set线性渐变色()
        {
            //if (渐变色深色 == null || 渐变色浅色 == null)
            //{
            //    return;
            //}
            //GradientStopCollection stops = new GradientStopCollection();
            //stops.Add(new GradientStop(((SolidColorBrush)渐变色深色).Color, 0));
            //stops.Add(new GradientStop(((SolidColorBrush)渐变色浅色).Color, 1));
            //线性渐变色 = new LinearGradientBrush(stops, new Point(0.5, 0), new Point(0.5, 1));
        }
        private void Set线性渐变色深浅()
        {
            //if (线性渐变色==null)
            //{
            //    return;
            //}   
            //渐变色深色 = new SolidColorBrush(线性渐变色.GradientStops[0].Color);
            //渐变色浅色 = new SolidColorBrush(线性渐变色.GradientStops[1].Color);
        }

        private void SetColor(string Name,object color)
        {
            //if (IsSetColor)
            //{
            //    return;
            //}
            //if (Application.Current.Resources.MergedDictionaries.Contains(Name))
            //{

            //}
            //Application.Current.Resources[Name] = color;
            //Application.Current.Resources.Remove(Name);
            //Application.Current.Resources.Add(Name, color);
            // System.Threading.Thread.Sleep(50);
        }

    }
}
