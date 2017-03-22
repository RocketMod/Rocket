using System;
using System.Collections.Generic;
using Rocket.API.Collections;
using Rocket.API.Commands;

namespace Rocket.API.Providers.Plugins
{
    [RocketProvider]
    public interface IRocketPluginProvider : IRocketProviderBase
    {
        List<IRocketPlugin> GetPlugins();
        IRocketPlugin GetPlugin(string name);
        List<Type> GetProviders();
        List<IRocketCommand> CommandProvider { get; }
        string GetPluginDirectory(string name);
    }
}
