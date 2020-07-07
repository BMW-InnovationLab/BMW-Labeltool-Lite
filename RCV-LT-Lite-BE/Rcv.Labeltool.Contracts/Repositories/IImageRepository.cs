using Rcv.LabelTool.Contracts.Models;
using System.Collections.Generic;

namespace Rcv.LabelTool.Contracts.Repositories
{
    public interface IImageRepository
    {
        /// <summary>
        /// Gets all images for a topic.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <param name="startPosition">The position from which to start looking.</param>
        /// <returns>
        /// A list of images. If no images are found, returns an empty list.
        /// </returns>
        /// <exception cref="ArgumentNullException">Raised whe the topic is null or an empty string.</exception>
        IEnumerable<ImageLabel> GetAll(Topic topic, uint startPosition = 0);

        /// <summary>
        /// Gets imagelabel with specified index from topic. If no imagelabel is avaiable, null will be returned.
        /// </summary>
        /// <param name="topic">The topic</param>
        /// <param name="index">Index of image to get</param>
        /// <returns>Image with specified index or NULL</returns>
        ImageLabel GetImageByIndex(Topic topic, uint index);

        /// <summary>
        /// Gets imagelabel with specified imageid from topic. If no imagelabel is avaiable, null will be returned.
        /// </summary>
        /// <param name="topic">The topic</param>
        /// <param name="imageId">Id of image to get</param>
        /// <si
        /// <returns>Imagelabel with specified id or NULL</returns>
        ImageLabel GetImageLabelById(Topic topic, string imageId);

        /// <summary>
        /// Gets index of image with specified storagepath from topic.
        /// </summary>
        /// <param name="topic">The topic</param>
        /// <param name="imageStoragePath">Storagepath of image to get</param>
        /// <returns>Index of image with given storagepath</returns>
        uint GetImageIndexByStoragePath(Topic topic, string imageStoragePath);

        /// <summary>
        /// Gets index of image with specified imageFileName (equal to id) from topic.
        /// </summary>
        /// <param name="topic">The topic</param>
        /// <param name="imageFileName">Name of imagefile</param>
        /// <returns>Index of image with given imageFilename</returns>
        uint GetImageIndexByImageFileName(Topic topic, string imageFileName);

        /// <summary>
        /// Count all Images for the specified topic.
        /// </summary>
        /// <param name="topic">The topic.</param>
        /// <returns>Number of images for this topic.</returns>
        uint CountAll(Topic topic);

        /// <summary>
        /// Gets imagefile from topic.
        /// </summary>
        /// <param name="topic">The topic</param>
        /// <param name="imageFileName">Name of imagefile</param>
        /// <returns>Content of imagefile as stream</returns>
        System.IO.Stream GetImage(Topic topic, string imageFileName);

        /// <summary>
        /// Checks if image with given imageName exists in given topic.
        /// </summary>
        /// <param name="topic">Topic to check for image</param>
        /// <param name="imageName">Name of image to check for</param>
        /// <returns>TRUE if image exists, otherwise FALSE</returns>
        bool ExistsImage(Topic topic, string imageName);

        /// <summary>
        /// Set size of image.
        /// </summary>
        /// <param name="topic">Topic where image is stored</param>
        /// <param name="imageLabel">Image without labels</param>
        void SetImageSize(Topic topic, ImageLabel imageLabel);
    }
}
