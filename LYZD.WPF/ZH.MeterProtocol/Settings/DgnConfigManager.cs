using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using ZH.MeterProtocol.Comm;
using ZH.MeterProtocol.Struct;

namespace ZH.MeterProtocol.Settings
{
    public class DgnConfigManager : SingletonBase<DgnConfigManager>
    {
        private DgnPortInfo[] dicPortConfigs = new DgnPortInfo[0];
        private readonly string configFilePath = Application.StartupPath + "\\System\\Rs485PortConfig.xml";

        private ComPortInfo[] _Rs485Port1s = new ComPortInfo[0];      //1路485通信串口信息
        private ComPortInfo[] _Rs485Port2s = new ComPortInfo[0];      //2路485通信串口信息
        private ComPortInfo[] _CarrierPorts = new ComPortInfo[0];     //载波通信串口信息
        private ComPortInfo[] _InfraredPorts = new ComPortInfo[0];    //红外通信串口信息



        public int GetChannelCount()
        {
            return _Rs485Port1s.Length;
        }

        public ComPortInfo GetConfig(int index)
        {
            return _Rs485Port1s[index];
        }
        /// <summary>
        /// 获取当前载波端口
        /// </summary>
        /// <returns></returns>
        public ComPortInfo GetCarrierPort(int bwIndex)
        {
            int portNum = 0;
            if (null != App.CarrierInfos && App.CarrierInfos.Length > bwIndex && null != App.CarrierInfos[bwIndex])
            {
                int.TryParse(App.CarrierInfos[bwIndex].Port.Replace("COM", ""), out portNum);
            }
            ComPortInfo port = new ComPortInfo(); ;
            foreach (ComPortInfo item in _CarrierPorts)
            {
                if (item.Port == portNum)
                {
                    port = item;
                    break;
                }
            }
            return port;
        }

        /// <summary>
        /// 获取 第 2 路 485 通讯 端口 
        /// </summary>
        /// <returns></returns>
        public ComPortInfo GetTwo485CpPort(string name)
        {
            int count = 5;//App.CUS.Meters.Count;
            string[] arr = name.Split('_');
            int bwh = Convert.ToInt32(arr[arr.Length - 1]);
            if (bwh <= (count / 2))
            {
                return _Rs485Port2s[0];
            }
            else
            {
                if (_Rs485Port2s.Length > 1)
                    return _Rs485Port2s[1];
                else
                    return _Rs485Port2s[0];
            }
        }

