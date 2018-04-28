using System;
using System.Collections.Generic;
using System.Linq;
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
                    ((IDependencyRegistrator) Activator.CreateInstance(type)).Register(container, resolver);
            };

            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            logger.LogDebug("Assemblies: ["
                + string.Join(", ", assemblies.Select(c => c.GetName().Name).ToArray())
                + "]");

            foreach (Assembly assembly in assemblies)
            {
                logger?.LogDebug("Registering assembly: " + assembly.FullName);
                foreach (Type type in assembly.GetTypesWithInterface<IDependencyRegistrator>())
                {
                    logger?.LogDebug("\tRegistering from IDependencyRegistrator: " + type.FullName);
                    ((IDependencyRegistrator) Activator.CreateInstance(type)).Register(container, resolver);
                }
            }
        }
    }
}