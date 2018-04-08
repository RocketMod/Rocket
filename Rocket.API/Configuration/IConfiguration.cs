using System.IO;

namespace Rocket.API.Configuration
{
    public interface IConfiguration : IConfigurationBase
    {
        void Load(IEnvironmentContext context);

        void Reload();

        void Save();
    }
}