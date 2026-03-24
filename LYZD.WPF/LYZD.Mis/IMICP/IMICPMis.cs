using LYZD.Core.Enum;
using LYZD.Core.Helper;
using LYZD.Core.Model.Meter;
using LYZD.Core.Model.Schema;
using LYZD.DAL;
using LYZD.DAL.Config;
using LYZD.Mis.Common;
using LYZD.Mis.IMICP.IMICPTables;
using LYZD.Mis.NanRui.LRDataTable;
using LYZD.Utility.Log;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LYZD.Mis.IMICP
{
    public class IMICPMis : OracleHelper, IMis
    {
        private int SusessCount = 0;
        public IMICPMis(string ip, int port, string dataSource, string userId, string pwd, string url)
        {
            this.Ip = ip;
            this.Port = port;
            this.DataSource = dataSource;
            this.UserId = userId;
            this.Password = pwd;
            this.WebServiceURL = url;
        }
        public bool Down(string barcode, ref TestMeterInfo meter)
        {
            if (ConfigHelper.Instance.RGTORZJ)
            {
                meter.Other5 = ConfigHelper.Instance.SysNO;
               
                string str = GetVeriTask(barcode, meter.Other5);
                //str = "{ \"resultFlag\":\"1\", \"veriTask\": { \"autoSealFlag\":\"01\", \"veriSch\":null, \"veriDevStat\":\"04\", \"devModel\":null, \"taskCateg\":\"13\", \"tPileNum\":0, \"equipCodeNew\":\"8309000004900010\", \"tasklssuTime\":\"2025-12-02 04:02:06\", \"testMode\":\"02\", \"taskNo\":\"6209202512020025\", \"trialSchld\":8000001020005759, \"devNum\":5, \"rckSchld\":null, \"taskPri\":7, \"taskStatus\":\"03\", \"devCls\":\"09\", \"arrBatchNo\":\"6225120000100634\" } }";

                try
                {
                    RecvData data = JsonHelper.反序列化字符串<RecvData>(str);
                    if (data.resultFlag == "1")
                    {
                        meter.MD_SchemeID = data.veriTask.trialSchId;  //检定方案标识
                        //meter.veriSch = data.veriTask.veriSch;         //方案名称
                        meter.MD_TerminalModel = data.veriTask.devCls; //设备分类
                        //meter.taskStatus = data.veriTask.taskStatus;   //任务状态
                        //meter.rckSchId = data.veriTask.rckSchId;       //复检方案标识
                        meter.MD_TaskNo = data.veriTask.taskNo;        //任务编号
                        //meter.taskIssuTime = data.veriTask.taskIssuTime; //任务下发时间
                        //meter.autoSealFlag = data.veriTask.autoSealFlag; //是否自动施封
                        //meter.taskCateg = data.veriTask.taskCateg;       //任务类型
                        //meter.taskPri = data.veriTask.taskPri;           //任务优先级
                        //meter.testMode = data.veriTask.testMode;         //检定方式
                        //meter.erpBatchNo = data.veriTask.erpBatchNo;     //ERP物料代码
                        //meter.devNum = data.veriTask.devNum;             //设备数量
                        //meter.tPileNum = data.veriTask.tPileNum;         //总垛数
                        meter.MD_TerminalModel = data.veriTask.devModel; //型号
                        //meter. = data.veriTask.equipCodeNew;             //新设备码
                        //meter.veriDevStat = data.veriTask.veriDevStat;   //检定设备状态
                        meter.MD_BatchNo = data.veriTask.arrBatchNo;     //到货批次号

                        string str69 = GetEquioParam("03", barcode);
                        LogManager.AddMessage(string.Format("返回数据，------返回数据{0}", str69), 1);

                        //str69 = "{\"errorinfo\":\"SUCCESS\", \"resultFlag\":\"1\", \"tmnlDet\":[ { \"logAddr\":null, \"commProtVer\":null, \"estabArchDate\":\"2025-02-18 12:00:00\", \"tmnlCarrSoftVer\":null, \"tmnlType\":\"11\", \"tmnlSpec\":\"24\", \"tmnlRs485Route\":null, \"tmnlMfr\":\"62008005\", \"devCodeNo\":\"8309000004900010\", \"tmnlConst\":null, \"tmnlChipMfr\":null, \"swVer\":null, \"tmnlReferFreg\":null, \"tmnlCommMode\":\"29\", \"tmnlCarrFreqRng\":null, \"tmnlDoChannel\":null, \"tmnlChipMode\":null, \"tmnlApPreLv\":\"32\", \"commaddr\":\"000001416222\", \"commProt\":\"09\", \"ftyNo\":\"00000001416222\", \"devId\":\"6225120000100637\", \"removeDate\":null, \"dsgnDur\":null, \"tmnlCarrType\":null, \"tmnlWireMode\":\"03\", \"tmnlReferCur\":null, \"tmnlReferVolt\":\"03\", \"hwVer\":\"05\", \"devStat\":\"04\", \"delvrInWhTime\":null, \"rmReason\":null, \"assetNo\":\"1430009000004410113050\", \"tmnlRc\":\"157\", \"lastChkDate\":null, \"tmnlSampPrcsLv\":null, \"barCode\":\"1430009000004410113050\", \"baudRate\":null, \"tmnlUpChannel\":null, \"madeStd\":null, \"tmnlCollMode\":\"43\", \"tmnlBaudrate\":null, \"tmnlCateg\":\"21\", \"tmnlRv\":\"03\", \"tmnlModel\":\"1644\", \"tmnlRpPreLy\":\"21\", \"arrBatchNo\":\"1425021800002010\" }], \"devCls\":\"09\" }";
                        RecvData data69 = JsonHelper.反序列化字符串<RecvData>(str69);
                        {
                            //读取数据库文件
                            if (data69.tmnlDet.Count > 0)
                            {
                                meter.MD_BarCode = data69.tmnlDet[0].barCode;  //条形码
                                meter.MD_TerminalModel = GetData("tmnlModel", data69.tmnlDet[0].tmnlModel); //采集终端型号 
                                string tmnlCollMode = GetData("tmnlCollMode", data69.tmnlDet[0].tmnlCollMode); //通讯类型
                                string tmnlType = GetData("tmnlType", data69.tmnlDet[0].tmnlType); //类型
                                meter.MD_WiringMode = GetData("tmnlWireMode", data69.tmnlDet[0].tmnlWireMode); //接线方式
                                string tmnlUpChannel = GetData("tmnlUpChannel", data69.tmnlDet[0].tmnlUpChannel);//上行通信信道
                                string tmnlConst = GetData("tmnlConst", data69.tmnlDet[0].tmnlConst); //有功无功常数
                                meter.MD_Constant = tmnlConst + "(" + tmnlConst + ")";
                                string tmnlCateg = GetData("tmnlCateg", data69.tmnlDet[0].tmnlCateg); //采集终端类别
                                string tmnlApPreLv = GetData("tmnlApPreLv", data69.tmnlDet[0].tmnlApPreLv); //有功准确度等级
                                string tmnlRpPreLv = GetData("tmnlRpPreLv", data69.tmnlDet[0].tmnlRpPreLv); //无功准确度等级                                                         
                                string tmnlVoltLv = data69.tmnlDet[0].tmnlVoltLv; //交流电压等级
                                string tmnlRcLv = data69.tmnlDet[0].tmnlRcLv; //交流电流等级
                                string tmnlReferFreg = data69.tmnlDet[0].tmnlReferFreg;


                                int pl = 50;
                                if (int.TryParse(tmnlReferFreg, out pl))
                                {
                                    meter.MD_Frequency = pl;
                                }
                                else
                                {
                                    meter.MD_Frequency = 50;
                                }
                                string tmnlCommMode = GetData("tmnlCommMode", data69.tmnlDet[0].tmnlCommMode); //通讯方式

                                string tmnlRc = GetData("tmnlRc", data69.tmnlDet[0].tmnlRc); //电流

                                int temp = 220;
                                int.TryParse(GetData("tmnlReferVolt", data69.tmnlDet[0].tmnlReferVolt), out temp); //参比电压
                                meter.MD_UB = temp;

                                meter.MD_UA = GetData("tmnlReferCur", data69.tmnlDet[0].tmnlRc).Replace("（", "(").Replace("）", ")"); //参比电流

                                string tmnlDoChannel = GetData("tmnlDoChannel", data69.tmnlDet[0].tmnlDoChannel); //下行通信信道
                                string tmnlSampPrcsLv = data69.tmnlDet[0].tmnlSampPrcsLv; //采样精度等级 
                                string tmnlSpec = GetData("tmnlSpec", data69.tmnlDet[0].tmnlSpec);  //规格
                                string tmnlChipMfr = GetData("tmnlChipMfr", data69.tmnlDet[0].tmnlChipMfr); //载波芯片厂商
                                string tmnlChipMode = GetData("tmnlChipMode", data69.tmnlDet[0].tmnlChipMode); //载波芯片型号
                                string tmnlCarrSoftVer = GetData("tmnlCarrSoftVer", data69.tmnlDet[0].tmnlCarrSoftVer); //载波软件版本
                                string tmnlCarrFreqRng = GetData("tmnlCarrFreqRng", data69.tmnlDet[0].tmnlCarrFreqRng); //载波频率范围
                                string tmnlRs485Route = data69.tmnlDet[0].tmnlRs485Route; //RS485路数

                                string tmnlCarrType = GetData("tmnlCarrType", data69.tmnlDet[0].tmnlCarrType); //载波类型
                                string tmnlPlusRoute = data69.tmnlDet[0].tmnlPlusRoute; //脉冲路数
                                string tmnlMfr = GetData("tmnlMfr", data69.tmnlDet[0].tmnlMfr); //生产厂家
                                meter.MD_Factory = tmnlMfr;
                                string tmnlBaudrate = data69.tmnlDet[0].tmnlBaudrate; //波特率
                                string estabArchDate = data69.tmnlDet[0].estabArchDate; //建档时间
                                string lastChkDate = data69.tmnlDet[0].lastChkDate;//上次检定时间
                                string devCodeNo = data69.tmnlDet[0].devCodeNo;//设备码
                                string arrBatchNo = data69.tmnlDet[0].arrBatchNo;//到货批次号
                                string devStat = data69.tmnlDet[0].devStat;//设备状态
                                string ftyNo = data69.tmnlDet[0].ftyNo;//出厂编号
                                meter.Other3 = data69.tmnlDet[0].assetNo; //资产编号
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                //数据字典暂时不全，无法完成
                string str = gkReciveVeriEquipDoc(barcode, ref meter);
                dynamic data = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(str);
                meter.MD_TaskNo = data.equipInfoList[0].taskNo;
                string equipCateg = data.equipInfoList[0].equipCateg;

                string bidBatchNo = data.equipInfoList[0].bidBatchNo;
                string sampleName = data.equipInfoList[0].sampleName;
                string manufacturerName = data.equipInfoList[0].manufacturerName;
                meter.MD_Factory = manufacturerName;

                string sampleModel = data.equipInfoList[0].sampleModel;
                string vacStandard = data.equipInfoList[0].vacStandard;

                if (vacStandard.Split('|')[0] == "3×220/380V")
                {
                    meter.MD_UB = 220f;
                    meter.MD_UA = vacStandard.Split('|')[1].Replace("A", "");
                }

                string conMode = data.equipInfoList[0].conMode;
                string wiringMode = data.equipInfoList[0].wiringMode;
                string freq = data.equipInfoList[0].freq;
                string commProt = data.equipInfoList[0].commProt;
                string carrierChipManufName = data.equipInfoList[0].carrierChipManufName;
                string carrierFreq = data.equipInfoList[0].carrierFreq;
                string carrierType = data.equipInfoList[0].carrierType;
                string bothWayPowerFlag = data.equipInfoList[0].bothWayPowerFlag;
                string apPreLevel = data.equipInfoList[0].apPreLevel;
                string apConstant = data.equipInfoList[0].apConstant;
                string rpPreLevel = data.equipInfoList[0].rpPreLevel;
                string rpConstant = data.equipInfoList[0].rpConstant;
                return true;
            }
        }

        public string GetData(string codeKey, string codevalue)
        {
            try
            {
                string str = "";
                if (!string.IsNullOrWhiteSpace(codevalue))
                {
                    string infoname = "p_code_type";
                    string dataname = "p_code";
                    GeneralDal generalDal = DALManager.IMICPData;

                    DynamicModel model = generalDal.GetByID(infoname, $"devCls ='{"09"}' and paraNo='{codeKey}'");
                    if (model == null)
                    {
                        return str;
                    }
                    DynamicModel models = generalDal.GetByID(dataname, $"codeKey ='{model.GetProperty("codeKey")}' and codeValue ='{codevalue}'");
                    if (models != null)
                    {
                        str = models.GetProperty("codeValueName").ToString();
                    }
                    return str;
                }
                else
                {
                    return str;
                }
            }
            catch
            {
                string str = "";
                return str;
            }
        }

        public void DownTask(string type, string data, ref RecvData obj)
        {
            throw new NotImplementedException();
        }
        public bool DownTask(string MD_BarCode, ref MT_DETECT_OUT_EQUIP mT_DETECT_TASK)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 和客户沟通方案采用本地方案
        /// </summary>
        /// <param name="SchemeDown"></param>
        /// <param name="schemeName"></param>
        /// <param name="Schema"></param>
        /// <returns></returns>
        public bool SchemeDown(string SchemeDown, out string schemeName, out Dictionary<string, SchemaNode> Schema)
        {
            if (ConfigHelper.Instance.RGTORZJ)
            {
                string str = GetVeriSchInfo64(SchemeDown);
                Schema = new Dictionary<string, SchemaNode>();
                schemeName = "";
                return true;
            }
            else
            {
                schemeName = SchemeDown.Split('|')[0];
                string str = gkReciveVeriSchemeInfo("");
                dynamic data = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(str.ToString());
                if (data.resultFlag == "1")
                {
                    int i = data.schemeList.Count;
                    foreach (var item in data.schemeList)
                    {
                    }
                }
                Schema = new Dictionary<string, SchemaNode>();
                schemeName = "";
                return true;
            }
        }

        public void ShowPanel(Control panel)
        {
            throw new NotImplementedException();
        }

        public bool Update(TestMeterInfo meters)
        {
            throw new NotImplementedException();
        }

        public bool Update(List<TestMeterInfo> meters)
        {
            if (ConfigHelper.Instance.RGTORZJ)
            {
                bool flag = false;
                foreach (TestMeterInfo item in meters)
                {
                    // UploadSealsCode(item); //施封信息上传                //6.15 数据上传
                    flag = ZongResult(item);
                    if (!flag)
                    {
                        LogManager.AddMessage("综合结论上传失败" + item.MD_BarCode, EnumLogSource.服务器日志, EnumLevel.Error);
                        return false;
                    }
                    flag = FenxiangResult(item);
                    if (!flag)
                    {
                        LogManager.AddMessage("分项结论上传失败" + item.MD_BarCode, EnumLogSource.服务器日志, EnumLevel.Error);
                        return false;
                    }
                }
                return flag;
            }
            else
            {
                bool flag = false;
                foreach (TestMeterInfo item in meters)
                {
                    flag= gkSendTestResultData(item);
                    if (!flag)
                    {
                        LogManager.AddMessage("结论上传失败" + item.MD_BarCode, EnumLogSource.服务器日志, EnumLevel.Error);
                        return false;
                    }
                }
                return flag;
            }
        }

        public bool UploadPackInfo(string barCode, string sysno, string detectTaskNo, string moduleBarCode, string id)
        {
            string webNoticeNo = "01";
            string moduleTypeCode = "";
            if (ConfigHelper.Instance.OperatingConditionsYesNo == "是")
            {
                JObject josend = new JObject();
                josend.Add("sysNo", sysno);
                josend.Add("detectTaskNo", detectTaskNo);
                josend.Add("webNoticeNo", webNoticeNo);

                JObject josendInfo = new JObject();
                josendInfo.Add("barCode", barCode); //
                josendInfo.Add("certDate", DateTime.Now.ToString("yyyy-MM-dd"));
                josendInfo.Add("moduleTypeCode", moduleTypeCode);
                josendInfo.Add("moduleBarCode", moduleBarCode);
                josendInfo.Add("hplcID", moduleBarCode);
                josendInfo.Add("isLegal", "00");
                josendInfo.Add("writeDate", DateTime.Now.ToString("yyyy-MM-dd"));

                josend.Add("hplcInfo", josendInfo);


                string url = GetJson("6.15");
                string str = josend.ToString();
                JObject json = JObject.Parse(str);
                string log = json.ToString();
                LogManager.AddMessage(string.Format("调用接口{0}，上报数据{1}", url, log), 1);
                string strrecv = Post(url, str);
                LogManager.AddMessage(string.Format("接口{0}，返回数据{1}", url, strrecv), 1);
                RecvData recv = JsonHelper.反序列化字符串<RecvData>(strrecv);
                if (recv.resultFlag == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }

        public void UpdateCompleted()
        {

        }

        public bool UpdateCompleted(string DETECT_TASK_NO, string SYS_NO)
        {
            return SendTaskFinish(DETECT_TASK_NO);
        }

        public void UpdateInit()
        {
            SusessCount = 0;
        }

        #region

        /// <summary>
        /// 分项结论上传
        /// </summary>
        /// <param name="meters"></param>
        /// <returns></returns>
        private bool FenxiangResult(TestMeterInfo item)
        {
            string sysno = ConfigHelper.Instance.SysNO;
            item.MD_TaskNo = item.MD_TaskNo;
            item.MD_BarCode = item.MD_BarCode;
            VeriDtlFormListsFX objFX = new VeriDtlFormListsFX();
            objFX.devCls = "09";
            objFX.veriTaskNo = item.MD_TaskNo;
            objFX.sysNO = sysno;

            foreach (var item1 in item.MeterResoultData)
            {
                //芯片ID单独上传
                if (item1.Key == "02016")
                {
                    string id = item1.Value.meterResoults[0].ItemDatas[1].TerminalData;
                    UploadPackInfo(item.MD_BarCode, sysno, item.MD_TaskNo, item.MD_CertificateNo, id);
                    continue;
                }
                if (item1.Key == ProjectID.基本误差试验)
                {
                    int counts = 1;
                    MtDetectTrmlItemRslt objFXYe = new MtDetectTrmlItemRslt();
                    objFXYe.veriTaskNo = item.MD_TaskNo; //"检定任务编号",
                    objFXYe.veriItem = "初始固有误差试验"; //检定项目
                    objFXYe.veriRslt = item1.Value.Result == "合格" ? "02" : "01"; //检定结果
                    objFXYe.veriStf = item.Checker1; //检定人员
                    objFXYe.veriDate = item.VerifyDate.Replace("/", "-"); //检定日期
                    objFXYe.veriItemNo = "12011";  //检定项编号
                    objFXYe.plantNo = ConfigHelper.Instance.PlantNO;  //设备档案编号，检定线台标识
                    objFXYe.plantElementNo = sysno;  //设备单元编号，多功能检定单元标识
                    objFXYe.machNo = sysno;          //专机编号，多功能检定仓标识
                    objFXYe.devSeatNo = item.MD_Epitope.ToString(); //表位编号
                    objFXYe.trialStf = item.Checker1;    //试验人员
                    objFXYe.checkStf = item.Checker2;    //核验人员
                    objFXYe.assetNo = item.Other3;               //资产编号 
                    objFXYe.barCode = item.MD_BarCode;
                    objFX.mtDetectTrmlItemRslt.Add(objFXYe);
                    foreach (var itemss in item1.Value.meterResoults)
                    {
                        item.VerifyDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        MtDetectTrmlSubitemRslt objFXNo = new MtDetectTrmlSubitemRslt();
                        objFXNo.veriSpotName = itemss.Datas["项目名"];  //检定点名称
                        objFXNo.veriTaskNo = item.MD_TaskNo;    //检定点编号--任务编号
                        objFXNo.veriRslt = itemss.Result == "合格" ? "02" : "01"; //检定结果 
                        objFXNo.veriSpotNo = counts.ToString();   //"检定点编码",
                        objFXNo.assetNo = item.Other3;   //"资产编号",
                        objFXNo.sysNo = sysno;  //系统编号
                        objFXNo.veriOrgNo = "01";     //"检定单位",
                        objFXNo.veriDate = Convert.ToDateTime(item.VerifyDate).ToString(); //"检定日期"
                        objFXNo.veriData = ""; //"检定数据",
                        objFXNo.veriItemParaNo = "12011";  //"检定项参数编码",
                        objFXNo.barCode = item.MD_BarCode;
                        objFX.mtDetectTrmlSubitemRslt.Add(objFXNo);
                        counts++;
                    }
                    continue;
                }
                if (item1.Key == ProjectID.常温基本误差)
                {
                    string[] strLen = { "电压误差", "电流误差", "有功功率误差", "无功功率误差", "视在功率基本误差", "功率因素基本误差" };
                    for(int i = 0; i < strLen.Length; i++)
                    {
                        int counts = 1;
                        MtDetectTrmlItemRslt objFXYe = new MtDetectTrmlItemRslt();
                        objFXYe.veriTaskNo = item.MD_TaskNo; //"检定任务编号",
                        objFXYe.veriItem = strLen[i]; //检定项目
                        objFXYe.veriRslt = item1.Value.Result == "合格" ? "02" : "01"; //检定结果
                        objFXYe.veriStf = item.Checker1; //检定人员
                        objFXYe.veriDate = item.VerifyDate.Replace("/", "-"); //检定日期
                        objFXYe.veriItemNo = item1.Key+i;  //检定项编号
                        objFXYe.plantNo = ConfigHelper.Instance.PlantNO;  //设备档案编号，检定线台标识
                        objFXYe.plantElementNo = sysno;  //设备单元编号，多功能检定单元标识
                        objFXYe.machNo = sysno;          //专机编号，多功能检定仓标识
                        objFXYe.devSeatNo = item.MD_Epitope.ToString(); //表位编号
                        objFXYe.trialStf = item.Checker1;    //试验人员
                        objFXYe.checkStf = item.Checker2;    //核验人员
                        objFXYe.assetNo = item.Other3;               //资产编号 
                        objFXYe.barCode = item.MD_BarCode;
                        objFX.mtDetectTrmlItemRslt.Add(objFXYe);
                        for (int j=0;j<2;j++)
                        {
                            item.VerifyDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            MtDetectTrmlSubitemRslt objFXNo = new MtDetectTrmlSubitemRslt();
                            objFXNo.veriSpotName = item1.Value.meterResoults[0].ItemDatas[2*i+j].Name;  //检定点名称
                            objFXNo.veriTaskNo = item.MD_TaskNo;    //检定点编号--任务编号
                            objFXNo.veriRslt = item1.Value.meterResoults[0].ItemDatas[2 * i].Resoult == "合格" ? "02" : "01"; //检定结果 
                            objFXNo.veriSpotNo = counts.ToString();   //"检定点编码",
                            objFXNo.assetNo = item.Other3;   //"资产编号",
                            objFXNo.sysNo = sysno;  //系统编号
                            objFXNo.veriOrgNo = "01";     //"检定单位",
                            objFXNo.veriDate = Convert.ToDateTime(item.VerifyDate).ToString(); //"检定日期"
                            objFXNo.veriData = ""; //"检定数据",
                            objFXNo.veriItemParaNo = item1.Key + i;   //"检定项参数编码",
                            objFXNo.barCode = item.MD_BarCode;
                            objFX.mtDetectTrmlSubitemRslt.Add(objFXNo);
                            counts++;
                        }
                    }
                    continue;
                }
                if (item1.Key == ProjectID.走字试验)
                {
                    item1.Value.meterResoults[0].Datas["项目名"]= "常数试验";

                    MtDetectTrmlItemRslt objFXYe = new MtDetectTrmlItemRslt();
                    objFXYe.veriTaskNo = item.MD_TaskNo; //"检定任务编号",
                    objFXYe.veriItem = GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[0]; //检定项目
                    objFXYe.veriRslt = item1.Value.Result == "合格" ? "02" : "01"; //检定结果
               
                    objFXYe.veriStf = item.Checker1; //检定人员
                    objFXYe.veriDate = item.VerifyDate.Replace("/", "-"); //检定日期
                    objFXYe.veriItemNo = GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1];  //检定项编号
                    objFXYe.plantNo = ConfigHelper.Instance.PlantNO;  //设备档案编号，检定线台标识
                    objFXYe.plantElementNo = sysno;  //设备单元编号，多功能检定单元标识
                    objFXYe.machNo = sysno;          //专机编号，多功能检定仓标识
                    objFXYe.devSeatNo = item.MD_Epitope.ToString(); //表位编号
                    objFXYe.trialStf = item.Checker2;    //试验人员
                    objFXYe.checkStf = item.Checker2;    //核验人员
                    objFXYe.assetNo = item.Other3;               //资产编号 //挂表扫码
                    objFXYe.barCode = item.MD_BarCode;
                    objFX.mtDetectTrmlItemRslt.Add(objFXYe);

                    int counts = 1;

                    foreach (var itemss in item1.Value.meterResoults)
                    {
                        item.VerifyDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        MtDetectTrmlSubitemRslt objFXNo = new MtDetectTrmlSubitemRslt();
                        objFXNo.veriSpotName = itemss.Datas["项目名"];  //检定点名称
                        objFXNo.veriTaskNo = item.MD_TaskNo;    //检定点编号--任务编号
                        objFXNo.veriRslt = itemss.Result == "合格" ? "02" : "01"; //检定结果 
                        objFXNo.veriSpotNo = counts.ToString();   //"检定点编码",
                        objFXNo.assetNo = item.Other3;   //"资产编号",
                        objFXNo.sysNo = sysno;  //系统编号
                        objFXNo.veriOrgNo = "01";     //"检定单位",
                        objFXNo.veriDate = Convert.ToDateTime(item.VerifyDate).ToString(); //"检定日期"
                        objFXNo.veriData = ""; //"检定数据",
                        objFXNo.veriItemParaNo = "12011";  //"检定项参数编码",
                        objFXNo.barCode = item.MD_BarCode;
                        objFX.mtDetectTrmlSubitemRslt.Add(objFXNo);
                        counts++;
                    }

                    continue;
                }
                if (item1.Key == ProjectID.日计时误差)
                {
                    int counts = 1;
                    MtDetectTrmlItemRslt objFXYe = new MtDetectTrmlItemRslt();
                    objFXYe.veriTaskNo = item.MD_TaskNo; //"检定任务编号",
                    objFXYe.veriItem = "日计时误差"; //检定项目
                    objFXYe.veriRslt = item1.Value.Result == "合格" ? "02" : "01"; //检定结果
                    objFXYe.veriStf = item.Checker1; //检定人员
                    objFXYe.veriDate = item.VerifyDate.Replace("/", "-"); //检定日期
                    objFXYe.veriItemNo = ProjectID.日计时误差;  //检定项编号
                    objFXYe.plantNo = ConfigHelper.Instance.PlantNO;  //设备档案编号，检定线台标识
                    objFXYe.plantElementNo = sysno;  //设备单元编号，多功能检定单元标识
                    objFXYe.machNo = sysno;          //专机编号，多功能检定仓标识
                    objFXYe.devSeatNo = item.MD_Epitope.ToString(); //表位编号
                    objFXYe.trialStf = item.Checker1;    //试验人员
                    objFXYe.checkStf = item.Checker2;    //核验人员
                    objFXYe.assetNo = item.Other3;               //资产编号 
                    objFXYe.barCode = item.MD_BarCode;
                    objFX.mtDetectTrmlItemRslt.Add(objFXYe);
                    foreach (var itemss in item1.Value.meterResoults)
                    {
                        item.VerifyDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        MtDetectTrmlSubitemRslt objFXNo = new MtDetectTrmlSubitemRslt();
                        objFXNo.veriSpotName = itemss.Datas["项目名"];  //检定点名称
                        objFXNo.veriTaskNo = item.MD_TaskNo;    //检定点编号--任务编号
                        objFXNo.veriRslt = itemss.Result == "合格" ? "02" : "01"; //检定结果 
                        objFXNo.veriSpotNo = counts.ToString();   //"检定点编码",
                        objFXNo.assetNo = item.Other3;   //"资产编号",
                        objFXNo.sysNo = sysno;  //系统编号
                        objFXNo.veriOrgNo = "01";     //"检定单位",
                        objFXNo.veriDate = Convert.ToDateTime(item.VerifyDate).ToString(); //"检定日期"
                        objFXNo.veriData = itemss.Datas["误差1"]+","+ 
                            itemss.Datas["误差2"] + "," +
                            itemss.Datas["误差3"] + "," +
                            itemss.Datas["误差4"] + "," +
                            itemss.Datas["误差5"] ; //"检定数据",
                        objFXNo.veriItemParaNo = ProjectID.日计时误差;   //"检定项参数编码",
                        objFXNo.barCode = item.MD_BarCode;
                        objFX.mtDetectTrmlSubitemRslt.Add(objFXNo);
                        counts++;
                    }
                    continue;
                }
                MtDetectTrmlItemRslt objFXYes = new MtDetectTrmlItemRslt();
                objFXYes.veriTaskNo = item.MD_TaskNo; //"检定任务编号",
                objFXYes.veriItem = GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[0]; //检定项目
                objFXYes.veriRslt = item1.Value.Result == "合格" ? "02" : "01"; //检定结果
                string strdata = "";
                objFXYes.veriStf = item.Checker1; //检定人员
                objFXYes.veriDate = item.VerifyDate.Replace("/", "-"); //检定日期
                objFXYes.veriItemNo = GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1];  //检定项编号
                objFXYes.plantNo = ConfigHelper.Instance.PlantNO;  //设备档案编号，检定线台标识
                objFXYes.plantElementNo = sysno;  //设备单元编号，多功能检定单元标识
                objFXYes.machNo = sysno;          //专机编号，多功能检定仓标识
                objFXYes.devSeatNo = item.MD_Epitope.ToString(); //表位编号
                objFXYes.trialStf = item.Checker2;    //试验人员
                objFXYes.checkStf = item.Checker2;    //核验人员
                objFXYes.assetNo = item.Other3;               //资产编号 //挂表扫码
                objFXYes.barCode = item.MD_BarCode;
                objFX.mtDetectTrmlItemRslt.Add(objFXYes);

                int count = 1;
                foreach (var itemss in item1.Value.meterResoults[0].ItemDatas)
                {
                    item.VerifyDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    MtDetectTrmlSubitemRslt objFXNo = new MtDetectTrmlSubitemRslt();
                    objFXNo.veriSpotName = itemss.Name;  //检定点名称
                    objFXNo.veriTaskNo = item.MD_TaskNo;    //检定点编号--任务编号
                    objFXNo.veriRslt = itemss.Resoult == "合格" ? "02" : "01"; //检定结果 
                    objFXNo.veriSpotNo = count.ToString();   //"检定点编码",
                    objFXNo.assetNo = item.Other3; ;  //"资产编号",
                    objFXNo.sysNo = sysno;  //系统编号
                    objFXNo.veriOrgNo = "01";     //"检定单位",
                    objFXNo.veriDate = Convert.ToDateTime(item.VerifyDate).ToString(); //"检定日期"
                    objFXNo.veriData = ""; //"检定数据",
                    objFXNo.veriItemParaNo = GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1];  //"检定项参数编码",
                    objFXNo.barCode = item.MD_BarCode;
                    objFX.mtDetectTrmlSubitemRslt.Add(objFXNo);
                    count++;
                }
            }

            string url = GetJson("6.13");
            string str = JsonHelper.序列化对象(objFX);
            JObject json = JObject.Parse(str);
            string log = json.ToString();
            LogManager.AddMessage(string.Format("调用接口{0}，上报数据\r\n{1}", url, log), 1);
            string strrecv = Post(url, str);
            LogManager.AddMessage(string.Format("分项结论上传，返回数据\r\n{0}", strrecv), 1);
            RecvData recv = JsonHelper.反序列化字符串<RecvData>(strrecv);

            LogManager.AddMessage("分项结论上传完成" + item.MD_BarCode + "---" + strrecv, EnumLogSource.服务器日志, EnumLevel.Information);
            if (recv.resultFlag == "1")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 总结论上传
        /// </summary>
        /// <param name="meters"></param>
        /// <returns></returns>
        private bool ZongResult(TestMeterInfo item)
        {
            try
            {
                string sysno = item.Other5;
                VeriDtlFormLists obj = new VeriDtlFormLists();
                obj.sysNO = sysno;
                obj.veriTaskNo = item.MD_TaskNo;
                obj.devCls = "09";
                    VeriDtlFormList objYes = new VeriDtlFormList();
                    objYes.veriTaskNo = item.MD_TaskNo;
                    objYes.plantElementNo = sysno;
                    objYes.machNo = sysno;
                    objYes.devSeatNo = item.MD_Epitope.ToString();
                    objYes.devCls = "09";
                    objYes.assetNo = item.Other3;   //--资产编号
                    objYes.barCode = item.MD_BarCode;
                    objYes.veriRslt = item.Result == "合格" ? "02" : "01";
                    objYes.veriStf = item.Checker1;
                    objYes.veriDept = "";
                    objYes.veriDate = item.VerifyDate.Replace("/", "-");
                    objYes.faultReason = "";  //-- 
                    objYes.checkStf = item.Checker2;
                    objYes.trialStf = item.Checker2;
                    objYes.plantNo = ConfigHelper.Instance.PlantNO;
                    objYes.platformNo = item.MD_DeviceID;
                    objYes.temp = item.Temperature;
                    objYes.humid = item.Humidity;
                    objYes.checkDate = item.VerifyDate.Replace("/", "-");
                    objYes.frstLvFaultReason = "";  //-- 
                    objYes.scndLvFaultReason = ""; //-- 
                    obj.veriDtlFormList.Add(objYes);
                if (item.Result == Const.不合格)
                {
                    VeriDisqualReasonList objNO = new VeriDisqualReasonList();
                    objNO.veriTaskNo = item.MD_TaskNo;
                    objNO.sysNo = sysno;
                    objNO.veriUnitNo = "";
                    objNO.barCode = item.MD_BarCode;
                    objNO.disqualReason = "";
                    obj.veriDisqualReasonList.Add(objNO);
                }

                string url = GetJson("6.12");
                string str = JsonHelper.序列化对象(obj);
                JObject json = JObject.Parse(str);
                string log = json.ToString();
                LogManager.AddMessage(string.Format("调用接口{0}，上报数据{1}\r\n", url, log), 1);
                string strrecv = Post(url, str);
                LogManager.AddMessage(string.Format("综合结论上传，接口{0}，返回数据\r\n{1}", url, strrecv), 1);
                RecvData recv = JsonHelper.反序列化字符串<RecvData>(strrecv);
                if (recv.resultFlag == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                LogManager.AddMessage(ex.Message, EnumLogSource.服务器日志, EnumLevel.Error);
                return false;
            }
        }
        #endregion

        #region  检定业务接口
        /// <summary>
        /// 检定任务信息获取 6.1
        /// </summary>
        private string GetVeriTask(string barcode, string sysNO)
        {
            try
            {
                string url = GetJson("6.1");
                SXDataTableSend obj = new SXDataTableSend();
                obj.barCode = barcode;
                obj.sysNo = sysNO;
                string str = JsonHelper.序列化对象(obj);
                JObject json = JObject.Parse(str);
                string log = json.ToString();
                LogManager.AddMessage(string.Format("调用接口{0}，上报数据{1}", url, log), 1);
                string strrecv = Post(url, str);
                LogManager.AddMessage(string.Format("接口{0}，返回数据{1}", url, strrecv), 1);

                return strrecv;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(ex.Message, EnumLogSource.服务器日志, EnumLevel.Error);
                return "";
            }

        }

        /// <summary>
        /// 6.2
        /// </summary>
        /// <param name="v"></param>
        private string GetEquipDet()
        {
            try
            {
                SendData62 obj = new SendData62();
                obj.taskNo = "1401731313767250";
                obj.sysNo = "997";
                obj.pageNo = 0;
                obj.pageSize = 1000;
                string url = GetJson("6.2");
                string str = JsonHelper.序列化对象(obj);
                JObject json = JObject.Parse(str);
                string log = json.ToString();
                LogManager.AddMessage(string.Format("调用接口{0}，上报数据{1}", url, log), 1);
                string strrecv = Post(url, str);
                LogManager.AddMessage(string.Format("接口{0}，返回数据{1}", url, strrecv), 1);

                return strrecv;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(ex.Message, EnumLogSource.服务器日志, EnumLevel.Error);
                return "";
            }
        }

        /// <summary>
        /// 6.4 检定方案信息获取接口
        /// </summary>
        /// <param name="v"></param>
        /// <param name="meter"></param>
        /// <returns></returns>
        private string GetVeriSchInfo64(string SchId)
        {
            try
            {
                GetVeriSchInfo obj = new GetVeriSchInfo();
                obj.trialSchId = SchId;

                string url = GetJson("6.4");
                string str = JsonHelper.序列化对象(obj);
                JObject json = JObject.Parse(str);
                string log = json.ToString();
                LogManager.AddMessage(string.Format("调用接口{0}，上报数据{1}", url, log), 1);
                string strrecv = Post(url, str);
                LogManager.AddMessage(string.Format("接口{0}，返回数据{1}", url, strrecv), 1);

                return strrecv;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(ex.Message, EnumLogSource.服务器日志, EnumLevel.Error);
                return "";
            }
        }

        /// <summary>
        /// 6.9
        /// </summary>
        /// <param name="meter"></param>
        /// <returns></returns>
        private string GetEquioParam(string type, string data)
        {
            string url = GetJson("6.9");

            SXDataTableSend obj = new SXDataTableSend
            {
                devCls = "09",
                type = type,
                barCodes = data,
                arrBatchNo = "",
                devCodeNo = ""
            };
            string str = JsonHelper.序列化对象(obj);
            JObject json = JObject.Parse(str);
            string log = json.ToString();
            LogManager.AddMessage(string.Format("调用接口{0}，上报数据\r\n{1}", url, log), 1);
            string strrecv = Post(url, str);
            LogManager.AddMessage(string.Format("接口{0}，返回数据\r\n{1}", url, strrecv), 1);
            return strrecv;
        }

        /// <summary>
        /// 6.14
        /// </summary>
        /// <returns></returns>
        public bool UploadSealsCode(TestMeterInfo item)
        {
            string url = GetJson("6.14");

            UpLoadSeal obj = new UpLoadSeal();
            obj.devCls = "09";
            obj.taskNo = item.MD_TaskNo;
            obj.sysNo = item.Other5;
            obj.psOrgNo = "01"; //供电单位编号

            //foreach(TestMeterInfo item in meters)
            //{
            SealInst seal = new SealInst();
            seal.barCode = item.MD_BarCode;
            seal.sealPosition = "01";
            seal.sealBarCode = item.Other4;
            seal.sealDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            seal.sealerNo = item.Checker1;
            seal.validFlag = "01";
            seal.releaseInfo = "";
            obj.sealInst.Add(seal);

            string str = JsonHelper.序列化对象(obj);

            JObject json = JObject.Parse(str);
            string log = json.ToString();
            LogManager.AddMessage(string.Format("调用接口{0}，上报数据{1}", url, log), 1);

            string strrecv = Post(url, str);
            LogManager.AddMessage(string.Format("接口{0}，返回数据{1}", url, strrecv), 1);
            RecvData recv = JsonHelper.反序列化字符串<RecvData>(strrecv);
            if (recv.resultFlag == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 6.18
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="jsonParas"></param>
        /// <returns></returns>

        public bool SendTaskFinish(string taskno)
        {
            SXDataTableSend obj = new SXDataTableSend();
            obj.taskNo = taskno;
            obj.flag = "01";

            string url = GetJson("6.18");
            string str = JsonHelper.序列化对象(obj);
            JObject json = JObject.Parse(str);
            string log = json.ToString();
            LogManager.AddMessage(string.Format("调用接口{0}，上报数据{1}", url, log), 1);
            string strrecv = Post(url, str);
            LogManager.AddMessage(string.Format("接口{0}，返回数据{1}", url, strrecv), 1);
            RecvData recv = JsonHelper.反序列化字符串<RecvData>(strrecv);
            if (recv.resultFlag == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 方法
        public string Post(string Url, string jsonParas)
        {

            string strURL = Url;
            //创建一个HTTP请求  
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
            //Post请求方式  
            request.Method = "POST";
            //内容类型
            request.ContentType = "application/Json";

            //设置参数，并进行URL编码 

            string paraUrlCoded = jsonParas;//System.Web.HttpUtility.UrlEncode(jsonParas);   

            byte[] payload;
            //将Json字符串转化为字节  
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            //设置请求的ContentLength   
            request.ContentLength = payload.Length;
            //发送请求，获得请求流 

            Stream writer;
            try
            {
                writer = request.GetRequestStream();//获取用于写入请求数据的Stream对象
            }
            catch (Exception ex)
            {
                writer = null;
                LogManager.AddMessage("连接服务器失败" + ex.Message, EnumLogSource.服务器日志, EnumLevel.Tip);
                return "连接服务器失败" + ex.Message;
            }
            //将请求参数写入流
            writer.Write(payload, 0, payload.Length);
            writer.Close();//关闭请求流
                           // String strValue = "";//strValue为http响应所返回的字符流
            HttpWebResponse response;
            try
            {
                //获得响应流
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }
            Stream s = response.GetResponseStream();
            //  Stream postData = Request.InputStream;
            StreamReader sRead = new StreamReader(s);
            string postContent = sRead.ReadToEnd();
            sRead.Close();
            return postContent;//返回Json数据
        }



        public class JsonHelper
        {
            public static string 序列化对象(object obj)
            {

                return JsonConvert.SerializeObject(obj);
            }

            public static T 反序列化字符串<T>(string data)
            {
                return JsonConvert.DeserializeObject<T>(data);  // 尖括号<>中填入对象的类名
            }
        }

        public string GetJson(string str)
        {
            string url = "";
            switch (str)
            {
                case "6.1":
                    url = "/restful/sxyk/detectService/getVeriTask";
                    break;
                case "6.2":
                    url = "/restful/sxyk/detectService/getEquipDET";
                    break;
                case "6.4":
                    url = "/restful/sxyk/detectService/getVeriSchInfo";
                    break;
                case "6.9":
                    url = "/restful/common/getEquipParam";
                    break;
                case "6.12":
                    url = "/restful/sxyk/detectService/setResults";
                    break;
                case "6.13":
                    url = "/restful/sxyk/detectService/getDETedTestData";
                    break;
                case "6.14":
                    url = "/restful/sxyk/detectService/uploadSealsCode";
                    break;
                case "6.15":
                    url = "/restful/sxyk/detectService/getLegalCertRslt";
                    break;
                case "6.18":
                    url = "/restful/sxyk/detectService/sendTaskFinish";
                    break;
                case "6.2ZJ":
                    url = "/Restful/gkReciveVeriSchemeInfo";
                    break;
                case "6.3ZJ":
                    url = "/Restful/gkReciveVeriEquipDoc";
                    break;
                case "6.4ZJ":
                    url = "/Restful/gkSendTestResultData";
                    break;
                case "6.5ZJ":
                    url = "/Restful/gkSendTestResultData";
                    break;

            }
            return "http://" + Ip + ":" + Port + url;
        }

        #endregion

        #region 质检业务接口

        /// <summary>
        /// 方案详情接口
        /// </summary>
        /// <param name="schemeNo">方案编号</param>
        public string gkReciveVeriSchemeInfo(string schemeNo)
        {
            try
            {

                JObject joSend = new JObject();
                joSend.Add("schemeNo", schemeNo);//方案编号
                string str = joSend.ToString();
                string url = GetJson("6.2ZJ");
                JObject json = JObject.Parse(str);
                string log = json.ToString();
                LogManager.AddMessage(string.Format("调用接口{0}，上报数据\r\n{1}", url, log), 1);
                string strrecv = Post(url, str);
                LogManager.AddMessage(string.Format("接口{0}，返回数据\r\n{1}", url, strrecv), 1);

                return strrecv;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(ex.Message, EnumLogSource.服务器日志, EnumLevel.Error);
                return "";
            }
        }

        /// <summary>
        /// 资产档案获取接口
        /// </summary>
        /// <param name="schemeNo"></param>
        /// <returns></returns>
        public string gkReciveVeriEquipDoc(string barcode, ref TestMeterInfo meter)
        {
            try
            {
                JObject joSend = new JObject();
                joSend.Add("sysNo", ConfigHelper.Instance.SysNO);
                joSend.Add("taskNo", meter.MD_TaskNo);
                joSend.Add("barCode", barcode);

                string str = joSend.ToString();
                string url = GetJson("6.3ZJ");
                JObject json = JObject.Parse(str);
                string log = json.ToString();
                LogManager.AddMessage(string.Format("调用接口{0}，上报数据\r\n{1}", url, log), 1);
                string strrecv = Post(url, str);
                LogManager.AddMessage(string.Format("接口{0}，返回数据\r\n{1}", url, strrecv), 1);
                return strrecv;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(ex.Message, EnumLogSource.服务器日志, EnumLevel.Error);
                return "";
            }
        }

        /// <summary>
        /// 试验结果上传接口
        /// </summary>
        /// <param name="schemeNo"></param>
        /// <returns></returns>
        public bool gkSendTestResultData(TestMeterInfo meter)
        {
            try
            {
                JObject joSend = new JObject();
                joSend.Add("taskNo", meter.MD_TaskNo);
                JArray jArray = new JArray();

                foreach (var item1 in meter.MeterResoultData)
                {
                    switch (item1.Key)
                    {
                        case ProjectID.外观:
                            外观检查(jArray, meter, item1);
                            break;
                        case ProjectID.终端停_上电事件:
                            停上电事件(jArray, meter, item1);
                            break;
                        case ProjectID.终端密钥恢复:
                            读取终端信息(jArray, meter, item1, "V_TMNL_TMLKEYRCV");
                            break;
                        case ProjectID.读取终端信息:
                            读取终端信息(jArray, meter, item1, "V_TMNL_RDTMLINFO");
                            break;
                        case ProjectID.时钟召测和对时:
                            读取终端信息(jArray, meter, item1, "V_TMNL_CLKTEST");
                            break;
                        case ProjectID.基本参数:
                            读取终端信息(jArray, meter, item1, "V_TMNL_BASPAR");
                            break;
                        case ProjectID.事件参数:
                            读取终端信息(jArray, meter, item1, "V_TMNL_EVTPAR");
                            break;
                        case ProjectID.状态量采集:
                            读取终端信息(jArray, meter, item1, "V_TMNL_STATCOL");
                            break;
                        case ProjectID.实时和当前数据:
                            读取终端信息(jArray, meter, item1, "V_TMNL_TMLCURDT");
                            break;
                        case ProjectID.电能表实时数据:
                            电能表实时数据(jArray, meter, item1, "V_TMNL_EMRTDT");
                            break;
                        case ProjectID.电能表当前数据:
                            读取终端信息(jArray, meter, item1, "V_TMNL_EMCURDT");
                            break;


                        case ProjectID.历史日数据:
                            历史日数据(jArray, meter, item1, "V_TMNL_HISDAYDT");
                            break;
                        case ProjectID.历史月数据:   //--
                            历史日数据(jArray, meter, item1, "V_TMNL_HISMONDT");
                            break;
                        case ProjectID.负荷曲线:     //--
                            读取终端信息(jArray, meter, item1, "V_TMNL_LOADCURVE");
                            break;
                        case ProjectID.终端采集645表计数据:
                            读取终端信息(jArray, meter, item1, "V_TMNL_TML645COL");
                            break;
                        case ProjectID.终端主动上报:
                            读取终端信息(jArray, meter, item1, "V_TMNL_TMLRPT");
                            break;
                        case ProjectID.终端对时事件:
                            读取终端信息(jArray, meter, item1, "V_TMNL_TMLCLKEVT");
                            break;
                        case ProjectID.终端485抄表错误:
                            读取终端信息(jArray, meter, item1, "V_TMNL_TML485ERR");
                            break;
                        case ProjectID.终端编程事件:
                            读取终端信息(jArray, meter, item1, "V_TMNL_TMLPROG");
                            break;

                        case ProjectID.分脉冲量采集12个:  //-
                            分脉冲量采集12个(jArray, meter, item1, "V_TMNL_12PMCOL");
                            break;
                        case ProjectID.分脉冲量采集120个: //-
                            分脉冲量采集12个(jArray, meter, item1, "V_TMNL_120PMCOL");
                            break;
                        case ProjectID.总加组日和月电量召集:
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.分时段电能量数据存储:
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.抄表MAC验证:
                            读取终端信息(jArray, meter, item1, "V_TMNL_MTRMAC");
                            break;
                        case ProjectID.随机交采数据:
                            读取终端信息(jArray, meter, item1, "V_TMNL_RANDTEST");
                            break;
                        case ProjectID.电能表时间超差事件:
                            读取终端信息(jArray, meter, item1, "V_TMNL_EMCLKERR");
                            break;
                        case ProjectID.电能表超差事件:
                            读取终端信息(jArray, meter, item1, "V_TMNL_EMENERGERR");
                            break;
                        case ProjectID.电能表飞走事件: //-
                            读取终端信息(jArray, meter, item1, "V_TMNL_EMFLY");
                            break;
                        case ProjectID.电能表停走事件: //-
                            读取终端信息(jArray, meter, item1, "V_TMNL_EMSTOP");
                            break;
                        case ProjectID.购电参数设置事件:
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.终端相序异常事件:
                            读取终端信息(jArray, meter, item1, "V_TMNL_TMLPHASE");
                            break;
                        case ProjectID.全事件采集上报:
                            读取终端信息(jArray, meter, item1, "V_TMNL_EMALLEVT");
                            break;
                        case ProjectID.终端抄表失败:
                            读取终端信息(jArray, meter, item1, "V_TMNL_TMLMTRFAIL");
                            break;
                        case ProjectID.透明方案:
                            读取终端信息(jArray, meter, item1, "V_TMNL_TRANSPLAN");
                            break;
                        case ProjectID.抄表与费率参数:
                            读取终端信息(jArray, meter, item1, "V_TMNL_MTRPAR");
                            break;
                        case ProjectID.电能表事件补抄:
                            读取终端信息(jArray, meter, item1, "V_TMNL_EMEVTSUPP");
                            break;
                        case ProjectID.负荷曲线补抄:
                            读取终端信息(jArray, meter, item1, "V_TMNL_SUPPLOAD");
                            break;
                        case ProjectID.电压合格率统计:
                            读取终端信息(jArray, meter, item1, "V_TMNL_VOLTQUAL");
                            break;
                        case ProjectID.电源影响:            //
                            item1.Value.meterResoults[0].Datas["项目名"] = "电源影响";
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.历史日数据补抄:
                            读取终端信息(jArray, meter, item1, "V_TMNL_SUPPHISDAY");
                            break;


                        case ProjectID.结算日冻结:
                            读取终端信息(jArray, meter, item1, "V_TMNL_SETTLEDAY");
                            break;
                        case ProjectID.分组抄表:
                            读取终端信息(jArray, meter, item1, "V_TMNL_GRPMTRTEST");
                            break;
                        case ProjectID.电能表示度下降事件:
                            读取终端信息(jArray, meter, item1, "V_TMNL_EMDEC");
                            break;
                        case ProjectID.电能表数据变更监控记录:
                            读取终端信息(jArray, meter, item1, "V_TMNL_EMDATACHG");
                            break;
                        case ProjectID.交采电量清零:
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.终端维护:
                            读取终端信息(jArray, meter, item1, "V_TMNL_TMLMAINT");
                            break;
                        case ProjectID.安全模式:
                            读取终端信息(jArray, meter, item1, "V_TMNL_SECMODE");
                            break;
                        case ProjectID.密钥下装:
                            读取终端信息(jArray, meter, item1, "V_TMNL_TMLKEYUPD");
                            break;
                        case ProjectID.日计时误差:
                            读取终端信息(jArray, meter, item1, "V_TMNL_DAYTIMERR");
                            break;
                        case ProjectID.电流不平衡影响:
                            读取终端信息(jArray, meter, item1, "V_TMNL_ACUNBAL");
                            break;
                        case ProjectID.功率因数越限统计:
                            读取终端信息(jArray, meter, item1, "V_TMNL_PFLIMIT");
                            break;
                        case ProjectID.常温基本误差:
                            item1.Value.meterResoults[0].Datas["项目名"] = "常温基本误差";
                            读取终端信息(jArray, meter, item1, "V_TMNL_TEMPERR");
                            break;
                        case ProjectID.时段功控:
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.厂休功控:
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.月电控:
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.购电控:
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.催费告警:
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.保电功能:
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.剔除功能:
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.遥控功能:
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.营业报停功控:
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.当前功率下浮控:
                            其他项目(jArray, meter, item1);
                            break;
                        case ProjectID.单个测量电设置采集档案:
                            读取终端信息(jArray, meter, item1, "V_TMNL_SINGLEPT");
                            break;
                        case ProjectID.组合参数读取与参数设置:
                            读取终端信息(jArray, meter, item1, "V_TMNL_COMBOPAR");
                            break;
                        case ProjectID.时段控与购电控同时投入:
                            读取终端信息(jArray, meter, item1, "V_TMNL_TIMEBUYCCTRL");
                            break;
                    }

                }

                joSend.Add("itemResultData", jArray);
                string str = joSend.ToString();
                string url = GetJson("6.4ZJ");
                JObject json = JObject.Parse(str);
                string log = json.ToString();
                LogManager.AddMessage(string.Format("调用接口{0}，上报数据\r\n{1}", url, log), 1);
                string strrecv = Post(url, str);
                LogManager.AddMessage(string.Format("接口{0}，返回数据\r\n{1}", url, strrecv), 1);

                RecvData recv = JsonHelper.反序列化字符串<RecvData>(strrecv);
                if (recv.resultFlag == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(ex.Message, EnumLogSource.服务器日志, EnumLevel.Error);
                return false;
            }
        }


        /// <summary>
        /// 结束试验接口
        /// </summary>
        /// <param name="schemeNo"></param>
        /// <returns></returns>
        public string gkSendTaskFinish(string schemeNo)
        {
            try
            {
                JObject joSend = new JObject();
                joSend.Add("taskNo", "任务编号");
                joSend.Add("plantNo", "设备编号");

                string str = joSend.ToString();
                string url = GetJson("6.5ZJ");
                JObject json = JObject.Parse(str);
                string log = json.ToString();
                LogManager.AddMessage(string.Format("调用接口{0}，上报数据\r\n{1}", url, log), 1);
                string strrecv = Post(url, str);
                LogManager.AddMessage(string.Format("接口{0}，返回数据\r\n{1}", url, strrecv), 1);

                return strrecv;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(ex.Message, EnumLogSource.服务器日志, EnumLevel.Error);
                return "";
            }
        }
        #endregion

        #region 质检分项结论接口


        public void 外观检查(JArray jArray, TestMeterInfo meter, KeyValuePair<string, Core.Model.DnbModel.DnbInfo.MeterResoultData> item1)
        {

            JArray jaValues = new JArray();
            JObject jObjectC = new JObject();
            jObjectC.Add("itemCode", GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]);
            jObjectC.Add("itemResult", item1.Value.Result == "合格" ? "02" : "01");
            jObjectC.Add("barCode", meter.MD_BarCode);
            jObjectC.Add("detectPerson", meter.Checker1);
            jObjectC.Add("plantNo", "1");
            jObjectC.Add("machNo", "1");
            jObjectC.Add("environTemp", meter.Temperature);
            jObjectC.Add("environHum", meter.Humidity);

            JObject jobjestcc = new JObject();
            jobjestcc.Add("veriPointSN", 1);
            jobjestcc.Add("tableName", "V_TMNL_STRUCT");
            JArray fieldValue = new JArray();
            JObject jobject = new JObject();
            jobject.Add(ToCamelCaseRegex("BAR_CODE"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("SAMPLE_NO"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("DEV_SEAT_NO"), meter.MD_Epitope);
            jobject.Add(ToCamelCaseRegex("TEST_CATEGORIES"), GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]);
            jobject.Add(ToCamelCaseRegex("VERI_POINT_NO"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_POINT_SN"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_ORG_NO"), "1");
            jobject.Add(ToCamelCaseRegex("TEST_REQUIRE"), "");
            jobject.Add(ToCamelCaseRegex("TEST_CONDITION"), "");
            jobject.Add(ToCamelCaseRegex("VERI_DATE"), meter.VerifyDate);

            jobject.Add(ToCamelCaseRegex("VERI_RSLT"), item1.Value.Result == "合格" ? "02" : "01");
            jobject.Add(ToCamelCaseRegex("APPEARANCE_SIZE"),"");
            jobject.Add(ToCamelCaseRegex("PARAM"), "");
            jobject.Add(ToCamelCaseRegex("COLLECT_ITEM"), "");
            jobject.Add(ToCamelCaseRegex("COLLECT_RESULT"),"");
            jobject.Add(ToCamelCaseRegex("TEST_RESULT"), "");
            jobject.Add(ToCamelCaseRegex("REMARK"), "");
            fieldValue.Add(jobject);
            jobjestcc.Add("fieldValue", fieldValue);
            jaValues.Add(jobjestcc);
            jObjectC.Add("testResultData", jaValues);
            jArray.Add(jObjectC);

        }


        public void 终端密钥恢复(JArray jArray, TestMeterInfo meter, KeyValuePair<string, Core.Model.DnbModel.DnbInfo.MeterResoultData> item1)
        {

            JArray jaValues = new JArray();

            JObject jObjectC = new JObject();
            jObjectC.Add("itemCode", GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]);
            jObjectC.Add("itemResult", item1.Value.Result);
            jObjectC.Add("barCode", meter.MD_BarCode);
            jObjectC.Add("detectPerson", meter.Checker1);
            jObjectC.Add("plantNo", "1");
            jObjectC.Add("machNo", "1");
            jObjectC.Add("environTemp", meter.Temperature);
            jObjectC.Add("environHum", meter.Humidity);


            JObject jobjestcc = new JObject();
            jobjestcc.Add("veriPointSN", 1);
            jobjestcc.Add("tableName", "V_TMNL_TMLKEYRCV");
            JArray fieldValue = new JArray();
            JObject jobject = new JObject();
            jobject.Add(ToCamelCaseRegex("RSLT_ID"), meter.MD_BarCode); //分项结论标识
            jobject.Add(ToCamelCaseRegex("TMNL_COMP_CONC_ID"), meter.MD_BarCode);

            jobject.Add(ToCamelCaseRegex("TASK_ID"), "0");
            jobject.Add(ToCamelCaseRegex("TASK_NO"), "0");
            jobject.Add(ToCamelCaseRegex("DEV_ID"), "09");
            jobject.Add(ToCamelCaseRegex("BAR_CODE"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("SAMPLE_NO"), "09");
            jobject.Add(ToCamelCaseRegex("BEFORE_TEST_STAT"), "02");
            jobject.Add(ToCamelCaseRegex("AFTER_TEST_STAT"), "02");
            jobject.Add(ToCamelCaseRegex("PLANT_ID"), "");
            jobject.Add(ToCamelCaseRegex("PLANT_ELEMENT_ID"), "0");
            jobject.Add(ToCamelCaseRegex("PLANT_MACH_ID"), "0");
            jobject.Add(ToCamelCaseRegex("PLANT_NO"), "0");
            jobject.Add(ToCamelCaseRegex("MACH_NO"), "0");
            jobject.Add(ToCamelCaseRegex("DEV_SEAT_NO"), meter.MD_Epitope);
            jobject.Add(ToCamelCaseRegex("V_TMNL_RDTMLINFO"), GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]);
            jobject.Add(ToCamelCaseRegex("VERI_POINT_NO"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_POINT_SN"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_ORG_NO"), "0");
            jobject.Add(ToCamelCaseRegex("ENVIRON_TEMPER"), meter.Temperature);
            jobject.Add(ToCamelCaseRegex("RELATIVE_HUM"), meter.Humidity);
            jobject.Add(ToCamelCaseRegex("TEST_REQUIRE"), "0");
            jobject.Add(ToCamelCaseRegex("TEST_CONDITION"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_DATE"), meter.VerifyDate);
            jobject.Add(ToCamelCaseRegex("VERI_RSLT"), item1.Value.Result == "合格" ? "02" : "01");
            //jobject.Add(ToCamelCaseRegex("CREATE_TIME"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            jobject.Add(ToCamelCaseRegex("VALID_FLAG"), "01");

            jobject.Add(ToCamelCaseRegex("MEASURE_VALUE"), item1.Value.Result == "合格" ? "02" : "01");
            jobject.Add(ToCamelCaseRegex("REMARK"), "");
            fieldValue.Add(jobject);
            jobjestcc.Add("fieldValue", fieldValue);
            jaValues.Add(jobjestcc);
            jObjectC.Add("testResultData", jaValues);
            jArray.Add(jObjectC);
        }
        public void 读取终端信息(JArray jArray, TestMeterInfo meter, KeyValuePair<string, Core.Model.DnbModel.DnbInfo.MeterResoultData> item1,string str)
        {

            JArray jaValues = new JArray();

            JObject jObjectC = new JObject();
            jObjectC.Add("itemCode", GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]);
            jObjectC.Add("itemResult", item1.Value.Result == "合格" ? "02" : "01");
            jObjectC.Add("barCode", meter.MD_BarCode);
            jObjectC.Add("detectPerson", meter.Checker1);
            jObjectC.Add("plantNo", "1");
            jObjectC.Add("machNo", "1");
            jObjectC.Add("environTemp", meter.Temperature);
            jObjectC.Add("environHum", meter.Humidity);

            JObject jobjestcc = new JObject();
            jobjestcc.Add("veriPointSN", 1);
            jobjestcc.Add("tableName", str );
            JArray fieldValue = new JArray();
            JObject jobject = new JObject();
            jobject.Add(ToCamelCaseRegex("RSLT_ID"), meter.MD_BarCode); //分项结论标识
            jobject.Add(ToCamelCaseRegex("TMNL_COMP_CONC_ID"), "");

            jobject.Add(ToCamelCaseRegex("TASK_ID"), "0");
            jobject.Add(ToCamelCaseRegex("TASK_NO"), "0");
            jobject.Add(ToCamelCaseRegex("DEV_ID"), "09");
            jobject.Add(ToCamelCaseRegex("BAR_CODE"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("SAMPLE_NO"), "09");
            jobject.Add(ToCamelCaseRegex("BEFORE_TEST_STAT"), "02");
            jobject.Add(ToCamelCaseRegex("AFTER_TEST_STAT"), "02");
            jobject.Add(ToCamelCaseRegex("PLANT_ID"), "");
            jobject.Add(ToCamelCaseRegex("PLANT_ELEMENT_ID"), "0");
            jobject.Add(ToCamelCaseRegex("PLANT_MACH_ID"), "0");
            jobject.Add(ToCamelCaseRegex("PLANT_NO"), "0");
            jobject.Add(ToCamelCaseRegex("MACH_NO"), "0");
            jobject.Add(ToCamelCaseRegex("DEV_SEAT_NO"), meter.MD_Epitope);
            jobject.Add(ToCamelCaseRegex("TEST_CATEGORIES"), GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]); //
            jobject.Add(ToCamelCaseRegex("VERI_POINT_NO"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_POINT_SN"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_ORG_NO"), "0");
            jobject.Add(ToCamelCaseRegex("ENVIRON_TEMPER"), meter.Temperature);
            jobject.Add(ToCamelCaseRegex("RELATIVE_HUM"), meter.Humidity);
            jobject.Add(ToCamelCaseRegex("TEST_REQUIRE"), "0");
            jobject.Add(ToCamelCaseRegex("TEST_CONDITION"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_DATE"), meter.VerifyDate);
            jobject.Add(ToCamelCaseRegex("VERI_RSLT"), item1.Value.Result == "合格" ? "02" : "01");
            //jobject.Add(ToCamelCaseRegex("CREATE_TIME"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            jobject.Add(ToCamelCaseRegex("VALID_FLAG"), "01");

            jobject.Add(ToCamelCaseRegex("MEASURE_VALUE"), item1.Value.Result == "合格" ? "02" : "01");
            jobject.Add(ToCamelCaseRegex("REMARK"), "");

            fieldValue.Add(jobject);
            jobjestcc.Add("fieldValue", fieldValue);

            jaValues.Add(jobjestcc);
            jObjectC.Add("testResultData", jaValues);
            jArray.Add(jObjectC);
        }


        public void 历史日数据(JArray jArray, TestMeterInfo meter, KeyValuePair<string, Core.Model.DnbModel.DnbInfo.MeterResoultData> item1, string str)
        {

            JArray jaValues = new JArray();

            JObject jObjectC = new JObject();
            jObjectC.Add("itemCode", GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]);
            jObjectC.Add("itemResult", item1.Value.Result == "合格" ? "02" : "01");
            jObjectC.Add("barCode", meter.MD_BarCode);
            jObjectC.Add("detectPerson", meter.Checker1);
            jObjectC.Add("plantNo", "1");
            jObjectC.Add("machNo", "1");
            jObjectC.Add("environTemp", meter.Temperature);
            jObjectC.Add("environHum", meter.Humidity);

            JObject jobjestcc = new JObject();
            jobjestcc.Add("veriPointSN", 1);
            jobjestcc.Add("tableName", str);
            JArray fieldValue = new JArray();
            JObject jobject = new JObject();
            jobject.Add(ToCamelCaseRegex("RSLT_ID"), meter.MD_BarCode); //分项结论标识
            jobject.Add(ToCamelCaseRegex("TMNL_COMP_CONC_ID"), meter.MD_BarCode);

            jobject.Add(ToCamelCaseRegex("TASK_ID"), "0");
            jobject.Add(ToCamelCaseRegex("TASK_NO"), "0");
            jobject.Add(ToCamelCaseRegex("DEV_ID"), "09");
            jobject.Add(ToCamelCaseRegex("BAR_CODE"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("SAMPLE_NO"), "09");
            jobject.Add(ToCamelCaseRegex("BEFORE_TEST_STAT"), "02");
            jobject.Add(ToCamelCaseRegex("AFTER_TEST_STAT"), "02");
            jobject.Add(ToCamelCaseRegex("PLANT_ID"), "");
            jobject.Add(ToCamelCaseRegex("PLANT_ELEMENT_ID"), "0");
            jobject.Add(ToCamelCaseRegex("PLANT_MACH_ID"), "0");
            jobject.Add(ToCamelCaseRegex("PLANT_NO"), "0");
            jobject.Add(ToCamelCaseRegex("MACH_NO"), "0");
            jobject.Add(ToCamelCaseRegex("DEV_SEAT_NO"), meter.MD_Epitope);
            jobject.Add(ToCamelCaseRegex("TEST_CATEGORIES"), GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]); //
            jobject.Add(ToCamelCaseRegex("VERI_POINT_NO"), "0"); //
            jobject.Add(ToCamelCaseRegex("VERI_POINT_SN"), "0"); //
            jobject.Add(ToCamelCaseRegex("VERI_ORG_NO"), "0");
            jobject.Add(ToCamelCaseRegex("ENVIRON_TEMPER"), meter.Temperature);
            jobject.Add(ToCamelCaseRegex("RELATIVE_HUM"), meter.Humidity);
            jobject.Add(ToCamelCaseRegex("TEST_REQUIRE"), "0");
            jobject.Add(ToCamelCaseRegex("TEST_CONDITION"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_DATE"), meter.VerifyDate);
            jobject.Add(ToCamelCaseRegex("VERI_RSLT"), item1.Value.Result == "合格" ? "02" : "01");
            //jobject.Add(ToCamelCaseRegex("CREATE_TIME"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            jobject.Add(ToCamelCaseRegex("VALID_FLAG"), "01");

            jobject.Add(ToCamelCaseRegex("MEASURE_VALUE"), item1.Value.Result == "合格" ? "02" : "01");
            jobject.Add(ToCamelCaseRegex("REMARK"), "");

            fieldValue.Add(jobject);
            jobjestcc.Add("fieldValue", fieldValue);

            jaValues.Add(jobjestcc);
            jObjectC.Add("testResultData", jaValues);
            jArray.Add(jObjectC);
        }
        public void 分脉冲量采集12个(JArray jArray, TestMeterInfo meter, KeyValuePair<string, Core.Model.DnbModel.DnbInfo.MeterResoultData> item1, string str)
        {

            JArray jaValues = new JArray();

            JObject jObjectC = new JObject();
            jObjectC.Add("itemCode", GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]);
            jObjectC.Add("itemResult", item1.Value.Result == "合格" ? "02" : "01");
            jObjectC.Add("barCode", meter.MD_BarCode);
            jObjectC.Add("detectPerson", meter.Checker1);
            jObjectC.Add("plantNo", "1");
            jObjectC.Add("machNo", "1");
            jObjectC.Add("environTemp", meter.Temperature);
            jObjectC.Add("environHum", meter.Humidity);


            JObject jobjestcc = new JObject();
            jobjestcc.Add("veriPointSN", 1);
            jobjestcc.Add("tableName", str);
            JArray fieldValue = new JArray();
            JObject jobject = new JObject();
            jobject.Add(ToCamelCaseRegex("RSLT_ID"), ""); //分项结论标识
            jobject.Add(ToCamelCaseRegex("TMNL_COMP_CONC_ID"), "");

            jobject.Add(ToCamelCaseRegex("TASK_ID"), "0");
            jobject.Add(ToCamelCaseRegex("TASK_NO"), "0");
            jobject.Add(ToCamelCaseRegex("DEV_ID"), "09");
            jobject.Add(ToCamelCaseRegex("BAR_CODE"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("SAMPLE_NO"), "09");
            jobject.Add(ToCamelCaseRegex("BEFORE_TEST_STAT"), "02");
            jobject.Add(ToCamelCaseRegex("AFTER_TEST_STAT"), "02");
            jobject.Add(ToCamelCaseRegex("PLANT_ID"), "");
            jobject.Add(ToCamelCaseRegex("PLANT_ELEMENT_ID"), "0");
            jobject.Add(ToCamelCaseRegex("PLANT_MACH_ID"), "0");
            jobject.Add(ToCamelCaseRegex("PLANT_NO"), "0");
            jobject.Add(ToCamelCaseRegex("MACH_NO"), "0");
            jobject.Add(ToCamelCaseRegex("DEV_SEAT_NO"), meter.MD_Epitope);
            jobject.Add(ToCamelCaseRegex("TEST_CATEGORIES"), GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]); //
            jobject.Add(ToCamelCaseRegex("VERI_POINT_NO"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_POINT_SN"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_ORG_NO"), "0");
            jobject.Add(ToCamelCaseRegex("ENVIRON_TEMPER"), meter.Temperature);
            jobject.Add(ToCamelCaseRegex("RELATIVE_HUM"), meter.Humidity);
            jobject.Add(ToCamelCaseRegex("TEST_REQUIRE"), "0");
            jobject.Add(ToCamelCaseRegex("TEST_CONDITION"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_DATE"), meter.VerifyDate);
            jobject.Add(ToCamelCaseRegex("VERI_RSLT"), item1.Value.Result == "合格" ? "02" : "01");
            jobject.Add(ToCamelCaseRegex("VALID_FLAG"), "01");
            jobject.Add(ToCamelCaseRegex("ERR_ABS"), "01");
            jobject.Add(ToCamelCaseRegex("TEST_TIME"), "");
            jobject.Add(ToCamelCaseRegex("STAND_ACT_POWER"), item1.Value.meterResoults[0].ItemDatas[5].StandardData);
            jobject.Add(ToCamelCaseRegex("TEST_ACT_POWER"), item1.Value.meterResoults[0].ItemDatas[5].TerminalData);
            jobject.Add(ToCamelCaseRegex("MEASURE_VALUE"), item1.Value.Result == "合格" ? "02" : "01");
            jobject.Add(ToCamelCaseRegex("REMARK"), "");

            fieldValue.Add(jobject);
            jobjestcc.Add("fieldValue", fieldValue);
            jaValues.Add(jobjestcc);
            jObjectC.Add("testResultData", jaValues);
            jArray.Add(jObjectC);
        }

        public void 电能表实时数据(JArray jArray, TestMeterInfo meter, KeyValuePair<string, Core.Model.DnbModel.DnbInfo.MeterResoultData> item1,string str)
        {

            JArray jaValues = new JArray();
            JObject jObjectC = new JObject();
            jObjectC.Add("itemCode", GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]);
            jObjectC.Add("itemResult", item1.Value.Result);
            jObjectC.Add("barCode", meter.MD_BarCode);
            jObjectC.Add("detectPerson", meter.Checker1);
            jObjectC.Add("plantNo", ConfigHelper.Instance.PlantNO);
            jObjectC.Add("machNo", "0");
            jObjectC.Add("environTemp", meter.Temperature);
            jObjectC.Add("environHum", meter.Humidity);

            JObject jobjestcc = new JObject();
            jobjestcc.Add("veriPointSN", 1);
            jobjestcc.Add("tableName", str);
            JArray fieldValue = new JArray();
            JObject jobject = new JObject();
            jobject.Add(ToCamelCaseRegex("RSLT_ID"), meter.MD_BarCode); //分项结论标识
            jobject.Add(ToCamelCaseRegex("TMNL_COMP_CONC_ID"), meter.MD_BarCode);

            jobject.Add(ToCamelCaseRegex("TASK_ID"), "0");
            jobject.Add(ToCamelCaseRegex("TASK_NO"), "0");
            jobject.Add(ToCamelCaseRegex("DEV_ID"), "09");
            jobject.Add(ToCamelCaseRegex("BAR_CODE"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("SAMPLE_NO"), "09");
            jobject.Add(ToCamelCaseRegex("BEFORE_TEST_STAT"), "02");
            jobject.Add(ToCamelCaseRegex("AFTER_TEST_STAT"), "02");
            jobject.Add(ToCamelCaseRegex("PLANT_ID"), "");
            jobject.Add(ToCamelCaseRegex("PLANT_ELEMENT_ID"), "0");
            jobject.Add(ToCamelCaseRegex("PLANT_MACH_ID"), "0");
            jobject.Add(ToCamelCaseRegex("PLANT_NO"), "0");
            jobject.Add(ToCamelCaseRegex("MACH_NO"), "0");
            jobject.Add(ToCamelCaseRegex("DEV_SEAT_NO"), meter.MD_Epitope);
            jobject.Add(ToCamelCaseRegex("TEST_CATEGORIES"), GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]); //
            jobject.Add(ToCamelCaseRegex("VERI_POINT_NO"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_POINT_SN"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_ORG_NO"), "0");
            jobject.Add(ToCamelCaseRegex("ENVIRON_TEMPER"), meter.Temperature);
            jobject.Add(ToCamelCaseRegex("RELATIVE_HUM"), meter.Humidity);
            jobject.Add(ToCamelCaseRegex("TEST_REQUIRE"), "0");
            jobject.Add(ToCamelCaseRegex("TEST_CONDITION"), "0");
            jobject.Add(ToCamelCaseRegex("VERI_DATE"), meter.VerifyDate);
            jobject.Add(ToCamelCaseRegex("VERI_RSLT"), item1.Value.Result == "合格" ? "02" : "01");
            //jobject.Add(ToCamelCaseRegex("CREATE_TIME"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            jobject.Add(ToCamelCaseRegex("VALID_FLAG"), "01");
            jobject.Add(ToCamelCaseRegex("FORWARD_ACT_POWER"), item1.Value.meterResoults[0].ItemDatas[4].TerminalData);
            jobject.Add(ToCamelCaseRegex("REVERSE_ACT_POWER"), item1.Value.meterResoults[0].ItemDatas[5].TerminalData);
            jobject.Add(ToCamelCaseRegex("GROUP_REAC_POWER1"), item1.Value.meterResoults[0].ItemDatas[6].TerminalData);
            jobject.Add(ToCamelCaseRegex("GROUP_REAC_POWER2"), item1.Value.meterResoults[0].ItemDatas[7].TerminalData);
            jobject.Add(ToCamelCaseRegex("FORW_ACT_MAX_DEMAND"), item1.Value.meterResoults[0].ItemDatas[8].TerminalData);
            jobject.Add(ToCamelCaseRegex("VOLT_VALUE"), item1.Value.meterResoults[0].ItemDatas[9].TerminalData);
            jobject.Add(ToCamelCaseRegex("CUT_VALUE"), item1.Value.meterResoults[0].ItemDatas[10].TerminalData);
            jobject.Add(ToCamelCaseRegex("TER_DATE_TIME"), item1.Value.meterResoults[0].ItemDatas[11].TerminalData);
            jobject.Add(ToCamelCaseRegex("ERR_ABS"), "");
            jobject.Add(ToCamelCaseRegex("MEASURE_VALUE"), item1.Value.Result == "合格" ? "02" : "01");
            jobject.Add(ToCamelCaseRegex("REMARK"), "");
            fieldValue.Add(jobject);
            jobjestcc.Add("fieldValue", fieldValue);
            jaValues.Add(jobjestcc);
            jObjectC.Add("testResultData", jaValues);
            jArray.Add(jObjectC);
        }

        /// <summary>
        /// 终端应支持交采当前数据、
        /// 终端应可采集电能表实时数据、
        /// 终端应可采集电能表分钟冻结数据
        /// </summary>
        /// <param name="jArray"></param>
        /// <param name="meter"></param>
        public void 功能试验交采(JArray jArray, TestMeterInfo meter, KeyValuePair<string, Core.Model.DnbModel.DnbInfo.MeterResoultData> item1)
        {

            JArray jaValues = new JArray();
            JObject jObjectC = new JObject();
            jObjectC.Add("itemCode", item1.Key.ToString());
            jObjectC.Add("itemResult", item1.Value.Result);
            jObjectC.Add("barCode", meter.MD_BarCode);
            jObjectC.Add("detectPerson", meter.Checker1);
            jObjectC.Add("plantNo", "1");
            jObjectC.Add("machNo", "1");
            jObjectC.Add("environTemp", meter.Temperature);
            jObjectC.Add("environHum", meter.Humidity);

            JObject jobjestcc = new JObject();
            jobjestcc.Add("veriPointSN", 1);
            jobjestcc.Add("tableName", "V_TMNL_FUNC_SIMCOLL");
            JArray fieldValue = new JArray();
            JObject jobject = new JObject();
            jobject.Add(ToCamelCaseRegex("PARAM"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("EQ"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("ELEC_RATE"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("COLLECT_ITEM"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("COLLECT_RESULT"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("VERI_RSLT"), item1.Value.Result == "合格" ? "02" : "01");
            fieldValue.Add(jobject);
            jobjestcc.Add("fieldValue", fieldValue);
            jaValues.Add(jobjestcc);
            jObjectC.Add("testResultData", jaValues);
            jArray.Add(jObjectC);
        }

        /// <summary>
        /// 常温交流电压测量基本误差极限、
        /// 常温交流电流测量基本误差极限、
        /// 常温功率因数测量误差极限、
        /// 常温有功功率测量基本误差极限、
        /// 常温无功功率测量基本误差极限
        /// </summary>
        /// <param name="jArray"></param>
        /// <param name="meter"></param>
        public void 交流模拟量采集(JArray jArray, TestMeterInfo meter)
        {
            foreach (var item1 in meter.MeterResoultData)
            {
                JArray jaValues = new JArray();
                JObject jObjectC = new JObject();
                jObjectC.Add("itemCode", item1.Key.ToString());
                jObjectC.Add("itemResult", item1.Value.Result);
                jObjectC.Add("barCode", meter.MD_BarCode);
                jObjectC.Add("detectPerson", meter.Checker1);
                jObjectC.Add("plantNo", "1");
                jObjectC.Add("machNo", "1");
                jObjectC.Add("environTemp", meter.Temperature);
                jObjectC.Add("environHum", meter.Humidity);

                JObject jobjestcc = new JObject();
                jobjestcc.Add("veriPointSN", 1);
                jobjestcc.Add("tableName", "V_TMNL_FUNC_SIMCOLL");
                JArray fieldValue = new JArray();
                JObject jobject = new JObject();

                jobject.Add(ToCamelCaseRegex("BAR_CODE"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("SAMPLE_NO"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("PARAM"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("ANGLE"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("PF"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("INPUT_STD_VOLT"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("INPUT_STD_CUR"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("ANGLE_ERROER"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("ALLOW_ERR"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("VOLT_MAX_ERR_A"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("VOLT_MAX_ERR_B"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("VOLT_MAX_ERR_C"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("VERI_RSLT"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("REMARK"), meter.MD_BarCode);
                fieldValue.Add(jobject);
                jobjestcc.Add("fieldValue", fieldValue);
                jaValues.Add(jobjestcc);
                jObjectC.Add("testResultData", jaValues);
                jArray.Add(jObjectC);
            }
        }

        /// <summary>
        /// 频率变化影响
        /// 谐波影响、
        /// 超量限值影响、
        /// 不平衡影响
        /// </summary>
        /// <param name="jArray"></param>
        /// <param name="meter"></param>
        public void 影响量改变(JArray jArray, TestMeterInfo meter)
        {
            foreach (var item1 in meter.MeterResoultData)
            {
                JArray jaValues = new JArray();
                JObject jObjectC = new JObject();
                jObjectC.Add("itemCode", item1.Key.ToString());
                jObjectC.Add("itemResult", item1.Value.Result);
                jObjectC.Add("barCode", meter.MD_BarCode);
                jObjectC.Add("detectPerson", meter.Checker1);
                jObjectC.Add("plantNo", "1");
                jObjectC.Add("machNo", "1");
                jObjectC.Add("environTemp", meter.Temperature);
                jObjectC.Add("environHum", meter.Humidity);

                JObject jobjestcc = new JObject();
                jobjestcc.Add("veriPointSN", 1);
                jobjestcc.Add("tableName", "V_TMNL_FUNC_FREQCHG");
                JArray fieldValue = new JArray();
                JObject jobject = new JObject();

                jobject.Add(ToCamelCaseRegex("BAR_CODE"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("SAMPLE_NO"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("PARAM"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("PARAM_VAL"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("INPUT_STD"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("ALLOW_ERR"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("VOLT_CHG"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("CUR_CHG"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("AP_CHG"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("RP_CHG"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("VOLT_ERR"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("CUR_ERR"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("AP_ERR"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("RP_ERR"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("VERI_RSLT"), meter.MD_BarCode);
                fieldValue.Add(jobject);
                jobjestcc.Add("fieldValue", fieldValue);
                jaValues.Add(jobjestcc);
                jObjectC.Add("testResultData", jaValues);
                jArray.Add(jObjectC);
            }
        }

        public void 电能表数据采集(JArray jArray, TestMeterInfo meter)
        {
            foreach (var item1 in meter.MeterResoultData)
            {
                JArray jaValues = new JArray();
                JObject jObjectC = new JObject();
                jObjectC.Add("itemCode", item1.Key.ToString());
                jObjectC.Add("itemResult", item1.Value.Result);
                jObjectC.Add("barCode", meter.MD_BarCode);
                jObjectC.Add("detectPerson", meter.Checker1);
                jObjectC.Add("plantNo", "1");
                jObjectC.Add("machNo", "1");
                jObjectC.Add("environTemp", meter.Temperature);
                jObjectC.Add("environHum", meter.Humidity);

                JObject jobjestcc = new JObject();
                jobjestcc.Add("veriPointSN", 1);
                jobjestcc.Add("tableName", "V_TMNL_FUNC_MTCOLL");
                JArray fieldValue = new JArray();
                JObject jobject = new JObject();

                jobject.Add(ToCamelCaseRegex("BAR_CODE"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("SAMPLE_NO"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("EQ"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("ELEC_RATE"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("PARAM"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("TECHNICAL_REQUIRE"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("COLLECT_ITEM"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("COLLECT_RESULT"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("TEST_RESULT"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("VERI_RSLT"), meter.MD_BarCode);
                fieldValue.Add(jobject);
                jobjestcc.Add("fieldValue", fieldValue);
                jaValues.Add(jobjestcc);
                jObjectC.Add("testResultData", jaValues);
                jArray.Add(jObjectC);
            }
        }

        /// <summary>
        /// 常数设置与查询、\
        /// 限值参数设置与查询、
        /// 功控时段和定时设置与查询、
        /// 预付费控制参数设置与查询、
        /// 终端参数设置与查询、
        /// 抄表参数
        /// </summary>
        /// <param name="jArray"></param>
        /// <param name="meter"></param>
        public void 参数设置与查询(JArray jArray, TestMeterInfo meter)
        {
            foreach (var item1 in meter.MeterResoultData)
            {
                JArray jaValues = new JArray();
                JObject jObjectC = new JObject();
                jObjectC.Add("itemCode", item1.Key.ToString());
                jObjectC.Add("itemResult", item1.Value.Result);
                jObjectC.Add("barCode", meter.MD_BarCode);
                jObjectC.Add("detectPerson", meter.Checker1);
                jObjectC.Add("plantNo", "1");
                jObjectC.Add("machNo", "1");
                jObjectC.Add("environTemp", meter.Temperature);
                jObjectC.Add("environHum", meter.Humidity);

                JObject jobjestcc = new JObject();
                jobjestcc.Add("veriPointSN", 1);
                jobjestcc.Add("tableName", "V_TMNL_FUNC_CONST");
                JArray fieldValue = new JArray();
                JObject jobject = new JObject();

                jobject.Add(ToCamelCaseRegex("BAR_CODE"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("SAMPLE_NO"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("PARAM_ONE"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("PARAM_TWO"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("TECHNICAL_REQUIRE"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("SET_PARAM"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("SET_PARAM_VAL"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("GET_PARAM"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("GET_PARAM_VAL"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("TEST_RESULT"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("VERI_RSLT"), meter.MD_BarCode);
                fieldValue.Add(jobject);
                jobjestcc.Add("fieldValue", fieldValue);
                jaValues.Add(jobjestcc);
                jObjectC.Add("testResultData", jaValues);
                jArray.Add(jObjectC);
            }
        }

        /// <summary>
        /// 终端数据冻结、
        /// 抄表数据冻结、
        /// 事件记录数据冻结
        /// </summary>
        /// <param name="jArray"></param>
        /// <param name="meter"></param>
        public void 数据冻结(JArray jArray, TestMeterInfo meter)
        {
            foreach (var item1 in meter.MeterResoultData)
            {
                JArray jaValues = new JArray();
                JObject jObjectC = new JObject();
                jObjectC.Add("itemCode", item1.Key.ToString());
                jObjectC.Add("itemResult", item1.Value.Result);
                jObjectC.Add("barCode", meter.MD_BarCode);
                jObjectC.Add("detectPerson", meter.Checker1);
                jObjectC.Add("plantNo", "1");
                jObjectC.Add("machNo", "1");
                jObjectC.Add("environTemp", meter.Temperature);
                jObjectC.Add("environHum", meter.Humidity);

                JObject jobjestcc = new JObject();
                jobjestcc.Add("veriPointSN", 1);
                jobjestcc.Add("tableName", "V_TMNL_FUNC_FREEZE");
                JArray fieldValue = new JArray();
                JObject jobject = new JObject();

                jobject.Add(ToCamelCaseRegex("BAR_CODE"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("SAMPLE_NO"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("PARAM"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("TECHNICAL_REQUIRE"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("COLLECT_ITEM"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("COLLECT_RESULT"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("TEST_RESULT"), meter.MD_BarCode);
                jobject.Add(ToCamelCaseRegex("VERI_RSLT"), meter.MD_BarCode);
                fieldValue.Add(jobject);
                jobjestcc.Add("fieldValue", fieldValue);
                jaValues.Add(jobjestcc);
                jObjectC.Add("testResultData", jaValues);
                jArray.Add(jObjectC);
            }
        }

        private void 事件记录(JArray jArray, TestMeterInfo meter, KeyValuePair<string, Core.Model.DnbModel.DnbInfo.MeterResoultData> item1)
        {

            JArray jaValues = new JArray();
            JObject jObjectC = new JObject();
            jObjectC.Add("itemCode", GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]);
            jObjectC.Add("itemResult", item1.Value.Result);
            jObjectC.Add("barCode", meter.MD_BarCode);
            jObjectC.Add("detectPerson", meter.Checker1);
            jObjectC.Add("plantNo", "1");
            jObjectC.Add("machNo", "1");
            jObjectC.Add("environTemp", meter.Temperature);
            jObjectC.Add("environHum", meter.Humidity);

            JObject jobjestcc = new JObject();
            jobjestcc.Add("veriPointSN", 1);
            jobjestcc.Add("tableName", "V_TMNL_FUNC_EVENT_RECORD");
            JArray fieldValue = new JArray();
            JObject jobject = new JObject();

            jobject.Add(ToCamelCaseRegex("BAR_CODE"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("SAMPLE_NO"), "");
            jobject.Add(ToCamelCaseRegex("PARAM"), "");
            jobject.Add(ToCamelCaseRegex("TECHNICAL_REQUIRE"), "");
            jobject.Add(ToCamelCaseRegex("COLLECT_ITEM"), meter.MD_BarCode);
            string COLLECT_RESULT = "";
            string EXTRACT_EVENT = "";
            foreach (var itemss in item1.Value.meterResoults[0].ItemDatas)
            {
                COLLECT_RESULT += itemss.Name + '|';
                EXTRACT_EVENT += itemss.TerminalData + '|';
            }
            jobject.Add(ToCamelCaseRegex("COLLECT_RESULT"), COLLECT_RESULT);
            jobject.Add(ToCamelCaseRegex("EXTRACT_EVENT"), EXTRACT_EVENT);
            jobject.Add(ToCamelCaseRegex("VERI_RSLT"), meter.Result);
            fieldValue.Add(jobject);
            jobjestcc.Add("fieldValue", fieldValue);
            jaValues.Add(jobjestcc);
            jObjectC.Add("testResultData", jaValues);
            jArray.Add(jObjectC);


        }

        private void 停上电事件(JArray jArray, TestMeterInfo meter, KeyValuePair<string, Core.Model.DnbModel.DnbInfo.MeterResoultData> item1)
        {

            JArray jaValues = new JArray();
            JObject jObjectC = new JObject();
            jObjectC.Add("itemCode", GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]);
            jObjectC.Add("itemResult", item1.Value.Result);
            jObjectC.Add("barCode", meter.MD_BarCode);
            jObjectC.Add("detectPerson", meter.Checker1);
            jObjectC.Add("plantNo", "1");
            jObjectC.Add("machNo", "1");
            jObjectC.Add("environTemp", meter.Temperature);
            jObjectC.Add("environHum", meter.Humidity);

            JObject jobjestcc = new JObject();
            jobjestcc.Add("veriPointSN", 1);
            jobjestcc.Add("tableName", "V_TMNL_PWRONOFF");
            JArray fieldValue = new JArray();
            JObject jobject = new JObject();

            jobject.Add(ToCamelCaseRegex("RSLT_ID"), "");
            jobject.Add(ToCamelCaseRegex("TMNL_COMP_CONC_ID"), "");
            jobject.Add(ToCamelCaseRegex("TASK_ID"), "");
            jobject.Add(ToCamelCaseRegex("TASK_NO"), meter.MD_TaskNo);
            jobject.Add(ToCamelCaseRegex("DEV_ID"), "");
            jobject.Add(ToCamelCaseRegex("BAR_CODE"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("SAMPLE_NO"), "");
            jobject.Add(ToCamelCaseRegex("BEFORE_TEST_STAT"), "01");
            jobject.Add(ToCamelCaseRegex("AFTER_TEST_STAT"), "01");
            jobject.Add(ToCamelCaseRegex("PLANT_ID"), "01");
            jobject.Add(ToCamelCaseRegex("PLANT_ELEMENT_ID"), "01");
            jobject.Add(ToCamelCaseRegex("PLANT_MACH_ID"), "01");
            jobject.Add(ToCamelCaseRegex("PLANT_NO"), "01");
            jobject.Add(ToCamelCaseRegex("MACH_NO"), "01");
            jobject.Add(ToCamelCaseRegex("DEV_SEAT_NO"), meter.MD_Epitope); //
            jobject.Add(ToCamelCaseRegex("ON_POWER_TIME"), item1.Value.meterResoults[0].ItemDatas[7].Time); //
            jobject.Add(ToCamelCaseRegex("DOWN_POWER_TIME"), item1.Value.meterResoults[0].ItemDatas[6].Time); //
            jobject.Add(ToCamelCaseRegex("TEST_CATEGORIES"), GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]); //
            jobject.Add(ToCamelCaseRegex("VERI_POINT_NO"), "0"); //
            jobject.Add(ToCamelCaseRegex("VERI_POINT_SN"), "0"); //
   
            jobject.Add(ToCamelCaseRegex("VERI_RSLT"), item1.Value.Result == "合格" ? "02" : "01");

            fieldValue.Add(jobject);
            jobjestcc.Add("fieldValue", fieldValue);
            //jobjestcc.Add("fieldValue", JsonConvert.SerializeObject(jobject));
            //jobjestcc.Add("fieldValue", jobject);
            jaValues.Add(jobjestcc);
            jObjectC.Add("testResultData", jaValues);
            jArray.Add(jObjectC);
        }
        public void 其他项目(JArray jArray, TestMeterInfo meter, KeyValuePair<string, Core.Model.DnbModel.DnbInfo.MeterResoultData> item1)
        {
            
            JArray jaValues = new JArray();
            JObject jObjectC = new JObject();
            jObjectC.Add("itemCode", GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]);
            jObjectC.Add("itemResult", item1.Value.Result == "合格" ? "02" : "01");
            jObjectC.Add("barCode", meter.MD_BarCode);
            jObjectC.Add("detectPerson", meter.Checker1);
            jObjectC.Add("plantNo", "1");
            jObjectC.Add("machNo", "1");
            jObjectC.Add("environTemp", meter.Temperature);
            jObjectC.Add("environHum", meter.Humidity);

            JObject jobjestcc = new JObject();
            jobjestcc.Add("veriPointSN", 1);
            jobjestcc.Add("tableName", "V_TMNL_FUNC_OTHER_ITEM");
            JArray fieldValue = new JArray();
            JObject jobject = new JObject();

            jobject.Add(ToCamelCaseRegex("BAR_CODE"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("SAMPLE_NO"), meter.MD_BarCode);
            jobject.Add(ToCamelCaseRegex("PARAM"), GetItemId(item1.Value.meterResoults[0].Datas["项目名"]).Split(',')[1]);
            jobject.Add(ToCamelCaseRegex("TECHNICAL_REQUIRE"), meter.MD_WiringMode);
            jobject.Add(ToCamelCaseRegex("COLLECT_ITEM"), "");

            string COLLECT_RESULT = "";
            string EXTRACT_EVENT = "";
            foreach (var itemss in item1.Value.meterResoults[0].ItemDatas)
            {
                COLLECT_RESULT += itemss.Name + '|';
                EXTRACT_EVENT += itemss.TerminalData + '|';
            }
            jobject.Add(ToCamelCaseRegex("COLLECT_RESULT"), COLLECT_RESULT);
            jobject.Add(ToCamelCaseRegex("EXTRACT_EVENT"), EXTRACT_EVENT);
         
            jobject.Add(ToCamelCaseRegex("VERI_RSLT"), item1.Value.Result == "合格" ? "02" : "01");
            fieldValue.Add(jobject);
            jobjestcc.Add("fieldValue", fieldValue);
            jaValues.Add(jobjestcc);
            jObjectC.Add("testResultData", jaValues);
            jArray.Add(jObjectC);
        }
        #endregion



        /// <summary>
        /// 字段驼峰处理
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string ToCamelCaseRegex(string str)
        {
            string low = str.ToLower();

            return Regex.Replace(low, "_([a-z])", new MatchEvaluator(UpperMath)).Replace("_", "");
        }

        private static string UpperMath(Match match)
        {
            return match.Groups[1].Value.ToUpper();
        }

        private string GetItemId(string Name)
        {
            switch (Name)
            {
                case "外观":
                    return "外观,0100";
                case "终端密钥恢复":
                    return "终端密钥恢复,0201";
                case "读取终端信息":
                    return "读取终端信息,0202";
                case "时钟召测和对时":
                    return "时钟召测与对时,0203";
                case "基本参数":
                    return "基本参数,0204";
                case "抄表与费率参数":
                    return "抄表参数,0205";
                case "事件参数":
                    return "事件参数,0206";
                case "状态量采集":
                    return "状态量采集,0207";
                case "实时和当前数据":
                    return "终端当前数据,0208";
                case "电能表实时数据":
                    return "电能表实时数据,0209";
                case "电能表当前数据":
                    return "电能表当前数据,0210";
                case "历史日数据":
                    return "历史日数据,0211";
                case "历史月数据":
                    return "历史月数据,0210";
                case "负荷曲线":
                    return "负荷曲线,0213";
                case "终端采集645表计数据":
                    return "终端采集645表计数据,0214";
                case "终端主动上报":
                    return "终端主动上报,0215";
                case "终端对时事件":
                    return "终端对时事件,0216";
                case "终端485抄表错误":
                    return "终端故障记录485通讯故障,0217";
                case "终端编程事件":
                    return "终端编程事件,0218";
                case "电能表时间超差事件":
                    return "电能表时钟超差事件,0219";
                case "电能表示度下降事件":
                    return "电能表示度下降事件,0220";
                case "电能表超差事件":
                    return "电能量超差事件,0221";
                case "电能表飞走事件":
                    return "电能表飞走事件,0222";
                case "电能表停走事件":
                    return "电能表停走事件,0223";
                case "终端抄表失败":
                    return "终端抄表失败事件,0224";
                case "全事件采集上报":
                    return "电能表全事件采集,0225";
                case "电能表数据变更监控记录":
                    return "电能表数据变更监控记录,0226";
                case "12个/分脉冲量采集":
                    return "12个/分脉冲量采集,0227";
                case "120个/分脉冲量采集":
                    return "120个/分脉冲量采集,0228";
                case "总加组日和月电量召集":
                    return "总加组日电量和月电量采集,0229";
                case "分时段电能量数据存储":
                    return "分时段电能量数据存储,0230";
                case "购电参数设置事件":
                    return "购电参数设置事件,0241";
                case "终端停/上电事件":
                    return "停上电事件,0242";
                case "电压合格率统计":
                    return "电压合格率统计,0243";
                case "终端相序异常事件":
                    return "终端相序异常事件,0244";
                case "常温基本误差":
                    return "常温基本误差,0245";
                case "功率因素基本误差":
                    return "功率因素基本误差,0246";
                case "谐波影响":
                    return "谐波影响,0247";
                case "频率影响":
                    return "频率影响,0248";
                case "电源影响":
                    return "电源影响,0249";

                case "电流不平衡影响":
                    return "电流不平衡影响,0250";
                case "日计时误差":
                    return "日计时误差,0251";
                case "远程升级":
                    return "远程升级,0252";
                case "终端维护":
                    return "终端维护,0253";
                case "密钥下装":
                    return "终端密钥更新,0254";
                case "历史日数据补抄":
                    return "补抄测试（历史日数据）,0255";
                case "负荷曲线补抄":
                    return "补抄测试（负荷曲线数据）,0256";
                case "分组抄表":
                    return "分组抄表测试,0257";
                //case "数据检索测试":
                //    return "数据检索测试,0258";
                case "电能表事件补抄":
                    return "电表事件补抄测试,0259";
                case "抄表MAC验证":
                    return "抄表MAC验证,0260";
                case "随机交采数据":
                    return "随机交采测试,0261";
                case "单个测量电设置采集档案":
                    return "单个测量点设置采集档案,0262";
                case "时段控与购电控同时投入":
                    return "时段控与购电控同时投入,0263";
                case "组合参数读取与参数设置":
                    return "组合参数读取和参数设置,0264";
                case "透明方案":
                    return "透明方案,0265";
                case "精准校时":
                    return "精准校时,0266";
                case "结算日冻结":
                    return "结算日冻结,0267";
                case "安全模式":
                    return "安全模式,0268";
                case "功率因数越限统计":
                    return "功率因数越限统计,0269";
                case "时段功控":
                    return "时段功控,0231";
                case "厂休功控":
                    return "厂休功控,0232";
                case "营业报停功控":
                    return "营业报停功控,0233";
                case "当前功率下浮控":
                    return "当前功率下浮控,0234";
                case "月电控":
                    return "月电控,0235";
                case "购电控":
                    return "购电控,0236";
                case "催费告警":
                    return "催费告警,0237";
                case "保电功能":
                    return "保电功能,0238";
                case "剔除功能":
                    return "剔除功能,0239";
                case "遥控功能":
                    return "遥控功能,0240";
                case "终端逻辑地址查询":
                    return "终端逻辑地址查询,65";
                case "交采电量清零":
                    return "交采电量清零,66";
                case "常数试验":
                    return "常数试验,67";
                default:
                    return Name + ",1";
            }
            return "";
        }
    }
}
