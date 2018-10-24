using System;
using System.Collections.Generic;

namespace Proutines
{
    /// <summary>
    /// Extending the coroutines for more easy jobs
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Wait until the event is fired.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="count">fire count before continueing</param>
        /// <returns></returns>
        public static IYieldInstruction ToInstruction(this Action action, int count = 1)
        {
            if (count == 1)
                return new WaitForAction(action);

            return new WaitForMultipleActions(action, count);
        }

        /// <summary>
        /// Wait until the event is fired.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="count">fire count before continueing</param>
        /// <returns></returns>
        public static IYieldInstruction ToInstruction<T1, T2>(this Action<T1, T2> action, int count = 1)
        {
            if (count == 1)
                return new WaitForAction<T1, T2>(action);

            return new WaitForMultipleActions<T1, T2>(action, count);
        }

        /// <summary>
        /// Wait until the event is fired.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="count">fire count before continueing</param>
        /// <returns></returns>
        public static IYieldInstruction ToInstruction<T1, T2, T3>(this Action<T1, T2, T3> action, int count = 1)
        {
            if (count == 1)
                return new WaitForAction<T1, T2, T3>(action);

            return new WaitForMultipleActions<T1, T2, T3>(action, count);
        }

        /// <summary>
        /// Wait until the event is fired.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="count">fire count before continueing</param>
        /// <returns></returns>
        public static IYieldOperation<T> ToInstruction<T>(this Action<T> action, int count = 1)
        {
            if (count == 1)
                return new WaitForAction<T>(action);

            return new WaitForMultipleActions<T>(action, count);
        }

        /// <summary>
        /// Assign max cycles to a resetable instruction
        /// </summary>
        public static IYieldInstruction Loop(this IYieldInstruction instruction, int maxCycles)
        {
            if (maxCycles <= 0)
                return new WaitOne();

            if (maxCycles == 1)
                return instruction;

            if (!instruction.Resetable)
            {
                throw new ResetNotSupportedException(instruction);
            }

            return new MaxCyclesInstruction(instruction, maxCycles);
        }

        /// <summary>
        /// Assign max cycles to a resetable instruction
        /// </summary>
        public static IYieldInstruction LoopForever(this IYieldInstruction instruction)
        {
            if (!instruction.Resetable)
            {
                throw new ResetNotSupportedException(instruction);
            }

            return new LoopForever(instruction);
        }

        /// <summary>
        /// Wait on a coroutine
        /// </summary>
        /// <param name="coroutine">The coroutines to wait on</param>
        /// <param name="callback">What to do when finished</param>
        private static IEnumerator<IYieldInstruction> WaitOnMeRoutine(this IYieldInstruction coroutine, Action callback)
        {
            yield return coroutine;

            callback.Invoke();
        }

        /// <summary>
        /// Wait on a coroutines + a given time
        /// </summary>
        /// <param name="coroutine">The coroutines to wait on</param>
        /// <param name="time">A time to wait after the coroutine has finished</param>
        /// <param name="callback">What to do when finished</param>
        private static IEnumerator<IYieldInstruction> WaitOnMeRoutine(this IYieldInstruction coroutine, float time,
            Action callback)
        {
            yield return coroutine;

            yield return new WaitForSeconds(time);

            callback.Invoke();
        }

        /// <summary>
        /// Wait until a given time passes
        /// </summary>
        /// <param name="time">The time to wait</param>
        /// <param name="callback">What to do afterwards</param>
        /// <returns></returns>
        private static IEnumerator<IYieldInstruction> WaitForSecondsRoutine(float time, Action callback)
        {
            yield return new WaitForSeconds(time);

            callback.Invoke();
        }

        /// <summary>
        /// Wait for a given seconds and do something
        /// </summary>
        /// <param name="coroutinesExecutor"></param>
        /// <param name="time">The time to wait</param>
        /// <param name="callback">What to do afterwards</param>
        public static ICoroutine WaitAndDo(this ICoroutinesExecutor coroutinesExecutor, float time, Action callback)
        {
            return coroutinesExecutor.StartCoroutine(WaitForSecondsRoutine(time, callback));
        }

        public static IYieldInstruction WaitForValue<T>(this IYieldOperation<T> operation, Action<T> onValueReceived)
        {
            return WaitForOperationRoutine(operation, onValueReceived).ToInstruction();
        }

