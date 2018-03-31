namespace Rocket.API
{
    public enum State { Loaded, Unloaded };

    public interface ILifecycleController
    {
        State State { get; }
    }
}