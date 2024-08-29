[ActorMessageHandler(typeof(Bootstrap))]
internal static class BootstrapActorMessageHandler
{
    [ActorMessageHandlerMethod("OnCreate")]
    public static void OnCreate(this Bootstrap self)
    {
        Log.Info("Started");

        XC.Tick(1000, 1000, "Tick");

        XC.Delay(5000, "Delay");
    }

    [ActorMessageHandlerMethod("OnDestroy")]
    public static void OnDestroy(this Bootstrap self)
    {
        Log.Info("Destroy");
    }

    [ActorMessageHandlerMethod("Tick")]
    public static void Tick(this Bootstrap self)
    {
        Log.Info("Tick On Origin");

        GC.Collect();

        Log.Info("Count: " + AppDomain.CurrentDomain.GetAssemblies().Count(e => e.FullName.Contains("ModelHotfix")));
    }

    [ActorMessageHandlerMethod("Delay")]
    public static void Delay(this Bootstrap self)
    {
        Log.Info("Delay");
        Hotfix.Reload();
    }
}