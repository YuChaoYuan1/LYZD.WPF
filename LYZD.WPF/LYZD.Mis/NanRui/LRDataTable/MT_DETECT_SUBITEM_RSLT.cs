using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Mis.NanRui.LRDataTable
{
    /// <summary>
    /// 表E.2　检测子项结论表
    /// </summary>
    public class MT_DETECT_SUBITEM_RSLT
    {
        #region 属性
        /// <summary>
        /// 检定任务编号
        /// </summary>
        public string  DETECT_TASK_NO { get; set; }
        /// <summary>
        /// 系统编号
        /// </summary>
        public string  SYS_NO{ get; set; }
        /// <summary>
        /// 检定线台编号
        /// </summary>
        public string  DETECT_EQUIP_NO{ get; set; }
        /// <summary>
        /// 检定单元编号
        /// </summary>
        public string  DETECT_UNIT_NO{ get; set; }
        /// <summary>
        /// 表位编号
        /// </summary>
        public string  POSITION_NO{ get; set; }
        /// <summary>
        /// 设备条形码
        /// </summary>
        public string  BAR_CODE{ get; set; }
        /// <summary>
        /// 检定时间
        /// </summary>
        public string  DETECT_DATE{ get; set; }
        /// <summary>
        /// 测试项编号
        /// </summary>
        public string  ITEM_NO{ get; set; }
        /// <summary>
        /// 子项编号
        /// </summary>
        public string  SUBITEM_NO{ get; set; }
        /// <summary>
        /// 子项名称
        /// </summary>
        public string  SUBITEM_NAME{ get; set; }
        /// <summary>
        /// 数据1
        /// </summary>
        public string  DATA1{ get; set; }
        /// <summary>
        /// 数据2
        /// </summary>
        public string  DATA2{ get; set; }
        /// <summary>
        /// 子项结论
        /// </summary>
        public string  SUBITEM_CONC{ get; set; }
        /// <summary>
        /// 测试时间
        /// </summary>
        public string HANDLE_DATE{ get; set; }

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
            if (t.Name == "MT_DETECT_SUBITEM_RSLT")
            {
                Tablename = "MT_DETECT_SUBITEM_RSLT";
            }
            return string.Format("INSERT INTO sxykjd.{0}({1}) VALUES ({2})", Tablename, RowName, Value);

        }

        #endregion

        public Dictionary<string, int> SubitemDic = new Dictionary<string, int>();
    }
}
