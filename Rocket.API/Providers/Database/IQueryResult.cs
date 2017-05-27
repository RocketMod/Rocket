using System;

namespace Rocket.API.Providers.Database
{
    public interface IQueryResult
    {
        QueryState State { get; }
        Exception Exception { get; }
    }
}