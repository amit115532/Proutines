namespace Proutines
{
    internal class Or : YieldOperationBase<OrResult>
    {
        private readonly IYieldInstruction a;
        private readonly IYieldInstruction b;

        public Or(IYieldInstruction a, IYieldInstruction b)
        {
            this.a = a;
            this.b = b;
        }

        public override bool Resetable => a.Resetable && b.Resetable;

        protected override void Reset()
        {
            a.Reset();
            b.Reset();
        }

        protected override YieldValue WillYield()
        {
            if (!a.Yield())
            {
                return YieldValue.Finish(OrResult.First);
            }

            if (!b.Yield())
            {
                return YieldValue.Finish(OrResult.Second);
            }

            return YieldValue.Continue();
        }
    }
}