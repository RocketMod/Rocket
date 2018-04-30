using System;
using System.ComponentModel;
using Rocket.API.DependencyInjection;

namespace Rocket.Core.DependencyInjection
{
    public class UnityDescriptorContext : ITypeDescriptorContext
    {
        private UnityDescriptorContext(IDependencyContainer container)
        {
            UnityContainer = container;
        }

        public IDependencyContainer UnityContainer { get; set; }

        public object GetService(Type serviceType) => UnityContainer.Resolve(serviceType);

        public bool OnComponentChanging() => throw new NotImplementedException();

        public void OnComponentChanged()
        {
            throw new NotImplementedException();
        }

        public IContainer Container { get; set; }
        public object Instance { get; set; }
        public PropertyDescriptor PropertyDescriptor { get; }

        public static UnityDescriptorContext From(IDependencyContainer container)
            => new UnityDescriptorContext(container);
    }
}