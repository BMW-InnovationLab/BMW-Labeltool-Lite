namespace Rcv.LabelTool.Contracts.Services
{
    /// <summary>
    /// Service for random number creation.
    /// </summary>
    public interface IRandomService
    {
        /// <summary>
        /// Get next random number between 0 and maxValue.
        /// </summary>
        /// <param name="maxValue">Max value to get (exclusive).</param>
        /// <returns>Random number</returns>
        int Next(int maxValue);

        /// <summary>
        /// Get next random number between minValue and maxValue.
        /// </summary>
        /// <param name="minValue">Min value to get (inclusive).</param>
        /// <param name="maxValue">Max value to get (exclusive).</param>
        /// <returns>Random number</returns>
        int Next(int minValue, int maxValue);

        /// <summary>
        /// Get next random number between minValue and maxValue.
        /// </summary>
        /// <param name="minValue">Min value to get (inclusive).</param>
        /// <param name="maxValue">Max value to get (exclusive).</param>
        /// <returns>Random number</returns>
        double Next(double minValue, double maxValue);
    }
}
