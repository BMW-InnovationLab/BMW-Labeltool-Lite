using Microsoft.AspNetCore.Mvc;
using Rcv.Base.WebApi.Controller;
using Rcv.LabelTool.Backend.Util;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Services;
using System;

namespace Rcv.LabelTool.Backend.Controllers
{
    /// <summary>
    /// Abstract controller for operations which are based on topics.
    /// </summary>
    public abstract class BaseTopicController : AbstractController
    {
        /// <summary>
        /// Delegate for perform controller action with topic parameter.
        /// </summary>
        /// <returns>Action result of action</returns>
        protected delegate ActionResult ControllerTopicAction(Topic topic);

        #region member

        /// <summary>
        /// Topic service to work with.
        /// </summary>
        private ITopicService TopicService { get; }

        #endregion

        /// <summary>
        /// Create a new instance with topicservice as base service for 
        /// topic operations.
        /// </summary>
        /// <param name="topicService">Topic service of controller</param>
        protected BaseTopicController(ITopicService topicService) : base()
        {
            TopicService = topicService ?? throw new ArgumentNullException(nameof(topicService));
        }

        #region methods

        /// <summary>
        /// Perform action with topic parameter. Includes check of topicId
        /// for validity and conversion from topicId to topic.
        /// </summary>
        /// <param name="topicId">Id of topic to use</param>
        /// <param name="controllerAction">Action which should be performed</param>
        /// <returns>result of action</returns>
        protected ActionResult Execute(long topicId, ControllerTopicAction controllerAction)
        {
            #region validation

            // check if topic id is valid
            if (!TopicService.ExistsTopic(topicId))
            {
                return NotFound(Messages.TopicNotFound(topicId));
            }

            #endregion

            // execute base action with integrated action with topic parameter
            return base.Execute(() =>
            {
                Topic topic = TopicService.GetTopic(topicId);
                return controllerAction.Invoke(topic);
            });
        }

        #endregion
    }
}
