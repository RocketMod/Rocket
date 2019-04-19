using System;
using System.Linq;
using Rocket.API.DependencyInjection;
using Rocket.API.Scheduling;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Scheduling
{
    public class TaskRunnerProxy : ServiceProxy<ITaskRunner>, ITaskRunner
    {
        public TaskRunnerProxy(IDependencyContainer container) : base(container) { }
        public string ServiceName { get; } = "TaskRunner Proxy";
        public bool SupportsTask(IScheduledTask task) => ProxiedServices.Any(d => d.SupportsTask(task));

        public void Run(IScheduledTask task, Action<Exception> completedAction)
        {
            var runner = ProxiedServices.FirstOrDefault(d => d.SupportsTask(task));
            if (runner == null)
            {
                throw new NotSupportedException($"The task \"{task.Name}\" could not be run because no compatible task runner was found!");
            }

            runner.Run(task, completedAction);
        }
    }
}