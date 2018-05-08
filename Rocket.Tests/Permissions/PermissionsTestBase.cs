using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Configuration;
using Rocket.API.Permissions;
using Rocket.API.User;
using Rocket.Core.Permissions;
using Rocket.Tests.Mock;

namespace Rocket.Tests.Permissions
{
    [TestCategory("Permissions")]
    public abstract class PermissionsTestBase : RocketTestBase
    {
        protected IConfiguration PlayersConfig { get; private set; }
        protected IConfiguration GroupsConfig { get; private set; }
        protected IUser TestPlayer { get; private set; }

        [TestInitialize]
        public override void Bootstrap()
        {
            base.Bootstrap();

            object sampleGroupsPermissions = new
            {
                Groups = new object[]
                {
                    new GroupPermissionSection
                    {
                        Id = "TestGroup1",
                        Name = "TestGroup",
                        Priority = 2,
                        Permissions = new[]
                        {
                            "GroupPermission1"
                        }
                    },
                    new GroupPermissionSection
                    {
                        Id = "TestGroup2",
                        Name = "TestGroup2",
                        Priority = 1,
                        Permissions = new[]
                        {
                            "GroupPermission2",
                            "GroupPermission2.Child",
                            "!GroupPermission3"
                        }
                    },
                    new GroupPermissionSection
                    {
                        Id = "TestGroup3",
                        Name = "PrimaryGroup",
                        Priority = 3
                    }
                }
            };

            object samplePlayers = new
            {
                TestPlayer = new object[]
                {
                    new PlayerPermissionSection
                    {
                        Id = "TestPlayerId",
                        Groups = new[]
                        {
                            "TestGroup3", "TestGroup2",
                            "TestGroup4" /* doesn't exist, shouldn't be exposed by GetGroups */
                        },
                        Permissions = new[]
                        {
                            "PlayerPermission.Test",
                            "PlayerPermission.Test2.*",
                            "!PlayerPermission.Test3"
                        }
                    }
                }
            };

            PlayersConfig = GetConfigurationProvider();
            PlayersConfig.LoadFromObject(samplePlayers);

            GroupsConfig = GetConfigurationProvider();
            GroupsConfig.LoadFromObject(sampleGroupsPermissions);

            TestPlayer = new TestPlayer(Runtime.Container).User;
        }

        public virtual IConfiguration GetConfigurationProvider() => Runtime.Container.Resolve<IConfiguration>();

        [TestMethod]
        public virtual void TestUpdateGroup()
        {
            IPermissionProvider provider = LoadProvider();
            PermissionGroup group = new PermissionGroup();
            group.Id = "TestGroup1";
            group.Name = "UpdatedGroupName";
            group.Priority = -1;

            provider.UpdateGroup(group);
            Assert.AreEqual(group.Name, "UpdatedGroupName");
            Assert.AreEqual(group.Priority, -1);
        }

        [TestMethod]
        public virtual void TestGroupPermissions()
        {
            IPermissionProvider provider = LoadProvider();
            IPermissionGroup group = provider.GetGroup("TestGroup2");

            Assert.AreEqual(PermissionResult.Default,
                provider.CheckPermission(TestPlayer,
                    "GroupPermission1")); // permission of a group the player doesnt belong to
            Assert.AreEqual(PermissionResult.Default, provider.CheckPermission(TestPlayer, "NonExistantPermission"));
            Assert.AreEqual(PermissionResult.Grant, provider.CheckPermission(TestPlayer, "GroupPermission2"));
            Assert.AreEqual(PermissionResult.Deny, provider.CheckPermission(TestPlayer, "GroupPermission3"));

            Assert.AreEqual(PermissionResult.Default, provider.CheckPermission(group, "NonExistantPermission"));
            Assert.AreEqual(PermissionResult.Grant, provider.CheckPermission(group, "GroupPermission2"));
            Assert.AreEqual(PermissionResult.Deny, provider.CheckPermission(group, "GroupPermission3"));
        }

