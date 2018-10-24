using System;

namespace Proutines.Generators
{
    internal class CustomGenerator<T> : IYieldGenerator<T>
    {
        private readonly Func<T> func;

        public CustomGenerator(Func<T> func)
        {
            this.func = func;
        }

        public T GetValue()
        {
            return func.Invoke();
        }

        public bool Yield()
        {
            return false;
        }

        public void Reset()
        {
            // Left blank intentionally
        }

        public bool Resetable => true;
    }
}