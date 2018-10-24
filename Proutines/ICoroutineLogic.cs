using System;

namespace Proutines
{
    internal interface ICoroutineLogic : IDisposable
    {
        /// <summary>
        /// Execute the current instruction
        /// </summary>
        /// <returns>False if there are no more instructions to execute</returns>
        bool ExecuteCurrentInstruction();

        /// <summary>
        /// Gets the coroutine to return to the user
        /// </summary>
        ICoroutine GetExternalCoroutine();

        /// <summary>
        /// Disposes the coroutine from exception
        /// </summary>
        void DisposeFromException(Exception ex);
    }
}