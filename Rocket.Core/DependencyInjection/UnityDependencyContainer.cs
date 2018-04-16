using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.API.ServiceProxies;

namespace Rocket.Core.DependencyInjection
{
    public class UnityDependencyContainer : IDependencyContainer
    {
        private readonly IUnityContainer container;
        private ILogger Logger
        {
            get
            {
                TryGet<ILogger>(null, out var log);
                return log;
            }
        }

        public UnityDependencyContainer()
        {
            container = new UnityContainer();
            container.RegisterInstance<IDependencyContainer>(this);
            container.RegisterInstance<IDependencyResolver>(this);
        }

        private UnityDependencyContainer(UnityDependencyContainer parent)
        {
            container = parent.container.CreateChildContainer();
            container.RegisterInstance<IDependencyContainer>(this);
            container.RegisterInstance<IDependencyResolver>(this);
        }

        #region IDependencyContainer Implementation

        public IDependencyContainer CreateChildContainer() => new UnityDependencyContainer(this);

        public void RegisterSingletonType<TInterface, TClass>(params string[] mappingNames) where TClass : TInterface
        {
            if (!(typeof(ILogger).IsAssignableFrom(typeof(TInterface))))
                Logger?.LogDebug("\t\tRegistering singleton: <" + typeof(TInterface).Name + ", " + typeof(TClass).Name + ">; mappings: [" + string.Join(", ", mappingNames) + "]");

            if (mappingNames == null || mappingNames.Length == 0)
                mappingNames = new string[] { null };

            foreach (string mappingName in mappingNames)
                container.RegisterType<TInterface, TClass>(mappingName, new ContainerControlledLifetimeManager());
        }

        public void RegisterSingletonInstance<TInterface>(TInterface value, params string[] mappingNames)
        {
            if (!(typeof(ILogger).IsAssignableFrom(typeof(TInterface))))
                Logger?.LogDebug("\t\tRegistering singleton instance: <" + typeof(TInterface).Name + ", " + value.GetType().Name + ">; mappings: [" + string.Join(", ", mappingNames) + "]");

            if (mappingNames == null || mappingNames.Length == 0)
                mappingNames = new string[] { null };

            foreach (string mappingName in mappingNames)
                container.RegisterInstance(mappingName, value, new ContainerControlledLifetimeManager());
        }

        public void RegisterType<TInterface, TClass>(params string[] mappingNames) where TClass : TInterface
        {
            if (!(typeof(ILogger).IsAssignableFrom(typeof(TInterface))))
                Logger?.LogDebug("\t\tRegistering type: <" + typeof(TInterface).Name + ", " + typeof(TClass).Name + ">; mappings: [" + string.Join(", ", mappingNames) + "]");

            if (mappingNames == null || mappingNames.Length == 0)
                mappingNames = new string[] { null };

            foreach (string mappingName in mappingNames)
                container.RegisterType<TInterface, TClass>(mappingName);
        }

        public void RegisterInstance<TInterface>(TInterface value, params string[] mappingNames)
        {
            if (!(typeof(ILogger).IsAssignableFrom(typeof(TInterface))))
                Logger?.LogDebug("\t\tRegistering type instance: <" + typeof(TInterface).Name + ", " + value.GetType().Name + ">; mappings: [" + string.Join(", ", mappingNames) + "]");

            if (mappingNames == null || mappingNames.Length == 0)
                mappingNames = new string[] { null };

            foreach (string mappingName in mappingNames)
                container.RegisterInstance(mappingName, value);
        }

        #endregion

        #region IDependencyResolver Implementation

        #region IsRegistered Methods

        public bool IsRegistered<T>(string mappingName = null) => container.IsRegistered<T>(mappingName);

        public bool IsRegistered(Type type, string mappingName = null) => container.IsRegistered(type, mappingName);

        #endregion

        #region Activate Methods

        public T Activate<T>() => (T)Activate(typeof(T));

        [DebuggerStepThrough]
        public object Activate(Type type)
        {
            if (!(typeof(ILogger).IsAssignableFrom(type)))
                Logger?.LogDebug("Activating: " + type.Name);

            foreach (ConstructorInfo constructor in type.GetConstructors())
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Length <= 0) return Activator.CreateInstance(type);

                List<object> objectList = new List<object>();
                foreach (ParameterInfo parameterInfo in parameters)
                {
                    Type parameterType = parameterInfo.ParameterType;
                    if (!container.IsRegistered(parameterType)) return null;
                    objectList.Add(Get(parameterType));
                }

                return constructor.Invoke(objectList.ToArray());
            }

