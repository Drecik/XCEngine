internal class TestActor3
{
}

[ActorMessageDispatcher(typeof(TestActor3))]
internal class TestActor3MessageDispatcher : CommonActorMessageDispatcher<TestActor3>
{
}

[ActorMessageHandler(typeof(TestActor3))]
internal static class TestActor3MessageHandler
{
    [ActorMessageHandlerMethod("OnCreate")]
    public static void OnCreate(this TestActor3 self)
    {
        Log.Info("Started");
    }

    [ActorMessageHandlerMethod("OnDestroy")]
    public static void OnDestroy(this TestActor3 self)
    {
        Log.Info("Destroy");
    }

    [ActorMessageHandlerMethod("TestSend")]
    public static void TestSend(this TestActor3 self, int arg1, string arg2, double arg3)
    {
        Log.Info($"Receive send, {arg1}, {arg2}, {arg3}");
    }

    [ActorMessageHandlerMethod("TestCall")]
    public async static Task<int> TestCall(this TestActor3 self, int arg1, string arg2, double arg3)
    {
        Log.Info($"Receive call, {arg1}, {arg2}, {arg3}");

        await Task.Delay(1000);
        return 1;
    }
}