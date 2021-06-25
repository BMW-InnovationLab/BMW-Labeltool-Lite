namespace Rcv.LabelTool.Contracts.Models.View
{
    /// <summary>
    /// Contract for image-navigation result.
    /// </summary>
    public class ImageNavigationResultView
    {
        /// <summary>
        /// Imagelabel-structure of imagelabel which was found.
        /// </summary>
        public ImageLabelView ImageLabel { get; set; }

        /// <summary>
        /// Number of images which are related to topic of navigation.
        /// </summary>
        public int ImageCount { get; set; }

        /// <summary>
        /// Flag if previous image without label or segments is
        /// avaiable. Depends on labeltool mode.
        /// </summary>
        public bool HasPreviousBlank { get; set; }

        /// <summary>
        /// Flag if previous image is avaiable.
        /// </summary>
        public bool HasPrevious { get; set; }

        /// <summary>
        /// Flag if next image is avaiable.
        /// </summary>
        public bool HasNext { get; set; }

        /// <summary>
        /// Flag if next image without label or segments is
        /// avaiable. Depends on labeltool mode.
        /// </summary>
        public bool HasNextBlank { get; set; }
    }
}
