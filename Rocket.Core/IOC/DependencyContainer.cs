using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rocket.Core.IOC
{
    public class DependencyContainer : IDependencyResolver, IDependencyContainer
    {
        private IUnityContainer container;
        public IServiceLocator ServiceLocator { get; private set; }
        public DependencyContainer()
        {
            container = new UnityContainer();
            container.RegisterInstance<IDependencyContainer>(this);
            container.RegisterInstance<IDependencyResolver>(this);
            Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            ServiceLocator = new ServiceLocator(Microsoft.Practices.ServiceLocation.ServiceLocator.Current);
        }
        
        private void GuardRegistered(Type type, bool throwException = true)
        {
            if (!container.IsRegistered(type) && throwException)
                throw new Exception(string.Format("Type '{0}' not registered in container.", type.AssemblyQualifiedName));
        }

        private void GuardRegistered(Type type, string mapping, bool throwException = true)
        {
            if (!container.IsRegistered(type, mapping) && throwException)
                throw new Exception(string.Format("Type '{0}' not registered in container.", type.AssemblyQualifiedName));
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

        public bool IsRegistered<T>()
        {
            return container.IsRegistered<T>();
        }

        public bool IsRegistered(Type type)
        {
            return container.IsRegistered(type);
        }

        public T TryGet<T>()
        {
            GuardRegistered(typeof(T), false);
            return container.Resolve<T>();
        }

        public T TryGet<T>(params object[] parameters)
        {
            GuardRegistered(typeof(T), false);
            return container.Resolve<T>(new OrderedParametersOverride(parameters));
        }

        public object TryGet(Type type)
        {
            GuardRegistered(type, false);
            return container.Resolve(type);
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

        public void RegisterInstance<TInterface>(TInterface instance)
        {
            container.RegisterInstance<TInterface>(instance);
        }

        public void RegisterSingletonType<TInterface, TClass>() where TClass : TInterface
        {
            container.RegisterType<TInterface, TClass>(new ContainerControlledLifetimeManager(), new InjectionMember[0]);
        }

        public void RegisterType<TInterface, TClass>() where TClass : TInterface
        {
            container.RegisterType<TInterface, TClass>();
        }

        public T Get<T>(string mapping)
        {
            GuardRegistered(typeof(T), mapping);
            return container.Resolve<T>(mapping, new OrderedParametersOverride(new object[0]));
        }

        public T Get<T>(string mapping, params object[] parameters)
        {
            GuardRegistered(typeof(T), mapping);
            return container.Resolve<T>(mapping, new OrderedParametersOverride(parameters));
        }

        public bool IsRegistered<T>(string mapping)
        {
            return container.IsRegistered<T>(mapping);
        }

        public bool IsRegistered(Type type, string mapping)
        {
            return container.IsRegistered(type, mapping);
        }

        public T TryGet<T>(string mapping)
        {
            GuardRegistered(typeof(T), mapping, false);
            return container.Resolve<T>(mapping, new OrderedParametersOverride(new object[0]));
        }

        public T TryGet<T>(string mapping, params object[] parameters)
        {
            GuardRegistered(typeof(T), mapping, false);
            return container.Resolve<T>(mapping, new OrderedParametersOverride(parameters));
        }

        public object Get(Type serviceType, string mapping)
        {
            GuardRegistered(serviceType, mapping);
            return container.Resolve(serviceType, mapping, new OrderedParametersOverride(new object[0]));
        }

        public object Get(Type serviceType, string mapping, params object[] parameters)
        {
            GuardRegistered(serviceType, mapping);
            return container.Resolve(serviceType, mapping, new OrderedParametersOverride(parameters));
        }

        public object TryGet(Type serviceType, string mapping)
        {
            GuardRegistered(serviceType, mapping, false);
            return container.Resolve(serviceType, mapping, new OrderedParametersOverride(new object[0]));
        }

        public object TryGet(Type serviceType, string mapping, params object[] parameters)
        {
            GuardRegistered(serviceType, mapping, false);
            return container.Resolve(serviceType, mapping, new OrderedParametersOverride(parameters));
        }

        public void RegisterType<TInterface, TClass>(string mapping) where TClass : TInterface
        {
            container.RegisterType<TInterface, TClass>(mapping);
        }

        public void RegisterInstance<TInterface>(TInterface value, string mapping)
        {
            container.RegisterInstance<TInterface>(mapping, value);
        }
    }
}
