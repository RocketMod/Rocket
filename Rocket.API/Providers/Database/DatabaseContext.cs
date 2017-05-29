using System.Data.Linq;
using Rocket.API.Providers.Plugins;
using Rocket.API.Utils.Debugging;

namespace Rocket.API.Providers.Database
{
    public abstract class DatabaseContext : DataContext
    {
        public IRocketPlugin Plugin { get; }
        public IDatabaseProvider Provider { get; }

        internal DatabaseContext(IDatabaseProvider provider) : base(provider.Connection)
        {
            Provider = provider;
        }

        protected DatabaseContext(IRocketPlugin plugin, IDatabaseProvider provider) : base(provider.Connection)
        {
            Assert.NotNull(plugin, nameof(plugin));
            Assert.NotNull(provider, nameof(provider));
            Plugin = plugin;
            Provider = provider;
        }

        public abstract void OnDatabaseCreated();
    }
}