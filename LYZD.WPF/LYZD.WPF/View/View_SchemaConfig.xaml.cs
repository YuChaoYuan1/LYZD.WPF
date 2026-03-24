using LYZD.Core.Enum;
using LYZD.Utility.Log;
using LYZD.ViewModel;
using LYZD.ViewModel.Schema;
using LYZD.ViewModel.Schema.Error;
using LYZD.WPF.Model;
using LYZD.WPF.Schema;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;

namespace LYZD.WPF.View
{
    /// <summary>
    /// View_SchemaConfig.xaml 的交互逻辑
    /// </summary>
    public partial class View_SchemaConfig 
    {
        public View_SchemaConfig()
        {
            InitializeComponent();
            Name = "方案管理";
            DockStyle.IsFloating = true; //是否开始是全屏  
            DockStyle.IsMaximized = true;
            DockStyle.FloatingSize = new Size(SystemParameters.WorkArea.Width, SystemParameters.WorkArea.Height);
            treeFramework.ItemsSource = FullTree.Instance.Children;
            gridSchemas.DataContext = EquipmentData.SchemaModels;//方案列表
            comboBoxSchemas.SelectionChanged += ComboBoxSchemas_SelectionChanged;//选中方案改变触发事件
            if (EquipmentData.SchemaModels.SelectedSchema != null)
            {
                comboBoxSchemas.SelectedItem = EquipmentData.SchemaModels.SelectedSchema;
                viewModel.LoadSchema((int)EquipmentData.SchemaModels.SelectedSchema.GetProperty("ID"));
            }

            controlError.PointsChanged += controlEror_PointsChanged;
            controlError.AllPoints.PropertyChanged += AllPoints_PropertyChanged;

            checkBoxErrorView.Checked += CheckBoxErrorView_Checked;
            checkBoxErrorView.Unchecked += CheckBoxErrorView_Checked;

        }

        private SchemaViewModel viewModel
        {
            get { return Resources["SchemaViewModel"] as SchemaViewModel; }
        }

        private DynamicViewModel currentSchema
        {
            get { return comboBoxSchemas.SelectedItem as DynamicViewModel; }
        }

        void controlEror_PointsChanged(object sender, System.EventArgs e)
        {
            ErrorModel model = sender as ErrorModel;
            if (model is ErrorModel)
            {
                viewModel.UpdateErrorPoint(model);
            }
        }

       #region 用户事件
        //private void ButtonParaInfo_Click(object sender, RoutedEventArgs e)
        //{
        //    Button button = sender as Button;
        //    if (button == null) return;
        //    viewModel.ParaInfo.CommandFactoryMethod(button.Name);
        //}

        public override void Dispose()
        {
            controlError.PointsChanged -= controlEror_PointsChanged;
            checkBoxErrorView.Checked -= CheckBoxErrorView_Checked;
            checkBoxErrorView.Unchecked -= CheckBoxErrorView_Checked;
            comboBoxSchemas.SelectionChanged -= ComboBoxSchemas_SelectionChanged;
            dataGridGeneral.DataContext = null;
            dataGridGeneral.Columns.Clear();
            dataGridGeneral.ItemsSource = null;
            base.Dispose();
        }

        private void Button_Click_RemoveNode(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                SchemaNodeViewModel node = button.DataContext as SchemaNodeViewModel;
                if (node.Level == 1)
                {
                    viewModel.Children.Remove(node);
                }
                else
                {
                    node.ParentNode.Children.Remove(node);
                }
                viewModel.RefreshPointCount();
            }
        }

        //  object temCurrentNode2 = null;

        private void treeSchema_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SchemaNodeViewModel currentNode = treeSchema.SelectedItem as SchemaNodeViewModel;
            if (currentNode == null)
            {
                return;
            }

            // temCurrentNode2 =  treeSchema.SelectedItem;

            if (currentNode.Children.Count == 0)
            {
                //切换检定点时保存(不保存到数据库)
                if (viewModel.SelectedNode != null)
                {
                    viewModel.SelectedNode.ParaValuesCurrent = viewModel.ParaValuesConvertBack();
                }
                viewModel.SelectedNode = currentNode;
                viewModel.ParaNo = currentNode.ParaNo;
            }

            //12001:基本误差
            if (viewModel.ParaNo == ProjectID.基本误差试验 ||viewModel.ParaNo == ProjectID.初始固有误差)
            {
                checkBoxErrorView.IsChecked = true;
                checkBoxErrorView.Visibility = Visibility.Visible;
                gridGeneral.Visibility = Visibility.Collapsed;
                scrollViewError.Visibility = Visibility.Visible;
                controlError.AllPoints.Load(viewModel.SelectedNode.ParaValuesCurrent);
            }
            else
            {
                checkBoxErrorView.IsChecked = true;
                checkBoxErrorView.Visibility = Visibility.Collapsed;
                gridGeneral.Visibility = Visibility.Visible;
                scrollViewError.Visibility = Visibility.Collapsed;
            }
            //if (viewModel.ParaNo == ProjectID.初始固有误差)
            //{
            //    checkBoxErrorView.IsChecked = true;
            //    checkBoxErrorView.Visibility = Visibility.Visible;
            //    gridGeneral.Visibility = Visibility.Collapsed;
            //    scrollViewInitialError.Visibility = Visibility.Visible;
            //    controlInitialError.AllPoints.Load(viewModel.SelectedNode.ParaValuesCurrent);
            //}
            //else
            //{
            //    checkBoxErrorView.IsChecked = true;
            //    checkBoxErrorView.Visibility = Visibility.Collapsed;
            //    gridGeneral.Visibility = Visibility.Visible;
            //    scrollViewInitialError.Visibility = Visibility.Collapsed;
            //}

