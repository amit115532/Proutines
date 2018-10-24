namespace Proutines
{
    internal class MaxCyclesInstruction : IYieldInstruction
    {
        private readonly IYieldInstruction instruction;
        private readonly int maxCycles;

        private int currentCycle;

        public MaxCyclesInstruction(IYieldInstruction instruction, int maxCycles)
        {
            if (!instruction.Resetable)
            {
                throw new ResetNotSupportedException(instruction);
            }

            this.instruction = instruction;
            this.maxCycles = maxCycles;
        }

        public bool Yield()
        {
            if (!instruction.Yield())
            {
                currentCycle++;

                if (currentCycle == maxCycles)
                {
                    return false;
                }

                instruction.Reset();
            }

            return true;
        }

        public bool Resetable => true;

        public void Reset()
        {
            currentCycle = 0;
            instruction.Reset();
        }
    }
}