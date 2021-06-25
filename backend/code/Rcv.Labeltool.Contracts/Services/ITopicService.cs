using Rcv.LabelTool.Contracts.Models;
using System.Collections.Generic;

namespace Rcv.LabelTool.Contracts.Services
{
    /// <summary>
    /// Service definition for topic.
    /// </summary>
    public interface ITopicService
    {
        /// <summary>
        /// Checks if topic with given id exists.
        /// </summary>
        /// <param name="topicName">Id of topic.</param>
        /// <returns>TRUE if topic exists, otherwise FALSE</returns>
        bool ExistsTopic(long topicId);

        /// <summary>
        /// Get list of all topics.
        /// </summary>
        /// <returns>List of all topics</returns>
        IEnumerable<Topic> GetAll();


        /// <summary>
        /// Get topic with objectclass-information by id.
        /// </summary>
        /// <param name="topicId">Id of topic</param>
        /// <returns>Topic structure</returns>
        Topic GetTopic(long topicId);
    }
}
