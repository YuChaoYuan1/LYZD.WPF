using LYTest.Mis.IMICP;
using LYZD.DAL;
using LYZD.DAL.Config;
using LYZD.Utility.Log;
using LYZD.ViewModel.CheckController;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using static LYZD.Mis.IMICP.IMICPMis;

namespace LYZD.ViewModel.IMICP
{
    class OpenPortIMICP : EquipmentData
    {
        private static int 上报频率 = int.Parse(ConfigHelper.Instance.OperatingConditionsUpdataF);
        private static string 上报Url = string.Format("tcp://{0}:{1}", ConfigHelper.Instance.OperatingConditionsIp.Trim(), ConfigHelper.Instance.OperatingConditionsProt.Trim()).Replace(" ", "");
        public void openFWQ()
        {
            StateApi();
        }

        public static void ReceData(string Tstr)
        {
            string str = Tstr.Substring(0, 4);
            if (str == "12.8")
            {
                string table = Tstr.Substring(4);
                ShanXiTable recv = JsonHelper.反序列化字符串<ShanXiTable>(table);
                上报频率 = int.Parse(recv.frequency);

                //查询之前的配置信息
                DynamicModel model = DALManager.ApplicationDbDal.GetByID(EnumAppDbTable.T_CONFIG_PARA_VALUE.ToString(), string.Format("CONFIG_NO = '{0}'", "05002"));
                if (model != null)
                {
                    string[] strData = model.GetProperty("CONFIG_VALUE").ToString().Split('|');
                    strData[3] = 上报频率.ToString();

                    model.SetProperty("CONFIG_NO", model.GetProperty("CONFIG_NO").ToString());
                    model.SetProperty("CONFIG_VALUE", String.Join("|", strData));
                    DALManager.ApplicationDbDal.Update(EnumAppDbTable.T_CONFIG_PARA_VALUE.ToString(), string.Format("ID={0}", 49), model, new List<string> { "CONFIG_NAME", "CONFIG_VALUE" });
                }
                LogManager.AddMessage(string.Format("上报频率改变为{0}", 上报频率), 3);
            }
        }

        #region 服务器

        string ip = "";
        int port = 44309;
        private HttpServer httpServer = null;

