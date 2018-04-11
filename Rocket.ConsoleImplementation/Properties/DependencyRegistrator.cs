using Rocket.API;
using Rocket.API.Ioc;
using Rocket.API.Scheduler;

namespace Rocket.ConsoleImplementation.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<IImplementation, ConsoleImplementation>();
            container.RegisterSingletonType<ITaskScheduler, SimpleTaskScheduler>();
        }
    }
}