            //if (viewModel.ParaNo == "17003" || viewModel.ParaNo == "17001")
            //{
            //    ProtocolPanel.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    ProtocolPanel.Visibility = Visibility.Collapsed;
            //}

        }



        private void Button_Click_RemoveItem(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                DynamicViewModel modelTemp = button.DataContext as DynamicViewModel;
                if (modelTemp != null)
                {
                    viewModel.ParaValuesView.Remove(modelTemp);
                    if (viewModel.ParaValuesView.Count == 0)
                    {
                        if (viewModel.SelectedNode.Level == 1)
                        {
                            viewModel.Children.Remove(viewModel.SelectedNode);
                        }
                        else
                        {
                            viewModel.SelectedNode.ParentNode.Children.Remove(viewModel.SelectedNode);
                        }
                        viewModel.RefreshPointCount();
                        return;
                    }
                    else
                    {
                        viewModel.SelectedNode.ParaValuesCurrent = viewModel.ParaValuesConvertBack();
                        viewModel.RefreshPointCount();
                    }
                }
            }
        }

        private void Button_Click_AddItem(object sender, RoutedEventArgs e)
        {
            viewModel.AddNewParaValue();
            viewModel.SelectedNode.ParaValuesCurrent = viewModel.ParaValuesConvertBack();
            viewModel.RefreshPointCount();
        }

        private void ButtonClick_AddNode(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            if (button == null) return;
            if (button.Name != "buttonAdd") return;
            SchemaNodeViewModel nodeTemp = button.DataContext as SchemaNodeViewModel;
            if (nodeTemp == null) return;
            List<SchemaNodeViewModel> nodeList = new List<SchemaNodeViewModel>();
            if (nodeTemp.IsTerminal)
            {
                nodeList.Add(nodeTemp);
            }
            else
            {
                nodeList = nodeTemp.GetTerminalNodes();
            }
            List<string> namesList = new List<string>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                string noTemp = nodeList[i].ParaNo;
                if (viewModel.ExistNode(noTemp))
                {
                    namesList.Add(nodeList[i].Name);
                    continue;
                }
                SchemaNodeViewModel nodeNew = viewModel.AddParaNode(noTemp);

                if (i == nodeList.Count - 1)
                {
                    SelectNode(nodeNew);
                }
            }
            if (namesList.Count > 0)
            {
                LogManager.AddMessage(string.Format("检定点:{0}已存在,将不会重复添加!", string.Join(",", namesList)), EnumLogSource.用户操作日志, EnumLevel.Tip);
            }
        }

        private void SelectNode(SchemaNodeViewModel nodeTemp)
        {
            if (nodeTemp != null)
            {
                List<SchemaNodeViewModel> nodesList = new List<SchemaNodeViewModel>();
                #region 获取链
                nodesList.Add(nodeTemp);
                SchemaNodeViewModel nodeParentTemp = nodeTemp.ParentNode;
                while (nodeParentTemp != null && nodeParentTemp.Level >= 1)
                {
                    if (nodeParentTemp != null)
                    {
                        nodesList.Add(nodeParentTemp);
                    }
                    nodeParentTemp = nodeParentTemp.ParentNode;
                    if (nodeParentTemp == null)
                    {
                        break;
                    }
                }
                #endregion

                nodesList = OrderByListChildren(nodesList);

                TreeViewItem treeItem = treeSchema.ItemContainerGenerator.ContainerFromItem(nodesList[nodesList.Count - 1]) as TreeViewItem;
                if (treeItem == null)
                {
                    return;
                }
                else
                {
                    treeItem.IsExpanded = true;
                    treeItem.IsSelected = true;
                    treeItem.BringIntoView();
                }
                for (int i = nodesList.Count - 2; i >= 0; i--)
                {
                    treeItem = treeItem.ItemContainerGenerator.ContainerFromItem(nodesList[i]) as TreeViewItem;
                    if (treeItem == null)
                    {
                        return;
                    }
                    else
                    {
                        treeItem.IsExpanded = true;
                        treeItem.IsSelected = true;
                        treeItem.BringIntoView();
                    }
                }
            }
        }

        /// <summary>
        /// 方案排序
        /// </summary>
        /// <param name="ViewModel"></param>
        /// <returns></returns>
        public List<SchemaNodeViewModel> OrderByListChildren(List<SchemaNodeViewModel> ViewModel)
        {
            List<SchemaNodeViewModel> ViewModelList = new List<SchemaNodeViewModel>();
            SchemaNodeViewModel ViewModeTmp = new SchemaNodeViewModel();

         //string s=   DAL.SchemaFramework.GetSortNo(ViewModel[0].ParaNo);
            //DynamicModel modelTemp = SchemaFramework.GetParaFormat(ViewModel.ParaNo);
            //if (ViewModel == null) return ViewModelList;

            for (int i = 0; i < ViewModel.Count; i++)
            {
                if (ViewModel[i].ParaNo==null)
                {
                    ViewModel[i].ParaNo= DAL.SchemaFramework.GetSortNo(ViewModel[i].ParaNo);
                }
                for (int j =0; j < ViewModel[i].Children.Count; j++)
                {
                    if (ViewModel[i].Children[j].SortNo == null)
                    {
                        ViewModel[i].Children[j].SortNo = DAL.SchemaFramework.GetSortNo(ViewModel[i].Children[j].ParaNo);
                    }
                }
            }



            for (int k = 0; k < ViewModel.Count; k++)
            {
                for (int i = ViewModel[k].Children.Count; i > 0; i--)
                {
                    for (int j = 0; j < i - 1; j++)
                    {
                        if (ViewModel[k].Children[j].SortNo==null|| ViewModel[k].Children[j].SortNo == "")
                        {
                            continue;
                        }

                        if (int.Parse(ViewModel[k].Children[j].SortNo) > int.Parse(ViewModel[k].Children[j + 1].SortNo))
                        {
                            ViewModeTmp = ViewModel[k].Children[j];
                            ViewModel[k].Children[j] = ViewModel[k].Children[j + 1];
                            ViewModel[k].Children[j + 1] = ViewModeTmp;
                        }
                        if (ViewModel[k].Children[j].Children.Count > 0)
                        {
                            for (int p = 0; p < ViewModel[k].Children[j].Children.Count - 1; p++)
                            {
                                if (int.Parse(ViewModel[k].Children[j].Children[p].SortNo) > int.Parse(ViewModel[k].Children[j].Children[p + 1].SortNo))
                                {
                                    ViewModeTmp = ViewModel[k].Children[j].Children[p];
                                    ViewModel[k].Children[j].Children[p] = ViewModel[k].Children[j].Children[p + 1];
                                    ViewModel[k].Children[j].Children[p + 1] = ViewModeTmp;
                                }
                            }
                        }

                    }
                }
            }


            //for (int k = 0; k < ViewModel.Count; k++)
            //{
            //    for (int i = ViewModel[k].Children.Count; i > 0; i--)
            //    {
            //        for (int j = 0; j < i - 1; j++)
            //        {
            //            if (int.Parse(ViewModel[k].Children[j].ParaNo) > int.Parse(ViewModel[k].Children[j + 1].ParaNo))
            //            {
            //                ViewModeTmp = ViewModel[k].Children[j];
            //                ViewModel[k].Children[j] = ViewModel[k].Children[j + 1];
            //                ViewModel[k].Children[j + 1] = ViewModeTmp;
            //            }
            //            if (ViewModel[k].Children[j].Children.Count > 0)
            //            {
            //                for (int p = 0; p < ViewModel[k].Children[j].Children.Count - 1; p++)
            //                {
            //                    if (int.Parse(ViewModel[k].Children[j].Children[p].ParaNo) > int.Parse(ViewModel[k].Children[j].Children[p + 1].ParaNo))
            //                    {
            //                        ViewModeTmp = ViewModel[k].Children[j].Children[p];
            //                        ViewModel[k].Children[j].Children[p] = ViewModel[k].Children[j].Children[p + 1];
            //                        ViewModel[k].Children[j].Children[p + 1] = ViewModeTmp;
            //                    }
            //                }
            //            }

            //        }
            //    }
            //}

            return ViewModelList = ViewModel;

        }

        private void buttonClick_Save(object sender, RoutedEventArgs e)
        {
            viewModel.SaveParaValue();
        }


        private void SchemaDown(object sender, RoutedEventArgs e)
        {
            //model = new EquipmentData.Schema();
            //传入方案编号，及参数值，创建方案
            test();
            return;

            EquipmentData.SchemaModels.NewName = "创建方案名称测试";
            EquipmentData.SchemaModels.AddSchema(); //这个需要重写一个方法，刷新方案放在背后，并且把选中等方法添加到里面
            DynamicViewModel modelTemp = EquipmentData.SchemaModels.Schemas.FirstOrDefault(item => item.GetProperty("SCHEMA_NAME") as string == EquipmentData.SchemaModels.NewName);
            EquipmentData.SchemaModels.SelectedSchema = modelTemp;
            System.Threading.Thread.Sleep(1000);
            if (!EquipmentData.Schema.ExistNode("18001"))
            {
                // SchemaNodeViewModel nodeNew = 
                EquipmentData.Schema.AddParaNode("18001");//根据方案的编号，添加进了节点
                EquipmentData.Schema.ParaValuesView.Clear();//删除默认值的方案
            }
            //SchemaNodeViewModel nodeNew =
            //EquipmentData.Schema.ParaValuesView.Clear();

            List<string> propertyNames = new List<string>();
            for (int i = 0; i < EquipmentData.Schema.ParaInfo.CheckParas.Count; i++)
            {
                propertyNames.Add(EquipmentData.Schema.ParaInfo.CheckParas[i].ParaDisplayName);
            }

            for (int j = 0; j< 2; j++)
            {
                DynamicViewModel viewModel2 = new DynamicViewModel(propertyNames, 0);
                viewModel2.SetProperty("IsSelected", true);

                for (int i = 0; i < propertyNames.Count; i++)
                {
                    viewModel2.SetProperty(propertyNames[i], EquipmentData.Schema.ParaInfo.CheckParas[i].DefaultValue); //这里改成参数的值
                }
                viewModel2.SetProperty("功率因素", "0.5L");

                EquipmentData.Schema.ParaValuesView.Add(viewModel2);
            }
            viewModel.RefreshPointCount();
            EquipmentData.Schema.SaveParaValue();    //保存方案
            EquipmentData.SchemaModels.RefreshCurrrentSchema();

        }


        private void test()
        {
            EquipmentData.SchemaModels.NewName = "创建方案名称测试";
            EquipmentData.SchemaModels.AddDownSchema(); //这个需要重写一个方法，刷新方案放在背后，并且把选中等方法添加到里面
            DynamicViewModel modelTemp = EquipmentData.SchemaModels.Schemas.FirstOrDefault(item => item.GetProperty("SCHEMA_NAME") as string == EquipmentData.SchemaModels.NewName);
            EquipmentData.SchemaModels.SelectedSchema = modelTemp;
            //System.Threading.Thread.Sleep(1000);
            string[] keys = new string[] {"12001","12002" };
            string[] str = new string[] { "正向有功", "正向无功" , "反向有功" , "反向无功" };
            foreach (var key in keys)
            {
                if (!EquipmentData.Schema.ExistNode(key))
                {
                    SchemaNodeViewModel nodeNew = EquipmentData.Schema.AddParaNode(key);//根据方案的编号，添加进了节点
                    EquipmentData.Schema.ParaValuesView.Clear();//删除默认值的方案
                }
                List<string> propertyNames = new List<string>();

                for (int j = 0; j < EquipmentData.Schema.ParaInfo.CheckParas.Count; j++)
                {
                    propertyNames.Add(EquipmentData.Schema.ParaInfo.CheckParas[j].ParaDisplayName);
                }

                for (int i = 0; i <4; i++)
                {
                    DynamicViewModel viewModel2 = new DynamicViewModel(propertyNames, 0);
                    viewModel2.SetProperty("IsSelected", true);
                    for (int j = 0; j < propertyNames.Count; j++)
                    {
                        viewModel2.SetProperty(propertyNames[j], EquipmentData.Schema.ParaInfo.CheckParas[j].DefaultValue); //这里改成参数的值
                    }
                    if (key=="12001")
                    {
                        viewModel2.SetProperty(propertyNames[1], str[i]); //这里改成参数的值
                    }
                    EquipmentData.Schema.ParaValuesView.Add(viewModel2);
                }
                EquipmentData.Schema.SaveParaValue();    //保存方案
                EquipmentData.Schema.RefreshPointCount();
            }
          
            EquipmentData.SchemaModels.RefreshCurrrentSchema();
        }
        private void buttonClick_SortDefault(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定要默认排序吗?", "默认排序", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                viewModel.SortDefault();
            }
        }

        private void Event_NodeMove(object sender, DragEventArgs e)
        {
            SchemaNodeViewModel nodeSource = e.Data.GetData(typeof(SchemaNodeViewModel)) as SchemaNodeViewModel;
            if (nodeSource == null)
            { return; }
            Point pos = e.GetPosition(treeSchema);
            HitTestResult result = VisualTreeHelper.HitTest(treeSchema, pos);
            if (result == null)
                return;

            TreeViewItem selectedItem = Utils.FindVisualParent<TreeViewItem>(result.VisualHit);
            if (selectedItem == null)
            {
                return;
            }
            SchemaNodeViewModel nodeDest = selectedItem.DataContext as SchemaNodeViewModel;
            if (nodeDest == null)
            {
                return;
            }
            if (nodeDest == nodeSource)
            {
                return;
            }
            viewModel.MoveNode(nodeSource, nodeDest);
        }

        private void Button_Click_ItemUp(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;
            DynamicViewModel modelTemp = button.DataContext as DynamicViewModel;
            if (modelTemp == null)
            {
                return;
            }
            int index = viewModel.ParaValuesView.IndexOf(modelTemp);
            if (index > 0)
            {
                viewModel.ParaValuesView.Move(index, index - 1);
            }
            viewModel.SelectedNode.ParaValuesCurrent = viewModel.ParaValuesConvertBack();
        }

        private void Button_Click_ItemDown(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;
            DynamicViewModel modelTemp = button.DataContext as DynamicViewModel;
            if (modelTemp == null)
            {
                return;
            }
            int index = viewModel.ParaValuesView.IndexOf(modelTemp);
            if (index < viewModel.ParaValuesView.Count - 1)
            {
                viewModel.ParaValuesView.Move(index, index + 1);
            }
            viewModel.SelectedNode.ParaValuesCurrent = viewModel.ParaValuesConvertBack();
        }

        private void CheckBoxErrorView_Checked(object sender, RoutedEventArgs e)
        {
            if (viewModel.ParaNo != ProjectID.基本误差试验 && viewModel.ParaNo != ProjectID.初始固有误差)
            {
                return;
            }
            if (checkBoxErrorView.IsChecked.HasValue && checkBoxErrorView.IsChecked.Value)
            {
                gridGeneral.Visibility = Visibility.Collapsed;
                scrollViewError.Visibility = Visibility.Visible;
            }
            else
            {
                gridGeneral.Visibility = Visibility.Visible;
                scrollViewError.Visibility = Visibility.Collapsed;
            }
        }

        private void ComboBoxSchemas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentSchema != null)
            {
                viewModel.LoadSchema((int)currentSchema.GetProperty("ID"));
                tipsSchemeName.Text = currentSchema.GetProperty("SCHEMA_NAME").ToString();

            }


        }
        #endregion

        private void Click_SchemaOperation(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            if (button != null)
            {
                try
                {
                    switch (button.Name)
                    {
                        case "buttonNew":
                            MainViewModel.Instance.CommandFactoryMethod("新建方案|View_SchemaOperation|新建方案");
                            break;
                        case "buttonDelete":
                            MainViewModel.Instance.CommandFactoryMethod("删除方案|View_SchemaOperation|删除方案");
                            break;
                        case "buttonRename":
                            MainViewModel.Instance.CommandFactoryMethod("重命名方案|View_SchemaOperation|重命名方案");
                            break;
                        case "buttonCopy":
                            MainViewModel.Instance.CommandFactoryMethod("复制方案|View_SchemaOperation|复制方案");
                            break;
                    }
                }
                catch
                { }
            }
        }


        private XmlNode nodeDataFlags = null;

        /// <summary>
        /// 通信协议检查加载数据标识
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridGeneral_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (viewModel.ParaInfo==null)
            {
                return;
            }

            //为了降低损耗,加多一次判断
            List<string> list = new List<string>() { ProjectID.功率因素基本误差 };
            if (!list.Contains(viewModel.ParaInfo.ParaNo))  return;

            DataGridComboBoxColumn column = e.Column as DataGridComboBoxColumn;
            if (column == null)  return;
            Binding bindingColumn = column.SelectedItemBinding as Binding;
            if (bindingColumn == null) return;

            string pathTemp = bindingColumn.Path.Path;

            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            ComboBox comboBox = e.EditingElement as ComboBox;
            if (comboBox.SelectedItem == null)
            {
                return;
            }
            string value = comboBox.SelectedItem.ToString();
            switch (viewModel.ParaInfo.ParaNo)
            {
                //case ProjectID.电压基本误差 :
                //    if (pathTemp == "电压角度")
                //    {
                //        keyValues.Add("ΦUa", GetAngle(0, int.Parse(value)).ToString());
                //        keyValues.Add("ΦUb", GetAngle(240, int.Parse(value)).ToString());
                //        keyValues.Add("ΦUc", GetAngle(120, int.Parse(value)).ToString());
                //    }
                //    break;
                //case ProjectID.有功功率基本误差:
                //case ProjectID.无功功率基本误差:
                //    if (pathTemp == "电流角度")
                //    {
                //        keyValues.Add("ΦIa", GetAngle(0, int.Parse(value)).ToString());
                //        keyValues.Add("ΦIb", GetAngle(240, int.Parse(value)).ToString());
                //        keyValues.Add("ΦIc", GetAngle(120, int.Parse(value)).ToString());
                //    }
                //    break;
                case ProjectID.功率因素基本误差:
                    if (pathTemp == "功率角度")
                    {
                        keyValues.Add("ΦIa", GetAngle(0, int.Parse(value)).ToString());
                        keyValues.Add("ΦIb", GetAngle(240, int.Parse(value)).ToString());
                        keyValues.Add("ΦIc", GetAngle(120, int.Parse(value)).ToString());
                    }
                    break;
                default:
                    break;
            }

            if (keyValues.Count>0)
            {
                BindingExpression expressionTemp = e.EditingElement.GetBindingExpression(ComboBox.SelectedItemProperty);
                DynamicViewModel modelTemp = expressionTemp.DataItem as DynamicViewModel;
                foreach (var key in keyValues.Keys)
                {
                    modelTemp.SetProperty(key, keyValues[key]);
                }
            }

            //if (viewModel.ParaInfo != null && viewModel.ParaInfo.ParaNo == "17001")
            //{
            //    DataGridComboBoxColumn column = e.Column as DataGridComboBoxColumn;
            //    if (column == null)
            //    {
            //        return;
            //    }
            //    Binding bindingColumn = column.SelectedItemBinding as Binding;
            //    if (bindingColumn != null)
            //    {
            //        string pathTemp = bindingColumn.Path.Path;
            //        if (pathTemp == "数据项名称")
            //        {
            //            BindingExpression expressionTemp = e.EditingElement.GetBindingExpression(ComboBox.SelectedItemProperty);
            //            DynamicViewModel modelTemp = expressionTemp.DataItem as DynamicViewModel;
            //            //加载数据标识内容
            //            if (modelTemp != null)
            //            {
            //                if (nodeDataFlags == null)
            //                {
            //                    XmlDocument doc = new XmlDocument();
            //                    doc.Load(string.Format(@"{0}\xml\DataFlag.xml", Directory.GetCurrentDirectory()));
            //                    nodeDataFlags = doc.DocumentElement;
            //                }
            //                foreach (XmlNode nodeTemp in nodeDataFlags.ChildNodes)
            //                {
            //                    try
            //                    {
            //                        ComboBox comboBox = e.EditingElement as ComboBox;
            //                        if (nodeTemp.Attributes["DataFlagName"].Value == comboBox.SelectedItem.ToString())
            //                        {
            //                            modelTemp.SetProperty("标识编码", nodeTemp.Attributes["DataFlag"].Value);
            //                            //modelTemp.SetProperty("标识编码698", nodeTemp.Attributes["DataFlag698"].Value);
            //                            modelTemp.SetProperty("长度", nodeTemp.Attributes["DataLength"].Value);
            //                            modelTemp.SetProperty("小数位", nodeTemp.Attributes["DataSmallNumber"].Value);
            //                            modelTemp.SetProperty("数据格式", nodeTemp.Attributes["DataFormat"].Value);
            //                            //modelTemp.SetProperty("写入内容", nodeTemp.Attributes["ReadData"].Value);
            //                            //modelTemp.SetProperty("写入数据示例", nodeTemp.Attributes["Default"].Value);
            //                            if (nodeTemp.Attributes["ReadData"] != null)
            //                            {
            //                                modelTemp.SetProperty("写入内容", nodeTemp.Attributes["ReadData"].Value);
            //                            }
            //                            return;
            //                        }
            //                    }
            //                    catch
            //                    { }
            //                }
            //            }
            //        }
            //    }
            //}
        }
        /// <summary>
        /// 获取角度。A1-A2
        /// </summary>
        /// <param name="Angle1"></param>
        /// <param name="Angle2"></param>
        /// <returns></returns>
        private int GetAngle(int Angle1,int Angle2)
        {
            int A = Angle1 - Angle2;
            if (A < 0) A += 360;
            if (A>360) A %= 360;
            return A;
        }

        private void AllPoints_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (viewModel.SelectedNode != null && (viewModel.SelectedNode.ParaNo == ProjectID.基本误差试验 ))
            {
                ViewModel.Model.AsyncObservableCollection<DynamicViewModel> viewModelsTemp = viewModel.ParaValuesView;
                foreach (DynamicViewModel modelTemp in viewModelsTemp)
                {
                    if (e.PropertyName == "LapCountIb")     //相对于Ib圈数
                    {
                        modelTemp.SetProperty("误差圈数(Ib)", controlError.AllPoints.LapCountIb);
                    }
                    else if (e.PropertyName == "GuichengMulti")      //规程误差限倍数
                    {
                        modelTemp.SetProperty("误差限倍数(%)", controlError.AllPoints.GuichengMulti);
                    }
                }
            }
        }

        private void comboBoxSchemas_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show(comboBoxSchemas.Items[0].ToString());
        }

        /// <summary>
        /// 添加通讯协议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddProtocol(object sender, RoutedEventArgs e)
        {

            if (Protocol_Name.Text.Trim() != "") //
            {
                if (nodeDataFlags == null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(string.Format(@"{0}\xml\DataFlag.xml", Directory.GetCurrentDirectory()));
                    nodeDataFlags = doc.DocumentElement;
                }
                foreach (XmlNode nodeTemp in nodeDataFlags.ChildNodes)
                {
                    try
                    {
                        if (nodeTemp.Attributes["DataFlagName"].Value == Protocol_Name.Text.Trim())
                        {

                            viewModel.AddNewParaValue();
                            viewModel.SelectedNode.ParaValuesCurrent = viewModel.ParaValuesConvertBack();
                            viewModel.RefreshPointCount();

                            SchemaNodeViewModel currentNode = treeSchema.SelectedItem as SchemaNodeViewModel;
                            int Index = 1;
                            if (currentNode != null)
                            {
                                foreach (var item in currentNode.ParaValuesCurrent)
                                {
                                    if (item.GetProperty("PARA_VALUE").ToString().Split('|')[0] == Protocol_Name.Text.Trim())
                                    {
                                        Index++;
                                    }
                                }
                            }
                            DynamicViewModel modelTemp = viewModel.ParaValuesView[viewModel.ParaValuesView.Count - 1];
                            modelTemp.SetProperty("数据项名称", nodeTemp.Attributes["DataFlagName"].Value);
                            modelTemp.SetProperty("标识编码", nodeTemp.Attributes["DataFlag"].Value);
                            if (viewModel.ParaInfo.ParaNo=="17003")
                            {
                                modelTemp.SetProperty("标识编码698", nodeTemp.Attributes["DataFlag698"].Value);

                            }
                            modelTemp.SetProperty("长度", nodeTemp.Attributes["DataLength"].Value);
                            modelTemp.SetProperty("小数位", nodeTemp.Attributes["DataSmallNumber"].Value);
                            modelTemp.SetProperty("数据格式", nodeTemp.Attributes["DataFormat"].Value);
                            if (nodeTemp.Attributes["ReadData"] != null)
                            {
                                modelTemp.SetProperty("写入内容", nodeTemp.Attributes["ReadData"].Value);
                            }
                            modelTemp.SetProperty("检定编号", Index.ToString());
                            return;
                        }
                    }
                    catch
                    { }
                }
            }
        }

        /// <summary>
        /// 添加通讯协议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddProtocol2(object sender, RoutedEventArgs e)
        {
            //string Type = "645";
            string path2 = tipsSchemeName.Text.Trim();
            string path = @"E:\工作\代码\Test\Client\Client\SX\ConnProtocol\" + path2 + ".xml";
            //string path = @"E:\工作\代码\Test\Client\Client\SX\ConnProtocol2\" + path2 + ".xml";

            bool Pc = true; //是否排除名字是数字的项目
            viewModel.ParaValuesView.Clear();
            List<string> list = new List<string>();
            for (int i = 1; i < 100; i++)
            {
                list.Add(i.ToString());
            }
           // Dictionary<string, XmlNode> data = new Dictionary<string, XmlNode>();//数据标识作为键值

            //先取出数据标识和写入内容
            XmlDocument doc2 = new XmlDocument();
            doc2.Load(path);
            XmlNode node = doc2.DocumentElement;

            if (nodeDataFlags == null)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format(@"{0}\xml\DataFlag.xml", Directory.GetCurrentDirectory()));
                nodeDataFlags = doc.DocumentElement;
            }

            foreach (XmlNode nodeTemp in node.ChildNodes)
            {
                try
                {
                    if (nodeTemp.Attributes["ConnProtocolItem"] == null)
                        continue;
                    string name = nodeTemp.Attributes["ConnProtocolItem"].Value;
                    if (Pc && list.Contains(name))
                    {
                        if (Protocol_Name.Text!="")
                        {
                            continue;
                        }
                        //name = GetProtocolName(name);
                    }

                    viewModel.AddNewParaValue();
                    viewModel.SelectedNode.ParaValuesCurrent = viewModel.ParaValuesConvertBack();
                    viewModel.RefreshPointCount();
                    string OperType = "读";
                    if (nodeTemp.Attributes["OperType"].Value != "0")
                    {
                        OperType = "写";
                    }
                    string value = nodeTemp.Attributes["ItemCode"].Value;  //标识编码
                    //string value2 = nodeTemp.Attributes["ItemCode698"].Value;  //标识编码

                    foreach (XmlNode n in nodeDataFlags)
                    {
                        if (n.Attributes["DataFlag"] == null)
                            continue;
                        if (n.Attributes["DataFlag"].Value == value)
                        {
                            name = n.Attributes["DataFlagName"].Value;
                            break;
                        }
                    }
                    if (value== "04010000")
                    {

                    }
                   //根据标识编码到旧的里面找到名字

                    DynamicViewModel modelTemp = viewModel.ParaValuesView[viewModel.ParaValuesView.Count - 1];
                    modelTemp.SetProperty("数据项名称", name);
                    modelTemp.SetProperty("标识编码", value);
                    //modelTemp.SetProperty("标识编码698", value2);

                    modelTemp.SetProperty("长度", nodeTemp.Attributes["DataLen"].Value);
                    modelTemp.SetProperty("小数位", nodeTemp.Attributes["PointIndex"].Value);
                    modelTemp.SetProperty("数据格式", nodeTemp.Attributes["StrDataType"].Value);
                    modelTemp.SetProperty("功能", OperType);

                    if (nodeTemp.Attributes["WriteContent"] != null)
                    {
                        modelTemp.SetProperty("写入内容", nodeTemp.Attributes["WriteContent"].Value);
                    }

                }
                catch (System.Exception)
                {
                }
            }



            foreach (XmlNode nodeTemp in node.ChildNodes)
            {
                try
                {
                    //if (nodeTemp.Attributes["DataFlagName"].Value == Protocol_Name.Text.Trim())
                    //{

                    if (nodeTemp.Attributes["ConnProtocolItem"] == null)
                    {
                        continue;
                    }
                    string name = nodeTemp.Attributes["ConnProtocolItem"].Value;
                    if (list.Contains(name))
                    {
                        //continue;
                        //name= GetProtocolName(name);
                       // name = $"按键循环显示第{name}屏";
                    }
    

      
                }
                catch (System.Exception)
                {

                    throw;
                }
            }

            //if (Protocol_Name.Text.Trim()!="") //
            //{
            //    if (nodeDataFlags == null)
            //    {
            //        XmlDocument doc = new XmlDocument();
            //        doc.Load(string.Format(@"{0}\xml\DataFlag.xml", Directory.GetCurrentDirectory()));
            //        nodeDataFlags = doc.DocumentElement;
            //    }
            //    foreach (XmlNode nodeTemp in nodeDataFlags.ChildNodes)
            //    {
            //        try
            //        {
            //            if (nodeTemp.Attributes["DataFlagName"].Value == Protocol_Name.Text.Trim())
            //            {

            //                viewModel.AddNewParaValue();
            //                viewModel.SelectedNode.ParaValuesCurrent = viewModel.ParaValuesConvertBack();
            //                viewModel.RefreshPointCount();

            //                SchemaNodeViewModel currentNode = treeSchema.SelectedItem as SchemaNodeViewModel;
            //                int Index = 1;
            //                if (currentNode != null)
            //                {
            //                    foreach (var item in currentNode.ParaValuesCurrent)
            //                    {
            //                        if (item.GetProperty("PARA_VALUE").ToString().Split('|')[0] == Protocol_Name.Text.Trim())
            //                        {
            //                            Index++;
            //                        }
            //                    }
            //                }
            //                DynamicViewModel modelTemp = viewModel.ParaValuesView[viewModel.ParaValuesView.Count - 1];
            //                modelTemp.SetProperty("数据项名称", nodeTemp.Attributes["DataFlagName"].Value);
            //                modelTemp.SetProperty("标识编码", nodeTemp.Attributes["DataFlag"].Value);
            //                modelTemp.SetProperty("长度", nodeTemp.Attributes["DataLength"].Value);
            //                modelTemp.SetProperty("小数位", nodeTemp.Attributes["DataSmallNumber"].Value);
            //                modelTemp.SetProperty("数据格式", nodeTemp.Attributes["DataFormat"].Value);
            //                if (nodeTemp.Attributes["ReadData"] != null)
            //                {
            //                    modelTemp.SetProperty("写入内容", nodeTemp.Attributes["ReadData"].Value);
            //                }
            //                modelTemp.SetProperty("检定编号", Index.ToString());
            //                return;
            //            }
            //        }
            //        catch
            //        { }
            //    }
            //}
        }

        /// <summary>
        /// 根据编号获取通讯协议检查的名字
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        private string GetProtocolName(string Num)
        {
            string connProtocolItem = Num;
            switch (Num)
            {
                case "0":
                    connProtocolItem = "自动循环显示第1屏";
                    break;
                case "1":
                    connProtocolItem = "自动循环显示第2屏";
                    break;
                case "2":
                    connProtocolItem = "自动循环显示第3屏";
                    break;
                case "3":
                    connProtocolItem = "自动循环显示第4屏";
                    break;
                case "4":
                    connProtocolItem = "自动循环显示第5屏";
                    break;
                case "5":
                    connProtocolItem = "自动循环显示第6屏";
                    break;
                case "6":
                    connProtocolItem = "自动循环显示第7屏";
                    break;
                case "7":
                    connProtocolItem = "按键循环显示第1屏";
                    break;
                case "8":
                    connProtocolItem = "按键循环显示第2屏";
                    break;
                case "9":
                    connProtocolItem = "按键循环显示第3屏";
                    break;
                case "10":
                    connProtocolItem = "按键循环显示第4屏";
                    break;
                case "11":
                    connProtocolItem = "按键循环显示第5屏";
                    break;
                case "12":
                    connProtocolItem = "按键循环显示第6屏";
                    break;
                case "13":
                    connProtocolItem = "按键循环显示第7屏";
                    break;
                case "14":
                    connProtocolItem = "按键循环显示第8屏";
                    break;
                case "15":
                    connProtocolItem = "按键循环显示第9屏";
                    break;
                case "16":
                    connProtocolItem = "按键循环显示第10屏";
                    break;
                case "17":
                    connProtocolItem = "按键循环显示第11屏";
                    break;
                case "18":
                    connProtocolItem = "按键循环显示第12屏";
                    break;
                case "19":
                    connProtocolItem = "按键循环显示第13屏";
                    break;
                case "20":
                    connProtocolItem = "按键循环显示第14屏";
                    break;
                case "21":
                    connProtocolItem = "按键循环显示第15屏";
                    break;
                case "22":
                    connProtocolItem = "按键循环显示第16屏";
                    break;
                case "23":
                    connProtocolItem = "按键循环显示第17屏";
                    break;
                case "24":
                    connProtocolItem = "按键循环显示第18屏";
                    break;
                case "25":
                    connProtocolItem = "按键循环显示第19屏";
                    break;
                case "26":
                    connProtocolItem = "按键循环显示第20屏";
                    break;
                case "27":
                    connProtocolItem = "按键循环显示第21屏";
                    break;
                case "28":
                    connProtocolItem = "按键循环显示第22屏";
                    break;
                case "29":
                    connProtocolItem = "按键循环显示第23屏";
                    break;
                case "30":
                    connProtocolItem = "按键循环显示第24屏";
                    break;
                case "31":
                    connProtocolItem = "按键循环显示第25屏";
                    break;
                case "32":
                    connProtocolItem = "按键循环显示第26屏";
                    break;
                case "33":
                    connProtocolItem = "按键循环显示第27屏";
                    break;
                case "34":
                    connProtocolItem = "按键循环显示第28屏";
                    break;
                case "35":
                    connProtocolItem = "按键循环显示第29屏";
                    break;
                case "36":
                    connProtocolItem = "按键循环显示第30屏";
                    break;
                case "37":
                    connProtocolItem = "按键循环显示第31屏";
                    break;
                case "38":
                    connProtocolItem = "按键循环显示第32屏";
                    break;
                case "39":
                    connProtocolItem = "按键循环显示第33屏";
                    break;
                case "40":
                    connProtocolItem = "按键循环显示第34屏";
                    break;
                case "41":
                    connProtocolItem = "按键循环显示第35屏";
                    break;
                case "42":
                    connProtocolItem = "按键循环显示第36屏";
                    break;
                case "43":
                    connProtocolItem = "按键循环显示第37屏";
                    break;
                case "44":
                    connProtocolItem = "按键循环显示第38屏";
                    break;
                case "45":
                    connProtocolItem = "按键循环显示第39屏";
                    break;
                case "46":
                    connProtocolItem = "按键循环显示第40屏";
                    break;
                case "47":
                    connProtocolItem = "按键循环显示第41屏";
                    break;
                case "48":
                    connProtocolItem = "按键循环显示第42屏";
                    break;
                case "49":
                    connProtocolItem = "按键循环显示第43屏";
                    break;
                case "50":
                    connProtocolItem = "按键循环显示第44屏";
                    break;
                case "51":
                    connProtocolItem = "按键循环显示第45屏";
                    break;
                case "52":
                    connProtocolItem = "按键循环显示第46屏";
                    break;
                case "53":
                    connProtocolItem = "按键循环显示第47屏";
                    break;
                case "54":
                    connProtocolItem = "按键循环显示第48屏";
                    break;
                case "55":
                    connProtocolItem = "按键循环显示第49屏";
                    break;
                case "56":
                    connProtocolItem = "按键循环显示第50屏";
                    break;
                case "57":
                    connProtocolItem = "按键循环显示第51屏";
                    break;
                case "58":
                    connProtocolItem = "按键循环显示第52屏";
                    break;
                case "59":
                    connProtocolItem = "按键循环显示第53屏";
                    break;
                case "60":
                    connProtocolItem = "按键循环显示第54屏";
                    break;
                case "61":
                    connProtocolItem = "按键循环显示第55屏";
                    break;
                case "62":
                    connProtocolItem = "按键循环显示第56屏";
                    break;
                case "63":
                    connProtocolItem = "按键循环显示第57屏";
                    break;
                case "64":
                    connProtocolItem = "按键循环显示第58屏";
                    break;
                case "65":
                    connProtocolItem = "按键循环显示第59屏";
                    break;
                case "66":
                    connProtocolItem = "按键循环显示第60屏";
                    break;
                case "67":
                    connProtocolItem = "按键循环显示第61屏";
                    break;
                case "68":
                    connProtocolItem = "按键循环显示第62屏";
                    break;
                case "69":
                    connProtocolItem = "按键循环显示第63屏";
                    break;
                case "70":
                    connProtocolItem = "按键循环显示第64屏";
                    break;
                case "71":
                    connProtocolItem = "按键循环显示第65屏";
                    break;
                case "72":
                    connProtocolItem = "按键循环显示第66屏";
                    break;
                case "73":
                    connProtocolItem = "按键循环显示第67屏";
                    break;
                case "74":
                    connProtocolItem = "按键循环显示第68屏";
                    break;
                case "75":
                    connProtocolItem = "按键循环显示第69屏";
                    break;
                case "76":
                    connProtocolItem = "按键循环显示第70屏";
                    break;
                case "77":
                    connProtocolItem = "按键循环显示第71屏";
                    break;
                case "78":
                    connProtocolItem = "按键循环显示第72屏";
                    break;
                case "79":
                    connProtocolItem = "按键循环显示第73屏";
                    break;
                case "80":
                    connProtocolItem = "按键循环显示第74屏";
                    break;
                case "81":
                    connProtocolItem = "按键循环显示第75屏";
                    break;
                case "82":
                    connProtocolItem = "按键循环显示第76屏";
                    break;
                case "83":
                    connProtocolItem = "按键循环显示第77屏";
                    break;
                case "84":
                    connProtocolItem = "按键循环显示第78屏";
                    break;
                case "85":
                    connProtocolItem = "按键循环显示第79屏";
                    break;
                case "86":
                    connProtocolItem = "按键循环显示第80屏";
                    break;
                case "87":
                    connProtocolItem = "按键循环显示第81屏";
                    break;
                case "88":
                    connProtocolItem = "按键循环显示第82屏";
                    break;
                case "89":
                    connProtocolItem = "按键循环显示第83屏";
                    break;
                case "90":
                    connProtocolItem = "按键循环显示第84屏";
                    break;
                case "91":
                    connProtocolItem = "有功常数";
                    break;
                case "92":
                    connProtocolItem = "资产编号";
                    break;
                case "93":
                    connProtocolItem = "有功组合方式特征字";
                    break;
                case "94":
                    connProtocolItem = "电表运行特征字1";
                    break;
                case "95":
                    connProtocolItem = "自动循环显示屏数";
                    break;
                case "96":
                    connProtocolItem = "按键显示屏数";
                    break;
                case "97":
                    connProtocolItem = "每月第1结算日";
                    break;
                default:
                    break;
            }
            return connProtocolItem;
        }

        private void btn_RemoveCF(object sender, RoutedEventArgs e)
        {

            //for (int i = 0; i < viewModel.Children.Count; i++)
            //{
            //    viewModel.Children[i].ParaValuesCurrent
            //}


            for (int i = 0; i < viewModel.ParaValuesView.Count; i++)
            {
                DynamicViewModel modelTemp = viewModel.ParaValuesView[i];
                string s1 = modelTemp.GetProperty("功率方向").ToString();
                string s2 = modelTemp.GetProperty("功率元件").ToString();
                string s3 = modelTemp.GetProperty("功率因素").ToString();
                string s4 = modelTemp.GetProperty("电流倍数").ToString();
                for (int j = i+1; j < viewModel.ParaValuesView.Count; j++)
                {
                    DynamicViewModel item = viewModel.ParaValuesView[j];
                    string q1 = item.GetProperty("功率方向").ToString();
                    string q2 = item.GetProperty("功率元件").ToString();
                    string q3 = item.GetProperty("功率因素").ToString();
                    string q4 = item.GetProperty("电流倍数").ToString();

                    if (item != null)
                    {
                        if (s1==q1 && s2 == q2 && s3 == q3 && s4 == q4)
                        {
                            viewModel.ParaValuesView.Remove(modelTemp);
                            i--;
                            break;
                        }
                    }
                }
            }
            //viewModel.AddNewParaValue();
            //viewModel.SelectedNode.ParaValuesCurrent = viewModel.ParaValuesConvertBack();
            //viewModel.RefreshPointCount();
            //for (int i = 0; i < Schema[key].SchemaNodeValue.Count; i++)
            //{
            //    DynamicViewModel viewModel2 = new DynamicViewModel(propertyNames, 0);
            //    viewModel2.SetProperty("IsSelected", true);
            //    string[] value = Schema[key].SchemaNodeValue[i].Split('|');
            //    for (int j = 0; j < propertyNames.Count; j++)
            //    {
            //        viewModel2.SetProperty(propertyNames[j], value[j]); //这里改成参数的值
            //    }
            //    EquipmentData.Schema.ParaValuesView.Add(viewModel2);
            //}
            //errormodel = new ErrorModel
            //{
            //    Current = current,
            //    Factor = factor,
            //    FangXiang = category.Fangxiang,
            //    Component = category.Component,
            //    GuichengMulti = category.GuichengMulti,
            //    LapCountIb = category.LapCountIb
            //};
            //category.OnPointsChanged(errormodel);
            viewModel.SelectedNode.ParaValuesCurrent = viewModel.ParaValuesConvertBack();
            viewModel.RefreshPointCount();
            
        }

        private void Click_SchemaExpand(object sender, RoutedEventArgs e)
        {
            Windows.Window_ShemaSet shemaSet = new Windows.Window_ShemaSet();
            //shemaSet.Owner = Application.Current.MainWindow;
            //win.Owner = Application.Current.MainWindow
            shemaSet.ShowDialog();
        }
    }
}

