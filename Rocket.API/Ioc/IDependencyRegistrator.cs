namespace Rocket.API.Ioc
{
    public interface IDependencyRegistrator
    {
        void Register(IDependencyContainer container, IDependencyResolver resolver);
    }
}