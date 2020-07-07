using Microsoft.AspNetCore.Mvc;
using Rcv.LabelTool.Contracts.Enumerations;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Parameters;
using Rcv.LabelTool.Contracts.Results;
using Rcv.LabelTool.Contracts.Services;
using Rcv.LabelTool.Web.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rcv.LabelTool.Backend.Controllers
{
    /// <summary>
    /// Controller for navigation through topics in labeltool.
    /// </summary>
    [Route("api/navigate")]
    public class NavigationController : BaseTopicController
    {
        #region member

        /// <summary>
        /// Image service to work with.
        /// </summary>
        private IImageService ImageService { get; set; }

        #endregion

        #region constructor

        public NavigationController(IImageService imageService, ITopicService topicService) : base(topicService)
        {
            #region validation

            if (imageService == null)
            {
                throw new ArgumentNullException(nameof(imageService));
            }

            #endregion

            ImageService = imageService;
        }

        #endregion

        #region NavigationController

        /// <summary>
        /// Navigate to imagelabel with index from NavigationParameter in topic.
        /// </summary>
        /// <param name="topicId">ID of topic for navigation</param>
        /// <param name="navigationParameter">Navigation parameters</param>
        /// <returns>Result of navigation</returns>
        /// <response code="200">Result of navigation</response>
        /// <response code="400">Parameters for navigation request are required</response>
        /// <response code="404">The topic cannot be found or No matching result found for request</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpGet]
        [Route("{topicId}")]
        public ActionResult<ImageLabelNavigationResult> Navigate(long topicId, [FromQuery(Name = "")] NavigationParameter navigationParameter)
        {
            return Execute(topicId, (topic) =>
            {
                return Navigate(topic, navigationParameter, ENavigationDirection.Direct);
            });
        }

        /// <summary>
        /// Navigate to previous imagelabel which is blank (has no labels or segments) depending on
        /// current labeltool-mode from NavigationParameter. Navigation will be 
        /// performed from current index given at NavigationParameter.Index.
        /// </summary>
        /// <param name="topicId">ID of topic for navigation</param>
        /// <param name="navigationParameter">Navigation parameters</param>
        /// <returns>Result of navigation</returns>
        /// <response code="200">Result of navigation</response>
        /// <response code="400">Parameters for navigation request are required</response>
        /// <response code="404">The topic cannot be found or No matching result found for request</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpGet]
        [Route("{topicId}/previousblank")]
        public ActionResult<ImageLabelNavigationResult> PreviousBlank(long topicId, [FromQuery(Name = "")] NavigationParameter navigationParameter)
        {
            return Execute(topicId, (topic) =>
            {
                return Navigate(topic, navigationParameter, ENavigationDirection.PreviousBlank);
            });
        }

        /// <summary>
        /// Navigate to previous imagelabel. Navigation will be 
        /// performed from current index given at NavigationParameter.Index.
        /// </summary>
        /// <param name="topicId">ID of topic for navigation</param>
        /// <param name="navigationParameter">Navigation parameters</param>
        /// <returns>Result of navigation</returns>
        /// <response code="200">Result of navigation</response>
        /// <response code="400">Parameters for navigation request are required</response>
        /// <response code="404">The topic cannot be found or No matching result found for request</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpGet]
        [Route("{topicId}/previous")]
        public ActionResult<ImageLabelNavigationResult> Previous(long topicId, [FromQuery(Name = "")] NavigationParameter navigationParameter)
        {
            return Execute(topicId, (topic) =>
            {
                return Navigate(topic, navigationParameter, ENavigationDirection.Previous);
            });
        }

        /// <summary>
        /// Navigate to next imagelabel. Navigation will be 
        /// performed from current index given at NavigationParameter.Index.
        /// </summary>
        /// <param name="topicId">ID of topic for navigation</param>
        /// <param name="navigationParameter">Navigation parameters</param>
        /// <returns>Result of navigation</returns>
        /// <response code="200">Result of navigation</response>
        /// <response code="400">Parameters for navigation request are required</response>
        /// <response code="404">The topic cannot be found or No matching result found for request</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpGet]
        [Route("{topicId}/next")]
        public ActionResult<ImageLabelNavigationResult> Next(long topicId, [FromQuery(Name = "")] NavigationParameter navigationParameter)
        {
            return Execute(topicId, (topic) =>
            {
                return Navigate(topic, navigationParameter, ENavigationDirection.Next);
            });
        }

        /// <summary>
        /// Navigate to next imagelabel which is blank (has no labels or segments) depending on
        /// current labeltool-mode at NavigationParameter.LabelMode. Navigation will be 
        /// performed from current index given at NavigationParameter.Index.
        /// </summary>
        /// <param name="topicId">ID of topic for navigation</param>
        /// <param name="navigationParameter">Navigation parameters</param>
        /// <returns>Result of navigation</returns>
        /// <response code="200">Result of navigation</response>
        /// <response code="400">Parameters for navigation request are required</response>
        /// <response code="404">The topic cannot be found or No matching result found for request</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpGet]
        [Route("{topicId}/nextblank")]
        public ActionResult<ImageLabelNavigationResult> NextBlank(long topicId, [FromQuery(Name = "")] NavigationParameter navigationParameter)
        {
            return Execute(topicId, (topic) =>
            {
                return Navigate(topic, navigationParameter, ENavigationDirection.NextBlank);
            });
        }

        /// <summary>
        /// Reverse navigation to first imagelabel at end of topic which is blank.
        /// </summary>
        /// <param name="topicId">ID of topic for navigation</param>
        /// <param name="navigationParameter">Navigation parameters</param>
        /// <returns>Result of navigation</returns>
        /// <response code="200">Result of navigation</response>
        /// <response code="400">Parameters for navigation request are required</response>
        /// <response code="404">The topic cannot be found or No matching result found for request</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpGet]
        [Route("{topicId}/lastblank")]
        public ActionResult<ImageLabelNavigationResult> LastBlank(long topicId, [FromQuery(Name = "")] NavigationParameter navigationParameter)
        {
            return Execute(topicId, (topic) =>
            {
                ImageLabelNavigationResult result = GetImageLabelNavigationResult(topic, navigationParameter, ENavigationDirection.LastBlank);
                if (result == null)
                {
                    // if no last blank image found then navigate to first image
                    navigationParameter.Index = 0;
                    return Navigate(topic, navigationParameter, ENavigationDirection.Direct);
                }
                return Ok(result);
            });
        }

        /// <summary>
        /// Navigation to next imagelabel without labels or segments.
        /// If shuffle is true, it will navigate to a random imagelabel.
        /// </summary>
        /// <param name="topicId">ID of topic for navigation</param>
        /// <param name="navigationParameter">Navigation parameters</param>
        /// <returns>Result of navigation</returns>
        /// <response code="200">Result of navigation</response>
        /// <response code="400">Parameters for navigation request are required</response>
        /// <response code="404">The topic cannot be found or No matching result found for request</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpGet]
        [Route("{topicId}/blank")]
        public ActionResult<ImageLabelNavigationResult> Blank(long topicId, [FromQuery(Name = "")] NavigationParameter navigationParameter)
        {
            return Execute(topicId, (topic) =>
            {
                ImageLabelNavigationResult result = GetImageLabelNavigationResult(topic, navigationParameter, ENavigationDirection.Blank);
                if (result == null)
                {
                    // if no next blank image found then stay on current image
                    return Navigate(topic, navigationParameter, ENavigationDirection.Direct);
                }
                return Ok(result);
            });
        }

        #endregion

        #region private helper

        private ActionResult Navigate(Topic topic, NavigationParameter navigationParameter, ENavigationDirection navigationDirection)
        {
            #region validation

            CheckMandatoryParameter(nameof(navigationParameter), navigationParameter);

            #endregion

            // create naviation result
            ImageLabelNavigationResult result = GetImageLabelNavigationResult(topic, navigationParameter, navigationDirection);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }

        private ImageLabelNavigationResult GetImageLabelNavigationResult(Topic topic, NavigationParameter navigationParameter, ENavigationDirection navigationDirection)
        {
            // get list of all image labels
            List<ImageLabel> imageLabels = this.ImageService.GetImageLabels(topic).ToList();

            // fill imagelabel-structure of image label
            ImageLabel navigationImageLabel = imageLabels.Navigate(navigationDirection, navigationParameter.Index);

            if (navigationImageLabel == null)
            {
                // return http-404 if no label found
                return null;
            }

            // create naviation result
            return new ImageLabelNavigationResult
            {
                ImageLabel = this.ImageService.Navigate(topic, (uint)navigationImageLabel.Index),
                ImageCount = imageLabels.Count,
                HasNextBlank = imageLabels.HasNextBlank(navigationImageLabel),
                HasNext = imageLabels.HasNext(navigationImageLabel),
                HasPrevious = imageLabels.HasPrevious(navigationImageLabel),
                HasPreviousBlank = imageLabels.HasPreviousBlank(navigationImageLabel)
            };
        }

        #endregion
    }
}