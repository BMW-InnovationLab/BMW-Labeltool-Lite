using Rcv.LabelTool.Contracts.Models;

namespace Rcv.LabelTool.Contracts.Repositories
{
    /// <summary>
    /// Interface definition for image file repository.
    /// </summary>
    public interface IImageFileRepository
    {
        /// <summary>
        /// Deletes image from repository (without labels and segments!)
        /// </summary>
        /// <param name="topic">The topic</param>
        /// <param name="imageFileName">Name of imagefile</param>
        void DeleteImageFile(Topic topic, string imageFileName);

        /// <summary>
        /// Gets imagefile from topic.
        /// </summary>
        /// <param name="topic">The topic</param>
        /// <param name="imageFileName">Name of imagefile</param>
        /// <returns>Content of imagefile as stream</returns>
        System.IO.Stream GetImageFile(Topic topic, string imageFileName);

        /// <summary>
        /// Add image with given name to specified topic.
        /// </summary>
        /// <param name="topic">Topic where image should be stored</param>
        /// <param name="fileName">Filename of image to add</param>
        /// <param name="imageStream">Stream of image</param>
        /// <returns>Newly uploaded image</returns>
        string[] AddImageFile(Topic topic, string fileName, System.IO.Stream imageStream);
    }
}
