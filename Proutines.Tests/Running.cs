using System;
using System.Collections.Generic;
using System.Linq;
using Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Proutines.Tests
{
    [TestClass]
    public class Running
    {
        public Running()
        {
            TestUtils.InitializeSystem();
        }

        [TestMethod]
        public void IsRunningCoroutineProperlyForWaitOne()
        {
            var coroutinesExecutor = new SafeExternalCoroutinesExecutor();
            var executionNum = 0;

            void Func()
            {
                ++executionNum;
            }

            coroutinesExecutor.StartCoroutine(Coroutine(Func, new WaitOne()));

            for (var i = 0; i < 10; ++i)
            {
                Assert.AreEqual(executionNum, i);
                coroutinesExecutor.Update();
            }
        }

        [TestMethod]
        public void IsRunningCoroutineProperlyForNull()
        {
            var coroutinesExecutor = new SafeExternalCoroutinesExecutor();
            var executionNum = 0;

            void Func()
            {
                ++executionNum;
            }

            coroutinesExecutor.StartCoroutine(Coroutine(Func, null));

            for (var i = 0; i < 10; ++i)
            {
                Assert.AreEqual(executionNum, i);
                coroutinesExecutor.Update();
            }
        }

        [TestMethod]
        public void IsEndingCoroutineProperly()
        {
            var coroutinesExecutor = new SafeExternalCoroutinesExecutor();
            var isFinished = false;

            void Func()
            {
                isFinished = true;
            }

            coroutinesExecutor.StartCoroutine(Coroutine(null, (i) => LogUtils.Log(i.ToString()), Func, 10, null));

            for (var i = 0; i < 11; ++i)
            {
                Assert.IsFalse(isFinished);
                coroutinesExecutor.Update();
            }

            Assert.IsTrue(isFinished);
        }

        private IEnumerator<IYieldInstruction> Coroutine(Action action, IYieldInstruction instruction)
        {
            while (true)
            {
                action?.Invoke();
                yield return instruction;
            }
        }

        private IEnumerator<IYieldInstruction> Coroutine(Action firstAction, Action<int> iterationAction, Action finishAction, int numberOfIterations, IYieldInstruction instruction)
        {
            firstAction?.Invoke();

            foreach (var i in Enumerable.Range(0, numberOfIterations))
            {
                iterationAction?.Invoke(i);
                yield return instruction;
            }

            finishAction?.Invoke();
        }
    }
}
