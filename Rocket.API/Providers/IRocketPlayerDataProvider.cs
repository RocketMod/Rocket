using Rocket.API;

namespace Rocket.Core
{
    public interface IRocketPlayerDataProvider
    {
        T Get<T>(IRocketPlayer player, string key);
        bool Set<T>(IRocketPlayer player, string key, T value);
    }
}