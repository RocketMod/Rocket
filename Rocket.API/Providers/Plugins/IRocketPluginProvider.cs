using System;
using System.Collections.Generic;
using Rocket.API.Collections;

namespace Rocket.API.Providers.Plugins
{
    public interface IRocketPluginProvider : IRocketProviderBase
    {
        List<IRocketPlugin> GetPlugins();
        IRocketPlugin GetPlugin(string name);
        List<Type> GetProviders();
        RocketCommandList CommandProvider { get; }
        string GetPluginDirectory(string name);
    }
}
