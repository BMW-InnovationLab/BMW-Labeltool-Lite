using Microsoft.AspNetCore.Mvc;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Services;
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

        private IImageService ImageService { get; set; }

        private ILabelService LabelService { get; set; }

        #endregion

        #region constructor

        public ImagesLabelController(IImageService imageService, ILabelService labelService, ITopicService topicService) : base(topicService)
        {
            #region validation

            if (imageService == null)
            {
                throw new ArgumentNullException(nameof(imageService));
            }

            if (labelService == null)
            {
                throw new ArgumentNullException(nameof(labelService));
            }

            #endregion

            ImageService = imageService;
            LabelService = labelService;
        }

        #endregion

        #region ImagesLabelController

        /// <summary>
        /// Update labels of image with given imageId of topic with given topicId.
        /// Labels will be overwritten by provided labels-list.
        /// </summary>
        /// <param name="topicId">Id of topic where image is stored</param>
        /// <param name="imageId">Id of image to add labels</param>
        /// <param name="labels">Labels which are related to image</param>
        /// <returns>List of saved labels</returns>
        /// <response code="200">List of labels which have been saved</response>
        /// <response code="400">One or more parameters are not valid</response>
        /// <response code="404">Topic with id not found</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpPut]
        [Route("{topicId}/{imageId}/labels")]
        public ActionResult<IEnumerable<Label>> SaveOrUpdateLabels(long topicId, string imageId, [FromBody] IEnumerable<Label> labels)
        {
            return Execute(topicId, (topic) =>
            {
                #region validation

                CheckMandatoryParameter(nameof(imageId), imageId);
                CheckMandatoryParameter(nameof(labels), labels);

                if (!ImageService.ExistsImage(topic, imageId))
                {
                    return NotFound();
                }

                #endregion

                ImageLabel imageToSave = ImageService.GetImageLabel(topic, imageId);
                imageToSave.Labels = labels.ToList();

                return Ok(LabelService.SaveLabels(topic, imageToSave));
            });
        }

        #endregion
    }
}