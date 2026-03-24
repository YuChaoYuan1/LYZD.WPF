using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.Core.Enum
{
    /// <summary>
    /// 功率方向
    /// </summary>
    public enum PowerWay
    {
        组合有功 = 0,

        正向有功 = 1,

        反向有功 = 2,

        正向无功 = 3,

        反向无功 = 4,

        第一象限无功 = 5,

        第二象限无功 = 6,

        第三象限无功 = 7,

        第四象限无功 = 8,

        /// <summary>
        /// 错误的、未赋值的
        /// </summary>
        Error = 9,

        固有误差正向有功 = 11,

        固有误差反向有功 = 12,

        固有误差正向无功 = 13,

        固有误差反向无功 = 14,

    }
    /// <summary>
    /// 需量方向
    /// </summary>
    public enum XuliangFangxiang
    {
        正向有功最大需量 = 0,
        反向有功最大需量 = 1,
        组合无功1最大需量 = 2,
        组合无功2最大需量 = 3,
        第一象限无功最大需量 = 4,
        第二象限无功最大需量 = 5,
        第三象限无功最大需量 = 6,
        第四象限无功最大需量 = 7,

    }
}
