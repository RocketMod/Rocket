namespace Rocket.API.Providers
{
    public abstract class RocketProviderBase : IRocketProviderBase
    {
        public abstract void Load(bool isReload = false);
        public abstract void Unload();
    }
}
