using System.Collections.Generic;
using Rocket.API.Collections;
using Rocket.API.Commands;

namespace Rocket.API.Providers
{
    public interface IRocketCommandProvider : IRocketProviderBase
    {
        RocketCommandList Commands { get; }
        void AddCommands(IEnumerable<IRocketCommand> commands);
    }
}
