using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Mis.IMICP.IMICPTables
{
  

        #region 6.1
        public class SXDataTableSend
        {

            /// <summary>
            /// 条形码
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string barCode { get; set; }


            /// <summary>
            /// 系统编号
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string sysNo { get; set; }


            /// <summary>
            /// 检定任务号
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string taskNo { get; set; }

            /// <summary>
            ///试验方案名称
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string veriSch { get; set; }
            /// <summary>
            ///设备分类
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string devCls { get; set; }
            /// <summary>
            ///任务状态
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string taskStatus { get; set; }



            /// <summary>
            ///是否自动施封
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string autoSealFlag { get; set; }
            /// <summary>
            ///任务类型
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string taskCateg { get; set; }
            /// <summary>
            ///任务优先级
            /// </summary>
            /// \
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string taskPri { get; set; }
            /// <summary>
            ///检定方式
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string testMode { get; set; }
            /// <summary>
            ///ERP物料代码
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string erpBatchNo { get; set; }


            /// <summary>
            ///型号
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string devModel { get; set; }
            /// <summary>
            ///新设备码
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string equipCodeNew { get; set; }
            /// <summary>
            ///检定设备状态
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string veriDevStat { get; set; }
            /// <summary>
            ///到货批次号
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string arrBatchNo { get; set; }

            /// <summary>
            /// 获取类型，三选一，到货批次号=01/设备码=02/条形码串=03
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string type { get; set; }

            /// <summary>
            /// 条形码串，逗号分割
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string barCodes { get; set; }

            /// <summary>
            /// 设备码
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string devCodeNo { get; set; }

            /// <summary>
            /// 检定任务号
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string veriTaskNo { get; set; }

            /// <summary>
            /// 01-检定任务完成02-分拣任务完成03-复拣任务完成04-出入库任务完成05-盘点任务完成
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string flag { get; set; }


            /// <summary>
            /// 是否合格
            /// </summary>
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string isQualified { get; set; }
            /// <summary>
            /// 箱条码
            /// </summary>
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string boxBarCode { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string devBoxDate { get; set; }

        }
        /// <summary>
        /// 6.1 返回数据
        /// </summary>
        public class VeriTask
        {
            /// <summary>
            /// 条形码
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string barCode { get; set; }


            /// <summary>
            /// 系统编号
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string sysNo { get; set; }

            /// <summary>
            ///检定方案标识
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string trialSchId { get; set; }
            /// <summary>
            ///试验方案名称
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string veriSch { get; set; }
            /// <summary>
            ///设备分类
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string devCls { get; set; }
            /// <summary>
            ///任务状态
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string taskStatus { get; set; }
            /// <summary>
            ///复检方案标识
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string rckSchId { get; set; }
            /// <summary>
            ///任务编号
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string taskNo { get; set; }
            /// <summary>
            ///任务下发时间
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public DateTime taskIssuTime { get; set; }
            /// <summary>
            ///是否自动施封
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string autoSealFlag { get; set; }
            /// <summary>
            ///任务类型
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string taskCateg { get; set; }
            /// <summary>
            ///任务优先级
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string taskPri { get; set; }
            /// <summary>
            ///检定方式
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string testMode { get; set; }
            /// <summary>
            ///ERP物料代码
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string erpBatchNo { get; set; }
            /// <summary>
            ///设备数量
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string devNum { get; set; }
            /// <summary>
            ///总垛数
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string tPileNum { get; set; }
            /// <summary>
            ///型号
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string devModel { get; set; }
            /// <summary>
            ///新设备码
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string equipCodeNew { get; set; }
            /// <summary>
            ///检定设备状态
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string veriDevStat { get; set; }
            /// <summary>
            ///到货批次号
            /// </summary>
            /// 
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string arrBatchNo { get; set; }

        }
        #endregion

        #region 6.2
        public class SendData62
        {
            /// <summary>
            /// 系统编号
            /// </summary>
            /// 
            public string sysNo { get; set; }


            /// <summary>
            /// 检定任务号
            /// </summary>
            /// 
            public string taskNo { get; set; }
            public int pageNo { get; set; }

            public int pageSize { get; set; }
        }
        #endregion

        #region 6.4
        public class GetVeriSchInfo
        {
            public string trialSchId { get; set; }
        }
        #endregion


        #region 6.9 条码返回
        public class TmnlDet
        {
            public string tmnlVoltLv { get; set; }

            public string tmnlRcLv { get; set; }

            public string estabArchDate { get; set; }
            public string tmnlCarrSoftVer { get; set; }
            public string tmnlType { get; set; }
            public string tmnlSpec { get; set; }
            public string tmnlRs485Route { get; set; }
            public string tmnlMfr { get; set; }
            public string devCodeNo { get; set; }
            public string tmnlConst { get; set; }
            public string tmnlChipMfr { get; set; }
            public string tmnlReferFreg { get; set; }
            public string tmnlCommMode { get; set; }
            public string tmnlCarrFreqRng { get; set; }
            public string tmnlDoChannel { get; set; }
            public string tmnlSampPrcsLv { get; set; }

            public string tmnlChipMode { get; set; }
            public string tmnlApPreLv { get; set; }
            public string ftyNo { get; set; }
            public string tmnlCarrType { get; set; }
            public string tmnlPlusRoute { get; set; }

            public string tmnlWireMode { get; set; }
            public string tmnlReferCur { get; set; }
            public string tmnlReferVolt { get; set; }
            public string devStat { get; set; }
            public string tmnlRc { get; set; }
            public string lastChkDate { get; set; }

            public string barCode { get; set; }
            public string tmnlUpChannel { get; set; }
            public string tmnlCollMode { get; set; }
            public string tmnlCateg { get; set; }
            public string tmnlModel { get; set; }
            public string tmnlRpPreLv { get; set; }
            public string arrBatchNo { get; set; }
            public string assetNo { get; set; }
            public string tmnlHwVer { get; set; }
        public string tmnlBaudrate { get; set; }
        
    }
        #endregion

        #region  6.12
        /// <summary>
        /// 6.12 检定综合结论
        /// </summary>
        /// 
        public class VeriDtlFormLists
        {

            /// <summary>
            ///系统编号
            /// </summary>
            public string sysNO { get; set; }

            /// <summary>
            ///检定任务编号
            /// </summary>
            /// 

            public string veriTaskNo { get; set; }
            /// <summary>
            ///设备分类
            /// </summary>
            /// 
            public string devCls { get; set; }

            public List<VeriDtlFormList> veriDtlFormList = new List<VeriDtlFormList>();

            public List<VeriDisqualReasonList> veriDisqualReasonList = new List<VeriDisqualReasonList>();
        }

        public class VeriDtlFormList
        {
            /// <summary>
            ///设备分类
            /// </summary>
            public string devCls { get; set; }
            /// <summary>
            ///检定任务编号
            /// </summary>
            public string veriTaskNo { get; set; }
            /// <summary>
            ///设备单元编号
            /// </summary>
            public string plantElementNo { get; set; }
            /// <summary>
            ///专机编号
            /// </summary>
            public string machNo { get; set; }
            /// <summary>
            ///表位编号
            /// </summary>
            public string devSeatNo { get; set; }

            /// <summary>
            ///资产编号
            /// </summary>
            public string assetNo { get; set; }
            /// <summary>
            ///条形码
            /// </summary>
            public string barCode { get; set; }
            /// <summary>
            ///检定结果
            /// </summary>
            public string veriRslt { get; set; }
            /// <summary>
            ///检定人员
            /// </summary>
            public string veriStf { get; set; }
            /// <summary>
            ///检定部门
            /// </summary>
            public string veriDept { get; set; }
            /// <summary>
            ///检定日期
            /// </summary>
            public string veriDate { get; set; }
            /// <summary>
            ///故障原因
            /// </summary>
            public string faultReason { get; set; }
            /// <summary>
            ///核验人员
            /// </summary>
            public string checkStf { get; set; }
            /// <summary>
            ///试验人员
            /// </summary>
            public string trialStf { get; set; }
            /// <summary>
            ///设备档案编号，检定线台标识
            /// </summary>
            public string plantNo { get; set; }
            /// <summary>
            ///台体编号
            /// </summary>
            public string platformNo { get; set; }
            /// <summary>
            ///温度
            /// </summary>
            public string temp { get; set; }
            /// <summary>
            ///湿度
            /// </summary>
            public string humid { get; set; }
            /// <summary>
            ///核验日期
            /// </summary>
            public string checkDate { get; set; }
            /// <summary>
            ///一级故障原因
            /// </summary>
            public string frstLvFaultReason { get; set; }
            /// <summary>
            ///二级故障原因
            /// </summary>
            public string scndLvFaultReason { get; set; }
        }

        public class VeriDisqualReasonList
        {
            /// <summary>
            /// 检定任务号
            /// </summary>
            public string veriTaskNo { get; set; }

            /// <summary>
            /// 检定系统编号
            /// </summary>
            public string sysNo { get; set; }

            /// <summary>
            /// 检定单元编号
            /// </summary>
            public string veriUnitNo { get; set; }

            /// <summary>
            /// 条形码
            /// </summary>
            public string barCode { get; set; }

            /// <summary>
            /// 不合格原因编码
            /// </summary>
            public string disqualReason { get; set; }

        }

        #endregion

        #region 6.13
        /// <summary>
        /// 设备检定项结果
        /// </summary>
        /// 
        public class VeriDtlFormListsFX
        {

            /// <summary>
            ///系统编号
            /// </summary>
            public string sysNO { get; set; }

            /// <summary>
            ///检定任务编号
            /// </summary>
            /// 

            public string veriTaskNo { get; set; }
            /// <summary>
            ///设备分类
            /// </summary>
            /// 
            public string devCls { get; set; }

            public List<MtDetectTrmlSubitemRslt> mtDetectTrmlSubitemRslt = new List<MtDetectTrmlSubitemRslt>();

            public List<MtDetectTrmlItemRslt> mtDetectTrmlItemRslt = new List<MtDetectTrmlItemRslt>();
        }
        public class ItemRslt
        {
            /// <summary>
            ///检定任务编号
            /// </summary>
            public string veriTaskNo { get; set; }
            /// <summary>
            ///检定项目
            /// </summary>
            public string veriItem { get; set; }
            /// <summary>
            ///检定结果
            /// </summary>
            public string veriRslt { get; set; }
            /// <summary>
            ///检定数据
            /// </summary>
            /// 
            public string veriData { get; set; }

            /// <summary>
            ///检定人员
            /// </summary>
            public string veriStf { get; set; }
            /// <summary>
            ///检定日期
            /// </summary>
            public string veriDate { get; set; }
            /// <summary>
            ///检定项编号
            /// </summary>
            public string veriItemNo { get; set; }
            /// <summary>
            ///设备档案编号，检定线台标识
            /// </summary>
            public string plantNo { get; set; }
            /// <summary>
            ///设备单元编号，多功能检定单元标识
            /// </summary>
            public string plantElementNo { get; set; }
            /// <summary>
            ///专机编号，多功能检定仓标识
            /// </summary>
            public string machNo { get; set; }
            /// <summary>
            ///表位编号 
            /// </summary>
            public string devSeatNo { get; set; }
            /// <summary>
            ///试验人员
            /// </summary>
            public string trialStf { get; set; }
            /// <summary>
            ///核验人员
            /// </summary>
            public string checkStf { get; set; }

            /// <summary>
            ///资产编号
            /// </summary>
            public string assetNo { get; set; }
            /// <summary>
            ///设备条形码
            /// </summary>
            public string barCode { get; set; }

        }

        /// <summary>
        /// 设备检定点结果
        /// </summary>
        public class MtDetectTrmlSubitemRslt
        {
            public string veriSpotName { get; set; }

            public string veriTaskNo { get; set; }

            public string veriRslt { get; set; }

            public string veriSpotNo { get; set; }

            public string assetNo { get; set; }

            public string sysNo { get; set; }

            public string veriOrgNo { get; set; }

            public string veriDate { get; set; }



            public string veriData { get; set; }

            public string veriItemParaNo { get; set; }

            public string barCode { get; set; }
        }
        public class MtDetectTrmlItemRslt : ItemRslt
        {

        }

        #endregion

        #region 6.14
        public class UpLoadSeal : SXDataTableSend
        {
            public string psOrgNo { get; set; }
            public List<SealInst> sealInst = new List<SealInst>();
        }

        /// <summary>
        /// 施封信息集合
        /// </summary>
        public class SealInst
        {
            /// <summary>
            ///条形码
            /// </summary>
            public string barCode { get; set; }
            /// <summary>
            ///施封位置
            /// </summary>
            public string sealPosition { get; set; }
            /// <summary>
            ///施封条码
            /// </summary>
            public string sealBarCode { get; set; }
            /// <summary>
            ///施封日期
            /// </summary>
            public string sealDate { get; set; }
            /// <summary>
            ///施封人员
            /// </summary>
            public string sealerNo { get; set; }
            /// <summary>
            ///是否有效
            /// </summary>
            public string validFlag { get; set; }
            /// <summary>
            ///发行信息
            /// </summary>
            public string releaseInfo { get; set; }


        }
        #endregion


        public class RecvData
        {
            /// <summary>
            /// 成功标志
            /// </summary>
            public string resultFlag { get; set; }

            /// <summary>
            /// 错误信息
            /// </summary>
            public string errorInfo { get; set; }

            public string devCls { get; set; }

            public VeriTask veriTask { get; set; }

            public List<TmnlDet> tmnlDet { get; set; }
        }
    }

