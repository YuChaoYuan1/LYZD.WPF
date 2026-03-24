using LYZD.DAL;
using LYZD.DAL.Config;
using LYZD.Utility.Log;
using LYZD.ViewModel.CheckController;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace LYZD.ViewModel.IMICP
{
	public class restfulController : ApiController
	{
		string strrecv = "";
		public static string 序列化对象(object obj)
		{
			return JsonConvert.SerializeObject(obj);
		}
		/// <summary>
		/// 12.1 任务控制接口
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		[HttpPost]
		public IHttpActionResult gkjdSendCtrlTaskStatus([FromBody] ShanXiTable table)
		{
			string taskNo = table.taskNo;
			string plantNo = table.plantNo;
			string insrutiongsType = table.instructionsType;
			string str = "";
			LogManager.AddMessage(string.Format("12.1接受到的数据\r\n{0}", 序列化对象(table)));
			CallResult obj = new CallResult();
			obj.resultFlag = "1";
			obj.errorInfo = "";
			LogManager.AddMessage(string.Format("12.1返回的数据\r\n{0}", 序列化对象(obj)));

			EquipmentData.IMICPKz(insrutiongsType);
			LogManager.AddMessage(string.Format("任务控制，任务编号{0}，指令类型{1}", taskNo, str), EnumLogSource.检定业务日志, EnumLevel.Tip);
			return Json(obj);
		}

		/// <summary>
		/// 12.2 任务优先级调整接口
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		[HttpPost]
		public IHttpActionResult gkjdSendUpdateTaskPriority([FromBody] ShanXiTable table)
		{
			string taskNo = table.taskNo;
			string plantNo = table.plantNo;
			string priority = table.priority;

			LogManager.AddMessage(string.Format("12.2接受到的数据\r\n{0}", 序列化对象(table)));
			CallResult obj = new CallResult();
			obj.resultFlag = "1";
			obj.errorInfo = "";
			LogManager.AddMessage(string.Format("12.2返回的数据\r\n{0}", 序列化对象(obj)));
			LogManager.AddMessage(string.Format("任务优先级变更，任务编号{0}，优先级{1}(优先级越小，越考前)", taskNo, priority), EnumLogSource.检定业务日志, EnumLevel.Tip);
			return Json(obj);
		}

		/// <summary>
		/// 12.4 系统控制接口
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		[HttpPost]
		public IHttpActionResult gkjdSendSystemCtrl([FromBody] ShanXiTable table)
		{

			string plantNo = table.plantNo;
			string insrutiongsType = table.insrutiongsType;
			OpenPortIMICP open = new OpenPortIMICP();
			open.BootCheck();
			LogManager.AddMessage(string.Format("gkjdSendSystemCtrl----接受到的数据\r\n{0}", 序列化对象(table)));
			CallResult obj = new CallResult();
			obj.resultFlag = "1";
			obj.errorInfo = "";
			LogManager.AddMessage(string.Format("gkjdSendSystemCtrl----返回的数据\r\n{0}", 序列化对象(obj)));

			return Json(obj);
		}

		/// <summary>
		/// 12.8 设备输出信息上报频率配置接口
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		[HttpPost]
		public IHttpActionResult gkjdSetUploadFrequency([FromBody] ShanXiTable table)
		{
			string plantNo = table.plantNo;
			string machNo = table.machNo;
			string frequency = table.frequency;

			OpenPortIMICP.ReceData("12.8" + 序列化对象(table));

			LogManager.AddMessage(string.Format("12.8接受到的数据\r\n{0}", 序列化对象(table)));
			CallResult obj = new CallResult();
			obj.resultFlag = "1";
			obj.errorInfo = "";
			LogManager.AddMessage(string.Format("12.8返回的数据\r\n{0}", 序列化对象(obj)));
			return Json(obj);
		}

		/// <summary>
		/// 12.9  设备/专机工作状态获取接口
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		[HttpPost]
		public IHttpActionResult gkjdGetPlantCurrentState([FromBody] ShanXiTable table)
		{
			string plantNo = table.plantNo;
			string machNo = table.machNo;
			LogManager.AddMessage(string.Format("12.9接受到的数据\r\n{0}", 序列化对象(table)));
			CallResult129 obj = new CallResult129();
			obj.resultFlag = "1";
			obj.errorInfo = "";
			if (EquipmentData.Controller.IsChecking == false)
			{
				obj.currentState = "02";
			}
			else
			{
				obj.currentState = "04";
			}

			//obj.currentState = "01";
			obj.startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			LogManager.AddMessage(string.Format("12.9返回的数据\r\n{0}", 序列化对象(obj)));

			return Json(obj);
		}

		/// <summary>
		/// 12.10 设备/专机作业执行情况获取接口
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		[HttpPost]
		public IHttpActionResult gkjdGetJobExcuteState([FromBody] ShanXiTable table)
		{
			string plantNo = table.plantNo;
			string machNo = table.machNo;
			string taskNo = table.taskNo;
			string veriItemParaNo = table.veriItemParaNo;
			LogManager.AddMessage(string.Format("12.10接受到的数据\r\n{0}", 序列化对象(table)));

			GetJobExcuteStateResult ret = new GetJobExcuteStateResult();
			ret.resultFlag = "1";
			ret.errorInfo = "";
			ret.data = new System.Collections.Generic.List<GetJobExcuteStateData>();

			GetJobExcuteStateData data1 = new GetJobExcuteStateData();

			data1.veriItemParaNo = table.veriItemParaNo;

			//string sql = string.Format("select itemName from {0} where schId='{1}'", "P_scheme", data1.veriItemParaNo);

			//data1.veriItemParaName = DbAccess.ExecuteDataField(mStrDbPath, sql, "itemName");
			GeneralDal generalDal = DALManager.IMICPData;
			string infoname = "P_scheme";
			DynamicModel model = generalDal.GetByID(infoname, $" schId='{data1.veriItemParaNo}'");
			data1.veriItemParaName = model.GetProperty("itemName").ToString();
			data1.resultList = new System.Collections.Generic.List<VeriItemResult>();


			for (int j = 0; j < ConfigHelper.Instance.MeterCount; j++)
			{
				VeriItemResult result = new VeriItemResult();
				result.devSeatNo = (j + 1).ToString();
				result.veriItemParaVal = "02";
				data1.resultList.Add(result);
			}

			ret.data.Add(data1);

			string strSend = JsonConvert.SerializeObject(ret, Formatting.Indented);

			return Json(ret);
		}

		public IHttpActionResult gkSendVeriTask([FromBody] dynamic table)
		{
			LogManager.AddMessage(string.Format("gkSendVeriTask======================= 接受到的数据\r\n{0}", 序列化对象(table)));
			LogManager.AddMessage(string.Format("gkSendVeriTask======================= 接受到的数据\r\n{0}", 序列化对象(table)),1);
			CallResult obj = new CallResult();
			obj.resultFlag = "1";
			obj.errorInfo = "";
			LogManager.AddMessage(string.Format("gkSendVeriTask=======================返回的数据\r\n{0}", 序列化对象(obj)),1);
			dynamic data = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(table.ToString());

			ConfigHelper.Instance.SysNO = data.sysNo;
			VerifyBase.OnMeterInfo.MD_SchemeID = data.taskList[0].schemeNo+"|"+ data.taskList[0].schemeNo;
			return Json(obj);
		}


		#region  甘肃质检接口
		/// <summary>
		/// 7.1 任务控制接口
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public IHttpActionResult gkzjSendCtrlTaskStatus([FromBody] dynamic str)
		{
			dynamic data = JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject>(str.ToString());

			//string strlog = Convert.tojsom str.ToString();
			EquipmentData.IMICPKz(data.instructionsType);
			LogManager.AddMessage(string.Format("gkzjSendCtrlTaskStatus---接受到的数据\r\n{0}", 序列化对象(str.ToString())),2);
			CallResult obj = new CallResult();
			obj.resultFlag = "1";
			obj.errorInfo = "";
			LogManager.AddMessage(string.Format("gkzjSendCtrlTaskStatus---返回的数据\r\n{0}", 序列化对象(obj)),2);
			return Json(obj);
		}
		/// <summary>
		/// 任务优先级调整接口
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public IHttpActionResult gkzjSendUpdateTaskPriority([FromBody] dynamic str)
		{
			LogManager.AddMessage(string.Format("gkzjSendUpdateTaskPriority--- 接受到的数据\r\n{0}", 序列化对象(str.ToString())),2);
			CallResult obj = new CallResult();
			obj.resultFlag = "1";
			obj.errorInfo = "";
			LogManager.AddMessage(string.Format("gkzjSendUpdateTaskPriority---返回的数据\r\n{0}", 序列化对象(obj)),2);
			return Json(obj);
		}
		/// <summary>
		/// 7.3系统控制接口
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public IHttpActionResult gkzjSendSystemCtrl([FromBody] DynamicModel table)
		{
			LogManager.AddMessage(string.Format("gkzjSendSystemCtrl======================= 接受到的数据\r\n{0}", 序列化对象(table)));
			CallResult obj = new CallResult();
			obj.resultFlag = "1";
			obj.errorInfo = "";
			LogManager.AddMessage(string.Format("gkzjSendSystemCtrl--返回的数据\r\n{0}", 序列化对象(obj)));
			return Json(obj);
		}
		/// <summary>
		/// 7.4 系统控制应答
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public IHttpActionResult gkzjReceiveSystemCtrlRslt([FromBody] DynamicModel table)
		{
			LogManager.AddMessage(string.Format("gkzjReceiveSystemCtrlRslt======================= 接受到的数据\r\n{0}", 序列化对象(table)));
			CallResult obj = new CallResult();
			obj.resultFlag = "1";
			obj.errorInfo = "";
			LogManager.AddMessage(string.Format("gkzjReceiveSystemCtrlRslt--返回的数据\r\n{0}", 序列化对象(obj)));
			return Json(obj);
		}
	
		/// <summary>
		/// 7.5
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		//public IHttpActionResult gkzjSendSystemCtrl([FromBody] DynamicModel table)
		//{
		//	LogManager.AddMessage(string.Format("专机接口======================= 接受到的数据\r\n{0}", 序列化对象(table)));
		//	CallResult obj = new CallResult();
		//	obj.resultFlag = "1";
		//	obj.errorInfo = "";
		//	LogManager.AddMessage(string.Format("返回的数据\r\n{0}", 序列化对象(obj)));
		//	return Json(obj);
		//}

		/// <summary>
		/// 7.7
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public IHttpActionResult gkzjSetUploadFrequency([FromBody] DynamicModel table)
		{
			LogManager.AddMessage(string.Format("设备输出信息上报频率配置接口======================= 接受到的数据\r\n{0}", 序列化对象(table)));
			CallResult obj = new CallResult();
			obj.resultFlag = "1";
			obj.errorInfo = "";
			LogManager.AddMessage(string.Format("返回的数据\r\n{0}", 序列化对象(obj)));
			return Json(obj);
		}

		/// <summary>
		/// 7.8
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		public IHttpActionResult gkzjGetPlantCurrentState([FromBody] DynamicModel table)
		{
			LogManager.AddMessage(string.Format("设备/专机工作状态获取接口======================= 接受到的数据\r\n{0}", 序列化对象(table)));
			CallResult obj = new CallResult();
			obj.resultFlag = "1";
			obj.errorInfo = "";
			LogManager.AddMessage(string.Format("返回的数据\r\n{0}", 序列化对象(obj)));
			return Json(obj);
		}
		
		#endregion

		private string GetItemId(string Name)
		{
			switch (Name)
			{
				case "091001":
					return "类型标识,1";
				case "基本参数":
					return "基本参数,2";
				case "抄表与费率参数":
					return "抄表与费率参数,3";
				//case "基本参数":
				//    return "以太网参数设置,2";
				case "状态量采集":
					return "状态量采集,8";
				case "12个/分脉冲量采集":
					return "12个/分脉冲量采集,10";
				case "120个/分脉冲量采集":
					return "120个/分脉冲量采集,11";
				case "总加组日和月电量召集":
					return "总加组日电量与月电量采集,12";
				case "分时段电能量数据存储":
					return "分时段电能量数据存储,13";
				case "实时和当前数据":
					return "实时和当前数据,15";
				case "历史日数据":
					return "历史日数据,16";
				case "负荷曲线":
					return "负荷曲线,17";
				case "历史月数据":
					return "历史月数据,18";
				case "时段功控":
					return "时段功控,20";
				case "厂休功控":
					return "厂休功控,21";
				case "营业报停功控":
					return "营业报停功控,22";
				case "当前功率下浮控":
					return "营业报停功控,23";
				case "月电控":
					return "月电控,24";
				case "购电控":
					return "购电控,25";
				case "催费告警":
					return "催费告警,26";
				case "保电功能":
					return "保电功能,27";
				case "剔除功能":
					return "剔除功能,28";
				case "遥控功能":
					return "遥控功能,29";
				case "电能表超差事件":
					return "电能量超差事件,38";
				case "电能表飞走事件":
					return "电能表飞走事件,39";
				case "电能表停走事件":
					return "电能表停走事件,40";
				case "电能表时间超差事件":
					return "电能表时间超差事件,41";
				case "终端停/上电事件":
					return "终端停/上电事件,50";
				case "终端485抄表错误":
					return "终端485抄表错误事件,55";
				case "终端对时事件":
					return "对时事件,64";
				case "常温基本误差":
					return "常温基本误差,70";
				case "功率因素基本误差":
					return "功率因素基本误差,71";
				case "谐波影响":
					return "谐波影响,72";
				case "频率影响":
					return "频率影响,73";
				case "电流不平衡影响":
					return "电流不平衡影响,74";
				case "电源影响":
					return "电源影响试验,75";
				case "日计时误差":
					return "日计时误差,91";
				case "终端维护":
					return "数据初始化,99";
				//case "密钥下装":
				//    return "身份认证及密钥协商,100";
				case "密钥下装":
					return "密钥下装,100";
				case "事件参数":
					return "事件参数,7";
				case "电能表实时数据":
					return "电能表实时数据,14";
				case "电能表当前数据":
					return "电能表当前数据,9";
				case "读取终端信息":
					return "读取终端信息,19";
				case "终端逻辑地址查询":
					return "终端逻辑地址查询,65";
				//case "交采电量清零":
				//    return "数据初始化(通信参数除外),66";
				case "交采电量清零":
					return "交采电量清零,66";
				case "终端编程事件":
					return "终端编程事件,67";
				case "终端密钥恢复":
					return "终端密钥恢复,97";
				case "安全模式":
					return "禁用安全模式字,79";
				case "外观":
					return "外观检查,80";
				case "全事件采集上报":
					return "全事件采集上报,80";

			}
			return "";
		}
	}



	public class ShanXiTable
	{
		/// <summary>
		/// 任务编号
		/// </summary>
		/// 
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string taskNo { get; set; }
		/// <summary>
		/// 设备编号
		/// </summary>
		/// 
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string plantNo { get; set; }

		/// <summary>
		/// 指令类型
		/// </summary>
		/// 
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string instructionsType { get; set; }

		/// <summary>
		/// 优先级
		/// </summary>
		/// 
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string priority { get; set; }
		/// <summary>
		/// 指令执行标志
		/// </summary>
		/// 
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string flag { get; set; }

		/// <summary>
		/// 指令执行时间
		/// </summary>
		/// 
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string resultTime { get; set; }

		/// <summary>
		/// 专机编号
		/// </summary>
		/// 
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string machNo { get; set; }

		/// <summary>
		/// 上报频率
		/// </summary>
		/// 
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string frequency { get; set; }

		/// <summary>
		/// 试验项编号
		/// </summary>
		/// 
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string veriItemParaNo { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string insrutiongsType { get; set; }
	}

	public class CallResult
	{
		/// <summary>
		/// 调用结果标志
		/// </summary>
		public string resultFlag { get; set; }
		/// <summary>
		/// 错误信息
		/// </summary>
		public string errorInfo { get; set; }
	}
	public class CallResult129 : CallResult
	{
		public string currentState { get; set; }

		public string startTime { get; set; }
	}

	public class CallResult1210 : CallResult
	{
		public List<CallResult1210Data> data = new List<CallResult1210Data>();
	}

	public class CallResult1210Data
	{
		public string veriItemParaNo { get; set; }
		public string veriItemParaName { get; set; }

		public List<ResultList> resultList = new List<ResultList>();
	}

	public class ResultList
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string devSeatNo { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string veriItemParaVal { get; set; }
	}

	public class GetJobExcuteStateResult
	{
		/// <summary>
		/// 1 
		/// </summary>
		[DataMember]
		public string errorInfo;

		/// <summary>
		/// 2 
		/// </summary>
		[DataMember]
		public string resultFlag;

		/// <summary>
		/// 3 
		/// </summary>
		[DataMember]
		public List<GetJobExcuteStateData> data;
	}

	public class GetJobExcuteStateData
	{
		/// <summary>
		/// 1 试验小项编号
		/// </summary>
		[DataMember]
		public string veriItemParaNo;

		/// <summary>
		/// 2 试验小项名称
		/// </summary>
		[DataMember]
		public string veriItemParaName;

		/// <summary>
		/// 3 试验小项结果列表
		/// </summary>
		[DataMember]
		public List<VeriItemResult> resultList;
	}

	/// <summary>
	/// 试验小项结论,出参
	/// </summary>
	[DataContract]
	public class VeriItemResult
	{
		/// <summary>
		/// 1 表位编号
		/// </summary>
		[DataMember]
		public string devSeatNo;

		/// <summary>
		/// 2 试验小项结果值
		/// </summary>
		[DataMember]
		public string veriItemParaVal;
	}




}
