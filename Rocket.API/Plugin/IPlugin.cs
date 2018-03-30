using System.Collections.Generic;

namespace Rocket.API.Plugin
{
    public enum PluginState { Loaded, Unloaded, Failure, Cancelled };
    
    public interface IPlugin
    {
        IEnumerable<string> Capabilities { get; }
        string Name { get; }
        PluginState State { get; set; }
        void Load();
        void Unload();
    }
}
