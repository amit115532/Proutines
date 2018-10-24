using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Proutines.Tests
{
    [TestClass]
    public class ExtensionMethodsTest
    {
        public ExtensionMethodsTest()
        {
            TestUtils.InitializeSystem();
        }

        [TestMethod]
        public void TestBlock()
        {
            using (var threadedExecutor = new ThreadCoroutinesExecutor())
            {
                var iteration = 0;
                const int ITERATION_TO_FINISH = 10;

                bool ShouldFinish()
                {
                    if (iteration == ITERATION_TO_FINISH)
                    {
                        return true;
                    }

                    ++iteration;
                    return false;
                }

                var coroutine = threadedExecutor.StartCoroutine(Coroutine(ShouldFinish));

                coroutine.Block();

                Assert.AreEqual(iteration, ITERATION_TO_FINISH);
            }
        }

        [TestMethod]
        public void TestBlockWithException()
        {
            using (var threadedExecutor = new ThreadCoroutinesExecutor())
            {
                var iteration = 0;
                const int ITERATION_TO_EXCEPTION = 10;

                bool ShouldFinish()
                {
                    if (iteration == ITERATION_TO_EXCEPTION)
                    {
                        throw new ArgumentException();
                    }

                    ++iteration;
                    return false;
                }

                var coroutine = threadedExecutor.StartCoroutine(Coroutine(ShouldFinish));

                Assert.ThrowsException<CoroutineException>(() => coroutine.Block());

                Assert.AreEqual(iteration, ITERATION_TO_EXCEPTION);
            }
        }

        [TestMethod]
        public void TestBlockWithExceptionDisabled()
        {
            using (var threadedExecutor = new ThreadCoroutinesExecutor())
            {
                var iteration = 0;
                const int ITERATION_TO_EXCEPTION = 10;

                bool ShouldFinish()
                {
                    if (iteration == ITERATION_TO_EXCEPTION)
                    {
                        throw new ArgumentException();
                    }

                    ++iteration;
                    return false;
                }

                var coroutine = threadedExecutor.StartCoroutine(Coroutine(ShouldFinish));

                coroutine.BlockIgnoreException();

                Assert.AreEqual(iteration, ITERATION_TO_EXCEPTION);
            }
        }

        [TestMethod]
        public void TestEnqueue()
        {
            using (var executorA = new SafeExternalCoroutinesExecutor())
            using (var executorB = new SafeExternalCoroutinesExecutor())
            {
                bool isExecuted = false;
                bool isTested = false;

                var instruction = executorA.Enqueue(() => isExecuted = true);
                executorB.WaitAndDo(instruction, () =>
                {
                    Assert.IsTrue(isExecuted);
                    isTested = true;
                });

                executorA.Update();
                executorA.Update();
                executorB.Update();
                executorB.Update();

                Assert.IsTrue(isTested);
            }
        }

        private IEnumerator<IYieldInstruction> Coroutine(Func<bool> shouldFinish)
        {
            while (!shouldFinish.Invoke()) yield return null;
        }
    }
}