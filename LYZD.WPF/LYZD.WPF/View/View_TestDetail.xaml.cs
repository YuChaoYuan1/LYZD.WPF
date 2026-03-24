using LYZD.ViewModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Threading;
using LYZD.ViewModel.CheckInfo;
using LYZD.Utility.Log;
using LYZD.Core.Enum;
using LYZD.ViewModel.Const;
using LYZD.WPF.Converter;
namespace LYZD.WPF.View
{
    /// <summary>
    /// View_TestDetail.xaml 的交互逻辑
    /// </summary>
    public partial class View_TestDetail
    {

        string SetPath = System.IO.Directory.GetCurrentDirectory() + "\\Ini\\SetResoultConfig.ini";
        bool isAllVisible = false;
        List<string> visibleList = new List<string>();
        public View_TestDetail()
        {
            InitializeComponent();
            Name = "详细数据";
            GetSetResooltItemID();
            DataContext = EquipmentData.CheckResults;
            DockStyle.FloatingSize = SystemParameters.WorkArea.Size;
            EquipmentData.CheckResults.PropertyChanged += CheckResults_PropertyChanged;
            treeScheme.Loaded += treeScheme_Loaded;
            comboBoxMeterNo.Loaded += comboBoxMeterNo_Load;
            comboBoxMeterNo.SelectionChanged += ComboBoxMeterNo_SelectionChanged;


            comboBoxSchema.DataContext = EquipmentData.SchemaModels;
            Binding bindingRefresh = new Binding("IsChecking");
            bindingRefresh.Source = EquipmentData.Controller;
            bindingRefresh.Converter = new NotBoolConverter();
            buttonRefresh.SetBinding(IsEnabledProperty, bindingRefresh);
            comboBoxSchema.SetBinding(IsHitTestVisibleProperty, bindingRefresh);
        }
        /// <summary>
        /// 获取需要修改结论的项目id
        /// </summary>
        private void GetSetResooltItemID()
        {
            string str = OperateFile.GetINI("visible", "all", SetPath).Trim();
            if (str.ToLower() == "0")
            {
                isAllVisible = true;
                return;
            }
            for (int i = 0; i < 99; i++)
            {
                str = OperateFile.GetINI("visible", i.ToString(), SetPath).Trim();
                if (str != "")
                {
                    visibleList.Add(str);
                }
            }
        }
        private void ComboBoxMeterNo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EquipmentData.CheckResults.CheckNodeCurrent == null)
            {
                return;
            }
            Dispatcher.Invoke(new Action(() =>
            {
                if (comboBoxMeterNo.SelectedIndex < 0 || comboBoxMeterNo.SelectedIndex > EquipmentData.CheckResults.CheckNodeCurrent.CheckResults.Count)
                {
                    return;
                }


                DynamicViewModel model = EquipmentData.CheckResults.CheckNodeCurrent.CheckResults[comboBoxMeterNo.SelectedIndex];
                DataGridItemResoult.ItemsSource = model.ItemResoult;
            }));
        }

        void comboBoxMeterNo_Load(object sender, RoutedEventArgs e)
        {
            comboBoxMeterNo.Items.Clear();

            for (int i = 1; i <= EquipmentData.Equipment.MeterCount; i++)
            {
                comboBoxMeterNo.Items.Add("表位" + i);
            }
            if (comboBoxMeterNo.Items.Count > 0)
            {
                comboBoxMeterNo.SelectedIndex = 0;
            }
        }

        void treeScheme_Loaded(object sender, RoutedEventArgs e)
        {
            if (EquipmentData.CheckResults.CheckNodeCurrent == null)
            {
                return;
            }
            if (EquipmentData.CheckResults.CheckNodeCurrent.Level == 1)
            {
                TreeViewItem treeItem = treeScheme.ItemContainerGenerator.ContainerFromItem(EquipmentData.CheckResults.CheckNodeCurrent.Parent) as TreeViewItem;
                if (treeItem != null)
                {
                    treeItem.IsExpanded = true;
                }
            }
            ReloadColumn();
        }

        void CheckResults_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FlagLoadColumn" && !EquipmentData.CheckResults.FlagLoadColumn)
            {
                ReloadColumn();
            }
            if (e.PropertyName == "CheckNodeCurrent")
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    CheckNodeViewModel nodeSelected = treeScheme.SelectedItem as CheckNodeViewModel;
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
                        TreeViewItem treeItem = treeScheme.ItemContainerGenerator.ContainerFromItem(nodeList[nodeList.Count - 1]) as TreeViewItem;
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
        }
        private void RefItemResoult()
        {
            //DataGridItemResoult.Items.Clear();

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


        /// 加载检定点列
        /// <summary>
        /// 加载检定点列
        /// </summary>
        public void ReloadColumn()
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    while (dataGridCheck.Columns.Count > 1)
                    {
                        BindingOperations.ClearAllBindings(dataGridCheck.Columns[1]);
                        dataGridCheck.Columns.Remove(dataGridCheck.Columns[1]);
                    }
                    List<string> columnNames = EquipmentData.CheckResults.CheckNodeCurrent.CheckResults[0].GetAllProperyName();
                    setReoultMenu.Visibility = Visibility.Collapsed;
                    if (isAllVisible) setReoultMenu.Visibility = Visibility.Visible;
                    else
                    {
                        if (visibleList.Contains(EquipmentData.CheckResults.CheckNodeCurrent.ParaNo))
                        {
                            setReoultMenu.Visibility = Visibility.Visible;
                        }
                    }

                    if (columnNames.Find(item => item.IndexOf("分项结论") != -1) == null)
                    {
                        ItemTabPage.Visibility = Visibility.Collapsed;
                        Tab1.SelectedIndex = 0;
                    }
                    else
                    {
                        ItemTabPage.Visibility = Visibility.Visible;

                    }
                    columnNames = GetVisibleColumnName(columnNames);
                    double widthTemp = dataGridCheck.ActualWidth;
                    double columnWidth = 100;

                    if (columnNames.Count > 2)
                    {
                        columnWidth = (widthTemp - 100) / (columnNames.Count - 1 - columnNames.FindAll(item => item.IndexOf("分项结论") != -1).Count);
                    }
                    if (columnWidth < 70)
                    {
                        columnWidth = 70;
                    }
                    for (int i = 0; i < columnNames.Count; i++)
                    {
                        //if (columnNames[i] == "要检")
                        //{
                        //    DataGridCheckBoxColumn col = new DataGridCheckBoxColumn();
                        //    col.Header = "改变";
                        //    col.Width = new DataGridLength(columnWidth);
                        //    col.MinWidth = 50;
                        //    col.IsReadOnly = false;
                        //    dataGridCheck.Columns.Add(col);
                        //    continue;
                        //}

                        if (columnNames[i] == "要检" || columnNames[i] == "项目名" || columnNames[i] == "分项结论376" || columnNames[i] == "分项结论698")
                        {
                            continue;
                        }

                        DataGridTextColumn column = new DataGridTextColumn
                        {
                            Header = columnNames[i],
                            Binding = new Binding(columnNames[i]),
                            Width = new DataGridLength(columnWidth),
                            MinWidth = 50
                        };
                        dataGridCheck.Columns.Add(column);
                    };



                    //2列的情况
                    /*                if (dataGridCheck.Columns.Count == 3)
                                    {
                                        if (dataGridCheck.Columns[1].Header.ToString() == "检定数据" && dataGridCheck.Columns[2].Header.ToString() == "结论")
                                        {
                                            dataGridCheck.Columns[1].Width = new DataGridLength(columnWidth * 2 - 100);
                                            dataGridCheck.Columns[2].Width = 100;
                                        }
                                    }*/
                }));
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(string.Format("控件加载异常:{0}", ex.Message), EnumLogSource.用户操作日志, EnumLevel.Warning, ex);
            }
            try
            {
                ComboBoxMeterNo_SelectionChanged(null, null);
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(string.Format("控件加载异常2:{0}", ex.Message), EnumLogSource.用户操作日志, EnumLevel.Warning, ex);

            }
        }

        /// <summary>
        /// 获得需要显示的列名
        /// </summary>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        private List<string> GetVisibleColumnName(List<string> columnNames)
        {

            //TODO 设置显示协议对应的数据
            //这里需要给每个设定一个特殊标识-376/698/104--特殊标识来区分属于那个协议的详细数据
            int index_376_start = columnNames.FindIndex(item => item.IndexOf(ItemResoultkeyword.splite_Statr_376) != -1);
            int index_376_end = columnNames.FindIndex(item => item.IndexOf(ItemResoultkeyword.splite_End_376) != -1);
            int index_698_start = columnNames.FindIndex(item => item.IndexOf(ItemResoultkeyword.splite_Statr_698) != -1);
            int index_698_end = columnNames.FindIndex(item => item.IndexOf(ItemResoultkeyword.splite_End_698) != -1);
            //int index_104_start = columnNames.FindIndex(item => item.IndexOf("<104") != -1);
            //int index_104_end = columnNames.FindIndex(item => item.IndexOf("104>") != -1);

            int index = -1;
            if (index_376_start != -1 && index_698_start != -1)
            {
                index = Math.Min(index_376_start, index_698_start);
            }
            else if (index_376_start != -1)
            {
                index = index_376_start;
            }
            else if (index_698_start != -1)
            {
                index = index_698_start;
            }

            if (index == -1) //没有特殊协议的部分
            {
                return columnNames;
            }

            List<string> name = columnNames;
            List<string> name_698 = new List<string>();
            List<string> name_376 = new List<string>();
            List<string> test = ItemResoultkeyword.GetSpliteList();


            if (index_698_start != -1 & index_698_end != -1)   //获取698特殊检定数据部分
            {
                for (int i = index_698_start + 1; i < index_698_end; i++)
                {
                    name_698.Add(columnNames[i]);
                }
            }
            if (index_376_start != -1 & index_376_start != -1) //获取376特殊检定数据部分
            {
                for (int i = index_376_start + 1; i < index_376_end; i++)
                {
                    name_376.Add(columnNames[i]);
                }
            }


            if (EquipmentData.Equipment.TerminalProtocolType == TerminalProtocolTypeEnum._698)
            {
                name.RemoveAll(item => name_376.Contains(item));  //删除698以外的
            }
            else if (EquipmentData.Equipment.TerminalProtocolType == TerminalProtocolTypeEnum._376)
            {
                name.RemoveAll(item => name_698.Contains(item));
            }

            name.RemoveAll(item => test.Contains(item));
            return name;
        }

        public sealed override void Dispose()
        {
            //清除绑定
            dataGridCheck.Columns.Clear();
            decorator1.Target = null;
            treeScheme.DataContext = null;
            treeScheme.SelectedItemChanged -= treeScheme_SelectionChanged;
            EquipmentData.CheckResults.PropertyChanged -= CheckResults_PropertyChanged;
            comboBoxMeterNo.Loaded -= comboBoxMeterNo_Load;
            comboBoxMeterNo.SelectionChanged -= ComboBoxMeterNo_SelectionChanged;
            DataContext = null;
            base.Dispose();
        }

        private void treeScheme_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                object obj = treeScheme.SelectedItem;
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuTemp = e.OriginalSource as MenuItem;
            if (menuTemp == null) return;
            Windows.Window_TestDataSet window_TestData = new Windows.Window_TestDataSet();
            window_TestData.ShowDialog();
        }

        private void Button_Click_Refresh(object sender, RoutedEventArgs e)
        {
            EquipmentData.SchemaModels.RefreshCurrrentSchema();
        }
    }
}

