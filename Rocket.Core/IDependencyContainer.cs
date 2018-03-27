using System;
using System.Reflection;

namespace Rocket.Core
{
    public interface IDependencyContainer
    {
        void RegisterType<TInterface, TClass>() where TClass : TInterface;

        void RegisterSingletonType<TInterface, TClass>() where TClass : TInterface;

        void RegisterInstance<TInterface>(TInterface value);
    }
}