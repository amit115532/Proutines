using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Proutines.Tests
{
    [TestClass]
    public class Exceptions
    {
        public Exceptions()
        {
            TestUtils.InitializeSystem();
        }

        [TestMethod]
        public void ExceptionDisposingCoroutine()
        {
            using (var executor = new SafeExternalCoroutinesExecutor())
            {
                var iterationCount = 0;
                void OnIteration()
                {
                    ++iterationCount;
                }

                var coroutine = executor.StartCoroutine(CoroutineWithExceptionInBeginning(OnIteration));

                Assert.AreEqual(iterationCount, 0);
                Assert.AreEqual(executor.Count, 0);

                executor.Update();

                Assert.AreEqual(executor.Count, 1);

                executor.Update();

                Assert.IsTrue(coroutine.IsFinished);
                Assert.AreEqual(executor.Count, 0);

                executor.Update();

                Assert.AreEqual(iterationCount, 0);
            }
        }

        [TestMethod]
        public void CoroutineInterruptionOnException()
        {
            using (var executor = new SafeExternalCoroutinesExecutor())
            {
                var iterationCount = 0;
                var countForInterruption = 5;
                var wasInterrupted = false;
                void OnIteration()
                {
                    ++iterationCount;

                    if (iterationCount == countForInterruption)
                    {
                        throw new ArgumentException();
                    }
                }
                void OnInterrupt(InterruptReason reason, Exception ex)
                {
                    wasInterrupted = true;
                    Assert.AreEqual(InterruptReason.Exception, reason);
                    Assert.IsInstanceOfType(ex, typeof(ArgumentException));
                }

                executor.StartCoroutine(CoroutineWithInterruptMethod(OnIteration, OnInterrupt));

                foreach (var i in Enumerable.Range(0, countForInterruption))
                {
                    Assert.IsFalse(wasInterrupted);
                    executor.Update();
                }

                Assert.IsTrue(wasInterrupted);

                executor.Update();
                executor.Update();
                executor.Update();
                executor.Update();

                Assert.AreEqual(iterationCount, 5);
            }
        }

        private IEnumerator<IYieldInstruction> CoroutineWithExceptionInBeginning(Action onIteration)
        {
            throw new Exception();

            while (true)
            {
                onIteration?.Invoke();
                yield return null;
            }
        }

        private IEnumerator<IYieldInstruction> CoroutineWithInterruptMethod(Action iteration, Action<InterruptReason, Exception> onInterrupt)
        {
            yield return new SetInterruptMethod(onInterrupt);

            while (true)
            {
                iteration?.Invoke();
                yield return null;
            }
        }
    }
}