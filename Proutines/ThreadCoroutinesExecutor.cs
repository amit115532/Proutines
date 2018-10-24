using System;
using System.Threading;
using Logging;

namespace Proutines
{
    public class ThreadCoroutinesExecutor : ThreadSafeCoroutinesExecutor
    {
        private readonly int updateMs;
        private bool isStopped;

        public ThreadCoroutinesExecutor(int updateMs = 0)
        {
            this.updateMs = updateMs;
            new Thread(() => ThreadMethod(OnThreadStopped)).Start();
        }

        private void OnThreadStopped()
        {
            DisposeCoroutines();
        }

        private void ThreadMethod(Action onThreadStopped)
        {
            try
            {
                while (!isStopped)
                {
                    Update();

                    if (updateMs != 0)
                    {
                        Thread.Sleep(updateMs);
                    }
                }

                onThreadStopped.Invoke();
            }
            catch (Exception exception)
            {
                LogUtils.Log("Exception while executing coroutine: " + exception.Message + "\n" + exception.StackTrace);

                CurrentCoroutine?.DisposeFromException(exception);
                UpdateCoroutines(CurrentCoroutineIndex);

                ThreadMethod(onThreadStopped);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            isStopped = true;
        }
    }
}