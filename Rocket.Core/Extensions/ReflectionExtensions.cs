using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Rocket.API;

namespace Rocket.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<Type> FindTypes<T>(this ILifecycleObject @object,
                                                     bool includeAbstractAndInterfaces = true,
                                                     Func<Type, bool> predicate = null)
        {
            IEnumerable<Type> filter = @object.FindAllTypes(includeAbstractAndInterfaces)
                                              .Where(t => typeof(T).IsAssignableFrom(t));

            if (predicate != null) filter = filter.Where(predicate);

            return filter;
        }

        public static MethodBase GetCallingMethod(params Type[] skipTypes)
        {
            StackTrace st = new StackTrace();
            StackFrame target = null;
            for (int i = 0; i < st.FrameCount; i++)
            {
                var frame = st.GetFrame(i);
                if (skipTypes.Any(c => c == frame.GetMethod().DeclaringType))
                    continue;

                target = frame;
            }

            return target?.GetMethod();
        }

        public static IEnumerable<Type> FindAllTypes(this ILifecycleObject @object,
                                                     bool includeAbstractAndInterfaces = false)
            => @object.GetType().Assembly.FindAllTypes(includeAbstractAndInterfaces);

        public static IEnumerable<Type> FindAllTypes(this Assembly @object, bool includeAbstractAndInterfaces = false)
        {
            try
            {
                return @object.GetTypes()
                              .Where(c => includeAbstractAndInterfaces || !c.IsAbstract && !c.IsInterface);
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

        public static Dictionary<string, string> GetAssembliesFromDirectory(
            string directory, string extension = "*.dll")
        {
            Dictionary<string, string> l = new Dictionary<string, string>();
            IEnumerable<FileInfo> libraries =
                new DirectoryInfo(directory).GetFiles(extension, SearchOption.AllDirectories);
            foreach (FileInfo library in libraries)
                try
                {
                    AssemblyName name = AssemblyName.GetAssemblyName(library.FullName);
                    l.Add(name.FullName, library.FullName);
                }
                catch { }

            return l;
        }
    }
}