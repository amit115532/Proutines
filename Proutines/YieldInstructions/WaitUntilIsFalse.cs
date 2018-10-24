using System;

namespace Proutines
{
    /// <summary>
    /// Yield until an expression returns false
    /// </summary>
    public class WaitUntilIsFalse : IYieldInstruction
    {
        /// <summary>
        /// The expression to check if false
        /// </summary>
        private readonly Func<bool> expression;

        /// <summary>
        /// Yield until an expression returns false
        /// </summary>
        /// <param name="expression">The expression to check if false</param>
        public WaitUntilIsFalse(Func<bool> expression)
        {
            this.expression = expression;
        }

        /// <summary>
        /// This method will return true if the coroutine should yield
        /// </summary>
        /// <returns>True if the coroutine should yield, otherwise False</returns>
        public bool Yield()
        {
            return expression.Invoke();
        }

        public bool Resetable => true;

        public void Reset()
        {
            // Does nothing
        }
    }
}