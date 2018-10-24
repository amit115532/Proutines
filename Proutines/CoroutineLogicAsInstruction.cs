namespace Proutines
{
    internal class CoroutineLogicAsInstruction : IYieldInstruction
    {
        private readonly ICoroutineLogic coroutineLogic;

        public CoroutineLogicAsInstruction(ICoroutineLogic coroutineLogic)
        {
            this.coroutineLogic = coroutineLogic;
        }

        public bool Yield()
        {
            return coroutineLogic.ExecuteCurrentInstruction();
        }

        public bool Resetable => false;
        public void Reset()
        {
            throw new ResetNotSupportedException(this);
        }
    }
}