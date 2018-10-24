using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Proutines.Tests
{
    [TestClass]
    public class Disposing
    {
        public Disposing()
        {
            TestUtils.InitializeSystem();
        }

        [TestMethod]
        public void IsDisposingProperly()
        {
            var coroutinesExecutor = new SafeExternalCoroutinesExecutor();
            int iteration = 0;
            bool isDisposed = false;

            void OnIteration()
            {
                ++iteration;
            }

            void OnDispose(InterruptReason reason)
            {
                Assert.AreEqual(InterruptReason.Interrupted, reason);
                isDisposed = true;
            }

            var coroutine = coroutinesExecutor.StartCoroutine(CoroutineWithInterruptMethod(OnIteration, OnDispose));

            Assert.AreEqual(iteration, 0);
            coroutinesExecutor.Update();
            Assert.AreEqual(iteration, 1);
            coroutinesExecutor.Update();
            Assert.AreEqual(iteration, 2);
            coroutinesExecutor.Update();
            Assert.AreEqual(iteration, 3);
            coroutinesExecutor.Update();
            Assert.AreEqual(iteration, 4);

            Assert.IsFalse(isDisposed);

            coroutine.Dispose();

            Assert.IsTrue(isDisposed);

            for (int i = 0; i < 10; i++)
            {
                coroutinesExecutor.Update();
                Assert.AreEqual(iteration, 4);
            }
        }

        [TestMethod]
        public void ProperCountOfCoroutines()
        {
            var coroutinesExecutor = new SafeExternalCoroutinesExecutor();

            Assert.AreEqual(coroutinesExecutor.Count, 0);

            var coroutine = coroutinesExecutor.StartCoroutine(CoroutineWithInterruptMethod(null, null));

            for (int i = 0; i < 10; i++)
            {
                coroutinesExecutor.Update();
                Assert.AreEqual(coroutinesExecutor.Count, 1);
            }

            coroutine.Dispose();

            coroutinesExecutor.Update();
            Assert.AreEqual(coroutinesExecutor.Count, 0);
        }

        [TestMethod]
        public void ProperCountOfCoroutinesWhenUsingStopAll()
        {
            var coroutinesExecutor = new SafeExternalCoroutinesExecutor();

            Assert.AreEqual(coroutinesExecutor.Count, 0);
            var count = 10;
            for (var i = 0; i < count; ++i)
            {
                coroutinesExecutor.StartCoroutine(CoroutineWithInterruptMethod(null, null));
            }

            Assert.AreEqual(coroutinesExecutor.Count, 0);
            coroutinesExecutor.Update();
            Assert.AreEqual(coroutinesExecutor.Count, count);

            coroutinesExecutor.StopAllCoroutines();
            Assert.AreEqual(coroutinesExecutor.Count, count);
            coroutinesExecutor.Update();
            Assert.AreEqual(coroutinesExecutor.Count, 0);
        }

        [TestMethod]
        public void ProperCountOfCoroutinesWhenUsingDispose()
        {
            var coroutinesExecutor = new SafeExternalCoroutinesExecutor();

            Assert.AreEqual(coroutinesExecutor.Count, 0);
            var count = 10;
            for (var i = 0; i < count; ++i)
            {
                coroutinesExecutor.StartCoroutine(CoroutineWithInterruptMethod(null, null));
            }

            Assert.AreEqual(coroutinesExecutor.Count, 0);
            coroutinesExecutor.Update();
            Assert.AreEqual(coroutinesExecutor.Count, count);

            coroutinesExecutor.Dispose();
            Assert.AreEqual(coroutinesExecutor.Count, 0);
        }

        [TestMethod]
        public void DisposingProperlyWhenUsingStopAll()
        {
            var coroutinesExecutor = new SafeExternalCoroutinesExecutor();

            Assert.AreEqual(coroutinesExecutor.Count, 0);
            var count = 10;
            var disposedCount = 0;
            int iterations = 0;

            void OnDispose(InterruptReason reason)
            {
                Assert.AreEqual(InterruptReason.Interrupted, reason);
                ++disposedCount;
            }

            void OnIteration()
            {
                ++iterations;
            }

            for (var i = 0; i < count; ++i)
            {
                coroutinesExecutor.StartCoroutine(CoroutineWithInterruptMethod(OnIteration, OnDispose));
            }

            coroutinesExecutor.Update();

            coroutinesExecutor.StopAllCoroutines();

            Assert.AreEqual(disposedCount, 0);
            var currentIterations = iterations;

            coroutinesExecutor.Update();

            Assert.AreEqual(disposedCount, 10);
            Assert.AreEqual(currentIterations, iterations);
        }

        [TestMethod]
        public void DisposingInstantlyProperlyWhenUsingDispose()
        {
            var coroutinesExecutor = new SafeExternalCoroutinesExecutor();

            Assert.AreEqual(coroutinesExecutor.Count, 0);
            var count = 10;
            var disposedCount = 0;
            int iterations = 0;

            void OnDispose(InterruptReason reason)
            {
                Assert.AreEqual(InterruptReason.Interrupted, reason);

                ++disposedCount;
            }

            void OnIteration()
            {
                ++iterations;
            }

            for (var i = 0; i < count; ++i)
            {
                coroutinesExecutor.StartCoroutine(CoroutineWithInterruptMethod(OnIteration, OnDispose));
            }

            coroutinesExecutor.Update();

            var currentIterations = iterations;
            coroutinesExecutor.Dispose();

            Assert.AreEqual(disposedCount, 10);
            Assert.AreEqual(currentIterations, iterations);
        }

        private IEnumerator<IYieldInstruction> CoroutineWithInterruptMethod(Action onIteration, Action<InterruptReason> onDisposed)
        {
            yield return new SetInterruptMethod((reason) => onDisposed?.Invoke(reason));

            while (true)
            {
                onIteration?.Invoke();
                yield return null;
            }
        }
    }
}