namespace Proutines.Generators
{
    internal class LerpGenerator : ILerpGenerator
    {
        private double startTime;
        private bool isStart;
        private readonly double totalTime;
        private readonly LerpManipulator lerpManipulator;
        private double value;
        private readonly ITimeProvider timeProvider;

        public LerpGenerator(double totalTime, LerpManipulator lerpManipulator = null, ITimeProvider timeProvider = null)
        {
            this.timeProvider = timeProvider ?? TimeProviders.DefaultTimeProvider;
            this.totalTime = totalTime;
            this.lerpManipulator = lerpManipulator;

            isStart = true;
            value = 0;
        }

        public double GetValue()
        {
            if (lerpManipulator == null)
                return value;

            return lerpManipulator.Invoke(value);
        }

        public bool Yield()
        {
            var curTime = timeProvider.GetCurrentTime();

            if (isStart)
            {
                isStart = false;
                startTime = curTime;
            }

            var tempValue = (curTime - startTime) / totalTime;

            if (tempValue > 1)
                tempValue = 1;

            value = tempValue;

            return !(curTime - startTime > totalTime);
        }

        public bool Resetable => true;

        public void Reset()
        {
            value = 0;
            isStart = true;
        }
    }
}