
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API.Plugin
{
    public delegate void ExecutePluginDependendCodeAction(IPlugin plugin);

    public interface IPluginManager : ICommandHandler
    {
        IPlugin GetPlugin(string name);
        IPlugin GetPlugin<IPlugin>();
        bool PluginExists(string name);

        /// <summary>
        /// Executes code that depends on another plugin being available.
        /// Because directly referencing another plugin breaks a plugin if the referenced plugin is unavailable
        /// you should pass the name of the expected plugin to this method and write all code dependend on that 
        /// assembly inside of the delegate you pass to this method.
        /// 
        /// Example usage:
        /// pluginManager.ExecutePluginDependendCode("FancyFeast, (IRocketPlugin plugin) =>{
        ///     Feast pluginInstance = (Feast) plugin;
        ///     pluginInstance.StartFeast();
        /// });
        /// </summary>
        bool ExecutePluginDependendCode(string pluginName, ExecutePluginDependendCodeAction action);
    }
}