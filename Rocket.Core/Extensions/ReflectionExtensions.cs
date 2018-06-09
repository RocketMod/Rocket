using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
            List<Type> list = skipTypes?.ToList() ?? new List<Type>();
            list.Add(typeof(ReflectionExtensions));

            StackTrace st = new StackTrace();
            StackFrame target = null;
            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame frame = st.GetFrame(i);
                if (list.Any(c => c == frame.GetMethod()?.DeclaringType))
                    continue;

                target = frame;
                break;
            }

            return target?.GetMethod();
        }

        public static MethodBase GetCallingMethod(params Assembly[] skipAssemblies)
        {
            StackTrace st = new StackTrace();
            StackFrame target = null;
            for (int i = 0; i < st.FrameCount; i++)
            {
                StackFrame frame = st.GetFrame(i);
                if (skipAssemblies.Any(c => Equals(c, frame.GetMethod()?.DeclaringType?.Assembly)))
                    continue;

                target = frame;
            }

            return target?.GetMethod();
        }

        public static IEnumerable<Type> GetTypeHierarchy(this Type type)
        {
            List<Type> types = new List<Type> { type};
            while ((type = type.BaseType) != null)
            {
                types.Add(type);
            }

            return types;
        }

        internal static T GetPrivateProperty<T>(this object o, string property)
        {
            var prop = o.GetType()
                        .GetProperty(property, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            if(prop == null)
                throw new Exception("Property not found!");

            return (T) prop.GetGetMethod(true)
                    .Invoke(o, new object[0]);
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

        public static IEnumerable<Type> FindTypes<T>(this Assembly @object, bool includeAbstractAndInterfaces = false)
        {
            return FindAllTypes(@object).Where(c => typeof(T).IsAssignableFrom(c));
        }

        public static IEnumerable<Type> GetTypesWithInterface<TInterface>(this Assembly assembly)
        {
            return assembly.FindAllTypes().Where(t => typeof(TInterface).IsAssignableFrom(t));
        }

        public static Dictionary<string, string> GetAssembliesFromDirectory(
            string directory, string extension = "*.dll")
        {
            Dictionary<string, string> l = new Dictionary<string, string>();
            IEnumerable<FileInfo> assemblyFiles =
                new DirectoryInfo(directory).GetFiles(extension, SearchOption.AllDirectories);
            foreach (FileInfo assemblyFile in assemblyFiles)
                try
                {
                    AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyFile.FullName);
                    l.Add(GetVersionIndependentName(assemblyName.FullName), assemblyFile.FullName);
                }
                catch
                {

                }

            return l;
        }

        public static string GetVersionIndependentName(string name)
        {
            return GetVersionIndependentName(name, out _);
        }

        public static string GetVersionIndependentName(string name, out string extractedVersion)
        {
            Regex regex = new Regex("Version=(?<version>.+?), ", RegexOptions.Compiled);
            var match = regex.Match(name);
            extractedVersion = match.Groups[0].Value;
            return regex.Replace(name, "");
        }

        public static string GetDebugName(this MethodBase mb)
        {
            if (mb is MemberInfo mi && mi.DeclaringType != null) return mi.DeclaringType.Name + "." + mi.Name;

            return "<anonymous>#" + mb.Name;
        }
    }
}