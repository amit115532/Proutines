namespace Proutines.Generators
{
    internal class TimeGen : IYieldGenerator<double>
    {
        private readonly WaitForSeconds waitForSeconds;

        private double startTime;
        private readonly bool fromNow;
        private readonly ITimeProvider timeProvider;

        private bool isStart;

        public TimeGen(double amountOfSeconds, bool fromNow, ITimeProvider timeProvider)
        {
            isStart = true;
            this.fromNow = fromNow;
            this.timeProvider = timeProvider;

            waitForSeconds = new WaitForSeconds(amountOfSeconds, timeProvider);
        }

        public TimeGen(bool fromNow, ITimeProvider timeProvider)
        {
            isStart = true;
            this.fromNow = fromNow;
            this.timeProvider = timeProvider;
            waitForSeconds = null;
        }

        public double GetValue()
        {
            return timeProvider.GetCurrentTime() - startTime;
        }

        public bool Yield()
        {
            if (isStart)
            {
                startTime = fromNow ? timeProvider.GetCurrentTime() : 0;
                isStart = false;
            }

            return waitForSeconds == null || waitForSeconds.Yield();
        }

        public bool Resetable => true;

        public void Reset()
        {
            if (fromNow)
            {
                startTime = timeProvider.GetCurrentTime();
            }

            waitForSeconds?.Reset();
        }
    }
}