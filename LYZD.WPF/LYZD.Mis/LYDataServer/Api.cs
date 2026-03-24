using LYZD.Core.Enum;
using LYZD.Core.Model.DnbModel.DnbInfo;
using LYZD.Core.Model.Meter;
using LYZD.Core.Model.Schema;
using LYZD.Mis.Common;
using LYZD.Mis.NanRui.LRDataTable;
using LYZD.Utility.Log;
using ServiceInterface.DataFormat;
using ServiceInterface.DataFormat.MeterFormat;
using ServiceInterface.DataFormat.SchemeFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LYZD.Mis.LYDataServer
{
    public class Api : IMis
    {
        private readonly ServiceInterface.DataService.Services Service;
        private bool Allow = false;
        private string User;
        private string PassWord;


        public Api(string ip, int port, string dataSource, string userId, string pwd, string url)
        {
            Service = new ServiceInterface.DataService.Services(ip, port.ToString());
            PassWord = pwd;
            User = userId;
            InitItemNo();
            //
            //
            //
            //
        }

        public bool Down(string stationId, ref TestMeterInfo[] meter, out string msg)
        {
            msg = "";
            //if (!Allow)
            //{
            //    msg = "登录数据服务失败！";
            //    return false;
            //}
            if (string.IsNullOrWhiteSpace(stationId))
            {
                msg = "工位编号为空！";
                return false;
            }
            if (stationId.Length < 7)//2105040
            {
                msg = "工位编号长度不足7！";
                return false;
            }
            if (stationId[6] != '0' && stationId[6] != '1'
                && stationId[6] != '2' && stationId[6] != '3')//2105040
            {
                msg = "工位编号格式不匹配！";
                return false;
            }
            try
            {
                ApiResult<Dictionary<string, MeterInfo[]>> r = Service.Meter.GetMeterInfo(stationId);
                if (r.ResultFlag)
                {
                    //这里就是下载到的表信息，是一个字典集合，键是托盘编号,值是这个托盘内的表信息，表信息顺序是信息绑定工位所绑定的顺序
                    //如果流水线有分正转和反转的情况下载的时候自己计算表位号。。如果托盘该位置没有表,则数组那个值为null
                    Dictionary<string, MeterInfo[]> meters = r.Data;
                    if (meters == null)
                    {
                        msg = "下载表信息失败，Data集合NULL";
                        return false;
                    }
                    List<TestMeterInfo> meterss = new List<TestMeterInfo>();
                    foreach (KeyValuePair<string, MeterInfo[]> items in meters)
                    {
                        if (items.Value == null)
                        {
                            msg = "下载表信息失败，托盘集合NULL";
                            return false;
                        }
                        foreach (MeterInfo item in items.Value)
                        {

                            if (item == null)
                            {
                                meterss.Add(null);
                                continue;
                            }
                            TestMeterInfo cm = new TestMeterInfo();
                            cm.MD_CertificateNo = item.MD_CERTIFICATE_NO;
                            if (item.MD_FKTYPE == "远程费控") cm.FKType = 0;
                            else if (item.MD_FKTYPE == "本地费控") cm.FKType = 1;
                            else cm.FKType = 2;
                            cm.MD_Epitope = item.MD_EPITOPE;
                            //cm. = item.MD_SCHEME_ID;//与客户端id无关
                            //cm. = item.MD_TEST_DATE;
                            //cm. = item.MD_VALID_DATE;
                            cm.Temperature = item.MD_TEMPERATURE;
                            cm.Humidity = item.MD_HUMIDITY;
                            //cm. = item.MD_SUPERVISOR;
                            //cm. = item.MD_AUDIT_PERSON;
                            //cm. = item.MD_TEST_PERSON;
                            cm.Seal1 = item.MD_SEAL_1;
                            cm.Seal2 = item.MD_SEAL_2;
                            cm.Seal3 = item.MD_SEAL_3;
                            cm.Seal4 = item.MD_SEAL_4;
                            cm.Seal5 = item.MD_SEAL_5;
                            cm.Other1 = item.MD_OTHER_1;
                            cm.Other2 = item.MD_OTHER_2;
                            cm.Other3 = item.MD_OTHER_3;
                            cm.Other4 = item.MD_OTHER_4;
                            cm.Other5 = item.MD_OTHER_5;
                            //cm. = item.HANDLE_FLAG;
                            cm.MD_MadeNo = item.MD_MADE_NO;
                            //cm. = item.MD_METER_VERSION;//?
                            cm.MD_TaskNo = item.MD_TASK_NO;
                            cm.MD_Factory = item.MD_FACTORY;
                            cm.MD_BarCode = item.MD_BAR_CODE;
                            cm.Meter_ID = item.METER_ID;
                            //cm. = item.MD_DeviceNo;
                            cm.Result = item.MD_RESULT;
                            //cm. = item.MD_CHECKED;
                            cm.MD_UB = int.Parse(item.MD_UB.Trim('V', 'v'));
                            cm.MD_UA = item.MD_UA;
                            cm.MD_Frequency = int.Parse(item.MD_FREQUENCY.ToUpper().Trim('H', 'Z'));
                            if (item.MD_TESTMODEL != null && item.MD_TESTMODEL.IndexOf("周检") >= 0)
                                cm.MD_TestModel = "周检";
                            else
                                cm.MD_TestModel = "首检";

                            if (item.MD_TEST_TYPE != null && item.MD_TEST_TYPE.IndexOf("全检") >= 0)
                                cm.MD_TestType = "全检";
                            else
                                cm.MD_TestType = "抽检";
                            cm.MD_WiringMode = item.MD_WIRING_MODE;
                            cm.MD_ConnectionFlag = item.MD_CONNECTION_FLAG;
                            cm.MD_JJGC = item.MD_JJGC;
                            cm.MD_AssetNo = item.MD_ASSET_NO;
                            if (item.MD_METER_TYPE.Contains("集中器"))
                            {
                                cm.MD_TerminalType = "集中器I型22版";
                            }
                            else if (item.MD_METER_TYPE.Contains("专变"))
                            {
                                cm.MD_TerminalType = "专变III型22版";
                            }
                            else if (item.MD_METER_TYPE.Contains("ECU") || item.MD_METER_TYPE.Contains("融合"))
                            {
                                cm.MD_TerminalType = "融合终端";
                            }
                            else if (item.MD_METER_TYPE.Contains("能源") || item.MD_METER_TYPE.Contains("SCU"))
                            {
                                cm.MD_TerminalType = "能源控制器";
                            }
                            cm.MD_Constant = item.MD_CONSTANT;
                            cm.MD_Grane = item.MD_GRADE;
                            cm.MD_TerminalModel = item.MD_METER_MODEL;
                            cm.MD_Protocol_Type = item.MD_PROTOCOL_NAME;

                            cm.Address = item.MD_POSTAL_ADDRESS;
                            cm.MD_Customer = item.MD_CUSTOMER;
                            //cm. = item.MD_ERROR_DATA;
                            meterss.Add(cm);

                        }
                    }
                    meter = meterss.ToArray();
                    return true;
                }
                else
                {
                    msg = "下载表信息失败，" + r.ErrorMessage;
                    return false;
                }
                //if (r.ResultFlag)
                //{
                //    //这里就是下载到的表信息，是一个字典集合，键是托盘编号,值是这个托盘内的表信息，表信息顺序是信息绑定工位所绑定的顺序
                //    //如果流水线有分正转和反转的情况下载的时候自己计算表位号。。如果托盘该位置没有表,则数组那个值为null
                //    Dictionary<string, MeterInfo[]> meters = r.Data;
                //    if (meters == null)
                //    {
                //        msg = "下载表信息失败，Data集合NULL";
                //        return false;
                //    }
                //    int posSeq = (stationId[6] == '0' || stationId[6] == '2') ? 0 : 1;//0: tray1-pos1 或 1: 4-1
                //    int trayno = 0;//序号1-n
                //    int trayPosCount = meters.First().Value.Length; //6;
                //    int trayCol = (stationId[6] == '0' || stationId[6] == '1') ? 2 : 1;//2;
                //    int trayCount = meters.Count();
                //    int totalCount = trayCount * trayPosCount;

                //    bool rotate = false;
                //    int Seq = posSeq;
                //    if (trayCol == 1 && posSeq == 1)
                //    {
                //        posSeq = 0;
                //        rotate = true;
                //    }

                //    bool transformer = false;
                //    bool find = false;
                //    foreach (KeyValuePair<string, MeterInfo[]> items in meters)
                //    {
                //        foreach (MeterInfo item in items.Value)
                //        {
                //            if (item == null) continue;
                //            transformer = item.MD_CONNECTION_FLAG == "互感式";
                //            find = true;
                //            break;
                //        }
                //        if (find) break;
                //    }

                //    if (transformer && rotate)
                //    {
                //        //不处理
                //    }
                //    else if (transformer || rotate)
                //    {
                //        foreach (var item in meters)
                //        {
                //            for (int i = 0; i < trayPosCount / 2; i++)
                //            {
                //                var temp = item.Value[i];
                //                item.Value[i] = item.Value[trayPosCount - i - 1];
                //                item.Value[trayPosCount - i - 1] = temp;
                //            }
                //        }
                //    }

                //    foreach (KeyValuePair<string, MeterInfo[]> items in meters)
                //    {
                //        if (items.Value == null)
                //        {
                //            msg = "下载表信息失败，托盘集合NULL";
                //            return false;
                //        }
                //        ++trayno;
                //        int trayPos = 0;
                //        foreach (MeterInfo item in items.Value)
                //        {
                //            ++trayPos;
                //            int pos = (trayno - 1) * trayPosCount / trayCol
                //            + trayPos
                //            + posSeq * (-1 * (trayPos / (trayPosCount / trayCol + 1)) - (trayPos / (trayPosCount / trayCol + 1)) + 1) * (trayPosCount / trayCol)
                //            + (trayPos / (trayPosCount / trayCol + 1)
                //               + posSeq * (-1 * (trayPos / (trayPosCount / trayCol + 1)) - (trayPos / (trayPosCount / trayCol + 1)) + 1)
                //            ) * (trayCount - trayno) * trayPosCount
                //            - 1;
                //            if (item == null)
                //            {
                //                meter[pos] = null;
                //                continue;
                //            }
                //            meter[pos] = new TestMeterInfo();
                //            TestMeterInfo cm = meter[pos];
                //            cm.MD_CertificateNo = item.MD_CERTIFICATE_NO;
                //            if (item.MD_FKTYPE == "远程费控") cm.FKType = 0;
                //            else if (item.MD_FKTYPE == "本地费控") cm.FKType = 1;
                //            else cm.FKType = 2;
                //            //cm. = item.MD_SCHEME_ID;//与客户端id无关
                //            //cm. = item.MD_TEST_DATE;
                //            //cm. = item.MD_VALID_DATE;
                //            cm.Temperature = item.MD_TEMPERATURE;
                //            cm.Humidity = item.MD_HUMIDITY;
                //            //cm. = item.MD_SUPERVISOR;
                //            //cm. = item.MD_AUDIT_PERSON;
                //            //cm. = item.MD_TEST_PERSON;
                //            cm.Seal1 = item.MD_SEAL_1;
                //            cm.Seal2 = item.MD_SEAL_2;
                //            cm.Seal3 = item.MD_SEAL_3;
                //            cm.Seal4 = item.MD_SEAL_4;
                //            cm.Seal5 = item.MD_SEAL_5;
                //            cm.Other1 = item.MD_OTHER_1;
                //            cm.Other2 = item.MD_OTHER_2;
                //            cm.Other3 = item.MD_OTHER_3;
                //            cm.Other4 = item.MD_OTHER_4;
                //            cm.Other5 = item.MD_OTHER_5;
                //            //cm. = item.HANDLE_FLAG;
                //            cm.MD_MadeNo = item.MD_MADE_NO;
                //            //cm. = item.MD_METER_VERSION;//?
                //            cm.MD_TaskNo = item.MD_TASK_NO;
                //            cm.MD_Factory = item.MD_FACTORY;
                //            cm.MD_BarCode = item.MD_BAR_CODE;
                //            cm.Meter_ID = item.METER_ID;
                //            //cm. = item.MD_DeviceNo;
                //            cm.Result = item.MD_RESULT;
                //            //cm. = item.MD_CHECKED;
                //            cm.MD_UB = int.Parse(item.MD_UB.Trim('V', 'v'));
                //            cm.MD_UA = item.MD_UA;
                //            cm.MD_Frequency = int.Parse(item.MD_FREQUENCY.ToUpper().Trim('H', 'Z'));
                //            if (item.MD_TESTMODEL != null && item.MD_TESTMODEL.IndexOf("周检") >= 0)
                //                cm.MD_TestModel = "周检";
                //            else
                //                cm.MD_TestModel = "首检";

                //            if (item.MD_TEST_TYPE != null && item.MD_TEST_TYPE.IndexOf("全检") >= 0)
                //                cm.MD_TestType = "全检";
                //            else
                //                cm.MD_TestType = "抽检";
                //            cm.MD_WiringMode = item.MD_WIRING_MODE;
                //            cm.MD_ConnectionFlag = item.MD_CONNECTION_FLAG;
                //            cm.MD_JJGC = item.MD_JJGC;
                //            cm.MD_AssetNo = item.MD_ASSET_NO;
                //            cm.MD_TerminalType = item.MD_METER_TYPE;
                //            cm.MD_Constant = item.MD_CONSTANT;
                //            cm.MD_Grane = item.MD_GRADE;
                //            cm.MD_TerminalModel = item.MD_METER_MODEL;
                //            cm.MD_Protocol_Type = item.MD_PROTOCOL_NAME;

                //            cm.Address = item.MD_POSTAL_ADDRESS;
                //            cm.MD_Customer = item.MD_CUSTOMER;
                //            //cm. = item.MD_ERROR_DATA;


                //        }
                //    }

                //    return true;
                //}
                //else
                //{
                //    msg = "下载表信息失败，" + r.ErrorMessage;
                //    return false;
                //}
            }
            catch (Exception ex)
            {
                msg = "下载表信息异常，" + ex.Message;
                return false;
            }
        }
        public bool Down(string barcode, ref TestMeterInfo meter)
        {
            throw new NotImplementedException();
        }

        public bool DownTask(string MD_BarCode, ref MT_DETECT_OUT_EQUIP mT_DETECT_TASK)
        {
            throw new NotImplementedException();
        }

        public bool SchemeDown(string barcode, out string schemeName, out Dictionary<string, SchemaNode> Schema)
        {
            throw new NotImplementedException();
        }

        public bool SchemeDown(TestMeterInfo meter, out string schemeName, out Dictionary<string, SchemaNode> Schema, out string msg)
        {
            schemeName = "";
            Schema = new Dictionary<string, SchemaNode>();
            msg = "";
            //if (!Allow)
            //{
            //    msg = "登录数据服务失败！";
            //    return false;
            //}
            try
            {
                ApiResult<SchemaInfo> result = Service.Meter.GetSchemeInfo(meter.MD_TaskNo);
                if (result.ResultFlag)
                {
                    //这里就是下载到的方案了
                    SchemaInfo dschema = result.Data;
                    //方案内有项目节点列表，目前初稿，以检表软件为主，所以项目编号和检表项目编号保持一致
                    // 节点的TestValue是用|分隔的检定参数，检定参数和检定软件保持一直，所以检定软件可直接赋值使用
                    schemeName = dschema.SchemaName;
                    SchemaNode snd;
                    foreach (SchemaInfo.SchemaNode item in dschema.SchemaNodes)
                    {
                        var Item = ItemNoList.FirstOrDefault(x => x.ItemNo == item.ParaNo);
                        if (Item != null)
                        {
                            if (!Schema.ContainsKey(Item.ProjectID)) Schema.Add(Item.ProjectID, new SchemaNode());
                            snd = Schema[Item.ProjectID];
                            snd.SchemaNodeValue.Add(item.TestValue);
                            continue;
                        }
                        switch (item.ParaNo)
                        {
                            #region //计量
                            case "12001":
                                if (!Schema.ContainsKey(ProjectID.基本误差试验)) Schema.Add(ProjectID.基本误差试验, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue);
                                break;
                            case "12002":
                                if (!Schema.ContainsKey(ProjectID.起动试验)) Schema.Add(ProjectID.起动试验, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue);
                                break;
                            case "12003":
                                if (!Schema.ContainsKey(ProjectID.潜动试验)) Schema.Add(ProjectID.潜动试验, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue);
                                break;
                            case "15003":
                                if (!Schema.ContainsKey(ProjectID.需量示值误差)) Schema.Add(ProjectID.需量示值误差, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue);
                                break;
                            case "15013":
                                if (!Schema.ContainsKey(ProjectID.电能示值组合误差)) Schema.Add(ProjectID.电能示值组合误差, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue);
                                break;
                            case "15002":
                                if (!Schema.ContainsKey(ProjectID.日计时误差)) Schema.Add(ProjectID.日计时误差, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue);
                                break;
                            #endregion

                            #region 参数设置与查询
                            case "51001":
                                if (!Schema.ContainsKey(ProjectID.终端逻辑地址查询)) Schema.Add(ProjectID.终端逻辑地址查询, new SchemaNode());
                                snd = Schema[ProjectID.终端逻辑地址查询];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "51002":
                                if (!Schema.ContainsKey(ProjectID.终端密钥恢复)) Schema.Add(ProjectID.终端密钥恢复, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "51003":
                                if (!Schema.ContainsKey(ProjectID.时钟召测和对时)) Schema.Add(ProjectID.时钟召测和对时, new SchemaNode());
                                snd = Schema[ProjectID.时钟召测和对时];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "51004":
                                if (!Schema.ContainsKey(ProjectID.基本参数)) Schema.Add(ProjectID.基本参数, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "51005":
                                if (!Schema.ContainsKey(ProjectID.抄表与费率参数)) Schema.Add(ProjectID.抄表与费率参数, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "51006":
                                if (!Schema.ContainsKey(ProjectID.限值与阈值参数)) Schema.Add(ProjectID.限值与阈值参数, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "51007":
                                if (!Schema.ContainsKey(ProjectID.控制参数)) Schema.Add(ProjectID.控制参数, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "51008":
                                if (!Schema.ContainsKey(ProjectID.其他参数)) Schema.Add(ProjectID.其他参数, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "51009":
                                if (!Schema.ContainsKey(ProjectID.读取终端信息)) Schema.Add(ProjectID.读取终端信息, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "51010":
                                if (!Schema.ContainsKey(ProjectID.事件参数)) Schema.Add(ProjectID.事件参数, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "51011":
                                if (!Schema.ContainsKey(ProjectID.单个测量电设置采集档案)) Schema.Add(ProjectID.单个测量电设置采集档案, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "51012":
                                if (!Schema.ContainsKey(ProjectID.组合参数读取与参数设置)) Schema.Add(ProjectID.组合参数读取与参数设置, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "51013":
                                if (!Schema.ContainsKey(ProjectID.精准校时)) Schema.Add(ProjectID.精准校时, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            #endregion

                            #region 数据采集
                            case "52001":
                                if (!Schema.ContainsKey(ProjectID.状态量采集)) Schema.Add(ProjectID.状态量采集, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "52002":
                                if (!Schema.ContainsKey(ProjectID.电能表数据采集)) Schema.Add(ProjectID.电能表数据采集, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "52003":
                                if (!Schema.ContainsKey(ProjectID.分脉冲量采集12个)) Schema.Add(ProjectID.分脉冲量采集12个, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "52004":
                                if (!Schema.ContainsKey(ProjectID.分脉冲量采集120个)) Schema.Add(ProjectID.分脉冲量采集120个, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "52005":
                                if (!Schema.ContainsKey(ProjectID.总加组日和月电量召集)) Schema.Add(ProjectID.总加组日和月电量召集, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "52006":
                                if (!Schema.ContainsKey(ProjectID.分时段电能量数据存储)) Schema.Add(ProjectID.分时段电能量数据存储, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "52007":
                                if (!Schema.ContainsKey(ProjectID.电表日历与状态召集)) Schema.Add(ProjectID.电表日历与状态召集, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "52008":
                                if (!Schema.ContainsKey(ProjectID.电能表实时数据)) Schema.Add(ProjectID.电能表实时数据, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "52009":
                                if (!Schema.ContainsKey(ProjectID.电能表当前数据)) Schema.Add(ProjectID.电能表当前数据, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "52010":
                                if (!Schema.ContainsKey(ProjectID.电能表当前数据2路)) Schema.Add(ProjectID.电能表当前数据2路, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "52011":
                                if (!Schema.ContainsKey(ProjectID.电能表当前数据错误MAC)) Schema.Add(ProjectID.电能表当前数据错误MAC, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "52012":
                                if (!Schema.ContainsKey(ProjectID.终端采集645表计数据)) Schema.Add(ProjectID.终端采集645表计数据, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "52013":
                                if (!Schema.ContainsKey(ProjectID.总加组电量数据)) Schema.Add(ProjectID.总加组电量数据, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "52014":
                                if (!Schema.ContainsKey(ProjectID.终端主动上报)) Schema.Add(ProjectID.终端主动上报, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            #endregion

                            #region 数据处理
                            case "53001":
                                if (!Schema.ContainsKey(ProjectID.实时和当前数据)) Schema.Add(ProjectID.实时和当前数据, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "53002":
                                if (!Schema.ContainsKey(ProjectID.历史日数据)) Schema.Add(ProjectID.历史日数据, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "53003":
                                if (!Schema.ContainsKey(ProjectID.负荷曲线)) Schema.Add(ProjectID.负荷曲线, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "53004":
                                if (!Schema.ContainsKey(ProjectID.历史月数据)) Schema.Add(ProjectID.历史月数据, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "53005":
                                if (!Schema.ContainsKey(ProjectID.电压合格率统计)) Schema.Add(ProjectID.电压合格率统计, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "53006":
                                if (!Schema.ContainsKey(ProjectID.历史日数据补抄)) Schema.Add(ProjectID.历史日数据补抄, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "53007":
                                if (!Schema.ContainsKey(ProjectID.负荷曲线补抄)) Schema.Add(ProjectID.负荷曲线补抄, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "53008":
                                if (!Schema.ContainsKey(ProjectID.随机交采数据)) Schema.Add(ProjectID.随机交采数据, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "53009":
                                if (!Schema.ContainsKey(ProjectID.电能表事件补抄)) Schema.Add(ProjectID.电能表事件补抄, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "53010":
                                if (!Schema.ContainsKey(ProjectID.抄表MAC验证)) Schema.Add(ProjectID.抄表MAC验证, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "53011":
                                if (!Schema.ContainsKey(ProjectID.分组抄表)) Schema.Add(ProjectID.分组抄表, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "53012":
                                if (!Schema.ContainsKey(ProjectID.结算日冻结)) Schema.Add(ProjectID.结算日冻结, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "53013":
                                if (!Schema.ContainsKey(ProjectID.透明方案)) Schema.Add(ProjectID.透明方案, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            #endregion

                            #region 控制试验

                            case "54001":
                                if (!Schema.ContainsKey(ProjectID.时段功控)) Schema.Add(ProjectID.时段功控, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54002":
                                if (!Schema.ContainsKey(ProjectID.厂休功控)) Schema.Add(ProjectID.厂休功控, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54003":
                                if (!Schema.ContainsKey(ProjectID.营业报停功控)) Schema.Add(ProjectID.营业报停功控, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54004":
                                if (!Schema.ContainsKey(ProjectID.当前功率下浮控)) Schema.Add(ProjectID.当前功率下浮控, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54005":
                                if (!Schema.ContainsKey(ProjectID.月电控)) Schema.Add(ProjectID.月电控, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54006":
                                if (!Schema.ContainsKey(ProjectID.购电控)) Schema.Add(ProjectID.购电控, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54007":
                                if (!Schema.ContainsKey(ProjectID.催费告警)) Schema.Add(ProjectID.催费告警, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54008":
                                if (!Schema.ContainsKey(ProjectID.保电功能)) Schema.Add(ProjectID.保电功能, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54009":
                                if (!Schema.ContainsKey(ProjectID.剔除功能)) Schema.Add(ProjectID.剔除功能, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54010":
                                if (!Schema.ContainsKey(ProjectID.遥控功能)) Schema.Add(ProjectID.遥控功能, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54011":
                                if (!Schema.ContainsKey(ProjectID.时段控与购电控同时投入)) Schema.Add(ProjectID.时段控与购电控同时投入, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;

                            #endregion

                            #region 事件记录
                            case "55001":
                                if (!Schema.ContainsKey(ProjectID.电能表超差事件)) Schema.Add(ProjectID.电能表超差事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "55002":
                                if (!Schema.ContainsKey(ProjectID.电能表飞走事件)) Schema.Add(ProjectID.电能表飞走事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "55003":
                                if (!Schema.ContainsKey(ProjectID.电能表停走事件)) Schema.Add(ProjectID.电能表停走事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "55004":
                                if (!Schema.ContainsKey(ProjectID.电能表时间超差事件)) Schema.Add(ProjectID.电能表时间超差事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "55005":
                                if (!Schema.ContainsKey(ProjectID.终端停_上电事件)) Schema.Add(ProjectID.终端停_上电事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "55006":
                                if (!Schema.ContainsKey(ProjectID.终端停_上电事件_带主动上报)) Schema.Add(ProjectID.终端停_上电事件_带主动上报, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "55007":
                                if (!Schema.ContainsKey(ProjectID.终端对时事件)) Schema.Add(ProjectID.终端对时事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "55008":
                                if (!Schema.ContainsKey(ProjectID.全事件采集上报)) Schema.Add(ProjectID.全事件采集上报, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "55009":
                                if (!Schema.ContainsKey(ProjectID.购电参数设置事件)) Schema.Add(ProjectID.购电参数设置事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "55010":
                                if (!Schema.ContainsKey(ProjectID.终端485抄表错误)) Schema.Add(ProjectID.终端485抄表错误, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "55011":
                                if (!Schema.ContainsKey(ProjectID.电压越限事件)) Schema.Add(ProjectID.电压越限事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "55012":
                                if (!Schema.ContainsKey(ProjectID.终端参数变更事件)) Schema.Add(ProjectID.终端参数变更事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54013":
                                if (!Schema.ContainsKey(ProjectID.电能表常数变更事件)) Schema.Add(ProjectID.电能表常数变更事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54014":
                                if (!Schema.ContainsKey(ProjectID.电能表时段变更事件)) Schema.Add(ProjectID.电能表时段变更事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54015":
                                if (!Schema.ContainsKey(ProjectID.电能表抄表日变更事件)) Schema.Add(ProjectID.电能表抄表日变更事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54016":
                                if (!Schema.ContainsKey(ProjectID.电能表电池欠压事件)) Schema.Add(ProjectID.电能表电池欠压事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54017":
                                if (!Schema.ContainsKey(ProjectID.电能表编程次数变更事件)) Schema.Add(ProjectID.电能表编程次数变更事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54018":
                                if (!Schema.ContainsKey(ProjectID.电能表最大需量清零次数变更事件)) Schema.Add(ProjectID.电能表最大需量清零次数变更事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54019":
                                if (!Schema.ContainsKey(ProjectID.电能表断相次数变更事件)) Schema.Add(ProjectID.电能表断相次数变更事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54020":
                                if (!Schema.ContainsKey(ProjectID.电能表示度下降事件)) Schema.Add(ProjectID.电能表示度下降事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54021":
                                if (!Schema.ContainsKey(ProjectID.电流反向事件)) Schema.Add(ProjectID.电流反向事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54022":
                                if (!Schema.ContainsKey(ProjectID.电压断相事件)) Schema.Add(ProjectID.电压断相事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54023":
                                if (!Schema.ContainsKey(ProjectID.电压失压事件)) Schema.Add(ProjectID.电压失压事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54024":
                                if (!Schema.ContainsKey(ProjectID.终端相序异常事件)) Schema.Add(ProjectID.终端相序异常事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54025":
                                if (!Schema.ContainsKey(ProjectID.电流越限事件)) Schema.Add(ProjectID.电流越限事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54026":
                                if (!Schema.ContainsKey(ProjectID.视在功率越限事件)) Schema.Add(ProjectID.视在功率越限事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54027":
                                if (!Schema.ContainsKey(ProjectID.电能表运行状态字变位事件)) Schema.Add(ProjectID.电能表运行状态字变位事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54028":
                                if (!Schema.ContainsKey(ProjectID.电能表开表盖事件)) Schema.Add(ProjectID.电能表开表盖事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54029":
                                if (!Schema.ContainsKey(ProjectID.电能表开端钮盒事件)) Schema.Add(ProjectID.电能表开端钮盒事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54030":
                                if (!Schema.ContainsKey(ProjectID.终端编程事件)) Schema.Add(ProjectID.终端编程事件, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54031":
                                if (!Schema.ContainsKey(ProjectID.终端抄表失败)) Schema.Add(ProjectID.终端抄表失败, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "54032":
                                if (!Schema.ContainsKey(ProjectID.电能表数据变更监控记录)) Schema.Add(ProjectID.电能表数据变更监控记录, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            #endregion

                            #region 测量准确度

                            case "57001":
                                if (!Schema.ContainsKey(ProjectID.电压基本误差)) Schema.Add(ProjectID.电压基本误差, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "57002":
                                if (!Schema.ContainsKey(ProjectID.电流基本误差)) Schema.Add(ProjectID.电流基本误差, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "57003":
                                if (!Schema.ContainsKey(ProjectID.有功功率基本误差)) Schema.Add(ProjectID.有功功率基本误差, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "57004":
                                if (!Schema.ContainsKey(ProjectID.无功功率基本误差)) Schema.Add(ProjectID.无功功率基本误差, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "57005":
                                if (!Schema.ContainsKey(ProjectID.功率因素基本误差)) Schema.Add(ProjectID.功率因素基本误差, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "57006":
                                if (!Schema.ContainsKey(ProjectID.常温基本误差)) Schema.Add(ProjectID.常温基本误差, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "57007":
                                if (!Schema.ContainsKey(ProjectID.谐波影响)) Schema.Add(ProjectID.谐波影响, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "57008":
                                if (!Schema.ContainsKey(ProjectID.频率影响)) Schema.Add(ProjectID.频率影响, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "57009":
                                if (!Schema.ContainsKey(ProjectID.电源影响)) Schema.Add(ProjectID.电源影响, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "57010":
                                if (!Schema.ContainsKey(ProjectID.电流不平衡影响)) Schema.Add(ProjectID.电流不平衡影响, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "57011":
                                if (!Schema.ContainsKey(ProjectID.功率因数越限统计)) Schema.Add(ProjectID.功率因数越限统计, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;

                            #endregion

                            #region 终端维护
                            case "58001":
                                if (!Schema.ContainsKey(ProjectID.终端维护)) Schema.Add(ProjectID.终端维护, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "58002":
                                if (!Schema.ContainsKey(ProjectID.交采电量清零)) Schema.Add(ProjectID.交采电量清零, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            case "58003":
                                if (!Schema.ContainsKey(ProjectID.安全模式)) Schema.Add(ProjectID.安全模式, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            #endregion

                            case "14006":
                                if (!Schema.ContainsKey(ProjectID.通电检测)) Schema.Add(ProjectID.通电检测, new SchemaNode());
                                snd = Schema[item.ParaNo];
                                snd.SchemaNodeValue.Add(item.TestValue);
                                break;

                            case "60002":
                                if (!Schema.ContainsKey(ProjectID.密钥下装)) Schema.Add(ProjectID.密钥下装, new SchemaNode());
                                snd = Schema[ProjectID.密钥下装];
                                snd.SchemaNodeValue.Add(item.TestValue ?? "");
                                break;
                            default:
                                //
                                break;
                        }

                    }
                    return true;
                }
                else
                {
                    msg = "下载方案失败" + result.ErrorMessage;
                    return false;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        List<ItemNoClass> ItemNoList = new List<ItemNoClass>();
        private void InitItemNo()
        {
            ItemNoList.Clear();
            ItemNoList.Add(new ItemNoClass() { ProjectID = ProjectID.通电检测, ItemNo = "14006" });
            ItemNoList.Add(new ItemNoClass() { ProjectID = ProjectID.终端逻辑地址查询, ItemNo = "51001" });
            ItemNoList.Add(new ItemNoClass() { ProjectID = ProjectID.时钟召测和对时, ItemNo = "51003" });
            ItemNoList.Add(new ItemNoClass() { ProjectID = ProjectID.密钥下装, ItemNo = "60002" });
        }
        class ItemNoClass
        {
            /// <summary>
            /// 本地ID
            /// </summary>
            public string ProjectID;
            /// <summary>
            /// 平台ID
            /// </summary>
            public string ItemNo;
        }
        /// <summary>
        /// 本地编号转成平台编号
        /// </summary>
        /// <param name="No"></param>
        /// <returns></returns>
        private string GetItemNo(string No)
        {
            var item = ItemNoList.FirstOrDefault(x => x.ProjectID == No);
            if (item != null)
            {
                return item.ItemNo;
            }
            return "";
        }


        public bool SchemeDown(TestMeterInfo barcode, out string schemeName, out Dictionary<string, SchemaNode> Schema)
        {
            throw new NotImplementedException();
        }


        public void ShowPanel(Control panel)
        {
            throw new NotImplementedException();
        }

        public bool Update(List<TestMeterInfo> meters)
        {
            if (!Allow)
            {
                LogManager.AddMessage("登录数据服务失败", EnumLogSource.检定业务日志, EnumLevel.Information);
                return false;
            }
            List<TestResult> Results = new List<TestResult>();
            List<TestItemResult> TestItemResult = new List<TestItemResult>();
            foreach (var item in meters)
            {
                Dictionary<string, MeterResoultData> meterresoultdata = item.MeterResoultData;
                foreach (var items in meterresoultdata.Keys)
                {
                    TestResult test = new TestResult();
                    test.BarCode = item.MD_BarCode;
                    test.DeviceNo = item.MD_DeviceID;
                    test.MeterNo = item.MD_Epitope.ToString();
                    test.TaskNo = item.MD_TaskNo;
                    if (string.IsNullOrWhiteSpace(item.VerifyDate)) test.TestTime = DateTime.Now;
                    else test.TestTime = DateTime.Parse(item.VerifyDate);
                    if (string.IsNullOrWhiteSpace(item.EffectiveDate)) test.ValidTime = DateTime.Now;
                    else test.ValidTime = DateTime.Parse(item.EffectiveDate);
                    test.ItemIndex = "1";//子项目编号
                    List<string> ResultColumnNameList = new List<string>();
                    List<string> ResultColumnValueList = new List<string>();

                    test.ResultColumnName = ""; //结论列名

                    foreach (var ResultColumnNameitem in meterresoultdata[items].meterResoults[0].Datas)
                    {
                        if (ResultColumnNameitem.Key != "项目名")
                        {
                            ResultColumnNameList.Add(ResultColumnNameitem.Key);
                            ResultColumnValueList.Add(ResultColumnNameitem.Value);
                        }
                    }
                    test.ResultColumnName = string.Join("|", ResultColumnNameList);
                    test.Result = meterresoultdata[items].meterResoults[0].Result;
                    test.ResultValue = string.Join("^", ResultColumnValueList);
                    test.ParaNo = GetItemNo(items);//需要解析计算的项目编号
                    if (string.IsNullOrWhiteSpace(test.ParaNo))
                    {
                        LogManager.AddMessage($"项目编号没有找到转码编号，本地项目编号【{items}】", EnumLogSource.检定业务日志, EnumLevel.Information);
                        return false;
                    }
                    //test.TestColumnName = item.Value.ParameterNames;
                    //test.TestParameter = item.Value.ParameterValues;
                    Results.Add(test);
                    List<MeterItemResoultData> meterresoultItemdata = meterresoultdata[items].meterResoults;
                    foreach (var meterResoultsItem in meterresoultItemdata)
                    {
                        int i = 1;
                        foreach (var Subitem in meterResoultsItem.ItemDatas)
                        {
                            TestItemResult testItems = new TestItemResult();
                            testItems.BarCode = item.MD_BarCode;
                            testItems.Data1 = Subitem.StandardData;
                            testItems.Data2 = Subitem.TerminalData;
                            testItems.ItemName = Subitem.Name;
                            testItems.ItemNo = i.ToString();
                            testItems.ParaNo = test.ParaNo;
                            testItems.Result = Subitem.Resoult;
                            testItems.TaskNo = item.MD_TaskNo;
                            testItems.TestTime = Convert.ToDateTime(Subitem.Time);
                            TestItemResult.Add(testItems);
                            i++;
                        }
                    }
                }
            }
            CallResult result = Service.Meter.UploadResultData(Results, 60000);
            if (result.ResultFlag)
            {
                LogManager.AddMessage("UploadResultData上传数据结论完成，结论：" + result.ResultFlag.ToString(), EnumLogSource.检定业务日志, EnumLevel.Information);
                CallResult Itemresult = Service.Meter.UploadItemResultData(TestItemResult, 60000);
                if (Itemresult.ResultFlag)
                {
                    LogManager.AddMessage("UploadItemResultData上传数据结论成功", EnumLogSource.检定业务日志, EnumLevel.Information);
                    return true;
                }
                else
                {
                    LogManager.AddMessage("UploadItemResultData上传数据结论失败：" + Itemresult.ErrorMessage, EnumLogSource.检定业务日志, EnumLevel.TipsError);
                    return false;
                }
            }
            else
            {
                LogManager.AddMessage("UploadResultData上传数据结论失败：" + result.ErrorMessage, EnumLogSource.检定业务日志, EnumLevel.TipsError);
                return false;
            }


        }

        public bool Update(List<TestMeterInfo> meters, int maxtimems, out string msg)
        {
            try
            {
                //if (!Allow)
                //{
                //    LogManager.AddMessage("登录数据服务失败", EnumLogSource.检定业务日志, EnumLevel.Information);
                //    msg = "登录数据服务失败！";
                //    return false;
                //}
                List<TestResult> Results = new List<TestResult>();
                List<TestItemResult> TestItemResult = new List<TestItemResult>();
                foreach (var item in meters)
                {
                    Dictionary<string, MeterResoultData> meterresoultdata = item.MeterResoultData;
                    foreach (var items in meterresoultdata.Keys)
                    {
                        TestResult test = new TestResult();
                        test.BarCode = item.MD_BarCode;
                        test.DeviceNo = item.MD_DeviceID;
                        test.MeterNo = item.MD_Epitope.ToString();
                        test.TaskNo = item.MD_TaskNo;
                        if (string.IsNullOrWhiteSpace(item.VerifyDate)) test.TestTime = DateTime.Now;
                        else test.TestTime = DateTime.Parse(item.VerifyDate);
                        if (string.IsNullOrWhiteSpace(item.EffectiveDate)) test.ValidTime = DateTime.Now;
                        else test.ValidTime = DateTime.Parse(item.EffectiveDate);
                        test.ItemIndex = "1";//子项目编号
                        List<string> ResultColumnNameList = new List<string>();
                        List<string> ResultColumnValueList = new List<string>();

                        test.ResultColumnName = ""; //结论列名

                        foreach (var ResultColumnNameitem in meterresoultdata[items].meterResoults[0].Datas)
                        {
                            if (ResultColumnNameitem.Key != "项目名")
                            {
                                ResultColumnNameList.Add(ResultColumnNameitem.Key);
                                ResultColumnValueList.Add(ResultColumnNameitem.Value);
                            }
                        }
                        test.ResultColumnName = string.Join("|", ResultColumnNameList);
                        test.Result = meterresoultdata[items].meterResoults[0].Result;
                        test.ResultValue = string.Join("^", ResultColumnValueList);
                        test.ParaNo = GetItemNo(items);//需要解析计算的项目编号
                        if (string.IsNullOrWhiteSpace(test.ParaNo))
                        {
                            LogManager.AddMessage($"项目编号没有找到转码编号，本地项目编号【{items}】", EnumLogSource.检定业务日志, EnumLevel.Information);
                            msg = $"项目编号没有找到转码编号，本地项目编号【{items}】";
                            return false;
                        }
                        //test.TestColumnName = item.Value.ParameterNames;
                        //test.TestParameter = item.Value.ParameterValues;
                        Results.Add(test);
                        List<MeterItemResoultData> meterresoultItemdata = meterresoultdata[items].meterResoults;
                        foreach (var meterResoultsItem in meterresoultItemdata)
                        {
                            int i = 1;
                            foreach (var Subitem in meterResoultsItem.ItemDatas)
                            {
                                TestItemResult testItems = new TestItemResult();
                                testItems.BarCode = item.MD_BarCode;
                                testItems.Data1 = Subitem.StandardData;
                                testItems.Data2 = Subitem.TerminalData;
                                testItems.ItemName = Subitem.Name;
                                testItems.ItemNo = i.ToString();
                                testItems.ParaNo = test.ParaNo;
                                testItems.Result = Subitem.Resoult;
                                testItems.TaskNo = item.MD_TaskNo;
                                testItems.TestTime = Convert.ToDateTime(Subitem.Time);
                                TestItemResult.Add(testItems);
                                i++;
                            }
                        }
                    }
                }
                CallResult result = Service.Meter.UploadResultData(Results, 60000);
                if (result.ResultFlag)
                {
                    LogManager.AddMessage("UploadResultData上传数据结论完成，结论：" + result.ResultFlag.ToString(), EnumLogSource.检定业务日志, EnumLevel.Information);
                    CallResult Itemresult = Service.Meter.UploadItemResultData(TestItemResult, 60000);
                    if (Itemresult.ResultFlag)
                    {
                        LogManager.AddMessage("UploadItemResultData上传数据结论成功", EnumLogSource.检定业务日志, EnumLevel.Information);
                        msg = "UploadItemResultData上传数据结论成功";
                        return true;
                    }
                    else
                    {
                        LogManager.AddMessage("UploadItemResultData上传数据结论失败：" + Itemresult.ErrorMessage, EnumLogSource.检定业务日志, EnumLevel.TipsError);
                        msg = "UploadItemResultData上传数据结论失败：" + Itemresult.ErrorMessage;
                        return false;
                    }
                }
                else
                {
                    LogManager.AddMessage("UploadResultData上传数据结论失败：" + result.ErrorMessage, EnumLogSource.检定业务日志, EnumLevel.TipsError);
                    msg = "UploadResultData上传数据结论失败：" + result.ErrorMessage;
                    return false;
                }
            }
            catch (Exception ex)
            {

                LogManager.AddMessage("上传数据异常：" + ex.ToString(), EnumLogSource.检定业务日志, EnumLevel.TipsError);
                msg = "上传数据异常：" + ex.ToString();
                return false;
            }

        }

        public bool Update(TestMeterInfo meters)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCompleted()
        {
            return true;
        }

        public bool UpdateCompleted(string DETECT_TASK_NO, string SYS_NO)
        {
            throw new NotImplementedException();
        }

        public void UpdateInit()
        {

        }

        public bool UploadVersion(string stationId, string version, out string msg)
        {
            //登入API，并且上传软件版本号
            try
            {
                if (Service.LoginApi(User, PassWord))
                {
                    CallResult result = Service.Station.UploadSoftwareVersion(stationId, version, 10000);
                    if (result.ResultFlag)
                    {
                        msg = "上传软件版本成功";
                        Allow = true;
                        return true;
                    }
                    else
                    {
                        msg = "上传软件版本失败" + result.ErrorMessage;
                        return false;
                    }
                }
                else
                {
                    msg = "数据服务API登入失败";
                    return false;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        public bool GetServerTime(out string Msg)
        {
            Msg = "异常，访问时间数据接口失败——GetSystemTime()";
            ApiResult<DateTime> result = Service.Base.GetSystemTime();
            if (result.ResultFlag)
            {
                DateTime sysTime = result.Data;
                DateTime PCTime = DateTime.Now;
                if (sysTime>PCTime.AddMinutes(5)|| sysTime < PCTime.AddMinutes(-5)) {
                    Msg = "与系统时间超差,系统时间:"+ sysTime.ToString()+"|当前时间:"+ PCTime.ToString();
                    return false;
                }
                else
                {
                    Msg = "时间对比完成,系统时间:" + sysTime.ToString() + "|当前时间:" + PCTime.ToString();
                    return true;
                }
            }
            return false;
        }

        void IMis.UpdateCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
