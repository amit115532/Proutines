namespace Proutines.Generators
{
    internal class Lerpable : ILerpGenerator
    {
        private readonly IYieldGenerator<double> lerpable;

        public Lerpable(IYieldGenerator<double> lerpable)
        {
            this.lerpable = lerpable;
        }

        public double GetValue()
        {
            return lerpable.GetValue();
        }

        public bool Yield()
        {
            return lerpable.Yield();
        }

        public void Reset()
        {
            lerpable.Reset();
        }

        public bool Resetable => lerpable.Resetable;
    }
}