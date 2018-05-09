using Rocket.API.DependencyInjection;

namespace Rocket.Core.Migration
{
    public interface IMigrationStep
    {
        string Name { get; }
        void Migrate(IDependencyContainer container, string basePath);
    }
}