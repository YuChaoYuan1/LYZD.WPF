using LYZD.Core.Enum;
using LYZD.Core.Model.Meter;
using LYZD.Core.Model.Schema;
using LYZD.DAL;
using LYZD.DAL.Config;
using LYZD.Mis;
using LYZD.Mis.Common;
using LYZD.Mis.MDS;
using LYZD.Utility;
using LYZD.Utility.Log;
using LYZD.ViewModel.CheckController;
using LYZD.ViewModel.Const;
using LYZD.ViewModel.Device;
using LYZD.ViewModel.Model;
using LYZD.ViewModel.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using ZH.MeterProtocol.Enum;
using ZH.MeterProtocol.Protocols.DgnProtocol;
using ZH.MeterProtocol.Struct;

namespace LYZD.ViewModel.InputPara
{
    /// <summary>
    /// 表信息录入数据模型
    /// </summary>
    public class MeterInputParaViewModel : ViewModelBase
    {
        /// <summary>
        /// 正在下载true
        /// </summary>
        private static bool DownLoading = false;
        
        /// <summary>
        /// 参数录入时的构造函数,从内存中加载表信息,由于是在xaml页面中构造,所有没有参数
        /// </summary>
        public MeterInputParaViewModel()
        {

            Initial();
            for (int i = 0; i < ParasModel.AllUnits.Count; i++)
            {
                for (int j = 0; j < Meters.Count; j++)
                {
                    string fieldName = ParasModel.AllUnits[i].FieldName;
                    object objTemp = EquipmentData.MeterGroupInfo.Meters[j].GetProperty(fieldName);
                    Meters[j].SetProperty(fieldName, objTemp);
                }
            }
            RefreshFirstMeterInfo();
        }



        /// <summary>
        /// 程序启动时的构造函数,从数据库加载
        /// </summary>
        /// <param name="isCurrent"></param>
        public MeterInputParaViewModel(bool isCurrent)
        {
            Initial();
            LoadMetersFromTempDb();
            //如果表位数与当前表位数不符,执行换新表
            if (Meters.Count != EquipmentData.Equipment.MeterCount)
            {
                NewMeters();
            }
        }
        /// <summary>
        /// 空方法,用于初始化表信息
        /// </summary>
        public void Initialize()
        {
            //EquipmentData.ZaiBoInfo = new CarrierList();
            //EquipmentData.ZaiBoInfo.Load();
        }

        private InputParaViewModel parasModel = new InputParaViewModel();
        /// <summary>
        /// 表信息录入相关的数据模型
        /// </summary>
        public InputParaViewModel ParasModel
        {
            get { return parasModel; }
            set { SetPropertyValue(value, ref parasModel, "ParasModel"); }
        }
        private AsyncObservableCollection<DynamicViewModel> meters = new AsyncObservableCollection<DynamicViewModel>();
        /// <summary>
        /// 表信息集合
        /// </summary>
        public AsyncObservableCollection<DynamicViewModel> Meters
        {
            get { return meters; }
            set { SetPropertyValue(value, ref meters, "Meters"); }
        }

        private DynamicViewModel firstMeter = new DynamicViewModel(0);
        /// <summary>
        /// 表位基本信息
        /// </summary>
        public DynamicViewModel FirstMeter
        {
            get { return firstMeter; }
            set { SetPropertyValue(value, ref firstMeter, "FirstMeter"); }
        }
        /// <summary>
        /// 表位是否要检
        /// </summary>
        /// 
        public bool[] YaoJian
        {
            get
            {
                bool[] arrayTemp = new bool[EquipmentData.Equipment.MeterCount];
                for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
                {
                    arrayTemp[i] = Meters[i].GetProperty("MD_CHECKED") as string == "1";
                }
                return arrayTemp;
            }
        }
        /// <summary>
        /// 初始化表信息
        /// </summary>
        private void Initial()
        {
            int meterCount = EquipmentData.Equipment.MeterCount;
            #region 赋初值
            for (int i = 0; i < meterCount; i++)
            {
                DynamicViewModel viewModel = null;
                if (i >= Meters.Count)
                {
                    viewModel = new DynamicViewModel(i + 1);
                    meters.Add(viewModel);
                }
                else
                {
                    viewModel = Meters[i];
                }
                //设置默认值
                for (int j = 0; j < ParasModel.AllUnits.Count; j++)
                {
                    InputParaUnit paraUnit = ParasModel.AllUnits[j];
                    if (paraUnit.FieldName == "MD_DEVICE_ID")
                    {
                        viewModel.SetProperty("MD_DEVICE_ID", EquipmentData.Equipment.ID);
                    }
                    else if (paraUnit.FieldName == "MD_EPITOPE")
                    {
                        viewModel.SetProperty("MD_EPITOPE", i + 1);
                    }
                    else if (paraUnit.FieldName == "DTM_TEST_DATE")
                    {
                        viewModel.SetProperty("DTM_TEST_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        if (paraUnit.IsNewValue)
                        {
                            if (!string.IsNullOrEmpty(paraUnit.DefaultValue))
                            {
                                viewModel.SetProperty(paraUnit.FieldName, paraUnit.DefaultValue);
                            }
                            else
                            {
                                viewModel.SetProperty(paraUnit.FieldName, "");
                            }
                        }
                    }
                }
            }
            #endregion

            for (int i = 0; i < Meters.Count; i++)
            {
                Meters[i].PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "MD_CHECKED")
                    {
                        RefreshFirstMeterInfo();
                    }
                };
            }
        }

