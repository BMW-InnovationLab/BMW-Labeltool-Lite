using Microsoft.AspNetCore.Mvc;
using Rcv.LabelTool.Backend.Util;
using Rcv.LabelTool.Contracts.Models;
using Rcv.LabelTool.Contracts.Services;
using System;

namespace Rcv.LabelTool.Backend.Controllers
{
    /// <summary>
    /// Controller for imagedata operation.
    /// </summary>
    [Route("api/imagedata")]
    public class ImageDataController : BaseTopicController
    {
        #region member

        private IImageDataService ImageDataService { get; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new controller instance.
        /// </summary>
        /// <param name="imageDataService">Image data service</param>
        /// <param name="topicService">Topic service</param>
        public ImageDataController(IImageDataService imageDataService, ITopicService topicService) : base(topicService)
        {
            ImageDataService = imageDataService ?? throw new ArgumentNullException(nameof(imageDataService));
        }

        #endregion

        /// <summary>
        /// Get ImageData with given imageId of topic with given topicId.
        /// </summary>
        /// <param name="topicId">Id of topic where image is stored</param>
        /// <param name="imageId">Id of image to add labels</param>
        /// <returns>ImageData</returns>
        /// <response code="200">ImageData of image withe given topicId and imageId</response>
        /// <response code="400">One or more parameters are not valid</response>
        /// <response code="404">Image with id not found</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpGet("{topicId}/{imageId}")]
        public ActionResult<ImageData> GetImageData(long topicId, string imageId)
        {
            return Execute(topicId, (topic) =>
            {
                #region validation

                CheckMandatoryParameter(nameof(imageId), imageId);

                if (!ImageDataService.ExistsImage(topic, imageId))
                {
                    return NotFound(Messages.ImageNotFound(imageId));
                }

                #endregion

                ImageData imageData = ImageDataService.GetImageData(topic, imageId);

                return Ok(imageData);
            });
        }
    }
}
