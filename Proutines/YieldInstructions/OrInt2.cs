namespace Proutines
{
    internal class OrInt2 : YieldOperationBase<int>
    {
        private readonly Or baseImplementation;
        private readonly int bResult;
        private readonly IYieldOperation<int> a;

        public OrInt2(IYieldOperation<int> a, IYieldInstruction b, int bResult)
        {
            this.a = a;
            baseImplementation = new Or(a, b);
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

            var value = baseImplementation.Value == OrResult.First ? a.Value : bResult;
            return YieldValue.Finish(value);
        }
    }
}