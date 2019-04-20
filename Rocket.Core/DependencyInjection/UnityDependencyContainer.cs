using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.Configuration;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Lifetime;

namespace Rocket.Core.DependencyInjection
{
    public class UnityDependencyContainer : IDependencyContainer
    {
        private readonly IUnityContainer container;

        public UnityDependencyContainer()
        {
            container = new UnityContainer();
            container.RegisterInstance<IDependencyContainer>(this);
            container.RegisterInstance<IDependencyResolver>(this);
        }

        private UnityDependencyContainer(UnityDependencyContainer parent)
        {
            ParentContainer = parent;
            container = parent.container.CreateChildContainer();
            container.RegisterInstance<IDependencyContainer>(this);
            container.RegisterInstance<IDependencyResolver>(this);
        }

        private ILogger Logger
        {
            get
            {
                TryResolve(null, out ILogger log);
                return log;
            }
        }

        #region IDependencyContainer Host

        public IDependencyContainer CreateChildContainer() => new UnityDependencyContainer(this);
        public IDependencyContainer ParentContainer { get; }

        public void AddSingleton<TInterface, TClass>(params string[] mappingNames) where TClass : TInterface
        {
            if (!typeof(ILogger).IsAssignableFrom(typeof(TInterface)))
                Logger?.LogTrace("\t\tRegistering singleton: <"
                    + typeof(TInterface).Name
                    + ", "
                    + typeof(TClass).Name
                    + ">; mappings: ["
                    + string.Join(", ", mappingNames)
                    + "]");

            if (mappingNames == null || mappingNames.Length == 0)
                mappingNames = new string[] { null };

            string primaryName = mappingNames.First();
            container.RegisterType<TInterface, TClass>(primaryName, new ContainerControlledLifetimeManager());

            List<string> pendingNames = mappingNames.Skip(1).ToList();

            TInterface instance = container.Resolve<TInterface>(primaryName);
            foreach (string name in pendingNames)
                AddTransient(instance, name);
        }

        public void AddSingleton<TInterface>(TInterface value, params string[] mappingNames)
        {
            if (!typeof(ILogger).IsAssignableFrom(typeof(TInterface)))
                Logger?.LogTrace("\t\tRegistering singleton instance: <"
                    + typeof(TInterface).Name
                    + ", "
                    + value.GetType().Name
                    + ">; mappings: ["
                    + string.Join(", ", mappingNames)
                    + "]");

            if (mappingNames == null || mappingNames.Length == 0)
                mappingNames = new string[] { null };

            foreach (string mappingName in mappingNames)
                container.RegisterInstance(mappingName, value, new ContainerControlledLifetimeManager());
        }

        public void AddTransient<TInterface, TClass>(params string[] mappingNames) where TClass : TInterface
        {
            if (!typeof(ILogger).IsAssignableFrom(typeof(TInterface)))
                Logger?.LogTrace("\t\tRegistering type: <"
                    + typeof(TInterface).Name
                    + ", "
                    + typeof(TClass).Name
                    + ">; mappings: ["
                    + string.Join(", ", mappingNames)
                    + "]");

            if (mappingNames == null || mappingNames.Length == 0)
                mappingNames = new string[] { null };

            foreach (string mappingName in mappingNames)
                container.RegisterType<TInterface, TClass>(mappingName);
        }

        public void AddTransient<TInterface>(TInterface value, params string[] mappingNames)
        {
            if (!typeof(ILogger).IsAssignableFrom(typeof(TInterface)))
                Logger?.LogTrace("\t\tRegistering type instance: <"
                    + typeof(TInterface).Name
                    + ", "
                    + value.GetType().Name
                    + ">; mappings: ["
                    + string.Join(", ", mappingNames)
                    + "]");

            if (mappingNames == null || mappingNames.Length == 0)
                mappingNames = new string[] { null };

            foreach (string mappingName in mappingNames)
                container.RegisterInstance(mappingName, value);
        }

        public void Remove<T>(params string[] mappingNames)
        {
            Remove(typeof(T), mappingNames);
        }

        public void Remove(Type type, params string[] mappingNames)
        {
            foreach (IContainerRegistration registration in container.Registrations
                                                                    .Where(p => p.RegisteredType == type
                                                                        && p.LifetimeManager?.GetType() == typeof(ContainerControlledLifetimeManager)
                                                                        && (mappingNames == null
                                                                            || mappingNames.Length == 0
                                                                            || mappingNames.Any(c
                                                                                => c.Equals(p.Name)))))
                registration.LifetimeManager.RemoveValue();
        }

