using System.Threading.Tasks;
using Rocket.API.DependencyInjection;

namespace Rocket.Core.Configuration
{
    /// <summary>
    ///     Provides general settings for RocketMod and its own services.
    /// </summary>
    public interface IRocketConfigurationProvider: IService
    {
        /// <summary>
        ///     The RocketMod settings.
        /// </summary>
        RocketConfiguration Configuration { get; }

        /// <summary>
        ///     Loads the settings.
        /// </summary>
        Task LoadAsync();

        /// <summary>
        ///     Reloads the settings.
        /// </summary>
        Task ReloadAsync();

        /// <summary>
        ///     Saves the settings.
        /// </summary>
        Task SaveAsync();
    }
}