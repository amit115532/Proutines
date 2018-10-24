using System.Collections.Generic;

namespace Proutines.Generators
{
    internal class SequenceGenerator<T> : IYieldGenerator<T>
    {
        private readonly IEnumerator<T> seq;
        private bool wasInitialized;

        public SequenceGenerator(IEnumerable<T> seq)
        {
            this.seq = seq.GetEnumerator();
        }

        public T GetValue()
        {
            if (!wasInitialized)
            {
                wasInitialized = true;
                seq.MoveNext();
            }

            return seq.Current;
        }

        public bool Yield()
        {
            wasInitialized = true;
            return seq.MoveNext();
        }

        public void Reset()
        {
            wasInitialized = false;
            seq.Reset();
        }

        public bool Resetable => true;
    }
}