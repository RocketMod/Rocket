using System.IO;
using Rocket.API.Serialisation;

namespace Rocket.Plugins.Native
{
    public class NativeRocketPlugin : RocketPluginBase
    {
        public void Awake()
        {
            Name = GetType().Name;
            PluginManager = NativeRocketPluginProvider.Instance;
            base.Initialize(Path.Combine(WorkingDirectory, Name));
        }
    }

    public class NativeRocketPlugin<TRocketPluginConfiguration> : RocketPluginBase<TRocketPluginConfiguration> where TRocketPluginConfiguration : class, IRocketPluginConfiguration
    {
        public void Awake()
        {
            Name = GetType().Name;
            PluginManager = NativeRocketPluginProvider.Instance;
            base.Initialize(Path.Combine(WorkingDirectory, Name));
        }
    }
}