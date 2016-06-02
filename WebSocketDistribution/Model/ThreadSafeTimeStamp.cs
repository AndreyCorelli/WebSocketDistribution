using System;
using System.Threading;

namespace WebSocketDistribution.Model
{
    public class ThreadSafeTimeStamp
    {
        private long lastHit;

        public void Touch()
        {
            Interlocked.Exchange(ref lastHit, DateTime.Now.ToBinary());
        }

        public void SetTime(DateTime time)
        {
            Interlocked.Exchange(ref lastHit, time.ToBinary());
        }

        public DateTime GetLastHit()
        {
            var lastHitS = Interlocked.CompareExchange(ref lastHit, 0, 0);
            var time = DateTime.FromBinary(lastHitS);
            return time;
        }

        public DateTime? GetLastHitIfHitted()
        {
            var lastHitS = Interlocked.CompareExchange(ref lastHit, 0, 0);
            return lastHitS == 0 ? (DateTime?)null : DateTime.FromBinary(lastHitS);
        }

        public void ResetHit()
        {
            Interlocked.Exchange(ref lastHit, 0);
        }

        public void Copy(ThreadSafeTimeStamp source)
        {
            var sourceStamp = Interlocked.CompareExchange(ref source.lastHit, 0, 0);
            Interlocked.Exchange(ref lastHit, sourceStamp);
        }

        public void CopyCheckNull(ThreadSafeTimeStamp source)
        {
            if (source == null) return;
            var sourceStamp = Interlocked.CompareExchange(ref source.lastHit, 0, 0);
            Interlocked.Exchange(ref lastHit, sourceStamp);
        }
    }
}