        /// <summary>
        /// Wait for a given seconds and do something
        /// </summary>
        /// <param name="coroutinesExecutor"></param>
        /// <param name="operation">The operation to wait for</param>
        /// <param name="callback">What to do afterwards</param>
        public static ICoroutine WaitAndDo<T>(this ICoroutinesExecutor coroutinesExecutor, IYieldOperation<T> operation,
            Action<T> callback)
        {
            return coroutinesExecutor.StartCoroutine(WaitForOperationRoutine(operation, callback));
        }

        /// <summary>
        /// </summary>
        /// <param name="coroutinesExecutor"></param>
        /// <param name="coroutine">The operation to wait for</param>
        /// <param name="callback">What to do afterwards</param>
        public static ICoroutine WaitAndDoIfNoException<T>(this ICoroutinesExecutor coroutinesExecutor, ICoroutine<T> coroutine,
            Action<T> callback)
        {
            return coroutinesExecutor.StartCoroutine(WaitForCoroutineRoutineIfNoException(coroutine, callback));
        }

        /// <summary>
        /// </summary>
        /// <param name="coroutinesExecutor"></param>
        /// <param name="coroutine">The operation to wait for</param>
        /// <param name="callback">What to do afterwards</param>
        public static ICoroutine WaitAndDoIfNoException(this ICoroutinesExecutor coroutinesExecutor, ICoroutine coroutine,
            Action callback)
        {
            return coroutinesExecutor.StartCoroutine(WaitForCoroutineRoutineIfNoException(coroutine, callback));
        }

        public static ICoroutine WaitUntil(this ICoroutinesExecutor coroutinesExecutor, Func<bool> until,
            Action callback)
        {
            return coroutinesExecutor.StartCoroutine(WaitUntilRoutine(until, callback));
        }

        public static IYieldInstruction ToInstruction(this IEnumerator<IYieldInstruction> routine)
        {
            return new CoroutineLogicAsInstruction(new CoroutineLogic(routine));
        }

        /// <summary>
        /// Wait for a given seconds and do something
        /// </summary>
        public static ICoroutine WaitUntil(this ICoroutinesExecutor coroutinesExecutor, Func<bool> until,
            float maximumAllowedTime, Action<bool> callback)
        {
            var instruction = WaitUntilRoutine(until, null).ToInstruction();
            return coroutinesExecutor.WaitAndDo(instruction.AssignMaximumAllowedTime(maximumAllowedTime), callback);
        }

        private static IEnumerator<IYieldInstruction> WaitForOperationRoutine<T>(IYieldOperation<T> operation,
            Action<T> callback)
        {
            yield return operation;
            callback.Invoke(operation.Value);
        }

        private static IEnumerator<IYieldInstruction> WaitForCoroutineRoutineIfNoException<T>(ICoroutine<T> coroutine,
            Action<T> callback)
        {
            yield return coroutine;

            if (!coroutine.HasException)
            {
                callback.Invoke(coroutine.Value);
            }

            // If exception was thrown do not call callback
        }

        private static IEnumerator<IYieldInstruction> WaitForCoroutineRoutineIfNoException(ICoroutine coroutine,
            Action callback)
        {
            yield return coroutine;

            if (!coroutine.HasException)
            {
                callback.Invoke();
            }

            // If exception was thrown do not call callback
        }

        private static IEnumerator<IYieldInstruction> WaitUntilRoutine(Func<bool> until, Action callback)
        {
            do
            {
                yield return null;
            } while (!until.Invoke());

            callback?.Invoke();
        }

        /// <summary>
        /// Will not let the given instruction update more than the given time
        /// Notice that this can only be applied when yielding.
        /// Assigning a maximum time for a coroutine like this: coroutineManager.StartCoroutine(Enumerator()).AssignMaximumAllowedTime(5); will not work.
        /// Should be used like this: yield return new WaitInstruction().AssignMaximumAllowedTime(5);
        /// Or yield return new Enumerator().ToInstruction().AssignMaximumAllowedTime(5);
        /// </summary>
        /// <returns>True if the given instruction ended before the time passed, false otherwise</returns>
        public static IYieldOperation<bool> AssignMaximumAllowedTime(this IYieldInstruction instruction, float time)
        {
            return new MinimumTime(instruction, new WaitForSeconds(time));
        }

        /// <summary>
        /// Connects two instructions into one instruction and wait until both resolve together
        /// This is different from "And" because "With" requires both to finish in the same time
        /// </summary>
        public static IYieldInstruction With(this IYieldInstruction a, IYieldInstruction b)
        {
            return new With(a, b);
        }

