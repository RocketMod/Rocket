using Rocket.API;
using Rocket.API.DependencyInjection;

namespace Rocket.Tests.Properties {
    public class DependencyRegistrator : IDependencyRegistrator {
        public void Register(IDependencyContainer container, IDependencyResolver resolver) {
            container.RegisterSingletonType<IImplementation, Implementation>();
        }
    }
}