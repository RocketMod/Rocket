using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Rocket.API;

namespace Rocket.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<Type> FindTypes<T>(this ILifecycleObject @object, bool includeAbstractAndInterfaces = true,
            Func<Type, bool> predicate = null)
        {
            IEnumerable<Type> filter = @object.FindAllTypes(includeAbstractAndInterfaces).Where(t => typeof(T).IsAssignableFrom(t));

            if (predicate != null)
                filter = filter.Where(predicate);

            return filter;
        }

        public static IEnumerable<Type> FindAllTypes(this ILifecycleObject @object, bool includeAbstractAndInterfaces = false)
        {
            return @object.GetType().Assembly.FindAllTypes(includeAbstractAndInterfaces);
        }

        public static IEnumerable<Type> FindAllTypes(this Assembly @object, bool includeAbstractAndInterfaces = false)
        {
            try
            {
                return @object.GetTypes()
                    .Where(c => includeAbstractAndInterfaces || (!c.IsAbstract && !c.IsInterface));
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        public static IEnumerable<Type> GetTypesWithInterface<TInterface>(this Assembly assembly)
        {
            return assembly.FindAllTypes().Where(t => typeof(TInterface).IsAssignableFrom(t));
        }
    }
}