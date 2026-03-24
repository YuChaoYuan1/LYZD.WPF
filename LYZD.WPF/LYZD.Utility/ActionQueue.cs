using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LYZD.Utility
{
    /// <summary>
    /// 执行一系列动作线程的封装
    /// </summary>
    public class ActionQueue
    {
        public ActionQueue()
        {
            new Task(() => ExecuteProcess()).Start();
        }
        private AutoResetEvent waitHandle = new AutoResetEvent(false);
        private Queue<Action> actionQueue = new Queue<Action>();
        public void AddMessage(Action action)
        {
            actionQueue.Enqueue(action);
            waitHandle.Set();
        }
        /// <summary>
        /// 在此处等待或执行队列中的动作
        /// </summary>
        private void ExecuteProcess()
        {
            while (true)
            {
                if (actionQueue.Count > 0)
                {
                    try
                    {
                        Action action = actionQueue.Dequeue();
                        if (action != null)
                        {
                            action.Invoke();
                        }
                        action = null;
                    }
                    catch (Exception e)
                    {
                        //【标注】先注释
                        Log.LogManager.AddMessage(e.Message, Log.EnumLogSource.用户操作日志, Log.EnumLevel.Error, e);
                    }
                }
                else
                {
                    waitHandle.Reset();
                    waitHandle.WaitOne();
                }
            }
        }
    }
}
