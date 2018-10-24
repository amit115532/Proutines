using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Logging;

namespace Proutines.Tasks
{
    public delegate Task<T> IteratorMethodReturns<T>(IYield yield);

    public delegate Task IteratorMethod(IYield yield);

    internal class Yield : IYield
    {
        private Action continuation;
        private Task task;
        private IYieldInstruction nextInstruction;
        private readonly ICoroutinesExecutor executor;
        private readonly List<ICoroutine> children = new List<ICoroutine>();
        private bool disposed;
        private Exception externalException;
        private IYieldInstruction current;
        private bool isFirstInstruction = true;
        private int moveNextCount;
        private Exception lastTriedHandledException;
        private bool isResulted = true;

        public ICoroutine Coroutine { get; set; }

        public Yield(IteratorMethod method, ICoroutinesExecutor executor, Action<Exception> onException = null)
        {
            this.executor = executor;
            ExceptionHandler = onException;

            task = method.Invoke(this);
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            foreach (var child in children)
            {
                child.Dispose();
            }

            Disposed?.Invoke();

            if (task != null)
            {
                externalException = new CancellationException(CancelationReason.Interrupted);
            }
        }

        private void SignalInstructionException(Exception exception)
        {
            if (!disposed)
            {
                externalException = new CancellationException(CancelationReason.InstructionException, exception);
                MoveNext();
            }
        }

        private bool Execute()
        {
            if (continuation == null)
                return false;

            var t = continuation;
            continuation = null;
            t();

            return true;
        }

        public bool MoveNext()
        {
            if (task == null)
                return false;

            if (isFirstInstruction)
            {
                ++moveNextCount;
                if (moveNextCount > 1)
                {
                    isFirstInstruction = false;
                }
                else
                {
                    return true;
                }
            }

            ManageException(GetInnerExceptionOfTask());

            if (nextInstruction == null)
            {
                if (Execute())
                {
                    ManageException(GetInnerExceptionOfTask());
                    ManageException(externalException);
                }
            }

            if (nextInstruction != null)
            {
                Current = nextInstruction;
                nextInstruction = null;
                return true;
            }

            if (continuation != null)
            {
                return true;
            }

            task = null;
            Dispose();

            return false;
        }

        private void ManageException(Exception exception)
        {
            if (exception != null)
            {
                var isCancelation = exception is CancellationException;

                if (isCancelation)
                {
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }

                if (ExceptionHandler != null)
                {
                    ExceptionHandler.Invoke(exception);
                }
                else
                {
                    if (lastTriedHandledException == exception)
                    {
                        // This is the second time this exception is being handled... dispose
                        task = null;
                        continuation = null;
                        nextInstruction = null;

                        LogUtils.Log($"Unhandled coroutine exception: {exception.Message}\n{exception.StackTrace}", LogMessageType.Error);
                        Coroutine?.Dispose();
                    }
                    else
                    {
                        lastTriedHandledException = exception;
                        ExceptionDispatchInfo.Capture(exception).Throw();
                    }
                }
            }
        }

        private Exception GetInnerExceptionOfTask()
        {
            if (task?.Exception == null)
                return null;

            Exception inner = task.Exception;
            while (inner is AggregateException)
                inner = inner.InnerException;

            if (inner is CancellationException)
            {
                return null;
            }

            exceptionFromTask = inner;
            return inner;
        }

        public void Reset()
        {
        }

        public IYieldInstruction Current
        {
            get
            {
                if (isFirstInstruction)
                {
                    if (moveNextCount == 0)
                    {
                        return null;
                    }

                    return new SetInterruptMethod((reason, exception) =>
                    {
                        if (reason == InterruptReason.Exception)
                        {
                            if (exception == exceptionFromTask)
                            {
                                // Its the same exception from task...
                                task = null;
                                continuation = null;
                                nextInstruction = null;

                                Coroutine?.Dispose();

                                return;
                            }

                            var currentInstructionType = "unknown";
                            if (current != null)
                                currentInstructionType = current.GetType().Name;

                            LogUtils.Log(
                                $"An exception has been thrown from yield instruction of type: {currentInstructionType}. It is strongly recommended not to let code inside Yield function in YieldInstructions throw exceptions.",
                                LogMessageType.Warning);

                            SignalInstructionException(exception);
                        }
                        else
                        {
                            Coroutine?.Dispose();
                        }
                    }, InvokeOption.Interrupt, false);
                }

                if (externalException != null)
                    return null; return current;
            }
            private set => current = value;
        }

        object IEnumerator.Current => Current;

        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }

        public bool IsCompleted => false;

        /// <summary>
        /// Called when await finishes.
        /// This is in the context of the async method !!! VERY USEFUL !!!
        /// </summary>
        public void GetResult()
        {
            isResulted = true;
            isGoingToAwait = false;

            if (externalException != null)
            {
                var ex = externalException;
                externalException = null;
                ManageException(ex);
            }
        }

