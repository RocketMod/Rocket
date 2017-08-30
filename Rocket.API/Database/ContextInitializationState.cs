namespace Rocket.API.Providers.Database
{
    public enum ContextInitializationState
    {
        CONNECTED,
        NOT_CONNECTED,
        EXCEPTION,
        TIMEOUT,
        ABORTED
    }
}