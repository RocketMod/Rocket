using Rocket.API.Serialisation;

namespace Rocket.Plugins.Native
{
    public class NativeRocketPlugin : RocketPluginBase
    {
        public void Awake()
        {
            Name = GetType().Name;
            PluginManager = NativeRocketPluginProvider.Instance;
            base.Initialize();
        }
    }

    public class NativeRocketPlugin<RocketPluginConfiguration> : RocketPluginBase<RocketPluginConfiguration> where RocketPluginConfiguration : class, IRocketPluginConfiguration
    {
        public void Awake()
        {
            Name = GetType().Name;
            PluginManager = NativeRocketPluginProvider.Instance;
            base.Initialize();
        }
    }
}