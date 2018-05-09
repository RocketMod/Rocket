namespace Rocket.API.Economy
{
    /// <summary>
    ///     An economy currency.
    /// </summary>
    public interface IEconomyCurrency
    {
        /// <summary>
        ///     The currency name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Exchanges balance from a difference currency.
        /// </summary>
        /// <param name="amount">The amount to exchange.</param>
        /// <param name="targetCurrency">The currency to exchange from.</param>
        /// <exception>When the target currency is not supported.</exception>
        /// <returns></returns>
        decimal Exchange(double amount, IEconomyCurrency targetCurrency);

        /// <summary>
        ///     Checks if this cucrrency can be exchanged with the other one.
        /// </summary>
        /// <param name="currency">The currency to check.</param>
        /// <returns><b>true</b> if balance can be exchanged from the other currency; otherwise; <b>false</b>.</returns>
        bool CanExchange(IEconomyCurrency currency);
    }
}