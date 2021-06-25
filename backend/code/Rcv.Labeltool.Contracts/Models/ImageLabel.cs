using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Rcv.LabelTool.Contracts.Models
{
    /// <summary>
    /// Storageclass of Image with labels.
    /// </summary>
    [DataContract]
    public class ImageLabel
    {
        /// <summary>
        /// Id of image which will be generated in database.
        /// </summary>
        [JsonIgnore]
        public long ImageId { get; set; }

        /// <summary>
        /// ID of image. Equal to image file name.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Storage Path of image in local file system.
        /// </summary>
        [JsonIgnore]
        public string StoragePath { get; set; }

        /// <summary>
        /// Index of image. 
        /// </summary>
        [DataMember]
        public long Index { get; set; }

        /// <summary>
        /// Width of image.
        /// </summary>
        [DataMember]
        public int Width { get; set; }

        /// <summary>
        /// Height of image.
        /// </summary>
        [DataMember]
        public int Height { get; set; }

        /// <summary>
        /// List of Labels for this image. 
        /// Lazy-loaded. Check HasLabels first!
        /// </summary>
        [DataMember]
        public virtual IList<Label> Labels { get; set; }

        /// <summary>
        /// Lazy loading indicator for Labels. 
        /// If true, labels could be found in file system.
        /// </summary>
        [DataMember]
        public bool HasLabels { get; set; }
    }
}
