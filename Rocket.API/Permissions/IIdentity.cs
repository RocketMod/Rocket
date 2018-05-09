using System;

namespace Rocket.API.Permissions
{
    /// <summary>
    ///     Defines an identifiable object.
    /// </summary>
    public interface IIdentity
    {
        string Id { get; }
        string Name {
    }
}