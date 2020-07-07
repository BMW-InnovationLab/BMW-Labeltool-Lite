namespace Robotron.Imagine.Transformation
{
    /// <summary>
    /// Transformation parameters for FILL-transformation.
    /// </summary>
    public class Fill
    {
        /// <summary>
        /// Color to fill areas which are currently not filled.
        /// Color will be provided as #RRGGBB
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Optional height of target image.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Optional widht of target image.
        /// </summary>
        public int? Width { get; set; }
    }
}
