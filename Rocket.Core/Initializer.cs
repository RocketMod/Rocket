using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Core
{
    public class Initializer
    {
        public Initializer(IDependencyContainer container, IDependencyResolver resolver, ILog logger)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type type;
                try
                {
                    type = ((IEnumerable<Type>)assembly.GetTypes()).FirstOrDefault(t => ((IEnumerable<Type>)t.GetInterfaces()).Any(i => i == typeof(IDependencyRegistrator)));
                    if (type == null)
                    {
                        continue;
                    }
                    ((IDependencyRegistrator)Activator.CreateInstance(type)).Register(container, resolver);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // failed to load ex.Types
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
