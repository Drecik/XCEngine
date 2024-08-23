namespace XCEngine.Server
{
    /// <summary>
    /// Actor Call封装
    /// </summary>
    internal static class ActorCaller
    {
        struct ActorCallInfo
        {
            public int From;
            public long Time;
            public TaskCompletionSource<object> Tcs;
        }

        private static Dictionary<int, ActorCallInfo> _callInfoDict = new();

        /// <summary>
        /// Session Id生成器
        /// </summary>
        private static IIdGenerator _sessionIdGenerator = new ThreadSafeIdGenerator();

        public static Task<object> CallRaw(int actorId, string messageId, object data)
        {
            var actorContext = Actor.GetActorContext(actorId);
            if (actorContext == null)
            {
                Log.Info($"Actor: {actorId} not exist.");
                return null;
            }

            var sessionId = _sessionIdGenerator.GenerateId();
            var callInfo = new ActorCallInfo()
            {
                From = Actor.Self(),
                Time = TimeUtils.Now(),
                Tcs = new TaskCompletionSource<object>()
            };

            lock (_callInfoDict)
            {
                _callInfoDict[sessionId] = callInfo;
            }

            var msg = new ActorMessage()
            {
                MessageType = ActorMessage.EMessageType.Call,
                From = Actor.Self(),
                MessageId = messageId,
                MessageData = data,
                SessionId = sessionId
            };

            actorContext.ActorMessageQueue.PushMessage(msg);
            return callInfo.Tcs.Task;
        }

        public static void OnReply(int sessionId, object data)
        {
            ActorCallInfo callInfo;
            lock (_callInfoDict)
            {
                if (_callInfoDict.TryGetValue(sessionId, out callInfo) == false)
                {
                    Log.Warning($"Invalid session id: {sessionId}");
                    return;
                }

                _callInfoDict.Remove(sessionId);
            }

            callInfo.Tcs.SetResult(data);
        }
    }
}
