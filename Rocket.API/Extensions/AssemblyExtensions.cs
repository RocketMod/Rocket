using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.API.Extensions
{

    public static class AssemblyExtensions
    {
        public static List<Type> GetTypesFromParentClass(this Assembly assembly, Type parentClass)
        {
            List<Type> allTypes = new List<Type>();
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }
            foreach (Type type in types.Where(t => t != null))
            {
                if (type.IsSubclassOf(parentClass))
                {
                    allTypes.Add(type);
                }
            }
            return allTypes;
        }

        public static List<Type> GetTypesFromInterface(this Assembly assembly, string interfaceName)
        {
            List<Type> allTypes = new List<Type>();
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }
            foreach (Type type in types.Where(t => t != null))
            {
                if (type.GetInterface(interfaceName) != null)
                {
                    allTypes.Add(type);
                }
            }
            return allTypes;
        }
    }
}
