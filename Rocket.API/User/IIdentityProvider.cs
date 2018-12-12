using System.Threading.Tasks;

namespace Rocket.API.User
{
    public interface IIdentityProvider
    {
        /// <summary>
        /// Returns the IIdentity representation of the id (e.g steam ID)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IIdentity> GetIdentity(string id);
    }
}