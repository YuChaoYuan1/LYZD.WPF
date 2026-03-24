using LYZD.DAL;
using LYZD.ViewModel;
using LYZD.ViewModel.Menu;
using LYZD.ViewModel.User;
using LYZD.WPF.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LYZD.WPF.UiGeneral
{
    /// <summary>
    /// 目录生成器
    /// </summary>
    public class ControlFactory
    {
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="menuItem"></param>
        /// <param name="isMainMenu">是否为主界面目录</param>
        /// <returns></returns>
        public static Button CreateButton(MenuConfigItem menuItem, bool isMainMenu)
        {
            if (menuItem == null || !menuItem.IsValid)
            {
                return null;
            }
            if (isMainMenu && menuItem.IsMainMenu == EnumMainMenu.否)
            {
                return null;
            }
            string buttonHeader = menuItem.MenuName;
            #region 构造控件
            if (Application.Current.Resources.Contains(menuItem.MenuName))
            {
                buttonHeader = Application.Current.Resources[menuItem.MenuName] as string;
            }
            StackPanel stackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            Button button = new Button()
            {
                Content = stackPanel,
                Margin = new Thickness(3, 0, 3, 0),
                VerticalAlignment = VerticalAlignment.Bottom,
                Focusable = false,
                Style = Application.Current.Resources["StyleButtonMenu"] as Style
                //Style = Application.Current.Resources["GlassButton"] as Style

            };
            button.SetBinding(Button.CommandProperty, new Binding("LocalCommand"));
            Image imageCurrent = new Image()
            {
                //Margin = new Thickness(8, 0, 8, 0),  //===========
                Source = menuItem.MenuImage.ImageControl,
                Width = (menuItem.IsMainMenu == EnumMainMenu.否) ? 32 : 32,
                Height = (menuItem.IsMainMenu == EnumMainMenu.否) ? 32 : 32,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            TextBlock textBlockButtonText = new TextBlock()
            {
                //Foreground = System.Windows.Media.Brushes.White, //===========
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = buttonHeader 
            };
            stackPanel.Children.Add(imageCurrent);
            stackPanel.Children.Add(textBlockButtonText);
            #endregion
            #region 设置命令绑定
            if (menuItem.MenuType == EnumMenuType.设备控制 || menuItem.MenuType == EnumMenuType.检定控制)
            {
                button.DataContext = EquipmentData.DeviceManager;
                button.CommandParameter = menuItem.MenuClass;
            }
            else
            {
                button.CommandParameter = string.Format("{0}|{1}", menuItem.MenuName, menuItem.MenuClass);
            }
            #endregion
            #region 用户权限
            //控件仅有超级用户可见
            Binding bindingUserVisible = new Binding("USER_POWER");
            bindingUserVisible.Source = UserViewModel.Instance.CurrentUser;
            bindingUserVisible.Converter = Application.Current.Resources["UserVisibilityConverter"] as UserVisibilityConverter;
            bindingUserVisible.ConverterParameter = ((int)menuItem.UserCode).ToString();
            button.SetBinding(Button.VisibilityProperty, bindingUserVisible);
            //if (menuItem.UserCode == EnumUserVisible.普通用户不可见)
            //{
            //    //控件仅有管理员可见
            //    Binding bindingUserVisible = new Binding("chrQx");
            //    bindingUserVisible.Source = UserViewModel.Instance.CurrentUser;
            //    bindingUserVisible.Converter = new AdminUserVisiblityConverter();
            //    button.SetBinding(Button.VisibilityProperty, bindingUserVisible);
            //}
            //else if (menuItem.UserCode == EnumUserVisible.超级用户可见)
            //{
            //    //控件仅有超级用户可见
            //    Binding bindingUserVisible = new Binding("chrQx");
            //    bindingUserVisible.Source = UserViewModel.Instance.CurrentUser;
            //    bindingUserVisible.Converter = new UserVisibilityConverter();
            //    bindingUserVisible.ConverterParameter = ((int)menuItem.UserCode).ToString();
            //    button.SetBinding(Button.VisibilityProperty, bindingUserVisible);
            //}
            #endregion
            #region 检定时不可用
            if (menuItem.CheckingEnable == EnumCheckingEnable.不可用)
            {
                Binding bindingCheckEnable = new Binding("IsChecking");
                bindingCheckEnable.Source = EquipmentData.Controller;
                bindingCheckEnable.Mode = BindingMode.OneWay;
                bindingCheckEnable.Converter = new NotBoolConverter();
                button.SetBinding(Button.IsEnabledProperty, bindingCheckEnable);
            }
            #endregion
            return button;
        }

        /// <summary>
        /// 创建列
        /// </summary>
        /// <param name="columnHeader">列头名称</param>
        /// <param name="codeType">数据项编码</param>
        /// <param name="allowAdd">是否允许添加新项</param>
        /// <returns></returns>
        public static DataGridColumn CreateColumn(string columnHeader, string codeType, string bindingPath, bool allowAdd = false)
        {
            if (string.IsNullOrEmpty(codeType))
            {
                DataGridColumn column = new DataGridTextColumn
                {
                    Header = columnHeader,
                    Binding = new Binding(bindingPath) { Mode = BindingMode.TwoWay },
                    Width = 120,
                    IsReadOnly = false,
                    EditingElementStyle = Application.Current.Resources["StyleEditTextBox"] as Style
                };
                return column;
            }
            else
            {
                Dictionary<string, string> dictionary = CodeDictionary.GetLayer2(codeType);
                List<string> datasource = dictionary.Keys.ToList();
                if (allowAdd)
                {
                    datasource.Add("添加新项");
                }
                DataGridColumn column = new DataGridComboBoxColumn
                {
                    //StyleComboBox
                    Header = columnHeader,
                    Width = 80,
                    IsReadOnly = false,
                    ItemsSource = datasource,
                    SelectedItemBinding = new Binding(bindingPath) { Mode = BindingMode.TwoWay },
                    EditingElementStyle = Application.Current.Resources["StyleComboBox"] as Style,
                };
             
                return column;
            }
        }
    }
}
