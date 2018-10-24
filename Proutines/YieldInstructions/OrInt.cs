namespace Proutines
{
    internal class OrInt : YieldOperationBase<int>
    {
        private readonly Or baseImplementation;
        private readonly int aResult;
        private readonly int bResult;

        public OrInt(IYieldInstruction a, IYieldInstruction b, int aResult, int bResult)
        {
            baseImplementation = new Or(a, b);
            this.aResult = aResult;
            this.bResult = bResult;
        }

        public override bool Resetable => baseImplementation.Resetable;

        protected override void Reset()
        {
            ((IYieldInstruction)baseImplementation).Reset();
        }

        protected override YieldValue WillYield()
        {
            var isDone = !baseImplementation.Yield();

            if (!isDone)
            {
                return YieldValue.Continue();
            }

            var value = baseImplementation.Value == OrResult.First ? aResult : bResult;
            return YieldValue.Finish(value);
        }
    }
}