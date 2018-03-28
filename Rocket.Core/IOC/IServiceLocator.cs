using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.IOC
{
    public interface IServiceLocator
    {
        object GetService(Type serviceType);
        IEnumerable<object> GetAllInstances(Type serviceType);
        IEnumerable<TService> GetAllInstances<TService>();
        object GetInstance(Type serviceType);
        object GetInstance(Type serviceType, string mapppingName);
        TService GetInstance<TService>();
        TService GetInstance<TService>(string mapppingName);
    }
}
