namespace Proutines
{
    internal class MinimumTime : IYieldOperation<bool>
    {
        private readonly Or or;

        public MinimumTime(IYieldInstruction instruction, WaitForSeconds waitForSeconds)
        {
            or = new Or(instruction, waitForSeconds);
        }

        public bool Yield()
        {
            return or.Yield();
        }

        public bool Resetable => or.Resetable;
        public void Reset()
        {
            ((IYieldInstruction)or).Reset();
        }

        public bool Value => or.Value == OrResult.First;
        public bool IsDone => or.IsDone;
    }
}