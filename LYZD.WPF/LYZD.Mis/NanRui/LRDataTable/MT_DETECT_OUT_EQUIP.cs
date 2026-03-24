using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Mis.NanRui.LRDataTable
{
    /// <summary>
    /// 表B.8　检定设备出库明细
    /// </summary>
    public class MT_DETECT_OUT_EQUIP
    {
        /// <summary>
        /// 检定任务单编号
        /// </summary>
        public string  DETECT_TASK_NO;
        /// <summary>
        /// 系统编号
        /// </summary>
        public string  SYS_NO;
        /// <summary>
        /// 用表申请任务编号
        /// </summary>
        public string  IO_TASK_NO;
        /// <summary>
        /// 到货批次号
        /// </summary>
        public string  ARRIVE_BATCH_NO;
        /// <summary>
        /// 设备类别
        /// </summary>
        public string  EQUIP_CATEG;
        /// <summary>
        /// 设备条形码
        /// </summary>
        public string  BAR_CODE;
        /// <summary>
        /// 箱条形码
        /// </summary>
        public string  BOX_BAR_CODE;
        /// <summary>
        /// 垛号
        /// </summary>
        public string  PILE_NO;
        /// <summary>
        /// 抽检复检标识
        /// </summary>
        public string  REDETECT_FLAG;
        /// <summary>
        /// 站台号
        /// </summary>
        public string  PLATFORM_NO;
        /// <summary>
        /// 人员编号
        /// </summary>
        public string  EMP_NO;
        /// <summary>
        /// 台体类型
        /// </summary>
        public string  PLATFORM_TYPE;
        /// <summary>
        /// 平台写入时间
        /// </summary>
        public string  WRITE_DATE;
        /// <summary>
        /// 处理标记
        /// </summary>
        public string  HANDLE_FLAG;
        /// <summary>
        /// 处理时间
        /// </summary>
        public string HANDLE_DATE;

    }
}
