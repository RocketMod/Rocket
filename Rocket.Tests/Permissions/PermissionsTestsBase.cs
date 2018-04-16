using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Configuration;
using Rocket.API.Permissions;
using Rocket.Core.Permissions;

namespace Rocket.Tests.Permissions
{
    [TestCategory("Permissions")]
    public abstract class PermissionsTestsBase : RocketTestBase
    {
        protected IConfiguration PlayersConfig { get; private set; }
        protected IConfiguration GroupsConfig { get; private set; }
        protected TestPlayer TestPlayer { get; private set; }

        [TestInitialize]
        public override void Bootstrap()
        {
            base.Bootstrap();

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
                        "GroupPermission2.Child",
                        "!GroupPermission3"
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

            TestPlayer = new TestPlayer(Runtime.Container);
        }

        public virtual IConfiguration GetConfigurationProvider() => Runtime.Container.Get<IConfiguration>();

        [TestMethod]
        public virtual void TestUpdateGroup()
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
        public virtual void TestGroupPermissions()
        {
            var provider = LoadProvider();
            var group = provider.GetGroup("TestGroup2");

            Assert.AreEqual(PermissionResult.Default, provider.HasPermission(TestPlayer, "GroupPermission1")); // permission of a group the player doesnt belong to
            Assert.AreEqual(PermissionResult.Default, provider.HasPermission(TestPlayer, "NonExistantPermission"));
            Assert.AreEqual(PermissionResult.Grant, provider.HasPermission(TestPlayer, "GroupPermission2"));
            Assert.AreEqual(PermissionResult.Deny, provider.HasPermission(TestPlayer, "GroupPermission3"));

            Assert.AreEqual(PermissionResult.Default, provider.HasPermission(group, "NonExistantPermission"));
            Assert.AreEqual(PermissionResult.Grant, provider.HasPermission(group, "GroupPermission2"));
            Assert.AreEqual(PermissionResult.Deny, provider.HasPermission(group, "GroupPermission3"));
        }

        [TestMethod]
        public virtual void TestPlayerPermissions()
        {
            var provider = LoadProvider();
            Assert.AreEqual(PermissionResult.Grant, provider.HasPermission(TestPlayer, "PlayerPermission.Test"));
            Assert.AreEqual(PermissionResult.Deny, provider.HasPermission(TestPlayer, "PlayerPermission.Test3"));
            Assert.AreEqual(PermissionResult.Default, provider.HasPermission(TestPlayer, "PlayerPermission.NonExistantPermission"));
        }

        [TestMethod]
        public virtual void TestChildPermissions()
        {
            var provider = LoadProvider();
            //should not be inherited
            Assert.AreEqual(PermissionResult.Default, provider.HasPermission(TestPlayer, "PlayerPermission.Test.ChildNode"));

            //should be inherited from PlayerPermission.Test2.*
            Assert.AreEqual(PermissionResult.Grant,
                provider.HasPermission(TestPlayer, "PlayerPermission.Test2.ChildNode"));

            //only has permission to the childs; not to the node itself
            Assert.AreEqual(PermissionResult.Default, provider.HasPermission(TestPlayer, "PlayerPermission.Test2"));
        }

        [TestMethod]
        public virtual void TestAddPermissionToGroup()
        {
            var provider = LoadProvider();
            var group = provider.GetGroup("TestGroup2");
            provider.AddPermission(group, "DynamicGroupPermission");

            Assert.AreEqual(PermissionResult.Grant, provider.HasPermission(group, "DynamicGroupPermission"));
            Assert.AreEqual(PermissionResult.Grant, provider.HasPermission(TestPlayer, "DynamicGroupPermission"));
        }

        [TestMethod]
        public virtual void TestRemovePermissionFromGroup()
        {
            var provider = LoadProvider();
            var group = provider.GetGroup("TestGroup2");
            Assert.IsTrue(provider.RemovePermission(group, "GroupPermission2"));

            Assert.AreEqual(PermissionResult.Default, provider.HasPermission(group, "GroupPermission2"));
            Assert.AreEqual(PermissionResult.Default, provider.HasPermission(TestPlayer, "GroupPermission2"));
        }

