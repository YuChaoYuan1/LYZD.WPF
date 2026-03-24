using LYZD.Core.Model.Meter;
using LYZD.Core.Model.Schema;
using LYZD.Mis.NanRui.LRDataTable;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LYZD.Mis.Common
{
    public interface IMis
    {
        /// <summary>
        /// 显示面板
        /// </summary>
        void ShowPanel(Control panel);

        void UpdateInit();

        /// <summary>
        /// 上传检定记录_单条数据
        /// </summary>
        /// <param name="meterList">数据对象集合</param>
        /// <returns></returns>
        bool Update(TestMeterInfo meters);

        /// <summary>
        /// 上传检定记录_多条数据
        /// </summary>
        /// <param name="meterList">数据对象集合</param>
        /// <returns></returns>
        bool Update(List<TestMeterInfo> meters);

        /// <summary>
        /// 全部表上传完成
        /// <paramref name="taskNo">任务单号</paramref>
        /// </summary>
        void UpdateCompleted();


        /// <summary>
        /// 下载表信息
        /// </summary>
        /// <param name="barCode">条码号</param>
        /// <param name="Item">被检表对象</param>
        /// <returns></returns>
        bool Down(string barcode, ref TestMeterInfo meter);

        /// <summary>
        /// 下载方案
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="schemeName"></param>
        /// <returns></returns>
        bool SchemeDown(string barcode, out string schemeName, out Dictionary<string, SchemaNode> Schema);

        /// <summary>
        /// 自动获取任务
        /// </summary>
        /// <param name="MD_BarCode">条形码</param>
        /// <param name="mT_DETECT_TASK">出库信息</param>
        /// <returns></returns>
        bool DownTask(string MD_BarCode, ref MT_DETECT_OUT_EQUIP mT_DETECT_TASK);

        /// <summary>
        /// 全部表上传完成
        /// </summary>
        /// <param name="DETECT_TASK_NO">任务号</param>
        /// <param name="SYS_NO">系统编号</param>
        bool UpdateCompleted(string DETECT_TASK_NO, string SYS_NO);
    }
}
