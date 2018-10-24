namespace Proutines.Generators
{
    public interface IYieldGenerator<out TGen, out TRes> : IYieldOperation<TRes>, IYieldGenerator<TGen>
    {
        // Left blank intentionally
    }

    public interface IYieldGenerator<out T> : IYieldInstruction
    {
        T GetValue();
    }
}
