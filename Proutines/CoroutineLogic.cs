using System;
using System.Collections.Generic;

namespace Proutines
{
    /// <summary>
    /// Provides the logic for a single coroutine from an IEnumerator{IYieldInstruction}
    /// </summary>
    internal class CoroutineLogic : ICoroutineLogic, ICoroutine
    {
        private readonly IEnumerator<IYieldInstruction> enumerator;
        private SetInterruptMethod currentInterruptMethod;
        private bool isFirstInstruction = true;
        private readonly object locker = new object();
        public bool DisposeOnException { get; set; } = true;

        public CoroutineLogic(IEnumerator<IYieldInstruction> enumerator)
        {
            this.enumerator = enumerator;
        }

        public bool ExecuteCurrentInstruction()
        {
            if (IsFinished)
                return false;

            if (IsPaused)
                return true;

            var instruction = enumerator.Current;

            // If it yields just for 1 frame, make it continue
            if (instruction == null)
            {
                return MoveToNextInstruction();
            }

            return instruction.Yield() || MoveToNextInstruction();
        }

        private bool MoveToNextInstruction()
        {
            RecheckCoroutine:

            if (!MoveNext())
            {
                return false;
            }

            if (isFirstInstruction)
            {
                isFirstInstruction = false;

                if (enumerator.Current is SetInterruptMethod interruptMethod)
                {
                    SetInterruptMethod(interruptMethod);
                    goto RecheckCoroutine;
                }
            }

            return true;
        }

        public void Dispose()
        {
            if (DisposeWithoutInterrupting())
            {
                InvokeInterruptFunctionOnKill();
            }
        }

        private bool DisposeWithoutInterrupting()
        {
            if (!IsFinished)
            {
                isFinished = true;
                return true;
            }

            return false;
        }

        private bool MoveNext()
        {
            if (IsFinished)
                return false;

            if (enumerator.MoveNext())
            {
                return true;
            }

            InvokeInterruptFunctionOnEnd();
            isFinished = true;

            return false;
        }

        public void SetInterruptMethod(SetInterruptMethod interruptMethod)
        {
            currentInterruptMethod = interruptMethod;
        }

        private void InvokeInterruptFunctionOnKill()
        {
            currentInterruptMethod?.GetInterruptExecution().Invoke(InterruptReason.Interrupted, null);
        }

        private void InvokeInterruptFunctionOnEnd()
        {
            if (currentInterruptMethod?.InvokeOption == InvokeOption.InterruptAndEnd)
                currentInterruptMethod.GetInterruptExecution().Invoke(InterruptReason.End, null);
        }

        private void InvokeInterruptFunctionOnException(Exception ex)
        {
            currentInterruptMethod?.GetInterruptExecution().Invoke(InterruptReason.Exception, ex);
        }

        public ICoroutine GetExternalCoroutine()
        {
            return this;
        }

        public void DisposeFromException(Exception ex)
        {
            Exception = ex;
            hasException = true;

            if (currentInterruptMethod == null)
            {
                DisposeWithoutInterrupting();
                return;
            }

            if (currentInterruptMethod.DisposeOnException)
            {
                if (DisposeWithoutInterrupting())
                {
                    InvokeInterruptFunctionOnException(ex);
                }
            }
            else
            {
                InvokeInterruptFunctionOnException(ex);
            }
        }

        public bool Yield()
        {
            if (HasException)
            {
                throw new CoroutineException(Exception);
            }

            return !IsFinished;
        }

        public bool Resetable => false;
        public void Reset()
        {
            throw new ResetNotSupportedException(this);
        }

        public bool IsFinished => isFinished;
        private volatile bool isFinished;
        public bool IsPaused { get; set; }
        private volatile bool hasException;
        public bool HasException => hasException;
        public Exception Exception { get; private set; }
    }
}