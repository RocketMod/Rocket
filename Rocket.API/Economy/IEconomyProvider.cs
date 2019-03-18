using System.Collections.Generic;
using System.Threading.Tasks;
using Rocket.API.DependencyInjection;
using Rocket.API.User;

namespace Rocket.API.Economy
{
    /// <summary>
    ///     The service responsible for managing economy and currencies.
    /// </summary>
    public interface IEconomyProvider: IService
    {
        /// <summary>
        ///     All currencies. Contains at least one element.
        /// </summary>
        IEnumerable<IEconomyCurrency> Currencies { get; }

        /// <summary>
        ///     The default currency. Will never return null.
        /// </summary>
        IEconomyCurrency DefaultCurrency { get; }

        /// <summary>
        ///     Adds balance to the users account.
        /// </summary>
        /// <param name="owner">The account owner.</param>
        /// <param name="amount">The amount to add. Should not be negative.</param>
        /// <param name="reason">The reason of the transaction.</param>
        Task AddBalanceAsync(IUser owner, decimal amount, string reason = null);

        /// <summary>
        ///     Makes a transaction from one account to another.
        /// </summary>
        /// <param name="source">The source account.</param>
        /// <param name="target">The target account.</param>
        /// <param name="amount">The amount to transfer. Must not be negative.</param>
        /// <param name="reason">The transaction reason (optional).</param>
        /// <returns><b>True</b> if the transaction was successful; otherwise, <b>false</b>.</returns>
        Task<bool> TransferAsync(IEconomyAccount source, IEconomyAccount target, decimal amount, string reason = null);

        /// <summary>
        ///     Adds balance to the given account.
        /// </summary>
        /// <param name="amount">The amount to add. Should not be negative.</param>
        /// <param name="account">The account to add balance to.</param>
        /// <param name="reason">The reason of the transaction.</param>
        Task<bool> AddBalanceAsync(IEconomyAccount account, decimal amount, string reason = null);

        /// <summary>
        ///     Removes balance from the users account.
        /// </summary>
        /// <param name="owner">The account owner.</param>
        /// <param name="amount">The amount to remove. Should not be negative.</param>
        /// <param name="reason">The reason of the transaction.</param>
        /// <returns><b>true</b> if the balance could be removed; otherwise, <b>false</b>.</returns>
        Task<bool> RemoveBalanceAsync(IUser owner, decimal amount, string reason = null);

        /// <summary>
        ///     Removes balance from the users account based on a specific currency.
        /// </summary>
        /// <param name="amount">The amount to remove. Should not be negative.</param>
        /// <param name="reason">The reason of the transaction.</param>
        /// <param name="account">The account to remove balance from.</param>
        /// <returns><b>true</b> if the balance could be removed; otherwise, <b>false</b>.</returns>
        Task<bool> RemoveBalanceAsync(IEconomyAccount account, decimal amount, string reason = null);

        /// <summary>
        ///     Sets the balance of the users account.
        /// </summary>
        /// <param name="owner">The account owner.</param>
        /// <param name="amount">Amount to set the balance to.</param>
        Task SetBalanceAsync(IUser owner, decimal amount);

        /// <summary>
        ///     Sets the balance of the users account in a specific currency.
        /// </summary>
        /// <param name="account">The account to set the balance of.</param>
        /// <param name="amount">Amount to set the balance to.</param>
        Task SetBalanceAsync(IEconomyAccount account, decimal amount);

        /// <summary>
        ///     Checks if the account of the user can have negative balance.
        /// </summary>
        /// <param name="account">Checks if the given account has access.</param>
        /// <returns><b>true</b> if the account can have negative balance; otherwise, <b>false</b>.</returns>
        Task<bool> SupportsNegativeBalanceAsync(IEconomyAccount account);

        /// <summary>
        ///     Creates an account.
        /// </summary>
        /// <param name="owner">The owner of the account.</param>
        /// <param name="name">The name of the account.</param>
        /// <param name="account">The account instance if it was created; otherwise, <b>null</b>.</param>
        /// <exception>If the account creation failed because of an internal error.</exception>
        /// <returns>
        ///     <b>true</b> if account creation is supported and the account didn't exist already, and was created; otherwise,
        ///     <b>false</b>.
        /// </returns>
        Task<bool> CreateAccountAsync(IUser owner, string name, out IEconomyAccount account);

        /// <summary>
        ///     Creates an account.
        /// </summary>
        /// <param name="owner">The owner of the account.</param>
        /// <param name="name">The name of the account.</param>
        /// <param name="currency">The accounts currency.</param>
        /// <param name="account">The account instance if it was created; otherwise, <b>null</b>.</param>
        /// <returns><b>true</b> if account creation is supported and the account could be created; otherwise, <b>false</b>.</returns>
        Task<bool> CreateAccountAsync(IUser owner, string name, IEconomyCurrency currency, out IEconomyAccount account);

        /// <summary>
        ///     Deletes an account.
        /// </summary>
        /// <param name="account">The account to delete.</param>
        /// <returns><b>true</b> if the account could be deleted; otherwise, <b>false</b>.</returns>
        Task<bool> DeleteAccountAsync(IEconomyAccount account);

        /// <summary>
        ///     Gets a specific account.
        /// </summary>
        /// <param name="owner">The account owner.</param>
        /// <param name="accountName">The account name or null for the default account.</param>
        /// <returns>The requested account or null if it was not found.</returns>
        Task<IEconomyAccount> GetAccountAsync(IUser owner, string accountName = null);

        /// <summary>
        ///     Gets the accounts of the given user. Can return an empty set if no accounts were created yet.
        /// </summary>
        /// <param name="owner">The user whose accounts to get.</param>
        /// <returns>the accounts of the given user</returns>
        Task<IEnumerable<IEconomyAccount>> GetAccountsAsync(IUser owner);

        /// <summary>
        ///     Checks if the given user is supported by the economy provider.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns><b>True</b> if the user is supported; otherwise, <b>false</b>.</returns>
        bool SupportsUser(IUser user);

        /// <summary>
        ///     Gets the (total) balance for the given user.
        /// </summary>
        /// <param name="user">The identitiy whose balance to get.</param>
        /// <returns>The balance of the user. Might be negative.</returns>
        Task<decimal> GetBalanceAsync(IUser user);

        /// <summary>
        ///     Checks if the provider supports multiple accounts per user.
        /// </summary>
        bool SupportsMultipleAccounts { get; }
    }
}