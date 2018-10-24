using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Proutines.Tests
{
    [TestClass]
    public class Pausing
    {
        public Pausing()
        {
            TestUtils.InitializeSystem();
        }

        [TestMethod]
        public void IsPauseWorkingProperly()
        {
            using (var executor = new SafeExternalCoroutinesExecutor())
            {
                int iteration = 0;

                void OnIteration()
                {
                    ++iteration;
                }

                var coroutine = executor.StartCoroutine(Coroutine(OnIteration));

                foreach (var i in Enumerable.Range(0, 10))
                {
                    Assert.AreEqual(iteration, i);
                    executor.Update();
                }

                coroutine.IsPaused = true;

                foreach (var i in Enumerable.Range(0, 10))
                {
                    Assert.AreEqual(10, iteration);
                    executor.Update();
                }

                coroutine.Dispose();

                foreach (var i in Enumerable.Range(0, 10))
                {
                    executor.Update();
                    Assert.AreEqual(10, iteration);
                }
            }
        }

        private IEnumerator<IYieldInstruction> Coroutine(Action onIteration)
        {
            while (true)
            {
                onIteration?.Invoke();
                yield return null;
            }
        }
    }
}