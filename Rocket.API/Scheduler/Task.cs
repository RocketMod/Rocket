using System;

namespace Rocket.API.Scheduler
{
    public class Task
    {
        public ILifecycleObject Owner { get; }

        public Action Action { get; }

        public bool IsCancelled { get; }

        public bool IsAsync { get; set; }

        public bool IsRunning { get; }

        public bool IsContinuous { get; }
    }
}