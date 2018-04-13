using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Configuration;
using Rocket.Core.Configuration.Json;

namespace Rocket.Tests.Configuration
{
    [TestClass]
    [TestCategory("Configuration")]
    public class ConfigurationTests : RocketTestBase
    {
        protected object TestConfigObject { get; private set; }

        [TestInitialize]
        public void BootstrapConfigTest()
        {
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
        public void TestJsonConfig()
        {
            JsonConfiguration config = (JsonConfiguration)Runtime.Container.Get<IConfiguration>("defaultjson");

            string json =
                "{"
                    + "\"Test1\": \"A\","
                    + "\"NestedObjectTest\": {"
                        + "\"NestedStringValue\": \"B\","
                        + "\"NestedNumberValue\": 4,"
                        + "\"VeryNestedObject\": {"
                            + "\"Value\": \"3\""
                        + "}"
                    + "}"
                + "}";

            config.LoadFromJson(json);

            AssertConfigEquality(config);
            AssertSaveException(config);
        }

        [TestMethod]
        public void TestObjectConfig()
        {
            IConfiguration config = GetUnloadedConfig();
            config.LoadEmpty();

            config.Set(TestConfigObject);
            AssertConfigEquality(config);
            AssertSaveException(config);
        }

        [TestMethod]
        public void TestArrays()
        {
            var config = GetUnloadedConfig();
            config.LoadEmpty();

            var arraySection = config.CreateSection("ArrayTest", SectionType.Array);
            arraySection.Set(new[] { "Test1", "Test2"});
            var value = arraySection.Get(new string[0]);
            Assert.AreEqual(value.Length, 2);
            Assert.IsTrue(value.Contains("Test1"));
            Assert.IsTrue(value.Contains("Test2"));

            arraySection.Set(new string[0]);
            value = arraySection.Get(new string[0]);
            Assert.AreEqual(value.Length, 0);
        }

        [TestMethod]
        public void TestLoadFromObject()
        {
            IConfiguration config = LoadConfigFromObject();
            AssertConfigEquality(config);
            AssertSaveException(config);
        }

        [TestMethod]
        public void TestConfigSectionDeletion()
        {
            var config = LoadConfigFromObject();
            Assert.IsNotNull(config.GetSection("Test1"));
            Assert.IsTrue(config.RemoveSection("Test1"));
            Assert.ThrowsException<KeyNotFoundException>(() => config.GetSection("Test1"));
        }

        [TestMethod]
        public void TestConfigSectionCreation()
        {
            var config = LoadConfigFromObject();
            var section = config.CreateSection("dynamictest.test2", SectionType.Object);
            Assert.IsNotNull(section);
            Assert.IsTrue(section.IsNull);

            section.Set(new { test4 = false });
            Assert.IsFalse(section["test4"].Get<bool>());
            Assert.IsFalse(section.IsNull);

            var childSection = config.CreateSection("dynamictest.test2.test3", SectionType.Value);
            Assert.IsNotNull(childSection);
            Assert.IsTrue(childSection.IsNull);
            childSection.Set(true);
            Assert.IsFalse(childSection.IsNull);
            Assert.IsTrue(childSection.Get<bool>());

            Assert.IsFalse(section.IsNull);
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
            Assert.ThrowsException<NotSupportedException>(() => config.Save());
        }
        
        protected IConfiguration LoadConfigFromObject()
        {
            IConfiguration config = GetUnloadedConfig();
            config.LoadFromObject(TestConfigObject);
            return config;
        }

        protected virtual IConfiguration GetUnloadedConfig()
        {
            return Runtime.Container.Get<IConfiguration>();
        }

    }
}