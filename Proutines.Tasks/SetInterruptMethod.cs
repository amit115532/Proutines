using System;

namespace Proutines.Tasks
{
    internal class SetInterruptMethod : Proutines.SetInterruptMethod
    {
        internal SetInterruptMethod(Action<InterruptReason, Exception> interruptExecution, InvokeOption invokeOption, bool disposeOnException = true)
            : base(interruptExecution, invokeOption, disposeOnException)
        {
            // Left blank intentionally
        }
    }
}