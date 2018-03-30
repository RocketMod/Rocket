using System;
using System.Collections.Generic;

namespace Rocket.API.DependencyInjection
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
