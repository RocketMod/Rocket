using Rocket.Core;
using Rocket.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
