using System;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Permissions;

namespace Rocket.API.Economy
{
    /// <summary>
    ///     The service responsible for managing economy and currencies.
    /// </summary>
    public interface IEconomyProvider
    {
        /// <summary>
        ///     Adds balance to the command callers account.
        /// </summary>
        /// <param name="owner">The account owner.</param>
        /// <param name="amount">The amount to add. Should not be negative.</param>
        /// <param name="reason">The reason of the transaction.</param>
        void AddBalance(IIdentifiable owner, decimal amount, string reason = null);

        /// <summary>
        ///     Adds balance to the given account.
        /// </summary>
        /// <param name="amount">The amount to add. Should not be negative.</param>
        /// <param name="account">The account to add balance to.</param>
        /// <param name="reason">The reason of the transaction.</param>
        void AddBalance(IEconomyAccount account, decimal amount, string reason = null);

        /// <summary>
        ///     Removes balance from the command callers account.
        /// </summary>
        /// <param name="caller">The account owner.</param>
        /// <param name="amount">The amount to remove. Should not be negative.</param>
        /// <param name="reason">The reason of the transaction.</param>
        /// <seealso cref="SupportsNegativeBalance(Rocket.API.Commands.ICommandCaller)"/>
        /// <returns><b>true</b> if the balance could be removed; otherwise, <b>false</b>.</returns>
        bool RemoveBalance(ICommandCaller caller, decimal amount, string reason = null);

        /// <summary>
        ///     Removes balance from the command callers account based on a specific currency.
        /// </summary>
        /// <param name="amount">The amount to remove. Should not be negative.</param>
        /// <param name="reason">The reason of the transaction.</param>
        /// <param name="account">The account to remove balance from.</param>
        /// <seealso cref="SupportsNegativeBalance(Rocket.API.Commands.ICommandCaller)"/>
        /// <returns><b>true</b> if the balance could be removed; otherwise, <b>false</b>.</returns>
        bool RemoveBalance(IEconomyAccount account, decimal amount, string reason = null);

        /// <summary>
        ///     Sets the balance of the command callers account.
        /// </summary>
        /// <param name="caller">The account owner.</param>
        /// <param name="amount">The amount to set. See <see cref="SupportsNegativeBalance(ICommandCaller)"/>.</param>
        void SetBalance(ICommandCaller caller, decimal amount);

        /// <summary>
        ///     Sets the balance of the command callers account in a specific currency.
        /// </summary>
        /// <param name="amount">The amount to set. See <see cref="SupportsNegativeBalance(ICommandCaller)"/>.</param>
        /// <param name="account">The account to set the balance of.</param>
        void SetBalance(IEconomyAccount account, decimal amount);

        /// <summary>
        ///     Checks if the account of the command caller can have negative balance.
        /// </summary>
        /// <param name="caller"></param>
        /// <returns><b>true</b> if the account can have negative balance; otherwise, <b>false</b>.</returns>
        bool SupportsNegativeBalance(ICommandCaller caller);

        /// <summary>
        ///     Checks if the account of the command caller can have negative balance.
        /// </summary>
        /// <param name="account">Checks if the given account has access.</param>
        /// <returns><b>true</b> if the account can have negative balance; otherwise, <b>false</b>.</returns>
        bool SupportsNegativeBalance(IEconomyAccount account);

        /// <summary>
        ///     Defines if this provider supports the given user.
        /// </summary>
        /// <param name="user">The <see cref="IIdentifiable"/> to check.</param>
        /// <returns><b>true</b> if the command caller is supported; otherwise, <b>false</b>.</returns>
        bool SupportsUser(Type user);

        /// <summary>
        ///     Creates an account.
        /// </summary>
        /// <param name="owner">The owner of the account.</param>
        /// <param name="name">The name of the account.</param>
        /// <param name="account">The account instance if it was created; otherwise, <b>null</b>.</param>
        /// <exception>If the account creation failed because of an internal error.</exception>
        /// <returns><b>true</b> if account creation is supported and the account didn't exist already, and was created; otherwise, <b>false</b>.</returns>
        bool CreateAccount(IIdentifiable owner, string name, out IEconomyAccount account);

        /// <summary>
        ///     Creates an account.
        /// </summary>
        /// <param name="owner">The owner of the account.</param>
        /// <param name="name">The name of the account.</param>
        /// <param name="currency">The accounts currency.</param>
        /// <param name="account">The account instance if it was created; otherwise, <b>null</b>.</param>
        /// <returns><b>true</b> if account creation is supported and the account could be created; otherwise, <b>false</b>.</returns>
        bool CreateAccount(IIdentifiable owner, string name, IEconomyCurrrency currency, out IEconomyAccount account);

        /// <summary>
        ///     Deletes an account.
        /// </summary>
        /// <param name="account">The account to delete.</param>
        /// <returns><b>true</b> if the account could be deleted; otherwise, <b>false</b>.</returns>
        bool DeleteAccount(IEconomyAccount account);

        /// <summary>
        ///     All currencies. Contains at least one element.
        /// </summary>
        IEnumerable<IEconomyCurrrency> Currencies { get; }

        /// <summary>
        ///     Gets a specific account.
        /// </summary>
        /// <param name="owner">The account owner.</param>
        /// <param name="acccountName">The account name or null for the default account.</param>
        /// <returns>The requested account or null if it was not found.</returns>
        IEconomyAccount GetAccount(IIdentifiable owner, string acccountName = null);

        /// <summary>
        ///     Gets the accounts of the given user. Can return an empty set if no accounts were created yet.
        /// </summary>
        /// <param name="owner">The user whose accounts to get.</param>
        /// <returns>the accounts of the given user</returns>
        IEnumerable<IEconomyAccount> GetAccounts(IIdentifiable owner);

        /// <summary>
        ///     The default currency. Will never return null.
        /// </summary>
        IEconomyCurrrency DefaultCurrrency { get; }
    }
}