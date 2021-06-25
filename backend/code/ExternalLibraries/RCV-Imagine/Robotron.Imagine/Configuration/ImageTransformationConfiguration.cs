namespace Robotron.Imagine.Configuration
{
    /// <summary>
    /// Configuration section for image transformation.
    /// </summary>
    public class ImageTransformationConfiguration
    {
        /// <summary>
        /// Transformation modes for images.
        /// </summary>
        public EImageTransformationMode TransformationMode { get; set; }

        /// <summary>
        /// Fill color for transformation mode FILL.
        /// </summary>
        public string FillColor { get; set; }

        /// <summary>
        /// Target width of filled image when using mode FILL.
        /// If empty, width will be calculated.
        /// </summary>
        public int? FillWidth { get; set; }

        /// <summary>
        /// Target height of filled image when using mod FILL.
        /// If empty, height will be calculated.
        /// </summary>
        public int? FillHeight { get; set; }

        /// <summary>
        /// Target width of image when using mode SCALE. 
        /// If empty, width will be calculated.
        /// </summary>
        public int? ScaleWidth { get; set; }

        /// <summary>
        /// Target height of image when using mode SCALE.
        /// If empty, height will be calculated.
        /// </summary>
        public int? ScaleHeight { get; set; }
    }
}
