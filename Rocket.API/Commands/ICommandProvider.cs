using System.Collections.Generic;
using Rocket.API.ServiceProxies;

namespace Rocket.API.Commands
{
    public interface ICommandProvider : IProxyableService
    {
        IEnumerable<ICommand> Commands { get; }
    }
}