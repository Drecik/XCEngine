using System.Reflection;

namespace XCEngine.Server
{
    public static class Actor
    {
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
        public static int StartRaw<ActorType>(object data) where ActorType : class, new()
        {
            return StartRaw(typeof(ActorType), data);
        }

        /// <summary>
        /// 模板方式直接启动Actor
        /// </summary>
        /// <typeparam name="ActorType">actor类型</typeparam>
        /// <returns>actor id</returns>
        public static int Start<ActorType>() where ActorType : class, new()
        {
            return StartRaw<ActorType>(null);
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
            return StartRaw<ActorType>(MessageSerializer.Serialize(param));
        }

        public static int Start<ActorType, T1, T2>(T1 param1, T2 param2) where ActorType : class, new()
        {
            return StartRaw<ActorType>(MessageSerializer.Serialize(param1, param2));
        }

        public static int Start<ActorType, T1, T2, T3>(T1 param1, T2 param2, T3 param3) where ActorType : class, new()
        {
            return StartRaw<ActorType>(MessageSerializer.Serialize(param1, param2, param3));
        }

        public static int Start<ActorType, T1, T2, T3, T4>(T1 param1, T2 param2, T3 param3, T4 param4) where ActorType : class, new()
        {
            return StartRaw<ActorType>(MessageSerializer.Serialize(param1, param2, param3, param4));
        }

        public static int Start<ActorType, T1, T2, T3, T4, T5>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) where ActorType : class, new()
        {
            return StartRaw<ActorType>(MessageSerializer.Serialize(param1, param2, param3, param4, param5));
        }

