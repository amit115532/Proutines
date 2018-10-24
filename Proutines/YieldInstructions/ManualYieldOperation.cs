namespace Proutines
{
    /// <summary>
    /// A common use for a manual yield operation is as a parameter of a coroutine that will
    /// manualy set the value when ready
    /// </summary>
    public class ManualYieldOperation<T> : IYieldOperation<T>
    {
        public T Value { get; set; }
        private volatile bool isDone;
        public bool IsDone => isDone;

        public void SetValue(T value)
        {
            Value = value;
            isDone = true;
        }

        public bool Yield()
        {
            return !isDone;
        }

        public bool Resetable => false;
        public void Reset()
        {
            throw new ResetNotSupportedException(this);
        }
    }
}