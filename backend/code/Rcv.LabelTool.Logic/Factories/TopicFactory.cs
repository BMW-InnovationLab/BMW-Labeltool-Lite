using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Models.View;
using System;

namespace Rcv.LabelTool.Logic.Factories
{
    /// <summary>
    /// Factory for creation of topic-representation objects.
    /// </summary>
    public static class TopicFactory
    {
        /// <summary>
        /// Create topic view object from topic.
        /// </summary>
        /// <param name="topic">Source topic object</param>
        /// <returns>View representation of topic</returns>
        public static TopicView CreateView(Topic topic)
        {
            #region validation

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            #endregion

            return new TopicView()
            {
                Id = topic.Id,
                Name = topic.Name,
                Path = topic.Path
            };
        }

        /// <summary>
        /// Create topic object from topic view.
        /// </summary>
        /// <param name="topicView">Topic view to create topic from</param>
        /// <returns>Representation of topic</returns>
        public static Topic Create(TopicView topicView)
        {
            #region validation

            if (topicView == null)
            {
                throw new ArgumentNullException(nameof(topicView));
            }

            #endregion

            return new Topic()
            {
                Id = topicView.Id,
                Name = topicView.Name,
                Path = topicView.Path,
            };
        }
    }
}
