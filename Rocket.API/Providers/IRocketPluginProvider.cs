using Rocket.API.Commands;
using System.Collections.Generic;
using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Plugins;
using System;

namespace Rocket.API.Providers
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
