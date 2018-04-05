using System.IO;

namespace Rocket.API.Configuration
{
    public interface IConfigurationProvider
    {
        IConfigurationRoot Load(Stream stream);
    }
}