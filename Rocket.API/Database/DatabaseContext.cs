using System.Data.Linq;
using Rocket.API.Utils.Debugging;
using Rocket.API.Plugins;

namespace Rocket.API.Providers.Database
{
    public abstract class DatabaseContext : DataContext
    {
        public IPlugin Plugin { get; }
        public IDatabaseProvider Provider { get; }

        internal DatabaseContext(IDatabaseProvider provider) : base(provider.Connection)
        {
            Provider = provider;
        }

        protected DatabaseContext(IPlugin plugin, IDatabaseProvider provider) : base(provider.Connection)
        {
            Assert.NotNull(plugin, nameof(plugin));
            Assert.NotNull(provider, nameof(provider));
            Plugin = plugin;
            Provider = provider;
        }

        public abstract void OnDatabaseCreated();
    }
}