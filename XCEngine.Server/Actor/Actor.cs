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
        /// 模板方式启动Actor
        /// </summary>
        /// <typeparam name="ActorType">actor类型</typeparam>
        /// <param name="data">创建参数（Actor之间内存隔离，这里应该是序列化过的数据）</param>
        /// <returns>actor id</returns>
        public static int StartWithRaw<ActorType>(object data) where ActorType : class, new()
        {
            return StartWithRaw(typeof(ActorType), data);
        }

        /// <summary>
        /// 模板方式直接启动Actor
        /// </summary>
        /// <typeparam name="ActorType">actor类型</typeparam>
        /// <returns>actor id</returns>
        public static int Start<ActorType>() where ActorType : class, new()
        {
            return StartWithRaw<ActorType>(null);
        }

        /// <summary>
        /// 以模板方式的带有参数的方式启动Actor，参数不用序列化，会自动使用MessageSerializer进行序列化
        /// </summary>
        /// <typeparam name="ActorType">actor类型</typeparam>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="param">参数</param>
        /// <returns>actor id</returns>
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

        /// <summary>
        /// 用Type传参形式启动Actor
        /// </summary>
        /// <param name="type">actor type</param>
        /// <param name="data">创建参数（Actor之间内存隔离，这里应该是序列化过的数据）</param>
        /// <returns></returns>
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

        /// <summary>
        /// 用Type传参形式直接启动Actor
        /// </summary>
        /// <param name="type">actor type</typeparam>
        /// <returns>actor id</returns>
        public static int Start(Type type)
        {
            return StartWithRaw(type, null);
        }

        /// <summary>
        /// 用Type传参方式的带有参数的方式启动Actor，参数不用序列化，会自动使用MessageSerializer进行序列化
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="type">actor type</typeparam>
        /// <param name="param">参数</param>
        /// <returns>actor id</returns>
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
            Log.Info($"Kill actor[{actorId}]");

            var actorContext = GetActorContext(actorId);
            if (actorContext == null)
            {
                Log.Info($"Actor[{actorId}] not exist.");
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
            Log.Info($"Exit actor[{actorId}]");

            var actorContext = GetActorContext(actorId);
            if (actorContext == null)
            {
                Log.Info($"Actor[{actorId}] not exist.");
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
        /// <param name="actorId">接收消息的actor id</param>
        /// <param name="messageId">消息id</param>
        /// <param name="data">消息数据（Actor之间内存隔离，这里应该是序列化过的数据）</param>
        public static void SendWithRaw(int actorId, string messageId, object data)
        {
            var actorContext = GetActorContext(actorId);
            if (actorContext == null)
            {
                Log.Info($"Actor[{actorId}] not exist.");
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

        /// <summary>
        /// 向Actor以Send形式发送无参数消息
        /// </summary>
        /// <param name="actorId">接收消息的actor id</param>
        /// <param name="messageId">消息id</param>
        public static void Send(int actorId, string messageId)
        {
            SendWithRaw(actorId, messageId, null);
        }

        /// <summary>
        /// 向Actor以Send形式发送带参数的消息，参数不用序列化，会自动使用MessageSerializer进行序列化
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="actorId">接收消息的actor id</param>
        /// <param name="messageId">消息id</param>
        /// <param name="param">参数</param>
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

        //#region Call

        ///// <summary>
        ///// 向Actor以Call形式发送消息
        ///// 返回的数据应该也是需要经过序列化的数据
        ///// </summary>
        ///// <param name="actorId">接收消息的actor</param>
        ///// <param name="messageId">消息id</param>
        ///// <param name="data">消息参数（Actor之间内存隔离，这里应该是序列化过的数据）</param>
        ///// <returns>返回Task</returns>
        //public static async Task<object> CallWithRaw(int actorId, string messageId, object data)
        //{
        //    var actorContext = GetActorContext(actorId);
        //    if (actorContext == null)
        //    {
        //        Log.Info($"Actor: {actorId} not exist.");
        //        return null;
        //    }

        //    var tcs = new TaskCompletionSource<object>();
        //    var msg = new ActorMessage()
        //    {
        //        MessageType = ActorMessage.EMessageType.Call,
        //        From = ActorId.Value,
        //        To = actorId,
        //        MessageId = messageId,
        //        MessageData = data,
        //        MessageReplyTcs = tcs
        //    };

        //    actorContext.ActorMessageQueue.PushMessage(msg);

        //    return await tcs.Task;
        //}

        ///// <summary>
        ///// 向Actor以Call形式发送消息，需要指定返回参数类型
        ///// 返回的数据应该也是需要经过序列化的数据
        ///// </summary>
        ///// <typeparam name="RetType">返回的参数类型</typeparam>
        ///// <param name="actorId">接收消息的actor</param>
        ///// <param name="messageId">消息id</param>
        ///// <param name="data">消息参数（Actor之间内存隔离，这里应该是序列化过的数据）</param>
        ///// <returns>返回Task<RetType></returns>
        //public static async Task<RetType> CallWithRaw<RetType>(int actorId, string messageId, object data)
        //{
        //    var actorContext = GetActorContext(actorId);
        //    if (actorContext == null)
        //    {
        //        Log.Info($"Actor: {actorId} not exist.");
        //        return default;
        //    }

        //    var tcs = new TaskCompletionSource<object>();
        //    var msg = new ActorMessage()
        //    {
        //        MessageType = ActorMessage.EMessageType.Call,
        //        From = ActorId.Value,
        //        To = actorId,
        //        MessageId = messageId,
        //        MessageData = data,
        //        MessageReplyTcs = tcs
        //    };

        //    actorContext.ActorMessageQueue.PushMessage(msg);

        //    var ret = await tcs.Task;
        //    return MessageSerializer.Deserialize<RetType>(ret as byte[]);
        //}

        //#endregion

        /// <summary>
        /// 向指定Actor发送原始Message
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="message"></param>
        internal static void PushMessage(int actorId, ActorMessage message)
        {
            var actorContext = GetActorContext(actorId);
            if (actorContext == null)
            {
                return;
            }

            actorContext.ActorMessageQueue.PushMessage(message);
        }

        #endregion
    }
}
