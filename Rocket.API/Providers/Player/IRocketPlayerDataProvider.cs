using Rocket.API.Player;

namespace Rocket.API.Providers.Player
{
    [RocketProvider]
    public interface IRocketPlayerDataProvider : IRocketProviderBase
    {
        T Get<T>(IRocketPlayer player, string key);
        bool Set<T>(IRocketPlayer player, string key, T value);
    }
}