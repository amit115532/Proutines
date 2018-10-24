namespace Proutines
{
    /// <summary>
    /// This interface should be inherited to create a new yielding logic for coroutines
    /// </summary>
    public interface IYieldInstruction
    {
        /// <summary>
        /// This method will return true if the coroutine should yield
        /// TRY YOUR BEST NOT TO THROW EXCEPTIONS FROM HERE!!
        /// Exceptions can still be caught, but only on safe executors
        /// </summary>
        /// <returns>True if the coroutine should yield, otherwise False</returns>
        bool Yield();

        /// <summary>
        /// Is this instruction supports resetting?
        /// </summary>
        bool Resetable { get; }

        /// <summary>
        /// Do reset operations
        /// </summary>
        /// <exception cref="ResetNotSupportedException">When reset is not supported</exception>
        void Reset();
    }
}