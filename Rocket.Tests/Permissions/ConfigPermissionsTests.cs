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
            var provider = (ConfigurationPermissionProvider) Runtime.Container.Resolve<IPermissionProvider>("default_permissions");
            provider.LoadFromConfig(GroupsConfig, PlayersConfig);
            return provider;
        }

        protected override IPermissionChecker LoadChecker()
        {
            return (ConfigurationPermissionProvider) LoadProvider();
        }
    }
}