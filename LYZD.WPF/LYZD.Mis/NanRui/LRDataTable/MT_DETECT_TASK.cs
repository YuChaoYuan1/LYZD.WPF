using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Mis.NanRui.LRDataTable
{
    //检定任务表

    /// <summary>
    /// 表B.7　检定任务表
    /// </summary>
    public class MT_DETECT_TASK
    {
        /// <summary>
        /// 检定任务单编号
        /// </summary>
        public string DETECT_TASK_NO;
        /// <summary>
        /// 任务优先级
        /// </summary>
        public string TASK_PRIO;
        /// <summary>
        /// 检定方式
        /// </summary>
        public string DETECT_MODE;
        /// <summary>
        /// 系统编号
        /// </summary>
        public string SYS_NO;
        /// <summary>
        /// 到货批次号
        /// </summary>
        public string ARRIVE_BATCH_NO;
        /// <summary>
        /// A2  关联代码分类的设备类别实体记录，单相电能表、三相电能表、互感器、采集终端
        /// </summary>
        public string EQUIP_CATEG;
        /// <summary>
        /// 设备型号
        /// </summary>
        public string MODEL_CODE;
        /// <summary>
        /// 物料号
        /// </summary>
        public string ERP_BATCH_NO;
        /// <summary>
        /// 检定方案标识
        /// </summary>
        public string SCHEMA_ID;
        /// <summary>
        /// 复检方案标识
        /// </summary>
        public string REDETECT_SCHEMA;
        /// <summary>
        /// 是否复检
        /// </summary>
        public string REDETECT_FLAG;
        /// <summary>
        /// 复检数量
        /// </summary>
        public string REDETECT_QTY;
        /// <summary>
        /// 设备数量
        /// </summary>
        public string EQUIP_QTY;
        /// <summary>
        /// 垛总数
        /// </summary>
        public string PILE_QTY;
        /// <summary>
        /// 检验人
        /// </summary>
        public string EXEC_RESP_NAME;
        /// <summary>
        /// 核验人
        /// </summary>
        public string APPR_NAME;
        /// <summary>
        /// 是否自动施封
        /// </summary>
        public string IS_AUTO_SEAL;
        /// <summary>
        /// 任务状态
        /// </summary>
        public string TASK_STATUS;
        /// <summary>
        /// 平台写入时间
        /// </summary>
        public string WRITE_DATE;
        /// <summary>
        /// 处理标记
        /// </summary>
        public string HANDLE_FLAG;
        /// <summary>
        /// 处理时间
        /// </summary>
        public string HANDLE_DATE;
        /// <summary>
        /// 新设备码
        /// </summary>
        public string EQUIP_CODE_NEW;
        /// <summary>
        /// 变更参数类型
        /// </summary>
        public string PARAM_TYPE;
        /// <summary>
        /// 任务类型
        /// </summary>
        public string TASK_TYPE;
        /// <summary>
        /// 设备状态
        /// </summary>
        public string EQUIP_STATUS_CODE;

    }
}
