using System;
using System.Reflection;

namespace Rocket.IOC
{
    public interface IDependencyContainer
    {
        void RegisterType<TInterface, TClass>(string mappingName = null) where TClass : TInterface;

        void RegisterSingletonType<TInterface, TClass>(string mappingName = null) where TClass : TInterface;

        void RegisterInstance<TInterface>(TInterface value, string mappingName = null);

        void RegisterSingletonInstance<TInterface>(TInterface value, string mappingName = null);
    }
}