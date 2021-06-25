using Rcv.LabelTool.Contracts.Models;
using System.Collections.Generic;

namespace Rcv.LabelTool.Contracts.Repositories
{
    /// <summary>
    /// Repository definition for storing labels.
    /// </summary>
    public interface ILabelRepository
    {
        /// <summary>
        /// Get list of labelfile names without extensions. List of labelfile
        /// names must not correspond to list of image files. 
        /// Problem with images with same extension exists!
        /// </summary>
        /// <param name="topic">Topic of label files</param>
        /// <returns>List of labelfile names without extension</returns>
        IEnumerable<string> GetLabelFileNames(Topic topic);

        /// <summary>
        /// Get labels for image with given path at given topic.
        /// </summary>
        /// <param name="topic">Topic of image</param>
        /// <param name="image">Imagelabel to set labels</param>
        /// <returns>List of labels which are defined for image</returns>
        IEnumerable<Label> GetLabels(Topic topic, ImageLabel image);

        /// <summary>
        /// Add labels for given imagefile.
        /// </summary>
        /// <param name="topic">Topic of image</param>
        /// <param name="image">Image of labels</param>
        /// <param name="labels">List of labels to add</param>
        /// <returns>List of added labels</returns>
        IEnumerable<Label> AddLabels(Topic topic, ImageLabel image, IEnumerable<Label> labels);

        /// <summary>
        /// Add or remove or update labels for given imagefile with specified content.
        /// Any unknown labels will be added to collection of labels.
        /// Any known labels will be updated in collection of labels.
        /// Any not defined labels will be deleted in collection of labels.
        /// </summary>
        /// <param name="topic">topic of image</param>
        /// <param name="image">Image of labels</param>
        /// <param name="labels">List of labels to add/update/delete for image</param>
        /// <returns>List of labels of image</returns>
        IEnumerable<Label> AddOrUpdateOrDeleteLabels(Topic topic, ImageLabel image, IEnumerable<Label> labels);

        /// <summary>
        /// Deletes label of given imagefile.
        /// </summary>
        /// <param name="topic">topic of imagefile</param>
        /// <param name="image">ImageLabel structure</param>
        void DeleteLabels(Topic topic, ImageLabel image);
    }
}
