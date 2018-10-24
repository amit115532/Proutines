using System.Collections.Generic;
using System.Threading.Tasks;
using Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proutines.Tests;

namespace Proutines.Tasks.Tests
{
    [TestClass]
    public class Misc
    {
        public Misc()
        {
            TestUtils.InitializeSystem();
        }

        class Logger : ILogger
        {
            public List<string> Logs = new List<string>();

            public void Log(string message, LogMessageType type = LogMessageType.Log, object context = null)
            {
                Logs.Add(message);
            }

            public void Break()
            {

            }
        }

        [TestMethod]
        public void ShowingWarningOnConcurrentAwait()
        {
            var logger = new Logger();
            LogUtils.Logger = logger;

            using (var executor = new SafeExternalCoroutinesExecutor())
            {
                executor.StartTask(CallingAwaitConcurrently);

                executor.Update();

                Assert.IsTrue(logger.Logs[0].Contains("You are awaiting on the same yield object in parallel"));
            }
        }

        private async Task AwaitWaitOne(IYield yield)
        {
            await yield.WaitOne();
        }

        private async Task CallingAwaitConcurrently(IYield yield)
        {
#pragma warning disable 4014
            AwaitWaitOne(yield);
#pragma warning restore 4014
            await yield.WaitOne();
        }
    }
}
