using Rcv.LabelTool.Contracts.Models;
using System.Collections.Generic;

namespace Rcv.LabelTool.Contracts.Repositories
{
    /// <summary>
    /// Definition for repository of topics.
    /// </summary>
    public interface ITopicRepository
    {
        /// <summary>
        /// Checks if topic with given id exists.
        /// </summary>
        /// <param name="topicName">Id of topic</param>
        /// <returns>TRUE if topic exists, otherwise FALSE</returns>
        bool ExistsTopic(long topicId);

        /// <summary>
        /// Get topic by Id.
        /// </summary>
        /// <param name="topicId">Id of topic</param>
        /// <returns>Topic with given ID</returns>
        Topic GetTopic(long topicId);

        /// <summary>
        /// Gets all topics.
        /// </summary>
        /// <returns>List with all topics</returns>
        IEnumerable<Topic> GetTopics();
    }
}
