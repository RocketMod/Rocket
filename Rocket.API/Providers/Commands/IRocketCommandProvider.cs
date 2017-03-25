using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rocket.API.Commands;

namespace Rocket.API.Providers.Commands
{
    [RocketProvider]
    public interface IRocketCommandProvider : IRocketProviderBase
    {

        ReadOnlyCollection<IRocketCommand> Commands { get; }
    }
}
