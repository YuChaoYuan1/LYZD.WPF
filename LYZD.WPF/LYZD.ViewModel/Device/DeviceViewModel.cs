using LYZD.Core.Enum;
using LYZD.DAL;
using LYZD.DAL.Config;
using LYZD.Utility;
using LYZD.Utility.Log;
using LYZD.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZH.MeterProtocol.Encryption;

namespace LYZD.ViewModel.Device
{
    public class DeviceViewModel : ViewModelBase
    {
        private bool isBusy;
        /// 正在忙碌
        /// <summary>
        /// 正在忙碌
        /// </summary>
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetPropertyValue(value, ref isBusy, "IsBusy"); }
        }



        #region 设备状态
        private bool isReady;
        /// <summary>
        /// 设备就绪,连接以后就可以下发开始检定命令了
        /// </summary>
        public bool IsReady
        {
            get { return isReady; }
            set
            {
                SetPropertyValue(value, ref isReady, "IsReady");
                EquipmentData.Controller.OnPropertyChanged("IsEnable");
            }
        }

        private bool? isConnected = null;
        /// <summary>
        /// 台体设备连接正常
        /// </summary>
        public bool? IsConnected
        {
            get { return isConnected; }
            set
            {
                SetPropertyValue(value, ref isConnected, "IsConnected");
            }
        }
        #endregion

        #region 表位信息
        private bool selectAll;
        public bool SelectAll
        {
            get { return selectAll; }
            set
            {
                SetPropertyValue(value, ref selectAll, "SelectAll");
                for (int i = 0; i < MeterUnits.Count; i++)
                {
                    //MeterUnits[i].IsSelected = value;
                }
            }
        }

        private AsyncObservableCollection<MeterUnitViewModel> meterUnits = new AsyncObservableCollection<MeterUnitViewModel>();
        /// <summary>
        /// 电能表端口数据
        /// </summary>
        public AsyncObservableCollection<MeterUnitViewModel> MeterUnits
        {
            get { return meterUnits; }
            set { SetPropertyValue(value, ref meterUnits, "MeterUnits"); }
        }
        #endregion

        #region 配置相关


        private AsyncObservableCollection<string> meterPorts = new AsyncObservableCollection<string>();

      

        /// 表位端口配置字符串
        /// <summary>
        /// 表位端口配置字符串
        /// </summary>
        public AsyncObservableCollection<string> MeterPorts
        {
            get { return meterPorts; }
            set { meterPorts = value; }
        }
        #endregion


        /// <summary>
        /// 解析设备操作命令
        /// </summary>
        /// <param name="deviceCommand">格式:{设备名}|{方法名}|{参数1}|{参数2}|{参数3}...</param>
        public override void CommandFactoryMethod(string deviceCommand)
        {
            TaskManager.AddWcfAction(() =>
            {
                string[] arrayCommand = deviceCommand.Split('|');
                #region 合法性校验
                if (string.IsNullOrEmpty(arrayCommand[0]))
                {
                    LogManager.AddMessage(string.Format("设备控制方法不能为空:{0}", arrayCommand[0]), EnumLogSource.用户操作日志, EnumLevel.Warning);
                    return;
                }
                #endregion

                #region 获取方法
                try
                {
                    Type[] typeArray = Type.EmptyTypes;
                    if (arrayCommand.Length > 1)
                    {
                        typeArray = new Type[arrayCommand.Length - 1];
                        for (int i = 1; i < arrayCommand.Length; i++)
                        {
                            typeArray[i - 1] = typeof(string);
                        }
                    }
                    MethodInfo method = GetType().GetMethod(arrayCommand[0], typeArray);
                    if (method == null)
                    {
                        LogManager.AddMessage(string.Format("没有找到方法:{0}", deviceCommand), EnumLogSource.设备操作日志, EnumLevel.Warning);
                        return;
                    }
                    try
                    {
                        object[] arrayParams = null;
                        if (arrayCommand.Length > 1)
                        {
                            arrayParams = new object[arrayCommand.Length - 1];
                            for (int j = 0; j < arrayParams.Length; j++)
                            {
                                arrayParams[j] = arrayCommand[1 + j];
                            }
                        }
                        IsBusy = true;
                        object objReturn = method.Invoke(this, arrayParams);
                        IsBusy = false;
                        if (objReturn is int)
                        {
                            int resultTemp = (int)objReturn;
                            if (resultTemp == 0)
                            {
                                LogManager.AddMessage(string.Format("调用台体操作方法{0}成功", deviceCommand), EnumLogSource.设备操作日志);
                            }
                            else
                            {
                                LogManager.AddMessage(string.Format("调用台体操作方法{0}失败,返回值:{1}", deviceCommand, objReturn), EnumLogSource.设备操作日志, EnumLevel.Warning);
                            }
                        }
                    }
                    catch (Exception methodEx)
                    {
                        IsBusy = false;
                        LogManager.AddMessage(string.Format("调用方法出现异常:{0}:{1}", deviceCommand, methodEx.Message), EnumLogSource.设备操作日志, EnumLevel.Warning, methodEx);
                    }
                }
                catch (AmbiguousMatchException e)
                {
                    IsBusy = false;
                    LogManager.AddMessage(string.Format("找到不止一个具有指定名称的方法:{0}", deviceCommand), EnumLogSource.设备操作日志, EnumLevel.Warning, e);
                }
                #endregion
            });
        }

        private bool isOpenExportData=false;
        /// <summary>
        /// 打开数据管理
        /// </summary>
        public void ExportData()
        {
            if (isOpenExportData)
            {
                return;
            }
            isOpenExportData = true;
            Process[] processes = Process.GetProcesses();
            Process processTemp = processes.FirstOrDefault(item => item.ProcessName == "LYZD.DataManage");
            if (processTemp == null)
            {
                try
                {
                    Process.Start(string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), "LYZD.DataManager.exe"));

                    //等待一会儿，让程序彻底启动了，免得同时打开多个了
                    int index = 5;
                    Task task = new Task(() =>
                    {
                        while (index-->0)
                        {
                            Thread.Sleep(1000);
                        }
                        isOpenExportData = false;
                    });
                    task.Start();
                }
                catch (Exception e)
                {
                    LogManager.AddMessage(string.Format("数据管理程序启动失败:{0}", e.Message), EnumLogSource.用户操作日志, EnumLevel.Error);
                    isOpenExportData = false;
                }
            }
        }

       


        /// 连接设备
        /// <summary>
        /// 连接设备
        /// </summary>
        public void Link()
        {
            if (EquipmentData.Equipment.IsDemo)
            {
                EquipmentData.DeviceManager.IsConnected = true;
                IsReady = true;
                LogManager.AddMessage("联机成功", EnumLogSource.设备操作日志, EnumLevel.Information);
                return;
            }

            //EquipmentData.DeviceManager.IsConnected = true;
            //IsReady = true;
            //return;

            //【标注005】
            if (ConnectDeviceAll())
            {
                EquipmentData.DeviceManager.IsConnected = true;
                IsReady = true;
                //PowerOn();
                //EquipmentData.Controller.DeviceStart =0;  //开始台体状态0
            }
            else
            {
                IMICP.OpenPortIMICP open = new IMICP.OpenPortIMICP();
                open.AlarmData("联机失败", "检查网络信息");
                IsReady = false;
                EquipmentData.DeviceManager.IsConnected = false;
                //TODO 假联机测试使用
                //EquipmentData.DeviceManager.IsConnected = true;
                //IsReady = true;
            }
        }


        /// <summary>
        /// 连接加密机
        /// </summary>
        public bool LinkDog()
        {
            if (ConfigHelper.Instance.Dog_Type == "无" || EquipmentData.Equipment.IsDemo)
            {
                return true;
            }
            string ip = ConfigHelper.Instance.Dog_IP;
            int port = int.Parse(ConfigHelper.Instance.Dog_Prot);
            //string usbKey = ConfigHelper.Instance.Dog_key;
            //_SoftType = ConfigHelper.Instance.Dog_ConnectType == "服务器版" ? EncryConnMode.服务器版 : EncryConnMode.直连密码机版;
           ;
            //if (!EncrypGW.Link())
            if (TerminalProtocol.Encryption.IEncryptionFunction698.ConnectDevice(ip, port.ToString(), ConfigHelper.Instance.Dog_Overtime) != 0)
            {
                LogManager.AddMessage("加密机连接失败!", EnumLogSource.设备操作日志, EnumLevel.Information);
                IsReady = false;
                EquipmentData.DeviceManager.IsConnected = false;
                return false;
            }
            LogManager.AddMessage("加密机连接成功!", EnumLogSource.设备操作日志, EnumLevel.Information);
            return true;
        }


        /// <summary>
        /// 断开所有设备
        /// </summary>
        public void UnLink()
        {
            if (IsReady)
            {
               UnLinkDeviceAll();
            }
            IsReady = false;
        }
        /// <summary>
        /// 设备集合字典
        /// </summary>
        public Dictionary<string, List<DeviceData>> Devices = new Dictionary<string, List<DeviceData>>();

        /// <summary>
        /// 设备数量
        /// </summary>
        public Dictionary<string, int> DeviceCount = new Dictionary<string, int>();
        /// <summary>
        ///载入设备
        /// </summary>
        public void LoadDevices()
        {
            #region 加载设备列表

            Devices.Clear();  //设备列表
            //DeviceCount.Clear();
            //DevicesModel.Clear();
            Dictionary<string, string> EquipmentList = CodeDictionary.GetLayer2("EquipmentType");
            string[] name = EquipmentList.Keys.ToArray<string>();
            for (int i = 0; i < EquipmentList.Count; i++)
            {
                //在数据库中找到对应的数据
                DynamicModel model = DALManager.ApplicationDbDal.GetByID(EnumAppDbTable.T_DEVICE_CONFIG.ToString(), string.Format("DEVICE_NAME = '{0}'", name[i]));

                if (model != null)
                {
                    bool IsExist = model.GetProperty("DEVICE_ENABLED").ToString() == "1" ? true : false; //是否可以用--就是掐前面的勾打不打上-
                    string[] DeviceItemData = model.GetProperty("DEVICE_DATA").ToString().Split('$');///根据$分割，获得该设备配置集合
                    if (!IsExist)    continue;
                    //循环获得该设备名称下的所有设备，比如有俩个误差板
                    for (int j = 0; j < DeviceItemData.Length; j++)
                    {
                        DeviceData device = new DeviceData(); //创建一个设备
                        string[] data = DeviceItemData[j].Split('|');//获得串口数据
                        if (data.Length > 5)
                        {
                            string comAddress = data[0];
                            string[] t = comAddress.Split('_');//开始端口远程端口ip
                            if (t.Length >= 3)
                            {
                                device.Address = t[0];
                                if (device.Address == "127.0.0.1")
                                {
                                    device.Conn_Type = CommMode.COM;
                                }
                                else
                                { 
                                    device.Conn_Type = CommMode.远程服务器;
                                }
                                device.StartPort = t[1];
                                device.RemotePort = t[2];
                            }
                            device.IsExist = IsExist;
                            device.Model = data[1];
                            device.ComParam = data[2];
                            device.PortNum = data[3];
                            device.MaxTimePerFrame = data[4];
                            device.MaxTimePerByte = data[5];
                            //先把该设备加入字典
                            if (!Devices.ContainsKey(name[i]))
                            {
                                Devices.Add(name[i], new List<DeviceData>());
                            }
                            Devices[name[i]].Add(device);   //添加该设备
                        }
                    }
                }
            }

            #endregion

            #region 电能表

            MeterUnits.Clear();  //表列表
            DynamicModel meter_Model = DALManager.ApplicationDbDal.GetByID(EnumAppDbTable.T_DEVICE_CONFIG.ToString(), "DEVICE_NAME ='电能表'");
            if (meter_Model != null)
            {
                //193.168.18.1_10003_20000|38400,n,8,1|1|1|24|3000|10
                string[] MeterItemData = meter_Model.GetProperty("DEVICE_DATA").ToString().Split('$');///根据$分割
                for (int j = 0; j < MeterItemData.Length; j++)
                {
                    string[] data = MeterItemData[j].Split('|');//获得串口数据

                    if (data.Length > 6)
                    {
                        int start = int.Parse(data[2]);
                        int interval = int.Parse(data[3]);
                        int number = int.Parse(data[4]);
                        int value = 1;
                        while (value <= number)
                        {
                            MeterUnitViewModel meterItems = new MeterUnitViewModel();
                            meterItems.ComParam = data[1];
                            meterItems.PortNum = (start + (value - 1) * interval).ToString();
                            string[] t = data[0].Split('_');//开始端口远程端口ip
                            if (t.Length >= 3)
                            {
                                meterItems.Address = t[0];
                                if (meterItems.Address == "127.0.0.1")
                                {
                                    meterItems.Conn_Type = CommMode.COM;
                                }
                                else
                                {
                                    meterItems.Conn_Type = CommMode.远程服务器;
                                }
                                meterItems.StartPort = t[1];
                                meterItems.RemotePort = t[2];
                            }
                            meterItems.MaxTimePerFrame = data[5];
                            meterItems.MaxTimePerByte = data[6];
                            MeterUnits.Add(meterItems);
                            value++;
                        }
                    }
                    //System.IO.Ports.SerialPort serial = new System.IO.Ports.SerialPort();
                }

                LogManager.AddMessage("设备端口数据加载完成", EnumLogSource.设备操作日志);
                CheckController.MeterProtocolAdapter.Instance.SetBwCount(MeterUnits.Count);

            }
            #endregion

        }


        #region 设备控制部分
        /// <summary>
        /// 初始化所有设备
        /// </summary>
        /// <returns></returns>
        public void InitializeDevice()
        {
            if (EquipmentData.Equipment.IsDemo)
                return;
            bool IsOk = true;

            foreach (string key in Devices.Keys)
            {
                List<DeviceData> device = Devices[key]; //获得这个名称下的所有设备
                for (int i = 0; i < device.Count; i++)    //循环所有设备，进行初始化
                {
                    Type type = GetReflexObject(device[i].Model);//获得设备的实例
                    if (type != null)
                    {
                        device[i].Type = type;   //保存设备的实例
                        device[i].Obj = Activator.CreateInstance(type);// 创建此类型实例
                        string Conn_Type = "InitSetting";
                        if (device[i].Conn_Type ==CommMode.COM)
                        {
                            Conn_Type = "InitSettingCom";
                        }
                        MethodInfo mInfo = type.GetMethod(Conn_Type); //获取当前方法
                        int prot = Convert.ToInt32(device[i].PortNum);
                        int MaxWaitTime = Convert.ToInt32(device[i].MaxTimePerFrame);
                        int WaitSencondsPerByte = Convert.ToInt32(device[i].MaxTimePerByte);
                        object[] s;
                        if (device[i].Conn_Type == CommMode.COM)
                        {
                            s = new object[3] { prot, MaxWaitTime, WaitSencondsPerByte };
                        }
                        else
                        {
                            string  Ip =device[i].Address ;
                            int RemotePort = Convert.ToInt32(device[i].RemotePort );
                            int LocalStartPort = Convert.ToInt32(device[i].StartPort);
                            s = new object[6] { prot, MaxWaitTime, WaitSencondsPerByte, Ip , RemotePort, LocalStartPort };
                        }
                      
                        mInfo.Invoke(device[i].Obj, s);  //接收调用返回值，判断调用是否成功  new object[1] {5}
                        device[i].InitialStatus = true;
                        //if (device.Count > 1)  //设备数量大于1的化用第几路设备进行标识
                        //    LogManager.AddMessage($"第{i + 1}路{key}初始化成功", EnumLogSource.设备操作日志, EnumLevel.Information);
                        //else
                        //    LogManager.AddMessage(key + "初始化成功", EnumLogSource.设备操作日志, EnumLevel.Information);
                    }
                    else
                    {
                        IsOk = false;
                        device[i].InitialStatus = false;
                        if (device.Count > 1)  //设备数量大于1的化用第几路设备进行标识
                            LogManager.AddMessage($"第{i+1}路{key}初始化失败", EnumLogSource.设备操作日志, EnumLevel.TipsError );
                        else
                          LogManager.AddMessage(key + "初始化失败", EnumLogSource.设备操作日志, EnumLevel.TipsError);
                    }
                }
            }

            if (IsOk)
            {
                LogManager.AddMessage("设备初始化成功", EnumLogSource.设备操作日志, EnumLevel.Information);
            }
        }

        /// <summary>
        /// 连接所有设备
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 连接所有设备
        /// </summary>
        /// <returns></returns>
        private bool ConnectDeviceAll()
        {
            int RetryCount = 2;//重试次数
            bool IsOk = true;
            foreach (string key in Devices.Keys)
            {
                List<DeviceData> device = Devices[key]; //获得这个名称下的所有设备
                for (int i = 0; i < device.Count; i++)    //循环所有设备，进行开始联机
                {
                    for (int t = 1; t <= RetryCount; t++)//重试次数
                    {
                        if (!device[i].Status && device[i].InitialStatus)  //没有联机成功，并且初始化成功
                        {
                            if (DeviceName.误差板 == key)//误差板的情况特别处理
                            {
                                if (device.Count > 1)  //设备数量大于1的化用第几路设备进行标识
                                    LogManager.AddMessage($"正在联机第{i + 1}路{key}...", EnumLogSource.设备操作日志, EnumLevel.Information);
                                else
                                    LogManager.AddMessage("正在联机" + key + "...", EnumLogSource.设备操作日志, EnumLevel.Information);
                                //MeterUnits.Count / device.Count;//电表数量除以误差板数量
                                int num = EquipmentData.Equipment.MeterCount / device.Count;
                                string err = "";
                                for (int m = i * num; m < num * i + num - 1; m++) //循环该路误差板上所有表位
                                {
                                    MethodInfo mInfo = device[i].Type.GetMethod("Connect"); //获取当前方法
                                    string[] FrameAry = { };
                                    object[] parameters = new object[2] { (byte)(m + 1), FrameAry };
                                    int connOK = (int)mInfo.Invoke(device[i].Obj, parameters);  //接收调用返回值，判断调用是否成功  new object[1] {5}
                                    if (connOK != 0)
                                    {
                                        err += $"【{m + 1}】";
                                    }
                                }
                                if (err != "")
                                {
                                    //err = "表位：" + err + "误差板连接失败！！！";
                                    if (t == RetryCount)
                                    {
                                        if (device.Count > 1)  //设备数量大于1的化用第几路设备进行标识
                                            LogManager.AddMessage($"第{i + 1}路{key}联机失败,错误表位:{err}", EnumLogSource.设备操作日志, EnumLevel.TipsError);
                                        else
                                        {
                                            IMICP.OpenPortIMICP open = new IMICP.OpenPortIMICP();
                                            open.AlarmData($"第{i + 1}路{key}联机失败,错误表位:{err}", "检查网络信息");
                                            LogManager.AddMessage(key + "联机失败,错误表位" + err, EnumLogSource.设备操作日志, EnumLevel.TipsError);
                                        }

                                        device[i].Status = false;
                                        IsOk = false; //有一个设备联机不成功，就是不成功
                                    }
                                    else
                                    {
                                        LogManager.AddMessage(key + $"第{t}次联机失败,错误表位:{err}", EnumLogSource.设备操作日志, EnumLevel.TipsError);
                                    }
                                }
                                else
                                {
                                    if (device.Count > 1)  //设备数量大于1的化用第几路设备进行标识
                                        LogManager.AddMessage($"第{i + 1}路{key}联机成功", EnumLogSource.设备操作日志, EnumLevel.Information);
                                    else
                                        LogManager.AddMessage(key + "联机成功", EnumLogSource.设备操作日志, EnumLevel.Information);
                                    device[i].Status = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (device.Count > 1)  //设备数量大于1的化用第几路设备进行标识
                                    LogManager.AddMessage($"正在联机第{i + 1}路{key}...", EnumLogSource.设备操作日志, EnumLevel.Information);
                                else
                                    LogManager.AddMessage("正在联机" + key + "...", EnumLogSource.设备操作日志, EnumLevel.Information);
                                MethodInfo mInfo = device[i].Type.GetMethod("Connect"); //获取当前方法
                                string[] FrameAry = { };
                                object[] parameters = new object[1] { FrameAry };
                                int connOK = (int)mInfo.Invoke(device[i].Obj, parameters);  //接收调用返回值，判断调用是否成功  new object[1] {5}
                                if (connOK == 0)
                                {
                                    if (device.Count > 1)  //设备数量大于1的化用第几路设备进行标识
                                        LogManager.AddMessage($"第{i + 1}路{key}联机成功", EnumLogSource.设备操作日志, EnumLevel.Information);
                                    else
                                        LogManager.AddMessage(key + "联机成功", EnumLogSource.设备操作日志, EnumLevel.Information);
                                    device[i].Status = true;
                                    break;
                                }
                                else
                                {
                                    if (t == RetryCount)
                                    {
                                        if (device.Count > 1)  //设备数量大于1的化用第几路设备进行标识
                                            LogManager.AddMessage($"第{i + 1}路{key}联机失败", EnumLogSource.设备操作日志, EnumLevel.TipsError);
                                        else
                                        {
                                            IMICP.OpenPortIMICP open = new IMICP.OpenPortIMICP();
                                            open.AlarmData($"第{i + 1}路{key}联机失败", "检查网络信息");
                                            LogManager.AddMessage(key + "联机失败", EnumLogSource.设备操作日志);
                                        }

                                        device[i].Status = false;
                                        IsOk = false; //有一个设备联机不成功，就是不成功
                                    }
                                    else
                                    {

                                        LogManager.AddMessage(key + $"第{t}次联机失败", EnumLogSource.设备操作日志, EnumLevel.TipsError);
                                    }
                                }
                            }

                        }
                    }
                }
            }

            GetStdData();//启动时时读取标准表线程
            //if (IsOk)
            //{
            //    LogManager.AddMessage("联机成功", EnumLogSource.设备操作日志, EnumLevel.Information);
            //}
            return IsOk;
        }


        private void UnLinkDeviceAll()
        {
            try
            {
                if (Devices.ContainsKey(DeviceName.多功能板))
                {
                    EquipmentData.DeviceManager.Set_Mie();
                }
                if (Devices.ContainsKey(DeviceName.功率源))
                {
                    if (Devices[DeviceName.功率源][0].Status)
                    {
                        if (ConfigHelper.Instance.IsTipsPowerOff)
                        {
                            if (System.Windows.MessageBox.Show("是否关源", "提示", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Information) != System.Windows.MessageBoxResult.Yes)
                            {
                                return;
                            }
                        }
                        //关源
                        PowerOff();
                    }
                }
            }
            catch
            {
            }
        }



        /// <summary>
        /// 获得一个设备类的类型
        /// </summary>
        /// <returns></returns>
        public Type GetReflexObject(string deviceName)
        {
            try
            {
                //string filaName = $"LYZD.{deviceName}";
                string filaName = $"{deviceName}";
                string filePath = "Devices\\"+filaName + ".dll";
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                byte[] bFile = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();

                Assembly assembly = Assembly.Load(bFile);
                //string className = filaName+ "." + deviceName;
                string className = "ZH." + deviceName;    //ZH.3001
                Type type = assembly.GetType(className); //获取当前类的类型
                return type;

            }
            catch (Exception e)
            {
                LogManager.AddMessage("调用设备方法失败!错误代码20001-002:\r\n" + e, EnumLogSource.检定业务日志, EnumLevel.Error);
                return null;
            }

        }

          /// <summary>
          /// 反射设备方法
          /// </summary>
          /// <param name="deviceName">设备名称</param>
          /// <param name="functionName">方法名称</param>
          /// <param name="ID">设备ID,从0开始</param>
          /// <param name="value"></param>
          /// <returns></returns>
        private object DeviceControl(string deviceName, string functionName, int ID, params object[] value)
        {
            try
            {
                //这里需要修改，根据名称从设备列表中找到他的类型
                if (!Devices.ContainsKey(deviceName))
                {
                    LogManager.AddMessage($"没有添加设备【{deviceName}】", EnumLogSource.设备操作日志, EnumLevel.TipsError );
                    return false;
                }
                List<DeviceData> deviceList = Devices[deviceName];
                if (ID>= deviceList.Count)
                {
                    LogManager.AddMessage($"没有添加第{ID}路【{deviceName}】", EnumLogSource.设备操作日志, EnumLevel.TipsError);
                    return false;
                }
                DeviceData device = deviceList[ID];
                //DeviceData device = devices.FirstOrDefault(item => item.Model == deviceName);
                //if (device == null)   //没有找到对应的类型
                //{
                //    return false;
                //}
                if (device.Type == null)//联机出现问题，没有实例化
                {
                    //Lo15gManager.AddMessage($"{deviceName}还未连接", EnumLogSource.设备操作日志, EnumLevel.Information);
                    return false;
                }
                //if (device.Status==false)
                //{
                //    LogManager.AddMessage($"{deviceName}没有联机", EnumLogSource.设备操作日志, EnumLevel.Information);
                //    return false;
                //}
                MethodInfo mInfo = device.Type.GetMethod(functionName);
                return mInfo.Invoke(device.Obj, value);
            }
            catch (Exception e)
            {
                LogManager.AddMessage("调用设备方法失败!错误代码20001-001:" + e, EnumLogSource.设备操作日志, EnumLevel.Error);
                return false;
            }
        }


        #endregion


        #region 设备

        #region 功率源

        /// <summary>
        ///  设置谐波
        /// </summary>
        /// <param name="ua"> 谐波设置对象 Ua ；1-设置 ；0-不设置</param>
        /// <param name="ub">谐波设置对象 Ub ；1-设置 ；0-不设置</param>
        /// <param name="uc">谐波设置对象 Uc ；1-设置 ；0-不设置</param>
        /// <param name="ia">谐波设置对象 ia ；1-设置 ；0-不设置</param>
        /// <param name="ib">谐波设置对象 ib ；1-设置 ；0-不设置</param>
        /// <param name="ic">谐波设置对象 ic ；1-设置 ；0-不设置</param>
        /// <param name="harmonicType"> 特殊谐波：0表示正常谐波；1表示方形波，2表示尖顶波，3表示次谐波，4表示奇次谐波，5表示偶次谐波。</param>
        /// <returns></returns>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool ZH3001SetPowerHarmonic(string ua, string ub, string uc, string ia, string ib, string ic, int HarmonicType, int ID = 0)
        {
            string[] FrameAry = { };
            object[] paras = new object[] { ua, ub, uc, ia, ib, ic, HarmonicType, FrameAry };
            string value = DeviceControl(DeviceName.功率源, "setZH3001PowerHarmonic", ID, paras).ToString();
            if (value == "0")
                return true;
            return false;
        }

        //谐波-常规谐波
        public bool ZH3001SetPowerGetHarmonic(string ua, string ub, string uc, string ia, string ib, string ic, float[] HarmonicContent,
            float[] HarmonicPhase, bool OnOff, int ID = 0)
        {
            int v = OnOff ? 1 : 0;
            string[] FrameAry = { };
            object[] paras = new object[] { ua, ub, uc, ia, ib, ic, v, HarmonicContent, HarmonicPhase, FrameAry };
            string value = DeviceControl(DeviceName.功率源, "setZH3001PowerGetHarmonic", ID, paras).ToString();
            if (value == "0")
                return true;
            return false;
        }
        public bool PowerOn(int jxfs, double Ua, double Ub, double Uc, double Ia, double Ib, double Ic, double PhiUa, double PhiUb,
                            double PhiUc, double PhiIa, double PhiIb, double PhiIc, double Freq, int Mode,int ID=0)
        {
            string[] FrameAry = null;
            object[] paras = new object[] { jxfs, Ua, Ub, Uc, Ia, Ib, Ic, PhiUa, PhiUb, PhiUc, PhiIa, PhiIb, PhiIc, Freq, Mode, FrameAry };
            string value = DeviceControl(DeviceName.功率源, "PowerOn", ID, paras).ToString();
            if (value == "0")
                return true;
            return false;
        } 
        /// <summary>
        /// 
        /// CL309升源
        /// </summary>
        /// <param name="clfs">测量方式</param>
        /// <param name="glfx">功率方向</param>
        /// <param name="strGlys">功率因数</param>
        /// <param name="sng_xUb_A">A相电压</param>
        /// <param name="sng_xUb_B">B相电压</param>
        /// <param name="sng_xUb_C">C相电压</param>
        /// <param name="sng_xIb_A">A相电流</param>
        /// <param name="sng_xIb_B">B相电流</param>
        /// <param name="sng_xIb_C">C相电流</param>
        /// <param name="element">功率元件、H元、A元、B元、C元</param>
        /// <param name="sng_Freq">频率</param>
        /// <param name="bln_IsNxx">是否为逆相序</param>
        public bool CL309PowerOn(int clfs, int glfx, string strGlys, float sng_xUb_A, float sng_xUb_B, float sng_xUb_C, float sng_xIb_A, float sng_xIb_B, float sng_xIb_C, int element, float sng_Freq, bool bln_IsNxx,int ID=0)
        {
            string[] FrameAry = null;
            object[] paras = new object[] { clfs, glfx, strGlys, sng_xUb_A, sng_xUb_B, sng_xUb_C, sng_xIb_A, sng_xIb_B, sng_xIb_C, element, sng_Freq, bln_IsNxx,FrameAry };
            string value = DeviceControl(DeviceName.功率源, "PowerOn",ID, paras).ToString();
            if (value == "0")
                return true;
            return false;
        }
        /// <summary>
        /// 升源
        /// </summary>
        /// <returns></returns>
        public bool PowerOn(int ID=0)
        {
            string[] FrameAry = { };
            object[] paras = new object[2] { 1, FrameAry };
            string value = DeviceControl(DeviceName.功率源, "PowerOn_Off", ID, paras).ToString();
            if (value == "0")
                return true;
            return false;
        }

        /// <summary>
        /// 关源
        /// </summary>
        /// <returns></returns>
        public bool PowerOff(int ID=0)
        {
            string[] FrameAry = { };
            object[] paras = new object[2] { 0, FrameAry };
            string value = DeviceControl(DeviceName.功率源, "PowerOn_Off", ID,paras).ToString();
            if (value == "0")
                return true;
            return false;
        }

        /// <summary>
        /// 交流过量
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool SetExcessive(byte type, int bs, int cs, int time, int ontime, byte v, int ID = 0)
        {
            string[] FrameAry = { };
            object[] paras = new object[] { type, bs, cs, time, ontime, v, FrameAry };
            string value = DeviceControl(DeviceName.功率源, "SetExcessive", ID, paras).ToString();
            if (value == "0")
                return true;
            return false;
        }

        public bool GearlLock(Double u,Double i, byte v, byte ID = 0)
        {
            string[] FrameAry = { };
            object[] paras = new object[] { u, i, v, FrameAry };
            string value = DeviceControl(DeviceName.功率源, "GearlLock", ID, paras).ToString();
            if (value == "0")
                return true;
            return false;
        }
        #endregion

        #region 标准表


        #region 时时读取标准表

        //设置常数，读取测量值，启动走字，读电能
        //ReadstdZH311Param

        /// <summary>
        /// 是否以及开启了读取线程
        /// </summary>
        bool IsRunStdData = false;
        private void GetStdData()
        {
            if (IsRunStdData) return;
            if (Devices.ContainsKey(DeviceName.标准表))
            {
                List<DeviceData> device = Devices[DeviceName.标准表]; //获得这个名称下的所有设备
                for (int i = 0; i < device.Count; i++)
                {
                    if (!device[i].Status)
                    {
                        return;
                    }
                }
            }
            if (ConfigHelper.Instance.IsReadStd)   //自动读取标准表数据
            {
                DeviceData device = Devices[DeviceName.标准表][0];
                int value = ConfigHelper.Instance.Std_RedInterval;
                string name = device.Model;
                IsRunStdData = true;
                Task task = new Task(() =>
                {
                    if (device.Model == "ZH311")
                    {
                        RefStd();//311标准表
                    }
                    else
                    {
                        RefStdLd(); //雷电标准表
                    }
                    //while (true)
                    //{
                    //    if (EquipmentData.ApplicationIsOver == true) break;
                    //    Thread.Sleep(value); //标准表读取间隔
                    //    if (device.Model == "ZH311")
                    //    {
                    //        RefStd();//311标准表
                    //    }
                    //    else
                    //    {
                    //        RefStdLd(); //雷电标准表
                    //    }
                    //    if (EquipmentData.ApplicationIsOver == true) break;
                    //}
                });
                task.Start();
            }
        }

        /// <summary>
        /// 读取ZH311瞬时测量数据
        /// </summary>
        /// <returns></returns>
        public float[] Readstd()
        {
            object[] paras = null;
            object data = DeviceControl(DeviceName.标准表 , "ReadstdZH311Param", 0,paras);
            float[] f = (float[])data;
            return f;
        }

        /// <summary>
        /// 刷新标准表数据
        /// </summary>
        public void RefStd()
        {
            int value = ConfigHelper.Instance.Std_RedInterval;
            while (true)
            {
                if (EquipmentData.ApplicationIsOver == true) break;
                try
                {
                    //return;
                    float[] floatArray = Readstd();
                    if (floatArray.Count(a=>a==0)== floatArray.Length)  //全部都为0
                    {

                    }
                    if (floatArray != null && floatArray.Length > 28)
                    {
                        EquipmentData.StdInfo.Ua = floatArray[0];
                        EquipmentData.StdInfo.Ub = floatArray[2];
                        EquipmentData.StdInfo.Uc = floatArray[4];
                        EquipmentData.StdInfo.Ia = floatArray[1];
                        EquipmentData.StdInfo.Ib = floatArray[3];
                        EquipmentData.StdInfo.Ic = floatArray[5];

                        EquipmentData.StdInfo.PhaseUa = floatArray[6];
                        EquipmentData.StdInfo.PhaseUb = floatArray[8];
                        EquipmentData.StdInfo.PhaseUc = floatArray[10];

                        EquipmentData.StdInfo.PhaseIa = floatArray[7];
                        EquipmentData.StdInfo.PhaseIb = floatArray[9];
                        EquipmentData.StdInfo.PhaseIc = floatArray[11];

                        EquipmentData.StdInfo.PFA = floatArray[13];
                        EquipmentData.StdInfo.PFB = floatArray[14];
                        EquipmentData.StdInfo.PFC = floatArray[15];
                        EquipmentData.StdInfo.PF = floatArray[16];


                        EquipmentData.StdInfo.PhaseA = ConvertPharse(floatArray[6] - floatArray[7]);
                        EquipmentData.StdInfo.PhaseB = ConvertPharse(floatArray[8] - floatArray[9]);
                        EquipmentData.StdInfo.PhaseC = ConvertPharse(floatArray[10] - floatArray[11]);

                        EquipmentData.StdInfo.Pa = floatArray[17];
                        EquipmentData.StdInfo.Pb = floatArray[20];
                        EquipmentData.StdInfo.Pc = floatArray[23];

                        EquipmentData.StdInfo.Qa = floatArray[18];
                        EquipmentData.StdInfo.Qb = floatArray[21];
                        EquipmentData.StdInfo.Qc = floatArray[24];

                        EquipmentData.StdInfo.Sa = floatArray[19];
                        EquipmentData.StdInfo.Sb = floatArray[22];
                        EquipmentData.StdInfo.Sc = floatArray[25];


                        EquipmentData.StdInfo.Freq = floatArray[12];
                        EquipmentData.StdInfo.P = floatArray[26];
                        EquipmentData.StdInfo.Q = floatArray[27];
                        EquipmentData.StdInfo.S = floatArray[28];
                    }
                    else
                    {
                        if (floatArray != null)
                        {
                            LogManager.AddMessage("标准表数据解析失败,值：" + string.Join(",",floatArray ), EnumLogSource.设备操作日志, EnumLevel.Warning); ;
                        }
                        else
                        { 
                          LogManager.AddMessage("标准表数据解析失败,读回来值为空", EnumLogSource.设备操作日志, EnumLevel.Warning );
                        }
                    }
                }
                catch (Exception ex)
                {
                   LogManager.AddMessage("标准表数据读取失败" + ex, EnumLogSource.设备操作日志, EnumLevel.Warning );
                }
                if (EquipmentData.ApplicationIsOver == true) break;
                Thread.Sleep(value); //标准表读取间隔
            }


        }
        #endregion

        private float ConvertPharse(float v)
        {
            if (v < 0)
            {
                v += 360;
            }
            return v;
        }


        /// <summary>
        ///读取累积电量
        /// </summary>
        /// <returns >返回值float数组</returns>
        public float[] ReadEnergy()
        {
            float[] flaEnergy = null;
            string[] FrameAry = null;
            object[] paras = new object[] { flaEnergy, FrameAry };
            //DeviceControl(DevicesModel["标准表"], "readEnergy", paras);
            //TODO2不知道有没问题，需要测试
            StdControlBase("readEnergy", paras).ToString();  //新--加入了标准表状态判断
            return flaEnergy;
        }


        /// <summary>
        /// 1008H- 档位常数 读取与设置
        /// </summary>
        /// <param name="stdCmd">>0x10 读 ，0x13写</param>
        /// <param name="stdConst">常数</param>
        /// <param name="stdUIGear">电压电流挡位UA，ub，uc，ia，ib，ic</param>
        /// <returns></returns>
        public bool StdGear(byte stdCmd,ref long  stdConst,ref double[] stdUIGear)
        {
            string[] FrameAry = null;

            if (!ConfigHelper.Instance.GearModel)   //判断是不是自动挡位
            {
                for (int i = 0; i < stdUIGear.Length; i++)
                {
                    stdUIGear[i] = 0;
                }
            }
            else
            {
                double ia =Math.Max( Math.Max(stdUIGear[3], stdUIGear[4]), stdUIGear[5]);
                double isa;
                if (ia >= 10)
                    isa = 14;
                else if (ia >=2.5)
                    isa = 11;
                else if (ia >= 0.5)                             
                    isa = 9;
                else if (ia >=0.1)
                    isa =7;
                else 
                    isa = 5;
                stdUIGear[0] =0;
                stdUIGear[1] =isa;
                stdUIGear[2] =0;
                stdUIGear[3] = isa;
                stdUIGear[4] = 0;
                stdUIGear[5] = isa;
            }

            object[] paras = new object[] { stdCmd, stdConst, stdUIGear, FrameAry };
            string value = StdControlBase("stdGear2", paras).ToString();  //新--加入了标准表状态判断
            long.TryParse(paras[1].ToString(), out stdConst);
            if (value == "0")
                return true;
            return false;
        }
        /// <summary>
        /// 初始化标准表
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="MaxWaitTime">最大等待时间</param>
        /// <param name="WaitSencondsPerByte">帧最大等待时间</param>
        /// <returns></returns>
        public bool InitSettingCom(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte)
        {
            object[] paras = new object[] { ComNumber, MaxWaitTime, WaitSencondsPerByte };
            string value = DeviceControl(DeviceName.标准表, "InitSettingCom", 0,paras).ToString();
            if (value == "0")
                return true;
            return false;
        }

        /// <summary>
        ///  100bH-模式设置--底层有问题
        /// </summary>
        /// <param name="stdCmd"></param>
        /// <param name="strModeJxfs"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public bool stdMode(byte stdCmd, ref string[] strModeJxfs)
        {
                  //            明：自动手动模式标识字符’1’表示手动模式，字符’0’表示自动模式（ascii 码读取）
                  //     接线方式模式标识字符’1’表示3相4线制，字符’2’表示3相3线制（ascii 码读取）
                  //标准表模式标识字符’1’表示单相表，字符’3’表示三相表，写操作此位置填0（ascii 码读取）

            //string[] FrameAry = null;
            object[] paras = new object[1];// { ComNumber, MaxWaitTime, WaitSencondsPerByte , FrameAry };
            //string value = DeviceControl(DevicesModel["标准表"], "stdMode", paras).ToString();   、
            string value = StdControlBase("stdMode", paras).ToString();  //新--加入了标准表状态判断
            if (value == "0")
                return true;
            return false;
        }

        /// <summary>
        /// 100cH-启停标准表累积电能
        /// </summary>
        /// <param name="startOrStopStd">字符’1’表示清零当前电能并开始累计（ascii 码读取）</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public bool startStdEnergy(int startOrStopStd)
        {
            string[] FrameAry = null;
            object[] paras = new object[] { startOrStopStd, FrameAry };
            //string value = DeviceControl(DevicesModel["标准表"], "startStdEnergy", paras).ToString();
            string value = StdControlBase("startStdEnergy", paras).ToString();  //新--加入了标准表状态判断
            if (value == "0")
                return true;
            return false;
        }


        /// <summary>
        /// 脉冲校准误差 字符串形式(10字节)W， 单位 分数值。
        /// 误差板的值 × 1000000 下发，譬如误差板计算的误差0.01525%，则0.01525% ×1000000 = 152.5，
        /// 则下发字符串 ”152.5”。
        /// </summary>
        ///  <param name="Error">误差板的值</param>
        /// <returns></returns>
        public bool SetPulseCalibration(string Error)
        {
            object[] paras = new object[] { Error };
            string value = StdControlBase("SetPulseCalibration", paras).ToString();  //新--加入了标准表状态判断
            if (value == "0")
                return true;
            return false;
        }

        /// <summary>
        ///   设置脉冲
        /// </summary>
        /// <param name="pulseType">
        ///有功脉冲 设置字符’1’
        ///无功脉冲 设置字符’2’
        ///UA脉冲 设置字符’3’
        ///UB脉冲 设置字符’4’
        ///UC脉冲 设置字符’5’
        ///IA脉冲 设置字符’6’
        ///IB脉冲 设置字符’7’
        ///IC脉冲 设置字符’8’
        ///PA脉冲 设置字符’9’
        ///PB脉冲 设置字符’10’
        ///PC脉冲 设置字符’11’
        ///</param>
        /// <returns></returns>
        public bool SetPulseType(string pulseType)
        {
            object[] paras = new object[] { pulseType };
            //string value = DeviceControl(DevicesModel["标准表"], "SetPulseType", paras).ToString();
            string value = StdControlBase("SetPulseType", paras).ToString();  //新--加入了标准表状态判断
            if (value == "0")
                return true;
            return false;
        }

        /// <summary>
        /// 1004H-谐波含量
        /// </summary>
        /// <returns></returns>
        public float[] ReadHarmonicEnergy()
        {
            string[] FrameAry = null;
            float[] value = null;
            object[] paras = new object[] { value, FrameAry };
            StdControlBase("readHarmonicEnergy", paras);
            return (float[])paras[0];
            //返回值--0-6 a相U基波幅值-a相I。。。
            //7-12 a相u的第二次谐波含量--a相I的第二次谐波含量。。。最多64次
        }

        /// <summary>
        /// 1005H-谐波相位
        /// </summary>
        /// <returns></returns>
        public float[] ReadHarmonicAngle()
        {
            string[] FrameAry = null;
            float[] value = null;
            object[] paras = new object[] { value, FrameAry };
            StdControlBase("readHarmonicAngle", paras);
            return (float[])paras[0];
        }


        private object StdControlBase(string FunName,object[] paras)
        {
            object obj= DeviceControl(DeviceName.标准表, FunName,0, paras);     //操作过程
            return obj;
        }


        #endregion

        #region 误差板   控制继电器，设置标准参数，设置被检常数，读取计算值，启动，停止

        /// <summary>
        /// 设置脉冲输出（两组脉冲频率可以不一样，但不能超过 500 倍）
        /// </summary>
        /// <param name="contrnlType">0x00=两组都不输出脉冲；0x01=仅第一组输出设定脉冲；0x02=仅第二组输出设定脉冲；0x03=两组都输出设定脉冲；</param>
        /// <param name="bwNum">表位号</param>
        /// <param name="fq1">第一组脉冲-频率--比如1000HZ</param>
        /// <param name="PWM1">第一组脉冲-占空比-0.5就是百分50</param>
        /// <param name="PulseNum1">第一组脉冲-脉冲个数--0表示连续输出</param>
        /// <param name="fq2">第二组脉冲-频率--比如1000HZ</param>
        /// <param name="PWM2">第二组脉冲-占空比-0.5就是百分50</param>
        /// <param name="PulseNum2">第二组脉冲-脉冲个数--0表示连续输出</param>
        /// <returns></returns>
        public bool SetPulseOutput(byte contrnlType, byte bwNum, float fq1, float PWM1, int PulseNum1, float fq2, float PWM2, int PulseNum2, int ID = 0)
        {
            object[] paras = new object[] { contrnlType, bwNum , fq1 , PWM1 , PulseNum1,fq2, PWM2, PulseNum2 };
            DeviceControl(DeviceName.误差板, "SetPulseOutput", ID, paras);
            return true;
        }

        /// <summary>
        /// 控制表位继电器 
        /// </summary>
        /// <param name="contrnlType">控制类型--1开启-2关闭</param>
        /// <param name="EpitopeNo">表位编号0xFF为广播</param>
        public void ControlMeterRelay(int contrnlType, byte EpitopeNo,int ID=0)
        {
            string[] FrameAry = null;
       
           
            object[] paras = new object[] { contrnlType, EpitopeNo, (byte)0x01, (byte)0x01, FrameAry };
            DeviceControl(DeviceName.误差板, "ContnrlBw", ID, paras);
        }

        /// <summary>
        /// 控制通讯继电器切换
        /// </summary>
        /// <param name="contrnlType">0—表位485通信；1—表位 232 或者蓝牙48通信；2—蓝牙232通信；</param>
        /// <param name="EpitopeNo">表位编号0xFF为广播</param>
        public void ControlConnrRelay(int contrnlType, byte EpitopeNo, int ID = 0)
        {
            string[] FrameAry = null;
            byte[] data = new byte[2];
            for (int i = 0; i < data.Length; i++)
            {
                if (contrnlType == 0)
                    data[i] = 0x00;
                else if (contrnlType == 1)
                    data[i] = 0x01;
                else
                    data[i] = 0x02;
            }
            object[] paras = new object[] {4, EpitopeNo, data, (byte)0x01, FrameAry };
            DeviceControl(DeviceName.误差板, "ContnrlBw", ID, paras);
        }
        /// <summary>
        /// 控制通讯继电器切换2--混合终端 -控制1路485切换
        /// </summary>
        /// <param name="contrnlType_1">0=485I切换到485通讯，1=485I切换到蓝牙通讯方式</param>
        /// <param name="EpitopeNo">表位编号0xFF为广播</param>
        public void ControlConnrRelay2(int contrnlType,byte EpitopeNo, int ID = 0)
        {
            string[] FrameAry = null;
            byte[] data = new byte[3];
            if (contrnlType == 1)
                data[0] = 0x01;

            object[] paras = new object[] { 9, EpitopeNo, data, (byte)0x01, FrameAry };
            DeviceControl(DeviceName.误差板, "ContnrlBw", ID, paras);
        }
        /// <summary>
        /// 控制通讯继电器切换2--混合终端  -控制2路485切换
        /// </summary>
        /// <param name="contrnlType_1">0=485I切换到485_2通讯，1=485_3,2=485_4,3=232</param>
        /// <param name="EpitopeNo">表位编号0xFF为广播</param>
        public void ControlConnrRelay3(int contrnlType, byte EpitopeNo, int ID = 0)
        {
            string[] FrameAry = null;
            byte[] data = new byte[3];
            data[1] = (byte)contrnlType;
            object[] paras = new object[] { 8, EpitopeNo, data, (byte)0x01, FrameAry };
            DeviceControl(DeviceName.误差板, "ContnrlBw", ID, paras);
        }

        /// <summary>
        /// 控制通讯继电器切换2--混合终端  -控制2路485切换
        /// </summary>
        /// <param name="contrnlType">0=485I切换到485_2通讯，1=485_3,2=485_4,3=232</param>
        /// <param name="EpitopeNo">表位编号0xFF为广播</param>
        /// <param name="IsCan">是否切换到Can通讯口</param>
        /// <param name="ID"></param>
        public void ControlConnrRelayCan(int contrnlType, byte EpitopeNo, int IsCan, int ID = 0)
        {
            string[] FrameAry = null;
            byte[] data = new byte[3];
            data[1] = (byte)contrnlType;
            data[2] = (byte)IsCan;
            object[] paras = new object[] { 8, EpitopeNo, data, (byte)0x01, FrameAry };
            DeviceControl(DeviceName.误差板, "ContnrlBw", ID, paras);
        }

        /// <summary>
        /// 控制表位电压继电器 
        /// </summary>
        /// <param name="contrnlType">控制类型--1开启-2关闭</param>
        /// <param name="EpitopeNo">表位编号0xFF为广播</param>
        public void ControlMeterPowerRelay(int contrnlType, byte EpitopeNo, int ID = 0)
        {
            string[] FrameAry = null;
            byte[] data = new byte[1];
            for (int i = 0; i < data.Length; i++)
            {
                if (contrnlType == 0)
                    data[i] = 0x00;
                else if (contrnlType == 1)
                    data[i] = 0x01;
                else
                    data[i] = 0x00;
            }
            object[] paras = new object[] { 3, EpitopeNo, data, (byte)0x01, FrameAry };
            DeviceControl(DeviceName.误差板, "ContnrlBw", ID, paras);
        }
        /// <summary>
        /// 设置终端类型
        /// </summary>
        /// <param name="bwNum"></param>
        /// <param name="ZDType">0x01：集中器 I 型 13 版；上电默认态,0x02：集中器 I 型 22 版,0x03：专变 III 型 13 版；0x04：专变 III 型 22 版；0x05：融合终端 SCU；0x06：能源控制器 ECU；</param>
        /// <returns></returns>
        public void SetZDType(byte ZDType, int ID = 0)
        {
            byte btNum = 0xff;
            object[] paras = new object[] { btNum, ZDType};
            DeviceControl(DeviceName.误差板, "SetZDType", ID, paras);
        }

        /// <summary>
        /// 控制门继电器切换
        /// </summary>
        /// <param name="contrnlType">0—继电器断开，Door信号断开；默认值；1—继电器断开，Door信号闭合；</param>
        /// <param name="EpitopeNo">表位编号0xFF为广播</param>
        public void ControlDoorSignalRelayrRelay(int contrnlType, byte EpitopeNo, int ID = 0)
        {
            string[] FrameAry = null;
            byte[] data = new byte[1];
            data[0] = 0x00;
            if (contrnlType == 0)
                data[0] = 0x01;
            object[] paras = new object[] { 5, EpitopeNo, data, (byte)0x01, FrameAry };
            DeviceControl(DeviceName.误差板, "ContnrlBw", ID, paras);
        }

        /// <summary>
        /// 控制遥信状态输出
        /// <param name="EpitopeNo">表位号</param>
        /// <param name="RS1">遥信1 true输出，false不输出</param>
        /// <param name="RS2">遥信2 true输出，false不输出</param>
        /// <param name="RS3">遥信3 true输出，false不输出</param>
        /// <param name="RS4">遥信4 true输出，false不输出</param>
        /// <param name="RS5">遥信5 true输出，false不输出</param>
        /// <param name="RS6">遥信6 true输出，false不输出</param>
        /// <param name="ID"></param>
        public void ContnrRemoteSignalingStatusOutput(byte EpitopeNo, bool RS1, bool RS2, bool RS3, bool RS4, bool RS5, bool RS6, int ID = 0)
        {
            byte data = new byte();
            data = set_bit(data, 1, RS1);
            data = set_bit(data, 2, RS2);
            data = set_bit(data, 3, RS3);
            data = set_bit(data, 4, RS4);
            data = set_bit(data, 5, RS5);
            data = set_bit(data, 6, RS6);
            data = set_bit(data, 7, false);
            data = set_bit(data, 8, false);

            object[] paras = new object[] { EpitopeNo, data };
            DeviceControl(DeviceName.误差板, "ContnrRemoteSignalingStatusOutput", ID, paras);

        }
        /// <summary>
        /// 读取遥控信息
        /// </summary>
        /// <param name="EpitopeNo">表位号 FF广播</param>
        /// <param name="OutTriggerMode">触发方式0电平1脉冲 告警 -轮次 1 -轮次2-轮次 3 -轮次4  </param>
        /// <param name="OutPutValue">输出值 0没有1输出 告警 -轮次 1 -轮次2-轮次 3 -轮次4</param>
        /// <param name="FrameAry"></param>
        public void ReadRemoteControl(byte EpitopeNo, out int[] OutTriggerMode, out int[] OutPutValue, int ID = 0)
        {
            OutTriggerMode = new int[0];
            OutPutValue = new int[0];
            object[] paras = new object[] { EpitopeNo, OutTriggerMode, OutPutValue };
            DeviceControl(DeviceName.误差板, "ReadRemoteControl", ID, paras);
            OutTriggerMode = (int[])paras[1];
            OutPutValue = (int[])paras[2];
        }
        /// <summary>
        /// 设置某一位的值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index">要设置的位， 值从低到高为 1-8</param>
        /// <param name="flag">要设置的值 true / false</param>
        /// <returns></returns>
        private byte set_bit(byte data, int index, bool flag)
        {
            if (index > 8 || index < 1)
                throw new ArgumentOutOfRangeException();
            int v = index < 2 ? index : (2 << (index - 2));
            return flag ? (byte)(data | v) : (byte)(data & ~v);
        }


        /// <summary>
        /// 设置标准常数
        /// </summary>
        /// <param name="ControlType">控制类型（00-标准电能误差相关； 01-标准时钟日记时需量）</param>
        /// <param name="constant">常数</param>
        /// <param name="magnification">放大倍数-2就是缩小100倍</param>
        /// <param name="EpitopeNo">表位编号</param>
        public bool SetStandardConst(int ControlType, int constant, int magnification, byte EpitopeNo,int ID=0)
        {
            string[] FrameAry = null;
            object[] paras = new object[] { ControlType, constant, magnification, EpitopeNo, FrameAry };
            string value = DeviceControl(DeviceName.误差板, "SetStandardConstantQs",ID, paras).ToString();
            if (value == "0")
                return true;
            return false;
        }

        /// <summary>
        /// 设置被检常数
        /// </summary>
        /// <param name="ControlType">控制类型(6组被检:00-有功(含正向、反向，以下同,01-无功(正向、反向，以下同),04-日计时,05-需量）</param>
        /// <param name="constant">常数</param>
        /// <param name="magnification">放大倍数-2就是缩小100倍</param>
        /// <param name="qs">圈数</param>
        /// <param name="EpitopeNo">表位编号</param>
        public bool SetTestedConst(int ControlType, int constant, int magnification,int qs, byte EpitopeNo, int ID = 0)
        {

            //(int enlarge, int Constant, int fads, int qs, byte bwNum, out string[] FrameAry)
            string[] FrameAry = null;
            object[] paras = new object[] { ControlType, constant, magnification, qs, EpitopeNo, FrameAry };
            string value = DeviceControl(DeviceName.误差板, "SetBJConstantQs", ID, paras).ToString();
            if (value == "0")
                return true;
            return false;
        }


        /// <summary>
        /// 启动误差版
        /// </summary>
        /// <param name="ControlType">控制类型</param>
        /// <param name="EpitopeNo">表位号--FF广播</param>
        public bool StartWcb(int ControlType, byte EpitopeNo, int ID = 0)
        {
            string[] FrameAry = null;
            object[] paras = new object[] { ControlType, EpitopeNo, FrameAry };
            string value = DeviceControl(DeviceName.误差板, "Start",ID, paras).ToString();
            if (value == "0")
                return true;
            return false;
        }

        /// <summary>
        /// 停止误差版
        /// </summary>
        /// <param name="ControlType">控制类型</param>
        /// <param name="EpitopeNo">表位号--FF广播</param>
        public bool StopWcb(int ControlType, byte EpitopeNo, int ID = 0)
        {
            string[] FrameAry = null;
            object[] paras = new object[] { ControlType, EpitopeNo, FrameAry };
            string value = DeviceControl(DeviceName.误差板, "Stop",ID, paras).ToString();
            if (value == "0")
                return true;
            return false;
        }

        /// <summary>
        ///   读取误差计算值
        /// </summary>
        /// <param name="readType">读取类型(00--正向有功，01--正向无功，02--反向有功，03--反向无功，04--日计时误差</param>
        /// <param name="EpitopeNo">表位编号</param>
        /// <param name="OutWcData">误差值</param>
        /// <param name="OutBwNul">表位号</param>
        /// <param name="OutGroup">组别: 1个字节(00--有功，01--无功，04--日计时误差，05--需量，06--有功脉冲计数，07--无功</param>
        /// <param name="OutWcNul">第几次误差</param>
        public void ReadWcbData(int readType, byte EpitopeNo, out string[] OutWcData, out int OutBwNul, out int OutGroup, out int OutWcNul, int ID = 0)
        {
            string[] FrameAry = null;
            OutWcData = new string[1];
            OutBwNul = 0;
            OutGroup = 0;
            OutWcNul = 0;
            object[] paras = new object[] { readType, EpitopeNo,   OutWcData, OutBwNul, OutGroup, OutWcNul, FrameAry };
            DeviceControl(DeviceName.误差板, "ReadData", ID,paras);
            OutWcData = (string[])paras[2];
            if (OutWcData.Length<=0)
            {
                OutWcData = new string[1] { "0" };
            }
            OutBwNul = (int)paras[3];
            OutGroup = (int)paras[4];
            OutWcNul = (int)paras[5];

        }

        /// <summary>
        /// 电机控制
        /// </summary>
        /// <param name="ControlType">控制类型01压入表  00取出表</param>
        /// <param name="EpitopeNo">表位号--FF广播</param>
        public bool ElectricmachineryContrnl(int ControlType, byte EpitopeNo, int ID = 0)
        {
            string[] FrameAry = null;
            object[] paras = new object[] { ControlType, EpitopeNo, FrameAry };
            string value = DeviceControl(DeviceName.误差板, "ElectricmachineryContrnl", ID, paras).ToString();
            if (value == "0")
                return true;
            return false;
        }

        /// <summary>
        /// 读取表位电压短路和电流开路标志 
        /// </summary>
        /// <param name="reg">命令03读取表位电压短路和电流开路标志</param>
        /// <param name="bwNum">表位</param>
        /// <param name="OutResult">返回状态0:电压短路标志，00表示没短路，01表示短路；DATA1-电流开路标志，1:00表示没开路，01表示开路。</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public bool Read_Fault(byte reg, byte bwNum, out byte[] OutResult, int ID = 0)
        {
            string[] FrameAry = null;
            OutResult = new byte[7];
            object[] paras = new object[] { reg, bwNum, OutResult, FrameAry };
            string value = DeviceControl(DeviceName.误差板 , "Read_Fault", ID,paras).ToString();
            OutResult = (byte[])paras[2];
            if (value == "0")
                return true;
            return false;

        }


        #endregion

        #region 载波模块



        /// <summary>
        ///   多功能板控制载波切换通道
        /// </summary>
        /// <param name="ControlboardID">载波通道号(FF广播)</param>
        /// <param name="controlboardType">01=闭合，00=断开</param>
        public void CarrierModuleControl(int ControlboardID, int controlboardType,int ID=0)
        {
            string[] FrameAry = null;
            object[] paras = new object[] { ControlboardID, controlboardType, FrameAry };
            DeviceControl(DeviceName.载波, "SetZH3501Controlboard",ID, paras);
        }

        public void SetZBGZDYContrnl(int bwIndex, float u, bool[] bwselect, int ID = 0)
        {
            string[] FrameAry = null;
            object[] paras = new object[] { bwIndex, u, bwselect,FrameAry };
            DeviceControl(DeviceName.载波信号控制板, "SetZBGZDYContrnl", ID, paras);
        }
        /// <summary>
        /// 载波检定台载波信号的链路切换
        /// </summary>
        /// <param name="bwIndex">控制板ID号</param>
        /// <param name="path1">路径一  00-断开  01-闭合</param>
        /// <param name="path2">路径二  00-断开  01-闭合</param>
        /// <param name="path3">路径三  00-断开  01-闭合</param>
        /// <param name="path4">路径四  00-断开  01-闭合</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>

        public bool SetZBSJContrnl5(int bwIndex, int path1, int path2, int path3, int path4 ,int ID = 0)
        {
            string[] FrameAry = null;
            object[] paras = new object[] { bwIndex, path1, path2, path3, path4, FrameAry };
            int t = (int)DeviceControl(DeviceName.载波信号控制板, "SetZBSJContrnl", ID,paras);
            if (t == 0) return true;
            return false;
        }
        #endregion

        #region 偶次谐波

        /// <summary>
        ///  偶次谐波设置
        /// </summary>
        /// <param name="HarmonicA">A相偶次谐波 置1为启动偶次谐波发生，置0为停止偶次谐波发生</param>
        /// <param name="HarmonicB">B相偶次谐波 置1为启动偶次谐波发生，置0为停止偶次谐波发生</param>
        /// <param name="HarmonicC">C相偶次谐波 置1为启动偶次谐波发生，置0为停止偶次谐波发生</param>
        public void SetEvenHarmonic(string HarmonicA, string HarmonicB, string HarmonicC,int ID=0)
        {
            string[] FrameAry = null;
            object[] paras = new object[] { HarmonicA, HarmonicB, HarmonicC, FrameAry };
            DeviceControl(DeviceName.偶次谐波 , "SetZH1106Controlboard", ID,paras);
        }


        /// <summary>
        ///  读取 偶次谐波电流采样值
        /// </summary>
        /// <param name="floatCurrent"> A相正半波电流值 + A相负半波电流值 + B相正半波电流值 + B相负半波电流值 + C相正半波电流值 + C相负半波电流值</param>
        public void GetEvenHarmonicValue(out float[] floatCurrent, int ID = 0)
        {
            string[] FrameAry = null;
            floatCurrent = new float[0];
            object[] paras = new object[] { floatCurrent, FrameAry };
            DeviceControl(DeviceName.偶次谐波, "ReadZH1106Data",ID, paras);
        }

        #endregion

        #region 雷电标准表

     
        /// <summary>
        ///  设置接线方式 
        /// </summary>
        /// <param name="MetricIndex"></param>
        public void SetPuase(string BncCode, string MetricIndex, string aseCode)
        {
            object[] paras = new object[] { BncCode, MetricIndex, aseCode };
            DeviceControl(DeviceName.标准表, "setPuase",0, paras);
        }
        /// <summary>
        ///  设置常数
        /// </summary>
        /// <param name="MetricIndex"></param>
        /// <param name="Constant"></param>
        /// 
        public void SetConstant(string MetricIndex, float Constant)
        {
            object[] paras = new object[] { MetricIndex,Constant };
            DeviceControl(DeviceName.标准表, "setConstant",0, paras);
        }

        /// <summary>
        ///   读取标准表瞬时值
        /// </summary>
        /// 
        public float[] ReadStMeterInfo()
        {
            object[] paras = null;
            object obj = DeviceControl(DeviceName.标准表, "ReadStMeterInfo",0, paras);
            float[] value = (float[])obj;

          //StandarMeterInfo info= (StandarMeterInfo)obj;
            return value;
        }

        /// <summary>
        /// 刷新标准表数据
        /// </summary>
        public void RefStdLd()
        {
            int value = ConfigHelper.Instance.Std_RedInterval;
            while (true)
            {
                if (EquipmentData.ApplicationIsOver == true) break;
                Thread.Sleep(value); //标准表读取间隔
                try
                {
                    //return;
                    float[] floatArray = ReadStMeterInfo();
                    if (floatArray != null)
                    {
                        EquipmentData.StdInfo.Ua = floatArray[0];
                        EquipmentData.StdInfo.Ub = floatArray[1];
                        EquipmentData.StdInfo.Uc = floatArray[2];
                        EquipmentData.StdInfo.Ia = floatArray[3];
                        EquipmentData.StdInfo.Ib = floatArray[4];
                        EquipmentData.StdInfo.Ic = floatArray[5];

                        EquipmentData.StdInfo.PhaseUa = floatArray[17];
                        EquipmentData.StdInfo.PhaseUb = floatArray[18];
                        EquipmentData.StdInfo.PhaseUc = floatArray[19];

                        EquipmentData.StdInfo.PhaseIa = floatArray[20];
                        EquipmentData.StdInfo.PhaseIb = floatArray[21];
                        EquipmentData.StdInfo.PhaseIc = floatArray[22];

                        //EquipmentData.StdInfo.PhaseA = floatArray.;
                        //EquipmentData.StdInfo.PhaseB = floatArray[13];
                        //EquipmentData.StdInfo.PhaseC = floatArray[14];

                        EquipmentData.StdInfo.Pa = floatArray[6];
                        EquipmentData.StdInfo.Pb = floatArray[7];
                        EquipmentData.StdInfo.Pc = floatArray[8];

                        EquipmentData.StdInfo.Qa = floatArray[9];
                        EquipmentData.StdInfo.Qb = floatArray[10];
                        EquipmentData.StdInfo.Qc = floatArray[11];

                        EquipmentData.StdInfo.Sa = floatArray[12];
                        EquipmentData.StdInfo.Sb = floatArray[13];
                        EquipmentData.StdInfo.Sc = floatArray[14];


                        EquipmentData.StdInfo.Freq = floatArray[15];
                        //EquipmentData.StdInfo.P = floatArray.P;
                        //EquipmentData.StdInfo.Q = floatArray.Q;
                        //EquipmentData.StdInfo.S = floatArray.S;
                    }
                }
                catch (Exception )
                {}
                if (EquipmentData.ApplicationIsOver == true) break;
            }


        }

        //ReadStMeterInfo


        //StandarMeterInfo
        #endregion

        #region 互感器电机控制
        /// <summary>
        ///  表位互感器探针电机控制
        /// </summary>
        /// <param name="ControlboardID">0切换到互感器，1切换到直接</param>
        public void Hgq_Set(int controlId,int ID=0)
        {
            object[] paras = new object[] { };
            if (controlId == 0)
            {
                DeviceControl(DeviceName.互感器电机 , "Set_HGQ",ID, paras);
            }
            else
            { 
                DeviceControl(DeviceName.互感器电机, "Set_ZJ", ID, paras);
            }
            Hgq_Off();//过60秒之后关闭电机
        }

        bool Hgq_Is_Off = false;
        /// <summary>
        /// 关闭
        /// </summary>
        public void Hgq_Off(int ID = 0)
        {

            int index = 60;
            if (!Hgq_Is_Off)
            {
                Hgq_Is_Off = true;
                Task task = new Task(() =>
                {
                    while (true)
                    {
                        if (EquipmentData.ApplicationIsOver == true) break;
                        Thread.Sleep(1000);
                        index--;
                        if (index <= 0)
                        {
                            break;
                        }

                        if (EquipmentData.ApplicationIsOver == true) break;
                    }
                    object[] paras = new object[] { };
                    DeviceControl(DeviceName.互感器电机, "Set_Off", ID,paras);
                    Hgq_Is_Off = false;
                });
                task.Start();
            }
            else
            {
                index = 60;
            }



        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Hgq_Off2(int ID = 0)
        {
            object[] paras = new object[] { };
            DeviceControl(DeviceName.互感器电机, "Set_Off", ID , paras);
            Hgq_Is_Off = false;
        }
        #endregion

        #region 多功能板
        /// <summary>
        /// 设置三色灯
        /// </summary>
        /// <param name="iID">灯类型 18红、19黄、20绿</param>
        /// <param name="iType">等于0时灭、1时正常、2时闪烁</param>
        public void SetEquipmentThreeColor(byte iID, byte iType, int ID = 0)
        {
            object[] paras = new object[] { iID, iType };
            DeviceControl(DeviceName.多功能板, "MultiControl", ID, paras);
        }

        /// <summary>
        /// 红
        /// </summary>
        public void Set_Hong()
        {
            SetEquipmentThreeColor((byte)0x10, (byte)0x01);
            SetEquipmentThreeColor((byte)0x11, (byte)0x00);
            SetEquipmentThreeColor((byte)0x12, (byte)0x00);
        }
        /// <summary>
        /// 绿
        /// </summary>
        public void Set_Lv()
        {
            SetEquipmentThreeColor((byte)0x10, (byte)0x00);
            SetEquipmentThreeColor((byte)0x11, (byte)0x01);
            SetEquipmentThreeColor((byte)0x12, (byte)0x00);
        }
        /// <summary>
        ///黄
        /// </summary>
        public void Set_Huang()
        {
            SetEquipmentThreeColor((byte)0x10, (byte)0x00);
            SetEquipmentThreeColor((byte)0x11, (byte)0x00);
            SetEquipmentThreeColor((byte)0x12, (byte)0x01);
        }

        /// <summary>
        ///黄
        /// </summary>
        public void Set_Mie()
        {
            SetEquipmentThreeColor((byte)0x10, (byte)0x00);
            SetEquipmentThreeColor((byte)0x11, (byte)0x00);
            SetEquipmentThreeColor((byte)0x12, (byte)0x00);
        }

        /// <summary>
        /// 大小电流切换版_能源/融合供电
        /// </summary>
        /// <param name="iID">灯类型 18红、19黄、20绿</param>
        /// <param name="iType">等于0时灭、1时正常、2时闪烁</param>
        public void PowerTpye(byte iID, byte iType, int ID = 0)
        {
            object[] paras = new object[] { iID, iType };
            DeviceControl(DeviceName.多功能板, "MultiControl", ID, paras);
        }


        #endregion

        #region 功耗板
        /// <summary>
        ///    控制表位继电器 
        /// </summary>
        /// <param name="contrnlType">控制类型--1开启-2关闭</param>
        /// <param name="EpitopeNo">表位编号0xFF为广播</param>
        public bool Read_GH_Dissipation(int bwIndex, out float[] pd, int ID=0)
        {
            pd = new float[1];
            object[] paras = new object[] { bwIndex,  pd };
            bool t=(bool)DeviceControl(DeviceName .功耗板, "Read_GH_Dissipation", ID, paras);
            pd = (float[])paras[2];
            return t;
        }

        #endregion

        #region 时基源

        /// <summary>
        ///设置时基源输出脉冲
        /// </summary>
        /// <param name="iID">是否切换为时钟脉冲,true切换到时钟脉冲,false切换到电能脉冲</param>
        public void SetTimePulse(bool isTime,int ID=0)
        {
            string[] FrameAry = null;
            object[] paras = new object[] { isTime, FrameAry };
            DeviceControl(DeviceName.时基源 , "SetTimePulse",ID, paras);
        }
        #endregion

        #region 耐压仪及耐压测试板
        /// <summary>
        /// 启动，停止测试
        /// </summary>
        /// <param name="ValueType">01开始，02结束<param>
        /// <returns></returns>
        public bool Ainuo_Start(byte ValueType,int ID=0)
        {
            object[] paras = new object[] { ValueType };
            DeviceControl(DeviceName.耐压仪 , "Start", ID,paras);
            return true;
        }

        /// <summary>
        ///  设置当前测试方式下的参数
        /// </summary>
        /// <param name="U">电压</param>
        /// <param name="UpI">电流上限(毫安)</param>
        /// <param name="DownI">电流下限(毫安)</param>
        /// <param name="Time">测试时间(秒)</param>
        /// <param name="Pl">频率</param>
        /// <param name="UpTime">缓升时间</param>
        /// <param name="DownTime">缓降时间</param>
        /// <returns></returns>
        public bool Ainuo_SetModelValue(float U, float UpI, float DownI, int Time, int Pl, int UpTime, int DownTime, int ID = 0)
        {
            object[] paras = new object[] { U, UpI, DownI, Time, Pl, UpTime , DownTime };
            DeviceControl(DeviceName.耐压仪, "SetModelValue", ID, paras);
            return true;
        }
        #endregion

        #region 零线电流板
        //add yjt 20230103 新增零线电流控制启停
        /// <summary>
        /// 启停零线电流板
        /// </summary>
        /// <param name="StartZeroCurrent"></param>
        /// <returns></returns>
        public bool StartZeroCurrent(int A_kz, int BC_kz)
        {
            object[] paras = new object[] { A_kz, BC_kz };
            string value = DeviceControl(DeviceName.零线电流板, "StartZeroCurrent", 0, paras).ToString();
            if (value == "0")
                return true;
            return false;
        }



      
        #endregion

        #endregion
    }

    public class DeviceName
    {
        public const string 误差板 = "误差板";
        public const string 标准表 = "标准表";
        public const string 功率源 = "功率源";
        public const string 时基源 = "时基源";
        public const string 多功能板 = "多功能板";
        public const string 互感器电机 = "互感器电机";
        public const string 载波 = "载波";
        public const string 偶次谐波 = "偶次谐波";
        public const string 功耗板 = "功耗板 ";
        public const string 直流功耗板 = "直流功耗板";
        public const string 载波信号控制板 = "载波信号控制板";
        public const string 耐压仪 = "耐压仪";
        public const string 测试板 = "测试板";
        public const string 零线电流板 = "零线电流板";
        //public const string 测试板 = "测试板";
    }
}
