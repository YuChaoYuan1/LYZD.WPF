using LYZD.DAL.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.ViewModel.CheckController
{
    public  class VerifyConfig
    {
        //ConfigHelper.Instance.EquipmentNo;
        #region 台体信息
        /// <summary>
        ///TCP的IP地址
        /// </summary>
        public static string Tcp_Ip = ConfigHelper.Instance.Tcp_Ip;
        /// <summary>
        ///TCP的端口
        /// </summary>
        public static string Tcp_Port = ConfigHelper.Instance.Tcp_Port;

        /// <summary>
        ///台体编号
        /// </summary>
        public static string SetControl_BenthNo = ConfigHelper.Instance.EquipmentNo;
        #endregion

        #region 地区信息
        /// <summary>
        ///地区名称
        /// </summary>
        public static string AreaName = ConfigHelper.Instance.AreaName;
        /// <summary>
        /// 误差限比例
        /// </summary>
        public static string ErrorRatio = ConfigHelper.Instance.ErrorRatio;
        #endregion

        #region 检定信息
        /// <summary>
        /// 常数模式--是：固定常数--否：自动常数
        /// </summary>
        public static  bool ConstModel= ConfigHelper.Instance.ConstModel;
        /// <summary>
        /// 挡位模式--是：自动挡位--否：手动挡位---建议小电流0.01A一下用手动档，其他用自动档
        /// </summary>
        public static bool GearModel = ConfigHelper.Instance.GearModel;
          /// <summary>
          /// 标准表固定常数
          /// </summary>
        public static int StdConst = ConfigHelper.Instance.Std_Const;
        /// <summary>
        ///功率源稳定时间
        /// </summary>
        public static int WaitTime_PowerOn = ConfigHelper.Instance.WaitTime_PowerOn;
        /// <summary>
        ///复位等待时间
        /// </summary>
        public static int WaitTime_Reset = ConfigHelper.Instance.WaitTime_Reset;
        /// <summary>
        ///抄表等待时间
        /// </summary>
        public static int WaitTime_CopyMeter = ConfigHelper.Instance.WaitTime_CopyMeter;
        /// <summary>
        ///停电事件发生等待时间
        /// </summary>
        public static int WaitTime_StopPowerEvent = ConfigHelper.Instance.WaitTime_StopPowerEvent;
        /// <summary>
        /// 检定有效期
        /// </summary>
        public static string TestEffectiveTime = ConfigHelper.Instance.EquipmentNo;
        /// <summary>
        /// 温度
        /// </summary>
        public static float Temperature = ConfigHelper.Instance.Temperature;
        /// <summary>
        /// 湿度
        /// </summary>
        public static float Humidity = ConfigHelper.Instance.Humidity;

        #region 检定配置
        /// <summary>
        /// 误差计算取值数
        /// </summary>
        public static int ErrorCount = ConfigHelper.Instance.ErrorCount;
        /// <summary>
        /// 最大处理时间
        /// </summary>
        public static int MaxHandleTime = ConfigHelper.Instance.MaxHandleTime;
        /// <summary>
        /// 误差个数最大数
        /// </summary>
        public static int ErrorMax = ConfigHelper.Instance.ErrorMax;
        /// <summary>
        ///平均值小数位数
        /// </summary>
        public static int PjzDigit = ConfigHelper.Instance.PjzDigit;
        /// <summary>
        /// 误差起始采集次数(这个就是前面几个误差不要)
        /// </summary>
        public static int ErrorStartCount = ConfigHelper.Instance.ErrorStartCount;
        /// <summary>
        /// 跳差判断倍数
        /// </summary>
        public static float JumpJudgment = ConfigHelper.Instance.JumpJudgment;
        /// <summary>
        /// 偏差计算取值数
        /// </summary>
        public static int PcCount = ConfigHelper.Instance.PcCount;
        /// <summary>
        /// 是否使用时间来计算误差圈数
        /// </summary>
        public static bool  IsTimeWcLapCount = ConfigHelper.Instance.IsTimeWcLapCount;
        /// <summary>
        /// 出一个误差最小时间
        /// </summary>
        public static float  WcMinTime = ConfigHelper.Instance.WcMinTime;
        /// <summary>
        /// 表位压接等待时间
        /// </summary>
        public static int Mete_Press_Time=ConfigHelper.Instance.Mete_Press_Time;
        /// <summary>
        /// 不合格率报警(百分比)
        /// </summary>
        public static int FailureRate = ConfigHelper.Instance.FailureRate;
        /// <summary>
        /// 隔离不合格表位
        /// </summary>
        public static bool IsPartition_Meter = ConfigHelper.Instance.IsPartition_Meter;
        /// <summary>
        /// 终端地址使用十六进制
        /// </summary>
        public static bool IsHexAddress = ConfigHelper.Instance.IsHexAddress;
        #endregion

        #region 终端参数配置

        /// <summary>
        /// 终端地址长度-单位字节
        /// </summary>
        public static int AddressLen = ConfigHelper.Instance.AddressLen;
        #endregion

        #endregion

        #region 营销接口

        /// <summary>
        /// 营销接口类型
        /// </summary>
        public static string Marketing_Type = ConfigHelper.Instance.Marketing_Type;

        /// <summary>
        /// 营销下载标识--条形码  出厂编号 表位号
        /// </summary>
        public static string Marketing_DewnLoadNumber =  ConfigHelper.Instance.Marketing_DewnLoadNumber;

        /// <summary>
        /// 营销系统IP地址
        /// </summary>
        public static string Marketing_IP  = ConfigHelper.Instance.Marketing_IP;

        /// <summary>
        /// 营销系统端口号
        /// </summary>
        public static string Marketing_Prot = ConfigHelper.Instance.Marketing_Prot;

        /// <summary>
        /// 营销系统数据源--就是表名吧应该
        /// </summary>
        public static string Marketing_DataSource       = ConfigHelper.Instance.Marketing_DataSource;

        /// <summary>
        /// 营销——数据库用户名
        /// </summary>
        public static string Marketing_UserName          = ConfigHelper.Instance.Marketing_UserName;

        /// <summary>
        ///营销——数据库密码
        /// </summary>
        public static string Marketing_UserPassWord   = ConfigHelper.Instance.Marketing_UserPassWord;
        /// <summary>
        /// WebService路径
        /// </summary>
        public static string Marketing_WebService       = ConfigHelper.Instance.Marketing_WebService;

        /// <summary>
        /// 上传时时数据
        /// </summary>
        public static bool Marketing_UpData         = ConfigHelper.Instance.Marketing_UpData;

        #endregion

        #region 加密机
        /// <summary>
        /// 加密机类型
        /// </summary>
        public static string Dog_Type   = ConfigHelper.Instance.Dog_Type;
        /// <summary>
        /// 加密机IP
        /// </summary>
        public static string Dog_IP = ConfigHelper.Instance.Dog_IP;
        /// <summary>
        /// 加密机端口
        /// </summary>
        public static string Dog_Prot = ConfigHelper.Instance.Dog_Prot;
        /// <summary>
        /// 加密机密钥
        /// </summary>
        public static string Dog_key = ConfigHelper.Instance.Dog_key;
        /// <summary>
        /// 加密机认证类型--公钥--私钥
        /// </summary>
        public static string Dog_CheckingType = ConfigHelper.Instance.Dog_CheckingType;
        /// <summary>
        /// 加密机-是否进行密码机服务器连接
        /// </summary>
        public static bool Dog_IsPassWord = ConfigHelper.Instance.Dog_IsPassWord;
        /// <summary>
        /// 加密机连接模式---服务器版-直连加密机版
        /// </summary>
        public static string Dog_ConnectType = ConfigHelper.Instance.Dog_ConnectType;
        /// <summary>
        /// 加密机超时时间
        /// </summary>
        public static string Dog_Overtime = ConfigHelper.Instance.Dog_Overtime;
        /// <summary>
        /// 698密钥下装后是否启动密钥开关
        /// </summary>
        public static bool Dog_IsKeySwitch_698 = ConfigHelper.Instance.Dog_IsKeySwitch_698;
        /// <summary>
        ///376密钥下装后是否启动密钥开关
        /// </summary>
        public static bool Dog_IsKeySwitch_376 = ConfigHelper.Instance.Dog_IsKeySwitch_376;
        #endregion

        #region 终端检定参数 --还没有设置-先用
        /// <summary>
        /// 总加组数量
        /// </summary>
        public static byte GroupTotalCount = 1;
        /// <summary>
        /// 总加组1测量点数量
        /// </summary>
        public static byte GroupTotal1Count = 1;
        /// <summary>
        /// 总加组2测量点数量
        /// </summary>
        public static byte GroupTotal2Count = 1;
        /// <summary>
        /// 总加组2测量点
        /// </summary>
        public static string GroupTotal1Pn ="3";
        /// <summary>
        /// 总加组1测量点
        /// </summary>
        public static string GroupTotal2Pn = "2";
        /// <summary>
        /// 脉冲数量
        /// </summary>
        public static byte PulseCount = 2;

        /// <summary>
        /// 遥控数量
        /// </summary>
        public static byte RemoteControlCoutnt = 2;

        
        #endregion
    }
}
