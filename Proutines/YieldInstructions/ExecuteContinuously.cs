using System;

namespace Proutines
{
    /// <summary>
    /// Execute a method every iteration until a yield instruction ends
    /// </summary>
    public class ExecuteContinuously : IYieldInstruction
    {
        /// <summary>
        /// The method to execute every iteration
        /// </summary>
        private readonly Action method;

        /// <summary>
        /// The yield instruction to be checked every iteration
        /// </summary>
        public IYieldInstruction Until;

        /// <summary>
        /// Execute a method every iteration until a yield instruction ends
        /// </summary>
        /// <param name="method">The method to be executed every iteration</param>
        /// <param name="until">The yield instruction to be checked every iteration, will be forever if null</param>
        public ExecuteContinuously(Action method, IYieldInstruction until = null)
        {
            if (until == null)
                until = new Forever();

            this.method = method;
            Until = until;
        }

        public bool Yield()
        {
            method();
            return Until.Yield();
        }

        public bool Resetable => Until == null || Until.Resetable;
        public void Reset()
        {
            Until?.Reset();
        }
    }
}