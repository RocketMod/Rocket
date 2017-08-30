namespace Rocket.API.Providers
{
    [ProviderDefinition]
    public interface IGameProvider
    {
        string InstanceName { get; }
        string Name { get; }
        void Shutdown();
        void Reload();
    }
}