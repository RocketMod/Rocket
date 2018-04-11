using Rocket.API;
using Rocket.API.Ioc;

namespace Rocket.Tests.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<IImplementation, TestImplementation>();
        }
    }
}