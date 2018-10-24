using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Proutines.Tasks
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Will throw a <see cref="SignalStopException"/> when instruction finishes
        /// To cancel, use dispose on the return value
        /// </summary>
        public static ICoroutine SignalStopOn(this IYield yield, IYieldInstruction instruction)
        {
            return yield.Parallel(instruction.GetTaskMethod().Then(() => throw new SignalStopException()));
        }

        /// <summary>
        /// Will throw a <see cref="SignalStopException"/> when instruction finishes
        /// To cancel, use dispose on the return value
        /// </summary>
        public static ICoroutine SignalStopOn(this IYield yield, IteratorMethod instruction)
        {
            return yield.Parallel(instruction.Then(() => throw new SignalStopException()));
        }

        /// <summary>
        /// Will throw a <see cref="SignalStopException"/> when instruction finishes
        /// To cancel, use dispose on the return value
        /// </summary>
        public static ICoroutine SignalStopOn<T>(this IYield yield, IteratorMethodReturns<T> instruction)
        {
            return yield.Parallel(instruction.Then((e) => throw new SignalStopException<T>(e)));
        }

        /// <summary>
        /// Will throw a <see cref="SignalStopException"/> when instruction finishes
        /// To cancel, use dispose on the return value
        /// </summary>
        public static ICoroutine SignalStopOn<T>(this IYield yield, IYieldOperation<T> instruction)
        {
            return yield.Parallel(instruction.Then((e) => throw new SignalStopException<T>(e)).GetTaskMethod());
        }

        public static ICoroutine StartTask(this ICoroutinesExecutor executor, IteratorMethod method)
        {
            var yield = new Yield(method, executor);
            var coroutine = new WrappedCoroutine(executor.StartCoroutine(yield), yield);

            yield.Coroutine = coroutine;
            return coroutine;
        }
        public static ICoroutine StartTask(this ICoroutinesExecutor executor, IteratorMethod method, Action<Exception> onException)
        {
            var yield = new Yield(method, executor, onException);
            var coroutine = new WrappedCoroutine(executor.StartCoroutine(yield), yield);

            yield.Coroutine = coroutine;

            return coroutine;
        }
        public static ICoroutine<T> StartTask<T>(this ICoroutinesExecutor executor, IteratorMethodReturns<T> method)
        {
            var op = new ManualYieldOperation<T>();
            var yield = new Yield((a) => TaskRunner(a, method, op), executor);
            var coroutine = new WrappedCoroutine<T>(executor.StartCoroutine(yield), op, yield);
            yield.Coroutine = coroutine;

            return coroutine;
        }
        public static ICoroutine<T> StartTask<T>(this ICoroutinesExecutor executor, IteratorMethodReturns<T> method, Action<Exception> onException)
        {
            var op = new ManualYieldOperation<T>();
            var yield = new Yield((a) => TaskRunner(a, method, op), executor, onException);

            var coroutine = new WrappedCoroutine<T>(executor.StartCoroutine(yield), op, yield);

            yield.Coroutine = coroutine;
            return coroutine;
        }

        public static void StartTask(this ICoroutinesExecutor executor, IteratorMethod method, Action onFinished, Action<Exception> onException = null)
        {
            if (onException == null)
            {
                executor.WaitAndDo(executor.StartTask(method), onFinished);
            }
            else
            {
                var coroutine = executor.StartTask(method, onException);
                executor.WaitAndDoIfNoException(coroutine, onFinished);
            }
        }

        private static async Task TaskRunner<T>(IYield yield, IteratorMethodReturns<T> method,
            ManualYieldOperation<T> op)
        {
            var returnValue = await method.Invoke(yield);
            op.SetValue(returnValue);
        }

        public static IYieldOperation<T> ThreadParallel<T>(this IYield yield, Func<CancellationToken, T> task)
        {
            return new AsyncThread<T>(yield, task);
        }

        public static IYieldOperation<T> ThreadParallel<T>(this IYield yield, Func<CancellationToken, Task<T>> task)
        {
            return new AsyncThread<T>(yield, task);
        }

        public static IYieldInstruction ThreadParallel(this IYield yield, Action<CancellationToken> task)
        {
            return new AsyncThread(yield, task);
        }

        public static IYieldInstruction ThreadParallel(this IYield yield, Func<CancellationToken, Task> task)
        {
            return new AsyncThread(yield, task);
        }

        public static Task Return(this IYield yield, IteratorMethod method)
        {
            return method.Invoke(yield);
        }

        public static Task<T> Return<T>(this IYield yield, IteratorMethodReturns<T> method)
        {
            return method.Invoke(yield);
        }
        /// <summary>
        /// Will wait for one task and will kill all other tasks
        /// </summary>
        public static Task<int> First(this IYield yield, params ICoroutine[] coroutines)
        {
            return yield.FirstImplementation(coroutines);
        }

        private static async Task<int> FirstImplementation(this IYield yield, IYieldInstruction[] coroutines)
        {
            int finishedIndex;

            var oldExceptionHandler = yield.ExceptionHandler;
            try
            {
                yield.ExceptionHandler = null;
                var operation = Instruction.First(coroutines);
                finishedIndex = await yield.Return(operation);
            }
            finally
            {
                for (var i = 0; i < coroutines.Length; ++i)
                {
                    if (coroutines[i] is ICoroutine coroutine)
                    {
                        if (coroutine.IsFinished)
                        {
                            continue;
                        }

                        coroutine.Dispose();
                    }
                }

                yield.ExceptionHandler = oldExceptionHandler;
            }

            return finishedIndex;
        }

        public static ICoroutine<T> Parallel<T>(this IYield yield, ICoroutine<T> coroutine)
        {
            return yield.Parallel(coroutine.GetTaskMethod());
        }

        public static IYield WaitOne(this IYield yield)
        {
            return yield.Return(null);
        }

        public static IYield WaitForSeconds(this IYield yield, float seconds)
        {
            return yield.Return(new WaitForSeconds(seconds));
        }

        public static IYield WaitForSeconds(this IYield yield, double seconds)
        {
            return yield.Return(new WaitForSeconds(seconds));
        }

        private static async Task<IEnumerable<T>> SelectImplementation<T>(this IYield yield, IYieldOperation<T>[] operations)
        {
            var oldExceptionHandler = yield.ExceptionHandler;
            try
            {
                yield.ExceptionHandler = null;
                await yield.Return(Instruction.All(operations));

                return operations.Select((c) => c.Value);
            }
            finally
            {
                yield.ExceptionHandler = oldExceptionHandler;

                for (int i = 0; i < operations.Length; i++)
                {
                    if (operations[i] is ICoroutine<T> coroutine)
                    {
                        if (coroutine.IsFinished)
                            continue;

                        coroutine.Dispose();
                    }
                }
            }
        }

        private static async Task<IEnumerable<T>> OrderedSelectImplementation<T>(this IYield yield, IYieldOperation<T>[] operations)
        {
            var oldExceptionHandler = yield.ExceptionHandler;
            var finished = new List<IYieldOperation<T>>(operations.Length);

            try
            {
                yield.ExceptionHandler = null;

                while (finished.Count < operations.Length)
                {
                    var finishedCoroutine = await yield.GetFirstFinished(operations.Where((c) => !finished.Contains(c)).ToArray());
                    finished.Add(finishedCoroutine);
                }

                return finished.Select(f => f.Value);
            }
            finally
            {
                yield.ExceptionHandler = oldExceptionHandler;

                foreach (var operation in operations)
                {
                    if (operation is ICoroutine coroutine)
                    {
                        if (coroutine.IsFinished)
                            continue;

                        coroutine.Dispose();
                    }
                }
            }
        }

        private static async Task<IYieldOperation<T>> GetFirstFinished<T>(this IYield yield, IYieldOperation<T>[] coroutines)
        {
            while (true)
            {
                for (int i = 0; i < coroutines.Length; i++)
                {
                    if (!coroutines[i].Yield())
                    {
                        return coroutines[i];
                    }
                }

                await yield.WaitOne();
            }
        }

        public static Task<IEnumerable<T>> Select<T>(this IYield yield, params IteratorMethodReturns<T>[] methods)
        {
            var coroutines = methods.Select(iterator => yield.Parallel(iterator));
            return yield.SelectImplementation(coroutines.ToArray());
        }

        public static Task<IEnumerable<T>> Select<T>(this IYield yield, IEnumerable<IteratorMethodReturns<T>> methods)
        {
            var coroutines = methods.Select(iterator => yield.Parallel(iterator));
            return yield.SelectImplementation(coroutines.ToArray());
        }

        public static Task<IEnumerable<T>> Select<T>(this IYield yield, params IYieldOperation<T>[] coroutines)
        {
            return yield.SelectImplementation(coroutines);
        }
        public static Task<IEnumerable<T>> Select<T>(this IYield yield, IEnumerable<IYieldOperation<T>> coroutines)
        {
            return yield.SelectImplementation(coroutines.ToArray());
        }

        public static Task<IEnumerable<T>> OrderedSelect<T>(this IYield yield, params IteratorMethodReturns<T>[] methods)
        {
            var coroutines = methods.Select(iterator => yield.Parallel(iterator));
            return yield.OrderedSelectImplementation(coroutines.ToArray());
        }

        public static Task<IEnumerable<T>> OrderedSelect<T>(this IYield yield, IEnumerable<IteratorMethodReturns<T>> methods)
        {
            var coroutines = methods.Select(iterator => yield.Parallel(iterator));
            return yield.OrderedSelectImplementation(coroutines.ToArray());
        }

        public static Task<IEnumerable<T>> OrderedSelect<T>(this IYield yield, params IYieldOperation<T>[] coroutines)
        {
            return yield.OrderedSelectImplementation(coroutines);
        }

        public static Task<T> First<T>(this IYield yield, params IYieldOperation<T>[] operations)
        {
            return yield.First(operations.Select(c => c.GetTaskMethod()).ToArray());
        }

        public static Task<int> First(this IYield yield, params IYieldInstruction[] instructions)
        {
            var coroutines = instructions.Select(iterator => yield.Parallel(iterator.GetTaskMethod())).ToArray();
            return yield.FirstImplementation(coroutines);
        }

        public static Task<T> First<T>(this IYield yield, IEnumerable<IYieldOperation<T>> operations)
        {
            return yield.First(operations.Select(c => c.GetTaskMethod()).ToArray());
        }

        public static Task<int> First(this IYield yield, IEnumerable<IYieldInstruction> instructions)
        {
            var coroutines = instructions.Select(iterator => yield.Parallel(iterator.GetTaskMethod())).ToArray();
            return yield.FirstImplementation(coroutines);
        }

        public static async Task<T> First<T>(this IYield yield, params IteratorMethodReturns<T>[] iterators)
        {
            var coroutines = iterators.Select(iterator => yield.Parallel(iterator)).ToArray();
            var finishedIndex = await yield.FirstImplementation(coroutines);

            return coroutines[finishedIndex].Value;
        }
        public static async Task<T> First<T>(this IYield yield, IEnumerable<IteratorMethodReturns<T>> iterators)
        {
            var coroutines = iterators.Select(iterator => yield.Parallel(iterator)).ToArray();
            var finishedIndex = await yield.FirstImplementation(coroutines);

            return coroutines[finishedIndex].Value;
        }

        public static Task<int> First(this IYield yield, params IteratorMethod[] iterators)
        {
            var coroutines = iterators.Select(iterator => yield.Parallel(iterator)).ToArray();
            return yield.FirstImplementation(coroutines);
        }
        public static Task<int> First(this IYield yield, IEnumerable<IteratorMethod> iterators)
        {
            var coroutines = iterators.Select(iterator => yield.Parallel(iterator)).ToArray();
            return yield.FirstImplementation(coroutines);
        }

        public static Task All(this IYield yield, params ICoroutine[] coroutines)
        {
            return yield.AllSafeImplementation(Array.AsReadOnly(coroutines));
        }

        public static async Task All(this IYield yield, IEnumerable<IYieldInstruction> instructions)
        {
            var exceptions = new List<Exception>();
            var arr = instructions.ToArray();

            var oldExceptionHandler = yield.ExceptionHandler;
            try
            {
                yield.ExceptionHandler = null;

                while (true)
                {
                    try
                    {
                        await yield.Return(Instruction.All(arr));
                        break;
                    }
                    catch (SignalStopException)
                    {
                        throw;
                    }
                    catch (CancellationException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }

                yield.ExceptionHandler = oldExceptionHandler;

                if (exceptions.Count != 0)
                {
                    throw new AggregateException(exceptions);
                }
            }
            catch (SignalStopException)
            {
                yield.ExceptionHandler = oldExceptionHandler;
                throw;
            }
            catch (CancellationException)
            {
                yield.ExceptionHandler = oldExceptionHandler;
                throw;
            }
        }

        private static async Task AllSafeImplementation(this IYield yield, IEnumerable<ICoroutine> instructions)
        {
            var exceptions = new List<Exception>();
            var arr = instructions.ToArray();

            var oldExceptionHandler = yield.ExceptionHandler;
            yield.ExceptionHandler = null;

            while (true)
            {
                try
                {
                    await yield.Return(Instruction.All(arr));
                    break;
                }
                catch (SignalStopException)
                {
                    Finalize();
                    throw;
                }
                catch (CancellationException)
                {
                    Finalize();
                    throw;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            Finalize();

            if (exceptions.Count != 0)
            {
                throw new AggregateException(exceptions);
            }

            void Finalize()
            {
                yield.ExceptionHandler = oldExceptionHandler;
                foreach (var coroutine in arr)
                {
                    coroutine.Dispose();
                }
            }
        }

        /// <summary>
        /// Wait for all instructions to finish. 
        /// If one (or more) will throw an exception, it will still wait for the others to finish (or fail)
        /// and in the end will throw an AggregateException with all failures.
        /// </summary>
        public static Task All(this IYield yield, params IYieldInstruction[] instructions)
        {
            return yield.All(Array.AsReadOnly(instructions));
        }

        /// <summary>
        /// Wait for all instructions to finish. 
        /// If one (or more) will throw an exception, it will still wait for the others to finish (or fail)
        /// and in the end will throw an AggregateException with all failures.
        /// </summary>
        public static Task All(this IYield yield, params IteratorMethod[] methods)
        {
            return yield.AllSafeImplementation(methods.Select((m) => yield.Parallel(m)));
        }

        /// <summary>
        /// Wait for all instructions to finish. 
        /// If one (or more) will throw an exception, it will still wait for the others to finish (or fail)
        /// and in the end will throw an AggregateException with all failures.
        /// </summary>
        public static Task All<T>(this IYield yield, params IteratorMethodReturns<T>[] methods)
        {
            return yield.AllSafeImplementation(methods.Select((m) => yield.Parallel(m)));
        }

        public static IteratorMethod GetTaskMethod(this IYieldInstruction instruction)
        {
            return (y) => InstructionTaskMethod(y, instruction);
        }

        public static IteratorMethodReturns<T> GetTaskMethod<T>(this IYieldOperation<T> operation)
        {
            return (y) => OperationTaskMethod(y, operation);
        }

        private static async Task InstructionTaskMethod(IYield yield, IYieldInstruction instruction)
        {
            await yield.Return(instruction);
        }

        private static async Task<T> OperationTaskMethod<T>(IYield yield, IYieldOperation<T> operation)
        {
            return await yield.Return(operation);
        }

        //public static IteratorMethodReturns<T> Returns<T>(this IYieldInstruction instruction, Func<T> finished)
        //{
        //    return async (yield) =>
        //    {
        //        await yield.Return(instruction);
        //        return finished.Invoke();
        //    };
        //}

        //public static IteratorMethodReturns<T> Returns<T, T1>(this IYieldOperation<T1> instruction, Func<T1, T> finished)
        //{
        //    return async (yield) =>
        //    {
        //        var res = await yield.Return(instruction);
        //        return finished.Invoke(res);
        //    };
        //}

        //public static IteratorMethod Then<T1>(this IYieldOperation<T1> instruction, Action<T1> finished)
        //{
        //    return async (yield) =>
        //    {
        //        var res = await yield.Return(instruction);
        //        finished.Invoke(res);
        //    };
        //}
        //public static IteratorMethod Then<T1>(this IYieldOperation<T1> instruction, IEnumerator<IYieldInstruction> finished)
        //{
        //    return async yield =>
        //    {
        //        await yield.Return(instruction);
        //        await yield.Return(finished.ToInstruction());
        //    };
        //}
        //public static IteratorMethod Then<T1>(this IYieldOperation<T1> instruction, Action finished)
        //{
        //    return async (yield) =>
        //    {
        //        await yield.Return(instruction);
        //        finished.Invoke();
        //    };
        //}

        //public static IteratorMethod Then<T1>(this IYieldOperation<T1> instruction, IteratorMethod finished)
        //{
        //    return async (yield) =>
        //    {
        //        await yield.Return(instruction);
        //        await finished.Invoke(yield);
        //    };
        //}
        //public static IteratorMethod Then<T1>(this IYieldOperation<T1> instruction, Func<IYield, T1, Task> finished)
        //{
        //    return async (yield) =>
        //    {
        //        var res = await yield.Return(instruction);
        //        await finished.Invoke(yield, res);
        //    };
        //}

        //public static IteratorMethod Then(this IYieldInstruction instruction, IYieldInstruction finished)
        //{
        //    return async (yield) =>
        //    {
        //        await yield.Return(instruction);
        //        await yield.Return(finished);
        //    };
        //}

        //public static IteratorMethod Then(this IYieldInstruction instruction, Action finished)
        //{
        //    return async (yield) =>
        //    {
        //        await yield.Return(instruction);
        //        finished.Invoke();
        //    };
        //}
        //public static IteratorMethod Then(this IYieldInstruction instruction, IEnumerator<IYieldInstruction> finished)
        //{
        //    return async (yield) =>
        //    {
        //        await yield.Return(instruction);
        //        await yield.Return(finished.ToInstruction());
        //    };
        //}

        //public static IteratorMethod Then(this IYieldInstruction instruction, IteratorMethod finished)
        //{
        //    return async (yield) =>
        //    {
        //        await yield.Return(instruction);
        //        await finished.Invoke(yield);
        //    };
        //}
        //public static IteratorMethodReturns<T> Then<T>(this IYieldInstruction instruction, IteratorMethodReturns<T> finished)
        //{
        //    return async (yield) =>
        //    {
        //        await yield.Return(instruction);
        //        return await finished.Invoke(yield);
        //    };
        //}
        //public static IteratorMethodReturns<T> Returns<T>(this IYieldInstruction instruction, T finished)
        //{
        //    return async (yield) =>
        //    {
        //        await yield.Return(instruction);
        //        return finished;
        //    };
        //}
        //public static IteratorMethodReturns<T> Returns<T>(this IYieldInstruction instruction, IteratorMethodReturns<T> finished)
        //{
        //    return async (yield) =>
        //    {
        //        await yield.Return(instruction);
        //        return await finished.Invoke(yield);
        //    };
        //}

        public static IteratorMethodReturns<T> Returns<T>(this IteratorMethod task, Func<T> finished)
        {
            return async (yield) =>
            {
                await task.Invoke(yield);
                return finished.Invoke();
            };
        }

        public static IteratorMethod Then(this IteratorMethod task, Action finished)
        {
            return async (yield) =>
            {
                await task.Invoke(yield);
                finished.Invoke();
            };
        }

        public static IteratorMethod Then(this IteratorMethod task, IEnumerator<IYieldInstruction> finished)
        {
            return async (yield) =>
            {
                await task.Invoke(yield);
                await yield.Return(finished.ToInstruction());
            };
        }

        public static IteratorMethod Then(this IteratorMethod task, IYieldInstruction finished)
        {
            return async (yield) =>
            {
                await task.Invoke(yield);
                await yield.Return(finished);
            };
        }

        public static IteratorMethodReturns<T> Returns<T, TA>(this IteratorMethodReturns<TA> task, Func<TA, T> finished)
        {
            return async (yield) =>
            {
                var v = await task.Invoke(yield);
                return finished.Invoke(v);
            };
        }

        public static IteratorMethod Then<T>(this IteratorMethodReturns<T> task, Action<T> finished)
        {
            return async (yield) =>
            {
                var v = await task.Invoke(yield);
                finished.Invoke(v);
            };
        }
        public static IteratorMethod Then<T>(this IteratorMethodReturns<T> task, IteratorMethod finished)
        {
            return async (yield) =>
            {
                await task.Invoke(yield);
                await finished.Invoke(yield);
            };
        }

        public static IteratorMethod Then<T>(this IteratorMethodReturns<T> task, IEnumerator<IYieldInstruction> finished)
        {
            return async (yield) =>
            {
                await task.Invoke(yield);
                await yield.Return(finished.ToInstruction());
            };
        }

        public static IteratorMethodReturns<T> Returns<T>(this IteratorMethod task, T finished)
        {
            return async (yield) =>
            {
                await task.Invoke(yield);
                return finished;
            };
        }

        public static IteratorMethodReturns<T> Returns<TA, T>(this IteratorMethodReturns<TA> task, Func<IYield, TA, Task<T>> finished)
        {
            return async (yield) =>
            {
                var a = await task.Invoke(yield);
                return await finished.Invoke(yield, a);
            };
        }

        public static IteratorMethodReturns<T> Returns<T>(this IteratorMethod task, IteratorMethodReturns<T> finished)
        {
            return async (yield) =>
            {
                await task.Invoke(yield);
                return await finished.Invoke(yield);
            };
        }

        public static IteratorMethodReturns<T> Returns<T>(this IteratorMethod task, IYieldOperation<T> finished)
        {
            return async (yield) =>
            {
                await task.Invoke(yield);
                return await yield.Return(finished);
            };
        }

        public static IteratorMethod Then(this IteratorMethod task, IteratorMethod finished)
        {
            return async (yield) =>
            {
                await task.Invoke(yield);
                await finished.Invoke(yield);
            };
        }

        public static IteratorMethodReturns<T> Returns<TA, T>(this IteratorMethodReturns<TA> task, IteratorMethodReturns<T> finished)
        {
            return async (yield) =>
            {
                await task.Invoke(yield);
                return await finished.Invoke(yield);
            };
        }

        public static IteratorMethodReturns<T> Returns<TA, T>(this IteratorMethodReturns<TA> task, T finished)
        {
            return async (yield) =>
            {
                await task.Invoke(yield);
                var value = finished;
                return value;
            };
        }

        public static IteratorMethodReturns<T> Returns<TA, T>(this IteratorMethodReturns<TA> task, Func<T> finished)
        {
            return async (yield) =>
            {
                await task.Invoke(yield);
                var value = finished.Invoke();
                return value;
            };
        }
        public static IteratorMethodReturns<T> Returns<TA, T>(this IteratorMethodReturns<TA> task, IYieldOperation<T> finished)
        {
            return async (yield) =>
            {
                await task.Invoke(yield);
                return await yield.Return(finished);
            };
        }
    }
}