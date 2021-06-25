using Rcv.LabelTool.Contracts.Models;
using System.IO;

namespace Rcv.LabelTool.Contracts.Services
{
    /// <summary>
    /// Declares methods to add images.
    /// </summary>
    public interface IImageUploadService
    {
        /// <summary>
        /// Add image with given name and content to topic.
        /// </summary>
        /// <param name="topic">Topic where to store image</param>
        /// <param name="imageName">Name of image</param>
        /// <param name="imageStream">Content of image as stream</param>
        /// <returns>Newly uploaded image</returns>
        ImageLabel UploadImage(Topic topic, string imageName, Stream imageStream);
    }
}
