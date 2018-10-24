using System;

namespace Proutines
{
    /// <summary>
    /// Called when yielding a coroutine with an exception
    /// </summary>
    public class CoroutineException : Exception
    {
        public CoroutineException(Exception innerException) : base("Coroutine yielded an exception", innerException)
        {
            // Left blank intentionally
        }
    }
}