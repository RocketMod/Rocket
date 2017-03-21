using Rocket.API.Player;

namespace Rocket.API.Providers
{
    public interface IRocketPlayerDataProvider : IRocketProviderBase
    {
        T Get<T>(IRocketPlayer player, string key);
        bool Set<T>(IRocketPlayer player, string key, T value);
    }
}