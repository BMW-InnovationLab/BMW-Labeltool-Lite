using Microsoft.AspNetCore.Mvc;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Services;
using System.Collections.Generic;

namespace Rcv.LabelTool.Backend.Controllers
{
    /// <summary>
    /// Controller for simple topic actions.
    /// </summary>
    [Route("api/topics")]
    public class TopicsController : BaseTopicController
    {
        #region member

        private ITopicService TopicService { get; set; }

        #endregion


        #region constructor

        public TopicsController(ITopicService topicService) : base(topicService)
        {
            TopicService = topicService;
        }

        #endregion

        /// <summary>
        /// Get list of all topics.
        /// </summary>
        /// <returns>List of all topics</returns>
        /// <response code="200">List of all topics</response>        
        /// <response code="500">Unexpected error occures</response>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<Topic>> GetTopicsList()
        {
            return Execute(() =>
            {
                return Ok(TopicService.GetAll());
            });
        }
    }
}