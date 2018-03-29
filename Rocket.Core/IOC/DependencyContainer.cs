using Microsoft.Practices.Unity;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Rocket.API.IOC;

namespace Rocket.IOC
{
    public class DependencyContainer : IDependencyResolver, IDependencyContainer
    {
        private readonly IUnityContainer container;
        public IServiceLocator ServiceLocator { get; private set; }

        public DependencyContainer()
        {
            container = new UnityContainer();
            container.RegisterInstance<IDependencyContainer>(this);
            container.RegisterInstance<IDependencyResolver>(this);
            Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            ServiceLocator = new ServiceLocator(Microsoft.Practices.ServiceLocation.ServiceLocator.Current);
        }

        #region IDependencyContainer Implementation
        public void RegisterSingletonType<TInterface, TClass>(string mappingName = null) where TClass : TInterface
        {
            container.RegisterType<TInterface, TClass>(mappingName, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
        }

        public void RegisterSingletonInstance<TInterface>(TInterface value, string mappingName = null)
        {
            container.RegisterInstance<TInterface>(mappingName, value, new ContainerControlledLifetimeManager());
        }

        public void RegisterType<TInterface, TClass>(string mappingName = null) where TClass : TInterface
        {
            container.RegisterType<TInterface, TClass>(mappingName);
        }

        public void RegisterInstance<TInterface>(TInterface value, string mappingName = null)
        {
            container.RegisterInstance<TInterface>(mappingName, value);
        }
        #endregion

        #region IDependencyResolver Implementation
        #region IsRegistered Methods

        public bool IsRegistered<T>(string mappingName = null)
        {
            return container.IsRegistered<T>(mappingName);
        }

        public bool IsRegistered(Type type, string mappingName = null)
        {
            return container.IsRegistered(type, mappingName);
        }
        #endregion

        #region Activate Methods

        public T Activate<T>()
        {
            return (T)Activate(typeof(T));
        }

        public object Activate(Type type)
        {
            foreach (ConstructorInfo constructor in type.GetConstructors())
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Length <= 0)
                    return Activator.CreateInstance(type);

                List<object> objectList = new List<object>();
                foreach (ParameterInfo parameterInfo in parameters)
                {
                    Type parameterType = parameterInfo.ParameterType;
                    if (!container.IsRegistered(parameterType))
                    {
                        return null;
                    }
                    objectList.Add(Get(parameterType));
                }
                return constructor.Invoke(objectList.ToArray());
            }
            return null;
        }
        #endregion

        #region Get Methods

        /// <exception cref="UnityInstanceNotResolvedException">Thrown when no instance is resolved for the requested Type and Mapping.</exception>
        public T Get<T>(string mappingName = null)
        {
            if (IsRegistered<T>(mappingName))
            {
                return container.Resolve<T>(mappingName, new OrderedParametersOverride(new object[0]));
            }

            throw new UnityInstanceNotResolvedException(typeof(T), mappingName);
        }

        /// <exception cref="UnityInstanceNotResolvedException">Thrown when no instance is resolved for the requested Type and Mapping.</exception>
        public T Get<T>(string mappingName, params object[] parameters)
        {
            if (IsRegistered<T>(mappingName))
            {
                return container.Resolve<T>(mappingName, new OrderedParametersOverride(parameters));
            }

            throw new UnityInstanceNotResolvedException(typeof(T), mappingName);
        }

        /// <exception cref="UnityInstanceNotResolvedException">Thrown when no instance is resolved for the requested Type and Mapping.</exception>
        public object Get(Type serviceType, string mappingName = null)
        {
            if (IsRegistered(serviceType, mappingName))
            {
                return container.Resolve(serviceType, mappingName, new OrderedParametersOverride(new object[0]));
            }

            throw new UnityInstanceNotResolvedException(serviceType, mappingName);
        }

        /// <exception cref="UnityInstanceNotResolvedException">Thrown when no instance is resolved for the requested Type and Mapping.</exception>
        public object Get(Type serviceType, string mappingName, params object[] parameters)
        {
            if (IsRegistered(serviceType, mappingName))
            {
                return container.Resolve(serviceType, mappingName, new OrderedParametersOverride(parameters));
            }

            throw new UnityInstanceNotResolvedException(serviceType, mappingName);
        }

