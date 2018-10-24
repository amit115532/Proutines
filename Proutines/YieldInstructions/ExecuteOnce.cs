using System;

namespace Proutines
{
    public class ExecuteOnce : IYieldInstruction
    {
        private readonly Action method;

        public ExecuteOnce(Action method)
        {
            this.method = method;
        }

        public bool Yield()
        {
            method.Invoke();
            return false;
        }

        public bool Resetable => true;
        public void Reset()
        {
            // Left blank intentionally
        }
    }

    public class ExecuteOnce<T> : YieldOperationBase<T>
    {
        private readonly Func<T> method;

        public ExecuteOnce(Func<T> method)
        {
            this.method = method;
        }

        public override bool Resetable => true;

        protected override void Reset()
        {
            // Left blank intentionally
        }

        protected override YieldValue WillYield()
        {
            return YieldValue.Finish(method.Invoke());
        }
    }
}