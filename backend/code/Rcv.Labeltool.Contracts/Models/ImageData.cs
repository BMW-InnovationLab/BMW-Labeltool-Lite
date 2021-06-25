using System.Runtime.Serialization;

namespace Rcv.LabelTool.Contracts.Models
{
    /// <summary>
    /// Storageclass of Image data.
    /// </summary>
    [DataContract]
    public class ImageData
    {
        /// <summary>
        /// ID of image. Equal to image file name.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

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
    }
}
