using System;

namespace Proutines
{
    public class WaitForEvent<T> : IYieldOperation<T>
    {
        private readonly Action<Action<T>> subscribe;
        private readonly Action<Action<T>> unsubscribe;

        private bool isSubscribed;
        private volatile bool isCalled;

        public WaitForEvent(Action<Action<T>> subscribe, Action<Action<T>> unsubscribe)
        {
            this.subscribe = subscribe;
            this.unsubscribe = unsubscribe;

            Subscribe();
        }

        public bool Yield()
        {
            return !isCalled;
        }

        private void Method(T value)
        {
            Value = value;
            Unsubscribe();

            isCalled = true;
        }

        public void Reset()
        {
            isCalled = false;
            Subscribe();
        }

        private void Subscribe()
        {
            if (isSubscribed)
                return;

            subscribe.Invoke(Method);
            isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!isSubscribed)
                return;

            unsubscribe.Invoke(Method);
            isSubscribed = false;
        }

        public bool Resetable => true;
        public T Value { get; private set; }
        public bool IsDone => isCalled;
    }

    public class WaitForEvent : IYieldInstruction
    {
        private readonly Action<Action> subscribe;
        private readonly Action<Action> unsubscribe;

        private bool isSubscribed;
        private bool isCalled;

        public WaitForEvent(Action<Action> subscribe, Action<Action> unsubscribe)
        {
            this.subscribe = subscribe;
            this.unsubscribe = unsubscribe;

            Subscribe();
        }

        public bool Yield()
        {
            return !isCalled;
        }

        private void Method()
        {
            isCalled = true;
            Unsubscribe();
        }

        public void Reset()
        {
            isCalled = false;
            Subscribe();
        }

        private void Subscribe()
        {
            if (isSubscribed)
                return;

            subscribe.Invoke(Method);
            isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!isSubscribed)
                return;

            unsubscribe.Invoke(Method);
            isSubscribed = false;
        }

        public bool Resetable => true;
    }
}