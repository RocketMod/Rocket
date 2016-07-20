using Rocket.API;
using Rocket.API.Plugins;
using System.Reflection;

namespace Rocket.Plugins.Native
{
    public class NativeRocketPlugin : RocketPluginBase
    {
        public bool IsDependencyLoaded(string plugin)
        {
            return PluginManager.GetPlugin(plugin) != null;
        }

        public delegate void ExecuteDependencyCodeDelegate(IRocketPlugin plugin);

        public void ExecuteDependencyCode(string plugin, ExecuteDependencyCodeDelegate a)
        {
            IRocketPlugin p = PluginManager.GetPlugin(plugin);
            if (p != null)
                a(p);
        }

        public Assembly Assembly { get { return GetType().Assembly; } }
        public NativeRocketPlugin(IRocketPluginManager manager, string name) : base(manager, name)
        {

        }
    }

    public class NativeRocketPlugin<RocketPluginConfiguration> : RocketPluginBase<RocketPluginConfiguration> where RocketPluginConfiguration : class, IRocketPluginConfiguration
    {
        public NativeRocketPlugin(IRocketPluginManager manager, string name) : base(manager,name)
        {

        }
    }
}