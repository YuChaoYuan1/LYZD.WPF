//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace LYZD.Mis.Houda
//{
//    // Token: 0x02000061 RID: 97
//    public class OraDataHelper
//    {
//        // Token: 0x0600099D RID: 2461 RVA: 0x00058A28 File Offset: 0x00056C28
//        public static void GetDicPCodeTable()
//        {
//            OraDataHelper.g_DicPCodeTable = new Dictionary<string, StPCodeDicForMis>();
//        }


//        // Token: 0x0600099F RID: 2463 RVA: 0x0005999C File Offset: 0x00057B9C
//        public static MeterBasicInfo GetMeterModelFromClouMeterDataTmp(string strInfo)
//        {
//            MeterBasicInfo result;
//            if (string.IsNullOrEmpty(strInfo))
//            {
//                result = null;
//            }
//            else
//            {
//                List<MeterBasicInfo> list = new List<MeterBasicInfo>();
//                clsDataManage clsDataManage = new clsDataManage(false);
//                string sql = "select * from TMP_METER_INFO where LNG_BENCH_POINT_NO=" + strInfo;
//                list = clsDataManage.getBasicInformation(sql);
//                if (list.Count > 0)
//                {
//                    result = list[0];
//                }
//                else
//                {
//                    result = null;
//                }
//            }
//            return result;
//        }

//		// Token: 0x060009A8 RID: 2472 RVA: 0x0005F330 File Offset: 0x0005D530
//		public static string GetCodeByUserNameFromPrductionSchedulingSystem(string name)
//{
//    string text = "";
//    string result;
//    if (name == "")
//    {
//        result = text;
//    }
//    else
//    {
//        string sqlstring = string.Format("select organ_code from v_stru_organ where organ_name='{0}'", name);
//        DataSet dataSet = OraHelper.Query(sqlstring);
//        if (dataSet.Tables.Count > 0)
//        {
//            DataTable dataTable = dataSet.Tables[0];
//            if (dataTable.Rows.Count > 0)
//            {
//                text = dataTable.Rows[0]["organ_code"].ToString().Trim();
//            }
//        }
//        result = text;
//    }
//    return result;
//}

//// Token: 0x060009A9 RID: 2473 RVA: 0x0005F3D8 File Offset: 0x0005D5D8
//public static string GetCodeByUserNameFromVW(string VW, string name)
//{
//    string text = "";
//    string result;
//    if (name == "")
//    {
//        result = text;
//    }
//    else
//    {
//        string sqlstring = string.Format("select code from " + VW + " where code_name='{0}'", name);
//        DataSet dataSet = OraHelper.Query(sqlstring);
//        if (dataSet.Tables.Count > 0)
//        {
//            DataTable dataTable = dataSet.Tables[0];
//            if (dataTable.Rows.Count > 0)
//            {
//                text = dataTable.Rows[0]["code"].ToString().Trim();
//            }
//        }
//        result = text;
//    }
//    return result;
//}

//// Token: 0x060009AA RID: 2474 RVA: 0x0005F48C File Offset: 0x0005D68C
//public static string GetUserNameByCodeFromPrductionSchedulingSystem(string code)
//{
//    string text = "";
//    string result;
//    if (code == "")
//    {
//        result = text;
//    }
//    else
//    {
//        string sqlstring = string.Format("select organ_name from v_stru_organ where organ_code='{0}'", code);
//        DataSet dataSet = OraHelper.Query(sqlstring);
//        if (dataSet.Tables.Count > 0)
//        {
//            DataTable dataTable = dataSet.Tables[0];
//            if (dataTable.Rows.Count > 0)
//            {
//                text = dataTable.Rows[0]["organ_name"].ToString().Trim();
//            }
//        }
//        result = text;
//    }
//    return result;
//}

//// Token: 0x060009AB RID: 2475 RVA: 0x0005F534 File Offset: 0x0005D734
//public static StPCodeDicForMis GetPCodeDicFromPrductionSchedulingSystem(string codeType)
//{
//    StPCodeDicForMis stPCodeDicForMis = new StPCodeDicForMis();
//    string sqlstring;
//    if (GlobalUnit.MisInterFaceType != "厚达MIS接口")
//    {
//        sqlstring = string.Format("select * from mt_p_code@mid where code_type ='{0}'", codeType);
//    }
//    else
//    {
//        sqlstring = string.Format("select * from mt_p_code where code_type ='{0}'", codeType);
//    }
//    DataSet dataSet = OraHelper.Query(sqlstring);
//    if (dataSet.Tables.Count > 0)
//    {
//        DataTable dataTable = dataSet.Tables[0];
//        if (dataTable.Rows.Count > 0)
//        {
//            foreach (object obj in dataSet.Tables[0].Rows)
//            {
//                DataRow dataRow = (DataRow)obj;
//                string text = dataRow["value"].ToString().Trim();
//                string value = dataRow["name"].ToString().Trim();
//                if (text.Length > 0)
//                {
//                    if (!stPCodeDicForMis.DicPCode.ContainsKey(text))
//                    {
//                        stPCodeDicForMis.DicPCode.Add(text, value);
//                    }
//                }
//            }
//        }
//    }
//    return stPCodeDicForMis;
//}

//// Token: 0x060009AC RID: 2476 RVA: 0x0005F6B8 File Offset: 0x0005D8B8
//public static StPCodeDicForMis_Mysql GetPCodeDicFromPrductionSchedulingSystem_Mysql(string codeType)
//{
//    StPCodeDicForMis_Mysql stPCodeDicForMis_Mysql = new StPCodeDicForMis_Mysql();
//    string sqlstring = string.Format("select * from m_qt_p_code where code_type ='{0}'", codeType);
//    DataSet dataSet = MySqlHelper.Query(sqlstring);
//    if (dataSet.Tables.Count > 0)
//    {
//        DataTable dataTable = dataSet.Tables[0];
//        if (dataTable.Rows.Count > 0)
//        {
//            foreach (object obj in dataSet.Tables[0].Rows)
//            {
//                DataRow dataRow = (DataRow)obj;
//                string text = dataRow["code"].ToString().Trim();
//                string value = dataRow["name"].ToString().Trim();
//                if (text.Length > 0)
//                {
//                    if (!stPCodeDicForMis_Mysql.DicPCode.ContainsKey(text))
//                    {
//                        stPCodeDicForMis_Mysql.DicPCode.Add(text, value);
//                    }
//                }
//            }
//        }
//    }
//    return stPCodeDicForMis_Mysql;
//}

//// Token: 0x060009AD RID: 2477 RVA: 0x0005F80C File Offset: 0x0005DA0C
//private static Cus_PowerFangXiang GetGLFXFromString(string strValue)
//{
//    switch (strValue)
//    {
//        case "正向有功":
//            return Cus_PowerFangXiang.正向有功;
//        case "正向无功":
//            return Cus_PowerFangXiang.正向无功;
//        case "反向有功":
//            return Cus_PowerFangXiang.反向有功;
//        case "反向无功":
//            return Cus_PowerFangXiang.反向无功;
//        case "第一象限无功":
//            return Cus_PowerFangXiang.第一象限无功;
//        case "第二象限无功":
//            return Cus_PowerFangXiang.第二象限无功;
//        case "第三象限无功":
//            return Cus_PowerFangXiang.第三象限无功;
//        case "第四象限无功":
//            return Cus_PowerFangXiang.第四象限无功;
//    }
//    return Cus_PowerFangXiang.正向有功;
//}

//// Token: 0x060009AE RID: 2478 RVA: 0x0005F8FC File Offset: 0x0005DAFC
//private static Cus_PowerYuanJian GetYuanJianFromString(string strValue)
//{
//    Cus_PowerYuanJian result = Cus_PowerYuanJian.H;
//    if (strValue != null)
//    {
//        if (!(strValue == "ABC") && !(strValue == "AC"))
//        {
//            if (!(strValue == "A"))
//            {
//                if (!(strValue == "B"))
//                {
//                    if (strValue == "C")
//                    {
//                        result = Cus_PowerYuanJian.C;
//                    }
//                }
//                else
//                {
//                    result = Cus_PowerYuanJian.B;
//                }
//            }
//            else
//            {
//                result = Cus_PowerYuanJian.A;
//            }
//        }
//        else
//        {
//            result = Cus_PowerYuanJian.H;
//        }
//    }
//    return result;
//}

//// Token: 0x060009AF RID: 2479 RVA: 0x0005F96C File Offset: 0x0005DB6C
//public static bool UpdateMeterInfoToTianJinMis(MeterBasicInfo meter, string meter_id, out string strReturnMessage)
//{
//    strReturnMessage = "";
//    GlobalUnit.g_MsgControl.OutMessage(string.Format("开始上传[{0}]", meter.Mb_ChrTxm), false);
//    CheckBasicDataForTianJinMis ckRecords;
//    DataManager.ProgressMeterCheckRecode(meter, out ckRecords);
//    bool result;
//    if (!OraDataHelper.UpDataCheckRecords(ckRecords))
//    {
//        strReturnMessage = string.Format("电能表[{0}]检定记录上传失败!", meter.Mb_ChrTxm);
//        result = false;
//    }
//    else
//    {
//        CheckErrForTianJinMis[] ckErr;
//        DataManager.ProgressMeterErr(meter, out ckErr);
//        if (!OraDataHelper.UpDataCheckErr(ckErr))
//        {
//            strReturnMessage = string.Format("电能表[{0}]检定误差上传失败!", meter.Mb_ChrCcbh);
//            result = false;
//        }
//        else
//        {
//            result = true;
//        }
//    }
//    return result;
//}

//// Token: 0x060009B0 RID: 2480 RVA: 0x0005F9F8 File Offset: 0x0005DBF8
//public static bool UpdateMeterInfoToJiNanMis(MeterBasicInfo meter, out string strReturnMessage)
//{
//    strReturnMessage = "";
//    GlobalUnit.g_MsgControl.OutMessage(string.Format("开始上传[{0}]", meter.Mb_ChrTxm), false);
//    bool result;
//    try
//    {
//        string config = GlobalUnit.GetConfig("MIS_WEBSERVICE_URL", "");
//        OraDataHelper.m_webSev = new WebServiceClass(config, "http://server.webservice.core.epm");
//        WebServiceClass.HeaderBlock[] array = new WebServiceClass.HeaderBlock[2];
//        array[0] = new WebServiceClass.HeaderBlock();
//        array[0].Name = "username";
//        array[0].Content = "sysadmin";
//        array[0].Namespace = "Authorization";
//        array[0].Prefix = "ns1";
//        array[0].Actor = "http://schemas.xmlsoap.org/soap/actor/next";
//        array[1] = new WebServiceClass.HeaderBlock();
//        array[1].Name = "password";
//        array[1].Content = "1";
//        array[1].Namespace = "Authorization";
//        array[1].Prefix = "ns2";
//        array[1].Actor = "http://schemas.xmlsoap.org/soap/actor/next";
//        string text = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><DBSET><R><C N=\"ITEM_LIST\"><DBSET>";
//        string str = "";
//        string str2 = "";
//        text += "<R>";
//        string text2 = meter.Mb_chrOther2;
//        text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//        text2 = meter.Mb_chrOther1;
//        text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//        text2 = meter.Mb_ChrTxm;
//        text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//        text2 = "02001";
//        text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//        text2 = "02001_004";
//        text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//        text2 = "额定电压";
//        text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//        text2 = "01";
//        text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//        text += "<C N=\"STD_CODE\"/>";
//        text += "<C N=\"NUM_PRECISION\"/>";
//        text2 = "YYYY-MM-DD";
//        text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//        text2 = meter.Mb_chrUb.Trim();
//        text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//        text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//        text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//        text += "</R>";
//        text += "<R>";
//        text2 = meter.Mb_chrOther2;
//        text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//        text2 = meter.Mb_chrOther1;
//        text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//        text2 = meter.Mb_ChrTxm;
//        text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//        text2 = "02001";
//        text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//        text2 = "02001_005";
//        text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//        text2 = "电流量程";
//        text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//        text2 = "01";
//        text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//        text += "<C N=\"STD_CODE\"/>";
//        text += "<C N=\"NUM_PRECISION\"/>";
//        text2 = "YYYY-MM-DD";
//        text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//        text2 = meter.Mb_chrIb.Trim();
//        text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//        text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//        text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//        text += "</R>";
//        text += "<R>";
//        text2 = meter.Mb_chrOther2;
//        text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//        text2 = meter.Mb_chrOther1;
//        text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//        text2 = meter.Mb_ChrTxm;
//        text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//        text2 = "02001";
//        text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//        text2 = "02001_006";
//        text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//        text2 = "额定频率";
//        text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//        text2 = "01";
//        text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//        text += "<C N=\"STD_CODE\"/>";
//        text += "<C N=\"NUM_PRECISION\"/>";
//        text2 = "YYYY-MM-DD";
//        text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//        text2 = meter.Mb_chrHz.Trim();
//        text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//        text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//        text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//        text += "</R>";
//        text += "<R>";
//        text2 = meter.Mb_chrOther2;
//        text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//        text2 = meter.Mb_chrOther1;
//        text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//        text2 = meter.Mb_ChrTxm;
//        text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//        text2 = "02001";
//        text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//        text2 = "02001_007";
//        text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//        text2 = "样表常数";
//        text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//        text2 = "01";
//        text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//        text += "<C N=\"STD_CODE\"/>";
//        text += "<C N=\"NUM_PRECISION\"/>";
//        text2 = "YYYY-MM-DD";
//        text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//        text2 = meter.Mb_chrBcs.Trim();
//        text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//        text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//        text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//        text += "</R>";
//        text += "<R>";
//        text2 = meter.Mb_chrOther2;
//        text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//        text2 = meter.Mb_chrOther1;
//        text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//        text2 = meter.Mb_ChrTxm;
//        text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//        text2 = "02001";
//        text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//        text2 = "02001_008";
//        text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//        text2 = "环境温度";
//        text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//        text2 = "01";
//        text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//        text += "<C N=\"STD_CODE\"/>";
//        text += "<C N=\"NUM_PRECISION\"/>";
//        text2 = "YYYY-MM-DD";
//        text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//        text2 = meter.Mb_chrWd.Trim();
//        text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//        text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//        text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//        text += "</R>";
//        text += "<R>";
//        text2 = meter.Mb_chrOther2;
//        text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//        text2 = meter.Mb_chrOther1;
//        text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//        text2 = meter.Mb_ChrTxm;
//        text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//        text2 = "02001";
//        text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//        text2 = "02001_009";
//        text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//        text2 = "相对湿度";
//        text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//        text2 = "01";
//        text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//        text += "<C N=\"STD_CODE\"/>";
//        text += "<C N=\"NUM_PRECISION\"/>";
//        text2 = "YYYY-MM-DD";
//        text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//        text2 = meter.Mb_chrSd.Trim();
//        text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//        text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//        text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//        text += "</R>";
//        Dictionary<string, MeterQdQid> meterQdQids = meter.MeterQdQids;
//        if (meter.MeterQdQids.Count > 0)
//        {
//            foreach (string text3 in meterQdQids.Keys)
//            {
//                if (text3.Length > 3 && text3.Substring(0, 3) == 109.ToString())
//                {
//                    text += "<R>";
//                    text2 = meter.Mb_chrOther2;
//                    text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                    text2 = meter.Mb_chrOther1;
//                    text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                    text2 = meter.Mb_ChrTxm;
//                    text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                    text2 = "02005";
//                    text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                    if (meter.Mb_intClfs == 5)
//                    {
//                        text2 = "02005_004";
//                        text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                        text2 = "单相试验结果";
//                        text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                    }
//                    else if (meter.MeterQdQids[text3].Mqd_chrProjectName.IndexOf("反向有功") != -1)
//                    {
//                        text2 = "02005_006";
//                        text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                        text2 = "三相反向向试验结果";
//                        text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                    }
//                    else
//                    {
//                        text2 = "02005_005";
//                        text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                        text2 = "三相正向试验结果";
//                        text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                    }
//                    text2 = "01";
//                    text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                    text += "<C N=\"STD_CODE\"/>";
//                    text += "<C N=\"NUM_PRECISION\"/>";
//                    text2 = "YYYY-MM-DD";
//                    text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                    text2 = ((meter.MeterQdQids[text3].Mqd_chrJL.Trim() == "合格") ? "0" : "1");
//                    str2 = text2;
//                    text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//                    text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                    text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                    text += "</R>";
//                }
//            }
//            text += "<R>";
//            text2 = meter.Mb_chrOther2;
//            text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_chrOther1;
//            text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_ChrTxm;
//            text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//            text2 = "02005";
//            text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//            text2 = "02005_007";
//            text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//            text2 = "试验结论";
//            text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//            text2 = "01";
//            text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//            text += "<C N=\"STD_CODE\"/>";
//            text += "<C N=\"NUM_PRECISION\"/>";
//            text2 = "YYYY-MM-DD";
//            text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//            text = text + "<C N=\"DEFAULT_VALUE\">" + str2 + "</C>";
//            text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//            text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//            text += "</R>";
//        }
//        if (meter.MeterQdQids.Count > 0)
//        {
//            foreach (string text3 in meterQdQids.Keys)
//            {
//                if (text3.Length > 3 && text3.Substring(0, 3) == 110.ToString())
//                {
//                    text += "<R>";
//                    text2 = meter.Mb_chrOther2;
//                    text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                    text2 = meter.Mb_chrOther1;
//                    text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                    text2 = meter.Mb_ChrTxm;
//                    text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                    text2 = "02006";
//                    text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                    text2 = "02006_004";
//                    text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                    text2 = "试验结果";
//                    text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                    text2 = "01";
//                    text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                    text += "<C N=\"STD_CODE\"/>";
//                    text += "<C N=\"NUM_PRECISION\"/>";
//                    text2 = "YYYY-MM-DD";
//                    text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                    text2 = ((meter.MeterQdQids[text3].Mqd_chrJL.Trim() == "合格") ? "0" : "1");
//                    str2 = text2;
//                    text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//                    text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                    text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                    text += "</R>";
//                }
//            }
//            text += "<R>";
//            text2 = meter.Mb_chrOther2;
//            text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_chrOther1;
//            text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_ChrTxm;
//            text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//            text2 = "02006";
//            text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//            text2 = "02006_005";
//            text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//            text2 = "试验结论";
//            text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//            text2 = "01";
//            text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//            text += "<C N=\"STD_CODE\"/>";
//            text += "<C N=\"NUM_PRECISION\"/>";
//            text2 = "YYYY-MM-DD";
//            text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//            text = text + "<C N=\"DEFAULT_VALUE\">" + str2 + "</C>";
//            text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//            text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//            text += "</R>";
//        }
//        if (meter.MeterZZErrors.Count > 0)
//        {
//            text += "<R>";
//            text2 = meter.Mb_chrOther2;
//            text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_chrOther1;
//            text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_ChrTxm;
//            text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//            text2 = "02004";
//            text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//            if (meter.Mb_intClfs == 5)
//            {
//                text2 = "02004_005";
//                str = "单相Imax结论";
//            }
//            else
//            {
//                text2 = "02004_006";
//                str = "三相输出铭牌比对结论";
//            }
//            text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//            text = text + "<C N=\"COL_NAME\">" + str + "</C>";
//            text2 = "01";
//            text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//            text += "<C N=\"STD_CODE\"/>";
//            text += "<C N=\"NUM_PRECISION\"/>";
//            text2 = "YYYY-MM-DD";
//            text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//            string key = "106";
//            text2 = ((meter.MeterResults[key].Mr_chrRstValue == "合格") ? "0" : "1");
//            text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//            text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//            text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//            text += "</R>";
//            text += "<R>";
//            text2 = meter.Mb_chrOther2;
//            text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_chrOther1;
//            text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_ChrTxm;
//            text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//            text2 = "02004";
//            text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//            text2 = "02004_007";
//            text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//            text2 = "试验结论";
//            text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//            text2 = "01";
//            text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//            text += "<C N=\"STD_CODE\"/>";
//            text += "<C N=\"NUM_PRECISION\"/>";
//            text2 = "YYYY-MM-DD";
//            text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//            text2 = ((meter.MeterResults[key].Mr_chrRstValue == "合格") ? "0" : "1");
//            text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//            text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//            text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//            text += "</R>";
//        }
//        if (meter.Mb_intClfs != 5)
//        {
//            string[] xldata = DataManager.GetXLData(meter);
//            if ((xldata[0] != null && xldata[0] != "") || (xldata[1] != null && xldata[1] != ""))
//            {
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02053";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02053_004";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "Imax实际误差";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text2 = xldata[2];
//                text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//            }
//            if ((xldata[5] != null && xldata[5] != "") || (xldata[6] != null && xldata[6] != ""))
//            {
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02053";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02053_005";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "In实际误差";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text2 = xldata[7];
//                text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//            }
//            if ((xldata[10] != null && xldata[10] != "") || (xldata[11] != null && xldata[11] != ""))
//            {
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02053";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02053_006";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "0.1In实际误差";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text2 = xldata[12];
//                text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//            }
//            text += "<R>";
//            text2 = meter.Mb_chrOther2;
//            text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_chrOther1;
//            text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_ChrTxm;
//            text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//            text2 = "02053";
//            text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//            text2 = "02053_007";
//            text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//            text2 = "试验结论";
//            text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//            text2 = "01";
//            text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//            text += "<C N=\"STD_CODE\"/>";
//            text += "<C N=\"NUM_PRECISION\"/>";
//            text2 = "YYYY-MM-DD";
//            text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//            text2 = ((xldata[9] == "合格") ? "0" : "1");
//            text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//            text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//            text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//            text += "</R>";
//        }
//        string key2 = 2.ToString().PadLeft(3, '0');
//        if (meter.MeterDgns.ContainsKey(key2))
//        {
//            text += "<R>";
//            text2 = meter.Mb_chrOther2;
//            text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_chrOther1;
//            text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_ChrTxm;
//            text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//            text2 = "02010";
//            text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//            text2 = "02010_004";
//            text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//            text2 = "试验结果";
//            text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//            text2 = "01";
//            text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//            text += "<C N=\"STD_CODE\"/>";
//            text += "<C N=\"NUM_PRECISION\"/>";
//            text2 = "YYYY-MM-DD";
//            text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//            text2 = ((meter.MeterDgns[key2].Md_chrValue == "合格") ? "0" : "1");
//            text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//            text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//            text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//            text += "</R>";
//            text += "<R>";
//            text2 = meter.Mb_chrOther2;
//            text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_chrOther1;
//            text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_ChrTxm;
//            text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//            text2 = "02010";
//            text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//            text2 = "02010_005";
//            text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//            text2 = "试验结论";
//            text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//            text2 = "01";
//            text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//            text += "<C N=\"STD_CODE\"/>";
//            text += "<C N=\"NUM_PRECISION\"/>";
//            text2 = "YYYY-MM-DD";
//            text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//            text2 = ((meter.MeterDgns[key2].Md_chrValue == "合格") ? "0" : "1");
//            text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//            text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//            text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//            text += "</R>";
//        }
//        string text4 = string.Empty;
//        string text5 = string.Empty;
//        string text6 = string.Empty;
//        string text7 = string.Empty;
//        string str3 = string.Empty;
//        string str4 = string.Empty;
//        key2 = 5.ToString("D3");
//        if (meter.MeterDgns.ContainsKey(key2))
//        {
//            string str5 = (meter.MeterDgns[key2].Md_chrValue == "合格") ? "0" : "1";
//            bool flag = true;
//            if (flag)
//            {
//                string key3 = "00501";
//                if (meter.MeterDgns.ContainsKey(key3))
//                {
//                    string[] array2 = meter.MeterDgns[key3].Md_chrValue.Split(new char[]
//                    {
//                                '|'
//                    });
//                    if (array2.Length >= 4)
//                    {
//                        text6 = array2[1];
//                    }
//                }
//                key3 = "00502";
//                if (meter.MeterDgns.ContainsKey(key3))
//                {
//                    string[] array2 = meter.MeterDgns[key3].Md_chrValue.Split(new char[]
//                    {
//                                '|'
//                    });
//                    if (array2.Length >= 4)
//                    {
//                        text5 = array2[1];
//                    }
//                }
//                key3 = "00503";
//                if (meter.MeterDgns.ContainsKey(key3))
//                {
//                    string[] array2 = meter.MeterDgns[key3].Md_chrValue.Split(new char[]
//                    {
//                                '|'
//                    });
//                    if (array2.Length >= 4)
//                    {
//                        text4 = array2[1];
//                    }
//                }
//                key3 = "00504";
//                if (meter.MeterDgns.ContainsKey(key3))
//                {
//                    string[] array2 = meter.MeterDgns[key3].Md_chrValue.Split(new char[]
//                    {
//                                '|'
//                    });
//                    if (array2.Length >= 4)
//                    {
//                        text7 = array2[1];
//                    }
//                }
//                key3 = "00500";
//                if (meter.MeterDgns.ContainsKey(key3))
//                {
//                    string[] array2 = meter.MeterDgns[key3].Md_chrValue.Split(new char[]
//                    {
//                                '|'
//                    });
//                    if (array2.Length >= 4)
//                    {
//                        str4 = array2[1];
//                    }
//                }
//                key3 = "00507";
//                if (meter.MeterDgns.ContainsKey(key3))
//                {
//                    str3 = meter.MeterDgns[key3].Md_chrValue;
//                }
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02009";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02009_004";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "尖电量";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text = text + "<C N=\"DEFAULT_VALUE\">" + text4 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02009";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02009_005";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "峰电量";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text = text + "<C N=\"DEFAULT_VALUE\">" + text5 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02009";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02009_006";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "平电量";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text = text + "<C N=\"DEFAULT_VALUE\">" + text6 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02009";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02009_007";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "谷电量";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text = text + "<C N=\"DEFAULT_VALUE\">" + text7 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02009";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02009_008";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "各分时电量和";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                string str6 = (Convert.ToDouble(text4) + Convert.ToDouble(text5) + Convert.ToDouble(text6) + Convert.ToDouble(text7)).ToString();
//                text = text + "<C N=\"DEFAULT_VALUE\">" + str6 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02009";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02009_009";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "总电量";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text = text + "<C N=\"DEFAULT_VALUE\">" + str4 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02009";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02009_011";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "实际误差";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text = text + "<C N=\"DEFAULT_VALUE\">" + str3 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02009";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02009_012";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "试验结论";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text = text + "<C N=\"DEFAULT_VALUE\">" + str5 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//            }
//        }
//        key2 = "2";
//        if (meter.MeterErrAccords.ContainsKey(key2))
//        {
//            if (meter.MeterErrAccords[key2].lstTestPoint.Count > 0)
//            {
//                foreach (string key4 in meter.MeterErrAccords[key2].lstTestPoint.Keys)
//                {
//                    MeterErrAccordBase meterErrAccordBase = meter.MeterErrAccords[key2].lstTestPoint[key4];
//                    text += "<R>";
//                    text2 = meter.Mb_chrOther2;
//                    text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                    text2 = meter.Mb_chrOther1;
//                    text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                    text2 = meter.Mb_ChrTxm;
//                    text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                    text2 = "02013";
//                    text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                    if (meterErrAccordBase.Mea_PrjName.Contains("0.5L"))
//                    {
//                        text2 = "02013_005";
//                        str = "0.5L功率实际误差";
//                    }
//                    else
//                    {
//                        text2 = "02013_004";
//                        str = "1.0功率实际误差";
//                    }
//                    text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                    text = text + "<C N=\"COL_NAME\">" + str + "</C>";
//                    text2 = "01";
//                    text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                    text += "<C N=\"STD_CODE\"/>";
//                    text += "<C N=\"NUM_PRECISION\"/>";
//                    text2 = "YYYY-MM-DD";
//                    text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                    text2 = meterErrAccordBase.Mea_Wc;
//                    text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//                    text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                    text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                    text += "</R>";
//                    str2 = ((meterErrAccordBase.Mea_ItemResult == "合格") ? "0" : "1");
//                }
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02013";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02013_016";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "试验结论";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text = text + "<C N=\"DEFAULT_VALUE\">" + str2 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//            }
//        }
//        key2 = "1";
//        if (meter.MeterErrAccords.ContainsKey(key2))
//        {
//            if (meter.MeterErrAccords[key2].lstTestPoint.Count > 0)
//            {
//                foreach (string key4 in meter.MeterErrAccords[key2].lstTestPoint.Keys)
//                {
//                    MeterErrAccordBase meterErrAccordBase = meter.MeterErrAccords[key2].lstTestPoint[key4];
//                    if (meterErrAccordBase.Mea_PrjName.Contains("1.0 1.0Ib") || meterErrAccordBase.Mea_PrjName.Contains("0.5L 1.0Ib") || meterErrAccordBase.Mea_PrjName.Contains("1.0 0.1Ib"))
//                    {
//                        text += "<R>";
//                        text2 = meter.Mb_chrOther2;
//                        text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                        text2 = meter.Mb_chrOther1;
//                        text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                        text2 = meter.Mb_ChrTxm;
//                        text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                        text2 = "02014";
//                        text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                        if (meter.Mb_intClfs == 5)
//                        {
//                            if (meterErrAccordBase.Mea_PrjName.Contains("1.0 1.0Ib"))
//                            {
//                                text2 = "02014_004";
//                                str = "单相1.0功率Ib实际误差";
//                            }
//                            else if (meterErrAccordBase.Mea_PrjName.Contains("0.5L 1.0Ib"))
//                            {
//                                text2 = "02014_005";
//                                str = "单相0.5L功率Ib实际误差";
//                            }
//                            else if (meterErrAccordBase.Mea_PrjName.Contains("1.0 0.1Ib"))
//                            {
//                                text2 = "02014_006";
//                                str = "单相1.0功率0.1Ib实际误差";
//                            }
//                        }
//                        else if (meterErrAccordBase.Mea_PrjName.Contains("1.0 1.0Ib"))
//                        {
//                            text2 = "02014_007";
//                            str = "三相1.0功率In实际误差";
//                        }
//                        else if (meterErrAccordBase.Mea_PrjName.Contains("0.5L 1.0Ib"))
//                        {
//                            text2 = "02014_008";
//                            str = "三相0.5L功率In实际误差";
//                        }
//                        else if (meterErrAccordBase.Mea_PrjName.Contains("1.0 0.1Ib"))
//                        {
//                            text2 = "02014_009";
//                            str = "三相1.0功率0.1In实际误差";
//                        }
//                        text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                        text = text + "<C N=\"COL_NAME\">" + str + "</C>";
//                        text2 = "01";
//                        text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                        text += "<C N=\"STD_CODE\"/>";
//                        text += "<C N=\"NUM_PRECISION\"/>";
//                        text2 = "YYYY-MM-DD";
//                        text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                        text2 = meterErrAccordBase.Mea_Wc;
//                        text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//                        text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                        text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                        text += "</R>";
//                        str2 = ((meterErrAccordBase.Mea_ItemResult == "合格") ? "0" : "1");
//                    }
//                }
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02014";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02014_010";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "试验结论";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text = text + "<C N=\"DEFAULT_VALUE\">" + str2 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//            }
//        }
//        key2 = "3";
//        if (meter.MeterErrAccords.ContainsKey(key2))
//        {
//            if (meter.MeterErrAccords[key2].lstTestPoint.Count > 0)
//            {
//                foreach (string key4 in meter.MeterErrAccords[key2].lstTestPoint.Keys)
//                {
//                    MeterErrAccordBase meterErrAccordBase = meter.MeterErrAccords[key2].lstTestPoint[key4];
//                    text += "<R>";
//                    text2 = meter.Mb_chrOther2;
//                    text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                    text2 = meter.Mb_chrOther1;
//                    text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                    text2 = meter.Mb_ChrTxm;
//                    text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                    text2 = "02015";
//                    text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                    if (meter.Mb_intClfs == 5)
//                    {
//                        if (meterErrAccordBase.Mea_PrjName.Contains("0.05Ib"))
//                        {
//                            text2 = "02015_004";
//                            str = "单相0.05Ib实际误差";
//                        }
//                        else if (meterErrAccordBase.Mea_PrjName.Contains("1.0Ib"))
//                        {
//                            text2 = "02015_005";
//                            str = "单相Ib实际误差";
//                        }
//                        else if (meterErrAccordBase.Mea_PrjName.Contains("Imax"))
//                        {
//                            text2 = "02015_006";
//                            str = "单相Imax实际误差";
//                        }
//                    }
//                    else if (meterErrAccordBase.Mea_PrjName.Contains("0.05Ib"))
//                    {
//                        text2 = "02015_007";
//                        str = "三相0.05In实际误差";
//                    }
//                    else if (meterErrAccordBase.Mea_PrjName.Contains("1.0Ib"))
//                    {
//                        text2 = "02015_008";
//                        str = "三相In实际误差";
//                    }
//                    else if (meterErrAccordBase.Mea_PrjName.Contains("Imax"))
//                    {
//                        text2 = "02015_009";
//                        str = "三相Imax实际误差";
//                    }
//                    text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                    text = text + "<C N=\"COL_NAME\">" + str + "</C>";
//                    text2 = "01";
//                    text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                    text += "<C N=\"STD_CODE\"/>";
//                    text += "<C N=\"NUM_PRECISION\"/>";
//                    text2 = "YYYY-MM-DD";
//                    text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                    text2 = meterErrAccordBase.Mea_Wc;
//                    text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//                    text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                    text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                    text += "</R>";
//                    str2 = ((meterErrAccordBase.Mea_ItemResult == "合格") ? "0" : "1");
//                }
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02015";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02015_010";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "试验结论";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text = text + "<C N=\"DEFAULT_VALUE\">" + str2 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//            }
//        }
//        if (meter.MeterPowers.Count > 0)
//        {
//            foreach (string text8 in meter.MeterPowers.Keys)
//            {
//            }
//        }
//        key2 = 13.ToString("D3");
//        if (meter.MeterCostControls.ContainsKey(key2))
//        {
//            string str7 = (meter.MeterCostControls[key2].Mfk_chrJL.Trim() == "合格") ? "0" : "1";
//            if (meter.MeterCostControls[key2].Mfk_chrJL == "合格" || meter.MeterCostControls[key2].Mfk_chrJL == "公钥合格" || meter.MeterCostControls[key2].Mfk_chrJL == "私钥合格")
//            {
//                str7 = "0";
//            }
//            text += "<R>";
//            text2 = meter.Mb_chrOther2;
//            text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_chrOther1;
//            text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//            text2 = meter.Mb_ChrTxm;
//            text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//            text2 = "02045";
//            text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//            text2 = "02045_007";
//            text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//            text2 = "试验结论";
//            text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//            text2 = "01";
//            text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//            text += "<C N=\"STD_CODE\"/>";
//            text += "<C N=\"NUM_PRECISION\"/>";
//            text2 = "YYYY-MM-DD";
//            text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//            text = text + "<C N=\"DEFAULT_VALUE\">" + str7 + "</C>";
//            text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//            text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//            text += "</R>";
//        }
//        if (meter.MeterErrors.Keys.Count > 0)
//        {
//            string text9 = string.Empty;
//            string[] array3 = new string[meter.MeterErrors.Keys.Count];
//            meter.MeterErrors.Keys.CopyTo(array3, 0);
//            foreach (string text10 in array3)
//            {
//                if (text10.Length != 3)
//                {
//                    MeterError meterError = meter.MeterErrors[text10];
//                    string[] array4 = meterError.Me_chrWcMore.Split(new char[]
//                    {
//                                '|'
//                    });
//                    if (array4.Length > 2)
//                    {
//                        string a = text10.Substring(0, 1);
//                        if (!(a == "2"))
//                        {
//                            string text11 = text10.Substring(0, 3);
//                            string text12 = text10.Substring(7);
//                            string text13 = text10.Substring(3, 2);
//                            string text14 = text10.Substring(5, 2);
//                            switch (int.Parse(text11.Substring(1, 1)))
//                            {
//                                case 1:
//                                    meterError.Me_Glfx = "正向有功";
//                                    break;
//                                case 2:
//                                    meterError.Me_Glfx = "正向无功";
//                                    break;
//                                case 3:
//                                    meterError.Me_Glfx = "反向有功";
//                                    break;
//                                case 4:
//                                    meterError.Me_Glfx = "反向无功";
//                                    break;
//                                default:
//                                    meterError.Me_Glfx = "正向有功";
//                                    break;
//                            }
//                            meterError.Me_intYj = int.Parse(text11.Substring(2, 1));
//                            string text15;
//                            switch (int.Parse(text11.Substring(2, 1)))
//                            {
//                                case 1:
//                                    text15 = "合元";
//                                    break;
//                                case 2:
//                                    text15 = "A元";
//                                    break;
//                                case 3:
//                                    text15 = "B元";
//                                    break;
//                                case 4:
//                                    text15 = "C元";
//                                    break;
//                                default:
//                                    text15 = "合元";
//                                    break;
//                            }
//                            text9 += "<R>";
//                            text2 = meter.Mb_chrOther2;
//                            text9 = text9 + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                            text2 = meter.Mb_chrOther1;
//                            text9 = text9 + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                            text2 = meter.Mb_ChrTxm;
//                            text9 = text9 + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                            if (meter.Mb_intClfs == 5)
//                            {
//                                if (meterError.Me_Glfx == "正向有功")
//                                {
//                                    text2 = "02003_a";
//                                    text9 = text9 + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_a_004";
//                                        str = "单相正向有功1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_a_005";
//                                        str = "单相正向有功1.0功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_a_006";
//                                        str = "单相正向有功1.0功率Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_a_007";
//                                        str = "单相正向有功1.0功率0.1Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_a_008";
//                                        str = "单相正向有功1.0功率0.05Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_a_009";
//                                        str = "单相正向有功0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_a_010";
//                                        str = "单相正向有功0.5L功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_a_011";
//                                        str = "单相正向有功0.5L功率Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_a_012";
//                                        str = "单相正向有功0.5L功率0.2Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_a_013";
//                                        str = "单相正向有功0.5L功率0.1Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_a_014";
//                                        str = "单相正向有功0.8C功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_a_015";
//                                        str = "单相正向有功0.8C功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_a_016";
//                                        str = "单相正向有功0.8C功率Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_a_017";
//                                        str = "单相正向有功0.8C功率0.2Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_a_018";
//                                        str = "单相正向有功0.8C功率0.1Ib";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                                else if (meterError.Me_Glfx == "反向有功")
//                                {
//                                    text2 = "02003_b";
//                                    text9 = text9 + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_b_019";
//                                        str = "单相反向有功1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_b_020";
//                                        str = "单相反向有功1.0功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_b_021";
//                                        str = "单相反向有功1.0功率Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_b_022";
//                                        str = "单相反向有功1.0功率0.1Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_b_023";
//                                        str = "单相反向有功1.0功率0.05Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_b_024";
//                                        str = "单相反向有功0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_b_025";
//                                        str = "单相反向有功0.5L功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_b_026";
//                                        str = "单相反向有功0.5L功率Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_b_027";
//                                        str = "单相反向有功0.5L功率0.2Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_b_028";
//                                        str = "单相反向有功0.5L功率0.1Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_b_029";
//                                        str = "单相反向有功0.8C功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_b_030";
//                                        str = "单相反向有功0.8C功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_b_031";
//                                        str = "单相反向有功0.8C功率Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_b_032";
//                                        str = "单相反向有功0.8C功率0.2Ib";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_b_033";
//                                        str = "单相反向有功0.8C功率0.1Ib";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                                else
//                                {
//                                    text2 = "不存在";
//                                    str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                    {
//                                                meterError.Me_Glfx,
//                                                text15,
//                                                meterError.Me_chrGlys,
//                                                meterError.Me_dblxIb
//                                    });
//                                }
//                            }
//                            else if (meterError.Me_Glfx == "正向有功")
//                            {
//                                text2 = "02003_a";
//                                text9 = text9 + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                                if (text15 == "合元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_a_034";
//                                        str = "三相正向有功平衡1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_a_079";
//                                        str = "三相正向有功平衡1.0功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_a_035";
//                                        str = "三相正向有功平衡1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_a_080";
//                                        str = "三相正向有功平衡1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_a_036";
//                                        str = "三相正向有功平衡1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.01Ib")
//                                    {
//                                        text2 = "02003_a_037";
//                                        str = "三相正向有功平衡1.0功率0.01In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.02Ib")
//                                    {
//                                        text2 = "02003_a_070";
//                                        str = "三相正向有功平衡1.0功率0.02In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_a_038";
//                                        str = "三相正向有功平衡0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_a_082";
//                                        str = "三相正向有功平衡0.5L功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_a_039";
//                                        str = "三相正向有功平衡0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_a_083";
//                                        str = "三相正向有功平衡0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_a_040";
//                                        str = "三相正向有功平衡0.5L功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_a_074";
//                                        str = "三相正向有功平衡0.5L功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.02Ib")
//                                    {
//                                        text2 = "02003_a_041";
//                                        str = "三相正向有功平衡0.5L功率0.02In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_a_042";
//                                        str = "三相正向有功平衡0.8C功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_a_085";
//                                        str = "三相正向有功平衡0.8C功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_a_043";
//                                        str = "三相正向有功平衡0.8C功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_a_086";
//                                        str = "三相正向有功平衡0.8C功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_a_044";
//                                        str = "三相正向有功平衡0.8C功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_a_078";
//                                        str = "三相正向有功平衡0.8C功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.02Ib")
//                                    {
//                                        text2 = "02003_a_045";
//                                        str = "三相正向有功平衡0.8C功率0.02In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                                else if (text15 == "A元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_a_046";
//                                        str = "三相正向有功不平衡A相1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_a_047";
//                                        str = "三相正向有功不平衡A相1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_a_090";
//                                        str = "三相正向有功不平衡A相1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_a_048";
//                                        str = "三相正向有功不平衡A相1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_a_049";
//                                        str = "三相正向有功不平衡A相0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_a_050";
//                                        str = "三相正向有功不平衡A相0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_a_093";
//                                        str = "三相正向有功不平衡A相0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_a_051";
//                                        str = "三相正向有功不平衡A相0.5L功率0.1In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                                else if (text15 == "B元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_a_052";
//                                        str = "三相正向有功不平衡B相1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_a_053";
//                                        str = "三相正向有功不平衡B相1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_a_096";
//                                        str = "三相正向有功不平衡B相1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_a_054";
//                                        str = "三相正向有功不平衡B相1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_a_055";
//                                        str = "三相正向有功不平衡B相0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_a_056";
//                                        str = "三相正向有功不平衡B相0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_a_099";
//                                        str = "三相正向有功不平衡B相0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_a_057";
//                                        str = "三相正向有功不平衡B相0.5L功率0.1In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                                else if (text15 == "C元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_a_058";
//                                        str = "三相正向有功不平衡C相1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_a_059";
//                                        str = "三相正向有功不平衡C相1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_a_102";
//                                        str = "三相正向有功不平衡C相1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_a_060";
//                                        str = "三相正向有功不平衡C相1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_a_061";
//                                        str = "三相正向有功不平衡C相0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_a_062";
//                                        str = "三相正向有功不平衡C相0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_a_105";
//                                        str = "三相正向有功不平衡C相0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_a_063";
//                                        str = "三相正向有功不平衡C相0.5L功率0.1In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                            }
//                            else if (meterError.Me_Glfx == "反向有功")
//                            {
//                                text2 = "02003_b";
//                                text9 = text9 + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                                if (text15 == "合元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_b_067";
//                                        str = "三相反向有功平衡1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_b_112";
//                                        str = "三相反向有功平衡1.0功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_b_068";
//                                        str = "三相反向有功平衡1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_b_113";
//                                        str = "三相反向有功平衡1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_b_069";
//                                        str = "三相反向有功平衡1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.02Ib")
//                                    {
//                                        text2 = "02003_b_103";
//                                        str = "三相反向有功平衡1.0功率0.02In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.01Ib")
//                                    {
//                                        text2 = "02003_b_070";
//                                        str = "三相反向有功平衡1.0功率0.01In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_b_071";
//                                        str = "三相反向有功平衡0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_b_115";
//                                        str = "三相反向有功平衡0.5L功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_b_072";
//                                        str = "三相反向有功平衡0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_b_116";
//                                        str = "三相反向有功平衡0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_b_073";
//                                        str = "三相反向有功平衡0.5L功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_b_107";
//                                        str = "三相反向有功平衡0.5L功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.02Ib")
//                                    {
//                                        text2 = "02003_b_074";
//                                        str = "三相反向有功平衡0.5L功率0.02In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_b_075";
//                                        str = "三相反向有功平衡0.8C功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_b_118";
//                                        str = "三相反向有功平衡0.8C功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_b_076";
//                                        str = "三相反向有功平衡0.8C功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_b_119";
//                                        str = "三相反向有功平衡0.8C功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_b_077";
//                                        str = "三相反向有功平衡0.8C功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_b_111";
//                                        str = "三相反向有功平衡0.8C功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.8C" && meterError.Me_dblxIb == "0.02Ib")
//                                    {
//                                        text2 = "02003_b_078";
//                                        str = "三相反向有功平衡0.8C功率0.02In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                                else if (text15 == "A元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_b_079";
//                                        str = "三相反向有功不平衡A相1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_b_080";
//                                        str = "三相反向有功不平衡A相1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_b_123";
//                                        str = "三相反向有功不平衡A相1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_b_081";
//                                        str = "三相反向有功不平衡A相1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_b_082";
//                                        str = "三相反向有功不平衡A相0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_b_083";
//                                        str = "三相反向有功不平衡A相0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_b_126";
//                                        str = "三相反向有功不平衡A相0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_b_084";
//                                        str = "三相反向有功不平衡A相0.5L功率0.1In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                                else if (text15 == "B元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_b_085";
//                                        str = "三相反向有功不平衡B相1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_b_086";
//                                        str = "三相反向有功不平衡B相1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_b_129";
//                                        str = "三相反向有功不平衡B相1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_b_087";
//                                        str = "三相反向有功不平衡B相1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_b_088";
//                                        str = "三相反向有功不平衡B相0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_b_089";
//                                        str = "三相反向有功不平衡B相0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_b_132";
//                                        str = "三相反向有功不平衡B相0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_b_090";
//                                        str = "三相反向有功不平衡B相0.5L功率0.1In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                                else if (text15 == "C元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_b_091";
//                                        str = "三相反向有功不平衡C相1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_b_092";
//                                        str = "三相反向有功不平衡C相1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_b_135";
//                                        str = "三相反向有功不平衡C相1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_b_093";
//                                        str = "三相反向有功不平衡C相1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_b_094";
//                                        str = "三相反向有功不平衡C相0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_b_095";
//                                        str = "三相反向有功不平衡C相0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_b_138";
//                                        str = "三相反向有功不平衡C相0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_b_096";
//                                        str = "三相反向有功不平衡C相0.5L功率0.1In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                            }
//                            else if (meterError.Me_Glfx == "正向无功")
//                            {
//                                text2 = "02003_c";
//                                text9 = text9 + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                                if (text15 == "合元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_c_100";
//                                        str = "三相正向无功平衡1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_c_129";
//                                        str = "三相正向无功平衡1.0功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_c_101";
//                                        str = "三相正向无功平衡1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_c_130";
//                                        str = "三相正向无功平衡1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_c_102";
//                                        str = "三相正向无功平衡1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.02Ib")
//                                    {
//                                        text2 = "02003_c_103";
//                                        str = "三相正向无功平衡1.0功率0.02In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_c_104";
//                                        str = "三相正向无功平衡0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_c_132";
//                                        str = "三相正向无功平衡0.5L功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_c_105";
//                                        str = "三相正向无功平衡0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_c_133";
//                                        str = "三相正向无功平衡0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_c_106";
//                                        str = "三相正向无功平衡0.5L功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_c_107";
//                                        str = "三相正向无功平衡0.5L功率0.05In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                                else if (text15 == "A元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_c_108";
//                                        str = "三相正向无功不平衡A相1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_c_109";
//                                        str = "三相正向无功不平衡A相1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_c_135";
//                                        str = "三相正向无功不平衡A相1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_c_110";
//                                        str = "三相正向无功不平衡A相1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_c_111";
//                                        str = "三相正向无功不平衡A相0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_c_112";
//                                        str = "三相正向无功不平衡A相0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_c_136";
//                                        str = "三相正向无功不平衡A相0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_c_113";
//                                        str = "三相正向无功不平衡A相0.5L功率0.1In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                                else if (text15 == "B元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_c_114";
//                                        str = "三相正向无功不平衡B相1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_c_115";
//                                        str = "三相正向无功不平衡B相1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_c_137";
//                                        str = "三相正向无功不平衡B相1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_c_116";
//                                        str = "三相正向无功不平衡B相1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_c_117";
//                                        str = "三相正向无功不平衡B相0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_c_118";
//                                        str = "三相正向无功不平衡B相0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_c_138";
//                                        str = "三相正向无功不平衡B相0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_c_119";
//                                        str = "三相正向无功不平衡B相0.5L功率0.1In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                                else if (text15 == "C元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_c_120";
//                                        str = "三相正向无功不平衡C相1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_c_121";
//                                        str = "三相正向无功不平衡C相1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_c_139";
//                                        str = "三相正向无功不平衡C相1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_c_122";
//                                        str = "三相正向无功不平衡C相1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_c_123";
//                                        str = "三相正向无功不平衡C相0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_c_124";
//                                        str = "三相正向无功不平衡C相0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_c_140";
//                                        str = "三相正向无功不平衡C相0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_c_125";
//                                        str = "三相正向无功不平衡C相0.5L功率0.1In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                            }
//                            else if (meterError.Me_Glfx == "反向无功")
//                            {
//                                text2 = "02003_d";
//                                text9 = text9 + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                                if (text15 == "合元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_d_129";
//                                        str = "三相反向无功平衡1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_d_163";
//                                        str = "三相反向无功平衡1.0功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_d_130";
//                                        str = "三相反向无功平衡1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_d_164";
//                                        str = "三相反向无功平衡1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_d_131";
//                                        str = "三相反向无功平衡1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.02Ib")
//                                    {
//                                        text2 = "02003_d_132";
//                                        str = "三相反向无功平衡1.0功率0.02In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_d_133";
//                                        str = "三相反向无功平衡0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.5Imax")
//                                    {
//                                        text2 = "02003_d_166";
//                                        str = "三相反向无功平衡0.5L功率0.5Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_d_134";
//                                        str = "三相反向无功平衡0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_d_167";
//                                        str = "三相反向无功平衡0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_d_135";
//                                        str = "三相反向无功平衡0.5L功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_d_136";
//                                        str = "三相反向无功平衡0.5L功率0.05In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                                else if (text15 == "A元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_d_137";
//                                        str = "三相反向无功不平衡A相1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_d_138";
//                                        str = "三相反向无功不平衡A相1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_d_169";
//                                        str = "三相反向无功不平衡A相1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_d_139";
//                                        str = "三相反向无功不平衡A相1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_d_140";
//                                        str = "三相反向无功不平衡A相0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_d_141";
//                                        str = "三相反向无功不平衡A相0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_d_170";
//                                        str = "三相反向无功不平衡A相0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_d_142";
//                                        str = "三相反向无功不平衡A相0.5L功率0.1In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                                else if (text15 == "B元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_d_143";
//                                        str = "三相反向无功不平衡B相1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_d_144";
//                                        str = "三相反向无功不平衡B相1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_d_171";
//                                        str = "三相反向无功不平衡B相1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_d_145";
//                                        str = "三相反向无功不平衡B相1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_d_146";
//                                        str = "三相反向无功不平衡B相0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_d_147";
//                                        str = "三相反向无功不平衡B相0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_d_172";
//                                        str = "三相反向无功不平衡B相0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_d_148";
//                                        str = "三相反向无功不平衡B相0.5L功率0.1In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                                else if (text15 == "C元")
//                                {
//                                    if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_d_149";
//                                        str = "三相反向无功不平衡C相1.0功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_d_150";
//                                        str = "三相反向无功不平衡C相1.0功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_d_173";
//                                        str = "三相反向无功不平衡C相1.0功率0.1In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "1.0" && meterError.Me_dblxIb == "0.05Ib")
//                                    {
//                                        text2 = "02003_d_151";
//                                        str = "三相反向无功不平衡C相1.0功率0.05In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "Imax")
//                                    {
//                                        text2 = "02003_d_152";
//                                        str = "三相反向无功不平衡C相0.5L功率Imax";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "1.0Ib")
//                                    {
//                                        text2 = "02003_d_153";
//                                        str = "三相反向无功不平衡C相0.5L功率In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.2Ib")
//                                    {
//                                        text2 = "02003_d_174";
//                                        str = "三相反向无功不平衡C相0.5L功率0.2In";
//                                    }
//                                    else if (meterError.Me_chrGlys == "0.5L" && meterError.Me_dblxIb == "0.1Ib")
//                                    {
//                                        text2 = "02003_d_154";
//                                        str = "三相反向无功不平衡C相0.5L功率0.1In";
//                                    }
//                                    else
//                                    {
//                                        text2 = "不存在";
//                                        str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                        {
//                                                    meterError.Me_Glfx,
//                                                    text15,
//                                                    meterError.Me_chrGlys,
//                                                    meterError.Me_dblxIb
//                                        });
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                text2 = "不存在";
//                                str = string.Format("字段编码中不存在{0:D}{1:D}{2:D}{3:D}的编码", new object[]
//                                {
//                                            meterError.Me_Glfx,
//                                            text15,
//                                            meterError.Me_chrGlys,
//                                            meterError.Me_dblxIb
//                                });
//                            }
//                            text9 = text9 + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                            text9 = text9 + "<C N=\"COL_NAME\">" + str + "</C>";
//                            text2 = "01";
//                            text9 = text9 + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                            text9 += "<C N=\"STD_CODE\"/>";
//                            text9 += "<C N=\"NUM_PRECISION\">1</C>";
//                            text2 = "YYYY-MM-DD";
//                            text9 = text9 + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                            text2 = array4[3];
//                            text9 = text9 + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//                            text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                            text9 = text9 + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                            text9 += "</R>";
//                        }
//                    }
//                }
//            }
//            text += text9;
//            string key = "1031";
//            if (meter.MeterResults.ContainsKey(key))
//            {
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02003_a";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02003_a_158";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "正向有功试验结论";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text2 = ((meter.MeterResults[key].Mr_chrRstValue == "合格") ? "0" : "1");
//                text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//            }
//            key = "1032";
//            if (meter.MeterResults.ContainsKey(key))
//            {
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02003_a";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02003_a_159";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "反向有功试验结论";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text2 = ((meter.MeterResults[key].Mr_chrRstValue == "合格") ? "0" : "1");
//                text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//            }
//            key = "1033";
//            if (meter.MeterResults.ContainsKey(key))
//            {
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02003_a";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02003_a_160";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "正向无功试验结论";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text2 = ((meter.MeterResults[key].Mr_chrRstValue == "合格") ? "0" : "1");
//                text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//            }
//            key = "1034";
//            if (meter.MeterResults.ContainsKey(key))
//            {
//                text += "<R>";
//                text2 = meter.Mb_chrOther2;
//                text = text + "<C N=\"TASK_EQUIP_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_chrOther1;
//                text = text + "<C N=\"TASK_ID\">" + text2 + "</C>";
//                text2 = meter.Mb_ChrTxm;
//                text = text + "<C N=\"ASSET_NO\">" + text2 + "</C>";
//                text2 = "02003_a";
//                text = text + "<C N=\"GROUP_CODE\">" + text2 + "</C>";
//                text2 = "02003_a_161";
//                text = text + "<C N=\"COL_CODE\">" + text2 + "</C>";
//                text2 = "反向无功试验结论";
//                text = text + "<C N=\"COL_NAME\">" + text2 + "</C>";
//                text2 = "01";
//                text = text + "<C N=\"COL_TYPE\">" + text2 + "</C>";
//                text += "<C N=\"STD_CODE\"/>";
//                text += "<C N=\"NUM_PRECISION\"/>";
//                text2 = "YYYY-MM-DD";
//                text = text + "<C N=\"DATE_FORMAT\">" + text2 + "</C>";
//                text2 = ((meter.MeterResults[key].Mr_chrRstValue == "合格") ? "0" : "1");
//                text = text + "<C N=\"DEFAULT_VALUE\">" + text2 + "</C>";
//                text2 = string.Format("{0:d4}-{1:d2}-{2:d2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
//                text = text + "<C N=\"OPER_DATE\">" + text2 + "</C>";
//                text += "</R>";
//            }
//        }
//        text += "</DBSET></C></R></DBSET>";
//        string[] @params = new string[]
//        {
//                    "path:mrms/interfaces/stdinf/inf/service/PdaService",
//                    "methodName:SET_TEST_DATA_FOR_STD",
//                    "dataXmlStr:" + text
//        };
//        string text16 = string.Empty;
//        XmlDocument xmlDocument = new XmlDocument();
//        text16 = OraDataHelper.m_webSev.ExeMethod("invokeService", array, @params, "ns1:invokeServiceResponse");
//        xmlDocument.LoadXml(text16);
//        XmlNodeList childNodes = xmlDocument.DocumentElement.ChildNodes;
//        foreach (object obj in childNodes)
//        {
//            XmlElement xmlElement = (XmlElement)obj;
//            foreach (object obj2 in xmlElement)
//            {
//                XmlElement xmlElement2 = (XmlElement)obj2;
//                if (xmlElement2.GetAttribute("N").Equals("RTN_FLAG"))
//                {
//                    if (xmlElement2.InnerText != "1")
//                    {
//                        strReturnMessage = text16;
//                        return false;
//                    }
//                }
//            }
//        }
//        result = true;
//    }
//    catch (Exception ex)
//    {
//        LogInfoHelper.Write(ex);
//        GlobalUnit.g_MsgControl.OutMessage("上传失败，错误原因:" + ex.Message, false);
//        result = false;
//    }
//    return result;
//}

