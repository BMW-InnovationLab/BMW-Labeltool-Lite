using Microsoft.AspNetCore.Mvc;
using Rcv.LabelTool.Backend.Util;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Services;
using System;
using System.Collections.Generic;

namespace Rcv.LabelTool.Backend.Controllers
{
    /// <summary>
    /// Controller for suggest methods of objectdetection/imageclassification.
    /// </summary>
    [Route("api/suggest")]
    public class SuggestController : BaseTopicController
    {
        #region member

        private IImageService ImageService { get; }

        private ISuggestService SuggestService { get; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new controller instance.
        /// </summary>
        /// <param name="topicService">Topic service</param>
        /// <param name="imageService">Image service</param>
        /// <param name="suggestService">Suggest service</param>
        public SuggestController(ITopicService topicService, IImageService imageService, ISuggestService suggestService) : base(topicService)
        {
            ImageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            SuggestService = suggestService ?? throw new ArgumentNullException(nameof(suggestService));
        }

        #endregion

        /// <summary>
        /// Returns the bounding boxes found on the image.
        /// </summary>
        /// <param name="imageId">Name of image file</param>
        /// <param name="topicId">Id of topic</param>
        /// <returns>Collection of labels</returns>
        /// <response code="200">Returns the bounding boxes</response>
        /// <response code="404">Topic with id not found or image with id not found</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost("{topicId}/{imageId}/e")]
        public ActionResult<IEnumerable<Label>> GetBoundingBoxes(long topicId, string imageId)
        {
            return Execute(topicId, (topic) =>
            {
                #region validation

                if (!ImageService.ExistsImage(topic, imageId))
                {
                    return NotFound(Messages.ImageNotFound(imageId));
                }

                #endregion

                var suggestions = SuggestService.GetBoundingBoxes(topic, imageId);

                int id = 1;
                foreach (var suggestion in suggestions)
                {
                    suggestion.Id = id++;
                }

                return Ok(suggestions);
            });
        }
    }
}