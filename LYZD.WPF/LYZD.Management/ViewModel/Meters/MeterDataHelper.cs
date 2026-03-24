using LYZD.Core.Enum;
using LYZD.Core.Model.DnbModel.DnbInfo;
using LYZD.Core.Model.Meter;
using LYZD.DAL;
using LYZD.ViewModel;
using LYZD.ViewModel.CheckInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LYZD.DataManager.ViewModel.Meters
{
    public class MeterDataHelper
    {

        public static TestMeterInfo GetDnbInfoNew(OneMeterResult meterResult)
        {
            DynamicViewModel model = meterResult.MeterInfo;
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
            meterTemp.Other1 = model.GetProperty("MD_OTHER_1").ToString(); //备用1--是否需要上传
            meterTemp.Other2 = model.GetProperty("MD_OTHER_2").ToString(); //备用2--上传的标识
            meterTemp.Other3 = model.GetProperty("MD_OTHER_3").ToString(); //备用3
            meterTemp.Other4 = model.GetProperty("MD_OTHER_4").ToString(); //备用4
            meterTemp.Other5 = model.GetProperty("MD_OTHER_5").ToString();//备用5

            #endregion


            for (int i = 0; i < meterResult.Categories.Count; i++)
            {
                for (int j = 0; j < meterResult.Categories[i].ResultUnits.Count; j++)
                {
                    DynamicViewModel result = meterResult.Categories[i].ResultUnits[j];
                    string id = result.GetProperty("项目号").ToString(); //这个项目号的详细的项目号，需要分割找到大类编号
                    string DID = id.Split('_')[0];
                    if (!meterTemp.MeterResoultData.ContainsKey(DID))
                    {
                        meterTemp.MeterResoultData.Add(DID, new MeterResoultData());
                    }
                    MeterItemResoultData resultData = new MeterItemResoultData();
                    resultData.ID = id;
                    List<string> names = meterResult.Categories[i].Names.ToList();
                    //这里先排除不是对应协议的部分
                    string RemoveProtocol = "376";  //需要删除的协议
                    if (meterTemp.MD_Protocol_Type == "376.1")
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
                        string value = result.GetProperty(names[index]) as string;
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
                                meterTemp.MeterResoultData[resultData.ID].Result = "不合格";

                            }
                        }
                        else
                        {

                            resultData.Datas.Add(names[index], value);
                        }
                    }
                    meterTemp.MeterResoultData[DID].meterResoults.Add(resultData);

                }
            }

            return meterTemp;
        }



        #region 淘汰
        ///// <summary>
        ///// 方案参数
        ///// </summary>
        //public static Dictionary<string, DynamicModel> Models;
        //public static Dictionary<string, List<string>> ModelsName;
        ///// <summary>
        ///// 获得检定参数模型
        ///// </summary>
        //private static void GetModels()
        //{
        //    if (Models != null && ModelsName != null) return;
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
        //            List<string> value = null;
        //            if (str != null)
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
        //#endregion
        //#region 新--优化速度
        //public static TestMeterInfo GetDnbInfoNew(OneMeterResult meterResult)
        //{
        //    string str = "";
        //    DynamicViewModel model = meterResult.MeterInfo;
        //    TestMeterInfo meterTemp = new TestMeterInfo();
        //    #region 基本信息
        //    meterTemp.Meter_ID = model.GetProperty("METER_ID").ToString();
        //    meterTemp.MD_DeviceID = model.GetProperty("MD_DEVICE_ID").ToString();  //台体编号     MD_DEVICE_ID
        //    meterTemp.MD_Epitope = int.Parse(model.GetProperty("MD_EPITOPE").ToString());//表位号
        //    meterTemp.Result = model.GetProperty("MD_RESULT").ToString();  //表结论
        //    meterTemp.YaoJianYn = model.GetProperty("MD_CHECKED").ToString() == "1" ? true : false;
        //    meterTemp.MD_UB = float.Parse(model.GetProperty("MD_UB").ToString());
        //    meterTemp.MD_UA = model.GetProperty("MD_UA").ToString();
        //    meterTemp.MD_Frequency = int.Parse(model.GetProperty("MD_FREQUENCY").ToString());//频率
        //    meterTemp.MD_TestType = model.GetProperty("MD_TEST_TYPE").ToString();//检定类型---全性能
        //    meterTemp.MD_WiringMode = model.GetProperty("MD_WIRING_MODE").ToString();//测量方式-三相三。三相四
        //    meterTemp.MD_ConnectionFlag = model.GetProperty("MD_CONNECTION_FLAG").ToString();//直接还是互感
        //    meterTemp.MD_BarCode = model.GetProperty("MD_BAR_CODE").ToString(); //条形码
        //    meterTemp.MD_Constant = model.GetProperty("MD_CONSTANT").ToString();//常数
        //    meterTemp.MD_Grane = model.GetProperty("MD_GRADE").ToString();//等级
        //    meterTemp.MD_Factory = model.GetProperty("MD_FACTORY").ToString();//制作厂家
        //    meterTemp.MD_Customer = model.GetProperty("MD_CUSTOMER").ToString();//送检单位
        //    meterTemp.MD_TaskNo = model.GetProperty("MD_TASK_NO").ToString();//任务编号
        //    meterTemp.MD_MadeNo = model.GetProperty("MD_MADE_NO").ToString();//出厂编号
        //    meterTemp.MD_CertificateNo = model.GetProperty("MD_CERTIFICATE_NO").ToString();//证书编号
        //    meterTemp.MD_SchemeID = model.GetProperty("MD_SCHEME_ID").ToString();//方案编号
        //    meterTemp.MD_TerminalType  = model.GetProperty("MD_TERMINAL_TYPE").ToString();//终端类--集中器-专变
        //    meterTemp.MD_BatchNo = model.GetProperty("MD_BATCH_NO").ToString();//批次号
        //    meterTemp.Address = model.GetProperty("MD_POSTAL_ADDRESS").ToString();//通讯地址-逻辑地址
        //    meterTemp.MD_PortData = model.GetProperty("MD_PORT_DATA").ToString();//串口数据
        //    meterTemp.MD_IpAddress = model.GetProperty("MD_IP_ADDRESS").ToString();//IP地址

        //    meterTemp.MD_TerminalModel = model.GetProperty("MD_TERMINAL_MODEL").ToString();//终端型号

        //   meterTemp.MD_MeasurementNo = model.GetProperty("MD_MEASUREMENT_NO").ToString();  //计量编号
        //    meterTemp.MD_CarrierModel = model.GetProperty("MD_CARRIER_MODEL").ToString();   //载波型号
        //   meterTemp.MD_CollectorAddress = model.GetProperty("MD_COLLECTOR_ADDRESS").ToString();  //采集器地址
        //   meterTemp.MD_MadtDate = model.GetProperty("MD_MADT_DATE").ToString();  //出厂日期
        //   meterTemp.MD_CarrierFactory = model.GetProperty("MD_CREEIER_FACTORY").ToString(); //载波厂家

        //    //通讯方式
        //    switch (model.GetProperty("MD_CONN_TYPE").ToString())
        //    {
        //        case "232通讯":
        //            meterTemp.MD_ConnType = Cus_EmChannelType.Channel232;
        //            break;
        //        case "以太网通讯":
        //            meterTemp.MD_ConnType = Cus_EmChannelType.ChannelEther;
        //            break;
        //        case "维护口通讯":
        //            meterTemp.MD_ConnType = Cus_EmChannelType.ChannelMaintain;
        //            break;
        //        default:
        //            meterTemp.MD_ConnType = Cus_EmChannelType.ChannelEther;
        //            break;
        //    }
        //    meterTemp.MD_Protocol_Type = model.GetProperty("MD_PROTOCOL_TYPE").ToString(); //协议类型--376.1--698.45

        //    meterTemp.EffectiveDate = model.GetProperty("MD_VALID_DATA").ToString(); //有效期
        //    meterTemp.VerifyDate = model.GetProperty("MD_TEST_DATE").ToString();//检定日期[YYYY - MM - DD HH: NN:SS]
        //    meterTemp.Humidity = model.GetProperty("MD_HUMIDITY").ToString();//湿度
        //    meterTemp.Temperature = model.GetProperty("MD_TEMPERATURE").ToString();//温度
        //    meterTemp.Checker1 = model.GetProperty( "MD_TEST_PERSON").ToString();//检验员
        //    meterTemp.Checker2 = model.GetProperty("MD_AUDIT_PERSON").ToString();//核验员
        //    meterTemp.Other1 = model.GetProperty( "MD_OTHER_1").ToString(); //备用1--是否需要上传
        //    meterTemp.Other2 = model.GetProperty( "MD_OTHER_2").ToString(); //备用2--上传的标识
        //    meterTemp.Other3 = model.GetProperty( "MD_OTHER_3").ToString(); //备用3
        //    meterTemp.Other4 = model.GetProperty( "MD_OTHER_4").ToString(); //备用4
        //    meterTemp.Other5 = model.GetProperty("MD_OTHER_5").ToString();//备用5

        //    #endregion
        //    List<DynamicModel> models = new List<DynamicModel>();
        //    GeneralDal dal = DALManager.SchemaDal;
        //    List<string> tableNames = dal.GetTableNames();
        //    tableNames.Remove("T_SCHEMA_INFO");
        //    List<string> sqlList = new List<string>();
        //    string schemdID = model.GetProperty("MD_SCHEME_ID").ToString();  //方案id

        //    for (int i = 0; i < tableNames.Count; i++)
        //    {
        //        sqlList.Add(string.Format("select * from {0} where SCHEMA_ID={1}", tableNames[i], schemdID));
        //    }
        //    models = dal.GetList(tableNames, sqlList);

        //    GetModels();
        //    GetResoult(meterTemp);

        //    for (int i = 0; i < meterResult.Categories.Count; i++)
        //    {
        //        for (int j = 0; j < meterResult.Categories[i].ResultUnits.Count; j++)
        //        {
        //            DynamicViewModel result = meterResult.Categories[i].ResultUnits[j];
        //            string id = result.GetProperty("项目号").ToString(); //这个项目号的详细的项目号，需要分割找到大类编号
        //            string DID = id.Split('_')[0];


        //            List<string> model2 = null;
        //            if (ModelsName != null || ModelsName.ContainsKey(DID))
        //                model2 = ModelsName[DID];//检定点的格式

        //            //这个就是找到的检定参数了
        //            DynamicModel TestValue2 = models.Find(a => a.GetProperty("PARA_VALUE_NO").ToString() == id);
        //            string testv = "";
        //            if (TestValue2 != null)
        //            {
        //                testv = TestValue2.GetProperty("PARA_VALUE") as string;
        //            }
        //            Dictionary<string, string> TestValue = new Dictionary<string, string>();
        //            if (model2 != null && testv != null)
        //            {
        //                string[] value = testv.Split('|');//检定用到的参数
        //                if (model2.Count == value.Length)
        //                {
        //                    for (int n = 0; n < model2.Count; n++)
        //                    {
        //                        if (!TestValue.ContainsKey(model2[n]))
        //                        {
        //                            TestValue.Add(model2[n], value[n]);
        //                        }
        //                    }
        //                }
        //            }


        //            //switch (DID)
        //            //{
        //              //case ProjectID.接线检查:
        //            //        break;
        //            //    case ProjectID.基本误差试验://基本误差
        //            //        GetMeterError(meterTemp, result, TestValue);
        //            //        break;
        //            //    case ProjectID.起动试验:
        //            //        GetMeterQdQid(meterTemp, result, TestValue);
        //            //        break;
        //            //    case ProjectID.潜动试验:
        //            //        GetMeterQdQid(meterTemp, result, TestValue);
        //            //        break;
        //            //    case ProjectID.电能表常数试验:
        //            //        GetMeterZZError(meterTemp, result, TestValue);
        //            //        break;
        //            //    case ProjectID.日计时误差:
        //            //        GetClockError(meterTemp, result, TestValue);
        //            //        break;
        //            //    case ProjectID.身份认证:
        //            //    case ProjectID.密钥更新:
        //            //        GetMeterFK(meterTemp, result, TestValue);
        //            //        break;
        //            //    case ProjectID.GPS对时:
        //            //    case ProjectID.需量示值误差:
        //            //        GetDgn(meterTemp, result, TestValue);
        //            //        break;
        //            //    case ProjectID.误差一致性:
        //            //    case ProjectID.误差变差:
        //            //    case ProjectID.负载电流升将变差:
        //            //        GetMeterErrAccord(meterTemp, result, TestValue);
        //            //        break;
        //            //    case ProjectID.通讯协议检查试验:
        //            //        GetMeterDLTData(meterTemp, result, TestValue);
        //            //        break;
        //            //    case ProjectID.通讯协议检查试验2:
        //            //        GetMeterDLTData2(meterTemp, result, TestValue);
        //            //        break;
        //            //    case ProjectID.远程控制:
        //            //    case ProjectID.报警功能:
        //            //    case ProjectID.远程保电:
        //            //    case ProjectID.保电解除:
        //            //        GetMeterFK(meterTemp, result, TestValue);
        //            //        break;
        //            //    default:
        //            //        break;
        //            //}

        //        }
        //    }
        //    return meterTemp;

        //}





        //#region 淘汰

        //#endregion

        ///// <summary>
        ///// 排除没有读取到null的情况
        ///// </summary>
        ///// <param name="model"></param>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //private static string GetValue(DynamicViewModel model, string name)
        //{
        //    object obj = model.GetProperty(name);
        //    if (obj == null)
        //    {
        //        return "";
        //    }
        //    return obj.ToString();
        //}


        //#region 检定项目结论
        ///// <summary>
        ///// 获得总结论
        ///// </summary>
        //private static void GetResoult(TestMeterInfo meter)
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
        //        if (meter.MeterResults.ContainsKey(strVaule))
        //            meter.MeterResults.Remove(strVaule);
        //        MeterResult meterResult = new MeterResult();
        //        meterResult.Result = Core.Helper.Const.合格;
        //        meter.MeterResults.Add(strVaule, meterResult);
        //    }
        //}


        ///// <summary>
        ///// 获得基本误差结论
        ///// </summary>
        ///// <param name="meter">表类</param>
        ///// <param name="CheckResult">检定结论</param>
        ///// <param name="TestValue">检定参数值</param>
        //private static void GetMeterError(TestMeterInfo meter, DynamicViewModel CheckResult, Dictionary<string, string> TestValue)
        //{

        //    List<string> ModelName = CheckResult.GetAllProperyName();

        //    MeterError meterError = new MeterError();
        //    Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //    for (int j = 0; j < ModelName.Count; j++)
        //    {
        //        if (!ResultValue.ContainsKey(ModelName[j]))
        //        {
        //            string str = GetValue(CheckResult, ModelName[j]);
        //            ResultValue.Add(ModelName[j], str);
        //        }
        //    }
        //    string ItemKey = ResultValue["项目号"];
        //    string Para = ItemKey.Split('_')[0];

        //    meterError.GLYS = ResultValue["功率因素"];
        //    meterError.GLFX = ResultValue["功率方向"];
        //    //meterError.PrjNo = CheckResult.ItemKey;
        //    meterError.YJ = ResultValue["功率元件"];
        //    meterError.Result = ResultValue["结论"];
        //    //string[] results = result.Split('|');
        //    //meterError.Result = results[0];
        //    //meterError.AVR_DIS_REASON = results[1];
        //    meterError.IbX = ResultValue["电流倍数"];
        //    if (meterError.IbX == "Ib")
        //    {
        //        meterError.IbX = "1.0Ib";
        //    }
        //    meterError.WCData = ResultValue["误差1"] + "|" + ResultValue["误差2"];
        //    meterError.WCHZ = ResultValue["化整值"];
        //    meterError.WCValue = ResultValue["平均值"];
        //    meterError.Limit = ResultValue["误差上限"] + "|" + ResultValue["误差下限"];
        //    meterError.BPHUpLimit = ResultValue["误差上限"];
        //    meterError.BPHDownLimit = ResultValue["误差下限"];
        //    meterError.Circle = ResultValue["误差圈数"];

        //    if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //    {
        //        meterError.Result = "不合格";
        //    }
        //    if (meter.MeterErrors.ContainsKey(ItemKey))
        //        meter.MeterErrors.Remove(ItemKey);
        //    meter.MeterErrors.Add(ItemKey, meterError);

        //    ////如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //    if (meterError.Result == "不合格" && meter.MeterResults[Para].Result == "合格")
        //    {
        //        meter.MeterResults[Para].Result = Core.Helper.Const.不合格;
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
        //private static void GetMeterZZError(TestMeterInfo meter, DynamicViewModel CheckResult, Dictionary<string, string> TestValue)
        //{
        //    List<string> ModelName = CheckResult.GetAllProperyName();

        //    MeterZZError meterError = new MeterZZError();
        //    Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //    for (int j = 0; j < ModelName.Count; j++)
        //    {
        //        if (!ResultValue.ContainsKey(ModelName[j]))
        //        {
        //            string str = GetValue(CheckResult, ModelName[j]);
        //            ResultValue.Add(ModelName[j], str);
        //        }
        //    }
        //    string ItemKey = ResultValue["项目号"];
        //    string Para = ItemKey.Split('_')[0];
        //    meterError.Result = ResultValue["结论"];
        //    meterError.Fl = TestValue["费率"];
        //    meterError.GLYS = TestValue["功率因素"];
        //    meterError.IbX = TestValue["电流倍数"];
        //    if (meterError.IbX == "Ib")
        //    {
        //        meterError.IbX = "1.0Ib";
        //    }
        //    meterError.PowerWay = TestValue["功率方向"];
        //    meterError.TestWay = TestValue["走字试验方法类型"];
        //    meterError.YJ = TestValue["功率元件"];
        //    meterError.NeedEnergy = TestValue["走字电量(度)"];
        //    meterError.PowerSumEnd = ResultValue["起码"];
        //    meterError.PowerSumStart = ResultValue["止码"];
        //    meterError.WarkPower = "0";
        //    if (meterError.PowerSumEnd != "" && meterError.PowerSumStart != "")
        //    {
        //        meterError.WarkPower = (float.Parse(meterError.PowerSumStart) - float.Parse(meterError.PowerSumEnd)).ToString("f5");
        //    }
        //    meterError.PowerError = ResultValue["表码差"];
        //    meterError.STMEnergy = ResultValue["标准表脉冲"];  // ResultValue["标准表脉冲"]/d_meterInfo.GetBcs()[0])
        //    meterError.STMEnergy = ResultValue["标准表脉冲"];  // ResultValue["标准表脉冲"]/d_meterInfo.GetBcs()[0])
        //    meterError.Pules = ResultValue["表脉冲"];
        //    if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //    {
        //        meterError.Result = "不合格";
        //    }
        //    if (meter.MeterZZErrors.ContainsKey(ItemKey))
        //        meter.MeterZZErrors.Remove(ItemKey);
        //    meter.MeterZZErrors.Add(ItemKey, meterError);
        //    ////如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //    ///
        //    if (meterError.Result == "不合格" && meter.MeterResults[Para].Result == "合格")
        //    {
        //        meter.MeterResults[Para].Result = Core.Helper.Const.不合格;
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
        //private static void GetMeterQdQid(TestMeterInfo meter, DynamicViewModel CheckResult, Dictionary<string, string> TestValue)
        //{
        //    List<string> ModelName = CheckResult.GetAllProperyName();

        //    MeterQdQid meterError = new MeterQdQid();
        //    Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //    for (int j = 0; j < ModelName.Count; j++)
        //    {
        //        if (!ResultValue.ContainsKey(ModelName[j]))
        //        {
        //            string str = GetValue(CheckResult, ModelName[j]);
        //            ResultValue.Add(ModelName[j], str);
        //        }
        //    }
        //    string ItemKey = ResultValue["项目号"];
        //    string Para = ItemKey.Split('_')[0];
        //    meterError.ActiveTime = ResultValue["实际运行时间"].Trim('分');
        //    meterError.PowerWay = ResultValue["功率方向"];
        //    meterError.Voltage = ResultValue["试验电压"];
        //    meterError.TimeEnd = ResultValue["结束时间"];
        //    meterError.TimeStart = ResultValue["开始时间"];
        //    meterError.TimeStart = ResultValue["开始时间"];
        //    meterError.StandartTime = ResultValue["标准试验时间"];
        //    meterError.Current = ResultValue["试验电流"];
        //    meterError.Result = ResultValue["结论"];
        //    if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //    {
        //        meterError.Result = "不合格";
        //    }
        //    if (meter.MeterQdQids.ContainsKey(ItemKey))
        //        meter.MeterQdQids.Remove(ItemKey);
        //    meter.MeterQdQids.Add(ItemKey, meterError);
        //    //如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //    if (meterError.Result == "不合格" && meter.MeterResults[Para].Result == "合格")
        //    {
        //        meter.MeterResults[Para].Result = Core.Helper.Const.不合格;
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
        //private static void GetClockError(TestMeterInfo meter, DynamicViewModel CheckResult, Dictionary<string, string> TestValue)
        //{
        //    List<string> ModelName = CheckResult.GetAllProperyName();
        //    MeterDgn meterError = new MeterDgn();
        //    Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //    for (int j = 0; j < ModelName.Count; j++)
        //    {
        //        if (!ResultValue.ContainsKey(ModelName[j]))
        //        {
        //            string str = GetValue(CheckResult, ModelName[j]);
        //            ResultValue.Add(ModelName[j], str);
        //        }
        //    }
        //    string ItemKey = ResultValue["项目号"];
        //    string Para = ItemKey.Split('_')[0];
        //    meterError.Result = ResultValue["结论"];


        //    List<string> list = new List<string>();
        //    foreach (string item in ResultValue.Keys)
        //    {
        //        if (item != "结论")
        //        {
        //            list.Add(item);
        //        }
        //    }
        //    foreach (string item in TestValue.Keys)
        //    {
        //        list.Add(item);
        //    }
        //    meterError.Value = string.Join("|", list);

        //    if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //    {
        //        meterError.Result = "不合格";
        //    }
        //    if (meter.MeterDgns.ContainsKey(ItemKey))
        //        meter.MeterDgns.Remove(ItemKey);
        //    meter.MeterDgns.Add(ItemKey, meterError);
        //    ////如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //    ///
        //    if (meterError.Result == "不合格" && meter.MeterResults[Para].Result == "合格")
        //    {
        //        meter.MeterResults[Para].Result = Core.Helper.Const.不合格;
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
        //private static void GetMeterFK(TestMeterInfo meter, DynamicViewModel CheckResult, Dictionary<string, string> TestValue)
        //{

        //    List<string> ModelName = CheckResult.GetAllProperyName();

        //    MeterFK meterError = new MeterFK();
        //    Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //    for (int j = 0; j < ModelName.Count; j++)
        //    {
        //        if (!ResultValue.ContainsKey(ModelName[j]))
        //        {
        //            string str = GetValue(CheckResult, ModelName[j]);
        //            ResultValue.Add(ModelName[j], str);
        //        }
        //    }
        //    string ItemKey = ResultValue["项目号"];
        //    string Para = ItemKey.Split('_')[0];
        //    List<string> list = new List<string>();
        //    foreach (string item in ResultValue.Keys)
        //    {
        //        if (item != "结论")
        //        {
        //            list.Add(item);
        //        }
        //    }
        //    //switch (ItemKey)
        //    //{
        //    //    case ProjectID.远程控制:
        //    //        meterError.Name = "远程控制";
        //    //        meterError.Data = string.Join("|", list);
        //    //        break;
        //    //    case ProjectID.报警功能:
        //    //        meterError.Name = "报警功能";
        //    //        meterError.Data = string.Join("|", list);
        //    //        break;
        //    //    case ProjectID.远程保电:
        //    //    case "保电解除":
        //    //    case "密钥更新":
        //    //        meterError.Name = ResultValue["当前项目"];
        //    //        meterError.Data = ResultValue["检定信息"];
        //    //        break;
        //    //    default:  //费控通用
        //    //        meterError.Name = ResultValue["当前项目"];
        //    //        meterError.Data = ResultValue["检定信息"];
        //    //        break;
        //    //}

        //    meterError.Result = ResultValue["结论"];
        //    if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //    {
        //        meterError.Result = "不合格";
        //    }
        //    if (meter.MeterCostControls.ContainsKey(ItemKey))
        //        meter.MeterCostControls.Remove(ItemKey);
        //    meter.MeterCostControls.Add(ItemKey, meterError);
        //    ////如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //    ///
        //    if (meterError.Result == "不合格" && meter.MeterResults[Para].Result == "合格")
        //    {
        //        meter.MeterResults[Para].Result = Core.Helper.Const.不合格;
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
        //private static void GetDgn(TestMeterInfo meter, DynamicViewModel CheckResult, Dictionary<string, string> TestValue)
        //{
        //    List<string> ModelName = CheckResult.GetAllProperyName();

        //    MeterDgn meterError = new MeterDgn();
        //    Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //    for (int j = 0; j < ModelName.Count; j++)
        //    {
        //        if (!ResultValue.ContainsKey(ModelName[j]))
        //        {
        //            string str = GetValue(CheckResult, ModelName[j]);
        //            ResultValue.Add(ModelName[j], str);
        //        }
        //    }
        //    string ItemKey = ResultValue["项目号"];
        //    string Para = ItemKey.Split('_')[0];
        //    meterError.Result = ResultValue["结论"];

        //    List<string> list = new List<string>();
        //    foreach (string item in ResultValue.Keys)
        //    {
        //        if (item != "结论")
        //        {
        //            list.Add(item);
        //        }
        //    }
        //    meterError.Value = string.Join("|", list);
        //    if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //    {
        //        meterError.Result = "不合格";
        //    }
        //    if (meter.MeterDgns.ContainsKey(ItemKey))
        //        meter.MeterDgns.Remove(ItemKey);
        //    meter.MeterDgns.Add(ItemKey, meterError);
        //    ////如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //    ///
        //    if (meterError.Result == "不合格" && meter.MeterResults[Para].Result == "合格")
        //    {
        //        meter.MeterResults[Para].Result = Core.Helper.Const.不合格;
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
        //private static void GetMeterErrAccord(TestMeterInfo meter, DynamicViewModel CheckResult, Dictionary<string, string> TestValue)
        //{
        //    List<string> ModelName = CheckResult.GetAllProperyName();
        //    Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //    for (int j = 0; j < ModelName.Count; j++)
        //    {
        //        if (!ResultValue.ContainsKey(ModelName[j]))
        //        {
        //            string str = GetValue(CheckResult, ModelName[j]);
        //            ResultValue.Add(ModelName[j], str);
        //        }
        //    }
        //    string ItemKey = ResultValue["项目号"];
        //    string Para = ItemKey.Split('_')[0];

        //    string index = "";
        //    //switch (ItemKey)
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

        //    MeterErrAccord meterError = new MeterErrAccord();
        //    if (meter.MeterErrAccords.ContainsKey(index))   //先判断是不是有了，有了在原来基础上添加
        //        meterError = meter.MeterErrAccords[index];
        //    else
        //        meter.MeterErrAccords.Add(index, meterError);

        //    MeterErrAccordBase meterErr = new MeterErrAccordBase();
        //    meterErr.Freq = meter.MD_Frequency.ToString();
        //    meterError.Result = ResultValue["结论"];
        //    if (index == "1")    //补上圈数和误差限
        //    {
        //        meterErr.Name = "误差一致性";
        //        //误差1|误差2|平均值|化整值|样品均值|差值
        //        meterErr.IbX = TestValue["电流"];  //电流
        //        if (TestValue["电流"] == "Ib")
        //        {
        //            meterErr.IbX = "1.0Ib";
        //        }
        //        meterErr.PF = TestValue["功率因素"];   //功率因素
        //        meterErr.PulseCount = ResultValue["检定圈数"];
        //        meterErr.Limit = ResultValue["误差上限"] + "|" + ResultValue["误差下限"];
        //        meterErr.Data1 = ResultValue["误差1"] + "|" + ResultValue["误差2"] + "|" + ResultValue["平均值"] + "|" + ResultValue["化整值"];
        //        meterErr.ErrAver = ResultValue["样品均值"];
        //        meterErr.Error = ResultValue["差值"];

        //        int count = 1;
        //        if (meter.MeterErrAccords.ContainsKey(index))
        //            count = meter.MeterErrAccords[index].PointList.Keys.Count + 1;
        //        meterError.PointList.Add(count.ToString(), meterErr);
        //    }
        //    else if (index == "2")    //1.0IB，
        //    {
        //        meterErr.Name = "误差变差";
        //        meterErr.PulseCount = ResultValue["检定圈数"];
        //        meterErr.Limit = ResultValue["误差上限"] + "|" + ResultValue["误差下限"];
        //        meterErr.IbX = "1.0Ib";  //电流
        //        meterErr.PF = TestValue["功率因素"];  //功率因素
        //                                          //第一次误差1|第一次误差2|第一次平均值|第一次化整值|第二次误差1|第二次误差2|第二次平均值|第二次化整值|变差值
        //        meterErr.Data1 = ResultValue["第一次误差1"] + "|" + ResultValue["第一次误差2"] + "|" + ResultValue["第一次平均值"] + "|" + ResultValue["第一次化整值"];
        //        meterErr.Data2 = ResultValue["第二次误差1"] + "|" + ResultValue["第二次误差2"] + "|" + ResultValue["第二次平均值"] + "|" + ResultValue["第二次化整值"];
        //        meterErr.Error = ResultValue["变差值"];
        //        int count = 1;
        //        if (meter.MeterErrAccords.ContainsKey(index))
        //            count = meter.MeterErrAccords[index].PointList.Keys.Count + 1;
        //        meterError.PointList.Add(count.ToString(), meterErr);
        //    }
        //    else if (index == "3")   //这个需要加个判断，判断子项是否合格
        //    {

        //        string[] str = new string[] { "01Ib", "Ib", "Imax" };
        //        for (int j = 0; j < 3; j++)
        //        {
        //            meterErr = new MeterErrAccordBase();
        //            meterErr.Freq = meter.MD_Frequency.ToString();
        //            meterErr.Name = "负载电流升降变差";
        //            meterErr.PulseCount = ResultValue[str[j] + "检定圈数"];
        //            meterErr.IbX = str[j];  //电流
        //            if (meterErr.IbX == "Ib")
        //            {
        //                meterErr.IbX = "1.0Ib";
        //            }
        //            if (str[j] == "01Ib") meterErr.IbX = "0.1Ib";
        //            meterErr.PF = "1.0";   //功率因素
        //            meterErr.Data1 = ResultValue[str[j] + "上升误差1"] + "|" + ResultValue[str[j] + "上升误差2"] + "|" + ResultValue[str[j] + "上升平均值"] + "|" + ResultValue[str[j] + "上升化整值"];
        //            meterErr.Data2 = ResultValue[str[j] + "下降误差1"] + "|" + ResultValue[str[j] + "下降误差2"] + "|" + ResultValue[str[j] + "下降平均值"] + "|" + ResultValue[str[j] + "下降化整值"];
        //            meterErr.Error = ResultValue[str[j] + "差值"];
        //            meterErr.Result = Core.Helper.Const.不合格;
        //            if (!string.IsNullOrEmpty(meterErr.Error))
        //            {
        //                float t = float.Parse(meterErr.Error);
        //                if (Math.Abs(t) <= 0.25)
        //                    meterErr.Result = Core.Helper.Const.合格;
        //            }
        //            meterError.PointList.Add((j + 1).ToString(), meterErr);
        //        }



        //        //    for (int i = 0; i < CheckResult.CheckResults.Count; i++) //循环所有的表，获得这个表的检定结论
        //        //{

        //        //    meterErr.Freq = meter[i].MD_Frequency.ToString();
        //        //    meterError.Result = ResultValue["结论"];
        //        //    }
        //        //    meterError.Result = Core.Helper.Const.合格;
        //        //    for (int j = 1; j <= meterError.PointList.Count; j++)
        //        //    {
        //        //        if (meterError.PointList[j.ToString()].Result == Core.Helper.Const.不合格)     //有一个点不合格，总结论不合格
        //        //        {
        //        //            meterError.Result = Core.Helper.Const.不合格;
        //        //        }
        //        //    }
        //        //    if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //        //    {
        //        //        meterError.Result = "不合格";
        //        //    }
        //        //    //如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //        //    if (meterError.Result == "不合格" && meter[i].MeterResults[CheckResult.ParaNo].Result == "合格")
        //        //    {
        //        //        meter[i].MeterResults[CheckResult.ParaNo].Result = Core.Helper.Const.不合格;
        //        //    }
        //    }
        //    meterError.Result = Core.Helper.Const.合格;
        //    for (int j = 1; j <= meterError.PointList.Count; j++)
        //    {
        //        if (meterError.PointList[j.ToString()].Result == Core.Helper.Const.不合格)     //有一个点不合格，总结论不合格
        //        {
        //            meterError.Result = Core.Helper.Const.不合格;
        //        }
        //    }
        //    if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //    {
        //        meterError.Result = "不合格";
        //    }
        //    //如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //    if (meterError.Result == "不合格" && meter.MeterResults[Para].Result == "合格")
        //    {
        //        meter.MeterResults[Para].Result = Core.Helper.Const.不合格;
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
        //private static void GetMeterDLTData(TestMeterInfo meter, DynamicViewModel CheckResult, Dictionary<string, string> TestValue)
        //{
        //    List<string> ModelName = CheckResult.GetAllProperyName();
        //    MeterDLTData meterDLTData = new MeterDLTData();
        //    Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //    for (int j = 0; j < ModelName.Count; j++)
        //    {
        //        if (!ResultValue.ContainsKey(ModelName[j]))
        //        {
        //            string str = GetValue(CheckResult, ModelName[j]);
        //            ResultValue.Add(ModelName[j], str);
        //        }
        //    }
        //    string ItemKey = ResultValue["项目号"];
        //    string Para = ItemKey.Split('_')[0];

        //    List<string> list = new List<string>();
        //    meterDLTData.DataFlag = TestValue["标识编码"]; //645
        //    meterDLTData.DataFormat = TestValue["数据格式"];
        //    meterDLTData.DataLen = TestValue["长度"];
        //    meterDLTData.FlagMsg = TestValue["数据项名称"];
        //    meterDLTData.StandardValue = TestValue["写入内容"];
        //    meterDLTData.Value = ResultValue["检定信息"];
        //    meterDLTData.Result = ResultValue["结论"];

        //    if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //    {
        //        meterDLTData.Result = "不合格";
        //    }
        //    if (meter.MeterDLTDatas.ContainsKey(ItemKey))
        //        meter.MeterDLTDatas.Remove(ItemKey);
        //    meter.MeterDLTDatas.Add(ItemKey, meterDLTData);
        //    //如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //    if (meterDLTData.Result == "不合格" && meter.MeterResults[Para].Result == "合格")
        //    {
        //        meter.MeterResults[Para].Result = Core.Helper.Const.不合格;
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
        //private static void GetMeterDLTData2(TestMeterInfo meter, DynamicViewModel CheckResult, Dictionary<string, string> TestValue)
        //{
        //    List<string> ModelName = CheckResult.GetAllProperyName();

        //    MeterDLTData meterDLTData = new MeterDLTData();
        //    Dictionary<string, string> ResultValue = new Dictionary<string, string>();
        //    for (int j = 0; j < ModelName.Count; j++)
        //    {
        //        if (!ResultValue.ContainsKey(ModelName[j]))
        //        {
        //            string str = GetValue(CheckResult, ModelName[j]);
        //            ResultValue.Add(ModelName[j], str);
        //        }
        //    }
        //    string ItemKey = ResultValue["项目号"];
        //    string Para = ItemKey.Split('_')[0];

        //    List<string> list = new List<string>();
        //    foreach (string item in ResultValue.Keys)
        //    {
        //        if (item != "结论")
        //        {
        //            list.Add(item);
        //        }
        //    }
        //    //if (meter.MD_ProtocolName.IndexOf("645") != -1)
        //    //{
        //    //    meterDLTData.DataFlag = TestValue["标识编码"]; //645
        //    //}
        //    //else
        //    //{
        //    //    meterDLTData.DataFlag = TestValue["标识编码698"]; //698
        //    //}
        //    meterDLTData.DataFormat = TestValue["数据格式"];
        //    meterDLTData.DataLen = TestValue["长度"];
        //    meterDLTData.FlagMsg = TestValue["数据项名称"];
        //    meterDLTData.StandardValue = TestValue["写入内容"];

        //    meterDLTData.Value = ResultValue["检定信息"];
        //    meterDLTData.Result = ResultValue["结论"];

        //    if (ResultValue["结论"] == null || ResultValue["结论"].Trim() == "") //没有结论的就跳过
        //    {
        //        meterDLTData.Result = "不合格";
        //    }
        //    if (meter.MeterDLTDatas.ContainsKey(ItemKey))
        //        meter.MeterDLTDatas.Remove(ItemKey);
        //    meter.MeterDLTDatas.Add(ItemKey, meterDLTData);
        //    //如果当前项目不合格，并且总结论是合格，那么就把总结论修改成不合格(总结论指大项目结论)
        //    if (meterDLTData.Result == "不合格" && meter.MeterResults[Para].Result == "合格")
        //    {
        //        meter.MeterResults[Para].Result = Core.Helper.Const.不合格;
        //    }
        //}

        //#endregion
        #endregion

    }
}
