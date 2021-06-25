namespace Rcv.LabelTool.Contracts.Models
{
    /// <summary>
    /// Class for rect represenation.
    /// </summary>
    public class Rect
    {
        /// <summary>
        /// Distance of rect to left in pixels.
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Distance of rect to top in pixels.
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        /// Width of rect in pixels.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of rect in pixels.
        /// </summary>
        public int Height { get; set; }
    }
}
