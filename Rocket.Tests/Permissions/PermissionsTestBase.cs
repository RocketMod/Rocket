using System.Linq;
using System.Threading.Tasks;
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
                Users = new object[]
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

            TestPlayer = new TestPlayer(Runtime.Container, null).User;
        }

        public virtual IConfiguration GetConfigurationProvider() => Runtime.Container.Resolve<IConfiguration>();

        [TestMethod]
        public virtual async Task TestUpdateGroup()
        {
            IPermissionProvider provider = LoadProvider();
            PermissionGroup group = new PermissionGroup();
            group.Id = "TestGroup1";
            group.Name = "UpdatedGroupName";
            group.Priority = -1;

            await provider.UpdateGroupAsync(group);
            Assert.AreEqual(group.Name, "UpdatedGroupName");
            Assert.AreEqual(group.Priority, -1);
        }

        [TestMethod]
        public virtual async Task TestGroupPermissions()
        {
            IPermissionChecker checker = LoadChecker();
            IPermissionProvider provider = LoadProvider();

            IPermissionGroup group = await provider.GetGroupAsync("TestGroup2");

            Assert.AreEqual(PermissionResult.Default, await
                checker.CheckPermissionAsync(TestPlayer,
                    "GroupPermission1")); // permission of a group the player doesnt belong to
            Assert.AreEqual(PermissionResult.Default, await checker.CheckPermissionAsync(TestPlayer, "NonExistantPermission"));
            Assert.AreEqual(PermissionResult.Grant, await checker.CheckPermissionAsync(TestPlayer, "GroupPermission2"));
            Assert.AreEqual(PermissionResult.Deny, await checker.CheckPermissionAsync(TestPlayer, "GroupPermission3"));

            Assert.AreEqual(PermissionResult.Default, await checker.CheckPermissionAsync(group, "NonExistantPermission"));
            Assert.AreEqual(PermissionResult.Grant, await checker.CheckPermissionAsync(group, "GroupPermission2"));
            Assert.AreEqual(PermissionResult.Deny, await checker.CheckPermissionAsync(group, "GroupPermission3"));
        }

       

        [TestMethod]
        public virtual async Task TestPlayerPermissions()
        {
            IPermissionChecker checker = LoadChecker();
            Assert.AreEqual(PermissionResult.Grant, await checker.CheckPermissionAsync(TestPlayer, "PlayerPermission.Test"));
            Assert.AreEqual(PermissionResult.Deny, await checker.CheckPermissionAsync(TestPlayer, "PlayerPermission.Test3"));
            Assert.AreEqual(PermissionResult.Default, await checker.CheckPermissionAsync(TestPlayer, "PlayerPermission.NonExistantPermission"));
        }

        [TestMethod]
        public virtual async Task TestChildPermissions()
        {
            IPermissionChecker checker = LoadChecker();
            //should not be inherited
            Assert.AreEqual(PermissionResult.Default, await
                checker.CheckPermissionAsync(TestPlayer, "PlayerPermission.Test.ChildNode"));

            //should be inherited from PlayerPermission.Test2.*
            Assert.AreEqual(PermissionResult.Grant, await
                checker.CheckPermissionAsync(TestPlayer, "PlayerPermission.Test2.ChildNode"));

            //only has permission to the childs; not to the node itself
            Assert.AreEqual(PermissionResult.Default, await checker.CheckPermissionAsync(TestPlayer, "PlayerPermission.Test2"));
        }

        [TestMethod]
        public virtual async Task TestAddPermissionToGroup()
        {
            IPermissionChecker checker = LoadChecker();
            IPermissionProvider provider = LoadProvider();

            IPermissionGroup group = await provider.GetGroupAsync("TestGroup2");
            await provider.AddPermissionAsync(group, "DynamicGroupPermission");

            Assert.AreEqual(PermissionResult.Grant, await checker.CheckPermissionAsync(group, "DynamicGroupPermission"));
            Assert.AreEqual(PermissionResult.Grant, await checker.CheckPermissionAsync(TestPlayer, "DynamicGroupPermission"));
        }

        [TestMethod]
        public virtual async Task TestRemovePermissionFromGroup()
        {
            IPermissionProvider provider = LoadProvider();
            IPermissionChecker checker = LoadChecker();

            IPermissionGroup group = await provider.GetGroupAsync("TestGroup2");
            Assert.IsTrue(await provider.RemovePermissionAsync(group, "GroupPermission2"));

            Assert.AreEqual(PermissionResult.Default, await checker.CheckPermissionAsync(group, "GroupPermission2"));
            Assert.AreEqual(PermissionResult.Default, await checker.CheckPermissionAsync(TestPlayer, "GroupPermission2"));
        }

        [TestMethod]
        public virtual async Task TestAddPermissionToPlayer()
        {
            IPermissionChecker checker = LoadChecker();
            IPermissionProvider provider = LoadProvider();
            await provider.AddPermissionAsync(TestPlayer, "DynamicGroupPermission");

            Assert.AreEqual(PermissionResult.Grant, await checker.CheckPermissionAsync(TestPlayer, "DynamicGroupPermission"));
        }

        [TestMethod]
        public virtual async Task TestRemovePermissionFromPlayer()
        {
            IPermissionChecker checker = LoadChecker();
            IPermissionProvider provider = LoadProvider();

            Assert.IsTrue(await provider.RemovePermissionAsync(TestPlayer, "PlayerPermission.Test"));
            Assert.AreEqual(PermissionResult.Default, await checker.CheckPermissionAsync(TestPlayer, "PlayerPermission.Test"));
        }

        [TestMethod]
        public virtual async Task TestHasAllPermissionsPlayer()
        {
            IPermissionChecker checker = LoadChecker();

            Assert.AreEqual(PermissionResult.Grant, await checker.CheckHasAllPermissionsAsync(TestPlayer,
                "PlayerPermission.Test", "PlayerPermission.Test2.ChildNode",
                "GroupPermission2", "GroupPermission2.Child"));

            Assert.AreEqual(PermissionResult.Default,
                await checker.CheckHasAllPermissionsAsync(TestPlayer, "PlayerPermission.Test", "GroupPermission2",
                    "NonExistantPermission"));

            //GroupPermission3 is explicitly denied
            Assert.AreEqual(PermissionResult.Deny,
                await checker.CheckHasAllPermissionsAsync(TestPlayer, "PlayerPermission.Test", "GroupPermission2",
                    "GroupPermission3"));
        }

        [TestMethod]
        public virtual async Task TestHasAllPermissionsGroup()
        {
            IPermissionChecker checker = LoadChecker();
            IPermissionProvider provider = LoadProvider();
            IPermissionGroup group = await provider.GetGroupAsync("TestGroup2");

            Assert.AreEqual(PermissionResult.Grant,
                await checker.CheckHasAllPermissionsAsync(group, "GroupPermission2", "GroupPermission2.Child"));
            Assert.AreEqual(PermissionResult.Default,
                await checker.CheckHasAllPermissionsAsync(group, "GroupPermission2", "NonExistantPermission"));

            //GroupPermission3 is explicitly denied
            Assert.AreEqual(PermissionResult.Deny,
                await checker.CheckHasAllPermissionsAsync(group, "GroupPermission2", "GroupPermission3"));
        }

        [TestMethod]
        public virtual async Task TestHasAnyPermissionsPlayer()
        {
            IPermissionChecker checker = LoadChecker();
            Assert.AreEqual(PermissionResult.Grant,
                await checker.CheckHasAnyPermissionAsync(TestPlayer, "PlayerPermission.Test", "NonExistantPermission"));

            //Player does not inherit GroupPermission1
            Assert.AreEqual(PermissionResult.Default,
                await checker.CheckHasAnyPermissionAsync(TestPlayer, "NonExistantPermission", "GroupPermission1"));

            //GroupPermission3 is explicitly denied
            Assert.AreEqual(PermissionResult.Deny,
                await checker.CheckHasAnyPermissionAsync(TestPlayer, "NonExistantPermission", "GroupPermission3"));
        }

        [TestMethod]
        public virtual async Task TestHasAnyPermissionsGroup()
        {
            IPermissionChecker checker = LoadChecker();
            IPermissionProvider provider = LoadProvider();
            IPermissionGroup group = await provider.GetGroupAsync("TestGroup2");

            Assert.AreEqual(PermissionResult.Grant,
                await checker.CheckHasAnyPermissionAsync(group, "GroupPermission2", "NonExistantPermission"));

            Assert.AreEqual(PermissionResult.Default,
                await checker.CheckHasAnyPermissionAsync(group, "NonExistantPermission", "GroupPermission1"));

            //GroupPermission3 is explicitly denied
            Assert.AreEqual(PermissionResult.Deny,
                await checker.CheckHasAnyPermissionAsync(group, "NonExistantPermission", "GroupPermission3"));
        }

        [TestMethod]
        public virtual async Task TestGetGroupsAsync()
        {
            IPermissionProvider permissionProvider = LoadProvider();
            IPermissionGroup[] groups = (await permissionProvider.GetGroupsAsync(TestPlayer)).ToArray();
            Assert.AreEqual(groups.Length, 2);
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup2"));
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup3"));
        }

        [TestMethod]
        public virtual async Task TestSaveException()
        {
            // Config of permission provider has not been loaded from a file so it can not be saved

            IPermissionProvider permissionProvider = LoadProvider();
            Assert.ThrowsException<ConfigurationContextNotSetException>(() => permissionProvider.SaveAsync().GetAwaiter().GetResult());
        }

        [TestMethod]
        public virtual async Task TestDeleteGroup()
        {
            IPermissionProvider permissionProvider = LoadProvider();

            await permissionProvider.DeleteGroupAsync(await permissionProvider.GetGroupAsync("TestGroup3"));

            IPermissionGroup[] groups = (await permissionProvider.GetGroupsAsync(TestPlayer)).ToArray();
            Assert.AreEqual(groups.Length, 1);
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup2"));
        }

        [TestMethod]
        public virtual async Task TestCreateGroup()
        {
            IPermissionProvider permissionProvider = LoadProvider();

            await permissionProvider.CreateGroupAsync(new PermissionGroup
            {
                Id = "TestGroup4",
                Name = "DynamicAddedGroup",
                Priority = 0
            });

            IPermissionGroup[] groups = (await permissionProvider.GetGroupsAsync(TestPlayer)).ToArray();
            Assert.AreEqual(groups.Length, 3);
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup2"));
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup3"));
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup4"));
        }

        protected abstract IPermissionProvider LoadProvider();
        protected abstract IPermissionChecker LoadChecker();

        [TestMethod]
        public virtual void TestPermissionsLoad()
        {
            LoadProvider();
        }
    }
}