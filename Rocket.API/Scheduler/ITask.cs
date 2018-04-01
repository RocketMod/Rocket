using System;

namespace Rocket.API.Scheduler
{
    public interface ITask
    {
        ILifecycleObject Owner { get; }

        Action Action { get; }

        bool IsCancelled { get; }

        ExecutionTargetContext ExecutionTarget { get; }

        bool IsFinished { get; }
        
        void Cancel();
    }
}