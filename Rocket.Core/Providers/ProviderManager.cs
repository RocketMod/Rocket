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

        private MethodBase getMatchingMethod(object context, MethodBase[] type, object[] extraParameters)
        {
            return getMatchingMethod(context.GetType(), type, extraParameters);
        }

        private MethodBase getMatchingMethod(MethodBase[] methods, object[] extraParameters)
        {
            foreach(MethodBase method in methods){
                ParameterInfo[]  parameters = method.GetParameters();
                int extraParametersIndex = 0;
                foreach (ParameterInfo parameter in parameters)
                {
                    Type parameterType = parameter.ParameterType;
                    object provider = GetProvider(parameterType);
                    if (provider != null) continue;
                    if(extraParametersIndex == extraParameters.Length)
                    {
                        return method;
                    }
                    if (extraParameters[extraParametersIndex].GetType() == parameterType)
                    {
                        extraParametersIndex++;
                    }
                    else break;
                }
            }
            throw new Exception("Couldn't find matching method");
        }

        private object[] resolveArguments(ParameterInfo[] parameters, object[] extraParameters)
        {
            List<object> result = new List<object>();

            int extraParametersIndex = 0;
            foreach (ParameterInfo parameter in parameters)
            {
                Type parameterType = parameter.ParameterType;
                object provider = GetProvider(parameterType);
                if(provider != null)
                {
                    result.Add(provider);
                }
                if (extraParameters.GetType() == parameterType && extraParameters.Length < extraParametersIndex)
                {
                    result.Add(extraParameters[extraParametersIndex++]);
                }
                else
                {
                    if(parameterType.IsInterface) throw new Exception("Couldn't resolve provider "+ parameterType.FullName);
                    throw new Exception("Parameters don't match for " + parameterType.FullName);
                }
            }
            return result.ToArray();
        }


        public T CreateInstance<T>(params object[] extraParameters)
        {
            return (T)CreateInstance(typeof(T), extraParameters);
        }

        public object CreateInstance(Type type, params object[] extraParameters)
        {
            try
            {
                MethodBase[] constructors = type.GetConstructors();
                MethodBase constructor = getMatchingMethod(constructors, extraParameters);
                return Activator.CreateInstance(type, resolveArguments(constructor.GetParameters(), extraParameters));
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to construct type " + type.FullName, ex);
                throw;
            }
        }

        public T Call<T>(object context, string methodName, params object[] extraParameters)
        {
            return (T)Call(context, methodName, extraParameters);
        }

        public object Call(object context, string methodName, params object[] extraParameters)
        {
            Type type = context.GetType();
            try
            {
                MethodBase[] methods = type.GetMethods().Where(c => c.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase)).ToArray();
                MethodBase method = getMatchingMethod(context, methods, extraParameters);
                return method.Invoke(context, resolveArguments(method.GetParameters(), extraParameters));
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to call method " + type.FullName + "." + methodName, ex);
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


