using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Configuration;
using Rocket.API.Permissions;
using Rocket.Core.Permissions;

namespace Rocket.Tests.Permissions
{
    [TestClass]
    [TestCategory("Permissions")]
    public class PermissionsTests : RocketTestBase
    {
        protected IConfiguration PlayersConfig { get; private set; }
        protected IConfiguration GroupsConfig { get; private set; }
        protected TestPlayer TestPlayer { get; private set; }

        [TestInitialize]
        public void BootstrapPermissionTest()
        {
            object sampleGroupsPermissions = new
            {
                TestGroup1 = new
                {
                    Name = "TestGroup",
                    Priority = 2,
                    Permissions = new[]
                    {
                        "GroupPermission1"
                    }
                },
                TestGroup2 = new
                {
                    Name = "TestGroup2",
                    Priority = 1,
                    Permissions = new[]
                    {
                        "GroupPermission2",
                        "GroupPermission2.Child"
                    }
                },
                TestGroup3 = new
                {
                    Name = "PrimaryGroup",
                    Priority = 3
                }
            };

            object samplePlayers = new
            {
                TestPlayer = new
                {
                    TestPlayerId = new
                    {
                        LastDisplayName = "Trojaner",
                        Groups = new[]
                        {
                            "TestGroup3", "TestGroup2",
                            "TestGroup4" /* doesn't exist, shouldn't be exposed by GetGroups */
                        },
                        Permissions = new[]
                        {
                            "PlayerPermission.Test",
                            "PlayerPermission.Test2.*"
                        }
                    }
                }
            };

            PlayersConfig = GetConfigurationProvider();
            PlayersConfig.LoadFromObject(samplePlayers);

            GroupsConfig = GetConfigurationProvider();
            GroupsConfig.LoadFromObject(sampleGroupsPermissions);

            TestPlayer = new TestPlayer
            {
                Id = "TestPlayerId"
            };
        }

        public virtual IConfiguration GetConfigurationProvider() => Runtime.Container.Get<IConfiguration>();

        [TestMethod]
        public void TestUpdateGroup()
        {
            var provider = LoadProvider();
            var group = new PermissionGroup();
            group.Id = "TestGroup1";
            group.Name = "UpdatedGroupName";
            group.Priority = -1;

            provider.UpdateGroup(group);
            Assert.AreEqual(group.Name, "UpdatedGroupName");
            Assert.AreEqual(group.Priority, -1);
        }

        [TestMethod]
        public void TestGroupPermissions()
        {
            var provider = LoadProvider();
            Assert.IsFalse(provider.HasPermission(TestPlayer, "GroupPermission1"));
            Assert.IsTrue(provider.HasPermission(TestPlayer, "GroupPermission2"));
        }

        [TestMethod]
        public void TestPlayerPermissions()
        {
            var provider = LoadProvider();
            Assert.IsTrue(provider.HasPermission(TestPlayer, "PlayerPermission.Test"));
        }

        [TestMethod]
        public void TestChildPermissions()
        {
            var provider = LoadProvider();
            //should not be inherited
            Assert.IsFalse(provider.HasPermission(TestPlayer, "PlayerPermission.Test.ChildNode"));

            //should be inherited from PlayerPermission.Test2.*
            Assert.IsTrue(provider.HasPermission(TestPlayer, "PlayerPermission.Test2.ChildNode"));

            //only has permission to the childs; not to the node itself
            Assert.IsFalse(provider.HasPermission(TestPlayer, "PlayerPermission.Test2"));
        }

        [TestMethod]
        public void TestAddPermissionToGroup()
        {
            var provider = LoadProvider();
            var group = provider.GetGroup("TestGroup2");
            provider.AddPermission(group, "DynamicGroupPermission");

            Assert.IsTrue(provider.HasPermission(group, "DynamicGroupPermission"));
            Assert.IsTrue(provider.HasPermission(TestPlayer, "DynamicGroupPermission"));
        }

