using LYZD.Core.Enum;
using LYZD.Core.Model.DnbModel.DnbInfo;
using LYZD.Core.Model.Meter;
using LYZD.Core.Model.Schema;
using LYZD.DAL.Config;
using LYZD.Mis.Common;
using LYZD.Mis.DataHelper;
using LYZD.Mis.MDS.Table;
using LYZD.Mis.NanRui.LRDataTable;
using LYZD.Utility.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LYZD.Mis.NanRui
{
    public class NanRui : OracleHelper, IMis
    {
        /// <summary>
        /// 任务单号
        /// </summary>
        private string TaskNO = "";
        /// <summary>
        /// 成功上传数量
        /// </summary>
        private int SusessCount = 0;//
        public static Dictionary<string, Dictionary<string, string>> PCodeTable = new Dictionary<string, Dictionary<string, string>>();
        public NanRui(string ip, int port, string dataSource, string userId, string pwd, string url)
        {
            this.Ip = ip;
            this.Port = port;
            this.DataSource = dataSource;
            this.UserId = userId;
            this.Password = pwd;
            this.WebServiceURL = url;
        }
        public bool Down(string MD_BarCode, ref TestMeterInfo meter)
        {
            if (string.IsNullOrEmpty(MD_BarCode)) return false;
            try
            {
                if (PCodeTable.Count <= 0)
                    GetDicPCodeTable();

                string sql = string.Format(@"select TYPE_CODE as 终端类型,MANUFACTURER as 采集终端厂商,COLL_MODE as 采集终端通信类型 ,WIRING_MODE as 接线方式,VOLT_CODE as 电压，RATED_CURRENT as 电流，AP_PRE_LEVEL_CODE as 有功等级 ,RP_PRE_LEVEL_CODE as 无功等级,CONST_CODE as 常数 from MT_LC_EQUIP where BAR_CODE='{0}'", MD_BarCode.Trim());
                //string sql = string.Format(@"select * from MT_LC_EQUIP where BAR_CODE='1430009000004303001501'");
                DataTable dt = ExecuteReader(sql);
                if (dt.Rows.Count <= 0) return false;
                DataRow row = dt.Rows[0];

                //终端等级
                //string MeterLevel = string.Format(@"select * from mt_p_code where value='{0}' AND code_type ='tmnlRpLevel'", row["无功等级"].ToString().Trim());
                //DataTable MeterLeveldt = ExecuteReader(MeterLevel);
                //meter.MD_Grane = row["有功等级"].ToString().Trim() + "(" + Convert.ToDouble(MeterLeveldt.Rows[0]["NAME"].ToString()) + ")";

                string An = string.Format(@"select * from mt_p_code where value='{0}' AND code_type ='meterRcSort'", row["电流"].ToString().Trim());
                DataTable Andt = ExecuteReader(An);
                if (Andt.Rows.Count>0)
                {
                    meter.MD_UA = Andt.Rows[0]["NAME"].ToString().Trim();
                }

                string Uv = string.Format(@"select * from mt_p_code where value='{0}' AND code_type ='meterVolt'", row["电压"].ToString().Trim());
                DataTable MeterUv = ExecuteReader(Uv);
                if (MeterUv.Rows.Count > 0)
                {
                    meter.MD_UB = float.Parse(MeterUv.Rows[0]["NAME"].ToString().Remove(0, 2).Split('V')[0]);
                }

                string MD_Factory = string.Format(@"select * from mt_p_code where value='{0}'AND code_type ='meterFacturer'", row["采集终端厂商"].ToString().Trim());
                DataTable MD_Factorydt = ExecuteReader(MD_Factory);
                if (MD_Factorydt.Rows.Count > 0)
                {
                    meter.MD_Factory = MD_Factorydt.Rows[0]["NAME"].ToString().Trim();
                }


                string MD_WiringMode = string.Format(@"select * from mt_p_code where value='{0}'AND code_type ='wiringMode'", row["接线方式"].ToString().Trim());
                DataTable MD_WiringModedt = ExecuteReader(MD_WiringMode);
                if (MD_WiringModedt.Rows.Count > 0)
                {
                    meter.MD_WiringMode = MD_WiringModedt.Rows[0]["NAME"].ToString().Trim();
                }

                //string MD_TerminalType = string.Format(@"select * from mt_p_code where value='{0}'AND code_type ='collTmnlType'", row["终端类型"].ToString().Trim());
                //DataTable MD_TerminalTypeDt = ExecuteReader(MD_TerminalType);
                //if (MD_TerminalTypeDt.Rows[0]["NAME"].ToString().Trim().Contains("集中器"))
                //{
                //    meter.MD_TerminalType = "集中器I型22版";
                //}
                //else
                //{
                //    meter.MD_TerminalType = "专变III型22版";
                //}
                //select * from p_code where value='16' AND code_type ='meterConstCode'
                string MD_Constant = string.Format(@"select * from mt_p_code where value='{0}' AND code_type ='meterConstCode'", row["常数"].ToString().Trim());
                DataTable MD_ConstantDt = ExecuteReader(MD_Constant);
                if (MD_ConstantDt.Rows.Count > 0)
                {
                    meter.MD_Constant = MD_ConstantDt.Rows[0]["NAME"].ToString().Trim() + "(" + MD_ConstantDt.Rows[0]["NAME"].ToString().Trim() + ")";
                }
                //检定任务单，申请编号
                //LogManager.AddMessage("meter.MD_UB" + meter.MD_UB + "、" + "meter.MD_Factory" + meter.MD_Factory + "、" + "meter.MD_TerminalType" + meter.MD_TerminalType + "、" + "meter.MD_Constant" + meter.MD_Constant + "、", EnumLogSource.服务器日志, EnumLevel.Information);
                //meter.Type = GetPName("meterTypeCode", row["TYPE_CODE"]);

                return true;
            }
            catch (Exception ex)
            {
                //LogManager.AddMessage("meter.MD_UB" + meter.MD_UB + "、" + "meter.MD_Factory" + meter.MD_Factory + "、" + "meter.MD_TerminalType" + meter.MD_TerminalType + "、" + "meter.MD_Constant" + meter.MD_Constant + "、"+"\n"+ex.ToString(), EnumLogSource.服务器日志, EnumLevel.Warning);
                LogManager.AddMessage(ex.ToString(), EnumLogSource.服务器日志, EnumLevel.Warning);
                return false;
            }
            
        }

        public void CheckOrderNo(TestMeterInfo[] meterInfo, string OrderNo,out List<int> DefeatedNumber,string EquipmentNo)
        {
            DefeatedNumber = new List<int>();
            //Utility.Log.LogManager.AddMessage("开始读取数据", Utility.Log.EnumLogSource.设备操作日志, Utility.Log.EnumLevel.TipsError);
            //获取到中间库中下达给本台体中任务单号为OrderNo的所有数据
            string sql = string.Format(@"SELECT * FROM MT_DETECT_OUT_EQUIP WHERE DETECT_TASK_NO='{0}' AND SYS_NO='{1}' ORDER BY WRITE_DATE DESC", OrderNo, EquipmentNo);
            //Utility.Log.LogManager.AddMessage(sql, Utility.Log.EnumLogSource.设备操作日志, Utility.Log.EnumLevel.TipsError);
            DataTable dt = ExecuteReader(sql);
            //Utility.Log.LogManager.AddMessage("读取完成，开始验证", Utility.Log.EnumLogSource.设备操作日志, Utility.Log.EnumLevel.TipsError);
            //遍历要检表
            foreach (var item in meterInfo)
            {
                if (item.YaoJianYn) {
                    bool isok = false;
                    //遍历查出来的数据，有符合的就中断，没有的就记录下来
                    foreach (DataRow items in dt.Rows)
                    {
                        if(item.MD_BarCode == items["BAR_CODE"].ToString().Trim())
                        {
                            //Utility.Log.LogManager.AddMessage(item.MD_BarCode, Utility.Log.EnumLogSource.设备操作日志, Utility.Log.EnumLevel.TipsError);
                            isok = true;
                            break;
                        }
                    }
                    //上面遍历完数据表之后还没有就添加到list
                    if (isok == false)
                    {
                        DefeatedNumber.Add(item.MD_Epitope);
                    }
                    
                }
            }
        }

        /// <summary>
        /// 获取任务编号
        /// </summary>
        /// <param name="SYS_NO"></param>
        /// <param name="DETECT_TASK_NO"></param>
        /// <returns></returns>
        public bool DownTask(string MD_BarCode, ref MT_DETECT_OUT_EQUIP mT_DETECT_TASK)
        {
            //DETECT_TASK_NO = "";
            //string sql = string.Format(@"select TYPE_CODE * from MT_DETECT_TASK where SYS_NO='{0}'", SYS_NO.Trim());
            string sql = string.Format(@"select * from Mt_Detect_Out_Equip where BAR_CODE='{0}' ORDER BY WRITE_DATE DESC", MD_BarCode);
            DataTable dt = ExecuteReader(sql);
            if (dt.Rows.Count <= 0) return false;
            DataRow row = dt.Rows[0];
            mT_DETECT_TASK = new LRDataTable.MT_DETECT_OUT_EQUIP();
            mT_DETECT_TASK.DETECT_TASK_NO = row["DETECT_TASK_NO"].ToString();
            mT_DETECT_TASK.SYS_NO = row["SYS_NO"].ToString();
            mT_DETECT_TASK.IO_TASK_NO = row["IO_TASK_NO"].ToString();
            mT_DETECT_TASK.ARRIVE_BATCH_NO = row["ARRIVE_BATCH_NO"].ToString();
            mT_DETECT_TASK.EQUIP_CATEG = row["EQUIP_CATEG"].ToString();
            mT_DETECT_TASK.BAR_CODE = row["BAR_CODE"].ToString();
            mT_DETECT_TASK.BOX_BAR_CODE = row["BOX_BAR_CODE"].ToString();
            mT_DETECT_TASK.PILE_NO = row["PILE_NO"].ToString();
            mT_DETECT_TASK.REDETECT_FLAG = row["REDETECT_FLAG"].ToString();
            mT_DETECT_TASK.PLATFORM_NO = row["PLATFORM_NO"].ToString();
            mT_DETECT_TASK.EMP_NO = row["EMP_NO"].ToString();
            mT_DETECT_TASK.PLATFORM_TYPE = row["PLATFORM_TYPE"].ToString();
            mT_DETECT_TASK.WRITE_DATE = row["WRITE_DATE"].ToString();
            mT_DETECT_TASK.HANDLE_FLAG = row["HANDLE_FLAG"].ToString();
            mT_DETECT_TASK.HANDLE_DATE = row["HANDLE_DATE"].ToString();
            return true;
        }


        public bool SchemeDown(string barcode, out string schemeName, out Dictionary<string, SchemaNode> Schema)
        {
            schemeName = "获取失败";
            Schema = new Dictionary<string, SchemaNode>();
            return false;
        }

        public void ShowPanel(Control panel)
        {
            return;
        }

        private static string DETECT_TASK_NO=null;

        public bool Update(TestMeterInfo meter)
        {
            #region 数据插入中间库
            Dictionary<string, MeterResoultData> meterresoultdata = meter.MeterResoultData;

            List<string> SQLS = new List<string>();

            

            if (!string.IsNullOrEmpty(meter.MD_TaskNo)&& !string.IsNullOrEmpty(meter.MD_BarCode)) {
                string deletSqlMT_DETECT_TMNL_RSLT = string.Format("delete from sxykjd.MT_DETECT_TMNL_RSLT where DETECT_TASK_NO='{0}' and bar_code='{1}'", meter.MD_TaskNo, meter.MD_BarCode);

                ExecuteOne(deletSqlMT_DETECT_TMNL_RSLT);

                string deleteSqlMT_DETECT_ITEM_RSLT = string.Format("delete from sxykjd.MT_DETECT_ITEM_RSLT where DETECT_TASK_NO='{0}' and bar_code='{1}'", meter.MD_TaskNo, meter.MD_BarCode);

                ExecuteOne(deleteSqlMT_DETECT_ITEM_RSLT);

                string deleteSqlMT_DETECT_SUBITEM_RSLT = string.Format("delete from sxykjd.MT_DETECT_SUBITEM_RSLT where DETECT_TASK_NO='{0}' and bar_code='{1}'", meter.MD_TaskNo, meter.MD_BarCode);

                ExecuteOne(deleteSqlMT_DETECT_SUBITEM_RSLT);
            }
            else
            {
                return false;
            }
            

            MT_DETECT_TMNL_RSLT mT_DETECT_TMNL_RSLT = new MT_DETECT_TMNL_RSLT();
            mT_DETECT_TMNL_RSLT.DETECT_TASK_NO = meter.MD_TaskNo;
            DETECT_TASK_NO = mT_DETECT_TMNL_RSLT.DETECT_TASK_NO;
            mT_DETECT_TMNL_RSLT.SYS_NO = meter.MD_DeviceID;
            mT_DETECT_TMNL_RSLT.DETECT_EQUIP_NO = meter.MD_DeviceID;
           // mT_DETECT_TMNL_RSLT.LINE_NO = meter.MD_DeviceID;
            mT_DETECT_TMNL_RSLT.POSITION_NO = meter.MD_Epitope.ToString();
            mT_DETECT_TMNL_RSLT.BAR_CODE = meter.MD_BarCode;
            mT_DETECT_TMNL_RSLT.DETECT_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            mT_DETECT_TMNL_RSLT.CONC_CODE = meter.Result == "合格" ? "01" : "02";
            mT_DETECT_TMNL_RSLT.TEMP = meter.Temperature;
            mT_DETECT_TMNL_RSLT.HUMIDITY = meter.Humidity;
            mT_DETECT_TMNL_RSLT.DETECT_PERSON = meter.Checker1;
            mT_DETECT_TMNL_RSLT.AUDIT_PERSON = meter.Checker2;
            mT_DETECT_TMNL_RSLT.WRITE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mmm:ss");
            //这四个暂时不写，
            //mT_DETECT_TMNL_RSLT.SEAL_HANDLE_FLAG = "";
            //mT_DETECT_TMNL_RSLT.SEAL_HANDLE_DATE = "";
            //mT_DETECT_TMNL_RSLT.HANDLE_FLAG = "0";
            //mT_DETECT_TMNL_RSLT.HANDLE_DATE = "";
            //SQLS.Add(mT_DETECT_TMNL_RSLT);
            SQLS.Add(mT_DETECT_TMNL_RSLT.ToInsertString());

            foreach (var item in meterresoultdata.Keys)
            {
                MT_DETECT_ITEM_RSLT mT_DETECT_ITEM_RSLT = new MT_DETECT_ITEM_RSLT();

                mT_DETECT_ITEM_RSLT.DETECT_TASK_NO = meter.MD_TaskNo;
                mT_DETECT_ITEM_RSLT.SYS_NO = meter.MD_DeviceID;
                mT_DETECT_ITEM_RSLT.DETECT_EQUIP_NO = meter.MD_DeviceID;
                mT_DETECT_ITEM_RSLT.POSITION_NO = meter.MD_Epitope.ToString();
                mT_DETECT_ITEM_RSLT.BAR_CODE = meter.MD_BarCode;
                mT_DETECT_ITEM_RSLT.DETECT_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                mT_DETECT_ITEM_RSLT.ITEM_NO = GetItemId( meterresoultdata[item].meterResoults[0].Datas["项目名"]).Split(',')[1];
                mT_DETECT_ITEM_RSLT.ITEM_NAME = GetItemId(meterresoultdata[item].meterResoults[0].Datas["项目名"]).Split(',')[0];
                mT_DETECT_ITEM_RSLT.ITEM_CONC = meterresoultdata[item].meterResoults[0].Result == "合格" ? "01" : "02";

                mT_DETECT_ITEM_RSLT.DETECT_PERSON = meter.Checker1;
                mT_DETECT_ITEM_RSLT.AUDIT_PERSON = meter.Checker2;

                mT_DETECT_ITEM_RSLT.WRITE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                SQLS.Add(mT_DETECT_ITEM_RSLT.ToInsertString());
                int Index = 0;
                foreach (var Subitem in meterresoultdata[item].meterResoults[0].ItemDatas)
                {
                    MT_DETECT_SUBITEM_RSLT mT_DETECT_SUBITEM_RSLT = new MT_DETECT_SUBITEM_RSLT();
                    
                    if (!mT_DETECT_SUBITEM_RSLT.SubitemDic.Keys.Contains(Subitem.Name))
                    {
                        Index++;
                        mT_DETECT_SUBITEM_RSLT.SubitemDic.Add(Subitem.Name, Index);
                    }

                    mT_DETECT_SUBITEM_RSLT.DETECT_TASK_NO = meter.MD_TaskNo;
                    mT_DETECT_SUBITEM_RSLT.SYS_NO = meter.MD_DeviceID;
                    mT_DETECT_SUBITEM_RSLT.DETECT_EQUIP_NO = meter.MD_DeviceID;
                    mT_DETECT_SUBITEM_RSLT.DETECT_UNIT_NO = meter.MD_DeviceID;
                    mT_DETECT_SUBITEM_RSLT.POSITION_NO = meter.MD_Epitope.ToString();
                    mT_DETECT_SUBITEM_RSLT.BAR_CODE = meter.MD_BarCode;
                    mT_DETECT_SUBITEM_RSLT.DETECT_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    mT_DETECT_SUBITEM_RSLT.ITEM_NO = GetItemId(meterresoultdata[item].meterResoults[0].Datas["项目名"]).Split(',')[1];
                    mT_DETECT_SUBITEM_RSLT.SUBITEM_NO = mT_DETECT_SUBITEM_RSLT.SubitemDic[Subitem.Name].ToString();
                    mT_DETECT_SUBITEM_RSLT.SUBITEM_NAME = Subitem.Name;
                    mT_DETECT_SUBITEM_RSLT.DATA1 = Subitem.StandardData;
                    mT_DETECT_SUBITEM_RSLT.DATA2 = Subitem.TerminalData;
                    mT_DETECT_SUBITEM_RSLT.SUBITEM_CONC = Subitem.Resoult == "合格" ? "01" : "02";
                    mT_DETECT_SUBITEM_RSLT.DETECT_DATE = Subitem.Time;
                    SQLS.Add(mT_DETECT_SUBITEM_RSLT.ToInsertString());

                }

            }

            Execute(SQLS);

            #endregion

            return true;
           
        }

        public bool Update(List<TestMeterInfo>meters)
        {
            try
            {
                #region 删除历史数据
                List<string> DeletSQL = new List<string>();
                foreach (var item in meters)
                {
                    if (!string.IsNullOrEmpty(item.MD_TaskNo) && !string.IsNullOrEmpty(item.MD_BarCode))
                    {
                        string deletSqlMT_DETECT_TMNL_RSLT = string.Format("delete from MT_DETECT_TMNL_RSLT where DETECT_TASK_NO='{0}' and bar_code='{1}'", item.MD_TaskNo, item.MD_BarCode);

                        DeletSQL.Add(deletSqlMT_DETECT_TMNL_RSLT);
                        //ExecuteOne(deletSqlMT_DETECT_TMNL_RSLT);

                        string deleteSqlMT_DETECT_ITEM_RSLT = string.Format("delete from MT_DETECT_ITEM_RSLT where DETECT_TASK_NO='{0}' and bar_code='{1}'", item.MD_TaskNo, item.MD_BarCode);
                        DeletSQL.Add(deleteSqlMT_DETECT_ITEM_RSLT);
                        //ExecuteOne(deleteSqlMT_DETECT_ITEM_RSLT);

                        string deleteSqlMT_DETECT_SUBITEM_RSLT = string.Format("delete from MT_DETECT_SUBITEM_RSLT where DETECT_TASK_NO='{0}' and bar_code='{1}'", item.MD_TaskNo, item.MD_BarCode);
                        DeletSQL.Add(deleteSqlMT_DETECT_SUBITEM_RSLT);

                        string deletSqlMT_SEAL_INST = string.Format("delete from MT_SEAL_INST where SEAL_BAR_CODE='{0}' and bar_code='{1}'", item.Other4, item.MD_BarCode);
                        DeletSQL.Add(deletSqlMT_SEAL_INST);
                        //ExecuteOne(deleteSqlMT_DETECT_SUBITEM_RSLT);
                    }
                    else
                    {
                        return false;
                    }
                }
                #endregion

                Execute(DeletSQL);
                Thread.Sleep(3000);

                DETECT_TASK_NO = meters[0].MD_TaskNo;

                #region 插入数据

                List<string> SQLS = new List<string>();
                foreach (var item in meters)
                {
                    MT_DETECT_TMNL_RSLT mT_DETECT_TMNL_RSLT = new MT_DETECT_TMNL_RSLT();
                    mT_DETECT_TMNL_RSLT.DETECT_TASK_NO = item.MD_TaskNo;
                    mT_DETECT_TMNL_RSLT.SYS_NO = item.MD_DeviceID;
                    mT_DETECT_TMNL_RSLT.DETECT_EQUIP_NO = item.MD_DeviceID;
                    mT_DETECT_TMNL_RSLT.LINE_NO = item.MD_DeviceID;
                    mT_DETECT_TMNL_RSLT.POSITION_NO = item.MD_Epitope.ToString();
                    mT_DETECT_TMNL_RSLT.BAR_CODE = item.MD_BarCode;
                    mT_DETECT_TMNL_RSLT.DETECT_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    mT_DETECT_TMNL_RSLT.CONC_CODE = item.Result == "合格" ? "01" : "02";
                    mT_DETECT_TMNL_RSLT.TEMP = item.Temperature;
                    mT_DETECT_TMNL_RSLT.HUMIDITY = item.Humidity;
                    mT_DETECT_TMNL_RSLT.DETECT_PERSON = item.Checker1;
                    mT_DETECT_TMNL_RSLT.AUDIT_PERSON = item.Checker2;
                    mT_DETECT_TMNL_RSLT.WRITE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mmm:ss");
                    SQLS.Add(mT_DETECT_TMNL_RSLT.ToInsertString());

                    MT_SEAL_INST mT_SEAL_INST = new MT_SEAL_INST();
                    mT_SEAL_INST.BAR_CODE = item.MD_BarCode;
                    mT_SEAL_INST.DETECT_TASK_NO = item.MD_TaskNo;
                    mT_SEAL_INST.SYS_NO = "801";
                    mT_SEAL_INST.EQUIP_CATEG = "09";
                    mT_SEAL_INST.SEAL_POSITION = "01";
                    mT_SEAL_INST.SEAL_BAR_CODE = item.Other4;
                    mT_SEAL_INST.SEAL_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mmm:ss");
                    mT_SEAL_INST.SEALER_NO = item.Checker2;
                    mT_SEAL_INST.WRITE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mmm:ss");
                    SQLS.Add(mT_SEAL_INST.ToInsertString());

                    Dictionary<string, MeterResoultData> meterresoultdata = item.MeterResoultData;

                    foreach (var items in meterresoultdata.Keys)
                    {
                        MT_DETECT_ITEM_RSLT mT_DETECT_ITEM_RSLT = new MT_DETECT_ITEM_RSLT();

                        mT_DETECT_ITEM_RSLT.DETECT_TASK_NO = item.MD_TaskNo;
                        mT_DETECT_ITEM_RSLT.SYS_NO = "801";
                        mT_DETECT_ITEM_RSLT.DETECT_EQUIP_NO = item.MD_DeviceID;
                        mT_DETECT_ITEM_RSLT.DETECT_UNIT_NO = item.MD_DeviceID;
                        mT_DETECT_ITEM_RSLT.POSITION_NO = item.MD_Epitope.ToString();
                        mT_DETECT_ITEM_RSLT.BAR_CODE = item.MD_BarCode;
                        mT_DETECT_ITEM_RSLT.DETECT_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        mT_DETECT_ITEM_RSLT.ITEM_NO = GetItemId(meterresoultdata[items].meterResoults[0].Datas["项目名"]).Split(',')[1];
                        mT_DETECT_ITEM_RSLT.ITEM_NAME = GetItemId(meterresoultdata[items].meterResoults[0].Datas["项目名"]).Split(',')[0];
                        mT_DETECT_ITEM_RSLT.ITEM_CONC = meterresoultdata[items].meterResoults[0].Result == "合格" ? "01" : "02";

                        mT_DETECT_ITEM_RSLT.DETECT_PERSON = item.Checker1;
                        mT_DETECT_ITEM_RSLT.AUDIT_PERSON = item.Checker2;

                        mT_DETECT_ITEM_RSLT.WRITE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        SQLS.Add(mT_DETECT_ITEM_RSLT.ToInsertString());
                        int Index = 0;
                        foreach (var Subitem in meterresoultdata[items].meterResoults[0].ItemDatas)
                        {
                            MT_DETECT_SUBITEM_RSLT mT_DETECT_SUBITEM_RSLT = new MT_DETECT_SUBITEM_RSLT();

                            if (!mT_DETECT_SUBITEM_RSLT.SubitemDic.Keys.Contains(Subitem.Name))
                            {
                                Index++;
                                mT_DETECT_SUBITEM_RSLT.SubitemDic.Add(Subitem.Name, Index);
                            }

                            mT_DETECT_SUBITEM_RSLT.DETECT_TASK_NO = item.MD_TaskNo;
                            mT_DETECT_SUBITEM_RSLT.SYS_NO = "801";
                            mT_DETECT_SUBITEM_RSLT.DETECT_EQUIP_NO = item.MD_DeviceID;
                            mT_DETECT_SUBITEM_RSLT.DETECT_UNIT_NO = item.MD_DeviceID;
                            mT_DETECT_SUBITEM_RSLT.POSITION_NO = item.MD_Epitope.ToString();
                            mT_DETECT_SUBITEM_RSLT.BAR_CODE = item.MD_BarCode;
                            mT_DETECT_SUBITEM_RSLT.DETECT_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            mT_DETECT_SUBITEM_RSLT.ITEM_NO = GetItemId(meterresoultdata[items].meterResoults[0].Datas["项目名"]).Split(',')[1];
                            mT_DETECT_SUBITEM_RSLT.SUBITEM_NO = mT_DETECT_SUBITEM_RSLT.SubitemDic[Subitem.Name].ToString();
                            mT_DETECT_SUBITEM_RSLT.SUBITEM_NAME = Subitem.Name;
                            mT_DETECT_SUBITEM_RSLT.DATA1 = Subitem.StandardData;
                            mT_DETECT_SUBITEM_RSLT.DATA2 = Subitem.TerminalData;
                            mT_DETECT_SUBITEM_RSLT.SUBITEM_CONC = Subitem.Resoult == "合格" ? "01" : "02";
                            mT_DETECT_SUBITEM_RSLT.DETECT_DATE = Subitem.Time;
                            SQLS.Add(mT_DETECT_SUBITEM_RSLT.ToInsertString());

                        }

                    }

                }

                #endregion
                //这四个暂时不写，
                //mT_DETECT_TMNL_RSLT.SEAL_HANDLE_FLAG = "";
                //mT_DETECT_TMNL_RSLT.SEAL_HANDLE_DATE = "";
                //mT_DETECT_TMNL_RSLT.HANDLE_FLAG = "0";
                //mT_DETECT_TMNL_RSLT.HANDLE_DATE = "";
                //SQLS.Add(mT_DETECT_TMNL_RSLT);

                
                Execute(SQLS);
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库操作失败：" + ex.ToString());
                return false;

            }
            

            return true;
        }


        public void UpdateCompleted()
        {
            try
            {
                //获取到中间库中下达给本台体中任务单号为OrderNo的所有数据
                string EQUIP_QTY = string.Format(@"SELECT * FROM MT_DETECT_TASK WHERE DETECT_TASK_NO='{0}' AND SYS_NO='{1}' ORDER BY WRITE_DATE DESC", DETECT_TASK_NO,"999");
                DataTable AllMeterdt = ExecuteReader(EQUIP_QTY);
                int EQUIP_QTYNum = Convert.ToInt32(AllMeterdt.Rows[0]["EQUIP_QTY"].ToString().Trim());
                string xml = "<PARA>";
                xml += "<SYS_NO>801</SYS_NO>";
                // xml += "<SYS_NO>801</SYS_NO>";
                xml += "<DETECT_TASK_NO>" + DETECT_TASK_NO + "</DETECT_TASK_NO>";
                xml += "</PARA>";
                string[] args = new string[1];
                args[0] = xml;
                object result = WebServiceHelper.InvokeWebService(WebServiceURL, "getDETedTestData", args);
                if (result.ToString().ToUpper() == "FALSE")
                {
                    result = WebServiceHelper.InvokeWebService(WebServiceURL, "getDETedTestData", args);
                }
                else
                {
                    //MessageBox.Show("getDETedTestData完成，结论：" + result.ToString());
                    LogManager.AddMessage("getDETedTestData完成，结论：" + result.ToString(), EnumLogSource.检定业务日志, EnumLevel.Information);
                }

                if (!WebServiceHelper.GetResultByXml(result.ToString()))
                {
                    MessageBox.Show("数据上传失败，调用分项结论接口错误信息：" + WebServiceHelper.GetMessageByXml(result.ToString()));
                }

                bool DETECT_TASK = false;

                xml = "<PARA>";
                xml += "<SYS_NO>999</SYS_NO>";
                xml += "<DETECT_TASK_NO>" + DETECT_TASK_NO + "</DETECT_TASK_NO>";
                xml += "<VALID_QTY>" + EQUIP_QTYNum + "</VALID_QTY>";
                xml += "</PARA>";
                args[0] = xml;
                result = WebServiceHelper.InvokeWebService(WebServiceURL, "setResults", args);
                if (result.ToString().ToUpper() == "FALSE")
                {
                    result = WebServiceHelper.InvokeWebService(WebServiceURL, "setResults", args);

                }
                else
                {
                    DETECT_TASK = true;
                    LogManager.AddMessage("setResults完成，结论：" + result.ToString(), EnumLogSource.检定业务日志, EnumLevel.Information);
                }

                Thread.Sleep(2000);
                string PasMeter = string.Format(@"SELECT * FROM MT_DETECT_TMNL_RSLT WHERE DETECT_TASK_NO='{0}' AND SYS_NO='{1}' AND HANDLE_FLAG='2' ORDER BY WRITE_DATE DESC", DETECT_TASK_NO, ConfigHelper.Instance.EquipmentNo);
                DataTable PasMeterdt2 = ExecuteReader(PasMeter);
                if (PasMeterdt2.Rows.Count <= 0)
                {
                    MessageBox.Show("当前任务" + DETECT_TASK_NO + "|系统编号SYS_NO:平台未处理");
                }

                if (PasMeterdt2.Rows.Count < EQUIP_QTYNum)
                {
                    MessageBox.Show("当前任务" + DETECT_TASK_NO + "未完成，任务总数：" + EQUIP_QTYNum + "|当前已上传数量：" + PasMeterdt2.Rows.Count + WebServiceHelper.GetMessageByXml(result.ToString()));
                }
                if (DETECT_TASK)
                {

                    xml = "<PARA>";
                    xml += "<SYS_NO>999</SYS_NO>";
                    xml += "<DETECT_TASK_NO>" + DETECT_TASK_NO + "</DETECT_TASK_NO>";
                    xml += "<FLAG>01</FLAG>";
                    xml += "</PARA>";
                    args[0] = xml;
                    result = WebServiceHelper.InvokeWebService(WebServiceURL, "sendTaskFinishFlag", args);
                    if (result.ToString().ToUpper() == "FALSE")
                    {
                        result = WebServiceHelper.InvokeWebService(WebServiceURL, "sendTaskFinishFlag", args);
                    }
                    else
                    {
                        MessageBox.Show("sendTaskFinishFlag，结论：" + result.ToString());
                        LogManager.AddMessage("sendTaskFinishFlag，结论：" + result.ToString(), EnumLogSource.检定业务日志, EnumLevel.Information);
                    }


                    if (!WebServiceHelper.GetResultByXml(result.ToString()))
                    {
                        MessageBox.Show("数据上传失败，调用分项结论接口错误信息：" + WebServiceHelper.GetMessageByXml(result.ToString()));

                    }
                }
                else
                {
                    MessageBox.Show("setResults失败，检定完成sendTaskFinishFlag无法发送");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("web接口调用失败：" + ex.ToString());
            }
        }
        public bool UpdateCompleted(string DETECT_TASK_NO, string SYS_NO)
        {
            try
            {
                //获取到中间库中下达给本台体中任务单号为OrderNo的所有数据
                string EQUIP_QTY = string.Format(@"SELECT * FROM MT_DETECT_TASK WHERE DETECT_TASK_NO='{0}' AND SYS_NO='{1}' ORDER BY WRITE_DATE DESC", DETECT_TASK_NO, SYS_NO);
                DataTable AllMeterdt = ExecuteReader(EQUIP_QTY);
                int EQUIP_QTYNum = Convert.ToInt32(AllMeterdt.Rows[0]["EQUIP_QTY"].ToString().Trim());
                string xml = "<PARA>";
                xml += "<SYS_NO>"+ SYS_NO+"</SYS_NO>";
                xml += "<DETECT_TASK_NO>" + DETECT_TASK_NO + "</DETECT_TASK_NO>";
                xml += "</PARA>";
                string[] args = new string[1];
                args[0] = xml;
                object result = WebServiceHelper.InvokeWebService(WebServiceURL, "getDETedTestData", args);
                if (result.ToString().ToUpper() == "FALSE")
                {
                    result = WebServiceHelper.InvokeWebService(WebServiceURL, "getDETedTestData", args);
                }
                else
                {
                    //MessageBox.Show("getDETedTestData完成，结论：" + result.ToString());
                    LogManager.AddMessage("getDETedTestData完成，结论：" + result.ToString(), EnumLogSource.检定业务日志, EnumLevel.Information);
                }

                if (!WebServiceHelper.GetResultByXml(result.ToString()))
                {
                    MessageBox.Show("数据上传失败，调用分项结论接口错误信息：" + WebServiceHelper.GetMessageByXml(result.ToString()));
                }

                bool DETECT_TASK = false;

                xml = "<PARA>";
                xml += "<SYS_NO>" + SYS_NO + "</SYS_NO>";
                xml += "<DETECT_TASK_NO>" + DETECT_TASK_NO + "</DETECT_TASK_NO>";
                xml += "<VALID_QTY>" + EQUIP_QTYNum + "</VALID_QTY>";
                xml += "</PARA>";
                args[0] = xml;
                result = WebServiceHelper.InvokeWebService(WebServiceURL, "setResults", args);
                if (result.ToString().ToUpper() == "FALSE")
                {
                    result = WebServiceHelper.InvokeWebService(WebServiceURL, "setResults", args);

                }
                else
                {
                    DETECT_TASK = true;
                    LogManager.AddMessage("setResults完成，结论：" + result.ToString(), EnumLogSource.检定业务日志, EnumLevel.Information);
                }

                Thread.Sleep(2000);
                //string PasMeter = string.Format(@"SELECT * FROM MT_DETECT_TMNL_RSLT WHERE DETECT_TASK_NO='{0}' AND SYS_NO='{1}' AND HANDLE_FLAG='2' ORDER BY WRITE_DATE DESC", DETECT_TASK_NO, SYS_NO);
                string PasMeter = string.Format(@"SELECT * FROM MT_DETECT_TMNL_RSLT WHERE DETECT_TASK_NO='{0}' ORDER BY WRITE_DATE DESC", DETECT_TASK_NO, SYS_NO);
                DataTable PasMeterdt2 = ExecuteReader(PasMeter);
                if (PasMeterdt2.Rows.Count <= 0)
                {
                    MessageBox.Show("当前任务" + DETECT_TASK_NO + "|系统编号SYS_NO:" + SYS_NO + "|平台未处理");
                    return false;
                }

                if (PasMeterdt2.Rows.Count < EQUIP_QTYNum)
                {
                    MessageBox.Show("当前任务" + DETECT_TASK_NO + "未完成，任务总数：" + EQUIP_QTYNum + "|当前已上传数量：" + PasMeterdt2.Rows.Count + WebServiceHelper.GetMessageByXml(result.ToString()));
                    return false;
                }
                if (DETECT_TASK)
                {

                    xml = "<PARA>";
                    xml += "<SYS_NO>" + SYS_NO + "</SYS_NO>";
                    xml += "<DETECT_TASK_NO>" + DETECT_TASK_NO + "</DETECT_TASK_NO>";
                    xml += "<FLAG>01</FLAG>";
                    xml += "</PARA>";
                    args[0] = xml;
                    result = WebServiceHelper.InvokeWebService(WebServiceURL, "sendTaskFinishFlag", args);
                    if (result.ToString().ToUpper() == "FALSE")
                    {
                        result = WebServiceHelper.InvokeWebService(WebServiceURL, "sendTaskFinishFlag", args);
                    }
                    else
                    {
                        MessageBox.Show("sendTaskFinishFlag，结论：" + result.ToString());
                        LogManager.AddMessage("sendTaskFinishFlag，结论：" + result.ToString(), EnumLogSource.检定业务日志, EnumLevel.Information);
                    }


                    if (!WebServiceHelper.GetResultByXml(result.ToString()))
                    {
                        MessageBox.Show("数据上传失败，调用分项结论接口错误信息：" + WebServiceHelper.GetMessageByXml(result.ToString()));

                    }
                    return true;
                }
                else
                {
                    MessageBox.Show("setResults失败，检定完成sendTaskFinishFlag无法发送");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("web接口调用失败：" + ex.ToString());
                return false;
            }
        }
        public void UpdateInit()
        {
            SusessCount = 0;
        }


        #region


        #region 方法
        private string GetItemId(string Name)
        {
            switch (Name)
            {
                case "时钟召测和对时":
                    return "时钟召测和对时,1";
                case "基本参数":
                    return "基本参数,2";
                case "抄表与费率参数":
                    return "抄表与费率参数,3";
                //case "基本参数":
                //    return "以太网参数设置,2";
                case "状态量采集":
                    return "状态量采集,8";
                case "12个/分脉冲量采集":
                    return "12个/分脉冲量采集,10";
                case "120个/分脉冲量采集":
                    return "120个/分脉冲量采集,11";
                case "总加组日和月电量召集":
                    return "总加组日电量与月电量采集,12";
                case "分时段电能量数据存储":
                    return "分时段电能量数据存储,13";
                case "实时和当前数据":
                    return "实时和当前数据,15";
                case "历史日数据":
                    return "历史日数据,16";
                case "负荷曲线":
                    return "负荷曲线,17";
                case "历史月数据":
                    return "历史月数据,18";
                case "时段功控":
                    return "时段功控,20";
                case "厂休功控":
                    return "厂休功控,21";
                case "营业报停功控":
                    return "营业报停功控,22";
                case "当前功率下浮控":
                    return "营业报停功控,23";
                case "月电控":
                    return "月电控,24";
                case "购电控":
                    return "购电控,25";
                case "催费告警":
                    return "催费告警,26";
                case "保电功能":
                    return "保电功能,27";
                case "剔除功能":
                    return "剔除功能,28";
                case "遥控功能":
                    return "遥控功能,29";
                case "电能表超差事件":
                    return "电能量超差事件,38";
                case "电能表飞走事件":
                    return "电能表飞走事件,39";
                case "电能表停走事件":
                    return "电能表停走事件,40";
                case "电能表时间超差事件":
                    return "电能表时间超差事件,41";
                case "终端停/上电事件":
                    return "终端停/上电事件,50";
                case "终端485抄表错误":
                    return "终端485抄表错误事件,55";
                case "终端对时事件":
                    return "对时事件,64";
                case "常温基本误差":
                    return "常温基本误差,70";
                case "功率因素基本误差":
                    return "功率因素基本误差,71";
                case "谐波影响":
                    return "谐波影响,72";
                case "频率影响":
                    return "频率影响,73";
                case "电流不平衡影响":
                    return "电流不平衡影响,74";
                case "电源影响":
                    return "电源影响试验,75";
                case "日计时误差":
                    return "日计时误差,91";
                case "终端维护":
                    return "数据初始化,99";
                //case "密钥下装":
                //    return "身份认证及密钥协商,100";
                case "密钥下装":
                    return "密钥下装,100";
                case "事件参数":
                    return "事件参数,7";
                case "电能表实时数据":
                    return "电能表实时数据,14";
                case "电能表当前数据":
                    return "电能表当前数据,9";
                case "读取终端信息":
                    return "读取终端信息,19";
                case "终端逻辑地址查询":
                    return "终端逻辑地址查询,65";
                //case "交采电量清零":
                //    return "数据初始化(通信参数除外),66";
                case "交采电量清零":
                    return "交采电量清零,66";
                case "终端编程事件":
                    return "终端编程事件,67";
                case "终端密钥恢复":
                    return "终端密钥恢复,97";
                case "安全模式":
                    return "禁用安全模式字,79";
                case "外观":
                    return "外观检查,80";
                case "全事件采集上报":
                    return "全事件采集上报,80";
            }
            return "";
        }



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

       
        #endregion

        #endregion
    }
}
