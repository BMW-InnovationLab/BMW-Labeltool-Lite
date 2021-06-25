using Rcv.LabelTool.Contracts.Models;
using System.Collections.Generic;

namespace Rcv.LabelTool.Contracts.Services
{
    /// <summary>
    /// Declares methods to get next image and update existing images with labels.
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// Counts images that are stored in topic.
        /// </summary>
        /// <param name="topic">Topic where to count images</param>
        /// <returns>Count of images in topic</returns>
        uint ImageCount(Topic topic);

        /// <summary>
        /// Deletes image with all information.
        /// </summary>
        /// <param name="topicName">Topic where image should be deleted</param>
        /// <param name="imageFileName">Name of imagefile</param>
        void DeleteImage(Topic topic, string imageFileName);

        /// <summary>
        /// Get content of imagefile from topic.
        /// </summary>
        /// <param name="topic">Topic where image is stored</param>
        /// <param name="imageFileName">Name of imagefile</param>
        /// <returns>Stream of image</returns>
        System.IO.Stream GetImage(Topic topic, string imageFileName);

        /// <summary>
        /// Add image with given name and content to topic.
        /// </summary>
        /// <param name="topic">Topic where to store image</param>
        /// <param name="imageName">Name of image</param>
        /// <param name="imageStream">Content of image as stream</param>
        /// <returns>Newly uploaded image</returns>
        ImageLabel AddImage(Topic topic, string imageName, System.IO.Stream imageStream);

        /// <summary>
        /// Get list of all imagelabels of specified topic.
        /// </summary>
        /// <param name="topic">Topic where to get imagelabels from</param>
        /// <returns>List of imagelabels</returns>
        IList<ImageLabel> GetImageLabels(Topic topic);

        /// <summary>
        /// Checks if image with name exists in given topic.
        /// </summary>
        /// <param name="topic">Topic to check for image</param>
        /// <param name="imageName">Imagename to find in topic</param>
        /// <returns>TRUE if image exists, otherwise FALSE</returns>
        bool ExistsImage(Topic topic, string imageName);

        /// <summary>
        /// Get imagelabel structure of image with given name from 
        /// specified topic.
        /// </summary>
        /// <param name="topic">Topic where image is stored</param>
        /// <param name="imageId">Id of image</param>
        /// <returns>ImageLabel of image</returns>
        ImageLabel GetImageLabel(Topic topic, string imageId);
    }
}