        [TestMethod]
        public virtual void TestPlayerPermissions()
        {
            IPermissionProvider provider = LoadProvider();
            Assert.AreEqual(PermissionResult.Grant, provider.CheckPermission(TestPlayer, "PlayerPermission.Test"));
            Assert.AreEqual(PermissionResult.Deny, provider.CheckPermission(TestPlayer, "PlayerPermission.Test3"));
            Assert.AreEqual(PermissionResult.Default,
                provider.CheckPermission(TestPlayer, "PlayerPermission.NonExistantPermission"));
        }

        [TestMethod]
        public virtual void TestChildPermissions()
        {
            IPermissionProvider provider = LoadProvider();
            //should not be inherited
            Assert.AreEqual(PermissionResult.Default,
                provider.CheckPermission(TestPlayer, "PlayerPermission.Test.ChildNode"));

            //should be inherited from PlayerPermission.Test2.*
            Assert.AreEqual(PermissionResult.Grant,
                provider.CheckPermission(TestPlayer, "PlayerPermission.Test2.ChildNode"));

            //only has permission to the childs; not to the node itself
            Assert.AreEqual(PermissionResult.Default, provider.CheckPermission(TestPlayer, "PlayerPermission.Test2"));
        }

        [TestMethod]
        public virtual void TestAddPermissionToGroup()
        {
            IPermissionProvider provider = LoadProvider();
            IPermissionGroup group = provider.GetGroup("TestGroup2");
            provider.AddPermission(group, "DynamicGroupPermission");

            Assert.AreEqual(PermissionResult.Grant, provider.CheckPermission(group, "DynamicGroupPermission"));
            Assert.AreEqual(PermissionResult.Grant, provider.CheckPermission(TestPlayer, "DynamicGroupPermission"));
        }

        [TestMethod]
        public virtual void TestRemovePermissionFromGroup()
        {
            IPermissionProvider provider = LoadProvider();
            IPermissionGroup group = provider.GetGroup("TestGroup2");
            Assert.IsTrue(provider.RemovePermission(group, "GroupPermission2"));

            Assert.AreEqual(PermissionResult.Default, provider.CheckPermission(group, "GroupPermission2"));
            Assert.AreEqual(PermissionResult.Default, provider.CheckPermission(TestPlayer, "GroupPermission2"));
        }

        [TestMethod]
        public virtual void TestAddPermissionToPlayer()
        {
            IPermissionProvider provider = LoadProvider();
            provider.AddPermission(TestPlayer, "DynamicGroupPermission");

            Assert.AreEqual(PermissionResult.Grant, provider.CheckPermission(TestPlayer, "DynamicGroupPermission"));
        }

        [TestMethod]
        public virtual void TestRemovePermissionFromPlayer()
        {
            IPermissionProvider provider = LoadProvider();

            Assert.IsTrue(provider.RemovePermission(TestPlayer, "PlayerPermission.Test"));
            Assert.AreEqual(PermissionResult.Default, provider.CheckPermission(TestPlayer, "PlayerPermission.Test"));
        }

        [TestMethod]
        public virtual void TestHasAllPermissionsPlayer()
        {
            IPermissionProvider provider = LoadProvider();
            Assert.AreEqual(PermissionResult.Grant, provider.CheckHasAllPermissions(TestPlayer,
                "PlayerPermission.Test", "PlayerPermission.Test2.ChildNode",
                "GroupPermission2", "GroupPermission2.Child"));

            Assert.AreEqual(PermissionResult.Default,
                provider.CheckHasAllPermissions(TestPlayer, "PlayerPermission.Test", "GroupPermission2",
                    "NonExistantPermission"));

            //GroupPermission3 is explicitly denied
            Assert.AreEqual(PermissionResult.Deny,
                provider.CheckHasAllPermissions(TestPlayer, "PlayerPermission.Test", "GroupPermission2",
                    "GroupPermission3"));
        }

