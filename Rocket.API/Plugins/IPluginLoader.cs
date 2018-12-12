using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API.DependencyInjection;

namespace Rocket.API.Plugins
{
    /// <summary>
    ///     A service which provides plugins and is responsible for managing them.
    /// </summary>
    public interface IPluginLoader : IEnumerable<IPlugin>, IProxyableService
    {
        /// <summary>
        ///     The provided plugins.
        /// </summary>
        IEnumerable<IPlugin> Plugins { get; }

        /* Todo: make extension methods:
        /// <summary>
        ///     Gets a plugin by name.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <returns>The plugin instance if it was found; otherwise, <b>null</b>.</returns>
        IPlugin GetPlugin(string name);

        /// <summary>
        ///     Checks if a plugin exists with the given name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns><b>true</b> if the plugin exists; otherwise, <b>false</b>.</returns>
        bool PluginExists(string name);
        */

        /// <summary>
        ///     Initializes the plugin manager.
        /// </summary>
        Task InitAsync();

        Task<bool> ActivatePluginAsync(string name);

        Task<bool> DeactivatePluginAsync(string name);

        Task<bool> PluginExistsAsync(string name);

        IPlugin GetPlugin(string name);

        /// <summary>
        ///     Executes soft depend code if the given plugin was loaded.
        /// </summary>
        /// <remarks>
        ///     Directly referencing another plugin breaks the calling plugin when the referenced plugin is not available yet.
        ///     <br /><br />
        ///     The full qualifying type names (types with fully declared namespaces) must be used instead of "using" statements
        ///     for
        ///     namespaces of the referenced plugin.<br /><br />
        ///     <b>Example:</b>
        ///     <code>
        ///         pluginManager.ExecuteSoftDependCode("FancyFeast, (IRocketPlugin plugin) =>{
        ///             FancyFeastPlugin.Feast pluginInstance = (FancyFeastPlugin.Feast) plugin;
        ///             pluginInstance.StartFeast();
        ///         });
        ///     </code>
        /// </remarks>
        /// <param name="pluginName">The name of the referenced plugin.</param>
        /// <param name="action">The action to be invoked when the plugin was found.</param>
        Task ExecuteSoftDependCodeAsync(string pluginName, Func<IPlugin, Task> action);
    }
}