        /// <exception cref="UnityInstanceNotResolvedException">Thrown when no instances are resolved for the requested Type.</exception>
        public IEnumerable<T> GetAll<T>()
        {
            var instances = container.ResolveAll<T>();

            if (instances.Count() != 0)
            {
                return instances;
            }

            throw new UnityInstanceNotResolvedException(typeof(T));
        }

        /// <exception cref="UnityInstanceNotResolvedException">Thrown when no instances are resolved for the requested Type.</exception>
        public IEnumerable<T> GetAll<T>(params object[] parameters)
        {
            var instances = container.ResolveAll<T>(new OrderedParametersOverride(parameters));

            if (instances.Count() != 0)
            {
                return instances;
            }

            throw new UnityInstanceNotResolvedException(typeof(T));
        }

        /// <exception cref="UnityInstanceNotResolvedException">Thrown when no instances are resolved for the requested Type.</exception>
        public IEnumerable<object> GetAll(Type type)
        {
            var instances = container.ResolveAll(type);

            if (instances.Count() != 0)
            {
                return instances;
            }

            throw new UnityInstanceNotResolvedException(type);
        }

        /// <exception cref="UnityInstanceNotResolvedException">Thrown when no instances are resolved for the requested Type.</exception>
        public IEnumerable<object> GetAll(Type type, params object[] parameters)
        {
            var instances = container.ResolveAll(type, new OrderedParametersOverride(parameters));

            if (instances.Count() != 0)
            {
                return instances;
            }

            throw new UnityInstanceNotResolvedException(type);
        }

        #endregion

        #region TryGet Methods

        /// <returns><value>true</value> when an instance is resolved.</returns>
        public bool TryGet<T>(string mappingName, out T output)
        {
            if (IsRegistered<T>(mappingName))
            {
                output = container.Resolve<T>(mappingName, new OrderedParametersOverride(new object[0]));

                return true;
            }

            output = default(T);

            return false;
        }

        /// <returns><value>true</value> when an instance is resolved.</returns>
        public bool TryGet<T>(string mappingName, out T output, params object[] parameters)
        {
            if (IsRegistered<T>(mappingName))
            {
                output = container.Resolve<T>(mappingName, new OrderedParametersOverride(parameters));

                return true;
            }

            output = default(T);

            return false;
        }

        /// <returns><value>true</value> when an instance is resolved.</returns>
        public bool TryGet(Type serviceType, string mappingName, out object output)
        {
            if (IsRegistered(serviceType, mappingName))
            {
                output = container.Resolve(serviceType, mappingName, new OrderedParametersOverride(new object[0]));

                return true;
            }

            if (serviceType.IsValueType)
            {
                output = Activator.CreateInstance(serviceType);
            }
            else
            {
                output = null;
            }

            return false;
        }

        /// <returns><value>true</value> when an instance is resolved.</returns>
        public bool TryGet(Type serviceType, string mappingName, out object output, params object[] parameters)
        {
            if (IsRegistered(serviceType, mappingName))
            {
                output = container.Resolve(serviceType, mappingName, new OrderedParametersOverride(parameters));

                return true;
            }

            if (serviceType.IsValueType)
            {
                output = Activator.CreateInstance(serviceType);
            }
            else
            {
                output = null;
            }

            return false;
        }

        /// <returns><value>true</value> when at least one instance is resolved.</returns>
        public bool TryGetAll<T>(out IEnumerable<T> output)
        {
            output = container.ResolveAll<T>();

            if (output.Count() != 0)
            {
                return true;
            }

            output = null;
            return false;
        }

        /// <returns><value>true</value> when at least one instance is resolved.</returns>
        public bool TryGetAll<T>(out IEnumerable<T> output, params object[] parameters)
        {
            output = container.ResolveAll<T>(new OrderedParametersOverride(parameters));

            if (output.Count() != 0)
            {
                return true;
            }

            output = null;
            return false;
        }

        /// <returns><value>true</value> when at least one instance is resolved.</returns>
        public bool TryGetAll(Type serviceType, out IEnumerable<object> output)
        {
            output = container.ResolveAll(serviceType);

            if (output.Count() != 0)
            {
                return true;
            }

            output = null;
            return false;
        }

        /// <returns><value>true</value> when at least one instance is resolved.</returns>
        public bool TryGetAll(Type serviceType, out IEnumerable<object> output, params object[] parameters)
        {
            output = container.ResolveAll(serviceType, new OrderedParametersOverride(parameters));

            if (output.Count() != 0)
            {
                return true;
            }

            output = null;
            return false;
        }

        #endregion
        #endregion
    }
}
