using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Rocket.API.Providers;
using Rocket.API.Providers.Logging;
using Rocket.API.Providers.Plugins;
using Assert = Rocket.API.Utils.Debugging.Assert;

namespace Rocket.Core.Providers
{
    public class ProviderManager
    {
        private List<Type> providerDefinitionTypes = new List<Type>();
        private List<ProviderImplementation> providerImplementations = new List<ProviderImplementation>();
        private List<ProviderImplementation> providerProxyImplementations = new List<ProviderImplementation>();

        public delegate void ProviderLoaded();
        public event ProviderLoaded OnProvidersLoaded;

        internal void AddProviderDefinition<T>()
        {
            providerDefinitionTypes.Add(typeof(T));
        }

        internal void AddProviderImplementation<T>(ProviderBase instance)
        {
            providerImplementations.Add(new ProviderImplementation(typeof(T), instance.GetType()) { Instance = instance });
        }

        internal void LoadRocketProviders(Type[] types)
        {
            loadProviderDefinitionsFromTypes(types);
            loadProviderImplementationsFromTypes(types);

            instanciateProviderImplemenations();

            loadProviderImplementations();

            OnProvidersLoaded?.Invoke();
        }

        private void loadProviderImplementations()
        {
            foreach (ProviderImplementation implementation in providerProxyImplementations)
            {
                implementation.Load();
            }
            foreach (ProviderImplementation implementation in providerImplementations)
            {
                implementation.Load();
            }
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
                if (!type.IsAssignableFrom(typeof(ProviderBase))) continue;
                ProviderProxyAttribute proxyAttribute = (ProviderProxyAttribute)type.GetCustomAttributes(typeof(ProviderProxyAttribute), true).FirstOrDefault();

                foreach (Type providerType in type.GetInterfaces())
                {
                    ProviderDefinitionAttribute providerAttribute = (ProviderDefinitionAttribute)providerType.GetCustomAttributes(typeof(ProviderDefinitionAttribute), true).First();
                    if (providerAttribute != null)
                    {
                        if (proxyAttribute != null && providerAttribute.MultiInstance)
                        {
                            providerProxyImplementations.Add(new ProviderImplementation(providerType, type));
                        }
                        else
                        {
                            providerImplementations.Add(new ProviderImplementation(providerType, type));
                        }
                    }
                }
            }
        }
        
        public T GetProvider<T>() where T : class
        {
            return (T)GetProvider(typeof(T));
        }

        public object GetProvider(Type providerDefinitionType)
        {
            if (!providerDefinitionType.IsInterface) throw new ArgumentException($"The type {providerDefinitionType.FullName} is no interface");
            if (!providerDefinitionTypes.Contains(providerDefinitionType)) throw new ArgumentException($"The type {providerDefinitionType.FullName} is not a known provider interface");

            object proxyImplementation = providerProxyImplementations.Where(p => p.Definition.Type.FullName.Equals(providerDefinitionType.FullName, StringComparison.OrdinalIgnoreCase) && p.Enabled).FirstOrDefault()?.Implementation;
            if (proxyImplementation != null) return proxyImplementation;


            object providerImplementation = providerImplementations.Where(p => p.Definition.Type.FullName.Equals(providerDefinitionType.FullName, StringComparison.OrdinalIgnoreCase) && p.Enabled).FirstOrDefault()?.Implementation;
            if (providerImplementation != null) return providerImplementation;

            return null;
        }
    }
}
