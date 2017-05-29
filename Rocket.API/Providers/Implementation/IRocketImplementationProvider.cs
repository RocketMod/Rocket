using Rocket.API.Collections;
using Rocket.API.Providers.Implementation.Managers;
using System;
using System.Collections.ObjectModel;

namespace Rocket.API.Providers.Implementation
{
    [RocketProvider]
    public interface IRocketImplementationProvider : IRocketProviderBase
    {
        string InstanceName { get; }
        string Name { get; }
        IChatManager Chat { get; }
        IPlayerManager Players { get; }
        ReadOnlyCollection<Type> Providers { get; }
        TranslationList DefaultTranslation { get; }
        void Shutdown();
        void Reload();
    }
}