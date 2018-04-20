using Rocket.API;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;
using Rocket.Tests.TestProviders;

namespace Rocket.Tests.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<IImplementation, TestImplementation>();
            container.RegisterSingletonType<IPlayerManager, TestPlayerManager>();
        }
    }
}