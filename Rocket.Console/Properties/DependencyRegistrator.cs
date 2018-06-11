using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Scheduler;
using Rocket.API.User;

namespace Rocket.Console.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<IHost, ConsoleHost>();
            container.RegisterSingletonType<ITaskScheduler, SimpleTaskScheduler>();
            container.RegisterSingletonType<IUserManager, ConsoleUserManager>("host");
        }
    }
}