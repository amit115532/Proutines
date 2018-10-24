using System;
using Logging;

namespace Proutines.Tests
{
    public class TestLogger : ILogger
    {
        public void Log(string message, LogMessageType type = LogMessageType.Log, object context = null)
        {
            Console.WriteLine(message);
        }

        public void Break()
        {
            // Left blank intentionally
        }
    }
}