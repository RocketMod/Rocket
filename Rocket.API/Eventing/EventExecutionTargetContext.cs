namespace Rocket.API.Eventing {
    public enum EventExecutionTargetContext {
        NextFrame,
        NextAsyncFrame,
        NextPhysicsUpdate,
        Sync
    }
}