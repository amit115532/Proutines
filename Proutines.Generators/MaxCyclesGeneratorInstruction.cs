namespace Proutines.Generators
{
    internal class MaxCyclesGeneratorInstruction<T> : IYieldGenerator<T>
    {
        private readonly IYieldGenerator<T> instruction;
        private readonly int maxCycles;

        private int currentCycle;

        public MaxCyclesGeneratorInstruction(IYieldGenerator<T> instruction, int maxCycles)
        {
            if (!instruction.Resetable)
                throw new ResetNotSupportedException(instruction);

            this.instruction = instruction;
            this.maxCycles = maxCycles;
        }

        public T GetValue()
        {
            return instruction.GetValue();
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
            instruction.Reset();
            currentCycle = 0;
        }
    }
}