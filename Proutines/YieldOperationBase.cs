namespace Proutines
{
    public abstract class YieldOperationBase<T> : IYieldOperation<T>
    {
        protected struct YieldValue
        {
            public readonly bool IsDone;
            public readonly T Value;

            /// <summary>
            /// IsDone is true when using this constructor
            /// You can use the empty constructor when the operation isnt finished
            /// </summary>
            public YieldValue(T value)
            {
                IsDone = true;
                Value = value;
            }

            /// <summary>
            /// Finish the operation
            /// </summary>
            public static YieldValue Finish(T value)
            {
                return new YieldValue(value);
            }

            /// <summary>
            /// Continue the operation
            /// </summary>
            public static YieldValue Continue()
            {
                return new YieldValue();
            }
        }

        public bool Yield()
        {
            if (isDone)
            {
                throw new OperationAlreadyFinishedException(GetType().Name);
            }

            var yieldResult = WillYield();

            if (!yieldResult.IsDone)
                return true;

            Value = yieldResult.Value;

            isDone = true;
            return false;
        }

        void IYieldInstruction.Reset()
        {
            isDone = false;
            Value = default(T);

            Reset();
        }

        public abstract bool Resetable { get; }
        protected abstract void Reset();

        protected abstract YieldValue WillYield();

        public T Value { get; private set; }

        private volatile bool isDone;
        public bool IsDone => isDone;
    }
}