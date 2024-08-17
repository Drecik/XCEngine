using System.Reflection;

namespace XCEngine.Server
{
    public static class Actor
    {
        /// <summary>
        /// 当前运行的ActorId
        /// </summary>
        public static ThreadLocal<int> ActorId = new();

        /// <summary>
        /// Actor Id生成
        /// </summary>
        private static IIdGenerator _actorIdGenerator = new ThreadSafeIdGenerator();

        /// <summary>
        /// 所有ActorContext集合
        /// </summary>
        private static Dictionary<int, ActorContext> _actorContextDict = new();

        /// <summary>
        /// Actor消息处理句柄
        /// </summary>
        private static Dictionary<Type, Action<object, ActorMessage>> _actorMessageHandlerDict = new();

        /// <summary>
        /// Message序列化对象
        /// </summary>
        public static ISerializer MessageSerializer = new MemoryPackSerializer();

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        internal static bool Initialize()
        {
            if (InitializeMessageHandlerDict() == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 初始化消息句柄字典
        /// </summary>
        /// <returns></returns>
        internal static bool InitializeMessageHandlerDict()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    Type type = types[i];
                    var attribute = type.GetCustomAttribute<ActorMessageHandlerAttribute>();
                    if (attribute != null)
                    {
                        var handler = Activator.CreateInstance(type) as IActorMessageHandler;
                        if (handler == null)
                        {
                            Log.Error($"Invalid actor message handler type: {type.Name}, need to implement IActorMessageHandler");
                            return false;
                        }

                        _actorMessageHandlerDict[attribute.ActorType] = handler.OnMessage;
                    }
                }
            }

            return true;
        }

        #endregion

        /// <summary>
        /// 获取Actor Context
        /// </summary>
        /// <param name="actorId"></param>
        /// <returns></returns>
        internal static ActorContext GetActorContext(int actorId)
        {
            lock (_actorContextDict)
            {
                if (_actorContextDict.TryGetValue(actorId, out ActorContext actorContext))
                {
                    return actorContext;
                }
                return null;
            }
        }

        /// <summary>
        /// 获取Actor Context数量
        /// </summary>
        /// <returns></returns>
        internal static int GetActorContextCount()
        {
            return _actorContextDict.Count;
        }

        #region Actor操作

        #region Start

        /// <summary>
        /// 启动Actor
        /// </summary>
        /// <typeparam name="T">actor类型</typeparam>
        /// <param name="param">创建参数</param>
        /// <returns></returns>
        public static int StartWithRaw<ActorType>(object data) where ActorType : class, new()
        {
            return StartWithRaw(typeof(ActorType), data);
        }

        public static int Start<ActorType>() where ActorType : class, new()
        {
            return StartWithRaw<ActorType>(null);
        }

        public static int Start<ActorType, T>(T param) where ActorType : class, new()
        {
            return StartWithRaw<ActorType>(MessageSerializer.Serialize(param));
        }

        public static int Start<ActorType, T1, T2>(T1 param1, T2 param2) where ActorType : class, new()
        {
            return StartWithRaw<ActorType>(MessageSerializer.Serialize(param1, param2));
        }

        public static int Start<ActorType, T1, T2, T3>(T1 param1, T2 param2, T3 param3) where ActorType : class, new()
        {
            return StartWithRaw<ActorType>(MessageSerializer.Serialize(param1, param2, param3));
        }

        public static int Start<ActorType, T1, T2, T3, T4>(T1 param1, T2 param2, T3 param3, T4 param4) where ActorType : class, new()
        {
            return StartWithRaw<ActorType>(MessageSerializer.Serialize(param1, param2, param3, param4));
        }

