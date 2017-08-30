using System.IO;
using Rocket.API.Serialization;

namespace Rocket.Core.Providers.Plugin.Native
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

    public class NativeRocketPlugin<TRocketPluginConfiguration> : RocketPluginBase<TRocketPluginConfiguration> where TRocketPluginConfiguration : class, IPluginConfiguration
    {
        public void Awake()
        {
            Name = GetType().Name;
            PluginManager = NativeRocketPluginProvider.Instance;
            base.Initialize(Path.Combine(WorkingDirectory, Name));
        }
    }
}