        public string SelectIp()
        {
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();

                    string str = (ConfigHelper.Instance.OperatingConditionsIp).ToString().Substring(0, 6).ToString();
                    if (AddressIP.Contains("10.212"))
                    {
                        break;
                    }
                }
            }
            return AddressIP;
        }
        ///开启服务器
        public async void StateApi()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ConfigHelper.Instance.DevControllServerPort))
                {
                    port = Convert.ToInt32(ConfigHelper.Instance.DevControllServerPort);
                }
                ip = SelectIp();
                httpServer = new HttpServer(ip, port);
                await httpServer.StartHttpServer();
                LogManager.AddMessage("服务器开启成功IP：" + ip + "端口：" + port, EnumLogSource.服务器日志, EnumLevel.Information);
               
            }
            catch (Exception ex)
            {
                LogManager.AddMessage("服务器开启失败" + ex.Message, EnumLogSource.服务器日志, EnumLevel.Information);
                LogManager.AddMessage(string.Format("服务器开启失败：{0}  ", ex.Message), EnumLogSource.服务器日志, EnumLevel.Information);
            }
        }

        /// <summary>
        /// 停止服务器
        /// </summary>
        public async void EndApi()
        {
            try
            {
                ip = SelectIp();
                httpServer = new HttpServer(ip, port);
                await httpServer.CloseHttpServer();

            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region 工况数据上传




        ///// <summary>
        ///// 设备事件上报接口  button11_Click
        ///// </summary>
        ///// <param name="plantEventType">事件类型</param>
        ///// <param name="veriltemParaNo">试验项编号</param>
        //public void EventEscalation(string plantEventType, string veriltemParaNo)
        //{
        //    //判断任务号是否是一个
        //    for (int i = 0; i < EquipmentData.MeterGroupInfo.Meters.Count; i++)
        //    {

        //    }
        //    taskno = EquipmentData.MeterGroupInfo.Meters[0].GetProperty("MD_TASK_NO").ToString();  ///任务号
        //    //获取电表任务信息
        //    //0101:开机；
        //    //0102:关机；
        //    //0103:开始预热；
        //    //0104:结束预热；
        //    //0105:任务开始；
        //    //0106:任务结束；
        //    //0107:暂停；
        //    //0108:暂停结束;
        //    //0109:开始检修；
        //    //0110:结束检修；
        //    //0111:异常停机；
        //    //0112:异常停机结束；
        //    //0119:开始试验项；
        //    //0120:结束试验项；

        //    string eventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        //    string devSeatStatusList = null;

        //    string topic = "GK_JD_PLAN_EVENT_TOPIC";
        //    string topicName = "设备事件上报接口";

        //    JObject joSend = new JObject();
        //    joSend.Add("plantNo", plantNo);//设备编号
        //    joSend.Add("taskNo", taskno);//任务编号
        //    joSend.Add("plantEventType", plantEventType);//事件类型
        //    joSend.Add("eventTime", eventTime);//事件发生时间  
        //    joSend.Add("veriltemParaNo", veriltemParaNo);//试验项项参数编码  //项目编号
        //    joSend.Add("devSeatStatusList", devSeatStatusList);//事件类型   

        //    string jsonSend = joSend.ToString();

        //    bool ret = KafkaClient.SendToKafka(topic, jsonSend, 上报Url);
        //    if (ret)
        //    {
        //        LogManager.AddMessage(string.Format("13.2---上报数据成功 Url:{0},topic:{1},接口描述:{2},上报数据Json内容:\r\n{3}", 上报Url, topic, topicName, jsonSend));
        //    }
        //    else
        //    {

        //    }
        //}


        ///// <summary>
        ///// 13.4   完成
        ///// </summary>
        ///// <param name="curWkStatusCode">当前状态 </param>
        ///// <param name="previousState">上一状态</param>
        //public void WorkingStatus(string curWkStatusCode, string previousState)
        //{

        //    //01:预热；02:空闲；03:离线；04:运行；05:告警；06:自校准
        //    string startTime = DateTime.Now.AddSeconds(-2).ToString("yyyy-MM-dd HH:mm:ss");

        //    string receiveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


        //    string topic = "GK_JD_PLAN_WORKING_STATUS_TOPIC";
        //    string topicName = "设备工作状态信息上报接口";

        //    JObject joSend = new JObject();
        //    joSend.Add("plantNo", plantNo);//设备编号
        //    joSend.Add("curWkStatusCode", curWkStatusCode);//当前状态
        //    joSend.Add("startTime", startTime);//当前状态开始时间
        //    joSend.Add("previousState", previousState);//上一状态
        //    joSend.Add("receiveTime ", receiveTime);//数据上报时间

        //    string jsonSend = joSend.ToString();

        //    bool ret = KafkaClient.SendToKafka(topic, jsonSend, 上报Url);
        //    if (ret)
        //    {
        //        LogManager.AddMessage(string.Format("上报数据成功 Url:{0},topic:{1},接口描述:{2},上报数据Json内容\r\n:{3}", 上报Url, topic, topicName, jsonSend));
        //    }
        //    else
        //    {

        //    }
        //}
        ///// <summary>
        ///// 13.6 不用传 告警  button 13
        ///// </summary> 
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public void AlarmData(string warnInfo, string warnSuggest)
        //{

        //    string curWkStatusCode = "04";//01:预热；02:空闲；03:离线；04:运行；05:告警；06:自校准
        //    string warnCode = "001";
        //    string startTime = DateTime.Now.AddSeconds(-2).ToString("yyyy-MM-dd HH:mm:ss");
        //    string warnLevel = "02";//01:紧急；02:一般；03:提示
        //    string plantWarnType = "02";//01;心跳；02; 网络;03; 功能;04; 安全;05; 检定质量 06:故障
        //    string epiPos = "";
        //    string receiveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


        //    string topic = "GK_JD_PLAN_ALARM_INFO_TOPIC";
        //    string topicName = "设备告警信息上报接口";

        //    JObject joSend = new JObject();
        //    joSend.Add("plantNo", plantNo);//设备编号
        //    joSend.Add("warnCode", warnCode);//告警代码
        //    joSend.Add("startTime", startTime);//当前状态开始时间
        //    joSend.Add("warnLevel", warnLevel);//告警等级
        //    joSend.Add("plantWarnType", plantWarnType);//告警类型
        //    joSend.Add("warnInfo", warnInfo);//告警描述
        //    joSend.Add("warnSuggest ", warnSuggest);//处理建议
        //    joSend.Add("epiPos", epiPos);//告警表位
        //    joSend.Add("receiveTime ", receiveTime);//数据上报时间


        //    string jsonSend = joSend.ToString();

        //    bool ret = KafkaClient.SendToKafka(topic, jsonSend, 上报Url);
        //    if (ret)
        //    {
        //        LogManager.AddMessage(string.Format("13.6---上报数据成功 Url:{0},topic:{1},接口描述:{2},上报数据Json内容:\r\n{3}", 上报Url, topic, topicName, jsonSend));
        //    }
        //    else 
        //    {
        //    }
        //}

        ///// <summary>
        ///// 13.8  完成
        ///// </summary>
        ///// <param name="ip"></param>
        ///// <param name="port"></param>
        ///// <param name="frequency"></param>
        //public void Updata(string ip, string port, string frequency)
        //{
        //    LogManager.AddMessage(string.Format("默认上报频率：{0}", 上报频率.ToString()));

        //    Thread thread = new Thread(new ThreadStart(YUNXING138));
        //    thread.Start();
        //}




        ///// <summary>
        ///// 开机检查 13.9  上传数据不确定
        ///// </summary>
        //public static void BootCheck()
        //{

        //    string itemNo = "01";
        //    string itemName = "温度";
        //    string itemType = "02";
        //    string machNo = "";//专机编号
        //    string devSeatNo = "01";
        //    string startTime = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");
        //    string finishTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    string checkConc = "01";

        //    string warnCode = "01";
        //    string warnLevel = "02";//01:紧急；02:一般；03:提示
        //    string plantWarnType = "03";//01;心跳；02; 网络;03; 功能;04; 安全;05; 检定质量 06:故障
        //    string warnInfo = "升压失败";
        //    string warnSuggest = "";
        //    string epiPos = "";
        //    string receiveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


        //    string topic = "GK_JD_BOOT_CHECK_TOPIC";
        //    string topicName = "开机检查结果上报";

        //    JObject joSend = new JObject();
        //    joSend.Add("plantNo", plantNo);//设备编号
        //    joSend.Add("itemNo", itemNo);//参数编号
        //    joSend.Add("itemName", itemName);//参数名称
        //    joSend.Add("itemType", itemType);//参数类型
        //    joSend.Add("detectSn", "01");//顺序号
        //    joSend.Add("startTime", startTime);//开始时间
        //    joSend.Add("finishTime", finishTime);//完成时间
        //    joSend.Add("checkConc", checkConc);//检查结果
        //    joSend.Add("checkData", "");//检查数据

        //    if (checkConc == "02")
        //    {
        //        JArray ja_Warn = new JArray();
        //        JObject joWarn = new JObject();

        //        joWarn.Add("plantNo", plantNo);//设备编号
        //        joWarn.Add("machNo", machNo);//上报时间
        //        joWarn.Add("warnCode", warnCode);//告警代码
        //        joWarn.Add("startTime", startTime);//当前状态开始时间
        //        joWarn.Add("warnLevel", warnLevel);//告警等级
        //        joWarn.Add("plantWarnType", plantWarnType);//告警类型
        //        joWarn.Add("warnInfo", warnInfo);//告警描述
        //        joWarn.Add("warnSuggest ", warnSuggest);//处理建议
        //        joWarn.Add("epiPos", epiPos);//告警表位
        //        joWarn.Add("receiveTime ", receiveTime);//数据上报时间
        //        ja_Warn.Add(joWarn);
        //        joSend.Add("plantWarnList", ja_Warn);//专机告警信息
        //    }
        //    else
        //    {
        //        joSend.Add("plantWarnList", null);//专机告警信息
        //    }

        //    string jsonSend = joSend.ToString();

        //    bool ret = KafkaClient.SendToKafka(topic, jsonSend, 上报Url);
        //    if (ret)
        //    {
        //        LogManager.AddMessage(string.Format("13.9---上报数据成功 Url:{0},topic:{1},接口描述:{2},上报数据Json内容:{3}\r\n", 上报Url, topic, topicName, jsonSend));
        //    }
        //    else
        //    {

        //    }
        //}
        #endregion


        #region  智慧实验室工况数据上传
        bool flag = ConfigHelper.Instance.RGTORZJ;
        string taskno = "";
        static string plantNo = ConfigHelper.Instance.PlantNO;
        /// <summary>
        /// 8.1 设备事件上报接口
        /// </summary>
        /// <param name="plantEventType"></param>
        /// <param name="veriltemParaNo"></param>
        /// <param name="flag"> flag=是  人工台  false 质检设备</param>
        public void EventEscalation(string plantEventType, string veriltemParaNo)
        {
            //判断任务号是否是一个
            for (int i = 0; i < EquipmentData.MeterGroupInfo.Meters.Count; i++)
            {

            }
            taskno = EquipmentData.MeterGroupInfo.Meters[0].GetProperty("MD_TASK_NO").ToString();  ///任务号
            //获取电表任务信息
            //0101:开机；
            //0102:关机；
            //0103:开始预热；
            //0104:结束预热；
            //0105:任务开始；
            //0106:任务结束；
            //0107:暂停；
            //0108:暂停结束;
            //0109:开始检修；
            //0110:结束检修；
            //0111:异常停机；
            //0112:异常停机结束；
            //0119:开始试验项；
            //0120:结束试验项；

            string eventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string devSeatStatusList = null;

            string topic = "GK_ZJ_PLAN_EVENT_TOPIC";
            string topicName = "设备事件上报接口";

            if (flag)
            {
                topic = "GK_JD_PLAN_EVENT_TOPIC";
            }

            JObject joSend = new JObject();
            joSend.Add("plantNo", plantNo);//设备编号
            joSend.Add("taskNo", taskno);//任务编号
            joSend.Add("plantEventType", plantEventType);//事件类型
            joSend.Add("eventTime", eventTime);//事件发生时间  
            joSend.Add("veriltemParaNo", veriltemParaNo);//试验项项参数编码  //项目编号
            joSend.Add("devSeatStatusList", devSeatStatusList);//事件类型   

            string jsonSend = joSend.ToString();
            KafkaClient kafka = new KafkaClient(上报Url);
            bool ret = kafka.SendToKafka(topic, jsonSend, 上报Url);

            if (ret)
            {
                LogManager.AddMessage(string.Format("设备事件上报接口---上报数据成功 Url:{0},topic:{1},接口描述:{2},上报数据Json内容:\r\n{3}", 上报Url, topic, topicName, jsonSend), 3);
            }
            else
            {

            }
        }

        /// <summary>
        /// 8.2 专机编号
        /// </summary>
        /// <param name="plantEventType"></param>
        /// <param name="veriltemParaNo"></param>
        /// <param name="str"></param>
        public void ZJ_MACH_EVENT_TOPIC(string plantEventType, string veriltemParaNo)
        {
            //判断任务号是否是一个
            for (int i = 0; i < EquipmentData.MeterGroupInfo.Meters.Count; i++)
            {

            }
            taskno = EquipmentData.MeterGroupInfo.Meters[0].GetProperty("MD_TASK_NO").ToString();  ///任务号
            //获取电表任务信息
            //0101:开机；
            //0102:关机；
            //0103:开始预热；
            //0104:结束预热；
            //0105:任务开始；
            //0106:任务结束；
            //0107:暂停；
            //0108:暂停结束;
            //0109:开始检修；
            //0110:结束检修；
            //0111:异常停机；
            //0112:异常停机结束；
            //0119:开始试验项；
            //0120:结束试验项；

            string eventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string devSeatStatusList = null;

            string topic = "GK_ZJ_MACH_EVENT_TOPIC";
            string topicName = "专机事件上报接口";

            JObject joSend = new JObject();
            joSend.Add("plantNo", plantNo);//设备编号
            joSend.Add("taskNo", taskno);//任务编号
            joSend.Add("plantEventType", plantEventType);//事件类型
            joSend.Add("eventTime", eventTime);//事件发生时间  
            joSend.Add("veriltemParaNo", veriltemParaNo);//试验项项参数编码  //项目编号
            joSend.Add("devSeatStatusList", devSeatStatusList);//事件类型   

            string jsonSend = joSend.ToString();
            KafkaClient kafka = new KafkaClient(上报Url);
            bool ret = kafka.SendToKafka(topic, jsonSend, 上报Url);
            if (ret)
            {
                LogManager.AddMessage(string.Format("专机事件上报接口---上报数据成功 Url:{0},topic:{1},接口描述:{2},上报数据Json内容:\r\n{3}", 上报Url, topic, topicName, jsonSend), 3);
            }
            else
            {

            }
        }

        /// <summary>
        /// 8.3 设备工作状态信息上报接口
        /// </summary>
        /// <param name="plantEventType"></param>
        /// <param name="veriltemParaNo"></param>
        /// <param name="str"></param>
        public void WorkingStatus(string plantEventType, string veriltemParaNo)
        {

            //判断任务号是否是一个
            for (int i = 0; i < EquipmentData.MeterGroupInfo.Meters.Count; i++)
            {

            }
            taskno = EquipmentData.MeterGroupInfo.Meters[0].GetProperty("MD_TASK_NO").ToString();  ///任务号
            //获取电表任务信息
            //0101:开机；
            //0102:关机；
            //0103:开始预热；
            //0104:结束预热；
            //0105:任务开始；
            //0106:任务结束；
            //0107:暂停；
            //0108:暂停结束;
            //0109:开始检修；
            //0110:结束检修；
            //0111:异常停机；
            //0112:异常停机结束；
            //0119:开始试验项；
            //0120:结束试验项；

            string eventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string devSeatStatusList = null;

            string topic = "GK_ZJ_PLAN_WORKING_STATUS_TOPIC";
            string topicName = "设备工作状态信息上报接口";
            if (flag)
            {
                topic = "GK_JD_PLAN_WORKING_STATUS_TOPIC";
            }

            JObject joSend = new JObject();
            joSend.Add("plantNo", plantNo);//设备编号
            joSend.Add("taskNo", taskno);//任务编号
            joSend.Add("plantEventType", plantEventType);//事件类型
            joSend.Add("eventTime", eventTime);//事件发生时间  
            joSend.Add("veriltemParaNo", veriltemParaNo);//试验项项参数编码  //项目编号
            joSend.Add("devSeatStatusList", devSeatStatusList);//事件类型   

            string jsonSend = joSend.ToString();

            KafkaClient kafka = new KafkaClient(上报Url);
            bool ret = kafka.SendToKafka(topic, jsonSend, 上报Url);
            if (ret)
            {
                LogManager.AddMessage(string.Format("设备工作状态信息上报接口---上报数据成功 Url:{0},topic:{1},接口描述:{2},上报数据Json内容:\r\n{3}", 上报Url, topic, topicName, jsonSend), 3);
            }
            else
            {

            }
        }

        /// <summary>
        /// 8.4 推送专机工作状态信息
        /// </summary>
        /// <param name="plantEventType"></param>
        /// <param name="veriltemParaNo"></param>
        /// <param name="str"></param>
        public void ZJWORKING_STATUS(string plantEventType, string veriltemParaNo)
        {
            //判断任务号是否是一个
            for (int i = 0; i < EquipmentData.MeterGroupInfo.Meters.Count; i++)
            {

            }
            taskno = EquipmentData.MeterGroupInfo.Meters[0].GetProperty("MD_TASK_NO").ToString();  ///任务号
            //获取电表任务信息
            //0101:开机；
            //0102:关机；
            //0103:开始预热；
            //0104:结束预热；
            //0105:任务开始；
            //0106:任务结束；
            //0107:暂停；
            //0108:暂停结束;
            //0109:开始检修；
            //0110:结束检修；
            //0111:异常停机；
            //0112:异常停机结束；
            //0119:开始试验项；
            //0120:结束试验项；

            string eventTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string devSeatStatusList = null;

            string topic = "GK_ZJ_MACH_WORKING_STATUS_TOPIC";
            string topicName = "推送专机工作状态信息";

            JObject joSend = new JObject();
            joSend.Add("plantNo", plantNo);//设备编号
            joSend.Add("taskNo", taskno);//任务编号
            joSend.Add("plantEventType", plantEventType);//事件类型
            joSend.Add("eventTime", eventTime);//事件发生时间  
            joSend.Add("veriltemParaNo", veriltemParaNo);//试验项项参数编码  //项目编号
            joSend.Add("devSeatStatusList", devSeatStatusList);//事件类型   

            string jsonSend = joSend.ToString();

            KafkaClient kafka = new KafkaClient(上报Url);
            bool ret = kafka.SendToKafka(topic, jsonSend, 上报Url);
            if (ret)
            {
                LogManager.AddMessage(string.Format("推送专机工作状态信息---上报数据成功 Url:{0},topic:{1},接口描述:{2},上报数据Json内容:\r\n{3}", 上报Url, topic, topicName, jsonSend), 3);
            }
            else
            {

            }
        }


        /// <summary>
        /// 8.5 设备告警信息
        /// </summary> 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AlarmData(string warnInfo, string warnSuggest)
        {

            string curWkStatusCode = "04";//01:预热；02:空闲；03:离线；04:运行；05:告警；06:自校准
            string warnCode = "001";
            string startTime = DateTime.Now.AddSeconds(-2).ToString("yyyy-MM-dd HH:mm:ss");
            string warnLevel = "02";//01:紧急；02:一般；03:提示
            string plantWarnType = "02";//01;心跳；02; 网络;03; 功能;04; 安全;05; 检定质量 06:故障
            string epiPos = "";
            string receiveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


            string topic = "GK_ZJ_PLAN_WARN_INFO_TOPIC";
            string topicName = "设备告警信息上报接口";
            if (flag)
            {
                topic = "GK_JD_PLAN_ALARM_INFO_TOPIC";
            }
            JObject joSend = new JObject();
            joSend.Add("plantNo", plantNo);//设备编号
            joSend.Add("warnCode", warnCode);//告警代码
            joSend.Add("startTime", startTime);//当前状态开始时间
            joSend.Add("warnLevel", warnLevel);//告警等级
            joSend.Add("plantWarnType", plantWarnType);//告警类型
            joSend.Add("warnInfo", warnInfo);//告警描述
            joSend.Add("warnSuggest ", warnSuggest);//处理建议
            joSend.Add("epiPos", epiPos);//告警表位
            joSend.Add("receiveTime ", receiveTime);//数据上报时间


            string jsonSend = joSend.ToString();

            KafkaClient kafka = new KafkaClient(上报Url);
            bool ret = kafka.SendToKafka(topic, jsonSend, 上报Url);
            if (ret)
            {
                LogManager.AddMessage(string.Format("设备告警信息上报接口--上报数据成功 Url:{0},topic:{1},接口描述:{2},上报数据Json内容:\r\n{3}", 上报Url, topic, topicName, jsonSend), 3);
            }
            else
            {
            }
        }

        /// <summary>
        /// 8.6 专机告警信息
        /// </summary>
        /// <param name="warnInfo"></param>
        /// <param name="warnSuggest"></param>
        /// <param name="str"></param>
        public void ZJAlarmData(string warnInfo, string warnSuggest, string str = "质检")
        {

            string curWkStatusCode = "04";//01:预热；02:空闲；03:离线；04:运行；05:告警；06:自校准
            string warnCode = "001";
            string startTime = DateTime.Now.AddSeconds(-2).ToString("yyyy-MM-dd HH:mm:ss");
            string warnLevel = "02";//01:紧急；02:一般；03:提示
            string plantWarnType = "02";//01;心跳；02; 网络;03; 功能;04; 安全;05; 检定质量 06:故障
            string epiPos = "";
            string receiveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


            string topic = "GK_ZJ_MACH_WARN_INFO_TOPIC";
            string topicName = "推送设备告警信息信息";

            JObject joSend = new JObject();
            joSend.Add("plantNo", plantNo);//设备编号
            joSend.Add("warnCode", warnCode);//告警代码
            joSend.Add("startTime", startTime);//当前状态开始时间
            joSend.Add("warnLevel", warnLevel);//告警等级
            joSend.Add("plantWarnType", plantWarnType);//告警类型
            joSend.Add("warnInfo", warnInfo);//告警描述
            joSend.Add("warnSuggest ", warnSuggest);//处理建议
            joSend.Add("epiPos", epiPos);//告警表位
            joSend.Add("receiveTime ", receiveTime);//数据上报时间


            string jsonSend = joSend.ToString();

            KafkaClient kafka = new KafkaClient(上报Url);
            bool ret = kafka.SendToKafka(topic, jsonSend, 上报Url);
            if (ret)
            {
                LogManager.AddMessage(string.Format("专机告警信息上报接口--上报数据成功 Url:{0},topic:{1},接口描述:{2},上报数据Json内容:\r\n{3}", 上报Url, topic, topicName, jsonSend), 3);
            }
            else
            {
            }
        }

        ///8.7  设备输出信息上报接口  button 14
        ///

        /// <summary>
        /// 上报频率设置
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="frequency"></param>
        public void Updata(string ip, string port, string frequency)
        {
            LogManager.AddMessage(string.Format("默认上报频率：{0}", 上报频率.ToString()), 3);

            Thread thread = new Thread(new ThreadStart(YUNXING138));
            thread.Start();
        }
        private void YUNXING138()
        {
            while (true)
            {
                OUTPUT_INFO();
                Thread.Sleep(上报频率 * 1000);
            }
        }

        public void OUTPUT_INFO()
        {
            bool flag = ConfigHelper.Instance.RGTORZJ;
            string topic = "GK_ZJ_OUTPUT_INFO_TOPIC";
            string topicName = "设备输出信息上报接口";
            if (flag)
            {
                topic = " GK_JD_OUTPUT_INFO_TOPIC";
            }
            string jsonSend = "";
            bool ret = true;
            try
            {
                string machNo = "";//专机编号
                string devSeatNo = "01"; //
                string occurTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                JObject joSend = new JObject();
                joSend.Add("plantNo", plantNo);//设备编号
                joSend.Add("machNo", machNo);//专机编号
                joSend.Add("devSeatNo", devSeatNo);//表位编号
                joSend.Add("occurTime", occurTime);//上报时间
                #region 上报数据
                JArray jaValues = new JArray();

                JObject joValue = new JObject();
                joValue.Add("outParaName", "voltageA");//电压
                joValue.Add("outParaValue", EquipmentData.StdInfo.Ua.ToString());//电压A
                jaValues.Add(joValue);

                joValue = new JObject();
                joValue.Add("outParaName", "voltageB");//电压
                joValue.Add("outParaValue", EquipmentData.StdInfo.Ub.ToString());//电压B
                jaValues.Add(joValue);

                joValue = new JObject();
                joValue.Add("outParaName", "voltageC");//电压
                joValue.Add("outParaValue", EquipmentData.StdInfo.Uc.ToString());//电压C
                jaValues.Add(joValue);

                joValue = new JObject();
                joValue.Add("outParaName", "currentA");//电流
                joValue.Add("outParaValue", Convert.ToDecimal(Decimal.Parse(EquipmentData.StdInfo.Ia.ToString(), System.Globalization.NumberStyles.Float)));//电流A
                jaValues.Add(joValue);


                joValue = new JObject();
                joValue.Add("outParaName", "currentB");//电流
                joValue.Add("outParaValue", Convert.ToDecimal(Decimal.Parse(EquipmentData.StdInfo.Ib.ToString(), System.Globalization.NumberStyles.Float)));//电流B
                jaValues.Add(joValue);

                joValue = new JObject();
                joValue.Add("outParaName", "currentC");//电流
                joValue.Add("outParaValue", Convert.ToDecimal(Decimal.Parse(EquipmentData.StdInfo.Ic.ToString(), System.Globalization.NumberStyles.Float)));//电流C
                jaValues.Add(joValue);

                joValue = new JObject();
                joValue.Add("outParaName", "phaseA");//相位
                joValue.Add("outParaValue", EquipmentData.StdInfo.PhaseA.ToString());//相位
                jaValues.Add(joValue);


                joValue = new JObject();
                joValue.Add("outParaName", "phaseB");//相位
                joValue.Add("outParaValue", EquipmentData.StdInfo.PhaseB.ToString());//相位
                jaValues.Add(joValue);

                joValue = new JObject();
                joValue.Add("outParaName", "phaseC");//相位
                joValue.Add("outParaValue", EquipmentData.StdInfo.PhaseC.ToString());//相位
                jaValues.Add(joValue);

                //joValue = new JObject();
                //joValue.Add("outParaName", "activePowerA");//有功功率
                //joValue.Add("outParaValue", EquipmentData.StdInfo.Pa);
                //jaValues.Add(joValue);

                //joValue = new JObject();
                //joValue.Add("outParaName", "activePowerB");//有功功率
                //joValue.Add("outParaValue", EquipmentData.StdInfo.Pb);
                //jaValues.Add(joValue);

                //joValue = new JObject();
                //joValue.Add("outParaName", "activePowerC");//有功功率
                //joValue.Add("outParaValue", EquipmentData.StdInfo.Pc);
                //jaValues.Add(joValue);

                //joValue = new JObject();
                //joValue.Add("outParaName", "frequency");//频率
                //joValue.Add("outParaValue", EquipmentData.StdInfo.Freq);
                //jaValues.Add(joValue);

                //joValue = new JObject();
                //joValue.Add("outParaName", "temp");//温度
                //joValue.Add("outParaValue", MeterGroupInfo.Meters[0].GetProperty("MD_TEMPERATURE").ToString());
                //jaValues.Add(joValue);

                //joValue = new JObject();
                //joValue.Add("outParaName", "humidit");//湿度
                //joValue.Add("outParaValue", MeterGroupInfo.Meters[0].GetProperty("MD_HUMIDITY").ToString());
                jaValues.Add(joValue);

                joSend.Add("realTimeValues", jaValues);
                joSend.Add("remarks", "");
                #endregion
                jsonSend = joSend.ToString();

                KafkaClient kafka = new KafkaClient(上报Url);
                ret = kafka.SendToKafka(topic, jsonSend, 上报Url);

            }
            catch (Exception ex)
            {
                LogManager.AddMessage(ex.Message + "设备输出信息上报接口失败-------------", 3);
            }
            finally
            {

                if (ret)
                {
                    LogManager.AddMessage(string.Format("设备输出信息上报接口---上报数据成功 Url:{0},topic:{1},接口描述:{2},上报数据Json内容:\r\n{3}", 上报Url, topic, topicName, jsonSend), 3);
                }
                else
                {

                }
            }
        }

        ///8.8 日志
        public void GK_ZJ_LOG_INFO_TOPIC(string logtime, string plantno)
        {
            string topic = "GK_ZJ_LOG_INFO_TOPIC";
            string topicName = "推送设备日志信息信息";

            JObject joSend = new JObject();
            joSend.Add("plantNo", plantNo);//设备编号
            joSend.Add("machNo", plantNo);//专机编号
            joSend.Add("logTime", plantNo);//时间
            joSend.Add("logInfo", plantNo);//内容

            string jsonSend = joSend.ToString();
            KafkaClient kafka = new KafkaClient(上报Url);
            bool ret = kafka.SendToKafka(topic, jsonSend, 上报Url);
            if (ret)
            {
                LogManager.AddMessage(string.Format("推送设备日志信息信息--上报数据成功 Url:{0},topic:{1},接口描述:{2},上报数据Json内容:\r\n{3}", 上报Url, topic, topicName, jsonSend), 3);
            }
            else
            {
            }
        }

        /// 8.9 开机检查结果上报 BootCheck 
        public void BootCheck()
        {

            string itemNo = "01";
            string itemName = "温度";
            string itemType = "02";
            string machNo = "";//专机编号
            string devSeatNo = "01";
            string startTime = DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");
            string finishTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string checkConc = "01";

            string warnCode = "01";
            string warnLevel = "02";//01:紧急；02:一般；03:提示
            string plantWarnType = "03";//01;心跳；02; 网络;03; 功能;04; 安全;05; 检定质量 06:故障
            string warnInfo = "升压失败";
            string warnSuggest = "";
            string epiPos = "";
            string receiveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


            string topic = "GK_ZJ_BOOT_CHECK_TOPIC";
            string topicName = "开机检查结果上报";
            if (flag)
            {
                topic = "GK_JD_BOOT_CHECK_TOPIC";
            }
            JObject joSend = new JObject();
            joSend.Add("plantNo", plantNo);//设备编号
            joSend.Add("itemNo", itemNo);//参数编号
            joSend.Add("itemName", itemName);//参数名称
            joSend.Add("itemType", itemType);//参数类型
            joSend.Add("detectSn", "01");//顺序号
            joSend.Add("startTime", startTime);//开始时间
            joSend.Add("finishTime", finishTime);//完成时间
            joSend.Add("checkConc", checkConc);//检查结果
            joSend.Add("checkData", "");//检查数据

            if (checkConc == "02")
            {
                JArray ja_Warn = new JArray();
                JObject joWarn = new JObject();

                joWarn.Add("plantNo", plantNo);//设备编号
                joWarn.Add("machNo", machNo);//上报时间
                joWarn.Add("warnCode", warnCode);//告警代码
                joWarn.Add("startTime", startTime);//当前状态开始时间
                joWarn.Add("warnLevel", warnLevel);//告警等级
                joWarn.Add("plantWarnType", plantWarnType);//告警类型
                joWarn.Add("warnInfo", warnInfo);//告警描述
                joWarn.Add("warnSuggest ", warnSuggest);//处理建议
                joWarn.Add("epiPos", epiPos);//告警表位
                joWarn.Add("receiveTime ", receiveTime);//数据上报时间
                ja_Warn.Add(joWarn);
                joSend.Add("plantWarnList", ja_Warn);//专机告警信息
            }
            else
            {
                joSend.Add("plantWarnList", null);//专机告警信息
            }

            string jsonSend = joSend.ToString();

            KafkaClient kafka = new KafkaClient(上报Url);
            bool ret = kafka.SendToKafka(topic, jsonSend, 上报Url);
            if (ret)
            {
                LogManager.AddMessage(string.Format("13.9---上报数据成功 Url:{0},topic:{1},接口描述:{2},上报数据Json内容:\r\n{3}", 上报Url, topic, topicName, jsonSend), 3);
            }
            else
            {

            }
        }

        #endregion
    }
}