        /// <summary>
        /// Will not let the given instruction update more than the given time
        /// </summary>
        /// <returns>True if the given instruction ended before the time passed, false otherwise</returns>
        public static IYieldOperation<bool> StartCoroutineWithMaxAllowedTime(
            this ICoroutinesExecutor coroutinesExecutor, IEnumerator<IYieldInstruction> enumerator,
            float maximumAllowedTime)
        {
            var coroutine = coroutinesExecutor.StartCoroutine(enumerator);

            var yieldOperation = new ManualYieldOperation<bool>();
            coroutinesExecutor.StartCoroutine(MaxAllowedTimeRoutine(coroutine, maximumAllowedTime, yieldOperation));

            return yieldOperation;
        }

        internal static IEnumerator<IYieldInstruction> GenerateRoutine(this IYieldInstruction instruction)
        {
            yield return instruction;
        }

        internal static IEnumerator<IYieldInstruction> MaxAllowedTimeRoutine(this ICoroutine coroutine, float maxAllowedTime, ManualYieldOperation<bool> operation)
        {
            var maxAllowedTimeOperation = coroutine.AssignMaximumAllowedTime(maxAllowedTime);

            yield return maxAllowedTimeOperation;

            var isCoroutineFinishedOnTime = maxAllowedTimeOperation.Value;
            if (!isCoroutineFinishedOnTime)
            {
                coroutine.Dispose();
            }

            operation.SetValue(isCoroutineFinishedOnTime);
        }

        /// <summary>
        /// Wait until a given instruction has finished
        /// </summary>
        /// <param name="coroutinesExecutor"></param>
        /// <param name="instruction">The wait instruction</param>
        /// <param name="callback">What to do afterwards</param>
        public static ICoroutine WaitAndDo(this ICoroutinesExecutor coroutinesExecutor, IYieldInstruction instruction,
            Action callback)
        {
            return coroutinesExecutor.StartCoroutine(instruction.WaitOnMeRoutine(callback));
        }

        /// <summary>
        /// Execute a given instruction
        /// </summary>
        public static ICoroutine ExecuteInstruction(this ICoroutinesExecutor coroutinesExecutor, IYieldInstruction instruction)
        {
            return coroutinesExecutor.StartCoroutine(instruction.WaitOnMeRoutine(() => { /* left blank intentionally*/ }));
        }

        public static Execute Until(this Execute execute, IYieldInstruction yieldInstruction)
        {
            execute.BreakInstruction = yieldInstruction;
            return execute;
        }

        public static ExecuteContinuously Until(this ExecuteContinuously execute, IYieldInstruction yieldInstruction)
        {
            execute.Until = yieldInstruction;
            return execute;
        }

        public static ExecuteContinuously ExecuteWhile(this IYieldInstruction yieldInstruction, Action action)
        {
            return new ExecuteContinuously(action, yieldInstruction);
        }

        public static ExecuteContinuously Forever(this ExecuteContinuously execute)
        {
            execute.Until = new Forever();
            return execute;
        }

        public static Execute Every(this Execute execute, IYieldInstruction yieldInstruction)
        {
            if (!yieldInstruction.Resetable)
                throw new ResetNotSupportedException(yieldInstruction);

            execute.IterationDelay = yieldInstruction;
            return execute;
        }

        public static IYieldInstruction Enqueue(this ICoroutinesExecutor executor, Action func)
        {
            return executor.ExecuteInstruction(new ExecuteOnce(func));
        }

        public static IYieldOperation<T> Enqueue<T>(this ICoroutinesExecutor executor, Func<T> func)
        {
            var operation = new ExecuteOnce<T>(func);
            executor.ExecuteInstruction(operation);

            return operation.Wrap();
        }

        /// <summary>
        /// Wraps a yield operation that uses only Status.IsDone and will not call Yield()
        /// </summary>
        public static IYieldOperation<T> Wrap<T>(this IYieldOperation<T> operation)
        {
            return new WrappedYieldOperation<T>(operation);
        }

        /// <summary>
        /// Wait until a given coroutine has finished + a given time
        /// </summary>
        /// <param name="coroutinesExecutor"></param>
        /// <param name="coroutine">The coroutine to wait for</param>
        /// <param name="time">The time to wait after the coroutine was finished</param>
        /// <param name="callback">What to do afterwards</param>
        public static ICoroutine WaitAndDo(this ICoroutinesExecutor coroutinesExecutor, IYieldInstruction coroutine,
            float time, Action callback)
        {
            return coroutinesExecutor.StartCoroutine(coroutine.WaitOnMeRoutine(time, callback));
        }