        [TestMethod]
        public void TestRemovePermissionFromGroup()
        {
            var provider = LoadProvider();
            var group = provider.GetGroup("TestGroup2");
            Assert.IsTrue(provider.RemovePermission(group, "GroupPermission2"));

            Assert.IsFalse(provider.HasPermission(group, "GroupPermission2"));
            Assert.IsFalse(provider.HasPermission(TestPlayer, "GroupPermission2"));
        }

        [TestMethod]
        public void TestAddPermissionToPlayer()
        {
            var provider = LoadProvider();
            provider.AddPermission(TestPlayer, "DynamicGroupPermission");
            Assert.IsTrue(provider.HasPermission(TestPlayer, "DynamicGroupPermission"));
        }

        [TestMethod]
        public void TestRemovePermissionFromPlayer()
        {
            var provider = LoadProvider();
            Assert.IsTrue(provider.RemovePermission(TestPlayer, "PlayerPermission.Test"));
            Assert.IsFalse(provider.HasPermission(TestPlayer, "PlayerPermission.Test"));
        }

        [TestMethod]
        public void TestHasAllPermissions()
        {
            var provider = LoadProvider();
            var group = provider.GetGroup("TestGroup2");

            Assert.IsTrue(provider.HasAllPermissions(group, "GroupPermission2", "GroupPermission2.Child"));
            Assert.IsTrue(provider.HasAllPermissions(TestPlayer,
                "PlayerPermission.Test", "PlayerPermission.Test2.ChildNode",
                "GroupPermission2", "GroupPermission2.Child"));

            var failPerms = new[] { "PlayerPermission.Test", "GroupPermission2", "NonExistantPermission" };

            Assert.IsFalse(provider.HasAllPermissions(group, failPerms));
            Assert.IsFalse(provider.HasAllPermissions(TestPlayer, failPerms));
        }

        [TestMethod]
        public void TestHasAnyPermissions()
        {
            var provider = LoadProvider();
            var group = provider.GetGroup("TestGroup2");

            Assert.IsTrue(provider.HasAnyPermissions(group, "GroupPermission2", "NonExistantPermission"));
            Assert.IsTrue(provider.HasAnyPermissions(TestPlayer, "PlayerPermission.Test", "NonExistantPermission"));

            var perms = new[] { "NonExistantPermission", "NonExistantPermission2" };

            Assert.IsFalse(provider.HasAnyPermissions(group, perms));
            Assert.IsFalse(provider.HasAnyPermissions(TestPlayer, perms));
        }

        [TestMethod]
        public void TestGetGroups()
        {
            IPermissionProvider permissionProvider = LoadProvider();
            IPermissionGroup[] groups = permissionProvider.GetGroups(TestPlayer).ToArray();
            Assert.AreEqual(groups.Length, 2);
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup2"));
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup3"));
        }

        [TestMethod]
        public void TestSaveException()
        {
            // Config of permission provider has not been loaded from a file so it can not be saved

            IPermissionProvider permissionProvider = LoadProvider();
            Assert.ThrowsException<NotSupportedException>(() => permissionProvider.Save());
        }

        [TestMethod]
        public void TestDeleteGroup()
        {
            IPermissionProvider permissionProvider = LoadProvider();

            permissionProvider.DeleteGroup(permissionProvider.GetGroup("TestGroup3"));

            IPermissionGroup[] groups = permissionProvider.GetGroups(TestPlayer).ToArray();
            Assert.AreEqual(groups.Length, 1);
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup2"));
        }

        [TestMethod]
        public void TestCreateGroup()
        {
            IPermissionProvider permissionProvider = LoadProvider();

            permissionProvider.CreateGroup(new PermissionGroup { Id = "TestGroup4", Name = "DynamicAddedGroup", Priority = 0 });

            IPermissionGroup[] groups = permissionProvider.GetGroups(TestPlayer).ToArray();
            Assert.AreEqual(groups.Length, 3);
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup2"));
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup3"));
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup4"));
        }

        protected IPermissionProvider LoadProvider()
        {
            var provider = GetPermissionProvider();
            provider.Load(GroupsConfig, PlayersConfig);
            return provider;
        }

        protected virtual IPermissionProvider GetPermissionProvider()
        {
            return Runtime.Container.Get<IPermissionProvider>();
        }
    }
}