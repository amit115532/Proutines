namespace Proutines
{
    public class LoopForever : IYieldInstruction
    {
        private readonly IYieldInstruction instruction;

        public LoopForever(IYieldInstruction instruction)
        {
            if (!instruction.Resetable)
                throw new ResetNotSupportedException(instruction);

            this.instruction = instruction;
        }

        public bool Yield()
        {
            if (!instruction.Yield())
            {
                instruction.Reset();
            }

            return true;
        }

        public bool Resetable => true;
        public void Reset()
        {
            instruction.Reset();
        }
    }
}