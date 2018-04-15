using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Configuration;
using Rocket.Core.Configuration.Json;

namespace Rocket.Tests.Configuration
{
    [TestClass]
    public class JsonConfigurationTests : ConfigurationTestsBase
    {
        [TestMethod]
        public void TestJsonLoading()
        {
            JsonConfiguration config = (JsonConfiguration) GetConfig();

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

        protected override IConfiguration GetConfig()
        {
            return Runtime.Container.Get<IConfiguration>("json");
        }
    }
}