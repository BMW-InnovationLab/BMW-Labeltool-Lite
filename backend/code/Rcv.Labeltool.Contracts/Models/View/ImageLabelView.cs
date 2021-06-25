using System.Collections.Generic;

namespace Rcv.LabelTool.Contracts.Models.View
{
    /// <summary>
    /// Storageclass of Image with labels.
    /// </summary>
    public class ImageLabelView
    {
        /// <summary>
        /// ID of image. Equal to image file name.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Index of image.
        /// </summary>
        public long Index { get; set; }

        /// <summary>
        /// Width of image.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of image.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Flag if image has labels.
        /// </summary>
        public bool HasLabels { get; set; }

        /// <summary>
        /// List of Labels for this image.
        /// Lazy-loaded. Check HasLabels first!
        /// </summary>
        public IEnumerable<LabelView> Labels { get; set; }
    }
}