        public IAwaiter GetAwaiter()
        {
            isGoingToAwait = false;

            if (!isResulted)
            {
                var frame = new StackTrace().GetFrame(3);

                var fileName = frame.GetFileName();
                var line = frame.GetFileLineNumber();
                var locationStr = fileName == null ? $"line {line}" : $" file {fileName}.cs:({line})";
                var declaringType = frame.GetMethod().DeclaringType;

                var typeStr = declaringType == null ? "UNKNOWN_DECLARING_TYPE" : declaringType.Name;
                var str = $"{typeStr}:{frame.GetMethod().Name} at {locationStr}";

                LogUtils.Log($"You are awaiting on the same yield object in parallel, This is a no op. the previous await instruction will never be executed. Please check the method: {str}", LogMessageType.Warning);
            }

            isResulted = false;

            return this;
        }

        public event Action Disposed;

        private bool isGoingToAwait = false;

        public IYield Return(IYieldInstruction instruction)
        {
            if(isGoingToAwait)
            {
                var frame = new StackTrace().GetFrame(3);

                var fileName = frame.GetFileName();
                var line = frame.GetFileLineNumber();
                var locationStr = fileName == null ? $"line {line}" : $" file {fileName}.cs:({line})";
                var declaringType = frame.GetMethod().DeclaringType;

                var typeStr = declaringType == null ? "UNKNOWN_DECLARING_TYPE" : declaringType.Name;
                var str = $"{typeStr}:{frame.GetMethod().Name} at {locationStr}";

                LogUtils.Log($"You are calling yield.Return without awaiting, This is a no op. the instruction will never be executed and not wait will occure. Please check the method: {str}", LogMessageType.Warning);

            }

            isGoingToAwait = true;
            nextInstruction = instruction;
            return this;
        }

        public IYield<T> Return<T>(IYieldOperation<T> operation)
        {
            if (isGoingToAwait)
            {
                var frame = new StackTrace().GetFrame(3);

                var fileName = frame.GetFileName();
                var line = frame.GetFileLineNumber();
                var locationStr = fileName == null ? $"line {line}" : $" file {fileName}.cs:({line})";
                var declaringType = frame.GetMethod().DeclaringType;

                var typeStr = declaringType == null ? "UNKNOWN_DECLARING_TYPE" : declaringType.Name;
                var str = $"{typeStr}:{frame.GetMethod().Name} at {locationStr}";

                LogUtils.Log($"You are calling yield.Return without awaiting, This is a no op. the instruction will never be executed and not wait will occure. Please check the method: {str}", LogMessageType.Warning);
            }

            isGoingToAwait = true;
            nextInstruction = operation;
            return new Yield<T>(this, operation);
        }

        public ICoroutine Parallel(IteratorMethod method, ParallalOption option = ParallalOption.AsChild)
        {
            if (option == ParallalOption.AsChild)
            {
                var coroutine = executor.StartTask(method, OnChildException);
                children.Add(coroutine);
                return coroutine;
            }

            return executor.StartTask(method);
        }

        private void OnChildException(Exception childException)
        {
            externalException = childException;
        }

        public ICoroutine<T> Parallel<T>(IteratorMethodReturns<T> method, ParallalOption option = ParallalOption.AsChild)
        {
            if (option == ParallalOption.AsChild)
            {
                var coroutine = executor.StartTask(method, OnChildException);
                children.Add(coroutine);
                return coroutine;
            }

            return executor.StartTask(method);
        }

        public bool HasException => externalException != null || exceptionFromTask != null;
        public Action<Exception> ExceptionHandler { get; set; }

        private Exception exceptionFromTask;
    }

    internal class Yield<T> : IYield<T>
    {
        #region members
        private readonly TaskCompletionSource<T> task;
        public Task<T> Task => task.Task;

        private readonly IYieldOperation<T> yieldOperation;
        private readonly IYield @yield;
        #endregion
        #region constructors
        public Yield(IYield @yield, IYieldOperation<T> yieldOperation)
        {
            this.yield = yield;
            this.yieldOperation = yieldOperation;

            task = new TaskCompletionSource<T>();
        }
        #endregion

        public void Dispose()
        {
            yield.Dispose();
        }

        public bool MoveNext()
        {
            return yield.MoveNext();
        }

        public void Reset()
        {
            yield.Reset();
        }

        public T GetResult()
        {
            yield.GetResult();
            task.SetResult(yieldOperation.Value);
            return yieldOperation.Value;
        }

        public IAwaiter<T> GetAwaiter()
        {
            return this;
        }

        object IEnumerator.Current => yield.Current;

        public IYieldInstruction Current => yield.Current;
        public void OnCompleted(Action continuation)
        {
            yield.OnCompleted(continuation);
        }
        public bool IsCompleted => false;
    }
}