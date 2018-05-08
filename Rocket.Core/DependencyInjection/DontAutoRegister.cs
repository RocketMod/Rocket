using System;

namespace Rocket.Core.DependencyInjection
{
    /// <summary>
    ///     Prevents a class from being auto registered for dependency injection.
    /// </summary>
    public class DontAutoRegisterAttribute : Attribute { }
}