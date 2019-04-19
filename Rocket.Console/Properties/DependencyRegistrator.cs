using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Scheduling;
using Rocket.API.User;
using Rocket.Core.Scheduling;
using Rocket.Core.User;

namespace Rocket.Console.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<IHost, ConsoleHost>();
            container.RegisterSingletonType<IUserManager, StdConsoleUserManager>("host", "stdconsole");
        }
    }
}