        [TestMethod]
        public virtual void TestHasAllPermissionsGroup()
        {
            IPermissionProvider provider = LoadProvider();
            IPermissionGroup group = provider.GetGroup("TestGroup2");

            Assert.AreEqual(PermissionResult.Grant,
                provider.CheckHasAllPermissions(group, "GroupPermission2", "GroupPermission2.Child"));
            Assert.AreEqual(PermissionResult.Default,
                provider.CheckHasAllPermissions(group, "GroupPermission2", "NonExistantPermission"));

            //GroupPermission3 is explicitly denied
            Assert.AreEqual(PermissionResult.Deny,
                provider.CheckHasAllPermissions(group, "GroupPermission2", "GroupPermission3"));
        }

        [TestMethod]
        public virtual void TestHasAnyPermissionsPlayer()
        {
            IPermissionProvider provider = LoadProvider();
            Assert.AreEqual(PermissionResult.Grant,
                provider.CheckHasAnyPermission(TestPlayer, "PlayerPermission.Test", "NonExistantPermission"));

            //Player does not inherit GroupPermission1
            Assert.AreEqual(PermissionResult.Default,
                provider.CheckHasAnyPermission(TestPlayer, "NonExistantPermission", "GroupPermission1"));

            //GroupPermission3 is explicitly denied
            Assert.AreEqual(PermissionResult.Deny,
                provider.CheckHasAnyPermission(TestPlayer, "NonExistantPermission", "GroupPermission3"));
        }

        [TestMethod]
        public virtual void TestHasAnyPermissionsGroup()
        {
            IPermissionProvider provider = LoadProvider();
            IPermissionGroup group = provider.GetGroup("TestGroup2");

            Assert.AreEqual(PermissionResult.Grant,
                provider.CheckHasAnyPermission(group, "GroupPermission2", "NonExistantPermission"));

            Assert.AreEqual(PermissionResult.Default,
                provider.CheckHasAnyPermission(group, "NonExistantPermission", "GroupPermission1"));

            //GroupPermission3 is explicitly denied
            Assert.AreEqual(PermissionResult.Deny,
                provider.CheckHasAnyPermission(group, "NonExistantPermission", "GroupPermission3"));
        }

        [TestMethod]
        public virtual void TestGetGroups()
        {
            IPermissionProvider permissionProvider = LoadProvider();
            IPermissionGroup[] groups = permissionProvider.GetGroups(TestPlayer).ToArray();
            Assert.AreEqual(groups.Length, 2);
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup2"));
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup3"));
        }

        [TestMethod]
        public virtual void TestSaveException()
        {
            // Config of permission provider has not been loaded from a file so it can not be saved

            IPermissionProvider permissionProvider = LoadProvider();
            Assert.ThrowsException<ConfigurationContextNotSetException>(() => permissionProvider.Save());
        }

        [TestMethod]
        public virtual void TestDeleteGroup()
        {
            IPermissionProvider permissionProvider = LoadProvider();

            permissionProvider.DeleteGroup(permissionProvider.GetGroup("TestGroup3"));

            IPermissionGroup[] groups = permissionProvider.GetGroups(TestPlayer).ToArray();
            Assert.AreEqual(groups.Length, 1);
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup2"));
        }

        [TestMethod]
        public virtual void TestCreateGroup()
        {
            IPermissionProvider permissionProvider = LoadProvider();

            permissionProvider.CreateGroup(new PermissionGroup
            {
                Id = "TestGroup4",
                Name = "DynamicAddedGroup",
                Priority = 0
            });

            IPermissionGroup[] groups = permissionProvider.GetGroups(TestPlayer).ToArray();
            Assert.AreEqual(groups.Length, 3);
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup2"));
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup3"));
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup4"));
        }

        protected abstract IPermissionProvider LoadProvider();

        protected abstract IPermissionProvider GetPermissionProvider();

        [TestMethod]
        public virtual void TestPermissionsLoad()
        {
            LoadProvider();
        }
    }
}