namespace Proutines
{
    public class Never : IYieldInstruction
    {
        public bool Yield()
        {
            return false;
        }

        public bool Resetable => true;

        public void Reset()
        {
            // Left blank intentionlly
        }
    }
}