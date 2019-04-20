using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;

namespace Rocket.Core.DependencyInjection
{
    public class RegistrationByConvention
    {
        private readonly IDependencyContainer container;
        private readonly IDependencyResolver resolver;
        private readonly ILogger logger;
        private readonly List<string> activatedAssemblies = new List<string>();

        public RegistrationByConvention(IDependencyContainer container, IDependencyResolver resolver, ILogger logger)
        {
            this.container = container;
            this.resolver = resolver;
            this.logger = logger;
            AppDomain.CurrentDomain.AssemblyLoad += (sender, args) => RegisterAssembly(args.LoadedAssembly);

            var coreAssembly = typeof(RegistrationByConvention).Assembly;
            RegisterAssembly(coreAssembly);

            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            logger.LogTrace("Assemblies: ["
                + string.Join(", ", assemblies.Select(c => c.GetName().Name).ToArray())
                + "]");

            foreach (Assembly assembly in assemblies)
            {
                RegisterAssembly(assembly);
            }
        }

        private void RegisterAssembly(Assembly assembly)
        {
            if (activatedAssemblies.Contains(assembly.FullName))
            {
                return;
            }

            logger?.LogTrace("Registering assembly: " + assembly.FullName);
            foreach (Type type in assembly.GetTypesWithInterface<IServiceConfigurator>())
            {
                logger?.LogTrace("\tRegistering from IServiceConfigurator: " + type.FullName);
                ((IServiceConfigurator)Activator.CreateInstance(type)).ConfigureServices(container);
            }

            activatedAssemblies.Add(assembly.FullName);
        }
    }
}