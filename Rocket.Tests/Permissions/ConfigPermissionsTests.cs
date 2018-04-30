using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Permissions;
using Rocket.Core.Permissions;

namespace Rocket.Tests.Permissions
{
    [TestClass]
    public class ConfigPermissionsTests : PermissionsTestBase
    {
        protected override IPermissionProvider LoadProvider()
        {
            ConfigurationPermissionProvider provider = (ConfigurationPermissionProvider) GetPermissionProvider();
            provider.LoadFromConfig(GroupsConfig, PlayersConfig);
            return provider;
        }

        protected override IPermissionProvider GetPermissionProvider()
            => Runtime.Container.Resolve<IPermissionProvider>("default_permissions");
    }
}