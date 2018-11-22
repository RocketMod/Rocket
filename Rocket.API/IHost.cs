using System;
using System.Threading.Tasks;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Eventing;

namespace Rocket.API
{
    /// <summary>
    ///     A game specific implemention of RocketMod. Implementations are responsible for providing game specific features.
    /// </summary>
    public interface IHost : IEventEmitter, IConfigurationContext, IService
    {
        /// <summary>
        ///     The host version.
        /// </summary>
        Version HostVersion { get; }

        /// <summary>
        ///     The name of the game version.
        /// </summary>
        Version GameVersion { get; }

        /// <summary>
        ///     The name of the server.
        /// </summary>
        string ServerName { get; }

        /// <summary>
        ///     The port of the server.
        /// </summary>
        ushort ServerPort { get; }

        /// <summary>
        ///     Gets the console. <br />
        ///     <b>Might return null.</b>
        /// </summary>
        IConsole Console { get; }

        /// <summary>
        ///     The name of the game.
        /// </summary>
        string GameName { get; }

        /// <summary>
        ///     Initializes the implementation.
        /// </summary>
        /// <param name="runtime">The RocketMod runtime.</param>
        Task InitAsync(IRuntime runtime);

        /// <summary>
        ///     Shuts the hots down.
        /// </summary>
        Task ShutdownAsync();

        /// <summary>
        ///     Reloads the host (e.g. reload configs, commands, etc.)
        /// </summary>
        Task ReloadAsync();
    }
}