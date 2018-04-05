using System;
using System.Reflection;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.Extensions;

namespace Rocket.Core.DependencyInjection
{
    public class RegistrationByConvention
    {
        public RegistrationByConvention(IDependencyContainer container, IDependencyResolver resolver, ILogger logger)
        {
            AppDomain.CurrentDomain.AssemblyLoad += (sender, args) =>
            {
                foreach (Type type in args.LoadedAssembly.GetTypesWithInterface<IDependencyRegistrator>())
                    ((IDependencyRegistrator)Activator.CreateInstance(type)).Register(container, resolver);
            };

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
                foreach (Type type in assembly.GetTypesWithInterface<IDependencyRegistrator>())
                    ((IDependencyRegistrator)Activator.CreateInstance(type)).Register(container, resolver);
        }
    }
}