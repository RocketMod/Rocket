using System;
using System.Collections.Generic;

namespace Rocket.API.Configuration
{
    public interface IConfigurationBase : IEnumerable<IConfigurationSection>
    {
        /// <summary>Gets a configuration section.</summary>
        /// <param name="path">The configuration path.</param>
        /// <returns>The configuration section.</returns>
        IConfigurationSection this[string path] { get; }

        IConfigurationBase Parent { get; }

        IConfiguration Root { get; }


        /// <summary>
        /// Gets a configuration sub-section with the specified path.
        /// </summary>
        /// <param name="path">The path of the configuration section.</param>
        /// <returns>The <see cref="IConfigurationSection" />.</returns>
        /// <remarks>
        ///     This method will never return <c>null</c>. If no matching sub-section is found with the specified path,
        ///     an empty <see cref="T:IConfigurationSection" /> will be returned.
        /// </remarks>
        IConfigurationSection GetSection(string path);

        IConfigurationSection CreateSection(string path, SectionType type);

        bool RemoveSection(string path);

        /// <summary>
        /// Gets the immediate descendant configuration sub-sections.
        /// </summary>
        /// <returns>The configuration sub-sections.</returns>
        IEnumerable<IConfigurationSection> GetChildren();


        /// <summary>
        /// Gets the path of the config section
        /// </summary>
        string Path { get; }

        void Set(object o);

        T Get<T>();
        object Get(Type t);

        T Get<T>(T defaultValue);
        object Get(Type t, object defaultValue);

        bool TryGet<T>(out T value);
        bool TryGet(Type t, out object value);

        bool ChildExists(string path);
    }
}