//// Token: 0x060009B1 RID: 2481 RVA: 0x00067DAC File Offset: 0x00065FAC
//public static bool UpdateMeterInfoToDongRuanSG186(MeterBasicInfo meter, string meter_id, out string strReturnMessage)
//{
//    strReturnMessage = "";
//    GlobalUnit.g_MsgControl.OutMessage(string.Format("开始上传[{0}]", meter.Mb_ChrCcbh), false);
//    string mb_ChrTxm = "";
//    if (meter.Mb_ChrCcbh != null && meter.Mb_ChrCcbh != "")
//    {
//        if (!DataManager.GetMeterIdFromSG186ByMeterCcbh(meter, out meter_id, out mb_ChrTxm))
//        {
//            strReturnMessage = string.Format("电能表[{0}]meter_id获取失败!", meter.Mb_ChrCcbh);
//            return false;
//        }
//        meter.Mb_ChrTxm = mb_ChrTxm;
//    }
//    else
//    {
//        if (meter.Mb_ChrTxm == null || !(meter.Mb_ChrTxm != ""))
//        {
//            strReturnMessage = string.Format("电能表[{0}]meter_id获取失败!", meter.Mb_ChrCcbh);
//            return false;
//        }
//        if (!DataManager.GetMeterIdFromSG186ByMeterTxm(meter, out meter_id))
//        {
//            strReturnMessage = string.Format("电能表[{0}]meter_id失败!", meter.Mb_ChrCcbh);
//            return false;
//        }
//    }
//    long readID = OraDataHelper.GetReadID("D_METER_DETECT");
//    bool result;
//    if (readID == 0L)
//    {
//        strReturnMessage = string.Format("电能表[{0}]检定记录上传失败!", meter.Mb_ChrCcbh);
//        result = false;
//    }
//    else
//    {
//        OraDataHelper.DeleteBeforeUpdateInfoForDongRuanSG186(meter, meter_id, readID);
//        OraDataHelper.GetBeforeUpdateInfoForDongRuanSG186(meter);
//        CheckRecordsForDongRuanSG186 ckRecords;
//        DataManager.ProgressMeterCheckRecode(meter, meter_id, out ckRecords);
//        if (!OraDataHelper.UpDataCheckRecords(ckRecords, readID))
//        {
//            strReturnMessage = string.Format("电能表[{0}]检定记录上传失败!", meter.Mb_ChrCcbh);
//            result = false;
//        }
//        else
//        {
//            Dgn_CheckRecordsForDongRuanSG186 dgnRecords;
//            DataManager.ProgressMeterDgnCheckRecords(meter, meter_id, out dgnRecords);
//            if (!OraDataHelper.UpDataDgnCheckResult(dgnRecords, readID))
//            {
//                strReturnMessage = string.Format("电能表[{0}]多功能检定项目上传失败!", meter.Mb_ChrCcbh);
//                result = false;
//            }
//            else
//            {
//                CheckResultForDongRuanSG186 ckResult;
//                DataManager.ProgressMeterResult(meter, meter_id, out ckResult);
//                if (!OraDataHelper.UpDataCheckResult(ckResult, readID))
//                {
//                    strReturnMessage = string.Format("电能表[{0}]检定结论上传失败!", meter.Mb_ChrCcbh);
//                    result = false;
//                }
//                else
//                {
//                    CheckErrForDongRuanSG186[] ckErr;
//                    DataManager.ProgressMeterErr(meter, meter_id, out ckErr);
//                    if (!OraDataHelper.UpDataCheckErr(ckErr, readID))
//                    {
//                        strReturnMessage = string.Format("电能表[{0}]检定误差上传失败!", meter.Mb_ChrCcbh);
//                        result = false;
//                    }
//                    else
//                    {
//                        readID = OraDataHelper.GetReadID("D_METER_DIGIT_WALK");
//                        ZZ_CheckRecordsForDongRuanSG186[] zzRecords;
//                        DataManager.ProgressMeterZZ(meter, meter_id, out zzRecords);
//                        if (!OraDataHelper.UpDataCheckZZRecords(zzRecords, readID))
//                        {
//                            strReturnMessage = string.Format("电能表[{0}]走字记录上传失败!", meter.Mb_ChrCcbh);
//                            result = false;
//                        }
//                        else
//                        {
//                            ZZ_CheckRegisterForDongRuanSG186[] registerRecords;
//                            DataManager.ProgressMeterRegister(meter, meter_id, out registerRecords);
//                            if (!OraDataHelper.UpDataCheckRegisterRecords(registerRecords, readID))
//                            {
//                                strReturnMessage = "上传失败[电能表走字计度器].....";
//                                result = false;
//                            }
//                            else
//                            {
//                                result = true;
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }
//    return result;
//}

//// Token: 0x060009B2 RID: 2482 RVA: 0x00068020 File Offset: 0x00066220
//public static void GetBeforeUpdateInfoForDongRuanSG186(MeterBasicInfo meter)
//{
//    bool flag = false;
//    string sqlstring;
//    if (GlobalUnit.MisInterFaceType.Contains("新疆"))
//    {
//        sqlstring = string.Format("select * from SA_ORG where DEPT_NAME='{0}'", meter.Mb_chrSjdw);
//    }
//    else
//    {
//        sqlstring = string.Format("select * from SA_ORG where ORG_NAME='{0}'", meter.Mb_chrSjdw);
//    }
//    DataSet dataSet = OraHelper.Query(sqlstring);
//    List<string> values = GlobalUnit.g_SystemConfig.ZiDianGroup.getValues("班组名称");
//    if (dataSet.Tables.Count > 0)
//    {
//        DataTable dataTable = dataSet.Tables[0];
//        if (dataTable.Rows.Count > 0)
//        {
//            if (GlobalUnit.MisInterFaceType.Contains("新疆"))
//            {
//                meter.Mb_chrSjdwNo = dataTable.Rows[0]["DEPT_ID"].ToString().Trim();
//            }
//            else
//            {
//                meter.Mb_chrSjdwNo = dataTable.Rows[0]["ORG_NO"].ToString().Trim();
//            }
//            foreach (string arg in values)
//            {
//                if (GlobalUnit.MisInterFaceType.Contains("新疆"))
//                {
//                    sqlstring = string.Format("select * from sa_dept where DEPT_ID='{0}' and dept_name='{1}'", meter.Mb_chrSjdwNo, arg);
//                }
//                else
//                {
//                    sqlstring = string.Format("select * from sa_dept where org_no='{0}' and org_name='{1}'", meter.Mb_chrSjdwNo, arg);
//                }
//                dataSet = OraHelper.Query(sqlstring);
//                if (dataSet.Tables.Count > 0)
//                {
//                    dataTable = dataSet.Tables[0];
//                    if (dataTable.Rows.Count > 0)
//                    {
//                        string arg2 = dataTable.Rows[0]["DEPT_ID"].ToString().Trim();
//                        sqlstring = string.Format("select * from sa_user where real_name like '%{0}%' and DEPT_ID='{1}'", meter.Mb_ChrJyy, arg2);
//                        dataSet = OraHelper.Query(sqlstring);
//                        if (dataSet.Tables.Count > 0)
//                        {
//                            dataTable = dataSet.Tables[0];
//                            if (dataTable.Rows.Count > 0)
//                            {
//                                meter.Mb_ChrJyyNo = dataTable.Rows[0]["USER_ID"].ToString().Trim();
//                                flag = true;
//                            }
//                        }
//                        sqlstring = string.Format("select * from sa_user where real_name like '%{0}%' and DEPT_ID='{1}'", meter.Mb_ChrHyy, arg2);
//                        dataSet = OraHelper.Query(sqlstring);
//                        if (dataSet.Tables.Count > 0)
//                        {
//                            dataTable = dataSet.Tables[0];
//                            if (dataTable.Rows.Count > 0)
//                            {
//                                meter.Mb_ChrHyyNo = dataTable.Rows[0]["USER_ID"].ToString().Trim();
//                                flag = true;
//                            }
//                        }
//                        sqlstring = string.Format("select * from sa_user where real_name like '%{0}%' and DEPT_ID='{1}'", meter.Mb_chrZhuGuan, arg2);
//                        dataSet = OraHelper.Query(sqlstring);
//                        if (dataSet.Tables.Count > 0)
//                        {
//                            dataTable = dataSet.Tables[0];
//                            if (dataTable.Rows.Count > 0)
//                            {
//                                meter.Mb_chrZhuGuanNo = dataTable.Rows[0]["USER_ID"].ToString().Trim();
//                                flag = true;
//                            }
//                        }
//                        if (flag)
//                        {
//                            break;
//                        }
//                    }
//                }
//            }
//        }
//    }
//}

//// Token: 0x060009B3 RID: 2483 RVA: 0x000683E4 File Offset: 0x000665E4
//public static void DeleteBeforeUpdateInfoForDongRuanSG186(MeterBasicInfo meter, string meterid, long readid)
//{
//    string sqlstring = string.Format("delete from d_md_volt_test where EQUIP_ID='{0}'", meterid);
//    OraHelper.Query(sqlstring);
//    sqlstring = string.Format("delete from D_REGISTER_INIT_DIGIT_WALK where METER_ID='{0}'", meterid);
//    OraHelper.Query(sqlstring);
//    sqlstring = string.Format("delete from D_REGISTER_DIGIT_WALK where METER_ID='{0}'", meterid);
//    OraHelper.Query(sqlstring);
//    sqlstring = string.Format("delete from D_METER_DIGIT_WALK where METER_ID='{0}'", meterid);
//    OraHelper.Query(sqlstring);
//    sqlstring = string.Format("delete from D_MULTFUNC_DETECT where METER_ID={0}", meterid);
//    OraHelper.Query(sqlstring);
//    sqlstring = string.Format("delete from D_METER_DETECT_CONC where METER_ID={0}", meterid);
//    OraHelper.Query(sqlstring);
//    sqlstring = string.Format("delete from D_METER_ERR where read_id={0}", readid);
//    OraHelper.Query(sqlstring);
//    sqlstring = string.Format("delete from D_METER_DETECT where meter_id={0}", meterid);
//    OraHelper.Query(sqlstring);
//}

//// Token: 0x060009B4 RID: 2484 RVA: 0x00068498 File Offset: 0x00066698
//public static bool UpDataCheckRecords(CheckRecordsForDongRuanSG186 ckRecords, long readID)
//{
//    string sqlstring = string.Format("insert into D_METER_DETECT values({0},{1},'{2}','{3}',{4},'{5}',{6},'{7}',{8},'{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}',{18},'{19}','{20}','{21}','{22}')", new object[]
//    {
//                readID,
//                DateTime.Now.ToString("yyyyMMdd"),
//                ckRecords.app_no,
//                ckRecords.chk_no,
//                ckRecords.meter_id,
//                ckRecords.CHECKER_NAME,
//                ckRecords.chk_date,
//                ckRecords.checker_no,
//                ckRecords.chk_rec_date,
//                ckRecords.equip_cert_no,
//                ckRecords.chk_desk_no,
//                ckRecords.meter_loc,
//                ckRecords.temp,
//                ckRecords.humidity,
//                ckRecords.chk_basis,
//                ckRecords.chk_const,
//                ckRecords.chk_conc,
//                ckRecords.cert_id,
//                ckRecords.chk_valid_date,
//                ckRecords.chk_remark,
//                ckRecords.org_no,
//                ckRecords.Recipient_name,
//                ckRecords.org_company
//    });
//    bool result;
//    if (OraHelper.ExecuteSql(sqlstring) <= 0)
//    {
//        result = false;
//    }
//    else
//    {
//        if (ckRecords.chk_conc == "1")
//        {
//            sqlstring = string.Format("Update D_Meter set detect='01' where meter_id='{0}' and detect<>'09'", ckRecords.meter_id);
//        }
//        else
//        {
//            sqlstring = string.Format("Update D_Meter set detect='02' where meter_id='{0}' and detect<>'09'", ckRecords.meter_id);
//        }
//        OraHelper.ExecuteSql(sqlstring);
//        result = true;
//    }
//    return result;
//}

