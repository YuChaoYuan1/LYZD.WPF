using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LYZD.SocketComm
{
    /// <summary>
    /// 通讯连接池、使用连接名称获取或设置、使用连接对象
    /// </summary>
    public class SockPool
    {
        /// <summary>
        /// 通讯池单例对象
        /// </summary>
        public static SockPool Instance { get; private set; }

        /// <summary>
        /// 连接对象列表
        /// </summary>
        private Dictionary<string, Connection> dic_Sock = new Dictionary<string, Connection>();

        /// <summary>
        /// 线程锁,每个端口一个线程锁
        /// </summary>
        private Dictionary<string, object> dic_Lock = new Dictionary<string, object>();

        private object objLock = new object();

        static SockPool()
        {
            Instance = new SockPool();
        }

        public SockPool()
        {
            if (Instance != null)
            {
                throw new Exception("单例模式构造，不允许多次实例化对象");
            }
        }

        /// <summary>
        /// 清除所有连接
        /// </summary>
        public void Clear()
        {
            lock (objLock)
            {
                foreach (string szKey in dic_Sock.Keys)
                {
                    dic_Sock[szKey].Close();
                }
                dic_Sock.Clear();
                dic_Lock.Clear();
            }
        }




    }
}
