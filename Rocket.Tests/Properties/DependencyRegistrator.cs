using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;
using Rocket.Tests.Mock.Providers;

namespace Rocket.Tests.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<IHost, TestHost>();
            container.RegisterSingletonType<IPlayerManager, TestPlayerManager>();
        }
    }
}