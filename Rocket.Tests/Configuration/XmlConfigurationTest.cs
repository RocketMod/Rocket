using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rocket.API.Configuration;
using Rocket.Core.Configuration.Xml;

namespace Rocket.Tests.Configuration
{
    [TestClass]
    public class XmlConfigurationTests : ConfigurationTestBase
    {
        protected override IConfiguration GetConfig() => Runtime.Container.Resolve<IConfiguration>("xml");

        [TestMethod]
        public void TestXmlLoading()
        {
            XmlConfiguration config = (XmlConfiguration) GetConfig();

            string xml =
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                + "<Config>"
                + "<Test1>A</Test1>"
                + "<NestedObjectTest>"
                + "<NestedStringValue>B</NestedStringValue>"
                + "<NestedNumberValue>4</NestedNumberValue>"
                + "<VeryNestedObject>"
                + "<Value>3</Value>"
                + "</VeryNestedObject>"
                + "</NestedObjectTest>"
                + "</Config>";

            config.LoadFromXml(xml);

            AssertConfigEquality(config);
            AssertSaveException(config);
        }
    }
}