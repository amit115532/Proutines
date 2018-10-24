using System;

namespace Proutines.Generators
{
    internal class OnChangeYieldGenerator<T> : IYieldGenerator<T>
    {
        private readonly IYieldGenerator<T> wrapped;
        private readonly Action<T> func;
        private T lastValue;
        private bool isStart = true;

        public OnChangeYieldGenerator(IYieldGenerator<T> wrapped, Action<T> func)
        {
            this.wrapped = wrapped;
            this.func = func;
        }

        public T GetValue()
        {
            return wrapped.GetValue();
        }

        public bool Yield()
        {
            var value = (wrapped.Yield());
            var currentValue = wrapped.GetValue();
            if (isStart || !currentValue.Equals(lastValue))
            {
                isStart = false;
                lastValue = currentValue;
                func.Invoke(currentValue);
            }

            if (value)
            {
                return true;
            }

            return false;
        }

        public bool Resetable => wrapped.Resetable;
        public void Reset()
        {
            isStart = true;
            wrapped.Reset();
        }
    }
}