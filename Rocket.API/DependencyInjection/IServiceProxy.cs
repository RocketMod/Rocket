using System.Collections.Generic;

namespace Rocket.API.DependencyInjection
{
    /// <summary>
    ///     <inheritdoc /><br /><br />
    ///     <b>Must deriver from <i>T</i></b>.
    /// </summary>
    /// <remarks>
    ///     <inheritdoc />
    /// </remarks>
    /// <typeparam name="T">The service to proxy (e.g. IUserManager).</typeparam>
    public interface IServiceProxy<T> : IServiceProxy where T : IProxyableService
    {
        /// <summary>
        ///     q
        ///     All service instances which were proxied.
        /// </summary>
        IEnumerable<T> ProxiedServices { get; }
    }

    /// <summary>
    ///     Defines a proxy service implementation.
    /// </summary>
    /// <remarks>
    ///     A service proxy allows to have multiple implementations for the same service. For example,
    ///     we can have multiple implementations for <i>IPermissionProvider</i>. The proxy allows to do this:
    ///     <code>
    ///         bool hasPermission = container.Resolve&lt;IPermissionProvider&gt;().CheckPermission(player, "mypermission") == PermissionResult.Grant;
    ///     </code>
    ///     It looks like the code is only calling one permission provider, but in reality it is calling the the proxy which is
    ///     calling all of the registered permission providers.<br />
    ///     This allows us to avoid having issues with load order of providers and prevents to do always foreach with
    ///     container.ResolveAll&lt;&gt;.
    /// </remarks>
    public interface IServiceProxy : IService { }
}