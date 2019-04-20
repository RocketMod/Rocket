using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.User;
using Rocket.Core.User;

namespace Rocket.Console.Properties
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IDependencyContainer container)
        {
            container.AddSingleton<IHost, ConsoleHost>();
            container.AddSingleton<IUserManager, StdConsoleUserManager>("host", "stdconsole");
        }
    }
}