using System.Collections.Generic;
using System.Data;

namespace Rocket.API.Providers.Database
{
    [ProviderDefinition]
    public interface IDatabaseProvider
    {
        IDbConnection Connection { get; }
        void Setup(string connectionString);
        ContextInitializationResult InitializeContext(DatabaseContext context);
        bool TableExists(string name);
        List<DatabaseContext> DatabaseContexts { get; }
    }
}