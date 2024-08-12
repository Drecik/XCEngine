using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCEngine.Server
{
    /// <summary>
    /// 工作线程封装
    /// </summary>
    internal class WorkThread
    {
        /// <summary>
        /// 内部定义的工作线程Id
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 是否停止
        /// </summary>
        private bool _stop = false;

        /// <summary>
        /// 内部线程
        /// </summary>
        Thread _thread = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">内部定义的工作线程Id</param>
        public WorkThread(int id)
        {
            Id = id;
        }

        /// <summary>
        /// 启动工作线程
        /// </summary>
        public void Start()
        {
            if (_thread != null)
            {
                Log.Error($"WorkThread[{Id}] Already Started.");
                return;
            }

            _stop = false;
            _thread = new Thread(() => Run());
            _thread.Name = $"WorkThread[{Id}:D2]";
            _thread.Start();
        }

        void Run()
        {
            WorkThreadContext.WorkId.Value = Id;

            while (_stop == false)
            {
                lock (this)
                {
                    //if (_messageList.Count == 0)
                    //{
                    //    // 1秒返回检查下停止标记
                    //    Monitor.Wait(this, 1000, true);
                    //}

                    //if (_messageList.Count == 0)
                    //{
                    //    continue;
                    //}

                    //CommonUtils.Swap(ref _messageList, ref _tempMessageList);
                    // TODO
                }

                //for (int i = 0; i < _tempMessageList.Count; ++i)
                //{
                //    ExecuteMessage(_tempMessageList[i]);
                //}

                //_tempMessageList.Clear();
            }
        }

        ///// <summary>
        ///// 执行EntityMessage
        ///// </summary>
        ///// <param name="message"></param>
        //void ExecuteMessage(EntityMessage message)
        //{
        //    System.Threading.SynchronizationContext.SetSynchronizationContext(new SynchronizationContext(_id, message.EntityId));
        //    WorkThreadContext.CurrentEntityId = message.EntityId;

        //    EntityManager.Instance.ExecuteMessage(message);
        //}

        ///// <summary>
        ///// 像这个线程发送Entity消息
        ///// </summary>
        ///// <param name="message"></param>
        //public void SendMessage(EntityMessage message)
        //{
        //    lock (this)
        //    {
        //        _messageList.Add(message);
        //        Monitor.Pulse(this);
        //    }
        //}

        /// <summary>
        /// 停止线程
        /// </summary>
        public void Stop()
        {
            _stop = true;
            _thread?.Join();
        }
    }
}
