using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Rocket.API.Assets;
using Rocket.API.Providers;
using Rocket.API.Providers.Plugins;
using Rocket.Core.Providers;
using UnityEngine;

namespace Rocket.Core.Managers
{
    public class RocketProviderManager
    {
        internal static GameObject providersGameObject = new GameObject("Rocket Providers");
        private readonly List<Type> providerTypes = new List<Type>();
        private Dictionary<string, IRocketProviderBase> providerProxies = new Dictionary<string, IRocketProviderBase>();
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
                    providerProxies.Add(proxyAttribute.Provider.FullName, (IRocketProviderBase)Activator.CreateInstance(type));
                }
            }
        }

        internal ProviderRegistration registerProvider<T>() where T : IRocketProviderBase
        {
            return registerProvider(typeof(T));
        }

        internal ProviderRegistration registerProvider(Type provider)
        {
            Type providerType = provider.DeclaringType.GetInterfaces().FirstOrDefault();
            if (providerType == null) throw new ArgumentException("The given type is no interface");
            if (providerTypes.Contains(providerType)) throw new ArgumentException("The given type is not a known provider interface");
            
            ProviderRegistration result = new ProviderRegistration(providerType.FullName,provider);
            providers.Add(result);
            return result;
        }

        public T GetProviderProxy<T>() where T : IRocketProviderBase
        {
            return (T)GetProviderProxy(typeof(T));
        }

        public IRocketProviderBase GetProviderProxy(Type providerProxy)
        {
            Type providerType = providerProxy.DeclaringType.GetInterfaces().FirstOrDefault(typeof(IRocketProviderBase).IsAssignableFrom);
             if (providerType != null && providerProxies.ContainsKey(providerType.FullName))
            {
                return providerProxies[providerType.FullName];
            }
            return null;
        }

        public List<T> GetProviders<T>() where T : IRocketProviderBase
        {
            return GetProviders(typeof(T)).Cast<T>().ToList();
        }

        public T GetProvider<T>() where T: IRocketProviderBase
        {
            var proxy = GetProviderProxy<T>();
            if (proxy != null)
                return proxy;
            return GetProviders<T>().FirstOrDefault();
        }

        public List<IRocketProviderBase> GetProviders(Type providerType)
        {
            if (!providerType.IsInterface) throw new ArgumentException("The given type is no interface");
            if (providerTypes.Contains(providerType)) throw new ArgumentException("The given type is not a known provider interface");
            return providers.Where(p => p.ProviderType == providerType.FullName && p.Enabled).Select(p => p.Implementation).ToList();
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
            //All providers must be loaded at this point;
            string providerFileName = "Providers.config.xml";

            List <ProviderRegistration> persistantProviderRegistrations = new List<ProviderRegistration>();
            XmlSerializer serializer = new XmlSerializer(typeof(List<ProviderRegistration>));

            if (File.Exists(providerFileName))
            {
                using (StreamReader reader = new StreamReader(providerFileName))
                {
                    persistantProviderRegistrations = (List<ProviderRegistration>)serializer.Deserialize(reader);
                }
            }

            foreach (ProviderRegistration provider in persistantProviderRegistrations) {
                if (provider.Provider.Resolve()) {
                    R.Logger.Info("Resolving " + provider.ProviderType + " " + provider.Provider);
                }
                else
                {
                    R.Logger.Error("Failed to resolve "+ provider.ProviderType+" " + provider.Provider);
                }
                if (provider.Enabled && provider.ProviderType == typeof(IRocketPluginProvider).FullName) {
                    provider.Load();
                    foreach (Type newProviderType in ((IRocketPluginProvider) provider.Implementation).LoadProviders()) {
                        registerProvider(newProviderType);
                    }
                }
            }

            foreach (ProviderRegistration provider in providers)
            {
                if (persistantProviderRegistrations.FirstOrDefault(p => p.Equals(provider) && !p.Enabled) == null) {
                    if (providerProxies.ContainsKey(provider.ProviderType)) {
                        provider.Load();
                    }
                    else {
                        if (providers.FirstOrDefault(p => p.Enabled && p.ProviderType == provider.ProviderType) == null) {
                            provider.Load();
                        }
                    }
                }
            }
            using (StreamWriter writer = new StreamWriter(providerFileName))
            {
                serializer.Serialize(writer,providers);
            }
        }

        public void RegisterProvider(IRocketProviderBase pluginProvider)
        {
            //??
        }
    }
}
