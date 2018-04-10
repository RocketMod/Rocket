using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Configuration;
using Rocket.Core.Configuration.Json;

namespace Rocket.Tests.Tests
{
    [TestClass]
    public class ConfigTests : RocketTestBase
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
            JsonConfiguration config = (JsonConfiguration) Runtime.Container.Get<IConfiguration>("defaultjson");

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

            TestConfig(config);
            TestSaveException(config);
        }

        [TestMethod]
        public void TestJsonSetObjectConfig()
        {
            JsonConfiguration config = (JsonConfiguration) Runtime.Container.Get<IConfiguration>("defaultjson");
            config.LoadEmpty();

            config.Set(TestConfigObject);
            TestConfig(config);
            TestSaveException(config);
        }

        [TestMethod]
        public void TestJsonLoadFromObjectConfig()
        {
            IConfiguration config = LoadConfig();
            TestConfig(config);
            TestSaveException(config);
        }

        protected IConfiguration LoadConfig()
        {
            IConfiguration config = GetConfig();
            config.LoadFromObject(TestConfigObject);
            return config;
        }

        protected virtual IConfiguration GetConfig()
        {
            return Runtime.Container.Get<IConfiguration>();
        }
        
        public void TestSaveException(IConfiguration config)
        {
            // Config has not been loaded from a file so it can not be saved
            Assert.ThrowsException<NotSupportedException>(() => config.Save());
        }

        public void TestConfig(IConfiguration config)
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
    }
}