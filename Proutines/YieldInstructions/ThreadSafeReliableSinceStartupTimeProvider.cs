using System;
using System.Threading;

namespace Proutines
{
    public class ThreadSafeReliableSinceStartupTimeProvider : ITimeProvider
    {
        private readonly uint startTime;
        private volatile uint wrapCount;
        private volatile uint lastWrapTime;
        private readonly object locker = new object();

        public ThreadSafeReliableSinceStartupTimeProvider()
        {
            startTime = unchecked((uint)Environment.TickCount);
            lastWrapTime = unchecked((uint)Environment.TickCount - startTime);
        }

        public long GetElapsedTime()
        {
            var elapsed = unchecked((uint)Environment.TickCount - startTime);

            if (elapsed < lastWrapTime)
            {
                if (Monitor.TryEnter(locker))
                {
                    wrapCount++;
                    lastWrapTime = elapsed;
                }
                else
                {
                    Monitor.Wait(locker);
                }
            }

            return ((long)wrapCount << 32) + elapsed;
        }

        public double GetCurrentTime()
        {
            var time = (GetElapsedTime() * 1E-03);
            return time;
        }
    }
}