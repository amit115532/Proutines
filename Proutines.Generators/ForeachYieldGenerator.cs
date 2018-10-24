using System;

namespace Proutines.Generators
{
    internal class ForeachYieldGenerator<T> : IYieldGenerator<T>
    {
        private readonly IYieldGenerator<T> wrapped;
        private readonly Action<T> func;

        public ForeachYieldGenerator(IYieldGenerator<T> wrapped, Action<T> func)
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
            func.Invoke(wrapped.GetValue());

            if (value)
            {
                return true;
            }

            return false;
        }

        public bool Resetable => wrapped.Resetable;
        public void Reset()
        {
            wrapped.Reset();
        }
    }
}