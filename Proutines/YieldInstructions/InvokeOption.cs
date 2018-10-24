namespace Proutines
{
    /// <summary>
    /// Options for <see cref="SetInterruptMethod"/> invokation
    /// </summary>
    public enum InvokeOption
    {
        /// <summary>
        /// The method will be invoked only if the coroutine has interrupted
        /// </summary>
        Interrupt,
        /// <summary>
        /// The method will be invoked if the coroutine has interrupted or ended
        /// </summary>
        InterruptAndEnd
    }
}