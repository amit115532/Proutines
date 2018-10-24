using System;

namespace Proutines.Generators
{
    internal class SelectYieldGenerator<TIn, TOut> : IYieldGenerator<TOut>
    {
        private readonly IYieldGenerator<TIn> wrapped;
        private readonly Func<TIn, TOut> func;

        public SelectYieldGenerator(IYieldGenerator<TIn> wrapped, Func<TIn, TOut> func)
        {
            this.wrapped = wrapped;
            this.func = func;
        }

        public TOut GetValue()
        {
            return func.Invoke(wrapped.GetValue());
        }

        public bool Yield()
        {
            if (wrapped.Yield())
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