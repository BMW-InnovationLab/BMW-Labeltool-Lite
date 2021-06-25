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
    /// Controller for management of objectclasses.
    /// </summary>
    [Route("api/objectclasses")]
    public class ObjectClassController : BaseTopicController
    {
        #region member

        private IObjectClassService ObjectClassService { get; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new controller instance.
        /// </summary>
        /// <param name="objectClassService">Objectclass service</param>
        /// <param name="topicService">Topic service</param>
        public ObjectClassController(IObjectClassService objectClassService, ITopicService topicService) : base(topicService)
        {
            ObjectClassService = objectClassService ?? throw new ArgumentNullException(nameof(objectClassService));
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
        [HttpGet("{topicId}")]
        public ActionResult<IEnumerable<ObjectClassView>> GetObjectClasses(long topicId)
        {
            return Execute(topicId, (topic) =>
            {
                // get objectclasses from repository
                IEnumerable<ObjectClass> objectClasses = ObjectClassService.GetObjectClasses(topic);

                // return objectclass views
                List<ObjectClassView> objectClassViews = new List<ObjectClassView>();
                foreach (ObjectClass objectClass in objectClasses)
                {
                    objectClassViews.Add(ObjectClassFactory.CreateView(objectClass));
                }

                return Ok(objectClassViews.OrderBy(o => o.Sort));
            });
        }
    }
}