        public void Dispose()
        {
            (container as UnityContainer)?.Dispose();
        }

        #endregion

        #region IDependencyResolver Host

        #region IsRegistered Methods

        public bool IsRegistered<T>(string mappingName = null) => container.IsRegistered<T>(mappingName);

        public bool IsRegistered(Type type, string mappingName = null) => container.IsRegistered(type, mappingName);

        #endregion

        #region Activate Methods

        public T Activate<T>() => (T)Activate(typeof(T));

        [DebuggerStepThrough]
        public object Activate(Type type)
        {
            if (!typeof(ILogger).IsAssignableFrom(type))
                Logger?.LogTrace("Activating: " + type.Name);

            foreach (ConstructorInfo constructor in type.GetConstructors())
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Length <= 0) return Activator.CreateInstance(type);

                List<object> objectList = new List<object>();
                foreach (ParameterInfo parameterInfo in parameters)
                {
                    Type parameterType = parameterInfo.ParameterType;
                    if (!container.IsRegistered(parameterType))
                    {
                        if (!typeof(ILogger).IsAssignableFrom(type))
                            Logger?.LogError(
                                $"Failed to activate \"{type.Name}\" because the parameter type \"{parameterType.Name}\" could not be resolved.");
                        return null;
                    }

                    objectList.Add(Resolve(parameterType));
                }

                return constructor.Invoke(objectList.ToArray());
            }

