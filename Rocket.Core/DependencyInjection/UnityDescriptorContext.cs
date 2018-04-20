using System;
using System.ComponentModel;
using Rocket.API.DependencyInjection;

namespace Rocket.Core.DependencyInjection
{
    public class UnityDescriptorContext : ITypeDescriptorContext
    {
        public static UnityDescriptorContext From(IDependencyContainer container)
        {
            return new UnityDescriptorContext(container);
        }

        private UnityDescriptorContext(IDependencyContainer container)
        {
            UnityContainer= container;
        }

        public object GetService(Type serviceType)
        {
            return UnityContainer.Get(serviceType);
        }

        public bool OnComponentChanging()
        {
            throw new NotImplementedException();
        }

        public void OnComponentChanged()
        {
            throw new NotImplementedException();
        }

        public IContainer Container { get; set; }
        public object Instance { get; set; }
        public PropertyDescriptor PropertyDescriptor { get; }
        public IDependencyContainer UnityContainer { get; set; }
    }
}