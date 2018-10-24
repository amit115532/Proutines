namespace Proutines.Generators
{
    internal class LoopForeverYieldGenerator<T> : IYieldGenerator<T>
    {
        private readonly IYieldGenerator<T> yieldGenerator;

        public LoopForeverYieldGenerator(IYieldGenerator<T> yieldGenerator)
        {
            if (!yieldGenerator.Resetable)
            {
                throw new ResetNotSupportedException(yieldGenerator);
            }

            this.yieldGenerator = yieldGenerator;
        }

        public T GetValue()
        {
            return yieldGenerator.GetValue();
        }

        public bool Yield()
        {
            if (!yieldGenerator.Yield())
            {
                yieldGenerator.Reset();
            }

            return true;
        }

        public bool Resetable => true;

        public void Reset()
        {
            yieldGenerator.Reset();
        }
    }
}