            return null;
        }

        #endregion

        #region Get Methods

        /// <exception cref="ServiceResolutionFailedException">
        ///     Thrown when no instance is resolved for the requested Type and
        ///     Mapping.
        /// </exception>
        public T Resolve<T>(string mappingName = null)
        {
            if (!typeof(ILogger).IsAssignableFrom(typeof(T)))
                Logger?.LogTrace("Trying to resolve: <" + typeof(T).Name + ">; mappingName: " + mappingName);

            if (IsRegistered<T>(mappingName))
                return container.Resolve<T>(mappingName);

            throw new ServiceResolutionFailedException(typeof(T), mappingName);
        }

        /// <exception cref="ServiceResolutionFailedException">
        ///     Thrown when no instance is resolved for the requested Type and
        ///     Mapping.
        /// </exception>
        public T Resolve<T>(string mappingName, params object[] parameters)
        {
            if (!typeof(ILogger).IsAssignableFrom(typeof(T)))
                Logger?.LogTrace("Trying to resolve: <" + typeof(T).Name + ">; mappingName: " + mappingName);

            if (parameters.Length > 0)
            {
                throw new NotSupportedException("Resolving with parameters is currently not supported.");
            }

            if (IsRegistered<T>(mappingName))
                return container.Resolve<T>(mappingName/*, new OrderedParametersOverride(parameters)*/);

            throw new ServiceResolutionFailedException(typeof(T), mappingName);
        }

        /// <exception cref="ServiceResolutionFailedException">
        ///     Thrown when no instance is resolved for the requested Type and
        ///     Mapping.
        /// </exception>
        public object Resolve(Type serviceType, string mappingName = null)
        {
            if (!typeof(ILogger).IsAssignableFrom(serviceType))
                Logger?.LogTrace("Trying to resolve: <" + serviceType.Name + ">; mappingName: " + mappingName);

            if (IsRegistered(serviceType, mappingName))
                return container.Resolve(serviceType, mappingName);

            throw new ServiceResolutionFailedException(serviceType, mappingName);
        }

        /// <exception cref="ServiceResolutionFailedException">
        ///     Thrown when no instance is resolved for the requested Type and
        ///     Mapping.
        /// </exception>
        public object Resolve(Type serviceType, string mappingName, params object[] parameters)
        {
            if (!typeof(ILogger).IsAssignableFrom(serviceType))
                Logger?.LogTrace("Trying to resolve: <" + serviceType.Name + ">; mappingName: " + mappingName);

            if (parameters.Length > 0)
            {
                throw new NotSupportedException("Resolving with parameters is currently not supported.");
            }

            if (IsRegistered(serviceType, mappingName))
                return container.Resolve(serviceType, mappingName /*, new OrderedParametersOverride(parameters)*/);

            throw new ServiceResolutionFailedException(serviceType, mappingName);
        }

        /// <exception cref="ServiceResolutionFailedException">Thrown when no instances are resolved for the requested Type.</exception>
        public IEnumerable<T> ResolveAll<T>()
        {
            if (!typeof(ILogger).IsAssignableFrom(typeof(T)))
                Logger?.LogTrace("Trying to resolve all: <" + typeof(T).Name + ">");

            return container.ResolveAll<T>()
                            .Where(c => !(c is IServiceProxy));
        }

        /// <exception cref="ServiceResolutionFailedException">Thrown when no instances are resolved for the requested Type.</exception>
        public IEnumerable<T> ResolveAll<T>(params object[] parameters)
        {
            if (!typeof(ILogger).IsAssignableFrom(typeof(T)))
                Logger?.LogTrace("Trying to resolve all: <" + typeof(T).Name + ">");

            if (parameters.Length > 0)
            {
                throw new NotSupportedException("Resolving with parameters is currently not supported.");
            }

            return container.ResolveAll<T>(/*new OrderedParametersOverride(parameters)*/)
                            .Where(c => !(c is IServiceProxy));
        }

        /// <exception cref="ServiceResolutionFailedException">Thrown when no instances are resolved for the requested Type.</exception>
        public IEnumerable<object> ResolveAll(Type type)
        {
            if (!typeof(ILogger).IsAssignableFrom(type))
                Logger?.LogTrace("Trying to resolve all: <" + type.Name + ">");

            return container.ResolveAll(type)
                            .Where(c => !(c is IServiceProxy));
        }

        /// <exception cref="ServiceResolutionFailedException">Thrown when no instances are resolved for the requested Type.</exception>
        public IEnumerable<object> ResolveAll(Type type, params object[] parameters)
        {
            if (!typeof(ILogger).IsAssignableFrom(type))
                Logger?.LogTrace("Trying to resolve all: <" + type.Name + ">");

            if (parameters.Length > 0)
            {
                throw new NotSupportedException("Resolving with parameters is currently not supported.");
            }

            return container.ResolveAll(type/*, new OrderedParametersOverride(parameters)*/)
                            .Where(c => !(c is IServiceProxy));
        }

        #endregion

        #region TryResolve Methods

        /// <returns>
        ///     <value>true</value>
        ///     when an instance is resolved.
        /// </returns>
        public bool TryResolve<T>(string mappingName, out T output)
        {
            if (!typeof(ILogger).IsAssignableFrom(typeof(T)) && !typeof(IRocketConfigurationProvider).IsAssignableFrom(typeof(T)))
                Logger?.LogTrace("Trying to resolve: <" + typeof(T).Name + ">; mappingName: " + mappingName);

            if (IsRegistered<T>(mappingName))
            {
                output = container.Resolve<T>(mappingName);

                return true;
            }

            output = default(T);

            return false;
        }

        /// <returns>
        ///     <value>true</value>
        ///     when an instance is resolved.
        /// </returns>
        public bool TryResolve<T>(string mappingName, out T output, params object[] parameters)
        {
            if (!typeof(ILogger).IsAssignableFrom(typeof(T)))
                Logger?.LogTrace("Trying to resolve: <" + typeof(T).Name + ">; mappingName: " + mappingName);

            if (parameters.Length > 0)
            {
                throw new NotSupportedException("Resolving with parameters is currently not supported.");
            }

            if (IsRegistered<T>(mappingName))
            {
                output = container.Resolve<T>(mappingName/*, new OrderedParametersOverride(parameters)*/);

                return true;
            }

            output = default(T);

            return false;
        }

        /// <returns>
        ///     <value>true</value>
        ///     when an instance is resolved.
        /// </returns>
        public bool TryResolve(Type serviceType, string mappingName, out object output)
        {
            if (!typeof(ILogger).IsAssignableFrom(serviceType))
                Logger?.LogTrace("Trying to resolve: <" + serviceType.Name + ">; mappingName: " + mappingName);

            if (IsRegistered(serviceType, mappingName))
            {
                output = container.Resolve(serviceType, mappingName);

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
        public bool TryResolve(Type serviceType, string mappingName, out object output, params object[] parameters)
        {
            if (!typeof(ILogger).IsAssignableFrom(serviceType))
                Logger?.LogTrace("Trying to resolve: <" + serviceType.Name + ">; mappingName: " + mappingName);

            if (parameters.Length > 0)
            {
                throw new NotSupportedException("Resolving with parameters is currently not supported.");
            }

            if (IsRegistered(serviceType, mappingName))
            {
                output = container.Resolve(serviceType, mappingName/*, new OrderedParametersOverride(parameters)*/);

                return true;
            }

            if (serviceType.IsValueType)
                output = Activator.CreateInstance(serviceType);
            else
                output = null;

            return false;
        }

        #endregion

        #endregion
    }
}