using System.Data;
using System.Linq;
using ZH.MeterProtocol.Protocols.DLT698.Enum;
using ZH.MeterProtocol.Struct;

namespace ZH.MeterProtocol
{
    public class MeterProtocal
    {
        static MeterProtocal()
        {
            if (Protocals == null)
                Protocals = new MeterProtocal().ProtocalLib();
        }
        public static DataTable Protocals { get; set; }

        public static MeterProtocalItem Select(string name)
        {
            DataRow[] rows = Protocals.Select("name='" + name + "' OR dlt64507Id='" + name + "' OR dlt698Id='" + name + "'");
            if (rows.Count() > 0)
                return new MeterProtocalItem(rows[0]);
            else
                return null;
        }

        /// <summary>
        /// 获取通讯协议名称
        /// </summary>
        /// <returns></returns>
        public static string GetProtocalName(string name,string Code)
        {
            DataRow[] rows = Protocals.Select("name='" + name + "' AND dlt64507Id='" + Code + "'");
            if (rows.Count() > 0)
            {
                return "CDLT6452007";
            }
            rows = Protocals.Select("name='" + name + "' AND dlt698Id='" + Code + "'");
            if (rows.Count() > 0)
            {
                return "CDLT698";
            }
            return "";
        }


        private DataTable ProtocalLib()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("dlt64507Id", typeof(string));
            dt.Columns.Add("dlt64507Len", typeof(int)); //字节数量
            dt.Columns.Add("dlt64507Dot", typeof(int)); //小数点位数
            dt.Columns.Add("dlt698Id", typeof(string));
            dt.Columns.Add("dlt698Mode", typeof(int));
            dt.Columns.Add("dlt698Dot", typeof(int));   //小数点位数
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("dlt64507Format", typeof(string));
            dt.Rows.Add("070001FF", 2, 0, "F1000500", EmSecurityMode.ClearTextRand, 0, "身份认证有效时长", "NNNN");//身份认证有效时长

            //新添加
            dt.Rows.Add("04000306", 3, 0, "401C0200", EmSecurityMode.ClearTextRand, 0, "电流互感器变比", "NNNNNN");//电流互感器变比
            dt.Rows.Add("04000307", 3, 0, "401D0200", EmSecurityMode.ClearTextRand, 0, "电压互感器变比", "NNNNNN");//电压互感器变比
            dt.Rows.Add("0400040E", 6, 0, "40030200", EmSecurityMode.ClearTextRand, 0, "用户号", "NNNNNNNNNNNN");//用户号
            dt.Rows.Add("04000C03", 4, 0, "04000C03", EmSecurityMode.ClearTextRand, 0, "02级密码", "NNNNNNNN");//02级密码
            dt.Rows.Add("04000C05", 4, 0, "04000C05", EmSecurityMode.ClearTextRand, 0, "04级密码", "NNNNNNNN");//04级密码
            dt.Rows.Add("04001003", 4, 2, "401F0201", EmSecurityMode.ClearTextRand, 2, "透支金额限值", "XXXXXX.XX");//透支金额限值
            dt.Rows.Add("04001004", 4, 2, "401F0202", EmSecurityMode.ClearTextRand, 2, "囤积金额限值", "XXXXXX.XX");//囤积金额限值
            dt.Rows.Add("04001005", 4, 2, "401F0203", EmSecurityMode.ClearTextRand, 2, "合闸允许金额限值", "XXXXXX.XX");//合闸允许金额限值
            dt.Rows.Add("04001404", 2, 0, "F1000E00", EmSecurityMode.ClearTextRand, 0, "红外认证时效", "NNNN");//红外认证时效
            dt.Rows.Add("04040255", 5, 0, "F3010255", EmSecurityMode.ClearTextRand, 0, "按键循环显示第85屏", "NNNNNNNNNN");//按键循环显示第85屏
            dt.Rows.Add("04040256", 5, 0, "F3010256", EmSecurityMode.ClearTextRand, 0, "按键循环显示第86屏", "NNNNNNNNNN");//按键循环显示第86屏
            dt.Rows.Add("04040257", 5, 0, "F3010257", EmSecurityMode.ClearTextRand, 0, "按键循环显示第87屏", "NNNNNNNNNN");//按键循环显示第87屏
            dt.Rows.Add("04040258", 5, 0, "F3010258", EmSecurityMode.ClearTextRand, 0, "按键循环显示第88屏", "NNNNNNNNNN");//按键循环显示第88屏
            dt.Rows.Add("04040259", 5, 0, "F3010259", EmSecurityMode.ClearTextRand, 0, "按键循环显示第89屏", "NNNNNNNNNN");//按键循环显示第89屏
            dt.Rows.Add("0404025A", 5, 0, "F301025A", EmSecurityMode.ClearTextRand, 0, "按键循环显示第90屏", "NNNNNNNNNN");//按键循环显示第90屏
            dt.Rows.Add("040605FF", 64, 4, "401B0200", EmSecurityMode.ClearTextRand, 4, "备用套阶梯参数数据块", "NNNN.NNNN");//备用套阶梯参数数据块


            //名字不同部分
            dt.Rows.Add("04000302", 1, 0, "F3000300", EmSecurityMode.ClearTextRand, 0, "每屏显示时间", "NN");//每屏显示时间
            dt.Rows.Add("04000305", 5, 0, "F3010401", EmSecurityMode.ClearTextRand, 0, "按键循环显示屏数", "NN");//按键循环显示屏数
            dt.Rows.Add("04000401", 6, 0, "40010200", EmSecurityMode.ClearTextRand, 0, "通讯地址", "NNNNNNNNNNNN");//通讯地址
            dt.Rows.Add("04000403", 32, 0, "41030200", EmSecurityMode.ClearTextRand, 0, "资产管理编码(ASCII 码)", "NN…NN");//资产管理编码(ASCII 码)
            dt.Rows.Add("04000409", 3, 0, "41090200", EmSecurityMode.ClearTextRand, 0, "电能表有功常数", "XXXXXX");//电能表有功常数
            dt.Rows.Add("04000701", 1, 0, "F2020201", EmSecurityMode.ClearTextRand, 0, "通信速率特征字（调制红外）", "NN");//通信速率特征字（调制红外）
            dt.Rows.Add("04000703", 1, 0, "F2010200", EmSecurityMode.ClearTextRand, 0, "通信速率特征字（485-1）", "NN");//通信速率特征字（485-1）
            dt.Rows.Add("04000704", 1, 0, "F2010202", EmSecurityMode.ClearTextRand, 0, "通信速率特征字（485-2）", "NN");//通信速率特征字（485-2）
            dt.Rows.Add("04000705", 1, 0, "F2090200", EmSecurityMode.ClearTextRand, 0, "通信口3通信速率特征字（模块）", "NN");//通信口3通信速率特征字（模块）
            dt.Rows.Add("04001405", 2, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "主动上报状态字自动复位延时时间", "NNNN");//主动上报状态字自动复位延时时间
            dt.Rows.Add("50060300", 2, 0, "50060300", EmSecurityMode.ClearTextRand, 0, "月冻结属性表", "XXXXXXXXXXXXXXXXXXXXX");//月冻结属性表
            dt.Rows.Add("50040300", 2, 0, "50040300", EmSecurityMode.ClearTextRand, 0, "日冻结属性表", "XXXXXXXXXXXXXXXXXXXXX");//日冻结属性表
            dt.Rows.Add("50030300", 2, 0, "50030300", EmSecurityMode.ClearTextRand, 0, "小时冻结属性表", "XXXXXXXXXXXXXXXXXXXXX");//小时冻结属性表
            dt.Rows.Add("50020300", 2, 0, "50020300", EmSecurityMode.ClearTextRand, 0, "分钟冻结属性表", "XXXXXXXXXXXXXXXXXXXXX");//分钟冻结属性表
            dt.Rows.Add("50000300", 2, 0, "50000300", EmSecurityMode.ClearTextRand, 0, "瞬时冻结属性表", "XXXXXXXXXXXXXXXXXXXXX");//瞬时冻结属性表
            dt.Rows.Add("50050300", 2, 0, "50050300", EmSecurityMode.ClearTextRand, 0, "结算日冻结属性表", "XXXXXXXXXXXXXXXXXXXXX");//结算日冻结属性表
            dt.Rows.Add("50080300", 2, 0, "50080300", EmSecurityMode.ClearTextRand, 0, "时区表切换冻结属性表", "XXXXXXXXXXXXXXXXXXXXX");//时区表切换冻结属性表
            dt.Rows.Add("50090300", 2, 0, "50090300", EmSecurityMode.ClearTextRand, 0, "日时段表切换冻结属性表", "XXXXXXXXXXXXXXXXXXXXX");//日时段表切换冻结属性表
            dt.Rows.Add("500A0300", 2, 0, "500A0300", EmSecurityMode.ClearTextRand, 0, "费率电价切换冻结属性表", "XXXXXXXXXXXXXXXXXXXXX");//费率电价切换冻结属性表
            dt.Rows.Add("0400040D", 0, 0, "44000301", EmSecurityMode.ClearTextRand, 0, "协议版本号", "NNNNNNNN");//协议版本号
            dt.Rows.Add("FFFFFFFF", 0, 0, "50000201", EmSecurityMode.ClearTextRand, 0, "(上1次)瞬时冻结记录", "");//(上1次)瞬时冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50000202", EmSecurityMode.ClearTextRand, 0, "(上2次)瞬时冻结记录", "");//(上2次)瞬时冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50000203", EmSecurityMode.ClearTextRand, 0, "(上3次)瞬时冻结记录", "");//(上3次)瞬时冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50000204", EmSecurityMode.ClearTextRand, 0, "(上4次)瞬时冻结记录", "");//(上4次)瞬时冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50000205", EmSecurityMode.ClearTextRand, 0, "(上5次)瞬时冻结记录", "");//(上5次)瞬时冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50010201", EmSecurityMode.ClearTextRand, 0, "(上1次)秒冻结记录", "");//(上1次)秒冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50010202", EmSecurityMode.ClearTextRand, 0, "(上2次)秒冻结记录", "");//(上2次)秒冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50010203", EmSecurityMode.ClearTextRand, 0, "(上3次)秒冻结记录", "");//(上3次)秒冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50010204", EmSecurityMode.ClearTextRand, 0, "(上4次)秒冻结记录", "");//(上4次)秒冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50010205", EmSecurityMode.ClearTextRand, 0, "(上5次)秒冻结记录", "");//(上5次)秒冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50020201", EmSecurityMode.ClearTextRand, 0, "(上1次)分钟冻结记录", "");//(上1次)分钟冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50020202", EmSecurityMode.ClearTextRand, 0, "(上1次)分钟冻结记录", "");//(上1次)分钟冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50020203", EmSecurityMode.ClearTextRand, 0, "(上1次)分钟冻结记录", "");//(上1次)分钟冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50020204", EmSecurityMode.ClearTextRand, 0, "(上1次)分钟冻结记录", "");//(上1次)分钟冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50020205", EmSecurityMode.ClearTextRand, 0, "(上1次)分钟冻结记录", "");//(上1次)分钟冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50030201", EmSecurityMode.ClearTextRand, 0, "(上1次)小时冻结记录", "");//(上1次)小时冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50030202", EmSecurityMode.ClearTextRand, 0, "(上2次)小时冻结记录", "");//(上2次)小时冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50030203", EmSecurityMode.ClearTextRand, 0, "(上3次)小时冻结记录", "");//(上3次)小时冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50030204", EmSecurityMode.ClearTextRand, 0, "(上4次)小时冻结记录", "");//(上4次)小时冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50030205", EmSecurityMode.ClearTextRand, 0, "(上5次)小时冻结记录", "");//(上5次)小时冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50040201", EmSecurityMode.ClearTextRand, 0, "(上1次)日冻结记录", "");//(上1次)日冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50040202", EmSecurityMode.ClearTextRand, 0, "(上2次)日冻结记录", "");//(上2次)日冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50040203", EmSecurityMode.ClearTextRand, 0, "(上3次)日冻结记录", "");//(上3次)日冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50040204", EmSecurityMode.ClearTextRand, 0, "(上4次)日冻结记录", "");//(上4次)日冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50040205", EmSecurityMode.ClearTextRand, 0, "(上5次)日冻结记录", "");//(上5次)日冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50050201", EmSecurityMode.ClearTextRand, 0, "(上1次)结算日冻结记录", "");//(上1次)结算日冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50050202", EmSecurityMode.ClearTextRand, 0, "(上2次)结算日冻结记录", "");//(上2次)结算日冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50050203", EmSecurityMode.ClearTextRand, 0, "(上3次)结算日冻结记录", "");//(上3次)结算日冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50050204", EmSecurityMode.ClearTextRand, 0, "(上4次)结算日冻结记录", "");//(上4次)结算日冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50050205", EmSecurityMode.ClearTextRand, 0, "(上5次)结算日冻结记录", "");//(上5次)结算日冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50060201", EmSecurityMode.ClearTextRand, 0, "(上1次)月冻结记录", "");//(上1次)月冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50060202", EmSecurityMode.ClearTextRand, 0, "(上2次)月冻结记录", "");//(上2次)月冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50060203", EmSecurityMode.ClearTextRand, 0, "(上3次)月冻结记录", "");//(上3次)月冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50060204", EmSecurityMode.ClearTextRand, 0, "(上4次)月冻结记录", "");//(上4次)月冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50060205", EmSecurityMode.ClearTextRand, 0, "(上5次)月冻结记录", "");//(上5次)月冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50070201", EmSecurityMode.ClearTextRand, 0, "(上1次)年冻结记录", "");//(上1次)年冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50070202", EmSecurityMode.ClearTextRand, 0, "(上2次)年冻结记录", "");//(上2次)年冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50070203", EmSecurityMode.ClearTextRand, 0, "(上3次)年冻结记录", "");//(上3次)年冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50070204", EmSecurityMode.ClearTextRand, 0, "(上4次)年冻结记录", "");//(上4次)年冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50070205", EmSecurityMode.ClearTextRand, 0, "(上5次)年冻结记录", "");//(上5次)年冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50080201", EmSecurityMode.ClearTextRand, 0, "(上1次)时区表切换冻结记录", "");//(上1次)时区表切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50080202", EmSecurityMode.ClearTextRand, 0, "(上2次)时区表切换冻结记录", "");//(上2次)时区表切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50080203", EmSecurityMode.ClearTextRand, 0, "(上3次)时区表切换冻结记录", "");//(上3次)时区表切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50080204", EmSecurityMode.ClearTextRand, 0, "(上4次)时区表切换冻结记录", "");//(上4次)时区表切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50080205", EmSecurityMode.ClearTextRand, 0, "(上5次)时区表切换冻结记录", "");//(上5次)时区表切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50090201", EmSecurityMode.ClearTextRand, 0, "(上1次)日时段表切换冻结记录", "");//(上1次)日时段表切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50090202", EmSecurityMode.ClearTextRand, 0, "(上2次)日时段表切换冻结记录", "");//(上2次)日时段表切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50090203", EmSecurityMode.ClearTextRand, 0, "(上3次)日时段表切换冻结记录", "");//(上3次)日时段表切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50090204", EmSecurityMode.ClearTextRand, 0, "(上4次)日时段表切换冻结记录", "");//(上4次)日时段表切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "50090205", EmSecurityMode.ClearTextRand, 0, "(上5次)日时段表切换冻结记录", "");//(上5次)日时段表切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "500A0201", EmSecurityMode.ClearTextRand, 0, "(上1次)费率电价切换冻结记录", "");//(上1次)费率电价切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "500A0202", EmSecurityMode.ClearTextRand, 0, "(上2次)费率电价切换冻结记录", "");//(上2次)费率电价切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "500A0203", EmSecurityMode.ClearTextRand, 0, "(上3次)费率电价切换冻结记录", "");//(上3次)费率电价切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "500A0204", EmSecurityMode.ClearTextRand, 0, "(上4次)费率电价切换冻结记录", "");//(上4次)费率电价切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "500A0205", EmSecurityMode.ClearTextRand, 0, "(上5次)费率电价切换冻结记录", "");//(上5次)费率电价切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "500B0201", EmSecurityMode.ClearTextRand, 0, "(上1次)阶梯切换冻结记录", "");//(上1次)阶梯切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "500B0202", EmSecurityMode.ClearTextRand, 0, "(上2次)阶梯切换冻结记录", "");//(上2次)阶梯切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "500B0203", EmSecurityMode.ClearTextRand, 0, "(上3次)阶梯切换冻结记录", "");//(上3次)阶梯切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "500B0204", EmSecurityMode.ClearTextRand, 0, "(上4次)阶梯切换冻结记录", "");//(上4次)阶梯切换冻结记录
            dt.Rows.Add("FFFFFFFF", 0, 0, "500B0205", EmSecurityMode.ClearTextRand, 0, "(上5次)阶梯切换冻结记录", "");//(上5次)阶梯切换冻结记录
            dt.Rows.Add("500B0300", 2, 0, "500B0300", EmSecurityMode.ClearTextRand, 0, "阶梯切换冻结属性表", "XXXXXXXXXXXXXXXXXXXXX");//阶梯切换冻结属性表
            dt.Rows.Add("50110300", 2, 0, "50110300", EmSecurityMode.ClearTextRand, 0, "阶梯结算冻结属性表", "XXXXXXXXXXXXXXXXXXXXX");//阶梯结算冻结属性表
            dt.Rows.Add("040604FF", 64, 4, "401A0200", EmSecurityMode.ClearTextRand, 4, "当前套阶梯参数数据块", "NNNN.NNNN");//当前套阶梯参数数据块
            dt.Rows.Add("401A0201", 4, 2, "401A0201", EmSecurityMode.ClearTextRand, 2, "当前套阶梯值1～6", "NNNNNN.NN");//当前套阶梯值1～6
            dt.Rows.Add("401A0202", 4, 4, "401A0202", EmSecurityMode.ClearTextRand, 4, "当前套阶梯电价1～7", "NNNN.NNNN");//当前套阶梯电价1～7
            dt.Rows.Add("401A0203", 3, 0, "401A0203", EmSecurityMode.ClearTextRand, 0, "当前套年第1～4结算日", "MMDDhh");//当前套年第1～4结算日
            dt.Rows.Add("401B0201", 4, 2, "401B0201", EmSecurityMode.ClearTextRand, 2, "备用套阶梯值1～6", "NNNNNN.NN");//备用套阶梯值1～6
            dt.Rows.Add("401B0202", 4, 4, "401B0202", EmSecurityMode.ClearTextRand, 4, "备用套阶梯电价1～7", "NNNN.NNNN");//备用套阶梯电价1～7
            dt.Rows.Add("401B0203", 3, 0, "401B0203", EmSecurityMode.ClearTextRand, 0, "备用套年第1～4结算日", "MMDDhh");//备用套年第1～4结算日
            dt.Rows.Add("04000109", 5, 0, "400B0200", EmSecurityMode.ClearTextRand, 0, "两套阶梯切换时间", "YYMMDDhhmm");//两套阶梯切换时间

