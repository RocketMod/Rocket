using Rocket.API.Collections;
using Rocket.API.Providers.Implementation.Managers;
using System;
using System.Collections.ObjectModel;

namespace Rocket.API.Providers.Implementation
{
    [ProviderDefinition]
    public interface IGameProvider
    {
        string InstanceName { get; }
        string Name { get; }
        void Shutdown();
        void Reload();
    }
}