namespace Rocket.Tests.Permissions
{
    /*
    [TestClass]
    public class ProxyPermissionsTests : PermissionsTestBase
    {
        protected override IPermissionProvider GetPermissionProvider()
            => Runtime.Container.Resolve<IPermissionProvider>("proxy_permissions");
        
        protected override IPermissionProvider LoadProvider()
        {
            IServiceProxy<IPermissionProvider> prov = (IServiceProxy<IPermissionProvider>) GetPermissionProvider();
            foreach (IPermissionProvider proxy in prov.ProxiedServices)
                proxy.Activate(GroupsConfig, PlayersConfig);

            return (IPermissionProvider) prov;
        }

        [TestMethod]
        public override void TestPermissionsLoad()
        {
            //proxy does not support loading from configs
            Assert.ThrowsException<NotSupportedException>(() => base.TestPermissionsLoad());
        }

        [TestMethod]
        public override void TestUpdateGroup()
        {
            //Groups are not supported on proxy
            Assert.ThrowsException<NotSupportedException>(() => base.TestUpdateGroup());
        }

        [TestMethod]
        public override void TestAddPermissionToGroup()
        {
            //Groups are not supported on proxy
            Assert.ThrowsException<NotSupportedException>(() => base.TestAddPermissionToGroup());
        }

        [TestMethod]
        public override void TestAddPermissionToPlayer()
        {
            //Adding permissions is not supported on proxy
            Assert.ThrowsException<NotSupportedException>(() => base.TestAddPermissionToPlayer());
        }

        [TestMethod]
        public override void TestCreateGroup()
        {
            //Groups are not supported on proxy
            Assert.ThrowsException<NotSupportedException>(() => base.TestCreateGroup());
        }

        [TestMethod]
        public override void TestDeleteGroup()
        {
            //Groups are not supported on proxy
            Assert.ThrowsException<NotSupportedException>(() => base.TestCreateGroup());
        }

        [TestMethod]
        public override void TestGetGroups()
        {
            //Groups are not supported on proxy
            Assert.ThrowsException<NotSupportedException>(() => base.TestCreateGroup());
        }

        [TestMethod]
        public override void TestRemovePermissionFromGroup()
        {
            //Groups are not supported on proxy
            Assert.ThrowsException<NotSupportedException>(() => base.TestRemovePermissionFromGroup());
        }

        [TestMethod]
        public override void TestRemovePermissionFromPlayer()
        {
            //Removing permissions is not supported on proxy
            Assert.ThrowsException<NotSupportedException>(() => base.TestRemovePermissionFromPlayer());
        }
    }
    */
}