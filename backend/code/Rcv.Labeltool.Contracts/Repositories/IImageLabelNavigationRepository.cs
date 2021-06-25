using Rcv.LabelTool.Contracts.Enumerations;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Results;

namespace Rcv.LabelTool.Contracts.Repositories
{
    /// <summary>
    /// Repository for navigation through images with labels.
    /// </summary>
    public interface IImageLabelNavigationRepository
    {
        /// <summary>
        /// Navigate to next/previous image in topic. 
        /// </summary>
        /// <param name="topic">Topic where navigation should be performed</param>
        /// <param name="startIndex">Current index where navigation is started</param>
        /// <param name="labelMode">Mode of labeling</param>
        /// <param name="navigationDirection">Direction of navigation</param>
        /// <returns>Result of navigation</returns>
        ImageNavigationResult GetNavigationResult(Topic topic, long startIndex, ELabelMode labelMode, ENavigationDirection navigationDirection);
    }
}
