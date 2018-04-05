using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;

namespace Rocket.Core.DependencyInjection {
    public class RegistrationByConvention {
        public RegistrationByConvention(IDependencyContainer container, IDependencyResolver resolver, ILogger logger) {
            AppDomain.CurrentDomain.AssemblyLoad += (sender, args) => {
                foreach (Type type in getTypesWithInterface<IDependencyRegistrator>(args.LoadedAssembly))
                    ((IDependencyRegistrator) Activator.CreateInstance(type)).Register(container, resolver);
            };

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            foreach (Type type in getTypesWithInterface<IDependencyRegistrator>(assembly))
                ((IDependencyRegistrator) Activator.CreateInstance(type)).Register(container, resolver);
        }

        private IEnumerable<Type> getTypesWithInterface<TInterface>(Assembly assembly) {
            try {
                return assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i == typeof(TInterface)));
            }
            catch (ReflectionTypeLoadException e) {
                return e.Types.Where(t => t != null && t.GetInterfaces().Contains(typeof(TInterface)));
            }
            catch (Exception) {
                throw;
            }
        }
    }
}