using System.IO;

namespace Rocket.API.Configuration
{
    public interface IConfiguration : IConfigurationBase
    {
        void Load(Stream stream);

        void Save(Stream stream);
    }
}