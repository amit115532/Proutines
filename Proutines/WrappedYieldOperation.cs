namespace Proutines
{
    public class WrappedYieldOperation<T> : YieldOperationBase<T>
    {
        private readonly IYieldOperation<T> yieldOperation;

        public WrappedYieldOperation(IYieldOperation<T> yieldOperation)
        {
            this.yieldOperation = yieldOperation;
        }

        public override bool Resetable => false;

        protected override void Reset()
        {
            throw new ResetNotSupportedException(this);
        }

        protected override YieldValue WillYield()
        {
            var isDone = yieldOperation.IsDone;

            if (isDone)
            {
                return YieldValue.Finish(yieldOperation.Value);
            }

            return YieldValue.Continue();
        }
    }
}