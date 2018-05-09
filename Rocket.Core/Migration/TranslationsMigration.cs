using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Rocket.API;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.I18N;
using Rocket.API.Logging;
using Rocket.Core.Configuration;
using Rocket.Core.Configuration.JsonNetBase;
using Rocket.Core.Configuration.Xml;
using Rocket.Core.Logging;

namespace Rocket.Core.Migration
{
    public class TranslationsMigration : IMigrationStep
    {
        public string Name => "Translations";

        public void Migrate(IDependencyContainer container, string basePath)
        {
            IImplementation implementation = container.Resolve<IImplementation>();

            ITranslationCollection translationsLocator = container.Resolve<ITranslationCollection>();

            ConfigurationContext toContext =
                new ConfigurationContext(implementation.WorkingDirectory, "Rocket.Unturned.Translations");
            translationsLocator.Load(toContext, new Dictionary<string, string>());

            ILogger logger = container.Resolve<ILogger>();
            XmlConfiguration xmlConfiguration = (XmlConfiguration) container.Resolve<IConfiguration>("xml");
            xmlConfiguration.ConfigurationRoot = "Translations";

            ConfigurationContext fromContext = new ConfigurationContext(basePath, "Rocket.en.translation");

            if (!xmlConfiguration.Exists(fromContext))
            {
                logger.LogError(
                    "Translations migration failed: Rocket.en.translation.xml was not found in: " + basePath);
                return;
            }

            xmlConfiguration.Load(fromContext);

            JArray translations = (JArray) ((JsonNetConfigurationSection) xmlConfiguration["Translation"]).Node;
            foreach (JToken translation in translations)
            {
                JToken id = translation["@Id"];
                JToken value = translation["@Value"];

                translationsLocator.Set(id.Value<string>(), value.Value<string>());
            }

            translationsLocator.Save();
        }
    }
}