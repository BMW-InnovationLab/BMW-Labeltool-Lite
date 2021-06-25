using Microsoft.AspNetCore.Mvc;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Models.View;
using Rcv.LabelTool.Contracts.Services;
using Rcv.LabelTool.Logic.Factories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rcv.LabelTool.Backend.Controllers
{
    /// <summary>
    /// Controller for simple topic actions.
    /// </summary>
    [Route("api/topics")]
    public class TopicsController : BaseTopicController
    {
        #region member

        private ITopicService TopicService { get; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new controller instance.
        /// </summary>
        /// <param name="topicService">Topic service</param>
        public TopicsController(ITopicService topicService) : base(topicService)
        {
            TopicService = topicService;
        }

        #endregion

        /// <summary>
        /// Get list of all topics.
        /// </summary>
        /// <returns>List of all views of topics</returns>
        /// <response code="200">List of all topics</response>
        /// <response code="500">Unexpected error occurs</response>
        [HttpGet]
        public ActionResult<IEnumerable<TopicView>> GetTopicsList()
        {
            return Execute(() =>
            {
                IEnumerable<Topic> topics = TopicService.GetAll().OrderBy(o => o.Path, StringComparer.InvariantCultureIgnoreCase).ThenBy(o => o.Name, StringComparer.InvariantCultureIgnoreCase);

                List<TopicView> topicViews = new List<TopicView>();
                foreach (Topic topic in topics)
                {
                    topicViews.Add(TopicFactory.CreateView(topic));
                }

                return Ok(topicViews);
            });
        }
    }
}