//// Token: 0x060009B5 RID: 2485 RVA: 0x00068624 File Offset: 0x00066824
//public static bool UpDataCheckRecords(CheckBasicDataForTianJinMis ckRecords)
//{
//    string sqlstring = string.Format("update sf_dnbxy_sj set xylx='{0}',Tempture='{1}',Humiture='{2}',Starttest='{3}',Undercur='{4}',Xyrq=to_date('{5}','yyyy-MM-dd HH24:mi:ss'),Xyry='{6}',Hyry='{7}',Xyjl='{8}',Bjlx='{9}',BZSB='{10}'where Txbm = '{11}'", new object[]
//    {
//                ckRecords.xylx.Trim(),
//                ckRecords.Tempture.Trim(),
//                ckRecords.Humiture.Trim(),
//                ckRecords.Starttest.Trim(),
//                ckRecords.Undercur.Trim(),
//                ckRecords.Xyrq,
//                ckRecords.Xyry.Trim(),
//                ckRecords.Hyry.Trim(),
//                ckRecords.Xyjl.Trim(),
//                ckRecords.Bjlx.Trim(),
//                ckRecords.BZSB.Trim(),
//                ckRecords.Txbm.Trim()
//    });
//    return OraHelper.ExecuteSql(sqlstring) > 0;
//}

//// Token: 0x060009B6 RID: 2486 RVA: 0x00068704 File Offset: 0x00066904
//public static bool UpDataDgnCheckResult(Dgn_CheckRecordsForDongRuanSG186 dgnRecords, long readID)
//{
//    string sqlstring = string.Format("insert into D_MULTFUNC_DETECT values(SEQ_D_MULTFUNC_DETECT.NEXTVAL,{1},{2},'{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}','{51}','{52}','{53}','{54}','{55}')", new object[]
//    {
//                dgnRecords.id,
//                readID,
//                dgnRecords.meter_id,
//                dgnRecords.ar_ts_chk,
//                dgnRecords.TS_CHK_CONC_CODE,
//                dgnRecords.DAILY_TIMING_ERR1,
//                dgnRecords.DAILY_TIMING_ERR2,
//                dgnRecords.DAILY_TIMING_ERR3,
//                dgnRecords.DAILY_TIMING_ERR4,
//                dgnRecords.DAILY_TIMING_ERR5,
//                dgnRecords.DAILY_TIMING_ERR6,
//                dgnRecords.DAILY_TIMING_ERR7,
//                dgnRecords.DAILY_TIMING_ERR8,
//                dgnRecords.DAILY_TIMING_ERR9,
//                dgnRecords.DAILY_TIMING_ERR10,
//                dgnRecords.DAILY_TIMING_ERR_AVG,
//                dgnRecords.DAILY_TIMING_ERR_INT,
//                dgnRecords.DE_STD_IMAX,
//                dgnRecords.DE_IMAX,
//                dgnRecords.DEMAND_READING_ERR,
//                dgnRecords.DE_INT_IMAX,
//                dgnRecords.DE_STD_IB,
//                dgnRecords.DE_IB_ACT,
//                dgnRecords.DE_IB,
//                dgnRecords.DE_IB_INT,
//                dgnRecords.DE_P1IB_STD,
//                dgnRecords.DE_P1IB_ACT,
//                dgnRecords.DE_P1IB,
//                dgnRecords.DE_P1IB_INT,
//                dgnRecords.SEL_PERIOD,
//                dgnRecords.DMD_PERIOD_IB,
//                dgnRecords.DE_PERIOD_IB,
//                dgnRecords.DE_PERIOD_IB_INT,
//                dgnRecords.BF_PQ,
//                dgnRecords.AF_PQ,
//                dgnRecords.CONC,
//                dgnRecords.BF_PQ_U100T20MS,
//                dgnRecords.AF_PQ_U100T20MS,
//                dgnRecords.CONC_U100T20MS,
//                dgnRecords.BF_PQ_U50T1M,
//                dgnRecords.AF_PQ_U50T1M,
//                dgnRecords.CONCLUSION_U50T1M,
//                dgnRecords.CI_CHK_CONC_CODE,
//                dgnRecords.RP_MEMORY_CHK,
//                dgnRecords.OTHER_MEMORY_CHK,
//                dgnRecords.GPS_CALIBRATE_FLAG,
//                dgnRecords.TS_ERR_CONC_CODE,
//                dgnRecords.CHANGE1_TYPE,
//                dgnRecords.CHANGE2_TYPE,
//                dgnRecords.CHANGE1_ERR,
//                dgnRecords.CHANGE2_ERR,
//                dgnRecords.INT_CHANGE1_ERR,
//                dgnRecords.INT_CHANGE2_ERR,
//                dgnRecords.DAILY_TIMING_ERR_CONC,
//                dgnRecords.DE_ERR_CONC,
//                dgnRecords.DMD_PERIOD_ERR_CONC
//    });
//    return OraHelper.ExecuteSql(sqlstring) > 0;
//}

//public static bool UpDataCheckErr(CheckErrForTianJinMis[] ckErr)
//{
//    bool result;
//    if (ckErr == null || ckErr.Length <= 0)
//    {
//        result = true;
//    }
//    else
//    {
//        for (int i = 0; i < ckErr.Length; i++)
//        {
//            string sqlstring = string.Format("insert into sf_dnbxy_jg values('{0}','{1}',to_date('{2}','yyyy-MM-dd HH24:mi:ss'),'{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')", new object[]
//            {
//                        ckErr[i].Txbm.Trim(),
//                        ckErr[i].Ccbh.Trim(),
//                        ckErr[i].Xyrq,
//                        ckErr[i].Error1.Trim(),
//                        ckErr[i].Error2.Trim(),
//                        ckErr[i].Chaninct.Trim(),
//                        ckErr[i].Balance.Trim(),
//                        ckErr[i].Dlfd.Trim(),
//                        ckErr[i].Dyfd.Trim(),
//                        ckErr[i].Xw.Trim(),
//                        ckErr[i].Bz.Trim(),
//                        ckErr[i].Xylx.Trim(),
//                        ckErr[i].Xx.Trim(),
//                        ckErr[i].JLDH.Trim()
//            });
//            if (OraHelper.ExecuteSql(sqlstring) <= 0)
//            {
//                return false;
//            }
//        }
//        result = true;
//    }
//    return result;
//}

//// Token: 0x060009BA RID: 2490 RVA: 0x00068D54 File Offset: 0x00066F54
//public static bool UpDataCheckZZRecords(ZZ_CheckRecordsForDongRuanSG186[] zzRecords, long readID)
//{
//    bool result;
//    if (zzRecords == null || zzRecords.Length <= 0)
//    {
//        result = true;
//    }
//    else
//    {
//        string text = "";
//        string sqlstring = string.Format("insert into D_METER_DIGIT_WALK values({0},{1},'{2}','{3}',{4},'{5}',{6},'{7}',{8},'{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}')", new object[]
//        {
//                    readID,
//                    zzRecords[0].send_sn,
//                    zzRecords[0].walk_no,
//                    zzRecords[0].app_no,
//                    zzRecords[0].meter_id,
//                    zzRecords[0].RUNING_PERSON_NAME,
//                    zzRecords[0].RUNING_DATE,
//                    zzRecords[0].checker_no,
//                    zzRecords[0].chk_rec_date,
//                    zzRecords[0].runing_desk_no,
//                    zzRecords[0].temp,
//                    zzRecords[0].humidity,
//                    zzRecords[0].pointer_type_code,
//                    zzRecords[0].STD_READING_AVG,
//                    zzRecords[0].STD_RELATIVE_ERR,
//                    zzRecords[0].comp_err,
//                    zzRecords[0].TIME_CALIBRATE_FLAG,
//                    zzRecords[0].conc_code,
//                    zzRecords[0].runing_remark,
//                    zzRecords[0].org_no
//        });
//        string meter_id = zzRecords[0].meter_id;
//        if (zzRecords[0].conc_code.CompareTo("0") == 0 || text.CompareTo("02") == 0)
//        {
//            text = "02";
//        }
//        else
//        {
//            text = "01";
//        }
//        if (OraHelper.ExecuteSql(sqlstring) <= 0)
//        {
//            result = false;
//        }
//        else
//        {
//            sqlstring = string.Format("Update D_Meter set DIGIT_WALK='{0}' where meter_id='{1}'", text, meter_id);
//            result = (OraHelper.ExecuteSql(sqlstring) > 0);
//        }
//    }
//    return result;
//}


//// Token: 0x060009BD RID: 2493 RVA: 0x00069360 File Offset: 0x00067560
//public static bool UpDataCheckVoltRecords(CheckVoltForDongRuanSG186 voltRecords, long readID)
//{
//    string text = "";
//    string sqlstring = string.Format("insert into D_MD_VOLT_TEST values({0},'{1}','{2}','{3}',{4},'{5}',{6},{7},{8},'{9}','{10}',{11},'{12}',{13})", new object[]
//    {
//                readID,
//                voltRecords.app_no,
//                voltRecords.WITHSTAND_VOLT_NO,
//                voltRecords.EQUIP_CATEG,
//                voltRecords.EQUIP_ID,
//                voltRecords.VOLT_DESK_NO,
//                voltRecords.TEMP,
//                voltRecords.HUMIDITY,
//                voltRecords.VOLT_TEST_VALUE,
//                voltRecords.VOLT_CONC,
//                voltRecords.VOLT_CHK_PERSON_NO,
//                voltRecords.VOLT_DATE,
//                voltRecords.CHECKER_NO,
//                voltRecords.CHK_DATE
//    });
//    bool result;
//    if (OraHelper.ExecuteSql(sqlstring) <= 0)
//    {
//        result = false;
//    }
//    else
//    {
//        string equip_ID = voltRecords.EQUIP_ID;
//        if (voltRecords.VOLT_CONC.CompareTo("0") == 0 || text.CompareTo("02") == 0)
//        {
//            text = "02";
//        }
//        else
//        {
//            text = "01";
//        }
//        sqlstring = string.Format("Update D_Meter set volt_test='{0}' where meter_id='{1}'", text, equip_ID);
//        result = (OraHelper.ExecuteSql(sqlstring) > 0);
//    }
//    return result;
//}

//// Token: 0x060009BE RID: 2494 RVA: 0x000694B4 File Offset: 0x000676B4
//public static bool UpdateMeterInfoToPuHuaSG186(MeterBasicInfo meterinfo, string meter_id, out string strReturnMessage)
//{
//    strReturnMessage = "";
//    CheckBasicDataForPuHuaSG186 basicData = DataManager.ProgressMeterCheckBasicData(meterinfo, meter_id);
//    bool result;
//    if (!OraDataHelper.UpDataBasicData(basicData))
//    {
//        strReturnMessage = "上传失败[电能表基础数据].....";
//        result = false;
//    }
//    else
//    {
//        long readID = OraDataHelper.GetReadID();
//        if (readID == 0L)
//        {
//            strReturnMessage = "上传失败[获取ReadID].....";
//            result = false;
//        }
//        else
//        {
//            CheckRecordsForPuHuaSG186 ckRecords;
//            DataManager.ProgressMeterCheckRecode(meterinfo, meter_id, out ckRecords);
//            ckRecords.meter_id = meter_id.ToString();
//            if (!OraDataHelper.UpDataCheckRecords(ckRecords, readID))
//            {
//                strReturnMessage = "上传失败[电能表检定记录].....";
//                result = false;
//            }
//            else
//            {
//                Dgn_CheckRecordsForPuHuaSG186 dgnRecords;
//                DataManager.ProgressMeterDgnCheckRecords(meterinfo, meter_id, out dgnRecords);
//                if (!OraDataHelper.UpDataDgnCheckResult(dgnRecords, readID))
//                {
//                    strReturnMessage = "上传失败[电能表检定多功能检定项目].....";
//                    result = false;
//                }
//                else
//                {
//                    CheckResultForPuHuaSG186 ckResult;
//                    DataManager.ProgressMeterResult(meterinfo, meter_id, out ckResult);
//                    if (!OraDataHelper.UpDataCheckResult(ckResult, readID))
//                    {
//                        strReturnMessage = "上传失败[电能表检定结论].....";
//                        result = false;
//                    }
//                    else
//                    {
//                        CheckErrForPuHuaSG186[] ckErr;
//                        DataManager.ProgressMeterErr(meterinfo, meter_id, out ckErr);
//                        if (!OraDataHelper.UpDataCheckErr(ckErr, readID))
//                        {
//                            strReturnMessage = "上传失败[电能表检定误差].....";
//                            result = false;
//                        }
//                        else
//                        {
//                            ZZ_CheckRecordsForPuHuaSG186[] zzRecords;
//                            DataManager.ProgressMeterZZ(meterinfo, meter_id, out zzRecords);
//                            if (!OraDataHelper.UpDataCheckZZRecords(zzRecords))
//                            {
//                                strReturnMessage = "上传失败[电能表走字记录].....";
//                                result = false;
//                            }
//                            else
//                            {
//                                result = true;
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }
//    return result;
//}

//// Token: 0x060009BF RID: 2495 RVA: 0x000695E0 File Offset: 0x000677E0
//public static bool UpDataBasicData(CheckBasicDataForPuHuaSG186 basicData)
//{
//    string sqlstring = string.Format("insert into PUB_DETECT_TSK values(SEQ_PUB_DETECT_TSK.NEXTVAL,'{1}','{2}','{3}',(select asset_no from d_meter where bar_code='{5}'),'{5}','{6}','{7}','{8}',to_date('{9}','YYYY-MM-DD'),'{10}','{11}','{12}',to_date('{13}','YYYY-MM-DD'),'{14}','{15}','{16}','{17}','{18}')", new object[]
//    {
//                basicData.Tsk_Id,
//                basicData.Mis_No,
//                basicData.App_No,
//                basicData.Equip_Id,
//                basicData.Asset_No,
//                basicData.Bar_Code,
//                basicData.Made_No,
//                basicData.Chk_Desk_No,
//                basicData.Checker_No,
//                basicData.Chk_Date,
//                basicData.Send_Sn,
//                basicData.Chk_Flag,
//                basicData.Load_Flag,
//                basicData.Chk_Date,
//                basicData.Manufacturer,
//                basicData.Volt_Code,
//                basicData.Rated_Current,
//                basicData.Model_Code,
//                basicData.Type_Code
//    });
//    return OraHelper.ExecuteSql(sqlstring) > 0;
//}

//// Token: 0x060009C0 RID: 2496 RVA: 0x000696C4 File Offset: 0x000678C4
//public static bool UpDataCheckRecords(CheckRecordsForPuHuaSG186 ckRecords, long readID)
//{
//    string sqlstring = string.Format("insert into PUB_METR_DET values({0},'{1}'||substr2(to_char(SEQ_PUB_METR_DET.Currval),length2(SEQ_PUB_METR_DET.Currval)-5,6),'{2}','{3}'||substr2(to_char(SEQ_PUB_METR_DET.Currval),length2(SEQ_PUB_METR_DET.Currval)-5,6),{4},'{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}')", new object[]
//    {
//                readID,
//                DateTime.Now.ToString("yyyyMMdd"),
//                ckRecords.app_no,
//                ckRecords.chk_no,
//                ckRecords.meter_id,
//                ckRecords.chk_type_c,
//                ckRecords.checker_nm,
//                ckRecords.chk_date,
//                ckRecords.checker_no,
//                ckRecords.chk_rec_dt,
//                ckRecords.equip_cert,
//                ckRecords.chk_deskno,
//                ckRecords.meter_loc,
//                ckRecords.temp,
//                ckRecords.humidity,
//                ckRecords.chk_basis,
//                ckRecords.chk_const,
//                ckRecords.chk_conc,
//                ckRecords.cert_id,
//                ckRecords.chk_validt,
//                ckRecords.chk_remark,
//                ckRecords.org_no,
//                ckRecords.Recipientn
//    });
//    return OraHelper.ExecuteSql(sqlstring) > 0;
//}

//// Token: 0x060009C1 RID: 2497 RVA: 0x000697FC File Offset: 0x000679FC
//public static bool UpDataDgnCheckResult(Dgn_CheckRecordsForPuHuaSG186 dgnRecords, long readID)
//{
//    string sqlstring = string.Format("insert into PUB_MULF_DET values(SEQ_PUB_MULF_DET.NEXTVAL,{1},{2},'{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}','{41}','{42}','{43}','{44}','{45}','{46}')", new object[]
//    {
//                dgnRecords.id,
//                readID,
//                dgnRecords.meter_id,
//                dgnRecords.ar_ts_chk,
//                dgnRecords.ts_chk_con,
//                dgnRecords.daily_err_1,
//                dgnRecords.daily_err_2,
//                dgnRecords.daily_err_3,
//                dgnRecords.daily_err_4,
//                dgnRecords.daily_err_5,
//                dgnRecords.daily_err_6,
//                dgnRecords.daily_err_7,
//                dgnRecords.daily_err_8,
//                dgnRecords.daily_err_9,
//                dgnRecords.daily_err_10,
//                dgnRecords.daily_erra,
//                dgnRecords.daily_erri,
//                dgnRecords.de_std_ima,
//                dgnRecords.de_imax,
//                dgnRecords.demand_err,
//                dgnRecords.de_int_ima,
//                dgnRecords.de_std_ib,
//                dgnRecords.de_ib_act,
//                dgnRecords.de_ib,
//                dgnRecords.de_ib_int,
//                dgnRecords.de_p1ib_st,
//                dgnRecords.de_p1ib_ac,
//                dgnRecords.de_p1ib,
//                dgnRecords.de_p1ib_in,
//                dgnRecords.sel_period,
//                dgnRecords.dmd_period,
//                dgnRecords.de_period,
//                dgnRecords.de_periodi,
//                dgnRecords.bf_pq,
//                dgnRecords.af_pq,
//                dgnRecords.conc,
//                dgnRecords.bf_pq_u100,
//                dgnRecords.af_pq_u100,
//                dgnRecords.conc_u100t,
//                dgnRecords.bf_pq_u50t,
//                dgnRecords.af_pq_u50t,
//                dgnRecords.conclusion,
//                dgnRecords.ci_chk_con,
//                dgnRecords.rp_memory,
//                dgnRecords.other_memo,
//                dgnRecords.gps_calibr,
//                dgnRecords.ts_err_con
//    });
//    return OraHelper.ExecuteSql(sqlstring) > 0;
//}

//// Token: 0x060009C2 RID: 2498 RVA: 0x00069A28 File Offset: 0x00067C28
//public static bool UpDataCheckResult(CheckResultForPuHuaSG186 ckResult, long readID)
//{
//    string sqlstring = string.Format("insert into PUB_METR_CON values(SEQ_PUB_METR_CON.NEXTVAL,{1},{2},'{3}',{4},'{5}','{6}','{7}','{8}',{9},{10},{11})", new object[]
//    {
//                ckResult.id,
//                readID,
//                ckResult.meter_id,
//                ckResult.both_way_p,
//                ckResult.start_conc,
//                ckResult.creep_conc,
//                ckResult.volt_conc,
//                ckResult.start_curr,
//                ckResult.start_date,
//                ckResult.volt_testv,
//                ckResult.err_upperl,
//                ckResult.err_lowerl
//    });
//    return OraHelper.ExecuteSql(sqlstring) > 0;
//}

//// Token: 0x060009C3 RID: 2499 RVA: 0x00069AE0 File Offset: 0x00067CE0
//public static bool UpDataCheckErr(CheckErrForPuHuaSG186[] ckErr, long readID)
//{
//    bool result;
//    if (ckErr == null || ckErr.Length <= 0)
//    {
//        result = true;
//    }
//    else
//    {
//        for (int i = 0; i < ckErr.Length; i++)
//        {
//            string sqlstring = string.Format("insert into PUB_METR_ERR values(SEQ_PUB_METR_ERR.NEXTVAL,{1},'{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}')", new object[]
//            {
//                        ckErr[i].id,
//                        readID,
//                        ckErr[i].chk_type_c,
//                        ckErr[i].pf,
//                        ckErr[i].load_currt,
//                        ckErr[i].both_way_p,
//                        ckErr[i].orgnstderr,
//                        ckErr[i].err1,
//                        ckErr[i].err2,
//                        ckErr[i].err3,
//                        ckErr[i].err4,
//                        ckErr[i].err5,
//                        ckErr[i].ave_err,
//                        ckErr[i].int_conver,
//                        ckErr[i].std_err_in,
//                        ckErr[i].load_stats,
//                        ckErr[i].test_group
//            });
//            if (OraHelper.ExecuteSql(sqlstring) <= 0)
//            {
//                return false;
//            }
//        }
//        result = true;
//    }
//    return result;
//}

//// Token: 0x060009C4 RID: 2500 RVA: 0x00069C6C File Offset: 0x00067E6C
//public static bool UpDataCheckZZRecords(ZZ_CheckRecordsForPuHuaSG186[] zzRecords)
//{
//    bool result;
//    if (zzRecords == null || zzRecords.Length <= 0)
//    {
//        result = true;
//    }
//    else
//    {
//        for (int i = 0; i < zzRecords.Length; i++)
//        {
//            string sqlstring = string.Format("insert into PUB_METR_WLK values(SEQ_PUB_METR_WLK.NEXTVAL,'{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}')", new object[]
//            {
//                        zzRecords[i].read_id,
//                        zzRecords[i].send_sn,
//                        zzRecords[i].walk_no,
//                        zzRecords[i].app_no,
//                        zzRecords[i].meter_id,
//                        zzRecords[i].runing_nam,
//                        zzRecords[i].runing_dat,
//                        zzRecords[i].checker_no,
//                        zzRecords[i].chk_rec_dt,
//                        zzRecords[i].runing_des,
//                        zzRecords[i].temp,
//                        zzRecords[i].humidity,
//                        zzRecords[i].pointer_ty,
//                        zzRecords[i].std_readav,
//                        zzRecords[i].std_relati,
//                        zzRecords[i].comp_err,
//                        zzRecords[i].time_calib,
//                        zzRecords[i].conc_code,
//                        zzRecords[i].runing_rem,
//                        zzRecords[i].org_no
//            });
//            if (OraHelper.ExecuteSql(sqlstring) <= 0)
//            {
//                return false;
//            }
//        }
//        result = true;
//    }
//    return result;
//}

//// Token: 0x060009C5 RID: 2501 RVA: 0x00069E34 File Offset: 0x00068034
//public static bool UpdateMeterInfoToPrductionSchedulingSystem(MeterBasicInfo meterInfo, out string strReturnMessage)
//{
//    strReturnMessage = "";
//    ArrayList arrayList = new ArrayList();
//    MT_METER mt_meterByBarcode = new MT_METERServices().GetMt_meterByBarcode(meterInfo.Mb_ChrTxm);
//    bool result;
//    if (mt_meterByBarcode == null)
//    {
//        result = false;
//    }
//    else
//    {
//        string detect_TASK_NO = mt_meterByBarcode.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        meterInfo.Mb_chrOther5 = detect_TASK_NO;
//        meterInfo.Mb_ChrTxm = meterInfo.Mb_ChrTxm.Trim();
//        if (OraDataHelper.g_DicPCodeTable == null)
//        {
//            OraDataHelper.GetDicPCodeTable();
//        }
//        if (GlobalUnit.MisInterFaceType == "")
//        {
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_BASICERR_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "' and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_MEASURE_REPEAT_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_STARTING_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_CREEPING_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_CONST_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_DAYERR_MET_CONC  where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_ESAM_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.mt_detect_met_rslt where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_PRESETPARAM_CHECK_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_PRESETPARAM_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_PASSWORD_CHANGE_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_CONSIST_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.Mt_OVERLOAD_MET_CONC  where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_TS_MET_CONC  where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_ERROR_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_VARIATION_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_DEMANDVALUE_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from bjxm3_result.MT_ESAM_SECURITY_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//        }
//        else
//        {
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_BASICERR_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "' and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_INTUIT_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_MEASURE_REPEAT_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_DEVIATION_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_STARTING_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_CREEPING_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_CONST_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_DAYERR_MET_CONC  where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_ESAM_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_DETECT_RSLT where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_PRESETPARAM_CHECK_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_PRESETPARAM_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_PASSWORD_CHANGE_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_CONSIST_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_DETECT_RSLT where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from Mt_OVERLOAD_MET_CONC  where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_TS_MET_CONC  where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_ERROR_MET_CONC where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_VOLT_MET_CONC  where  detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_VARIATION_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_DEMANDVALUE_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_ESAM_SECURITY_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_HUTCHISON_COMBINA_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_TIME_ERROR_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_INFLUENCE_QTY_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_GPS_TIMING where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_INFLUENCE_QTY_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_WAVE_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_WARNNING_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_EP_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_EC_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_ESAM_READ_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_PARA_SETTING_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_FEE_TMNL_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_POWER_MEASURE_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_POWER_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_STANDARD_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_MAX_DEMANDVALUE_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//            arrayList.Add(string.Concat(new string[]
//            {
//                        "delete from MT_FEE_MET_CONC where detect_task_no = '",
//                        detect_TASK_NO,
//                        "'and bar_code='",
//                        meterInfo.Mb_ChrTxm,
//                        "' "
//            }));
//        }
//        string[] array = OraDataHelper.GetMT_INTUIT_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        array = OraDataHelper.GetMT_DEMANDVALUE_MET_CONC(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        array = OraDataHelper.GetMT_STARTING_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        array = OraDataHelper.GetMT_CREEPING_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        array = OraDataHelper.GetMT_STANDARD_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        array = OraDataHelper.GetMT_BASICERR_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        array = OraDataHelper.GetMT_DEVIATION_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        array = OraDataHelper.GetMT_CONST_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        arrayList.Add(OraDataHelper.GetMT_DAYERR_MET_CONCByMt(mt_meterByBarcode, meterInfo));
//        array = OraDataHelper.GetMT_TS_MET_CONC(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        arrayList.Add(OraDataHelper.GetMT_ESAM_MET_CONCByMt(mt_meterByBarcode, meterInfo));
//        arrayList.Add(OraDataHelper.GetMT_ESAM_SECURITY_MET_CONCByMt(mt_meterByBarcode, meterInfo));
//        array = OraDataHelper.GetMT_CONSIST_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        array = OraDataHelper.GetMT_ERROR_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        array = OraDataHelper.GetMT_VARIATION_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        arrayList.Add(OraDataHelper.GetMT_OVERLOAD_MET_CONCByMt(mt_meterByBarcode, meterInfo));
//        arrayList.Add(OraDataHelper.GetMT_VOLT_MET_CONCByMt(mt_meterByBarcode, meterInfo));
//        arrayList.Add(OraDataHelper.GetMT_EQ_MET_CONCByMt(mt_meterByBarcode, meterInfo));
//        array = OraDataHelper.GetMT_HUTCHISON_COMBINA_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        array = OraDataHelper.GetMT_TIME_ERROR_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        array = OraDataHelper.GetMT_INFLUENCE_QTY_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        array = OraDataHelper.GetMT_WAVE_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//        if (array != null)
//        {
//            foreach (string value in array)
//            {
//                arrayList.Add(value);
//            }
//        }
//        arrayList.Add(OraDataHelper.GetMT_CONTROL_MET_CONCByMt(mt_meterByBarcode, meterInfo));
//        arrayList.Add(OraDataHelper.GetMT_FEE_TMNL_CONCByMt(mt_meterByBarcode, meterInfo));
//        arrayList.Add(OraDataHelper.GetMT_POWER_MEASURE_MET_CONCByMt(mt_meterByBarcode, meterInfo));
//        arrayList.Add(OraDataHelper.GetMT_MAX_DEMANDVALUE_MET_CONCByMt(mt_meterByBarcode, meterInfo));
//        arrayList.Add(OraDataHelper.GetMT_POWER_MET_CONCByMt(mt_meterByBarcode, meterInfo));
//        arrayList.Add(OraDataHelper.GetMT_DETECT_RSLTByMt(mt_meterByBarcode, meterInfo));
//        result = OraHelper.ExecuteSqlTran(arrayList);
//    }
//    return result;
//}

//// Token: 0x060009C6 RID: 2502 RVA: 0x0006B1F8 File Offset: 0x000693F8
//public static bool UpdateMeterInfoToHouDaMisSystem(MeterBasicInfo meterInfo, out string strReturnMessage)
//{
//    strReturnMessage = "";
//    ArrayList arrayList = new ArrayList();
//    MT_METER mt_meterByBarcode = new MT_METERServices().GetMt_meterByBarcode(meterInfo.Mb_ChrTxm);
//    string detect_TASK_NO = mt_meterByBarcode.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    meterInfo.Mb_chrOther5 = detect_TASK_NO;
//    meterInfo.Mb_ChrTxm = meterInfo.Mb_ChrTxm.Trim();
//    if (OraDataHelper.g_DicPCodeTable == null)
//    {
//        OraDataHelper.GetDicPCodeTable();
//    }
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_BASICERR_MET_CONC where detect_task_no = '",
//                detect_TASK_NO,
//                "' and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_MEASURE_REPEAT_MET_CONC where detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_STARTING_MET_CONC where detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_CREEPING_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_CONST_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_DAYERR_MET_CONC  where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_ESAM_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.mt_detect_met_rslt where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_PRESETPARAM_CHECK_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_PRESETPARAM_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_PASSWORD_CHANGE_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_CONSIST_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.Mt_OVERLOAD_MET_CONC  where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_TS_MET_CONC  where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_ERROR_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_VARIATION_MET_CONC where detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_DEMANDVALUE_MET_CONC where detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_ESAM_SECURITY_MET_CONC where detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_DETECT_MET_RSLT where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_EQUIP_UNPASS_REASON where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    string[] array = new string[0];
//    string key = "77000";
//    string[] array2 = new string[meterInfo.MeterDgns.Keys.Count];
//    meterInfo.MeterDgns.Keys.CopyTo(array2, 0);
//    for (int i = 0; i < meterInfo.MeterDgns.Keys.Count; i++)
//    {
//        if (array2[i].IndexOf("77000") == 0)
//        {
//            if (meterInfo.MeterDgns[key].AVR_CONCLUSION == "不合格")
//            {
//                array = OraDataHelper.GetMT_INTUIT_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//                if (array != null)
//                {
//                    foreach (string value in array)
//                    {
//                        arrayList.Add(value);
//                    }
//                }
//            }
//        }
//    }
//    array = OraDataHelper.GetMT_DEMANDVALUE_MET_CONC(mt_meterByBarcode, meterInfo);
//    for (int i = 0; i < array.Length; i++)
//    {
//        if (array[i] != null)
//        {
//            arrayList.Add(array[i]);
//        }
//    }
//    array = OraDataHelper.GetMT_STARTING_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//    if (array != null)
//    {
//        foreach (string value in array)
//        {
//            arrayList.Add(value);
//        }
//    }
//    array = OraDataHelper.GetMT_CREEPING_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//    if (array != null)
//    {
//        foreach (string value in array)
//        {
//            arrayList.Add(value);
//        }
//    }
//    array = OraDataHelper.GetMT_PRESETPARAM_CHECK_MET_CONCByMtFK(mt_meterByBarcode, meterInfo);
//    if (array != null)
//    {
//        foreach (string value in array)
//        {
//            arrayList.Add(value);
//        }
//    }
//    array = OraDataHelper.GetMT_BASICERR_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//    if (array != null)
//    {
//        foreach (string value in array)
//        {
//            arrayList.Add(value);
//        }
//    }
//    array = OraDataHelper.GetMT_DEVIATION_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//    if (array != null)
//    {
//        foreach (string value in array)
//        {
//            arrayList.Add(value);
//        }
//    }
//    array = OraDataHelper.GetMT_CONST_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//    if (array != null)
//    {
//        foreach (string value in array)
//        {
//            arrayList.Add(value);
//        }
//    }
//    arrayList.Add(OraDataHelper.GetMT_DAYERR_MET_CONCByMt(mt_meterByBarcode, meterInfo));
//    array = OraDataHelper.GetMT_TS_MET_CONC(mt_meterByBarcode, meterInfo);
//    if (array != null)
//    {
//        foreach (string value in array)
//        {
//            arrayList.Add(value);
//        }
//    }
//    arrayList.Add(OraDataHelper.GetMT_ESAM_MET_CONCByMt(mt_meterByBarcode, meterInfo));
//    arrayList.Add(OraDataHelper.GetMT_ESAM_SECURITY_MET_CONCByMt(mt_meterByBarcode, meterInfo));
//    array = OraDataHelper.GetMT_CONSIST_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//    if (array != null)
//    {
//        foreach (string value in array)
//        {
//            arrayList.Add(value);
//        }
//    }
//    array = OraDataHelper.GetMT_ERROR_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//    if (array != null)
//    {
//        foreach (string value in array)
//        {
//            arrayList.Add(value);
//        }
//    }
//    array = OraDataHelper.GetMT_VARIATION_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//    if (array != null)
//    {
//        foreach (string value in array)
//        {
//            arrayList.Add(value);
//        }
//    }
//    arrayList.Add(OraDataHelper.GetMT_DETECT_RSLTByMt(mt_meterByBarcode, meterInfo));
//    if (meterInfo.Mb_Result.IndexOf("不合格") != -1)
//    {
//        arrayList.Add(OraDataHelper.GetMT_EQUIP_UNPASS_REASON(mt_meterByBarcode, meterInfo));
//    }
//    if (meterInfo.Mb_ChrTxm != "" && !meterInfo.YaoJianYnSave)
//    {
//        arrayList.Add(OraDataHelper.GetMT_EQUIP_UNPASS_REASON(mt_meterByBarcode, meterInfo));
//    }
//    return OraHelper.ExecuteSqlTran(arrayList);
//}

//// Token: 0x060009C7 RID: 2503 RVA: 0x0006BBC4 File Offset: 0x00069DC4
//public static bool UpdateMeterInfoTmpToHouDaMisSystem(MeterBasicInfo meterInfo, out string strReturnMessage)
//{
//    strReturnMessage = "";
//    ArrayList arrayList = new ArrayList();
//    MT_METER mt_meterByBarcode = new MT_METERServices().GetMt_meterByBarcode(meterInfo.Mb_ChrTxm);
//    string detect_TASK_NO = mt_meterByBarcode.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    meterInfo.Mb_chrOther5 = detect_TASK_NO;
//    meterInfo.Mb_ChrTxm = meterInfo.Mb_ChrTxm.Trim();
//    if (OraDataHelper.g_DicPCodeTable == null)
//    {
//        OraDataHelper.GetDicPCodeTable();
//    }
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_BASICERR_MET_CONC where detect_task_no = '",
//                detect_TASK_NO,
//                "' and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_MEASURE_REPEAT_MET_CONC where detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_STARTING_MET_CONC where detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_CREEPING_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_CONST_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_DAYERR_MET_CONC  where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_ESAM_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.mt_detect_met_rslt where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_PRESETPARAM_CHECK_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_PRESETPARAM_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_PASSWORD_CHANGE_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_CONSIST_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.Mt_OVERLOAD_MET_CONC  where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_TS_MET_CONC  where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_ERROR_MET_CONC where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_VARIATION_MET_CONC where detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_DEMANDVALUE_MET_CONC where detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_ESAM_SECURITY_MET_CONC where detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_DETECT_MET_RSLT where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "delete from bjxm3_result.MT_EQUIP_UNPASS_REASON where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    string[] array = new string[0];
//    string[] array2 = new string[meterInfo.MeterDgns.Keys.Count];
//    array = OraDataHelper.GetMT_INTUIT_MET_CONCByMt(mt_meterByBarcode, meterInfo);
//    if (array != null)
//    {
//        foreach (string value in array)
//        {
//            arrayList.Add(value);
//        }
//    }
//    arrayList.Add(OraDataHelper.GetMT_DETECT_RSLTTMPByMt(mt_meterByBarcode, meterInfo));
//    arrayList.Add(OraDataHelper.GetMT_EQUIP_UNPASS_REASONTMP(mt_meterByBarcode, meterInfo));
//    return OraHelper.ExecuteSqlTran(arrayList);
//}

