using Rocket.API.Plugin;

namespace Rocket.API.Scheduler
{
    public interface ITaskManager
    {
        void RegisterTasks(IRegisterableObject plugin);

        void UnregisterTasks(IRegisterableObject plugin);
    }
}