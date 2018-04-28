namespace Rocket.API.Configuration
{
    public interface IConfiguration : IConfigurationElement
    {
        bool IsLoaded { get; }
        bool Exist(IConfigurationContext context);

        void Load(IConfigurationContext context, object defaultConfiguration);

        void SetContext(IConfigurationContext context);

        void LoadEmpty();

        void LoadFromObject(object o);

        void Reload();

        void Save();
    }
}