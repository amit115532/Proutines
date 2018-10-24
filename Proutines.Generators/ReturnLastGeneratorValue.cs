namespace Proutines.Generators
{
    internal class ReturnLastGeneratorValue<T> : YieldOperationBase<T>
    {
        private readonly IYieldGenerator<T> generator;

        public ReturnLastGeneratorValue(IYieldGenerator<T> generator)
        {
            this.generator = generator;
        }

        public T GetValue()
        {
            return generator.GetValue();
        }

        protected override YieldValue WillYield()
        {
            if (generator.Yield())
            {
                return YieldValue.Continue();
            }

            return YieldValue.Finish(generator.GetValue());
        }

        protected override void Reset()
        {
            generator.Reset();
        }

        public override bool Resetable => generator.Resetable;
    }
}