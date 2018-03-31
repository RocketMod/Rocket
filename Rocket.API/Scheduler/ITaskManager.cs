using Rocket.API.Plugin;

namespace Rocket.API.Scheduler
{
    public interface ITaskManager
    {
        void RegisterTasks(ILifecycleObject plugin);

        void UnregisterTasks(ILifecycleObject plugin);
    }
}