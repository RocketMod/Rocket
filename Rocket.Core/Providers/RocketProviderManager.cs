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
using Assert = Rocket.API.Utils.Debugging.Assert;

namespace Rocket.Core.Managers
{
    public class ProviderManager
    {
        internal static GameObject providersGameObject = new GameObject("Rocket Providers");
        private readonly List<Type> providerTypes = new List<Type>();
        private Dictionary<string, IRocketProviderBase> providerProxies = new Dictionary<string, IRocketProviderBase>();
        private readonly List<ProviderRegistration> providers = new List<ProviderRegistration>();

        internal ProviderManager()
        {

        }
        internal void LoadRocketProviders(Type[] types)
        {
            loadProviderDefinitionsFromTypes(types);
            loadProviderImplementationsFromTypes(types);

            instanciateProviderImplemenations();
        }

        private void instanciateProviderImplemenations()
        {
            foreach (ProviderImplementation provider in providerImplementations)
            {
                if (!provider.Enabled) continue;
                if (((ProviderDefinitionAttribute)provider.Definition.Type.GetCustomAttributes(typeof(ProviderDefinitionAttribute), true).First()).MultiInstance)
                {
                    provider.Instanciate();
                }
                else
                {
                    IEnumerable<ProviderImplementation> allProvidersOfThisDefinition = providerImplementations.Where(p => p.Definition == provider.Definition);
                    foreach (ProviderImplementation implementation in allProvidersOfThisDefinition)
                    {
                        implementation.Enabled = false;
                    }
                    ProviderImplementation choosenProvider = allProvidersOfThisDefinition.First();
                    choosenProvider.Enabled = true;
                    choosenProvider.Instanciate();
                }
            }
        }

        private void loadProviderDefinitionsFromTypes(Type[] types)
        {
            foreach (Type type in types)
            {
                if (type.GetCustomAttributes(typeof(ProviderDefinitionAttribute), true).Length > 0 && type.IsInterface)
                {
                    providerDefinitionTypes.Add(type);
                }
            }
        }

        private void loadProviderImplementationsFromTypes(Type[] types)
        {
            foreach (Type type in types)
            {
                ProviderProxyAttribute proxyAttribute = (ProviderProxyAttribute)type.GetCustomAttributes(typeof(ProviderProxyAttribute), true).FirstOrDefault();

                if (proxyAttribute == null)
                    continue;

                foreach (Type providerType in type.GetInterfaces())
                {
                    ProviderDefinitionAttribute providerAttribute = (ProviderDefinitionAttribute)providerType.GetCustomAttributes(typeof(ProviderDefinitionAttribute), true).First();
                    if (providerAttribute == null)
                    {
                        if (providerDefinitionTypes.Contains(providerType))
                        {
                            providerImplementations.Add(new ProviderImplementation(providerType, type));
                            break;
                        }
                    }
                    else if (providerAttribute.MultiInstance)
                    {
                        providerImplementations.Add(new ProviderImplementation(providerType, type));
                    }
                }
            }
        }

        public T GetProvider<T>()
        {
            return (T)GetProvider(typeof(T));
        }

        public IRocketProviderBase GetProvider(Type type)
        {
            var providers = GetProviders(type);
            if (providers.Count == 0)
            {
                R.Logger.LogMessage(LogLevel.WARN, "Could not find provider implementations for: " + type.FullName, ConsoleColor.DarkRed);
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
            return providers.Where(p => p.ProviderType.Equals(providerType.FullName, StringComparison.OrdinalIgnoreCase) && p.Enabled && p.Implementation != null).Select(p => p.Implementation).ToList();
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

            foreach (ProviderRegistration providerRegistration in persistantProviderRegistrations)
            {
                if (providers.Any(c => c.ProviderImplementation.TypeName.Equals(providerRegistration.ProviderImplementation.TypeName, StringComparison.OrdinalIgnoreCase)))
                    continue; //Prevent duplicate registration

                if (providerRegistration.ProviderImplementation.Resolve())
                {
                    Console.WriteLine("Resolving " + providerRegistration.ProviderType + " " + providerRegistration.ProviderImplementation);
                }
                else
                {
                    Console.WriteLine("Failed to resolve " + providerRegistration.ProviderType + " " + providerRegistration.ProviderImplementation);
                }


                var provAttr = (RocketProviderAttribute)providerRegistration.Provider.GetCustomAttributes(typeof(RocketProviderAttribute), true).First();
                if (provAttr == null || (!provAttr.SupportsMultiple && GetProvider(providerRegistration.Provider) != null))
                {
                    Console.WriteLine("WARN: " + providerRegistration.ProviderImplementation.Type.FullName + " could not be registered because provider was not found or provider has already implementation!");
                    continue;
                }

                providers.Add(providerRegistration);

                if (providerRegistration.Enabled && providerRegistration.ProviderType == typeof(IRocketPluginProvider).FullName)
                {
                    //foreach (Type newProviderType in ((IRocketPluginProvider) provider.Implementation).LoadProviders()) {
                    //    registerProvider(newProviderType);
                    //}
                }
            }

            foreach (ProviderRegistration provider in providers)
            {
                try
                {
                    if (persistantProviderRegistrations.FirstOrDefault(p => p.Equals(provider) && !p.Enabled) != null)
                        continue;

                    if (providerProxies.ContainsKey(provider.ProviderType))
                    {
                        Console.WriteLine("Loading provider implementation: " + provider.ProviderType);
                        provider.Load();
                        provider.Implementation?.Load();
                        continue;
                    }

                    if (providers.FirstOrDefault(p => p.Enabled && p.ProviderType == provider.ProviderType) == null)
                    {
                        Console.WriteLine("Loading provider implementation: " + provider.ProviderType);
                        provider.Load();
                        if (provider.Implementation == null)
                            Console.WriteLine("Warning: " + provider.ProviderImplementation.TypeName + " has no implementation");
                        provider.Implementation?.Load();
                    }
                    else
                    {
                        Console.WriteLine("Skipping provider implementation: " + provider.ProviderType + ", a implementation was already loaded for this provider");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to load provider implementation: " + provider.Implementation.GetType().FullName);
                    Console.WriteLine(e);
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