            //---------------------------------------------------------------------------------
            dt.Rows.Add("00000000", 4, 2, "00000201", EmSecurityMode.ClearText,2, "(当前)组合有功总电能", "XXXXXX.XX"); //高精度:00000400,包含[总 尖 峰 平 谷]
            dt.Rows.Add("00000001", 4, 2, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)组合有功总电能", "XXXXXX.XX"); //高精度
            dt.Rows.Add("00000100", 4, 2, "00000202", EmSecurityMode.ClearText, 2, "(当前)组合有功尖电能", "XXXXXX.XX"); //高精度:00000400,包含[总 尖 峰 平 谷]
            dt.Rows.Add("00000200", 4, 2, "00000203", EmSecurityMode.ClearText, 2, "(当前)组合有功峰电能", "XXXXXX.XX"); //高精度:00000400,包含[总 尖 峰 平 谷]
            dt.Rows.Add("00000300", 4, 2, "00000204", EmSecurityMode.ClearText, 2, "(当前)组合有功平电能", "XXXXXX.XX"); //高精度:00000400,包含[总 尖 峰 平 谷]
            dt.Rows.Add("00000400", 4, 2, "00000205", EmSecurityMode.ClearText, 2, "(当前)组合有功谷电能", "XXXXXX.XX"); //高精度:00000400,包含[总 尖 峰 平 谷]
            dt.Rows.Add("0000FF00", 20, 0, "00000200", EmSecurityMode.ClearText, 2, "(当前)组合有功电能数据块", "XXXXXX.XX"); //高精度:00000400,包含[总 尖 峰 平 谷]
            dt.Rows.Add("0000FF00", 20, 0, "00000400", EmSecurityMode.ClearText, 4, "(当前)组合有功电能数据块高精度", "XXXXXX.XX"); //高精度:00000400,包含[总 尖 峰 平 谷]

            dt.Rows.Add("00010000", 4, 2, "00100201", EmSecurityMode.ClearText, 2, "(当前)正向有功总电能", "XXXXXX.XX");
            dt.Rows.Add("00010001", 4, 2, "50050201", EmSecurityMode.ClearText, 2, "(上1结算日)正向有功总电能", "XXXXXX.XX"); //高精度

            dt.Rows.Add("00010100", 4, 2, "00100201", EmSecurityMode.ClearText, 2, "(当前)正向有功尖电能", "XXXXXX.XX"); //高精度:00000400,包含[总 尖 峰 平 谷]
            dt.Rows.Add("00010200", 4, 2, "00100202", EmSecurityMode.ClearText, 2, "(当前)正向有功峰电能", "XXXXXX.XX"); //高精度:00000400,包含[总 尖 峰 平 谷]
            dt.Rows.Add("00010300", 4, 2, "00100203", EmSecurityMode.ClearText, 2, "(当前)正向有功平电能", "XXXXXX.XX"); //高精度:00000400,包含[总 尖 峰 平 谷]
            dt.Rows.Add("00010400", 4, 2, "00100204", EmSecurityMode.ClearText, 2, "(当前)正向有功谷电能", "XXXXXX.XX"); //高精度:00000400,包含[总 尖 峰 平 谷]
            dt.Rows.Add("0001FF00", 20, 0, "00100200", EmSecurityMode.ClearText, 2, "(当前)正向有功电能数据块", "XXXXXX.XX");

            dt.Rows.Add("00020000", 4, 2, "00200201", EmSecurityMode.ClearText, 2, "(当前)反向有功总电能", "XXXXXX.XX");
            dt.Rows.Add("00020001", 4, 2, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)反向有功总电能", "XXXXXX.XX"); //高精度

            dt.Rows.Add("00020100", 4, 0, "00200201", EmSecurityMode.ClearText, 2, "(当前)反向有功尖电能", "XXXXXX.XX");
            dt.Rows.Add("00020200", 4, 0, "00200202", EmSecurityMode.ClearText, 2, "(当前)反向有功峰电能", "XXXXXX.XX");
            dt.Rows.Add("00020300", 4, 0, "00200203", EmSecurityMode.ClearText, 2, "(当前)反向有功平电能", "XXXXXX.XX");
            dt.Rows.Add("00020400", 4, 0, "00200204", EmSecurityMode.ClearText, 2, "(当前)反向有功谷电能", "XXXXXX.XX");
            dt.Rows.Add("0002FF00", 20, 0, "00200200", EmSecurityMode.ClearText, 2, "(当前)反向有功电能数据块", "XXXXXX.XX");

            dt.Rows.Add("00030000", 4, 2, "00300201", EmSecurityMode.ClearText, 2, "(当前)组合无功1总电能", "XXXXXX.XX"); //高精度
            dt.Rows.Add("00030001", 4, 2, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)组合无功1总电能", "XXXXXX.XX"); //高精度
            dt.Rows.Add("0003FF00", 20, 0, "00300200", EmSecurityMode.ClearText, 2, "(当前)组合无功1电能数据块", "XXXXXX.XX"); //高精度

            dt.Rows.Add("00040000", 4, 2, "00400201", EmSecurityMode.ClearText, 2, "(当前)组合无功2总电能", "XXXXXX.XX"); //高精度
            dt.Rows.Add("00040001", 4, 2, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)组合无功2总电能", "XXXXXX.XX"); //高精度
            dt.Rows.Add("0004FF00", 4, 2, "00400200", EmSecurityMode.ClearText, 2, "(当前)组合无功2电能数据块", "XXXXXX.XX"); //高精度

            dt.Rows.Add("00050000", 4, 2, "00500201", EmSecurityMode.ClearText, 2, "(当前)第一象限无功总电能", "XXXXXX.XX"); //高精度
            dt.Rows.Add("00050001", 4, 2, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)第一象限无功总电能", "XXXXXX.XX"); //高精度

            dt.Rows.Add("00060000", 4, 2, "00600201", EmSecurityMode.ClearText, 2, "(当前)第二象限无功总电能", "XXXXXX.XX"); //高精度
            dt.Rows.Add("00060001", 4, 2, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)第二象限无功总电能", "XXXXXX.XX"); //高精度

            dt.Rows.Add("00070000", 4, 2, "00700201", EmSecurityMode.ClearText, 2, "(当前)第三象限无功总电能", "XXXXXX.XX"); //高精度
            dt.Rows.Add("00070001", 4, 2, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)第三象限无功总电能", "XXXXXX.XX"); //高精度

            dt.Rows.Add("00080000", 4, 2, "00800201", EmSecurityMode.ClearText, 2, "(当前)第四象限无功总电能", "XXXXXX.XX"); //高精度
            dt.Rows.Add("00080001", 4, 2, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)第四象限无功总电能", "XXXXXX.XX"); //高精度

