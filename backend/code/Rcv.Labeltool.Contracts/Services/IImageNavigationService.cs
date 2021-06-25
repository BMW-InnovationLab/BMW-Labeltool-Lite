using Rcv.LabelTool.Contracts.Enumerations;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Results;

namespace Rcv.LabelTool.Contracts.Services
{
    /// <summary>
    /// Service to navigate through images.
    /// </summary>
    public interface IImageNavigationService
    {
        /// <summary>
        /// Navigate to next/previous image in topic. 
        /// </summary>
        /// <param name="topic">Topic where navigation should be performed</param>
        /// <param name="startIndex">Current index where navigation is started</param>
        /// <param name="labelMode">Mode of labeling</param>
        /// <param name="navigationDirection">Direction of navigation</param>
        /// <returns>Result of navigation</returns>
        ImageNavigationResult Navigate(Topic topic, long startIndex, ELabelMode labelMode, ENavigationDirection navigationDirection);
    }
}
