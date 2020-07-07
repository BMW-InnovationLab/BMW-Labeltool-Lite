using Microsoft.AspNetCore.Mvc;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Services;
using System;
using System.Collections.Generic;

namespace Rcv.LabelTool.Backend.Controllers
{
    /// <summary>
    /// Controller for management of objectclasses.
    /// </summary>
    [Route("api/objectclasses")]
    public class ObjectClassController : BaseTopicController
    {
        #region member

        private IObjectClassService ObjectClassService { get; set; }

        #endregion

        #region constructor

        public ObjectClassController(IObjectClassService objectClassService, ITopicService topicService) : base(topicService)
        {
            #region validation

            if (objectClassService == null)
            {
                throw new ArgumentNullException(nameof(objectClassService));
            }

            #endregion

            ObjectClassService = objectClassService;
        }

        #endregion

        /// <summary>
        /// Gets list of objectclasses for specified topic.
        /// </summary>
        /// <param name="topicId">Id of topic</param>
        /// <returns>List of objectclasses</returns>
        /// <response code="200">List of objectclasses</response>
        /// <response code="404">Topic with id not found</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpGet]
        [Route("{topicId}")]
        public ActionResult<IEnumerable<ObjectClass>> GetObjectClasses(long topicId)
        {
            return Execute(topicId, (topic) =>
            {
                // get objectclasses from repository
                return Ok(ObjectClassService.GetObjectClasses(topic));
            });
        }
    }
}