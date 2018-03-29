using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Reflection;
using Rocket.API.DependencyInjection;

namespace Rocket.Core.DependencyInjection
{
    public class UnityDependencyContainer : IDependencyContainer
    {
        internal readonly IUnityContainer container;

        public IServiceLocator ServiceLocator { get; private set; }

        public UnityDependencyContainer()
        {
            container = new UnityContainer();
            ServiceLocator = new UnityServiceLocator(container);
            container.RegisterInstance<IDependencyContainer>(this);
            container.RegisterInstance<IDependencyResolver>(this);
        }

        public UnityDependencyContainer(UnityDependencyContainer parent)
        {
            container = parent.container.CreateChildContainer();
            ServiceLocator = new UnityServiceLocator(container);
            container.RegisterInstance<IDependencyContainer>(this);
            container.RegisterInstance<IDependencyResolver>(this);
        }

        private void GuardRegistered(Type type, bool throwException = true)
        {
            if (!container.IsRegistered(type) && throwException)
                throw new Exception($"Type '{type.AssemblyQualifiedName}' not registered in container.");
        }

        private void GuardRegistered(Type type, string mappingName, bool throwException = true)
        {
            if (!container.IsRegistered(type, mappingName) && throwException)
                throw new Exception($"Type '{type.AssemblyQualifiedName}' not registered in container.");
        }

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

        public T Get<T>()
        {
            GuardRegistered(typeof(T));
            return container.Resolve<T>();
        }

        public T Get<T>(params object[] parameters)
        {
            GuardRegistered(typeof(T));
            return container.Resolve<T>(new OrderedParametersOverride(parameters));
        }

        public object Get(Type type)
        {
            GuardRegistered(type);
            return container.Resolve(type);
        }

        public object Get(Type type, params object[] parameters)
        {
            GuardRegistered(type);
            return container.Resolve(type, new OrderedParametersOverride(parameters));
        }

        public IEnumerable<T> GetAll<T>()
        {
            GuardRegistered(typeof(T));
            return container.ResolveAll<T>();
        }

        public IEnumerable<T> GetAll<T>(params object[] parameters)
        {
            GuardRegistered(typeof(T));
            return container.ResolveAll<T>(new OrderedParametersOverride(parameters));
        }

        public IEnumerable<object> GetAll(Type type)
        {
            GuardRegistered(type);
            return container.ResolveAll(type);
        }

        public IEnumerable<object> GetAll(Type type, params object[] parameters)
        {
            GuardRegistered(type);
            return container.ResolveAll(type,new OrderedParametersOverride(parameters));
        }

        public T TryGet<T>(params object[] parameters)
        {
            GuardRegistered(typeof(T), false);
            return container.Resolve<T>(new OrderedParametersOverride(parameters));
        }

        public object TryGet(Type type, params object[] parameters)
        {
            GuardRegistered(type, false);
            return container.Resolve(type,new OrderedParametersOverride(parameters));
        }

        public IEnumerable<T> TryGetAll<T>()
        {
            GuardRegistered(typeof(T), false);
            return container.ResolveAll<T>();
        }

        public IEnumerable<T> TryGetAll<T>(params object[] parameters)
        {
            GuardRegistered(typeof(T), false);
            return container.ResolveAll<T>(new OrderedParametersOverride(parameters));
        }

        public IEnumerable<object> TryGetAll(Type type)
        {
            GuardRegistered(type, false);
            return container.ResolveAll(type);
        }

        public IEnumerable<object> TryGetAll(Type type, params object[] parameters)
        {
            GuardRegistered(type, false);
            return container.ResolveAll(type, new OrderedParametersOverride(parameters));
        }
        
        public void RegisterSingletonType<TInterface, TClass>(string mappingName = null) where TClass : TInterface
        {
            container.RegisterType<TInterface, TClass>(mappingName = null,new ContainerControlledLifetimeManager(), new InjectionMember[0]);
        }
        
        public void RegisterType<TInterface, TClass>(string mappingName = null) where TClass : TInterface
        {
            container.RegisterType<TInterface, TClass>(mappingName);
        }

        public void RegisterInstance<TInterface>(TInterface value, string mappingName = null)
        {
            container.RegisterInstance<TInterface>(mappingName, value);
        }

        public T Get<T>(string mappingName = null)
        {
            GuardRegistered(typeof(T), mappingName);
            return container.Resolve<T>(mappingName, new OrderedParametersOverride(new object[0]));
        }

        public T Get<T>(string mappingName, params object[] parameters)
        {
            GuardRegistered(typeof(T), mappingName);
            return container.Resolve<T>(mappingName, new OrderedParametersOverride(parameters));
        }

        public bool IsRegistered<T>(string mappingName = null)
        {
            return container.IsRegistered<T>(mappingName);
        }

        public bool IsRegistered(Type type, string mappingName = null)
        {
            return container.IsRegistered(type, mappingName);
        }

        public T TryGet<T>(string mappingName = null)
        {
            GuardRegistered(typeof(T), mappingName, false);
            return container.Resolve<T>(mappingName, new OrderedParametersOverride(new object[0]));
        }

        public T TryGet<T>(string mappingName, params object[] parameters)
        {
            GuardRegistered(typeof(T), mappingName, false);
            return container.Resolve<T>(mappingName, new OrderedParametersOverride(parameters));
        }

        public object Get(Type serviceType, string mappingName = null)
        {
            GuardRegistered(serviceType, mappingName);
            return container.Resolve(serviceType, mappingName, new OrderedParametersOverride(new object[0]));
        }

        public object Get(Type serviceType, string mappingName, params object[] parameters)
        {
            GuardRegistered(serviceType, mappingName);
            return container.Resolve(serviceType, mappingName, new OrderedParametersOverride(parameters));
        }

        public object TryGet(Type serviceType, string mappingName = null)
        {
            GuardRegistered(serviceType, mappingName, false);
            return container.Resolve(serviceType, mappingName, new OrderedParametersOverride(new object[0]));
        }

        public object TryGet(Type serviceType, string mappingName, params object[] parameters)
        {
            GuardRegistered(serviceType, mappingName, false);
            return container.Resolve(serviceType, mappingName, new OrderedParametersOverride(parameters));
        }


    }
}
