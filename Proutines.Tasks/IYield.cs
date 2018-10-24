using System;
using System.Collections.Generic;

namespace Proutines.Tasks
{
    public interface IYield<out T> : IEnumerator<IYieldInstruction>, IAwaiter<T>, IAwaitable<T>
    {
        // Left blank intentionally
    }

    /// <summary>
    /// Object that can be used like the yield keyword inside an iterator block, but in an async function
    /// Acts as an iterator and as an awaiter
    /// </summary>
    public interface IYield : IEnumerator<IYieldInstruction>, IAwaiter, IAwaitable
    {
        /// <summary>
        /// Called when the object has been disposed
        /// </summary>
        event Action Disposed;

        /// <summary>
        /// Just like yield return.
        /// Can be used like this:
        /// await yield.Return(new WaitForSeconds(5));
        /// </summary>
        /// <param name="instruction">Defines how long to wait</param>
        IYield Return(IYieldInstruction instruction);

        /// <summary>
        /// Just like yield return but also allows a return value
        /// Used like thid:
        /// var = yield.Return(new 
        /// </summary>
        /// <param name="operation">Defines how long to wait and the object received</param>
        IYield<T> Return<T>(IYieldOperation<T> operation);

        ICoroutine Parallel(IteratorMethod method, ParallalOption option = ParallalOption.AsChild);
        ICoroutine<T> Parallel<T>(IteratorMethodReturns<T> method, ParallalOption option = ParallalOption.AsChild);

        bool HasException { get; }
        Action<Exception> ExceptionHandler { get; set; }
    }
}