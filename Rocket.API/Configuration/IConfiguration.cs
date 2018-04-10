using System.IO;

namespace Rocket.API.Configuration
{
    public interface IConfiguration : IConfigurationBase
    {
        bool Exist(IEnvironmentContext context);

        void Load(IEnvironmentContext context, object defaultConfiguration);

        void LoadEmpty();

        void LoadFromObject(object o);

        void Reload();

        void Save();

        bool IsLoaded { get; }
    }
}