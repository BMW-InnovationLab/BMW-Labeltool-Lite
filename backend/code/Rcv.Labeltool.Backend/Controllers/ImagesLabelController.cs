using Microsoft.AspNetCore.Mvc;
using Rcv.LabelTool.Backend.Util;
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
    /// Controller for image label operation.
    /// </summary>
    [Route("api/images")]
    public class ImagesLabelController : BaseTopicController
    {
        #region member

        private IImageService ImageService { get; }

        private ILabelService LabelService { get; }

        private IObjectClassService ObjectClassService { get; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new controller instance.
        /// </summary>
        /// <param name="imageService">Image service</param>
        /// <param name="labelService">Label service</param>
        /// <param name="objectClassService">Objectclass service</param>
        /// <param name="topicService">Topic service</param>
        public ImagesLabelController(IImageService imageService, ILabelService labelService, IObjectClassService objectClassService, ITopicService topicService) : base(topicService)
        {
            ImageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            LabelService = labelService ?? throw new ArgumentNullException(nameof(labelService));
            ObjectClassService = objectClassService ?? throw new ArgumentNullException(nameof(objectClassService));
        }

        #endregion

        #region ImagesLabelController

        /// <summary>
        /// Update labels of image with given imageId of topic with given topicId.
        /// Labels will be overwritten by provided labels-list.
        /// </summary>
        /// <param name="topicId">Id of topic where image is stored</param>
        /// <param name="imageId">Id of image to add labels</param>
        /// <param name="labelViews">Labels which are related to image</param>
        /// <returns>List of saved labels</returns>
        /// <response code="200">List of labels which have been saved</response>
        /// <response code="400">One or more parameters are not valid</response>
        /// <response code="404">Topic with id not found</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpPut("{topicId}/{imageId}/labels")]
        public ActionResult<IEnumerable<LabelView>> SaveOrUpdateLabels(long topicId, string imageId, [FromBody] IEnumerable<LabelView> labelViews)
        {
            return Execute(topicId, (topic) =>
            {
                #region validation

                CheckMandatoryParameter(nameof(imageId), imageId);
                CheckMandatoryParameter(nameof(labelViews), labelViews);

                if (!ImageService.ExistsImage(topic, imageId))
                {
                    return NotFound(Messages.ImageNotFound(imageId));
                }

                // check referenced objectclass ids
                var objectClasses = ObjectClassService.GetObjectClasses(topic);
                foreach (var labelView in labelViews)
                {
                    if (!objectClasses.Any(o => o.Id == labelView.ObjectClassId))
                    {
                        return NotFound(Messages.ObjectClassNotFound(labelView.ObjectClassId));
                    }
                }

                ImageLabel image = ImageService.GetImageLabel(topic, imageId);
                if (labelViews.Any(o => o.Right > image.Width || o.Bottom > image.Height))
                {
                    return BadRequest(Messages.LabelCoordinatesOutsideOfImage(image.Width, image.Height));
                }

                #endregion

                // convert labelview to labels
                List<Label> labels = new List<Label>();
                foreach (LabelView labelView in labelViews)
                {
                    var label = LabelFactory.Create(labelView);
                    label.ObjectClass = objectClasses.Single(o => o.Id == label.ObjectClassId);
                    labels.Add(label);
                }

                // save labels                
                IEnumerable<Label> savedLabels = LabelService.SaveLabels(topic, image, labels);

                // convert result to labelviews
                List<LabelView> savedLabelViews = new List<LabelView>();
                foreach (Label label in savedLabels)
                {
                    savedLabelViews.Add(LabelFactory.CreateView(label));
                }

                return Ok(savedLabelViews);
            });
        }

        #endregion
    }
}