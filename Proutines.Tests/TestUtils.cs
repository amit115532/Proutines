using Logging;

namespace Proutines.Tests
{
    public static class TestUtils
    {
        public static void InitializeSystem()
        {
            if (!LogUtils.IsLoggerSet)
            {
                LogUtils.Logger = new TestLogger();
            }

            if (!TimeProviders.IsTimeProviderSet)
            {
                TimeProviders.DefaultTimeProvider = TimeProviders.UnreliableRealTimeProvider;
            }
        }
    }
}