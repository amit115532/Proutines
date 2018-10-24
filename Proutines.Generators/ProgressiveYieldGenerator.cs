namespace Proutines.Generators
{
    public abstract class ProgressiveYieldGenerator<TRes> : YieldOperationBase<TRes>, IYieldGenerator<double, TRes>
    {
        public abstract double GetValue();
    }

    public abstract class ProgressiveYieldGenerator : IYieldGenerator<double>
    {
        public abstract double GetValue();

        public abstract bool Yield();

        public abstract bool Resetable { get; }
        public abstract void Reset();
    }
}