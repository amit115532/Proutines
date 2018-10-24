using System;

namespace Proutines.Tasks
{
    /// <summary>
    /// When this exception is thrown, coroutine should stop, but can take its time. (means it can await)
    /// </summary>
    public class SignalStopException : Exception
    {
        public SignalStopException() : base("Coroutine was signaled to stop")
        {
            // Left blank intentionally
        }

        public SignalStopException(string message) : base(message)
        {
            // Left blank intentionally
        }
    }

    /// <summary>
    /// When this exception is thrown, coroutine should stop, but can take its time. (means it can await)
    /// </summary>
    public class SignalStopException<T> : SignalStopException
    {
        public T Value { get; }

        public SignalStopException(string message, T value) : base(message)
        {
            Value = value;
        }

        public SignalStopException(T value)
        {
            Value = value;
        }
    }
}