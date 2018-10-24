using System;

namespace Proutines
{
    /// <summary>
    /// Thrown when requesting instruction reset when not supported
    /// </summary>
    public class ResetNotSupportedException : Exception
    {
        public ResetNotSupportedException(IYieldInstruction instructionNotSupportReset) 
            : base($"The instruction cannot reset: {instructionNotSupportReset.GetType().Name}")
        {
            // Left blank intentionally
        }
    }
}