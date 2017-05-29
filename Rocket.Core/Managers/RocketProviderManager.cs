using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Rocket.API.Providers;
using Rocket.API.Providers.Logging;
using Rocket.API.Providers.Plugins;
using Rocket.Core.Providers;
using UnityEngine;
using Assert = Rocket.Core.Utils.Debugging.Assert;

namespace Rocket.Core.Managers
{
    public class RocketProviderManager
    {
        internal static GameObject providersGameObject = new GameObject("Rocket Providers");
        private readonly List<Type> providerTypes = new List<Type>();
        private Dictionary<string, IRocketProviderBase> providerProxies = new Dictionary<string, IRocketProviderBase>();
        private readonly List<ProviderRegistration> providers = new List<ProviderRegistration>();

        internal RocketProviderManager()
        {
            LoadFromAssembly(typeof(API.Environment).Assembly);
            LoadFromAssembly(typeof(R).Assembly);
        }

        private void LoadFromAssembly(Assembly asm)
        {
            foreach (Type type in asm.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(RocketProviderAttribute), true).Length > 0 && type.IsInterface && typeof(IRocketProviderBase).IsAssignableFrom(type))
                {
                    Console.WriteLine("Registering provider: " + type.FullName); //logger not ready yet
                    providerTypes.Add(type);
                }
                }

            foreach (Type type in asm.GetTypes())
            {
                if (!typeof(IRocketProviderBase).IsAssignableFrom(type))
                    continue;

                RocketProviderProxyAttribute proxyAttribute = (RocketProviderProxyAttribute)type.GetCustomAttributes(typeof(RocketProviderProxyAttribute), true).FirstOrDefault();
                
                if (proxyAttribute == null)
                    continue;

                if (proxyAttribute.Provider == null)
                {
                    proxyAttribute.Provider =
                        type.GetInterfaces().First(typeof(IRocketProviderBase).IsAssignableFrom);
                }

                Console.WriteLine("Registering provider proxy: " + type.FullName + "[" + proxyAttribute.Provider.FullName + "]"); //logger not ready yet
                providerProxies.Add(proxyAttribute.Provider.FullName, (IRocketProviderBase)Activator.CreateInstance(type));
            }

        }

        internal ProviderRegistration registerProvider<T>(bool autoLoad = false) where T : IRocketProviderBase
        {
            return registerProvider(typeof(T), autoLoad);
        }

        internal ProviderRegistration registerProvider(Type provider, bool autoLoad = false)
        {
            Console.WriteLine("Registering provider implementation: " + provider.FullName + " (autoload: " + autoLoad + ")");
            Assert.NotNull(provider, nameof(provider));
            Type providerType = provider.GetInterfaces().FirstOrDefault(typeof(IRocketProviderBase).IsAssignableFrom);
            Assert.NotNull(providerType, nameof(providerType));

            bool isClass = !provider.IsInterface && !provider.IsAbstract;
            Assert.IsTrue(isClass, nameof(isClass));

            if (!providerTypes.Contains(providerType))
                throw new ArgumentException($"The type {providerType.FullName} is not a known provider interface");

            ProviderRegistration result = new ProviderRegistration(providerType.FullName, provider);
            providers.Add(result);

            if (autoLoad)
            {
                Console.WriteLine("Loading provider: " + provider.FullName);
                result.Load();
                result.Implementation.Load();
            }

            return result;
        }

        public T GetProviderProxy<T>() where T : IRocketProviderBase
        {
            return (T)GetProviderProxy(typeof(T));
        }

        public IRocketProviderBase GetProviderProxy(Type providerProxy)
        {
            Assert.NotNull(providerProxy, nameof(providerProxy));

            if (providerProxies.ContainsKey(providerProxy.FullName))
            {
                return providerProxies[providerProxy.FullName];
            }
            return null;
        }

        public List<T> GetProviders<T>() where T : IRocketProviderBase
        {
            return GetProviders(typeof(T)).Cast<T>().ToList();
        }

        public T GetProvider<T>() where T : IRocketProviderBase
        {
            return (T) GetProvider(typeof(T));
        }

        public IRocketProviderBase GetProvider(Type type)
        {
            var providers = GetProviders(type);
            if (providers.Count == 0)
            {
                R.Logger.Log(LogLevel.WARN, "Could not find provider implementations for: " + type.FullName);
                return null;
            }

            var proxy = GetProviderProxy(type);
            if (proxy != null)
                return proxy;
            return providers.FirstOrDefault();
        }

        public List<IRocketProviderBase> GetProviders(Type providerType)
        {
            if (!providerType.IsInterface) throw new ArgumentException($"The type {providerType.FullName} is no interface");
            if (!providerTypes.Contains(providerType)) throw new ArgumentException($"The type {providerType.FullName} is not a known provider interface");
            return providers.Where(p => p.ProviderType.Equals(providerType.FullName, StringComparison.OrdinalIgnoreCase) && p.Enabled).Select(p => p.Implementation).ToList();
        }

        internal void Unload()
        {
            providers.Where(p => p.Enabled).All(p =>
            {
                p.Implementation.Unload();
                return true;
            });
        }


        internal void Load()
        {
            //All providers must be loaded at this point;
            string providerFileName = "Providers.config.xml";

            List<ProviderRegistration> persistantProviderRegistrations = new List<ProviderRegistration>();
            XmlSerializer serializer = new XmlSerializer(typeof(List<ProviderRegistration>));

            if (File.Exists(providerFileName))
            {
                using (StreamReader reader = new StreamReader(providerFileName))
                {
                    persistantProviderRegistrations = (List<ProviderRegistration>)serializer.Deserialize(reader);
                }
            }

            foreach (ProviderRegistration provider in persistantProviderRegistrations)
            {
                if (provider.Provider.Resolve())
                {
                    Console.WriteLine("Resolving " + provider.ProviderType + " " + provider.Provider);
                }
                else
                {
                    Console.WriteLine("Failed to resolve " + provider.ProviderType + " " + provider.Provider);
                }

                providers.Add(provider);

                if (provider.Enabled && provider.ProviderType == typeof(IRocketPluginProvider).FullName)
                {
                    //foreach (Type newProviderType in ((IRocketPluginProvider) provider.Implementation).LoadProviders()) {
                    //    registerProvider(newProviderType);
                    //}
                }
            }

            foreach (ProviderRegistration provider in providers)
            {
                if (persistantProviderRegistrations.FirstOrDefault(p => p.Equals(provider) && !p.Enabled) != null)
                    continue;

                if (providerProxies.ContainsKey(provider.ProviderType))
                {
                    provider.Load();
                    provider.Implementation.Load();
                    continue;
                }

                if (providers.FirstOrDefault(p => p.Enabled && p.ProviderType == provider.ProviderType) == null)
                {
                    provider.Load();
                    provider.Implementation.Load();
                }
            }
            using (StreamWriter writer = new StreamWriter(providerFileName))
            {
                serializer.Serialize(writer, providers);
            }
        }

        public void RegisterProvider(IRocketPluginProvider pluginProvider)
        {
            //todo: register instance providers
        }
    }
}
