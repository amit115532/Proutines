using System;

namespace Proutines
{
    /// <summary>
    /// Sometimes its helpful to wait until a certain value is returned 
    /// such as, yield until a character is dead, yield until health is reduced
    /// </summary>
    /// <typeparam name="T">The value to compare</typeparam>
    public class WaitForMethodValue<T> : IYieldInstruction
    {
        /// <summary>
        /// The value to check on
        /// </summary>
        private readonly T value;
        /// <summary>
        /// The method to check its returned value
        /// </summary>
        private readonly Func<T> method;

        /// <summary>
        /// Wait until a certain value is returned
        /// </summary>
        /// <param name="method">The method to check its returned value</param>
        /// <param name="value">The value to check on</param>
        public WaitForMethodValue(Func<T> method, T value)
        {
            this.value = value;
            this.method = method;
        }

        /// <summary>
        /// This method will return true if the coroutine should yield
        /// </summary>
        /// <returns>True if the coroutine should yield, otherwise False</returns>
        public bool Yield()
        {
            return !method.Invoke().Equals(value);
        }

        public bool Resetable => true;

        public void Reset()
        {
            // Does nothing
        }
    }
}