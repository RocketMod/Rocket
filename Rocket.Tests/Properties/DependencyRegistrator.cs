using Rocket.API.DependencyInjection;

namespace Rocket.Tests
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<IImplementation, Implementation>();
        }
    }
}
