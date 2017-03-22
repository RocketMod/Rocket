using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Providers;

namespace Rocket.Core.Managers
{
    public class RocketProviderManager
    {
        private readonly List<Type> availableProviderTypes;
        private readonly List<Type> builtinProviders;
        private Dictionary<Type, RocketProviderBase> providerProxies = new Dictionary<Type, RocketProviderBase>();
        private readonly List<IProviderRegistration> providers = new List<IProviderRegistration>();

        internal RocketProviderManager() {
            
        }

        private IProviderRegistration getProviderInterface(Type provider)
        {
            IProviderRegistration currentProviderType = null;
            Type[] currentProviderTypes = provider.GetInterfaces();
            foreach (IProviderRegistration registration in availableProviderTypes)
            {
                if (currentProviderTypes.Contains(registration.Type))
                {
                    currentProviderType = registration;
                    break;
                }
            }
            if (currentProviderType == null) throw new ArgumentException("Provider has no known interface");
            if (!(provider.IsAssignableFrom(typeof(RocketProviderBase)))) throw new ArgumentException("Provider does not implement RocketProviderBase");
            return currentProviderType;
        }

        private T registerProvider<T>() where T : RocketProviderBase
        {
            return (T)registerProvider(typeof(T));
        }

        private static IRocketProviderBase registerProvider(Type provider)
        {
            if (!provider.IsInterface) throw new Exception("The given type is not an interface");
            IProviderRegistration currentProviderType = getProviderInterface(provider);

            if (!currentProviderType.AllowMultipleInstances)
            {
                providers.Where(p => p.Type == currentProviderType.Type).All(p => { p.Enabled = false; p.Implementation.Unload(); return true; });
            }

            Type t = typeof(ProviderRegistration<>).MakeGenericType(currentProviderType.GetType().GetGenericArguments()[0]);
            IProviderRegistration result = (IProviderRegistration)Activator.CreateInstance(t, currentProviderType, (RocketProviderBase)Activator.CreateInstance(provider));
            providers.Add(result);
            return result.Implementation;
        }

        public T GetProvider<T>() where T : IRocketProviderBase
        {
            return (T)GetProvider(typeof(T));
        }

        public bool RunOnProvider<T>(Action<T> action)
        {
            var providers = GetProviders<T>();
            if (providers.Count == 0)
                return false;

            foreach (var provider in providers)
                action.Invoke(provider);

            return true;
        }

        public IRocketProviderBase GetProvider(Type providerType)
        {
            var providerRegistration = availableProviderTypes.FirstOrDefault(t => t.Type == providerType);
            if (providerRegistration == null) throw new ArgumentException("The given type is not a known provider interface");
            if (providerRegistration.AllowMultipleInstances)
            {
                //use proxies to handle multiple providers
                return getProxyForProvider(providerRegistration);
            }

            return GetProviders(providerType).FirstOrDefault();
        }

        public List<T> GetProviders<T>()
        {
            return GetProviders(typeof(T)).Cast<T>().ToList();
        }

        public List<IRocketProviderBase> GetProviders(Type providerType)
        {
            if (!providerType.IsInterface) throw new ArgumentException("The given type is no interface");
            if (availableProviderTypes.FirstOrDefault(t => t.Type == providerType) == null) throw new ArgumentException("The given type is not a known provider interface");
            return providers.Where(p => p.Type == providerType && p.Enabled).Select(p => p.Implementation).ToList();
        }

        internal void Reload()
        {
            throw new NotImplementedException();
        }

        private RocketProviderBase getProxyForProvider(IProviderRegistration providerRegistration)
        {
            if (cachedProxies.ContainsKey(providerRegistration))
                return cachedProxies[providerRegistration];
            return null;
        }
    }
}
