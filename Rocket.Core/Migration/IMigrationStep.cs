using Rocket.API.DependencyInjection;

namespace Rocket.Core.Migration {
    public interface IMigrationStep
    {
        void Migrate(IDependencyContainer container, string basePath);
    }
}