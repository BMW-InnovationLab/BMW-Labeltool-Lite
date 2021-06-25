using Microsoft.AspNetCore.Mvc;
using Rcv.LabelTool.Backend.Util;
using Rcv.LabelTool.Contracts.Enumerations;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Models.View;
using Rcv.LabelTool.Contracts.Parameters;
using Rcv.LabelTool.Contracts.Results;
using Rcv.LabelTool.Contracts.Services;
using Rcv.LabelTool.Logic.Factories;
using Serilog;
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

        private ILabelService LabelService { get; }

        private IImageNavigationService NavigationService { get; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new controller instance.
        /// </summary>
        /// <param name="topicService">Topic service</param>
        /// <param name="navigationService">Navigation service</param>
        /// <param name="labelService">Label service</param>
        public NavigationController(ITopicService topicService, IImageNavigationService navigationService, ILabelService labelService) : base(topicService)
        {
            LabelService = labelService ?? throw new ArgumentNullException(nameof(labelService));
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
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
        [HttpGet("{topicId}")]
        public ActionResult<ImageNavigationResultView> Navigate(long topicId, [FromQuery(Name = "")] NavigationParameter navigationParameter)
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
        [HttpGet("{topicId}/previousblank")]
        public ActionResult<ImageNavigationResultView> PreviousBlank(long topicId, [FromQuery(Name = "")] NavigationParameter navigationParameter)
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
        [HttpGet("{topicId}/previous")]
        public ActionResult<ImageNavigationResultView> Previous(long topicId, [FromQuery(Name = "")] NavigationParameter navigationParameter)
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
        [HttpGet("{topicId}/next")]
        public ActionResult<ImageNavigationResultView> Next(long topicId, [FromQuery(Name = "")] NavigationParameter navigationParameter)
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
        [HttpGet("{topicId}/nextblank")]
        public ActionResult<ImageNavigationResultView> NextBlank(long topicId, [FromQuery(Name = "")] NavigationParameter navigationParameter)
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
        [HttpGet("{topicId}/lastblank")]
        public ActionResult<ImageNavigationResultView> LastBlank(long topicId, [FromQuery(Name = "")] NavigationParameter navigationParameter)
        {
            return Execute(topicId, (topic) =>
            {
                if (navigationParameter != null)
                {
                    navigationParameter.Index = 0;
                }

                return Navigate(topic, navigationParameter, ENavigationDirection.LastBlank);
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
        [HttpGet("{topicId}/blank")]
        public ActionResult<ImageNavigationResultView> Blank(long topicId, [FromQuery(Name = "")] NavigationParameter navigationParameter)
        {
            return Execute(topicId, (topic) =>
            {
                return Navigate(topic, navigationParameter, ENavigationDirection.Blank);
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
            ImageNavigationResultView result = GetImageLabelNavigationResult(topic, navigationParameter, navigationDirection);
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound(Messages.ImageNotFound((uint)navigationParameter.Index));
        }

        private ImageNavigationResultView GetImageLabelNavigationResult(Topic topic, NavigationParameter navigationParameter, ENavigationDirection navigationDirection)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            // fill imagelabel-structure of image label            
            ImageNavigationResult image = NavigationService.Navigate(topic, navigationParameter.Index, navigationParameter.LabelMode, navigationDirection);


            // default navigation if not label was found on navigation to blank
            if (image == null && (navigationDirection == ENavigationDirection.Blank || navigationDirection == ENavigationDirection.LastBlank))
            {
                image = NavigationService.Navigate(topic, navigationParameter.Index, navigationParameter.LabelMode, ENavigationDirection.Direct);
            }

            if (image == null)
            {
                // return http-404 if no label found
                return null;
            }

            // create naviation result
            ImageNavigationResultView navigationResult = ImageNavigationResultFactory.CreateView(image);

            switch (navigationParameter.LabelMode)
            {
                case ELabelMode.ObjectDetection:
                    if (image.ImageLabel.HasLabels)
                    {
                        IEnumerable<Label> labels = LabelService.GetLabels(topic, image.ImageLabel);
                        navigationResult.ImageLabel.Labels = labels.Select(LabelFactory.CreateView);
                    }
                    break;
            }

            sw.Stop();
            Log.Information($"Navigation takes {sw.ElapsedMilliseconds} ms");

            return navigationResult;
        }

        #endregion
    }
}