        /// <summary>
        /// 从临时数据库加载表信息
        /// </summary>
        private void LoadMetersFromTempDb()
        {

            List<DynamicModel> models = DALManager.MeterTempDbDal.GetList("T_TMP_METER_INFO", string.Format("1=1 order by MD_EPITOPE"));
            for (int i = 0; i < models.Count; i++)
            {
                object obj = models[i].GetProperty("MD_EPITOPE");
                if (obj is int)
                {
                    int index = (int)obj;
                    if (index <= Meters.Count && index > 0)
                    {
                        //Meters[index - 1] = new DynamicViewModel(models[i], index);
                        for (int j = 0; j < ParasModel.AllUnits.Count; j++)
                        {
                            InputParaUnit paraUnitTemp = ParasModel.AllUnits[j];
                            if (paraUnitTemp.ValueType == InputParaUnit.EnumValueType.编码值)
                            {
                                Meters[index - 1].SetProperty(paraUnitTemp.FieldName, CodeDictionary.GetNameLayer2(paraUnitTemp.CodeType, models[i].GetProperty(paraUnitTemp.FieldName) as string));
                            }
                            else
                            {
                                Meters[index - 1].SetProperty(paraUnitTemp.FieldName, models[i].GetProperty(paraUnitTemp.FieldName));
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < Meters.Count; i++)
            {
                string pkObj = Meters[i].GetProperty("METER_ID") as string;
                if (string.IsNullOrEmpty(pkObj) || pkObj.Length < 8)
                {
                    Meters[i].SetProperty("METER_ID", GetUniquenessID8(i + 1).ToString());
                }
            }
            RefreshFirstMeterInfo();
        }
        /// <summary>
        /// 验证数据是否完整
        /// </summary>
        /// <param name="stringError"></param>
        /// <returns></returns>
        public bool CheckInfoCompleted(out string stringError)
        {
            stringError = "";
            if (EquipmentData.SchemaModels.SelectedSchema == null)
            {
                stringError = "检定方案不能为空,请指定当前检定方案!";
                return false;
            }
            bool[] yaojian = YaoJian;
            bool flagHaveYaojian = false;
            for (int i = 0; i < yaojian.Length; i++)
            {
                if (yaojian[i])
                {
                    flagHaveYaojian = true;
                    break;
                }
            }
            if (!flagHaveYaojian)
            {
                stringError = "请至少选择一块要检的表";
                return false;
            }
            for (int j = 0; j < Meters.Count; j++)
            {
                if (!yaojian[j])
                {
                    continue;
                }
                for (int i = 0; i < ParasModel.AllUnits.Count; i++)
                {
                    if (ParasModel.AllUnits[i].IsDisplayMember && ParasModel.AllUnits[i].IsNecessary)
                    {
                        if (Meters[j].GetProperty(ParasModel.AllUnits[i].FieldName) == null || string.IsNullOrEmpty(Meters[j].GetProperty(ParasModel.AllUnits[i].FieldName).ToString()))
                        {
                            stringError = string.Format("表位{0}缺少信息: {1}", j + 1, ParasModel.AllUnits[i].DisplayName);
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 更新第一块要检表信息
        /// </summary>
        private void RefreshFirstMeterInfo()
        {
            bool[] yaojian = YaoJian;
            for (int i = 0; i < yaojian.Length; i++)
            {
                if (yaojian[i])
                {
                    FirstMeter = Meters[i];
                    break;
                }
            }
        }
        /// <summary>
        /// 保存表信息
        /// </summary>
        private void SaveMeterInfo()
        {
            #region 转换显示数据为数据库数据
            List<DynamicModel> models = new List<DynamicModel>();
            for (int i = 0; i < Meters.Count; i++)
            {
                DynamicModel modelTemp = new DynamicModel();

                for (int j = 0; j < ParasModel.AllUnits.Count; j++)
                {
                    InputParaUnit paraUnitTemp = ParasModel.AllUnits[j];
                    if (paraUnitTemp.ValueType == InputParaUnit.EnumValueType.编码值)
                    {
                        modelTemp.SetProperty(paraUnitTemp.FieldName, CodeDictionary.GetValueLayer2(paraUnitTemp.CodeType, Meters[i].GetProperty(paraUnitTemp.FieldName) as string));
                    }
                    else
                    {
                        modelTemp.SetProperty(paraUnitTemp.FieldName, Meters[i].GetProperty(paraUnitTemp.FieldName));
                    }
                }
                modelTemp.SetProperty("MD_SCHEME_ID", EquipmentData.Schema.SchemaId);  //方案编号
                modelTemp.SetProperty("MD_TEMPERATURE", ConfigHelper.Instance.Temperature);  //温度
                modelTemp.SetProperty("MD_HUMIDITY", ConfigHelper.Instance.Humidity);  //湿度
                modelTemp.SetProperty("MD_OTHER_2", "未上传");   //设置表位需要上传数据

                //modelTemp.SetProperty("MD_SUPERVISOR", "");  //主管
                //if (GetMeterInfo(i, "MD_AUDIT_PERSON").Trim()=="")
                //{
                //    modelTemp.SetProperty("MD_AUDIT_PERSON", EquipmentData.LastCheckInfo.AuditPerson);  //核验员
                //}
                //if (GetMeterInfo(i, "MD_AUDIT_PERSON").Trim() == "")
                //{
                //    modelTemp.SetProperty("MD_TEST_PERSON", EquipmentData.LastCheckInfo.TestPerson);  //检验员
                //}
                //if (GetMeterInfo(i, "MD_AUDIT_PERSON").Trim() == "")
                //{
                //    modelTemp.SetProperty("MD_TEST_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));  //检定日期   
                //}
                //if (GetMeterInfo(i, "MD_AUDIT_PERSON").Trim() == "")
                //{
                //   modelTemp.SetProperty("MD_VALID_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));  //计检日
                //}
                //modelTemp.SetProperty("MD_AUDIT_PERSON", EquipmentData.LastCheckInfo.AuditPerson);  //核验员
                //modelTemp.SetProperty("MD_TEST_PERSON", EquipmentData.LastCheckInfo.TestPerson);  //检验员
                //modelTemp.SetProperty("MD_TEST_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));  //检定日期   
                //modelTemp.SetProperty("MD_VALID_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));  //计检日期

                //TODO2保存没有录入的数据
                models.Add(modelTemp);
            }
            #endregion
            #region 获取当前表数量
            List<string> pkList = new List<string>();
            for (int i = 0; i < Meters.Count; i++)
            {
                pkList.Add(string.Format("METER_ID = '{0}'", Meters[i].GetProperty("METER_ID")));
            }
            string pkWhere = string.Join(" or ", pkList);
            #endregion
            int countInDb = DALManager.MeterTempDbDal.GetCount("T_TMP_METER_INFO", pkWhere);
            #region 插入新数据
            if (countInDb != Meters.Count)
            {
                int deleteCount = DALManager.MeterTempDbDal.Delete("T_TMP_METER_INFO", "1=1");
                LogManager.AddMessage(string.Format("数据库中表数量:{1}块 与当前录入表数量:{2}块 不一致,删除表信息,共删除{0}条表信息数据.", deleteCount, countInDb, Meters.Count), EnumLogSource.数据库存取日志);
                int insertCount = DALManager.MeterTempDbDal.Insert("T_TMP_METER_INFO", models);
                LogManager.AddMessage(string.Format("更新表信息,共插入{0}条表信息数据.", insertCount), EnumLogSource.数据库存取日志);
                return;
            }
            #endregion
            #region 更新现有信息
            List<string> fieldNames = new List<string>();
            var namesTemp = from item in ParasModel.AllUnits select item.FieldName;
            fieldNames = namesTemp.ToList();
            fieldNames.Remove("METER_ID");
            int updateCount = DALManager.MeterTempDbDal.Update("T_TMP_METER_INFO", "METER_ID", models, fieldNames);
            LogManager.AddMessage(string.Format("更新表信息,共更新{0}条表信息数据.", updateCount), EnumLogSource.数据库存取日志);
            #endregion
        }
        /// <summary>
        /// 更新表信息
        /// </summary>
        public void UpdateMeterInfo()
        {
            string errorString = "";
            if (!CheckInfoCompleted(out errorString))
            {
                MessageBox.Show(errorString, "表信息不完整");
                return;
            }
            string MD_TaskNo = EquipmentData.LoadDETECT_TASK.DETECT_TASK_NO;
            if (ConfigHelper.Instance.VerifyModel != "自动模式")
            {

                //要检定数量-电压-电流-频率-解析方式-互感器-协议类型-等级-常数
                string tips = $"检定数量:{ YaoJian.Count(item => item == true)}";
                tips += $"\r\n电压:{Meters[0].GetProperty("MD_UB")}";
                tips += $"\r\n电流:{Meters[0].GetProperty("MD_UA")}";
                tips += $"\r\n接线方式:{Meters[0].GetProperty("MD_WIRING_MODE")}";
                tips += $"\r\n{Meters[0].GetProperty("MD_CONNECTION_FLAG")}";
                tips += $"\r\n等级:{Meters[0].GetProperty("MD_GRADE")}";
                tips += $"\r\n常数{Meters[0].GetProperty("MD_CONSTANT")}";
                tips += $"\r\n通讯协议:{Meters[0].GetProperty("MD_PROTOCOL_TYPE")}";
                tips += $"\r\n任务号:{Meters[0].GetProperty("MD_TASK_NO")}";
                if (MessageBox.Show(tips, "请确定", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    return;
                }
                MD_TaskNo = Meters[0].GetProperty("MD_TASK_NO").ToString(); ;
            }

            SaveMeterInfo();
            //MD_TerminalType
            if (ConfigHelper.Instance.Marketing_Type == "南瑞_智慧工控平台2.0" && (ConfigHelper.Instance.AreaName == "山西太原"))
            {
                Utility.TaskManager.AddWcfAction(() =>
                {
                    List<int> DefeatedNumber = new List<int>();

                    CheckOrderNo(VerifyBase.meterInfo, MD_TaskNo, out DefeatedNumber);
                    if (DefeatedNumber.Count > 0)
                    {
                        string tip = "第";
                        foreach (var item in DefeatedNumber)
                        {
                            tip += item.ToString() + ",";
                        }
                        tip += "表位核验失败";
                        Utility.Log.LogManager.AddMessage(tip, Utility.Log.EnumLogSource.设备操作日志, Utility.Log.EnumLevel.TipsError);
                    }
                    else
                    {
                        Utility.Log.LogManager.AddMessage("数据核验完成", Utility.Log.EnumLogSource.设备操作日志, Utility.Log.EnumLevel.Information);
                    }
                });

            }
            EquipmentData.LoadDETECT_TASK.DETECT_TASK_NO = MD_TaskNo;
            EquipmentData.MeterGroupInfo.LoadMetersFromTempDb(); //重新读取
            EquipmentData.CheckResults.RefreshYaojian();

            ////【标注：检定--发送表的数据】 
            TaskManager.AddWcfAction(() =>
            {
                //将表数据存放到类中
                VerifyBase.meterInfo = GetVerifyMeterInfo();
                VerifyBase.InitTerminalTalks();
                //IsReadAddress(VerifyBase.meterInfo);

                SetTerminalTypeErrorBoard();
                //大小电流切换版
                if (ConfigHelper.Instance.Marketing_Type == "LY数据服务" && ConfigHelper.Instance.VerifyModel == "自动模式")
                {
                    SetPowerContlor();
                }
            });


            if (EquipmentData.Controller.Index == -1)
            {
                EquipmentData.LastCheckInfo.SchemaId = EquipmentData.Schema.SchemaId;
                EquipmentData.LastCheckInfo.CheckIndex = 0;
                EquipmentData.Controller.Index = 0;
            }
            UiInterface.CloseWindow("参数录入");
            EquipmentData.NavigateCurrentUi();
        }

        /// <summary>
        /// 流水线自动录入完成
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdateMeterInfoAuto(out string msg)
        {
            msg = "";

            if (!CheckInfoCompleted(out string errorString))
            {
                msg = $"表信息不完整{errorString}";
                LogManager.AddMessage(msg, EnumLogSource.服务器日志, EnumLevel.Error);
                return false;
            }
            SaveMeterInfo();
            EquipmentData.MeterGroupInfo.LoadMetersFromTempDb(); //重新读取
            EquipmentData.CheckResults.RefreshYaojian();

            ////【标注：检定--发送表的数据】 
            TaskManager.AddWcfAction(() =>
            {
                //将表数据存放到类中
                VerifyBase.meterInfo = GetVerifyMeterInfo();
                VerifyBase.InitTerminalTalks();

            });


            if (EquipmentData.Controller.Index == -1)
            {
                EquipmentData.LastCheckInfo.SchemaId = EquipmentData.Schema.SchemaId;
                EquipmentData.LastCheckInfo.CheckIndex = 0;
                EquipmentData.Controller.Index = 0;
            }
            return true;
        }

        private void CheckOrderNo(TestMeterInfo[] meterInfo, string OrderNo, out List<int> DefeatedNumber)
        {
            List<int> newDefeatedNumber = new List<int>();
            try
            {
                IMis mis = MISFactory.Create();
                string EquipmentNo = ConfigHelper.Instance.EquipmentNo;
                ((Mis.NanRui.NanRui)mis).CheckOrderNo(meterInfo, OrderNo, out DefeatedNumber, EquipmentNo);
            }
            catch (Exception ex)
            {
                DefeatedNumber = newDefeatedNumber;
                Utility.Log.LogManager.AddMessage("平台连接验证数据异常" + "\n" + ex.ToString(), Utility.Log.EnumLogSource.设备操作日志, Utility.Log.EnumLevel.Error);
            }


        }


        /// <summary>
        /// 根据终端类型设置误差版
        /// </summary>
        public void SetTerminalTypeErrorBoard()
        {
            foreach (var item in VerifyBase.meterInfo)
            {
                if (item.YaoJianYn)
                {
                    int num = EquipmentData.DeviceManager.Devices[DeviceName.误差板].Count;

                    byte type = 0;
                    for (int i = 0; i < num; i++)
                    {
                        switch (item.MD_TerminalType)
                        {
                            case "集中器I型13版":
                                type = 1;
                                break;
                            case "集中器I型22版":
                                type = 2;
                                break;
                            case "专变III型13版":
                                type = 3;
                                break;
                            case "专变III型22版":
                                type = 4;
                                break;
                            case "融合终端":
                                type = 5;
                                break;
                            case "能源控制器":
                                type = 6;
                                break;
                            default:
                                break;
                        }
                        EquipmentData.DeviceManager.SetZDType(type, (byte)i);
                    }
                    break;

                }
            }
        }

        /// <summary>
        /// 流水线控制大小电流切换版
        /// </summary>
        public void SetPowerContlor()
        {
            foreach (var item in VerifyBase.meterInfo)
            {
                if (item.YaoJianYn)
                {
                    int num = EquipmentData.DeviceManager.Devices[DeviceName.多功能板].Count;
                    byte type = 0;
                    for (int i = 0; i < num; i++)
                    {
                        switch (item.MD_TerminalType)
                        {
                            case "集中器I型13版":
                            case "集中器I型22版":
                            case "专变III型13版":
                            case "专变III型22版":
                                type = 0;
                                break;
                            case "融合终端":
                            case "能源控制器":
                                type = 1;
                                break;
                            default:
                                break;
                        }
                        EquipmentData.DeviceManager.PowerTpye((byte)0x13, type, i);
                        System.Threading.Thread.Sleep(150);
                    }
                    break;

                }
            }


        }

        /// <summary>
        /// 更新表信息
        /// </summary>
        public void UpdateMeterInfo2()
        {
            string errorString = "";
            if (!CheckInfoCompleted(out errorString))
            {
                MessageBox.Show(errorString, "表信息不完整");
                return;
            }

            SaveMeterInfo();



            EquipmentData.MeterGroupInfo.LoadMetersFromTempDb(); //重新读取
            EquipmentData.CheckResults.RefreshYaojian();

            //【标注：检定--发送表的数据】 
            TaskManager.AddWcfAction(() =>
            {
                //将表数据存放到类中
                VerifyBase.meterInfo = GetVerifyMeterInfo();
                VerifyBase.InitTerminalTalks();
                //IsReadAddress(VerifyBase.meterInfo);
            });


            if (EquipmentData.Controller.Index == -1)
            {
                EquipmentData.LastCheckInfo.SchemaId = EquipmentData.Schema.SchemaId;
                EquipmentData.LastCheckInfo.CheckIndex = 0;
                EquipmentData.Controller.Index = 0;
            }
            //UiInterface.CloseWindow("参数录入");
            //EquipmentData.NavigateCurrentUi();
        }



        /// <summary>
        /// 换新表
        /// </summary>
        public void NewMeters()
        {
            if (MessageBox.Show("确认要更换新表吗?更换新表操作将会删除当前批次表的检定结论,请确认检定数据已经上传,或者当前检定数据无效再执行更换新表操作", "更换新表", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                EquipmentData.Controller.Index = -1;
                Initial();
                for (int i = 0; i < Meters.Count; i++)
                {
                    Meters[i].SetProperty("METER_ID", GetUniquenessID8(i + 1).ToString());
                }
                SaveMeterInfo();
                LoadMetersFromTempDb();

                //清空临时数据库中的结论
                CheckResultBll.Instance.DeleteResultFromTempDb();
                //删除检定结论
                EquipmentData.CheckResults.ClearAllResult();

                EquipmentData.LastCheckInfo.CheckIndex = -1;
                EquipmentData.NavigateCurrentUi();
                EquipmentData.SchemaModels.RefreshCurrrentSchema();
            }
        }

        /// <summary>
        /// 换新表
        /// </summary>
        public void NewMeters2()
        {
            EquipmentData.Controller.Index = -1;
            Initial();
            for (int i = 0; i < Meters.Count; i++)
            {
                Meters[i].SetProperty("METER_ID", GetUniquenessID8(i + 1).ToString());
            }
            SaveMeterInfo();
            LoadMetersFromTempDb();

            //清空临时数据库中的结论
            CheckResultBll.Instance.DeleteResultFromTempDb();
            //删除检定结论
            EquipmentData.CheckResults.ClearAllResult();
            EquipmentData.LastCheckInfo.CheckIndex = -1;
            //EquipmentData.NavigateCurrentUi();
        }



        /// <summary>
        /// 转换数据库信息到检定时需要用的表数据类
        /// </summary>
        /// <returns></returns>
        public TestMeterInfo[] GetVerifyMeterInfo()
        {
            EquipmentData.TerminalIndexEthernet.Clear();
            bool[] yaojianTemp = YaoJian;
            TestMeterInfo[] meterInfos = new TestMeterInfo[Meters.Count];
            //string str = "";
            ZH.MeterProtocol.App.CarrierInfos = new CarrierWareInfo[Meters.Count];
            for (int i = 0; i < Meters.Count; i++)
            {
                int t = 0;
                float t2 = 0f;
                meterInfos[i] = new TestMeterInfo();
                //表唯一ID
                meterInfos[i].Meter_ID = GetMeterInfo(i, "METER_ID");
                //电压
                float.TryParse(GetMeterInfo(i, "MD_UB"), out t2);
                meterInfos[i].MD_UB = t2;
                //电流
                meterInfos[i].MD_UA = GetMeterInfo(i, "MD_UA");
                //频率
                int.TryParse(GetMeterInfo(i, "MD_FREQUENCY"), out t);
                meterInfos[i].MD_Frequency = t;
                //检定类型--全检抽检
                meterInfos[i].MD_TestType = GetMeterInfo(i, "MD_TEST_TYPE");
                //  测量方式--单相-三相三线-三相四线
                meterInfos[i].MD_WiringMode = GetMeterInfo(i, "MD_WIRING_MODE");
                //互感器
                meterInfos[i].MD_ConnectionFlag = GetMeterInfo(i, "MD_CONNECTION_FLAG");
                //表位
                meterInfos[i].MD_Epitope = i + 1;
                //条形码
                meterInfos[i].MD_BarCode = GetMeterInfo(i, "MD_BAR_CODE");
                //批次号
                meterInfos[i].MD_BatchNo = GetMeterInfo(i, "MD_BATCH_NO");
                //任务编号
                meterInfos[i].MD_TaskNo = GetMeterInfo(i, "MD_TASK_NO");
                //等级
                meterInfos[i].MD_Grane = GetMeterInfo(i, "MD_GRADE");
                //常数
                meterInfos[i].MD_Constant = GetMeterInfo(i, "MD_CONSTANT");
                //通讯地址 --逻辑地址
                meterInfos[i].Address = GetMeterInfo(i, "MD_POSTAL_ADDRESS");

                //出厂编号
                meterInfos[i].MD_MadeNo = GetMeterInfo(i, "MD_MADE_NO");
                //终端类型
                meterInfos[i].MD_TerminalType = GetMeterInfo(i, "MD_TERMINAL_TYPE");
                //制造厂家
                meterInfos[i].MD_Factory = GetMeterInfo(i, "MD_FACTORY");
                //送检单位
                meterInfos[i].MD_Customer = GetMeterInfo(i, "MD_CUSTOMER");
                //终端型号
                meterInfos[i].MD_TerminalModel = GetMeterInfo(i, "MD_TERMINAL_MODEL");
                //计量编号
                meterInfos[i].MD_MeasurementNo = GetMeterInfo(i, "MD_MEASUREMENT_NO");
                //IP地址
                meterInfos[i].MD_IpAddress = GetMeterInfo(i, "MD_IP_ADDRESS");
                //是否要检
                meterInfos[i].YaoJianYn = yaojianTemp[i];
                //证书编号
                meterInfos[i].MD_CertificateNo = GetMeterInfo(i, "MD_CERTIFICATE_NO");
                //载波厂家
                meterInfos[i].MD_CarrierFactory = GetMeterInfo(i, "MD_CREEIER_FACTORY");
                //载波型号
                meterInfos[i].MD_CarrierModel = GetMeterInfo(i, "MD_CARRIER_MODEL");
                //采集器地址
                meterInfos[i].MD_CollectorAddress = GetMeterInfo(i, "MD_COLLECTOR_ADDRESS");
                //台体编号
                meterInfos[i].MD_DeviceID = GetMeterInfo(i, "MD_DEVICE_ID");
                //出厂日期
                meterInfos[i].MD_MadtDate = GetMeterInfo(i, "MD_MADT_DATE");
                //串口数据

                if (GetMeterInfo(i, "MD_PORT_DATA") != null)
                {
                    EquipmentData.DeviceManager.MeterUnits[i].ComParam = GetMeterInfo(i, "MD_PORT_DATA");
                }

                meterInfos[i].MD_PortData = GetMeterInfo(i, "MD_PORT_DATA");
                //方案编号
                meterInfos[i].MD_SchemeID = GetMeterInfo(i, "MD_SCHEME_ID");
                //检验员
                meterInfos[i].MD_TestPerson = GetMeterInfo(i, "MD_TEST_PERSON");
                meterInfos[i].FKType = 1;
                meterInfos[i].MD_JJGC = "JJG596-2012";
                //协议类型
                meterInfos[i].MD_Protocol_Type = GetMeterInfo(i, "MD_PROTOCOL_TYPE");
                //通讯方式
                //meterInfos[i].MD_ConnType = GetMeterInfo(i, "MD_CONN_TYPE");
                string connType = GetMeterInfo(i, "MD_CONN_TYPE");
                switch (connType)
                {
                    case "232通讯":
                        meterInfos[i].MD_ConnType = Cus_EmChannelType.Channel232;
                        break;
                    case "以太网通讯":
                        meterInfos[i].MD_ConnType = Cus_EmChannelType.ChannelEther;
                        break;
                    case "维护口通讯":
                        meterInfos[i].MD_ConnType = Cus_EmChannelType.ChannelMaintain;
                        break;
                    default:
                        meterInfos[i].MD_ConnType = Cus_EmChannelType.ChannelEther;
                        break;
                }


                meterInfos[i].EffectiveDate = GetMeterInfo(i, "MD_VALID_DATA");  //有效期
                meterInfos[i].VerifyDate = GetMeterInfo(i, "MD_TEST_DATE"); //检定日期[YYYY - MM - DD HH: NN:SS]
                meterInfos[i].Humidity = GetMeterInfo(i, "MD_HUMIDITY");//湿度
                meterInfos[i].Temperature = GetMeterInfo(i, "MD_TEMPERATURE");//温度
                meterInfos[i].Checker1 = GetMeterInfo(i, "MD_TEST_PERSON"); //检验员
                meterInfos[i].Checker2 = GetMeterInfo(i, "MD_AUDIT_PERSON"); //核验员
                meterInfos[i].Other1 = GetMeterInfo(i, "MD_OTHER_1"); //备用1
                meterInfos[i].Other2 = GetMeterInfo(i, "MD_OTHER_2"); //备用2
                meterInfos[i].Other3 = GetMeterInfo(i, "MD_OTHER_3"); //备用3
                meterInfos[i].Other4 = GetMeterInfo(i, "MD_OTHER_4"); //备用4
                meterInfos[i].Other5 = GetMeterInfo(i, "MD_OTHER_5"); //备用5


                meterInfos[i].ProtInfo = new ZH.MeterProtocol.Struct.ComPortInfo();
                int.TryParse(EquipmentData.DeviceManager.MeterUnits[i].PortNum, out t);//端口号
                meterInfos[i].ProtInfo.Port = t;
                meterInfos[i].ProtInfo.Setting = EquipmentData.DeviceManager.MeterUnits[i].ComParam;
                meterInfos[i].ProtInfo.IP = EquipmentData.DeviceManager.MeterUnits[i].Address;
                meterInfos[i].ProtInfo.IsExist = true;
                meterInfos[i].ProtInfo.OtherParams = string.Empty;
                int.TryParse(EquipmentData.DeviceManager.MeterUnits[i].StartPort, out t);//端口号
                meterInfos[i].ProtInfo.StartPort = t;
                int.TryParse(EquipmentData.DeviceManager.MeterUnits[i].RemotePort, out t);//端口号
                meterInfos[i].ProtInfo.RemotePort = t;

                if (EquipmentData.DeviceManager.MeterUnits[i].Conn_Type == CommMode.COM)
                {
                    meterInfos[i].ProtInfo.LinkType = LinkType.COM;
                }
                else
                {
                    meterInfos[i].ProtInfo.LinkType = LinkType.UDP;
                }
                meterInfos[i].ProtInfo.MaxTimePerByte = EquipmentData.DeviceManager.MeterUnits[i].MaxTimePerByte;
                meterInfos[i].ProtInfo.MaxTimePerFrame = EquipmentData.DeviceManager.MeterUnits[i].MaxTimePerFrame;
                Core.DataBase.clsWcLimitDataControl.IBString = meterInfos[i].MD_UA;

                if (meterInfos[i].Address == null || meterInfos[i].Address == "")
                {

                    if (meterInfos[i].MD_Protocol_Type == "376.1")
                    {
                        meterInfos[i].Address = "AAAAAAAAAAAA";
                    }
                    else
                    {
                        meterInfos[i].Address = "FFFFFFFF";
                    }
                }
            }
            //VerifyBase.Talkers = new TerminalTalker[Meters.Count];
            VerifyBase.TerminalChannelType = meterInfos[0].MD_ConnType;
            if (meterInfos[0].MD_Protocol_Type == "376.1")
            {
                VerifyBase.TerminalProtocolType = TerminalProtocolTypeEnum._376;
                EquipmentData.Equipment.TerminalProtocolType = TerminalProtocolTypeEnum._376;
            }
            else
            {
                VerifyBase.TerminalProtocolType = TerminalProtocolTypeEnum._698;
                EquipmentData.Equipment.TerminalProtocolType = TerminalProtocolTypeEnum._698;
            }
            //设置232通讯的数量
            if (meterInfos[0].MD_ConnType == Cus_EmChannelType.Channel232)  //232通讯需要修改最大下线程的数量
            {
                int count = meterInfos.Select(t => t.ProtInfo.Port).Distinct().Count();
                CheckController.MulitThread.SocketThreadManager.Instance.MaxThread = count;
                CheckController.MulitThread.SocketThreadManager.Instance.MaxTaskCountPerThread = meterInfos.Length / count;
                VerifyBase.Conn232Number = count;
                //CheckController.MulitThread.SocketThreadManager.Instance.MaxTaskCountPerThread = 1;
                //CheckController.MulitThread.SocketThreadManager.Instance.MaxThread = meterInfos.Length;
            }
            else
            {
                VerifyBase.Conn232Number = meterInfos.Length;
                CheckController.MulitThread.SocketThreadManager.Instance.MaxTaskCountPerThread = 1;
                CheckController.MulitThread.SocketThreadManager.Instance.MaxThread = meterInfos.Length;
            }

            for (int i = 0; i < Meters.Count; i++)
            {
                if (meterInfos[i].YaoJianYn && !string.IsNullOrWhiteSpace(meterInfos[i].Address) && meterInfos[i].Address != "FFFFFFFF")
                {
                    EquipmentData.TerminalIndexEthernet.Add(meterInfos[i].Address, i);
                }
            }

            return meterInfos;
        }

        public void UpdateCheckFlag()
        {
            for (int i = 0; i < Meters.Count; i++)
            {
                VerifyBase.meterInfo[i].YaoJianYn = YaoJian[i];
            }
        }







        #region 表的唯一编号
        private long longMac = 0;
        /// <summary>
        /// 获取8字节唯一ID：4字节时间戳+3字节主机MAC+1字节自增序列
        /// </summary>
        /// <param name="id">自增序列，只取1字节</param>
        /// <returns>8字节唯一ID</returns>
        private long GetUniquenessID8(int id)
        {
            string strMac = "";
            long lngMac = GetMac(out strMac);

            string s = string.Format("{0:X8}{1:X6}{2:X2}", GetTimeStamp(), ((int)(lngMac)) & 0x00FFFFFF, ((byte)id));
            long n = Convert.ToInt64(s, 16);

            return n;
        }
        /// <summary>
        /// 获取本机MAC地址
        /// </summary>
        /// <param name="MacString">MAC字符串</param>
        /// <returns>MAC值</returns>
        private long GetMac(out string MacString)
        {
            string macAddress = "";
            if (longMac == 0)
            {
                try
                {
                    NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                    foreach (NetworkInterface adapter in nics)
                    {
                        if (!adapter.GetPhysicalAddress().ToString().Equals(""))
                        {
                            macAddress = adapter.GetPhysicalAddress().ToString();
                            longMac = Convert.ToInt64(macAddress, 16);
                            for (int i = 1; i < 6; i++)
                            {
                                macAddress = macAddress.Insert(3 * i - 1, ":");
                            }
                            break;
                        }
                    }

                }
                catch
                {
                }
            }
            MacString = macAddress;
            return longMac;
        }
        /// <summary>
        /// 获得当前时间的4字节时间戳
        /// </summary>
        /// <returns></returns>
        private int GetTimeStamp()
        {
            DateTime timeStamp = new DateTime(1970, 1, 1); //得到1970年的时间戳 
            long a = (DateTime.UtcNow.Ticks - timeStamp.Ticks) / 10000000; //注意这里有时区问题，用now就要减掉8个小时
            int b = (int)a;
            return b;
        }
        /// <summary>
        /// 获取12字节唯一ID：4字节时间戳+4字节主机MAC+2字节进程PID+2字节自增序列
        /// </summary>
        /// <param name="id">自增序列，只取2字节</param>
        /// <returns>12字节唯一ID</returns>
        private long GetUniquenessID12(int id)
        {
            string strMac = "";
            long lngMac = GetMac(out strMac);
            Process curPro = Process.GetCurrentProcess();
            string s = string.Format("{0:X8}{1:X8}{2:X4}{3:X4}", GetTimeStamp(), (int)(lngMac), (short)curPro.Id, ((short)id));
            long n = Convert.ToInt64(s, 16);

            return n;
        }
        #endregion

        /// <summary>
        /// 获取表信息
        /// </summary>
        /// <param name="index">表序号,从0开始</param>
        /// <param name="fieldName">表字段名称</param>
        /// <returns></returns>
        public string GetMeterInfo(int index, string fieldName)
        {
            InputParaUnit paraUnit = ParasModel.AllUnits.FirstOrDefault(item => item.FieldName == fieldName);
            if (paraUnit != null && index >= 0 && index < Meters.Count)
            {
                //string resultTemp = Meters[index].GetProperty(fieldName) as string;

                object obj = Meters[index].GetProperty(fieldName);
                string resultTemp = null;
                if (obj != null)
                    resultTemp = obj.ToString();

                if (paraUnit.ValueType == InputParaUnit.EnumValueType.编码值)
                {
                    resultTemp = CodeDictionary.GetValueLayer2(paraUnit.CodeType, resultTemp);
                }
                return resultTemp;
            }
            else
            { return ""; }
        }
        /// <summary>
        /// 获取表信息
        /// </summary>
        /// <param name="index">表序号,从0开始</param>
        /// <param name="fieldName">表字段名称</param>
        /// <returns></returns>
        public void SetMeterInfo(int index, string fieldName, string value)
        {
            InputParaUnit paraUnit = ParasModel.AllUnits.FirstOrDefault(item => item.FieldName == fieldName);
            if (paraUnit != null && index >= 0 && index < Meters.Count)
            {
                Meters[index].SetProperty(fieldName, value);

            }
        }



        #region 电表信息下载

        /// <summary>
        /// 下载电表信息
        /// </summary>
        public bool Frame_DownMeterInfoFromMis()
        {
            if (DownLoading)
            {
                LogManager.AddMessage(EquipmentData.Equipment.ID + "号下载电表信息!", EnumLogSource.服务器日志, EnumLevel.Information);
                return false;
            }
            try
            {
                DownLoading = true;
                bool down = false;
                LogManager.AddMessage(EquipmentData.Equipment.ID + "号开始下载电表信息!", EnumLogSource.服务器日志, EnumLevel.Information);
                switch (ConfigHelper.Instance.Marketing_Type)
                {
                    case "南瑞_智慧工控平台2.0":
                        down = Do_DownFromNanRuiMeterInfoFromMis();
                        break;
                    case "LY数据服务":
                        down = Do_DownMeterInfoFromDataLY();
                        break;
                    case "智慧工控平台":
                        Do_DownFromGuoJinMeterInfoFromMis();
                        break;
                    default:
                        down = Do_DownFromNanRuiMeterInfoFromMis();
                        break;
                }
                return down;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage("下载电表信息异常!" + ex.ToString(), EnumLogSource.服务器日志, EnumLevel.Error);
                if (ConfigHelper.Instance.Marketing_Type == "LY数据服务" && ConfigHelper.Instance.VerifyModel == "自动模式")
                {
                    InnerCommand.VerifyControl.SendMsg(CtrlCmd.MsgType.故障, ex.ToString());
                }
                return false;
            }
            finally
            {
                DownLoading = false;
            }
        }

        /// <summary>
        /// 获取任务
        /// </summary>
        public void Frame_DownModuleTask()
        {

            LogManager.AddMessage(EquipmentData.Equipment.ID + "获取任务编号!", EnumLogSource.服务器日志, EnumLevel.Information);
            try
            {
                if (ConfigHelper.Instance.Marketing_Type == "南瑞_智慧工控平台2.0")
                {
                    Do_DownFromNanRuiTaskInfoFromMis();
                }
                else if (ConfigHelper.Instance.Marketing_Type == "智慧工控平台")
                {
                    Do_DownFromGuojinTaskInfoFromMis();
                }
            }
            catch (Exception ex)
            {
                LogManager.AddMessage("获取任务编号!" + ex.ToString(), EnumLogSource.服务器日志, EnumLevel.Error);
            }
        }

        private void Do_DownFromNanRuiTaskInfoFromMis()
        {
            IMis mis = MISFactory.Create();
            TestMeterInfo testMeter = null;
            try
            {
                for (int i = 0; i < Meters.Count; i++)
                {
                    if (Meters[i].GetProperty("MD_CHECKED").ToString() == "1")
                    {
                        testMeter = VerifyBase.meterInfo[i];
                        mis.DownTask(Meters[i].GetProperty("MD_BAR_CODE").ToString(), ref EquipmentData.LoadDETECT_TASK);
                        testMeter = VerifyBase.meterInfo[i];
                        Meters[i].SetProperty("MD_TASK_NO", EquipmentData.LoadDETECT_TASK.DETECT_TASK_NO);
                        Meters[i].SetProperty("MD_BATCH_NO", EquipmentData.LoadDETECT_TASK.ARRIVE_BATCH_NO);
                        Meters[i].SetProperty("MD_Other4", EquipmentData.LoadDETECT_TASK.EQUIP_CATEG);//等级
                    }
                }
                OperateFile.WriteINI("LoadDETECT_TASK", "DETECT_TASK_NO", EquipmentData.LoadDETECT_TASK.DETECT_TASK_NO, System.IO.Directory.GetCurrentDirectory() + "\\Ini\\LoadDETECT_TASK.ini");
                LogManager.AddMessage("下载电表信息完成正在录入到本地", EnumLogSource.服务器日志, EnumLevel.Information);
                VerifyBase.meterInfo = GetVerifyMeterInfo();  //把表信息给到检定系统
                VerifyBase.InitTerminalTalks();
                LogManager.AddMessage("录入本地完成", EnumLogSource.服务器日志, EnumLevel.Information);
            }
            catch (Exception ex)
            {
                LogManager.AddMessage("存储电表信息失败", EnumLogSource.服务器日志, EnumLevel.Information);
            }
        }

        private void Do_DownFromGuojinTaskInfoFromMis()
        {
            IMis mis = MISFactory.Create();
            TestMeterInfo testMeter = null;
            try
            {
                LYZD.Mis.IMICP.IMICPTables.RecvData obj = new LYZD.Mis.IMICP.IMICPTables.RecvData();

                string type = "";
                string data = "";
                switch (ConfigHelper.Instance.Marketing_DewnLoadNumber)
                {
                    case "批次号":
                        type = "01";
                        data = meters[0].GetProperty("MD_BATCH_NO").ToString();
                        break;
                    case "条形码":
                        type = "03";
                        for (int i = 0; i < meters.Count; i++)
                        {
                            if (VerifyBase.meterInfo[i].YaoJianYn == true)
                            {
                                data += meters[i].GetProperty("MD_BAR_CODE").ToString().Trim() + ",";
                            }
                        }
                        data = data.Substring(0, data.Length - 1).Trim();
                        break;
                }
                LogManager.AddMessage("下载电表信息完成正在录入到本地", EnumLogSource.服务器日志, EnumLevel.Information);


                VerifyBase.meterInfo = GetVerifyMeterInfo();  //把表信息给到检定系统
                VerifyBase.InitTerminalTalks();
                LogManager.AddMessage("录入本地完成", EnumLogSource.服务器日志, EnumLevel.Information);
            }
            catch (Exception ex)
            {
                LogManager.AddMessage("存储电表信息失败", EnumLogSource.服务器日志, EnumLevel.Information);
            }
        }

        private bool Do_DownFromNanRuiMeterInfoFromMis()
        {
            IMis mis = MISFactory.Create();
            TestMeterInfo testMeter = null;
            try
            {
                for (int i = 0; i < Meters.Count; i++)
                {
                    if (Meters[i].GetProperty("MD_CHECKED").ToString() == "1")
                    {
                        testMeter = VerifyBase.meterInfo[i];
                        if (mis.Down(Meters[i].GetProperty("MD_BAR_CODE").ToString(), ref testMeter))
                        {
                            Meters[i].SetProperty("MD_UB",testMeter.MD_UB.ToString());//电压
                            Meters[i].SetProperty("MD_UA", testMeter.MD_UA);//电流
                            AutoFieldName("MD_UA", testMeter.MD_UA);
                            Meters[i].SetProperty("MD_WIRING_MODE", testMeter.MD_WiringMode);// 测量方式--单相-三相三线-三相四线
                            Meters[i].SetProperty("MD_CONSTANT", testMeter.MD_Constant);
                            Meters[i].SetProperty("MD_FACTORY", testMeter.MD_Factory);//制造厂家
                            AutoFieldName("MD_FACTORY", testMeter.MD_Factory);
                            Meters[i].SetProperty("MD_POSTAL_ADDRESS", null);
                            Meters[i].SetProperty("MD_TERMINAL_TYPE", testMeter.MD_TerminalType);
                            //Meters[i].SetProperty("MD_GRADE", testMeter.MD_Grane);//等级
                            Meters[i].SetProperty("MD_FREQUENCY", testMeter.MD_Frequency.ToString()); //频率
                            AutoFieldName("MD_FREQUENCY", testMeter.MD_Frequency.ToString());
                            Meters[i].SetProperty("MD_TASK_NO", testMeter.MD_TaskNo);//制造厂家
                        }
                        else
                        {
                            LogManager.AddMessage(Meters[i].GetProperty("MD_BAR_CODE").ToString() + "获取不到数据", EnumLogSource.服务器日志, EnumLevel.Warning);
                        }
                    }
                }
                LogManager.AddMessage("下载电表信息完成正在录入到本地", EnumLogSource.服务器日志, EnumLevel.Information);

                VerifyBase.meterInfo = GetVerifyMeterInfo();  //把表信息给到检定系统
                //VerifyBase.InitTerminalTalks();
                LogManager.AddMessage("录入本地完成", EnumLogSource.服务器日志, EnumLevel.Information);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage("存储电表信息失败，异常：" + ex.ToString(), EnumLogSource.服务器日志, EnumLevel.Information);
                return false;
            }
        }

        private bool Do_DownMeterInfoFromDataLY()
        {
            try
            {
                bool down = false;
                TestMeterInfo[] downMeters = new TestMeterInfo[Meters.Count];
                IMis mis = MISFactory.Create();
                if (mis is Mis.LYDataServer.Api api)
                {
                    down = api.Down(ConfigHelper.Instance.EquipmentNo, ref downMeters, out string msg);
                    if (!down) InnerCommand.VerifyControl.SendMsg(CtrlCmd.MsgType.故障, msg);
                }
                
                for (int i = 0; i < Meters.Count; i++)
                {
                    Meters[i].SetProperty("MD_CHECKED", "0");
                }

                foreach (TestMeterInfo item in downMeters)
                {
                    Meters[item.MD_Epitope - 1].SetProperty("MD_CHECKED", "1");
                    Meters[item.MD_Epitope - 1].SetProperty("MD_BAR_CODE", item.MD_BarCode);//条形码
                    Meters[item.MD_Epitope - 1].SetProperty("MD_Epitope", item.MD_Epitope);//条形码
                    Meters[item.MD_Epitope - 1].SetProperty("MD_TASK_NO", item.MD_TaskNo);//任务编号
                    Meters[item.MD_Epitope - 1].SetProperty("MD_GRADE", item.MD_Grane);//等级
                    Meters[item.MD_Epitope - 1].SetProperty("MD_CONSTANT", item.MD_Constant);//常数
                    AutoFieldName("MD_CONSTANT", item.MD_Constant);
                    Meters[item.MD_Epitope - 1].SetProperty("MD_POSTAL_ADDRESS", item.Address);//逻辑地址
                    Meters[item.MD_Epitope - 1].SetProperty("MD_FACTORY", item.MD_Factory);//制造厂家
                    AutoFieldName("MD_FACTORY", item.MD_Factory);
                    Meters[item.MD_Epitope - 1].SetProperty("MD_BATCH_NO", item.MD_BatchNo);//批次号
                    Meters[item.MD_Epitope - 1].SetProperty("MD_UB", item.MD_UB);//电压
                    Meters[item.MD_Epitope - 1].SetProperty("MD_UA", item.MD_UA);//电流
                    AutoFieldName("MD_UA", item.MD_UA);
                    Meters[item.MD_Epitope - 1].SetProperty("MD_FREQUENCY", item.MD_Frequency);//频率
                    Meters[item.MD_Epitope - 1].SetProperty("MD_TEST_TYPE", item.MD_TestType);//鉴定类型
                    Meters[item.MD_Epitope - 1].SetProperty("MD_WIRING_MODE", item.MD_WiringMode);// 测量方式--单相-三相三线-三相四线
                    Meters[item.MD_Epitope - 1].SetProperty("MD_CONNECTION_FLAG", item.MD_ConnectionFlag);//互感器
                    Meters[item.MD_Epitope - 1].SetProperty("MD_TERMINAL_TYPE", item.MD_TerminalType);//终端类型
                    Meters[item.MD_Epitope - 1].SetProperty("MD_CONN_TYPE", "维护口通讯");//通讯方式
                    Meters[item.MD_Epitope - 1].SetProperty("MD_PROTOCOL_TYPE", item.MD_Protocol_Type);//协议类型
                    Meters[item.MD_Epitope - 1].SetProperty("MD_PORT_DATA", "9600,e,8,1");//波特率item.MD_PortData
                    Meters[item.MD_Epitope - 1].SetProperty("MD_FACTORY", item.MD_Factory);//制造厂家
                    AutoFieldName("MD_FACTORY", item.MD_Factory);
                }
                LogManager.AddMessage("下载电表信息完成正在录入到本地", EnumLogSource.服务器日志, EnumLevel.Information);
                VerifyBase.meterInfo = GetVerifyMeterInfo();  //把表信息给到检定系统
                LogManager.AddMessage("录入本地完成", EnumLogSource.服务器日志, EnumLevel.Information);
                return down;
            }
            catch (Exception ex)
            {
                LogManager.AddMessage("存储电表信息失败，异常：" + ex.ToString(), EnumLogSource.服务器日志, EnumLevel.Error);
                return false;
            }


        }

        /// <summary>
        /// 读取参数
        /// </summary>
        private object Locked = new object();
        /// <summary>
        ///从厚达数据库下载数据
        /// </summary>
        private void Do_DownFromHoudaMeterInfoFromMis()
        {

            //Meters[1].SetProperty("MD_METER_TYPE", "112233");//电压
            //初始化所有表位 为 不挂表状态
            for (int i = 0; i < Meters.Count; i++)
            {
                Meters[i].SetProperty("MD_CHECKED", "0");
            }

            IMis mis = MISFactory.Create();
            //从厚达数据库下载数据
            Dictionary<int, TestMeterInfo> meterDic = ((Mis.Houda.Houda)mis).GetMeterModel();
            if (meterDic.Count <= 0)
            {
                LogManager.AddMessage("下载电表信息失败");
                return;
            }

            foreach (int key in meterDic.Keys)
            {
                TestMeterInfo meter = meterDic[key];
                if (meter == null) continue;
                Meters[key - 1].SetProperty("MD_BAR_CODE", meter.MD_BarCode);//条形码
                                                                             //string str=
                Meters[key - 1].SetProperty("MD_UB", meter.MD_UB);//电压
                Meters[key - 1].SetProperty("MD_UA", meter.MD_UA);//电流
                Meters[key - 1].SetProperty("MD_FREQUENCY", 50);//频率
                //Meters[key - 1].SetProperty("MD_TESTMODEL", meter.MD_TestModel);//首检抽检
                //if (meter.MD_TestModel == null || meter.MD_TestModel=="")
                //{  
                //    Meters[key - 1].SetProperty("MD_TESTMODEL", "首检");//首检抽检
                //}

                if (meter.MD_TestType.IndexOf("抽") != -1)
                {
                    Meters[key - 1].SetProperty("MD_TEST_TYPE", "质量抽检");//全检抽检
                }
                else
                {
                    Meters[key - 1].SetProperty("MD_TEST_TYPE", "到货全检");//全检抽检
                }

                Meters[key - 1].SetProperty("MD_WIRING_MODE", meter.MD_WiringMode);// 测量方式--单相-三相三线-三相四线
                Meters[key - 1].SetProperty("MD_CONNECTION_FLAG", meter.MD_ConnectionFlag);//互感器
                //Meters[key - 1].SetProperty("MD_JJGC", meter.MD_JJGC);//检定规程
                //Meters[key - 1].SetProperty("MD_ASSET_NO", meter.MD_AssetNo);//资产编号  /
                //Meters[key - 1].SetProperty("MD_METER_TYPE", meter.MD_MeterType);//表类型 /
                Meters[key - 1].SetProperty("MD_CONSTANT", meter.MD_Constant);//常数
                Meters[key - 1].SetProperty("MD_GRADE", meter.MD_Grane.ToUpper());//等级
                //Meters[key - 1].SetProperty("MD_METER_MODEL", meter.MD_MeterModel);//表型号
                //Meters[key - 1].SetProperty("MD_PROTOCOL_NAME", meter.MD_ProtocolName);//通讯协议
                //Meters[key - 1].SetProperty("MD_CARR_NAME", meter.MD_CarrName);//载波协议
                Meters[key - 1].SetProperty("MD_POSTAL_ADDRESS", meter.Address);//通讯地址00100201
                Meters[key - 1].SetProperty("MD_FACTORY", meter.MD_Factory);//制造厂家
                Meters[key - 1].SetProperty("MD_CUSTOMER", meter.MD_Customer);//送检单位
                Meters[key - 1].SetProperty("MD_TASK_NO", meter.MD_TaskNo);//任务编号
                Meters[key - 1].SetProperty("MD_MADE_NO", meter.MD_MadeNo);//出厂编号
                Meters[key - 1].SetProperty("MD_CERTIFICATE_NO", meter.MD_CertificateNo);//证书编号

                //if (meter.FKType==0)
                //    Meters[key - 1].SetProperty("MD_FKTYPE", "远程费控");//费控类型
                //else if(meter.FKType == 1)
                //    Meters[key - 1].SetProperty("MD_FKTYPE", "本地费控");//费控类型
                //else
                //    Meters[key - 1].SetProperty("MD_FKTYPE","不带费控");//费控类型


                Meters[key - 1].SetProperty("MD_UB", meter.MD_UB);//电压
                //Meters[key - 1].SetProperty("MD_SEAL_1", meter.Seal1);//铅封1
                //Meters[key - 1].SetProperty("MD_SEAL_2", meter.Seal2);//铅封2
                //Meters[key - 1].SetProperty("MD_SEAL_3", meter.Seal3);//铅封3
                //Meters[key - 1].SetProperty("MD_SEAL_4", meter.Seal4);//铅封4
                //Meters[key - 1].SetProperty("MD_SEAL_5", meter.Seal5);//铅封5
                //Meters[key - 1].SetProperty("MD_OTHER_1", meter.Other1);//备用1
                //Meters[key - 1].SetProperty("MD_OTHER_2", meter.Other2);//备用2
                //Meters[key - 1].SetProperty("MD_OTHER_3", meter.Other3);//备用3
                //Meters[key - 1].SetProperty("MD_OTHER_4", meter.Other4);//备用4
                //Meters[key - 1].SetProperty("MD_OTHER_5", meter.Other5);//备用5
                Meters[key - 1].SetProperty("MD_CHECKED", "1");   //设置表位要检定
                Meters[key - 1].SetProperty("MD_OTHER_1", "1");   //设置表位需要上传数据
                Meters[key - 1].SetProperty("MD_OTHER_2", "未上传");   //设置表位需要上传数据


                //temmeter.Other1


            }

            LogManager.AddMessage(EquipmentData.Equipment.ID + "号下载电表信息完毕!", EnumLogSource.数据库存取日志, EnumLevel.Information);

            VerifyBase.meterInfo = GetVerifyMeterInfo();  //把表信息给到检定系统
            //VerifyBase.InitTerminalTalks();
            //if (ConfigHelper.Instance.AreaName == "北京" && VerifyBase.OnMeterInfo.MD_JJGC == "IR46")
            //{
            //    VerifyBase.IsDoubleProtocol = true;  //双协议电表
            //}
            //else
            //{ 
            //    VerifyBase.IsDoubleProtocol = false ;  //双协议电表
            //}
        }

        /// <summary>
        /// 智慧工控平台
        /// </summary>
        private void Do_DownFromGuoJinMeterInfoFromMis()
        {
            IMis mis = MISFactory.Create();
            TestMeterInfo testMeter = null;
            try
            {

                for (int i = 0; i < Meters.Count; i++)
                {
                    if (Meters[i].GetProperty("MD_CHECKED").ToString() == "1")
                    {
                        string strValue = GetMeterInfo(i, "MD_BAR_CODE");
                        testMeter = VerifyBase.meterInfo[i];
                        if (mis.Down(strValue, ref testMeter))
                        {

                            Meters[i].SetProperty("MD_TASK_NO", testMeter.MD_TaskNo); //任务号

                            Meters[i].SetProperty("MD_BATCH_NO", testMeter.MD_BatchNo);//批次号

                            Meters[i].SetProperty("MD_UB", testMeter.MD_UB.ToString()); //电压

                            if (testMeter.MD_UA.Contains("（"))
                            {
                                testMeter.MD_UA = testMeter.MD_UA.Replace("（", "(");
                            }
                            if (testMeter.MD_UA.Contains("）"))
                            {
                                testMeter.MD_UA = testMeter.MD_UA.Replace("）", ")");
                            }
                            Meters[i].SetProperty("MD_UA", testMeter.MD_UA); //电流

                            Meters[i].SetProperty("MD_GRANE", testMeter.MD_Grane); //等级

                            Meters[i].SetProperty("MD_WIRING_MODE", testMeter.MD_WiringMode); //测量方式

                            Meters[i].SetProperty("MD_CONSTANT", testMeter.MD_Constant); //常数
                            Meters[i].SetProperty("MD_FACTORY", testMeter.MD_Factory); //厂家
                            Meters[i].SetProperty("MD_FREQUENCY", testMeter.MD_Frequency.ToString()); //频率
                            Meters[i].SetProperty("MD_TERMINAL_TYPE", testMeter.MD_TerminalType.ToString()); ///// 终端类型--集中器
                            Meters[i].SetProperty("MD_OTHER_5", testMeter.Other5.ToString()); //系统编号
                            Meters[i].SetProperty("MD_OTHER_3", testMeter.Other3.ToString()); //系统编号
                            AutoFieldName("MD_FACTORY", testMeter.MD_Factory);
                            AutoFieldName("MD_UA", testMeter.MD_UA);
                        }
                        else
                        {
                            LogManager.AddMessage(Meters[i].GetProperty("MD_BAR_CODE").ToString() + "获取不到数据", EnumLogSource.服务器日志, EnumLevel.Warning);

                        }
                    }

                    Meters[i].SetProperty("MD_UB", testMeter.MD_UB.ToString()); //电压
                    Meters[i].SetProperty("MD_UA", testMeter.MD_UA); //电流
                }
                LogManager.AddMessage("下载电表信息完成正在录入到本地", EnumLogSource.服务器日志, EnumLevel.Information);

                VerifyBase.meterInfo = GetVerifyMeterInfo();  //把表信息给到检定系统
                LogManager.AddMessage("录入本地完成", EnumLogSource.服务器日志, EnumLevel.Information);
            }
            catch (Exception ex)
            {
                LogManager.AddMessage("存储电表信息失败，异常：" + ex.ToString(), EnumLogSource.服务器日志, EnumLevel.Information);
            }
        }

        #endregion

        /// <summary>
        /// 自动添加编码
        /// </summary>
        /// <param name="FiledName">编码对应的数据库列名</param>
        /// <param name="FiledValue">添加的新值</param>
        public void AutoFieldName(string FiledName, string FiledValue)
        {
            //1:判断是否有这个列
            InputParaUnit unitTemp = ParasModel.AllUnits.FirstOrDefault(item => item.FieldName == FiledName);
            if (unitTemp != null)
            {
                //找到这个列对应的节点
                CodeTree.CodeTreeNode nodeTemp = CodeTree.CodeTreeViewModel.Instance.GetCodeByEnName(unitTemp.CodeType, 2);
                if (nodeTemp != null)
                {
                    string name = FiledValue;//这里就是要添加的新值
                    CodeTree.CodeTreeNode nodeTemp1 = nodeTemp.Children.FirstOrDefault(item => item.CODE_NAME == name);
                    if (nodeTemp1 == null) //没有这个节点的话
                    {
                        //编码的话先获取最大的长度
                        //然后从长度那位数开始循环，判断那个没有用过
                        //nodeTemp.Children.
                        List<string> list = new List<string>();
                        var qry = from s in nodeTemp.Children
                                  orderby s.CODE_VALUE.Length descending
                                  select s;
                        int len = qry.First().CODE_VALUE.Length - 1;

                        int index = (int)Math.Pow(10, len);
                        int end = (int)Math.Pow(10, len + 1);
                        string value = "999";
                        for (int i = index; i < end; i++)
                        {
                            if (nodeTemp.Children.FirstOrDefault(item => item.CODE_VALUE == i.ToString()) == null)
                            {
                                value = i.ToString();
                                break;
                            }
                        }
                        nodeTemp.Children.Add(new CodeTree.CodeTreeNode()
                        {
                            CODE_LEVEL = nodeTemp.CODE_LEVEL + 1,
                            CODE_PARENT = nodeTemp.CODE_TYPE,
                            CODE_NAME = name,
                            CODE_VALUE = value,
                            CODE_ENABLED = true,
                            FlagChanged = true
                        });
                        nodeTemp.SaveCode();
                    }

                }
            }
        }

        #region 方案下载

        /// <summary>
        /// 下载方案
        /// </summary>
        public bool Frame_DownSchemeMis()
        {

            //不开线程下载，下载完成了再执行 
            LogManager.AddMessage("开始下载方案");
            if (ConfigHelper.Instance.Marketing_Type == "智慧计量工控平台")
            {
                Do_DownSchemeInfoFromMisGuoJin();
                return true;
            }
            else
            {
                return Do_DownSchemeInfoFromMis();
            }

        }
        /// <summary>
        /// 下载方案信息
        /// </summary>
        /// <param name="obj"></param>
        private bool Do_DownSchemeInfoFromMis()
        {
            try
            {
                bool bOK = false;
                TestMeterInfo meter = VerifyBase.OnMeterInfo;
                if (meter == null)
                {
                    LogManager.AddMessage("没有要检表信息，下载方案信息失败");
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(meter.MD_BarCode))
                {
                    string schemeName = "";
                    Dictionary<string, SchemaNode> Schema = null;
                    IMis mis = MISFactory.Create();
                    if (mis is Mis.LYDataServer.Api api)
                    {
                        bOK = api.SchemeDown(meter, out schemeName, out Schema, out string msg);
                    }
                    if (bOK)
                    {
                        DynamicViewModel modelTemp = EquipmentData.SchemaModels.Schemas.FirstOrDefault(item => item.GetProperty("SCHEMA_NAME") as string == schemeName);
                        if (modelTemp != null)   //说明本地已经有这个方案了，切换方案就好
                        {
                            EquipmentData.SchemaModels.SelectedSchema = modelTemp;
                        }
                        else  //没有找到方案，进行创建方案
                        {
                            EquipmentData.SchemaModels.NewName = schemeName;
                            EquipmentData.SchemaModels.AddDownSchema(); //这个需要重写一个方法，刷新方案放在背后，并且把选中等方法添加到里面
                            modelTemp = EquipmentData.SchemaModels.Schemas.FirstOrDefault(item => item.GetProperty("SCHEMA_NAME") as string == EquipmentData.SchemaModels.NewName);
                            EquipmentData.SchemaModels.SelectedSchema = modelTemp;
                            System.Threading.Thread.Sleep(200);
                            foreach (var key in Schema.Keys)
                            {
                                if (!EquipmentData.Schema.ExistNode(key))
                                {
                                    SchemaNodeViewModel nodeNew = EquipmentData.Schema.AddParaNode(key);//根据方案的编号，添加进了节点
                                    EquipmentData.Schema.ParaValuesView.Clear();//删除默认值的方案
                                }
                                List<string> propertyNames = new List<string>();

                                for (int j = 0; j < EquipmentData.Schema.ParaInfo.CheckParas.Count; j++)
                                {
                                    propertyNames.Add(EquipmentData.Schema.ParaInfo.CheckParas[j].ParaDisplayName);
                                }

                                for (int i = 0; i < Schema[key].SchemaNodeValue.Count; i++)
                                {
                                    DynamicViewModel viewModel2 = new DynamicViewModel(propertyNames, 0);
                                    viewModel2.SetProperty("IsSelected", true);
                                    string[] value = Schema[key].SchemaNodeValue[i].Split('|');
                                    for (int j = 0; j < propertyNames.Count; j++)
                                    {
                                        viewModel2.SetProperty(propertyNames[j], value[j]); //这里改成参数的值
                                    }
                                    EquipmentData.Schema.ParaValuesView.Add(viewModel2);
                                }
                                EquipmentData.Schema.SelectedNode.ParaValuesCurrent = EquipmentData.Schema.ParaValuesConvertBack();
                                //EquipmentData.Schema.SaveDownParaValue();    //保存方案
                                EquipmentData.Schema.RefreshPointCount();
                            }
                            EquipmentData.Schema.SaveParaValue();    //保存方案
                            EquipmentData.SchemaModels.RefreshCurrrentSchema();
                        }
                        return true;
                    }
                    else
                    {
                        InnerCommand.VerifyControl.SendMsg("下载方案信息失败");
                        return false;
                    }
                }
                else
                {
                    InnerCommand.VerifyControl.SendMsg("空条码不下载方案");
                    return false;
                }
            }
            catch (Exception ex)
            {

                InnerCommand.VerifyControl.SendMsg($"下载方案异常，{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 工控平台下载信息
        /// </summary>
        private void Do_DownSchemeInfoFromMisGuoJin()
        {
            bool bOK = false;
            TestMeterInfo meter = VerifyBase.OnMeterInfo;
            if (meter == null)
            {
                LogManager.AddMessage("没有要检表信息，下载方案信息失败");
                return;
            }
            string ip = ConfigHelper.Instance.Produce_IP;
            int port = int.Parse(ConfigHelper.Instance.Produce_Prot);
            string dataSource = ConfigHelper.Instance.Produce_DataSource;
            string userId = ConfigHelper.Instance.Produce_UserName;
            string pwd = ConfigHelper.Instance.Produce_UserPassWord; ;
            string url = ConfigHelper.Instance.Marketing_WebService;
            IMis mis = MISFactory.Create();
            string schemeName = "";
            Dictionary<string, SchemaNode> Schema = null;
            if (!string.IsNullOrWhiteSpace(meter.MD_SchemeID))
            {
                bOK = mis.SchemeDown(meter.MD_SchemeID, out schemeName, out Schema);
            }

            if (bOK)
            {
            //    DynamicViewModel modelTemp = EquipmentData.SchemaModels.Schemas.FirstOrDefault(item => item.GetProperty("SCHEMA_NAME") as string == schemeName);
            //    if (modelTemp != null)   //说明本地已经有这个方案了，切换方案就好
            //    {
            //        EquipmentData.SchemaModels.SelectedSchema = modelTemp;
            //    }
            //    else  //没有找到方案，进行创建方案
            //    {
            //        EquipmentData.SchemaModels.NewName = schemeName;
            //        EquipmentData.SchemaModels.AddDownSchema(); //这个需要重写一个方法，刷新方案放在背后，并且把选中等方法添加到里面
            //        modelTemp = EquipmentData.SchemaModels.Schemas.FirstOrDefault(item => item.GetProperty("SCHEMA_NAME") as string == EquipmentData.SchemaModels.NewName);
            //        EquipmentData.SchemaModels.SelectedSchema = modelTemp;
            //        System.Threading.Thread.Sleep(200);
            //        foreach (var key in Schema.Keys)
            //        {
            //            if (!EquipmentData.Schema.ExistNode(key))
            //            {
            //                SchemaNodeViewModel nodeNew = EquipmentData.Schema.AddParaNode(key);//根据方案的编号，添加进了节点
            //                EquipmentData.Schema.ParaValuesView.Clear();//删除默认值的方案
            //            }
            //            List<string> propertyNames = new List<string>();

            //            for (int j = 0; j < EquipmentData.Schema.ParaInfo.CheckParas.Count; j++)
            //            {
            //                propertyNames.Add(EquipmentData.Schema.ParaInfo.CheckParas[j].ParaDisplayName);
            //            }

            //            for (int i = 0; i < Schema[key].SchemaNodeValue.Count; i++)
            //            {
            //                DynamicViewModel viewModel2 = new DynamicViewModel(propertyNames, 0);
            //                viewModel2.SetProperty("IsSelected", true);
            //                string[] value = Schema[key].SchemaNodeValue[i].Split('|');
            //                for (int j = 0; j < propertyNames.Count; j++)
            //                {
            //                    viewModel2.SetProperty(propertyNames[j], value[j]); //这里改成参数的值
            //                }
            //                EquipmentData.Schema.ParaValuesView.Add(viewModel2);
            //            }
            //            EquipmentData.Schema.SelectedNode.ParaValuesCurrent = EquipmentData.Schema.ParaValuesConvertBack();
            //            //EquipmentData.Schema.SaveDownParaValue();    //保存方案
            //            EquipmentData.Schema.RefreshPointCount();
            //        }
            //        EquipmentData.Schema.SaveParaValue();    //保存方案
            //        EquipmentData.SchemaModels.RefreshCurrrentSchema();
            //    }
            }
        }
        #endregion


        #region 电表地址
        bool IsReadMeterAddres = false;
        /// <summary>
        /// 读取电表地址
        /// </summary>
        public void Read_Meter_Addres()
        {
            if (IsReadMeterAddres) return;
            System.Threading.Tasks.Task.Factory.StartNew(() => IsReadAddress());
        }


        public void AnalyzeBarcodeGetAddress()
        {
            if (IsReadMeterAddres) return;

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                IsReadMeterAddres = true;
                for (int i = 0; i < Meters.Count; i++)
                {
                    if (Meters[i].GetProperty("MD_CHECKED").ToString() == "1")
                    {
                        string MD_BAR_CODE = Meters[i].GetProperty("MD_BAR_CODE").ToString();
                        if (!string.IsNullOrWhiteSpace(MD_BAR_CODE) && MD_BAR_CODE.Length >= 22)
                        {
                            //MD_POSTAL_ADDRESS
                            Meters[i].SetProperty("MD_POSTAL_ADDRESS", MD_BAR_CODE.Substring(9, 12));
                        }
                    }
                }
                IsReadMeterAddres = false;
            });
        }

        /// <summary>
        /// 探测地址的
        /// </summary>
        /// <param name="meterInfos"></param>
        public void IsReadAddress()
        {
            DeviceControlS device = new DeviceControlS();
            int index = -1;
            for (int i = 0; i < meters.Count; i++)
            {
                if (YaoJian[i])
                {
                    index = i;
                    break;
                }
            }
            if (index == -1)
            {
                LogManager.AddMessage("没有要检的表", EnumLogSource.设备操作日志, EnumLevel.Warning);
                return;
            }
            VerifyBase.meterInfo = GetVerifyMeterInfo();  //把表信息给到检定系统
            LogManager.AddMessage("开始升源", EnumLogSource.设备操作日志, EnumLevel.Information);
            Cus_PowerYuanJian ele = Cus_PowerYuanJian.H;
            if (GetMeterInfo(index, "MD_WIRING_MODE") == "单相") ele = Cus_PowerYuanJian.A;
            float ub = float.Parse(GetMeterInfo(index, "MD_UB"));
            device.PowerOn(ub, ub, ub, 0, 0, 0, ele, PowerWay.正向有功, "1.0"); //升个电压
            System.Threading.Thread.Sleep(10000);
            LogManager.AddMessage("正在读取电表地址", EnumLogSource.设备操作日志, EnumLevel.Information);
            EquipmentData.Controller.UpdateMeterProtocol();
            IsReadMeterAddres = true;
            try
            {
                byte[][] setData = new byte[Meters.Count][];
                int[] TalkResult;
                Dictionary<int, string[]> RecData = new Dictionary<int, string[]>();
                for (int i = 0; i < Meters.Count; i++)
                {
                    if (YaoJian[i])
                    {
                        setData[i] = VerifyBase.Talkers[i].Framer698.ReadData_address("40010200");
                    }
                }
                TalkResult = TerminalProtocalAdapter698.Instance.WriteData(setData, RecData, 5000);
                for (int i = 0; i < Meters.Count; i++)
                {
                    if (YaoJian[i])
                    {
                        if (TalkResult[i] == 0)
                        {
                            if (RecData[i].Length >= 8)
                            {
                                string add = GetData(RecData, i, 5, EnumTerimalDataType.e_string);
                                Meters[i].SetProperty("MD_POSTAL_ADDRESS", add);//通讯地址
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
            IsReadMeterAddres = false;
            device.PowerOff();//关源
            LogManager.AddMessage("地址读取完成", EnumLogSource.设备操作日志, EnumLevel.Information);
        }

        private string GetData(Dictionary<int, string[]> RecData, int index, int serinalNumber, EnumTerimalDataType e_type)
        {
            try
            {
                string strReturn = "";
                strReturn = RecData[index][serinalNumber].Split('：')[1];
                switch (e_type)
                {
                    case EnumTerimalDataType.e_byte:
                        return Convert.ToByte(strReturn).ToString();
                    case EnumTerimalDataType.e_int:
                        return Convert.ToInt32(strReturn).ToString();
                    case EnumTerimalDataType.e_float:
                        return Convert.ToSingle(strReturn).ToString();
                    case EnumTerimalDataType.e_double:
                        return Convert.ToDouble(strReturn).ToString();
                    case EnumTerimalDataType.e_datetime:
                        return Convert.ToDateTime(strReturn).ToString();
                    case EnumTerimalDataType.e_bs8:
                        if (strReturn.Length == 8)
                            return strReturn;
                        else
                            return "99999999";
                    case EnumTerimalDataType.e_bs16:
                        if (strReturn.Length == 16)
                            return strReturn;
                        else
                            return "9999999999999999";
                    case EnumTerimalDataType.e_string:
                        return strReturn;
                    default:
                        return "99999";
                }
            }
            catch
            {
                switch (e_type)
                {
                    case EnumTerimalDataType.e_byte:
                        return "99";
                    case EnumTerimalDataType.e_int:
                        return "99999";
                    case EnumTerimalDataType.e_float:
                        return "99999.9";
                    case EnumTerimalDataType.e_double:
                        return "99999.99";
                    case EnumTerimalDataType.e_datetime:
                        return "2000-1-1";
                    case EnumTerimalDataType.e_bs8:
                        return "99999999";
                    case EnumTerimalDataType.e_bs16:
                        return "9999999999999999";
                    case EnumTerimalDataType.e_string:
                        return "99999";
                    default:
                        return "99999";
                }
            }
        }
        #endregion

    }
}
