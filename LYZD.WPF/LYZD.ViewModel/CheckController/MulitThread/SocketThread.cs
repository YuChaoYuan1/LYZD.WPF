using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LYZD.ViewModel.CheckController.MulitThread
{
    class SocketThread
    {
        Thread socketThread = null;

        /// <summary>
        /// 运行标志
        /// </summary>
        private bool m_bol_IsRun = false;

        /// <summary>
        /// 工作完成标志
        /// </summary>
        private bool m_bol_IsWorkFinished = false;

        /// <summary>
        /// 线程编号
        /// </summary>
        public int ThreadID { get; set; }

        /// <summary>
        /// 任务数量
        /// </summary>
        public int TaskCount { get; set; }

        public Action<int> DoWork { get; set; }

        /// <summary>
        /// 停止当前工作任务
        /// </summary>
        public void Stop()
        {
            m_bol_IsRun = false;
        }

        public bool IsWorkFinshed
        {
            get { return m_bol_IsWorkFinished; }
        }

        /// <summary>
        /// 启动工作线程
        /// </summary>
        public void Start()
        {
            socketThread = new Thread(StartWork);
            socketThread.Start();
        }

        private void StartWork()
        {
            // 初始化标志
            m_bol_IsRun = true;
            m_bol_IsWorkFinished = false;

            //计算负载
            int startpos = (ThreadID - 1) * TaskCount;
            int endpos = startpos + TaskCount;

            //调用方法
            try
            {
                //bool[] isOpen = new bool[Adapter.Instance.BwCount];
                for (int i = startpos; i < endpos; i++)
                {
                    if (!VerifyBase.meterInfo[i].YaoJianYn) continue;
                   // TODO 232通讯部分
                    //if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.Channel232 || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelMaintain)
                    //{
                    //    EquipmentData.DeviceManager.ControlConnrRelay(1, (byte)(i + 1), ThreadID - 1); //切到232 --452
                    //}
                    if (DoWork != null)
                    {
                        DoWork(i);
                    }
                    //if (VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.Channel232 || VerifyBase.TerminalChannelType == Core.Enum.Cus_EmChannelType.ChannelMaintain)
                    //{
                    //    EquipmentData.DeviceManager.ControlConnrRelay(0, (byte)(i + 1), ThreadID - 1); //切回485--232
                    //}
                    //if (!m_bol_IsRun)
                    //    break;
                }
            }
            catch { }
            finally
            {
                //恢复标志
                m_bol_IsRun = false;
                m_bol_IsWorkFinished = true;
            }
        }

    }
}
