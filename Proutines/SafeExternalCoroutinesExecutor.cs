using System;
using System.Collections.Generic;
using Logging;

namespace Proutines
{
    public class SafeExternalCoroutinesExecutor : IExternalCoroutinesExecutor
    {
        private readonly bool shouldLogExceptionsOnRootCoroutine;
        private readonly ThreadSafeCoroutinesExecutor externalCoroutinesExecutorImplementation;

        public SafeExternalCoroutinesExecutor(bool shouldLogExceptionsOnRootCoroutine = true)
        {
            this.shouldLogExceptionsOnRootCoroutine = shouldLogExceptionsOnRootCoroutine;
            externalCoroutinesExecutorImplementation = new ThreadSafeCoroutinesExecutor();
        }

        public void Dispose()
        {
            externalCoroutinesExecutorImplementation.Dispose();
        }

        public ICoroutine StartCoroutine(IEnumerator<IYieldInstruction> coroutineEnumerator)
        {
            return externalCoroutinesExecutorImplementation.StartCoroutine(coroutineEnumerator);
        }

        public void StopAllCoroutines()
        {
            externalCoroutinesExecutorImplementation.StopAllCoroutines();
        }

        public int Count => externalCoroutinesExecutorImplementation.Count;

        public void Update()
        {
            try
            {
                externalCoroutinesExecutorImplementation.Update();
            }
            catch (Exception ex)
            {
                if (shouldLogExceptionsOnRootCoroutine)
                {
                    LogUtils.Log("Exception while executing coroutine: showing message and stack trace", LogMessageType.Error);
                    LogUtils.Log(ex.Message + "\n" + ex.StackTrace, LogMessageType.Error);
                }

                externalCoroutinesExecutorImplementation.CurrentCoroutine?.DisposeFromException(ex);
                var lastCoroutineIndex = externalCoroutinesExecutorImplementation.CurrentCoroutineIndex;
                if (lastCoroutineIndex != -1)
                {
                    externalCoroutinesExecutorImplementation.UpdateCoroutines(lastCoroutineIndex);
                }
            }
        }
    }
}