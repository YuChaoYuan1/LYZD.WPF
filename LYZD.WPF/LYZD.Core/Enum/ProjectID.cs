
namespace LYZD.Core.Enum
{
    /// <summary>
    /// 检定项目编码--主要用于数据上传部分
    /// </summary>
    public class ProjectID
    {

        public const string IP地址和端口设置 = "00001";


        public const string 外观 = "00002";
        public const string 通电检测 = "00003";
        public const string 通电检查 = "01001";
        public const string 终端逻辑地址查询 = "01002";
        public const string 终端密钥恢复 = "01003";
        public const string 时钟召测和对时 = "01004";
        public const string 基本参数 = "01005";
        public const string 抄表与费率参数 = "01006";
        public const string 限值与阈值参数 = "01007";
        public const string 控制参数 = "01008";
        public const string 其他参数 = "01009";
        public const string 读取终端信息 = "01010";
        public const string 事件参数 = "01011";
        public const string 单个测量电设置采集档案 = "01012";
        public const string 组合参数读取与参数设置 = "01013";
        public const string 精准校时 = "01014";

        public const string 状态量采集 = "02001";
        public const string 电能表数据采集 = "02002";
        public const string 分脉冲量采集12个 = "02003";
        public const string 分脉冲量采集120个 = "02004";
        public const string 总加组日和月电量召集 = "02005";
        public const string 分时段电能量数据存储 = "02006";
        public const string 电表日历与状态召集 = "02007";
        public const string 电能表实时数据 = "02008";
        public const string 电能表当前数据 = "02009";
        public const string 电能表当前数据2路 = "02010";
        public const string 电能表当前数据错误MAC = "02011";
        public const string 终端采集645表计数据 = "02012";
        public const string HPLC载波通讯 = "02013";
        public const string 总加组电量数据 = "02014";
        public const string 终端主动上报 = "02015";

        public const string 实时和当前数据 = "03001";
        public const string 历史日数据 = "03002";
        public const string 负荷曲线 = "03003";
        public const string 历史月数据 = "03004";
        public const string 电压合格率统计 = "03005";
        public const string 历史日数据补抄 = "03006";
        public const string 负荷曲线补抄 = "03007";
        public const string 随机交采数据 = "03008";
        public const string 电能表事件补抄 = "03009";
        public const string 抄表MAC验证 = "03010";
        public const string 分组抄表 = "03011";
        public const string 结算日冻结 = "03012";
        public const string 透明方案 = "03013";


        public const string 时段功控 = "04001";
        public const string 厂休功控 = "04002";
        public const string 营业报停功控 = "04003";
        public const string 当前功率下浮控 = "04004";
        public const string 月电控 = "04005";
        public const string 购电控 = "04006";
        public const string 催费告警 = "04007";
        public const string 保电功能 = "04008";
        public const string 剔除功能 = "04009";
        public const string 遥控功能 = "04010";
        public const string 时段控与购电控同时投入 = "04011";

        public const string 电能表常数变更事件 = "05001";
        public const string 电能表时段变更事件 = "05002";
        public const string 电能表抄表日变更事件 = "05003";
        public const string 电能表电池欠压事件 = "05004";
        public const string 电能表编程次数变更事件 = "05005";
        public const string 电能表最大需量清零次数变更事件 = "05006";
        public const string 电能表断相次数变更事件 = "05007";
        public const string 电能表示度下降事件 = "05008";
        public const string 电能表超差事件 = "05009";
        public const string 电能表飞走事件 = "05010";
        public const string 电能表停走事件 = "05011";
        public const string 电能表时间超差事件 = "05012";
        public const string 终端参数变更事件 = "05013";
        public const string 电流反向事件 = "05014";
        public const string 电压断相事件 = "05015";
        public const string 电压失压事件 = "05016";
        public const string 终端相序异常事件 = "05017";
        public const string 终端停_上电事件 = "05018";
        public const string 终端停_上电事件_带主动上报 = "05019";
        public const string 电压_电流不平衡度越限事件 = "05020";
        public const string 购电参数设置事件 = "05021";
        public const string 终端485抄表错误 = "05022";
        public const string 电压越限事件 = "05023";
        public const string 电流越限事件 = "05024";
        public const string 视在功率越限事件 = "05025";
        public const string 电能表运行状态字变位事件 = "05026";
        public const string 电能表开表盖事件 = "05027";
        public const string 电能表开端钮盒事件 = "05028";
        public const string 磁场异常事件 = "05029";
        public const string 终端对时事件 = "05030";
        public const string 终端停_上电事件_有效性 = "05031";
        public const string 全事件采集上报 = "05032";
        public const string 电能表对时 = "05033";
        public const string 终端编程事件 = "05034";
        public const string 终端抄表失败 = "05035";
        public const string 电能表数据变更监控记录 = "05036";

        public const string 定时发送1类数据 = "06001";
        public const string 定时发送2类数据 = "06002";

        public const string 基本误差试验 = "12001";
        public const string 起动试验 = "12002";
        public const string 潜动试验 = "12003";
        public const string 走字试验 = "12004";
        public const string 影响量试验 = "12005";
        public const string 日计时误差 = "12006";
        public const string 需量示值误差 = "12012";
        public const string 初始固有误差 = "12013";
        public const string 误差一致性 = "12015";
        public const string 误差变差 = "12016";
        public const string 负载电流升降变差 = "12017";
        public const string 电能示值组合误差 = "12018";

        public const string 电压基本误差 = "13001";
        public const string 电流基本误差 = "13002";
        public const string 有功功率基本误差 = "13003";
        public const string 无功功率基本误差 = "13004";
        public const string 功率因素基本误差Old = "13005";
        public const string 功率因素基本误差 = "13006";
        public const string 常温基本误差 = "13007";
        public const string 谐波影响 = "13008";
        public const string 频率影响 = "13009";
        public const string 电源影响 = "13010";
        public const string 电流不平衡影响 = "13011";
        public const string 功率因数越限统计 = "13012";


        public const string 频率改变 = "12005001";
        public const string 电压改变 = "12005002";
        public const string 方形波波形改变 = "12005003";
        public const string 尖顶波波形改变 = "12005004";
        public const string 间谐波波形改变 = "12005005";
        public const string 奇次谐波波形试验 = "12005006";


        public const string 改485口为抄表口 = "15001";
        public const string 终端维护 = "15002";
        public const string 交采电量清零 = "15003";
        public const string 安全模式 = "15004";

        public const string 身份认证及密钥协商 = "19001";
        public const string 密钥下装 = "19002";

    }
}
