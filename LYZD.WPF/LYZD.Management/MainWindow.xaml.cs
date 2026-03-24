using LYZD.DataManager.Controls;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using LYZD.DataManager.ViewModel;
using LYZD.DataManager.Mark.ViewModel;
using LYZD.DataManager.ViewModel.Meters;
using LYZD.ViewModel.InputPara;
using System.Windows.Data;
using LYZD.DataManager.Converter;
using System.Linq;
using System.Windows.Media;
using System.Windows;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckInfo;
using LYZD.Utility;
using Aspose.Cells;
using System.Data;
using System.ComponentModel;
using System.IO;
using LYZD.Mis.Common;
using System.Diagnostics;
using Aspose.Words;
using LYZD.Core.Enum;
using System.Reflection;
using LYZD.DAL;
using Aspose.Words.Tables;
using LYZD.Core.Model.DnbModel.DnbInfo;
using System.Text;

namespace LYZD.DataManager
{
    /// <summary>
    /// 主窗体
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// 主窗体
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            LoadColumns();
            LoadTemplates();
            viewModel.SearchMeters();
            checkBoxPreview.IsChecked = Properties.Settings.Default.IsPreview;
            datagridMeter.ColumnReordered += datagridMeter_ColumnReordered;

            //PageMeters pageMeters = new PageMeters();
            //Pages.Add(pageMeters);
            //frameMain.Navigate(pageMeters);
            textBlockMessage.DataContext = MessageDisplay.Instance;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (DetailedData.Instance != null)
            {
                DetailedData.Instance.IsClose = true;
                DetailedData.Instance.Close();
            }
            base.OnClosing(e);
        }


        void datagridMeter_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            var columnsTemp = datagridMeter.Columns.OrderBy(item => item.DisplayIndex);
            var columnNames = from item in columnsTemp select item.Header.ToString();
            Properties.Settings.Default.ColumnNames = string.Join(",", columnNames);
            Properties.Settings.Default.Save();
        }
        private MetersViewModel meterResults = new MetersViewModel(null);
        /// <summary>
        /// 表信息模型
        /// </summary>
        private MetersViewModel viewModel
        {
            get
            {
                return Resources["MetersViewModel"] as MetersViewModel;
            }
        }
        /// <summary>
        /// 窗体的拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private List<Page> pages = new List<Page>();
        /// <summary>
        /// 页面列表
        /// </summary>
        public List<Page> Pages { get { return pages; } set { pages = value; } }


        #region 数据
        /// <summary>
        /// 加载列
        /// </summary>
        private void LoadColumns()
        {
            if (Properties.Settings.Default.ColumnNames == null)
            {
                Properties.Settings.Default.ColumnNames = "";
            }
            string[] columnNames = Properties.Settings.Default.ColumnNames.Split(',');
            //这里添加分项总结论
            //1：先获得大项目的名字，来创建列
            //2: 在获取每个大项目下所有结论的集合


            //columnNames = SortColunms(columnNames);//对列进行排序
            List<string> resoultS = GetResoultName();
            if (columnNames.Length != MetersViewModel.ParasModel.AllUnits.Count + resoultS.Count)
            {
                List<string> list = new List<string>();
                list.AddRange(resoultS);
                for (int i = 0; i < MetersViewModel.ParasModel.AllUnits.Count; i++)
                {
                    list.Add(MetersViewModel.ParasModel.AllUnits[i].DisplayName);
                }

                columnNames = list.ToArray();
                Properties.Settings.Default.ColumnNames = string.Join(",", columnNames);
                Properties.Settings.Default.Save();
            }

            for (int i = 0; i < columnNames.Length; i++)
            {
                InputParaUnit paraUnit = MetersViewModel.ParasModel.AllUnits.FirstOrDefault(item => item.DisplayName == columnNames[i]);
                if (paraUnit != null)
                {
                    DataGridColumn column = new DataGridTextColumn
                    {
                        Header = paraUnit.DisplayName,
                        Binding = new Binding(paraUnit.FieldName),
                        IsReadOnly = true
                    };
                    datagridMeter.Columns.Add(column);
                }
                else
                {
                    if (resoultS.Contains(columnNames[i]))
                    {
                        DataGridColumn column = new DataGridTextColumn
                        {
                            Header = columnNames[i],
                            Binding = new Binding(columnNames[i]),
                            IsReadOnly = true
                        };
                        datagridMeter.Columns.Add(column);
                    }
                }




                #region 要检

                //if (paraUnit.FieldName == "CHR_CHECKED")
                //{
                //    Binding cellBinding = new Binding("CHR_CHECKED");
                //    cellBinding.Mode = BindingMode.TwoWay;
                //    cellBinding.Converter = new BoolBitConverter();
                //    DataGridColumn columnYaojian = new DataGridCheckBoxColumn
                //    {
                //        Header = paraUnit.DisplayName,
                //        Binding = cellBinding,
                //        IsReadOnly = true
                //    };
                //    datagridMeter.Columns.Add(columnYaojian);
                //}

                //else
                //{
                //    DataGridColumn column = new DataGridTextColumn
                //    {
                //        Header = paraUnit.DisplayName,
                //        Binding = new Binding(paraUnit.FieldName),
                //        IsReadOnly = true
                //    };
                //    datagridMeter.Columns.Add(column);
                //}
                #endregion
            }
        }


        private List<string> GetResoultName()
        {
            List<string> ProjectName = new List<string>();
            //获取所有ID的名称
            try
            {
                FieldInfo[] f_key = typeof(UniversityMeterID).GetFields();
                for (int i = 0; i < f_key.Length; i++)
                {
                    ProjectName.Add(f_key[i].Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return ProjectName;
        }


        private string[] SortColunms(string[] columnNames)
        {
            List<string> list = new List<string>() {"表位", "结论", "条形码","资产编号", "任务编号", "通讯地址", "检定规程", "台体编号", "检定日期", "计检日期", "检验员", "核验员", "主管",
                "证书编号", "出厂编号", "温度", "湿度" };
            List<string> value = columnNames.ToList();

            List<string> resoult = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                if (value.Contains(list[i]))  //找到这个值
                {
                    resoult.Add(list[i]); //按照顺序添加指定的排序
                    value.Remove(list[i]); //删除旧的
                }
            }

            for (int i = 0; i < value.Count; i++)
            {
                resoult.Add(value[i]); //按照顺序添加指定的排序
            }
            return resoult.ToArray();
        }

        /// <summary>
        /// 加载报表模板列表
        /// </summary>
        private void LoadTemplates()
        {
            string[] fileNames = Directory.GetFiles(string.Format(@"{0}\Res\Word", Directory.GetCurrentDirectory()));
            System.Collections.Generic.List<string> listNames = new System.Collections.Generic.List<string>();
            foreach (string fileName in fileNames)
            {
                string[] arrayName = fileName.Split('\\');
                string nameTemp = arrayName[arrayName.Length - 1];
                if (nameTemp.EndsWith(".doc") || nameTemp.EndsWith(".docx"))
                {
                    listNames.Add(nameTemp);
                }
            }
            comboBoxTemplates.ItemsSource = listNames;
            comboBoxTemplates.SelectedItem = Properties.Settings.Default.ReportPath;
        }
        #endregion

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                ReportHelper.WordApp.Quit(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges);
            }
            catch
            { }
            try
            {
                DocumentViewModel.WordApp.Quit(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges);
            }
            catch
            { }
            base.OnClosed(e);
        }
        private ContextMenu menuTemp
        {
            get { return Resources["contextMenu"] as ContextMenu; }
        }
        private void datagridMeter_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            #region 寻找鼠标点到的单元格
            DataGridCell cellTemp = null;
            Point pointTemp = e.GetPosition(datagridMeter);
            HitTestResult hitResult = VisualTreeHelper.HitTest(datagridMeter, pointTemp);
            if (hitResult != null)
            {
                cellTemp = Utils.FindVisualParent<DataGridCell>(hitResult.VisualHit);
            }
            if (cellTemp == null)
            {
                //menuTemp.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                menuTemp.Visibility = Visibility.Visible;
            }
            #endregion
            DynamicViewModel modelTemp = cellTemp.DataContext as DynamicViewModel;
            string fieldName = "";
            #region 获取列对应的字段名称
            if (cellTemp.Column is DataGridTextColumn)
            {
                DataGridTextColumn columnTemp = cellTemp.Column as DataGridTextColumn;
                Binding bindingTemp = columnTemp.Binding as Binding;
                fieldName = bindingTemp.Path.Path;
            }
            else
            {
                if (cellTemp.Column is DataGridCheckBoxColumn)
                {
                    DataGridCheckBoxColumn columnTemp = cellTemp.Column as DataGridCheckBoxColumn;
                    Binding bindingTemp = columnTemp.Binding as Binding;
                    fieldName = bindingTemp.Path.Path;
                }
            }
            #endregion
            if (string.IsNullOrEmpty(fieldName))
            {
                //menuTemp.Visibility = Visibility.Collapsed;
                return;
            }
            object cellValue = modelTemp.GetProperty(fieldName);
            string temp = "";
            if (cellValue != null)
            {
                temp = cellValue.ToString();
            }
            viewModel.LoadFilterCollection(fieldName, temp);
        }

        private void datagridMeter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// 预览按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Properties.Settings.Default.IsPreview = checkBoxPreview.IsChecked.HasValue && checkBoxPreview.IsChecked.Value;
            Properties.Settings.Default.Save();
        }
        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_SearchMeters(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.SearchMeters1();
        }

        /// <summary>
        /// 选中模板改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxTemplates.SelectedItem != null)
            {
                Properties.Settings.Default.ReportPath = comboBoxTemplates.SelectedItem.ToString();
                Properties.Settings.Default.Save();

                //MainWindow windowTemp = Application.Current.MainWindow as MainWindow;
                //if (windowTemp != null)
                //{
                //    PageInsertBookmark pageWordTemplate = windowTemp.Pages.FirstOrDefault(item => item is PageInsertBookmark) as PageInsertBookmark;
                //    if (pageWordTemplate != null)
                //    {
                //        Dispatcher.BeginInvoke(new Action(() =>
                //        {
                //            pageWordTemplate.OpenTemplate();
                //        }));
                //    }
                //}
            }
        }
        /// <summary>
        /// 详细数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            DynamicViewModel dynamic = datagridMeter.SelectedItem as DynamicViewModel;
            if (dynamic == null)
            {
                return;
            }

            OneMeterResult meterResult = null;
            if (datagridMeter.SelectedIndex >= 0 && datagridMeter.SelectedIndex <= viewModel.ResultCollection.ItemsSource.Count)
            {
                meterResult = viewModel.ResultCollection.ItemsSource[datagridMeter.SelectedIndex];
            }
            //OneMeterResult meterResult = new OneMeterResult(dynamic.GetProperty("METER_ID").ToString(), false);
            if (meterResult == null)
            {
                return;
            }

            LoadMeterDataGrids(meterResult);
            try
            {
                string bar = dynamic.GetProperty("MD_BAR_CODE").ToString();
                string ID = dynamic.GetProperty("MD_EPITOPE").ToString();
                string res = dynamic.GetProperty("MD_RESULT").ToString();
                DetailedData.Instance.Title = $"详细数据--条形码【{bar}】--表位号【{ID}】--{res}";
            }
            catch (Exception)
            {
            }

            DetailedData.Instance.Show();
            if (DetailedData.Instance.WindowState == WindowState.Minimized)
            {
                DetailedData.Instance.WindowState = WindowState.Maximized;
            }




        }
        #region 导出excel文件


        /// <summary>
        /// 加载结论对应的表格
        /// </summary>
        private void LoadMeterDataGrids(OneMeterResult meterResult)
        {
            if (meterResult == null)
            {
                return;
            }
            DetailedData.Instance.resultContainer.Children.Clear();
            List<string> list = ItemResoultkeyword.GetSpliteList();  //排除关键字
            for (int i = 0; i < meterResult.Categories.Count; i++)
            {
                //TODO 显示分项数据的--问题是绑定的时候速度特别慢--然后字体颜色没有改变--暂时不用
                if (false && (meterResult.Categories[i].Names.Contains("分项结论376") || meterResult.Categories[i].Names.Contains("分项结论698")))
                {
                    List<string> name = new List<string>() { "项目名称", "测试时间", "标准数据", "终端数据", "结论", "提示信息" };
                    List<string> english_Name = new List<string>() { "Name", "Time", "StandaedData", "TerminalData", "Resoult", "Tips" };

                    string protocol = meterResult.MeterInfo.GetProperty("MD_PROTOCOL_TYPE") as string == "376.1" ? "376" : "698";

                    for (int w = 0; w < meterResult.Categories[i].ResultUnits.Count; w++)
                    {
                        if (!meterResult.Categories[i].Names.Contains("分项结论" + protocol)) continue;
                        DataGrid dataGrid = new DataGrid()
                        {
                            Margin = new Thickness(3),
                            HeadersVisibility = DataGridHeadersVisibility.All,
                            IsReadOnly = true,
                            Style = Application.Current.Resources["dataGridStyleMeterDetailResult"] as System.Windows.Style,
                        };
                        string value = meterResult.Categories[i].ResultUnits[w].GetProperty("分项结论" + protocol) as string;

                        if (meterResult.Categories[i].ResultUnits[w].ItemResoult.Count > 0) meterResult.Categories[i].ResultUnits[w].ItemResoult.Clear();
                        if (value == null || value.ToString() == "") continue;
                        //里面写真正的业务内容
                        string[] tmp = value.ToString().Split('#');
                        for (int s = 0; s < tmp.Length; s++)
                        {
                            meterResult.Categories[i].ResultUnits[w].ItemResoult.Add(new ItemResoultDataFormat(tmp[s]));
                        }
                        dataGrid.ItemsSource = meterResult.Categories[i].ResultUnits[w].ItemResoult;

                        for (int j = 0; j < name.Count; j++)
                        {
                            DataGridTextColumn column = new DataGridTextColumn()
                            {
                                Header = name[j],
                                Binding = new Binding(english_Name[j]),
                                Width = new DataGridLength(1, DataGridLengthUnitType.Auto)
                            };
                            dataGrid.Columns.Add(column);
                        }
                        DetailedData.Instance.resultContainer.Children.Add(dataGrid);
                    }
                }
                else
                {
                    DataGrid dataGrid = new DataGrid()
                    {
                        Margin = new Thickness(3),
                        HeadersVisibility = DataGridHeadersVisibility.All,
                        IsReadOnly = true,
                        Style = Application.Current.Resources["dataGridStyleMeterDetailResult"] as System.Windows.Style,
                    };
                    dataGrid.ItemsSource = meterResult.Categories[i].ResultUnits;
                    for (int j = 0; j < meterResult.Categories[i].Names.Count; j++)
                    {
                        string columnName = meterResult.Categories[i].Names[j];
                        if (columnName == "要检" || columnName == "项目名" || columnName == "项目号" || list.Contains(columnName))
                        {
                            continue;
                        }
                        DataGridTextColumn column = new DataGridTextColumn()
                        {
                            Header = columnName,
                            Binding = new Binding(columnName),
                            Width = new DataGridLength(1, DataGridLengthUnitType.Auto)
                        };
                        dataGrid.Columns.Add(column);
                    }
                    dataGrid.PreviewMouseWheel += (sender, e) =>
                    {
                        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                        eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                        eventArg.Source = sender;
                        dataGrid.RaiseEvent(eventArg);
                    };
                    DetailedData.Instance.resultContainer.Children.Add(dataGrid);
                }






                //dataGrid.ItemsSource = meterResult.Categories[i].ResultUnits;
                //for (int j = 0; j < meterResult.Categories[i].Names.Count; j++)
                //{
                //    string columnName = meterResult.Categories[i].Names[j];
                //    if (columnName == "要检" || columnName == "项目名" || columnName == "项目号"|| list.Contains(columnName))
                //    {
                //        continue;
                //    }
                //    DataGridTextColumn column = new DataGridTextColumn()
                //    {
                //        Header = columnName,
                //        Binding = new Binding(columnName),
                //        Width = new DataGridLength(1, DataGridLengthUnitType.Auto)
                //    };
                //    dataGrid.Columns.Add(column);
                //}
                //DetailedData.Instance.resultContainer.Children.Add(dataGrid);
            }
        }


        /// <summary>
        /// 导出excel文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Export_Excel_Click(object sender, RoutedEventArgs e)
        {
            var meters = viewModel.Meters.Where(item => ((bool)item.GetProperty("IsSelected")));
            int s = meters.Count();
            if (meters == null || meters.Count() == 0)
            {
                MessageBox.Show("请至少选择一块表!");
                return;
            }
            foreach (DynamicViewModel meter in meters)
            {
                string barcode = meter.GetProperty("MD_BAR_CODE") as string;
                string MD_FACTORY = meter.GetProperty("MD_FACTORY") as string;
                string MD_TERMINAL_TYPE = meter.GetProperty("MD_TERMINAL_TYPE") as string;
                string MD_WIRING_MODE = meter.GetProperty("MD_WIRING_MODE") as string;
                //OneMeterResult meterResult = new OneMeterResult(meter.GetProperty("METER_ID").ToString(), false);
                OneMeterResult meterResult = viewModel.ResultCollection.ItemsSource[meter.Index - 1];

                try
                {
                    Save_Excel(meterResult, MD_TERMINAL_TYPE, MD_WIRING_MODE, MD_FACTORY, barcode);
                }
                catch (Exception ex)
                {
                    MessageDisplay.Instance.Message = $"文件【{barcode}】保存失败\r\n" + ex;
                }

            }
        }


        private void Save_Excel(OneMeterResult meterResult, string MD_TERMINAL_TYPE, string MD_WIRING_MODE, string MD_FACTORY, string barcode)
        {

            Dictionary<string, DataTable> dataTab = GetDataTabelS2(meterResult);
            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook();
            Aspose.Cells.Worksheet sheet = wbook.Worksheets[0];
            sheet.Name = "数据";
            int row = 0;
            int col = 0;
            Aspose.Cells.Style style = wbook.Styles[wbook.Styles.Add()];
            Aspose.Cells.Style style2 = wbook.Styles[wbook.Styles.Add()];
            style.Pattern = Aspose.Cells.BackgroundType.Solid;
            style.Font.IsBold = true;
            style.Font.Color = System.Drawing.Color.FromArgb(0, 0, 255);
            style.Font.Name = "宋体";//文字字体 
            style.Font.Size = 10;//文字大小
            //style = sheet.Cells["A1"].GetStyle();


            style2.Pattern = Aspose.Cells.BackgroundType.Solid;
            style2.Font.IsBold = false;
            style2.Font.Name = "宋体";//文字字体 
            style2.Font.Size = 10;//文字大小
            style2.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin; //左边框 
            style2.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin; //右边框  
            style2.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin; //上边框  
            style2.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin; //下边框
            //style2 = sheet.Cells["A1"].GetStyle();

            Aspose.Cells.Style style3 = wbook.Styles[wbook.Styles.Add()];
            style3.Pattern = Aspose.Cells.BackgroundType.Solid;
            style3.Font.IsBold = true;
            style3.Font.Color = System.Drawing.Color.FromArgb(0, 0, 255);
            style3.Font.Name = "宋体";//文字字体 
            style3.Font.Size = 10;//文字大小
            style3.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin; //下边框

            sheet.Cells[row, 1].Value = "表类型";
            sheet.Cells[row, 1].PutValue("表类型");
            sheet.Cells[row, 1].SetStyle(style);

            sheet.Cells[row, 2].Value = "接线方式";
            sheet.Cells[row, 2].PutValue("接线方式");
            sheet.Cells[row, 2].SetStyle(style);

            sheet.Cells[row, 3].Value = "表厂家";
            sheet.Cells[row, 3].PutValue("表厂家");
            sheet.Cells[row, 3].SetStyle(style);

            sheet.Cells[row, 4].Value = "表条码";
            sheet.Cells[row, 4].PutValue("表条码");
            sheet.Cells[row, 4].SetStyle(style);

            row++;

            sheet.Cells[row, 1].Value = MD_TERMINAL_TYPE;
            sheet.Cells[row, 1].PutValue(MD_TERMINAL_TYPE);
            sheet.Cells[row, 1].SetStyle(style2);

            sheet.Cells[row, 2].Value = MD_WIRING_MODE;
            sheet.Cells[row, 2].PutValue(MD_WIRING_MODE);
            sheet.Cells[row, 2].SetStyle(style2);

            sheet.Cells[row, 3].Value = MD_FACTORY;
            sheet.Cells[row, 3].PutValue(MD_FACTORY);
            sheet.Cells[row, 3].SetStyle(style2);

            sheet.Cells[row, 4].Value = barcode;
            sheet.Cells[row, 4].PutValue(barcode);
            sheet.Cells[row, 4].SetStyle(style2);

            sheet.Cells[row, 5].Value = "";
            sheet.Cells[row, 5].PutValue("");
            sheet.Cells[row, 5].SetStyle(style2);

            sheet.Cells[row, 6].Value = "";
            sheet.Cells[row, 6].PutValue("");
            sheet.Cells[row, 6].SetStyle(style2);

            row++;


            sheet.Cells[row, 1].Value = "检测项目与分项结论";
            sheet.Cells[row, 1].PutValue("检测项目与分项结论");
            sheet.Cells[row, 1].SetStyle(style3);

            sheet.Cells[row, 2].Value = "时间";
            sheet.Cells[row, 2].PutValue("时间");
            sheet.Cells[row, 2].SetStyle(style3);

            sheet.Cells[row, 3].Value = "标准数据";
            sheet.Cells[row, 3].PutValue("标准数据");
            sheet.Cells[row, 3].SetStyle(style3);

            sheet.Cells[row, 4].Value = "终端数据";
            sheet.Cells[row, 4].PutValue("终端数据");
            sheet.Cells[row, 4].SetStyle(style3);

            sheet.Cells[row, 5].Value = "结论";
            sheet.Cells[row, 5].PutValue("结论");
            sheet.Cells[row, 5].SetStyle(style3);

            sheet.Cells[row, 6].Value = "";
            sheet.Cells[row, 6].PutValue("");
            sheet.Cells[row, 6].SetStyle(style3);

            row++;

            foreach (var item in dataTab.Keys)
            {
                DataTable table = dataTab[item];
                sheet.Cells[row, 1].Value = item;
                sheet.Cells[row, 1].PutValue(item);
                sheet.Cells[row, 1].SetStyle(style);

                row++;

                int num = 1;
                foreach (DataRow Row in table.Rows)
                {
                    sheet.Cells[row, 0].Value = num;
                    sheet.Cells[row, 0].PutValue(num);
                    sheet.Cells[row, 0].SetStyle(style2);
                    col = 1;

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        sheet.Cells[row, col].Value = Row[i].ToString().Trim();
                        sheet.Cells[row, col].PutValue(Row[i].ToString().Trim());
                        string a = Row[i].ToString().Trim();
                        sheet.Cells[row, col].SetStyle(style2);
                        //System.Threading.Thread.Sleep(2);
                        col++;
                    }
                    num++;
                    row++;
                }

            }
            sheet.AutoFitColumns();

            //限制最大列宽
            for (int i = 0; i < 10; i++)
            {
                //当前列宽
                int pixel = sheet.Cells.GetColumnWidthPixel(i);
                //设置最大列宽
                if (pixel > 240)
                {
                    sheet.Cells.SetColumnWidthPixel(i, 240);
                }
                else if (pixel < 40)
                    sheet.Cells.SetColumnWidthPixel(i, 40);
                else
                {
                    sheet.Cells.SetColumnWidthPixel(i, pixel);
                }
            }

            //System.IO.MemoryStream ms = wbook.SaveToStream();

            //string file = @"C:\Users\zhang\Desktop\excel\1.xls";
            if (!System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory() + @"\Res\Excel"))
            {
                System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + @"\Res\Excel");
                System.Threading.Thread.Sleep(500);
            }
            string Path2 = System.IO.Directory.GetCurrentDirectory() + @"\Res\Excel\" + DateTime.Now.ToString("yyyyMMdd");
            if (!System.IO.Directory.Exists(Path2))
            {
                System.IO.Directory.CreateDirectory(Path2);
                System.Threading.Thread.Sleep(500);
            }
            string name = meterResult.MeterInfo.GetProperty("MD_BAR_CODE") as string;
            if (name == null || name == "")
            {
                name = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            string file = Path2 + "\\" + name + ".xls";
            wbook.Save(file, Aspose.Cells.SaveFormat.Excel97To2003);
            System.Threading.Thread.Sleep(1000);
            MessageBox.Show($"文件路径:【{file}】保存成功");
            //System.Diagnostics.Process.Start(file); //打开excel文件
            return;




            #region MyRegion

            //DataTable[] dataTables = GetDataTabelS(meterResult);   //获取到这个表的所有项目的datatable

            //Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook();
            //Aspose.Cells.Worksheet sheet = wbook.Worksheets[0];
            //sheet.Name = "数据";
            //int row = 0;
            //int col = 0;
            //Aspose.Cells.Style style = wbook.Styles[wbook.Styles.Add()];
            //Aspose.Cells.Style style2 = wbook.Styles[wbook.Styles.Add()];
            //style.ForegroundColor = System.Drawing.Color.FromArgb(31, 78, 120);
            //style.Pattern = Aspose.Cells.BackgroundType.Solid;
            //style.Font.IsBold = true;
            //style.Font.Color = System.Drawing.Color.FromArgb(255, 255, 255);
            //style.Font.Name = "宋体";//文字字体 
            //style.Font.Size = 15;//文字大小
            //style.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin; //左边框 
            //style.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin; //右边框  
            //style.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin; //上边框  
            //style.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin; //下边框
            ////style = sheet.Cells["A1"].GetStyle();


            //style2.Pattern = Aspose.Cells.BackgroundType.Solid;
            //style2.Font.IsBold = false;
            //style2.Font.Name = "宋体";//文字字体 
            //style2.Font.Size = 15;//文字大小
            //style2.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin; //左边框 
            //style2.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin; //右边框  
            //style2.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin; //上边框  
            //style2.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin; //下边框
            ////style2 = sheet.Cells["A1"].GetStyle();

            //int Start_A = 0;
            //int Start_B = 0;
            //int End_A = 0;
            //int End_B = 0;
            ////cells.GetColumnWidthPixel
            //for (int t = 0; t < dataTables.Length; t++)
            //{
            //    DataTable table = dataTables[t];
            //    Start_A = row;
            //    Start_B = col;

            //    for (int i = 0; i < table.Columns.Count; i++) //列名
            //    {
            //        sheet.Cells[row, col].Value = table.Columns[i].ColumnName.ToString().Trim();
            //        sheet.Cells[row, col].PutValue(table.Columns[i].ColumnName.ToString().Trim());
            //        sheet.Cells[row, col].SetStyle(style);
            //        //sheet.Cells[row, col].
            //        col++;
            //    }
            //    End_B = col;
            //    col = 0;
            //    row++;
            //    int index = 1;
            //    End_A = 1;
            //    foreach (DataRow Row in table.Rows)
            //    {

            //        for (int i = 0; i < table.Columns.Count; i++)
            //        {
            //            sheet.Cells[row, col].Value = Row[i].ToString().Trim();
            //            sheet.Cells[row, col].PutValue(Row[i].ToString().Trim());
            //            if (index % 2 == 0)
            //            {
            //                style2.ForegroundColor = System.Drawing.Color.FromArgb(254, 255, 255);
            //            }
            //            else
            //            {
            //                style2.ForegroundColor = System.Drawing.Color.FromArgb(216, 230, 243);
            //            }
            //            sheet.Cells[row, col].SetStyle(style2);
            //            //System.Threading.Thread.Sleep(2);
            //            col++;
            //        }
            //        col = 0;
            //        row++;
            //        index++;
            //        End_A++; ;
            //    }



            //    var range = sheet.Cells.CreateRange(Start_A, Start_B, End_A, End_B);
            //    range.SetOutlineBorder(Aspose.Cells.BorderType.TopBorder, Aspose.Cells.CellBorderType.Thick, System.Drawing.Color.Blue);
            //    range.SetOutlineBorder(Aspose.Cells.BorderType.BottomBorder, Aspose.Cells.CellBorderType.Thick, System.Drawing.Color.Blue);
            //    range.SetOutlineBorder(Aspose.Cells.BorderType.LeftBorder, Aspose.Cells.CellBorderType.Thick, System.Drawing.Color.Blue);
            //    range.SetOutlineBorder(Aspose.Cells.BorderType.RightBorder, Aspose.Cells.CellBorderType.Thick, System.Drawing.Color.Blue);
            //    row++;
            //}


            //sheet.AutoFitColumns();


            ////限制最大列宽
            //for (int i = 0; i < 10; i++)
            //{
            //    //当前列宽
            //    int pixel = sheet.Cells.GetColumnWidthPixel(i);
            //    //设置最大列宽
            //    if (pixel > 180)
            //    {
            //        sheet.Cells.SetColumnWidthPixel(i, 180);
            //    }
            //    else if (pixel < 80)
            //        sheet.Cells.SetColumnWidthPixel(i, 100);
            //    else
            //    {
            //        sheet.Cells.SetColumnWidthPixel(i, pixel);
            //    }
            //    //if (i == 0)
            //    //{
            //    //    sheet.Cells.SetColumnWidthPixel(i, 150);

            //    //}
            //    //else
            //    //{ 
            //    //sheet.Cells.SetColumnWidthPixel(i, 120);

            //    //}

            //}
            ////System.IO.MemoryStream ms = wbook.SaveToStream();

            ////string file = @"C:\Users\zhang\Desktop\excel\1.xls";
            //if (!System.IO.Directory.Exists(System.IO.Directory.GetCurrentDirectory() + @"\Res\Excel"))
            //{
            //    System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + @"\Res\Excel");
            //    System.Threading.Thread.Sleep(500);
            //}
            //string file = System.IO.Directory.GetCurrentDirectory() + @"\Res\Excel\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            //wbook.Save(file, Aspose.Cells.SaveFormat.Excel97To2003);
            //System.Threading.Thread.Sleep(1000);
            //MessageBox.Show($"文件路径:【{file}】保存成功");
            ////System.Diagnostics.Process.Start(file); //打开excel文件
            #endregion

        }

        private Dictionary<string, DataTable> GetDataTabelS2(OneMeterResult meterResult)
        {
            Dictionary<string, DataTable> Resoults = new Dictionary<string, DataTable>();
            OneMeterResult Result = meterResult;
            string Type = "376";
            if (meterResult.MeterInfo.GetProperty("MD_PROTOCOL_TYPE") as string == "698.45")
            {
                Type = "698";
            }
            string[] data;
            string name = "";
            DataTable dataTable;
            List<string> colunmName = new List<string>() { "名称", "检定时间", "标准数据", "检定数据", "结论", "提示信息" };
            for (int i = 0; i < meterResult.Categories.Count; i++)
            {
                if (meterResult.Categories[i].Names.Count(item => item == "分项结论" + Type) > 0)
                {
                    for (int j = 0; j < meterResult.Categories[i].ResultUnits.Count; j++)
                    {
                        string str = meterResult.Categories[i].ResultUnits[j].GetProperty("分项结论" + Type) as string;
                        name = meterResult.Categories[i].ResultUnits[j].GetProperty("项目名") as string;
                        name += "(" + meterResult.Categories[i].ResultUnits[j].GetProperty("结论") as string + ")";
                        if (str != null && str != "")
                        {
                            data = str.Split('#');
                            dataTable = new DataTable();
                            for (int col = 0; col < colunmName.Count; col++)
                            {
                                dataTable.Columns.Add(colunmName[col]);//构建表头 
                            }
                            for (int s = 0; s < data.Length; s++)
                            {
                                string[] tem = data[s].Split('|');
                                if (tem.Length > 5)
                                {
                                    DataRow row = dataTable.NewRow();

                                    for (int d = 0; d < dataTable.Columns.Count; d++)
                                    {
                                        row[d] = tem[d];
                                    }
                                    dataTable.Rows.Add(row);

                                }
                            }
                            if (!Resoults.ContainsKey(name))
                            {
                                Resoults.Add(name, dataTable);
                            }
                        }
                    }
                }
                else   //没有分项结论的
                {
                    for (int j = 0; j < meterResult.Categories[i].ResultUnits.Count; j++)
                    {
                        name = meterResult.Categories[i].ResultUnits[j].GetProperty("项目号") as string;
                        name = GetIdNameValue(name.Split('_')[0]);
                        if (name == "") continue;
                        name += "(" + meterResult.Categories[i].ResultUnits[j].GetProperty("结论") as string + ")";
                        dataTable = new DataTable();
                        DataRow row2 = dataTable.NewRow();

                        for (int col = 0; col < meterResult.Categories[i].Names.Count; col++)
                        {
                            dataTable.Columns.Add(meterResult.Categories[i].Names[col]);//构建表头 
                        }

                        for (int col = 0; col < dataTable.Columns.Count; col++)
                        {
                            row2[col] = meterResult.Categories[i].ResultUnits[j].GetProperty(meterResult.Categories[i].Names[col]);
                        }
                        dataTable.Rows.Add(row2);
                        if (!Resoults.ContainsKey(name))
                        {
                            Resoults.Add(name, dataTable);
                        }
                        else
                        {
                            Resoults[name].Rows.Add(row2);
                        }
                    }
                }

            }
            return Resoults;

        }


        private string GetIdNameValue(string ID)
        {
            //初始化所有的结论都是合格，如果检定过程中有不合格就修改为不合格
            try
            {
                FieldInfo[] f_key = typeof(ProjectID).GetFields();
                for (int i = 0; i < f_key.Length; i++)
                {

                    var a = f_key[i].GetValue(new ProjectID()).ToString();
                    if (f_key[i].GetValue(new ProjectID()).ToString() == ID)
                    {
                        return f_key[i].Name;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

            }
            return "";
        }


        /// <summary>
        /// 获取表
        /// </summary>
        private DataTable[] GetDataTabelS(OneMeterResult meterResult)
        {
            if (meterResult == null)
            {
                return null;
            }
            DataTable[] dataTables = new DataTable[meterResult.Categories.Count + 1];

            DataTable dataTable = new DataTable();
            //先添加表数据
            for (int i = 0; i < datagridMeter.Columns.Count; i++)
            {
                dataTable.Columns.Add(datagridMeter.Columns[i].Header.ToString());//构建表头 
            }
            DataRow row2 = dataTable.NewRow();

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                row2[i] = meterResult.MeterInfo.GetProperty(datagridMeter.Columns[i].SortMemberPath);
            }
            dataTable.Rows.Add(row2);
            dataTables[0] = dataTable;


            for (int i = 0; i < meterResult.Categories.Count; i++)
            {
                dataTable = new DataTable();


                dataTable.Columns.Add("项目名");//构建表头 
                for (int j = 0; j < meterResult.Categories[i].Names.Count; j++)
                {
                    string columnName = meterResult.Categories[i].Names[j];
                    if (columnName == "要检" || columnName == "项目号" || columnName == "项目名")
                    {
                        continue;
                    }
                    dataTable.Columns.Add(columnName);//构建表头 
                }

                LYZD.ViewModel.Model.AsyncObservableCollection<DynamicViewModel> s = meterResult.Categories[i].ResultUnits;
                for (int q = 0; q < s.Count; q++)
                {
                    DataRow row = dataTable.NewRow();
                    for (int j = 0; j < dataTable.Columns.Count; j++)   //循环所有列
                    {
                        row[j] = s[q].GetProperty(dataTable.Columns[j].ColumnName) as string;
                    }
                    dataTable.Rows.Add(row);
                }
                dataTables[i + 1] = dataTable;
            }
            return dataTables;

        }
        #endregion

        private void IsCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBoxTemp = sender as CheckBox;
            if (checkBoxTemp != null)
            {
                DynamicViewModel modelTemp = checkBoxTemp.DataContext as DynamicViewModel;
                if (modelTemp != null)
                {
                    if (checkBoxTemp.IsChecked.HasValue && checkBoxTemp.IsChecked.Value)
                    {
                        modelTemp.SetProperty("IsSelected", true);
                    }
                    else
                    {
                        modelTemp.SetProperty("IsSelected", false);
                    }
                }
            }
        }
        private void test_Click(object sender, RoutedEventArgs e)
        {
            //DetailedData.Instance.Show();
        }

        private void Btn_UpData_Click1(object sender, RoutedEventArgs e)
        {
            Utility.Log.LogManager.AddMessage("不存在条码号为()的工单记录", Utility.Log.EnumLogSource.设备操作日志, Utility.Log.EnumLevel.Warning);
        }

        /// <summary>
        /// 上传数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_UpData_Click(object sender, RoutedEventArgs e)
        {
            var meters = viewModel.Meters.Where(item => ((bool)item.GetProperty("IsSelected")));
            int s = meters.Count();
            if (meters == null || meters.Count() == 0)
            {
                MessageBox.Show("请至少选择一块表!");
                return;
            }
            string Tips = "";
            foreach (DynamicViewModel meter in meters)
            {
                string barcode = meter.GetProperty("MD_BAR_CODE") as string;
                Tips += $"【{barcode}】\r\n";
            }
            if (MessageBox.Show("是否确定上传表：\r\n" + Tips + "数据", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }


            List<Core.Model.Meter.TestMeterInfo> temmeter = new List<Core.Model.Meter.TestMeterInfo>();
            foreach (DynamicViewModel meter in meters)
            {
                OneMeterResult meterResult = viewModel.ResultCollection.ItemsSource[meter.Index - 1];

                Core.Model.Meter.TestMeterInfo temmeter2 = MeterDataHelper.GetDnbInfoNew(meterResult);
                if (temmeter2 != null)
                {
                    temmeter.Add(temmeter2);
                }
            }


            Mis.Common.IMis miss = Mis.MISFactory.Create();
            miss.UpdateInit();
            if (miss.Update(temmeter))
            {
                foreach (DynamicViewModel meter in meters)
                {
                    meter.SetProperty("MD_OTHER_2", "已上传");
                    SetMeterData(meter);
                }
                MessageBox.Show("数据上传完成");
            }
            else
            {
                MessageBox.Show("数据插入到中间库失败");
            }

            #region

            //foreach (DynamicViewModel meter in meters)
            //{
            //    OneMeterResult meterResult = viewModel.ResultCollection.ItemsSource[meter.Index - 1];

            //    Core.Model.Meter.TestMeterInfo temmeter2 = MeterDataHelper.GetDnbInfoNew(meterResult);

            //    string barcode = meter.GetProperty("METER_ID") as string;
            //    Mis.Common.IMis mis = Mis.MISFactory.Create();
            //    mis.UpdateInit();
            //    //OneMeterResult meterResult = new OneMeterResult(meter.GetProperty("METER_ID").ToString(), false);

            //    //OneMeterResult meterResult = viewModel.ResultCollection.ItemsSource[meter.Index-1];

            //    //Core.Model.Meter.TestMeterInfo temmeter2 = MeterDataHelper.GetDnbInfoNew2(meterResult);
            //    Core.Model.Meter.TestMeterInfo m = new Core.Model.Meter.TestMeterInfo();
            //    m.Meter_ID = barcode;
            //    //Core.Model.Meter.TestMeterInfo temmeter = Mis.DataHelper.DataManage.GetDnbInfoNew(m, true);
            //    if (mis.Update(temmeter2))
            //    {
            //        Utility.Log.LogManager.AddMessage(string.Format("电能表[{0}]检定记录上传成功!", temmeter2.MD_BarCode), Utility.Log.EnumLogSource.数据库存取日志, Utility.Log.EnumLevel.Warning);
            //        MessageDisplay.Instance.Message = string.Format("电能表[{0}]检定记录上传成功!", temmeter2.MD_BarCode);
            //        meter.SetProperty("MD_OTHER_2", "已上传");
            //        SetMeterData(meter);

            //        mis.UpdateCompleted();
            //        //int updateCount = DAL.DALManager.MeterDbDal.Update("T_METER_INFO", "METER_ID", models, fieldNames);
            //        //modelTemp.SetProperty("MD_OTHER_2", "未上传");   //设置表位需要上传数据
            //    }
            //    else
            //    {
            //        Utility.Log.LogManager.AddMessage(string.Format("电能表[{0}]检定记录上传失败!", temmeter2.MD_BarCode), Utility.Log.EnumLogSource.数据库存取日志, Utility.Log.EnumLevel.TipsError);
            //        MessageDisplay.Instance.Message = string.Format("电能表[{0}]检定记录上传失败!", temmeter2.MD_BarCode);
            //    }

            //}

            #endregion
        }

        /// <summary>
        /// 修改数据库里面的上传标识
        /// </summary>
        /// <param name="meter"></param>
        private void SetMeterData(DynamicViewModel meter)
        {
            List<string> fieldNames = new List<string>() { "MD_OTHER_2" };//更新上传的状态
            List<DynamicModel> models = new List<DynamicModel>();
            DynamicModel model = new DynamicModel();
            model.SetProperty("METER_ID", meter.GetProperty("METER_ID"));
            model.SetProperty("MD_OTHER_2", meter.GetProperty("MD_OTHER_2"));
            models.Add(model);
            int updateCount = DALManager.MeterDbDal.Update("METER_INFO", "METER_ID", models, fieldNames);

        }

        private void Btn_Task_Click(object sender, RoutedEventArgs e)
        {
            var meters = viewModel.Meters.Where(item => ((bool)item.GetProperty("IsSelected")));
            int s = meters.Count();
            if (meters == null || meters.Count() == 0)
            {
                MessageBox.Show("请至少选择一块表!");
                return;
            }
            string Tips = "";
            string MD_TASK_NO = EquipmentData.LoadDETECT_TASK.DETECT_TASK_NO;
            foreach (DynamicViewModel meter in meters)
            {
                MD_TASK_NO = meter.GetProperty("MD_TASK_NO") as string;
                Tips = $"【{MD_TASK_NO}】\r\n";
            }
            if (MessageBox.Show("是否确定完成任务：\r\n" + Tips + "数据", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            Mis.Common.IMis mis = Mis.MISFactory.Create();
            mis.UpdateInit();

            mis.UpdateCompleted(MD_TASK_NO, EquipmentData.Equipment.ID);
        }

        /// <summary>
        /// 模板制作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Model_Click(object sender, RoutedEventArgs e)
        {
            Process[] processes = Process.GetProcesses();
            Process processTemp = processes.FirstOrDefault(item => item.ProcessName == "LYTest.WordTemplate.exe");
            if (processTemp == null)
            {
                try
                {
                    Process.Start(string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), "LYTest.WordTemplate.exe"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("模板编辑工具启动失败" + ex);
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 打印报告
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_PrintWord_Click(object sender, RoutedEventArgs e)
        {
            var meters = viewModel.Meters.Where(item => ((bool)item.GetProperty("IsSelected")));
            int s = meters.Count();
            if (meters == null || meters.Count() == 0)
            {
                MessageBox.Show("请至少选择一块表!");
                return;
            }
            string Tips = "";
            foreach (DynamicViewModel meter in meters)
            {
                string barcode = meter.GetProperty("MD_BAR_CODE") as string;
                Tips += $"【{barcode}】\r\n";
            }
            string msg = "是否确定打印表:";
            if (checkBoxPreview.IsChecked.HasValue && checkBoxPreview.IsChecked.Value)
            {
                msg = "是否确定预览表:";
            }
            if (MessageBox.Show(msg + "\r\n" + Tips + "数据", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            //查找对应模板的所有书签M_是表信息,D_是台体信息，R_是结论数据
            string path = string.Format(@"{0}\Res\Word\{1}", Directory.GetCurrentDirectory(), comboBoxTemplates.Text);
            if (!File.Exists(path))
            {
                MessageBox.Show("未找到模板文件:" + path);
            }

            foreach (DynamicViewModel meter in meters)
            {
                //OneMeterResult meterResult = new OneMeterResult(meter.GetProperty("METER_ID").ToString(), false);
                OneMeterResult meterResult = viewModel.ResultCollection.ItemsSource[meter.Index - 1];

                if (meterResult != null)
                {
                    Print(path, meterResult);
                }
            }


        }


        private void Btn_BatchCertifi_Click(object sender, RoutedEventArgs e)
        {
            string MD_BatchNo = viewModel.BackWhere1().Replace("'", "").Replace(" ", "").Split('=')[1];
            var meters = viewModel.Meters.Where(item => (string)item.GetProperty("MD_BATCH_NO") == MD_BatchNo);

            int s = meters.Count();
            if (meters == null || meters.Count() == 0)
            {
                MessageBox.Show("该批次没有数据");
                return;
            }
            
            string msg = "是否确定打印表:";
            if (MessageBox.Show(msg + "\r\n" + MD_BatchNo + "批次数据", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }

            //查找对应模板的所有书签M_是表信息,D_是台体信息，R_是结论数据
            string path = string.Format(@"{0}\Res\Word\{1}", Directory.GetCurrentDirectory(), comboBoxTemplates.Text);
            if (!File.Exists(path))
            {
                MessageBox.Show("未找到模板文件:" + path);
            }

            List<Core.Model.Meter.TestMeterInfo> temmeter = new List<Core.Model.Meter.TestMeterInfo>();
            foreach (DynamicViewModel meter in meters)
            {
                OneMeterResult meterResult = viewModel.ResultCollection.ItemsSource[meter.Index - 1];

                Core.Model.Meter.TestMeterInfo temmeter2 = MeterDataHelper.GetDnbInfoNew(meterResult);
                if (temmeter2 != null)
                {
                    temmeter.Add(temmeter2);
                }
            }

            Print(path, temmeter, MD_BatchNo);
        }

        private void Print(string path, List<Core.Model.Meter.TestMeterInfo> meterResult, string MD_BatchNo)
        {
            Document doc = new Document(path);
            if (doc == null)
            {
                MessageBox.Show(string.Format(@"未能找到对应 {0}的模板文件...", path), "打印失败");
                return;
            }
            Dictionary<string, string> DeviceValue = new Dictionary<string, string>();

            string DeviceDataPath = System.IO.Directory.GetCurrentDirectory() + "\\Ini\\DeviceData.ini";
            string[] NameS = LYZD.ViewModel.Const.OperateFile.GetINI("Data", "名称", DeviceDataPath).Split('|');
            string[] Values = LYZD.ViewModel.Const.OperateFile.GetINI("Data", "值", DeviceDataPath).Split('|');
            for (int i = 0; i < NameS.Length; i++)
            {
                if (!DeviceValue.ContainsKey(NameS[i]))
                {
                    DeviceValue.Add(NameS[i], Values[i]);
                }
            }
            string[] titles = new string[] { "序号", "故障类型", "出厂编号", "检定员", "检定时间" };
            foreach (Bookmark mark in doc.Range.Bookmarks)
            {
                DocumentBuilder builder = new DocumentBuilder(doc);
                string name = mark.Text; //书签的关键字
                if (name.StartsWith("M_")) //表信息
                {
                    //这里需要传入Mark和表的信息,替换Mark的text就可以
                    ReplaceMeterInfo(mark, meterResult);
                }
                if (name.StartsWith("T_"))
                {
                    string TestType = "全检";
                    if (name == "T_SotpFailureTableMark")
                    {
                        TestType = "抽检";
                    }
                    else if (name == "T_InspecFailureTableMark")
                    {
                        TestType = "全检";
                    }
                    #region 不合格表数据
                    List<ModelInfo> modelInfos = GetModelInfo(meterResult, TestType);
                    builder.MoveToBookmark(mark.Name);
                    builder.StartTable();
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center; // RowAlignment.Center;   
                    for (int i = 0; i < 5; i++)
                    {
                        builder.InsertCell();
                        //Table单元格边框线样式
                        builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                        //Table此单元格宽度
                        builder.CellFormat.Width = 600;
                        //此单元格中内容垂直对齐方式
                        builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                        builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.None;
                        builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;
                        //字体大小
                        builder.Font.Size = 10;
                        //是否加粗
                        builder.Bold = true;
                        //向此单元格中添加内容
                        builder.Write(titles[i]);
                    }
                    builder.EndRow();
                    if (modelInfos.Count==0) {
                        for (int i = 0; i < 5; i++)
                        {
                            for (int l = 0; l < 5; l++)
                            {
                                builder.InsertCell();
                                //Table单元格边框线样式
                                builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                                //Table此单元格宽度
                                builder.CellFormat.Width = 600;
                                //此单元格中内容垂直对齐方式
                                builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                                builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.None;
                                builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;
                                //字体大小
                                builder.Font.Size = 10;
                                //是否加粗
                                builder.Bold = true;
                                //向此单元格中添加内容
                                builder.Write("/");
                            }
                            builder.EndRow();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < modelInfos.Count; i++)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                builder.InsertCell();
                                builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                                builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                                builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.None;
                                builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;
                                //字体大小
                                builder.Font.Size = 10;
                                //是否加粗
                                builder.Bold = false;
                                if (j < 1)
                                {
                                    builder.Write(modelInfos[i].SerialNumber);
                                }
                                else if (j == 1)
                                {
                                    builder.Write(modelInfos[i].FaultType);
                                }
                                else if (j == 2)
                                {
                                    builder.Write(modelInfos[i].BarCode.ToString());
                                }
                                else if (j == 3)
                                {
                                    builder.Write(modelInfos[i].Verifier);
                                }
                                else if (j == 4)
                                {
                                    builder.Write(modelInfos[i].CHeckTime);
                                }

                            }
                            builder.EndRow();
                        }
                    }
                    
                    mark.Text = "";
                    #endregion
                }
                if (name.StartsWith("B_")) {
                    string TestType = "全检";
                    if (name== "B_SotpBarCodeTableMark") {
                        TestType = "抽检";
                    }
                    #region 条码表
                    builder.MoveToBookmark(mark.Name);
                    builder.StartTable();
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center; // RowAlignment.Center;
                    var FaillResult = meterResult.Where(item => item.MD_TestType == TestType);
                    #region 表头
                    for (int i = 0; i < 3; i++)
                    {
                        builder.InsertCell();
                        //Table单元格边框线样式
                        builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                        //Table此单元格宽度
                        builder.CellFormat.Width = 600;
                        //此单元格中内容垂直对齐方式
                        builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                        builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.None;
                        builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;
                        //字体大小
                        builder.Font.Size = 10;
                        //是否加粗
                        builder.Bold = true;
                        //向此单元格中添加内容
                        builder.Write("条形码");
                    }
                    builder.EndRow();
                    #endregion

                    if (FaillResult.Count()==0) {
                        for (int i = 0; i < 3; i++)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                builder.InsertCell();
                                //Table单元格边框线样式
                                builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                                //Table此单元格宽度
                                builder.CellFormat.Width = 600;
                                //此单元格中内容垂直对齐方式
                                builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                                builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.None;
                                builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;
                                //字体大小
                                builder.Font.Size = 10;
                                //是否加粗
                                builder.Bold = true;
                                //向此单元格中添加内容
                                builder.Write("——————");
                            }
                            builder.EndRow();
                        }
                    }
                    else
                    {
                        List<string> FaillResultList = new List<string>();
                        foreach (var item in FaillResult)
                        {
                            FaillResultList.Add(item.MD_BarCode);
                        }
                        List<List<string>> Tlist = SpiltList<string>(FaillResultList, (FaillResultList.Count / 3));

                        for (int i = 0; i < Tlist.Count; i++)
                        {
                            for (int j = 0; j < Tlist[i].Count; j++)
                            {
                                builder.InsertCell();
                                //Table单元格边框线样式
                                builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                                //Table此单元格宽度
                                builder.CellFormat.Width = 600;
                                //此单元格中内容垂直对齐方式
                                builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                                builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.None;
                                builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.None;
                                //字体大小
                                builder.Font.Size = 10;
                                //是否加粗
                                builder.Bold = true;
                                //向此单元格中添加内容
                                builder.Write(Tlist[i][j]);

                            }
                            builder.EndRow();
                        }
                    }
                    mark.Text = "";
                    #endregion
                }

            }

            Document docPrint = doc;
            string tmpPath = string.Format(@"{0}\Res\Word\Tem\{1}", Directory.GetCurrentDirectory(), "printTmp.doc");
            if (!File.Exists(tmpPath))
            {
                File.Create(path);
            }
            doc.Save(tmpPath, Aspose.Words.SaveFormat.Doc);
            docPrint.Print();
            docPrint.Clone();
        }

        private void Print(string path, OneMeterResult meterResult)
        {

            Document doc = new Document(path);
            if (doc == null)
            {
                MessageBox.Show(string.Format(@"未能找到对应 {0}的模板文件...", path), "打印失败");
                return;
            }
            Dictionary<string, string> DeviceValue = new Dictionary<string, string>();

            string DeviceDataPath = System.IO.Directory.GetCurrentDirectory() + "\\Ini\\DeviceData.ini";
            string[] NameS = LYZD.ViewModel.Const.OperateFile.GetINI("Data", "名称", DeviceDataPath).Split('|');
            string[] Values = LYZD.ViewModel.Const.OperateFile.GetINI("Data", "值", DeviceDataPath).Split('|');
            for (int i = 0; i < NameS.Length; i++)
            {
                if (!DeviceValue.ContainsKey(NameS[i]))
                {
                    DeviceValue.Add(NameS[i], Values[i]);
                }
            }
            foreach (Bookmark mark in doc.Range.Bookmarks)
            {
                string name = mark.Text; //书签的关键字
                if (name.StartsWith("M_")) //表信息
                {
                    //这里需要传入Mark和表的信息,替换Mark的text就可以
                    ReplaceMeterInfo(mark, meterResult);
                }
                else if (name.StartsWith("D_")) //台体信息
                {
                    ReplaceDeviceInfo(mark, DeviceValue);
                }
                else if (name.StartsWith("R_")) //结论数据
                {
                    ReplaceResult(mark, meterResult);
                }
            }
            //doc.AppendDocument(doc, ImportFormatMode.KeepSourceFormatting);

            Document docPrint = doc;

            //是否预览 --预览同一时间只能支持一个,而打印可以多个。所以临时文件分为俩个
            if (checkBoxPreview.IsChecked.HasValue && checkBoxPreview.IsChecked.Value)
            {
                //临时文件
                string tmpPath = string.Format(@"{0}\Res\Word\Tem\{1}", Directory.GetCurrentDirectory(), "tmp.doc");
                if (!File.Exists(tmpPath))
                {
                    File.Create(path);
                }
                if (!IsFileReady(tmpPath)) //预览的话判断一下文件是不是被占用了
                {
                    MessageBox.Show("预览文件同一时间只能打开一个,如需预览请关闭之前打开的预览文件,重新预览");
                    return;
                }
                doc.Save(tmpPath, Aspose.Words.SaveFormat.Doc);
                Process.Start(tmpPath);  //预览文件
            }
            else
            {
                //临时文件
                string tmpPath = string.Format(@"{0}\Res\Word\Tem\{1}", Directory.GetCurrentDirectory(), "printTmp.doc");
                if (!File.Exists(tmpPath))
                {
                    File.Create(path);
                }
                doc.Save(tmpPath, Aspose.Words.SaveFormat.Doc);
                docPrint.Print();
                docPrint.Clone();
            }
        }

        /// <summary>
        /// 判断文件是否占用
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private bool IsFileReady(string filepath)
        {
            if (File.Exists(filepath) == false)
            {
                return true;
            }
            try
            {
                File.Open(filepath, FileMode.Open).Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region 替换数据


        #region 批次数据
        private void ReplaceMeterInfo(Bookmark mark, List<Core.Model.Meter.TestMeterInfo> meterInfos)
        {
            string name = mark.Text.Substring(2);
            Core.Model.Meter.TestMeterInfo info = new Core.Model.Meter.TestMeterInfo();
            info = meterInfos[0];
            foreach (Core.Model.Meter.TestMeterInfo item in meterInfos)
            {
                if (item.Result == "合格")
                {
                    info = item;
                    break;
                }
            }
            switch (name)
            {
                case "MD_BatchNo":
                    mark.Text = info.MD_BatchNo;
                    break;
                case "MD_TerminalModel":
                    mark.Text = info.MD_TerminalType;
                    break;
                case "MD_Factory":
                    mark.Text = info.MD_Factory;
                    break;
                case "MD_UB":
                    mark.Text = "3*"+ info.MD_UB.ToString();
                    break;
                case "MD_UA":
                    mark.Text ="3*"+ info.MD_UA;
                    break;
                case "CheckDateTime":
                    DateTime dateTime = DateTime.MinValue;
                    foreach (var item in meterInfos)
                    {
                        if (!string.IsNullOrWhiteSpace(item.VerifyDate) && item.MD_TestType == "全检")
                        {
                            if (DateTime.Parse(item.VerifyDate) > dateTime)
                            {
                                dateTime = DateTime.Parse(item.VerifyDate);
                            }
                        }
                    }
                    mark.Text = dateTime.ToString("yyyy/MM/dd");
                    break;
                case "MD_TEST_PERSON":
                    string MD_TEST_PERSON = info.Checker1;
                    mark.Text = MD_TEST_PERSON;
                    break;
                case "MeterNumber":
                    mark.Text = meterInfos.Count.ToString();
                    break;
                case "InspectionNumber":
                    int InspectionNumber = meterInfos.Where(item => item.MD_TestType == "全检").Count();
                    mark.Text = InspectionNumber.ToString();
                    break;
                case "SpotCheckMeterNumber":
                    int SpotCheckMeterNumber = meterInfos.Where(item => item.MD_TestType == "抽检").Count();
                    mark.Text = SpotCheckMeterNumber.ToString();
                    break;
                case "Pass":
                    int Pass = meterInfos.Where(item => item.MD_TestType == "全检" && item.Result == "合格").Count();
                    mark.Text = Pass.ToString();
                    break;
                case "Failure":
                    int Failure = meterInfos.Where(item => item.MD_TestType == "全检" && item.Result == "不合格").Count();
                    mark.Text = Failure.ToString();
                    break;
                case "FailureRate":
                    InspectionNumber = meterInfos.Where(item => item.MD_TestType == "全检").Count();
                    Failure = meterInfos.Where(item => item.MD_TestType == "全检" && item.Result == "不合格").Count();
                    if (InspectionNumber == 0 || Failure == 0)
                    {
                        mark.Text = "0";
                    }
                    else
                    {
                        mark.Text = (Failure / InspectionNumber * 100.0f).ToString();
                    }
                    break;
                case "PassRate":
                    InspectionNumber = meterInfos.Where(item => item.MD_TestType == "全检").Count();
                    Pass = meterInfos.Where(item => item.MD_TestType == "全检" && item.Result == "合格").Count();
                    if (InspectionNumber == 0 || Pass == 0)
                    {
                        mark.Text = "0";
                    }
                    else
                    {
                        mark.Text = (Pass / InspectionNumber * 100.0f).ToString();
                    }
                    break;
                case "SpotcheckPassNumber":
                    int SpotcheckPassNumber = meterInfos.Where(item => item.MD_TestType == "抽检" && item.Result == "合格").Count();
                    mark.Text = SpotcheckPassNumber.ToString(); ;
                    break;
                case "SpotcheckFailureNumber":
                    int SpotcheckFailureNumber = meterInfos.Where(item => item.MD_TestType == "抽检" && item.Result == "不合格").Count();
                    mark.Text = SpotcheckFailureNumber.ToString();
                    break;
                case "SpotcheckPassNumberRata":
                    SpotCheckMeterNumber = meterInfos.Where(item => item.MD_TestType == "抽检").Count();
                    SpotcheckPassNumber = meterInfos.Where(item => item.MD_TestType == "抽检" && item.Result == "合格").Count();
                    if (SpotCheckMeterNumber == 0 || SpotcheckPassNumber == 0)
                    {
                        mark.Text = "0";
                    }
                    else
                    {
                        mark.Text = (SpotcheckPassNumber / SpotCheckMeterNumber * 100.0f).ToString();
                    }
                    break;
                case "SpotcheckFailureNumberRate":
                    SpotCheckMeterNumber = meterInfos.Where(item => item.MD_TestType == "抽检").Count();
                    SpotcheckFailureNumber = meterInfos.Where(item => item.MD_TestType == "抽检" && item.Result == "不合格").Count();
                    if (SpotCheckMeterNumber == 0 || SpotcheckFailureNumber == 0)
                    {
                        mark.Text = "0";
                    }
                    else
                    {
                        mark.Text = (SpotcheckFailureNumber / SpotCheckMeterNumber * 100.0f).ToString();
                    }
                    break;
                case "SpotCheckReslut":
                    SpotCheckMeterNumber = meterInfos.Where(item => item.MD_TestType == "抽检").Count();
                    SpotcheckFailureNumber = meterInfos.Where(item => item.MD_TestType == "抽检" && item.Result == "不合格").Count();
                    mark.Text = RetSpotReslut(SpotCheckMeterNumber, SpotcheckFailureNumber);
                    break;
                case "LastCheckTime":
                    dateTime = DateTime.MinValue;
                    foreach (var item in meterInfos)
                    {
                        if (!string.IsNullOrWhiteSpace(item.VerifyDate)&&item.MD_TestType=="抽检")
                        {
                            if (DateTime.Parse(item.VerifyDate)> dateTime) {
                                dateTime = DateTime.Parse(item.VerifyDate);
                            }
                        }
                    }
                    mark.Text = dateTime.ToString("yyyy/MM/dd");
                    break;
            }
        }

        private string RetSpotReslut(int allSpotNumber, int SpotcheckFailureNumber)
        {
            if (allSpotNumber < 50)
            {
                if (SpotcheckFailureNumber > 0)
                {
                    return "不合格";
                }
                else
                {
                    return "合格";
                }
            }
            else if (allSpotNumber >= 50 && allSpotNumber < 80)
            {
                if (SpotcheckFailureNumber >= 1)
                {
                    return "不合格";
                }
                else
                {
                    return "合格";
                }
            }
            else if (allSpotNumber >= 80 && allSpotNumber < 125)
            {
                if (SpotcheckFailureNumber >= 2)
                {
                    return "不合格";
                }
                else
                {
                    return "合格";
                }
            }
            else if (allSpotNumber >= 125 && allSpotNumber < 200)
            {
                if (SpotcheckFailureNumber >= 3)
                {
                    return "不合格";
                }
                else
                {
                    return "合格";
                }
            }
            else if (allSpotNumber >= 200 && allSpotNumber < 315)
            {
                if (SpotcheckFailureNumber >= 5)
                {
                    return "不合格";
                }
                else
                {
                    return "合格";
                }
            }
            else if (allSpotNumber >= 315 && allSpotNumber < 500)
            {
                if (SpotcheckFailureNumber >= 7)
                {
                    return "不合格";
                }
                else
                {
                    return "合格";
                }
            }
            else if (allSpotNumber >= 500 && allSpotNumber < 800)
            {
                if (SpotcheckFailureNumber >= 10)
                {
                    return "不合格";
                }
                else
                {
                    return "合格";
                }
            }
            else if (allSpotNumber >= 800 && allSpotNumber < 1250)
            {
                if (SpotcheckFailureNumber >= 14)
                {
                    return "不合格";
                }
                else
                {
                    return "合格";
                }
            }
            else
            {
                if (SpotcheckFailureNumber >= 21)
                {
                    return "不合格";
                }
                else
                {
                    return "合格";
                }
            }

        }
        #endregion
        /// <summary>
        /// 替换表信息
        /// </summary>
        /// <param name="mark"></param>
        /// <param name="meterResult"></param>
        private void ReplaceMeterInfo(Bookmark mark, OneMeterResult meterResult)
        {
            DynamicViewModel meterInfo = meterResult.MeterInfo;

            string name = mark.Text.Substring(2);
            string HandelString = "";
            if (name.Substring(name.LastIndexOf('_') + 1).IndexOf("$") != -1) //判断是否需要代码解析
            {
                HandelString = name.Substring(name.LastIndexOf('_') + 1);
                name = name.Substring(0, name.LastIndexOf('_'));//排除代码那部分
            }
            var s = meterInfo.GetProperty(name);
            string value = "";
            if (s != null)
            {
                value = s.ToString();
            }
            if (HandelString != "")
            {
                try
                {
                    value = StringHandel(value, HandelString);
                }
                catch (Exception ex)
                {
                    MessageDisplay.Instance.Message = string.Format($"书签{mark.Text}字符串处理时失败  " + ex.ToString());
                }

            }
            mark.Text = value;

        }
        /// <summary>
        /// 替换台体信息
        /// </summary>
        /// <param name="mark"></param>
        /// <param name="meterResult"></param>
        private void ReplaceDeviceInfo(Bookmark mark, Dictionary<string, string> DeviceValue)
        {
            string name = mark.Text.Substring(2);
            string value = "";
            if (DeviceValue.ContainsKey(name))
            {
                value = DeviceValue[name];
            }
            mark.Text = value;
        }

        /// <summary>
        /// 替换结论数据
        /// </summary>
        /// <param name="mark"></param>
        /// <param name="meterResult"></param>
        private void ReplaceResult2(Bookmark mark, OneMeterResult meterResult)
        {
            string value = "";
            string tem = mark.Text.Substring(2);
            if (tem.Substring(tem.LastIndexOf('_') + 1).IndexOf("$") != -1) //判断是否需要代码解析
            {
                //先获得解析的代码
                string[] data = tem.Substring(tem.LastIndexOf('_') + 1).Split('$');
                tem = tem.Substring(0, tem.LastIndexOf('_'));//排除代码那部分
                string ID = tem.Substring(0, tem.LastIndexOf('_'));
                string Name = tem.Substring(tem.LastIndexOf('_') + 1);
                bool T = false;
                for (int i = 0; i < meterResult.Categories.Count; i++)
                {
                    OneMeterResult.ViewCategory view = meterResult.Categories[i];
                    for (int j = 0; j < view.ResultUnits.Count; j++)
                    {
                        var s = view.ResultUnits[j].GetProperty("项目号");
                        string id = "";
                        if (s != null)
                        {
                            id = s.ToString();
                        }
                        if (id == ID)
                        {
                            s = view.ResultUnits[i].GetProperty(Name);
                            if (s != null)
                            {
                                value = s.ToString();
                            }
                            T = true;
                            break;
                        }
                    }
                    if (T) break;
                }
                mark.Text = value;
                //先判断解析的格式等一系列是否正常
                if (data[0] != "" && data[1] != "")
                {
                    int v = 0;
                    if (int.TryParse(data[1], out v))
                    {
                        string[] temp = value.Split(data[0].ToCharArray());
                        if (v < temp.Length)
                        {
                            mark.Text = value.Split(data[0].ToCharArray())[v];
                        }
                    }
                }
            }
            //这个是旧的，为了不影响老的功能暂时不合并单独写
            else
            {
                string ID = tem.Substring(0, tem.LastIndexOf('_'));
                string Name = tem.Substring(tem.LastIndexOf('_') + 1);
                bool T = false;
                for (int i = 0; i < meterResult.Categories.Count; i++)
                {
                    OneMeterResult.ViewCategory view = meterResult.Categories[i];
                    for (int j = 0; j < view.ResultUnits.Count; j++)
                    {
                        var s = view.ResultUnits[j].GetProperty("项目号");
                        string id = "";
                        if (s != null)
                        {
                            id = s.ToString();
                        }
                        if (id == ID)
                        {
                            s = view.ResultUnits[i].GetProperty(Name);
                            if (s != null)
                            {
                                value = s.ToString();
                            }
                            T = true;
                            break;
                        }
                    }
                    if (T) break;
                }
                mark.Text = value;

            }


        }



        /// <summary>
        /// 替换结论数据
        /// </summary>
        /// <param name="mark"></param>
        /// <param name="meterResult"></param>
        private void ReplaceResult(Bookmark mark, OneMeterResult meterResult)
        {
            string value = "";
            string tem = mark.Text.Substring(2);
            string HandelString = "";
            if (tem.Substring(tem.LastIndexOf('_') + 1).IndexOf("$") != -1) //判断是否需要代码解析
            {
                HandelString = tem.Substring(tem.LastIndexOf('_') + 1);
                tem = tem.Substring(0, tem.LastIndexOf('_'));//排除代码那部分
            }

            string ID = tem.Substring(0, tem.LastIndexOf('_'));
            string Name = tem.Substring(tem.LastIndexOf('_') + 1);
            bool T = false;
            for (int i = 0; i < meterResult.Categories.Count; i++)
            {
                OneMeterResult.ViewCategory view = meterResult.Categories[i];
                for (int j = 0; j < view.ResultUnits.Count; j++)
                {
                    var s = view.ResultUnits[j].GetProperty("项目号");
                    string id = "";
                    if (s != null)
                    {
                        id = s.ToString();
                    }
                    if (id == ID)
                    {
                        s = view.ResultUnits[i].GetProperty(Name);
                        if (s != null)
                        {
                            value = s.ToString();
                        }
                        T = true;
                        break;
                    }
                }
                if (T) break;
            }
            if (HandelString != "")
            {
                try
                {
                    value = StringHandel(value, HandelString);
                }
                catch (Exception ex)
                {
                    MessageDisplay.Instance.Message = string.Format($"书签{mark.Text}字符串处理时失败  " + ex.ToString());
                }

            }
            mark.Text = value;
        }


        /// <summary>
        /// 字符串处理
        /// </summary>
        /// <param name="value">现在的值</param>
        /// <param name="str">处理方式</param>
        /// <returns></returns>
        private string StringHandel(string value, string handelStr)
        {
            string str = handelStr.Substring(1, 1);  //&Tyyyy年MM月dd日  &V(|)    &S(|,,1)
            string format = handelStr.Substring(2);//字符串格式，根据需要在分割
            string temValue = value;
            switch (str)
            {
                case "T"://日期格式化  tostring
                    temValue = Convert.ToDateTime(value).ToString(format);
                    break;
                case "S"://字符串分割  splite('/')[0]
                         //以最后一个符号作为分割，前面的是分割的字符串，后面的是下标
                    int tem1 = int.Parse(format.Substring(format.LastIndexOf('|') + 1));
                    string tem2 = format.Substring(0, format.LastIndexOf('|'));
                    temValue = value.Split(tem2.ToCharArray())[tem1];
                    break;
                default:
                    break;
            }
            return temValue;
        }
        #endregion
        #region 公式计算

        /// <summary>
        /// 计算公式 
        /// </summary>
        private void FormulaCalculation(Document doc)
        {

            NodeCollection node = doc.GetChildNodes(NodeType.Table, true);
            for (int i = 0; i < node.Count; i++)   //便利文档中所有的表格
            {
                Table table = (Table)node[i];
                foreach (Bookmark mark in table.Range.Bookmarks)
                {
                    string name = mark.Text; //书签的关键字
                    if (name.StartsWith("=AVERAGE")) //计算平均值
                    {
                        //这里需要传入Mark和表的信息,替换Mark的text就可以
                        CalculationAVERAGE(table, mark);
                    }
                    else if (name.StartsWith("=SUM"))
                    {
                        CalculationSUM(table, mark);
                    }

                }
            }
        }

        /// <summary>
        /// 计算平均值
        /// </summary>
        /// <param name="mark"></param>
        private void CalculationAVERAGE(Aspose.Words.Tables.Table table, Bookmark mark)
        {
            //=AVERAGE(a1,a2)
            //mark.Text = "=AVERAGE(A1:a3,b2)";
            string tem = mark.Text.Substring(mark.Text.IndexOf("(") + 1).TrimEnd(')');    //中间的值

            //逗号代表单个的单元格，冒号代表连续的
            List<string> values = GetCellValueList(table, tem);

            double sum = 0;
            for (int i = 0; i < values.Count; i++)
            {
                if (double.TryParse(values[i], out double value))
                {
                    sum += value;
                }
            }
            mark.Text = (sum / values.Count).ToString("f2");
        }
        /// <summary>
        /// 计算和
        /// </summary>
        /// <param name="mark"></param>
        private void CalculationSUM(Aspose.Words.Tables.Table table, Bookmark mark)
        {
            //=AVERAGE(a1,a2)
            //mark.Text = "=AVERAGE(A1:a3,b2)";
            string tem = mark.Text.Substring(mark.Text.IndexOf("(") + 1).TrimEnd(')');    //中间的值

            //逗号代表单个的单元格，冒号代表连续的
            List<string> values = GetCellValueList(table, tem);

            double sum = 0;
            for (int i = 0; i < values.Count; i++)
            {
                if (double.TryParse(values[i], out double value))
                {
                    sum += value;
                }
            }
            mark.Text = sum.ToString("f2");
        }

        #region 单元格的方法

        private string GetCellValue(Table table, int row, int cell)
        {
            string b = "0";
            try
            {
                b = table.Rows[row - 1].Cells[cell - 1].ToTxt().ToString().Trim('\n').Trim('\r').Trim();
            }
            catch (Exception)
            {

            }
            return b;

        }
        /// <summary>
        ///获取单元格坐标
        /// </summary>
        /// <returns></returns>
        private bool GetCellIndex(string str, ref int rowIndex, ref int cellIndex)
        {
            if (str.Length < 2)
            {
                return false;
            }
            //string cell = str.Substring(0, 1);//第几列
            char cell = str[0];
            string row = str.Substring(1); //第几行

            try
            {
                cellIndex = Convert.ToInt32(cell) - 96;  //计算是第几列
                rowIndex = int.Parse(row);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 传入一个字符串，返回 字符串代表的所有单元格的值
        /// </summary>
        /// <returns></returns>
        private List<string> GetCellValueList(Aspose.Words.Tables.Table table, string str)
        {
            List<string> value = new List<string>();
            string[] CellData = str.ToLower().Split(',');

            for (int i = 0; i < CellData.Length; i++)
            {

                if (CellData[i].Trim() == "") continue;
                string[] data = CellData[i].Split(':'); //开始区域和结束区域，是一个矩形区域 a1:a2

                if (data.Length == 1)//这个代表一个单元格
                {
                    int row1 = 0;
                    int cell1 = 0;
                    if (GetCellIndex(data[0], ref row1, ref cell1))
                    {
                        value.Add(GetCellValue(table, row1, cell1));
                    }
                }
                else if (data.Length == 2)//这个代表一个矩形区域
                {
                    int row1 = 0;
                    int cell1 = 0;
                    int row2 = 0;
                    int cell2 = 0;
                    //开始和结束区域
                    if (GetCellIndex(data[0], ref row1, ref cell1) && GetCellIndex(data[1], ref row2, ref cell2))
                    {

                        for (int r = row1; r <= row2; r++)
                        {
                            for (int c = cell1; c <= cell2; c++)
                            {
                                value.Add(GetCellValue(table, r, c));
                            }
                        }
                    }
                }
                //根据列和行取出单元格的值，转数字--默认0
                //求和-求平均值-
                //应该先分割字符串-在判断公式-因为还有求和-求减的公式
            }
            return value;
        }

        #endregion

        private string GetName(string ProjectId)
        {
            switch (ProjectId)
            {
                case ProjectID.读取终端信息:
                    return "读取终端信息";
                case ProjectID.时钟召测和对时:
                    return "时钟召测和对时";
                case ProjectID.日计时误差:
                    return "日计时误差";
                case ProjectID.基本参数:
                    return "基本参数";
                case ProjectID.抄表与费率参数:
                    return "抄表与费率参数";
                case ProjectID.事件参数:
                    return "事件参数";
                case ProjectID.状态量采集:
                    return "状态量采集";
                case ProjectID.电能表当前数据:
                    return "电能表当前数据";
                case ProjectID.电能表实时数据:
                    return "电能表实时数据";
                case ProjectID.分脉冲量采集12个:
                    return "12个/分钟脉冲采集";
                case ProjectID.终端停_上电事件:
                    return " 终端停上电事件";
                case ProjectID.时段功控:
                    return "时段功控";
                case ProjectID.购电控:
                    return "购电控";
                case ProjectID.终端密钥恢复:
                    return "终端密钥恢复";
                case ProjectID.密钥下装:
                    return "密钥下装";
                case ProjectID.终端维护:
                    return "终端维护";
                case ProjectID.交采电量清零:
                    return "交采电量清零";
                case ProjectID.电压合格率统计:
                    return "电压合格率统计";
            }
            return "无";
        }
        /// <summary>
        /// 获取打印模型
        /// </summary>
        /// <param name="meterResult">结论集合</param>
        /// <param name="MD_TestType">全检/抽检</param>
        /// <returns></returns>
        private List<ModelInfo> GetModelInfo(List<Core.Model.Meter.TestMeterInfo> meterResult, string MD_TestType)
        {
            Dictionary<string, StringBuilder> dic = new Dictionary<string, StringBuilder>();
            foreach (Core.Model.Meter.TestMeterInfo TestMeterInfoitem in meterResult)
            {
                if (TestMeterInfoitem.MD_TestType == MD_TestType)
                {
                    foreach (var Ritem in TestMeterInfoitem.MeterResoultData)
                    {
                        if (Ritem.Value.Result == "不合格")
                        {
                            string KeyName = GetName(Ritem.Key);
                            if (KeyName == "无")
                            {
                                continue;
                            }
                            if (dic.ContainsKey(KeyName))
                            {
                                StringBuilder outValue;
                                dic.TryGetValue(KeyName, out outValue);
                                dic[KeyName] = outValue.Append("|" + TestMeterInfoitem.MD_BarCode);
                            }
                            else
                            {
                                dic.Add(KeyName, new StringBuilder(TestMeterInfoitem.MD_BarCode));
                            }
                        }
                    }
                }
            }

            List<ModelInfo> modelInfos = new List<ModelInfo>();
            int SerialNumber = 1;
            foreach (var item in dic)
            {
                ModelInfo modelInfo = new ModelInfo();
                modelInfo.SerialNumber = SerialNumber.ToString();
                modelInfo.FaultType = item.Key;
                Core.Model.Meter.TestMeterInfo TestMeterInfoitem = meterResult.Where(x => x.MD_BarCode == item.Value.ToString().Split('|')[0]).FirstOrDefault();
                modelInfo.CHeckTime = DateTime.Parse(TestMeterInfoitem.VerifyDate).ToString("yyyy/MM/dd");
                modelInfo.BarCode = item.Value.Replace("|", "\n");
                modelInfo.Verifier = TestMeterInfoitem.Checker1;
                modelInfos.Add(modelInfo);
                SerialNumber++;
            }
            return modelInfos;
        }

        /// <summary>
        /// 按指定数量均分
        /// </summary>
        /// <returns></returns>
        private static List<List<T>> SpiltList<T>(List<T> Lists, int num)
        {
            List<List<T>> fz = new List<List<T>>();
            //元素数量大于等于 分组数量
            if (Lists.Count >= num&& num!=0)
            {
                int avg = Lists.Count / num; //每组数量
                int vga = Lists.Count % num; //余数
                for (int i = 0; i < num; i++)
                {
                    List<T> cList = new List<T>();
                    if (i + 1 == num)
                    {
                        cList = Lists.Skip(avg * i).ToList<T>();
                    }
                    else
                    {
                        cList = Lists.Skip(avg * i).Take(avg).ToList<T>();
                    }
                    fz.Add(cList);
                }
            }
            else
            {
                //最后一组 数量<=num
                fz.Add(Lists);//元素数量小于分组数量
            }
            return fz;
        }
        #endregion

        
    }
}

