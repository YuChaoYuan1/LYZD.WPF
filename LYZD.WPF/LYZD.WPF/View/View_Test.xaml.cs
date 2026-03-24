//using DevComponents.WpfDock;
using LYZD.Utility;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckInfo;
using LYZD.WPF.Converter;
using LYZD.WPF.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace LYZD.WPF.View
{
    /// <summary>
    /// View_Test.xaml 的交互逻辑
    /// </summary>
    public partial class View_Test
    {
        public View_Test()
        {
            InitializeComponent();
            Name = "检定";
            InitializeColumns();
            comboBoxSchema.DataContext = EquipmentData.SchemaModels;
            treeSchema1.DataContext = EquipmentData.CheckResults;
            EquipmentData.CheckResults.PropertyChanged += CheckResults_PropertyChanged;
            DockStyle.Position = eDockSide.Tab;
            DockStyle.FloatingSize = SystemParameters.WorkArea.Size;
            textBlockCheckPara.DataContext = EquipmentData.Controller;
            Binding bindingRefresh = new Binding("IsChecking");
            bindingRefresh.Source = EquipmentData.Controller;
            bindingRefresh.Converter = new NotBoolConverter();
            buttonRefresh.SetBinding(IsEnabledProperty, bindingRefresh);
            comboBoxSchema.SetBinding(IsHitTestVisibleProperty, bindingRefresh);
            treeSchema1.Loaded += treeScheme_Loaded;
        }

        /// <summary>
        /// 初始化表位消息
        /// </summary>
        private void InitializeColumns()
        {
            //删除所有表位列
            GridViewColumnCollection columns = Application.Current.Resources["ColumnCollection"] as GridViewColumnCollection;
            while (columns.Count > 2)
            {
                columns.RemoveAt(2);
            }
            //载入表位
            for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
            {
                //CheckBox checkBoxTemp = new CheckBox
                //{
                //    HorizontalAlignment = HorizontalAlignment.Center,
                //    VerticalAlignment = VerticalAlignment.Center,
                //    Content = string.Format("{0}表位", i + 1),
                //};
                //checkBoxTemp.PreviewMouseLeftButtonDown += CheckBoxTemp_PreviewMouseLeftButtonDown;
                //Binding binding = new Binding("MD_CHECKED");
                //binding.Source = EquipmentData.MeterGroupInfo.Meters[i];
                //binding.Converter = new BoolBitConverter();
                //checkBoxTemp.SetBinding(ToggleButton.IsCheckedProperty, binding);
                //GridViewColumn column = new GridViewColumn
                //{
                //    Header = checkBoxTemp,
                //    Width = 60,  //TODO2:修改检定界面列大小
                //};

                Grid grid = new Grid();
                Border border = new Border();
                //border.Background = Brushes.Red;
                //文本
                Binding bindingColor2 = new Binding("MD_ONLINE");
                bindingColor2.Source = EquipmentData.MeterGroupInfo.Meters[i];
                bindingColor2.Converter = new OnlineStatusColorConverter();
                border.SetBinding(Border.BackgroundProperty, bindingColor2);

                grid.Children.Add(border);

                   
                CheckBox checkBoxTemp = new CheckBox
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Content = string.Format("{0}表位", i + 1),
                };
                 
                checkBoxTemp.PreviewMouseLeftButtonDown += CheckBoxTemp_PreviewMouseLeftButtonDown;
                Binding binding = new Binding("MD_CHECKED");
                binding.Source = EquipmentData.MeterGroupInfo.Meters[i];
                binding.Converter = new BoolBitConverter();
                checkBoxTemp.SetBinding(ToggleButton.IsCheckedProperty, binding);

                grid.Children.Add(checkBoxTemp);

                GridViewColumn column = new GridViewColumn
                {
                    Header = grid,
                    Width = 60,  //TODO2:修改检定界面列大小
                };

                #region 动态模板
                DataTemplate dataTemplateTemp = new DataTemplate();
                FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock), "textBlock");
                //上下文
                Binding bindingDataContext = new Binding(string.Format("ResultSummary.表位{0}", i + 1));
                factory.SetBinding(TextBlock.DataContextProperty, bindingDataContext);
                ////文本
                Binding bindingText = new Binding("ResultValue");
                factory.SetBinding(TextBlock.TextProperty, bindingText);
                //Binding bindingText2 = new Binding("TerminalOnline");
                //factory.SetBinding(TextBlock.TagProperty, bindingText2);
                dataTemplateTemp.VisualTree = factory;
                Binding bindingColor = new Binding("Result");
                bindingColor.Converter = new ResultColorConverter();
                factory.SetBinding(TextBlock.ForegroundProperty, bindingColor);
                column.CellTemplate = dataTemplateTemp;
                #endregion
                //TerminalOnline
                columns.Add(column);  //添加列

            }

        }


        /// <summary>
        /// 表位复选框改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxTemp_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CheckBox temp = sender as CheckBox;
            if (temp.IsChecked.HasValue)
            {
                DynamicViewModel modelTemp = temp.GetBindingExpression(CheckBox.IsCheckedProperty).DataItem as DynamicViewModel;
                DAL.DynamicModel Model2 = new DAL.DynamicModel();
                if (modelTemp != null)
                {
                    object objTemp = modelTemp.GetProperty("MD_CHECKED");
                    if (objTemp.ToString() == "1")
                    {
                        modelTemp.SetProperty("MD_CHECKED", "0");
                        Model2.SetProperty("MD_CHECKED", "0");
                    }
                    else
                    {
                        modelTemp.SetProperty("MD_CHECKED", "1");
                        Model2.SetProperty("MD_CHECKED", "1");

                    }
                    TaskManager.AddWcfAction(() =>
                    {
                        EquipmentData.MeterGroupInfo.UpdateCheckFlag();
                    });
                    EquipmentData.CheckResults.RefreshYaojian();
                }
                string id = modelTemp.GetProperty("METER_ID") as string;
                string where1 = $"METER_ID = '{id}'";
                DAL.DALManager.MeterTempDbDal.Update("T_TMP_METER_INFO", where1, Model2, new List<string> { "MD_CHECKED" });
            }
            e.Handled = true;

        }

        void treeScheme_Loaded(object sender, RoutedEventArgs e)
        {
            if (EquipmentData.CheckResults.CheckNodeCurrent == null)
            {
                return;
            }
            if (EquipmentData.CheckResults.CheckNodeCurrent.Level == 1)
            {
                TreeViewItem treeItem = treeSchema1.ItemContainerGenerator.ContainerFromItem(EquipmentData.CheckResults.CheckNodeCurrent.Parent) as TreeViewItem;
                if (treeItem != null)
                {
                    treeItem.IsExpanded = true;
                }
            }
        }



        private void CheckResults_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CheckNodeCurrent")
            {
                return;
            }
            Dispatcher.Invoke(new Action(() =>
            {
                CheckNodeViewModel nodeSelected = treeSchema1.SelectedItem as CheckNodeViewModel;
                if (nodeSelected == EquipmentData.CheckResults.CheckNodeCurrent)
                {
                    return;
                }
                if (EquipmentData.CheckResults.CheckNodeCurrent != null)
                {
                    List<CheckNodeViewModel> nodeList = new List<CheckNodeViewModel>() { EquipmentData.CheckResults.CheckNodeCurrent };
                    CheckNodeViewModel nodeParent = EquipmentData.CheckResults.CheckNodeCurrent.Parent;
                    while (nodeParent != null)
                    {
                        nodeList.Add(nodeParent);
                        nodeParent = nodeParent.Parent;
                    }
                    TreeViewItem treeItem = treeSchema1.ItemContainerGenerator.ContainerFromItem(nodeList[nodeList.Count - 1]) as TreeViewItem;
                    if (treeItem == null)
                    {
                        return;
                    }
                    else
                    {
                        treeItem.IsExpanded = true;
                    }
                    for (int i = nodeList.Count - 2; i >= 0; i--)
                    {
                        treeItem = treeItem.ItemContainerGenerator.ContainerFromItem(nodeList[i]) as TreeViewItem;
                        if (treeItem == null)
                        {
                            break;
                        }
                        else
                        {
                            treeItem.IsExpanded = true;
                        }
                    }
                    if (treeItem != null)
                    {
                        treeItem.IsSelected = true;
                        treeItem.BringIntoView();
                    }
                }
            }));
        }

        public override void Dispose()
        {
            treeSchema1.Loaded -= treeScheme_Loaded;
            BindingOperations.ClearAllBindings(this);
            comboBoxSchema.DataContext = null;
            treeSchema1.DataContext = null;
            treeViewDecorator.Target = null;
            EquipmentData.CheckResults.PropertyChanged -= CheckResults_PropertyChanged;
            treeSchema1.SelectedItemChanged -= treeSchema1_SelectedItemChanged;
            base.Dispose();
        }



        /// <summary>
        /// 展开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_SchemaExpand(object sender, RoutedEventArgs e)
        {
            MenuItem menuTemp = e.OriginalSource as MenuItem;
            if (menuTemp != null)
            {
                if (menuTemp.Name == "clearData")  //清除数据
                {
                    string[] arrayResult = new string[EquipmentData.CheckResults.CheckNodeCurrent.CheckResults.Count];
                    List<string> list = EquipmentData.CheckResults.CheckNodeCurrent.CheckResults[0].GetAllProperyName();

                    for (int i = 0; i < list.Count; i++)     //修改所有属性
                    {
                        if (list[i] == "要检") continue;
                        for (int k = 0; k < EquipmentData.CheckResults.CheckNodeCurrent.CheckResults.Count; k++)
                        {
                            arrayResult[k] = "";
                        }
                        EquipmentData.CheckResults.UpdateCheckResult(EquipmentData.CheckResults.CheckNodeCurrent.ItemKey, list[i], arrayResult);
                    }

                }

                else if (menuTemp.Name == "setResoultOK")//手动合格
                {
                    List<string> list = EquipmentData.CheckResults.CheckNodeCurrent.CheckResults[0].GetAllProperyName();
                    string[] arrayResult = new string[EquipmentData.CheckResults.CheckNodeCurrent.CheckResults.Count];
                    for (int k = 0; k < EquipmentData.CheckResults.CheckNodeCurrent.CheckResults.Count; k++)
                    {
                        arrayResult[k] = "合格";
                    }
                    EquipmentData.CheckResults.UpdateCheckResult(EquipmentData.CheckResults.CheckNodeCurrent.ItemKey, "结论", arrayResult);
                }
                else if (menuTemp.Name == "setResoultNO")  //手动不合格
                {
                    List<string> list = EquipmentData.CheckResults.CheckNodeCurrent.CheckResults[0].GetAllProperyName();
                    string[] arrayResult = new string[EquipmentData.CheckResults.CheckNodeCurrent.CheckResults.Count];
                    for (int k = 0; k < EquipmentData.CheckResults.CheckNodeCurrent.CheckResults.Count; k++)
                    {
                        arrayResult[k] = "不合格";
                    }
                    EquipmentData.CheckResults.UpdateCheckResult(EquipmentData.CheckResults.CheckNodeCurrent.ItemKey, "结论", arrayResult);
                }
                else
                {
                    for (int i = 0; i < EquipmentData.CheckResults.Categories.Count; i++)
                    {
                        SetNodeExpanded(EquipmentData.CheckResults.Categories[i], menuTemp.Name == "menuExpand");
                    }
                }

            }
        }
        private void SetNodeExpanded(CheckNodeViewModel nodeTemp, bool isExpanded)
        {
            if (nodeTemp != null)
            {
                for (int i = 0; i < nodeTemp.Children.Count; i++)
                {

                    nodeTemp.IsExpanded = isExpanded;
                    for (int j = 0; j < nodeTemp.Children.Count; j++)
                    {
                        SetNodeExpanded(nodeTemp.Children[i], isExpanded);
                    }
                }
            }
        }

        /// <summary>
        /// 选中项发生改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeSchema1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            Dispatcher.Invoke(new Action(() =>
            {
                object obj = treeSchema1.SelectedItem;
                if (EquipmentData.Controller.IsChecking)
                {
                    if (obj is CheckNodeViewModel)
                    {
                        if (((CheckNodeViewModel)obj).CheckResults.Count > 0)
                        {
                            EquipmentData.CheckResults.CheckNodeCurrent = (CheckNodeViewModel)obj;
                        }
                    }
                }
                else
                {
                    if (obj is CheckNodeViewModel)
                    {
                        if (((CheckNodeViewModel)obj).CheckResults.Count > 0)
                        {
                            EquipmentData.Controller.Index = EquipmentData.CheckResults.ResultCollection.IndexOf((CheckNodeViewModel)obj);
                        }
                    }
                }
            }));

        }
        /// 更新当前方案
        private void Button_Click_Refresh(object sender, RoutedEventArgs e)
        {

            EquipmentData.SchemaModels.RefreshCurrrentSchema();

        }


    }
}

