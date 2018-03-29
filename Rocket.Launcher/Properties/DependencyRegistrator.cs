using Rocket.Core;
using Rocket.IOC;
using Rocket.API.IOC;

namespace Rocket.Launcher.Properties
{
    public class DependencyRegistrator : IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<ITestThingie, TestThingie>();
        }
    }
}
