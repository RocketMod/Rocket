using Rocket.API.Commands;

namespace Rocket.API.Providers
{
    public interface IRocketCommandProvider : IRocketProviderBase
    {
        RocketCommandList Commands { get; }
    }
}
