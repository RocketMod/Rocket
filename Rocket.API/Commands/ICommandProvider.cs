using System.Collections.Generic;
using Rocket.API.DependencyInjection;

namespace Rocket.API.Commands
{
    /// <summary>
    ///     A service which provides a set of commands.
    /// </summary>
    public interface ICommandProvider : IProxyableService
    {
        /// <summary>
        ///     The owner of the commands.
        /// </summary>
        ILifecycleObject Owner { get; }

        /// <summary>
        ///     The commands of this provider.
        ///     <para><b>This property will never return null.</b></para>
        /// </summary>
        IEnumerable<ICommand> Commands { get; }
    }
}