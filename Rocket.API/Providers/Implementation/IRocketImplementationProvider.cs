using Rocket.API.Implementation.Managers;
using System;
using System.Collections.Generic;
using Rocket.API.Collections;

namespace Rocket.API.Providers.Implementation
{
    public interface IRocketImplementationProvider : IRocketProviderBase
    {
        string InstanceName { get; }
        string Name { get; }
        IChatManager Chat { get; }
        IPlayerManager Players { get; }

        List<Type> Providers { get; }

        TranslationList DefaultTranslation { get; }

        void Shutdown();
        void Reload();
    }
}