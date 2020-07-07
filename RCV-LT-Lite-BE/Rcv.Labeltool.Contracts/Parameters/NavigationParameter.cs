namespace Rcv.LabelTool.Contracts.Parameters
{
    /// <summary>
    /// Parameter structure for navigation through labeltool dataset.
    /// </summary>
    public class NavigationParameter
    {
        /// <summary>
        /// Index of navigation. Index of elements is 0-based.
        /// First image will be requested with index of 0.
        /// </summary>
        public int Index { get; set; } = 0;
    }
}
