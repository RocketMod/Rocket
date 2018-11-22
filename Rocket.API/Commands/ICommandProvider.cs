using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API.DependencyInjection;

namespace Rocket.API.Commands
{
    /// <summary>
    ///     A service which provides a set of commands.
    /// </summary>
    public interface ICommandProvider : IProxyableService, IService
    {
        /// <summary>
        ///     The commands of this provider.
        ///     <para>
        ///         <b>This property will never return null.</b>
        ///     </para>
        /// </summary>
        IEnumerable<ICommand> Commands { get; }

        /// <summary>
        ///     The owner of the command.
        /// </summary>
        /// <param name="command">The command to get the owner of.</param>
        ILifecycleObject GetOwner(ICommand command);

        /// <summary>
        ///     Inits the commands provider.
        /// </summary>
        Task InitAsync();
    }
}