using System.Collections.Generic;

namespace Rocket.API.ServiceProxies
{
    public interface IServiceProxy<T> where T: IProxyableService
    {
        IEnumerable<T> ProxiedProviders { get; }
    }
}