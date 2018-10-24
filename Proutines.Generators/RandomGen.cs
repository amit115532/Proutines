using System;

namespace Proutines.Generators
{
    internal class RandomGen : IYieldGenerator<int>
    {
        private readonly int min;
        private readonly int max;
        private readonly Random rand;

        private int current;

        public RandomGen(int min, int max, int? seed = null)
        {
            rand = seed != null ? new Random(seed.Value) : new Random();

            this.max = max;
            this.min = min;
        }

        public int GetValue()
        {
            return current;
        }

        public bool Yield()
        {
            current = rand.Next(min, max);

            return false;
        }

        public bool Resetable => true;
        public void Reset()
        {
            // Left blank intentionally
        }
    }

    internal class RandomGenDouble : IYieldGenerator<double>
    {
        private readonly double min;
        private readonly double max;
        private readonly bool hasMinMax;
        private readonly Random rand;

        private double current;

        public RandomGenDouble(double min, double max, int? seed = null)
        {
            rand = seed != null ? new Random(seed.Value) : new Random();

            this.max = max;
            this.min = min;
            hasMinMax = true;
        }
        public RandomGenDouble(int? seed = null)
        {
            rand = seed != null ? new Random(seed.Value) : new Random();
            hasMinMax = false;
        }

        public double GetValue()
        {
            return current;
        }

        public bool Yield()
        {
            if (!hasMinMax)
            {
                current = rand.NextDouble();
            }
            else
            {
                current = rand.NextDouble() * (max - min) + min;
            }

            return false;
        }

        public bool Resetable => true;
        public void Reset()
        {
            // Left blank intentionally
        }
    }
}