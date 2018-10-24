namespace Proutines
{
    public interface IYieldOperation<out T> : IYieldInstruction
    {
        /// <summary>
        /// The result of the operation
        /// </summary>
        /// <exception cref="OperationNotReadyException">If <see cref="IsDone"/> is false</exception>
        T Value { get; }

        /// <summary>
        /// Is the operation done?
        /// </summary>
        bool IsDone { get; }
    }
}