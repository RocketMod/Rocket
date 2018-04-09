using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Configuration;
using Rocket.API.Permissions;

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
                            Groups = new[] { "TestGroup3", "TestGroup2", "TestGroup4" /* doesn't exist, shouldn't be exposed by GetGroups */ }
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

        public virtual IConfiguration GetConfigurationProvider()
        {
            return Runtime.Container.Get<IConfiguration>();
        }

        [TestMethod]
        public void TestGroups()
        {
            IPermissionProvider permissionProvider = Runtime.Container.Get<IPermissionProvider>();
            permissionProvider.Load(GroupsConfig, PlayersConfig);

            var groups = permissionProvider.GetGroups(TestPlayer).ToArray();
            Assert.AreEqual(groups.Length, 2);
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup2"));
            Assert.IsTrue(groups.Select(c => c.Id).Contains("TestGroup3"));

            // Config has not been loaded from a file so it can not be saved
            Assert.ThrowsException<NotSupportedException>(() => permissionProvider.Save());
        }
    }
}