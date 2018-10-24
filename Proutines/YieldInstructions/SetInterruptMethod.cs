using System;
using Logging;

namespace Proutines
{
    /// <summary>
    /// A coroutine can set an interrupt method to be executed when interrupting. It can override the previously 
    /// interrupt method simply by setting a new one.
    /// To set an interrupt method, yield on this class instance, it wont cause your coroutine to yield
    /// </summary>
    public class SetInterruptMethod : IYieldInstruction
    {
        /// <summary>
        /// The method to execute when interrupting
        /// </summary>
        private readonly Action<InterruptReason, Exception> interruptExecution;

        public readonly bool DisposeOnException = true;

        /// <summary>
        /// Decide when to invoke the interrupt method
        /// </summary>
        public InvokeOption InvokeOption { get; private set; }

        /// <summary>
        /// Set a new interrupt method
        /// </summary>
        /// <param name="interruptExecution">The method to be executed when interrupting</param>
        /// <param name="invokeOption">Decide when to invoke the interrupt method</param>
        public SetInterruptMethod(Action interruptExecution, InvokeOption invokeOption = InvokeOption.Interrupt)
        {
            this.interruptExecution = (a, b) => interruptExecution.Invoke();
            InvokeOption = invokeOption;
        }

        /// <summary>
        /// Set a new interrupt method
        /// </summary>
        /// <param name="interruptExecution">The method to be executed when interrupting</param>
        /// <param name="invokeOption">Decide when to invoke the interrupt method</param>
        public SetInterruptMethod(Action<InterruptReason> interruptExecution, InvokeOption invokeOption = InvokeOption.Interrupt)
        {
            this.interruptExecution = (a, b) => interruptExecution.Invoke(a);
            InvokeOption = invokeOption;
        }

        /// <summary>
        /// Set a new interrupt method
        /// </summary>
        /// <param name="interruptExecution">The method to be executed when interrupting</param>
        /// <param name="invokeOption">Decide when to invoke the interrupt method</param>
        public SetInterruptMethod(Action<InterruptReason, Exception> interruptExecution, InvokeOption invokeOption = InvokeOption.Interrupt)
        {
            this.interruptExecution = interruptExecution;
            InvokeOption = invokeOption;
        }

        /// <summary>
        /// Set a new interrupt method
        /// </summary>
        protected SetInterruptMethod(Action<InterruptReason, Exception> interruptExecution, InvokeOption invokeOption, bool disposeOnException = true)
        {
            this.interruptExecution = interruptExecution;
            DisposeOnException = disposeOnException;
            InvokeOption = invokeOption;
        }

        /// <summary>
        /// Get the method to be executed when interrupting
        /// </summary>
        public Action<InterruptReason, Exception> GetInterruptExecution()
        {
            return interruptExecution;
        }

        public bool Yield()
        {
            LogUtils.Log("SetInterruptMethod can only be used once and as the first yielded instruction of the coroutine. It will be ignored", LogMessageType.Warning);
            return false;
        }

        public bool Resetable => false;
        public void Reset()
        {
            throw new ResetNotSupportedException(this);
        }
    }
}