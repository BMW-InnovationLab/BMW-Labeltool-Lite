using Rcv.LabelTool.Contracts.Models;

namespace Rcv.LabelTool.Contracts.Services
{
    /// <summary>
    /// Interface for imagedata-services.
    /// </summary>
    public interface IImageDataService
    {
        /// <summary>
        /// Get imagedata structure of image with given name from
        /// specified topic.
        /// </summary>
        /// <param name="topic">Topic where image is stored</param>
        /// <param name="imageId">Id of image</param>
        /// <returns>ImageData of image</returns>
        ImageData GetImageData(Topic topic, string imageId);

        /// <summary>
        /// Checks if image with name exists in given topic.
        /// </summary>
        /// <param name="topic">Topic to check for image</param>
        /// <param name="imageName">Imagename to find in topic</param>
        /// <returns>TRUE if image exists, otherwise FALSE</returns>
        bool ExistsImage(Topic topic, string imageName);
    }
}
