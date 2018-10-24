using System;
using System.Collections.Generic;

namespace Proutines
{
    public interface ICoroutinesExecutor : IDisposable
    {
        /// <summary>
        /// Creates a new coroutine from the enumerator and starts to execute it
        /// </summary>
        /// <param name="coroutineEnumerator">The coroutine enumerator</param>
        /// <returns>A coroutine </returns>
        ICoroutine StartCoroutine(IEnumerator<IYieldInstruction> coroutineEnumerator);

        /// <summary>
        /// Stop all executing coroutines
        /// </summary>
        void StopAllCoroutines();

        int Count { get; }
    }
}