//// Token: 0x060009C8 RID: 2504 RVA: 0x0006C1BC File Offset: 0x0006A3BC
//public static bool UpdateMeterRjsInfoToHouDaMisSystem(MeterBasicInfo meterInfo, out string strReturnMessage)
//{
//    strReturnMessage = "";
//    ArrayList arrayList = new ArrayList();
//    MT_METER mt_meterByBarcode = new MT_METERServices().GetMt_meterByBarcode(meterInfo.Mb_ChrTxm);
//    string detect_TASK_NO = mt_meterByBarcode.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    meterInfo.Mb_chrOther5 = detect_TASK_NO;
//    meterInfo.Mb_ChrTxm = meterInfo.Mb_ChrTxm.Trim();
//    if (OraDataHelper.g_DicPCodeTable == null)
//    {
//        OraDataHelper.GetDicPCodeTable();
//    }
//    arrayList.Add(string.Concat(new string[]
//    {
//                "update  bjxm3_result.MT_DAYERR_MET_CONC t set t.ERROR='-0.1428|-0.1301|-0.1240|-0.1307|-0.1262|',t.AVG_ERR='-0.1308',t.INT_CONVERT_ERR='-0.13',t.CONC_CODE='01'  where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    arrayList.Add(string.Concat(new string[]
//    {
//                "update  bjxm3_result.MT_DETECT_MET_RSLT m set m.DAYERR_CONC_CODE='01' where  detect_task_no = '",
//                detect_TASK_NO,
//                "'and bar_code='",
//                meterInfo.Mb_ChrTxm,
//                "' "
//    }));
//    return OraHelper.ExecuteSqlTran(arrayList);
//}

//// Token: 0x060009C9 RID: 2505 RVA: 0x0006C2B0 File Offset: 0x0006A4B0
//public static string[] GetMT_WAVE_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    List<string> list = new List<string>();
//    List<MeterCarrierData> list2 = new List<MeterCarrierData>();
//    string[] array = null;
//    string[] result;
//    if (meterInfo.MeterCarrierDatas.Count == 0)
//    {
//        result = array;
//    }
//    else
//    {
//        foreach (string text in meterInfo.MeterCarrierDatas.Keys)
//        {
//            list.Add(text);
//            list2.Add(meterInfo.MeterCarrierDatas[text]);
//        }
//        array = new string[list2.Count];
//        for (int i = 0; i < list2.Count; i++)
//        {
//            MT_WAVE_MET_CONC mt_WAVE_MET_CONC = new MT_WAVE_MET_CONC();
//            mt_WAVE_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//            mt_WAVE_MET_CONC.EQUIP_CATEG = "01";
//            if (meterInfo.Mb_intClfs == 5)
//            {
//                mt_WAVE_MET_CONC.SYS_NO = "401";
//            }
//            else
//            {
//                mt_WAVE_MET_CONC.SYS_NO = "402";
//            }
//            mt_WAVE_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//            mt_WAVE_MET_CONC.DETECT_EQUIP_NO = meterInfo._intTaiNo.ToString().Trim();
//            mt_WAVE_MET_CONC.DETECT_UNIT_NO = string.Empty;
//            mt_WAVE_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString().Trim();
//            mt_WAVE_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//            mt_WAVE_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//            mt_WAVE_MET_CONC.PARA_INDEX = (i + 1).ToString();
//            mt_WAVE_MET_CONC.DETECT_ITEM_POINT = (i + 1).ToString();
//            mt_WAVE_MET_CONC.IS_VALID = "1";
//            mt_WAVE_MET_CONC.CONC_CODE = ((list2[i].Mce_ItemResult.Trim() == "合格") ? "01" : "02");
//            mt_WAVE_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//            mt_WAVE_MET_CONC.HANDLE_FLAG = "0";
//            array[i] = new MT_WAVE_MET_CONCService().InsertSQL(mt_WAVE_MET_CONC);
//        }
//        result = array;
//    }
//    return result;
//}

//// Token: 0x060009CA RID: 2506 RVA: 0x0006C4FC File Offset: 0x0006A6FC
//public static string GetMT_WARNNING_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string result;
//    if (MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.报警功能) == "")
//    {
//        result = "";
//    }
//    else
//    {
//        MT_WARNNING_MET_CONC mt_WARNNING_MET_CONC = new MT_WARNNING_MET_CONC();
//        string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//        mt_WARNNING_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_WARNNING_MET_CONC.EQUIP_CATEG = "01";
//        if (meterInfo.Mb_intClfs == 5)
//        {
//            mt_WARNNING_MET_CONC.SYS_NO = "401";
//        }
//        else
//        {
//            mt_WARNNING_MET_CONC.SYS_NO = "402";
//        }
//        mt_WARNNING_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_WARNNING_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//        mt_WARNNING_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_WARNNING_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_WARNNING_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_WARNNING_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_WARNNING_MET_CONC.PARA_INDEX = "01";
//        mt_WARNNING_MET_CONC.DETECT_ITEM_POINT = "01";
//        mt_WARNNING_MET_CONC.IS_VALID = "1";
//        mt_WARNNING_MET_CONC.WARN_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.报警功能);
//        mt_WARNNING_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_WARNNING_MET_CONC.HANDLE_FLAG = "0";
//        result = new MT_WARNNING_MET_CONCService().InsertSQL(mt_WARNNING_MET_CONC);
//    }
//    return result;
//}

//// Token: 0x060009CB RID: 2507 RVA: 0x0006C658 File Offset: 0x0006A858
//public static string GetMT_EP_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string result;
//    if (MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.保电功能) == "")
//    {
//        result = "";
//    }
//    else
//    {
//        MT_EP_MET_CONC mt_EP_MET_CONC = new MT_EP_MET_CONC();
//        string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//        mt_EP_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_EP_MET_CONC.EQUIP_CATEG = "01";
//        if (meterInfo.Mb_intClfs == 5)
//        {
//            mt_EP_MET_CONC.SYS_NO = "401";
//        }
//        else
//        {
//            mt_EP_MET_CONC.SYS_NO = "402";
//        }
//        mt_EP_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_EP_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//        mt_EP_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_EP_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_EP_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_EP_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_EP_MET_CONC.PARA_INDEX = "01";
//        mt_EP_MET_CONC.DETECT_ITEM_POINT = "01";
//        mt_EP_MET_CONC.IS_VALID = "1";
//        mt_EP_MET_CONC.EH_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.保电功能);
//        mt_EP_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_EP_MET_CONC.HANDLE_FLAG = "0";
//        result = new MT_EP_MET_CONCService().InsertSQL(mt_EP_MET_CONC);
//    }
//    return result;
//}

//// Token: 0x060009CC RID: 2508 RVA: 0x0006C7B4 File Offset: 0x0006A9B4
//public static string GetMT_EC_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string result;
//    if (MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.保电解除) == "")
//    {
//        result = "";
//    }
//    else
//    {
//        MT_EC_MET_CONC mt_EC_MET_CONC = new MT_EC_MET_CONC();
//        string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//        mt_EC_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_EC_MET_CONC.EQUIP_CATEG = "01";
//        if (meterInfo.Mb_intClfs == 5)
//        {
//            mt_EC_MET_CONC.SYS_NO = "401";
//        }
//        else
//        {
//            mt_EC_MET_CONC.SYS_NO = "402";
//        }
//        mt_EC_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_EC_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//        mt_EC_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_EC_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_EC_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_EC_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_EC_MET_CONC.PARA_INDEX = "01";
//        mt_EC_MET_CONC.DETECT_ITEM_POINT = "01";
//        mt_EC_MET_CONC.IS_VALID = "1";
//        mt_EC_MET_CONC.EC_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.保电解除);
//        mt_EC_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_EC_MET_CONC.HANDLE_FLAG = "0";
//        result = new MT_EC_MET_CONCService().InsertSQL(mt_EC_MET_CONC);
//    }
//    return result;
//}

//// Token: 0x060009CD RID: 2509 RVA: 0x0006C910 File Offset: 0x0006AB10
//public static string GetMT_ESAM_READ_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string result;
//    if (MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.数据回抄) == "")
//    {
//        result = "";
//    }
//    else
//    {
//        string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//        MT_ESAM_READ_MET_CONC mt_ESAM_READ_MET_CONC = new MT_ESAM_READ_MET_CONC();
//        mt_ESAM_READ_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_ESAM_READ_MET_CONC.EQUIP_CATEG = "01";
//        if (meterInfo.Mb_intClfs == 5)
//        {
//            mt_ESAM_READ_MET_CONC.SYS_NO = "401";
//        }
//        else
//        {
//            mt_ESAM_READ_MET_CONC.SYS_NO = "402";
//        }
//        mt_ESAM_READ_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_ESAM_READ_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//        mt_ESAM_READ_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_ESAM_READ_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_ESAM_READ_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_ESAM_READ_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_ESAM_READ_MET_CONC.PARA_INDEX = "01";
//        mt_ESAM_READ_MET_CONC.DETECT_ITEM_POINT = "01";
//        mt_ESAM_READ_MET_CONC.IS_VALID = "1";
//        mt_ESAM_READ_MET_CONC.CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.数据回抄);
//        mt_ESAM_READ_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_ESAM_READ_MET_CONC.HANDLE_FLAG = "0";
//        result = new MT_ESAM_READ_MET_CONCService().InsertSQL(mt_ESAM_READ_MET_CONC);
//    }
//    return result;
//}

//// Token: 0x060009CE RID: 2510 RVA: 0x0006CA6C File Offset: 0x0006AC6C
//public static string GetMT_PARA_SETTING_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string result;
//    if (MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.参数设置) == "")
//    {
//        result = "";
//    }
//    else
//    {
//        MT_PARA_SETTING_MET_CONC mt_PARA_SETTING_MET_CONC = new MT_PARA_SETTING_MET_CONC();
//        mt_PARA_SETTING_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_PARA_SETTING_MET_CONC.EQUIP_CATEG = "01";
//        if (meterInfo.Mb_intClfs == 5)
//        {
//            mt_PARA_SETTING_MET_CONC.SYS_NO = "401";
//        }
//        else
//        {
//            mt_PARA_SETTING_MET_CONC.SYS_NO = "402";
//        }
//        mt_PARA_SETTING_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_PARA_SETTING_MET_CONC.DETECT_EQUIP_NO = meterInfo._intTaiNo;
//        mt_PARA_SETTING_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_PARA_SETTING_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_PARA_SETTING_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_PARA_SETTING_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_PARA_SETTING_MET_CONC.PARA_INDEX = "01";
//        mt_PARA_SETTING_MET_CONC.DETECT_ITEM_POINT = "01";
//        mt_PARA_SETTING_MET_CONC.IS_VALID = "1";
//        mt_PARA_SETTING_MET_CONC.CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.参数设置);
//        mt_PARA_SETTING_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_PARA_SETTING_MET_CONC.HANDLE_FLAG = "0";
//        result = new MT_PARA_SETTING_MET_CONCService().InsertSQL(mt_PARA_SETTING_MET_CONC);
//    }
//    return result;
//}

//// Token: 0x060009CF RID: 2511 RVA: 0x0006CBB8 File Offset: 0x0006ADB8
//public static string GetMT_FEE_TMNL_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string result;
//    if (meterInfo.MeterFunctions.Count == 0)
//    {
//        result = "";
//    }
//    else if (!meterInfo.MeterFunctions.ContainsKey("004"))
//    {
//        result = "";
//    }
//    else
//    {
//        MT_FEE_MET_CONC mt_FEE_MET_CONC = new MT_FEE_MET_CONC();
//        mt_FEE_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_FEE_MET_CONC.EQUIP_CATEG = "01";
//        if (meterInfo.Mb_intClfs == 5)
//        {
//            mt_FEE_MET_CONC.SYS_NO = "401";
//        }
//        else
//        {
//            mt_FEE_MET_CONC.SYS_NO = "402";
//        }
//        mt_FEE_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_FEE_MET_CONC.DETECT_EQUIP_NO = meterInfo._intTaiNo.ToString().Trim();
//        mt_FEE_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_FEE_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_FEE_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_FEE_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_FEE_MET_CONC.PARA_INDEX = "01";
//        mt_FEE_MET_CONC.DETECT_ITEM_POINT = "01";
//        mt_FEE_MET_CONC.IS_VALID = "1";
//        mt_FEE_MET_CONC.LOAD_CURRENT = "05";
//        mt_FEE_MET_CONC.BOTH_WAY_POWER_FLAG = "0";
//        mt_FEE_MET_CONC.CONC_CODE = ((meterInfo.MeterFunctions["004"].Mf_chrValue.ToString().Trim() == "合格") ? "01" : "02");
//        mt_FEE_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode("1.0");
//        string strName;
//        if (meterInfo.Mb_intClfs == 1)
//        {
//            strName = "AC";
//        }
//        else
//        {
//            strName = "ABC";
//        }
//        mt_FEE_MET_CONC.CUR_PHASE_CODE = OraDataHelper.g_DicPCodeTable["currentPhaseCode"].GetPCode(strName);
//        mt_FEE_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode("1.0Ib");
//        mt_FEE_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_FEE_MET_CONC.HANDLE_FLAG = "0";
//        result = new MT_FEE_MET_CONCService().InsertSQL(mt_FEE_MET_CONC);
//    }
//    return result;
//}

//// Token: 0x060009D0 RID: 2512 RVA: 0x0006CDF4 File Offset: 0x0006AFF4
//public static string GetMT_MAX_DEMANDVALUE_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string result;
//    if (meterInfo.MeterFunctions.Count == 0)
//    {
//        result = "";
//    }
//    else if (!meterInfo.MeterFunctions.ContainsKey("006"))
//    {
//        result = "";
//    }
//    else
//    {
//        MT_MAX_DEMANDVALUE_MET_CONC mt_MAX_DEMANDVALUE_MET_CONC = new MT_MAX_DEMANDVALUE_MET_CONC();
//        mt_MAX_DEMANDVALUE_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_MAX_DEMANDVALUE_MET_CONC.EQUIP_CATEG = "01";
//        mt_MAX_DEMANDVALUE_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        if (meterInfo.Mb_intClfs == 5)
//        {
//            mt_MAX_DEMANDVALUE_MET_CONC.SYS_NO = "401";
//        }
//        else
//        {
//            mt_MAX_DEMANDVALUE_MET_CONC.SYS_NO = "402";
//        }
//        mt_MAX_DEMANDVALUE_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_MAX_DEMANDVALUE_MET_CONC.DETECT_EQUIP_NO = meterInfo._intTaiNo.ToString().Trim();
//        mt_MAX_DEMANDVALUE_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_MAX_DEMANDVALUE_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_MAX_DEMANDVALUE_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_MAX_DEMANDVALUE_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_MAX_DEMANDVALUE_MET_CONC.PARA_INDEX = "01";
//        mt_MAX_DEMANDVALUE_MET_CONC.DETECT_ITEM_POINT = "01";
//        mt_MAX_DEMANDVALUE_MET_CONC.IS_VALID = "1";
//        mt_MAX_DEMANDVALUE_MET_CONC.LOAD_CURRENT = "05";
//        mt_MAX_DEMANDVALUE_MET_CONC.BOTH_WAY_POWER_FLAG = "0";
//        mt_MAX_DEMANDVALUE_MET_CONC.CONC_CODE = ((meterInfo.MeterFunctions["006"].Mf_chrValue.ToString().Trim() == "合格") ? "01" : "02");
//        mt_MAX_DEMANDVALUE_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode("1.0");
//        string strName;
//        if (meterInfo.Mb_intClfs == 1)
//        {
//            strName = "AC";
//        }
//        else
//        {
//            strName = "ABC";
//        }
//        mt_MAX_DEMANDVALUE_MET_CONC.CUR_PHASE_CODE = OraDataHelper.g_DicPCodeTable["currentPhaseCode"].GetPCode(strName);
//        mt_MAX_DEMANDVALUE_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode("1.0Ib");
//        mt_MAX_DEMANDVALUE_MET_CONC.VOLT_RATIO = OraDataHelper.g_DicPCodeTable["meterTestVolt"].GetPCode("100%Un");
//        mt_MAX_DEMANDVALUE_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_MAX_DEMANDVALUE_MET_CONC.HANDLE_FLAG = "0";
//        result = new MT_MAX_DEMANDVALUE_MET_CONCService().InsertSQL(mt_MAX_DEMANDVALUE_MET_CONC);
//    }
//    return result;
//}

//// Token: 0x060009D1 RID: 2513 RVA: 0x0006D060 File Offset: 0x0006B260
//public static string GetMT_POWER_MEASURE_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string result;
//    if (meterInfo.MeterFunctions.Count == 0)
//    {
//        result = "";
//    }
//    else if (!meterInfo.MeterFunctions.ContainsKey("001"))
//    {
//        result = "";
//    }
//    else
//    {
//        MT_POWER_MEASURE_MET_CONC mt_POWER_MEASURE_MET_CONC = new MT_POWER_MEASURE_MET_CONC();
//        mt_POWER_MEASURE_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_POWER_MEASURE_MET_CONC.EQUIP_CATEG = "01";
//        if (meterInfo.Mb_intClfs == 5)
//        {
//            mt_POWER_MEASURE_MET_CONC.SYS_NO = "401";
//        }
//        else
//        {
//            mt_POWER_MEASURE_MET_CONC.SYS_NO = "402";
//        }
//        mt_POWER_MEASURE_MET_CONC.DETECT_EQUIP_NO = meterInfo._intTaiNo.ToString().Trim();
//        mt_POWER_MEASURE_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_POWER_MEASURE_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_POWER_MEASURE_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_POWER_MEASURE_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_POWER_MEASURE_MET_CONC.PARA_INDEX = "01";
//        mt_POWER_MEASURE_MET_CONC.DETECT_ITEM_POINT = "01";
//        mt_POWER_MEASURE_MET_CONC.IS_VALID = "1";
//        mt_POWER_MEASURE_MET_CONC.LOAD_CURRENT = "05";
//        mt_POWER_MEASURE_MET_CONC.BOTH_WAY_POWER_FLAG = "0";
//        mt_POWER_MEASURE_MET_CONC.CONC_CODE = ((meterInfo.MeterFunctions["001"].Mf_chrValue.ToString().Trim() == "合格") ? "01" : "02");
//        mt_POWER_MEASURE_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode("1.0");
//        string strName;
//        if (meterInfo.Mb_intClfs == 1)
//        {
//            strName = "AC";
//        }
//        else
//        {
//            strName = "ABC";
//        }
//        mt_POWER_MEASURE_MET_CONC.CUR_PHASE_CODE = OraDataHelper.g_DicPCodeTable["currentPhaseCode"].GetPCode(strName);
//        mt_POWER_MEASURE_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode("1.0Ib");
//        mt_POWER_MEASURE_MET_CONC.VOLT_RATIO = OraDataHelper.g_DicPCodeTable["meterTestVolt"].GetPCode("100%Un");
//        mt_POWER_MEASURE_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_POWER_MEASURE_MET_CONC.HANDLE_FLAG = "0";
//        result = new MT_POWER_MEASURE_MET_CONCService().InsertSQL(mt_POWER_MEASURE_MET_CONC);
//    }
//    return result;
//}

//// Token: 0x060009D2 RID: 2514 RVA: 0x0006D2A8 File Offset: 0x0006B4A8
//public static string GetMT_POWER_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string result;
//    if (meterInfo.MeterPowers.Count == 0)
//    {
//        result = "";
//    }
//    else
//    {
//        MT_POWER_MET_CONC mt_POWER_MET_CONC = new MT_POWER_MET_CONC();
//        mt_POWER_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_POWER_MET_CONC.EQUIP_CATEG = "01";
//        if (meterInfo.Mb_intClfs == 5)
//        {
//            mt_POWER_MET_CONC.SYS_NO = "401";
//        }
//        else
//        {
//            mt_POWER_MET_CONC.SYS_NO = "402";
//        }
//        mt_POWER_MET_CONC.DETECT_EQUIP_NO = meterInfo._intTaiNo;
//        mt_POWER_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_POWER_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_POWER_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_POWER_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_POWER_MET_CONC.PARA_INDEX = "01";
//        mt_POWER_MET_CONC.DETECT_ITEM_POINT = "01";
//        mt_POWER_MET_CONC.IS_VALID = "1";
//        foreach (string key in meterInfo.MeterPowers.Keys)
//        {
//            mt_POWER_MET_CONC.CUR_ACT_POWER = "";
//            mt_POWER_MET_CONC.CUR_ACT_POWER_ERR = "";
//            mt_POWER_MET_CONC.CUR_ACT_POWER_RESULT = "";
//            mt_POWER_MET_CONC.CUR_INS_POWER = meterInfo.MeterPowers[key].Md_Ia_ReactiveS.ToString().Trim();
//            mt_POWER_MET_CONC.CUR_INS_POWER_ERR = meterInfo.MeterPowers[key].AVR_CUR_CIR_S_LIMIT.Trim();
//            mt_POWER_MET_CONC.CUR_INS_POWER_RESULT = "合格";
//            mt_POWER_MET_CONC.VOL_ACT_POWER = meterInfo.MeterPowers[key].Md_Ua_ReactiveP.ToString().Trim();
//            mt_POWER_MET_CONC.VOL_ACT_POWER_ERR = meterInfo.MeterPowers[key].AVR_VOT_CIR_P_LIMIT.Trim();
//            mt_POWER_MET_CONC.VOL_ACT_POWER_RESULT = "合格";
//            mt_POWER_MET_CONC.VOL_INS_POWER = meterInfo.MeterPowers[key].Md_Ua_ReactiveS.ToString().Trim();
//            mt_POWER_MET_CONC.VOL_INS_POWER_ERR = meterInfo.MeterPowers[key].AVR_VOT_CIR_S_LIMIT.Trim();
//            mt_POWER_MET_CONC.VOL_INS_POWER_RESULT = "合格";
//            mt_POWER_MET_CONC.TEST_ITEM = "功耗试验";
//            mt_POWER_MET_CONC.POWER_CONSUM_TYPE = "功耗试验";
//            mt_POWER_MET_CONC.ERR_ABS = string.Concat(new string[]
//            {
//                        meterInfo.MeterPowers[key].AVR_CUR_CIR_S_LIMIT.Trim(),
//                        "|",
//                        meterInfo.MeterPowers[key].AVR_VOT_CIR_P_LIMIT.Trim(),
//                        "|",
//                        meterInfo.MeterPowers[key].AVR_VOT_CIR_S_LIMIT.Trim()
//            });
//            mt_POWER_MET_CONC.CONC_CODE = "01";
//        }
//        mt_POWER_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_POWER_MET_CONC.HANDLE_FLAG = "0";
//        result = new MT_POWER_MET_CONCService().InsertSQL(mt_POWER_MET_CONC);
//    }
//    return result;
//}

//// Token: 0x060009D3 RID: 2515 RVA: 0x0006D5D0 File Offset: 0x0006B7D0
//private static string[] GetMT_HUTCHISON_COMBINA_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    List<string> list = new List<string>();
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    string text = 5.ToString().PadLeft(3, '0');
//    string[] result;
//    if (!meterInfo.MeterDgns.ContainsKey(text))
//    {
//        result = null;
//    }
//    else
//    {
//        MT_HUTCHISON_COMBINA_MET_CONC mt_HUTCHISON_COMBINA_MET_CONC = new MT_HUTCHISON_COMBINA_MET_CONC();
//        MT_HUTCHISON_COMBINA_MET_CONCService mt_HUTCHISON_COMBINA_MET_CONCService = new MT_HUTCHISON_COMBINA_MET_CONCService();
//        mt_HUTCHISON_COMBINA_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_HUTCHISON_COMBINA_MET_CONC.EQUIP_CATEG = "01";
//        mt_HUTCHISON_COMBINA_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_HUTCHISON_COMBINA_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//        mt_HUTCHISON_COMBINA_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_HUTCHISON_COMBINA_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_HUTCHISON_COMBINA_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_HUTCHISON_COMBINA_MET_CONC.DETECT_DATE = DateTime.Now.ToString();
//        mt_HUTCHISON_COMBINA_MET_CONC.PARA_INDEX = "01";
//        mt_HUTCHISON_COMBINA_MET_CONC.DETECT_ITEM_POINT = "01";
//        mt_HUTCHISON_COMBINA_MET_CONC.BOTH_WAY_POWER_FLAG = OraDataHelper.g_DicPCodeTable["powerFlag"].GetPCode("正向有功");
//        mt_HUTCHISON_COMBINA_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode("1.0Ib");
//        mt_HUTCHISON_COMBINA_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode("1.0");
//        mt_HUTCHISON_COMBINA_MET_CONC.CONTROL_METHOD = OraDataHelper.g_DicPCodeTable["meterTestCtrlMode"].GetPCode("标准表法");
//        mt_HUTCHISON_COMBINA_MET_CONC.ERR_DOWN = "-0.01";
//        mt_HUTCHISON_COMBINA_MET_CONC.ERR_UP = "0.01";
//        mt_HUTCHISON_COMBINA_MET_CONC.VOLTAGE = "100";
//        mt_HUTCHISON_COMBINA_MET_CONC.TOTAL_READING_ERR = "";
//        mt_HUTCHISON_COMBINA_MET_CONC.TOTAL_INCREMENT = "";
//        mt_HUTCHISON_COMBINA_MET_CONC.SUMER_ALL_INCREMENT = "";
//        mt_HUTCHISON_COMBINA_MET_CONC.SHARP_INCREMENT = "";
//        mt_HUTCHISON_COMBINA_MET_CONC.PEAK_INCREMENT = "";
//        mt_HUTCHISON_COMBINA_MET_CONC.FLAT_INCREMENT = "";
//        mt_HUTCHISON_COMBINA_MET_CONC.VALLEY_INCREMENT = "";
//        mt_HUTCHISON_COMBINA_MET_CONC.VALUE_CONC_CODE = "";
//        mt_HUTCHISON_COMBINA_MET_CONC.CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.计度器示值组合误差);
//        mt_HUTCHISON_COMBINA_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_HUTCHISON_COMBINA_MET_CONC.HANDLE_FLAG = "0";
//        mt_HUTCHISON_COMBINA_MET_CONC.FEE_RATIO = "5";
//        float num = 0f;
//        float num2 = 0f;
//        float num3 = 0f;
//        float num4 = 0f;
//        float num5 = 0f;
//        string key = text + "07";
//        int num6 = 0;
//        for (int i = 0; i < 20; i++)
//        {
//            key = text + i.ToString("D2");
//            if (meterInfo.MeterDgns.ContainsKey(key))
//            {
//                string md_chrValue = meterInfo.MeterDgns[key].Md_chrValue;
//                if (!string.IsNullOrEmpty(md_chrValue))
//                {
//                    if (md_chrValue.IndexOf('|') == -1)
//                    {
//                        mt_HUTCHISON_COMBINA_MET_CONC.VALUE_CONC_CODE = md_chrValue;
//                        mt_HUTCHISON_COMBINA_MET_CONC.FEE_VALUE = md_chrValue;
//                    }
//                    else
//                    {
//                        string[] array = md_chrValue.Split(new char[]
//                        {
//                                    '|'
//                        });
//                        if (array.Length >= 4)
//                        {
//                            if (array[3] == "尖")
//                            {
//                                num += float.Parse(array[2]);
//                            }
//                            else if (array[3] == "峰")
//                            {
//                                num6++;
//                                num2 += float.Parse(array[2]);
//                            }
//                            else if (array[3] == "平")
//                            {
//                                num6++;
//                                num3 += float.Parse(array[2]);
//                            }
//                            else if (array[3] == "谷")
//                            {
//                                num6++;
//                                num4 += float.Parse(array[2]);
//                            }
//                            else if (array[3] == "总")
//                            {
//                                num6++;
//                                num5 += float.Parse(array[2]);
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        float num7 = num + num2 + num3 + num4;
//        float num8 = num5 - num7;
//        mt_HUTCHISON_COMBINA_MET_CONC.SUMER_ALL_INCREMENT = num7.ToString("F2");
//        mt_HUTCHISON_COMBINA_MET_CONC.SHARP_INCREMENT = num.ToString("F2");
//        mt_HUTCHISON_COMBINA_MET_CONC.PEAK_INCREMENT = num2.ToString("F2");
//        mt_HUTCHISON_COMBINA_MET_CONC.FLAT_INCREMENT = num3.ToString("F2");
//        mt_HUTCHISON_COMBINA_MET_CONC.VALLEY_INCREMENT = num4.ToString("F2");
//        mt_HUTCHISON_COMBINA_MET_CONC.TOTAL_INCREMENT = num5.ToString("F2");
//        mt_HUTCHISON_COMBINA_MET_CONC.TOTAL_READING_ERR = num8.ToString("F2");
//        mt_HUTCHISON_COMBINA_MET_CONC.IR_TIME = num6.ToString();
//        mt_HUTCHISON_COMBINA_MET_CONC.IR_READING = num5.ToString("F2");
//        mt_HUTCHISON_COMBINA_MET_CONC.FEE_RATIO = "总";
//        list.Add(mt_HUTCHISON_COMBINA_MET_CONCService.InsertSQL(mt_HUTCHISON_COMBINA_MET_CONC));
//        result = list.ToArray();
//    }
//    return result;
//}

//// Token: 0x060009D4 RID: 2516 RVA: 0x0006DAF0 File Offset: 0x0006BCF0
//private static string GetMT_EQ_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string key = 9.ToString().PadLeft(3, '0');
//    string result;
//    if (!meterInfo.MeterCostControls.ContainsKey(key))
//    {
//        result = null;
//    }
//    else
//    {
//        MT_EQ_MET_CONC mt_EQ_MET_CONC = new MT_EQ_MET_CONC();
//        mt_EQ_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_EQ_MET_CONC.EQUIP_CATEG = "01";
//        mt_EQ_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_EQ_MET_CONC.DETECT_EQUIP_NO = meterInfo._intTaiNo;
//        mt_EQ_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_EQ_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_EQ_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_EQ_MET_CONC.DETECT_DATE = DateTime.Now.ToString();
//        mt_EQ_MET_CONC.PARA_INDEX = "01";
//        mt_EQ_MET_CONC.DETECT_ITEM_POINT = "01";
//        mt_EQ_MET_CONC.IS_VALID = "01";
//        mt_EQ_MET_CONC.TOTAL_EQ = "03";
//        mt_EQ_MET_CONC.SURPLUS_EQ = "0";
//        mt_EQ_MET_CONC.CURR_ELEC_PRICE = "0";
//        string mfk_chrData = meterInfo.MeterCostControls[key].Mfk_chrData;
//        if (!string.IsNullOrEmpty(mfk_chrData))
//        {
//            string[] array = mfk_chrData.Split(new char[]
//            {
//                        '|'
//            });
//            if (array.Length >= 6)
//            {
//                mt_EQ_MET_CONC.TOTAL_EQ = array[2];
//                mt_EQ_MET_CONC.SURPLUS_EQ = array[4];
//                mt_EQ_MET_CONC.CURR_ELEC_PRICE = array[0];
//            }
//        }
//        mt_EQ_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode("Imax");
//        mt_EQ_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode("1.0");
//        mt_EQ_MET_CONC.CONC_CODE = ((meterInfo.MeterCostControls[key].Mfk_chrJL.Trim() == "合格") ? "01" : "02");
//        mt_EQ_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_EQ_MET_CONC.HANDLE_FLAG = "0";
//        result = new MT_EQ_MET_CONCService().InsertSQL(mt_EQ_MET_CONC);
//    }
//    return result;
//}

//// Token: 0x060009D5 RID: 2517 RVA: 0x0006DD14 File Offset: 0x0006BF14
//public static string[] GetMT_INTUIT_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string[] array = new string[1];
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    MT_INTUIT_MET_CONC mt_INTUIT_MET_CONC = new MT_INTUIT_MET_CONC();
//    mt_INTUIT_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_INTUIT_MET_CONC.EQUIP_CATEG = "01";
//    mt_INTUIT_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_INTUIT_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//    mt_INTUIT_MET_CONC.DETECT_UNIT_NO = "01";
//    mt_INTUIT_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//    mt_INTUIT_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//    mt_INTUIT_MET_CONC.DETECT_DATE = DateTime.Now.ToString();
//    mt_INTUIT_MET_CONC.PARA_INDEX = "1";
//    mt_INTUIT_MET_CONC.DETECT_ITEM_POINT = "02";
//    mt_INTUIT_MET_CONC.IS_VALID = "01";
//    mt_INTUIT_MET_CONC.DETECT_CONTENT = "05";
//    mt_INTUIT_MET_CONC.CONC_CODE = "02";
//    mt_INTUIT_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//    mt_INTUIT_MET_CONC.HANDLE_FLAG = "0";
//    array[0] = new MT_INTUIT_MET_CONCService().InsertSQL(mt_INTUIT_MET_CONC);
//    return array;
//}

//// Token: 0x060009D6 RID: 2518 RVA: 0x0006DE3C File Offset: 0x0006C03C
//public static string[] GetMT_PRESETPARAM_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string[] array = new string[3];
//    MT_PRESETPARAM_MET_CONCService mt_PRESETPARAM_MET_CONCService = new MT_PRESETPARAM_MET_CONCService();
//    MT_PRESETPARAM_MET_CONC mt_PRESETPARAM_MET_CONC = new MT_PRESETPARAM_MET_CONC();
//    mt_PRESETPARAM_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_PRESETPARAM_MET_CONC.EQUIP_CATEG = "01";
//    mt_PRESETPARAM_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_PRESETPARAM_MET_CONC.DETECT_EQUIP_NO = meterInfo._intTaiNo;
//    mt_PRESETPARAM_MET_CONC.DETECT_UNIT_NO = string.Empty;
//    mt_PRESETPARAM_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//    mt_PRESETPARAM_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//    mt_PRESETPARAM_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//    mt_PRESETPARAM_MET_CONC.PARA_INDEX = "01";
//    mt_PRESETPARAM_MET_CONC.DETECT_ITEM_POINT = "01";
//    mt_PRESETPARAM_MET_CONC.CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.电量清零);
//    mt_PRESETPARAM_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//    mt_PRESETPARAM_MET_CONC.HANDLE_FLAG = "0";
//    mt_PRESETPARAM_MET_CONC.DATA_ITEM_NAME = "设置时间";
//    array[0] = mt_PRESETPARAM_MET_CONCService.InsertSQL(mt_PRESETPARAM_MET_CONC);
//    mt_PRESETPARAM_MET_CONC.DATA_ITEM_NAME = "清空电量";
//    array[1] = mt_PRESETPARAM_MET_CONCService.InsertSQL(mt_PRESETPARAM_MET_CONC);
//    mt_PRESETPARAM_MET_CONC.DATA_ITEM_NAME = "清空事件";
//    array[2] = mt_PRESETPARAM_MET_CONCService.InsertSQL(mt_PRESETPARAM_MET_CONC);
//    return array;
//}

