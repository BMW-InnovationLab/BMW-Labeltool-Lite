using Rcv.LabelTool.Contracts.Models;
using System.Collections.Generic;

namespace Rcv.LabelTool.Contracts.Repositories
{
    /// <summary>
    /// Interface defintion of image data repository.
    /// </summary>
    public interface IImageDataRepository
    {
        /// <summary>
        /// Gets all images for a topic.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <returns>
        /// A list of images. If no images are found, returns an empty list.
        /// </returns>        
        IList<ImageLabel> GetImages(Topic topic);

        /// <summary>
        /// Gets imagelabel with specified imageid from topic. If no imagelabel is avaiable, null will be returned.
        /// </summary>
        /// <param name="topic">The topic</param>
        /// <param name="imageId">Id of image to get</param>
        /// <returns>Imagelabel with specified id or NULL</returns>
        ImageLabel GetImageLabelById(Topic topic, string imageId);

        /// <summary>
        /// Count all Images for the specified topic.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <returns>Number of images for this topic.</returns>
        uint CountAll(Topic topic);

        /// <summary>
        /// Deletes image from repository (without labels and segments!)
        /// </summary>
        /// <param name="topic">The topic</param>
        /// <param name="imageFileName">Name of imagefile</param>
        void DeleteImage(Topic topic, string imageFileName);

        /// <summary>
        /// Add image with given name to specified topic.
        /// </summary>
        /// <param name="topic">Topic where image should be stored</param>
        /// <param name="image">Image to add</param>
        /// <returns>Newly uploaded image</returns>
        ImageLabel AddImage(Topic topic, ImageLabel image);

        /// <summary>
        /// Checks if image with given imageName exists in given topic.
        /// </summary>
        /// <param name="topic">Topic to check for image</param>
        /// <param name="imageName">Name of image to check for</param>
        /// <returns>TRUE if image exists, otherwise FALSE</returns>
        bool ExistsImage(Topic topic, string imageName);
    }
}
