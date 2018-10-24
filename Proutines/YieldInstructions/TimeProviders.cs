using Logging;

namespace Proutines
{
    /// <summary>
    /// Contains different functionalities for <see cref="ITimeProvider"/>
    /// </summary>
    public static class TimeProviders
    {
        /// <summary>
        /// Get or Set the default time provider for this system
        /// </summary>
        private static ITimeProvider _defaultTimeProvider;

        /// <summary>
        /// Get or Set the default time provider for this system
        /// </summary>
        public static ITimeProvider DefaultTimeProvider
        {
            get
            {
                if (_defaultTimeProvider == null)
                {
                    LogUtils.Break(MessageBuilder.Trace("The default time provider has not been assigned"));
                }

                return _defaultTimeProvider;
            }
            set => _defaultTimeProvider = value;
        }

        /// <summary>
        /// An implementation of time provider providing the time since startup
        /// </summary>
        public static ITimeProvider UnreliableRealTimeProvider { get; }

        /// <summary>
        /// An implementation of time provider providing the time since program startup
        /// </summary>
        public static ITimeProvider RealTimeProvider { get; }

        public static bool IsTimeProviderSet => _defaultTimeProvider != null;

        static TimeProviders()
        {
            RealTimeProvider = new ThreadSafeReliableSinceStartupTimeProvider();
            UnreliableRealTimeProvider = new SinceStartupTimeProvider();
        }
    }
}