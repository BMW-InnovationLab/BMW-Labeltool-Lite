using Microsoft.AspNetCore.Mvc;
using Rcv.LabelTool.Backend.Statics;
using Rcv.LabelTool.Backend.Util;
using Rcv.LabelTool.Contracts.Services;
using System;

namespace Rcv.LabelTool.Backend.Controllers
{
    /// <summary>
    /// Controller for simple image operations.
    /// </summary>
    [Route("api/images")]
    public class ImagesController : BaseTopicController
    {
        #region member

        /// <summary>
        /// Image service to work with.
        /// </summary>
        private IImageService ImageService { get; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new controller instance.
        /// </summary>
        /// <param name="imageService">Image service</param>
        /// <param name="topicService">Topic service</param>
        public ImagesController(IImageService imageService, ITopicService topicService) : base(topicService)
        {
            ImageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        }

        #endregion

        #region ImagesController

        /// <summary>
        /// Get image from topic with given name.
        /// </summary>
        /// <param name="topicId">Id of topic where image is stored</param>
        /// <param name="imageId">Name of image</param>
        /// <returns>Image from topic</returns>
        /// <response code="200">Requested image</response>
        /// <response code="400">Image file name must be provided</response>
        /// <response code="404">The topic or image cannot be found</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpGet("v2/{topicId}/{imageId}")]
        [ResponseCache(CacheProfileName = CacheProfileNames.Never)]
        public ActionResult GetImage(long topicId, string imageId)
        {
            return Execute(topicId, (topic) =>
            {
                #region validation

                CheckMandatoryParameter(nameof(imageId), imageId);

                if (!ImageService.ExistsImage(topic, imageId))
                {
                    return NotFound(Messages.ImageNotFound(imageId));
                }

                #endregion

                return new FileStreamResult(ImageService.GetImage(topic, imageId), MimeMapping.MimeUtility.GetMimeMapping(imageId));
            });
        }

        /// <summary>
        /// Get number of images which are avaiable for topic with given topicId.
        /// </summary>
        /// <param name="topicId">Id of topic</param>
        /// <returns>Number of images in topic</returns>
        /// <response code="200">Number of images in topic</response>
        /// <response code="404">Topic with id not found</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpGet("{topicId}/imagecount")]
        public ActionResult<uint> ImageCount(long topicId)
        {
            return Execute(topicId, (topic) =>
            {
                return Ok(ImageService.ImageCount(topic));
            });
        }

        /// <summary>
        /// Delete image with given name from topic with given id.
        /// </summary>
        /// <param name="topicId">Id of topic where image is stored</param>
        /// <param name="imageId">Id of image to delete</param>
        /// <returns>Index of next item to show</returns>
        /// <response code="200">Index of next item to show</response>
        /// <response code="400">One or more parameters are not valid</response>
        /// <response code="404">Topic with id not found</response>
        /// <response code="500">Unexpected error occures</response>
        [HttpDelete("{topicId}/{imageId}/e")]
        public ActionResult<long> DeleteImage(long topicId, string imageId)
        {
            return Execute(topicId, (topic) =>
            {
                #region validation

                CheckMandatoryParameter(nameof(imageId), imageId);

                if (!ImageService.ExistsImage(topic, imageId))
                {
                    return NotFound(Messages.ImageNotFound(imageId));
                }

                #endregion

                // get index of image to delete
                long currentImageIndex = ImageService.GetImageLabel(topic, imageId).Index;

                ImageService.DeleteImage(topic, imageId);

                // get count of images after image has been deleted
                long imageCount = ImageService.ImageCount(topic);

                if (imageCount == 0)
                {
                    // all images have been deleted
                    return Ok(0);
                }
                else if (imageCount == currentImageIndex)
                {
                    // last image has been deleted
                    // navigate backwards
                    return Ok(currentImageIndex - 1);
                }

                // dataset includes still images and deleted images was not last one
                // from topic. Stay on current index
                return Ok(currentImageIndex);
            });
        }

        #endregion
    }
}