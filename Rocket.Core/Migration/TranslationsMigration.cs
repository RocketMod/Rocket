using Rocket.API.DependencyInjection;

namespace Rocket.Core.Migration
{
    public class TranslationsMigration : IMigrationStep
    {
        public string Name => "Translations";
        public void Migrate(IDependencyContainer container, string basePath)
        {

        }
    }
}