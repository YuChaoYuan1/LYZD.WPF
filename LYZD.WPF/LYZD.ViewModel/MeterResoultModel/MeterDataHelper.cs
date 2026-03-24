using LYZD.Core.Enum;
using LYZD.Core.Model.DnbModel.DnbInfo;
using LYZD.Core.Model.Meter;
using LYZD.DAL;
using LYZD.ViewModel.CheckInfo;
using LYZD.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace LYZD.ViewModel.MeterResoultModel
{
    /// <summary>
    /// 检定结论获取
    /// </summary>
    public class MeterDataHelper
    {
        /// <summary>
        /// 获取所有的结论--这个是临时数据库获取--一般只用于自动化线
        /// </summary>
        /// <returns></returns>
        public static TestMeterInfo[] GetDnbInfoNew()
        {

            TestMeterInfo[] meterTemp = GetMeterInfo();
            for (int i = 0; i < EquipmentData.CheckResults.ResultCollection.Count; i++)
            {
                CheckNodeViewModel result = EquipmentData.CheckResults.ResultCollection[i];   //结论
                for (int j = 0; j < result.CheckResults.Count; j++)
                {
                    if (!meterTemp[j].YaoJianYn) continue;
                    if (!meterTemp[j].MeterResoultData.ContainsKey(EquipmentData.CheckResults.ResultCollection[i].ParaNo))
                    {
                        meterTemp[j].MeterResoultData.Add(EquipmentData.CheckResults.ResultCollection[i].ParaNo, new MeterResoultData());
                    }
                    MeterItemResoultData resultData = new MeterItemResoultData();
                    resultData.ID = result.ItemKey;
                    List<string> names = result.CheckResults[j].GetAllProperyName();
                    DynamicModel resoultData = result.CheckResults[j].GetDataSource();

                    //这里先排除不是对应协议的部分
                    string RemoveProtocol = "376";  //需要删除的协议
                    if (meterTemp[j].MD_Protocol_Type == "376.1")
                    {
                        RemoveProtocol = "698";
                    }
                    if (names.Contains("分项结论" + RemoveProtocol))
                        names.Remove("分项结论" + RemoveProtocol);
                    if (names.Contains("要检"))
                        names.Remove("要检");
                    if (names.Contains("项目号"))
                        names.Remove("项目号");
                    if (names.Contains("<" + RemoveProtocol) && names.Contains(RemoveProtocol + ">")) //包括含开头
                    {
                        names.RemoveRange(names.IndexOf("<" + RemoveProtocol), names.LastIndexOf(RemoveProtocol + ">") - names.IndexOf("<" + RemoveProtocol) + 1);
                    }
                    names.RemoveAll(item => ItemResoultkeyword.GetSpliteList().Contains(item));
                    for (int index = 0; index < names.Count; index++)
                    {
                        string value = resoultData.GetProperty(names[index]) as string;
                        if (names[index].IndexOf("分项结论") != -1)
                        {
                            if (value == "") continue;
                            string[] tmp = value.ToString().Split('#');
                            for (int s = 0; s < tmp.Length; s++)
                            {
                                resultData.ItemDatas.Add(new ItemData(tmp[s]));
                            }
                        }
                        else if (names[index] == "结论")
                        {
                            if (value == "合格")
                                resultData.Result = value;
                            else
                            {
                                resultData.Result = "不合格";
                                meterTemp[j].MeterResoultData[EquipmentData.CheckResults.ResultCollection[i].ParaNo].Result = "不合格";
                            }
                        }
                        else
                        {

                            resultData.Datas.Add(names[index], value);
                        }
                    }
                    meterTemp[j].MeterResoultData[EquipmentData.CheckResults.ResultCollection[i].ParaNo].meterResoults.Add(resultData);
                }
            }
            return meterTemp;
        }

        //应该支持俩种获取
        //逐个获取和全部获取
        //这里应该支持所有的情况--以后有问题只需要更改mis就行--这里永远不更改

        /// <summary>
        /// 获取表的基本数据(参数录入的数据)
        /// </summary>
        /// <returns></returns>
        private static TestMeterInfo[] GetMeterInfo()
        {
            TestMeterInfo[] testMeters = new TestMeterInfo[EquipmentData.MeterGroupInfo.Meters.Count];
            for (int i = 0; i < testMeters.Length; i++)
            {
                DynamicViewModel model = EquipmentData.MeterGroupInfo.Meters[i];
                TestMeterInfo meterTemp = new TestMeterInfo();
                #region 基本信息
                meterTemp.Meter_ID = model.GetProperty("METER_ID").ToString();
                meterTemp.MD_DeviceID = model.GetProperty("MD_DEVICE_ID").ToString();  //台体编号     MD_DEVICE_ID
                meterTemp.MD_Epitope = int.Parse(model.GetProperty("MD_EPITOPE").ToString());//表位号
                meterTemp.Result = model.GetProperty("MD_RESULT").ToString();  //表结论
                meterTemp.YaoJianYn = model.GetProperty("MD_CHECKED").ToString() == "1" ? true : false;
                meterTemp.MD_UB = float.Parse(model.GetProperty("MD_UB").ToString());
                meterTemp.MD_UA = model.GetProperty("MD_UA").ToString();
                meterTemp.MD_Frequency = int.Parse(model.GetProperty("MD_FREQUENCY").ToString());//频率
                meterTemp.MD_TestType = model.GetProperty("MD_TEST_TYPE").ToString();//检定类型---全性能
                meterTemp.MD_WiringMode = model.GetProperty("MD_WIRING_MODE").ToString();//测量方式-三相三。三相四
                meterTemp.MD_ConnectionFlag = model.GetProperty("MD_CONNECTION_FLAG").ToString();//直接还是互感
                meterTemp.MD_BarCode = model.GetProperty("MD_BAR_CODE").ToString(); //条形码
                meterTemp.MD_Constant = model.GetProperty("MD_CONSTANT").ToString();//常数
                meterTemp.MD_Grane = model.GetProperty("MD_GRADE").ToString();//等级
                meterTemp.MD_Factory = model.GetProperty("MD_FACTORY").ToString();//制作厂家
                meterTemp.MD_Customer = model.GetProperty("MD_CUSTOMER").ToString();//送检单位
                meterTemp.MD_TaskNo = model.GetProperty("MD_TASK_NO").ToString();//任务编号
                meterTemp.MD_MadeNo = model.GetProperty("MD_MADE_NO").ToString();//出厂编号
                meterTemp.MD_CertificateNo = model.GetProperty("MD_CERTIFICATE_NO").ToString();//证书编号
                meterTemp.MD_SchemeID = model.GetProperty("MD_SCHEME_ID").ToString();//方案编号
                meterTemp.MD_TerminalType = model.GetProperty("MD_TERMINAL_TYPE").ToString();//终端类--集中器-专变
                meterTemp.MD_BatchNo = model.GetProperty("MD_BATCH_NO").ToString();//批次号
                meterTemp.Address = model.GetProperty("MD_POSTAL_ADDRESS").ToString();//通讯地址-逻辑地址
                meterTemp.MD_PortData = model.GetProperty("MD_PORT_DATA").ToString();//串口数据
                meterTemp.MD_IpAddress = model.GetProperty("MD_IP_ADDRESS").ToString();//IP地址

                meterTemp.MD_TerminalModel = model.GetProperty("MD_TERMINAL_MODEL").ToString();//终端型号

                meterTemp.MD_MeasurementNo = model.GetProperty("MD_MEASUREMENT_NO").ToString();  //计量编号
                meterTemp.MD_CarrierModel = model.GetProperty("MD_CARRIER_MODEL").ToString();   //载波型号
                meterTemp.MD_CollectorAddress = model.GetProperty("MD_COLLECTOR_ADDRESS").ToString();  //采集器地址
                meterTemp.MD_MadtDate = model.GetProperty("MD_MADT_DATE").ToString();  //出厂日期
                //meterTemp.MD_CarrierFactory = model.GetProperty("MD_CREEIER_FACTORY").ToString(); //载波厂家

                //通讯方式
                switch (model.GetProperty("MD_CONN_TYPE").ToString())
                {
                    case "232通讯":
                        meterTemp.MD_ConnType = Cus_EmChannelType.Channel232;
                        break;
                    case "以太网通讯":
                        meterTemp.MD_ConnType = Cus_EmChannelType.ChannelEther;
                        break;
                    case "维护口通讯":
                        meterTemp.MD_ConnType = Cus_EmChannelType.ChannelMaintain;
                        break;
                    default:
                        meterTemp.MD_ConnType = Cus_EmChannelType.ChannelEther;
                        break;
                }
                meterTemp.MD_Protocol_Type = model.GetProperty("MD_PROTOCOL_TYPE").ToString(); //协议类型--376.1--698.45

                //meterTemp.EffectiveDate = model.GetProperty("MD_VALID_DATA").ToString(); //有效期
                meterTemp.VerifyDate = model.GetProperty("MD_TEST_DATE").ToString();//检定日期[YYYY - MM - DD HH: NN:SS]
                meterTemp.Humidity = model.GetProperty("MD_HUMIDITY").ToString();//湿度
                meterTemp.Temperature = model.GetProperty("MD_TEMPERATURE").ToString();//温度
                meterTemp.Checker1 = model.GetProperty("MD_TEST_PERSON").ToString();//检验员
                meterTemp.Checker2 = model.GetProperty("MD_AUDIT_PERSON").ToString();//核验员
                meterTemp.Other1 = model.GetProperty("MD_OTHER_1").ToString(); //备用1
                meterTemp.Other2 = model.GetProperty("MD_OTHER_2").ToString(); //备用2
                meterTemp.Other3 = model.GetProperty("MD_OTHER_3").ToString(); //备用3
                meterTemp.Other4 = model.GetProperty("MD_OTHER_4").ToString(); //备用4
                meterTemp.Other5 = model.GetProperty("MD_OTHER_5").ToString();//备用5

                #endregion
                testMeters[meterTemp.MD_Epitope - 1] = meterTemp;
            }

            return testMeters;
        }


        #region 淘汰
        /// <summary>
        /// 方案参数
        /// </summary>
        //public static Dictionary<string, DynamicModel> Models;
        //public static Dictionary<string, List<string>> ModelsName;

        //public static TestMeterInfo[] GetDnbInfoNew2()
        //{

        //    TestMeterInfo[] meterTemp = GetMeterInfo();
        //    GetModels();
        //    //var s = EquipmentData.MeterGroupInfo;

        //    GetResoult(meterTemp);
        //    for (int i = 0; i < EquipmentData.CheckResults.ResultCollection.Count; i++)
        //    {
        //        CheckNodeViewModel result = EquipmentData.CheckResults.ResultCollection[i];   //结论
        //        string TestValueString = EquipmentData.Schema.ParaValues[i].GetProperty("PARA_VALUE") as string;  //检定的参数
        //        List<string > model = null;
        //        if (ModelsName != null || ModelsName.ContainsKey(result.ParaNo))
        //            model = ModelsName[result.ParaNo];//检定点的格式
        //        Dictionary<string, string> TestValue = new Dictionary<string, string>();
        //        if (model!=null && TestValueString!=null)
        //        {
        //            string[] value = TestValueString.Split('|');//检定用到的参数
        //            for (int j = 0; j < model.Count; j++)
        //            {
        //                if (!TestValue.ContainsKey(model[j]))
        //                {
        //                    TestValue.Add(model[j], value[j]);
        //                }
        //            }
        //        }
        //        switch (result.ParaNo)
        //        {

        //            case ProjectID.基本误差试验://基本误差
        //                GetMeterError(meterTemp, result, TestValue);
        //                break;
        //            case ProjectID.起动试验:
        //                GetMeterQdQid(meterTemp, result, TestValue);
        //                break;
        //            case ProjectID.潜动试验:
        //                GetMeterQdQid(meterTemp, result, TestValue);
        //                break;
        //            case ProjectID.日计时误差:
        //                GetClockError(meterTemp, result, TestValue);
        //                break;
        //            case ProjectID.影响量试验:
        //            case ProjectID.电压基本误差:
        //            case ProjectID.电流基本误差:
        //            case ProjectID.有功功率基本误差:
        //            case ProjectID.无功功率基本误差:
        //            case ProjectID.功率因素基本误差:
        //            //    case ProjectID.需量示值误差:
        //            //        GetDgn(meterTemp, result, TestValue);

        //            case ProjectID.通电检查:
        //            case ProjectID.终端逻辑地址查询:
        //            case ProjectID.终端密钥恢复:
        //            case ProjectID.时钟召测和对时:
        //            case ProjectID.基本参数:
        //            case ProjectID.抄表与费率参数:
        //            case ProjectID.限值与阈值参数:
        //            case ProjectID.控制参数:
        //            case ProjectID.其他参数:
        //            case ProjectID.读取终端信息:
        //            case ProjectID.事件参数:

        //            case ProjectID.状态量采集:
        //            case ProjectID.电能表数据采集:
        //            case ProjectID.分脉冲量采集12个:
        //            case ProjectID.分脉冲量采集120个:
        //            case ProjectID.总加组日和月电量召集:
        //            case ProjectID.分时段电能量数据存储:
        //            case ProjectID.电表日历与状态召集:
        //            case ProjectID.电能表实时数据:
        //            case ProjectID.电能表当前数据:
        //            case ProjectID.电能表当前数据2路:
        //            case ProjectID.电能表当前数据错误MAC:
        //            case ProjectID.终端采集645表计数据:
        //            case ProjectID.HPLC载波通讯:

        //            case ProjectID.实时和当前数据:
        //            case ProjectID.历史日数据:
        //            case ProjectID.负荷曲线:
        //            case ProjectID.历史月数据:
        //            case ProjectID.电压合格率统计:

        //            case ProjectID.时段功控:
        //            case ProjectID.厂休功控:
        //            case ProjectID.营业报停功控:
        //            case ProjectID.当前功率下浮控:
        //            case ProjectID.月电控:
        //            case ProjectID.购电控:
        //            case ProjectID.催费告警:
        //            case ProjectID.保电功能:
        //            case ProjectID.剔除功能:
        //            case ProjectID.遥控功能:

        //            case ProjectID.电能表常数变更事件:
        //            case ProjectID.电能表时段变更事件:
        //            case ProjectID.电能表抄表日变更事件:
        //            case ProjectID.电能表电池欠压事件:
        //            case ProjectID.电能表编程次数变更事件:
        //            case ProjectID.电能表最大需量清零次数变更事件:
        //            case ProjectID.电能表断相次数变更事件:
        //            case ProjectID.电能表示度下降事件:
        //            case ProjectID.电能表超差事件:
        //            case ProjectID.电能表飞走事件:
        //            case ProjectID.电能表停走事件:
        //            case ProjectID.电能表时间超差事件:
        //            case ProjectID.终端参数变更事件:
        //            case ProjectID.电流反向事件:
        //            case ProjectID.电压断相事件:
        //            case ProjectID.电压失压事件:
        //            case ProjectID.终端相序异常事件:
        //            case ProjectID.终端停_上电事件:
        //            case ProjectID.终端停_上电事件_带主动上报:
        //            case ProjectID.电压_电流不平衡度越限事件:
        //            case ProjectID.购电参数设置事件:
        //            case ProjectID.终端485抄表错误:
        //            case ProjectID.电压越限事件:
        //            case ProjectID.电流越限事件:
        //            case ProjectID.视在功率越限事件:
        //            case ProjectID.电能表运行状态字变位事件:
        //            case ProjectID.电能表开表盖事件:
        //            case ProjectID.电能表开端钮盒事件:
        //            case ProjectID.磁场异常事件:
        //            case ProjectID.终端对时事件:
        //            case ProjectID.终端停_上电事件_有效性:
        //            case ProjectID.全事件采集上报:
        //            case ProjectID.电能表对时:
        //            case ProjectID.终端编程事件:
        //            case ProjectID.终端抄表失败:
        //            case ProjectID.电能表数据变更监控记录:

        //            case ProjectID.定时发送1类数据:
        //            case ProjectID.定时发送2类数据:




        //            case ProjectID.频率改变:
        //            case ProjectID.电压改变:
        //            case ProjectID.方形波波形改变:
        //            case ProjectID.尖顶波波形改变:
        //            case ProjectID.间谐波波形改变:
        //            case ProjectID.奇次谐波波形试验:

        //            case ProjectID.改485口为抄表口:
        //            case ProjectID.终端维护:

        //            case ProjectID.身份认证及密钥协商:
        //            case ProjectID.密钥下装:

        //            default:
        //                break;
        //        }
        //    }
        //    return meterTemp;
        //}

        ///// <summary>
        ///// 获得检定参数模型
        ///// </summary>
        //private static void GetModels()
        //{
        //    if (Models != null && ModelsName!=null) return;
        //    Models = new Dictionary<string, DynamicModel>();
        //    ModelsName = new Dictionary<string, List<string>>();
        //    List<DynamicModel> models = DALManager.ApplicationDbDal.GetList(EnumAppDbTable.T_SCHEMA_PARA_FORMAT.ToString());
        //    for (int i = 0; i < models.Count; i++)
        //    {
        //        string name = models[i].GetProperty("PARA_NO").ToString();

        //        if (string.IsNullOrEmpty(name))
        //        {
        //            continue;
        //        }
        //        if (!ModelsName.ContainsKey(name))
        //        {
        //            string str = models[i].GetProperty("PARA_VIEW") as string;
        //            List<string> value=null;
        //            if (str!=null)
        //            {
        //                value = str.Split('|').ToList();
        //            }
        //            ModelsName.Add(name, value);
        //        };
        //        if (!Models.ContainsKey(name))
        //        {
        //            Models.Add(name, models[i]);
        //        };
        //    }


        //}




        ///// <summary>
        ///// 获得总结论
        ///// </summary>
        //private static void GetResoult(TestMeterInfo[] meter)
        //{
        //    //初始化所有的结论都是合格，如果检定过程中有不合格就修改为不合格
        //    List<string> ID = new List<string>();
        //    try
        //    {
        //        FieldInfo[] f_key = typeof(ProjectID).GetFields();
        //        for (int i = 0; i < f_key.Length; i++)
        //        {
        //            ID.Add(f_key[i].GetValue(new ProjectID()).ToString());
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    for (int j = 0; j < ID.Count; j++)
        //    {
        //        string strVaule = ID[j];//获取值
        //        for (int i = 0; i < meter.Length; i++) //循环所有的表，获得这个表的检定结论
        //        {
        //            if (!meter[i].YaoJianYn)
        //            {
        //                continue;
        //            }
        //            if (meter[i].MeterResults.ContainsKey(strVaule))
        //                meter[i].MeterResults.Remove(strVaule);
        //            MeterResult meterResult = new MeterResult();
        //            meterResult.Result = Core.Helper.Const.合格;
        //            meter[i].MeterResults.Add(strVaule, meterResult);
        //        }
        //    }
        //}

        //#region 检定项目结论
        ///// <summary>
        ///// 获得基本误差结论
        ///// </summary>
        ///// <param name="meter">表类</param>
        ///// <param name="CheckResult">检定结论</param>
        ///// <param name="TestValue">检定参数值</param>
        //private static void GetMeterError(TestMeterInfo[] meter, CheckNodeViewModel CheckResult, Dictionary<string, string> TestValue)
        //{

        //    List<string> ModelName = CheckResult.DisplayModel.GetDisplayNames(); //结论的列名

        //    for (int i = 0; i < CheckResult.CheckResults.Count; i++) //循环所有的表，获得这个表的检定结论
        //    {
        //        if (!meter[i].YaoJianYn)
        //        {
        //            continue;
        //        }
        //        MeterError meterError = new MeterError();
        //        Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //        for (int j = 0; j < ModelName.Count; j++)
        //        {
        //            if (!ResultValue.ContainsKey(ModelName[j]))
        //            {
        //                string str = CheckResult.CheckResults[i].GetProperty(ModelName[j]) as string;
        //                ResultValue.Add(ModelName[j], str);
        //            }
        //        }
        //        meterError.GLYS = ResultValue["功率因素"];
        //        meterError.GLFX = ResultValue["功率方向"];
        //        meterError.PrjNo = CheckResult.ItemKey;
        //        meterError.YJ = ResultValue["功率元件"];
        //        meterError.Result = ResultValue["结论"];
        //        //string[] results = result.Split('|');
        //        //meterError.Result = results[0];
        //        //meterError.AVR_DIS_REASON = results[1];
        //        meterError.IbX = ResultValue["电流倍数"];
        //        if (meterError.IbX == "Ib")
        //        {
        //            meterError.IbX = "1.0Ib";
        //        }
        //        meterError.WCData = ResultValue["误差1"] + "|" + ResultValue["误差2"];
        //        meterError.WCHZ = ResultValue["化整值"];
        //        meterError.WCValue = ResultValue["平均值"];
        //        meterError.Limit = ResultValue["误差上限"] + "|" + ResultValue["误差下限"];
        //        meterError.BPHUpLimit = ResultValue["误差上限"];
        //        meterError.BPHDownLimit = ResultValue["误差下限"];
        //        meterError.Circle = ResultValue["误差圈数"];

        //        if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //        {
        //            meterError.Result = "不合格";
        //        }
        //        if (meter[i].MeterErrors.ContainsKey(CheckResult.ItemKey))
        //            meter[i].MeterErrors.Remove(CheckResult.ItemKey);
        //        meter[i].MeterErrors.Add(CheckResult.ItemKey, meterError);

        //        //如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //        if (meterError.Result == "不合格" && meter[i].MeterResults[CheckResult.ParaNo].Result=="合格")
        //        {
        //            meter[i].MeterResults[CheckResult.ParaNo].Result = Core.Helper.Const.不合格;
        //        }
        //    }
        //}
        ///// <summary>
        /////走字试验
        ///// </summary>
        ///// <param name="meter"></param>
        ///// <param name="colunmName"></param>
        ///// <param name="value"></param>
        ///// <param name="result"></param>
        ///// <param name="KeyNo"></param>
        //private static void GetMeterZZError(TestMeterInfo[] meter, CheckNodeViewModel CheckResult, Dictionary<string, string> TestValue)
        //{
        //    List<string> ModelName = CheckResult.DisplayModel.GetDisplayNames(); //结论的列名
        //    for (int i = 0; i < CheckResult.CheckResults.Count; i++) //循环所有的表，获得这个表的检定结论
        //    {
        //        if (!meter[i].YaoJianYn)
        //        {
        //            continue;
        //        }
        //        MeterZZError meterError = new MeterZZError();
        //        Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //        for (int j = 0; j < ModelName.Count; j++)
        //        {
        //            if (!ResultValue.ContainsKey(ModelName[j]))
        //            {
        //                string str = CheckResult.CheckResults[i].GetProperty(ModelName[j]) as string;
        //                ResultValue.Add(ModelName[j], str);
        //            }
        //        }
        //        meterError.Result = ResultValue["结论"];
        //        meterError.Fl = TestValue["费率"];
        //        meterError.GLYS = TestValue["功率因素"];
        //        meterError.IbX = TestValue["电流倍数"];
        //        if (meterError.IbX == "Ib")
        //        {
        //            meterError.IbX = "1.0Ib";
        //        }
        //        meterError.PowerWay = TestValue["功率方向"];
        //        meterError.TestWay = TestValue["走字试验方法类型"];
        //        meterError.YJ = TestValue["功率元件"];
        //        meterError.NeedEnergy = TestValue["走字电量(度)"];
        //        meterError.PowerSumEnd = ResultValue["起码"];
        //        meterError.PowerSumStart = ResultValue["止码"];
        //        meterError.WarkPower = "0";
        //        if (meterError.PowerSumEnd != "" && meterError.PowerSumStart != "")
        //        {
        //            meterError.WarkPower = (float.Parse(meterError.PowerSumStart) - float.Parse(meterError.PowerSumEnd)).ToString("f5");
        //        }
        //        meterError.PowerError = ResultValue["表码差"];
        //        meterError.STMEnergy = ResultValue["标准表脉冲"];  // ResultValue["标准表脉冲"]/d_meterInfo.GetBcs()[0])
        //        meterError.STMEnergy = ResultValue["标准表脉冲"];  // ResultValue["标准表脉冲"]/d_meterInfo.GetBcs()[0])
        //        meterError.Pules = ResultValue["表脉冲"];
        //        if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //        {
        //            meterError.Result = "不合格";
        //        }
        //        if (meter[i].MeterZZErrors.ContainsKey(CheckResult.ItemKey))
        //            meter[i].MeterZZErrors.Remove(CheckResult.ItemKey);
        //        meter[i].MeterZZErrors.Add(CheckResult.ItemKey, meterError);
        //        //如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //        if (meterError.Result == "不合格" && meter[i].MeterResults[CheckResult.ParaNo].Result == "合格")
        //        {
        //            meter[i].MeterResults[CheckResult.ParaNo].Result = Core.Helper.Const.不合格;
        //        }
        //    }
        //}

        ///// <summary>
        /////启动潜动试验
        ///// </summary>
        ///// <param name="meter"></param>
        ///// <param name="colunmName"></param>
        ///// <param name="value"></param>
        ///// <param name="result"></param>
        ///// <param name="KeyNo"></param>
        //private static void GetMeterQdQid(TestMeterInfo[] meter, CheckNodeViewModel CheckResult, Dictionary<string, string> TestValue)
        //{
        //    List<string> ModelName = CheckResult.DisplayModel.GetDisplayNames(); //结论的列名
        //    for (int i = 0; i < CheckResult.CheckResults.Count; i++) //循环所有的表，获得这个表的检定结论
        //    {
        //        if (!meter[i].YaoJianYn)
        //        {
        //            continue;
        //        }
        //        MeterQdQid meterError = new MeterQdQid();
        //        Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //        for (int j = 0; j < ModelName.Count; j++)
        //        {
        //            if (!ResultValue.ContainsKey(ModelName[j]))
        //            {
        //                string str = CheckResult.CheckResults[i].GetProperty(ModelName[j]) as string;
        //                ResultValue.Add(ModelName[j], str);
        //            }
        //        }
        //        meterError.ActiveTime = ResultValue["实际运行时间"].Trim('分');
        //        meterError.PowerWay = ResultValue["功率方向"];
        //        meterError.Voltage = ResultValue["试验电压"];
        //        meterError.TimeEnd = ResultValue["结束时间"];
        //        meterError.TimeStart = ResultValue["开始时间"];
        //        meterError.TimeStart = ResultValue["开始时间"];
        //        meterError.StandartTime = ResultValue["标准试验时间"];
        //        meterError.Current = ResultValue["试验电流"];
        //        meterError.Result = ResultValue["结论"];
        //        if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //        {
        //            meterError.Result = "不合格";
        //        }
        //        if (meter[i].MeterQdQids.ContainsKey(CheckResult.ItemKey))
        //            meter[i].MeterQdQids.Remove(CheckResult.ItemKey);
        //        meter[i].MeterQdQids.Add(CheckResult.ItemKey, meterError);
        //        //如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //        if (meterError.Result == "不合格" && meter[i].MeterResults[CheckResult.ParaNo].Result == "合格")
        //        {
        //            meter[i].MeterResults[CheckResult.ParaNo].Result = Core.Helper.Const.不合格;
        //        }
        //    }
        //}

        ///// <summary>
        /////日记时试验
        ///// </summary>
        ///// <param name="meter"></param>
        ///// <param name="colunmName"></param>
        ///// <param name="value"></param>
        ///// <param name="result"></param>
        ///// <param name="KeyNo"></param>            +
        //private static void GetClockError(TestMeterInfo[] meter, CheckNodeViewModel CheckResult, Dictionary<string, string> TestValue)
        //{
        //    List<string> ModelName = CheckResult.DisplayModel.GetDisplayNames(); //结论的列名
        //    for (int i = 0; i < CheckResult.CheckResults.Count; i++) //循环所有的表，获得这个表的检定结论
        //    {
        //        if (!meter[i].YaoJianYn)
        //        {
        //            continue;
        //        }
        //        MeterDgn meterError = new MeterDgn();
        //        Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //        for (int j = 0; j < ModelName.Count; j++)
        //        {
        //            if (!ResultValue.ContainsKey(ModelName[j]))
        //            {
        //                string str = CheckResult.CheckResults[i].GetProperty(ModelName[j]) as string;
        //                ResultValue.Add(ModelName[j], str);
        //            }
        //        }
        //        List<string> list = new List<string>();
        //        foreach (string  item in ResultValue.Keys)
        //        {
        //            if (item!= "结论")
        //            {
        //                list.Add(item);
        //            }
        //        }
        //        foreach (string item in TestValue.Keys)
        //        {
        //           list.Add(item);
        //        }
        //        meterError.Value = string.Join("|", list);
        //        meterError.Result = ResultValue["结论"];
        //        if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //        {
        //            meterError.Result = "不合格";
        //        }
        //        if (meter[i].MeterDgns.ContainsKey(CheckResult.ItemKey))
        //            meter[i].MeterDgns.Remove(CheckResult.ItemKey);
        //        meter[i].MeterDgns.Add(CheckResult.ItemKey, meterError);
        //        //如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //        if (meterError.Result == "不合格" && meter[i].MeterResults[CheckResult.ParaNo].Result == "合格")
        //        {
        //            meter[i].MeterResults[CheckResult.ParaNo].Result = Core.Helper.Const.不合格;
        //        }
        //    }
        //}

        ///// <summary>
        /////费控试验
        ///// </summary>
        ///// <param name="meter"></param>
        ///// <param name="colunmName"></param>
        ///// <param name="value"></param>
        ///// <param name="result"></param>
        ///// <param name="KeyNo"></param>
        //private static void GetMeterFK(TestMeterInfo[] meter, CheckNodeViewModel CheckResult, Dictionary<string, string> TestValue)
        //{
        //    List<string> ModelName = CheckResult.DisplayModel.GetDisplayNames(); //结论的列名
        //    for (int i = 0; i < CheckResult.CheckResults.Count; i++) //循环所有的表，获得这个表的检定结论
        //    {
        //        if (!meter[i].YaoJianYn)
        //        {
        //            continue;
        //        }
        //        MeterFK meterError = new MeterFK();
        //        Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //        for (int j = 0; j < ModelName.Count; j++)
        //        {
        //            if (!ResultValue.ContainsKey(ModelName[j]))
        //            {
        //                string str = CheckResult.CheckResults[i].GetProperty(ModelName[j]) as string;
        //                ResultValue.Add(ModelName[j], str);
        //            }
        //        }
        //        List<string> list = new List<string>();
        //        foreach (string item in ResultValue.Keys)
        //        {
        //            if (item != "结论")
        //            {
        //                list.Add(item);
        //            }
        //        }
        //        switch (CheckResult.ItemKey)
        //        {
        //            //case ProjectID.远程控制:
        //            //    meterError.Name = "远程控制";
        //            //    meterError.Data = string.Join("|", list);
        //            //    break;
        //            //case ProjectID.报警功能:
        //            //    meterError.Name = "报警功能";
        //            //    meterError.Data = string.Join("|", list);
        //            //    break;
        //            //case ProjectID.远程保电:
        //            case "保电解除":
        //            case "密钥更新":
        //                meterError.Name = ResultValue["当前项目"];
        //                meterError.Data = ResultValue["检定信息"];
        //                break;
        //            default:  //费控通用
        //                meterError.Name = ResultValue["当前项目"];
        //                meterError.Data = ResultValue["检定信息"];
        //                break;
        //        }

        //        meterError.Result = ResultValue["结论"];
        //        if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //        {
        //            meterError.Result = "不合格";
        //        }
        //        if (meter[i].MeterCostControls.ContainsKey(CheckResult.ItemKey))
        //            meter[i].MeterCostControls.Remove(CheckResult.ItemKey);
        //        meter[i].MeterCostControls.Add(CheckResult.ItemKey, meterError);
        //        //如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //        if (meterError.Result == "不合格" && meter[i].MeterResults[CheckResult.ParaNo].Result == "合格")
        //        {
        //            meter[i].MeterResults[CheckResult.ParaNo].Result = Core.Helper.Const.不合格;
        //        }
        //    }
        //}

        ///// <summary>
        /////多功能试验
        ///// </summary>
        ///// <param name="meter"></param>
        ///// <param name="colunmName"></param>
        ///// <param name="value"></param>
        ///// <param name="result"></param>
        ///// <param name="KeyNo"></param>            +
        //private static void GetDgn(TestMeterInfo[] meter, CheckNodeViewModel CheckResult, Dictionary<string, string> TestValue)
        //{


        //    List<string> ModelName = CheckResult.DisplayModel.GetDisplayNames(); //结论的列名
        //    for (int i = 0; i < CheckResult.CheckResults.Count; i++) //循环所有的表，获得这个表的检定结论
        //    {
        //        if (!meter[i].YaoJianYn)
        //        {
        //            continue;
        //        }
        //        MeterDgn meterError = new MeterDgn();
        //        Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //        for (int j = 0; j < ModelName.Count; j++)
        //        {
        //            if (!ResultValue.ContainsKey(ModelName[j]))
        //            {
        //                string str = CheckResult.CheckResults[i].GetProperty(ModelName[j]) as string;
        //                ResultValue.Add(ModelName[j], str);
        //            }
        //        }
        //        List<string> list = new List<string>();
        //        foreach (string item in ResultValue.Keys)
        //        {
        //            if (item != "结论")
        //            {
        //                list.Add(item);
        //            }
        //        }
        //        meterError.Value = string.Join("|", list);
        //        meterError.Result = ResultValue["结论"];
        //        if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //        {
        //            meterError.Result = "不合格";
        //        }
        //        if (meter[i].MeterDgns.ContainsKey(CheckResult.ItemKey))
        //            meter[i].MeterDgns.Remove(CheckResult.ItemKey);
        //        meter[i].MeterDgns.Add(CheckResult.ItemKey, meterError);
        //        //如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //        if (meterError.Result == "不合格" && meter[i].MeterResults[CheckResult.ParaNo].Result == "合格")
        //        {
        //            meter[i].MeterResults[CheckResult.ParaNo].Result = Core.Helper.Const.不合格;
        //        }
        //    }
        //}


        ///// <summary>
        ///// 误差一致性
        ///// </summary>
        ///// <param name="meter"></param>
        ///// <param name="colunmName"></param>
        ///// <param name="value"></param>
        ///// <param name="result"></param>
        ///// <param name="KeyNo"></param>
        ///// <param name="Parameter_colName"></param>
        ///// <param name="Parameter_Value"></param>
        //private static void GetMeterErrAccord(TestMeterInfo[] meter, CheckNodeViewModel CheckResult, Dictionary<string, string> TestValue)
        //{
        //    string index = "";
        //    //switch (CheckResult.ItemKey)
        //    //{
        //    //    case ProjectID.误差一致性:
        //    //        index = "1";
        //    //        break;
        //    //    case ProjectID.误差变差:
        //    //        index = "2";
        //    //        break;
        //    //    case ProjectID.负载电流升将变差:
        //    //        index = "3";
        //    //        break;
        //    //    default:
        //    //        break;
        //    //}
        //    List<string> ModelName = CheckResult.DisplayModel.GetDisplayNames(); //结论的列名
        //    for (int i = 0; i < CheckResult.CheckResults.Count; i++) //循环所有的表，获得这个表的检定结论
        //    {
        //        if (!meter[i].YaoJianYn)
        //        {
        //            continue;
        //        }
        //        MeterErrAccord meterError = new MeterErrAccord();
        //        if (meter[i].MeterErrAccords.ContainsKey(index))   //先判断是不是有了，有了在原来基础上添加
        //            meterError = meter[i].MeterErrAccords[index];
        //        else
        //            meter[i].MeterErrAccords.Add(index, meterError);


        //        Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //        for (int j = 0; j < ModelName.Count; j++)
        //        {
        //            if (!ResultValue.ContainsKey(ModelName[j]))
        //            {
        //                string str = CheckResult.CheckResults[i].GetProperty(ModelName[j]) as string;
        //                ResultValue.Add(ModelName[j], str);
        //            }
        //        }
        //        MeterErrAccordBase meterErr = new MeterErrAccordBase();

        //        meterErr.Freq = meter[i].MD_Frequency.ToString();
        //        meterError.Result = ResultValue["结论"];
        //        if (index=="1")    //补上圈数和误差限
        //        {
        //            meterErr.Name = "误差一致性";
        //            //误差1|误差2|平均值|化整值|样品均值|差值
        //            meterErr.IbX = TestValue["电流"];  //电流
        //            if (TestValue["电流"] == "Ib")
        //            {
        //                meterErr.IbX = "1.0Ib";
        //            }
        //            meterErr.PF = TestValue["功率因素"];   //功率因素
        //            meterErr.PulseCount = ResultValue["检定圈数"];
        //            meterErr.Limit = ResultValue["误差上限"] + "|" + ResultValue["误差下限"];
        //            meterErr.Data1 = ResultValue["误差1"] + "|" + ResultValue["误差2"] + "|" + ResultValue["平均值"] + "|" + ResultValue["化整值"];
        //            meterErr.ErrAver = ResultValue["样品均值"];
        //            meterErr.Error = ResultValue["差值"];

        //            int count = 1;
        //            if (meter[i].MeterErrAccords.ContainsKey(index))
        //                count = meter[i].MeterErrAccords[index].PointList.Keys.Count + 1;
        //            meterError.PointList.Add(count.ToString(), meterErr);
        //        }
        //        else if (index == "2")    //1.0IB，
        //        {
        //            meterErr.Name = "误差变差";
        //            meterErr.PulseCount = ResultValue["检定圈数"];
        //            meterErr.Limit = ResultValue["误差上限"] + "|" + ResultValue["误差下限"];
        //            meterErr.IbX = "1.0Ib";  //电流
        //            meterErr.PF = TestValue["功率因素"];  //功率因素
        //                                              //第一次误差1|第一次误差2|第一次平均值|第一次化整值|第二次误差1|第二次误差2|第二次平均值|第二次化整值|变差值
        //            meterErr.Data1 = ResultValue["第一次误差1"] + "|" + ResultValue["第一次误差2"] + "|" + ResultValue["第一次平均值"] + "|" + ResultValue["第一次化整值"];
        //            meterErr.Data2 = ResultValue["第二次误差1"] + "|" + ResultValue["第二次误差2"] + "|" + ResultValue["第二次平均值"] + "|" + ResultValue["第二次化整值"];
        //            meterErr.Error = ResultValue["变差值"];
        //            int count = 1;
        //            if (meter[i].MeterErrAccords.ContainsKey(index))
        //                count = meter[i].MeterErrAccords[index].PointList.Keys.Count + 1;
        //            meterError.PointList.Add(count.ToString(), meterErr);
        //        }
        //        else if (index == "3")   //这个需要加个判断，判断子项是否合格
        //        {

        //            string[] str = new string[] { "01Ib", "Ib", "Imax" };
        //            for (int j = 0; j < 3; j++)
        //            {
        //                meterErr = new MeterErrAccordBase();
        //                meterErr.Freq = meter[i].MD_Frequency.ToString();
        //                meterErr.Name = "负载电流升降变差";
        //                meterErr.PulseCount = ResultValue[str[j] + "检定圈数"];
        //                meterErr.IbX = str[j];  //电流
        //                if (meterErr.IbX == "Ib")
        //                {
        //                    meterErr.IbX = "1.0Ib";
        //                }
        //                if (str[j] == "01Ib") meterErr.IbX = "0.1Ib";
        //                meterErr.PF = "1.0";   //功率因素
        //                meterErr.Data1 = ResultValue[str[j] + "上升误差1"] + "|" + ResultValue[str[j] + "上升误差2"] + "|" + ResultValue[str[j] + "上升平均值"] + "|" + ResultValue[str[j] + "上升化整值"];
        //                meterErr.Data2 = ResultValue[str[j] + "下降误差1"] + "|" + ResultValue[str[j] + "下降误差2"] + "|" + ResultValue[str[j] + "下降平均值"] + "|" + ResultValue[str[j] + "下降化整值"];
        //                meterErr.Error = ResultValue[str[j] + "差值"];
        //                meterErr.Result = Core.Helper.Const.不合格;
        //                if (!string.IsNullOrEmpty(meterErr.Error))
        //                {
        //                    float t = float.Parse(meterErr.Error);
        //                    if (Math.Abs(t) <= 0.25)
        //                        meterErr.Result = Core.Helper.Const.合格;
        //                }
        //                meterError.PointList.Add((j + 1).ToString(), meterErr);
        //            }
        //        }
        //        meterError.Result = Core.Helper.Const.合格;
        //        for (int j = 1; j <= meterError.PointList.Count; j++)
        //        {
        //            if (meterError.PointList[j.ToString()].Result == Core.Helper.Const.不合格)     //有一个点不合格，总结论不合格
        //            {
        //                meterError.Result = Core.Helper.Const.不合格;
        //            }
        //        }
        //        if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //        {
        //            meterError.Result = "不合格";
        //        }
        //        //如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //        if (meterError.Result == "不合格" && meter[i].MeterResults[CheckResult.ParaNo].Result == "合格")
        //        {
        //            meter[i].MeterResults[CheckResult.ParaNo].Result = Core.Helper.Const.不合格;
        //        }
        //    }

        //}


        ///// <summary>
        /////规约一致性（通讯协议检查）
        ///// </summary>
        ///// <param name="meter"></param>
        ///// <param name="colunmName"></param>
        ///// <param name="value"></param>
        ///// <param name="result"></param>
        ///// <param name="KeyNo"></param>            +
        //private static void GetMeterDLTData(TestMeterInfo[] meter, CheckNodeViewModel CheckResult, Dictionary<string, string> TestValue)
        //{
        //    List<string> ModelName = CheckResult.DisplayModel.GetDisplayNames(); //结论的列名
        //    for (int i = 0; i < CheckResult.CheckResults.Count; i++) //循环所有的表，获得这个表的检定结论
        //    {
        //        if (!meter[i].YaoJianYn)
        //        {
        //            continue;
        //        }
        //        MeterDLTData meterDLTData = new MeterDLTData();
        //        Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //        for (int j = 0; j < ModelName.Count; j++)
        //        {
        //            if (!ResultValue.ContainsKey(ModelName[j]))
        //            {
        //                string str = CheckResult.CheckResults[i].GetProperty(ModelName[j]) as string;
        //                ResultValue.Add(ModelName[j], str);
        //            }
        //        }
        //        meterDLTData.DataFlag = TestValue["标识编码"]; //645
        //        meterDLTData.DataFormat = TestValue["数据格式"];
        //        meterDLTData.DataLen = TestValue["长度"];
        //        meterDLTData.FlagMsg = TestValue["数据项名称"];
        //        meterDLTData.StandardValue = TestValue["写入内容"];
        //        meterDLTData.Value = ResultValue["检定信息"];
        //        meterDLTData.Result = ResultValue["结论"];
        //        if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //        {
        //            meterDLTData.Result = "不合格";
        //        }
        //        if (meter[i].MeterDLTDatas.ContainsKey(CheckResult.ItemKey))
        //            meter[i].MeterDLTDatas.Remove(CheckResult.ItemKey);
        //        meter[i].MeterDLTDatas.Add(CheckResult.ItemKey, meterDLTData);
        //        //如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //        if (meterDLTData.Result == "不合格" && meter[i].MeterResults[CheckResult.ParaNo].Result == "合格")
        //        {
        //            meter[i].MeterResults[CheckResult.ParaNo].Result = Core.Helper.Const.不合格;
        //        }
        //    }
        //}

        ///// <summary>
        /////规约一致性（通讯协议检查）
        ///// </summary>
        ///// <param name="meter"></param>
        ///// <param name="colunmName"></param>
        ///// <param name="value"></param>
        ///// <param name="result"></param>
        ///// <param name="KeyNo"></param>           
        //private static void GetMeterDLTData2(TestMeterInfo[] meter, CheckNodeViewModel CheckResult, Dictionary<string, string> TestValue)
        //{
        //    List<string> ModelName = CheckResult.DisplayModel.GetDisplayNames(); //结论的列名
        //    for (int i = 0; i < CheckResult.CheckResults.Count; i++) //循环所有的表，获得这个表的检定结论
        //    {
        //        if (!meter[i].YaoJianYn)
        //        {
        //            continue;
        //        }
        //        MeterDLTData meterDLTData = new MeterDLTData();
        //        Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //        for (int j = 0; j < ModelName.Count; j++)
        //        {
        //            if (!ResultValue.ContainsKey(ModelName[j]))
        //            {
        //                string str = CheckResult.CheckResults[i].GetProperty(ModelName[j]) as string;
        //                ResultValue.Add(ModelName[j], str);
        //            }
        //        }
        //        List<string> list = new List<string>();
        //        foreach (string item in ResultValue.Keys)
        //        {
        //            if (item != "结论")
        //            {
        //                list.Add(item);
        //            }
        //        }
        //        //if (meter[i].MD_ProtocolName.IndexOf("645") != -1)
        //        //{
        //        //    meterDLTData.DataFlag = TestValue["标识编码"]; //645
        //        //}
        //        //else
        //        //{
        //        //    meterDLTData.DataFlag = TestValue["标识编码698"]; //698
        //        //}
        //        meterDLTData.DataFormat = TestValue["数据格式"];
        //        meterDLTData.DataLen = TestValue["长度"];
        //        meterDLTData.FlagMsg = TestValue["数据项名称"];
        //        meterDLTData.StandardValue = TestValue["写入内容"];

        //        meterDLTData.Value = ResultValue["检定信息"];
        //        meterDLTData.Result = ResultValue["结论"];
        //        if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //        {
        //            meterDLTData.Result = "不合格";
        //        }
        //        if (meter[i].MeterDLTDatas.ContainsKey(CheckResult.ItemKey))
        //            meter[i].MeterDLTDatas.Remove(CheckResult.ItemKey);
        //        meter[i].MeterDLTDatas.Add(CheckResult.ItemKey, meterDLTData);
        //        //如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //        if (meterDLTData.Result == "不合格" && meter[i].MeterResults[CheckResult.ParaNo].Result == "合格")
        //        {
        //            meter[i].MeterResults[CheckResult.ParaNo].Result = Core.Helper.Const.不合格;
        //        }
        //    }
        //}

        //#endregion

        #endregion

    }
}
