namespace Proutines
{
    /// <summary>
    /// It is sometimes helpful to wait for a number of seconds before moving on to the next statement
    /// </summary>
    public class WaitForSeconds : IYieldInstruction
    {
        /// <summary>
        /// The number of seconds to wait
        /// </summary>
        public double TimeToWait { get; set; }

        /// <summary>
        /// The time this yield instruction has started
        /// </summary>
        public double StartTime { get; set; }

        /// <summary>
        /// The time provider
        /// </summary>
        public ITimeProvider TimeProvider { get; set; }

        private bool isFirstYield = true;

        /// <summary>
        /// Wait a number of seconds before moving on to the next statement
        /// </summary>
        /// <param name="seconds">The number of seconds to wait</param>
        /// <param name="timeProvider">The time provider</param>
        public WaitForSeconds(double seconds, ITimeProvider timeProvider)
        {
            TimeProvider = timeProvider;
            TimeToWait = seconds;
        }

        /// <summary>
        /// Wait a number of seconds before moving on to the next statement.
        /// Will use <see cref="TimeProviders.DefaultTimeProvider"/> as the time provider
        /// </summary>
        /// <param name="seconds">The number of seconds to wait</param>
        public WaitForSeconds(double seconds)
            : this(seconds, TimeProviders.DefaultTimeProvider)
        {
            // Left blank intentionally
        }

        /// <summary>
        /// Wait a number of seconds before moving on to the next statement
        /// </summary>
        /// <param name="seconds">The number of seconds to wait</param>
        /// <param name="timeProvider">The time provider</param>
        public WaitForSeconds(float seconds, ITimeProvider timeProvider)
        {
            TimeProvider = timeProvider;
            TimeToWait = seconds;
        }

        /// <summary>
        /// Wait a number of seconds before moving on to the next statement.
        /// Will use <see cref="TimeProviders.DefaultTimeProvider"/> as the time provider
        /// </summary>
        /// <param name="seconds">The number of seconds to wait</param>
        public WaitForSeconds(float seconds)
            : this(seconds, TimeProviders.DefaultTimeProvider)
        {
            // Left blank intentionally
        }

        /// <summary>
        /// This method will return true if the coroutine should yield
        /// </summary>
        /// <returns>True if the coroutine should yield, otherwise False</returns>
        public bool Yield()
        {
            if (isFirstYield)
            {
                StartTime = TimeProvider.GetCurrentTime();
                isFirstYield = false;
            }

            return TimeProvider.GetCurrentTime() < StartTime + TimeToWait;
        }

        public bool Resetable => true;

        public void Reset()
        {
            isFirstYield = true;
        }
    }
}