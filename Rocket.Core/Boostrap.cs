using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Core
{
    public class Boostrap
    {
        private static DependencyContainer container;
        static Boostrap()
        {
            container = new DependencyContainer();
            container.RegisterSingletonType<ILog, ConsoleLogger>();
            container.Activate(typeof(Initializer));
        }
    }
}
