using System;

namespace Proutines
{
    internal class WaitForAction<T1, T2, T3> : IYieldInstruction
    {
        private readonly WaitForMultipleActions<T1, T2, T3> baseInstruction;

        public WaitForAction(Action<T1, T2, T3> action)
        {
            baseInstruction = new WaitForMultipleActions<T1, T2, T3>(action, 1);
        }

        public bool Yield()
        {
            return baseInstruction.Yield();
        }

        public bool Resetable => true;

        public void Reset()
        {
            baseInstruction.Reset();
        }
    }

    internal class WaitForAction<T1, T2> : IYieldInstruction
    {
        private readonly WaitForMultipleActions<T1, T2> baseInstruction;

        public WaitForAction(Action<T1, T2> action)
        {
            baseInstruction = new WaitForMultipleActions<T1, T2>(action, 1);
        }

        public bool Yield()
        {
            return baseInstruction.Yield();
        }

        public bool Resetable => true;

        public void Reset()
        {
            baseInstruction.Reset();
        }
    }

    internal class WaitForAction<T> : IYieldOperation<T>
    {
        private readonly WaitForMultipleActions<T> baseInstruction;

        public WaitForAction(Action<T> action)
        {
            baseInstruction = new WaitForMultipleActions<T>(action, 1);
        }

        public bool Yield()
        {
            return baseInstruction.Yield();
        }

        public bool Resetable => true;

        public void Reset()
        {
            baseInstruction.Reset();
        }

        public T Value => baseInstruction.Value;
        public bool IsDone => baseInstruction.IsDone;
    }

    internal class WaitForAction : IYieldInstruction
    {
        private readonly WaitForMultipleActions baseInstruction;

        public WaitForAction(Action action)
        {
            baseInstruction = new WaitForMultipleActions(action, 1);
        }

        public bool Yield()
        {
            return baseInstruction.Yield();
        }

        public bool Resetable => true;

        public void Reset()
        {
            baseInstruction.Reset();
        }
    }
}