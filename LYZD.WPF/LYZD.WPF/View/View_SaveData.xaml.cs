using LYZD.Core.Enum;
using LYZD.Core.Struct;
using LYZD.DAL.Config;
using LYZD.DAL.DataBaseView;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckInfo;
using LYZD.ViewModel.User;
using LYZD.WPF.Converter;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Aspose.Cells;
using System.Windows.Input;

namespace LYZD.WPF.View
{
    /// <summary>
    /// 审核存盘界面
    /// </summary>
    public partial class View_SaveData
    {
        public View_SaveData()
        {
            InitializeComponent();
            Name = "审核存盘";
            DockStyle.IsFloating = true; //是否开始是全屏  
            DockStyle.IsMaximized = true;

            DockStyle.FloatingSize = SystemParameters.WorkArea.Size;
            LoadColumns();
            Random ran = new Random();
         
            textBoxTemperature.Text = NextDouble(ran, ConfigHelper.Instance.Temperature).ToString("F1"); ; ;// Properties.Settings.Default.Temperature;
            textBoxHumidy.Text = NextDouble(ran, ConfigHelper.Instance.Humidity).ToString("F1");// = Properties.Settings.Default.Humidy;
            textBoxValidate.Text = ConfigHelper.Instance.TestEffectiveTime.ToString();// Properties.Settings.Default.ValidateTime.ToString();
            treeSchema1.DataContext = EquipmentData.CheckResults;
            datagridMeters.SelectedIndex = 0;
            LoadMeterInfo();
            LoadUsers();
        }
        public double NextDouble(Random ran, double minValue)
        {
            return ran.NextDouble() * (5) + minValue;
        }
        /// <summary>
        /// 所有表的结论
        /// </summary>
        private AllMeterResult viewModel
        {
            get { return Resources["AllMeterResult"] as AllMeterResult; }
        }
        /// <summary>
        /// 结论总览加载表信息列
        /// </summary>
        private void LoadColumns()
        {
            GridViewColumnCollection columns = Application.Current.Resources["ColumnCollectionSave"] as GridViewColumnCollection;
            while (columns.Count > 2)
            {
                columns.RemoveAt(2);
            }
            for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
            {
                GridViewColumn column = new GridViewColumn
                {
                    Header = string.Format("表位{0}", i + 1),
                    //DisplayMemberBinding = new Binding(string.Format("ResultSummary.表位{0}.ResultValue", i + 1)),
                    Width = 58,
                };
                #region 动态模板
                DataTemplate dataTemplateTemp = new DataTemplate();
                FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock), "textBlock");
                //上下文
                Binding bindingDataContext = new Binding(string.Format("ResultSummary.表位{0}", i + 1));
                factory.SetBinding(TextBlock.DataContextProperty, bindingDataContext);
                //文本
                Binding bindingText = new Binding("ResultValue");
                factory.SetBinding(TextBlock.TextProperty, bindingText);
                dataTemplateTemp.VisualTree = factory;
                Binding bindingColor = new Binding("Result");
                bindingColor.Converter = new ResultColorConverter();
                factory.SetBinding(TextBlock.ForegroundProperty, bindingColor);
                column.CellTemplate = dataTemplateTemp;
                #endregion
                columns.Add(column);
            }
        }
        /// <summary>
        /// 加载结论对应的表格
        /// </summary>
        private void LoadMeterDataGrids(OneMeterResult meterResult)
        {
            if (meterResult == null)
            {
                return;
            }
            resultContainer.Children.Clear();
            List<string> list = ItemResoultkeyword.GetSpliteList();  //排除关键字
            for (int i = 0; i < meterResult.Categories.Count; i++)
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
                    if (columnName == "要检" || columnName == "项目名" || columnName == "项目号" || columnName == "分项结论" || list .Contains(columnName))
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
                resultContainer.Children.Add(dataGrid);
            }
        }
        /// <summary>
        /// 选中表发生变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OneMeterResult meterResult = datagridMeters.SelectedItem as OneMeterResult;
            LoadMeterDataGrids(meterResult);
        }
        /// <summary>
        /// 加载表基本信息
        /// </summary>
        private void LoadMeterInfo()
        {
            //800是参数录入对应的列
            Dictionary<string, string> dictionaryColumn = ResultViewHelper.GetPkDisplayDictionary("800");
            foreach (string fieldName in dictionaryColumn.Keys)
            {
                if (fieldName == "MD_CHECKED")
                {
                    continue;
                }
                Grid gridTemp = new Grid()
                {
                    Margin = new Thickness(2),
                };
                while (gridTemp.ColumnDefinitions.Count < 2)
                {
                    gridTemp.ColumnDefinitions.Add(
                        new ColumnDefinition()
                        {
                            Width = new GridLength(1, GridUnitType.Star)
                        });
                }
                gridTemp.ColumnDefinitions[0].Width = new GridLength(70);
                TextBlock textBlockName = new TextBlock()
                {
                    Text = dictionaryColumn[fieldName],
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(3, 0, 3, 0)
                };
                TextBlock textBlockValue = new TextBlock()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(3, 0, 3, 0)
                };
                textBlockValue.SetBinding(TextBlock.TextProperty, new Binding(string.Format("MeterInfo.{0}", fieldName)));
                Grid.SetColumn(textBlockValue, 1);
                gridTemp.Children.Add(textBlockName);
                gridTemp.Children.Add(textBlockValue);
                stackPanelMeterInfo.Children.Add(gridTemp);
            }
        }
        /// <summary>
        /// 加载用户
        /// </summary>
        private void LoadUsers()
        {
            List<string> userNames = UserViewModel.Instance.GetList("");
            comboBoxAuditor.ItemsSource = userNames;
            comboBoxAuditor.SelectedItem = EquipmentData.LastCheckInfo.AuditPerson;

            comboBoxBoss.ItemsSource = userNames;

            comboBoxTester.ItemsSource = userNames;
            comboBoxTester.SelectedItem = EquipmentData.LastCheckInfo.TestPerson;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //IMis mis2 = MISFactory.Create();
            //mis2.UpdateCompleted();


            //温度,湿度,批准人,检验员,核验员,有效期,检定日期
            string[] arrayField = new string[]
                {
                    "MD_TEMPERATURE",
                    "MD_HUMIDITY",
                    "MD_SUPERVISOR",
                    "MD_TEST_PERSON",
                    "MD_AUDIT_PERSON",
                    "MD_VALID_DATE",
                    "MD_TEST_DATE"
                };
            //Properties.Settings.Default.Temperature = textBoxTemperature.Text;
            //Properties.Settings.Default.Humidy = textBoxHumidy.Text;
            int intTemp = int.Parse(ConfigHelper.Instance.TestEffectiveTime);
            int.TryParse(textBoxValidate.Text, out intTemp);
     
            //Properties.Settings.Default.Save();
            //有效期
            string stringValidDate = DateTime.Now.AddMonths(intTemp).ToString();
            string strTestDate = DateTime.Now.ToString();
            string[] arrayValue = new string[]
                {
                    textBoxTemperature.Text,
                    textBoxHumidy.Text,
                    comboBoxBoss.SelectedValue as string,
                    comboBoxTester.Text,
                    comboBoxAuditor.SelectedValue as string,
                    stringValidDate,
                    strTestDate,
                };
            bool[] yaojianTemp = EquipmentData.MeterGroupInfo.YaoJian;
            List<string> sqlList = new List<string>();
            for (int i = 0; i < EquipmentData.MeterGroupInfo.Meters.Count; i++)
            {
                if (yaojianTemp[i])
                {
                    for (int j = 0; j < arrayField.Length; j++)
                    {
                        EquipmentData.MeterGroupInfo.Meters[i].SetProperty(arrayField[j], arrayValue[j]);
                    }
                }
            }

            viewModel.SaveAllInfo();

            if (ConfigHelper.Instance.VerifyModel == "自动模式")
            {
                //不需要上传
                #region MyRegion

                // #region 上传到中间库
                // bool blnTotalUpRst = true;
                // int iUpdateOkSum = 0;

                // //string ip = ConfigHelper.Instance.Produce_IP;
                // //int port = int.Parse(ConfigHelper.Instance.Produce_Prot);
                // //string dataSource = ConfigHelper.Instance.Produce_DataSource;
                // //string userId = ConfigHelper.Instance.Produce_UserName;
                // //string pwd = ConfigHelper.Instance.Produce_UserPassWord; ;
                // //string url = ConfigHelper.Instance.Marketing_WebService;

                // IMis mis =  MISFactory.Create();
                ////IMis mis = new Mis.MDS.MDS(ip, port, dataSource, userId, pwd, url);
                // mis.UpdateInit();
                // foreach (TestMeterInfo meter in ViewModel.CheckController.VerifyBase.meterInfo)
                // {
                //     if (!meter.YaoJianYn) continue;

                //     TestMeterInfo temmeter = Mis.DataHelper.DataManage.GetDnbInfoNew(meter,false);
                //     bool bUpdateOk = mis.Update(temmeter);
                //     if (!bUpdateOk)
                //     {
                //         blnTotalUpRst = false;
                //         LogManager.AddMessage(string.Format("上传到生产调度中间库失败，条形码{0}", temmeter.MD_BarCode), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                //         continue;
                //     }
                //     else
                //     {
                //         iUpdateOkSum++;
                //     }
                //     LogManager.AddMessage(string.Format("电能表[{0}]检定记录上传成功!", temmeter.MD_BarCode), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                // }



                // //mis.UpdateCompleted();
                // #endregion

                // if (blnTotalUpRst)        //TODO2这里写抬起电机，然后发送检定完成
                // {
                //     EquipmentData.DeviceManager.ElectricmachineryContrnl(00,0xff);  //取出表

                //     int time = 15;
                //     while (time > 0)
                //     {
                //         EquipmentData.Controller.MessageTips = $"松开压接电机等待{time}秒";
                //         time--;
                //     }

                //     MeterState[] meterState = new MeterState[EquipmentData.Equipment.MeterCount];
                //     for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)   //读取表位状态
                //     {
                //         byte[] OutResult;
                //         bool t = EquipmentData.DeviceManager.Read_Fault(03, (byte)i, out OutResult);  //读取表位的状态
                //         if (t)  //读取成功
                //         {
                //             meterState[i].U = (MeterState_U)OutResult[0];
                //             meterState[i].I = (MeterState_I)OutResult[1];
                //             meterState[i].Motor = (MeterState_Motor)OutResult[2];
                //             meterState[i].YesOrNo = (MeterState_YesOrNo)OutResult[3];
                //             meterState[i].CT = (MeterState_CT)OutResult[4];
                //             meterState[i].Trip = (MeterState_Trip)OutResult[5];
                //             meterState[i].TemperatureI = (MeterState_TemperatureI)OutResult[6];
                //         }
                //         else
                //         {
                //             meterState[i].YesOrNo = MeterState_YesOrNo.没挂表;
                //         }
                //     }
                //     bool[] rpMeter = GetRepeatPressMeters(meterState);

                //     if (Array.IndexOf(rpMeter, true) >= 0)
                //     {
                //         //ZH.Dispatcher.DispatcherManager.Instance.Parms.In_RunningLogType = 2;
                //         //ZH.Dispatcher.DispatcherManager.Instance.Parms.In_RunningLogStrMsg = "有表位不到上限位，不能继续，请手动处理。";
                //         //ZH.Dispatcher.DispatcherManager.Instance.Excute(ZH.Dispatcher.DispatcherEnum.WriteRunningLogA);
                //         //MessageBox.Show("系统检测到表位未完全松表，请处理后按【确定】按钮继续执行！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //         LogManager.AddMessage(string.Format("系统检测到表位未完全松表"), EnumLogSource.设备操作日志, EnumLevel.Warning);

                //     }

                //     //通知一批表检定完成
                //     EquipmentData.CallMsg("CompelateOneBatch");

                //     //ZH.Dispatcher.DispatcherManager.Instance.Excute(ZH.Dispatcher.DispatcherEnum.SetTaskFlagFinished);

                //     EquipmentData.MeterGroupInfo.NewMeters2();   //换新表
                //     //MeterGroup.NewMeters();
                //     //MeterGroup.SaveTmp();

                //     //System.Threading.Thread.Sleep(4000);
                //     ////转到参数录入界面
                //     //App.Message.Show();
                // }
                #endregion
               //EquipmentData.UpMeterData(ViewModel.CheckController.VerifyBase.meterInfo);
                //EquipmentData.CallMsg("VerifyCompelate");

            }
            //上传数据测试
            //EquipmentData.MeterGroupInfo.NewMeters();
        }


        //将表位状态字符串转换为布尔值
        private bool[] GetRepeatPressMeters(MeterState[] states)
        {
            bool[] rp = new bool[states.Length];
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i] == null) continue;
                if (states[i].Motor== MeterState_Motor.电机不确定 || states[i].Motor == MeterState_Motor.电机行在下)
                {
                    rp[i] = true;
                }
            }
            return rp;
        }

        #region 导出excel测试

        private void Button_Click2(object sender, RoutedEventArgs e)
        {

            OneMeterResult meterResult = datagridMeters.SelectedItem as OneMeterResult;
            DataTable[] dataTables = GetDataTabelS(meterResult);   //获取到这个表的所有项目的datatable

            Aspose.Cells.Workbook wbook = new Aspose.Cells.Workbook();
            Aspose.Cells.Worksheet sheet = wbook.Worksheets[0];
            sheet.Name = "数据";
            int row = 0;
            int col = 0;
            Aspose.Cells.Style style = wbook.Styles[wbook.Styles.Add()];
            Aspose.Cells.Style style2 = wbook.Styles[wbook.Styles.Add()];
            style.ForegroundColor = System.Drawing.Color.FromArgb(31, 78, 120);
            style.Pattern = Aspose.Cells.BackgroundType.Solid;
            style.Font.IsBold = true;
            style.Font.Color = System.Drawing.Color.FromArgb(255, 255, 255);
            style.Font.Name = "宋体";//文字字体 
            style.Font.Size = 15;//文字大小
            style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; //左边框 
            style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin; //右边框  
            style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin; //上边框  
            style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; //下边框
            //style = sheet.Cells["A1"].GetStyle();


            style2.Pattern = Aspose.Cells.BackgroundType.Solid;
            style2.Font.IsBold = false;
            style2.Font.Name = "宋体";//文字字体 
            style2.Font.Size = 15;//文字大小
            style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; //左边框 
            style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin; //右边框  
            style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin; //上边框  
            style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; //下边框
            //style2 = sheet.Cells["A1"].GetStyle();

            int Start_A = 0;
            int Start_B = 0;
            int End_A = 0;
            int End_B = 0;
            //cells.GetColumnWidthPixel
            for (int t = 0; t < dataTables.Length; t++)
            {
                DataTable table = dataTables[t];
                Start_A = row;
                Start_B = col;

                for (int i = 0; i < table.Columns.Count; i++) //列名
                {
                    sheet.Cells[row, col].Value = table.Columns[i].ColumnName.ToString().Trim();
                    sheet.Cells[row, col].PutValue(table.Columns[i].ColumnName.ToString().Trim());
                    sheet.Cells[row, col].SetStyle(style);
                    //sheet.Cells[row, col].
                   col++;
                }
                End_B = col;
                col =0;
                row++;
                int index = 1;
                End_A = 1;
                foreach (DataRow Row in table.Rows)
                {
                 
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        sheet.Cells[row, col].Value = Row[i].ToString().Trim();
                        sheet.Cells[row, col].PutValue(Row[i].ToString().Trim());
                        if (index % 2 == 0)
                        {
                           style2.ForegroundColor = System.Drawing.Color.FromArgb(254, 255, 255);
                        }
                        else
                        { 
                         style2.ForegroundColor = System.Drawing.Color.FromArgb(216, 230, 243);
                        }
                        sheet.Cells[row, col].SetStyle(style2);
                        //System.Threading.Thread.Sleep(2);
                        col++;
                    }
                    col = 0;
                    row++;
                    index++;
                    End_A++; ;
                }

              
             
                var range = sheet.Cells.CreateRange(Start_A, Start_B, End_A, End_B);
                range.SetOutlineBorder(Aspose.Cells.BorderType.TopBorder, Aspose.Cells.CellBorderType.Thick, System.Drawing.Color.Blue);
                range.SetOutlineBorder(Aspose.Cells.BorderType.BottomBorder, Aspose.Cells.CellBorderType.Thick, System.Drawing.Color.Blue);
                range.SetOutlineBorder(Aspose.Cells.BorderType.LeftBorder, Aspose.Cells.CellBorderType.Thick, System.Drawing.Color.Blue);
                range.SetOutlineBorder(Aspose.Cells.BorderType.RightBorder, Aspose.Cells.CellBorderType.Thick, System.Drawing.Color.Blue);
                row++;
            }
  

            sheet.AutoFitColumns();


            //限制最大列宽
            for (int i = 0; i < 10; i++)
            {
                //当前列宽
                int pixel = sheet.Cells.GetColumnWidthPixel(i);
                //设置最大列宽
                if (pixel > 180)
                {
                    sheet.Cells.SetColumnWidthPixel(i, 180);
                }
                else if(pixel <80)
                    sheet.Cells.SetColumnWidthPixel(i,100);
                else
                {
                    sheet.Cells.SetColumnWidthPixel(i, pixel);
                }
                //if (i == 0)
                //{
                //    sheet.Cells.SetColumnWidthPixel(i, 150);

                //}
                //else
                //{ 
                //sheet.Cells.SetColumnWidthPixel(i, 120);

                //}

            }
            //System.IO.MemoryStream ms = wbook.SaveToStream();

            string file = @"C:\Users\zhang\Desktop\excel\1.xls";
            wbook.Save(file, SaveFormat.Excel97To2003);
            System.Threading.Thread.Sleep(1500);
            //string file = @"C:\Users\zhang\Desktop\excel\1.xls";
            //FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
            //StreamWriter sw = new StreamWriter(new BufferedStream(fs), System.Text.Encoding.Default);
            //for (int t = 0; t < dataTables.Length; t++)
            //{
            //    //dataTableToCsv(dataTables[i], @"C:\Users\zhang\Desktop\excel\1.xls");
            //    DataTable table = dataTables[t];
            //    string line2 = "";
            //    for (int i = 0; i < table.Columns.Count; i++)
            //    {
            //        line2 += table.Columns[i].ColumnName.ToString().Trim() + "\t"; //内容：自动跳到下一单元格
            //    }
            //    line2 = line2.Substring(0, line2.Length - 1) + "\n";
            //    sw.Write(line2);
            //    foreach (DataRow row in table.Rows)
            //    {
            //        string line = "";

            //        for (int i = 0; i < table.Columns.Count; i++)
            //        {
            //            line += row[i].ToString().Trim() + "\t"; //内容：自动跳到下一单元格
            //        }
            //        line = line.Substring(0, line.Length - 1) + "\n";
            //        sw.Write(line);
            //    }
            //    sw.Write("\n");
            //}

            //sw.Close();
            //fs.Close();
            ////dataTableToCsv(dataTables[0], @"C:\Users\zhang\Desktop\excel\1.xls");
            ////dataTableToCsv(dataTables[1], @"C:\Users\zhang\Desktop\excel\1.xls");

            System.Diagnostics.Process.Start(file); //打开excel文件



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
            DataTable[] dataTables = new DataTable[meterResult.Categories.Count];
            for (int i = 0; i < meterResult.Categories.Count; i++)
            {
                DataTable dataTable = new DataTable();
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

                ViewModel.Model.AsyncObservableCollection<DynamicViewModel> s = meterResult.Categories[i].ResultUnits;
                for (int q = 0; q < s.Count; q++)
                {
                    DataRow row = dataTable.NewRow();
                    for (int j = 0; j < dataTable.Columns.Count; j++)   //循环所有列
                    {
                        row[j] = s[q].GetProperty(dataTable.Columns[j].ColumnName) as string;
                    }
                    dataTable.Rows.Add(row);
                }
                dataTables[i] = dataTable;
            }
            return dataTables;

        }



        private void dataTableToCsv(DataTable table, string file)
        {
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(new BufferedStream(fs), System.Text.Encoding.Default);

            string line2 = "";
            for (int i = 0; i < table.Columns.Count; i++)
            {
                line2 += table.Columns[i].ColumnName.ToString() .Trim() + "\t"; //内容：自动跳到下一单元格
            }

            line2 = line2.Substring(0, line2.Length - 1) + "\n";
            sw.Write(line2);
            foreach (DataRow row in table.Rows)
            {
                string line = "";

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    line += row[i].ToString().Trim() + "\t"; //内容：自动跳到下一单元格
                }
                line = line.Substring(0, line.Length - 1) + "\n";
                sw.Write(line);
            }
            sw.Write("\n");
            sw.Close();
            fs.Close();
        }


        #endregion


    }
}
