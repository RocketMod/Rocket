using System.Linq;
using System.Threading;
using Rocket.API.Scheduling;

namespace Rocket.Console.Scheduling
{
    public class AsyncThreadPool
    {
        private readonly SimpleTaskScheduler scheduler;

        public AsyncThreadPool(SimpleTaskScheduler scheduler)
        {
            this.scheduler = scheduler;
        }

        private readonly EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private Thread singleCallsThread;
        private Thread continousCallsThreads;

        public void Start()
        {
            singleCallsThread = new Thread(SingleThreadLoop);
            singleCallsThread.Start();

            continousCallsThreads = new Thread(ContinousThreadLoop);
            continousCallsThreads.Start();
        }

        private void ContinousThreadLoop()
        {
            while (true)
            {
                var cpy = scheduler.Tasks.ToList(); // we need a copy because the task list may be modified at runtime

                foreach (ITask task in cpy.Where(c => !c.IsFinished && !c.IsCancelled))
                {
                    if (task.Period == null)
                        if (task.ExecutionTarget != ExecutionTargetContext.EveryAsyncFrame
                            && task.ExecutionTarget != ExecutionTargetContext.EveryFrame
                            && task.ExecutionTarget != ExecutionTargetContext.EveryPhysicsUpdate)
                            continue;

                    scheduler.RunTask(task);
                }

                Thread.Sleep(20);
            }
        }

        private void SingleThreadLoop()
        {
            while (true)
            {
                waitHandle.WaitOne();
                var cpy = scheduler.Tasks.ToList(); // we need a copy because the task list may be modified at runtime

                foreach (ITask task in cpy.Where(c => !c.IsFinished && !c.IsCancelled))
                {
                    if (task.ExecutionTarget != ExecutionTargetContext.NextAsyncFrame &&
                        task.ExecutionTarget != ExecutionTargetContext.NextFrame &&
                        task.ExecutionTarget != ExecutionTargetContext.NextPhysicsUpdate &&
                        task.ExecutionTarget != ExecutionTargetContext.Async)
                        continue;

                    scheduler.RunTask(task);
                }
            }
        }
    }
}