using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LYTest.VerifyService
{
    [ServiceContract]
    public interface IVerifyMessage
    {
        /// <summary>
        /// 客户端发起连接请求
        /// </summary>
        /// <returns>获取到的连接ID</returns>
        [OperationContract]
        int RequestId();
        /// <summary>
        /// 运行日志上报
        /// </summary>
        /// <param name="connectionId">连接ID</param>
        /// <param name="messageSourse">消息数据源</param>
        /// <param name="messageType">消息类型</param>
        /// <param name="message">消息内容</param>
        /// <returns>消息处理结果</returns>
        [OperationContract]
        bool OutMessage(int connectionId, int messageSourse, int messageType, string message);
        /// <summary>
        /// 上传检定数据
        /// </summary>
        /// <param name="connectionId">连接ID</param>
        /// <param name="itemKey">检定点编号</param>
        /// <param name="dataName">结论名称</param>
        /// <param name="dataValue">结论内容</param>
        /// <returns>数据处理结果</returns>
        [OperationContract]
        bool OutVerifyData(int connectionId, string itemKey, string dataName, string[] dataValue);
        /// <summary>
        /// 上传一块表的数据,代码暂时未处理,预留
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="itemKey"></param>
        /// <param name="dataName"></param>
        /// <param name="meterIndex"></param>
        /// <param name="dataValue"></param>
        /// <returns></returns>
        [OperationContract]
        bool OutOneMeterData(int connectionId, string itemKey, string dataName, int meterIndex, string dataValue);
        /// <summary>
        /// 设备状态监视
        /// </summary>
        /// <param name="connectionId">连接ID</param>
        /// <param name="monitorType">监视的数据类型</param>
        /// <param name="formatData">数据格式</param>
        /// <returns>数据处理结果</returns>
        [OperationContract]
        bool OutMonitorInfo(int connectionId, int monitorType, string formatData);
        /// <summary>
        /// 通知当前检定项执行完毕
        /// </summary>
        /// <param name="connectionId">连接ID</param>
        /// <returns>处理结果</returns>
        [OperationContract]
        bool VerifyFinished(int connectionId);
        /// <summary>
        /// 上传设备操作报文
        /// </summary>
        /// <param name="connectionId">连接ID</param>
        /// <param name="frameString">报文内容</param>
        /// <returns>报文处理结果</returns>
        [OperationContract]
        bool OutFrame(int connectionId, string frameString);
        /// <summary>
        /// 检定心跳包,用与同步台体检定状态
        /// </summary>
        /// <param name="connectionId">连接ID</param>
        /// <param name="checkState">检定状态</param>
        /// <returns>处理结果</returns>
        [OperationContract]
        bool NotifyIsChecking(int connectionId, string checkState);
    }
}
