namespace Proutines.Tasks
{
    public class WrappedCoroutine : ICoroutine
    {
        private readonly IYield yield;
        private readonly ICoroutine wrappedCoroutine;
        private bool isDisposed;

        public WrappedCoroutine(ICoroutine wrappedCoroutine, IYield yield)
        {
            this.yield = yield;
            this.wrappedCoroutine = wrappedCoroutine;
        }

        public bool Yield()
        {
            return wrappedCoroutine.Yield();
        }

        public bool Resetable => false;
        public void Reset()
        {
            throw new ResetNotSupportedException(this);
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;

            yield.Dispose();
            wrappedCoroutine.Dispose();
            yield.MoveNext(); // So that the exception will be actually thrown
        }

        public bool IsFinished => wrappedCoroutine.IsFinished;

        public bool IsPaused
        {
            get => wrappedCoroutine.IsPaused;
            set => wrappedCoroutine.IsPaused = value;
        }

        public bool HasException => yield.HasException;
    }

    public class WrappedCoroutine<T> : WrappedCoroutine, ICoroutine<T>
    {
        private readonly IYieldOperation<T> wrappedOperation;

        public WrappedCoroutine(ICoroutine wrappedCoroutine, IYieldOperation<T> wrappedOperation, IYield yield)
            : base(wrappedCoroutine, yield)
        {
            this.wrappedOperation = wrappedOperation;
        }

        public T Value => wrappedOperation.Value;
        public bool IsDone => wrappedOperation.IsDone;
    }
}