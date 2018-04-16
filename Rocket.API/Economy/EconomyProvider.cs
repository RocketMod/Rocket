using Rocket.API.Commands;

namespace Rocket.API.Economy
{
    public interface EconomyProvider
    {
        decimal GetBalance(ICommandCaller caller);

        void AddBalance(ICommandCaller caller, decimal amount, string reason = null);

        bool RemoveBalance(ICommandCaller caller, decimal amount, string reason = null);

        void SetBalance(ICommandCaller caller, decimal amount);
    }
}