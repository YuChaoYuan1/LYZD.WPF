using LYZD.DAL;
using LYZD.DAL.Config;
using LYZD.ViewModel;
using LYZD.ViewModel.CodeTree;
using LYZD.ViewModel.InputPara;
using LYZD.WPF.Controls;
using LYZD.WPF.Converter;
using LYZD.WPF.Model;
using LYZD.WPF.UiGeneral;
using LYZD.WPF.View.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LYZD.WPF.View
{
    /// <summary>
    /// View_Input.xaml 的交互逻辑
    /// </summary>
    public partial class View_Input
    {
        public View_Input()
        {
            InitializeComponent();
            Name = "参数录入";

            //Utility.Log.LogManager.AddMessage("定位参数录入003", Utility.Log.EnumLogSource.用户操作日志, Utility.Log.EnumLevel.Warning);

            DockStyle.IsFloating = true;
            DockStyle.IsMaximized = true;
            DockStyle.FloatingSize = SystemParameters.WorkArea.Size;
            for (int i = 0; i < viewModel.ParasModel.AllUnits.Count; i++)
            {
                if (viewModel.ParasModel.AllUnits[i].IsSame && viewModel.ParasModel.AllUnits[i].IsNecessary)
                {
                    AddBasicPara(viewModel.ParasModel.AllUnits[i]);
                }
            }
            comboBoxSchema.DataContext = EquipmentData.SchemaModels;
            GenerateColumns();
        }

        private MeterInputParaViewModel viewModel
        {
            get { return Resources["MeterInputParaViewModel"] as MeterInputParaViewModel; }
        }
        private void AddBasicPara(InputParaUnit paraUnit)
        {
            StackPanel stackPanel = new StackPanel()
            {
                Margin = new Thickness(5, 3, 5, 3),
                Orientation = Orientation.Horizontal,
            };
            TextBlock textBlock = new TextBlock
            {
                Text = paraUnit.DisplayName,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 50
            };
            ControlEnumComboBox comboBox = new ControlEnumComboBox()
            {
                EnumName = paraUnit.CodeType,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Style = Application.Current.Resources["StyleComboBox"] as Style,
                Width = 80,
                Tag = paraUnit.FieldName,

            };

            comboBox.SetBinding(ComboBox.SelectedItemProperty, new Binding(string.Format("FirstMeter.{0}", paraUnit.FieldName)) { Mode = BindingMode.TwoWay });
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(comboBox);
            wrapPanelParas.Children.Add(stackPanel);
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                string fieldName = comboBox.Tag as string;
                if (!string.IsNullOrEmpty(fieldName))
                {
                    for (int i = 0; i < viewModel.Meters.Count; i++)
                    {
                      viewModel.Meters[i].SetProperty(fieldName, comboBox.SelectedItem);     
                    }
                }
            }
        }



        public bool IsAllSelected
        {                                                                                                                           
            get { return (bool)GetValue(IsAllSelectedProperty); }
            set { SetValue(IsAllSelectedProperty, value); }
        }

        // 使用DependencyProperty作为IsAllSelected的备份存储。这将启用动画、样式设置、绑定等。。
        public static readonly DependencyProperty IsAllSelectedProperty =
            DependencyProperty.Register("IsAllSelected", typeof(bool), typeof(View_Input), new PropertyMetadata(false));

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "IsAllSelected")
            {
                for (int i = 0; i < viewModel.Meters.Count; i++)
                {
                    viewModel.Meters[i].SetProperty("MD_CHECKED", IsAllSelected ? "1" : "0");
                }
            }
            base.OnPropertyChanged(e);
        }


       /// <summary>
       /// 生成表格列
       /// </summary>
        private void GenerateColumns()
        {
            #region 要检
            CheckBox checkbox = new CheckBox
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ToolTip = "表要检标记,全选"
            };
            Binding binding = new Binding("IsAllSelected")
            {
                Source = this
            };
            checkbox.SetBinding(CheckBox.IsCheckedProperty, binding);
            Binding cellBinding = new Binding("MD_CHECKED");
            cellBinding.Mode = BindingMode.TwoWay;
            cellBinding.Converter = new BoolBitConverter();
            DataGridColumn columnYaojian = Resources["KeyYaojianColumn"] as DataGridColumn;
            columnYaojian.Header = checkbox;
            dgv_MeterData.Columns.Add(columnYaojian);
            #endregion
            for (int i = 0; i < viewModel.ParasModel.AllUnits.Count; i++)
            {
                InputParaUnit paraUnit = viewModel.ParasModel.AllUnits[i];
                if (paraUnit.IsDisplayMember && (!paraUnit.IsSame) && (paraUnit.FieldName != "MD_CHECKED"))
                {
                    DataGridColumn column;
                    column = ControlFactory.CreateColumn(paraUnit.DisplayName, paraUnit.CodeType, paraUnit.FieldName, true);

                    if (paraUnit.FieldName == "MD_BAR_CODE")
                    {
                        column.Width = 170;
                    }
                    else if (paraUnit.FieldName == "MD_POSTAL_ADDRESS")
                    {
                        //column.IsReadOnly = true;
                        column.Width = 100;
                    }
                    else if (paraUnit.FieldName == "MD_EPITOPE")
                    {
                        column.IsReadOnly = true;
                        column.Width = 40;
                    }
                    else
                    {
                        column.MinWidth = 40;
                        column.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
                    }

                    dgv_MeterData.Columns.Add(column);
                }
            }
        }


        #region 单元格
        public DataGridCell GetCell(DataGrid grid,int row,int column)
        {
            DataGridRow rowContainer = GetRow(grid, row);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter==null)
                {
                    grid.ScrollIntoView(rowContainer,grid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        
        }
        /// <summary>
        /// 获取行索引取的行对象
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataGridRow GetRow(DataGrid grid,int index)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row==null)
            {
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        
        }

        private T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual visual = (Visual)VisualTreeHelper.GetChild(parent,i);
                child = visual as T;
                if (child==null)
                {
                    child = GetVisualChild<T>(visual);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        #endregion
        private void dgv_MeterData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //跳转行
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var colunm = e.Column as DataGridBoundColumn;
                if (colunm != null)
                {
                    var bindingPath = (colunm.Binding as Binding).Path.Path;
                    if (bindingPath == "MD_BAR_CODE")
                    {
                        int rowIndex = e.Row.GetIndex();
                        int colunIndex = 0;
                        if (dgv_MeterData.CurrentCell.Column!=null) {
                            colunIndex = dgv_MeterData.CurrentCell.Column.DisplayIndex;
                        }
                        
                        DynamicViewModel modelTemp = viewModel.Meters[rowIndex];
                        bool T = false;
                        for (int i = rowIndex + 1; i < viewModel.Meters.Count - 1; i++)
                        {
                            DynamicViewModel modelTemp1 = viewModel.Meters[i];
                            if (modelTemp1.GetProperty("MD_CHECKED").ToString() == "1")
                            {
                                T = true;
                                rowIndex = i;
                                break;
                            }
                        }

                        var cell = GetCell(dgv_MeterData, rowIndex, colunIndex);
                        if (cell != null)
                        {
                            cell.IsSelected = true;
                            cell.Focus();
                            dgv_MeterData.BeginEdit();
                        }

                    }

                }
            }



            if (!(checkBoxQuickInput.IsChecked.HasValue && checkBoxQuickInput.IsChecked.Value))
            {
                return;
            }
            #region 校验单元格
            string columnHeader = e.Column.Header as string;
            InputParaUnit paraUnit = viewModel.ParasModel.AllUnits.FirstOrDefault(item => item.DisplayName == columnHeader);
            if (paraUnit == null)
            {
                return;
            }
            string fieldTemp = paraUnit.FieldName;

            DynamicViewModel meterCurrent = e.Row.DataContext as DynamicViewModel;
            if (meterCurrent == null)
            {
                return;
            }
            int indexTemp = viewModel.Meters.IndexOf(meterCurrent);
            #endregion

            object cellValue = "";
            ComboBox currentElement1 = e.EditingElement as ComboBox;
            if (currentElement1 != null)
            {
                cellValue = currentElement1.SelectedItem;
            }
            TextBox currentElement2 = e.EditingElement as TextBox;
            if (currentElement2 != null)
            {
                cellValue = currentElement2.Text;
            }

            if (cellValue == null)
            {
                return;
            }

            if (cellValue.Equals(meterCurrent.GetProperty(fieldTemp)))
            {
                return;
            }

            //if (fieldTemp == "AVR_BAR_CODE")
            //{
            //    if (!string.IsNullOrEmpty(cellValue as string))
            //    {
            //        string tempBarCode = cellValue.ToString();
            //        int assertNoStartIndex = ConfigHelper.Instance.AssertNoStartIndex - 1;
            //        int assertNoLength = ConfigHelper.Instance.AssertNoLength;
            //        if (ConfigHelper.Instance.IsAssertNoFromBarCode && assertNoStartIndex >= 0)
            //        {
            //            if (tempBarCode.Length >= assertNoStartIndex + assertNoLength)
            //            {
            //                string assertNo = string.Format("{0}{1}{2}", ConfigHelper.Instance.AssertNoStartStr, tempBarCode.Substring(assertNoStartIndex, assertNoLength), ConfigHelper.Instance.AssertNoEndStr);
            //                viewModel.Meters[indexTemp].SetProperty("AVR_MADE_NO", assertNo);
            //            }
            //        }
            //    }
            //    return;
            //}

            //不需要快速录入的
            List<string> NoQuickInput = new List<string>() { "MD_BAR_CODE","MD_ASSET_NO","MD_MADE_NO" };
           // List<string> NoQuickInput = new List<string>() { "MD_MADE_NO" };
            if (NoQuickInput.Contains(fieldTemp))
            {
                return;
            }

            //证书编号自动录入部分
            string CertificateIndex = "";
            for (int i = indexTemp; i < viewModel.Meters.Count; i++)
            {
                if (fieldTemp == "MD_CERTIFICATE_NO")//证书编号
                {
                    CertificateIndex = (i + 1).ToString().PadLeft(3, '0');
                    viewModel.Meters[i].SetProperty(fieldTemp, cellValue + CertificateIndex);
                }
                else if (fieldTemp == "MD_BAR_CODE" || fieldTemp == "MD_ASSET_NO")    //资产编号，条形码
                {
                    viewModel.Meters[i].SetProperty(fieldTemp, cellValue + (i + 1).ToString());
                }
                else
                {
                    viewModel.Meters[i].SetProperty(fieldTemp, cellValue);
                }
            }
   
        }

        /// <summary>
        /// 是否需要检定复选框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox boxYaojian = sender as CheckBox;
            if (boxYaojian != null)
            {
                string ischeck = boxYaojian.IsChecked == true ? "1" : "0";
                DynamicViewModel modelTemp = boxYaojian.DataContext as DynamicViewModel;
                if (modelTemp != null)
                {
                    modelTemp.SetProperty("MD_CHECKED", ischeck);
                }
            }

        }

        /// <summary>
        /// 添加新的编码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Event_AddNew(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = e.OriginalSource as ComboBox;
            if (comboBox == null || comboBox.SelectedItem as string != "添加新项")
            {
                return;
            }
            #region 添加新的编码
            BindingExpression expression = comboBox.GetBindingExpression(ComboBox.SelectedItemProperty);
            if (expression == null)
            {
                return;
            }
            //解析编码路径
            string strPath = expression.ParentBinding.Path.Path;
            //如果没有创建新的值,就恢复原来的值
            string oldValue = (expression.DataItem as DynamicViewModel).GetProperty(strPath) as string;
            InputParaUnit unitTemp = viewModel.ParasModel.AllUnits.FirstOrDefault(item => item.FieldName == strPath);
            if (unitTemp != null)
            {
                //获取节点
                CodeTreeNode nodeTemp = CodeTreeViewModel.Instance.GetCodeByEnName(unitTemp.CodeType, 2);
                if (nodeTemp != null)
                {
                    Window_AddNewCode windowTemp = new Window_AddNewCode(nodeTemp);
                    bool? boolTemp = windowTemp.ShowDialog();
                    if (boolTemp.HasValue && boolTemp.Value)
                    {
                        DataGridCell cellTemp = Utils.FindVisualParent<DataGridCell>(comboBox);
                        if (cellTemp != null)
                        {
                            DataGridComboBoxColumn columnTemp = cellTemp.Column as DataGridComboBoxColumn;
                            if (columnTemp != null && nodeTemp.Children.Count > 0)
                            {
                                Dictionary<string, string> dictionary = CodeDictionary.GetLayer2(nodeTemp.CODE_TYPE);
                                List<string> dataSource = dictionary.Keys.ToList();
                                dataSource.Add("添加新项");
                                columnTemp.ItemsSource = dataSource;
                                comboBox.SelectedItem = nodeTemp.Children[nodeTemp.Children.Count - 1].CODE_NAME;
                                return;
                            }
                        }
                    }
                }
            }
            #endregion
            comboBox.SelectedItem = oldValue;
        }
        private void Click_IsChecked(object sender, RoutedEventArgs e)
        {
            int index = -1;
            MenuItem menuTemp = e.OriginalSource as MenuItem;
            if (menuTemp != null)
            {
                string ischek = "-1";
                bool UpOrDown = true; //true是下，false向上
                switch (menuTemp.Name)
                {
                    case "UpCheck": //向上选表
                        ischek = "1";
                        UpOrDown = false;
                        break;
                    case "DownCheck":  //向下选表
                        ischek = "1";
                        UpOrDown = true;
                        break;
                    case "NoUpCheck"://向上取消选表
                        ischek = "0";
                        UpOrDown = false;
                        break;
                    case "NoDownCheck": //向下取消选表
                        ischek = "0";
                        UpOrDown = true;
                        break;
                    default:
                        break;
                }
                var _cells = dgv_MeterData.SelectedCells; //获取选中单元格的列表
                if (_cells.Any())
                {
                    index = dgv_MeterData.Items.IndexOf(_cells.First().Item);
                }

                if (index >= 0 && ischek != "-1")
                {
                    if (UpOrDown)  //向下
                    {
                        for (int i = index; i < viewModel.Meters.Count; i++)
                        {
                            DynamicViewModel modelTemp = viewModel.Meters[i];
                            modelTemp.SetProperty("MD_CHECKED", ischek);
                        }
                    }
                    else
                    {
                        for (int i = 0; i <= index; i++)
                        {
                            DynamicViewModel modelTemp = viewModel.Meters[i];
                            modelTemp.SetProperty("MD_CHECKED", ischek);
                        }
                    }
                }
            }
        }

        //private void Click_DwonMeterInfo(object sender, RoutedEventArgs e)
        //{
        //    viewModel.Frame_DownMeterInfoFromMis();
        //}
    }
}
