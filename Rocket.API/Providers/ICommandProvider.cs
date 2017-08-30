using System.Collections.ObjectModel;
using Rocket.API.Commands;
using System;

namespace Rocket.API.Providers
{
    [ProviderDefinition]
    public interface ICommandProvider { 
        ReadOnlyCollection<Type> Commands { get; }
    }
}
