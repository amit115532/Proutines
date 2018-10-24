namespace Proutines
{
    public class OrValue<T> : YieldOperationBase<T>
    {
        private readonly IYieldOperation<T> a;
        private readonly IYieldOperation<T> b;
        private readonly Or or;

        public OrValue(IYieldOperation<T> a, IYieldOperation<T> b)
        {
            this.a = a;
            this.b = b;
            or = new Or(a, b);
        }

        public override bool Resetable => a.Resetable && b.Resetable;

        protected override void Reset()
        {
            a.Reset();
            b.Reset();
        }

        protected override YieldValue WillYield()
        {
            if (or.Yield())
            {
                return YieldValue.Continue();
            }

            var value = or.Value == OrResult.First ? a.Value : b.Value;
            return YieldValue.Finish(value);
        }
    }
}