[UseCommonActorMessageDispatcher]
internal class HttpClientActor
{
}


[ActorMessageHandler(typeof(HttpClientActor))]
internal static class HttpClientActorMessageHandler
{
    [ActorMessageHandlerMethod(nameof(OnCreate))]
    public static void OnCreate(this HttpClientActor self)
    {
        XC.Tick(3000, 3000, "OnTick");
    }

    [ActorMessageHandlerMethod(nameof(OnTick))]
    public static async void OnTick(this HttpClientActor self)
    {
        var url = "http://localhost:10086/";
        Log.Info(await (await HttpUtils.GetAsync(url)).ReadAsStringAsync());
    }
}