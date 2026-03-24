using LYZD.Core.Enum;
using LYZD.Core.Helper;
using LYZD.Core.Model.DnbModel.DnbInfo;
using LYZD.Core.Model.Meter;
using LYZD.Core.Model.Schema;
using LYZD.DAL.Config;
using LYZD.Mis.Common;
using LYZD.Mis.MDS.Table;
using LYZD.Mis.NanRui.LRDataTable;
using LYZD.Utility.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZH.MeterProtocol;

namespace LYZD.Mis.Houda
{
    public class Houda : OracleHelper, IMis
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
        public Houda(string ip, int port, string dataSource, string userId, string pwd, string url)
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
            //if (string.IsNullOrEmpty(MD_BarCode)) return false;

            //if (PCodeTable.Count <= 0)
            //    GetDicPCodeTable();
            //string sql = string.Format(@"SELECT * FROM mt_detect_out_equip t1 INNER JOIN mt_meter t2 ON t1.bar_code=t2.bar_code WHERE t2.bar_code='{0}' ORDER BY t1.write_date DESC", MD_BarCode.Trim());
            //DataTable dt = ExecuteReader(sql);
            //if (dt.Rows.Count <= 0) return false;
            //DataRow row = dt.Rows[0];
            //meter.Meter_ID = row["METER_ID"].ToString().Trim();
            //meter.MD_BarCode = row["BAR_CODE"].ToString().Trim();              //条形码
            ////meter.MD_AssetNo = row["ASSET_NO"].ToString().Trim();              //申请编号 --资产编号
            //meter.MD_MadeNo = row["MADE_NO"].ToString().Trim();                //出厂编号
            //meter.MD_TaskNo = row["DETECT_TASK_NO"].ToString().Trim();         //检定任务单，申请编号

            //meter.Type = GetPName("meterTypeCode", row["TYPE_CODE"]);
            return true;
        }

        //TODO2下载方案
        public bool SchemeDown(string MD_BarCode, out string schemeName, out Dictionary<string, SchemaNode> Schema)
        {
            throw new NotImplementedException();
        }

        public void ShowPanel(Control panel)
        {
            throw new NotImplementedException();
        }

        //TODO2上传检定记录

        public bool Update(TestMeterInfo meterInfo)
        {
            return true;
        }

        public bool Update(List<TestMeterInfo> meters)
        {
            throw new NotImplementedException();
        }

        private List<string> RemoveNull(List<string> sqlList)
        {

            List<string> str = new List<string>();
            for (int i = 0; i < sqlList.Count; i++)
            {
                if (sqlList[i] != null && sqlList[i] != "")
                {
                    str.Add(sqlList[i]);
                }
            }
            return str;
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

        public Dictionary<int, TestMeterInfo> GetMeterModel()
        {
            return null;
        }

        public bool DownTask(string MD_BarCode, ref MT_DETECT_OUT_EQUIP mT_DETECT_TASK)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCompleted(string DETECT_TASK_NO, string SYS_NO)
        {
            throw new NotImplementedException();
        }
    }
}
