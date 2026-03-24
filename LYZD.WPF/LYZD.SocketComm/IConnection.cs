using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.SocketComm
{
    interface IConnection
    {
        /// <summary>
        /// 连接名称
        /// </summary>
        string ConnectName { get; set; }

        /// <summary>
        /// 最大等待时间
        /// </summary>
        int MaxWaitSenconds { get; set; }

        /// <summary>
        /// 字节间隔最大等待时间
        /// </summary>
        int WaitSecondsPerByte { get; set; }

        bool Open();

        bool Close();

        /// <summary>
        /// 更新端口信息
        /// </summary>
        /// <param name="p_str_BaudRate"></param>
        /// <returns></returns>
        bool UpdateBaudSetting(string p_str_BaudRate);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="p_byt_Data">要发送的数据</param>
        /// <param name="p_bol_IsReturn">量否需要回复</param>
        /// <param name="p_int_WaitTime">发送后等待时间</param>
        /// <returns>发送是否成功</returns>
        bool SendData(ref byte[] p_byt_Data, bool p_bol_IsReturn, int p_int_WaitTime);
    }
}
