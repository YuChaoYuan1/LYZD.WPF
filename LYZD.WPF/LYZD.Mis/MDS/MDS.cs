using LYZD.Core.Enum;
using LYZD.Core.Helper;
using LYZD.Core.Model.DnbModel.DnbInfo;
using LYZD.Core.Model.Meter;
using LYZD.Core.Model.Schema;
using LYZD.DAL.Config;
using LYZD.Mis.Common;
using LYZD.Mis.MDS.Table;
using LYZD.Mis.NanRui.LRDataTable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LYZD.Mis.MDS
{
    public class MDS : OracleHelper, IMis
    {
        /// <summary>
        /// 任务单号
        /// </summary>
        private string TaskNO = "";
        /// <summary>
        /// 成功上传数量
        /// </summary>
        private int SusessCount = 0;//
        public MDS(string ip, int port, string dataSource, string userId, string pwd, string url)
        {
            this.Ip = ip;
            this.Port = port;
            this.DataSource = dataSource;
            this.UserId = userId;
            this.Password = pwd;
            this.WebServiceURL = url;
        }
        public static Dictionary<string, Dictionary<string, string>> PCodeTable = new Dictionary<string, Dictionary<string, string>>();

        public bool Down(string MD_BarCode, ref TestMeterInfo meter)
        {
            if (string.IsNullOrEmpty(MD_BarCode)) return false;

            if (PCodeTable.Count <= 0)
                GetDicPCodeTable();
            string sql = string.Format(@"SELECT * FROM mt_detect_out_equip t1 INNER JOIN mt_meter t2 ON t1.bar_code=t2.bar_code WHERE t2.bar_code='{0}' ORDER BY t1.write_date DESC", MD_BarCode.Trim());
            DataTable dt = ExecuteReader(sql);
            if (dt.Rows.Count <= 0) return false;
            DataRow row = dt.Rows[0];
            meter.Meter_ID = row["METER_ID"].ToString().Trim();
            meter.MD_BarCode = row["BAR_CODE"].ToString().Trim();              //条形码
            //meter.MD_AssetNo = row["ASSET_NO"].ToString().Trim();              //申请编号 --资产编号
            meter.MD_MadeNo = row["MADE_NO"].ToString().Trim();                //出厂编号
            meter.MD_TaskNo = row["DETECT_TASK_NO"].ToString().Trim();         //检定任务单，申请编号

            //meter.Type = GetPName("meterTypeCode", row["TYPE_CODE"]);

            return true;
        }

        public bool SchemeDown(string MD_BarCode, out string schemeName, out Dictionary<string, SchemaNode> Schema)
        {
            Schema = new Dictionary<string, SchemaNode>();
            schemeName = "";
            return true;
        }
        

        Dictionary<string, string> nameList = new Dictionary<string, string>(); //用来判断是否已经添加过了该项，用于需要同时698和645协议检查的


        public Dictionary<string, SchemaNode> SortScheme(Dictionary<string, SchemaNode> Schema) 
        {
            Dictionary<string, SchemaNode> TemScheme = new Dictionary<string, SchemaNode>();

            //if (Schema.ContainsKey(ProjectID.接线检查))
            //    TemScheme.Add(ProjectID.接线检查,Schema[ProjectID.接线检查]);
            //if (Schema.ContainsKey(ProjectID.密钥更新_预先调试))
            //    TemScheme.Add(ProjectID.密钥更新_预先调试, Schema[ProjectID.密钥更新_预先调试]);
            //if (Schema.ContainsKey(ProjectID.密钥恢复_预先调试))
            //    TemScheme.Add(ProjectID.密钥恢复_预先调试, Schema[ProjectID.密钥恢复_预先调试]);
            //if (Schema.ContainsKey(ProjectID.起动试验))
            //    TemScheme.Add(ProjectID.起动试验, Schema[ProjectID.起动试验]);
            //if (Schema.ContainsKey(ProjectID.潜动试验))
            //    TemScheme.Add(ProjectID.潜动试验, Schema[ProjectID.潜动试验]);
            //if (Schema.ContainsKey(ProjectID.基本误差试验))
            //    TemScheme.Add(ProjectID.基本误差试验, Schema[ProjectID.基本误差试验]);
            //if (Schema.ContainsKey(ProjectID.电能表常数试验))
            //    TemScheme.Add(ProjectID.电能表常数试验, Schema[ProjectID.电能表常数试验]);
            //if (Schema.ContainsKey(ProjectID.GPS对时))
            //    TemScheme.Add(ProjectID.GPS对时, Schema[ProjectID.GPS对时]);
            //if (Schema.ContainsKey(ProjectID.日计时误差))
            //    TemScheme.Add(ProjectID.日计时误差, Schema[ProjectID.日计时误差]);
            //if (Schema.ContainsKey(ProjectID.需量示值误差))
            //    TemScheme.Add(ProjectID.需量示值误差, Schema[ProjectID.需量示值误差]);
            //if (Schema.ContainsKey(ProjectID.电量清零))
            //    TemScheme.Add(ProjectID.电量清零, Schema[ProjectID.电量清零]);
            //if (Schema.ContainsKey(ProjectID.误差一致性))
            //    TemScheme.Add(ProjectID.误差一致性, Schema[ProjectID.误差一致性]);
            //if (Schema.ContainsKey(ProjectID.误差变差))
            //    TemScheme.Add(ProjectID.误差变差, Schema[ProjectID.误差变差]);
            //if (Schema.ContainsKey(ProjectID.负载电流升将变差))
            //    TemScheme.Add(ProjectID.负载电流升将变差, Schema[ProjectID.负载电流升将变差]);
            //if (Schema.ContainsKey(ProjectID.通讯协议检查试验))
            //    TemScheme.Add(ProjectID.通讯协议检查试验, Schema[ProjectID.通讯协议检查试验]);
            //if (Schema.ContainsKey(ProjectID.通讯协议检查试验2))
            //    TemScheme.Add(ProjectID.通讯协议检查试验2, Schema[ProjectID.通讯协议检查试验2]);
            //if (Schema.ContainsKey(ProjectID.身份认证))
            //    TemScheme.Add(ProjectID.身份认证, Schema[ProjectID.身份认证]);
            //if (Schema.ContainsKey(ProjectID.远程控制))
            //    TemScheme.Add(ProjectID.远程控制, Schema[ProjectID.远程控制]);
            //if (Schema.ContainsKey(ProjectID.报警功能))
            //    TemScheme.Add(ProjectID.报警功能, Schema[ProjectID.报警功能]);
            //if (Schema.ContainsKey(ProjectID.远程保电))
            //    TemScheme.Add(ProjectID.远程保电, Schema[ProjectID.远程保电]);
            //if (Schema.ContainsKey(ProjectID.保电解除))
            //    TemScheme.Add(ProjectID.保电解除, Schema[ProjectID.保电解除]);
            //if (Schema.ContainsKey(ProjectID.数据回抄))
            //    TemScheme.Add(ProjectID.数据回抄, Schema[ProjectID.数据回抄]);
            //if (Schema.ContainsKey(ProjectID.钱包初始化))
            //    TemScheme.Add(ProjectID.钱包初始化, Schema[ProjectID.钱包初始化]);
            //if (Schema.ContainsKey(ProjectID.密钥更新))
            //    TemScheme.Add(ProjectID.密钥更新, Schema[ProjectID.密钥更新]);

            //  身份认证 = "25001";
            //  远程控制 = "25002";
            //  报警功能 = "25003";
            //  远程保电 = "25004";
            //  保电解除 = "25005";
            //  数据回抄 = "25006";
            //  钱包初始化 = "25007";
            //  密钥更新 = "25008";
            //  密钥恢复 = "25009";

            return TemScheme;
        }

        private int GetTestIndex(string name,string opertype)
        {
            int TestIndex = 1;
            string key = name +"|"+ opertype + "|" + TestIndex;
            while (nameList.ContainsKey(key))  //知道他不存在为止
            {
                TestIndex++;
                key = name + "|" + opertype + "|" + TestIndex;
            }
            nameList.Add(key, key);
            return TestIndex;
        }

        /// <summary>
        /// 根据编号获取通讯协议检查的名字
        /// </summary>
        /// <param name="Num"></param>
        /// <returns></returns>
        private string GetProtocolName(string Num)
        {
            string connProtocolItem = Num;
            switch (Num)
            {
                case "0":
                    connProtocolItem = "自动循环显示第1屏";
                    break;
                case "1":
                    connProtocolItem = "自动循环显示第2屏";
                    break;
                case "2":
                    connProtocolItem = "自动循环显示第3屏";
                    break;
                case "3":
                    connProtocolItem = "自动循环显示第4屏";
                    break;
                case "4":
                    connProtocolItem = "自动循环显示第5屏";
                    break;
                case "5":
                    connProtocolItem = "自动循环显示第6屏";
                    break;
                case "6":
                    connProtocolItem = "自动循环显示第7屏";
                    break;
                case "7":
                    connProtocolItem = "按键循环显示第1屏";
                    break;
                case "8":
                    connProtocolItem = "按键循环显示第2屏";
                    break;
                case "9":
                    connProtocolItem = "按键循环显示第3屏";
                    break;
                case "10":
                    connProtocolItem = "按键循环显示第4屏";
                    break;
                case "11":
                    connProtocolItem = "按键循环显示第5屏";
                    break;
                case "12":
                    connProtocolItem = "按键循环显示第6屏";
                    break;
                case "13":
                    connProtocolItem = "按键循环显示第7屏";
                    break;
                case "14":
                    connProtocolItem = "按键循环显示第8屏";
                    break;
                case "15":
                    connProtocolItem = "按键循环显示第9屏";
                    break;
                case "16":
                    connProtocolItem = "按键循环显示第10屏";
                    break;
                case "17":
                    connProtocolItem = "按键循环显示第11屏";
                    break;
                case "18":
                    connProtocolItem = "按键循环显示第12屏";
                    break;
                case "19":
                    connProtocolItem = "按键循环显示第13屏";
                    break;
                case "20":
                    connProtocolItem = "按键循环显示第14屏";
                    break;
                case "21":
                    connProtocolItem = "按键循环显示第15屏";
                    break;
                case "22":
                    connProtocolItem = "按键循环显示第16屏";
                    break;
                case "23":
                    connProtocolItem = "按键循环显示第17屏";
                    break;
                case "24":
                    connProtocolItem = "按键循环显示第18屏";
                    break;
                case "25":
                    connProtocolItem = "按键循环显示第19屏";
                    break;
                case "26":
                    connProtocolItem = "按键循环显示第20屏";
                    break;
                case "27":
                    connProtocolItem = "按键循环显示第21屏";
                    break;
                case "28":
                    connProtocolItem = "按键循环显示第22屏";
                    break;
                case "29":
                    connProtocolItem = "按键循环显示第23屏";
                    break;
                case "30":
                    connProtocolItem = "按键循环显示第24屏";
                    break;
                case "31":
                    connProtocolItem = "按键循环显示第25屏";
                    break;
                case "32":
                    connProtocolItem = "按键循环显示第26屏";
                    break;
                case "33":
                    connProtocolItem = "按键循环显示第27屏";
                    break;
                case "34":
                    connProtocolItem = "按键循环显示第28屏";
                    break;
                case "35":
                    connProtocolItem = "按键循环显示第29屏";
                    break;
                case "36":
                    connProtocolItem = "按键循环显示第30屏";
                    break;
                case "37":
                    connProtocolItem = "按键循环显示第31屏";
                    break;
                case "38":
                    connProtocolItem = "按键循环显示第32屏";
                    break;
                case "39":
                    connProtocolItem = "按键循环显示第33屏";
                    break;
                case "40":
                    connProtocolItem = "按键循环显示第34屏";
                    break;
                case "41":
                    connProtocolItem = "按键循环显示第35屏";
                    break;
                case "42":
                    connProtocolItem = "按键循环显示第36屏";
                    break;
                case "43":
                    connProtocolItem = "按键循环显示第37屏";
                    break;
                case "44":
                    connProtocolItem = "按键循环显示第38屏";
                    break;
                case "45":
                    connProtocolItem = "按键循环显示第39屏";
                    break;
                case "46":
                    connProtocolItem = "按键循环显示第40屏";
                    break;
                case "47":
                    connProtocolItem = "按键循环显示第41屏";
                    break;
                case "48":
                    connProtocolItem = "按键循环显示第42屏";
                    break;
                case "49":
                    connProtocolItem = "按键循环显示第43屏";
                    break;
                case "50":
                    connProtocolItem = "按键循环显示第44屏";
                    break;
                case "51":
                    connProtocolItem = "按键循环显示第45屏";
                    break;
                case "52":
                    connProtocolItem = "按键循环显示第46屏";
                    break;
                case "53":
                    connProtocolItem = "按键循环显示第47屏";
                    break;
                case "54":
                    connProtocolItem = "按键循环显示第48屏";
                    break;
                case "55":
                    connProtocolItem = "按键循环显示第49屏";
                    break;
                case "56":
                    connProtocolItem = "按键循环显示第50屏";
                    break;
                case "57":
                    connProtocolItem = "按键循环显示第51屏";
                    break;
                case "58":
                    connProtocolItem = "按键循环显示第52屏";
                    break;
                case "59":
                    connProtocolItem = "按键循环显示第53屏";
                    break;
                case "60":
                    connProtocolItem = "按键循环显示第54屏";
                    break;
                case "61":
                    connProtocolItem = "按键循环显示第55屏";
                    break;
                case "62":
                    connProtocolItem = "按键循环显示第56屏";
                    break;
                case "63":
                    connProtocolItem = "按键循环显示第57屏";
                    break;
                case "64":
                    connProtocolItem = "按键循环显示第58屏";
                    break;
                case "65":
                    connProtocolItem = "按键循环显示第59屏";
                    break;
                case "66":
                    connProtocolItem = "按键循环显示第60屏";
                    break;
                case "67":
                    connProtocolItem = "按键循环显示第61屏";
                    break;
                case "68":
                    connProtocolItem = "按键循环显示第62屏";
                    break;
                case "69":
                    connProtocolItem = "按键循环显示第63屏";
                    break;
                case "70":
                    connProtocolItem = "按键循环显示第64屏";
                    break;
                case "71":
                    connProtocolItem = "按键循环显示第65屏";
                    break;
                case "72":
                    connProtocolItem = "按键循环显示第66屏";
                    break;
                case "73":
                    connProtocolItem = "按键循环显示第67屏";
                    break;
                case "74":
                    connProtocolItem = "按键循环显示第68屏";
                    break;
                case "75":
                    connProtocolItem = "按键循环显示第69屏";
                    break;
                case "76":
                    connProtocolItem = "按键循环显示第70屏";
                    break;
                case "77":
                    connProtocolItem = "按键循环显示第71屏";
                    break;
                case "78":
                    connProtocolItem = "按键循环显示第72屏";
                    break;
                case "79":
                    connProtocolItem = "按键循环显示第73屏";
                    break;
                case "80":
                    connProtocolItem = "按键循环显示第74屏";
                    break;
                case "81":
                    connProtocolItem = "按键循环显示第75屏";
                    break;
                case "82":
                    connProtocolItem = "按键循环显示第76屏";
                    break;
                case "83":
                    connProtocolItem = "按键循环显示第77屏";
                    break;
                case "84":
                    connProtocolItem = "按键循环显示第78屏";
                    break;
                case "85":
                    connProtocolItem = "按键循环显示第79屏";
                    break;
                case "86":
                    connProtocolItem = "按键循环显示第80屏";
                    break;
                case "87":
                    connProtocolItem = "按键循环显示第81屏";
                    break;
                case "88":
                    connProtocolItem = "按键循环显示第82屏";
                    break;
                case "89":
                    connProtocolItem = "按键循环显示第83屏";
                    break;
                case "90":
                    connProtocolItem = "按键循环显示第84屏";
                    break;
                case "91":
                    connProtocolItem = "有功常数";
                    break;
                case "92":
                    connProtocolItem = "资产编号";
                    break;
                case "93":
                    connProtocolItem = "有功组合方式特征字";
                    break;
                case "94":
                    connProtocolItem = "电表运行特征字1";
                    break;
                case "95":
                    connProtocolItem = "自动循环显示屏数";
                    break;
                case "96":
                    connProtocolItem = "按键显示屏数";
                    break;
                case "97":
                    connProtocolItem = "每月第1结算日";
                    break;
                default:
                    break;
            }
            return connProtocolItem;
        }

        private string JoinValue(params string[] values)
        {
            return string.Join("|", values);
        }


        /// <summary>
        /// 获取名字
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        protected string GetPName(string type, object code)
        {
            string s = code.ToString().Trim();
            if (PCodeTable[type].ContainsKey(s))
                return PCodeTable[type][s];

            return "";
        }


        public void ShowPanel(Control panel)
        {
            throw new NotImplementedException();
        }

        public bool Update(List<TestMeterInfo> meters)
        {
            throw new NotImplementedException();
        }

        public bool Update(TestMeterInfo meterInfo)
        {
            List<string> sqlList = new List<string>();

            MT_METER meter = GetMt_meter(meterInfo.MD_BarCode);
            if (meter == null) return false;
            TaskNO = meter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO;
            //meterInfo.Other5 = TaskNO;
            meterInfo.MD_BarCode = meterInfo.MD_BarCode.Trim();

            if (PCodeTable.Count <= 0)
                GetDicPCodeTable();

            #region 删除旧数据
            sqlList.Add("delete from MT_BASICERR_MET_CONC where detect_task_no = '" + TaskNO + "' and bar_code='" + meterInfo.MD_BarCode + "' "); //基本误差
            sqlList.Add("delete from MT_CONST_MET_CONC where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");  //走字
            sqlList.Add("delete from MT_STARTING_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' "); //启动
            sqlList.Add("delete from MT_CREEPING_MET_CONC where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");//潜动
            sqlList.Add("delete from MT_DAYERR_MET_CONC  where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' "); //日记时
            sqlList.Add("delete from MT_STANDARD_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' "); //规约一致性
            sqlList.Add("delete from MT_ESAM_READ_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' "); //费控试验
            sqlList.Add("delete from MT_DETECT_RSLT where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");  //综合结论


            //sqlList.Add("delete from MT_INTUIT_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_MEASURE_REPEAT_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_DEVIATION_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_ESAM_MET_CONC where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_DETECT_RSLT where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_PRESETPARAM_CHECK_MET_CONC where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_PRESETPARAM_MET_CONC where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_PASSWORD_CHANGE_MET_CONC where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_CONSIST_MET_CONC where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_DETECT_RSLT where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from Mt_OVERLOAD_MET_CONC  where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_TS_MET_CONC  where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_ERROR_MET_CONC where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_VOLT_MET_CONC  where  detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_VARIATION_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_DEMANDVALUE_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_ESAM_SECURITY_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_HUTCHISON_COMBINA_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_TIME_ERROR_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_INFLUENCE_QTY_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_GPS_TIMING where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_INFLUENCE_QTY_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_WAVE_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_WARNNING_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_EP_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_EC_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_PARA_SETTING_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_FEE_TMNL_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_POWER_MEASURE_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_POWER_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_MAX_DEMANDVALUE_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_FEE_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");

            //sqlList.Add("delete from MT_CONTROL_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_EQ_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_SURPLUS_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");
            //sqlList.Add("delete from MT_CLOCK_VALUE_MET_CONC where detect_task_no = '" + TaskNO + "'and bar_code='" + meterInfo.MD_BarCode + "' ");

            #endregion

            //Execute(sqlList);     //TODO2删除旧的数据，先注释了
            //sqlList.Clear();

            string[] sql;
            ////外观检查试验
            ////sqlList.Add(GetMT_INTUIT_MET_CONCByMt(meter, meterInfo));

            ////需量示值误差
            ////sql = GetMT_DEMANDVALUE_MET_CONC(meter, meterInfo);
            ////if (sql != null)
            ////{
            ////    foreach (string strQuest in sql)
            ////    {
            ////        sqlList.Add(strQuest);
            ////    }
            ////}
            ////启动
            //sql = GetMT_STARTING_MET_CONCByMt(meter, meterInfo);
            //if (sql != null)
            //{
            //    foreach (string strQuest in sql)
            //    {
            //        sqlList.Add(strQuest);
            //    }
            //}
            ////潜动
            //sql = GetMT_CREEPING_MET_CONCByMt(meter, meterInfo);
            //if (sql != null)
            //{
            //    foreach (string strQuest in sql)
            //    {
            //        sqlList.Add(strQuest);
            //    }
            //}
            ////规约一致性
            ////sql = GetMT_STANDARD_MET_CONCByMt(meter, meterInfo);
            ////if (sql != null)
            ////{
            ////    foreach (string strQuest in sql)
            ////    {
            ////        sqlList.Add(strQuest);
            ////    }
            ////}

            ////基本误差试验
            //sql = GetMT_BASICERR_MET_CONCByMt(meter, meterInfo);
            //if (sql != null)
            //{
            //    foreach (string strQuest in sql)
            //    {
            //        sqlList.Add(strQuest);
            //    }
            //}

            //////标准偏差（测量重复性检测）
            ////sql = GetMT_DEVIATION_MET_CONCByMt(meter, meterInfo);
            ////if (sql != null)
            ////{
            ////    foreach (string strQuest in sql)
            ////    {
            ////        sqlList.Add(strQuest);
            ////    }
            ////}

            ////常数
            //sql = GetMT_CONST_MET_CONCByMt(meter, meterInfo);
            //if (sql != null)
            //{
            //    foreach (string strQuest in sql)
            //    {
            //        sqlList.Add(strQuest);
            //    }
            //}
            //////日计时
            //sqlList.Add(GetMT_DAYERR_MET_CONCByMt(meter, meterInfo));


            ////时间误差
            ////sqlList.Add(GetMT_CLOCK_VALUE_MET_CONC(meter, meterInfo));

            ////时段投切
            ////sql = GetMT_TS_MET_CONC(meter, meterInfo);
            ////if (sql != null)
            ////{
            ////    foreach (string strQuest in sql)
            ////    {
            ////        sqlList.Add(strQuest);
            ////    }
            ////}

            //////密钥更新
            ////sqlList.Add(GetMT_ESAM_MET_CONCByMt(meter, meterInfo));

            //////身份认证
            ////sqlList.Add(GetMT_ESAM_SECURITY_MET_CONCByMt(meter, meterInfo));


            ////总结论
            ////sqlList.Add(GetMT_DETECT_RSLTByMt(meter, meterInfo));


            Execute(sqlList);
            SusessCount++;
            return true;
        }
        public void UpdateCompleted()
        {
            string sys_no = "450";

            string xml = "<PARA>";
            xml += "<SYS_NO>" + sys_no + "</SYS_NO>";
            xml += "<DETECT_TASK_NO>" + TaskNO + "</DETECT_TASK_NO>";
            xml += "</PARA>";
            string[] args = new string[1];
            args[0] = xml;
            object result = WebServiceHelper.InvokeWebService(WebServiceURL, "getDETedTestData", args);
            if (result.ToString().ToUpper() == "FALSE")
                result = WebServiceHelper.InvokeWebService(WebServiceURL, "getDETedTestData", args);


            if (!WebServiceHelper.GetResultByXml(result.ToString()))
            {
                MessageBox.Show("数据上传失败，调用分项结论接口错误信息：" + WebServiceHelper.GetMessageByXml(result.ToString()));
                return;
            }

            xml = "<PARA>";
            xml += "<SYS_NO>" + sys_no + "</SYS_NO>";
            xml += "<DETECT_TASK_NO>" + TaskNO + "</DETECT_TASK_NO>";
            xml += "<VALID_QTY>" + SusessCount + "</VALID_QTY>";
            xml += "</PARA>";
            args[0] = xml;
            result = WebServiceHelper.InvokeWebService(WebServiceURL, "setResults", args);
            if (result.ToString().ToUpper() == "FALSE")
                result = WebServiceHelper.InvokeWebService(WebServiceURL, "setResults", args);

            if (!WebServiceHelper.GetResultByXml(result.ToString()))
            {
                MessageBox.Show("数据上传失败，调用综合结论接口错误信息：" + WebServiceHelper.GetMessageByXml(result.ToString()));
                return;
            }
        }

        public void UpdateInit()
        {
            SusessCount = 0;
        }

        //#region 检定项目数据
        //protected string GetPCode(string type, string name)
        //{
        //    string code = "";
        //    Dictionary<string, string> dic = PCodeTable[type];
        //    foreach (string k in dic.Keys)
        //    {
        //        if (dic[k] == name)
        //        {
        //            code = k;
        //            break;
        //        }
        //    }
        //    return code;
        //}



        ///// <summary>
        ///// 基本误差数据
        ///// </summary>
        //private string[] GetMT_BASICERR_MET_CONCByMt(MT_METER mtMeter, TestMeterInfo meter)
        //{
        //    List<string> sql = new List<string>();
        //    string[] keys = new string[meter.MeterErrors.Keys.Count];
        //    meter.MeterErrors.Keys.CopyTo(keys, 0);
        //    for (int i = 0; i < keys.Length; i++)
        //    {
        //        string key = keys[i];
        //        MeterError meterErr = meter.MeterErrors[key];

        //        string[] wc = meterErr.WCData.Split('|');
        //        if (wc.Length < 2) continue;


        //        MT_BASICERR_MET_CONC entity = new MT_BASICERR_MET_CONC
        //        {
        //            DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO,
        //            EQUIP_CATEG = "01",
        //            SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO,
        //            DETECT_EQUIP_NO = meter.MD_DeviceID,
        //            DETECT_UNIT_NO = string.Empty,
        //            POSITION_NO = meter.MD_Epitope.ToString(),
        //            BAR_CODE = mtMeter.BAR_CODE,
        //            DETECT_DATE = meter.VerifyDate.ToString(),
        //            PARA_INDEX = "1",
        //            DETECT_ITEM_POINT = (i + 1).ToString(),
        //            IS_VALID = "1",
        //            HANDLE_FLAG = "0",
        //            HANDLE_DATE = ""
        //        };
        //        entity.BOTH_WAY_POWER_FLAG = GetPCode("powerFlag", meterErr.GLFX);
        //        string abc = "";
        //        string strYj;
        //        switch (meterErr.YJ)
        //        {
        //            case "A":
        //                strYj = "A元";
        //                break;
        //            case "B":
        //                strYj = "B元";
        //                break;
        //            case "C":
        //                strYj = "C元";
        //                break;
        //            case "H":
        //                strYj = "合元";
        //                break;
        //            default:
        //                strYj = "合元";
        //                break;
        //        }
        //        if (strYj == "合元")
        //        {
        //            if (meter.MD_WiringMode == "三相三线")
        //                abc = "AC";
        //            else
        //                abc = "ABC";
        //        }
        //        else
        //            abc = strYj.Trim('元');

        //        entity.IABC = GetPCode("currentPhaseCode", abc);
        //        entity.LOAD_VOLTAGE = GetPCode("meterTestVolt", "100%Un");
        //        entity.LOAD_CURRENT = GetPCode("meterTestCurLoad", meterErr.IbX);
        //        entity.FREQ = GetPCode("meterTestFreq", "50");
        //        entity.PF = GetPCode("meterTestPowerFactor", meterErr.GLYS.Trim());

        //        entity.DETECT_CIRCLE = meterErr.Circle;// "2";          //Circle
        //        entity.SIMPLING = "2";


        //        entity.ERROR = meterErr.WCData;
        //        entity.AVE_ERR = meterErr.WCValue;    //平均值
        //        entity.INT_CONVERT_ERR = meterErr.WCHZ;  //化整值
        //        if (meterErr.Limit != null)
        //        {
        //            entity.ERR_UP = meterErr.Limit.Trim().Split('|')[0];   //误差上限
        //            entity.ERR_DOWN = meterErr.Limit.Trim().Split('|')[1];
        //        }

        //        entity.CONC_CODE = meterErr.Result == Const.合格 ? "01" : "02";
        //        entity.WRITE_DATE = DateTime.Now.ToString();
        //        entity.HANDLE_FLAG = "0";
        //        sql.Add(entity.ToInsertString());
        //    }
        //    return sql.ToArray(); ;

        //}
        ///// <summary>
        ///// 标准偏差（测量重复性检测）
        ///// </summary>
        //private string[] GetMT_DEVIATION_MET_CONCByMt(MT_METER mtMeter, TestMeterInfo meter)
        //{
        //    List<string> sql = new List<string>();
        //    //string[] keys = new string[meter.MeterErrors.Keys.Count];
        //    //meter.MeterErrors.Keys.CopyTo(keys, 0);
        //    //for (int i = 0; i < keys.Length; i++)
        //    //{
        //    //    string key = keys[i];

        //    //    if (key.Length <= 3) continue;       //如果ID长度是3表示是大项目，则跳过
        //    //    if (key.Substring(0, 1) != "2") continue;

        //    //    MeterError meterErr = meter.MeterErrors[key];

        //    //    string[] wc = meterErr.WcMore.Split('|');
        //    //    if (wc.Length <= 2) continue;

        //    //    string tmpHi = key.Substring(0, 3);            //取出误差类别+功率方向+元件
        //    //    string tmpLo = key.Substring(7);               //取出谐波+相序
        //    //    string tmpGlys = key.Substring(3, 2);          //取出功率因素
        //    //    string tmpxIb = key.Substring(5, 2);           //取出电流倍数

        //    //    MT_MEASURE_REPEAT_MET_CONC entity = new MT_MEASURE_REPEAT_MET_CONC
        //    //    {
        //    //        DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO,
        //    //        EQUIP_CATEG = "01",
        //    //        SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO,
        //    //        DETECT_EQUIP_NO = meter.BenthNo,
        //    //        DETECT_UNIT_NO = string.Empty,
        //    //        POSITION_NO = meter.MD_Epitope.ToString(),
        //    //        BAR_CODE = mtMeter.BAR_CODE,
        //    //        DETECT_DATE = meter.VerifyDate.ToString(),
        //    //        PARA_INDEX = "1",
        //    //        DETECT_ITEM_POINT = (i + 1).ToString(),
        //    //        CONC_CODE = meterErr.Result == Const.合格 ? "01" : "02",
        //    //        WRITE_DATE = DateTime.Now.ToString(),
        //    //        HANDLE_FLAG = "0",

        //    //        IS_VALID = "1",
        //    //        BOTH_WAY_POWER_FLAG = tmpHi.Substring(1, 1)
        //    //    };

        //    //    string abc = "";
        //    //    switch (int.Parse(tmpHi.Substring(2, 1)))
        //    //    {
        //    //        case 1:
        //    //            abc = "A元";
        //    //            break;
        //    //        case 2:
        //    //            abc = "B元";
        //    //            break;
        //    //        case 3:
        //    //            abc = "C元";
        //    //            break;
        //    //        case 4:
        //    //            abc = "合元";
        //    //            break;
        //    //        default:
        //    //            abc = "合元";
        //    //            break;
        //    //    }

        //    //    if (abc == "合元")
        //    //    {
        //    //        if (meter.MD_WiringMode == "三相三线")
        //    //            abc = "AC";
        //    //        else
        //    //            abc = "ABC";
        //    //    }
        //    //    else
        //    //        abc = abc.Trim('元');
        //    //    entity.IABC = GetPCode("currentPhaseCode", abc);

        //    //    entity.LOAD_VOLTAGE = GetPCode("meterTestVolt", "100%Un");
        //    //    entity.LOAD_CURRENT = GetPCode("meterTestCurLoad", meterErr.IbX);
        //    //    entity.FREQ = GetPCode("meterFreq", "50");
        //    //    entity.PF = GetPCode("meterTestPowerFactor", meterErr.GLYS);


        //    //    string[] arrValue = meterErr.WcMore.Split('|');
        //    //    if (arrValue.Length >= 6)
        //    //    {
        //    //        entity.SIMPLING = arrValue[0] + "|" + arrValue[1] + "|" + arrValue[2] + "|" + arrValue[3] + "|" + arrValue[4];
        //    //        entity.DEVIATION_LIMT = arrValue[5];
        //    //    }

        //    //    sql.Add(entity.ToInsertString());
        //    //}
        //    return sql.ToArray(); ;
        //}
        ///// <summary>
        ///// 启动
        ///// </summary>
        //private string[] GetMT_STARTING_MET_CONCByMt(MT_METER mtMeter, TestMeterInfo meter)
        //{
        //    Dictionary<string, MeterQdQid> meterQdQids = meter.MeterQdQids;
        //    List<string> sql = new List<string>();
        //    int index = 0;
        //    foreach (string key in meterQdQids.Keys)
        //    {
        //        string current = Convert.ToSingle(meterQdQids[key].Current) + "Ib";

        //        MT_STARTING_MET_CONC entity = new MT_STARTING_MET_CONC
        //        {
        //            DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO,
        //            EQUIP_CATEG = "01",
        //            SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO,
        //            DETECT_EQUIP_NO = meter.MD_DeviceID,
        //            DETECT_UNIT_NO = string.Empty,
        //            POSITION_NO = meter.MD_Epitope.ToString(),
        //            BAR_CODE = mtMeter.BAR_CODE,
        //            DETECT_DATE = meter.VerifyDate.ToString(),
        //            PARA_INDEX = index.ToString(),
        //            DETECT_ITEM_POINT = index.ToString(),
        //            WRITE_DATE = DateTime.Now.ToString(),
        //            HANDLE_FLAG = "0",
        //            CONC_CODE = meter.MeterQdQids[key].Result.Trim() == Const.合格 ? "01" : "02",

        //            IS_VALID = "1",
        //            LOAD_VOLTAGE = GetPCode("meterTestVolt", "100%Un"),//潜动默认115%
        //            PULES = "1",
        //            BOTH_WAY_POWER_FLAG = GetPCode("powerFlag", meterQdQids[key].PowerWay),
        //            //LOAD_CURRENT = g_DicPCodeTable["meterTestCurLoad",current),
        //            LOAD_CURRENT = meterQdQids[key].Current.ToString(),

        //            START_CURRENT = Convert.ToSingle(meterQdQids[key].Current).ToString("F5"),
        //        };

        //        if (!string.IsNullOrEmpty(meterQdQids[key].StandartTime))
        //            entity.TEST_TIME = (Convert.ToSingle(meterQdQids[key].StandartTime) * 60).ToString("F0");
        //        if (!string.IsNullOrEmpty(meterQdQids[key].ActiveTime))
        //            entity.REAL_TEST_TIME = (Convert.ToSingle(meterQdQids[key].ActiveTime) * 60).ToString("F0");

        //        entity.LOAD_CURRENT = GetPCode("meterTestCurLoad", current);
        //        sql.Add(entity.ToInsertString());
        //        index++;
        //    }
        //    return sql.ToArray();
        //}

        ///// <summary>
        ///// 潜动
        ///// </summary>
        //private string[] GetMT_CREEPING_MET_CONCByMt(MT_METER mtMeter, TestMeterInfo meter)
        //{
        //    int index = 0;
        //    Dictionary<string, MeterQdQid> meterQdQids = meter.MeterQdQids;

        //    List<string> sql = new List<string>();
        //    foreach (string key in meterQdQids.Keys)
        //    {
        //        MT_CREEPING_MET_CONC entity = new MT_CREEPING_MET_CONC
        //        {
        //            DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO,
        //            EQUIP_CATEG = "01",
        //            SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO,
        //            DETECT_EQUIP_NO = meter.MD_DeviceID,
        //            DETECT_UNIT_NO = string.Empty,
        //            POSITION_NO = meter.MD_Epitope.ToString(),
        //            BAR_CODE = mtMeter.BAR_CODE,
        //            DETECT_DATE = meter.VerifyDate.ToString(),
        //            PARA_INDEX = index.ToString(),
        //            DETECT_ITEM_POINT = index.ToString(),
        //            WRITE_DATE = DateTime.Now.ToString(),
        //            HANDLE_FLAG = "0",
        //            CONC_CODE = meter.MeterQdQids[key].Result.Trim() == Const.合格 ? "01" : "02",

        //            IS_VALID = "1",
        //            LOAD_VOLTAGE = GetPCode("meterTestVolt", "115%Un"),//潜动默认115%
        //            PULES = "0",
        //            BOTH_WAY_POWER_FLAG = GetPCode("powerFlag", meter.MeterQdQids[key].PowerWay),
        //            LOAD_CURRENT = GetPCode("meterTestCurLoad", "0"),
        //            TEST_TIME = (Convert.ToSingle(meterQdQids[key].ActiveTime) * 60).ToString("F0"),
        //            REAL_TEST_TIME = (Convert.ToSingle(meterQdQids[key].StandartTime) * 60).ToString("F0"),
        //        };
        //        if (!string.IsNullOrEmpty(meterQdQids[key].ActiveTime))
        //            entity.TEST_TIME = (Convert.ToSingle(meterQdQids[key].ActiveTime) * 60).ToString("F0");
        //        if (!string.IsNullOrEmpty(meterQdQids[key].StandartTime))
        //            entity.REAL_TEST_TIME = (Convert.ToSingle(meterQdQids[key].StandartTime) * 60).ToString("F0");
        //        entity.LOAD_CURRENT = GetPCode("meterTestCurLoad", meterQdQids[key].Current);
        //        sql.Add(entity.ToInsertString());
        //        index++;
        //    }
        //    return sql.ToArray();
        //}

        ///// <summary>
        ///// 校核常数(走字)
        ///// </summary>
        //private string[] GetMT_CONST_MET_CONCByMt(MT_METER mtMeter, TestMeterInfo meter)
        //{
        //    List<string> sql = new List<string>();
        //    int index = 0;
        //    foreach (string key in meter.MeterZZErrors.Keys)
        //    {
        //        MeterZZError meterError = meter.MeterZZErrors[key];
        //        string current = GetPCode("meterTestCurLoad", meterError.IbXString);
        //        string[] levs = Core.Function.Number.GetDj(meter.MD_Grane);

        //        MT_CONST_MET_CONC entity = new MT_CONST_MET_CONC
        //        {
        //            DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO,
        //            EQUIP_CATEG = "01",
        //            SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO,
        //            DETECT_EQUIP_NO = meter.MD_DeviceID,
        //            DETECT_UNIT_NO = string.Empty,
        //            POSITION_NO = meter.MD_Epitope.ToString(),
        //            BAR_CODE = mtMeter.BAR_CODE,
        //            DETECT_DATE = meter.VerifyDate.ToString(),
        //            DETECT_ITEM_POINT = (index + 1).ToString(),
        //            PARA_INDEX = (index + 1).ToString(),
        //            CONC_CODE = meterError.Result.Trim() == Const.合格 ? "01" : "02",
        //            WRITE_DATE = DateTime.Now.ToString(),
        //            HANDLE_FLAG = "0",

        //            IS_VALID = "1",
        //            VOLT = GetPCode("meterTestVolt", "100%Un"),
        //            LOAD_CURRENT = current,
        //            DIFF_READING = meterError.WarkPower.ToString().Trim(),
        //            PF = GetPCode("meterTestPowerFactor", meterError.GLYS.Trim()),
        //            FEE_RATIO = meterError.Fl.Trim(),
        //            START_READING = meterError.PowerStart.ToString(),
        //            END_READING = meterError.PowerEnd.ToString(),
        //            ERROR = meterError.PowerError.Trim(),
        //            STANDARD_READING = (Convert.ToSingle(meterError.STMEnergy) / meter.GetBcs()[0]).ToString("F4"),
        //            REAL_PULES = meterError.Pules.Trim(),
        //            QUALIFIED_PULES = meterError.STMEnergy.Trim(),
        //            ERR_UP = (Convert.ToDouble(levs[0]) * 1.0).ToString("0.0"),
        //            ERR_DOWN = (Convert.ToDouble(levs[0]) * (-1.0)).ToString("0.0"),
        //            CONST_ERR = meterError.PowerError.Trim(),
        //            READ_TYPE_CODE = "",
        //        };

        //        //PowerError STMEnergy
        //        sql.Add(entity.ToInsertString());

        //        entity.CONC_CODE = meterError.Result.Trim() == Const.合格 ? "01" : "02";
        //        entity.BOTH_WAY_POWER_FLAG = GetPCode("powerFlag", meterError.PowerWay);
        //        entity.ERROR = meterError.PowerError.Trim();
        //        entity.STANDARD_READING = (Convert.ToSingle(meterError.STMEnergy) / meter.GetBcs()[0]).ToString("F4");
        //        entity.REAL_PULES = meterError.Pules.Trim();
        //        entity.QUALIFIED_PULES = meterError.STMEnergy.Trim();
        //        entity.ERR_UP = "2";
        //        entity.ERR_DOWN = "-2";
        //        entity.CONST_ERR = meterError.PowerError.Trim();
        //        entity.CONTROL_METHOD = GetPCode("meterTestCtrlMode", meterError.TestWay);
        //        sql.Add(entity.ToInsertString());
        //    }
        //    return sql.ToArray();
        //}

        ///// <summary>
        ///// 日计时
        ///// </summary>
        //private string GetMT_DAYERR_MET_CONCByMt(MT_METER mtMeter, TestMeterInfo meter)
        //{
        //    string error = "", ave = "", hz = "", result = "";

        //    string ItemKey = Cus_DgnItem.由电源供电的时钟试验;

        //    //平均值
        //    string key = ItemKey + "01";
        //    if (meter.MeterDgns.ContainsKey(key))
        //    {
        //        string[] v = meter.MeterDgns[key].Value.Split('|');
        //        if (v.Length > 1)
        //        {
        //            ave = v[0];
        //            hz = v[1];
        //        }
        //    }

        //    //前五次
        //    key = ItemKey + "02";
        //    if (meter.MeterDgns.ContainsKey(key))
        //        error = meter.MeterDgns[key].Value;


        //    ItemKey = Cus_DgnItem.由电源供电的时钟试验;
        //    if (meter.MeterDgns.ContainsKey(ItemKey))
        //        result = meter.MeterDgns[ItemKey].Result == Const.合格 ? "01" : "02";

        //    MT_DAYERR_MET_CONC entity = new MT_DAYERR_MET_CONC
        //    {
        //        DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO,
        //        EQUIP_CATEG = "01",
        //        SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO,
        //        DETECT_EQUIP_NO = meter.MD_DeviceID,
        //        DETECT_UNIT_NO = string.Empty,
        //        POSITION_NO = meter.MD_Epitope.ToString(),
        //        BAR_CODE = mtMeter.BAR_CODE,
        //        DETECT_DATE = meter.VerifyDate.ToString(),
        //        PARA_INDEX = "1",
        //        DETECT_ITEM_POINT = "01",
        //        IS_VALID = "1",
        //        SEC_PILES = "1",      //ProtocolInfo额外。
        //        TEST_TIME = "120",
        //        SIMPLING = "5",
        //        ERROR = error,
        //        AVG_ERR = ave,
        //        INT_CONVERT_ERR = hz,
        //        ERR_ABS = "0.5",
        //        CONC_CODE = result,
        //        WRITE_DATE = DateTime.Now.ToString(),
        //        HANDLE_FLAG = "0"
        //    };
        //    return entity.ToInsertString();
        //}
        //// <summary>
        ///// 密钥更新
        ///// </summary>
        //private string GetMT_ESAM_MET_CONCByMt(MT_METER mtMeter, TestMeterInfo meter)
        //{
        //    MT_ESAM_MET_CONC entity = new MT_ESAM_MET_CONC
        //    {
        //        DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO,
        //        EQUIP_CATEG = "01",
        //        SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO,
        //        DETECT_EQUIP_NO = meter.MD_DeviceID,
        //        DETECT_UNIT_NO = string.Empty,
        //        POSITION_NO = meter.MD_Epitope.ToString(),
        //        BAR_CODE = mtMeter.BAR_CODE,
        //        DETECT_DATE = meter.VerifyDate.ToString(),
        //        PARA_INDEX = "01",
        //        DETECT_ITEM_POINT = "01",
        //        IS_VALID = "1",
        //        LOAD_VOLTAGE = GetPCode("meterTestVolt", "100%Un"),//
        //        KEY_NUM = "3",//密钥条数
        //        KEY_VER = "04",//密钥版本
        //        KEY_STATUS = GetPCode("secretKeyStatus", "正式密钥"), //密钥状态
        //        KEY_TYPE = GetPCode("secretKeyType", "身份认证密钥"), //密钥类型
        //        CONC_CODE = MisDataHelper.GetFkConclusion(meter, Cus_CostControlItem.密钥更新),
        //        WRITE_DATE = DateTime.Now.ToString(),
        //        HANDLE_FLAG = "0"
        //    };
        //    return entity.ToInsertString();
        //}

        ///// <summary>
        ///// 身份认证
        ///// </summary>
        //private string GetMT_ESAM_SECURITY_MET_CONCByMt(MT_METER mtMeter, TestMeterInfo meter)
        //{
        //    MT_ESAM_SECURITY_MET_CONC entity = new MT_ESAM_SECURITY_MET_CONC
        //    {
        //        DETECT_TASK_NO = mtMeter.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO,
        //        EQUIP_CATEG = "01",
        //        SYS_NO = mtMeter.MT_DATECT_OUT_EQUIP.SYS_NO,
        //        DETECT_EQUIP_NO = meter.MD_DeviceID,
        //        DETECT_UNIT_NO = string.Empty,
        //        POSITION_NO = meter.MD_Epitope.ToString(),
        //        BAR_CODE = mtMeter.BAR_CODE,
        //        DETECT_DATE = meter.VerifyDate.ToString(),
        //        PARA_INDEX = "01",
        //        DETECT_ITEM_POINT = "01",
        //        IS_VALID = "1",
        //        ESAM_ID = MisDataHelper.GetFkValue(meter, Cus_CostControlItem.身份认证),//
        //        CONC_CODE = MisDataHelper.GetFkConclusion(meter, Cus_CostControlItem.身份认证),
        //        WRITE_DATE = DateTime.Now.ToString(),
        //        HANDLE_FLAG = "0"
        //    };

        //    return entity.ToInsertString();
        //}
        //#endregion

        #region 方法

        /// <summary>
        /// 从生产调度系统数据库根据代码获取代码值
        /// </summary>
        /// <param name="codeType"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetPCodeDic(string codeType)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string sql = string.Format(@"select * from mt_p_code where code_type ='{0}'", codeType);
            DataTable dr = ExecuteReader(sql);
            foreach (DataRow r in dr.Rows)
            {
                string value = r["value"].ToString().Trim();
                string name = r["name"].ToString().Trim();
                if (value.Length > 0)
                {
                    if (!dic.ContainsKey(value))
                        dic.Add(value, name);
                }
            }
            return dic;
        }


        /// <summary>
        /// 此函数可以添加字典，不可以修改或删除字典
        /// </summary>
        private void GetDicPCodeTable()
        {
            //获取MIS字典表信息
            //功率方向
            PCodeTable.Add("powerFlag", GetPCodeDic("powerFlag"));
            //电流相别
            PCodeTable.Add("currentPhaseCode", GetPCodeDic("currentPhaseCode"));
            //电流
            PCodeTable.Add("meterTestCurLoad", GetPCodeDic("meterTestCurLoad"));
            //功率因数
            PCodeTable.Add("itRatedLoadPf", GetPCodeDic("itRatedLoadPf"));
            //功率因数
            PCodeTable.Add("meterTestPowerFactor", GetPCodeDic("meterTestPowerFactor"));
            ////试验电压
            //PCodeTable.Add("meter_Test_Volt", GetPCodeDic("meter_Test_Volt"));
            //试验电压
            PCodeTable.Add("meterTestVolt", GetPCodeDic("meterTestVolt"));
            //额定电压
            PCodeTable.Add("meterVolt", GetPCodeDic("meterVolt"));
            //额定电流
            PCodeTable.Add("meterRcSort", GetPCodeDic("meterRcSort"));

            //费率
            PCodeTable.Add("tari_ff", GetPCodeDic("tari_ff"));
            //等级
            PCodeTable.Add("meterAccuracy", GetPCodeDic("meterAccuracy"));
            //电表类型
            PCodeTable.Add("meterTypeCode", GetPCodeDic("meterTypeCode"));
            //电表常数
            PCodeTable.Add("meterConstCode", GetPCodeDic("meterConstCode"));
            //接线方式
            PCodeTable.Add("wiringMode", GetPCodeDic("wiringMode"));
            //电表型号
            PCodeTable.Add("meterModelNo", GetPCodeDic("meterModelNo"));
            //厂家
            PCodeTable.Add("meterFacturer", GetPCodeDic("meterFacturer"));
            //频率1
            PCodeTable.Add("meterTestFreq", GetPCodeDic("meterTestFreq"));
            //密钥状态
            PCodeTable.Add("secretKeyStatus", GetPCodeDic("secretKeyStatus"));
            //密钥类型
            PCodeTable.Add("secretKeyType", GetPCodeDic("secretKeyType"));
            //走字方法
            PCodeTable.Add("meterTestCtrlMode", GetPCodeDic("meterTestCtrlMode"));
            //载波模块
            PCodeTable.Add("LocalChipManu", GetPCodeDic("LocalChipManu"));

            //电能表通讯规约
            PCodeTable.Add("commProtocol", GetPCodeDic("commProtocol"));
            PCodeTable.Add("conMode", GetPCodeDic("conMode"));
            PCodeTable.Add("DET_TYPE", GetPCodeDic("DET_TYPE"));
            PCodeTable.Add("equip_fee_rate", GetPCodeDic("equip_fee_rate"));
            PCodeTable.Add("tripMode", GetPCodeDic("tripMode"));



        }

        private MT_METER GetMt_meter(string MD_BarCode)
        {
            string strSysNo = "";
            string strDetectTaskNo = "";
            string strSysEquipType = "";

            //根据条码号取出工单号 根据条码号取出工单号
            string sql = "SELECT DETECT_TASK_NO FROM MT_DETECT_OUT_EQUIP WHERE BAR_CODE='" + MD_BarCode + "'order by  write_date desc";
            object o = ExecuteScalar(sql);
            if (o == null)
            {
                Utility.Log.LogManager.AddMessage("不存在条码号为(" + MD_BarCode + ")的工单记录", Utility.Log.EnumLogSource.检定业务日志, Utility.Log.EnumLevel.Warning);
                //("不存在条码号为(" + MD_BarCode + ")的工单记录");
                return null;
            }

            string detetTaskNo = o.ToString().Trim();

            //根据任务号查询 系统编号 和设备类型
            //sql = string.Format(@"SELECT * FROM MT_DETECT_TASK T WHERE T.TASK_STATUS='01' AND T.DETECT_TASK_NO ='{0}'", detetTaskNo);

            sql = string.Format(@"SELECT * FROM MT_DETECT_TASK T WHERE  T.DETECT_TASK_NO ='{0}'", detetTaskNo);

            DataTable dr = ExecuteReader(sql);
            if (dr.Rows.Count > 0)
            {
                strSysNo = dr.Rows[0]["SYS_NO"].ToString().Trim();     //系统编号
                //strSysEquipType = dr.Rows[0]["EQUIP_TYPE"].ToString().Trim();
                strDetectTaskNo = dr.Rows[0]["DETECT_TASK_NO"].ToString().Trim();    //任务编号
            }
            else
            {
                Utility.Log.LogManager.AddMessage("不存在任务号为(" + detetTaskNo + ")的工单记录", Utility.Log.EnumLogSource.检定业务日志, Utility.Log.EnumLevel.Warning);

                //App.Message.Show(false, "不存在任务号为(" + detetTaskNo + ")的工单记录");
                return null;
            }

            sql = "select * from mt_meter where bar_code='" + MD_BarCode.Trim() + "'";

            DataTable dr1 = ExecuteReader(sql);
            //if (dr1.Rows.Count > 0) return null;
            DataRow row = dr1.Rows[0];

            MT_METER model = new MT_METER();

            if (row["METER_ID"].ToString() != "")
            {
                model.METER_ID = row["METER_ID"].ToString();
            }
            model.BAR_CODE = row["BAR_CODE"].ToString();
            model.LOT_NO = row["LOT_NO"].ToString();
            model.ASSET_NO = row["ASSET_NO"].ToString();
            model.MADE_NO = row["MADE_NO"].ToString();
            model.SORT_CODE = row["SORT_CODE"].ToString();
            model.TYPE_CODE = row["TYPE_CODE"].ToString();
            model.MODEL_CODE = row["MODEL_CODE"].ToString();
            model.WIRING_MODE = row["WIRING_MODE"].ToString();
            model.VOLT_CODE = row["VOLT_CODE"].ToString();
            model.OVERLOAD_FACTOR = row["OVERLOAD_FACTOR"].ToString();
            model.AP_PRE_LEVEL_CODE = row["AP_PRE_LEVEL_CODE"].ToString();
            model.CONST_CODE = row["CONST_CODE"].ToString();
            model.RP_CONSTANT = row["RP_CONSTANT"].ToString();
            model.PULSE_CONSTANT_CODE = row["PULSE_CONSTANT_CODE"].ToString();
            model.FREQ_CODE = row["FREQ_CODE"].ToString();
            model.RATED_CURRENT = row["RATED_CURRENT"].ToString();
            model.CON_MODE = row["CON_MODE"].ToString();
            model.SOFT_VER = row["SOFT_VER"].ToString();
            model.HARD_VER = row["HARD_VER"].ToString();
            model.MT_DATECT_OUT_EQUIP.SYS_NO = strSysNo;
            model.MT_DATECT_OUT_EQUIP.DETECT_TASK_NO = strDetectTaskNo;

            return model;

        }
        private PowerWay GetGLFXFromString(string strValue)
        {
            PowerWay fx = PowerWay.正向有功;
            switch (strValue)
            {
                case "正向有功":
                    fx = PowerWay.正向有功;
                    break;
                case "正向无功":
                    fx = PowerWay.正向无功;
                    break;
                case "反向有功":
                    fx = PowerWay.反向有功;
                    break;
                case "反向无功":
                    fx = PowerWay.反向无功;
                    break;
                case "第一象限无功":
                    fx = PowerWay.第一象限无功;
                    break;
                case "第二象限无功":
                    fx = PowerWay.第二象限无功;
                    break;
                case "第三象限无功":
                    fx = PowerWay.第三象限无功;
                    break;
                case "第四象限无功":
                    fx = PowerWay.第四象限无功;
                    break;
                default:
                    fx = PowerWay.正向有功;
                    break;
            }
            return fx;
        }

        private Cus_PowerYuanJian GetYuanJianFromString(string value)
        {
            Cus_PowerYuanJian yj = Cus_PowerYuanJian.H;
            switch (value)
            {
                case "ABC":
                case "AC":
                    yj = Cus_PowerYuanJian.H;
                    break;
                case "A":
                    yj = Cus_PowerYuanJian.A;
                    break;
                case "B":
                    yj = Cus_PowerYuanJian.B;
                    break;
                case "C":
                    yj = Cus_PowerYuanJian.C;
                    break;
            }
            return yj;
        }

        public bool DownTask(string MD_BarCode, ref MT_DETECT_OUT_EQUIP mT_DETECT_TASK)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCompleted(string DETECT_TASK_NO, string SYS_NO)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
