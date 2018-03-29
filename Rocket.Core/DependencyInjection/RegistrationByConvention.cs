using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API.DependencyInjection;
using Rocket.API.Logging;

namespace Rocket.Core.DependencyInjection
{
    public class RegistrationByConvention
    {
        public RegistrationByConvention(IDependencyContainer container, IDependencyResolver resolver, ILogger logger)
        {
            AppDomain.CurrentDomain.AssemblyLoad += (object sender, AssemblyLoadEventArgs args) =>
            {
                foreach (Type type in getTypesWithInterface<IDependencyRegistrator>(args.LoadedAssembly))
                {
                    ((IDependencyRegistrator)Activator.CreateInstance(type)).Register(container, resolver);
                }
            };

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                foreach(Type type in getTypesWithInterface<IDependencyRegistrator>(assembly)){
                    ((IDependencyRegistrator)Activator.CreateInstance(type)).Register(container, resolver);
                }
            }
        }

        private IEnumerable<Type> getTypesWithInterface<TInterface>(Assembly assembly)
        {
            try
            {
                return ((IEnumerable<Type>)assembly.GetTypes()).Where(t => ((IEnumerable<Type>)t.GetInterfaces()).Any(i => i == typeof(TInterface)));
            }
            catch (ReflectionTypeLoadException ex)
            {
                // failed to load ex.Types
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