        /// <summary>
        /// 获取红外端口信息
        /// </summary>
        /// <returns></returns>
        public ComPortInfo GetInfaredPort()
        {
            return _InfraredPorts[0];
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <returns></returns>
        public bool Load(ComPortInfo[] comPort)
        {

            _Rs485Port1s = new ComPortInfo[comPort.Length];
            _Rs485Port1s = comPort;

            //【标注协议】
            return true;   //根据之前设置的数据，获得端口的配置

            //_Rs485Port1s

            //#region 加载端口配置
            //DeviceInfor device = ConfigHardware.GetConfig("RS485");
            //if (device != null)
            //{
            //    //485通讯端口及通讯参数
            //    int length = device.Ports.Count;
            //    _Rs485Port1s = new ComPortInfo[length];

            //    for (int i = 0; i < length; i++)
            //    {
            //        PortInfor ptf = device.Ports[i];

            //        ComPortInfo p = new ComPortInfo
            //        {
            //            Port = ptf.Number,
            //            Setting = ptf.Parameter,
            //            LinkType = Common.ParsePortType(ptf.Server),
            //            IsExist = device.IsExist
            //        };

            //        if (ptf.Server.Contains("Can")) //Can口通信
            //        {
            //            CanCannel canTmp = ConfigHelper.GetCanServer(ptf.Server);

            //            string deviceType = Convert.ToUInt32(canTmp.Value1).ToString();
            //            string deviceInd = Convert.ToUInt32(canTmp.SettingBase).ToString();
            //            string canInd = Convert.ToUInt32(canTmp.SettingExpand).ToString();
            //            string CanCmtID = ptf.Number.ToString();
            //            p.OtherParams = string.Format("CanKaName_{0}_{1}_{2}_{3}", deviceType, deviceInd, canInd, CanCmtID);
            //        }
            //        else if (ptf.Server != "COM") //2018
            //        {
            //            p.IP = ConfigHelper.GetServer(ptf.Server).Value1;
            //        }

            //        _Rs485Port1s[i] = p;

            //    }
            //}

            //device = ConfigHardware.GetConfig("Two485");
            //if (device != null)
            //{

            //    //485通讯端口及通讯参数
            //    int length = device.Ports.Count;
            //    _Rs485Port2s = new ComPortInfo[length];

            //    for (int i = 0; i < length; i++)
            //    {
            //        PortInfor ptf = device.Ports[i];

            //        ComPortInfo p = new ComPortInfo
            //        {
            //            Port = ptf.Number,
            //            Setting = ptf.Parameter,
            //            LinkType = Common.ParsePortType(ptf.Server),
            //            IsExist = device.IsExist
            //        };

            //        if (ptf.Server.Contains("Can")) //Can通信
            //        {
            //            CanCannel canTmp = ConfigHelper.GetCanServer(ptf.Server);

            //            string deviceType = Convert.ToUInt32(canTmp.Value1).ToString();
            //            string deviceInd = Convert.ToUInt32(canTmp.SettingBase).ToString();
            //            string canInd = Convert.ToUInt32(canTmp.SettingExpand).ToString();
            //            string CanCmtID = ptf.Number.ToString();
            //            p.OtherParams = string.Format("CanKaName_{0}_{1}_{2}_{3}", deviceType, deviceInd, canInd, CanCmtID);
            //        }
            //        else if (ptf.Server != "COM") //2018
            //        {
            //            p.IP = ConfigHelper.GetServer(ptf.Server).Value1;
            //        }

            //        _Rs485Port2s[i] = p;

            //    }
            //}

            //device = ConfigHardware.GetConfig("Carrier");
            //if (device != null)
            //{

            //    //载波通讯端口及通讯参数
            //    int length = device.Ports.Count;
            //    _CarrierPorts = new ComPortInfo[length];

            //    for (int i = 0; i < length; i++)
            //    {
            //        PortInfor ptf = device.Ports[i];
            //        ComPortInfo p = new ComPortInfo
            //        {
            //            Port = ptf.Number,
            //            Setting = ptf.Parameter,
            //            LinkType = Common.ParsePortType(ptf.Server),
            //            IsExist = device.IsExist
            //        };

            //        if (ptf.Server != "COM") //2018
            //        {
            //            p.IP = ConfigHelper.GetServer(ptf.Server).Value1;
            //        }

            //        _CarrierPorts[i] = p;

            //    }
            //}

            //device = ConfigHardware.GetConfig("Infrared");//红外设备通讯端口及通讯参数
            //if (device != null)
            //{
            //    int length = device.Ports.Count;
            //    _InfraredPorts = new ComPortInfo[length];

            //    for (int i = 0; i < length; i++)
            //    {
            //        PortInfor ptf = device.Ports[i];
            //        ComPortInfo p = new ComPortInfo
            //        {
            //            Port = ptf.Number,
            //            Setting = ptf.Parameter,
            //            LinkType = Common.ParsePortType(ptf.Server),
            //            IsExist = device.IsExist
            //        };

            //        if (ptf.Server != "COM") //2018
            //        {
            //            p.IP = ConfigHelper.GetServer(ptf.Server).Value1;
            //        }

            //        _InfraredPorts[i] = p;

            //    }
            //}

            //return true;

            //#endregion                     
        }




        #region 暂时没有用到
        ///// <summary>
        ///// 保存配置
        ///// </summary>
        ///// <returns></returns>
        //public bool Save()
        //{
        //    using (FileStream fs = new FileStream(configFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        //    {
        //        XmlSerializer serializer = new XmlSerializer(dicPortConfigs.GetType());
        //        serializer.Serialize(fs, dicPortConfigs);
        //        return true;
        //    }
        //}

        /// <summary>
        /// 设置通道数
        /// <paramref name="channelCount">通道数</paramref>
        /// </summary>
        public void SetChannelCount(int channelCount, int baseCom, string IP, string remotePort, int LocalBasePort)
        {
            dicPortConfigs = new DgnPortInfo[channelCount];
            for (int i = 0; i < dicPortConfigs.Length; i++)
            {
                dicPortConfigs[i] = new DgnPortInfo()
                {
                    PortNumber = baseCom + i,
                    PortType = PortType.ZHDevices1,
                    Setting = IP + "|" + remotePort + "|" + LocalBasePort,

                };
            }
        }
        #endregion

    }
}
