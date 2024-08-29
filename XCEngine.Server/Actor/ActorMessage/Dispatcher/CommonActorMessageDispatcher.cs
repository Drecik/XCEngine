using System.Reflection;
using System.Runtime.CompilerServices;

namespace XCEngine.Server
{
    public class CommonActorMessageDispatcher : IActorMessageDispatcher
    {
        protected Type _actorType;

        /// <summary>
        /// Message方法信息
        /// </summary>
        class MessageMethodInfo
        {
            /// <summary>
            /// 消息Id
            /// </summary>
            public string MessageId;

            /// <summary>
            /// 是否异步方法
            /// </summary>
            public bool IsAsync;

            /// <summary>
            /// 方法入口
            /// </summary>
            public MethodInfo Method;
        }

        private Dictionary<string, MessageMethodInfo> _messageMethodInfoDict = new();

        public CommonActorMessageDispatcher(Type actorType)
        {
            _actorType = actorType;

            // 查找所有该Actor的消息方法

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().AsEnumerable();
            if (ServerConfig.GetConfig("EnableHotfix", false) == true)
            {
                assemblies = Hotfix.ModelHotfixDllList;
            }

            // TODO：可以优化下，先全局遍历一遍，再按类型拿
            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; ++i)
                {
                    Type type = types[i];
                    var attribute = type.GetCustomAttribute<ActorMessageHandlerAttribute>();
                    if (attribute != null && attribute.ActorType == _actorType)
                    {
                        foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                        {
                            var methodAttr = method.GetCustomAttribute<ActorMessageHandlerMethodAttribute>();
                            if (methodAttr != null)
                            {
                                var methodInfo = new MessageMethodInfo();
                                methodInfo.MessageId = methodAttr.MessageId;
                                methodInfo.IsAsync = method.GetCustomAttribute<AsyncStateMachineAttribute>() != null || method.ReturnType.IsSubclassOf(typeof(Task));
                                methodInfo.Method = method;
                                _messageMethodInfoDict.Add(methodInfo.MessageId, methodInfo);
                            }
                        }
                    }
                }
            }
        }

        public async void OnMessage(object actor, ActorMessage actorMessage)
        {
            if (actorMessage.MessageType == ActorMessage.EMessageType.Create)
            {
                actorMessage.MessageId = "OnCreate";
            }
            else if (actorMessage.MessageType == ActorMessage.EMessageType.Destroy)
            {
                actorMessage.MessageId = "OnDestroy";
            }

            if (_messageMethodInfoDict.TryGetValue(actorMessage.MessageId, out var methodInfo))
            {
                if (actorMessage.MessageType == ActorMessage.EMessageType.System)
                {
                    // 系统消息无需反序列化
                    var systemArgs = actorMessage.MessageData as object[];
                    var args = new object[1 + systemArgs.Length];
                    for (int i = 0; i < systemArgs.Length; ++i)
                    {
                        args[i + 1] = systemArgs[i];
                    }
                    args[0] = actor;
                    methodInfo.Method.Invoke(null, args);
                }
                else
                {
                    object ret = await InvokeWithArgs(actor, methodInfo, GetParameters(actor, methodInfo, actorMessage.MessageData));
                    if (actorMessage.MessageType == ActorMessage.EMessageType.Call)
                    {
                        Actor.Return(Actor.MessageSerializer.Serialize(ret.GetType(), ret));
                    }
                }
            }
            else
            {
                if (actorMessage.MessageType != ActorMessage.EMessageType.Create && actorMessage.MessageType != ActorMessage.EMessageType.Destroy)
                {
                    Log.Error($"Actor: {_actorType.Name} receive invalid message id: {actorMessage.MessageId}");
                }
            }
        }

        async Task<object> InvokeWithArgs(object actor, MessageMethodInfo methodInfo, object[] args)
        {
            if (methodInfo.IsAsync)
            {
                return await methodInfo.Method.InvokeAsync(null, args);
            }
            else
            {
                return methodInfo.Method.Invoke(null, args);
            }
        }

        object[] GetParameters(object actor, MessageMethodInfo methodInfo, object messageData)
        {
            var methodParameters = methodInfo.Method.GetParameters();
            if (methodParameters.Length == 1)
            {
                return new object[1] { actor };
            }
            else if (methodParameters.Length == 2)
            {
                var data = messageData as byte[];
                object param = Actor.MessageSerializer.Deserialize(methodParameters[1].ParameterType, data);
                return new object[2] { actor, param };
            }
            else if (methodParameters.Length == 3)
            {
                var data = messageData as byte[];
                var (param1, param2) = Actor.MessageSerializer.Deserialize(methodParameters[1].ParameterType, methodParameters[2].ParameterType, data);
                return new object[3] { actor, param1, param2 };
            }
            else if (methodParameters.Length == 4)
            {
                var data = messageData as byte[];
                var (param1, param2, param3) = Actor.MessageSerializer.Deserialize(methodParameters[1].ParameterType, methodParameters[2].ParameterType, methodParameters[3].ParameterType, data);
                return new object[4] { actor, param1, param2, param3 };
            }
            else if (methodParameters.Length == 5)
            {
                var data = messageData as byte[];
                var (param1, param2, param3, param4) = Actor.MessageSerializer.Deserialize(methodParameters[1].ParameterType, methodParameters[2].ParameterType, methodParameters[3].ParameterType, methodParameters[4].ParameterType, data);
                return new object[5] { actor, param1, param2, param3, param4 };
            }
            else if (methodParameters.Length == 6)
            {
                var data = messageData as byte[];
                var (param1, param2, param3, param4, param5) = Actor.MessageSerializer.Deserialize(methodParameters[1].ParameterType, methodParameters[2].ParameterType, methodParameters[3].ParameterType, methodParameters[4].ParameterType, methodParameters[5].ParameterType, data);
                return new object[6] { actor, param1, param2, param3, param4, param5 };
            }
            else
            {
                throw new Exception("方法不支持5个以上参数");
            }
        }
    }

    public class CommonActorMessageDispatcher<T> : CommonActorMessageDispatcher
    {
        public CommonActorMessageDispatcher()
             : base(typeof(T))
        {
        }
    }
}
