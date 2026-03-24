using LYZD.Core.Enum;
using LYZD.ViewModel.CheckController;
using LYZD.ViewModel.Device;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LYZD.ViewModel.DebugViewModel
{
    public class ErrorControlViewModel : ViewModelBase
    {
        #region 旧

        #region 属性

        private bool TaskState = false;

        private int startNo = 1;
        /// <summary>
        /// 开始编号
        /// </summary>
        public int StartNo
        {
            get { return startNo; }
            set
            { SetPropertyValue(value, ref startNo, "StartNo"); }
        }

        private int endNo = 1;
        /// <summary>
        /// 结束编号
        /// </summary>
        public int EndNo
        {
            get { return endNo; }
            set
            {
                SetPropertyValue(value, ref endNo, "EndNo");
            }
        }


        #endregion


        #region 继电器控制方法

        /// <summary>
        /// 关闭所有继电器
        /// </summary>
        public void RelayAll_Off()
        {
            Utility.TaskManager.AddDeviceAction(() =>
            {
                EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)0xFF);
            });
        }

        /// <summary>
        /// 开启所有继电器
        /// </summary>
        public void RelayAll_On()
        {

            Utility.TaskManager.AddDeviceAction(() =>
            {
                EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)0xFF);
            });
        }

        public void Relay_On()
        {

            Utility.TaskManager.AddDeviceAction(() =>
            {
                for (int i = StartNo; i <= EndNo; i++)
                {
                    EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)i);
                }
            });

        }
        public void Relay_Off()
        {
            Utility.TaskManager.AddDeviceAction(() =>
            {
                for (int i = StartNo; i <= EndNo; i++)
                {
                    EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)i);
                }
            });
        }
        #endregion


        #region 电机控制

        public void Read_Fault()
        {

            //电压短路标标志00正常，01短路，02继电器不工作
            //电流短路标标志，00正在，01旁路成功，02旁路继电器不工作
            //电机行程标志，00电机行程不确定，01电机行程在上取表出座位置，02电机行程在下，压表入座位置
            //挂表状态标志，00没挂表，01以挂表
            //CT电量过载标志，00正常，01过载(北京改造线科陆CT)
            //跳匝指示灯标志 00正常，01以输出跳匝信号
            //二级设备温度板电流过载标志，00没过载，01过载(北京改造线，互感表)

            Utility.TaskManager.AddDeviceAction(() =>
            {
                string str = "";
                for (int i = StartNo; i <= EndNo; i++)
                /// <param name="OutResult">返回状态0:00表示没短路，01表示短路；DATA1-电流开路标志，1:00表示没开路，01表示开路。</param>
                {
                    byte[] OutResult;
                    bool t = EquipmentData.DeviceManager.Read_Fault(03, (byte)i, out OutResult);
                    if (t)
                    {
                        str += $"表位【{i}】";
                        str += (MeterState_U)OutResult[0] + "|";
                        str += (MeterState_I)OutResult[1] + "|";
                        str += (MeterState_Motor)OutResult[2] + "|";
                        str += (MeterState_YesOrNo)OutResult[3] + "|";
                        str += (MeterState_CT)OutResult[4] + "|";
                        str += (MeterState_Trip)OutResult[5] + "|";
                        str += (MeterState_TemperatureI)OutResult[6] + "|";

                        //switch (OutResult[0])
                        //{
                        //    case 0:
                        //        str += "电压没有短路";
                        //        break;
                        //    case 1:
                        //        str += "电压短路";
                        //        break;
                        //    default:
                        //        break;
                        //}
                        //str += $"|";
                        //switch (OutResult[1])
                        //{
                        //    case 0:
                        //        str += "电流没有开路";
                        //        break;
                        //    case 1:
                        //        str += "电流开路";
                        //        break;
                        //    default:
                        //        break;
                    }
                    else
                    {
                        str += $"表位【{i}】读取失败";
                    }
                    str += "\r\n";
                }
                Utility.Log.LogManager.AddMessage(str);
            });
        }

        public void Tesk_Read_Fault()
        {

            if (TaskState)
            {
                return;
            }
            Utility.Log.LogManager.AddMessage("开始读取");
            TaskState = true;
            Task task = new Task(() =>
            {
                while (true)
                {
                    Read_Fault2();
                    System.Threading.Thread.Sleep(1000);
                    if (!TaskState)
                    {
                        Utility.Log.LogManager.AddMessage("停止读取");
                        break;
                    }

                }
            });
            task.Start();

        }
        public void StopTask()
        {
            TaskState = false;
        }
        public void Read_Fault2()
        {
            string str = "";
            for (int i = StartNo; i <= EndNo; i++)
            /// <param name="OutResult">返回状态0:00表示没短路，01表示短路；DATA1-电流开路标志，1:00表示没开路，01表示开路。</param>
            {
                byte[] OutResult;
                bool t = EquipmentData.DeviceManager.Read_Fault(03, (byte)i, out OutResult);
                if (t)
                {
                    str += $"表位【{i}】";
                    str += (MeterState_U)OutResult[0] + "|";
                    str += (MeterState_I)OutResult[1] + "|";
                    str += (MeterState_Motor)OutResult[2] + "|";
                    str += (MeterState_YesOrNo)OutResult[3] + "|";
                    str += (MeterState_CT)OutResult[4] + "|";
                    str += (MeterState_Trip)OutResult[5] + "|";
                    str += (MeterState_TemperatureI)OutResult[6] + "|";
                }
                else
                {
                    str += $"表位【{i}】读取失败";
                }
                str += "\r\n";
            }
            Utility.Log.LogManager.AddMessage(str);

        }
        #endregion

        #endregion

        public ErrorControlViewModel()
        {
            PulseTypeList.Clear();
            PulseTypeList.Add("有功误差");
            PulseTypeList.Add("无功误差");
            PulseTypeList.Add("有功脉冲计数");
            PulseTypeList.Add("无功脉冲计数");
            PulseTypeList.Add("时钟误差");
            PulseType = "有功误差";
        }

        public Dictionary<string, MeterStartControlViewModel> meterStartS = new Dictionary<string, MeterStartControlViewModel>();

        public bool isAllCheck1 = false;
        public bool IsAllCheck1
        {
            get { return isAllCheck1; }
            set
            {
                SetPropertyValue(value, ref isAllCheck1, "IsAllCheck1");
                foreach (var item in meterStartS.Keys)
                {
                    if (int.Parse(item) <= meterStartS.Keys.Count / 2)
                    {
                        MeterStartControlViewModel model = meterStartS[item];
                        model.IsCheck = isAllCheck1;
                    }
                }
            }
        }
        public bool isAllCheck2 = false;
        public bool IsAllCheck2
        {
            get { return isAllCheck2; }
            set
            {
                SetPropertyValue(value, ref isAllCheck2, "IsAllCheck2");
                foreach (var item in meterStartS.Keys)
                {
                    if (int.Parse(item) > meterStartS.Keys.Count / 2)
                    {
                        MeterStartControlViewModel model = meterStartS[item];
                        model.IsCheck = IsAllCheck2;
                    }
                }
            }
        }
        #region 新--表位控制
        private int meterNo = 1;
        /// <summary>
        ///表位编号
        /// </summary>
        public int MeterNo
        {
            get { return meterNo; }
            set
            { SetPropertyValue(value, ref meterNo, "MeterNo"); }
        }
        private string address = "";
        /// <summary>
        /// 开始编号
        /// </summary>
        public string Address
        {
            get { return address; }
            set
            { SetPropertyValue(value, ref address, "Address"); }
        }

        //读取表地址
        public void ReadAddress()
        {

            Utility.TaskManager.AddDeviceAction(() =>
            {
                EquipmentData.Controller.UpdateMeterProtocol();
                Address = MeterProtocolAdapter.Instance.ReadAddress(meterNo - 1);
            });

        }



        public void Set_HG()
        {
            Utility.Log.LogManager.AddMessage("正在切换到互感式");
            //EquipmentData.DeviceManager.Hgq_Set(0);
            Utility.TaskManager.AddDeviceAction(() =>
            {
                EquipmentData.DeviceManager.Hgq_Set(0);
            });

        }
        public void Set_ZJ()
        {
            Utility.Log.LogManager.AddMessage("正在切换到直接式");
            //EquipmentData.DeviceManager.Hgq_Set(1);
            Utility.TaskManager.AddDeviceAction(() =>
            {
                EquipmentData.DeviceManager.Hgq_Set(1);
            });

        }
        public void Set_Off()
        {
            Utility.Log.LogManager.AddMessage("正在关闭互感器电机");
            //EquipmentData.DeviceManager.Hgq_Set(1);
            Utility.TaskManager.AddDeviceAction(() =>
            {
                EquipmentData.DeviceManager.Hgq_Off2();
            });

        }
        public void Zaibo_On()
        {
            Utility.Log.LogManager.AddMessage("正在开启载波电源");
            //EquipmentData.DeviceManager.Hgq_Set(1);
            Utility.TaskManager.AddDeviceAction(() =>
            {
                bool[] t = new bool[1];
                t[0] = true;
                EquipmentData.DeviceManager.SetZBGZDYContrnl(255, 12, t);
            });

        }

        public void Zaibo_Off()
        {
            Utility.Log.LogManager.AddMessage("正在关闭载波电源");
            //EquipmentData.DeviceManager.Hgq_Set(1);
            Utility.TaskManager.AddDeviceAction(() =>
            {
                bool[] t = new bool[1];
                t[0] = false;
                EquipmentData.DeviceManager.SetZBGZDYContrnl(255, 12, t);
            });

        }
        #region 台体状态灯
        /// <summary>
        /// 红
        /// </summary>
        public void Set_Hong()
        {
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x10, (byte)0x01);
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x11, (byte)0x00);
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x12, (byte)0x00);
        }
        /// <summary>
        /// 绿
        /// </summary>
        public void Set_Lv()
        {
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x10, (byte)0x00);
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x11, (byte)0x01);
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x12, (byte)0x00);
        }
        /// <summary>
        ///黄
        /// </summary>
        public void Set_Huang()
        {
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x10, (byte)0x00);
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x11, (byte)0x00);
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x12, (byte)0x01);
        }
        /// <summary>
        /// 红 --闪烁
        /// </summary>
        public void Set_Hong2()
        {
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x10, (byte)0x01);
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x11, (byte)0x00);
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x12, (byte)0x00);
        }
        /// <summary>
        /// 绿
        /// </summary>
        public void Set_Lv2()
        {
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x10, (byte)0x00);
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x11, (byte)0x01);
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x12, (byte)0x00);
        }
        /// <summary>
        ///黄
        /// </summary>
        public void Set_Huang2()
        {
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x10, (byte)0x00);
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x11, (byte)0x00);
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x12, (byte)0x01);
        }
        /// <summary>
        ///灭
        /// </summary>
        public void Set_Guan()
        {
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x10, (byte)0x00);
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x11, (byte)0x00);
            EquipmentData.DeviceManager.SetEquipmentThreeColor((byte)0x12, (byte)0x00);
        }
        #endregion

        #region 继电器控制
        /// <summary>
        /// 恢复所有表位
        /// </summary>
        public void Set_RelayAll_On()
        {

            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;

            for (int i = 0; i < num; i++)
            {
                EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)0xFF, i);
                System.Threading.Thread.Sleep(150);
            }
            //Utility.TaskManager.AddDeviceAction(() =>
            //{
            //    EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)0xFF);
            //});
        }
        /// <summary>
        /// 隔离选中表位
        /// </summary>
        public void Set_RelayAll_Off()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            num = meterStartS.Keys.Count / num;//每一路线上的数量
            int index = 0;
            int value = 0;
            foreach (var item in meterStartS.Keys)
            {
                MeterStartControlViewModel model = meterStartS[item];
                if (model.IsCheck)
                {
                    if (index >= num * (value + 1))
                    {
                        value++;
                    }
                    EquipmentData.DeviceManager.ControlMeterRelay(2, byte.Parse(item), value);
                    System.Threading.Thread.Sleep(80);
                }
                index++;
            }
        }


        public void Set_RelayAllSelect_On()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            num = meterStartS.Keys.Count / num;//每一路线上的数量
            int index = 0;
            int value = 0;
            foreach (var item in meterStartS.Keys)
            {
                MeterStartControlViewModel model = meterStartS[item];
                if (model.IsCheck)
                {
                    if (index >= num * (value + 1))
                    {
                        value++;
                    }
                    EquipmentData.DeviceManager.ControlMeterRelay(1, byte.Parse(item), value);
                    System.Threading.Thread.Sleep(80);
                }
                index++;
            }
        }


        /// <summary>
        /// 全表位上电
        /// </summary>
        public void Set_RelayAll_PowerOn()
        {

            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;

            for (int i = 0; i < num; i++)
            {
                EquipmentData.DeviceManager.ControlMeterPowerRelay(0, (byte)0xFF, i);
                System.Threading.Thread.Sleep(150);
            }
            //Utility.TaskManager.AddDeviceAction(() =>
            //{
            //    EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)0xFF);
            //});
        }

        /// <summary>
        /// 全表位下电
        /// </summary>
        public void Set_RelayAll_PowerOff()
        {

            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;

            for (int i = 0; i < num; i++)
            {
                EquipmentData.DeviceManager.ControlMeterPowerRelay(1, (byte)0xFF, i);
                System.Threading.Thread.Sleep(150);
            }
            //Utility.TaskManager.AddDeviceAction(() =>
            //{
            //    EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)0xFF);
            //});
        }

        /// <summary>
        /// 选中表位电压继电器上电
        /// </summary>
        public void Set_Relay_PowerOn()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            num = meterStartS.Keys.Count / num;//每一路线上的数量
            int index = 0;
            int value = 0;
            foreach (var item in meterStartS.Keys)
            {
                MeterStartControlViewModel model = meterStartS[item];
                if (model.IsCheck)
                {
                    if (index >= num * (value + 1))
                    {
                        value++;
                    }
                    EquipmentData.DeviceManager.ControlMeterPowerRelay(0, byte.Parse(item), value);
                    System.Threading.Thread.Sleep(80);
                }
                index++;
            }
        }

        /// <summary>
        /// 选中表位电压继电器下电
        /// </summary>
        public void Set_Relay_PowerOff()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            num = meterStartS.Keys.Count / num;//每一路线上的数量
            int index = 0;
            int value = 0;
            foreach (var item in meterStartS.Keys)
            {
                MeterStartControlViewModel model = meterStartS[item];
                if (model.IsCheck)
                {
                    if (index >= num * (value + 1))
                    {
                        value++;
                    }
                    EquipmentData.DeviceManager.ControlMeterPowerRelay(1, byte.Parse(item), value);
                    System.Threading.Thread.Sleep(80);
                }
                index++;
            }
        }

        /// <summary>
        /// 供电-能源/融合模式
        /// </summary>
        public void PowerTpye_On()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.多功能板].Count;
            for (int i = 0; i < num; i++)
            {
                EquipmentData.DeviceManager.PowerTpye((byte)0x13, (byte)0x01, i);
                System.Threading.Thread.Sleep(150);
            }

        }

        /// <summary>
        /// 供电-集中/专变模式
        /// </summary>
        public void PowerTpye_Off()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.多功能板].Count;
            for (int i = 0; i < num; i++)
            {
                EquipmentData.DeviceManager.PowerTpye((byte)0x13, (byte)0x00, i);
                System.Threading.Thread.Sleep(150);
            }

        }

        #endregion

        #region 混合终端控制
        private string selectZDLX = "集中器I型13版";

        public string SelectZDLX
        {
            get { return selectZDLX; }
            set { SetPropertyValue(value, ref selectZDLX, "SelectZDLX"); }
        }
        /// <summary>
        /// 设置终端类型
        /// </summary>
        public void Set_TerminalType()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            //num = meterStartS.Keys.Count / num;//每一路线上的数量
            byte type = 0;
            switch (SelectZDLX)
            {
                case "集中器I型13版":
                    type = 1;
                    break;
                case "集中器I型22版":
                    type = 2;
                    break;
                case "专变III型13版":
                    type = 3;
                    break;
                case "专变III型22版":
                    type = 4;
                    break;
                case "融合终端":
                    type = 5;
                    break;
                case "能源控制器":
                    type = 6;
                    break;
                case "智能融合终端25版":
                    type = 7;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < num; i++)
            {
                EquipmentData.DeviceManager.SetZDType(type, (byte)i);
            }
        }
        /// <summary>
        /// 1路切换到485
        /// </summary>
        public void Set_Terminal_1_485()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            num = meterStartS.Keys.Count / num;//每一路线上的数量
            int index = 0;
            int value = 0;
            foreach (var item in meterStartS.Keys)
            {
                MeterStartControlViewModel model = meterStartS[item];
                if (model.IsCheck)
                {
                    if (index >= num * (value + 1))
                    {
                        value++;
                    }
                    EquipmentData.DeviceManager.ControlConnrRelay2(0, byte.Parse(item), value);
                    System.Threading.Thread.Sleep(150);
                }
                index++;
            }
        }
        /// <summary>
        /// 1路切换到蓝牙
        /// </summary>
        public void Set_Terminal_1_ITO()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            num = meterStartS.Keys.Count / num;//每一路线上的数量
            int index = 0;
            int value = 0;
            foreach (var item in meterStartS.Keys)
            {
                MeterStartControlViewModel model = meterStartS[item];
                if (model.IsCheck)
                {
                    if (index >= num * (value + 1))
                    {
                        value++;
                    }
                    EquipmentData.DeviceManager.ControlConnrRelay2(1, byte.Parse(item), value);
                    System.Threading.Thread.Sleep(150);
                }
                index++;
            }
        }
        /// <summary>
        /// 2路切换到485_2
        /// </summary>
        public void Set_Terminal_2_4852()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            num = meterStartS.Keys.Count / num;//每一路线上的数量
            int index = 0;
            int value = 0;
            foreach (var item in meterStartS.Keys)
            {
                MeterStartControlViewModel model = meterStartS[item];
                if (model.IsCheck)
                {
                    if (index >= num * (value + 1))
                    {
                        value++;
                    }
                    EquipmentData.DeviceManager.ControlConnrRelay3(0, byte.Parse(item), value);
                    System.Threading.Thread.Sleep(150);
                }
                index++;
            }
        }
        /// <summary>
        /// 2路切换到485_3
        /// </summary>
        public void Set_Terminal_2_4853()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            num = meterStartS.Keys.Count / num;//每一路线上的数量
            int index = 0;
            int value = 0;
            foreach (var item in meterStartS.Keys)
            {
                MeterStartControlViewModel model = meterStartS[item];
                if (model.IsCheck)
                {
                    if (index >= num * (value + 1))
                    {
                        value++;
                    }
                    EquipmentData.DeviceManager.ControlConnrRelay3(1, byte.Parse(item), value);
                    System.Threading.Thread.Sleep(150);
                }
                index++;
            }
        }
        /// <summary>
        /// 2路切换到485_4
        /// </summary>
        public void Set_Terminal_2_4854()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            num = meterStartS.Keys.Count / num;//每一路线上的数量
            int index = 0;
            int value = 0;
            foreach (var item in meterStartS.Keys)
            {
                MeterStartControlViewModel model = meterStartS[item];
                if (model.IsCheck)
                {
                    if (index >= num * (value + 1))
                    {
                        value++;
                    }
                    EquipmentData.DeviceManager.ControlConnrRelay3(2, byte.Parse(item), value);
                    System.Threading.Thread.Sleep(150);
                }
                index++;
            }
        }
        /// <summary>
        /// 2路切换到232
        /// </summary>
        public void Set_Terminal_2_232()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            num = meterStartS.Keys.Count / num;//每一路线上的数量
            int index = 0;
            int value = 0;
            foreach (var item in meterStartS.Keys)
            {
                MeterStartControlViewModel model = meterStartS[item];
                if (model.IsCheck)
                {
                    if (index >= num * (value + 1))
                    {
                        value++;
                    }
                    EquipmentData.DeviceManager.ControlConnrRelay3(3, byte.Parse(item), value);
                    System.Threading.Thread.Sleep(150);
                }
                index++;
            }
        }


        #endregion

        /// <summary>
        /// 电机下行
        /// </summary>
        public void ElectricmachineryContrnl_Down()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            num = meterStartS.Keys.Count / num;//每一路线上的数量
            int index = 0;
            int value = 0;
            foreach (var item in meterStartS.Keys)
            {
                MeterStartControlViewModel model = meterStartS[item];
                if (model.IsCheck)
                {
                    if (index >= num * (value + 1))
                    {
                        value++;
                    }
                    if (index >= 8)
                    {
                        EquipmentData.DeviceManager.ElectricmachineryContrnl(01, byte.Parse(item), 1);
                    }
                    else
                    {
                        EquipmentData.DeviceManager.ElectricmachineryContrnl(01, byte.Parse(item), 0);
                    }
                    System.Threading.Thread.Sleep(80);
                }
                index++;
            }
        }
        /// <summary>
        /// 上
        /// </summary>
        public void ElectricmachineryContrnl_Up()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            num = meterStartS.Keys.Count / num;//每一路线上的数量
            int index = 0;
            int value = 0;
            foreach (var item in meterStartS.Keys)
            {
                MeterStartControlViewModel model = meterStartS[item];
                if (model.IsCheck)
                {
                    if (index >= num * (value + 1))
                    {
                        value++;
                    }
                    if (index >= 8)
                    {
                        EquipmentData.DeviceManager.ElectricmachineryContrnl(00, byte.Parse(item), 1);
                    }
                    else
                    {
                        EquipmentData.DeviceManager.ElectricmachineryContrnl(00, byte.Parse(item), 0);
                    }
                    System.Threading.Thread.Sleep(80);
                }
                index++;
            }
        }
        /// <summary>
        /// 切换到485
        /// </summary>
        public void Set_Conn485()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            num = meterStartS.Keys.Count / num;//每一路线上的数量
            int index = 0;
            int value = 0;
            foreach (var item in meterStartS.Keys)
            {
                MeterStartControlViewModel model = meterStartS[item];
                if (model.IsCheck)
                {
                    if (index >= num * (value + 1))
                    {
                        value++;
                    }
                    EquipmentData.DeviceManager.ControlConnrRelay(0, byte.Parse(item), value);
                    System.Threading.Thread.Sleep(150);
                }
                index++;
            }
        }
        /// <summary>
        /// 切换到232 
        /// </summary>
        public void Set_Conn232()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            num = meterStartS.Keys.Count / num;//每一路线上的数量
            int index = 0;
            int value = 0;
            foreach (var item in meterStartS.Keys)
            {
                MeterStartControlViewModel model = meterStartS[item];
                if (model.IsCheck)
                {
                    if (index >= num * (value + 1))
                    {
                        value++;
                    }
                    EquipmentData.DeviceManager.ControlConnrRelay(1, byte.Parse(item), value);
                    System.Threading.Thread.Sleep(150);
                }
                index++;
            }

        }
        #endregion


        /// <summary>
        /// 重启电脑
        /// </summary>
        public void RestareWindow()
        {
            if (MessageBox.Show("确定重启计算机！", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                EquipmentData.RestareWindow();
            }
        }

        /// <summary>
        /// 重启程序
        /// </summary>
        public void RestareExe()
        {
            //Application.ExitThread();
            //System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);

        }

        /// <summary>
        /// 发送检定完成
        /// </summary>
        public void VerfiyEnd()
        {

            //System.Diagnostics.Stopwatch TimeS = new System.Diagnostics.Stopwatch();
            //TimeS.Start();// 开始
            // ViewModel.MeterResoultModel.MeterDataHelper.GetDnbInfoNew();
            //TimeS.Stop();// 结束
            //Utility.Log.LogManager.AddMessage(TimeS.ElapsedMilliseconds.ToString());
            EquipmentData.CallMsg("CompelateOneBatch");
        }

        /// <summary>
        /// 开始自动检定
        /// </summary>
        public void StartAutoVerify()
        {

        }

        #region 误差调试

        #region 属性
        private int stdConst = 1000000;
        /// <summary>
        /// 标准常数
        /// </summary>
        public int StdConst
        {
            get { return stdConst; }
            set
            { SetPropertyValue(value, ref stdConst, "StdConst"); }
        }
        private int testedConst = 6400;
        /// <summary>
        /// 被检常数
        /// </summary>
        public int TestedConst
        {
            get { return testedConst; }
            set
            { SetPropertyValue(value, ref testedConst, "TestedConst"); }
        }
        private int testQs = 2;
        /// <summary>
        /// 检定圈数
        /// </summary>
        public int TestQs
        {
            get { return testQs; }
            set
            { SetPropertyValue(value, ref testQs, "TestQs"); }
        }

        private ObservableCollection<string> pulseTypeList = new ObservableCollection<string>();
        /// <summary>
        ///脉冲类型
        public ObservableCollection<string> PulseTypeList
        {
            get { return pulseTypeList; }
            set { SetPropertyValue(value, ref pulseTypeList, "PulseTypeList"); }
        }
        private string pulseType;
        /// <summary>
        ///脉冲类型
        public string PulseType
        {
            get { return pulseType; }
            set
            {
                SetPropertyValue(value, ref pulseType, "PulseType");
                switch (PulseType)
                {
                    case "有功误差":
                        PulseTypeValee = 0;
                        break;
                    case "无功误差":
                        PulseTypeValee = 1;
                        break;
                    case "有功脉冲计数":
                        PulseTypeValee = 6;
                        break;
                    case "无功脉冲计数":
                        PulseTypeValee = 7;
                        break;
                    case "时钟误差":
                        PulseTypeValee = 4;
                        break;
                    default:
                        break;
                }
            }
        }

        int PulseTypeValee = 0;
        #endregion


        /// <summary>
        /// 设置标准表标准常数
        /// </summary>
        public void SetStdConst()
        {
            long sstdConst = StdConst;
            double[] stdUIGear = new double[] { 0, 0, 0, 0, 0, 0 };
            EquipmentData.DeviceManager.StdGear(0x13, ref sstdConst, ref stdUIGear);
        }
        /// <summary>
        /// 设置误差板标准常数
        /// </summary>
        public void SetWCStdConst()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            int errStaconst = StdConst / 100;
            if (PulseType == "时钟误差") errStaconst = StdConst;
            for (int i = 0; i < num; i++)
            {
                EquipmentData.DeviceManager.SetStandardConst(PulseTypeValee, errStaconst, -2, (byte)0xFF, i);
                System.Threading.Thread.Sleep(150);
            }
        }
        /// <summary>
        /// 设置误差板被检常数
        /// </summary>
        public void SetWcTestConst()
        {
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            for (int i = 0; i < num; i++)
            {
                EquipmentData.DeviceManager.SetTestedConst(PulseTypeValee, TestedConst, 0, TestQs, (byte)0xFF, i);
                System.Threading.Thread.Sleep(150);
            }
        }

        bool WcStart = false;
        /// <summary>
        /// 启动误差板
        /// </summary>
        public void StartWc()
        {
            if (WcStart) return;
            if (PulseTypeValee != 4)
            {
                EquipmentData.DeviceManager.SetPulseType((PulseTypeValee + 49).ToString("x"));
            }
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            for (int i = 0; i < num; i++)
            {
                EquipmentData.DeviceManager.StartWcb(PulseTypeValee, (byte)0xFF, i);
                System.Threading.Thread.Sleep(150);
            }
            WcStart = true;
            //TODO 这里应该时时读取误差，然后刷新到界面上
            //Task.Factory.StartNew(()=> {
            //    while (true)
            //    {
            //        if (!WcStart) break;
            //        System.Threading.Thread.Sleep(1000);



            //    }
            //});


            //这里开始读取误差
        }
        /// <summary>
        /// 停止误差板
        /// </summary>
        public void StopWc()
        {
            WcStart = false;
            int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;
            for (int i = 0; i < num; i++)
            {
                EquipmentData.DeviceManager.StopWcb(PulseTypeValee, (byte)0xFF, i);
                System.Threading.Thread.Sleep(150);
            }

        }

        /// <summary>
        /// 根据电压电流获取常数
        /// </summary>
        public void GetStdConst()
        {
            StdConst = GetStdConst(EquipmentData.StdInfo.Ua, EquipmentData.StdInfo.Ia);
        }
        /// <summary>
        /// 固定常数下获取设置的常数值
        /// </summary>
        ///<param name="u">电压</param>
        /// <param name="i">电流</param>
        /// <returns></returns>
        private int GetStdConst(float u, float i)
        {
            int constants = 0;
            if (constants == 0)//0的情况就用这里设置的，否则用设置里面设置的
            {
                #region 获取电压电流的挡位

                //电压挡位
                int Gear_U = 2;  // 预设220V挡
                int Gear_I = 6;  // 预设220V挡
                if (u > 350)             // 380V
                    Gear_U = 3;
                else if (u > 144)         // 220V
                    Gear_U = 2;
                else if (u > 72)            // 100V
                    Gear_U = 1;
                else if (u > 0)               // 57.7V
                    Gear_U = 0;

                if (i > 60)                     // 100A
                    Gear_I = 6;
                else if (i > 30)               // 50A
                    Gear_I = 5;
                else if (i > 15)                // 20A
                    Gear_I = 5;
                else if (i > 7)                // 10A
                    Gear_I = 3;
                else if (i > 1.5)                   // 2.5A
                    Gear_I = 2;
                else if (i > 0.3)             // 0.5A
                    Gear_I = 1;
                else if (i > 0.03)          // 0.1A
                    Gear_I = 0;
                else            // 0.1A
                    Gear_I = 0;
                #endregion


                #region 根据电压电流挡位获取常数 --采用降一档
                switch (Gear_U)
                {
                    case 0:            // 57.7V
                        switch (Gear_I)
                        {
                            case 0: // 0.1A及以下
                                constants = 1600000000;
                                break;
                            case 1: // 0.5A
                                constants = 400000000;
                                break;
                            case 2: // 2.5A
                                constants = 100000000;
                                break;
                            case 3: // 10A
                                constants = 25000000;
                                break;
                            case 4: // 20A
                                constants = 8000000;
                                break;
                            case 5: // 50A
                                constants = 4000000;
                                break;
                            case 6: // 100A
                                constants = 4000000;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 1:          // 100V
                        switch (Gear_I)
                        {
                            case 0: // 0.1A及以下
                                constants = 800000000;
                                break;
                            case 1: // 0.5A
                                constants = 200000000;
                                break;
                            case 2: // 2.5A
                                constants = 50000000;
                                break;
                            case 3: // 10A
                                constants = 12500000;
                                break;
                            case 4: // 20A
                                constants = 4000000;
                                break;
                            case 5: // 50A
                                constants = 2000000;
                                break;
                            case 6: // 100A
                                constants = 2000000;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 2:       // 220V
                        switch (Gear_I)
                        {
                            case 0: // 0.1A及以下
                                constants = 400000000;
                                break;
                            case 1: // 0.5A
                                constants = 100000000;
                                break;
                            case 2: // 2.5A
                                constants = 25000000;
                                break;
                            case 3: // 10A
                                constants = 6000000;
                                break;
                            case 4: // 20A
                                constants = 2000000;
                                break;
                            case 5: // 50A
                                constants = 1000000;
                                break;
                            case 6: // 100A
                                constants = 1000000;
                                break;
                            default:
                                break;
                        }
                        break;
                    case 3:     // 380V
                        switch (Gear_I)
                        {
                            case 0: // 0.1A及以下
                                constants = 200000000;
                                break;
                            case 1: // 0.5A
                                constants = 50000000;
                                break;
                            case 2: // 2.5A
                                constants = 12000000;
                                break;
                            case 3: // 10A
                                constants = 3000000;
                                break;
                            case 4: // 20A
                                constants = 1000000;
                                break;
                            case 5: // 50A
                                constants = 500000;
                                break;
                            case 6: // 100A
                                constants = 500000;
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                #endregion
            }
            return constants;
        }
        #endregion
    }
}
