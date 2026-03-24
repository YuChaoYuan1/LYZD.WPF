using System;
using System.Data;

namespace ZH.MeterProtocol.Struct
{
    /// <summary>
    /// 功能描述：数据标识信息
    /// </summary>
    [Serializable()]
    public class MeterProtocalItem
    {

        public MeterProtocalItem(DataRow row)
        {
            Name = row["name"].ToString();
            DataFlag645 = row["dlt64507Id"].ToString();
            DataFlag698 = row["dlt698Id"].ToString();
            Length645 = Convert.ToInt32(row["dlt64507Len"]);
            Dot645 = Convert.ToInt32(row["dlt64507Dot"]);
            Format645 = row["dlt64507Format"].ToString();
            Mode = Convert.ToInt32(row["dlt698Mode"]);
            Dot698 = Convert.ToInt32(row["dlt698Dot"]);
        }
        /// <summary>
        /// 数据标识名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 数据标识 645
        /// </summary>
        public string DataFlag645 { get; set; }

        /// <summary>
        /// 数据标识 698
        /// </summary>
        public string DataFlag698 { get; set; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public int Length645 { get; set; }

        /// <summary>
        /// 小数位
        /// </summary>
        public int Dot645 { get; set; }
        /// <summary>
        /// 数据格式
        /// </summary>
        public string Format645 { get; set; }

        /// <summary>
        /// dlt698Mode
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// 小数位
        /// </summary>
        public int Dot698 { get; set; }
    }
}
