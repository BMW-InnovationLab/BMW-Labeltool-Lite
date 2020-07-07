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
        /// Gets list of labelfile names without extension for given topic.
        /// E.g. List with 003, 004, IMG_019
        /// </summary>
        /// <param name="topic">Topic of labels</param>
        /// <returns>List of labelnames</returns>
        IEnumerable<string> GetLabeledImageFileNames(Topic topic);

        /// <summary>
        /// Gets list of labels for image which is stored in given topic.
        /// </summary>
        /// <param name="topic">Topic of image</param>
        /// <param name="imageLabel">Image structure without labels</param>        
        /// <returns>List of labels</returns>
        IEnumerable<Label> GetLabels(Topic topic, ImageLabel imageLabel);

        /// <summary>
        /// Stores labels in file system.
        /// </summary>
        /// <param name="topic">Topic where to store labels</param>
        /// <param name="image">Image with labels to store</param>
        /// <returns>List of saved labels</returns>
        IEnumerable<Label> SaveLabels(Topic topic, ImageLabel image);
    }
}