        [TestMethod]
        public virtual void TestAddPermissionToPlayer()
        {
            var provider = LoadProvider();
            provider.AddPermission(TestPlayer, "DynamicGroupPermission");

            Assert.AreEqual(PermissionResult.Grant, provider.HasPermission(TestPlayer, "DynamicGroupPermission"));
        }

        [TestMethod]
        public virtual void TestRemovePermissionFromPlayer()
        {
            var provider = LoadProvider();

            Assert.IsTrue(provider.RemovePermission(TestPlayer, "PlayerPermission.Test"));
            Assert.AreEqual(PermissionResult.Default, provider.HasPermission(TestPlayer, "PlayerPermission.Test"));
        }

        [TestMethod]
        public virtual void TestHasAllPermissionsPlayer()
        {
            var provider = LoadProvider();
            Assert.AreEqual(PermissionResult.Grant, provider.HasAllPermissions(TestPlayer,
                "PlayerPermission.Test", "PlayerPermission.Test2.ChildNode",
                "GroupPermission2", "GroupPermission2.Child"));
            
            Assert.AreEqual(PermissionResult.Default, provider.HasAllPermissions(TestPlayer, "PlayerPermission.Test", "GroupPermission2", "NonExistantPermission"));

            //GroupPermission3 is explicitly denied
            Assert.AreEqual(PermissionResult.Deny, provider.HasAllPermissions(TestPlayer, "PlayerPermission.Test", "GroupPermission2", "GroupPermission3"));
        }

        [TestMethod]
        public virtual void TestHasAllPermissionsGroup()
        {
            var provider = LoadProvider();
            var group = provider.GetGroup("TestGroup2");

            Assert.AreEqual(PermissionResult.Grant, provider.HasAllPermissions(group, "GroupPermission2", "GroupPermission2.Child"));
            Assert.AreEqual(PermissionResult.Default, provider.HasAllPermissions(group, "GroupPermission2", "NonExistantPermission"));

            //GroupPermission3 is explicitly denied
            Assert.AreEqual(PermissionResult.Deny, provider.HasAllPermissions(group, "GroupPermission2", "GroupPermission3"));
        }

        [TestMethod]
        public virtual void TestHasAnyPermissionsPlayer()
        {
            var provider = LoadProvider();
            Assert.AreEqual(PermissionResult.Grant, provider.HasAnyPermissions(TestPlayer, "PlayerPermission.Test", "NonExistantPermission"));

            //Player does not inherit GroupPermission1
            Assert.AreEqual(PermissionResult.Default, provider.HasAnyPermissions(TestPlayer, "NonExistantPermission", "GroupPermission1"));

            //GroupPermission3 is explicitly denied
            Assert.AreEqual(PermissionResult.Deny, provider.HasAnyPermissions(TestPlayer, "NonExistantPermission", "GroupPermission3"));
        }

        [TestMethod]
        public virtual void TestHasAnyPermissionsGroup()
        {
            var provider = LoadProvider();
            var group = provider.GetGroup("TestGroup2");

            Assert.AreEqual(PermissionResult.Grant, provider.HasAnyPermissions(group, "GroupPermission2", "NonExistantPermission"));

            Assert.AreEqual(PermissionResult.Default, provider.HasAnyPermissions(group, "NonExistantPermission", "GroupPermission1"));
    
            //GroupPermission3 is explicitly denied
            Assert.AreEqual(PermissionResult.Deny, provider.HasAnyPermissions(group, "NonExistantPermission", "GroupPermission3"));
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
            Assert.ThrowsException<NotSupportedException>(() => permissionProvider.Save());
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

            permissionProvider.CreateGroup(new PermissionGroup { Id = "TestGroup4", Name = "DynamicAddedGroup", Priority = 0 });

            IPermissionGroup[] groups = permissionProvider.GetGroups(TestPlayer).ToArray();
            Assert.AreEqual(groups.Length, 3);
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup2"));
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup3"));
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup4"));
        }

        protected virtual IPermissionProvider LoadProvider()
        {
            var provider = GetPermissionProvider();
            provider.Load(GroupsConfig, PlayersConfig);
            return provider;
        }

        protected abstract IPermissionProvider GetPermissionProvider();

        [TestMethod]
        public void TestPermissionsLoad()
        {
            var notLoadedProvider = GetPermissionProvider();
            notLoadedProvider.Load(GroupsConfig, PlayersConfig); //should not throw an exception
        }
    }
}