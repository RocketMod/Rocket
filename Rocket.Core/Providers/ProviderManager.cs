using System;
using System.Collections.Generic;
using System.Data.Odbc;
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

        [Obsolete]
        //TODO: Remove after reusing in the other method
        private bool GetDIMatch(MethodBase[] targets, out MethodBase match, out object[] matchArguments, params object[] customArguments)
        {
            match = null;
            matchArguments = null;

            bool matched = false;
            foreach (var target in targets)
            {
                bool success = //GetDIArguments(target.GetParameters(), out matchArguments, customArguments);
                if (success && matched)
                {
                    return false; // multiple matches
                }
                if (success)
                {
                    matched = true;
                    match = target;
                }
            }


            if (matched)
                return true;

            match = null;
            matchArguments = null;
            return false;
        }

        [Obsolete]
        //TODO: Remove after reusing in the other method
        private bool getFittingMethod(ParameterInfo[] parameters, out object[] args, params object[] customArguments)
        {
            args = null;
            var result = new List<object>();

            int nIndex = 0;
            foreach (var param in parameters)
            {
                var provider = GetProvider(param.ParameterType);

                if (provider != null)
                {
                    result.Add(provider);
                    continue;
                }

                result.Add(customArguments[nIndex]);
                nIndex++;
            }

            if (result.Count != parameters.Length)
                return false;
            

            if (parameters.Where((param, i) => result[i] != null && !param.ParameterType.IsInstanceOfType(result[i])).Any())
                return false;

            args = result.ToArray();
            
            return true;
        }


        private MethodInfo getMatchingMethod(object context, string methodName, object[] customArguments)
        {
            Type type = context.GetType();
            MethodInfo[] methods = type.GetMethods().Where(c => c.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase)).ToArray();
            //TODO: Get matching method by customArguments and name
            return methods[0];
        }

        private ConstructorInfo getMatchingConstructor(Type type,object[] customArguments)
        {
            ConstructorInfo[] methods = type.GetConstructors();
            //TODO: Get matching constructor by customArguments
            return methods[0];
        }

        private object[] resolveArguments(ParameterInfo[] parameters, object[] customArguments)
        {
            //TODO: Resolve dependencies and return merged array to pass to the method / constructor
            return new object[0];
        }


        public T CreateInstance<T>(params object[] customArguments)
        {
            return (T)CreateInstance(typeof(T), customArguments);
        }

        public object CreateInstance(Type type, params object[] customArguments)
        {
            try
            {
                ConstructorInfo constructor = getMatchingConstructor(type, customArguments);
                return Activator.CreateInstance(type, resolveArguments(constructor.GetParameters(), customArguments));
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to construct type: " + type.FullName, ex);
                throw;
            }
        }


        public T Call<T>(object context, string methodName, params object[] customArguments)
        {
            return (T)Call(context, methodName, customArguments);
        }

        public object Call(object context, string methodName, params object[] customArguments)
        {
            Type type = context.GetType();
            try
            {
                MethodInfo method = getMatchingMethod(context, methodName, customArguments);
                return method.Invoke(context, resolveArguments(method.GetParameters(),customArguments));
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to call method: " + type.FullName + "." + methodName,ex);
            }
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
