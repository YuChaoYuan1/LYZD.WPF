using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.ViewModel.InputPara
{
    /// <summary>
    /// 变量比较的关系
    /// </summary>
    public enum EnumCompare
    {
        清空筛选条件 = 0,
        等于 = 1,
        大于 = 2,
        小于 = 3,
        不等于 = 4,
        包含 = 5,
        开头是 = 6,
        结尾是 = 7,
        自定义筛选条件 = 8
    }
}
