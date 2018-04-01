using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Plugin;

namespace Rocket.API.Reflection
{
    public static class ReflectionHelper
    {
        public static List<Type> FindTypes<T>(this ILifecycleObject @object, bool includeAbstractAndInterfaces = true, Func<Type, bool> predicate = null)
        {
            var filter = FindAllTypes(@object).Where(t => typeof(T).IsAssignableFrom(t));
            if (includeAbstractAndInterfaces)
                filter = filter.Where(c => !c.IsAbstract && !c.IsInterface);

            if (predicate != null)
                filter = filter.Where(predicate);

            return filter.ToList();
        }

        public static List<Type> FindAllTypes(this ILifecycleObject @object, Func<Type, bool> predicate = null)
        {
            throw new NotImplementedException();
        }
    }
}