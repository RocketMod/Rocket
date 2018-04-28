using System.Collections.Generic;
using Rocket.API.ServiceProxies;

namespace Rocket.API.Commands
{
    /// <summary>
    /// A command provider provides a set of commands.
    /// </summary>
    public interface ICommandProvider : IProxyableService
    {
        /// <summary>
        /// The commands of this provider.<br/><br/>
        /// <b>This property will never return null.</b>
        /// </summary>
        IEnumerable<ICommand> Commands { get; }
    }
}