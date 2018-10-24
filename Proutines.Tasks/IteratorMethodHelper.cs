namespace Proutines.Tasks
{
    public static class IteratorMethodHelper
    {
        public static IteratorMethod Wrap(IteratorMethod method)
        {
            return method;
        }

        public static IteratorMethodReturns<T> Wrap<T>(IteratorMethodReturns<T> method)
        {
            return method;
        }
    }
}