using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZH.MeterProtocol.Comm;

namespace LYZD.ViewModel.CheckController.MulitThread
{
    public class SocketThreadManager : SingletonBase<SocketThreadManager>
    {
        /// <summary>
        /// 最大线程数量
        /// </summary>
        public int MaxThread { get; set; }

        /// <summary>
        /// 每个线程最大任务数
        /// </summary>
        public int MaxTaskCountPerThread { get; set; }

        /// <summary>
        /// 工作线程数组
        /// </summary>
        private SocketThread[] SocketThreads = new SocketThread[0];

        public SocketThreadManager()
        {
            MaxThread =1;
            MaxTaskCountPerThread = 1;
        }

        public Action<int> DoWork
        {
            private get;
            set;
        }
        /// <summary>
        /// 启动线程
        /// </summary>
        /// <returns>启动线程是否成功</returns>
        public bool Start()
        {
            //结束上一次的线程

            SocketThreads = new SocketThread[MaxThread];
            for (int i = 0; i < MaxThread; i++)
            {
                //if (!VerifyBase.meterInfo[i].YaoJianYn) continue;
                SocketThread newThread = new SocketThread()
                {
                    ThreadID = i + 1,                      //线程编号,用于线程自己推导起始位置
                    TaskCount = MaxTaskCountPerThread,
                    DoWork = this.DoWork
                };
                SocketThreads[i] = newThread;
                newThread.Start();
                System.Threading.Thread.Sleep(50);
            }
            return true;
        }

        /// <summary>
        /// 停止所有工作线程
        /// </summary>
        public void Stop()
        {
            //首先发出停止指令
            foreach (SocketThread SocketThread in SocketThreads)
            {
                SocketThread.Stop();
            }
            //等待所有工作线程都完成
            bool isAllThreaWorkDone = false;
            while (!isAllThreaWorkDone)
            {
                isAllThreaWorkDone = IsWorkDone();
            }

        }

        /// <summary>
        /// 等待所有线程工作完成
        /// </summary>
        public bool IsWorkDone()
        {
            bool isAllThreaWorkDone = true;

            foreach (SocketThread SocketThread in SocketThreads)
            {
                if (SocketThread == null)
                    continue;
                isAllThreaWorkDone = SocketThread.IsWorkFinshed;
                if (!isAllThreaWorkDone) break;
            }
            if (isAllThreaWorkDone)
            {
                //Comm.GlobalUnit.g_MsgControl.OutMessage("当前操作已经完成!", false);
                //所有操作完成后，关闭一下485端口，如果不关，标准表读取线程将无法读取标准表
                bool[] isOpen = new bool[VerifyBase.meterInfo.Length];
                //Helper.EquipHelper.Instance.InitPara_CommTest(isOpen); 
            }
            return isAllThreaWorkDone;
        }
    }
}
