using Rocket.API;
using Rocket.API.Plugins;
using System.Reflection;

namespace Rocket.Plugins.Native
{
    public class NativeRocketPlugin : RocketPlugin<NativeRocketPlugin>
    {
        public Assembly Assembly { get { return GetType().Assembly; } }
        public NativeRocketPlugin(IRocketPluginManager<NativeRocketPlugin> manager, string name) : base(manager, name)
        {

        }
    }

    public class NativeRocketPlugin<RocketPluginConfiguration> : RocketPlugin<NativeRocketPlugin, RocketPluginConfiguration> where RocketPluginConfiguration : class, IRocketPluginConfiguration
    {
        public NativeRocketPlugin(IRocketPluginManager<NativeRocketPlugin> manager, string name) : base(manager,name)
        {

        }
    }
}