namespace XCEngine.Server
{
    public static class Actor
    {
        /// <summary>
        /// Actor Id生成
        /// </summary>
        private static IIdGenerator _actorIdGenerator = new ThreadSafeIdGenerator();

        /// <summary>
        /// 所有ActorContext字典
        /// </summary>
        private static Dictionary<int, ActorContext> _actorContextDict = new();

        /// <summary>
        /// 启动Actor
        /// </summary>
        /// <typeparam name="T">actor类型</typeparam>
        /// <param name="param">创建参数</param>
        /// <returns></returns>
        public static int Start<T>(object param) where T : class, new()
        {
            return Start(typeof(T), param);
        }

        public static int Start(Type type, object param)
        {
            // TODO：检查消息处理器是不是存在

            object actor = Activator.CreateInstance(type);
            int actorId = _actorIdGenerator.GenerateId();

            lock (_actorContextDict)
            {
                _actorContextDict.Add(actorId, new ActorContext()
                {
                    ActorId = actorId,
                    ActorObject = actor
                });
            }

            Log.Info($"Start actor: {type.Name}[{actorId}]");

            // TODO：发送Create Actor消息

            return actorId;
        }

        /// <summary>
        /// 杀死其他Actor
        /// </summary>
        /// <param name="actorId"></param>
        public static void Kill(int actorId)
        {

        }

        /// <summary>
        /// 将自己退出
        /// </summary>
        public static void Exit()
        {

        }

        /// <summary>
        /// 给Actor命名
        /// </summary>
        /// <param name="actorId">actor id</param>
        /// <param name="name">名字</param>
        public static void Name(int actorId, string name)
        {

        }

        /// <summary>
        /// 向Actor以Send形式发送消息
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="param"></param>
        public static void Send(int actorId, object param)
        {

        }

        /// <summary>
        /// 向Actor以Call形式发送消息
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Task<object> Call(int actorId, object param)
        {

        }
    }
}
