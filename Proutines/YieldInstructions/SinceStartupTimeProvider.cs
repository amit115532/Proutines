using System;

namespace Proutines
{
    public class SinceStartupTimeProvider : ITimeProvider
    {
        private readonly int startTime = Environment.TickCount;

        public double GetCurrentTime()
        {
            var time = (Environment.TickCount - startTime) * 1E-03;
            return time;
        }
    }
}