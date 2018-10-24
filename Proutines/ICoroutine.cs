using System;

namespace Proutines
{
    /// <summary>
    /// A coroutine a user can use when starting a new coroutine using the <see cref="ICoroutinesExecutor"/>
    /// </summary>
    public interface ICoroutine : IYieldInstruction, IDisposable
    {
        bool IsFinished { get; }
        bool IsPaused { get; set; }

        bool HasException { get; }
    }

    /// <summary>
    /// A coroutine a user can use when starting a new coroutine using the <see cref="ICoroutinesExecutor"/>
    /// </summary>
    public interface ICoroutine<out T> : ICoroutine, IYieldOperation<T>
    {
        // Left blank intentionally
    }
}