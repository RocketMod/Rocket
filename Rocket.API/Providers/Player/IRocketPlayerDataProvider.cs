using Rocket.API.Player;

namespace Rocket.API.Providers.Player
{
    [ProviderDefinition]
    public interface IRocketPlayerDataProvider
    {
        T Get<T>(IRocketPlayer player, string key);
        bool Set<T>(IRocketPlayer player, string key, T value);
    }
}