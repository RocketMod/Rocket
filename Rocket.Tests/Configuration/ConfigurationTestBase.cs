using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Configuration;

namespace Rocket.Tests.Configuration
{
    [TestCategory("Configuration")]
    public abstract class ConfigurationTestBase : RocketTestBase
    {
        protected object TestConfigObject { get; private set; }

        [TestInitialize]
        public override void Bootstrap()
        {
            base.Bootstrap();

            TestConfigObject = new
            {
                Test1 = "A",
                NestedObjectTest = new
                {
                    NestedStringValue = "B",
                    NestedNumberValue = 4,
                    VeryNestedObject = new
                    {
                        Value = "3"
                    }
                }
            };
        }

        [TestMethod]
        public virtual void TestObjectConfig()
        {
            IConfiguration config = GetConfig();
            config.LoadEmpty();

            config.Set(TestConfigObject);
            AssertConfigEquality(config);
            AssertSaveException(config);
        }

        [TestMethod]
        public virtual void TestArrays()
        {
            IConfiguration config = GetConfig();
            config.LoadEmpty();

            IConfigurationSection arraySection = config.CreateSection("ArrayTest", SectionType.Array);
            arraySection.Set(new[] {"Test1", "Test2"});
            string[] value = arraySection.Get(new string[0]);
            Assert.AreEqual(value.Length, 2);
            Assert.IsTrue(value.Contains("Test1"));
            Assert.IsTrue(value.Contains("Test2"));

            arraySection.Set(new string[0]);
            value = arraySection.Get(new string[0]);
            Assert.AreEqual(value.Length, 0);
        }

        [TestMethod]
        public virtual void TestLoadFromObject()
        {
            IConfiguration config = LoadConfigFromObject();
            AssertConfigEquality(config);
            AssertSaveException(config);
        }

        [TestMethod]
        public virtual void TestConfigSectionDeletion()
        {
            IConfiguration config = LoadConfigFromObject();
            Assert.IsNotNull(config.GetSection("Test1"));
            Assert.IsTrue(config.DeleteSection("Test1"));
            Assert.ThrowsException<KeyNotFoundException>(() => config.GetSection("Test1"));
        }

        [TestMethod]
        public virtual void TestConfigSectionCreation()
        {
            IConfiguration config = LoadConfigFromObject();
            IConfigurationSection section = config.CreateSection("dynamictest.test2", SectionType.Object);
            Assert.IsNotNull(section);
            Assert.IsTrue(section.HasValue);

            section.Set(new {test4 = false});
            Assert.IsFalse(section["test4"].Get<bool>());
            Assert.IsFalse(section.HasValue);

            IConfigurationSection childSection = config.CreateSection("dynamictest.test2.test3", SectionType.Value);
            Assert.IsNotNull(childSection);
            Assert.IsTrue(childSection.HasValue);
            childSection.Set(true);
            Assert.IsFalse(childSection.HasValue);
            Assert.IsTrue(childSection.Get<bool>());

            Assert.IsFalse(section.HasValue);
        }

        [TestMethod]
        public void TestNotLoadedException()
        {
            IConfiguration config = GetConfig();
            Assert.ThrowsException<ConfigurationNotLoadedException>(() => config["a"]);
            Assert.ThrowsException<ConfigurationNotLoadedException>(() => config.GetSection("A"));
            Assert.ThrowsException<ConfigurationNotLoadedException>(() => config.Set("ll"));
        }

        public void AssertConfigEquality(IConfiguration config)
        {
            Assert.AreEqual(config.GetSection("Test1").Get<string>(), "A");
            Assert.AreEqual(config.GetSection("NestedObjectTest")
                                  .GetSection("NestedStringValue")
                                  .Get<string>(), "B");

            Assert.AreEqual(config.GetSection("NestedObjectTest")
                                  .GetSection("NestedNumberValue")
                                  .Get<int>(), 4);

            Assert.AreEqual(config.GetSection("NestedObjectTest")
                                  .GetSection("VeryNestedObject")
                                  .GetSection("Value")
                                  .Get<string>(), "3");

            Assert.AreEqual(config["Test1"].Get<string>(), "A");
            Assert.AreEqual(config["NestedObjectTest.NestedStringValue"].Get<string>(), "B");
            Assert.AreEqual(config["NestedObjectTest.NestedNumberValue"].Get<int>(), 4);
            Assert.AreEqual(config["NestedObjectTest.VeryNestedObject.Value"].Get<string>(), "3");

            Assert.AreEqual(config["NestedObjectTest.VeryNestedObject.Value"].Key, "Value");
            Assert.AreEqual(config["NestedObjectTest.VeryNestedObject.Value"].Path,
                "NestedObjectTest.VeryNestedObject.Value");

            Assert.AreEqual(config["NestedObjectTest"]["NestedStringValue"].Get<string>(), "B");
            Assert.AreEqual(config["NestedObjectTest"]["NestedNumberValue"].Get<int>(), 4);
            Assert.AreEqual(config["NestedObjectTest"]["VeryNestedObject"]["Value"].Get<string>(), "3");

            Assert.AreEqual(config["NestedObjectTest"]["VeryNestedObject"].GetSection("Value").Get<string>(), "3");

            Assert.AreEqual(config.GetSection("NestedObjectTest.NestedStringValue").Get<string>(), "B");
            Assert.AreEqual(config.GetSection("NestedObjectTest.NestedNumberValue").Get<int>(), 4);
            Assert.AreEqual(config.GetSection("NestedObjectTest.VeryNestedObject.Value").Get<string>(), "3");
        }

        public void AssertSaveException(IConfiguration config)
        {
            // Config has not been loaded from a file so it can not be saved
            Assert.ThrowsException<ConfigurationContextNotSetException>(() => config.SaveAsync().GetAwaiter().GetResult());
        }

        protected IConfiguration LoadConfigFromObject()
        {
            IConfiguration config = GetConfig();
            config.LoadFromObject(TestConfigObject);
            return config;
        }

        protected abstract IConfiguration GetConfig();
    }
}