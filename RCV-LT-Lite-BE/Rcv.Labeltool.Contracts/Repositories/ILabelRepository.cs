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
        /// <param name="imageLabel">Imagelabel to set labels</param>
        /// <returns>List of labels which are defined for image</returns>
        IEnumerable<Label> GetLabels(Topic topic, ImageLabel imageLabel);

        /// <summary>
        /// Creates label for given imagefile with specified content.
        /// </summary>
        /// <param name="topic">topic of image</param>
        /// <param name="imageLabel">ImageLabel strucutre</param>
        /// <returns>List of created labels</returns>
        IEnumerable<Label> CreateLabels(Topic topic, ImageLabel imageLabel);

        /// <summary>
        /// Deletes label of given imagefile.
        /// </summary>
        /// <param name="topic">topic of imagefile</param>
        /// <param name="imageLabel">ImageLabel structure</param>
        void DeleteLabels(Topic topic, ImageLabel imageLabel);
    }
}
