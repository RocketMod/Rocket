using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Permissions;
using Rocket.API.ServiceProxies;

namespace Rocket.Tests.Permissions
{
    [TestClass]
    public class ProxyPermissionsTests : PermissionsTestsBase
    {
        protected override IPermissionProvider GetPermissionProvider()
        {
            return Runtime.Container.Get<IPermissionProvider>("proxy_permissions");
        }

        protected override IPermissionProvider LoadProvider()
        {
            var prov = (IServiceProxy<IPermissionProvider>)GetPermissionProvider();
            foreach (var proxy in prov.ProxiedServices)
                proxy.Load(GroupsConfig, PlayersConfig);

            return (IPermissionProvider)prov;
        }

        [TestMethod]
        public void TestPermissionsLoad()
        {
            //proxy does not support loading from configs
            Assert.ThrowsException<NotSupportedException>(() => base.TestPermissionsLoad());
        }


        [TestMethod]
        public override void TestGroupPermissions()
        {
            //Groups are not supported on proxy
            Assert.ThrowsException<NotSupportedException>(() => base.TestGroupPermissions());
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
        public override void TestHasAnyPermissionsGroup()
        {
            Assert.ThrowsException<NotSupportedException>(() => base.TestHasAllPermissionsGroup());
        }

        [TestMethod]
        public override void TestHasAllPermissionsGroup()
        {
            Assert.ThrowsException<NotSupportedException>(() => base.TestHasAllPermissionsGroup());
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
}