using LYZD.Utility.Log;
using System;
using System.Collections.Generic;

namespace LYZD.DAL.Config
{
    /// <summary>
    /// 配置信息管理
    /// </summary>
    public class ConfigHelper
    {
        private static ConfigHelper instance = null;

        public static ConfigHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConfigHelper();
                }
                return instance;
            }
        }

        /// 配置信息列表
        /// <summary>
        /// 配置信息列表
        /// </summary>
        private Dictionary<EnumConfigId, List<string>> configDictionary = new Dictionary<EnumConfigId, List<string>>();


        /// 从数据库加载所有配置信息
        /// <summary>
        /// 从数据库加载所有配置信息
        /// </summary>
        public void LoadAllConfig()
        {
            configDictionary.Clear();
            List<DynamicModel> models = DALManager.ApplicationDbDal.GetList(EnumAppDbTable.T_CONFIG_PARA_VALUE.ToString());
            for (int i = 0; i < models.Count; i++)
            {
                string configNo = models[i].GetProperty("CONFIG_NO") as string;
                string stringValue = models[i].GetProperty("CONFIG_VALUE") as string;
                if (configNo != null)
                {
                    configNo = configNo.TrimStart('0');
                    EnumConfigId configId = EnumConfigId.未知编号配置;
                    Enum.TryParse(configNo, out configId);
                    if (configDictionary.ContainsKey(configId))
                    {
                        configDictionary[configId].Add(stringValue);
                    }
                    else
                    {
                        configDictionary.Add(configId, new List<string> { stringValue });
                    }
                }
            }
            List<DynamicModel> modelsFormat = DALManager.ApplicationDbDal.GetList(EnumAppDbTable.T_CONFIG_PARA_FORMAT.ToString());
            for (int i = 0; i < modelsFormat.Count; i++)
            {
                string configNo = modelsFormat[i].GetProperty("CONFIG_NO") as string;
                string defaultValue = modelsFormat[i].GetProperty("CONFIG_DEFAULT_VALUE") as string;
                if (configNo != null)
                {
                    configNo = configNo.TrimStart('0');
                    EnumConfigId configId = EnumConfigId.未知编号配置;
                    Enum.TryParse(configNo, out configId);
                    if (!dictionaryFormat.ContainsKey(configId))
                    {
                        dictionaryFormat.Add(configId, defaultValue);
                    }
                }
            }
        }

        public List<string> GetConfig(EnumConfigId configId)
        {
            if (configDictionary.ContainsKey(configId))
            {
                return configDictionary[configId];
            }
            else
            {
                return new List<string>();
            }
        }

        public string GetConfigString(EnumConfigId configId)
        {
            if (configDictionary.ContainsKey(configId))
            {
                List<string> valueList = configDictionary[configId];
                if (valueList.Count > 0)
                {
                    return valueList[0];
                }
            }
            LogManager.AddMessage(string.Format("配置信息:{0}获取失败!", configId), EnumLogSource.用户操作日志, EnumLevel.Error);
            return "";
        }

        #region 获取值
        /// <summary>
        /// 获取配置值,如果获取失败,取默认值
        /// </summary>
        /// <param name="configId">配置编号</param>
        /// <param name="indexTemp">值序号</param>
        /// <returns></returns>
        public string GetConfigString(EnumConfigId configId, int indexTemp)
        {
            string stringTemp = GetConfigString(configId);
            try
            {
                return stringTemp.Split('|')[indexTemp];
            }
            catch
            {
                return GetDefaultValue(configId, indexTemp);
            }
        }
        /// <summary>
        /// 获取配置值,如果获取失败,取默认值
        /// </summary>
        /// <param name="configId">配置编号</param>
        /// <param name="indexTemp">值序号</param>
        /// <returns></returns>
        public bool GetConfigBool(EnumConfigId configId, int indexTemp, bool initialValue)
        {
            string stringTemp = GetConfigString(configId);
            try
            {
                string boolString = stringTemp.Split('|')[indexTemp];
                if (boolString == "是")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                string boolString = GetDefaultValue(configId, indexTemp);
                if (boolString == "是")
                {
                    return true;
                }
                else if (boolString == "否")
                {
                    return false;
                }
                else
                {
                    return initialValue;
                }
            }
        }
        /// <summary>
        /// 获取配置值,如果获取失败,取默认值
        /// </summary>
        /// <param name="configId">配置编号</param>
        /// <param name="indexTemp">值序号</param>
        /// <returns></returns>
        public int GetConfigInt(EnumConfigId configId, int indexTemp, int initialValue)
        {
            string stringTemp = GetConfigString(configId);
            try
            {
                string intString = stringTemp.Split('|')[indexTemp];
                return int.Parse(intString);
            }
            catch
            {
                string intString = GetDefaultValue(configId, indexTemp);
                int boolTemp = 0;
                if (!int.TryParse(intString, out boolTemp))
                {
                    return initialValue;
                }
                return boolTemp;
            }
        }
        /// <summary>
        /// 获取配置值,如果获取失败,取默认值
        /// </summary>
        /// <param name="configId">配置编号</param>
        /// <param name="indexTemp">值序号</param>
        /// <returns></returns>
        public float GetConfigFloat(EnumConfigId configId, int indexTemp, float initialValue)
        {
            string stringTemp = GetConfigString(configId);
            try
            {
                string floatString = stringTemp.Split('|')[indexTemp];
                return float.Parse(floatString);
            }
            catch
            {
                string floatString = GetDefaultValue(configId, indexTemp);
                float floatTemp = 0;
                if (!float.TryParse(floatString, out floatTemp))
                {
                    return initialValue;
                }
                return floatTemp;
            }
        }
        #endregion

        private void SaveConfigValue(EnumConfigId configId, string configValue)
        {
            string sql = string.Format("update {0} set config_value = '{1}' where config_no='{2}'", EnumAppDbTable.T_CONFIG_PARA_VALUE.ToString(), configValue, ((int)configId).ToString().PadLeft(5, '0'));
            DALManager.ApplicationDbDal.ExecuteOperation(new List<string> { sql });
        }
        /// <summary>
        /// 保存配置值
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="indexTemp"></param>
        /// <param name="objTemp"></param>
        private void SaveConfigValue(EnumConfigId configId, int indexTemp, object objTemp)
        {
            string temp = GetConfigString(configId);
            string[] arrayTemp = temp.Split('|');
            if (arrayTemp.Length > indexTemp)
            {
                string valueTemp = objTemp == null ? "" : objTemp.ToString();
                if (objTemp is bool)
                {
                    if ((bool)objTemp)
                    {
                        valueTemp = "是";
                    }
                    else
                    {
                        valueTemp = "否";
                    }
                }
                arrayTemp[indexTemp] = valueTemp;
                temp = string.Join("|", arrayTemp);
                // SaveConfigValue(EnumConfigId.运行环境, temp);
            }
        }

        #region 获取默认值

        private Dictionary<EnumConfigId, string> dictionaryFormat = new Dictionary<EnumConfigId, string>();


        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="indexTemp"></param>
        /// <returns></returns>
        private string GetDefaultValue(EnumConfigId configId, int indexTemp)
        {
            string strResult = "";
            if (dictionaryFormat.ContainsKey(configId))
            {
                string valueTemp = dictionaryFormat[configId];
                if (valueTemp != null)
                {
                    string[] arrayDefault = valueTemp.Split('|');
                    if (arrayDefault.Length > indexTemp)
                    {
                        return arrayDefault[indexTemp];
                    }
                }
            }
            return strResult;
        }
        #endregion



        #region 装置信息
        public string EquipmentNo    //检定台的编号
        {
            get
            {
                return GetConfigString(EnumConfigId.基本信息, 0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.基本信息, 0, value);
            }
        }
        public string EquipmentType  //单相台还是三相台
        {
            get
            {
                return GetConfigString(EnumConfigId.基本信息, 1);
            }
            set
            {
                SaveConfigValue(EnumConfigId.基本信息, 1, value);
            }
        }
        public int MeterCount   /// 表位的数量
        {
            get
            {
                return GetConfigInt(EnumConfigId.基本信息, 2, 24);
            }
            set
            {
                SaveConfigValue(EnumConfigId.基本信息, 2, value);
            }
        }
        /// <summary>
        /// 检测表类型--终端还是电表
        /// </summary>
        public string MeterType
        {
            get
            {
                return GetConfigString(EnumConfigId.基本信息, 3);
            }
            set
            {
                SaveConfigValue(EnumConfigId.基本信息, 3, value);
            }
        }

        /// <summary>
        /// 检定模式-自动模式--手动模式
        /// </summary>
        public string VerifyModel
        {
            get
            {
                return GetConfigString(EnumConfigId.基本信息, 4);
            }
            set
            {
                SaveConfigValue(EnumConfigId.基本信息, 4, value);
            }
        }

        /// <summary>
        /// 自动登入
        /// </summary>
        public bool AutoLogin
        {
            get
            {
                return GetConfigBool(EnumConfigId.基本信息, 5, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.基本信息, 5, value);
            }
        }
        /// <summary>
        /// 是否弹出错误提示界面
        /// </summary>
        public bool IsShowErrorTips
        {
            get
            {
                return GetConfigBool(EnumConfigId.基本信息, 6, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.基本信息, 6, value);
            }
        }
        /// <summary>
        /// TCP 的IP地址
        /// </summary>
        public string Tcp_Ip
        {
            get
            {
                return GetConfigString(EnumConfigId.基本信息, 7);
            }
            set
            {
                SaveConfigValue(EnumConfigId.基本信息, 7, value);
            }
        }
        /// <summary>
        /// TCP端口
        /// </summary>
        public string Tcp_Port
        {
            get
            {
                return GetConfigString(EnumConfigId.基本信息, 8);
            }
            set
            {
                SaveConfigValue(EnumConfigId.基本信息,8, value);
            }
        }


        /// <summary>
        /// 系统编号
        /// </summary>
        public string SysNO
        {
            get
            {
                return GetConfigString(EnumConfigId.基本信息, 9);
            }
            set
            {
                SaveConfigValue(EnumConfigId.基本信息, 9, value);
            }
        }
        #region 区域信息
        /// <summary>
        ///地区名称
        /// </summary>
        public string AreaName
        {
            get
            {
                return GetConfigString(EnumConfigId.地区信息, 0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.地区信息, 0, value);
            }
        }
        /// <summary>
        /// 误差限比例
        /// </summary>
        public string ErrorRatio
        {
            get
            {
                return GetConfigString(EnumConfigId.地区信息,1);
            }
            set
            {
                SaveConfigValue(EnumConfigId.地区信息, 1, value);
            }
        }
        #endregion

        #region 显示设置
        /// <summary>
        /// 是否显示版本号
        /// </summary>
        public bool IsVersionNumber
        {
            get
            {
                return GetConfigBool(EnumConfigId.显示设置, 0, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.显示设置, 0, value);
            }
        }
        /// <summary>
        /// 是否显示装置编号
        /// </summary>
        public bool IsDeviceNumber
        {
            get
            {
                return GetConfigBool(EnumConfigId.显示设置,1, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.显示设置, 1, value);
            }
        }
        /// <summary>
        /// 开始检定按钮是否悬浮 --是就是悬浮
        /// </summary>
        public bool IsTestButtonSuspension
        {
            get
            {
                return GetConfigBool(EnumConfigId.显示设置, 2, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.显示设置,2, value);
            }
        }
        #endregion

        #region 日志设置
        /// <summary>
        /// 是否打开【流程】日志
        /// </summary>
        public bool IsOpenLog_Process
        {
            get
            {
                return GetConfigBool(EnumConfigId.日志设置, 0, true);
            }
            set
            {
                SaveConfigValue(EnumConfigId.日志设置, 0, value);
            }
        }
        /// <summary>
        /// 是否打开【提示】日志
        /// </summary>
        public bool IsOpenLog_Tips
        {
            get
            {
                return GetConfigBool(EnumConfigId.日志设置, 1, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.日志设置,1, value);
            }
        }
        /// <summary>
        /// 是否打开【详细】日志
        /// </summary>
        public bool IsOpenLog_Detailed
        {
            get
            {
                return GetConfigBool(EnumConfigId.日志设置, 2, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.日志设置, 2, value);
            }
        }
        /// <summary>
        /// 是否打开【表位通讯帧】日志
        /// </summary>
        public bool IsOpenLog_MeterFrame
        {
            get
            {
                return GetConfigBool(EnumConfigId.日志设置, 3, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.日志设置,3, value);
            }
        }
        #endregion

        #endregion

        #region 检定信息
        /// <summary>
        /// 常数模式--是：固定常数--否：自动常数
        /// </summary>
        public bool ConstModel
        {
            get
            {
                return GetConfigBool(EnumConfigId.标准器设置, 0, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.标准器设置, 0, value);
            }
        }

        /// <summary>
        /// 挡位模式--是：自动挡位--否：手动挡位---建议小电流0.01A一下用手动档，其他用自动档
        /// </summary>
        public bool GearModel
        {
            get
            {
                return GetConfigBool(EnumConfigId.标准器设置, 1, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.标准器设置, 1, value);
            }
        }


        /// <summary>
        /// 是否读取标准表数据（是的话联机成功会时时读取标准表数据）
        /// </summary>
        public bool IsReadStd
        {
            get
            {
                return GetConfigBool(EnumConfigId.标准器设置,2, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.标准器设置, 2, value);
            }
        }


        /// <summary>
        ///标准表读取间隔
        /// </summary>
        public int Std_RedInterval
        {
            get
            {
                return GetConfigInt(EnumConfigId.标准器设置, 3,3);
            }
            set
            {
                SaveConfigValue(EnumConfigId.标准器设置, 3, value);
            }
        }

        /// <summary>
        ///标准表固定常数
        /// </summary>
        public int Std_Const
        {
            get
            {
                return GetConfigInt(EnumConfigId.标准器设置, 4, 1000000);
            }
            set
            {
                SaveConfigValue(EnumConfigId.标准器设置, 4, value);
            }
        }


        /// <summary>
        /// 检定有效期
        /// </summary>
        public string TestEffectiveTime
        {
            get
            {
                return GetConfigString(EnumConfigId.检定有效期, 0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定有效期, 0, value);
            }
        }

        /// <summary>
        /// 温度
        /// </summary>
        public float Temperature
        {
            get
            {
                return GetConfigFloat(EnumConfigId.检定有效期, 1, 20);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定有效期, 1, value);
            }
        }
        /// <summary>
        /// 湿度
        /// </summary>
        public float Humidity
        {
            get
            {
                return GetConfigFloat(EnumConfigId.检定有效期, 2, 60);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定有效期, 2, value);
            }
        }

        #region 检定配置


        /// <summary>
        /// 误差计算取值数
        /// </summary>
        public int ErrorCount
        {
            get
            {
                return GetConfigInt(EnumConfigId.检定配置, 0,2);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定配置, 0, value);
            }
        }
        /// <summary>
        /// 最大处理时间
        /// </summary>
        public int MaxHandleTime
        {
            get
            {
                return GetConfigInt(EnumConfigId.检定配置, 1, 300);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定配置, 1, value);
            }
        }
        /// <summary>
        /// 误差个数最大数
        /// </summary>
        public int ErrorMax
        {
            get
            {
                return GetConfigInt(EnumConfigId.检定配置, 2, 5);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定配置, 2, value);
            }
        }
        /// <summary>
        ///平均值小数位数
        /// </summary>
        public int PjzDigit
        {
            get
            {
                return GetConfigInt(EnumConfigId.检定配置, 3,4);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定配置, 3, value);
            }
        }

        /// <summary>
        /// 误差起始采集次数(这个就是前面几个误差不要)
        /// </summary>
        public int ErrorStartCount
        {
            get
            {
                return GetConfigInt(EnumConfigId.检定配置, 4, 0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定配置, 4, value);
            }
        }
        /// <summary>
        /// 跳差判断倍数
        /// </summary>
        public float JumpJudgment
        {
            get
            {
                return GetConfigFloat(EnumConfigId.检定配置, 5, 1f);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定配置, 5, value);
            }
        }
 

        /// <summary>
        /// 偏差计算取值数
        /// </summary>
        public int PcCount
        {
            get
            {
                return GetConfigInt(EnumConfigId.检定配置,6, 5);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定配置, 6, value);
            }
        }
        /// <summary>
        /// 是否使用时间来计算误差圈数
        /// </summary>
        public bool IsTimeWcLapCount
        {
            get
            {
                return GetConfigBool(EnumConfigId.检定配置, 7, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定配置, 7, value);
            }
        }
        /// <summary>
        /// 出一个误差最小时间
        /// </summary>
        public float WcMinTime
        {
            get
            {
                return GetConfigFloat(EnumConfigId.检定配置,8, 5);
            }
            set
            {
                GetConfigFloat(EnumConfigId.检定配置,8, value);
            }
        }
        ////temperature   humidity


        #endregion


        #region 时间设置
        /// <summary>
        ///功率源稳定时间
        /// </summary>
        public int WaitTime_PowerOn
        {
            get
            {
                return GetConfigInt(EnumConfigId.时间设置, 0, 5);
            }
            set
            {
                SaveConfigValue(EnumConfigId.时间设置, 0, value);
            }
        }
        /// <summary>
        ///复位等待时间
        /// </summary>
        public int WaitTime_Reset
        {
            get
            {
                return GetConfigInt(EnumConfigId.时间设置, 1, 90);
            }
            set
            {
                SaveConfigValue(EnumConfigId.时间设置, 1, value);
            }
        }
        /// <summary>
        ///抄表等待时间
        /// </summary>
        public int WaitTime_CopyMeter
        {
            get
            {
                return GetConfigInt(EnumConfigId.时间设置,2, 180);
            }
            set
            {
                SaveConfigValue(EnumConfigId.时间设置,2, value);
            }
        }
        /// <summary>
        ///停电事件发生等待时间
        /// </summary>
        public int WaitTime_StopPowerEvent
        {
            get
            {
                return GetConfigInt(EnumConfigId.时间设置, 3, 1080);
            }
            set
            {
                SaveConfigValue(EnumConfigId.时间设置, 3, value);
            }
        }


        #endregion

        #region 终端参数配置
        /// <summary>
        /// 终端地址长度--单位字节
        /// </summary>
        public int AddressLen
        {
            get
            {
                return GetConfigInt(EnumConfigId.终端参数配置 ,0, 2);
            }
            set
            {
                SaveConfigValue(EnumConfigId.终端参数配置,0, value);
            }
        }

        #endregion

        #region 电机配置
        /// <summary>
        ///  是否自动压接表位(是，检定开始前会压接表位，检定结束会抬起表位)
        /// </summary>
        public bool IsMete_Press
        {
            get
            {
                return GetConfigBool(EnumConfigId.电机配置, 0, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.电机配置, 0, value);
            }
        }
        /// <summary>
        ///电机压接等待时间秒
        /// </summary>
        public int Mete_Press_Time
        {
            get
            {
                return GetConfigInt(EnumConfigId.电机配置, 1, 8);
            }
            set
            {
                SaveConfigValue(EnumConfigId.电机配置, 1, value);
            }
        }
        /// <summary>
        ///是否自动切换互感器
        /// </summary>
        public bool Is_Hgq_AutoCut
        {
            get
            {
                return GetConfigBool(EnumConfigId.电机配置, 2, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.电机配置, 2, value);
            }
        }
        #endregion

        #region 检定过程配置
        /// <summary>
        /// 不合格率报警
        /// </summary>
        public int FailureRate
        {
            get
            {
                return GetConfigInt(EnumConfigId.检定过程配置, 0,0);
            }
            set
            {
                GetConfigInt(EnumConfigId.检定过程配置, 0, value);
            }
        }
        /// <summary>
        ///启动和潜动的时候同步检定通讯协议试验
        /// </summary>
        public bool IsSame
        {
            get
            {
                return GetConfigBool(EnumConfigId.检定过程配置, 1, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定过程配置, 1, value);
            }
        }

        /// <summary>
        ///隔离不合格表位
        /// </summary>
        public bool IsPartition_Meter
        {
            get
            {
                return GetConfigBool(EnumConfigId.检定过程配置,2, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定过程配置, 2, value);
            }
        }
        /// <summary>
        ///终端地址使用十六进制
        /// </summary>
        public bool IsHexAddress
        {
            get
            {
                return GetConfigBool(EnumConfigId.检定过程配置,3, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定过程配置, 3, value);
            }
        }
        /// <summary>
        ///是否跳过完成的检定项目-true跳过
        /// </summary>
        public bool IsSkipCompleted
        {
            get
            {
                return GetConfigBool(EnumConfigId.检定过程配置, 4, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定过程配置, 4, value);
            }
        }
        /// <summary>
        ///程序退出是否提示关源--true弹出提示，false--直接关源
        /// </summary>
        public bool IsTipsPowerOff
        {
            get
            {
                return GetConfigBool(EnumConfigId.检定过程配置, 5, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定过程配置, 5, value);
            }
        }
        #endregion

        #region 蓝牙光电模块配置
        /// <summary>
        ///ping码
        /// </summary>
        public string Bluetooth_Ping
        {
            get
            {
                return GetConfigString(EnumConfigId.蓝牙光电模式配置, 0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.蓝牙光电模式配置, 0, value);
            }
        }
        /// <summary>
        ///光模式类型--内置光模块-外置光模块
        /// </summary>
        public string Bluetooth_LightModel
        {
            get
            {
                return GetConfigString(EnumConfigId.蓝牙光电模式配置, 1);
            }
            set
            {
                SaveConfigValue(EnumConfigId.蓝牙光电模式配置, 1, value);
            }
        }
        /// <summary>
        ///蓝牙模块发设功率
        /// </summary>
        public string Bluetooth_BluetoothTransmitPower
        {
            get
            {
                return GetConfigString(EnumConfigId.蓝牙光电模式配置, 2);
            }
            set
            {
                SaveConfigValue(EnumConfigId.蓝牙光电模式配置, 2, value);
            }
        }
        /// <summary>
        ///蓝牙模块通信模式-普通检定模式--脉冲跟随模式
        /// </summary>
        public string Bluetooth_CommunicationMode
        {
            get
            {
                return GetConfigString(EnumConfigId.蓝牙光电模式配置, 3);
            }
            set
            {
                SaveConfigValue(EnumConfigId.蓝牙光电模式配置, 3, value);
            }
        }
        /// <summary>
        ///检测表发射功率
        /// </summary>
        public string Bluetooth_MeterTransmitPower
        {
            get
            {
                return GetConfigString(EnumConfigId.蓝牙光电模式配置, 4);
            }
            set
            {
                SaveConfigValue(EnumConfigId.蓝牙光电模式配置, 4, value);
            }
        }
        /// <summary>
        ///检测表频段-全频段-带内频段-外频段
        /// </summary>
        public string Bluetooth_MeterFrequencyBand
        {
            get
            {
                return GetConfigString(EnumConfigId.蓝牙光电模式配置, 5);
            }
            set
            {
                SaveConfigValue(EnumConfigId.蓝牙光电模式配置, 5, value);
            }
        }
        /// <summary>
        ///检测表通道数量
        /// </summary>
        public int Bluetooth_MeterPassCount
        {
            get
            {
                return GetConfigInt(EnumConfigId.蓝牙光电模式配置, 6, 5);
            }
            set
            {
                SaveConfigValue(EnumConfigId.蓝牙光电模式配置, 6, value);
            }
        }
        #endregion

        #region 特殊配置--特定表位设备
        /// <summary>
        ///设备表位设置-
        ///功耗
        ///英文逗号分割如1,2,3...0代表全部表位,-1代表全部不设置
        /// </summary>
        public string DeviceMeter_PowerWaste
        {
            get
            {
                return GetConfigString(EnumConfigId.特殊配置, 0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.特殊配置, 0, value);
            }
        }
        /// <summary>
        ///设备表位设置
        ///二次回路检测设备
        ///英文逗号分割如1,2,3...0代表全部表位,-1代表全部不设置
        /// </summary>
        public string DeviceMeter_2DVD
        {
            get
            {
                return GetConfigString(EnumConfigId.特殊配置, 1);
            }
            set
            {
                SaveConfigValue(EnumConfigId.特殊配置, 1, value);
            }
        }

        /// <summary>
        ///通电检测供电时间
        /// </summary>
        public string PowerOnTime
        {
            get
            {
                return GetConfigString(EnumConfigId.特殊配置, 2);
            }
            set
            {
                SaveConfigValue(EnumConfigId.特殊配置, 2, value);
            }
        }

        #endregion

        #endregion

        #region 加密机
        /// <summary>
        /// 加密机类型
        /// </summary>
        public string Dog_Type
        {
            get
            {
                return GetConfigString(EnumConfigId.加密机配置, 0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.加密机配置, 0, value);
            }
        }
        /// <summary>
        /// 加密机IP
        /// </summary>
        public string Dog_IP
        {
            get
            {
                return GetConfigString(EnumConfigId.加密机配置, 1);
            }
            set
            {
                SaveConfigValue(EnumConfigId.加密机配置, 1, value);
            }
        }
        /// <summary>
        /// 加密机端口
        /// </summary>
        public string Dog_Prot
        {
            get
            {
                return GetConfigString(EnumConfigId.加密机配置, 2);
            }
            set
            {
                SaveConfigValue(EnumConfigId.加密机配置, 2, value);
            }
        }
        /// <summary>
        /// 加密机密钥
        /// </summary>
        public string Dog_key

        {
            get
            {
                return GetConfigString(EnumConfigId.加密机配置, 3);
            }
            set
            {
                SaveConfigValue(EnumConfigId.加密机配置, 3, value);
            }
        }
        /// <summary>
        /// 加密机认证类型--公钥--私钥
        /// </summary>
        public string Dog_CheckingType
        {
            get
            {
                return GetConfigString(EnumConfigId.加密机配置, 4);
            }
            set
            {
                SaveConfigValue(EnumConfigId.加密机配置, 4, value);
            }
        }
        /// <summary>
        /// 加密机-是否进行密码机服务器连接
        /// </summary>
        public bool Dog_IsPassWord
        {
            get
            {
                return GetConfigBool(EnumConfigId.加密机配置, 0,false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.加密机配置, 0, value);
            }
        }
        /// <summary>
        /// 加密机连接模式---服务器版-直连加密机版
        /// </summary>
        public string Dog_ConnectType
        {
            get
            {
                return GetConfigString(EnumConfigId.加密机配置, 6);
            }
            set
            {
                SaveConfigValue(EnumConfigId.加密机配置, 6, value);
            }
        }
        /// <summary>
        /// 加密机超时时间
        /// </summary>
        public string Dog_Overtime
        {
            get
            {
                return GetConfigString(EnumConfigId.加密机配置, 7);
            }
            set
            {
                SaveConfigValue(EnumConfigId.加密机配置, 7, value);
            }
        }
        /// <summary>
        /// 加密机-376.1密钥下装后是否启动密钥开关
        /// </summary>
        public bool Dog_IsKeySwitch_376
        {
            get
            {
                return GetConfigBool(EnumConfigId.加密机配置, 8, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.加密机配置, 8, value);
            }
        }
        /// <summary>
        /// 加密机-698密钥下装后是否启动密钥开关
        /// </summary>
        public bool Dog_IsKeySwitch_698
        {
            get
            {
                return GetConfigBool(EnumConfigId.加密机配置, 9, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.加密机配置, 9, value);
            }
        }
        #endregion

        #region 营销接口

        /// <summary>
        /// 营销接口类型
        /// </summary>
        public string Marketing_Type    
        {
            get
            {
                return GetConfigString(EnumConfigId.营销接口配置, 0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 0, value);
            }
        }

        /// <summary>
        /// 营销下载标识--0条形码  1出厂编号 2表位号
        /// </summary>
        public string Marketing_DewnLoadNumber
        {
            get
            {
                return GetConfigString(EnumConfigId.营销接口配置, 1);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 1, value);
            }
        }

        /// <summary>
        /// 营销系统IP地址
        /// </summary>
        public string Marketing_IP
        {
            get
            {
                return GetConfigString(EnumConfigId.营销接口配置, 2);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 2, value);
            }
        }

        /// <summary>
        /// 营销系统端口号
        /// </summary>
        public string Marketing_Prot
        {
            get
            {
                return GetConfigString(EnumConfigId.营销接口配置,3);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 3, value);
            }
        }

        /// <summary>
        /// 营销系统数据源--就是表名吧应该
        /// </summary>
        public string Marketing_DataSource
        {
            get
            {
                return GetConfigString(EnumConfigId.营销接口配置, 4);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 4, value);
            }
        }

        /// <summary>
        /// 营销——数据库用户名
        /// </summary>
        public string Marketing_UserName
        {
            get
            {
                return GetConfigString(EnumConfigId.营销接口配置, 5);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 5, value);
            }
        }

        /// <summary>
        ///营销——数据库密码
        /// </summary>
        public string Marketing_UserPassWord
        {
            get
            {
                return GetConfigString(EnumConfigId.营销接口配置, 6);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 6, value);
            }
        }

        /// <summary>
        /// WebService路径
        /// </summary>
        public string Marketing_WebService
        {
            get
            {
                return GetConfigString(EnumConfigId.营销接口配置, 7);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 7, value);
            }
        }

        /// <summary>
        /// 上传时时数据
        /// </summary>
        public bool Marketing_UpData
        {
            get
            {
                return GetConfigBool(EnumConfigId.营销接口配置, 8,false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 8, value);
            }
        }
        /// <summary>
        /// 生产平台IP地址
        /// </summary>
        public string Produce_IP
        {
            get
            {
                return GetConfigString(EnumConfigId.营销接口配置, 9);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 9, value);
            }
        }

        /// <summary>
        /// 生产平台端口号
        /// </summary>
        public string Produce_Prot
        {
            get
            {
                return GetConfigString(EnumConfigId.营销接口配置, 10);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 10, value);
            }
        }

        /// <summary>
        /// 生产平台数据源--就是表名吧应该
        /// </summary>
        public string Produce_DataSource
        {
            get
            {
                return GetConfigString(EnumConfigId.营销接口配置, 11);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 11, value);
            }
        }

        /// <summary>
        /// 生产平台——数据库用户名
        /// </summary>
        public string Produce_UserName
        {
            get
            {
                return GetConfigString(EnumConfigId.营销接口配置, 12);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 12, value);
            }
        }

        /// <summary>
        ///生产平台——数据库密码
        /// </summary>
        public string Produce_UserPassWord
        {
            get
            {
                return GetConfigString(EnumConfigId.营销接口配置, 13);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 13, value);
            }
        }

        /// <summary>
        /// 是否自MDS下载检定方案
        /// </summary>
        public bool IsMdsDownScheme
        {
            get
            {
                return GetConfigBool(EnumConfigId.营销接口配置, 14, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.营销接口配置, 14, value);
            }
        }
        #endregion

        #region 网络信息

        /// <summary>
        /// 服务器IP
        /// </summary>
        public string SetControl_Ip
        {
            get
            {
                return GetConfigString(EnumConfigId.集控线配置, 0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.集控线配置, 0, value);
            }
        }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public string SetControl_Prot
        {
            get
            {
                return GetConfigString(EnumConfigId.集控线配置, 1);
            }
            set
            {
                SaveConfigValue(EnumConfigId.集控线配置, 1, value);
            }
        }

        /// <summary>
        /// 是否启用工况信息上报
        /// </summary>
        public string OperatingConditionsYesNo
        {
            get
            {
                return GetConfigString(EnumConfigId.工控平台上报, 0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.工控平台上报, 0, value);
            }
        }
        /// <summary>
        /// 工况服务器IP
        /// </summary>
        public string OperatingConditionsIp
        {
            get
            {
                return GetConfigString(EnumConfigId.工控平台上报, 1);
            }
            set
            {
                SaveConfigValue(EnumConfigId.工控平台上报, 1, value);
            }
        }
        /// <summary>
        /// 服务器端口
        /// </summary>  
        public string OperatingConditionsProt
        {
            get
            {
                return GetConfigString(EnumConfigId.工控平台上报, 2);
            }
            set
            {
                SaveConfigValue(EnumConfigId.工控平台上报, 2, value);
            }
        }

    
        /// <summary>
        /// 工况信息上报频率
        /// </summary>
        public string OperatingConditionsUpdataF
        {
            get
            {
                return GetConfigString(EnumConfigId.工控平台上报, 3);
            }
            set
            {
                SaveConfigValue(EnumConfigId.工控平台上报, 3, value);
            }
        }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string PlantNO
        {
            get
            {
                return GetConfigString(EnumConfigId.工控平台上报, 4);
            }
            set
            {
                SaveConfigValue(EnumConfigId.工控平台上报, 4, value);
            }
        }

        /// <summary>
        /// 工控服务器端口
        /// </summary>
        public string DevControllServerPort
        {
            get
            {
                return GetConfigString(EnumConfigId.工控平台上报, 5);
            }
            set
            {
                SaveConfigValue(EnumConfigId.工控平台上报, 5, value);
            }
        }
        /// <summary>
        /// 工控服务器端口
        /// </summary>
        public bool RGTORZJ
        {
            get
            {
                return GetConfigBool(EnumConfigId.工控平台上报, 6,false);
            }
            set
            {
                GetConfigBool(EnumConfigId.工控平台上报, 6, value);
            }
        }
        
        #endregion

    }
}
