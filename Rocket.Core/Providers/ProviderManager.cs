using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Rocket.API.Providers;
using System.Reflection;

namespace Rocket.Core.Providers
{
    public class ProviderManager : IProviderManager
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

        internal void LoadRocketProviders()
        {
            Type[] types = new Type[0]; //TODO: Get types from folder Providers and Rocket.Core

            loadProviderDefinitionsFromTypes(types);
            loadProviderImplementationsFromTypes(types);

            instanciateProviderImplemenations();

            loadProviderImplementations();
            
            OnProvidersLoaded?.Invoke();

        }

        public T CreateInstance<T>(params object[] arguments)
        {
            Type type = typeof(T);
            //TODO: Instanciate new object with injected providers and optional arguments
            ConstructorInfo constructor = type.GetConstructors().OrderBy(c => c.GetParameters().Length).FirstOrDefault();//TODO: Decide which constructor to use for dependency injection (find the one closest to the arguments that we can satisfy)

            //
            ParameterInfo[] parameters = constructor.GetParameters();
            object[] constructorArguments = arguments;
            //Prepare & add all the instances required from the provider list

            return (T)Activator.CreateInstance(type, constructorArguments);
        }

        public T Call<T>(object context,string methodName,params object[] arguments)
        {
            Type type = context.GetType();
            MethodInfo method = type.GetMethod(methodName);
            ParameterInfo[] parameters = method.GetParameters();

            object[] methodArguments = arguments;
            //Prepare & add all the instances required from the provider list

            return (T)method.Invoke(context, methodArguments);
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

                foreach (Type providerType in type.GetInterfaces())
                {
                    ProviderDefinitionAttribute providerAttribute = (ProviderDefinitionAttribute)providerType.GetCustomAttributes(typeof(ProviderDefinitionAttribute), true).First();
                    if (providerAttribute != null)
                    {
                        if (type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(ProxyBase<Type>).GetGenericTypeDefinition() && providerAttribute.MultiInstance)
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
            if (providerDefinitionType.GetType() == typeof(IProviderManager)) return this;
            if (!providerDefinitionType.IsInterface) throw new ArgumentException($"The type {providerDefinitionType.FullName} is no interface");
            if (!providerDefinitionTypes.Contains(providerDefinitionType)) throw new ArgumentException($"The type {providerDefinitionType.FullName} is not a known provider interface");

            object proxyImplementation = providerProxyImplementations.Where(p => p.Definition.Type.FullName.Equals(providerDefinitionType.FullName, StringComparison.OrdinalIgnoreCase) && p.Enabled).FirstOrDefault()?.Implementation;
            if (proxyImplementation != null) return proxyImplementation;


            object providerImplementation = providerImplementations.Where(p => p.Definition.Type.FullName.Equals(providerDefinitionType.FullName, StringComparison.OrdinalIgnoreCase) && p.Enabled).FirstOrDefault()?.Implementation;
            if (providerImplementation != null) return providerImplementation;

            return null;
        }

        internal List<T> GetProviders<T>()
        {
            throw new NotImplementedException();
        }

        public void Reload()
        {
            throw new NotImplementedException();
        }
    }
}
