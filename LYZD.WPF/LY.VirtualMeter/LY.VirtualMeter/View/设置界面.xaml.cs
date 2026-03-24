using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LY.VirtualMeter.View
{
    /// <summary>
    /// 设置界面.xaml 的交互逻辑
    /// </summary>
    public partial class 设置界面 : UserControl
    {
        public 设置界面()
        {
            InitializeComponent();
            DataContext = AppData.BaseData;
            //初始化数据();
        }
        private void 初始化数据()
        {
            //AppData.BaseData.ConnTypeList.Add("1");
            //AppData.BaseData.ConnTypeList.Add("1");
            //AppData.BaseData.ConnTypeList.Add("1");
            //AppData.BaseData.ConnTypeList.Add("串口");
            //ViewModel.MeterPortData meterPort = new ViewModel.MeterPortData();
            //AppData.BaseData.Meter_PortData.Add(meterPort);
            //AppData.BaseData.Meter_PortData.Add(meterPort);
            //AppData.BaseData.Meter_PortData.Add(meterPort);

        }

        private void btn_save(object sender, RoutedEventArgs e)
        {

            try
            {
                string path = System.Windows.Forms.Application.StartupPath + "\\Ini\\VirtualMeter.ini";
                Core.Function.WriteINI("Data", "MeterCount", AppData.BaseData.Num_Meter.ToString(), path);
                Core.Function.WriteINI("Data", "VirtualCount", AppData.BaseData.Num_Terminal.ToString(), path);
                Core.Function.WriteINI("Data", "ProtocolType", AppData.BaseData.ConnType.ToString(), path);
                for (int i = 1; i <= AppData.BaseData.Meter_PortData.Count; i++)
                {
                    List<string> list = new List<string>();
                    list.Add(AppData.BaseData.Meter_PortData[i - 1].PortNo.ToString());
                    list.Add(AppData.BaseData.Meter_PortData[i - 1].Server_Ip.ToString());
                    list.Add(AppData.BaseData.Meter_PortData[i - 1].Server_TelePort.ToString());
                    list.Add(AppData.BaseData.Meter_PortData[i - 1].Server_LocalPort.ToString());
                    list.Add(AppData.BaseData.Meter_PortData[i - 1].Rate.ToString());
                    list.Add(AppData.BaseData.Meter_PortData[i - 1].DeviceID.ToString());
                    list.Add(AppData.BaseData.Meter_PortData[i - 1].ChanneNo.ToString());
                    Core.Function.WriteINI("Data", "Meter" + i, string.Join("|", list), path);
                }
                MessageBox.Show("保存成功,重启软件生效！");

            }
            catch (Exception ex)
            {

                MessageBox.Show("保存失败" + ex.Message);
            }


        }

        /// <summary>
        /// 基本参数设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_JBCS(object sender, RoutedEventArgs e)
        {
            try
            {

                if (SetTab.SelectedIndex < 0 || SetTab.SelectedIndex > AppData.BaseData.Num_Meter)
                {
                    return;
                }
                int index = SetTab.SelectedIndex;
                AppData.BaseData.MeterObject[index].Ua = AppData.BaseData.设置[index].Ua;
                AppData.BaseData.MeterObject[index].Ub = AppData.BaseData.设置[index].Ub;
                AppData.BaseData.MeterObject[index].Uc = AppData.BaseData.设置[index].Uc;
                AppData.BaseData.MeterObject[index].Ia = AppData.BaseData.设置[index].Ia;
                AppData.BaseData.MeterObject[index].Ib = AppData.BaseData.设置[index].Ib;
                AppData.BaseData.MeterObject[index].Ic = AppData.BaseData.设置[index].Ic;
                AppData.BaseData.MeterObject[index].Phia = AppData.BaseData.设置[index].Phia;
                AppData.BaseData.MeterObject[index].Phib = AppData.BaseData.设置[index].Phib;
                AppData.BaseData.MeterObject[index].Phic = AppData.BaseData.设置[index].Phic;
                MessageBox.Show("设置成功");
            }
            catch (Exception ex)
            {

                MessageBox.Show("设置失败,请保存设置参数正确\r\n" + ex);
            }
        }

        private void Btn_电量平均设置(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SetTab.SelectedIndex < 0 || SetTab.SelectedIndex > AppData.BaseData.Num_Meter)
                {
                    return;
                }
                int index = SetTab.SelectedIndex;
                AppData.BaseData.MeterObject[index].Pq[0, 0] = double.Parse(AppData.BaseData.设置[index].正向有功);
                for (int Ti = 1; Ti <= 4; Ti++)
                    AppData.BaseData.MeterObject[index].Pq[0, Ti] = double.Parse(AppData.BaseData.设置[index].正向有功) / 4;

                AppData.BaseData.MeterObject[index].Pq[1, 0] = double.Parse(AppData.BaseData.设置[index].反向有功);
                for (int Ti = 1; Ti <= 4; Ti++)
                    AppData.BaseData.MeterObject[index].Pq[1, Ti] = double.Parse(AppData.BaseData.设置[index].反向有功) / 4;

                AppData.BaseData.MeterObject[index].Pq[4, 0] = double.Parse(AppData.BaseData.设置[index].一象限无功);
                for (int Ti = 1; Ti <= 4; Ti++)
                    AppData.BaseData.MeterObject[index].Pq[4, Ti] = double.Parse(AppData.BaseData.设置[index].一象限无功) / 4;

                AppData.BaseData.MeterObject[index].Pq[5, 0] = double.Parse(AppData.BaseData.设置[index].二象限无功);
                for (int Ti = 1; Ti <= 4; Ti++)
                    AppData.BaseData.MeterObject[index].Pq[5, Ti] = double.Parse(AppData.BaseData.设置[index].二象限无功) / 4;

                AppData.BaseData.MeterObject[index].Pq[6, 0] = double.Parse(AppData.BaseData.设置[index].三象限无功);
                for (int Ti = 1; Ti <= 4; Ti++)
                    AppData.BaseData.MeterObject[index].Pq[6, Ti] = double.Parse(AppData.BaseData.设置[index].三象限无功) / 4;

                AppData.BaseData.MeterObject[index].Pq[7, 0] = double.Parse(AppData.BaseData.设置[index].四象限无功);
                for (int Ti = 1; Ti <= 4; Ti++)
                    AppData.BaseData.MeterObject[index].Pq[71, Ti] = double.Parse(AppData.BaseData.设置[index].四象限无功) / 4;
                MessageBox.Show("设置成功");
            }
            catch (Exception ex)
            {

                MessageBox.Show("设置失败,请保存设置参数正确\r\n" + ex);
            }
        }

        private void Btn_电量随机设置(object sender, RoutedEventArgs e)
        {
            try
            {

                if (SetTab.SelectedIndex < 0 || SetTab.SelectedIndex > AppData.BaseData.Num_Meter)
                {
                    return;
                }
                int index = SetTab.SelectedIndex;
                AppData.BaseData.MeterObject[index].Pq[0, 0] = double.Parse(AppData.BaseData.设置[index].正向有功);
                double[] dTmp = new double[4];
                dTmp = GetRnadom(double.Parse(AppData.BaseData.设置[index].正向有功));
                for (int Ti = 1; Ti <= 4; Ti++)
                    AppData.BaseData.MeterObject[index].Pq[0, Ti] = dTmp[Ti - 1];

                AppData.BaseData.MeterObject[index].Pq[1, 0] = double.Parse(AppData.BaseData.设置[index].反向有功);
                dTmp = GetRnadom(double.Parse(AppData.BaseData.设置[index].反向有功));
                for (int Ti = 1; Ti <= 4; Ti++)
                    AppData.BaseData.MeterObject[index].Pq[1, Ti] = dTmp[Ti - 1];

                AppData.BaseData.MeterObject[index].Pq[4, 0] = double.Parse(AppData.BaseData.设置[index].一象限无功);
                dTmp = GetRnadom(double.Parse(AppData.BaseData.设置[index].一象限无功));
                for (int Ti = 1; Ti <= 4; Ti++)
                    AppData.BaseData.MeterObject[index].Pq[4, Ti] = dTmp[Ti - 1];

                AppData.BaseData.MeterObject[index].Pq[5, 0] = double.Parse(AppData.BaseData.设置[index].二象限无功);
                dTmp = GetRnadom(double.Parse(AppData.BaseData.设置[index].二象限无功));
                for (int Ti = 1; Ti <= 4; Ti++)
                    AppData.BaseData.MeterObject[index].Pq[5, Ti] = dTmp[Ti - 1];

                AppData.BaseData.MeterObject[index].Pq[6, 0] = double.Parse(AppData.BaseData.设置[index].三象限无功);
                dTmp = GetRnadom(double.Parse(AppData.BaseData.设置[index].三象限无功));
                for (int Ti = 1; Ti <= 4; Ti++)
                    AppData.BaseData.MeterObject[index].Pq[6, Ti] = dTmp[Ti - 1];

                AppData.BaseData.MeterObject[index].Pq[7, 0] = double.Parse(AppData.BaseData.设置[index].四象限无功);
                dTmp = GetRnadom(double.Parse(AppData.BaseData.设置[index].四象限无功));
                for (int Ti = 1; Ti <= 4; Ti++)
                    AppData.BaseData.MeterObject[index].Pq[71, Ti] = dTmp[Ti - 1];
                MessageBox.Show("设置成功");
            }
            catch (Exception ex)
            {

                MessageBox.Show("设置失败,请保存设置参数正确\r\n" + ex);
            }
        }

        private void btn_状态改变(object sender, RoutedEventArgs e)
        {
            try
            {

                if (SetTab.SelectedIndex < 0 || SetTab.SelectedIndex > AppData.BaseData.Num_Meter)
                {
                    return;
                }
                int index = SetTab.SelectedIndex;
                if (AppData.BaseData.设置[index].最近一次编程时间 == true)
                {
                    AppData.BaseData.MeterObject[index].LastBCtime = DateAndTime.Now;
                    AppData.BaseData.MeterObject[index].BCCS = (AppData.BaseData.MeterObject[index].BCCS + 1) % 100;
                }
                if (AppData.BaseData.设置[index].最大需量清零时间 == true)
                {
                    AppData.BaseData.MeterObject[index].LastQLtime = DateAndTime.Now;
                    AppData.BaseData.MeterObject[index].QLCS = (AppData.BaseData.MeterObject[index].QLCS + 1) % 100;
                }

                if (AppData.BaseData.设置[index].电池工作时间 == true)
                    AppData.BaseData.MeterObject[index].DCWorkTime = (AppData.BaseData.MeterObject[index].DCWorkTime + 1) % 100;
                if (AppData.BaseData.设置[index].断相次数 == true)
                {
                    AppData.BaseData.MeterObject[index].DXCS = (AppData.BaseData.MeterObject[index].DXCS + 1) % 100;
                    AppData.BaseData.MeterObject[index].DXFSTime = DateTime.Now.ToString("ssmmHHddMMyy");
                    AppData.BaseData.MeterObject[index].DXJSTime = DateTime.Now.AddMinutes(1).ToString("ssmmHHddMMyy");
                }
                if (AppData.BaseData.设置[index].常数 == true)
                {
                    if (AppData.BaseData.MeterObject[index].BiaoYgCS == 600)
                    {
                        AppData.BaseData.MeterObject[index].BiaoYgCS = 800;
                        AppData.BaseData.MeterObject[index].BiaoWgCS = 800;
                    }
                    else
                    {
                        AppData.BaseData.MeterObject[index].BiaoYgCS = 600;
                        AppData.BaseData.MeterObject[index].BiaoWgCS = 600;
                    }
                }
                if (AppData.BaseData.设置[index].时段 == true)
                {
                    if (AppData.BaseData.MeterObject[index].ShiDuan[0].Hour == 5)
                    {
                        AppData.BaseData.MeterObject[index].LastSDtime = DateTime.Now;
                        AppData.BaseData.MeterObject[index].ShiDuan[0] = AppData.BaseData.MeterObject[index].ShiDuan[0].AddHours(1);
                        AppData.BaseData.MeterObject[index].SDBCCS++;
                    }
                    else
                    {
                        AppData.BaseData.MeterObject[index].LastSDtime = DateTime.Now;
                        AppData.BaseData.MeterObject[index].ShiDuan[0] = AppData.BaseData.MeterObject[index].ShiDuan[0].AddHours(-1);
                        AppData.BaseData.MeterObject[index].SDBCCS++;
                    }

                }
                if (AppData.BaseData.设置[index].自动抄表日 == true)
                {
                    if (AppData.BaseData.MeterObject[index].AutoDate == "0001")
                        AppData.BaseData.MeterObject[index].AutoDate = "0002";
                    else
                        AppData.BaseData.MeterObject[index].AutoDate = "0001";
                }
                if (AppData.BaseData.设置[index].清零次数 == true)
                    AppData.BaseData.MeterObject[index].QLCS = (AppData.BaseData.MeterObject[index].QLCS + 1) % 100;
                if (AppData.BaseData.设置[index].编程次数 == true)
                    AppData.BaseData.MeterObject[index].BCCS = (AppData.BaseData.MeterObject[index].BCCS + 1) % 100;

                if (AppData.BaseData.设置[index].电池异常 == true)
                    AppData.BaseData.MeterObject[index].DcZhenChang = 0;
                else
                    AppData.BaseData.MeterObject[index].DcZhenChang = 1;

                if (AppData.BaseData.设置[index].电表运行状态字 == true)
                {
                    if (AppData.BaseData.设置[index].状态字7 == "0000")
                        AppData.BaseData.设置[index].状态字7 = "0001";
                    else
                        AppData.BaseData.设置[index].状态字7 = "0000";
                }
                MessageBox.Show("设置成功");
            }
            catch (Exception ex)
            {

                MessageBox.Show("设置失败,请保存设置参数正确\r\n" + ex);
            }
        }

        private void btn_运行状态字(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SetTab.SelectedIndex < 0 || SetTab.SelectedIndex > AppData.BaseData.Num_Meter)
                {
                    return;
                }
                int index = SetTab.SelectedIndex;
                AppData.BaseData.MeterObject[index].B_ZT[0] = AppData.BaseData.设置[index].状态字1;
                AppData.BaseData.MeterObject[index].B_ZT[1] = AppData.BaseData.设置[index].状态字2;
                AppData.BaseData.MeterObject[index].B_ZT[2] = AppData.BaseData.设置[index].状态字3;
                AppData.BaseData.MeterObject[index].B_ZT[3] = AppData.BaseData.设置[index].状态字4;
                AppData.BaseData.MeterObject[index].B_ZT[4] = AppData.BaseData.设置[index].状态字5;
                AppData.BaseData.MeterObject[index].B_ZT[5] = AppData.BaseData.设置[index].状态字6;
                AppData.BaseData.MeterObject[index].B_ZT[6] = AppData.BaseData.设置[index].状态字7;
                MessageBox.Show("设置成功");
            }
            catch (Exception ex)
            {

                MessageBox.Show("设置失败,请保存设置参数正确\r\n" + ex);
            }
        }

        private void btn_其他设置(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SetTab.SelectedIndex < 0 || SetTab.SelectedIndex > AppData.BaseData.Num_Meter)
                {
                    return;
                }
                int index = SetTab.SelectedIndex;
                AppData.BaseData.MeterObject[index].TimeLoss = double.Parse(AppData.BaseData.设置[index].时间慢系统分);
                AppData.BaseData.MeterObject[index].DateLoss = double.Parse(AppData.BaseData.设置[index].日期慢系统天);
                AppData.BaseData.MeterObject[index].zzXiShu = double.Parse(AppData.BaseData.设置[index].走字系数);
                AppData.BaseData.MeterObject[index].DnXiShu = double.Parse(AppData.BaseData.设置[index].电能系数);

                AppData.BaseData.MeterObject[index].BAddress = AppData.BaseData.设置[index].表地址.PadLeft(12, '0');
                for (int i = 0; i < AppData.BaseData.TerminalObject.Count; i++)
                {
                    AppData.BaseData.TerminalObject[i].ZbAddress = AppData.BaseData.设置[index].载波地址.PadLeft(12, '0');

                }
                MessageBox.Show("设置成功");
            }
            catch (Exception ex)
            {

                MessageBox.Show("设置失败,请保存设置参数正确\r\n" + ex);
            }
        }
        public double[] GetRnadom(double dTmp)
        {
            Thread.Sleep(10);
            double[] dsTmp = new double[4]; int iTmp = 0;
            iTmp = int.Parse(dTmp.ToString());
            dsTmp[0] = (new Random()).Next(int.Parse((iTmp / 5).ToString()), int.Parse((iTmp / 3).ToString()));
            iTmp = int.Parse((dTmp - dsTmp[0]).ToString());
            dsTmp[1] = (new Random()).Next(int.Parse((iTmp / 4).ToString()), int.Parse((iTmp / 2).ToString()));
            iTmp = int.Parse((dTmp - dsTmp[0] - dsTmp[1]).ToString());
            dsTmp[2] = (new Random()).Next(int.Parse((iTmp / 3).ToString()), int.Parse((iTmp).ToString()));
            dsTmp[3] = int.Parse((dTmp - dsTmp[0] - dsTmp[1] - dsTmp[2]).ToString());
            return dsTmp;
        }

        private void meterGrid_CurrentCellChanged(object sender, DataGridCellEditEndingEventArgs e)
        {
            string columnHeader = e.Column.Header as string;
            if (columnHeader == "端口号")
            {
                int a = FindRowIndex(e.Row);
                string newValue = (e.EditingElement as TextBox).Text;
                if (Core.Function.IsNumber(newValue))
                {
                    int prot = int.Parse(newValue)+1;
                    for (int i = a+1; i < AppData.BaseData.Meter_PortData.Count; i++)
                    {
                        AppData.BaseData.Meter_PortData[i].PortNo = prot;
                        prot++;
                    }
                }
            }

        }
        //获取行索引
        private int FindRowIndex(DataGridRow row)
        {
            DataGrid dataGrid = ItemsControl.ItemsControlFromItemContainer(row) as DataGrid;

            int index = dataGrid.ItemContainerGenerator.IndexFromContainer(row);

            return index;
        }
    }
}