            dt.Rows.Add("00900100", 4, 2, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(当前)剩余电量", "XXXXXX.XX");
            dt.Rows.Add("00900200", 4, 2, "202C0201", EmSecurityMode.ClearText, 2, "(当前)剩余金额", "XXXXXX.XX");

            dt.Rows.Add("01010000", 8, 0, "10100201", EmSecurityMode.ClearTextRand, 0, "(当前)正向有功总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("01010001", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)正向有功总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");

            dt.Rows.Add("0101FF00", 8, 0, "10100200", EmSecurityMode.ClearTextRand, 0, "(当前)正向有功最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0101FF01", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)正向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0101FF02", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上2结算日)正向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0101FF03", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上3结算日)正向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0101FF04", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上4结算日)正向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0101FF05", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上5结算日)正向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0101FF06", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上6结算日)正向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0101FF07", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上7结算日)正向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0101FF08", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上8结算日)正向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0101FF09", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上9结算日)正向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0101FF0A", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上10结算日)正向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");

            dt.Rows.Add("01020000", 8, 0, "10200201", EmSecurityMode.ClearTextRand, 2, "(当前)反向有功总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("01020001", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)反向有功总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");

            dt.Rows.Add("0102FF00", 8, 0, "10200200", EmSecurityMode.ClearTextRand, 2, "(当前)反向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0102FF01", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)反向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0102FF02", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上2结算日)反向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0102FF03", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上3结算日)反向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0102FF04", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上4结算日)反向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0102FF05", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上5结算日)反向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0102FF06", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上6结算日)反向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0102FF07", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上7结算日)反向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0102FF08", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上8结算日)反向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0102FF09", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上9结算日)反向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0102FF0A", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上10结算日)反向有功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");

            dt.Rows.Add("01030000", 8, 0, "10300200", EmSecurityMode.ClearTextRand, 2, "(当前)组合无功1总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("01030001", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)组合无功1总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");

            dt.Rows.Add("0103FF00", 8, 0, "10300200", EmSecurityMode.ClearTextRand, 2, "(当前)组合无功1总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0103FF01", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)组合无功1总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0103FF02", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上2结算日)组合无功1总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0103FF03", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上3结算日)组合无功1总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0103FF04", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上4结算日)组合无功1总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0103FF05", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上5结算日)组合无功1总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0103FF06", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上6结算日)组合无功1总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0103FF07", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上7结算日)组合无功1总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0103FF08", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上8结算日)组合无功1总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0103FF09", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上9结算日)组合无功1总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0103FF0A", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上10结算日)组合无功1总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");


            dt.Rows.Add("01040000", 8, 0, "10400200", EmSecurityMode.ClearTextRand, 2, "(当前)组合无功2总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("01040001", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)组合无功2总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");

            dt.Rows.Add("0104FF00", 8, 0, "10400200", EmSecurityMode.ClearTextRand, 2, "(当前)组合无功2总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0104FF01", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)组合无功2总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0104FF02", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上2结算日)组合无功2总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0104FF03", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上3结算日)组合无功2总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0104FF04", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上4结算日)组合无功2总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0104FF05", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上5结算日)组合无功2总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0104FF06", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上6结算日)组合无功2总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0104FF07", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上7结算日)组合无功2总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0104FF08", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上8结算日)组合无功2总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0104FF09", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上9结算日)组合无功2总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0104FF0A", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上10结算日)组合无功2总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");


            dt.Rows.Add("01050000", 8, 0, "10500201", EmSecurityMode.ClearTextRand, 2, "(当前)第一象限无功总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("01050001", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)第一象限无功总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");

            dt.Rows.Add("0105FF00", 8, 0, "10500200", EmSecurityMode.ClearTextRand, 2, "(当前)第一象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0105FF01", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)第一象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0105FF02", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上2结算日)第一象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0105FF03", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上3结算日)第一象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0105FF04", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上4结算日)第一象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0105FF05", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上5结算日)第一象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0105FF06", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上6结算日)第一象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0105FF07", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上7结算日)第一象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0105FF08", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上8结算日)第一象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0105FF09", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上9结算日)第一象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0105FF0A", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上10结算日)第一象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");


            dt.Rows.Add("01060000", 8, 0, "10600201", EmSecurityMode.ClearTextRand, 2, "(当前)第二象限无功总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("01060001", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)第二象限无功总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");

            dt.Rows.Add("0106FF00", 8, 0, "10600200", EmSecurityMode.ClearTextRand, 2, "(当前)第二象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0106FF01", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)第二象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0106FF02", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上2结算日)第二象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0106FF03", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上3结算日)第二象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0106FF04", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上4结算日)第二象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0106FF05", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上5结算日)第二象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0106FF06", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上6结算日)第二象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0106FF07", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上7结算日)第二象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0106FF08", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上8结算日)第二象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0106FF09", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上9结算日)第二象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0106FF0A", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上10结算日)第二象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");

            dt.Rows.Add("01070000", 8, 0, "10700201", EmSecurityMode.ClearTextRand, 2, "(当前)第三象限无功总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("01070001", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)第三象限无功总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");

            dt.Rows.Add("0107FF00", 8, 0, "10700200", EmSecurityMode.ClearTextRand, 2, "(当前)第三象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0107FF01", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)第三象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0107FF02", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上2结算日)第三象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0107FF03", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上3结算日)第三象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0107FF04", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上4结算日)第三象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0107FF05", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上5结算日)第三象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0107FF06", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上6结算日)第三象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0107FF07", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上7结算日)第三象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0107FF08", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上8结算日)第三象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0107FF09", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上9结算日)第三象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0107FF0A", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上10结算日)第三象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");

            dt.Rows.Add("01080000", 8, 0, "10800201", EmSecurityMode.ClearTextRand, 2, "(当前)第四象限无功总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("01080001", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)第四象限无功总最大需量及发生时间", "XX.XXXXYYMMDDHHmm");

            dt.Rows.Add("0108FF00", 8, 0, "10800200", EmSecurityMode.ClearTextRand, 2, "(当前)第四象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0108FF01", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上1结算日)第四象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0108FF02", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上2结算日)第四象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0108FF03", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上3结算日)第四象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0108FF04", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上4结算日)第四象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0108FF05", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上5结算日)第四象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0108FF06", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上6结算日)第四象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0108FF07", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上7结算日)第四象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0108FF08", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上8结算日)第四象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0108FF09", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上9结算日)第四象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");
            dt.Rows.Add("0108FF0A", 8, 0, "FFFFFFFF", EmSecurityMode.ClearText, 2, "(上10结算日)第四象限无功总最大需量及发生时间数据块", "XX.XXXXYYMMDDHHmm");



            dt.Rows.Add("02010100", 2, 1, "20000201", EmSecurityMode.ClearTextRand, 0, "A相电压", "XXX.X");
            dt.Rows.Add("02010200", 2, 1, "20000202", EmSecurityMode.ClearTextRand, 0, "B相电压", "XXX.X");
            dt.Rows.Add("02010300", 2, 1, "20000203", EmSecurityMode.ClearTextRand, 0, "C相电压", "XXX.X");

            dt.Rows.Add("0201FF00", 2, 1, "20000200", EmSecurityMode.ClearTextRand, 1, "电压数据块", "XXX.X");
            dt.Rows.Add("02020100", 3, 3, "20010201", EmSecurityMode.ClearTextRand, 0, "A相电流", "XXX.XXX");
            dt.Rows.Add("02020200", 3, 3, "20010202", EmSecurityMode.ClearTextRand, 0, "B相电流", "XXX.XXX");
            dt.Rows.Add("02020300", 3, 3, "20010203", EmSecurityMode.ClearTextRand, 0, "C相电流", "XXX.XXX");
            dt.Rows.Add("0202FF00", 3, 3, "20010200", EmSecurityMode.ClearTextRand, 3, "电流数据块", "XXX.XXX");
            dt.Rows.Add("02030000", 3, 4, "20040201", EmSecurityMode.ClearTextRand, 1, "瞬时总有功功率", "XX.XXXX");
            dt.Rows.Add("02030000", 3, 4, "20040202", EmSecurityMode.ClearTextRand, 1, "瞬时A相有功功率", "XX.XXXX");
            dt.Rows.Add("02050000", 3, 4, "FFFFFFFF", EmSecurityMode.ClearTextRand, 1, "瞬时总视在功率", "XX.XXXX");


            dt.Rows.Add("02060000", 2, 3, "200A0201", EmSecurityMode.ClearTextRand, 3, "总功率因数", "X.XXX");

            dt.Rows.Add("02800001", 3, 3, "20010400", EmSecurityMode.ClearTextRand, 3, "零线电流", "XXX.XXX");

            dt.Rows.Add("02800008", 2, 2, "20110200", EmSecurityMode.ClearTextRand, 2, "时钟电池电压", "XX.XX");
            dt.Rows.Add("02800009", 2, 2, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "停电抄表电池电压", "XX.XX");
            dt.Rows.Add("0280000B", 4, 4, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "当前阶梯电价", "XXXX.XXXX");
            dt.Rows.Add("02800020", 4, 4, "201A0200", EmSecurityMode.ClearTextRand, 4, "当前电价", "NNNN.NNNN");
            dt.Rows.Add("028000FE", 3, 4, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "内蒙当前电价", "NN.NNNN");

            dt.Rows.Add("03050000", 6, 0, "300D0400", EmSecurityMode.ClearTextRand, 0, "全失压总次数,总累计时间", "XXXXXX.XXXXXX");
            dt.Rows.Add("03050001", 15, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)全失压发生时刻,电流值,结束时刻", "YYMMDDhhmmssXXX.XXXYYMMDDhhmmss");
            dt.Rows.Add("03050002", 15, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)全失压发生时刻,电流值,结束时刻", "YYMMDDhhmmssXXX.XXXYYMMDDhhmmss");
            dt.Rows.Add("03050003", 15, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)全失压发生时刻,电流值,结束时刻", "YYMMDDhhmmssXXX.XXXYYMMDDhhmmss");
            dt.Rows.Add("03050004", 15, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)全失压发生时刻,电流值,结束时刻", "YYMMDDhhmmssXXX.XXXYYMMDDhhmmss");
            dt.Rows.Add("03050005", 15, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)全失压发生时刻,电流值,结束时刻", "YYMMDDhhmmssXXX.XXXYYMMDDhhmmss");
            dt.Rows.Add("03050006", 15, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)全失压发生时刻,电流值,结束时刻", "YYMMDDhhmmssXXX.XXXYYMMDDhhmmss");
            dt.Rows.Add("03050007", 15, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)全失压发生时刻,电流值,结束时刻", "YYMMDDhhmmssXXX.XXXYYMMDDhhmmss");
            dt.Rows.Add("03050008", 15, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)全失压发生时刻,电流值,结束时刻", "YYMMDDhhmmssXXX.XXXYYMMDDhhmmss");
            dt.Rows.Add("03050009", 15, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)全失压发生时刻,电流值,结束时刻", "YYMMDDhhmmssXXX.XXXYYMMDDhhmmss");
            dt.Rows.Add("0305000A", 15, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)全失压发生时刻,电流值,结束时刻", "YYMMDDhhmmssXXX.XXXYYMMDDhhmmss");

            dt.Rows.Add("030C0101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相过流记录", "YYMMDDhhmmss");

            dt.Rows.Add("03110000", 3, 0, "30110400", EmSecurityMode.ClearTextRand, 0, "掉电总次数", "XXXXXX");
            dt.Rows.Add("03110001", 12, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)掉电发生时刻,结束时刻", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03110002", 12, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)掉电发生时刻,结束时刻", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03110003", 12, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)掉电发生时刻,结束时刻", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03110004", 12, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)掉电发生时刻,结束时刻", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03110005", 12, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)掉电发生时刻,结束时刻", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03110006", 12, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)掉电发生时刻,结束时刻", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03110007", 12, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)掉电发生时刻,结束时刻", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03110008", 12, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)掉电发生时刻,结束时刻", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03110009", 12, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)掉电发生时刻,结束时刻", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("0311000A", 12, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)掉电发生时刻,结束时刻", "YYMMDDhhmmssYYMMDDhhmmss");

            dt.Rows.Add("03120000", 18, 0, "30090400", EmSecurityMode.ClearTextRand, 0, "需量超限总次数记录", "XXXXXX");
            dt.Rows.Add("03120101", 20, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)正向有功需量超限记录", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03120102", 20, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)正向有功需量超限记录", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03120103", 20, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)正向有功需量超限记录", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03120104", 20, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)正向有功需量超限记录", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03120105", 20, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)正向有功需量超限记录", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03120106", 20, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)正向有功需量超限记录", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03120107", 20, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)正向有功需量超限记录", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03120108", 20, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)正向有功需量超限记录", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("03120109", 20, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)正向有功需量超限记录", "YYMMDDhhmmssYYMMDDhhmmss");
            dt.Rows.Add("0312010A", 20, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)正向有功需量超限记录", "YYMMDDhhmmssYYMMDDhhmmss");


            dt.Rows.Add("03300000", 3, 0, "30120400", EmSecurityMode.ClearTextRand, 0, "编程总次数", "XXXXXX");
            dt.Rows.Add("03300001", 50, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)编程记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300002", 50, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)编程记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300003", 50, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)编程记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300004", 50, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)编程记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300005", 50, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)编程记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300006", 50, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)编程记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300007", 50, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)编程记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300008", 50, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)编程记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300009", 50, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)编程记录", "YYMMDDhhmmss");
            dt.Rows.Add("0330000A", 50, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)编程记录", "YYMMDDhhmmss");

            dt.Rows.Add("03300100", 3, 0, "30130400", EmSecurityMode.ClearTextRand, 0, "电表清零总次数", "XXXXXX");
            dt.Rows.Add("03300101", 106, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)电表清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300102", 106, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)电表清零记录", "YYMMDDhhmmss");

            dt.Rows.Add("03300200", 3, 0, "30140400", EmSecurityMode.ClearTextRand, 0, "需量清零总次数", "XXXXXX");
            dt.Rows.Add("03300201", 194, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)需量清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300202", 194, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)需量清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300203", 194, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)需量清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300204", 194, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)需量清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300205", 194, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)需量清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300206", 194, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)需量清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300207", 194, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)需量清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300208", 194, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)需量清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300209", 194, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)需量清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("0330020A", 194, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)需量清零记录", "YYMMDDhhmmss");

            dt.Rows.Add("03300300", 3, 0, "30150400", EmSecurityMode.ClearTextRand, 0, "事件清零总次数", "XXXXXX");
            dt.Rows.Add("03300301", 14, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)事件清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300302", 14, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)事件清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300303", 14, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)事件清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300304", 14, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)事件清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300305", 14, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)事件清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300306", 14, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)事件清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300307", 14, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)事件清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300308", 14, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)事件清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("03300309", 14, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)事件清零记录", "YYMMDDhhmmss");
            dt.Rows.Add("0330030A", 14, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)事件清零记录", "YYMMDDhhmmss");

            dt.Rows.Add("03300400", 3, 0, "30160400", EmSecurityMode.ClearTextRand, 0, "校时总次数", "XXXXXX");
            dt.Rows.Add("03300401", 16, 0, "30160200", EmSecurityMode.ClearText, 0, "(上1次)校时记录", "XXXXXX"); //311B,330D0200
            dt.Rows.Add("03300402", 16, 0, "FFFFFFFF", EmSecurityMode.ClearText, 0, "(上2次)校时记录", "XXXXXX");
            dt.Rows.Add("03300403", 16, 0, "FFFFFFFF", EmSecurityMode.ClearText, 0, "(上3次)校时记录", "XXXXXX");
            dt.Rows.Add("03300404", 16, 0, "FFFFFFFF", EmSecurityMode.ClearText, 0, "(上4次)校时记录", "XXXXXX");
            dt.Rows.Add("03300405", 16, 0, "FFFFFFFF", EmSecurityMode.ClearText, 0, "(上5次)校时记录", "XXXXXX");
            dt.Rows.Add("03300406", 16, 0, "FFFFFFFF", EmSecurityMode.ClearText, 0, "(上6次)校时记录", "XXXXXX");
            dt.Rows.Add("03300407", 16, 0, "FFFFFFFF", EmSecurityMode.ClearText, 0, "(上7次)校时记录", "XXXXXX");
            dt.Rows.Add("03300408", 16, 0, "FFFFFFFF", EmSecurityMode.ClearText, 0, "(上8次)校时记录", "XXXXXX");
            dt.Rows.Add("03300409", 16, 0, "FFFFFFFF", EmSecurityMode.ClearText, 0, "(上9次)校时记录", "XXXXXX");
            dt.Rows.Add("0330040A", 16, 0, "FFFFFFFF", EmSecurityMode.ClearText, 0, "(上10次)校时记录", "XXXXXX");

            dt.Rows.Add("03300600", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "时区表编程总次数", "XXXXXX");

            dt.Rows.Add("03300D00", 3, 0, "301B0400", EmSecurityMode.ClearTextRand, 0, "开表盖总次数", "XXXXXX");
            dt.Rows.Add("03300D01", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)开表盖记录", "XXXXXX");
            dt.Rows.Add("03300D02", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)开表盖记录", "XXXXXX");
            dt.Rows.Add("03300D03", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)开表盖记录", "XXXXXX");
            dt.Rows.Add("03300D04", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)开表盖记录", "XXXXXX");
            dt.Rows.Add("03300D05", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)开表盖记录", "XXXXXX");
            dt.Rows.Add("03300D06", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)开表盖记录", "XXXXXX");
            dt.Rows.Add("03300D07", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)开表盖记录", "XXXXXX");
            dt.Rows.Add("03300D08", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)开表盖记录", "XXXXXX");
            dt.Rows.Add("03300D09", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)开表盖记录", "XXXXXX");
            dt.Rows.Add("03300D0A", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)开表盖记录", "XXXXXX");

            dt.Rows.Add("03300E00", 3, 0, "301C0400", EmSecurityMode.ClearTextRand, 0, "开端钮盒总次数", "XXXXXX");
            dt.Rows.Add("03300E01", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)开端钮盒记录", "XXXXXX");
            dt.Rows.Add("03300E02", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)开端钮盒记录", "XXXXXX");
            dt.Rows.Add("03300E03", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)开端钮盒记录", "XXXXXX");
            dt.Rows.Add("03300E04", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)开端钮盒记录", "XXXXXX");
            dt.Rows.Add("03300E05", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)开端钮盒记录", "XXXXXX");
            dt.Rows.Add("03300E06", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)开端钮盒记录", "XXXXXX");
            dt.Rows.Add("03300E07", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)开端钮盒记录", "XXXXXX");
            dt.Rows.Add("03300E08", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)开端钮盒记录", "XXXXXX");
            dt.Rows.Add("03300E09", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)开端钮盒记录", "XXXXXX");
            dt.Rows.Add("03300E0A", 60, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)开端钮盒记录", "XXXXXX");

            dt.Rows.Add("30160200", 3, 0, "30160200", EmSecurityMode.ClearTextRand, 0, "电能表校时事件2", "XXXXXX");
            dt.Rows.Add("30160200", 3, 0, "30160600", EmSecurityMode.ClearTextRand, 0, "电能表校时事件6", "XXXXXX");

            dt.Rows.Add("04000101", 4, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "日期及星期", "YYMMDDWW");
            dt.Rows.Add("04000103", 2, 0, "41000200", EmSecurityMode.ClearTextRand, 0, "最大需量周期", "NN");
            dt.Rows.Add("04000104", 2, 0, "41010200", EmSecurityMode.ClearTextRand, 0, "滑差时间", "NN");

            dt.Rows.Add("04000106", 5, 0, "40080200", EmSecurityMode.ClearTextRand, 0, "两套时区表切换时间", "YYMMDDhhmm");
            dt.Rows.Add("04000107", 5, 0, "40090200", EmSecurityMode.ClearTextRand, 0, "两套日时段表切换时间", "YYMMDDhhmm");
            dt.Rows.Add("04000108", 5, 0, "400A0200", EmSecurityMode.ClearTextRand, 0, "两套费率电价切换时间", "YYMMDDhhmm");
            //dt.Rows.Add("04000108", 5, 0, "400A0200", EmSecurityMode.ClearTextRand, 0, "两套费率电价切换时间", "YYMMDDhhmm");

            dt.Rows.Add("04000109", 5, 0, "400B0200", EmSecurityMode.ClearTextRand, 0, "两套阶梯电价切换时间", "YYMMDDhhmm");
            dt.Rows.Add("0400010C", 0, 0, "40000200", EmSecurityMode.ClearText, 0, "日期时间", "YYMMDDWWhhmmss\\YYYYMMDDHHmmss");

            dt.Rows.Add("04000201", 1, 0, "400C0201", EmSecurityMode.ClearTextRand, 0, "年时区数", "NN");
            dt.Rows.Add("04000202", 1, 0, "400C0202", EmSecurityMode.ClearTextRand, 0, "日时段表数", "NN");
            dt.Rows.Add("04000203", 1, 0, "400C0203", EmSecurityMode.ClearTextRand, 0, "日时段数", "NN");
            dt.Rows.Add("04000204", 1, 0, "400C0204", EmSecurityMode.ClearTextRand, 0, "费率数", "NN");
            dt.Rows.Add("04000205", 2, 0, "400C0205", EmSecurityMode.ClearTextRand, 0, "公共假日数", "NNNN");
            dt.Rows.Add("04000207", 1, 0, "400D0200", EmSecurityMode.ClearTextRand, 0, "阶梯数", "NN");

            dt.Rows.Add("04000301", 1, 0, "F3000401", EmSecurityMode.ClearTextRand, 0, "自动循环显示屏数", "NN");
            //dt.Rows.Add("04000302", 1, 0, "F3000300", EmSecurityMode.ClearTextRand, 0, "每屏显示时间", "NN");
            dt.Rows.Add("04000302", 1, 0, "F3000300", EmSecurityMode.ClearTextRand, 0, "自动轮显每屏显示时间", "NN");
            dt.Rows.Add("FFFFFFFF", 1, 0, "F3010300", EmSecurityMode.ClearTextRand, 0, "按键轮显每屏显示时间", "NN");

            dt.Rows.Add("04000308", 1, 0, "40070201", EmSecurityMode.ClearTextRand, 0, "上电全显时间", "NN");
            dt.Rows.Add("FFFFFFFF", 1, 0, "40070202", EmSecurityMode.ClearTextRand, 0, "背光点亮时间", "NN");
            dt.Rows.Add("FFFFFFFF", 1, 0, "40070203", EmSecurityMode.ClearTextRand, 0, "显示查看背光亮点时间", "NN");
            dt.Rows.Add("FFFFFFFF", 1, 0, "40070204", EmSecurityMode.ClearTextRand, 0, "无电按键屏幕驻留最大时间", "NN");
            dt.Rows.Add("04000303", 1, 0, "40070205", EmSecurityMode.ClearTextRand, 0, "显示电能小数位数", "NN");
            dt.Rows.Add("04000304", 1, 0, "40070206", EmSecurityMode.ClearTextRand, 0, "显示功率(最大需量)小数位数", "NN");
            dt.Rows.Add("04000305", 5, 0, "F3010401", EmSecurityMode.ClearTextRand, 0, "按键显示屏数", "NN");

            dt.Rows.Add("04000401", 6, 0, "40010200", EmSecurityMode.ClearTextRand, 0, "通信地址", "NNNNNNNNNNNN");
            dt.Rows.Add("04000401", 6, 0, "40010200", EmSecurityMode.ClearTextRand, 0, "通讯地址", "NNNNNNNNNNNN");

            dt.Rows.Add("04000402", 6, 0, "40020200", EmSecurityMode.ClearTextRand, 0, "表号", "NNNNNNNNNNNN");
            dt.Rows.Add("07010104", 6, 0, "40030200", EmSecurityMode.ClearTextRand, 0, "客户编号", "NNNNNNNNNNNN");


            dt.Rows.Add("04000403", 32, 0, "41030200", EmSecurityMode.ClearTextRand, 0, "资产编号", "NN…NN");

            dt.Rows.Add("04000404", 6, 0, "41040200", EmSecurityMode.ClearTextRand, 0, "额定电压", "XXXXXXXX");
            dt.Rows.Add("04000405", 6, 0, "41050200", EmSecurityMode.ClearTextRand, 0, "额定电流", "XXXXXXXX");
            dt.Rows.Add("04000406", 6, 0, "41060200", EmSecurityMode.ClearTextRand, 0, "最大电流", "XXXXXXXX");

            dt.Rows.Add("04000407", 6, 0, "41070200", EmSecurityMode.ClearTextRand, 0, "有功等级", "XXXXXXXX");
            dt.Rows.Add("04000409", 3, 0, "41090200", EmSecurityMode.ClearTextRand, 0, "有功常数", "XXXXXX");
            dt.Rows.Add("0400040B", 10, 0, "410B0200", EmSecurityMode.ClearTextRand, 0, "电表型号", "XX…XX");
            dt.Rows.Add("0400040C", 10, 0, "43000400", EmSecurityMode.ClearTextRand, 0, "生产日期", "XX…XX");

            dt.Rows.Add("04000501", 2, 0, "20140201", EmSecurityMode.ClearTextRand, 0, "电表运行状态字1", "XXXX");
            dt.Rows.Add("04000502", 2, 0, "20140202", EmSecurityMode.ClearTextRand, 0, "电表运行状态字2", "XXXX");
            dt.Rows.Add("04000503", 2, 0, "20140203", EmSecurityMode.ClearTextRand, 0, "电表运行状态字3", "XXXX");//如698时 0104 == 00000001 -- 00000100 == bit0-bit7,bit8--bit15
            dt.Rows.Add("04000504", 2, 0, "20140204", EmSecurityMode.ClearTextRand, 0, "电表运行状态字4", "XXXX");
            dt.Rows.Add("04000505", 2, 0, "20140205", EmSecurityMode.ClearTextRand, 0, "电表运行状态字5", "XXXX");
            dt.Rows.Add("04000506", 2, 0, "20140206", EmSecurityMode.ClearTextRand, 0, "电表运行状态字6", "XXXX");
            dt.Rows.Add("04000507", 2, 0, "20140207", EmSecurityMode.ClearTextRand, 0, "电表运行状态字7", "XXXX");
            dt.Rows.Add("04000508", 4, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "密钥状态", "XXXXXXXX");

            dt.Rows.Add("04000601", 1, 0, "41120200", EmSecurityMode.ClearTextRand, 0, "有功组合方式特征字", "NN");
            dt.Rows.Add("04000602", 1, 0, "41130200", EmSecurityMode.ClearTextRand, 0, "无功组合方式1特征字", "NN");
            dt.Rows.Add("04000603", 1, 0, "41140200", EmSecurityMode.ClearTextRand, 0, "无功组合方式2特征字", "NN");

            dt.Rows.Add("04000701", 1, 0, "F2020201", EmSecurityMode.ClearTextRand, 0, "调制型红外光口波特率特征字", "NN");  //接口22  调制型红外光口通信速率特征字
            dt.Rows.Add("04000702", 1, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "接触式红外光口波特率特征字", "NN");  //接口22
            dt.Rows.Add("04000703", 1, 0, "F2010201", EmSecurityMode.ClearTextRand, 0, "通信口1波特率特征字", "NN");// 接口22
            dt.Rows.Add("04000704", 1, 0, "F2010202", EmSecurityMode.ClearTextRand, 0, "通信口2波特率特征字", "NN");// 接口22
            dt.Rows.Add("04000705", 1, 0, "F2090201", EmSecurityMode.ClearTextRand, 0, "通信口3波特率特征字", "NN");// 接口22

            dt.Rows.Add("04000901", 1, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "负荷记录模式字", "NN");
            dt.Rows.Add("04000902", 1, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "定时冻结数据模式字", "NN");
            dt.Rows.Add("04000903", 1, 0, "50000300", EmSecurityMode.ClearTextRand, 0, "瞬时冻结数据模式字", "NN");
            dt.Rows.Add("04000904", 1, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "约定冻结数据模式字", "NN");
            dt.Rows.Add("04000905", 1, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "整点冻结数据模式字", "NN");
            dt.Rows.Add("04000906", 1, 0, "50040300", EmSecurityMode.ClearTextRand, 0, "日冻结数据模式字", "NN");

            dt.Rows.Add("04000A01", 4, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "负荷记录起始时间", "YYMMDDhhmm");
            dt.Rows.Add("04000A02", 2, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "第1类负荷记录间隔时间", "NN");
            dt.Rows.Add("04000A03", 2, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "第2类负荷记录间隔时间", "NN");
            dt.Rows.Add("04000A04", 2, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "第3类负荷记录间隔时间", "NN");
            dt.Rows.Add("04000A05", 2, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "第4类负荷记录间隔时间", "NN");
            dt.Rows.Add("04000A06", 2, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "第5类负荷记录间隔时间", "NN");
            dt.Rows.Add("04000A07", 2, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "第6类负荷记录间隔时间", "NN");

            dt.Rows.Add("04000B01", 2, 0, "41160201", EmSecurityMode.ClearTextRand, 0, "每月第1结算日", "DDhh");
            dt.Rows.Add("04000B01", 2, 0, "41160201", EmSecurityMode.ClearTextRand, 0, "每月第一结算日", "DDhh");
            dt.Rows.Add("04000B02", 2, 0, "41160202", EmSecurityMode.ClearTextRand, 0, "每月第2结算日", "DDhh");
            dt.Rows.Add("04000B03", 2, 0, "41160203", EmSecurityMode.ClearTextRand, 0, "每月第3结算日", "DDhh");

            dt.Rows.Add("FFFFFFFF", 4, 2, "401E0200", EmSecurityMode.ClearTextRand, 2, "报警金额", "XXXXXX.XX");
            dt.Rows.Add("04001001", 4, 2, "401E0201", EmSecurityMode.ClearTextRand, 2, "报警金额1限值", "XXXXXX.XX");
            dt.Rows.Add("04001002", 4, 2, "401E0202", EmSecurityMode.ClearTextRand, 2, "报警金额2限值", "XXXXXX.XX");

            dt.Rows.Add("04001101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "电表运行特征字1", "NN");
            dt.Rows.Add("04001104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "主动上报模式字", "XXXXXXXXXXXX");

            dt.Rows.Add("04001201", 5, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "整点冻结起始时间", "YYMMDDhhmm");
            dt.Rows.Add("04001202", 1, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "整点冻结时间间隔", "NN");
            dt.Rows.Add("04001203", 2, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "日冻结时间", "hhmm");
            dt.Rows.Add("04001204", 4, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "定时冻结时间", "MMDDhhmm");
            dt.Rows.Add("04001401", 2, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "跳闸延时时间", "NNNN");

            dt.Rows.Add("04001501", 12, 0, "202F0200", EmSecurityMode.ClearTextRand, 0, "主动上报状态字", "XXXXXXXXXXXXXXXXXXXXXXXX"); //未验证
            dt.Rows.Add("04001503", 2, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "跳闸延时时间", "NNNN");

            dt.Rows.Add("04010000", 12, 0, "40140200", EmSecurityMode.ClearTextRand, 0, "第一套时区表数据", "MMDDNN...MMDDNN"); //当前套日时段表
            dt.Rows.Add("04010001", 42, 0, "40160201", EmSecurityMode.ClearTextRand, 0, "第一套第1日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表
            dt.Rows.Add("04010002", 42, 0, "40160202", EmSecurityMode.ClearTextRand, 0, "第一套第2日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表
            dt.Rows.Add("04010003", 42, 0, "40160203", EmSecurityMode.ClearTextRand, 0, "第一套第3日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表
            dt.Rows.Add("04010004", 42, 0, "40160204", EmSecurityMode.ClearTextRand, 0, "第一套第4日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表
            dt.Rows.Add("04010005", 42, 0, "40160205", EmSecurityMode.ClearTextRand, 0, "第一套第5日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表
            dt.Rows.Add("04010006", 42, 0, "40160206", EmSecurityMode.ClearTextRand, 0, "第一套第6日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表
            dt.Rows.Add("04010007", 42, 0, "40160207", EmSecurityMode.ClearTextRand, 0, "第一套第7日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表
            dt.Rows.Add("04010008", 42, 0, "40160208", EmSecurityMode.ClearTextRand, 0, "第一套第8日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表

            dt.Rows.Add("04020000", 42, 0, "40150200", EmSecurityMode.ClearTextRand, 0, "第二套时区表数据", "MMDDNN...MMDDNN"); //当前套日时段表
            dt.Rows.Add("04020001", 42, 0, "40170201", EmSecurityMode.ClearTextRand, 0, "第二套第1日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表
            dt.Rows.Add("04020002", 42, 0, "40170202", EmSecurityMode.ClearTextRand, 0, "第二套第2日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表
            dt.Rows.Add("04020003", 42, 0, "40170203", EmSecurityMode.ClearTextRand, 0, "第二套第3日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表
            dt.Rows.Add("04020004", 42, 0, "40170204", EmSecurityMode.ClearTextRand, 0, "第二套第4日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表
            dt.Rows.Add("04020005", 42, 0, "40170205", EmSecurityMode.ClearTextRand, 0, "第二套第5日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表
            dt.Rows.Add("04020006", 42, 0, "40170206", EmSecurityMode.ClearTextRand, 0, "第二套第6日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表
            dt.Rows.Add("04020007", 42, 0, "40170207", EmSecurityMode.ClearTextRand, 0, "第二套第7日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表
            dt.Rows.Add("04020008", 42, 0, "40170208", EmSecurityMode.ClearTextRand, 0, "第二套第8日时段数据", "hhmmNN...hhmmNN"); //当前套日时段表


            dt.Rows.Add("04040101", 5, 0, "F3000201", EmSecurityMode.ClearTextRand, 0, "自动循环显示第1屏", "NNNNNNNNNN");
            dt.Rows.Add("04040102", 5, 0, "F3000202", EmSecurityMode.ClearTextRand, 0, "自动循环显示第2屏", "NNNNNNNNNN");
            dt.Rows.Add("04040103", 5, 0, "F3000203", EmSecurityMode.ClearTextRand, 0, "自动循环显示第3屏", "NNNNNNNNNN");
            dt.Rows.Add("04040104", 5, 0, "F3000204", EmSecurityMode.ClearTextRand, 0, "自动循环显示第4屏", "NNNNNNNNNN");
            dt.Rows.Add("04040105", 5, 0, "F3000205", EmSecurityMode.ClearTextRand, 0, "自动循环显示第5屏", "NNNNNNNNNN");
            dt.Rows.Add("04040106", 5, 0, "F3000206", EmSecurityMode.ClearTextRand, 0, "自动循环显示第6屏", "NNNNNNNNNN");
            dt.Rows.Add("04040107", 5, 0, "F3000207", EmSecurityMode.ClearTextRand, 0, "自动循环显示第7屏", "NNNNNNNNNN");
            dt.Rows.Add("04040108", 5, 0, "F3000208", EmSecurityMode.ClearTextRand, 0, "自动循环显示第8屏", "NNNNNNNNNN");
            dt.Rows.Add("04040109", 5, 0, "F3000209", EmSecurityMode.ClearTextRand, 0, "自动循环显示第9屏", "NNNNNNNNNN");
            dt.Rows.Add("0404010A", 5, 0, "F300020A", EmSecurityMode.ClearTextRand, 0, "自动循环显示第10屏", "NNNNNNNNNN");
            dt.Rows.Add("0404010B", 5, 0, "F300020B", EmSecurityMode.ClearTextRand, 0, "自动循环显示第11屏", "NNNNNNNNNN");
            dt.Rows.Add("0404010C", 5, 0, "F300020C", EmSecurityMode.ClearTextRand, 0, "自动循环显示第12屏", "NNNNNNNNNN");
            dt.Rows.Add("0404010D", 5, 0, "F300020D", EmSecurityMode.ClearTextRand, 0, "自动循环显示第13屏", "NNNNNNNNNN");
            dt.Rows.Add("0404010E", 5, 0, "F300020E", EmSecurityMode.ClearTextRand, 0, "自动循环显示第14屏", "NNNNNNNNNN");
            dt.Rows.Add("0404010F", 5, 0, "F300020F", EmSecurityMode.ClearTextRand, 0, "自动循环显示第15屏", "NNNNNNNNNN");
            dt.Rows.Add("04040110", 5, 0, "F3000210", EmSecurityMode.ClearTextRand, 0, "自动循环显示第16屏", "NNNNNNNNNN");
            dt.Rows.Add("04040111", 5, 0, "F3000211", EmSecurityMode.ClearTextRand, 0, "自动循环显示第17屏", "NNNNNNNNNN");
            dt.Rows.Add("04040112", 4, 0, "F3000212", EmSecurityMode.ClearTextRand, 0, "自动循环显示第18屏", "NNNNNNNNNN");


            dt.Rows.Add("04040201", 5, 0, "F3010201", EmSecurityMode.ClearTextRand, 0, "按键循环显示第1屏", "NNNNNNNNNN");
            dt.Rows.Add("04040202", 5, 0, "F3010202", EmSecurityMode.ClearTextRand, 0, "按键循环显示第2屏", "NNNNNNNNNN");
            dt.Rows.Add("04040203", 5, 0, "F3010203", EmSecurityMode.ClearTextRand, 0, "按键循环显示第3屏", "NNNNNNNNNN");
            dt.Rows.Add("04040204", 5, 0, "F3010204", EmSecurityMode.ClearTextRand, 0, "按键循环显示第4屏", "NNNNNNNNNN");
            dt.Rows.Add("04040205", 5, 0, "F3010205", EmSecurityMode.ClearTextRand, 0, "按键循环显示第5屏", "NNNNNNNNNN");
            dt.Rows.Add("04040206", 5, 0, "F3010206", EmSecurityMode.ClearTextRand, 0, "按键循环显示第6屏", "NNNNNNNNNN");
            dt.Rows.Add("04040207", 5, 0, "F3010207", EmSecurityMode.ClearTextRand, 0, "按键循环显示第7屏", "NNNNNNNNNN");
            dt.Rows.Add("04040208", 5, 0, "F3010208", EmSecurityMode.ClearTextRand, 0, "按键循环显示第8屏", "NNNNNNNNNN");
            dt.Rows.Add("04040209", 5, 0, "F3010209", EmSecurityMode.ClearTextRand, 0, "按键循环显示第9屏", "NNNNNNNNNN");
            dt.Rows.Add("0404020A", 5, 0, "F301020A", EmSecurityMode.ClearTextRand, 0, "按键循环显示第10屏", "NNNNNNNNNN");
            dt.Rows.Add("0404020B", 5, 0, "F301020B", EmSecurityMode.ClearTextRand, 0, "按键循环显示第11屏", "NNNNNNNNNN");
            dt.Rows.Add("0404020C", 5, 0, "F301020C", EmSecurityMode.ClearTextRand, 0, "按键循环显示第12屏", "NNNNNNNNNN");
            dt.Rows.Add("0404020D", 5, 0, "F301020D", EmSecurityMode.ClearTextRand, 0, "按键循环显示第13屏", "NNNNNNNNNN");
            dt.Rows.Add("0404020E", 5, 0, "F301020E", EmSecurityMode.ClearTextRand, 0, "按键循环显示第14屏", "NNNNNNNNNN");
            dt.Rows.Add("0404020F", 5, 0, "F301020F", EmSecurityMode.ClearTextRand, 0, "按键循环显示第15屏", "NNNNNNNNNN");
            dt.Rows.Add("04040210", 5, 0, "F3010210", EmSecurityMode.ClearTextRand, 0, "按键循环显示第16屏", "NNNNNNNNNN");
            dt.Rows.Add("04040211", 5, 0, "F3010211", EmSecurityMode.ClearTextRand, 0, "按键循环显示第17屏", "NNNNNNNNNN");
            dt.Rows.Add("04040212", 5, 0, "F3010212", EmSecurityMode.ClearTextRand, 0, "按键循环显示第18屏", "NNNNNNNNNN");
            dt.Rows.Add("04040213", 5, 0, "F3010213", EmSecurityMode.ClearTextRand, 0, "按键循环显示第19屏", "NNNNNNNNNN");
            dt.Rows.Add("04040214", 5, 0, "F3010214", EmSecurityMode.ClearTextRand, 0, "按键循环显示第20屏", "NNNNNNNNNN");
            dt.Rows.Add("04040215", 5, 0, "F3010215", EmSecurityMode.ClearTextRand, 0, "按键循环显示第21屏", "NNNNNNNNNN");
            dt.Rows.Add("04040216", 5, 0, "F3010216", EmSecurityMode.ClearTextRand, 0, "按键循环显示第22屏", "NNNNNNNNNN");
            dt.Rows.Add("04040217", 5, 0, "F3010217", EmSecurityMode.ClearTextRand, 0, "按键循环显示第23屏", "NNNNNNNNNN");
            dt.Rows.Add("04040218", 5, 0, "F3010218", EmSecurityMode.ClearTextRand, 0, "按键循环显示第24屏", "NNNNNNNNNN");
            dt.Rows.Add("04040219", 5, 0, "F3010219", EmSecurityMode.ClearTextRand, 0, "按键循环显示第25屏", "NNNNNNNNNN");
            dt.Rows.Add("0404021A", 5, 0, "F301021A", EmSecurityMode.ClearTextRand, 0, "按键循环显示第26屏", "NNNNNNNNNN");
            dt.Rows.Add("0404021B", 5, 0, "F301021B", EmSecurityMode.ClearTextRand, 0, "按键循环显示第27屏", "NNNNNNNNNN");
            dt.Rows.Add("0404021C", 5, 0, "F301021C", EmSecurityMode.ClearTextRand, 0, "按键循环显示第28屏", "NNNNNNNNNN");
            dt.Rows.Add("0404021D", 5, 0, "F301021D", EmSecurityMode.ClearTextRand, 0, "按键循环显示第29屏", "NNNNNNNNNN");
            dt.Rows.Add("0404021E", 5, 0, "F301021E", EmSecurityMode.ClearTextRand, 0, "按键循环显示第30屏", "NNNNNNNNNN");
            dt.Rows.Add("0404021F", 5, 0, "F301021F", EmSecurityMode.ClearTextRand, 0, "按键循环显示第31屏", "NNNNNNNNNN");
            dt.Rows.Add("04040220", 5, 0, "F3010220", EmSecurityMode.ClearTextRand, 0, "按键循环显示第32屏", "NNNNNNNNNN");
            dt.Rows.Add("04040221", 5, 0, "F3010221", EmSecurityMode.ClearTextRand, 0, "按键循环显示第33屏", "NNNNNNNNNN");
            dt.Rows.Add("04040222", 5, 0, "F3010222", EmSecurityMode.ClearTextRand, 0, "按键循环显示第34屏", "NNNNNNNNNN");
            dt.Rows.Add("04040223", 5, 0, "F3010223", EmSecurityMode.ClearTextRand, 0, "按键循环显示第35屏", "NNNNNNNNNN");
            dt.Rows.Add("04040224", 5, 0, "F3010224", EmSecurityMode.ClearTextRand, 0, "按键循环显示第36屏", "NNNNNNNNNN");
            dt.Rows.Add("04040225", 5, 0, "F3010225", EmSecurityMode.ClearTextRand, 0, "按键循环显示第37屏", "NNNNNNNNNN");
            dt.Rows.Add("04040226", 5, 0, "F3010226", EmSecurityMode.ClearTextRand, 0, "按键循环显示第38屏", "NNNNNNNNNN");
            dt.Rows.Add("04040227", 5, 0, "F3010227", EmSecurityMode.ClearTextRand, 0, "按键循环显示第39屏", "NNNNNNNNNN");
            dt.Rows.Add("04040228", 5, 0, "F3010228", EmSecurityMode.ClearTextRand, 0, "按键循环显示第40屏", "NNNNNNNNNN");
            dt.Rows.Add("04040229", 5, 0, "F3010229", EmSecurityMode.ClearTextRand, 0, "按键循环显示第41屏", "NNNNNNNNNN");
            dt.Rows.Add("0404022A", 5, 0, "F301022A", EmSecurityMode.ClearTextRand, 0, "按键循环显示第42屏", "NNNNNNNNNN");
            dt.Rows.Add("0404022B", 5, 0, "F301022B", EmSecurityMode.ClearTextRand, 0, "按键循环显示第43屏", "NNNNNNNNNN");
            dt.Rows.Add("0404022C", 5, 0, "F301022C", EmSecurityMode.ClearTextRand, 0, "按键循环显示第44屏", "NNNNNNNNNN");
            dt.Rows.Add("0404022D", 5, 0, "F301022D", EmSecurityMode.ClearTextRand, 0, "按键循环显示第45屏", "NNNNNNNNNN");
            dt.Rows.Add("0404022E", 5, 0, "F301022E", EmSecurityMode.ClearTextRand, 0, "按键循环显示第46屏", "NNNNNNNNNN");
            dt.Rows.Add("0404022F", 5, 0, "F301022F", EmSecurityMode.ClearTextRand, 0, "按键循环显示第47屏", "NNNNNNNNNN");
            dt.Rows.Add("04040230", 5, 0, "F3010230", EmSecurityMode.ClearTextRand, 0, "按键循环显示第48屏", "NNNNNNNNNN");
            dt.Rows.Add("04040231", 5, 0, "F3010231", EmSecurityMode.ClearTextRand, 0, "按键循环显示第49屏", "NNNNNNNNNN");
            dt.Rows.Add("04040232", 5, 0, "F3010232", EmSecurityMode.ClearTextRand, 0, "按键循环显示第50屏", "NNNNNNNNNN");
            dt.Rows.Add("04040233", 5, 0, "F3010233", EmSecurityMode.ClearTextRand, 0, "按键循环显示第51屏", "NNNNNNNNNN");
            dt.Rows.Add("04040234", 5, 0, "F3010234", EmSecurityMode.ClearTextRand, 0, "按键循环显示第52屏", "NNNNNNNNNN");
            dt.Rows.Add("04040235", 5, 0, "F3010235", EmSecurityMode.ClearTextRand, 0, "按键循环显示第53屏", "NNNNNNNNNN");
            dt.Rows.Add("04040236", 5, 0, "F3010236", EmSecurityMode.ClearTextRand, 0, "按键循环显示第54屏", "NNNNNNNNNN");
            dt.Rows.Add("04040237", 5, 0, "F3010237", EmSecurityMode.ClearTextRand, 0, "按键循环显示第55屏", "NNNNNNNNNN");
            dt.Rows.Add("04040238", 5, 0, "F3010238", EmSecurityMode.ClearTextRand, 0, "按键循环显示第56屏", "NNNNNNNNNN");
            dt.Rows.Add("04040239", 5, 0, "F3010239", EmSecurityMode.ClearTextRand, 0, "按键循环显示第57屏", "NNNNNNNNNN");
            dt.Rows.Add("0404023A", 5, 0, "F301023A", EmSecurityMode.ClearTextRand, 0, "按键循环显示第58屏", "NNNNNNNNNN");
            dt.Rows.Add("0404023B", 5, 0, "F301023B", EmSecurityMode.ClearTextRand, 0, "按键循环显示第59屏", "NNNNNNNNNN");
            dt.Rows.Add("0404023C", 5, 0, "F301023C", EmSecurityMode.ClearTextRand, 0, "按键循环显示第60屏", "NNNNNNNNNN");
            dt.Rows.Add("0404023D", 5, 0, "F301023D", EmSecurityMode.ClearTextRand, 0, "按键循环显示第61屏", "NNNNNNNNNN");
            dt.Rows.Add("0404023E", 5, 0, "F301023E", EmSecurityMode.ClearTextRand, 0, "按键循环显示第62屏", "NNNNNNNNNN");
            dt.Rows.Add("0404023F", 5, 0, "F301023F", EmSecurityMode.ClearTextRand, 0, "按键循环显示第63屏", "NNNNNNNNNN");
            dt.Rows.Add("04040240", 5, 0, "F3010240", EmSecurityMode.ClearTextRand, 0, "按键循环显示第64屏", "NNNNNNNNNN");
            dt.Rows.Add("04040241", 5, 0, "F3010241", EmSecurityMode.ClearTextRand, 0, "按键循环显示第65屏", "NNNNNNNNNN");
            dt.Rows.Add("04040242", 5, 0, "F3010242", EmSecurityMode.ClearTextRand, 0, "按键循环显示第66屏", "NNNNNNNNNN");
            dt.Rows.Add("04040243", 5, 0, "F3010243", EmSecurityMode.ClearTextRand, 0, "按键循环显示第67屏", "NNNNNNNNNN");
            dt.Rows.Add("04040244", 5, 0, "F3010244", EmSecurityMode.ClearTextRand, 0, "按键循环显示第68屏", "NNNNNNNNNN");

            dt.Rows.Add("04040245", 5, 0, "F3010245", EmSecurityMode.ClearTextRand, 0, "按键循环显示第69屏", "NNNNNNNNNN");
            dt.Rows.Add("04040246", 5, 0, "F3010246", EmSecurityMode.ClearTextRand, 0, "按键循环显示第70屏", "NNNNNNNNNN");
            dt.Rows.Add("04040247", 5, 0, "F3010247", EmSecurityMode.ClearTextRand, 0, "按键循环显示第71屏", "NNNNNNNNNN");
            dt.Rows.Add("04040248", 5, 0, "F3010248", EmSecurityMode.ClearTextRand, 0, "按键循环显示第72屏", "NNNNNNNNNN");
            dt.Rows.Add("04040249", 5, 0, "F3010249", EmSecurityMode.ClearTextRand, 0, "按键循环显示第73屏", "NNNNNNNNNN");
            dt.Rows.Add("0404024A", 5, 0, "F301024A", EmSecurityMode.ClearTextRand, 0, "按键循环显示第74屏", "NNNNNNNNNN");
            dt.Rows.Add("0404024B", 5, 0, "F301024B", EmSecurityMode.ClearTextRand, 0, "按键循环显示第75屏", "NNNNNNNNNN");
            dt.Rows.Add("0404024C", 5, 0, "F301024C", EmSecurityMode.ClearTextRand, 0, "按键循环显示第76屏", "NNNNNNNNNN");
            dt.Rows.Add("0404024D", 5, 0, "F301024D", EmSecurityMode.ClearTextRand, 0, "按键循环显示第77屏", "NNNNNNNNNN");
            dt.Rows.Add("0404024E", 5, 0, "F301024E", EmSecurityMode.ClearTextRand, 0, "按键循环显示第78屏", "NNNNNNNNNN");
            dt.Rows.Add("0404024F", 5, 0, "F301024F", EmSecurityMode.ClearTextRand, 0, "按键循环显示第79屏", "NNNNNNNNNN");
            dt.Rows.Add("04040250", 5, 0, "F3010250", EmSecurityMode.ClearTextRand, 0, "按键循环显示第80屏", "NNNNNNNNNN");
            dt.Rows.Add("04040251", 5, 0, "F3010251", EmSecurityMode.ClearTextRand, 0, "按键循环显示第81屏", "NNNNNNNNNN");
            dt.Rows.Add("04040252", 5, 0, "F3010252", EmSecurityMode.ClearTextRand, 0, "按键循环显示第82屏", "NNNNNNNNNN");
            dt.Rows.Add("04040253", 5, 0, "F3010253", EmSecurityMode.ClearTextRand, 0, "按键循环显示第83屏", "NNNNNNNNNN");
            dt.Rows.Add("04040254", 5, 0, "F3010254", EmSecurityMode.ClearTextRand, 0, "按键循环显示第84屏", "NNNNNNNNNN");

            dt.Rows.Add("FFFFFFFF", 4, 4, "401A0201", EmSecurityMode.ClearTextRand, 0, "第一套阶梯电价1", "NNNN.NNNN"); //第一套阶梯电价1
            dt.Rows.Add("FFFFFFFF", 4, 4, "401A0202", EmSecurityMode.ClearTextRand, 0, "第一套阶梯电价2", "NNNN.NNNN"); //第一套阶梯电价
            dt.Rows.Add("FFFFFFFF", 4, 4, "401A0203", EmSecurityMode.ClearTextRand, 0, "第一套阶梯电价3", "NNNN.NNNN"); //第一套阶梯电价
            dt.Rows.Add("FFFFFFFF", 4, 4, "401A0204", EmSecurityMode.ClearTextRand, 0, "第一套阶梯电价4", "NNNN.NNNN"); //第一套阶梯电价
            dt.Rows.Add("FFFFFFFF", 4, 4, "401A0205", EmSecurityMode.ClearTextRand, 0, "第一套阶梯电价5", "NNNN.NNNN"); //第一套阶梯电价
            dt.Rows.Add("FFFFFFFF", 4, 4, "401A0206", EmSecurityMode.ClearTextRand, 0, "第一套阶梯电价6", "NNNN.NNNN"); //第一套阶梯电价
            dt.Rows.Add("FFFFFFFF", 4, 4, "401A0207", EmSecurityMode.ClearTextRand, 0, "第一套阶梯电价7", "NNNN.NNNN"); //第一套阶梯电价
            dt.Rows.Add("FFFFFFFF", 4, 4, "401A0208", EmSecurityMode.ClearTextRand, 0, "第一套阶梯电价8", "NNNN.NNNN"); //第一套阶梯电价
            dt.Rows.Add("FFFFFFFF", 4, 0, "401A0200", EmSecurityMode.ClearTextRand, 0, "第一套阶梯电价", "NNNN.NNNN"); //第一套阶梯电价

            dt.Rows.Add("FFFFFFFF", 4, 4, "401B0201", EmSecurityMode.ClearTextRand, 0, "第二套阶梯电价1", "NNNN.NNNN"); //第一套阶梯电价1
            dt.Rows.Add("FFFFFFFF", 4, 4, "401B0202", EmSecurityMode.ClearTextRand, 0, "第二套阶梯电价2", "NNNN.NNNN"); //第一套阶梯电价
            dt.Rows.Add("FFFFFFFF", 4, 4, "401B0203", EmSecurityMode.ClearTextRand, 0, "第二套阶梯电价3", "NNNN.NNNN"); //第一套阶梯电价
            dt.Rows.Add("FFFFFFFF", 4, 4, "401B0204", EmSecurityMode.ClearTextRand, 0, "第二套阶梯电价4", "NNNN.NNNN"); //第一套阶梯电价
            dt.Rows.Add("FFFFFFFF", 4, 4, "401B0205", EmSecurityMode.ClearTextRand, 0, "第二套阶梯电价5", "NNNN.NNNN"); //第一套阶梯电价
            dt.Rows.Add("FFFFFFFF", 4, 4, "401B0206", EmSecurityMode.ClearTextRand, 0, "第二套阶梯电价6", "NNNN.NNNN"); //第一套阶梯电价
            dt.Rows.Add("FFFFFFFF", 4, 4, "401B0207", EmSecurityMode.ClearTextRand, 0, "第二套阶梯电价7", "NNNN.NNNN"); //第一套阶梯电价
            dt.Rows.Add("FFFFFFFF", 4, 4, "401B0208", EmSecurityMode.ClearTextRand, 0, "第二套阶梯电价8", "NNNN.NNNN"); //第一套阶梯电价
            dt.Rows.Add("FFFFFFFF", 4, 0, "401B0200", EmSecurityMode.ClearTextRand, 0, "第二套阶梯电价", "NNNN.NNNN"); //第一套阶梯电价

            dt.Rows.Add("04050101", 4, 4, "40180201", EmSecurityMode.ClearTextRand, 4, "第一套费率电价1", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("04050102", 4, 4, "40180202", EmSecurityMode.ClearTextRand, 4, "第一套费率电价2", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("04050103", 4, 4, "40180203", EmSecurityMode.ClearTextRand, 4, "第一套费率电价3", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("04050104", 4, 4, "40180204", EmSecurityMode.ClearTextRand, 4, "第一套费率电价4", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("04050105", 4, 4, "40180205", EmSecurityMode.ClearTextRand, 4, "第一套费率电价5", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("04050106", 4, 4, "40180206", EmSecurityMode.ClearTextRand, 4, "第一套费率电价6", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("04050107", 4, 4, "40180207", EmSecurityMode.ClearTextRand, 4, "第一套费率电价7", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("04050108", 4, 4, "40180208", EmSecurityMode.ClearTextRand, 4, "第一套费率电价8", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("040501FF", 92, 0, "40180200", EmSecurityMode.ClearTextRand, 0, "第一套费率电价", "NNNN.NNNN"); //当前套费率电价

            dt.Rows.Add("04050201", 4, 4, "40190201", EmSecurityMode.ClearTextRand, 4, "第二套费率电价1", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("04050202", 4, 4, "40190202", EmSecurityMode.ClearTextRand, 4, "第二套费率电价2", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("04050203", 4, 4, "40190203", EmSecurityMode.ClearTextRand, 4, "第二套费率电价3", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("04050204", 4, 4, "40190204", EmSecurityMode.ClearTextRand, 4, "第二套费率电价4", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("04050205", 4, 4, "40190205", EmSecurityMode.ClearTextRand, 4, "第二套费率电价5", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("04050206", 4, 4, "40190206", EmSecurityMode.ClearTextRand, 4, "第二套费率电价6", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("04050207", 4, 4, "40190207", EmSecurityMode.ClearTextRand, 4, "第二套费率电价7", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("04050208", 4, 4, "40190208", EmSecurityMode.ClearTextRand, 4, "第二套费率电价8", "NNNN.NNNN"); //当前套费率电价
            dt.Rows.Add("040502FF", 4, 0, "40190200", EmSecurityMode.ClearTextRand, 0, "第二套费率电价", "NNNN.NNNN"); //当前套费率电价

            dt.Rows.Add("040604FF", 64, 4, "FFFFFFFF", EmSecurityMode.ClearTextRand, 4, "第一套阶梯参数数据块", "NNNN.NNNN"); //当前套费率电价

            dt.Rows.Add("04090801", 2, 1, "FFFFFFFF", EmSecurityMode.ClearTextRand, 4, "过流事件电流触发下限", "NNN.N"); //当前套费率电价
            dt.Rows.Add("04090802", 1, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 4, "过流事件判定延时时间", "NN"); //当前套费率电价

            dt.Rows.Add("041B0200", 4, 0, "401B0200", EmSecurityMode.ClearTextRand, 0, "第二套阶梯电价", "NNNNNNNN"); //当前套费率电价

            dt.Rows.Add("05000001", 5, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)定时冻结时间", "YYMMDDhhmm");
            dt.Rows.Add("05000901", 32, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)定时冻结正向有功最大需量及发生时间数据", "XX.XXXXYYMMDDhhmm");
            dt.Rows.Add("05020001", 5, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)两套时区表切换时间", "YYMMDDhhmm");
            dt.Rows.Add("05010001", 5, 0, "50000200", EmSecurityMode.ClearTextRand, 0, "(上1次)瞬时冻结", "YYMMDDhhmm");
            dt.Rows.Add("05020101", 20, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)两套时区表切换正向有功电能数据", "XXXXXX.XX");
            dt.Rows.Add("05030001", 5, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)两套日时段表切换时间", "YYMMDDhhmm");
            dt.Rows.Add("05030101", 20, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)两套日时段表切换正向有功电能数据", "YYMMDDhhmm");

            dt.Rows.Add("05060101", 4, 0, "50040200", EmSecurityMode.ClearTextRand, 0, "(上1次)日冻结正向有功电能数据", "XXXXXX.XX");
            dt.Rows.Add("05060102", 4, 0, "50040200", EmSecurityMode.ClearTextRand, 0, "(上2次)日冻结正向有功电能数据", "XXXXXX.XX");
            dt.Rows.Add("05060103", 4, 0, "50040200", EmSecurityMode.ClearTextRand, 0, "(上3次)日冻结正向有功电能数据", "XXXXXX.XX");
            dt.Rows.Add("05060104", 4, 0, "50040200", EmSecurityMode.ClearTextRand, 0, "(上4次)日冻结正向有功电能数据", "XXXXXX.XX");
            dt.Rows.Add("05060105", 4, 0, "50040200", EmSecurityMode.ClearTextRand, 0, "(上5次)日冻结正向有功电能数据", "XXXXXX.XX");
            dt.Rows.Add("05060106", 4, 0, "50040200", EmSecurityMode.ClearTextRand, 0, "(上6次)日冻结正向有功电能数据", "XXXXXX.XX");
            dt.Rows.Add("05060107", 4, 0, "50040200", EmSecurityMode.ClearTextRand, 0, "(上7次)日冻结正向有功电能数据", "XXXXXX.XX");
            dt.Rows.Add("05060108", 4, 0, "50040200", EmSecurityMode.ClearTextRand, 0, "(上8次)日冻结正向有功电能数据", "XXXXXX.XX");
            dt.Rows.Add("05060109", 4, 0, "50040200", EmSecurityMode.ClearTextRand, 0, "(上9次)日冻结正向有功电能数据", "XXXXXX.XX");
            dt.Rows.Add("0506010A", 4, 0, "50040200", EmSecurityMode.ClearTextRand, 0, "(上10次)日冻结正向有功电能数据", "XXXXXX.XX");


            dt.Rows.Add("10000100", 3, 0, "30000A01", EmSecurityMode.ClearTextRand, 0, "失压总次数", "XXXXXX");
            dt.Rows.Add("10010001", 3, 0, "30000A02", EmSecurityMode.ClearTextRand, 0, "A相失压总次数", "XXXXXX"); //未验证
            dt.Rows.Add("10010101", 6, 0, "30000700", EmSecurityMode.ClearTextRand, 0, "(上1次)A相失压发生时刻", "YYMMDDhhmmss"); //未验证
            dt.Rows.Add("10010102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10010103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10010104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10010105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10010106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10010107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10010108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10010109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1001010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相失压发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("10012501", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10012502", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10012503", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10012504", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10012505", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10012506", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10012507", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10012508", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10012509", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1001250A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相失压结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("10020001", 3, 0, "30000A03", EmSecurityMode.ClearTextRand, 0, "B相失压总次数", "XXXXXX");
            dt.Rows.Add("10020101", 6, 0, "30000800", EmSecurityMode.ClearTextRand, 0, "(上1次)B相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10020102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10020103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10020104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10020105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10020106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10020107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10020108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10020109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1002010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相失压发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("10022501", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10022502", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10022503", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10022504", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10022505", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10022506", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10022507", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10022508", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10022509", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1002250A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相失压结束时刻", "YYMMDDhhmmss");


            dt.Rows.Add("10030001", 3, 0, "30000A04", EmSecurityMode.ClearTextRand, 0, "C相失压总次数", "XXXXXX");
            dt.Rows.Add("10030101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10030102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10030103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10030104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10030105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10030106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10030107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10030108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10030109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相失压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1003010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相失压发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("10032501", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10032502", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10032503", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10032504", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10032505", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10032506", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10032507", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10032508", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("10032509", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相失压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1003250A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相失压结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("11010001", 3, 0, "30010A02", EmSecurityMode.ClearTextRand, 0, "A相欠压总次数", "XXXXXX"); //未验证
            dt.Rows.Add("11010101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相欠压发生时刻", "YYMMDDhhmmss"); //未验证
            dt.Rows.Add("11010102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11010103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11010104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11010105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11010106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11010107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11010108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11010109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1101010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相欠压发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("11012501", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11012502", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11012503", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11012504", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11012505", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11012506", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11012507", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11012508", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11012509", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1101250A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相欠压结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("11020001", 3, 0, "30010A03", EmSecurityMode.ClearTextRand, 0, "B相欠压总次数", "XXXXXX"); //未验证
            dt.Rows.Add("11020101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相欠压发生时刻", "YYMMDDhhmmss"); //未验证
            dt.Rows.Add("11020102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11020103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11020104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11020105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11020106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11020107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11020108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11020109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1102010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相欠压发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("11022501", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11022502", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11022503", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11022504", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11022505", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11022506", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11022507", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11022508", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11022509", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1102250A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相欠压结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("11030001", 3, 0, "30010A04", EmSecurityMode.ClearTextRand, 0, "C相欠压总次数", "XXXXXX"); //未验证
            dt.Rows.Add("11030101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相欠压发生时刻", "YYMMDDhhmmss"); //未验证
            dt.Rows.Add("11030102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11030103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11030104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11030105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11030106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11030107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11030108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11030109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相欠压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1103010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相欠压发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("11032501", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11032502", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11032503", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11032504", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11032505", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11032506", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11032507", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11032508", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("11032509", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相欠压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1103250A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相欠压结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("12010001", 3, 0, "30020A02", EmSecurityMode.ClearTextRand, 0, "A相过压总次数", "XXXXXX"); //未验证
            dt.Rows.Add("12010101", 6, 0, "30000700", EmSecurityMode.ClearTextRand, 0, "(上1次)A相过压发生时刻", "YYMMDDhhmmss"); //未验证
            dt.Rows.Add("12010102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12010103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12010104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12010105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12010106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12010107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12010108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12010109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1201010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相过压发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("12012501", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12012502", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12012503", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12012504", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12012505", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12012506", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12012507", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12012508", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12012509", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1201250A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相过压结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("12020001", 3, 0, "30020A03", EmSecurityMode.ClearTextRand, 0, "B相过压总次数", "XXXXXX");
            dt.Rows.Add("12020101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12020102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12020103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12020104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12020105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12020106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12020107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12020108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12020109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1202010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相过压发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("12022501", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12022502", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12022503", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12022504", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12022505", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12022506", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12022507", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12022508", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12022509", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1202250A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相过压结束时刻", "YYMMDDhhmmss");


            dt.Rows.Add("12030001", 3, 0, "30020A04", EmSecurityMode.ClearTextRand, 0, "C相过压总次数", "XXXXXX");
            dt.Rows.Add("12030101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12030102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12030103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12030104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12030105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12030106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12030107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12030108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12030109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相过压发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1203010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相过压发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("12032501", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12032502", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12032503", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12032504", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12032505", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12032506", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12032507", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12032508", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("12032509", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相过压结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1203250A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相过压结束时刻", "YYMMDDhhmmss");

            //------------------------------------------------------------------------------------------------------------------------
            dt.Rows.Add("13010001", 3, 0, "30030A02", EmSecurityMode.ClearTextRand, 0, "A相断相总次数", "XXXXXX");
            dt.Rows.Add("13010101", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13010102", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13010103", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13010104", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13010105", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13010106", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13010107", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13010108", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13010109", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1301010A", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相断相发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("13012501", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13012502", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13012503", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13012504", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13012505", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13012506", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13012507", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13012508", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13012509", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1301250A", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相断相结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("13020001", 3, 0, "30030A03", EmSecurityMode.ClearTextRand, 0, "B相断相总次数", "XXXXXX");
            dt.Rows.Add("13020101", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13020102", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13020103", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13020104", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13020105", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13020106", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13020107", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13020108", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13020109", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1302010A", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相断相发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("13022501", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13022502", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13022503", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13022504", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13022505", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13022506", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13022507", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13022508", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13022509", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1302250A", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相断相结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("13030001", 3, 0, "30030A04", EmSecurityMode.ClearTextRand, 0, "C相断相总次数", "XXXXXX");
            dt.Rows.Add("13030101", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13030102", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13030103", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13030104", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13030105", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13030106", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13030107", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13030108", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13030109", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相断相发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1303010A", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相断相发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("13032501", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13032502", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13032503", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13032504", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13032505", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13032506", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13032507", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13032508", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("13032509", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相断相结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1303250A", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相断相结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("14000001", 3, 0, "300F0400", EmSecurityMode.ClearTextRand, 0, "电压逆相序总次数", "XXXXXX");
            dt.Rows.Add("14000101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)电压逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14000102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)电压逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14000103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)电压逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14000104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)电压逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14000105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)电压逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14000106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)电压逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14000107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)电压逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14000108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)电压逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14000109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)电压逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1400010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)电压逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14001201", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)电压逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14001202", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)电压逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14001203", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)电压逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14001204", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)电压逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14001205", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)电压逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14001206", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)电压逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14001207", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)电压逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14001208", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)电压逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("14001209", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)电压逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1400120A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)电压逆相序结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("15000001", 3, 0, "30100400", EmSecurityMode.ClearTextRand, 0, "电流逆相序总次数", "XXXXXX");
            dt.Rows.Add("15000101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)电流逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15000102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)电流逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15000103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)电流逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15000104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)电流逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15000105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)电流逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15000106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)电流逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15000107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)电流逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15000108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)电流逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15000109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)电流逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1500010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)电流逆相序发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15001201", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)电流逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15001202", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)电流逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15001203", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)电流逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15001204", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)电流逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15001205", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)电流逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15001206", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)电流逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15001207", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)电流逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15001208", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)电流逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("15001209", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)电流逆相序结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1500120A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)电流逆相序结束时刻", "YYMMDDhhmmss");


            dt.Rows.Add("16000001", 3, 0, "301D0400", EmSecurityMode.ClearTextRand, 0, "电压不平衡总次数", "XXXXXX");
            dt.Rows.Add("16000101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)电压不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16000102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)电压不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16000103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)电压不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16000104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)电压不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16000105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)电压不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16000106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)电压不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16000107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)电压不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16000108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)电压不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16000109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)电压不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1600010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)电压不平衡发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("16001301", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)电压不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16001302", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)电压不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16001303", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)电压不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16001304", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)电压不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16001305", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)电压不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16001306", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)电压不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16001307", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)电压不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16001308", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)电压不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("16001309", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)电压不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1600130A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)电压不平衡结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("17000001", 3, 0, "301E0400", EmSecurityMode.ClearTextRand, 0, "电流不平衡总次数", "XXXXXX");
            dt.Rows.Add("17000101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)电流不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17000102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)电流不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17000103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)电流不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17000104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)电流不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17000105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)电流不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17000106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)电流不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17000107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)电流不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17000108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)电流不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17000109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)电流不平衡发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1700010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)电流不平衡发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("17001301", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)电流不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17001302", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)电流不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17001303", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)电流不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17001304", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)电流不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17001305", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)电流不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17001306", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)电流不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17001307", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)电流不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17001308", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)电流不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("17001309", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)电流不平衡结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1700130A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)电流不平衡结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("18010001", 3, 0, "30040A02", EmSecurityMode.ClearTextRand, 0, "A相失流总次数", "XXXXXX");
            dt.Rows.Add("18010101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18010102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18010103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18010104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18010105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18010106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18010107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18010108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18010109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1801010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相失流发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("18012101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18012102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18012103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18012104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18012105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18012106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18012107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18012108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18012109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1801210A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相失流结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("18020001", 3, 0, "30040A03", EmSecurityMode.ClearTextRand, 0, "B相失流总次数", "XXXXXX");
            dt.Rows.Add("18020101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18020102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18020103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18020104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18020105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18020106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18020107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18020108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18020109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1802010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相失流发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("18022101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18022102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18022103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18022104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18022105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18022106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18022107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18022108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18022109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1802210A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相失流结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("18030001", 3, 0, "30040A04", EmSecurityMode.ClearTextRand, 0, "C相失流总次数", "XXXXXX");
            dt.Rows.Add("18030101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18030102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18030103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18030104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18030105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18030106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18030107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18030108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18030109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1803010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相失流发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("18032101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18032102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18032103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18032104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18032105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18032106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18032107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18032108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("18032109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1803210A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相失流结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("19010001", 3, 0, "30050A02", EmSecurityMode.ClearTextRand, 0, "A相过流总次数", "XXXXXX");
            dt.Rows.Add("19010002", 3, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "A相过流总累计时间", "XXXXXX");
            dt.Rows.Add("19010101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19010102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19010103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19010104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19010105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19010106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19010107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19010108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19010109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1901010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相失流发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("19012101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19012102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19012103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19012104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19012105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19012106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19012107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19012108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19012109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1901210A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相失流结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("19020001", 3, 0, "30050A03", EmSecurityMode.ClearTextRand, 0, "B相过流总次数", "XXXXXX");
            dt.Rows.Add("19020101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19020102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19020103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19020104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19020105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19020106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19020107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19020108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19020109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1902010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相失流发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("19022101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19022102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19022103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19022104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19022105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19022106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19022107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19022108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19022109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1902210A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相失流结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("19030001", 3, 0, "30050A04", EmSecurityMode.ClearTextRand, 0, "C相过流总次数", "XXXXXX");
            dt.Rows.Add("19030101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19030102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19030103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19030104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19030105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19030106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19030107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19030108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19030109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相失流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1903010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相失流发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("19032101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19032102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19032103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19032104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19032105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19032106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19032107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19032108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("19032109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相失流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1903210A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相失流结束时刻", "YYMMDDhhmmss");

            //======================================
            dt.Rows.Add("1A010001", 3, 0, "30060A02", EmSecurityMode.ClearTextRand, 0, "A相断流总次数", "XXXXXX");
            dt.Rows.Add("1A010101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A010102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A010103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A010104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A010105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A010106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A010107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A010108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A010109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A01010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相断流发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("1A012101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A012102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A012103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A012104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A012105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A012106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A012107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A012108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A012109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A01210A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相断流结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("1A020001", 3, 0, "30060A03", EmSecurityMode.ClearTextRand, 0, "B相过流总次数", "XXXXXX");
            dt.Rows.Add("1A020101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A020102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A020103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A020104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A020105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A020106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A020107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A020108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A020109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A02010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相断流发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("1A022101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A022102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A022103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A022104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A022105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A022106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A022107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A022108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A022109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A02210A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相断流结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("1A030001", 3, 0, "30060A04", EmSecurityMode.ClearTextRand, 0, "C相过流总次数", "XXXXXX");
            dt.Rows.Add("1A030101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A030102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A030103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A030104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A030105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A030106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A030107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A030108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A030109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相断流发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A03010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相断流发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("1A032101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A032102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A032103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A032104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A032105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A032106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A032107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A032108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A032109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相断流结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1A03210A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相断流结束时刻", "YYMMDDhhmmss");


            dt.Rows.Add("1B010001", 3, 0, "30070A02", EmSecurityMode.ClearTextRand, 0, "A相功率反向总次数", "XXXXXX");
            dt.Rows.Add("1B010101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B010102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B010103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B010104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B010105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B010106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B010107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B010108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B010109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B01010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B011201", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B011202", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B011203", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B011204", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B011205", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B011206", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B011207", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B011208", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B011209", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B01120A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相功率反向结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("1B020001", 3, 0, "30070A03", EmSecurityMode.ClearTextRand, 0, "B相功率反向总次数", "XXXXXX");
            dt.Rows.Add("1B020101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B020102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B020103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B020104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B020105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B020106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B020107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B020108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B020109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B02010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B021201", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B021202", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B021203", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B021204", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B021205", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B021206", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B021207", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B021208", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B021209", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B02120A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相功率反向结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("1B030001", 3, 0, "30070A04", EmSecurityMode.ClearTextRand, 0, "C相功率反向总次数", "XXXXXX");
            dt.Rows.Add("1B030101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B030102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B030103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B030104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B030105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B030106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B030107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B030108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B030109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B03010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相功率反向发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B031201", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B031202", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B031203", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B031204", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B031205", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B031206", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B031207", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B031208", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B031209", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相功率反向结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1B03120A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相功率反向结束时刻", "YYMMDDhhmmss");


            dt.Rows.Add("1C010001", 3, 0, "30080A02", EmSecurityMode.ClearTextRand, 0, "A相过载总次数", "XXXXXX");
            dt.Rows.Add("1C010101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C010102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C010103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C010104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C010105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C010106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C010107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C010108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C010109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C01010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相过载发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("1C011201", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)A相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C011202", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)A相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C011203", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)A相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C011204", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)A相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C011205", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)A相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C011206", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)A相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C011207", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)A相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C011208", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)A相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C011209", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)A相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C01120A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)A相过载结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("1C020001", 3, 0, "30080A03", EmSecurityMode.ClearTextRand, 0, "B相过载总次数", "XXXXXX");
            dt.Rows.Add("1C020101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C020102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C020103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C020104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C020105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C020106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C020107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C020108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C020109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C02010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相过载发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("1C021201", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)B相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C021202", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)B相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C021203", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)B相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C021204", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)B相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C021205", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)B相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C021206", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)B相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C021207", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)B相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C021208", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)B相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C021209", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)B相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C02120A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)B相过载结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("1C030001", 3, 0, "30080A04", EmSecurityMode.ClearTextRand, 0, "C相过载总次数", "XXXXXX");
            dt.Rows.Add("1C030101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C030102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C030103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C030104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C030105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C030106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C030107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C030108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C030109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相过载发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C03010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相过载发生时刻", "YYMMDDhhmmss");

            dt.Rows.Add("1C031201", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)C相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C031202", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)C相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C031203", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)C相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C031204", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)C相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C031205", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)C相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C031206", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)C相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C031207", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)C相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C031208", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)C相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C031209", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)C相过载结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1C03120A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)C相过载结束时刻", "YYMMDDhhmmss");

            dt.Rows.Add("1F000001", 3, 0, "300B0400", EmSecurityMode.ClearTextRand, 0, "总功率因数超下限总次数", "XXXXXX");
            dt.Rows.Add("1F010101", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)总功率因数超下限发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010102", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)总功率因数超下限发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010103", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)总功率因数超下限发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010104", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)总功率因数超下限发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010105", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)总功率因数超下限发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010106", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)总功率因数超下限发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010107", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)总功率因数超下限发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010108", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)总功率因数超下限发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010109", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)总功率因数超下限发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F01010A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)总功率因数超下限发生时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010601", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)总功率因数超下限结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010602", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)总功率因数超下限结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010603", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)总功率因数超下限结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010604", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)总功率因数超下限结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010605", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)总功率因数超下限结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010606", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)总功率因数超下限结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010607", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)总功率因数超下限结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010608", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)总功率因数超下限结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F010609", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)总功率因数超下限结束时刻", "YYMMDDhhmmss");
            dt.Rows.Add("1F01060A", 6, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)总功率因数超下限结束时刻", "YYMMDDhhmmss");


            dt.Rows.Add("21000000", 3, 0, "30070400", EmSecurityMode.ClearTextRand, 0, "潮流反向总次数", "XXXXXX");
            dt.Rows.Add("21000001", 91, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上1次)潮流反向记录", "YYMMDDhhmmss");
            dt.Rows.Add("21000002", 91, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上2次)潮流反向记录", "YYMMDDhhmmss");
            dt.Rows.Add("21000003", 91, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上3次)潮流反向记录", "YYMMDDhhmmss");
            dt.Rows.Add("21000004", 91, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上4次)潮流反向记录", "YYMMDDhhmmss");
            dt.Rows.Add("21000005", 91, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上5次)潮流反向记录", "YYMMDDhhmmss");
            dt.Rows.Add("21000006", 91, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上6次)潮流反向记录", "YYMMDDhhmmss");
            dt.Rows.Add("21000007", 91, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上7次)潮流反向记录", "YYMMDDhhmmss");
            dt.Rows.Add("21000008", 91, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上8次)潮流反向记录", "YYMMDDhhmmss");
            dt.Rows.Add("21000009", 91, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上9次)潮流反向记录", "YYMMDDhhmmss");
            dt.Rows.Add("2100000A", 91, 0, "FFFFFFFF", EmSecurityMode.ClearTextRand, 0, "(上10次)潮流反向记录", "YYMMDDhhmmss");
            dt.Rows.Add("FFFFFFFF", 3, 0, "301F0400", EmSecurityMode.ClearTextRand, 0, "跳闸总次数", "XXXXXX");
            dt.Rows.Add("FFFFFFFF", 3, 0, "30200400", EmSecurityMode.ClearTextRand, 0, "合闸总次数", "XXXXXX");

            //---------------------------------------------------------------------------------------------------------
            //DLT698.45专用协议

            dt.Rows.Add("04000105", 0, 0, "41020200", EmSecurityMode.ClearTextRand, 0, "校表脉冲宽度", "XXXX");
            dt.Rows.Add("04800003", 0, 0, "43000301", EmSecurityMode.ClearTextRand, 0, "厂家编号", "NNNNNNNN");
            dt.Rows.Add("04800001", 0, 0, "43000302", EmSecurityMode.ClearTextRand, 0, "厂家软件版本号", "NNNNNNNN");
            dt.Rows.Add("04800002", 0, 0, "43000304", EmSecurityMode.ClearTextRand, 0, "厂家硬件版本号", "NNNNNNNN");
            dt.Rows.Add("04000801", 0, 0, "40120200", EmSecurityMode.ClearTextRand, 0, "周休日特征字", "NNNNNNNN");
            dt.Rows.Add("04000802", 0, 0, "40130200", EmSecurityMode.ClearTextRand, 0, "周休日采用的日时段表号", "NNNNNNNN");
            //dt.Rows.Add("0400040D", 0, 0, "44000301", EmSecurityMode.ClearTextRand, 0, "协议版本号", "NNNNNNNN");
            dt.Rows.Add("0400040D", 16, 0, "44000301", EmSecurityMode.ClearTextRand, 0, "协议版本号(ASCII码)", "XXXX");

            dt.Rows.Add("-------DLT698.45专用协议--------", 0, 0, "", EmSecurityMode.ClearText, 0, "", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "44010200", EmSecurityMode.ClearTextRand, 0, "明文拉合闸密码", "NNNNNNNN");
            dt.Rows.Add("FFFFFFFF", 0, 0, "F1010200", EmSecurityMode.ClearText, 0, "安全模式参数", "YYYYMMDDHHmmss");

            //dt.Rows.Add("FFFFFFFF", 0, 0, "50000201", EmSecurityMode.ClearTextRand, 0, "(上1次)瞬时冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50000202", EmSecurityMode.ClearTextRand, 0, "(上2次)瞬时冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50000203", EmSecurityMode.ClearTextRand, 0, "(上3次)瞬时冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50000204", EmSecurityMode.ClearTextRand, 0, "(上4次)瞬时冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50000205", EmSecurityMode.ClearTextRand, 0, "(上5次)瞬时冻结记录", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50000300", EmSecurityMode.ClearTextRand, 0, "瞬时冻结对象属性表", "");

            //dt.Rows.Add("FFFFFFFF", 0, 0, "50010201", EmSecurityMode.ClearTextRand, 0, "(上1次)秒冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50010202", EmSecurityMode.ClearTextRand, 0, "(上2次)秒冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50010203", EmSecurityMode.ClearTextRand, 0, "(上3次)秒冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50010204", EmSecurityMode.ClearTextRand, 0, "(上4次)秒冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50010205", EmSecurityMode.ClearTextRand, 0, "(上5次)秒冻结记录", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50010300", EmSecurityMode.ClearTextRand, 0, "秒冻结对象属性表", "");

            //dt.Rows.Add("FFFFFFFF", 0, 0, "50020201", EmSecurityMode.ClearTextRand, 0, "(上1次)分钟冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50020202", EmSecurityMode.ClearTextRand, 0, "(上1次)分钟冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50020203", EmSecurityMode.ClearTextRand, 0, "(上1次)分钟冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50020204", EmSecurityMode.ClearTextRand, 0, "(上1次)分钟冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50020205", EmSecurityMode.ClearTextRand, 0, "(上1次)分钟冻结记录", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50020300", EmSecurityMode.ClearTextRand, 0, "分钟冻结对象属性表", "");

            //dt.Rows.Add("FFFFFFFF", 0, 0, "50030201", EmSecurityMode.ClearTextRand, 0, "(上1次)小时冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50030202", EmSecurityMode.ClearTextRand, 0, "(上2次)小时冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50030203", EmSecurityMode.ClearTextRand, 0, "(上3次)小时冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50030204", EmSecurityMode.ClearTextRand, 0, "(上4次)小时冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50030205", EmSecurityMode.ClearTextRand, 0, "(上5次)小时冻结记录", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50030300", EmSecurityMode.ClearTextRand, 0, "小时冻结对象属性表", "");

            //dt.Rows.Add("FFFFFFFF", 0, 0, "50040201", EmSecurityMode.ClearTextRand, 0, "(上1次)日冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50040202", EmSecurityMode.ClearTextRand, 0, "(上2次)日冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50040203", EmSecurityMode.ClearTextRand, 0, "(上3次)日冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50040204", EmSecurityMode.ClearTextRand, 0, "(上4次)日冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50040205", EmSecurityMode.ClearTextRand, 0, "(上5次)日冻结记录", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50040300", EmSecurityMode.ClearTextRand, 0, "日冻结对象属性表", "");

            //dt.Rows.Add("FFFFFFFF", 0, 0, "50050201", EmSecurityMode.ClearTextRand, 0, "(上1次)结算日冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50050202", EmSecurityMode.ClearTextRand, 0, "(上2次)结算日冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50050203", EmSecurityMode.ClearTextRand, 0, "(上3次)结算日冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50050204", EmSecurityMode.ClearTextRand, 0, "(上4次)结算日冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50050205", EmSecurityMode.ClearTextRand, 0, "(上5次)结算日冻结记录", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50050300", EmSecurityMode.ClearTextRand, 0, "结算日冻结对象属性表", "");

            //月冻结
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50060201", EmSecurityMode.ClearTextRand, 0, "(上1次)月冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50060202", EmSecurityMode.ClearTextRand, 0, "(上2次)月冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50060203", EmSecurityMode.ClearTextRand, 0, "(上3次)月冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50060204", EmSecurityMode.ClearTextRand, 0, "(上4次)月冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50060205", EmSecurityMode.ClearTextRand, 0, "(上5次)月冻结记录", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50060300", EmSecurityMode.ClearTextRand, 0, "月冻结对象属性表", "");

            //年冻结
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50070201", EmSecurityMode.ClearTextRand, 0, "(上1次)年冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50070202", EmSecurityMode.ClearTextRand, 0, "(上2次)年冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50070203", EmSecurityMode.ClearTextRand, 0, "(上3次)年冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50070204", EmSecurityMode.ClearTextRand, 0, "(上4次)年冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50070205", EmSecurityMode.ClearTextRand, 0, "(上5次)年冻结记录", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50070300", EmSecurityMode.ClearTextRand, 0, "年冻结对象属性表", "");

            //时区表切换冻结
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50080201", EmSecurityMode.ClearTextRand, 0, "(上1次)时区表切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50080202", EmSecurityMode.ClearTextRand, 0, "(上2次)时区表切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50080203", EmSecurityMode.ClearTextRand, 0, "(上3次)时区表切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50080204", EmSecurityMode.ClearTextRand, 0, "(上4次)时区表切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50080205", EmSecurityMode.ClearTextRand, 0, "(上5次)时区表切换冻结记录", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50080300", EmSecurityMode.ClearTextRand, 0, "时区表切换冻结对象属性表", "");

            //日时段表切换冻结记录
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50090201", EmSecurityMode.ClearTextRand, 0, "(上1次)日时段表切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50090202", EmSecurityMode.ClearTextRand, 0, "(上2次)日时段表切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50090203", EmSecurityMode.ClearTextRand, 0, "(上3次)日时段表切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50090204", EmSecurityMode.ClearTextRand, 0, "(上4次)日时段表切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "50090205", EmSecurityMode.ClearTextRand, 0, "(上5次)日时段表切换冻结记录", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50090300", EmSecurityMode.ClearTextRand, 0, "日时段表切换冻结对象属性表", "");

            //dt.Rows.Add("FFFFFFFF", 0, 0, "500A0201", EmSecurityMode.ClearTextRand, 0, "(上1次)费率电价切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "500A0202", EmSecurityMode.ClearTextRand, 0, "(上2次)费率电价切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "500A0203", EmSecurityMode.ClearTextRand, 0, "(上3次)费率电价切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "500A0204", EmSecurityMode.ClearTextRand, 0, "(上4次)费率电价切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "500A0205", EmSecurityMode.ClearTextRand, 0, "(上5次)费率电价切换冻结记录", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "500A0300", EmSecurityMode.ClearTextRand, 0, "费率电价切换冻结对象属性表", "");

            //dt.Rows.Add("FFFFFFFF", 0, 0, "500B0201", EmSecurityMode.ClearTextRand, 0, "(上1次)阶梯切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "500B0202", EmSecurityMode.ClearTextRand, 0, "(上2次)阶梯切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "500B0203", EmSecurityMode.ClearTextRand, 0, "(上3次)阶梯切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "500B0204", EmSecurityMode.ClearTextRand, 0, "(上4次)阶梯切换冻结记录", "");
            //dt.Rows.Add("FFFFFFFF", 0, 0, "500B0205", EmSecurityMode.ClearTextRand, 0, "(上5次)阶梯切换冻结记录", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "500B0300", EmSecurityMode.ClearTextRand, 0, "阶梯切换冻结对象属性表", "");

            dt.Rows.Add("FFFFFFFF", 0, 0, "50110201", EmSecurityMode.ClearTextRand, 0, "(上1次)阶梯结算冻结", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50110202", EmSecurityMode.ClearTextRand, 0, "(上2次)阶梯结算冻结", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50110203", EmSecurityMode.ClearTextRand, 0, "(上3次)阶梯结算冻结", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50110204", EmSecurityMode.ClearTextRand, 0, "(上4次)阶梯结算冻结", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50110205", EmSecurityMode.ClearTextRand, 0, "(上5次)阶梯结算冻结", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "50110300", EmSecurityMode.ClearTextRand, 0, "阶梯结算冻结对象属性表", "");
            //---------------------------------------------------------------------------------------------------------
            dt.Rows.Add("FFFFFFFF", 0, 0, "F1000500", EmSecurityMode.ClearTextRand, 0, "会话时效门限", "");
            dt.Rows.Add("02800022", 0, 0, "F1000600", EmSecurityMode.ClearTextRand, 0, "会话时效剩余时间", "");
            dt.Rows.Add("FFFFFFFF", 0, 0, "40070207", EmSecurityMode.ClearTextRand, 0, "液晶12字样意义", "");

            dt.Rows.Add("FFFFFFFF", 3, 0, "302A0400", EmSecurityMode.ClearTextRand, 0, "电能表恒定磁场干扰总次数", "XXXXXX");

            dt.Rows.Add("FFFFFFFF", 0, 0, "43000700", EmSecurityMode.ClearTextRand, 0, "允许跟随上报", "");                //0（禁止跟随上报）
            dt.Rows.Add("FFFFFFFF", 0, 0, "43000800", EmSecurityMode.ClearTextRand, 0, "允许主动上报", "");                //1（允许主动上报）
            dt.Rows.Add("FFFFFFFF", 0, 0, "43000A00", EmSecurityMode.ClearTextRand, 0, "上报通道", "");                    //设置的为支持上报的通道号： F2010201（485通道），F2090201(载波通道)

            dt.Rows.Add("FFFFFFFF", 0, 0, "20150400", EmSecurityMode.ClearTextRand, 0, "上报模式字", "");                  //00000000
            dt.Rows.Add("FFFFFFFF", 0, 0, "30050B00", EmSecurityMode.ClearTextRand, 0, "电能表过流事件", "");              //1:开启主动上报
            dt.Rows.Add("FFFFFFFF", 0, 0, "301B0800", EmSecurityMode.ClearTextRand, 0, "电能表开盖事件", "");              //1:开启主动上报
            dt.Rows.Add("FFFFFFFF", 0, 0, "302A0800", EmSecurityMode.ClearTextRand, 0, "电能表恒定磁场干扰事件", "");      //1:开启主动上报
            dt.Rows.Add("FFFFFFFF", 0, 0, "30130800", EmSecurityMode.ClearTextRand, 0, "电能表清零事件", "");              //1:开启主动上报
            dt.Rows.Add("FFFFFFFF", 0, 0, "302B0800", EmSecurityMode.ClearTextRand, 0, "电能表负荷开关误动作事件", "");    //1:开启主动上报

         

            return dt;
        }

	}

}




