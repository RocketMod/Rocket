using System.IO;

namespace Rocket.API.Configuration
{
    public interface IConfiguration : IConfigurationBase
    {
        void Load(IEnvironmentContext context);

        void LoadEmpty();

        void LoadFromObject(object o);

        void Reload();

        void Save();

        bool IsLoaded { get; }
    }
}