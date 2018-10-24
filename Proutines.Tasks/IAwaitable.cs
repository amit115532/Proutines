namespace Proutines.Tasks
{
    public interface IAwaitable<out TResult>
    {
        IAwaiter<TResult> GetAwaiter();
    }

    public interface IAwaitable
    {
        IAwaiter GetAwaiter();
    }
}