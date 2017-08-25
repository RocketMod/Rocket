using System.Collections.ObjectModel;
using Rocket.API.Commands;

namespace Rocket.API.Providers.Commands
{
    [ProviderDefinition]
    public interface IRocketCommandProvider { 
        ReadOnlyCollection<IRocketCommand> Commands { get; }
    }
}
