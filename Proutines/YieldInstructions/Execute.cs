using System;
using Logging;

namespace Proutines
{
    public class Execute : IYieldInstruction
    {
        private readonly Action method;
        public IYieldInstruction IterationDelay { get; set; }
        public IYieldInstruction BreakInstruction { get; set; }

        public Execute(Action method)
        {
            this.method = method.AssertNotNull();
            IterationDelay = null;
            BreakInstruction = null;
        }

        public Execute(Action method, IYieldInstruction iterationDelay = null, IYieldInstruction breakInstruction = null)
        {
            if (iterationDelay != null && !iterationDelay.Resetable)
            {
                throw new ResetNotSupportedException(iterationDelay);
            }

            this.method = method.AssertNotNull();
            IterationDelay = iterationDelay;
            BreakInstruction = breakInstruction;
        }

        private void InvokeIteration()
        {
            method.Invoke();
        }

        public bool Yield()
        {
            if (IterationDelay == null)
            {
                InvokeIteration();
            }
            else
            {
                if (!IterationDelay.Yield())
                {
                    InvokeIteration();
                    IterationDelay.Reset();
                    return true;
                }
            }

            if (BreakInstruction == null)
            {
                return true;
            }

            return BreakInstruction.Yield();
        }

        public bool Resetable => BreakInstruction == null || BreakInstruction.Resetable;
        public void Reset()
        {
            IterationDelay?.Reset();
            BreakInstruction?.Reset();
        }
    }
}