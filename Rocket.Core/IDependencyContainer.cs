using System;
using System.Reflection;

namespace Rocket.Core
{
    public interface IDependencyContainer
    {
        void RegisterType<TInterface, TClass>() where TClass : TInterface;
        void RegisterType<TInterface, TClass>(string mapping) where TClass : TInterface;

        void RegisterSingletonType<TInterface, TClass>() where TClass : TInterface;

        void RegisterInstance<TInterface>(TInterface value);
        void RegisterInstance<TInterface>(TInterface value, string mapping);
    }
}