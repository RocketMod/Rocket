using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rocket.IOC
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
        
        //TODO: What was the purpose of this method if it was set not to throw an exception?
        private void GuardRegistered(Type type, string mappingName = null)
        {
            if (!container.IsRegistered(type, mappingName))
            {
                throw new Exception(string.Format("Type '{0}' not registered in container.", type.AssemblyQualifiedName));
            }
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

        public IEnumerable<T> GetAll<T>()
        {
            return container.ResolveAll<T>();
        }

        public IEnumerable<T> GetAll<T>(params object[] parameters)
        {
            return container.ResolveAll<T>(new OrderedParametersOverride(parameters));
        }

        public IEnumerable<object> GetAll(Type type)
        {
            return container.ResolveAll(type);
        }

        public IEnumerable<object> GetAll(Type type, params object[] parameters)
        {
            return container.ResolveAll(type, new OrderedParametersOverride(parameters));
        }
 
        public void RegisterSingletonType<TInterface, TClass>(string mappingName = null) where TClass : TInterface
        {
            container.RegisterType<TInterface, TClass>(mappingName, new ContainerControlledLifetimeManager(), new InjectionMember[0]);
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

        public bool TryGet<T>(string mappingName, out T output)
        {
            try
            {
                GuardRegistered(typeof(T), mappingName);
                output = container.Resolve<T>(mappingName, new OrderedParametersOverride(new object[0]));

                return true;
            }
            catch
            {
                output = default(T);

                return false;
            }
        }

        public bool TryGet<T>(string mappingName, out T output, params object[] parameters)
        {
            try
            {
                GuardRegistered(typeof(T), mappingName);
                output = container.Resolve<T>(mappingName, new OrderedParametersOverride(parameters));

                return true;
            }
            catch
            {
                output = default(T);

                return false;
            }
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

        public bool TryGet(Type serviceType, string mappingName, out object output)
        {
            try
            {
                GuardRegistered(serviceType, mappingName);
                output = container.Resolve(serviceType, mappingName, new OrderedParametersOverride(new object[0]));

                return true;
            }
            catch
            {
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
        }

        public bool TryGet(Type serviceType, string mappingName, out object output, params object[] parameters)
        {
            try
            {
                GuardRegistered(serviceType, mappingName);
                output = container.Resolve(serviceType, mappingName, new OrderedParametersOverride(parameters));

                return true;
            }
            catch
            {
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
        }
    }
}
