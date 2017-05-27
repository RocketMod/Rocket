using System;
using Rocket.API.Providers.Database;

namespace Rocket.Core.Providers.Database
{
    public class AdoNetQueryResult : IQueryResult
    {
        public AdoNetQueryResult(QueryState state)
        {
            State = state;
        }

        public QueryState State { get; }
        public Exception Exception { get; set; }
    }
}