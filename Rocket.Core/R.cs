using Rocket.Core;
using Rocket.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket
{
    public static class R
    {
        private static DependencyContainer _container;
        public static void Bootstrap()
        {
            _container = new DependencyContainer();
            _container.RegisterSingletonType<ILog, ConsoleLogger>();
            _container.Activate(typeof(Initializer));
        }

        public static IServiceLocator ServiceLocator => _container.ServiceLocator;
    }
}
