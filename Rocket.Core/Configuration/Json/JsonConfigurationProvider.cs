using System.IO;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.Json
{
    public class JsonConfigurationProvider : IConfigurationProvider
    {
        public IConfigurationRoot Load(Stream stream)
        {
            JsonConfigurationRoot root = new JsonConfigurationRoot();
            root.Load(stream);
            return root;
        }
    }
}