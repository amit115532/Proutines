namespace Proutines.Generators
{
    internal class PingPongResetableYieldGenerator : ILerpGenerator
    {
        private int iterations;
        private readonly IYieldGenerator<double> generator;

        public PingPongResetableYieldGenerator(ILerpGenerator generator)
        {
            if (!generator.Resetable)
                throw new ResetNotSupportedException(generator);

            this.generator = generator;
        }

        public double GetValue()
        {
            return iterations % 2 == 0 ? generator.GetValue() : 1 - generator.GetValue();
        }

        public bool Yield()
        {
            if (!generator.Yield())
            {
                ++iterations;

                if (iterations == 2)
                {
                    // So next value wont be messed up
                    iterations = 1;
                    return false;
                }
                generator.Reset();
            }

            return true;
        }

        public bool Resetable => true;

        public void Reset()
        {
            iterations = 0;
            generator.Reset();
        }
    }
}