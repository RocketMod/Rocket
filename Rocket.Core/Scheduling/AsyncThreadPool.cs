using System.Linq;
using System.Threading;
using Rocket.API.Scheduling;

namespace Rocket.Core.Scheduling
{
    public class AsyncThreadPool
    {
        private readonly DefaultTaskScheduler taskScheduler;
        public EventWaitHandle EventWaitHandle { get; }
        public AsyncThreadPool(DefaultTaskScheduler scheduler)
        {
            taskScheduler = scheduler;
            EventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        }

        private Thread taskThread;
        private bool _run;

        public void Start()
        {
            _run = true;
            taskThread = new Thread(ContinousThreadLoop);
            taskThread.Start();
        }

        public void Stop()
        {
            _run = false;
            taskThread = null;
        }

        private void ContinousThreadLoop()
        {
            while (_run)
            {
                var cpy = taskScheduler.Tasks.Where(c => !c.IsFinished && !c.IsCancelled).ToList(); // we need a copy because the task list may be modified at runtime

                foreach (IScheduledTask task in cpy)
                {
                    if (task.Period == null || (task.Period != null && (task.ExecutionTarget != ExecutionTargetContext.Async && task.ExecutionTarget != ExecutionTargetContext.Sync)))
                        if (task.ExecutionTarget != ExecutionTargetContext.EveryAsyncFrame && task.ExecutionTarget != ExecutionTargetContext.EveryFrame)
                            continue;

                    taskScheduler.RunTask(task);
                }

                foreach (IScheduledTask task in cpy)
                {
                    if (task.ExecutionTarget != ExecutionTargetContext.NextAsyncFrame && task.ExecutionTarget != ExecutionTargetContext.NextFrame &&
                        task.ExecutionTarget != ExecutionTargetContext.Async && task.ExecutionTarget != ExecutionTargetContext.Sync)
                        continue;

                    taskScheduler.RunTask(task);
                }

                if (cpy.Count == 0)
                {
                    EventWaitHandle.WaitOne();
                }

                Thread.Sleep(20);
            }
        }
    }
}