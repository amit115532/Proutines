namespace Proutines.Generators
{
    internal class RangeGen : IYieldGenerator<int>
    {
        private readonly int maxValue;
        private readonly bool hasMaxValue;
        private readonly int? fromValue;

        private int currentValue;

        public RangeGen(int fromValue, int length)
        {
            this.fromValue = fromValue;
            maxValue = fromValue + length - 1;
            currentValue = fromValue - 1;
            hasMaxValue = true;
        }

        public RangeGen(int length)
        {
            fromValue = null;
            maxValue = length - 1;
            currentValue = -1;
            hasMaxValue = true;
        }

        public RangeGen()
        {
            fromValue = null;
            hasMaxValue = false;
            currentValue = -1;
        }

        public int GetValue()
        {
            return currentValue;
        }

        public bool Yield()
        {
            ++currentValue;

            if (hasMaxValue && currentValue >= maxValue)
            {
                return false;
            }

            return true;
        }

        public bool Resetable => true;
        public void Reset()
        {
            if (fromValue == null)
                currentValue = -1;
            else
                currentValue = fromValue.Value - 1;
        }
    }
}