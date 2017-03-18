using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API.Providers
{
    public class ProviderRegistration
    {
        public ProviderRegistration(ProviderRegistration registration, RocketProviderBase implementation)
        {
            Type = registration.Type;
            AllowMultipleInstances = registration.AllowMultipleInstances;
            Implementation = implementation;
        }

        public ProviderRegistration(Type type, bool allowMultipleInstances)
        {
            Type = type;
            AllowMultipleInstances = allowMultipleInstances;
        }

        public Type Type { get; private set; }
        public bool Enabled { get; set; } = true;
        public bool AllowMultipleInstances { get; private set; }
        public RocketProviderBase Implementation { get; set; } = null;
    }
}
