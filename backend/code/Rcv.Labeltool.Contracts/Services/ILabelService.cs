using Rcv.LabelTool.Contracts.Models;
using System.Collections.Generic;

namespace Rcv.LabelTool.Contracts.Services
{
    /// <summary>
    /// Interface for label-services.
    /// </summary>
    public interface ILabelService
    {
        /// <summary>
        /// Gets list of labels for image which is stored in given topic.
        /// </summary>
        /// <param name="topic">Topic of image</param>
        /// <param name="imageLabel">Image structure without labels</param>        
        /// <returns>List of labels</returns>
        IEnumerable<Label> GetLabels(Topic topic, ImageLabel imageLabel);

        /// <summary>
        /// Add labels in file system.
        /// </summary>
        /// <param name="topic">Topic where to add labels</param>
        /// <param name="image">Image where to add labels</param>
        /// <param name="labels">List of labels to add for image</param>
        /// <returns>List of saved labels</returns>
        IEnumerable<Label> AddLabels(Topic topic, ImageLabel image, IEnumerable<Label> labels);

        /// <summary>
        /// Stores labels in file system.
        /// </summary>
        /// <param name="topic">Topic where to store labels</param>
        /// <param name="image">Image where to store labels</param>
        /// <param name="labels">List of labels to create for image</param>
        /// <returns>List of saved labels</returns>
        IEnumerable<Label> SaveLabels(Topic topic, ImageLabel image, IEnumerable<Label> labels);

        /// <summary>
        /// Delete all labels which are related to given image.
        /// </summary>
        /// <param name="topic">Topic where image is stored</param>
        /// <param name="image">Image where labels are related to</param>
        void DeleteLabels(Topic topic, ImageLabel image);
    }
}
