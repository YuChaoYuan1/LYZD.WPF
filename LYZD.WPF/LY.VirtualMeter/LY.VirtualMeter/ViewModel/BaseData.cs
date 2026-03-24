using LY.VirtualMeter.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LY.VirtualMeter.ViewModel
{
    /// <summary>
    /// 基本数据
    /// </summary>
  public   class BaseData : NotifyPropertyBase
    {

        private Meter selectMeter ;
        /// <summary>
        /// 选中的虚拟表
        /// </summary>
        public Meter SelectMeter
        {
            get { return selectMeter; }
            set { SetPropertyValue(value, ref selectMeter, "SelectMeter"); }
        }



        private int num_Meter=1;
        /// <summary>
        /// 虚拟表数量
        /// </summary>
        public int Num_Meter
        {
            get { return num_Meter; }
            set { SetPropertyValue(value, ref num_Meter, "Num_Meter"); }
        }
        private ObservableCollection<Meter> meterObject =new ObservableCollection<Meter>();
        /// <summary>
        /// 虚拟表对象
        /// </summary>
        public ObservableCollection<Meter> MeterObject
        {
            get { return meterObject; }
            set { SetPropertyValue(value, ref meterObject, "MeterObject"); }
        }
        

        private ObservableCollection<Server> terminalObject = new ObservableCollection<Server>();
        /// <summary>
        /// 终端对象
        /// </summary>
        public ObservableCollection<Server> TerminalObject
        {
            get { return terminalObject; }
            set { SetPropertyValue(value, ref terminalObject, "TerminalObject"); }
        }

        private int num_Terminal=1;
        /// <summary>
        /// 终端数量
        /// </summary>
        public int Num_Terminal
        {
            get { return num_Terminal; }
            set { SetPropertyValue(value, ref num_Terminal, "Num_Terminal"); }
        }

        private string  connType = "串口";
        /// <summary>
        /// 通讯方式
        /// </summary>
        public string ConnType
        {
            get { return connType; }
            set { SetPropertyValue(value, ref connType, "ConnType"); }
        }

        private List<string> connTypeList = new List<string>();
        /// <summary>
        /// 终端数量
        /// </summary>
        public List<string> ConnTypeList
        {
            get { return connTypeList; }
            set { SetPropertyValue(value, ref connTypeList, "ConnTypeList"); }
        }
        private List<string> protocolType = new List<string>();
        /// <summary>
        /// 协议类型
        /// </summary>
        public List<string> ProtocolType
        {
            get { return protocolType; }
            set { SetPropertyValue(value, ref protocolType, "ProtocolType"); }
        }
        private int selectProtocol = 2;
        /// <summary>
        /// 当前协议
        /// </summary>
        public int SelectProtocol
        {
            get { return selectProtocol; }
            set { SetPropertyValue(value, ref selectProtocol, "SelectProtocol");
                for (int intInc = 0; intInc < AppData.BaseData.MeterObject.Count; intInc++)
                    AppData.BaseData.MeterObject[intInc].BiaoType =SelectProtocol;
            }
        }

        private ObservableCollection<MeterPortData> meter_PortData = new ObservableCollection<MeterPortData>();
        /// <summary>
        /// 表位端口数据
        /// </summary>
        public ObservableCollection<MeterPortData> Meter_PortData
        {
            get { return meter_PortData; }
            set { SetPropertyValue(value, ref meter_PortData, "Meter_PortData"); }
        }

        private ObservableCollection<SetViewModel> _设置 = new ObservableCollection<SetViewModel>();
        /// <summary>
        /// 虚拟表对象
        /// </summary>
        public ObservableCollection<SetViewModel> 设置
        {
            get { return _设置; }
            set { SetPropertyValue(value, ref _设置, "设置"); }
        }

        private ObservableCollection<MeterDLViewModel> _电量信息 = new ObservableCollection<MeterDLViewModel>();
        /// <summary>
        /// 虚拟表对象
        /// </summary>
        public ObservableCollection<MeterDLViewModel> 电量信息
        {
            get { return _电量信息; }
            set { SetPropertyValue(value, ref _电量信息, "电量信息"); }
        }

        public void 获取电量()
        {
            for (int i = 0; i< Num_Meter; i++)
            {
               MeterObject[i].当前显示时间 =MeterObject[i].DateBron().ToString();
               MeterObject[i].当前显示电量 =MeterObject[i].Pq[0, 0].ToString(); ;

            }



            if (AppData.BaseData.SelectMeter == null)
            {
                return;
            }
            for (int i = 0; i <= 8; i++)
            {
               电量信息[i].总需量 = SelectMeter.Zdxl[0, 0];
               电量信息[i].需量1 =SelectMeter.Zdxl[0, 1];
               电量信息[i].需量2 =SelectMeter.Zdxl[0, 2];
               电量信息[i].需量3 =SelectMeter.Zdxl[0, 3];
               电量信息[i].需量4 =SelectMeter.Zdxl[0, 4];
               电量信息[i].总时间 =SelectMeter.ZdxlTime[0, 0].ToString();
               电量信息[i].时间1 = SelectMeter.ZdxlTime[0, 1].ToString();
               电量信息[i].时间2 = SelectMeter.ZdxlTime[0, 2].ToString();
               电量信息[i].时间3 = SelectMeter.ZdxlTime[0, 3].ToString();
               电量信息[i].时间4 = SelectMeter.ZdxlTime[0, 4].ToString();
               电量信息[i].总电量 = SelectMeter.Pq[0, 0];
               电量信息[i].电量1 = SelectMeter.Pq[0, 1];
               电量信息[i].电量2 = SelectMeter.Pq[0, 2];
               电量信息[i].电量3 = SelectMeter.Pq[0, 3];
               电量信息[i].电量4 = SelectMeter.Pq[0, 4];
            }

            SelectMeter.P = SelectMeter.Pa + SelectMeter.Pb + SelectMeter.Pc;
            SelectMeter.Q = SelectMeter.Qa + SelectMeter.Qb + SelectMeter.Qc;
            SelectMeter.S = SelectMeter.Sa + SelectMeter.Sb + SelectMeter.Sc;


        }

        public bool isLog = false;
        /// <summary>
        /// 是否保存日志
        /// </summary>
        public bool IsLog
        {
            get { return isLog; }
            set { SetPropertyValue(value, ref isLog, "IsLog"); }
        }
        public bool isAnalysis = false;
        /// <summary>
        /// 是否解析报文
        /// </summary>
        public bool IsAnalysis
        {
            get { return isAnalysis; }
            set { SetPropertyValue(value, ref isAnalysis, "IsAnalysis"); }
        }
    }

    public class MeterPortData : NotifyPropertyBase
    {
        private int meterNo = 1;
        /// <summary>
        /// 表位号
        /// </summary>
        public int MeterNo
        {
            get { return meterNo; }
            set { SetPropertyValue(value, ref meterNo, "MeterNo"); }
        }

        private int portNo = 1;
        /// <summary>
        /// 端口号
        /// </summary>
        public int PortNo
        {
            get { return portNo; }
            set { SetPropertyValue(value, ref portNo, "PortNo"); }
        }
        private string server_Ip ="192.168.100.100";
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string Server_Ip
        {
            get { return server_Ip; }
            set { SetPropertyValue(value, ref server_Ip, "Server_Ip"); }
        }
        private int server_TelePort = 1;
        /// <summary>
        /// 远程端口
        /// </summary>
        public int Server_TelePort
        {
            get { return server_TelePort; }
            set { SetPropertyValue(value, ref server_TelePort, "Server_TelePort"); }
        }
        private int server_LocalPort = 1;
        /// <summary>
        /// 本地端口
        /// </summary>
        public int Server_LocalPort
        {
            get { return server_LocalPort; }
            set { SetPropertyValue(value, ref server_LocalPort, "Server_LocalPort"); }
        }
        private int rate = 1;
        /// <summary>
        /// 速率
        /// </summary>
        public int Rate
        {
            get { return rate; }
            set { SetPropertyValue(value, ref rate, "Rate"); }
        }
        private int deviceID = 1;
        /// <summary>
        /// 设备索引
        /// </summary>
        public int DeviceID
        {
            get { return deviceID; }
            set { SetPropertyValue(value, ref deviceID, "DeviceID"); }
        }
        private int channeNo = 1;
        /// <summary>
        /// 通道号
        /// </summary>
        public int ChanneNo
        {
            get { return channeNo; }
            set { SetPropertyValue(value, ref channeNo, "ChanneNo"); }
        }

    }
}
