using LYZD.DAL.DataBaseView;
using LYZD.DataManager.ViewModel;
using LYZD.Utility;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckInfo;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace LYZD.DataManager
{
    /// <summary>
    /// DetailedData.xaml 的交互逻辑
    /// </summary>
    public partial class DetailedData : Window
    {
        private AllMeterResult meterResults = new AllMeterResult(null);
        public DetailedData()
        {
            InitializeComponent();
            gridMeterResult.DataContext = meterResults;
            //LoadMeterInfo();
            LoadEquipInfo();
        }

        private static DetailedData instance = null;

        public static DetailedData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DetailedData();
                }
                return instance;
            }
        }
        string DeviceDataPath = System.IO.Directory.GetCurrentDirectory() + "\\Ini\\DeviceData.ini";
        /// <summary>
        /// 加载台体信息
        /// </summary>
        private void LoadEquipInfo()
        {
            string[] NameS = LYZD.ViewModel.Const.OperateFile.GetINI("Data", "名称", DeviceDataPath).Split('|');
            string[] Values = LYZD.ViewModel.Const.OperateFile.GetINI("Data", "值", DeviceDataPath).Split('|');

            for (int i = 0; i < NameS.Length; i++)
            {
                string nameTemp = NameS[i];
                string value = Values[i];
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
                gridTemp.ColumnDefinitions[0].Width = new GridLength(90);
                TextBlock textBlockName = new TextBlock()
                {
                    Text = nameTemp,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(3, 0, 3, 0),
                    ToolTip = nameTemp
                };
                TextBox textBoxValue = new TextBox()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(3, 0, 3, 0),
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    Text=value
                };
                Grid.SetColumn(textBoxValue, 1);
                gridTemp.Children.Add(textBlockName);
                gridTemp.Children.Add(textBoxValue);
                stackPanelEquipInfo.Children.Add(gridTemp);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string name = "";
            string value = "";
            for (int i = 0; i < stackPanelEquipInfo.Children.Count; i++)
            {
                if (stackPanelEquipInfo.Children[i] is  Grid)
                {
                    Grid gridTemp = stackPanelEquipInfo.Children[i] as Grid;
                    for (int j = 0; j < gridTemp.Children.Count; j++)
                    {

                        if (gridTemp.Children[j] is TextBlock) //名称
                        {
                            TextBlock textBlock = gridTemp.Children[j] as TextBlock;
                            name += textBlock.Text +"|";

                        }
                        else if (gridTemp.Children[j] is TextBox)   //值
                        {
                            TextBox textBlock = gridTemp.Children[j] as TextBox;
                            value += textBlock.Text + "|";
                        }

                    }
                }
            }

            name = name.TrimEnd('|');
            value = value.TrimEnd('|');

            try
            {
                LYZD.ViewModel.Const.OperateFile.WriteINI("Data", "名称", name, DeviceDataPath);
                LYZD.ViewModel.Const.OperateFile.WriteINI("Data", "值", value, DeviceDataPath);
                MessageBox.Show("保存成功");
            }
            catch (System.Exception ex)
            {

                MessageBox.Show("保存失败"+ex);
            }
        }

        public bool IsClose = false;

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!IsClose)
            {
                e.Cancel = true;
                this.Hide();
            }
            else
            {
                base.OnClosing(e);
            }
        }
    }
}
