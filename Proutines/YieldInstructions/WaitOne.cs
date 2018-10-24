namespace Proutines
{
    /// <summary>
    /// Wait a frame before moving on 
    /// yield return null is an optimized version
    /// of
    /// yield return new WaitFrame();
    /// </summary>
    public class WaitOne : IYieldInstruction
    {
        /// <summary>
        /// This method will return true if the coroutine should yield
        /// </summary>
        /// <returns>True if the coroutine should yield, otherwise False</returns>
        public bool Yield()
        {
            return false;
        }

        public bool Resetable => true;

        public void Reset()
        {
            // Does nothing
        }
    }
}