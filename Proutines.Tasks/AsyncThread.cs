using System;
using System.Threading;
using System.Threading.Tasks;

namespace Proutines.Tasks
{
    internal class AsyncThread : IYieldInstruction
    {
        private bool isSubscribedToYieldDisposed;
        private readonly Task task;
        private readonly CancellationTokenSource cancellationToken;
        private readonly IYield yield;

        internal AsyncThread(IYield yield, Action<CancellationToken> task)
        {
            this.yield = yield;
            cancellationToken = new CancellationTokenSource();
            SubscribeToYieldDisposed(yield);

            this.task = Task.Run(() =>
            {
                task.Invoke(cancellationToken.Token);
            }, cancellationToken.Token);
        }

        internal AsyncThread(IYield yield, Func<CancellationToken, Task> task)
        {
            this.yield = yield;
            cancellationToken = new CancellationTokenSource();
            SubscribeToYieldDisposed(yield);

            this.task = Task.Run(() =>
            {
                task.Invoke(cancellationToken.Token);
            }, cancellationToken.Token);
        }

        /// <summary>
        /// Using this overload is not recommended. no cancellation token available.
        /// Please use Task coroutines and use yield.ThreadParallel instead
        /// </summary>
        public AsyncThread(Action task)
        {
            this.task = Task.Run(task);
        }

        private void SubscribeToYieldDisposed(IYield yield)
        {
            if (!isSubscribedToYieldDisposed)
            {
                isSubscribedToYieldDisposed = true;

                yield.Disposed += OnYieldDisposed;
            }
        }
        private void OnYieldDisposed()
        {
            if (!IsFinished())
            {
                UnsubscribeFromYieldDisposed(yield);
                cancellationToken.Cancel();
            }
        }

        private void UnsubscribeFromYieldDisposed(IYield yield)
        {
            if (isSubscribedToYieldDisposed)
            {
                isSubscribedToYieldDisposed = false;

                yield.Disposed -= OnYieldDisposed;
            }
        }

        private bool IsFinished()
        {
            return (task.IsCanceled || task.IsCompleted || task.IsFaulted);
        }

        public bool Yield()
        {
            if (IsFinished())
            {
                if (yield != null)
                    UnsubscribeFromYieldDisposed(yield);

                if (task.IsCanceled)
                {
                    throw new CancellationException(CancelationReason.Interrupted);
                }

                if (task.IsFaulted)
                {
                    if (task.Exception != null)
                        throw task.Exception;

                    throw new Exception("Task faulted");
                }

                return false;
            }

            return true;
        }

        public bool Resetable => false;
        public void Reset()
        {
            throw new ResetNotSupportedException(this);
        }
    }

    internal class AsyncThread<T> : IYieldOperation<T>
    {
        private bool isSubscribedToYieldDisposed;

        public T Value
        {
            get
            {
                if (task.IsCanceled)
                {
                    throw new CancellationException(CancelationReason.Interrupted);
                }

                if (task.IsFaulted)
                {
                    if (task.Exception != null)
                        throw task.Exception;

                    throw new Exception("Task faulted");
                }

                if (task.IsCompleted)
                {
                    return task.Result;
                }

                throw new OperationNotReadyException();
            }
        }

        private readonly Task<T> task;
        private readonly CancellationTokenSource cancellationToken;
        private readonly IYield yield;

        internal AsyncThread(IYield yield, Func<CancellationToken, T> task)
        {
            this.yield = yield;
            SubscribeToYieldDisposed(yield);
            cancellationToken = new CancellationTokenSource();

            this.task = Task.Run(() => task.Invoke(cancellationToken.Token), cancellationToken.Token);
        }

        /// <summary>
        /// Using this overload is not recommended. no cancellation token available.
        /// Please use Task coroutines and use yield.ThreadParallel instead
        /// </summary>
        public AsyncThread(Func<T> task)
        {
            cancellationToken = new CancellationTokenSource();

            this.task = Task.Run(() => task.Invoke());
        }

        public AsyncThread(IYield yield, Func<CancellationToken, Task<T>> task)
        {
            this.yield = yield;
            SubscribeToYieldDisposed(yield);
            cancellationToken = new CancellationTokenSource();

            this.task = Task.Run(() => task.Invoke(cancellationToken.Token), cancellationToken.Token);
        }

        private void SubscribeToYieldDisposed(IYield yield)
        {
            if (!isSubscribedToYieldDisposed)
            {
                isSubscribedToYieldDisposed = true;

                yield.Disposed += OnYieldDisposed;
            }
        }

        private void UnsubscribeFromYieldDisposed(IYield yield)
        {
            if (isSubscribedToYieldDisposed)
            {
                isSubscribedToYieldDisposed = false;

                yield.Disposed -= OnYieldDisposed;
            }
        }

        private void OnYieldDisposed()
        {
            if (!IsFinished())
            {
                UnsubscribeFromYieldDisposed(yield);
                cancellationToken.Cancel();
            }
        }

        public bool Yield()
        {
            var yield = !IsFinished();

            if (yield == false)
            {
                if (this.yield != null)
                    UnsubscribeFromYieldDisposed(this.yield);
            }

            return yield;
        }

        public bool Resetable => false;
        public void Reset()
        {
            throw new ResetNotSupportedException(this);
        }

        private bool IsFinished()
        {
            return (task.IsCanceled || task.IsCompleted || task.IsFaulted);
        }

        public bool IsDone => !Yield();
    }
}