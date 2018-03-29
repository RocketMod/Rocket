namespace Rocket.API.Plugin
{
    public enum PluginState { Loaded, Unloaded, Failure, Cancelled };
    
    public interface IPlugin
    {
        string Name { get; }
        PluginState State { get; set; }
        void Load();
        void Unload();
    }
}
