namespace Proutines
{
    internal class With : IYieldInstruction
    {
        private readonly IYieldInstruction a;
        private readonly IYieldInstruction b;

        public With(IYieldInstruction a, IYieldInstruction b)
        {
            this.a = a;
            this.b = b;
        }

        public bool Yield()
        {
            var aYield = a.Yield();
            var bYield = b.Yield();

            return aYield || bYield;
        }

        public bool Resetable => a.Resetable && b.Resetable;

        public void Reset()
        {
            a.Reset();
            b.Reset();
        }
    }
}