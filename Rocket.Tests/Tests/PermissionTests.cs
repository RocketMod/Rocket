using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Configuration;
using Rocket.API.Permissions;
using Rocket.Core.Configuration.Json;

namespace Rocket.Tests.Tests
{
    [TestClass]
    public class PermissionTests : RocketTestBase
    {
        private IConfiguration playersConfig;
        private IConfiguration groupsConfig;

        [TestInitialize]
        public void BootstrapPermissionTest()
        {
            object sampleGroupsPermissions = new
            {
                Groups = new
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
                }
            };

            object samplePlayers = new
            {
                Players = new
                {
                    TestPlayer = new
                    {
                        TestPlayerId = new
                        {
                            LastDisplayName = "Trojaner",
                            Groups = new[] { "TestGroup3", "TestGroup2" /* doesn't exist */ }
                        }
                    }
                }
            };

            playersConfig = Runtime.Container.Get<IConfiguration>();
            playersConfig.LoadFromObject(samplePlayers);

            groupsConfig = Runtime.Container.Get<IConfiguration>();
            groupsConfig.LoadFromObject(sampleGroupsPermissions);
        }

        [TestMethod]
        public void TestGroups()
        {
            TestPlayer player = new TestPlayer
            {
                Id = "TestPlayerId"
            };

            IPermissionProvider permissionProvider = Runtime.Container.Get<IPermissionProvider>();
            permissionProvider.Load(groupsConfig, playersConfig);

            var groups = permissionProvider.GetGroups(player).ToArray();
            Assert.AreEqual(groups.Length, 2);
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup3"));
            Assert.IsFalse(groups.Select(c => c.Id).Contains("TestGroup4"));
        }
    }
}