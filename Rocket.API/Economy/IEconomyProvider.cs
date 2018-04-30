using System;
using Rocket.API.Commands;

namespace Rocket.API.Economy
{
    /// <summary>
    ///     The service responsible for managing economy and currencies.
    /// </summary>
    public interface IEconomyProvider
    {
        /// <summary>
        ///     Get a command callers balance.
        /// </summary>
        /// <param name="caller">the account owner.</param>
        /// <returns>the balance of the command caller.</returns>
        decimal GetBalance(ICommandCaller caller);

        /// <summary>
        ///     Adds balance to the command callers account.
        /// </summary>
        /// <param name="caller">the account owner.</param>
        /// <param name="amount">the amount to add. Should not be negative.</param>
        /// <param name="reason">the reason of transaction.</param>
        void AddBalance(ICommandCaller caller, decimal amount, string reason = null);

        /// <summary>
        ///     Removes balance from the command callers account.
        /// </summary>
        /// <param name="caller">the account owner.</param>
        /// <param name="amount">the amount to remove. Should not be negative.</param>
        /// <param name="reason">the reason of transaction.</param>
        /// <returns><b>true</b> if the balance could be removed; otherwise, <b>false</b>.</returns>
        bool RemoveBalance(ICommandCaller caller, decimal amount, string reason = null);

        /// <summary>
        ///     Sets the balance of the command callers account.
        /// </summary>
        /// <param name="caller">The account owner.</param>
        /// <param name="amount">The amount to set. See <see cref="SupportsNegativeBalance"/>.</param>
        void SetBalance(ICommandCaller caller, decimal amount);

        /// <summary>
        ///     Defines if the account of the command caller can have negative balance.
        /// </summary>
        /// <param name="caller"></param>
        /// <returns><b>true</b> if the account can have negative balance; otherwise, <b>false</b>.</returns>
        bool SupportsNegativeBalance(ICommandCaller caller);

        /// <summary>
        ///     Defines if this provider supports the given command caller.
        /// </summary>
        /// <param name="commandCaller">The command caller to check.</param>
        /// <returns><b>true</b> if the command caller is supported; otherwise, <b>false</b>.</returns>
        bool SupportsCaller(Type commandCaller);

        /// <summary>
        ///     The current currency name.
        /// </summary>
        string CurrencyName { get; }
    }
}