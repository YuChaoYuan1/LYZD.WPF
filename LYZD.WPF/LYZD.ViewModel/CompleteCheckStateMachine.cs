using LYZD.Core.Model.Meter;
using LYZD.DAL.Config;
using LYZD.Mis;
using LYZD.Mis.Common;
using LYZD.Utility.Log;
using LYZD.ViewModel.CheckController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.ViewModel
{
    /// 检定结束状态机 
    /// <summary>
    /// 检定结束状态机
    /// </summary>
    public class CompleteCheckStateMachine
    {
        #region 供外部调用
        /// 打开状态机，在此内部修改业务流程
        /// <summary>
        /// 打开状态机，在此内部修改业务流程
        /// </summary>
        /// <param name="isAuto">是否自动完成任务上报各项步骤</param>
        /// <param name="stateValue">当前状态</param>
        public void Start(bool isAuto, EnumStepCompleteCheck stateValue)
        {
            //置忙标志
            IsBusy = true;
            //执行当前状态的方法
            CurrentStep = stateValue;
            AutoFlag = isAuto;
            //判断是否自动执行
            while (AutoFlag)
            {
                if (result)
                {
                    //空闲时不执行
                    if (CurrentStep == EnumStepCompleteCheck.Idle)
                    {
                        break;
                    }
                    //如果不是最后一步，执行下一步的动作
                    else if (CurrentStep != EnumStepCompleteCheck.NotifyCheckFinished)
                    {
                        CurrentStep = (EnumStepCompleteCheck)((int)(CurrentStep + 1));
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    //如果表没有在上位
                    //通知集控软件手动上传检定完成通知，跳转到参数录入界面
                    if (CurrentStep == EnumStepCompleteCheck.MeterUp)
                    {
                        //不上报检定完成直接转到参数录入界面
                        //在集控软件端通知调度中心摘表完成
                        currentStep = EnumStepCompleteCheck.NotifyCheckFinished;
                        //触发完成事件
                        if (EventStepFinished != null)
                        {
                            EventStepFinished(false, "由于表没有全部抬起，没有通知调度中心检定完成，请手动调用试验完成接口！");
                        }
                        break;
                    }
                    //如果执行失败就退出状态机
                    else
                    {
                        break;
                    }
                }
            }
            //清忙标志
            IsBusy = false;
        }
        /// 完成每一步动作的事件
        /// <summary>
        /// 成每一步动作的事件
        /// </summary>
        public event DelegateStepFinished EventStepFinished;
        /// 是否自动完成步骤
        /// <summary>
        /// 是否自动完成步骤
        /// </summary>
        public bool AutoFlag { get; private set; }
        private EnumStepCompleteCheck currentStep = EnumStepCompleteCheck.Idle;
        /// 当前步骤,对外只读
        /// <summary>
        /// 当前步骤,对外只读
        /// </summary>
        public EnumStepCompleteCheck CurrentStep
        {
            get { return currentStep; }
            private set
            {
                currentStep = value;
                StateFactory();
            }
        }
        /// 状态机工作中
        /// <summary>
        /// 状态机工作中
        /// </summary>
        public bool IsBusy { get; private set; }
        #endregion

        #region 内部逻辑
        /// 执行当前状态方法的结果
        /// <summary>
        /// 执行当前状态方法的结果
        /// </summary>
        private bool result = false;
        /// 执行当前方法显示的消息
        /// <summary>
        /// 执行当前方法显示的消息
        /// </summary>
        private string message = "";
        /// 临时创建的委托
        /// <summary>
        /// 临时创建的委托
        /// </summary>
        /// <param name="executeResult"></param>
        /// <param name="message"></param>
        public delegate void DelegateStepFinished(bool executeResult, string message);
        /// 工厂方法，定义各个步骤要执行的动作
        /// <summary>
        /// 工厂方法，定义各个步骤要执行的动作
        /// </summary>
        private void StateFactory()
        {
            switch (CurrentStep)
            {
                case EnumStepCompleteCheck.Idle:
                    result = true;
                    message = "";
                    break;
                case EnumStepCompleteCheck.TransportTempToDB:
                    ProcessTransportTempToDB();
                    break;
                case EnumStepCompleteCheck.UpdateToThirdPart:
                    ProcessUpdateToThirdPart();
                    break;
                case EnumStepCompleteCheck.NotifyUpdateDataFinished:
                    ProcessNotifyUpdateDataFinished();
                    break;
                case EnumStepCompleteCheck.MeterUp:
                   // ProcessMeterUp();
                    break;
                case EnumStepCompleteCheck.NotifyCheckFinished:
                    ProcessNotifyCheckFinished();
                    break;
            }

            //写日志信息
            ProcessMessage();

            //触发当前步骤完成事件
            if (EventStepFinished != null)
            {
                EventStepFinished(result, message);
            }
        }
        /// 将日志写入数据库
        /// <summary>
        /// 将日志写入数据库
        /// </summary>
        private void ProcessMessage()
        {
            //if (result)
            //{
            //    ZH.Dispatcher.DispatcherManager.Instance.Parms.In_RunningLogType = 0;
            //    ZH.Dispatcher.DispatcherManager.Instance.Parms.In_RunningLogStrMsg = message;
            //    ZH.Dispatcher.DispatcherManager.Instance.Excute(ZH.Dispatcher.DispatcherEnum.WriteRunningLog);
            //}
            //else
            //{
            //    ZH.Dispatcher.DispatcherManager.Instance.Parms.In_RunningLogType = 2;
            //    ZH.Dispatcher.DispatcherManager.Instance.Parms.In_RunningLogStrMsg = message;
            //    ZH.Dispatcher.DispatcherManager.Instance.Excute(ZH.Dispatcher.DispatcherEnum.WriteRunningLog);
            //}
        }
        /// 将表抬起
        /// <summary>
        /// 将表抬起
        /// </summary>
        private void ProcessMeterUp()
        {
            //App.EnableReadWcb = false;

            //System.Threading.Thread.Sleep(2000);//等待2秒至读误差板线程没有影响以后再操作
            //                                    // 查询是否所有的表都抬起
            //for (int retryTimes = 0; retryTimes < 3; retryTimes++)
            //{
            //    // 将表抬起
            //    MotorSafeControl.Instance.Control("挂表位", false);
            //    //等待15秒
            //    System.Threading.Thread.Sleep(15000);

            //    string[] states = EquipHelper.Instance.GetBWStatus();

            //    bool[] rpMeter = new bool[App.CUS.Meters.Count];
            //    for (int i = 0; i < rpMeter.Length; i++)
            //    {
            //        if (states[i].IndexOf("011") < 0 && states[i].IndexOf("010") < 0)
            //        {
            //            rpMeter[i] = true;
            //        }
            //    }
            //    if (Array.IndexOf(rpMeter, true) >= 0)
            //    {
            //        if (retryTimes == 2)
            //        {
            //            result = false;
            //            message = "有表位不到上限位，不能继续，请手动处理!";
            //        }
            //    }
            //    else
            //    {
            //        result = true;
            //        message = "所有表位都在上位，可以进行下一步操作！";
            //        break;
            //    }

            //    System.Threading.Thread.Sleep(2000);
            //}
        }
        /// 临时库数据转移到正式库
        /// <summary>
        /// 临时库数据转移到正式库
        /// </summary>
        private void ProcessTransportTempToDB()
        {
            ////更新临时库中的表信息
            //App.CUS.SaveTempDB();//临时库

            //string sqlError = "";
            //string accessError = "";
            //int saveResult = App.CUS.TransportTempDBToDB(out accessError, out sqlError);
            //if (saveResult == 0)
            //{
            //    result = true;
            //    message = "临时库数据转移到正式数据库成功！";
            //}
            //else
            //{
            //    result = false;
            //    if (saveResult == 1)
            //    {
            //        message = string.Format("Access数据库临时数据转移失败:{0}", accessError);
            //    }
            //    else if (saveResult == 2)
            //    {
            //        message = string.Format("本地Sql数据库保存失败:{0}", accessError);
            //    }
            //    else if (saveResult == 3)
            //    {
            //        message = string.Format("Access数据库临时数据转移失败:{0}\r\n", accessError);
            //        message += string.Format("本地Sql数据库保存失败:{0}", accessError);
            //    }
            //}
        }

        /// <summary>
        /// 将数据上传给第三方
        /// </summary>
        private void ProcessUpdateToThirdPart()
        {
            #region 创建数据库连接
            //Mis.DataHelper.DataManage DataManage = Mis.DataHelper.DataManage(false);
            string sqlIP = ConfigHelper.Instance.Marketing_IP;
            string sqlUserName = ConfigHelper.Instance.Marketing_UserName;
            string sqlPassWord = ConfigHelper.Instance.Marketing_UserPassWord;
            //Mis.DataHelper.DataManage DataManageServer = new Mis.DataHelper.DataManage(sqlIP, sqlUserName, sqlPassWord);
            #endregion

            #region 上传检定结论到第三方服务器
            //上传结论成功的表位数量
            int iUpdateOkSum = 0;
            int iUpdateFailSum = 0;
            IMis mis = MISFactory.Create();
            mis.UpdateInit();
            foreach (TestMeterInfo meter in VerifyBase.meterInfo)
            {
                #region 上传一块表的记录到数据库
                if (!meter.YaoJianYn)
                {
                    continue;
                }
                string strMessage = "";
                bool bUpdateOk = mis.Update(meter);

                //bool bUpdateOk = ZH.Mis.MisData.MisDataHelper.UpdateMeterInfoToMis(meter, "", out strMessage);
                if (!bUpdateOk)
                {
                    LogManager.AddMessage(string.Format("上传到生产调度中间库失败，条形码{0}:{1}", meter.MD_BarCode, strMessage, EnumLogSource.数据库存取日志, EnumLevel.Warning));

                    iUpdateFailSum++;
                    continue;
                }
                else
                {
                    iUpdateOkSum++;
                }
                //DataManage.UpdateToMisOk(meter.Meter_ID, meter.BenthNo);
                LogManager.AddMessage(string.Format("电能表{0}检定记录上传成功!", meter.MD_BarCode));
                #endregion
            }
            mis.UpdateCompleted();
            #endregion

            if (iUpdateFailSum > 0)
            {
                result = false;
                message = string.Format("检定结论上传到生产调度中心失败，未上报的电能表数量：{0}", iUpdateFailSum);
            }
            else
            {
                result = true;
            }
        }
        /// 通知数据上传完成
        /// <summary>
        /// 通知数据上传完成
        /// </summary>
        private void ProcessNotifyUpdateDataFinished()
        {
            //单相台系统编号为201
            string strXml = string.Format("<PARA><SYS_NO>201</SYS_NO><TASK_NO>{0}</TASK_NO></PARA>", "App.CUS.TaskNo");
            object result = WebServiceHelper.InvokeWebService(ConfigHelper.Instance.Marketing_WebService, "SingleService", "setDETedTestData", new string[1] { strXml });
            #region webservice应答处理
            if (WebServiceHelper.GetResultByXml(result.ToString()))
            {
                result = true;
                message = string.Format("通知调度中心更新检定结论完成:{0}！", strXml);
            }
            else
            {
                result = false;
                message = string.Format("通知调度中心更新检定结论失败：{0}", result.ToString());
            }
            #endregion

        }

        /// 通知检定完成
        /// <summary>
        /// 通知检定完成
        /// </summary>
        private void ProcessNotifyCheckFinished()
        {
            string strXml = string.Format("<PARA><SYS_NO>201</SYS_NO><EQUIP_NO>{0}</EQUIP_NO><TASK_NO>{1}</TASK_NO></PARA>", EquipmentData.Equipment.ID.PadLeft(3, '0'), "App.CUS.TaskNo");
            object result = WebServiceHelper.InvokeWebService(ConfigHelper.Instance.Marketing_WebService, "SingleService", "setTestFinished", new string[1] { strXml });
            #region webservice应答处理
            if (WebServiceHelper.GetResultByXml(result.ToString()))
            {
                result = true;
                message = string.Format("检定完成,通知调度中心机器人摘表:{0}！", strXml);
            }
            else
            {
                result = false;
                message = string.Format("检定完成,通知调度中心机器人摘表失败：{0}", result.ToString());
            }
            #endregion
        }
        #endregion
    }
    /// 结束检定的状态
    /// <summary>
    /// 结束检定的状态
    /// </summary>
    public enum EnumStepCompleteCheck
    {
        /// 空闲
        /// <summary>
        /// 空闲
        /// </summary>
        Idle = 0,
        /// 将临时库数据保存到正式库
        /// <summary>
        /// 将临时库数据保存到正式库
        /// </summary>
        TransportTempToDB = 1,
        /// 将数据保存到第三方用户数据库
        /// <summary>
        /// 将数据保存到第三方用户数据库
        /// </summary>
        UpdateToThirdPart = 2,
        /// 通知上传数据完成
        /// <summary>
        /// 通知上传数据完成
        /// </summary>
        NotifyUpdateDataFinished = 3,
        /// 将表抬起
        /// <summary>
        /// 将表抬起
        /// </summary>
        MeterUp = 4,
        /// 通知检定完成
        /// <summary>
        /// 通知检定完成
        /// </summary>
        NotifyCheckFinished = 5
    }

}
