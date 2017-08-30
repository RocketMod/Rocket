using Rocket.API.Player;

namespace Rocket.API.Providers
{
    [ProviderDefinition]
    public interface IPlayerDataProvider
    {
        T Get<T>(IPlayer player, string key);
        bool Set<T>(IPlayer player, string key, T value);
    }
}