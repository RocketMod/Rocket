using Rocket.API.Implementation.Managers;
using Rocket.API.Serialisation;
using System;
using System.Collections.Generic;
using Rocket.API.Collections;

namespace Rocket.API.Providers
{
    public delegate void ImplementationInitialized();
    public delegate void ImplementationShutdown();
    public delegate void ImplementationReload();

    public interface IRocketImplementationProvider : IRocketProviderBase
    {
        string InstanceName { get; }
        string Name { get; }
        IChatManager Chat { get; }
        IPlayerManager Players { get; }

        event ImplementationInitialized OnInitialized;
        event ImplementationShutdown OnShutdown;
        event ImplementationReload OnReload;

        List<Type> Providers { get; }

        TranslationList DefaultTranslation { get; }

        void Shutdown();
        void Reload();
    }
}