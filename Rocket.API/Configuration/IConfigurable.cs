namespace Rocket.API.Configuration
{
    public interface IConfigurable
    {
        IConfiguration Configuration { get; }
    }
}