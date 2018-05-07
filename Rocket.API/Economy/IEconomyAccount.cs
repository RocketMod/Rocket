using Rocket.API.Permissions;
using Rocket.API.Player;

namespace Rocket.API.Economy
{
    /// <summary>
    ///     An economy account.
    /// </summary>
    public interface IEconomyAccount
    {
        /// <summary>
        ///     The owner of the account.
        /// </summary>
        IIdentifiable Owner { get; }

        /// <summary>
        ///     The name of the account. 
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The accounts balance.
        /// </summary>
        decimal Balance { get; }
        
        /// <summary>
        ///     The accounts currency. Can be null.
        /// </summary>
        IEconomyCurrency Currency { get; }
    }
}