using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.Core.Model.DnbModel.DnbInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZH.MeterProtocol.Protocols.DgnProtocol;
using ZH.MeterProtocol.Struct;

namespace LYZD.Core.Model.Meter
{
    [Serializable()]
    /// <summary>
    /// 检定表的数据
    /// </summary>
    public class TestMeterInfo
    {

        #region MyRegion
        /// <summary>
        /// GPRS上线状态 
        /// </summary>
        public bool Bol_GprsStatus { get; set; }

        //TerminalInfos
        public string MD_JJGC { get; set; }
        public int FKType { get; set; }


        /// <summary>
        /// 协议类型--376.1-698.45
        /// </summary>
        public string MD_Protocol_Type { get; set; }
        /// <summary>
        /// 表id，用户数据库查找
        /// </summary>
        public string Meter_ID { get; set; }
        /// <summary>
        /// 表号，用于加密参数 
        /// </summary>
        public string MD_MeterNo { get; set; }
        /// <summary>
        /// 表位号
        /// </summary>
        public int MD_Epitope { get; set; }
        /// <summary>
        /// 电压
        /// </summary>
        public float MD_UB { get; set; }
        /// <summary>
        /// 电流
        /// </summary>
        public string MD_UA { get; set; }
        /// <summary>
        /// 频率
        /// </summary>
        public int MD_Frequency { get; set; }

        /// <summary>
        /// 首检还是周检
        /// </summary>
        public string MD_TestModel { get; set; }

