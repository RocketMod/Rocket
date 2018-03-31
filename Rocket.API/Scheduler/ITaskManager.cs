using Rocket.API.Plugin;

namespace Rocket.API.Scheduler
{
    public interface ITaskManager
    {
        void RegisterTasks(ILifecycleController plugin);

        void UnregisterTasks(ILifecycleController plugin);
    }
}