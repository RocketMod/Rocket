using Rocket.API.Commands;

namespace Rocket.API.Providers
{
    public interface IRocketCommandProvider
    {
        RocketCommandList Commands { get; }
    }
}