//// Token: 0x060009D7 RID: 2519 RVA: 0x0006DF70 File Offset: 0x0006C170
//public static string[] GetMT_PASSWORD_CHANGE_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string[] array = new string[2];
//    MT_PASSWORD_CHANGE_MET_CONCService mt_PASSWORD_CHANGE_MET_CONCService = new MT_PASSWORD_CHANGE_MET_CONCService();
//    MT_PASSWORD_CHANGE_MET_CONC mt_PASSWORD_CHANGE_MET_CONC = new MT_PASSWORD_CHANGE_MET_CONC();
//    mt_PASSWORD_CHANGE_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_PASSWORD_CHANGE_MET_CONC.EQUIP_CATEG = "01";
//    mt_PASSWORD_CHANGE_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_PASSWORD_CHANGE_MET_CONC.DETECT_EQUIP_NO = meterInfo._intTaiNo;
//    mt_PASSWORD_CHANGE_MET_CONC.DETECT_UNIT_NO = string.Empty;
//    mt_PASSWORD_CHANGE_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//    mt_PASSWORD_CHANGE_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//    mt_PASSWORD_CHANGE_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//    mt_PASSWORD_CHANGE_MET_CONC.PARA_INDEX = "01";
//    mt_PASSWORD_CHANGE_MET_CONC.DETECT_ITEM_POINT = "01";
//    mt_PASSWORD_CHANGE_MET_CONC.CONC_CODE = "01";
//    mt_PASSWORD_CHANGE_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//    mt_PASSWORD_CHANGE_MET_CONC.HANDLE_FLAG = "0";
//    mt_PASSWORD_CHANGE_MET_CONC.PASSWORD_LEVEL = "02级";
//    array[0] = mt_PASSWORD_CHANGE_MET_CONCService.InsertSQL(mt_PASSWORD_CHANGE_MET_CONC);
//    mt_PASSWORD_CHANGE_MET_CONC.PASSWORD_LEVEL = "04级";
//    array[1] = mt_PASSWORD_CHANGE_MET_CONCService.InsertSQL(mt_PASSWORD_CHANGE_MET_CONC);
//    return array;
//}

//// Token: 0x060009D8 RID: 2520 RVA: 0x0006E08C File Offset: 0x0006C28C
//public static string GetMT_ADDRESS_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    MT_ADDRESS_MET_CONC mt_ADDRESS_MET_CONC = new MT_ADDRESS_MET_CONC();
//    mt_ADDRESS_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_ADDRESS_MET_CONC.EQUIP_CATEG = "01";
//    mt_ADDRESS_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_ADDRESS_MET_CONC.DETECT_EQUIP_NO = meterInfo._intTaiNo;
//    mt_ADDRESS_MET_CONC.DETECT_UNIT_NO = string.Empty;
//    mt_ADDRESS_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//    mt_ADDRESS_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//    mt_ADDRESS_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//    mt_ADDRESS_MET_CONC.PARA_INDEX = "1";
//    mt_ADDRESS_MET_CONC.MET_ADDRESS = meterInfo.Mb_chrAddr;
//    mt_ADDRESS_MET_CONC.CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.通信测试);
//    mt_ADDRESS_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//    mt_ADDRESS_MET_CONC.HANDLE_FLAG = "0";
//    return new MT_ADDRESS_MET_CONCService().InsertSQL(mt_ADDRESS_MET_CONC);
//}

//// Token: 0x060009D9 RID: 2521 RVA: 0x0006E17C File Offset: 0x0006C37C
//public static string GetMT_COMMINICATE_MET_CONC(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    MT_COMMINICATE_MET_CONC mt_COMMINICATE_MET_CONC = new MT_COMMINICATE_MET_CONC();
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    mt_COMMINICATE_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_COMMINICATE_MET_CONC.EQUIP_CATEG = "01";
//    mt_COMMINICATE_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_COMMINICATE_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//    mt_COMMINICATE_MET_CONC.DETECT_UNIT_NO = string.Empty;
//    mt_COMMINICATE_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//    mt_COMMINICATE_MET_CONC.BAR_CODE = meterInfo.Mb_ChrTxm;
//    mt_COMMINICATE_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//    mt_COMMINICATE_MET_CONC.PARA_INDEX = "01";
//    mt_COMMINICATE_MET_CONC.DETECT_ITEM_POINT = "01";
//    mt_COMMINICATE_MET_CONC.CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.通信测试);
//    mt_COMMINICATE_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//    mt_COMMINICATE_MET_CONC.HANDLE_FLAG = "0";
//    return new MT_COMMINICATE_MET_CONCService().InsertSQL(mt_COMMINICATE_MET_CONC);
//}

//// Token: 0x060009DA RID: 2522 RVA: 0x0006E27C File Offset: 0x0006C47C
//public static string[] GetMT_BASICERR_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    int num = 0;
//    string[] array = new string[meterInfo.MeterErrors.Keys.Count];
//    string[] array2 = new string[meterInfo.MeterErrors.Keys.Count];
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    meterInfo.MeterErrors.Keys.CopyTo(array2, 0);
//    for (int i = 0; i < array2.Length; i++)
//    {
//        string text = array2[i];
//        if (text.Length != 3)
//        {
//            MeterError meterError = meterInfo.MeterErrors[text];
//            string[] array3 = meterError.Me_chrWcMore.Split(new char[]
//            {
//                        '|'
//            });
//            if (array3.Length > 2)
//            {
//                string a = text.Substring(0, 1);
//                if (!(a == "2"))
//                {
//                    string text2 = text.Substring(0, 3);
//                    string text3 = text.Substring(7);
//                    string text4 = text.Substring(3, 2);
//                    string text5 = text.Substring(5, 2);
//                    MT_BASICERR_MET_CONC mt_BASICERR_MET_CONC = new MT_BASICERR_MET_CONC();
//                    mt_BASICERR_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//                    mt_BASICERR_MET_CONC.EQUIP_CATEG = "01";
//                    mt_BASICERR_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//                    mt_BASICERR_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//                    mt_BASICERR_MET_CONC.DETECT_UNIT_NO = string.Empty;
//                    mt_BASICERR_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//                    mt_BASICERR_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//                    mt_BASICERR_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//                    mt_BASICERR_MET_CONC.PARA_INDEX = "1";
//                    mt_BASICERR_MET_CONC.DETECT_ITEM_POINT = (i + 1).ToString();
//                    mt_BASICERR_MET_CONC.IS_VALID = "1";
//                    switch (int.Parse(text2.Substring(1, 1)))
//                    {
//                        case 1:
//                            meterError.Me_Glfx = "正向有功";
//                            break;
//                        case 2:
//                            meterError.Me_Glfx = "反向有功";
//                            break;
//                        case 3:
//                            meterError.Me_Glfx = "正向无功";
//                            break;
//                        case 4:
//                            meterError.Me_Glfx = "反向无功";
//                            break;
//                        default:
//                            meterError.Me_Glfx = "正向有功";
//                            break;
//                    }
//                    mt_BASICERR_MET_CONC.BOTH_WAY_POWER_FLAG = OraDataHelper.g_DicPCodeTable["powerFlag"].GetPCode(meterError.Me_Glfx);
//                    meterError.Me_intYj = int.Parse(text2.Substring(2, 1));
//                    string text6;
//                    switch (int.Parse(text2.Substring(2, 1)))
//                    {
//                        case 1:
//                            text6 = "合元";
//                            break;
//                        case 2:
//                            text6 = "A元";
//                            break;
//                        case 3:
//                            text6 = "B元";
//                            break;
//                        case 4:
//                            text6 = "C元";
//                            break;
//                        default:
//                            text6 = "合元";
//                            break;
//                    }
//                    string strName;
//                    if (text6 == "合元")
//                    {
//                        if (meterInfo.Mb_intClfs == 1)
//                        {
//                            strName = "AC";
//                        }
//                        else
//                        {
//                            strName = "ABC";
//                        }
//                    }
//                    else
//                    {
//                        strName = text6.Trim(new char[]
//                        {
//                                    '元'
//                        });
//                    }
//                    mt_BASICERR_MET_CONC.IABC = OraDataHelper.g_DicPCodeTable["currentPhaseCode"].GetPCode(strName);
//                    mt_BASICERR_MET_CONC.LOAD_VOLTAGE = OraDataHelper.g_DicPCodeTable["meterTestVolt"].GetPCode("100%Un");
//                    if (GlobalUnit.MisInterFaceType == "厚达MIS接口")
//                    {
//                        mt_BASICERR_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meterTestCurLoad"].GetPCode(meterError.Me_dblxIb);
//                        mt_BASICERR_MET_CONC.FREQ = OraDataHelper.g_DicPCodeTable["meterFreq"].GetPCode("50");
//                        mt_BASICERR_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode(meterError.Me_chrGlys.Trim());
//                    }
//                    else
//                    {
//                        mt_BASICERR_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode(meterError.Me_dblxIb);
//                        mt_BASICERR_MET_CONC.FREQ = OraDataHelper.g_DicPCodeTable["meter_Test_Freq"].GetPCode("50");
//                        mt_BASICERR_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode(meterError.Me_chrGlys.Trim());
//                    }
//                    mt_BASICERR_MET_CONC.DETECT_CIRCLE = "2";
//                    mt_BASICERR_MET_CONC.SIMPLING = "2";
//                    string text7 = "";
//                    for (int j = 0; j < array3.Length; j++)
//                    {
//                        if (!string.IsNullOrEmpty(array3[j].Trim()) && array3[j].Trim().Length != 0)
//                        {
//                            double num2 = Convert.ToDouble(array3[j]);
//                            if (num2 > 100.0)
//                            {
//                                array3[j] = "+100.0000";
//                            }
//                            else if (num2 < -100.0)
//                            {
//                                array3[j] = "-100.0000";
//                            }
//                        }
//                    }
//                    for (int j = 0; j < array3.Length - 2; j++)
//                    {
//                        text7 = text7 + array3[j] + "|";
//                    }
//                    mt_BASICERR_MET_CONC.ERROR = text7.Substring(0, text7.Length - 1);
//                    if (array3[array3.Length - 2].Length > 8)
//                    {
//                        mt_BASICERR_MET_CONC.AVE_ERR = array3[array3.Length - 2].Substring(0, 8);
//                    }
//                    else
//                    {
//                        mt_BASICERR_MET_CONC.AVE_ERR = array3[array3.Length - 2];
//                    }
//                    if (array3[array3.Length - 1].Length > 8)
//                    {
//                        mt_BASICERR_MET_CONC.INT_CONVERT_ERR = array3[array3.Length - 1].Substring(0, 8);
//                    }
//                    else
//                    {
//                        mt_BASICERR_MET_CONC.INT_CONVERT_ERR = array3[array3.Length - 1];
//                    }
//                    if (meterError.AVR_UPPER_LIMIT != null)
//                    {
//                        mt_BASICERR_MET_CONC.ERR_UP = meterError.AVR_UPPER_LIMIT.Trim().Split(new char[]
//                        {
//                                    '|'
//                        })[0];
//                    }
//                    if (meterError.AVR_UPPER_LIMIT != null)
//                    {
//                        mt_BASICERR_MET_CONC.ERR_DOWN = meterError.AVR_LOWER_LIMIT.Trim().Split(new char[]
//                        {
//                                    '|'
//                        })[0];
//                    }
//                    mt_BASICERR_MET_CONC.CONC_CODE = ((meterError.Me_chrWcJl == "合格") ? "01" : "02");
//                    mt_BASICERR_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//                    mt_BASICERR_MET_CONC.HANDLE_FLAG = "0";
//                    array[num++] = new MT_BASICERR_MET_CONCService().InsertSQL(mt_BASICERR_MET_CONC);
//                }
//            }
//        }
//    }
//    string[] array4 = new string[num];
//    Array.Copy(array, array4, num);
//    return array4;
//}

//// Token: 0x060009DB RID: 2523 RVA: 0x0006E984 File Offset: 0x0006CB84
//public static string[] GetMT_DEVIATION_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    int num = 0;
//    string[] array = new string[meterInfo.MeterErrors.Keys.Count];
//    string[] array2 = new string[meterInfo.MeterErrors.Keys.Count];
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    meterInfo.MeterErrors.Keys.CopyTo(array2, 0);
//    for (int i = 0; i < array2.Length; i++)
//    {
//        string text = array2[i];
//        if (text.Length > 3)
//        {
//            string a = text.Substring(0, 1);
//            if (!(a != "2"))
//            {
//                MeterError meterError = meterInfo.MeterErrors[text];
//                string[] array3 = meterError.Me_chrWcMore.Split(new char[]
//                {
//                            '|'
//                });
//                if (array3.Length > 2)
//                {
//                    string text2 = text.Substring(0, 3);
//                    string text3 = text.Substring(7);
//                    string text4 = text.Substring(3, 2);
//                    string text5 = text.Substring(5, 2);
//                    MT_MEASURE_REPEAT_MET_CONC mt_MEASURE_REPEAT_MET_CONC = new MT_MEASURE_REPEAT_MET_CONC();
//                    mt_MEASURE_REPEAT_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//                    mt_MEASURE_REPEAT_MET_CONC.EQUIP_CATEG = "01";
//                    mt_MEASURE_REPEAT_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//                    mt_MEASURE_REPEAT_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//                    mt_MEASURE_REPEAT_MET_CONC.DETECT_UNIT_NO = string.Empty;
//                    mt_MEASURE_REPEAT_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//                    mt_MEASURE_REPEAT_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//                    mt_MEASURE_REPEAT_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//                    mt_MEASURE_REPEAT_MET_CONC.PARA_INDEX = "1";
//                    mt_MEASURE_REPEAT_MET_CONC.DETECT_ITEM_POINT = (i + 1).ToString();
//                    mt_MEASURE_REPEAT_MET_CONC.IS_VALID = "1";
//                    mt_MEASURE_REPEAT_MET_CONC.BOTH_WAY_POWER_FLAG = text2.Substring(1, 1);
//                    string text6;
//                    switch (int.Parse(text2.Substring(2, 1)))
//                    {
//                        case 1:
//                            text6 = "A元";
//                            break;
//                        case 2:
//                            text6 = "B元";
//                            break;
//                        case 3:
//                            text6 = "C元";
//                            break;
//                        case 4:
//                            text6 = "合元";
//                            break;
//                        default:
//                            text6 = "合元";
//                            break;
//                    }
//                    if (text6 == "合元")
//                    {
//                        if (meterInfo.Mb_intClfs == 1)
//                        {
//                            text6 = "AC";
//                        }
//                        else
//                        {
//                            text6 = "ABC";
//                        }
//                    }
//                    else
//                    {
//                        text6 = text6.Trim(new char[]
//                        {
//                                    '元'
//                        });
//                    }
//                    mt_MEASURE_REPEAT_MET_CONC.IABC = OraDataHelper.g_DicPCodeTable["currentPhaseCode"].GetPCode(text6);
//                    mt_MEASURE_REPEAT_MET_CONC.LOAD_VOLTAGE = OraDataHelper.g_DicPCodeTable["meterTestVolt"].GetPCode("100%Un");
//                    if (GlobalUnit.MisInterFaceType == "厚达MIS接口")
//                    {
//                        mt_MEASURE_REPEAT_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meterTestCurLoad"].GetPCode(meterError.Me_dblxIb);
//                        mt_MEASURE_REPEAT_MET_CONC.FREQ = OraDataHelper.g_DicPCodeTable["meterFreq"].GetPCode("50");
//                        mt_MEASURE_REPEAT_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode(meterError.Me_chrGlys);
//                    }
//                    else
//                    {
//                        mt_MEASURE_REPEAT_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode(meterError.Me_dblxIb);
//                        mt_MEASURE_REPEAT_MET_CONC.FREQ = OraDataHelper.g_DicPCodeTable["meter_Test_Freq"].GetPCode("50");
//                        mt_MEASURE_REPEAT_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode(meterError.Me_chrGlys.Trim());
//                    }
//                    string[] array4 = meterError.Me_chrWcMore.Split(new char[]
//                    {
//                                '|'
//                    });
//                    if (array4.Length >= 6)
//                    {
//                        mt_MEASURE_REPEAT_MET_CONC.SIMPLING = string.Concat(new string[]
//                        {
//                                    array4[0],
//                                    "|",
//                                    array4[1],
//                                    "|",
//                                    array4[2],
//                                    "|",
//                                    array4[3],
//                                    "|",
//                                    array4[4]
//                        });
//                        mt_MEASURE_REPEAT_MET_CONC.DEVIATION_LIMT = array4[5];
//                    }
//                    mt_MEASURE_REPEAT_MET_CONC.CONC_CODE = ((meterError.Me_chrWcJl == "合格") ? "01" : "02");
//                    mt_MEASURE_REPEAT_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//                    mt_MEASURE_REPEAT_MET_CONC.HANDLE_FLAG = "0";
//                    array[num++] = new MT_MEASURE_REPEAT_MET_CONCService().InsertSQL(mt_MEASURE_REPEAT_MET_CONC);
//                }
//            }
//        }
//    }
//    string[] array5 = new string[num];
//    Array.Copy(array, array5, num);
//    return array5;
//}

//// Token: 0x060009DC RID: 2524 RVA: 0x0006EE7C File Offset: 0x0006D07C
//public static string[] GetMT_STARTING_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    int num = 0;
//    int num2 = 0;
//    string strName = "";
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    Dictionary<string, MeterQdQid> meterQdQids = meterInfo.MeterQdQids;
//    foreach (string text in meterQdQids.Keys)
//    {
//        if (text.Length > 3 && text.Substring(0, 3) == 109.ToString())
//        {
//            num2++;
//        }
//    }
//    string[] array = new string[num2];
//    foreach (string text in meterQdQids.Keys)
//    {
//        if (text.Length > 3 && text.Substring(0, 3) == 109.ToString())
//        {
//            MT_STARTING_MET_CONC mt_STARTING_MET_CONC = new MT_STARTING_MET_CONC();
//            mt_STARTING_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//            mt_STARTING_MET_CONC.EQUIP_CATEG = "01";
//            mt_STARTING_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//            mt_STARTING_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//            mt_STARTING_MET_CONC.DETECT_UNIT_NO = string.Empty;
//            mt_STARTING_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//            mt_STARTING_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//            mt_STARTING_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//            mt_STARTING_MET_CONC.PARA_INDEX = num.ToString();
//            mt_STARTING_MET_CONC.DETECT_ITEM_POINT = num.ToString();
//            mt_STARTING_MET_CONC.IS_VALID = "1";
//            mt_STARTING_MET_CONC.LOAD_VOLTAGE = OraDataHelper.g_DicPCodeTable["meterTestVolt"].GetPCode("100%Un");
//            mt_STARTING_MET_CONC.PULES = "1";
//            if (meterInfo.MeterQdQids[text].Mqd_chrProjectName.IndexOf("正向有功") != -1)
//            {
//                strName = "正向有功";
//            }
//            else if (meterInfo.MeterQdQids[text].Mqd_chrProjectName.IndexOf("反向有功") != -1)
//            {
//                strName = "反向有功";
//            }
//            else if (meterInfo.MeterQdQids[text].Mqd_chrProjectName.IndexOf("正向无功") != -1)
//            {
//                strName = "正向无功";
//            }
//            else if (meterInfo.MeterQdQids[text].Mqd_chrProjectName.IndexOf("反向无功") != -1)
//            {
//                strName = "反向无功";
//            }
//            mt_STARTING_MET_CONC.BOTH_WAY_POWER_FLAG = OraDataHelper.g_DicPCodeTable["powerFlag"].GetPCode(strName);
//            string strName2 = Convert.ToSingle(meterQdQids[text].Mqd_chrDL).ToString("F3") + "Ib";
//            if (GlobalUnit.MisInterFaceType == "厚达MIS接口")
//            {
//                mt_STARTING_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meterTestCurLoad"].GetPCode(strName2);
//            }
//            else
//            {
//                mt_STARTING_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode(strName2);
//            }
//            if (meterQdQids[text].AVR_ACTUAL_TIME != "")
//            {
//                mt_STARTING_MET_CONC.TEST_TIME = (Convert.ToSingle(meterQdQids[text].AVR_ACTUAL_TIME) * 60f).ToString("F0");
//            }
//            if (meterQdQids[text].AVR_STANDARD_TIME != "")
//            {
//                mt_STARTING_MET_CONC.REAL_TEST_TIME = (Convert.ToSingle(meterQdQids[text].AVR_STANDARD_TIME) * 60f).ToString("F0");
//            }
//            mt_STARTING_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//            mt_STARTING_MET_CONC.HANDLE_FLAG = "0";
//            mt_STARTING_MET_CONC.START_CURRENT = (Convert.ToSingle(meterQdQids[text].Mqd_chrDL) * meterInfo.GetIb()[0]).ToString();
//            mt_STARTING_MET_CONC.CONC_CODE = "01";
//            array[num++] = new MT_STARTING_MET_CONCService().InsertSQL(mt_STARTING_MET_CONC);
//        }
//    }
//    return array;
//}

//// Token: 0x060009DD RID: 2525 RVA: 0x0006F32C File Offset: 0x0006D52C
//public static string[] GetMT_CREEPING_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    int num = 0;
//    int num2 = 0;
//    string strName = "";
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    Dictionary<string, MeterQdQid> meterQdQids = meterInfo.MeterQdQids;
//    foreach (string text in meterQdQids.Keys)
//    {
//        if (text.Length > 3 && text.Substring(0, 3) == 110.ToString())
//        {
//            num2++;
//        }
//    }
//    string[] array = new string[num2];
//    foreach (string text in meterQdQids.Keys)
//    {
//        if (text.Length > 3 && text.Substring(0, 3) == 110.ToString())
//        {
//            MT_CREEPING_MET_CONC mt_CREEPING_MET_CONC = new MT_CREEPING_MET_CONC();
//            mt_CREEPING_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//            mt_CREEPING_MET_CONC.EQUIP_CATEG = "01";
//            mt_CREEPING_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//            mt_CREEPING_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//            mt_CREEPING_MET_CONC.DETECT_UNIT_NO = string.Empty;
//            mt_CREEPING_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//            mt_CREEPING_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//            mt_CREEPING_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//            mt_CREEPING_MET_CONC.PARA_INDEX = num.ToString();
//            mt_CREEPING_MET_CONC.DETECT_ITEM_POINT = num.ToString();
//            mt_CREEPING_MET_CONC.IS_VALID = "1";
//            mt_CREEPING_MET_CONC.LOAD_VOLTAGE = OraDataHelper.g_DicPCodeTable["meterTestVolt"].GetPCode("115%Un");
//            mt_CREEPING_MET_CONC.PULES = "0";
//            if (meterInfo.MeterQdQids[text].Mqd_chrProjectName.IndexOf("正向有功") != -1)
//            {
//                strName = "正向有功";
//            }
//            else if (meterInfo.MeterQdQids[text].Mqd_chrProjectName.IndexOf("反向有功") != -1)
//            {
//                strName = "反向有功";
//            }
//            else if (meterInfo.MeterQdQids[text].Mqd_chrProjectName.IndexOf("正向无功") != -1)
//            {
//                strName = "正向无功";
//            }
//            else if (meterInfo.MeterQdQids[text].Mqd_chrProjectName.IndexOf("反向无功") != -1)
//            {
//                strName = "正向无功";
//            }
//            mt_CREEPING_MET_CONC.BOTH_WAY_POWER_FLAG = OraDataHelper.g_DicPCodeTable["powerFlag"].GetPCode(strName);
//            if (GlobalUnit.MisInterFaceType == "厚达MIS接口")
//            {
//                mt_CREEPING_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meterTestCurLoad"].GetPCode(meterQdQids[text].Mqd_chrDL);
//            }
//            else
//            {
//                mt_CREEPING_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode("0");
//            }
//            if (meterQdQids[text].AVR_ACTUAL_TIME != "")
//            {
//                mt_CREEPING_MET_CONC.TEST_TIME = (Convert.ToSingle(meterQdQids[text].AVR_ACTUAL_TIME) * 60f).ToString("F0");
//            }
//            if (meterQdQids[text].AVR_STANDARD_TIME != "")
//            {
//                mt_CREEPING_MET_CONC.REAL_TEST_TIME = (Convert.ToSingle(meterQdQids[text].AVR_STANDARD_TIME) * 60f).ToString("F0");
//            }
//            mt_CREEPING_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//            mt_CREEPING_MET_CONC.HANDLE_FLAG = "0";
//            mt_CREEPING_MET_CONC.CONC_CODE = ((meterInfo.MeterQdQids[text].Mqd_chrJL.Trim() == "合格") ? "01" : "02");
//            array[num++] = new MT_CREEPING_MET_CONCService().InsertSQL(mt_CREEPING_MET_CONC);
//        }
//    }
//    return array;
//}

//// Token: 0x060009DE RID: 2526 RVA: 0x0006F7B8 File Offset: 0x0006D9B8
//public static string[] GetMT_STANDARD_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    int num = 0;
//    int num2 = 0;
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    Dictionary<string, MeterDLTData> meterDLTDatas = meterInfo.MeterDLTDatas;
//    foreach (string key in meterDLTDatas.Keys)
//    {
//        num2++;
//    }
//    string[] array = new string[num2];
//    foreach (string key in meterDLTDatas.Keys)
//    {
//        MT_STANDARD_MET_CONC mt_STANDARD_MET_CONC = new MT_STANDARD_MET_CONC();
//        mt_STANDARD_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_STANDARD_MET_CONC.EQUIP_CATEG = "01";
//        mt_STANDARD_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_STANDARD_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//        mt_STANDARD_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_STANDARD_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_STANDARD_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_STANDARD_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_STANDARD_MET_CONC.PARA_INDEX = num.ToString();
//        mt_STANDARD_MET_CONC.DETECT_ITEM_POINT = num.ToString();
//        mt_STANDARD_MET_CONC.IS_VALID = "1";
//        mt_STANDARD_MET_CONC.DATA_FLAG = meterDLTDatas[key].AVR_DI0_DI3;
//        mt_STANDARD_MET_CONC.DETECT_BASIS = "";
//        mt_STANDARD_MET_CONC.SETTING_VALUE = "";
//        mt_STANDARD_MET_CONC.READ_VALUE = meterDLTDatas[key].Mdlt_chrValue.Trim();
//        mt_STANDARD_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_STANDARD_MET_CONC.HANDLE_FLAG = "0";
//        mt_STANDARD_MET_CONC.CONC_CODE = ((meterDLTDatas[key].AVR_CONC.Trim() == "合格") ? "01" : "02");
//        array[num++] = new MT_STANDARD_MET_CONCService().InsertSQL(mt_STANDARD_MET_CONC);
//    }
//    return array;
//}

//// Token: 0x060009DF RID: 2527 RVA: 0x0006FA14 File Offset: 0x0006DC14
//public static string[] GetMT_CONST_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    DataTable dataTable = new DataTable();
//    dataTable.Columns.Add("Keys", typeof(string));
//    dataTable.Columns.Add("PrjId", typeof(string));
//    foreach (string text in meterInfo.MeterZZErrors.Keys)
//    {
//        MeterZZError meterZZError = meterInfo.MeterZZErrors[text];
//        dataTable.Rows.Add(new object[]
//        {
//                    text,
//                    meterZZError.Me_chrProjectNo
//        });
//    }
//    DataRow[] array = dataTable.Select("Keys <>'' and PrjId <> ''", "PrjId asc");
//    string[] array2 = new string[array.Length];
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    for (int i = 0; i < array.Length; i++)
//    {
//        MeterZZError meterZZError2 = meterInfo.MeterZZErrors[array[i][0].ToString()];
//        string me_chrProjectNo = meterZZError2.Me_chrProjectNo;
//        switch (int.Parse(me_chrProjectNo[0].ToString()))
//        {
//            case 1:
//                meterZZError2.Mz_chrJdfx = "正向有功";
//                break;
//            case 2:
//                meterZZError2.Mz_chrJdfx = "正向无功";
//                break;
//            case 3:
//                meterZZError2.Mz_chrJdfx = "反向有功";
//                break;
//            case 4:
//                meterZZError2.Mz_chrJdfx = "反向无功";
//                break;
//            default:
//                meterZZError2.Mz_chrJdfx = "正向有功";
//                break;
//        }
//        MT_CONST_MET_CONC mt_CONST_MET_CONC = new MT_CONST_MET_CONC();
//        mt_CONST_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_CONST_MET_CONC.EQUIP_CATEG = "01";
//        mt_CONST_MET_CONC.SYS_NO = (i + 1).ToString();
//        mt_CONST_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//        mt_CONST_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_CONST_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_CONST_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_CONST_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_CONST_MET_CONC.DETECT_ITEM_POINT = (i + 1).ToString();
//        mt_CONST_MET_CONC.IS_VALID = "1";
//        mt_CONST_MET_CONC.PARA_INDEX = (i + 1).ToString();
//        mt_CONST_MET_CONC.VOLT = OraDataHelper.g_DicPCodeTable["meterTestVolt"].GetPCode("100%Un");
//        if (GlobalUnit.MisInterFaceType == "厚达MIS接口")
//        {
//            mt_CONST_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meterTestCurLoad"].GetPCode(meterZZError2.Mz_chrxIbString);
//        }
//        else
//        {
//            mt_CONST_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode(meterZZError2.Mz_chrxIbString);
//        }
//        mt_CONST_MET_CONC.REAL_PULES = "";
//        mt_CONST_MET_CONC.DIFF_READING = meterZZError2.Mz_chrQiZiMaC.ToString().Trim();
//        mt_CONST_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode(meterZZError2.Mz_chrGlys.Trim());
//        mt_CONST_MET_CONC.FEE_RATIO = meterZZError2.Mz_chrFl.Trim();
//        mt_CONST_MET_CONC.START_READING = meterZZError2.Mz_chrQiMa.ToString();
//        mt_CONST_MET_CONC.END_READING = meterZZError2.Mz_chrZiMa.ToString();
//        mt_CONST_MET_CONC.ERROR = meterZZError2.Mz_chrWc.Trim();
//        mt_CONST_MET_CONC.STANDARD_READING = (Convert.ToSingle(meterZZError2.AVR_STANDARD_METER_ENERGY) / (float)meterInfo.GetBcs()[0]).ToString("F4");
//        mt_CONST_MET_CONC.CONC_CODE = ((meterZZError2.Mz_chrJL.Trim() == "合格") ? "01" : "02");
//        mt_CONST_MET_CONC.REAL_PULES = meterZZError2.Mz_chrPules.Trim();
//        mt_CONST_MET_CONC.QUALIFIED_PULES = meterZZError2.AVR_STANDARD_METER_ENERGY.Trim();
//        string[] dj = Number.getDj(meterInfo.Mb_chrBdj);
//        mt_CONST_MET_CONC.ERR_UP = (Convert.ToDouble(dj[0]) * 1.0).ToString("0.0");
//        mt_CONST_MET_CONC.ERR_DOWN = (Convert.ToDouble(dj[0]) * -1.0).ToString("0.0");
//        mt_CONST_MET_CONC.CONST_ERR = meterZZError2.Mz_chrWc.Trim();
//        mt_CONST_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_CONST_MET_CONC.HANDLE_FLAG = "0";
//        if (GlobalUnit.MisInterFaceType != "厚达MIS接口")
//        {
//            mt_CONST_MET_CONC.IR_LAST_READING = "0";
//        }
//        mt_CONST_MET_CONC.READ_TYPE_CODE = "";
//        array2[i] = new MT_CONST_MET_CONCService().InsertSQL(mt_CONST_MET_CONC);
//        mt_CONST_MET_CONC.BOTH_WAY_POWER_FLAG = OraDataHelper.g_DicPCodeTable["powerFlag"].GetPCode(meterZZError2.Mz_chrJdfx);
//        mt_CONST_MET_CONC.ERROR = meterZZError2.Mz_chrWc.Trim();
//        mt_CONST_MET_CONC.STANDARD_READING = (Convert.ToSingle(meterZZError2.AVR_STANDARD_METER_ENERGY) / (float)meterInfo.GetBcs()[0]).ToString("F4");
//        mt_CONST_MET_CONC.CONC_CODE = ((meterZZError2.Mz_chrJL.Trim() == "合格") ? "01" : "02");
//        mt_CONST_MET_CONC.REAL_PULES = meterZZError2.Mz_chrPules.Trim();
//        mt_CONST_MET_CONC.QUALIFIED_PULES = meterZZError2.AVR_STANDARD_METER_ENERGY.Trim();
//        mt_CONST_MET_CONC.ERR_UP = "2";
//        mt_CONST_MET_CONC.ERR_DOWN = "-2";
//        mt_CONST_MET_CONC.CONST_ERR = meterZZError2.Mz_chrWc.Trim();
//        mt_CONST_MET_CONC.CONTROL_METHOD = OraDataHelper.g_DicPCodeTable["meterTestCtrlMode"].GetPCode("标准表法");
//        mt_CONST_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_CONST_MET_CONC.HANDLE_FLAG = "0";
//        array2[i] = new MT_CONST_MET_CONCService().InsertSQL(mt_CONST_MET_CONC);
//    }
//    return array2;
//}

//// Token: 0x060009E0 RID: 2528 RVA: 0x00070058 File Offset: 0x0006E258
//public static string GetMT_DAYERR_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string error = "";
//    string avg_ERR = "";
//    string int_CONVERT_ERR = "";
//    string conc_CODE = "";
//    string text = 2.ToString().PadLeft(3, '0');
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    string key = text + "01";
//    if (meterInfo.MeterDgns.ContainsKey(key))
//    {
//        string[] array = meterInfo.MeterDgns[key].Md_chrValue.Split(new char[]
//        {
//                    '|'
//        });
//        if (array.Length > 1)
//        {
//            avg_ERR = array[0];
//            int_CONVERT_ERR = array[1];
//        }
//    }
//    key = text + "02";
//    if (meterInfo.MeterDgns.ContainsKey(key))
//    {
//        error = meterInfo.MeterDgns[key].Md_chrValue + "|";
//    }
//    text = 2.ToString("D3");
//    if (meterInfo.MeterDgns.ContainsKey(text))
//    {
//        conc_CODE = ((meterInfo.MeterDgns[text].Md_chrValue == "合格") ? "01" : "02");
//    }
//    MT_DAYERR_MET_CONC mt_DAYERR_MET_CONC = new MT_DAYERR_MET_CONC();
//    mt_DAYERR_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_DAYERR_MET_CONC.EQUIP_CATEG = "01";
//    mt_DAYERR_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_DAYERR_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//    mt_DAYERR_MET_CONC.DETECT_UNIT_NO = string.Empty;
//    mt_DAYERR_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//    mt_DAYERR_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//    mt_DAYERR_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//    mt_DAYERR_MET_CONC.PARA_INDEX = "1";
//    mt_DAYERR_MET_CONC.DETECT_ITEM_POINT = "01";
//    mt_DAYERR_MET_CONC.IS_VALID = "1";
//    mt_DAYERR_MET_CONC.SEC_PILES = "1";
//    mt_DAYERR_MET_CONC.TEST_TIME = "120";
//    mt_DAYERR_MET_CONC.SIMPLING = "5";
//    mt_DAYERR_MET_CONC.ERROR = error;
//    mt_DAYERR_MET_CONC.AVG_ERR = avg_ERR;
//    mt_DAYERR_MET_CONC.INT_CONVERT_ERR = int_CONVERT_ERR;
//    mt_DAYERR_MET_CONC.ERR_ABS = "0.5";
//    mt_DAYERR_MET_CONC.CONC_CODE = conc_CODE;
//    mt_DAYERR_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//    mt_DAYERR_MET_CONC.HANDLE_FLAG = "0";
//    return new MT_DAYERR_MET_CONCService().InsertSQL(mt_DAYERR_MET_CONC);
//}