            return null;
        }

        #endregion

        #region Get Methods

        /// <exception cref="NotResolvedException">
        ///     Thrown when no instance is resolved for the requested Type and
        ///     Mapping.
        /// </exception>
        public T Get<T>(string mappingName = null)
        {
            if (!(typeof(ILogger).IsAssignableFrom(typeof(T))))
                Logger?.LogDebug("Trying to resolve: <" + typeof(T).Name + ">; mappingName: " + mappingName);

            if (IsRegistered<T>(mappingName))
                return container.Resolve<T>(mappingName, new OrderedParametersOverride(new object[0]));

            throw new NotResolvedException(typeof(T), mappingName);
        }

        /// <exception cref="NotResolvedException">
        ///     Thrown when no instance is resolved for the requested Type and
        ///     Mapping.
        /// </exception>
        public T Get<T>(string mappingName, params object[] parameters)
        {
            if (!(typeof(ILogger).IsAssignableFrom(typeof(T))))
                Logger?.LogDebug("Trying to resolve: <" + typeof(T).Name + ">; mappingName: " + mappingName);

            if (IsRegistered<T>(mappingName))
                return container.Resolve<T>(mappingName, new OrderedParametersOverride(parameters));

            throw new NotResolvedException(typeof(T), mappingName);
        }

        /// <exception cref="NotResolvedException">
        ///     Thrown when no instance is resolved for the requested Type and
        ///     Mapping.
        /// </exception>
        public object Get(Type serviceType, string mappingName = null)
        {
            if (!(typeof(ILogger).IsAssignableFrom(serviceType)))
                Logger?.LogDebug("Trying to resolve: <" + serviceType.Name + ">; mappingName: " + mappingName);

            if (IsRegistered(serviceType, mappingName))
                return container.Resolve(serviceType, mappingName, new OrderedParametersOverride(new object[0]));

            throw new NotResolvedException(serviceType, mappingName);
        }

        /// <exception cref="NotResolvedException">
        ///     Thrown when no instance is resolved for the requested Type and
        ///     Mapping.
        /// </exception>
        public object Get(Type serviceType, string mappingName, params object[] parameters)
        {
            if (!(typeof(ILogger).IsAssignableFrom(serviceType)))
                Logger?.LogDebug("Trying to resolve: <" + serviceType.Name + ">; mappingName: " + mappingName);

            if (IsRegistered(serviceType, mappingName))
                return container.Resolve(serviceType, mappingName, new OrderedParametersOverride(parameters));

            throw new NotResolvedException(serviceType, mappingName);
        }

        /// <exception cref="NotResolvedException">Thrown when no instances are resolved for the requested Type.</exception>
        public IEnumerable<T> GetAll<T>()
        {
            if (!(typeof(ILogger).IsAssignableFrom(typeof(T))))
                Logger?.LogDebug("Trying to resolve all: <" + typeof(T).Name + ">");

            IEnumerable<T> instances = container.ResolveAll<T>()
                                                .Where(c => !(c is IServiceProxy));

            if (instances.Count() != 0) return instances;

            throw new NotResolvedException(typeof(T));
        }

        /// <exception cref="NotResolvedException">Thrown when no instances are resolved for the requested Type.</exception>
        public IEnumerable<T> GetAll<T>(params object[] parameters)
        {
            if (!(typeof(ILogger).IsAssignableFrom(typeof(T))))
                Logger?.LogDebug("Trying to resolve all: <" + typeof(T).Name + ">");

            IEnumerable<T> instances = container.ResolveAll<T>(new OrderedParametersOverride(parameters))
                                                .Where(c => !(c is IServiceProxy));

            if (instances.Count() != 0) return instances;

            throw new NotResolvedException(typeof(T));
        }

        /// <exception cref="NotResolvedException">Thrown when no instances are resolved for the requested Type.</exception>
        public IEnumerable<object> GetAll(Type type)
        {
            if (!(typeof(ILogger).IsAssignableFrom(type)))
                Logger?.LogDebug("Trying to resolve all: <" + type.Name + ">");

            IEnumerable<object> instances = container.ResolveAll(type)
                                                     .Where(c => !(c is IServiceProxy));

            if (instances.Count() != 0) return instances;

            throw new NotResolvedException(type);
        }

        /// <exception cref="NotResolvedException">Thrown when no instances are resolved for the requested Type.</exception>
        public IEnumerable<object> GetAll(Type type, params object[] parameters)
        {
            if (!(typeof(ILogger).IsAssignableFrom(type)))
                Logger?.LogDebug("Trying to resolve all: <" + type.Name + ">");

            IEnumerable<object> instances = container.ResolveAll(type, new OrderedParametersOverride(parameters))
                                                     .Where(c => !(c is IServiceProxy));

            if (instances.Count() != 0) return instances;

            throw new NotResolvedException(type);
        }

        #endregion

        #region TryGet Methods

        /// <returns>
        ///     <value>true</value>
        ///     when an instance is resolved.
        /// </returns>
        public bool TryGet<T>(string mappingName, out T output)
        {
            if (!(typeof(ILogger).IsAssignableFrom(typeof(T))))
                Logger?.LogDebug("Trying to resolve: <" + typeof(T).Name + ">; mappingName: " + mappingName);

            if (IsRegistered<T>(mappingName))
            {
                output = container.Resolve<T>(mappingName, new OrderedParametersOverride(new object[0]));

                return true;
            }

            output = default(T);

            return false;
        }

        /// <returns>
        ///     <value>true</value>
        ///     when an instance is resolved.
        /// </returns>
        public bool TryGet<T>(string mappingName, out T output, params object[] parameters)
        {
            if (!(typeof(ILogger).IsAssignableFrom(typeof(T))))
                Logger?.LogDebug("Trying to resolve: <" + typeof(T).Name + ">; mappingName: " + mappingName);

            if (IsRegistered<T>(mappingName))
            {
                output = container.Resolve<T>(mappingName, new OrderedParametersOverride(parameters));

                return true;
            }

            output = default(T);

            return false;
        }

        /// <returns>
        ///     <value>true</value>
        ///     when an instance is resolved.
        /// </returns>
        public bool TryGet(Type serviceType, string mappingName, out object output)
        {
            if (!(typeof(ILogger).IsAssignableFrom(serviceType)))
                Logger?.LogDebug("Trying to resolve: <" + serviceType.Name + ">; mappingName: " + mappingName);

            if (IsRegistered(serviceType, mappingName))
            {
                output = container.Resolve(serviceType, mappingName, new OrderedParametersOverride(new object[0]));

                return true;
            }

            if (serviceType.IsValueType)
                output = Activator.CreateInstance(serviceType);
            else
                output = null;

            return false;
        }

        /// <returns>
        ///     <value>true</value>
        ///     when an instance is resolved.
        /// </returns>
        public bool TryGet(Type serviceType, string mappingName, out object output, params object[] parameters)
        {
            if (!(typeof(ILogger).IsAssignableFrom(serviceType)))
                Logger?.LogDebug("Trying to resolve: <" + serviceType.Name + ">; mappingName: " + mappingName);

            if (IsRegistered(serviceType, mappingName))
            {
                output = container.Resolve(serviceType, mappingName, new OrderedParametersOverride(parameters));

                return true;
            }

            if (serviceType.IsValueType)
                output = Activator.CreateInstance(serviceType);
            else
                output = null;

            return false;
        }

        /// <returns>
        ///     <value>true</value>
        ///     when at least one instance is resolved.
        /// </returns>
        public bool TryGetAll<T>(out IEnumerable<T> output)
        {
            if (!(typeof(ILogger).IsAssignableFrom(typeof(T))))
                Logger?.LogDebug("Trying to resolve all: <" + typeof(T).Name + ">");

            output = container.ResolveAll<T>()
                              .Where(c => !(c is IServiceProxy));

            if (output.Count() != 0) return true;

            output = new List<T>();
            return false;
        }

        /// <returns>
        ///     <value>true</value>
        ///     when at least one instance is resolved.
        /// </returns>
        public bool TryGetAll<T>(out IEnumerable<T> output, params object[] parameters)
        {
            if (!(typeof(ILogger).IsAssignableFrom(typeof(T))))
                Logger?.LogDebug("Trying to resolve all: <" + typeof(T).Name + ">");

            output = container.ResolveAll<T>(new OrderedParametersOverride(parameters))
                              .Where(c => !(c is IServiceProxy));

            if (output.Count() != 0) return true;

            output = new List<T>();
            return false;
        }

        /// <returns>
        ///     <value>true</value>
        ///     when at least one instance is resolved.
        /// </returns>
        public bool TryGetAll(Type serviceType, out IEnumerable<object> output)
        {
            if (!(typeof(ILogger).IsAssignableFrom(serviceType)))
                Logger?.LogDebug("Trying to resolve all: <" + serviceType.Name + ">");

            output = container.ResolveAll(serviceType)
                              .Where(c => !(c is IServiceProxy));

            if (output.Count() != 0) return true;

            output = new List<object>();
            return false;
        }

        /// <returns>
        ///     <value>true</value>
        ///     when at least one instance is resolved.
        /// </returns>
        public bool TryGetAll(Type serviceType, out IEnumerable<object> output, params object[] parameters)
        {
            if (!(typeof(ILogger).IsAssignableFrom(serviceType)))
                Logger?.LogDebug("Trying to resolve all: <" + serviceType.Name + ">");

            output = container.ResolveAll(serviceType, new OrderedParametersOverride(parameters))
                              .Where(c => !(c is IServiceProxy));

            if (output.Count() != 0) return true;

            output = new List<object>();
            return false;
        }

        #endregion

        #endregion
    }
}