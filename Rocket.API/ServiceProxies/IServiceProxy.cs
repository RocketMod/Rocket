using System.Collections.Generic;

namespace Rocket.API.ServiceProxies
{
    public interface IServiceProxy<T> : IServiceProxy where T : IProxyableService
    {
        IEnumerable<T> ProxiedServices { get; }
    }

    public interface IServiceProxy { }
}