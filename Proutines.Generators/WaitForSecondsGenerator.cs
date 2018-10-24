namespace Proutines.Generators
{
    public class WaitForSecondsGenerator : IYieldGenerator<float>
    {
        private readonly IYieldGenerator<float> generator;
        private IYieldInstruction currentWaitInstruction;

        public WaitForSecondsGenerator(IYieldGenerator<float> generator)
        {
            this.generator = generator;
        }

        public float GetValue()
        {
            return generator.GetValue();
        }

        public bool Yield()
        {
            if (currentWaitInstruction == null)
            {
                var genYield = generator.Yield();
                if (!genYield)
                {
                    return false;
                }

                currentWaitInstruction = new WaitForSeconds(generator.GetValue());
            }
            else
            {
                if (!currentWaitInstruction.Yield())
                {
                    currentWaitInstruction = null;
                }
            }

            return true;
        }

        public bool Resetable => generator.Resetable;
        public void Reset()
        {
            currentWaitInstruction = null;
            generator.Reset();
        }
    }
    public class WaitForSecondsGeneratorInt : IYieldGenerator<int>
    {
        private readonly IYieldGenerator<int> generator;
        private IYieldInstruction currentWaitInstruction;

        public WaitForSecondsGeneratorInt(IYieldGenerator<int> generator)
        {
            this.generator = generator;
        }

        public int GetValue()
        {
            return generator.GetValue();
        }

        public bool Yield()
        {
            if (currentWaitInstruction == null)
            {
                var genYield = generator.Yield();
                if (!genYield)
                {
                    return false;
                }

                currentWaitInstruction = new WaitForSeconds(generator.GetValue());
            }
            else
            {
                if (!currentWaitInstruction.Yield())
                {
                    currentWaitInstruction = null;
                }
            }

            return true;
        }

        public bool Resetable => generator.Resetable;
        public void Reset()
        {
            currentWaitInstruction = null;
            generator.Reset();
        }
    }

    public class WaitForSecondsGeneratorDouble : IYieldGenerator<double>
    {
        private readonly IYieldGenerator<double> generator;
        private IYieldInstruction currentWaitInstruction;

        public WaitForSecondsGeneratorDouble(IYieldGenerator<double> generator)
        {
            this.generator = generator;

        }

        public double GetValue()
        {
            return generator.GetValue();
        }

        public bool Yield()
        {
            if (currentWaitInstruction == null)
            {
                var genYield = generator.Yield();
                if (!genYield)
                {
                    return false;
                }

                currentWaitInstruction = new WaitForSeconds((float)generator.GetValue());
            }
            else
            {
                if (!currentWaitInstruction.Yield())
                {
                    currentWaitInstruction = null;
                }
            }

            return true;
        }

        public bool Resetable => generator.Resetable;
        public void Reset()
        {
            currentWaitInstruction = null;
            generator.Reset();
        }
    }
}