//// Token: 0x060009E1 RID: 2529 RVA: 0x000702F0 File Offset: 0x0006E4F0
//public static string[] GetMT_TS_MET_CONC(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    int num = 0;
//    string[] array = new string[10];
//    string text = 4.ToString("D3");
//    string text2 = "";
//    if (meterInfo.MeterDgns.ContainsKey(text))
//    {
//        text2 = ((meterInfo.MeterDgns[text].Md_chrValue == "合格") ? "01" : "02");
//    }
//    MT_TS_MET_CONC mt_TS_MET_CONC = new MT_TS_MET_CONC();
//    mt_TS_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_TS_MET_CONC.EQUIP_CATEG = "01";
//    mt_TS_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_TS_MET_CONC.DETECT_EQUIP_NO = meterInfo._intTaiNo;
//    mt_TS_MET_CONC.DETECT_UNIT_NO = string.Empty;
//    mt_TS_MET_CONC.BAR_CODE = meterInfo.Mb_ChrTxm;
//    mt_TS_MET_CONC.DETECT_DATE = DateTime.Now.ToString();
//    mt_TS_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//    mt_TS_MET_CONC.IS_VALID = "1";
//    mt_TS_MET_CONC.CONC_CODE = text2;
//    mt_TS_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//    mt_TS_MET_CONC.HANDLE_FLAG = "0";
//    for (int i = 0; i < 10; i++)
//    {
//        string key = text + (i + 1).ToString("D2");
//        if (meterInfo.MeterDgns.ContainsKey(key))
//        {
//            string[] array2 = meterInfo.MeterDgns[key].Md_chrValue.Split(new char[]
//            {
//                        '|'
//            });
//            if (array2.Length >= 4)
//            {
//                mt_TS_MET_CONC.FEE = OraDataHelper.g_DicPCodeTable["tari_ff"].GetPCode(array2[3]);
//                mt_TS_MET_CONC.START_TIME = array2[0];
//                mt_TS_MET_CONC.TS_START_TIME = array2[0];
//                mt_TS_MET_CONC.TS_REAL_TIME = array2[1];
//                mt_TS_MET_CONC.TS_ERR_CONC_CODE = text2;
//                mt_TS_MET_CONC.TS_ERR = array2[2];
//                mt_TS_MET_CONC.ERR_UP = "+300";
//                mt_TS_MET_CONC.ERR_DOWM = "-300";
//                mt_TS_MET_CONC.TS_WAY = "电量";
//                mt_TS_MET_CONC.PARA_INDEX = (num + 1).ToString();
//                mt_TS_MET_CONC.DETECT_ITEM_POINT = (num + 1).ToString();
//                mt_TS_MET_CONC.VOLT = "220";
//                array[num++] = new MT_TS_MET_CONCService().InsertSQL(mt_TS_MET_CONC);
//            }
//        }
//    }
//    string[] array3 = new string[num];
//    Array.Copy(array, array3, num);
//    return array3;
//}

//// Token: 0x060009E2 RID: 2530 RVA: 0x000705A8 File Offset: 0x0006E7A8
//public static string[] GetMT_DEMANDVALUE_MET_CONC(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string[] array = new string[3];
//    MT_DEMANDVALUE_MET_CONCService mt_DEMANDVALUE_MET_CONCService = new MT_DEMANDVALUE_MET_CONCService();
//    MT_DEMANDVALUE_MET_CONC mt_DEMANDVALUE_MET_CONC = new MT_DEMANDVALUE_MET_CONC();
//    string[] xldata = DataManager.GetXLData(meterInfo);
//    if ((xldata[0] != null && xldata[0] != "") || (xldata[1] != null && xldata[1] != ""))
//    {
//        string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//        mt_DEMANDVALUE_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_DEMANDVALUE_MET_CONC.EQUIP_CATEG = "01";
//        mt_DEMANDVALUE_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_DEMANDVALUE_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//        mt_DEMANDVALUE_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_DEMANDVALUE_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_DEMANDVALUE_MET_CONC.BAR_CODE = meterInfo.Mb_ChrTxm;
//        mt_DEMANDVALUE_MET_CONC.DETECT_DATE = DateTime.Now.ToString();
//        mt_DEMANDVALUE_MET_CONC.PARA_INDEX = "1";
//        mt_DEMANDVALUE_MET_CONC.IS_VALID = "1";
//        mt_DEMANDVALUE_MET_CONC.HANDLE_FLAG = "0";
//        mt_DEMANDVALUE_MET_CONC.HANDLE_DATE = "";
//        mt_DEMANDVALUE_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_DEMANDVALUE_MET_CONC.DEMAND_PERIOD = "15";
//        mt_DEMANDVALUE_MET_CONC.DEMAND_TIME = "1";
//        mt_DEMANDVALUE_MET_CONC.DEMAND_VALUE_ERR_ABS = mtMeter.AP_PRE_LEVEL_CODE;
//        mt_DEMANDVALUE_MET_CONC.CLEAR_DATA_RST = ((xldata[4] == "合格") ? "01" : "02");
//        mt_DEMANDVALUE_MET_CONC.DEMAND_INTERVAL = "1";
//        mt_DEMANDVALUE_MET_CONC.VALUE_CONC_CODE = ((xldata[4] == "合格") ? "01" : "02");
//        mt_DEMANDVALUE_MET_CONC.DETECT_ITEM_POINT = "1";
//        mt_DEMANDVALUE_MET_CONC.BOTH_WAY_POWER_FLAG = "0";
//        mt_DEMANDVALUE_MET_CONC.VOLTAGE = "100%Un";
//        mt_DEMANDVALUE_MET_CONC.REAL_DEMAND = xldata[1];
//        mt_DEMANDVALUE_MET_CONC.DEMAND_STANDARD = xldata[0].Substring(0, 7);
//        mt_DEMANDVALUE_MET_CONC.REAL_PERIOD = "15";
//        mt_DEMANDVALUE_MET_CONC.CONTROL_METHOD = "01";
//        mt_DEMANDVALUE_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode("1.0");
//        mt_DEMANDVALUE_MET_CONC.ERR_UP = OraDataHelper.g_DicPCodeTable["meterAccuracy"].GetPName(mtMeter.AP_PRE_LEVEL_CODE);
//        mt_DEMANDVALUE_MET_CONC.ERR_DOWM = OraDataHelper.g_DicPCodeTable["meterAccuracy"].GetPName(mtMeter.AP_PRE_LEVEL_CODE);
//        mt_DEMANDVALUE_MET_CONC.CHK_CONC_CODE = ((xldata[4] == "合格") ? "01" : "02");
//        mt_DEMANDVALUE_MET_CONC.LOAD_CURRENT = "08";
//        mt_DEMANDVALUE_MET_CONC.DEMAND_VALUE_ERR = xldata[2];
//        array[0] = mt_DEMANDVALUE_MET_CONCService.InsertSQL(mt_DEMANDVALUE_MET_CONC);
//    }
//    if ((xldata[5] != null && xldata[5] != "") || (xldata[6] != null && xldata[6] != ""))
//    {
//        string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//        mt_DEMANDVALUE_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_DEMANDVALUE_MET_CONC.EQUIP_CATEG = "01";
//        if (meterInfo.Mb_intClfs == 5)
//        {
//            mt_DEMANDVALUE_MET_CONC.SYS_NO = "401";
//        }
//        else
//        {
//            mt_DEMANDVALUE_MET_CONC.SYS_NO = "402";
//        }
//        mt_DEMANDVALUE_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_DEMANDVALUE_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//        mt_DEMANDVALUE_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_DEMANDVALUE_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_DEMANDVALUE_MET_CONC.BAR_CODE = meterInfo.Mb_ChrTxm;
//        mt_DEMANDVALUE_MET_CONC.DETECT_DATE = DateTime.Now.ToString();
//        mt_DEMANDVALUE_MET_CONC.PARA_INDEX = "2";
//        mt_DEMANDVALUE_MET_CONC.IS_VALID = "1";
//        mt_DEMANDVALUE_MET_CONC.HANDLE_FLAG = "0";
//        mt_DEMANDVALUE_MET_CONC.HANDLE_DATE = "";
//        mt_DEMANDVALUE_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_DEMANDVALUE_MET_CONC.DEMAND_PERIOD = "15";
//        mt_DEMANDVALUE_MET_CONC.DEMAND_TIME = "1";
//        mt_DEMANDVALUE_MET_CONC.DEMAND_VALUE_ERR_ABS = mtMeter.AP_PRE_LEVEL_CODE;
//        mt_DEMANDVALUE_MET_CONC.CLEAR_DATA_RST = ((xldata[9] == "合格") ? "01" : "02");
//        mt_DEMANDVALUE_MET_CONC.DEMAND_INTERVAL = "1";
//        mt_DEMANDVALUE_MET_CONC.VALUE_CONC_CODE = ((xldata[9] == "合格") ? "01" : "02");
//        mt_DEMANDVALUE_MET_CONC.DETECT_ITEM_POINT = "2";
//        mt_DEMANDVALUE_MET_CONC.BOTH_WAY_POWER_FLAG = "0";
//        mt_DEMANDVALUE_MET_CONC.VOLTAGE = "100%Un";
//        mt_DEMANDVALUE_MET_CONC.REAL_DEMAND = xldata[5].Substring(0, 7);
//        mt_DEMANDVALUE_MET_CONC.DEMAND_STANDARD = xldata[6];
//        mt_DEMANDVALUE_MET_CONC.REAL_PERIOD = "15";
//        mt_DEMANDVALUE_MET_CONC.CONTROL_METHOD = "01";
//        mt_DEMANDVALUE_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode("1.0");
//        mt_DEMANDVALUE_MET_CONC.ERR_UP = OraDataHelper.g_DicPCodeTable["meterAccuracy"].GetPName(mtMeter.AP_PRE_LEVEL_CODE);
//        mt_DEMANDVALUE_MET_CONC.ERR_DOWM = OraDataHelper.g_DicPCodeTable["meterAccuracy"].GetPName(mtMeter.AP_PRE_LEVEL_CODE);
//        mt_DEMANDVALUE_MET_CONC.CHK_CONC_CODE = ((xldata[9] == "合格") ? "01" : "02");
//        mt_DEMANDVALUE_MET_CONC.LOAD_CURRENT = "05";
//        mt_DEMANDVALUE_MET_CONC.DEMAND_VALUE_ERR = xldata[7];
//        array[1] = mt_DEMANDVALUE_MET_CONCService.InsertSQL(mt_DEMANDVALUE_MET_CONC);
//    }
//    if (xldata[12] != null && xldata[12] != "")
//    {
//        string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//        mt_DEMANDVALUE_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_DEMANDVALUE_MET_CONC.EQUIP_CATEG = "01";
//        mt_DEMANDVALUE_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_DEMANDVALUE_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//        mt_DEMANDVALUE_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_DEMANDVALUE_MET_CONC.BAR_CODE = meterInfo.Mb_ChrTxm;
//        mt_DEMANDVALUE_MET_CONC.DETECT_DATE = DateTime.Now.ToString();
//        mt_DEMANDVALUE_MET_CONC.PARA_INDEX = "3";
//        mt_DEMANDVALUE_MET_CONC.IS_VALID = "1";
//        mt_DEMANDVALUE_MET_CONC.HANDLE_FLAG = "0";
//        mt_DEMANDVALUE_MET_CONC.HANDLE_DATE = "";
//        mt_DEMANDVALUE_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_DEMANDVALUE_MET_CONC.DEMAND_PERIOD = "15";
//        mt_DEMANDVALUE_MET_CONC.DEMAND_TIME = "1";
//        mt_DEMANDVALUE_MET_CONC.DEMAND_VALUE_ERR_ABS = mtMeter.AP_PRE_LEVEL_CODE;
//        mt_DEMANDVALUE_MET_CONC.CLEAR_DATA_RST = ((xldata[14] == "合格") ? "01" : "02");
//        mt_DEMANDVALUE_MET_CONC.DEMAND_INTERVAL = "1";
//        mt_DEMANDVALUE_MET_CONC.VALUE_CONC_CODE = ((xldata[14] == "合格") ? "01" : "02");
//        mt_DEMANDVALUE_MET_CONC.DETECT_ITEM_POINT = "3";
//        mt_DEMANDVALUE_MET_CONC.BOTH_WAY_POWER_FLAG = "0";
//        mt_DEMANDVALUE_MET_CONC.VOLTAGE = "100%Un";
//        mt_DEMANDVALUE_MET_CONC.REAL_DEMAND = xldata[10].Substring(0, 7);
//        mt_DEMANDVALUE_MET_CONC.DEMAND_STANDARD = xldata[11];
//        mt_DEMANDVALUE_MET_CONC.REAL_PERIOD = "15";
//        mt_DEMANDVALUE_MET_CONC.CONTROL_METHOD = "01";
//        mt_DEMANDVALUE_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode("1.0");
//        mt_DEMANDVALUE_MET_CONC.ERR_UP = OraDataHelper.g_DicPCodeTable["meterAccuracy"].GetPName(mtMeter.AP_PRE_LEVEL_CODE);
//        mt_DEMANDVALUE_MET_CONC.ERR_DOWM = OraDataHelper.g_DicPCodeTable["meterAccuracy"].GetPName(mtMeter.AP_PRE_LEVEL_CODE);
//        mt_DEMANDVALUE_MET_CONC.CHK_CONC_CODE = ((xldata[14] == "合格") ? "01" : "02");
//        mt_DEMANDVALUE_MET_CONC.LOAD_CURRENT = "00";
//        mt_DEMANDVALUE_MET_CONC.DEMAND_VALUE_ERR = xldata[12];
//        array[2] = mt_DEMANDVALUE_MET_CONCService.InsertSQL(mt_DEMANDVALUE_MET_CONC);
//    }
//    return array;
//}

//// Token: 0x060009E3 RID: 2531 RVA: 0x00070D90 File Offset: 0x0006EF90
//public static string GetMT_ESAM_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    MT_ESAM_MET_CONC mt_ESAM_MET_CONC = new MT_ESAM_MET_CONC();
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    mt_ESAM_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_ESAM_MET_CONC.EQUIP_CATEG = "01";
//    mt_ESAM_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_ESAM_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//    mt_ESAM_MET_CONC.DETECT_UNIT_NO = string.Empty;
//    mt_ESAM_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//    mt_ESAM_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//    mt_ESAM_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//    mt_ESAM_MET_CONC.PARA_INDEX = "01";
//    mt_ESAM_MET_CONC.DETECT_ITEM_POINT = "01";
//    mt_ESAM_MET_CONC.IS_VALID = "1";
//    mt_ESAM_MET_CONC.LOAD_VOLTAGE = OraDataHelper.g_DicPCodeTable["meterTestVolt"].GetPCode("100%Un");
//    mt_ESAM_MET_CONC.KEY_NUM = "3";
//    mt_ESAM_MET_CONC.KEY_VER = "04";
//    mt_ESAM_MET_CONC.KEY_STATUS = OraDataHelper.g_DicPCodeTable["secretKeyStatus"].GetPCode("正式密钥");
//    mt_ESAM_MET_CONC.KEY_TYPE = OraDataHelper.g_DicPCodeTable["secretKeyType"].GetPCode("身份认证密钥");
//    mt_ESAM_MET_CONC.CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.密钥更新);
//    mt_ESAM_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//    mt_ESAM_MET_CONC.HANDLE_FLAG = "0";
//    return new MT_ESAM_MET_CONCService().InsertSQL(mt_ESAM_MET_CONC);
//}

//// Token: 0x060009E4 RID: 2532 RVA: 0x00070F14 File Offset: 0x0006F114
//public static string GetMT_ESAM_SECURITY_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    MT_ESAM_SECURITY_MET_CONC mt_ESAM_SECURITY_MET_CONC = new MT_ESAM_SECURITY_MET_CONC();
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    mt_ESAM_SECURITY_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_ESAM_SECURITY_MET_CONC.EQUIP_CATEG = "01";
//    mt_ESAM_SECURITY_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_ESAM_SECURITY_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//    mt_ESAM_SECURITY_MET_CONC.DETECT_UNIT_NO = string.Empty;
//    mt_ESAM_SECURITY_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//    mt_ESAM_SECURITY_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//    mt_ESAM_SECURITY_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//    mt_ESAM_SECURITY_MET_CONC.PARA_INDEX = "01";
//    mt_ESAM_SECURITY_MET_CONC.DETECT_ITEM_POINT = "01";
//    mt_ESAM_SECURITY_MET_CONC.IS_VALID = "1";
//    mt_ESAM_SECURITY_MET_CONC.ESAM_ID = MisDataHelper.GetFkValue(meterInfo, Cus_CostControlItem.身份认证);
//    mt_ESAM_SECURITY_MET_CONC.CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.身份认证);
//    mt_ESAM_SECURITY_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//    mt_ESAM_SECURITY_MET_CONC.HANDLE_FLAG = "0";
//    return new MT_ESAM_SECURITY_MET_CONCService().InsertSQL(mt_ESAM_SECURITY_MET_CONC);
//}

//// Token: 0x060009E5 RID: 2533 RVA: 0x0007102C File Offset: 0x0006F22C
//public static string[] GetMT_CONSIST_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    MT_CONSIST_MET_CONCService mt_CONSIST_MET_CONCService = new MT_CONSIST_MET_CONCService();
//    string key = "1";
//    string[] result;
//    if (!meterInfo.MeterErrAccords.ContainsKey(key))
//    {
//        result = null;
//    }
//    else if (meterInfo.MeterErrAccords[key].lstTestPoint.Count < 0)
//    {
//        result = null;
//    }
//    else
//    {
//        string[] array = new string[meterInfo.MeterErrAccords[key].lstTestPoint.Keys.Count];
//        string[] array2 = new string[meterInfo.MeterErrAccords[key].lstTestPoint.Keys.Count];
//        if (meterInfo.MeterErrAccords.ContainsKey(key))
//        {
//            try
//            {
//                int num = 0;
//                MT_CONSIST_MET_CONC mt_CONSIST_MET_CONC = new MT_CONSIST_MET_CONC();
//                string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//                mt_CONSIST_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//                mt_CONSIST_MET_CONC.EQUIP_CATEG = "01";
//                mt_CONSIST_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//                mt_CONSIST_MET_CONC.DETECT_UNIT_NO = string.Empty;
//                mt_CONSIST_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//                mt_CONSIST_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//                mt_CONSIST_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//                mt_CONSIST_MET_CONC.IS_VALID = "1";
//                foreach (string key2 in meterInfo.MeterErrAccords[key].lstTestPoint.Keys)
//                {
//                    MeterErrAccordBase meterErrAccordBase = meterInfo.MeterErrAccords[key].lstTestPoint[key2];
//                    mt_CONSIST_MET_CONC.SYS_NO = (num + 1).ToString();
//                    mt_CONSIST_MET_CONC.BOTH_WAY_POWER_FLAG = OraDataHelper.g_DicPCodeTable["powerFlag"].GetPCode("正向有功");
//                    string[] array3 = meterErrAccordBase.Mea_WcLimit.Split(new char[]
//                    {
//                                '|'
//                    });
//                    mt_CONSIST_MET_CONC.ERR_DOWN = array3[1];
//                    mt_CONSIST_MET_CONC.ERR_UP = array3[1];
//                    if (GlobalUnit.MisInterFaceType == "厚达MIS接口")
//                    {
//                        mt_CONSIST_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meterTestCurLoad"].GetPCode(meterErrAccordBase.Mea_xib);
//                    }
//                    else
//                    {
//                        mt_CONSIST_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode(meterErrAccordBase.Mea_xib);
//                    }
//                    mt_CONSIST_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode(meterErrAccordBase.Mea_Glys);
//                    mt_CONSIST_MET_CONC.SIMPLING = "2";
//                    mt_CONSIST_MET_CONC.PARA_INDEX = num.ToString();
//                    mt_CONSIST_MET_CONC.DETECT_ITEM_POINT = num.ToString();
//                    mt_CONSIST_MET_CONC.PULES = meterErrAccordBase.Mea_Qs.ToString();
//                    mt_CONSIST_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//                    mt_CONSIST_MET_CONC.HANDLE_FLAG = "0";
//                    mt_CONSIST_MET_CONC.CONC_CODE = ((meterErrAccordBase.Mea_ItemResult == "合格") ? "01" : "02");
//                    mt_CONSIST_MET_CONC.ALL_AVG_ERROR = meterErrAccordBase.Mea_WcAver.Trim();
//                    mt_CONSIST_MET_CONC.ALL_INT_ERROR = meterErrAccordBase.Mea_Wc.Trim();
//                    try
//                    {
//                        string[] array4 = meterErrAccordBase.Mea_Wc1.Split(new char[]
//                        {
//                                    '|'
//                        });
//                        mt_CONSIST_MET_CONC.ALL_ERROR = array4[0] + "|" + array4[1];
//                        mt_CONSIST_MET_CONC.ERROR = meterErrAccordBase.Mea_Wc.Trim();
//                        mt_CONSIST_MET_CONC.AVG_ERROR = array4[2];
//                        if (Convert.ToDecimal(mt_CONSIST_MET_CONC.AVG_ERROR) > 1m)
//                        {
//                            mt_CONSIST_MET_CONC.AVG_ERROR = array4[2].Substring(0, 7);
//                        }
//                        mt_CONSIST_MET_CONC.INT_CONVERT_ERR = array4[3];
//                    }
//                    catch
//                    {
//                        mt_CONSIST_MET_CONC.ERROR = "+0.0000|+0.0000";
//                        mt_CONSIST_MET_CONC.AVG_ERROR = "+0.0000";
//                        mt_CONSIST_MET_CONC.INT_CONVERT_ERR = "+0.00";
//                    }
//                    array2[num++] = mt_CONSIST_MET_CONCService.InsertSQL(mt_CONSIST_MET_CONC);
//                }
//            }
//            catch
//            {
//            }
//        }
//        result = array2;
//    }
//    return result;
//}

//// Token: 0x060009E6 RID: 2534 RVA: 0x000714C8 File Offset: 0x0006F6C8
//public static string[] GetMT_ERROR_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    int num = 0;
//    string key = "2";
//    string[] result;
//    if (!meterInfo.MeterErrAccords.ContainsKey(key))
//    {
//        result = null;
//    }
//    else if (meterInfo.MeterErrAccords[key].lstTestPoint.Count < 0)
//    {
//        result = null;
//    }
//    else
//    {
//        string[] array = new string[meterInfo.MeterErrAccords[key].lstTestPoint.Keys.Count];
//        string[] array2 = new string[meterInfo.MeterErrAccords[key].lstTestPoint.Keys.Count];
//        MT_ERROR_MET_CONCService mt_ERROR_MET_CONCService = new MT_ERROR_MET_CONCService();
//        MT_ERROR_MET_CONC mt_ERROR_MET_CONC = new MT_ERROR_MET_CONC();
//        string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//        mt_ERROR_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_ERROR_MET_CONC.EQUIP_CATEG = "01";
//        mt_ERROR_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//        mt_ERROR_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_ERROR_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_ERROR_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_ERROR_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_ERROR_MET_CONC.IS_VALID = "1";
//        foreach (string key2 in meterInfo.MeterErrAccords[key].lstTestPoint.Keys)
//        {
//            MeterErrAccordBase meterErrAccordBase = meterInfo.MeterErrAccords[key].lstTestPoint[key2];
//            mt_ERROR_MET_CONC.SYS_NO = (num + 1).ToString();
//            mt_ERROR_MET_CONC.BOTH_WAY_POWER_FLAG = OraDataHelper.g_DicPCodeTable["powerFlag"].GetPCode("正向有功");
//            string[] array3 = meterErrAccordBase.Mea_WcLimit.Split(new char[]
//            {
//                        '|'
//            });
//            mt_ERROR_MET_CONC.ERR_DOWN = array3[1];
//            mt_ERROR_MET_CONC.ERR_UP = array3[0];
//            if (GlobalUnit.MisInterFaceType == "厚达MIS接口")
//            {
//                mt_ERROR_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meterTestCurLoad"].GetPCode(meterErrAccordBase.Mea_xib);
//            }
//            else
//            {
//                mt_ERROR_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode(meterErrAccordBase.Mea_xib);
//            }
//            mt_ERROR_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode(meterErrAccordBase.Mea_Glys);
//            mt_ERROR_MET_CONC.SIMPLING = "2";
//            mt_ERROR_MET_CONC.PARA_INDEX = (num + 1).ToString();
//            mt_ERROR_MET_CONC.DETECT_ITEM_POINT = (num + 1).ToString();
//            mt_ERROR_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//            mt_ERROR_MET_CONC.HANDLE_FLAG = "0";
//            try
//            {
//                string[] array4 = meterErrAccordBase.Mea_Wc1.Split(new char[]
//                {
//                            '|'
//                });
//                mt_ERROR_MET_CONC.ONCE_ERR = array4[0] + "|" + array4[1];
//                mt_ERROR_MET_CONC.AVG_ONCE_ERR = array4[2];
//                mt_ERROR_MET_CONC.INT_ONCE_ERR = array4[3];
//                mt_ERROR_MET_CONC.ERROR = meterErrAccordBase.Mea_Wc.Trim();
//            }
//            catch
//            {
//                mt_ERROR_MET_CONC.ONCE_ERR = "+0.0000";
//                mt_ERROR_MET_CONC.AVG_ONCE_ERR = "+0.0000";
//                mt_ERROR_MET_CONC.INT_ONCE_ERR = "+0.0000";
//                mt_ERROR_MET_CONC.ERROR = "+0.0000";
//            }
//            mt_ERROR_MET_CONC.CONC_CODE = ((meterErrAccordBase.Mea_ItemResult == "合格") ? "01" : "02");
//            try
//            {
//                string[] array4 = meterErrAccordBase.Mea_Wc2.Split(new char[]
//                {
//                            '|'
//                });
//                mt_ERROR_MET_CONC.TWICE_ERR = array4[0] + "|" + array4[1];
//                mt_ERROR_MET_CONC.AVG_TWICE_ERR = array4[2];
//                if (Convert.ToDecimal(mt_ERROR_MET_CONC.AVG_TWICE_ERR) > 1m)
//                {
//                    mt_ERROR_MET_CONC.AVG_TWICE_ERR = array4[2].Substring(0, 7);
//                }
//                mt_ERROR_MET_CONC.INT_TWICE_ERR = array4[3];
//                mt_ERROR_MET_CONC.ERROR = meterErrAccordBase.Mea_Wc.Trim();
//            }
//            catch
//            {
//                mt_ERROR_MET_CONC.TWICE_ERR = "+0.0000";
//                mt_ERROR_MET_CONC.AVG_TWICE_ERR = "+0.0000";
//                mt_ERROR_MET_CONC.INT_TWICE_ERR = "+0.0000";
//                mt_ERROR_MET_CONC.ERROR = "+0.0000";
//            }
//            mt_ERROR_MET_CONC.PULES = meterErrAccordBase.Mea_Qs.ToString();
//            mt_ERROR_MET_CONC.SYS_NO = (num + 1).ToString();
//            mt_ERROR_MET_CONC.PARA_INDEX = (num + 1).ToString();
//            mt_ERROR_MET_CONC.DETECT_ITEM_POINT = (num + 1).ToString();
//            mt_ERROR_MET_CONC.CONC_CODE = ((meterErrAccordBase.Mea_ItemResult == "合格") ? "01" : "02");
//            array2[num++] = mt_ERROR_MET_CONCService.InsertSQL(mt_ERROR_MET_CONC);
//        }
//        result = array2;
//    }
//    return result;
//}

//// Token: 0x060009E7 RID: 2535 RVA: 0x00071A34 File Offset: 0x0006FC34
//public static string[] GetMT_VARIATION_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    int num = 0;
//    string key = "3";
//    string[] result;
//    if (!meterInfo.MeterErrAccords.ContainsKey(key))
//    {
//        result = null;
//    }
//    else if (meterInfo.MeterErrAccords[key].lstTestPoint.Count < 0)
//    {
//        result = null;
//    }
//    else
//    {
//        string[] array = new string[meterInfo.MeterErrAccords[key].lstTestPoint.Keys.Count];
//        string[] array2 = new string[meterInfo.MeterErrAccords[key].lstTestPoint.Keys.Count];
//        MT_VARIATION_MET_CONCService mt_VARIATION_MET_CONCService = new MT_VARIATION_MET_CONCService();
//        MT_VARIATION_MET_CONC mt_VARIATION_MET_CONC = new MT_VARIATION_MET_CONC();
//        string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//        mt_VARIATION_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_VARIATION_MET_CONC.EQUIP_CATEG = "01";
//        mt_VARIATION_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//        mt_VARIATION_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_VARIATION_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_VARIATION_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_VARIATION_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_VARIATION_MET_CONC.IS_VALID = "1";
//        mt_VARIATION_MET_CONC.HANDLE_FLAG = "0";
//        foreach (string key2 in meterInfo.MeterErrAccords[key].lstTestPoint.Keys)
//        {
//            MeterErrAccordBase meterErrAccordBase = meterInfo.MeterErrAccords[key].lstTestPoint[key2];
//            mt_VARIATION_MET_CONC.SYS_NO = (num + 1).ToString();
//            mt_VARIATION_MET_CONC.BOTH_WAY_POWER_FLAG = OraDataHelper.g_DicPCodeTable["powerFlag"].GetPCode("正向有功");
//            if (GlobalUnit.MisInterFaceType == "厚达MIS接口")
//            {
//                mt_VARIATION_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meterTestCurLoad"].GetPCode(meterErrAccordBase.Mea_xib);
//            }
//            else
//            {
//                mt_VARIATION_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode(meterErrAccordBase.Mea_xib);
//            }
//            mt_VARIATION_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode(meterErrAccordBase.Mea_Glys);
//            mt_VARIATION_MET_CONC.IABC = OraDataHelper.g_DicPCodeTable["currentPhaseCode"].GetPCode("ABC");
//            mt_VARIATION_MET_CONC.DETECT_CIRCLE = meterErrAccordBase.Mea_Qs.ToString();
//            mt_VARIATION_MET_CONC.SIMPLING = "2";
//            mt_VARIATION_MET_CONC.WAIT_TIME = "30";
//            mt_VARIATION_MET_CONC.PARA_INDEX = (num + 1).ToString();
//            mt_VARIATION_MET_CONC.DETECT_ITEM_POINT = (num + 1).ToString();
//            mt_VARIATION_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//            mt_VARIATION_MET_CONC.HANDLE_FLAG = "0";
//            try
//            {
//                string[] array3 = meterErrAccordBase.Mea_Wc1.Split(new char[]
//                {
//                            '|'
//                });
//                mt_VARIATION_MET_CONC.UP_ERR1 = array3[0];
//                mt_VARIATION_MET_CONC.UP_ERR2 = array3[1];
//                mt_VARIATION_MET_CONC.AVG_UP_ERR = array3[2];
//                mt_VARIATION_MET_CONC.INT_UP_ERR = array3[3];
//                mt_VARIATION_MET_CONC.VARIATION_ERR = meterErrAccordBase.Mea_Wc.Trim();
//                mt_VARIATION_MET_CONC.INT_VARIATION_ERR = meterErrAccordBase.Mea_Wc.Trim();
//            }
//            catch
//            {
//                mt_VARIATION_MET_CONC.UP_ERR1 = "+0.0000";
//                mt_VARIATION_MET_CONC.UP_ERR2 = "+0.0000";
//                mt_VARIATION_MET_CONC.AVG_UP_ERR = "+0.00";
//                mt_VARIATION_MET_CONC.INT_UP_ERR = "+0.0000";
//                mt_VARIATION_MET_CONC.INT_VARIATION_ERR = "+0.00";
//            }
//            mt_VARIATION_MET_CONC.CONC_CODE = ((meterErrAccordBase.Mea_ItemResult == "合格") ? "01" : "02");
//            try
//            {
//                string[] array3 = meterErrAccordBase.Mea_Wc2.Split(new char[]
//                {
//                            '|'
//                });
//                mt_VARIATION_MET_CONC.DOWN_ERR1 = array3[0];
//                mt_VARIATION_MET_CONC.DOWN_ERR2 = array3[1];
//                mt_VARIATION_MET_CONC.AVG_DOWN_ERR = array3[2];
//                mt_VARIATION_MET_CONC.INT_DOWN_ERR = array3[3];
//                mt_VARIATION_MET_CONC.VARIATION_ERR = meterErrAccordBase.Mea_Wc.Trim();
//                mt_VARIATION_MET_CONC.INT_VARIATION_ERR = meterErrAccordBase.Mea_Wc.Trim();
//            }
//            catch
//            {
//                mt_VARIATION_MET_CONC.DOWN_ERR1 = "+0.0000";
//                mt_VARIATION_MET_CONC.DOWN_ERR2 = "+0.0000";
//                mt_VARIATION_MET_CONC.AVG_DOWN_ERR = "+0.00";
//                mt_VARIATION_MET_CONC.INT_DOWN_ERR = "+0.0000";
//                mt_VARIATION_MET_CONC.VARIATION_ERR = "+0.00";
//                mt_VARIATION_MET_CONC.INT_VARIATION_ERR = "+0.00";
//            }
//            mt_VARIATION_MET_CONC.SYS_NO = (num + 1).ToString();
//            mt_VARIATION_MET_CONC.PARA_INDEX = (num + 1).ToString();
//            mt_VARIATION_MET_CONC.DETECT_ITEM_POINT = (num + 1).ToString();
//            mt_VARIATION_MET_CONC.CONC_CODE = ((meterErrAccordBase.Mea_ItemResult == "合格") ? "01" : "02");
//            array2[num++] = mt_VARIATION_MET_CONCService.InsertSQL(mt_VARIATION_MET_CONC);
//        }
//        result = array2;
//    }
//    return result;
//}

