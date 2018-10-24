using System;
// ReSharper disable NotAccessedField.Local
// ReSharper disable DelegateSubtraction

namespace Proutines
{
    internal class WaitForMultipleActions<T1, T2, T3> : IYieldInstruction
    {
        private readonly int initialCount;
        private int raiseCountLeft;
        private bool isSubscribed;
        Action<T1, T2, T3> action;

        public WaitForMultipleActions(Action<T1, T2, T3> action, int count)
        {
            initialCount = count;
            this.action = action;
            raiseCountLeft = count;
        }

        private void OnEvent(T1 a, T2 b, T3 c)
        {
            --raiseCountLeft;
        }

        private void Subscribe()
        {
            if (isSubscribed)
                return;

            action += OnEvent;
            isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!isSubscribed)
                return;

            action -= OnEvent;
            isSubscribed = false;
        }

        public bool Yield()
        {
            var yield = raiseCountLeft > 0;

            if (!yield)
            {
                Unsubscribe();
                return false;
            }

            return true;
        }

        public bool Resetable => true;

        public void Reset()
        {
            Unsubscribe();
            Subscribe();
            raiseCountLeft = initialCount;
        }
    }

    internal class WaitForMultipleActions<T1, T2> : IYieldInstruction
    {
        private readonly int initialCount;
        private int raiseCountLeft;
        private bool isSubscribed;
        Action<T1, T2> action;

        public WaitForMultipleActions(Action<T1, T2> action, int count)
        {
            initialCount = count;
            this.action = action;
            raiseCountLeft = count;
        }

        private void OnEvent(T1 a, T2 b)
        {
            --raiseCountLeft;
        }

        private void Subscribe()
        {
            if (isSubscribed)
                return;

            action += OnEvent;
            isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!isSubscribed)
                return;

            action -= OnEvent;
            isSubscribed = false;
        }

        public bool Yield()
        {
            var yield = raiseCountLeft > 0;

            if (!yield)
            {
                Unsubscribe();
                return false;
            }

            return true;
        }

        public bool Resetable => true;

        public void Reset()
        {
            Unsubscribe();
            Subscribe();
            raiseCountLeft = initialCount;
        }
    }
    internal class WaitForMultipleActions<T1> : IYieldOperation<T1>
    {
        private readonly int initialCount;
        private int raiseCountLeft;
        private bool isSubscribed;
        private Action<T1> action;

        public WaitForMultipleActions(Action<T1> action, int count)
        {
            initialCount = count;
            this.action = action;
            raiseCountLeft = count;
        }

        private void OnEvent(T1 a)
        {
            --raiseCountLeft;
        }

        private void Subscribe()
        {
            if (isSubscribed)
                return;

            action += OnEvent;
            isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!isSubscribed)
                return;

            action -= OnEvent;
            isSubscribed = false;
        }

        public bool Yield()
        {
            var yield = raiseCountLeft > 0;

            if (!yield)
            {
                Unsubscribe();
                IsDone = true;
                return false;
            }

            return true;
        }

        public bool Resetable => true;

        public void Reset()
        {
            Unsubscribe();
            Subscribe();
            IsDone = false;
            raiseCountLeft = initialCount;
        }

        public T1 Value { get; private set; }
        public bool IsDone { get; private set; }
    }

    internal class WaitForMultipleActions : IYieldInstruction
    {
        private readonly int initialCount;
        private int raiseCountLeft;
        private bool isSubscribed;
        Action action;

        public WaitForMultipleActions(Action action, int count)
        {
            initialCount = count;
            this.action = action;
            raiseCountLeft = count;
        }

        private void OnEvent()
        {
            --raiseCountLeft;
        }

        private void Subscribe()
        {
            if (isSubscribed)
                return;

            action += OnEvent;
            isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!isSubscribed)
                return;

            action -= OnEvent;
            isSubscribed = false;
        }

        public bool Yield()
        {
            var yield = raiseCountLeft > 0;

            if (!yield)
            {
                Unsubscribe();
                return false;
            }

            return true;
        }

        public bool Resetable => true;

        public void Reset()
        {
            Unsubscribe();
            Subscribe();
            raiseCountLeft = initialCount;
        }
    }
}