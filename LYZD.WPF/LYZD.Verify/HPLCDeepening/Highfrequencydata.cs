using LYZD.Core.Enum;
using LYZD.Core.Function;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYZD.Verify.HPLCDeepening
{
    /// <summary>
    /// 高频数据采集
    /// </summary>
    public class Highfrequencydata : VerifyBase
    {
        #region 高频数据采集
        //public override void Verify()
        //{
        //    try
        //    {
        //        base.Verify();
        //        //切换规约
        //        //1）切换台体232通讯规约为698.45，波特率为115200;
        //        //2）读取并验证模块ID;
        //        //3）切换台体232通讯规约为376.2，波特率为115200；
        //        OnMeterInfo.MD_Protocol_Type = "698.45";
        //        //读取并验证模块ID
        //        StartVerify698();
        //        OnMeterInfo.MD_Protocol_Type = "376.2";

        //        //4）读取安全模式参数(F1010200)，如果未启用安全模式，则设置为启用；
        //        //5）读取ESAM序列号(F1000200)、对称密钥版本(F1000400)、计数器(F1000700)；
        //        //6）读取主站证书(F1000C00)；
        //        //7）读取终端证书(F1000A00)；
        //        //8）建立应用连接；
        //        ConnectLink(false);

        //        //9）禁止终端主动上报(43000800)；
        //        SetData_698_No("07 01 04 43 00 08 00 03 01 00", "禁用终端主动上报");

        //        //10）清空采集档案配置表（60008600）；
        //        //11）清空采集任务配置表(60128100)；
        //        //12）清空普通采集方案(60148100)；

        //        SetData_698_No("07 01 36 60 00 86 00 00 00", "清空采集档案配置表");
        //        SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
        //        SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

        //        //13）数据区初始化（43000300）；
        //        ResetTerimal_698(2);

        //        //14）控制虚拟CCO断电；
        //        //15）延时等待120秒；
        //        //16）控制虚拟CCO上电，默认其运行状态字的路由完成标志为路由学习完成，工作标志为正在工作，工作开关的工作状态为抄表，当前状态为抄表；
        //        //17）建立应用连接；
        //        WaitTime("延时，", 120);
        //        ConnectLink2(false);

        //        //18）对时至当日12:00:00；
        //       // MessageAdd("终端对时到2015-11-30 23:59:00", EnumLogType.错误信息);
        //        string str = System.DateTime.Now.ToString("yyyy-MM-dd");
        //        string str1= str = str + " 12:00:00";
        //        TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 31, Convert.ToDateTime(str1).ToString(), RecData, MaxWaitSeconds_Write);

        //        //19）配置虚拟表，设置部分电表抄不通；

        //        //20）下发采集档案；+
        //        SetData_698_No("0701396000800001020204120001020A5507050000000000011606160351F2010201090600000000000011041100160312089812000F0204550705000000000000090600000000000012000112000101000204120002020A5507050000000000021606160351F2010201090600000000000011041100160312089812000F02045507050000000000000906000000000000120001120001010000", "下发采集档案");

        //        //21）设置普通采集方案(60147F00)，方案1抄实时值 （00100200/00200200/20000200），采集类型为采集当前数据，存储时标为任务开始时间，MS为全地址；
        //        //方案2日冻结50040200（00100200/00200200/20000200），采集类型为按冻结时标采集，存储时标为相对上日23:59，MS为全地址；
        //        //方案3分钟冻结50020200（00100200/00200200/20000200），采集类型为按时标间隔采集，15min间隔，存储时标为数据冻结时标，MS为全地址；
        //        //22）设置任务配置表(60127F00)，任务1，对应方案1，执行频率30min，延时0min，优先级0，开始时间：当前时间，结束时间：2099.01.01 00:00:00；
        //        //任务2，对应方案2，执行频率1d，延时10min，优先级0，开始时间：当前时间，结束时间：2099.01.01 00:00:00；
        //        //任务3，对应方案3，执行频率1h，延时0min，优先级0，开始时间：当前时间，结束时间：2099.01.01 00:00:00；

        //        //23）延时等待5min；
        //        WaitTime("延时，", 300);
        //        //24）0503召测60120300，召测实时数据，sel7，全地址，00100200/00200200/20000200，除抄不通的表，其他表都抄读成功；

        //        //25）对时过日，当日23:59:50；
        //        str1 = str + "23:59:50";
        //        TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 31, Convert.ToDateTime(str1).ToString(), RecData, MaxWaitSeconds_Write);
        //        //26）延时等待10min；
        //        WaitTime("延时，", 600);

        //        //27）0503召测60120300，召测上一日23:00-当日00:00之间的分钟冻结数据， 全地址，50020200（00100200/00200200/20000200），除抄不通的表，其他表都抄读成功，且曲线为4个点数值， 存储时间分别代表上一日23:00/23:15/23:30/23:45；
        //        //28）等待5min；
        //        WaitTime("延时，", 300);
        //        //29）0503召测60120300，召测日冻结数据，全地址，50040200（00100200/00200200），除抄不通的表，其他表都抄读成功。
        //        //30）控制虚拟CCO断电；


        //        //31）恢复虚拟表；

        //        ControlVirtualMeter("NoResMac,0000000000000000");//恢复虚拟表响应

        //    }
        //    catch (Exception ex)
        //    {

        //        MessageAdd(ex.ToString(), EnumLogType.错误信息);
        //    }
        //}

        //1）切换台体232通讯规约为698.45，波特率为115200;
        //2）读取并验证模块ID;
        //3）切换台体232通讯规约为376.2，波特率为115200；
        //4）读取安全模式参数(F1010200)，如果未启用安全模式，则设置为启用；
        //5）读取ESAM序列号(F1000200)、对称密钥版本(F1000400)、计数器(F1000700)；
        //6）读取主站证书(F1000C00)；
        //7）读取终端证书(F1000A00)；
        //8）建立应用连接；
        //9）禁止终端主动上报(43000800)；
        //10）清空采集档案配置表（60008600）；
        //11）清空采集任务配置表(60128100)；
        //12）清空普通采集方案(60148100)；
        //13）数据区初始化（43000300）；
        //14）控制虚拟CCO断电；
        //15）延时等待120秒；
        //16）控制虚拟CCO上电，默认其运行状态字的路由完成标志为路由学习完成，工作标志为正在工作，工作开关的工作状态为抄表，当前状态为抄表；
        //17）建立应用连接；
        //18）对时至当日12:00:00；
        //19）配置虚拟表，设置部分电表抄不通；
        //20）下发采集档案；
        //21）设置普通采集方案(60147F00)，方案1抄实时值 （00100200/00200200/20000200），采集类型为采集当前数据，存储时标为任务开始时间，MS为全地址；
        //方案2日冻结50040200（00100200/00200200/20000200），采集类型为按冻结时标采集，存储时标为相对上日23:59，MS为全地址；
        //方案3分钟冻结50020200（00100200/00200200/20000200），采集类型为按时标间隔采集，15min间隔，存储时标为数据冻结时标，MS为全地址；
        //22）设置任务配置表(60127F00)，任务1，对应方案1，执行频率30min，延时0min，优先级0，开始时间：当前时间，结束时间：2099.01.01 00:00:00；
        //任务2，对应方案2，执行频率1d，延时10min，优先级0，开始时间：当前时间，结束时间：2099.01.01 00:00:00；
        //任务3，对应方案3，执行频率1h，延时0min，优先级0，开始时间：当前时间，结束时间：2099.01.01 00:00:00；
        //23）延时等待5min；
        //24）0503召测60120300，召测实时数据，sel7，全地址，00100200/00200200/20000200，除抄不通的表，其他表都抄读成功；
        //25）对时过日，当日23:59:50；
        //26）延时等待10min；
        //27）0503召测60120300，召测上一日23:00-当日00:00之间的分钟冻结数据， 全地址，50020200（00100200/00200200/20000200），除抄不通的表，其他表都抄读成功，且曲线为4个点数值， 存储时间分别代表上一日23:00/23:15/23:30/23:45；
        //28）等待5min；
        //29）0503召测60120300，召测日冻结数据，全地址，50040200（00100200/00200200），除抄不通的表，其他表都抄读成功。
        //30）控制虚拟CCO断电；
        //31）恢复虚拟表；
        #endregion


        #region 97 安全拓展  未完成
        /// <summary>
        ///安全拓展
        /// </summary>

        //public override void Verify()
        //{
        //    ConnectLink(false);

        //    #region 读取终端对称密钥版本
        //    MessageAdd("正在读取终端对称密钥版本", EnumLogType.提示与流程信息, true);
        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            setData[i] = Talkers[i].Framer698.ReadData_05("F1000400");
        //        }
        //    }
        //    TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            if (TalkResult[i] == 0)
        //            {
        //                TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
        //            }
        //            else
        //            {
        //                TempData[i].Tips = "无回复";
        //                TempData[i].Resoult = "不合格";
        //            }
        //        }
        //    }
        //    AddItemsResoult("终端对称密钥版本", TempData);
        //    #endregion

        //    #region 读取终端证书版本
        //    MessageAdd("正在读取终端证书版本", EnumLogType.提示与流程信息, true);

        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            setData[i] = Talkers[i].Framer698.ReadData_05("F1000800");
        //        }
        //    }
        //    TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            if (TalkResult[i] == 0)
        //            {
        //                TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
        //            }
        //            else
        //            {
        //                TempData[i].Tips = "无回复";
        //                TempData[i].Resoult = "不合格";
        //            }
        //        }
        //    }
        //    AddItemsResoult("终端证书版本", TempData);
        //    #endregion


        //    MessageAdd("更新错误密钥", EnumLogType.提示与流程信息, true);


        //    MessageAdd("更新错误证书", EnumLogType.提示与流程信息, true);
        //    //8）更新错误密钥（F1000700），终端应返回更新失败；
        //    //9）更新错误证书（F1000800），终端应返回更新失败；

        //    MessageAdd("正在更新后读取终端对称密钥版本", EnumLogType.提示与流程信息, true);
        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            setData[i] = Talkers[i].Framer698.ReadData_05("F1000400");

        //        }
        //    }
        //    TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            if (TalkResult[i] == 0)
        //            {
        //                TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
        //            }
        //            else
        //            {
        //                TempData[i].Tips = "无回复";
        //                TempData[i].Resoult = "不合格";
        //            }
        //        }
        //    }
        //    AddItemsResoult("更新后终端对称密钥版本", TempData);


        //    //判断更新后数据是否一样

        //    MessageAdd("正在读取更新后终端证书版本", EnumLogType.提示与流程信息, true);

        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            setData[i] = Talkers[i].Framer698.ReadData_05("F1000800");
        //        }
        //    }
        //    TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            if (TalkResult[i] == 0)
        //            {
        //                TempData[i].Data = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
        //            }
        //            else
        //            {
        //                TempData[i].Tips = "无回复";
        //                TempData[i].Resoult = "不合格";
        //            }
        //        }
        //    }
        //    AddItemsResoult("更新后终端证书版本", TempData);

        //    //10）再次读取终端对称密钥版本（F1000400），比对密钥版本是否发生变化；
        //    //11）再次读取终端证书版本（F1000800），比对证书版本是否发生变化；
        //    //12） 恢复错误密钥（F1000700），终端应返回恢复失败；
        //    //13） 再次读取终端对称密钥版本（F1000400），比对密钥版本是否发生变化；
        //}


        //protected override bool CheckPara()
        //{
        //    //1）读取安全模式参数(F1010200)，如果未启用安全模式，则设置为启用；
        //    //2）读取ESAM序列号(F1000200)、对称密钥版本(F1000400)、计数器(F1000700)；
        //    //3）读取主站证书(F1000C00)；
        //    //4）读取终端证书(F1000A00)；
        //    //5）建立应用连接；
        //    //6）读取终端对称密钥版本（F1000400）；
        //    //7）读取终端证书版本（F1000800）；
        //    //8）更新错误密钥（F1000700），终端应返回更新失败；
        //    //9）更新错误证书（F1000800），终端应返回更新失败；
        //    //10） 再次读取终端对称密钥版本（F1000400），比对密钥版本是否发生变化；
        //    //11） 再次读取终端证书版本（F1000800），比对证书版本是否发生变化；
        //    //12） 恢复错误密钥（F1000700），终端应返回恢复失败；
        //    //13） 再次读取终端对称密钥版本（F1000400），比对密钥版本是否发生变化；


        //    //ResultNames = new string[] { "上1次终端初始化事件", "终端初始化事件当前记录数", "读取终端时钟", "恢复出厂默认参数", "终端对时", "结论" };
        //    return true;
        //}

        #endregion


        #region  177 容量监测(分支过载事件)  未完成
        /// <summary>
        ///安全拓展
        /// </summary>

        //public override void Verify()
        //{
        //    ConnectLink(false);
        //    SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");
        //    SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");
        //    SetData_698("06011831A10900030100", "设置事件有效标志");
        //    ResetTerimal_698(2); WaitTime("延时，", 120);
        //    SetData_698_No("07 01 04 43 00 08 00 03 01 00", "禁用终端主动上报");

        //    //11） 下装表档案(60000200),依次下发3块电表，配置序号为[101，102，103]，电表地址为[220000000101，220000000102，220000000103]，通信规约为DL / T698.45、波特率为115200，端口为F2010201, 电压互感器变比1，电流互感器变比1，用户类型为1，附属信息设置40360200数值为0x0200;
        //    //12） 下装普通采集方案(60147F00)，序号1，记录列为电压(20000200)、电流（20010200）、视在功率（20060200），电表集合为一组用户类型，用户类型为1，存储时标选择存储方式1（任务开始时间），采集方式为采集当前数据；
        //    //13） 下装采集任务(60127F00)，序号1，执行频率为1分钟，任务开始时间为次日00: 00:00，结束时间为2099 - 09 - 09 09:09:09，延时为0分钟，任务优先级127，时段为00: 00 - 23:59前闭后闭；
        //    //14） 下发过载容量配置47110200，分支1：电表地址为[220000000101]，最大负荷3KVA, 最大持续时间为180秒；分支2：电表地址为[220000000102]，最大负荷3KVA, 最大持续时间为180秒；分支3：电表地址为[220000000103]，最大负荷3KVA, 最大持续时间为180秒；
        //    //15） 配置模拟表，视在功率为2.1 KVA。(总视在功率还是分项视在功率)
        //07 01 01 60 12 7f 00 01 01 02 0c 11 01 54 01 00 01 16 01 11 01 1c 07 e1 08 09 00 02 00 1c 08 33 09 09 09 09 09 54 00 00 00 11 7f 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3b 00

        //    SetData_698_No("01 01 02 06 11 01 12 01 00 02 02 11 00 00 01 03 5b 00 20 00 02 00 5b 00 20 01 02 00 5b 00 20 06 02 00 5c 01 16 01 00", "下装普通采集方案");
        //    SetData_698_No("01 01 02 06 11 01 12 01 00 02 02 11 00 00 01 03 5b 00 20 00 02 00 5b 00 20 01 02 00 5b 00 20 06 02 00 5c 01 16 01 00", "下装普通采集方案");
        //    string str = System.DateTime.Now.ToString("yyyy-MM-dd");
        //    string str1 = str = str + " 23:59:50";
        //    TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 31, Convert.ToDateTime(str1).ToString(), RecData, MaxWaitSeconds_Write);

        //    //读取采集任务数据记录表(60120300)，电表集合为一组配置序号区间，配置序号[101，102，103]，记录列为电压(20000200)、电流（20010200）、视在功率（20060200），判断数据是否正确；
        //    //19） 延时等待3min，召测分支过载事件（31A10200）, 应未产生分支过载事件；
        //    //20） 配置模拟表，视在功率为3.8 KVA。
        //    //21） 延时等待1min，等待终端抄表；
        //    //22） 读取采集任务数据记录表(60120300)，电表集合为一组配置序号区间，配置序号[101，102，103]，记录列为电压(20000200)、电流（20010200）、视在功率（20060200），判断数据是否正确；
        //    //23） 延时等待1分钟，召测分支过载事件（31A10200）,应未产生分支过载事件；
        //    //24） 延时等待3分钟，召测分支过载事件（31A10200）,应正确产生分支过载事件；
        //    //25） 查询显示屏幕，能够查询到分支过载事件正确显示。
        //}


        //protected override bool CheckPara()
        //{
        //    //1） 读取安全模式参数(F1010200)，如果未启用安全模式，则设置为启用；
        //    //2） 读取ESAM序列号(F1000200)、对称密钥版本(F1000400)、计数器(F1000700)；
        //    //3） 读取主站证书(F1000C00)；
        //    //4） 读取终端证书(F1000A00)；
        //    //5） 建立应用连接；
        //    //6） 清空采集任务配置表(60128100)；
        //    //7） 清空普通采集方案(60148100)；
        //    //8） 设置事件有效标志(31A10900)；
        //    //9） 数据区初始化(43000300)，延时等待120秒；
        //    //10） 禁止终端主动上报（43000800）；
        //    //11） 下装表档案(60000200),依次下发3块电表，配置序号为[101，102，103]，电表地址为[220000000101，220000000102，220000000103]，通信规约为DL / T698.45、波特率为115200，端口为F2010201, 电压互感器变比1，电流互感器变比1，用户类型为1，附属信息设置40360200数值为0x0200;
        //    //12） 下装普通采集方案(60147F00)，序号1，记录列为电压(20000200)、电流（20010200）、视在功率（20060200），电表集合为一组用户类型，用户类型为1，存储时标选择存储方式1（任务开始时间），采集方式为采集当前数据；
        //    //13） 下装采集任务(60127F00)，序号1，执行频率为1分钟，任务开始时间为次日00: 00:00，结束时间为2099 - 09 - 09 09:09:09，延时为0分钟，任务优先级127，时段为00: 00 - 23:59前闭后闭；
        //    //14） 下发过载容量配置47110200，分支1：电表地址为[220000000101]，最大负荷3KVA, 最大持续时间为180秒；分支2：电表地址为[220000000102]，最大负荷3KVA, 最大持续时间为180秒；分支3：电表地址为[220000000103]，最大负荷3KVA, 最大持续时间为180秒；
        //    //15） 配置模拟表，视在功率为2.1 KVA。(总视在功率还是分项视在功率)
        //    //16） 终端校时到当日的23: 59:50(40000200)；
        //    //17） 延时等待1min，等待终端抄表；
        //    //18） 读取采集任务数据记录表(60120300)，电表集合为一组配置序号区间，配置序号[101，102，103]，记录列为电压(20000200)、电流（20010200）、视在功率（20060200），判断数据是否正确；
        //    //19） 延时等待3min，召测分支过载事件（31A10200）, 应未产生分支过载事件；
        //    //20） 配置模拟表，视在功率为3.8 KVA。
        //    //21） 延时等待1min，等待终端抄表；
        //    //22） 读取采集任务数据记录表(60120300)，电表集合为一组配置序号区间，配置序号[101，102，103]，记录列为电压(20000200)、电流（20010200）、视在功率（20060200），判断数据是否正确；
        //    //23） 延时等待1分钟，召测分支过载事件（31A10200）,应未产生分支过载事件；
        //    //24） 延时等待3分钟，召测分支过载事件（31A10200）,应正确产生分支过载事件；
        //    //25） 查询显示屏幕，能够查询到分支过载事件正确显示。

        //    //ResultNames = new string[] { "上1次终端初始化事件", "终端初始化事件当前记录数", "读取终端时钟", "恢复出厂默认参数", "终端对时", "结论" };
        //    return true;
        //}

        #endregion


        #region  138 交流模拟量   未完成

        //public override void Verify()
        //{
        //    ConnectLink(true);

        //    #region 清空 ----校时 ---初始化---再次联机
        //    MessageAdd("清空采集任务配置表", EnumLogType.提示与流程信息);
        //    SetData_698_No("07 01 04 60 12 81 00 00 00", "清空采集任务配置表");

        //    MessageAdd("清空普通采集方案", EnumLogType.提示与流程信息);
        //    SetData_698_No("07 01 05 60 14 81 00 00 00", "清空普通采集方案");

        //    MessageAdd("清空采集档案配置表", EnumLogType.提示与流程信息);
        //    SetData_698_No("07 01 05 60 00 86 00 00 00", "清空采集档案配置表");


        //    DateTime dttmp = DateTime.Now;
        //    MessageAdd("终端校时到当前事件", EnumLogType.提示与流程信息, true);
        //    TalkResult = TerminalProtocalAdapter.Instance.WriteData_Afn04(5, 0, 31, Convert.ToDateTime(dttmp).ToString(), RecData, MaxWaitSeconds_Write);

        //    MessageAdd("数据区初始化", EnumLogType.提示与流程信息, true);

        //    ResetTerimal_698(2);

        //    WaitTime("延时等待，", 120);

        //    ConnectLink2(true);
        //    #endregion

        //    # region 下装交流采样档案
        //    MessageAdd("下装交流采样档案", EnumLogType.提示与流程信息, true);

        //    #endregion

        //    #region 下装普通采集方案
        //    MessageAdd("下装普通采集方案", EnumLogType.提示与流程信息, true);
        //    string str = "5b00001002005b00002002005b00003002005b00004002005b00200002005b00200102005b00200402005b00200502005b00200A02005b0010100200";
        //    string str1 = "07011460147F000101020611011201000202110000010a" + str + "5C03010705900000000001160100";
        //    SetData_698_No(str1, "下装普通采集方案");
        //    #endregion

        //    #region 下装采集任务
        //    MessageAdd("下装采集任务", EnumLogType.提示与流程信息, true);
        //    string str采集任务 = "07 01 10 60 12 7f 00 01 01 02 0c 11 01 54 03 00 01 16 01 11 01 1c 07 e0 01 01 00 00 00 1c 08 33 09 09 09 09 09 54 01 00 00 11 02 16 01 12 00 00 12 00 00 02 02 16 00 01 01 02 04 11 00 11 00 11 17 11 3b 00";
        //    SetData_698_No(str采集任务, "下装采集任务");
        //    #endregion

        //    WaitTime("等待终端抄表，", 240);

        //    #region  读取标准表数据
        //    MessageAdd("读取标准表数据", EnumLogType.提示与流程信息);
        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            Talkers[i].Framer698.sAPDU = "05 03 12 60 12 03 00 05 " + Talkers[i].Framer698.SetDateTimeBCD(dttmp.AddSeconds(10), false) + " 03 02 07 05 00 00 00 00 00 03 07 05 00 00 00 00 00 04 05 00 20 2A 02 00 00 60 40 02 00 00 60 41 02 00 00 60 42 02 00 01 50 04 02 00 03 20 21 02 00 00 10 02 00 00 20 02 00 00 ";
        //            setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
        //        }
        //    }
        //    TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);

        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            TempData[i].StdData = "500000,125000,125000,125000,125000";
        //            if (TalkResult[i] == 0)
        //            {
        //                string sTmp = GetData(RecData, i, 20, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 21, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 22, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 23, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 24, EnumTerimalDataType.e_string);
        //                if (sTmp != "500000,125000,125000,125000,125000")
        //                    TempData[i].Resoult = "不合格";
        //                TempData[i].Data = sTmp;
        //            }
        //            else
        //            {
        //                TempData[i].Resoult = "不合格";
        //                TempData[i].Tips = "无回复";
        //            }
        //        }
        //    }
        //    AddItemsResoult("电表正向有功", TempData);

        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            TempData[i].StdData = "400000,100000,100000,100000,100000";
        //            if (TalkResult[i] == 0)
        //            {
        //                string sTmp = GetData(RecData, i, 25, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 26, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 27, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 28, EnumTerimalDataType.e_string) + "," + GetData(RecData, i, 29, EnumTerimalDataType.e_string);
        //                if (sTmp != "400000,100000,100000,100000,100000")
        //                    TempData[i].Resoult = "不合格";
        //                TempData[i].Data = sTmp;
        //            }
        //            else
        //            {
        //                TempData[i].Resoult = "不合格";
        //                TempData[i].Tips = "无回复";
        //            }
        //        }
        //    }
        //    AddItemsResoult("电表反向有功", TempData);
        //    #endregion

        //    #region 读取采集任务监控数据
        //    MessageAdd("读取采集任务监控数据", EnumLogType.提示与流程信息);
        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            Talkers[i].Framer698.sAPDU = " 05 03 35 60 34 02 00 01 60 35 02 01 11 01 00 00 ".Replace(" ", "");
        //            setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);
        //        }
        //    }
        //    TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            if (TalkResult[i] == 0)
        //            {
        //                TempData[i].Data = GetData(RecData, i, 10, EnumTerimalDataType.e_string);
        //            }
        //            else
        //            {
        //                TempData[i].Tips = "无回复";
        //                TempData[i].Resoult = "不合格";
        //            }
        //        }
        //    }
        //    AddItemsResoult("读取采集任务监控数据", TempData);
        //    #endregion


        //    MessageAdd("读取采集任务数据记录表", EnumLogType.提示与流程信息);

        //    #region 删除交流采样表计档案
        //    MessageAdd("删除交流采样表计档案", EnumLogType.提示与流程信息);
        //    SetData_698_No("07 01 13 60 00 83 00 12 00 0A 00  ", "删除采集档案");
        //    #endregion 
        //}


        //protected override bool CheckPara()
        //{
        //    //1）读取安全模式参数(F1010200)，如果未启用安全模式，则设置为启用；
        //    //2）读取ESAM序列号(F1000200)、对称密钥版本(F1000400)、计数器(F1000700)；
        //    //3）读取主站证书(F1000C00)；
        //    //4）读取终端证书(F1000A00)；
        //    //5）建立应用连接，采用数字签名的认证机制；
        //    //6）清空采集任务配置表(60128100)；
        //    //7）清空普通采集方案(60148100)；
        //    //8）清空采集档案配置表(60008600)；
        //    //9）终端校时到当前时间(40000200)；
        //    //10）数据区初始化(43000300)，延时等待120秒；
        //    //11）建立应用连接，采用数字签名的认证机制；
        //    //18）下装交流采样档案(60000200),下发1块电表，序号为5，电表地址为900000000001，通信规约DL / T698.45，端口为交采端口；
        //    //12）下装普通采集方案(60147F00)，记录列为正向有功电能(00100200)、反向有功电能(00200200)、正向无功电能(00300200)、反向无功电能(00400200)、电压(20000200)、电流(20010200)、有功功率(20040200)、无功功率(20050200)、功率因数(200A0200)、当月最大有功需量及发生时间(10100200)，电表集合为一组用户地址为900000000001，存储时标选择1（任务开始时间），采集方式为采集当前数据；
        //    //13）下装采集任务(60127F00)，执行频率为1分钟1次，任务开始时间为2016 - 01 - 01 00:00:00，结束时间为2099 - 09 - 09 09:09:09，延时为0秒，时段为00: 00 - 23:59；
        //    //14）延时等待4min，等待终端抄表；
        //    //15）读取标准表数据；
        //    //16）读取采集任务监控数据(60340200)，判断任务执行结束时间及采集成功数量；
        //    //17）读取采集任务数据记录表(60120300)，记录列为正向有功电能(00100200)、反向有功电能(00200200)、正向无功电能(00300200)、反向无功电能(00400200)、电压(20000200)、电流(20010200)、有功功率(20040200)、无功功率(20050200)、功率因数(200A0200)、当月最大有功需量及发生时间(10100200)，判断数据是否正确；
        //    //删除交流采样表计档案(60008300)。

        //    //   13
        //    //07 01 0c 60 14 7f 00 01 01 02 06 11 01 12 01 00 02 02 11 00 00 01 0a 5b 00 00 10 02 00 5b 00 00 20 02 00 5b 00 00 30 02 00 5b 00 00 40 02 00 5b 00 20 00 02 00 5b 00 20 01 02 00 5b 00 20 04 02 00 5b 00 20 05 02 00 5b 00 20 0a 02 00 5b 00 10 10 02 00 5c 03 01 07 05 09 00 00 00 00 01 16 01 00
        //    //ResultNames = new string[] { "标准表数据", "采集任务监控数据", "采集任务数据记录表", "终端草表", "终端对时", "结论" };
        //    return true;
        //}
        #endregion


        #region 111	设备管理


        //public override void Verify()
        //{
        //    ConnectLink(true);

        //    #region 读取设备运行信息
        //    //05 01 08 40 00 0c 00 00   
        //    MessageAdd("正在读取ESAM信息", EnumLogType.提示与流程信息, true);
        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            Talkers[i].Framer698.sAPDU = "05 01 08 40 00 0c 00 00  ";
        //            setData[i] = Talkers[i].Framer698.ReadData(Talkers[i].Framer698.sAPDU);//设备运行信息
        //        }
        //    }
        //    TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, MaxWaitSeconds_Write);
        //    for (int i = 0; i < MeterNumber; i++)
        //    {
        //        if (meterInfo[i].YaoJianYn)
        //        {
        //            if (TalkResult[i] == 0)
        //            {
        //                Talkers[i].Framer698.cTESAMNO = GetData(RecData, i, 6, EnumTerimalDataType.e_string);
        //                TempData[i].Data = "设备运行信息:" + Talkers[i].Framer698.cTESAMNO;
        //                TempData[i].Resoult = "合格";
        //            }
        //            else
        //            {
        //                MessageAdd("终端" + (i + 1) + "无回复消息！", EnumLogType.错误信息);
        //                TempData[i].Resoult = "不合格";
        //            }

        //        }
        //    }
        //    testTempData.Add(TempData);


        //    #endregion
        //}


        //protected override bool CheckPara()
        //{
        //    //1）检测软件开启独立检测信道，等待设备上线；
        //    //2）读取安全模式参数(F1010200)，如果未启用安全模式，则设置为启用；
        //    //3）读取ESAM序列号(F1000200)、对称密钥版本(F1000400)、计数器(F1000700)；
        //    //4）读取主站证书(F1000C00)；
        //    //5）读取终端证书(F1000A00)；
        //    //6）建立应用连接，采用数字签名的认证机制；
        //    //7）读取设备运行信息（43000C00）,检查设备参数与检测软件配置是否正确。
        //    //8）配置相应信道心跳周期为60秒；
        //    //9）延时130秒，检测心跳是否正常，是否收到2个心跳；
        //    //10）读取终端时钟(40000200)，检查是否正常响应；
        //    //11）读取时钟源参数(40060200)，检查是否正常响应；
        //    //12）读取设备运行信息（43000C00）,检查设备参数与检测软件配置是否正确；
        //    //13）清空采集任务配置表(60128100)；
        //    //14）清空上报方案(601C8100)；
        //    //15）禁止终端主动上报(43000800)；
        //    //16）设置事件有效标志为有效(36000900)；
        //    //17）设置事件上报标志(36000800)，设置为事件发生恢复均上报；
        //    //18）配置终端上报通道（43000A00）为相应通道；
        //    //19）配置系统监测事件参数（36000600），设置内存、存储、CPU上限为1 %，温度上限为25℃，监测周期为1分钟；
        //    //20）下装上报方案(601C7F00)，最大上报次数为1次，上报类型为对象属性数据，上报内容为设备运行信息(43000C00)；
        //    //21）下装上报任务(60127F00)执行方案1，执行频率为1分钟一次，任务开始时间为当日00: 00:00，结束时间为2099 - 09 - 09 09:09:09，延时为0分钟，时段为00: 00 - 23:59；
        //    //22）允许终端主动上报(43000800)；
        //    //23）事件初始化(43000500)，延时等待30秒； 
        //    //24）延时等待120秒；
        //    //25）检查是否收到系统检测事件（3600），CPU、内存、存储、温度越限发生事件各1条。
        //    //26）配置系统监测事件参数（36000600），设置内存、存储、CPU上限为100 %，温度上限为125℃，监测周期为1分钟；
        //    //27）延时等待120秒；
        //    //28）检查是否收到系统检测事件（3600），CPU、内存、存储、温度越限结束事件各1条。
        //    //29）检查上报数据，应符合上报任务配置，上报条数不少于4条；
        //    //30）设置事件有效标志为无效(36000900)
        //    //31)禁止终端主动上报(43000800)；

        //    //ResultNames = new string[] { "上1次终端初始化事件", "终端初始化事件当前记录数", "读取终端时钟", "恢复出厂默认参数", "终端对时", "结论" };
        //    return true;
        //}
        #endregion


        #region 90 文件传输

        //public override void Verify()
        //{
        //    ConnectLink(true);

        //}


        //protected override bool CheckPara()
        //{

        //    //1)读取安全模式参数(F1010200)，如果未启用安全模式，则设置为启用；
        //    //2)读取ESAM序列号(F1000200)、对称密钥版本(F1000400)、计数器(F1000700)；
        //    //3)读取主站证书(F1000C00)；
        //    //4)读取终端证书(F1000A00)；
        //    //5)建立应用连接,客户机发送帧最大尺寸256K；
        //    //【生成随机文件，大小为300K】
        //    //0a 0a  2e  2f 66 69 6c 65 2e 64 61 74   目标文件的到吗
        //    //68 53 00 43 05 01 00 00 00 00 00 10 76 65 07 01 18 F0 01 07 00 02 03 02 06 0A 00 0A 19 2F 75 70 64 61 74 65 2F 64 6F 77 6E 6C 6F 61 64 2F 66 69 6C 65 2E 64 61 74 06 00 08 00 00 04 03 C0 0A 04 56 31 2E 31 16 00 06 00 03 FF DF 02 02 16 00 09 00 00 E1 74 16


        //    //6)启动文件传输（F0010700），目标文件为“./ file.dat”；   07 01 18 f0 01 07 00 02 01 0a 0a 2e 2f 66 69 6c 65 2e 64 61 74 00
        //    //7)写文件（F0010800）第一帧，帧长度L为（256K）;
        //    //    8)写文件（F0010800）第二帧，帧长度L为（44K）；
        //    //9)写文件（F0010800）第三帧，帧长度L为文件剩余字节 + 报文其它占用字节；
        //    //10)终端复位；
        //    //11)延时120秒；
        //    //12)建立应用连接,客户机接收帧最大尺256K；
        //    //13)读取文件(F0007F00)第一帧，目标文件路径为“./ file.dat”，帧长度L为（256K）；
        //    //14)读取文件(F0007F00)第二帧，帧长度L为（44K）；
        //    //15)读取文件(F0007F00)第三帧，帧长度L为文件剩余大小 + 报文其它占用字节；
        //    //验证原始文件与读取到的文件是否一致，一致则测试合格。


        //    //ResultNames = new string[] { "上1次终端初始化事件", "终端初始化事件当前记录数", "读取终端时钟", "恢复出厂默认参数", "终端对时", "结论" };
        //    return true;

        //}


        #endregion


        #region    PowerConsume 功耗试验

        private bool[] BuYaoJianBiaoWei = null;   //不要检定的表位
        public override void Verify()
        {
            MessageAdd("功耗验检定开始...", EnumLogType.提示与流程信息);

            base.Verify();

            bool[] YJMeter = new bool[MeterNumber];

            BuYaoJianBiaoWei = new bool[MeterNumber];

            BuYaoJianBiaoWei.Fill(false);
          
            //关闭其余继电器，打开要测的继电器
            for (int i = 0; i < MeterNumber; i++)
            {
                if (!meterInfo[i].YaoJianYn)
                {
                    EquipmentData.DeviceManager.ControlMeterRelay(2, (byte)i);  //关闭表位继电器
                }
                else
                {
                    EquipmentData.DeviceManager.ControlMeterRelay(1, (byte)i);  //关闭表位继电器
                }
            }
           
            //下发参数

            //获取要做功耗的表位
            for (int i = 0; i < MeterNumber; i++)
            {
                if (meterInfo[i].YaoJianYn)
                {

                    //68H + 13H + FEH + LEN + 1AH + NUM + DATA + CS  读取误差版上的功耗

                   // 68 13 FE 08 1A 01 00 FE
                }
            }
        }
        #endregion
    }
}
