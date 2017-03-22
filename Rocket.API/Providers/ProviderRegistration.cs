using System;
using System.Linq;
using Rocket.API.Providers;

namespace Rocket.API.Providers
{
    
    public class ProviderRegistration
    {
        public ProviderRegistration(IRocketProviderBase implementation, Type providerType)
        {
            Implementation = implementation;
            ProviderType = providerType;
        }

        public Type ProviderType { get; private set; }
        public bool Enabled { get; set; } = true;
        public IRocketProviderBase Implementation { get; set; }
    }
}
