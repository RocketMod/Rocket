namespace Rocket.API.DependencyInjection
{
    public interface  IDependencyRegistrator
    {
        void Register(IDependencyContainer container, IDependencyResolver resolver);
    }
}