//// Token: 0x060009E8 RID: 2536 RVA: 0x00071FBC File Offset: 0x000701BC
//public static string GetMT_OVERLOAD_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    int num = 0;
//    string key = "4";
//    string result;
//    if (!meterInfo.MeterErrAccords.ContainsKey(key))
//    {
//        result = null;
//    }
//    else if (meterInfo.MeterErrAccords[key].lstTestPoint.Count < 0)
//    {
//        result = null;
//    }
//    else
//    {
//        string[] array = new string[meterInfo.MeterErrAccords[key].lstTestPoint.Keys.Count];
//        string text = "";
//        MT_OVERLOAD_MET_CONCService mt_OVERLOAD_MET_CONCService = new MT_OVERLOAD_MET_CONCService();
//        MT_OVERLOAD_MET_CONC mt_OVERLOAD_MET_CONC = new MT_OVERLOAD_MET_CONC();
//        foreach (string key2 in meterInfo.MeterErrAccords[key].lstTestPoint.Keys)
//        {
//            MeterErrAccordBase meterErrAccordBase = meterInfo.MeterErrAccords[key].lstTestPoint[key2];
//            mt_OVERLOAD_MET_CONC.DETECT_TASK_NO = meterInfo.Mb_chrOther5;
//            mt_OVERLOAD_MET_CONC.BAR_CODE = meterInfo.Mb_ChrTxm;
//            mt_OVERLOAD_MET_CONC.SYS_NO = (num + 1).ToString();
//            string[] array2 = meterErrAccordBase.Mea_WcLimit.Split(new char[]
//            {
//                        '|'
//            });
//            mt_OVERLOAD_MET_CONC.ERR_DOWN = array2[1];
//            mt_OVERLOAD_MET_CONC.ERR_UP = array2[0];
//            if (GlobalUnit.MisInterFaceType == "厚达MIS接口")
//            {
//                mt_OVERLOAD_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meterTestCurLoad"].GetPCode("10Ib");
//            }
//            else
//            {
//                mt_OVERLOAD_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode("10Ib");
//            }
//            mt_OVERLOAD_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode(meterErrAccordBase.Mea_Glys);
//            mt_OVERLOAD_MET_CONC.SIMPLING = "2";
//            mt_OVERLOAD_MET_CONC.PARA_INDEX = num.ToString();
//            mt_OVERLOAD_MET_CONC.DETECT_ITEM_POINT = num.ToString();
//            mt_OVERLOAD_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//            mt_OVERLOAD_MET_CONC.HANDLE_FLAG = "0";
//            mt_OVERLOAD_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//            mt_OVERLOAD_MET_CONC.IS_VALID = "1";
//            try
//            {
//                string[] array3 = meterErrAccordBase.Mea_Wc1.Split(new char[]
//                {
//                            '|'
//                });
//                mt_OVERLOAD_MET_CONC.AVG_ERR = array3[0];
//                mt_OVERLOAD_MET_CONC.ERROR = array3[2];
//                mt_OVERLOAD_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//                mt_OVERLOAD_MET_CONC.INT_CONVERTER_ERR = array3[2];
//                mt_OVERLOAD_MET_CONC.PULES = meterErrAccordBase.Mea_Qs.ToString();
//                mt_OVERLOAD_MET_CONC.WAIT_TIME = "30";
//                mt_OVERLOAD_MET_CONC.OVERLOAD_TIME = "60";
//            }
//            catch
//            {
//                mt_OVERLOAD_MET_CONC.AVG_ERR = "+0.0000";
//                mt_OVERLOAD_MET_CONC.ERROR = "+0.0000";
//            }
//            mt_OVERLOAD_MET_CONC.CONC_CODE = ((meterErrAccordBase.Mea_ItemResult == "合格") ? "01" : "02");
//            text = mt_OVERLOAD_MET_CONCService.InsertSQL(mt_OVERLOAD_MET_CONC);
//        }
//        result = text;
//    }
//    return result;
//}

//// Token: 0x060009E9 RID: 2537 RVA: 0x00072328 File Offset: 0x00070528
//public static string GetMT_VOLT_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    MT_VOLT_MET_CONCService mt_VOLT_MET_CONCService = new MT_VOLT_MET_CONCService();
//    return mt_VOLT_MET_CONCService.InsertSQL(new MT_VOLT_MET_CONC
//    {
//        DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO,
//        EQUIP_CATEG = "01",
//        SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO,
//        DETECT_EQUIP_NO = meterInfo._intTaiNo,
//        DETECT_UNIT_NO = string.Empty,
//        POSITION_NO = meterInfo.Mb_intBno.ToString(),
//        BAR_CODE = mtMeter.BAR_CODE,
//        DETECT_DATE = meterInfo.Mb_DatJdrq.ToString(),
//        PARA_INDEX = "01",
//        DETECT_ITEM_POINT = "01",
//        IS_VALID = "1",
//        VOLT_TEST_VALUE = "4000",
//        TEST_TIME = "60",
//        VOLT_OBJ = "02",
//        LEAK_CURRENT_LIMIT = "5",
//        POSITION_LEAK_LIMIT = "5",
//        CONC_CODE = "01",
//        WRITE_DATE = DateTime.Now.ToString(),
//        HANDLE_FLAG = "0"
//    });
//}

//// Token: 0x060009EA RID: 2538 RVA: 0x00072460 File Offset: 0x00070660
//private static string GetMT_CONTROL_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    MT_CONTROL_MET_CONC mt_CONTROL_MET_CONC = new MT_CONTROL_MET_CONC();
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    mt_CONTROL_MET_CONC.EQUIP_ID = mtMeter.METER_ID;
//    if (mt_CONTROL_MET_CONC.EQUIP_ID.Length == 0)
//    {
//        mt_CONTROL_MET_CONC.EQUIP_ID = "1234";
//    }
//    mt_CONTROL_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_CONTROL_MET_CONC.EQUIP_CATEG = "01";
//    mt_CONTROL_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_CONTROL_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//    mt_CONTROL_MET_CONC.DETECT_UNIT_NO = string.Empty;
//    mt_CONTROL_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//    mt_CONTROL_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//    mt_CONTROL_MET_CONC.DETECT_DATE = string.Format("to_date('{0}','yyyy-MM-dd HH24:mi:ss')", meterInfo.Mb_DatJdrq.ToString());
//    mt_CONTROL_MET_CONC.PARA_INDEX = "01";
//    mt_CONTROL_MET_CONC.DETECT_ITEM_POINT = "01";
//    mt_CONTROL_MET_CONC.IS_VALID = "1";
//    mt_CONTROL_MET_CONC.SETTING_WARN_VALUE1 = "";
//    mt_CONTROL_MET_CONC.SETTING_WARN_VALUE2 = "";
//    mt_CONTROL_MET_CONC.REAL_WARN_VALUE1 = "";
//    mt_CONTROL_MET_CONC.REAL_WARN_VALUE2 = "";
//    mt_CONTROL_MET_CONC.CHK_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.控制功能);
//    mt_CONTROL_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//    mt_CONTROL_MET_CONC.HANDLE_FLAG = "0";
//    return new MT_CONTROL_MET_CONCService().InsertSQL(mt_CONTROL_MET_CONC);
//}

//// Token: 0x060009EB RID: 2539 RVA: 0x000725D4 File Offset: 0x000707D4
//private static string[] GetMT_CONTROL_MET_CONCByMt2(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    List<string> list = new List<string>();
//    string chk_CONC_CODE = string.Empty;
//    int num = 1;
//    string text = 22.ToString("D3");
//    string text2 = 21.ToString("D3");
//    string[] result;
//    if (!meterInfo.MeterCostControls.ContainsKey(text) || !meterInfo.MeterCostControls.ContainsKey(text2))
//    {
//        result = null;
//    }
//    else
//    {
//        MT_CONTROL_MET_CONC mt_CONTROL_MET_CONC = new MT_CONTROL_MET_CONC();
//        MT_CONTROL_MET_CONCService mt_CONTROL_MET_CONCService = new MT_CONTROL_MET_CONCService();
//        mt_CONTROL_MET_CONC.EQUIP_ID = mtMeter.METER_ID;
//        if (mt_CONTROL_MET_CONC.EQUIP_ID.Length == 0)
//        {
//            mt_CONTROL_MET_CONC.EQUIP_ID = "1234";
//        }
//        mt_CONTROL_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_CONTROL_MET_CONC.EQUIP_CATEG = "01";
//        mt_CONTROL_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_CONTROL_MET_CONC.DETECT_EQUIP_NO = meterInfo._intTaiNo;
//        mt_CONTROL_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_CONTROL_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_CONTROL_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_CONTROL_MET_CONC.DETECT_DATE = string.Format("to_date('{0}','yyyy-MM-dd HH24:mi:ss')", meterInfo.Mb_DatJdrq.ToString());
//        mt_CONTROL_MET_CONC.PARA_INDEX = "01";
//        mt_CONTROL_MET_CONC.IS_VALID = "1";
//        mt_CONTROL_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_CONTROL_MET_CONC.HANDLE_FLAG = "0";
//        string text3 = string.Empty;
//        string a = string.Empty;
//        foreach (string text4 in meterInfo.MeterCostControls.Keys)
//        {
//            string mfk_chrData = meterInfo.MeterCostControls[text4].Mfk_chrData;
//            if (text4.Length > 3)
//            {
//                a = text4.Substring(0, 3);
//                if (!(a != text) || !(a != text2))
//                {
//                    text3 = text4.Substring(3);
//                    string text5 = text3;
//                    if (text5 != null)
//                    {
//                        if (text5 == "01" || text5 == "02" || text5 == "04" || text5 == "06")
//                        {
//                            chk_CONC_CODE = ((mfk_chrData == "合格") ? "01" : "02");
//                            num++;
//                            mt_CONTROL_MET_CONC.CHK_CONC_CODE = chk_CONC_CODE;
//                            mt_CONTROL_MET_CONC.DETECT_ITEM_POINT = num.ToString("D2");
//                        }
//                    }
//                    list.Add(mt_CONTROL_MET_CONCService.InsertSQL(mt_CONTROL_MET_CONC));
//                }
//            }
//        }
//        int count = list.Count;
//        string[] array = new string[count];
//        for (int i = 0; i < count; i++)
//        {
//            array[i] = list[i];
//        }
//        result = array;
//    }
//    return result;
//}

//// Token: 0x060009EC RID: 2540 RVA: 0x000728F4 File Offset: 0x00070AF4
//private static string[] GetMT_PRESETPARAM_MET_CONCByMtFK(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    List<string> list = new List<string>();
//    string conc_CODE = string.Empty;
//    int num = 1;
//    string key = 22.ToString("D3");
//    string[] result;
//    if (!meterInfo.MeterCostControls.ContainsKey(key))
//    {
//        result = null;
//    }
//    else
//    {
//        MT_PRESETPARAM_MET_CONC mt_PRESETPARAM_MET_CONC = new MT_PRESETPARAM_MET_CONC();
//        MT_PRESETPARAM_MET_CONCService mt_PRESETPARAM_MET_CONCService = new MT_PRESETPARAM_MET_CONCService();
//        mt_PRESETPARAM_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_PRESETPARAM_MET_CONC.EQUIP_CATEG = "01";
//        mt_PRESETPARAM_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_PRESETPARAM_MET_CONC.DETECT_EQUIP_NO = meterInfo._intTaiNo;
//        mt_PRESETPARAM_MET_CONC.DETECT_UNIT_NO = string.Empty;
//        mt_PRESETPARAM_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//        mt_PRESETPARAM_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//        mt_PRESETPARAM_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_PRESETPARAM_MET_CONC.PARA_INDEX = "01";
//        mt_PRESETPARAM_MET_CONC.IS_VALID = "1";
//        mt_PRESETPARAM_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//        mt_PRESETPARAM_MET_CONC.HANDLE_FLAG = "0";
//        string text = string.Empty;
//        foreach (string text2 in meterInfo.MeterCostControls.Keys)
//        {
//            string mfk_chrJL = meterInfo.MeterCostControls[text2].Mfk_chrJL;
//            if (text2.Length > 3)
//            {
//                text = text2.Substring(3);
//                string text3 = text;
//                if (text3 != null)
//                {
//                    if (text3 == "01" || text3 == "02" || text3 == "04" || text3 == "06")
//                    {
//                        conc_CODE = ((mfk_chrJL == "合格") ? "01" : "02");
//                        num++;
//                        mt_PRESETPARAM_MET_CONC.CONC_CODE = conc_CODE;
//                        mt_PRESETPARAM_MET_CONC.DETECT_ITEM_POINT = num.ToString("D2");
//                    }
//                }
//                list.Add(mt_PRESETPARAM_MET_CONCService.InsertSQL(mt_PRESETPARAM_MET_CONC));
//            }
//        }
//        int count = list.Count;
//        string[] array = new string[count];
//        for (int i = 0; i < count; i++)
//        {
//            array[i] = list[i];
//        }
//        result = array;
//    }
//    return result;
//}

//// Token: 0x060009ED RID: 2541 RVA: 0x00072B70 File Offset: 0x00070D70
//private static string[] GetMT_PRESETPARAM_CHECK_MET_CONCByMtFK(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    int num = 0;
//    int num2 = 0;
//    int taiType = 0;
//    List<object> list = new List<object>();
//    string sqlstring = string.Format("select schema_no from vw_c_arrive_para where detect_task_no in\r\n                   (select detect_task_No from mt_detect_out_equip where bar_code = '{0}' and handle_flag='0')", meterInfo.Mb_ChrTxm);
//    DataSet dataSet = OraHelper.QueryFa(sqlstring);
//    DataTable dataTable = dataSet.Tables[0];
//    string text = dataTable.Rows[0]["schema_no"].ToString().Trim();
//    string[] array = text.Split(new char[]
//    {
//                '-'
//    });
//    string text2 = OraDataHelper.g_DicPCodeTable["wiringMode"].GetPName(array[0]).ToString();
//    string text3 = OraDataHelper.g_DicPCodeTable["conMode"].GetPName(array[1]).ToString();
//    string text4 = OraDataHelper.g_DicPCodeTable["meterAccuracy"].GetPName(array[2]).ToString();
//    string text5 = OraDataHelper.g_DicPCodeTable["meterTypeCode"].GetPName(array[4]).ToString();
//    string text6 = OraDataHelper.g_DicPCodeTable["DET_TYPE"].GetPName(array[7]).ToString();
//    string text7 = OraDataHelper.g_DicPCodeTable["equip_fee_rate"].GetPName(array[8]).ToString();
//    if (text6.IndexOf("抽检") != -1)
//    {
//        text6 = "抽检";
//    }
//    else
//    {
//        text6 = "全检";
//    }
//    string text8 = OraDataHelper.g_DicPCodeTable["tripMode"].GetPName(array[9]).ToString();
//    string vFAName = string.Concat(new string[]
//    {
//                text2,
//                text3,
//                text4,
//                text5,
//                text6,
//                text7,
//                text8
//    });
//    Dictionary<string, MeterDLTData> meterDLTDatas = meterInfo.MeterDLTDatas;
//    Model_Plan model_Plan = new Model_Plan(taiType, vFAName);
//    Plan_ConnProtocolCheck plan_ConnProtocolCheck = new Plan_ConnProtocolCheck(taiType, vFAName);
//    foreach (string text9 in meterDLTDatas.Keys)
//    {
//        num2++;
//    }
//    string[] array2 = new string[num2];
//    foreach (string text9 in meterDLTDatas.Keys)
//    {
//        MT_PRESETPARAM_CHECK_MET_CONC mt_PRESETPARAM_CHECK_MET_CONC = new MT_PRESETPARAM_CHECK_MET_CONC();
//        if (meterDLTDatas[text9].AVR_DI_MSG != null)
//        {
//            string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//            mt_PRESETPARAM_CHECK_MET_CONC.DATA_ITEM_NAME = meterDLTDatas[text9].AVR_DI_MSG.ToString();
//            mt_PRESETPARAM_CHECK_MET_CONC.DATA_IDENTION = meterDLTDatas[text9].AVR_DI0_DI3.ToString();
//            mt_PRESETPARAM_CHECK_MET_CONC.CONTROL_CODE = "11";
//            for (int i = 0; i < plan_ConnProtocolCheck.Count; i++)
//            {
//                if (plan_ConnProtocolCheck.GetPlan(i).ItemCode645 == mt_PRESETPARAM_CHECK_MET_CONC.DATA_IDENTION)
//                {
//                    mt_PRESETPARAM_CHECK_MET_CONC.STANDARD_VALUE = plan_ConnProtocolCheck.GetPlan(i).WriteContent.ToString();
//                }
//            }
//            mt_PRESETPARAM_CHECK_MET_CONC.DATA_FORMAT = meterDLTDatas[text9].AVR_DI_FORMAT.ToString();
//            mt_PRESETPARAM_CHECK_MET_CONC.IS_DATA_BLOCK = "否";
//            mt_PRESETPARAM_CHECK_MET_CONC.DETER_UPPER_LIMIT = "";
//            mt_PRESETPARAM_CHECK_MET_CONC.DETER_LOWER_LIMIT = "";
//            mt_PRESETPARAM_CHECK_MET_CONC.CONC_CODE = ((meterDLTDatas[text9].AVR_CONC.Trim() == "合格") ? "01" : "02");
//            mt_PRESETPARAM_CHECK_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//            mt_PRESETPARAM_CHECK_MET_CONC.HANDLE_FLAG = "0";
//            mt_PRESETPARAM_CHECK_MET_CONC.HANDLE_DATE = DateTime.Now.ToString();
//            mt_PRESETPARAM_CHECK_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//            mt_PRESETPARAM_CHECK_MET_CONC.EQUIP_CATEG = "01";
//            mt_PRESETPARAM_CHECK_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//            mt_PRESETPARAM_CHECK_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//            mt_PRESETPARAM_CHECK_MET_CONC.DETECT_UNIT_NO = string.Empty;
//            mt_PRESETPARAM_CHECK_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString();
//            mt_PRESETPARAM_CHECK_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//            mt_PRESETPARAM_CHECK_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//            mt_PRESETPARAM_CHECK_MET_CONC.PARA_INDEX = "1";
//            mt_PRESETPARAM_CHECK_MET_CONC.DETECT_ITEM_POINT = text9.Substring(2, 1).ToString();
//            mt_PRESETPARAM_CHECK_MET_CONC.REAL_VALUE = meterDLTDatas[text9].Mdlt_chrValue.ToString();
//            array2[num++] = new MT_PRESETPARAM_CHECK_MET_CONCService().InsertSQL(mt_PRESETPARAM_CHECK_MET_CONC);
//        }
//    }
//    return array2;
//}

//// Token: 0x060009EE RID: 2542 RVA: 0x000730B4 File Offset: 0x000712B4
//public static string[] GetMT_TIME_ERROR_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string[] array = null;
//    string key = 6.ToString("D3");
//    string conc_CODE = "";
//    int num = 0;
//    string[] array2 = new string[]
//    {
//                "00601",
//                "00602",
//                "00603",
//                "00604"
//    };
//    if (meterInfo.MeterDgns.ContainsKey(key))
//    {
//        conc_CODE = ((meterInfo.MeterDgns[key].Md_chrValue == "合格") ? "01" : "02");
//        num++;
//    }
//    string[] result;
//    if (num == 0)
//    {
//        result = array;
//    }
//    else
//    {
//        array = new string[4];
//        for (int i = 0; i < 4; i++)
//        {
//            MT_TIME_ERROR_MET_CONC mt_TIME_ERROR_MET_CONC = new MT_TIME_ERROR_MET_CONC();
//            string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//            mt_TIME_ERROR_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//            mt_TIME_ERROR_MET_CONC.EQUIP_CATEG = "01";
//            mt_TIME_ERROR_MET_CONC.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//            mt_TIME_ERROR_MET_CONC.DETECT_EQUIP_NO = config.Trim();
//            mt_TIME_ERROR_MET_CONC.DETECT_UNIT_NO = string.Empty;
//            mt_TIME_ERROR_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString().Trim();
//            mt_TIME_ERROR_MET_CONC.BAR_CODE = meterInfo.Mb_ChrTxm;
//            mt_TIME_ERROR_MET_CONC.DETECT_DATE = DateTime.Now.ToString();
//            mt_TIME_ERROR_MET_CONC.BOTH_WAY_POWER_FLAG = OraDataHelper.g_DicPCodeTable["powerFlag"].GetPCode("正向有功");
//            mt_TIME_ERROR_MET_CONC.CONC_CODE = conc_CODE;
//            mt_TIME_ERROR_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//            mt_TIME_ERROR_MET_CONC.HANDLE_FLAG = "0";
//            mt_TIME_ERROR_MET_CONC.PARA_INDEX = "1";
//            mt_TIME_ERROR_MET_CONC.DETECT_ITEM_POINT = "1";
//            if (GlobalUnit.MisInterFaceType == "厚达MIS接口")
//            {
//                mt_TIME_ERROR_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meterTestCurLoad"].GetPCode("1.0Ib");
//            }
//            else
//            {
//                mt_TIME_ERROR_MET_CONC.LOAD_CURRENT = OraDataHelper.g_DicPCodeTable["meter_Test_CurLoad"].GetPCode("1.0Ib");
//            }
//            mt_TIME_ERROR_MET_CONC.VOLTAGE = meterInfo.Mb_chrUb.ToString().Trim();
//            mt_TIME_ERROR_MET_CONC.PF = OraDataHelper.g_DicPCodeTable["meterTestPowerFactor"].GetPCode("1.0");
//            mt_TIME_ERROR_MET_CONC.ERR_UP = "0.01";
//            mt_TIME_ERROR_MET_CONC.ERR_DOWN = "-0.01";
//            mt_TIME_ERROR_MET_CONC.CONTROL_WAY = "电量";
//            mt_TIME_ERROR_MET_CONC.IR_TIME = "1";
//            try
//            {
//                string md_chrValue = meterInfo.MeterDgns[array2[i]].Md_chrValue;
//                string[] array3 = md_chrValue.Split(new char[]
//                {
//                            '|'
//                });
//                if (array3.Length >= 6)
//                {
//                    mt_TIME_ERROR_MET_CONC.VALUE_CONC_CODE = array3[4];
//                    mt_TIME_ERROR_MET_CONC.TOTAL_READING_ERR = array3[4];
//                    mt_TIME_ERROR_MET_CONC.IR_READING = (float.Parse(array3[3]) - float.Parse(array3[2])).ToString("F2");
//                    mt_TIME_ERROR_MET_CONC.FEE_RATIO = array3[5];
//                }
//                MT_TIME_ERROR_MET_CONCService mt_TIME_ERROR_MET_CONCService = new MT_TIME_ERROR_MET_CONCService();
//                array[i] = mt_TIME_ERROR_MET_CONCService.InsertSQL(mt_TIME_ERROR_MET_CONC);
//            }
//            catch
//            {
//            }
//        }
//        result = array;
//    }
//    return result;
//}

//// Token: 0x060009EF RID: 2543 RVA: 0x0007343C File Offset: 0x0007163C
//public static string GetMT_GPS_TIMINGByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    string key = 7.ToString().PadLeft(3, '0');
//    key = 7.ToString("D3");
//    string result;
//    if (meterInfo.MeterDgns.ContainsKey(key))
//    {
//        string conc_CODE = (meterInfo.MeterDgns[key].Md_chrValue == "合格") ? "01" : "02";
//        MT_GPS_TIMING mt_GPS_TIMING = new MT_GPS_TIMING();
//        string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//        mt_GPS_TIMING.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//        mt_GPS_TIMING.EQUIP_CATEG = "01";
//        mt_GPS_TIMING.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//        mt_GPS_TIMING.DETECT_EQUIP_NO = config.ToString().Trim();
//        mt_GPS_TIMING.DETECT_UNIT_NO = string.Empty;
//        mt_GPS_TIMING.POSITION_NO = meterInfo.Mb_intBno.ToString().Trim();
//        mt_GPS_TIMING.BAR_CODE = mtMeter.BAR_CODE;
//        mt_GPS_TIMING.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//        mt_GPS_TIMING.PARA_INDEX = "1";
//        mt_GPS_TIMING.DETECT_ITEM_POINT = "01";
//        mt_GPS_TIMING.CONC_CODE = conc_CODE;
//        mt_GPS_TIMING.WRITE_DATE = DateTime.Now.ToString();
//        mt_GPS_TIMING.HANDLE_FLAG = "0";
//        result = new MT_GPS_TIMINGService().InsertSQL(mt_GPS_TIMING);
//    }
//    else
//    {
//        result = "";
//    }
//    return result;
//}

//// Token: 0x060009F0 RID: 2544 RVA: 0x000735BC File Offset: 0x000717BC
//public static string[] GetMT_INFLUENCE_QTY_MET_CONCByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    List<string> list = new List<string>();
//    List<MeterSpecialErr> list2 = new List<MeterSpecialErr>();
//    string[] array = null;
//    string[] result;
//    if (meterInfo.MeterSpecialErrs.Count == 0)
//    {
//        result = array;
//    }
//    else
//    {
//        foreach (string text in meterInfo.MeterSpecialErrs.Keys)
//        {
//            list.Add(text);
//            list2.Add(meterInfo.MeterSpecialErrs[text]);
//        }
//        array = new string[list2.Count];
//        for (int i = 0; i < list2.Count; i++)
//        {
//            MT_INFLUENCE_QTY_MET_CONC mt_INFLUENCE_QTY_MET_CONC = new MT_INFLUENCE_QTY_MET_CONC();
//            mt_INFLUENCE_QTY_MET_CONC.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//            mt_INFLUENCE_QTY_MET_CONC.EQUIP_CATEG = "01";
//            if (meterInfo.Mb_intClfs == 5)
//            {
//                mt_INFLUENCE_QTY_MET_CONC.SYS_NO = "401";
//            }
//            else
//            {
//                mt_INFLUENCE_QTY_MET_CONC.SYS_NO = "402";
//            }
//            mt_INFLUENCE_QTY_MET_CONC.DETECT_EQUIP_NO = meterInfo._intTaiNo.ToString().Trim();
//            mt_INFLUENCE_QTY_MET_CONC.DETECT_UNIT_NO = string.Empty;
//            mt_INFLUENCE_QTY_MET_CONC.POSITION_NO = meterInfo.Mb_intBno.ToString().Trim();
//            mt_INFLUENCE_QTY_MET_CONC.BAR_CODE = mtMeter.BAR_CODE;
//            mt_INFLUENCE_QTY_MET_CONC.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//            mt_INFLUENCE_QTY_MET_CONC.PARA_INDEX = (i + 1).ToString();
//            mt_INFLUENCE_QTY_MET_CONC.DETECT_ITEM_POINT = (i + 1).ToString();
//            mt_INFLUENCE_QTY_MET_CONC.EFFECT_TEST_ITEM = list2[i].Mse_PrjName.Trim();
//            mt_INFLUENCE_QTY_MET_CONC.CHK_CONC_CODE = ((list2[i].Mse_Result.Trim() == "合格") ? "01" : "02");
//            mt_INFLUENCE_QTY_MET_CONC.WRITE_DATE = DateTime.Now.ToString();
//            mt_INFLUENCE_QTY_MET_CONC.HANDLE_FLAG = "0";
//            mt_INFLUENCE_QTY_MET_CONC.IS_VALID = "1";
//            array[i] = new MT_INFLUENCE_QTY_MET_CONCService().InsertSQL(mt_INFLUENCE_QTY_MET_CONC);
//        }
//        result = array;
//    }
//    return result;
//}

//// Token: 0x060009F1 RID: 2545 RVA: 0x00073810 File Offset: 0x00071A10
//public static string GetMT_DETECT_RSLTByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    MT_DETECT_RSLT mt_DETECT_RSLT = new MT_DETECT_RSLT();
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    mt_DETECT_RSLT.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_DETECT_RSLT.EQUIP_CATEG = "01";
//    mt_DETECT_RSLT.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_DETECT_RSLT.DETECT_EQUIP_NO = config.Trim();
//    mt_DETECT_RSLT.DETECT_UNIT_NO = string.Empty;
//    mt_DETECT_RSLT.POSITION_NO = meterInfo.Mb_intBno.ToString();
//    mt_DETECT_RSLT.BAR_CODE = mtMeter.BAR_CODE;
//    mt_DETECT_RSLT.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//    if (!meterInfo.YaoJianYn || !meterInfo.YaoJianYnSave)
//    {
//        if (mtMeter.BAR_CODE == "")
//        {
//            mt_DETECT_RSLT.CONC_CODE = string.Empty;
//        }
//        else
//        {
//            mt_DETECT_RSLT.CONC_CODE = "02";
//            mt_DETECT_RSLT.ELE_CHK_CONC_CODE = "02";
//            mt_DETECT_RSLT.FAULT_TYPE = "00120";
//            mt_DETECT_RSLT.INTUIT_CONC_CODE = "02";
//            mt_DETECT_RSLT.UNPASS_REASON = "00120";
//        }
//    }
//    else
//    {
//        mt_DETECT_RSLT.CONC_CODE = ((meterInfo.Mb_chrResult.Trim() == "合格") ? "01" : "02");
//        mt_DETECT_RSLT.INTUIT_CONC_CODE = MisDataHelper.GetInsulationConclusionFromHouda(meterInfo);
//        if (mt_DETECT_RSLT.INTUIT_CONC_CODE == "03" || mt_DETECT_RSLT.INTUIT_CONC_CODE == "02")
//        {
//            mt_DETECT_RSLT.CONC_CODE = "02";
//            mt_DETECT_RSLT.FAULT_TYPE = "001";
//            mt_DETECT_RSLT.UNPASS_REASON = "001";
//        }
//    }
//    mt_DETECT_RSLT.VOLT_CONC_CODE = MisDataHelper.GetVoltConcFromHouda(meterInfo);
//    if (mt_DETECT_RSLT.VOLT_CONC_CODE == "03" || mt_DETECT_RSLT.VOLT_CONC_CODE == "02")
//    {
//        mt_DETECT_RSLT.CONC_CODE = "02";
//        mt_DETECT_RSLT.FAULT_TYPE = "027";
//        mt_DETECT_RSLT.UNPASS_REASON = "027";
//    }
//    mt_DETECT_RSLT.BASICERR_CONC_CODE = MisDataHelper.GetBasicConclusion(meterInfo, Cus_MeterResultPrjID.基本误差试验);
//    if (mt_DETECT_RSLT.BASICERR_CONC_CODE == "03")
//    {
//        mt_DETECT_RSLT.CONC_CODE = "02";
//        mt_DETECT_RSLT.ELE_CHK_CONC_CODE = "02";
//        mt_DETECT_RSLT.FAULT_TYPE = "00120";
//        mt_DETECT_RSLT.UNPASS_REASON = "00120";
//    }
//    mt_DETECT_RSLT.CONST_CONC_CODE = MisDataHelper.GetBasicConclusion(meterInfo, Cus_MeterResultPrjID.走字试验);
//    mt_DETECT_RSLT.STARTING_CONC_CODE = "01";
//    mt_DETECT_RSLT.CREEPING_CONC_CODE = MisDataHelper.GetQiQianDongConclusion(meterInfo, Cus_MeterResultPrjID.潜动试验);
//    mt_DETECT_RSLT.DAYERR_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.日计时误差);
//    mt_DETECT_RSLT.ERROR_CONC_CODE = MisDataHelper.GetErrorRecordConclusion(meterInfo, Cus_ErrorAccordItem.误差变差);
//    mt_DETECT_RSLT.CONSIST_CONC_CODE = MisDataHelper.GetErrorRecordConclusion(meterInfo, Cus_ErrorAccordItem.误差一致性);
//    mt_DETECT_RSLT.PASSWORD_CHANGE_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.电量清零);
//    mt_DETECT_RSLT.VARIATION_CONC_CODE = MisDataHelper.GetErrorRecordConclusion(meterInfo, Cus_ErrorAccordItem.负载电流升降变差);
//    mt_DETECT_RSLT.OVERLOAD_CONC_CODE = MisDataHelper.GetErrorRecordConclusion(meterInfo, Cus_ErrorAccordItem.电流过载);
//    mt_DETECT_RSLT.TS_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.时段投切);
//    mt_DETECT_RSLT.RUNING_CONC_CODE = "";
//    mt_DETECT_RSLT.PERIOD_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.时段投切);
//    mt_DETECT_RSLT.VALUE_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.最大需量Imax);
//    if (mt_DETECT_RSLT.VALUE_CONC_CODE == "")
//    {
//        mt_DETECT_RSLT.VALUE_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.最大需量10Ib);
//    }
//    mt_DETECT_RSLT.KEY_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.密钥更新);
//    mt_DETECT_RSLT.METER_ERROR_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.计度器示值组合误差);
//    mt_DETECT_RSLT.ESAM_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.身份认证);
//    mt_DETECT_RSLT.SWITCH_ON_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.远程控制);
//    mt_DETECT_RSLT.SWITCH_OUT_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.远程控制);
//    mt_DETECT_RSLT.EH_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.远程保电);
//    mt_DETECT_RSLT.EC_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.保电解除);
//    mt_DETECT_RSLT.WARN_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.报警功能);
//    mt_DETECT_RSLT.WARN_CANCEL_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.报警功能);
//    mt_DETECT_RSLT.REMOTE_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.数据回抄);
//    mt_DETECT_RSLT.SURPLUS_CONC_CODE = string.Empty;
//    mt_DETECT_RSLT.GPS_CONC = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.GPS对时);
//    if (mt_DETECT_RSLT.GPS_CONC == "03")
//    {
//        mt_DETECT_RSLT.GPS_CONC = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.GPS对时);
//    }
//    mt_DETECT_RSLT.RESET_EQ_MET_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.钱包初始化);
//    if (mt_DETECT_RSLT.RESET_EQ_MET_CONC_CODE == "03")
//    {
//        mt_DETECT_RSLT.RESET_EQ_MET_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.电量清零);
//    }
//    mt_DETECT_RSLT.RESET_DEMAND_MET_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.钱包初始化);
//    if (mt_DETECT_RSLT.RESET_DEMAND_MET_CONC_CODE == "03")
//    {
//        mt_DETECT_RSLT.RESET_DEMAND_MET_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.电量清零);
//    }
//    mt_DETECT_RSLT.TIMING_MET_CONC_CODE = string.Empty;
//    mt_DETECT_RSLT.COMMINICATE_MET_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.GPS对时);
//    mt_DETECT_RSLT.ADDRESS_MET_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.通信测试);
//    mt_DETECT_RSLT.MULTI_INTERFACE_MET_CONC_CODE = "";
//    mt_DETECT_RSLT.LEAP_YEAR_MET_CONC = string.Empty;
//    mt_DETECT_RSLT.PARA_READ_MET_CONC_CODE = string.Empty;
//    mt_DETECT_RSLT.SETTING_CONC_CODE = string.Empty;
//    mt_DETECT_RSLT.DEVIATION_MET_CONC = MisDataHelper.GetBasicConclusion(meterInfo, Cus_MeterResultPrjID.标准偏差);
//    mt_DETECT_RSLT.DETECT_PERSON = meterInfo.Mb_ChrJyy.Trim();
//    mt_DETECT_RSLT.AUDIT_PERSON = meterInfo.Mb_ChrHyy.Trim();
//    mt_DETECT_RSLT.WRITE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
//    mt_DETECT_RSLT.HANDLE_FLAG = "0";
//    mt_DETECT_RSLT.TEMP = meterInfo.Mb_chrWd;
//    mt_DETECT_RSLT.HUMIDITY = meterInfo.Mb_chrSd;
//    mt_DETECT_RSLT.PRESET_PARAMET_CHECK_CONC_CODE = MisDataHelper.GetPRESETPARAMETCHECKConclusion(meterInfo);
//    mt_DETECT_RSLT.ELE_ENERGY_FUNC_CONC_CODE = MisDataHelper.GetFunctionConclusion(meterInfo, Cus_FunctionItem.计量功能);
//    mt_DETECT_RSLT.RATE_TIME_FUNC_CONC_CODE = MisDataHelper.GetFunctionConclusion(meterInfo, Cus_FunctionItem.费率时段功能);
//    mt_DETECT_RSLT.INFLUENCE_QTY_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.报警功能);
//    if (mt_DETECT_RSLT.CONC_CODE == "02")
//    {
//        if (mt_DETECT_RSLT.BASICERR_CONC_CODE == "02")
//        {
//            string strName = "电流变化引起的百分误差（基本误差）";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.CONST_CONC_CODE == "02")
//        {
//            string strName = "电能表常数试验";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.STARTING_CONC_CODE == "02")
//        {
//            string strName = "起动试验";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.CREEPING_CONC_CODE == "02")
//        {
//            string strName = "潜动试验";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.DAYERR_CONC_CODE == "02")
//        {
//            string strName = "日计时误差";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.GPS_CONC == "02")
//        {
//            string strName = "校时失败";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.VALUE_CONC_CODE == "02")
//        {
//            string strName = "需量示值误差";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.PRESET_PARAMET_CHECK_CONC_CODE == "02")
//        {
//            string strName = "预置参数错误";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.ESAM_CONC_CODE == "02")
//        {
//            string strName = "安全认证试验";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.KEY_CONC_CODE == "02")
//        {
//            string strName = "密钥更新试验";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.SWITCH_ON_CONC_CODE == "02" || mt_DETECT_RSLT.SWITCH_OUT_CONC_CODE == "02")
//        {
//            string strName = "远程控制试验";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.WARN_CANCEL_CONC_CODE == "02" || mt_DETECT_RSLT.WARN_CONC_CODE == "02")
//        {
//            string strName = "报警功能";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.EH_CONC_CODE == "02" || mt_DETECT_RSLT.EC_CONC_CODE == "02")
//        {
//            string strName = "费控功能";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.RESET_EQ_MET_CONC_CODE == "02" || mt_DETECT_RSLT.RESET_DEMAND_MET_CONC_CODE == "02" || mt_DETECT_RSLT.REMOTE_CONC_CODE == "02")
//        {
//            string strName = "费控功能";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.ERROR_CONC_CODE == "02")
//        {
//            string strName = "误差变差试验";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.CONSIST_CONC_CODE == "02")
//        {
//            string strName = "误差一致性试验";
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.VARIATION_CONC_CODE == "02")
//        {
//            string strName = "负载电流升降变差试验";
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.METER_ERROR_CONC_CODE == "02")
//        {
//            string strName = "计度器总电能示值误差";
//            mt_DETECT_RSLT.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//            mt_DETECT_RSLT.FAULT_TYPE = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//    }
//    mt_DETECT_RSLT.HARD_VERSION = "01";
//    return new MT_DETECT_RSLTService().InsertSQL(mt_DETECT_RSLT);
//}

