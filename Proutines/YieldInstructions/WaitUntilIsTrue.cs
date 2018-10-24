using System;

namespace Proutines
{
    /// <summary>
    /// Yield until an expression returns true
    /// </summary>
    public class WaitUntilIsTrue : IYieldInstruction
    {
        /// <summary>
        /// The expression to check if true
        /// </summary>
        private readonly Func<bool> expression;

        /// <summary>
        /// Yield until an expression returns true
        /// </summary>
        /// <param name="expression">The expression to check if true</param>
        public WaitUntilIsTrue(Func<bool> expression)
        {
            this.expression = expression;
        }

        /// <summary>
        /// This method will return true if the coroutine should yield
        /// </summary>
        /// <returns>True if the coroutine should yield, otherwise False</returns>
        public bool Yield()
        {
            return !expression.Invoke();
        }

        public bool Resetable => true;

        public void Reset()
        {
            // does nothing
        }
    }
}