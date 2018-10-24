namespace Proutines
{
    /// <summary>
    /// A common use for a manual yield operation is as a parameter of a coroutine that will
    /// manualy set done when ready
    /// </summary>
    public class ManualYieldInstruction : IYieldInstruction
    {
        private bool isDone;

        public void SetDone()
        {
            isDone = true;
        }

        public bool Yield()
        {
            return !isDone;
        }

        public bool Resetable => false;
        public void Reset()
        {
            throw new ResetNotSupportedException(this);
        }
    }
}