        /// <summary>
        /// 用Type传参形式启动Actor
        /// </summary>
        /// <param name="type">actor type</param>
        /// <param name="data">创建参数（Actor之间内存隔离，这里应该是序列化过的数据）</param>
        /// <returns></returns>
        public static int StartRaw(Type type, object data)
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
                From = Self(),
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
            return StartRaw(type, null);
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
            return StartRaw(type, MessageSerializer.Serialize(param));
        }

        public static int Start<T1, T2>(Type type, T1 param1, T2 param2)
        {
            return StartRaw(type, MessageSerializer.Serialize(param1, param2));
        }

        public static int Start<T1, T2, T3>(Type type, T1 param1, T2 param2, T3 param3)
        {
            return StartRaw(type, MessageSerializer.Serialize(param1, param2, param3));
        }

        public static int Start<T1, T2, T3, T4>(Type type, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            return StartRaw(type, MessageSerializer.Serialize(param1, param2, param3, param4));
        }

        public static int Start<T1, T2, T3, T4, T5>(Type type, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            return StartRaw(type, MessageSerializer.Serialize(param1, param2, param3, param4, param5));
        }

        #endregion

        /// <summary>
        /// 获取自己的ActorId
        /// </summary>
        /// <returns></returns>
        public static int Self()
        {
            return (SynchronizationContext.Current as SynchronizationContext)?.ActorId ?? 0;
        }

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
                From = Self(),
            };

            actorContext.ActorMessageQueue.PushMessage(msg);
        }

        /// <summary>
        /// 将自己退出
        /// </summary>
        public static void Exit()
        {
            int actorId = Self();
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
                From = Self(),
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
        public static void SendRaw(int actorId, string messageId, object data)
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
                From = Self(),
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
            SendRaw(actorId, messageId, null);
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
            SendRaw(actorId, messageId, MessageSerializer.Serialize(param));
        }

        public static void Send<T1, T2>(int actorId, string messageId, T1 param1, T2 param2)
        {
            SendRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2));
        }

        public static void Send<T1, T2, T3>(int actorId, string messageId, T1 param1, T2 param2, T3 param3)
        {
            SendRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3));
        }

        public static void Send<T1, T2, T3, T4>(int actorId, string messageId, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            SendRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3, param4));
        }

        public static void Send<T1, T2, T3, T4, T5>(int actorId, string messageId, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            SendRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3, param4, param5));
        }

        #endregion

        #region Call

        /// <summary>
        /// 向Actor以Call形式发送消息
        /// 返回的数据应该也是需要经过序列化的数据
        /// </summary>
        /// <param name="actorId">接收消息的actor</param>
        /// <param name="messageId">消息id</param>
        /// <param name="data">消息参数（Actor之间内存隔离，这里应该是序列化过的数据）</param>
        /// <returns>返回的序列化过的数据</returns>
        public static Task<object> CallRaw(int actorId, string messageId, object data)
        {
            return ActorCaller.CallRaw(actorId, messageId, data);
        }

        #region Call Templates

        #region Template1

        /// <summary>
        /// 向Actor以Call形式发送消息
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public static async Task Call(int actorId, string messageId)
        {
            await CallRaw(actorId, messageId, null);
        }

        /// <summary>
        /// 向Actor以Call形式发送带参数的消息，参数不用序列化，会自动使用MessageSerializer进行序列化
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="actorId">接收消息的actor id</param>
        /// <param name="messageId">消息id</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public static Task Call<T>(int actorId, string messageId, T param)
        {
            return CallRaw(actorId, messageId, MessageSerializer.Serialize(param));
        }

        public static Task Call<T1, T2>(int actorId, string messageId, T1 param1, T2 param2)
        {
            return CallRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2));
        }

        public static Task Call<T1, T2, T3>(int actorId, string messageId, T1 param1, T2 param2, T3 param3)
        {
            return CallRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3));
        }

        public static Task Call<T1, T2, T3, T4>(int actorId, string messageId, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            return CallRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3, param4));
        }

        public static Task Call<T1, T2, T3, T4, T5>(int actorId, string messageId, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            return CallRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3, param4, param5));
        }

        #endregion

        #region Template2

        /// <summary>
        /// 向Actor以Call形式发送消息，需要指定返回参数类型
        /// </summary>
        /// <typeparam name="RetType">返回的参数类型</typeparam>
        /// <param name="actorId">接收消息的actor</param>
        /// <param name="messageId">消息id</param>
        /// <returns>反序列化号的RetType值</returns>
        public static async Task<RetType> Call<RetType>(int actorId, string messageId)
        {
            var ret = await CallRaw(actorId, messageId, null);
            return MessageSerializer.Deserialize<RetType>(ret as byte[]);
        }

        /// <summary>
        /// 向Actor以Call形式发送消息，需要指定返回参数类型，参数不用序列化，会自动使用MessageSerializer进行序列化
        /// </summary>
        /// <typeparam name="RetType">返回的参数类型</typeparam>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="actorId">接受消息的actor</param>
        /// <param name="messageId">消息id</param>
        /// <param name="param">参数</param>
        /// <returns>反序列化号的RetType值</returns>
        public static async Task<RetType> Call<RetType, T>(int actorId, string messageId, T param)
        {
            var ret = await CallRaw(actorId, messageId, MessageSerializer.Serialize(param));
            return MessageSerializer.Deserialize<RetType>(ret as byte[]);
        }

        public static async Task<RetType> Call<RetType, T1, T2>(int actorId, string messageId, T1 param1, T2 param2)
        {
            var ret = await CallRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2));
            return MessageSerializer.Deserialize<RetType>(ret as byte[]);
        }

        public static async Task<RetType> Call<RetType, T1, T2, T3>(int actorId, string messageId, T1 param1, T2 param2, T3 param3)
        {
            var ret = await CallRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3));
            return MessageSerializer.Deserialize<RetType>(ret as byte[]);
        }

        public static async Task<RetType> Call<RetType, T1, T2, T3, T4>(int actorId, string messageId, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            var ret = await CallRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3, param4));
            return MessageSerializer.Deserialize<RetType>(ret as byte[]);
        }

        public static async Task<RetType> Call<RetType, T1, T2, T3, T4, T5>(int actorId, string messageId, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            var ret = await CallRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3, param4, param5));
            return MessageSerializer.Deserialize<RetType>(ret as byte[]);
        }

        #endregion

        #region Template3

        /// <summary>
        /// 向Actor以Call形式发送消息，需要指定返回参数类型
        /// </summary>
        /// <param name="retType">返回的参数类型</param>
        /// <param name="actorId">接收消息的actor</param>
        /// <param name="messageId">消息id</param>
        /// <returns>反序列化号的RetType值</returns>
        public static async Task<object> Call(Type retType, int actorId, string messageId)
        {
            var ret = await CallRaw(actorId, messageId, null);
            return MessageSerializer.Deserialize(retType, ret as byte[]);
        }

        /// <summary>
        /// 向Actor以Call形式发送消息，需要指定返回参数类型，参数不用序列化，会自动使用MessageSerializer进行序列化
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="retType">返回的参数类型</param>
        /// <param name="actorId">接受消息的actor</param>
        /// <param name="messageId">消息id</param>
        /// <param name="param">参数</param>
        /// <returns>反序列化号的RetType值</returns>
        public static async Task<object> Call<T>(Type retType, int actorId, string messageId, T param)
        {
            var ret = await CallRaw(actorId, messageId, MessageSerializer.Serialize(param));
            return MessageSerializer.Deserialize(retType, ret as byte[]);
        }

        public static async Task<object> Call<T1, T2>(Type retType, int actorId, string messageId, T1 param1, T2 param2)
        {
            var ret = await CallRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2));
            return MessageSerializer.Deserialize(retType, ret as byte[]);
        }

        public static async Task<object> Call<T1, T2, T3>(Type retType, int actorId, string messageId, T1 param1, T2 param2, T3 param3)
        {
            var ret = await CallRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3));
            return MessageSerializer.Deserialize(retType, ret as byte[]);
        }

        public static async Task<object> Call<T1, T2, T3, T4>(Type retType, int actorId, string messageId, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            var ret = await CallRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3, param4));
            return MessageSerializer.Deserialize(retType, ret as byte[]);
        }

        public static async Task<object> Call<T1, T2, T3, T4, T5>(Type retType, int actorId, string messageId, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            var ret = await CallRaw(actorId, messageId, MessageSerializer.Serialize(param1, param2, param3, param4, param5));
            return MessageSerializer.Deserialize(retType, ret as byte[]);
        }

        #endregion

        #endregion

        #endregion

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

        /// <summary>
        /// Call的情况返回数据
        /// </summary>
        /// <param name="data">返回的数据（Actor之间内存隔离，这里应该是序列化过的数据）</param>
        public static void Return(object data)
        {
            int sessionId = (SynchronizationContext.Current as SynchronizationContext).SessionId;
            if (sessionId == 0)
            {
                return;
            }

            ActorCaller.OnReply(sessionId, data);
        }

        /// <summary>
        /// 派发消息
        /// </summary>
        /// <param name="actorContext"></param>
        /// <param name="message"></param>
        internal static void DispatchMessage(ActorContext actorContext, ActorMessage message)
        {
            try
            {
                if (message.MessageType == ActorMessage.EMessageType.AwaitCallback)
                {
                    // Await Callback内部处理了
                    object[] args = (object[])message.MessageData;
                    if (args == null)
                    {
                        return;
                    }

                    SendOrPostCallback callback = args[0] as SendOrPostCallback;
                    object state = args[1];
                    callback.Invoke(state);
                    return;
                }

                // 其他消息执行自定义回调里面执行
                actorContext.MessageHandler?.Invoke(actorContext.ActorObject, message);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        #endregion
    }
}
