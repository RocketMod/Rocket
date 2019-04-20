using System;
using System.Linq;
using System.Threading;
using Rocket.API.Scheduling;

namespace Rocket.Core.Scheduling
{
    public class AsyncTaskRunner : IDisposable
    {
        private readonly ITaskScheduler taskScheduler;
        private readonly ExecutionTargetSide executionSide;
        protected EventWaitHandle EventWaitHandle { get; set; }

        public virtual bool IsRunning => isRunning;

        private volatile bool isRunning;
        private Thread taskThread;

        public AsyncTaskRunner(ITaskScheduler taskScheduler, ExecutionTargetSide executionSide)
        {
            this.taskScheduler = taskScheduler;
            this.executionSide = executionSide;

            EventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        }

        public virtual void Start()
        {
            if (isRunning)
            {
                return;
            }

            isRunning = true;
            taskThread = new Thread(ThreadLoop);
            taskThread.Start();
        }

        public virtual void Stop()
        {
            if (!isRunning)
            {
                return;
            }

            isRunning = false;
            taskThread = null;
        }

        public virtual void NotifyTaskScheduled()
        {
            EventWaitHandle.Set();
        }

        protected virtual void ThreadLoop()
        {
            while (isRunning)
            {
                taskScheduler.RunFrameUpdate(executionSide);

                var cpy = taskScheduler.Tasks.Where(c => !c.IsFinished).ToList(); // we need a copy because the task list may be modified

                if (cpy.Count == 0)
                {
                    EventWaitHandle.WaitOne();
                }

                Thread.Sleep(20);
            }
        }

        public virtual void Dispose()
        {
            if (isRunning)
            {
                Stop();
            }

            EventWaitHandle?.Dispose();
        }
    }
}