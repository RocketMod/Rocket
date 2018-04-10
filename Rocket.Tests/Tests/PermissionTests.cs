using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Configuration;
using Rocket.API.Permissions;
using Rocket.Core.Permissions;

namespace Rocket.Tests.Tests
{
    [TestClass]
    public class PermissionTests : RocketTestBase
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
                    Priority = 2
                },
                TestGroup2 = new
                {
                    Name = "TestGroup2",
                    Priority = 1
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

            permissionProvider.CreateGroup(new PermissionGroup { Id = "TestGroup4", Name = "DynamicAddedGroup", Priority = 0});

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