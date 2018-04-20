using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Configuration;
using Rocket.API.Permissions;

namespace Rocket.Tests.Permissions
{
    [TestClass]
    public class ConfigPermissionsTests : PermissionsTestBase
    {
        protected override IPermissionProvider GetPermissionProvider()
        {
            return Runtime.Container.Get<IPermissionProvider>("default_permissions");
        }
    }
}