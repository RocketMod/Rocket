using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Rocket.API.Utils
{
    public class TaskDispatcher : MonoBehaviour
    {
        public static TaskDispatcher Instance { get; private set; }
        private static readonly List<Action> QueuedMainActions = new List<Action>();
        private static readonly List<Action> QueuedMainFixedActions = new List<Action>();
        private static readonly List<Action> QueuedAsyncActions = new List<Action>();
        private static int _mainThreadId;
        private Thread _thread;

        protected void Awake()
        {
            if (Instance != null)
            {
                Logging.Logger.Warn("There can only be one instance of TaskDispatcher");
                Destroy(this);
                return;
            }

            Instance = this;
        }

        protected void Start()
        {
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
            _thread = new Thread(AsyncUpdate);
            _thread.Start();
        }

        public bool IsCurrentThreadMainThread => Thread.CurrentThread.ManagedThreadId == _mainThreadId;

        /// <summary>
        /// Calls the action on the main thread on the next Update()
        /// </summary>
        /// <param name="action">The action to queue for the next Update() call</param>
        public void QueueUpdate(Action action)
        {
            lock (QueuedMainActions)
            {
                QueuedMainActions.Add(action);
            }
        }
        /// <summary>
        /// Runs the given action on the main thread. Will invoke immediately if the current thread is the main thread, will queue if not.
        /// </summary>
        /// <param name="action">The action to run on the main thread</param>
        public void RunOnMainThread(Action action)
        {
            if (IsCurrentThreadMainThread) action.Invoke();
            else QueueUpdate(action);
        }

        /// <summary>
        /// Calls the action on the main thread on the next FixedUpdate()
        /// </summary>
        /// <param name="action">The action to queue for thenext FixedUpdate() call</param>
        public void QueueUpdateFixed(Action action)
        {
            lock (QueuedMainFixedActions)
            {
                QueuedMainFixedActions.Add(action);
            }
        }

        /// <summary>
        /// Calls the action on the next async thread Update() call
        /// </summary>
        /// <param name="action">The action to call async</param>
        public void QueueAsync(Action action)
        {
            lock (QueuedAsyncActions)
            {
                QueuedAsyncActions.Add(action);
            }
        }

        private static int numThreads;
        public static void RunAsync(Action a)
        {
            while (numThreads >= 8)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, a);
        }

        private static void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch (Exception ex)
            {
                Logging.Logger.Error("Error while running action", ex);
            }
            finally
            {
                Interlocked.Decrement(ref numThreads);
            }
        }

        protected void Update()
        {
            lock (QueuedMainActions)
            {
                if (QueuedMainActions.Count == 0)
                {
                    return;
                }

                foreach (Action action in QueuedMainActions)
                {
                    action.Invoke();
                }
                QueuedMainActions.Clear();
            }
        }

        protected void FixedUpdate()
        {
            lock (QueuedMainFixedActions)
            {
                if (QueuedMainFixedActions.Count == 0)
                {
                    return;
                }
                foreach (Action action in QueuedMainFixedActions)
                {
                    action.Invoke();
                }
                QueuedMainFixedActions.Clear();
            }
        }

        private void AsyncUpdate()
        {
            lock (QueuedAsyncActions)
            {
                if (QueuedAsyncActions.Count == 0)
                {
                    return;
                }

                foreach (Action action in QueuedAsyncActions)
                {
                    action.Invoke();
                }
                QueuedAsyncActions.Clear();
            }
            Thread.Sleep(10);
        }

        public void Shutdown()
        {
            lock (QueuedAsyncActions)
            {
                QueuedAsyncActions?.Clear();
            }

            lock (QueuedMainActions)
            {
                QueuedAsyncActions?.Clear();
            }

            lock (QueuedAsyncActions)
            {
                QueuedAsyncActions?.Clear();
            }

            _thread?.Interrupt();
            _thread = new Thread(AsyncUpdate);
            _thread.Start();
        }
    }
}