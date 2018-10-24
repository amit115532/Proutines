namespace Proutines
{
    /// <summary>
    /// This interface is created due to the different types of time providers in many systems. 
    /// And the need to make a standart
    /// </summary>
    public interface ITimeProvider
    {
        /// <summary>
        /// Get the current time of the system
        /// </summary>
        /// <returns>The current time of the system</returns>
        double GetCurrentTime();
    }
}