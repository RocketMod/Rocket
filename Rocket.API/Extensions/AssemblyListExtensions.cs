using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Core.Extensions
{
    public static class AssemblyListExtensions
    {
        public static List<Type> GetTypes(this List<Assembly> assemblies)
        {
            List<Type> allTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types;
                }
                allTypes.AddRange(types);
            }
            return allTypes;
        }

        public static List<Type> GetTypesFromParentClass(this List<Assembly> assemblies, Type parentClass)
        {
            List<Type> allTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                allTypes.AddRange(assembly.GetTypesFromParentClass(parentClass));
            }
            return allTypes;
        }
    }
}
