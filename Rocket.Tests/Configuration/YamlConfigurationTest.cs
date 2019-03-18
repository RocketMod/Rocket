using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Configuration;
using Rocket.Core.Configuration.Json;
using Rocket.Core.Configuration.Yaml;

namespace Rocket.Tests.Configuration
{
    [TestClass]
    public class YamlConfigurationTest : ConfigurationTestBase
    {
        [TestMethod]
        public void TestYamlLoading()
        {
            YamlConfiguration config = (YamlConfiguration)GetConfig();
            var nl = Environment.NewLine;

            string yaml = 
                $"Test1: \"A\"{nl}" +
                $"NestedObjectTest:{nl}" +
                $"  NestedStringValue: \"B\"{nl}" + 
                $"  NestedNumberValue: 4{nl}" +
                $"  VeryNestedObject:{nl}" + 
                $"    Value: \"3\"{nl}";

            config.LoadFromYaml(yaml);

            AssertConfigEquality(config);
            AssertSaveException(config);
        }

        protected override IConfiguration GetConfig() => Runtime.Container.Resolve<IConfiguration>("yaml");
    }
}