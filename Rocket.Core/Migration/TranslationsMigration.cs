using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Rocket.API;
using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.I18N;
using Rocket.API.Logging;
using Rocket.API.Permissions;
using Rocket.Core.Configuration;
using Rocket.Core.Configuration.JsonNetBase;
using Rocket.Core.Configuration.Xml;
using Rocket.Core.Logging;
using Rocket.Core.Migration.LegacyTranslations;
using Rocket.Core.Permissions;

namespace Rocket.Core.Migration
{
    public class TranslationsMigration : IMigrationStep
    {
        public string Name => "Translations";
        public void Migrate(IDependencyContainer container, string basePath)
        {
            var implementation = container.Resolve<IImplementation>();

            var translationsLocator = container.Resolve<ITranslationLocator>();

            var toContext = new ConfigurationContext(implementation.WorkingDirectory, "Rocket.Unturned.Translations");
            translationsLocator.Load(toContext, new Dictionary<string, string>());
            
            var logger = container.Resolve<ILogger>();
            var xmlConfiguration = (XmlConfiguration)container.Resolve<IConfiguration>("xml");
            xmlConfiguration.ConfigurationRoot = "Translations";

            var fromContext = new ConfigurationContext(basePath, "Rocket.en.translation");

            if (!xmlConfiguration.Exists(fromContext))
            {
                logger.LogError("Translations migration failed: Rocket.en.translation.xml was not found in: " + basePath);
                return;
            }

            xmlConfiguration.Load(fromContext);

            var translations = (JArray)((JsonNetConfigurationSection) xmlConfiguration["Translation"]).Node;
            foreach (var translation in translations)
            {
                var id = translation["@Id"];
                var value = translation["@Value"];

                translationsLocator.SetFormat(id.Value<string>(), value.Value<string>());
            }
            translationsLocator.Save();
        }
    }
}