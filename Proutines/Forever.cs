namespace Proutines
{
    public class Forever : IYieldInstruction
    {
        public bool Yield()
        {
            return true;
        }

        public bool Resetable => true;

        public void Reset()
        {
            // Left blank intentionlly
        }
    }
}