        public static IYieldInstruction And(this IYieldInstruction a, IYieldInstruction b)
        {
            return new And(a, b);
        }

        public static IYieldInstruction Then(this IYieldInstruction a, IYieldInstruction b)
        {
            return new After(a, b);
        }

        public static IYieldOperation<T> Then<T>(this IYieldInstruction a, IYieldOperation<T> b)
        {
            return new After<T>(a, b);
        }

        public static IYieldInstruction Then(this IYieldInstruction a, Action b)
        {
            return new OnFinishedInstruction(a, b);
        }

        public static IYieldInstruction Then<T>(this IYieldOperation<T> a, Action<T> b)
        {
            return new OnFinishedInstructionWithParameter<T>(a, b);
        }

        public static IYieldInstruction Then(this IYieldInstruction a, Func<IYieldInstruction> b)
        {
            return new AfterFunction(a, b);
        }

        public static IYieldOperation<T> Then<T>(this IYieldInstruction a, Func<IYieldOperation<T>> b)
        {
            return new AfterFunction<T>(a, b);
        }

        public static IYieldInstruction Then<T>(this IYieldOperation<T> a, Func<T, IYieldInstruction> b)
        {
            return new AfterFunctionWithParameter<T>(a, b);
        }

        public static IYieldOperation<TOut> Then<TIn, TOut>(this IYieldOperation<TIn> a, Func<TIn, IYieldOperation<TOut>> b)
        {
            return new AfterFunctionWithParameter<TIn, TOut>(a, b);
        }

        public static IYieldOperation<T> Returns<T>(this IYieldInstruction a, Func<T> b)
        {
            return new OnFinishedInstruction<T>(a, b);
        }

        public static IYieldOperation<T> Returns<T>(this IYieldInstruction a, T b)
        {
            return new OnFinishedInstruction<T>(a, b);
        }

        public static IYieldOperation<TOut> Returns<TIn, TOut>(this IYieldOperation<TIn> a, Func<TIn, TOut> b)
        {
            return new OnFinishedInstruction<TIn, TOut>(a, b);
        }

        public static IYieldOperation<OrResult> Or(this IYieldInstruction a, IYieldInstruction b)
        {
            return new Or(a, b);
        }

        public static IYieldOperation<T> Or<T>(this IYieldOperation<T> a, IYieldOperation<T> b)
        {
            return new OrValue<T>(a, b);
        }

        public static ICoroutine ExecuteContinuously(this ICoroutinesExecutor executor, Action action)
        {
            return executor.StartCoroutine(UpdateRoutine(action));
        }

        private static IEnumerator<IYieldInstruction> UpdateRoutine(Action action)
        {
            restart:

            yield return null;
            action.Invoke();

            goto restart;
            // ReSharper disable once IteratorNeverReturns
        }

        public static ICoroutine ExecuteContinuously(this ICoroutinesExecutor executor, Action<int> action)
        {
            return executor.StartCoroutine(UpdateRoutine(action));
        }

        private static IEnumerator<IYieldInstruction> UpdateRoutine(Action<int> action)
        {
            var iteration = 0;

            restart:

            yield return null;
            action.Invoke(iteration);
            ++iteration;

            goto restart;
            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Will block the current thread until operation finishes.
        /// Do not call this function from the same thrad this operation is executing
        /// </summary>
        /// <param name="operation">The operation to wait for</param>
        /// <returns>The operation value</returns>
        public static T Block<T>(this IYieldOperation<T> operation)
        {
            while (!operation.IsDone)
            {
                operation.Yield();
            }

            // Tests show that without this one more yield 
            return operation.Value;
        }

        /// <summary>
        /// Will block the current thread until instruction finishes.
        /// Do not call this function from the same thrad this instruction is executing
        /// </summary>
        /// <param name="instruction">The instruction to wait for</param>
        public static void Block(this IYieldInstruction instruction)
        {
            while (instruction.Yield())
            {
                // Left blank intentionally
            }
        }

        /// <summary>
        /// Will block the current thread until instruction finishes.
        /// Do not call this function from the same thrad this instruction is executing
        /// </summary>
        /// <param name="instruction">The instruction to wait for</param>
        public static void BlockIgnoreException(this IYieldInstruction instruction)
        {
            try
            {
                while (instruction.Yield())
                {
                    // Left blank intentionally
                }
            }
            catch (CoroutineException)
            {
                // Left blank intentionally
            }
        }
    }
}