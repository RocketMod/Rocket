namespace Rocket.API.Configuration
{
    /// <summary>
    ///     Defines a sections type.
    /// </summary>
    public enum SectionType
    {
        /// <summary>
        ///     A value section (e.g. properties, fields, etc.). Can not have child sections.
        /// </summary>
        Value,

        /// <summary>
        ///     An array section (e.g. int arrays, string arrays, etc.). Can not have child sections.
        /// </summary>
        Array,

        /// <summary>
        ///     An object section which can contain child sections.
        /// </summary>
        Object
    }
}