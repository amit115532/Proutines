using System;

namespace Proutines.Tasks
{
    public class CancellationException : Exception
    {
        public CancelationReason Reason { get; }

        public CancellationException(CancelationReason reason, Exception innerException = null) : base($"Task was canceled due to an {GetStringReason(reason)}", innerException)
        {
            Reason = reason;
        }

        private static string GetStringReason(CancelationReason reason)
        {
            switch (reason)
            {
                case CancelationReason.Interrupted:
                    return "outside interruption";
                case CancelationReason.InstructionException:
                    return "exception in instruction";
                default:
                    return "unknown reason";
            }
        }
    }
}