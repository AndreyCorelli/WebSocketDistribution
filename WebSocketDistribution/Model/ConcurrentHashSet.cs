using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WebSocketDistribution.Model
{
    public class ConcurrentHashSet<T> where T : class
    {
        private readonly HashSet<T> items = new HashSet<T>();

        private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        //private const int LockTimeout = 1000;

        public List<T> GetItems()
        {
            locker.EnterReadLock();
            try
            {
                return items.ToList();
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public void AddOrReplace(T item)
        {
            locker.EnterWriteLock();
            try
            {
                items.Add(item);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        public void TryRemove(T item)
        {
            locker.EnterWriteLock();
            try
            {
                items.Remove(item);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }
    }
}
