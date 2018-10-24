using System;

namespace Proutines
{
    public class OperationAlreadyFinishedException : Exception
    {
        public OperationAlreadyFinishedException(string yieldOperationName)
            : base($"A value was already set for yield operation {yieldOperationName}. " +
                   $"You may be yielding the same operation in parallel. Please wrap the operation instead")
        {
            // Left blank intentionally
        }
    }
}