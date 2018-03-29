namespace Rocket.API.IOC
{
    public interface  IDependencyRegistrator
    {
        void Register(IDependencyContainer container, IDependencyResolver resolver);
    }
}
