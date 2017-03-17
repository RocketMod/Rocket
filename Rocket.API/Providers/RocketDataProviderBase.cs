namespace Rocket.API.Providers
{
    public class RocketDataProviderBase : RocketProviderBase
    {
        public virtual bool Save()
        {
            return false;
        }
    }
}