//// Token: 0x060009F2 RID: 2546 RVA: 0x000744DC File Offset: 0x000726DC
//public static string GetMT_DETECT_RSLTTMPByMt(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    MT_DETECT_RSLT mt_DETECT_RSLT = new MT_DETECT_RSLT();
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    mt_DETECT_RSLT.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_DETECT_RSLT.EQUIP_CATEG = "01";
//    mt_DETECT_RSLT.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_DETECT_RSLT.DETECT_EQUIP_NO = config.Trim();
//    mt_DETECT_RSLT.DETECT_UNIT_NO = string.Empty;
//    mt_DETECT_RSLT.POSITION_NO = meterInfo.Mb_intBno.ToString();
//    mt_DETECT_RSLT.BAR_CODE = mtMeter.BAR_CODE;
//    mt_DETECT_RSLT.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//    mt_DETECT_RSLT.CONC_CODE = "02";
//    mt_DETECT_RSLT.FAULT_TYPE = "001";
//    mt_DETECT_RSLT.VOLT_CONC_CODE = "03";
//    mt_DETECT_RSLT.INTUIT_CONC_CODE = "02";
//    mt_DETECT_RSLT.BASICERR_CONC_CODE = "03";
//    mt_DETECT_RSLT.CONST_CONC_CODE = "03";
//    mt_DETECT_RSLT.STARTING_CONC_CODE = "03";
//    mt_DETECT_RSLT.CREEPING_CONC_CODE = "03";
//    mt_DETECT_RSLT.DAYERR_CONC_CODE = "03";
//    mt_DETECT_RSLT.ERROR_CONC_CODE = "03";
//    mt_DETECT_RSLT.CONSIST_CONC_CODE = "03";
//    mt_DETECT_RSLT.PASSWORD_CHANGE_CONC_CODE = "03";
//    mt_DETECT_RSLT.VARIATION_CONC_CODE = "03";
//    mt_DETECT_RSLT.OVERLOAD_CONC_CODE = "03";
//    mt_DETECT_RSLT.TS_CONC_CODE = "03";
//    mt_DETECT_RSLT.RUNING_CONC_CODE = "03";
//    mt_DETECT_RSLT.PERIOD_CONC_CODE = "03";
//    mt_DETECT_RSLT.VALUE_CONC_CODE = "03";
//    mt_DETECT_RSLT.KEY_CONC_CODE = "03";
//    mt_DETECT_RSLT.METER_ERROR_CONC_CODE = "03";
//    mt_DETECT_RSLT.ESAM_CONC_CODE = "03";
//    mt_DETECT_RSLT.SWITCH_ON_CONC_CODE = "03";
//    mt_DETECT_RSLT.SWITCH_OUT_CONC_CODE = "03";
//    mt_DETECT_RSLT.EH_CONC_CODE = "03";
//    mt_DETECT_RSLT.EC_CONC_CODE = "03";
//    mt_DETECT_RSLT.WARN_CONC_CODE = "03";
//    mt_DETECT_RSLT.WARN_CANCEL_CONC_CODE = "03";
//    mt_DETECT_RSLT.REMOTE_CONC_CODE = "03";
//    mt_DETECT_RSLT.SURPLUS_CONC_CODE = string.Empty;
//    mt_DETECT_RSLT.GPS_CONC = "03";
//    mt_DETECT_RSLT.RESET_EQ_MET_CONC_CODE = "03";
//    mt_DETECT_RSLT.RESET_DEMAND_MET_CONC_CODE = "03";
//    mt_DETECT_RSLT.TIMING_MET_CONC_CODE = string.Empty;
//    mt_DETECT_RSLT.COMMINICATE_MET_CONC_CODE = "03";
//    mt_DETECT_RSLT.ADDRESS_MET_CONC_CODE = "03";
//    mt_DETECT_RSLT.MULTI_INTERFACE_MET_CONC_CODE = "";
//    mt_DETECT_RSLT.LEAP_YEAR_MET_CONC = string.Empty;
//    mt_DETECT_RSLT.PARA_READ_MET_CONC_CODE = string.Empty;
//    mt_DETECT_RSLT.SETTING_CONC_CODE = string.Empty;
//    mt_DETECT_RSLT.DEVIATION_MET_CONC = "03";
//    mt_DETECT_RSLT.DETECT_PERSON = meterInfo.Mb_ChrJyy.Trim();
//    mt_DETECT_RSLT.AUDIT_PERSON = meterInfo.Mb_ChrHyy.Trim();
//    mt_DETECT_RSLT.WRITE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
//    mt_DETECT_RSLT.HANDLE_FLAG = "0";
//    mt_DETECT_RSLT.TEMP = meterInfo.Mb_chrWd;
//    mt_DETECT_RSLT.HUMIDITY = meterInfo.Mb_chrSd;
//    mt_DETECT_RSLT.PRESET_PARAMET_CHECK_CONC_CODE = "03";
//    mt_DETECT_RSLT.ELE_ENERGY_FUNC_CONC_CODE = "03";
//    mt_DETECT_RSLT.RATE_TIME_FUNC_CONC_CODE = "03";
//    mt_DETECT_RSLT.INFLUENCE_QTY_CONC_CODE = "03";
//    mt_DETECT_RSLT.HARD_VERSION = "01";
//    return new MT_DETECT_RSLTService().InsertSQL(mt_DETECT_RSLT);
//}

//// Token: 0x060009F3 RID: 2547 RVA: 0x00074814 File Offset: 0x00072A14
//public static string GetMT_EQUIP_UNPASS_REASONTMP(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    List<string> list = new List<string>();
//    MT_EQUIP_UNPASS_REASON mt_EQUIP_UNPASS_REASON = new MT_EQUIP_UNPASS_REASON();
//    mt_EQUIP_UNPASS_REASON.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_EQUIP_UNPASS_REASON.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_EQUIP_UNPASS_REASON.UNPASS_REASON = "00120";
//    mt_EQUIP_UNPASS_REASON.EQUIP_CATEG = "01";
//    mt_EQUIP_UNPASS_REASON.BAR_CODE = mtMeter.BAR_CODE;
//    mt_EQUIP_UNPASS_REASON.WRITE_DATE = DateTime.Now.ToString();
//    mt_EQUIP_UNPASS_REASON.HANDLE_FLAG = "0";
//    return new MT_EQUIP_UNPASS_REASONService().InsertSQL(mt_EQUIP_UNPASS_REASON);
//}

//// Token: 0x060009F4 RID: 2548 RVA: 0x000748B0 File Offset: 0x00072AB0
//public static string GetMT_EQUIP_UNPASS_REASON(MT_METER mtMeter, MeterBasicInfo meterInfo)
//{
//    List<string> list = new List<string>();
//    MT_EQUIP_UNPASS_REASON mt_EQUIP_UNPASS_REASON = new MT_EQUIP_UNPASS_REASON();
//    mt_EQUIP_UNPASS_REASON.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_EQUIP_UNPASS_REASON.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_EQUIP_UNPASS_REASON.EQUIP_CATEG = "01";
//    mt_EQUIP_UNPASS_REASON.BAR_CODE = mtMeter.BAR_CODE;
//    MT_DETECT_RSLT mt_DETECT_RSLT = new MT_DETECT_RSLT();
//    string config = GlobalUnit.GetConfig("BEIJINGCLIENTCODE", "");
//    mt_DETECT_RSLT.DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
//    mt_DETECT_RSLT.EQUIP_CATEG = "01";
//    mt_DETECT_RSLT.SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO;
//    mt_DETECT_RSLT.DETECT_EQUIP_NO = config.Trim();
//    mt_DETECT_RSLT.DETECT_UNIT_NO = string.Empty;
//    mt_DETECT_RSLT.POSITION_NO = meterInfo.Mb_intBno.ToString();
//    mt_DETECT_RSLT.BAR_CODE = mtMeter.BAR_CODE;
//    mt_DETECT_RSLT.DETECT_DATE = meterInfo.Mb_DatJdrq.ToString();
//    if (!meterInfo.YaoJianYn)
//    {
//        if (mtMeter.BAR_CODE == "")
//        {
//            mt_DETECT_RSLT.CONC_CODE = string.Empty;
//        }
//        else
//        {
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = "00120";
//        }
//    }
//    else
//    {
//        mt_DETECT_RSLT.CONC_CODE = ((meterInfo.Mb_chrResult.Trim() == "合格") ? "01" : "02");
//        mt_DETECT_RSLT.INTUIT_CONC_CODE = MisDataHelper.GetInsulationConclusionFromHouda(meterInfo);
//        if (mt_DETECT_RSLT.INTUIT_CONC_CODE == "03" || mt_DETECT_RSLT.INTUIT_CONC_CODE == "02")
//        {
//            mt_DETECT_RSLT.CONC_CODE = "02";
//            mt_DETECT_RSLT.FAULT_TYPE = "001";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = "001";
//        }
//    }
//    mt_DETECT_RSLT.VOLT_CONC_CODE = MisDataHelper.GetVoltConcFromHouda(meterInfo);
//    if (mt_DETECT_RSLT.VOLT_CONC_CODE == "03" || mt_DETECT_RSLT.VOLT_CONC_CODE == "02")
//    {
//        mt_DETECT_RSLT.CONC_CODE = "02";
//        mt_DETECT_RSLT.FAULT_TYPE = "027";
//        mt_EQUIP_UNPASS_REASON.UNPASS_REASON = "027";
//    }
//    mt_DETECT_RSLT.BASICERR_CONC_CODE = MisDataHelper.GetBasicConclusion(meterInfo, Cus_MeterResultPrjID.基本误差试验);
//    if (mt_DETECT_RSLT.BASICERR_CONC_CODE == "03")
//    {
//        mt_DETECT_RSLT.CONC_CODE = "02";
//        mt_DETECT_RSLT.ELE_CHK_CONC_CODE = "02";
//        mt_DETECT_RSLT.FAULT_TYPE = "00120";
//        mt_EQUIP_UNPASS_REASON.UNPASS_REASON = "00120";
//    }
//    mt_DETECT_RSLT.CONST_CONC_CODE = MisDataHelper.GetBasicConclusion(meterInfo, Cus_MeterResultPrjID.走字试验);
//    mt_DETECT_RSLT.STARTING_CONC_CODE = "01";
//    mt_DETECT_RSLT.CREEPING_CONC_CODE = MisDataHelper.GetQiQianDongConclusion(meterInfo, Cus_MeterResultPrjID.潜动试验);
//    mt_DETECT_RSLT.DAYERR_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.日计时误差);
//    mt_DETECT_RSLT.ERROR_CONC_CODE = MisDataHelper.GetErrorRecordConclusion(meterInfo, Cus_ErrorAccordItem.误差变差);
//    mt_DETECT_RSLT.CONSIST_CONC_CODE = MisDataHelper.GetErrorRecordConclusion(meterInfo, Cus_ErrorAccordItem.误差一致性);
//    mt_DETECT_RSLT.PASSWORD_CHANGE_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.电量清零);
//    mt_DETECT_RSLT.VARIATION_CONC_CODE = MisDataHelper.GetErrorRecordConclusion(meterInfo, Cus_ErrorAccordItem.负载电流升降变差);
//    mt_DETECT_RSLT.OVERLOAD_CONC_CODE = MisDataHelper.GetErrorRecordConclusion(meterInfo, Cus_ErrorAccordItem.电流过载);
//    mt_DETECT_RSLT.TS_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.时段投切);
//    mt_DETECT_RSLT.RUNING_CONC_CODE = "";
//    mt_DETECT_RSLT.PERIOD_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.时段投切);
//    mt_DETECT_RSLT.VALUE_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.最大需量Imax);
//    if (mt_DETECT_RSLT.VALUE_CONC_CODE == "")
//    {
//        mt_DETECT_RSLT.VALUE_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.最大需量10Ib);
//    }
//    mt_DETECT_RSLT.KEY_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.密钥更新);
//    mt_DETECT_RSLT.METER_ERROR_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.计度器示值组合误差);
//    mt_DETECT_RSLT.ESAM_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.身份认证);
//    mt_DETECT_RSLT.SWITCH_ON_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.远程控制);
//    mt_DETECT_RSLT.SWITCH_OUT_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.远程控制);
//    mt_DETECT_RSLT.EH_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.远程保电);
//    mt_DETECT_RSLT.EC_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.保电解除);
//    mt_DETECT_RSLT.WARN_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.报警功能);
//    mt_DETECT_RSLT.WARN_CANCEL_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.报警功能);
//    mt_DETECT_RSLT.REMOTE_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.数据回抄);
//    mt_DETECT_RSLT.SURPLUS_CONC_CODE = string.Empty;
//    mt_DETECT_RSLT.GPS_CONC = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.GPS对时);
//    if (mt_DETECT_RSLT.GPS_CONC == "03")
//    {
//        mt_DETECT_RSLT.GPS_CONC = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.GPS对时);
//    }
//    mt_DETECT_RSLT.RESET_EQ_MET_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.钱包初始化);
//    if (mt_DETECT_RSLT.RESET_EQ_MET_CONC_CODE == "03")
//    {
//        mt_DETECT_RSLT.RESET_EQ_MET_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.电量清零);
//    }
//    mt_DETECT_RSLT.RESET_DEMAND_MET_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.钱包初始化);
//    if (mt_DETECT_RSLT.RESET_DEMAND_MET_CONC_CODE == "03")
//    {
//        mt_DETECT_RSLT.RESET_DEMAND_MET_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.电量清零);
//    }
//    mt_DETECT_RSLT.TIMING_MET_CONC_CODE = string.Empty;
//    mt_DETECT_RSLT.COMMINICATE_MET_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.GPS对时);
//    mt_DETECT_RSLT.ADDRESS_MET_CONC_CODE = MisDataHelper.GetDgnConclusion(meterInfo, Cus_DgnItem.通信测试);
//    mt_DETECT_RSLT.MULTI_INTERFACE_MET_CONC_CODE = "";
//    mt_DETECT_RSLT.LEAP_YEAR_MET_CONC = string.Empty;
//    mt_DETECT_RSLT.PARA_READ_MET_CONC_CODE = string.Empty;
//    mt_DETECT_RSLT.SETTING_CONC_CODE = string.Empty;
//    mt_DETECT_RSLT.DEVIATION_MET_CONC = MisDataHelper.GetBasicConclusion(meterInfo, Cus_MeterResultPrjID.标准偏差);
//    mt_DETECT_RSLT.DETECT_PERSON = meterInfo.Mb_ChrJyy.Trim();
//    mt_DETECT_RSLT.AUDIT_PERSON = meterInfo.Mb_ChrHyy.Trim();
//    mt_DETECT_RSLT.WRITE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
//    mt_DETECT_RSLT.HANDLE_FLAG = "0";
//    mt_DETECT_RSLT.TEMP = meterInfo.Mb_chrWd;
//    mt_DETECT_RSLT.HUMIDITY = meterInfo.Mb_chrSd;
//    mt_DETECT_RSLT.PRESET_PARAMET_CHECK_CONC_CODE = MisDataHelper.GetPRESETPARAMETCHECKConclusion(meterInfo);
//    mt_DETECT_RSLT.ELE_ENERGY_FUNC_CONC_CODE = MisDataHelper.GetFunctionConclusion(meterInfo, Cus_FunctionItem.计量功能);
//    mt_DETECT_RSLT.RATE_TIME_FUNC_CONC_CODE = MisDataHelper.GetFunctionConclusion(meterInfo, Cus_FunctionItem.费率时段功能);
//    mt_DETECT_RSLT.INFLUENCE_QTY_CONC_CODE = MisDataHelper.GetFkConclusion(meterInfo, Cus_CostControlItem.报警功能);
//    if (mt_DETECT_RSLT.CONC_CODE == "02")
//    {
//        if (mt_DETECT_RSLT.BASICERR_CONC_CODE == "02")
//        {
//            string strName = "电流变化引起的百分误差（基本误差）";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.CONST_CONC_CODE == "02")
//        {
//            string strName = "电能表常数试验";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.STARTING_CONC_CODE == "02")
//        {
//            string strName = "起动试验";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.CREEPING_CONC_CODE == "02")
//        {
//            string strName = "潜动试验";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.DAYERR_CONC_CODE == "02")
//        {
//            string strName = "日计时误差";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.GPS_CONC == "02")
//        {
//            string strName = "校时失败";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.VALUE_CONC_CODE == "02")
//        {
//            string strName = "需量示值误差";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.PRESET_PARAMET_CHECK_CONC_CODE == "02")
//        {
//            string strName = "预置参数错误";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.ESAM_CONC_CODE == "02")
//        {
//            string strName = "安全认证试验";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.KEY_CONC_CODE == "02")
//        {
//            string strName = "密钥更新试验";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.SWITCH_ON_CONC_CODE == "02" || mt_DETECT_RSLT.SWITCH_OUT_CONC_CODE == "02")
//        {
//            string strName = "远程控制试验";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.WARN_CANCEL_CONC_CODE == "02" || mt_DETECT_RSLT.WARN_CONC_CODE == "02")
//        {
//            string strName = "报警功能";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.EH_CONC_CODE == "02" || mt_DETECT_RSLT.EC_CONC_CODE == "02")
//        {
//            string strName = "费控功能";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.RESET_EQ_MET_CONC_CODE == "02" || mt_DETECT_RSLT.RESET_DEMAND_MET_CONC_CODE == "02" || mt_DETECT_RSLT.REMOTE_CONC_CODE == "02")
//        {
//            string strName = "费控功能";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.ERROR_CONC_CODE == "02")
//        {
//            string strName = "误差变差试验";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.CONSIST_CONC_CODE == "02")
//        {
//            string strName = "误差一致性试验";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.VARIATION_CONC_CODE == "02")
//        {
//            string strName = "负载电流升降变差试验";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//        else if (mt_DETECT_RSLT.METER_ERROR_CONC_CODE == "02")
//        {
//            string strName = "计度器总电能示值误差";
//            mt_EQUIP_UNPASS_REASON.UNPASS_REASON = OraDataHelper.g_DicPCodeTable["metDetectUnqualified"].GetPCode(strName);
//        }
//    }
//    mt_EQUIP_UNPASS_REASON.WRITE_DATE = DateTime.Now.ToString();
//    mt_EQUIP_UNPASS_REASON.HANDLE_FLAG = "0";
//    return new MT_EQUIP_UNPASS_REASONService().InsertSQL(mt_EQUIP_UNPASS_REASON);
//}

//// Token: 0x060009F5 RID: 2549 RVA: 0x000753D8 File Offset: 0x000735D8
//public static long GetReadID()
//{
//    string strSQL = "SELECT SEQ_PUB_METR_DET.NEXTVAL FROM DUAL";
//    long result;
//    using (OracleDataReader oracleDataReader = OraHelper.ExecuteReader(strSQL))
//    {
//        if (oracleDataReader.Read())
//        {
//            result = oracleDataReader.GetInt64(0);
//        }
//        else
//        {
//            result = 0L;
//        }
//    }
//    return result;
//}

//// Token: 0x060009F6 RID: 2550 RVA: 0x00075434 File Offset: 0x00073634
//public static long GetReadID(string strTableName)
//{
//    string strSQL = string.Format("SELECT SEQ_{0}.NEXTVAL FROM DUAL", strTableName);
//    long result;
//    using (OracleDataReader oracleDataReader = OraHelper.ExecuteReader(strSQL))
//    {
//        if (oracleDataReader.Read())
//        {
//            result = oracleDataReader.GetInt64(0);
//        }
//        else
//        {
//            result = 0L;
//        }
//    }
//    return result;
//}

//// Token: 0x060009F7 RID: 2551 RVA: 0x00075498 File Offset: 0x00073698
//public static bool UpdateMeterInfoToShanXiDiDianMis(MeterBasicInfo meterinfo, string meter_id, out string strReturnMessage)
//{
//    strReturnMessage = "";
//    string config = GlobalUnit.GetConfig("MIS_USERID", "");
//    string str = GlobalUnit.GetConfig("MIS_PASSWORD", "");
//    string url = "http://10.10.10.155:7001/web/services/PublicdataService";
//    str = "X4jeynXll+vTB5W9h2+gQnyqJGo0lXQFM86MRsGQ";
//    string str2 = "05";
//    string str3 = "JD";
//    string text = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><L4S>";
//    text = text + "<USERNAME>" + config + "</USERNAME>";
//    text = text + "<PASSWORD>" + str + "</PASSWORD>";
//    text = text + "<SYSTEMCODE>" + str2 + "</SYSTEMCODE>";
//    text = text + "<COMPANYCODE>" + str3 + "</COMPANYCODE>";
//    text += "</L4S>";
//    string[] array = new string[]
//    {
//                text
//    };
//    object obj = WebServiceHelper.InvokeWebService(url, "logon4session", array);
//    bool result;
//    if (!WebServiceHelper.CheckLogin(obj.ToString()))
//    {
//        MessageBox.Show("数据上传失败，错误信息：" + WebServiceHelper.CheckLogin2(obj.ToString()));
//        result = false;
//    }
//    else
//    {
//        string xmlValue = WebServiceHelper.GetXmlValue(obj.ToString());
//        meterinfo.Mb_chrZsbh = "1122121";
//        meterinfo.Mb_chrSjdw = "本局";
//        string text2;
//        switch (meterinfo.Mb_intClfs)
//        {
//            case 0:
//                text2 = "3";
//                break;
//            case 1:
//            case 2:
//            case 3:
//            case 4:
//                text2 = "2";
//                break;
//            case 5:
//                text2 = "1";
//                break;
//            default:
//                text2 = "3";
//                break;
//        }
//        url = GlobalUnit.GetConfig("MIS_WEBSERVICE_URL", "");
//        text = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
//        text = text + "<pmetersyndata><head><iedntitycode>" + xmlValue + "</iedntitycode>";
//        text = text + "<SYSTEMCODE>" + str2 + "</SYSTEMCODE>";
//        text = text + "<COMPANYCODE>" + str3 + "</COMPANYCODE>";
//        text += "</head><pmeterlist><pmeter>";
//        text = text + "<bar_code>" + meterinfo.Mb_ChrTxm + "</bar_code>";
//        string str4 = clsMain2.getIniString("meterFacturer", meterinfo.Mb_chrzzcj, "0020", Application.StartupPath + "\\Plugins\\MisZiDian.ini");
//        text = text + "<manufacturer>" + str4 + "</manufacturer>";
//        text = text + "<made_date>" + ((meterinfo.Mb_chrCcrq == "") ? "2016" : meterinfo.Mb_chrCcrq) + "0101</made_date>";
//        text = text + "<lot_no>" + meterinfo.Mb_chrZsbh + "</lot_no>";
//        str4 = clsMain2.getIniString("orgno", meterinfo.Mb_chrSjdw, "61101", Application.StartupPath + "\\Plugins\\MisZiDian.ini");
//        text = text + "<belong_dept>" + str4 + "</belong_dept>";
//        string text3 = meterinfo.Mb_chrSjdw;
//        if (text3 != null)
//        {
//            if (text3 == "本局")
//            {
//                str4 = "3";
//                goto IL_2F5;
//            }
//            if (text3 == "集团公司")
//            {
//                str4 = "1";
//                goto IL_2F5;
//            }
//        }
//        str4 = "2";
//    IL_2F5:
//        text = text + "<pr_code>" + str4 + "</pr_code>";
//        text = text + "<pr_org>" + meterinfo.Mb_chrSjdw + "</pr_org>";
//        str4 = clsMain2.getIniString("meterSort", meterinfo.Mb_chrBlx, "01", Application.StartupPath + "\\Plugins\\MisZiDian.ini");
//        text = text + "<sort_code>" + str4 + "</sort_code>";
//        str4 = clsMain2.getIniString("meterTypeCode", meterinfo.Mb_ChrBmc, "18", Application.StartupPath + "\\Plugins\\MisZiDian.ini");
//        text = text + " <type_code>" + str4 + "</type_code>";
//        str4 = clsMain2.getIniString("meterModelNo", meterinfo.Mb_Bxh, "001", Application.StartupPath + "\\Plugins\\MisZiDian.ini");
//        text = text + "<model_code>" + str4 + "</model_code>";
//        text3 = meterinfo.Mb_chrUb;
//        if (text3 != null)
//        {
//            if (text3 == "220")
//            {
//                if (text2 == "1")
//                {
//                    str4 = "01";
//                }
//                else
//                {
//                    str4 = "03";
//                }
//                goto IL_452;
//            }
//            if (text3 == "100")
//            {
//                str4 = "04";
//                goto IL_452;
//            }
//            if (text3 == "57.7")
//            {
//                str4 = "05";
//                goto IL_452;
//            }
//        }
//        str4 = "01";
//    IL_452:
//        text = text + "<volt_code>" + str4 + "</volt_code>";
//        str4 = clsMain2.getIniString("meterRcSort", meterinfo.Mb_chrIb + "A", "001", Application.StartupPath + "\\Plugins\\MisZiDian.ini");
//        text = text + "<rated_current>" + str4 + "</rated_current>";
//        text = text + "<wiring_mode>" + text2 + "</wiring_mode>";
//        object obj2 = text;
//        text = string.Concat(new object[]
//        {
//                    obj2,
//                    "<self_factor>",
//                    Convert.ToDouble(GlobalUnit.Imax) / Convert.ToDouble(GlobalUnit.Ib),
//                    "</self_factor>"
//        });
//        text += "<overload_factor>01</overload_factor>";
//        str4 = clsMain2.getIniString("meterAccuracy", Number.getDj(meterinfo.Mb_chrBdj)[0], "01", Application.StartupPath + "\\Plugins\\MisZiDian.ini");
//        text = text + "<ap_pre_level_code>" + str4 + "</ap_pre_level_code>";
//        str4 = clsMain2.getIniString("meterAccuracy", Number.getDj(meterinfo.Mb_chrBdj)[1], "", Application.StartupPath + "\\Plugins\\MisZiDian.ini");
//        text = text + "<rp_pre_level_code>" + str4 + "</rp_pre_level_code>";
//        str4 = clsMain2.getIniString("meterConstCode", meterinfo.Mb_chrBcs, "967", Application.StartupPath + "\\Plugins\\MisZiDian.ini");
//        text = text + "<const_code>" + str4 + "</const_code>";
//        text = text + "<rp_constant>" + str4 + "</rp_constant> ";
//        obj2 = text;
//        text = string.Concat(new object[]
//        {
//                    obj2,
//                    "<meter_digits>",
//                    meterinfo.Mb_intBno,
//                    "</meter_digits></pmeter> </pmeterlist></pmetersyndata>"
//        });
//        array[0] = text;
//        obj = WebServiceHelper.InvokeWebService(url, "pMeterSynData", array);
//        List<Info> list = new List<Info>();
//        int num = -1;
//        foreach (Info info in list)
//        {
//            num = (int)Convert.ToInt16(info.processcode);
//        }
//        if (num != 0)
//        {
//            MessageBox.Show("数据上传失败，错误信息：" + WebServiceHelper.CheckLogin2(obj.ToString()));
//            result = false;
//        }
//        else
//        {
//            text = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
//            text = text + "<pdetectsetdata><head><iedntitycode>" + xmlValue + "</iedntitycode>";
//            text = text + "<SYSTEMCODE>" + str2 + "</SYSTEMCODE>";
//            text = text + "<COMPANYCODE>" + str3 + "</COMPANYCODE>";
//            text += "</head><pdetectsetlist><pdetectset>";
//            text += "<in_lot_no>12121121</in_lot_no>";
//            text += "<in_equip_categ>01</in_equip_categ>";
//            text = text + "<in_barcode>" + meterinfo.Mb_ChrTxm + "</in_barcode>";
//            str4 = clsMain2.getIniString("orgno", meterinfo.Mb_chrSjdw, "61101", Application.StartupPath + "\\Plugins\\MisZiDian.ini");
//            text = text + "<in_orgno>" + str4 + "</in_orgno>";
//            text = text + "<in_checker_name>" + meterinfo.Mb_ChrJyy + "</in_checker_name>";
//            text = text + "<in_chk_date>" + Convert.ToDateTime(meterinfo.Mb_DatJdrq).ToString("yyyyMMdd") + "</in_chk_date>";
//            text = text + " <in_chk_conc>" + ((meterinfo.Mb_chrResult == "合格") ? "0" : "1") + "</in_chk_conc>";
//            text = text + "<in_cert_id>" + meterinfo.Mb_chrZsbh + "</in_cert_id>";
//            text = text + "<in_chk_valid_date>" + Convert.ToDateTime(meterinfo.Mb_Datjjrq).ToString("yyyyMMdd") + "</in_chk_valid_date>";
//            text += "<in_status_code>1</in_status_code>";
//            text += "</pdetectset></pdetectsetlist></pdetectsetdata>";
//            array[0] = text;
//            obj = WebServiceHelper.InvokeWebService(url, "pDetectSetData", array);
//            num = -1;
//            foreach (Info info in list)
//            {
//                num = (int)Convert.ToInt16(info.processcode);
//            }
//            if (num != 0)
//            {
//                MessageBox.Show("数据上传失败，错误信息：" + WebServiceHelper.CheckLogin2(obj.ToString()));
//                result = false;
//            }
//            else
//            {
//                result = true;
//            }
//        }
//    }
//    return result;
//}

//// Token: 0x060009F8 RID: 2552 RVA: 0x00075DA8 File Offset: 0x00073FA8
//public static void GetBeforeUpdateUserInfo(MeterBasicInfo meter)
//{
//    string sqlstring = string.Format("select * from sa_user where real_name = '{0}'", meter.Mb_ChrJyy);
//    DataSet dataSet = OraHelper.Query(sqlstring);
//    if (dataSet.Tables.Count > 0)
//    {
//        DataTable dataTable = dataSet.Tables[0];
//        if (dataTable.Rows.Count > 0)
//        {
//            meter.Mb_ChrJyyNo = dataTable.Rows[0]["USER_ID"].ToString().Trim();
//        }
//    }
//    sqlstring = string.Format("select * from sa_user where real_name = '{0}'", meter.Mb_ChrHyy);
//    dataSet = OraHelper.Query(sqlstring);
//    if (dataSet.Tables.Count > 0)
//    {
//        DataTable dataTable = dataSet.Tables[0];
//        if (dataTable.Rows.Count > 0)
//        {
//            meter.Mb_ChrHyyNo = dataTable.Rows[0]["USER_ID"].ToString().Trim();
//        }
//    }
//    sqlstring = string.Format("select * from sa_user where real_name = '{0}'", meter.Mb_chrZhuGuan);
//    dataSet = OraHelper.Query(sqlstring);
//    if (dataSet.Tables.Count > 0)
//    {
//        DataTable dataTable = dataSet.Tables[0];
//        if (dataTable.Rows.Count > 0)
//        {
//            meter.Mb_chrZhuGuanNo = dataTable.Rows[0]["USER_ID"].ToString().Trim();
//        }
//    }
//}

//// Token: 0x060009F9 RID: 2553 RVA: 0x00075F24 File Offset: 0x00074124
//private static bool ExecuteSqlTrans(string SelectString, string InsertString, string UpdateString)
//{
//    DataSet dataSet = OraHelper.Query(SelectString);
//    bool result;
//    if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
//    {
//        result = (OraHelper.ExecuteSql(UpdateString) > 0);
//    }
//    else
//    {
//        result = (OraHelper.ExecuteSql(InsertString) > 0);
//    }
//    return result;
//}

//// Token: 0x060009FA RID: 2554 RVA: 0x00075F84 File Offset: 0x00074184
//public static bool GetMeterSNByTaskNo(string taskNo, string equipmentNo, ref DnbGroupInfo groupInfo)
//{
//    List<MT_METER_POSI> list = new MT_METER_POSIService().GetList(taskNo, equipmentNo);
//    bool result;
//    if (list.Count == 0)
//    {
//        result = false;
//    }
//    else
//    {
//        foreach (MT_METER_POSI mt_METER_POSI in list)
//        {
//            int num = 0;
//            if (int.TryParse(mt_METER_POSI.EQUIP_INDX, out num))
//            {
//                if (num > 0 && num <= groupInfo.MeterGroup.Count)
//                {
//                    groupInfo.MeterGroup[num - 1].Mb_ChrTxm = mt_METER_POSI.BAR_CODE;
//                    groupInfo.MeterGroup[num - 1].YaoJianYnSave = true;
//                }
//            }
//        }
//        result = true;
//    }
//    return result;
//}



//// Token: 0x04000475 RID: 1141
//private static WebServiceClass m_webSev;

//// Token: 0x04000476 RID: 1142
//public static Dictionary<string, StPCodeDicForMis> g_DicPCodeTable;
//	}
//}


