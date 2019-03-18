using System;
using System.Collections.Generic;

namespace Rocket.API.Configuration
{
    /// <summary>
    ///     Represents a configuration node or element.
    /// </summary>
    public interface IConfigurationElement : IEnumerable<IConfigurationSection>
    {
        /// <summary>
        ///     The section type.
        /// </summary>
        SectionType Type { get; }

        /// <summary>
        ///     Gets a child configuration node. See <see cref="GetSection" />.
        /// </summary>
        /// <param name="path">The configuration path.</param>
        /// <returns>The configuration section.</returns>
        IConfigurationSection this[string path] { get; }

        /// <summary>
        ///     The parent configuration node.
        /// </summary>
        IConfigurationElement ParentElement { get; }

        /// <summary>
        ///     The root configuration node. Can be null.
        /// </summary>
        IConfiguration Root { get; }

        /// <summary>
        ///     The separated absolute path (e.g. MyConfig.ChildSection.MyValue).
        /// </summary>
        string Path { get; }


        /// <summary>
        ///     Gets a configuration sub-section with the specified path.
        /// </summary>
        /// <param name="path">The relative path to the section to get. See <see cref="Path" />.</param>
        /// <returns>The <see cref="IConfigurationSection" />.</returns>
        /// <remarks>
        ///     This method will never return <c>null</c>. If no matching sub-section is found with the specified path,
        ///     an empty <see cref="T:IConfigurationSection" /> will be returned.
        /// </remarks>
        IConfigurationSection GetSection(string path);

        /// <summary>
        ///     Creates a new configuration section.
        /// </summary>
        /// <param name="path">The relative path to the section to create. See <see cref="Path" />.</param>
        /// <param name="type">The section type. See <see cref="SectionType" />.</param>
        /// <returns>the created section.</returns>
        IConfigurationSection CreateSection(string path, SectionType type);

        /// <summary>
        ///     Deletes a configuration section.
        /// </summary>
        /// <param name="path">The relative path to the section to delete. See <see cref="Path" />.</param>
        /// <returns><b>true</b> if the section exists and could be deleted; otherwise, <b>false</b>.</returns>
        bool DeleteSection(string path);

        /// <summary>
        ///     Gets the child configuration nodes.
        /// </summary>
        /// <returns>the child configuration nodes.</returns>
        IEnumerable<IConfigurationSection> GetChildren();

        /// <summary>
        ///     Sets the sections value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        void Set(object value);

        /// <summary>
        ///     Gets the sections parsed value.
        /// </summary>
        /// <typeparam name="T">The type to parse the value as.</typeparam>
        /// <returns>the parsed value.</returns>
        T Get<T>();

        /// <summary>
        ///     GEts the sections value.
        /// </summary>
        /// <returns>the sections value.</returns>
        dynamic Get();

        /// <summary>
        ///     Gets the sections parsed value.
        /// </summary>
        /// <param name="type">The type to parse the value as.</param>
        /// <returns>the parsed value if a value exists and it could be parsed; otherwise, the default value.</returns>
        object Get(Type type);

        /// <summary>
        ///     Gets the sections parsed value.
        /// </summary>
        /// <typeparam name="T">The type to parse the value as.</typeparam>
        /// <param name="defaultValue">The default value to use when the value could not be parsed or does not exist.</param>
        /// <returns>the parsed value if a value exists and it could be parsed; otherwise, the default value.</returns>
        T Get<T>(T defaultValue);

        /// <summary>
        ///     Gets the sections parsed value.
        /// </summary>
        /// <param name="type">The type to parse the value as.</param>
        /// <param name="defaultValue">The default value to use when the value could not be parsed or does not exist.</param>
        /// <returns>the parsed value if a value exists and it could be parsed; otherwise, the default value.</returns>
        object Get(Type type, object defaultValue);

        /// <summary>
        ///     Tries to get a sections parsed value.
        /// </summary>
        /// <typeparam name="T">The type to parse the value as.</typeparam>
        /// <param name="value">The parsed value if a value exists and it could be parsed.</param>
        /// <returns><b>true</b> if the value exists and could be parsed; otherwise, <b>false</b>.</returns>
        bool TryGet<T>(out T value);

        /// <summary>
        ///     Tries to get a sections parsed value.
        /// </summary>
        /// <param name="type">The type to parse the value as.</param>
        /// <param name="value">The parsed value if a value exists and it could be parsed.</param>
        /// <returns><b>true</b> if the value exists and could be parsed; otherwise, <b>false</b>.</returns>
        bool TryGet(Type type, out object value);

        /// <summary>
        ///     Checks if a child exists.
        /// </summary>
        /// <param name="path">The relative path to the child section. See <see cref="Path" />.</param>
        /// <returns></returns>
        bool ChildExists(string path);

        /// <summary>
        ///     Returns a cloned instance excluding the context.
        /// </summary>
        /// <returns>the clone.</returns>
        IConfigurationElement Clone();
    }
}