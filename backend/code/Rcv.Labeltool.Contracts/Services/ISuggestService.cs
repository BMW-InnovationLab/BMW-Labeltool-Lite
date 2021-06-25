using Rcv.LabelTool.Contracts.Models;
using System.Collections.Generic;

namespace Rcv.LabelTool.Contracts.Services
{
    /// <summary>
    /// Service definition for automated segmentation.
    /// </summary>
    public interface ISuggestService
    {
        /// <summary>
        ///  Returns the bounding boxes found on the image.
        /// </summary>
        /// <param name="imageId">Name of imagefile</param>
        /// <param name="topic">Topic</param>
        /// <returns>Collection of labels</returns>
        IEnumerable<Label> GetBoundingBoxes(Topic topic, string imageId);
    }
}
