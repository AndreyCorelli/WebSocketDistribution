using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WebSocketDistribution.Model
{
    public class MessageStorage
    {
        private readonly List<string> messageList = new List<string>();

        private readonly ReaderWriterLockSlim listLocker = new ReaderWriterLockSlim();

        public void StoreMessages(List<string> messsagesToDistribute)
        {
            if (messsagesToDistribute.Count == 0) return;
            // определить тренд
            if (messsagesToDistribute.Count == 0) return;
            listLocker.EnterWriteLock();
            try
            {
                messageList.AddRange(messsagesToDistribute);
            }
            finally
            {
                listLocker.ExitWriteLock();
            }
        }

        public List<string> FlushMessages()
        {
            listLocker.EnterWriteLock();
            try
            {
                var list = messageList.ToList();
                messageList.Clear();
                return list;
            }
            finally
            {
                listLocker.ExitWriteLock();
            }
        }

        public List<string> ReadMessages()
        {
            listLocker.EnterReadLock();
            try
            {
                var list = messageList.ToList();
                return list;
            }
            finally
            {
                listLocker.ExitReadLock();
            }
        }
    }
}
