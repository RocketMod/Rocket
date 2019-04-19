using System;
using Rocket.API.DependencyInjection;

namespace Rocket.API.Scheduling
{
    public interface ITaskRunner: IProxyableService
    {
        bool SupportsTask(IScheduledTask task);

        void Run(IScheduledTask task, Action<Exception> completedAction);
    }
}