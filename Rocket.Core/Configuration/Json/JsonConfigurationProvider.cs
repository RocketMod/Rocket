using System.IO;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.Json
{
    public class JsonConfigurationProvider : IConfigurationProvider
    {
        private JsonConfigurationRoot root;

        public IConfigurationRoot Load(Stream stream)
        {
            root = new JsonConfigurationRoot();
            root.Load(stream);
            return root;
        }

        public IConfigurationRoot Root => root;

    }
}