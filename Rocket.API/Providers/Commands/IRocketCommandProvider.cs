using System.Collections.Generic;
using Rocket.API.Commands;

namespace Rocket.API.Providers.Commands
{
    [RocketProvider]
    public interface IRocketCommandProvider : IRocketProviderBase
    {
        List<IRocketCommand> Commands { get; }
    }
}
