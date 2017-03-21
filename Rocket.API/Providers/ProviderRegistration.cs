using System;
using Rocket.API.Providers;

namespace Rocket.API.Providers
{
    
    public class ProviderRegistration<T> : IProviderRegistration<T> where T: IRocketProviderBase
    {
        public ProviderRegistration(ProviderRegistration<T> registration, T implementation)
        {
            Type = registration.Type;
            AllowMultipleInstances = registration.AllowMultipleInstances;
            Implementation = implementation;
        }

        public ProviderRegistration(bool allowMultipleInstances)
        {
            Type = typeof(T);
            AllowMultipleInstances = allowMultipleInstances;
        }

        public Type Type { get; private set; }
        public bool Enabled { get; set; } = true;
        public bool AllowMultipleInstances { get; private set; }
        public IRocketProviderBase Implementation { get; set; }
        public T ImplementationSafe => (T) Implementation;
    }

    public interface IProviderRegistration<out T> : IProviderRegistration where T: IRocketProviderBase
    {

    }

    public interface IProviderRegistration
    {
        Type Type { get; }
        bool Enabled { get; set; }
        bool AllowMultipleInstances { get; }
        IRocketProviderBase Implementation { get; set; }
    }
}
