using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;
using Rocket.Tests.Mock.Providers;

namespace Rocket.Tests.Properties
{
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IDependencyContainer container)
        {
            container.AddSingleton<IHost, TestHost>();
            container.AddSingleton<IPlayerManager, TestPlayerManager>();
        }
    }
}