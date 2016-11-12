using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Logger = Rocket.API.Logging.Logger;

namespace Rocket.Core.IPC
{

    public class LongPollingEvent<T>
    {
        public AutoResetEvent ResetEvent { get; private set; } = new AutoResetEvent(false);
        public Queue<T> Queue { get; private set; } = new Queue<T>();

        public void Invoke(T item)
        {
            lock (Queue)
            {
                Queue.Enqueue(item);
            }
            ResetEvent.Set();
        }

        public T Request()
        {
            try
            {
                lock (Queue)
                {
                    if (Queue.Count != 0)
                        return Queue.Dequeue();
                }

                ResetEvent.WaitOne(30000);

                lock (Queue)
                {
                    if (Queue.Count != 0)
                        return Queue.Dequeue();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return default(T);
        }

        public Queue<T> RequestAll()
        {
            try
            {
                lock (Queue)
                {
                    if (Queue.Count != 0)
                    {
                        Queue<T> result = new Queue<T>(Queue);
                        Queue.Clear();
                        return result;
                    }
                }

                ResetEvent.WaitOne(30000);

                lock (Queue)
                {
                    if (Queue.Count != 0)
                    {
                        Queue<T> result = new Queue<T>(Queue);
                        Queue.Clear();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return new Queue<T>();
        }
    }

    public class LongPollingEvent
    {
        public AutoResetEvent ResetEvent { get; private set; } = new AutoResetEvent(false);

        public void Invoke()
        {
            ResetEvent.Set();
        }
        public bool Request()
        {
            try
            {
                ResetEvent.WaitOne(30000);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }
    }

}
