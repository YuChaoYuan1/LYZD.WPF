using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Mis.NanRui.LRDataTable
{
    /// <summary>
    /// B.14　施封明细信息
    /// </summary>
    public class MT_SEAL_INST
    {
        #region 属性
        /// <summary>
        /// 检定任务单号
        /// </summary>
        public string DETECT_TASK_NO { get; set; }
        /// <summary>
        /// 系统编号
        /// </summary>
        public string SYS_NO { get; set; }
        /// <summary>
        /// 设备类别
        /// </summary>
        public string EQUIP_CATEG { get; set; }
        /// <summary>
        /// 设备条码
        /// </summary>
        public string BAR_CODE { get; set; }
        /// <summary>
        /// 施封位置
        /// </summary>
        public string SEAL_POSITION { get; set; }
        /// <summary>
        /// 施封条码
        /// </summary>
        public string SEAL_BAR_CODE { get; set; }
        /// <summary>
        /// 施封时间
        /// </summary>
        public string SEAL_DATE { get; set; }
        /// <summary>
        /// 施封人员
        /// </summary>
        public string SEALER_NO { get; set; }
        /// <summary>
        /// 写入时间
        /// </summary>
        public string WRITE_DATE { get; set; }
        /// <summary>
        /// 处理标记
        /// </summary>
        public string HANDLE_FLAG { get; set; }
        /// <summary>
        /// 平台处理时间
        /// </summary>
        public string HANDLE_DATE { get; set; }
        /// <summary>
        /// 施封是否有效
        /// </summary>
        public string IS_VALID { get; set; }
        #endregion

        #region 公共函数
        public string ToInsertString()
        {
            string RowName = "";
            string Value = "";
            Type t = GetType();
            PropertyInfo[] propertys = t.GetProperties();
            foreach (PropertyInfo p in propertys)
            {
                object v = p.GetValue(this, null);
                if (v != null)
                {
                    if (string.IsNullOrEmpty(v.ToString())) continue;
                    RowName += "" + p.Name + ",";
                    //if (t.Name == "MT_CONTROL_MET_CONC" && p.Name == "DETECT_DATE"
                    //|| (t.Name == "MT_DETECT_RSLT" && p.Name == "WRITE_DATE")//济南
                    if (p.Name == "WRITE_DATE" || p.Name == "DETECT_DATE")
                    {
                        Value += string.Format("'{0}'", v.ToString()) + ",";
                    }
                    else
                    {
                        Value += "'" + v.ToString() + "',";
                    }
                }
            }
            RowName = RowName.TrimEnd(',');
            Value = Value.TrimEnd(',');

            string Tablename = t.Name;
            if (t.Name == "MT_SEAL_INST")
            {
                Tablename = "MT_SEAL_INST";
            }
            return string.Format("INSERT INTO sxykjd.{0}({1}) VALUES ({2})", Tablename, RowName, Value);

        }

        #endregion
    }
}
