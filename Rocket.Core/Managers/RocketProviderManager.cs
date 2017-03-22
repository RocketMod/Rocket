using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Proxies;
using System.Text;
using Rocket.API.Providers;
using Rocket.API.Providers.Implementation;

namespace Rocket.Core.Managers
{
    public class RocketProviderManager
    {
        private readonly List<Type> providerTypes = new List<Type>();
        private Dictionary<Type, IRocketProviderBase> providerProxies = new Dictionary<Type, IRocketProviderBase>();

        private readonly List<ProviderRegistration> providers = new List<ProviderRegistration>();

        internal RocketProviderManager() {

            foreach (Type type in typeof(Rocket.API.Environment).Assembly.GetTypes()) {
                if (type.GetCustomAttributes(typeof(RocketProviderAttribute), true).Length > 0 && type.IsInterface && type.IsAssignableFrom(typeof(IRocketProviderBase))) {
                    providerTypes.Add(type);
                }
            }

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(RocketProviderAttribute), true).Length > 0 && type.IsAssignableFrom(typeof(IRocketProviderBase))) {
                    registerProvider(type);
                }
                RocketProviderProxyAttribute proxyAttribute =  type.GetCustomAttributes(typeof(RocketProviderProxyAttribute), true).Cast<RocketProviderProxyAttribute>().FirstOrDefault();
                if (proxyAttribute != null && type.IsAssignableFrom(typeof(IRocketProviderBase)))
                {
                    providerProxies.Add(proxyAttribute.Provider, (IRocketProviderBase)Activator.CreateInstance(type));
                }
            }
        }

        internal T registerProvider<T>() where T : IRocketProviderBase
        {
            return (T)registerProvider(typeof(T));
        }

        internal IRocketProviderBase registerProvider(Type provider)
        {
            Type providerType = provider.DeclaringType.GetInterfaces().FirstOrDefault();
            if (providerType == null) throw new ArgumentException("The given type is no interface");
            if (providerTypes.Contains(providerType)) throw new ArgumentException("The given type is not a known provider interface");

            //Check if there is a proxy for this provider interface, if not - don't allow multiple enabled providers of this kind
            if (!providerProxies.ContainsKey(providerType))
            {
                providers.Where(p => p.ProviderType == providerType).All(p => { p.Enabled = false; p.Implementation.Unload(); return true; });
            }

            ProviderRegistration result = new ProviderRegistration((IRocketProviderBase)Activator.CreateInstance(provider),providerType);
            providers.Add(result);
            return result.Implementation;
        }

        public T GetProvider<T>() where T : IRocketProviderBase
        {
            return (T)GetProvider(typeof(T));
        }

        public IRocketProviderBase GetProvider(Type providerType)
        {
            if (!providerType.IsInterface) throw new ArgumentException("The given type is no interface");
            if (providerTypes.Contains(providerType)) throw new ArgumentException("The given type is not a known provider interface");
            if (providerProxies.ContainsKey(providerType))
            {
                return providerProxies[providerType];
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
            if (providerTypes.Contains(providerType)) throw new ArgumentException("The given type is not a known provider interface");
            return providers.Where(p => p.ProviderType == providerType && p.Enabled).Select(p => p.Implementation).ToList();
        }

        internal void Unload()
        {
            providers.Where(p => p.Enabled).All(p => {
                p.Implementation.Unload();
                return true;
            });
        }

        internal void Load()
        {
            providers.Where(p => p.Enabled).All(p => {
                p.Implementation.Load();
                return true;
            });
        }
    }
}