        public static int Start<ActorType, T1, T2, T3, T4, T5>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) where ActorType : class, new()
        {
            return StartWithRaw<ActorType>(MessageSerializer.Serialize(param1, param2, param3, param4, param5));
        }

        public static int StartWithRaw(Type type, object data)
        {
            if (_actorMessageHandlerDict.TryGetValue(type, out var handler) == false)
            {
                Log.Warning($"{type.Name} has no actor message handler");
            }

            object actor = Activator.CreateInstance(type);
            int actorId = _actorIdGenerator.GenerateId();
            var actorContext = new ActorContext()
            {
                ActorId = actorId,
                ActorObject = actor,
                MessageHandler = handler,
                ActorMessageQueue = new ActorMessageQueue() { ActorId = actorId }
            };

            lock (_actorContextDict)
            {
                _actorContextDict.Add(actorId, actorContext);
            }

            Log.Info($"Start actor: {type.Name}[{actorId}]");

            var msg = new ActorMessage()
            {
                MessageType = ActorMessage.EMessageType.Create,
                From = ActorId.Value,
                To = actorId,
                MessageData = data
            };

            actorContext.ActorMessageQueue.PushMessage(msg);

            return actorId;
        }

        public static int Start(Type type)
        {
            return StartWithRaw(type, null);
        }

        public static int Start<T>(Type type, T param)
        {
            return StartWithRaw(type, MessageSerializer.Serialize(param));
        }

        public static int Start<T1, T2>(Type type, T1 param1, T2 param2)
        {
            return StartWithRaw(type, MessageSerializer.Serialize(param1, param2));
        }

        public static int Start<T1, T2, T3>(Type type, T1 param1, T2 param2, T3 param3)
        {
            return StartWithRaw(type, MessageSerializer.Serialize(param1, param2, param3));
        }

        public static int Start<T1, T2, T3, T4>(Type type, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            return StartWithRaw(type, MessageSerializer.Serialize(param1, param2, param3, param4));
        }

        public static int Start<T1, T2, T3, T4, T5>(Type type, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            return StartWithRaw(type, MessageSerializer.Serialize(param1, param2, param3, param4, param5));
        }

        #endregion

        /// <summary>
        /// 杀死其他Actor
        /// </summary>
        /// <param name="actorId"></param>
        public static void Kill(int actorId)
        {
            Log.Info($"Kill actor {actorId}");

            var actorContext = GetActorContext(actorId);
            if (actorContext == null)
            {
                Log.Info($"Actor: {actorId} not exist.");
                return;
            }

            var msg = new ActorMessage()
            {
                MessageType = ActorMessage.EMessageType.Destroy,
                From = ActorId.Value,
                To = actorId
            };

            actorContext.ActorMessageQueue.PushMessage(msg);
        }

        /// <summary>
        /// 将自己退出
        /// </summary>
        public static void Exit()
        {
            int actorId = ActorId.Value;
            Log.Info($"Exit actor {actorId}");

            var actorContext = GetActorContext(actorId);
            if (actorContext == null)
            {
                Log.Info($"Actor: {actorId} not exist.");
                return;
            }

            var msg = new ActorMessage()
            {
                MessageType = ActorMessage.EMessageType.Destroy,
                From = ActorId.Value,
                To = actorId
            };

            actorContext.ActorMessageQueue.PushMessage(msg);
        }

        #region Send

        /// <summary>
        /// 向Actor以Send形式发送消息
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="param"></param>
        public static void SendWithRaw(int actorId, string messageId, object data)
        {
            var actorContext = GetActorContext(actorId);
            if (actorContext == null)
            {
                Log.Info($"Actor: {actorId} not exist.");
                return;
            }

            var msg = new ActorMessage()
            {
                MessageType = ActorMessage.EMessageType.Send,
                From = ActorId.Value,
                To = actorId,
                MessageId = messageId,
                MessageData = data
            };

            actorContext.ActorMessageQueue.PushMessage(msg);
        }

        public static void Send(int actorId, string messageId)
        {
            SendWithRaw(actorId, messageId, null);
        }

        public static void Send<T>(int actorId, string messageId, T param)
        {
            SendWithRaw(actorId, messageId, MessageSerializer.Serialize(param));
        }

        public static void Send<T1, T2>(int actorId, string messageId, T1 param1, T2 param2)
        {
            SendWithRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2));
        }

        public static void Send<T1, T2, T3>(int actorId, string messageId, T1 param1, T2 param2, T3 param3)
        {
            SendWithRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3));
        }

        public static void Send<T1, T2, T3, T4>(int actorId, string messageId, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            SendWithRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3, param4));
        }

        public static void Send<T1, T2, T3, T4, T5>(int actorId, string messageId, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            SendWithRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3, param4, param5));
        }

        #endregion

        ///// <summary>
        ///// 向Actor以Call形式发送消息
        ///// </summary>
        ///// <param name="actorId"></param>
        ///// <param name="param"></param>
        ///// <returns></returns>
        //public static Task Call(int actorId, string messageId, byte[] data = null)
        //{
        //    var actorContext = GetActorContext(actorId);
        //    if (actorContext == null)
        //    {
        //        Log.Info($"Actor: {actorId} not exist.");
        //        return Task.CompletedTask;
        //    }

        //    var tcs = new TaskCompletionSource();
        //    var msg = new ActorMessage()
        //    {
        //        MessageType = ActorMessage.EMessageType.Send,
        //        From = ActorId.Value,
        //        To = actorId,
        //        MessageId = messageId,
        //        MessageData = data,
        //        MessageReplyTcs = tcs
        //    };

        //    actorContext.ActorMessageQueue.PushMessage(msg);
        //    return tcs.Task;
        //}

        #endregion
    }
}
