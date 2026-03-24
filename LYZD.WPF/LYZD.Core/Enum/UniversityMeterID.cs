using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Core.Enum
{
    /// <summary>
    /// 大项目编号
    /// </summary>
    public class UniversityMeterID
    {
        /// <summary>
        ///没有结论，默认合格
        /// </summary>
        public const string 外观检查 = "000";

        public const string 参数设置与查询 = "01";
        public const string 数据采集 = "02";
        public const string 数据处理 = "03";
        public const string 控制试验 = "04";
        public const string 事件记录 = "05";
        public const string 任务上报 = "06";
        public const string 准确度试验 = "12";
        public const string 终端维护 = "15";
        public const string 身份认证及密钥协商试验 = "19";


    }
}