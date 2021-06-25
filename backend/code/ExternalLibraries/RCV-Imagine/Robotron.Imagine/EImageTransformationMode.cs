namespace Robotron.Imagine
{
    /// <summary>
    /// Transformation mode of image resizing.
    /// </summary>
    public enum EImageTransformationMode
    {
        /// <summary>
        /// No image transformation will be performed.
        /// </summary>
        None,

        /// <summary>
        /// Image will be cropped (Information will be lost).
        /// </summary>
        Crop,

        /// <summary>
        /// Image will be filled.
        /// </summary>
        Fill,

        /// <summary>
        /// Image will be scaled.
        /// </summary>
        Scale
    }
}
