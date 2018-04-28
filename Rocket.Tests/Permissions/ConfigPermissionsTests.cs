using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Permissions;

namespace Rocket.Tests.Permissions
{
    [TestClass]
    public class ConfigPermissionsTests : PermissionsTestBase
    {
        protected override IPermissionProvider GetPermissionProvider()
            => Runtime.Container.Get<IPermissionProvider>("default_permissions");
    }
}