        /// <summary>
        /// 全检抽检
        /// </summary>
        public string MD_TestType { get; set; }
        /// <summary>
        /// 测量方式--单相-三相三线-三相四线
        /// </summary>
        public string MD_WiringMode { get; set; }
        /// <summary>
        /// 互感器(直接式-互感式)
        /// </summary>
        public string MD_ConnectionFlag { get; set; }
        /// <summary>
        /// 止逆器--有止逆,无止逆
        /// </summary>
        public string MD_CheckDevice { get; set; }
        /// <summary>
        /// 条形码
        /// </summary>
        public string MD_BarCode { get; set; }
        /// <summary>
        /// 资产编号
        /// </summary>
        public string MD_AssetNo { get; set; }
        /// <summary>
        /// 常数
        /// </summary>
        public string MD_Constant { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public string MD_Grane { get; set; }
        /// <summary>
        /// 通讯地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 制造厂家
        /// </summary>
        public string MD_Factory { get; set; }
        /// <summary>
        /// 送检单位
        /// </summary>
        public string MD_Customer { get; set; }
        /// <summary>
        /// 任务编号
        /// </summary>
        public string MD_TaskNo { get; set; }

        /// <summary>
        /// 出厂编号
        /// </summary>
        public string MD_MadeNo { get; set; }
        /// <summary>
        /// 证书编号
        /// </summary>
        public string MD_CertificateNo { get; set; }
        /// <summary>
        /// 是否要检
        /// </summary>
        public bool YaoJianYn { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string MD_BatchNo { get; set; }
        /// <summary>
        /// 终端类型--集中器
        /// </summary>
        public string MD_TerminalType { get; set; }
        /// <summary>
        /// 终端型号
        /// </summary>
        public string MD_TerminalModel { get; set; }
        /// <summary>
        /// 计量编号
        /// </summary>
        public string MD_MeasurementNo { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string MD_IpAddress { get; set; }
        /// <summary>
        /// 通讯方式
        /// </summary>
        public Cus_EmChannelType MD_ConnType { get; set; }
        /// <summary>
        /// 载波厂家
        /// </summary>
        public string MD_CarrierFactory { get; set; }
        /// <summary>
        /// 载波型号
        /// </summary>
        public string MD_CarrierModel { get; set; }
        /// <summary>
        /// 采集器地址
        /// </summary>
        public string MD_CollectorAddress { get; set; }
        /// <summary>
        /// 台体编号
        /// </summary>
        public string MD_DeviceID { get; set; }
        /// <summary>
        /// 出厂日期
        /// </summary>
        public string MD_MadtDate { get; set; }
        /// <summary>
        /// 串口数据
        /// </summary>
        public string MD_PortData { get; set; }
        /// <summary>
        /// 方案编号
        /// </summary>
        public string MD_SchemeID { get; set; }
        /// <summary>
        /// 检验员
        /// </summary>
        public string MD_TestPerson { get; set; }
        /// <summary>
        /// 载波协议
        /// </summary>
        public string MD_CarrName { get; set; }
        /// <summary>
        /// 类别：10 智能表，13 物联电能表
        /// </summary>
        public string MD_Sort { get; set; }
        #endregion


        #region 密钥属性

        /// <summary>
        /// 表随机数
        /// </summary>
        public string Rand { get; set; }

        /// <summary>
        /// 会话计数器 698用
        /// </summary>
        public string SessionNo { get; set; }
        /// <summary>
        /// ESAM序列号
        /// </summary>
        public string EsamId { get; set; }
        /// <summary>
        /// ESAM密钥信息
        /// </summary>
        public string EsamKey { get; set; }
        /// <summary>
        /// ESAM密钥状态[私钥,公钥]: 0-公钥,1-私钥,2-未知
        /// </summary>
        public int EsamStatus { get; set; }
        /// <summary>
        /// 会话密钥,只用在DLT698密钥
        /// </summary>
        public string SessionKey { get; set; }
        #endregion

        /// <summary>
        /// 表位端口信息
        /// </summary>
        public ComPortInfo ProtInfo { get; set; }

        /// <summary>
        /// 电能表多功能通信配置协议
        /// </summary>
        public DgnProtocolInfo DgnProtocol { get; set; }


        //private object obj;
        /// <summary>
        /// 设备实例
        ///MethodInfo mInfo = type.GetMethod(方法名称); //获取当前方法
        /// mInfo.Invoke(type, value);  //接收调用返回值，判断调用是否成功  new object[1] {5}
        /// </summary>
        public object Obj { get; set; }


        //private Type type;
        /// <summary>
        /// 设备 类型
        ///MethodInfo mInfo = type.GetMethod(方法名称); //获取当前方法
        /// mInfo.Invoke(type, value);  //接收调用返回值，判断调用是否成功  new object[1] {5}
        /// </summary>
        public Type Type { get; set; }




        #region 公用
        /// <summary>
        /// 获取表常数 
        /// </summary>
        /// <returns>[有功，无功]</returns>
        public int[] GetBcs()
        {
            MD_Constant = MD_Constant.Replace("（", "(").Replace("）", ")");

            if (MD_Constant.Trim().Length < 1)
            {
                return new int[] { 1, 1 };
            }

            string[] arTmp = MD_Constant.Trim().Replace(")", "").Split('(');

            if (arTmp.Length == 1)
            {
                if (Number.IsNumeric(arTmp[0]))
                    return new int[] { int.Parse(arTmp[0]), int.Parse(arTmp[0]) };
                else
                    return new int[] { 1, 1 };
            }
            else
            {
                if (Number.IsNumeric(arTmp[0]) && Number.IsNumeric(arTmp[1]))
                    return new int[] { int.Parse(arTmp[0]), int.Parse(arTmp[1]) };
                else
                    return new int[] { 1, 1 };
            }
        }




        /// <summary>
        /// 获取电流
        /// </summary>
        /// <returns>[最小电流,最大电流]</returns>
        public float[] GetIb()
        {
            MD_UA = MD_UA.Replace("（", "(").Replace("）", ")");

            if (MD_UA.Trim().Length < 1)
            {
                return new float[] { 1, 1 };
            }

            string[] arTmp = MD_UA.Trim().Replace(")", "").Split('(');

            if (arTmp.Length == 1)
            {
                if (Number.IsNumeric(arTmp[0]))
                    return new float[] { float.Parse(arTmp[0]), float.Parse(arTmp[0]) };
                else
                    return new float[] { 1, 1 };
            }
            else
            {
                if (Number.IsNumeric(arTmp[0]) && Number.IsNumeric(arTmp[1]))
                    return new float[] { float.Parse(arTmp[0]), float.Parse(arTmp[1]) };
                else
                    return new float[] { 1, 1 };
            }
        }


        #endregion


        #region 基本信息
        /// <summary>
        /// 检定日期[YYYY-MM-DD HH:NN:SS]
        /// </summary>
        public string VerifyDate { get; set; }
        /// <summary>
        /// 有效期--计检日期[YYYY-MM-DD HH:NN:SS]
        /// </summary>
        public string EffectiveDate { get; set; }
       
        /// <summary>
        /// 湿度
        /// </summary>
        public string Humidity { get; set; }
        /// <summary>
        /// 温度
        /// </summary>
        public string Temperature { get; set; }
        /// <summary>
        /// 检验员
        /// </summary>
        public string Checker1 { get; set; }
        /// <summary>
        /// 核验员
        /// </summary>
        public string Checker2 { get; set; }

        /// <summary>
        /// 备用1--存放是否需要上传数据--仅自动化线使用，下载数据时赋值
        /// </summary>
        public string Other1 { get; set; }
        /// <summary>
        /// 备用2--存放上传标识--未上传和已上传
        /// </summary>
        public string Other2 { get; set; }
        /// <summary>
        /// 备用3--存放脉冲类型--仅用于物联网表，无--蓝牙脉冲--光电脉冲
        /// </summary>
        public string Other3 { get; set; }
        /// <summary>
        /// 备用4
        /// </summary>
        public string Other4 { get; set; }
        /// <summary>
        /// 备用5
        /// </summary>
        public string Other5 { get; set; }

        /// <summary>
        /// 铅封1
        /// </summary>
        public string Seal1 { get; set; }

        /// <summary>
        /// 铅封2
        /// </summary>
        public string Seal2 { get; set; }

        /// <summary>
        /// 铅封3
        /// </summary>
        public string Seal3 { get; set; }

        /// <summary>
        /// 37铅封号4
        /// </summary>
        public string Seal4 { get; set; }
        /// <summary>
        /// 38铅封号5 不要占用铅封
        /// </summary>
        public string Seal5 { get; set; }

        /// <summary>
        ///  表位在托盘内的ID
        /// </summary>
        public int Meter_TrayTotalID { get; set; }
        /// <summary>
        /// 保存系统编号
        /// </summary>
        public string Meter_SysNo { get; set; }
        #endregion



        #region 一块表检定数据模型

        /// <summary>
        /// 检定数据
        /// </summary>
        public Dictionary<string, MeterResoultData> MeterResoultData = new Dictionary<string, MeterResoultData>();

        /// <summary>
        /// 表的检定结论
        /// </summary>
        public string Result { get; set; }
        ///// <summary>
        ///// 电能表走字数据误差集；Key值为Prj_ID
        ///// </summary>
        //public Dictionary<string, MeterZZError> MeterZZErrors = new Dictionary<string, MeterZZError>();
        ///// <summary>
        ///// 电能表误差集合Key值为项目Prj_ID值，由于特殊检定部分被T出去单独建结构所以不会出现关键字重复的情况 
        ///// </summary>
        //public Dictionary<string, MeterError> MeterErrors = new Dictionary<string, MeterError>();
        ///// <summary>
        ///// 潜动启动数据；Key值为项目Prj_ID值
        ///// </summary>
        //public Dictionary<string, MeterQdQid> MeterQdQids = new Dictionary<string, MeterQdQid>();
        ///// <summary>
        ///// 电能表多功能数据集； Key值为项目Prj_ID值
        ///// </summary>
        //public Dictionary<string, MeterDgn> MeterDgns = new Dictionary<string, MeterDgn>();
        ///// <summary>
        ///// 电能表结论集；Key值为检定项目ID编号格式化字符串。格式为[检定项目ID号]参照数据库结构设计文档中附2
        ///// </summary>
        //public Dictionary<string, MeterResult> MeterResults = new Dictionary<string, MeterResult>();
        ///// <summary>
        ///// 费控数据；Key值为项目Prj_ID值
        ///// </summary>
        //public Dictionary<string, MeterFK> MeterCostControls = new Dictionary<string, MeterFK>();
        ///// <summary>
        ///// 电能表误差一致性集；Key值为项目Prj_ID值
        ///// </summary>
        //public Dictionary<string, MeterErrAccord> MeterErrAccords = new Dictionary<string, MeterErrAccord>();
        ///// <summary>
        ///// 规约一致性数据
        ///// </summary>
        //public Dictionary<string, MeterDLTData> MeterDLTDatas = new Dictionary<string, MeterDLTData>();
        #endregion

    }
}
