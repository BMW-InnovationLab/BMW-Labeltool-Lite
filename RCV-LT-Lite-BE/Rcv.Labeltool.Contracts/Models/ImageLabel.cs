using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Rcv.LabelTool.Contracts.Models
{
    /// <summary>
    /// Storageclass of Image with labels.
    /// </summary>
    public class ImageLabel
    {
        /// <summary>
        /// ID of image. Equal to image file name.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Url of image. Used for request image.
        /// </summary>
        [Obsolete("Will be hidden in next iteration with JSONIgnore.")]
        public string Url { get; set; }

        /// <summary>
        /// Path of image. In combination with hostname url for request image.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Storage Path of image in local file system.
        /// </summary>
        [JsonIgnore]
        public string StoragePath { get; set; }

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
        /// List of Labels for this image. 
        /// Lazy-loaded. Check HasLabels first!
        /// </summary>
        public IList<Label> Labels { get; set; }

        /// <summary>
        /// Lazy loading indicator for Labels. 
        /// If true, labels could be found in file system.
        /// </summary>
        public bool HasLabels { get; set; }
    }
}
