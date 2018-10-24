namespace Proutines
{
    /// <summary>
    /// A finished yield operation. will return true for isDone.
    /// </summary>
    public class EndedYieldOperation<T> : IYieldOperation<T>
    {
        public T Value { get; }
        public bool IsDone => true;

        public EndedYieldOperation(T value)
        {
            Value = value;
        }

        public bool Yield()
        {
            return false;
        }

        public bool Resetable => true;
        public void Reset()
        {
            // Left blank intentionally
        }
    }
}