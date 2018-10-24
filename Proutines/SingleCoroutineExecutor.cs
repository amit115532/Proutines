using System.Collections.Generic;
using Logging;

namespace Proutines.Internal
{
    /// <summary>
    /// An optimized version supporting only one coroutine at a time. Use this only when necessary 
    /// </summary>
    public class SingleCoroutineExecutor : IExternalCoroutinesExecutor
    {
        private ICoroutineLogic coroutine;

        public bool HasCoroutine => coroutine != null;
        public int Count => HasCoroutine ? 1 : 0;

        public void Dispose()
        {
            StopAllCoroutines();
            Update();
        }

        public ICoroutine StartCoroutine(IEnumerator<IYieldInstruction> coroutineEnumerator)
        {
            if (HasCoroutine)
            {
                LogUtils.Log("Already contains a coroutine. disposing old one", LogMessageType.Warning);

                coroutine.Dispose();
                coroutine.ExecuteCurrentInstruction();
            }

            coroutine = new CoroutineLogic(coroutineEnumerator);

            return coroutine.GetExternalCoroutine();
        }

        public void StopAllCoroutines()
        {
            if (HasCoroutine)
            {
                coroutine.Dispose();
            }
        }

        public void Update()
        {
            if (!HasCoroutine)
                return;

            if (!coroutine.ExecuteCurrentInstruction())
            {
                coroutine = null;
            }
        }
    }
}