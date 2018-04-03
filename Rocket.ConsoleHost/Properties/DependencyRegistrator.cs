using Rocket.API.DependencyInjection;
using Rocket.API.Scheduler;

namespace Rocket.ConsoleImplementation
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<ITaskScheduler, SimpleTaskScheduler>();
        }
    }
}
