using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Repositories;
using Rcv.LabelTool.Contracts.Services;
using System;
using System.Collections.Generic;

namespace Rcv.LabelTool.Web.Services
{
    public class TopicService : ITopicService
    {
        #region member

        private ITopicRepository TopicRepository { get; }

        #endregion

        #region constructor

        public TopicService(ITopicRepository topicRepository)
        {
            TopicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        }

        #endregion

        #region ITopicService

        public IEnumerable<Topic> GetAll()
        {
            return TopicRepository.GetTopics();
        }

        public Topic GetTopic(long topicId)
        {
            return TopicRepository.GetTopic(topicId);
        }

        public bool ExistsTopic(long topicId)
        {
            return TopicRepository.ExistsTopic(topicId);
        }

        #endregion
    }
}